@Echo OFF
SETLOCAL
SET ERRORLEVEL=

SET "DNX_RUNTIME_PATH=%~dp0."
SET "CROSSGEN_PATH=%~dp0crossgen.exe"

"%~dp0dnx" --appbase "%CD%" --lib "%~dp0lib\Microsoft.Dnx.Project" "Microsoft.Dnx.Project" crossgen --exePath "%CROSSGEN_PATH%" --runtimePath "%DNX_RUNTIME_PATH%" %*

exit /b %ERRORLEVEL%
ENDLOCAL
