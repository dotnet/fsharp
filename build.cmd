rem Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
@if "%_echo%"=="" echo off 

setlocal enableDelayedExpansion

:ARGUMENTS_VALIDATION

if /I "%1" == "--help"   (goto :USAGE)
if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)
goto :ARGUMENTS_OK

:USAGE

echo Build and run a subset of test suites
echo.
echo Usage:
echo.
echo build.cmd ^<all^|net40^|coreclr^|pcls^|vs^>
echo           ^<proto^|protofx^>
echo           ^<ci^|ci_part1^|ci_part2^|ci_part3^|microbuild^>
echo           ^<debug^|release^>
echo           ^<diag^|publicsign^>
echo           ^<test^|test-net40-coreunit^|test-coreclr-coreunit^|test-compiler-unit^|test-pcl-coreunit^|test-net40-fsharp^|test-coreclr-fsharp^|test-net40-fsharpqa^>
echo           ^<include tag^>
echo           ^<init^>
echo.
echo No arguments default to 'default', meaning this (no testing)
echo.
echo     build.cmd net40 
echo.
echo.Other examples:
echo.
echo.    build.cmd net40            (build compiler for .NET Framework)
echo.    build.cmd coreclr          (build compiler for .NET Core)
echo.    build.cmd vs               (build Visual Studio IDE Tools)
echo.    build.cmd all              (build everything)
echo.    build.cmd test             (build and test default targets)
echo.    build.cmd net40 test       (build and test net40)
echo.    build.cmd coreclr test     (build and test net40)
echo.    build.cmd vs test          (build and test net40)
echo.    build.cmd all test         (build and test net40)
echo.    build.cmd nobuild test include Conformance (run only tests marked with Conformance category)
echo.    build.cmd nobuild test include Expensive (run only tests marked with Expensive category)
echo.
goto :success

:ARGUMENTS_OK

rem disable setup build by setting FSC_BUILD_SETUP=0
if /i '%FSC_BUILD_SETUP%' == '' (set FSC_BUILD_SETUP=1) 

rem by default don't build coreclr lkg.  However allow configuration by setting an environment variable : set BUILD_PROTO_WITH_CORECLR_LKG = 1
if '%BUILD_PROTO_WITH_CORECLR_LKG%' =='' (set BUILD_PROTO_WITH_CORECLR_LKG=0) 

set BUILD_PROTO=0
set BUILD_PHASE=1
set BUILD_NET40=0
set BUILD_CORECLR=0
set BUILD_PORTABLE=0
set BUILD_VS=0
set BUILD_CONFIG=release
set BUILD_CONFIG_LOWERCASE=release
set BUILD_DIAG=
set BUILD_PUBLICSIGN=0

set TEST_NET40_COMPILERUNIT_SUITE=0
set TEST_NET40_COREUNIT_SUITE=0
set TEST_NET40_FSHARP_SUITE=0
set TEST_NET40_FSHARPQA_SUITE=0
set TEST_CORECLR_COREUNIT_SUITE=0
set TEST_CORECLR_FSHARP_SUITE=0
set TEST_PORTABLE_COREUNIT_SUITE=0
set TEST_VS_IDEUNIT_SUITE=0
set INCLUDE_TEST_SPEC_NUNIT=
set INCLUDE_TEST_TAGS=

set PUBLISH_VSIX=0
set MYGET_APIKEY=


REM ------------------ Parse all arguments -----------------------

set _autoselect=1
set _autoselect_tests=0
set /a counter=0
for /l %%x in (1 1 9) do (
    set /a counter=!counter!+1
    set /a nextcounter=!counter!+1
    call :PROCESS_ARG %%!counter! %%!nextcounter! "!counter!"
)
for %%i in (%BUILD_FSC_DEFAULT%) do ( call :PROCESS_ARG %%i )

REM apply defaults

if /i '%_autoselect%' == '1' (
    set BUILD_NET40=1
)

if /i '%_autoselect_tests%' == '1' (
    if /i '%BUILD_NET40%' == '1' (
        set TEST_NET40_COMPILERUNIT_SUITE=1
        set TEST_NET40_COREUNIT_SUITE=1
        set TEST_NET40_FSHARP_SUITE=1
        set TEST_NET40_FSHARPQA_SUITE=1
    )

    if /i '%BUILD_CORECLR%' == '1' (
        set TEST_CORECLR_FSHARP_SUITE=1
        set TEST_CORECLR_COREUNIT_SUITE=1
    )

    if /i '%BUILD_PORTABLE%' == '1' (
        set TEST_PORTABLE_COREUNIT_SUITE=1
    )

    if /i '%BUILD_VS%' == '1' (
        set TEST_VS_IDEUNIT_SUITE=1
    )
)

goto :MAIN

REM ------------------ Procedure to parse one argument -----------------------

:PROCESS_ARG
set ARG=%~1
set ARG2=%~2
if "%ARG%" == "1" if "%2" == "" (set ARG=default)
if "%2" == "" if not "%ARG%" == "default" goto :EOF

if /i '%ARG%' == 'net40' (
    set _autoselect=0
    set BUILD_NET40=1
)

if /i '%ARG%' == 'coreclr' (
    set _autoselect=0
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
)

if /i '%ARG%' == 'pcls' (
    set _autoselect=0
    set BUILD_PORTABLE=1
)

if /i '%ARG%' == 'vs' (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_VS=1
)

if /i '%ARG%' == 'vstest' (
    set TEST_VS_IDEUNIT_SUITE=1
)

if /i '%ARG%' == 'nobuild' (
    set BUILD_PHASE=0
)
if /i '%ARG%' == 'all' (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set CI=1
)

if /i '%ARG%' == 'microbuild' (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%

    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=0
    set TEST_CORECLR_FSHARP_SUITE=0
    set TEST_PORTABLE_COREUNIT_SUITE=1
    set TEST_VS_IDEUNIT_SUITE=1
    set CI=1
    set PUBLISH_VSIX=1
)

REM These divide 'ci' into two chunks which can be done in parallel
if /i '%ARG%' == 'ci_part1' (
    set _autoselect=0

    REM what we do
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_VS_IDEUNIT_SUITE=1
    set CI=1
)

if /i '%ARG%' == 'ci_part2' (
    set _autoselect=0

    REM what we do
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_PORTABLE=1

    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_PORTABLE_COREUNIT_SUITE=1
    set CI=1

)

if /i '%ARG%' == 'ci_part3' (
    set _autoselect=0

    REM what we do
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_PROTO=1
    set BUILD_CORECLR=1

    set TEST_CORECLR_FSHARP_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=1
    set CI=1

)

if /i '%ARG%' == 'proto' (
    set BUILD_PROTO=1
)

if /i '%ARG%' == 'diag' (
    set BUILD_DIAG=/v:detailed
    if not defined APPVEYOR ( set BUILD_LOG=fsharp_build_log.log )
)

if /i '%ARG%' == 'debug' (
    set BUILD_CONFIG=debug
)

if /i '%ARG%' == 'release' (
    set BUILD_CONFIG=release
)

if /i '%ARG%' == 'test' (
    set _autoselect_tests=1
)

if /i '%ARG%' == 'include' (
    set /a counter=!counter!+1
    if '!INCLUDE_TEST_SPEC_NUNIT!' == '' ( set INCLUDE_TEST_SPEC_NUNIT=cat == %ARG2% ) else (set INCLUDE_TEST_SPEC_NUNIT=cat == %ARG2% or !INCLUDE_TEST_SPEC_NUNIT! )
    if '!INCLUDE_TEST_TAGS!' == '' ( set INCLUDE_TEST_TAGS=%ARG2% ) else (set INCLUDE_TEST_TAGS=%ARG2%;!INCLUDE_TEST_TAGS! )
)

if /i '%ARG%' == 'test-all' (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%

    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_PORTABLE_COREUNIT_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=1
    set TEST_VS_IDEUNIT_SUITE=1
)

if /i '%ARG%' == 'test-net40-fsharpqa' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_NET40_FSHARPQA_SUITE=1
)

if /i '%ARG%' == 'test-compiler-unit' (
    set BUILD_NET40=1
    set TEST_NET40_COMPILERUNIT_SUITE=1
)

if /i '%ARG%' == 'test-net40-coreunit' (
    set BUILD_NET40=1
    set TEST_NET40_COREUNIT_SUITE=1
)

if /i '%ARG%' == 'test-coreclr-coreunit' (
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set TEST_CORECLR_COREUNIT_SUITE=1
)

if /i '%ARG%' == 'test-pcl-coreunit' (
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT_SUITE=1
)

if /i '%ARG%' == 'test-net40-fsharp' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_NET40_FSHARP_SUITE=1
)

if /i '%ARG%' == 'test-coreclr-fsharp' (
    set BUILD_NET40=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set TEST_CORECLR_FSHARP_SUITE=1
)

if /i '%ARG%' == 'publicsign' (
    set BUILD_PUBLICSIGN=1
)

if /i '%ARG%' == 'init' (
    set BUILD_PROTO_WITH_CORECLR_LKG=1
)

if /i [%ARG:~0,13%] == [MYGET_APIKEY:] (
    set MYGET_APIKEY=%ARG:~13%
)

goto :EOF
:: Note: "goto :EOF" returns from an in-batchfile "call" command
:: in preference to returning from the entire batch file.


REM ------------------ Report config -----------------------

:MAIN

REM after this point, ARG variable should not be used, use only BUILD_* or TEST_*

echo Build/Tests configuration:
echo.
echo BUILD_PROTO=%BUILD_PROTO%
echo BUILD_PROTO_WITH_CORECLR_LKG=%BUILD_PROTO_WITH_CORECLR_LKG%
echo BUILD_NET40=%BUILD_NET40%
echo BUILD_CORECLR=%BUILD_CORECLR%
echo BUILD_PORTABLE=%BUILD_PORTABLE%
echo BUILD_VS=%BUILD_VS%
echo BUILD_SETUP=%BUILD_SETUP%
echo BUILD_CONFIG=%BUILD_CONFIG%
echo BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%
echo.
echo TEST_NET40_COMPILERUNIT_SUITE=%TEST_NET40_COMPILERUNIT_SUITE%
echo TEST_NET40_COREUNIT_SUITE=%TEST_NET40_COREUNIT_SUITE%
echo TEST_NET40_FSHARP_SUITE=%TEST_NET40_FSHARP_SUITE%
echo TEST_NET40_FSHARPQA_SUITE=%TEST_NET40_FSHARPQA_SUITE%
echo TEST_CORECLR_COREUNIT_SUITE=%TEST_CORECLR_COREUNIT_SUITE%
echo TEST_CORECLR_FSHARP_SUITE=%TEST_CORECLR_FSHARP_SUITE%
echo TEST_PORTABLE_COREUNIT_SUITE=%TEST_PORTABLE_COREUNIT_SUITE%
echo TEST_VS_IDEUNIT_SUITE=%TEST_VS_IDEUNIT_SUITE%
echo INCLUDE_TEST_SPEC_NUNIT=%INCLUDE_TEST_SPEC_NUNIT%
echo INCLUDE_TEST_TAGS=%INCLUDE_TEST_TAGS%
echo PUBLISH_VSIX=%PUBLISH_VSIX%
echo MYGET_APIKEY=%MYGET_APIKEY%


echo .
echo Environment
echo 
set
echo .
echo .

echo ---------------- Done with arguments, starting preparation -----------------
set BuildToolsPackage=Microsoft.VSSDK.BuildTools.15.0.26124-RC3
if '%VSSDKInstall%'=='' (
     set VSSDKInstall=%~dp0packages\%BuildToolsPackage%\tools\vssdk
)
if '%VSSDKToolsPath%'=='' (
     set VSSDKToolsPath=%~dp0packages\%BuildToolsPackage%\tools\vssdk\bin
)
if '%VSSDKIncludes%'=='' (
     set VSSDKIncludes=%~dp0packages\%BuildToolsPackage%\tools\vssdk\inc
)

if '%RestorePackages%'=='' (
    set RestorePackages=true
)

@echo VSSDKInstall:   %VSSDKInstall%
@echo VSSDKToolsPath: %VSSDKToolsPath%
@echo VSSDKIncludes:  %VSSDKIncludes%

@echo on
@call src\update.cmd signonly

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS150COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=15.0
if not '%VisualStudioVersion%' == '' goto vsversionset

if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS150COMNTOOLS%\..\..\ide\devenv.exe" set VisualStudioVersion=15.0
if not '%VisualStudioVersion%' == '' goto vsversionset

if exist "%VS140COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset

if exist "%VS120COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0

:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find an installation of Visual Studio && goto :failure

if exist "%VS150COMNTOOLS%\..\..\MSBuild\15.0\Bin\MSBuild.exe" (
    set _msbuildexe="%VS150COMNTOOLS%\..\..\MSBuild\15.0\Bin\MSBuild.exe"
    goto :havemsbuild
)
if exist "%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" (
    set _msbuildexe="%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
    goto :havemsbuild
)
if exist "%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" (
    set _msbuildexe="%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
    goto :havemsbuild
)
echo Error: Could not find MSBuild.exe. && goto :failure
goto :eof

:havemsbuild
set _nrswitch=/nr:false

rem uncomment to use coreclr msbuild not ready yet!!!!
rem set _msbuildexe=%~dp0Tools\CoreRun.exe %~dp0Tools\MSBuild.exe
rem set _nrswitch=

:: See <http://www.appveyor.com/docs/environment-variables>
if defined APPVEYOR (
   rem See <http://www.appveyor.com/docs/build-phase>
   if exist "C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" (
    rem HACK HACK HACK
   set _msbuildexe=%_msbuildexe% /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
   )
)

REM set msbuildflags=/maxcpucount %_nrswitch% /nologo
set msbuildflags=%_nrswitch% /nologo
set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

echo ---------------- Done with prepare, starting package restore ----------------
set _nugetexe="%~dp0.nuget\NuGet.exe"
set _nugetconfig="%~dp0.nuget\NuGet.Config"

if '%RestorePackages%' == 'true' (
    %_ngenexe% install %_nugetexe%  /nologo 

    %_nugetexe% restore packages.config -PackagesDirectory packages -ConfigFile %_nugetconfig%
    @if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure

    if '%BUILD_VS%' == '1' (
        %_nugetexe% restore vsintegration\packages.config -PackagesDirectory packages -ConfigFile %_nugetconfig%
        @if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure
    )

    if '%BUILD_SETUP%' == '1' (
        %_nugetexe% restore setup\packages.config -PackagesDirectory packages -ConfigFile %_nugetconfig%
        @if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure
    )
)

if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '1' (
    :: Restore the Tools directory
    call %~dp0init-tools.cmd
)

set _dotnetexe=%~dp0Tools\dotnetcli\dotnet.exe
set NUGET_PACKAGES=%~dp0Packages

set _fsiexe="packages\FSharp.Compiler.Tools.4.0.1.21\tools\fsi.exe"
if not exist %_fsiexe% echo Error: Could not find %_fsiexe% && goto :failure
%_ngenexe% install %_fsiexe% /nologo 

if not exist %_nugetexe% echo Error: Could not find %_nugetexe% && goto :failure
%_ngenexe% install %_nugetexe% /nologo 

echo ---------------- Done with package restore, starting proto ------------------------

rem Decide if Proto need building
if NOT EXIST Proto\net40\bin\fsc-proto.exe (
  set BUILD_PROTO=1
)

set _dotnetexe=%~dp0Tools\dotnetcli\dotnet.exe
set _architecture=win7-x64

rem Build Proto
if '%BUILD_PROTO%' == '1' (
  rmdir /s /q Proto

  if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '1' (

    pushd .\lkg\fsc & %_dotnetexe% restore & popd & if ERRORLEVEL 1 echo Error:%errorlevel% dotnet restore failed & goto :failure
    pushd .\lkg\fsi & %_dotnetexe% restore & popd & if ERRORLEVEL 1 echo Error:%errorlevel% dotnet restore failed & goto :failure
    pushd .\lkg\fsc & %_dotnetexe% publish project.json --no-build -o %~dp0Tools\lkg -r !_architecture! & popd & if ERRORLEVEL 1 echo Error: dotnet publish failed  & goto :failure
    pushd .\lkg\fsi & %_dotnetexe% publish project.json --no-build -o %~dp0Tools\lkg -r !_architecture! & popd & if ERRORLEVEL 1 echo Error: dotnet publish failed  & goto :failure

    echo %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
         %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
    @if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

    echo %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
         %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
    @if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

  )

  if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '0' (

    echo %_ngenexe% install packages\FSharp.Compiler.Tools.4.0.1.21\tools\fsc.exe /nologo 
         %_ngenexe% install packages\FSharp.Compiler.Tools.4.0.1.21\tools\fsc.exe /nologo 

    echo %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
         %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
    @if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

    echo %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
         %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
    @if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

  )
)


echo ---------------- Done with proto, starting build ------------------------

if '%BUILD_PHASE%' == '1' (
   echo %_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG% /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%
        %_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG% /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%
   @if ERRORLEVEL 1 echo Error: '%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%  /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%' failed && goto :failure
)

echo ---------------- Done with build, starting update/prepare ---------------

if '%BUILD_NET40%' == '1' (
    call src\update.cmd %BUILD_CONFIG% -ngen
)

@echo set NUNITPATH=packages\NUnit.Console.3.0.0\tools\
set NUNITPATH=packages\NUnit.Console.3.0.0\tools\
if not exist %NUNITPATH% echo Error: Could not find %NUNITPATH% && goto :failure

@echo xcopy "%NUNITPATH%*.*"  "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
      xcopy "%NUNITPATH%*.*"  "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y

@echo xcopy "%~dp0tests\fsharpqa\testenv\src\nunit*.*" "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
      xcopy "%~dp0tests\fsharpqa\testenv\src\nunit*.*" "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set SYSWOW64=.
if "%OSARCH%"=="AMD64" set SYSWOW64=SysWoW64

if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

echo  SDK environment vars from Registry
                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6.2\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\WOW6432Node\Microsoft\Microsoft SDKs\NETFXSDK\4.6.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B

set PATH=%PATH%;%WINSDKNETFXTOOLS%
for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

set REGEXE32BIT=reg.exe

IF NOT DEFINED SNEXE32  IF EXIST "%WINSDKNETFXTOOLS%\sn.exe"               set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64  IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"           set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe
IF NOT DEFINED ildasm   IF EXIST "%WINSDKNETFXTOOLS%\ildasm.exe"           set ildasm=%WINSDKNETFXTOOLS%ildasm.exe


echo .
echo  SDK environment vars
echo  =======================
echo WINSDKNETFXTOOLS:  %WINSDKNETFXTOOLS%
echo SNEXE32:           %SNEXE32%
echo SNEXE64:           %SNEXE64%
echo ILDASM:            %ILDASM%
echo

if 'TEST_NET40_COMPILERUNIT_SUITE' == '0' and 'TEST_PORTABLE_COREUNIT_SUITE' == '0' and 'TEST_CORECLR_COREUNIT_SUITE' == '0' and 'TEST_VS_IDEUNIT_SUITE' == '0' and 'TEST_NET40_FSHARP_SUITE' == '0' and 'TEST_NET40_FSHARPQA_SUITE' == '0' goto :success

echo ---------------- Done with update, starting tests -----------------------

if NOT "%INCLUDE_TEST_SPEC_NUNIT%" == "" (
    set WHERE_ARG_NUNIT=--where "%INCLUDE_TEST_SPEC_NUNIT%"
)
if NOT "%INCLUDE_TEST_TAGS%" == "" (
    set TTAGS_ARG_RUNALL=-ttags:%INCLUDE_TEST_TAGS%
)
echo WHERE_ARG_NUNIT=!WHERE_ARG_NUNIT!

set NUNITPATH=%~dp0tests\fsharpqa\testenv\bin\nunit\
set NUNIT3_CONSOLE=%~dp0packages\NUnit.Console.3.0.0\tools\nunit3-console.exe
set link_exe=%~dp0packages\VisualCppTools.14.0.24519-Pre\lib\native\bin\link.exe
if not exist "%link_exe%" (
    echo Error: failed to find '%link_exe%' use nuget to restore the VisualCppTools package
    goto :failure
)

if /I not '%single_threaded%' == 'true' (set PARALLEL_ARG=-procs:%NUMBER_OF_PROCESSORS%) else set PARALLEL_ARG=-procs:0

set FSCBINPATH=%~dp0%BUILD_CONFIG%\net40\bin
set RESULTSDIR=%~dp0tests\TestResults
if not exist "%RESULTSDIR%" (mkdir "%RESULTSDIR%")

ECHO FSCBINPATH=%FSCBINPATH%
ECHO RESULTSDIR=%RESULTSDIR%
ECHO link_exe=%link_exe%
ECHO NUNIT3_CONSOLE=%NUNIT3_CONSOLE%
ECHO NUNITPATH=%NUNITPATH%

REM ---------------- net40-fsharp  -----------------------


if '%TEST_NET40_FSHARP_SUITE%' == '1' (

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
    set XMLFILE=!RESULTSDIR!\test-net40-fsharp-results.xml
    if '%CI%' == '1' (
        set OUTPUTFILE=!RESULTSDIR!\test-net40-fsharp-output.log
        set OUTPUTARG=--output:"!OUTPUTFILE!" 
        set ERRORFILE=!RESULTSDIR!\test-net40-fsharp-errors.log
        set ERRORARG=--err:"!ERRORFILE!" 
    )

    echo "!NUNIT3_CONSOLE!" --verbose "!FSCBINPATH!\FSharp.Tests.FSharpSuite.dll" --framework:V4.0 --work:"!FSCBINPATH!"  !OUTPUTARG! !ERRORARG! --result:"!XMLFILE!;format=nunit3" !WHERE_ARG_NUNIT!
         "!NUNIT3_CONSOLE!" --verbose "!FSCBINPATH!\FSharp.Tests.FSharpSuite.dll" --framework:V4.0 --work:"!FSCBINPATH!"  !OUTPUTARG! !ERRORARG! --result:"!XMLFILE!;format=nunit3" !WHERE_ARG_NUNIT!

    if errorlevel 1 (
        type "!ERRORFILE!"
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-fsharp failed, see log above -- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- net40-fsharpqa  -----------------------

set OSARCH=%PROCESSOR_ARCHITECTURE%

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
set HOSTED_COMPILER=1

if '%TEST_NET40_FSHARPQA_SUITE%' == '1' (

	set FSC=!FSCBINPATH!\fsc.exe
	set FSCOREDLLPATH=!FSCBinPath!\FSharp.Core.dll
	set PATH=!FSCBINPATH!;!PATH!

	where.exe perl > NUL 2> NUL
	if errorlevel 1 (
		echo Error: perl is not in the PATH, it is required for the net40-fsharpqa test suite
		goto :failure
	)

	set OUTPUTFILE=test-net40-fsharpqa-results.log
	set ERRORFILE=test-net40-fsharpqa-errors.log
	set FAILENV=test-net40-fsharpqa-errors

	pushd %~dp0tests\fsharpqa\source
	echo perl %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot !RESULTSDIR! -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no !TTAGS_ARG_RUNALL! !PARALLEL_ARG!
		 perl %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot !RESULTSDIR! -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no !TTAGS_ARG_RUNALL! !PARALLEL_ARG!

    popd
    if ERRORLEVEL 1 (
        type "%RESULTSDIR%\!OUTPUTFILE!"
        echo -----------------------------------------------------------------
        type "%RESULTSDIR%\!ERRORFILE!"
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-fsharpqa failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- net40-compilerunit  -----------------------

if '%TEST_NET40_COMPILERUNIT_SUITE%' == '1' (

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
    set XMLFILE=!RESULTSDIR!\test-net40-compilerunit-results.xml
    if '%CI%' == '1' (
        set OUTPUTFILE=!RESULTSDIR!\test-net40-compilerunit-output.log
        set ERRORFILE=!RESULTSDIR!\test-net40-compilerunit-errors.log
	    set ERRORARG=--err:"!ERRORFILE!" 
	    set OUTPUTARG=--output:"!OUTPUTFILE!" 
		)
    set ERRORFILE=!RESULTSDIR!\test-net40-compilerunit-errors.log
    echo "!NUNIT3_CONSOLE!" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG!  !ERRORARG! --work:"!FSCBINPATH!" "!FSCBINPATH!\..\..\net40\bin\FSharp.Compiler.Unittests.dll" !WHERE_ARG_NUNIT!
         "!NUNIT3_CONSOLE!" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG!  !ERRORARG! --work:"!FSCBINPATH!" "!FSCBINPATH!\..\..\net40\bin\FSharp.Compiler.Unittests.dll" !WHERE_ARG_NUNIT!

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        type "!OUTPUTFILE!"
        echo -----------------------------------------------------------------
        type "!ERRORFILE!"
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-compilerunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- net40-coreunit  -----------------------

if '%TEST_NET40_COREUNIT_SUITE%' == '1' (

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
    set XMLFILE=!RESULTSDIR!\test-net40-coreunit-results.xml
    if '%CI%' == '1' (
        set ERRORFILE=!RESULTSDIR!\test-net40-coreunit-errors.log
        set OUTPUTFILE=!RESULTSDIR!\test-net40-coreunit-output.log
	    set ERRORARG=--err:"!ERRORFILE!" 
	    set OUTPUTARG=--output:"!OUTPUTFILE!" 
		)

    echo "!NUNIT3_CONSOLE!" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! --work:"!FSCBINPATH!" "!FSCBINPATH!\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!
         "!NUNIT3_CONSOLE!" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! --work:"!FSCBINPATH!" "!FSCBINPATH!\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        type "!OUTPUTFILE!"
        echo -----------------------------------------------------------------
        type "!ERRORFILE!"
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-coreunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM  ---------------- portable-coreunit  -----------------------

if '%TEST_PORTABLE_COREUNIT_SUITE%' == '1' (

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
		set XMLFILE=!RESULTSDIR!\test-portable-coreunit-results.xml
    if '%CI%' == '1' (
    	set OUTPUTFILE=!RESULTSDIR!\test-portable-coreunit-output.log
	    set ERRORFILE=!RESULTSDIR!\test-portable-coreunit-errors.log
        set ERRORARG=--err:"!ERRORFILE!" 
        set OUTPUTARG=--output:"!OUTPUTFILE!" 
		)

    echo "!NUNIT3_CONSOLE!" /framework:V4.0 /result="!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! /work="!FSCBINPATH!" "!FSCBINPATH!\..\..\portable7\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable47\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable78\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable259\bin\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!
         "!NUNIT3_CONSOLE!" /framework:V4.0 /result="!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! /work="!FSCBINPATH!" "!FSCBINPATH!\..\..\portable7\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable47\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable78\bin\FSharp.Core.Unittests.dll" "!FSCBINPATH!\..\..\portable259\bin\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        type "!OUTPUTFILE!"
        echo -----------------------------------------------------------------
        type "!ERRORFILE!"
        echo -----------------------------------------------------------------
        echo Error: Running tests portable-coreunit failed, see logs above -- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM  ---------------- coreclr-coreunit  -----------------------

if '%TEST_CORECLR_COREUNIT_SUITE%' == '1' (

    set XMLFILE=!RESULTSDIR!\test-coreclr-coreunit-results.xml
    set OUTPUTFILE=!RESULTSDIR!\test-coreclr-coreunit-output.log
    set ERRORFILE=!RESULTSDIR!\test-coreclr-coreunit-errors.log

    echo "%_dotnetexe%" "%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\FSharp.Core.Unittests\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!
         "%_dotnetexe%" "%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\FSharp.Core.Unittests\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!

    if ERRORLEVEL 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests coreclr-coreunit failed, see logs above-- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- coreclr-fsharp  -----------------------

if '%TEST_CORECLR_FSHARP_SUITE%' == '1' (

    set single_threaded=true
    set permutations=FSC_CORECLR

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
    set XMLFILE=!RESULTSDIR!\test-coreclr-fsharp-results.xml
    echo "%_dotnetexe%" "%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\FSharp.Core.Unittests\FSharp.Core.Unittests.dll" !WHERE_ARG_NUNIT!
         "%_dotnetexe%" "%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\FSharp.Tests.FSharpSuite.DrivingCoreCLR\FSharp.Tests.FSharpSuite.DrivingCoreCLR.dll" !WHERE_ARG_NUNIT!

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests coreclr-fsharp failed, see logs above-- FAILED
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- vs-ideunit  -----------------------

if '%TEST_VS_IDEUNIT_SUITE%' == '1' (

    set OUTPUTARG=
    set ERRORARG=
    set OUTPUTFILE=
    set ERRORFILE=
	set XMLFILE=!RESULTSDIR!\test-vs-ideunit-results.xml
    if '%CI%' == '1' (
    	set OUTPUTFILE=!RESULTSDIR!\test-vs-ideunit-output.log
	    set ERRORFILE=!RESULTSDIR!\test-vs-ideunit-errors.log
        set ERRORARG=--err:"!ERRORFILE!" 
        set OUTPUTARG=--output:"!OUTPUTFILE!" 
	)

	pushd !FSCBINPATH!
    echo "!NUNIT3_CONSOLE!" --verbose --x86 --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! --work:"!FSCBINPATH!"  --workers=1 --agents=1 --full "!FSCBINPATH!\VisualFSharp.Unittests.dll" !WHERE_ARG_NUNIT!
         "!NUNIT3_CONSOLE!" --verbose --x86 --framework:V4.0 --result:"!XMLFILE!;format=nunit3" !OUTPUTARG! !ERRORARG! --work:"!FSCBINPATH!"  --workers=1 --agents=1 --full "!FSCBINPATH!\VisualFSharp.Unittests.dll" !WHERE_ARG_NUNIT!
	popd

    if errorlevel 1 (
        echo --------begin vs-ide-unit output ---------------------
        type "!OUTPUTFILE!"
        echo --------end vs-ide-unit output -----------------------
        echo -------begin vs-ide-unit errors ----------------------
        type "!ERRORFILE!"
        echo -------end vs-ide-unit errors ------------------------
        echo Error: Running tests vs-ideunit failed, see logs above, search for "Errors and Failures"  -- FAILED
        echo ----------------------------------------------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- publish-vsix -----------------------

if '%PUBLISH_VSIX%' == '1' (
    if not '%MYGET_APIKEY%' == '' (
        powershell -noprofile -executionPolicy ByPass -file "%~dp0setup\publish-assets.ps1" -binariesPath "%~dp0%BUILD_CONFIG%" -branchName "%BUILD_SOURCEBRANCH%" -apiKey "%MYGET_APIKEY%"
        if errorlevel 1 goto :failure
    ) else (
        echo No MyGet API key specified, skipping package publish.
    )
)

goto :success
REM ------ exit -------------------------------------
:failure
endlocal
exit /b 1

:success
endlocal
exit /b 0
