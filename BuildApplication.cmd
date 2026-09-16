@echo off
rem Rebuilds the PDF Signer assemblies (runtime, Common, Setup, WFA) in the shipping FullRelease/x86 configuration.
rem The installer is not built; run BuildInstaller.cmd afterwards to package the output.
setlocal
set msBuildDir=c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\
pushd "%~dp0"

rem Building the runtime's solution target also builds Setup, WFA and Common through its dependencies.
"%msBuildDir%\msbuild.exe" "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=FullRelease /p:Platform=x86 /t:"PDF Signer for Tungsten Capture:Rebuild" /l:FileLogger,Microsoft.Build.Engine;logfile=BuildApplication.log %*
if errorlevel 1 goto builderror

popd
goto :eof

:builderror
popd
echo "Error during application build, see BuildApplication.log"
pause
exit /b 1
