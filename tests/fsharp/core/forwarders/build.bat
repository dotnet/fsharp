@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  REM Skipping test for FSI.EXE
  goto Skip
)

REM **************************

REM only a valid test if generics supported

mkdir orig
mkdir split
%CSC% /nologo  /target:library /out:orig\a.dll /define:PART1;PART2 a.cs 
  @if ERRORLEVEL 1 goto Error
%CSC% /nologo  /target:library /out:orig\b.dll /r:orig\a.dll b.cs 
  @if ERRORLEVEL 1 goto Error
"%FSC%" -a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll c.fs
  @if ERRORLEVEL 1 goto Error
 
%CSC% /nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT a.cs  
  @if ERRORLEVEL 1 goto Error
%CSC% /nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT a.cs
  @if ERRORLEVEL 1 goto Error

copy /y orig\b.dll split\b.dll
copy /y orig\c.dll split\c.dll
 
"%FSC%" -o:orig\test.exe -r:orig\b.dll -r:orig\a.dll test.fs
  @if ERRORLEVEL 1 goto Error
"%FSC%" -o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll test.fs
  @if ERRORLEVEL 1 goto Error
"%FSC%" -o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll test.fs
  @if ERRORLEVEL 1 goto Error

pushd split
"%PEVERIFY%" a-part1.dll
  @if ERRORLEVEL 1 goto Error
REM "%PEVERIFY%" a.dll
REM   @if ERRORLEVEL 1 goto Error
"%PEVERIFY%" b.dll
  @if ERRORLEVEL 1 goto Error
"%PEVERIFY%" c.dll
  @if ERRORLEVEL 1 goto Error
"%PEVERIFY%" test.exe
  @if ERRORLEVEL 1 goto Error
"%PEVERIFY%" test-against-c.exe
  @if ERRORLEVEL 1 goto Error
popd



:Ok
echo Built fsharp %~n0 ok.
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

