@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)

rem ===================================================
rem Test unsigned build
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=
set outfile=unsigned
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA1 key full signed  Command Line
rem ===================================================
set test_keyfile=sha1full.snk
set test_delaysign=
set extra_defines=
set test_outfile=sha1-full-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA1 key delayl signed  Command Line
rem ===================================================
set test_keyfile=sha1delay.snk
set test_delaysign=true
set extra_defines=
set test_outfile=sha1-delay-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 256 bit key fully signed  Command Line
rem ===================================================
set test_keyfile=sha256full.snk
set test_delaysign=
set extra_defines=
set test_outfile=sha256-full-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 256 bit key delay signed  Command Line
rem ===================================================
set test_keyfile=sha256delay.snk
set test_delaysign=true
set extra_defines=
set test_outfile=sha256-delay-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 512 bit key fully signed  Command Line
rem ===================================================
set test_keyfile=sha512full.snk
set test_delaysign=
set extra_defines=
set test_outfile=sha512-full-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 512 bit key delay signed  Command Line
rem ===================================================
set test_keyfile=sha512delay.snk
set test_delaysign=true
set extra_defines=
set test_outfile=sha512-delay-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 1024 bit key fully signed  Command Line
rem ===================================================
set test_keyfile=sha1024full.snk
set test_delaysign=
set extra_defines=
set test_outfile=sha1024-full-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 1024 bit key delay signed  Command Line
rem ===================================================
set test_keyfile=sha1024delay.snk
set test_delaysign=true
set extra_defines=
set test_outfile=sha1024-delay-cl
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error


rem ===================================================
rem Test SHA1 key full signed  Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA1
set test_outfile=sha1-full-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA1 key delayl signed  Attributes
rem ===================================================
set test_keyfile=sha1delay.snk
set test_delaysign=true
set extra_defines=--define:SHA1 --define:DELAY
set test_outfile=sha1-delay-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 256 bit key fully signed  Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA256
set test_outfile=sha256-full-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 256 bit key delay signed  Attributes
rem ===================================================
set test_keyfile=sha256delay.snk
set test_delaysign=
set extra_defines=--define:SHA256 --define:DELAY
set test_outfile=sha256-delay-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 512 bit key fully signed  Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA512
set test_outfile=sha512-full-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 512 bit key delay signed Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA512 --define:DELAY
set test_outfile=sha512-delay-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 1024 bit key fully signed  Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA1024
set test_outfile=sha1024-full-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

rem ===================================================
rem Test SHA 1024 bit key delay signed  Attributes
rem ===================================================
set test_keyfile=
set test_delaysign=
set extra_defines=--define:SHA1024 --define:DELAY
set test_outfile=sha1024-delay-attributes
call %~d0%~p0..\..\single-test-build.bat
@if ERRORLEVEL 1 goto Error

endlocal
exit /b 0

:Error
endlocal
exit /b %ERRORLEVEL%
