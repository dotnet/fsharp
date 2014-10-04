@if "%_echo%"=="" echo off

call %~d0%~p0..\..\..\config.bat

%CSC% /target:library /out:HighRankArrayTests.dll .\Class1.cs

call %~d0%~p0..\..\single-test-build.bat

exit /b %ERRORLEVEL%
