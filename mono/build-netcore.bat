@echo off

:: Check prerequisites
set _msbuildexe="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see http://www.microsoft.com/en-us/download/details.aspx?id=40760. && goto :eof

set msbuildflags=/maxcpucount

::Build

%_msbuildexe% %msbuildflags% lib\netcore\build-fsc-netcore.proj /v:n
@if ERRORLEVEL 1 echo Error: "%_msbuildexe% %msbuildflags% lib\netcore\build-fsc-netcore.proj" failed  && goto :failure


@echo "Finished"
goto :eof

:failure
exit /b 1
