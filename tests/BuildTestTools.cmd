@echo off

if /i "%1" == "debug" goto :ok
if /i "%1" == "release" goto :ok
if /i "%1" == "vsdebug" goto :ok
if /i "%1" == "vsrelease" goto :ok

echo Builds a few test tools using latest compiler and runtime
echo Usage:
echo    BuildTestTools.cmd debug
echo    BuildTestTools.cmd release
echo    BuildTestTools.cmd vsdebug
echo    BuildTestTools.cmd vsrelease
exit /b 1

:ok

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find a Visual Studio Installed.MSBuild.exe.  Please see http://www.visualstudio.com/en-us/news/vs2015-vs.aspx. && goto :eof

if '%VisualStudioVersion%'=='14.0' set _msbuildexe="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if '%VisualStudioVersion%'=='12.0' set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see http://www.microsoft.com/en-us/download/details.aspx?id=40760. && goto :eof

if not exist "%~dp0\fsharpqa\testenv\bin" mkdir "%~dp0\fsharpqa\testenv\bin"  || goto :error
%_msbuildexe% %~dp0\fsharpqa\testenv\src\ILComparer\ILComparer.fsproj /p:Configuration=%1 /t:Build  || goto :error
xcopy /Y %~dp0\fsharpqa\testenv\src\ILComparer\bin\%1\* %~dp0\fsharpqa\testenv\bin  || goto :error

%_msbuildexe% %~dp0\fsharpqa\testenv\src\HostedCompilerServer\HostedCompilerServer.fsproj /p:Configuration=%1 /t:Build  || goto :error
xcopy /Y %~dp0\fsharpqa\testenv\src\HostedCompilerServer\bin\%1\* %~dp0\fsharpqa\testenv\bin  || goto :error

if exist %~dp0\..\%1\net40\bin (
    xcopy /Y %~dp0\..\%1\net40\bin\FSharp.Core.sigdata %~dp0\fsharpqa\testenv\bin  || goto :error
    xcopy /Y %~dp0\..\%1\net40\bin\FSharp.Core.optdata %~dp0\fsharpqa\testenv\bin  || goto :error
)

goto :EOF

:error
echo Failed with error %errorlevel%.
exit /b %errorlevel%
