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

module Tests.TasksUsingCoroutines

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Tests.Coroutines
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

/// The extra data stored in ResumableStateMachine for tasks
[<Struct; NoComparison; NoEquality>]
type TaskStateMachineData2<'TOverall> =

    /// Holds the final result of the state machine (between the 'return' and the execution of the finally clauses, after which we we eventually call SetResult)
    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    /// When interpreted, holds the awaiter 
    [<DefaultValue(false)>]
    val mutable Awaiter : ICriticalNotifyCompletion

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'TOverall>

and TaskStateMachine2<'TOverall> = ResumableStateMachine<TaskStateMachineData2<'TOverall>>

and TaskStateMachineResumption2<'TOverall> = ResumptionFunc<TaskStateMachineData2<'TOverall>>

type TaskCode2<'TOverall, 'T> = ResumableCode<TaskStateMachineData2<'TOverall>, 'T>

[<AutoOpen>]
module TaskMethodRequire = 

    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2, 'TOverall 
                                when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode2<'TOverall, 'TResult2>) -> TaskCode2<'TOverall, 'TResult2>) > 
                             (priority: ^Priority)
                             (task: ^TaskLike)
                             (continuation: ^TResult1 -> TaskCode2<'TOverall, 'TResult2>) 
                             : TaskCode2<'TOverall, 'TResult2> = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * continuation: (^TResult1 -> TaskCode2<'TOverall, 'TResult2>) -> TaskCode2<'TOverall, 'TResult2>) (priority, task, continuation))

    
    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskCode2<'T, 'T>)> 
                             (priority: ^Priority)
                             (task: ^TaskLike) 
                             : TaskCode2<'T, 'T> = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskCode2<'T, 'T>) (priority, task))

type TaskBuilderCoroutines() =
    
    member inline _.Delay(f : unit -> TaskCode2<'TOverall, 'T>) : TaskCode2<'TOverall, 'T> =
        TaskCode2<'TOverall, 'T>(fun sm -> (f()).Invoke(&sm))

    member inline _.Run(code : TaskCode2<'TOverall, 'T>) : Task<'TOverall> = 
        if __useResumableCode then 
            __structStateMachine<TaskStateMachine2<'TOverall>, Task<'TOverall>>
                // IAsyncStateMachine.MoveNext
                (MoveNextMethodImpl<_>(fun sm -> 
                    if __useResumableCode then 
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint 
                        //if verbose then printfn $"[{sm.Id}] Run: resumable code, sm.ResumptionPoint = {sm.ResumptionPoint}"
                        try
                            let __stack_code_fin = code.Invoke(&sm)
                            if __stack_code_fin then
                                //if verbose then printfn $"[{sm.Id}] terminate"
                                sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                        with exn ->
                            sm.Data.MethodBuilder.SetException exn
                        //if verbose then printfn $"[{sm.Id}] done MoveNext, sm.ResumptionPoint = {sm.ResumptionPoint}"
                        //-- RESUMABLE CODE END
                    else
                        failwith "Run: non-resumable - unreachable"))

                // IAsyncStateMachine.SetStateMachine
                (SetStateMachineMethodImpl<_>(fun sm state -> 
                    sm.Data.MethodBuilder.SetStateMachine(state)))

                // Other interfaces (IResumableStateMachine)
                [| 
                   (typeof<IResumableStateMachine<TaskStateMachineData2<'TOverall>>>, "get_ResumptionPoint", GetResumptionPointMethodImpl<TaskStateMachine2<'TOverall>>(fun sm -> 
                        sm.ResumptionPoint) :> _);
                   (typeof<IResumableStateMachine<TaskStateMachineData2<'TOverall>>>, "get_Data", GetResumableStateMachineDataMethodImpl<TaskStateMachine2<'TOverall>, TaskStateMachineData2<'TOverall>>(fun sm -> 
                        sm.Data) :> _);
                   (typeof<IResumableStateMachine<TaskStateMachineData2<'TOverall>>>, "set_Data", SetResumableStateMachineDataMethodImpl<TaskStateMachine2<'TOverall>, TaskStateMachineData2<'TOverall>>(fun sm data -> 
                        sm.Data <- data) :> _);
                 |]

                // Start
                (AfterCode<TaskStateMachine2<'TOverall>,_>(fun sm -> 
                    sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
                    sm.Data.MethodBuilder.Start(&sm)
                    sm.Data.MethodBuilder.Task))
        else 
            TaskBuilderCoroutines.RunDynamic(code)

    static member RunDynamic(code : TaskCode2<'TOverall, 'T>) : Task<'TOverall> = 
        // TODO: the MoveNxt on TaskStateMachine2 (ResumableStateMachine) is not adequate
        // It djust does MoveNext and not this:
        //
        //try
        //    let step = sm.ResumptionFunc.Invoke(&sm) 
        //    if step then 
        //        sm.MethodBuilder.SetResult(sm.Result)
        //    else
        //        sm.MethodBuilder.AwaitUnsafeOnCompleted(&sm.Awaiter, &sm)
        //with exn ->
        //    sm.MethodBuilder.SetException exn

        let mutable sm = TaskStateMachine2<'TOverall>()
        sm.ResumptionFunc <- TaskStateMachineResumption2<_>(fun sm -> code.Invoke(&sm)) 
        sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
        sm.Data.MethodBuilder.Start(&sm)
        sm.Data.MethodBuilder.Task        

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : TaskCode2<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return (value: 'TOverall) : TaskCode2<'TOverall, 'T> = 
        TaskCode2<'TOverall, _>(fun sm -> 
            sm.Data.Result <- value
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine(task1: TaskCode2<'TOverall, unit>, task2: TaskCode2<'TOverall, 'T>) : TaskCode2<'TOverall, 'T> =
        ResumableCode.Combine(task1, task2)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While ([<InlineIfLambda>] condition : unit -> bool, body : TaskCode2<'TOverall, unit>) : TaskCode2<'TOverall, unit> =
        ResumableCode.While(condition, body)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith (body: TaskCode2<'TOverall, 'T>, catch: exn -> TaskCode2<'TOverall, 'T>) : TaskCode2<'TOverall, 'T> =
        ResumableCode.TryWith(body, catch)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally (body: TaskCode2<'TOverall, 'T>, [<InlineIfLambda>] compensation : unit -> unit) : TaskCode2<'TOverall, 'T> =
        ResumableCode.TryFinally(body, NonResumableCode<_,_>(fun _ -> compensation()))

    member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (resource : 'Resource, body : 'Resource -> TaskCode2<'TOverall, 'T>) : TaskCode2<'TOverall, 'T> = 
        ResumableCode.Using(resource, body)

    member inline _.For (sequence : seq<'T>, body : 'T -> TaskCode2<'TOverall, unit>) : TaskCode2<'TOverall, unit> =
        ResumableCode.For(sequence, body)

    member inline _.ReturnFrom (task: Task<'TOverall>) : TaskCode2<'TOverall, 'T> = 
        TaskCode2<'TOverall, _>(fun sm -> 
            //-- RESUMABLE CODE START
            // This becomes a state machine variable
            let mutable awaiter = task.GetAwaiter()

            let mutable __stack_fin = true
            if not task.IsCompleted then
                // This will yield with __stack_yield_fin = false
                // This will resume with __stack_yield_fin = true
                let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                __stack_fin <- __stack_yield_fin
            if __stack_fin then 
                sm.Data.Result <- awaiter.GetResult()
                true
            else
                sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                false
            //-- RESUMABLE CODE END
            )

[<AutoOpen>]
module TaskBuilder = 

    let taskUsingCoroutines = TaskBuilderCoroutines()

[<AutoOpen>]
module ContextSensitiveTasks = 
    [<Sealed>] 
    type TaskWitnesses() =

        interface IPriority1
        interface IPriority2
        interface IPriority3

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
                  (priority: IPriority2, task: ^TaskLike, continuation: (^TResult1 -> TaskCode2<'TOverall, 'TResult2>)) : TaskCode2<'TOverall, 'TResult2> =

            TaskCode2<'TOverall, _>(fun sm -> 
                //-- RESUMABLE CODE START
                ignore priority
                // Get an awaiter from the awaitable
                let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                let mutable __stack_fin = true
                if not (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                    // This will yield with __stack_yield_fin = false
                    // This will resume with __stack_yield_fin = true
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    __stack_fin <- __stack_yield_fin
                    
                if __stack_fin then 
                    let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                    (continuation result).Invoke(&sm)
                else
                    sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                    false
                //-- RESUMABLE CODE END
            )

        static member inline CanBind (priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode2<'TOverall, 'TResult2>)) : TaskCode2<'TOverall, 'TResult2> =

            TaskCode2<'TOverall, _>(fun sm -> 
                //-- RESUMABLE CODE START
                ignore priority
                // Get an awaiter from the task
                let mutable awaiter = task.GetAwaiter()

                let mutable __stack_fin = true
                if not awaiter.IsCompleted then
                    // This will yield with __stack_yield_fin = false
                    // This will resume with __stack_yield_fin = true
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    __stack_fin <- __stack_yield_fin
                if __stack_fin then 
                    let result = awaiter.GetResult()
                    (continuation result).Invoke(&sm)
                else
                    sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                    false
                //-- RESUMABLE CODE END
            )

        static member inline CanBind (priority: IPriority1, computation: Async<'TResult1>, continuation: ('TResult1 -> TaskCode2<'TOverall, 'TResult2>)) : TaskCode2<'TOverall, 'TResult2> =
            TaskWitnesses.CanBind (priority, Async.StartAsTask computation, continuation)

        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)>
              (priority: IPriority2, task: ^TaskLike) : TaskCode2< ^T,  ^T> =

            TaskCode2<_, _>(fun sm -> 
                //-- RESUMABLE CODE START
                ignore priority
                // Get an awaiter from the awaitable
                let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                let mutable __stack_fin = true
                if not (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                    // This will yield with __stack_yield_fin = false
                    // This will resume with __stack_yield_fin = true
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    __stack_fin <- __stack_yield_fin
                if __stack_fin then 
                    sm.Data.Result <- (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                    true
                else 
                    sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                    false
                //-- RESUMABLE CODE END
            )

        static member inline CanReturnFrom (priority: IPriority1, task: Task<'T>) : TaskCode2<'T, 'T> =

            TaskCode2<_, _>(fun sm -> 
                //-- RESUMABLE CODE START
                ignore priority
                let mutable awaiter = task.GetAwaiter()
                let mutable __stack_fin = true
                if not task.IsCompleted then
                    // This will yield with __stack_yield_fin = false
                    // This will resume with __stack_yield_fin = true
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    __stack_fin <- __stack_yield_fin
                if __stack_fin then 
                    sm.Data.Result <- awaiter.GetResult()
                    true
                else
                    sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                    false
                //-- RESUMABLE CODE END
            )

        static member inline CanReturnFrom (priority: IPriority1, computation: Async<'T>)  : TaskCode2<'T, 'T> =
            TaskWitnesses.CanReturnFrom (priority, Async.StartAsTask computation)

    [<AutoOpen>]
    module TaskHelpers = 

        type TaskBuilderCoroutines with

            member inline _.Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                               when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode2<'TOverall, 'TResult2>) -> TaskCode2<'TOverall, 'TResult2>)> 
                        (task: ^TaskLike, continuation: ^TResult1 -> TaskCode2<'TOverall, 'TResult2>)  : TaskCode2<'TOverall, 'TResult2> =

                RequireCanBind< TaskWitnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<TaskWitnesses> task continuation

            member inline _.ReturnFrom< ^TaskLike, 'T when (TaskWitnesses or ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskCode2<'T, 'T>) > 
                        (task: ^TaskLike)  : TaskCode2<'T, 'T> =

                RequireCanReturnFrom< TaskWitnesses, ^TaskLike, 'T> Unchecked.defaultof<TaskWitnesses> task

#if TEST
type ITaskThing =
    abstract member Taskify : 'a option -> 'a Task

type SmokeTestsForCompilation() =


    member _.tinyTask() =
        taskUsingCoroutines {
            return 1
        }
        |> ignore


    member _.tbind() =
        taskUsingCoroutines {
            let! x = Task.FromResult(1)
            return 1 + x
        }
        |> ignore


    member _.tnested() =
        taskUsingCoroutines {
            let! x = taskUsingCoroutines { return 1 }
            return x
        }
        |> ignore


    member _.tcatch0() =
        taskUsingCoroutines {
            try 
               return 1
            with e -> 
               return 2
        }
        |> ignore


    member _.tcatch1() =
        taskUsingCoroutines {
            try 
               let! x = Task.FromResult 1
               return x
            with e -> 
               return 2
        }
    
        |> ignore



    member _.t3() =
        let t2() =
            taskUsingCoroutines {
                System.Console.WriteLine("hello")
                return 1
            }
        taskUsingCoroutines {
            System.Console.WriteLine("hello")
            let! x = t2()
            System.Console.WriteLine("world")
            return 1 + x
        }
        |> ignore


    member _.t3b() =
        taskUsingCoroutines {
            System.Console.WriteLine("hello")
            let! x = Task.FromResult(1)
            System.Console.WriteLine("world")
            return 1 + x
        }
        |> ignore


    member _.t3c() =
        taskUsingCoroutines {
            System.Console.WriteLine("hello")
            do! Task.Delay(100)
            System.Console.WriteLine("world")
            return 1 
        }
        |> ignore


    // This tests an exception match
    member _.t67() =
        taskUsingCoroutines {
            try
                do! Task.Delay(0)
            with
            | :? ArgumentException -> 
                ()
            | _ -> 
                ()
        }
        |> ignore


    // This tests compiling an incomplete exception match
    member _.t68() =
        taskUsingCoroutines {
            try
                do! Task.Delay(0)
            with
            | :? ArgumentException -> 
                ()
        }
        |> ignore


    member _.testCompileAsyncWhileLoop() =
        taskUsingCoroutines {
            let mutable i = 0
            while i < 1 do
                i <- i + 1
                do! Task.Yield()
            return i
        }
        |> ignore


exception TestException of string

[<AutoOpen>]
module Helpers = 
    let BIG = 10
    // let BIG = 10000
    let require x msg = if not x then failwith msg
    let failtest str = raise (TestException str)

type Basics() = 

    member _.testShortCircuitResult() =
        printfn "Running testShortCircuitResult..."
        let t =
            taskUsingCoroutines {
                let! x = Task.FromResult(1)
                let! y = Task.FromResult(2)
                return x + y
            }
        require t.IsCompleted "didn't short-circuit already completed tasks"
        printfn "t.Result = %A" t.Result
        require (t.Result = 3) "wrong result"


    member _.testDelay() =
        printfn "Running testDelay..."
        let mutable x = 0
        let t =
            taskUsingCoroutines {
                do! Task.Delay(50)
                x <- x + 1
            }
        printfn "task created and first step run...."
        require (x = 0) "task already ran"
        printfn "waiting...."
        t.Wait()


    member _.testNoDelay() =
        printfn "Running testNoDelay..."
        let mutable x = 0
        let t =
            taskUsingCoroutines {
                x <- x + 1
                do! Task.Delay(5)
                x <- x + 1
            }
        require (x = 1) "first part didn't run yet"
        t.Wait()


    member _.testNonBlocking() =
        printfn "Running testNonBlocking..."
        let sw = Stopwatch()
        sw.Start()
        let t =
            taskUsingCoroutines {
                do! Task.Yield()
                Thread.Sleep(100)
            }
        sw.Stop()
        require (sw.ElapsedMilliseconds < 50L) "sleep blocked caller"
        t.Wait()


    member _.testCatching1() =
        printfn "Running testCatching1..."
        let mutable x = 0
        let mutable y = 0
        let t =
            taskUsingCoroutines {
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
        t.Wait()
        require (y = 1) "bailed after exn"
        require (x = 0) "ran past failure"


    member _.testCatching2() =
        printfn "Running testCatching2..."
        let mutable x = 0
        let mutable y = 0
        let t =
            taskUsingCoroutines {
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
        t.Wait()
        require (y = 1) "bailed after exn"
        require (x = 0) "ran past failure"


    member _.testNestedCatching() =
        printfn "Running testNestedCatching..."
        let mutable counter = 1
        let mutable caughtInner = 0
        let mutable caughtOuter = 0
        let t1() =
            taskUsingCoroutines {
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
            taskUsingCoroutines {
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
            t2.Wait()
            require false "ran past failed task wait"
        with
        | :? AggregateException as exn ->
            require (exn.InnerExceptions.Count = 1) "more than 1 exn"
        require (caughtInner = 1) "didn't catch inner"
        require (caughtOuter = 2) "didn't catch outer"


    member _.testWhileLoopSync() =
        printfn "Running testWhileLoopSync..."
        let t =
            taskUsingCoroutines {
                let mutable i = 0
                while i < 10 do
                    i <- i + 1
                return i
            }
        //t.Wait() no wait required for sync loop
        require (t.IsCompleted) "didn't do sync while loop properly - not completed"
        require (t.Result = 10) "didn't do sync while loop properly - wrong result"


    member _.testWhileLoopAsyncZeroIteration() =
        printfn "Running testWhileLoopAsyncZeroIteration..."
        for i in 1 .. 5 do 
            let t =
                taskUsingCoroutines {
                    let mutable i = 0
                    while i < 0 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            t.Wait()
            require (t.Result = 0) "didn't do while loop properly"


    member _.testWhileLoopAsyncOneIteration() =
        printfn "Running testWhileLoopAsyncOneIteration..."
        for i in 1 .. 5 do 
            let t =
                taskUsingCoroutines {
                    let mutable i = 0
                    while i < 1 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            t.Wait()
            require (t.Result = 1) "didn't do while loop properly"


    member _.testWhileLoopAsync() =
        printfn "Running testWhileLoopAsync..."
        for i in 1 .. 5 do 
            let t =
                taskUsingCoroutines {
                    let mutable i = 0
                    while i < 10 do
                        i <- i + 1
                        do! Task.Yield()
                    return i
                }
            t.Wait()
            require (t.Result = 10) "didn't do while loop properly"


    member _.testTryFinallyHappyPath() =
        printfn "Running testTryFinallyHappyPath..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                taskUsingCoroutines {
                    try
                        require (not ran) "ran way early"
                        do! Task.Delay(100)
                        require (not ran) "ran kinda early"
                    finally
                        ran <- true
                }
            t.Wait()
            require ran "never ran"

    member _.testTryFinallySadPath() =
        printfn "Running testTryFinallySadPath..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                taskUsingCoroutines {
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
            with
            | _ -> ()
            require ran "never ran"


    member _.testTryFinallyCaught() =
        printfn "Running testTryFinallyCaught..."
        for i in 1 .. 5 do 
            let mutable ran = false
            let t =
                taskUsingCoroutines {
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
    

    member _.testUsing() =
        printfn "Running testUsing..."
        for i in 1 .. 5 do 
            let mutable disposed = false
            let t =
                taskUsingCoroutines {
                    use d = { new IDisposable with member _.Dispose() = disposed <- true }
                    require (not disposed) "disposed way early"
                    do! Task.Delay(100)
                    require (not disposed) "disposed kinda early"
                }
            t.Wait()
            require disposed "never disposed B"


    member _.testUsingFromTask() =
        printfn "Running testUsingFromTask..."
        let mutable disposedInner = false
        let mutable disposed = false
        let t =
            taskUsingCoroutines {
                use! d =
                    taskUsingCoroutines {
                        do! Task.Delay(50)
                        use i = { new IDisposable with member _.Dispose() = disposedInner <- true }
                        require (not disposed && not disposedInner) "disposed inner early"
                        return { new IDisposable with member _.Dispose() = disposed <- true }
                    }
                require disposedInner "did not dispose inner after task completion"
                require (not disposed) "disposed way early"
                do! Task.Delay(50)
                require (not disposed) "disposed kinda early"
            }
        t.Wait()
        require disposed "never disposed C"


    member _.testUsingSadPath() =
        printfn "Running testUsingSadPath..."
        let mutable disposedInner = false
        let mutable disposed = false
        let t =
            taskUsingCoroutines {
                try
                    use! d =
                        taskUsingCoroutines {
                            do! Task.Delay(50)
                            use i = { new IDisposable with member _.Dispose() = disposedInner <- true }
                            failtest "uhoh"
                            require (not disposed && not disposedInner) "disposed inner early"
                            return { new IDisposable with member _.Dispose() = disposed <- true }
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
        t.Wait()
        require (not disposed) "disposed thing that never should've existed"


    member _.testForLoopA() =
        printfn "Running testForLoopA..."
        let list = ["a"; "b"; "c"] |> Seq.ofList
        let t =
            taskUsingCoroutines {
                printfn "entering loop..." 
                let mutable x = Unchecked.defaultof<_>
                let e = list.GetEnumerator()
                while e.MoveNext() do 
                    x <- e.Current
                    printfn "x = %A" x 
                    do! Task.Yield()
                    printfn "x = %A" x 
            }
        t.Wait()


    member _.testForLoopComplex() =
        printfn "Running testForLoopComplex..."
        let mutable disposed = false
        let wrapList =
            let raw = ["a"; "b"; "c"] |> Seq.ofList
            let getEnumerator() =
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
                member _.GetEnumerator() : IEnumerator<string> = getEnumerator()
                member _.GetEnumerator() : IEnumerator = upcast getEnumerator()
            }
        let t =
            taskUsingCoroutines {
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
        t.Wait()
        require disposed "never disposed D"
        require (t.Result = 1) "wrong result"


    member _.testForLoopSadPath() =
        printfn "Running testForLoopSadPath..."
        for i in 1 .. 5 do 
            let wrapList = ["a"; "b"; "c"]
            let t =
                taskUsingCoroutines {
                        let mutable index = 0
                        do! Task.Yield()
                        for x in wrapList do
                            do! Task.Yield()
                            index <- index + 1
                        return 1
                }
            require (t.Result = 1) "wrong result"


    member _.testForLoopSadPathComplex() =
        printfn "Running testForLoopSadPathComplex..."
        for i in 1 .. 5 do 
            let mutable disposed = false
            let wrapList =
                let raw = ["a"; "b"; "c"] |> Seq.ofList
                let getEnumerator() =
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
                    member _.GetEnumerator() : IEnumerator<string> = getEnumerator()
                    member _.GetEnumerator() : IEnumerator = upcast getEnumerator()
                }
            let mutable caught = false
            let t =
                taskUsingCoroutines {
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
    

    member _.testExceptionAttachedToTaskWithoutAwait() =
        for i in 1 .. 5 do 
            let mutable ranA = false
            let mutable ranB = false
            let t =
                taskUsingCoroutines {
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
                taskUsingCoroutines {
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


    member _.testExceptionAttachedToTaskWithAwait() =
        printfn "running testExceptionAttachedToTaskWithAwait"
        for i in 1 .. 5 do 
            let mutable ranA = false
            let mutable ranB = false
            let t =
                taskUsingCoroutines {
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
                taskUsingCoroutines {
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
    

    member _.testExceptionThrownInFinally() =
        printfn "running testExceptionThrownInFinally"
        for i in 1 .. 5 do 
            let mutable ranInitial = false
            let mutable ranNext = false
            let mutable ranFinally = 0
            let t =
                taskUsingCoroutines {
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
                t.Wait()
                require false "shouldn't get here"
            with
            | _ -> ()
            require ranNext "didn't run next"
            require (ranFinally = 1) "didn't run finally exactly once"


    member _.test2ndExceptionThrownInFinally() =
        printfn "running test2ndExceptionThrownInFinally"
        for i in 1 .. 5 do 
            let mutable ranInitial = false
            let mutable ranNext = false
            let mutable ranFinally = 0
            let t =
                taskUsingCoroutines {
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
                t.Wait()
                require false "shouldn't get here"
            with
            | _ -> ()
            require ranNext "didn't run next"
            require (ranFinally = 1) "didn't run finally exactly once"
    

    member _.testFixedStackWhileLoop() =
        printfn "running testFixedStackWhileLoop"
        for i in 1 .. 100 do 
            let t =
                taskUsingCoroutines {
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


    member _.testFixedStackForLoop() =
        for i in 1 .. 100 do 
            printfn "running testFixedStackForLoop"
            let mutable ran = false
            let t =
                taskUsingCoroutines {
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


    member _.testTypeInference() =
        let t1 : string Task =
            taskUsingCoroutines {
                return "hello"
            }
        let t2 =
            taskUsingCoroutines {
                let! s = t1
                return s.Length
            }
        t2.Wait()


    member _.testNoStackOverflowWithImmediateResult() =
        printfn "running testNoStackOverflowWithImmediateResult"
        let longLoop =
            taskUsingCoroutines {
                let mutable n = 0
                while n < BIG do
                    n <- n + 1
                    return! Task.FromResult(())
            }
        longLoop.Wait()
    

    member _.testNoStackOverflowWithYieldResult() =
        printfn "running testNoStackOverflowWithYieldResult"
        let longLoop =
            taskUsingCoroutines {
                let mutable n = 0
                while n < BIG do
                    let! _ =
                        taskUsingCoroutines {
                            do! Task.Yield()
                            let! _ = Task.FromResult(0)
                            n <- n + 1
                        }
                    n <- n + 1
            }
        longLoop.Wait()


    member _.testSmallTailRecursion() =
        printfn "running testSmallTailRecursion"
        let rec loop n =
            taskUsingCoroutines {
                if n < 100 then
                    do! Task.Yield()
                    let! _ = Task.FromResult(0)
                    return! loop (n + 1)
                else
                    return ()
            }
        let shortLoop =
            taskUsingCoroutines {
                return! loop 0
            }
        shortLoop.Wait()
    

    member _.testTryOverReturnFrom() =
        printfn "running testTryOverReturnFrom"
        let inner() =
            taskUsingCoroutines {
                do! Task.Yield()
                failtest "inner"
                return 1
            }
        let t =
            taskUsingCoroutines {
                try
                    do! Task.Yield()
                    return! inner()
                with
                | TestException "inner" -> return 2
            }
        require (t.Result = 2) "didn't catch"


    member _.testTryFinallyOverReturnFromWithException() =
        printfn "running testTryFinallyOverReturnFromWithException"
        let inner() =
            taskUsingCoroutines {
                do! Task.Yield()
                failtest "inner"
                return 1
            }
        let mutable m = 0
        let t =
            taskUsingCoroutines {
                try
                    do! Task.Yield()
                    return! inner()
                finally
                    m <- 1
            }
        try
            t.Wait()
        with
        | :? AggregateException -> ()
        require (m = 1) "didn't run finally"
    

    member _.testTryFinallyOverReturnFromWithoutException() =
        printfn "running testTryFinallyOverReturnFromWithoutException"
        let inner() =
            taskUsingCoroutines {
                do! Task.Yield()
                return 1
            }
        let mutable m = 0
        let t =
            taskUsingCoroutines {
                try
                    do! Task.Yield()
                    return! inner()
                finally
                    m <- 1
            }
        try
            t.Wait()
        with
        | :? AggregateException -> ()
        require (m = 1) "didn't run finally"

    // no need to call this, we just want to check that it compiles w/o warnings
    member _.testTrivialReturnCompiles (x : 'a) : 'a Task =
        taskUsingCoroutines {
            do! Task.Yield()
            return x
        }

    // no need to call this, we just want to check that it compiles w/o warnings
    member _.testTrivialTransformedReturnCompiles (x : 'a) (f : 'a -> 'b) : 'b Task =
        taskUsingCoroutines {
            do! Task.Yield()
            return f x
        }


    member _.testAsyncsMixedWithTasks() =
        let t =
            taskUsingCoroutines {
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


    // no need to call this, we just want to check that it compiles w/o warnings
    member _.testDefaultInferenceForReturnFrom() =
        let t = taskUsingCoroutines { return Some "x" }
        taskUsingCoroutines {
            let! r = t
            if r = None then
                return! failwithf "Could not find x" 
            else
                return r
        }
        |> ignore


    // no need to call this, just check that it compiles
    member _.testCompilerInfersArgumentOfReturnFrom() =
        taskUsingCoroutines {
            if true then return 1
            else return! failwith ""
        }
        |> ignore


module M = 
    printfn "Running tests..."
    try
//        Basics().testShortCircuitResult()
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
#endif
