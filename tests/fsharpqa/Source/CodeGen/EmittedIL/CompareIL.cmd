REM == %1 --> assembly
REM == %2 --> NetFx20|NetFx40 (default is NetFx20) - case insensitive
REM == %3 --> directory

if not '%3' == '' ( set dll=%3\%~nx1)
if '%3' == '' ( set dll=%~nx1)

REM
rem compute the location of ildasm
rem
if exist "%WindowsSDK_ExecutablePath_x86%" set WINSDKNETFXTOOLS_X86=%WindowsSDK_ExecutablePath_x86%
if not "%WindowsSDK_ExecutablePath_x86%" == "" goto :havesdk
set REGEXE32BIT=reg.exe
                                FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\NETFXSDK\4.6\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder')  DO SET WINSDKNETFXTOOLS_x86=%%B
if "%WINSDKNETFXTOOLS_x86%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS_x86=%%B
:havesdk

@echo "%WINSDKNETFXTOOLS_X86%ildasm.exe" /TEXT /LINENUM /NOBAR %dll% >%~n1.il
"%WINSDKNETFXTOOLS_X86%ildasm.exe" /TEXT /LINENUM /NOBAR %dll% >%~n1.il
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

