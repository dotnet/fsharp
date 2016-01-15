@if "%_echo%"=="" echo off

setlocal
REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
if EXIST build.ok DEL /f /q build.ok

call %~d0%~p0..\..\..\config.bat

if NOT "%FSC:NOTAVAIL=X%" == "%FSC%" ( 
  ECHO Skipping test for FSI.EXE
  goto Skip
)


rem  SET FSC=C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\coreclr C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\fsc.exe --targetprofile:netcore --noframework --simpleresolution --target:library -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\mscorlib.ni.dll

rem    REM unsigned build of F# app 
rem    "%FSC%" %fsc_flags% -o:test-unsigned.exe -g test.fs      
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM =========================================================================================
rem    REM test by signing with command line arguments
rem    REM =========================================================================================
rem    
rem    REM delay signed build of F# App with SHA1
rem    "%FSC%" %fsc_flags% --delaysign --keyfile:sha1delay.snk -o:test-cl-delay-sha1.exe test.fs
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM delay signed build of F# App with SHA256
rem    "%FSC%" %fsc_flags% --delaysign --keyfile:sha256delay.snk -o:test-cl-delay-sha256.exe test.fs
rem    @if ERRORLEVEL 1 goto Error

REM full signed build of F# App with SHA1
rem    "%FSC%" %fsc_flags% --keyfile:sha1full.snk -o:test-cl-sha1.exe test.fs
rem    @if ERRORLEVEL 1 goto Error

rem C:\KevinRansom\visualfsharp\release\net40\bin\fsc.exe --target:library --keyfile:sha1full.snk -o:test-cl-sha1.dll test.fs
rem @if ERRORLEVEL 1 goto Error

C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\corerun.exe C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\fsc.exe --targetprofile:netcore --noframework --simpleresolution --target:library -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\mscorlib.ni.dll -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\system.runtime.dll --keyfile:sha1full.snk -o:test-cl-sha1.dll test.fs
@if ERRORLEVEL 1 goto Error

C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\corerun.exe C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\fsc.exe --targetprofile:netcore --noframework --simpleresolution --target:library -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\mscorlib.ni.dll -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\system.runtime.dll --keyfile:sha256full.snk -o:test-cl-sha256.dll test.fs
@if ERRORLEVEL 1 goto Error

C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\corerun.exe C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\fsc.exe --targetprofile:netcore --noframework --simpleresolution --target:library -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\mscorlib.ni.dll -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\system.runtime.dll --keyfile:sha512full.snk -o:test-cl-sha512.dll test.fs
@if ERRORLEVEL 1 goto Error

C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\corerun.exe C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\fsc.exe --targetprofile:netcore --noframework --simpleresolution --target:library -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\mscorlib.ni.dll -r:C:\KevinRansom\visualfsharp\tests\testbin\release\coreclr\fsc\win7-x86\system.runtime.dll --keyfile:sha1024full.snk -o:test-cl-sha1024.dll test.fs
@if ERRORLEVEL 1 goto Error

rem    REM full signed build of F# App with SHA256
rem    "%FSC%" %fsc_flags% --keyfile:sha256full.snk -o:test-cl-sha256.exe test.fs
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM =========================================================================================
rem    REM test by signing with Attributes
rem    REM =========================================================================================
rem    
rem    REM delay signed build of F# App with SHA1
rem    "%FSC%" %fsc_flags% --define:DELAYSIGN --define:SHA1 -o:test-attributes-delay-sha1.exe test.fs
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM delay signed build of F# App with SHA256
rem    "%FSC%" %fsc_flags% --define:DELAYSIGN --define:SHA256 -o:test-attributes-delay-sha256.exe test.fs
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM full signed build of F# App with SHA1
rem    "%FSC%" %fsc_flags% --define:SHA1 -o:test-attributes-sha1.exe test.fs
rem    @if ERRORLEVEL 1 goto Error
rem    
rem    REM full signed build of F# App with SHA256
rem    "%FSC%" %fsc_flags% --define:SHA256 -o:test-attributes-sha256.exe test.fs
rem    @if ERRORLEVEL 1 goto Error

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
