module ``FSharp-Tests-Optimize``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let runpl = attempt {

    let { Directory = dir; Config = cfg } = FSharpTestSuite.testContext ()
    let! vars = FSharpQATestSuite.envLstData ()

    printfn "Directory: %s" dir
    printfn "%A" vars

    let allVars = 
        vars
        |> List.append (cfg.EnvironmentVariables |> Map.toList)
        |> List.append ["FSC_PIPE",   cfg.FSC]
        |> List.append ["FSI_PIPE",   cfg.FSI] //that should be fsiAnyCpu
        |> List.append ["FSI32_PIPE", cfg.FSI]
        |> List.append ["CSC_PIPE",   cfg.CSC]
        |> List.append ["REDUCED_RUNTIME", "1"] //the peverify it's not implemented
        |> Map.ofList

    do! RunPl.runpl dir allVars 

    }


module Simple = 

    [<Test; FSharpQASuiteTest("Simple")>]
    let ``Simple`` () =
        runpl |> check 

module Libraries_Core_Collections =

    [<Test; FSharpQASuiteTest("Libraries/Core/Collections")>]
    let ``Libraries/Core/Collections`` () =
        runpl |> check 


module CompilerOptions_fsc_Removed =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/Removed")>]
    let Removed () =
        runpl |> check 

module CompilerOptions_fsc_checked =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/checked")>]
    let ``checked`` () =
        runpl |> check 

module CompilerOptions_fsc_cliversion =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/cliversion")>]
    let cliversion () =
        runpl |> check 

module CompilerOptions_fsc_codepage =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/codepage")>]
    let codepage () =
        runpl |> check 

module CompilerOptions_fsc_crossoptimize =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/crossoptimize")>]
    let crossoptimize () =
        runpl |> check 


module CompilerOptions_fsc_debug =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/debug")>]
    let debug () =
        runpl |> check 

module CompilerOptions_fsc_dumpAllCommandLineOptions =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/dumpAllCommandLineOptions")>]
    let dumpAllCommandLineOptions () =
        runpl |> check 

module CompilerOptions_fsc_flaterrors =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/flaterrors")>]
    let flaterrors () =
        runpl |> check 

module CompilerOptions_fsc_gccerrors =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/gccerrors")>]
    let gccerrors () =
        runpl |> check 

module CompilerOptions_fsc_help =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/help")>]
    let help () =
        runpl |> check 

module CompilerOptions_fsc_highentropyva =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/highentropyva")>]
    let highentropyva () =
        runpl |> check 

module CompilerOptions_fsc_invalid =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/invalid")>]
    let invalid () =
        runpl |> check 

module CompilerOptions_fsc_lib =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/lib")>]
    let lib () =
        runpl |> check 

module CompilerOptions_fsc_noframework =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/noframework")>]
    let noframework () =
        runpl |> check 

module CompilerOptions_fsc_nologo =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/nologo")>]
    let nologo () =
        runpl |> check 

//optimize


module CompilerOptions_fsc_out =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/out")>]
    let ``CompilerOptions/fsc/out`` () =
        runpl |> check 

//pdb
//platform
//reference
//simpleresolution
//standalone
//staticlink
//subsystemversion
//tailcalls
//target
//times
//warn
//warnaserror

module CompilerOptions_fsc_warnon =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/warnon")>]
    let ``CompilerOptions/fsc/warnon`` () =
        runpl |> check 

//win32res


