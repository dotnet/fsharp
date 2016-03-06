@ECHO OFF

SET ROOT_DIR=%~dp0%..
FOR /F "delims=" %%F IN ("%ROOT_DIR%") DO SET "ROOT_DIR=%%~fF"

echo ==============================================
echo normal build using .NET cli fsc
echo ==============================================

SET DOTNET_FSC_PATH=
SET DOTNET_FSC_EXEC=

call "%ROOT_DIR%\scripts\create-packages.bat"
if ERRORLEVEL 1 GOTO :FAIL

echo =====================================================
echo Sanity check: Rebuild eveything using built fsc.exe
echo =====================================================

call "%ROOT_DIR%\scripts\use_built_fsc_to_create_packages.bat"
if ERRORLEVEL 1 GOTO :FAIL


:DONE

goto :EOF

:FAIL

echo Failed with errors
exit /B 1
