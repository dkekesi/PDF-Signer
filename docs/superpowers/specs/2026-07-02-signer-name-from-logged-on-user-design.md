# Signer name from logged-on user in the certification clause — design

**Date:** 2026-07-02
**Status:** Approved

## Goal

Add a boolean option to the PDF Signer (SBB) signing provider. When checked, the
certification clause ("Hitelesítési záradék") names the logged-in operator (their
human display name, not their login name) as the person responsible for the copy,
instead of the name taken from the signing certificate.

## Background

The clause's responsible-person name comes from `MetaData.SignerFullName`, set in
`SignatureOperation.SignDocument` ("PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb").
Today the rule is:

- a signing certificate is present (only the PDF Signer/SBB provider selects a local
  certificate) → `SigningCertificate.GetIssuedToName`
- no certificate (all other providers) → `UserDisplayName`

`UserDisplayName` is already the human name: `FrmPdfSigner` reads
`System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName` at form
load and passes it to every `SignDocument` call. All three clause formats (text, XML,
Dublin Core) render `SignerFullName`, so they all pick the change up automatically.

## Design

### 1. Data model & persistence (PDF Signer for Tungsten Capture Common)

New property on `PDFSignerCryptoProvider`:

```vb
Public Property SignerNameFromLoggedOnUser As Boolean
```

Default `False` — existing batch classes read the missing CSS value as false and keep
today's behavior. Wired through the standard persistence stations (the fifth station,
the setup UI checkbox, is section 2):

| Station | Change |
|---|---|
| `Models\CSS.vb` | new const `SignerNameFromLoggedOnUser` (`BaseNamespace`-prefixed, like `AllowQualifiedCertificatesOnly`) |
| `LoadSetupDataInAdmin` / `SaveSetupDataInAdmin` | read/write with `Converter.StringToBoolean` / `Converter.BooleanToNumericString`, same as the other bools |
| `SetupDataToXml` / `SetupDataFromXml` | new `SignerNameFromLoggedOnUser` element; old exports lacking the element deserialize as `False` |
| `Parser\SetupParser.vb` | runtime read into `PDFSignerProvider`, next to the `AllowQualifiedCertificatesOnly` line |

No `Validate()` change — a bool needs no validation.

### 2. Setup UI (PDF Signer for Tungsten Capture Setup)

New `CheckBox` `ChkSignerNameFromLoggedOnUser` on `PanelPDFSigner` in `FrmSetup`,
placed to the right of `ChkAllowQualifiedCertificatesOnly` on the same row (the panel
has no free row below it — the time-stamp group starts 23px lower), data-bound to
`BsPDFSignerCryptoProvider` property `SignerNameFromLoggedOnUser` with
`DataSourceUpdateMode.OnPropertyChanged` (same binding style as its neighbor).

Label text in `FrmSetup.resx` (the setup form is English-only, no `hu` satellite):

> Use the logged-in user's name in the clause instead of the certificate name

### 3. Runtime (PDF Signer for Tungsten Capture)

Single change in `SignatureOperation.SignDocument`: use the certificate's issued-to
name only when a certificate is present **and** the provider is not a
`PDFSignerCryptoProvider` with the new flag set; otherwise use `UserDisplayName`:

```vb
Dim signerProvider = TryCast(CryptographicProviderSettings, PDFSignerCryptoProvider)
If SigningCertificate IsNot Nothing AndAlso
   Not (signerProvider IsNot Nothing AndAlso signerProvider.SignerNameFromLoggedOnUser) Then
    .MetaDataSettings.SignerFullName = SigningCertificate.GetIssuedToName
Else
    .MetaDataSettings.SignerFullName = UserDisplayName
End If
```

No method-signature or form changes: `UserDisplayName` already reaches this method for
every provider.

### 4. Error handling

If the flag is on but the AD display name is empty (e.g., a local account with no full
name), the existing empty-name guard in `SignDocument` fails the operation with the
localized `Exception_Signer_Name_Not_Provided` message. This is the agreed behavior:
the option exists for compliance, so silently substituting the certificate name would
put the wrong name in a legal clause. No new code or message resources are needed.

### 5. Out of scope

- Online help (Help & Manual project): needs a sentence about the new checkbox —
  manual authoring step, not part of the code change.
- The cryptographic signature and its visible appearance are untouched; only the
  clause text changes.
- Other providers (MNB, MQFTP, PDF Streamer) already use the logged-in user's name
  because they select no local certificate; they get no new option.

## Verification

No automated tests exist in this repo; verification is:

1. Clean build (`msbuild "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false` — 0 errors).
2. Manual smoke test:
   - Setup: check the box, save, reopen — value round-trips.
   - Setup: XML export → import — value round-trips; importing an old export leaves it unchecked.
   - Runtime: sign with the flag off → clause shows the certificate name (unchanged behavior).
   - Runtime: sign with the flag on → clause shows the logged-in user's display name.
