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
        let __jumptable<'T> (_x:int) (_code: 'T)  : 'T = Unchecked.defaultof<_> 
        
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __stateMachine<'T> (x: 'T) : 'T = x

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __newLabel() : int = 0

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __newEntryPoint() : int = 0

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __code<'T> (_f: unit -> 'T) : 'T = Unchecked.defaultof<_> 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __label (_n: int) : unit = Unchecked.defaultof<_> 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __return (_v: 'T) : 'U = Unchecked.defaultof<_> 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __entryPoint<'T> (_n: int) (_f: unit -> 'T) : 'T = Unchecked.defaultof<_> 

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __goto<'T> (_n: int) : 'T = Unchecked.defaultof<_> 

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
    member __.IsCompleted = completed

[<AbstractClass>]
type TaskStateMachine() =
    member val ResumptionPoint : int = 0 with get, set
    member val Current : obj = null with get, set

    /// Await the given awaiter and resume at the given entry point
    abstract Await: awaiter : ICriticalNotifyCompletion * pc: int -> unit 

[<AbstractClass>]
type TaskStateMachine<'T>() =
    inherit TaskStateMachine()

    let mutable methodBuilder = AsyncTaskMethodBuilder<Task<'T>>()
    
    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> TaskStep<'T>

    override sm.Await(awaiter, pc) = 
        sm.ResumptionPoint <- pc
        let mutable sm = sm
        let mutable awaiter = awaiter
        assert (not (isNull awaiter))
        // Tell the builder to call us again when done.
        methodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)

    interface IAsyncStateMachine with

        member this.MoveNext() =
            try
                let step = this.Step this.ResumptionPoint
                if step.IsCompleted then 
                    let res = unbox<'T>(this.Current)
                    methodBuilder.SetResult(Task.FromResult res)
            with exn ->
                methodBuilder.SetException exn

        member __.SetStateMachine(_) = () // Doesn't really apply since we're a reference type.

    member this.Start() =
        let mutable machine = (this :> IAsyncStateMachine)
        try
            methodBuilder.Start(&machine)
            methodBuilder.Task.Unwrap()
        with exn ->
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
    let inline ret<'T> (sm: TaskStateMachine) (x : 'T) = 
        sm.Current <- (box x)
        TaskStep<'T>(true)

    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2 when (^Priority or ^TaskLike): (static member CanBind : TaskStateMachine * ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) > (sm: TaskStateMachine) (x: ^Priority) (y: ^TaskLike) k = 
        ((^Priority or ^TaskLike): (static member CanBind : TaskStateMachine * ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) (sm, x, y, k))

    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: TaskStateMachine * ^Priority * ^TaskLike -> TaskStep<'T>)> (sm: TaskStateMachine) (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : TaskStateMachine * ^Priority * ^TaskLike -> TaskStep<'T>) (sm, x, y))

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
            (sm: TaskStateMachine, awaitable : ^Awaitable, continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable)) // get an awaiter from the awaitable
                let CONT = __newEntryPoint () 
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                    __entryPoint<TaskStep<'TResult2>> CONT (fun () -> 
                        continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter)))
                else
                    sm.Await(awaiter, CONT)
                    TaskStep<'TResult2>(false)

        static member inline GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (sm, task : ^TaskLike, continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
                TaskLikeBind<'TResult2>.GenericAwait(sm, awaitable, continuation)

    /// Special case of the above for `Task<'TResult1>`. Have to write this T by hand to avoid confusing the compiler
    /// trying to decide between satisfying the constraints with `Task` or `Task<'TResult1>`.
    let inline bindTask (sm: TaskStateMachine) (task : Task<'TResult1>) (continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let CONT = __newEntryPoint()
        let awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then 
            __entryPoint<TaskStep<'TResult2>> CONT (fun () -> 
                continuation (awaiter.GetResult())
            )
        else
            sm.Await(awaiter, CONT)
            TaskStep<'TResult2>(false)

    /// Special case of the above for `Task<'TResult1>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    let inline bindTaskConfigureFalse (sm: TaskStateMachine) (task : Task<'TResult1>) (continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let CONT = __newEntryPoint ()
        let awaiter = task.ConfigureAwait(false).GetAwaiter()
        if awaiter.IsCompleted then
            __entryPoint<TaskStep<'TResult2>> CONT (fun () -> 
                continuation (awaiter.GetResult())
            )
        else
            sm.Await(awaiter, CONT)
            TaskStep<'TResult2>(false)

type TaskSpec<'T> = TaskStateMachine -> TaskStep<'T>

// New style task builder.
type TaskBuilder() =
    // These methods are consistent between all builders.
    member inline __.Delay(f : unit -> TaskSpec<'T>) = (fun sm -> f () sm)

    member inline __.Run(code : TaskSpec<'T>) = 
        let sm = 
            __stateMachine
                { new TaskStateMachine<'T>() with 
                    member sm.Step(pc) = __jumptable pc (code sm) }
        sm.Start()

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    member inline __.Zero() : TaskSpec<unit> = (fun sm -> 
        sm.Current <- (box ())
        TaskStep<unit>(true))

    member inline __.Return (x: 'T) : TaskSpec<'T> = (fun sm -> 
        sm.Current <- (box x)
        TaskStep<'T>(true))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline __.Combine(task1: TaskSpec<unit>, task2: TaskSpec<'T>) : TaskSpec<'T> = (fun sm -> 
        let step = task1 sm
        if step.IsCompleted then 
            task2 sm
        else 
            TaskStep<'T>(false))

    /// Builds a step that executes the body while the condition predicate is true.
    member inline __.While(condition : unit -> bool, body : TaskSpec<unit>) : TaskSpec<unit> = (fun sm -> 
        let ENTRY = __newLabel()
        __label ENTRY
        let guard = __code condition
        if guard then
            let step = __code (fun () -> body sm)
            if step.IsCompleted then 
                __goto<TaskStep<unit>> ENTRY
            else
                TaskStep<unit>(false)
        else
            sm.Current <- (box ())
            TaskStep<unit>(true))

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryWith(body : TaskSpec<'T>, catch : exn -> TaskSpec<'T>) : TaskSpec<'T> = (fun sm -> 
        try
            let CODE = __newLabel()
            __label CODE
            __code<TaskStep<'T>> (fun () -> body sm)
        with exn -> 
            catch exn sm)

    member inline __.TryFinally(body : TaskSpec<'T>, compensation : unit -> unit) = (fun sm -> 
        /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
        /// to retrieve the step, and in the continuation of the step (if any).
        // codegen
        let step =
            try
                let CODE = __newLabel()
                __label CODE
                __code<TaskStep<'T>> (fun () -> body sm)
            with _ ->
                compensation()
                reraise()

        if step.IsCompleted then 
            compensation()
        step)

    member inline this.Using(disp : #IDisposable, body : #IDisposable -> TaskSpec<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, body : 'T -> TaskSpec<unit>) : TaskSpec<unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), this.Delay (fun () -> body e.Current))))

    member inline __.ReturnFrom (task: Task<'T>) : TaskSpec<'T> = (fun sm -> 
        let CONT = __newEntryPoint ()
        if task.IsCompleted then
            __entryPoint CONT (fun () -> 
                sm.Current <- (box task.Result)
                TaskStep<'T>(true))
        else
            sm.Await(task.GetAwaiter(), CONT)
            sm.ResumptionPoint <- CONT
            TaskStep<'T>(false))


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
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>(sm, _priority: IPriority2, taskLike : ^TaskLike, k: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
                  = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (sm, taskLike, k)

        static member inline CanBind (sm, _priority: IPriority1, task: Task<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
                  = bindTask sm task k                      

        static member inline CanBind (sm, _priority: IPriority1, computation  : Async<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> 
                  = bindTask sm (Async.StartAsTask computation) k     

        // Give the type arguments explicitly to make it match the signature precisely
        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)> 
              (sm, _priority: IPriority1, taskLike: ^TaskLike) : TaskStep< ^T > 
                  = TaskLikeBind< ^T >.GenericAwait< ^TaskLike, ^Awaiter, ^T> (sm, taskLike, ret< ^T > sm)

        static member inline CanReturnFrom (sm, _priority: IPriority1, computation : Async<'T>) 
                  = bindTask sm (Async.StartAsTask computation) (ret< 'T > sm) : TaskStep<'T>

    type TaskBuilder with
        member inline builder.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: TaskStateMachine * Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, continuation: ^TResult1 -> TaskSpec<'TResult2>) : TaskSpec<'TResult2>
                  = (fun sm -> RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> sm Unchecked.defaultof<Witnesses> task (fun x -> continuation x sm))

        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: TaskStateMachine * Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskSpec<'T> 
                  = (fun sm -> RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> sm Unchecked.defaultof<Witnesses> task)


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
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> (sm, _priority: IPriority3, taskLike: ^TaskLike, k: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (sm, taskLike, k)

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter 
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> (sm, _priority: IPriority2, configurableTaskLike: ^TaskLike, k: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = TaskLikeBind<'TResult2>.GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1> (sm, configurableTaskLike, k)

        static member inline CanBind (sm, _priority :IPriority1, task: Task<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = bindTaskConfigureFalse sm task k

        static member inline CanBind (sm, _priority: IPriority1, computation : Async<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2>
              = bindTaskConfigureFalse sm (Async.StartAsTask computation) k

(*
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) > (sm, _priority: IPriority2, taskLike: ^Awaitable) 
                            = TaskLikeBind< ^T >.GenericAwait< ^Awaitable, ^Awaiter, ^T >(sm, taskLike, ret)
        
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) > (sm, _: IPriority1, configurableTaskLike: ^TaskLike)
                            = TaskLikeBind< ^TResult1 >.GenericAwaitConfigureFalse(sm, configurableTaskLike, ret)


        static member inline CanReturnFrom (sm, _priority: IPriority1, computation: Async<'T>) 
                            = bindTaskConfigureFalse sm (Async.StartAsTask computation) ret
*)

    type TaskBuilder with
        member inline builder.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: TaskStateMachine * Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskSpec<'TResult2>
                  = (fun sm -> RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> sm Unchecked.defaultof<Witnesses> task continuation)
(*
        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: TaskStateMachine * Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> builder.TaskStateMachine Unchecked.defaultof<Witnesses> task
*)
#endif
