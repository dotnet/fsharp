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
    /// A marker interface to give priority to different available overloads
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads
    type IPriority1 = interface inherit IPriority2 end

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// Represents the state of a computation: either awaiting something with a
/// continuation, or completed with a return value.
//
// Uses a struct-around-single-reference to allow future changes in representation (the representation is
// not revealed in the signature)
[<Struct; NoComparison; NoEquality>]
type TaskStep<'T>(action: int, data: obj) =
    static member Return (x: 'T) = TaskStep<'T>(0, box x)
    static member ReturnFrom (task: Task<'T>) = TaskStep<'T>(1, box task)
    static member Await (completion:  ICriticalNotifyCompletion) = TaskStep<'T>(2, box completion)
    member __.IsReturn = (action = 0)
    member __.IsReturnFrom = (action = 1)
    member __.IsAwait = (action = 2)
    member __.GetAwaitable() = (data :?> ICriticalNotifyCompletion) 
    member __.GetNextTask() = (data :?> Task<'T>) 
    member __.GetResult() = (data :?> 'T) 

    //| Return of 'T
    //| ReturnFrom of Task<'T>
    //| Await of ICriticalNotifyCompletion * (unit -> TaskStep<'T>)

[<AutoOpen>]
module TaskHelpers = 
    //let __jumptable (_x:int) = ()

    type SM() =
        let mutable conts = ResizeArray<(unit -> obj)>()
        let mutable pc = 0
        member __.__genlabel() = 
            let v = conts.Count
            conts.Add(Unchecked.defaultof<_>)
            v

        member __.__gencode n (f: unit -> TaskStep<'T>) = 
            conts.[n] <- (f >> box)

        member sm.__code (f: unit -> TaskStep<'T>) =
            let n = sm.__genlabel ()
            sm.__gencode n f
            n

        member __.__jmp<'T> n = 
            match conts.[n]() with 
            | :? TaskStep<'T> as t -> t
            | res -> 
                printfn "T = %A" typeof<'T>
                printfn "res.GetType() = %A" (res.GetType())
                failwith "invalid type"
        member __.__setpc v = pc <- v
        member __.__getpc = pc 
        //-> unit (* unit -> TasskStep<'T> *)

    //let inline unwrapException (agg : AggregateException) =
    //    let inners = agg.InnerExceptions
    //    if inners.Count = 1 then inners.[0]
    //    else agg :> Exception

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    let inline zero() = TaskStep<unit>.Return ()

    /// Used to return a value.
    let inline ret (x : 'T) = TaskStep<'T>.Return x

    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2 when (^Priority or ^TaskLike): (static member CanBind : SM * ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) > (sm: SM) (x: ^Priority) (y: ^TaskLike) k = 
        ((^Priority or ^TaskLike): (static member CanBind : SM * ^Priority * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>) (sm, x, y, k))

    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: SM * ^Priority * ^TaskLike -> TaskStep<'T>)> (sm: SM) (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : SM * ^Priority * ^TaskLike -> TaskStep<'T>) (sm, x, y))

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
            (sm: SM, awaitable : ^Awaitable, continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable)) // get an awaiter from the awaitable
                let ENTRY = sm.__genlabel () 
                let CONT = sm.__genlabel () 
                sm.__gencode ENTRY (fun () -> 
                    if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                        sm.__jmp<'TResult2> CONT
                    else
                        sm.__setpc CONT
                        TaskStep<'TResult2>.Await (awaiter))
                sm.__gencode CONT (fun () -> 
                    continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter)))
                sm.__jmp ENTRY

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
    let inline bindTask (sm: SM) (task : Task<'TResult1>) (continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let awaiter = task.GetAwaiter()
        let mutable result = Unchecked.defaultof<_>
        let ENTRY = sm.__genlabel()
        let CONT = sm.__genlabel()
        sm.__gencode ENTRY (fun () -> 
            if awaiter.IsCompleted then 
                // Continue directly
                sm.__jmp<'TResult2> CONT
            else
                // Await and continue later when a result is available.
                sm.__setpc CONT
                TaskStep<'TResult2>.Await (awaiter)
        )
        sm.__gencode CONT (fun () -> 
           result <- awaiter.GetResult()
           continuation result
        )
        sm.__jmp ENTRY

    /// Special case of the above for `Task<'TResult1>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    let inline bindTaskConfigureFalse (sm: SM) (task : Task<'TResult1>) (continuation : 'TResult1 -> TaskStep<'TResult2>) =
        let awaiter = task.ConfigureAwait(false).GetAwaiter()
        // codegen
        let ENTRY = sm.__genlabel ()
        let CONT = sm.__genlabel ()
        sm.__gencode ENTRY (fun () -> 
            if awaiter.IsCompleted then
                // Continue directly
                sm.__jmp<'TResult2> CONT
            else
                // Await and continue later when a result is available.
                sm.__setpc CONT
                TaskStep<'TResult2>.Await (awaiter)
        )
        sm.__gencode CONT (fun () -> 
           continuation (awaiter.GetResult())
        )
        // execute
        sm.__jmp ENTRY

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let inline combine (sm: SM) (step : TaskStep<unit>) (continuation : unit -> TaskStep<'T>) : TaskStep<'T> =
        let mutable step = step
        let ENTRY = sm.__genlabel ()
        let CONT = sm.__genlabel ()
        sm.__gencode ENTRY (fun () -> 
            if step.IsReturn then 
                sm.__jmp<'T> CONT
            elif step.IsReturnFrom then
                let t = step.GetNextTask()
                sm.__setpc CONT
                TaskStep<'T>.Await (t.GetAwaiter())
            else  
                // CODEGEN: TODO: this doesn't feel right, we are not jumping to a label
                // CODEGEN: instead, all code paths should end up executing the continuation
                //
                // Whenever an Await has been generated elsewhere, the pc has already been set to
                // the resumption point.
                let rp = sm.__getpc
                sm.__setpc (sm.__code (fun () -> 
                    step <- sm.__jmp<unit> rp
                    sm.__jmp<'T> ENTRY))
                TaskStep<'T>.Await (step.GetAwaitable()))

        sm.__gencode CONT (fun () -> 
            continuation ())

        sm.__jmp ENTRY

    /// Builds a step that executes the body while the condition predicate is true.
    let inline whileLoop (sm: SM) (cond : unit -> bool) (body : unit -> TaskStep<unit>) : TaskStep<unit> =
        let ENTRY = sm.__genlabel()
        sm.__gencode ENTRY (fun () -> 
            if cond() then
                combine sm (body()) (fun () -> sm.__jmp<unit> ENTRY)
            else
                zero())
        sm.__jmp<unit> ENTRY

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline tryWith (sm: SM) (code : unit -> TaskStep<'T>) (catch : exn -> TaskStep<'T>) : TaskStep<'T> =
        let ENTRY = sm.__genlabel()
        // On resume, we have to go through into the INNER_ENTRY protected by the try/with
        let mutable INNER_ENTRY = sm.__code code 
        sm.__gencode ENTRY (fun () -> 
            try
                let step = sm.__jmp<'T> INNER_ENTRY
                if step.IsReturn then 
                    step
                elif step.IsReturnFrom then
                    let t = step.GetNextTask()
                    let awaitable = t.GetAwaiter()
                    sm.__setpc (sm.__code (fun () ->
                        try
                             // note, this may raise exceptions, but the code is generated in the context of the try-with
                             awaitable.GetResult() |> TaskStep<_>.Return
                        with exn -> 
                             catch exn))
                    TaskStep<'T>.Await(awaitable)
                else 
                    // CODEGEN: This can be:
                    // pc <- step.GetResumeLabel()
                    // TaskStep<_>.Await (step.GetAwaitable())
                    let rp = sm.__getpc
                    sm.__setpc (sm.__code (fun () -> 
                        INNER_ENTRY <- rp
                        sm.__jmp<'T> ENTRY))
                    TaskStep<_>.Await (step.GetAwaitable())
            with exn -> 
                catch exn
        )
        sm.__jmp<'T> ENTRY

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline tryFinally (sm: SM) (code : unit -> TaskStep<'T>) compensation =
        // codegen
        let ENTRY = sm.__genlabel()
        let mutable INNER_ENTRY = sm.__code code 
        sm.__gencode ENTRY (fun () -> 
            let mutable step =
                try
                    sm.__jmp<'T> INNER_ENTRY
                    // Important point: we use a try/with, not a try/finally, to implement tryFinally.
                    // The reason for this is that if we're just building a continuation, we definitely *shouldn't*
                    // execute the `compensation()` part yet -- the actual execution of the asynchronous code hasn't completed!
                with _ ->
                    compensation()
                    reraise()

            if step.IsReturn then 
                compensation()
                step
            elif step.IsReturnFrom then
                let t = step.GetNextTask()
                let awaitable = t.GetAwaiter()
                sm.__setpc (sm.__code (fun () ->
                    let result =
                        try
                            awaitable.GetResult() |> TaskStep<_>.Return
                        with _ ->
                            compensation()
                            reraise()
                    compensation() // if we got here we haven't run compensation(), because we would've reraised after doing so
                    result))
                TaskStep<_>.Await(awaitable)
            else 
                let rp = sm.__getpc
                sm.__setpc (sm.__code (fun () -> 
                    // go back to get inside the try/finally again
                    INNER_ENTRY <- rp
                    sm.__jmp<'T> ENTRY))
                TaskStep<'T>.Await (step.GetAwaitable()) 
        )
        sm.__jmp<'T> ENTRY

    /// Implements a using statement that disposes `disp` after `body` has completed.
    let inline using (sm: SM) (disp : #IDisposable) (body : _ -> TaskStep<'T>) =
        // A using statement is just a try/finally with the finally block disposing if non-null.
        tryFinally sm
            (fun () -> body disp)
            (fun () -> if not (isNull (box disp)) then disp.Dispose())

    /// Implements a loop that runs `body` for each element in `sequence`.
    let inline forLoop sm (sequence : seq<'T>) (body : 'T -> TaskStep<unit>) =
        // A for loop is just a using statement on the sequence's enumerator...
        using sm (sequence.GetEnumerator())
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> whileLoop sm e.MoveNext (fun () -> body e.Current))

    /// Runs a step as a task -- with a short-circuit for immediately completed steps.
    let inline run (sm: SM) (code : unit -> TaskStep<'T>) =
        // CODEGEN: TODO: make this a field of the generated object
        let mutable methodBuilder = AsyncTaskMethodBuilder<Task<'T>>()
        
        // CODEGEN: generate the code and set the initial PC
        do sm.__setpc (sm.__code code)
        
        let mutable machine = 
            { new IAsyncStateMachine with

                /// Proceed to one of three states: result, failure, or awaiting.
                /// If awaiting, MoveNext() will be called again when the awaitable completes.
                member this.MoveNext() =
                    try
                        // CODEGEN: this is a jumptable into the generated code
                        let step = sm.__jmp<'T> sm.__getpc
                        if step.IsReturn then 
                            let res = step.GetResult()
                            methodBuilder.SetResult(Task.FromResult res)
                        elif step.IsReturnFrom then 
                            methodBuilder.SetResult (step.GetNextTask())
                        else 
                            let mutable this = this
                            let mutable await = step.GetAwaitable()
                            assert (not (isNull await))
                            // Tell the builder to call us again when done.
                            methodBuilder.AwaitUnsafeOnCompleted(&await, &this)    
                    with exn ->
                        methodBuilder.SetException exn

                member __.SetStateMachine(_) = () // Doesn't really apply since we're a reference type.
            }

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

// New style task builder.
type TaskBuilder() =
    let _sm = SM()
    // These methods are consistent between all builders.
    member inline __.Delay(f : unit -> TaskStep<'T>) = f
    member inline this.Run(f : unit -> TaskStep<'T>) = run this.SM f
    member inline __.Zero() = zero()
    member inline __.Return(x) = ret x
    member inline this.Combine(step : TaskStep<unit>, continuation) = combine this.SM step continuation
    member inline this.While(condition : unit -> bool, body : unit -> TaskStep<unit>) = whileLoop this.SM condition body
    member inline this.For(sequence : seq<'T>, body : 'T -> TaskStep<unit>) = forLoop this.SM sequence body
    member inline this.TryWith(body : unit -> TaskStep<'T>, catch : exn -> TaskStep<'T>) = tryWith this.SM body catch
    member inline this.TryFinally(body : unit -> TaskStep<'T>, compensation : unit -> unit) = tryFinally this.SM body compensation
    member inline this.Using(disp : #IDisposable, body : #IDisposable -> TaskStep<'T>) = using this.SM disp body
    member inline __.ReturnFrom (task: Task<'T>) : TaskStep<'T> = TaskStep<_>.ReturnFrom task
    member __.SM = _sm

[<AutoOpen>]
module ContextSensitiveTasks =

    let task<'T> = TaskBuilder()

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
                  = TaskLikeBind< ^T >.GenericAwait< ^TaskLike, ^Awaiter, ^T> (sm, taskLike, ret)

        static member inline CanReturnFrom (sm, _priority: IPriority1, computation : Async<'T>) 
                  = bindTask sm (Async.StartAsTask computation) ret : TaskStep<'T>

    type TaskBuilder with
        member inline builder.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: SM * Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> builder.SM Unchecked.defaultof<Witnesses> task continuation

        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: SM * Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> builder.SM Unchecked.defaultof<Witnesses> task


module ContextInsensitiveTasks =

    let task<'T> = TaskBuilder()

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

        static member inline CanBind (sm, _priority :IPriority1, task: Task<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> = bindTaskConfigureFalse sm task k

        static member inline CanBind (sm, _priority: IPriority1, computation : Async<'TResult1>, k: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> = bindTaskConfigureFalse sm (Async.StartAsTask computation) k

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
                                           when (Witnesses or  ^TaskLike): (static member CanBind: SM * Witnesses * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2>
                  = RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2> builder.SM Unchecked.defaultof<Witnesses> task continuation
(*
        member inline builder.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: SM * Witnesses * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> builder.SM Unchecked.defaultof<Witnesses> task
*)
#endif
