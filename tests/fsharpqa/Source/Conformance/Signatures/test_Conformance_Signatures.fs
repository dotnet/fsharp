module ``FSharpQA-Tests-Conformance-Signatures``

open NUnit.Framework

open NUnitConf
open RunPlTest


module SignatureConformance =

    [<Test; FSharpQASuiteTest("Conformance/Signatures/SignatureConformance")>]
    let SignatureConformance () = runpl |> check


module SignatureTypes =

    [<Test; FSharpQASuiteTest("Conformance/Signatures/SignatureTypes")>]
    let SignatureTypes () = runpl |> check
