@echo off

where >NUL 2>&1 link
IF ERRORLEVEL 1 (
    IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
        echo Script requires link.exe, but Ultimate is not installed. Skipping.
        exit /b 0
    )
)

REM %1 -- assembly to check
REM %2 -- expected value ("yes" or "no")
link /dump /headers %1 | find "High Entropy Virtual Addresses" > NUL
IF /I "%2"=="yes" IF     ERRORLEVEL 1 EXIT /B 1
IF /I "%2"=="no"  IF NOT ERRORLEVEL 1 EXIT /B 1