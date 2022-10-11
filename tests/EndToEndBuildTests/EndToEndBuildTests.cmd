@echo off
rem
rem End to end tests
rem TestTypeProviders
rem

setlocal
set __scriptpath=%~dp0
set configuration=Debug

:parseargs
if "%1" == "" goto argsdone
if /i "%1" == "-c" goto set_configuration

echo Unsupported argument: %1
goto failure

:set_configuration
set configuration=%2
shift
shift
goto parseargs

:argsdone

echo %__scriptpath%BasicProvider\TestBasicProvider.cmd -c %configuration%
call %__scriptpath%BasicProvider\TestBasicProvider.cmd -c %configuration%
if ERRORLEVEL 1 echo Error: TestBasicProvider failed  && goto :failure

rem echo %__scriptpath%ComboProvider\TestComboProvider.cmd -c %configuration%
rem call %__scriptpath%ComboProvider\TestComboProvider.cmd -c %configuration%
rem if ERRORLEVEL 1 echo Error: TestComboProvider failed  && goto :failure

:success
endlocal
echo Succeeded
exit /b 0

:failure
endlocal
echo Failed
exit /b 1
