@echo off

REM Use Link.exe from the nuget package

REM %LINK_EXE% -- Path to link.exe
REM %1 -- assembly to check
REM %2 -- expected value ("yes" or "no")
%LINK_EXE% /dump /headers %1 | find "High Entropy Virtual Addresses" > NUL
IF /I "%2"=="yes" IF     ERRORLEVEL 1 EXIT /B 1
IF /I "%2"=="no"  IF NOT ERRORLEVEL 1 EXIT /B 1