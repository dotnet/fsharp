@echo off

:: Check prerequisites
set _msbuildexe="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see http://www.microsoft.com/en-us/download/details.aspx?id=40760. && goto :eof

set msbuildflags=/maxcpucount
set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Note: Could not find ngen.exe. 

::Build

%_ngenexe% install .\.nuget\NuGet.exe 

.\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
@if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

%_ngenexe% install packages\FSharp.Compiler.Tools.4.0.1.21\tools\fsc.exe

set BUILD_NET40=1
set BUILD_NET40_FSHARP_CORE=1
set BUILD_PORTABLE=1
set TEST_NET40_COREUNIT_SUITE=1
set TEST_PORTABLE_COREUNIT_SUITE=1

%_msbuildexe% src\fsharp-proto-build.proj
@if ERRORLEVEL 1 echo Error: "%_msbuildexe% src\fsharp-proto-build.proj" failed  && goto :failure

%_ngenexe% install Proto\net440\bin\fsc-proto.exe

%_msbuildexe% %msbuildflags% build-everything.proj /p:TargetDotnetProfile=net40 /p:Configuration=Release
@if ERRORLEVEL 1 echo Error: "%_msbuildexe% %msbuildflags% src\fsharp-library-build.proj /p:TargetDotnetProfile=net40 /p:Configuration=Release" failed  && goto :failure

%_msbuildexe% %msbuildflags% src/fsharp/FSharp.Core/FSharp.Core.fsproj /p:TargetDotnetProfile=monotouch /p:Configuration=Release /p:KeyFile=..\..\..\mono\mono.snk
@if ERRORLEVEL 1 echo Error: "%_msbuildexe% %msbuildflags% src\fsharp-library-build.proj /p:TargetDotnetProfile=monotouch /p:Configuration=Release /p:KeyFile=..\..\..\mono.snk" failed  && goto :failure


@echo "Finished"
goto :eof

:failure
exit /b 1
