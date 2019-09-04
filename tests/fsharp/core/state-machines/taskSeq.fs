
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
    member val ResumptionFunc : MachineFunc<TaskSeqStateMachine<'T>> = Unchecked.defaultof<_> with get, set

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
            let completed = sm.Step ()
            if completed then 
                tcs.SetResult sm.Current.IsSome
        with exn ->
            Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
            tcs.SetException exn

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IAsyncEnumerable<'T>)

type TaskSeqCode<'T> = delegate of byref<TaskSeqStateMachine<'T>> -> bool    

type TaskSeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> (__expand_f ()).Invoke &sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
#if ENABLED
        (__stateMachine
            { new TaskSeqStateMachine<'T>() with 
                member sm.Step () = __jumptableSMH sm.ResumptionPoint (__expand_code sm) }).Start()
#else
            let sm = 
                { new TaskSeqStateMachine<'T>() with 
                    member sm.Step () = 
                        let mutable sm = sm
                        sm.ResumptionFunc.Invoke &sm }
            sm.ResumptionFunc <- MachineFunc<_>(fun sm -> __expand_code.Invoke &sm)
            sm.Start()
#endif

    [<NoDynamicInvocation>]
    member inline __.Zero() : TaskSeqCode<'T> =
        TaskSeqCode(fun _sm -> true)

    [<NoDynamicInvocation>]
    member inline __.Combine(step1: TaskSeqCode<'T>, __expand_task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            let ``__machine_step$cont`` = step1.Invoke &sm
            if ``__machine_step$cont`` then
                __expand_task2.Invoke &sm
            else 
                if __stateMachinesSupported then
                    // If state machines are supported, then the resumption jumps directly into the code for __expand_task1
                    // at the point where computation was suspended.  This code includes the subsequent invoke of
                    // __expand_task2
                    ()
                else
                    // If state machines are not supported, then we must adjust the resumption to also run __expand_task2
                    let rec combine () =
                        MachineFunc<TaskSeqStateMachine<_>>(fun sm -> 
                            let ``__machine_step$cont`` = sm.ResumptionFunc.Invoke(&sm)
                            if ``__machine_step$cont`` then 
                                __expand_task2.Invoke(&sm)
                            else
                                sm.ResumptionFunc <- combine ()
                                false)

                    sm.ResumptionFunc <- combine ()
                false)
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            let mutable step = true
            while (step = true) && __expand_condition() do
                let ``__machine_step$cont`` = __expand_body.Invoke &sm
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
        TaskSeqCode(fun sm -> 
            let mutable step = true
            let mutable caught = false
            let mutable savedExn = Unchecked.defaultof<_>
            try
                let ``__machine_step$cont`` = __expand_body.Invoke &sm
                step <- ``__machine_step$cont``
                // TODO: If we are not doing state machine compilation, then if "``__machine_step$cont``" is false we must adjust the 
                // ResumptionFunc to re-enter the try/with
            with exn -> 
                caught <- true
                savedExn <- exn

            if caught then 
                (__expand_catch savedExn).Invoke &sm
            else
                step)

    [<NoDynamicInvocation>]
    member inline __.TryFinallyAsync(__expand_body: TaskSeqCode<'T>, compensation : unit -> Task<unit>) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            let mutable step = true
            sm.PushDispose compensation
            try
                let ``__machine_step$cont`` = __expand_body.Invoke &sm
                // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                // may skip this step.
                step <- ``__machine_step$cont``
            with _ ->
                sm.PopDispose()
                compensation().Result // TODO: async execution of this
                reraise()

            if (step = true) then 
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
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.UsingAsync(disp : #IAsyncDisposable, __expand_body : #IAsyncDisposable -> TaskSeqCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinallyAsync(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.DisposeAsync() else Task.FromResult()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    [<NoDynamicInvocation>]
    member inline this.For(source : IAsyncEnumerable<'TElement>, __expand_body : 'TElement -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            this.UsingAsync(source.GetAsyncEnumerator(sm.CancellationToken), 
                // TODO: This should call WhileAsync
                (fun e -> this.While((fun () -> e.MoveNextAsync().Result), (fun sm -> (__expand_body e.Current).Invoke &sm)))).Invoke(&sm))

    [<NoDynamicInvocation>]
    member inline __.Yield (v: 'T) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            let CONT = __entryPoint (MachineFunc<_>(fun sm -> true))
#if ENABLED
            if __stateMachinesSupported then 
                sm.ResumptionPoint <- __entryPointIdSMH CONT
            else 
                sm.ResumptionFunc <- CONT
#else
            sm.ResumptionFunc <- CONT
#endif
            sm.Current <- ValueSome v
            true)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    [<NoDynamicInvocation>]
    member inline __.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        TaskSeqCode(fun sm -> 
            let mutable awaiter = task.GetAwaiter()
            let CONT = __entryPoint (MachineFunc<_>(fun sm -> (__expand_continuation (awaiter.GetResult())).Invoke &sm))
            if awaiter.IsCompleted then 
                CONT.Invoke &sm
            else
#if ENABLED
                if __stateMachinesSupported then 
                    sm.ResumptionPoint <- __entryPointIdSMH CONT
                else 
                    sm.ResumptionFunc <- CONT
#else
                sm.ResumptionFunc <- CONT
#endif
                // Tell the builder to call us again when done.
                Console.WriteLine("[{0}] UnsafeOnCompleted", sm.GetHashCode())
                let sm = sm
                awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
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
