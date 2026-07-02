// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Optimizations

open Xunit
open FSharp.Test.Compiler

// Regression tests for https://github.com/dotnet/fsharp/issues/13099
// State machine lowering (BindResumableCodeDefinitions) dropped a `let this = receiver in ()`
// binding whose variable is unused, discarding the receiver's side effect. In Release the
// optimizer reduces a unit-typed member access (e.g. `.End`) to that shape, so the receiver
// exception was swallowed; Debug never produced the shape.
//
// Each test compiles a snippet under both Debug (optimize=false) and Release (optimize=true)
// and asserts the side effect is observed.
//
// Note: snippets execute *in-process* via reflection. They MUST NOT call `exit`
// because that would terminate the test runner. Use `failwith` (or simply let an
// expected exception propagate) to signal test failure.
module TaskCEUnitPropertyAccess =

    let private run (optimize: bool) (source: string) =
        Fsx source
        |> asExe
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

    [<InlineData(false)>]   // Debug
    [<InlineData(true)>]    // Release
    [<Theory>]
    let ``TaskCE_UnitPropertyAccess_PreservesReceiverSideEffects`` (optimize: bool) =
        run optimize """
type SomeOutputType() =
    member x.End = ()

let someFunctionWithReturnType () =
    failwith "This should be raised"
    SomeOutputType()

let theTaskAtHand () =
    task { (someFunctionWithReturnType ()).End }

try
    theTaskAtHand().Wait()
    failwith "Expected exception was not raised; the optimizer dropped the side-effectful receiver."
with
| :? System.AggregateException as ex when ex.InnerException.Message = "This should be raised" -> ()
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``UnitPropertyAccess_OnSideEffectfulReceiver_OutsideTaskCE_Raises`` (optimize: bool) =
        run optimize """
type SomeOutputType() =
    member x.End = ()

let someFunctionWithReturnType () =
    failwith "boom"
    SomeOutputType()

let test () = (someFunctionWithReturnType ()).End

try
    test ()
    failwith "Expected exception was not raised; the optimizer dropped the side-effectful receiver."
with
| ex when ex.Message = "boom" -> ()
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``TaskCE_UnitMethodCall_PreservesReceiverSideEffects`` (optimize: bool) =
        run optimize """
type SomeOutputType() =
    member x.Finish() = ()

let someFunctionWithReturnType () =
    failwith "boom"
    SomeOutputType()

let theTaskAtHand () =
    task { (someFunctionWithReturnType ()).Finish() }

try
    theTaskAtHand().Wait()
    failwith "Expected exception was not raised; the optimizer dropped the side-effectful receiver."
with
| :? System.AggregateException as ex when ex.InnerException.Message = "boom" -> ()
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``TaskCE_UnitPropertyAccess_RunsBothReceiverAndGetterEffects`` (optimize: bool) =
        run optimize """
let mutable counter = 0

type Tracker() =
    member x.Done = counter <- counter + 1

let makeTracker () =
    counter <- counter + 1
    Tracker()

let theTaskAtHand () =
    task { (makeTracker ()).Done }

theTaskAtHand().Wait()
if counter <> 2 then
    failwithf "Expected counter=2 (receiver + getter side effects) but got %d" counter
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``TaskCE_NonUnitPropertyAccess_OnSideEffectfulReceiver_Raises`` (optimize: bool) =
        run optimize """
type HasValue() =
    member x.Value = 42

let makeHasValue () =
    failwith "boom"
    HasValue()

let theTaskAtHand () =
    task { let _ = (makeHasValue ()).Value in () }

try
    theTaskAtHand().Wait()
    failwith "Expected exception was not raised."
with
| :? System.AggregateException as ex when ex.InnerException.Message = "boom" -> ()
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``AsyncCE_UnitPropertyAccess_PreservesReceiverSideEffects`` (optimize: bool) =
        run optimize """
type SomeOutputType() =
    member x.End = ()

let someFunctionWithReturnType () =
    failwith "boom"
    SomeOutputType()

let theAsyncAtHand () =
    async { (someFunctionWithReturnType ()).End }

try
    theAsyncAtHand () |> Async.RunSynchronously
    failwith "Expected exception was not raised."
with
| ex when ex.Message = "boom" -> ()
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``TaskCE_NestedUnitPropertyAccess_PreservesReceiverSideEffects`` (optimize: bool) =
        run optimize """
type Inner() =
    member x.End = ()

type Outer() =
    member x.Inner = Inner()

let makeOuter () =
    failwith "boom"
    Outer()

let theTaskAtHand () =
    task { (makeOuter ()).Inner.End }

try
    theTaskAtHand().Wait()
    failwith "Expected exception was not raised; the optimizer dropped the side-effectful receiver."
with
| :? System.AggregateException as ex when ex.InnerException.Message = "boom" -> ()
"""
