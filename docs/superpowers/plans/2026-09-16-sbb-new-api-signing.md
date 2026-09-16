# Built-in PDF Signer provider on the SecureBlackbox v24 component API — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the legacy `TEl*`-based built-in signing module with a port of PDF Streamer's core signing pipeline on the SecureBlackbox v24 `PDFSigner`/`PDFVerifier`/`CertificateValidator` components, with a PAdES-level + revocation-switch configuration model and a matching Setup UI.

**Architecture:** The Common project gets the new configuration fields (integer-backed like the existing ones) and the XML-import-only legacy derivation. The runtime project's `Signature\Modules\SBB\` folder is rebuilt from focused units: pure translators and a signing-plan builder, an event logger, a certificate harvester, an ETSI validity calculator, and one orchestrator (`SbbSignatureCreator`) that keeps the old module entry points so `SignatureOperation` barely changes. A new MSTest project covers every pure unit; SBB-touching code is verified by build and a manual signing matrix.

**Tech Stack:** VB.NET on .NET Framework 4.8 (SDK-style projects, Option Strict Off; the runtime project has **Option Infer Off**, so every `Dim` there must carry an explicit type), nsoftware SecureBlackbox 24.0.9710 (`lib\nsoftware.SecureBlackbox.dll`), NLog, WinForms (plain, no DevExpress in Setup), MSTest 3.x, MSBuild from Visual Studio 18, `vstest.console.exe`.

**Spec:** `docs/superpowers/specs/2026-09-16-sbb-new-api-signing-design.md`

## Global Constraints

- Worktree: all work happens in `C:\Projects\PDF Signer\.claude\worktrees\sbb-new-api` on branch `worktree-sbb-new-api`. Quote every path (they contain spaces). Never run git against the main checkout.
- Build command (used as "the build" everywhere below):
  `& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false /v:m`
  Success = output ends with `0 Error(s)`.
- Test command (used as "the tests" everywhere below), run after the build:
  `& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "PDF Signer for Tungsten Capture Tests\bin\Debug\PDFSignerTests.dll" /Platform:x86`
- English identifiers only (global rule 1). Operator-facing log/error text in the runtime module stays **Hungarian** string literals, as in the other modules. Setup UI captions stay English. Validation messages go into the resx files in English and Hungarian.
- Comments describe the present only (global rule 2): no "previously", "replaced", "legacy" narration in code comments. Doc comments on every public/Friend type and member. Comments at most 2 lines.
- Persisted names are contracts: CSS suffix `RevocationCheck` and XML element `RevocationCheck` keep their names even though the property is renamed `RevocationCheckProtocol`.
- The provider signs **unsigned input only**; existing-signature handling, PKI-cache config, known-certificate learning, endpoint limiting and readiness are out of scope.
- Every pass runs once; there is no lean/full retry.
- Spec deviations locked in by this plan (update the spec in Task 16): the enum is named `PAdESLevelType` (the property is `PAdESLevel`, so the enum cannot share the name inside the class) and the provider property is `Integer`-typed like `RevocationCheckProtocol`/`SignatureHashMethod`, because WinForms `SelectedValue` binding to `ComboKeyValue.Id` needs an integer target.
- The runtime project's root namespace is `PDFSigner`, which collides with the nsoftware `PDFSigner` component class inside that project. Every runtime file that uses the component declares `Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner` and writes the type as `SbbPdfSigner`; the event-args types (`PDFSignerErrorEventArgs` …) and the test project (namespace `PDFSignerTests`) are unaffected.
- Commit after every task with a message in the imperative mood and the trailer `Co-Authored-By: Claude Fable 5.1 <noreply@anthropic.com>`.

---

## File structure

| Path (relative to repo root) | Status | Responsibility |
|---|---|---|
| `PDF Signer for Tungsten Capture Tests\PDF Signer for Tungsten Capture Tests.vbproj` | create | MSTest net48 x86 test project (`PDFSignerTests`) |
| `PDF Signer for Tungsten Capture Tests\Support\TestPdf.vb` | create | Minimal one-page PDF bytes for SBB smoke tests |
| `PDF Signer for Tungsten Capture Tests\*Tests.vb` | create | One test class per unit (named below per task) |
| `PDF Signer for Tungsten Capture.slnx` | modify | Add the test project, excluded from FullRelease |
| `PDF Signer for Tungsten Capture Common\PDF Signer for Tungsten Capture Common.vbproj`, `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj` | modify | `InternalsVisibleTo("PDFSignerTests")`, version bump |
| `lib\nsoftware.SecureBlackbox.dll` | replace | 24.0.9710 build from `C:\Projects\PDF Streamer\lib\` |
| `PDF Signer for Tungsten Capture Common\Models\Enum.vb` | modify | `PAdESLevelType` enum |
| `PDF Signer for Tungsten Capture Common\Models\CSS.vb` | modify | New CSS constants, three removed |
| `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFSignerCryptoProvider.vb` | rewrite | New fields, load rules, validation |
| `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\LegacyLevelDerivation.vb` | create | Pure derivation used by XML import only |
| `PDF Signer for Tungsten Capture Common\Parser\SetupParser.vb` | modify | Runtime CSS load with the "absent level → defaults" rule |
| `PDF Signer for Tungsten Capture Common\Language\Messages.resx`, `.hu.resx`, `Messages.Designer.vb`; `PDF Signer for Tungsten Capture Setup\Language\Messages.resx`, `.hu.resx`, `Messages.Designer.vb` | modify | Two new validation messages, one removed |
| `PDF Signer for Tungsten Capture Setup\FrmSetup.vb`, `FrmSetup.Designer.vb`, `FrmSetup.resx`, `Helper\ComboFiller.vb` | modify | PAdES level combo, revocation group, enable/disable logic |
| `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbLicense.vb` | create | Licensed component factory |
| `...\SBB\DerReader.vb` | create | Minimal DER TLV reader |
| `...\SBB\QualifiedCertificateDetector.vb` | create | QC Statements parsing |
| `...\SBB\SigningCertificatePrecheck.vb` | create | Offline signing-certificate checks |
| `...\SBB\SbbTranslator.vb` | create | Enum/int → SBB names and Hungarian |
| `...\SBB\ChainContextTracker.vb` | create | `ChainContext` + tracker |
| `...\SBB\SbbErrorTranslator.vb` | create | Error code/description → Hungarian |
| `...\SBB\TsaUrlBuilder.vb` | create | TSA credentials in URL |
| `...\SBB\SigningPlan.vb`, `SigningPlanBuilder.vb` | create | Plan model and pure builder |
| `...\SBB\EntityNaming.vb` | create | S0 / S0T0 / T0 display ids |
| `PDF Signer for Tungsten Capture\Helper\MomentFormat.vb` | create | Local + UTC moment rendering |
| `...\SBB\EtsiValidity\ValidityModels.vb`, `AlgorithmSunsetTable.vb`, `EtsiValidityCalculator.vb`, `EtsiValidityInputBuilder.vb` | create | ETSI end-of-validity |
| `...\SBB\SbbEventLogger.vb` | create | PDFSigner event → log lines |
| `...\SBB\WindowsCertificateStores.vb` | create | Root/CA store lists |
| `...\SBB\DocumentCertificateHarvester.vb` | create | Probe + harvest of the document |
| `...\SBB\SbbSignatureCreator.vb` | create | Orchestrator (module entry points) |
| `...\SBB\SBBPDF.vb`, `...\SBB\SBBCodeTranslator.vb` | delete | Legacy engine |
| `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb` | modify | Instantiate `SbbSignatureCreator` |
| `Help\Content\Modules\Administration\SetupDocumentClass.htm`, `CLAUDE.md`, spec | modify | Documentation |

---

### Task 1: Test project scaffold

**Files:**
- Create: `PDF Signer for Tungsten Capture Tests\PDF Signer for Tungsten Capture Tests.vbproj`
- Create: `PDF Signer for Tungsten Capture Tests\ProjectSmokeTests.vb`
- Modify: `PDF Signer for Tungsten Capture.slnx`
- Modify: `PDF Signer for Tungsten Capture Common\PDF Signer for Tungsten Capture Common.vbproj` (add ItemGroup)
- Modify: `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj` (add ItemGroup)
- Modify: `.gitignore`

**Interfaces:**
- Produces: assembly `PDFSignerTests` (x86, net48) that can see `Friend` members of `PDFSignerCommon` and `PDFSigner`. All later tests live in this project.

- [ ] **Step 1: Create the project file**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <OutputType>Library</OutputType>
    <RootNamespace>PDFSignerTests</RootNamespace>
    <AssemblyName>PDFSignerTests</AssemblyName>
    <IsTestProject>true</IsTestProject>
    <IsPackable>false</IsPackable>
    <LangVersion>latest</LangVersion>
    <OptionStrict>Off</OptionStrict>
    <OptionInfer>On</OptionInfer>
    <Configurations>Debug;Release;FullRelease</Configurations>
    <Platforms>x86</Platforms>
    <PlatformTarget>x86</PlatformTarget>
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
    <NoWarn>41999,42016,42017,42018,42019,42020,42021,42022,42032,42036</NoWarn>
    <!-- Same classic search order as the product projects, so DevExpress/Kofax dependencies of the referenced exe resolve. -->
    <AssemblySearchPaths>{CandidateAssemblyFiles};{HintPathFromItem};{TargetFrameworkDirectory};{Registry:Software\Microsoft\.NETFramework,v4.8,AssemblyFoldersEx};{AssemblyFolders};{GAC};{RawFileName}</AssemblySearchPaths>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.8.3" />
    <PackageReference Include="MSTest.TestFramework" Version="3.8.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\PDF Signer for Tungsten Capture Common\PDF Signer for Tungsten Capture Common.vbproj" />
    <ProjectReference Include="..\PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj" />
    <Reference Include="nsoftware.SecureBlackbox">
      <HintPath>..\lib\nsoftware.SecureBlackbox.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Import Include="Microsoft.VisualStudio.TestTools.UnitTesting" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Write the smoke test**

`PDF Signer for Tungsten Capture Tests\ProjectSmokeTests.vb`:

```vb
Imports PDFSignerCommon

''' <summary>Proves the test project can load both product assemblies and see their Friend types.</summary>
<TestClass>
Public Class ProjectSmokeTests
    <TestMethod>
    Public Sub Common_enum_is_visible()
        Assert.AreEqual(2, CInt(RevocationType.OCSP))
    End Sub

    <TestMethod>
    Public Sub Runtime_friend_type_is_visible()
        Assert.IsNotNull(GetType(PDFSigner.SignatureRequest))
    End Sub
End Class
```

- [ ] **Step 3: Expose internals from both product projects**

Add to `PDF Signer for Tungsten Capture Common\PDF Signer for Tungsten Capture Common.vbproj` and to `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj`, as a new `<ItemGroup>` right before the closing `</Project>`:

```xml
  <ItemGroup>
    <InternalsVisibleTo Include="PDFSignerTests" />
  </ItemGroup>
```

- [ ] **Step 4: Register the project in the solution**

In `PDF Signer for Tungsten Capture.slnx`, add after the WFA `<Project>` element:

```xml
  <Project Path="PDF Signer for Tungsten Capture Tests/PDF Signer for Tungsten Capture Tests.vbproj">
    <BuildDependency Project="PDF Signer for Tungsten Capture/PDF Signer for Tungsten Capture.vbproj" />
    <Platform Project="x86" />
    <Build Solution="FullRelease|*" Project="false" />
  </Project>
```

Append `TestResults/` on its own line to `.gitignore`.

- [ ] **Step 5: Build and run the tests**

Run the build, then the tests. Expected: build `0 Error(s)`; vstest reports `Passed!  - Failed: 0, Passed: 2`. If the runtime exe fails to load in the test host with `BadImageFormatException`, the test assembly is not x86: confirm `<PlatformTarget>x86</PlatformTarget>` and the `/Platform:x86` switch.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Add PDFSignerTests MSTest project and expose internals to it"
```

---

### Task 2: SecureBlackbox 24.0.9710 and the licensed component factory

**Files:**
- Replace: `lib\nsoftware.SecureBlackbox.dll`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbLicense.vb`
- Create: `PDF Signer for Tungsten Capture Tests\Support\TestPdf.vb`
- Create: `PDF Signer for Tungsten Capture Tests\SbbLicenseTests.vb`

**Interfaces:**
- Produces: `Friend Module SbbLicense` with `Key As String`, `CreateSigner() As PDFSigner`, `CreateVerifier() As PDFVerifier`, `CreateCertificateManager() As CertificateManager`, `CreateCertificateValidator() As CertificateValidator`. `Friend Module TestPdf` with `Minimal() As Byte()`.

- [ ] **Step 1: Replace the assembly**

```powershell
Copy-Item "C:\Projects\PDF Streamer\lib\nsoftware.SecureBlackbox.dll" "lib\nsoftware.SecureBlackbox.dll" -Force
[System.Diagnostics.FileVersionInfo]::GetVersionInfo("lib\nsoftware.SecureBlackbox.dll").FileVersion
```
Expected: `24.0.9710.1002`. Run the build; expected `0 Error(s)` (the legacy `TEl*` types used by `SBBPDF.vb` and the pre/post processors exist in this build too).

- [ ] **Step 2: Write the failing license test**

`PDF Signer for Tungsten Capture Tests\Support\TestPdf.vb`:

```vb
Imports System.Text

''' <summary>Builds a minimal, unsigned, one-page PDF in memory for SecureBlackbox smoke tests.</summary>
Friend Module TestPdf
    Friend Function Minimal() As Byte()
        Dim objects As String() = {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] >>"}
        Dim sb As New StringBuilder("%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Integer)
        For i As Integer = 0 To objects.Length - 1
            offsets.Add(sb.Length)
            sb.Append($"{i + 1} 0 obj" & vbLf & objects(i) & vbLf & "endobj" & vbLf)
        Next
        Dim xref As Integer = sb.Length
        sb.Append("xref" & vbLf & $"0 {objects.Length + 1}" & vbLf & "0000000000 65535 f " & vbLf)
        For Each o As Integer In offsets
            sb.Append(o.ToString("D10") & " 00000 n " & vbLf)
        Next
        sb.Append("trailer" & vbLf & $"<< /Size {objects.Length + 1} /Root 1 0 R >>" & vbLf & "startxref" & vbLf & xref & vbLf & "%%EOF" & vbLf)
        Return Encoding.ASCII.GetBytes(sb.ToString())
    End Function
End Module
```

`PDF Signer for Tungsten Capture Tests\SbbLicenseTests.vb`:

```vb
Imports System.IO
Imports nsoftware.SecureBlackbox
Imports PDFSigner

<TestClass>
Public Class SbbLicenseTests
    <TestMethod>
    Public Sub Runtime_license_is_accepted_by_the_verifier()
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            Using doc As New MemoryStream(TestPdf.Minimal())
                verifier.InputStream = doc
                verifier.AutoValidateSignatures = False
                verifier.Open(False)
                Assert.AreEqual(0, verifier.Signatures.Count)
                verifier.Close(False)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Every_factory_returns_a_licensed_component()
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateSigner().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateVerifier().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateCertificateManager().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateCertificateValidator().RuntimeLicense)
    End Sub
End Class
```

- [ ] **Step 3: Run the tests to see them fail**

Build. Expected: compile error `'SbbLicense' is not declared`.

- [ ] **Step 4: Write the factory**

`PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbLicense.vb`:

```vb
Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Creates SecureBlackbox components with the product runtime license applied; each component validates its own RuntimeLicense.</summary>
Friend Module SbbLicense
    ''' <summary>nsoftware runtime key, expires 2026-11-15; the legacy API receives the same key through SBUtils.Unit.SetLicenseKey.</summary>
    Friend Const Key As String = "53424E4A41444E58524632303236313131353159554631393534004955475658465444494C465800303030303030303000004A574A30585037483239325A0000"

    ''' <summary>A licensed PDFSigner.</summary>
    Friend Function CreateSigner() As SbbPdfSigner
        Return New SbbPdfSigner With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed PDFVerifier.</summary>
    Friend Function CreateVerifier() As PDFVerifier
        Return New PDFVerifier With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed CertificateManager.</summary>
    Friend Function CreateCertificateManager() As CertificateManager
        Return New CertificateManager With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed CertificateValidator.</summary>
    Friend Function CreateCertificateValidator() As CertificateValidator
        Return New CertificateValidator With {.RuntimeLicense = Key}
    End Function
End Module
```

- [ ] **Step 5: Run the tests**

Build, then tests. Expected: 4 passed. If `Runtime_license_is_accepted_by_the_verifier` fails with a SecureBlackbox licensing exception (message mentions "license"), replace `Key` with PDF Streamer's key `53424E4A41444E58524632303236313131353159554631393534004B4D455A4444554E45594D5400303030303030303000004A574A30585037483239325A0000` and re-run; note the swap in the commit message.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Upgrade SecureBlackbox to 24.0.9710 and add the licensed component factory"
```

---

### Task 3: Validation message resources

**Files:**
- Modify: `PDF Signer for Tungsten Capture Common\Language\Messages.resx`, `Messages.hu.resx`, `Messages.Designer.vb`
- Modify: `PDF Signer for Tungsten Capture Setup\Language\Messages.resx`, `Messages.hu.resx`, `Messages.Designer.vb`

**Interfaces:**
- Produces: `Messages.PAdES_Level_Missing`, `Messages.LTV_Requires_Revocation_Embedding` in both `PDFSignerCommon.My.Resources.Messages` and `PDFSignerSetup.My.Resources.Messages`. Removes `Messages.LTA_Signature_Config_Incorrect` from both.

- [ ] **Step 1: Add the two entries to all four resx files**

In each `Messages.resx` (English), replace the whole `<data name="LTA_Signature_Config_Incorrect" ...>...</data>` element with:

```xml
  <data name="LTV_Requires_Revocation_Embedding" xml:space="preserve">
    <value>PAdES B-LT and B-LTA require revocation checking and embedding of the revocation information to be enabled.</value>
  </data>
  <data name="PAdES_Level_Missing" xml:space="preserve">
    <value>PAdES level is not selected.</value>
  </data>
```

In each `Messages.hu.resx`, replace the same element with:

```xml
  <data name="LTV_Requires_Revocation_Embedding" xml:space="preserve">
    <value>A PAdES B-LT és B-LTA szintekhez a visszavonás-ellenőrzést és a visszavonási információk beágyazását is be kell kapcsolni.</value>
  </data>
  <data name="PAdES_Level_Missing" xml:space="preserve">
    <value>Nincs kiválasztva PAdES szint.</value>
  </data>
```

- [ ] **Step 2: Update the generated designer classes by hand**

The build does not run the resx custom tool, so edit `Messages.Designer.vb` in both projects: delete the `LTA_Signature_Config_Incorrect` property block (the `'''<summary>` comment lines above it, the `Friend Shared ReadOnly Property ... End Property` block) and insert, in the same style and alphabetical position:

```vb
        '''<summary>
        '''  Looks up a localized string similar to PAdES B-LT and B-LTA require revocation checking and embedding of the revocation information to be enabled..
        '''</summary>
        Friend Shared ReadOnly Property LTV_Requires_Revocation_Embedding() As String
            Get
                Return ResourceManager.GetString("LTV_Requires_Revocation_Embedding", resourceCulture)
            End Get
        End Property

        '''<summary>
        '''  Looks up a localized string similar to PAdES level is not selected..
        '''</summary>
        Friend Shared ReadOnly Property PAdES_Level_Missing() As String
            Get
                Return ResourceManager.GetString("PAdES_Level_Missing", resourceCulture)
            End Get
        End Property
```

Check with `Select-String -Path "*\Language\Messages*.Designer.vb" -Pattern "LTA_Signature_Config_Incorrect"` that no designer file still mentions the old key. (`PDFSignerCryptoProvider.Validate` still references it; that reference is replaced in Task 4, so the build is expected to fail until then — do not build here.)

- [ ] **Step 3: Commit**

```
git add -A
git commit -m "Add PAdES level and LTV revocation validation messages"
```

---

### Task 4: Configuration model, persistence, derivation and validation (Common)

**Files:**
- Modify: `PDF Signer for Tungsten Capture Common\Models\Enum.vb`
- Modify: `PDF Signer for Tungsten Capture Common\Models\CSS.vb:52-68`
- Create: `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\LegacyLevelDerivation.vb`
- Rewrite: `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFSignerCryptoProvider.vb`
- Modify: `PDF Signer for Tungsten Capture Common\Parser\SetupParser.vb:74-95`
- Create: `PDF Signer for Tungsten Capture Tests\LegacyLevelDerivationTests.vb`
- Create: `PDF Signer for Tungsten Capture Tests\PDFSignerCryptoProviderTests.vb`

**Interfaces:**
- Produces: `Public Enum PAdESLevelType { BaselineB = 3, BaselineT = 4, BaselineLT = 5, BaselineLTA = 6 }`; `CSS.PAdESLevel`, `CSS.EnableRevocationChecking`, `CSS.EmbedRevocationInformation`; `PDFSignerCryptoProvider` properties `PAdESLevel As Integer` (default `BaselineB`), `EnableRevocationChecking As Boolean` (False), `RevocationCheckProtocol As Integer` (default `RevocationType.OCSP`), `EmbedRevocationInformation As Boolean` (True); `PDFSignerCryptoProvider.IsDefinedLevel(Integer) As Boolean`; `LegacyLevelDerivation.FromLegacy(TimeStampingEnabled As Boolean, DocumentTimeStamp As Boolean, RevocationCheck As Integer) As LegacyLevelDerivation` with `Level`, `EnableRevocationChecking`, `RevocationCheckProtocol`, `EmbedRevocationInformation`.
- Removes: `IsTimeStampingEnabled`, `IsDocumentTimeStamp`, `IsSinglePassPadesBLTA`, `RevocationCheck` (property) and the CSS constants `IsTimeStampingEnabled`, `IsDocumentTimeStamp`, `IsSinglePassPadesBLTA`. The Setup form still binds to the removed names until Task 5; the Setup project compiles regardless because bindings are strings.

- [ ] **Step 1: Write the failing derivation tests**

`PDF Signer for Tungsten Capture Tests\LegacyLevelDerivationTests.vb`:

```vb
Imports PDFSignerCommon

<TestClass>
Public Class LegacyLevelDerivationTests
    <TestMethod>
    Public Sub No_timestamping_is_BaselineB()
        Dim d = LegacyLevelDerivation.FromLegacy(False, False, RevocationType.CRL)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineB), CInt(d.Level))
    End Sub

    <TestMethod>
    Public Sub Timestamping_without_document_timestamp_and_without_revocation_is_BaselineT()
        Dim d = LegacyLevelDerivation.FromLegacy(True, False, RevocationType.None)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineT), CInt(d.Level))
        Assert.IsFalse(d.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), d.RevocationCheckProtocol)
    End Sub

    <TestMethod>
    Public Sub Timestamping_without_document_timestamp_with_revocation_is_BaselineLT()
        Dim d = LegacyLevelDerivation.FromLegacy(True, False, RevocationType.OCSPWithCRLFallback)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLT), CInt(d.Level))
        Assert.IsTrue(d.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSPWithCRLFallback), d.RevocationCheckProtocol)
    End Sub

    <TestMethod>
    Public Sub Document_timestamp_is_BaselineLTA_regardless_of_revocation()
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), CInt(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.None).Level))
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), CInt(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.OCSP).Level))
    End Sub

    <TestMethod>
    Public Sub Embedding_is_always_on()
        Assert.IsTrue(LegacyLevelDerivation.FromLegacy(False, False, RevocationType.None).EmbedRevocationInformation)
        Assert.IsTrue(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.CRL).EmbedRevocationInformation)
    End Sub
End Class
```

`PDF Signer for Tungsten Capture Tests\PDFSignerCryptoProviderTests.vb`:

```vb
Imports System.Xml.Linq
Imports PDFSignerCommon
Imports PDFSignerCommon.My.Resources

<TestClass>
Public Class PDFSignerCryptoProviderTests
    Private Shared Function Enabled(level As PAdESLevelType, Optional checking As Boolean = False, Optional embed As Boolean = True,
                                    Optional protocol As RevocationType = RevocationType.OCSP, Optional tsa As String = "https://tsa.example/ts") As PDFSignerCryptoProvider
        Return New PDFSignerCryptoProvider With {
            .Enabled = True,
            .PAdESLevel = level,
            .EnableRevocationChecking = checking,
            .EmbedRevocationInformation = embed,
            .RevocationCheckProtocol = protocol,
            .TSAURL = If(tsa Is Nothing, Nothing, New Uri(tsa))}
    End Function

    <TestMethod>
    Public Sub Defaults_are_BaselineB_with_OCSP_protocol_and_embedding_on()
        Dim p As New PDFSignerCryptoProvider
        Assert.AreEqual(CInt(PAdESLevelType.BaselineB), p.PAdESLevel)
        Assert.IsFalse(p.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), p.RevocationCheckProtocol)
        Assert.IsTrue(p.EmbedRevocationInformation)
        Assert.AreEqual(CInt(HashType.SHA256), p.SignatureHashMethod)
    End Sub

    <TestMethod>
    Public Sub Disabled_provider_is_valid_whatever_it_holds()
        Dim p As New PDFSignerCryptoProvider With {.Enabled = False, .PAdESLevel = 0}
        Assert.AreEqual(String.Empty, p.Validate())
    End Sub

    <TestMethod>
    Public Sub Undefined_level_is_rejected()
        Dim p = Enabled(PAdESLevelType.BaselineB)
        p.PAdESLevel = 1
        Assert.AreEqual(Messages.PAdES_Level_Missing, p.Validate())
    End Sub

    <TestMethod>
    Public Sub BaselineB_needs_no_tsa()
        Assert.AreEqual(String.Empty, Enabled(PAdESLevelType.BaselineB, tsa:=Nothing).Validate())
    End Sub

    <TestMethod>
    Public Sub Timestamped_levels_need_a_tsa_url()
        For Each level In {PAdESLevelType.BaselineT, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.AreEqual(Messages.TSA_URL_Missing, Enabled(level, checking:=True, tsa:=Nothing).Validate(), level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Checking_on_needs_a_protocol()
        Assert.AreEqual(Messages.Revocation_Method_Missing, Enabled(PAdESLevelType.BaselineB, checking:=True, protocol:=RevocationType.None).Validate())
    End Sub

    <TestMethod>
    Public Sub Long_term_levels_need_checking_and_embedding()
        For Each level In {PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.AreEqual(Messages.LTV_Requires_Revocation_Embedding, Enabled(level, checking:=False, embed:=True).Validate(), level.ToString())
            Assert.AreEqual(Messages.LTV_Requires_Revocation_Embedding, Enabled(level, checking:=True, embed:=False).Validate(), level.ToString())
            Assert.AreEqual(String.Empty, Enabled(level, checking:=True, embed:=True).Validate(), level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub BaselineLTA_needs_a_timestamp_hash()
        Dim p = Enabled(PAdESLevelType.BaselineLTA, checking:=True)
        p.TimeStampHashMethod = 0
        Assert.AreEqual(Messages.Time_Stamp_Hash_Algorithm_Missing, p.Validate())
    End Sub

    <TestMethod>
    Public Sub Xml_round_trip_keeps_the_new_fields()
        Dim p = Enabled(PAdESLevelType.BaselineLT, checking:=True, protocol:=RevocationType.CRL)
        p.EmbedRevocationInformation = True
        Dim restored As New PDFSignerCryptoProvider
        restored.SetupDataFromXml(p.SetupDataToXml(EncryptSecrets:=True))
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLT), restored.PAdESLevel)
        Assert.IsTrue(restored.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.CRL), restored.RevocationCheckProtocol)
        Assert.IsTrue(restored.EmbedRevocationInformation)
        Assert.IsNull(p.SetupDataToXml(EncryptSecrets:=False).Element("IsTimeStampingEnabled"))
    End Sub

    <TestMethod>
    Public Sub Xml_without_a_level_is_derived_from_the_legacy_elements()
        Dim legacy As XElement = <PDFSignerCryptoProvider>
                                     <Enabled>1</Enabled>
                                     <SigningOrganization>Org</SigningOrganization>
                                     <SigningReason>Reason</SigningReason>
                                     <RevocationCheck>2</RevocationCheck>
                                     <SignatureHashMethod>28932</SignatureHashMethod>
                                     <AllowQualifiedCertificatesOnly>0</AllowQualifiedCertificatesOnly>
                                     <SignerNameFromLoggedOnUser>0</SignerNameFromLoggedOnUser>
                                     <IsTimeStampingEnabled>1</IsTimeStampingEnabled>
                                     <TSAURL>https://tsa.example/ts</TSAURL>
                                     <TSAUserName></TSAUserName>
                                     <TSAPassword></TSAPassword>
                                     <IsDocumentTimeStamp>1</IsDocumentTimeStamp>
                                     <IsSinglePassPadesBLTA>1</IsSinglePassPadesBLTA>
                                     <TimeStampHashMethod>28932</TimeStampHashMethod>
                                     <IsProxyEnabled>0</IsProxyEnabled>
                                     <ProxyServer></ProxyServer>
                                     <ProxyPort>8080</ProxyPort>
                                     <ProxyAuthMethod>0</ProxyAuthMethod>
                                     <ProxyUserName></ProxyUserName>
                                     <ProxyPassword></ProxyPassword>
                                 </PDFSignerCryptoProvider>
        Dim p As New PDFSignerCryptoProvider
        p.SetupDataFromXml(legacy)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), p.PAdESLevel)
        Assert.IsTrue(p.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), p.RevocationCheckProtocol)
        Assert.IsTrue(p.EmbedRevocationInformation)
        Assert.AreEqual("Org", p.SigningOrganization)
    End Sub
End Class
```

Note for the XML tests: `SetupDataFromXml` decrypts `TSAPassword`/`ProxyPassword` with `Encrypt.AES256Decrypt`; empty elements must decrypt to an empty string. If `AES256Decrypt("")` throws, wrap the two password reads in `SetupDataFromXml` with `If String.IsNullOrEmpty(value) Then "" Else enc.AES256Decrypt(value)` — that is a bug fix inside the scope of this task.

- [ ] **Step 2: Build to see the tests fail**

Build. Expected: compile errors in the test project (`PAdESLevelType`, `LegacyLevelDerivation`, `PAdESLevel` not declared).

- [ ] **Step 3: Add the enum and CSS constants**

In `Models\Enum.vb`, after `RevocationType`:

```vb
''' <summary>PAdES baseline level the built-in provider produces; values equal PDF Streamer's PAdESLevel members.</summary>
Public Enum PAdESLevelType As Integer
    BaselineB = 3
    BaselineT = 4
    BaselineLT = 5
    BaselineLTA = 6
End Enum
```

In `Models\CSS.vb`, replace the `' PDF Signer properties` block through `TimeStampHashMethod` with:

```vb
    ' PDF Signer properties
    Public Const IsPDFSignerEnabled As String = BaseNamespace + "IsPDFSignerEnabled"
    Public Const SigningOrganization As String = BaseNamespace + "SigningOrganization"
    Public Const SigningReason As String = BaseNamespace + "SigningReason"
    Public Const PAdESLevel As String = BaseNamespace + "PAdESLevel"
    Public Const SignatureHashMethod As String = BaseNamespace + "SignatureHashMethod"
    Public Const AllowQualifiedCertificatesOnly As String = BaseNamespace + "AllowQualifiedCertificatesOnly"
    Public Const SignerNameFromLoggedOnUser As String = BaseNamespace + "SignerNameFromLoggedOnUser"

    Public Const TSAURL As String = BaseNamespace + "TSAURL"
    Public Const TSAUserName As String = BaseNamespace + "TSAUserName"
    Public Const TSAPassword As String = BaseNamespace + "TSAPassword"
    Public Const TimeStampHashMethod As String = BaseNamespace + "TimeStampHashMethod"

    Public Const EnableRevocationChecking As String = BaseNamespace + "EnableRevocationChecking"
    Public Const RevocationCheck As String = BaseNamespace + "RevocationCheck" ' revocation protocol; name kept for stored data
    Public Const EmbedRevocationInformation As String = BaseNamespace + "EmbedRevocationInformation"
```

- [ ] **Step 4: Write the derivation class**

`Models\Setup\Providers\LegacyLevelDerivation.vb`:

```vb
''' <summary>Provider values derived from a backup file written before the PAdESLevel element existed (timestamp switches + one revocation setting).</summary>
Public Class LegacyLevelDerivation
    Public Property Level As PAdESLevelType
    Public Property EnableRevocationChecking As Boolean
    Public Property RevocationCheckProtocol As Integer
    Public Property EmbedRevocationInformation As Boolean

    ''' <summary>Maps the timestamp switches and the single revocation setting onto a level and the three revocation switches.</summary>
    Public Shared Function FromLegacy(TimeStampingEnabled As Boolean, DocumentTimeStamp As Boolean, RevocationCheck As Integer) As LegacyLevelDerivation
        Dim checking As Boolean = RevocationCheck <> RevocationType.None
        Dim level As PAdESLevelType
        If Not TimeStampingEnabled Then
            level = PAdESLevelType.BaselineB
        ElseIf Not DocumentTimeStamp Then
            ' A timestamped signature whose revocation data was collected is a B-LT.
            level = If(checking, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineT)
        Else
            level = PAdESLevelType.BaselineLTA
        End If

        Return New LegacyLevelDerivation With {
            .Level = level,
            .EnableRevocationChecking = checking,
            .RevocationCheckProtocol = If(checking, RevocationCheck, CInt(RevocationType.OCSP)),
            .EmbedRevocationInformation = True}
    End Function
End Class
```

- [ ] **Step 5: Rewrite the provider class**

Replace the whole content of `Models\Setup\Providers\PDFSignerCryptoProvider.vb` with:

```vb
Imports PDFSignerCommon.My.Resources

''' <summary>Settings of the built-in SecureBlackbox signing provider ("PDF Signer") for one document class.</summary>
Public Class PDFSignerCryptoProvider
    Inherits CryptoProviderBase

    Public Overrides Function ToString() As String
        Return "PDF Signer"
    End Function

    Public Overrides ReadOnly Property ProviderType As CryptoProviderType = CryptoProviderType.PDFSigner
    Public Overrides ReadOnly Property ConfigXmlElementName As String = "PDFSignerCryptoProvider"
    Public Overrides ReadOnly Property SupportsLocalCertificates As Boolean = True

    Public Property SigningOrganization As String
    Public Property SigningReason As String
    ''' <summary>PAdES baseline level to produce (a <see cref="PAdESLevelType"/> value).</summary>
    Public Property PAdESLevel As Integer
    Public Property SignatureHashMethod As Integer

    Public Property AllowQualifiedCertificatesOnly As Boolean
    Public Property SignerNameFromLoggedOnUser As Boolean
    Public Property TSAURL As Uri
    Public Property TSAUserName As String
    Public Property TSAPassword As String
    ''' <summary>Hash of the B-LTA archive document timestamp.</summary>
    Public Property TimeStampHashMethod As Integer

    ''' <summary>Whether the chains of the new signature and its timestamps are revocation-checked.</summary>
    Public Property EnableRevocationChecking As Boolean
    ''' <summary>Protocol of the revocation check (a <see cref="RevocationType"/> value); stored under the CSS/XML name RevocationCheck.</summary>
    Public Property RevocationCheckProtocol As Integer
    ''' <summary>Whether the revocation data is written into the document (B-LT / B-LTA).</summary>
    Public Property EmbedRevocationInformation As Boolean

    Public Property IsProxyEnabled As Boolean
    Public Property ProxyServer As String
    Public Property ProxyPort As Integer
    Public Property ProxyAuthMethod As Integer
    Public Property ProxyUserName As String
    Public Property ProxyPassword As String

    Public Sub New()
        PAdESLevel = PAdESLevelType.BaselineB
        SignatureHashMethod = HashType.SHA256
        TimeStampHashMethod = HashType.SHA256
        EnableRevocationChecking = False
        RevocationCheckProtocol = RevocationType.OCSP
        EmbedRevocationInformation = True
        ProxyPort = 8080
        ProxyAuthMethod = ProxyAuthenticationMethod.NoAuthentication
    End Sub

    ''' <summary>True when the value is one of the four offered baseline levels.</summary>
    Public Shared Function IsDefinedLevel(Level As Integer) As Boolean
        Return Level >= PAdESLevelType.BaselineB AndAlso Level <= PAdESLevelType.BaselineLTA
    End Function

    Private ReadOnly Property IsTimestamped As Boolean
        Get
            Return PAdESLevel = PAdESLevelType.BaselineT OrElse IsLongTerm
        End Get
    End Property

    Private ReadOnly Property IsLongTerm As Boolean
        Get
            Return PAdESLevel = PAdESLevelType.BaselineLT OrElse PAdESLevel = PAdESLevelType.BaselineLTA
        End Get
    End Property

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(SigningOrganization) Then SigningOrganization = SigningOrganization.Trim
        If Not String.IsNullOrEmpty(SigningReason) Then SigningReason = SigningReason.Trim
        If Not String.IsNullOrEmpty(TSAUserName) Then TSAUserName = TSAUserName.Trim
        If Not String.IsNullOrEmpty(ProxyServer) Then ProxyServer = ProxyServer.Trim
        If Not String.IsNullOrEmpty(ProxyUserName) Then ProxyUserName = ProxyUserName.Trim

        If Not Enabled Then Return String.Empty

        If SignatureHashMethod = HashType.None Then Return Messages.Signature_Hash_Algorithm_Missing
        If Not IsDefinedLevel(PAdESLevel) Then Return Messages.PAdES_Level_Missing

        If IsTimestamped AndAlso String.IsNullOrEmpty(TSAURL?.ToString) Then Return Messages.TSA_URL_Missing
        If PAdESLevel = PAdESLevelType.BaselineLTA AndAlso TimeStampHashMethod = HashType.None Then Return Messages.Time_Stamp_Hash_Algorithm_Missing

        If EnableRevocationChecking AndAlso RevocationCheckProtocol = RevocationType.None Then Return Messages.Revocation_Method_Missing
        ' B-LT / B-LTA only exist with collected and embedded revocation data.
        If IsLongTerm AndAlso Not (EnableRevocationChecking AndAlso EmbedRevocationInformation) Then Return Messages.LTV_Requires_Revocation_Embedding

        If IsProxyEnabled Then
            If String.IsNullOrEmpty(ProxyServer) Then Return Messages.Proxy_Server_Missing
            If ProxyServer.Contains("://") OrElse ProxyServer.Contains(":") Then Return Messages.Proxy_Server_Invalid
            If ProxyAuthMethod = ProxyAuthenticationMethod.UserPassword AndAlso String.IsNullOrWhiteSpace(ProxyUserName) Then Return Messages.Proxy_User_Missing
        End If

        Return String.Empty
    End Function

    ''' <summary>Loads the block from the document class; a class without a stored PAdES level keeps the constructor defaults.</summary>
    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        With Parser
            Dim levelValue As String = .ReadSetupCSS(CSS.PAdESLevel)
            If String.IsNullOrEmpty(levelValue) Then Return

            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsPDFSignerEnabled))
            SigningOrganization = .ReadSetupCSS(CSS.SigningOrganization)
            SigningReason = .ReadSetupCSS(CSS.SigningReason)
            PAdESLevel = .ReadSetupCSSInteger(CSS.PAdESLevel, PAdESLevelType.BaselineB)
            SignatureHashMethod = .ReadSetupCSSInteger(CSS.SignatureHashMethod, HashType.SHA256)
            AllowQualifiedCertificatesOnly = Converter.StringToBoolean(.ReadSetupCSS(CSS.AllowQualifiedCertificatesOnly))
            SignerNameFromLoggedOnUser = Converter.StringToBoolean(.ReadSetupCSS(CSS.SignerNameFromLoggedOnUser))

            TSAURL = Converter.StringToUri(.ReadSetupCSS(CSS.TSAURL))
            TSAUserName = .ReadSetupCSS(CSS.TSAUserName)
            TSAPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.TSAPassword))
            TimeStampHashMethod = .ReadSetupCSSInteger(CSS.TimeStampHashMethod, HashType.SHA256)

            EnableRevocationChecking = Converter.StringToBoolean(.ReadSetupCSS(CSS.EnableRevocationChecking))
            RevocationCheckProtocol = .ReadSetupCSSInteger(CSS.RevocationCheck, RevocationType.OCSP)
            EmbedRevocationInformation = Converter.StringToBoolean(.ReadSetupCSS(CSS.EmbedRevocationInformation))

            IsProxyEnabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsProxyEnabled))
            ProxyAuthMethod = .ReadSetupCSSInteger(CSS.ProxyAuthMethod, 0)
            ProxyServer = .ReadSetupCSS(CSS.ProxyServer)
            ProxyPort = .ReadSetupCSSInteger(CSS.ProxyPort, 8080)
            ProxyUserName = .ReadSetupCSS(CSS.ProxyUserName)
            ProxyPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.ProxyPassword))
        End With
    End Sub

    Public Overrides Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
        With Parser
            .WriteSetupCSS(CSS.IsPDFSignerEnabled, Converter.BooleanToNumericString(Enabled))
            .WriteSetupCSS(CSS.SigningOrganization, SigningOrganization)
            .WriteSetupCSS(CSS.SigningReason, SigningReason)
            .WriteSetupCSS(CSS.PAdESLevel, PAdESLevel)
            .WriteSetupCSS(CSS.SignatureHashMethod, SignatureHashMethod)
            .WriteSetupCSS(CSS.AllowQualifiedCertificatesOnly, Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly))
            .WriteSetupCSS(CSS.SignerNameFromLoggedOnUser, Converter.BooleanToNumericString(SignerNameFromLoggedOnUser))

            .WriteSetupCSS(CSS.TSAURL, If(TSAURL Is Nothing, String.Empty, TSAURL.ToString))
            .WriteSetupCSS(CSS.TSAUserName, TSAUserName)
            .WriteSetupCSS(CSS.TSAPassword, enc.AES256Encrypt(TSAPassword))
            .WriteSetupCSS(CSS.TimeStampHashMethod, TimeStampHashMethod)

            .WriteSetupCSS(CSS.EnableRevocationChecking, Converter.BooleanToNumericString(EnableRevocationChecking))
            .WriteSetupCSS(CSS.RevocationCheck, RevocationCheckProtocol)
            .WriteSetupCSS(CSS.EmbedRevocationInformation, Converter.BooleanToNumericString(EmbedRevocationInformation))

            .WriteSetupCSS(CSS.IsProxyEnabled, Converter.BooleanToNumericString(IsProxyEnabled))
            .WriteSetupCSS(CSS.ProxyAuthMethod, ProxyAuthMethod)
            .WriteSetupCSS(CSS.ProxyServer, ProxyServer)
            .WriteSetupCSS(CSS.ProxyPort, ProxyPort.ToString)
            .WriteSetupCSS(CSS.ProxyUserName, ProxyUserName)
            .WriteSetupCSS(CSS.ProxyPassword, enc.AES256Encrypt(ProxyPassword))
        End With
    End Sub

    Public Overrides Function SetupDataToXml(EncryptSecrets As Boolean) As XElement
        Return New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("SigningOrganization", SigningOrganization),
                New XElement("SigningReason", SigningReason),
                New XElement("PAdESLevel", PAdESLevel),
                New XElement("SignatureHashMethod", SignatureHashMethod),
                New XElement("AllowQualifiedCertificatesOnly", Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly)),
                New XElement("SignerNameFromLoggedOnUser", Converter.BooleanToNumericString(SignerNameFromLoggedOnUser)),
                New XElement("TSAURL", TSAURL),
                New XElement("TSAUserName", TSAUserName),
                New XElement("TSAPassword", SecretToXml(TSAPassword, EncryptSecrets)),
                New XElement("TimeStampHashMethod", TimeStampHashMethod),
                New XElement("EnableRevocationChecking", Converter.BooleanToNumericString(EnableRevocationChecking)),
                New XElement("RevocationCheck", RevocationCheckProtocol),
                New XElement("EmbedRevocationInformation", Converter.BooleanToNumericString(EmbedRevocationInformation)),
                New XElement("IsProxyEnabled", Converter.BooleanToNumericString(IsProxyEnabled)),
                New XElement("ProxyServer", ProxyServer),
                New XElement("ProxyPort", ProxyPort),
                New XElement("ProxyAuthMethod", ProxyAuthMethod),
                New XElement("ProxyUserName", ProxyUserName),
                New XElement("ProxyPassword", SecretToXml(ProxyPassword, EncryptSecrets)))
    End Function

    ''' <summary>Restores the block from a backup; a backup without a PAdESLevel element is mapped through <see cref="LegacyLevelDerivation"/>.</summary>
    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        SigningOrganization = ConfigElement.Element("SigningOrganization")
        SigningReason = ConfigElement.Element("SigningReason")
        SignatureHashMethod = CInt(ConfigElement.Element("SignatureHashMethod"))
        AllowQualifiedCertificatesOnly = Converter.StringToBoolean(ConfigElement.Element("AllowQualifiedCertificatesOnly"))
        SignerNameFromLoggedOnUser = Converter.StringToBoolean(ConfigElement.Element("SignerNameFromLoggedOnUser"))

        TSAURL = Converter.StringToUri(ConfigElement.Element("TSAURL"))
        TSAUserName = ConfigElement.Element("TSAUserName")
        TSAPassword = enc.AES256Decrypt(ConfigElement.Element("TSAPassword"))
        TimeStampHashMethod = CInt(ConfigElement.Element("TimeStampHashMethod"))

        If ConfigElement.Element("PAdESLevel") IsNot Nothing Then
            PAdESLevel = CInt(ConfigElement.Element("PAdESLevel"))
            EnableRevocationChecking = Converter.StringToBoolean(ConfigElement.Element("EnableRevocationChecking"))
            RevocationCheckProtocol = CInt(ConfigElement.Element("RevocationCheck"))
            EmbedRevocationInformation = Converter.StringToBoolean(ConfigElement.Element("EmbedRevocationInformation"))
        Else
            Dim derived As LegacyLevelDerivation = LegacyLevelDerivation.FromLegacy(
                Converter.StringToBoolean(ConfigElement.Element("IsTimeStampingEnabled")),
                Converter.StringToBoolean(ConfigElement.Element("IsDocumentTimeStamp")),
                CInt(ConfigElement.Element("RevocationCheck")))
            PAdESLevel = derived.Level
            EnableRevocationChecking = derived.EnableRevocationChecking
            RevocationCheckProtocol = derived.RevocationCheckProtocol
            EmbedRevocationInformation = derived.EmbedRevocationInformation
        End If

        IsProxyEnabled = Converter.StringToBoolean(ConfigElement.Element("IsProxyEnabled"))
        ProxyServer = ConfigElement.Element("ProxyServer")
        ProxyPort = ConfigElement.Element("ProxyPort")
        ProxyAuthMethod = CInt(ConfigElement.Element("ProxyAuthMethod"))
        ProxyUserName = ConfigElement.Element("ProxyUserName")
        ProxyPassword = enc.AES256Decrypt(ConfigElement.Element("ProxyPassword"))
    End Sub
End Class
```

- [ ] **Step 6: Update the runtime CSS parser**

In `Parser\SetupParser.vb`, replace the `' PDF Signer values` block (the 20 `.PDFSignerProvider.` lines) with:

```vb
                ' PDF Signer values; a document class without a stored PAdES level keeps the provider at its defaults.
                Dim levelValue As String = GetSetupCSSValue(SetupDocumentClassElement, CSS.PAdESLevel)
                If Not String.IsNullOrEmpty(levelValue) Then
                    .PDFSignerProvider.Enabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsPDFSignerEnabled))
                    .PDFSignerProvider.SigningOrganization = GetSetupCSSValue(SetupDocumentClassElement, CSS.SigningOrganization)
                    .PDFSignerProvider.SigningReason = GetSetupCSSValue(SetupDocumentClassElement, CSS.SigningReason)
                    .PDFSignerProvider.PAdESLevel = Converter.StringToInteger(levelValue)
                    .PDFSignerProvider.SignatureHashMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.SignatureHashMethod)
                    .PDFSignerProvider.AllowQualifiedCertificatesOnly = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.AllowQualifiedCertificatesOnly))
                    .PDFSignerProvider.SignerNameFromLoggedOnUser = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignerNameFromLoggedOnUser))
                    .PDFSignerProvider.TSAURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAURL))
                    .PDFSignerProvider.TSAUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAUserName)
                    .PDFSignerProvider.TSAPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAPassword))
                    .PDFSignerProvider.TimeStampHashMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.TimeStampHashMethod)
                    .PDFSignerProvider.EnableRevocationChecking = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.EnableRevocationChecking))
                    .PDFSignerProvider.RevocationCheckProtocol = GetSetupCSSValue(SetupDocumentClassElement, CSS.RevocationCheck)
                    .PDFSignerProvider.EmbedRevocationInformation = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.EmbedRevocationInformation))
                    .PDFSignerProvider.IsProxyEnabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsProxyEnabled))
                    .PDFSignerProvider.ProxyServer = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyServer)
                    .PDFSignerProvider.ProxyPort = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyPort))
                    .PDFSignerProvider.ProxyAuthMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyAuthMethod)
                    .PDFSignerProvider.ProxyUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyUserName)
                    .PDFSignerProvider.ProxyPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyPassword))
                End If
```

- [ ] **Step 7: Build and run the tests**

Build. The runtime project's `SBBPDF.vb` still reads `_settings.IsTimeStampingEnabled`, `IsDocumentTimeStamp`, `IsSinglePassPadesBLTA`, `RevocationCheck`: to keep the solution compiling until Task 14 deletes the file, apply these minimal edits in `SBBPDF.vb`: replace every `_settings.RevocationCheck` with `_settings.RevocationCheckProtocol`, every `_settings.IsTimeStampingEnabled` with `(_settings.PAdESLevel <> PAdESLevelType.BaselineB)`, every `_settings.IsDocumentTimeStamp` with `(_settings.PAdESLevel = PAdESLevelType.BaselineLTA)` and every `_settings.IsSinglePassPadesBLTA` with `(_settings.PAdESLevel = PAdESLevelType.BaselineLTA)`. Then build again (expected `0 Error(s)`) and run the tests. Expected: all tests pass (4 from earlier tasks + 5 derivation + 10 provider).

- [ ] **Step 8: Commit**

```
git add -A
git commit -m "Model the PAdES level and revocation switches in the PDF Signer provider settings"
```

---

### Task 5: Setup UI — PAdES level combo and revocation group

**Files:**
- Modify: `PDF Signer for Tungsten Capture Setup\Helper\ComboFiller.vb`
- Modify: `PDF Signer for Tungsten Capture Setup\FrmSetup.Designer.vb`
- Modify: `PDF Signer for Tungsten Capture Setup\FrmSetup.resx`
- Modify: `PDF Signer for Tungsten Capture Setup\FrmSetup.vb`

**Interfaces:**
- Consumes: `PDFSignerCryptoProvider.PAdESLevel`, `EnableRevocationChecking`, `RevocationCheckProtocol`, `EmbedRevocationInformation`; `PAdESLevelType`.
- Produces: controls `ComboPAdESLevel`, `LblPAdESLevel` (renamed `Label10`), `GrpRevocation`, `ChkEnableRevocationChecking`, `LblRevocationProtocol`, `ComboRevocationCheckProtocol` (renamed `ComboRevocationCheck`), `ChkEmbedRevocationInformation`; `ComboFiller.FillComboWithPAdESLevels(ComboBox)`.

There is no automated UI test; the deliverable is verified by a clean build plus opening the form in the Capture Administration module (Step 7).

- [ ] **Step 1: Add the level combo filler**

Append to `Helper\ComboFiller.vb` before `End Class`:

```vb
    ''' <summary>Fills a combo with the four PAdES baseline levels the built-in provider offers.</summary>
    Friend Sub FillComboWithPAdESLevels(Combo As ComboBox)
        Dim levels = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineB, .DisplayText = "PAdES B-B (signature only)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineT, .DisplayText = "PAdES B-T (signature time stamp)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineLT, .DisplayText = "PAdES B-LT (embedded revocation information)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineLTA, .DisplayText = "PAdES B-LTA (archive document time stamp)"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = levels
    End Sub
```

- [ ] **Step 2: Designer code — declarations and construction**

In `FrmSetup.Designer.vb`:

1. Delete the three declarations `Private WithEvents ChkTimeStampingEnabled ...`, `Private WithEvents ChkIsDocumentTimeStamp ...`, `Private WithEvents ChkSinglePassPadesLTA ...` and their `New CheckBox()` lines (lines 74, 77, 84).
2. Rename every occurrence of `ComboRevocationCheck` to `ComboRevocationCheckProtocol` and every occurrence of `Label10` to `LblPAdESLevel` (declaration, `New`, `resources.ApplyResources`, `.Name =`, `Controls.Add`).
3. Add declarations at the end of the class, next to `ChkSignerNameFromLoggedOnUser`:

```vb
    Private WithEvents ComboPAdESLevel As ComboBox
    Private WithEvents GrpRevocation As GroupBox
    Private WithEvents ChkEnableRevocationChecking As CheckBox
    Private WithEvents LblRevocationProtocol As Label
    Private WithEvents ChkEmbedRevocationInformation As CheckBox
```

4. Add construction lines next to `ChkSignerNameFromLoggedOnUser = New CheckBox()`:

```vb
        ComboPAdESLevel = New ComboBox()
        GrpRevocation = New GroupBox()
        ChkEnableRevocationChecking = New CheckBox()
        LblRevocationProtocol = New Label()
        ChkEmbedRevocationInformation = New CheckBox()
```

5. Add `GrpRevocation.SuspendLayout()` next to `GrpProxy.SuspendLayout()` and `GrpRevocation.ResumeLayout(False)` + `GrpRevocation.PerformLayout()` next to the `GrpProxy.ResumeLayout(False)` pair.

- [ ] **Step 3: Designer code — control setup blocks**

Replace the `' ComboRevocationCheck` block (now `ComboRevocationCheckProtocol`) with:

```vb
        ' 
        ' ComboRevocationCheckProtocol
        ' 
        resources.ApplyResources(ComboRevocationCheckProtocol, "ComboRevocationCheckProtocol")
        ComboRevocationCheckProtocol.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "RevocationCheckProtocol", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboRevocationCheckProtocol.DropDownStyle = ComboBoxStyle.DropDownList
        ComboRevocationCheckProtocol.FormattingEnabled = True
        ComboRevocationCheckProtocol.Name = "ComboRevocationCheckProtocol"
```

Delete the `' ChkSinglePassPadesLTA`, `' ChkIsDocumentTimeStamp` and `' ChkTimeStampingEnabled` blocks and their `GrpTimeStamp.Controls.Add(...)` / `PanelPDFSigner.Controls.Add(ChkTimeStampingEnabled)` lines.

Add these blocks right after the `' LblPAdESLevel` block:

```vb
        ' 
        ' ComboPAdESLevel
        ' 
        resources.ApplyResources(ComboPAdESLevel, "ComboPAdESLevel")
        ComboPAdESLevel.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "PAdESLevel", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboPAdESLevel.DropDownStyle = ComboBoxStyle.DropDownList
        ComboPAdESLevel.FormattingEnabled = True
        ComboPAdESLevel.Name = "ComboPAdESLevel"
        ' 
        ' ChkEnableRevocationChecking
        ' 
        resources.ApplyResources(ChkEnableRevocationChecking, "ChkEnableRevocationChecking")
        ChkEnableRevocationChecking.BackColor = Drawing.SystemColors.ControlLightLight
        ChkEnableRevocationChecking.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "EnableRevocationChecking", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkEnableRevocationChecking.Name = "ChkEnableRevocationChecking"
        ChkEnableRevocationChecking.UseVisualStyleBackColor = False
        ' 
        ' GrpRevocation
        ' 
        resources.ApplyResources(GrpRevocation, "GrpRevocation")
        GrpRevocation.Controls.Add(ChkEmbedRevocationInformation)
        GrpRevocation.Controls.Add(ComboRevocationCheckProtocol)
        GrpRevocation.Controls.Add(LblRevocationProtocol)
        GrpRevocation.Name = "GrpRevocation"
        GrpRevocation.TabStop = False
        ' 
        ' LblRevocationProtocol
        ' 
        resources.ApplyResources(LblRevocationProtocol, "LblRevocationProtocol")
        LblRevocationProtocol.Name = "LblRevocationProtocol"
        ' 
        ' ChkEmbedRevocationInformation
        ' 
        resources.ApplyResources(ChkEmbedRevocationInformation, "ChkEmbedRevocationInformation")
        ChkEmbedRevocationInformation.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "EmbedRevocationInformation", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkEmbedRevocationInformation.Name = "ChkEmbedRevocationInformation"
        ChkEmbedRevocationInformation.UseVisualStyleBackColor = True
```

In the `PanelPDFSigner` block, the `Controls.Add` list must become (order = z-order, first on top; the checkbox that overlays a group border must precede the group):

```vb
        PanelPDFSigner.Controls.Add(ChkAllowQualifiedCertificatesOnly)
        PanelPDFSigner.Controls.Add(ComboSignatureHashMethod)
        PanelPDFSigner.Controls.Add(ChkEnableRevocationChecking)
        PanelPDFSigner.Controls.Add(ChkProxyEnabled)
        PanelPDFSigner.Controls.Add(Label11)
        PanelPDFSigner.Controls.Add(ComboPAdESLevel)
        PanelPDFSigner.Controls.Add(LblPAdESLevel)
        PanelPDFSigner.Controls.Add(TxtSigningReason)
        PanelPDFSigner.Controls.Add(GrpTimeStamp)
        PanelPDFSigner.Controls.Add(GrpRevocation)
        PanelPDFSigner.Controls.Add(GrpProxy)
        PanelPDFSigner.Controls.Add(Label9)
        PanelPDFSigner.Controls.Add(TxtSigningOrganization)
        PanelPDFSigner.Controls.Add(Label8)
        PanelPDFSigner.Controls.Add(ChkSignerNameFromLoggedOnUser)
```

`GrpTimeStamp.Controls.Add` keeps `ComboTimeStampHashMethod`, `Label18`, `TxtTSAPassword`, `Label16`, `TxtTSAUser`, `Label17`, `TxtTSAURL`, `Label15` (the two checkbox lines removed).

- [ ] **Step 4: Resx — layout entries**

In `FrmSetup.resx`:

1. Delete every `<data>` whose name starts with `ChkTimeStampingEnabled.`, `>>ChkTimeStampingEnabled.`, `ChkIsDocumentTimeStamp.`, `>>ChkIsDocumentTimeStamp.`, `ChkSinglePassPadesLTA.`, `>>ChkSinglePassPadesLTA.`.
2. Rename `ComboRevocationCheck.*` / `>>ComboRevocationCheck.*` to `ComboRevocationCheckProtocol.*` / `>>ComboRevocationCheckProtocol.*` and set `ComboRevocationCheckProtocol.Location` = `64, 22`, `.Size` = `200, 21`, `.TabIndex` = `1`, `.Anchor` = `Top, Left`, `>>ComboRevocationCheckProtocol.Parent` = `GrpRevocation`.
3. Rename `Label10.*` / `>>Label10.*` to `LblPAdESLevel.*` / `>>LblPAdESLevel.*`, set `LblPAdESLevel.Text` = `PAdES level`, `LblPAdESLevel.Size` = `64, 13`.
4. Change existing values: `Label18.Location` = `5, 76`; `Label18.Anchor` = `Top, Left`; `ComboTimeStampHashMethod.Location` = `111, 73`; `ComboTimeStampHashMethod.Size` = `233, 21`; `ComboTimeStampHashMethod.Anchor` = `Top, Left, Right`; `Label18.TabIndex` = `6`; `ComboTimeStampHashMethod.TabIndex` = `7`; `ChkSignerNameFromLoggedOnUser.TabIndex` = `9`; `ChkProxyEnabled.Location` = `4, 246`; `ChkProxyEnabled.TabIndex` = `13`; `GrpProxy.Location` = `0, 246`; `GrpProxy.TabIndex` = `14`; `PanelPDFSigner.Size` = `625, 321`; `TabPDFSigner.Size` = `631, 327`; `TabCryptoProviders.Size` = `639, 353`; `$this.ClientSize` = `662, 980`; `$this.MinimumSize` = `678, 1019`; and add 49 to the Y coordinate of `BtnOK.Location`, `BtnCancel.Location`, `BtnImportSettings.Location`, `BtnExportSettings.Location`, `BtnResetSettings.Location` (every control anchored `Bottom`). Add `GrpTimeStamp.Text` = `Time stamping` (a new `<data name="GrpTimeStamp.Text" xml:space="preserve"><value>Time stamping</value></data>`).
5. Add the new controls' entries (each as `<data name="..." type="..."><value>...</value></data>`; `Location`/`Size` use `type="System.Drawing.Point, System.Drawing"` / `type="System.Drawing.Size, System.Drawing"`, `TabIndex` uses `type="System.Int32, mscorlib"`, `AutoSize` uses `type="System.Boolean, mscorlib"`, `Anchor` uses `type="System.Windows.Forms.AnchorStyles, System.Windows.Forms"`, text and the `>>` metadata entries use `xml:space="preserve"` with no type; copy the exact `type` strings from the neighbouring `ChkProxyEnabled` / `GrpProxy` entries):

| Name | Value |
|---|---|
| `ComboPAdESLevel.Anchor` | `Top, Left, Right` |
| `ComboPAdESLevel.Location` | `117, 50` |
| `ComboPAdESLevel.Size` | `224, 21` |
| `ComboPAdESLevel.TabIndex` | `5` |
| `>>ComboPAdESLevel.Name` | `ComboPAdESLevel` |
| `>>ComboPAdESLevel.Type` | `System.Windows.Forms.ComboBox, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089` |
| `>>ComboPAdESLevel.Parent` | `PanelPDFSigner` |
| `>>ComboPAdESLevel.ZOrder` | `5` |
| `ChkEnableRevocationChecking.AutoSize` | `True` |
| `ChkEnableRevocationChecking.Location` | `4, 196` |
| `ChkEnableRevocationChecking.Size` | `168, 17` |
| `ChkEnableRevocationChecking.TabIndex` | `11` |
| `ChkEnableRevocationChecking.Text` | `Revocation checking enabled` |
| `>>ChkEnableRevocationChecking.Name` / `.Type` (CheckBox) / `.Parent` = `PanelPDFSigner` / `.ZOrder` = `2` | |
| `GrpRevocation.Anchor` | `Top, Left, Right` |
| `GrpRevocation.Location` | `0, 196` |
| `GrpRevocation.Size` | `625, 50` |
| `GrpRevocation.TabIndex` | `12` |
| `>>GrpRevocation.Name` / `.Type` (GroupBox) / `.Parent` = `PanelPDFSigner` / `.ZOrder` = `9` | |
| `LblRevocationProtocol.AutoSize` | `True` |
| `LblRevocationProtocol.Location` | `5, 25` |
| `LblRevocationProtocol.Size` | `46, 13` |
| `LblRevocationProtocol.TabIndex` | `0` |
| `LblRevocationProtocol.Text` | `Protocol` |
| `>>LblRevocationProtocol.Name` / `.Type` (Label) / `.Parent` = `GrpRevocation` / `.ZOrder` = `2` | |
| `ChkEmbedRevocationInformation.AutoSize` | `True` |
| `ChkEmbedRevocationInformation.Location` | `290, 24` |
| `ChkEmbedRevocationInformation.Size` | `256, 17` |
| `ChkEmbedRevocationInformation.TabIndex` | `2` |
| `ChkEmbedRevocationInformation.Text` | `Embed revocation information into the document` |
| `>>ChkEmbedRevocationInformation.Name` / `.Type` (CheckBox) / `.Parent` = `GrpRevocation` / `.ZOrder` = `0` | |
| `>>ComboRevocationCheckProtocol.ZOrder` | `1` |

The `.Type` values for CheckBox, GroupBox and Label follow the ComboBox pattern with the class name swapped.

- [ ] **Step 5: Form logic**

In `FrmSetup.vb`:

1. In `FrmSetup_Load`, inside the `With filler` block, replace `.FillComboWithRevocationMethods(ComboRevocationCheck)` with `.FillComboWithRevocationMethods(ComboRevocationCheckProtocol)` and add `.FillComboWithPAdESLevels(ComboPAdESLevel)`.
2. After `LoadedValueSnapshot = Settings.GetValueSnapshot()` add `RefreshPdfSignerControlState()`.
3. In `OpenFileDialog_FileOk` after the last `ResetBindings(False)` and in `BtnResetSettings_Click` after `BsMNBSignerCryptoProvider.ResumeBinding()` add `RefreshPdfSignerControlState()`.
4. Add these members before `#Region "Import/Export/Reset settings"`:

```vb
    ''' <summary>Enables the time-stamp and revocation controls that the selected PAdES level allows; B-LT/B-LTA force both revocation switches on.</summary>
    Private Sub RefreshPdfSignerControlState()
        If ComboPAdESLevel.SelectedValue Is Nothing Then Return

        Dim level As Integer = CInt(ComboPAdESLevel.SelectedValue)
        Dim longTerm As Boolean = level = PAdESLevelType.BaselineLT OrElse level = PAdESLevelType.BaselineLTA

        GrpTimeStamp.Enabled = level <> PAdESLevelType.BaselineB
        Label18.Enabled = level = PAdESLevelType.BaselineLTA
        ComboTimeStampHashMethod.Enabled = level = PAdESLevelType.BaselineLTA

        If longTerm Then
            ChkEnableRevocationChecking.Checked = True
            ChkEmbedRevocationInformation.Checked = True
        End If
        ChkEnableRevocationChecking.Enabled = Not longTerm
        ChkEmbedRevocationInformation.Enabled = Not longTerm AndAlso ChkEnableRevocationChecking.Checked
        LblRevocationProtocol.Enabled = ChkEnableRevocationChecking.Checked
        ComboRevocationCheckProtocol.Enabled = ChkEnableRevocationChecking.Checked
    End Sub

    Private Sub ComboPAdESLevel_SelectedValueChanged(sender As Object, e As EventArgs) Handles ComboPAdESLevel.SelectedValueChanged
        RefreshPdfSignerControlState()
    End Sub

    Private Sub ChkEnableRevocationChecking_CheckedChanged(sender As Object, e As EventArgs) Handles ChkEnableRevocationChecking.CheckedChanged
        RefreshPdfSignerControlState()
    End Sub
```

The handlers read the controls, not the model, because `Handles` handlers run before the binding pushes the new value into the provider.

- [ ] **Step 6: Build**

Run the build. Expected `0 Error(s)`. A resx error such as `Could not find any resources appropriate for the specified culture` or a designer `NullReferenceException` at form load means a `>>X.Type` or `>>X.Parent` entry is missing or misspelled.

- [ ] **Step 7: Manual check in Capture Administration**

Copy `PDF Signer for Tungsten Capture Setup\bin\Debug\PDFSignerSetup.dll` and `PDF Signer for Tungsten Capture Common\bin\Debug\PDFSignerCommon.dll` to `C:\Program Files (x86)\Tungsten\Capture\Bin\` (needs an elevated shell), open a document class' PDF Signer setup and confirm: the level combo offers four entries; B-B disables the time-stamp group; B-T enables it with the hash combo disabled; B-LTA enables the hash combo; B-LT/B-LTA tick and lock both revocation checkboxes; unticking "Revocation checking enabled" at B-B disables the protocol combo and the embed checkbox; OK saves and re-opening shows the same values; closing without changes asks nothing. Record the outcome in the commit message.

- [ ] **Step 8: Commit**

```
git add -A
git commit -m "Setup: replace the time-stamp switches with a PAdES level combo and a revocation group"
```

---

### Task 6: DER reader, qualified-certificate detector, signing-certificate pre-checks

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\DerReader.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\QualifiedCertificateDetector.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SigningCertificatePrecheck.vb`
- Create: `PDF Signer for Tungsten Capture Tests\DerReaderTests.vb`
- Create: `PDF Signer for Tungsten Capture Tests\QualifiedCertificateDetectorTests.vb`
- Create: `PDF Signer for Tungsten Capture Tests\SigningCertificatePrecheckTests.vb`

**Interfaces:**
- Produces: `DerReader.TryRead(Data As Byte(), ByRef Offset As Integer, ByRef Element As DerReader.DerElement) As Boolean`, `DerReader.Children(Content As Byte()) As List(Of DerReader.DerElement)`, `DerReader.IsOid(Element, EncodedOid As Byte()) As Boolean`; `QualifiedCertificateDetector.IsQualified(X509Certificate2) As Boolean`, `.ContainsQcCompliance(Byte()) As Boolean`; `SigningCertificatePrecheck.Check(Certificate As X509Certificate2, RequireOcspResponder As Boolean) As String` (Hungarian error or Nothing), `.HasSigningKeyUsage(X509Certificate2) As Boolean`, `.ChainIsComplete(X509Certificate2) As Boolean`, `.HasOcspResponder(X509Certificate2) As Boolean`, `.AiaContainsOcsp(Byte()) As Boolean`, `.CommonName(X509Certificate2) As String`.

- [ ] **Step 1: Write the failing tests**

`DerReaderTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class DerReaderTests
    <TestMethod>
    Public Sub Reads_a_short_form_element()
        Dim data As Byte() = {&H6, &H3, &H2A, &H3, &H4}
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsTrue(DerReader.TryRead(data, offset, e))
        Assert.AreEqual(CByte(&H6), e.Tag)
        CollectionAssert.AreEqual(New Byte() {&H2A, &H3, &H4}, e.Content)
        Assert.AreEqual(5, offset)
    End Sub

    <TestMethod>
    Public Sub Reads_a_long_form_length()
        Dim content(199) As Byte
        Dim data As New List(Of Byte) From {&H4, &H81, &HC8}
        data.AddRange(content)
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsTrue(DerReader.TryRead(data.ToArray(), offset, e))
        Assert.AreEqual(200, e.Content.Length)
        Assert.AreEqual(203, offset)
    End Sub

    <TestMethod>
    Public Sub Truncated_input_is_rejected()
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsFalse(DerReader.TryRead(New Byte() {&H30, &H5, &H1}, offset, e))
        Assert.IsFalse(DerReader.TryRead(New Byte() {&H30}, offset, e))
        Assert.IsFalse(DerReader.TryRead(Nothing, offset, e))
    End Sub

    <TestMethod>
    Public Sub Children_splits_a_sequence()
        Dim seq As Byte() = {&H2, &H1, &H5, &H6, &H2, &H2A, &H3}
        Dim children = DerReader.Children(seq)
        Assert.AreEqual(2, children.Count)
        Assert.AreEqual(CByte(&H2), children(0).Tag)
        Assert.IsTrue(DerReader.IsOid(children(1), New Byte() {&H2A, &H3}))
    End Sub
End Class
```

`QualifiedCertificateDetectorTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class QualifiedCertificateDetectorTests
    ' QCStatements ::= SEQUENCE OF QCStatement; second statement is id-etsi-qcs-QcCompliance (0.4.0.1862.1.1).
    Private Shared ReadOnly WithCompliance As Byte() = {
        &H30, &H12,
        &H30, &H6, &H6, &H4, &H2A, &H3, &H4, &H5,
        &H30, &H8, &H6, &H6, &H4, &H0, &H8E, &H46, &H1, &H1}

    Private Shared ReadOnly WithoutCompliance As Byte() = {
        &H30, &H8,
        &H30, &H6, &H6, &H4, &H2A, &H3, &H4, &H5}

    <TestMethod>
    Public Sub Detects_the_QcCompliance_statement()
        Assert.IsTrue(QualifiedCertificateDetector.ContainsQcCompliance(WithCompliance))
    End Sub

    <TestMethod>
    Public Sub Other_statements_do_not_count()
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(WithoutCompliance))
    End Sub

    <TestMethod>
    Public Sub Malformed_or_empty_input_is_not_qualified()
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(New Byte() {}))
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(New Byte() {&H30, &H9, &H1}))
    End Sub

    <TestMethod>
    Public Sub Certificate_without_the_extension_is_not_qualified()
        Using cert = TestCertificates.SelfSigned("CN=Plain", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(QualifiedCertificateDetector.IsQualified(cert))
        End Using
    End Sub
End Class
```

`SigningCertificatePrecheckTests.vb` (plus the shared certificate factory `Support\TestCertificates.vb`):

```vb
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

''' <summary>Self-signed test certificates built with CertificateRequest (available from .NET Framework 4.7.2).</summary>
Friend Module TestCertificates
    Friend Function SelfSigned(subject As String, signing As Boolean, notAfter As Date, Optional notBefore As Date? = Nothing) As X509Certificate2
        Using rsa As RSA = RSA.Create(2048)
            Dim request As New CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            Dim usage As X509KeyUsageFlags = If(signing, X509KeyUsageFlags.DigitalSignature, X509KeyUsageFlags.KeyEncipherment)
            request.CertificateExtensions.Add(New X509KeyUsageExtension(usage, critical:=True))
            Dim start As Date = If(notBefore, Date.Now.AddDays(-1))
            Dim cert As X509Certificate2 = request.CreateSelfSigned(New DateTimeOffset(start), New DateTimeOffset(notAfter))
            ' Re-import so the private key is attached in the persisted form SecureBlackbox and X509Chain expect.
            Return New X509Certificate2(cert.Export(X509ContentType.Pfx, "t"), "t", X509KeyStorageFlags.Exportable)
        End Using
    End Function
End Module
```

```vb
Imports PDFSigner

<TestClass>
Public Class SigningCertificatePrecheckTests
    ' AuthorityInfoAccess with one OCSP access description (1.3.6.1.5.5.7.48.1, URI http://ocsp.test).
    Private Shared ReadOnly AiaWithOcsp As Byte() = {
        &H30, &H1E, &H30, &H1C, &H6, &H8, &H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H1,
        &H86, &H10, &H68, &H74, &H74, &H70, &H3A, &H2F, &H2F, &H6F, &H63, &H73, &H70, &H2E, &H74, &H65, &H73, &H74}

    ' Same shape with caIssuers (…48.2) only.
    Private Shared ReadOnly AiaWithCaIssuersOnly As Byte() = {
        &H30, &H1E, &H30, &H1C, &H6, &H8, &H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H2,
        &H86, &H10, &H68, &H74, &H74, &H70, &H3A, &H2F, &H2F, &H6F, &H63, &H73, &H70, &H2E, &H74, &H65, &H73, &H74}

    <TestMethod>
    Public Sub Aia_with_ocsp_is_detected()
        Assert.IsTrue(SigningCertificatePrecheck.AiaContainsOcsp(AiaWithOcsp))
        Assert.IsFalse(SigningCertificatePrecheck.AiaContainsOcsp(AiaWithCaIssuersOnly))
        Assert.IsFalse(SigningCertificatePrecheck.AiaContainsOcsp(New Byte() {}))
    End Sub

    <TestMethod>
    Public Sub Key_usage_must_allow_signing()
        Using signing = TestCertificates.SelfSigned("CN=Signer", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsTrue(SigningCertificatePrecheck.HasSigningKeyUsage(signing))
        End Using
        Using other = TestCertificates.SelfSigned("CN=Encipher", signing:=False, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(SigningCertificatePrecheck.HasSigningKeyUsage(other))
            StringAssert.Contains(SigningCertificatePrecheck.Check(other, False), "Digital Signature")
        End Using
    End Sub

    <TestMethod>
    Public Sub Expired_certificate_is_reported()
        Using expired = TestCertificates.SelfSigned("CN=Old", signing:=True, notAfter:=Date.Now.AddDays(-1), notBefore:=Date.Now.AddDays(-10))
            StringAssert.Contains(SigningCertificatePrecheck.Check(expired, False), "már nem érvényes")
        End Using
    End Sub

    <TestMethod>
    Public Sub Untrusted_self_signed_certificate_fails_the_chain_check()
        Using cert = TestCertificates.SelfSigned("CN=Island", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(SigningCertificatePrecheck.ChainIsComplete(cert))
            StringAssert.Contains(SigningCertificatePrecheck.Check(cert, False), "tanúsítványlánc")
            Assert.AreEqual("Island", SigningCertificatePrecheck.CommonName(cert))
        End Using
    End Sub
End Class
```

- [ ] **Step 2: Build to see the tests fail**

Build. Expected: compile errors for the three undeclared modules.

- [ ] **Step 3: Write DerReader**

```vb
Imports System.Linq

''' <summary>Minimal DER reader for the two certificate extensions the module inspects (QC statements, authority information access).</summary>
Friend Module DerReader
    ''' <summary>One DER tag-length-value element: the tag byte and the content bytes.</summary>
    Friend Structure DerElement
        Friend Tag As Byte
        Friend Content As Byte()
    End Structure

    ''' <summary>Reads the element starting at Offset and advances Offset past it; False when the bytes end early or the length is unusable.</summary>
    Friend Function TryRead(Data As Byte(), ByRef Offset As Integer, ByRef Element As DerElement) As Boolean
        If Data Is Nothing OrElse Offset < 0 OrElse Offset + 2 > Data.Length Then Return False

        Dim tag As Byte = Data(Offset)
        Dim pos As Integer = Offset + 1
        Dim first As Integer = Data(pos)
        pos += 1

        Dim length As Integer
        If first < &H80 Then
            length = first
        Else
            ' Long form: the low bits give the byte count of the length (1-4 supported).
            Dim count As Integer = first And &H7F
            If count = 0 OrElse count > 4 OrElse pos + count > Data.Length Then Return False
            length = 0
            For i As Integer = 1 To count
                length = (length << 8) Or Data(pos)
                pos += 1
            Next
        End If

        If length < 0 OrElse pos + length > Data.Length Then Return False
        Dim content(length - 1) As Byte
        Array.Copy(Data, pos, content, 0, length)
        Element = New DerElement With {.Tag = tag, .Content = content}
        Offset = pos + length
        Return True
    End Function

    ''' <summary>Child elements of a constructed element's content, stopping at the first malformed element.</summary>
    Friend Function Children(Content As Byte()) As List(Of DerElement)
        Dim res As New List(Of DerElement)
        If Content Is Nothing Then Return res
        Dim offset As Integer = 0
        While offset < Content.Length
            Dim element As DerElement
            If Not TryRead(Content, offset, element) Then Exit While
            res.Add(element)
        End While
        Return res
    End Function

    ''' <summary>True when the element is an OBJECT IDENTIFIER with exactly the given encoded value.</summary>
    Friend Function IsOid(Element As DerElement, EncodedOid As Byte()) As Boolean
        Return Element.Tag = &H6 AndAlso Element.Content IsNot Nothing AndAlso Element.Content.SequenceEqual(EncodedOid)
    End Function
End Module
```

- [ ] **Step 4: Write QualifiedCertificateDetector**

```vb
Imports System.Linq
Imports System.Security.Cryptography.X509Certificates

''' <summary>Decides whether a certificate carries the ETSI QcCompliance statement, i.e. is a qualified certificate.</summary>
Friend Module QualifiedCertificateDetector
    Private Const QcStatementsExtensionOid As String = "1.3.6.1.5.5.7.1.3"
    ''' <summary>id-etsi-qcs-QcCompliance (0.4.0.1862.1.1) in DER content form.</summary>
    Private ReadOnly QcComplianceOid As Byte() = {&H4, &H0, &H8E, &H46, &H1, &H1}

    ''' <summary>True when the certificate's QC Statements extension lists QcCompliance.</summary>
    Friend Function IsQualified(Certificate As X509Certificate2) As Boolean
        Dim extension As X509Extension = Certificate.Extensions.Cast(Of X509Extension).FirstOrDefault(Function(x) x.Oid IsNot Nothing AndAlso x.Oid.Value = QcStatementsExtensionOid)
        If extension Is Nothing Then Return False
        Return ContainsQcCompliance(extension.RawData)
    End Function

    ''' <summary>True when the DER QCStatements value (SEQUENCE OF SEQUENCE { statementId, statementInfo }) contains QcCompliance.</summary>
    Friend Function ContainsQcCompliance(QcStatements As Byte()) As Boolean
        Dim offset As Integer = 0
        Dim outer As DerReader.DerElement
        If Not DerReader.TryRead(QcStatements, offset, outer) OrElse outer.Tag <> &H30 Then Return False

        For Each statement As DerReader.DerElement In DerReader.Children(outer.Content)
            If statement.Tag <> &H30 Then Continue For
            Dim parts As List(Of DerReader.DerElement) = DerReader.Children(statement.Content)
            If parts.Count > 0 AndAlso DerReader.IsOid(parts(0), QcComplianceOid) Then Return True
        Next
        Return False
    End Function
End Module
```

- [ ] **Step 5: Write SigningCertificatePrecheck**

```vb
Imports System.Linq
Imports System.Security.Cryptography.X509Certificates

''' <summary>Offline checks of the operator's signing certificate before any SecureBlackbox pass; every failure is a Hungarian operator message.</summary>
Friend Module SigningCertificatePrecheck
    Private Const AuthorityInformationAccessOid As String = "1.3.6.1.5.5.7.1.1"
    ''' <summary>id-ad-ocsp (1.3.6.1.5.5.7.48.1) in DER content form.</summary>
    Private ReadOnly OcspAccessMethodOid As Byte() = {&H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H1}

    ''' <summary>Returns the first failing check's message, or Nothing when the certificate can sign now.</summary>
    Friend Function Check(Certificate As X509Certificate2, RequireOcspResponder As Boolean) As String
        Dim name As String = CommonName(Certificate)
        If Certificate.NotBefore > Date.Now Then Return $"'{name}' tanúsítvány csak {Certificate.NotBefore} után érvényes!"
        If Certificate.NotAfter < Date.Now Then Return $"'{name}' tanúsítvány már nem érvényes (lejárt: {Certificate.NotAfter})!"
        If Not Certificate.HasPrivateKey Then Return $"'{name}' tanúsítvány nem tartalmaz privát kulcsot! Privát kulcs nélkül nem lehet aláírást létrehozni!"
        If Not HasSigningKeyUsage(Certificate) Then Return $"'{name}' tanúsítvány nem használható elektronikus aláírás készítésére (nincs beállítva a 'Digital Signature' vagy 'Non Repudiation' flag)!"
        If Not ChainIsComplete(Certificate) Then Return $"'{name}' tanúsítványhoz tartozó tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig!"
        If RequireOcspResponder AndAlso Not HasOcspResponder(Certificate) Then Return $"'{name}' tanúsítvány visszavonási állapota nem ellenőrizhető OCSP-vel, de a dokumentumtípus beállításai megkövetelik!"
        Return Nothing
    End Function

    ''' <summary>Subject common name, or the whole subject when it has none.</summary>
    Friend Function CommonName(Certificate As X509Certificate2) As String
        Try
            Dim cn As String = Certificate.GetNameInfo(X509NameType.SimpleName, False)
            Return If(String.IsNullOrEmpty(cn), Certificate.Subject, cn)
        Catch
            Return Certificate.Subject
        End Try
    End Function

    ''' <summary>True when a KeyUsage extension is present and allows DigitalSignature or NonRepudiation; an absent extension does not qualify.</summary>
    Friend Function HasSigningKeyUsage(Certificate As X509Certificate2) As Boolean
        For Each extension As X509Extension In Certificate.Extensions
            Dim usage As X509KeyUsageExtension = TryCast(extension, X509KeyUsageExtension)
            If usage IsNot Nothing Then
                Return (usage.KeyUsages And X509KeyUsageFlags.DigitalSignature) <> 0 OrElse
                       (usage.KeyUsages And X509KeyUsageFlags.NonRepudiation) <> 0
            End If
        Next
        Return False
    End Function

    ''' <summary>True when the Windows stores let the chain reach a trusted root; time and revocation findings are ignored here.</summary>
    Friend Function ChainIsComplete(Certificate As X509Certificate2) As Boolean
        Using chain As New X509Chain()
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid Or
                                                  X509VerificationFlags.IgnoreCtlNotTimeValid Or
                                                  X509VerificationFlags.IgnoreNotTimeNested
            chain.Build(Certificate)
            For Each status As X509ChainStatus In chain.ChainStatus
                If (status.Status And X509ChainStatusFlags.PartialChain) <> 0 OrElse
                   (status.Status And X509ChainStatusFlags.UntrustedRoot) <> 0 OrElse
                   (status.Status And X509ChainStatusFlags.NotSignatureValid) <> 0 Then
                    Return False
                End If
            Next
            Return True
        End Using
    End Function

    ''' <summary>True when any Authority Information Access extension names an OCSP responder.</summary>
    Friend Function HasOcspResponder(Certificate As X509Certificate2) As Boolean
        For Each extension As X509Extension In Certificate.Extensions
            If extension.Oid IsNot Nothing AndAlso extension.Oid.Value = AuthorityInformationAccessOid AndAlso AiaContainsOcsp(extension.RawData) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>True when the DER AuthorityInfoAccessSyntax (SEQUENCE OF SEQUENCE { accessMethod, accessLocation }) has an id-ad-ocsp entry.</summary>
    Friend Function AiaContainsOcsp(Aia As Byte()) As Boolean
        Dim offset As Integer = 0
        Dim outer As DerReader.DerElement
        If Not DerReader.TryRead(Aia, offset, outer) OrElse outer.Tag <> &H30 Then Return False

        For Each description As DerReader.DerElement In DerReader.Children(outer.Content)
            If description.Tag <> &H30 Then Continue For
            Dim parts As List(Of DerReader.DerElement) = DerReader.Children(description.Content)
            If parts.Count > 0 AndAlso DerReader.IsOid(parts(0), OcspAccessMethodOid) Then Return True
        Next
        Return False
    End Function
End Module
```

- [ ] **Step 6: Build and run the tests**

Expected: all tests pass (previous 19 + 4 + 4 + 4). If `CreateSelfSigned` is unavailable, the test project targets an older framework: it must be `net48`.

- [ ] **Step 7: Commit**

```
git add -A
git commit -m "Add DER reader, qualified-certificate detection and offline signing-certificate checks"
```

---

### Task 7: Translators — SBB names, chain context, error text, TSA URL

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbTranslator.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\ChainContextTracker.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbErrorTranslator.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\TsaUrlBuilder.vb`
- Create: `PDF Signer for Tungsten Capture Tests\SbbTranslatorTests.vb`, `ChainContextTrackerTests.vb`, `SbbErrorTranslatorTests.vb`, `TsaUrlBuilderTests.vb`

**Interfaces:**
- Produces: `SbbTranslator.HashAlgorithmName(Integer) As String`, `.RevocationCheck(RevocationType) As PDFSignerRevocationChecks`, `.RevocationCheckForValidator(RevocationType) As CertificateValidatorRevocationChecks`, `.ProxyAuth(ProxyAuthenticationMethod) As ProxyAuthTypes`, `.ProxyAuthName(ProxyAuthenticationMethod) As String`, `.ChainKind(Integer) As String`, `.EventKind(String) As String`, `.ChainValidity(ChainValidities) As String`, `.ValidatorChainValidity(CertificateValidatorChainValidationResults) As String`, `.SignatureValidity(SignatureValidities) As String`.
- `ChainContext` (class: `SubjectCommonName`, `IssuerCommonName`, `IsEmpty`, `Shared Empty`), `ChainContextTracker` (`Current`, `Update(subjectRdn, issuerRdn)`, `Reset()`, `Restore(ChainContext)`, `Shared CommonName(rdn)`).
- `SbbErrorTranslator.Describe(ErrorCode As Integer, Description As String, Context As ChainContext, Optional EntityPrefix As String = Nothing) As String`, `.WrapperText(Integer) As String`, `.ExceptionText(Exception) As String`, `.StripComponentPrefix(String) As String`, `.StripUserInfo(String) As String`.
- `TsaUrlBuilder.WithCredentials(TsaUrl As String, User As String, Password As String) As String`.

- [ ] **Step 1: Write the failing tests**

`SbbTranslatorTests.vb`:

```vb
Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports PDFSignerCommon

<TestClass>
Public Class SbbTranslatorTests
    <TestMethod>
    Public Sub Hash_ids_map_to_sbb_names()
        Assert.AreEqual("SHA1", SbbTranslator.HashAlgorithmName(HashType.SHA1))
        Assert.AreEqual("SHA224", SbbTranslator.HashAlgorithmName(HashType.SHA224))
        Assert.AreEqual("SHA256", SbbTranslator.HashAlgorithmName(HashType.SHA256))
        Assert.AreEqual("SHA384", SbbTranslator.HashAlgorithmName(HashType.SHA384))
        Assert.AreEqual("SHA512", SbbTranslator.HashAlgorithmName(HashType.SHA512))
    End Sub

    <TestMethod>
    Public Sub Unknown_hash_is_rejected()
        Assert.ThrowsException(Of NotSupportedException)(Sub() SbbTranslator.HashAlgorithmName(0))
    End Sub

    <TestMethod>
    Public Sub Revocation_types_map_to_both_component_enums()
        Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SbbTranslator.RevocationCheck(RevocationType.None))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyCRL, SbbTranslator.RevocationCheck(RevocationType.CRL))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, SbbTranslator.RevocationCheck(RevocationType.OCSP))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSPOrCRL, SbbTranslator.RevocationCheck(RevocationType.OCSPWithCRLFallback))
        Assert.AreEqual(CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL, SbbTranslator.RevocationCheckForValidator(RevocationType.OCSPWithCRLFallback))
        Assert.AreEqual(CertificateValidatorRevocationChecks.crcAnyCRL, SbbTranslator.RevocationCheckForValidator(RevocationType.CRL))
    End Sub

    <TestMethod>
    Public Sub Proxy_methods_map_to_sbb_auth_types()
        Assert.AreEqual(ProxyAuthTypes.patNoAuthentication, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.NoAuthentication))
        Assert.AreEqual(ProxyAuthTypes.patBasic, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.UserPassword))
        Assert.AreEqual(ProxyAuthTypes.patDigest, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.Digest))
        Assert.AreEqual(ProxyAuthTypes.patNTLM, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.NTLM))
        Assert.AreEqual("NTLM", SbbTranslator.ProxyAuthName(ProxyAuthenticationMethod.NTLM))
    End Sub

    <TestMethod>
    Public Sub Chain_kinds_and_event_kinds_are_hungarian()
        Assert.AreEqual("CRL", SbbTranslator.ChainKind(2))
        Assert.AreEqual("OCSP válasz", SbbTranslator.ChainKind(3))
        Assert.AreEqual("CRL letöltve", SbbTranslator.EventKind("CRLRetrieved"))
        Assert.AreEqual("SomethingNew", SbbTranslator.EventKind("SomethingNew"))
    End Sub

    <TestMethod>
    Public Sub Validities_are_hungarian()
        Assert.AreEqual("OK", SbbTranslator.ChainValidity(ChainValidities.cvtValid))
        Assert.AreEqual("a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig", SbbTranslator.ChainValidity(ChainValidities.cvtCantBeEstablished))
        Assert.AreEqual("a tanúsítvány érvénytelen", SbbTranslator.ValidatorChainValidity(CertificateValidatorChainValidationResults.cvtInvalid))
        Assert.AreEqual("az aláírás érvényes", SbbTranslator.SignatureValidity(SignatureValidities.svtValid))
    End Sub
End Class
```

`ChainContextTrackerTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class ChainContextTrackerTests
    <TestMethod>
    Public Sub Common_name_is_cut_out_of_a_slash_rdn()
        Assert.AreEqual("Name", ChainContextTracker.CommonName("/C=HU/O=DocSoft/CN=Name"))
        Assert.AreEqual("Name", ChainContextTracker.CommonName("/CN=Name/O=DocSoft"))
        Assert.AreEqual("/O=NoCn", ChainContextTracker.CommonName("/O=NoCn"))
        Assert.IsNull(ChainContextTracker.CommonName(Nothing))
    End Sub

    <TestMethod>
    Public Sub A_new_subject_drops_the_old_issuer_and_an_empty_issuer_keeps_it()
        Dim t As New ChainContextTracker
        t.Update("/CN=Leaf", "/CN=CA")
        Assert.AreEqual("CA", t.Current.IssuerCommonName)
        t.Update("/CN=Leaf", Nothing)
        Assert.AreEqual("CA", t.Current.IssuerCommonName)
        t.Update("/CN=Other", Nothing)
        Assert.AreEqual("Other", t.Current.SubjectCommonName)
        Assert.IsNull(t.Current.IssuerCommonName)
        t.Reset()
        Assert.IsTrue(t.Current.IsEmpty)
        t.Restore(New ChainContext("S", "I"))
        Assert.AreEqual("I", t.Current.IssuerCommonName)
    End Sub
End Class
```

`SbbErrorTranslatorTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class SbbErrorTranslatorTests
    Private Shared ReadOnly Tsa As New ChainContext("e-Szigno TSA 2020 01", "e-Szigno Qualified TSA CA 2020")

    <TestMethod>
    Public Sub Renders_the_reference_ocsp_line()
        Dim line As String = SbbErrorTranslator.Describe(28311557, "OCSP error 2002 (location: http://etsaca2020-ocsp1.e-szigno.hu)", Tsa, "'S0' aláírás-időbélyege")
        Assert.AreEqual(
            "'S0' aláírás-időbélyege: tanúsítványlánc visszavonás-ellenőrzési hiba: " &
            "nincs használható OCSP válasz ehhez a válaszadóhoz (offline módban: nincs megfelelő beágyazott válasz) " &
            "a(z) 'e-Szigno TSA 2020 01' tanúsítványhoz (kiadó: 'e-Szigno Qualified TSA CA 2020'), " &
            "válaszadó: http://etsaca2020-ocsp1.e-szigno.hu [SBB 28311557 / OCSP 2002]", line)
    End Sub

    <DataTestMethod>
    <DataRow(2001, "a kapott OCSP válasz elutasítva")>
    <DataRow(2003, "az OCSP válaszadó nem érhető el")>
    <DataRow(2005, "az OCSP válaszadó tanúsítványát visszavonták")>
    Public Sub Maps_ocsp_sub_codes(sub As Integer, fragment As String)
        Dim line As String = SbbErrorTranslator.Describe(28311557, $"OCSP error {sub} (location: http://ocsp.example)", Tsa)
        StringAssert.Contains(line, fragment)
        StringAssert.EndsWith(line, $"[SBB 28311557 / OCSP {sub}]")
    End Sub

    <DataTestMethod>
    <DataRow(1001, "a CRL elutasítva")>
    <DataRow(1004, "egyetlen CRL elosztási pont sem")>
    Public Sub Maps_crl_sub_codes(sub As Integer, fragment As String)
        Dim line As String = SbbErrorTranslator.Describe(28311557, $"CRL error {sub} (location: http://crl.example/a.crl)", Tsa)
        StringAssert.Contains(line, fragment)
        StringAssert.Contains(line, "elosztási pont: http://crl.example/a.crl")
    End Sub

    <TestMethod>
    Public Sub Empty_location_means_an_already_held_response()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2001 (location: )", Tsa), "válaszadó: nincs megadva (már meglévő válasz)")
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "CRL error 1001 (location: )", Tsa), "elosztási pont: nincs megadva (már meglévő CRL)")
    End Sub

    <TestMethod>
    Public Sub Unknown_codes_are_decoded_not_dropped()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2099 (location: http://x)", Tsa), "ismeretlen OCSP hiba")
        StringAssert.Contains(SbbErrorTranslator.Describe(&H1B00007, "OCSP error 2003 (location: http://x)", Nothing), "ismeretlen hiba (modul 0x1B0, hiba 7)")
        Assert.AreEqual("ismeretlen hiba (modul 0xFFFF, hiba 65535)", SbbErrorTranslator.WrapperText(-1))
    End Sub

    <TestMethod>
    Public Sub Non_template_descriptions_are_kept()
        Assert.AreEqual("általános hiba: Something odd happened [SBB 1048585]", SbbErrorTranslator.Describe(1048585, "Something odd happened", Nothing))
        Assert.AreEqual("általános hiba: Something odd happened – a(z) 'e-Szigno TSA 2020 01' tanúsítványhoz (kiadó: 'e-Szigno Qualified TSA CA 2020') [SBB 1048585]",
                        SbbErrorTranslator.Describe(1048585, "Something odd happened", Tsa))
        StringAssert.Contains(SbbErrorTranslator.Describe(1048585, "", Nothing), "(nincs leírás)")
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2003 (location: http://x)", ChainContext.Empty), "ismeretlen tanúsítványhoz")
    End Sub

    <TestMethod>
    Public Sub User_info_is_stripped_from_urls()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2003 (location: http://user:pw@ocsp.example/x)", Tsa), "válaszadó: http://ocsp.example/x")
        Assert.AreEqual("http://tsa.example/ts", SbbErrorTranslator.StripUserInfo("http://user:pw@tsa.example/ts"))
        Assert.AreEqual("https://tsa.example/ts?a=b@c", SbbErrorTranslator.StripUserInfo("https://tsa.example/ts?a=b@c"))
        Assert.AreEqual("tsa.example", SbbErrorTranslator.StripUserInfo("tsa.example"))
        Assert.IsNull(SbbErrorTranslator.StripUserInfo(Nothing))
    End Sub

    <TestMethod>
    Public Sub Component_prefixes_are_stripped_from_exception_text()
        Assert.AreEqual("Chain validation failed (721417/0xB0209)", SbbErrorTranslator.StripComponentPrefix("[SBPAdES.EElPDFAdvancedPublicKeySecurityHandlerError] Chain validation failed (721417/0xB0209)"))
        Assert.AreEqual("failure info is 25", SbbErrorTranslator.StripComponentPrefix("[SBCMS.EElCMSError] [SBTSPClient.EElTSPError] failure info is 25"))
        Assert.AreEqual("[tx=1] a hívás megszakadt", SbbErrorTranslator.StripComponentPrefix("[tx=1] a hívás megszakadt"))
        Assert.AreEqual("[SBPAdES hiba] szöveg", SbbErrorTranslator.StripComponentPrefix("[SBPAdES hiba] szöveg"))
        Assert.AreEqual("Chain validation failed", SbbErrorTranslator.ExceptionText(New Exception("[SBPAdES.EElPDFError] Chain validation failed")))
    End Sub
End Class
```

`TsaUrlBuilderTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class TsaUrlBuilderTests
    <TestMethod>
    Public Sub Missing_user_returns_the_url_unchanged()
        Assert.AreEqual("https://btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", Nothing, Nothing))
        Assert.AreEqual("https://btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "", "x"))
    End Sub

    <TestMethod>
    Public Sub Credentials_are_embedded_after_the_scheme_and_the_query_survives()
        Assert.AreEqual("https://2.1.19581:secret@btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "2.1.19581", "secret"))
        Assert.AreEqual("https://user:pw@www.netlock.hu/timestampm.cgi?promoid=mNbR649Uqh", TsaUrlBuilder.WithCredentials("https://www.netlock.hu/timestampm.cgi?promoid=mNbR649Uqh", "user", "pw"))
        Assert.AreEqual("http://user:pw@tsa.example.com/ts", TsaUrlBuilder.WithCredentials("http://tsa.example.com/ts", "user", "pw"))
    End Sub

    <DataTestMethod>
    <DataRow("a:b", "p@ss/wd", "https://a%3Ab:p%40ss%2Fwd@btsa.e-szigno.hu/tsa")>
    <DataRow("user", "jelszó", "https://user:jelsz%C3%B3@btsa.e-szigno.hu/tsa")>
    Public Sub Reserved_and_non_ascii_characters_are_percent_encoded(user As String, password As String, expected As String)
        Assert.AreEqual(expected, TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", user, password))
    End Sub

    <TestMethod>
    Public Sub Null_password_is_empty_and_an_invalid_url_throws()
        Assert.AreEqual("https://user:@btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "user", Nothing))
        Assert.ThrowsException(Of ArgumentException)(Sub() TsaUrlBuilder.WithCredentials("btsa.e-szigno.hu", "user", "pw"))
    End Sub
End Class
```

- [ ] **Step 2: Build to see the tests fail**

Build. Expected: compile errors for the four undeclared types.

- [ ] **Step 3: Write SbbTranslator**

```vb
Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon

''' <summary>Maps the provider's integer/enum settings onto SecureBlackbox names and enums, and SecureBlackbox enums onto Hungarian log text.</summary>
Friend Module SbbTranslator
    Private ReadOnly HashNames As New Dictionary(Of Integer, String) From {
        {HashType.SHA1, "SHA1"},
        {HashType.SHA224, "SHA224"},
        {HashType.SHA256, "SHA256"},
        {HashType.SHA384, "SHA384"},
        {HashType.SHA512, "SHA512"}}

    ''' <summary>SecureBlackbox hash algorithm name of a <see cref="HashType"/> value.</summary>
    Friend Function HashAlgorithmName(HashMethod As Integer) As String
        Dim name As String = Nothing
        If HashNames.TryGetValue(HashMethod, name) Then Return name
        Throw New NotSupportedException($"Nem támogatott lenyomatképző algoritmus azonosító: {HashMethod}")
    End Function

    Friend Function RevocationCheck(Protocol As RevocationType) As PDFSignerRevocationChecks
        Select Case Protocol
            Case RevocationType.None : Return PDFSignerRevocationChecks.crcNone
            Case RevocationType.CRL : Return PDFSignerRevocationChecks.crcAnyCRL
            Case RevocationType.OCSP : Return PDFSignerRevocationChecks.crcAnyOCSP
            Case RevocationType.OCSPWithCRLFallback : Return PDFSignerRevocationChecks.crcAnyOCSPOrCRL
            Case Else : Throw New NotSupportedException($"Nem támogatott visszavonás-ellenőrzési típus: {Protocol}")
        End Select
    End Function

    ''' <summary>Same mapping as <see cref="RevocationCheck"/> onto the CertificateValidator enum.</summary>
    Friend Function RevocationCheckForValidator(Protocol As RevocationType) As CertificateValidatorRevocationChecks
        Select Case Protocol
            Case RevocationType.None : Return CertificateValidatorRevocationChecks.crcNone
            Case RevocationType.CRL : Return CertificateValidatorRevocationChecks.crcAnyCRL
            Case RevocationType.OCSP : Return CertificateValidatorRevocationChecks.crcAnyOCSP
            Case RevocationType.OCSPWithCRLFallback : Return CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL
            Case Else : Throw New NotSupportedException($"Nem támogatott visszavonás-ellenőrzési típus: {Protocol}")
        End Select
    End Function

    Friend Function ProxyAuth(Method As ProxyAuthenticationMethod) As ProxyAuthTypes
        Select Case Method
            Case ProxyAuthenticationMethod.NoAuthentication : Return ProxyAuthTypes.patNoAuthentication
            Case ProxyAuthenticationMethod.UserPassword : Return ProxyAuthTypes.patBasic
            Case ProxyAuthenticationMethod.Digest : Return ProxyAuthTypes.patDigest
            Case ProxyAuthenticationMethod.NTLM : Return ProxyAuthTypes.patNTLM
            Case Else : Throw New NotSupportedException($"Nem támogatott proxy hitelesítési mód: {Method}")
        End Select
    End Function

    ''' <summary>Log name of a proxy authentication method.</summary>
    Friend Function ProxyAuthName(Method As ProxyAuthenticationMethod) As String
        Select Case Method
            Case ProxyAuthenticationMethod.UserPassword : Return "user/password"
            Case ProxyAuthenticationMethod.Digest : Return "digest"
            Case ProxyAuthenticationMethod.NTLM : Return "NTLM"
            Case Else : Return "nincs"
        End Select
    End Function

    ''' <summary>Hungarian name of a chain element kind reported by the download/needed events (1 certificate, 2 CRL, 3 OCSP).</summary>
    Friend Function ChainKind(Kind As Integer) As String
        Select Case Kind
            Case 1 : Return "tanúsítvány"
            Case 2 : Return "CRL"
            Case 3 : Return "OCSP válasz"
            Case Else : Return $"lánc elem ({Kind})"
        End Select
    End Function

    Private ReadOnly EventKinds As New Dictionary(Of String, String) From {
        {"CertProcessingStarted", "tanúsítvány feldolgozása"},
        {"CertProcessingCompleted", "tanúsítvány feldolgozva"},
        {"CertValidationStarted", "tanúsítvány ellenőrzése"},
        {"CertValidationCompleted", "tanúsítvány ellenőrizve"},
        {"CACertLocated", "CA tanúsítvány megtalálva"},
        {"CRLRetrieved", "CRL letöltve"},
        {"OCSPCheckCompleted", "OCSP ellenőrzés befejezve"},
        {"CertificateValidationBegin", "tanúsítvány ellenőrzése"},
        {"CertificateValidationEnd", "tanúsítvány ellenőrizve"},
        {"CertificateProcessingBegin", "tanúsítvány feldolgozása"},
        {"CertificateProcessingEnd", "tanúsítvány feldolgozva"},
        {"CertificateCRLRevocationCheckBegin", "CRL ellenőrzése"},
        {"CertificateCRLRevocationCheckEnd", "CRL ellenőrzés befejezve"},
        {"CertificateOCSPRevocationCheckBegin", "OCSP ellenőrzés"},
        {"CertificateOCSPRevocationCheckEnd", "OCSP ellenőrzés befejezve"},
        {"CRLValidationBegin", "CRL ellenőrzése"},
        {"CRLValidationEnd", "CRL ellenőrizve"},
        {"OCSPValidationBegin", "OCSP ellenőrzése"},
        {"OCSPValidationEnd", "OCSP ellenőrizve"}}

    ''' <summary>Hungarian name of a chain-validation progress step; unknown steps are returned unchanged because they are log-only.</summary>
    Friend Function EventKind(Kind As String) As String
        Dim text As String = Nothing
        If Kind IsNot Nothing AndAlso EventKinds.TryGetValue(Kind, text) Then Return text
        Return Kind
    End Function

    Friend Function ChainValidity(Validity As ChainValidities) As String
        Select Case Validity
            Case ChainValidities.cvtValid : Return "OK"
            Case ChainValidities.cvtValidButUntrusted : Return "a tanúsítvány érvényes, de nem megbízható"
            Case ChainValidities.cvtInvalid : Return "a tanúsítvány érvénytelen"
            Case ChainValidities.cvtCantBeEstablished : Return "a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig"
            Case Else : Return Validity.ToString()
        End Select
    End Function

    Friend Function ValidatorChainValidity(Validity As CertificateValidatorChainValidationResults) As String
        Select Case Validity
            Case CertificateValidatorChainValidationResults.cvtValid : Return "OK"
            Case CertificateValidatorChainValidationResults.cvtValidButUntrusted : Return "a tanúsítvány érvényes, de nem megbízható"
            Case CertificateValidatorChainValidationResults.cvtInvalid : Return "a tanúsítvány érvénytelen"
            Case CertificateValidatorChainValidationResults.cvtCantBeEstablished : Return "a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig"
            Case Else : Return Validity.ToString()
        End Select
    End Function

    Friend Function SignatureValidity(Validity As SignatureValidities) As String
        Select Case Validity
            Case SignatureValidities.svtValid : Return "az aláírás érvényes"
            Case SignatureValidities.svtUnknown : Return "az aláírás érvényessége ismeretlen"
            Case SignatureValidities.svtCorrupted : Return "az aláírás sérült"
            Case SignatureValidities.svtSignerNotFound : Return "az aláíró tanúsítvány nem található"
            Case SignatureValidities.svtFailure : Return "az aláírás ellenőrzése meghiúsult"
            Case SignatureValidities.svtReferenceCorrupted : Return "a hivatkozott adat sérült"
            Case Else : Return Validity.ToString()
        End Select
    End Function
End Module
```

- [ ] **Step 4: Write ChainContextTracker**

```vb
''' <summary>Subject and issuer common names of the certificate SecureBlackbox is processing; either may be Nothing.</summary>
Friend NotInheritable Class ChainContext
    Friend ReadOnly Property SubjectCommonName As String
    Friend ReadOnly Property IssuerCommonName As String

    Friend Sub New(SubjectCommonName As String, IssuerCommonName As String)
        Me.SubjectCommonName = SubjectCommonName
        Me.IssuerCommonName = IssuerCommonName
    End Sub

    ''' <summary>Context with no observed certificate.</summary>
    Friend Shared ReadOnly Empty As New ChainContext(Nothing, Nothing)

    Friend ReadOnly Property IsEmpty As Boolean
        Get
            Return String.IsNullOrEmpty(SubjectCommonName) AndAlso String.IsNullOrEmpty(IssuerCommonName)
        End Get
    End Property
End Class

''' <summary>Follows the certificate under validation from chain events so an error event, which names no certificate, can be attributed to it.</summary>
Friend NotInheritable Class ChainContextTracker
    Private _subject As String
    Private _issuer As String

    ''' <summary>Snapshot of the certificate currently being processed.</summary>
    Friend ReadOnly Property Current As ChainContext
        Get
            Return New ChainContext(_subject, _issuer)
        End Get
    End Property

    ''' <summary>Records the certificate (and issuer, when given) named by a chain event; a different subject starts a fresh context.</summary>
    Friend Sub Update(SubjectRdn As String, IssuerRdn As String)
        Dim subject As String = CommonName(SubjectRdn)
        Dim issuer As String = CommonName(IssuerRdn)
        If Not String.IsNullOrEmpty(subject) Then
            If Not String.Equals(subject, _subject, StringComparison.Ordinal) Then _issuer = Nothing
            _subject = subject
        End If
        If Not String.IsNullOrEmpty(issuer) Then _issuer = issuer
    End Sub

    Friend Sub Reset()
        _subject = Nothing
        _issuer = Nothing
    End Sub

    ''' <summary>Reinstates a snapshot taken from <see cref="Current"/>; Nothing empties the context.</summary>
    Friend Sub Restore(Context As ChainContext)
        _subject = Context?.SubjectCommonName
        _issuer = Context?.IssuerCommonName
    End Sub

    ''' <summary>CN of a SecureBlackbox slash-style RDN ("/C=HU/O=X/CN=Name" gives "Name"); the input itself when it has no CN.</summary>
    Friend Shared Function CommonName(Rdn As String) As String
        If String.IsNullOrEmpty(Rdn) Then Return Rdn
        Dim cnStart As Integer = Rdn.IndexOf("CN=", StringComparison.OrdinalIgnoreCase)
        If cnStart < 0 Then Return Rdn
        Dim cnEnd As Integer = Rdn.IndexOf("/"c, cnStart)
        Return If(cnEnd < 0, Rdn.Substring(cnStart + 3), Rdn.Substring(cnStart + 3, cnEnd - cnStart - 3))
    End Function
End Class
```

- [ ] **Step 5: Write SbbErrorTranslator**

```vb
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>Turns a SecureBlackbox error (code + description) or exception into one Hungarian log line; raw codes stay in a trailing "[SBB …]" suffix.</summary>
Friend Module SbbErrorTranslator
    ''' <summary>The two revocation templates SecureBlackbox formats: "OCSP error 2002 (location: http://…)" and "CRL error 1001 (location: )".</summary>
    Private ReadOnly RevocationTemplate As New Regex(
        "^\s*(?<family>OCSP|CRL)\s+error\s+(?<sub>[0-9]{1,9})\s*\(location:\s*(?<loc>[^)]*)\)\s*$",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

    Private ReadOnly OcspSubCodes As New Dictionary(Of Integer, String) From {
        {2001, "a kapott OCSP válasz elutasítva (a válaszadó tanúsítványlánca nem megbízható, vagy az aláírása érvénytelen)"},
        {2002, "nincs használható OCSP válasz ehhez a válaszadóhoz (offline módban: nincs megfelelő beágyazott válasz)"},
        {2003, "az OCSP válaszadó nem érhető el vagy elutasította a kérést"},
        {2004, "egyetlen OCSP válaszadó sem adott elfogadható választ"},
        {2005, "az OCSP válaszadó tanúsítványát visszavonták"}}

    Private ReadOnly CrlSubCodes As New Dictionary(Of Integer, String) From {
        {1001, "a CRL elutasítva (a kibocsátó lánca nem megbízható, vagy az aláírása érvénytelen)"},
        {1002, "nincs használható CRL ehhez a kibocsátóhoz (offline módban: nincs megfelelő beágyazott CRL)"},
        {1003, "a CRL nem tölthető le erről a helyről"},
        {1004, "egyetlen CRL elosztási pont sem adott használható CRL-t"},
        {1005, "a CRL aláírójának tanúsítványát visszavonták"}}

    ''' <summary>Module-level wrapper codes: the general SB_ERROR_* set, the PDF set, the PAdES chain failure and the validator revocation wrapper.</summary>
    Private ReadOnly WrapperCodes As New Dictionary(Of Integer, String) From {
        {1048577, "érvénytelen paraméter"},
        {1048578, "érvénytelen beállítás"},
        {1048579, "érvénytelen állapot"},
        {1048580, "érvénytelen érték"},
        {1048581, "a privát kulcs nem található"},
        {1048582, "a felhasználó megszakította"},
        {1048583, "a fájl nem található"},
        {1048584, "nem támogatott funkció vagy művelet"},
        {1048585, "általános hiba"},
        {26214401, "a bemeneti fájl nem létezik"},
        {26214402, "a fájl már titkosított"},
        {26214403, "a fájl nincs titkosítva"},
        {26214405, "érvénytelen jelszó"},
        {26214406, "a fájl visszafejtése sikertelen"},
        {26214407, "a dokumentum már alá van írva"},
        {26214408, "a dokumentum nincs aláírva"},
        {26214409, "ez az aláírástípus nem frissíthető"},
        {26214410, "nem támogatott PDF funkció vagy művelet"},
        {26214411, "nincs megadva időbélyeg szerver"},
        {26214412, "a komponens nincs szerkesztő módban"},
        {721417, "a tanúsítványlánc ellenőrzése sikertelen (PAdES)"},
        {28311556, "tanúsítványlánc-ellenőrzési hiba"},
        {28311557, "tanúsítványlánc visszavonás-ellenőrzési hiba"}}

    ''' <summary>Builds the log line; EntityPrefix is prepended when given. A non-template description omits the certificate clause when the context has no subject.</summary>
    Friend Function Describe(ErrorCode As Integer, Description As String, Context As ChainContext, Optional EntityPrefix As String = Nothing) As String
        Dim sb As New StringBuilder
        If Not String.IsNullOrEmpty(EntityPrefix) Then sb.Append(EntityPrefix).Append(": ")
        sb.Append(WrapperText(ErrorCode)).Append(": ")

        Dim m As Match = RevocationTemplate.Match(If(Description, String.Empty))
        If m.Success Then
            Dim ocsp As Boolean = m.Groups("family").Value.Equals("OCSP", StringComparison.OrdinalIgnoreCase)
            Dim subCode As Integer = Integer.Parse(m.Groups("sub").Value, CultureInfo.InvariantCulture)
            Dim location As String = m.Groups("loc").Value.Trim()
            Dim table As Dictionary(Of Integer, String) = If(ocsp, OcspSubCodes, CrlSubCodes)
            Dim text As String = Nothing
            sb.Append(If(table.TryGetValue(subCode, text), text, If(ocsp, "ismeretlen OCSP hiba", "ismeretlen CRL hiba")))
            sb.Append(" "c).Append(CertificateClause(Context)).Append(", ")
            If location.Length = 0 Then
                sb.Append(If(ocsp, "válaszadó: nincs megadva (már meglévő válasz)", "elosztási pont: nincs megadva (már meglévő CRL)"))
            Else
                sb.Append(If(ocsp, "válaszadó: ", "elosztási pont: ")).Append(StripUserInfo(location))
            End If
            sb.Append(" [SBB ").Append(ErrorCode).Append(If(ocsp, " / OCSP ", " / CRL ")).Append(subCode).Append("]"c)
        Else
            sb.Append(If(String.IsNullOrWhiteSpace(Description), "(nincs leírás)", Description.Trim()))
            If Context IsNot Nothing AndAlso Not String.IsNullOrEmpty(Context.SubjectCommonName) Then
                sb.Append(" – ").Append(CertificateClause(Context))
            End If
            sb.Append(" [SBB ").Append(ErrorCode).Append("]"c)
        End If
        Return sb.ToString()
    End Function

    ''' <summary>Hungarian text of a wrapper code; unknown codes are decoded into module (high 16 bits) and sub-error (low 16 bits).</summary>
    Friend Function WrapperText(ErrorCode As Integer) As String
        Dim text As String = Nothing
        If WrapperCodes.TryGetValue(ErrorCode, text) Then Return text
        Dim moduleId As Integer = (ErrorCode >> 16) And &HFFFF
        Dim subError As Integer = ErrorCode And &HFFFF
        Return $"ismeretlen hiba (modul 0x{moduleId:X}, hiba {subError})"
    End Function

    Private Function CertificateClause(Context As ChainContext) As String
        If Context Is Nothing OrElse String.IsNullOrEmpty(Context.SubjectCommonName) Then Return "ismeretlen tanúsítványhoz"
        If String.IsNullOrEmpty(Context.IssuerCommonName) Then Return $"a(z) '{Context.SubjectCommonName}' tanúsítványhoz"
        Return $"a(z) '{Context.SubjectCommonName}' tanúsítványhoz (kiadó: '{Context.IssuerCommonName}')"
    End Function

    ''' <summary>Message of an exception without the SecureBlackbox unit prefix it opens with.</summary>
    Friend Function ExceptionText(Ex As Exception) As String
        Return StripComponentPrefix(Ex?.Message)
    End Function

    ''' <summary>Strips every leading bracket holding a dotted, whitespace-free "SB…" unit name; any other leading bracket is message text.</summary>
    Friend Function StripComponentPrefix(Message As String) As String
        If String.IsNullOrEmpty(Message) Then Return Message
        Dim text As String = Message.TrimStart()
        While text.Length > 0 AndAlso text(0) = "["c
            Dim close As Integer = text.IndexOf("]"c)
            If close < 0 Then Exit While
            Dim unit As String = text.Substring(1, close - 1)
            If Not unit.StartsWith("SB", StringComparison.Ordinal) OrElse Not unit.Contains(".") OrElse unit.IndexOfAny({" "c, vbTab(0)}) >= 0 Then Exit While
            text = text.Substring(close + 1).TrimStart()
        End While
        Return text
    End Function

    ''' <summary>Removes "user:password@" from the authority of a URL; an '@' in the path or query is left alone.</summary>
    Friend Function StripUserInfo(Url As String) As String
        If String.IsNullOrEmpty(Url) Then Return Url
        Dim schemeEnd As Integer = Url.IndexOf("://", StringComparison.Ordinal)
        If schemeEnd < 0 Then Return Url
        Dim authorityStart As Integer = schemeEnd + 3
        Dim authorityEnd As Integer = Url.IndexOf("/"c, authorityStart)
        Dim at As Integer = Url.IndexOf("@"c, authorityStart)
        If at >= 0 AndAlso (authorityEnd < 0 OrElse at < authorityEnd) Then Return Url.Remove(authorityStart, at - authorityStart + 1)
        Return Url
    End Function
End Module
```

- [ ] **Step 6: Write TsaUrlBuilder**

```vb
''' <summary>Builds the PDFSigner.TimestampServer URL; credentials go into the URL because OnTimestampRequest is not raised for http(s) in this SecureBlackbox build.</summary>
Friend Module TsaUrlBuilder
    ''' <summary>The URL with user and password percent-encoded as its user-info part, or unchanged when no user is configured. The result holds the plaintext password; never log it.</summary>
    Friend Function WithCredentials(TsaUrl As String, User As String, Password As String) As String
        If String.IsNullOrEmpty(User) Then Return TsaUrl

        Dim schemeEnd As Integer = TsaUrl.IndexOf("://", StringComparison.Ordinal)
        If schemeEnd < 0 Then Throw New ArgumentException($"Érvénytelen időbélyeg szolgáltató URL: '{TsaUrl}'", NameOf(TsaUrl))

        Dim userInfo As String = Uri.EscapeDataString(User) & ":" & Uri.EscapeDataString(If(Password, String.Empty))
        Return TsaUrl.Insert(schemeEnd + 3, userInfo & "@")
    End Function
End Module
```

- [ ] **Step 7: Build and run the tests**

Expected: all tests pass. If `Uri.EscapeDataString("jelszó")` yields something other than `jelsz%C3%B3`, the test project's `<LangVersion>` or file encoding is wrong: the test source files must be saved as UTF-8 with BOM (VB reads ANSI otherwise).

- [ ] **Step 8: Commit**

```
git add -A
git commit -m "Add SecureBlackbox translators, chain context tracking and TSA URL builder"
```

---

### Task 8: Signing plan and builder

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SigningPlan.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SigningPlanBuilder.vb`
- Create: `PDF Signer for Tungsten Capture Tests\SigningPlanBuilderTests.vb`

**Interfaces:**
- Consumes: `PDFSignerCryptoProvider.PAdESLevel`, `EnableRevocationChecking`, `RevocationCheckProtocol`; `SbbTranslator.RevocationCheck`.
- Produces: `SigningPlan` with `SignatureLevel As PAdESSignatureLevels`, `Revocation As PDFSignerRevocationChecks`, `SignPassRevocationCheck As PDFSignerRevocationChecks`, `EmbedSignatureTimestamp`, `CheckSigningCertRevocation`, `CheckTimestampCertRevocation`, `UpdateToEmbed`, `AddDocumentTimestamp`, `EmbedDocumentTimestampRevocation` (all Boolean); `SigningPlanBuilder.Build(Settings As PDFSignerCryptoProvider) As SigningPlan`.

- [ ] **Step 1: Write the failing tests**

```vb
Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports PDFSignerCommon

<TestClass>
Public Class SigningPlanBuilderTests
    Private Shared Function Cfg(level As PAdESLevelType, Optional checking As Boolean = False, Optional protocol As RevocationType = RevocationType.OCSP) As PDFSignerCryptoProvider
        Return New PDFSignerCryptoProvider With {.PAdESLevel = level, .EnableRevocationChecking = checking, .RevocationCheckProtocol = protocol, .EmbedRevocationInformation = True}
    End Function

    <TestMethod>
    Public Sub BaselineB_is_signature_only()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineB, p.SignatureLevel)
        Assert.IsFalse(p.EmbedSignatureTimestamp)
        Assert.IsFalse(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
        Assert.IsFalse(p.EmbedDocumentTimestampRevocation)
    End Sub

    <TestMethod>
    Public Sub BaselineT_signs_with_a_signature_timestamp_and_no_update()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.EmbedSignatureTimestamp)
        Assert.IsFalse(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
    End Sub

    <TestMethod>
    Public Sub BaselineLT_is_two_phase_without_document_timestamp()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLT, checking:=True))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.EmbedSignatureTimestamp)
        Assert.IsTrue(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
        Assert.IsFalse(p.EmbedDocumentTimestampRevocation)
    End Sub

    <TestMethod>
    Public Sub BaselineLTA_adds_a_lean_document_timestamp_and_its_own_update()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=True))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.UpdateToEmbed)
        Assert.IsTrue(p.AddDocumentTimestamp)
        Assert.IsTrue(p.EmbedDocumentTimestampRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=False)).EmbedDocumentTimestampRevocation)
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=False)).AddDocumentTimestamp)
    End Sub

    <TestMethod>
    Public Sub Checking_off_forces_crcNone_and_checking_on_maps_the_protocol()
        Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=False)).Revocation)
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyCRL, SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=True, protocol:=RevocationType.CRL)).Revocation)
    End Sub

    <TestMethod>
    Public Sub Non_deferred_levels_check_during_sign_and_deferred_levels_sign_lean()
        For Each level In {PAdESLevelType.BaselineB, PAdESLevelType.BaselineT}
            Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, SigningPlanBuilder.Build(Cfg(level, checking:=True)).SignPassRevocationCheck, level.ToString())
            Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SigningPlanBuilder.Build(Cfg(level, checking:=False)).SignPassRevocationCheck, level.ToString())
        Next
        For Each level In {PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Dim p = SigningPlanBuilder.Build(Cfg(level, checking:=True))
            Assert.AreEqual(PDFSignerRevocationChecks.crcNone, p.SignPassRevocationCheck, level.ToString())
            Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, p.Revocation, level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Explicit_validator_checks_belong_to_the_non_embedding_levels_only()
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=True)).CheckSigningCertRevocation)
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=True)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=False)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLT, checking:=True)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=True)).CheckSigningCertRevocation)

        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=True)).CheckTimestampCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=False)).CheckTimestampCertRevocation)
        For Each level In {PAdESLevelType.BaselineB, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.IsFalse(SigningPlanBuilder.Build(Cfg(level, checking:=True)).CheckTimestampCertRevocation, level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Undefined_level_is_rejected()
        Assert.ThrowsException(Of NotSupportedException)(Sub() SigningPlanBuilder.Build(New PDFSignerCryptoProvider With {.PAdESLevel = 0}))
    End Sub
End Class
```

- [ ] **Step 2: Build to see the tests fail**

Expected: compile errors for `SigningPlan` / `SigningPlanBuilder`.

- [ ] **Step 3: Write SigningPlan**

```vb
Imports nsoftware.SecureBlackbox

''' <summary>The passes one signing call runs, decided once from the provider settings.</summary>
Friend NotInheritable Class SigningPlan
    ''' <summary>Level of the Sign pass: paslBaselineB, or paslBaselineT (B-LT/B-LTA are lifted later by Update).</summary>
    Friend Property SignatureLevel As PAdESSignatureLevels
    ''' <summary>Revocation policy of the checking passes: the configured protocol, or crcNone when checking is off.</summary>
    Friend Property Revocation As PDFSignerRevocationChecks
    ''' <summary>RevocationCheck applied by the Sign passes: equals <see cref="Revocation"/> for B-B/B-T, crcNone for the two-pass levels.</summary>
    Friend Property SignPassRevocationCheck As PDFSignerRevocationChecks
    ''' <summary>True when the Sign pass requests a signature timestamp from the TSA.</summary>
    Friend Property EmbedSignatureTimestamp As Boolean
    ''' <summary>True when a fail-closed CertificateValidator pass checks the signing chain before signing (B-B/B-T with checking on).</summary>
    Friend Property CheckSigningCertRevocation As Boolean
    ''' <summary>True when a fail-closed CertificateValidator pass checks the TSA chain after signing (B-T with checking on).</summary>
    Friend Property CheckTimestampCertRevocation As Boolean
    ''' <summary>True when an Update pass collects and embeds the new signature's revocation data (B-LT/B-LTA).</summary>
    Friend Property UpdateToEmbed As Boolean
    ''' <summary>True when a lean document-timestamp pass appends the archive timestamp (B-LTA).</summary>
    Friend Property AddDocumentTimestamp As Boolean
    ''' <summary>True when a further Update pass embeds the archive timestamp's own TSA-chain revocation (B-LTA with checking on).</summary>
    Friend Property EmbedDocumentTimestampRevocation As Boolean
End Class
```

- [ ] **Step 4: Write SigningPlanBuilder**

```vb
Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon

''' <summary>Pure mapping from the provider settings to a <see cref="SigningPlan"/>; no I/O.</summary>
Friend Module SigningPlanBuilder
    Friend Function Build(Settings As PDFSignerCryptoProvider) As SigningPlan
        Dim protocol As PDFSignerRevocationChecks = SbbTranslator.RevocationCheck(CType(Settings.RevocationCheckProtocol, RevocationType))
        Dim revocation As PDFSignerRevocationChecks = If(Settings.EnableRevocationChecking, protocol, PDFSignerRevocationChecks.crcNone)
        Dim level As Integer = Settings.PAdESLevel

        Select Case level
            Case PAdESLevelType.BaselineB
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineB,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = revocation,
                    .CheckSigningCertRevocation = Settings.EnableRevocationChecking}
            Case PAdESLevelType.BaselineT
                ' A non-LTV Sign() does not check revocation itself, so the two chains get explicit validator checks.
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineT,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = revocation,
                    .EmbedSignatureTimestamp = True,
                    .CheckSigningCertRevocation = Settings.EnableRevocationChecking,
                    .CheckTimestampCertRevocation = Settings.EnableRevocationChecking}
            Case PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA
                ' Sign lean at B-T, then Update() checks and embeds once; B-LTA appends the archive timestamp afterwards.
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineT,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = PDFSignerRevocationChecks.crcNone,
                    .EmbedSignatureTimestamp = True,
                    .UpdateToEmbed = True,
                    .AddDocumentTimestamp = (level = PAdESLevelType.BaselineLTA),
                    .EmbedDocumentTimestampRevocation = (level = PAdESLevelType.BaselineLTA) AndAlso Settings.EnableRevocationChecking}
            Case Else
                Throw New NotSupportedException($"Nem támogatott PAdES szint: {level}")
        End Select
    End Function
End Module
```

- [ ] **Step 5: Build and run the tests**

Expected: all tests pass.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Add the signing plan builder for the four PAdES baseline levels"
```

---

### Task 9: Entity naming and moment formatting

**Files:**
- Create: `PDF Signer for Tungsten Capture\Helper\MomentFormat.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\EntityNaming.vb`
- Create: `PDF Signer for Tungsten Capture Tests\MomentFormatTests.vb`, `EntityNamingTests.vb`

**Interfaces:**
- Produces: `MomentFormat.DateTimePattern`, `MomentFormat.Log(DateTimeOffset) As String` ("local (utc UTC)"), `MomentFormat.Display(DateTimeOffset, Utc As Boolean) As String`, `MomentFormat.FromUtc(DateTime) As DateTimeOffset`.
- `EntityNaming` with `Shared Empty`, `Shared EntityKey(SbbLabel, DocumentOrder, IsDocumentTimestamp) As String`, `Shared Build(Entities As IList(Of EntityDescriptor), Timestamps As IList(Of TimestampDescriptor)) As EntityNaming`, `Display(SbbLabel) As String`, `DisplayTimestamp(ParentSbbLabel, Ordinal) As String`, `Shared FromVerifier(PDFVerifier) As EntityNaming`; nested structures `EntityDescriptor(SbbLabel, IsDocumentTimestamp)` and `TimestampDescriptor(SbbLabel, ParentSbbLabel)`.

- [ ] **Step 1: Write the failing tests**

`MomentFormatTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class MomentFormatTests
    <TestMethod>
    Public Sub Log_form_carries_local_then_utc()
        Dim text As String = MomentFormat.Log(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero))
        StringAssert.EndsWith(text, "(2026.09.08. 17:18:20 UTC)")
        StringAssert.Matches(text, New Text.RegularExpressions.Regex("^\d{4}\.\d{2}\.\d{2}\. \d{2}:\d{2}:\d{2} \("))
    End Sub

    <TestMethod>
    Public Sub Display_utc_is_the_compact_form()
        Assert.AreEqual("2026.09.08. 17:18:20 UTC", MomentFormat.Display(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero), Utc:=True))
        Assert.AreEqual(TimeSpan.Zero, MomentFormat.FromUtc(New Date(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)).Offset)
    End Sub
End Class
```

`EntityNamingTests.vb`:

```vb
Imports PDFSigner

<TestClass>
Public Class EntityNamingTests
    <TestMethod>
    Public Sub Signatures_document_timestamps_and_own_timestamps_get_display_ids()
        Dim naming = EntityNaming.Build(
            {New EntityNaming.EntityDescriptor("Signature1", False), New EntityNaming.EntityDescriptor("DocTS1", True)},
            {New EntityNaming.TimestampDescriptor("TS1", "Signature1"), New EntityNaming.TimestampDescriptor("TS2", "DocTS1")})
        Assert.AreEqual("S0", naming.Display("Signature1"))
        Assert.AreEqual("T0", naming.Display("DocTS1"))
        Assert.AreEqual("S0T0", naming.Display("TS1"))
        Assert.AreEqual("T0", naming.Display("TS2"))
        Assert.AreEqual("S0T1", naming.DisplayTimestamp("Signature1", 1))
        Assert.AreEqual("Unknown", naming.Display("Unknown"))
        Assert.IsNull(naming.Display(Nothing))
    End Sub

    <TestMethod>
    Public Sub Entity_key_falls_back_to_the_document_order()
        Assert.AreEqual("Sig", EntityNaming.EntityKey("Sig", 3, False))
        Assert.AreEqual("S3", EntityNaming.EntityKey("", 3, False))
        Assert.AreEqual("DT2", EntityNaming.EntityKey(Nothing, 2, True))
        Assert.AreEqual("x", EntityNaming.Empty.Display("x"))
    End Sub
End Class
```

- [ ] **Step 2: Build to see the tests fail**

Expected: compile errors for `MomentFormat` / `EntityNaming`.

- [ ] **Step 3: Write MomentFormat**

`PDF Signer for Tungsten Capture\Helper\MomentFormat.vb`:

```vb
Imports System.Globalization

''' <summary>Renders moments for the signature log: local time first, the UTC instant in parentheses.</summary>
Friend Module MomentFormat
    ''' <summary>Dotted date and time to the second.</summary>
    Friend Const DateTimePattern As String = "yyyy.MM.dd. HH:mm:ss"

    ''' <summary>Wraps a UTC wall-clock value regardless of its Kind.</summary>
    Friend Function FromUtc(Utc As Date) As DateTimeOffset
        Return New DateTimeOffset(Date.SpecifyKind(Utc, DateTimeKind.Utc))
    End Function

    ''' <summary>"2026.09.08. 19:18:20 (2026.09.08. 17:18:20 UTC)" in the process time zone.</summary>
    Friend Function Log(Moment As DateTimeOffset) As String
        Dim local As String = TimeZoneInfo.ConvertTime(Moment, TimeZoneInfo.Local).ToString(DateTimePattern, CultureInfo.InvariantCulture)
        Dim utc As String = Moment.UtcDateTime.ToString(DateTimePattern, CultureInfo.InvariantCulture)
        Return $"{local} ({utc} UTC)"
    End Function

    ''' <summary>One half only: the UTC instant suffixed with " UTC", or the local time without a marker.</summary>
    Friend Function Display(Moment As DateTimeOffset, Utc As Boolean) As String
        If Utc Then Return Moment.UtcDateTime.ToString(DateTimePattern, CultureInfo.InvariantCulture) & " UTC"
        Return TimeZoneInfo.ConvertTime(Moment, TimeZoneInfo.Local).ToString(DateTimePattern, CultureInfo.InvariantCulture)
    End Function
End Module
```

- [ ] **Step 4: Write EntityNaming**

```vb
Imports nsoftware.SecureBlackbox

''' <summary>Display ids of a document's signing entities for the log: signatures S0, S1…, document timestamps T0, T1…, a signature's own timestamps S0T0…</summary>
Friend NotInheritable Class EntityNaming
    ''' <summary>One PDFSignature entity in document order.</summary>
    Friend Structure EntityDescriptor
        Friend ReadOnly SbbLabel As String
        Friend ReadOnly IsDocumentTimestamp As Boolean
        Friend Sub New(SbbLabel As String, IsDocumentTimestamp As Boolean)
            Me.SbbLabel = SbbLabel
            Me.IsDocumentTimestamp = IsDocumentTimestamp
        End Sub
    End Structure

    ''' <summary>One TimestampInfo in SecureBlackbox list order with the label of the entity it seals.</summary>
    Friend Structure TimestampDescriptor
        Friend ReadOnly SbbLabel As String
        Friend ReadOnly ParentSbbLabel As String
        Friend Sub New(SbbLabel As String, ParentSbbLabel As String)
            Me.SbbLabel = SbbLabel
            Me.ParentSbbLabel = ParentSbbLabel
        End Sub
    End Structure

    ''' <summary>Maps nothing: every label passes through.</summary>
    Friend Shared ReadOnly Empty As New EntityNaming(New Dictionary(Of String, String)(StringComparer.Ordinal), New HashSet(Of String)(StringComparer.Ordinal))

    Private ReadOnly _display As Dictionary(Of String, String)
    Private ReadOnly _documentTimestamps As HashSet(Of String)

    Private Sub New(Display As Dictionary(Of String, String), DocumentTimestamps As HashSet(Of String))
        _display = Display
        _documentTimestamps = DocumentTimestamps
    End Sub

    ''' <summary>The key an entity is tracked by: its SecureBlackbox label, or "S"/"DT" plus document order when it has none.</summary>
    Friend Shared Function EntityKey(SbbLabel As String, DocumentOrder As Integer, IsDocumentTimestamp As Boolean) As String
        If Not String.IsNullOrEmpty(SbbLabel) Then Return SbbLabel
        Return If(IsDocumentTimestamp, $"DT{DocumentOrder}", $"S{DocumentOrder}")
    End Function

    ''' <summary>Builds the map from entities (document order) and timestamps (SecureBlackbox order); a timestamp label never overrides an entity label.</summary>
    Friend Shared Function Build(Entities As IList(Of EntityDescriptor), Timestamps As IList(Of TimestampDescriptor)) As EntityNaming
        Dim display As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim documentTimestamps As New HashSet(Of String)(StringComparer.Ordinal)
        Dim signatures As Integer = 0
        Dim docTimestamps As Integer = 0
        For Each e As EntityDescriptor In Entities
            If String.IsNullOrEmpty(e.SbbLabel) Then Continue For
            If e.IsDocumentTimestamp Then
                display(e.SbbLabel) = $"T{docTimestamps}"
                docTimestamps += 1
                documentTimestamps.Add(e.SbbLabel)
            Else
                display(e.SbbLabel) = $"S{signatures}"
                signatures += 1
            End If
        Next

        Dim naming As New EntityNaming(display, documentTimestamps)
        Dim ordinalByParent As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        For Each t As TimestampDescriptor In Timestamps
            If String.IsNullOrEmpty(t.SbbLabel) OrElse String.IsNullOrEmpty(t.ParentSbbLabel) Then Continue For
            Dim ordinal As Integer = 0
            ordinalByParent.TryGetValue(t.ParentSbbLabel, ordinal)
            ordinalByParent(t.ParentSbbLabel) = ordinal + 1
            If Not display.ContainsKey(t.SbbLabel) Then display(t.SbbLabel) = naming.DisplayTimestamp(t.ParentSbbLabel, ordinal)
        Next
        Return naming
    End Function

    ''' <summary>Display id of a label; an unknown, empty or Nothing label is returned as is.</summary>
    Friend Function Display(SbbLabel As String) As String
        Dim id As String = Nothing
        If SbbLabel IsNot Nothing AndAlso _display.TryGetValue(SbbLabel, id) Then Return id
        Return SbbLabel
    End Function

    ''' <summary>Display id of the Ordinal-th (0-based) timestamp sealing the parent: "S0T1" under a signature, the parent's own id under a document timestamp.</summary>
    Friend Function DisplayTimestamp(ParentSbbLabel As String, Ordinal As Integer) As String
        If ParentSbbLabel IsNot Nothing AndAlso _documentTimestamps.Contains(ParentSbbLabel) Then Return Display(ParentSbbLabel)
        Return $"{Display(ParentSbbLabel)}T{Ordinal}"
    End Function

    ''' <summary>Reads the entity and timestamp lists of a verifier that has run Verify() (parse-only is enough).</summary>
    Friend Shared Function FromVerifier(Verifier As PDFVerifier) As EntityNaming
        Dim entities As New List(Of EntityDescriptor)
        For i As Integer = 0 To Verifier.Signatures.Count - 1
            Dim s As PDFSignature = Verifier.Signatures(i)
            Dim isDocTs As Boolean = s.SignatureType = PDFSignatureTypes.pstDocumentTimestamp
            entities.Add(New EntityDescriptor(EntityKey(s.EntityLabel, i, isDocTs), isDocTs))
        Next
        Dim timestamps As New List(Of TimestampDescriptor)
        For j As Integer = 0 To Verifier.Timestamps.Count - 1
            Dim t As TimestampInfo = Verifier.Timestamps(j)
            timestamps.Add(New TimestampDescriptor(t.EntityLabel, t.ParentEntity))
        Next
        Return Build(entities, timestamps)
    End Function
End Class
```

- [ ] **Step 5: Build and run the tests**

Expected: all tests pass.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Add entity display naming and moment formatting for the signature log"
```

---

### Task 10: ETSI end-of-validity models and calculator

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\EtsiValidity\ValidityModels.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\EtsiValidity\AlgorithmSunsetTable.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\EtsiValidity\EtsiValidityCalculator.vb`
- Create: `PDF Signer for Tungsten Capture Tests\EtsiValidityCalculatorTests.vb`

**Interfaces:**
- Produces (all `Friend`): enums `ValidityStatus { ValidUntil, Expired, BrokenChain, MissingRevocationData, Invalid, SuspectTimeline }`, `LayerKind { SignatureTimestamp, DocumentTimestamp }`, `AggregationPolicy { All, Any }`; classes `RevocationInfo` (`Revoked`, `RevocationTime`, `Reason`, `InvalidityDate As DateTimeOffset?`, `EffectiveR()`), `CertInfo` (`NotBefore`, `NotAfter`, `Revocation`, `CommonName`), `AlgorithmId`, `Layer` (`Kind`, `DisplayId`, `DocumentOrderIndex`, `TrustedTime`, `CertPath As List(Of CertInfo)`, `Algorithm`, `MissingRevocationFrom As DateTimeOffset?`, `MissingData`), `SignatureInput` (`Id`, `DisplayId`, `DocumentOrderIndex`, `SignerPath`, `Algorithm`, `ClaimedSigningTime?`, `CoveringTimestamps As List(Of Layer)`, `MissingData`, `SignerLeaf`), `DocumentValidationInput` (`Aggregation`, `Signatures`, `Sunset`), `PerSignatureResult` (`SignatureId`, `DisplayId`, `Expiry`, `EvidenceExpiry`, `Status`, `GapBefore?`, `Trace As List(Of String)`), `DocumentValidityResult` (`Expiry`, `EvidenceExpiry`, `Status`, `PerSignature`); `AlgorithmSunsetTable` (`Shared Empty`, `EarliestSunset(AlgorithmId) As DateTimeOffset`); `EtsiValidityCalculator.Compute(DocumentValidationInput, AtTime As DateTimeOffset) As DocumentValidityResult`, `.TraceMoment(DateTimeOffset) As String`, `.OutcomeMoment(DateTimeOffset) As String`.

- [ ] **Step 1: Write the failing tests**

```vb
Imports PDFSigner

<TestClass>
Public Class EtsiValidityCalculatorTests
    Private Shared Function Y(year As Integer) As DateTimeOffset
        Return New DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero)
    End Function

    Private Shared Function Cert(notAfter As Integer, Optional notBefore As Integer = 2000, Optional rev As RevocationInfo = Nothing) As CertInfo
        Return New CertInfo With {.NotBefore = Y(notBefore), .NotAfter = Y(notAfter), .Revocation = rev, .CommonName = "CN"}
    End Function

    Private Shared Function Ts(kind As LayerKind, genTime As Integer, tsaNotAfter As Integer, docOrder As Integer,
                               Optional rev As RevocationInfo = Nothing, Optional missingFrom As Integer? = Nothing, Optional missingData As Boolean = False) As Layer
        Return New Layer With {
            .Kind = kind, .DocumentOrderIndex = docOrder, .TrustedTime = Y(genTime),
            .CertPath = New List(Of CertInfo) From {Cert(tsaNotAfter, rev:=rev)},
            .MissingRevocationFrom = If(missingFrom.HasValue, Y(missingFrom.Value), CType(Nothing, DateTimeOffset?)),
            .MissingData = missingData}
    End Function

    Private Shared Function Sig(id As String, signerNotAfter As Integer, docOrder As Integer, Optional signerRev As RevocationInfo = Nothing,
                                Optional missingData As Boolean = False, ParamArray covering As Layer()) As SignatureInput
        Return New SignatureInput With {
            .Id = id, .DocumentOrderIndex = docOrder,
            .SignerPath = New List(Of CertInfo) From {Cert(signerNotAfter, rev:=signerRev)},
            .CoveringTimestamps = covering.ToList(), .MissingData = missingData}
    End Function

    Private Shared Function Run(atTime As DateTimeOffset, ParamArray sigs As SignatureInput()) As DocumentValidityResult
        Return EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Signatures = sigs.ToList()}, atTime)
    End Function

    Private Shared Function Revoked(time As Integer, Optional reason As String = "affiliationChanged", Optional invalidityDate As Integer? = Nothing) As RevocationInfo
        Return New RevocationInfo With {.Revoked = True, .RevocationTime = Y(time), .Reason = reason,
            .InvalidityDate = If(invalidityDate.HasValue, Y(invalidityDate.Value), CType(Nothing, DateTimeOffset?))}
    End Function

    <TestMethod> Public Sub A_plain_signature()
        Dim r = Run(Y(2020), Sig("S", 2026, 0))
        Assert.AreEqual(Y(2026), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub B_inner_timestamp_does_not_shorten()
        Dim r = Run(Y(2020), Sig("S", 2030, 0, Nothing, False, Ts(LayerKind.SignatureTimestamp, 2024, 2026, 1)))
        Assert.AreEqual(Y(2030), r.Expiry)
    End Sub

    <TestMethod> Public Sub C_timestamp_extends()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2040), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub D_archival_chain()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1), Ts(LayerKind.DocumentTimestamp, 2029, 2045, 2)))
        Assert.AreEqual(Y(2045), r.Expiry)
    End Sub

    <TestMethod> Public Sub E_late_archive_gap()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2028, 2050, 1)))
        Assert.AreEqual(Y(2026), r.Expiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.Status)
        Assert.AreEqual(Y(2028), r.PerSignature(0).GapBefore)
    End Sub

    <TestMethod> Public Sub F_expired_but_covered()
        Dim r = Run(Y(2030), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2027, 1), Ts(LayerKind.DocumentTimestamp, 2026, 2050, 2)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub G_expired_uncovered_gap()
        Dim r = Run(Y(2030), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2027, 1), Ts(LayerKind.DocumentTimestamp, 2028, 2050, 2)))
        Assert.AreEqual(Y(2027), r.Expiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.Status)
    End Sub

    <TestMethod> Public Sub H_revoked_signer_poe_before_revocation()
        Dim r = Run(Y(2030), Sig("S", 2050, 0, Revoked(2025), False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2040), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub I_key_compromise_is_invalid_from_creation()
        Dim r = Run(Y(2030), Sig("S", 2050, 0, Revoked(2026, "keyCompromise", 2023), False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2023), r.Expiry)
        Assert.AreEqual(ValidityStatus.Invalid, r.Status)
        Assert.AreNotEqual(DateTimeOffset.MinValue, r.Expiry)
    End Sub

    <TestMethod> Public Sub J_two_signatures_all_and_any()
        Dim a = Sig("A", 2045, 0)
        Dim b = Sig("B", 2031, 1)
        Assert.AreEqual(Y(2031), EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Aggregation = AggregationPolicy.All, .Signatures = New List(Of SignatureInput) From {a, b}}, Y(2020)).Expiry)
        Assert.AreEqual(Y(2045), EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Aggregation = AggregationPolicy.Any, .Signatures = New List(Of SignatureInput) From {a, b}}, Y(2020)).Expiry)
    End Sub

    <TestMethod> Public Sub K_missing_revocation_data()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2050, 1, missingFrom:=2027)))
        Assert.AreEqual(Y(2027), r.Expiry)
        Assert.AreEqual(ValidityStatus.MissingRevocationData, r.Status)
    End Sub

    <TestMethod> Public Sub Suspect_timeline_when_a_later_timestamp_has_an_earlier_gentime()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1), Ts(LayerKind.DocumentTimestamp, 2024, 2031, 2)))
        Assert.AreEqual(ValidityStatus.SuspectTimeline, r.Status)
    End Sub

    <TestMethod> Public Sub Missing_signer_caps_evidence_while_archival_extends()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, True, Ts(LayerKind.DocumentTimestamp, 2025, 2045, 1)))
        Assert.AreEqual(Y(2045), r.Expiry)
        Assert.AreEqual(Y(2026), r.EvidenceExpiry)
        Assert.AreEqual(ValidityStatus.MissingRevocationData, r.PerSignature(0).Status)
    End Sub

    <TestMethod> Public Sub Missing_mid_chain_timestamp_freezes_evidence()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False,
            Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1),
            Ts(LayerKind.DocumentTimestamp, 2028, 2035, 2, missingData:=True),
            Ts(LayerKind.DocumentTimestamp, 2032, 2050, 3)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(Y(2035), r.EvidenceExpiry)
    End Sub

    <TestMethod> Public Sub Severe_status_survives_the_evidence_divergence()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False,
            Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1),
            Ts(LayerKind.DocumentTimestamp, 2028, 2035, 2, missingData:=True),
            Ts(LayerKind.DocumentTimestamp, 2032, 2050, 3),
            Ts(LayerKind.DocumentTimestamp, 2060, 2070, 4)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(Y(2035), r.EvidenceExpiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.PerSignature(0).Status)
    End Sub

    <TestMethod> Public Sub No_signatures_expire_now()
        Dim r = Run(Y(2020))
        Assert.AreEqual(Y(2020), r.Expiry)
        Assert.AreEqual(ValidityStatus.Expired, r.Status)
    End Sub

    <TestMethod> Public Sub Moment_helpers_render_sentinels_and_utc()
        Assert.AreEqual("+∞", EtsiValidityCalculator.TraceMoment(DateTimeOffset.MaxValue))
        Assert.AreEqual("-∞", EtsiValidityCalculator.OutcomeMoment(DateTimeOffset.MinValue))
        Assert.AreEqual("2026.09.08. 17:18:20 UTC", EtsiValidityCalculator.TraceMoment(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero)))
        StringAssert.EndsWith(EtsiValidityCalculator.OutcomeMoment(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero)), " UTC)")
    End Sub
End Class
```

- [ ] **Step 2: Build to see the tests fail**

Expected: compile errors for the model types.

- [ ] **Step 3: Write ValidityModels and AlgorithmSunsetTable**

`EtsiValidity\ValidityModels.vb`:

```vb
''' <summary>Outcome codes of the end-of-validity calculation; the reported time is always a concrete instant.</summary>
Friend Enum ValidityStatus
    ValidUntil
    Expired
    BrokenChain
    MissingRevocationData
    Invalid
    SuspectTimeline
End Enum

''' <summary>What a covering layer is; the signature itself is implicit in <see cref="SignatureInput"/>.</summary>
Friend Enum LayerKind
    SignatureTimestamp
    DocumentTimestamp
End Enum

''' <summary>Per-document aggregation: All = valid only while every signature is; Any = while at least one is.</summary>
Friend Enum AggregationPolicy
    All
    Any
End Enum

''' <summary>Revocation facts for one certificate; Nothing on a certificate means no revocation cap.</summary>
Friend NotInheritable Class RevocationInfo
    Friend Property Revoked As Boolean
    Friend Property RevocationTime As DateTimeOffset
    ''' <summary>CRL reason name, e.g. "keyCompromise".</summary>
    Friend Property Reason As String
    Friend Property InvalidityDate As DateTimeOffset?

    ''' <summary>Effective revocation instant: the invalidity date for compromise reasons, else the revocation time.</summary>
    Friend Function EffectiveR() As DateTimeOffset
        If InvalidityDate.HasValue AndAlso (Reason = "keyCompromise" OrElse Reason = "cACompromise") Then Return InvalidityDate.Value
        Return RevocationTime
    End Function
End Class

''' <summary>One certificate of a path.</summary>
Friend NotInheritable Class CertInfo
    Friend Property NotBefore As DateTimeOffset
    Friend Property NotAfter As DateTimeOffset
    Friend Property Revocation As RevocationInfo
    Friend Property CommonName As String
End Class

''' <summary>Algorithm identifiers for the sunset lookup.</summary>
Friend NotInheritable Class AlgorithmId
    Friend Property SignatureAlgOid As String
    Friend Property HashAlgOid As String
    Friend Property KeyBits As Integer
End Class

''' <summary>A covering timestamp layer; CertPath(0) is the TSA certificate.</summary>
Friend NotInheritable Class Layer
    Friend Property Kind As LayerKind
    ''' <summary>Log id of the timestamp (S0T0 / T1); the calculator falls back to "L{n}" when Nothing.</summary>
    Friend Property DisplayId As String
    Friend Property DocumentOrderIndex As Integer
    ''' <summary>The validated genTime.</summary>
    Friend Property TrustedTime As DateTimeOffset
    Friend Property CertPath As New List(Of CertInfo)
    Friend Property Algorithm As AlgorithmId
    ''' <summary>Instant from which the DSS lacks revocation data this layer would need, else Nothing.</summary>
    Friend Property MissingRevocationFrom As DateTimeOffset?
    ''' <summary>True when the TSA path lacks a CA certificate / CRL / OCSP; makes the layer's validity end "soft".</summary>
    Friend Property MissingData As Boolean
End Class

''' <summary>A signature and its covering timestamps; SignerPath(0) is the signer certificate.</summary>
Friend NotInheritable Class SignatureInput
    Friend Property Id As String
    Friend Property DisplayId As String
    Friend Property DocumentOrderIndex As Integer
    Friend Property SignerPath As New List(Of CertInfo)
    Friend Property Algorithm As AlgorithmId
    ''' <summary>Untrusted claimed signing time.</summary>
    Friend Property ClaimedSigningTime As DateTimeOffset?
    Friend Property CoveringTimestamps As New List(Of Layer)
    ''' <summary>True when the signer path lacks a CA certificate / CRL / OCSP; caps the evidence expiry at the signer's NotAfter.</summary>
    Friend Property MissingData As Boolean

    Friend ReadOnly Property SignerLeaf As CertInfo
        Get
            Return SignerPath(0)
        End Get
    End Property
End Class

''' <summary>Whole-document input.</summary>
Friend NotInheritable Class DocumentValidationInput
    Friend Property Aggregation As AggregationPolicy = AggregationPolicy.All
    Friend Property Signatures As New List(Of SignatureInput)
    Friend Property Sunset As AlgorithmSunsetTable = AlgorithmSunsetTable.Empty
End Class

''' <summary>Per-signature result; both expiries are concrete instants.</summary>
Friend NotInheritable Class PerSignatureResult
    Friend Property SignatureId As String
    Friend Property DisplayId As String
    Friend Property Expiry As DateTimeOffset
    ''' <summary>Expiry provable from embedded evidence; equals <see cref="Expiry"/> when no entity lacks data.</summary>
    Friend Property EvidenceExpiry As DateTimeOffset
    Friend Property Status As ValidityStatus
    Friend Property GapBefore As DateTimeOffset?
    Friend Property Trace As New List(Of String)
End Class

''' <summary>Whole-document result.</summary>
Friend NotInheritable Class DocumentValidityResult
    Friend Property Expiry As DateTimeOffset
    Friend Property EvidenceExpiry As DateTimeOffset
    Friend Property Status As ValidityStatus
    Friend Property PerSignature As New List(Of PerSignatureResult)
End Class
```

`EtsiValidity\AlgorithmSunsetTable.vb`:

```vb
''' <summary>Algorithm-sunset lookup keyed by OID; ships empty (no cap), the shape allows a policy table later without touching the calculator.</summary>
Friend NotInheritable Class AlgorithmSunsetTable
    Friend Shared ReadOnly Empty As New AlgorithmSunsetTable(New Dictionary(Of String, DateTimeOffset))

    Private ReadOnly _byOid As IDictionary(Of String, DateTimeOffset)

    Friend Sub New(ByOid As IDictionary(Of String, DateTimeOffset))
        _byOid = If(ByOid, New Dictionary(Of String, DateTimeOffset))
    End Sub

    ''' <summary>Earliest sunset among the algorithm's signature and hash OIDs; MaxValue means no cap.</summary>
    Friend Function EarliestSunset(Alg As AlgorithmId) As DateTimeOffset
        If Alg Is Nothing Then Return DateTimeOffset.MaxValue
        Dim earliest As DateTimeOffset = DateTimeOffset.MaxValue
        For Each oid As String In {Alg.SignatureAlgOid, Alg.HashAlgOid}
            Dim d As DateTimeOffset
            If Not String.IsNullOrEmpty(oid) AndAlso _byOid.TryGetValue(oid, d) AndAlso d < earliest Then earliest = d
        Next
        Return earliest
    End Function
End Class
```

- [ ] **Step 4: Write EtsiValidityCalculator**

```vb
Imports System.Linq

''' <summary>Pure ETSI EN 319 102-1 long-term validity-expiry calculator: archival expiry (extends across missing data) and evidence expiry (freezes on it). No SecureBlackbox, no wall clock.</summary>
Friend Module EtsiValidityCalculator
    Private Enum WalkMode
        Archival
        Evidence
    End Enum

    Private Structure SelfValidity
        Friend EndTime As DateTimeOffset
        Friend Missing As Boolean
        Friend Revocation As DateTimeOffset?
    End Structure

    Private Structure WalkResult
        Friend Reach As DateTimeOffset
        Friend Missing As Boolean
        Friend Gap As Boolean
        Friend GapBefore As DateTimeOffset?
    End Structure

    Friend Function Compute(Input As DocumentValidationInput, AtTime As DateTimeOffset) As DocumentValidityResult
        Dim per As List(Of PerSignatureResult) = Input.Signatures.Select(Function(s) Evaluate(s, AtTime, Input.Sunset)).ToList()

        If per.Count = 0 Then
            Return New DocumentValidityResult With {.Expiry = AtTime, .EvidenceExpiry = AtTime, .Status = ValidityStatus.Expired, .PerSignature = per}
        End If

        ' All: the document is valid only while every signature is (min); Any: max.
        Dim driver As PerSignatureResult = If(Input.Aggregation = AggregationPolicy.All,
                                              per.OrderBy(Function(p) p.Expiry).First(),
                                              per.OrderByDescending(Function(p) p.Expiry).First())
        Dim evidenceExpiry As DateTimeOffset = If(Input.Aggregation = AggregationPolicy.All,
                                                  per.Min(Function(p) p.EvidenceExpiry),
                                                  per.Max(Function(p) p.EvidenceExpiry))

        Dim status As ValidityStatus = driver.Status
        If status = ValidityStatus.ValidUntil AndAlso driver.Expiry < AtTime Then status = ValidityStatus.Expired

        Return New DocumentValidityResult With {.Expiry = driver.Expiry, .EvidenceExpiry = evidenceExpiry, .Status = status, .PerSignature = per}
    End Function

    Private Function Evaluate(S As SignatureInput, AtTime As DateTimeOffset, Sunset As AlgorithmSunsetTable) As PerSignatureResult
        Dim trace As New List(Of String)
        Dim layers As List(Of Layer) = S.CoveringTimestamps.OrderBy(Function(l) l.TrustedTime).ToList()

        ' T0 is the first covering timestamp's genTime, else now; the claimed time is never trusted.
        Dim t0 As DateTimeOffset = If(layers.Count > 0, layers(0).TrustedTime, AtTime)
        Dim self As SelfValidity = SelfValidityEnd(S.SignerPath, S.Algorithm, Nothing, S.MissingData, Sunset)
        Dim selfId As String = If(S.DisplayId, S.Id)
        trace.Add($"{selfId}: T0={Fmt(t0)} E0={Fmt(self.EndTime)}{If(self.Missing, " (hiányzó adat)", "")}")

        If t0 < S.SignerLeaf.NotBefore Then Return Invalid(S, S.SignerLeaf.NotBefore, "a token megelőzi saját tanúsítványát", trace)
        If self.Revocation.HasValue AndAlso t0 >= self.Revocation.Value Then
            Return Invalid(S, self.Revocation.Value, $"visszavonás ({Fmt(self.Revocation.Value)}) a létezési bizonyíték előtt", trace)
        End If

        ' A later-in-document-order timestamp with an earlier genTime is a suspect timeline.
        Dim suspect As Boolean = False
        For i As Integer = 1 To layers.Count - 1
            If layers(i).DocumentOrderIndex < layers(i - 1).DocumentOrderIndex Then suspect = True
        Next

        Dim arch As WalkResult = Walk(selfId, layers, self.EndTime, self.Missing, Sunset, WalkMode.Archival, trace)
        Dim evid As WalkResult = Walk(selfId, layers, self.EndTime, self.Missing, Sunset, WalkMode.Evidence, trace)

        Dim status As ValidityStatus
        If arch.Gap Then
            status = ValidityStatus.BrokenChain
        ElseIf arch.Missing Then
            status = ValidityStatus.MissingRevocationData
        ElseIf suspect Then
            status = ValidityStatus.SuspectTimeline
        Else
            status = If(arch.Reach >= AtTime, ValidityStatus.ValidUntil, ValidityStatus.Expired)
        End If

        ' Missing embedded evidence is surfaced only when the archival outcome is otherwise clean.
        If evid.Reach < arch.Reach AndAlso (status = ValidityStatus.ValidUntil OrElse status = ValidityStatus.Expired) Then
            status = ValidityStatus.MissingRevocationData
        End If

        trace.Add($"Eredmény: {status}, archív-lejárat={Fmt(arch.Reach)}, bizonyítható-lejárat={Fmt(evid.Reach)}")
        Return New PerSignatureResult With {
            .SignatureId = S.Id, .DisplayId = S.DisplayId, .Expiry = arch.Reach, .EvidenceExpiry = evid.Reach,
            .Status = status, .GapBefore = arch.GapBefore, .Trace = trace}
    End Function

    ''' <summary>One chain walk; in Evidence mode the walk freezes as soon as the reach is derived from missing data.</summary>
    Private Function Walk(SelfId As String, Layers As List(Of Layer), E0 As DateTimeOffset, E0Missing As Boolean,
                          Sunset As AlgorithmSunsetTable, Mode As WalkMode, Trace As List(Of String)) As WalkResult
        Dim allowCrossMissing As Boolean = Mode = WalkMode.Archival
        Dim tag As String = If(allowCrossMissing, "arch", "evid")
        Dim result As New WalkResult With {.Reach = E0, .Missing = E0Missing}

        If Not allowCrossMissing AndAlso result.Missing Then
            Trace.Add($"  [{tag}] {SelfId} hiányzó adat → befagyasztva {Fmt(result.Reach)}")
            Return result
        End If

        For i As Integer = 0 To Layers.Count - 1
            Dim l As Layer = Layers(i)
            Dim ei As SelfValidity = SelfValidityEnd(l.CertPath, l.Algorithm, l.MissingRevocationFrom, l.MissingData, Sunset)
            Dim layerId As String = If(l.DisplayId, $"L{i + 1}")
            ' A layer that predates its own certificate, or was revoked before its genTime, cannot contribute.
            Dim rejected As Boolean = l.TrustedTime < l.CertPath(0).NotBefore OrElse (ei.Revocation.HasValue AndAlso l.TrustedTime >= ei.Revocation.Value)

            If l.TrustedTime <= result.Reach Then
                If Not rejected AndAlso ei.EndTime > result.Reach Then
                    result.Reach = ei.EndTime
                    result.Missing = ei.Missing
                End If
                Trace.Add($"  [{tag}] {layerId}: T={Fmt(l.TrustedTime)} E={Fmt(ei.EndTime)} → reach={Fmt(result.Reach)}{If(result.Missing, " (hiányzó adat)", "")}")
                If Not allowCrossMissing AndAlso result.Missing Then
                    Trace.Add($"  [{tag}] befagyasztva {Fmt(result.Reach)} (hiányzó adat – időbélyeg nem terjeszti tovább)")
                    Exit For
                End If
            Else
                result.Gap = True
                result.GapBefore = l.TrustedTime
                Trace.Add($"  [{tag}] {layerId}: T={Fmt(l.TrustedTime)} > reach {Fmt(result.Reach)} → lánc megszakadt")
                Exit For
            End If
        Next
        Return result
    End Function

    ''' <summary>E_i = min(path NotAfter, revocation cap, missing-data cap, algorithm sunset); missing data keeps the end "soft".</summary>
    Private Function SelfValidityEnd(Path As List(Of CertInfo), Alg As AlgorithmId, MissingRevocationFrom As DateTimeOffset?,
                                     MissingData As Boolean, Sunset As AlgorithmSunsetTable) As SelfValidity
        Dim result As New SelfValidity With {.EndTime = DateTimeOffset.MaxValue}
        For Each c As CertInfo In Path
            If c.NotAfter < result.EndTime Then result.EndTime = c.NotAfter
        Next

        For Each c As CertInfo In Path
            If c.Revocation IsNot Nothing AndAlso c.Revocation.Revoked Then
                Dim ri As DateTimeOffset = c.Revocation.EffectiveR()
                If Not result.Revocation.HasValue OrElse ri < result.Revocation.Value Then result.Revocation = ri
                If ri < result.EndTime Then result.EndTime = ri
            End If
        Next

        If MissingRevocationFrom.HasValue AndAlso MissingRevocationFrom.Value < result.EndTime Then
            result.EndTime = MissingRevocationFrom.Value
            result.Missing = True
        End If

        Dim sunsetCap As DateTimeOffset = Sunset.EarliestSunset(Alg)
        If sunsetCap < result.EndTime Then
            result.EndTime = sunsetCap
            result.Missing = False
        End If

        ' A missing CA certificate / CRL / OCSP dominates: applied last so a public sunset cap cannot clear it.
        If MissingData Then result.Missing = True
        Return result
    End Function

    Private Function Invalid(S As SignatureInput, Boundary As DateTimeOffset, Reason As String, Trace As List(Of String)) As PerSignatureResult
        Trace.Add($"Érvénytelen: {Reason}; lejárat={Fmt(Boundary)}")
        Return New PerSignatureResult With {
            .SignatureId = S.Id, .DisplayId = S.DisplayId, .Expiry = Boundary, .EvidenceExpiry = Boundary,
            .Status = ValidityStatus.Invalid, .Trace = Trace}
    End Function

    ''' <summary>Compact UTC form for the dense trace lines; ±∞ for the two extreme values.</summary>
    Friend Function TraceMoment(T As DateTimeOffset) As String
        Return If(Sentinel(T), MomentFormat.Display(T, Utc:=True))
    End Function

    ''' <summary>Dual local (UTC) form for operator-facing outcome lines; ±∞ for the two extreme values.</summary>
    Friend Function OutcomeMoment(T As DateTimeOffset) As String
        Return If(Sentinel(T), MomentFormat.Log(T))
    End Function

    Private Function Sentinel(T As DateTimeOffset) As String
        If T = DateTimeOffset.MaxValue Then Return "+∞"
        If T = DateTimeOffset.MinValue Then Return "-∞"
        Return Nothing
    End Function

    Private Function Fmt(T As DateTimeOffset) As String
        Return TraceMoment(T)
    End Function
End Module
```

- [ ] **Step 5: Build and run the tests**

Expected: all tests pass. If `If(Sentinel(T), ...)` complains, both operands are `String`; keep the two-argument `If` (null-coalescing) form.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Port the ETSI end-of-validity calculator"
```

---

### Task 11: ETSI validity input builder (SecureBlackbox adapter)

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\EtsiValidity\EtsiValidityInputBuilder.vb`

**Interfaces:**
- Consumes: `EntityNaming`, `ChainContextTracker`, `SbbErrorTranslator`, `SbbLicense`, `MomentFormat`, the Task 10 models.
- Produces: `EtsiValidityInputBuilder.From(Verifier As PDFVerifier, AtTime As DateTimeOffset, Processing As Action(Of String), Trace As Action(Of String)) As DocumentValidationInput`. `Verifier` must have run `Verify()` with `AutoValidateSignatures = False` and `OfflineMode = True`; `Processing` receives concise Hungarian per-entity lines (never Nothing), `Trace` may be Nothing.

This unit reads SecureBlackbox objects, so it is verified by the build and by the manual matrix (Task 16). Keep it free of any logic that could live in the calculator.

- [ ] **Step 1: Write the adapter**

```vb
Imports System.Globalization
Imports nsoftware.SecureBlackbox

''' <summary>Maps a parsed PDFVerifier onto the pure validity model: entities, covering layers, times, and per-entity MissingData from an offline re-validation at each proof-of-existence moment.</summary>
Friend Module EtsiValidityInputBuilder
    ''' <summary>ChainValidationDetails bits meaning "material is missing": cvrUnknownCA, cvrCRLNotVerified, cvrOCSPNotVerified.</summary>
    Private Const MissingDataMask As Integer = &H20 Or &H80 Or &H100
    ''' <summary>Seconds an open-ended OCSP response (no nextUpdate) stays acceptable after thisUpdate; Adobe-parity window.</summary>
    Private Const OpenEndedRevocationGraceSeconds As Integer = 300
    Private Const SbbMomentFormat As String = "yyyy-MM-dd HH:mm:ss"

    Friend Function From(Verifier As PDFVerifier, AtTime As DateTimeOffset, Processing As Action(Of String), Trace As Action(Of String)) As DocumentValidationInput
        Dim poe As New PoeValidator(Verifier, Processing, Trace)
        Dim naming As EntityNaming = EntityNaming.FromVerifier(Verifier)

        Dim docTimestamps As New List(Of KeyValuePair(Of Integer, PDFSignature))
        Dim ordinary As New List(Of KeyValuePair(Of Integer, PDFSignature))
        For i As Integer = 0 To Verifier.Signatures.Count - 1
            Dim s As PDFSignature = Verifier.Signatures(i)
            If s.SignatureType = PDFSignatureTypes.pstDocumentTimestamp Then
                docTimestamps.Add(New KeyValuePair(Of Integer, PDFSignature)(i, s))
            Else
                ordinary.Add(New KeyValuePair(Of Integer, PDFSignature)(i, s))
            End If
        Next

        Dim signatures As New List(Of SignatureInput)
        For Each entry As KeyValuePair(Of Integer, PDFSignature) In ordinary
            Dim order As Integer = entry.Key
            Dim sig As PDFSignature = entry.Value
            Dim sigId As String = EntityNaming.EntityKey(sig.EntityLabel, order, False)
            Dim covering As New List(Of Layer)

            ' The signature's own embedded timestamps (ParentEntity = EntityLabel), numbered S0T0, S0T1, …
            Dim ownTimestamps As Integer = 0
            For j As Integer = 0 To Verifier.Timestamps.Count - 1
                Dim ts As TimestampInfo = Verifier.Timestamps(j)
                If Not String.Equals(ts.ParentEntity, sig.EntityLabel, StringComparison.Ordinal) Then Continue For
                Dim tsId As String = naming.DisplayTimestamp(sig.EntityLabel, ownTimestamps)
                ownTimestamps += 1
                Dim tsTime As DateTimeOffset = Parse(ts.Time, AtTime)
                covering.Add(New Layer With {
                    .Kind = LayerKind.SignatureTimestamp, .DisplayId = tsId, .DocumentOrderIndex = order, .TrustedTime = tsTime,
                    .CertPath = New List(Of CertInfo) From {CertByIndex(Verifier, ts.CertificateIndex, AtTime)},
                    .MissingData = poe.IsMissing(CertByIndexRaw(Verifier, ts.CertificateIndex), tsTime, AtTime, tsId)})
            Next

            ' Every document timestamp later in document order covers this signature.
            For Each dtEntry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
                If dtEntry.Key <= order Then Continue For
                Dim dtTime As DateTimeOffset = SigTime(dtEntry.Value, AtTime)
                Dim dtDisplayId As String = naming.Display(EntityNaming.EntityKey(dtEntry.Value.EntityLabel, dtEntry.Key, True))
                covering.Add(New Layer With {
                    .Kind = LayerKind.DocumentTimestamp, .DisplayId = dtDisplayId, .DocumentOrderIndex = dtEntry.Key, .TrustedTime = dtTime,
                    .CertPath = New List(Of CertInfo) From {SignerCert(Verifier, dtEntry.Value, AtTime)},
                    .MissingData = poe.IsMissing(FindSignerCert(Verifier, dtEntry.Value), dtTime, AtTime, dtDisplayId)})
            Next

            ' An approval signature is validated at its earliest covering genTime (its own claimed time is untrusted), else now.
            Dim sigMoment As DateTimeOffset = SignaturePoeMoment(Verifier, sig, order, docTimestamps, AtTime)
            Dim sigDisplayId As String = naming.Display(sigId)
            signatures.Add(New SignatureInput With {
                .Id = sigId, .DisplayId = sigDisplayId, .DocumentOrderIndex = order,
                .SignerPath = New List(Of CertInfo) From {SignerCert(Verifier, sig, AtTime)},
                .ClaimedSigningTime = TryParseMoment(sig.ClaimedSigningTime),
                .CoveringTimestamps = covering,
                .MissingData = poe.IsMissing(FindSignerCert(Verifier, sig), sigMoment, AtTime, sigDisplayId)})
        Next

        ' A document timestamp is a validity object of its own, covered by the later document timestamps.
        For Each entry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
            Dim order As Integer = entry.Key
            Dim dt As PDFSignature = entry.Value
            Dim covering As New List(Of Layer)
            For Each laterEntry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
                If laterEntry.Key <= order Then Continue For
                Dim laterTime As DateTimeOffset = SigTime(laterEntry.Value, AtTime)
                Dim laterDisplayId As String = naming.Display(EntityNaming.EntityKey(laterEntry.Value.EntityLabel, laterEntry.Key, True))
                covering.Add(New Layer With {
                    .Kind = LayerKind.DocumentTimestamp, .DisplayId = laterDisplayId, .DocumentOrderIndex = laterEntry.Key, .TrustedTime = laterTime,
                    .CertPath = New List(Of CertInfo) From {SignerCert(Verifier, laterEntry.Value, AtTime)},
                    .MissingData = poe.IsMissing(FindSignerCert(Verifier, laterEntry.Value), laterTime, AtTime, laterDisplayId)})
            Next

            Dim dtOwn As DateTimeOffset = SigTime(dt, AtTime)
            Dim dtId As String = EntityNaming.EntityKey(dt.EntityLabel, order, True)
            Dim dtOwnDisplayId As String = naming.Display(dtId)
            signatures.Add(New SignatureInput With {
                .Id = dtId, .DisplayId = dtOwnDisplayId, .DocumentOrderIndex = order,
                .SignerPath = New List(Of CertInfo) From {SignerCert(Verifier, dt, AtTime)},
                .ClaimedSigningTime = TryParseMoment(dt.ClaimedSigningTime),
                .CoveringTimestamps = covering,
                .MissingData = poe.IsMissing(FindSignerCert(Verifier, dt), dtOwn, AtTime, dtOwnDisplayId)})
        Next

        Return New DocumentValidationInput With {.Aggregation = AggregationPolicy.All, .Signatures = signatures, .Sunset = AlgorithmSunsetTable.Empty}
    End Function

    ''' <summary>Earliest genTime among the timestamps covering an approval signature, or AtTime when none covers it.</summary>
    Private Function SignaturePoeMoment(Verifier As PDFVerifier, Sig As PDFSignature, Order As Integer,
                                        DocTimestamps As List(Of KeyValuePair(Of Integer, PDFSignature)), AtTime As DateTimeOffset) As DateTimeOffset
        Dim min As DateTimeOffset = DateTimeOffset.MaxValue
        Dim covered As Boolean = False
        For j As Integer = 0 To Verifier.Timestamps.Count - 1
            Dim ts As TimestampInfo = Verifier.Timestamps(j)
            If Not String.Equals(ts.ParentEntity, Sig.EntityLabel, StringComparison.Ordinal) Then Continue For
            Dim t As DateTimeOffset = Parse(ts.Time, AtTime)
            If t < min Then min = t
            covered = True
        Next
        For Each dtEntry As KeyValuePair(Of Integer, PDFSignature) In DocTimestamps
            If dtEntry.Key <= Order Then Continue For
            Dim t As DateTimeOffset = SigTime(dtEntry.Value, AtTime)
            If t < min Then min = t
            covered = True
        Next
        Return If(covered, min, AtTime)
    End Function

    Private Function SignerCert(Verifier As PDFVerifier, Sig As PDFSignature, AtTime As DateTimeOffset) As CertInfo
        Dim cert As Certificate = FindSignerCert(Verifier, Sig)
        ' Not embedded: NotAfter becomes AtTime so the result stays a concrete instant.
        If cert Is Nothing Then Return New CertInfo With {.NotBefore = DateTimeOffset.MinValue, .NotAfter = AtTime, .CommonName = "ismeretlen"}
        Return ToCertInfo(cert)
    End Function

    Private Function CertByIndex(Verifier As PDFVerifier, Index As Integer, AtTime As DateTimeOffset) As CertInfo
        Dim cert As Certificate = CertByIndexRaw(Verifier, Index)
        If cert Is Nothing Then Return New CertInfo With {.NotBefore = DateTimeOffset.MinValue, .NotAfter = AtTime, .CommonName = "ismeretlen"}
        Return ToCertInfo(cert)
    End Function

    ''' <summary>The embedded certificate that signed the entity (matched by issuer and serial), or Nothing.</summary>
    Friend Function FindSignerCert(Verifier As PDFVerifier, Sig As PDFSignature) As Certificate
        If String.IsNullOrEmpty(Sig.IssuerRDN) OrElse Sig.SerialNumber Is Nothing Then Return Nothing
        For c As Integer = 0 To Verifier.Certificates.Count - 1
            Dim cert As Certificate = Verifier.Certificates(c)
            If String.Equals(cert.IssuerRDN, Sig.IssuerRDN, StringComparison.OrdinalIgnoreCase) AndAlso BytesEqual(cert.SerialNumber, Sig.SerialNumber) Then Return cert
        Next
        Return Nothing
    End Function

    Private Function CertByIndexRaw(Verifier As PDFVerifier, Index As Integer) As Certificate
        If Index < 0 OrElse Index >= Verifier.Certificates.Count Then Return Nothing
        Return Verifier.Certificates(Index)
    End Function

    Private Function ToCertInfo(Cert As Certificate) As CertInfo
        Return New CertInfo With {.NotBefore = ToMoment(Cert.ValidFrom), .NotAfter = ToMoment(Cert.ValidTo), .CommonName = Cert.SubjectRDN}
    End Function

    Private Function SigTime(Sig As PDFSignature, AtTime As DateTimeOffset) As DateTimeOffset
        Dim s As String = If(String.IsNullOrEmpty(Sig.ValidatedSigningTime), Sig.ClaimedSigningTime, Sig.ValidatedSigningTime)
        Return Parse(s, AtTime)
    End Function

    Private Function Parse(Value As String, Fallback As DateTimeOffset) As DateTimeOffset
        Dim parsed As DateTimeOffset? = TryParseMoment(Value)
        Return If(parsed.HasValue, parsed.Value, Fallback)
    End Function

    Private Function ToMoment(Value As String) As DateTimeOffset
        Dim parsed As DateTimeOffset? = TryParseMoment(Value)
        Return If(parsed.HasValue, parsed.Value, DateTimeOffset.MaxValue)
    End Function

    ''' <summary>SecureBlackbox times are UTC strings; unparsable or empty text gives Nothing.</summary>
    Private Function TryParseMoment(Value As String) As DateTimeOffset?
        If String.IsNullOrEmpty(Value) Then Return Nothing
        Dim dt As Date
        If Date.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, dt) Then
            Return New DateTimeOffset(dt, TimeSpan.Zero)
        End If
        Return Nothing
    End Function

    Private Function BytesEqual(A As Byte(), B As Byte()) As Boolean
        If A Is Nothing OrElse B Is Nothing OrElse A.Length <> B.Length Then Return False
        For i As Integer = 0 To A.Length - 1
            If A(i) <> B(i) Then Return False
        Next
        Return True
    End Function

    ''' <summary>Re-validates a leaf chain offline over the document's embedded material at a given moment; memoised per (leaf, moment).</summary>
    Private NotInheritable Class PoeValidator
        Private ReadOnly _knownCerts As New CertificateList
        Private ReadOnly _trustedAnchors As New CertificateList
        Private ReadOnly _ocspBytes As New List(Of Byte())
        Private ReadOnly _crlBytes As New List(Of Byte())
        Private ReadOnly _cache As New Dictionary(Of String, Boolean)
        Private ReadOnly _processing As Action(Of String)
        Private ReadOnly _trace As Action(Of String)

        Friend Sub New(Verifier As PDFVerifier, Processing As Action(Of String), Trace As Action(Of String))
            _processing = Processing
            _trace = Trace
            ' Standalone copies: the verifier-owned objects are only valid while the verifier lives.
            For i As Integer = 0 To Verifier.Certificates.Count - 1
                Dim b As Byte() = Verifier.Certificates(i).Bytes
                If b Is Nothing OrElse b.Length = 0 Then Continue For
                Dim cert As New Certificate(b, 0, b.Length)
                _knownCerts.Add(cert)
                ' Self-signed anchors are trusted explicitly so their own revocation is not looked up offline.
                If cert.SelfSigned Then _trustedAnchors.Add(cert)
            Next
            For i As Integer = 0 To Verifier.OCSPs.Count - 1
                Dim b As Byte() = Verifier.OCSPs(i).Bytes
                If b IsNot Nothing AndAlso b.Length > 0 Then _ocspBytes.Add(b)
            Next
            For i As Integer = 0 To Verifier.CRLs.Count - 1
                Dim b As Byte() = Verifier.CRLs(i).Bytes
                If b IsNot Nothing AndAlso b.Length > 0 Then _crlBytes.Add(b)
            Next
        End Sub

        ''' <summary>True when the leaf's chain lacks a CA certificate / CRL / OCSP the DSS would need at Moment; a Nothing leaf counts as missing.</summary>
        Friend Function IsMissing(Leaf As Certificate, Moment As DateTimeOffset, AtTime As DateTimeOffset, Label As String) As Boolean
            If Leaf Is Nothing Then
                _processing($"  {Label} tanúsítványa nincs beágyazva – hitelesség nem bizonyítható")
                Return True
            End If

            Dim key As String = If(Leaf.SerialNumber Is Nothing, String.Empty, BitConverter.ToString(Leaf.SerialNumber)) & "|" & Leaf.IssuerRDN & "|" & Moment.UtcTicks
            Dim cached As Boolean
            If _cache.TryGetValue(key, cached) Then Return cached

            Dim result As Boolean
            Dim validationLog As String = Nothing
            Dim context As New ChainContextTracker
            Using validator As CertificateValidator = SbbLicense.CreateCertificateValidator()
                Try
                    validator.Certificate = Leaf
                    validator.KnownCertificates = _knownCerts
                    validator.TrustedCertificates = _trustedAnchors
                    For Each o As Byte() In _ocspBytes
                        validator.KnownOCSPs.Add(New OCSPResponse(o, 0, o.Length))
                    Next
                    For Each c As Byte() In _crlBytes
                        validator.KnownCRLs.Add(New CRL(c, 0, c.Length))
                    Next
                    validator.RevocationCheck = CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL
                    validator.OfflineMode = True
                    validator.Config("ImplicitlyTrustSelfSignedCertificates=true")
                    validator.Config("ForceCompleteChainValidationForTrusted=false")
                    validator.Config("RevocationMomentGracePeriod=" & OpenEndedRevocationGraceSeconds)
                    If Moment < AtTime Then validator.ValidationMoment = Moment.UtcDateTime.ToString(SbbMomentFormat, CultureInfo.InvariantCulture)

                    AddHandler validator.OnBeforeCertificateValidation, Sub(s, e) context.Update(e.Cert, e.CACert)
                    AddHandler validator.OnError, Sub(s, e) _processing("  " & SbbErrorTranslator.Describe(e.ErrorCode, e.Description, context.Current, Label))

                    validator.Validate()
                    result = (validator.ChainValidationDetails And MissingDataMask) <> 0
                    Try
                        validationLog = validator.ValidationLog
                    Catch
                        validationLog = Nothing
                    End Try
                Catch ex As Exception
                    ' An unvalidatable chain counts as missing evidence, the conservative outcome.
                    _processing($"  {Label}: hiba a létezési-bizonyíték ellenőrzése közben: {SbbErrorTranslator.ExceptionText(ex)}")
                    _cache(key) = True
                    Return True
                End Try
            End Using

            Dim momentText As String = MomentFormat.Log(Moment)
            _processing($"  {Label} létezési-bizonyíték időpontjában ({momentText}) ellenőrizve: " & If(result, "hiányzó visszavonási/lánc-adat", "a lánc bizonyítható"))
            If _trace IsNot Nothing Then _trace($"{Label} tanúsítványlánc-ellenőrzési napló (létezési bizonyíték: {momentText}):{Environment.NewLine}{validationLog}")

            _cache(key) = result
            Return result
        End Function
    End Class
End Module
```

- [ ] **Step 2: Build**

Run the build. Expected `0 Error(s)`. Likely compile issues and their fixes: an event handler lambda parameter type mismatch means the event args property is named differently in this SBB build — open the nsoftware `PDFVerifier`/`CertificateValidator` members with the Object Browser equivalent (`ildasm` or `[Reflection.Assembly]::LoadFrom` in PowerShell listing the `*EventArgs` type's properties) and adapt only the property name, not the design.

- [ ] **Step 3: Run the tests**

Expected: all existing tests still pass (this task adds none).

- [ ] **Step 4: Commit**

```
git add -A
git commit -m "Add the ETSI validity input builder over PDFVerifier"
```

---

### Task 12: PDFSigner event logger

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbEventLogger.vb`
- Create: `PDF Signer for Tungsten Capture Tests\SbbEventLoggerTests.vb`

**Interfaces:**
- Consumes: `SbbTranslator`, `SbbErrorTranslator`, `ChainContextTracker`, `EntityNaming`, `MomentFormat`.
- Produces: `SbbEventLogger` with `New(Log As StringBuilder, Logger As NLog.Logger)`, `Naming As EntityNaming` (settable, default `EntityNaming.Empty`), `Attach(PDFSigner)`, `Detach(PDFSigner)`, `Shared TimestampValidatedLine(EntityId, IssuerRDN, Time, ValidationResult As Integer, ChainValidationResult As Integer) As String`.

- [ ] **Step 1: Write the failing test for the pure line builder**

```vb
Imports PDFSigner

<TestClass>
Public Class SbbEventLoggerTests
    <TestMethod>
    Public Sub Timestamp_line_rerenders_the_sbb_time_and_translates_both_validities()
        Dim line As String = SbbEventLogger.TimestampValidatedLine("S0T0", "/C=HU/CN=TSA CA", "2026-09-08 17:18:20", 0, 0)
        StringAssert.StartsWith(line, "S0T0 (kiadó: 'TSA CA') ellenőrizve ")
        StringAssert.Contains(line, "(2026.09.08. 17:18:20 UTC)")
        StringAssert.EndsWith(line, "érvényesség: az aláírás érvényes, lánc érvényesség: OK")
    End Sub

    <TestMethod>
    Public Sub Unparsable_time_is_echoed_in_quotes()
        StringAssert.Contains(SbbEventLogger.TimestampValidatedLine("T0", "/CN=X", "soon", 0, 0), "ellenőrizve 'soon' időpontban")
    End Sub
End Class
```

- [ ] **Step 2: Build to see the test fail**

Expected: `'SbbEventLogger' is not declared`.

- [ ] **Step 3: Write the logger**

```vb
Imports System.Globalization
Imports System.Text
Imports NLog
Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Writes PDFSigner validation events into the Hungarian signature log; chain events during a TLS handshake describe the TLS server certificate and go to trace only.</summary>
Friend NotInheritable Class SbbEventLogger
    Private ReadOnly _log As StringBuilder
    Private ReadOnly _logger As Logger
    ''' <summary>Certificate currently processed, so OnError (which names none) can be attributed.</summary>
    Private ReadOnly _context As New ChainContextTracker
    Private _contextBeforeTls As ChainContext
    ''' <summary>TLS handshakes in progress; while positive, chain events belong to the TLS server certificate.</summary>
    Private _tlsHandshakeDepth As Integer

    ''' <summary>Display ids (S0 / T0 / S0T0) for known entity labels; unmapped labels pass through.</summary>
    Friend Property Naming As EntityNaming = EntityNaming.Empty

    Friend Sub New(Log As StringBuilder, Logger As Logger)
        _log = Log
        _logger = Logger
    End Sub

    Friend Sub Attach(S As SbbPdfSigner)
        _tlsHandshakeDepth = 0
        _context.Reset()
        _contextBeforeTls = Nothing
        AddHandler S.OnError, AddressOf Signer_OnError
        AddHandler S.OnChainElementDownload, AddressOf Signer_OnChainElementDownload
        AddHandler S.OnChainValidated, AddressOf Signer_OnChainValidated
        AddHandler S.OnChainValidationProgress, AddressOf Signer_OnChainValidationProgress
        AddHandler S.OnTimestampRequest, AddressOf Signer_OnTimestampRequest
        AddHandler S.OnTimestampValidated, AddressOf Signer_OnTimestampValidated
        AddHandler S.OnSignatureValidated, AddressOf Signer_OnSignatureValidated
        AddHandler S.OnSignatureFound, AddressOf Signer_OnSignatureFound
        AddHandler S.OnTimestampFound, AddressOf Signer_OnTimestampFound
        AddHandler S.OnChainElementNeeded, AddressOf Signer_OnChainElementNeeded
        AddHandler S.OnEncrypted, AddressOf Signer_OnEncrypted
        AddHandler S.OnTLSHandshake, AddressOf Signer_OnTLSHandshake
        AddHandler S.OnTLSCertValidate, AddressOf Signer_OnTLSCertValidate
        AddHandler S.OnTLSEstablished, AddressOf Signer_OnTLSEstablished
        AddHandler S.OnTLSShutdown, AddressOf Signer_OnTLSShutdown
    End Sub

    Friend Sub Detach(S As SbbPdfSigner)
        RemoveHandler S.OnError, AddressOf Signer_OnError
        RemoveHandler S.OnChainElementDownload, AddressOf Signer_OnChainElementDownload
        RemoveHandler S.OnChainValidated, AddressOf Signer_OnChainValidated
        RemoveHandler S.OnChainValidationProgress, AddressOf Signer_OnChainValidationProgress
        RemoveHandler S.OnTimestampRequest, AddressOf Signer_OnTimestampRequest
        RemoveHandler S.OnTimestampValidated, AddressOf Signer_OnTimestampValidated
        RemoveHandler S.OnSignatureValidated, AddressOf Signer_OnSignatureValidated
        RemoveHandler S.OnSignatureFound, AddressOf Signer_OnSignatureFound
        RemoveHandler S.OnTimestampFound, AddressOf Signer_OnTimestampFound
        RemoveHandler S.OnChainElementNeeded, AddressOf Signer_OnChainElementNeeded
        RemoveHandler S.OnEncrypted, AddressOf Signer_OnEncrypted
        RemoveHandler S.OnTLSHandshake, AddressOf Signer_OnTLSHandshake
        RemoveHandler S.OnTLSCertValidate, AddressOf Signer_OnTLSCertValidate
        RemoveHandler S.OnTLSEstablished, AddressOf Signer_OnTLSEstablished
        RemoveHandler S.OnTLSShutdown, AddressOf Signer_OnTLSShutdown
    End Sub

    Private Sub Signer_OnError(sender As Object, e As PDFSignerErrorEventArgs)
        Dim msg As String = SbbErrorTranslator.Describe(e.ErrorCode, e.Description, _context.Current)
        Line(msg)
        _logger.Error("{0}", msg)
    End Sub

    Private Sub Signer_OnChainElementDownload(sender As Object, e As PDFSignerChainElementDownloadEventArgs)
        _context.Update(e.CertRDN, e.CACertRDN)
        Dim cn As String = ChainContextTracker.CommonName(e.CertRDN)
        Dim label As String = If(String.IsNullOrEmpty(cn), String.Empty, $" ('{cn}' tanúsítványhoz)")
        ChainLine($"  {SbbTranslator.ChainKind(e.Kind)} letöltése{label}, URL: {e.Location}")
    End Sub

    Private Sub Signer_OnChainValidated(sender As Object, e As PDFSignerChainValidatedEventArgs)
        Dim who As String = If(String.IsNullOrEmpty(e.SubjectRDN), Naming.Display(e.EntityLabel), ChainContextTracker.CommonName(e.SubjectRDN))
        ChainLine($"'{who}' tanúsítványlánc ellenőrizve, érvényesség: {SbbTranslator.ChainValidity(CType(e.ValidationResult, ChainValidities))}")
    End Sub

    Private Sub Signer_OnChainValidationProgress(sender As Object, e As PDFSignerChainValidationProgressEventArgs)
        _context.Update(e.CertRDN, e.CACertRDN)
        Dim caCn As String = ChainContextTracker.CommonName(e.CACertRDN)
        Dim issuer As String = If(String.IsNullOrEmpty(caCn), String.Empty, $" (kiadó: '{caCn}')")
        ChainLine($"  {SbbTranslator.EventKind(e.EventKind)}, tanúsítvány: '{ChainContextTracker.CommonName(e.CertRDN)}'{issuer}")
    End Sub

    ''' <summary>The TSA value mirrors TimestampServer, which may carry credentials; they are stripped before logging.</summary>
    Private Sub Signer_OnTimestampRequest(sender As Object, e As PDFSignerTimestampRequestEventArgs)
        Line($"Időbélyeg kérése, TSA URL: {SbbErrorTranslator.StripUserInfo(e.TSA)}")
    End Sub

    Private Sub Signer_OnTimestampValidated(sender As Object, e As PDFSignerTimestampValidatedEventArgs)
        Line(TimestampValidatedLine(Naming.Display(e.EntityLabel), e.IssuerRDN, e.Time, e.ValidationResult, e.ChainValidationResult))
    End Sub

    ''' <summary>The timestamp-validated line; SecureBlackbox's UTC genTime is re-rendered in the local (UTC) form, or echoed in quotes when it does not parse.</summary>
    Friend Shared Function TimestampValidatedLine(EntityId As String, IssuerRDN As String, Time As String, ValidationResult As Integer, ChainValidationResult As Integer) As String
        Dim validity As String = SbbTranslator.SignatureValidity(CType(ValidationResult, SignatureValidities))
        Dim chainValidity As String = SbbTranslator.ChainValidity(CType(ChainValidationResult, ChainValidities))
        Dim parsed As Date
        Dim moment As String
        If Date.TryParseExact(Time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, parsed) Then
            moment = MomentFormat.Log(MomentFormat.FromUtc(parsed))
        Else
            moment = $"'{Time}'"
        End If
        Return $"{EntityId} (kiadó: '{ChainContextTracker.CommonName(IssuerRDN)}') ellenőrizve {moment} időpontban, érvényesség: {validity}, lánc érvényesség: {chainValidity}"
    End Function

    Private Sub Signer_OnSignatureValidated(sender As Object, e As PDFSignerSignatureValidatedEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}') ellenőrizve, érvényesség: {SbbTranslator.SignatureValidity(CType(e.ValidationResult, SignatureValidities))}")
    End Sub

    Private Sub Signer_OnSignatureFound(sender As Object, e As PDFSignerSignatureFoundEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} megtalálva (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}'), tanúsítvány: {If(e.CertFound, "megtalálva", "nem található")}")
    End Sub

    Private Sub Signer_OnTimestampFound(sender As Object, e As PDFSignerTimestampFoundEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} megtalálva (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}'), tanúsítvány: {If(e.CertFound, "megtalálva", "nem található")}")
    End Sub

    Private Sub Signer_OnChainElementNeeded(sender As Object, e As PDFSignerChainElementNeededEventArgs)
        Dim caCn As String = ChainContextTracker.CommonName(e.CACertRDN)
        Dim issuer As String = If(String.IsNullOrEmpty(caCn), String.Empty, $" (kiadó: '{caCn}')")
        ChainLine($"  {SbbTranslator.ChainKind(e.Kind)} szükséges, tanúsítvány: '{ChainContextTracker.CommonName(e.CertRDN)}'{issuer}")
    End Sub

    Private Sub Signer_OnEncrypted(sender As Object, e As PDFSignerEncryptedEventArgs)
        Line($"A PDF dokumentum titkosított (tanúsítvány alapú: {e.CertUsed})")
    End Sub

    Private Sub Signer_OnTLSHandshake(sender As Object, e As PDFSignerTLSHandshakeEventArgs)
        If _tlsHandshakeDepth = 0 Then _contextBeforeTls = _context.Current
        _tlsHandshakeDepth += 1
        TraceLine($"  TLS kézfogás indul: {e.Host}")
    End Sub

    Private Sub Signer_OnTLSCertValidate(sender As Object, e As PDFSignerTLSCertValidateEventArgs)
        TraceLine($"  TLS szerver tanúsítvány ellenőrzése: {e.ServerHost} ({e.ServerIP}), elfogadva: {e.Accept}")
    End Sub

    Private Sub Signer_OnTLSEstablished(sender As Object, e As PDFSignerTLSEstablishedEventArgs)
        If _tlsHandshakeDepth > 0 Then _tlsHandshakeDepth -= 1
        If _tlsHandshakeDepth = 0 AndAlso _contextBeforeTls IsNot Nothing Then
            _context.Restore(_contextBeforeTls)
            _contextBeforeTls = Nothing
        End If
        TraceLine($"  TLS kapcsolat létrejött: {e.Host}, verzió: {e.Version}, titkosítás: {e.Ciphersuite}")
    End Sub

    Private Sub Signer_OnTLSShutdown(sender As Object, e As PDFSignerTLSShutdownEventArgs)
        TraceLine($"  TLS kapcsolat lezárva: {e.Host}")
    End Sub

    Private Sub TraceLine(M As String)
        If _logger.IsTraceEnabled Then Line(M)
    End Sub

    ''' <summary>Chain lines during a TLS handshake describe the TLS server certificate and are demoted to trace.</summary>
    Private Sub ChainLine(M As String)
        If _tlsHandshakeDepth > 0 Then
            TraceLine(M)
        Else
            Line(M)
        End If
    End Sub

    Private Sub Line(M As String)
        _log.AppendLine(M)
    End Sub
End Class
```

- [ ] **Step 4: Build and run the tests**

Expected `0 Error(s)` and all tests pass. If a handler's `e` type name does not exist in this build, list the nsoftware event-args types with
`([Reflection.Assembly]::LoadFrom((Resolve-Path "lib\nsoftware.SecureBlackbox.dll"))).GetTypes() | Where-Object { $_.Name -like "PDFSigner*EventArgs" } | Select-Object -ExpandProperty Name`
and use the listed name; property names come from `.GetProperties()` on that type.

- [ ] **Step 5: Commit**

```
git add -A
git commit -m "Add the PDFSigner event logger for the signature log"
```

---

### Task 13: Windows certificate stores and document harvester

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\WindowsCertificateStores.vb`
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\DocumentCertificateHarvester.vb`

**Interfaces:**
- Consumes: `SbbLicense`, `EntityNaming`, `MemoryTributary`.
- Produces: `WindowsCertificateStores` with `Shared Load() As WindowsCertificateStores`, `TrustedRoots As CertificateList` (Root, CurrentUser + LocalMachine), `IntermediateCertificates As CertificateList` (CA, CurrentUser + LocalMachine). `HarvestedMaterial` with `CertificateBytes`, `CrlBytes`, `OcspBytes` (lists of `Byte()`), `LastSignatureEntityLabel As String`, `SignatureTimestampCertificateBytes As Byte()`, `Naming As EntityNaming`, `BuildKnownCertificates(StoreCertificates As CertificateList) As CertificateList`, `ApplyKnownRevocation(Signer As PDFSigner)`, `ApplyKnownRevocation(Validator As CertificateValidator)`, `Shared Empty() As HarvestedMaterial`. `DocumentCertificateHarvester.Probe(Document As MemoryTributary, ByRef Encrypted As Boolean, ByRef EntityCount As Integer)` and `.Harvest(Document As MemoryTributary) As HarvestedMaterial`.

- [ ] **Step 1: Write WindowsCertificateStores**

```vb
Imports System.Security.Cryptography.X509Certificates
Imports NLog
Imports nsoftware.SecureBlackbox

''' <summary>Trust anchors and intermediate certificates of the Windows stores as SecureBlackbox lists, loaded once per signing call.</summary>
Friend NotInheritable Class WindowsCertificateStores
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    ''' <summary>Root store of the current user and the machine.</summary>
    Friend ReadOnly Property TrustedRoots As New CertificateList
    ''' <summary>Intermediate CA store of the current user and the machine.</summary>
    Friend ReadOnly Property IntermediateCertificates As New CertificateList

    Friend Shared Function Load() As WindowsCertificateStores
        Dim res As New WindowsCertificateStores
        For Each location As StoreLocation In {StoreLocation.CurrentUser, StoreLocation.LocalMachine}
            AddStore(StoreName.Root, location, res.TrustedRoots)
            AddStore(StoreName.CertificateAuthority, location, res.IntermediateCertificates)
        Next
        Return res
    End Function

    ''' <summary>Adds every certificate of one store; an unreadable store is logged and skipped so signing can still proceed on the other stores.</summary>
    Private Shared Sub AddStore(Name As StoreName, Location As StoreLocation, Target As CertificateList)
        Try
            Using store As New X509Store(Name, Location)
                store.Open(OpenFlags.ReadOnly Or OpenFlags.OpenExistingOnly)
                For Each cert As X509Certificate2 In store.Certificates
                    Dim raw As Byte() = cert.RawData
                    Target.Add(New Certificate(raw, 0, raw.Length))
                Next
            End Using
        Catch ex As Exception
            _logger.Warn(ex, "A(z) {0}\{1} tanúsítványtár nem olvasható", Location, Name)
        End Try
    End Sub
End Class
```

- [ ] **Step 2: Write the harvester**

```vb
Imports System.IO
Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Certificates, CRLs and OCSP responses the document currently carries, kept as DER bytes so they outlive the verifier that read them.</summary>
Friend NotInheritable Class HarvestedMaterial
    Friend ReadOnly Property CertificateBytes As New List(Of Byte())
    Friend ReadOnly Property CrlBytes As New List(Of Byte())
    Friend ReadOnly Property OcspBytes As New List(Of Byte())
    ''' <summary>EntityLabel of the last signature entity (the target of the next Update pass), or Nothing.</summary>
    Friend Property LastSignatureEntityLabel As String
    ''' <summary>DER of the TSA certificate of the last signature's first timestamp, or Nothing.</summary>
    Friend Property SignatureTimestampCertificateBytes As Byte()
    Friend Property Naming As EntityNaming = EntityNaming.Empty

    ''' <summary>Material of an unsigned document: nothing harvested.</summary>
    Friend Shared Function Empty() As HarvestedMaterial
        Return New HarvestedMaterial
    End Function

    ''' <summary>A new list holding the store certificates followed by the harvested ones; the input list is never mutated.</summary>
    Friend Function BuildKnownCertificates(StoreCertificates As CertificateList) As CertificateList
        Dim combined As New CertificateList
        If StoreCertificates IsNot Nothing Then
            For i As Integer = 0 To StoreCertificates.Count - 1
                combined.Add(StoreCertificates(i))
            Next
        End If
        For Each b As Byte() In CertificateBytes
            combined.Add(New Certificate(b, 0, b.Length))
        Next
        Return combined
    End Function

    ''' <summary>Feeds the harvested CRLs/OCSP responses to a signer so an earlier pass's downloads are not repeated.</summary>
    Friend Sub ApplyKnownRevocation(Signer As SbbPdfSigner)
        For Each crl As Byte() In CrlBytes
            Signer.KnownCRLs.Add(New CRL(crl, 0, crl.Length))
        Next
        For Each ocsp As Byte() In OcspBytes
            Signer.KnownOCSPs.Add(New OCSPResponse(ocsp, 0, ocsp.Length))
        Next
    End Sub

    ''' <summary>Feeds the harvested CRLs/OCSP responses to a validator.</summary>
    Friend Sub ApplyKnownRevocation(Validator As CertificateValidator)
        For Each crl As Byte() In CrlBytes
            Validator.KnownCRLs.Add(New CRL(crl, 0, crl.Length))
        Next
        For Each ocsp As Byte() In OcspBytes
            Validator.KnownOCSPs.Add(New OCSPResponse(ocsp, 0, ocsp.Length))
        Next
    End Sub
End Class

''' <summary>Parse-only reads of the working document: encryption/entity probe and harvest of its embedded validation material.</summary>
Friend Module DocumentCertificateHarvester
    ''' <summary>Opens the document without validation and reports whether it is encrypted and how many signature entities it holds.</summary>
    Friend Sub Probe(Document As MemoryTributary, ByRef Encrypted As Boolean, ByRef EntityCount As Integer)
        Document.Seek(0, SeekOrigin.Begin)
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            verifier.InputStream = Document
            verifier.AutoValidateSignatures = False
            Try
                verifier.Open(False)
                Encrypted = verifier.DocumentInfo IsNot Nothing AndAlso verifier.DocumentInfo.EncryptionType <> PDFEncryptionTypes.petNone
                EntityCount = verifier.Signatures.Count
                verifier.Close(False)
            Finally
                Document.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
    End Sub

    ''' <summary>Parse-only Verify() (no network, no chain build) capturing the DSS and the last entity's label; Verify() closes the document itself.</summary>
    Friend Function Harvest(Document As MemoryTributary) As HarvestedMaterial
        Dim harvest As New HarvestedMaterial
        Document.Seek(0, SeekOrigin.Begin)
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            verifier.InputStream = Document
            verifier.AutoValidateSignatures = False
            Try
                verifier.Verify()

                If verifier.Signatures.Count > 0 Then
                    Dim last As PDFSignature = verifier.Signatures(verifier.Signatures.Count - 1)
                    harvest.LastSignatureEntityLabel = last.EntityLabel
                    For j As Integer = 0 To verifier.Timestamps.Count - 1
                        Dim ts As TimestampInfo = verifier.Timestamps(j)
                        If Not String.Equals(ts.ParentEntity, last.EntityLabel, StringComparison.Ordinal) Then Continue For
                        If ts.CertificateIndex >= 0 AndAlso ts.CertificateIndex < verifier.Certificates.Count Then
                            harvest.SignatureTimestampCertificateBytes = verifier.Certificates(ts.CertificateIndex).Bytes
                        End If
                        Exit For
                    Next
                End If

                harvest.Naming = EntityNaming.FromVerifier(verifier)
                For i As Integer = 0 To verifier.Certificates.Count - 1
                    AddIfNotEmpty(harvest.CertificateBytes, verifier.Certificates(i).Bytes)
                Next
                For i As Integer = 0 To verifier.CRLs.Count - 1
                    AddIfNotEmpty(harvest.CrlBytes, verifier.CRLs(i).Bytes)
                Next
                For i As Integer = 0 To verifier.OCSPs.Count - 1
                    AddIfNotEmpty(harvest.OcspBytes, verifier.OCSPs(i).Bytes)
                Next
            Finally
                Document.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
        Return harvest
    End Function

    Private Sub AddIfNotEmpty(Target As List(Of Byte()), Bytes As Byte())
        If Bytes IsNot Nothing AndAlso Bytes.Length > 0 Then Target.Add(Bytes)
    End Sub
End Module
```

- [ ] **Step 3: Build and run the tests**

Expected `0 Error(s)`; all tests still pass.

- [ ] **Step 4: Commit**

```
git add -A
git commit -m "Add Windows store loading and document material harvesting for the SBB module"
```

---

### Task 14: The orchestrator and the module switch-over

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SbbSignatureCreator.vb`
- Modify: `PDF Signer for Tungsten Capture\Signature\SignatureOperation.vb:97-101, 265-269, 298-302`
- Delete: `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb`, `PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBCodeTranslator.vb`

**Interfaces:**
- Consumes: everything from Tasks 2 and 6–13, `SignatureRequest`/`SignatureResult`/`SigningCertificate` (`Models\SignatureModel.vb`), `MemoryTributary`.
- Produces: `Friend Class SbbSignatureCreator` with `Initialize(ProviderSettings As PDFSignerCryptoProvider)`, `Shared ActivateLicense()`, `GetCertificatesFromStore(QualifiedCertificatesOnly As Boolean) As List(Of SigningCertificate)`, `SignDocument(Request As SignatureRequest) As SignatureResult`.

- [ ] **Step 1: Write the orchestrator**

```vb
Imports System.IO
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports NLog
Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Built-in signing module: runs the planned SecureBlackbox passes over an unsigned PDF and returns the signed stream, its validity end and the log.</summary>
Friend Class SbbSignatureCreator
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Private ReadOnly _log As New StringBuilder
    Private _settings As PDFSignerCryptoProvider
    Private _stores As WindowsCertificateStores
    Private _harvest As HarvestedMaterial = HarvestedMaterial.Empty()
    Private _eventLogger As SbbEventLogger

    Friend Sub Initialize(ProviderSettings As PDFSignerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    ''' <summary>Applies the legacy-API license used by the PDF pre/post processors; the new components carry their own RuntimeLicense.</summary>
    Friend Shared Sub ActivateLicense()
        SBUtils.Unit.SetLicenseKey(SbbLicense.Key)
        SBPDF.Unit.Initialize()
        SBPDFSecurity.Unit.Initialize()
    End Sub

    ''' <summary>Currently valid signing-capable certificates of the user's personal store, optionally qualified ones only.</summary>
    Friend Function GetCertificatesFromStore(QualifiedCertificatesOnly As Boolean) As List(Of SigningCertificate)
        Dim res As New List(Of SigningCertificate)
        Using store As New X509Store(StoreName.My, StoreLocation.CurrentUser)
            store.Open(OpenFlags.ReadOnly Or OpenFlags.OpenExistingOnly)
            For Each cert As X509Certificate2 In store.Certificates
                If cert.NotBefore >= Date.Now OrElse cert.NotAfter <= Date.Now Then Continue For
                If Not SigningCertificatePrecheck.HasSigningKeyUsage(cert) Then Continue For
                Dim qualified As Boolean = QualifiedCertificateDetector.IsQualified(cert)
                If QualifiedCertificatesOnly AndAlso Not qualified Then Continue For
                res.Add(New SigningCertificate With {.Certificate = cert, .IsQualified = qualified})
            Next
        End Using
        Return res
    End Function

    Friend Function SignDocument(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult
        _eventLogger = New SbbEventLogger(_log, _logger)
        Try
            Dim plan As SigningPlan = SigningPlanBuilder.Build(_settings)

            Append("Aláíró tanúsítvány off-line ellenőrzése")
            Dim cert As X509Certificate2 = Request.SigningCertificate?.Certificate
            If cert Is Nothing Then Return Fail(res, "Nincs kiválasztva aláíró tanúsítvány!")
            Dim requireOcsp As Boolean = _settings.EnableRevocationChecking AndAlso _settings.RevocationCheckProtocol = RevocationType.OCSP
            Dim precheckError As String = SigningCertificatePrecheck.Check(cert, requireOcsp)
            If precheckError IsNot Nothing Then Return Fail(res, precheckError)

            _stores = WindowsCertificateStores.Load()
            Dim working As New MemoryTributary
            Request.FileToSign.Seek(0, SeekOrigin.Begin)
            Request.FileToSign.CopyTo(working)

            Dim encrypted As Boolean
            Dim entityCount As Integer
            DocumentCertificateHarvester.Probe(working, encrypted, entityCount)
            If encrypted Then Return Fail(res, "A PDF dokumentum titkosított, titkosított dokumentum nem írható alá!")
            If entityCount > 0 Then Return Fail(res, "A PDF dokumentum már tartalmaz aláírást vagy időbélyeget; a beépített szolgáltató csak aláíratlan dokumentumot ír alá!")

            If plan.CheckSigningCertRevocation Then
                Dim leaf As New Certificate(cert.RawData, 0, cert.RawData.Length)
                If Not RunValidatorCheck(leaf, "aláíró tanúsítvány", res) Then Return res
            End If

            working = RunSignPass(plan, cert, working, res)
            If working Is Nothing Then Return res
            _harvest = DocumentCertificateHarvester.Harvest(working)
            _eventLogger.Naming = _harvest.Naming

            If plan.CheckTimestampCertRevocation Then
                Dim tsaBytes As Byte() = _harvest.SignatureTimestampCertificateBytes
                If tsaBytes Is Nothing Then Return Fail(res, "Az aláírás-időbélyeg tanúsítványa nem található a dokumentumban!")
                If Not RunValidatorCheck(New Certificate(tsaBytes, 0, tsaBytes.Length), "időbélyeg-szolgáltató tanúsítvány", res) Then Return res
            End If

            If plan.UpdateToEmbed Then
                working = RunUpdatePass(plan, working, res)
                If working Is Nothing Then Return res
                _harvest = DocumentCertificateHarvester.Harvest(working)
            End If

            If plan.AddDocumentTimestamp Then
                working = RunDocumentTimestampPass(plan, working, res)
                If working Is Nothing Then Return res
                _harvest = DocumentCertificateHarvester.Harvest(working)
                _eventLogger.Naming = _harvest.Naming
                If plan.EmbedDocumentTimestampRevocation Then
                    working = RunUpdatePass(plan, working, res)
                    If working Is Nothing Then Return res
                    _harvest = DocumentCertificateHarvester.Harvest(working)
                End If
            End If

            Dim validity As Date
            If Not TryComputeValidity(working, res, validity) Then Return res

            working.Seek(0, SeekOrigin.Begin)
            res.SignedFile = working
            res.SignatureExpiration = validity
            res.SignatureLog = _log.ToString()
            Return res
        Catch ex As Exception
            _logger.Error(ex, "Váratlan hiba az aláírás közben")
            res.ExceptionText = ex.ToString()
            Return Fail(res, $"Váratlan hiba az aláírás közben: {SbbErrorTranslator.ExceptionText(ex)}")
        End Try
    End Function

#Region "Passes"

    ''' <summary>Sign pass at the plan's level; B-LT/B-LTA sign lean (crcNone) and are lifted by the Update pass.</summary>
    Private Function RunSignPass(Plan As SigningPlan, SigningCert As X509Certificate2, Source As MemoryTributary, Res As SignatureResult) As MemoryTributary
        _log.AppendLine()
        Append("PDF dokumentum aláírása")
        If Plan.EmbedSignatureTimestamp Then Append($"Időbélyeg szolgáltató URL: {_settings.TSAURL}, felhasználó: '{_settings.TSAUserName}'")
        LogProxy()

        Using certManager As CertificateManager = SbbLicense.CreateCertificateManager()
            certManager.ImportFromObject(SigningCert)
            Dim failure As String = Nothing
            Dim output As MemoryTributary = RunSignerPass(Source,
                Sub(signer As SbbPdfSigner)
                    signer.SigningCertificate = certManager.Certificate
                    signer.NewSignature.Level = Plan.SignatureLevel
                    signer.NewSignature.HashAlgorithm = SbbTranslator.HashAlgorithmName(_settings.SignatureHashMethod)
                    signer.NewSignature.AuthorName = _settings.SigningOrganization
                    signer.NewSignature.Reason = _settings.SigningReason
                    signer.Widget.Invisible = True
                    signer.RevocationCheck = Plan.SignPassRevocationCheck
                    signer.OfflineMode = False
                    signer.IgnoreChainValidationErrors = False
                    If Plan.EmbedSignatureTimestamp Then signer.TimestampServer = TsaUrl()
                    ConfigureProxy(signer.Proxy)
                End Sub,
                Sub(signer As SbbPdfSigner) signer.Sign(),
                failure)
            If output Is Nothing Then
                Fail(Res, $"Hiba a hitelesítési művelet közben: {failure}")
                Return Nothing
            End If
            Append("Hitelesítési művelet befejezve")
            Return output
        End Using
    End Function

    ''' <summary>Update pass over the last entity: checks the chains with the configured protocol and embeds the collected revocation data; no TSA.</summary>
    Private Function RunUpdatePass(Plan As SigningPlan, Source As MemoryTributary, Res As SignatureResult) As MemoryTributary
        _log.AppendLine()
        Dim entityLabel As String = _harvest.LastSignatureEntityLabel
        If String.IsNullOrEmpty(entityLabel) Then
            Fail(Res, "Nem található aláírás a visszavonási adatok beágyazásához!")
            Return Nothing
        End If
        Append($"Visszavonási adatok beágyazása (Update, {_harvest.Naming.Display(entityLabel)})")
        LogProxy()

        Dim failure As String = Nothing
        Dim output As MemoryTributary = RunSignerPass(Source,
            Sub(signer As SbbPdfSigner)
                signer.RevocationCheck = Plan.Revocation
                signer.OfflineMode = False
                signer.IgnoreChainValidationErrors = False
                signer.Config("AutoCollectRevocationInfo=true")
                signer.Config("IncludeRevocationInfoToAdbeAttribute=true")
                signer.Config("CollectRevInfoForTimestamps=true")
                ConfigureProxy(signer.Proxy)
            End Sub,
            Sub(signer As SbbPdfSigner) signer.Update(entityLabel),
            failure)
        If output Is Nothing Then
            Fail(Res, $"Hiba a visszavonási adatok beágyazása közben: {failure}")
            Return Nothing
        End If
        Append("Visszavonási adatok beágyazva")
        Return output
    End Function

    ''' <summary>Lean archive document timestamp: no revocation checking or collection in the creating pass (SecureBlackbox never embeds the new timestamp's own revocation there).</summary>
    Private Function RunDocumentTimestampPass(Plan As SigningPlan, Source As MemoryTributary, Res As SignatureResult) As MemoryTributary
        _log.AppendLine()
        Append("Dokumentumszintű időbélyeg készítése")
        Append($"Időbélyeg szolgáltató URL: {_settings.TSAURL}, felhasználó: '{_settings.TSAUserName}'")

        Dim failure As String = Nothing
        Dim output As MemoryTributary = RunSignerPass(Source,
            Sub(signer As SbbPdfSigner)
                signer.NewSignature.SignatureType = PDFSignatureTypes.pstDocumentTimestamp
                signer.NewSignature.HashAlgorithm = SbbTranslator.HashAlgorithmName(_settings.TimeStampHashMethod)
                signer.Widget.Invisible = True
                signer.TimestampServer = TsaUrl()
                signer.RevocationCheck = PDFSignerRevocationChecks.crcNone
                signer.OfflineMode = False
                ConfigureProxy(signer.Proxy)
            End Sub,
            Sub(signer As SbbPdfSigner) signer.Sign(),
            failure)
        If output Is Nothing Then
            Fail(Res, $"Hiba a dokumentumszintű időbélyeg készítése közben: {failure}")
            Return Nothing
        End If
        Append("Dokumentumszintű időbélyeg elkészítve")
        Return output
    End Function

    ''' <summary>One PDFSigner pass: fresh component and output buffer, stores, harvested material, then configure and execute. Never throws; a failure comes back as text.</summary>
    Private Function RunSignerPass(Source As MemoryTributary, Configure As Action(Of SbbPdfSigner), Execute As Action(Of SbbPdfSigner), ByRef Failure As String) As MemoryTributary
        Dim output As New MemoryTributary
        Using signer As SbbPdfSigner = SbbLicense.CreateSigner()
            Try
                Source.Seek(0, SeekOrigin.Begin)
                signer.InputStream = Source
                signer.OutputStream = output
                signer.TrustedCertificates = _stores.TrustedRoots
                signer.KnownCertificates = _harvest.BuildKnownCertificates(_stores.IntermediateCertificates)
                _harvest.ApplyKnownRevocation(signer)
                Configure(signer)
                _eventLogger.Attach(signer)
                Try
                    Execute(signer)
                Finally
                    _eventLogger.Detach(signer)
                End Try
                output.Seek(0, SeekOrigin.Begin)
                Failure = Nothing
                Return output
            Catch ex As Exception
                Failure = SbbErrorTranslator.ExceptionText(ex)
                output.Dispose()
                Return Nothing
            End Try
        End Using
    End Function

    ''' <summary>Fail-closed online CertificateValidator pass; False (with the error recorded in Res) unless the chain is cvtValid.</summary>
    Private Function RunValidatorCheck(Leaf As Certificate, Label As String, Res As SignatureResult) As Boolean
        _log.AppendLine()
        Append($"Visszavonás-ellenőrzés: {Label}")
        Dim context As New ChainContextTracker
        Using validator As CertificateValidator = SbbLicense.CreateCertificateValidator()
            Try
                validator.Certificate = Leaf
                validator.TrustedCertificates = _stores.TrustedRoots
                validator.KnownCertificates = _harvest.BuildKnownCertificates(_stores.IntermediateCertificates)
                _harvest.ApplyKnownRevocation(validator)
                validator.RevocationCheck = SbbTranslator.RevocationCheckForValidator(CType(_settings.RevocationCheckProtocol, RevocationType))
                validator.OfflineMode = False
                ConfigureProxy(validator.Proxy)

                AddHandler validator.OnBeforeCertificateValidation, Sub(s, e) context.Update(e.Cert, e.CACert)
                AddHandler validator.OnError, Sub(s, e) Append("  " & SbbErrorTranslator.Describe(e.ErrorCode, e.Description, context.Current, Label))
                AddHandler validator.OnBeforeOCSPDownload, Sub(s, e)
                                                              context.Update(e.Cert, e.CACert)
                                                              Append($"  OCSP letöltése, URL: {e.Location} ('{ChainContextTracker.CommonName(e.Cert)}' tanúsítványhoz)")
                                                          End Sub
                AddHandler validator.OnBeforeCRLDownload, Sub(s, e)
                                                             context.Update(e.Cert, e.CACert)
                                                             Append($"  CRL letöltése, URL: {e.Location} ('{ChainContextTracker.CommonName(e.Cert)}' tanúsítványhoz)")
                                                         End Sub
                AddHandler validator.OnAfterCertificateValidation, Sub(s, e) Append($"  tanúsítványlánc elem ellenőrizve, érvényesség: {e.Validity}, részletek: 0x{e.ValidationDetails:X} ('{ChainContextTracker.CommonName(e.Cert)}')")

                validator.Validate()
                If validator.ChainValidationResult = CertificateValidatorChainValidationResults.cvtValid Then
                    Append($"A(z) {Label} tanúsítványlánca érvényes")
                    Return True
                End If
                Fail(Res, $"A(z) {Label} tanúsítványlánca nem érvényes: {SbbTranslator.ValidatorChainValidity(validator.ChainValidationResult)} (részletek: 0x{validator.ChainValidationDetails:X})")
                Return False
            Catch ex As Exception
                Fail(Res, $"Hiba a(z) {Label} visszavonás-ellenőrzése közben: {SbbErrorTranslator.ExceptionText(ex)}")
                Return False
            End Try
        End Using
    End Function

    ''' <summary>Offline parse of the finished document and the ETSI validity calculation; the archival expiry becomes the result's validity end.</summary>
    Private Function TryComputeValidity(Final As MemoryTributary, Res As SignatureResult, ByRef Validity As Date) As Boolean
        _log.AppendLine()
        Append("Hitelesség lejárati idejének meghatározása")
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            Try
                Final.Seek(0, SeekOrigin.Begin)
                verifier.InputStream = Final
                verifier.OfflineMode = True
                verifier.AutoValidateSignatures = False
                verifier.Verify()

                Dim atTime As DateTimeOffset = DateTimeOffset.UtcNow
                Dim traceSink As Action(Of String) = Nothing
                If _logger.IsTraceEnabled Then traceSink = Sub(m As String) _logger.Trace("{0}", m)
                Dim input As DocumentValidationInput = EtsiValidityInputBuilder.From(verifier, atTime, AddressOf Append, traceSink)
                Dim result As DocumentValidityResult = EtsiValidityCalculator.Compute(input, atTime)
                LogValidity(result)
                Validity = result.Expiry.UtcDateTime
                Return True
            Catch ex As Exception
                Fail(Res, $"Hiba a PDF dokumentum hitelességi lejárati idejének meghatározása közben: {SbbErrorTranslator.ExceptionText(ex)}")
                Return False
            Finally
                Final.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
    End Function

    Private Sub LogValidity(Result As DocumentValidityResult)
        Dim whenText As String = EtsiValidityCalculator.OutcomeMoment(Result.Expiry)
        Dim outcome As String
        Select Case Result.Status
            Case ValidityStatus.ValidUntil : outcome = $"Hitelesség lejárati ideje meghatározva: {whenText}"
            Case ValidityStatus.Expired : outcome = $"Hitelesség lejárati ideje múltbeli ({whenText})"
            Case ValidityStatus.BrokenChain : outcome = $"Hitelességi lánc megszakadt; lejárat: {whenText}"
            Case ValidityStatus.MissingRevocationData : outcome = $"Hiányzó visszavonási adat, lejárat: {whenText}"
            Case ValidityStatus.Invalid : outcome = $"Az aláírás létrejöttétől érvénytelen; lejárat: {whenText}"
            Case ValidityStatus.SuspectTimeline : outcome = $"Gyanús időrend; lejárat: {whenText}"
            Case Else : outcome = $"Hitelesség lejárati ideje: {whenText}"
        End Select
        Append(outcome)
        If Result.Status = ValidityStatus.ValidUntil Then
            _logger.Info("{0}", outcome)
        Else
            _logger.Warn("{0}", outcome)
        End If

        Dim evidenceText As String = EtsiValidityCalculator.OutcomeMoment(Result.EvidenceExpiry)
        If Result.EvidenceExpiry < Result.Expiry Then
            Append($"Bizonyítható hitelességi lejárat (hiányzó tanúsítvány/visszavonási adat miatt): {evidenceText}")
        Else
            Append($"Bizonyítható hitelességi lejárat: {evidenceText}")
        End If

        For Each per As PerSignatureResult In Result.PerSignature
            _logger.Debug("Hitelesség [{0}]: {1}, lejárat={2}", If(per.DisplayId, per.SignatureId), per.Status, EtsiValidityCalculator.TraceMoment(per.Expiry))
            For Each line As String In per.Trace
                _logger.Debug("  {0}", line)
            Next
        Next
    End Sub

#End Region

#Region "Helpers"

    Private Function TsaUrl() As String
        Return TsaUrlBuilder.WithCredentials(_settings.TSAURL.ToString(), _settings.TSAUserName, _settings.TSAPassword)
    End Function

    Private Sub ConfigureProxy(Proxy As ProxySettings)
        If Not _settings.IsProxyEnabled Then Return
        Proxy.ProxyType = ProxyTypes.cptHTTP
        Proxy.Address = _settings.ProxyServer
        Proxy.Port = _settings.ProxyPort
        Proxy.Authentication = SbbTranslator.ProxyAuth(CType(_settings.ProxyAuthMethod, ProxyAuthenticationMethod))
        Proxy.Username = _settings.ProxyUserName
        Proxy.Password = _settings.ProxyPassword
    End Sub

    Private Sub LogProxy()
        If Not _settings.IsProxyEnabled Then Return
        Append($"HTTP proxy szerver {_settings.ProxyServer}:{_settings.ProxyPort}, felhasználó: '{_settings.ProxyUserName}', autentikáció: {SbbTranslator.ProxyAuthName(CType(_settings.ProxyAuthMethod, ProxyAuthenticationMethod))} beállítva")
    End Sub

    Private Sub Append(Message As String)
        _log.AppendLine(Message)
    End Sub

    ''' <summary>Records the operator-facing error and the log on the result and returns it, so callers can `Return Fail(...)`.</summary>
    Private Function Fail(Res As SignatureResult, Message As String) As SignatureResult
        Res.ErrorMessage = Message
        _log.AppendLine(Message)
        Res.SignatureLog = _log.ToString()
        _logger.Error("{0}", Message)
        Return Res
    End Function

#End Region
End Class
```

- [ ] **Step 2: Switch `SignatureOperation` over**

In `Signature\SignatureOperation.vb`:

- Replace
  ```vb
            If TypeOf sigReq.SignSettings Is PDFSignerCryptoProvider Then
                Using sbb As New SBBPDF
                    sbb.Initialize(sigReq.SignSettings)
                    sigRes = sbb.SignDocument(sigReq)
                End Using
            End If
  ```
  with
  ```vb
            If TypeOf sigReq.SignSettings Is PDFSignerCryptoProvider Then
                Dim sbb As New SbbSignatureCreator
                sbb.Initialize(sigReq.SignSettings)
                sigRes = sbb.SignDocument(sigReq)
            End If
  ```
- Replace the body of `GetCertificatesFromMyWinCertStore` with
  ```vb
        Dim sbb As New SbbSignatureCreator
        Return sbb.GetCertificatesFromStore(QualifiedCertificatesOnly)
  ```
- Replace the body of `ActivateSBBLicense` with `SbbSignatureCreator.ActivateLicense()`.

- [ ] **Step 3: Delete the legacy engine**

```powershell
Remove-Item "PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBPDF.vb", "PDF Signer for Tungsten Capture\Signature\Modules\SBB\SBBCodeTranslator.vb"
Select-String -Path "PDF Signer for Tungsten Capture\**\*.vb" -Pattern "SBBPDF|SBBCodeTranslator|_CodeTranslator" -Recurse
```
The search must return nothing. `ServerCertificateValidator.vb` stays (its `HostBoundCertificateValidator` is no longer referenced by the module; keep the file untouched, it compiles on its own).

- [ ] **Step 4: Build and run the tests**

Expected `0 Error(s)` and all tests pass. Typical fixes, none of which change the design: event-args type or property names (see Task 12 Step 4 for the discovery command); `certManager.Certificate` being read-only in a lambda closure (assign it to a local `Dim signingCert As Certificate = certManager.Certificate` before the lambda).

- [ ] **Step 5: Smoke run**

Copy `PDF Signer for Tungsten Capture\bin\Debug\PDFSigner.exe` to `C:\Program Files (x86)\Tungsten\Capture\Bin\` (elevated), open a batch in PDF Signer with a document class configured for B-B, no revocation checking, and sign one unsigned document. Expected: the document signs, the log shows "PDF dokumentum aláírása" … "Hitelesség lejárati ideje meghatározva", Adobe Reader shows a valid signature. Record the outcome in the commit message; the full matrix is Task 16.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Replace the legacy TEl* signing module with SbbSignatureCreator on the SecureBlackbox component API"
```

---

### Task 15: Documentation, versions and the spec deviations

**Files:**
- Modify: `Help\Content\Modules\Administration\SetupDocumentClass.htm` (the "PDF Signer" section, lines ~121-152)
- Modify: `CLAUDE.md`
- Modify: `docs\superpowers\specs\2026-09-16-sbb-new-api-signing-design.md` (§3.1, §3.2)
- Modify: `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj`, `PDF Signer for Tungsten Capture Common\PDF Signer for Tungsten Capture Common.vbproj`, `PDF Signer for Tungsten Capture Setup\PDF Signer for Tungsten Capture Setup.vbproj` (`<Version>`)

- [ ] **Step 1: Help page**

In `SetupDocumentClass.htm`, inside the PDF Signer section, keep the existing HTML structure (the same `<p>`/`<li>` classes as the neighbouring items) and:

1. Under "Aláírási beállítások (Signature settings)": remove the "Visszavonás ellenőrzés (Revocation check)" item and add after "Aláírás oka":
   `PAdES szint (PAdES level): az elkészítendő aláírás szintje. PAdES B-B: csak aláírás. PAdES B-T: aláírás és aláírás-időbélyeg. PAdES B-LT: aláírás, időbélyeg és a dokumentumba beágyazott visszavonási információk (hosszú távú ellenőrizhetőség). PAdES B-LTA: B-LT kiegészítve dokumentumszintű archív időbélyeggel.`
2. Replace the "Időbélyegzés beállításai" list with: "TSA URL", "TSA felhasználónév (TSA user name)", "Jelszó (Password)" (texts unchanged) and `Hash algoritmus (Hash method): a B-LTA archív időbélyeg lenyomatképző algoritmusa.` Remove the two removed checkbox items; keep the "Figyelem!" note but reword its first clause to `A PAdES B-LTA aláírások dokumentumonként két időbélyeget követelnek meg, …`.
3. Add a new group before the proxy group: heading `Visszavonás-ellenőrzés beállításai (Revocation checking)` with items:
   `Visszavonás-ellenőrzés bekapcsolva (Revocation checking enabled): az aláíró tanúsítvány és az időbélyeg-szolgáltató tanúsítványláncának visszavonási állapotát a rendszer online ellenőrzi; visszavont vagy nem megbízható lánc esetén az aláírás meghiúsul.`
   `Protokoll (Protocol): CRL, OCSP vagy OCSP (CRL fallback).`
   `Visszavonási információk beágyazása (Embed revocation information into the document): a letöltött visszavonási adatok a dokumentumba kerülnek (B-LT és B-LTA esetén kötelező).`
4. Add a note paragraph at the end of the PDF Signer section: `Frissítés korábbi verzióról: a dokumentumosztályok PDF Signer beállításait a frissítés előtt XML fájlba kell exportálni, majd a frissítés után importálni; a PAdES szint nélkül mentett beállításokat a rendszer alapértelmezettként kezeli.`

The screenshot `Resources\Images\Screenshots\CryptoProviderSetupPDFSigner.png` is retaken from the Task 5 form (any PNG editor; 96 dpi, same crop as the current image).

- [ ] **Step 2: CLAUDE.md**

In the "Architecture" bullet for the runtime, change `SBB` (local SecureBlackbox signing) to: `SBB` (local signing on the SecureBlackbox v24 component API: `SbbSignatureCreator` runs the passes of a `SigningPlan` built from the PAdES level and revocation switches; unsigned input only; validity end from `EtsiValidity`). In the "Build" section add a bullet: `PDF Signer for Tungsten Capture Tests` (MSTest, x86, excluded from FullRelease) covers the pure units; run it with `vstest.console.exe … /Platform:x86` after an MSBuild Debug build. Replace the sentence "There are no automated tests or linters" accordingly. Add to "Build gotchas": the SecureBlackbox assembly is 24.0.9710 (netstandard2.0 build shared with PDF Streamer) and the new components take `RuntimeLicense` from `SbbLicense.Key`.

- [ ] **Step 3: Spec deviations**

In the spec, §3.1: rename the enum to `PAdESLevelType` in the code block and text. §3.2: change the `PAdESLevel` row's type to `Integer (PAdESLevelType)` and `RevocationCheckProtocol` default stays `OCSP`. Add one sentence under §3.2: "Enum-backed settings are `Integer` properties, matching the existing provider fields and the WinForms `SelectedValue` bindings."

- [ ] **Step 4: Version bump**

Set `<Version>11.3.0.0</Version>` in the three vbproj files (runtime, Common, Setup). Leave the WiX `Package Version` alone (it is bumped at release time).

- [ ] **Step 5: Build and run the tests**

Expected `0 Error(s)`, all tests pass.

- [ ] **Step 6: Commit**

```
git add -A
git commit -m "Document the PAdES level and revocation settings; bump assemblies to 11.3"
```

---

### Task 16: Manual verification matrix (merge gate)

**Files:** none (results go into the final commit message / PR description).

**Prerequisites:** a Capture test batch class with unsigned PDFs, a signing certificate in the user's personal store with an OCSP responder, a reachable TSA (with credentials if it needs them), Adobe Reader.

- [ ] **Step 1: Deploy the Debug build**

Run the build, then copy `PDFSigner.exe`, `PDFSignerCommon.dll`, `PDFSignerSetup.dll` from the `bin\Debug` folders to `C:\Program Files (x86)\Tungsten\Capture\Bin\` (elevated shell).

- [ ] **Step 2: Sign one document per level**

For each row, configure the document class in Setup, sign one unsigned document in PDF Signer, open the result in Adobe Reader (Signature Panel) and record pass/fail with the relevant log lines:

| # | Level | Checking | Expected in Adobe Reader | Expected in the log |
|---|---|---|---|---|
| 1 | B-B | off | valid signature, no timestamp, "not LTV enabled" | no validator pass, "Hitelesség lejárati ideje meghatározva" |
| 2 | B-B | on, OCSP | as 1 | "Visszavonás-ellenőrzés: aláíró tanúsítvány" … "tanúsítványlánca érvényes" |
| 3 | B-T | on, OCSP | signature with embedded timestamp, not LTV | signing pre-check, "Időbélyeg kérése", TSA post-check passes |
| 4 | B-LT | on (forced) | timestamp present, "LTV enabled" | Update pass with OCSP/CRL downloads, "Visszavonási adatok beágyazva" |
| 5 | B-LTA | on (forced) | as 4 plus a document timestamp; last entity is a timestamp | archive timestamp pass, then a second Update on the timestamp |
| 6 | B-LT | on, proxy enabled with the site proxy | as 4 | proxy line logged, downloads succeed |
| 7 | B-T | on | error, no output | with a revoked or untrusted test certificate: "tanúsítványlánca nem érvényes" |
| 8 | any | any | error, no output | input already signed: "csak aláíratlan dokumentumot ír alá" |
| 9 | B-B | off | error, no output | qualified-only switch on, non-qualified certificate: the certificate does not appear in the picker |

Also confirm for row 4 or 5 that the Capture index field mapped to `IndexSignatureValidUntil` receives a date years in the future (the ETSI archival expiry), and that the batch closes normally.

- [ ] **Step 3: Record**

Write the matrix results into `docs\superpowers\plans\2026-09-16-sbb-new-api-signing.md` under a `## Verification record` heading (date, build commit, rows with pass/fail and notes) and commit:

```
git add -A
git commit -m "Record the manual signing verification matrix"
```

Any failing row is fixed in a follow-up task on the same branch before the branch is finished (superpowers:finishing-a-development-branch).

---

## Self-review notes

- Spec coverage: §2 decisions → Tasks 2 (assembly, license), 4 (migration rule, defaults), 8 (levels, no existing-signature pass), 13/14 (chain building from stores + harvested material), 1 (tests); §3 → Tasks 3–4; §4.1 files → Tasks 2, 6–14 (`SbbDiagnostics`-style dumps intentionally absent per §2); §4.2 → Tasks 6, 13, 14; §4.3/§4.4 → Tasks 8, 14; §4.5 → Tasks 7, 12, 14; §5 → Task 5; §6 → Task 14 Step 2; §7 → Tasks 1, 4, 6–10, 12 and Task 16 (manual matrix); §8 → Task 15.
- Type consistency: `PAdESLevelType` / `PDFSignerCryptoProvider.PAdESLevel As Integer` throughout; `SigningPlan` members identical in Tasks 8 and 14; `HarvestedMaterial` members identical in Tasks 13 and 14; `EtsiValidityInputBuilder.From(verifier, atTime, processing, trace)` identical in Tasks 11 and 14; `SbbEventLogger.Naming` used in Task 14.

