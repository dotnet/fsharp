// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

    open Microsoft.FSharp.Core
    open System.Runtime.CompilerServices

    /// A marker interface to give priority to different available overloads
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads
    type IPriority1 = interface inherit IPriority2 end

    module CodeGenHelpers = 
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __jumptable : int -> (unit -> 'T) -> 'T

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __stateMachineStruct<'Template, 'Meth1, 'Meth2, 'Result> : meth1: 'Meth1 -> meth2: 'Meth2 -> after: (unit -> 'Result) -> 'Result

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __stateMachine<'T> : _obj: 'T -> 'T

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __newEntryPoint: unit -> int

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __machine<'T> : 'T

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __machineAddr<'T> : byref<'T>

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __entryPoint: int -> unit

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __return : 'T -> 'T

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the result of a computation, a value of true indicates completion
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T, 'TOverall> =
    new : completed: bool -> TaskStep<'T, 'TOverall>
    member IsCompleted: bool

[<Struct; NoComparison; NoEquality>]
/// This is used by the compiler as a template for creating state machine structs
type TaskStateMachineTemplate<'T> =
    [<DefaultValue(false)>]
    val mutable Result : 'T

    [<DefaultValue>]
    val mutable ResumptionPoint : int

    [<DefaultValue>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'T>

    interface IAsyncStateMachine

type TaskBuilder =
    new: unit -> TaskBuilder
    member inline Combine: task1: TaskStep<unit, 'TOverall> * task2: (unit -> TaskStep<'T, 'TOverall>) -> TaskStep<'T, 'TOverall>
    member inline Delay: f: (unit -> TaskStep<'T, 'TOverall>) -> (unit -> TaskStep<'T, 'TOverall>)
    member inline For: sequence: seq<'T> * body: ('T -> TaskStep<unit, 'TOverall>) -> TaskStep<unit, 'TOverall>
    member inline Return: x: 'T -> TaskStep<'T, 'T>
    member inline ReturnFrom: task: Task<'T> -> TaskStep<'T, 'T>
    member inline Run: code: (unit -> TaskStep<'T, 'T>) -> Task<'T>
    member inline TryFinally: body: (unit -> TaskStep<'T, 'TOverall>) * fin: (unit -> unit) -> TaskStep<'T, 'TOverall>
    member inline TryWith: body: (unit -> TaskStep<'T, 'TOverall>) * catch: (exn -> TaskStep<'T, 'TOverall>) -> TaskStep<'T, 'TOverall>
    member inline Using: disp: 'Resource * body: ('Resource -> TaskStep<'T, 'TOverall>) -> TaskStep<'T, 'TOverall> when 'Resource :> IDisposable
    member inline While: condition: (unit -> bool) * body: (unit -> TaskStep<unit, 'TOverall>) -> TaskStep<unit, 'TOverall>
    member inline Zero: unit -> TaskStep<unit, 'TOverall>

[<AutoOpen>]
module ContextSensitiveTasks = 

    /// Builds a `System.Th`reading.Tasks.Task<'T>` similarly to a C# async/await method.
    /// Use this like `task { let! taskResult = someTask(); return taskResult.ToString(); }`.
    val task : TaskBuilder

    /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
    [<Sealed; NoComparison; NoEquality>]
    type Witnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like types can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall >
                : priority: IPriority2 * taskLike: ^TaskLike * k: ( ^TResult1 -> TaskStep< 'TResult2, 'TOverall>) -> TaskStep< 'TResult2, 'TOverall>
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        static member inline CanBind: priority: IPriority1 * task: Task<'TResult1> * k: ('TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>

        /// Provides evidence that F# Async computations can be used in 'bind' in a task computation expression
        static member inline CanBind: priority: IPriority1 * computation: Async<'TResult1> * k: ('TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>

        /// Provides evidence that task-like types can be used in 'return' in a task workflow
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T> : priority: IPriority1 * taskLike: ^TaskLike -> TaskStep< ^T, ^T > 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit ->  ^T)

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        static member inline CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T, 'T>

    type TaskBuilder with
      /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
      member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>
          when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>)

      /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
      member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'T, 'T >
          when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T, 'T>)

module ContextInsensitiveTasks = 

    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method, but where
    /// awaited tasks are not automatically configured to resume on the captured context.
    ///
    /// This is often preferable when writing library code that is not context-aware, but undesirable when writing
    /// e.g. code that must interact with user interface controls on the same thread as its caller.
    val task : TaskBuilder

    /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
    [<Sealed; NoComparison; NoEquality>]
    type Witnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > : priority: IPriority3 * taskLike: ^TaskLike * k: ( ^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>
            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)

        /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter, 'TOverall > : priority: IPriority2 * taskLike: ^TaskLike * k: (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>
            when  ^TaskLike: (member ConfigureAwait: bool ->  ^Awaitable)
            and ^Awaitable: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit ->  ^TResult1)

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        static member inline CanBind: priority: IPriority1 * task: Task<'TResult1> * k: ('TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>

        /// Provides evidence that F# async computations can be used in 'bind' in a task computation expression
        static member inline CanBind: priority: IPriority1 * computation: Async<'TResult1> * k: ('TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>

(*
        /// Provides evidence that types following the "awaitable" pattern can be used in 'return!' in a task computation expression
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T> : IPriority2 * taskLike: ^Awaitable -> TaskStep< ^T>
                                                when  ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                                and ^Awaiter :> ICriticalNotifyCompletion
                                                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                and ^Awaiter: (member GetResult: unit -> ^T)

        /// Provides evidence that types following the task-like pattern can be used in 'return!' in a task computation expression
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                and ^Awaiter :> ICriticalNotifyCompletion 
                                                and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                and ^Awaiter : (member GetResult : unit -> ^T) > : IPriority1 * configurableTaskLike: ^TaskLike -> TaskStep< ^T>

        /// Provides evidence that F# async computations can be used in 'return!' in a task computation expression
        static member inline CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T>
*)

    type TaskBuilder with

      /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
      member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>
          when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>)

(*
      /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
      member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'TResult >
          when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'TResult>)
*)

#endif