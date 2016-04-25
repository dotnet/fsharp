REM == %1 --> assembly
REM == %2 --> NetFx20|NetFx40 (default is NetFx20) - case insensitive
REM == %3 --> directory

if not '%3' == '' ( set dll=%3\%~nx1)
if '%3' == '' ( set dll=%~nx1)

"%ildasm%" /TEXT /LINENUM /NOBAR %dll% >%~n1.il
IF NOT ERRORLEVEL 0 exit 1

IF /I     "%2"=="NetFx40" goto :NetFx4
IF /I NOT "%2"=="NetFx40" goto :NetFx2
exit 1

:NetFx4
..\..\..\..\testenv\bin\ILComparer.exe "%~n1.il.netfx4.bsl" "%~n1.il"
exit /b %ERRORLEVEL%

:NetFx2
..\..\..\..\testenv\bin\ILComparer.exe "%~n1.il.bsl"        "%~n1.il"
exit /b %ERRORLEVEL%

