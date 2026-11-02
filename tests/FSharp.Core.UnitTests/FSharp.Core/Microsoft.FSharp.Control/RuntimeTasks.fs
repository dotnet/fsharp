module FSharp.Core.UnitTests.Control.RuntimeTasks

#if NET11_0_OR_GREATER
open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

open Microsoft.FSharp.Control.RuntimeAsync.TaskExpressions
open Xunit

exception TestException of string

let BIG = 10

let require x msg =
    if not x then
        failwith msg

let failtest str = raise (TestException str)
let resultOf (task: Task<'T>) = task.GetAwaiter().GetResult()

let private delayed value =
    Task.Delay(1).ContinueWith(fun (_: Task) -> value)

type GatedAwaitable(started: ResizeArray<string>, name: string, gate: Task) =
    member _.GetAwaiter() =
        started.Add name
        gate.GetAwaiter()

// ---------------------------------------------------------------------------
// SmokeTestsForCompilation
// ---------------------------------------------------------------------------

[<Fact>]
let tinyTask () =
    task { return 1 }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let tbind () =
    task {
        let! x = Task.FromResult(1)
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

[<Fact>]
let tnested () =
    task {
        let! x = task { return 1 }
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let tcatch0 () =
    task {
        try
            return 1
        with e ->
            return 2
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let tcatch1 () =
    task {
        try
            let! x = Task.FromResult 1
            return x
        with e ->
            return 2
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let t3 () =
    let t2 () =
        task {
            System.Console.WriteLine("hello")
            return 1
        }

    task {
        System.Console.WriteLine("hello")
        let! x = t2 ()
        System.Console.WriteLine("world")
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

[<Fact>]
let t3b () =
    task {
        System.Console.WriteLine("hello")
        let! x = Task.FromResult(1)
        System.Console.WriteLine("world")
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

[<Fact>]
let t3c () =
    task {
        System.Console.WriteLine("hello")
        do! Task.Delay(100)
        System.Console.WriteLine("world")
        return 1
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

// This tests an exception match
[<Fact>]
let t67 () =
    task {
        try
            do! Task.Delay(0)
        with
        | :? ArgumentException -> ()
        | _ -> ()
    }
    |> fun t ->
        t.Wait()

        if t.Result <> () then
            failwith "failed"

// This tests compiling an incomplete exception match
[<Fact>]
let t68 () =
    task {
        try
            do! Task.Delay(0)
        with :? ArgumentException ->
            ()
    }
    |> fun t ->
        t.Wait()

        if t.Result <> () then
            failwith "failed"

[<Fact>]
let testCompileAsyncWhileLoop () =
    task {
        let mutable i = 0

        while i < 5 do
            i <- i + 1
            do! Task.Yield()

        return i
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 5 then
            failwith "failed"

[<Fact>]
let merge2tasks () =
    task {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let mergeSourcesOverlap () =
    let started = ResizeArray()
    let left = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)
    let right = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)

    let t =
        task {
            let! _ = GatedAwaitable(started, "left", left.Task)
            and! _ = GatedAwaitable(started, "right", right.Task)
            return ()
        }

    require (List.ofSeq started = [ "left"; "right" ]) "MergeSources ran sequentially"
    right.SetResult()
    left.SetResult()
    t.Wait()

[<Fact>]
let merge3tasks () =
    task {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        and! z = Task.FromResult(3)
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

[<Fact>]
let mergeYieldAndTask () =
    task {
        let! _ = Task.Yield()
        and! y = Task.FromResult(1)
        return y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let mergeTaskAndYield () =
    task {
        let! x = Task.FromResult(1)
        and! _ = Task.Yield()
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let merge2valueTasks () =
    task {
        let! x = ValueTask<int>(Task.FromResult(1))
        and! y = ValueTask<int>(Task.FromResult(2))
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let merge2valueTasksAndYield () =
    task {
        let! x = ValueTask<int>(Task.FromResult(1))
        and! y = ValueTask<int>(Task.FromResult(2))
        and! _ = Task.Yield()
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let mergeYieldAnd2tasks () =
    task {
        let! _ = Task.Yield()
        and! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let merge2tasksAndValueTask () =
    task {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        and! z = ValueTask<int>(Task.FromResult(3))
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

[<Fact>]
let merge2asyncs () =
    task {
        let! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let merge3asyncs () =
    task {
        let! x = async { return 1 }
        and! y = async { return 2 }
        and! z = async { return 3 }
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

[<Fact>]
let mergeYieldAndAsync () =
    task {
        let! _ = Task.Yield()
        and! y = async { return 1 }
        return y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let mergeAsyncAndYield () =
    task {
        let! x = async { return 1 }
        and! _ = Task.Yield()
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

[<Fact>]
let mergeYieldAnd2asyncs () =
    task {
        let! _ = Task.Yield()
        and! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

[<Fact>]
let merge2asyncsAndValueTask () =
    task {
        let! x = async { return 1 }
        and! y = async { return 2 }
        and! z = ValueTask<int>(Task.FromResult(3))
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

// ---------------------------------------------------------------------------
// Basics
// ---------------------------------------------------------------------------

[<Fact>]
let testShortCircuitResult () =
    let t =
        task {
            let! x = Task.FromResult(1)
            let! y = Task.FromResult(2)
            return x + y
        }

    require t.IsCompleted "didn't short-circuit already completed tasks"
    require (t.Result = 3) "wrong result"

[<Fact>]
let testDelay () =
    let mutable x = 0

    let t =
        task {
            do! Task.Delay(50)
            x <- x + 1
        }

    require (x = 0) "task already ran"
    t.Wait()

[<Fact>]
let testNonBlocking () =
    let allowContinue = new SemaphoreSlim(0)
    let continueToFinish = new ManualResetEventSlim(false)
    let finished = new ManualResetEventSlim()

    let t =
        task {
            do! allowContinue.WaitAsync()
            continueToFinish.Wait()
            finished.Set()
        }

    allowContinue.Release() |> ignore
    require (not finished.IsSet) "sleep blocked caller"
    continueToFinish.Set()
    t.Wait()

// Exception-handling and disposal coverage.

[<Fact>]
let testCatching1 () =
    let mutable x = 0
    let mutable y = 0

    let t =
        task {
            try
                do! Task.Delay(0)
                failtest "hello"
                x <- 1
                do! Task.Delay(100)
            with
            | TestException msg -> require (msg = "hello") "message tampered"
            | _ -> require false "other exn type"

            y <- 1
        }

    t.Wait()
    require (y = 1) "bailed after exn"
    require (x = 0) "ran past failure"

[<Fact>]
let testCatching2 () =
    let mutable x = 0
    let mutable y = 0

    let t =
        task {
            try
                do! Task.Yield() // can't skip through this
                failtest "hello"
                x <- 1
                do! Task.Delay(100)
            with
            | TestException msg -> require (msg = "hello") "message tampered"
            | _ -> require false "other exn type"

            y <- 1
        }

    t.Wait()
    require (y = 1) "bailed after exn"
    require (x = 0) "ran past failure"

[<Fact>]
let testCatchingInApplicative () =
    let mutable x = 0
    let mutable y = 0

    let t =
        task {
            try
                let! _ =
                    task {
                        do! Task.Delay(100)
                        x <- 1
                    }

                and! _ = task { failtest "hello" }
                ()
            with
            | TestException msg -> require (msg = "hello") "message tampered"
            | _ -> require false "other exn type"

            y <- 1
        }

    t.Wait()
    require (y = 1) "bailed after exn"
    require (x = 1) "exit too early"

[<Fact>]
let testNestedCatching () =
    let mutable counter = 1
    let mutable caughtInner = 0
    let mutable caughtOuter = 0

    let t1 () =
        task {
            try
                do! Task.Yield()
                failtest "hello"
            with TestException msg as exn ->
                caughtInner <- counter
                counter <- counter + 1
                raise exn
        }

    let t2 =
        task {
            try
                do! t1 ()
            with
            | TestException msg as exn ->
                caughtOuter <- counter
                raise exn
            | e -> require false (sprintf "invalid msg type %s" e.Message)
        }

    try
        t2.Wait()
        require false "ran past failed task wait"
    with :? AggregateException as exn ->
        require (exn.InnerExceptions.Count = 1) "more than 1 exn"

    require (caughtInner = 1) "didn't catch inner"
    require (caughtOuter = 2) "didn't catch outer"

[<Fact>]
let testWhileLoopSync () =
    let t =
        task {
            let mutable i = 0

            while i < 10 do
                i <- i + 1

            return i
        }
    //t.Wait() no wait required for sync loop
    require (t.IsCompleted) "didn't do sync while loop properly - not completed"
    require (t.Result = 10) "didn't do sync while loop properly - wrong result"

[<Fact>]
let testWhileLoopAsyncZeroIteration () =
    for i in 1..5 do
        let t =
            task {
                let mutable i = 0

                while i < 0 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 0) "didn't do while loop properly"

[<Fact>]
let testWhileLoopAsyncOneIteration () =
    for i in 1..5 do
        let t =
            task {
                let mutable i = 0

                while i < 1 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 1) "didn't do while loop properly"

[<Fact>]
let testWhileLoopAsync () =
    for i in 1..5 do
        let t =
            task {
                let mutable i = 0

                while i < 10 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 10) "didn't do while loop properly"

[<Fact>]
let testForLoopA () =
    let list = [ "a"; "b"; "c" ] |> Seq.ofList

    let t =
        task {
            let mutable x = Unchecked.defaultof<_>
            let e = list.GetEnumerator()

            while e.MoveNext() do
                x <- e.Current
                do! Task.Yield()
        }

    t.Wait()

[<Fact>]
let testForLoopComplex () =
    let mutable disposed = false

    let wrapList =
        let raw = [ "a"; "b"; "c" ] |> Seq.ofList

        let getEnumerator () =
            let raw = raw.GetEnumerator()

            { new IEnumerator<string> with
                member _.MoveNext() =
                    require (not disposed) "moved next after disposal"
                    raw.MoveNext()

                member _.Current =
                    require (not disposed) "accessed current after disposal"
                    raw.Current

                member _.Current =
                    require (not disposed) "accessed current (boxed) after disposal"
                    box raw.Current

                member _.Dispose() =
                    require (not disposed) "disposed twice"
                    disposed <- true
                    raw.Dispose()

                member _.Reset() =
                    require (not disposed) "reset after disposal"
                    raw.Reset()
            }

        { new IEnumerable<string> with
            member _.GetEnumerator() : IEnumerator<string> = getEnumerator ()
            member _.GetEnumerator() : IEnumerator = upcast getEnumerator ()
        }

    let t =
        task {
            let mutable index = 0
            do! Task.Yield()

            for x in wrapList do
                do! Task.Yield()
                do! Task.Yield()

                match index with
                | 0 -> require (x = "a") "wrong first value"
                | 1 -> require (x = "b") "wrong second value"
                | 2 -> require (x = "c") "wrong third value"
                | _ -> require false "iterated too far!"

                index <- index + 1
                do! Task.Yield()
                do! Task.Yield()

            do! Task.Yield()
            return 1
        }

    t.Wait()
    require disposed "never disposed D"
    require (t.Result = 1) "wrong result"

[<Fact>]
let testForLoopSadPath () =
    for i in 1..5 do
        let wrapList = [ "a"; "b"; "c" ]

        let t =
            task {
                let mutable index = 0
                do! Task.Yield()

                for x in wrapList do
                    do! Task.Yield()
                    index <- index + 1

                return 1
            }

        require (t.Result = 1) "wrong result"

[<Fact>]
let testForLoopSadPathComplex () =
    for i in 1..5 do
        let mutable disposed = false

        let wrapList =
            let raw = [ "a"; "b"; "c" ] |> Seq.ofList

            let getEnumerator () =
                let raw = raw.GetEnumerator()

                { new IEnumerator<string> with
                    member _.MoveNext() =
                        require (not disposed) "moved next after disposal"
                        raw.MoveNext()

                    member _.Current =
                        require (not disposed) "accessed current after disposal"
                        raw.Current

                    member _.Current =
                        require (not disposed) "accessed current (boxed) after disposal"
                        box raw.Current

                    member _.Dispose() =
                        require (not disposed) "disposed twice"
                        disposed <- true
                        raw.Dispose()

                    member _.Reset() =
                        require (not disposed) "reset after disposal"
                        raw.Reset()
                }

            { new IEnumerable<string> with
                member _.GetEnumerator() : IEnumerator<string> = getEnumerator ()
                member _.GetEnumerator() : IEnumerator = upcast getEnumerator ()
            }

        let mutable caught = false

        let t =
            task {
                try
                    let mutable index = 0
                    do! Task.Yield()

                    for x in wrapList do
                        do! Task.Yield()

                        match index with
                        | 0 -> require (x = "a") "wrong first value"
                        | _ -> failtest "uhoh"

                        index <- index + 1
                        do! Task.Yield()

                    do! Task.Yield()
                    return 1
                with TestException "uhoh" ->
                    caught <- true
                    return 2
            }

        require (t.Result = 2) "wrong result"
        require caught "didn't catch exception"
        require disposed "never disposed A"

[<Fact>]
let testExceptionAttachedToTaskWithoutAwait () =
    for i in 1..5 do
        let mutable ranA = false
        let mutable ranB = false

        let t =
            task {
                ranA <- true
                failtest "uhoh"
                ranB <- true
            }

        require ranA "didn't run immediately"
        require (not ranB) "ran past exception"
        require (not (isNull t.Exception)) "didn't capture exception"
        require (t.Exception.InnerExceptions.Count = 1) "captured more exceptions"
        require (t.Exception.InnerException = TestException "uhoh") "wrong exception"
        let mutable caught = false
        let mutable ranCatcher = false

        let catcher =
            task {
                try
                    ranCatcher <- true
                    let! result = t
                    return false
                with TestException "uhoh" ->
                    caught <- true
                    return true
            }

        require ranCatcher "didn't run"
        require catcher.Result "didn't catch"
        require caught "didn't catch"

[<Fact>]
let testExceptionAttachedToTaskWithAwait () =
    for i in 1..5 do
        let mutable ranA = false
        let mutable ranB = false

        let t =
            task {
                ranA <- true
                failtest "uhoh"
                do! Task.Delay(100)
                ranB <- true
            }

        require ranA "didn't run immediately"
        require (not ranB) "ran past exception"
        require (not (isNull t.Exception)) "didn't capture exception"
        require (t.Exception.InnerExceptions.Count = 1) "captured more exceptions"
        require (t.Exception.InnerException = TestException "uhoh") "wrong exception"
        let mutable caught = false
        let mutable ranCatcher = false

        let catcher =
            task {
                try
                    ranCatcher <- true
                    let! result = t
                    return false
                with TestException "uhoh" ->
                    caught <- true
                    return true
            }

        require ranCatcher "didn't run"
        require catcher.Result "didn't catch"
        require caught "didn't catch"

[<Fact>]
let testFixedStackWhileLoop () =
    for i in 1..100 do
        let t =
            task {
                let mutable maxDepth = Nullable()
                let mutable i = 0

                while i < BIG do
                    i <- i + 1
                    do! Task.Yield()

                    if i % 100 = 0 then
                        let stackDepth = StackTrace().FrameCount

                        if maxDepth.HasValue && stackDepth > maxDepth.Value then
                            failwith "Stack depth increased!"

                        maxDepth <- Nullable(stackDepth)

                return i
            }

        t.Wait()
        require (t.Result = BIG) "didn't get to big number"

[<Fact>]
let testFixedStackForLoop () =
    for i in 1..100 do
        let mutable ran = false

        let t =
            task {
                let mutable maxDepth = Nullable()

                for i in Seq.init BIG id do
                    do! Task.Yield()

                    if i % 100 = 0 then
                        let stackDepth = StackTrace().FrameCount

                        if maxDepth.HasValue && stackDepth > maxDepth.Value then
                            failwith "Stack depth increased!"

                        maxDepth <- Nullable(stackDepth)

                ran <- true
                return ()
            }

        t.Wait()
        require ran "didn't run all"

[<Fact>]
let testTypeInference () =
    let t1: string Task = task { return "hello" }

    let t2 =
        task {
            let! (s: string) = t1
            return s.Length
        }

    t2.Wait()

[<Fact>]
let testNoStackOverflowWithImmediateResult () =
    let longLoop =
        task {
            let mutable n = 0

            while n < BIG do
                n <- n + 1
                return! Task.FromResult(())
        }

    longLoop.Wait()

[<Fact>]
let testNoStackOverflowWithYieldResult () =
    let longLoop =
        task {
            let mutable n = 0

            while n < BIG do
                let! _ =
                    task {
                        do! Task.Yield()
                        let! _ = Task.FromResult(0)
                        n <- n + 1
                    }

                n <- n + 1
        }

    longLoop.Wait()

[<Fact>]
let testSmallTailRecursion () =
    let rec loop n =
        task {
            if n < 100 then
                do! Task.Yield()
                let! _ = Task.FromResult(0)
                return! loop (n + 1)
            else
                return ()
        }

    let shortLoop = task { return! loop 0 }
    shortLoop.Wait()

[<Fact>]
let testTryOverReturnFrom () =
    let inner () =
        task {
            do! Task.Yield()
            failtest "inner"
            return 1
        }

    let t =
        task {
            try
                do! Task.Yield()
                return! inner ()
            with TestException "inner" ->
                return 2
        }

    require (t.Result = 2) "didn't catch"

[<Fact>]
let testAsyncsMixedWithTasks () =
    let t =
        task {
            do! Task.Delay(1)
            do! Async.Sleep(1)

            let! x =
                async {
                    do! Async.Sleep(1)
                    return 5
                }

            return! async { return x + 3 }
        }

    let result = t.Result
    require (result = 8) "something weird happened"

[<Fact>]
let testAsyncsMixedWithTasks_ShouldNotSwitchContext () =
    let t =
        task {
            let a = Thread.CurrentThread.ManagedThreadId
            let! b = async { return Thread.CurrentThread.ManagedThreadId }
            let c = Thread.CurrentThread.ManagedThreadId
            return $"Before: {a}, in async: {b}, after async: {c}"
        }

    let d = Thread.CurrentThread.ManagedThreadId
    let actual = $"{t.Result}, after task: {d}"

    require (actual = $"Before: {d}, in async: {d}, after async: {d}, after task: {d}") actual

// no need to call this, we just want to check that it compiles w/o warnings
let testTrivialReturnCompiles (x: 'a) : 'a Task =
    task {
        do! Task.Yield()
        return x
    }

// no need to call this, we just want to check that it compiles w/o warnings
let testTrivialTransformedReturnCompiles (x: 'a) (f: 'a -> 'b) : 'b Task =
    task {
        do! Task.Yield()
        return f x
    }

// no need to call this, we just want to check that it compiles w/o warnings
let testDefaultInferenceForReturnFrom () =
    let t = task { return Some "x" }

    task {
        let! r = t

        if r = None then
            // Divergence from task {}: ReturnFrom is overloaded, so the generic
            // failwithf result needs an explicit Task<_> annotation.
            return! (failwithf "Could not find x": string option Task)
        else
            return r
    }
    |> ignore

// no need to call this, just check that it compiles
let testCompilerInfersArgumentOfReturnFrom () =
    task { if true then return 1 else return! (failwith "": int Task) }
    |> ignore

// Overload-resolution cases from the bottom of Tasks.fs (Issue12184*), compile-only.
type Issue12184() =
    member this.TaskMethod() =
        task {
            // The overload resolution for Bind commits to 'Async<int>' since the type annotation is present.
            let! result = this.AsyncMethod(21)
            return result
        }

    member _.AsyncMethod(value: int) : Async<int> = async { return (value * 2) }

type Issue12184b() =
    member this.TaskMethod() =
        task {
            // The overload resolution for Bind commits to 'YieldAwaitable' since the type annotation is present.
            let! result = this.AsyncMethod(21)
            return result
        }

    member _.AsyncMethod(_value: int) : System.Runtime.CompilerServices.YieldAwaitable = Task.Yield()

// Issue12184c from Tasks.fs is omitted: it relies on task {}'s Bind overload
// resolution committing to Task<_> for an unannotated argument, which the
// task builder's overload set does not support.

module Issue12184d =
    let TaskMethod (t: ValueTask) =
        task {
            let! result = t
            return result
        }

module Issue12184e =
    let TaskMethod (t: ValueTask<int>) =
        task {
            let! result = t
            return result
        }

module Issue12184f =
    let TaskMethod (t: Task) =
        task {
            let! result = t
            return result
        }

// ---------------------------------------------------------------------------
// Exception-handling and disposal coverage.
// ---------------------------------------------------------------------------

[<Fact>]
let testNoDelay () =
    let mutable x = 0

    let t =
        task {
            x <- x + 1
            do! Task.Delay(5)
            x <- x + 1
        }

    require (x = 1) "first part didn't run yet"
    t.Wait()

[<Fact>]
let testTryFinallyHappyPath () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            task {
                try
                    require (not ran) "ran way early"
                    do! Task.Delay(100)
                    require (not ran) "ran kinda early"
                finally
                    ran <- true
            }

        t.Wait()
        require ran "never ran"

[<Fact>]
let testTryFinallySadPath () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            task {
                try
                    require (not ran) "ran way early"
                    do! Task.Delay(100)
                    require (not ran) "ran kinda early"
                    failtest "uhoh"
                finally
                    ran <- true
            }

        try
            t.Wait()
        with _ ->
            ()

        require ran "never ran"

[<Fact>]
let testTryFinallyCaught () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            task {
                try
                    try
                        require (not ran) "ran way early"
                        do! Task.Delay(100)
                        require (not ran) "ran kinda early"
                        failtest "uhoh"
                    finally
                        ran <- true

                    return 1
                with _ ->
                    return 2
            }

        require (t.Result = 2) "wrong return"
        require ran "never ran"

[<Fact>]
let testUsing () =
    for i in 1..5 do
        let mutable disposed = false

        let t =
            task {
                use d =
                    { new IDisposable with
                        member _.Dispose() = disposed <- true
                    }

                require (not disposed) "disposed way early"
                do! Task.Delay(100)
                require (not disposed) "disposed kinda early"
            }

        t.Wait()
        require disposed "never disposed B"

[<Fact>]
let testUsingFromTask () =
    let mutable disposedInner = false
    let mutable disposed = false

    let t =
        task {
            use! d =
                task {
                    do! Task.Delay(50)

                    use i =
                        { new IDisposable with
                            member _.Dispose() = disposedInner <- true
                        }

                    require (not disposed && not disposedInner) "disposed inner early"

                    return
                        { new IDisposable with
                            member _.Dispose() = disposed <- true
                        }
                }

            require disposedInner "did not dispose inner after task completion"
            require (not disposed) "disposed way early"
            do! Task.Delay(50)
            require (not disposed) "disposed kinda early"
        }

    t.Wait()
    require disposed "never disposed C"

[<Fact>]
let testUsingSadPath () =
    let mutable disposedInner = false
    let mutable disposed = false

    let t =
        task {
            try
                use! d =
                    task {
                        do! Task.Delay(50)

                        use i =
                            { new IDisposable with
                                member _.Dispose() = disposedInner <- true
                            }

                        failtest "uhoh"
                        require (not disposed && not disposedInner) "disposed inner early"

                        return
                            { new IDisposable with
                                member _.Dispose() = disposed <- true
                            }
                    }

                ()
            with TestException msg ->
                require disposedInner "did not dispose inner after task completion"
                require (not disposed) "disposed way early"
                do! Task.Delay(50)
                require (not disposed) "disposed kinda early"
        }

    t.Wait()
    require (not disposed) "disposed thing that never should've existed"

[<Fact>]
let testUsingAsyncDisposableSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            task { disposed <- disposed + 1 } |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(100)
                require (disposed = 0) "disposed kinda early"
            }

        t.Wait()
        require (disposed >= 1) "never disposed B"
        require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testUsingAsyncDisposableAsync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            task {
                                do! Task.Delay(10)
                                disposed <- disposed + 1
                            }
                            |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(100)
                require (disposed = 0) "disposed kinda early"
            }

        t.Wait()
        require (disposed >= 1) "never disposed B"
        require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testUsingAsyncDisposableExnAsync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            task {
                                do! Task.Delay(10)
                                disposed <- disposed + 1
                            }
                            |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                failtest "oops"
            }

        try
            t.Wait()
        with :? AggregateException ->
            require (disposed >= 1) "never disposed B"
            require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testUsingAsyncDisposableExnSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            task {
                                disposed <- disposed + 1
                                do! Task.Delay(10)
                            }
                            |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                failtest "oops"
            }

        try
            t.Wait()
        with :? AggregateException ->
            require (disposed >= 1) "never disposed B"
            require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testUsingAsyncDisposableDelayExnSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            task {
                                disposed <- disposed + 1
                                do! Task.Delay(10)
                            }
                            |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(10)
                require (disposed = 0) "disposed kind of early"
                failtest "oops"
            }

        try
            t.Wait()
        with :? AggregateException ->
            require (disposed >= 1) "never disposed B"
            require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testUsingBindAsyncDisposableSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            task {
                use! d =
                    task {
                        do! Task.Delay(10)

                        return
                            { new IAsyncDisposable with
                                member _.DisposeAsync() =
                                    task { disposed <- disposed + 1 } |> ValueTask
                            }
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(100)
                require (disposed = 0) "disposed kinda early"
            }

        t.Wait()
        require (disposed >= 1) "never disposed B"
        require (disposed <= 1) "too many dispose on B"

[<Fact>]
let testExceptionThrownInFinally () =
    for i in 1..5 do
        use stepOutside = new SemaphoreSlim(0)
        use ranInitial = new ManualResetEventSlim()
        use ranNext = new ManualResetEventSlim()
        let mutable ranFinally = 0

        let t =
            task {
                try
                    ranInitial.Set()
                    do! Task.Yield()
                    do! stepOutside.WaitAsync()
                    ranNext.Set()
                finally
                    ranFinally <- ranFinally + 1
                    failtest "finally exn!"
            }

        require ranInitial.IsSet "didn't run initial"
        require (not ranNext.IsSet) "ran next too early"
        stepOutside.Release() |> ignore

        try
            t.Wait()
            require false "shouldn't get here"
        with _ ->
            ()

        require ranNext.IsSet "didn't run next"
        require (ranFinally = 1) "didn't run finally exactly once"

[<Fact>]
let test2ndExceptionThrownInFinally () =
    for i in 1..5 do
        use ranInitial = new ManualResetEventSlim()
        use continueTask = new SemaphoreSlim(0)
        use ranNext = new ManualResetEventSlim()
        let mutable ranFinally = 0

        let t =
            task {
                try
                    ranInitial.Set()
                    do! continueTask.WaitAsync()
                    ranNext.Set()
                    do! Task.Yield()
                    failtest "uhoh"
                finally
                    ranFinally <- ranFinally + 1
                    failtest "2nd exn!"
            }

        ranInitial.Wait()
        continueTask.Release() |> ignore

        try
            t.Wait()
            require false "shouldn't get here"
        with _ ->
            ()

        require ranNext.IsSet "didn't run next"
        require (ranFinally = 1) "didn't run finally exactly once"

[<Fact>]
let testTryFinallyOverReturnFromWithException () =
    let inner () =
        task {
            do! Task.Yield()
            failtest "inner"
            return 1
        }

    let mutable m = 0

    let t =
        task {
            try
                do! Task.Yield()
                return! inner ()
            finally
                m <- 1
        }

    try
        t.Wait()
    with :? AggregateException ->
        ()

    require (m = 1) "didn't run finally"

[<Fact>]
let testTryFinallyOverReturnFromWithoutException () =
    let inner () =
        task {
            do! Task.Yield()
            return 1
        }

    let mutable m = 0

    let t =
        task {
            try
                do! Task.Yield()
                return! inner ()
            finally
                m <- 1
        }

    try
        t.Wait()
    with :? AggregateException ->
        ()

    require (m = 1) "didn't run finally"

// A minimal custom awaitable, exercising the SRTP Bind/ReturnFrom/MergeSources
// fallbacks (task {} supports arbitrary task-likes the same way).
type CustomAwaitable(result: int) =
    member _.GetAwaiter() = (Task.FromResult result).GetAwaiter()

[<Fact>]
let testCustomAwaitable () =
    let t =
        task {
            let! x = CustomAwaitable 20
            let! y = CustomAwaitable 20
            return x + y
        }

    require (t.Result = 40) "custom awaitable bind"

    let t2 = task { return! CustomAwaitable 42 }
    require (t2.Result = 42) "custom awaitable return from"

    let t3 =
        task {
            let! x = CustomAwaitable 20
            and! y = CustomAwaitable 22
            return x + y
        }

    require (t3.Result = 42) "custom awaitable merge sources"

[<Fact>]
let testTaskUsesSyncContext () =
    for i in 1..5 do
        let mutable ran = false
        let mutable posted = false
        let oldSyncContext = SynchronizationContext.Current

        let syncContext =
            { new SynchronizationContext() with
                member _.Post(d, state) =
                    posted <- true
                    d.Invoke(state)
            }

        try
            SynchronizationContext.SetSynchronizationContext syncContext
            let tid = System.Threading.Thread.CurrentThread.ManagedThreadId
            require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread A"
            require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread A"

            let t =
                task {
                    let tid2 = System.Threading.Thread.CurrentThread.ManagedThreadId
                    require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread B"
                    require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread B"
                    do! Task.Yield()
                    require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread C"
                    require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread C"
                    ran <- true
                }

            t.Wait()
            require ran "never ran"
            require posted "never posted"
        finally
            SynchronizationContext.SetSynchronizationContext oldSyncContext

type private DualDisposable(syncDispose: unit -> unit, asyncDispose: unit -> ValueTask) =
    interface IDisposable with member _.Dispose() = syncDispose()
    interface IAsyncDisposable with member _.DisposeAsync() = asyncDispose()

[<Fact>]
let testCancellationIdentity () =
    use cancellation = new CancellationTokenSource()
    let input = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let output = task { let! value = input.Task in return value }
    require (not output.IsCompleted) "cancellation input must suspend"
    cancellation.Cancel()
    input.SetCanceled(cancellation.Token)
    try
        resultOf output |> ignore
        failwith "cancellation was lost"
    with :? OperationCanceledException as error ->
        require (error.CancellationToken = cancellation.Token) "cancellation token changed"
    require output.IsCanceled "task cancellation status"

[<Fact>]
let testDualDisposalPriority () =
    let cleanup = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let mutable synchronous = 0
    let mutable asynchronous = 0
    let resource =
        new DualDisposable(
            (fun () -> synchronous <- synchronous + 1),
            (fun () -> asynchronous <- asynchronous + 1; ValueTask(cleanup.Task)))
    let output = task { use resource = resource in return 42 }
    require (not output.IsCompleted && asynchronous = 1 && synchronous = 0) "async disposal must take priority and suspend"
    cleanup.SetResult()
    require (resultOf output = 42 && asynchronous = 1 && synchronous = 0) "disposal must finish once"

#endif
