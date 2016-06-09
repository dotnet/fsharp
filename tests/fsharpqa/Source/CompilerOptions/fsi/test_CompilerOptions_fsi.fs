module ``FSharpQA-Tests-CompilerOptions-fsi``

open NUnit.Framework

open NUnitConf
open RunPlTest


module arguments =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/arguments")>]
    let arguments () = runpl |> check


module exename =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/exename")>]
    let exename () = runpl |> check


module help =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/help")>]
    let help () = runpl |> check


module highentropyva =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/highentropyva")>]
    let highentropyva () = runpl |> check


module nologo =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/nologo")>]
    let nologo () = runpl |> check


module subsystemversion =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/subsystemversion")>]
    let subsystemversion () = runpl |> check


module times =

    [<Test; FSharpQASuiteTest("CompilerOptions/fsi/times")>]
    let times () = runpl |> check
