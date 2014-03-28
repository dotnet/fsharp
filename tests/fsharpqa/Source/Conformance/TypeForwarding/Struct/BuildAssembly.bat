@echo off
csc /t:library Struct_Forwarder.cs
csc /define:FORWARD /t:library /r:Struct_Forwarder.dll Struct_Library.cs