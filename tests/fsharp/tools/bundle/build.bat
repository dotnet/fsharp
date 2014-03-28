@if "%_echo%"=="" echo off

setlocal
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat


if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)


"%FSC%" %fsc_flags% --progress --standalone -o:test-one-fsharp-module.exe -g test-one-fsharp-module.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%"  test-one-fsharp-module.exe
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:test_two_fsharp_modules_module_1.dll -g test_two_fsharp_modules_module_1.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%"  test_two_fsharp_modules_module_1.dll
if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g test_two_fsharp_modules_module_2.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%"  test_two_fsharp_modules_module_2.exe
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g test_two_fsharp_modules_module_2.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%"  test_two_fsharp_modules_module_2_as_dll.dll
if ERRORLEVEL 1 goto Error


:Ok
echo Built fsharp %~f0 ok.
echo. > build.ok
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%
