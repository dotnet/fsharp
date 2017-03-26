@echo off

setlocal enableDelayedExpansion

if "" == "%link_exe%" (
    echo "it is blank"
)

set link_exe=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\VC\Tools\MSVC\14.10.25017\bin\HostX64\x86\link.exe

if "" == "%link_exe%" (
    echo "it is blank"
)
if exist "%link_exe%"  (
    echo link_exe exist "%link_exe%"
)
if "" == "%link_exe%" (
    echo "it is blank"
    set link_exe="%~dp0packages\VisualCppTools.14.0.24519-Pre\lib\native\bin\link.exe"
)
echo link_exe=%link_exe%

REM set link_exe_short=C:\PROGRA~2\MIB055~1\2017\PROFES~1\VC\Tools\MSVC\1410~1.250\bin\HostX64\x86\link.exe

REM if exist "%link_exe_short%"  (
REM     echo link_exe_short exist "%link_exe_short%"
REM )

REM set VCToolsInstallDir=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\VC\Tools\MSVC\14.10.25017\
REM set link_exe=%VCToolsInstallDir%bin\HostX64\x86\link.exe

REM if not exist "%link_exe%"  (
REM     echo link_exe exist "%link_exe%""
REM )

REM if "" == "%link_exe%" (
REM     echo "it is blank"
REM )