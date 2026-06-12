# PDF Signer for Tungsten Capture → Git migration design

**Date:** 2026-06-12
**Goal:** Stand up the PDF Signer for Tungsten Capture solution as a standalone Git repository in `c:\Projects\PDF Signer`, copied out of the TFS working copy `c:\Projects\PDFSigner\11.0_3.0`. The original folder stays under TFS, untouched. Companion to the PDF Streamer migration done earlier the same day (`2026-06-12-pdfstreamer-git-migration-design.md`).

## Decisions

- **History:** fresh start, one initial commit; TFS retains the old history.
- **Remote:** `origin = https://github.com/dkekesi/PDF-Signer.git`, **nothing pushed** — the user pushes manually.
- **TFS bindings:** stripped from the copied files only.
- **Cross-solution `PDFStreamer.WCFCommon` reference:** the runtime vbproj's HintPath `..\PDFStreamer.WCFCommon\bin\Release\net48\PDFStreamer.WCFCommon.dll` cannot resolve in the new repo (the Streamer lives in its own repo now). The pre-existing vendor folder `lib\PDFSigner\` (stale 2023 copies of `PDFStreamer.Common.dll` + `PDFStreamer.WCFCommon.dll`) is **refreshed** from the current Streamer Release build (2026-06-11, incl. `log4net.dll`) and the HintPath is repointed to `..\lib\PDFSigner\PDFStreamer.WCFCommon.dll`.
- **Help content:** the installer's `HeatDirectory`/`SourceDirHU` harvest `$(SolutionDir)..\Help\Output\Online Help` — a folder **outside** the working copy (`c:\Projects\PDFSigner\Help\Output\Online Help`, shared across TFS version folders). That generated webhelp (~3.4 MB) is vendored into the new repo at `Help\Output\Online Help` and the wixproj paths change from `$(SolutionDir)..\Help\...` to `$(SolutionDir)Help\...`, making the repo self-contained.

## Copy manifest

Source root: `c:\Projects\PDFSigner\11.0_3.0`. Destination: `c:\Projects\PDF Signer`.

| Item | Notes |
|---|---|
| `PDF Signer for Tungsten Capture.sln` | `GlobalSection(TeamFoundationVersionControl)` removed |
| `PDF Signer for Tungsten Capture\` | excl. `bin\`, `obj\`, `.vs\`, `*.vspscc`, `*.user`, `*.log` |
| `PDF Signer for Tungsten Capture Common\` | same exclusions |
| `PDF Signer for Tungsten Capture Setup\` | same exclusions |
| `PDF Signer for Tungsten Capture WFA\` | same exclusions |
| `PDF Signer for Tungsten Capture Installer\` | same exclusions (drops `HeatWaveConvert.log`) |
| `lib\Kofax*\` (all four version folders) | Kofax SDK per Capture version |
| `lib\PDFSigner\` | refreshed from `PDFStreamer.WCFCommon\bin\Release\net48\` (see Decisions) |
| `lib\nsoftware.SecureBlackbox.dll` | referenced by runtime vbproj |
| `lib\TallComponents.PDF.Controls.WinForms.dll` | referenced by runtime vbproj |
| `Help\` | authoring project; `Output\Dani` (personal output) skipped |
| outer `Help\Output\Online Help` → `Help\Output\Online Help` | installer harvest source, vendored in |
| `Dokumentaciok\` | Signer developer/installation manuals |
| `PDFSigner32bit.cmd` | stale `PDF Signer for Kofax Capture` names fixed to `Tungsten` |
| `CLAUDE.md` | **rewritten** Signer-only (Streamer/TFS sections dropped, Git noted) |
| this design doc | copied to `docs\superpowers\specs\` in the new repo |

Explicitly **not** copied: all `PDFStreamer.*` project folders and `PDF Streamer.sln` (own repo at `c:\Projects\PDF Streamer`), `Transformers\`, `MNBSignerTest\` (separate standalone solution), `packages\` (a packages.config-era cache; the runtime vbproj uses `PackageReference` — NLog, NLog.WindowsIdentity, Fody, Costura.Fody — restored from the global NuGet cache, not from `packages\`), `.nuget\`, `.tfignore`, `*.vssscc`, `Testfiles\`, `PDF Examples\`, `Signed document for ETSI conformance checker.pdf`, `todo_PDF_Streamer.txt`, `SafeStartService.cmd`/`SafeStopService.cmd`, `PDFStreamer64bit*.cmd`, `lib\DevExpress\` (Signer has no DevExpress HintPaths; resolves from installed DevExpress), `lib\TiffViewerControl.dll`/`BitMiracle.LibTiff.NET.dll`/`Equin.ApplicationFramework.BindingListView.dll` (referenced by no Signer project).

## TFS binding cleanup (copied files only)

1. `.sln`: delete `GlobalSection(TeamFoundationVersionControl)`. (`TestCaseManagementSettings` referencing the long-gone `PDF Signer NET.vsmdi` is left as-is — not a TFS binding.)
2. The four `.vbproj` + the `.wixproj`: delete `SccProjectName`, `SccLocalPath`, `SccAuxPath`, `SccProvider`.
3. `*.vspscc`/`*.vssscc` excluded at copy time.

## Git setup

`.gitignore` (`bin/`, `obj/`, `.vs/`, `*.user`, `*.log`), `git init -b main`, single commit `Import PDF Signer for Tungsten Capture from TFS working copy 11.0_3.0`, `git remote add origin https://github.com/dkekesi/PDF-Signer.git`, no push.

## Verification

- Verified with `msbuild "PDF Signer for Tungsten Capture.sln" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:PostBuildEvent=` → all four projects built. Notes discovered during verification: `/restore` is required (runtime vbproj uses PackageReference); all four projects have a post-build event copying their output to `C:\Program Files (x86)\Tungsten\Capture\Bin\` (needs admin + installed Capture — disabled via `/p:PostBuildEvent=` for verification); the runtime vbproj has no Debug|x86 configuration, so the Debug|x86 solution config fails on it (pre-existing; shipping config is FullRelease|x86, which maps the runtime to Release|x86). The WiX installer was not exercised (not built in Debug).
- `git status` clean; `git ls-files` free of `bin/`, `obj/`, `.vssscc`, `.vspscc`, Streamer files.
- Original `11.0_3.0` untouched apart from this spec file.

## Out of scope

- Pushing to GitHub (user does it).
- TFS history migration.
- Retiring the Signer files from the TFS working copy — copies coexist; authoritative copy TBD.
