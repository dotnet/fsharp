@if "%_echo%"=="" echo off

setlocal

REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


rem fsc.exe building

    %CSC% /nologo  /target:library /out:cslib.dll cslib.cs
    @if ERRORLEVEL 1 goto Error

    "%FSC%" %fsc_flags% -o:test.exe -r cslib.dll -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test.exe 
    @if ERRORLEVEL 1 goto Error

   "%FSC%" %fsc_flags% -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test-with-debug-data.exe 
    @if ERRORLEVEL 1 goto Error


    "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -r cslib.dll -g test.fsx
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test--optimize.exe 
    @if ERRORLEVEL 1 goto Error

    rmdir /s /q test--downtarget
    mkdir test--downtarget

	dir "%FSCOREDLLVPREVPATH%"
	dir "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\"
	REM Compile with FSharp.Core 4.3.1.0.  Add the FSHARP_CORE_31 and Portable defines.
	"%FSC%" %fsc_flags% -o:test--downtarget\test--downtarget.exe --noframework -r "%FSCOREDLLVPREVPATH%" -r "%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" -r System.dll -r System.Runtime.dll -r System.Xml.dll -r System.Data.dll -r System.Web.dll -r System.Core.dll -r System.Numerics.dll -r cslib.dll -g test.fsx --define:FSHARP_CORE_31 --define:Portable
    @if ERRORLEVEL 1 goto Error

	copy /y "%FSCOREDLLVPREVPATH%" test--downtarget\FSharp.Core.dll
    @if ERRORLEVEL 1 goto Error

    "%PEVERIFY%" test--downtarget\test--downtarget.exe 
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

