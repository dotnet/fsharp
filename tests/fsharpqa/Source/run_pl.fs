module RunPlTest

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let runplWithCmds cmds = attempt {

    let { Directory = dir; Config = cfg } = FSharpTestSuite.testContext ()
    let! vars = FSharpQATestSuite.envLstData ()

    printfn "Directory: %s" dir
    printfn "Test env var:"
    vars |> List.iter (fun (k, v) -> printfn "%s=%s" k v)

    let allVars = 
        vars
        |> List.append (cfg.EnvironmentVariables |> Map.toList)
        |> List.append ["FSC_PIPE",   cfg.FSC]
        |> List.append ["FSI_PIPE",   cfg.FSI] //that should be fsiAnyCpu
        |> List.append ["FSI32_PIPE", cfg.FSI]
        |> List.append ["CSC_PIPE",   cfg.CSC]
        |> List.append ["REDUCED_RUNTIME", "1"] //the peverify it's not implemented
        |> Map.ofList

    printfn "All env var:"
    allVars |> Map.iter (printfn "%s=%s")

    do! RunPl.runpl dir allVars 

    }

let runpl = runplWithCmds Map.empty
