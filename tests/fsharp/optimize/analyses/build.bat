if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat
@if ERRORLEVEL 1 goto Error

if not "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)

rem recall  >fred.txt 2>&1 merges stderr into the stdout redirect
rem however 2>&1  >fred.txt did not seem to do it.

echo == FunctionSizes
"%FSC%" %fsc_flags% --nologo -O --test:FunctionSizes sizes.fs >sizes.FunctionSizes.output.test.txt 2>&1
echo == TotalSizes
"%FSC%" %fsc_flags% --nologo -O --test:TotalSizes sizes.fs >sizes.TotalSizes.output.test.txt 2>&1
echo == HasEffect
"%FSC%" %fsc_flags% --nologo -O --test:HasEffect effects.fs >effects.HasEffect.output.test.txt 2>&1
echo == NoNeedToTailcall
"%FSC%" %fsc_flags% --nologo -O --test:NoNeedToTailcall tailcalls.fs >tailcalls.NoNeedToTailcall.output.test.txt 2>&1


if NOT EXIST sizes.FunctionSizes.output.test.bsl COPY sizes.FunctionSizes.output.test.txt sizes.FunctionSizes.output.test.bsl
if NOT EXIST sizes.TotalSizes.output.test.bsl COPY sizes.TotalSizes.output.test.txt sizes.TotalSizes.output.test.bsl
if NOT EXIST effects.HasEffect.output.test.bsl COPY effects.HasEffect.output.test.txt effects.HasEffect.output.test.bsl
if NOT EXIST tailcalls.NoNeedToTailcall.output.test.bsl COPY tailcalls.NoNeedToTailcall.output.test.txt tailcalls.NoNeedToTailcall.output.test.bsl

%FSDIFF% sizes.FunctionSizes.output.test.txt sizes.FunctionSizes.output.test.bsl > sizes.FunctionSizes.output.test.diff
%FSDIFF% sizes.TotalSizes.output.test.txt sizes.TotalSizes.output.test.bsl > sizes.TotalSizes.output.test.diff
%FSDIFF% effects.HasEffect.output.test.txt effects.HasEffect.output.test.bsl > effects.HasEffect.output.test.diff
%FSDIFF% tailcalls.NoNeedToTailcall.output.test.txt tailcalls.NoNeedToTailcall.output.test.bsl > tailcalls.NoNeedToTailcall.output.test.diff

echo ======== Differences From ========
TYPE sizes.FunctionSizes.output.test.diff
TYPE sizes.TotalSizes.output.test.diff
TYPE effects.HasEffect.output.test.diff
TYPE tailcalls.NoNeedToTailcall.output.test.diff
echo ========= Differences To =========

TYPE sizes.FunctionSizes.output.test.diff > zz.alldiffs
TYPE sizes.TotalSizes.output.test.diff >> zz.alldiffs
TYPE effects.HasEffect.output.test.diff >> zz.alldiffs
TYPE tailcalls.NoNeedToTailcall.output.test.diff >> zz.alldiffs

for /f %%c IN (zz.alldiffs) do (
  echo NOTE -------------------------------------
  echo NOTE ---------- THERE ARE DIFFs ----------
  echo NOTE -------------------------------------
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
endlocal
exit /b %ERRORLEVEL%
