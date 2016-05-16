@echo off
"%~dp0..\tools\crossgen" %* >%2.out
if errorlevel 1 (
    set errorreply=%errorlevel%
    type %1.out
    exit /b %errorreply%
)
