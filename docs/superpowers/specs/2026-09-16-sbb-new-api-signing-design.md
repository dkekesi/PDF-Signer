# Built-in "PDF Signer" provider on the SecureBlackbox v24 component API

**Date:** 2026-09-16
**Status:** implemented
**Branch / worktree:** `worktree-sbb-new-api` at `.claude\worktrees\sbb-new-api`

## 1. Goal

Re-implement the built-in cryptographic provider (displayed as **PDF Signer**, module
`Signature\Modules\SBB\`) on the nsoftware SecureBlackbox v24 component API (`PDFSigner`,
`PDFVerifier`, `CertificateValidator`, `CertificateManager`), mirroring the signing method
PDF Streamer adopted in its 2026-06-21 redesign (`C:\Projects\PDF Streamer`,
`docs\superpowers\specs\2026-06-21-secureblackbox-new-api-migration-design.md` and the follow-up
specs listed in its `CLAUDE.md`). The provider gets PDF Streamer's configuration surface for the
parts that apply to an interactive desktop signer, and the admin Setup UI is updated to match.

The other providers (PDF Streamer client, MQFTP, MNB Signer) are out of scope, as are the
pre/post processors (`PDFPreProcessor.vb`, `PDFPostProcessor.vb`), which keep using the legacy
`TElPDFDocument` API for attachments and metadata.

## 2. Decisions taken during brainstorming

| Topic | Decision |
|---|---|
| Port scope | PDF Streamer's core pipeline: signing-plan builder, sign / update-to-embed / lean document-timestamp passes, fail-closed validator checks, error and event translation, ETSI end-of-validity. |
| Dropped on purpose | Existing-signature pass (the provider signs **unsigned documents only**), PKI-cache `Config` strings, lean/full known-certificate retry and learning, revocation endpoint limiter, revocation readiness and early-embedding behaviour, attachment pass, diagnostics dump. One attempt per pass. |
| Levels offered | PAdES B-B, B-T, B-LT, B-LTA. None, DocumentTimestampOnly and DigitalSeal are not offered. |
| Structure | Port in place under `Signature\Modules\SBB\`, split into focused files; module keeps its de-facto contract so `SignatureOperation` and `FrmPdfSigner` change only where field names change. |
| SecureBlackbox assembly | Upgrade `lib\nsoftware.SecureBlackbox.dll` from 24.0.9650 (.NET Framework build) to PDF Streamer's 24.0.9710 (netstandard2.0 build, loads on net48). The lean document-timestamp workaround and the error-code translation were verified on that build. Both builds contain the legacy `TEl*` types, so the pre/post processors keep working. |
| License | The existing key (same product key as PDF Streamer's, expires 2026-11-15) is set as the per-instance `RuntimeLicense` of every new component. `SBUtils.Unit.SetLicenseKey` stays for the legacy API used by the pre/post processors. |
| Migration | No derivation from the old CSS values. A document class whose `PAdESLevel` CSS entry is missing loads the PDF Signer provider at constructor defaults. Only the XML backup import (`SetupDataFromXml`) derives the new fields from the old ones. |
| Tests | New MSTest project for the pure pieces; signing itself is verified manually. |
| Chain building | Trusted anchors from the Windows `Root` stores; known certificates from the Windows `CA` stores plus every certificate harvested from the document as it stands (signature chain, signature-timestamp and document-timestamp token certificates). |

## 3. Configuration model (Common project)

### 3.1 New enum

`Models\Enum.vb`:

```vb
Public Enum PAdESLevelType As Integer
    BaselineB = 3
    BaselineT = 4
    BaselineLT = 5
    BaselineLTA = 6
End Enum
```

The numeric values equal PDF Streamer's `PDFStreamer.Contracts.PAdESLevel` members so both
products agree on meaning. The undefined values 0-2 are rejected by validation.

### 3.2 `PDFSignerCryptoProvider` fields

| Property | Type | Default | CSS suffix (`CSS.BaseNamespace` + …) | XML element | Note |
|---|---|---|---|---|---|
| `Enabled` | Boolean | False | `IsPDFSignerEnabled` | `Enabled` | unchanged |
| `SigningOrganization` | String | "" | `SigningOrganization` | same | unchanged |
| `SigningReason` | String | "" | `SigningReason` | same | unchanged |
| `SignatureHashMethod` | Integer (`HashType`) | SHA256 | `SignatureHashMethod` | same | unchanged |
| `AllowQualifiedCertificatesOnly` | Boolean | False | `AllowQualifiedCertificatesOnly` | same | unchanged |
| `SignerNameFromLoggedOnUser` | Boolean | False | `SignerNameFromLoggedOnUser` | same | unchanged |
| **`PAdESLevel`** | `Integer (PAdESLevelType)` | `BaselineB` | **`PAdESLevel`** (new) | **`PAdESLevel`** (new) | stored as the integer value |
| `TSAURL` | Uri | Nothing | `TSAURL` | same | unchanged |
| `TSAUserName` | String | "" | `TSAUserName` | same | unchanged |
| `TSAPassword` | String | "" | `TSAPassword` | same | AES, unchanged |
| `TimeStampHashMethod` | Integer (`HashType`) | SHA256 | `TimeStampHashMethod` | same | used by the B-LTA archive timestamp only |
| **`EnableRevocationChecking`** | Boolean | False | **`EnableRevocationChecking`** (new) | **`EnableRevocationChecking`** (new) | |
| **`RevocationCheckProtocol`** | Integer (`RevocationType`) | OCSP | `RevocationCheck` (kept) | `RevocationCheck` (kept) | renamed property `RevocationCheck` → `RevocationCheckProtocol`; the persisted names are contracts and stay |
| **`EmbedRevocationInformation`** | Boolean | True | **`EmbedRevocationInformation`** (new) | **`EmbedRevocationInformation`** (new) | |
| `IsProxyEnabled`, `ProxyServer`, `ProxyPort`, `ProxyAuthMethod`, `ProxyUserName`, `ProxyPassword` | | | | | unchanged |

Enum-backed settings are `Integer` properties, matching the existing provider fields and the
WinForms `SelectedValue` bindings.

Removed properties and CSS constants: `IsTimeStampingEnabled`, `IsDocumentTimeStamp`,
`IsSinglePassPadesBLTA` (CSS suffixes `TimeStampingEnabled`, `DocumentTimeStamp`,
`SinglePassPadesBLTA`). Existing CSS entries with those names are left untouched in Capture and
are never read again.

### 3.3 Loading rules

- **`LoadSetupDataInAdmin` (CSS, admin) and `SetupParser.Parse` (CSS, runtime):** if the
  `PAdESLevel` CSS entry is absent or empty, the whole PDF Signer provider block stays at the
  constructor defaults listed above (including `Enabled = False`). Nothing else is read for the
  block. The other providers and the document-class index settings load as before.
- **`SetupDataFromXml` (backup import):** if the `PAdESLevel` element is absent, the new fields
  are derived by `LegacyLevelDerivation.FromLegacy` (a pure `Shared` function):
  - `IsTimeStampingEnabled = 0` → `BaselineB`
  - `IsTimeStampingEnabled = 1`, `IsDocumentTimeStamp = 0` → `BaselineLT` when the old
    `RevocationCheck` ≠ None, else `BaselineT`
  - `IsTimeStampingEnabled = 1`, `IsDocumentTimeStamp = 1` → `BaselineLTA`
  - `EnableRevocationChecking` = old `RevocationCheck` ≠ None
  - `RevocationCheckProtocol` = old `RevocationCheck`, or OCSP when it was None
  - `EmbedRevocationInformation` = True
- **Rollout consequence:** every customer document class must be exported to XML before the new
  version is installed, then re-imported (or re-entered) in the Setup panel. This goes into the
  installation manual and the release notes.

### 3.4 Validation (`Validate()`), when `Enabled`

1. `SignatureHashMethod` required (existing message).
2. `PAdESLevel` must be one of the four defined values (new message `PAdES_Level_Missing`).
3. B-T / B-LT / B-LTA: `TSAURL` required (existing message).
4. B-LTA: `TimeStampHashMethod` required (existing message).
5. `EnableRevocationChecking = True`: `RevocationCheckProtocol` ≠ None (existing message).
6. B-LT / B-LTA: `EnableRevocationChecking` and `EmbedRevocationInformation` must both be True
   (new message `LTV_Requires_Revocation_Embedding`, replaces `LTA_Signature_Config_Incorrect`).
7. Proxy rules unchanged.

New message keys are added to `Common\Language\Messages.resx` / `.hu.resx` and to the Setup
project's duplicate `Messages.resx` / `.hu.resx`, in English and Hungarian.

## 4. Signing engine (runtime project, `Signature\Modules\SBB\`)

### 4.1 Files

| File | Responsibility |
|---|---|
| `SbbLicense.vb` | Creates licensed `PDFSigner`, `PDFVerifier`, `CertificateValidator`, `CertificateManager` (`RuntimeLicense = Key`). Holds the key constant. |
| `SigningPlan.vb` | Immutable description of the passes: `SignatureLevel`, `EmbedSignatureTimestamp`, `UpdateToEmbed`, `AddDocumentTimestamp`, `EmbedDocumentTimestampRevocation`, `Revocation`, `SignPassRevocationCheck`, `CheckSigningCertRevocation`, `CheckTimestampCertRevocation`. |
| `SigningPlanBuilder.vb` | Pure `PDFSignerCryptoProvider` → `SigningPlan` mapping (§4.3). |
| `SbbTranslator.vb` | `HashType` → SBB hash name; `RevocationType` → `PDFSignerRevocationChecks` / `CertificateValidatorRevocationChecks`; `ProxyAuthenticationMethod` → `ProxyAuthTypes`; SBB validity/detail enums → Hungarian. |
| `SbbErrorTranslator.vb` | SBB `OnError` code + description and exception messages → Hungarian log lines; OCSP (2001-2005) and CRL (1001-1005) sub-codes; `StripComponentPrefix`, `StripUserInfo`. |
| `SbbEventLogger.vb` | Attaches `PDFSigner` and `CertificateValidator` events (`OnError`, `OnChainElementDownload`, `OnChainElementNeeded`, `OnChainValidated`, `OnChainValidationProgress`, `OnTimestampRequest`, `OnTimestampValidated`, `OnTLSCertValidate`, `OnBeforeCertificateValidation`, `OnAfterCertificateValidation`) to the signature log. No endpoint limiting. |
| `TsaUrlBuilder.vb` | Embeds TSA credentials as URL user-info (`OnTimestampRequest` is not raised for HTTP(S) in this build). |
| `DocumentCertificateHarvester.vb` | Parse-only `PDFVerifier` pass returning the certificates, CRLs and OCSP responses currently carried by the document, plus the last signature's `EntityLabel`. |
| `DerReader.vb` | Minimal DER reader for the two certificate extensions the module inspects (QC statements, authority information access). |
| `QualifiedCertificateDetector.vb` | Decides whether a certificate carries the ETSI QcCompliance statement, i.e. is a qualified certificate. |
| `SigningCertificatePrecheck.vb` | Offline checks of the operator's signing certificate before any SecureBlackbox pass; every failure is a Hungarian operator message. |
| `ChainContextTracker.vb` | Follows the certificate under validation from chain events so an error event, which names no certificate, can be attributed to it. |
| `TlsHandshakeScope.vb` | Tracks nested TLS handshakes so chain events during a pending handshake are demoted to trace in the signature log. |
| `EntityNaming.vb` | Display ids of a document's signing entities for the log: signatures S0, S1…, document timestamps T0, T1…, a signature's own timestamps S0T0…. |
| `WindowsCertificateStores.vb` | Trust anchors and intermediate certificates of the Windows stores as SecureBlackbox lists, loaded once per signing call and copied per pass (§4.2). |
| `Helper\MomentFormat.vb` | Renders moments for the signature log: local time first, the UTC instant in parentheses. |
| `EtsiValidity\ValidityModels.vb`, `AlgorithmSunsetTable.vb`, `EtsiValidityInputBuilder.vb`, `EtsiValidityCalculator.vb` | Port of PDF Streamer's ETSI EN 319 102-1 end-of-validity calculation; yields `ValidUntil` and `EvidenceValidUntil`. |
| `SbbSignatureCreator.vb` | The orchestrator. Public surface unchanged from `SBBPDF`: `Initialize(PDFSignerCryptoProvider)`, `ActivateLicense()`, `GetCertificatesFromStore(QualifiedCertificatesOnly)`, `SignDocument(SignatureRequest) As SignatureResult`. Not `IDisposable`: it holds no unmanaged or disposable state of its own. |
| `SigningBufferFactory.vb` | Chooses the working-copy/pass-output buffer: `MemoryTributary`, or a self-deleting temp `FileStream` for large documents (§4.4). |
| kept: `CertificateTranslator.vb`, `ServerCertificateValidator.vb` | unchanged |
| deleted: `SBBPDF.vb`, `SBBCodeTranslator.vb` | legacy engine and its `TEl*` code tables |

`SignatureOperation.vb` swaps `New SBBPDF` for `New SbbSignatureCreator`; nothing else in the
dispatch changes.

### 4.2 Certificate handling

- `GetCertificatesFromStore` enumerates `CurrentUser\My` with `X509Store`, filtering on the
  validity window, `DigitalSignature Or NonRepudiation` key usage, and (when requested) the
  qualified-certificate statement, decided by parsing the QC Statements extension
  (OID 1.3.6.1.5.5.7.1.3) of the `X509Certificate2` for the `QcCompliance` statement
  (OID 0.4.0.1862.1.1) in a small pure helper `QualifiedCertificateDetector` (unit-tested; no
  SecureBlackbox dependency). Returns `SigningCertificate` items as today.
- At sign time the picked `X509Certificate2` is imported with `CertificateManager.ImportFromObject`
  and assigned to `PDFSigner.SigningCertificate`.
- Offline pre-checks on the picked certificate, with the existing Hungarian messages: not yet
  valid, expired, no private key, wrong key usage, incomplete chain (`X509Chain`, no revocation),
  and no AIA OCSP responder when the protocol is OCSP.
- `TrustedCertificates` = Windows `Root` (CurrentUser + LocalMachine), loaded once per
  `SignDocument` call. `KnownCertificates` for every pass = Windows `CA` (CurrentUser +
  LocalMachine) + the certificates harvested from the document at that point (§4.1
  `DocumentCertificateHarvester`), so the Update and archive-timestamp passes see the TSA chain
  even when it is not installed in Windows. Harvested CRLs and OCSP responses are passed as
  `KnownCRLs` / `KnownOCSPs` so a later pass never re-downloads what an earlier pass embedded.
- Each pass gets its own `CertificateList` copies of `TrustedCertificates` and the Windows-store
  part of `KnownCertificates` (`WindowsCertificateStores.CopyOf`), because SecureBlackbox consumes
  a list assigned to one component.

### 4.3 Signing plan

| Level | Sign pass | Signing-chain check | Update to embed | Archive timestamp | Archive TS revocation |
|---|---|---|---|---|---|
| B-B | `paslBaselineB`, `RevocationCheck` = protocol when checking on, no TSA | validator pre-check when checking on | no | no | – |
| B-T | `paslBaselineT`, `RevocationCheck` = protocol when checking on, TSA | validator pre-check when checking on; TSA-chain post-check when checking on | no | no | – |
| B-LT | `paslBaselineT`, `RevocationCheck` = `crcNone`, TSA | in the Update pass | yes | no | – |
| B-LTA | `paslBaselineT`, `RevocationCheck` = `crcNone`, TSA | in the Update pass | yes | yes, lean | follow-up `Update` on the timestamp when checking on |

`Revocation` = protocol when `EnableRevocationChecking`, else `crcNone`. B-LT / B-LTA always
embed (validation guarantees the switches are on); B-B / B-T never embed.

### 4.4 `SignDocument` sequence

1. Build the plan; run the offline certificate pre-checks; load the stores.
2. Probe the input with `PDFVerifier.Open` (no validation): an encrypted document or one that
   already has signatures or document timestamps fails with a dedicated Hungarian message. The
   provider handles unsigned input only.
3. **Signing-chain pre-check** (B-B / B-T, checking on): `CertificateValidator` over the signing
   certificate with `RevocationCheck` = protocol, trusted roots, known certificates and proxy.
   `ChainValidationResult` must be `cvtValid`, else the call fails with the translated details.
4. **Sign pass.** `SigningCertificate`, `NewSignature.Level`, `NewSignature.HashAlgorithm`,
   `AuthorName` (= `SigningOrganization`), `Reason`, `Widget.Invisible = True`,
   `RevocationCheck` = `SignPassRevocationCheck`, `OfflineMode = False`,
   `IgnoreChainValidationErrors = False`, `TimestampServer` when `EmbedSignatureTimestamp`,
   proxy settings, then `Sign()`. When the document class sets a signature policy URL, the sign
   pass applies that policy (`NewSignature.PolicyID`, `PolicyHash` as hex converted from the stored
   base64, `PolicyHashAlgorithm` = `SignatureHashMethod`, `PolicyURI`); PDFSigner offers no
   commitment-type setting.
4a. **TSA-chain post-check** (B-T, checking on): the same validator check over the TSA
   certificate harvested from the fresh signature timestamp.
5. **Update pass** (B-LT / B-LTA): `RevocationCheck` = `Revocation`,
   `Config("AutoCollectRevocationInfo=true")`, `Config("IncludeRevocationInfoToAdbeAttribute=true")`,
   `Config("CollectRevInfoForTimestamps=true")`, no `TimestampServer`, `Update(lastEntityLabel)`.
6. **Archive timestamp pass** (B-LTA): `NewSignature.SignatureType = pstDocumentTimestamp`,
   `NewSignature.HashAlgorithm` = `TimeStampHashMethod`, `Widget.Invisible = True`,
   `TimestampServer`, `RevocationCheck = crcNone`, `Sign()`. Then, when checking is on, a further
   Update pass on the new timestamp's entity label with the same collection config, because
   SecureBlackbox v24 never embeds a document timestamp's own revocation in the creating pass.
7. **Validity.** `EtsiValidityCalculator` over an offline `PDFVerifier` pass; `ValidUntil` goes to
   `SignatureResult.SignatureExpiration` (feeds the `IndexSignatureValidUntil` index field);
   `EvidenceValidUntil` is written to the log only.
8. Return `SignedFile`; on any failure return `ErrorMessage` plus the accumulated log.

Every pass runs once; there is no lean/full retry. `SigningBufferFactory` backs the working copy
and every pass output with a self-deleting temp `FileStream` (`FileOptions.DeleteOnClose`, in
`Path.GetTempPath()`) when the input is a `FileStream` or longer than
`Constant.FileStreamSizeLimit`, and with `MemoryTributary` otherwise.

### 4.5 Errors and logging

Failures set `SignatureResult.ErrorMessage` (Hungarian, operator-facing) and append to the
signature log returned in `SignatureResult.SignatureLog`, which `FrmPdfSigner` shows and stores in
the `SignatureLogContent` CSS. SBB errors are translated by `SbbErrorTranslator` into
`<wrapper text> <sub-code text> <certificate CN> <responder URL without user-info> [SBB <code> /
OCSP|CRL <sub>]`. Unexpected exceptions are logged through NLog and reported with
`ExceptionText`. Log and error texts stay Hungarian string literals in the module, matching the
other modules. `SbbErrorTranslator.ScrubUrlCredentials` strips `user:password@` from any URL
found in an SBB error description or exception text before it is logged.

## 5. Setup UI (`PDF Signer for Tungsten Capture Setup\FrmSetup`)

Plain WinForms, English captions in `FrmSetup.resx`, bindings through the existing
`BsPDFSignerCryptoProvider`.

- Removed: `ChkTimeStampingEnabled`, `ChkIsDocumentTimeStamp`, `ChkSinglePassPadesLTA`.
- Added `LblPAdESLevel` + `ComboPAdESLevel` ("PAdES level": B-B, B-T, B-LT, B-LTA) above the
  timestamp group; filled by `ComboFiller.FillComboWithPAdESLevels`, bound to `PAdESLevel` via
  `SelectedValue`.
- `GrpTimeStamp` keeps `TxtTSAURL`, `TxtTSAUser`, `TxtTSAPassword`, `ComboTimeStampHashMethod`.
  Enabled for B-T / B-LT / B-LTA; the hash combo is enabled for B-LTA only.
- New `GrpRevocation` ("Revocation checking") holds `ChkEnableRevocationChecking`,
  `ComboRevocationCheckProtocol` (the existing revocation combo, relabelled "Protocol", bound to
  `RevocationCheckProtocol`) and `ChkEmbedRevocationInformation` ("Embed revocation information
  into the document"). Protocol and embed are enabled only while checking is on. Selecting B-LT
  or B-LTA sets both switches on and disables the two checkboxes.
- Enable/disable logic runs from the level combo's `SelectedValueChanged` and the checking
  checkbox's `CheckedChanged`, and once after binding in `FrmSetup_Load` and after import/reset.
- The dirty-check snapshot (`Setup.GetValueSnapshot`, built from `SetupDataToXml`) needs no
  change beyond the provider's XML fields.
- Tab order is set explicitly for the new controls.

## 6. Runtime integration

- `SignatureOperation.vb`: instantiate `SbbSignatureCreator`; `ActivateSBBLicense` keeps calling
  `SBUtils.Unit.SetLicenseKey` for the legacy API and no longer registers the legacy CRL/OCSP
  factories.
- `FrmPdfSigner.vb`: no logic change; the qualified-only filter and the certificate requirement
  keep working through the unchanged provider properties.
- `MetaDataModel.vb` and the WFA project: no change (they do not read the affected fields).
- `SignatureResult`: no shape change.
- `SignatureRequest.MetaDataSettings` supplies the document class's signature policy (URL, base64
  hash, OID) that the sign pass applies.

## 7. Tests

New project `PDF Signer for Tungsten Capture Tests` (`PDFSignerTests`, MSTest, net48,
`<IsTestProject>true`), referencing the runtime and Common projects; both expose internals with
`InternalsVisibleTo("PDFSignerTests")`. Not part of the installer; `CopyToCaptureBin` is not
defined for it. Added to the slnx.

Covered:

- `SigningPlanBuilder`: the four levels × `EnableRevocationChecking` on/off (embedding is fixed
  by validation), asserting every plan flag.
- `PDFSignerCryptoProvider.DeriveFromLegacyXml`: all legacy combinations of the three booleans ×
  revocation None/other.
- `PDFSignerCryptoProvider.Validate`: the rules in §3.4.
- `SbbTranslator`: hash names, revocation and proxy mappings, unsupported hash rejection.
- `SbbErrorTranslator`: OCSP/CRL sub-codes, wrapper codes, prefix and user-info stripping (cases
  ported from PDF Streamer's `SbbErrorTranslatorTests`).
- `EtsiValidityCalculator`: cases ported from PDF Streamer's `EtsiValidityCalculatorTests`.

Manual verification before merge: sign one unsigned document at each of the four levels against
a real TSA with revocation checking on, open each in Adobe Reader and confirm the expected level
(signature panel: timestamp present, LTV enabled, document timestamp present), plus one B-B run
with checking off, one run with a revoked or untrusted certificate (expect a clear error), and
one signed input (expect the "unsigned only" error).

## 8. Documentation and housekeeping

- `Help\Content\Modules\Administration\SetupDocumentClass.htm`: replace the timestamp checkbox
  descriptions with the PAdES level and revocation group; refresh the screenshot.
- Installation manual notes (`Dokumentaciok\`): export-before-upgrade requirement.
- `CLAUDE.md`: test project, "unsigned input only" rule, SBB build 9710, `RuntimeLicense`.
- Version bump in the runtime, Common and Setup vbproj files.
- `lib\nsoftware.SecureBlackbox.dll` replaced by the 24.0.9710 build copied from
  `C:\Projects\PDF Streamer\lib\`.

## 9. Out of scope

Existing-signature handling, visible signature appearance, PFX or smart-card certificate sources
beyond the Windows store, a Hungarian Setup UI, changes to the other three providers, and any
change to PDF Streamer.
