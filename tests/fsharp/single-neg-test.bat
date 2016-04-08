@if "%_echo%"=="" echo off
setlocal
set ERRORMSG=

call %~d0%~p0..\config.bat
if errorlevel 1 (
    set ERRORMSG=%ERRORMSG% config.bat failed;
    goto :ERROR
)
if not exist "%FSC%" (
  set ERRORMSG=Could not find FSC at path "%FSC%"
  goto :ERROR
)

%FSDIFF% %~f0 %~f0
@if ERRORLEVEL 1 (
    set ERRORMSG=%ERRORMSG% FSDIFF likely not found;
    goto Error
)

set testname=%1

REM == Set baseline (fsc vs vs, in case the vs baseline exists)
IF     EXIST %testname%.vsbsl (set BSLFILE=%testname%.vsbsl)
IF NOT EXIST %testname%.vsbsl (set BSLFILE=%testname%.bsl)

set sources=
if exist "%testname%.mli" (set sources=%sources% %testname%.mli)
if exist "%testname%.fsi" (set sources=%sources% %testname%.fsi)
if exist "%testname%.ml" (set sources=%sources% %testname%.ml)
if exist "%testname%.fs" (set sources=%sources% %testname%.fs)
if exist "%testname%.fsx" (set sources=%sources% %testname%.fsx)
if exist "%testname%a.mli" (set sources=%sources% %testname%a.mli)
if exist "%testname%a.fsi" (set sources=%sources% %testname%a.fsi)
if exist "%testname%a.ml" (set sources=%sources% %testname%a.ml)
if exist "%testname%a.fs" (set sources=%sources% %testname%a.fs)
if exist "%testname%b.mli" (set sources=%sources% %testname%b.mli)
if exist "%testname%b.fsi" (set sources=%sources% %testname%b.fsi)
if exist "%testname%b.ml" (set sources=%sources% %testname%b.ml)
if exist "%testname%b.fs" (set sources=%sources% %testname%b.fs)
if exist "helloWorldProvider.dll" (set sources=%sources% -r:helloWorldProvider.dll)

if exist "%testname%-pre.fs" (
    echo set sources=%sources% -r:%testname%-pre.dll
    set sources=%sources% -r:%testname%-pre.dll
)

REM check negative tests for bootstrapped fsc.exe due to line-ending differences
if "%FSC:fscp=X%" == "%FSC%" ( 

    if exist "%testname%-pre.fs" (
        echo "sources=%sources%"
	    "%FSC%" %fsc_flags% -a -o:%testname%-pre.dll  "%testname%-pre.fs" 
        @if ERRORLEVEL 1 (
            set ERRORMSG=%ERRORMSG% FSC failed for precursor library code for  %sources%;
            goto SetError
		)
    )

    echo Negative typechecker testing: %testname%
    echo "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources%
    "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% > %testname%.errors
    "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 1> %testname%.err1
    "%FSC%" %fsc_flags% --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 2> %testname%.err
    @if NOT ERRORLEVEL 1 (
        set ERRORMSG=%ERRORMSG% FSC passed unexpectedly for  %sources%;
        goto SetError
    )

    %FSDIFF% %testname%.err %testname%.bsl > %testname%.diff
    for /f %%c IN (%testname%.diff) do (
        echo ***** %testname%.err %testname%.bsl differed: a bug or baseline may neeed updating
        set ERRORMSG=%ERRORMSG% %testname%.err %testname%.bsl differ;

        IF DEFINED WINDIFF  (start %windiff% %testname%.bsl  %testname%.err)
        goto SetError
    )
    echo Good, output %testname%.err matched %testname%.bsl

    echo "%FSC%" %fsc_flags% --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources%
    "%FSC%" %fsc_flags% --test:ContinueAfterParseFailure --vserrors --warnaserror --nologo --maxerrors:10000 -a -o:%testname%.dll  %sources% 2> %testname%.vserr
    @if NOT ERRORLEVEL 1 (
        set ERRORMSG=%ERRORMSG% FSC passed unexpectedly for  %sources%;
        goto SetError
    )

    %FSDIFF% %testname%.vserr %BSLFILE% > %testname%.vsdiff

    for /f %%c IN (%testname%.vsdiff) do (
        echo ***** %testname%.vserr %BSLFILE% differed: a bug or baseline may neeed updating
        set ERRORMSG=%ERRORMSG% %testname%.vserr %BSLFILE% differ;
        IF DEFINED WINDIFF  (start %windiff% %BSLFILE%  %testname%.vserr)
        goto SetError
    )
    echo Good, output %testname%.vserr matched %BSLFILE%
)

:Ok
echo Ran fsharp %~f0 ok.
endlocal
exit /b 0
goto :EOF

:Skip
echo Skipped %~f0
endlocal
exit /b 0
goto :EOF

:Error
echo %ERRORMSG%
exit /b %ERRORLEVEL% 
goto :EOF

:SETERROR
set NonexistentErrorLevel 2> nul
goto Error
goto :EOF

