// TaskBuilder.fs - TPL task computation expressions for F#
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

[<Struct; NoComparison; NoEquality>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
[<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
/// The extra data stored in ResumableStateMachine for tasks
type TaskStateMachineData<'T> =

    /// Holds the final result of the state machine
    [<DefaultValue(false)>]
    val mutable Result : 'T

    /// Holds the MethodBuilder for the state machine
    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'T>

/// This is used by the compiler as a template for creating state machine structs
and [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
    TaskStateMachine<'TOverall> = ResumableStateMachine<TaskStateMachineData<'TOverall>>

/// Represents the runtime continuation of a task state machine created dynamically
and [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
    TaskResumptionFunc<'TOverall> = ResumptionFunc<TaskStateMachineData<'TOverall>>

/// A special compiler-recognised delegate type for specifying blocks of task code
/// with access to the state machine.
and [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    [<CompilerMessage("This construct  is for use by compiled F# code and should not be used directly", 1204, IsHidden=true)>]
    TaskCode<'TOverall, 'T> = ResumableCode<TaskStateMachineData<'TOverall>, 'T>

[<Class>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type TaskBuilderBase =
    
    /// Specifies the sequential composition of two units of task code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Combine: task1: TaskCode<'TOverall, unit> * task2: TaskCode<'TOverall, 'T> -> TaskCode<'TOverall, 'T>
    
    /// Specifies the delayed execution of a unit of task code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Delay: f: (unit -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
    
    /// Specifies the iterative execution of a unit of task code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline For: sequence: seq<'T> * body: ('T -> TaskCode<'TOverall, unit>) -> TaskCode<'TOverall, unit>
    
    /// Specifies a unit of task code which returns a value
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Return: value: 'T -> TaskCode<'T, 'T>
    
    /// Specifies a unit of task code which excuted using try/finally semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline TryFinally: body: TaskCode<'TOverall, 'T> * compensation: (unit -> unit) -> TaskCode<'TOverall, 'T>
    
    /// Specifies a unit of task code which excuted using try/with semantics
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline TryWith: body: TaskCode<'TOverall, 'T> * catch: (exn -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
    
#if NETSTANDARD2_1
    /// Specifies a unit of task code which binds to the resource and disposes it asynchronously
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Using<'Resource, 'TOverall, 'T when 'Resource :> IAsyncDisposable> : resource: 'Resource * body: ('Resource -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T>
#endif

    /// Specifies the iterative execution of a unit of task code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline While: condition: (unit -> bool) * body: TaskCode<'TOverall, unit> -> TaskCode<'TOverall, unit>
    
    /// Specifies a unit of task code which produces no result
    [<DefaultValue>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Zero: unit -> TaskCode<'TOverall, unit>

    /// Specifies a unit of task code which draws its result from a task
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline ReturnFrom: task: Task<'T> -> TaskCode<'T, 'T>

    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    static member ReturnFromDynamic: sm: byref<TaskStateMachine<'T>> * task: Task<'T> -> bool

[<Class>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type TaskBuilder =
    inherit TaskBuilderBase

    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    static member RunDynamic: code: TaskCode<'T, 'T> -> Task<'T>
    
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Run: code: TaskCode<'T, 'T> -> Task<'T>

[<Class>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type BackgroundTaskBuilder =
    inherit TaskBuilderBase

    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    member inline Run: code: TaskCode<'T, 'T> -> Task<'T>

/// Provides evidence that various types can be used in bind and return constructs in task computation expressions
[<Sealed; NoComparison; NoEquality>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
type TaskWitnesses =
        interface IPriority1
        interface IPriority2
        interface IPriority3

        /// Provides evidence that task-like types can be used in 'bind' in a task computation expression
                
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > :
            priority: IPriority2 *
            task: ^TaskLike *
            continuation: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                -> TaskCode<'TOverall, 'TResult2>
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

        /// Provides evidence that tasks can be used in 'bind' in a task computation expression
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanBind:
            priority: IPriority1 *
            task: Task<'TResult1> *
            continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that F# Async computations can be used in 'bind' in a task computation expression
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanBind:
            priority: IPriority1 *
            computation: Async<'TResult1> *
            continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                -> TaskCode<'TOverall, 'TResult2>

        /// Provides evidence that task-like types can be used in 'return' in a task workflow
                
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T> : 
            priority: IPriority2 *
            task: ^TaskLike
                -> TaskCode< ^T, ^T > 
                when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit ->  ^T)

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanReturnFrom:
            priority: IPriority1 *
            task: Task<'T>
                -> TaskCode<'T, 'T>

        /// Provides evidence that F# Async computations can be used in 'return' in a task computation expression
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanReturnFrom:
            priority: IPriority1 *
            computation: Async<'T>
                -> TaskCode<'T, 'T>

        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall > :
            sm: byref<TaskStateMachine<'TOverall>> *
            priority: IPriority2 *
            task: ^TaskLike *
            continuation: ( ^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                -> bool
                when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                and ^Awaiter: (member GetResult:  unit ->  ^TResult1) 

        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member CanBindDynamic:
            sm: byref<TaskStateMachine<'TOverall>> *
            priority: IPriority1 *
            task: Task<'TResult1> *
            continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)
                -> bool

        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member inline CanReturnFromDynamic< ^TaskLike, ^Awaiter, ^T> :
            sm: byref<TaskStateMachine< ^T >> *
            priority: IPriority2 *
            task: ^TaskLike
                -> bool
                when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                and ^Awaiter :> ICriticalNotifyCompletion
                and ^Awaiter: (member get_IsCompleted: unit -> bool)
                and ^Awaiter: (member GetResult: unit ->  ^T)

        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        static member CanReturnFromDynamic:
            sm: byref<TaskStateMachine<'T>> *
            task: Task<'T>
                -> bool

[<AutoOpen>]
module TaskHelpers = 

    type TaskBuilderBase with 
        /// Provides the ability to bind to a variety of tasks, using context-sensitive semantics
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> :
                            task: ^TaskLike * 
                            continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)
                                -> TaskCode<'TOverall, 'TResult2>        

        /// Provides the ability to return results from a variety of tasks, using context-sensitive semantics
        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline ReturnFrom: task: ^TaskLike -> TaskCode< 'T, 'T >
            when (TaskWitnesses or  ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskCode<'T, 'T>)

        [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
        member inline Using: resource: 'Resource * body: ('Resource -> TaskCode<'TOverall, 'T>) -> TaskCode<'TOverall, 'T> when 'Resource :> IDisposable

/// Contains the `task` computation expression builder.
[<AutoOpen>]
[<Experimental("Experimental library feature, requires '--langversion:preview'")>]
module TaskBuilder = 

    /// Builds a task using computation expression syntax
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val task: TaskBuilder

    /// Builds a background task using computation expression syntax. A background
    /// task immediately executes a dummy task with <c>ConfigureAwait(false)</c>, which
    /// means the contents of the task are scheduled in the background thread pool.
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val backgroundTask: BackgroundTaskBuilder

    
#endif
