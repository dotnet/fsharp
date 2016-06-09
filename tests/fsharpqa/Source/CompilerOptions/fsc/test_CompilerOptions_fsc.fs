module ``FSharpQA-Tests-CompilerOptions-fsc``

open NUnit.Framework

open NUnitConf
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

//optimize


module out =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/out")>]
    let out () = runpl |> check 

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

module warnon =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsc/warnon")>]
    let warnon () = runpl |> check 

//win32res

