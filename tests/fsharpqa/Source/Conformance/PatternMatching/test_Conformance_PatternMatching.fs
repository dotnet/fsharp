module ``FSharpQA-Tests-Conformance-PatternMatching``

open NUnit.Framework

open NUnitConf
open RunPlTest

module And =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/And")>]
    let And () = runpl |> check


module Array =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Array")>]
    let Array () = runpl |> check


module As =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/As")>]
    let As () = runpl |> check


module ConsList =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/ConsList")>]
    let ConsList () = runpl |> check


module DynamicTypeTest =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/DynamicTypeTest")>]
    let DynamicTypeTest () = runpl |> check


module Expression =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Expression")>]
    let Expression () = runpl |> check


module Named =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Named")>]
    let Named () = runpl |> check


module Null =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Null")>]
    let Null () = runpl |> check


module Record =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Record")>]
    let Record () = runpl |> check


module Simple =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Simple")>]
    let Simple () = runpl |> check


module SimpleConstant =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/SimpleConstant")>]
    let SimpleConstant () = runpl |> check


module Tuple =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Tuple")>]
    let Tuple () = runpl |> check


module TypeAnnotated =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/TypeAnnotated")>]
    let TypeAnnotated () = runpl |> check


module TypeConstraint =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/TypeConstraint")>]
    let TypeConstraint () = runpl |> check


module Union =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Union")>]
    let Union () = runpl |> check


module Wildcard =

    [<Test; FSharpQASuiteTest("Conformance/PatternMatching/Wildcard")>]
    let Wildcard () = runpl |> check
