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

    type MachineFunc<'Machine> = delegate of byref<'Machine> -> bool

    type MoveNextMethod<'Template> = delegate of byref<'Template> -> unit

    type SetMachineStateMethod<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

    type AfterMethod<'Template, 'Result> = delegate of byref<'Template> -> 'Result

    module StateMachineHelpers = 
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __entryPoint: MachineFunc<'Machine> -> MachineFunc<'Machine>

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __stateMachinesSupported<'T> : bool 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __jumptableSMH : pc: int -> code: 'T -> 'T

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __stateMachineSMH<'T> : _obj: 'T -> 'T

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __stateMachineStructSMH<'Template, 'Result> : moveNext: MoveNextMethod<'Template> -> _setMachineState: MoveNextMethod<'Template> -> after: AfterMethod<'Template, 'Result> -> 'Result

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        val __entryPointIdSMH: MachineFunc<'Machine> -> int

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the result of a computation, a value of true indicates completion
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T> =
    new : completed: bool -> TaskStep<'T>
    member IsCompleted: bool

[<Struct; NoComparison; NoEquality>]
/// This is used by the compiler as a template for creating state machine structs
type TaskStateMachine<'T> =

    /// Holds the final result of the state machine
    [<DefaultValue(false)>]
    val mutable Result : 'T

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint : int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc : MachineFunc<TaskStateMachine<'T>>

    /// When interpreted, holds the awaiter used to suspend of the state machine
    [<DefaultValue(false)>]
    val mutable Awaiter : ICriticalNotifyCompletion

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'T>

    // For debugging
    member Address: nativeint

    interface IAsyncStateMachine

type TaskCode<'TOverall, 'T> = delegate of byref<TaskStateMachine<'TOverall>> -> TaskStep<'T>

[<Class>]
type TaskBuilder =
    
    member inline Combine: task1: TaskCode<'TOverall, unit> * task2: TaskCode<'TOverall, 'T> -> TaskCode<'TOverall, 'T>
    
    member inline Delay: f: (unit -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
    
    member inline For: sequence: seq<'T> * body: ('T -> TaskCode<'TOverall, unit>) -> TaskCode<'TOverall, unit>
    
    member inline Return: x: 'T -> TaskCode<'T, 'T>
    
    member inline Run: code: TaskCode<'T, 'T> -> Task<'T>
    
    member inline TryFinally: body: TaskCode<'TOverall, 'T> * fin: (unit -> unit) -> TaskCode<'TOverall, 'T>
    
    member inline TryWith: body: TaskCode<'TOverall, 'T> * catch: (exn -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
    
    member inline Using: disp: 'Resource * body: ('Resource -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T> when 'Resource :> IDisposable
    
    member inline While: condition: (unit -> bool) * body: TaskCode<'TOverall, unit> -> TaskCode<'TOverall, unit>
    
    [<DefaultValue>]
    member inline Zero: unit -> TaskCode<'TOverall, unit>

    member inline ReturnFrom: task: Task<'T> -> TaskCode<'T, 'T>

and 

    /// Provides evidence that various types can be used in bind and return constructs in task computation expressions
    [<Sealed; NoComparison; NoEquality>]
    Witnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like types can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall >
                : priority: IPriority2 * taskLike: ^TaskLike * k: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind: priority: IPriority1 * task: Task<'TResult1> * k: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that F# Async computations can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind: priority: IPriority1 * computation: Async<'TResult1> * k: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that task-like types can be used in 'return' in a task workflow
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T> : priority: IPriority2 * taskLike: ^TaskLike -> TaskCode< ^T, ^T > 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit ->  ^T)

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom: IPriority1 * computation: Task<'T> -> TaskCode<'T, 'T>

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskCode<'T, 'T>

[<AutoOpen>]
module TaskHelpers = 

    /// Builds a `System.Threading.Tasks.Task<'T>` similarly to a C# async/await method.
    /// Use this like `task { let! taskResult = someTask(); return taskResult.ToString(); }`.
    val task : TaskBuilder

    type TaskBuilder with 
        /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
        member inline Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> 
                     : task: ^TaskLike * continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>        

        /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
        member inline ReturnFrom: a: ^TaskLike -> TaskCode< 'T, 'T >
            when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskCode<'T, 'T>)


(*
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
        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > : priority: IPriority3 * taskLike: ^TaskLike * k: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)

        /// Provides evidence that task-like computations can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter, 'TOverall > : priority: IPriority2 * taskLike: ^TaskLike * k: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
            when  ^TaskLike: (member ConfigureAwait: bool ->  ^Awaitable)
            and ^Awaitable: (member GetAwaiter:  unit ->  ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit ->  ^TResult1)

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind: priority: IPriority1 * task: Task<'TResult1> * k: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that F# async computations can be used in 'bind' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanBind: priority: IPriority1 * computation: Async<'TResult1> * k: ('TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that types following the "awaitable" pattern can be used in 'return!' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T> : IPriority2 * taskLike: ^Awaitable -> TaskStep< ^T, ^T>
                                                when  ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                                and ^Awaiter :> ICriticalNotifyCompletion
                                                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                and ^Awaiter: (member GetResult: unit -> ^T)

        /// Provides evidence that types following the task-like pattern can be used in 'return!' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                and ^Awaiter :> ICriticalNotifyCompletion 
                                                and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                and ^Awaiter : (member GetResult : unit -> ^T) > : IPriority1 * configurableTaskLike: ^TaskLike -> TaskStep< ^T, ^T>

        /// Provides evidence that F# async computations can be used in 'return!' in a task computation expression
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom: IPriority1 * computation: Async<'T> -> TaskStep<'T, 'T>

    type TaskBuilder with

        /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
        [<NoDynamicInvocation>]
        member inline Bind : task: ^TaskLike * continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>
            when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)

        /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
        [<NoDynamicInvocation>]
        member inline ReturnFrom: a: ^TaskLike -> TaskStep< 'T, 'T >
            when (Witnesses or  ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T, 'T>)
*)
#endif