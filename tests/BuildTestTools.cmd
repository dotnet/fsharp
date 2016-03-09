@echo off

if /i "%1" == "debug" goto :ok
if /i "%1" == "release" goto :ok

echo Builds a few test tools using latest compiler and runtime
echo Usage:
echo    BuildTestTools.cmd debug
echo    BuildTestTools.cmd release
exit /b 1

:ok

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 15.0\common7\ide\devenv.exe" set VisualStudioVersion=15.0
if exist "%ProgramFiles%\Microsoft Visual Studio 15.0\common7\ide\devenv.exe" set VisualStudioVersion=15.0
if exist "%VS150COMNTOOLS%" set VisualStudioVersion=15.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%VS140COMNTOOLS%" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%VS120COMNTOOLS%" set VisualStudioVersion=12.0

:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find an installation of Visual Studio && goto :eof

if exist "%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" set _msbuildexe="%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if exist "%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"      set _msbuildexe="%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe. && goto :eof

if not exist "%~dp0fsharpqa\testenv\bin" mkdir "%~dp0fsharpqa\testenv\bin"  || goto :error
%_msbuildexe% %~dp0fsharpqa\testenv\src\ILComparer\ILComparer.fsproj /p:Configuration=%1 /t:Build  || goto :error
xcopy /Y %~dp0fsharpqa\testenv\src\ILComparer\bin\%1\* %~dp0fsharpqa\testenv\bin  || goto :error

%_msbuildexe% %~dp0fsharpqa\testenv\src\diff\diff.fsproj /p:Configuration=%1 /t:Build  || goto :error
xcopy /Y %~dp0fsharpqa\testenv\src\diff\bin\%1\* %~dp0fsharpqa\testenv\bin  || goto :error

%_msbuildexe% %~dp0fsharpqa\testenv\src\HostedCompilerServer\HostedCompilerServer.fsproj /p:Configuration=%1 /t:Build  || goto :error
xcopy /Y %~dp0fsharpqa\testenv\src\HostedCompilerServer\bin\%1\* %~dp0fsharpqa\testenv\bin  || goto :error

%_msbuildexe% %~dp0fsharpqa\testenv\src\ExecAssembly\ExecAssembly.fsproj /p:Configuration=%1 /t:Build /p:Platform=x86  || goto :error
xcopy /IY %~dp0fsharpqa\testenv\src\ExecAssembly\bin\%1\* %~dp0fsharpqa\testenv\bin\x86  || goto :error

%_msbuildexe% %~dp0fsharpqa\testenv\src\ExecAssembly\ExecAssembly.fsproj /p:Configuration=%1 /t:Build /p:Platform=x64  || goto :error
xcopy /IY %~dp0fsharpqa\testenv\src\ExecAssembly\bin\%1\* %~dp0fsharpqa\testenv\bin\AMD64  || goto :error

if exist %~dp0..\%1\net40\bin (
    xcopy /Y %~dp0..\%1\net40\bin\FSharp.Core.sigdata %~dp0fsharpqa\testenv\bin  || goto :error
    xcopy /Y %~dp0..\%1\net40\bin\FSharp.Core.optdata %~dp0fsharpqa\testenv\bin  || goto :error
)

echo set NUNITPATH=%~dp0%..\packages\NUnit.Console.3.0.0\tools\
set NUNITPATH=%~dp0%..\packages\NUnit.Console.3.0.0\tools\
echo if not exist "%NUNITPATH%" 
if not exist "%NUNITPATH%" (
    echo here
    pushd %~dp0..
    .\.nuget\nuget.exe restore packages.config -PackagesDirectory packages
    popd
)    
echo  "%NUNITPATH%*.*"  "%~dp0fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
xcopy "%NUNITPATH%*.*"  "%~dp0fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
echo  "%~dp0fsharpqa\testenv\src\nunit*.*" "%~dp0fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
xcopy "%~dp0fsharpqa\testenv\src\nunit*.*" "%~dp0fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
goto :EOF

:error
echo Failed with error %errorlevel%.
exit /b %errorlevel%
