@echo OFF
setlocal

rem "ttags" indicates what test areas will be run, based on the tags in the test.lst files
set TTAGS_ARG=
set _tmp=%2
if not "%_tmp%" == "" set TTAGS_ARG=-ttags:%_tmp:"=%

rem "nottags" indicates which test areas/test cases will NOT be run, based on the tags in the test.lst and env.lst files
set NO_TTAGS_ARG=-nottags:ReqPP
set _tmp=%3
if not "%3" == "" set NO_TTAGS_ARG=-nottags:ReqPP,%_tmp:"=%

rem Use commented line to enable parallel execution of tests
set PARALLEL_ARG=
set PARALLEL_ARG=-procs:3

rem This can be set to 1 to reduce the number of permutations used and avoid some of the extra-time-consuming tests
set REDUCED_RUNTIME=1
if "%REDUCED_RUNTIME%" == "1" set NO_TTAGS_ARG=%NO_TTAGS_ARG%,Expensive

rem Set this to 1 in order to use an external compiler host process
rem    This only has an effect when running the FSHARPQA tests, but can
rem    greatly speed up execution since fsc.exe does not need to be spawned thousands of times
rem set HOSTED_COMPILER=1

if /I "%1" == "fsharp" (goto :FSHARP)
if /I "%1" == "fsharpqa" (goto :FSHARPQA)

echo Usage:
echo.
echo RunTests.cmd ^<fsharp^|fsharpqa^>
echo.
exit /b 1


:FSHARP

echo perl %~dp0\fsharpqa\testenv\bin\runall.pl %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
     perl %~dp0\fsharpqa\testenv\bin\runall.pl %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
goto :EOF


:FSHARPQA

set OSARCH=%PROCESSOR_ARCHITECTURE%

set X86_PROGRAMFILES=%ProgramFiles%
if "%OSARCH%"=="AMD64" set X86_PROGRAMFILES=%ProgramFiles(x86)%

set REGEXE32BIT=reg.exe
if not "%OSARCH%"=="x86" set REGEXE32BIT=%WINDIR%\syswow64\reg.exe

                            FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.1A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.1\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
if "%WINSDKNETFXTOOLS%"=="" FOR /F "tokens=2* delims=	 " %%A IN ('%REGEXE32BIT% QUERY "HKLM\Software\Microsoft\Microsoft SDKs\Windows\v7.0A\WinSDK-NetFx40Tools" /v InstallationFolder') DO SET WINSDKNETFXTOOLS=%%B
set PATH=%PATH%;%WINSDKNETFXTOOLS%

IF NOT DEFINED SNEXE32 IF EXIST "%WINSDKNETFXTOOLS%sn.exe"               set SNEXE32=%WINSDKNETFXTOOLS%sn.exe
IF NOT DEFINED SNEXE64 IF EXIST "%WINSDKNETFXTOOLS%x64\sn.exe"           set SNEXE64=%WINSDKNETFXTOOLS%x64\sn.exe
IF NOT DEFINED GACUTILEXE32 IF EXIST "%WINSDKNETFXTOOLS%gacutil.exe"     set GACUTILEXE32=%WINSDKNETFXTOOLS%gacutil.exe
IF NOT DEFINED GACUTILEXE64 IF EXIST "%WINSDKNETFXTOOLS%x64\gacutil.exe" set GACUTILEXE64=%WINSDKNETFXTOOLS%x64\gacutil.exe

for /f "tokens=1-2*" %%a in ('%REGEXE32BIT% QUERY "HKLM\SOFTWARE\Microsoft\FSharp\3.1\Runtime\v4.0" /ve') DO set FSCPATH=%%c
set PATH=%PATH%;%FSCPATH%

for /d %%i in (%WINDIR%\Microsoft.NET\Framework\v4.0.?????) do set CORDIR=%%i
set PATH=%PATH%;%CORDIR%

if not exist %WINDIR%\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll set NO_TTAGS_ARG=%NO_TTAGS_ARG%,Req20 

pushd %~dp0\fsharpqa\source

echo perl %~dp0\fsharpqa\testenv\bin\runall.pl %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%
     perl %~dp0\fsharpqa\testenv\bin\runall.pl %TTAGS_ARG% %NO_TTAGS_ARG% %PARALLEL_ARG%

popd
goto :EOF
