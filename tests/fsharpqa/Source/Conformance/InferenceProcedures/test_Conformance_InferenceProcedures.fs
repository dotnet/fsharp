module ``FSharpQA-Tests-Conformance-InferenceProcedures``

open NUnit.Framework

open NUnitConf
open RunPlTest


module ByrefSafetyAnalysis =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/ByrefSafetyAnalysis")>]
    let ByrefSafetyAnalysis () = runpl |> check


module ConstraintSolving =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/ConstraintSolving")>]
    let ConstraintSolving () = runpl |> check


module DispatchSlotChecking =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/DispatchSlotChecking")>]
    let DispatchSlotChecking () = runpl |> check


module DispatchSlotInference =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/DispatchSlotInference")>]
    let DispatchSlotInference () = runpl |> check


module FunctionApplicationResolution =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/FunctionApplicationResolution")>]
    let FunctionApplicationResolution () = runpl |> check


module Generalization =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/Generalization")>]
    let Generalization () = runpl |> check


module MethodApplicationResolution =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/MethodApplicationResolution")>]
    let MethodApplicationResolution () = runpl |> check


module NameResolution =

    module AutoOpen =

        [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/NameResolution/AutoOpen")>]
        let AutoOpen () = runpl |> check

    module Misc =

        [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/NameResolution/Misc")>]
        let Misc () = runpl |> check

    module RequireQualifiedAccess =

        [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess")>]
        let RequireQualifiedAccess () = runpl |> check


module RecursiveSafetyAnalysis =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/RecursiveSafetyAnalysis")>]
    let RecursiveSafetyAnalysis () = runpl |> check


module ResolvingApplicationExpressions =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/ResolvingApplicationExpressions")>]
    let ResolvingApplicationExpressions () = runpl |> check


module TypeInference =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/TypeInference")>]
    let TypeInference () = runpl |> check


module WellFormednessChecking =

    [<Test; FSharpQASuiteTest("Conformance/InferenceProcedures/WellFormednessChecking")>]
    let WellFormednessChecking () = runpl |> check
