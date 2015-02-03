@echo on

set APPVEYOR_CI=1

:: Check prerequisites
set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see http://www.microsoft.com/en-us/download/details.aspx?id=40760. && goto :eof

set _gacutilexe="%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\gacutil.exe"
if not exist %_gacutilexe% echo Error: Could not find gacutil.exe.  && goto :eof

set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :eof

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :eof

::Build
%_gacutilexe%  /i lkg\FSharp-2.0.50726.900\bin\FSharp.Core.dll
@if ERRORLEVEL 1 echo Error: gacutil failed && goto :eof

%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :eof

%_ngenexe% install Proto\net40\bin\fsc-proto.exe
@if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :eof

%_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true 
@if ERRORLEVEL 1 echo Error: library debug build failed && goto :eof

%_msbuildexe% src/fsharp-compiler-build.proj /p:UseNugetPackages=true 
@if ERRORLEVEL 1 echo Error: compile debug build failed && goto :eof

REM We don't build new net20 FSharp.Core anymore
REM %_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true /p:TargetFramework=net20
REM @if ERRORLEVEL 1 echo Error: library net20 debug build failed && goto :eof

%_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable47
@if ERRORLEVEL 1 echo Error: library portable47 debug build failed && goto :eof

%_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable7
@if ERRORLEVEL 1 echo Error: library portable7 debug build failed && goto :eof


%_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable78
@if ERRORLEVEL 1 echo Error: library portable78 debug build failed && goto :eof

%_msbuildexe% src/fsharp-library-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable259
@if ERRORLEVEL 1 echo Error: library portable259 debug build failed && goto :eof




%_msbuildexe% src/fsharp-library-unittests-build.proj /p:UseNugetPackages=true
@if ERRORLEVEL 1 echo Error: library unittests debug build failed && goto :eof

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable47
@if ERRORLEVEL 1 echo Error: library unittests debug build failed portable47 && goto :eof

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable7
@if ERRORLEVEL 1 echo Error: library unittests debug build failed portable7 && goto :eof

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:UseNugetPackages=true /p:TargetFramework=portable78
@if ERRORLEVEL 1 echo Error: library unittests debug build failed portable78 && goto :eof


@echo on
call src\update.cmd debug -ngen

@echo on
call tests\BuildTestTools.cmd debug 
REM @if ERRORLEVEL 1 echo Error: 'tests\BuildTestTools.cmd debug' failed && goto :eof

@echo on

pushd tests

REM Disabled while working out perl problem, see https://github.com/Microsoft/visualfsharp/pull/169
REM call RunTests.cmd debug fsharp Smoke
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd debug fsharpqa Smoke' failed && goto :eof

REM Disabled while working out perl problem, see https://github.com/Microsoft/visualfsharp/pull/169
REM call RunTests.cmd debug fsharpqa Smoke
REM @if ERRORLEVEL 1 echo Error: 'RunTests.cmd debug fsharpqa Smoke' failed && goto :eof

set PATH=%PATH%;%~dp0%packages\NUnit.Runners.2.6.3\tools\
call RunTests.cmd debug coreunit
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd debug coreunit' failed && goto :eof

popd



