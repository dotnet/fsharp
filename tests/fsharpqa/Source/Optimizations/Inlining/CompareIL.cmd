REM == %1 --> assembly
REM == %2 --> token that identify the method

ildasm /TEXT /NOBAR "%~nx1" >"%~n1.il"
IF NOT ERRORLEVEL 0 exit /b 1

fsi --debug- --nologo --quiet --exec Comparer.fsx "%~n1.il.bsl" "%~n1.il" "%~2"
