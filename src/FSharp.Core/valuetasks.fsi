// ValueTaskBuilder.fs - TPL value task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.FSharp.Control

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE && NETSTANDARD2_1
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Collections

/// <summary>
/// The extra data stored in ResumableStateMachine for valuetasks
/// </summary>
[<Struct; NoComparison; NoEquality>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
[<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                  1204,
                  IsHidden = true)>]
type ValueTaskStateMachineData<'T> =

    /// <summary>
    /// Holds the final result of the state machine
    /// </summary>
    [<DefaultValue(false)>]
    val mutable Result: 'T

    /// <summary>
    /// Holds the MethodBuilder for the state machine
    /// </summary>
    [<DefaultValue(false)>]
    val mutable MethodBuilder: AsyncValueTaskMethodBuilder<'T>

/// <summary>
/// This is used by the compiler as a template for creating state machine structs
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] ValueTaskStateMachine<'TOverall> =
    ResumableStateMachine<ValueTaskStateMachineData<'TOverall>>

/// <summary>
/// Represents the runtime continuation of a valuetask state machine created dynamically
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] ValueTaskResumptionFunc<'TOverall> =
    ResumptionFunc<ValueTaskStateMachineData<'TOverall>>

/// <summary>
/// A special compiler-recognised delegate type for specifying blocks of valuetask code
/// with access to the state machine.
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] ValueTaskCode<'TOverall, 'T> =
    ResumableCode<ValueTaskStateMachineData<'TOverall>, 'T>

/// <summary>
/// Contains methods to build valuetasks using the F# computation expression syntax
/// </summary>
[<Class>]
type ValueTaskBuilderBase =

    /// <summary>
    /// Specifies the sequential composition of two units of valuetask code.
    /// </summary>
    member inline Combine:
        task1: ValueTaskCode<'TOverall, unit> * task2: ValueTaskCode<'TOverall, 'T> -> ValueTaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies the delayed execution of a unit of valuetask code.
    /// </summary>
    member inline Delay: generator: (unit -> ValueTaskCode<'TOverall, 'T>) -> ValueTaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies the iterative execution of a unit of valuetask code.
    /// </summary>
    member inline For:
        sequence: seq<'T> * body: ('T -> ValueTaskCode<'TOverall, unit>) -> ValueTaskCode<'TOverall, unit>

    /// <summary>
    /// Specifies a unit of valuetask code which returns a value
    /// </summary>
    member inline Return: value: 'T -> ValueTaskCode<'T, 'T>

    /// <summary>
    /// Specifies a unit of valuetask code which excuted using try/finally semantics
    /// </summary>
    member inline TryFinally:
        body: ValueTaskCode<'TOverall, 'T> * [<InlineIfLambda>] compensation: (unit -> unit) ->
            ValueTaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies a unit of valuetask code which excuted using try/with semantics
    /// </summary>
    member inline TryWith:
        body: ValueTaskCode<'TOverall, 'T> * catch: (exn -> ValueTaskCode<'TOverall, 'T>) ->
            ValueTaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies a unit of valuetask code which binds to the resource implementing IAsyncDisposable and disposes it asynchronously
    /// </summary>
    member inline Using<'Resource, 'TOverall, 'T when 'Resource :> IAsyncDisposable> :
        resource: 'Resource * body: ('Resource -> ValueTaskCode<'TOverall, 'T>) -> ValueTaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies the iterative execution of a unit of valuetask code.
    /// </summary>
    member inline While:
        condition: (unit -> bool) * body: ValueTaskCode<'TOverall, unit> -> ValueTaskCode<'TOverall, unit>

    /// <summary>
    /// Specifies a unit of valuetask code which produces no result
    /// </summary>
    [<DefaultValue>]
    member inline Zero: unit -> ValueTaskCode<'TOverall, unit>

/// <summary>
/// Contains methods to build tasks using the F# computation expression syntax
/// </summary>
[<Class>]
type ValueTaskBuilder =
    inherit ValueTaskBuilderBase

    /// <summary>
    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve valuetasks or other reflective execution of F# code.
    /// </summary>
    static member RunDynamic: code: ValueTaskCode<'T, 'T> -> ValueTask<'T>

    /// Hosts the valuetask code in a state machine and starts the valuetask.
    member inline Run: code: ValueTaskCode<'T, 'T> -> ValueTask<'T>

/// Contains the `valuetask` computation expression builder.
[<AutoOpen>]
module ValueTaskBuilder =

    /// <summary>
    /// Builds a valuetask using computation expression syntax.
    /// </summary>
    ///
    /// <example-tbd></example-tbd>
    val valuetask: ValueTaskBuilder

// Contains the `valuetask` computation expression builder.
namespace Microsoft.FSharp.Control.ValueTaskBuilderExtensions

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

/// <summary>
/// Contains low-priority overloads for the `valuetask` computation expression builder.
/// </summary>
//
// Note: they are low priority because they are auto-opened first, and F# has a rule
// that extension method opened later in sequence get higher priority
//
// AutoOpen is by assembly attribute to get sequencing of AutoOpen correct and
// so each gives different priority
module LowPriority =

    type ValueTaskBuilderBase with

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from a task-like value
        /// satisfying the GetAwaiter pattern and calls a continuation.
        /// </summary>
        [<NoEagerConstraintApplication>]
        member inline Bind< ^TaskLike, 'TResult1, 'TResult2, ^Awaiter, 'TOverall> :
            task: ^TaskLike * continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                ValueTaskCode<'TOverall, 'TResult2>
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'TResult1)

        /// <summary>
        /// Specifies a unit of valuetask code which draws its result from a task-like value
        /// satisfying the GetAwaiter pattern.
        /// </summary>
        [<NoEagerConstraintApplication>]
        member inline ReturnFrom< ^TaskLike, ^Awaiter, 'T> :
            task: ^TaskLike -> ValueTaskCode<'T, 'T>
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'T)

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        /// </summary>
        [<NoEagerConstraintApplication>]
        static member inline BindDynamic< ^TaskLike, 'TResult1, 'TResult2, ^Awaiter, 'TOverall> :
            sm: byref<ValueTaskStateMachine<'TOverall>> *
            task: ^TaskLike *
            continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                bool
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'TResult1)

        /// <summary>
        /// Specifies a unit of valuetask code which binds to the resource implementing IDisposable and disposes it synchronously
        /// </summary>
        member inline Using:
            resource: 'Resource * body: ('Resource -> ValueTaskCode<'TOverall, 'T>) -> ValueTaskCode<'TOverall, 'T>
                when 'Resource :> IDisposable

/// <summary>
/// Contains medium-priority overloads for the `valuetask` computation expression builder.
/// </summary>
module MediumPriority =

    type ValueTaskBuilderBase with

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from an F# async value then calls a continuation.
        /// </summary>
        member inline Bind:
            computation: Async<'TResult1> * continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                ValueTaskCode<'TOverall, 'TResult2>

        /// <summary>
        /// Specifies a unit of task code which draws a result from an F# async value.
        /// </summary>
        member inline ReturnFrom: computation: Async<'T> -> ValueTaskCode<'T, 'T>

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from a task then calls a continuation.
        /// </summary>
        member inline Bind:
            task: Task<'TResult1> * continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                ValueTaskCode<'TOverall, 'TResult2>

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from a task.
        /// </summary>
        member inline ReturnFrom: task: Task<'T> -> ValueTaskCode<'T, 'T>

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        /// </summary>
        static member BindDynamic:
            sm: byref<ValueTaskStateMachine<'TOverall>> *
            task: Task<'TResult1> *
            continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                bool

/// <summary>
/// Contains high-priority overloads for the `valuetask` computation expression builder.
/// </summary>
module HighPriority =

    type ValueTaskBuilderBase with

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from a valuetask then calls a continuation.
        /// </summary>
        member inline Bind:
            task: ValueTask<'TResult1> * continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                ValueTaskCode<'TOverall, 'TResult2>

        /// <summary>
        /// Specifies a unit of valuetask code which draws a result from a valuetask.
        /// </summary>
        member inline ReturnFrom: task: ValueTask<'T> -> ValueTaskCode<'T, 'T>

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve valuetasks or other reflective execution of F# code.
        /// </summary>
        static member BindDynamic:
            sm: byref<ValueTaskStateMachine<'TOverall>> *
            task: ValueTask<'TResult1> *
            continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>) ->
                bool

#endif
