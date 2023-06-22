@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0\common\build.ps1""" -build -restore %*"