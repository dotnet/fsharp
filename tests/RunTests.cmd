@if "%_echo%"=="" echo off
setlocal

set FLAVOR=%1
if /I "%FLAVOR%" == "debug"     (goto :FLAVOR_OK)
if /I "%FLAVOR%" == "release"   (goto :FLAVOR_OK)
if /I "%FLAVOR%" == "vsdebug"   (goto :FLAVOR_OK)
if /I "%FLAVOR%" == "vsrelease" (goto :FLAVOR_OK)
goto :USAGE

:flavor_ok

set NUNITPATH=%~dp0\fsharpqa\testenv\bin\nunit\
if not exist "%~dp0%..\packages\NUnit.Console.3.0.0\tools\" (
    pushd %~dp0
    ..\.nuget\nuget.exe restore ..\packages.config -PackagesDirectory ..\packages
    call buildtesttools.cmd %FLAVOR%
    popd
)
SET NUNIT3_CONSOLE=%~dp0%..\packages\NUnit.Console.3.0.0\tools\nunit3-console.exe

:: Check prerequisites
set fsi=

if exist "%VS140COMNTOOLS%..\ide\devenv.exe"                                        set FSI="%VS140COMNTOOLS%..\..\..\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"
if not '%fsi%' == '' goto fsiset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe"  set FSI="%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"
if exist "%ProgramFiles%\Microsoft Visual Studio 14.0\common7\ide\devenv.exe"       set FSI="%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"
if not '%fsi%' == '' goto fsiset

if exist "%VS120COMNTOOLS%..\ide\devenv.exe"                                        set FSI="%VS120COMNTOOLS%..\..\..\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"
if not '%fsi%' == '' goto fsiset
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe"  set FSI="%ProgramFiles(x86)%\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"
if exist "%ProgramFiles%\Microsoft Visual Studio 12.0\common7\ide\devenv.exe"       set FSI="%ProgramFiles%\Microsoft SDKs\F#\4.0\Framework\v4.0\Fsi.exe"

:fsiset
if '%fsi%' == '' echo Error: Could not find an installation of FSI && goto :failure

rem "ttags" indicates what test areas will be run, based on the tags in the test.lst files
set TTAGS_ARG=
SET TTAGS=
set _tmp=%3
if not '%_tmp%' == '' (
    set TTAGS_ARG=-ttags:%_tmp:"=%
    set TTAGS=%_tmp:"=%
)
if /I '%_tmp%' == 'coreclr' (
    set single_threaded=true
    set permutations=FSC_CORECLR
)

rem "nottags" indicates which test areas/test cases will NOT be run, based on the tags in the test.lst and env.lst files
set NO_TTAGS_ARG=-nottags:ReqPP,NOOPEN
set NO_TTAGS=ReqPP,NOOPEN
set _tmp=%4
if not '%_tmp%' == '' (
    set NO_TTAGS_ARG=-nottags:ReqPP,NOOPEN,%_tmp:"=%
    set NO_TTAGS=ReqPP,NOOPEN,%_tmp:"=%
)

if /I "%APPVEYOR_CI%" == "1" (
    set NO_TTAGS_ARG=%NO_TTAGS_ARG%,NO_CI
    set NO_TTAGS=%NO_TTAGS%,NO_CI
)

if /I not '%single_threaded%' == 'true' (set PARALLEL_ARG=-procs:%NUMBER_OF_PROCESSORS%) else set PARALLEL_ARG=-procs:0

rem This can be set to 1 to reduce the number of permutations used and avoid some of the extra-time-consuming tests
set REDUCED_RUNTIME=1
if "%REDUCED_RUNTIME%" == "1" (
    set NO_TTAGS_ARG=%NO_TTAGS_ARG%,Expensive
    set NO_TTAGS=%NO_TTAGS%,Expensive
)

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
set HOSTED_COMPILER=1

rem path to fsc.exe which will be used by tests
set FSCBINPATH=%~dp0..\%FLAVOR%\net40\bin
ECHO FSCBINPATH%

rem folder where test logs/results will be dropped
set RESULTSDIR=%~dp0\TestResults
if not exist "%RESULTSDIR%" (mkdir "%RESULTSDIR%")

setlocal EnableDelayedExpansion

SET CONV_V2_TO_V3_CMD="%FSI%" --exec --nologo "%~dp0%\Convert-NUnit2Args-to-NUnit3Where.fsx" -- "!TTAGS!" "!NO_TTAGS!"
echo %CONV_V2_TO_V3_CMD%

SET CONV_V2_TO_V3_CMD_TEMPFILE=%~dp0%nunit3args.txt

%CONV_V2_TO_V3_CMD% >%CONV_V2_TO_V3_CMD_TEMPFILE%

IF ERRORLEVEL 1 (
  echo Error converting args to nunit 3 test selection language, the nunit3-console --where argument
  type "%CONV_V2_TO_V3_CMD_TEMPFILE%"
  del /Q "%CONV_V2_TO_V3_CMD_TEMPFILE%"
  exit /b 1
)

set /p TTAGS_NUNIT_WHERE=<%CONV_V2_TO_V3_CMD_TEMPFILE%
if not '!TTAGS_NUNIT_WHERE!' == '' (set TTAGS_NUNIT_WHERE=--where "!TTAGS_NUNIT_WHERE!")

del /Q "%CONV_V2_TO_V3_CMD_TEMPFILE%"

setlocal DisableDelayedExpansion

if /I "%2" == "fsharp" (goto :FSHARP)
if /I "%2" == "fsharpqa" (goto :FSHARPQA)
if /I "%2" == "fsharpqadowntarget" (goto :FSHARPQA)
if /I "%2" == "fsharpqaredirect" (goto :FSHARPQA)
if /I "%2" == "compilerunit" (
   set compilerunitsuffix=net40
   goto :COMPILERUNIT
)

if /I "%2" == "coreunitall" (
   goto :COREUNITALL
)

if /I "%2" == "coreunitportable" (
   goto :COREUNITPORTABLE
)

if /I "%2" == "coreunit" (
   set coreunitsuffix=net40
   goto :COREUNIT
)
if /I "%2" == "coreunitportable47" (
   set coreunitsuffix=portable47
   goto :COREUNIT
)
if /I "%2" == "coreunitportable7" (
   set coreunitsuffix=portable7
   goto :COREUNIT
)
if /I "%2" == "coreunitportable78" (
   set coreunitsuffix=portable78
   goto :COREUNIT
)
if /I "%2" == "coreunitportable259" (
   set coreunitsuffix=portable259
   goto :COREUNIT
)
if /I "%2" == "coreunitcoreclr" (
   set coreunitsuffix=coreclr
   goto :COREUNIT_CORECLR
)
if /I "%2" == "ideunit" (goto :IDEUNIT)

:USAGE

echo Usage:
echo.
echo RunTests.cmd ^<debug^|release^|vsdebug^|vsrelease^> ^<fsharp^|fsharpqa^|coreunit^|coreunitall^|coreunitportable^|coreunitportable47^|coreunitportable7^|coreunitportable78^|coreunitportable259^|coreunitcoreclr^|ideunit^|compilerunit^> [TagToRun^|"Tags,To,Run"] [TagNotToRun^|"Tags,Not,To,Run"]
echo.
exit /b 1

:FSHARP

if not '%FSHARP_TEST_SUITE_USE_NUNIT_RUNNER%' == '' (
    goto :FSHARP_NUNIT
)

set RESULTFILE=FSharp_Results.log
set FAILFILE=FSharp_Failures.log
set FAILENV=FSharp_Failures

rem Hosted compiler not supported for FSHARP suite
set HOSTED_COMPILER=

where.exe perl > NUL 2> NUL
if errorlevel 1 (
  echo Error: perl is not in the PATH
  exit /b 1
)

echo perl %~dp0\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:yes %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
     perl %~dp0\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:yes %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
)
if errorlevel 1 (
  type %RESULTSDIR%\%FAILFILE%
  exit /b 1
)
goto :EOF


:FSHARP_NUNIT

set FSHARP_TEST_SUITE_CONFIGURATION=%FLAVOR%

set XMLFILE=FSharpNunit_Xml.xml
set OUTPUTFILE=FSharpNunit_Output.log
set ERRORFILE=FSharpNunit_Error.log

echo "%NUNIT3_CONSOLE%" "%FSCBINPATH%\FSharp.Tests.FSharp.dll" --framework:V4.0 %TTAGS_NUNIT_WHERE% %NO_TTAGS_NUNIT_WHERE% --work="%FSCBINPATH%"  --output="%OUTPUTFILE%" --err="%ERRORFILE%" --result="%XMLFILE%;format=nunit2"
"%NUNIT3_CONSOLE%" "%FSCBINPATH%\FSharp.Tests.FSharp.dll" --framework:V4.0 %TTAGS_NUNIT_WHERE% %NO_TTAGS_NUNIT_WHERE% --work="%FSCBINPATH%"  --output="%OUTPUTFILE%" --err="%ERRORFILE%" --result="%XMLFILE%;format=nunit2"

call :UPLOAD_XML "%XMLFILE%"
goto :EOF


:FSHARPQA
set OSARCH=%PROCESSOR_ARCHITECTURE%

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
set PATH=%PATH%;%WINSDKNETFXTOOLS%

IF NOT DEFINED SNEXE32 IF EXIST "%WINSDKNETFXTOOLS%sn.exe"               set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64 IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"           set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe
IF NOT DEFINED GACUTILEXE32 IF EXIST "%WINSDKNETFXTOOLS%gacutil.exe"     set GACUTILEXE32=%WINSDKNETFXTOOLS%gacutil.exe
IF NOT DEFINED GACUTILEXE64 IF EXIST "%WINSDKNETFXTOOLS%x64\gacutil.exe" set GACUTILEXE64=%WINSDKNETFXTOOLS%x64\gacutil.exe

set FSC=%FSCBINPATH%\fsc.exe
set PATH=%FSCBINPATH%;%PATH%

set FSCVPREVBINPATH=%X86_PROGRAMFILES%\Microsoft SDKs\F#\3.1\Framework\v4.0
set FSCVPREV=%FSCVPREVBINPATH%\fsc.exe

REM == VS-installed paths to FSharp.Core.dll
set FSCOREDLLPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0
set FSCOREDLL20PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v2.0\2.3.0.0
set FSCOREDLLPORTABLEPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.4.0
set FSCOREDLLNETCOREPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.4.0
set FSCOREDLLNETCORE78PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.4.0
set FSCOREDLLNETCORE259PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.4.0
set FSDATATPPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers
set FSCOREDLLVPREVPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.1.0

REM == open source logic
if exist "%FSCBinPath%\FSharp.Core.dll" set FSCOREDLLPATH=%FSCBinPath%
if exist "%FSCBinPath%\..\..\net20\bin\FSharp.Core.dll" set FSCOREDLL20PATH=%FSCBinPath%\..\..\net20\bin
if exist "%FSCBinPath%\..\..\portable47\bin\FSharp.Core.dll" set FSCOREDLLPORTABLEPATH=%FSCBinPath%\..\..\portable47\bin
if exist "%FSCBinPath%\..\..\portable7\bin\FSharp.Core.dll" set FSCOREDLLNETCOREPATH=%FSCBinPath%\..\..\portable7\bin
IF exist "%FSCBinPath%\..\..\portable78\bin\FSharp.Core.dll" set FSCOREDLLNETCORE78PATH=%FSCBinPath%\..\..\portable78\bin
IF exist "%FSCBinPath%\..\..\portable259\bin\FSharp.Core.dll" set FSCOREDLLNETCORE259PATH=%FSCBinPath%\..\..\portable259\bin
if exist "%FSCBinPath%\FSharp.Data.TypeProviders.dll" set FSDATATPPATH=%FSCBinPath%

set FSCOREDLLPATH=%FSCOREDLLPATH%\FSharp.Core.dll
set FSCOREDLL20PATH=%FSCOREDLL20PATH%\FSharp.Core.dll
set FSCOREDLLPORTABLEPATH=%FSCOREDLLPORTABLEPATH%\FSharp.Core.dll
set FSCOREDLLNETCOREPATH=%FSCOREDLLNETCOREPATH%\FSharp.Core.dll
set FSCOREDLLNETCORE78PATH=%FSCOREDLLNETCORE78PATH%\FSharp.Core.dll
set FSCOREDLLNETCORE259PATH=%FSCOREDLLNETCORE259PATH%\FSharp.Core.dll
set FSDATATPPATH=%FSDATATPPATH%\FSharp.Data.TypeProviders.dll
set FSCOREDLLVPREVPATH=%FSCOREDLLVPREVPATH%\FSharp.Core.dll

for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

if not exist %WINDIR%\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll set NO_TTAGS_ARG=%NO_TTAGS_ARG%,Req20

set RESULTFILE=FSharpQA_Results.log
set FAILFILE=FSharpQA_Failures.log
set FAILENV=FSharpQA_Failures

if /I "%2" == "fsharpqadowntarget" (
   set ISCFLAGS=--noframework -r "%FSCOREDLLVPREVPATH%" -r "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" -r System.dll -r System.Runtime.dll -r System.Xml.dll -r System.Data.dll -r System.Web.dll -r System.Core.dll -r System.Numerics.dll
   set NO_TTAGS_ARG=%NO_TTAGS_ARG%,NoCrossVer,FSI
   set RESULTFILE=FSharpQADownTarget_Results.log
   set FAILFILE=FSharpQADownTarget_Failures.log
   set FAILENV=FSharpQADownTarget_Failures
)

if /I "%2" == "fsharpqaredirect" (
   set ISCFLAGS=--noframework -r "%FSCOREDLLVPREVPATH%" -r "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" -r System.dll -r System.Runtime.dll -r System.Xml.dll -r System.Data.dll -r System.Web.dll -r System.Core.dll -r System.Numerics.dll
   set PLATFORM=%OSARCH%
   set SIMULATOR_PIPE="%~dp0\fsharpqa\testenv\bin\$PLATFORM\ExecAssembly.exe"
   set NO_TTAGS_ARG=%NO_TTAGS_ARG%,NoCrossVer,FSI
   set RESULTFILE=FSharpQARedirect_Results.log
   set FAILFILE=FSharpQARedirect_Failures.log
   set FAILENV=FSharpQARedirect_Failures
)

where.exe perl > NUL 2> NUL
if errorlevel 1 (
  echo Error: perl is not in the PATH
  exit /b 1
)

pushd %~dp0\fsharpqa\source
echo perl %~dp0\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:yes %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
     perl %~dp0\fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:yes %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%

popd
goto :EOF

:COREUNIT

set XMLFILE=%RESULTSDIR%\CoreUnit_%coreunitsuffix%_Xml.xml
set OUTPUTFILE=%RESULTSDIR%\CoreUnit_%coreunitsuffix%_Output.log
set ERRORFILE=%RESULTSDIR%\CoreUnit_%coreunitsuffix%_Error.log

echo "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\%coreunitsuffix%\bin\FSharp.Core.Unittests.dll"
     "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\%coreunitsuffix%\bin\FSharp.Core.Unittests.dll"

call :UPLOAD_XML "%XMLFILE%"
goto :EOF

:COREUNITALL

set XMLFILE=%RESULTSDIR%\CoreUnit_all_Xml.xml
set OUTPUTFILE=%RESULTSDIR%\CoreUnit_all_Output.log
set ERRORFILE=%RESULTSDIR%\CoreUnit_all_Error.log

echo "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\net40\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"
     "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\net40\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"

call :UPLOAD_XML "%XMLFILE%"
goto :EOF

:COREUNITPORTABLE

set XMLFILE=%RESULTSDIR%\CoreUnit_Portable_Xml.xml
set OUTPUTFILE=%RESULTSDIR%\CoreUnit_Portable_Output.log
set ERRORFILE=%RESULTSDIR%\CoreUnit_Portable_Error.log

echo "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"
     "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"

call :UPLOAD_XML "%XMLFILE%"
goto :EOF

:COREUNIT_CORECLR

set XMLFILE=CoreUnit_%coreunitsuffix%_Xml.xml
set OUTPUTFILE=CoreUnit_%coreunitsuffix%_Output.log
set ERRORFILE=CoreUnit_%coreunitsuffix%_Error.log

set testbinpath=%~dp0%testbin\
set architecturepath=\coreclr\fsc\win7-x86
set CORERUNPATH="%testbinpath%%flavor%%architecturepath%"

echo "%CORERUNPATH%\corerun.exe" "%FSCBINPATH%\..\..\%coreunitsuffix%\bin\FSharp.Core.Unittests.exe"
     "%CORERUNPATH%\corerun.exe" "%FSCBINPATH%\..\..\%coreunitsuffix%\bin\FSharp.Core.Unittests.exe"

goto :EOF

:COMPILERUNIT

set XMLFILE=%RESULTSDIR%\CompilerUnit_%compilerunitsuffix%_Xml.xml
set OUTPUTFILE=%RESULTSDIR%\CompilerUnit_%compilerunitsuffix%_Output.log
set ERRORFILE=%RESULTSDIR%\CompilerUnit_%compilerunitsuffix%_Error.log

echo "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\%compilerunitsuffix%\bin\FSharp.Compiler.Unittests.dll"
     "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\%compilerunitsuffix%\bin\FSharp.Compiler.Unittests.dll"

call :UPLOAD_XML "%XMLFILE%"
goto :EOF

:IDEUNIT

set XMLFILE=%RESULTSDIR%\IDEUnit_Xml.xml
set OUTPUTFILE=%RESULTSDIR%\IDEUnit_Output.log
set ERRORFILE=%RESULTSDIR%\IDEUnit_Error.log

pushd %FSCBINPATH%
echo "%NUNIT3_CONSOLE%" --x86 /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\Unittests.dll"
     "%NUNIT3_CONSOLE%" --x86 /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\Unittests.dll"
popd
call :UPLOAD_XML "%XMLFILE%"
goto :EOF

:UPLOAD_XML

rem See <http://www.appveyor.com/docs/environment-variables>
if not defined APPVEYOR goto :EOF
powershell -File Upload-Results.ps1 %RESULTSDIR%\%XMLFILE%

goto :EOF

:UPLOAD_XML

rem See <http://www.appveyor.com/docs/environment-variables>
if not defined APPVEYOR goto :EOF

set saved_errorlevel=%errorlevel%
echo Saved errorlevel %saved_errorlevel%
powershell -File Upload-Results.ps1 "%~1"
if %saved_errorlevel% neq 0 exit /b %saved_errorlevel%
goto :EOF

:: Note: "goto :EOF" returns from an in-batchfile "call" command
:: in preference to returning from the entire batch file.
