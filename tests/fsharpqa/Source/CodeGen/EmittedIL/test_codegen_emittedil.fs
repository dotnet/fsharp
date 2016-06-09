module ``FSharpQA-Tests-CodeGen-EmittedIL``

open NUnit.Framework

open NUnitConf
open RunPlTest


module AsyncExpressionStepping =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/AsyncExpressionStepping")>]
    let AsyncExpressionStepping () = runpl |> check

module AttributeTargets =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/AttributeTargets")>]
    let AttributeTargets () = runpl |> check

module CCtorDUWithMember =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/CCtorDUWithMember")>]
    let CCtorDUWithMember () = runpl |> check

module CompiledNameAttribute =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/CompiledNameAttribute")>]
    let CompiledNameAttribute () = runpl |> check

module ComputationExpressions =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/ComputationExpressions")>]
    let ComputationExpressions () = runpl |> check

module DoNotBoxStruct =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/DoNotBoxStruct")>]
    let DoNotBoxStruct () = runpl |> check

module GeneratedIterators =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/GeneratedIterators")>]
    let GeneratedIterators () = runpl |> check

module InequalityComparison =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/InequalityComparison")>]
    let InequalityComparison () = runpl |> check

module ListExpressionStepping =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/ListExpressionStepping")>]
    let ListExpressionStepping () = runpl |> check

module MethodImplAttribute =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/MethodImplAttribute")>]
    let MethodImplAttribute () = runpl |> check

module Misc =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/Misc")>]
    let Misc () = runpl |> check

module Mutation =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/Mutation")>]
    let Mutation () = runpl |> check

module Operators =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/Operators")>]
    let Operators () = runpl |> check

module QueryExpressionStepping =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/QueryExpressionStepping")>]
    let QueryExpressionStepping () = runpl |> check

module SeqExpressionStepping =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/SeqExpressionStepping")>]
    let SeqExpressionStepping () = runpl |> check

module SeqExpressionTailCalls =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/SeqExpressionTailCalls")>]
    let SeqExpressionTailCalls () = runpl |> check

module SerializableAttribute =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/SerializableAttribute")>]
    let SerializableAttribute () = runpl |> check

module StaticInit =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/StaticInit")>]
    let StaticInit () = runpl |> check

module SteppingMatch =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/SteppingMatch")>]
    let SteppingMatch () = runpl |> check



module TailCalls =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/TailCalls")>]
    let TailCalls () = runpl |> check

module TestFunctions =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/TestFunctions")>]
    let TestFunctions () = runpl |> check

module Tuples =

    [<Test; FSharpQASuiteTest("CodeGen/EmittedIL/Tuples")>]
    let Tuples () = runpl |> check

