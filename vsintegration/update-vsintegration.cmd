@rem ===========================================================================================================
@rem Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, 
@rem               Version 2.0.  See License.txt in the project root for license information.
@rem ===========================================================================================================

rem @echo off
setlocal

if /i "%1" == "debug" goto :ok
if /i "%1" == "release" goto :ok

echo Clobbers existing Visual Studio installation of F# bits
echo Usage:
echo    update-vsintegration.cmd debug 
echo    update-vsintegration.cmd release 
exit /b 1

:ok

set BINDIR=%~dp0..\%1\net40\bin

if /i "%PROCESSOR_ARCHITECTURE%"=="x86" set X86_PROGRAMFILES=%ProgramFiles%
if /I "%PROCESSOR_ARCHITECTURE%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B

set GACUTIL="%WINSDKNETFXTOOLS%gacutil.exe"
set SN32="%WINSDKNETFXTOOLS%sn.exe"
set SN64="%WINSDKNETFXTOOLS%x64\sn.exe"
set NGEN32=%windir%\Microsoft.NET\Framework\v4.0.30319\ngen.exe
set NGEN64=%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe

copy /y "%BINDIR%\fsc.exe" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\fsc.exe.config" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\FSharp.Build.dll" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\FSharp.Compiler.dll" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\FSharp.Compiler.Interactive.Settings.dll" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\Fsi.exe" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\Fsi.exe.config" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\FsiAnyCPU.exe" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\FsiAnyCPU.exe.config" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\Microsoft.FSharp.targets" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"
copy /y "%BINDIR%\Microsoft.Portable.FSharp.targets" "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0"

mkdir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055"
copy /y "%BINDIR%\FSharp.Core.dll" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055"
copy /y "%BINDIR%\FSharp.Core.optdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055"
copy /y "%BINDIR%\FSharp.Core.sigdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055"
copy /y "%BINDIR%\FSharp.Core.xml" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055"

mkdir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.9055"
copy /y "%BINDIR%\..\..\portable7\bin\FSharp.Core.dll" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.9055"
copy /y "%BINDIR%\..\..\portable7\bin\FSharp.Core.optdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.9055"
copy /y "%BINDIR%\..\..\portable7\bin\FSharp.Core.sigdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.9055"
copy /y "%BINDIR%\..\..\portable7\bin\FSharp.Core.xml" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.9055"

mkdir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.9055"
copy /y "%BINDIR%\..\..\portable78\bin\FSharp.Core.dll" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.9055"
copy /y "%BINDIR%\..\..\portable78\bin\FSharp.Core.optdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.9055"
copy /y "%BINDIR%\..\..\portable78\bin\FSharp.Core.sigdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.9055"
copy /y "%BINDIR%\..\..\portable78\bin\FSharp.Core.xml" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.9055"

mkdir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.9055"
copy /y "%BINDIR%\..\..\portable259\bin\FSharp.Core.dll" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.9055"
copy /y "%BINDIR%\..\..\portable259\bin\FSharp.Core.optdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.9055"
copy /y "%BINDIR%\..\..\portable259\bin\FSharp.Core.sigdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.9055"
copy /y "%BINDIR%\..\..\portable259\bin\FSharp.Core.xml" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.9055"

mkdir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.9055"
copy /y "%BINDIR%\..\..\portable47\bin\FSharp.Core.dll" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.9055"
copy /y "%BINDIR%\..\..\portable47\bin\FSharp.Core.optdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.9055"
copy /y "%BINDIR%\..\..\portable47\bin\FSharp.Core.sigdata" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.9055"
copy /y "%BINDIR%\..\..\portable47\bin\FSharp.Core.xml" "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.9055"

REM echo ^<configuration^>^<runtime^>^<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1" appliesTo="v4.0.30319"^>^<dependentAssembly^>^<assemblyIdentity name="FSharp.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" /^> ^<bindingRedirect oldVersion="2.0.0.0-4.4.0.0" newVersion="4.4.0.9055"/^>^</dependentAssembly^>^</assemblyBinding^>^</runtime^>^</configuration^> > "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055\pub.config"

if /I "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    REG ADD "HKLM\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\F# 4.0 Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055\
    REG ADD "HKLM\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.50709\AssemblyFoldersEx\F# 4.0 Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055\
)
REG ADD "HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\F# 4.0 Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055\
REG ADD "HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.50709\AssemblyFoldersEx\F# 4.0 Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.9055\



rem Disable strong-name validation for F# binaries built from open source that are signed with the microsoft key
%SN32% -Vr FSharp.Core,b03f5f7f11d50a3a
%SN32% -Vr FSharp.Build,b03f5f7f11d50a3a
%SN32% -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
%SN32% -Vr FSharp.Compiler.Hosted,b03f5f7f11d50a3a

%SN32% -Vr FSharp.Compiler,b03f5f7f11d50a3a
%SN32% -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
%SN32% -Vr FSharp.Editor,b03f5f7f11d50a3a
%SN32% -Vr FSharp.LanguageService,b03f5f7f11d50a3a
%SN32% -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
%SN32% -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
%SN32% -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
%SN32% -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
%SN32% -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
%SN32% -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
%SN32% -Vr Unittests,b03f5f7f11d50a3a
%SN32% -Vr Salsa,b03f5f7f11d50a3a

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    %SN64% -Vr FSharp.Core,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.Build,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.Compiler.Hosted,b03f5f7f11d50a3a

    %SN64% -Vr FSharp.Compiler,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.Editor,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.LanguageService,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
    %SN64% -Vr FSharp.VS.FSI,b03f5f7f11d50a3a
    %SN64% -Vr Unittests,b03f5f7f11d50a3a
    %SN64% -Vr Salsa,b03f5f7f11d50a3a
)

%GACUTIL% /if %BINDIR%\FSharp.Compiler.Interactive.Settings.dll
%GACUTIL% /if %BINDIR%\FSharp.Compiler.Server.Shared.dll

rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll

"%NGEN32%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsc.exe" /queue:1
"%NGEN32%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsi.exe" /queue:1
"%NGEN32%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsiAnyCpu.exe" /queue:1
"%NGEN32%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\FSharp.Build.dll" /queue:1

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%NGEN64%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\fsiAnyCpu.exe" /queue:1
    "%NGEN64%" install "%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0\FSharp.Build.dll" /queue:1
)
