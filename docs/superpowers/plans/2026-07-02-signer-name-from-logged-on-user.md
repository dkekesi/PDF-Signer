# Signer Name From Logged-On User Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a boolean option to the PDF Signer (SBB) provider that, when checked, puts the logged-in operator's human display name (instead of the signing certificate's issued-to name) into the certification clause.

**Architecture:** A new `SignerNameFromLoggedOnUser` boolean flows through the existing per-batch-class configuration pipeline (CSS key → admin load/save → XML export/import → runtime `SetupParser` → data-bound checkbox on the PDF Signer setup panel). At runtime, one branch in `SignatureOperation.SignDocument` prefers `UserDisplayName` over the certificate name when the flag is set. Spec: `docs/superpowers/specs/2026-07-02-signer-name-from-logged-on-user-design.md`.

**Tech Stack:** VB.NET, .NET Framework 4.8, WinForms (localizable form — layout lives in `FrmSetup.resx`), MSBuild.

**Verification model:** This repo has no automated tests or linters (see CLAUDE.md); verification is a clean build plus a manual smoke test. Build command used throughout (from the repo root, paths contain spaces — keep the quotes):

```
msbuild "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false
```

Expected: build succeeds with `0 Error(s)` in the output. (MSBuild lives at `c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\` if it is not on PATH.)

**Not in this plan:** Online-help update (manual Help & Manual authoring step) and vbproj version bumps (release-time step).

---

### Task 1: Configuration plumbing (Common project)

Add the property and wire it through all persistence stations. All files are in `PDF Signer for Tungsten Capture Common\`.

**Files:**
- Modify: `PDF Signer for Tungsten Capture Common\Models\CSS.vb:58`
- Modify: `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFSignerCryptoProvider.vb` (property, admin load/save, XML export/import)
- Modify: `PDF Signer for Tungsten Capture Common\Parser\SetupParser.vb:80`

- [ ] **Step 1: Add the CSS key constant**

In `Models\CSS.vb`, after the `AllowQualifiedCertificatesOnly` constant (line 58), add:

```vb
    Public Const SignerNameFromLoggedOnUser As String = BaseNamespace + "SignerNameFromLoggedOnUser"
```

Result (context):

```vb
    Public Const AllowQualifiedCertificatesOnly As String = BaseNamespace + "AllowQualifiedCertificatesOnly"
    Public Const SignerNameFromLoggedOnUser As String = BaseNamespace + "SignerNameFromLoggedOnUser"
```

- [ ] **Step 2: Add the property to the provider model**

In `Models\Setup\Providers\PDFSignerCryptoProvider.vb`, after the `AllowQualifiedCertificatesOnly` property (line 20), add:

```vb
    Public Property SignerNameFromLoggedOnUser As Boolean
```

Result (context):

```vb
    Public Property AllowQualifiedCertificatesOnly As Boolean
    Public Property SignerNameFromLoggedOnUser As Boolean
    Public Property TSAURL As Uri
```

No constructor or `Validate()` change: the default `False` preserves today's behavior, and a bool needs no validation.

- [ ] **Step 3: Wire admin load**

In the same file, in `LoadSetupDataInAdmin`, after the `AllowQualifiedCertificatesOnly` line (line 100), add:

```vb
            SignerNameFromLoggedOnUser = Converter.StringToBoolean(.ReadSetupCSS(CSS.SignerNameFromLoggedOnUser))
```

- [ ] **Step 4: Wire admin save**

In `SaveSetupDataInAdmin`, after the `AllowQualifiedCertificatesOnly` line (line 126), add:

```vb
            .WriteSetupCSS(CSS.SignerNameFromLoggedOnUser, Converter.BooleanToNumericString(SignerNameFromLoggedOnUser))
```

- [ ] **Step 5: Wire XML export**

In `SetupDataToXml`, after the `AllowQualifiedCertificatesOnly` element (line 156), add:

```vb
                New XElement("SignerNameFromLoggedOnUser", Converter.BooleanToNumericString(SignerNameFromLoggedOnUser)),
```

- [ ] **Step 6: Wire XML import**

In `SetupDataFromXml`, after the `AllowQualifiedCertificatesOnly` line (line 180), add:

```vb
        SignerNameFromLoggedOnUser = Converter.StringToBoolean(ConfigElement.Element("SignerNameFromLoggedOnUser"))
```

Old exports lacking the element yield `Nothing`, which `Converter.StringToBoolean` maps to `False` — the same pattern the other bools rely on.

- [ ] **Step 7: Wire the runtime setup parser**

In `Parser\SetupParser.vb`, after the `AllowQualifiedCertificatesOnly` line (line 80), add:

```vb
                .PDFSignerProvider.SignerNameFromLoggedOnUser = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignerNameFromLoggedOnUser))
```

- [ ] **Step 8: Build**

Run the Debug build command from the plan header. Expected: `0 Error(s)`.

- [ ] **Step 9: Commit**

```bash
git add "PDF Signer for Tungsten Capture Common/Models/CSS.vb" "PDF Signer for Tungsten Capture Common/Models/Setup/Providers/PDFSignerCryptoProvider.vb" "PDF Signer for Tungsten Capture Common/Parser/SetupParser.vb"
git commit -m "Add SignerNameFromLoggedOnUser option to the PDF Signer provider config"
```

---

### Task 2: Runtime name selection (main module)

**Files:**
- Modify: `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb:42-46`

- [ ] **Step 1: Change the name-source branch**

In `SignatureOperation.vb`, `SignDocument`, replace:

```vb
                If SigningCertificate IsNot Nothing Then
                    .MetaDataSettings.SignerFullName = SigningCertificate.GetIssuedToName
                Else
                    .MetaDataSettings.SignerFullName = UserDisplayName
                End If
```

with:

```vb
                Dim signerProvider = TryCast(CryptographicProviderSettings, PDFSignerCryptoProvider)
                If SigningCertificate IsNot Nothing AndAlso
                   Not (signerProvider IsNot Nothing AndAlso signerProvider.SignerNameFromLoggedOnUser) Then
                    .MetaDataSettings.SignerFullName = SigningCertificate.GetIssuedToName
                Else
                    .MetaDataSettings.SignerFullName = UserDisplayName
                End If
```

Behavior: certificate name only when a certificate is present **and** the provider is not a `PDFSignerCryptoProvider` with the flag set; otherwise the logged-in user's display name. If the flag is on and `UserDisplayName` is empty, the existing guard a few lines below (`SignatureOperation.vb:51`) fails the operation with the localized `Exception_Signer_Name_Not_Provided` message — that is the agreed error behavior, no new code needed.

- [ ] **Step 2: Build**

Run the Debug build command from the plan header. Expected: `0 Error(s)`.

- [ ] **Step 3: Commit**

```bash
git add "PDF Signer for Tungsten Capture/Signature/SignatureOperation.vb"
git commit -m "Use logged-on user's name in the clause when the PDF Signer option is set"
```

---

### Task 3: Setup UI checkbox (Setup project)

`FrmSetup` is a localizable WinForms form: control properties (location, size, text) live in `FrmSetup.resx` and are applied via `resources.ApplyResources`. Both files are hand-edited; no Visual Studio designer needed.

**Placement note (small deviation from the spec):** the spec says "directly below `ChkAllowQualifiedCertificatesOnly`", but the panel has no free row there — `GrpTimeStamp` starts at y=95, only 23px under the checkbox at y=72. The new checkbox goes **to the right on the same row** (x=366, the right-hand column where Label9/Label11 start), as a two-line wrapped checkbox so the full label fits. Step 5 updates the spec wording to match.

**Files:**
- Modify: `PDF Signer for Tungsten Capture Setup\FrmSetup.Designer.vb` (4 edits)
- Modify: `PDF Signer for Tungsten Capture Setup\FrmSetup.resx` (1 block)
- Modify: `docs\superpowers\specs\2026-07-02-signer-name-from-logged-on-user-design.md` (placement wording)

- [ ] **Step 1: Designer — instantiate the checkbox**

In `FrmSetup.Designer.vb`, after line 113 (`Me.ChkAllowQualifiedCertificatesOnly = New System.Windows.Forms.CheckBox()`), add:

```vb
        Me.ChkSignerNameFromLoggedOnUser = New System.Windows.Forms.CheckBox()
```

- [ ] **Step 2: Designer — add to panel, configure, declare**

Three edits in `FrmSetup.Designer.vb`:

(a) In the `'PanelPDFSigner'` block, after `Me.PanelPDFSigner.Controls.Add(Me.Label8)` (line 836), add:

```vb
        Me.PanelPDFSigner.Controls.Add(Me.ChkSignerNameFromLoggedOnUser)
```

(b) After the `ChkAllowQualifiedCertificatesOnly` configuration block (ends line 845 with `Me.ChkAllowQualifiedCertificatesOnly.UseVisualStyleBackColor = True`), add:

```vb
        '
        'ChkSignerNameFromLoggedOnUser
        '
        resources.ApplyResources(Me.ChkSignerNameFromLoggedOnUser, "ChkSignerNameFromLoggedOnUser")
        Me.ChkSignerNameFromLoggedOnUser.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "SignerNameFromLoggedOnUser", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkSignerNameFromLoggedOnUser.Name = "ChkSignerNameFromLoggedOnUser"
        Me.ChkSignerNameFromLoggedOnUser.UseVisualStyleBackColor = True
```

(c) In the field declarations at the bottom, next to `Private WithEvents ChkAllowQualifiedCertificatesOnly As CheckBox` (line 1609), add:

```vb
    Private WithEvents ChkSignerNameFromLoggedOnUser As CheckBox
```

- [ ] **Step 3: Resx — add the control's resources**

In `FrmSetup.resx`, immediately after the `&gt;&gt;ChkAllowQualifiedCertificatesOnly.ZOrder` data element (line 2498), add:

```xml
  <data name="ChkSignerNameFromLoggedOnUser.Location" type="System.Drawing.Point, System.Drawing">
    <value>366, 61</value>
  </data>
  <data name="ChkSignerNameFromLoggedOnUser.Size" type="System.Drawing.Size, System.Drawing">
    <value>253, 32</value>
  </data>
  <data name="ChkSignerNameFromLoggedOnUser.TabIndex" type="System.Int32, mscorlib">
    <value>9</value>
  </data>
  <data name="ChkSignerNameFromLoggedOnUser.Text" xml:space="preserve">
    <value>Use the logged-in user's name in the certification clause instead of the certificate name</value>
  </data>
  <data name="&gt;&gt;ChkSignerNameFromLoggedOnUser.Name" xml:space="preserve">
    <value>ChkSignerNameFromLoggedOnUser</value>
  </data>
  <data name="&gt;&gt;ChkSignerNameFromLoggedOnUser.Type" xml:space="preserve">
    <value>System.Windows.Forms.CheckBox, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </data>
  <data name="&gt;&gt;ChkSignerNameFromLoggedOnUser.Parent" xml:space="preserve">
    <value>PanelPDFSigner</value>
  </data>
  <data name="&gt;&gt;ChkSignerNameFromLoggedOnUser.ZOrder" xml:space="preserve">
    <value>13</value>
  </data>
```

Notes: no `AutoSize` entry — the code default (`False`) plus the explicit 253×32 size makes the 90-character label wrap to two lines, ending at y=93, 2px above `GrpTimeStamp` (y=95). ZOrder 13 = appended after the panel's existing 13 controls (0–12); it is designer metadata only and nothing overlaps.

- [ ] **Step 4: Build**

Run the Debug build command from the plan header. Expected: `0 Error(s)`.

- [ ] **Step 5: Update the spec's placement sentence**

In `docs\superpowers\specs\2026-07-02-signer-name-from-logged-on-user-design.md`, section 2, replace:

```
placed directly below `ChkAllowQualifiedCertificatesOnly`, data-bound to
```

with:

```
placed to the right of `ChkAllowQualifiedCertificatesOnly` on the same row (the panel
has no free row below it — the time-stamp group starts 23px lower), data-bound to
```

- [ ] **Step 6: Commit**

```bash
git add "PDF Signer for Tungsten Capture Setup/FrmSetup.Designer.vb" "PDF Signer for Tungsten Capture Setup/FrmSetup.resx" "docs/superpowers/specs/2026-07-02-signer-name-from-logged-on-user-design.md"
git commit -m "Add signer-name-from-logged-on-user checkbox to the PDF Signer setup panel"
```

---

### Task 4: Final verification

- [ ] **Step 1: Clean Debug build of the whole solution**

Run the Debug build command from the plan header. Expected: `0 Error(s)`.

- [ ] **Step 2: Manual smoke test (user-executed — needs an installed Capture)**

Report these to the user as their acceptance checklist; they cannot be automated here:

1. **Setup round-trip:** In Capture Administration, open the PDF Signer setup panel for a document class, check the new box, save, reopen — the box stays checked.
2. **XML export/import round-trip:** Export the configuration, re-import it — the value survives. Import a pre-change export — the box is unchecked.
3. **Clause with flag off:** Sign a document with the PDF Signer (SBB) provider — the clause's "A másolat képi vagy tartalmi egyezéséért felelős személy neve:" line shows the certificate's issued-to name (unchanged behavior).
4. **Clause with flag on:** Same, with the box checked — the line shows the logged-in user's AD display name.
5. **UI sanity:** The new checkbox renders fully (two lines, not clipped) and enables/disables together with the rest of the PDF Signer panel.
