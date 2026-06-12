# PDF Signer Security/Memory/Performance Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the 11 approved audit fixes (S1, S2, S3, M1, M2, M3, P1, P2+D3, P3, P5, D1-cheap) from `docs/superpowers/specs/2026-06-12-pdfsigner-security-hardening-design.md`.

**Architecture:** All changes are inside the existing VB.NET solution (`PDF Signer for Tungsten Capture.slnx`, .NET Framework 4.8, SDK-style projects). One new file (`ServerCertificateValidator.vb`); everything else modifies existing files. No test infrastructure exists — each task is verified by a solution build, plus a PowerShell reflection harness for the `Encrypt` rework.

**Tech Stack:** VB.NET / WinForms, SecureBlackbox (SBB), WCF, MSBuild from VS 18.

---

## Conventions for every task

**Build command** (referred to as *BUILD* below):

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "C:\Projects\PDF Signer\PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false /v:m /nologo
```

Expected: output ends without any `error` lines ("Build succeeded" semantics; exit code 0). Pre-existing warnings other than BC42353 are acceptable.

**Git:** The working tree contains unrelated pending changes (SDK migration). `git add` ONLY the files named in each commit step — never `git add -A` or `git add .`. Commit messages end with:

```
Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>
```

**Language:** Log messages and operator-visible strings are Hungarian; code comments follow the surrounding file's language (mostly English).

All paths below are relative to `C:\Projects\PDF Signer\`.

---

### Task 1: D1 cheap half — explicit returns + safe `-B` parsing

**Files:**
- Modify: `PDF Signer for Tungsten Capture\FrmPdfSigner.vb` (ctor ~line 114, `SigningTasksRunning` ~line 341, `SelectNextSignableDocument` ~line 1125)
- Modify: `PDF Signer for Tungsten Capture\Parser\BatchParser.vb` (`GetBatchTreeItemIcon` ~line 186)

- [ ] **Step 1: Fix the `-B` argument parsing in `FrmPdfSigner.New`**

Replace:

```vb
        If args IsNot Nothing AndAlso args.Count > 0 AndAlso args(0).StartsWith("-B") Then
            _SingleBatchOpenID = args(0).Substring(2) ' we cut off "-B" from the beginning
        End If
```

with:

```vb
        If args IsNot Nothing AndAlso args.Count > 0 AndAlso args(0).StartsWith("-B") Then
            Dim batchId As Integer
            If Integer.TryParse(args(0).Substring(2), batchId) Then ' we cut off "-B" from the beginning
                _SingleBatchOpenID = batchId
            End If
        End If
```

(Invalid IDs leave `_SingleBatchOpenID` = 0 → the batch-open dialog appears, same as launching without arguments.)

- [ ] **Step 2: Add `Return False` to `SigningTasksRunning`**

Replace:

```vb
    Private Function SigningTasksRunning(ShowMessage As Boolean) As Boolean
        If _ActiveTasks IsNot Nothing AndAlso _ActiveTasks.Any(Function(f) Not f.IsCompleted) Then
            If ShowMessage Then MsgBox(Messages.Signature_Tasks_Running, MsgBoxStyle.Exclamation, Resources.MessageBoxTitle)
            Return True
        End If
    End Function
```

with:

```vb
    Private Function SigningTasksRunning(ShowMessage As Boolean) As Boolean
        If _ActiveTasks IsNot Nothing AndAlso _ActiveTasks.Any(Function(f) Not f.IsCompleted) Then
            If ShowMessage Then MsgBox(Messages.Signature_Tasks_Running, MsgBoxStyle.Exclamation, Resources.MessageBoxTitle)
            Return True
        End If

        Return False
    End Function
```

- [ ] **Step 3: Add `Return False` to `SelectNextSignableDocument`**

At the end of the function (currently ends with the `If AlwaysShowDocument Then ... End If` block followed by `End Function`), insert `Return False` between `End If` and `End Function`:

```vb
        ' if no signable document at all, then open first document, if requested
        If AlwaysShowDocument Then
            nextDocItem = batchCont.OfType(Of DocumentItem).FirstOrDefault
            If nextDocItem IsNot Nothing Then
                Dim firstNode = BatchTree.FindNodeByKeyID(nextDocItem.ID)
                BatchTree.FocusedNode = firstNode
                Return False ' we didn't find any document, just displayed the first one, hence False is returned
            End If
        End If

        Return False
    End Function
```

- [ ] **Step 4: Add `Return Nothing` to `GetBatchTreeItemIcon` in `BatchParser.vb`**

The function currently ends:

```vb
        If Item.GetType Is GetType(PageItem) Then
            Dim i As PageItem = CType(Item, PageItem)

            If i.IsRejected Then
                Return IconType.RejectedPage
            Else
                Return IconType.Page
            End If
        End If
    End Function
```

Insert `Return Nothing` before `End Function` (preserves the current implicit enum default for unknown item types):

```vb
        If Item.GetType Is GetType(PageItem) Then
            Dim i As PageItem = CType(Item, PageItem)

            If i.IsRejected Then
                Return IconType.RejectedPage
            Else
                Return IconType.Page
            End If
        End If

        Return Nothing
    End Function
```

- [ ] **Step 5: BUILD; verify zero errors AND zero BC42353 warnings** (previously three).

- [ ] **Step 6: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/FrmPdfSigner.vb" "PDF Signer for Tungsten Capture/Parser/BatchParser.vb"
git commit -m "Add explicit returns (BC42353) and safe -B argument parsing"
```

---

### Task 2: S3 — scrub SBB license-generator comment

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb:39-40`

- [ ] **Step 1: Delete the two comment lines**

Remove:

```vb
        ' to generate license key go to https://www.nsoftware.com/full/SBNJA
        ' And use this product key: XSBNJ-ADNXR-F2026-11151-YUF19-54BXW-CXM
```

The `SBUtils.Unit.SetLicenseKey(...)` line stays unchanged.

- [ ] **Step 2: BUILD** (or skip — comment-only change; build in next task covers it).

- [ ] **Step 3: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/SBB/SBBPDF.vb"
git commit -m "Remove SBB license generator reference from source"
```

---

### Task 3: S2 — versioned ciphertext with random IV in `Encrypt.vb`

**Files:**
- Modify: `PDF Signer for Tungsten Capture Common\Encrypt.vb:46-98`

- [ ] **Step 1: Capture a legacy ciphertext sample BEFORE changing the code**

BUILD first (so `PDFSignerCommon.dll` is current), then run in a fresh PowerShell:

```powershell
[Reflection.Assembly]::LoadFrom("C:\Projects\PDF Signer\PDF Signer for Tungsten Capture Common\bin\Debug\PDFSignerCommon.dll") | Out-Null
$enc = New-Object PDFSignerCommon.Encrypt
$cipher = $enc.AES256Encrypt("Próba_jelszó123")
$cipher | Set-Content "$env:TEMP\pdfsigner-legacy-cipher.txt"
"Legacy cipher: $cipher"
```

Expected: a Base64 string with NO `v2:` prefix, saved to `%TEMP%\pdfsigner-legacy-cipher.txt`. (If the type name `PDFSignerCommon.Encrypt` fails, check the root namespace with `[Reflection.Assembly]::LoadFrom(...).GetTypes() | Where-Object Name -eq 'Encrypt'` and use that full name here and in Step 4.)

- [ ] **Step 2: Rewrite the AES members of `Encrypt.vb`**

Keep `GetRSAKeyFromString` and `RSAVerifySignedString` untouched. Add the prefix constant under the existing key constants:

```vb
    Private Const AesIV256 As String = "uB$xu!yCE|2k}1M2"
    Private Const AesKey256 As String = "Ms@fr8U1#BUiVZ^*}MgR8E{gDekTctHM"
    Private Const CipherTextV2Prefix As String = "v2:"
```

Replace `AES256Encrypt` and `AES256Decrypt` entirely with:

```vb
    ''' <summary>
    ''' Encrypt string with AES256 (v2 format: random IV prepended to the ciphertext).
    ''' Use it to encrypt stored login passwords that application must have access to.
    ''' </summary>
    ''' <param name="StringToEncrypt">String to encrypt</param>
    ''' <returns>Encrypted string value ("v2:" + Base64(IV || ciphertext))</returns>
    Public Function AES256Encrypt(StringToEncrypt As String) As String
        If String.IsNullOrEmpty(StringToEncrypt) Then Return String.Empty

        Try
            Dim iv(15) As Byte
            Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()
                rng.GetBytes(iv)
            End Using

            Using aes256 As New AesCryptoServiceProvider() With {
                .BlockSize = 128,
                .KeySize = 256,
                .IV = iv,
                .Key = Encoding.UTF8.GetBytes(AesKey256),
                .Mode = CipherMode.CBC,
                .Padding = PaddingMode.PKCS7
            }
                Dim src As Byte() = Encoding.Unicode.GetBytes(StringToEncrypt)
                Using encrypt As ICryptoTransform = aes256.CreateEncryptor()
                    Dim cipherBytes As Byte() = encrypt.TransformFinalBlock(src, 0, src.Length)
                    Dim payload(iv.Length + cipherBytes.Length - 1) As Byte
                    Buffer.BlockCopy(iv, 0, payload, 0, iv.Length)
                    Buffer.BlockCopy(cipherBytes, 0, payload, iv.Length, cipherBytes.Length)
                    Return CipherTextV2Prefix + Convert.ToBase64String(payload)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Decrypt string with AES256. Accepts both the v2 format (random IV) and the
    ''' legacy static-IV format for backward compatibility with existing setup data
    ''' and license files.
    ''' </summary>
    ''' <param name="StringToDecrypt">String to decrypt</param>
    ''' <returns>Decrypted string value</returns>
    Public Function AES256Decrypt(StringToDecrypt As String) As String
        If String.IsNullOrEmpty(StringToDecrypt) Then Return String.Empty

        If StringToDecrypt.StartsWith(CipherTextV2Prefix, StringComparison.Ordinal) Then
            Return AES256DecryptV2(StringToDecrypt.Substring(CipherTextV2Prefix.Length))
        End If

        Return AES256DecryptLegacy(StringToDecrypt)
    End Function

    Private Function AES256DecryptV2(Base64Payload As String) As String
        Try
            Dim payload As Byte() = Convert.FromBase64String(Base64Payload)
            If payload.Length <= 16 Then Return Nothing

            Dim iv(15) As Byte
            Buffer.BlockCopy(payload, 0, iv, 0, 16)

            Using aes256 As New AesCryptoServiceProvider() With {
                .BlockSize = 128,
                .KeySize = 256,
                .IV = iv,
                .Key = Encoding.UTF8.GetBytes(AesKey256),
                .Mode = CipherMode.CBC,
                .Padding = PaddingMode.PKCS7
            }
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(payload, 16, payload.Length - 16)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    Private Function AES256DecryptLegacy(StringToDecrypt As String) As String
        Using aes256 As New AesCryptoServiceProvider() With {
            .BlockSize = 128,
            .KeySize = 256,
            .IV = Encoding.UTF8.GetBytes(AesIV256),
            .Key = Encoding.UTF8.GetBytes(AesKey256),
            .Mode = CipherMode.CBC,
            .Padding = PaddingMode.PKCS7
        }
            Dim src As Byte() = System.Convert.FromBase64String(StringToDecrypt)

            Try
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(src, 0, src.Length)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            Catch
                Return Nothing
            End Try
        End Using
    End Function
```

- [ ] **Step 3: BUILD; verify zero errors.**

- [ ] **Step 4: Verify round-trip + backward compatibility with the harness** (fresh PowerShell):

```powershell
[Reflection.Assembly]::LoadFrom("C:\Projects\PDF Signer\PDF Signer for Tungsten Capture Common\bin\Debug\PDFSignerCommon.dll") | Out-Null
$enc = New-Object PDFSignerCommon.Encrypt

# 1) v2 round-trip
$c1 = $enc.AES256Encrypt("Próba_jelszó123")
if (-not $c1.StartsWith("v2:")) { throw "v2 prefix missing: $c1" }
if ($enc.AES256Decrypt($c1) -ne "Próba_jelszó123") { throw "v2 round-trip failed" }

# 2) random IV: same plaintext, different ciphertext
$c2 = $enc.AES256Encrypt("Próba_jelszó123")
if ($c1 -eq $c2) { throw "IV is not random" }
if ($enc.AES256Decrypt($c2) -ne "Próba_jelszó123") { throw "second round-trip failed" }

# 3) legacy ciphertext still decrypts
$legacy = Get-Content "$env:TEMP\pdfsigner-legacy-cipher.txt"
if ($enc.AES256Decrypt($legacy) -ne "Próba_jelszó123") { throw "legacy decrypt failed" }

# 4) empty input
if ($enc.AES256Encrypt("") -ne "") { throw "empty encrypt changed" }
if ($enc.AES256Decrypt("") -ne "") { throw "empty decrypt changed" }

"ALL ENCRYPT CHECKS PASSED"
```

Expected: `ALL ENCRYPT CHECKS PASSED`.

- [ ] **Step 5: Commit**

```powershell
git add "PDF Signer for Tungsten Capture Common/Encrypt.vb"
git commit -m "Use random IV with versioned ciphertext format for stored passwords"
```

---

### Task 4: M2 — dispose the MQFTP `FileSystemWatcher`

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\MQFTP\MQFTP.vb:69-75`

- [ ] **Step 1: Wrap the watcher in `Using`**

Replace:

```vb
            Dim pdfWatcher As New FileSystemWatcher With {
                .Path = _settings.MQFolderIn,
                .Filter = pdfFileNameWithExtension,
                .EnableRaisingEvents = True,
                .IncludeSubdirectories = False
            }
            pdfWatchRes = pdfWatcher.WaitForChanged(WatcherChangeTypes.Changed Or WatcherChangeTypes.Created, timeOut)
```

with:

```vb
            Using pdfWatcher As New FileSystemWatcher With {
                .Path = _settings.MQFolderIn,
                .Filter = pdfFileNameWithExtension,
                .EnableRaisingEvents = True,
                .IncludeSubdirectories = False
            }
                pdfWatchRes = pdfWatcher.WaitForChanged(WatcherChangeTypes.Changed Or WatcherChangeTypes.Created, timeOut)
            End Using
```

(`pdfWatchRes` is already declared at method top, so the post-block checks compile unchanged.)

- [ ] **Step 2: BUILD; verify zero errors.**

- [ ] **Step 3: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/MQFTP/MQFTP.vb"
git commit -m "Dispose MQFTP FileSystemWatcher"
```

---

### Task 5: P1 — remove dead buffering

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamer.vb:13-56`
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb:46-54`
- Modify: `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb:110`

- [ ] **Step 1: Delete the dead `PDFStreamer.SignDocument` method** (`PDFStreamer.vb` lines 13–56, the whole `Friend Function SignDocument(...) ... End Function` block). `SignDocumentStream` stays.

- [ ] **Step 2: Delete the commented-out call site** in `SignatureOperation.vb`:

```vb
                'sigRes = pdfstr.SignDocument(sigReq)
```

- [ ] **Step 3: Read directly into the byte array in `MNBSigner.SignSinglePass`**

Replace:

```vb
        Dim res As New SignatureResult
        Dim PdfBin As Byte()

        Using mt As New MemoryTributary
            Request.FileToSign.Seek(0, SeekOrigin.Begin)
            Request.FileToSign.CopyTo(mt)
            PdfBin = mt.ToArray
        End Using
```

with:

```vb
        Dim res As New SignatureResult

        ' read the whole document into the request buffer without an intermediate copy
        Dim PdfBin(CInt(Request.FileToSign.Length) - 1) As Byte
        Request.FileToSign.Seek(0, SeekOrigin.Begin)
        Dim offset As Integer = 0
        While offset < PdfBin.Length
            Dim bytesRead As Integer = Request.FileToSign.Read(PdfBin, offset, PdfBin.Length - offset)
            If bytesRead = 0 Then Exit While
            offset += bytesRead
        End While
```

- [ ] **Step 4: BUILD; verify zero errors.**

- [ ] **Step 5: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/PDFStreamer/PDFStreamer.vb" "PDF Signer for Tungsten Capture/Signature/Modules/MNBSigner/MNBSigner.vb" "PDF Signer for Tungsten Capture/Signature/SignatureOperation.vb"
git commit -m "Remove dead buffered PDFStreamer call and MNB double copy"
```

---

### Task 6: P5 — truncate the reused stream in `PDFStreamer.SignDocumentStream`

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamer.vb` (read-back loop, formerly lines 100–109)

- [ ] **Step 1: Add `SetLength` after the read-back loop**

After:

```vb
        While hasData
            count = req.DocumentStream.Read(buffer, 0, bufferLen)
            If count > 0 Then
                Request.FileToSign.Write(buffer, 0, count)
            Else
                hasData = False
            End If
        End While
```

insert:

```vb
        ' truncate: a signed file shorter than the original must not leave stale trailing bytes
        Request.FileToSign.SetLength(Request.FileToSign.Position)
```

- [ ] **Step 2: BUILD; verify zero errors.**

- [ ] **Step 3: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/PDFStreamer/PDFStreamer.vb"
git commit -m "Truncate reused stream after writing signed content"
```

---

### Task 7: M1 — deterministic stream disposal, delete GarbageCollector

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb` (const line 7, GC calls lines 64–67 and 258–261, `Finally` block)
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb` (`SignMultiPass` error paths)
- Delete: `PDF Signer for Tungsten Capture\Helper\GarbageCollector.vb`

- [ ] **Step 1: In `SignatureOperation.vb`, remove both GC blocks and the constant**

Delete line 7:

```vb
    Private Const GCFileSizeLimit As Integer = 10485760
```

Delete (lines 64–67):

```vb
            ' garbage collect if file is larger than 10 MB
            If Document.FileSize > GCFileSizeLimit Then
                GarbageCollector.Execute()
            End If
```

Delete from the `Finally` (lines 258–261):

```vb
            ' garbage collect in case file was larger than 10 MB
            If Document.FileSize > GCFileSizeLimit Then
                GarbageCollector.Execute()
            End If
```

- [ ] **Step 2: Dispose `SignedFile` in the same `Finally`**

The `Finally` currently ends (after Step 1's deletion):

```vb
        Finally
            If FileToSign IsNot Nothing Then
                FileToSign.Close()
                FileToSign.Dispose()

                If TypeOf FileToSign Is FileStream Then
                    Dim tempFile As String = CType(FileToSign, FileStream).Name
                    If File.Exists(tempFile) Then File.Delete(tempFile)
                End If
            End If

            Document.ProgressPercent = 100
        End Try
```

Insert before `Document.ProgressPercent = 100`:

```vb
            ' deterministically release the signed-file stream (MNB MemoryTributary / MQFTP FileStream);
            ' SBB and PDFStreamer return the input stream, which is disposed above
            If sigRes IsNot Nothing AndAlso sigRes.SignedFile IsNot Nothing AndAlso sigRes.SignedFile IsNot FileToSign Then
                sigRes.SignedFile.Dispose()
                sigRes.SignedFile = Nothing
            End If
```

- [ ] **Step 3: Delete `PDF Signer for Tungsten Capture\Helper\GarbageCollector.vb`** (SDK-style project — no project file edit needed).

- [ ] **Step 4: Dispose `mtSigned` on the error paths of `MNBSigner.SignMultiPass`**

Add `mtSigned.Dispose()` immediately before `Return res` in all five error blocks and the `Catch`:

1. Init failure:
```vb
            If Not InitRes.Success OrElse InitRes.Value Is Nothing OrElse String.IsNullOrWhiteSpace(InitRes.Value.Guid) Then
                res.ErrorMessage = GetErrors(InitRes)
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If
```
2. Chunk-send failure:
```vb
                If Not ChunkSendRes.Success Then
                    res.ErrorMessage = GetErrors(ChunkSendRes)
                    res.SignatureLog = _sbSignLog.ToString
                    mtSigned.Dispose()
                    Return res
                End If
```
3. Timeout (`SigningStatus = 1`):
```vb
            If SigningStatus = 1 Then ' timeout
                res.ErrorMessage = $"Időtúllépés: nem érkezett aláírt fájl a szerverről {(Now - StartTime).TotalSeconds} mp alatt."
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If
```
4. Server error (`SigningStatus = -1`):
```vb
            If SigningStatus = -1 OrElse Not ServerRes.Success Then ' error while signing
                res.ErrorMessage = GetErrors(ServerRes)
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If
```
5. Chunk-receive failure:
```vb
                    If Not ChunkReceiveRes.Success Then
                        res.ErrorMessage = GetErrors(ChunkReceiveRes)
                        res.SignatureLog = _sbSignLog.ToString
                        mtSigned.Dispose()
                        Return res
                    End If
```
6. Catch:
```vb
        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = ex.ToString
            mtSigned.Dispose()
            Return res
        End Try
```

- [ ] **Step 5: BUILD; verify zero errors** (also confirms nothing else referenced `GarbageCollector`).

- [ ] **Step 6: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/SignatureOperation.vb" "PDF Signer for Tungsten Capture/Signature/Modules/MNBSigner/MNBSigner.vb" "PDF Signer for Tungsten Capture/Helper/GarbageCollector.vb"
git commit -m "Dispose signing streams deterministically, remove forced GC"
```

---

### Task 8: P3 — single certificate enumeration per document switch

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb` (`GetCertificatesFromStore`, lines 56–69)
- Modify: `PDF Signer for Tungsten Capture\FrmPdfSigner.vb` (`BarComboProvider_EditValueChanged` ~line 578, `RefreshCryptoProviders` ~line 1197, new field)

- [ ] **Step 1: Compute `IsCertificateQualified` once per certificate**

In `GetCertificatesFromStore`, replace the loop body:

```vb
            If cert.ValidFrom < Now AndAlso cert.ValidTo > Now AndAlso (cert.Extensions.KeyUsage.DigitalSignature OrElse cert.Extensions.KeyUsage.NonRepudiation) Then
                If QualifiedCertificatesOnly AndAlso Not IsCertificateQualified(cert) Then
                    Continue For
                End If

                sigCert.Certificate = cert.ToX509Certificate2(False)
                sigCert.IsQualified = IsCertificateQualified(cert)
                res.Add(sigCert)
            End If
```

with:

```vb
            If cert.ValidFrom < Now AndAlso cert.ValidTo > Now AndAlso (cert.Extensions.KeyUsage.DigitalSignature OrElse cert.Extensions.KeyUsage.NonRepudiation) Then
                Dim isQualified As Boolean = IsCertificateQualified(cert)
                If QualifiedCertificatesOnly AndAlso Not isQualified Then
                    Continue For
                End If

                sigCert.Certificate = cert.ToX509Certificate2(False)
                sigCert.IsQualified = isQualified
                res.Add(sigCert)
            End If
```

- [ ] **Step 2: Add the guard field to `FrmPdfSigner`**

Next to the other private fields (after `Private _SaveLayout As Boolean = True`):

```vb
    Private _RefreshingProviders As Boolean
```

- [ ] **Step 3: Skip the duplicate refresh in `BarComboProvider_EditValueChanged`**

Replace:

```vb
        If selectedProvider.SupportsLocalCertificates AndAlso BtnRefreshCertificates.Enabled Then
            BarComboCert.Enabled = True
            BtnRefreshCertificates_ItemClick(Nothing, Nothing)
        Else
            BarComboCert.Enabled = False
        End If
```

with:

```vb
        If selectedProvider.SupportsLocalCertificates AndAlso BtnRefreshCertificates.Enabled Then
            BarComboCert.Enabled = True
            ' RefreshCryptoProviders triggers the refresh itself; avoid enumerating the cert store twice
            If Not _RefreshingProviders Then BtnRefreshCertificates_ItemClick(Nothing, Nothing)
        Else
            BarComboCert.Enabled = False
        End If
```

- [ ] **Step 4: Set the guard in `RefreshCryptoProviders`**

Replace:

```vb
        For Each provider As CryptoProviderBase In activeProviders
            ComboProvider.Items.Add(provider)

            If provider.ProviderType = docitem.SetupData.DefaultCryptographicProvider Then
                BarComboProvider.EditValue = provider
            End If
        Next

        BtnRefreshCertificates_ItemClick(Nothing, Nothing)
```

with:

```vb
        _RefreshingProviders = True
        Try
            For Each provider As CryptoProviderBase In activeProviders
                ComboProvider.Items.Add(provider)

                If provider.ProviderType = docitem.SetupData.DefaultCryptographicProvider Then
                    BarComboProvider.EditValue = provider
                End If
            Next
        Finally
            _RefreshingProviders = False
        End Try

        BtnRefreshCertificates_ItemClick(Nothing, Nothing)
```

- [ ] **Step 5: BUILD; verify zero errors.**

- [ ] **Step 6: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/SBB/SBBPDF.vb" "PDF Signer for Tungsten Capture/FrmPdfSigner.vb"
git commit -m "Enumerate certificate store once per document activation"
```

---

### Task 9: M3 — symmetric event unhooking and SBB object disposal

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb` (`SignDocument` `Finally`, re-open sites ~349/~430, `PADESHandler_OnCertValidatorPrepared`, `PADESHandler_OnCertValidatorFinished`, `GetCertificatesFromStore`)

- [ ] **Step 1: Unhook `TSPClient` handlers in `SignDocument`'s `Finally`**

After the existing six `RemoveHandler` lines, add:

```vb
            RemoveHandler TSPClient.OnBeforeSign, AddressOf TSPClient_OnBeforeSign
            RemoveHandler TSPClient.OnCertificateValidate, AddressOf TSPClient_OnCertificateValidate
            RemoveHandler TSPClient.OnHTTPError, AddressOf TSPClient_OnHTTPError
            RemoveHandler TSPClient.OnTSPError, AddressOf TSPClient_OnTSPError
```

- [ ] **Step 2: `RemoveHandler` before `AddHandler` at the document re-open sites**

Site 1 (revocation-info loop, ~line 349):

```vb
                        PADESSignatureHandler = CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler)
                        RemoveHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                        AddHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
```

Site 2 (document-timestamp revocation, ~line 430):

```vb
                            RemoveHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                            AddHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
```

- [ ] **Step 3: `RemoveHandler` before `AddHandler` inside `PADESHandler_OnCertValidatorPrepared`**

Replace the ten `AddHandler CertValidator.* ...` lines at the end of the method with remove-then-add pairs (the same validator instance can be prepared multiple times per run):

```vb
        RemoveHandler CertValidator.OnBeforeCRLRetrieverUse, AddressOf CertValidator_OnBeforeCRLRetrieverUse
        RemoveHandler CertValidator.OnBeforeOCSPClientUse, AddressOf CertValidator_OnBeforeOCSPClientUse
        RemoveHandler CertValidator.OnCRLError, AddressOf CertValidator_OnCRLError
        RemoveHandler CertValidator.OnCRLNeeded, AddressOf CertValidator_OnCRLNeeded
        RemoveHandler CertValidator.OnCRLRetrieved, AddressOf CertValidator_OnCRLRetrieved
        RemoveHandler CertValidator.OnOCSPError, AddressOf CertValidator_OnOCSPError
        RemoveHandler CertValidator.OnAfterCRLUse, AddressOf CertValidator_OnAfterCRLUse
        RemoveHandler CertValidator.OnAfterOCSPResponseUse, AddressOf CertValidator_OnAfterOCSPResponseUse
        RemoveHandler CertValidator.OnBeforeCertificateValidation, AddressOf CertValidator_OnBeforeCertificateValidation
        RemoveHandler CertValidator.OnAfterCertificateValidation, AddressOf CertValidator_OnAfterCertificateValidation

        AddHandler CertValidator.OnBeforeCRLRetrieverUse, AddressOf CertValidator_OnBeforeCRLRetrieverUse
        AddHandler CertValidator.OnBeforeOCSPClientUse, AddressOf CertValidator_OnBeforeOCSPClientUse
        AddHandler CertValidator.OnCRLError, AddressOf CertValidator_OnCRLError
        AddHandler CertValidator.OnCRLNeeded, AddressOf CertValidator_OnCRLNeeded
        AddHandler CertValidator.OnCRLRetrieved, AddressOf CertValidator_OnCRLRetrieved
        AddHandler CertValidator.OnOCSPError, AddressOf CertValidator_OnOCSPError
        AddHandler CertValidator.OnAfterCRLUse, AddressOf CertValidator_OnAfterCRLUse
        AddHandler CertValidator.OnAfterOCSPResponseUse, AddressOf CertValidator_OnAfterOCSPResponseUse
        AddHandler CertValidator.OnBeforeCertificateValidation, AddressOf CertValidator_OnBeforeCertificateValidation
        AddHandler CertValidator.OnAfterCertificateValidation, AddressOf CertValidator_OnAfterCertificateValidation
```

- [ ] **Step 4: Unhook the validator handlers in `PADESHandler_OnCertValidatorFinished`**

At the end of the method add the same ten `RemoveHandler` lines as Step 3 (remove only — the validator is finished).

- [ ] **Step 5: Dispose SBB objects at the end of `SignDocument`'s `Finally`**

After the temp-file cleanup block, add:

```vb
            ' release SBB objects deterministically (types that implement IDisposable)
            Try
                TryCast(doc, IDisposable)?.Dispose()
                TryCast(SystemStore, IDisposable)?.Dispose()
                TryCast(CertStorage, IDisposable)?.Dispose()
                TryCast(_TrustedRootCertStorage, IDisposable)?.Dispose()
                TryCast(HTTPSClient, IDisposable)?.Dispose()
                TryCast(TSPClient, IDisposable)?.Dispose()
            Catch
            End Try
```

(`doc`, `SystemStore`, `CertStorage` are declared before the `Try`, so they are in scope. `TryCast(...)?.Dispose()` is a no-op for types without `IDisposable` — no per-type verification needed.)

- [ ] **Step 6: Dispose the store in `GetCertificatesFromStore`**

Before `Return res` add:

```vb
        TryCast(SystemStore, IDisposable)?.Dispose()
```

(`ToX509Certificate2(False)` copies the certificate, so the returned list does not reference the store.)

- [ ] **Step 7: BUILD; verify zero errors.** If a `TryCast(..., IDisposable)` line fails to compile because the compiler statically knows the type never implements `IDisposable` (BC30311 family), delete that single line — that type needs no disposal.

- [ ] **Step 8: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/SBB/SBBPDF.vb"
git commit -m "Unhook SBB event handlers symmetrically and dispose SBB objects"
```

---

### Task 10: S1 — TLS certificate validation for TSA/OCSP/CRL channels

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\ServerCertificateValidator.vb`
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb` (`HTTPSClient_OnCertificateValidate`, comment at line ~109, `CertValidator_OnBeforeOCSPClientUse`, `CertValidator_OnBeforeCRLRetrieverUse`)

- [ ] **Step 1: Create `ServerCertificateValidator.vb`** with this exact content:

```vb
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports SBCertValidator
Imports SBX509

''' <summary>
''' TLS szerver tanúsítványok ellenőrzése a Windows tanúsítványtár alapján
''' (lánc felépítése + hosztnév egyezés).
''' </summary>
Friend Class ServerCertificateValidator

    ''' <summary>
    ''' Validates a TLS server certificate: chain to the Windows trust store plus
    ''' hostname match. CA certificates (chain members presented during the handshake)
    ''' only need a valid chain.
    ''' </summary>
    Friend Shared Function Validate(Certificate As TElX509Certificate, TargetHost As String, ByRef FailureReason As String) As Boolean
        FailureReason = Nothing

        If String.IsNullOrEmpty(TargetHost) Then
            FailureReason = "nincs megadva cél kiszolgálónév"
            Return False
        End If

        Dim winCert As X509Certificate2
        Try
            winCert = Certificate.ToX509Certificate2(False)
        Catch ex As Exception
            FailureReason = $"a tanúsítvány nem konvertálható: {ex.Message}"
            Return False
        End Try

        ' chain validation against the Windows trust store
        ' (revocation checking at the TLS layer is intentionally off; document-level
        ' revocation checking is governed separately by the provider settings)
        Using chain As New X509Chain()
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck
            If Not chain.Build(winCert) Then
                Dim sb As New StringBuilder
                For Each status As X509ChainStatus In chain.ChainStatus
                    If sb.Length > 0 Then sb.Append(", ")
                    sb.Append(status.StatusInformation.Trim())
                Next
                FailureReason = $"a tanúsítványlánc nem érvényes: {sb}"
                Return False
            End If
        End Using

        ' the TLS event fires for every certificate of the server's chain;
        ' hostname matching only applies to the leaf (non-CA) certificate
        If IsCertificateAuthority(winCert) Then Return True

        If Not MatchesHost(winCert, TargetHost) Then
            FailureReason = $"a tanúsítvány nem a(z) '{TargetHost}' kiszolgálóhoz tartozik"
            Return False
        End If

        Return True
    End Function

    Private Shared Function IsCertificateAuthority(Certificate As X509Certificate2) As Boolean
        For Each ext As X509Extension In Certificate.Extensions
            Dim basicConstraints As X509BasicConstraintsExtension = TryCast(ext, X509BasicConstraintsExtension)
            If basicConstraints IsNot Nothing Then
                Return basicConstraints.CertificateAuthority
            End If
        Next
        Return False
    End Function

    Private Shared Function MatchesHost(Certificate As X509Certificate2, TargetHost As String) As Boolean
        Dim names As List(Of String) = GetSubjectAlternativeDnsNames(Certificate)

        If names.Count = 0 Then
            ' no SAN extension: fall back to the subject name (CN)
            Dim cn As String = Certificate.GetNameInfo(X509NameType.DnsName, False)
            If Not String.IsNullOrEmpty(cn) Then names.Add(cn)
        End If

        For Each name As String In names
            If HostNameMatches(name, TargetHost) Then Return True
        Next

        Return False
    End Function

    Private Shared Function HostNameMatches(Pattern As String, Host As String) As Boolean
        If String.Equals(Pattern, Host, StringComparison.OrdinalIgnoreCase) Then Return True

        ' wildcard: "*.example.com" matches exactly one leading label
        If Pattern.StartsWith("*.", StringComparison.Ordinal) Then
            Dim dotIndex As Integer = Host.IndexOf("."c)
            If dotIndex > 0 Then
                Return String.Equals(Pattern.Substring(2), Host.Substring(dotIndex + 1), StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Extracts dNSName entries from the SAN extension (OID 2.5.29.17) at the DER
    ''' level, independent of OS display language.
    ''' </summary>
    Private Shared Function GetSubjectAlternativeDnsNames(Certificate As X509Certificate2) As List(Of String)
        Dim res As New List(Of String)

        For Each ext As X509Extension In Certificate.Extensions
            If ext.Oid Is Nothing OrElse ext.Oid.Value <> "2.5.29.17" Then Continue For

            Dim data As Byte() = ext.RawData
            ' DER SEQUENCE of GeneralName; dNSName is context tag [2] = &H82 (IA5String)
            Dim pos As Integer = 0
            Dim seqLen As Integer
            If Not ReadTagAndLength(data, pos, &H30, seqLen) Then Continue For

            Dim seqEnd As Integer = pos + seqLen
            While pos < seqEnd
                Dim tag As Byte = data(pos)
                Dim len As Integer
                If Not ReadTagAndLength(data, pos, tag, len) Then Exit While

                If tag = &H82 Then
                    res.Add(Encoding.ASCII.GetString(data, pos, len))
                End If
                pos += len
            End While
        Next

        Return res
    End Function

    ''' <summary>
    ''' Reads a DER tag + length header; on success Position points at the content.
    ''' </summary>
    Private Shared Function ReadTagAndLength(Data As Byte(), ByRef Position As Integer, ExpectedTag As Byte, ByRef Length As Integer) As Boolean
        Length = 0
        If Position >= Data.Length OrElse Data(Position) <> ExpectedTag Then Return False
        Position += 1

        If Position >= Data.Length Then Return False
        Dim first As Byte = Data(Position)
        Position += 1

        If first < &H80 Then
            Length = first
        Else
            Dim byteCount As Integer = first And &H7F
            If byteCount = 0 OrElse byteCount > 4 OrElse Position + byteCount > Data.Length Then Return False
            For i As Integer = 1 To byteCount
                Length = (Length << 8) Or CInt(Data(Position))
                Position += 1
            Next
        End If

        Return Position + Length <= Data.Length
    End Function

End Class

''' <summary>
''' Egy adott kiszolgálóhoz kötött TLS tanúsítvány-ellenőrző az OCSP/CRL letöltő
''' HTTP kliensekhez. (Lambdák ByRef paraméterekkel nem használhatók eseménykezelőként,
''' ezért kell külön osztály.)
''' </summary>
Friend Class HostBoundCertificateValidator
    Private ReadOnly _targetHost As String
    Private ReadOnly _log As StringBuilder

    Friend Sub New(TargetHost As String, Log As StringBuilder)
        _targetHost = TargetHost
        _log = Log
    End Sub

    Friend Sub OnCertificateValidate(Sender As Object, X509Certificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        Dim failureReason As String = Nothing
        If ServerCertificateValidator.Validate(X509Certificate, _targetHost, failureReason) Then
            Validity = TSBCertificateValidity.cvOk
        Else
            Validity = TSBCertificateValidity.cvInvalid
            _log.AppendLine($"  TLS tanúsítvány hiba ({_targetHost}): {failureReason}")
        End If
    End Sub
End Class
```

- [ ] **Step 2: Make `HTTPSClient_OnCertificateValidate` validate instead of forcing `cvOk`**

Replace:

```vb
    Private Sub HTTPSClient_OnCertificateValidate(Sender As Object, X509Certificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        Validity = TSBCertificateValidity.cvOk
    End Sub
```

with:

```vb
    Private Sub HTTPSClient_OnCertificateValidate(Sender As Object, X509Certificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        Dim targetHost As String = _settings.TSAURL?.Host
        Dim failureReason As String = Nothing

        If ServerCertificateValidator.Validate(X509Certificate, targetHost, failureReason) Then
            Validity = TSBCertificateValidity.cvOk
        Else
            Validity = TSBCertificateValidity.cvInvalid
            _sbSignLog.AppendLine($"  TLS tanúsítvány hiba ({targetHost}): {failureReason}")
        End If
    End Sub
```

- [ ] **Step 3: Update the misleading comments on the three `AutoValidateCertificates = False` lines**

Line ~109:

```vb
        HTTPSClient.AutoValidateCertificates = False ' validation is done in HTTPSClient_OnCertificateValidate (SBB's built-in validation does not work)
```

In `CertValidator_OnBeforeOCSPClientUse` and `CertValidator_OnBeforeCRLRetrieverUse`:

```vb
            clnt.HTTPClient.AutoValidateCertificates = False ' validation is done by the HostBoundCertificateValidator attached below
```
```vb
            retr.HTTPClient.AutoValidateCertificates = False ' validation is done by the HostBoundCertificateValidator attached below
```

- [ ] **Step 4: Attach a host-bound validator in `CertValidator_OnBeforeOCSPClientUse`**

Inside the `If TypeOf OCSPClient Is TElHTTPOCSPClient Then` block, after the existing proxy configuration lines (before the proxy log `If`), add:

```vb
            Try
                Dim hostValidator As New HostBoundCertificateValidator(New Uri(OCSPLocation).Host, _sbSignLog)
                AddHandler clnt.HTTPClient.OnCertificateValidate, AddressOf hostValidator.OnCertificateValidate
                ' no RemoveHandler: the OCSP client is transient, released after the retrieval
            Catch ex As UriFormatException
                _sbSignLog.AppendLine($"  Érvénytelen OCSP URL: {OCSPLocation}")
            End Try
```

- [ ] **Step 5: Attach a host-bound validator in `CertValidator_OnBeforeCRLRetrieverUse`**

Inside the `If TypeOf Retriever Is TElHTTPCRLRetriever Then` block, after the existing proxy configuration lines, add:

```vb
            Try
                Dim hostValidator As New HostBoundCertificateValidator(New Uri(Location).Host, _sbSignLog)
                AddHandler retr.HTTPClient.OnCertificateValidate, AddressOf hostValidator.OnCertificateValidate
                ' no RemoveHandler: the CRL retriever is transient, released after the retrieval
            Catch ex As UriFormatException
                _sbSignLog.AppendLine($"  Érvénytelen CRL URL: {Location}")
            End Try
```

- [ ] **Step 6: BUILD; verify zero errors.** If `TSBCertificateValidity` is not found in the new file, check the exact namespace used by `SBBPDF.vb`'s imports (`SBCertValidator`) and adjust the `Imports` line.

- [ ] **Step 7: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/Signature/Modules/SBB/ServerCertificateValidator.vb" "PDF Signer for Tungsten Capture/Signature/Modules/SBB/SBBPDF.vb"
git commit -m "Validate TLS server certificates on TSA/OCSP/CRL channels"
```

**Manual acceptance (after deployment, user):** sign with timestamping against the real TSA — must succeed; point `TSAURL` at a host with a non-matching certificate — must fail with the `TLS tanúsítvány hiba` log line.

---

### Task 11: P2 + D3 — single completion routine, no UI-thread sleeps

**Files:**
- Modify: `PDF Signer for Tungsten Capture\FrmPdfSigner.vb` (`BtnSign_ItemClick` ~481–502, `BtnSkipSign_ItemClick` ~553–575, `BtnReject_ItemClick` ~652–674; new method)

- [ ] **Step 1: Add the completion routine** (place it after `SigningTasksRunning`):

```vb
    ''' <summary>
    ''' Common epilogue of sign/skip/reject: jump to the next signable document, or —
    ''' when none is left — wait for the running signing tasks, re-check (a task may
    ''' have failed and made its document signable again), then offer to close the batch.
    ''' </summary>
    Private Async Function FinishDocumentActionAsync(ClosingLogMessage As String) As Task
        If SelectNextSignableDocument(False) Then Return

        If _ActiveTasks IsNot Nothing Then Await Task.WhenAll(_ActiveTasks)

        ' yield once so data-binding callbacks posted by the worker threads are processed
        Await Task.Yield()

        ' we have to check it again, as a previously in-progress task might have come back with an error
        If SelectNextSignableDocument(False) Then Return

        If Not AllDocumentsProcessed() Then Return
        If _Batch Is Nothing Then Return

        _logger.Debug(ClosingLogMessage, _Batch.Name)

        Dim keepApplicationOpen As Boolean = CloseBatch(True, True, True)

        ' if program was opened in single batch mode, then we close the application
        If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()
    End Function
```

(Note: unlike the old inline blocks, `_Batch` is checked *before* the `_logger.Debug` call — the old code dereferenced `_Batch.Name` before its own `IsNot Nothing` check.)

- [ ] **Step 2: Replace the epilogue in `BtnSign_ItemClick`**

Replace everything from `' we didn't move to a different document after signing ...` down to its matching final `End If` (the block containing `Await Task.WhenAll(_ActiveTasks)`, `Thread.Sleep(200)`, `Messages.Batch_Closing`) with:

```vb
        Await FinishDocumentActionAsync(Messages.Batch_Closing)
```

- [ ] **Step 3: Replace the epilogue in `BtnSkipSign_ItemClick`**

Inside `If doc.IsSkipped Then`, replace the block from `' we didn't move to a different document after skipping ...` to its matching final `End If` with:

```vb
            Await FinishDocumentActionAsync(Messages.Batch_Closing_With_Rejected_PDF)
```

- [ ] **Step 4: Replace the epilogue in `BtnReject_ItemClick`**

Inside `If currentItem.IsRejected Then`, replace the block from `' we didn't move to a different document after rejecting ...` to its matching final `End If` with:

```vb
            Await FinishDocumentActionAsync(Messages.Batch_Closing_With_Rejected_PDF)
```

- [ ] **Step 5: Verify no `Thread.Sleep` remains in `FrmPdfSigner.vb`**

```powershell
Select-String -Path "C:\Projects\PDF Signer\PDF Signer for Tungsten Capture\FrmPdfSigner.vb" -Pattern "Thread.Sleep"
```

Expected: no output.

- [ ] **Step 6: BUILD; verify zero errors.**

- [ ] **Step 7: Commit**

```powershell
git add "PDF Signer for Tungsten Capture/FrmPdfSigner.vb"
git commit -m "Extract document-action epilogue, drop UI-thread sleeps"
```

**Manual acceptance (user):** sign, skip, and reject documents through end-of-batch — the close-batch prompt must still appear, with no UI freeze while tasks finish.

---

### Task 12: Final verification — shipping build

- [ ] **Step 1: FullRelease|x86 build (the shipping configuration):**

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "C:\Projects\PDF Signer\PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=FullRelease /p:Platform=x86 /p:CopyToCaptureBin=false /flp:LogFile=build-fullrelease.log
```

Expected: log contains ` 0 Error(s)` and no `BC42353`.

```powershell
Select-String -Path build-fullrelease.log -Pattern " 0 Error\(s\)"
Select-String -Path build-fullrelease.log -Pattern "BC42353"   # expect: no output
Remove-Item build-fullrelease.log
```

- [ ] **Step 2: Confirm only intended commits were made**

```powershell
git log --oneline -14
git status --short
```

Expected: the 11 commits from Tasks 1–11 on top of the spec commit; `git status` shows only the pre-existing unrelated modifications.

- [ ] **Step 3: Report the remaining manual smoke tests to the user** (TSA signing for S1, end-of-batch flow for P2, password re-save in Setup for S2 — listed in the spec's Verification section).
