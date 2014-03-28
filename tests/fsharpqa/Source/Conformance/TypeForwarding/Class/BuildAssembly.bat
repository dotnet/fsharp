@echo off
csc /t:library Class_Forwarder.cs
csc /define:FORWARD /t:library /r:Class_Forwarder.dll Class_Library.cs
