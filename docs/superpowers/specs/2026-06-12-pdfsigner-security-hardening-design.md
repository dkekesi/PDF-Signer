# PDF Signer — Security, Memory, and Performance Hardening

**Date:** 2026-06-12
**Status:** Approved design, pending implementation
**Scope:** Items S1, S2, S3, M1, M2, M3, P1, P2(+D3), P3, P5, D1-cheap selected from the 2026-06-12 code audit.

## Background

A full audit of the solution (runtime, Common, Setup, WFA) identified security holes,
memory leaks, performance issues, and design discrepancies. The user selected the items
below for implementation. Decisions taken during design review:

- **S1:** Always validate TLS server certificates (no config toggle).
- **S2:** Random-IV versioned ciphertext for stored passwords; license file format unchanged.
- **S3:** Remove the license-generator comment only; runtime key stays in source.
- **P2:** Full restructure of the completion logic (includes the D3 deduplication).

There is no automated test infrastructure; the acceptance gate is a clean
FullRelease|x86 build plus the manual smoke tests listed at the end.

## S1 — Enforce TLS certificate validation (SBB HTTPS clients)

**Problem:** `SBBPDF.vb` disables certificate validation on every HTTPS channel used
during signing: `HTTPSClient.AutoValidateCertificates = False` (line 109) with
`HTTPSClient_OnCertificateValidate` unconditionally returning `cvOk` (lines 607–609),
and `AutoValidateCertificates = False` on the OCSP client (line 717) and CRL retriever
(line 738) with no validating handler at all. TSA/OCSP/CRL traffic is open to MITM.

**Design:**

- New `Friend Class ServerCertificateValidator` in the runtime project
  (`Signature\Modules\SBB\`):
  - `Validate(Certificate As TElX509Certificate, TargetHost As String) As Boolean`
  - Converts via `ToX509Certificate2(False)`, builds an `X509Chain` against the
    Windows store (`X509RevocationMode.NoCheck` at the TLS layer — document-level
    revocation checking is a separate concern already governed by
    `_settings.RevocationCheck`), then matches `TargetHost` against the certificate's
    SAN DNS entries (wildcard-aware, RFC 6125 style) with CN fallback.
  - Enterprise-internal CAs work automatically if their roots are deployed to the
    Windows trust store.
- `HTTPSClient_OnCertificateValidate` sets `Validity` to `cvOk`/`cvInvalid` based on
  the validator; the target host is the TSA URL host (`_settings.TSAURL`).
- OCSP client and CRL retriever: in `CertValidator_OnBeforeOCSPClientUse` /
  `CertValidator_OnBeforeCRLRetrieverUse` the target URL is a parameter; attach an
  `OnCertificateValidate` handler to that client validating against that URL's host.
- On rejection: append a Hungarian log line naming the host and the chain/hostname
  failure reason to `_sbSignLog`, so the operator-visible signature log explains the
  failure.
- `AutoValidateCertificates` stays `False` everywhere (SBB's built-in validation is
  reported broken); our callbacks are the validation.

## S2 — Versioned ciphertext with random IV (`Encrypt.vb`)

**Problem:** `Encrypt.vb:8-9` hardcodes the AES-256 key **and a static IV**. All stored
credentials (TSA, proxy, MQFTP, MNB passwords, PDF Streamer auth code — decrypted in
`SetupParser.vb:84-120`) share it; identical plaintexts yield identical ciphertexts.

**Constraint:** Encrypted values live in Capture batch-class setup data that every
operator station must decrypt, so a machine-bound key (DPAPI) is not usable; a shipped
key is unavoidable. The license file uses the same class but is RSA-signed; its AES
layer is obfuscation only and its format is produced by the external license generator
tool — it stays untouched.

**Design:**

- New format: the string `v2:` + Base64(16-byte cryptographically random IV ‖
  AES-256-CBC ciphertext). Same embedded key, same `Encoding.Unicode` plaintext
  encoding, same CBC/PKCS7 parameters.
- `AES256Encrypt` always emits v2. `AES256Decrypt` branches on the `v2:` prefix;
  anything else uses the legacy static-IV path. Existing setup data and license files
  keep decrypting; passwords re-saved from the Setup panel upgrade transparently.
- `RandomNumberGenerator` for IVs.
- **Deployment caveat (goes into release notes):** setup data saved by an upgraded
  admin station cannot be decrypted by un-upgraded runtime stations. Roll out the
  runtime before or together with the Setup panel.

## S3 — Remove license-generator comment (`SBBPDF.vb:39-40`)

Delete the two comment lines containing the nsoftware generator URL and product key.
The runtime key in `SetLicenseKey(...)` stays (it must ship in the binary regardless).
No git history rewrite.

## M1 — Deterministic stream disposal; delete `GarbageCollector`

**Problem:** `SignatureResult.SignedFile` (a `MemoryTributary` holding the whole signed
PDF for MNB, or an open `FileStream` for MQFTP) is copied out in
`SignatureOperation.vb:186-189` and abandoned. `GarbageCollector.Execute()` (double
`GC.Collect` + `WaitForPendingFinalizers`, `SignatureOperation.vb:65-67` and `:258-261`)
exists to mop this up.

**Design:**

- In `SignatureOperation.SignDocument`'s `Finally`: dispose `sigRes.SignedFile` when it
  is not `Nothing` and not the same reference as `FileToSign` (SBB and PDFStreamer
  return the input stream, which the existing `Finally` already disposes).
- In `MNBSigner.SignMultiPass`: dispose `mtSigned` on every error-return path.
- Remove both `GarbageCollector.Execute()` call sites and delete
  `Helper\GarbageCollector.vb` (plus the `GCFileSizeLimit` constant if unused after).
- Out of scope: the MQFTP delete-while-open issue (deselected item M4) is unchanged;
  the response `FileStream` is now disposed after `SignDocument` completes, which is
  later than `MQFTP`'s own `DeleteWhenAvailable` call.

## M2 — Dispose the MQFTP `FileSystemWatcher`

`MQFTP.vb:69-75`: wrap the watcher in `Using`; hoist `pdfWatchRes` (already declared at
method top) so the post-`Using` checks compile unchanged.

## M3 — Symmetric event unhooking and SBB object disposal (`SBBPDF.vb`)

- `Finally` of `SignDocument`: add the missing `RemoveHandler`s for the four
  `TSPClient` events (`OnBeforeSign`, `OnCertificateValidate`, `OnHTTPError`,
  `OnTSPError`, added at lines 124–127).
- Re-open sites (lines 349, 430): `RemoveHandler` before `AddHandler` for
  `OnCertValidatorPrepared` so re-opening the document does not double-subscribe.
- `PADESHandler_OnCertValidatorFinished`: unhook the ten `CertValidator.*` handlers
  added in `PADESHandler_OnCertValidatorPrepared` (lines 657–666).
- Dispose in `Finally`: `doc`, `SystemStore`, `CertStorage`, `_TrustedRootCertStorage`,
  `HTTPSClient`, `TSPClient`; in `GetCertificatesFromStore`: the `TElWinCertStorage`.
  Each conditional on the SBB type actually implementing `IDisposable` (verify at
  implementation; skip types that don't).

## P1 — Remove dead buffering

- Delete `PDFStreamer.SignDocument` (`PDFStreamer.vb:13-56`; only call site is
  commented out at `SignatureOperation.vb:110` — delete that comment line too).
- `MNBSigner.SignSinglePass` (`MNBSigner.vb:50-54`): read the stream directly into a
  right-sized `Byte()` (loop until filled) instead of copying through a
  `MemoryTributary` and calling `ToArray()` (two full in-memory copies today).

## P2 + D3 — Single completion routine, no UI-thread sleeps

**Problem:** `BtnSign_ItemClick`, `BtnSkipSign_ItemClick`, `BtnReject_ItemClick`
(`FrmPdfSigner.vb:481-502, 553-575, 652-674`) each contain a near-identical ~22-line
block: select-next → `Await Task.WhenAll(_ActiveTasks)` → **`Thread.Sleep(200)` on the
UI thread** → re-check → close batch. The sleep papers over a perceived race with
data-binding updates from worker threads.

**Design:**

```vb
Private Async Function FinishDocumentActionAsync(closingLogMessage As String) As Task
    If SelectNextSignableDocument(False) Then Return
    If _ActiveTasks IsNot Nothing Then Await Task.WhenAll(_ActiveTasks)
    Await Task.Yield() ' drain any data-binding invokes queued by worker threads
    If SelectNextSignableDocument(False) Then Return
    If Not AllDocumentsProcessed() Then Return
    _logger.Debug(closingLogMessage, _Batch.Name)
    If _Batch Is Nothing Then Return
    Dim keepApplicationOpen As Boolean = CloseBatch(True, True, True)
    If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()
End Function
```

- The three handlers call `Await FinishDocumentActionAsync(...)` with their existing
  log message (`Messages.Batch_Closing` for sign,
  `Messages.Batch_Closing_With_Rejected_PDF` for skip/reject).
- Why the sleep can go: `Await Task.WhenAll` resumes via the WinForms
  `SynchronizationContext`, i.e. as a posted message that runs **after** any
  data-binding `BeginInvoke`s the worker threads queued before task completion (the
  tasks set `ProgressPercent`/`ErrorMessage` synchronously before finishing). The
  single `Task.Yield` is cheap insurance for invokes posted during the continuation
  itself.
- Behavior preserved: same guard order, same log messages, same single-batch-mode exit.
- `FileOperation` retry sleeps are out of scope (they run inside the signing task).

## P3 — Cheaper certificate refresh

- `SBBPDF.GetCertificatesFromStore` (`SBBPDF.vb:56-69`): call `IsCertificateQualified`
  once per certificate, reuse for the filter and the `IsQualified` flag.
- `FrmPdfSigner.RefreshCryptoProviders` (`:1197-1211`) enumerates the store twice per
  document switch: once via the `BarComboProvider_EditValueChanged` handler when
  `EditValue` is assigned, once via the explicit `BtnRefreshCertificates_ItemClick`
  call. Add a `_refreshingProviders` guard flag: set around the combo population so the
  `EditValueChanged` handler skips its refresh, keeping exactly one enumeration.
- Behavior preserved: certificates are still re-read on every document activation
  (smart-card/token plug-in keeps working) and via the manual refresh button.

## P5 — Truncate the reused stream (`PDFStreamer.vb`)

After the WCF read-back loop in `SignDocumentStream` (lines 100–109):
`Request.FileToSign.SetLength(Request.FileToSign.Position)` — a signed file shorter
than the original can no longer leave stale trailing bytes.

## D1 (cheap half) — Explicit returns and safe argument parsing

- Add explicit `Return` statements matching current implicit behavior to the three
  BC42353 functions: `FrmPdfSigner.SigningTasksRunning` (`Return False`),
  `FrmPdfSigner.SelectNextSignableDocument` (`Return False`),
  `BatchParser.GetBatchTreeItemIcon` (verify the implicit default at implementation —
  expected `Return Nothing`).
- `FrmPdfSigner.New` (`:114-116`): parse `-B<id>` with `Integer.TryParse`; invalid
  input leaves `_SingleBatchOpenID` = 0, falling back to the batch-open dialog.
- Build must end with zero BC42353 warnings.

## Verification

1. `msbuild "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug
   /p:Platform="Any CPU" /p:CopyToCaptureBin=false` — zero errors, BC42353 gone.
2. `PDFSigner32bit.cmd` (FullRelease|x86 + MSI) — zero errors.
3. Round-trip unit check of `Encrypt` v2/legacy paths via a throwaway harness during
   implementation (no test project exists).
4. **Manual acceptance (user):** sign a batch with the SBB provider against a real TSA
   (S1 — valid endpoint passes, a host with a bad certificate fails with a readable
   log); sign/skip/reject through end-of-batch (P2 — close-batch prompt still appears,
   no UI freeze); re-save provider passwords in the Setup panel and sign again (S2).

## Addendum (2026-06-12, round 2): follow-up batch A1–A5, B1, B3–B10

Approved scope: the deselected audit items A1–A5 and the reviewer follow-ups B1,
B3–B10 from round 1 (B2 — `X509Chain.ExtraStore` population — and the large
refactors A6–A8 remain out of scope). Decisions taken:

- **A1 (MNB Basic auth over HTTP):** enforce in both places. Runtime
  (`MNBSigner.SignDocument`) refuses user/password authentication on a non-HTTPS
  URL with a Hungarian error; the provider's `Validate()` in
  `MNBSignerCryptoProvider.vb` rejects the combination at save time (new
  `Messages` resource entry + `hu` variant).
- **A2 (MQFTP delete-while-open):** open the response stream with
  `FileShare.Delete` (new optional share-mode parameter on
  `FileOperation.ReadFileStreamWhenAvailable`), so `DeleteWhenAvailable` succeeds
  immediately and the file disappears when the stream is disposed (in
  `SignatureOperation`'s `Finally`). Not verifiable without a live MTRACK
  exchange — flagged for the user's MQFTP smoke test.
- **A3 (MQFTP response race):** arm the `FileSystemWatcher` *before* moving the
  request file out of staging, and check `File.Exists` before blocking in
  `WaitForChanged`; track arrival in a local boolean instead of relying solely on
  `WaitForChangedResult`.
- **A4 (impersonation logon type):** `LOGON32_LOGON_NEW_CREDENTIALS` +
  `LOGON32_PROVIDER_WINNT50` (both enum members already exist in
  `NativeMethods.vb`). Credentials then apply to outbound network access only —
  flagged for the MQFTP smoke test.
- **A5:** delete the dead `SHA256Managed` lines (and the stale comment) from
  `Encrypt.RSAVerifySignedString`.
- **B1:** `_DocumentActionRunning` reentrancy guard wrapping the bodies of the
  three sign/skip/reject handlers (early return + `Try/Finally` reset).
- **B3:** wildcard bases must contain a dot (`*.com` rejected); CN fallback only
  when the certificate has *no* SAN extension (not merely no dNSName entries).
- **B4:** `SBBPDF` implements `IDisposable` (disposing its two `ReadOnly` HTTP
  clients; documented as single-use); `SignatureOperation` wraps all three
  `SBBPDF` usages in `Using`; `SafeDispose` logs swallowed exceptions via NLog
  Debug.
- **B5:** `Encrypt` polish — `IVSizeBytes` constant, shared `CreateAes(iv)`
  factory, stricter v2 length guard (≥ 32 and block-aligned), legacy decrypt
  returns `Nothing` on malformed Base64 instead of throwing (improves the
  license-load failure mode from crash to error message).
- **B6:** `SignMultiPass` ownership-flag `Try/Finally` replaces the six inline
  `mtSigned.Dispose()` calls; `SignSinglePass` throws `EndOfStreamException`
  (Hungarian message) on short read.
- **B7:** `ComboProvider.Items.Clear()` moved inside the refresh guard; stale
  `BarComboProvider.EditValue` reset to the first active provider when the
  previous selection is no longer offered; the trailing certificate refresh runs
  only when the selected provider supports local certificates.
- **B8:** explicit `Return IconType.Batch` (with comment) instead of
  `Return Nothing`; NLog warning on malformed `-B` argument.
- **B9:** validator file XML docs unified to English; DER parser comments
  (tautological-tag call site, long-form length); set the SBB `Reason` flag on
  `cvInvalid` if a suitable constant exists, otherwise document why it stays 0.
- **B10:** the `AddHandler` block in `SBBPDF.SignDocument` moves inside the
  `Try` so the unhooking `Finally` is always reachable once handlers exist.

Verification as in round 1: builds (Debug + FullRelease|x86), the Encrypt
PowerShell harness (legacy sample reconstructed manually with the static IV,
since the legacy encrypt path no longer exists), and the user's manual smoke
tests — now explicitly including an MQFTP exchange test (A2/A3/A4).

## Risks

- **S1** is the highest-deployment-risk change: endpoints with certificates not
  chaining to the Windows store stop working *by design*. Mitigation: clear log
  message; internal CAs deployed via Windows trust store keep working.
- **S2** mixed-version fleets: v2 values are unreadable by old runtimes (release-note
  caveat).
- **P2** changes timing in the most behavior-sensitive UI path; mitigated by keeping
  guard order identical and the manual smoke test.
- **M3** SBB disposal: if a type's `Dispose` tears down shared state unexpectedly,
  scope back to handler unhooking only (decided per type at implementation).
