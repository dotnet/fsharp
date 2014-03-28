@if "%_echo%"=="" echo off

setlocal 
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

"%FSC%" %fsc_flags% --optimize -o both69514.exe -g lib69514.fs app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" both69514.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize- -o both69514-noopt.exe -g lib69514.fs app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" both69514-noopt.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize -a -g lib69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib69514.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -r:lib69514.dll -g app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" app69514.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize- -o:lib69514-noopt.dll -a -g lib69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib69514-noopt.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" app69514-noopt.exe
@if ERRORLEVEL 1 goto Error



"%FSC%" %fsc_flags% --optimize- -o:lib69514-noopt-withsig.dll -a -g lib69514.fsi lib69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib69514-noopt-withsig.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" app69514-noopt-withsig.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -o:lib69514-withsig.dll -a -g lib69514.fsi lib69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib69514-withsig.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -r:lib69514-withsig.dll -o:app69514-withsig.exe -g app69514.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" app69514-withsig.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -o:lib.dll -a -g lib.ml
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib.dll
@if ERRORLEVEL 1 goto Error

%CSC% /nologo /r:"%FSCOREDLLPATH%" /r:lib.dll /out:test.exe test.cs 
if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -o:lib--optimize.dll -a -g lib.ml
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" lib--optimize.dll
@if ERRORLEVEL 1 goto Error

%CSC% /nologo /r:"%FSCOREDLLPATH%" /r:lib--optimize.dll /out:test--optimize.exe test.cs 
if ERRORLEVEL 1 goto Error

set dicases= flag_deterministic_init1.fs lib_deterministic_init1.fs flag_deterministic_init2.fs lib_deterministic_init2.fs flag_deterministic_init3.fs lib_deterministic_init3.fs flag_deterministic_init4.fs lib_deterministic_init4.fs flag_deterministic_init5.fs lib_deterministic_init5.fs flag_deterministic_init6.fs lib_deterministic_init6.fs flag_deterministic_init7.fs lib_deterministic_init7.fs flag_deterministic_init8.fs lib_deterministic_init8.fs flag_deterministic_init9.fs lib_deterministic_init9.fs flag_deterministic_init10.fs lib_deterministic_init10.fs flag_deterministic_init11.fs lib_deterministic_init11.fs flag_deterministic_init12.fs lib_deterministic_init12.fs flag_deterministic_init13.fs lib_deterministic_init13.fs flag_deterministic_init14.fs lib_deterministic_init14.fs flag_deterministic_init15.fs lib_deterministic_init15.fs flag_deterministic_init16.fs lib_deterministic_init16.fs flag_deterministic_init17.fs lib_deterministic_init17.fs flag_deterministic_init18.fs lib_deterministic_init18.fs flag_deterministic_init19.fs lib_deterministic_init19.fs flag_deterministic_init20.fs lib_deterministic_init20.fs flag_deterministic_init21.fs lib_deterministic_init21.fs flag_deterministic_init22.fs lib_deterministic_init22.fs flag_deterministic_init23.fs lib_deterministic_init23.fs flag_deterministic_init24.fs lib_deterministic_init24.fs flag_deterministic_init25.fs lib_deterministic_init25.fs flag_deterministic_init26.fs lib_deterministic_init26.fs flag_deterministic_init27.fs lib_deterministic_init27.fs flag_deterministic_init28.fs lib_deterministic_init28.fs flag_deterministic_init29.fs lib_deterministic_init29.fs flag_deterministic_init30.fs lib_deterministic_init30.fs flag_deterministic_init31.fs lib_deterministic_init31.fs flag_deterministic_init32.fs lib_deterministic_init32.fs flag_deterministic_init33.fs lib_deterministic_init33.fs flag_deterministic_init34.fs lib_deterministic_init34.fs flag_deterministic_init35.fs lib_deterministic_init35.fs flag_deterministic_init36.fs lib_deterministic_init36.fs flag_deterministic_init37.fs lib_deterministic_init37.fs flag_deterministic_init38.fs lib_deterministic_init38.fs flag_deterministic_init39.fs lib_deterministic_init39.fs flag_deterministic_init40.fs lib_deterministic_init40.fs flag_deterministic_init41.fs lib_deterministic_init41.fs flag_deterministic_init42.fs lib_deterministic_init42.fs flag_deterministic_init43.fs lib_deterministic_init43.fs flag_deterministic_init44.fs lib_deterministic_init44.fs flag_deterministic_init45.fs lib_deterministic_init45.fs flag_deterministic_init46.fs lib_deterministic_init46.fs flag_deterministic_init47.fs lib_deterministic_init47.fs flag_deterministic_init48.fs lib_deterministic_init48.fs flag_deterministic_init49.fs lib_deterministic_init49.fs flag_deterministic_init50.fs lib_deterministic_init50.fs flag_deterministic_init51.fs lib_deterministic_init51.fs flag_deterministic_init52.fs lib_deterministic_init52.fs flag_deterministic_init53.fs lib_deterministic_init53.fs flag_deterministic_init54.fs lib_deterministic_init54.fs flag_deterministic_init55.fs lib_deterministic_init55.fs flag_deterministic_init56.fs lib_deterministic_init56.fs flag_deterministic_init57.fs lib_deterministic_init57.fs flag_deterministic_init58.fs lib_deterministic_init58.fs flag_deterministic_init59.fs lib_deterministic_init59.fs flag_deterministic_init60.fs lib_deterministic_init60.fs flag_deterministic_init61.fs lib_deterministic_init61.fs flag_deterministic_init62.fs lib_deterministic_init62.fs flag_deterministic_init63.fs lib_deterministic_init63.fs flag_deterministic_init64.fs lib_deterministic_init64.fs flag_deterministic_init65.fs lib_deterministic_init65.fs flag_deterministic_init66.fs lib_deterministic_init66.fs flag_deterministic_init67.fs lib_deterministic_init67.fs flag_deterministic_init68.fs lib_deterministic_init68.fs flag_deterministic_init69.fs lib_deterministic_init69.fs flag_deterministic_init70.fs lib_deterministic_init70.fs flag_deterministic_init71.fs lib_deterministic_init71.fs flag_deterministic_init72.fs lib_deterministic_init72.fs flag_deterministic_init73.fs lib_deterministic_init73.fs flag_deterministic_init74.fs lib_deterministic_init74.fs flag_deterministic_init75.fs lib_deterministic_init75.fs flag_deterministic_init76.fs lib_deterministic_init76.fs flag_deterministic_init77.fs lib_deterministic_init77.fs flag_deterministic_init78.fs lib_deterministic_init78.fs flag_deterministic_init79.fs lib_deterministic_init79.fs flag_deterministic_init80.fs lib_deterministic_init80.fs flag_deterministic_init81.fs lib_deterministic_init81.fs flag_deterministic_init82.fs lib_deterministic_init82.fs flag_deterministic_init83.fs lib_deterministic_init83.fs flag_deterministic_init84.fs lib_deterministic_init84.fs flag_deterministic_init85.fs lib_deterministic_init85.fs 

"%FSC%" %fsc_flags% --optimize- -o test_deterministic_init.exe %dicases% test_deterministic_init.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -o test_deterministic_init--optimize.exe %dicases% test_deterministic_init.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init--optimize.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize- -a -o test_deterministic_init_lib.dll %dicases% 
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init_lib.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe test_deterministic_init.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init_exe.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -a -o test_deterministic_init_lib--optimize.dll %dicases% 
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init_lib--optimize.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe test_deterministic_init.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_deterministic_init_exe--optimize.exe
@if ERRORLEVEL 1 goto Error



set static_init_cases= test0.fs test1.fs test2.fs test3.fs test4.fs test5.fs test6.fs

"%FSC%" %fsc_flags% --optimize- -o test_static_init.exe %static_init_cases% static-main.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -o test_static_init--optimize.exe %static_init_cases% static-main.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init--optimize.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --optimize- -a -o test_static_init_lib.dll %static_init_cases% 
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init_lib.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe static-main.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init_exe.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -a -o test_static_init_lib--optimize.dll %static_init_cases% 
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init_lib--optimize.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --optimize -r test_static_init_lib--optimize.dll -o test_static_init_exe--optimize.exe static-main.fs
if ERRORLEVEL 1 goto Error

"%PEVERIFY%" test_static_init_exe--optimize.exe
@if ERRORLEVEL 1 goto Error



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

