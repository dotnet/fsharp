// Task builder for F# that compiles to allocation-free paths for synchronous code.
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
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

/// The extra data stored in ResumableStateMachine for tasks
[<Struct; NoComparison; NoEquality>]
type TaskStateMachineData<'TOverall> =

    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    [<DefaultValue(false)>]
    val mutable Awaiter: ICriticalNotifyCompletion

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'TOverall>

and TaskStateMachine<'TOverall> = ResumableStateMachine<TaskStateMachineData<'TOverall>>
and TaskResumptionFunc<'TOverall> = ResumptionFunc<TaskStateMachineData<'TOverall>>
and TaskResumptionFuncExecutor<'TOverall> = ResumptionFuncExecutor<TaskStateMachineData<'TOverall>>
and TaskCode<'TOverall, 'T> = ResumableCode<TaskStateMachineData<'TOverall>, 'T>

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

    let inline SetResumptionFunc (sm: byref<ResumableStateMachine<'Data>>) f =
        let (_, e, ssm) = sm.ResumptionFuncData
        sm.ResumptionFuncData <- (f, e, ssm)

    let inline GetResumptionFunc (sm: byref<ResumableStateMachine<'Data>>) =
        let (f, _, _) = sm.ResumptionFuncData
        f

type TaskBuilder() =

    member inline _.Delay(f : unit -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<'TOverall, 'T>(fun sm -> (f()).Invoke(&sm))

    // This is the dynamic implementation - this is not used
    // for statically compiled tasks.  An executor (resumptionFuncExecutor) is 
    // registered with the state machine, plus the initial resumption.
    // The executor stays constant throughout the execution, it wraps each step
    // of the execution in a try/with.  The resumption is changed at each step
    // to represent the continuation of the computation.
    static member RunDynamic(code: TaskCode<'T, 'T>) : Task<'T> = 
        let mutable sm = TaskStateMachine<'T>()
        let initialResumptionFunc = TaskResumptionFunc<'T>(fun sm -> code.Invoke(&sm))
        let resumptionFuncExecutor = 
            TaskResumptionFuncExecutor<'T>(fun sm f -> 
                //let addr =  sm.Data.Id
                //let addr = NativeInterop.NativePtr.toNativeInt &&sm.ResumptionPoint
                //printfn "[%d][%d] in executor, salt = %d" System.Threading.Thread.CurrentThread.ManagedThreadId addr sm.Data.Salt
                // The alternative if resumable code could not be used
                try
                    sm.Data.Awaiter <- null
                    //printfn "[%d][%d] in executor: Invoke" System.Threading.Thread.CurrentThread.ManagedThreadId addr
                    let step = f.Invoke(&sm) 
                    if step then 
                        //printfn "[%d][%d] in executor: SetResult %A" System.Threading.Thread.CurrentThread.ManagedThreadId addr sm.Data.Result
                        sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                    else
                        //printfn "[%d][%d] in executor: Await" System.Threading.Thread.CurrentThread.ManagedThreadId addr
                        // In the dynamic implementation the AwaitUnsafeOnCompleted must be called after the
                        // return to the trampoline. This is because the ResumbleCode.*Dynamic adjust
                        // the continuation by mutation as we come back down the stack.  The Awaiter
                        // is always set before each return of 'false'.
                        assert not (isNull sm.Data.Awaiter)
                        sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&sm.Data.Awaiter, &sm)

                with exn ->
                    //printfn "[%d][%d] in executor: SetException" System.Threading.Thread.CurrentThread.ManagedThreadId  addr
                    sm.Data.MethodBuilder.SetException exn)
        let setStateMachine =
            SetStateMachineMethodImpl<TaskStateMachine<'T>>(fun sm state -> 
                //let addr =  NativeInterop.NativePtr.toNativeInt &&sm.ResumptionPoint
                //let addr =  sm.Data.Id
                //printfn "[%d][%d] SetStateMachine" System.Threading.Thread.CurrentThread.ManagedThreadId  addr
                sm.Data.MethodBuilder.SetStateMachine(state))
        sm.ResumptionFuncData <- (initialResumptionFunc, resumptionFuncExecutor, setStateMachine)
        sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
        sm.Data.MethodBuilder.Start(&sm)
        sm.Data.MethodBuilder.Task

    member inline _.Run(code : TaskCode<'T, 'T>) : Task<'T> = 
         if __useResumableCode then 

            // This is the static implementation.  A new struct type is created.
            __structStateMachine<TaskStateMachine<'T>, Task<'T>>
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
                        failwith "unreachable"))

                // IAsyncStateMachine.SetStateMachine
                (SetStateMachineMethodImpl<_>(fun sm state -> 
                    sm.Data.MethodBuilder.SetStateMachine(state)))

                // Other interfaces (IResumableStateMachine)
                [| 
                   (typeof<IResumableStateMachine<TaskStateMachineData<'T>>>, "get_ResumptionPoint", GetResumptionPointMethodImpl<TaskStateMachine<'T>>(fun sm -> 
                        sm.ResumptionPoint) :> _);
                   (typeof<IResumableStateMachine<TaskStateMachineData<'T>>>, "get_Data", GetResumableStateMachineDataMethodImpl<TaskStateMachine<'T>, TaskStateMachineData<'T>>(fun sm -> 
                        sm.Data) :> _);
                   (typeof<IResumableStateMachine<TaskStateMachineData<'T>>>, "set_Data", SetResumableStateMachineDataMethodImpl<TaskStateMachine<'T>, TaskStateMachineData<'T>>(fun sm data -> 
                        sm.Data <- data) :> _);
                 |]

                // Start
                (AfterCode<TaskStateMachine<'T>,_>(fun sm -> 
                    sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
                    sm.Data.MethodBuilder.Start(&sm)
                    sm.Data.MethodBuilder.Task))
         else
            TaskBuilder.RunDynamic(code)

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : TaskCode<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return (value: 'T) : TaskCode<'T, 'T> = 
        TaskCode<'T, _>(fun sm -> 
            sm.Data.Result <- value
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine(task1: TaskCode<'TOverall, unit>, task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        ResumableCode.Combine(task1, task2)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While ([<InlineIfLambda>] condition : unit -> bool, body : TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        ResumableCode.While(condition, body)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith (body: TaskCode<'TOverall, 'T>, catch: exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        ResumableCode.TryWith(body, catch)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally (body: TaskCode<'TOverall, 'T>, [<InlineIfLambda>] compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        ResumableCode.TryFinally(body, NonResumableCode<_,_>(fun _ -> compensation()))

    member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (resource : 'Resource, body : 'Resource -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> = 
        ResumableCode.Using(resource, body)

    member inline _.For (sequence : seq<'T>, body : 'T -> TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        ResumableCode.For(sequence, body)

    static member ReturnFromDynamic (sm: byref<TaskStateMachine<'T>>, task: Task<'T>) : bool = 
        let mutable awaiter = task.GetAwaiter()

        let cont =
            TaskResumptionFunc<'T>(fun sm -> 
                sm.Data.Result <- awaiter.GetResult()
                true)

        // shortcut to continue immediately
        if task.IsCompleted then
            cont.Invoke(&sm)
        else
            // If the task definition has not been converted to a state machine then a continuation function is used
            SetResumptionFunc &sm cont
            sm.Data.Awaiter <- awaiter
            false

    member inline _.ReturnFrom (task: Task<'T>) : TaskCode<'T, 'T> = 
        TaskCode<'T, _>(fun sm -> 
            if __useResumableCode then 
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
            else
                TaskBuilder.ReturnFromDynamic(&sm, task)
                //-- RESUMABLE CODE END
            )

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
                  (sm: byref<_>, priority: IPriority2, task: ^TaskLike, continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : bool =

                ignore priority
                let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

                let cont = 
                    (TaskResumptionFunc<'TOverall>( fun sm -> 
                        let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                        (continuation result).Invoke(&sm)))

                // shortcut to continue immediately
                if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                    cont.Invoke(&sm)
                else
                    sm.Data.Awaiter <- awaiter
                    SetResumptionFunc &sm cont
                    false

        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
                  (priority: IPriority2, task: ^TaskLike, continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

            TaskCode<'TOverall, _>(fun sm -> 
                if __useResumableCode then 
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
                else
                    TaskWitnesses.CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall>(&sm, priority, task, continuation)
                //-- RESUMABLE CODE END
            )

        static member inline CanBindDynamic (sm: byref<_>, priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : bool =
            ignore priority
            let mutable awaiter = task.GetAwaiter()

            let cont = 
                (TaskResumptionFunc<'TOverall>(fun sm -> 
                    //Console.WriteLine("[{0}] resumed CanBind(Task)", sm.MethodBuilder.Task.Id)
                    let result = awaiter.GetResult()
                    (continuation result).Invoke(&sm)))

            // shortcut to continue immediately
            if awaiter.IsCompleted then 
                cont.Invoke(&sm)
            else
                sm.Data.Awaiter <- awaiter
                SetResumptionFunc &sm cont
                false

        static member inline CanBind (priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

            TaskCode<'TOverall, _>(fun sm -> 
                if __useResumableCode then 
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
                else
                    TaskWitnesses.CanBindDynamic(&sm, priority, task, continuation)
                //-- RESUMABLE CODE END
            )

        static member inline CanBind (priority: IPriority1, computation: Async<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
            TaskWitnesses.CanBind (priority, Async.StartAsTask computation, continuation)

        static member inline CanReturnFromDynamic< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)>
              (sm: byref<TaskStateMachine< ^T >>, priority: IPriority2, task: ^TaskLike) : bool =

            ignore priority
            let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

            let cont =
                (TaskResumptionFunc< ^T >(fun sm -> 
                    sm.Data.Result <- (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                    true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                cont.Invoke(&sm)
            else
                sm.Data.Awaiter <- awaiter
                SetResumptionFunc &sm cont
                false

        static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                           when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                           and ^Awaiter :> ICriticalNotifyCompletion
                                           and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                           and ^Awaiter: (member GetResult: unit ->  ^T)>
              (priority: IPriority2, task: ^TaskLike) : TaskCode< ^T,  ^T> =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
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
                else
                    TaskWitnesses.CanReturnFromDynamic(&sm, priority, task)
                //-- RESUMABLE CODE END
            )

        static member CanReturnFromDynamic (sm: byref<_>, task: Task<'T>) : bool =

            let mutable awaiter = task.GetAwaiter()

            let cont =
                (TaskResumptionFunc<'T>(fun sm -> 
                    sm.Data.Result <- awaiter.GetResult()
                    true))

            // shortcut to continue immediately
            if task.IsCompleted then
                cont.Invoke(&sm)
            else
                // If the task definition has not been converted to a state machine then a continuation function is used
                sm.Data.Awaiter <- awaiter
                SetResumptionFunc &sm cont
                false

        static member inline CanReturnFrom (priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

            TaskCode<_, _>(fun sm -> 
                if __useResumableCode then 
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
                else
                    TaskWitnesses.CanReturnFromDynamic(&sm, task)
                //-- RESUMABLE CODE END
            )

        static member inline CanReturnFrom (priority: IPriority1, computation: Async<'T>)  : TaskCode<'T, 'T> =
            TaskWitnesses.CanReturnFrom (priority, Async.StartAsTask computation)

    [<AutoOpen>]
    module TaskHelpers = 

        type TaskBuilder with

            member inline _.Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                               when (TaskWitnesses or  ^TaskLike): (static member CanBind: TaskWitnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> 
                        (task: ^TaskLike, continuation: ^TResult1 -> TaskCode<'TOverall, 'TResult2>)  : TaskCode<'TOverall, 'TResult2> =

                RequireCanBind< TaskWitnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<TaskWitnesses> task continuation

            member inline _.ReturnFrom< ^TaskLike, 'T when (TaskWitnesses or ^TaskLike): (static member CanReturnFrom: TaskWitnesses * ^TaskLike -> TaskCode<'T, 'T>) > 
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
