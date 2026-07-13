// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Collections

/// <summary>
/// The extra data stored in ResumableStateMachine for tasks
/// </summary>
[<Struct; NoComparison; NoEquality>]
[<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                  1204,
                  IsHidden = true)>]
type TaskStateMachineData<'T> =

    /// <summary>
    /// Holds the final result of the state machine
    /// </summary>
    [<DefaultValue(false)>]
    val mutable Result: 'T

    /// <summary>
    /// Holds the MethodBuilder for the state machine
    /// </summary>
    [<DefaultValue(false)>]
    val mutable MethodBuilder: AsyncTaskMethodBuilder<'T>

/// <summary>
/// This is used by the compiler as a template for creating state machine structs
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] TaskStateMachine<'TOverall> =
    ResumableStateMachine<TaskStateMachineData<'TOverall>>

/// <summary>
/// Represents the runtime continuation of a task state machine created dynamically
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] TaskResumptionFunc<'TOverall> = ResumptionFunc<TaskStateMachineData<'TOverall>>

/// <summary>
/// A special compiler-recognised delegate type for specifying blocks of task code
/// with access to the state machine.
/// </summary>
and [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly",
                      1204,
                      IsHidden = true)>] TaskCode<'TOverall, 'T> = ResumableCode<TaskStateMachineData<'TOverall>, 'T>

/// <summary>
/// Contains methods to build tasks using the F# computation expression syntax
/// </summary>
[<Class>]
type TaskBuilderBase =

    /// <summary>
    /// Specifies the sequential composition of two units of task code.
    /// </summary>
    member inline Combine: task1: TaskCode<'TOverall, unit> * task2: TaskCode<'TOverall, 'T> -> TaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies the delayed execution of a unit of task code.
    /// </summary>
    member inline Delay: generator: (unit -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies the iterative execution of a unit of task code.
    /// </summary>
    member inline For: sequence: seq<'T> * body: ('T -> TaskCode<'TOverall, unit>) -> TaskCode<'TOverall, unit>

    /// <summary>
    /// Specifies a unit of task code which returns a value
    /// </summary>
    member inline Return: value: 'T -> TaskCode<'T, 'T>

    /// <summary>
    /// Specifies a unit of task code which executed using try/finally semantics
    /// </summary>
    member inline TryFinally:
        body: TaskCode<'TOverall, 'T> * [<InlineIfLambda>] compensation: (unit -> unit) -> TaskCode<'TOverall, 'T>

    /// <summary>
    /// Specifies a unit of task code which executed using try/with semantics
    /// </summary>
    member inline TryWith:
        body: TaskCode<'TOverall, 'T> * catch: (exn -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>

#if NETSTANDARD2_1
    /// <summary>
    /// Specifies a unit of task code which binds to the resource implementing IAsyncDisposable and disposes it asynchronously
    /// </summary>
    member inline Using<'Resource, 'TOverall, 'T when 'Resource :> IAsyncDisposable> :
        resource: 'Resource * body: ('Resource -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
#endif

    /// <summary>
    /// Specifies the iterative execution of a unit of task code.
    /// </summary>
    member inline While: condition: (unit -> bool) * body: TaskCode<'TOverall, unit> -> TaskCode<'TOverall, unit>

    /// <summary>
    /// Specifies a unit of task code which produces no result
    /// </summary>
    [<DefaultValue>]
    member inline Zero: unit -> TaskCode<'TOverall, unit>

/// <summary>
/// Contains methods to build tasks using the F# computation expression syntax
/// </summary>
[<Class>]
type TaskBuilder =
    inherit TaskBuilderBase

    /// <summary>
    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    /// </summary>
    static member RunDynamic: code: TaskCode<'T, 'T> -> Task<'T>

    /// Hosts the task code in a state machine and starts the task.
    member inline Run: code: TaskCode<'T, 'T> -> Task<'T>

/// <summary>
/// Contains methods to build tasks using the F# computation expression syntax
/// </summary>
[<Class>]
type BackgroundTaskBuilder =
    inherit TaskBuilderBase

    /// <summary>
    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    /// </summary>
    static member RunDynamic: code: TaskCode<'T, 'T> -> Task<'T>

    /// <summary>
    /// Hosts the task code in a state machine and starts the task, executing in the threadpool using Task.Run
    /// </summary>
    member inline Run: code: TaskCode<'T, 'T> -> Task<'T>

/// Contains the `task` computation expression builder.
[<AutoOpen>]
module TaskBuilder =

    /// <summary>
    /// Builds a task using computation expression syntax.
    /// </summary>
    ///
    /// <example-tbd></example-tbd>
    val task: TaskBuilder

    /// <summary>
    /// Builds a task using computation expression syntax which switches to execute on a background thread if not
    /// already doing so.
    /// </summary>
    ///
    /// <remarks>
    /// If the task is created on a foreground thread (where <see cref="P:System.Threading.SynchronizationContext.Current"/> is non-null)
    /// its body is executed on a background thread using <see cref="M:System.Threading.Tasks.Task.Run"/>.
    /// If created on a background thread (where <see cref="P:System.Threading.SynchronizationContext.Current"/> is null) it is executed
    /// immediately on that thread.
    /// </remarks>
    ///
    /// <example-tbd></example-tbd>
    val backgroundTask: BackgroundTaskBuilder

// Contains the `task` computation expression builder.
namespace Microsoft.FSharp.Control.TaskBuilderExtensions

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

/// <summary>
/// Contains low-priority overloads for the `task` computation expression builder.
/// </summary>
//
// Note: they are low priority because they are auto-opened first, and F# has a rule
// that extension method opened later in sequence get higher priority
//
// AutoOpen is by assembly attribute to get sequencing of AutoOpen correct and
// so each gives different priority
module LowPriority =

    type TaskBuilderBase with

        /// <summary>
        /// Specifies a unit of task code which draws a result from a task-like value
        /// satisfying the GetAwaiter pattern and calls a continuation.
        /// </summary>
        [<NoEagerConstraintApplication>]
        member inline Bind< ^TaskLike, 'TResult1, 'TResult2, ^Awaiter, 'TOverall> :
            task: ^TaskLike * continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) ->
                TaskCode<'TOverall, 'TResult2>
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'TResult1)

        /// <summary>
        /// Specifies a unit of task code which draws its result from a task-like value
        /// satisfying the GetAwaiter pattern.
        /// </summary>
        [<NoEagerConstraintApplication>]
        member inline ReturnFrom< ^TaskLike, ^Awaiter, 'T> :
            task: ^TaskLike -> TaskCode<'T, 'T>
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'T)

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        /// </summary>
        [<NoEagerConstraintApplication>]
        static member inline BindDynamic< ^TaskLike, 'TResult1, 'TResult2, ^Awaiter, 'TOverall> :
            sm: byref<TaskStateMachine<'TOverall>> *
            task: ^TaskLike *
            continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) ->
                bool
                when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit -> 'TResult1)

        /// <summary>
        /// Specifies a unit of task code which binds to the resource implementing IDisposable and disposes it synchronously
        /// </summary>
        member inline Using:
            resource: 'Resource * body: ('Resource -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
                when 'Resource :> IDisposable | null

    type TaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for two task-like values.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter1, ^Awaiter2> :
            task1: ^TaskLike1 * task2: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

    type BackgroundTaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for two task-like values.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter1, ^Awaiter2> :
            task1: ^TaskLike1 * task2: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

module LowPlusPriority =

    type TaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for an async and a task-like value.
        /// </summary>
        member inline MergeSources< ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter2> :
            computation: Async< ^TResult1 > * task: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

        /// <summary>
        /// Implementation of the `and!` operation for a task-like value and an async.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TResult1, ^TResult2, ^Awaiter1> :
            task: ^TaskLike1 * computation: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)

    type BackgroundTaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for an async and a task-like value.
        /// </summary>
        member inline MergeSources< ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter2> :
            computation: Async< ^TResult1 > * task: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

        /// <summary>
        /// Implementation of the `and!` operation for a task-like value and an async.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TResult1, ^TResult2, ^Awaiter1> :
            task: ^TaskLike1 * computation: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)

/// <summary>
/// Contains medium-priority overloads for the `task` computation expression builder.
/// </summary>
module MediumPriority =

    type TaskBuilderBase with

        /// <summary>
        /// Specifies a unit of task code which draws a result from an F# async value then calls a continuation.
        /// </summary>
        member inline Bind:
            computation: Async<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) ->
                TaskCode<'TOverall, 'TResult2>

        /// <summary>
        /// Specifies a unit of task code which draws a result from an F# async value.
        /// </summary>
        member inline ReturnFrom: computation: Async<'T> -> TaskCode<'T, 'T>

    type TaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for a task and a task-like value.
        /// </summary>
        member inline MergeSources< ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter2> :
            task1: Task< ^TResult1 > * task2: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

        /// <summary>
        /// Implementation of the `and!` operation for a task-like value and a task.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TResult1, ^TResult2, ^Awaiter1> :
            task1: ^TaskLike1 * task2: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)

        /// <summary>
        /// Implementation of the `and!` operation for two asyncs.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            computation1: Async< ^TResult1 > * computation2: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

        /// <summary>
        /// Implementation of the `and!` operation for a task and an async.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            task: Task< ^TResult1 > * computation: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

        /// <summary>
        /// Implementation of the `and!` operation for an async and a task.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            computation: Async< ^TResult1 > * task: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

    type BackgroundTaskBuilder with

        /// <summary>
        /// Implementation of the `and!` operation for a task and a task-like value.
        /// </summary>
        member inline MergeSources< ^TaskLike2, ^TResult1, ^TResult2, ^Awaiter2> :
            task1: Task< ^TResult1 > * task2: ^TaskLike2 -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
                and ^Awaiter2 :> ICriticalNotifyCompletion
                and ^Awaiter2: (member get_IsCompleted: unit -> bool)
                and ^Awaiter2: (member GetResult: unit -> ^TResult2)

        /// <summary>
        /// Implementation of the `and!` operation for a task-like value and a task.
        /// </summary>
        member inline MergeSources< ^TaskLike1, ^TResult1, ^TResult2, ^Awaiter1> :
            task1: ^TaskLike1 * task2: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>
                when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
                and ^Awaiter1 :> ICriticalNotifyCompletion
                and ^Awaiter1: (member get_IsCompleted: unit -> bool)
                and ^Awaiter1: (member GetResult: unit -> ^TResult1)

        /// <summary>
        /// Implementation of the `and!` operation for two asyncs.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            computation1: Async< ^TResult1 > * computation2: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

        /// <summary>
        /// Implementation of the `and!` operation for a task and an async.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            task: Task< ^TResult1 > * computation: Async< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

        /// <summary>
        /// Implementation of the `and!` operation for an async and a task.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            computation: Async< ^TResult1 > * task: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

/// <summary>
/// Contains high-priority overloads for the `task` computation expression builder.
/// </summary>
module HighPriority =

    type TaskBuilderBase with

        /// <summary>
        /// Specifies a unit of task code which draws a result from a task then calls a continuation.
        /// </summary>
        member inline Bind:
            task: Task<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) ->
                TaskCode<'TOverall, 'TResult2>

        /// <summary>
        /// Specifies a unit of task code which draws a result from a task.
        /// </summary>
        member inline ReturnFrom: task: Task<'T> -> TaskCode<'T, 'T>

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        /// </summary>
        static member BindDynamic:
            sm: byref<TaskStateMachine<'TOverall>> *
            task: Task<'TResult1> *
            continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) ->
                bool

    type TaskBuilder with
        /// <summary>
        /// Implementation of the `and!` operation for two tasks.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            task1: Task< ^TResult1 > * task2: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

    type BackgroundTaskBuilder with
        /// <summary>
        /// Implementation of the `and!` operation for two tasks.
        /// </summary>
        member inline MergeSources< ^TResult1, ^TResult2> :
            task1: Task< ^TResult1 > * task2: Task< ^TResult2 > -> Task<struct (^TResult1 * ^TResult2)>

namespace Microsoft.FSharp.Control

open System.Threading.Tasks
open Microsoft.FSharp.Core

/// <summary>Contains camelCase module-level functions for <see cref="T:System.Threading.Tasks.Task`1"/> computations.</summary>
///
/// <category index="1">Async Programming</category>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Task =

    /// <summary>Creates a task that returns the given value.</summary>
    ///
    /// <param name="value">The value to return.</param>
    ///
    /// <returns>A completed task that returns <c>value</c>.</returns>
    ///
    /// <example id="task-result-1">
    /// <code lang="fsharp">
    /// let t = Task.result 42
    /// t.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Result")>]
    val inline result: value: 'T -> Task<'T>

    /// <summary>Creates a task that applies the mapping function to the result of the given task.</summary>
    ///
    /// <param name="mapping">The function to apply to the result.</param>
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A task that applies <c>mapping</c> to the result of <c>task</c>.</returns>
    ///
    /// <example id="task-map-1">
    /// <code lang="fsharp">
    /// let t = Task.result 21 |> Task.map (fun x -> x * 2)
    /// t.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val inline map: mapping: ('T -> 'U) -> task: Task<'T> -> Task<'U>

    /// <summary>Creates a task that passes the result of the given task to the binder function.</summary>
    ///
    /// <param name="binder">A function that takes the result of the task and returns a new task.</param>
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A task that performs a monadic bind on the result of <c>task</c>.</returns>
    ///
    /// <example id="task-bind-1">
    /// <code lang="fsharp">
    /// let t = Task.result 21 |> Task.bind (fun x -> Task.result (x * 2))
    /// t.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Bind")>]
    val inline bind: binder: ('T -> Task<'U>) -> task: Task<'T> -> Task<'U>

    /// <summary>Creates a task that runs the given task and ignores its result.</summary>
    ///
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A task that is equivalent to the input task, but disregards the result.</returns>
    ///
    /// <example id="task-ignore-1">
    /// <code lang="fsharp">
    /// let t : Task&lt;unit&gt; = Task.result 42 |> Task.ignore&lt;int&gt;
    /// t.Result // evaluates to ()
    /// </code>
    /// </example>
    [<CompiledName("Ignore")>]
    [<RequiresExplicitTypeArguments>]
    val inline ignore<'T> : task: Task<'T> -> Task<unit>

    /// <summary>Creates a task that runs the given task.
    /// If it raises an exception, the handler function is called with the exception and its result is returned.</summary>
    ///
    /// <param name="handler">A function to handle exceptions, returning a recovery value.</param>
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A task that returns the result of <c>task</c>, or the result of <c>handler</c> if an exception is raised.</returns>
    ///
    /// <example id="task-catchwith-1">
    /// <code lang="fsharp">
    /// let safeDiv x y =
    ///     task { return x / y }
    ///     |> Task.catchWith (fun _ -> 0)
    /// (safeDiv 10 0).Result // evaluates to 0
    /// </code>
    /// </example>
    [<CompiledName("CatchWith")>]
    val inline catchWith: handler: (exn -> 'T) -> task: Task<'T> -> Task<'T>

    /// <summary>Creates a task that runs the given task and returns its result as <c>Ok</c>,
    /// or returns <c>Error</c> with the exception if one is raised.</summary>
    ///
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A task that returns <c>Ok</c> of the result or <c>Error</c> of the exception.</returns>
    ///
    /// <example id="task-catch-1">
    /// <code lang="fsharp">
    /// let safeDiv x y = task { return x / y } |> Task.catch
    /// (safeDiv 10 2).Result // evaluates to Ok 5
    /// (safeDiv 10 0).Result // evaluates to Error (DivideByZeroException ...)
    /// </code>
    /// </example>
    [<CompiledName("Catch")>]
    val catch: task: Task<'T> -> Task<Result<'T, exn>>

    /// <summary>A completed task that returns <c>unit</c>. This is a <c>Task&lt;unit&gt;</c> (not the non-generic <c>Task.CompletedTask</c>).</summary>
    ///
    /// <example id="task-empty-1">
    /// <code lang="fsharp">
    /// Task.empty.Result // evaluates to ()
    /// </code>
    /// </example>
    [<CompiledName("Empty")>]
    val empty: Task<unit>

#if NETSTANDARD2_1
    /// <summary>Converts a <see cref="T:System.Threading.Tasks.ValueTask`1"/> to a <see cref="T:System.Threading.Tasks.Task`1"/>.</summary>
    ///
    /// <param name="valueTask">The input value task.</param>
    ///
    /// <returns>A task equivalent to the given value task.</returns>
    ///
    /// <example id="task-ofvaluetask-1">
    /// <code lang="fsharp">
    /// let vt = ValueTask&lt;int&gt;(42)
    /// let t = Task.ofValueTask vt
    /// t.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("OfValueTask")>]
    val inline ofValueTask: valueTask: ValueTask<'T> -> Task<'T>
#endif

#if NETSTANDARD2_1
/// <summary>Contains camelCase module-level functions for <see cref="T:System.Threading.Tasks.ValueTask`1"/> computations.</summary>
///
/// <category index="1">Async Programming</category>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ValueTask =

    /// <summary>Creates a value task that returns the given value.</summary>
    ///
    /// <param name="value">The value to return.</param>
    ///
    /// <returns>A completed value task that returns <c>value</c>.</returns>
    ///
    /// <example id="valuetask-result-1">
    /// <code lang="fsharp">
    /// let vt = ValueTask.result 42
    /// vt.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Result")>]
    val inline result: value: 'T -> ValueTask<'T>

    /// <summary>Creates a value task that applies the mapping function to the result of the given value task.</summary>
    ///
    /// <param name="mapping">The function to apply to the result.</param>
    /// <param name="task">The input value task.</param>
    ///
    /// <returns>A value task that applies <c>mapping</c> to the result of <c>task</c>.</returns>
    ///
    /// <example id="valuetask-map-1">
    /// <code lang="fsharp">
    /// let vt = ValueTask.result 21 |> ValueTask.map (fun x -> x * 2)
    /// vt.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Map")>]
    val inline map: mapping: ('T -> 'U) -> task: ValueTask<'T> -> ValueTask<'U>

    /// <summary>Creates a value task that passes the result of the given value task to the binder function.</summary>
    ///
    /// <param name="binder">A function that takes the result of the value task and returns a new value task.</param>
    /// <param name="task">The input value task.</param>
    ///
    /// <returns>A value task that performs a monadic bind on the result of <c>task</c>.</returns>
    ///
    /// <example id="valuetask-bind-1">
    /// <code lang="fsharp">
    /// let vt = ValueTask.result 21 |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
    /// vt.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("Bind")>]
    val inline bind: binder: ('T -> ValueTask<'U>) -> task: ValueTask<'T> -> ValueTask<'U>

    /// <summary>Creates a value task that runs the given value task and ignores its result.</summary>
    ///
    /// <remarks>When the value task is already synchronously complete, this avoids allocating a <c>Task</c>.</remarks>
    ///
    /// <param name="task">The input value task.</param>
    ///
    /// <returns>A value task that is equivalent to the input value task, but disregards the result.</returns>
    ///
    /// <example id="valuetask-ignore-1">
    /// <code lang="fsharp">
    /// let vt : ValueTask&lt;unit&gt; = ValueTask.result 42 |> ValueTask.ignore&lt;int&gt;
    /// vt.Result // evaluates to ()
    /// </code>
    /// </example>
    [<CompiledName("Ignore")>]
    [<RequiresExplicitTypeArguments>]
    val inline ignore<'T> : task: ValueTask<'T> -> ValueTask<unit>

    /// <summary>Creates a value task that runs the given value task.
    /// If it raises an exception, the handler function is called with the exception and its result is returned.</summary>
    ///
    /// <param name="handler">A function to handle exceptions, returning a recovery value.</param>
    /// <param name="task">The input value task.</param>
    ///
    /// <returns>A value task that returns the result of <c>task</c>, or the result of <c>handler</c> if an exception is raised.</returns>
    ///
    /// <example id="valuetask-catchwith-1">
    /// <code lang="fsharp">
    /// let safeDiv x y =
    ///     task { return x / y }
    ///     |> ValueTask.ofTask
    ///     |> ValueTask.catchWith (fun _ -> 0)
    /// (safeDiv 10 0).Result // evaluates to 0
    /// </code>
    /// </example>
    [<CompiledName("CatchWith")>]
    val inline catchWith: handler: (exn -> 'T) -> task: ValueTask<'T> -> ValueTask<'T>

    /// <summary>Creates a value task that runs the given value task and returns its result as <c>Ok</c>,
    /// or returns <c>Error</c> with the exception if one is raised.</summary>
    ///
    /// <param name="task">The input value task.</param>
    ///
    /// <returns>A value task that returns <c>Ok</c> of the result or <c>Error</c> of the exception.</returns>
    ///
    /// <example id="valuetask-catch-1">
    /// <code lang="fsharp">
    /// let safeDiv x y = task { return x / y } |> ValueTask.ofTask |> ValueTask.catch
    /// (safeDiv 10 2).Result // evaluates to Ok 5
    /// (safeDiv 10 0).Result // evaluates to Error (DivideByZeroException ...)
    /// </code>
    /// </example>
    [<CompiledName("Catch")>]
    val catch: task: ValueTask<'T> -> ValueTask<Result<'T, exn>>

    /// <summary>A completed value task that returns <c>unit</c>.</summary>
    ///
    /// <example id="valuetask-empty-1">
    /// <code lang="fsharp">
    /// ValueTask.empty.Result // evaluates to ()
    /// </code>
    /// </example>
    [<CompiledName("Empty")>]
    val empty: ValueTask<unit>

    /// <summary>Converts a <see cref="T:System.Threading.Tasks.Task`1"/> to a <see cref="T:System.Threading.Tasks.ValueTask`1"/>.</summary>
    ///
    /// <param name="task">The input task.</param>
    ///
    /// <returns>A value task equivalent to the given task.</returns>
    ///
    /// <example id="valuetask-oftask-1">
    /// <code lang="fsharp">
    /// let t = Task.FromResult 42
    /// let vt = ValueTask.ofTask t
    /// vt.Result // evaluates to 42
    /// </code>
    /// </example>
    [<CompiledName("OfTask")>]
    val inline ofTask: task: Task<'T> -> ValueTask<'T>
#endif
