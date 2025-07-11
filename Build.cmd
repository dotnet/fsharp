@echo off 
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0eng\common\build.ps1""" -build -restore -projects %~dp0eng\common\internal\Tools.csproj"
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0eng\build.ps1""" -build -restore %*"
