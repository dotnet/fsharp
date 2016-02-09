@echo off

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
echo build.cmd ^<all^|build^|debug^|release^|compiler^|pcls^|vs^|ci^|ci_part1^|ci_part2^>
echo.
echo No arguments default to 'build' 
echo.
echo To specify multiple values, separate strings by comma
echo.
echo The example below run pcls, vs and qa:
echo.
echo build.cmd pcls,vs,debug
exit /b 1

:ARGUMENTS_OK

set BUILD_PROTO=0
set BUILD_NET40=1
set BUILD_PORTABLE47=0
set BUILD_PORTABLE7=0
set BUILD_PORTABLE78=0
set BUILD_PORTABLE259=0
set BUILD_VS=0
set BUILD_FSHARP_DATA_TYPEPROVIDERS=0
set TEST_COMPILERUNIT=0
set TEST_NET40_COREUNIT=0
set TEST_PORTABLE47_COREUNIT=0
set TEST_PORTABLE7_COREUNIT=0
set TEST_PORTABLE78_COREUNIT=0
set TEST_PORTABLE259_COREUNIT=0
set TEST_VS=0
set TEST_FSHARP_SUITE=0
set TEST_TAGS=
set TEST_FSHARPQA_SUITE=0
set BUILD_CONFIG=Release
set BUILD_CONFIG_LOWER=release

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
set ARG=%~1

if "%ARG%" == "1" if "%2" == "" (
    set ARG=vuild
)

if "%2" == "" if not "%ARG%" == "build" goto :EOF

echo Parse argument %ARG%

if /i '%ARG%' == 'compiler' (
    set TEST_COMPILERUNIT=1
)

if /i '%ARG%' == 'pcls' (
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set TEST_PORTABLE47_COREUNIT=1
    set TEST_PORTABLE7_COREUNIT=1
    set TEST_PORTABLE78_COREUNIT=1
    set TEST_PORTABLE259_COREUNIT=1
)

if /i '%ARG%' == 'vs' (
    set BUILD_VS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'all' (
    set BUILD_PROTO=1
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set BUILD_VS=1
    set BUILD_FSHARP_DATA_TYPEPROVIDERS=1
    set TEST_COMPILERUNIT=1
    set TEST_PORTABLE47_COREUNIT=1
    set TEST_PORTABLE7_COREUNIT=1
    set TEST_PORTABLE78_COREUNIT=1
    set TEST_PORTABLE259_COREUNIT=1
    set TEST_VS=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
)

REM Same as 'all' but smoke testing only
if /i '%ARG%' == 'ci' (
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set BUILD_VS=1
    set BUILD_FSHARP_DATA_TYPEPROVIDERS=1
    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE47_COREUNIT=1
    set TEST_PORTABLE7_COREUNIT=1
    set TEST_PORTABLE78_COREUNIT=1
    set TEST_PORTABLE259_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_VS=0
    set TEST_TAGS=
)

REM These divide 'ci' into three chunks which can be done in parallel

if /i '%ARG%' == 'ci_part1' (
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set BUILD_VS=1
    set BUILD_FSHARP_DATA_TYPEPROVIDERS=1
    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE47_COREUNIT=1
    set TEST_PORTABLE7_COREUNIT=1
    set TEST_PORTABLE78_COREUNIT=1
    set TEST_PORTABLE259_COREUNIT=1
    set TEST_VS=0
    set TEST_TAGS=
)

if /i '%ARG%' == 'ci_part2' (
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set BUILD_FSHARP_DATA_TYPEPROVIDERS=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_FSHARP_SUITE=1
    set TEST_TAGS=
)

if /i '%ARG%' == 'smoke' (
    REM Smoke tests are a very small quick subset of tests

    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=0
    set TEST_TAGS=Smoke

)

if /i '%ARG%' == 'debug' (
    set BUILD_CONFIG=Debug
    set BUILD_CONFIG_LOWER=debug
)

if /i '%ARG%' == 'build' (
    set BUILD_PORTABLE47=1
    set BUILD_PORTABLE7=1
    set BUILD_PORTABLE78=1
    set BUILD_PORTABLE259=1
    set BUILD_VS=1
)

goto :EOF

:MAIN

REM after this point, ARG variable should not be used, use only BUILD_* or TEST_*

echo Build/Tests configuration:
echo.
echo BUILD_NET40=%BUILD_NET40%
echo BUILD_PORTABLE47=%BUILD_PORTABLE47%
echo BUILD_PORTABLE7=%BUILD_PORTABLE7%
echo BUILD_PORTABLE78=%BUILD_PORTABLE78%
echo BUILD_PORTABLE259=%BUILD_PORTABLE259%
echo BUILD_VS=%BUILD_VS%
echo BUILD_FSHARP_DATA_TYPEPROVIDERS=%BUILD_FSHARP_DATA_TYPEPROVIDERS%
echo.
echo TEST_COMPILERUNIT=%TEST_COMPILERUNIT%
echo TEST_PORTABLE47_COREUNIT=%TEST_PORTABLE47_COREUNIT%
echo TEST_PORTABLE7_COREUNIT=%TEST_PORTABLE7_COREUNIT%
echo TEST_PORTABLE78_COREUNIT=%TEST_PORTABLE78_COREUNIT%
echo TEST_PORTABLE259_COREUNIT=%TEST_PORTABLE259_COREUNIT%
echo TEST_VS=%TEST_VS%
echo TEST_FSHARP_SUITE=%TEST_FSHARP_SUITE%
echo TEST_FSHARPQA_SUITE=%TEST_FSHARPQA_SUITE%
echo TEST_TAGS=%TEST_TAGS%
echo BUILD_CONFIG=%BUILD_CONFIG%
echo BUILD_CONFIG_LOWER=%BUILD_CONFIG_LOWER%
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

%_ngenexe% install .\.nuget\NuGet.exe 

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

:: Build Proto
if NOT EXIST Proto\net40\bin\fsc-proto.exe (
%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure
)
if '%BUILD_PROTO%' == '1' (
%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure
)

%_ngenexe% install Proto\net40\bin\fsc-proto.exe
@if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-build.proj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: compiler build failed && goto :failure

if '%BUILD_FSHARP_DATA_TYPEPROVIDERS%' == '1' (
%_msbuildexe% src/fsharp-typeproviders-build.proj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: type provider build failed && goto :failure
)

if '%BUILD_PORTABLE47%' == '1' (
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library portable47 build failed && goto :failure
)

if '%BUILD_PORTABLE7%' == '1' (
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library portable7 build failed && goto :failure
)

if '%BUILD_PORTABLE78%' == '1' (
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library portable78 build failed && goto :failure
)

if '%BUILD_PORTABLE259%' == '' (
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library portable259 build failed && goto :failure
)

if '%TEST_COMPILERUNIT%' == '1' (
%_msbuildexe% src/fsharp-compiler-unittests-build.proj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: compiler unittests build failed && goto :failure
)
if '%TEST_COREUNIT%' == '1' (
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library unittests build failed && goto :failure
)

if '%TEST_PORTABLE47_COREUNIT%' == '1' (
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable47 && goto :failure
)

if '%TEST_PORTABLE7_COREUNIT%' == '1' (
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable7 && goto :failure
)

if '%TEST_PORTABLE78_COREUNIT%' == '1' (
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable78 && goto :failure
)

if '%TEST_PORTABLE259_COREUNIT%' == '1' (
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable259 && goto :failure
)

if '%BUILD_VS%' == '1' (
%_msbuildexe% VisualFSharp.sln /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: VS integration build failed && goto :failure
)

@echo on
call src\update.cmd %BUILD_CONFIG_LOWER% -ngen

REM Remove lingering copies of the OSS FSharp.Core from the GAC
gacutil /u "FSharp.Core, Version=4.4.1.9055, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"

REM This clobbers the installed F# SDK on the machine
REM call vsintegration\update-vsintegration.cmd %BUILD_CONFIG_LOWER%
pushd tests

@echo on
call BuildTestTools.cmd %BUILD_CONFIG_LOWER% 
@if ERRORLEVEL 1 echo Error: 'BuildTestTools.cmd %BUILD_CONFIG_LOWER%' failed && goto :failure

@echo on
if '%TEST_FSHARP_SUITE%' == '1' (
set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=true

%_msbuildexe% fsharp\fsharp.tests.fsproj /p:Configuration=%BUILD_CONFIG%
@if ERRORLEVEL 1 echo Error: fsharp cambridge tests for nunit failed && goto :failure

call RunTests.cmd %BUILD_CONFIG_LOWER% fsharp %TEST_TAGS% 
@if ERRORLEVEL 1 type testresults\fsharp_failures.log && echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% fsharp %TEST_TAGS%' failed && goto :failure
set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=
)

if '%TEST_FSHARPQA_SUITE%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% fsharpqa %TEST_TAGS% 
@if ERRORLEVEL 1 type testresults\fsharpqa_failures.log && echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% fsharpqa %TEST_TAGS%' failed && goto :failure
)

if '%TEST_COMPILERUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% compilerunit %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% compilerunit' failed && goto :failure
)

if '%TEST_NET40_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunit %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunit' failed && goto :failure
)

if '%TEST_PORTABLE47_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable47 %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable47' failed && goto :failure
)

if '%TEST_PORTABLE7_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable7 %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable7' failed && goto :failure
)

if '%TEST_PORTABLE78_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable78 %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable78' failed && goto :failure
)

if '%TEST_PORTABLE259_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable259 %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable259' failed && goto :failure
)

if '%TEST_PORTABLE259_COREUNIT%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable259 %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable259' failed && goto :failure
)

if '%TEST_VS%' == '1' (
call RunTests.cmd %BUILD_CONFIG_LOWER% ideunit %TEST_TAGS% 
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% coreunitportable259' failed && goto :failure
)

popd

goto :eof

:failure
exit /b 1
