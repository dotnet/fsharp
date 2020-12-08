REM == %1 --> assembly

ildasm /TEXT /LINENUM /NOBAR "%~nx1" >"%~n1.il"
IF NOT ERRORLEVEL 0 exit 1

echo %~dp0..\..\..\testenv\bin\ILComparer.exe "%~n1.il.bsl" "%~n1.il"
     %~dp0..\..\..\testenv\bin\ILComparer.exe "%~n1.il.bsl" "%~n1.il"

if /i "%TEST_UPDATE_BSL%" == "1" (
  echo copy /y "%~n1.il" "%~n1.il.bsl"
  copy /y "%~n1.il" "%~n1.il.bsl"
)

exit /b %ERRORLEVEL%

