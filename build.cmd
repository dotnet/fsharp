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
echo           ^<ci^|ci_part1^|ci_part2^|microbuild^>
echo           ^<debug^|release^>
echo           ^<diag^|publicsign^>
echo           ^<test^|test-net40-coreunit^|test-coreclr-coreunit^|test-compiler-unit^|test-pcl-coreunit^|test-net40-fsharp^|test-net40-fsharpqa^>
echo           ^<include tag^|exclude tag^>
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
echo.    build.cmd nobuild test include Conformance (tests marked with Conformance tag)
echo.    build.cmd nobuild test exclude Slow (no tests marked with Slow tag)
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
set EXCLUDE_TEST_SPEC_NUNIT=cat == Expensive
set INCLUDE_TEST_TAGS=
set EXCLUDE_TEST_TAGS=Expensive


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

)

if /i '%ARG%' == 'microbuild' (
    set _autoselect=0
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=0
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

)

if /i '%ARG%' == 'ci_part2' (
    set _autoselect=0

    REM what we do
    set BUILD_PROTO_WITH_CORECLR_LKG=1
    set BUILD_PROTO=1
    set BUILD_NET40=1
    set BUILD_CORECLR=1
    set BUILD_PORTABLE=1

    set TEST_NET40_COREUNIT_SUITE=1
    set TEST_NET40_FSHARP_SUITE=1
    set TEST_PORTABLE_COREUNIT_SUITE=1
    set TEST_CORECLR_COREUNIT_SUITE=1

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
if /i '%ARG%' == 'exclude' (
    set /a counter=!counter!+1
	if '!EXCLUDE_TEST_SPEC_NUNIT!' == '' ( set EXCLUDE_TEST_SPEC_NUNIT=cat == %ARG2% ) else (set EXCLUDE_TEST_SPEC_NUNIT=cat == %ARG2% or !EXCLUDE_TEST_SPEC_NUNIT! )
	if '!EXCLUDE_TEST_TAGS!' == '' ( set EXCLUDE_TEST_TAGS=%ARG2% ) else (set EXCLUDE_TEST_TAGS=%ARG2%;!EXCLUDE_TEST_TAGS! )
)
if /i '%ARG%' == 'noskip' (
	set EXCLUDE_TEST_SPEC_NUNIT=
	set EXCLUDE_TEST_TAGS=
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

    set EXCLUDE_TEST_TAGS=
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
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_PORTABLE_COREUNIT_SUITE=1
)


if /i '%ARG%' == 'test-net40-fsharp' (
    set BUILD_NET40=1
    set BUILD_PORTABLE=1
    set TEST_NET40_FSHARP_SUITE=1
)

if /i '%ARG%' == 'test-coreclr-fsharp' (
    set BUILD_CORECLR=1
    set TEST_CORECLR_FSHARP_SUITE=1
)

if /i '%ARG%' == 'publicsign' (
    set BUILD_PUBLICSIGN=1
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
echo EXCLUDE_TEST_SPEC_NUNIT=%EXCLUDE_TEST_SPEC_NUNIT%
echo EXCLUDE_TEST_TAGS=%EXCLUDE_TEST_TAGS%
echo INCLUDE_TEST_TAGS=%INCLUDE_TEST_TAGS%

echo.

echo ---------------- Done with arguments, starting preparation -----------------

if "%RestorePackages%"=="" ( 
    set RestorePackages=true
)

@echo on

@call src\update.cmd signonly

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
    :: Restore the tools directory
    call %~dp0init-tools.cmd
)

set _dotnetexe=%~dp0Tools\dotnetcli\dotnet.exe

set _fsiexe="packages\FSharp.Compiler.Tools.4.0.1.19\tools\fsi.exe"
if not exist %_fsiexe% echo Error: Could not find %_fsiexe% && goto :failure
%_ngenexe% install %_fsiexe% /nologo 

if not exist %_nugetexe% echo Error: Could not find %_nugetexe% && goto :failure
%_ngenexe% install %_nugetexe% /nologo 

echo ---------------- Done with package restore, starting proto ------------------------

rem Decide if Proto need building
if NOT EXIST Proto\net40\bin\fsc-proto.exe (
  set BUILD_PROTO=1
)


rem Build Proto
if '%BUILD_PROTO%' == '1' (
  rmdir /s /q Proto

  if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '1' (

    pushd .\lkg & %_dotnetexe% restore --packages %~dp0\packages &popd
    @if ERRORLEVEL 1 echo Error: dotnet restore failed  && goto :failure

    pushd .\lkg & %_dotnetexe% publish project.json -o %~dp0\Tools\lkg -r win7-x64 &popd
    @if ERRORLEVEL 1 echo Error: dotnet publish failed  && goto :failure

    echo %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
         %_msbuildexe% %msbuildflags% src\fsharp-proto-build.proj
    @if ERRORLEVEL 1 echo Error: compiler proto build failed && goto :failure

    echo %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
         %_ngenexe% install Proto\net40\bin\fsc-proto.exe /nologo 
    @if ERRORLEVEL 1 echo Error: NGen of proto failed  && goto :failure

  )

  if '%BUILD_PROTO_WITH_CORECLR_LKG%' == '0' (

    echo %_ngenexe% install packages\FSharp.Compiler.Tools.4.0.1.19\tools\fsc.exe /nologo 
         %_ngenexe% install packages\FSharp.Compiler.Tools.4.0.1.19\tools\fsc.exe /nologo 

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

if '%BUILD_CORECLR%' == '1' (
  echo Restoring CoreCLR packages and runtimes necessary for actually running and testing
  echo  %_nugetexe% restore .\tests\fsharp\project.json -PackagesDirectory packages -ConfigFile %_nugetconfig%
  %_nugetexe% restore .\tests\fsharp\project.json -PackagesDirectory packages -ConfigFile %_nugetconfig%
  
  echo Deploy x86 compiler to tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x86, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x86 --copyCompiler:yes --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x86 --copyCompiler:yes --v:quiet
  echo Deploy x86 runtime and FSharp.Core library to tests\testbin\%BUILD_CONFIG%\coreclr\win7-x86, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x86 --copyCompiler:no --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x86 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x86 --copyCompiler:no --v:quiet

  echo Deploy x64 compiler to tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x64 --copyCompiler:yes --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\win7-x64 --copyCompiler:yes --v:quiet
  echo Deploy x64 runtime and FSharp.Core library to tests\testbin\%BUILD_CONFIG%\coreclr\win7-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x64 --copyCompiler:no --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/win7-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\win7-x64 --copyCompiler:no --v:quiet

  echo Deploy linux compiler to tests\testbin\%BUILD_CONFIG%\coreclr\fsc\ubuntu.14.04-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\ubuntu.14.04-x64 --copyCompiler:yes --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\ubuntu.14.04-x64 --copyCompiler:yes --v:quiet
  REM echo Deploy linux runtime and FSharp.Core library to tests\testbin\%BUILD_CONFIG%\coreclr\ubuntu.14.04-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\ubuntu.14.04-x64 --copyCompiler:no --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/ubuntu.14.04-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\ubuntu.14.04-x64 --copyCompiler:no --v:quiet

  echo Deploy OSX compiler to tests\testbin\%BUILD_CONFIG%\coreclr\fsc\osx.10.10-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\osx.10.10-x64 --copyCompiler:yes --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\fsc\osx.10.10-x64 --copyCompiler:yes --v:quiet
  echo Deploy OSX runtime and FSharp.Core library to tests\testbin\%BUILD_CONFIG%\coreclr\osx.10.10-x64, ready for testing
  echo %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\osx.10.10-x64 --copyCompiler:no --v:quiet
       %_fsiexe% --exec tests\fsharpqa\testenv\src\DeployProj\DeployProj.fsx --targetPlatformName:.NETStandard,Version=v1.6/osx.10.10-x64 --projectJsonLock:%~dp0tests\fsharp\project.lock.json --packagesDir:%~dp0\packages --fsharpCore:%BUILD_CONFIG%\coreclr\bin\FSharp.Core.dll --output:tests\testbin\%BUILD_CONFIG%\coreclr\osx.10.10-x64 --copyCompiler:no --v:quiet
)


if 'TEST_NET40_COMPILERUNIT_SUITE' == '0' and 'TEST_PORTABLE_COREUNIT_SUITE' == '0' and 'TEST_CORECLR_COREUNIT_SUITE' == '0' and 'TEST_VS_IDEUNIT_SUITE' == '0' and 'TEST_NET40_FSHARP_SUITE' == '0' and 'TEST_NET40_FSHARPQA_SUITE' == '0' goto :success

echo ---------------- Done with update, starting tests -----------------------


pushd tests


rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
rem Note: each setlocal must be matched by an executed endlocal
setlocal disableDelayedExpansion
if "%INCLUDE_TEST_SPEC_NUNIT%" == "" (
    if NOT "%EXCLUDE_TEST_SPEC_NUNIT%" == "" (
        set WHERE_ARG_NUNIT=--where "!(%EXCLUDE_TEST_SPEC_NUNIT%)"
	)
)
if NOT "%INCLUDE_TEST_SPEC_NUNIT%" == "" (
    if "%EXCLUDE_TEST_SPEC_NUNIT%" == "" (
        set WHERE_ARG_NUNIT=--where "%INCLUDE_TEST_SPEC_NUNIT%"
	)
    if NOT "%EXCLUDE_TEST_SPEC_NUNIT%" == "" (
		set WHERE_ARG_NUNIT=--where "%INCLUDE_TEST_SPEC_NUNIT% and !(%EXCLUDE_TEST_SPEC_NUNIT%)"
	)
)
if NOT "%INCLUDE_TEST_TAGS%" == "" (
    set INCLUDE_ARG_RUNALL=-ttags:%INCLUDE_TEST_TAGS%
)
if NOT "%EXCLUDE_TEST_TAGS%" == "" (
    set EXCLUDE_ARG_RUNALL=-nottags:%EXCLUDE_TEST_TAGS%
)
echo WHERE_ARG_NUNIT=%WHERE_ARG_NUNIT%
rem Re-enable delayed expansion. We can't use endlocal here since we want to keep the variables we've computed.
rem Note: each setlocal must be matched by an executed endlocal
setlocal enableDelayedExpansion


set NUNITPATH=%~dp0tests\fsharpqa\testenv\bin\nunit\
set NUNIT3_CONSOLE=%~dp0packages\NUnit.Console.3.0.0\tools\nunit3-console.exe
set link_exe=%~dp0packages\VisualCppTools.14.0.24519-Pre\lib\native\bin\link.exe
if not exist "%link_exe%" (
    echo Error: failed to find '%link_exe%' use nuget to restore the VisualCppTools package
    goto :failed_tests
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


set XMLFILE=%RESULTSDIR%\test-net40-fsharp-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-net40-fsharp-output.log
set ERRORFILE=%RESULTSDIR%\test-net40-fsharp-errors.log

set command="%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\FSharp.Tests.FSharp.dll" --framework:V4.0 --work:"%FSCBINPATH%"  --output:"!OUTPUTFILE!" --err:"!ERRORFILE!" --result:"!XMLFILE!;format=nunit3" 

if '%TEST_NET40_FSHARP_SUITE%' == '1' (
    rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
    rem Note: each setlocal must be matched by an executed endlocal
    setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 

    call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"

    if NOT '!saved_errorlevel!' == '0' (
        type "!ERRORFILE!"
        echo Error: 'Running tests net40-fsharp' failed
        goto :failed_tests
    )
)


REM ---------------- net40-fsharpqa  -----------------------

set OSARCH=%PROCESSOR_ARCHITECTURE%

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
set HOSTED_COMPILER=1

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set SYSWOW64=.
if "%OSARCH%"=="AMD64" set SYSWOW64=SysWoW64

if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

							FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B

set PATH=%PATH%;%WINSDKNETFXTOOLS%
for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

set REGEXE32BIT=reg.exe

IF NOT DEFINED SNEXE32  IF EXIST "%WINSDKNETFXTOOLS%sn.exe"               set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64  IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"           set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe
IF NOT DEFINED ildasm   IF EXIST "%WINSDKNETFXTOOLS%ildasm.exe"           set ildasm=%WINSDKNETFXTOOLS%ildasm.exe


if '%TEST_NET40_FSHARPQA_SUITE%' == '1' (

	set FSC=!FSCBINPATH!\fsc.exe
	set PATH=!FSCBINPATH!;!PATH!

	set FSCVPREVBINPATH=!X86_PROGRAMFILES!\Microsoft SDKs\F#\4.0\Framework\v4.0
	set FSCVPREV=!FSCVPREVBINPATH!\fsc.exe

	REM == VS-installed paths to FSharp.Core.dll
	set FSCOREDLLPATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.1.0
	set FSCOREDLL20PATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v2.0\2.3.0.0
	set FSCOREDLLPORTABLEPATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.41.0
	set FSCOREDLLNETCOREPATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.41.0
	set FSCOREDLLNETCORE78PATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.41.0
	set FSCOREDLLNETCORE259PATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.41.0
	set FSDATATPPATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers
	set FSCOREDLLVPREVPATH=!X86_PROGRAMFILES!\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0

	REM == open source logic
	if exist "!FSCBinPath!\FSharp.Core.dll" set FSCOREDLLPATH=!FSCBinPath!
	if exist "!FSCBinPath!\..\..\net20\bin\FSharp.Core.dll" set FSCOREDLL20PATH=!FSCBinPath!\..\..\net20\bin
	if exist "!FSCBinPath!\..\..\portable47\bin\FSharp.Core.dll" set FSCOREDLLPORTABLEPATH=!FSCBinPath!\..\..\portable47\bin
	if exist "!FSCBinPath!\..\..\portable7\bin\FSharp.Core.dll" set FSCOREDLLNETCOREPATH=!FSCBinPath!\..\..\portable7\bin
	IF exist "!FSCBinPath!\..\..\portable78\bin\FSharp.Core.dll" set FSCOREDLLNETCORE78PATH=!FSCBinPath!\..\..\portable78\bin
	IF exist "!FSCBinPath!\..\..\portable259\bin\FSharp.Core.dll" set FSCOREDLLNETCORE259PATH=!FSCBinPath!\..\..\portable259\bin

	set FSCOREDLLPATH=!FSCOREDLLPATH!\FSharp.Core.dll
	set FSCOREDLL20PATH=!FSCOREDLL20PATH!\FSharp.Core.dll
	set FSCOREDLLPORTABLEPATH=!FSCOREDLLPORTABLEPATH!\FSharp.Core.dll
	set FSCOREDLLNETCOREPATH=!FSCOREDLLNETCOREPATH!\FSharp.Core.dll
	set FSCOREDLLNETCORE78PATH=!FSCOREDLLNETCORE78PATH!\FSharp.Core.dll
	set FSCOREDLLNETCORE259PATH=!FSCOREDLLNETCORE259PATH!\FSharp.Core.dll
	set FSCOREDLLVPREVPATH=!FSCOREDLLVPREVPATH!\FSharp.Core.dll

	where.exe perl > NUL 2> NUL
	if errorlevel 1 (
		echo Error: perl is not in the PATH, it is required for the net40-fsharpqa test suite
		goto :failed_tests
	)

	set OUTPUTFILE=test-net40-fsharpqa-results.log
	set ERRORFILE=test-net40-fsharpqa-errors.log
	set FAILENV=test-net40-fsharpqa-errors


	pushd %~dp0tests\fsharpqa\source
	echo perl %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no %INCLUDE_ARG_RUNALL% %EXCLUDE_ARG_RUNALL% %PARALLEL_ARG%
		 perl %~dp0tests\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results !OUTPUTFILE! -log !ERRORFILE! -fail !FAILENV! -cleanup:no %INCLUDE_ARG_RUNALL% %EXCLUDE_ARG_RUNALL% %PARALLEL_ARG%

	popd
    if ERRORLEVEL 1 (
        type "%RESULTSDIR%\!OUTPUTFILE!"
        type "%RESULTSDIR%\!ERRORFILE!"
        echo Error: 'Running tests net40-fsharpqa' failed
        goto :failed_tests
    )
)

REM ---------------- net40-compilerunit  -----------------------

set XMLFILE=%RESULTSDIR%\test-net40-compilerunit-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-net40-compilerunit-output.log
set ERRORFILE=%RESULTSDIR%\test-net40-compilerunit-errors.log
set command="%NUNIT3_CONSOLE%" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" --output:"!OUTPUTFILE!" --err:"!ERRORFILE!" --work:"%FSCBINPATH%" "%FSCBINPATH%\..\..\net40\bin\FSharp.Compiler.Unittests.dll"
if '%TEST_NET40_COMPILERUNIT_SUITE%' == '1' (

    rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
    rem Note: each setlocal must be matched by an executed endlocal
    setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 

    call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"
    if NOT '!saved_errorlevel!' == '0' (
        type "!OUTPUTFILE!"
        type "!ERRORFILE!"
        echo Error: 'Running tests net40-compilerunit' failed
        goto :failed_tests
    )
)

REM ---------------- net40-coreunit  -----------------------

set XMLFILE=%RESULTSDIR%\test-net40-coreunit-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-net40-coreunit-output.log
set ERRORFILE=%RESULTSDIR%\test-net40-coreunit-errors.log
set command="%NUNIT3_CONSOLE%" --verbose --framework:V4.0 --result:"!XMLFILE!;format=nunit3" --output:"!OUTPUTFILE!" --err:"!ERRORFILE!" --work:"%FSCBINPATH%" "%FSCBINPATH%\FSharp.Core.Unittests.dll"
if '%TEST_NET40_COREUNIT_SUITE%' == '1' (

	rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
	rem Note: each setlocal must be matched by an executed endlocal
	setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 

	call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"
    if NOT '!saved_errorlevel!' == '0' (
        type "!OUTPUTFILE!"
        type "!ERRORFILE!"
        echo Error: 'Running tests net40-coreunit' failed 
        goto :failed_tests
    )
)

REM  ---------------- portable-coreunit  -----------------------

set XMLFILE=%RESULTSDIR%\test-portable-coreunit-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-portable-coreunit-output.log
set ERRORFILE=%RESULTSDIR%\test-portable-coreunit-errors.log
set command="%NUNIT3_CONSOLE%" /framework:V4.0 /result="!XMLFILE!;format=nunit3" /output="!OUTPUTFILE!" /err="!ERRORFILE!" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"

if '%TEST_PORTABLE_COREUNIT_SUITE%' == '1' (
	rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
	rem Note: each setlocal must be matched by an executed endlocal
	setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 


	call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"
    if NOT '!saved_errorlevel!' == '0' (
        type "!OUTPUTFILE!"
        type "!ERRORFILE!"
        echo Error: 'Running tests portable-coreunit' failed 
        goto :failed_tests
    )
)

REM  ---------------- coreclr-coreunit  -----------------------

set XMLFILE=%RESULTSDIR%\test-coreclr-coreunit-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-coreclr-coreunit-output.log
set ERRORFILE=%RESULTSDIR%\test-coreclr-coreunit-errors.log

set architecture=win7-x64
set CORERUNPATH=%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\!architecture!

set command="!CORERUNPATH!\corerun.exe" "%~dp0tests\testbin\!BUILD_CONFIG!\coreclr\fsharp.core.unittests\FSharp.Core.Unittests.exe"
if '%TEST_CORECLR_COREUNIT_SUITE%' == '1' (

	rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
	rem Note: each setlocal must be matched by an executed endlocal
	setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 

	rem call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"

    if ERRORLEVEL 1 (
        rem type "!OUTPUTFILE!"
        rem type "!ERRORFILE!"
        echo Error: 'Running tests coreclr-coreunit' failed 
        goto :failed_tests
    )
)

REM ---------------- coreclr-fsharp  -----------------------

set single_threaded=true
set permutations=FSC_CORECLR
set XMLFILE=%RESULTSDIR%\test-coreclr-fsharp-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-coreclr-fsharp-output.log
set ERRORFILE=%RESULTSDIR%\test-coreclr-fsharp-errors.log

set command="%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\..\..\coreclr\bin\FSharp.Tests.FSharp.dll" --framework:V4.0 --work:"%FSCBINPATH%"  --output:"!OUTPUTFILE!" --err:"!ERRORFILE!" --result:"!XMLFILE!;format=nunit3" 

if '%TEST_CORECLR_FSHARP_SUITE%' == '1' (
	rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
	rem Note: each setlocal must be matched by an executed endlocal
	setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 

	call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"
    if NOT '!saved_errorlevel!' == '0' (
        type "!OUTPUTFILE!"
        type "!ERRORFILE!"
        echo Error: 'Running tests coreclr-fsharp' failed 
        goto :failed_tests
    )
)


REM ---------------- vs-ideunit  -----------------------

set XMLFILE=%RESULTSDIR%\test-vs-ideunit-results.xml
set OUTPUTFILE=%RESULTSDIR%\test-vs-ideunit-output.log
set ERRORFILE=%RESULTSDIR%\test-vs-ideunit-errors.log

set command="%NUNIT3_CONSOLE%" --verbose --x86 --framework:V4.0 --result:"!XMLFILE!;format=nunit3" --output:"!OUTPUTFILE!" --err:"!ERRORFILE!" --work:"%FSCBINPATH%"  --workers=1 --agents=1 --full "%FSCBINPATH%\VisualFSharp.Unittests.dll"
if '%TEST_VS_IDEUNIT_SUITE%' == '1' (
	rem Turn off delayed expansion when manipulating variables where a ! may appear in the argument text (CMD batch file oddity)
	rem Note: each setlocal must be matched by an executed endlocal
	pushd %FSCBINPATH%
	setlocal disableDelayedExpansion
    echo %command% %WHERE_ARG_NUNIT%
         %command%  %WHERE_ARG_NUNIT%
    endlocal 
	popd
	call :UPLOAD_TEST_RESULTS "!XMLFILE!" "!OUTPUTFILE!"  "!ERRORFILE!"
    if NOT '!saved_errorlevel!' == '0' (
        type "!OUTPUTFILE!"
        type "!ERRORFILE!"
        echo Error: 'Running tests vs-ideunit' failed 
        goto :failed_tests
    )
)


:successful_tests
popd
endlocal
endlocal
goto :success

:failed_tests
popd
endlocal
endlocal
goto :failure

REM ------ upload test results procedure -------------------------------------

:UPLOAD_TEST_RESULTS

set saved_errorlevel=%errorlevel%
echo Saved errorlevel %saved_errorlevel%

rem See <http://www.appveyor.com/docs/environment-variables>
if not defined APPVEYOR goto :SKIP_APPVEYOR_UPLOAD

echo powershell -File Upload-Results.ps1 "%~1"
     powershell -File Upload-Results.ps1 "%~1"

:SKIP_APPVEYOR_UPLOAD

goto :EOF
:: Note: "goto :EOF" returns from an in-batchfile "call" command
:: in preference to returning from the entire batch file.

REM ------ exit -------------------------------------


:failure
endlocal
exit /b 1

:success
endlocal
exit /b 0
