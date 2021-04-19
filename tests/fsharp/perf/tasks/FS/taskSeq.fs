
module Tests.TaskSeqBuilder

open System.Threading.Tasks.Sources

#nowarn "42"
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

let verbose = false


type TaskSeqStatus =  DONE = 0 | YIELD = 1 | AWAIT = 2

type taskSeq<'T> = IAsyncEnumerable<'T>

[<AbstractClass; NoComparison; NoEquality>]
type TaskSeqStateMachine<'T>(state: int) =
    let mutable cancellationToken : CancellationToken = Unchecked.defaultof<_>
    let mutable disposalStack : ResizeArray<(unit -> Task<unit>)> = Unchecked.defaultof<_>
    let mutable awaiter : ICriticalNotifyCompletion  = Unchecked.defaultof<_> 
    let initialThreadId = Environment.CurrentManagedThreadId
    let mutable promiseOfValueOrEnd: ManualResetValueTaskSourceCore<bool> = Unchecked.defaultof<_>
    let mutable builder : AsyncIteratorMethodBuilder = Unchecked.defaultof<_>
    let mutable state : int = state

    member _.Awaiter with get() = awaiter and set v = awaiter <- v
    member _.ResumptionPoint with get() = state and set v = state <- v
    member _.CancellationToken = cancellationToken
    member _.MethodBuilder = builder
    member val Current : ValueOption<'T> = Unchecked.defaultof<_> with get, set
    member val ResumptionFunc : TaskMachineFunc<'T> = Unchecked.defaultof<_> with get, set

    /// Proceed to the next state or raise an exception
    abstract Step : unit -> TaskSeqStatus

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.AwaitCompleted(awaiter: TaskAwaiter<_>, contID) =
        state <- contID
        let mutable awaiter = awaiter
        let mutable sm = sm
        if verbose then printfn "calling AwaitUnsafeOnCompleted"
        sm.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
        TaskSeqStatus.AWAIT

    interface IValueTaskSource with
        member _.GetResult(token: int16) = 
            if verbose then printfn "IValueTaskSource.GetResult..."
            promiseOfValueOrEnd.GetResult(token) |> ignore
        member _.GetStatus(token: int16) =
            promiseOfValueOrEnd.GetStatus(token)
        member _.OnCompleted(continuation, state, token, flags) =
            promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags)

    interface IValueTaskSource<bool> with
        member _.GetStatus(token: int16) =
            promiseOfValueOrEnd.GetStatus(token)
        member _.GetResult(token: int16) = 
            if verbose then printfn "IValueTaskSource<bool>.GetResult..."
            promiseOfValueOrEnd.GetResult(token)
        member _.OnCompleted(continuation, state, token, flags) =
            promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags)

    member _.PushDispose (f: unit -> Task<unit>) =
        match disposalStack with 
        | null -> disposalStack <- ResizeArray()
        | _ -> ()
        disposalStack.Add(f)

    member _.PopDispose () =
        match disposalStack with 
        | null -> ()
        | _ -> 
            disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member sm.MoveNext() : unit =
        try
            //Console.WriteLine("[{0}] resuming by invoking {1}....", sm.MethodBuilder.Task.Id, hashq sm.ResumptionFunc )
            let step = sm.Step ()
            match step with 
            | TaskSeqStatus.AWAIT -> 
                ()
            | TaskSeqStatus.YIELD -> 
                promiseOfValueOrEnd.SetResult(true)
            | _ (* TaskSeqStatus.DONE *) -> 
                //Console.WriteLine("[{0}] SetResult {1}", sm.MethodBuilder.Task.Id, sm.Result)
                promiseOfValueOrEnd.SetResult(false)
                builder.Complete()

        with exn ->
            //Console.WriteLine("[{0}] SetException {1}", sm.MethodBuilder.Task.Id, exn)
            promiseOfValueOrEnd.SetException(exn)
            builder.Complete()

    interface IAsyncStateMachine with 
        
        member sm.MoveNext() = sm.MoveNext()

        member _.SetStateMachine(_state) = () // not needed for reference type

    interface System.Collections.Generic.IAsyncEnumerable<'T> with
        member sm.GetAsyncEnumerator(ct) = 
            if (sm.ResumptionPoint = -1 && initialThreadId = Environment.CurrentManagedThreadId) then
                if verbose then printfn "GetAsyncEnumerator, reusing..."
                sm.ResumptionPoint <- 0;
                builder <- AsyncIteratorMethodBuilder.Create();
                (sm :> IAsyncEnumerator<_>)
                //<>w__disposeMode = false;
                //<perf1_AsyncEnumerable>d__ = this;
            else
                if verbose then printfn "GetAsyncEnumerator, cloning..."
                let clone = sm.MemberwiseClone() :?> TaskSeqStateMachine<'T>
                clone.ResumptionPoint <- 0
                //clone.CancellationToken <- ct
                (clone :> System.Collections.Generic.IAsyncEnumerator<'T>)

    interface IAsyncDisposable  with
        member _.DisposeAsync() =
            if verbose then printfn "DisposeAsync..." 
            task {
                match disposalStack with 
                | null -> ()
                | _ -> 
                let mutable exn = None
                for d in Seq.rev disposalStack do 
                    try 
                      do! d()
                    with e ->
                      if exn.IsNone then 
                          exn <- Some e
                match exn with 
                | None -> () 
                | Some e -> raise e 
            }
            |> ValueTask
       

    interface System.Collections.Generic.IAsyncEnumerator<'T> with
        
        member sm.Current = match sm.Current with ValueSome x -> x | ValueNone -> failwith "no current value"
        
        member sm.MoveNextAsync() = 
            if verbose then printfn "MoveNextAsync..."
            if state = -1 then // can't use as IAsyncEnumerator before IAsyncEnumerable
                ValueTask<bool>()
            else
                if verbose then printfn "MoveNextAsync moving..."
                promiseOfValueOrEnd.Reset()
                let mutable stateMachine = sm
                builder.MoveNext(&stateMachine)
                let version = promiseOfValueOrEnd.Version
                if verbose then printfn $"MoveNextAsync moved, promiseOfValueOrEnd.Version = {version}..." 
                if verbose then printfn $"MoveNextAsync moved, promiseOfValueOrEnd.GetStatus(version) = {promiseOfValueOrEnd.GetStatus(version)}..." 
                if promiseOfValueOrEnd.GetStatus(version) = ValueTaskSourceStatus.Succeeded then
                    if verbose then printfn "MoveNextAsync Succeeded..."
                    new ValueTask<bool>(promiseOfValueOrEnd.GetResult(version))
                else
                    if verbose then printfn "MoveNextAsync Pending?..."
                    new ValueTask<bool>(sm, version)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.Start() = (sm :> IAsyncEnumerable<'T>)

and TaskMachineFunc<'T> = TaskSeqStateMachine<'T> -> TaskSeqStatus
    
type TaskSeqCode<'T> = TaskSeqStateMachine<'T> -> TaskSeqStatus
type TaskSeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline _.Delay(__expand_f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline _.Run(__expand_code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        if __useResumableStateMachines then
            (__resumableStateMachine
                { new TaskSeqStateMachine<'T>(-1) with 
                    member sm.Step () = 
                        __resumeAt sm.ResumptionPoint
                        __expand_code sm
                }
            ).Start()
        else
            let sm = 
                { new TaskSeqStateMachine<'T>(-1) with 
                    member sm.Step () = 
                        let code = sm.ResumptionFunc sm
                        match code with 
                        | TaskSeqStatus.AWAIT ->
                            sm.Awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNext()))
                        | _ -> ()
                        code
                }
            sm.ResumptionFunc <- __expand_code
            sm.Start()

    [<NoDynamicInvocation>]
    member inline _.Zero() : TaskSeqCode<'T> =
        (fun _sm -> TaskSeqStatus.DONE)

    [<NoDynamicInvocation>]
    member inline _.Combine(__expand_task1: TaskSeqCode<'T>, __expand_task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let __stack_step = __expand_task1 sm
                if __stack_step = TaskSeqStatus.DONE then
                    __expand_task2 sm
                else
                    __stack_step
            else

                let step = __expand_task1 sm
                if step = TaskSeqStatus.DONE then
                    __expand_task2 sm
                else 
                    // Adjust the resumption to also run __expand_task2 on completion
                    let rec resume rf =
                        (fun (sm :TaskSeqStateMachine<_>) -> 
                            let step = rf sm
                            if step = TaskSeqStatus.DONE then 
                                __expand_task2 sm
                            else
                                sm.ResumptionFunc <- resume sm.ResumptionFunc
                                step)

                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                    step)
            
    [<NoDynamicInvocation>]
    member inline _.While(__expand_condition : unit -> bool, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let mutable __stack_step = TaskSeqStatus.DONE
                while (__stack_step = TaskSeqStatus.DONE) && __expand_condition() do
                    //if verbose then Console.WriteLine("[{0}] while loop before body", sm.GetHashCode())
                    let __stack_step2 = __expand_body sm
                    __stack_step <- __stack_step2
                    //if verbose then Console.WriteLine("[{0}] while loop after body, step = {1}", sm.GetHashCode(), step)
                //if verbose then Console.WriteLine("[{0}] finishing while with {1}", sm.GetHashCode(), step)
                __stack_step
            else
                failwith "reflective execution of TaskSeq While loop NYI")

            
    [<NoDynamicInvocation>]
    member inline _.WhileAsync(__expand_condition : unit -> ValueTask<bool>, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let mutable __stack_step = TaskSeqStatus.DONE
                let mutable __stack_fin = false
                while not __stack_fin do
                    if verbose then printfn $"starting guard task"
                    let __stack_guard = __expand_condition()
                    if __stack_guard.IsCompleted then
                        if verbose then printfn $"guard task was synchronous"
                        __stack_fin <- not __stack_guard.Result
                    else
                        // Async wait for guard task
                        if verbose then printfn $"async wait for guard task"
                        let mutable awaiter = __stack_guard.AsTask().GetAwaiter()
                        match __resumableEntry() with 
                        | Some contID ->
                            if awaiter.IsCompleted then 
                                __resumeAt contID
                            else
                                __stack_step <- sm.AwaitCompleted(awaiter, contID)
                                __stack_fin <- true
                        | None ->
                             // RESUME - we may jump directly to here on resumption. 'awaiter' will be captured and available
                            __stack_fin <- not (awaiter.GetResult())

                    if not __stack_fin then
                        let __stack_step2 = __expand_body sm
                        __stack_step <- __stack_step2
                        __stack_fin <- not (__stack_step = TaskSeqStatus.DONE)
                __stack_step
            else
                failwith "reflective execution of TaskSeq While loop NYI")

    [<NoDynamicInvocation>]
    member inline _.TryWith(__expand_body : TaskSeqCode<'T>, __expand_catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let mutable __stack_step = TaskSeqStatus.DONE
                let mutable __stack_caught = false
                let mutable __stack_exn = Unchecked.defaultof<_>
                try
                    let __stack_step2 = __expand_body sm
                    __stack_step <- __stack_step2
                with exn -> 
                    __stack_caught <- true
                    __stack_exn <- exn

                if __stack_caught then 
                    __expand_catch __stack_exn sm
                else
                    __stack_step
            else
                failwith "reflective execution of TaskSeq TryWith NYI")

    [<NoDynamicInvocation>]
    member inline _.TryFinallyAsync(__expand_body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                //if verbose then Console.WriteLine("[{0}] enter try/finally", sm.GetHashCode())
                let mutable __stack_step = TaskSeqStatus.DONE
                sm.PushDispose compensation
                try
                    //if verbose then Console.WriteLine("[{0}] try/finally before body", sm.GetHashCode())
                    let __stack_step2 = __expand_body sm
                    //if verbose then Console.WriteLine("[{0}] try/finally after body", sm.GetHashCode())
                    __stack_step <- __stack_step2
                with _ ->
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                    reraise()

                if __stack_step = TaskSeqStatus.DONE then 
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                //if verbose then Console.WriteLine("[{0}] finishing try/finally with {1}", sm.GetHashCode(), step)
                __stack_step
            else
                failwith "reflective execution of TaskSeq TryFinallyAsync NYI")

    [<NoDynamicInvocation>]
    member inline this.TryFinally(__expand_body: TaskSeqCode<'T>, compensation : unit -> unit) : TaskSeqCode<'T> =
        this.TryFinallyAsync(__expand_body, fun () -> Task.FromResult(compensation()))

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> TaskSeqCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> __expand_body disp sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.UsingAsync(disp : #IAsyncDisposable, __expand_body : #IAsyncDisposable -> TaskSeqCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinallyAsync(
            (fun sm -> __expand_body disp sm),
            (fun () -> 
                if not (isNull (box disp)) then 
                    // TODO should be async
                    (disp.DisposeAsync().AsTask().Wait(); Task.FromResult())
                else Task.FromResult()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> __expand_body e.Current sm))))

    [<NoDynamicInvocation>]
    member inline this.For(source: #IAsyncEnumerable<'TElement>, __expand_body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            this.UsingAsync(source.GetAsyncEnumerator(sm.CancellationToken), 
                (fun e -> this.WhileAsync((fun () -> e.MoveNextAsync()), 
                                          (fun sm -> __expand_body e.Current sm)))) sm)

    [<NoDynamicInvocation>]
    member inline _.Yield (v: 'T) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then 
                match __resumableEntry() with
                | Some contID ->
                    //if verbose then Console.WriteLine("[{0}] suspending at yield of {1}", sm.GetHashCode(), v)
                    sm.ResumptionPoint <- contID
                    sm.Current <- ValueSome v
                    TaskSeqStatus.YIELD
                | None -> 
                    //if verbose then Console.WriteLine("[{0}] resuming after yield of {1}", sm.GetHashCode(), v)
                    TaskSeqStatus.DONE
            else
                sm.ResumptionFunc <- (fun sm -> TaskSeqStatus.DONE)
                sm.Current <- ValueSome v
                TaskSeqStatus.YIELD)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    [<NoDynamicInvocation>]
    member inline _.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then 
                let mutable awaiter = task.GetAwaiter()
                match __resumableEntry() with 
                | Some contID ->
                    if awaiter.IsCompleted then 
                        __resumeAt contID
                    else
                        sm.AwaitCompleted(awaiter, contID)
                | None ->
                     // RESUME - we may jump directly to here on resumption. 'awaiter' will be captured and available
                    __expand_continuation (awaiter.GetResult()) sm

            else
                let mutable awaiter = task.GetAwaiter()
                let cont sm = __expand_continuation (awaiter.GetResult()) sm
                if awaiter.IsCompleted then 
                    cont sm
                else
                    sm.ResumptionFunc <- cont
                    sm.Awaiter <- awaiter
                    TaskSeqStatus.AWAIT)

let taskSeq = TaskSeqBuilder()


module TaskSeq =
    let toList (t: taskSeq<'T>) =
        [ let e = t.GetAsyncEnumerator(CancellationToken())
          try 
              while (let vt = e.MoveNextAsync() in if vt.IsCompleted then vt.Result else vt.AsTask().Result) do 
                  yield e.Current
          finally
              e.DisposeAsync().AsTask().Wait() ]

    let toArray (t: taskSeq<'T>) =
        [| let e = t.GetAsyncEnumerator(CancellationToken())
           try 
              while (let vt = e.MoveNextAsync() in if vt.IsCompleted then vt.Result else vt.AsTask().Result) do 
                   yield e.Current
           finally 
               e.DisposeAsync().AsTask().Wait() |]

    let toArrayAsync (t: taskSeq<'T>) : Task<'T[]> =
        task { 
           let res = ResizeArray<'T>()
           let e = t.GetAsyncEnumerator(CancellationToken())
           let mutable go = true
           let! step = e.MoveNextAsync()
           go <- step
           while go do 
               res.Add e.Current
               if verbose then printfn "yield %A" e.Current
               let! step = e.MoveNextAsync()
               go <- step
           return res.ToArray()
        }

    let iter f (t: taskSeq<'T>) =
        let e = t.GetAsyncEnumerator(CancellationToken())
        try 
            while (let vt = e.MoveNextAsync() in if vt.IsCompleted then vt.Result else vt.AsTask().Result) do 
                f e.Current
        finally 
            e.DisposeAsync().AsTask().Wait()


module Examples =

    let t1 () = 
        taskSeq {
           printfn "in t1"
           yield "a"
           let x = 1
           let! v =
               task { 
                   printfn "hey"
                   do! Task.Delay(10) 
               }
           yield "b"
           let! v =
               task { 
                   printfn "hey yo"
                   do! Task.Delay(10) 
               }
           yield "c"
        }

    let t2 () = 
        taskSeq {
           printfn "in t2"
           yield "d"
           printfn "in t2 b"
           for x in t1 () do 
               printfn "t2 - got %A" x
               yield "e"
               let! v = 
                   task {
                       printfn "hey yo"
                       do! Task.Delay(200) 
                   }
               yield "[T1]" + x
           let! v =
               task {
                   printfn "hey yo"
                   do! Task.Delay(10)
               }
           yield "f"
        }

    let perf1 (x: int) = 
        taskSeq {
           yield 1
           yield 2
           if x >= 2 then 
               yield 3
               yield 4
        }

    let perf2 () = 
        taskSeq {
           for i1 in perf1 3 do
             for i2 in perf1 3 do
               for i3 in perf1 3 do
                 for i4 in perf1 3 do
                   for i5 in perf1 3 do
                      yield! perf1 i5
        }

    let perf1_AsyncSeq (x: int) = 
        FSharp.Control.AsyncSeqExtensions.asyncSeq {
           yield 1
           yield 2
           if x >= 2 then 
               yield 3
               yield 4
        }

    let perf2_AsyncSeq () = 
        FSharp.Control.AsyncSeqExtensions.asyncSeq {
           for i1 in perf1_AsyncSeq 3 do
             for i2 in perf1_AsyncSeq 3 do
               for i3 in perf1_AsyncSeq 3 do
                 for i4 in perf1_AsyncSeq 3 do
                   for i5 in perf1_AsyncSeq 3 do
                     yield! perf1_AsyncSeq i5
        }

    //let dumpTaskSeq (t: IAsyncEnumerable<_>) = 
    //    printfn "-----"
    //    let e = t.GetAsyncEnumerator(CancellationToken())
    //    while (let vt = e.MoveNextAsync() in if vt.IsCompleted then vt.Result else vt.AsTask().Result) do 
    //        printfn "yield %A" e.Current

    //dumpTaskSeq (t1())
    //dumpTaskSeq (t2())

    // printfn "t1() = %A" (TaskSeq.toArray (t1()))
    // printfn "t2() = %A" (TaskSeq.toArray (t2()))

    // printfn "perf2() = %A" (TaskSeq.toArray (perf2()))

