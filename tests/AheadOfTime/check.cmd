@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0Trimming\check.ps1""""
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0Equality\check.ps1""""
