
module Tests.TaskSeqUsingCoroutines

open System.Runtime.CompilerServices
open System.Threading.Tasks.Sources

#nowarn "42"
open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

let verbose = false

let inline MoveNext(x: byref<'T> when 'T :> IAsyncStateMachine) = x.MoveNext()

type TaskSeqStatus =  DONE = 0 | YIELD = 1 | AWAIT = 2

type taskSeq<'T> = IAsyncEnumerable<'T>

[<Struct; NoComparison; NoEquality>]
type TaskSeqStateMachineData<'T> =
    [<DefaultValue(false)>]
    val mutable cancellationToken : CancellationToken
    [<DefaultValue(false)>]
    val mutable disposalStack : ResizeArray<(unit -> Task<unit>)>
    [<DefaultValue(false)>]
    val mutable awaiter : ICriticalNotifyCompletion
    [<DefaultValue(false)>]
    val mutable promiseOfValueOrEnd: ManualResetValueTaskSourceCore<bool>
    [<DefaultValue(false)>]
    val mutable builder : AsyncIteratorMethodBuilder
    [<DefaultValue(false)>]
    val mutable taken : bool
    [<DefaultValue(false)>]
    val mutable current : ValueOption<'T>

    member data.PushDispose (f: unit -> Task<unit>) =
        match data.disposalStack with 
        | null -> data.disposalStack <- ResizeArray()
        | _ -> ()
        data.disposalStack.Add(f)

    member data.PopDispose () =
        match data.disposalStack with 
        | null -> ()
        | _ -> 
            data.disposalStack.RemoveAt(data.disposalStack.Count - 1)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member AwaitCompleted(rsm: byref<ResumableStateMachine<TaskSeqStateMachineData<'TOverall>>>, awaiter: TaskAwaiter<_>, contID) =
        rsm.ResumptionPoint <- contID
        let mutable awaiter = awaiter
        if verbose then printfn "calling AwaitUnsafeOnCompleted"
        rsm.Data.builder.AwaitUnsafeOnCompleted(&awaiter, &rsm)
        false

[<AbstractClass; NoComparison; NoEquality>]
type TaskSeqStateMachine<'T>() =
    let initialThreadId = Environment.CurrentManagedThreadId
    [<DefaultValue(false)>]
    val mutable Machine : ResumableStateMachine<TaskSeqStateMachineData<'T>>

    member sm.Awaiter with get() = sm.Machine.Data.awaiter and set v = sm.Machine.Data.awaiter <- v
    member sm.ResumptionPoint with get() = sm.Machine.ResumptionPoint and set v = sm.Machine.ResumptionPoint <- v
    member sm.CancellationToken = sm.Machine.Data.cancellationToken
    member sm.MethodBuilder = sm.Machine.Data.builder

    /// Proceed to the next state or raise an exception
    abstract Step : unit -> TaskSeqStatus

    interface IValueTaskSource with
        member sm.GetResult(token: int16) = 
            if verbose then printfn "IValueTaskSource.GetResult..."
            sm.Machine.Data.promiseOfValueOrEnd.GetResult(token) |> ignore
        member sm.GetStatus(token: int16) =
            sm.Machine.Data.promiseOfValueOrEnd.GetStatus(token)
        member sm.OnCompleted(continuation, state, token, flags) =
            sm.Machine.Data.promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags)

    interface IValueTaskSource<bool> with
        member sm.GetStatus(token: int16) =
            sm.Machine.Data.promiseOfValueOrEnd.GetStatus(token)
        member sm.GetResult(token: int16) = 
            if verbose then printfn "IValueTaskSource<bool>.GetResult..."
            sm.Machine.Data.promiseOfValueOrEnd.GetResult(token)
        member sm.OnCompleted(continuation, state, token, flags) =
            sm.Machine.Data.promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags)

    member sm.MoveNext() : unit =
        try
            //Console.WriteLine("[{0}] resuming by invoking {1}....", sm.MethodBuilder.Task.Id, hashq sm.ResumptionFunc )
            sm.Machine.Data.current <- ValueNone
            let step = sm.Step ()
            match step with 
            | TaskSeqStatus.AWAIT -> 
                ()
            | TaskSeqStatus.YIELD -> 
                sm.Machine.Data.promiseOfValueOrEnd.SetResult(true)
            | _ (* TaskSeqStatus.DONE *) -> 
                //Console.WriteLine("[{0}] SetResult {1}", sm.MethodBuilder.Task.Id, sm.Result)
                sm.Machine.Data.promiseOfValueOrEnd.SetResult(false)
                sm.Machine.Data.builder.Complete()

        with exn ->
            //Console.WriteLine("[{0}] SetException {1}", sm.MethodBuilder.Task.Id, exn)
            sm.Machine.Data.promiseOfValueOrEnd.SetException(exn)
            sm.Machine.Data.builder.Complete()

    interface IAsyncStateMachine with 
        
        member sm.MoveNext() = sm.MoveNext()

        member _.SetStateMachine(_state) = () // not needed for reference type

    interface System.Collections.Generic.IAsyncEnumerable<'T> with
        member sm.GetAsyncEnumerator(ct) = 
            if (not sm.Machine.Data.taken && initialThreadId = Environment.CurrentManagedThreadId) then
                if verbose then printfn "GetAsyncEnumerator, reusing..."
                sm.Machine.Data.taken <- true
                sm.Machine.Data.builder <- AsyncIteratorMethodBuilder.Create();
                (sm :> IAsyncEnumerator<_>)
                //<>w__disposeMode = false;
                //<perf1_AsyncEnumerable>d__ = this;
            else
                if verbose then printfn "GetAsyncEnumerator, cloning..."
                let clone = sm.MemberwiseClone() :?> TaskSeqStateMachine<'T>
                clone.Machine.Data.taken <- true
                //clone.CancellationToken <- ct
                (clone :> System.Collections.Generic.IAsyncEnumerator<'T>)

    interface IAsyncDisposable  with
        member sm.DisposeAsync() =
            if verbose then printfn "DisposeAsync..." 
            task {
                match sm.Machine.Data.disposalStack with 
                | null -> ()
                | _ -> 
                let mutable exn = None
                for d in Seq.rev sm.Machine.Data.disposalStack do 
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
        
        member sm.Current =
            match sm.Machine.Data.current with
            | ValueSome x -> x
            | ValueNone -> failwith "no current value"
        
        member sm.MoveNextAsync() = 
            if verbose then printfn "MoveNextAsync..."
            if sm.Machine.ResumptionPoint = -1 then // can't use as IAsyncEnumerator before IAsyncEnumerable
                ValueTask<bool>()
            else
                if verbose then printfn "MoveNextAsync moving..."
                sm.Machine.Data.promiseOfValueOrEnd.Reset()
                let mutable stateMachine = sm
                sm.Machine.Data.builder.MoveNext(&stateMachine)
                let version = sm.Machine.Data.promiseOfValueOrEnd.Version
                if verbose then printfn $"MoveNextAsync moved, promiseOfValueOrEnd.Version = {version}..." 
                if verbose then printfn $"MoveNextAsync moved, promiseOfValueOrEnd.GetStatus(version) = {sm.Machine.Data.promiseOfValueOrEnd.GetStatus(version)}..." 
                if sm.Machine.Data.promiseOfValueOrEnd.GetStatus(version) = ValueTaskSourceStatus.Succeeded then
                    if verbose then printfn "MoveNextAsync Succeeded..."
                    new ValueTask<bool>(sm.Machine.Data.promiseOfValueOrEnd.GetResult(version))
                else
                    if verbose then printfn "MoveNextAsync Pending?..."
                    new ValueTask<bool>(sm, version)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.Start() = (sm :> IAsyncEnumerable<'T>)

type TaskSeqCode<'T> = ResumableCode<TaskSeqStateMachineData<'T>, unit>
and TaskSeqResumption<'T> = ResumptionFunc<TaskSeqStateMachineData<'T>>
and TaskSeqResumptionExecutor<'T> = ResumptionFuncExecutor<TaskSeqStateMachineData<'T>>


type TaskSeqBuilder() =

    member inline _.Delay(f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> f().Invoke(&sm))


    member inline _.Run(code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        let sm = 
            { new TaskSeqStateMachine<'T>() with 
                [<ResumableCode>]
                member sm.Step () = 
                    if __useResumableCode then
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint
                        let __step_code_fin = code.Invoke(&sm.Machine)
                        if __step_code_fin then
                            if sm.Machine.Data.current.IsSome then
                                TaskSeqStatus.YIELD
                            else
                                TaskSeqStatus.DONE
                        else
                            TaskSeqStatus.AWAIT
                        //-- RESUMABLE CODE END
                    else 
                        // The MoveNext will call resumptionFuncExecutor which in turn calls the resumptionFunc
                        MoveNext(&sm.Machine)
                        if sm.ResumptionPoint = -2 then
                            TaskSeqStatus.DONE
                        elif sm.Machine.Data.current.IsSome then
                            TaskSeqStatus.YIELD
                        else
                            sm.Awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNext()))
                            TaskSeqStatus.AWAIT
            }
        if not __useResumableCode then
            let initialResumptionFunc = TaskSeqResumption<'T>(fun sm -> code.Invoke(&sm))
            let resumptionFuncExecutor = TaskSeqResumptionExecutor<'T>(fun sm f -> 
                    // TODO: add exception handling?
                    if f.Invoke(&sm) then 
                        sm.ResumptionPoint <- -2)
            let setStateMachine = SetStateMachineMethodImpl<_>(fun sm f -> ())
            sm.Machine.ResumptionFuncData <- (initialResumptionFunc, resumptionFuncExecutor, setStateMachine)
        sm.Start()


    member inline _.Zero() : TaskSeqCode<'T> =
        ResumableCode.Zero()

    member inline _.Combine(task1: TaskSeqCode<'T>, task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        ResumableCode.Combine(task1, task2)
            
    member inline _.WhileAsync([<InlineIfLambda>] condition : unit -> ValueTask<bool>, body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        let mutable condition_res = true
        ResumableCode.While((fun () -> condition_res), 
            ResumableCode<_,_>(fun sm -> 
                let mutable __stack_condition_fin = true
                let __stack_vtask = condition()
                if __stack_vtask.IsCompleted then
                    __stack_condition_fin <- true
                    condition_res <- __stack_vtask.Result
                else
                    let task = __stack_vtask.AsTask()
                    let mutable awaiter = task.GetAwaiter()
                    // This will yield with __stack_fin = false
                    // This will resume with __stack_fin = true
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    __stack_condition_fin <- __stack_yield_fin

                    if __stack_condition_fin then 
                        condition_res <- task.Result
                    else
                        //if verbose then printfn "calling AwaitUnsafeOnCompleted"
                        sm.Data.builder.AwaitUnsafeOnCompleted(&awaiter, &sm)

                if __stack_condition_fin then 
                    if condition_res then 
                        body.Invoke(&sm)
                    else
                        true
                else
                    false
                ))

    member inline b.While([<InlineIfLambda>] condition : unit -> bool, body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        b.WhileAsync((fun () -> ValueTask<bool>(condition())), body)

    member inline _.TryWith(body : TaskSeqCode<'T>, catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        ResumableCode.TryWith(body, catch)

    member inline _.TryFinallyAsync(body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        ResumableCode.TryFinallyAsync(
            TaskSeqCode<'T>(fun sm -> 
                sm.Data.PushDispose (fun () -> compensation())
                body.Invoke(&sm)), 
            ResumableCode<_,_>(fun sm -> sm.Data.PopDispose(); true))

    member inline _.TryFinally(body: TaskSeqCode<'T>, compensation : unit -> unit) : TaskSeqCode<'T> =
        ResumableCode.TryFinally(
            TaskSeqCode<'T>(fun sm -> 
                sm.Data.PushDispose (fun () -> Task.FromResult(compensation()))
                body.Invoke(&sm)), 
            NonResumableCode<_,_>(fun sm -> sm.Data.PopDispose()))

    member inline this.Using(disp : #IDisposable, body : #IDisposable -> TaskSeqCode<'T>) : TaskSeqCode<'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> (body disp).Invoke(&sm)),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.UsingAsync(disp : #IAsyncDisposable, body : #IAsyncDisposable -> TaskSeqCode<'T>) : TaskSeqCode<'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinallyAsync(
            (fun sm -> (body disp).Invoke(&sm)),
            (fun () -> 
                if not (isNull (box disp)) then 
                    // TODO should be async
                    (disp.DisposeAsync().AsTask().Wait(); Task.FromResult())
                else Task.FromResult()))

    member inline this.For(sequence : seq<'TElement>, body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke(&sm)))))

    member inline this.For(source: #IAsyncEnumerable<'TElement>, body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            this.UsingAsync(source.GetAsyncEnumerator(sm.Data.cancellationToken), 
                (fun e -> this.WhileAsync((fun () -> e.MoveNextAsync()), 
                                          (fun sm -> (body e.Current).Invoke(&sm))))).Invoke(&sm))

    member inline _.Yield (v: 'T) : TaskSeqCode<'T>  =
        TaskSeqCode<'T>(fun sm -> 
            // This will yield with __stack_fin = false
            // This will resume with __stack_fin = true
            let __stack_fin = ResumableCode.Yield().Invoke(&sm)
            sm.Data.current <- ValueSome v
            __stack_fin)

    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    member inline _.Bind (task: Task<'TResult1>, continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        TaskSeqCode<'T>(fun sm -> 
            let mutable awaiter = task.GetAwaiter()
            let mutable __stack_fin = true
            if not awaiter.IsCompleted then 
                // This will yield with __stack_fin2 = false
                // This will resume with __stack_fin2 = true
                let __stack_fin2 = ResumableCode.Yield().Invoke(&sm)
                __stack_fin <- __stack_fin2

            if __stack_fin then 
                let result = awaiter.GetResult()
                (continuation result).Invoke(&sm)
            else
                if verbose then printfn "calling AwaitUnsafeOnCompleted"
                sm.Data.builder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                false)

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

    //let perf1_AsyncSeq (x: int) = 
    //    FSharp.Control.AsyncSeqExtensions.asyncSeq {
    //       yield 1
    //       yield 2
    //       if x >= 2 then 
    //           yield 3
    //           yield 4
    //    }

    //let perf2_AsyncSeq () = 
    //    FSharp.Control.AsyncSeqExtensions.asyncSeq {
    //       for i1 in perf1_AsyncSeq 3 do
    //         for i2 in perf1_AsyncSeq 3 do
    //           for i3 in perf1_AsyncSeq 3 do
    //             for i4 in perf1_AsyncSeq 3 do
    //               for i5 in perf1_AsyncSeq 3 do
    //                 yield! perf1_AsyncSeq i5
    //    }

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

