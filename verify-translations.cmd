@echo off

%~dp0artifacts\bin\fsi\Release\net462\fsi.exe %~dp0src\scripts\VerifyAllTranslations.fsx -- %~dp0
