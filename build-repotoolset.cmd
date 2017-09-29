@echo off
powershell -ExecutionPolicy ByPass %~dp0build\Build.ps1 -restore -build -deploy %*
exit /b %ErrorLevel%
