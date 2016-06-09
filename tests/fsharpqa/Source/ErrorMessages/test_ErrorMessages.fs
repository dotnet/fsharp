module ``FSharpQA-Tests-ErrorMessages``

open NUnit.Framework

open NUnitConf
open RunPlTest

module NameResolution =

    [<Test; FSharpQASuiteTest("ErrorMessages/NameResolution")>]
    let NameResolution () = runpl |> check

