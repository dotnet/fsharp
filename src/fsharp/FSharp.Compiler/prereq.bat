@ECHO OFF

SET PROJ_DIR=%~dp0%

echo ==============================================
echo Run fssrgen
echo ==============================================
REM
REM in precompile was
REM   "dotnet fssrgen \"%project:Directory%\\..\\FSComp.txt\" \"%project:Directory%\\FSComp.fs\"  \"%project:Directory%\\FSComp.resx\" DNXCORE50"
REM
REM but a precompile event disable incremental compilation, do let's do that before build
REM

dotnet fssrgen "%PROJ_DIR%\..\FSComp.txt" "%PROJ_DIR%\FSComp.fs"  "%PROJ_DIR%\FSComp.resx" DNXCORE50
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo Run FsLex
echo ==============================================

SET FSLEX=%PROJ_DIR%..\..\..\lkg\FSharp-14.0.23413.0\bin\FsLex.exe

"%FSLEX%" --unicode -o pplex.fs --lexlib Internal.Utilities.Text.Lexing  "%PROJ_DIR%\..\pplex.fsl" 
if ERRORLEVEL 1 GOTO :FAIL

"%FSLEX%" --unicode -o lex.fs --lexlib Internal.Utilities.Text.Lexing  "%PROJ_DIR%\..\lex.fsl" 
if ERRORLEVEL 1 GOTO :FAIL

"%FSLEX%" --unicode -o illex.fs --lexlib Internal.Utilities.Text.Lexing  "%PROJ_DIR%\..\..\absil\illex.fsl"
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo Run FsYacc
echo ==============================================

SET FSYACC=%PROJ_DIR%..\..\..\lkg\FSharp-14.0.23413.0\bin\FsYacc.exe

"%FSYACC%" --open Microsoft.FSharp.Compiler --module Microsoft.FSharp.Compiler.PPParser -o pppars.fs --internal --lexlib Internal.Utilities.Text.Lexing --parslib Internal.Utilities.Text.Parsing  "%PROJ_DIR%\..\pppars.fsy" 
if ERRORLEVEL 1 GOTO :FAIL

"%FSYACC%" --open Microsoft.FSharp.Compiler --module Microsoft.FSharp.Compiler.Parser -o pars.fs --internal --lexlib Internal.Utilities.Text.Lexing --parslib Internal.Utilities.Text.Parsing  "%PROJ_DIR%\..\pars.fsy" 
if ERRORLEVEL 1 GOTO :FAIL

"%FSYACC%" --open Microsoft.FSharp.Compiler.AbstractIL --module Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiParser -o ilpars.fs --internal --lexlib Internal.Utilities.Text.Lexing --parslib Internal.Utilities.Text.Parsing  "%PROJ_DIR%\..\..\absil\ilpars.fsy"
if ERRORLEVEL 1 GOTO :FAIL

:DONE

echo Done.
GOTO :EOF

:FAIL

echo Failed with errors.
exit /B 1
