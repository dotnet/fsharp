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
echo build.cmd ^<all^|proto^|build^|debug^|release^|diag^|compiler^|coreclr^|pcls^|vs^|ci^|ci_part1^|ci_part2^>
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
set BUILD_NET40=0
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
    set BUILD_TEST_TOOLS=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'vs' (
    set BUILD_VS=1
    set BUILD_TEST_TOOLS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'diag' (
    set BUILD_DIAG=/v:diag
)

if /i '%ARG%' == 'all' (
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_TEST_TOOLS=1

    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_CORECLR=1
    set TEST_VS=1

    set SKIP_EXPENSIVE_TESTS=0
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
    set BUILD_TEST_TOOLS=1

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

REM These divide 'ci' into three chunks which can be done in parallel
if /i '%ARG%' == 'ci_part1' (
    set BUILD_PROTO=1
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_CORECLR=0
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_TEST_TOOLS=1

    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_CORECLR=0
    set TEST_TAGS=
    set TEST_VS=1
)

if /i '%ARG%' == 'ci_part2' (
    set BUILD_PROTO=1
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_TEST_TOOLS=1
    set TEST_CORECLR=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_FSHARP_SUITE=1
)

if /i '%ARG%' == 'coreclr' (
    set BUILD_CORECLR=1
    set BUILD_TEST_TOOLS=1
    set TEST_CORECLR=1
)

if /i '%ARG%' == 'net40' (
    set BUILD_NET40=1
    set BUILD_TEST_TOOLS=1
    set TEST_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
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
    set BUILD_TEST_TOOLS=0
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

    set BUILD_TEST_TOOLS=1
    set SKIP_EXPENSIVE_TESTS=1
    set TEST_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=0
    set TEST_TAGS=Smoke
)

if /i '%ARG%' == 'test-fsharpqa' (
    set BUILD_NET40=1
    set BUILD_TEST_TOOLS=1
    set TEST_FSHARPQA_SUITE=1
)

if /i '%ARG%' == 'test-compilerunit' (
    set BUILD_NET40=1
    set BUILD_TEST_TOOLS=1
    set TEST_COMPILERUNIT=1
)

if /i '%ARG%' == 'test-coreunit' (
    set BUILD_NET40=1
    set BUILD_TEST_TOOLS=1
    set TEST_NET40_COREUNIT=1
)

if /i '%ARG%' == 'test-coreclr' (
    set BUILD_CORECLR=1
    set BUILD_TEST_TOOLS=1
    set TEST_CORECLR=1
)

if /i '%ARG%' == 'test-pcls' (
    set BUILD_PORTABLE=1
    set BUILD_TEST_TOOLS=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'test-vs' (
    set BUILD_VS=1
    set BUILD_TEST_TOOLS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'test-fsharp' (
    set BUILD_TEST_TOOLS=1
    set TEST_FSHARP_SUITE=1
)

goto :EOF

:MAIN
rem always build at least one version of the compiler --- desktop is most convenient for now 
if '%BUILD_PROTO%' == '0' (
    if '%BUILD_NET40%' == '0' (
        if '%BUILD_CORECLR%' == '0' ( 
            @echo no compiler specified default to desktop
            set BUILD_NET40=1
        )
    )
)

REM after this point, ARG variable should not be used, use only BUILD_* or TEST_*

echo Build/Tests configuration:
echo.
echo BUILD_PROTO=%BUILD_PROTO%
echo BUILD_NET40=%BUILD_NET40%
echo BUILD_CORECLR=%BUILD_CORECLR%
echo BUILD_PORTABLE=%BUILD_PORTABLE%
echo BUILD_VS=%BUILD_VS%
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

rem ============================
rem publish the lkg / crossgen
rem ============================
%_dotnetexe% restore .\lkg\project.json --configfile ..\.nuget\nuget.config
@if ERRORLEVEL 1 echo Error: %_dotnetexe% restore .\lkg\project.json --configfile ..\.nuget\nuget.config   failed  && goto :failure
%_dotnetexe% publish .\lkg\project.json --no-build --configuration release -f dnxcore50 -r win7-x64 -o Tools\lkg
@if ERRORLEVEL 1 echo Error: %_dotnetexe% publish .\lkg\project.json --no-build --configuration release -f dnxcore50 -r win7-x64  -o tools\lkg   failed && goto :failure

rem rename fsc and corehost.exe to allow fsc.exe to to start compiler
pushd .\Tools\lkg
fc fsc.exe coreconsole.exe >nul
@if ERRORLEVEL 1 (
  copy fsc.exe fsc.dll
  copy coreconsole.exe fsc.exe
)
fc fsi.exe coreconsole.exe >nul
@if ERRORLEVEL 1 (
  copy fsi.exe fsi.dll
  copy coreconsole.exe fsi.exe
)
popd

rem copy targetfile into tools directory ... temporary fix until packaging complete
copy src\fsharp\FSharp.Build\Microsoft.FSharp.targets tools\Microsoft.FSharp.targets
copy src\fsharp\FSharp.Build\Microsoft.Portable.FSharp.targets tools\Microsoft.Portable.FSharp.targets
copy lkg\FSharp-14.0.23413.0\bin\fsharp.core.dll tools\fsharp.core.dll
copy lkg\FSharp-14.0.23413.0\bin\fsharp.build.dll tools\fsharp.build.dll

:: Build Proto
if NOT EXIST Proto\bin\fsc.dll (set BUILD_PROTO=1)


rem The buildtools currently do not deploy crossgen to the tools directory it should.  Note "1.0.2-rc2-24027"
if NOT EXIST Tools\crossgen.exe (
    copy packages\runtime.win7-x64.Microsoft.NETCore.Runtime.CoreCLR\1.0.2-rc2-24027\tools\crossgen.exe Tools
)

:: Build
if '%BUILD_PROTO%' == '1' (
    %_dotnetexe% restore  .\Proto\project.json --configfile ..\.nuget\nuget.config
    @if ERRORLEVEL 1 echo Error:     %_dotnetexe% restore  --configfile ..\.nuget\nuget.config .\Proto\project.json failed  && goto :failure

    %_dotnetexe% publish .\Proto\project.json --no-build --configuration release -f .NETStandard,Version=v1.5 -r win7-x64 -o Proto\bin
    @if ERRORLEVEL 1 echo Error: %_dotnetexe% publish src\fsharp\Fsc\project.json --no-build --configuration release -f .NETStandard,Version=v1.5 -r win7-x64 -o Proto\bin    failed  && goto :failure

    pushd .\Proto\bin
    copy coreconsole.exe fsc.exe
    copy coreconsole.exe fsi.exe
    popd
)

%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%
@if ERRORLEVEL 1 echo Error: '%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%' failed && goto :failure

@echo on
REM call src\update.cmd %BUILD_CONFIG_LOWERCASE% -ngen

pushd tests

if '%BUILD_TEST_TOOLS%' == '1' (
    @echo on
    call BuildTestTools.cmd %BUILD_CONFIG_LOWERCASE% 
    @if ERRORLEVEL 1 echo Error: 'BuildTestTools.cmd %BUILD_CONFIG_LOWERCASE%' failed && goto :failed_tests
)

@if "%_echo%"=="" echo off
if '%TEST_FSHARP_SUITE%' == '1' (
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=true
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharp %TEST_TAGS% 
    if ERRORLEVEL 1 (
        type testresults\FSharpNunit_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharp %TEST_TAGS%' failed
        goto :failed_tests
    )
    set FSHARP_TEST_SUITE_USE_NUNIT_RUNNER=
)

if '%TEST_FSHARPQA_SUITE%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharpqa %TEST_TAGS% 
    if ERRORLEVEL 1 (
        type testresults\fsharpqa_failures.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% fsharpqa %TEST_TAGS%' failed
        goto :failed_tests
    )
)

if '%TEST_COMPILERUNIT%' == '1' (
    call RunTests.cmd %BUILD_CONFIG_LOWERCASE% compilerunit %TEST_TAGS% 
    if ERRORLEVEL 1 (
        type testresults\CompilerUnit_net40_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWERCASE% compilerunit' failed
        goto :failed_tests
    )
)

if '%TEST_NET40_COREUNIT%' == '1' (
    if '%TEST_PORTABLE_COREUNIT%' == '1' (
        call RunTests.cmd %BUILD_CONFIG_LOWERCASE% coreunitall %TEST_TAGS% 
        if ERRORLEVEL 1 (
        echo "type testresults\CoreUnit_net40_Error.log "
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
    if ERRORLEVEL 1 echo Error: 'RunTests.cmd %BUILD_CONFIG_LOWER% ideunit  %TEST_TAGS%' failed && goto :failed_tests
)

:finished
echo "Finished"
popd
goto :eof

:failed_tests
popd

:failure
exit /b 1
