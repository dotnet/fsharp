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
        let __newEntryPoint() : int = failwith "__newEntryPoint should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __machine<'T> : 'T = failwith "__machine should always be removed from compiled code"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __entryPoint (_n: int) : unit = failwith "__entryPoint should always be removed from compiled code" 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __return<'T> (_v: 'T) : 'T = failwith "__return should always be removed from compiled code"

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
type TaskStep<'T>(completed: bool) = 
    member x.IsCompleted = completed

[<AbstractClass>]
type TaskStateMachine() =
    member val Current : obj = null with get, set

    /// Await the given awaiter and resume at the given entry point
    abstract Await: awaiter : ICriticalNotifyCompletion * pc: int -> unit 

[<AbstractClass>]
type TaskStateMachine<'T>() =
    inherit TaskStateMachine()
    let mutable resumptionPoint = 0 

    let mutable methodBuilder = AsyncTaskMethodBuilder<Task<'T>>()
    
    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> TaskStep<'T>

    [<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
    override sm.Await (awaiter, pc) = 
        resumptionPoint <- pc
        let mutable sm = sm
        let mutable awaiter = awaiter
        assert (not (isNull awaiter))
        // Tell the builder to call us again when done.
        //Console.WriteLine("[{0}] AwaitUnsafeOnCompleted", sm.GetHashCode())
        methodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)

    interface IAsyncStateMachine with

        [<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
        member this.MoveNext() =
            try
                //Console.WriteLine("[{0}] step from {1}", this.GetHashCode(), resumptionPoint)
                let step = this.Step resumptionPoint
                if step.IsCompleted then 
                    //Console.WriteLine("[{0}] unboxing result", this.GetHashCode())
                    let res = unbox<'T>(this.Current)
                    //Console.WriteLine("[{0}] SetResult {1}", this.GetHashCode(), res)
                    methodBuilder.SetResult(Task.FromResult res)
            with exn ->
              //Console.WriteLine("[{0}] exception {1}", this.GetHashCode(), exn)
                methodBuilder.SetException exn

        member __.SetStateMachine(_) = () // Doesn't really apply since we're a reference type.

    [<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
    member this.Start() =
        let mutable machine = this 
        try
          //Console.WriteLine("[{0}] start", this.GetHashCode())
            methodBuilder.Start(&machine)
          //Console.WriteLine("[{0}] unwrap", this.GetHashCode())
            methodBuilder.Task.Unwrap()
        with exn ->
          //Console.WriteLine("[{0}] start exception", this.GetHashCode())
            // Any exceptions should go on the task, rather than being thrown from this call.
            // This matches C# behavior where you won't see an exception until awaiting the task,
            // even if it failed before reaching the first "await".
            let src = new TaskCompletionSource<_>()
            src.SetException exn
            src.Task


[<AutoOpen>]
module TaskHelpers = 

    //let inline unwrapException (agg : AggregateException) =
    //    let inners = agg.InnerExceptions
    //    if inners.Count = 1 then inners.[0]
    //    else agg :> Exception

    /// Used to return a value.
    let inline ret<'T> (x : 'T) = 
        __machine<TaskStateMachine>.Current <- (box x)
        TaskStep<'T>(true)

    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2 when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) > (x: ^Priority) (y: ^TaskLike) __expand_continuation = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) (x, y, __expand_continuation))

    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskStep<'T>)> (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskStep<'T>) (x, y))

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

        static member inline GenericAwait< ^Awaitable, ^Awaiter, ^TResult1
                                            when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion 
                                            and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                            and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (awaitable : ^Awaitable, __expand_continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable)) // get an awaiter from the awaitable
                let CONT = __newEntryPoint () 
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                    __entryPoint CONT
                    __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                else
                    __machine<TaskStateMachine>.Await (awaiter, CONT)
                    __return (TaskStep<'TResult2>(false))

        static member inline GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (task : ^TaskLike, __expand_continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
                TaskLikeBind<'TResult2>.GenericAwait(awaitable, __expand_continuation)

    /// Special case of the above for `Task<'TResult1>`. Have to write this T by hand to avoid confusing the compiler
    /// trying to decide between satisfying the constraints with `Task` or `Task<'TResult1>`.
    let inline bindTask (task : Task<'TResult1>) (__expand_continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let CONT = __newEntryPoint()
        let awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then 
            __entryPoint CONT
            __expand_continuation (awaiter.GetResult())
        else
            __machine<TaskStateMachine>.Await (awaiter, CONT)
            __return (TaskStep<'TResult2>(false))

    /// Special case of the above for `Task<'TResult1>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    let inline bindTaskConfigureFalse (task : Task<'TResult1>) (__expand_continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let CONT = __newEntryPoint ()
        let awaiter = task.ConfigureAwait(false).GetAwaiter()
        if awaiter.IsCompleted then
            __entryPoint CONT
            __expand_continuation (awaiter.GetResult())
        else
            __machine<TaskStateMachine>.Await (awaiter, CONT)
            __return (TaskStep<'TResult2>(false))

// New style task builder.
type TaskBuilder() =
    
    member inline __.Delay(__expand_f : unit -> TaskStep<'T>) = __expand_f

    member inline __.Run(__expand_code : unit -> TaskStep<'T>) : Task<'T> = 
        (__stateMachine
            { new TaskStateMachine<'T>() with 
                member __.Step pc = __jumptable pc __expand_code }).Start()

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    member inline __.Zero() : TaskStep<unit> =
        __machine<TaskStateMachine>.Current <- (box ())
        TaskStep<unit>(true)

    member inline __.Return (x: 'T) : TaskStep<'T> =
        __machine<TaskStateMachine>.Current <- (box x)
        TaskStep<'T>(true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline __.Combine(``__machine_step$cont``: TaskStep<unit>, __expand_task2: unit -> TaskStep<'T>) : TaskStep<'T> =
        if ``__machine_step$cont``.IsCompleted then 
            __expand_task2()
        else
            TaskStep<'T>(``__machine_step$cont``.IsCompleted)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> TaskStep<unit>) : TaskStep<unit> =
        let mutable completed = true 
        while completed && __expand_condition() do
            completed <- false 
            // The body of the 'while' may include an early exit, e.g. return from entire method
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step 
            completed <- ``__machine_step$cont``.IsCompleted
        __machine<TaskStateMachine>.Current <- (box ())
        TaskStep<unit>(completed)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryWith(__expand_body : unit -> TaskStep<'T>, __expand_catch : exn -> TaskStep<'T>) : TaskStep<'T> =
        let mutable completed = TaskStep<'T>(false)
        let mutable caught = false
        let mutable savedExn = Unchecked.defaultof<_>
        try
            // The try block may contain resumption points.
            // This is handled by the state machine rewriting 
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            completed <- ``__machine_step$cont``
        with exn -> 
            // The catch block may not contain resumption points.
            caught <- true
            savedExn <- exn

        if caught then 
            // Place the catch code outside the catch block 
            __expand_catch savedExn
        else
            completed

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryFinally(__expand_body: unit -> TaskStep<'T>, compensation : unit -> unit) : TaskStep<'T> =
        let mutable completed = TaskStep<'T>(false)
        try
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            completed <- ``__machine_step$cont``
        with _ ->
            compensation()
            reraise()

        if completed.IsCompleted then 
            compensation()
        completed

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> TaskStep<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, __expand_body : 'T -> TaskStep<unit>) : TaskStep<unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    member inline __.ReturnFrom (task: Task<'T>) : TaskStep<'T> =
        let CONT = __newEntryPoint ()
        if task.IsCompleted then
            __entryPoint CONT
            __machine<TaskStateMachine>.Current <- box (task.GetAwaiter().GetResult())
            TaskStep<'T>(true)
        else
            __machine<TaskStateMachine>.Await(task.GetAwaiter(), CONT)
            __return (TaskStep<'T>(false))

[<AutoOpen>]
module ContextSensitiveTasks =

    let task = TaskBuilder()

    [<Sealed>]
    type Witnesses() =

        interface IPriority1
        interface IPriority2
        interface IPriority3

        // Give the type arguments explicitly to make it match the signature precisely
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>(_priority: IPriority2, taskLike : ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
                  = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (taskLike, __expand_continuation)

        static member inline CanBind (_priority: IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
                  = bindTask task __expand_continuation                      

        static member inline CanBind (_priority: IPriority1, computation  : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> 
                  = bindTask (Async.StartAsTask computation) __expand_continuation     

        // Give the type arguments explicitly to make it match the signature precisely
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)> 
              (_priority: IPriority1, taskLike: ^TaskLike) : TaskStep< ^T > 
                  = TaskLikeBind< ^T >.GenericAwait< ^TaskLike, ^Awaiter, ^T> (taskLike, ret< ^T >)

        static member inline CanReturnFrom (_priority: IPriority1, computation : Async<'T>) 
                  = bindTask (Async.StartAsTask computation) (ret< 'T >) : TaskStep<'T>

    type TaskBuilder with
        member inline builder.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> Unchecked.defaultof<Witnesses> task __expand_continuation

        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> task

module ContextInsensitiveTasks =

    let task = TaskBuilder()

    [<Sealed; NoComparison; NoEquality>]
    type Witnesses() = 
        interface IPriority1
        interface IPriority2
        interface IPriority3

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> (_priority: IPriority3, taskLike: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (taskLike, __expand_continuation)

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter 
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> (_priority: IPriority2, configurableTaskLike: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = TaskLikeBind<'TResult2>.GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1> (configurableTaskLike, __expand_continuation)

        static member inline CanBind (_priority :IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = bindTaskConfigureFalse task __expand_continuation

        static member inline CanBind (_priority: IPriority1, computation : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = bindTaskConfigureFalse (Async.StartAsTask computation) __expand_continuation

(*
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) > (_priority: IPriority2, taskLike: ^Awaitable) 
                            = TaskLikeBind< ^T >.GenericAwait< ^Awaitable, ^Awaiter, ^T >(taskLike, ret)
        
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) > (_: IPriority1, configurableTaskLike: ^TaskLike)
                            = TaskLikeBind< ^TResult1 >.GenericAwaitConfigureFalse(configurableTaskLike, ret)


        static member inline CanReturnFrom (_priority: IPriority1, computation: Async<'T>) 
                            = bindTaskConfigureFalse sm (Async.StartAsTask computation) ret
*)

    type TaskBuilder with
        member inline builder.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> Unchecked.defaultof<Witnesses> task __expand_continuation
(*
        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> builder.TaskStateMachine Unchecked.defaultof<Witnesses> task
*)
#endif
