@if "%_echo%"=="" echo off

echo on

setlocal

call %~d0%~p0..\..\..\config.bat
@if ErrorLEVEL 1 goto :Error


  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-nosig-codepage-65001.fs
   IF ERRORLEVEL 1 goto :Error

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% --utf8output --codepage:65001 kanji-unicode-utf8-withsig-codepage-65001.fs
   IF ERRORLEVEL 1 goto :Error

REM  if exist test.ok (del /f /q test.ok)
REM  "%FSI%" %fsi_flags% --utf8output --codepage:65001 < kanji-unicode-utf8-withsig-codepage-65001.fs
REM   IF ERRORLEVEL 1 goto :Error

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-withsig-codepage-65001.fs
   IF ERRORLEVEL 1 goto :Error


REM  if exist test.ok (del /f /q test.ok)
REM  "%FSI%" %fsi_flags% --utf8output < kanji-unicode-utf8-withsig-codepage-65001.fs
REM   IF ERRORLEVEL 1 goto :Error

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% --utf8output --codepage:65000  kanji-unicode-utf7-codepage-65000.fs
   IF ERRORLEVEL 1 goto :Error

REM  if exist test.ok (del /f /q test.ok)
REM  "%FSI%" %fsi_flags% --utf8output --codepage:65000  < kanji-unicode-utf7-codepage-65000.fs
REM   IF ERRORLEVEL 1 goto :Error

  if exist test.ok (del /f /q test.ok)
  "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf16.fs
   IF ERRORLEVEL 1 goto :Error


REM  if exist test.ok (del /f /q test.ok)
REM  "%FSI%" %fsi_flags% --utf8output < kanji-unicode-utf16.fs
REM   IF ERRORLEVEL 1 goto :Error

call %~d0%~p0..\..\single-test-run.bat
exit /b %ErrorLEVEL%

:Error
endlocal
exit /b %ErrorLEVEL%

