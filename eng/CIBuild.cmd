@echo off
echo test
powershell -noprofile -executionPolicy RemoteSigned -file "%~dp0\Build.ps1" -ci -restore -build -bootstrap -pack -sign -publish -binaryLog %*
