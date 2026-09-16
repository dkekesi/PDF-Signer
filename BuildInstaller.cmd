@echo off
rem Rebuilds PDFSignerSetup.msi from the assemblies already in each project's bin\Release folder.
rem Run BuildApplication.cmd first; this script does not compile the application.
setlocal
set msBuildDir=c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\
pushd "%~dp0"

if not exist "PDF Signer for Tungsten Capture\bin\Release\PDFSigner.exe" (
    echo "PDFSigner.exe not found in bin\Release, run BuildApplication.cmd first"
    goto builderror
)

del "PDF Signer for Tungsten Capture Installer\bin\*.msi" 2>nul

"%msBuildDir%\msbuild.exe" "PDF Signer for Tungsten Capture Installer\PDF Signer for Tungsten Capture Installer.wixproj" /restore /p:Configuration=Release /p:Platform=x86 /t:Rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=BuildInstaller.log %*
if errorlevel 1 goto builderror

popd
goto :eof

:builderror
popd
echo "Error during installer build, see BuildInstaller.log"
pause
exit /b 1
