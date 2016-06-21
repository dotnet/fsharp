module ``FSharpQA-Tests-Libraries``

open NUnit.Framework

open NUnitConf
open RunPlTest

module Control =

    [<Test; FSharpQASuiteTest("Libraries/Control")>]
    let Control () = runpl |> check

module Portable =

    [<Test; FSharpQASuiteTest("Libraries/Portable")>]
    let Portable () = runpl |> check

