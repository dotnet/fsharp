@Echo OFF
SETLOCAL
SET ERRORLEVEL=

"%~dp0dnx" %DNX_OPTIONS% "%~dp0lib\Microsoft.Dnx.Tooling\Microsoft.Dnx.Tooling.dll" %*

exit /b %ERRORLEVEL%
ENDLOCAL
