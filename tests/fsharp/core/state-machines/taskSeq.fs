
module Tests.TaskSeqBuilder

#nowarn "42"
open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct>]
type TaskSeqStep = 
    | AWAIT = 1uy
    | YIELD = 2uy
    | DONE = 3uy

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

    member val Current : 'T = Unchecked.defaultof<'T> with get, set
    member val ResumptionPoint : int = 0 with get, set
    member val Resumption : (TaskSeqStateMachine<'T> -> TaskSeqStep) = Unchecked.defaultof<_> with get, set

    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> TaskSeqStep

    interface IAsyncEnumerable<'T> with
        member this.GetAsyncEnumerator(ct) = 
            cancellationToken <- ct
            // TODO: make new object if needed
            (this :> IAsyncEnumerator<'T>)

    interface IAsyncEnumerator<'T> with
        
        member sm.Current = sm.Current
        
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
            let step = sm.Step sm.ResumptionPoint
            match step with 
            | TaskSeqStep.DONE  -> tcs.SetResult false
            | TaskSeqStep.YIELD -> tcs.SetResult true
            | TaskSeqStep.AWAIT -> ()
            | _ -> failwith "unreachable"
        with exn ->
            Console.WriteLine("[{0}] exception {1}", sm.GetHashCode(), exn)
            tcs.SetException exn

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IAsyncEnumerable<'T>)

type TaskSeqCode<'T> = TaskSeqStateMachine<'T> -> TaskSeqStep    

type TaskSeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : TaskSeqCode<'T>) : IAsyncEnumerable<'T> = 
        (__stateMachine
            { new TaskSeqStateMachine<'T>() with 
                member sm.Step pc = __jumptable pc (__expand_code sm) }).Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : TaskSeqCode<'T> =
        (fun _sm -> TaskSeqStep.DONE)

    [<NoDynamicInvocation>]
    member inline __.Combine(step1: TaskSeqCode<'T>, __expand_task2: TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            let ``__machine_step$cont`` = step1 sm
            match ``__machine_step$cont`` with 
            | TaskSeqStep.DONE  -> __expand_task2 sm
            | v -> v)
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable step = TaskSeqStep.DONE
            while (step = TaskSeqStep.DONE) && __expand_condition() do
                let ``__machine_step$cont`` = __expand_body sm
                match ``__machine_step$cont`` with 
                | TaskSeqStep.DONE  -> ()
                | v -> step <- v
            step)

    // Todo: async condition in while loop
    //member inline __.WhileAsync(__expand_condition : unit -> Task<bool>, __expand_body : unit -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
    //    let mutable step = TaskSeqStep<'T>(DONE) 
    //    while step.IsDone && __expand_condition() do
    //        let ``__machine_step$cont`` = __expand_body ()
    //        step <- ``__machine_step$cont``
    //    step

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : TaskSeqCode<'T>, __expand_catch : exn -> TaskSeqCode<'T>) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable step = TaskSeqStep.DONE
            let mutable caught = false
            let mutable savedExn = Unchecked.defaultof<_>
            try
                let ``__machine_step$cont`` = __expand_body sm
                step <- ``__machine_step$cont``
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
            let mutable step = TaskSeqStep.DONE
            sm.PushDispose compensation
            try
                let ``__machine_step$cont`` = __expand_body sm
                // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                // may skip this step.
                step <- ``__machine_step$cont``
            with _ ->
                sm.PopDispose()
                compensation().Result // TODO: async execution of this
                reraise()

            if (step = TaskSeqStep.DONE) then 
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
            this.UsingAsync (source.GetAsyncEnumerator(sm.CancellationToken), 
                // TODO: This should call WhileAsync
                (fun e -> this.While((fun () -> e.MoveNextAsync().Result), (fun sm -> __expand_body e.Current sm)))) sm)

    [<NoDynamicInvocation>]
    member inline __.Yield (v: 'T) : TaskSeqCode<'T> =
        (fun sm -> 
            let CONT = __entryPoint (fun sm -> TaskSeqStep.DONE)
            if __stateMachinesSupported then 
               sm.ResumptionPoint <- __entryPointStaticId CONT
            else
               sm.Resumption <- CONT
            sm.Current <- v
            TaskSeqStep.YIELD)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqCode<'T> =
        this.For(source, (fun v -> this.Yield(v)))

    [<NoDynamicInvocation>]
    member inline __.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqCode<'T>)) : TaskSeqCode<'T> =
        (fun sm -> 
            let mutable awaiter = task.GetAwaiter()
            let CONT = __entryPoint (fun sm -> __expand_continuation (awaiter.GetResult()) sm)
            if awaiter.IsCompleted then 
                CONT sm
            else
                if __stateMachinesSupported then 
                   sm.ResumptionPoint <- __entryPointStaticId CONT
                else
                   sm.Resumption <- CONT
                // Tell the builder to call us again when done.
                Console.WriteLine("[{0}] UnsafeOnCompleted", sm.GetHashCode())
                awaiter.UnsafeOnCompleted(Action(fun () -> sm.MoveNextAsync()))
                TaskSeqStep.AWAIT)

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
