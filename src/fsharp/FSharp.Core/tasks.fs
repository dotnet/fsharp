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

namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
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
type TaskStep<'T>(contents: TaskStepContents<'T>) =
    member __.Contents = contents

    static member Return x = TaskStep<'T>(Return x)
    static member Await (completion, continuation) = TaskStep<'T>(Await (completion, continuation))
    static member ReturnFrom task = TaskStep<'T>(ReturnFrom task)

and 
    [<NoComparison; NoEquality>]
    TaskStepContents<'T> =
    | Await of ICriticalNotifyCompletion * (unit -> TaskStep<'T>)
    | Return of 'T
    | ReturnFrom of Task<'T>



[<AutoOpen>]
module TaskHelpers = 

    /// Implements the machinery of running a `TaskStep` as a task returning a continuation task.
    type StepStateMachine<'T>(firstStep: TaskStep<'T>) =
        let methodBuilder = AsyncTaskMethodBuilder<Task<'T>>()

        /// The continuation we left off awaiting on our last MoveNext().
        let mutable continuation = fun () -> firstStep

        /// Start execution as a `Task<Task<'T>>`.
        member this.Run() =
            let mutable this = this
            methodBuilder.Start(&this)
            methodBuilder.Task
    
        interface IAsyncStateMachine with

            /// Proceed to one of three states: result, failure, or awaiting.
            /// If awaiting, MoveNext() will be called again when the awaitable completes.
            member this.MoveNext() =
                let mutable await = 
                    try
                        match continuation().Contents with
                        | Return r ->
                            methodBuilder.SetResult(Task.FromResult r)
                            null
                        | ReturnFrom t ->
                            methodBuilder.SetResult t
                            null
                        | Await (await, next) ->
                            continuation <- next
                            await
                    with exn ->
                        methodBuilder.SetException exn
                        null

                if not (isNull await) then
                    let mutable this = this
                    // Tell the builder to call us again when this thing is done.
                    methodBuilder.AwaitUnsafeOnCompleted(&await, &this)    

            member __.SetStateMachine(_) = () // Doesn't really apply since we're a reference type.

    let unwrapException (agg : AggregateException) =
        let inners = agg.InnerExceptions
        if inners.Count = 1 then inners.[0]
        else agg :> Exception

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    let zero = TaskStep<unit>.Return ()

    /// Used to return a value.
    let inline ret (x : 'T) = TaskStep<'T>.Return x

    let inline RequireCanBind< ^Priority, ^TaskLike, 'TResult1, ^TResult2 when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike -> (('TResult1 -> TaskStep< ^TResult2 >) -> TaskStep< ^TResult2 >))                                   > (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike -> (('TResult1 -> TaskStep< ^TResult2 >) -> TaskStep< ^TResult2 >)) (x,y))

    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'TResult when (^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskStep<'TResult>)> (x: ^Priority) (y: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskStep< 'TResult >) (x,y))

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
            (awaitable : ^Awaitable, continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable)) // get an awaiter from the awaitable
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                    continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                else
                    TaskStep<_>.Await (awaiter, fun () -> continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter)))

        static member inline GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) >
            (task : ^TaskLike, continuation : ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
                let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
                TaskLikeBind<'TResult2>.GenericAwait(awaitable, continuation)

    /// Special case of the above for `Task<'T>`. Have to write this T by hand to avoid confusing the compiler
    /// trying to decide between satisfying the constraints with `Task` or `Task<'T>`.
    let bindTask (task : Task<'T>) (continuation : 'T -> TaskStep<'TResult>) =
        let awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then 
            // Continue directly
            continuation (awaiter.GetResult())
        else
            // Await and continue later when a result is available.
            TaskStep<_>.Await (awaiter, (fun () -> continuation (awaiter.GetResult())))

    /// Special case of the above for `Task<'T>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    let bindTaskConfigureFalse (task : Task<'T>) (continuation : 'T -> TaskStep<'TResult>) =
        let awaiter = task.ConfigureAwait(false).GetAwaiter()
        if awaiter.IsCompleted then
            // Continue directly
            continuation (awaiter.GetResult())
        else
            // Await and continue later when a result is available.
            TaskStep<_>.Await (awaiter, (fun () -> continuation (awaiter.GetResult())))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let rec combine (step : TaskStep<unit>) (continuation : unit -> TaskStep<'TResult>) =
        match step.Contents with
        | Return _ -> continuation()
        | ReturnFrom t -> TaskStep<_>.Await (t.GetAwaiter(), continuation)
        | Await (awaitable, next) -> TaskStep<_>.Await (awaitable, fun () -> combine (next()) continuation)

    /// Builds a step that executes the body while the condition predicate is true.
    let whileLoop (cond : unit -> bool) (body : unit -> TaskStep<unit>) =
        if cond() then
            // Create a self-referencing closure to test whether to repeat the loop on future iterations.
            let mutable repeat = Unchecked.defaultof<_>
            repeat <- fun () ->
                if cond() then
                    combine (body()) repeat
                else
                    zero
            // Run the body the first time and chain it to the repeat logic.
            combine (body()) repeat
        else zero

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let rec tryWith (step : unit -> TaskStep<'T>) (catch : exn -> TaskStep<'T>) =
        try
            let stepResult = step()
            match stepResult.Contents with
            | Return _ -> stepResult
            | ReturnFrom t ->
                let awaitable = t.GetAwaiter()
                TaskStep<_>.Await(awaitable, fun () ->
                    try
                        awaitable.GetResult() |> TaskStep<_>.Return
                    with exn -> 
                        catch exn)
            | Await (awaitable, next) -> 
                TaskStep<_>.Await (awaitable, fun () -> tryWith next catch)
        with exn -> catch exn

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let rec tryFinally (step : unit -> TaskStep<'T>) fin =
        let stepResult =
            try step()
            // Important point: we use a try/with, not a try/finally, to implement tryFinally.
            // The reason for this is that if we're just building a continuation, we definitely *shouldn't*
            // execute the `fin()` part yet -- the actual execution of the asynchronous code hasn't completed!
            with _ ->
                fin()
                reraise()

        match stepResult.Contents with
        | Return _ ->
            fin()
            stepResult
        | ReturnFrom t ->
            let awaitable = t.GetAwaiter()
            TaskStep<_>.Await(awaitable, fun () ->
                let result =
                    try
                        awaitable.GetResult() |> TaskStep<_>.Return
                    with _ ->
                        fin()
                        reraise()
                fin() // if we got here we haven't run fin(), because we would've reraised after doing so
                result)
        | Await (awaitable, next) ->
            TaskStep<_>.Await (awaitable, fun () -> tryFinally next fin)

    /// Implements a using statement that disposes `disp` after `body` has completed.
    let using (disp : #IDisposable) (body : _ -> TaskStep<'T>) =
        // A using statement is just a try/finally with the finally block disposing if non-null.
        tryFinally
            (fun () -> body disp)
            (fun () -> if not (isNull (box disp)) then disp.Dispose())

    /// Implements a loop that runs `body` for each element in `sequence`.
    let forLoop (sequence : seq<'T>) (body : 'T -> TaskStep<unit>) =
        // A for loop is just a using statement on the sequence's enumerator...
        using (sequence.GetEnumerator())
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> whileLoop e.MoveNext (fun () -> body e.Current))

    /// Runs a step as a task -- with a short-circuit for immediately completed steps.
    let run (firstStep : unit -> TaskStep<'T>) =
        try
            let firstStepResult = firstStep()
            match firstStepResult.Contents with
            | Return x -> Task.FromResult x
            | ReturnFrom task -> task
            | Await _ -> StepStateMachine<'T>(firstStepResult).Run().Unwrap() // sadly can't do tail recursion

        with exn ->
            // Any exceptions should go on the task, rather than being thrown from this call.
            // This matches C# behavior where you won't see an exception until awaiting the task,
            // even if it failed before reaching the first "await".
            let src = new TaskCompletionSource<_>()
            src.SetException exn
            src.Task

// New style task builder.
type TaskBuilder() =
    // These methods are consistent between all builders.
    member __.Delay(f : unit -> TaskStep<'T>) = f
    member __.Run(f : unit -> TaskStep<'T>) = run f
    member __.Zero() = zero
    member __.Return(x) = ret x
    member __.Combine(step : TaskStep<unit>, continuation) = combine step continuation
    member __.While(condition : unit -> bool, body : unit -> TaskStep<unit>) = whileLoop condition body
    member __.For(sequence : seq<'T>, body : 'T -> TaskStep<unit>) = forLoop sequence body
    member __.TryWith(body : unit -> TaskStep<'T>, catch : exn -> TaskStep<'T>) = tryWith body catch
    member __.TryFinally(body : unit -> TaskStep<'T>, fin : unit -> unit) = tryFinally body fin
    member __.Using(disp : #IDisposable, body : #IDisposable -> TaskStep<'T>) = using disp body
    member __.ReturnFrom (task: Task<'T>) : TaskStep<'T> = TaskStep<_>.ReturnFrom task

[<AutoOpen>]
module ContextSensitiveTasks =

    let task = TaskBuilder()

    [<Sealed>]
    type Witnesses() =

        // Give the type arguments explicitly to make it match the signature precisely
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter 
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>(_priority: IPriority2, taskLike : ^TaskLike) 
              : ((^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >) 
                  = fun k -> TaskLikeBind< 'TResult2 >.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (taskLike, k)

        static member CanBind (_priority: IPriority1, task: Task<'TResult1>)
              : (('TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >)
                  = fun k -> bindTask task k                      

        static member CanBind (_priority: IPriority1, computation  : Async<'TResult1>)
              : (('TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >) 
                  = fun k -> bindTask (Async.StartAsTask computation) k     

        // Give the type arguments explicitly to make it match the signature precisely
        static member inline CanReturnFrom< ^TaskLike, ^T, ^Awaiter
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)> 
              (_priority: IPriority1, taskLike: ^TaskLike) : TaskStep< ^T > 
                  = TaskLikeBind< ^T >.GenericAwait< ^TaskLike, ^Awaiter, ^T> (taskLike, ret)

        static member CanReturnFrom (_priority: IPriority1, computation : Async<'T>) 
                  = bindTask (Async.StartAsTask computation) ret : TaskStep<'T>

    type TaskBuilder with
        member inline __.Bind< ^TaskLike, 'TResult1, ^TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike -> (('TResult1 -> TaskStep< ^TResult2 >) -> TaskStep< ^TResult2 >))> 
                    (task: ^TaskLike, continuation: 'TResult1 -> TaskStep< ^TResult2 >) : TaskStep< ^TResult2 >
                  = RequireCanBind< Witnesses, ^TaskLike, 'TResult1, ^TResult2> Unchecked.defaultof<Witnesses> task continuation

        member inline __.ReturnFrom< ^TaskLike, 'TResult  when (Witnesses or ^TaskLike): (static member CanReturnFrom : Witnesses * ^TaskLike -> TaskStep<'TResult>) > (task: ^TaskLike) : TaskStep<'TResult> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'TResult> Unchecked.defaultof<Witnesses> task

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
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> (_priority: IPriority3, taskLike: ^TaskLike) : ((^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >)
              = fun k -> TaskLikeBind<'TResult2>.GenericAwait< ^TaskLike, ^Awaiter, ^TResult1> (taskLike, k)

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter 
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> (_priority: IPriority2, configurableTaskLike: ^TaskLike) : ((^TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >) 
              = fun k -> TaskLikeBind<'TResult2>.GenericAwaitConfigureFalse< ^TaskLike, ^Awaitable, ^Awaiter, ^TResult1> (configurableTaskLike, k)

        static member CanBind (_priority :IPriority1, task: Task<'TResult1>) : (('TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >) = fun k -> bindTaskConfigureFalse task k

        static member CanBind (_priority: IPriority1, computation : Async<'TResult1>) : (('TResult1 -> TaskStep< 'TResult2 >) -> TaskStep< 'TResult2 >) = fun k -> bindTaskConfigureFalse (Async.StartAsTask computation) k

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
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult1) > (_: IPriority1, configurableTaskLike: ^TaskLike) = 
                                    TaskLikeBind< ^TResult1 >.GenericAwaitConfigureFalse(configurableTaskLike, ret)
        
        static member CanReturnFrom (_priority: IPriority1, computation   : Async<'T>      ) = bindTaskConfigureFalse (Async.StartAsTask computation) ret

    type TaskBuilder with
        member inline __.Bind< ^TaskLike, 'TResult1, ^TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike -> (('TResult1 -> TaskStep< ^TResult2 >) -> TaskStep< ^TResult2 >))> 
                    (task: ^TaskLike, continuation: 'TResult1 -> TaskStep< ^TResult2 >) : TaskStep< ^TResult2 >
                  = RequireCanBind< Witnesses, ^TaskLike, 'TResult1, ^TResult2> Unchecked.defaultof<Witnesses> task continuation

        member inline __.ReturnFrom< ^TaskLike, 'TResult  when (Witnesses or ^TaskLike): (static member CanReturnFrom : Witnesses * ^TaskLike -> TaskStep<'TResult>) > (task: ^TaskLike) : TaskStep<'TResult> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'TResult> Unchecked.defaultof<Witnesses> task
