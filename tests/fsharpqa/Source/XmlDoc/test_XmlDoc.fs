module ``FSharpQA-Tests-XmlDoc``

open NUnit.Framework

open NUnitConf
open RunPlTest



module Basic =

    [<Test; FSharpQASuiteTest("XmlDoc/Basic")>]
    let Basic () = runpl |> check


module OCamlDoc =

    [<Test; FSharpQASuiteTest("XmlDoc/OCamlDoc")>]
    let OCamlDoc () = runpl |> check



module UnitOfMeasure =

    [<Test; FSharpQASuiteTest("XmlDoc/UnitOfMeasure")>]
    let UnitOfMeasure () = runpl |> check
