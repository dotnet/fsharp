
module Tests.TaskSeqBuilder

#nowarn "42"
open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

type IAsyncDisposable =
    abstract DisposeAsync: unit -> Task<unit>

type IAsyncEnumerator<'T> =
    inherit IAsyncDisposable
    abstract Current: 'T
    abstract MoveNextAsync: unit -> Task<bool>

type IAsyncEnumerable<'T> =
    abstract GetAsyncEnumerator: ct: CancellationToken -> IAsyncEnumerator<'T>

[<AbstractClass>]
type TaskSeqStateMachine<'T>() =
    let mutable tcs = Unchecked.defaultof<TaskCompletionSource<bool>>
    let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
    let disposalStack = ResizeArray<(unit -> Task<unit>)>()

    member val Current : ValueOption<'T> = Unchecked.defaultof<_> with get, set
    member val ResumptionPoint : int = 0 with get, set
    member val ResumptionFunc : (TaskSeqStateMachine<'T> -> bool) = Unchecked.defaultof<_> with get, set
    member val Awaiter : ICriticalNotifyCompletion = Unchecked.defaultof<_> with get, set

    /// Proceed to the next state or raise an exception
    abstract Step : unit -> bool

    interface IAsyncEnumerable<'T> with
        member this.GetAsyncEnumerator(ct) = 
            cancellationToken <- ct
            // TODO: make new object if needed
            (this :> IAsyncEnumerator<'T>)

    interface IAsyncEnumerator<'T> with
        
        member sm.Current = match sm.Current with ValueSome x -> x | ValueNone -> failwith "no current value"
        
        // TODO: no early disposal yet - disposal only by driving sequence to the end
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
        
        member this.MoveNextAsync() = 
            tcs <- new TaskCompletionSource<bool>()
            this.MoveNextAsync()
            tcs.Task

    member __.PushDispose (f: unit -> Task<unit>) = disposalStack.Add(f)
    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member __.CancellationToken = cancellationToken

    member sm.MoveNextAsync() : unit =
        try
            Console.WriteLine("[{0}] step from {1}", sm.GetHashCode(), sm.ResumptionPoint)
            sm.Current <- ValueNone
            let completed = sm.Step ()
            if completed then 
                tcs.SetResult sm.Current.IsSome
        with exn ->
            Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
            tcs.SetException exn

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IAsyncEnumerable<'T>)

type TaskSeqCode<'T> = TaskSeqStateMachine<'T> -> bool    

type TaskSeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        if __useResumableCode then
            (__resumableObject
                { new TaskSeqStateMachine<'T>() with 
                    member sm.Step () = 
                        __resumeAt sm.ResumptionPoint
                        __expand_code sm }).Start()
        else
            let sm = 
                { new TaskSeqStateMachine<'T>() with 
                    member sm.Step () = 
                        let completed = sm.ResumptionFunc sm
                        if not completed then 
                            sm.Awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
                        completed }
            sm.ResumptionFunc <- __expand_code
            sm.Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : TaskSeqCode<'T> =
        (fun _sm -> true)

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_task1: TaskSeqCode<'T>, __expand_task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableCode then
                let ``__machine_step$cont`` = __expand_task1 sm
                if ``__machine_step$cont`` then
                    // If state machines are supported, then the resumption jumps directly into the code for __expand_task1
                    // at the point where computation was suspended.  This code includes the subsequent invoke of
                    __expand_task2 sm
                else
                    false
            else

                let completed = __expand_task1 sm
                if completed then
                    __expand_task2 sm
                else 
                    // If state machines are not supported, then we must adjust the resumption to also run __expand_task2 on completion
                    let rec resume rf =
                        (fun (sm :TaskSeqStateMachine<_>) -> 
                            let completed = rf sm
                            if completed then 
                                __expand_task2 sm
                            else
                                sm.ResumptionFunc <- resume sm.ResumptionFunc
                                false)

                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                    false)
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable step = true
            while step && __expand_condition() do
                let ``__machine_step$cont`` = __expand_body sm
                match ``__machine_step$cont`` with 
                | true  -> ()
                | v -> step <- v
            step)

    // Todo: async condition in while loop
    //member inline __.WhileAsync(__expand_condition : unit -> Task<bool>, __expand_body : unit -> bool<'T>) : bool<'T> =
    //    let mutable step = bool<'T>(DONE) 
    //    while step.IsDone && __expand_condition() do
    //        let ``__machine_step$cont`` = __expand_body ()
    //        step <- ``__machine_step$cont``
    //    step

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : TaskSeqCode<'T>, __expand_catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable step = true
            let mutable caught = false
            let mutable savedExn = Unchecked.defaultof<_>
            try
                let ``__machine_step$cont`` = __expand_body sm
                step <- ``__machine_step$cont``
                // TODO: If we are not doing state machine compilation, then if "``__machine_step$cont``" is false we must adjust the 
                // ResumptionFunc to re-enter the try/with
            with exn -> 
                caught <- true
                savedExn <- exn

            if caught then 
                __expand_catch savedExn sm
            else
                step)

    [<NoDynamicInvocation>]
    member inline __.TryFinallyAsync(__expand_body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable step = true
            sm.PushDispose compensation
            try
                let ``__machine_step$cont`` = __expand_body sm
                // TODO: If we are not doing state machine compilation, then if "``__machine_step$cont``" is false we must adjust the 
                // ResumptionFunc to re-enter the try/with
                // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                // may skip this step.
                step <- ``__machine_step$cont``
            with _ ->
                sm.PopDispose()
                compensation().Result // TODO: async execution of this
                reraise()

            if step then 
                sm.PopDispose()
                compensation().Result // TODO: async execution of this
            step)

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
            (fun () -> if not (isNull (box disp)) then disp.DisposeAsync() else Task.FromResult()))

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
            if __useResumableCode then 
                match __resumableEntry() with
                | Some contID ->
                    sm.ResumptionPoint <- contID
                    sm.Current <- ValueSome v
                    true
                | None -> 
                    true
            else
                sm.ResumptionFunc <- (fun sm -> true)
                sm.Current <- ValueSome v
                true)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    [<NoDynamicInvocation>]
    member inline __.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        (fun sm -> 
            if __useResumableCode then 
                let mutable awaiter = task.GetAwaiter()
                match __resumableEntry() with 
                | Some contID ->
                    if awaiter.IsCompleted then 
                        __resumeAt contID
                    else
                        sm.ResumptionPoint <- contID
                        // Tell the builder to call us again when done.
                        awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
                        false
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
                    false)

let taskSeq = TaskSeqBuilder()

module Examples =

    let t1 () = 
        taskSeq {
           printfn "in t1"
           yield "a"
           let x = 1
           let! v = task { printfn "hey"
                           do! Task.Delay(100) }
           yield "b"
           let! v = task { printfn "hey yo"
                           do! Task.Delay(100) }
           yield "c"
        }

    let t2 () = 
        taskSeq {
           printfn "in t2"
           yield "d"
           for x in t1 () do 
               printfn "t2 - got %A" x
               yield "e"
               let! v = task { printfn "hey yo"
                               do! Task.Delay(100) }
               yield "[T1]" + x
           let! v = task { printfn "hey yo"
                           do! Task.Delay(100) }
           yield "f"
        }

    let dumpTaskSeq (t: IAsyncEnumerable<_>) = 
        let e = t.GetAsyncEnumerator(CancellationToken())
        while e.MoveNextAsync().Result do 
            printfn "yield %A" e.Current
    dumpTaskSeq (t1())
    dumpTaskSeq (t2())
