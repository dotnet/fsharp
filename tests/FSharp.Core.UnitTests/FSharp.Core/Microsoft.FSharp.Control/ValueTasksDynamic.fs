// Tests for TaskBuilder.fs
//
// Written in 2016 by Robert Peele (humbobst@gmail.com)
//
// To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights
// to this software to the public domain worldwide. This software is distributed without any warranty.
//
// You should have received a copy of the CC0 Public Domain Dedication along with this software.
// If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.
//
//
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the 'dynamic' implementation of the task type when state machine
// compilation fails.

namespace FSharp.Core.UnitTests.Control.ValueTasksDynamic
#if NETCOREAPP
#nowarn "1204" // construct only for use in compiled code
#nowarn "3511" // state machine not staticlly compilable - the one in 'Run'
open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Control
#if STANDALONE
[<AttributeUsage(AttributeTargets.Method, AllowMultiple=false)>]
type FactAttribute() = inherit Attribute()
#else
open Xunit
open System.Runtime.CompilerServices

#endif

// Delegates to task, except 'Run' which is deliberately not inlined, hence no chance
// of static compilation of state machines.  
type ValueTaskBuilderDynamic() =
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member _.Run(code) = valuetask.Run(code) // warning 3511 is generated here: state machine not compilable

    member inline _.Delay f = valuetask.Delay(f)
    [<DefaultValue>]
    member inline _.Zero()  = valuetask.Zero()
    member inline _.Return (value) = valuetask.Return(value)
    member inline _.Combine(task1, task2) = valuetask.Combine(task1, task2)
    member inline _.While ([<InlineIfLambda>] condition, body) = valuetask.While(condition, body)
    member inline _.TryWith (body, catch) = valuetask.TryWith(body, catch)
    member inline _.TryFinally (body, compensation ) = valuetask.TryFinally(body, compensation)
    member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IAsyncDisposable> (resource: 'Resource, body: 'Resource -> ValueTaskCode<'TOverall, 'T>) =
        valuetask.Using(resource, body)
    member inline _.For (sequence, body) = valuetask.For(sequence, body)
    member inline _.ReturnFrom (t: ValueTask<'T>) = valuetask.ReturnFrom(t)

[<AutoOpen>]
module TaskBuilderDynamicLowPriority = 

    // Low priority extension method
    type ValueTaskBuilderDynamic with
        member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (resource: 'Resource, body: 'Resource -> ValueTaskCode<'TOverall, 'T>) =
            valuetask.Using(resource, body)


[<AutoOpen>]
module Value = 

    [<AutoOpen>]
    module ValueTaskLowProrityExtensions = 

        type ValueTaskBuilderDynamic with
            member inline _.ReturnFrom< ^TaskLike, ^Awaiter, ^T
                                                  when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                  and ^Awaiter :> ICriticalNotifyCompletion
                                                  and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                                  and ^Awaiter: (member GetResult: unit ->  ^T)>
                    (t: ^TaskLike) : ValueTaskCode< ^T,  ^T> =
                valuetask.ReturnFrom(t)
            member inline _.Bind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                                when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                                and ^Awaiter :> ICriticalNotifyCompletion
                                                and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                                and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
                        (t: ^TaskLike, continuation: (^TResult1 -> ValueTaskCode<'TOverall, 'TResult2>)) : ValueTaskCode<'TOverall, 'TResult2> =
                valuetask.Bind(t, continuation)


    [<AutoOpen>]
    module HighLowProrityExtensions = 

        type ValueTaskBuilderDynamic with
            member inline _.Bind (t: ValueTask<'TResult1>, continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>)) : ValueTaskCode<'TOverall, 'TResult2> =
                valuetask.Bind(t, continuation)

            member inline _.Bind (computation: Async<'TResult1>, continuation: ('TResult1 -> ValueTaskCode<'TOverall, 'TResult2>)) : ValueTaskCode<'TOverall, 'TResult2> =
                valuetask.Bind(computation, continuation)

            member inline _.ReturnFrom (t: ValueTask<'T>) : ValueTaskCode<'T, 'T> =
                valuetask.ReturnFrom(t)

            member inline _.ReturnFrom (computation: Async<'T>)  : ValueTaskCode<'T, 'T> =
                valuetask.ReturnFrom(computation)



    let valuetaskDynamic = ValueTaskBuilderDynamic()
    type Do_no_use_valuetask_in_this_file_use_taskDynamic_instead = | Nope 
    let valuetask = Do_no_use_valuetask_in_this_file_use_taskDynamic_instead.Nope

[<AutoOpen>]
module ValueTask =
    let wait (task :ValueTask<'t>) =
        if not task.IsCompleted then
            task.AsTask().Wait()

    let unitvaluetask (task: ValueTask<unit>) : ValueTask =
        if task.IsCompleted then
            ValueTask.CompletedTask
        else
            task.AsTask() |> ValueTask
        

type ITaskThing =
    abstract member Taskify : 'a option -> 'a ValueTask

type SmokeTestsForCompilation() =

    [<Fact>]
    member __.tinyTask() =
        valuetaskDynamic {
            return 1
        }
        |> fun t -> 
            wait t
            if t.Result <> 1 then failwith "failed"

    [<Fact>]
    member __.tbind() =
        valuetaskDynamic {
            let! x = ValueTask.FromResult(1)
            return 1 + x
        }
        |> fun t -> 
            wait t
            if t.Result <> 2 then failwith "failed"

    [<Fact>]
    member __.tnested() =
        valuetaskDynamic {
            let! x = valuetaskDynamic { return 1 }
            return x
        }
        |> fun t -> 
            wait t
            if t.Result <> 1 then failwith "failed"

    [<Fact>]
    member __.tcatch0() =
        valuetaskDynamic {
            try 
               return 1
            with e -> 
               return 2
        }
        |> fun t -> 
            wait t
            if t.Result <> 1 then failwith "failed"

    [<Fact>]
    member __.tcatch1() =
        valuetaskDynamic {
            try 
               let! x = ValueTask.FromResult 1
               return x
            with e -> 
               return 2
        }
        |> fun t -> 
            wait t
            if t.Result <> 1 then failwith "failed"


    [<Fact>]
    member __.t3() =
        let t2() =
            valuetaskDynamic {
                System.Console.WriteLine("hello")
                return 1
            }
        valuetaskDynamic {
            System.Console.WriteLine("hello")
            let! x = t2()
            System.Console.WriteLine("world")
            return 1 + x
        }
        |> fun t -> 
            wait t
            if t.Result <> 2 then failwith "failed"

    [<Fact>]
    member __.t3b() =
        valuetaskDynamic {
            System.Console.WriteLine("hello")
            let! x = ValueTask.FromResult(1)
            System.Console.WriteLine("world")
            return 1 + x
        }
        |> fun t -> 
            wait t
            if t.Result <> 2 then failwith "failed"

    [<Fact>]
    member __.t3c() =
        valuetaskDynamic {
            System.Console.WriteLine("hello")
            do! Task.Delay(100)
            System.Console.WriteLine("world")
            return 1 
        }
        |> fun t -> 
            wait t
            if t.Result <> 1 then failwith "failed"

    [<Fact>]
    // This tests an exception match
    member __.t67() =
        valuetaskDynamic {
            try
                do! Task.Delay(0)
            with
            | :? ArgumentException -> 
                ()
            | _ -> 
                ()
        }
        |> fun t -> 
            wait t
            if t.Result <> () then failwith "failed"

    [<Fact>]
    // This tests compiling an incomplete exception match
    member __.t68() =
        valuetaskDynamic {
            try
                do! Task.Delay(0)
            with
            | :? ArgumentException -> 
                ()
        }
        |> fun t -> 
            wait t
            if t.Result <> () then failwith "failed"

    [<Fact>]
    member __.testCompileAsyncWhileLoop() =
        valuetaskDynamic {
            let mutable i = 0
            while i < 5 do
                i <- i + 1
                do! Task.Yield()
            return i
        }
        |> fun t -> 
            wait t
            if t.Result <> 5 then failwith "failed"


exception TestException of string

[<AutoOpen>]
module Helpers = 
    let BIG = 10
    // let BIG = 10000
    let require x msg = if not x then failwith msg
    let failtest str = raise (TestException str)

type Basics() = 
    [<Fact>]
    member __.testShortCircuitResult() =
        printfn "Running testShortCircuitResult..."
        let t =
            valuetaskDynamic {
                let! x = ValueTask.FromResult(1)
                let! y = ValueTask.FromResult(2)
                return x + y
            }
        require t.IsCompleted "didn't short-circuit already completed tasks"
        printfn "t.Result = %A" t.Result
        require (t.Result = 3) "wrong result"

    [<Fact>]
    member __.testDelay() =
        printfn "Running testDelay..."
        let mutable x = 0
        let t =
            valuetaskDynamic {
                do! Task.Delay(50)
                x <- x + 1
            }
        printfn "task created and first step run...."
        require (x = 0) "task already ran"
        printfn "waiting...."
        wait t

    [<Fact>]
    member __.testNoDelay() =
        printfn "Running testNoDelay..."
        let mutable x = 0
        let t =
            valuetaskDynamic {
                x <- x + 1
                do! Task.Delay(5)
                x <- x + 1
            }
        require (x = 1) "first part didn't run yet"
        wait t

    [<Fact>]
    member __.testNonBlocking() =
        printfn "Running testNonBlocking..."
        let sw = Stopwatch()
        sw.Start()
        let t =
            valuetaskDynamic {
                do! Task.Yield()
                Thread.Sleep(100)
            }
        sw.Stop()
        require (sw.ElapsedMilliseconds < 50L) "sleep blocked caller"
        wait t

    [<Fact>]
    member __.testCatching1() =
        printfn "Running testCatching1..."
        let mutable x = 0
        let mutable y = 0
        let t =
            valuetaskDynamic {
                try
                    do! Task.Delay(0)
                    failtest "hello"
                    x <- 1
                    do! Task.Delay(100)
                with
                | TestException msg ->
                    require (msg = "hello") "message tampered"
                | _ ->
                    require false "other exn type"
                    require false "other exn type"
                y <- 1
            }
        wait t
        require (y = 1) "bailed after exn"
        require (x = 0) "ran past failure"

    [<Fact>]
    member __.testCatching2() =
        printfn "Running testCatching2..."
        let mutable x = 0
        let mutable y = 0
        let t =
            valuetaskDynamic {
                try
                    do! Task.Yield() // can't skip through this
                    failtest "hello"
                    x <- 1
                    do! Task.Delay(100)
                with
                | TestException msg ->
                    require (msg = "hello") "message tampered"
                | _ ->
                    require false "other exn type"
                y <- 1
            }
        wait t
        require (y = 1) "bailed after exn"
        require (x = 0) "ran past failure"

    [<Fact>]
    member __.testNestedCatching() =
        printfn "Running testNestedCatching..."
        let mutable counter = 1
        let mutable caughtInner = 0
        let mutable caughtOuter = 0
        let t1() =
            valuetaskDynamic {
                try
                    do! Task.Yield()
                    failtest "hello"
                with
                | TestException msg as exn ->
                    caughtInner <- counter
                    counter <- counter + 1
                    raise exn
            }
        let t2 =
            valuetaskDynamic {
                try
                    do! t1()
                with
                | TestException msg as exn ->
                    caughtOuter <- counter
                    raise exn
                | e ->
                    require false (sprintf "invalid msg type %s" e.Message)
            }
        try
            wait t2
            require false "ran past failed task wait"
        with
        | :? AggregateException as exn ->
            require (exn.InnerExceptions.Count = 1) "more than 1 exn"
        require (caughtInner = 1) "didn't catch inner"
        require (caughtOuter = 2) "didn't catch outer"

    [<Fact>]
    member __.testWhileLoopSync() =
        printfn "Running testWhileLoopSync..."
        let t =
            valuetaskDynamic {
                let mutable i = 0
                while i < 10 do
                    i <- i + 1
                return i
            }
        //wait t no wait required for sync loop
        require (t.IsCompleted) "didn't do sync while loop properly - not completed"
        require (t.Result = 10) "didn't do sync while loop properly - wrong result"

    [<Fact>]
    member __.testWhileLoopAsyncZeroIteration() =
        printfn "Running testWhileLoopAsyncZeroIteration..."
        for i in 1 .. 5 do 
            let t =
                valuetaskDynamic {
                    let mutable i = 0
                    while i < 0 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            wait t
            require (t.Result = 0) "didn't do while loop properly"

    [<Fact>]
    member __.testWhileLoopAsyncOneIteration() =
        printfn "Running testWhileLoopAsyncOneIteration..."
        for i in 1 .. 5 do 
            let t =
                valuetaskDynamic {
                    let mutable i = 0
                    while i < 1 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            wait t
            require (t.Result = 1) "didn't do while loop properly"

    [<Fact>]
    member __.testWhileLoopAsync() =
        printfn "Running testWhileLoopAsync..."
        for i in 1 .. 5 do 
            let t =
                valuetaskDynamic {
                    let mutable i = 0
                    while i < 10 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            wait t
            require (t.Result = 10) "didn't do while loop properly"

    [<Fact>]
    member __.testTryFinallyHappyPath() =
        printfn "Running testTryFinallyHappyPath..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                valuetaskDynamic {
                    try
                        require (not ran) "ran way early"
                        do! Task.Delay(100)
                        require (not ran) "ran kinda early"
                    finally
                        ran <- true
                }
            wait t
            require ran "never ran"
    [<Fact>]
    member __.testTryFinallySadPath() =
        printfn "Running testTryFinallySadPath..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                valuetaskDynamic {
                    try
                        require (not ran) "ran way early"
                        do! Task.Delay(100)
                        require (not ran) "ran kinda early"
                        failtest "uhoh"
                    finally
                        ran <- true
                }
            try
                wait t
            with
            | _ -> ()
            require ran "never ran"

    [<Fact>]
    member __.testTryFinallyCaught() =
        printfn "Running testTryFinallyCaught..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                valuetaskDynamic {
                    try
                        try
                            require (not ran) "ran way early"
                            do! Task.Delay(100)
                            require (not ran) "ran kinda early"
                            failtest "uhoh"
                        finally
                            ran <- true
                        return 1
                    with
                    | _ -> return 2
                }
            require (t.Result = 2) "wrong return"
            require ran "never ran"
    
    [<Fact>]
    member __.testUsing() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = false
            let t =
                valuetaskDynamic {
                    use d = { new IDisposable with member __.Dispose() = disposed <- true }
                    require (not disposed) "disposed way early"
                    do! Task.Delay(100)
                    require (not disposed) "disposed kinda early"
                }
            wait t
            require disposed "never disposed B"

    [<Fact>]
    member __.testUsingAsyncDisposableSync() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = 0
            let t =
                valuetaskDynamic {
                    use d = 
                        { new IAsyncDisposable with 
                            member __.DisposeAsync() = 
                                valuetaskDynamic { 
                                   System.Console.WriteLine "incrementing"
                                   disposed <- disposed + 1 }
                                |> unitvaluetask 
                        }
                    require (disposed = 0) "disposed way early"
                    System.Console.WriteLine "delaying"
                    do! Task.Delay(100)
                    System.Console.WriteLine "testing"
                    require (disposed = 0) "disposed kinda early"
                }
            wait t
            require (disposed >= 1) "never disposed B"
            require (disposed <= 1) "too many dispose on B"

    [<Fact>]
    member __.testUsingAsyncDisposableAsync() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = 0
            let t =
                valuetaskDynamic {
                    use d = 
                        { new IAsyncDisposable with 
                            member __.DisposeAsync() = 
                                valuetaskDynamic { 
                                    do! Task.Delay(10)
                                    disposed <- disposed + 1 
                                }
                                |> unitvaluetask
                        }
                    require (disposed = 0) "disposed way early"
                    do! Task.Delay(100)
                    require (disposed = 0) "disposed kinda early"
                }
            wait t
            require (disposed >= 1) "never disposed B"
            require (disposed <= 1) "too many dispose on B"

    [<Fact>]
    member __.testUsingAsyncDisposableExnAsync() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = 0
            let t =
                valuetaskDynamic {
                    use d = 
                        { new IAsyncDisposable with 
                            member __.DisposeAsync() = 
                                valuetaskDynamic { 
                                    do! Task.Delay(10)
                                    disposed <- disposed + 1 
                                }
                                |> unitvaluetask 
                        }
                    require (disposed = 0) "disposed way early"
                    failtest "oops"
                    
                }
            try wait t
            with | :? AggregateException -> 
                require (disposed >= 1) "never disposed B"
                require (disposed <= 1) "too many dispose on B"

    [<Fact>]
    member __.testUsingAsyncDisposableExnSync() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = 0
            let t =
                valuetaskDynamic {
                    use d = 
                        { new IAsyncDisposable with 
                            member __.DisposeAsync() = 
                                valuetaskDynamic { 
                                    disposed <- disposed + 1 
                                    do! Task.Delay(10)
                                }
                                |> unitvaluetask 
                        }
                    require (disposed = 0) "disposed way early"
                    failtest "oops"
                    
                }
            try wait t
            with | :? AggregateException -> 
                require (disposed >= 1) "never disposed B"
                require (disposed <= 1) "too many dispose on B"

    [<Fact>]
    member __.testUsingAsyncDisposableDelayExnSync() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = 0
            let t =
                valuetaskDynamic {
                    use d = 
                        { new IAsyncDisposable with 
                            member __.DisposeAsync() = 
                                valuetaskDynamic { 
                                    disposed <- disposed + 1 
                                    do! Task.Delay(10)
                                }
                                |> unitvaluetask 
                        }
                    require (disposed = 0) "disposed way early"
                    do! Task.Delay(10)
                    require (disposed = 0) "disposed kind of early"
                    failtest "oops"
                    
                }
            try wait t
            with | :? AggregateException -> 
                require (disposed >= 1) "never disposed B"
                require (disposed <= 1) "too many dispose on B"

    [<Fact>]
    member __.testUsingFromTask() =
        printfn "Running testUsingFromTask..."
        let mutable disposedInner = false
        let mutable disposed = false
        let t =
            valuetaskDynamic {
                use! d =
                    valuetaskDynamic {
                        do! Task.Delay(50)
                        use i = { new IDisposable with member __.Dispose() = disposedInner <- true }
                        require (not disposed && not disposedInner) "disposed inner early"
                        return { new IDisposable with member __.Dispose() = disposed <- true }
                    }
                require disposedInner "did not dispose inner after task completion"
                require (not disposed) "disposed way early"
                do! Task.Delay(50)
                require (not disposed) "disposed kinda early"
            }
        wait t
        require disposed "never disposed C"

    [<Fact>]
    member __.testUsingSadPath() =
        printfn "Running testUsingSadPath..."
        let mutable disposedInner = false
        let mutable disposed = false
        let t =
            valuetaskDynamic {
                try
                    use! d =
                        valuetaskDynamic {
                            do! Task.Delay(50)
                            use i = { new IDisposable with member __.Dispose() = disposedInner <- true }
                            failtest "uhoh"
                            require (not disposed && not disposedInner) "disposed inner early"
                            return { new IDisposable with member __.Dispose() = disposed <- true }
                        }
                    ()
                with
                | TestException msg ->
                    printfn "caught TestException"
                    require disposedInner "did not dispose inner after task completion"
                    require (not disposed) "disposed way early"
                    do! Task.Delay(50)
                    printfn "resumed after delay"
                    require (not disposed) "disposed kinda early"
            }
        wait t
        require (not disposed) "disposed thing that never should've existed"

    [<Fact>]
    member __.testForLoopA() =
        printfn "Running testForLoopA..."
        let list = ["a"; "b"; "c"] |> Seq.ofList
        let t =
            valuetaskDynamic {
                printfn "entering loop..." 
                let mutable x = Unchecked.defaultof<_>
                let e = list.GetEnumerator()
                while e.MoveNext() do 
                    x <- e.Current
                    printfn "x = %A" x 
                    do! Task.Yield()
                    printfn "x = %A" x 
            }
        wait t

    [<Fact>]
    member __.testForLoopComplex() =
        printfn "Running testForLoopComplex..."
        let mutable disposed = false
        let wrapList =
            let raw = ["a"; "b"; "c"] |> Seq.ofList
            let getEnumerator() =
                let raw = raw.GetEnumerator()
                { new IEnumerator<string> with
                    member __.MoveNext() =
                        require (not disposed) "moved next after disposal"
                        raw.MoveNext()
                    member __.Current =
                        require (not disposed) "accessed current after disposal"
                        raw.Current
                    member __.Current =
                        require (not disposed) "accessed current (boxed) after disposal"
                        box raw.Current
                    member __.Dispose() =
                        require (not disposed) "disposed twice"
                        disposed <- true
                        raw.Dispose()
                    member __.Reset() =
                        require (not disposed) "reset after disposal"
                        raw.Reset()
                }
            { new IEnumerable<string> with
                member __.GetEnumerator() : IEnumerator<string> = getEnumerator()
                member __.GetEnumerator() : IEnumerator = upcast getEnumerator()
            }
        let t =
            valuetaskDynamic {
                let mutable index = 0
                do! Task.Yield()
                printfn "entering loop..." 
                for x in wrapList do
                    printfn "x = %A, index = %d" x index
                    do! Task.Yield()
                    printfn "back from yield" 
                    do! Task.Yield()
                    printfn "back from yield" 
                    match index with
                    | 0 -> require (x = "a") "wrong first value"
                    | 1 -> require (x = "b") "wrong second value"
                    | 2 -> require (x = "c") "wrong third value"
                    | _ -> require false "iterated too far!"
                    index <- index + 1
                    printfn "yield again" 
                    do! Task.Yield()
                    printfn "yield again again" 
                    do! Task.Yield()
                    printfn "looping again..." 
                do! Task.Yield()
                return 1
            }
        wait t
        require disposed "never disposed D"
        require (t.Result = 1) "wrong result"

    [<Fact>]
    member __.testForLoopSadPath() =
        printfn "Running testForLoopSadPath..."
        for i in 1 .. 5 do 
            let wrapList = ["a"; "b"; "c"]
            let t =
                valuetaskDynamic {
                        let mutable index = 0
                        do! Task.Yield()
                        for x in wrapList do
                            do! Task.Yield()
                            index <- index + 1
                        return 1
                }
            require (t.Result = 1) "wrong result"

    [<Fact>]
    member __.testForLoopSadPathComplex() =
        printfn "Running testForLoopSadPathComplex..."
        for i in 1 .. 5 do 
            let mutable disposed = false
            let wrapList =
                let raw = ["a"; "b"; "c"] |> Seq.ofList
                let getEnumerator() =
                    let raw = raw.GetEnumerator()
                    { new IEnumerator<string> with
                        member __.MoveNext() =
                            require (not disposed) "moved next after disposal"
                            raw.MoveNext()
                        member __.Current =
                            require (not disposed) "accessed current after disposal"
                            raw.Current
                        member __.Current =
                            require (not disposed) "accessed current (boxed) after disposal"
                            box raw.Current
                        member __.Dispose() =
                            require (not disposed) "disposed twice"
                            disposed <- true
                            raw.Dispose()
                        member __.Reset() =
                            require (not disposed) "reset after disposal"
                            raw.Reset()
                    }
                { new IEnumerable<string> with
                    member __.GetEnumerator() : IEnumerator<string> = getEnumerator()
                    member __.GetEnumerator() : IEnumerator = upcast getEnumerator()
                }
            let mutable caught = false
            let t =
                valuetaskDynamic {
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
                    with
                    | TestException "uhoh" ->
                        caught <- true
                        return 2
                }
            require (t.Result = 2) "wrong result"
            require caught "didn't catch exception"
            require disposed "never disposed A"
    
    [<Fact>]
    member __.testExceptionAttachedToTaskWithoutAwait() =
        for i in 1 .. 5 do 
            let mutable ranA = false
            let mutable ranB = false
            let t =
                valuetaskDynamic {
                    ranA <- true
                    failtest "uhoh"
                    ranB <- true
                }
            require ranA "didn't run immediately"
            require (not ranB) "ran past exception"
            require (not (isNull (t.AsTask().Exception))) "didn't capture exception"
            require (t.AsTask().Exception.InnerExceptions.Count = 1) "captured more exceptions"
            require (t.AsTask().Exception.InnerException = TestException "uhoh") "wrong exception"
            let mutable caught = false
            let mutable ranCatcher = false
            let catcher =
                valuetaskDynamic {
                    try
                        ranCatcher <- true
                        let! result = t
                        return false
                    with
                    | TestException "uhoh" ->
                        caught <- true
                        return true
                }
            require ranCatcher "didn't run"
            require catcher.Result "didn't catch"
            require caught "didn't catch"

    [<Fact>]
    member __.testExceptionAttachedToTaskWithAwait() =
        printfn "running testExceptionAttachedToTaskWithAwait"
        for i in 1 .. 5 do 
            let mutable ranA = false
            let mutable ranB = false
            let t =
                valuetaskDynamic {
                    ranA <- true
                    failtest "uhoh"
                    do! Task.Delay(100)
                    ranB <- true
                }
            require ranA "didn't run immediately"
            require (not ranB) "ran past exception"
            require (not (isNull (t.AsTask().Exception))) "didn't capture exception"
            require (t.AsTask().Exception.InnerExceptions.Count = 1) "captured more exceptions"
            require (t.AsTask().Exception.InnerException = TestException "uhoh") "wrong exception"
            let mutable caught = false
            let mutable ranCatcher = false
            let catcher =
                valuetaskDynamic {
                    try
                        ranCatcher <- true
                        let! result = t
                        return false
                    with
                    | TestException "uhoh" ->
                        caught <- true
                        return true
                }
            require ranCatcher "didn't run"
            require catcher.Result "didn't catch"
            require caught "didn't catch"
    
    [<Fact>]
    member __.testExceptionThrownInFinally() =
        printfn "running testExceptionThrownInFinally"
        for i in 1 .. 5 do 
            let mutable ranInitial = false
            let mutable ranNext = false
            let mutable ranFinally = 0
            let t =
                valuetaskDynamic {
                    try
                        ranInitial <- true
                        do! Task.Yield()
                        Thread.Sleep(100) // shouldn't be blocking so we should get through to requires before this finishes
                        ranNext <- true
                    finally
                        ranFinally <- ranFinally + 1
                        failtest "finally exn!"
                }
            require ranInitial "didn't run initial"
            require (not ranNext) "ran next too early"
            try
                wait t
                require false "shouldn't get here"
            with
            | _ -> ()
            require ranNext "didn't run next"
            require (ranFinally = 1) "didn't run finally exactly once"

    [<Fact>]
    member __.test2ndExceptionThrownInFinally() =
        printfn "running test2ndExceptionThrownInFinally"
        for i in 1 .. 5 do 
            let mutable ranInitial = false
            let mutable ranNext = false
            let mutable ranFinally = 0
            let t =
                valuetaskDynamic {
                    try
                        ranInitial <- true
                        do! Task.Yield()
                        Thread.Sleep(100) // shouldn't be blocking so we should get through to requires before this finishes
                        ranNext <- true
                        failtest "uhoh"
                    finally
                        ranFinally <- ranFinally + 1
                        failtest "2nd exn!"
                }
            require ranInitial "didn't run initial"
            require (not ranNext) "ran next too early"
            try
                wait t
                require false "shouldn't get here"
            with
            | _ -> ()
            require ranNext "didn't run next"
            require (ranFinally = 1) "didn't run finally exactly once"
    
    [<Fact>]
    member __.testFixedStackWhileLoop() =
        printfn "running testFixedStackWhileLoop"
        for i in 1 .. 100 do 
            let t =
                valuetaskDynamic {
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
            wait t
            require (t.Result = BIG) "didn't get to big number"

    [<Fact>]
    member __.testFixedStackForLoop() =
        for i in 1 .. 100 do 
            printfn "running testFixedStackForLoop"
            let mutable ran = false
            let t =
                valuetaskDynamic {
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
            wait t
            require ran "didn't run all"

    [<Fact>]
    member __.testTypeInference() =
        let t1 : string ValueTask =
            valuetaskDynamic {
                return "hello"
            }
        let t2 =
            valuetaskDynamic {
                let! s = t1
                return s.Length
            }
        wait t2

    [<Fact>]
    member __.testNoStackOverflowWithImmediateResult() =
        printfn "running testNoStackOverflowWithImmediateResult"
        let longLoop =
            valuetaskDynamic {
                let mutable n = 0
                while n < BIG do
                    n <- n + 1
                    return! ValueTask.FromResult(())
            }
        wait longLoop
    
    [<Fact>]
    member __.testNoStackOverflowWithYieldResult() =
        printfn "running testNoStackOverflowWithYieldResult"
        let longLoop =
            valuetaskDynamic {
                let mutable n = 0
                while n < BIG do
                    let! _ =
                        valuetaskDynamic {
                            do! Task.Yield()
                            let! _ = ValueTask.FromResult(0)
                            n <- n + 1
                        }
                    n <- n + 1
            }
        wait longLoop

    [<Fact>]
    member __.testSmallTailRecursion() =
        printfn "running testSmallTailRecursion"
        let rec loop n =
            valuetaskDynamic {
                if n < 100 then
                    do! Task.Yield()
                    let! _ = ValueTask.FromResult(0)
                    return! loop (n + 1)
                else
                    return ()
            }
        let shortLoop =
            valuetaskDynamic {
                return! loop 0
            }
        wait shortLoop
    
    [<Fact>]
    member __.testTryOverReturnFrom() =
        printfn "running testTryOverReturnFrom"
        let inner() =
            valuetaskDynamic {
                do! Task.Yield()
                failtest "inner"
                return 1
            }
        let t =
            valuetaskDynamic {
                try
                    do! Task.Yield()
                    return! inner()
                with
                | TestException "inner" -> return 2
            }
        require (t.Result = 2) "didn't catch"

    [<Fact>]
    member __.testTryFinallyOverReturnFromWithException() =
        printfn "running testTryFinallyOverReturnFromWithException"
        let inner() =
            valuetaskDynamic {
                do! Task.Yield()
                failtest "inner"
                return 1
            }
        let mutable m = 0
        let t =
            valuetaskDynamic {
                try
                    do! Task.Yield()
                    return! inner()
                finally
                    m <- 1
            }
        try
            wait t
        with
        | :? AggregateException -> ()
        require (m = 1) "didn't run finally"
    
    [<Fact>]
    member __.testTryFinallyOverReturnFromWithoutException() =
        printfn "running testTryFinallyOverReturnFromWithoutException"
        let inner() =
            valuetaskDynamic {
                do! Task.Yield()
                return 1
            }
        let mutable m = 0
        let t =
            valuetaskDynamic {
                try
                    do! Task.Yield()
                    return! inner()
                finally
                    m <- 1
            }
        try
            wait t
        with
        | :? AggregateException -> ()
        require (m = 1) "didn't run finally"

    // no need to call this, we just want to check that it compiles w/o warnings
    member __.testTrivialReturnCompiles (x : 'a) : 'a ValueTask =
        valuetaskDynamic {
            do! Task.Yield()
            return x
        }

    // no need to call this, we just want to check that it compiles w/o warnings
    member __.testTrivialTransformedReturnCompiles (x : 'a) (f : 'a -> 'b) : 'b ValueTask =
        valuetaskDynamic {
            do! Task.Yield()
            return f x
        }

    [<Fact>]
    member __.testAsyncsMixedWithTasks() =
        let t =
            valuetaskDynamic {
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
    // no need to call this, we just want to check that it compiles w/o warnings
    member __.testDefaultInferenceForReturnFrom() =
        let t = valuetaskDynamic { return Some "x" }
        valuetaskDynamic {
            let! r = t
            if r = None then
                return! failwithf "Could not find x" 
            else
                return r
        }
        |> ignore

    [<Fact>]
    // no need to call this, just check that it compiles
    member __.testCompilerInfersArgumentOfReturnFrom() =
        valuetaskDynamic {
            if true then return 1
            else return! failwith ""
        }
        |> ignore


[<CollectionDefinition("BasicsNotInParallel", DisableParallelization = true)>]
type BasicsNotInParallel() = 

    [<Fact; >]
    member __.testTaskUsesSyncContext() =
        printfn "Running testBackgroundTask..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let mutable posted = false
            let oldSyncContext = SynchronizationContext.Current
            let syncContext = { new SynchronizationContext()  with member _.Post(d,state) = posted <- true; d.Invoke(state) }
            try 
                SynchronizationContext.SetSynchronizationContext syncContext
                let tid = System.Threading.Thread.CurrentThread.ManagedThreadId 
                require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread A"
                require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread A"
                let t =
                    valuetaskDynamic {
                        let tid2 = System.Threading.Thread.CurrentThread.ManagedThreadId 
                        require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread B"
                        require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread B"
                        require (tid = tid2) "expected synchronous start for task B2"
                        do! Task.Yield()
                        require (not (isNull SynchronizationContext.Current)) "need sync context non null on foreground thread C"
                        require (SynchronizationContext.Current = syncContext) "need sync context known on foreground thread C"
                        ran <- true
                    }
                wait t
                require ran "never ran"
                require posted "never posted"
            finally
                SynchronizationContext.SetSynchronizationContext oldSyncContext
                 
#if STANDALONE 
module M = 
  [<EntryPoint>]
  let main argv =
    printfn "Running tests..."
    try
        Basics().testShortCircuitResult()
        Basics().testDelay()
        Basics().testNoDelay()
        Basics().testNonBlocking()
        
        Basics().testCatching1()
        Basics().testCatching2()
        Basics().testNestedCatching()
        Basics().testWhileLoopSync()
        Basics().testWhileLoopAsyncZeroIteration()
        Basics().testWhileLoopAsyncOneIteration()
        Basics().testWhileLoopAsync()
        Basics().testTryFinallyHappyPath()
        Basics().testTryFinallySadPath()
        Basics().testTryFinallyCaught()
        Basics().testUsing()
        Basics().testUsingFromTask()
        Basics().testUsingSadPath()
        Basics().testForLoopA()
        Basics().testForLoopSadPath()
        Basics().testForLoopSadPathComplex()
        Basics().testExceptionAttachedToTaskWithoutAwait()
        Basics().testExceptionAttachedToTaskWithAwait()
        Basics().testExceptionThrownInFinally()
        Basics().test2ndExceptionThrownInFinally()
        Basics().testFixedStackWhileLoop()
        Basics().testFixedStackForLoop()
        Basics().testTypeInference()
        Basics().testNoStackOverflowWithImmediateResult()
        Basics().testNoStackOverflowWithYieldResult()
        ////// we don't support TCO, so large tail recursions will stack overflow
        ////// or at least use O(n) heap. but small ones should at least function OK.
        //testSmallTailRecursion()
        Basics().testTryOverReturnFrom()
        Basics().testTryFinallyOverReturnFromWithException()
        Basics().testTryFinallyOverReturnFromWithoutException()
        Basics().testAsyncsMixedWithTasks()
        printfn "Passed all tests!"
    with exn ->
        eprintfn "************************************"
        eprintfn "Exception: %O" exn
        printfn "Test failed... exiting..."
        eprintfn "************************************"
        exit 1
        
    printfn "Tests passed ok..., sleeping a bit in case there are background delayed exceptions"
    Thread.Sleep(500)
    printfn "Exiting..."
    //System.Console.ReadLine()
    0
#endif
#endif
