module ``FSharpQA-Tests-Conformance-Expressions``

open NUnit.Framework

open NUnitConf
open RunPlTest


module ApplicationExpressions =

    module Assertion =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/ApplicationExpressions/Assertion")>]
        let Assertion () = runpl |> check

    module BasicApplication =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/ApplicationExpressions/BasicApplication")>]
        let BasicApplication () = runpl |> check

    module ObjectConstruction =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/ApplicationExpressions/ObjectConstruction")>]
        let ObjectConstruction () = runpl |> check


module BindingExpressions =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/BindingExpressions")>]
    let BindingExpressions () = runpl |> check


module ConstantExpressions =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/ConstantExpressions")>]
    let ConstantExpressions () = runpl |> check


module ControlFlowExpressions =


    module Assertion =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/Assertion")>]
        let Assertion () = runpl |> check

    module Conditional =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/Conditional")>]
        let Conditional () = runpl |> check

    module ParenthesizedAndBlock =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/ParenthesizedAndBlock")>]
        let ParenthesizedAndBlock () = runpl |> check

    module PatternMatching =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/PatternMatching")>]
        let PatternMatching () = runpl |> check

    module SequenceIteration =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/SequenceIteration")>]
        let SequenceIteration () = runpl |> check

    module SequentialExecution =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/SequentialExecution")>]
        let SequentialExecution () = runpl |> check

    module SimpleFor =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/SimpleFor")>]
        let SimpleFor () = runpl |> check

    module TryCatch =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/TryCatch")>]
        let TryCatch () = runpl |> check

    module TryFinally =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/TryFinally")>]
        let TryFinally () = runpl |> check

    module While =
        [<Test; FSharpQASuiteTest("Conformance/Expressions/ControlFlowExpressions/While")>]
        let While () = runpl |> check


module DataExpressions =

    module AddressOf =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/AddressOf")>]
        let AddressOf () = runpl |> check

    module ComputationExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/ComputationExpressions")>]
        let ComputationExpressions () = runpl |> check

    module ObjectExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/ObjectExpressions")>]
        let ObjectExpressions () = runpl |> check

    module QueryExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/QueryExpressions")>]
        let QueryExpressions () = runpl |> check

    module RangeExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/RangeExpressions")>]
        let RangeExpressions () = runpl |> check

    module SequenceExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/SequenceExpressions")>]
        let SequenceExpressions () = runpl |> check

    module Simple =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/Simple")>]
        let Simple () = runpl |> check

    module TupleExpressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/DataExpressions/TupleExpressions")>]
        let TupleExpressions () = runpl |> check

module ElaborationAndElaboratedExpressions =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/ElaborationAndElaboratedExpressions")>]
    let ElaborationAndElaboratedExpressions () = runpl |> check


module EvaluationAndValues =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/EvaluationAndValues")>]
    let EvaluationAndValues () = runpl |> check


module EvaluationOfElaboratedForms =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/EvaluationOfElaboratedForms")>]
    let EvaluationOfElaboratedForms () = runpl |> check


module ExpressionQuotations =

    module Baselines =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/ExpressionQuotations/Baselines")>]
        let Baselines () = runpl |> check

    module Regressions =

        [<Test; FSharpQASuiteTest("Conformance/Expressions/ExpressionQuotations/Regressions")>]
        let Regressions () = runpl |> check


module SomeCheckingAndInferenceTerminology =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/SomeCheckingAndInferenceTerminology")>]
    let SomeCheckingAndInferenceTerminology () = runpl |> check


module SyntacticSugar =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/SyntacticSugar")>]
    let SyntacticSugar () = runpl |> check


module SyntacticSugarAndAmbiguities =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/SyntacticSugarAndAmbiguities")>]
    let SyntacticSugarAndAmbiguities () = runpl |> check


module TyperelatedExpressions =

    [<Test; FSharpQASuiteTest("Conformance/Expressions/Type-relatedExpressions")>]
    let TyperelatedExpressions () = runpl |> check
