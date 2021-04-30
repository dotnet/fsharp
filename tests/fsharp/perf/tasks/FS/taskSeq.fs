
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
    
[<ResumableCode>]
type TaskSeqCode<'T> = delegate of TaskSeqStateMachine<'T> -> TaskSeqStatus

type TaskSeqBuilder() =

    member inline _.Delay(f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> f().Invoke(sm))


    member inline _.Run(code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        let sm = 
            { new TaskSeqStateMachine<'T>(-1) with 
                [<ResumableCode>]
                member sm.Step () = 
                    if __useResumableCode then
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint
                        code.Invoke(sm)
                    else 
                        let code = sm.ResumptionFunc sm
                        match code with 
                        | TaskSeqStatus.AWAIT ->
                            sm.Awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNext()))
                        | _ -> ()
                        code
                    //-- RESUMABLE CODE END
            }
        if not __useResumableCode then
            sm.ResumptionFunc <- (fun sm -> code.Invoke(sm))
        sm.Start()


    member inline _.Zero() : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun _sm -> TaskSeqStatus.DONE)

    member inline _.Combine(task1: TaskSeqCode<'T>, task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then
                let __stack_step = task1.Invoke(sm)
                if __stack_step = TaskSeqStatus.DONE then
                    task2.Invoke(sm)
                else
                    __stack_step
            else

                let step = task1.Invoke(sm)
                if step = TaskSeqStatus.DONE then
                    task2.Invoke(sm)
                else 
                    // Adjust the resumption to also run task2 on completion
                    let rec resume rf =
                        (fun (sm :TaskSeqStateMachine<_>) -> 
                            let step = rf sm
                            if step = TaskSeqStatus.DONE then 
                                task2.Invoke(sm)
                            else
                                sm.ResumptionFunc <- resume sm.ResumptionFunc
                                step)

                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                    step)
            
    member inline _.WhileAsync([<InlineIfLambda>] condition : unit -> ValueTask<bool>, body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then
                //-- RESUMABLE CODE START
                let mutable __stack_step = TaskSeqStatus.DONE
                let mutable __stack_proceed = true
                while __stack_proceed do
                    if verbose then printfn $"starting guard task"
                    let __stack_guard = condition()
                    if __stack_guard.IsCompleted then
                        if verbose then printfn $"guard task was synchronous"
                        __stack_proceed <- __stack_guard.Result
                    else
                        // Async wait for guard task
                        if verbose then printfn $"async wait for guard task"
                        let mutable awaiter = __stack_guard.AsTask().GetAwaiter() // **
                        match __resumableEntry() with 
                        | Some contID ->
                            if awaiter.IsCompleted then // **
                                __resumeAt contID
                            else
                                __stack_step <- sm.AwaitCompleted(awaiter, contID) // **
                                __stack_proceed <- false
                        | None ->
                             // RESUME - we may jump directly to here on resumption. 'awaiter' will be captured and available
                            __stack_proceed <- awaiter.GetResult()

                    if __stack_proceed then
                        let __stack_step2 = body.Invoke(sm)
                        __stack_step <- __stack_step2
                        __stack_proceed <- (__stack_step = TaskSeqStatus.DONE)
                __stack_step
                //-- RESUMABLE CODE END
            else
                failwith "reflective execution of TaskSeq While loop NYI")

    member inline b.While([<InlineIfLambda>] condition : unit -> bool, body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        b.WhileAsync((fun () -> ValueTask<bool>(condition())), body)

    member inline _.TryWith(body : TaskSeqCode<'T>, catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then
                let mutable __stack_step = TaskSeqStatus.DONE
                let mutable __stack_caught = false
                let mutable __stack_exn = Unchecked.defaultof<_>
                try
                    let __stack_step2 = body.Invoke(sm)
                    __stack_step <- __stack_step2
                with exn -> 
                    __stack_caught <- true
                    __stack_exn <- exn

                if __stack_caught then 
                    (catch __stack_exn).Invoke sm
                else
                    __stack_step
            else
                failwith "reflective execution of TaskSeq TryWith NYI")

    member inline _.TryFinallyAsync(body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then
                let mutable __stack_step = TaskSeqStatus.DONE
                sm.PushDispose compensation
                try
                    let __stack_step2 = body.Invoke sm
                    __stack_step <- __stack_step2
                with _ ->
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                    reraise()

                if __stack_step = TaskSeqStatus.DONE then 
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                __stack_step
            else
                failwith "reflective execution of TaskSeq TryFinallyAsync NYI")

    member inline this.TryFinally(body: TaskSeqCode<'T>, compensation : unit -> unit) : TaskSeqCode<'T> =
        this.TryFinallyAsync(body, fun () -> Task.FromResult(compensation()))

    member inline this.Using(disp : #IDisposable, body : #IDisposable -> TaskSeqCode<'T>) : TaskSeqCode<'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> (body disp).Invoke sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.UsingAsync(disp : #IAsyncDisposable, body : #IAsyncDisposable -> TaskSeqCode<'T>) : TaskSeqCode<'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinallyAsync(
            (fun sm -> (body disp).Invoke sm),
            (fun () -> 
                if not (isNull (box disp)) then 
                    // TODO should be async
                    (disp.DisposeAsync().AsTask().Wait(); Task.FromResult())
                else Task.FromResult()))

    member inline this.For(sequence : seq<'TElement>, body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke sm))))

    member inline this.For(source: #IAsyncEnumerable<'TElement>, body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            this.UsingAsync(source.GetAsyncEnumerator(sm.CancellationToken), 
                (fun e -> this.WhileAsync((fun () -> e.MoveNextAsync()), 
                                          (fun sm -> (body e.Current).Invoke sm)))).Invoke sm)

    member inline _.Yield (v: 'T) : TaskSeqCode<'T>  =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then 
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

    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    member inline _.Bind (task: Task<'TResult1>, continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            if __useResumableCode then 
                let mutable awaiter = task.GetAwaiter()
                match __resumableEntry() with 
                | Some contID ->
                    if awaiter.IsCompleted then 
                        __resumeAt contID
                    else
                        sm.AwaitCompleted(awaiter, contID)
                | None ->
                     // RESUME - we may jump directly to here on resumption. 'awaiter' will be captured and available
                    (continuation (awaiter.GetResult())).Invoke sm

            else
                let mutable awaiter = task.GetAwaiter()
                let cont sm = (continuation (awaiter.GetResult())).Invoke sm
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

    let dumpTaskSeq (t: IAsyncEnumerable<_>) = 
        printfn "-----"
        let e = t.GetAsyncEnumerator(CancellationToken())
        while (let vt = e.MoveNextAsync() in if vt.IsCompleted then vt.Result else vt.AsTask().Result) do 
            printfn "yield %A" e.Current

    dumpTaskSeq (t1())
    dumpTaskSeq (t2())

    printfn "t1() = %A" (TaskSeq.toArray (t1()))
    printfn "t2() = %A" (TaskSeq.toArray (t2()))

    printfn "perf2() = %A" (TaskSeq.toArray (perf2()))

