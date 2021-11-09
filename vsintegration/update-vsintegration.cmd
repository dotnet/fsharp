@rem ===========================================================================================================
@rem Copyright (c) Microsoft Corporation.  All Rights Reserved.
@rem               See License.txt in the project root for license information.
@rem ===========================================================================================================

@rem Notes/instructions for modifications:
@rem
@rem * Do not use "::" for comments, as the line will be parsed and can create spurious 
@rem   errors, i.e. if it contains variables, "|" or ">"  characters, esp. within "IF" 
@rem   and "FOR" compound statements
@rem
@rem * The coloring method uses the colors from color /h through a hacky trick with findstr. 
@rem   Only use filename-safe characters if you use CALL :colorEcho
@rem
@rem * Parts of this batch file require administrator permission. If such permissions aren't
@rem   available, a warning will be issued and relevant parts will not be executed.
@rem
@rem * Currently, only one paramter is parsed and combinations are not possible
@rem
@rem * Installation of F# FSC compiler and FSI are done in the SHARED SDK directory. Henceforth
@rem   each installation of Visual Studio 2017 will use the updated FSC.exe and the commandline
@rem   FSI.exe. The in-product VS FSI plugin, syntax highlighting and IntelliSense must be 
@rem   installed through VSIXInstaller.exe debug\net40\bin\VisualFSharpFull.vsix
@rem   
@rem   This procedure needs to be changed once F# supports multiple side-by-side installations
@rem   at which point everything will go through VSIXInstaller.exe

@echo off
setlocal EnableDelayedExpansion

rem Count errors, warnings and succesful copies
set ERRORCOUNT=0
set WARNCOUNT=0
set COPYCOUNT=0

rem Enable colors, but can ONLY BE USED WITH PRINTING LINES THAT FIT IN A FILENAME!
for /F "tokens=1,2 delims=#" %%a in ('"prompt #$H#$E# & echo on & for %%b in (1) do     rem"') do (
  set "DEL=%%a"
)

if /i "%1" == "debug" (
    set ACTION=debug
    set DEPLOY=yes
    set BINDIR=%~dp0..\%1\net40\bin
    goto :ok
)
if /i "%1" == "release" (
    set ACTION=release
    set DEPLOY=yes
    set BINDIR=%~dp0..\%1\net40\bin
    goto :ok
)
if /i "%1" == "restore" (
    set ACTION=restore
    set DEPLOY=no
    set BINDIR=%~dp0..\%1
    goto :ok
)
if /i "%1" == "backup" (
    set ACTION=backup
    set DEPLOY=no
    set BINDIR=%~dp0..\restore
    goto :ok
)

set GOTOHELP=yes

:ok

set RESTOREDIR=%~dp0..\restore
set TOPDIR=%~dp0..

rem By using a token that does not exist in paths, this will resolve any ".." and "." in the path, even if path contains spaces
FOR /F "tokens=*" %%I IN ("%RESTOREDIR%") DO set RESTOREDIR=%%~fI
FOR /F "tokens=*" %%I IN ("%BINDIR%") DO set BINDIR=%%~fI
FOR /F "tokens=*" %%I IN ("%TOPDIR%") DO set TOPDIR=%%~fI

if /i "%GOTOHELP%" == "yes" goto :help
GOTO :start


:help

echo.
echo Installs or restores F# SDK bits, which applies system-wide to all Visual Studio
echo 2017 installations. After running this, each project targeting F# 4.5 will use
echo your locally built FSC.exe. It will not update other F# tools, see remarks below.
echo.
echo Requires Administrator privileges for removing/restoring strong-naming.
echo.
echo Syntax: %0 [debug^|release^|restore^|backup]
echo.
echo   debug     integrates debug builds of FSC, FSI ^& tools
echo   release   integrates release builds of FSC, FSI ^& tools
echo   restore   restores original SDK from an earlier backup
echo   backup    backups the files that would be overwritten, does not deploy anything
echo.
echo Paths used:
echo.
echo Root location:        %TOPDIR%
echo Debug bin location:   %TOPDIR%\debug\net40\bin
echo Release bin location: %TOPDIR%\release\net40\bin
echo Backup location:      %RESTOREDIR%
echo.
echo Remarks:
echo.
echo This script should only be run after build.cmd has completed successfully.
echo.
echo Clearing the git repository may clear the backup directory. To be on the safe
echo side, you should place a copy of the backup dir outside of the git repo.
echo.
echo This batch script will only update the relevant SDK bits, and remove or restore
echo strong-naming automatically. It is recommended that you also update the F# Tools
echo by running the following two commands after a build of "build vs" or
echo "build vs debug" has completed. More instructions in DEVGUIDE.md in the root.
echo.
echo For Release builds:
echo.
echo ^> VSIXInstaller.exe /u:"VisualFSharp"
echo ^> VSIXInstaller.exe release\net40\bin\VisualFSharpFull.vsix
echo.
echo For Debug builds:
echo.
echo ^> VSIXInstaller.exe /u:"VisualFSharp"
echo ^> VSIXInstaller.exe debug\net40\bin\VisualFSharpFull.vsix
echo.

exit /b 1

:start

echo.
if "%DEPLOY%" == "yes" echo Starting deployment of %ACTION% bits.
if not "%DEPLOY%" == "yes" echo Starting %ACTION%
echo.

rem This check whether we're started with administrator rights
CALL :checkPrequisites

if /i "%PROCESSOR_ARCHITECTURE%"=="x86" set X86_PROGRAMFILES=%ProgramFiles%
if /I "%PROCESSOR_ARCHITECTURE%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

rem See https://stackoverflow.com/a/17113667/111575 on 2^>NUL for suppressing the error "ERROR: The system was unable to find the specified registry key or value." from reg.exe, this fixes #3619
rem The delims are a TAB and a SPACE, do not normalize it!
                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6.2\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6.1\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder 2^>NUL') DO SET WINSDKNETFXTOOLS=%%B

set SN32="%WINSDKNETFXTOOLS%sn.exe"
set SN64="%WINSDKNETFXTOOLS%x64\sn.exe"
set NGEN32=%windir%\Microsoft.NET\Framework\v4.0.30319\ngen.exe
set NGEN64=%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe

set FSHARPVERSION=4.3
set FSHARPVERSION2=43

rem The various locations of the SDK and tools

rem SDK path, will be created if it doesn't exist
set COMPILERSDKPATH=%X86_PROGRAMFILES%\Microsoft SDKs\F#\%FSHARPVERSION%\Framework\v4.0

rem Main assemblies path, will be created if it doesn't exist
set COMPILERMAINASSEMBLIESPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.%FSHARPVERSION%.0

rem The .NET Core 3.7 assemblies path, will be created if it doesn't exist
set COMPILER7ASSEMBLIESPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.%FSHARPVERSION2%.0

rem The .NET Core 3.78 assemblies path, will be created if it doesn't exist
set COMPILER78ASSEMBLIESPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.%FSHARPVERSION2%.0

rem The .NET Core 3.259 assemblies path, will be created if it doesn't exist
set COMPILER259ASSEMBLIESPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.%FSHARPVERSION2%.0

rem The .NET Portable 3.47 assemblies path, will be created if it doesn't exist
set COMPILER47ASSEMBLIESPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.%FSHARPVERSION2%.0

rem Try to create target and backup folders, if needed
set RESTOREBASE=%RESTOREDIR%

rem Only create backup dirs if we are backupping or restoring 
rem (in the latter case, the directories should already be there, but if not, it prevents errors later on)
if "!DEPLOY!" == "no" (
    CALL :tryCreateFolder "!RESTOREBASE!\compiler_sdk"
    CALL :tryCreateFolder "!RESTOREBASE!\main_assemblies"
    CALL :tryCreateFolder "!RESTOREBASE!\profile_7"
    CALL :tryCreateFolder "!RESTOREBASE!\profile_78"
    CALL :tryCreateFolder "!RESTOREBASE!\profile_259"
    CALL :tryCreateFolder "!RESTOREBASE!\profile_47"
)
CALL :tryCreateFolder "!COMPILERSDKPATH!"
CALL :tryCreateFolder "!COMPILERMAINASSEMBLIESPATH!"
CALL :tryCreateFolder "!COMPILER7ASSEMBLIESPATH!" & 
CALL :tryCreateFolder "!COMPILER78ASSEMBLIESPATH!"
CALL :tryCreateFolder "!COMPILER259ASSEMBLIESPATH!"
CALL :tryCreateFolder "!COMPILER47ASSEMBLIESPATH!"

rem If one or more directories could not be created, exit early with a non-zero error code
if "!CREATEFAILED!"=="true" CALL :exitFailDir & EXIT /B 1

rem Deploying main files, fsi.exe and fsc.exe and related

echo.
CALL :colorEcho 02 "[!ACTION!] Processing files for compiler_sdk" & echo.

set SOURCEDIR=%BINDIR%
set RESTOREDIR=!RESTOREBASE!\compiler_sdk
CALL :checkAvailability compiler_sdk
if "!BIN_AVAILABLE!" == "true" (
    CALL :backupAndOrCopy fsc.exe "!COMPILERSDKPATH!"
    CALL :backupAndOrCopy fsc.exe.config "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy FSharp.Build.dll "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy FSharp.Compiler.Service.dll "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy FSharp.Compiler.Interactive.Settings.dll "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy fsi.exe "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy fsi.exe.config "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy fsiAnyCpu.exe "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy fsiAnyCpu.exe.config "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy Microsoft.FSharp.Targets "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy Microsoft.Portable.FSharp.Targets "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy Microsoft.FSharp.NetSdk.props "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy Microsoft.FSharp.NetSdk.targets "%COMPILERSDKPATH%"
    CALL :backupAndOrCopy Microsoft.FSharp.Overrides.NetSdk.targets "%COMPILERSDKPATH%"

    rem Special casing for SupportedRuntimes.xml, it has a different source directory, it's always there
    set SOURCEDIR="%TOPDIR%\vsintegration\src\SupportedRuntimes"
    CALL :backupAndOrCopy SupportedRuntimes.xml "%COMPILERSDKPATH%"
)



rem Deploying main assemblies

echo.
CALL :colorEcho 02 "[!ACTION!] Processing files for main_assemblies" & echo.

set SOURCEDIR=%BINDIR%
set RESTOREDIR=!RESTOREBASE!\main_assemblies
CALL :checkAvailability main_assemblies
if "!BIN_AVAILABLE!" == "true" (
    CALL :backupAndOrCopy FSharp.Core.dll "%COMPILERMAINASSEMBLIESPATH%"
    CALL :backupAndOrCopy FSharp.Core.xml "%COMPILERMAINASSEMBLIESPATH%"
)

 

REM TODO: this was already here (2017-09-28) and was already commented out, I think (AB) that these redirects aren't necessary anymore and can be permanently removed
REM echo ^<configuration^>^<runtime^>^<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1" appliesTo="v4.0.30319"^>^<dependentAssembly^>^<assemblyIdentity name="FSharp.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" /^> ^<bindingRedirect oldVersion="2.0.0.0-4.%FSHARPVERSION%.0" newVersion="4.%FSHARPVERSION%.0"/^>^</dependentAssembly^>^</assemblyBinding^>^</runtime^>^</configuration^> > "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.%FSHARPVERSION%.0\pub.config"

rem To add registry keys and to change strong-name validation requires Administrator access 

if "%DEPLOY%" == "yes" if "!ISADMIN!" == "yes" (
    echo.
    CALL :colorEcho 02 "[!ACTION!] Setting or adding registry keys for open source assemblies" & echo.
    if /I "!PROCESSOR_ARCHITECTURE!"=="AMD64" (
        REG ADD "HKLM\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\F# !FSHARPVERSION! Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.!FSHARPVERSION!.0\
        REG ADD "HKLM\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.50709\AssemblyFoldersEx\F# !FSHARPVERSION! Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.!FSHARPVERSION!.0\
    )
    REG ADD "HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\F# !FSHARPVERSION! Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.!FSHARPVERSION!.0\
    REG ADD "HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.50709\AssemblyFoldersEx\F# !FSHARPVERSION! Core Assemblies (Open Source)" /ve /t REG_SZ /f /d "!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.!FSHARPVERSION!.0\

    rem Disable strong-name validation for F# binaries built from open source that are signed with the microsoft key
    echo.
    CALL :colorEcho 02 "[!ACTION!] Removing strong-name validation of F# binaries" & echo.
    !SN32! -Vr FSharp.Core,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.Build,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr HostedCompilerServer,b03f5f7f11d50a3a 1>NUL 2>NUL

    !SN32! -Vr FSharp.Compiler,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.Editor,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.LanguageService,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr FSharp.VS.FSI,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr VisualFSharp.UnitTests,b03f5f7f11d50a3a 1>NUL 2>NUL
    !SN32! -Vr VisualFSharp.Salsa,b03f5f7f11d50a3a 1>NUL 2>NUL

    REM Do this *in addition* to the above for x64 systems
    if /i "!PROCESSOR_ARCHITECTURE!"=="AMD64" (
        !SN64! -Vr FSharp.Core,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.Build,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr HostedCompilerServer,b03f5f7f11d50a3a 1>NUL 2>NUL

        !SN64! -Vr FSharp.Compiler,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.Editor,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.LanguageService,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.LanguageService.Base,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.ProjectSystem.Base,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr FSharp.VS.FSI,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr VisualFSharp.UnitTests,b03f5f7f11d50a3a 1>NUL 2>NUL
        !SN64! -Vr VisualFSharp.Salsa,b03f5f7f11d50a3a 1>NUL 2>NUL
    )

    rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
    
    echo.
    CALL :colorEcho 02 "[!ACTION!] Queuing for NGEN of FSI and FSC binaries" & echo.
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsc.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsc.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsi.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsi.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsiAnyCpu.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsiAnyCpu.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\FSharp.Build.dll"
    "!NGEN32!" install "!COMPILERSDKPATH!\FSharp.Build.dll" /queue:1 1>NUL
    
    if /i "!PROCESSOR_ARCHITECTURE!"=="AMD64" (
        echo [!ACTION!] NGEN64 of "!COMPILERSDKPATH!\fsiAnyCpu.exe"
        "!NGEN64!" install "!COMPILERSDKPATH!\fsiAnyCpu.exe" /queue:1 1>NUL
        echo [!ACTION!] NGEN64 of "!COMPILERSDKPATH!\FSharp.Build.dll"
        "!NGEN64!" install "!COMPILERSDKPATH!\FSharp.Build.dll" /queue:1 1>NUL
    )
)

if "%DEPLOY%" == "yes" if "!ISADMIN!" == "no" (
    echo.
    CALL :colorEcho 0E "[!ACTION!] SKIPPED (no admin) Setting or adding registry keys for open source assemblies" & echo.
    CALL :colorEcho 0E "[!ACTION!] SKIPPED (no admin) Removing strong-name validation of F# binaries" & echo.
    CALL :colorEcho 02 "[!ACTION!] SKIPPED (no admin) Queuing for NGEN of FSI and FSC binaries" & echo.
    SET /A WARNCOUNT+=3
)

rem Re-enable certain settings when restoring, NGEN the original files again, requires admin rights
if "%ACTION%" == "restore" if "!ISADMIN!" == "yes" (

    rem Re-enable strong-name validation for F# binaries that were previously installed
    echo.
    CALL :colorEcho 02 "[!ACTION!] Re-enabling strong-name validation of original F# binaries" & echo.
    !SN32! -Vu FSharp.Core,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.Build,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu HostedCompilerServer,b03f5f7f11d50a3a 2>NUL 1>NUL

    !SN32! -Vu FSharp.Compiler,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.Editor,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.LanguageService,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.LanguageService.Base,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.ProjectSystem.Base,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu FSharp.VS.FSI,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu VisualFSharp.UnitTests,b03f5f7f11d50a3a 2>NUL 1>NUL
    !SN32! -Vu VisualFSharp.Salsa,b03f5f7f11d50a3a 2>NUL 1>NUL

    REM Do this *in addition* to the above for x64 systems
    if /i "!PROCESSOR_ARCHITECTURE!"=="AMD64" (
        !SN64! -Vu FSharp.Core,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.Build,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu HostedCompilerServer,b03f5f7f11d50a3a 2>NUL 1>NUL

        !SN64! -Vu FSharp.Compiler,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.Editor,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.LanguageService,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.LanguageService.Base,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.ProjectSystem.Base,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu FSharp.VS.FSI,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu VisualFSharp.UnitTests,b03f5f7f11d50a3a 2>NUL 1>NUL
        !SN64! -Vu VisualFSharp.Salsa,b03f5f7f11d50a3a 2>NUL 1>NUL
    )

    rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
    
    echo.
    CALL :colorEcho 02 "[!ACTION!] Queuing for NGEN of FSI and FSC binaries" & echo.
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsc.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsc.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsi.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsi.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\fsiAnyCpu.exe"
    "!NGEN32!" install "!COMPILERSDKPATH!\fsiAnyCpu.exe" /queue:1 1>NUL
    echo [!ACTION!] NGEN of "!COMPILERSDKPATH!\FSharp.Build.dll"
    "!NGEN32!" install "!COMPILERSDKPATH!\FSharp.Build.dll" /queue:1 1>NUL
    
    if /i "!PROCESSOR_ARCHITECTURE!"=="AMD64" (
        echo [!ACTION!] NGEN64 of "!COMPILERSDKPATH!\fsiAnyCpu.exe"
        "!NGEN64!" install "!COMPILERSDKPATH!\fsiAnyCpu.exe" /queue:1 1>NUL
        echo [!ACTION!] NGEN64 of "!COMPILERSDKPATH!\FSharp.Build.dll"
        "!NGEN64!" install "!COMPILERSDKPATH!\FSharp.Build.dll" /queue:1 1>NUL
    )
)

if "%ACTION%" == "restore" if "!ISADMIN!" == "no" (
    CALL :colorEcho 0E "[!ACTION!] SKIPPED (no admin) Re-enabling strong-name validation of original F# binaries" & echo.
    CALL :colorEcho 0E "[!ACTION!] SKIPPED (no admin) Queuing for NGEN of FSI and FSC binaries" & echo.
    set /A WARNCOUNT+=2
)
GOTO :summary

:checkAvailability
rem Checks whether a given source is available, issues a warning otherwise, SOURCEDIR must be set to the appropriate binaries

rem This will simultaneously remove the quotes of the original param and add the filename to it, then it is surrounded by quotes again
FOR /F "usebackq tokens=*" %%I IN ('%SOURCEDIR%')  DO set SOURCE="%%~fI\*"
if not exist !SOURCE! (
    rem For debug and release deploy it matters, but for restore and backup we don't care
    set BIN_AVAILABLE=true
    if "!DEPLOY!" == "yes" (
        echo [!ACTION!] Source bindir does not exist: !SOURCE!
        CALL :colorEcho 0E "[!ACTION!] Source binaries not found, deploy of %1 skipped" & echo. & set /A WARNCOUNT+=1
        set BIN_AVAILABLE=false
    )
    
) else (
    set BIN_AVAILABLE=true
)

EXIT /B

  
:backupAndOrCopy
rem Creates a backup and copies, depending on whether debug, release, restore or backup is selected

rem This will simultaneously remove the quotes of the original param and add the filename to it, then it is surrounded by quotes again
FOR /F "usebackq tokens=*" %%I IN ('%2')           DO set TARGET="%%~fI\%1"
FOR /F "usebackq tokens=*" %%I IN ('%RESTOREDIR%') DO set BACKUP="%%~fI\%1"
FOR /F "usebackq tokens=*" %%I IN ('%SOURCEDIR%')  DO set SOURCE="%%~fI\%1"

if "%ACTION%" == "backup" (
    rem When backing up, the target becomes the source
    
    if not exist !TARGET! (
        rem Remove a file from the backup location if it is not part of this SDK install
        DEL /f !BACKUP! 1>NUL 2>NUL
    ) else (
        rem Otherwise, copy over the original
        CALL :copyFile !TARGET! !BACKUP!
    )
)

if "%ACTION%" == "restore" (
    rem When restoring, the backup location becomes the source
    
    if not exist !BACKUP! (
        rem If this file didn't exist in the previous installation, we should remove it to prevent confusion of left-over bits
        DEL /f !TARGET! 1>NUL 2>NUL
    ) else (
        rem Otherwise, copy over the original
        CALL :copyFile !BACKUP! !TARGET!
    )
)

if "%DEPLOY%" == "yes" (
    rem Deploy of debug or release build, depending on selected action
    CALL :copyFile !SOURCE! !TARGET!
)


EXIT /B

rem Copies a file and logs errors in red, warnings in yellow
:copyFile 
FOR /F "usebackq tokens=*" %%I IN ('%1') DO set SOURCE="%%~fI"
FOR /F "usebackq tokens=*" %%I IN ('%2') DO set TARGET="%%~fI"

echo [%ACTION%] source: !SOURCE!
echo [%ACTION%] target: !TARGET!
if EXIST !SOURCE! (
    copy /y !SOURCE! !TARGET! 1>NUL 2>copyresult.log
    if "!errorlevel!" == "0" echo [!ACTION!] 1 file copied & set /A COPYCOUNT+=1
    if not "!errorlevel!" == "0" (
        set /p COPYRESULT=<copyresult.log 
        CALL :colorEcho 0C "[!ACTION!] !COPYRESULT!"  & echo.
        

        rem No admin right needed to *read* from program-files, but admin rights usually required to write to program-files.
        if "!ISADMIN!" == "no" (
            if not "!ACTION!" == "backup" (
                CALL :colorEcho 0C "[!ACTION!] No admin rights detected, try again with elevated permissions (run as Administrator)" & echo. & set /A ERRORCOUNT+=1
            )
        ) else (
            CALL :colorEcho 0C "[!ACTION!] Failed copying this file; make sure the file is not in use and is not read-only, system or hidden." & echo. & set /A ERRORCOUNT+=1
        )
    )
    del copyresult.log 2>nul
    set COPYRESULT=
) else (
    if "%ACTION%" == "backup"  CALL [backup] File not found, nothing to backup
    if "%ACTION%" == "restore" CALL :colorEcho 0E "[restore] File not found, not able to restore, possibly it didn't exist originally" & echo. & set /A WARNCOUNT+=1
    if "%DEPLOY%" == "yes"     CALL :colorEcho 0C "[!ACTION!] File not found, not able to deploy" & echo. & set /A ERRORCOUNT+=1
)

EXIT /B

rem Creates a folder, if it already exists, it will do nothing, if there's an access-denied, it will set %CREATEFAILED% to true
:tryCreateFolder

rem Add a backslash safely, by taking care of auxiliary quotes
FOR /F "usebackq tokens=*" %%I IN ('%1') DO set FOLDER_TO_BE_CREATED="%%~fI\"

if not exist !FOLDER_TO_BE_CREATED! (
    mkdir !FOLDER_TO_BE_CREATED! 2>NUL
    if "!errorlevel!" EQU "0" (
        echo [!ACTION!] Created directory !FOLDER_TO_BE_CREATED!
    ) else (
        set CREATEFAILED=true
        echo Failed to create %1
        CALL :colorEcho 0C "Could not create directory, check access rights or whether a file with that name exists "
        echo.
        echo.
    )
) 

EXIT /B

:summary

echo.
if not "%ACTION%" == "restore" if not "%ACTION%" == "backup" echo Finished installing F# SDK and other bits. The following directories were updated and & echo a backup is written to %RESTOREDIR%.
if "%ACTION%" == "restore" echo Finished restoring original F# SDK and other bits. The following directories were used while & echo restoring a backup from %RESTOREDIR%.
if "%ACTION%" == "backup" echo Finished creating a backup in %RESTOREBASE%.

echo.
echo Root location:         %TOPDIR%
if "!ACTION!" == "debug"    echo Debug bin location:    %TOPDIR%\debug\net40\bin
if "!ACTION!" == "release"  echo Release bin location:  %TOPDIR%\release\net40\bin
if "!DEPLOY!" == "no"       echo Backup location:       %RESTOREBASE%
echo.
echo Target locations used:
echo.
echo Win SDK tools:               %WINSDKNETFXTOOLS%
echo Compiler SDK path:           %COMPILERSDKPATH%
echo F# compiler main assemblies: %COMPILERMAINASSEMBLIESPATH%
echo Portable profile 7:          %COMPILER7ASSEMBLIESPATH%
echo Portable profile 78:         %COMPILER78ASSEMBLIESPATH%
echo Portable profile 259:        %COMPILER259ASSEMBLIESPATH%
echo Portable profile 47:         %COMPILER47ASSEMBLIESPATH%
echo.

rem Display success, warning, error counts

if "%ACTION%" == "backup"   SET VERB=backed up
if "%ACTION%" == "restore"  SET VERB=restored
if "%DEPLOY%" == "yes"      SET VERB=deployed
CALL :colorEcho 0A "A total of %COPYCOUNT% file(s) were %VERB%." & echo.

if %ERRORCOUNT% equ 1 CALL :colorEcho 0C "%ERRORCOUNT% error reported, see log" & echo.
if %ERRORCOUNT% gtr 1 CALL :colorEcho 0C "%ERRORCOUNT% errors reported, see log" & echo.
if %ERRORCOUNT% equ 0 CALL :colorEcho 0A "No errors reported" & echo.

if %WARNCOUNT% equ 1 CALL :colorEcho 0E "%WARNCOUNT% warning reported, see log" & echo.
if %WARNCOUNT% gtr 1 CALL :colorEcho 0E "%WARNCOUNT% warnings reported, see log" & echo.
if %WARNCOUNT% equ 0 CALL :colorEcho 0A "No warnings reported" & echo.

rem Return non-zero error code for use-cases where this script is called from other scripts
if %ERRORCOUNT% gtr 0 EXIT /B 1
EXIT /B 0

GOTO :EOF

:exitFailDir

echo.
CALL :colorEcho 0C "One or more directories failed to be created. No files have been copied." & echo.
echo.
echo Possible causes include:
echo - Insufficient rights to create directories in this folder
echo - A file with that name already exists
echo.
echo No error is raised if the directory exists.
echo No files were copied or backed up.
echo.

rem Return non-zero error code for use-cases where this script is called from other scripts
EXIT /B 1

:checkPrequisites
rem Whether or not we have administrator rights

SET ISADMIN=yes

rem The error level of NET SESSION is set to 2 when you don't have administrator rights, simplest hack
net sessions 1>NUL 2>NUL
if %ERRORLEVEL% GTR 0 (
    SET ISADMIN=no
    CALL :colorEcho 0E "[!ACTION!] Started without administrator access, strong-naming will not be adjusted, reg-keys not changed" & echo.    
    SET /A WARNCOUNT+=1
)

EXIT /B


rem See: https://stackoverflow.com/a/21666354/111575
rem Prevent accidentally entering the colorEcho label
GOTO :EOF
:colorEcho
<nul set /p ".=%DEL%" > "%~2"
findstr /v /a:%1 /R "^$" "%~2" nul
del "%~2" > nul 2>&1i