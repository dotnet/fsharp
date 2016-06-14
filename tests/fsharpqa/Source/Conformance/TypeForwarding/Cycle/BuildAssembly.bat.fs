module ``Conformance-TypeForwarding-Cycle-BuildAssembly``

open NUnit.Framework

open NUnitConf
open PlatformHelpers
open System.IO



let run arg1 cwd (cfg: RunPl.RunPlConfig) = attempt {

    let exec p = Command.exec cwd cfg.envVars { Output = Inherit; Input = None} p >> checkResult
    let csc = Commands.csc exec cfg.CSC_PIPE

    // :001
    let label001 = attempt {
        // csc /define:BASIC001A /t:library /out:Cycle_Forward001a.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC001A /t:library /out:Cycle_Forward001a.dll" [ "Cycle_Forwarder.cs" ]

        // csc /define:BASIC001B /t:library /out:Cycle_Forward001b.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC001B /t:library /out:Cycle_Forward001b.dll" [ "Cycle_Forwarder.cs" ]

        // csc /define:FORWARD /t:library /r:Cycle_forward001a.dll /r:Cycle_forward001b.dll Cycle_Library.cs
        do! csc "/define:FORWARD /t:library /r:Cycle_forward001a.dll /r:Cycle_forward001b.dll" [ "Cycle_Library.cs" ]
        }

    // :002a
    let label002a = attempt {
        // csc /define:BASIC002A /t:library /out:Cycle_Forward002a.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC002A /t:library /out:Cycle_Forward002a.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:FORWARD /t:library /r:Cycle_Forward002a.dll Cycle_Library.cs
        do! csc "/define:FORWARD /t:library /r:Cycle_Forward002a.dll" [ "Cycle_Library.cs" ]
        // goto Exit
        }

    // :002b
    let label002b = attempt {
        // csc /define:BASIC002A /t:library /out:Cycle_Forward002a.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC002A /t:library /out:Cycle_Forward002a.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:FORWARD /t:library /r:Cycle_Forward002a.dll Cycle_Library.cs
        do! csc "/define:FORWARD /t:library /r:Cycle_Forward002a.dll" [ "Cycle_Library.cs" ]

        // csc /define:BASIC002B /t:library /out:Cycle_Forward002b.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC002B /t:library /out:Cycle_Forward002b.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:FORWARDFOO /t:library /r:Cycle_Forward002b.dll Cycle_Forwarder.cs
        do! csc "/define:FORWARDFOO /t:library /r:Cycle_Forward002b.dll" [ "Cycle_Forwarder.cs" ]
        // goto Exit
        }

    // :003
    let label003 = attempt {
        // csc /t:library /out:cycle_library003.dll cycle_library003.cs
        do! csc "/t:library /out:cycle_library003.dll" [ "cycle_library003.cs" ]
        // csc /define:BASIC003B /t:library /r:cycle_library003.dll /out:Cycle_Forward003b.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC003B /t:library /r:cycle_library003.dll /out:Cycle_Forward003b.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:BASIC003A /t:library /r:cycle_forward003b.dll /r:cycle_library003.dll /out:Cycle_Forward003a.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC003A /t:library /r:cycle_forward003b.dll /r:cycle_library003.dll /out:Cycle_Forward003a.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:FORWARD /t:library /r:cycle_forward003a.dll /r:cycle_forward003b.dll /out:cycle_library003.dll cycle_library003.cs
        do! csc "/define:FORWARD /t:library /r:cycle_forward003a.dll /r:cycle_forward003b.dll /out:cycle_library003.dll" [ "cycle_library003.cs" ]
        // goto Exit
        }

    // :004
    let label004 = attempt {
        // csc /define:BASIC004A /t:library /out:Cycle_Forward004a.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC004A /t:library /out:Cycle_Forward004a.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:BASIC004B /t:library /out:Cycle_Forward004b.dll Cycle_Forwarder.cs
        do! csc "/define:BASIC004B /t:library /out:Cycle_Forward004b.dll" [ "Cycle_Forwarder.cs" ]
        // csc /define:FORWARD /t:library /r:cycle_forward004a.dll Cycle_Library.cs
        do! csc "/define:FORWARD /t:library /r:cycle_forward004a.dll" [ "Cycle_Library.cs" ]
        // goto Exit
        }



    // @echo off
    ignore "useless"

    return! match arg1 with
            // if %1 == 001 goto 001
            | "001" -> label001
            // if %1 == 002a goto 002a
            | "002a" -> label002a
            // if %1 == 002b goto 002b
            | "002b" -> label002b
            // if %1 == 003 goto 003
            | "003" -> label003
            // if %1 == 004 goto 004
            | "004" -> label004
            // goto Exit
            //:Exit
            | a -> NUnitConf.genericError (sprintf "arg %s not implemented" a)
    }
