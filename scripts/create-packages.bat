@ECHO OFF

SET ROOT_DIR=%~dp0%..
FOR /F "delims=" %%F IN ("%ROOT_DIR%") DO SET "ROOT_DIR=%%~fF"

echo ==============================================
echo .NET cli conf
echo ==============================================

REM response file are ok
SET DOTNET_COMPILEFSC_USE_RESPONSE_FILE=1

echo DOTNET_COMPILEFSC_USE_RESPONSE_FILE=%DOTNET_COMPILEFSC_USE_RESPONSE_FILE%
echo DOTNET_FSC_PATH=%DOTNET_FSC_PATH%
echo DOTNET_FSC_EXEC=%DOTNET_FSC_EXEC%

echo ==============================================
echo Restore
echo ==============================================

cd "%ROOT_DIR%\src\fsharp"
if ERRORLEVEL 1 GOTO :FAIL

dotnet restore
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo FSharp.Compiler prerequisites
echo ==============================================

cd "%ROOT_DIR%\src\fsharp\FSharp.Compiler"
if ERRORLEVEL 1 GOTO :FAIL

call "prereq.bat"
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo Build FSharp.Core
echo ==============================================

cd "%ROOT_DIR%\src\fsharp\FSharp.Core"
if ERRORLEVEL 1 GOTO :FAIL

dotnet --verbose build --configuration Release
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo Build FSharp.Compiler
echo ==============================================

cd "%ROOT_DIR%\src\fsharp\FSharp.Compiler"
if ERRORLEVEL 1 GOTO :FAIL

dotnet --verbose build --configuration Release
if ERRORLEVEL 1 GOTO :FAIL

echo ==============================================
echo Build Fsc
echo ==============================================

cd "%ROOT_DIR%\src\fsharp\Fsc"
if ERRORLEVEL 1 GOTO :FAIL

dotnet --verbose build --configuration Release
if ERRORLEVEL 1 GOTO :FAIL

:DONE

goto :EOF

:FAIL

echo Failed with errors
exit /B 1
