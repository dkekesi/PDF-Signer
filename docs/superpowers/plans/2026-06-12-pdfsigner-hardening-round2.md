# PDF Signer Hardening Round 2 Implementation Plan (A1–A5, B1, B3–B10)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the round-2 addendum of `docs/superpowers/specs/2026-06-12-pdfsigner-security-hardening-design.md`: deselected audit items A1–A5 and reviewer follow-ups B1, B3–B10.

**Architecture:** All changes inside the existing solution; no new files. Verification = solution builds + the Encrypt PowerShell harness (legacy ciphertext reconstructed manually). MQFTP changes (A2–A4) cannot be runtime-verified here — user smoke test required.

**Tech Stack:** VB.NET / net48, SecureBlackbox, WCF, NLog, MSBuild VS18.

---

## Conventions for every task

**BUILD** (PowerShell):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "C:\Projects\PDF Signer\PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false /v:m /nologo
```

Expected: zero errors; pre-existing MSB3270 warnings acceptable; no BC42328/BC42353.

**Git:** `git add` ONLY the files named per task (never `-A`/`.`); commit footer:

```
Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>
```

Operator-visible strings Hungarian; internal comments English. Paths relative to `C:\Projects\PDF Signer\`.

---

### Task 1: A5 + B5 — `Encrypt.vb` cleanup

**Files:** Modify `PDF Signer for Tungsten Capture Common\Encrypt.vb` (read it first; it has the v2 random-IV format from round 1).

- [ ] **Step 1: A5 — remove dead code from `RSAVerifySignedString`.** Delete these four lines from the method (behavior unchanged — `VerifyData` does its own hashing):

```vb
            'Don't do this, do the same as you did in SignData:
            'byte[] bytesToVerify = Convert.FromBase64String(originalMessage);
```
and
```vb
                Dim Hash = New SHA256Managed()
                Dim hashedData As Byte() = Hash.ComputeHash(signedBytes)
```

- [ ] **Step 2: B5 — constants + factory.** Under the existing constants add:

```vb
    Private Const IVSizeBytes As Integer = 16 ' AES block size; also the length of the v2 prepended IV
```

Add a private factory (place before `AES256Encrypt`):

```vb
    Private Function CreateAes(IV As Byte()) As AesCryptoServiceProvider
        Return New AesCryptoServiceProvider() With {
            .BlockSize = 128,
            .KeySize = 256,
            .IV = IV,
            .Key = Encoding.UTF8.GetBytes(AesKey256),
            .Mode = CipherMode.CBC,
            .Padding = PaddingMode.PKCS7
        }
    End Function
```

- [ ] **Step 3: B5 — use the factory and constant in all three AES methods.**
  - `AES256Encrypt`: `Dim iv(IVSizeBytes - 1) As Byte`; `Using aes256 As AesCryptoServiceProvider = CreateAes(iv)` (drop the inline initializer).
  - `AES256DecryptV2`: replace the guard `If payload.Length <= 16 Then Return Nothing` with
    ```vb
            If payload.Length < IVSizeBytes * 2 OrElse (payload.Length - IVSizeBytes) Mod IVSizeBytes <> 0 Then Return Nothing
    ```
    `Dim iv(IVSizeBytes - 1) As Byte`; `Buffer.BlockCopy(payload, 0, iv, 0, IVSizeBytes)`; `Using aes256 As AesCryptoServiceProvider = CreateAes(iv)`; `TransformFinalBlock(payload, IVSizeBytes, payload.Length - IVSizeBytes)`.
  - `AES256DecryptLegacy`: restructure so the WHOLE body (including `Convert.FromBase64String`) is inside `Try ... Catch Return Nothing`, using the factory:
    ```vb
    Private Function AES256DecryptLegacy(StringToDecrypt As String) As String
        Try
            Dim src As Byte() = Convert.FromBase64String(StringToDecrypt)

            Using aes256 As AesCryptoServiceProvider = CreateAes(Encoding.UTF8.GetBytes(AesIV256))
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(src, 0, src.Length)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function
    ```
    (Intentional behavior change per spec: malformed Base64 now returns `Nothing` instead of throwing — improves the license-load failure mode.)

- [ ] **Step 4: BUILD; zero errors.**

- [ ] **Step 5: Harness verification** (fresh PowerShell; note single-quoted strings — the key/IV contain `$`):

```powershell
[Reflection.Assembly]::LoadFrom("C:\Projects\PDF Signer\PDF Signer for Tungsten Capture Common\bin\Debug\PDFSignerCommon.dll") | Out-Null
$enc = New-Object PDFSignerCommon.Encrypt
$p = 'Próba_jelszó123'
$c1 = $enc.AES256Encrypt($p)
if (-not $c1.StartsWith('v2:')) { throw 'v2 prefix missing' }
if ($enc.AES256Decrypt($c1) -ne $p) { throw 'v2 round-trip failed' }
# reconstruct a LEGACY ciphertext manually (static IV) and verify the legacy path
$aes = [System.Security.Cryptography.AesCryptoServiceProvider]::new()
$aes.BlockSize = 128; $aes.KeySize = 256
$aes.IV  = [Text.Encoding]::UTF8.GetBytes('uB$xu!yCE|2k}1M2')
$aes.Key = [Text.Encoding]::UTF8.GetBytes('Ms@fr8U1#BUiVZ^*}MgR8E{gDekTctHM')
$aes.Mode = 'CBC'; $aes.Padding = 'PKCS7'
$src = [Text.Encoding]::Unicode.GetBytes($p)
$legacy = [Convert]::ToBase64String($aes.CreateEncryptor().TransformFinalBlock($src, 0, $src.Length))
if ($enc.AES256Decrypt($legacy) -ne $p) { throw 'legacy decrypt failed' }
if ($null -ne $enc.AES256Decrypt('***not-base64***')) { throw 'malformed legacy input should return Nothing' }
if ($null -ne $enc.AES256Decrypt('v2:' + [Convert]::ToBase64String([byte[]](1..20)))) { throw 'short v2 payload should return Nothing' }
'ALL ENCRYPT CHECKS PASSED'
```

- [ ] **Step 6: Commit** — `git add "PDF Signer for Tungsten Capture Common/Encrypt.vb"`, message `Remove dead hash code and deduplicate AES configuration`.

---

### Task 2: A1 — refuse Basic auth over plain HTTP (MNB)

**Files:**
- Modify `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb`
- Modify `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\MNBSignerCryptoProvider.vb`
- Modify the Common project's `Messages` resources (base resx + `.hu.resx` + the checked-in `Messages.Designer.vb`) — locate them by finding where `MNBSigner_URL_Missing` is defined (`git grep -n "MNBSigner_URL_Missing" -- "*.resx" "*.Designer.vb"`).

- [ ] **Step 1: Runtime guard.** In `MNBSigner.SignDocument`, immediately before `Using clt As New MNBSignerServiceClient(...)`, insert:

```vb
        ' Basic credentials must never travel over plain HTTP
        If _settings.IsMNBSignerUserPasswordAuthentication AndAlso _settings.MNBSignerURL.Scheme <> Uri.UriSchemeHttps Then
            Dim httpRes As New SignatureResult With {
                .ErrorMessage = "Felhasználónév/jelszó alapú (Basic) hitelesítés csak HTTPS kapcsolaton engedélyezett! Állítsa át az MNBSigner URL-t HTTPS-re a dokumentumosztály beállításaiban."
            }
            httpRes.SignatureLog = httpRes.ErrorMessage
            Return httpRes
        End If
```

- [ ] **Step 2: Setup-time validation.** In `MNBSignerCryptoProvider.Validate()`, after the existing scheme check (`MNBSigner_URL_Not_Http` return), add:

```vb
            If IsMNBSignerUserPasswordAuthentication AndAlso MNBSignerURL.Scheme.ToLower <> "https" Then
                Return Messages.MNBSigner_Basic_Auth_Requires_Https
            End If
```

- [ ] **Step 3: Resource entry.** Add key `MNBSigner_Basic_Auth_Requires_Https` to the base resx AND the `.hu.resx` next to the existing `MNBSigner_*` entries (match the language of neighboring base-resx values — inspect `MNBSigner_URL_Missing`'s value first), value (Hungarian):

```
Felhasználónév/jelszó alapú (Basic) hitelesítés csak HTTPS URL-lel engedélyezett az MNBSigner szolgáltatónál!
```

Also add the matching generated property to the checked-in `Messages.Designer.vb`, copying the exact pattern of the `MNBSigner_URL_Missing` property (the designer file does not regenerate under plain MSBuild).

- [ ] **Step 4: BUILD; zero errors.**

- [ ] **Step 5: Commit** — `git add` the four touched files; message `Refuse MNB Basic authentication over plain HTTP`.

---

### Task 3: A2 + A3 + A4 — MQFTP exchange fixes

**Files:**
- Modify `PDF Signer for Tungsten Capture\Signature\Modules\MQFTP\MQFTP.vb`
- Modify `PDF Signer for Tungsten Capture\FileOperation\FileOperation.vb`

- [ ] **Step 1: A2 — share-mode parameter.** In `FileOperation.ReadFileStreamWhenAvailable`, change the signature to

```vb
    Friend Function ReadFileStreamWhenAvailable(FileNameWithPath As String, Optional ShareMode As FileShare = FileShare.Read) As Stream
```

and replace `Dim strm As FileStream = File.OpenRead(FileNameWithPath)` with

```vb
                Dim strm As New FileStream(FileNameWithPath, FileMode.Open, FileAccess.Read, ShareMode)
```

- [ ] **Step 2: A3 — arm the watcher before the request leaves staging; A2 — open with delete sharing.** In `MQFTP.SignDocument`: replace the declaration `Dim pdfWatchRes As WaitForChangedResult` at the top with `Dim responseArrived As Boolean`. Then restructure the send/wait/read region (currently: staging write `Using fs` → `MoveWhenAvailable` → log → `Using pdfWatcher ... WaitForChanged` → `If Not pdfWatchRes.TimedOut`) to:

```vb
            ' write file into output MQ exchange folder
            _sbSignLog.AppendLine("Aláírás nélküli PDF fájl küldése hitelesítésre")
            Using fs As New FileStream(pdfOutStagingPath, FileMode.Create, FileAccess.Write, FileShare.None)
                Request.FileToSign.Seek(0, SeekOrigin.Begin)
                Request.FileToSign.CopyTo(fs)
            End Using

            ' arm the watcher before the request leaves staging, so a fast response cannot be missed
            Using pdfWatcher As New FileSystemWatcher With {
                .Path = _settings.MQFolderIn,
                .Filter = pdfFileNameWithExtension,
                .EnableRaisingEvents = True,
                .IncludeSubdirectories = False
            }
                fileOp.MoveWhenAvailable(pdfOutStagingPath, pdfOutPath)

                _sbSignLog.AppendLine("Várakozás MTRACK válaszra")

                If File.Exists(pdfInPath) Then
                    responseArrived = True
                Else
                    responseArrived = Not pdfWatcher.WaitForChanged(WatcherChangeTypes.Changed Or WatcherChangeTypes.Created, timeOut).TimedOut
                End If
            End Using

            If responseArrived Then
                _sbSignLog.AppendLine("MTRACK válasz beérkezett, válasz beolvasása")

                ' open with FileShare.Delete so the exchange file can be deleted while we still hold the stream
                signedDoc = fileOp.ReadFileStreamWhenAvailable(pdfInPath, FileShare.Read Or FileShare.Delete)

                If signedDoc IsNot Nothing Then
                    _sbSignLog.AppendLine("MTRACK válasz beolvasva")
                    fileOp.DeleteWhenAvailable(pdfInPath)
                Else
                    res.ErrorMessage = "Időtúllépés: az MTRACK felől beérkezett hitelesített dokumentumot nem sikerült beolvasni a fájlrendszerből a határidőn (10 mp) belül, mert egy másik folyamat lock-olta!"
                    res.SignatureLog = _sbSignLog.ToString
                    Return res
                End If
            End If
        End Using
```

and change the post-impersonation timeout check from `If pdfWatchRes.TimedOut Then` to `If Not responseArrived Then` (body unchanged).

- [ ] **Step 3: A4 — logon type.** In `ImpersonateIdentity`, replace

```vb
        Dim success = NativeMethods.LogonUser(UserName, DomainName, Password, CInt(NativeMethods.LogonType.LOGON32_LOGON_INTERACTIVE), CInt(NativeMethods.LogonProvider.LOGON32_PROVIDER_DEFAULT), userToken)
```

with

```vb
        ' NEW_CREDENTIALS: the supplied credentials apply to outbound network access only (share access)
        Dim success = NativeMethods.LogonUser(UserName, DomainName, Password, CInt(NativeMethods.LogonType.LOGON32_LOGON_NEW_CREDENTIALS), CInt(NativeMethods.LogonProvider.LOGON32_PROVIDER_WINNT50), userToken)
```

- [ ] **Step 4: BUILD; zero errors.**

- [ ] **Step 5: Commit** — both files; message `Fix MQFTP response race, delete-while-open and logon type`.

---

### Task 4: B6 — MNB stream ownership

**Files:** Modify `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb`.

- [ ] **Step 1: `SignSinglePass` short-read guard.** After the fill loop (`While offset < PdfBin.Length ... End While`), insert:

```vb
        If offset < PdfBin.Length Then
            Throw New EndOfStreamException($"A dokumentum stream a vártnál rövidebb ({offset}/{PdfBin.Length} byte)!")
        End If
```

(`System.IO` is already imported; the exception surfaces via `SignatureOperation`'s catch-all.)

- [ ] **Step 2: `SignMultiPass` ownership-flag Finally.** Remove ALL six `mtSigned.Dispose()` lines (added in round 1). Move the two trailing statements (`res.SignatureLog = _sbSignLog.ToString` and `res.SignedFile = mtSigned`) from after `End Try` to the end of the `Try` body, followed by `Return res`. Add a `Finally`:

```vb
        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = ex.ToString
            Return res

        Finally
            ' ownership transfers to the caller only when SignedFile was assigned on the success path
            If res.SignedFile IsNot mtSigned Then mtSigned.Dispose()
        End Try
    End Function
```

(after this there must be NO code between `End Try` and `End Function`).

- [ ] **Step 3: BUILD; zero errors. Commit** — message `Consolidate MNB stream disposal into ownership-flag Finally`.

---

### Task 5: B8 — explicit enum return + `-B` warning

**Files:** Modify `PDF Signer for Tungsten Capture\Parser\BatchParser.vb`, `PDF Signer for Tungsten Capture\FrmPdfSigner.vb`.

- [ ] **Step 1:** In `GetBatchTreeItemIcon`, replace the trailing `Return Nothing` with:

```vb
        ' unreachable: all TreeItem kinds are handled above; explicit value (enum zero) for clarity
        Return IconType.Batch
```

- [ ] **Step 2:** In `FrmPdfSigner.New`, add an `Else` branch to the TryParse:

```vb
            Dim batchId As Integer
            If Integer.TryParse(args(0).Substring(2), batchId) Then ' we cut off "-B" from the beginning
                _SingleBatchOpenID = batchId
            Else
                _logger.Warn("Érvénytelen -B kötegazonosító a parancssorban: {0}", args(0))
            End If
```

(`_logger` is a field initializer, so it is already assigned when the constructor body runs.)

- [ ] **Step 3: BUILD; zero errors. Commit** — both files; message `Explicit icon fallback and warning on malformed -B argument`.

---

### Task 6: B1 + B7 — FrmPdfSigner guards

**Files:** Modify `PDF Signer for Tungsten Capture\FrmPdfSigner.vb`.

- [ ] **Step 1: B1 — reentrancy guard.** Add a field next to `_RefreshingProviders`:

```vb
    Private _DocumentActionRunning As Boolean
```

Wrap the ENTIRE body of each of the three handlers `BtnSign_ItemClick`, `BtnSkipSign_ItemClick`, `BtnReject_ItemClick` (all `Async Sub`) in:

```vb
        If _DocumentActionRunning Then Return
        _DocumentActionRunning = True
        Try
            ' ... existing body unchanged ...
        Finally
            _DocumentActionRunning = False
        End Try
```

(Existing body indented one level; `Try/Finally` across `Await` is valid. Existing early `Return`s inside the body are fine — `Finally` still resets the flag.)

- [ ] **Step 2: B7 — refresh leftovers.** Replace the body of `RefreshCryptoProviders` with:

```vb
    Private Sub RefreshCryptoProviders(docitem As DocumentItem)
        Dim activeProviders As List(Of CryptoProviderBase) = docitem.SetupData.CryptographicProviders.Where(Function(x) x.Enabled).ToList

        _RefreshingProviders = True
        Try
            ComboProvider.Items.Clear()

            For Each provider As CryptoProviderBase In activeProviders
                ComboProvider.Items.Add(provider)

                If provider.ProviderType = docitem.SetupData.DefaultCryptographicProvider Then
                    BarComboProvider.EditValue = provider
                End If
            Next

            ' reset stale selection when the previous document's provider is not offered here
            Dim currentSelection As CryptoProviderBase = TryCast(BarComboProvider.EditValue, CryptoProviderBase)
            If currentSelection Is Nothing OrElse Not activeProviders.Contains(currentSelection) Then
                BarComboProvider.EditValue = activeProviders.FirstOrDefault
            End If
        Finally
            _RefreshingProviders = False
        End Try

        ' enumerate the certificate store only when the selected provider can actually use local certificates
        Dim selectedProvider As CryptoProviderBase = TryCast(BarComboProvider.EditValue, CryptoProviderBase)
        If selectedProvider IsNot Nothing AndAlso selectedProvider.SupportsLocalCertificates Then
            BtnRefreshCertificates_ItemClick(Nothing, Nothing)
        End If
    End Sub
```

- [ ] **Step 3: BUILD; zero errors. Commit** — message `Guard document actions against reentry and skip needless cert refresh`.

---

### Task 7: B3 + B9 — `ServerCertificateValidator` hardening + polish

**Files:** Modify `PDF Signer for Tungsten Capture\Signature\Modules\SBB\ServerCertificateValidator.vb`.

- [ ] **Step 1: B3 — wildcard base must contain a dot.** In `HostNameMatches`, replace the wildcard branch with:

```vb
        ' wildcard: "*.example.com" matches exactly one leading label
        If Pattern.StartsWith("*.", StringComparison.Ordinal) Then
            Dim base As String = Pattern.Substring(2)
            If base.IndexOf("."c) < 0 Then Return False ' reject single-label bases like "*.com"

            Dim dotIndex As Integer = Host.IndexOf("."c)
            If dotIndex > 0 Then
                Return String.Equals(base, Host.Substring(dotIndex + 1), StringComparison.OrdinalIgnoreCase)
            End If
        End If
```

- [ ] **Step 2: B3 — CN fallback only without a SAN extension.** Change `GetSubjectAlternativeDnsNames` to report extension presence:

```vb
    Private Shared Function GetSubjectAlternativeDnsNames(Certificate As X509Certificate2, ByRef HasSanExtension As Boolean) As List(Of String)
        Dim res As New List(Of String)
        HasSanExtension = False

        For Each ext As X509Extension In Certificate.Extensions
            If ext.Oid Is Nothing OrElse ext.Oid.Value <> "2.5.29.17" Then Continue For
            HasSanExtension = True
            ' ... rest of the existing body unchanged ...
```

and in `MatchesHost`:

```vb
        Dim hasSan As Boolean
        Dim names As List(Of String) = GetSubjectAlternativeDnsNames(Certificate, hasSan)

        If Not hasSan Then
            ' RFC 6125: fall back to the subject CN only when the certificate has no SAN extension at all
            Dim cn As String = Certificate.GetNameInfo(X509NameType.DnsName, False)
            If Not String.IsNullOrEmpty(cn) Then names.Add(cn)
        End If
```

- [ ] **Step 3: B9 — docs/comments.** Translate the two Hungarian class-level XML summaries to English (methods are already English). Add at the inner `ReadTagAndLength` call site: `' the tag byte was already read into "tag"; the call only validates and consumes the header`. In the long-form length branch add: `' high bit set: the low 7 bits give the count of following big-endian length bytes`.

- [ ] **Step 4: B9 — `Reason` flag.** Check via PowerShell reflection on `lib\nsoftware.SecureBlackbox.dll` whether a validity-reason flags enum/constant set exists (search type names matching `Reason` in `SBX509`/`SBCertValidator`, e.g. `TSBCertificateValidityReason` with an `UnknownCA`-like member). If found, in `HostBoundCertificateValidator.OnCertificateValidate`'s failure branch set `Reason` accordingly (e.g. `Reason = Reason Or CInt(<enum>.<member>)`); if nothing suitable exists, add the comment `' SBB exposes no suitable reason flag here; the failure cause is in the signature log` instead. Report which.

- [ ] **Step 5: BUILD; zero errors. Commit** — message `Tighten hostname matching and document the DER parser`.

---

### Task 8: B4 + B10 — SBBPDF lifecycle

**Files:** Modify `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb`, `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb`.

- [ ] **Step 1: B4 — NLog + logging SafeDispose.** Add `Imports NLog` and a field `Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()` to `SBBPDF`. In `SafeDispose`, replace the bare `Catch` with:

```vb
        Catch ex As Exception
            _logger.Debug(ex, "SBB objektum felszabadítása sikertelen")
        End Try
```

- [ ] **Step 2: B4 — IDisposable.** `SBBPDF` gets `Implements IDisposable` and (placed after `SafeDispose`):

```vb
    ''' <summary>
    ''' SBBPDF instances are single-use: disposing releases the HTTP/TSP clients,
    ''' so a disposed instance must not sign again. Callers wrap usages in Using.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        SafeDispose(HTTPSClient)
        SafeDispose(TSPClient)
    End Sub
```

Remove the `SafeDispose(HTTPSClient)` and `SafeDispose(TSPClient)` lines from `SignDocument`'s `Finally` (the other four `SafeDispose` calls there stay).

- [ ] **Step 3: B4 — `Using` at all three call sites in `SignatureOperation.vb`:**

```vb
            If TypeOf sigReq.SignSettings Is PDFSignerCryptoProvider Then
                Using sbb As New SBBPDF
                    sbb.Initialize(sigReq.SignSettings)
                    sigRes = sbb.SignDocument(sigReq)
                End Using
            End If
```

```vb
    Friend Function GetCertificatesFromMyWinCertStore(QualifiedCertificatesOnly As Boolean) As List(Of SigningCertificate)
        Using sbb As New SBBPDF
            Return sbb.GetCertificatesFromStore(QualifiedCertificatesOnly)
        End Using
    End Function
```

```vb
    Friend Sub ActivateSBBLicense()
        Using e As New SBBPDF
            e.ActivateLicense()
        End Using
    End Sub
```

- [ ] **Step 4: B10 — handlers attach inside the `Try`.** In `SignDocument`, the block of `AddHandler` lines (HTTPSClient ×2 incl. the `_tsaHostValidator` creation, PADES handlers ×4, TSPClient ×4) currently sits BEFORE `Try`. Move that whole block to immediately AFTER the `Try` line (before the first `_sbSignLog.AppendLine`). Nothing else reorders; the `Finally`'s `RemoveHandler`s are no-ops for handlers never attached.

- [ ] **Step 5: BUILD; zero errors, zero BC42328. Commit** — both files; message `Make SBBPDF disposable and attach handlers inside the guarded block`.

---

### Task 9: Final verification

- [ ] **Step 1:** FullRelease|x86 build with log:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "C:\Projects\PDF Signer\PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=FullRelease /p:Platform=x86 /p:CopyToCaptureBin=false /flp:LogFile=build-fullrelease.log /v:q /nologo
Select-String -Path build-fullrelease.log -Pattern " 0 Error\(s\)"
Select-String -Path build-fullrelease.log -Pattern "BC42328|BC42353"   # expect none
Remove-Item build-fullrelease.log
```

- [ ] **Step 2:** `git log --oneline` shows the 8 task commits; `git status --short` shows nothing unexpected.

- [ ] **Step 3:** Relay to the user: MQFTP smoke test now required (A2/A3/A4 — full exchange round-trip incl. credential access to the share), plus the round-1 smoke tests if not yet done.
