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

    module CodeGenHelpers = 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __jumptable<'T> (_x:int) (_code: unit -> 'T)  : 'T = failwith "__jumptable should always be removed from compiled code"
        
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __stateMachine<'T> (_x: 'T) : 'T = failwith "__stateMachine should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        // The template is written to a new struct type.  Any mention of the template in any of the code is rewritten to that
        // new struct type.  Meth1 and Meth2 are used to implement the methods on the interface implemented by the struct type.
        let __stateMachineStruct<'Template, 'Meth1, 'Meth2, 'Result> (_meth1: 'Meth1) (_meth2: 'Meth2) (_after: unit -> 'Result): 'Result = failwith "__stateMachineStruct should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __newEntryPoint() : int = failwith "__newEntryPoint should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __machine<'T> : 'T = failwith "__machine should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __machineAddr<'T> : byref<'T> = (# "ldnull throw" : byref<'T> #)

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __entryPoint (_n: int) : unit = failwith "__entryPoint should always be removed from compiled code" 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __return<'T> (_v: 'T) : 'T = failwith "__return should always be removed from compiled code"
#endif

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.CodeGenHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the state of a computation: either awaiting something with a
/// continuation, or completed with a return value.
//
// Uses a struct-around-single-reference to allow future changes in representation (the representation is
// not revealed in the signature)
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T, 'TOverall>(completed: bool) = 
    member x.IsCompleted = completed

[<Struct; NoComparison; NoEquality>]
type TaskStateMachineTemplate<'T> =

    [<DefaultValue(false)>]
    val mutable Result : 'T

    [<DefaultValue>]
    val mutable ResumptionPoint : int

    [<DefaultValue>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'T>

    interface IAsyncStateMachine with 
        member this.MoveNext() = failwith "template"
        member this.SetStateMachine(_machine) = failwith "template"

module Helpers =
    [<NoDynamicInvocation>]
    let inline Start (sm: byref<TaskStateMachineTemplate<'T>>) = 
        //Console.WriteLine("[{0}] start", sm.GetHashCode())
        sm.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
        sm.MethodBuilder.Start(&sm)
        //Console.WriteLine("[{0}] unwrap", sm.GetHashCode())
        sm.MethodBuilder.Task

    [<NoDynamicInvocation>]
    let inline Await (sm: byref<TaskStateMachineTemplate<'T>>) (awaiter: byref<('Awaiter :> ICriticalNotifyCompletion)>, pc) =
        sm.ResumptionPoint <- pc
        //assert (not (isNull awaiter))
        // Tell the builder to call us again when done.
        //Console.WriteLine("[{0}] AwaitUnsafeOnCompleted", sm.GetHashCode())
        sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)

    [<NoDynamicInvocation>]
    let inline SetResult (sm: byref<TaskStateMachineTemplate<'T>>) (v: 'T) = 
        sm.Result <- v

[<AutoOpen>]
module TaskHelpers = 

    //let inline unwrapException (agg : AggregateException) =
    //    let inners = agg.InnerExceptions
    //    if inners.Count = 1 then inners.[0]
    //    else agg :> Exception

    /// Used to return a value.
    [<NoDynamicInvocation>]
    let inline ret<'T> (x : 'T) = 
        Helpers.SetResult __machineAddr<TaskStateMachineTemplate<'T>> x
        TaskStep<'T, 'T>(true)

    [<NoDynamicInvocation>]
    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2, 'TOverall when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>) > (x: ^Priority) (y: ^TaskLike) __expand_continuation = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>) (x, y, __expand_continuation))

    [<NoDynamicInvocation>]
    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskStep<'T, 'T>)> (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskStep<'T, 'T>) (x, y))

    type TaskLikeBind<'TResult2> =
        // We put the output generic parameter up here at the class level, so it doesn't get subject to
        // inline rules. If we put it all in the inline function, then the compiler gets confused at the
        // below and demands that the whole function either is limited to working with (x : obj), or must
        // be inline itself.
        //
        // let yieldThenReturn (x : 'TResult2) =
        //     task {
        //         do! Task.Yield()
        //         return x
        //     }

        [<NoDynamicInvocation>]
        static member inline GenericAwait< ^Awaitable, ^Awaiter, ^TResult1, 'TOverall
                                            when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion 
                                            and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                            and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (awaitable : ^Awaitable, __expand_continuation : ^TResult1 -> TaskStep<'TResult2, 'TOverall>) : TaskStep<'TResult2, 'TOverall> =
                let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable)) // get an awaiter from the awaitable
                let CONT = __newEntryPoint () 
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                    __entryPoint CONT
                    __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                else
                    Helpers.Await __machineAddr<TaskStateMachineTemplate<'TOverall>> (&awaiter, CONT)
                    TaskStep<'TResult2, 'TOverall>(false)

        [<NoDynamicInvocation>]
        static member inline GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1, 'TOverall
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (task : ^TaskLike, __expand_continuation : ^TResult1 -> TaskStep<'TResult2, 'TOverall>) : TaskStep<'TResult2, 'TOverall> =
                let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
                TaskLikeBind<'TResult2>.GenericAwait< ^Awaitable, ^Awaiter, ^TResult1, 'TOverall>(awaitable, __expand_continuation)

    /// Special case of the above for `Task<'TResult1>`. Have to write this T by hand to avoid confusing the compiler
    /// trying to decide between satisfying the constraints with `Task` or `Task<'TResult1>`.
    [<NoDynamicInvocation>]
    let inline bindTask (task : Task<'TResult1>) (__expand_continuation : 'TResult1 -> TaskStep<'TResult2, 'TOverall>) =
        let CONT = __newEntryPoint()
        let mutable awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then 
            __entryPoint CONT
            __expand_continuation (awaiter.GetResult())
        else
            Helpers.Await __machineAddr<TaskStateMachineTemplate<'TOverall>> (&awaiter, CONT)
            TaskStep<'TResult2, 'TOverall>(false)

    /// Special case of the above for `Task<'TResult1>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    [<NoDynamicInvocation>]
    let inline bindTaskConfigureFalse (task : Task<'TResult1>) (__expand_continuation : 'TResult1 -> TaskStep<'TResult2, 'TOverall>) =
        let CONT = __newEntryPoint ()
        let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
        if awaiter.IsCompleted then
            __entryPoint CONT
            __expand_continuation (awaiter.GetResult())
        else
            Helpers.Await __machineAddr<TaskStateMachineTemplate<'TOverall>> (&awaiter, CONT)
            TaskStep<'TResult2, 'TOverall>(false)

// New style task builder.
type TaskBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> TaskStep<'T, 'TOverall>) = __expand_f

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : unit -> TaskStep<'T, 'T>) : Task<'T> = 
        __stateMachineStruct<TaskStateMachineTemplate<'T>, (unit -> unit), (IAsyncStateMachine -> unit), Task<'T>>
            // MoveNext
            (fun () -> 
                __jumptable 
                    (let v = __machineAddr<TaskStateMachineTemplate<'T>> in v.ResumptionPoint) 
                    (fun () -> 
                        try
                            //Console.WriteLine("[{0}] step from {1}", sm.GetHashCode(), resumptionPoint)
                            let ``__machine_step$cont`` = __expand_code()
                            if ``__machine_step$cont``.IsCompleted then 
                                let v = __machineAddr<TaskStateMachineTemplate<'T>>
                                //Console.WriteLine("[{0}] SetResult {1}", sm.GetHashCode(), res)
                                v.MethodBuilder.SetResult(v.Result)
                        with exn ->
                            //Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
                            let v = __machineAddr<TaskStateMachineTemplate<'T>>
                            v.MethodBuilder.SetException exn))
            // SetStateMachine
            (fun machine -> 
                let v = __machineAddr<TaskStateMachineTemplate<'T>>
                v.MethodBuilder.SetStateMachine(machine))
            // Start
            (fun () -> Helpers.Start __machineAddr<TaskStateMachineTemplate<'T>>)
        

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<NoDynamicInvocation; DefaultValue>]
    member inline __.Zero() : TaskStep<unit, 'TOverall> = TaskStep<unit, 'TOverall>(true)

    [<NoDynamicInvocation>]
    member inline __.Return (x: 'T) : TaskStep<'T, 'T> = ret x

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    [<NoDynamicInvocation>]
    member inline __.Combine(``__machine_step$cont``: TaskStep<unit, 'TOverall>, __expand_task2: unit -> TaskStep<'T, 'TOverall>) : TaskStep<'T, 'TOverall> =
        if ``__machine_step$cont``.IsCompleted then 
            __expand_task2()
        else
            TaskStep<'T, 'TOverall>(``__machine_step$cont``.IsCompleted)

    /// Builds a step that executes the body while the condition predicate is true.
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> TaskStep<unit, 'TOverall>) : TaskStep<unit, 'TOverall> =
        let mutable __stack_completed = true 
        while __stack_completed && __expand_condition() do
            __stack_completed <- false 
            // The body of the 'while' may include an early exit, e.g. return from entire method
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step 
            __stack_completed <- ``__machine_step$cont``.IsCompleted
        TaskStep<unit, 'TOverall>(__stack_completed)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : unit -> TaskStep<'T, 'TOverall>, __expand_catch : exn -> TaskStep<'T, 'TOverall>) : TaskStep<'T, 'TOverall> =
        let mutable __stack_completed = false
        let mutable __stack_caught = false
        let mutable __stack_savedExn = Unchecked.defaultof<_>
        try
            // The try block may contain resumption points.
            // This is handled by the state machine rewriting 
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            __stack_completed <- ``__machine_step$cont``.IsCompleted
        with exn -> 
            // The catch block may not contain resumption points.
            __stack_caught <- true
            __stack_savedExn <- exn

        if __stack_caught then 
            // Place the catch code outside the catch block 
            __expand_catch __stack_savedExn
        else
            TaskStep<'T, 'TOverall>(__stack_completed)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    [<NoDynamicInvocation>]
    member inline __.TryFinally(__expand_body: unit -> TaskStep<'T, 'TOverall>, compensation : unit -> unit) : TaskStep<'T, 'TOverall> =
        let mutable __stack_completed = false
        try
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            __stack_completed <- ``__machine_step$cont``.IsCompleted
        with _ ->
            compensation()
            reraise()

        if __stack_completed then 
            compensation()
        TaskStep<'T, 'TOverall>(__stack_completed)

    [<NoDynamicInvocation>]
    member inline builder.Using(disp : #IDisposable, __expand_body : #IDisposable -> TaskStep<'T, 'TOverall>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        builder.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline builder.For(sequence : seq<'T>, __expand_body : 'T -> TaskStep<unit, 'TOverall>) : TaskStep<unit, 'TOverall> =
        // A for loop is just a using statement on the sequence's enumerator...
        builder.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> builder.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    [<NoDynamicInvocation>]
    member inline __.ReturnFrom (task: Task<'T>) : TaskStep<'T, 'T> =
        let CONT = __newEntryPoint ()
        let mutable awaiter = task.GetAwaiter()
        if task.IsCompleted then
            __entryPoint CONT
            ret (awaiter.GetResult())
        else
            Helpers.Await __machineAddr<TaskStateMachineTemplate<'T>> (&awaiter, CONT)
            TaskStep<'T, 'T>(false)

[<AutoOpen>]
module ContextSensitiveTasks =

    let task = TaskBuilder()

    [<Sealed>]
    type Witnesses() =

        interface IPriority1
        interface IPriority2
        interface IPriority3

        // Give the type arguments explicitly to make it match the signature precisely
        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>(_priority: IPriority2, taskLike : ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
                  = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1, 'TOverall> (taskLike, __expand_continuation)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority: IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
                  = bindTask task __expand_continuation                      

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority: IPriority1, computation  : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall> 
                  = bindTask (Async.StartAsTask computation) __expand_continuation     

        // Give the type arguments explicitly to make it match the signature precisely
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)> 
              (_priority: IPriority1, taskLike: ^TaskLike) : TaskStep< ^T, ^T > 
                  = TaskLikeBind< ^T >.GenericAwait< ^TaskLike, ^Awaiter, ^T, ^T > (taskLike, ret< ^T >)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, computation : Async<'T>) 
                  = bindTask (Async.StartAsTask computation) (ret<'T>) : TaskStep<'T, 'T>

    type TaskBuilder with
        [<NoDynamicInvocation>]
        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2 , 'TOverall
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2, 'TOverall>) : TaskStep<'TResult2, 'TOverall>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> task __expand_continuation

        [<NoDynamicInvocation>]
        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T, 'T>) > (task: ^TaskLike) : TaskStep<'T, 'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> task

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
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> (_priority: IPriority3, taskLike: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
              = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1, 'TOverall> (taskLike, __expand_continuation)

        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> (_priority: IPriority2, configurableTaskLike: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
              = TaskLikeBind<'TResult2>.GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1, 'TOverall> (configurableTaskLike, __expand_continuation)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority :IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
              = bindTaskConfigureFalse task __expand_continuation

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority: IPriority1, computation : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2, 'TOverall>)) : TaskStep<'TResult2, 'TOverall>
              = bindTaskConfigureFalse (Async.StartAsTask computation) __expand_continuation

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) > (_priority: IPriority2, taskLike: ^Awaitable) 
                            = TaskLikeBind< ^T >.GenericAwait< ^Awaitable, ^Awaiter, ^T, ^T >(taskLike, ret< ^T > )
        
        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^T ) > (_: IPriority1, configurableTaskLike: ^TaskLike)
                            = TaskLikeBind< ^T >.GenericAwaitConfigureFalse(configurableTaskLike, ret< ^T >)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, computation: Async<'T>) 
                            = bindTaskConfigureFalse (Async.StartAsTask computation) ret

    type TaskBuilder with
        [<NoDynamicInvocation>]
        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2 , 'TOverall
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2, 'TOverall>) -> TaskStep<'TResult2, 'TOverall>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2, 'TOverall>) : TaskStep<'TResult2, 'TOverall>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> task __expand_continuation

        [<NoDynamicInvocation>]
        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T, 'T>) > (task: ^TaskLike) : TaskStep<'T, 'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> task
#endif
