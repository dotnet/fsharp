@echo off

:ARGUMENTS_VALIDATION

if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)

set BUILD_PROFILE=%1

if /I "%BUILD_PROFILE%" == "debug" (
	goto :ARGUMENTS_OK
)
if /I "%BUILD_PROFILE%" == "release" (
	goto :ARGUMENTS_OK
)

echo '%BUILD_PROFILE%' is not a valid profile
goto :USAGE

:USAGE

echo Usage:
echo Builds the source tree using a specific configuration
echo jenkins-build.cmd ^<debug^|release^>
exit /b 1

:ARGUMENTS_OK

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

set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

echo Restoring nuget packages:

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

echo Building the source tree using configuration: %BUILD_PROFILE%

%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

%_ngenexe% install Proto\net40\bin\fsc-proto.exe
@if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: compiler build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library portable47 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library portable7 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library portable78 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library portable259 build failed && goto :failure

echo Building the test tree using configuration: %BUILD_PROFILE%

%_msbuildexe% src/fsharp-compiler-unittests-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: compiler unittests build failed && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library unittests build failed && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable47 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable7 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable78 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: library unittests build failed portable259 && goto :failure

%_msbuildexe% tests/fsharp\fsharp.tests.fsproj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: fsharp cambridge tests for nunit failed && goto :failure

%_msbuildexe% vsintegration\fsharp-vsintegration-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: VS integration build failed && goto :failure

%_msbuildexe% vsintegration\fsharp-vsintegration-unittests-build.proj /p:Configuration=%BUILD_PROFILE%
@if ERRORLEVEL 1 echo Error: VS integration unit tests build failed && goto :failure

echo Running update scripts
call src\update.cmd %BUILD_PROFILE% -ngen
call vsintegration\update-vsintegration.cmd %BUILD_PROFILE%

REM Build the tools that are used for testing
@echo on
call tests/BuildTestTools.cmd %BUILD_PROFILE% 
@if ERRORLEVEL 1 echo Error: 'BuildTestTools.cmd %BUILD_PROFILE%' failed && goto :failure

REM Actually run the tests

cd tests

call RunTests.cmd %BUILD_PROFILE% fsharp Smoke
@if ERRORLEVEL 1 type testresults\fsharp_failures.log && echo Error: 'RunTests.cmd %BUILD_PROFILE% fsharp %CONF_CAMBRIDGE_SUITE%' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% fsharpqa Smoke
REM @if ERRORLEVEL 1 type testresults\fsharpqa_failures.log && echo Error: 'RunTests.cmd %BUILD_PROFILE% fsharpqa %CONF_QA_SUITE%' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% compilerunit
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% compilerunit' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% coreunit
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% coreunit' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% coreunitportable47
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% coreunitportable47' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% coreunitportable7
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% coreunitportable7' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% coreunitportable78
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% coreunitportable78' failed && goto :failure

REM call RunTests.cmd %BUILD_PROFILE% coreunitportable259
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_PROFILE% coreunitportable259' failed && goto :failure

goto :eof

:failure
exit /b 1
