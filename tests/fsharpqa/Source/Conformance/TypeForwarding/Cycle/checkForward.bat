@echo off
call %1
if errorlevel == 1 exit 1
if errorlevel == 0 exit -1
if errorlevel == -1 exit 0
if errorlevel == %2 exit 0
exit 1