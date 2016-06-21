module ``FSharpQA-Tests-Diagnostics``

open NUnit.Framework

open NUnitConf
open RunPlTest


module async =

    [<Test; FSharpQASuiteTest("Diagnostics/async")>]
    let async () = runpl |> check


module General =

    [<Test; FSharpQASuiteTest("Diagnostics/General")>]
    let General () = runpl |> check


module NONTERM =

    [<Test; FSharpQASuiteTest("Diagnostics/NONTERM")>]
    let NONTERM () = runpl |> check


module ParsingAtEOF =

    [<Test; FSharpQASuiteTest("Diagnostics/ParsingAtEOF")>]
    let ParsingAtEOF () = runpl |> check

