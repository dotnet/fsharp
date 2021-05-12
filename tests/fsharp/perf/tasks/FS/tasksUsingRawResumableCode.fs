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

module Tests.TasksUsingRawResumableCode

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

/// Acts as a template for struct state machines introduced by __structStateMachine, and also as a reflective implementation
[<Struct; NoComparison; NoEquality>]
type TaskStateMachine<'TOverall> =

    /// Holds the final result of the state machine (between the 'return' and the execution of the finally clauses, after which we we eventually call SetResult)
    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint : int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc : TaskStateMachineResumption<'TOverall>

    /// When interpreted, holds the awaiter 
    [<DefaultValue(false)>]
    val mutable Awaiter : ICriticalNotifyCompletion

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'TOverall>

    //member sm.Address = 0n
        //let addr = &&sm.ResumptionPoint
        //Microsoft.FSharp.NativeInterop.NativePtr.toNativeInt addr

    interface IAsyncStateMachine with 
        
        // Used when interpreted.  For "__structStateMachine" it is replaced.
        member sm.MoveNext() = 
            try
                let step = sm.ResumptionFunc.Invoke(&sm) 
                if step then 
                    sm.MethodBuilder.SetResult(sm.Result)
                else
                    sm.MethodBuilder.AwaitUnsafeOnCompleted(&sm.Awaiter, &sm)
            with exn ->
                sm.MethodBuilder.SetException exn

        // Used when interpreted.  For "__structStateMachine" it is replaced.
        member sm.SetStateMachine(state) = 
            sm.MethodBuilder.SetStateMachine(state)

and TaskStateMachineResumption<'TOverall> = delegate of byref<TaskStateMachine<'TOverall>> -> bool

/// Represents a code fragment of a task.  When statically compiled, TaskCode is always removed and the body of the code inlined
/// into an invocation.
[<ResumableCode>]
type TaskCode<'TOverall, 'T> = delegate of byref<TaskStateMachine<'TOverall>> -> bool

[<AutoOpen>]
module TaskMethodRequire = 

    
    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2, 'TOverall 
                                when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) > 
                             (priority: ^Priority)
                             (task: ^TaskLike)
                             (continuation: ^TResult1 -> TaskCode<'TOverall, 'TResult2>) 
                             : TaskCode<'TOverall, 'TResult2> = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) (priority, task, continuation))

    
    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskCode<'T, 'T>)> 
                             (priority: ^Priority)
                             (task: ^TaskLike) 
                             : TaskCode<'T, 'T> = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskCode<'T, 'T>) (priority, task))

type TaskBuilder() =
    
    member inline _.Delay(f : unit -> TaskCode<'TOverall, 'T>) : TaskCode<_,_> =
        TaskCode<'TOverall, 'T>(fun sm -> (f()).Invoke(&sm))

    member inline _.Run(code : TaskCode<'TOverall, 'TOverall>) : Task<'TOverall> = 
        if __useResumableCode then 
            __structStateMachine<TaskStateMachine<'TOverall>, Task<'TOverall>>
                // IAsyncStateMachine.MoveNext
                (MoveNextMethodImpl<_>(fun sm -> 
                    if __useResumableCode then 
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint 
                        try
                            let __stack_step = code.Invoke(&sm)
                            if __stack_step then 
                                sm.MethodBuilder.SetResult(sm.Result)
                        with exn ->
                            sm.MethodBuilder.SetException exn
                        //-- RESUMABLE CODE END
                    else
                        failwith "unreachable"))

                // IAsyncStateMachine.SetStateMachine
                (SetStateMachineMethodImpl<_>(fun sm state -> 
                    sm.MethodBuilder.SetStateMachine(state)))

                // Other interfaces
                [| |]

                // After code
                (AfterCode<_,_>(fun sm -> 
                    sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
                    sm.MethodBuilder.Start(&sm)
                    sm.MethodBuilder.Task))
        else
            TaskBuilder.RunDynamic(code)

    static member RunDynamic(task : TaskCode<'TOverall, 'TOverall>) : Task<'TOverall> = 
        let mutable sm = TaskStateMachine<'TOverall>()
        sm.ResumptionFunc <- TaskStateMachineResumption<_>(fun sm -> task.Invoke(&sm)) 
        sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
        sm.MethodBuilder.Start(&sm)
        sm.MethodBuilder.Task        

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : TaskCode<'TOverall, unit> =
        TaskCode<_, unit>(fun sm ->
            true)

    member inline _.Return (value: 'TOverall) : TaskCode<'TOverall, 'TOverall> = 
        TaskCode<_, _>(fun sm -> 
            sm.Result <- value
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine(task1: TaskCode<'TOverall, unit>, task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then
                //-- RESUMABLE CODE START
                // NOTE: The code for task1 may contain await points! Resuming may branch directly
                // into this code!
                let __stack_step = task1.Invoke(&sm)
                if __stack_step then 
                    task2.Invoke(&sm)
                else
                    false
                //-- RESUMABLE CODE END
            else
                TaskBuilder.CombineDynamic(task1, task2).Invoke(&sm))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    static member CombineDynamic(task1: TaskCode<'TOverall, unit>, task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if task1.Invoke(&sm) then 
                task2.Invoke(&sm)
            else
                let rec resume (mf: TaskStateMachineResumption<_>) =
                    TaskStateMachineResumption<_>(fun sm -> 
                        if mf.Invoke(&sm) then 
                            task2.Invoke(&sm)
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false)

                sm.ResumptionFunc <- resume sm.ResumptionFunc
                false)

    member inline _.WhileAsync([<InlineIfLambda>] condition : unit -> Task<bool>, body : TaskCode<'TOverall,unit>) : TaskCode<'TOverall,unit> =
        TaskCode<'TOverall,unit>(fun sm -> 
            if __useResumableCode then
                //-- RESUMABLE CODE START
                let mutable __stack_step = false
                let mutable __stack_proceed = true
                while __stack_proceed do
                    let __stack_guard = condition()
                    if __stack_guard.IsCompleted then
                        __stack_proceed <- __stack_guard.Result
                    else
                        // Async wait for guard task
                        let mutable awaiter = __stack_guard.GetAwaiter() // **
                        match __resumableEntry() with 
                        | Some contID ->
                            if awaiter.IsCompleted then 
                                __resumeAt contID
                            else
                                sm.ResumptionPoint <- contID
                                sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                                __stack_proceed <- false
                        | None ->
                            // Label contID: 
                            __stack_proceed <- awaiter.GetResult()

                    if __stack_proceed then
                        let __stack_step2 = body.Invoke(&sm)
                        __stack_step <- __stack_step2
                        __stack_proceed <- __stack_step2 
                __stack_step
                //-- RESUMABLE CODE END
            else
                failwith "reflective execution of WhileAsync NYI")

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While ([<InlineIfLambda>] condition : unit -> bool, body : TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_completed = true 
                while __stack_completed && condition() do
                    __stack_completed <- false 
                    // NOTE: The body of the state machine code for 'while' may contain await points, so resuming
                    // the code will branch directly into the expanded 'body', branching directly into the while loop
                    let __stack_step = body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step 
                    __stack_completed <- __stack_step
                __stack_completed
                //-- RESUMABLE CODE END
            else
                TaskBuilder.WhileDynamic(condition, body).Invoke(&sm))

    static member WhileDynamic (condition: unit -> bool, body: TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        TaskCode<_, _>(fun sm ->
            let rec repeat() = 
                TaskStateMachineResumption<_>(fun sm -> 
                    if condition() then 
                        if body.Invoke (&sm) then
                            repeat().Invoke(&sm)
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    else
                        true)
            and resume (mf: TaskStateMachineResumption<_>) =
                TaskStateMachineResumption<_>(fun sm -> 
                    let step = mf.Invoke(&sm)
                    if step then 
                        repeat().Invoke(&sm)
                    else
                        sm.ResumptionFunc <- resume sm.ResumptionFunc
                        false)

            repeat().Invoke(&sm))

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith (body: TaskCode<'TOverall, 'T>, catch: exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_completed = false
                let mutable __stack_caught = false
                let mutable __stack_savedExn = Unchecked.defaultof<_>
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/with for code as we enter into the handler.
                __stack_completed <- __stack_completed || __stack_completed
                try
                    // The try block may contain await points.
                    let __stack_step = body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step
                    __stack_completed <- __stack_step
                with exn -> 
                    // Note, remarkExpr in the F# compiler detects this pattern as the code
                    // is inlined and elides the debug sequence point on either the 'compensation'
                    // or 'reraise' statement for the code. This is because the inlining will associate
                    // the sequence point with the 'try' of the TryFinally because that is the range
                    // given for the whole expression 
                    //      task.TryWith(....) 
                    // If you change this code you should check debug sequence points and the generated
                    // code tests for try/with in tasks.
                    __stack_caught <- true
                    __stack_savedExn <- exn

                if __stack_caught then 
                    // Place the catch code outside the catch block 
                    (catch __stack_savedExn).Invoke(&sm)
                else
                    __stack_completed
                //-- RESUMABLE CODE END

            else
                TaskBuilder.TryWithDynamic(body, catch).Invoke(&sm))

    static member TryWithDynamic (body: TaskCode<'TOverall, 'T>, handler: exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            let rec resume (mf: TaskStateMachineResumption<_>) =
                TaskStateMachineResumption<_>(fun sm -> 
                    try
                        if mf.Invoke (&sm) then 
                            true
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    with exn -> 
                        (handler exn).Invoke(&sm))
            try
                let step = body.Invoke (&sm)
                if not step then 
                    sm.ResumptionFunc <- sm.ResumptionFunc
                step
                        
            with exn -> 
                (handler exn).Invoke(&sm))

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally (body: TaskCode<'TOverall, 'T>, [<InlineIfLambda>] compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_completed = false
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/finally. The 'try' is used as the range for the
                // F# computation expression desugaring to 'TryFinally' and this range in turn gets applied
                // to inlined code.
                __stack_completed <- __stack_completed || __stack_completed
                try
                    let __stack_step = body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                    // may skip this step.
                    __stack_completed <- __stack_step
                with _ ->
                    // Note, remarkExpr in the F# compiler detects this pattern as the code
                    // is inlined and elides the debug sequence point on either the 'compensation'
                    // or 'reraise' statement for the code. This is because the inlining will associate
                    // the sequence point with the 'try' of the TryFinally because that is the range
                    // given for the whole expression 
                    //      task.TryFinally(....) 
                    // If you change this code you should check debug sequence points and the generated
                    // code tests for try/finally in tasks.
                    compensation()
                    reraise()

                if __stack_completed then 
                    compensation()
                __stack_completed
                //-- RESUMABLE CODE END
            else
                TaskBuilder.TryFinallyDynamic(body, compensation).Invoke(&sm))

    static member TryFinallyDynamic (body: TaskCode<'TOverall, 'T>, compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            let rec resume (mf: TaskStateMachineResumption<_>) =
                TaskStateMachineResumption<_>(fun sm -> 
                    let mutable completed = false
                    try
                        completed <- mf.Invoke (&sm)
                        if not completed then 
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                    with _ ->
                        compensation()
                        reraise()
                    if completed then 
                        compensation()
                    completed)

            let mutable completed = false
            try
                completed <- body.Invoke (&sm)
                if not completed then 
                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                       
            with _ ->
                compensation()
                reraise()

            if completed then 
                compensation()

            completed)

    member inline builder.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (resource : 'Resource, body : 'Resource -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        builder.TryFinally(
            TaskCode<_, _>(fun sm -> (body resource).Invoke(&sm)),
            (fun () -> if not (isNull (box resource)) then resource.Dispose()))

    member inline builder.For (sequence : seq<'T>, body : 'T -> TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        builder.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> builder.While((fun () -> e.MoveNext()), TaskCode<_, _>(fun sm -> (body e.Current).Invoke(&sm)))))

    member inline _.ReturnFrom (task: Task<'T>) : TaskCode<'T, 'T> = 
        TaskCode<_, _>(fun sm -> 
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                // This becomes a state machine variable
                let mutable awaiter = task.GetAwaiter()

                match __resumableEntry() with 
                | Some contID ->
                    // shortcut to continue immediately
                    if task.IsCompleted then
                        __resumeAt contID
                    else
                        sm.ResumptionPoint <- contID
                        sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                        false
                | None ->
                    // Label contID: 
                    sm.Result <- awaiter.GetResult()
                    true
                //-- RESUMABLE CODE END
            else
                TaskBuilder.ReturnFromDynamic(task).Invoke(&sm))

    static member ReturnFromDynamic (task: Task<'T>) : TaskCode<'T, 'T> = 
        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let cont =
                TaskStateMachineResumption<'T>(fun sm -> 
                    sm.Result <- awaiter.GetResult()
                    true)

            // shortcut to continue immediately
            if task.IsCompleted then
                cont.Invoke(&sm)
            else
                // If the task definition has not been converted to a state machine then a continuation function is used
                sm.ResumptionFunc <- cont
                sm.Awaiter <- awaiter
                false)

[<AutoOpen>]
module TaskBuilder = 

    let taskUsingRawResumableCode = TaskBuilder()

[<AutoOpen>]
module ContextSensitiveTasks = 
    [<Sealed>] 
    type TaskWitnesses() =

        interface IPriority1
        interface IPriority2
        interface IPriority3

        static member inline CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
                  (priority: IPriority2, task: ^TaskLike, continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

            TaskCode<'TOverall, 'TResult2>(fun sm -> 
                ignore priority
                let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                let cont = 
                    (TaskStateMachineResumption<'TOverall>( fun sm -> 
                        let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                        (continuation result).Invoke(&sm)))

                // shortcut to continue immediately
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                    cont.Invoke(&sm)
                else
                    sm.Awaiter <- awaiter
                    sm.ResumptionFunc <- cont
                    false)

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
                  (priority: IPriority2, task: ^TaskLike, continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
                    // Get an awaiter from the awaitable
                    let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                    match __resumableEntry() with
                    | Some contID ->
                        // shortcut to continue immediately
                        if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                            __resumeAt contID
                        else
                            sm.ResumptionPoint <- contID
                            sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                            false
                    | None -> 
                        // Label contID: 
                        let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                        (continuation result).Invoke(&sm)
                else
                    TaskWitnesses.CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall>(priority, task, continuation).Invoke(&sm))

        static member inline CanBindDynamic (priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
            TaskCode<_, _>(fun sm -> 
                ignore priority
                let mutable awaiter = task.GetAwaiter()

                let cont = 
                    (TaskStateMachineResumption<'TOverall>(fun sm -> 
                        //Console.WriteLine("[{0}] resumed CanBind(Task)", sm.MethodBuilder.Task.Id)
                        let result = awaiter.GetResult()
                        (continuation result).Invoke(&sm)))

                // shortcut to continue immediately
                if awaiter.IsCompleted then 
                    cont.Invoke(&sm)
                else
                    sm.Awaiter <- awaiter
                    sm.ResumptionFunc <- cont
                    false)

        static member inline CanBind (priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
                    // Get an awaiter from the task
                    let mutable awaiter = task.GetAwaiter()

                    match __resumableEntry() with
                    | Some contID ->
                        if awaiter.IsCompleted then 
                            __resumeAt contID
                        else
                            sm.ResumptionPoint <- contID
                            sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                            false
                    | None ->
                        // Label contID: 
                        let result = awaiter.GetResult()
                        (continuation result).Invoke(&sm)
                else
                    TaskWitnesses.CanBindDynamic(priority, task, continuation).Invoke(&sm))

        static member inline CanBind (priority: IPriority1, computation: Async<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
            TaskWitnesses.CanBind (priority, Async.StartAsTask computation, continuation)

        static member inline CanReturnFromDynamic< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)>
              (priority: IPriority2, task: ^TaskLike) : TaskCode< ^T, ^T > =

            TaskCode<_, _>(fun sm -> 
                ignore priority
                let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                let cont =
                    (TaskStateMachineResumption< ^T >(fun sm -> 
                        sm.Result <- (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                        true))

                // shortcut to continue immediately
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                    cont.Invoke(&sm)
                else
                    sm.Awaiter <- awaiter
                    sm.ResumptionFunc <- cont
                    false)

        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)>
              (priority: IPriority2, task: ^TaskLike) : TaskCode< ^T, ^T > =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
                    // Get an awaiter from the awaitable
                    let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                    match __resumableEntry() with
                    | Some contID ->
                        // shortcut to continue immediately
                        if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                            __resumeAt contID
                        else
                            sm.ResumptionPoint <- contID
                            sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                            false
                    | None ->
                        // Label contID: 
                        sm.Result <- (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                        true
                else
                    TaskWitnesses.CanReturnFromDynamic(priority, task).Invoke(&sm))

        static member inline CanReturnFrom (priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
                    let mutable awaiter = task.GetAwaiter()

                    match __resumableEntry() with
                    | Some contID -> 
                        // shortcut to continue immediately
                        if task.IsCompleted then
                            __resumeAt contID
                        else
                            sm.ResumptionPoint <- contID
                            sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                            false
                    | None ->
                        sm.Result <- awaiter.GetResult()
                        true
                else
                    TaskWitnesses.CanReturnFromDynamic(priority, task).Invoke(&sm))

        static member CanReturnFromDynamic (priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

            TaskCode<_, _>(fun sm -> 
                ignore priority
                let mutable awaiter = task.GetAwaiter()

                let cont =
                    (TaskStateMachineResumption<'T>(fun sm -> 
                        sm.Result <- awaiter.GetResult()
                        true))

                // shortcut to continue immediately
                if task.IsCompleted then
                    cont.Invoke(&sm)
                else
                    // If the task definition has not been converted to a state machine then a continuation function is used
                    sm.Awaiter <- awaiter
                    sm.ResumptionFunc <- cont
                    false)

        static member inline CanReturnFrom (priority: IPriority1, computation: Async<'T>)  : TaskCode<'T, 'T> =
            TaskWitnesses.CanReturnFrom (priority, Async.StartAsTask computation)

    [<AutoOpen>]
    module TaskHelpers = 

        type TaskBuilder with

            member inline _.Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                               when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> 
                        (task: ^TaskLike, continuation: ^TResult1 -> TaskCode<'TOverall, 'TResult2>)  : TaskCode<'TOverall, 'TResult2> =

                RequireCanBind< TaskWitnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<TaskWitnesses> task continuation

            member inline _.ReturnFrom< ^TaskLike, 'T  when (TaskWitnesses or ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskCode<'T, 'T>) > 
                        (task: ^TaskLike)  : TaskCode<'T, 'T> =

                RequireCanReturnFrom< TaskWitnesses, ^TaskLike, 'T> Unchecked.defaultof<TaskWitnesses> task

module Tests =
    open System.Collections
    open System.Collections.Generic
    open System.Threading
    open System.Diagnostics
    type ITaskThing =
        abstract member Taskify : 'a option -> 'a Task

    type SmokeTestsForCompilation() =

        member __.tinyTask() =
            taskUsingRawResumableCode {
                return 1
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 1 then failwith "failed"

        member __.tbind() =
            taskUsingRawResumableCode {
                let! x = Task.FromResult(1)
                return 1 + x
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 2 then failwith "failed"

        member __.tnested() =
            taskUsingRawResumableCode {
                let! x = taskUsingRawResumableCode { return 1 }
                return x
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 1 then failwith "failed"

        member __.tcatch0() =
            taskUsingRawResumableCode {
                try 
                   return 1
                with e -> 
                   return 2
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 1 then failwith "failed"

        member __.tcatch1() =
            taskUsingRawResumableCode {
                try 
                   let! x = Task.FromResult 1
                   return x
                with e -> 
                   return 2
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 1 then failwith "failed"


        member __.t3() =
            let t2() =
                taskUsingRawResumableCode {
                    System.Console.WriteLine("hello")
                    return 1
                }
            taskUsingRawResumableCode {
                System.Console.WriteLine("hello")
                let! x = t2()
                System.Console.WriteLine("world")
                return 1 + x
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 2 then failwith "failed"

        member __.t3b() =
            taskUsingRawResumableCode {
                System.Console.WriteLine("hello")
                let! x = Task.FromResult(1)
                System.Console.WriteLine("world")
                return 1 + x
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 2 then failwith "failed"

        member __.t3c() =
            taskUsingRawResumableCode {
                System.Console.WriteLine("hello")
                do! Task.Delay(100)
                System.Console.WriteLine("world")
                return 1 
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 1 then failwith "failed"

        // This tests an exception match
        member __.t67() =
            taskUsingRawResumableCode {
                try
                    do! Task.Delay(0)
                with
                | :? ArgumentException -> 
                    ()
                | _ -> 
                    ()
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> () then failwith "failed"

        // This tests compiling an incomplete exception match
        member __.t68() =
            taskUsingRawResumableCode {
                try
                    do! Task.Delay(0)
                with
                | :? ArgumentException -> 
                    ()
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> () then failwith "failed"

        member __.testCompileAsyncWhileLoop() =
            taskUsingRawResumableCode {
                let mutable i = 0
                while i < 5 do
                    i <- i + 1
                    do! Task.Yield()
                return i
            }
            |> fun t -> 
                t.Wait()
                if t.Result <> 5 then failwith "failed"


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
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    do! Task.Yield()
                    Thread.Sleep(100)
                }
            sw.Stop()
            printfn "sw.ElapsedMilliseconds = %A" sw.ElapsedMilliseconds
            require (sw.ElapsedMilliseconds < 80L) "sleep blocked caller"
            sw.Start()
            t.Wait()
            printfn "sw.ElapsedMilliseconds = %A" sw.ElapsedMilliseconds
            require (sw.ElapsedMilliseconds > 80L) "wait didn't block caller"


        member _.testCatching1() =
            printfn "Running testCatching1..."
            let mutable x = 0
            let mutable y = 0
            let t =
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    try
                        printfn "yielding"
                        do! Task.Yield() // can't skip through this
                        printfn "raising TestException"
                        failtest "hello"
                        x <- 1
                        do! Task.Delay(100)
                    with
                    | TestException msg ->
                        printfn "caught TestException"
                        require (msg = "hello") "message tampered"
                    | _ ->
                        require false "other exn type"
                    y <- 1
                }
            t.Wait()
            require (x = 0) "ran past failure"
            require (y = 1) "bailed after exn"


        member _.testNestedCatching() =
            printfn "Running testNestedCatching..."
            let mutable counter = 1
            let mutable caughtInner = 0
            let mutable caughtOuter = 0
            let t1() =
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    use! d =
                        taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    try
                        use! d =
                            taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                    taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    return "hello"
                }
            let t2 =
                taskUsingRawResumableCode {
                    let! s = t1
                    return s.Length
                }
            t2.Wait()


        member _.testNoStackOverflowWithImmediateResult() =
            printfn "running testNoStackOverflowWithImmediateResult"
            let longLoop =
                taskUsingRawResumableCode {
                    let mutable n = 0
                    while n < BIG do
                        n <- n + 1
                        return! Task.FromResult(())
                }
            longLoop.Wait()
    

        member _.testNoStackOverflowWithYieldResult() =
            printfn "running testNoStackOverflowWithYieldResult"
            let longLoop =
                taskUsingRawResumableCode {
                    let mutable n = 0
                    while n < BIG do
                        let! _ =
                            taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    if n < 100 then
                        do! Task.Yield()
                        let! _ = Task.FromResult(0)
                        return! loop (n + 1)
                    else
                        return ()
                }
            let shortLoop =
                taskUsingRawResumableCode {
                    return! loop 0
                }
            shortLoop.Wait()
    

        member _.testTryOverReturnFrom() =
            printfn "running testTryOverReturnFrom"
            let inner() =
                taskUsingRawResumableCode {
                    printfn "about to yield"
                    do! Task.Yield()
                    printfn "about to raise"
                    failtest "inner"
                    printfn "after raise"
                    return 1
                }
            let t =
                taskUsingRawResumableCode {
                    try
                        //do! Task.Yield()
                        return! inner()
                    with
                    | TestException "inner" -> 
                       printfn "in catch"
                       return 2
                }
            require (t.Result = 2) "didn't catch"


        member _.testTryFinallyOverReturnFromWithException() =
            printfn "running testTryFinallyOverReturnFromWithException"
            let inner() =
                taskUsingRawResumableCode {
                    do! Task.Yield()
                    failtest "inner"
                    return 1
                }
            let mutable m = 0
            let t =
                taskUsingRawResumableCode {
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
                taskUsingRawResumableCode {
                    do! Task.Yield()
                    return 1
                }
            let mutable m = 0
            let t =
                taskUsingRawResumableCode {
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
            taskUsingRawResumableCode {
                do! Task.Yield()
                return x
            }

        // no need to call this, we just want to check that it compiles w/o warnings
        member _.testTrivialTransformedReturnCompiles (x : 'a) (f : 'a -> 'b) : 'b Task =
            taskUsingRawResumableCode {
                do! Task.Yield()
                return f x
            }


        member _.testAsyncsMixedWithTasks() =
            let t =
                taskUsingRawResumableCode {
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
            let t = taskUsingRawResumableCode { return Some "x" }
            taskUsingRawResumableCode {
                let! r = t
                if r = None then
                    return! failwithf "Could not find x" 
                else
                    return r
            }
            |> ignore


        // no need to call this, just check that it compiles
        member _.testCompilerInfersArgumentOfReturnFrom() =
            taskUsingRawResumableCode {
                if true then return 1
                else return! failwith ""
            }
            |> ignore


    module M = 
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
