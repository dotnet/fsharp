// TaskBuilder.fs - TPL task computation expressions for F#
//
// Originally written in 2016 by Robert Peele (humbobst@gmail.com)
// New operator-based overload resolution for F# 4.0 compatibility by Gustavo Leon in 2018.
// Revised for insertion into FSHarp.Core by Microsoft, 2019.
//
// To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights
// to this software to the public domain worldwide. This software is distributed without any warranty.

namespace Mirosoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

[<NoComparison; NoEquality>]
/// Represents the state of a computation: either awaiting something with a continuation, or completed with a return value.
type TaskStep<'T> =
  | Await of ICriticalNotifyCompletion * (unit -> TaskStep<'T>)
  | Return of 'T
  | ReturnFrom of Task<'T>

[<AutoOpen>]
module TaskHelpers = 

    /// Implements the machinery of running a `TaskStep<'m, 'm>` as a task returning a continuation task.
    type StepStateMachine<'T>(firstStep) as this =
        let methodBuilder = AsyncTaskMethodBuilder<Task<'T>>()

        /// The continuation we left off awaiting on our last MoveNext().
        let mutable continuation = fun () -> firstStep

        /// Returns next pending awaitable or null if exiting (including tail call).
        let nextAwaitable() =
            try
                match continuation() with
                | Return r ->
                    methodBuilder.SetResult(Task.FromResult(r))
                    null
                | ReturnFrom t ->
                    methodBuilder.SetResult(t)
                    null
                | Await (await, next) ->
                    continuation <- next
                    await
            with
            | exn ->
                methodBuilder.SetException(exn)
                null

        let mutable self = this

        /// Start execution as a `Task<Task<'T>>`.
        member __.Run() =
            methodBuilder.Start(&self)
            methodBuilder.Task
    
        interface IAsyncStateMachine with

            /// Proceed to one of three states: result, failure, or awaiting.
            /// If awaiting, MoveNext() will be called again when the awaitable completes.
            member __.MoveNext() =
                let mutable await = nextAwaitable()
                if not (isNull await) then
                    // Tell the builder to call us again when this thing is done.
                    methodBuilder.AwaitUnsafeOnCompleted(&await, &self)    

            member __.SetStateMachine(_) = () // Doesn'T really apply since we're a reference type.

    let unwrapException (agg : AggregateException) =
        let inners = agg.InnerExceptions
        if inners.Count = 1 then inners.[0]
        else agg :> Exception

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    let zero = Return ()

    /// Used to return a value.
    let ret (x : 'T) = Return x

    [<Sealed>]
    type Binder<'T> =
        // We put the output generic parameter up here at the class level, so it doesn'T get subject to
        // inline rules. If we put it all in the inline function, then the compiler gets confused at the
        // below and demands that the whole function either is limited to working with (x : obj), or must
        // be inline itself.
        //
        // let yieldThenReturn (x : 'T) =
        //     task {
        //         do! Task.Yield()
        //         return x
        //     }

        static member inline GenericAwait< ^Awaitable, ^Awaiter, ^TResult
                                            when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion 
                                            and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                            and ^Awaiter : (member GetResult : unit -> ^TResult) >
            (abl : ^Awaitable, continuation : ^TResult -> TaskStep<'T>) : TaskStep<'T> =
                let awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(abl)) // get an awaiter from the awaitable
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then // shortcut to continue immediately
                    continuation (^Awaiter : (member GetResult : unit -> ^TResult)(awaiter))
                else
                    Await (awaiter, fun () -> continuation (^Awaiter : (member GetResult : unit -> ^TResult)(awaiter)))

        static member inline GenericAwaitConfigureFalse< ^Task, ^Awaitable, ^Awaiter, ^TResult
                                                        when ^Task : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^TResult) >
            (task : ^Task, continuation : ^TResult -> TaskStep<'T>) : TaskStep<'T> =
                let abl = (^Task : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
                Binder<'T>.GenericAwait(abl, continuation)

    /// Special case of the above for `Task<'T>`. Have to write this T by hand to avoid confusing the compiler
    /// trying to decide between satisfying the constraints with `Task` or `Task<'T>`.
    let bindTask (task : Task<'T>) (continuation : 'T -> TaskStep<'TResult>) =
        let awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then // Proceed to the next step based on the result we already have.
            continuation(awaiter.GetResult())
        else // Await and continue later when a result is available.
            Await (awaiter, (fun () -> continuation(awaiter.GetResult())))

    /// Special case of the above for `Task<'T>`, for the context-insensitive builder.
    /// Have to write this T by hand to avoid confusing the compiler thinking our built-in bind method
    /// defined on the builder has fancy generic constraints on inp and T parameters.
    let bindTaskConfigureFalse (task : Task<'T>) (continuation : 'T -> TaskStep<'TResult>) =
        let awaiter = task.ConfigureAwait(false).GetAwaiter()
        if awaiter.IsCompleted then // Proceed to the next step based on the result we already have.
            continuation(awaiter.GetResult())
        else // Await and continue later when a result is available.
            Await (awaiter, (fun () -> continuation(awaiter.GetResult())))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let rec combine (step : TaskStep<unit>) (continuation : unit -> TaskStep<'TResult>) =
        match step with
        | Return _ -> continuation()
        | ReturnFrom t ->
            Await (t.GetAwaiter(), continuation)
        | Await (awaitable, next) ->
            Await (awaitable, fun () -> combine (next()) continuation)

    /// Builds a step that executes the body while the condition predicate is true.
    let whileLoop (cond : unit -> bool) (body : unit -> TaskStep<unit>) =
        if cond() then
            // Create a self-referencing closure to test whether to repeat the loop on future iterations.
            let rec repeat () =
                if cond() then
                    let body = body()
                    match body with
                    | Return _ -> repeat()
                    | ReturnFrom t -> Await(t.GetAwaiter(), repeat)
                    | Await (awaitable, next) ->
                        Await (awaitable, fun () -> combine (next()) repeat)
                else zero
            // Run the body the first time and chain it to the repeat logic.
            combine (body()) repeat
        else zero

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let rec tryWith(step : unit -> TaskStep<'T>) (catch : exn -> TaskStep<'T>) =
        try
            match step() with
            | Return _ as i -> i
            | ReturnFrom t ->
                let awaitable = t.GetAwaiter()
                Await(awaitable, fun () ->
                    try
                        awaitable.GetResult() |> Return
                    with
                    | exn -> catch exn)
            | Await (awaitable, next) -> Await (awaitable, fun () -> tryWith next catch)
        with
        | exn -> catch exn

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let rec tryFinally (step : unit -> TaskStep<'T>) fin =
        let step =
            try step()
            // Important point: we use a try/with, not a try/finally, to implement tryFinally.
            // The reason for this is that if we're just building a continuation, we definitely *shouldn'T*
            // execute the `fin()` part yet -- the actual execution of the asynchronous code hasn'T completed!
            with
            | _ ->
                fin()
                reraise()
        match step with
        | Return _ as i ->
            fin()
            i
        | ReturnFrom t ->
            let awaitable = t.GetAwaiter()
            Await(awaitable, fun () ->
                let result =
                    try
                        awaitable.GetResult() |> Return
                    with
                    | _ ->
                        fin()
                        reraise()
                fin() // if we got here we haven'T run fin(), because we would've reraised after doing so
                result)
        | Await (awaitable, next) ->
            Await (awaitable, fun () -> tryFinally next fin)

    /// Implements a using statement that disposes `disp` after `body` has completed.
    let using (disp : #IDisposable) (body : _ -> TaskStep<'T>) =
        // A using statement is just a try/finally with the finally block disposing if non-null.
        tryFinally
            (fun () -> body disp)
            (fun () -> if not (isNull (box disp)) then disp.Dispose())

    /// Implements a loop that runs `body` for each element in `sequence`.
    let forLoop (sequence : 'T seq) (body : 'T -> TaskStep<unit>) =
        // A for loop is just a using statement on the sequence's enumerator...
        using (sequence.GetEnumerator())
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> whileLoop e.MoveNext (fun () -> body e.Current))

    /// Runs a step as a task -- with a short-circuit for immediately completed steps.
    let run (firstStep : unit -> TaskStep<'T>) =
        try
            match firstStep() with
            | Return x -> Task.FromResult(x)
            | ReturnFrom t -> t
            | Await _ as step -> StepStateMachine<'T>(step).Run().Unwrap() // sadly can'T do tail recursion
        // Any exceptions should go on the task, rather than being thrown from this call.
        // This matches C# behavior where you won'T see an exception until awaiting the task,
        // even if it failed before reaching the first "await".
        with
        | exn ->
            let src = new TaskCompletionSource<_>()
            src.SetException(exn)
            src.Task

    type Priority3 = obj
    type Priority2 = IComparable

    [<NoComparison; NoEquality>]
    type BindSensitive =
        | Priority1 
        //static member inline (>>=) (_:Priority2, taskLike : ^TaskLike) : ((^T -> TaskStep< ^TResult >) -> TaskStep< ^TResult >) = fun k -> Binder<_>.GenericAwait (taskLike, k)
        static member        (>>=) (_: BindSensitive, task: Task<'T>)         : (('T -> TaskStep< 'TResult >) -> TaskStep< 'TResult >) = fun k -> bindTask task k                      
        //static member        (>>=) (_:BindSensitive, computation  : Async<'T>)       : (('T -> TaskStep< 'TResult >) -> TaskStep< 'TResult >) = fun k -> bindTask (Async.StartAsTask computation) k     

    [<NoComparison; NoEquality>]
    type ReturnFromSensitive = 
        | Priority1 
        static member inline ($) (_:ReturnFromSensitive, taskLike: ^TaskLike) : TaskStep< ^T > 
    //                when  ^TaskLike: (member GetAwaiter:  ^TaskLike ->  ^Awaiter)
    //                and ^Awaiter :> ICriticalNotifyCompletion
    //                and ^Awaiter: (member get_IsCompleted:  ^Awaiter -> bool)
    //                and ^Awaiter: (member GetResult:  ^Awaiter ->  ^T)
           = Binder< ^T >.GenericAwait (taskLike, ret)

        //static member        ($) (_:ReturnFromSensitive, computation : Async<'T>) = bindTask (Async.StartAsTask computation) ret : TaskStep<'T>

    [<NoComparison; NoEquality>]
    type BindInsensitive = 
        | Priority1 
        //static member inline (>>=) (_:Priority3, taskLike            : 'T) : (('T -> TaskStep< 'TResult >) -> TaskStep< 'TResult >) = fun k -> Binder<'TResult>.GenericAwait (taskLike, k)
        //static member inline (>>=) (_:Priority2, configurableTaskLike: 'T) = fun (k :  _ -> TaskStep<'TResult>) -> Binder<'TResult>.GenericAwaitConfigureFalse (configurableTaskLike, k)
        static member        (>>=) (_:BindInsensitive, task: Task<'T>)  : (('T -> TaskStep< 'TResult >) -> TaskStep< 'TResult >) = fun (k : 'T -> TaskStep<'TResult>) -> bindTaskConfigureFalse task k
        //static member        (>>=) (_:BindInsensitive, computation   : Async<'T>) = fun (k : 'T -> TaskStep<'TResult>) -> bindTaskConfigureFalse (Async.StartAsTask computation) k

    [<NoComparison; NoEquality>]
    type ReturnFromInsensitive = 
        | Priority1 with
        static member inline ($) (_:Priority2, taskLike            ) = Binder<_>.GenericAwait(taskLike, ret)
        //static member inline ($) (_:ReturnFromInsensitive, configurableTaskLike) = Binder<_>.GenericAwaitConfigureFalse(configurableTaskLike, ret)
        //static member        ($) (_:ReturnFromInsensitive, computation   : Async<'T>      ) = bindTaskConfigureFalse (Async.StartAsTask computation) ret

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
    member __.ReturnFrom (task: Task<'T>) : TaskStep<'T> = ReturnFrom task

[<AutoOpen>]
module ContextSensitiveTasks =

    let task = TaskBuilder()

    type TaskBuilder with
        member inline __.Bind (task: Task<'T>, continuation : 'T -> TaskStep<'TResult>) : TaskStep<'TResult> = (Unchecked.defaultof<BindSensitive> >>= task) continuation
        //member inline __.ReturnFrom computation                               : TaskStep<'TResult> = Unchecked.defaultof<ReturnFromSensitive> $ computation

module ContextInsensitiveTasks =

    let task = TaskBuilder()

    type TaskBuilder with
        member inline __.Bind (task: Task<'T>, continuation : 'T -> TaskStep<'TResult>) : TaskStep<'TResult> = (Unchecked.defaultof<BindInsensitive> >>= task) continuation
//        member inline __.Bind (task, continuation : 'T -> TaskStep<'TResult>) : TaskStep<'TResult> = (Unchecked.defaultof<BindInsensitive> >>= task) continuation
//        member inline __.ReturnFrom computation                               : TaskStep<'TResult> = Unchecked.defaultof<ReturnFromInsensitive> $ computation