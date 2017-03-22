echo "VS150COMNTOOLS: %VS150COMNTOOLS%"
REM https://github.com/fsharp/FAKE/blob/master/src/app/FakeLib/MSBuildHelper.fs
if "%VS150COMNTOOLS%" EQU "" if exists "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat"
)
if "%VS150COMNTOOLS%" EQU "" if exists "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat"
)
if "%VS150COMNTOOLS%" EQU "" if exists "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"
)
if "%VS150COMNTOOLS%" EQU "" if exists "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat"
)
if "%VS150COMNTOOLS%" EQU "" (
    @echo "VS150COMNTOOLS not set"
    exit /B 1
)
echo "VS150COMNTOOLS: %VS150COMNTOOLS%"
msbuild /version
where msbuild