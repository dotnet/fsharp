module ``FSharpQA-Tests-Conformance-BasicGrammarElements``

open NUnit.Framework

open NUnitConf
open RunPlTest


module Constants =

    [<Test; FSharpQASuiteTest("Conformance/BasicGrammarElements/Constants")>]
    let Constants () = runpl |> check


module OperatorNames =

    [<Test; FSharpQASuiteTest("Conformance/BasicGrammarElements/OperatorNames")>]
    let OperatorNames () = runpl |> check


module PrecedenceAndOperators =

    [<Test; FSharpQASuiteTest("Conformance/BasicGrammarElements/PrecedenceAndOperators")>]
    let PrecedenceAndOperators () = runpl |> check
