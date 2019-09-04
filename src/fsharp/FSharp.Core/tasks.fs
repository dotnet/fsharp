// TaskBuilder.fs - TPL task computation expressions for F#
//
// Originally written in 2016 by Robert Peele (humbobst@gmail.com)
// New operator-based overload resolution for F# 4.0 compatibility by Gustavo Leon in 2018.
// Revised for insertion into FSHarp.Core by Microsoft, 2019.
//
// Original notice:
// To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights
// to this software to the public domain worldwide. This software is distributed without any warranty.
//
// Updates:
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#if FSHARP_CORE
namespace Microsoft.FSharp.Core.CompilerServices

    open System.Runtime.CompilerServices
    open Microsoft.FSharp.Core

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
        /// Statically determines whether a computation description is converted or not
        let __stateMachinesSupported<'T> : bool = false
        
        /// Indicates that the given closure can be used as an entry point in a state machine
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __entryPoint(code: MachineFunc<'Machine>) : MachineFunc<'Machine> = code

        /// In converted code, fetches the jumptable ID of the given entry point
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __entryPointIdSMH(_ep: MachineFunc<'Machine>) : int =
            failwith "__entryPointIdSMH should always be guarded by __stateMachinesSupported and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        /// If a computation description has been converted, inserts a jump table between different entry points
        let __jumptableSMH<'T> (_x:int) (_code: 'T)  : 'T =
            failwith "__jumptableSMH should always be guarded by __stateMachinesSupported and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        /// Attempts to convert a computation description to a reference state machine
        let __stateMachineSMH<'T> (_x: 'T) : 'T =
            failwith "__stateMachineSMH should always be guarded by __stateMachinesSupported and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        /// Describes a value-type (struct) state machine based on the given template.
        ///
        /// The template guides the generation of a new struct type.  Any mention of the template in any of the code is rewritten to that
        /// new struct type.  Meth1 and Meth2 are used to implement the methods on the interface implemented by the struct type.
        let __stateMachineStructSMH<'Template, 'Result> (_moveNext: MoveNextMethod<'Template>) (_setMachineState: MoveNextMethod<'Template>) (_after: AfterMethod<'Template, 'Result>): 'Result =
            failwith "__stateMachineStructSMH should always be guarded by __stateMachinesSupported and only used in valid state machine implementations"
        
#endif

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the state of a computation: either awaiting something with a
/// continuation, or completed with a return value.
//
// Uses a struct-around-single-reference to allow future changes in representation (the representation is
// not revealed in the signature)
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T>(completed: bool) = 
    member x.IsCompleted = completed

//[<NoComparison; NoEquality>]
//type TaskStateMachine<'T> =
//    [<DefaultValue(false)>]
//    val mutable State : TaskStateMachine<'T>

[<Struct; NoComparison; NoEquality>]
/// Acts as a template for struct state machines introduced by __stateMachineStructSMH, and also as a reflective implementation
type TaskStateMachine<'TOverall> =

    /// Holds the final result of the state machine
    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint : int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc : MachineFunc<TaskStateMachine<'TOverall>>

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'TOverall>

    interface IAsyncStateMachine with 
        
        // Used when interpreted.  For "__stateMachineStructSMH" it is replaced.
        member sm.MoveNext() = 
            try
                //Console.WriteLine("unconverted [{0}] step from {1}", sm.GetHashCode(), resumptionPoint)
                let step = sm.ResumptionFunc.Invoke(&sm) 
                if step then 
                    //Console.WriteLine("unconverted [{0}] SetResult {1}", sm.GetHashCode(), res)
                    sm.MethodBuilder.SetResult(sm.Result)
            with exn ->
                //Console.WriteLine("unconverted [{0}] exception {1}", sm.GetHashCode(), exn)
                sm.MethodBuilder.SetException exn

        // Used when interpreted.  For "__stateMachineStructSMH" it is replaced.
        member sm.SetStateMachine(state) = 
            sm.MethodBuilder.SetStateMachine(state)

/// Represents a code fragment of a task.  When statically compiled, TaskCode is always removed and the body of the code inlined
/// into an invocation.
type TaskCode<'TOverall, 'T> = delegate of byref<TaskStateMachine<'TOverall>> -> TaskStep<'T>

[<AutoOpen>]
module TaskMethodRequire = 

    [<NoDynamicInvocation>]
    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2, 'TOverall 
                                when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) > (priority: ^Priority) (task: ^TaskLike) __expand_continuation = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) (priority, task, __expand_continuation))

    [<NoDynamicInvocation>]
    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskCode<'T, 'T>)> (priority: ^Priority) (task: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskCode<'T, 'T>) (priority, task))

// New style task builder.
type TaskBuilder() =

    member inline __.Delay(__expand_f : unit -> TaskCode<'TOverall, 'T>) =
        TaskCode<'TOverall, 'T>(fun sm -> (__expand_f()).Invoke(&sm))

    member inline __.Run(__expand_code : TaskCode<'TOverall, 'TOverall>) : Task<'TOverall> = 
#if ENABLED
        if __stateMachinesSupported then 
            __stateMachineStructSMH<TaskStateMachine<'TOverall>, Task<'TOverall>>
                // MoveNext
                (MachineFunc<_,_>(fun sm -> 
                    __jumptableSMH 
                        sm.ResumptionPoint 
                        (   try
                                //Console.WriteLine("[{0}] step from {1}", sm.GetHashCode(), resumptionPoint)
                                let ``__machine_step$cont`` = __expand_code.Invoke(&sm)
                                if ``__machine_step$cont``.IsCompleted then 
                                    //Console.WriteLine("[{0}] SetResult {1}", sm.GetHashCode(), res)
                                    sm.MethodBuilder.SetResult(sm.Result)
                            with exn ->
                                //Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
                                sm.MethodBuilder.SetException exn)))

                // SetStateMachine
                (MachineFunc<_,_,_>(fun sm state -> 
                    sm.MethodBuilder.SetStateMachine(state)))

                // Start
                (MachineFunc<_,_>(fun sm -> 
                    //Console.WriteLine("[{0}] start", sm.GetHashCode())
                    sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
                    sm.MethodBuilder.Start(&sm)
                    //Console.WriteLine("[{0}] unwrap", sm.GetHashCode())
                    sm.MethodBuilder.Task))
        else
#endif
            let mutable sm = TaskStateMachine<'TOverall>()
            sm.ResumptionFunc <- MachineFunc<_>(fun sm -> __expand_code.Invoke(&sm).IsCompleted) 
            //Console.WriteLine("[{0}] start", sm.GetHashCode())
            sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
            sm.MethodBuilder.Start(&sm)
            //Console.WriteLine("[{0}] unwrap", sm.GetHashCode())
            sm.MethodBuilder.Task        

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    //[<DefaultValue>]
    member inline __.Zero() : TaskCode<'TOverall, unit> =
        TaskCode<_,_>(fun sm ->
            TaskStep<unit>(true))

    member inline __.Return (x: 'TOverall) : TaskCode<'TOverall, 'TOverall> = 
        TaskCode<_,_>(fun sm -> 
            sm.Result <- x
            TaskStep<'TOverall>(true))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline __.Combine(__expand_task1: TaskCode<'TOverall, unit>, __expand_task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_,_>(fun sm ->
            let ``__machine_step$cont`` = __expand_task1.Invoke(&sm)
            if ``__machine_step$cont``.IsCompleted then 
                __expand_task2.Invoke(&sm)
            else
                TaskStep<'T>(false))

    /// Builds a step that executes the body while the condition predicate is true.
    member inline __.While (__expand_condition : unit -> bool, __expand_body : TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        TaskCode<_,_>(fun sm ->
            let mutable __stack_completed = true 
            while __stack_completed && __expand_condition() do
                __stack_completed <- false 
                // The body of the 'while' may include an early exit, e.g. return from entire method
                let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                // If we make it to the assignment we prove we've made a step 
                __stack_completed <- ``__machine_step$cont``.IsCompleted
            TaskStep<unit>(__stack_completed))

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryWith (__expand_body : TaskCode<'TOverall, 'T>, __expand_catch : exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_,_>(fun sm ->
            let mutable __stack_completed = false
            let mutable __stack_caught = false
            let mutable __stack_savedExn = Unchecked.defaultof<_>
            try
                // The try block may contain resumption points.
                // This is handled by the state machine rewriting 
                let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                // may skip this step.
                __stack_completed <- ``__machine_step$cont``.IsCompleted
            with exn -> 
                // The catch block may not contain resumption points.
                __stack_caught <- true
                __stack_savedExn <- exn

            if __stack_caught then 
                // Place the catch code outside the catch block 
                (__expand_catch __stack_savedExn).Invoke(&sm)
            else
                TaskStep<'T>(__stack_completed))

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryFinally (__expand_body: TaskCode<'TOverall, 'T>, compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        TaskCode<_,_>(fun sm ->
            let mutable __stack_completed = false
            try
                let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                // may skip this step.
                __stack_completed <- ``__machine_step$cont``.IsCompleted
            with _ ->
                compensation()
                reraise()

            if __stack_completed then 
                compensation()
            TaskStep<'T>(__stack_completed))

    member inline builder.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (disp : 'Resource, __expand_body : 'Resource -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        builder.TryFinally(
            TaskCode<_,_>(fun sm -> (__expand_body disp).Invoke(&sm)),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline builder.For (sequence : seq<'T>, __expand_body : 'T -> TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        builder.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> builder.While((fun () -> e.MoveNext()), TaskCode<_,_>(fun sm -> (__expand_body e.Current).Invoke(&sm)))))

    [<NoDynamicInvocation>]
    member inline __.ReturnFrom (task: Task<'T>) = 
        TaskCode<_,_>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let CONT =
                __entryPoint (MachineFunc<TaskStateMachine<'T>>(fun sm -> 
                    let result = awaiter.GetResult()
                    sm.Result <- result
                    true))

            // shortcut to continue immediately
            if task.IsCompleted then
                let __stack_completed = CONT.Invoke(&sm)
                TaskStep<'T>(__stack_completed)
            else
#if ENABLED
                if __stateMachinesSupported then 
                    // If the task definition has been converted to a state machine then a continuation label is used
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else
                    // If the task definition has not been converted to a state machine then a continuation function is used
                    sm.ResumptionFunc <- CONT
#else
                sm.ResumptionFunc <- CONT
#endif
                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                TaskStep<'T>(false))

and [<Sealed>] Witnesses() =

    interface IPriority1
    interface IPriority2
    interface IPriority3

    static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                        when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                        and ^Awaiter :> ICriticalNotifyCompletion
                                        and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                        and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
              (_priority: IPriority2, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

        TaskCode<_,_>(fun sm -> 
            // Get an awaiter from the awaitable
            let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

            let CONT = 
                __entryPoint (MachineFunc<TaskStateMachine<'TOverall>>( fun sm -> 
                    let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                    let ``__machine_step$cont`` = (__expand_continuation result).Invoke(&sm)
                    ``__machine_step$cont``.IsCompleted))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                let __stack_completed = CONT.Invoke(&sm)
                TaskStep<'TResult2>(__stack_completed)
            else
#if ENABLED
                if __stateMachinesSupported then 
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else
                    sm.ResumptionFunc <- CONT
#else
                ignore sm.ResumptionFunc
                //sm.ResumptionFunc <- CONT
#endif
                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                TaskStep<'TResult2>(false))

    static member inline CanBind (_priority: IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

        TaskCode<_,_>(fun sm -> 
            // Get an awaiter from the task
            let mutable awaiter = task.GetAwaiter()

            let CONT = 
                __entryPoint (MachineFunc<TaskStateMachine<'TOverall>>(fun sm -> 
                    let result = awaiter.GetResult()
                    let ``__machine_step$cont`` = (__expand_continuation result).Invoke(&sm)
                    ``__machine_step$cont``.IsCompleted))

            // shortcut to continue immediately
            if awaiter.IsCompleted then 
                let __stack_completed = CONT.Invoke(&sm)
                TaskStep<'TResult2>(__stack_completed)
            else
#if ENABLED
                if __stateMachinesSupported then 
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else
                    sm.ResumptionFunc <- CONT
#else
                sm.ResumptionFunc <- CONT
#endif
                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                TaskStep<'TResult2>(false))

    static member inline CanBind (_priority: IPriority1, computation: Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
        Witnesses.CanBind (_priority, Async.StartAsTask computation, __expand_continuation)

    static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                       when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                       and ^Awaiter :> ICriticalNotifyCompletion
                                       and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                       and ^Awaiter: (member GetResult: unit ->  ^T)>
          (_priority: IPriority2, task: ^TaskLike) : TaskCode< ^T, ^T > =

        TaskCode<_,_>(fun sm -> 
            // Get an awaiter from the awaitable
            let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

            let CONT =
                __entryPoint (MachineFunc<TaskStateMachine< ^T >>(fun sm -> 
                    let result = (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                    sm.Result <- result
                    true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                let __stack_completed = CONT.Invoke(&sm)
                TaskStep< ^T > (__stack_completed)
            else
#if ENABLED
                if __stateMachinesSupported then 
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else
                    sm.ResumptionFunc <- CONT
#else
                sm.ResumptionFunc <- CONT
#endif
                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                TaskStep<'T>(false))

    static member inline CanReturnFrom (_priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

        TaskCode<_,_>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let CONT =
                __entryPoint (MachineFunc<TaskStateMachine<'T>>(fun sm -> 
                    let result = awaiter.GetResult()
                    sm.Result <- result
                    true))

            // shortcut to continue immediately
            if task.IsCompleted then
                let __stack_completed = CONT.Invoke(&sm)
                TaskStep<'T>(__stack_completed)
            else
#if ENABLED
                if __stateMachinesSupported then 
                    // If the task definition has been converted to a state machine then a continuation label is used
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else
                    // If the task definition has not been converted to a state machine then a continuation function is used
                    sm.ResumptionFunc <- CONT
#else
                sm.ResumptionFunc <- CONT
#endif
                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                TaskStep<'T>(false))

    static member inline CanReturnFrom (_priority: IPriority1, computation: Async<'T>) : TaskCode<'T, 'T> =
        Witnesses.CanReturnFrom (_priority, Async.StartAsTask computation)

[<AutoOpen>]
module TaskHelpers = 

    let task = TaskBuilder()

    type TaskBuilder with

        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskCode<'TOverall, 'TResult2>) : TaskCode<'TOverall, 'TResult2> =

            RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> task __expand_continuation

        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskCode<'T, 'T>) > 
                    (task: ^TaskLike) : TaskCode<'T, 'T> =

            RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> task

(*

module ContextInsensitiveTasks =

    let task = TaskBuilder()

    [<Sealed; NoComparison; NoEquality>]
    type Witnesses() = 
        interface IPriority1
        interface IPriority2
        interface IPriority3

        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> 
                     (_priority: IPriority3, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            // get an awaiter from the task
            let mutable awaiter = (^TaskLike : (member GetAwaiter : unit -> ^Awaiter)(task))
            let CONT = __entryPoint (fun () -> __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))) 
            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> 
                     (_priority: IPriority2, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable))
            let CONT = __entryPoint (fun () -> __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))) 
            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority :IPriority1, sm: TaskStateMachine<'TOverall>, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __entryPoint (fun () -> __expand_continuation (awaiter.GetResult()))
            if awaiter.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority: IPriority1, sm: TaskStateMachine<'TOverall>, computation : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            Witnesses.CanBind (_priority, sm, Async.StartAsTask computation, __expand_continuation)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) >
               (_priority: IPriority3, sm: TaskStateMachine< ^T >, task: ^Awaitable) =

            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(task))
            let CONT = __entryPoint (fun () -> sm.SetResult (^Awaiter : (member GetResult : unit -> ^T)(awaiter)); TaskStep<'T>(true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep< ^T >(false)
        
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^T ) > (_: IPriority2, sm: TaskStateMachine< ^T >, task: ^TaskLike) =

            let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable))
            let CONT = __entryPoint (fun () -> sm.SetResult (^Awaiter : (member GetResult : unit -> ^T)(awaiter)); TaskStep<'T>(true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep< ^T >(false)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, sm: TaskStateMachine<'T>, task: Task<'T>) : TaskStep<'T> =
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __entryPoint (fun () -> sm.SetResult (awaiter.GetResult()); TaskStep<'T>(true))
            if task.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'T>(false)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, sm: TaskStateMachine<'T>, computation: Async<'T>) =
            Witnesses.CanReturnFrom (_priority, sm, Async.StartAsTask computation)

    type TaskStateMachine<'TOverall> with
        [<NoDynamicInvocation>]
        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * TaskStateMachine<'TOverall> * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
            RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> sm task __expand_continuation

        [<NoDynamicInvocation>]
        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * TaskStateMachine<'T> * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> sm task
*)

#endif
