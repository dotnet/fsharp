@if "%_echo%"=="" echo off
setlocal
set ERRORMSG=

dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  set ERRORMSG=%ERRORMSG% Skipped because 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\config.bat
if errorlevel 1 (
  set ERRORMSG=%ERRORMSG% config.bat failed;
  goto :ERROR
)

if not exist "%FSC%" (
  set ERRORMSG=%ERRORMSG% fsc.exe not found at the location "%FSC%"
  goto :ERROR
)
if not exist "%FSI%" (
  set ERRORMSG=%ERRORMSG% fsi.exe not found at the location "%FSI%"
  goto :ERROR
)

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
if exist test2-hw.mli (set sourceshw=%sourceshw% test2-hw.mli)
if exist test2-hw.ml (set sourceshw=%sourceshw% test2-hw.ml)
if exist test-hw.fsi (set sourceshw=%sourceshw% test-hw.fsi)
if exist test-hw.fs (set sourceshw=%sourceshw% test-hw.fs)
if exist test2-hw.fsi (set sourceshw=%sourceshw% test2-hw.fsi)
if exist test2-hw.fs (set sourceshw=%sourceshw% test2-hw.fs)
if exist test-hw.fsx (set sourceshw=%sourceshw% test-hw.fsx)
if exist test2-hw.fsx (set sourceshw=%sourceshw% test2-hw.fsx)

:START

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
)

if not defined PERMUTATIONS (
    echo "PERMUTATIONS not defined. Running everything."
    set PERMUTATIONS=%PERMUTATIONS_LIST%
)

for %%A in (%PERMUTATIONS%) do (
    call :%%A
    IF ERRORLEVEL 1 EXIT /B 1
)

if "%ERRORMSG%"==""  goto Ok

set NonexistentErrorLevel 2> nul
goto :ERROR

:END

:EXIT_PATHS

:Ok
echo Ran fsharp %~f0 ok.
exit /b 0
goto :EOF

:Skip
echo Skipped %~f0
exit /b 0
goto :EOF

:Error
echo %ERRORMSG%
exit /b %ERRORLEVEL% 
goto :EOF

REM =========================================
REM THE TESTS
REM =========================================

:FSI_STDIN
@echo do :FSI_STDIN
if NOT EXIST dont.pipe.to.stdin (
  if exist test.ok (del /f /q test.ok)
  %CLIX% "%FSI%" %fsi_flags% < %sources% && (
  dir test.ok > NUL 2>&1 ) || (
  @echo FSI_STDIN failed;
  set ERRORMSG=%ERRORMSG% FSI_STDIN failed;
  )
)
goto :EOF

:FSI_STDIN_OPT
@echo do :FSI_STDIN_OPT
if NOT EXIST dont.pipe.to.stdin (
  if exist test.ok (del /f /q test.ok)
  %CLIX% "%FSI%" %fsi_flags% --optimize < %sources% && (
  dir test.ok > NUL 2>&1 ) || (
  @echo FSI_STDIN_OPT failed
  set ERRORMSG=%ERRORMSG% FSI_STDIN_OPT failed;
  )
)
goto :EOF

:FSI_STDIN_GUI
@echo do :FSI_STDIN_GUI
if NOT EXIST dont.pipe.to.stdin (
  if exist test.ok (del /f /q test.ok)
  %CLIX% "%FSI%" %fsi_flags% --gui < %sources% && (
  dir test.ok > NUL 2>&1 ) || (
  @echo FSI_STDIN_GUI failed;
  set ERRORMSG=%ERRORMSG% FSI_STDIN_GUI failed;
  )
)
goto :EOF

:FSI_FILE
@echo do :FSI_FILE
if NOT EXIST dont.run.as.script (
  if exist test.ok (del /f /q test.ok)
  %CLIX% "%FSI%" %fsi_flags% %sources% && (
  dir test.ok > NUL 2>&1 ) || (
  @echo FSI_FILE failed
  set ERRORMSG=%ERRORMSG% FSI_FILE failed;
  )
)
goto :EOF

:FSC_BASIC
@echo do :FSC_BASIC
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_BASIC failed
  set ERRORMSG=%ERRORMSG% FSC_BASIC failed;
  )
goto :EOF

:FSC_BASIC_64
@echo do :FSC_BASIC_64
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\testX64.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_BASIC_64 failed
  set ERRORMSG=%ERRORMSG% FSC_BASIC_64 failed;
  )
goto :EOF

:FSC_CORECLR
@echo do :FSC_CORECLR
  if exist test.ok (del /f /q test.ok)
  set platform=win7-x64
  set packagesDir=%~d0%~p0..\..\packages
  For %%A in ("%cd%") do ( Set TestCaseName=%%~nxA)
  %CLIX% %~d0%~p0..\testbin\%flavor%\coreclr\%platform%\corerun.exe %~d0%~p0..\testbin\%flavor%\coreclr\fsharp\core\%TestCaseName%\output\test.exe
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_CORECLR failed
  set ERRORMSG=%ERRORMSG% FSC_CORECLR failed;
  )
goto :EOF

:FSC_HW
@echo do :FSC_HW
if exist test-hw.* (
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test-hw.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo  :FSC_HW failed
  set ERRORMSG=%ERRORMSG% FSC_HW failed;
  )
)
goto :EOF

:FSC_O3
@echo do :FSC_O3
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test--optimize.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_O3 failed
  set ERRORMSG=%ERRORMSG% FSC_03 failed;
  )
goto :EOF

:FSC_OPT_MINUS_DEBUG
@echo do :FSC_OPT_MINUS_DEBUG
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test--optminus--debug.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_OPT_MINUS_DEBUG failed
  set ERRORMSG=%ERRORMSG% FSC_OPT_MINUS_DEBUG failed;
  )
goto :EOF

:FSC_OPT_PLUS_DEBUG
@echo do :FSC_OPT_PLUS_DEBUG
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test--optplus--debug.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FSC_OPT_PLUS_DEBUG failed
  set ERRORMSG=%ERRORMSG% FSC_OPT_PLUS_DEBUG failed;
  )
goto :EOF

:GENERATED_SIGNATURE
@echo do :GENERATED_SIGNATURE
if NOT EXIST dont.use.generated.signature (
  if exist test.ml (
    if exist test.ok (del /f /q test.ok)
    %CLIX% tmptest1.exe && (
    dir test.ok > NUL 2>&1 ) || (
    @echo :GENERATED_SIGNATURE failed
    set ERRORMSG=%ERRORMSG% FSC_GENERATED_SIGNATURE failed;
    )
  )
)
goto :EOF

:EMPTY_SIGNATURE
@echo do :EMPTY_SIGNATURE
if NOT EXIST dont.use.empty.signature (
  if exist test.ml (
    if exist test.ok (del /f /q test.ok)
    %CLIX% tmptest2.exe && (
    dir test.ok > NUL 2>&1 ) || (
    @echo :EMPTY_SIGNATURE failed
    set ERRORMSG=%ERRORMSG% FSC_EMPTY_SIGNATURE failed;
    )
  )
)
goto :EOF

:EMPTY_SIGNATURE_OPT
@echo do :EMPTY_SIGNATURE_OPT
if NOT EXIST dont.use.empty.signature (
  if exist test.ml (
    if exist test.ok (del /f /q test.ok)
    %CLIX% tmptest2--optimize.exe && (
      dir test.ok > NUL 2>&1 ) || (
      @echo :EMPTY_SIGNATURE_OPT --optimize failed
      set ERRORMSG=%ERRORMSG% EMPTY_SIGNATURE_OPT --optimize failed;
    )
  )
)
goto :EOF

:FRENCH
@echo do :FRENCH
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test.exe fr-FR && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :FRENCH failed
  set ERRORMSG=%ERRORMSG% FRENCH failed;
  )
goto :EOF

:SPANISH
@echo do :SPANISH
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test.exe es-ES && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :SPANISH failed
  set ERRORMSG=%ERRORMSG% SPANISH failed;
  )
goto :EOF

:AS_DLL
@echo do :AS_DLL
if NOT EXIST dont.compile.test.as.dll (
  if exist test.ok (del /f /q test.ok)
  %CLIX% .\test--optimize-client-of-lib.exe && (
  dir test.ok > NUL 2>&1 ) || (
  @echo :AS_DLL failed
  set ERRORMSG=%ERRORMSG% AS_DLL failed;
  )
)
goto :EOF

:WRAPPER_NAMESPACE
@echo do :WRAPPER_NAMESPACE
if NOT EXIST dont.use.wrapper.namespace (
  if exist test.ml (
    if exist test.ok (del /f /q test.ok)
    %CLIX% .\tmptest3.exe && (
    dir test.ok > NUL 2>&1 ) || (
    @echo :WRAPPER_NAMESPACE failed
    set ERRORMSG=%ERRORMSG% WRAPPER_NAMESPACE failed;
    )
  )
)
goto :EOF

:WRAPPER_NAMESPACE_OPT
@echo do :WRAPPER_NAMESPACE_OPT
if NOT EXIST dont.use.wrapper.namespace (
  if exist test.ml (
    if exist test.ok (del /f /q test.ok)
    %CLIX% .\tmptest3--optimize.exe && (
    dir test.ok > NUL 2>&1 ) || (
    @echo :WRAPPER_NAMESPACE_OPT failed
    set ERRORMSG=%ERRORMSG% WRAPPER_NAMESPACE_OPT failed;
    )
  )
)
goto :EOF