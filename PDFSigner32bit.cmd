set msBuildDir=c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\
del "PDF Signer for Tungsten Capture Installer\bin\*.msi"

"%msBuildDir%\msbuild.exe" "PDF Signer for Tungsten Capture.sln" /p:Configuration=FullRelease /property:Platform="x86" /l:FileLogger,Microsoft.Build.Engine;logfile=BuildReleasePDFSignerForTungstenCapture.log /t:Rebuild
find " 0 Error(s)" BuildReleasePDFSignerForTungstenCapture.log
if errorlevel 1 goto builderror

goto done

:builderror
echo "Error during build"
pause

:done
