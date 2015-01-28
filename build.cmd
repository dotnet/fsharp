@echo off

:: Check prerequisites
set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see http://www.microsoft.com/en-us/download/details.aspx?id=40760. && goto :eof

set _gacutilexe="%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\gacutil.exe"
if not exist %_gacutilexe% echo Error: Could not find gacutil.exe.  && goto :eof

::Build
%_gacutilexe%  /i lkg\FSharp-2.0.50726.900\bin\FSharp.Core.dll
%_msbuildexe% src\fsharp-proto-build.proj
ngen install lib\proto\fsc-proto.exe
%_msbuildexe% src/fsharp-library-build.proj 
%_msbuildexe% src/fsharp-compiler-build.proj 
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=net20
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable47
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable7
%_msbuildexe% src/fsharp-library-build.proj /p:TargetFramework=portable78
%_msbuildexe% src/fsharp-library-unittests-build.proj
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable47
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable7
%_msbuildexe% src/fsharp-library-unittests-build.proj /p:TargetFramework=portable78
src\update.cmd debug -ngen
tests\BuildTestTools.cmd debug 
