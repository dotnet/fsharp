@echo off
setlocal

set "TESTDIR=%~dp0"
set "REPOROOT=%TESTDIR%..\.."
set "PROJECT=%TESTDIR%FSharp.Compiler.LanguageServer.IntegrationTests.csproj"
set "RUNSETTINGS=%TESTDIR%.runsettings"
set "CONFIG=Release"

:: Resolve vstest.console.dll path
if not defined VSTEST_CONSOLE_PATH (
    for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -find **\vstest.console.dll`) do if not defined VSTEST_CONSOLE_PATH set "VSTEST_CONSOLE_PATH=%%i"
)

if not exist "%VSTEST_CONSOLE_PATH%" (
    echo ERROR: vstest.console.dll not found at:
    echo   %VSTEST_CONSOLE_PATH%
    echo.
    echo Set the VSTEST_CONSOLE_PATH environment variable to the correct path.
    exit /b 1
)

:: Build the test project
echo === Building %PROJECT% ===
dotnet build "%PROJECT%" -c %CONFIG%
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed.
    exit /b %ERRORLEVEL%
)

:: Locate the compiled test assembly
set "TESTDLL=%REPOROOT%\artifacts\bin\FSharp.Compiler.LanguageServer.IntegrationTests\%CONFIG%\net472\FSharp.Compiler.LanguageServer.IntegrationTests.dll"
if not exist "%TESTDLL%" (
    echo ERROR: Test assembly not found at:
    echo   %TESTDLL%
    exit /b 1
)

:: Run the tests
echo.
echo === Running tests ===
echo vstest: %VSTEST_CONSOLE_PATH%
echo assembly: %TESTDLL%
echo.
dotnet exec "%VSTEST_CONSOLE_PATH%" "%TESTDLL%" /Settings:"%RUNSETTINGS%" %*

exit /b %ERRORLEVEL%
