rem Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
@if "%_echo%"=="" echo off

setlocal enableDelayedExpansion

:ARGUMENTS_VALIDATION
if /I "%1" == "--help"  (goto :USAGE)
if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)
goto :ARGUMENTS_OK


:USAGE

echo Build and run a subset of test suites
echo.
echo Usage:
echo.
echo build.cmd ^<all^|net40^|coreclr^|vs^|fcs^>
echo           ^<proto^|protofx^>
echo           ^<ci^|ci_part1^|ci_part2^|ci_part3^|ci_part4^|microbuild^|nuget^>
echo           ^<debug^|release^>
echo           ^<diag^|publicsign^>
echo           ^<nobuild^|test^|no-test^|test-net40-coreunit^|test-coreclr-coreunit^|test-compiler-unit^|test-net40-ideunit^|test-net40-fsharp^|test-coreclr-fsharp^|test-net40-fsharpqa^|end-2-end^>
echo           ^<include tag^>
echo           ^<init^>
echo.
echo No arguments default to default", meaning this (no testing)
echo.
echo     build.cmd net40 
echo.
echo.Other examples:
echo.
echo.    build.cmd net40            (build compiler for .NET Framework)
echo.    build.cmd coreclr          (build compiler for .NET Core)
echo.    build.cmd buildfromsource  (build compiler for .NET Core -- Verify that buildfromsource works)
echo.    build.cmd vs               (build Visual Studio IDE Tools)
echo.    build.cmd all              (build everything)
echo.    build.cmd test             (build and test default targets)
echo.    build.cmd net40 test       (build and test compiler for .NET Framework)
echo.    build.cmd coreclr test     (build and test compiler for .NET Core)
echo.    build.cmd vs test          (build and test Visual Studio IDE Tools)
echo.    build.cmd all test         (build and test everything)
echo.    build.cmd nobuild test include Conformance (run only tests marked with Conformance category)
echo.    build.cmd nobuild test include Expensive (run only tests marked with Expensive category)
echo.
goto :success

:ARGUMENTS_OK

rem disable setup build by setting FSC_BUILD_SETUP=0
if /i "%FSC_BUILD_SETUP%" == "" (set FSC_BUILD_SETUP=1) 

rem by default don't build coreclr lkg.  However allow configuration by setting an environment variable : set BUILD_PROTO_WITH_CORECLR_LKG = 1
if "%BUILD_PROTO_WITH_CORECLR_LKG%" =="" (set BUILD_PROTO_WITH_CORECLR_LKG=0) 

set BUILD_PROTO=0
set BUILD_PHASE=1
set BUILD_NET40=0
set BUILD_NET40_FSHARP_CORE=0
set BUILD_CORECLR=0
set BUILD_FROMSOURCE=0
set BUILD_VS=0
set BUILD_FCS=0
set BUILD_CONFIG=release
set BUILD_DIAG=
set BUILD_PUBLICSIGN=0

set TEST_NET40_COMPILERUNIT_SUITE=0
set TEST_NET40_COREUNIT_SUITE=0
set TEST_NET40_FSHARP_SUITE=0
set TEST_NET40_FSHARPQA_SUITE=0
set TEST_CORECLR_COREUNIT_SUITE=0
set TEST_CORECLR_FSHARP_SUITE=0
set TEST_VS_IDEUNIT_SUITE=0
set TEST_FCS=0
set TEST_END_2_END=0
set INCLUDE_TEST_TAGS=

set COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES=0

set SIGN_TYPE=%PB_SIGNTYPE%

REM ------------------ Parse all arguments -----------------------

set _autoselect=1
set _autoselect_tests=0
set no_test=0
set /a counter=0
for /l %%x in (1 1 9) do (
    set /a counter=!counter!+1
    set /a nextcounter=!counter!+1
    call :PROCESS_ARG %%!counter! %%!nextcounter! "!counter!"
)
for %%i in (%BUILD_FSC_DEFAULT%) do ( call :PROCESS_ARG %%i )

REM apply defaults

if /i "%_buildexit%" == "1" (
		exit /B %_buildexitvalue%
)

if /i "%_autoselect%" == "1" (
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_NET40=1
)

if /i "%_autoselect_tests%" == "1" (
    if /i "%BUILD_NET40_FSHARP_CORE%" == "1" (
        set TEST_NET40_COREUNIT_SUITE=1
    )

    if /i "%BUILD_NET40%" == "1" (
        set TEST_NET40_COMPILERUNIT_SUITE=1
        set TEST_NET40_COREUNIT_SUITE=1
        set TEST_NET40_FSHARP_SUITE=1
        set TEST_NET40_FSHARPQA_SUITE=1
    )

    if /i "%BUILD_FCS%" == "1" (
        set TEST_FCS=1
    )

    if /i "%BUILD_CORECLR%" == "1" (
        set TEST_CORECLR_FSHARP_SUITE=1
        set TEST_CORECLR_COREUNIT_SUITE=1
    )

    if /i "%BUILD_VS%" == "1" (
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

rem Do no work
if /i "%ARG%" == "none" (
    set _buildexit=1
    set _buildexitvalue=0
)

if /i "%ARG%" == "net40-lib" (
    set _autoselect=0
    set BUILD_NET40_FSHARP_CORE=1
)

if /i "%ARG%" == "net40" (
    set _autoselect=0
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_NET40=1
    set COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES=1
)

if /i "%ARG%" == "coreclr" (
    set _autoselect=0
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set BUILD_FROMSOURCE=1
)

if /i "%ARG%" == "buildfromsource" (
    set _autoselect=0
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_FROMSOURCE=1
)

if /i "%ARG%" == "vs" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_VS=1
    set COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES=1
)

if /i "%ARG%" == "fcs" (
    set _autoselect=0
    set BUILD_FCS=1
)

if /i "%ARG%" == "vstest" (
    set TEST_VS_IDEUNIT_SUITE=1
)

if /i "%ARG%" == "nobuild" (
    set BUILD_PHASE=0
)
if /i "%ARG%" == "all" (
    set _autoselect=0
    set COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES=1
    set BUILD_PROTO=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_VS=1
    set BUILD_FCS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set BUILD_NUGET=1
    set CI=1
)

if /i "%ARG%" == "microbuild" (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set BUILD_NUGET=1
    set BUILD_MICROBUILD=1

    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=0
    set TEST_CORECLR_FSHARP_SUITE=0
    set TEST_VS_IDEUNIT_SUITE=1
    set CI=1

    REM redirecting TEMP directories
    set TEMP=%~dp0%BUILD_CONFIG%\TEMP
    set TMP=%~dp0%BUILD_CONFIG%\TEMP
)

if /i "%ARG%" == "nuget" (
    set _autoselect=0

    set BUILD_PROTO=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set BUILD_NUGET=1
)

REM These divide "ci" into three chunks which can be done in parallel
if /i "%ARG%" == "ci_part1" (
    set _autoselect=0

    REM what we do - build and test Visual F# Tools, including setup and nuget
    set BUILD_PROTO=1
    set BUILD_NUGET=1
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_VS=1
    set TEST_VS_IDEUNIT_SUITE=1
    set BUILD_CORECLR=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set CI=1
)

if /i "%ARG%" == "ci_part2" (
    set _autoselect=0

    REM what we do - test F# on .NET Framework
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set CI=1
)

if /i "%ARG%" == "ci_part3" (
    set _autoselect=0

    REM what we do: test F# on Core CLR: nuget requires coreclr, fcs requires coreclr
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_PROTO=1
    set BUILD_CORECLR=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_NET40=1
    set TEST_CORECLR_FSHARP_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=1
    set CI=1
)

if /i "%ARG%" == "ci_part4" (
    set _autoselect=0

    REM what we do: test F# on Core CLR: nuget requires coreclr, fcs requires coreclr
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_PROTO=1
    set BUILD_CORECLR=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_NET40=1
    set BUILD_FCS=1
    set TEST_FCS=1
    set CI=1
)

if /i "%ARG%" == "end-2-end" (
    set BUILD_PROTO=1
    set BUILD_CORECLR=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_NET40=1
    set TEST_END_2_END=1
)

if /i "%ARG%" == "proto" (
    set _autoselect=0
    set BUILD_PROTO=1
)

if /i "%ARG%" == "diag" (
    set BUILD_DIAG=/v:detailed
    if not defined APPVEYOR ( set BUILD_LOG=fsharp_build_log.log )
)

if /i "%ARG%" == "debug" (
    set BUILD_CONFIG=debug
)

if /i "%ARG%" == "release" (
    set BUILD_CONFIG=release
)

if /i "%ARG%" == "test-sign" (
    set SIGN_TYPE=test
)

if /i "%ARG%" == "real-sign" (
    set SIGN_TYPE=real
)

if /i "%ARG%" == "test" (
    set _autoselect_tests=1
)

if /i "%ARG%" == "no-test" (
    set no_test=1
)

if /i "%ARG%" == "include" (
    set /a counter=!counter!+1
    if "!INCLUDE_TEST_TAGS!" == "" ( set INCLUDE_TEST_TAGS=%ARG2% ) else (set INCLUDE_TEST_TAGS=%ARG2%;!INCLUDE_TEST_TAGS! )
)

if /i "%ARG%" == "test-all" (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_CORECLR=1
    set BUILD_VS=1
    set BUILD_FCS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set BUILD_NUGET=1

    set TEST_NET40_COMPILERUNIT_SUITE=1
    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_NET40_FSHARPQA_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=1
    set TEST_VS_IDEUNIT_SUITE=1
    set TEST_FCS=1
    set TEST_END_2_END=1
)

if /i "%ARG%" == "test-net40-fsharpqa" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set TEST_NET40_FSHARPQA_SUITE=1
)

if /i "%ARG%" == "test-compiler-unit" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set TEST_NET40_COMPILERUNIT_SUITE=1
)

if /i "%ARG%" == "test-net40-ideunit" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_VS=1
    set TEST_VS_IDEUNIT_SUITE=1
)

if /i "%ARG%" == "test-net40-coreunit" (
    set _autoselect=0
    set BUILD_NET40_FSHARP_CORE=1
    set TEST_NET40_COREUNIT_SUITE=1
)

if /i "%ARG%" == "test-coreclr-coreunit" (
    set _autoselect=0
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set TEST_CORECLR_COREUNIT_SUITE=1
)

if /i "%ARG%" == "test-net40-fsharp" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set TEST_NET40_FSHARP_SUITE=1
)

if /i "%ARG%" == "test-fcs" (
    set _autoselect=0
    set BUILD_FCS=1
    set TEST_FCS=1
)

if /i "%ARG%" == "test-coreclr-fsharp" (
    set _autoselect=0
    set BUILD_NET40=1
    set BUILD_NET40_FSHARP_CORE=1
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_CORECLR=1
    set TEST_CORECLR_FSHARP_SUITE=1
)

if /i "%ARG%" == "publicsign" (
    set BUILD_PUBLICSIGN=1
)

if /i "%ARG%" == "init" (
    set BUILD_PROTO_WITH_CORECLR_LKG=1
)

goto :EOF
:: Note: "goto :EOF" returns from an in-batchfile "call" command
:: in preference to returning from the entire batch file.

REM ------------------ Report config -----------------------

:MAIN

REM after this point, ARG variable should not be used, use only BUILD_* or TEST_*

REM all PB_* variables override any settings

REM if the `PB_SKIPTESTS` variable is set to 'true' then no tests should be built or run, even if explicitly specified
if /i "%PB_SKIPTESTS%" == "true" (
    set TEST_NET40_COMPILERUNIT_SUITE=0
    set TEST_NET40_COREUNIT_SUITE=0
    set TEST_NET40_FSHARP_SUITE=0
    set TEST_NET40_FSHARPQA_SUITE=0
    set TEST_CORECLR_COREUNIT_SUITE=0
    set TEST_CORECLR_FSHARP_SUITE=0
    set TEST_VS_IDEUNIT_SUITE=0
)

if /i "%TEST_NET40_FSHARP_SUITE" == "1" (
    if /i "%TEST_CORECLR_FSHARP_SUITE%" == "1" (
        TEST_END_2_END=1
    )
)

rem Decide if Proto need building
if NOT EXIST Proto\net40\bin\fsc.exe (
  set BUILD_PROTO=1
)

rem
rem This stops the dotnet cli from hunting around and 
rem finding the highest possible dotnet sdk version to use.
rem
rem description of dotnet lookup here:  
rem     https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet?tabs=netcore2x
set DOTNET_MULTILEVEL_LOOKUP=false

echo Build/Tests configuration:
echo.
echo BUILD_PROTO=%BUILD_PROTO%
echo BUILD_PROTO_WITH_CORECLR_LKG=%BUILD_PROTO_WITH_CORECLR_LKG%
echo BUILD_NET40=%BUILD_NET40%
echo BUILD_NET40_FSHARP_CORE=%BUILD_NET40_FSHARP_CORE%
echo BUILD_CORECLR=%BUILD_CORECLR%
echo BUILD_FROMSOURCE=%BUILD_FROMSOURCE%
echo BUILD_VS=%BUILD_VS%
echo BUILD_FCS=%BUILD_FCS%
echo BUILD_SETUP=%BUILD_SETUP%
echo BUILD_NUGET=%BUILD_NUGET%
echo BUILD_CONFIG=%BUILD_CONFIG%
echo BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%
echo BUILD_MICROBUILD=%BUILD_MICROBUILD%
echo.
echo PB_SKIPTESTS=%PB_SKIPTESTS%
echo PB_RESTORESOURCE=%PB_RESTORESOURCE%
echo.
echo SIGN_TYPE=%SIGN_TYPE%
echo.
echo COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES=%COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES%
echo.
echo TEST_FCS=%TEST_FCS%
echo TEST_NET40_COMPILERUNIT_SUITE=%TEST_NET40_COMPILERUNIT_SUITE%
echo TEST_NET40_COREUNIT_SUITE=%TEST_NET40_COREUNIT_SUITE%
echo TEST_NET40_FSHARP_SUITE=%TEST_NET40_FSHARP_SUITE%
echo TEST_NET40_FSHARPQA_SUITE=%TEST_NET40_FSHARPQA_SUITE%
echo TEST_CORECLR_COREUNIT_SUITE=%TEST_CORECLR_COREUNIT_SUITE%
echo TEST_CORECLR_FSHARP_SUITE=%TEST_CORECLR_FSHARP_SUITE%
echo TEST_VS_IDEUNIT_SUITE=%TEST_VS_IDEUNIT_SUITE%
echo INCLUDE_TEST_TAGS=%INCLUDE_TEST_TAGS%
echo TEMP=%TEMP%

:: load Visual Studio 2017 developer command prompt if VS150COMNTOOLS is not set

:: If this is not set, VsDevCmd.bat will change %cd% to [USERPROFILE]\source, causing the build to fail.
SET VSCMD_START_DIR=%cd%

:: Try find installation path of VS2017 with vswhere.exe
if "%VS150COMNTOOLS%" EQU "" if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\" (
    for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -property installationPath`) do set VS_INSTALLATION_PATH=%%i
)

if "%VS_INSTALLATION_PATH%" NEQ "" (
    call "%VS_INSTALLATION_PATH%\Common7\Tools\VsDevCmd.bat"
)

:: If there's no installation of VS2017 or VS2017 Preview, use the build tools
if "%VS150COMNTOOLS%" EQU "" if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat" (
    call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat"
)

echo.
echo Environment
set
echo.
echo.

echo ---------------- Done with arguments, starting preparation -----------------
rem set TargetFrameworkSDKToolsDirectory --- needed for sdk to find al.exe. 
if not "%TargetFrameworkSDKToolsDirectory%" == "" ( goto have_TargetFrameworkSDKToolsDirectory ) 
set TargetFrameworkSDKToolsDirectory=%WindowsSDK_ExecutablePath_x64%

if not "%TargetFrameworkSDKToolsDirectory%" == "" ( goto have_TargetFrameworkSDKToolsDirectory ) 
set TargetFrameworkSDKToolsDirectory=%WindowsSDK_ExecutablePath_x86%

:have_TargetFrameworkSDKToolsDirectory

if "%RestorePackages%"=="" (
    set RestorePackages=true
)

@echo VSSDKToolsPath: %VSSDKToolsPath%
@echo VSSDKIncludes:  %VSSDKIncludes%
@echo TargetFrameworkSDKToolsDirectory:  %TargetFrameworkSDKToolsDirectory%

@call src\update.cmd signonly

:: Check prerequisites
if not "%VisualStudioVersion%" == "" goto vsversionset
if exist "%VS160COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=16.0
if not "%VisualStudioVersion%" == "" goto vsversionset

if not "%VisualStudioVersion%" == "" goto vsversionset
if exist "%VS150COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=15.0
if not "%VisualStudioVersion%" == "" goto vsversionset

if not "%VisualStudioVersion%" == "" goto vsversionset
if exist "%VS150COMNTOOLS%\..\..\ide\devenv.exe" set VisualStudioVersion=15.0
if not "%VisualStudioVersion%" == "" goto vsversionset

if exist "%VS140COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not "%VisualStudioVersion%" == "" goto vsversionset

if exist "%VS120COMNTOOLS%\..\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0

:vsversionset
if "%VisualStudioVersion%" == "" echo Error: Could not find an installation of Visual Studio && goto :failure

if exist "%VS160COMNTOOLS%\..\..\MSBuild\Current\Bin\MSBuild.exe" (
    set _msbuildexe="%VS160COMNTOOLS%\..\..\MSBuild\Current\Bin\MSBuild.exe"
    goto :havemsbuild
)
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

set msbuildflags=%_nrswitch% /nologo /clp:Summary /v:minimal
set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

echo ---------------- Done with prepare, starting package restore ----------------

:: create a global.json
set /p DOTNET_TOOLS_VERSION=<"%~dp0DotnetCLIToolsVersion.txt"
echo { "sdk": { "version": "%DOTNET_TOOLS_VERSION%" } }>global.json

:: Restore the Tools directory
call "%~dp0init-tools.cmd"
set _dotnetexe=%~dp0Tools\dotnet20\dotnet.exe
set path=%~dp0Tools\dotnet20\;%path%

if not "%PB_PackageVersionPropsUrl%" == "" (
    echo ----------- do dependency uptake check -----------

    set dependencyUptakeDir=%~dp0Tools\dependencyUptake
    if not exist "!dependencyUptakeDir!" mkdir "!dependencyUptakeDir!"

    :: download package version overrides
    echo powershell -noprofile -executionPolicy RemoteSigned -command "Invoke-WebRequest -Uri '%PB_PackageVersionPropsUrl%' -OutFile '!dependencyUptakeDir!\PackageVersions.props'"
         powershell -noprofile -executionPolicy RemoteSigned -command "Invoke-WebRequest -Uri '%PB_PackageVersionPropsUrl%' -OutFile '!dependencyUptakeDir!\PackageVersions.props'"
    if ERRORLEVEL 1 echo Error downloading package version properties && goto :failure
)

if "%RestorePackages%" == "true" (
    if "%BUILD_FCS%" == "1" (
      cd fcs
      .paket\paket.exe restore
      cd..
      @if ERRORLEVEL 1 echo Error: Paket restore failed  && goto :failure
    )
)

echo ---------------- Done with package restore, verify buildfrom source ---------------
if "%BUILD_PROTO_WITH_CORECLR_LKG%" == "1" (
  pushd src
  call buildfromsource.cmd
  @if ERRORLEVEL 1 echo Error: buildfromsource.cmd failed  && goto :failure
  popd
)

echo ---------------- Done with package restore, starting proto ------------------------
set logdir=%~dp0%BUILD_CONFIG%\logs
if not exist "!logdir!" mkdir "!logdir!"

rem Build Proto
if "%BUILD_PROTO%" == "1" (
    rmdir /s /q Proto

    echo %_msbuildexe% proto.proj /t:Restore /bl:%~dp0Proto\proto.proj.restore.binlog
         %_msbuildexe% proto.proj /t:Restore /bl:%~dp0Proto\proto.proj.restore.binlog
    @if ERRORLEVEL 1 echo Error restoring proto failed && goto :failure

    echo %_msbuildexe% proto.proj /t:Build /bl:%~dp0Proto\proto.proj.build.binlog
         %_msbuildexe% proto.proj /t:Build /bl:%~dp0Proto\proto.proj.build.binlog
    @if ERRORLEVEL 1 echo Error building proto failed && goto :failure
)

echo ---------------- Done with SDK restore, starting build ------------------------

if "%BUILD_PHASE%" == "1" (

    echo %_msbuildexe% fsharp.proj /t:Restore /p:Configuration=%BUILD_CONFIG% /bl:%~dp0%BUILD_CONFIG%\fsharp.proj.restore.binlog
         %_msbuildexe% fsharp.proj /t:Restore /p:Configuration=%BUILD_CONFIG% /bl:%~dp0%BUILD_CONFIG%\fsharp.proj.restore.binlog

    echo %_msbuildexe% fsharp.proj /t:Build /p:Configuration=%BUILD_CONFIG% /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN% /bl:%~dp0%BUILD_CONFIG%\fsharp.proj.build.binlog
         %_msbuildexe% fsharp.proj /t:Build /p:Configuration=%BUILD_CONFIG% /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN% /bl:%~dp0%BUILD_CONFIG%\fsharp.proj.build.binlog

   @if ERRORLEVEL 1 echo Error build failed && goto :failure
)

echo ---------------- Done with build, starting assembly version checks ---------------
set asmvercheckpath=%~dp0tests\fsharpqa\testenv\src\AssemblyVersionCheck

if "%BUILD_NET40%" == "1" (
  echo #r @"%USERPROFILE%\.nuget\packages\Newtonsoft.Json\9.0.1\lib\net45\Newtonsoft.Json.dll">%asmvercheckpath%\assemblies.fsx
  echo "%~dp0%BUILD_CONFIG%\net40\bin\fsi.exe" "%asmvercheckpath%\AssemblyVersionCheck.fsx" -- "%~dp0build\config\AssemblySignToolData.json" "%~dp0%BUILD_CONFIG%"
       "%~dp0%BUILD_CONFIG%\net40\bin\fsi.exe" "%asmvercheckpath%\AssemblyVersionCheck.fsx" -- "%~dp0build\config\AssemblySignToolData.json" "%~dp0%BUILD_CONFIG%"
  if ERRORLEVEL 1 echo Error verifying assembly versions and commit hashes. && goto :failure
)

echo ---------------- Done with assembly version checks, starting assembly signing ---------------

if not "%SIGN_TYPE%" == "" (
    echo %_msbuildexe% build\projects\Signing.proj /t:Restore
         %_msbuildexe% build\projects\Signing.proj /t:Restore

    echo %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\AssemblySignToolData.json
         %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\AssemblySignToolData.json

    if ERRORLEVEL 1 echo Error running sign tool && goto :failure
)

echo ---------------- Done with assembly signing, start package creation ---------------

echo %_msbuildexe% %msbuildflags% build-nuget-packages.proj /p:Configuration=%BUILD_CONFIG% /t:Pack /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.build-nuget-packages.build.%BUILD_CONFIG%.binlog
     %_msbuildexe% %msbuildflags% build-nuget-packages.proj /p:Configuration=%BUILD_CONFIG% /t:Pack /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.build-nuget-packages.build.%BUILD_CONFIG%.binlog
if ERRORLEVEL 1 echo Error building NuGet packages && goto :failure

if not "%SIGN_TYPE%" == "" (
    echo %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\PackageSignToolData.json
         %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\PackageSignToolData.json
    if ERRORLEVEL 1 echo Error running sign tool && goto :failure
)

if "%BUILD_SETUP%" == "1" (
    echo %_msbuildexe% %msbuildflags% setup\build-insertion.proj /p:Configuration=%BUILD_CONFIG%  /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.build-insertion.build.%BUILD_CONFIG%.binlog
         %_msbuildexe% %msbuildflags% setup\build-insertion.proj /p:Configuration=%BUILD_CONFIG%  /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.build-insertion.build.%BUILD_CONFIG%.binlog
    if ERRORLEVEL 1 echo Error building insertion packages && goto :failure
)

if not "%SIGN_TYPE%" == "" (
    echo %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\InsertionSignToolData.json
         %_msbuildexe% build\projects\Signing.proj /t:DoSigning /p:SignType=%SIGN_TYPE% /p:Configuration=%BUILD_CONFIG% /p:ConfigFile=%~dp0build\config\InsertionSignToolData.json
    if ERRORLEVEL 1 echo Error running sign tool && goto :failure
)

echo ---------------- Done with signing, building insertion files ---------------

if "%BUILD_SETUP%" == "1" (
    echo %_msbuildexe% %msbuildflags% setup\Swix\Microsoft.FSharp.vsmanproj /p:Configuration=%BUILD_CONFIG% /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.setup-swix.build.%BUILD_CONFIG%.binlog
         %_msbuildexe% %msbuildflags% setup\Swix\Microsoft.FSharp.vsmanproj /p:Configuration=%BUILD_CONFIG% /bl:%~dp0%BUILD_CONFIG%\logs\msbuild.setup-swix.build.%BUILD_CONFIG%.binlog
    if ERRORLEVEL 1 echo Error building .vsmanproj && goto :failure
)

echo ---------------- Done building insertion files, starting pack/update/prepare ---------------

if "%BUILD_NET40_FSHARP_CORE%" == "1" (
  echo ----------------  start update.cmd ---------------
  call src\update.cmd %BUILD_CONFIG% -ngen
)

if "%COPY_FSCOMP_RESOURCE_FOR_BUILD_FROM_SOURCES%" == "1" (
  echo ----------------  copy fscomp resource for build from sources ---------------
  copy /y src\fsharp\FSharp.Compiler.Private\obj\%BUILD_CONFIG%\net40\FSComp.* src\buildfromsource\FSharp.Compiler.Private
)

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set SYSWOW64=.
if "%OSARCH%"=="AMD64" set SYSWOW64=SysWoW64

if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

echo SDK environment vars from Registry
echo ==================================

for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

set REGEXE32BIT=reg.exe

IF NOT DEFINED SNEXE32  IF EXIST "%WINSDKNETFXTOOLS%\sn.exe"                set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64  IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"             set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe

echo.
echo SDK environment vars
echo =======================
echo WINSDKNETFXTOOLS:  %WINSDKNETFXTOOLS%
echo SNEXE32:           %SNEXE32%
echo SNEXE64:           %SNEXE64%
echo

if "%TEST_NET40_COMPILERUNIT_SUITE%" == "0" if "%TEST_FCS%" == "0" if "%TEST_NET40_COREUNIT_SUITE%" == "0" if "TEST_CORECLR_FSHARP_SUITE" == "0" if "%TEST_CORECLR_COREUNIT_SUITE%" == "0" if "%TEST_VS_IDEUNIT_SUITE%" == "0" if "%TEST_NET40_FSHARP_SUITE%" == "0" if "%TEST_NET40_FSHARPQA_SUITE%" == "0" goto :success

if "%no_test%" == "1" goto :success

echo ---------------- Done with update, starting tests -----------------------

if NOT "%INCLUDE_TEST_TAGS%" == "" (
    set TTAGS_ARG_RUNALL=-ttags:%INCLUDE_TEST_TAGS%
)

set link_exe=%~dp0tests\fsharpqa\testenv\bin\link\link.exe
if not exist "%link_exe%" (
    echo Error: failed to find "%link_exe%" use nuget to restore the VisualCppTools package
    goto :failure
)

if /I not "%single_threaded%" == "true" (set PARALLEL_ARG=-procs:%NUMBER_OF_PROCESSORS%) else set PARALLEL_ARG=-procs:0

set FSCBINPATH=%~dp0%BUILD_CONFIG%\net40\bin
set RESULTSDIR=%~dp0tests\TestResults
if not exist "%RESULTSDIR%" (mkdir "%RESULTSDIR%")

ECHO FSCBINPATH=%FSCBINPATH%
ECHO RESULTSDIR=%RESULTSDIR%
ECHO link_exe=%link_exe%

REM ---------------- test-net40-fsharp  -----------------------

if "%TEST_NET40_FSHARP_SUITE%" == "1" (

    set LOGFILE=%~dp0tests\TestResults\FSharp.Tests.FSharpSuite.net40.trx
    echo "%_dotnetexe%" test "%~dp0tests\fsharp\FSharp.Tests.FSharpSuite.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\net40\bin"
         "%_dotnetexe%" test "%~dp0tests\fsharp\FSharp.Tests.FSharpSuite.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\net40\bin"

    if errorlevel 1 (
        echo --------------------------------------------------------------
        echo Error: Running tests net40-fsharp failed, see file `!LOGFILE!`
        echo --------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- test-fcs  -----------------------

if "%TEST_FCS%" == "1" (

    set LOGFILE=%~dp0tests\TestResults\FSharp.Compiler.Service.Tests.net40.trx
    echo "%_dotnetexe%" test "%~dp0fcs\FSharp.Compiler.Service.Tests\FSharp.Compiler.Service.Tests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0fcs\FSharp.Compiler.Service.Tests\FSharp.Compiler.Service.Tests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo --------------------------------------------------------------
        echo Error: Running net40 fcs tests, see file `!LOGFILE!`
        echo --------------------------------------------------------------
        goto :failure
    )

    set LOGFILE=%~dp0tests\TestResults\FSharp.Compiler.Service.Tests.coreclr.trx
    echo "%_dotnetexe%" test "%~dp0fcs\FSharp.Compiler.Service.Tests\FSharp.Compiler.Service.Tests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0fcs\FSharp.Compiler.Service.Tests\FSharp.Compiler.Service.Tests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo --------------------------------------------------------------
        echo Error: Running coreclr fcs tests, see file `!LOGFILE!`
        echo --------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- end2end  -----------------------
if "%TEST_END_2_END%" == "1" (

    pushd %~dp0tests\EndToEndBuildTests

    echo Execute end to end compiler tests
    echo call EndToEndBuildTests.cmd
    call EndToEndBuildTests.cmd
    if errorlevel 1 (
        popd
        Echo end to end tests failed.
        goto :failure
    )
)

REM ---------------- net40-fsharpqa  -----------------------

set OSARCH=%PROCESSOR_ARCHITECTURE%

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
set HOSTED_COMPILER=1

if "%TEST_NET40_FSHARPQA_SUITE%" == "1" (

    set CSC_PIPE=%USERPROFILE%\.nuget\packages\Microsoft.Net.Compilers\2.7.0\tools\csc.exe
    set FSC=!FSCBINPATH!\fsc.exe
    set FSCOREDLLPATH=!FSCBinPath!\FSharp.Core.dll
    set PATH=!FSCBINPATH!;!PATH!
    set perlexe=%USERPROFILE%\.nuget\packages\StrawberryPerl64\5.22.2.1\Tools\perl\bin\perl.exe
    if not exist !perlexe! (echo Error: perl was not downloaded from check the packages directory: !perlexe! && goto :failure )

    set OUTPUTFILE=test-net40-fsharpqa-results.log
    set ERRORFILE=test-net40-fsharpqa-errors.log
    set FAILENV=test-net40-fsharpqa-errors

    pushd %~dp0tests\fsharpqa\source
    echo !perlexe! %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot !RESULTSDIR! -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no !TTAGS_ARG_RUNALL! !PARALLEL_ARG!
         !perlexe! %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot !RESULTSDIR! -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no !TTAGS_ARG_RUNALL! !PARALLEL_ARG!

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

if "%TEST_NET40_COMPILERUNIT_SUITE%" == "1" (

    set LOGFILE=%~dp0tests\TestResults\FSharp.Compiler.UnitTests.net40.trx
    echo "%_dotnetexe%" test "%~dp0tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0tests\FSharp.Compiler.UnitTests\FSharp.Compiler.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-compilerunit failed, see file `!LOGFILE!`
        echo -----------------------------------------------------------------
        goto :failure
    )

    set LOGFILE=%~dp0tests\TestResults\FSharp.Build.UnitTests.net40.trx
    echo "%_dotnetexe%" test "%~dp0tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-compilernit failed, see file `!LOGFILE!`
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- net40-coreunit  -----------------------

if "%TEST_NET40_COREUNIT_SUITE%" == "1" (

    set LOGFILE=%~dp0tests\TestResults\FSharp.Core.UnitTests.net40.trx
    echo "%_dotnetexe%" test "%~dp0tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests net40-coreunit failed, see file `!LOGFILE!`
        echo -----------------------------------------------------------------
        goto :failure
    )
)

REM  ---------------- coreclr-coreunit  -----------------------

if "%TEST_CORECLR_COREUNIT_SUITE%" == "1" (

    set LOGFILE=%~dp0tests\TestResults\FSharp.Build.UnitTests.coreclr.trx
    echo "%_dotnetexe%" test "%~dp0tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0tests\FSharp.Build.UnitTests\FSharp.Build.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo -----------------------------------------------------------------
        echo Error: Running tests coreclr-compilernit failed, see file `!LOGFILE!`
        echo -----------------------------------------------------------------
        goto :failure
    )

    set LOGFILE=%~dp0tests\TestResults\FSharp.Core.UnitTests.coreclr.trx
    echo "%_dotnetexe%" test "%~dp0tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo ------------------------------------------------------------------
        echo Error: Running tests coreclr-coreunit failed, see file `!LOGFILE!`
        echo ------------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- coreclr-fsharp  -----------------------

if "%TEST_CORECLR_FSHARP_SUITE%" == "1" (
    set LOGFILE=%~dp0tests\TestResults\FSharp.Tests.FSharpSuite.coreclr.trx
    echo "%_dotnetexe%" test "%~dp0tests\fsharp\FSharp.Tests.FSharpSuite.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\coreclr\bin"
         "%_dotnetexe%" test "%~dp0tests\fsharp\FSharp.Tests.FSharpSuite.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f netcoreapp2.0 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\coreclr\bin"

    if errorlevel 1 (
        echo ----------------------------------------------------------------
        echo Error: Running tests coreclr-fsharp failed, see file `!LOGFILE!`
        echo ----------------------------------------------------------------
        goto :failure
    )
)

REM ---------------- vs-ideunit  -----------------------

if "%TEST_VS_IDEUNIT_SUITE%" == "1" (
    set LOGFILE=%~dp0tests\TestResults\GetTypesVSUnitTests.net40.trx
    echo "%_dotnetexe%" test "%~dp0vsintegration\tests\GetTypesVSUnitTests\GetTypesVSUnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"
         "%_dotnetexe%" test "%~dp0vsintegration\tests\GetTypesVSUnitTests\GetTypesVSUnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!"

    if errorlevel 1 (
        echo ---------------------------------------------------------------------------
        echo Error: Running tests net40-gettypesvsunittests failed, see file `!LOGFILE!`
        echo ---------------------------------------------------------------------------
        goto :failure
    )

    set LOGFILE=%~dp0tests\TestResults\VisualFSharp.UnitTests.net40.trx
    echo "%_dotnetexe%" test "%~dp0vsintegration\tests\UnitTests\VisualFSharp.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\net40\bin"
         "%_dotnetexe%" test "%~dp0vsintegration\tests\UnitTests\VisualFSharp.UnitTests.fsproj" --no-restore --no-build -c %BUILD_CONFIG% -f net472 -l "trx;LogFileName=!LOGFILE!" -o "%~dp0%BUILD_CONFIG%\net40\bin"
    if errorlevel 1 (
        echo ------------------------------------------------------------
        echo Error: Running tests vs-ideunit failed, see file `!LOGFILE!`
        echo ------------------------------------------------------------
        goto :failure
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
