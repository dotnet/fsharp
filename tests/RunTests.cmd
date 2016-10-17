@if "%_echo%"=="" echo off
setlocal

set FLAVOR=%1
if /I "%FLAVOR%" == "debug"     (goto :FLAVOR_OK)
if /I "%FLAVOR%" == "release"   (goto :FLAVOR_OK)
goto :USAGE

:FLAVOR_OK

set NUNITPATH=%~dp0fsharpqa\testenv\bin\nunit\
SET NUNIT3_CONSOLE=%~dp0..\packages\NUnit.Console.3.0.0\tools\nunit3-console.exe
SET FSI_TOOL=%~dp0..\%FLAVOR%\net40\bin\Fsi.exe

set link_exe=%~dp0..\packages\VisualCppTools.14.0.24519-Pre\lib\native\bin\link.exe
if not exist "%link_exe%" (
    set saved_errorlevel=1
    echo Error: failed to find '%link_exe%' use nuget to restore the VisualCppTools package
    goto :FINISHED
)


if /I not '%single_threaded%' == 'true' (set PARALLEL_ARG=-procs:%NUMBER_OF_PROCESSORS%) else set PARALLEL_ARG=-procs:0

rem This can be set to 1 to reduce the number of permutations used and avoid some of the extra-time-consuming tests
if "%SKIP_EXPENSIVE_TESTS%" == "1" (
    set EXCLUDE_ARG_NUNIT=--where "cat != Expensive"
    set EXCLUDE_ARG_RUNALL=-nottags:Expensive
)

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
set HOSTED_COMPILER=1

rem path to fsc.exe which will be used by tests
set FSCBINPATH=%~dp0..\%FLAVOR%\net40\bin
ECHO %FSCBINPATH%

rem folder where test logs/results will be dropped
set RESULTSDIR=%~dp0TestResults
if not exist "%RESULTSDIR%" (mkdir "%RESULTSDIR%")

if /I "%2" == "net40-fsharp-suite" (goto :NET40_FSHARP_SUITE)

if /I "%2" == "coreclr-fsharp-suite" (goto :CORECLR_FSHARP_SUITE)

if /I "%2" == "net40-fsharpqa-suite" (goto :NET40_FSHARPQA_SUITE)

if /I "%2" == "net40-fsharpqa-suite-downtarget" (goto :NET40_FSHARPQA_SUITE)

if /I "%2" == "net40-compilerunit-suite" (
   set compilerunitsuffix=net40
   goto :COMPILERUNIT_SUITE
)

if /I "%2" == "portable-coreunit-suite" (
   goto :PORTABLE_COREUNIT_SUITE
)

if /I "%2" == "net40-coreunit-suite" (
   goto :NET40_COREUNIT_SUITE
)
if /I "%2" == "coreclr-coreunit-suite" (
   goto :CORECLR_COREUNIT_SUITE
)
if /I "%2" == "vs-ideunit-suite" (goto :VS_IDEUNIT_SUITE)

REM ----------------------------------------------------------------------------

:USAGE

echo Usage:
echo.
echo RunTests.cmd ^<debug^|release^> ^<...suite....^> [TagToRun^|"Tags,To,Run"] [TagNotToRun^|"Tags,Not,To,Run"]
echo.
exit /b 1

REM ----------------------------------------------------------------------------

:NET40_FSHARP_SUITE

set XMLFILE=%RESULTSDIR%\net40-fsharp-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\net40-fsharp-suite-output.log
set ERRORFILE=%RESULTSDIR%\net40-fsharp-suite-errors.log

echo "%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\..\..\net40\bin\FSharp.Tests.FSharp.dll" --framework:V4.0 %EXCLUDE_ARG_NUNIT% --work:"%FSCBINPATH%"  --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --result:"%XMLFILE%;format=nunit2" 
"%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\..\..\net40\bin\FSharp.Tests.FSharp.dll" --framework:V4.0 %EXCLUDE_ARG_NUNIT% --work:"%FSCBINPATH%"  --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --result:"%XMLFILE%;format=nunit2"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:CORECLR_FSHARP_SUITE

set single_threaded=true
set permutations=FSC_CORECLR
set XMLFILE=%RESULTSDIR%\coreclr-fsharp-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\coreclr-fsharp-suite-output.log
set ERRORFILE=%RESULTSDIR%\coreclr-fsharp-suite-errors.log

echo "%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\..\..\coreclr\bin\FSharp.Tests.FSharp.dll" --framework:V4.0 %EXCLUDE_ARG_NUNIT% --work:"%FSCBINPATH%"  --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --result:"%XMLFILE%;format=nunit2" 
"%NUNIT3_CONSOLE%" --verbose "%FSCBINPATH%\..\..\coreclr\bin\FSharp.Tests.FSharp.dll" --framework:V4.0 %EXCLUDE_ARG_NUNIT% --work:"%FSCBINPATH%"  --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --result:"%XMLFILE%;format=nunit2"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:NET40_FSHARPQA_SUITE
set OSARCH=%PROCESSOR_ARCHITECTURE%

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set SYSWOW64=.
if "%OSARCH%"=="AMD64" set SYSWOW64=SysWoW64

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
set PATH=%PATH%;%WINSDKNETFXTOOLS%

IF NOT DEFINED SNEXE32  IF EXIST "%WINSDKNETFXTOOLS%sn.exe"               set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64  IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"           set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe
IF NOT DEFINED ildasm   IF EXIST "%WINSDKNETFXTOOLS%ildasm.exe"           set ildasm=%WINSDKNETFXTOOLS%ildasm.exe

set FSC=%FSCBINPATH%\fsc.exe
set PATH=%FSCBINPATH%;%PATH%

set FSCVPREVBINPATH=%X86_PROGRAMFILES%\Microsoft SDKs\F#\4.0\Framework\v4.0
set FSCVPREV=%FSCVPREVBINPATH%\fsc.exe

REM == VS-installed paths to FSharp.Core.dll
set FSCOREDLLPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.1.0
set FSCOREDLL20PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v2.0\2.3.0.0
set FSCOREDLLPORTABLEPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETPortable\3.47.41.0
set FSCOREDLLNETCOREPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.7.41.0
set FSCOREDLLNETCORE78PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.78.41.0
set FSCOREDLLNETCORE259PATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETCore\3.259.41.0
set FSDATATPPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\Type Providers
set FSCOREDLLVPREVPATH=%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0

REM == open source logic
if exist "%FSCBinPath%\FSharp.Core.dll" set FSCOREDLLPATH=%FSCBinPath%
if exist "%FSCBinPath%\..\..\net20\bin\FSharp.Core.dll" set FSCOREDLL20PATH=%FSCBinPath%\..\..\net20\bin
if exist "%FSCBinPath%\..\..\portable47\bin\FSharp.Core.dll" set FSCOREDLLPORTABLEPATH=%FSCBinPath%\..\..\portable47\bin
if exist "%FSCBinPath%\..\..\portable7\bin\FSharp.Core.dll" set FSCOREDLLNETCOREPATH=%FSCBinPath%\..\..\portable7\bin
IF exist "%FSCBinPath%\..\..\portable78\bin\FSharp.Core.dll" set FSCOREDLLNETCORE78PATH=%FSCBinPath%\..\..\portable78\bin
IF exist "%FSCBinPath%\..\..\portable259\bin\FSharp.Core.dll" set FSCOREDLLNETCORE259PATH=%FSCBinPath%\..\..\portable259\bin

set FSCOREDLLPATH=%FSCOREDLLPATH%\FSharp.Core.dll
set FSCOREDLL20PATH=%FSCOREDLL20PATH%\FSharp.Core.dll
set FSCOREDLLPORTABLEPATH=%FSCOREDLLPORTABLEPATH%\FSharp.Core.dll
set FSCOREDLLNETCOREPATH=%FSCOREDLLNETCOREPATH%\FSharp.Core.dll
set FSCOREDLLNETCORE78PATH=%FSCOREDLLNETCORE78PATH%\FSharp.Core.dll
set FSCOREDLLNETCORE259PATH=%FSCOREDLLNETCORE259PATH%\FSharp.Core.dll
set FSCOREDLLVPREVPATH=%FSCOREDLLVPREVPATH%\FSharp.Core.dll

for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

set RESULTFILE=net40-fsharpqa-suite-results.log
set FAILFILE=net40-fsharpqa-suite-errors.log
set FAILENV=net40-fsharpqa-suite-errors

if /I "%2" == "net40-fsharpqa-suite-downtarget" (
   set ISCFLAGS=--noframework -r "%FSCOREDLLVPREVPATH%" -r "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" -r System.dll -r System.Runtime.dll -r System.Xml.dll -r System.Data.dll -r System.Web.dll -r System.Core.dll -r System.Numerics.dll
   set EXCLUDE_ARG_RUNALL=%EXCLUDE_ARG_RUNALL%,NoCrossVer,FSI
   set RESULTFILE=net40-fsharpqa-downtarget-suite-results.log
   set FAILFILE=net40-fsharpqa-downtarget-suite-errors.log
   set FAILENV=net40-fsharpqa-downtarget-suite-errors
)

where.exe perl > NUL 2> NUL
if errorlevel 1 (
  echo Error: perl is not in the PATH
  exit /b 1
)

pushd %~dp0fsharpqa\source
echo perl %~dp0fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:no %INCLUDE_ARG_RUNALL% %EXCLUDE_ARG_RUNALL% %PARALLEL_ARG%
     perl %~dp0fsharpqa\testenv\bin\runall.pl -resultsroot %RESULTSDIR% -results %RESULTFILE% -log %FAILFILE% -fail %FAILENV% -cleanup:no %INCLUDE_ARG_RUNALL% %EXCLUDE_ARG_RUNALL% %PARALLEL_ARG%

popd
goto :EOF

REM ----------------------------------------------------------------------------

:NET40_COREUNIT_SUITE

set XMLFILE=%RESULTSDIR%\net40-coreunit-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\net40-coreunit-suite-output.log
set ERRORFILE=%RESULTSDIR%\net40-coreunit-suite-errors.log

echo "%NUNIT3_CONSOLE%" --verbose --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%" "%FSCBINPATH%\..\..\net40\bin\FSharp.Core.Unittests.dll"
     "%NUNIT3_CONSOLE%" --verbose --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%" "%FSCBINPATH%\..\..\net40\bin\FSharp.Core.Unittests.dll"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:PORTABLE_COREUNIT_SUITE

set XMLFILE=%RESULTSDIR%\portable-coreunit-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\portable-coreunit-suite-output.log
set ERRORFILE=%RESULTSDIR%\portable-coreunit-suite-errors.log

echo "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"
     "%NUNIT3_CONSOLE%" /framework:V4.0 /result="%XMLFILE%;format=nunit2" /output="%OUTPUTFILE%" /err="%ERRORFILE%" /work="%FSCBINPATH%" "%FSCBINPATH%\..\..\portable7\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable47\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable78\bin\FSharp.Core.Unittests.dll" "%FSCBINPATH%\..\..\portable259\bin\FSharp.Core.Unittests.dll"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:CORECLR_COREUNIT_SUITE

set XMLFILE=coreclr-coreunit-suite-results.xml
set OUTPUTFILE=coreclr-coreunit-suite-output.log
set ERRORFILE=coreclr-coreunit-suite-errors.log

set testbinpath=%~dp0testbin\
set architecturepath=coreclr\win7-x64
set CORERUNPATH="%testbinpath%%flavor%\%architecturepath%"

echo "%CORERUNPATH%\corerun.exe" "%testbinpath%%flavor%\coreclr\fsharp.core.unittests\FSharp.Core.Unittests.exe"
     "%CORERUNPATH%\corerun.exe" "%testbinpath%%flavor%\coreclr\fsharp.core.unittests\FSharp.Core.Unittests.exe"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:COMPILERUNIT_SUITE

set XMLFILE=%RESULTSDIR%\%compilerunitsuffix%-compilerunit-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\%compilerunitsuffix%-compilerunit-suite-output.log
set ERRORFILE=%RESULTSDIR%\%compilerunitsuffix%-compilerunit-suite-errors.log

echo "%NUNIT3_CONSOLE%" --verbose --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%" "%FSCBINPATH%\..\..\%compilerunitsuffix%\bin\FSharp.Compiler.Unittests.dll"
     "%NUNIT3_CONSOLE%" --verbose --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%" "%FSCBINPATH%\..\..\%compilerunitsuffix%\bin\FSharp.Compiler.Unittests.dll"

call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:VS_IDEUNIT_SUITE

set XMLFILE=%RESULTSDIR%\vs-ideunit-suite-results.xml
set OUTPUTFILE=%RESULTSDIR%\vs-ideunit-suite-output.log
set ERRORFILE=%RESULTSDIR%\vs-ideunit-suite-errors.log

pushd %FSCBINPATH%
echo "%NUNIT3_CONSOLE%" --verbose --x86 --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%"  --workers=1 --agents=1 --full "%FSCBINPATH%\VisualFSharp.Unittests.dll"
     "%NUNIT3_CONSOLE%" --verbose --x86 --framework:V4.0 %EXCLUDE_ARG_NUNIT% --result:"%XMLFILE%;format=nunit2" --output:"%OUTPUTFILE%" --err:"%ERRORFILE%" --work:"%FSCBINPATH%"  --workers=1 --agents=1 --full "%FSCBINPATH%\VisualFSharp.Unittests.dll"
popd
call :UPLOAD_TEST_RESULTS "%XMLFILE%" "%OUTPUTFILE%"  "%ERRORFILE%"
goto :EOF

REM ----------------------------------------------------------------------------

:UPLOAD_TEST_RESULTS

set saved_errorlevel=%errorlevel%
echo Saved errorlevel %saved_errorlevel%

rem See <http://www.appveyor.com/docs/environment-variables>
if not defined APPVEYOR goto :SKIP_APPVEYOR_UPLOAD

echo powershell -File Upload-Results.ps1 "%~1"
     powershell -File Upload-Results.ps1 "%~1"

:SKIP_APPVEYOR_UPLOAD
:FINISHED

if NOT %saved_errorlevel% == 0 exit /b 1
goto :EOF

:: Note: "goto :EOF" returns from an in-batchfile "call" command
:: in preference to returning from the entire batch file.
