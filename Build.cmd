@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "$timer = Measure-Command { & """%~dp0eng\build.ps1""" -build -restore %* }; $exitCode = $LASTEXITCODE; $timer | Select-Object TotalMinutes, TotalSeconds; exit $exitCode"
