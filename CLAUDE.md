# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository overview

**`PDF Signer for Tungsten Capture.slnx`** (VB.NET, SDK-style projects, .NET Framework 4.8) — a Doc-Soft custom module for Kofax/Tungsten Capture that signs scanned batch documents. Operators open Capture batches, review PDFs, and sign them. Converted to SDK-style projects + slnx on 2026-06-12 (previously legacy-style vbproj + .sln); the installer is WiX 6 SDK-style (previously WiX 3).

Source control is **Git** (migrated from TFS on 2026-06-12; see `docs\superpowers\specs\2026-06-12-pdfsigner-git-migration-design.md`). The TFS working copy at `C:\Projects\PDFSigner\11.0_3.0` still exists and keeps the pre-migration history. Project and folder names contain spaces, so always quote paths.

Comments, log messages, exception texts, and UI resources are largely **Hungarian**; keep new user-facing/log text consistent with the surrounding language. Resources have `hu` satellite variants (e.g. `Messages.hu.Designer.vb`).

"Kofax" and "Tungsten" refer to the same platform (vendor rebrand); class/namespace names still say Kofax (`KofaxRegistry`, `Kofax.Capture.SDK.*`) while product/solution names say Tungsten.

## Build

Builds use MSBuild from Visual Studio (scripts hardcode `c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\`). There are no linters; verification is build success (the scripts check MSBuild's exit code) plus the `PDF Signer for Tungsten Capture Tests` unit tests.

- `BuildApplication.cmd` — rebuilds the four assemblies in FullRelease|x86, the shipping configuration (solution target `PDF Signer for Tungsten Capture:Rebuild`; installer and tests are skipped). Extra arguments pass through to MSBuild, e.g. `/p:CopyToCaptureBin=false`.
- `BuildInstaller.cmd` — rebuilds only the MSI (`PDF Signer for Tungsten Capture Installer\bin\PDFSignerSetup.msi`) from the existing `bin\Release` outputs; run `BuildApplication.cmd` first.
- Quick check: `msbuild "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false` (the installer project only builds in FullRelease|x86).
- `PDF Signer for Tungsten Capture Tests` (MSTest, x86, excluded from FullRelease) covers the pure units of the signing engine and configuration model; run it with `vstest.console.exe "PDF Signer for Tungsten Capture Tests\bin\Debug\PDFSignerTests.dll" /Platform:x86` after an MSBuild Debug build.

Build gotchas:
- All four projects have a `CopyToCaptureBin` post-build target copying their output to `C:\Program Files (x86)\Tungsten\Capture\Bin\` — requires admin rights and an installed Capture; pass `/p:CopyToCaptureBin=false` to skip.
- `/restore` is required: the runtime uses `PackageReference` (NLog, NLog.WindowsIdentity, Fody, Costura.Fody — Costura embeds dependencies into `PDFSigner.exe`) and the installer resolves `WixToolset.Sdk` from NuGet.
- Product-wide assembly metadata (`Version`, `AssemblyVersion`, `FileVersion`, `Product`, `Company`, `Copyright`) lives in `Directory.Build.props` at the repo root, currently 11.2; each vbproj keeps only its `AssemblyTitle` and `AssemblyAttribute` items (ComVisible/Guid, the TallComponents license key). **Bump versions in `Directory.Build.props`.**
- The vbproj files override `AssemblySearchPaths` to restore the classic AssemblyFoldersEx/GAC lookup that Microsoft.NET.Sdk drops — without it the installed DevExpress and the Kofax SDK's dependencies do not resolve. The runtime keeps an explicit `mscorlib` reference; removing it breaks Fody ("Could not find 'System.Object'").
- The WiX 6 installer (`wix` CLI 6.0.2 installed) harvests the webhelp via a `Files` wildcard in `Help.wxs` (WiX 3's HeatDirectory is gone); the MSI package version is `$(var.ProductVersion)`, which the wixproj sets from the shared `$(Version)`.
- Vendored third-party DLLs live in `lib\`: nsoftware SecureBlackbox, TallComponents PDF viewer, and the Kofax SDK per Capture version (`lib\Kofax`, `lib\Kofax 11.1`, `lib\Kofax 11.1 SP1`, `lib\Kofax 11.2`). DevExpress v25.2 assemblies are resolved from the installed DevExpress, not from `lib`.
- PDF Signer no longer links any PDF Streamer assemblies: it calls PDF Streamer's REST streaming endpoint (`POST /api/v1/documents/stream`) over `HttpClient` (see `Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb`). PDF Streamer dropped its net48 `PDFStreamer.WCFCommon`/`PDFStreamer.Common` assemblies in its .NET 10 / CoreWCF migration. `MemoryTributary` (previously from WCFCommon) is now a local class at `PDF Signer for Tungsten Capture\Helper\MemoryTributary.vb`. PDF Streamer lives in its own Git repo at `C:\Projects\PDF Streamer` (github.com/dkekesi/PDF-Streamer).
- The solution has a third configuration, **FullRelease**, used for shipping builds.
- The installer harvests the online help at build time (`HeatDirectory` over `$(SolutionDir)Help\Output\Online Help`, regenerating `Help.wxs`); that folder is the generated output of the Help & Manual project in `Help\`.
- The SecureBlackbox assembly (`lib\nsoftware.SecureBlackbox.dll`) is 24.0.9710 (netstandard2.0 build, shared with PDF Streamer). The `SBB` module's `PDFSigner`/`PDFVerifier`/`CertificateValidator`/`CertificateManager` components take their license from `SbbLicense.Key` as a per-instance `RuntimeLicense`.

## Architecture

A Kofax/Tungsten Capture batch-class custom module (module ID `DocSoft.PDFSigner`).

- **PDF Signer for Tungsten Capture** — the runtime exe (`PDFSigner.exe`). `BatchManagement\` handles Capture batch polling/opening/closing via the Kofax SDK; `Signature\SignatureOperation.vb` dispatches to pluggable signing providers under `Signature\Modules\`: `SBB` (local signing on the SecureBlackbox v24 component API: `SbbSignatureCreator` runs the passes of a `SigningPlan` built from the PAdES level and revocation switches; unsigned input only; validity end from `EtsiValidity`), `MNBSigner` (MNB — Hungarian National Bank — web service), `MQFTP` (IBM MQ/FTP transfer), and `PDFStreamer` (delegates to the external PDF Streamer Windows service via its REST streaming endpoint, `POST {configured-url}` → `/api/v1/documents/stream`, using `HttpClient`). `Viewers\` has interchangeable PDF viewers (TallComponents and DevExpress) behind `ViewerBase`.
- **PDF Signer for Tungsten Capture Common** — shared between runtime and setup: `SetupModel` and parsers that read per-document-class configuration stored in Capture setup data, license handling, Kofax registry access, and `Models\Setup\Providers\` crypto-provider descriptors mirroring the runtime signing modules.
- **PDF Signer for Tungsten Capture Setup** — a library (`PDFSignerSetup`) hosting the admin setup panel inside Capture Administration, where the per-batch-class signing configuration is defined.
- **PDF Signer for Tungsten Capture WFA** — COM-visible workflow agent (`IACWorkflowAgent`, ProgId `DocSoft.PDFSignerWFA`) that inspects batch documents and routes the batch past the PDF Signer queue when nothing needs signing.
- **PDF Signer for Tungsten Capture Installer** — WiX MSI (`PDFSignerSetup.msi` output name `PDFSignerSetup`).

### Documentation

`Dokumentaciok\` holds the developer and installation manuals (Hungarian, Word/PDF); `Help\` is the Help & Manual authoring project whose generated webhelp (`Help\Output\Online Help`) is packaged by the installer.
