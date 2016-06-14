module ``FSharpQA-Tests-Conformance-TypeForwarding``

open NUnit.Framework

open NUnitConf
open PlatformHelpers
open RunPlTest



module Class =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Class")>]
    let Class () = check(attempt {
        let ``BuildAssembly.bat`` workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} p >> checkResult
            let csc = Commands.csc exec cfg.CSC_PIPE
           
            // @echo off
            ignore "useless"

            // csc /t:library Class_Forwarder.cs
            do! csc "/t:library" [ "Class_Forwarder.cs" ]
            // csc /define:FORWARD /t:library /r:Class_Forwarder.dll Class_Library.cs
            do! csc "/define:FORWARD /t:library /r:Class_Forwarder.dll" [ "Class_Library.cs" ]
            
            }

        let ``checkForward.bat`` exeToRun workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)

            // @echo off
            ignore "useless"

            // call %1
            return! match exec exeToRun "" with
                    // if errorlevel == 1 exit 1
                    | CmdResult.ErrorLevel (m, 1) -> NUnitConf.genericError (sprintf "Exit code was %d: %s" 1 m)
                    // if errorlevel == 0 exit -1
                    | CmdResult.Success -> NUnitConf.genericError (sprintf "Exit code was %d" 0)
                    // if errorlevel == -1 exit 0
                    | CmdResult.ErrorLevel (_, -1) -> succeed ()
                    // exit 1
                    | CmdResult.ErrorLevel (m, x) -> NUnitConf.genericError (sprintf "Exit code was %d: '%s'" x m)
            }

        let ``CheckRuntimeException.bat`` p1 p2 p3 workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)
            
            return! 
                // is32bitruntime.exe
                match exec "is32bitruntime.exe" "" with
                // IF ERRORLEVEL 1 (recomp /p:%1 /ee:%3) 
                | CmdResult.ErrorLevel(m,i) -> attempt {
                    printfn "Exit code %i: %s" i m
                    do! exec "recomp" (sprintf "/p:%s /ee:%s" p1 p3) |> checkResult
                    }
                // ELSE (recomp /p:%1 /ee:%2)
                | CmdResult.Success -> attempt {
                    printfn "Exit code 0"
                    do! exec "recomp" (sprintf "/p:%s /ee:%s" p1 p2) |> checkResult
                    }
            }


        let cmds cmd = 
            match cmd with
            | "BuildAssembly.bat" -> Some ``BuildAssembly.bat``
            | StartsWith "checkForward.bat " exeName -> Some (``checkForward.bat`` exeName)
            | StartsWith "CheckRuntimeException.bat " args ->
                let orEmpty = function None -> "" | Some s -> s
                let args = args.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
                let p1 = args |> List.tryItem 0 |> orEmpty
                let p2 = args |> List.tryItem 1 |> orEmpty
                let p3 = args |> List.tryItem 2 |> orEmpty
                Some (``CheckRuntimeException.bat`` p1 p2 p3)
            | _ -> None

        do! runplWithCmdsOverride cmds

        })


module Cycle =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Cycle")>]
    let Cycle () = check(attempt { 

        let ``checkForward.bat`` exeToRun anotherExitCodeOk workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)

            // @echo off
            ignore "useless"

            // call %1
            return! match exec exeToRun "" with
                    // if errorlevel == 1 exit 1
                    | CmdResult.ErrorLevel (m, 1) -> NUnitConf.genericError (sprintf "Exit code was %d: %s" 1 m)
                    // if errorlevel == 0 exit -1
                    | CmdResult.Success -> NUnitConf.genericError (sprintf "Exit code was %d" 0)
                    // if errorlevel == -1 exit 0
                    | CmdResult.ErrorLevel (_, -1) -> succeed ()
                    // if errorlevel == %2 exit 0
                    | CmdResult.ErrorLevel (_, n) when (Some (n.ToString())) = anotherExitCodeOk -> succeed ()
                    // exit 1
                    | CmdResult.ErrorLevel (m, x) -> NUnitConf.genericError (sprintf "Exit code was %d: '%s'" x m)
            }


        let cmds cmd = 
            match cmd with
            | StartsWith "BuildAssembly.bat " code ->
                Some (``Conformance-TypeForwarding-Cycle-BuildAssembly``.run code)
            | StartsWith "checkForward.bat " exeAndArgs ->
                let exe, arg1 = exeAndArgs |> splitAtFirst ((=) ' ')
                Some (``checkForward.bat`` exe arg1)
            | _ -> None

        do! runplWithCmdsOverride cmds

        })


module Delegate =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Delegate")>]
    let Delegate () = check(attempt {
        let ``BuildAssembly.bat`` workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} p >> checkResult
            let csc = Commands.csc exec cfg.CSC_PIPE
            
            // @echo off
            ignore "useless"

            // csc /t:library Delegate_Forwarder.cs
            do! csc "/t:library" [ "Delegate_Forwarder.cs" ]
            // csc /define:FORWARD /t:library /r:Delegate_Forwarder.dll Delegate_Library.cs
            do! csc "/define:FORWARD /t:library /r:Delegate_Forwarder.dll" [ "Delegate_Library.cs" ]
            }

        let cmds cmd = 
            match cmd with
            | "BuildAssembly.bat" -> Some ``BuildAssembly.bat``
            | _ -> None

        do! runplWithCmdsOverride cmds

        })


module Interface =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Interface")>]
    let Interface () = check(attempt {
        let ``BuildAssembly.bat`` workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} p >> checkResult
            let csc = Commands.csc exec cfg.CSC_PIPE
            
            // @echo off
            ignore "useless"

            // csc /t:library Interface_Forwarder.cs
            do! csc "/t:library" [ "Interface_Forwarder.cs" ]
            // csc /define:FORWARD /t:library /r:Interface_Forwarder.dll Interface_Library.cs
            do! csc "/define:FORWARD /t:library /r:Interface_Forwarder.dll" [ "Interface_Library.cs" ]
            }

        let cmds cmd = 
            match cmd with
            | "BuildAssembly.bat" -> Some ``BuildAssembly.bat``
            | _ -> None

        do! runplWithCmdsOverride cmds

        })


module Nested =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Nested")>]
    let Nested () = check(attempt {
        let ``BuildCSharp.bat`` param workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} p >> checkResult
            let csc = Commands.csc exec cfg.CSC_PIPE

            // @echo off
            ignore "useless"

            // SET PARAM=%1
            ignore "from arguments"

            // csc /t:library %PARAM%_Forwarder.cs
            do! csc "/t:library" [ sprintf "%s_Forwarder.cs" param ]

            // csc /define:FORWARD /t:library /r:%PARAM%_Forwarder.dll %PARAM%_Library.cs
            do! csc (sprintf "/define:FORWARD /t:library /r:%s_Forwarder.dll" param) [ sprintf "%s_Library.cs" param ]
            
            }

        let ``checkForward.bat`` exeToRun workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)

            // @echo off
            ignore "useless"

            // call %1
            return! match exec exeToRun "" with
                    // if errorlevel == 1 exit 1
                    | CmdResult.ErrorLevel (m, 1) -> NUnitConf.genericError (sprintf "Exit code was %d: %s" 1 m)
                    // if errorlevel == 0 exit -1
                    | CmdResult.Success -> NUnitConf.genericError (sprintf "Exit code was %d" 0)
                    // if errorlevel == -1 exit 0
                    | CmdResult.ErrorLevel (_, -1) -> succeed ()
                    // exit 1
                    | CmdResult.ErrorLevel (m, x) -> NUnitConf.genericError (sprintf "Exit code was %d: '%s'" x m)
            }

        let cmds cmd = 
            match cmd with
            | "BuildCSharp.bat Nested" -> Some (``BuildCSharp.bat`` "Nested")
            | StartsWith "checkForward.bat " exeName ->
                Some (``checkForward.bat`` exeName)
            | _ -> None

        do! runplWithCmdsOverride cmds

        })


module Struct =

    [<Test; FSharpQASuiteTest("Conformance/TypeForwarding/Struct")>]
    let Struct () = check(attempt {
        let ``BuildAssembly.bat`` workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} p >> checkResult
            let csc = Commands.csc exec cfg.CSC_PIPE
            
            // @echo off
            ignore "useless"

            // csc /t:library Struct_Forwarder.cs
            do! csc "/t:library" [ "Struct_Forwarder.cs" ]

            // csc /define:FORWARD /t:library /r:Struct_Forwarder.dll Struct_Library.cs
            do! csc "/define:FORWARD /t:library /r:Struct_Forwarder.dll" [ "Struct_Library.cs" ]

            }

        let ``checkForward.bat`` exeToRun workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)

            // @echo off
            ignore "useless"

            // call %1
            return! match exec exeToRun "" with
                    // if errorlevel == 1 exit 1
                    | CmdResult.ErrorLevel (m, 1) -> NUnitConf.genericError (sprintf "Exit code was %d: %s" 1 m)
                    // if errorlevel == 0 exit -1
                    | CmdResult.Success -> NUnitConf.genericError (sprintf "Exit code was %d" 0)
                    // if errorlevel == -1 exit 0
                    | CmdResult.ErrorLevel (_, -1) -> succeed ()
                    // exit 1
                    | CmdResult.ErrorLevel (m, x) -> NUnitConf.genericError (sprintf "Exit code was %d: '%s'" x m)
            }

        let ``CheckRuntimeException.bat`` p1 p2 p3 workDir (cfg: RunPl.RunPlConfig) = attempt {
            let exec p = Command.exec workDir cfg.envVars { Output = Inherit; Input = None} (p |> Commands.getfullpath workDir)
            
            return! 
                // is32bitruntime.exe
                match exec "is32bitruntime.exe" "" with
                // IF ERRORLEVEL 1 (recomp /p:%1 /ee:%3) 
                | CmdResult.ErrorLevel(m,i) -> attempt {
                    printfn "Exit code %i: %s" i m
                    do! exec "recomp" (sprintf "/p:%s /ee:%s" p1 p3) |> checkResult
                    }
                // ELSE (recomp /p:%1 /ee:%2)
                | CmdResult.Success -> attempt {
                    printfn "Exit code 0"
                    do! exec "recomp" (sprintf "/p:%s /ee:%s" p1 p2) |> checkResult
                    }
            }


        let cmds cmd = 
            match cmd with
            | "BuildAssembly.bat" -> Some ``BuildAssembly.bat``
            | StartsWith "checkForward.bat " exeName -> Some (``checkForward.bat`` exeName)
            | StartsWith "CheckRuntimeException.bat " args ->
                let orEmpty = function None -> "" | Some s -> s
                let args = args.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
                let p1 = args |> List.tryItem 0 |> orEmpty
                let p2 = args |> List.tryItem 1 |> orEmpty
                let p3 = args |> List.tryItem 2 |> orEmpty
                Some (``CheckRuntimeException.bat`` p1 p2 p3)
            | _ -> None

        do! runplWithCmdsOverride cmds

        })

