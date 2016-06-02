@echo off

REM Search for the Linker
REM Use VS2015 or fall back to VS2014
SET LINK_EXE="%VS150COMNTOOLS%\..\..\VC\bin\link.exe"
IF NOT EXIST %LINK_EXE% ( SET LINK_EXE="%VS140COMNTOOLS%..\..\VC\bin\link.exe" )
IF NOT EXIST %LINK_EXE% ( 
    @echo "Test Requires LINK.EXE" --- Not found 
    @echo "When installing VS please select "Select Visual C++ / Common Tools For Visual C++"
)

REM %LINK_EXE% -- Path to link.exe
REM %1 -- assembly to check
REM %2 -- expected value ("yes" or "no")
%LINK_EXE% /dump /headers %1 | find "High Entropy Virtual Addresses" > NUL
IF /I "%2"=="yes" IF     ERRORLEVEL 1 EXIT /B 1
IF /I "%2"=="no"  IF NOT ERRORLEVEL 1 EXIT /B 1