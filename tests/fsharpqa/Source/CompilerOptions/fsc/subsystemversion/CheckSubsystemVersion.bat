@echo off

REM %LINK_EXE% -- Path to link.exe
REM %1 -- assembly to check
REM %2 -- expected value ("4.00" etc...)

%LINK_EXE% /dump /headers %1 | find "%2 subsystem version" > NUL
IF ERRORLEVEL 1 EXIT /B 1
EXIT /B 0
