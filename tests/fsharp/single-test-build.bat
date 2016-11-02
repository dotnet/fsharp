@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok

if '%flavor%' == '' ( 
    set flavor=release
)

echo PERMUTATIONS='%permutations%'
call %~d0%~p0..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" (
  goto Skip
)

set source1=
if exist test.ml (set source1=test.ml)
if exist test.fs (set source1=test.fs)

set sources=
if exist testlib.fsi (set sources=%sources% testlib.fsi)
if exist testlib.fs (set sources=%sources% testlib.fs)
if exist test.mli (set sources=%sources% test.mli)
if exist test.ml (set sources=%sources% test.ml)
if exist test.fsi (set sources=%sources% test.fsi)
if exist test.fs (set sources=%sources% test.fs)
if exist test2.mli (set sources=%sources% test2.mli)
if exist test2.ml (set sources=%sources% test2.ml)
if exist test2.fsi (set sources=%sources% test2.fsi)
if exist test2.fs (set sources=%sources% test2.fs)
if exist test.fsx (set sources=%sources% test.fsx)
if exist test2.fsx (set sources=%sources% test2.fsx)

set sourceshw=
if exist test-hw.mli (set sourceshw=%sourceshw% test-hw.mli)
if exist test-hw.ml (set sourceshw=%sourceshw% test-hw.ml)
if exist test-hw.fsx (set sourceshw=%sourceshw% test-hw.fsx)
if exist test2-hw.mli (set sourceshw=%sourceshw% test2-hw.mli)
if exist test2-hw.ml (set sourceshw=%sourceshw% test2-hw.ml)
if exist test2-hw.fsx (set sourceshw=%sourceshw% test2-hw.fsx)

rem to run the 64 bit version of the code set FSC_BASIC_64=FSC_BASIC_64
set PERMUTATIONS_LIST=FSI_FILE FSI_STDIN FSI_STDIN_OPT FSI_STDIN_GUI FSC_BASIC %FSC_BASIC_64% FSC_HW FSC_O3 GENERATED_SIGNATURE EMPTY_SIGNATURE EMPTY_SIGNATURE_OPT FSC_OPT_MINUS_DEBUG FSC_OPT_PLUS_DEBUG FRENCH SPANISH AS_DLL WRAPPER_NAMESPACE WRAPPER_NAMESPACE_OPT

if "%SKIP_EXPENSIVE_TESTS%"=="1" (
    echo SKIP_EXPENSIVE_TESTS set
    
    if not defined PERMUTATIONS (
        powershell.exe %PSH_FLAGS% -command "&{& '%~d0%~p0\PickPermutations.ps1' '%cd%' '%FSC%' '%PERMUTATIONS_LIST%'}" > _perm.txt
        if errorlevel 1 (
            set ERRORMSG=%ERRORMSG% PickPermutations.ps1 failed;
            goto :ERROR
        )
        set /p PERMUTATIONS=<_perm.txt
    )
    
    powershell.exe %PSH_FLAGS% -command "&{& '%~d0%~p0\DecidePEVerify.ps1' '%cd%' '%FSC%'}"
    if errorlevel 1 (
        set ERRORMSG=%ERRORMSG% DecidePEVerify.ps1 failed;
        goto :ERROR
    )
)

if not defined PERMUTATIONS (
    echo "PERMUTATIONS not defined. Building everything."
    set PERMUTATIONS=%PERMUTATIONS_LIST%
)

for %%A in (%PERMUTATIONS%) do (
    call :%%A
    IF ERRORLEVEL 1 EXIT /B 1
)

:Ok
echo Built fsharp %~f0 ok.
echo. > build.ok
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0

:Error
echo Test Script Failed (perhaps test did not emit test.ok signal file?)
endlocal
exit /b %ERRORLEVEL%

:SETERROR
set NonexistentErrorLevel 2> nul
goto Error

:FSI_FILE
@echo do :FSI_FILE
goto :DO_NOOP

:FSI_STDIN
@echo do :FSI_STDIN
goto :DO_NOOP

:FSI_STDIN_OPT
@echo do :FSI_STDIN_OPT
goto :DO_NOOP

:FSI_STDIN_GUI
@echo do :FSI_STDIN_GUI
goto :DO_NOOP

:DO_NOOP
@echo No build action to take for this permutation
goto :EOF

:FRENCH
@echo do :FRENCH
goto :DOBASIC

:SPANISH
@echo do :SPANISH
goto :DOBASIC

:FSC_CORECLR
@Echo do :CORECLR
rem Build references currently hard coded need a better long term solution
set platform=win7-x64
For %%A in ("%cd%") do (Set TestCaseName=%%~nxA)
set command_line_args=
set command_line_args=%command_line_args% --exec %~d0%~p0..\fsharpqa\testenv\src\deployProj\CompileProj.fsx
set command_line_args=%command_line_args% --targetPlatformName:.NETStandard,Version=v1.6/%platform%
set command_line_args=%command_line_args% --source:"%~d0%~p0coreclr_utilities.fs" --source:"%sources%" 
set command_line_args=%command_line_args% --packagesDir:%~d0%~p0..\..\packages 
set command_line_args=%command_line_args% --projectJsonLock:%~d0%~p0project.lock.json
set command_line_args=%command_line_args% --fsharpCore:%~d0%~p0..\testbin\%flavor%\coreclr\fsc\%platform%\FSharp.Core.dll
set command_line_args=%command_line_args% --define:CoreClr --define:NetCore
set command_line_args=%command_line_args% --compilerPath:%~d0%~p0..\testbin\%flavor%\coreclr\fsc\%platform%
set command_line_args=%command_line_args% --copyCompiler:yes
set command_line_args=%command_line_args% --verbose:verbose
if not "%test_keyfile%" == "" (
    set command_line_args=%command_line_args% --keyfile:%test_keyfile%
)
if not "%test_delaysign%" == "" (
    set command_line_args=%command_line_args% --delaysign:yes
)
if not "%test_publicsign%" == "" (
    set command_line_args=%command_line_args% --publicsign:yes
)
if not "%extra_defines%" == "" (
    set command_line_args=%command_line_args% %extra_defines%
)
if "%test_outfile%" == "" (
    set command_line_args=%command_line_args% --output:%~d0%~p0..\testbin\%flavor%\coreclr\fsharp\core\%TestCaseName%\output\test.exe
)
if not "%test_outfile%" == "" (
    set command_line_args=%command_line_args% --output:%~d0%~p0..\testbin\%flavor%\coreclr\fsharp\core\%TestCaseName%\output\test-%test_outfile%.exe
)

echo %fsi% %command_line_args%
%fsi% %command_line_args%
echo Errorlevel: %errorlevel%
if ERRORLEVEL 1 goto Error
goto :EOF

:FSC_BASIC
@echo do :FSC_BASIC
:DOBASIC
"%FSC%" %fsc_flags% --define:BASIC_TEST -o:test.exe -g %sources%
if ERRORLEVEL 1 goto Error

if NOT EXIST dont.run.peverify (
    "%PEVERIFY%" test.exe
    @if ERRORLEVEL 1 goto Error
)
goto :EOF

:FSC_BASIC_64
@echo do :FSC_BASIC_64
"%FSC%" %fsc_flags% --define:BASIC_TEST --platform:x64 -o:testX64.exe -g %sources%
if ERRORLEVEL 1 goto Error

if NOT EXIST dont.run.peverify (
    "%PEVERIFY%" testX64.exe
    @if ERRORLEVEL 1 goto Error
)
goto :EOF

:FSC_HW
@echo do :FSC_HW
if exist test-hw.* (
  "%FSC%" %fsc_flags% -o:test-hw.exe -g %sourceshw%
  if ERRORLEVEL 1 goto Error

  if NOT EXIST dont.run.peverify (
    "%PEVERIFY%" test-hw.exe
    @if ERRORLEVEL 1 goto Error
  )
)
goto :EOF

:FSC_O3
@echo do :FSC_O3
"%FSC%" %fsc_flags% --optimize --define:PERF -o:test--optimize.exe -g %sources%
if ERRORLEVEL 1 goto Error

if NOT EXIST dont.run.peverify (
    "%PEVERIFY%" test--optimize.exe
    @if ERRORLEVEL 1 goto Error
)
goto :EOF

:GENERATED_SIGNATURE
@echo do :GENERATED_SIGNATURE
if NOT EXIST dont.use.generated.signature (
 if exist test.ml (

  echo Generating interface file...
  copy /y %source1% tmptest.ml
  REM NOTE: use --generate-interface-file since results may be in Unicode
  "%FSC%" %fsc_flags% --sig:tmptest.mli tmptest.ml
  if ERRORLEVEL 1 goto Error

  echo Compiling against generated interface file...
  "%FSC%" %fsc_flags% -o:tmptest1.exe tmptest.mli tmptest.ml
  if ERRORLEVEL 1 goto Error

  if NOT EXIST dont.run.peverify (
    "%PEVERIFY%" tmptest1.exe
    @if ERRORLEVEL 1 goto Error
  )
 )
)
goto :EOF

:EMPTY_SIGNATURE
@echo do :EMPTY_SIGNATURE
if NOT EXIST dont.use.empty.signature (
    if exist test.ml ( 
        echo Compiling against empty interface file...
        echo // empty file  > tmptest2.mli

        copy /y %source1% tmptest2.ml
        "%FSC%" %fsc_flags% --define:COMPILING_WITH_EMPTY_SIGNATURE -o:tmptest2.exe tmptest2.mli tmptest2.ml
        if ERRORLEVEL 1 goto Error

        if NOT EXIST dont.run.peverify (
            "%PEVERIFY%" tmptest2.exe
            @if ERRORLEVEL 1 goto Error
        )
    )
)
goto :EOF

:EMPTY_SIGNATURE_OPT
@echo do :EMPTY_SIGNATURE_OPT
if NOT EXIST dont.use.empty.signature (
    if exist test.ml ( 
        echo Compiling against empty interface file...
        echo // empty file  > tmptest2.mli

        copy /y %source1% tmptest2.ml
        "%FSC%" %fsc_flags% --define:COMPILING_WITH_EMPTY_SIGNATURE --optimize -o:tmptest2--optimize.exe tmptest2.mli tmptest2.ml
        if ERRORLEVEL 1 goto Error

        if NOT EXIST dont.run.peverify (
            "%PEVERIFY%" tmptest2--optimize.exe
            @if ERRORLEVEL 1 goto Error
        )
    )
)
goto :EOF

:FSC_OPT_MINUS_DEBUG
@echo do :FSC_OPT_MINUS_DEBUG
    "%FSC%" %fsc_flags% --optimize- --debug -o:test--optminus--debug.exe -g %sources%
    if ERRORLEVEL 1 goto Error

    if NOT EXIST dont.run.peverify (
        "%PEVERIFY%" test--optminus--debug.exe
        @if ERRORLEVEL 1 goto Error
    )
goto :EOF

:FSC_OPT_PLUS_DEBUG
@echo do :FSC_OPT_PLUS_DEBUG
    "%FSC%" %fsc_flags% --optimize+ --debug -o:test--optplus--debug.exe -g %sources%
    if ERRORLEVEL 1 goto Error

    if NOT EXIST dont.run.peverify (
        "%PEVERIFY%" test--optplus--debug.exe
        @if ERRORLEVEL 1 goto Error
    )
)
goto :EOF

REM Compile as a DLL to exercise pickling of interface data, then recompile the original source file referencing this DLL
REM THe second compilation will not utilize the information from the first in any meaningful way, but the
REM compiler will unpickle the interface and optimization data, so we test unpickling as well.

:AS_DLL
@echo do :AS_DLL
if NOT EXIST dont.compile.test.as.dll (
    "%FSC%" %fsc_flags% --optimize -a -o:test--optimize-lib.dll -g %sources%
    if ERRORLEVEL 1 goto Error
    "%FSC%" %fsc_flags% --optimize -r:test--optimize-lib.dll -o:test--optimize-client-of-lib.exe -g %sources%
    if ERRORLEVEL 1 goto Error

    if NOT EXIST dont.run.peverify (
        "%PEVERIFY%" test--optimize-lib.dll
        @if ERRORLEVEL 1 goto Error
    )

    if NOT EXIST dont.run.peverify (
        "%PEVERIFY%" test--optimize-client-of-lib.exe
    )
    @if ERRORLEVEL 1 goto Error
)
goto :EOF

:WRAPPER_NAMESPACE
@echo do :WRAPPER_NAMESPACE
if NOT EXIST dont.use.wrapper.namespace (
    if exist test.ml (
        echo Compiling when wrapped in a namespace declaration...

        echo module TestNamespace.TestModule > tmptest3.ml
        type %source1%  >> tmptest3.ml

        "%FSC%" %fsc_flags% -o:tmptest3.exe tmptest3.ml
        if ERRORLEVEL 1 goto Error

        if NOT EXIST dont.run.peverify (
            "%PEVERIFY%" tmptest3.exe
            @if ERRORLEVEL 1 goto Error
        )
    )
)
goto :EOF

:WRAPPER_NAMESPACE_OPT
@echo do :WRAPPER_NAMESPACE
if NOT EXIST dont.use.wrapper.namespace (
    if exist test.ml (
        echo Compiling when wrapped in a namespace declaration...

        echo module TestNamespace.TestModule > tmptest3.ml
        type %source1%  >> tmptest3.ml
      
        "%FSC%" %fsc_flags% --optimize -o:tmptest3--optimize.exe tmptest3.ml
        if ERRORLEVEL 1 goto Error

        if NOT EXIST dont.run.peverify (
            "%PEVERIFY%" tmptest3--optimize.exe
            @if ERRORLEVEL 1 goto Error
        )
    )
)
goto :EOF
