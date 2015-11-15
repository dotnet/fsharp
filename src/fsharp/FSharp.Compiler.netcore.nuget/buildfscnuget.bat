@echo off

set pkgversion="1.0.0-alpha-0002"

:: Check prerequisites
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
if '%VisualStudioVersion%' == '14.0' (
    if exist "%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" set _fsiexe="%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe"
)

if '%VisualStudioVersion%' == '12.0' (
    if exist "%ProgramFiles(x86)%\Microsoft SDKs\F#\3.1\Framework\v4.0\fsi.exe" set _fsiexe="%ProgramFiles(x86)%\Microsoft SDKs\F#\3.1\Framework\v4.0\fsi.exe"
)

if not exist "%~dp0..\..\..\release\coreclr\nuget" md "%~dp0..\..\..\release\coreclr\nuget"
%_fsiexe% --exec "%~dp0..\..\buildtools\buildnugets.fsx" %pkgversion% "%~dp0Microsoft.FSharp.Compiler.netcore.nuspec" "%~dp0..\..\..\release\coreclr\bin" "%~dp0..\..\..\release\coreclr\nuget"
