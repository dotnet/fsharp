@if "%_echo%"=="" echo off

:ARGUMENTS_VALIDATION

if /I "%1" == "/help"   (goto :USAGE)
if /I "%1" == "/h"      (goto :USAGE)
if /I "%1" == "/?"      (goto :USAGE)
goto :ARGUMENTS_OK

:USAGE

echo Build and run a subset of test suites
echo.
echo Usage:
echo.
echo build.cmd ^<all^|proto^|build^|debug^|release^|diag^|compiler^|coreclr^|pcls^|vs^|ci^|ci_part1^|ci_part2^|microbuild^>
echo.
echo No arguments default to 'build' 
echo.
echo To specify multiple values, separate strings by comma
echo.
echo The example below run pcls, vs and qa:
echo.
echo build.cmd pcls,vs,debug
exit /b 1

:ARGUMENTS_OK

set BUILD_PROTO=0
set BUILD_NET40=1
set BUILD_CORECLR=0
set BUILD_PORTABLE=0
set BUILD_VS=0
set BUILD_CONFIG=release
set BUILD_CONFIG_LOWERCASE=release
set BUILD_DIAG=

set TEST_COMPILERUNIT=0
set TEST_NET40_COREUNIT=0
set TEST_CORECLR=0
set TEST_PORTABLE_COREUNIT=0
set TEST_VS=0
set TEST_FSHARP_SUITE=0
set TEST_FSHARPQA_SUITE=0
set TEST_TAGS=
set SKIP_EXPENSIVE_TESTS=1

setlocal enableDelayedExpansion
set /a counter=0
for /l %%x in (1 1 9) do (
    set /a counter=!counter!+1
    call :SET_CONFIG %%!counter! "!counter!"
)
for %%i in (%BUILD_FSC_DEFAULT%) do ( call :SET_CONFIG %%i )

setlocal disableDelayedExpansion
echo.

goto :MAIN

:SET_CONFIG
set ARG=%~1
if "%ARG%" == "1" if "%2" == "" (set ARG=build)
if "%2" == "" if not "%ARG%" == "build" goto :EOF

echo Parse argument %ARG%

if /i '%ARG%' == 'compiler' (set TEST_COMPILERUNIT=1)

if /i '%ARG%' == 'pcls' (
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'vs' (
    set BUILD_VS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'diag' (
    set BUILD_DIAG=/v:detailed
    if not defined APPVEYOR ( set BUILD_LOG=fsharp_build_log.log )
)

if /i '%ARG%' == 'all' (
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1

    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_CORECLR=1
    set TEST_VS=1

    set SKIP_EXPENSIVE_TESTS=0
)

if /i '%ARG%' == 'microbuild' (
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=0
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=1
    
    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_CORECLR=0
    set TEST_PORTABLE_COREUNIT=0
    set TEST_VS=0
    set TEST_FSHARP_SUITE=0
    set TEST_FSHARPQA_SUITE=0
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'proto' (
    set BUILD_PROTO=1
)

REM Same as 'all' but smoke testing only
if /i '%ARG%' == 'ci' (
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1

    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_CORECLR=1
    set TEST_VS=0
    set TEST_TAGS=
    set CONF_FSHARPQA_SUITE=Smoke
)


REM These divide 'ci' into two chunks which can be done in parallel
if /i '%ARG%' == 'ci_part1' (
    set BUILD_PROTO=1
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_CORECLR=0
    set BUILD_PORTABLE=1
    set BUILD_VS=1

    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=0
    set TEST_PORTABLE_COREUNIT=0
    set TEST_CORECLR=0
    set TEST_FSHARPQA_SUITE=1
    set TEST_FSHARP_SUITE=0
    set TEST_VS=1
    set TEST_TAGS=
)

if /i '%ARG%' == 'ci_part2' (
    set BUILD_PROTO=1
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1

    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_CORECLR=1
    set TEST_FSHARPQA_SUITE=0
    set TEST_FSHARP_SUITE=1
    set TEST_VS=0
    set TEST_TAGS=
)


if /i '%ARG%' == 'coreclr' (
    set BUILD_CORECLR=1
    set TEST_CORECLR=1
)

if /i '%ARG%' == 'debug' (
    set BUILD_CONFIG=debug
    set BUILD_CONFIG_LOWERCASE=debug
)

if /i '%ARG%' == 'build' (
    set BUILD_PORTABLE=1
    set BUILD_VS=1
)

if /i '%ARG%' == 'notests' (
    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_CORECLR=0
    set TEST_PORTABLE_COREUNIT=0
    set TEST_VS=0
    set TEST_FSHARP_SUITE=0
    set TEST_FSHARPQA_SUITE=0
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'test-smoke' (
    REM Smoke tests are a very small quick subset of tests

    set SKIP_EXPENSIVE_TESTS=1
    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=0
    set TEST_TAGS=Smoke
)

if /i '%ARG%' == 'test-fsharpqa' (
    set BUILD_NET40=1
    set TEST_FSHARPQA_SUITE=1
)

if /i '%ARG%' == 'test-compilerunit' (
    set BUILD_NET40=1
    set TEST_COMPILERUNIT=1
)

if /i '%ARG%' == 'test-coreunit' (
    set BUILD_NET40=1
    set TEST_NET40_COREUNIT=1
)

if /i '%ARG%' == 'test-coreclr' (
    set BUILD_CORECLR=1
    set TEST_CORECLR=1
)

if /i '%ARG%' == 'test-pcls' (
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'test-vs' (
    set BUILD_VS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'test-fsharp' (
    set TEST_FSHARP_SUITE=1
)

goto :EOF

:MAIN

REM after this point, ARG variable should not be used, use only BUILD_* or TEST_*

echo Build/Tests configuration:
echo.
echo BUILD_PROTO=%BUILD_PROTO%
echo BUILD_NET40=%BUILD_NET40%
echo BUILD_CORECLR=%BUILD_CORECLR%
echo BUILD_PORTABLE=%BUILD_PORTABLE%
echo BUILD_VS=%BUILD_VS%
echo BUILD_SETUP=%BUILD_SETUP%
echo BUILD_CONFIG=%BUILD_CONFIG%
echo BUILD_CONFIG_LOWERCASE=%BUILD_CONFIG_LOWERCASE%
echo.
echo TEST_COMPILERUNIT=%TEST_COMPILERUNIT%
echo TEST_NET40_COREUNIT=%TEST_NET40_COREUNIT%
echo TEST_PORTABLE_COREUNIT=%TEST_PORTABLE_COREUNIT%
echo TEST_VS=%TEST_VS%
echo TEST_FSHARP_SUITE=%TEST_FSHARP_SUITE%
echo TEST_FSHARPQA_SUITE=%TEST_FSHARPQA_SUITE%
echo TEST_TAGS=%TEST_TAGS%
echo SKIP_EXPENSIVE_TESTS=%SKIP_EXPENSIVE_TESTS%
echo.

if "%RestorePackages%"=="" ( 
    set RestorePackages=true
)

@echo on

call src\update.cmd signonly

:: Check prerequisites
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS150COMNTOOLS%..\ide\devenv.exe" set VisualStudioVersion=15.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 15.0\common7\ide\devenv.exe" set VisualStudioVersion=15.0
if exist "%ProgramFiles%\Microsoft Visual Studio 15.0\common7\ide\devenv.exe" set VisualStudioVersion=15.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS140COMNTOOLS%..\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe" set VisualStudioVersion=14.0
if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS120COMNTOOLS%..\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe" set VisualStudioVersion=12.0

:vsversionset
if '%VisualStudioVersion%' == '' echo Error: Could not find an installation of Visual Studio && goto :failure

if exist "%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe" set _msbuildexe="%ProgramFiles(x86)%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if exist "%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"      set _msbuildexe="%ProgramFiles%\MSBuild\%VisualStudioVersion%\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe. && goto :failure
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
set msbuildflags=/maxcpucount %_nrswitch%
set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

if '%RestorePackages%' == 'true' (
    %_ngenexe% install .\.nuget\NuGet.exe 

    .\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
    @if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure
)

:: Restore the Tools directory
call %~dp0init-tools.cmd

set _dotnetexe=%~dp0Tools\dotnetcli\dotnet.exe
pushd .\lkg & %_dotnetexe% restore &popd
@if ERRORLEVEL 1 echo Error: dotnet restore failed  && goto :failure

pushd .\lkg & %_dotnetexe% publish project.json &popd
@if ERRORLEVEL 1 echo Error: dotnet publish failed  && goto :failure

rem rename fsc and coreconsole to allow fsc.exe to to start compiler
pushd .\lkg\bin\debug\dnxcore50\win7-x64\publish
fc fsc.exe corehost.exe >nul
@if ERRORLEVEL 1 (
  copy fsc.exe fsc.dll
  copy corehost.exe fsc.exe
)
popd

rem rename fsc and coreconsole to allow fsc.exe to to start compiler
pushd .\lkg\bin\debug\dnxcore50\win7-x64\publish
fc fsi.exe corehost.exe >nul
@if ERRORLEVEL 1 (
  copy fsi.exe fsi.dll
  copy corehost.exe fsi.exe
)
popd

rem copy targestfile into tools directory ... temporary fix until packaging complete.
copy src\fsharp\FSharp.Build\Microsoft.FSharp.targets tools\Microsoft.FSharp.targets
copy src\fsharp\FSharp.Build\Microsoft.Portable.FSharp.targets tools\Microsoft.Portable.FSharp.targets

:: Build Proto
if NOT EXIST Proto\net40\bin\fsc-proto.exe (set BUILD_PROTO=1)

:: Build
if '%BUILD_PROTO%' == '1' (
    %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
    @if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

    %_ngenexe% install Proto\net40\bin\fsc-proto.exe
    @if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure
)

%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%
@if ERRORLEVEL 1 echo Error: '%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%' failed && goto :failure

@echo on
call src\update.cmd %BUILD_CONFIG_LOWERCASE% -ngen

pushd tests

if 'TEST_COMPILERUNIT' == '0' and 'TEST_PORTABLE_COREUNIT' == '0' and 'TEST_CORECLR' == '0' and 'TEST_VS' == '0' and 'TEST_FSHARP_SUITE' == '0' and 'TEST_FSHARPQA_SUITE' == '0' goto :finished

@echo on
call BuildTestTools.cmd %BUILD_CONFIG_LOWERCASE% 
@if ERRORLEVEL 1 echo Error: 'BuildTestTools.cmd %BUILD_CONFIG_LOWERCASE%' failed && goto :failed_tests

@echo on
if '%TEST_FSHARP_SUITE%' == '1' (
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=true
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharp %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\FSharpNunit_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharp %TEST_TAGS%' failed
        goto :failed_tests
    )
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=
)

if '%TEST_FSHARPQA_SUITE%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharpqa %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\fsharpqa_failures.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharpqa %TEST_TAGS%' failed
        goto :failed_tests
    )
)

if '%TEST_COMPILERUNIT%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% compilerunit %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\CompilerUnit_net40_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% compilerunit' failed
        goto :failed_tests
    )
)
if '%TEST_NET40_COREUNIT%' == '1' (
    if '%TEST_PORTABLE_COREUNIT%' == '1' (
        call RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitall %TEST_TAGS% 
        @if ERRORLEVEL 1 (
        @echo "type testresults\CoreUnit_net40_Error.log "
            type testresults\CoreUnit_net40_Error.log 
            echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunit' failed 
            goto :failed_tests
        )
    )
    if '%TEST_PORTABLE_COREUNIT%' == '0' (
        call RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunit %TEST_TAGS% 
        @if ERRORLEVEL 1 (
            type testresults\CoreUnit_Portable_Error.log
            echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunit' failed 
            goto :failed_tests
        )
    )
)
if '%TEST_NET40_COREUNIT%' == '0' (
    if '%TEST_PORTABLE_COREUNIT%' == '1' (
        call RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitall %TEST_TAGS% 
        @if ERRORLEVEL 1 (
            type testresults\CoreUnit_all_Error.log
            echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitall %TEST_TAGS%' failed 
            goto :failed_tests
        )
    )
)
if '%TEST_CORECLR%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitcoreclr %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\CoreUnit_coreclr_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitcoreclr %TEST_TAGS%' failed 
        goto :failed_tests
    )
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharp coreclr
    @if ERRORLEVEL 1 (
        type testresults\FSharp_Failures.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitcoreclr %TEST_TAGS%' failed 
        goto :failed_tests
    )
)
if '%TEST_VS%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% ideunit %TEST_TAGS% 
    @if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% ideunit  %TEST_TAGS%' failed && goto :failed_tests
)

:finished
@echo "Finished"
popd
goto :eof

:failed_tests
popd

:failure
exit /b 1
