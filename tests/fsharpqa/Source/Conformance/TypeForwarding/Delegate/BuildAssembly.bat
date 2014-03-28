@echo off
csc /t:library Delegate_Forwarder.cs
csc /define:FORWARD /t:library /r:Delegate_Forwarder.dll Delegate_Library.cs
