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

namespace Microsoft.FSharp.Control

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

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

    let task = TaskBuilder()

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

(*

module ContextInsensitiveTasks =

    let task = TaskBuilder()

    [<Sealed; NoComparison; NoEquality>]
    type TaskWitnesses() = 
        interface IPriority1
        interface IPriority2
        interface IPriority3

        
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> 
                     (priority: IPriority3, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            // get an awaiter from the task
            ignore priority
            let mutable awaiter = (^TaskLike : (member GetAwaiter : unit -> ^Awaiter)(task))
            match __resumableEntry() with 
            | Some contID -> 
                // shortcut to continue immediately
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                    __resumeAt contID
                else
                    sm.Await (&awaiter, contID)
                    TaskStep<'TResult2>(false)
            | None ->
                continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))

        
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> 
                     (priority: IPriority2, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            ignore priority
            let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable))
            let CONT = __resumableEntry (fun () -> continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))) 
            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        
        static member inline CanBind (priority :IPriority1, sm: TaskStateMachine<'TOverall>, task: Task<'TResult1>, continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            ignore priority
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __resumableEntry (fun () -> continuation (awaiter.GetResult()))
            if awaiter.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        
        static member inline CanBind (priority: IPriority1, sm: TaskStateMachine<'TOverall>, computation : Async<'TResult1>, continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            TaskWitnesses.CanBind (priority, sm, Async.StartAsTask computation, continuation)

        
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) >
               (priority: IPriority3, sm: TaskStateMachine< ^T >, task: ^Awaitable) =

            // get an awaiter from the task
            ignore priority
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(task))
            let CONT = __resumableEntry (fun () -> sm.SetResult (^Awaiter : (member GetResult : unit -> ^T)(awaiter)); TaskStep<'T>(true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep< ^T >(false)
        
        
        static member inline CanReturnFrom< ^TaskLike, ^Awaitable, ^Awaiter, ^T
                                                        when ^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)
                                                        and ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                                        and ^Awaiter :> ICriticalNotifyCompletion 
                                                        and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                                        and ^Awaiter : (member GetResult : unit -> ^T ) > (_: IPriority2, sm: TaskStateMachine< ^T >, task: ^TaskLike) =

            let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable))
            let CONT = __resumableEntry (fun () -> sm.SetResult (^Awaiter : (member GetResult : unit -> ^T)(awaiter)); TaskStep<'T>(true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep< ^T >(false)

        
        static member inline CanReturnFrom (priority: IPriority1, sm: TaskStateMachine<'T>, task: Task<'T>) : TaskStep<'T> =
            ignore priority
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __resumableEntry (fun () -> sm.SetResult (awaiter.GetResult()); TaskStep<'T>(true))
            if task.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'T>(false)

        
        static member inline CanReturnFrom (priority: IPriority1, sm: TaskStateMachine<'T>, computation: Async<'T>) =
            TaskWitnesses.CanReturnFrom (priority, sm, Async.StartAsTask computation)

    type TaskStateMachine<'TOverall> with
        
        member inline _.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * TaskStateMachine<'TOverall> * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
            RequireCanBind< TaskWitnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<TaskWitnesses> sm task continuation

        
        member inline _.ReturnFrom< ^TaskLike, 'T  when (TaskWitnesses or ^TaskLike): (static member CanReturnFrom: TaskWitnesses * TaskStateMachine<'T> * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< TaskWitnesses, ^TaskLike, 'T> Unchecked.defaultof<TaskWitnesses> sm task
*)

#endif
