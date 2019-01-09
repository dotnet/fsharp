@echo off

%~dp0artifacts\bin\fsi\Release\net46\fsi.exe %~dp0src\scripts\VerifyAllTranslations.fsx -- %~dp0
