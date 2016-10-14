@if "%_echo%"=="" echo off

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
echo           ^<ci^|ci_part1^|ci_part2^|microbuild^>
echo           ^<debug^|release^>
echo           ^<diag^>
echo           ^<publicsign^>
echo           ^<notests^|test-coreunit^|test-corecompile^|test-smoke^|test-coreclr^|test-pcls^|test-fsharp^|test-fsharpqa^|test-vs^>
echo.
echo No arguments default to 'default', meaning this (no testing)
echo.
echo     build.cmd net40 pcls vs notests
echo.
echo.Other examples:
echo.
echo.    build net40            (build)
echo.    build test-net40       (build and test)
echo.    build coreclr          (build)
echo.    build test-coreclr     (build and test)
echo.    build vs               (build)
echo.    build test-vs          (build and test)
echo.    build all              (build and test)
echo.
echo The example below run pcls, vs and qa:
echo.
echo     build.cmd pcls vs debug
exit /b 1

:ARGUMENTS_OK

set BUILD_PROTO_WITH_CORECLR_LKG=1

set BUILD_PROTO=0
set BUILD_NET40=0
set BUILD_CORECLR=0
set BUILD_PORTABLE=0
set BUILD_VS=0
set BUILD_CONFIG=release
set BUILD_CONFIG_LOWERCASE=release
set BUILD_DIAG=
set BUILD_PUBLICSIGN=0

set TEST_NET40_COMPILERUNIT=0
set TEST_NET40_COREUNIT=0
set TEST_CORECLR_COREUNIT=0
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

rem disable setup build by setting FSC_BUILD_SETUP=0
if /i '%FSC_BUILD_SETUP%' == '' (set FSC_BUILD_SETUP=1) 
goto :MAIN

:SET_CONFIG
set ARG=%~1
if "%ARG%" == "1" if "%2" == "" (set ARG=default)
if "%2" == "" if not "%ARG%" == "default" goto :EOF

echo Parse argument %ARG%


if /i '%ARG%' == 'default' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
)

if /i '%ARG%' == 'net40' (
    set BUILD_NET40=1
)

if /i '%ARG%' == 'coreclr' (
    set BUILD_CORECLR=1
)

if /i '%ARG%' == 'pcls' (
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'vs' (
    set BUILD_VS=1
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

if /i '%ARG%' == 'all' (
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%

    set TEST_NET40_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_CORECLR_COREUNIT=1
    set TEST_VS=1

    set SKIP_EXPENSIVE_TESTS=0
)

if /i '%ARG%' == 'protofx' (
    set BUILD_PROTO_WITH_CORECLR_LKG=0
    set BUILD_PROTO=1
)

if /i '%ARG%' == 'microbuild' (
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=0
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    
    set TEST_NET40_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_CORECLR_COREUNIT=0
    set TEST_PORTABLE_COREUNIT=1
    set TEST_VS=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'proto' (
    set BUILD_PROTO=1
)

REM Same as 'all' 
if /i '%ARG%' == 'ci' (
    set SKIP_EXPENSIVE_TESTS=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%

    set TEST_NET40_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_CORECLR_COREUNIT=1
    set TEST_VS=0
    set TEST_TAGS=
)


REM These divide 'ci' into two chunks which can be done in parallel
if /i '%ARG%' == 'ci_part1' (

    REM what we do
    set BUILD_PROTO=1
    set BUILD_PORTABLE=1
    set BUILD_VS=1
    set BUILD_SETUP=%FSC_BUILD_SETUP%
    set TEST_NET40_COMPILERUNIT=1
    set TEST_FSHARPQA_SUITE=1
    set TEST_VS=1

    REM what we don't do
    set BUILD_CORECLR=0
    set TEST_NET40_COREUNIT=0
    set TEST_PORTABLE_COREUNIT=0
    set TEST_CORECLR_COREUNIT=0
    set TEST_FSHARP_SUITE=0
    set TEST_TAGS=
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'ci_part2' (

    REM what we do
	set BUILD_PROTO=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1
    set TEST_NET40_COREUNIT=1
    set TEST_PORTABLE_COREUNIT=1
    set TEST_CORECLR_COREUNIT=1
    set TEST_FSHARP_SUITE=1

    REM what we don't do
    set TEST_NET40_COMPILERUNIT=0
    set TEST_FSHARPQA_SUITE=0
    set TEST_VS=0
    set TEST_TAGS=
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'test-smoke' (
    REM Smoke tests are a very small quick subset of tests

    REM what we do
    set TEST_FSHARP_SUITE=1
    set TEST_TAGS=Smoke

    REM what we don't do
    set TEST_NET40_COMPILERUNIT=0
    set TEST_NET40_COREUNIT=0
    set TEST_FSHARPQA_SUITE=0
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'test-fsharpqa' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_FSHARPQA_SUITE=1
)

if /i '%ARG%' == 'test-compilerunit' (
    set BUILD_NET40=1
    set TEST_NET40_COMPILERUNIT=1
)

if /i '%ARG%' == 'test-net40' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_NET40_COMPILERUNIT=1
    set TEST_NET40_COREUNIT=1
    set TEST_FSHARP_SUITE=1
    set TEST_FSHARPQA_SUITE=1
    set SKIP_EXPENSIVE_TESTS=1
)

if /i '%ARG%' == 'test-coreunit' (
    set BUILD_NET40=1
    set TEST_NET40_COREUNIT=1
)

if /i '%ARG%' == 'test-coreclr' (
    set BUILD_CORECLR=1
    set TEST_CORECLR_COREUNIT=1

)

if /i '%ARG%' == 'test-pcls' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT=1
)

if /i '%ARG%' == 'test-vs' (
    set BUILD_NET40=1
    set BUILD_VS=1
    set TEST_VS=1
)

if /i '%ARG%' == 'test-fsharp' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_FSHARP_SUITE=1
)

if /i '%ARG%' == 'publicsign' (
    set BUILD_NET40=1
    set BUILD_PUBLICSIGN=1
)

goto :EOF

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
echo TEST_NET40_COMPILERUNIT=%TEST_NET40_COMPILERUNIT%
echo TEST_NET40_COREUNIT=%TEST_NET40_COREUNIT%
echo TEST_CORECLR_COREUNIT=%TEST_CORECLR_COREUNIT%
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
if not '%VisualStudioVersion%' == '' goto vsversionset

if not '%VisualStudioVersion%' == '' goto vsversionset
if exist "%VS150COMNTOOLS%..\..\ide\devenv.exe" set VisualStudioVersion=15.0
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

if exist "%VS150COMNTOOLS%..\..\MSBuild\15.0\Bin\MSBuild.exe" (
    set _msbuildexe="%VS150COMNTOOLS%..\..\MSBuild\15.0\Bin\MSBuild.exe"
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
set msbuildflags=/maxcpucount %_nrswitch%
set _ngenexe="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\ngen.exe"
if not exist %_ngenexe% echo Error: Could not find ngen.exe. && goto :failure

if '%RestorePackages%' == 'true' (
    %_ngenexe% install .\.nuget\NuGet.exe  /nologo 

    .\.nuget\NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget\nuget.config
    @if ERRORLEVEL 1 echo Error: Nuget restore failed  && goto :failure
)

if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '1' (
    :: Restore the Tools directory
    call %~dp0init-tools.cmd
)

set _dotnetexe=%~dp0Tools\dotnetcli\dotnet.exe

set _fsiexe="packages\FSharp.Compiler.Tools.4.0.1.10\tools\fsi.exe"
if not exist %_fsiexe% echo Error: Could not find %_fsiexe% && goto :failure
%_ngenexe% install %_fsiexe% /nologo 

set _nugetexe=".nuget\nuget.exe"
set _nugetconfig=".nuget\nuget.config"
if not exist %_nugetexe% echo Error: Could not find %_nugetexe% && goto :failure
%_ngenexe% install %_nugetexe% /nologo 

if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '1' (

    pushd .\lkg & %_dotnetexe% restore &popd
    @if ERRORLEVEL 1 echo Error: dotnet restore failed  && goto :failure

    pushd .\lkg & %_dotnetexe% publish project.json -o %~dp0\Tools\lkg -r win7-x64 &popd
    @if ERRORLEVEL 1 echo Error: dotnet publish failed  && goto :failure
)

if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '0' (
    %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
    rmdir /s /q %~dp0\Tools\lkg
)

rem copy targestfile into tools directory ... temporary fix until packaging complete.
copy src\fsharp\FSharp.Build\Microsoft.FSharp.targets tools\Microsoft.FSharp.targets
copy src\fsharp\FSharp.Build\Microsoft.Portable.FSharp.targets tools\Microsoft.Portable.FSharp.targets

:: Build Proto
if NOT EXIST Proto\net40\bin\fsc-proto.exe (set BUILD_PROTO=1)

:: Build
if '%BUILD_PROTO%' == '1' (
    %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
    @if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

    %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
    @if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure
)


echo ---------------- Done with proto, starting build ------------------------------------------

%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG% /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%
@if ERRORLEVEL 1 echo Error: '%_msbuildexe% %msbuildflags% build-everything.proj /p:Configuration=%BUILD_CONFIG% %BUILD_DIAG%  /p:BUILD_PUBLICSIGN=%BUILD_PUBLICSIGN%' failed && goto :failure


echo ---------------- Done with build, starting update/prepare ------------------------------------------

if 'BUILD_NET40' == '1' (
    call src\update.cmd %BUILD_CONFIG% -ngen
)

@echo set NUNITPATH=packages\NUnit.Console.3.0.0\tools\
set NUNITPATH=packages\NUnit.Console.3.0.0\tools\
if not exist %NUNITPATH% echo Error: Could not find %NUNITPATH% && goto :failure

@echo xcopy "%NUNITPATH%*.*"  "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
      xcopy "%NUNITPATH%*.*"  "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y

@echo xcopy "%~dp0tests\fsharpqa\testenv\src\nunit*.*" "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y
      xcopy "%~dp0tests\fsharpqa\testenv\src\nunit*.*" "%~dp0tests\fsharpqa\testenv\bin\nunit\*.*" /S /Q /Y

if '%BUILD_CORECLR%' == '1' (

  echo Restoring CoreCLR packages and runtimes necessary for actually running and testing
  %_nugetexe% restore .\tests\fsharp\project.json -PackagesDirectory packages  
  
  echo Deploy x86 version of compiler and dependencies, ready for testing
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x86 --copyCompiler:yes --v:quiet
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x86 --copyCompiler:no --v:quiet

  echo Deploy x64 version of compiler, ready for testing
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x64 --copyCompiler:yes --v:quiet
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x64 --copyCompiler:no --v:quiet

  echo Deploy linux version of built compiler, ready for testing
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\ubuntu.14.04-x64 --copyCompiler:yes --v:quiet
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\ubuntu.14.04-x64 --copyCompiler:no --v:quiet

  echo Deploy osx version of built compiler, ready for testing
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\osx.10.10-x64 --copyCompiler:yes --v:quiet
  %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\osx.10.10-x64 --copyCompiler:no --v:quiet

)


if 'TEST_NET40_COMPILERUNIT' == '0' and 'TEST_PORTABLE_COREUNIT' == '0' and 'TEST_CORECLR_COREUNIT' == '0' and 'TEST_VS' == '0' and 'TEST_FSHARP_SUITE' == '0' and 'TEST_FSHARPQA_SUITE' == '0' goto :finished

echo ---------------- Done with update, starting tests ------------------------------------------


pushd tests

if '%TEST_FSHARP_SUITE%' == '1' (
    call RunTests.cmd %BUILD_CONFIG% fsharp %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\FSharpNunit_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG% fsharp %TEST_TAGS%' failed
        goto :failed_tests
    )
)

if '%TEST_FSHARPQA_SUITE%' == '1' (
    call RunTests.cmd %BUILD_CONFIG% fsharpqa %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\fsharpqa_failures.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG% fsharpqa %TEST_TAGS%' failed
        goto :failed_tests
    )
)

if '%TEST_NET40_COMPILERUNIT%' == '1' (
    call RunTests.cmd %BUILD_CONFIG% compilerunit %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\CompilerUnit_net40_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG% compilerunit' failed
        goto :failed_tests
    )
)
if '%TEST_NET40_COREUNIT%' == '1' (
    if '%TEST_PORTABLE_COREUNIT%' == '1' (
        call RunTests.cmd %BUILD_CONFIG% coreunitall %TEST_TAGS% 
        @if ERRORLEVEL 1 (
        @echo "type testresults\CoreUnit_net40_Error.log "
            type testresults\CoreUnit_net40_Error.log 
            echo Error: 'RunTests.cmd %BUILD_CONFIG% coreunit' failed 
            goto :failed_tests
        )
    )
    if '%TEST_PORTABLE_COREUNIT%' == '0' (
        call RunTests.cmd %BUILD_CONFIG% coreunit %TEST_TAGS% 
        @if ERRORLEVEL 1 (
            type testresults\CoreUnit_Portable_Error.log
            echo Error: 'RunTests.cmd %BUILD_CONFIG% coreunit' failed 
            goto :failed_tests
        )
    )
)
if '%TEST_NET40_COREUNIT%' == '0' (
    if '%TEST_PORTABLE_COREUNIT%' == '1' (
        call RunTests.cmd %BUILD_CONFIG% coreunitall %TEST_TAGS% 
        @if ERRORLEVEL 1 (
            type testresults\CoreUnit_all_Error.log
            echo Error: 'RunTests.cmd %BUILD_CONFIG% coreunitall %TEST_TAGS%' failed 
            goto :failed_tests
        )
    )
)

if '%TEST_CORECLR_COREUNIT%' == '1' (


    call RunTests.cmd %BUILD_CONFIG% coreunitcoreclr %TEST_TAGS% 
    @if ERRORLEVEL 1 (
        type testresults\CoreUnit_coreclr_Error.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG% coreunitcoreclr %TEST_TAGS%' failed 
        goto :failed_tests
    )
    call RunTests.cmd %BUILD_CONFIG% fsharp coreclr
    @if ERRORLEVEL 1 (
        type testresults\FSharp_Failures.log
        echo Error: 'RunTests.cmd %BUILD_CONFIG% coreunitcoreclr %TEST_TAGS%' failed 
        goto :failed_tests
    )
)
if '%TEST_VS%' == '1' (
    call RunTests.cmd %BUILD_CONFIG% ideunit %TEST_TAGS% 
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
