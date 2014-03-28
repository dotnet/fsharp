@echo off
REM %1 -- assembly to check
REM %2 -- expected value ("4.00" etc...)

where >NUL 2>&1 link
IF ERRORLEVEL 1 (
    IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
        echo Script requires link.exe, but Ultimate is not installed. Skipping.
        exit /b 0
    )
)

link /dump /headers %1 | find "%2 subsystem version" > NUL
IF ERRORLEVEL 1 EXIT /B 1
EXIT /B 0
