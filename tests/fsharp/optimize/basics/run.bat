@if "%_echo%"=="" echo off

setlocal
dir build.ok > NUL ) || (
  @echo 'build.ok' not found.
  goto :ERROR
)

call %~d0%~p0..\..\..\config.bat
if ERRORLEVEL 1 goto Error

if not exist "%ILDASM%" (
   @echo '%ILDASM%' not found.
    goto Error 
)

"%ILDASM%" /nobar /out=test.il test.exe

"%ILDASM%" /nobar /out=test--optimize.il test--optimize.exe

type test--optimize.il | find /C "ShouldNotAppear" > count--optimize
type test.il | find /C "ShouldNotAppear" > count
for /f %%c IN (count--optimize) do (if NOT "%%c"=="0" (
   echo Error: optimizations not removed.  Relevant lines from IL file follow:
   type test--optimize.il | find "ShouldNotAppear"
   goto SetError)
)
for /f %%c IN (count) do (
   set NUMELIM=%%c
)

:Ok
echo Ran fsharp %~f0 ok - optimizations removed %NUMELIM% textual occurrences of optimizable identifiers from target IL 
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

:SetError
set NonexistentErrorLevel 2> nul
goto Error
