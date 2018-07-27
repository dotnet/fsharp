@echo off
rem
rem End to end tests
rem TestTypeProviders
rem

setlocal
set __scriptpath=%~dp0

@echo %__scriptpath%BasicProvider\TestBasicProvider.cmd
call %__scriptpath%BasicProvider\TestBasicProvider.cmd
@if ERRORLEVEL 1 echo Error: TestBasicProvider failed  && goto :failure

@echo %__scriptpath%ComboProvider\TestComboProvider.cmd
call %__scriptpath%ComboProvider\TestComboProvider.cmd
@if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

:success
endlocal
echo Succeeded
exit /b 0


:failure
endlocal
echo Failed
exit /b 1
