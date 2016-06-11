module ``FSharpQA-Tests-CompilerOptions-fsc``

open NUnit.Framework

open NUnitConf
open PlatformHelpers
open RunPlTest

module Removed =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/Removed")>]
    let Removed () = runpl |> check 

module Checked =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/checked")>]
    let Checked () = runpl |> check 

module cliversion =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/cliversion")>]
    let cliversion () = runpl |> check 

module codepage =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/codepage")>]
    let codepage () = runpl |> check 

module crossoptimize =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/crossoptimize")>]
    let crossoptimize () = runpl |> check 

module debug =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/debug")>]
    let debug () = runpl |> check 

module dumpAllCommandLineOptions =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/dumpAllCommandLineOptions")>]
    let dumpAllCommandLineOptions () = runpl |> check 

module flaterrors =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/flaterrors")>]
    let flaterrors () = runpl |> check 

module gccerrors =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/gccerrors")>]
    let gccerrors () = runpl |> check 

module help =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/help")>]
    let help () = runpl |> check 

module highentropyva =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/highentropyva")>]
    let highentropyva () = runpl |> check 

module invalid =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/invalid")>]
    let invalid () = runpl |> check 

module lib =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/lib")>]
    let lib () = runpl |> check 

module noframework =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/noframework")>]
    let noframework () = runpl |> check 

module nologo =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/nologo")>]
    let nologo () = runpl |> check 

module Optimize =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/optimize")>]
    let Optimize () = runpl |> check 

module out =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/out")>]
    let out () = runpl |> check 

module pdb =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/pdb")>]
    let pdb () = check(attempt {
        
        let ``IF EXIST {file}  DEL {file}`` fileName workDir envVars = attempt {
            let fileExists = Commands.fileExists workDir >> Option.isSome
            let del = Commands.rm workDir

            if fileExists fileName then del fileName
            }

        let ``IF     EXIST {file} EXIT 1`` fileName workDir envVars = attempt {
            let fileExists = Commands.fileExists workDir >> Option.isSome

            return! 
                if fileExists fileName 
                then NUnitConf.genericError (sprintf "Found not expected file '%s'" fileName)
                else Success
            }

        let ``IF NOT EXIST {file} EXIT 1`` fileName workDir envVars = attempt {
            let fileExists = Commands.fileExists workDir >> Option.isSome

            return!
                if not (fileExists fileName)
                then NUnitConf.genericError (sprintf "Not found expected file '%s'" fileName)
                else Success
            }

        do! [ "IF EXIST pdb01.pdb DEL pdb01.pdb",    ``IF EXIST {file}  DEL {file}`` "pdb01.pdb"
              "IF EXIST pdb01.pdb   DEL pdb01.pdb",  ``IF EXIST {file}  DEL {file}`` "pdb01.pdb"
              "IF EXIST pdb01x.pdb  DEL pdb01x.pdb", ``IF EXIST {file}  DEL {file}`` "pdb01x.pdb"
              "IF     EXIST pdb01x.pdb EXIT 1",      ``IF     EXIST {file} EXIT 1`` "pdb01x.pdb"
              "IF     EXIST pdb01.pdb EXIT 1",       ``IF     EXIST {file} EXIT 1`` "pdb01.pdb"
              "IF NOT EXIST pdb01.pdb EXIT 1",       ``IF NOT EXIST {file} EXIT 1`` "pdb01.pdb"
              "IF NOT EXIST pdb01x.pdb EXIT 1",      ``IF NOT EXIST {file} EXIT 1`` "pdb01x.pdb" ]
            |> Map.ofList
            |> runplWithCmds

        })
    

module platform =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/platform")>]
    let platform () = runpl |> check 

module reference =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/reference")>]
    let reference () = runpl |> check 

module responsefile =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/responsefile")>]
    let responsefile () = runpl |> check 

module simpleresolution =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/simpleresolution")>]
    let simpleresolution () = runpl |> check 

module standalone =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/standalone")>]
    let standalone () = runpl |> check 

module staticlink =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/staticlink")>]
    let staticlink () = runpl |> check 

module subsystemversion =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/subsystemversion")>]
    let subsystemversion () = runpl |> check 

module tailcalls =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/tailcalls")>]
    let tailcalls () = runpl |> check 

module target =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/target")>]
    let target () = runpl |> check 

module times =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/times")>]
    let times () = runpl |> check 

module warn =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/warn")>]
    let warn () = runpl |> check 

module warnaserror =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/warnaserror")>]
    let warnaserror () = runpl |> check 

module warnon =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/warnon")>]
    let warnon () = runpl |> check 

module win32res =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/win32res")>]
    let win32res () = runpl |> check 

