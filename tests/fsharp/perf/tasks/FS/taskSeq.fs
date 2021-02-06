
module Tests.TaskSeqBuilder

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

[<AbstractClass>]
type TaskSeqStateMachine<'T>() =
    let mutable tcs = Unchecked.defaultof<TaskCompletionSource<bool>>
    let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
    let disposalStack = ResizeArray<(unit -> Task<unit>)>()

    member val Current : ValueOption<'T> = Unchecked.defaultof<_> with get, set
    member val ResumptionPoint : int = 0 with get, set
    member val ResumptionFunc : (TaskSeqStateMachine<'T> -> TaskSeqStatus) = Unchecked.defaultof<_> with get, set
    member val Awaiter : ICriticalNotifyCompletion = Unchecked.defaultof<_> with get, set
    member _.CancellationToken with get() = cancellationToken and set v = cancellationToken <- v

    /// Proceed to the next state or raise an exception
    abstract Step : unit -> TaskSeqStatus

    interface System.Collections.Generic.IAsyncEnumerable<'T> with
        member this.GetAsyncEnumerator(ct) = 
            let clone = this.MemberwiseClone() :?> TaskSeqStateMachine<'T>
            clone.CancellationToken <- ct
            (clone :> System.Collections.Generic.IAsyncEnumerator<'T>)

    interface System.Collections.Generic.IAsyncEnumerator<'T> with
        
        member sm.Current = match sm.Current with ValueSome x -> x | ValueNone -> failwith "no current value"
        
        member __.DisposeAsync() = 
            task {
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
        
        member this.MoveNextAsync() = 
            tcs <- new TaskCompletionSource<bool>()
            this.MoveNextAsync()
            ValueTask<bool>(tcs.Task)

    member __.PushDispose (f: unit -> Task<unit>) = disposalStack.Add(f)

    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member sm.MoveNextAsync() : unit =
        try
            if verbose then if verbose then Console.WriteLine("[{0}] step from L{1}", sm.GetHashCode(), sm.ResumptionPoint)
            sm.Current <- ValueNone
            let code = sm.Step ()
            if verbose then Console.WriteLine("[{0}] step done {1}", sm.GetHashCode(), code)
            match code with 
            | TaskSeqStatus.AWAIT -> ()
            | TaskSeqStatus.YIELD -> tcs.SetResult true
            | _ (* TaskSeqStatus.DONE *) -> tcs.SetResult false
        with exn ->
            if verbose then Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
            tcs.SetException exn

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IAsyncEnumerable<'T>)

type TaskSeqCode<'T> = TaskSeqStateMachine<'T> -> TaskSeqStatus    

type TaskSeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        if __useResumableStateMachines then
            (__resumableStateMachine
                { new TaskSeqStateMachine<'T>() with 
                    member sm.Step () = 
                        __resumeAt sm.ResumptionPoint
                        __expand_code sm
                }
            ).Start()
        else
            let sm = 
                { new TaskSeqStateMachine<'T>() with 
                    member sm.Step () = 
                        let code = sm.ResumptionFunc sm
                        match code with 
                        | TaskSeqStatus.AWAIT -> sm.Awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
                        | _ -> ()
                        code
                }
            sm.ResumptionFunc <- __expand_code
            sm.Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : TaskSeqCode<'T> =
        (fun _sm -> TaskSeqStatus.DONE)

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_task1: TaskSeqCode<'T>, __expand_task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
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
    member inline __.While(__expand_condition : unit -> bool, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let mutable step = TaskSeqStatus.DONE
                while (step = TaskSeqStatus.DONE) && __expand_condition() do
                    if verbose then Console.WriteLine("[{0}] while loop before body", sm.GetHashCode())
                    let __stack_step = __expand_body sm
                    step <- __stack_step
                    if verbose then Console.WriteLine("[{0}] while loop after body, step = {1}", sm.GetHashCode(), step)
                if verbose then Console.WriteLine("[{0}] finishing while with {1}", sm.GetHashCode(), step)
                step
            else
                failwith "reflective execution of TaskSeq While loop NYI")

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : TaskSeqCode<'T>, __expand_catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                let mutable step = TaskSeqStatus.DONE
                let mutable caught = false
                let mutable savedExn = Unchecked.defaultof<_>
                try
                    let __stack_step = __expand_body sm
                    step <- __stack_step
                with exn -> 
                    caught <- true
                    savedExn <- exn

                if caught then 
                    __expand_catch savedExn sm
                else
                    step
            else
                failwith "reflective execution of TaskSeq TryWith NYI")

    [<NoDynamicInvocation>]
    member inline __.TryFinallyAsync(__expand_body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then
                if verbose then Console.WriteLine("[{0}] enter try/finally", sm.GetHashCode())
                let mutable step = TaskSeqStatus.DONE
                sm.PushDispose compensation
                try
                    if verbose then Console.WriteLine("[{0}] try/finally before body", sm.GetHashCode())
                    let __stack_step = __expand_body sm
                    if verbose then Console.WriteLine("[{0}] try/finally after body", sm.GetHashCode())
                    step <- __stack_step
                with _ ->
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                    reraise()

                if step = TaskSeqStatus.DONE then 
                    sm.PopDispose()
                    compensation().Result // TODO: async execution of this
                if verbose then Console.WriteLine("[{0}] finishing try/finally with {1}", sm.GetHashCode(), step)
                step
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
    member inline this.For(source : IAsyncEnumerable<'TElement>, __expand_body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            this.UsingAsync(source.GetAsyncEnumerator(sm.CancellationToken), 
                // TODO: This should call WhileAsync
                (fun e -> this.While((fun () -> e.MoveNextAsync().Result), (fun sm -> __expand_body e.Current sm)))) sm)

    [<NoDynamicInvocation>]
    member inline __.Yield (v: 'T) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then 
                match __resumableEntry() with
                | Some contID ->
                    if verbose then Console.WriteLine("[{0}] suspending at yield of {1}", sm.GetHashCode(), v)
                    sm.ResumptionPoint <- contID
                    sm.Current <- ValueSome v
                    TaskSeqStatus.YIELD
                | None -> 
                    if verbose then Console.WriteLine("[{0}] resuming after yield of {1}", sm.GetHashCode(), v)
                    TaskSeqStatus.DONE
            else
                sm.ResumptionFunc <- (fun sm -> TaskSeqStatus.DONE)
                sm.Current <- ValueSome v
                TaskSeqStatus.YIELD)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    [<NoDynamicInvocation>]
    member inline __.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableStateMachines then 
                let mutable awaiter = task.GetAwaiter()
                match __resumableEntry() with 
                | Some contID ->
                    if awaiter.IsCompleted then 
                        __resumeAt contID
                    else
                        sm.ResumptionPoint <- contID
                        // Tell the builder to call us again when done.
                        awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
                        // We have suspended
                        TaskSeqStatus.AWAIT
                | None ->
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
              while e.MoveNextAsync().Result do 
                  yield e.Current
          finally
              e.DisposeAsync().AsTask().Wait() ]

    let toArray (t: taskSeq<'T>) =
        [| let e = t.GetAsyncEnumerator(CancellationToken())
           try 
               while e.MoveNextAsync().Result do 
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
            while e.MoveNextAsync().Result do 
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
           for i in perf1 3 do
             for i in perf1 3 do
               for i in perf1 3 do
                 for i in perf1 3 do
                   for i in perf1 3 do
                   yield! perf1 i
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
           for i in perf1_AsyncSeq 3 do
             for i in perf1_AsyncSeq 3 do
               for i in perf1_AsyncSeq 3 do
                 for i in perf1_AsyncSeq 3 do
                   for i in perf1_AsyncSeq 3 do
                   yield! perf1_AsyncSeq i
        }

    let dumpTaskSeq (t: IAsyncEnumerable<_>) = 
        printfn "-----"
        let e = t.GetAsyncEnumerator(CancellationToken())
        while e.MoveNextAsync().Result do 
            printfn "yield %A" e.Current

    dumpTaskSeq (t1())
    dumpTaskSeq (t2())

    printfn "t1() = %A" (TaskSeq.toArray (t1()))
    printfn "t2() = %A" (TaskSeq.toArray (t2()))

    printfn "perf2() = %A" (TaskSeq.toArray (perf2()))

