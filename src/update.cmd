@rem ===========================================================================================================
@rem Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, 
@rem               Version 2.0.  See License.txt in the project root for license information.
@rem ===========================================================================================================

@echo off
setlocal

if /i "%1" == "debug" goto :ok
if /i "%1" == "release" goto :ok
if /i "%1" == "vsdebug" goto :ok
if /i "%1" == "vsrelease" goto :ok

echo GACs built binaries, adds required strong name verification skipping, and optionally NGens built binaries
echo Usage:
echo    update.cmd debug   [-ngen]
echo    update.cmd release [-ngen]
echo    update.cmd vsdebug [-ngen]
echo    update.cmd vsrelease [-ngen]
exit /b 1

:ok

set BINDIR=%~dp0..\%1\net40\bin
set FAKESIGN=".\packages\FakeSign.0.9.2\tools\FakeSign.exe"

rem Fakesign the path to these dlls.
rem the location of these should be %BINDIR\
%FAKESIGN% %BINDIR%\FSharp.Core,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.Build,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.Compiler.Hosted,b03f5f7f11d50a3a

%FAKESIGN% %BINDIR%\FSharp.Compiler,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.Editor,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.LanguageService,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.LanguageService.Base,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\FSharp.VS.FSI,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\Unittests,b03f5f7f11d50a3a
%FAKESIGN% %BINDIR%\Salsa,b03f5f7f11d50a3a

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    %FAKESIGN% %BINDIR%\FSharp.Core,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.Build,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.Compiler.Interactive.Settings,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.Compiler.Hosted,b03f5f7f11d50a3a

    %FAKESIGN% %BINDIR%\FSharp.Compiler,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.Compiler.Server.Shared,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.Editor,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.LanguageService,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.LanguageService.Base,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.LanguageService.Compiler,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.ProjectSystem.Base,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.ProjectSystem.FSharp,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.ProjectSystem.PropertyPages,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\FSharp.VS.FSI,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\Unittests,b03f5f7f11d50a3a
    %FAKESIGN% %BINDIR%\Salsa,b03f5f7f11d50a3a
)

rem
rem NGen fsc, fsi, fsiAnyCpu, and FSharp.Build.dll
if /i not "%2"=="-ngen" goto :donengen

"%NGEN32%" install "%BINDIR%\fsc.exe" /queue:1
"%NGEN32%" install "%BINDIR%\fsi.exe" /queue:1
"%NGEN32%" install "%BINDIR%\FSharp.Build.dll" /queue:1
"%NGEN32%" executeQueuedItems 1

if /i "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%NGEN64%" install "%BINDIR%\fsiAnyCpu.exe" /queue:1
    "%NGEN64%" install "%BINDIR%\FSharp.Build.dll" /queue:1
    "%NGEN64%" executeQueuedItems 1
)

:donengen
