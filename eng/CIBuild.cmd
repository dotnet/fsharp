@echo off
powershell -NoProfile -ExecutionPolicy ByPass       -file "%~dp0\common\build.ps1" -build -restore -projects %~dp0\common\internal\Tools.csproj
powershell -noprofile -executionPolicy RemoteSigned -file "%~dp0\Build.ps1" -ci -restore -build -bootstrap -pack -sign -publish -binaryLog %*
