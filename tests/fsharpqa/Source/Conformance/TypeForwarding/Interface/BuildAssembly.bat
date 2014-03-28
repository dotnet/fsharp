@echo off
csc /t:library Interface_Forwarder.cs
csc /define:FORWARD /t:library /r:Interface_Forwarder.dll Interface_Library.cs