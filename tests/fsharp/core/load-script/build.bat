if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)

call script > out.txt 2>&1

if NOT EXIST out.bsl COPY out.txt

%FSDIFF% out.txt out.bsl normalize > out.diff
%FSDIFF% z.output.fsi.help.txt z.output.fsi.help.bsl normalize > z.output.fsi.help.diff

echo ======== Differences From ========
TYPE  out.diff
echo ========= Differences To =========

for /f %%c IN (out.diff) do (
  echo NOTE -------------------------------------
  echo NOTE ---------- THERE ARE DIFFs ----------
  echo NOTE -------------------------------------
  echo .  
  echo To update baselines: "sd edit *bsl", "del *bsl", "build.bat" regenerates bsl, "sd diff ...", check what changed.  
  goto Error
)

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
rem Hardwire ERRORLEVEL to be 1, since routes in here from diff check do not have ERRORLEVEL set
endlocal
exit /b %ERRORLEVEL%
