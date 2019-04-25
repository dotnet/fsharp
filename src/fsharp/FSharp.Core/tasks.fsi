// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core.CompilerServices

    /// A marker interface to give priority to different available overloads
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads
    type IPriority1 = interface inherit IPriority2 end

namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the state of a computation: either awaiting something with a continuation, or completed with a return value.
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T> =
     static member Return : 'T -> TaskStep<'T>
     static member Await : ICriticalNotifyCompletion * (unit -> TaskStep<'T>) -> TaskStep<'T>
     static member ReturnFrom : Task<'T> -> TaskStep<'T>

type TaskBuilder =
    new: unit -> TaskBuilder
    member Combine: step: TaskStep<unit> * continuation: (unit -> TaskStep<'T>) -> TaskStep<'T>
    member Delay: f: (unit -> TaskStep<'T>) -> (unit -> TaskStep<'T>)
    member For: sequence: seq<'T> * body: ('T -> TaskStep<unit>) -> TaskStep<unit>
    member Return: x: 'T -> TaskStep<'T>
    member ReturnFrom: task: Task<'T> -> TaskStep<'T>
    member Run: f: (unit -> TaskStep<'T>) -> Task<'T>
    member TryFinally: body: (unit -> TaskStep<'T>) * fin: (unit -> unit) -> TaskStep<'T>
    member TryWith: body: (unit -> TaskStep<'T>) * catch: (exn -> TaskStep<'T>) -> TaskStep<'T>
    member Using: disp: 'Resource * body: ('Resource -> TaskStep<'T>) -> TaskStep<'T> when 'Resource :> IDisposable
    member While: condition: (unit -> bool) * body: (unit -> TaskStep<unit>) -> TaskStep<unit>
    member Zero: unit -> TaskStep<unit>

[<AutoOpen>]
module ContextSensitiveTasks = 

    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method.
    /// Use this like `task { let! taskResult = someTask(); return taskResult.ToString(); }`.
    val task: TaskBuilder

    /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
    [<Sealed; NoComparison; NoEquality>]
    type Witnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like types can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter >
                : priority: IPriority2 * taskLike: ^TaskLike -> (( ^TResult1 -> TaskStep< 'TResult2>) -> TaskStep< 'TResult2>)
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        static member CanBind: priority: IPriority1 * task: Task<'TResult1> -> (('TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)

        /// Provides evidence that F# Async computations can be used in 'bind' in a task computation expression
        static member CanBind: priority: IPriority1 * computation: Async<'TResult1> -> (('TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)

        /// Provides evidence that task-like types can be used in 'return' in a task workflow
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T> : priority: IPriority1 * taskLike: ^TaskLike -> TaskStep< ^T > 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit ->  ^T)

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        static member CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T>

    type TaskBuilder with
      /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
      member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >
          when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike -> ((^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >))

      /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
      member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'TResult >
          when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'TResult>)

module ContextInsensitiveTasks = 

    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method, but where
    /// awaited tasks are not automatically configured to resume on the captured context.
    ///
    /// This is often preferable when writing library code that is not context-aware, but undesirable when writing
    /// e.g. code that must interact with user interface controls on the same thread as its caller.
    val task: TaskBuilder

    /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
    [<Sealed; NoComparison; NoEquality>]
    type Witnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter > : priority: IPriority3 * taskLike: ^TaskLike -> (( ^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)
            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)

        /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter > : priority: IPriority2 * taskLike: ^TaskLike -> (( ^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)
            when  ^TaskLike: (member ConfigureAwait: bool ->  ^Awaitable)
            and ^Awaitable: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit ->  ^TResult1)

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        static member CanBind: priority: IPriority1 * task: Task<'TResult1> -> (('TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)

        /// Provides evidence that F# async computations can be used in 'bind' in a task computation expression
        static member CanBind: priority: IPriority1 * computation: Async<'TResult1> -> (('TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)

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
        static member CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T>

    type TaskBuilder with

      /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
      member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >
          when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike -> ((^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >))

      /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
      member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'TResult >
          when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'TResult>)

