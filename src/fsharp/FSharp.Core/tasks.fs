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

#if FSHARP_CORE

#nowarn "9"
#nowarn "51"
namespace Microsoft.FSharp.Core.CompilerServices

    open System.Runtime.CompilerServices
    open Microsoft.FSharp.Core

    /// A marker interface to give priority to different available overloads
    type IPriority3 = interface end

    /// A marker interface to give priority to different available overloads
    type IPriority2 = interface inherit IPriority3 end

    /// A marker interface to give priority to different available overloads
    type IPriority1 = interface inherit IPriority2 end

    type MoveNextMethod<'Template> = delegate of byref<'Template> -> unit

    type SetMachineStateMethod<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

    type AfterMethod<'Template, 'Result> = delegate of byref<'Template> -> 'Result

    module StateMachineHelpers = 

        /// Statically determines whether resumable code is being used
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __useResumableCode<'T> : bool = false
        
        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumableEntry () : int option = 
            failwith "__resumableEntry should always be guarded by __useResumableCode and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumeAt<'T> (_pc: int) : 'T = 
            failwith "__resumeAt should always be guarded by __useResumableCode and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumableObject<'T> (_x: 'T) : 'T =
            failwith "__resumableObject should always be guarded by __useResumableCode and only used in valid state machine implementations"

        [<MethodImpl(MethodImplOptions.NoInlining)>]
        let __resumableStruct<'Template, 'Result> (_moveNext: MoveNextMethod<'Template>) (_setMachineState: SetMachineStateMethod<'Template>) (_after: AfterMethod<'Template, 'Result>): 'Result =
            failwith "__resumableStruct should always be guarded by __useResumableCode and only used in valid state machine implementations"
        
#endif

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
namespace Microsoft.FSharp.Control

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

[<AutoOpen>]
module Utils2 = 

    let inline hashq x = Microsoft.FSharp.Core.LanguagePrimitives.PhysicalHash x

//[<NoComparison; NoEquality>]
//type TaskStateMachine<'T> =
//    [<DefaultValue(false)>]
//    val mutable State : TaskStateMachine<'T>

[<Struct; NoComparison; NoEquality>]
/// Acts as a template for struct state machines introduced by __resumableStruct, and also as a reflective implementation
type TaskStateMachine<'TOverall> =

    /// Holds the final result of the state machine (between the 'return' and the execution of the finally clauses, after which we we eventually call SetResult)
    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint : int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc : TaskMachineFunc<'TOverall>

    /// When interpreted, holds the awaiter 
    [<DefaultValue(false)>]
    val mutable Awaiter : ICriticalNotifyCompletion

    [<DefaultValue(false)>]
    val mutable MethodBuilder : AsyncTaskMethodBuilder<'TOverall>

    member sm.Address = 0n
        //let addr = &&sm.ResumptionPoint
        //Microsoft.FSharp.NativeInterop.NativePtr.toNativeInt addr

    interface IAsyncStateMachine with 
        
        // Used when interpreted.  For "__resumableStruct" it is replaced.
        member sm.MoveNext() = 
            try
                //Console.WriteLine("[{0}] resuming by invoking {1}....", sm.MethodBuilder.Task.Id, hashq sm.ResumptionFunc )
                let step = sm.ResumptionFunc.Invoke(&sm) 
                if step then 
                    //Console.WriteLine("[{0}] SetResult {1}", sm.MethodBuilder.Task.Id, sm.Result)
                    sm.MethodBuilder.SetResult(sm.Result)
                else
                    sm.MethodBuilder.AwaitUnsafeOnCompleted(&sm.Awaiter, &sm)
            with exn ->
                //Console.WriteLine("[{0}] SetException {1}", sm.MethodBuilder.Task.Id, exn)
                sm.MethodBuilder.SetException exn

        // Used when interpreted.  For "__resumableStruct" it is replaced.
        member sm.SetStateMachine(state) = 
            sm.MethodBuilder.SetStateMachine(state)

and TaskMachineFunc<'TOverall> = delegate of byref<TaskStateMachine<'TOverall>> -> bool

/// Represents a code fragment of a task.  When statically compiled, TaskCode is always removed and the body of the code inlined
/// into an invocation.
type TaskCode<'TOverall, 'T> = delegate of byref<TaskStateMachine<'TOverall>> -> bool

[<AutoOpen>]
module TaskMethodRequire = 

    [<NoDynamicInvocation>]
    let inline RequireCanBind< ^Priority, ^TaskLike, ^TResult1, 'TResult2, 'TOverall 
                                when (^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) > (priority: ^Priority) (task: ^TaskLike) __expand_continuation = 
        ((^Priority or ^TaskLike): (static member CanBind : ^Priority * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>) (priority, task, __expand_continuation))

    [<NoDynamicInvocation>]
    let inline RequireCanReturnFrom< ^Priority, ^TaskLike, 'T when (^Priority or ^TaskLike): (static member CanReturnFrom: ^Priority * ^TaskLike -> TaskCode<'T, 'T>)> (priority: ^Priority) (task: ^TaskLike) = 
        ((^Priority or ^TaskLike): (static member CanReturnFrom : ^Priority * ^TaskLike -> TaskCode<'T, 'T>) (priority, task))

// New style task builder.
type TaskBuilder() =

    member inline __.Delay(__expand_f : unit -> TaskCode<'TOverall, 'T>) =
        TaskCode<'TOverall, 'T>(fun sm -> (__expand_f()).Invoke(&sm))

    member inline __.Run(__expand_code : TaskCode<'TOverall, 'TOverall>) : Task<'TOverall> = 
        if __useResumableCode then 
            __resumableStruct<TaskStateMachine<'TOverall>, Task<'TOverall>>
                // MoveNext
                (MoveNextMethod<_>(fun sm -> 
                    __resumeAt sm.ResumptionPoint 
                    try
                        //Console.WriteLine("[{0}] step from {1}", sm.MethodBuilder.Task.Id, resumptionPoint)
                        let ``__machine_step$cont`` = __expand_code.Invoke(&sm)
                        if ``__machine_step$cont`` then 
                            //Console.WriteLine("[{0}] SetResult {1}", sm.MethodBuilder.Task.Id, res)
                            sm.MethodBuilder.SetResult(sm.Result)
                    with exn ->
                        //Console.WriteLine("[{0}] exception {1}", sm.MethodBuilder.Task.Id, exn)
                        sm.MethodBuilder.SetException exn))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    sm.MethodBuilder.SetStateMachine(state)))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    //Console.WriteLine("[{0}] start", sm.MethodBuilder.Task.Id)
                    sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
                    sm.MethodBuilder.Start(&sm)
                    //Console.WriteLine("[{0}] unwrap", sm.MethodBuilder.Task.Id)
                    sm.MethodBuilder.Task))
        else
            TaskBuilder.RunDynamic(__expand_code)

    static member RunDynamic(task : TaskCode<'TOverall, 'TOverall>) : Task<'TOverall> = 
        let mutable sm = TaskStateMachine<'TOverall>()
        sm.ResumptionFunc <- TaskMachineFunc<_>(fun sm -> task.Invoke(&sm)) 
        sm.MethodBuilder <- AsyncTaskMethodBuilder<'TOverall>.Create()
        //Console.WriteLine("[{0}] start", sm.MethodBuilder.Task.Id)
        sm.MethodBuilder.Start(&sm)
        //Console.WriteLine("[{0}] unwrap", sm.MethodBuilder.Task.Id)
        sm.MethodBuilder.Task        

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    //[<DefaultValue>]
    member inline __.Zero() : TaskCode<'TOverall, unit> =
        TaskCode<_, unit>(fun sm ->
            true)

    member inline __.Return (x: 'TOverall) : TaskCode<'TOverall, 'TOverall> = 
        TaskCode<_, _>(fun sm -> 
            sm.Result <- x
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline __.Combine(__expand_task1: TaskCode<'TOverall, unit>, __expand_task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then
                // NOTE: The code for __expand_task1 may contain await points! Resuming may branch directly
                // into this code!
                let ``__machine_step$cont`` = __expand_task1.Invoke(&sm)
                if ``__machine_step$cont`` then 
                    __expand_task2.Invoke(&sm)
                else
                    false
            else
                TaskBuilder.CombineDynamic(__expand_task1, __expand_task2).Invoke(&sm))

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    static member CombineDynamic(task1: TaskCode<'TOverall, unit>, task2: TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if task1.Invoke(&sm) then 
                task2.Invoke(&sm)
            else
                let rec resume (mf: TaskMachineFunc<_>) =
                    TaskMachineFunc<_>(fun sm -> 
                        //Console.WriteLine("[{0}] resuming Combine", sm.MethodBuilder.Task.Id)
                        if mf.Invoke(&sm) then 
                            task2.Invoke(&sm)
                        else
                            //Console.WriteLine("[{0}] rebinding ResumptionFunc for Combine (2)", sm.MethodBuilder.Task.Id)
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false)

                //Console.WriteLine("[{0}] rebinding ResumptionFunc for Combine (1)", sm.MethodBuilder.Task.Id)
                sm.ResumptionFunc <- resume sm.ResumptionFunc
                false)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline __.While (condition : unit -> bool, __expand_body : TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                let mutable __stack_completed = true 
                while __stack_completed && condition() do
                    __stack_completed <- false 
                    // NOTE: The body of the state machine code for 'while' may contain await points, so resuming
                    // the code will branch directly into the expanded '__expand_body', branching directly into the while loop
                    let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step 
                    __stack_completed <- ``__machine_step$cont``
                __stack_completed
            else
                TaskBuilder.WhileDynamic(condition, __expand_body).Invoke(&sm))

    static member WhileDynamic (condition: unit -> bool, body: TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        TaskCode<_, _>(fun sm ->
            let rec repeat() = 
                TaskMachineFunc<_>(fun sm -> 
                    //Console.WriteLine("[{0}] repeat WhileLoop", sm.MethodBuilder.Task.Id)
                    if condition() then 
                        if body.Invoke (&sm) then
                            repeat().Invoke(&sm)
                        else
                            //Console.WriteLine("[{0}] rebinding ResumptionFunc for While", sm.MethodBuilder.Task.Id)
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    else
                        true)
            and resume (mf: TaskMachineFunc<_>) =
                TaskMachineFunc<_>(fun sm -> 
                    //Console.WriteLine("[{0}] resume WhileLoop body", sm.MethodBuilder.Task.Id)
                    let step = mf.Invoke(&sm)
                    if step then 
                        repeat().Invoke(&sm)
                    else
                        //Console.WriteLine("[{0}] rebinding ResumptionFunc for While", sm.MethodBuilder.Task.Id)
                        sm.ResumptionFunc <- resume sm.ResumptionFunc
                        false)

            repeat().Invoke(&sm))

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryWith (__expand_body : TaskCode<'TOverall, 'T>, __expand_catch : exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                let mutable __stack_completed = false
                let mutable __stack_caught = false
                let mutable __stack_savedExn = Unchecked.defaultof<_>
                try
                    // The try block may contain await points.
                    let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step
                    __stack_completed <- ``__machine_step$cont``
                with exn -> 
                    // The catch block may not contain await points.
                    __stack_caught <- true
                    __stack_savedExn <- exn

                if __stack_caught then 
                    // Place the catch code outside the catch block 
                    (__expand_catch __stack_savedExn).Invoke(&sm)
                else
                    __stack_completed
            else
                TaskBuilder.TryWithDynamic(__expand_body, __expand_catch).Invoke(&sm))

    static member TryWithDynamic (body: TaskCode<'TOverall, 'T>, handler: exn -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            let rec resume (mf: TaskMachineFunc<_>) =
                TaskMachineFunc<_>(fun sm -> 
                    try
                        //Console.WriteLine("[{0}] resuming TryWith", sm.MethodBuilder.Task.Id)
                        if mf.Invoke (&sm) then 
                            //Console.WriteLine("[{0}] resumed TryWith completed", sm.MethodBuilder.Task.Id)
                            true
                        else
                            //Console.WriteLine("[{0}] rebinding ResumptionFunc for TryWith (2)", sm.MethodBuilder.Task.Id)
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    with exn -> 
                        //Console.WriteLine("[{0}] catch block of resumed TryWith", sm.MethodBuilder.Task.Id)
                        (handler exn).Invoke(&sm))
            try
                let step = body.Invoke (&sm)
                if not step then 
                    //Console.WriteLine("[{0}] rebinding ResumptionFunc {1} to {2} for TryWith, sm.addr = {3}", sm.MethodBuilder.Task.Id, hashq sm.ResumptionFunc, hashq rf2, sm.Address)
                    sm.ResumptionFunc <- sm.ResumptionFunc
                step
                        
            with exn -> 
                //Console.WriteLine("[{0}] catch block of TryWith", sm.MethodBuilder.Task.Id)
                (handler exn).Invoke(&sm))

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline __.TryFinally (__expand_body: TaskCode<'TOverall, 'T>, compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            if __useResumableCode then 
                let mutable __stack_completed = false
                try
                    let ``__machine_step$cont`` = __expand_body.Invoke (&sm)
                    // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                    // may skip this step.
                    __stack_completed <- ``__machine_step$cont``
                with _ ->
                    compensation()
                    reraise()

                if __stack_completed then 
                    compensation()
                __stack_completed
            else
                TaskBuilder.TryFinallyDynamic(__expand_body, compensation).Invoke(&sm))

    static member TryFinallyDynamic (body: TaskCode<'TOverall, 'T>, compensation : unit -> unit) : TaskCode<'TOverall, 'T> =
        TaskCode<_, _>(fun sm ->
            let rec resume (mf: TaskMachineFunc<_>) =
                TaskMachineFunc<_>(fun sm -> 
                    let mutable completed = false
                    try
                        //Console.WriteLine("[{0}] resumed TryFinally", sm.MethodBuilder.Task.Id)
                        completed <- mf.Invoke (&sm)
                        if not completed then 
                            //Console.WriteLine("[{0}] rebinding ResumptionFunc for TryFinally (2)", sm.MethodBuilder.Task.Id)
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
                    //Console.WriteLine("[{0}] rebinding ResumptionFunc for TryFinally (1)", sm.MethodBuilder.Task.Id)
                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                       
            with _ ->
                compensation()
                reraise()

            if completed then 
                compensation()

            completed)

    member inline builder.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable> (disp : 'Resource, __expand_body : 'Resource -> TaskCode<'TOverall, 'T>) : TaskCode<'TOverall, 'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        builder.TryFinally(
            TaskCode<_, _>(fun sm -> (__expand_body disp).Invoke(&sm)),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline builder.For (sequence : seq<'T>, __expand_body : 'T -> TaskCode<'TOverall, unit>) : TaskCode<'TOverall, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        builder.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> builder.While((fun () -> e.MoveNext()), TaskCode<_, _>(fun sm -> (__expand_body e.Current).Invoke(&sm)))))

    member inline __.ReturnFrom (task: Task<'T>) : TaskCode<'T, 'T> = 
        TaskCode<_, _>(fun sm -> 
            if __useResumableCode then 
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
                    //Console.WriteLine("[{0}] resumed ReturnFrom(Task<_>)", sm.MethodBuilder.Task.Id)
                    sm.Result <- awaiter.GetResult()
                    true
            else
                TaskBuilder.ReturnFromDynamic(task).Invoke(&sm))

    static member ReturnFromDynamic (task: Task<'T>) : TaskCode<'T, 'T> = 
        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let cont =
                TaskMachineFunc<'T>(fun sm -> 
                    //Console.WriteLine("[{0}] resumed ReturnFrom(Task<_>)", sm.MethodBuilder.Task.Id)
                    sm.Result <- awaiter.GetResult()
                    true)

            // shortcut to continue immediately
            if task.IsCompleted then
                cont.Invoke(&sm)
            else
                //Console.WriteLine("[{0}] setting ResumptionFunc for ReturnFrom", sm.MethodBuilder.Task.Id)
                // If the task definition has not been converted to a state machine then a continuation function is used
                sm.ResumptionFunc <- cont
                sm.Awaiter <- awaiter
                false)

and [<Sealed>] Witnesses() =

    interface IPriority1
    interface IPriority2
    interface IPriority3

    static member inline CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                        when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                        and ^Awaiter :> ICriticalNotifyCompletion
                                        and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                        and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
              (_priority: IPriority2, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

            let cont = 
                (TaskMachineFunc<'TOverall>( fun sm -> 
                    //Console.WriteLine("[{0}] resumed CanBind(TaskLike)", sm.MethodBuilder.Task.Id)
                    let result = (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))
                    (__expand_continuation result).Invoke(&sm)))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                cont.Invoke(&sm)
            else
                //Console.WriteLine("[{0}] setting ResumptionFunc for CanBind TaskLike to {1}, sm.addr = {2}", sm.MethodBuilder.Task.Id, hashq CONT, sm.Address)
                sm.Awaiter <- awaiter
                sm.ResumptionFunc <- cont
                false)

    static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall
                                        when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                        and ^Awaiter :> ICriticalNotifyCompletion
                                        and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                        and ^Awaiter: (member GetResult:  unit ->  ^TResult1)>
              (_priority: IPriority2, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

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
                    (__expand_continuation result).Invoke(&sm)
            else
                Witnesses.CanBindDynamic< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter , 'TOverall>(_priority, task, __expand_continuation).Invoke(&sm))

    static member inline CanBindDynamic (_priority: IPriority1, task: Task<'TResult1>, continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let cont = 
                (TaskMachineFunc<'TOverall>(fun sm -> 
                    //Console.WriteLine("[{0}] resumed CanBind(Task)", sm.MethodBuilder.Task.Id)
                    let result = awaiter.GetResult()
                    (continuation result).Invoke(&sm)))

            // shortcut to continue immediately
            if awaiter.IsCompleted then 
                cont.Invoke(&sm)
            else
                //Console.WriteLine("[{0}] setting ResumptionFunc for CanBind Task", sm.MethodBuilder.Task.Id)
                sm.Awaiter <- awaiter
                sm.ResumptionFunc <- cont
                false)

    static member inline CanBind (_priority: IPriority1, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =

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
                    (__expand_continuation result).Invoke(&sm)
            else
                Witnesses.CanBindDynamic(_priority, task, __expand_continuation).Invoke(&sm))

    static member inline CanBind (_priority: IPriority1, computation: Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskCode<'TOverall, 'TResult2>)) : TaskCode<'TOverall, 'TResult2> =
        Witnesses.CanBind (_priority, Async.StartAsTask computation, __expand_continuation)

    static member inline CanReturnFromDynamic< ^TaskLike, ^Awaiter, ^T
                                       when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                       and ^Awaiter :> ICriticalNotifyCompletion
                                       and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                       and ^Awaiter: (member GetResult: unit ->  ^T)>
          (_priority: IPriority2, task: ^TaskLike) : TaskCode< ^T, ^T > =

        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = (^TaskLike: (member GetAwaiter : unit -> ^Awaiter)(task)) 

            let cont =
                (TaskMachineFunc< ^T >(fun sm -> 
                    //Console.WriteLine("[{0}] resumed CanReturnFrom(TaskLike)", sm.MethodBuilder.Task.Id)
                    sm.Result <- (^Awaiter : (member GetResult : unit -> ^T)(awaiter))
                    true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then 
                cont.Invoke(&sm)
            else
                //Console.WriteLine("[{0}] setting ResumptionFunc for CanReturnFrom TaskLike", sm.MethodBuilder.Task.Id)
                sm.Awaiter <- awaiter
                sm.ResumptionFunc <- cont
                false)

    static member inline CanReturnFrom< ^TaskLike, ^Awaiter, ^T
                                       when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                       and ^Awaiter :> ICriticalNotifyCompletion
                                       and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                       and ^Awaiter: (member GetResult: unit ->  ^T)>
          (_priority: IPriority2, task: ^TaskLike) : TaskCode< ^T, ^T > =

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
                Witnesses.CanReturnFromDynamic(_priority, task).Invoke(&sm))

    static member inline CanReturnFrom (_priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

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
                Witnesses.CanReturnFromDynamic(_priority, task).Invoke(&sm))

    static member CanReturnFromDynamic (_priority: IPriority1, task: Task<'T>) : TaskCode<'T, 'T> =

        TaskCode<_, _>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()

            let cont =
                (TaskMachineFunc<'T>(fun sm -> 
                    //Console.WriteLine("[{0}] resumed CanReturnFrom(Task<_>)", sm.MethodBuilder.Task.Id)
                    sm.Result <- awaiter.GetResult()
                    true))

            // shortcut to continue immediately
            if task.IsCompleted then
                cont.Invoke(&sm)
            else
                //Console.WriteLine("[{0}] setting ResumptionFunc for CanReturnFrom Task", sm.MethodBuilder.Task.Id)
                // If the task definition has not been converted to a state machine then a continuation function is used
                sm.Awaiter <- awaiter
                sm.ResumptionFunc <- cont
                false)

    static member inline CanReturnFrom (_priority: IPriority1, computation: Async<'T>) : TaskCode<'T, 'T> =
        Witnesses.CanReturnFrom (_priority, Async.StartAsTask computation)

[<AutoOpen>]
module TaskHelpers = 

    let task = TaskBuilder()

    type TaskBuilder with

        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2, 'TOverall
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * ^TaskLike * (^TResult1 -> TaskCode<'TOverall, 'TResult2>) -> TaskCode<'TOverall, 'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskCode<'TOverall, 'TResult2>) : TaskCode<'TOverall, 'TResult2> =

            RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> task __expand_continuation

        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * ^TaskLike -> TaskCode<'T, 'T>) > 
                    (task: ^TaskLike) : TaskCode<'T, 'T> =

            RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> task

(*

module ContextInsensitiveTasks =

    let task = TaskBuilder()

    [<Sealed; NoComparison; NoEquality>]
    type Witnesses() = 
        interface IPriority1
        interface IPriority2
        interface IPriority3

        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaiter, 'TOverall
                                            when  ^TaskLike: (member GetAwaiter:  unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted:  unit -> bool)
                                            and ^Awaiter: (member GetResult:  unit ->  ^TResult1)> 
                     (_priority: IPriority3, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            // get an awaiter from the task
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
                __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))

        [<NoDynamicInvocation>]
        static member inline CanBind< ^TaskLike, ^TResult1, 'TResult2, ^Awaitable, ^Awaiter , 'TOverall
                                            when  ^TaskLike: (member ConfigureAwait:  bool ->  ^Awaitable)
                                            and ^Awaitable: (member GetAwaiter: unit ->  ^Awaiter)
                                            and ^Awaiter :> ICriticalNotifyCompletion
                                            and ^Awaiter: (member get_IsCompleted: unit -> bool)
                                            and ^Awaiter: (member GetResult: unit -> ^TResult1)> 
                     (_priority: IPriority2, sm: TaskStateMachine<'TOverall>, task: ^TaskLike, __expand_continuation: (^TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =

            let awaitable = (^TaskLike : (member ConfigureAwait : bool -> ^Awaitable)(task, false))
            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(awaitable))
            let CONT = __resumableEntry (fun () -> __expand_continuation (^Awaiter : (member GetResult : unit -> ^TResult1)(awaiter))) 
            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority :IPriority1, sm: TaskStateMachine<'TOverall>, task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __resumableEntry (fun () -> __expand_continuation (awaiter.GetResult()))
            if awaiter.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'TResult2>(false)

        [<NoDynamicInvocation>]
        static member inline CanBind (_priority: IPriority1, sm: TaskStateMachine<'TOverall>, computation : Async<'TResult1>, __expand_continuation: ('TResult1 -> TaskStep<'TResult2>)) : TaskStep<'TResult2> =
            Witnesses.CanBind (_priority, sm, Async.StartAsTask computation, __expand_continuation)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom< ^Awaitable, ^Awaiter, ^T
                                    when ^Awaitable : (member GetAwaiter : unit -> ^Awaiter)
                                    and ^Awaiter :> ICriticalNotifyCompletion 
                                    and ^Awaiter : (member get_IsCompleted : unit -> bool)
                                    and ^Awaiter : (member GetResult : unit -> ^T) >
               (_priority: IPriority3, sm: TaskStateMachine< ^T >, task: ^Awaitable) =

            // get an awaiter from the task
            let mutable awaiter = (^Awaitable : (member GetAwaiter : unit -> ^Awaiter)(task))
            let CONT = __resumableEntry (fun () -> sm.SetResult (^Awaiter : (member GetResult : unit -> ^T)(awaiter)); TaskStep<'T>(true))

            // shortcut to continue immediately
            if (^Awaiter : (member get_IsCompleted : unit -> bool)(awaiter)) then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep< ^T >(false)
        
        [<NoDynamicInvocation>]
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

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, sm: TaskStateMachine<'T>, task: Task<'T>) : TaskStep<'T> =
            let mutable awaiter = task.ConfigureAwait(false).GetAwaiter()
            let CONT = __resumableEntry (fun () -> sm.SetResult (awaiter.GetResult()); TaskStep<'T>(true))
            if task.IsCompleted then
                CONT.Invoke(&sm)
            else
                sm.Await (&awaiter, CONT)
                TaskStep<'T>(false)

        [<NoDynamicInvocation>]
        static member inline CanReturnFrom (_priority: IPriority1, sm: TaskStateMachine<'T>, computation: Async<'T>) =
            Witnesses.CanReturnFrom (_priority, sm, Async.StartAsTask computation)

    type TaskStateMachine<'TOverall> with
        [<NoDynamicInvocation>]
        member inline __.Bind< ^TaskLike, ^TResult1, 'TResult2 
                                           when (Witnesses or  ^TaskLike): (static member CanBind: Witnesses * TaskStateMachine<'TOverall> * ^TaskLike * (^TResult1 -> TaskStep<'TResult2>) -> TaskStep<'TResult2>)> 
                    (task: ^TaskLike, __expand_continuation: ^TResult1 -> TaskStep<'TResult2>) : TaskStep<'TResult2> =
            RequireCanBind< Witnesses, ^TaskLike, ^TResult1, 'TResult2, 'TOverall> Unchecked.defaultof<Witnesses> sm task __expand_continuation

        [<NoDynamicInvocation>]
        member inline __.ReturnFrom< ^TaskLike, 'T  when (Witnesses or ^TaskLike): (static member CanReturnFrom: Witnesses * TaskStateMachine<'T> * ^TaskLike -> TaskStep<'T>) > (task: ^TaskLike) : TaskStep<'T> 
                  = RequireCanReturnFrom< Witnesses, ^TaskLike, 'T> Unchecked.defaultof<Witnesses> sm task
*)

#endif
