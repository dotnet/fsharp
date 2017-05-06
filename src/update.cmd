@if "%_echo%"=="" echo off
@rem ===========================================================================================================
@rem Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, 
@rem               Version 2.0.  See License.txt in the project root for license information.
@rem ===========================================================================================================

if /i "%1" == "debug" goto :ok
if /i "%1" == "release" goto :ok
if /i "%1" == "signonly" goto :ok

echo adding required strong name verification skipping, and NGening built binaries
echo Usage:
echo    update.cmd debug   [-ngen]
echo    update.cmd release [-ngen]
exit /b 1

:ok

set BINDIR=%~dp0..\%1\net40\bin

if exist "%WindowsSDK_ExecutablePath_x64%" set WINSDKNETFXTOOLS_X64=%WindowsSDK_ExecutablePath_x64%
if exist "%WindowsSDK_ExecutablePath_x86%" set WINSDKNETFXTOOLS_X86=%WindowsSDK_ExecutablePath_x86%

if not "%WindowsSDK_ExecutablePath_x86%" == "" goto :havesdk

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

                                FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6.2\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6.1\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B

set WINSDKNETFXTOOLS_x64=%WINSDKNETFXTOOLS_x86%x64\

:havesdk
set SN32="%WINSDKNETFXTOOLS_x86%sn.exe"
set SN64="%WINSDKNETFXTOOLS_x64%sn.exe"

set NGEN32=%windir%\Microsoft.NET\Framework\v4.0.30319\ngen.exe
set NGEN64=%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe

rem Disable strong-name validation for F# binaries built from open source that are signed with the microsoft key
%SN32% -q -Vr FSharp.Core,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Build,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a

%SN32% -q -Vr fsc,b03f5f7f11d50a3a
%SN32% -q -Vr fsi,b03f5f7f11d50a3a
%SN32% -q -Vr fsiAnyCpu,b03f5f7f11d50a3a

%SN32% -q -Vr HostedCompilerServer,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Compiler,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Editor,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.UIResources,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.LanguageService,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
%SN32% -q -Vr VisualFSharp.Unittests,b03f5f7f11d50a3a
%SN32% -q -Vr VisualFSharp.Salsa,b03f5f7f11d50a3a
%SN32% -q -Vr FSharp.Compiler.Unittests,b03f5f7f11d50a3a
%SN32% -q -Vr Microsoft.VisualStudio.Shell.UI.Internal,b03f5f7f11d50a3a

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    %SN64% -q -Vr FSharp.Core,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Build,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a

    %SN64% -q -Vr fsc,b03f5f7f11d50a3a
    %SN64% -q -Vr fsi,b03f5f7f11d50a3a
    %SN64% -q -Vr fsiAnyCpu,b03f5f7f11d50a3a

    %SN64% -q -Vr HostedCompilerServer,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Compiler,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Editor,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.UIResources,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.LanguageService,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
    %SN64% -q -Vr VisualFSharp.Unittests,b03f5f7f11d50a3a
    %SN64% -q -Vr VisualFSharp.Salsa,b03f5f7f11d50a3a
    %SN64% -q -Vr FSharp.Compiler.Unittests,b03f5f7f11d50a3a
    %SN64% -q -Vr Microsoft.VisualStudio.Shell.UI.Internal,b03f5f7f11d50a3a
)

if /i "%1" == "signonly" goto :eof
if /i "%1" == "debug" set NGEN_FLAGS=/Debug

rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
if /i not "%2"=="-ngen" goto :donengen

"%NGEN32%" install "%BINDIR%\fsc.exe" %NGEN_FLAGS% /queue:1 /nologo 
"%NGEN32%" install "%BINDIR%\fsi.exe" %NGEN_FLAGS% /queue:1 /nologo 
"%NGEN32%" install "%BINDIR%\FSharp.Build.dll" %NGEN_FLAGS% /queue:1 /nologo 
"%NGEN32%" executeQueuedItems 1 /nologo 

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%NGEN64%" install "%BINDIR%\fsiAnyCpu.exe" %NGEN_FLAGS% /queue:1 /nologo 
    "%NGEN64%" install "%BINDIR%\FSharp.Build.dll" %NGEN_FLAGS% /queue:1 /nologo 
    "%NGEN64%" executeQueuedItems 1 /nologo 
)

:donengen
