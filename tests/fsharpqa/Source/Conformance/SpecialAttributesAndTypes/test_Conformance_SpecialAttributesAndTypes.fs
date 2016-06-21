module ``FSharpQA-Tests-Conformance-SpecialAttributesAndTypes``

open NUnit.Framework

open NUnitConf
open RunPlTest

module Imported =

    [<Test; FSharpQASuiteTest("Conformance/SpecialAttributesAndTypes/Imported/System.ThreadStatic")>]
    let SystemThreadStatic () = runpl |> check
