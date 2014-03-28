@echo off
if %1 == 001 goto 001
if %1 == 002a goto 002a
if %1 == 002b goto 002b
if %1 == 003 goto 003
if %1 == 004 goto 004

goto Exit

:001
csc /define:BASIC001A /t:library /out:Cycle_Forward001a.dll Cycle_Forwarder.cs
csc /define:BASIC001B /t:library /out:Cycle_Forward001b.dll Cycle_Forwarder.cs
csc /define:FORWARD /t:library /r:Cycle_forward001a.dll /r:Cycle_forward001b.dll Cycle_Library.cs

:002a
csc /define:BASIC002A /t:library /out:Cycle_Forward002a.dll Cycle_Forwarder.cs
csc /define:FORWARD /t:library /r:Cycle_Forward002a.dll Cycle_Library.cs
goto Exit

:002b
csc /define:BASIC002A /t:library /out:Cycle_Forward002a.dll Cycle_Forwarder.cs
csc /define:FORWARD /t:library /r:Cycle_Forward002a.dll Cycle_Library.cs

csc /define:BASIC002B /t:library /out:Cycle_Forward002b.dll Cycle_Forwarder.cs
csc /define:FORWARDFOO /t:library /r:Cycle_Forward002b.dll Cycle_Forwarder.cs
goto Exit

:003
csc /t:library /out:cycle_library003.dll cycle_library003.cs
csc /define:BASIC003B /t:library /r:cycle_library003.dll /out:Cycle_Forward003b.dll Cycle_Forwarder.cs
csc /define:BASIC003A /t:library /r:cycle_forward003b.dll /r:cycle_library003.dll /out:Cycle_Forward003a.dll Cycle_Forwarder.cs
csc /define:FORWARD /t:library /r:cycle_forward003a.dll /r:cycle_forward003b.dll /out:cycle_library003.dll cycle_library003.cs
goto Exit

:004
csc /define:BASIC004A /t:library /out:Cycle_Forward004a.dll Cycle_Forwarder.cs
csc /define:BASIC004B /t:library /out:Cycle_Forward004b.dll Cycle_Forwarder.cs
csc /define:FORWARD /t:library /r:cycle_forward004a.dll Cycle_Library.cs
goto Exit


:Exit
