module ``FSharpQA-Tests-Conformance-BasicTypeAndModuleDefinitions``

open NUnit.Framework

open NUnitConf
open RunPlTest



module ExceptionDefinitions =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions")>]
    let ExceptionDefinitions () = runpl |> check


module GeneratedEqualityHashingComparison =

    module Attributes =
    
        module Diags =
            [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Diags")>]
            let Diags () = runpl |> check
    
        module Legacy =
            [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy")>]
            let Legacy () = runpl |> check
    
        module New =
            [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New")>]
            let New () = runpl |> check

    module Basic =
        [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic")>]
        let Basic () = runpl |> check

    module IComparison =
        [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison")>]
        let IComparison () = runpl |> check


module ModuleDefinitions =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/ModuleDefinitions")>]
    let ModuleDefinitions () = runpl |> check


module NamespaceDeclGroups =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/NamespaceDeclGroups")>]
    let NamespaceDeclGroups () = runpl |> check


module NullRepresentations =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/NullRepresentations")>]
    let NullRepresentations () = runpl |> check


module RecordTypes =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/RecordTypes")>]
    let RecordTypes () = runpl |> check



module TypeAbbreviations =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/TypeAbbreviations")>]
    let TypeAbbreviations () = runpl |> check


module UnionTypes =

    [<Test; FSharpQASuiteTest("Conformance/BasicTypeAndModuleDefinitions/UnionTypes")>]
    let UnionTypes () = runpl |> check
