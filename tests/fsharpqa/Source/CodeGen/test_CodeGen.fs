module ``FSharpQA-Tests-CodeGen``

open NUnit.Framework

open NUnitConf
open RunPlTest

module StringEncoding =

    [<Test; FSharpQASuiteTest("CodeGen/StringEncoding")>]
    let StringEncoding () = runpl |> check

module Structure =

    [<Test; FSharpQASuiteTest("CodeGen/Structure")>]
    let Structure () = runpl |> check

