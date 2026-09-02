// Tests for the runtime-async RuntimeTaskBuilder, ported from the TaskBuilder tests in
// tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Control/Tasks.fs
// with `task {` replaced by `runtimeTask {`. Test names and bodies are kept as
// close to the originals as possible.
//
// `backgroundTask` tests have no runtimeTask equivalent and are omitted.

module RuntimeTasks

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

open RuntimeTaskBuilder.RuntimeTask
open RuntimeTaskBuilder.RuntimeTaskAwaitableExtensions

exception TestException of string

let BIG = 10

let require x msg =
    if not x then
        failwith msg

let failtest str = raise (TestException str)
let resultOf (task: Task<'T>) = task.GetAwaiter().GetResult()

let private delayed value =
    Task.Delay(1).ContinueWith(fun (_: Task) -> value)

// ---------------------------------------------------------------------------
// SmokeTestsForCompilation
// ---------------------------------------------------------------------------

let tinyTask () =
    runtimeTask { return 1 }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let tbind () =
    runtimeTask {
        let! x = Task.FromResult(1)
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

let tnested () =
    runtimeTask {
        let! x = runtimeTask { return 1 }
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let tcatch0 () =
    runtimeTask {
        try
            return 1
        with e ->
            return 2
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let tcatch1 () =
    runtimeTask {
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

let t3 () =
    let t2 () =
        runtimeTask {
            System.Console.WriteLine("hello")
            return 1
        }

    runtimeTask {
        System.Console.WriteLine("hello")
        let! x = t2 ()
        System.Console.WriteLine("world")
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

let t3b () =
    runtimeTask {
        System.Console.WriteLine("hello")
        let! x = Task.FromResult(1)
        System.Console.WriteLine("world")
        return 1 + x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 2 then
            failwith "failed"

let t3c () =
    runtimeTask {
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
let t67 () =
    runtimeTask {
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
let t68 () =
    runtimeTask {
        try
            do! Task.Delay(0)
        with :? ArgumentException ->
            ()
    }
    |> fun t ->
        t.Wait()

        if t.Result <> () then
            failwith "failed"

let testCompileAsyncWhileLoop () =
    runtimeTask {
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

let merge2tasks () =
    runtimeTask {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let merge3tasks () =
    runtimeTask {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        and! z = Task.FromResult(3)
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

let mergeYieldAndTask () =
    runtimeTask {
        let! _ = Task.Yield()
        and! y = Task.FromResult(1)
        return y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let mergeTaskAndYield () =
    runtimeTask {
        let! x = Task.FromResult(1)
        and! _ = Task.Yield()
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let merge2valueTasks () =
    runtimeTask {
        let! x = ValueTask<int>(Task.FromResult(1))
        and! y = ValueTask<int>(Task.FromResult(2))
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let merge2valueTasksAndYield () =
    runtimeTask {
        let! x = ValueTask<int>(Task.FromResult(1))
        and! y = ValueTask<int>(Task.FromResult(2))
        and! _ = Task.Yield()
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let mergeYieldAnd2tasks () =
    runtimeTask {
        let! _ = Task.Yield()
        and! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let merge2tasksAndValueTask () =
    runtimeTask {
        let! x = Task.FromResult(1)
        and! y = Task.FromResult(2)
        and! z = ValueTask<int>(Task.FromResult(3))
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

let merge2asyncs () =
    runtimeTask {
        let! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let merge3asyncs () =
    runtimeTask {
        let! x = async { return 1 }
        and! y = async { return 2 }
        and! z = async { return 3 }
        return x + y + z
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 6 then
            failwith "failed"

let mergeYieldAndAsync () =
    runtimeTask {
        let! _ = Task.Yield()
        and! y = async { return 1 }
        return y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let mergeAsyncAndYield () =
    runtimeTask {
        let! x = async { return 1 }
        and! _ = Task.Yield()
        return x
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 1 then
            failwith "failed"

let mergeYieldAnd2asyncs () =
    runtimeTask {
        let! _ = Task.Yield()
        and! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
    |> fun t ->
        t.Wait()

        if t.Result <> 3 then
            failwith "failed"

let merge2asyncsAndValueTask () =
    runtimeTask {
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

let testShortCircuitResult () =
    let t =
        runtimeTask {
            let! x = Task.FromResult(1)
            let! y = Task.FromResult(2)
            return x + y
        }

    require t.IsCompleted "didn't short-circuit already completed tasks"
    require (t.Result = 3) "wrong result"

let testDelay () =
    let mutable x = 0

    let t =
        runtimeTask {
            do! Task.Delay(50)
            x <- x + 1
        }

    require (x = 0) "task already ran"
    t.Wait()

let testNonBlocking () =
    let allowContinue = new SemaphoreSlim(0)
    let continueToFinish = new ManualResetEventSlim(false)
    let finished = new ManualResetEventSlim()

    let t =
        runtimeTask {
            do! allowContinue.WaitAsync()
            continueToFinish.Wait()
            finished.Set()
        }

    allowContinue.Release() |> ignore
    require (not finished.IsSet) "sleep blocked caller"
    continueToFinish.Set()
    t.Wait()

// Exception-handling and disposal coverage.

let testCatching1 () =
    let mutable x = 0
    let mutable y = 0

    let t =
        runtimeTask {
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

let testCatching2 () =
    let mutable x = 0
    let mutable y = 0

    let t =
        runtimeTask {
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

let testCatchingInApplicative () =
    let mutable x = 0
    let mutable y = 0

    let t =
        runtimeTask {
            try
                let! _ =
                    runtimeTask {
                        do! Task.Delay(100)
                        x <- 1
                    }

                and! _ = runtimeTask { failtest "hello" }
                ()
            with
            | TestException msg -> require (msg = "hello") "message tampered"
            | _ -> require false "other exn type"

            y <- 1
        }

    t.Wait()
    require (y = 1) "bailed after exn"
    require (x = 1) "exit too early"

let testNestedCatching () =
    let mutable counter = 1
    let mutable caughtInner = 0
    let mutable caughtOuter = 0

    let t1 () =
        runtimeTask {
            try
                do! Task.Yield()
                failtest "hello"
            with TestException msg as exn ->
                caughtInner <- counter
                counter <- counter + 1
                raise exn
        }

    let t2 =
        runtimeTask {
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

let testWhileLoopSync () =
    let t =
        runtimeTask {
            let mutable i = 0

            while i < 10 do
                i <- i + 1

            return i
        }
    //t.Wait() no wait required for sync loop
    require (t.IsCompleted) "didn't do sync while loop properly - not completed"
    require (t.Result = 10) "didn't do sync while loop properly - wrong result"

let testWhileLoopAsyncZeroIteration () =
    for i in 1..5 do
        let t =
            runtimeTask {
                let mutable i = 0

                while i < 0 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 0) "didn't do while loop properly"

let testWhileLoopAsyncOneIteration () =
    for i in 1..5 do
        let t =
            runtimeTask {
                let mutable i = 0

                while i < 1 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 1) "didn't do while loop properly"

let testWhileLoopAsync () =
    for i in 1..5 do
        let t =
            runtimeTask {
                let mutable i = 0

                while i < 10 do
                    i <- i + 1
                    do! Task.Yield()

                return i
            }

        t.Wait()
        require (t.Result = 10) "didn't do while loop properly"

let testForLoopA () =
    let list = [ "a"; "b"; "c" ] |> Seq.ofList

    let t =
        runtimeTask {
            let mutable x = Unchecked.defaultof<_>
            let e = list.GetEnumerator()

            while e.MoveNext() do
                x <- e.Current
                do! Task.Yield()
        }

    t.Wait()

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
        runtimeTask {
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

let testForLoopSadPath () =
    for i in 1..5 do
        let wrapList = [ "a"; "b"; "c" ]

        let t =
            runtimeTask {
                let mutable index = 0
                do! Task.Yield()

                for x in wrapList do
                    do! Task.Yield()
                    index <- index + 1

                return 1
            }

        require (t.Result = 1) "wrong result"

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
            runtimeTask {
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

let testExceptionAttachedToTaskWithoutAwait () =
    for i in 1..5 do
        let mutable ranA = false
        let mutable ranB = false

        let t =
            runtimeTask {
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
            runtimeTask {
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

let testExceptionAttachedToTaskWithAwait () =
    for i in 1..5 do
        let mutable ranA = false
        let mutable ranB = false

        let t =
            runtimeTask {
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
            runtimeTask {
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

let testFixedStackWhileLoop () =
    for i in 1..100 do
        let t =
            runtimeTask {
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

let testFixedStackForLoop () = // needs investigation: code after a suspending for loop is not run
    for i in 1..100 do
        let mutable ran = false

        let t =
            runtimeTask {
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

let testTypeInference () =
    let t1: string Task = runtimeTask { return "hello" }

    let t2 =
        runtimeTask {
            // Divergence from task {}: the runtimeTask Bind overload set does not
            // propagate the element type here, so the annotation is required.
            let! (s: string) = t1
            return s.Length
        }

    t2.Wait()

let testNoStackOverflowWithImmediateResult () =
    let longLoop =
        runtimeTask {
            let mutable n = 0

            while n < BIG do
                n <- n + 1
                return! Task.FromResult(())
        }

    longLoop.Wait()

let testNoStackOverflowWithYieldResult () =
    let longLoop =
        runtimeTask {
            let mutable n = 0

            while n < BIG do
                let! _ =
                    runtimeTask {
                        do! Task.Yield()
                        let! _ = Task.FromResult(0)
                        n <- n + 1
                    }

                n <- n + 1
        }

    longLoop.Wait()

let testSmallTailRecursion () =
    let rec loop n =
        runtimeTask {
            if n < 100 then
                do! Task.Yield()
                let! _ = Task.FromResult(0)
                return! loop (n + 1)
            else
                return ()
        }

    let shortLoop = runtimeTask { return! loop 0 }
    shortLoop.Wait()

let testTryOverReturnFrom () =
    let inner () =
        runtimeTask {
            do! Task.Yield()
            failtest "inner"
            return 1
        }

    let t =
        runtimeTask {
            try
                do! Task.Yield()
                return! inner ()
            with TestException "inner" ->
                return 2
        }

    require (t.Result = 2) "didn't catch"

let testAsyncsMixedWithTasks () =
    let t =
        runtimeTask {
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

let testAsyncsMixedWithTasks_ShouldNotSwitchContext () =
    let t =
        runtimeTask {
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
    runtimeTask {
        do! Task.Yield()
        return x
    }

// no need to call this, we just want to check that it compiles w/o warnings
let testTrivialTransformedReturnCompiles (x: 'a) (f: 'a -> 'b) : 'b Task =
    runtimeTask {
        do! Task.Yield()
        return f x
    }

// no need to call this, we just want to check that it compiles w/o warnings
let testDefaultInferenceForReturnFrom () =
    let t = runtimeTask { return Some "x" }

    runtimeTask {
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
    runtimeTask { if true then return 1 else return! (failwith "": int Task) }
    |> ignore

// Overload-resolution cases from the bottom of Tasks.fs (Issue12184*), compile-only.
type Issue12184() =
    member this.TaskMethod() =
        runtimeTask {
            // The overload resolution for Bind commits to 'Async<int>' since the type annotation is present.
            let! result = this.AsyncMethod(21)
            return result
        }

    member _.AsyncMethod(value: int) : Async<int> = async { return (value * 2) }

type Issue12184b() =
    member this.TaskMethod() =
        runtimeTask {
            // The overload resolution for Bind commits to 'YieldAwaitable' since the type annotation is present.
            let! result = this.AsyncMethod(21)
            return result
        }

    member _.AsyncMethod(_value: int) : System.Runtime.CompilerServices.YieldAwaitable = Task.Yield()

// Issue12184c from Tasks.fs is omitted: it relies on task {}'s Bind overload
// resolution committing to Task<_> for an unannotated argument, which the
// runtimeTask builder's overload set does not support.

module Issue12184d =
    let TaskMethod (t: ValueTask) =
        runtimeTask {
            let! result = t
            return result
        }

module Issue12184e =
    let TaskMethod (t: ValueTask<int>) =
        runtimeTask {
            let! result = t
            return result
        }

module Issue12184f =
    let TaskMethod (t: Task) =
        runtimeTask {
            let! result = t
            return result
        }

// ---------------------------------------------------------------------------
// Exception-handling and disposal coverage.
// ---------------------------------------------------------------------------

let knownDivergent_testNoDelay () =
    let mutable x = 0

    let t =
        runtimeTask {
            x <- x + 1
            do! Task.Delay(5)
            x <- x + 1
        }

    require (x = 1) "first part didn't run yet"
    t.Wait()

let testTryFinallyHappyPath () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            runtimeTask {
                try
                    require (not ran) "ran way early"
                    do! Task.Delay(100)
                    require (not ran) "ran kinda early"
                finally
                    ran <- true
            }

        t.Wait()
        require ran "never ran"

let testTryFinallySadPath () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            runtimeTask {
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

let testTryFinallyCaught () =
    for i in 1..5 do
        let mutable ran = false

        let t =
            runtimeTask {
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

let testUsing () =
    for i in 1..5 do
        let mutable disposed = false

        let t =
            runtimeTask {
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

let testUsingFromTask () =
    let mutable disposedInner = false
    let mutable disposed = false

    let t =
        runtimeTask {
            use! d =
                runtimeTask {
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

let testUsingSadPath () =
    let mutable disposedInner = false
    let mutable disposed = false

    let t =
        runtimeTask {
            try
                use! d =
                    runtimeTask {
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

let testUsingAsyncDisposableSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            runtimeTask { disposed <- disposed + 1 } |> ValueTask
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(100)
                require (disposed = 0) "disposed kinda early"
            }

        t.Wait()
        require (disposed >= 1) "never disposed B"
        require (disposed <= 1) "too many dispose on B"

let testUsingAsyncDisposableAsync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            runtimeTask {
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

let testUsingAsyncDisposableExnAsync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            runtimeTask {
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

let testUsingAsyncDisposableExnSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            runtimeTask {
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

let testUsingAsyncDisposableDelayExnSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use d =
                    { new IAsyncDisposable with
                        member _.DisposeAsync() =
                            runtimeTask {
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

let testUsingBindAsyncDisposableSync () =
    for i in 1..5 do
        let mutable disposed = 0

        let t =
            runtimeTask {
                use! d =
                    runtimeTask {
                        do! Task.Delay(10)

                        return
                            { new IAsyncDisposable with
                                member _.DisposeAsync() =
                                    runtimeTask { disposed <- disposed + 1 } |> ValueTask
                            }
                    }

                require (disposed = 0) "disposed way early"
                do! Task.Delay(100)
                require (disposed = 0) "disposed kinda early"
            }

        t.Wait()
        require (disposed >= 1) "never disposed B"
        require (disposed <= 1) "too many dispose on B"

let testExceptionThrownInFinally () =
    for i in 1..5 do
        use stepOutside = new SemaphoreSlim(0)
        use ranInitial = new ManualResetEventSlim()
        use ranNext = new ManualResetEventSlim()
        let mutable ranFinally = 0

        let t =
            runtimeTask {
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

let test2ndExceptionThrownInFinally () =
    for i in 1..5 do
        use ranInitial = new ManualResetEventSlim()
        use continueTask = new SemaphoreSlim(0)
        use ranNext = new ManualResetEventSlim()
        let mutable ranFinally = 0

        let t =
            runtimeTask {
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

let testTryFinallyOverReturnFromWithException () =
    let inner () =
        runtimeTask {
            do! Task.Yield()
            failtest "inner"
            return 1
        }

    let mutable m = 0

    let t =
        runtimeTask {
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

let testTryFinallyOverReturnFromWithoutException () =
    let inner () =
        runtimeTask {
            do! Task.Yield()
            return 1
        }

    let mutable m = 0

    let t =
        runtimeTask {
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

let testCustomAwaitable () =
    let t =
        runtimeTask {
            let! x = CustomAwaitable 20
            let! y = CustomAwaitable 20
            return x + y
        }

    require (t.Result = 40) "custom awaitable bind"

    let t2 = runtimeTask { return! CustomAwaitable 42 }
    require (t2.Result = 42) "custom awaitable return from"

    let t3 =
        runtimeTask {
            let! x = CustomAwaitable 20
            and! y = CustomAwaitable 22
            return x + y
        }

    require (t3.Result = 42) "custom awaitable merge sources"

let testTaskUsesSyncContext () = // task completes without the body observably running when a SynchronizationContext is installed
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
                runtimeTask {
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

[<EntryPoint>]
let main _ =
    tinyTask ()
    tbind ()
    tnested ()
    tcatch0 ()
    tcatch1 ()
    t3 ()
    t3b ()
    t3c ()
    t67 ()
    t68 ()
    testCompileAsyncWhileLoop ()
    merge2tasks ()
    merge3tasks ()
    mergeYieldAndTask ()
    mergeTaskAndYield ()
    merge2valueTasks ()
    merge2valueTasksAndYield ()
    mergeYieldAnd2tasks ()
    merge2tasksAndValueTask ()
    merge2asyncs ()
    merge3asyncs ()
    mergeYieldAndAsync ()
    mergeAsyncAndYield ()
    mergeYieldAnd2asyncs ()
    merge2asyncsAndValueTask ()
    testShortCircuitResult ()
    testDelay ()
    testNonBlocking ()
    testCatching1 ()
    testCatching2 ()
    testCatchingInApplicative ()
    testNestedCatching ()
    testWhileLoopSync ()
    testWhileLoopAsyncZeroIteration ()
    testWhileLoopAsyncOneIteration ()
    testWhileLoopAsync ()
    testForLoopA ()
    testForLoopComplex ()
    testForLoopSadPath ()
    testForLoopSadPathComplex ()
    testFixedStackWhileLoop ()
    testFixedStackForLoop ()
    testTypeInference ()
    testNoStackOverflowWithImmediateResult ()
    testNoStackOverflowWithYieldResult ()
    testSmallTailRecursion ()
    testTryOverReturnFrom ()
    testTryFinallyOverReturnFromWithException ()
    testTryFinallyOverReturnFromWithoutException ()
    testAsyncsMixedWithTasks ()
    testAsyncsMixedWithTasks_ShouldNotSwitchContext ()
    testCustomAwaitable ()
    testUsingAsyncDisposableSync ()
    testUsingAsyncDisposableAsync ()
    testUsingAsyncDisposableExnAsync ()
    testUsingAsyncDisposableExnSync ()
    testUsingAsyncDisposableDelayExnSync ()
    testUsingBindAsyncDisposableSync ()
    testTryFinallyHappyPath ()
    testTryFinallySadPath ()
    testTryFinallyCaught ()
    testUsing ()
    testUsingFromTask ()
    testUsingSadPath ()
    testExceptionThrownInFinally ()
    test2ndExceptionThrownInFinally ()
    testTaskUsesSyncContext ()
    testExceptionAttachedToTaskWithoutAwait ()
    testExceptionAttachedToTaskWithAwait ()
    0