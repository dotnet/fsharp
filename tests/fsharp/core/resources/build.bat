@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


  REM Note that you have a VS SDK dependence here.
  "%RESGEN%" /compile Resources.resx
  @if ERRORLEVEL 1 goto Error

  "%FSC%" %fsc_flags%  --resource:Resources.resources -o:test-embed.exe -g test.fs      
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" test-embed.exe 
  @if ERRORLEVEL 1 goto Error

  "%FSC%" %fsc_flags%  --linkresource:Resources.resources -o:test-link.exe -g test.fs      
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" test-link.exe
  @if ERRORLEVEL 1 goto Error

  "%FSC%" %fsc_flags%  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g test.fs      
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" test-embed-named.exe
  @if ERRORLEVEL 1 goto Error

  "%FSC%" %fsc_flags%  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g test.fs      
  @if ERRORLEVEL 1 goto Error

  "%PEVERIFY%" test-link-named.exe
  @if ERRORLEVEL 1 goto Error





:Ok
echo Built fsharp %~f0 ok.
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

