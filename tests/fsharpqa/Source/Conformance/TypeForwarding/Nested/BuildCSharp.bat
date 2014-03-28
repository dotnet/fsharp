@echo off
SET PARAM=%1
csc /t:library %PARAM%_Forwarder.cs
csc /define:FORWARD /t:library /r:%PARAM%_Forwarder.dll %PARAM%_Library.cs