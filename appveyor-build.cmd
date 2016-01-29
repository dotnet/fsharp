@if "%_echo%"=="" echo off

:ARGUMENTS_VALIDATION

if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)
goto :ARGUMENTS_OK

:USAGE

echo Build and run a subset of test suites
echo.
echo Usage:
echo.
echo appveyor-build.cmd ^<all^|net40^|portable47^|portable7^|portable78^|portable259^|vs^|cambridge_suite^|qa_suite^|smoke^|smoke_only^|build_only^>
echo.
echo No arguments default to 'smoke' ( build all profiles, run all unit tests, cambridge Smoke, fsharpqa Smoke)
echo.
echo To specify multiple values, separate strings by comma
echo.
echo The example below run portable tests, vs and qa:
echo.
echo appveyor-build.cmd portable,vs,qa_suite
exit /b 1

:ARGUMENTS_OK

set DO_NET40=0
set DO_PORTABLE=0
set DO_VS=0
set TEST_NET40=0
set TEST_PORTABLE=0
set TEST_VS=0
set TEST_CAMBRIDGE_SUITE=0
set CONF_CAMBRIDGE_SUITE=
set TEST_QA_SUITE=0
set CONF_QA_SUITE=

setlocal enableDelayedExpansion
set /a counter=0
for /l %%x in (1 1 9) do (
    set /a counter=!counter!+1
    call :SET_CONFIG %%!counter! "!counter!"
)
setlocal disableDelayedExpansion
echo.
echo.

goto :MAIN

:SET_CONFIG
set BUILD_PROFILE=%~1

if "%RestorePackages%"=="" ( 
    set RestorePackages=true 
)

if "%BUILD_PROFILE%" == "1" if "%2" == "" (
    set BUILD_PROFILE=smoke
)

if "%2" == "" if not "%BUILD_PROFILE%" == "smoke" goto :EOF

echo Parse argument %BUILD_PROFILE%

if /i '%BUILD_PROFILE%' == 'net40' (
    set DO_NET40=1
    set TEST_NET40=1
)

if /i '%BUILD_PROFILE%' == 'portable' (
    set DO_PORTABLE=1
    set TEST_PORTABLE=1
)

if /i '%BUILD_PROFILE%' == 'vs' (
    set DO_VS=1
    set TEST_VS=1
)

if /i '%BUILD_PROFILE%' == 'cambridge_suite' (
    set DO_NET40=1
    set TEST_CAMBRIDGE_SUITE=1
)

if /i '%BUILD_PROFILE%' == 'qa_suite' (
    set DO_NET40=1
    set TEST_QA_SUITE=1
)

if /i '%BUILD_PROFILE%' == 'all' (
    set DO_NET40=1
    set DO_PORTABLE=1
    set DO_VS=1
    set TEST_NET40=1
    set TEST_PORTABLE=1
    set TEST_VS=1
    set TEST_CAMBRIDGE_SUITE=1
    set TEST_QA_SUITE=1
)

if /i '%BUILD_PROFILE%' == 'smoke' (
    set DO_NET40=1
    set DO_PORTABLE=1
    set DO_VS=1
    set TEST_NET40=1
    set TEST_PORTABLE=1
    set TEST_VS=1
    set TEST_CAMBRIDGE_SUITE=1
    set CONF_CAMBRIDGE_SUITE=Smoke
    set TEST_QA_SUITE=1
    set CONF_QA_SUITE=Smoke
)

if /i '%BUILD_PROFILE%' == 'smoke_only' (
    set CONF_CAMBRIDGE_SUITE=Smoke
    set CONF_QA_SUITE=Smoke
)

if /i '%BUILD_PROFILE%' == 'build_only' (
    set TEST_NET40=0
    set TEST_PORTABLE47=0
    set TEST_PORTABLE7=0
    set TEST_PORTABLE78=0
    set TEST_PORTABLE259=0
    set TEST_VS=0
    set TEST_CAMBRIDGE_SUITE=0
    set TEST_QA_SUITE=0
)

goto :EOF

:MAIN

REM after this point, BUILD_PROFILE variable should not be used, use only DO_* or TEST_*

echo Build/Tests configuration:
echo.
echo DO_NET40=%DO_NET40%
echo DO_PORTABLE=%DO_PORTABLE%
echo DO_VS=%DO_VS%
echo.
echo TEST_NET40=%TEST_NET40%
echo TEST_PORTABLE=%TEST_PORTABLE%
echo TEST_VS=%TEST_VS%
echo TEST_CAMBRIDGE_SUITE=%TEST_CAMBRIDGE_SUITE%
echo CONF_CAMBRIDGE_SUITE=%CONF_CAMBRIDGE_SUITE%
echo TEST_QA_SUITE=%TEST_QA_SUITE%
echo CONF_QA_SUITE=%CONF_QA_SUITE%
echo.

@echo on

set APPVEYOR_CI=1

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS140COMNTOOLS%..\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS120COMNTOOLS%..\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0

:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find an installation of Visual Studio && goto :failure

if exist "%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" set _msbuildexe="%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if exist "%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"      set _msbuildexe="%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe. && goto :failure

:: See <http://www.appveyor.com/docs/environment-variables>
if defined APPVEYOR (
    rem See <http://www.appveyor.com/docs/build-phase>
    if exist "C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" (
	rem HACK HACK HACK
	set _msbuildexe=%_msbuildexe% /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
    )
)

set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

set DOTNET_HOME  .\packages\dotnet

rem check to see if the dotnet cli tool exists
set _dotnetexe=".\packages\dotnet\bin\dotnet.exe"
if not exist %_dotnetexe% (
    echo Error: Could not find %_dotnetexe%.
    rem do zipfile install nonsense
    if not exist packages ( md packages )
    if exist packages\dotnet ( rd packages /s /q )
    powershell.exe -executionpolicy unrestricted -command .\scripts\install-dotnetcli.ps1 https://dotnetcli.blob.core.windows.net/dotnet/dev/Binaries/Latest/dotnet-win-x64.latest.zip packages
    @if ERRORLEVEL 1 echo Error: fetch dotnetcli failed && goto :failure
)

copy /y .\packages\dotnet\bin\* .\packages\lkg
@if ERRORLEVEL 1 echo Error: copy of dotnet bin to lkg failed && goto :failure

rem rename fsc and coreconsole to allow fsc.exe to to start compiler
pushd .\packages\lkg & ren fsc.exe fsc.dll & popd
copy .\packages\lkg\coreconsole.exe .\packages\lkg\fsc.exe

rem rename fsi and coreconsole to allow fsi.exe to to start interative
pushd .\packages\lkg & ren fsi.exe fsi.dll & popd
copy .\packages\lkg\coreconsole.exe .\packages\lkg\fsi.exe

:: Build
%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

%_ngenexe% install Proto\net40\bin\fsc-proto.exe
@if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:Configuration=Release  /v:diag
@if ERRORLEVEL 1 echo Error: library build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=coreclr /p:Configuration=Release /p:RestorePackages=%RestorePackages%
@if ERRORLEVEL 1 echo Error: library coreclr build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-build.proj /p:TargetFramework=coreclr /p:Configuration=Release /p:RestorePackages=%RestorePackages%
@if ERRORLEVEL 1 echo Error: compiler coreclr build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: compiler build failed && goto :failure

if '%DO_PORTABLE%' == '1' (
    
    %_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library portable47 build failed && goto :failure
    
    %_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library portable7 build failed && goto :failure
    
    %_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library portable78 build failed && goto :failure
    
    %_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library portable259 build failed && goto :failure
)

if '%TEST_NET40%' == '1' (
    %_msbuildexe% src/fsharp-compiler-unittests-build.proj /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: compiler unittests build failed && goto :failure
    
    %_msbuildexe% src/fsharp-library-unittests-build.proj /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library unittests build failed && goto :failure
)

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=coreclr /p:Configuration=Release /p:RestorePackages=%RestorePackages%
@if ERRORLEVEL 1 echo Error: library unittests build failed coreclr && goto :failure
    
if '%TEST_PORTABLE%' == '1' (
    %_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library unittests build failed portable47 && goto :failure
    
    %_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library unittests build failed portable7 && goto :failure
    
    %_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library unittests build failed portable78 && goto :failure
    
    %_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: library unittests build failed portable259 && goto :failure
)

if '%DO_VS%' == '1' (
    %_msbuildexe% vsintegration\fsharp-vsintegration-build.proj /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: VS integration build failed && goto :failure
)

if '%TEST_VS%' == '1' (
    %_msbuildexe% vsintegration\fsharp-vsintegration-unittests-build.proj /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: VS integration unit tests build failed && goto :failure
)

@echo on
call src\update.cmd release -ngen
call vsintegration\update-vsintegration.cmd release
pushd tests

@echo on
call BuildTestTools.cmd release 
@if ERRORLEVEL 1 echo Error: 'BuildTestTools.cmd release' failed && goto :failure

@echo on

if '%TEST_CAMBRIDGE_SUITE%' == '1' (
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=true
    
    %_msbuildexe% fsharp\fsharp.tests.fsproj /p:Configuration=Release
    @if ERRORLEVEL 1 echo Error: fsharp cambridge tests for nunit failed && goto :failure
    
    call RunTests.cmd release fsharp %CONF_CAMBRIDGE_SUITE%
    @if ERRORLEVEL 1 type testresults\fsharp_failures.log && echo Error: 'RunTests.cmd release fsharp %CONF_CAMBRIDGE_SUITE%' failed && goto :failure
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=
)

if '%TEST_QA_SUITE%' == '1' (
    call RunTests.cmd release fsharpqa %CONF_QA_SUITE%
    @if ERRORLEVEL 1 type testresults\fsharpqa_failures.log && echo Error: 'RunTests.cmd release fsharpqa %CONF_QA_SUITE%' failed && goto :failure
)

if '%TEST_NET40%' == '1' (
    call RunTests.cmd release compilerunit
    @if ERRORLEVEL 1 echo Error: 'RunTests.cmd release compilerunit' failed && goto :failure

    if '%TEST_PORTABLE%' == '1' (
        call RunTests.cmd release coreunitall
        @if ERRORLEVEL 1 echo Error: 'RunTests.cmd release coreunitall' failed && goto :failure
    )
    if not '%TEST_PORTABLE%' == '1' (
        call RunTests.cmd release coreunit
    )
)
if '%TEST_NET40%' == '0' (
    if '%TEST_PORTABLE%' == '1' (
        call RunTests.cmd release coreunitportable
    )
)

rem tests for TEST_VS are not executed

call RunTests.cmd release fsharp coreclr
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd release coreclr' failed && goto :failure

call RunTests.cmd release coreunitcoreclr
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd release coreunit' failed && goto :failure

popd

goto :eof

:failure
exit /b 1
