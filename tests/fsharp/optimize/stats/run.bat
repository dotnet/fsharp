@if "%_echo%"=="" echo off
REM there is no build.bat that precedes this run.bat

call %~d0%~p0..\..\..\config.bat
if ERRORLEVEL 1 goto Error

if not exist "%ILDASM%" (goto Error)

where sd.exe 2> NUL
if not ERRORLEVEL 1 ( sd edit stats.txt ) else (attrib -r stats.txt )

"%ILDASM%" /nobar /out=FSharp.Core.il "%FSCOREDLLPATH%"
if ERRORLEVEL 1 goto Error

echo Counting TypeFuncs...
type FSharp.Core.il | find /C "extends Microsoft.FSharp.TypeFunc"        > count-Microsoft.FSharp-TypeFunc
echo Counting classes...
type FSharp.Core.il | find /C ".class"                                   > count-Microsoft.FSharp-.class
echo Counting methods...
type FSharp.Core.il | find /C ".method"                                  > count-Microsoft.FSharp-.method
echo Counting fields...
type FSharp.Core.il | find /C ".field"                                   > count-Microsoft.FSharp-.field

for /f %%c IN (count-Microsoft.FSharp-TypeFunc) do (
 for /f %%d IN (count-Microsoft.FSharp-.class) do (
  for /f %%e IN (count-Microsoft.FSharp-.method) do (
   for /f %%f IN (count-Microsoft.FSharp-.field) do (
         echo %date%, %time%, Microsoft.FSharp-TypeFunc, %%c, Microsoft.FSharp-classes, %%d,  Microsoft.FSharp-methods, %%e, ,  Microsoft.FSharp-fields, %%f,  >> stats.txt
   )
  )
 )
)

:Ok
echo Ran fsharp %~f0 and added stats ok
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

