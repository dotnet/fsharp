// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Optimizations

open Xunit
open FSharp.Test.Compiler

// Regression tests for https://github.com/dotnet/fsharp/issues/13099
// State machine lowering dropped an unused `let this = receiver in ()` binding, discarding the
// receiver's side effect. Each test compiles under Debug and Release and asserts it is observed.
// Snippets run in-process via reflection, so use `failwith` (not `exit`) to signal failure.
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
    let ``TaskCE_StructProjectionUnitAccess_PreservesReceiverSideEffects`` (optimize: bool) =
        run optimize """
#nowarn "52"
type Inner() =
    member _.End = ()

[<Struct; NoComparison; NoEquality>]
type Outer =
    val Inner: Inner
    new(i) = { Inner = i }
    member x.GetInner = x.Inner

let makeOuter () =
    failwith "boom"
    Outer(Inner())

let theTaskAtHand () =
    task { (makeOuter()).GetInner.End }

try
    theTaskAtHand().Wait()
    failwith "Expected exception was not raised; the receiver side effect was dropped."
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
