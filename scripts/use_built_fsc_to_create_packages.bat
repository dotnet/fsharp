@ECHO OFF

SET ROOT_DIR=%~dp0%..
FOR /F "delims=" %%F IN ("%ROOT_DIR%") DO SET "ROOT_DIR=%%~fF"

echo ================================================================================
echo Sanity check: Use the built fsc to rebuild all projects
echo 
echo The dotnet-compile-fsc locate the fsc using the env var DOTNET_FSC_PATH, if set
echo ================================================================================

REM
REM Copy built Fsc to a tempo dir and set the env var DOTNET_FSC_PATH
REM

SET BUILT_FSC_TEMP_DIR=%ROOT_DIR%\bin\builtfsc

RD /S /Q "%BUILT_FSC_TEMP_DIR%"

mkdir "%BUILT_FSC_TEMP_DIR%"
if ERRORLEVEL 1 GOTO :FAIL

cd "%ROOT_DIR%\src\fsharp\Fsc"
if ERRORLEVEL 1 GOTO :FAIL

dotnet --verbose publish -c Release -o "%BUILT_FSC_TEMP_DIR%"
if ERRORLEVEL 1 GOTO :FAIL

SET DOTNET_FSC_EXEC=run
SET DOTNET_FSC_PATH=%BUILT_FSC_TEMP_DIR%\Fsc.exe

echo "Using (%DOTNET_FSC_EXEC%) fsc=%DOTNET_FSC_PATH%"

REM
REM call the build script
REM

call  "%ROOT_DIR%\scripts\create-packages.bat"
if ERRORLEVEL 1 GOTO :FAIL

:DONE

goto :EOF

:FAIL

echo Failed with errors
exit /B 1
