# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository overview

**`PDF Signer for Tungsten Capture.sln`** (VB.NET, legacy-style projects, .NET Framework 4.8) — a Doc-Soft custom module for Kofax/Tungsten Capture that signs scanned batch documents. Operators open Capture batches, review PDFs, and sign them.

Source control is **Git** (migrated from TFS on 2026-06-12; see `docs\superpowers\specs\2026-06-12-pdfsigner-git-migration-design.md`). The TFS working copy at `C:\Projects\PDFSigner\11.0_3.0` still exists and keeps the pre-migration history. Project and folder names contain spaces, so always quote paths.

Comments, log messages, exception texts, and UI resources are largely **Hungarian**; keep new user-facing/log text consistent with the surrounding language. Resources have `hu` satellite variants (e.g. `Messages.hu.Designer.vb`).

"Kofax" and "Tungsten" refer to the same platform (vendor rebrand); class/namespace names still say Kofax (`KofaxRegistry`, `Kofax.Capture.SDK.*`) while product/solution names say Tungsten.

## Build

Builds use MSBuild from Visual Studio (scripts hardcode `c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\`). There are no automated tests or linters; verification is build success (scripts grep the log for " 0 Error(s)").

- `PDFSigner32bit.cmd` — rebuilds the solution (FullRelease|x86), the shipping configuration; also produces the MSI.
- Quick check: `msbuild "PDF Signer for Tungsten Capture.sln" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:PostBuildEvent=` (the installer project is not built in Debug; `Debug|x86` does not work — the runtime vbproj has no such configuration).

Build gotchas:
- All four projects have a post-build event copying their output to `C:\Program Files (x86)\Tungsten\Capture\Bin\` — requires admin rights and an installed Capture; pass `/p:PostBuildEvent=` to skip.
- The runtime vbproj uses `PackageReference` (NLog, NLog.WindowsIdentity, Fody, Costura.Fody — Costura embeds dependencies into `PDFSigner.exe`), so a NuGet restore is needed on a fresh clone.
- Vendored third-party DLLs live in `lib\`: nsoftware SecureBlackbox, TallComponents PDF viewer, and the Kofax SDK per Capture version (`lib\Kofax`, `lib\Kofax 11.1`, `lib\Kofax 11.1 SP1`, `lib\Kofax 11.2`). DevExpress v25.2 assemblies are resolved from the installed DevExpress, not from `lib`.
- `lib\PDFSigner\` holds the vendored **PDF Streamer** client assemblies (`PDFStreamer.WCFCommon.dll`, `PDFStreamer.Common.dll`, `log4net.dll`), refreshed from the PDF Streamer repo's Release build (last refresh 2026-06-11 build, at migration). PDF Streamer lives in its own Git repo at `C:\Projects\PDF Streamer` (github.com/dkekesi/PDF-Streamer); rebuild it there and re-copy these DLLs when the service contract changes. (In the old TFS layout this was a direct cross-solution reference to `PDFStreamer.WCFCommon\bin\Release\net48`.)
- The solution has a third configuration, **FullRelease**, used for shipping builds.
- The installer harvests the online help at build time (`HeatDirectory` over `$(SolutionDir)Help\Output\Online Help`, regenerating `Help.wxs`); that folder is the generated output of the Help & Manual project in `Help\`.

## Architecture

A Kofax/Tungsten Capture batch-class custom module (module ID `DocSoft.PDFSigner`).

- **PDF Signer for Tungsten Capture** — the runtime exe (`PDFSigner.exe`). `BatchManagement\` handles Capture batch polling/opening/closing via the Kofax SDK; `Signature\SignatureOperation.vb` dispatches to pluggable signing providers under `Signature\Modules\`: `SBB` (local SecureBlackbox signing), `MNBSigner` (MNB — Hungarian National Bank — web service), `MQFTP` (IBM MQ/FTP transfer), and `PDFStreamer` (delegates to the external PDF Streamer Windows service via a WCF service reference). `Viewers\` has interchangeable PDF viewers (TallComponents and DevExpress) behind `ViewerBase`.
- **PDF Signer for Tungsten Capture Common** — shared between runtime and setup: `SetupModel` and parsers that read per-document-class configuration stored in Capture setup data, license handling, Kofax registry access, and `Models\Setup\Providers\` crypto-provider descriptors mirroring the runtime signing modules.
- **PDF Signer for Tungsten Capture Setup** — a library (`PDFSignerSetup`) hosting the admin setup panel inside Capture Administration, where the per-batch-class signing configuration is defined.
- **PDF Signer for Tungsten Capture WFA** — COM-visible workflow agent (`IACWorkflowAgent`, ProgId `DocSoft.PDFSignerWFA`) that inspects batch documents and routes the batch past the PDF Signer queue when nothing needs signing.
- **PDF Signer for Tungsten Capture Installer** — WiX MSI (`PDFSignerSetup.msi` output name `PDFSignerSetup`).

### Documentation

`Dokumentaciok\` holds the developer and installation manuals (Hungarian, Word/PDF); `Help\` is the Help & Manual authoring project whose generated webhelp (`Help\Output\Online Help`) is packaged by the installer.
