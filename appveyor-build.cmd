@echo on

set APPVEYOR_CI=1

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0

:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find an installation of Visual Studio && goto :eof

if exist "%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" set _msbuildexe="%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if exist "%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"      set _msbuildexe="%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe. && goto :eof

set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .\.nuget\NuGet.Config
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

:: Build
%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

%_ngenexe% install Proto\net40\bin\fsc-proto.exe
@if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: compiler build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=coreclr /p:Configuration=Release /p:RestorePackages=true
@if ERRORLEVEL 1 echo Error: library coreclr build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library portable47 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library portable7 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library portable78 build failed && goto :failure

%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library portable259 build failed && goto :failure

%_msbuildexe% src/fsharp-compiler-unittests-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: compiler unittests build failed && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library unittests build failed && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library unittests build failed portable47 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library unittests build failed portable7 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library unittests build failed portable78 && goto :failure

%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable259 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: library unittests build failed portable259 && goto :failure

%_msbuildexe% vsintegration\fsharp-vsintegration-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: VS integration build failed && goto :failure

%_msbuildexe% vsintegration\fsharp-vsintegration-unittests-build.proj /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: VS integration unit tests build failed && goto :failure

@echo on
call src\update.cmd release -ngen

@echo on
call tests\BuildTestTools.cmd release 
@if ERRORLEVEL 1 echo Error: 'tests\BuildTestTools.cmd release' failed && goto :failure

@echo on
pushd tests

call RunTests.cmd release fsharp Smoke
@if ERRORLEVEL 1 type testresults\fsharp_failures.log && echo Error: 'RunTests.cmd release fsharp Smoke' failed && goto :failure

call RunTests.cmd release fsharpqa Smoke
@if ERRORLEVEL 1 type testresults\fsharpqa_failures.log && echo Error: 'RunTests.cmd release fsharpqa Smoke' failed && goto :failure

call RunTests.cmd release compilerunit
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd release compilerunit' failed && goto :failure

call RunTests.cmd release coreunit
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd release coreunit' failed && goto :failure

call RunTests.cmd release fsharp coreclr
@if ERRORLEVEL 1 echo Error: 'RunTests.cmd release coreclr' failed && goto :failure

popd

goto :eof

:failure
exit /b 1
