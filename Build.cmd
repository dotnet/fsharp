@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "Measure-Command { & """%~dp0eng\build.ps1""" -build -restore %* } | Select-Object TotalMinutes, TotalSeconds"
