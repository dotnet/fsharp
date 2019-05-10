
module TaskSeq

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FSharp.Core.CompilerServices.CodeGenHelpers

let [<Literal>] AWAIT = 1
let [<Literal>] YIELD = 2
let [<Literal>] DONE = 3

[<Struct>]
type TaskSeqStep<'T>(res: int) = 
    member x.IsYield = (res = 2)
    member x.IsDone = (res = 3)

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
    let mutable current : 'T = Unchecked.defaultof<'T> 
    let mutable resumptionPoint = 0 
    let mutable tcs = Unchecked.defaultof<TaskCompletionSource<bool>>
    let mutable cancellationToken = Unchecked.defaultof<CancellationToken>
    let disposalStack = ResizeArray<(unit -> Task<unit>)>()
    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> TaskSeqStep<'T>

    interface IAsyncEnumerable<'T> with
        member this.GetAsyncEnumerator(ct) = 
            cancellationToken <- ct
            // TODO: make new object if needed
            (this :> IAsyncEnumerator<'T>)

    interface IAsyncEnumerator<'T> with
        
        member __.Current = current
        
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
            this.MoveNextAsync(tcs)
            tcs.Task

    member __.PushDispose (f: unit -> Task<unit>) = disposalStack.Add(f)
    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member __.CancellationToken = cancellationToken

    member __.Yield (v: 'T, pc: int) = 
        resumptionPoint <- pc
        current <- v
        TaskSeqStep<'T>(YIELD)

    member this.Await (awaiter: ICriticalNotifyCompletion, pc: int) = 
        resumptionPoint <- pc
        assert (not (isNull awaiter))
        // Tell the builder to call us again when done.
        Console.WriteLine("[{0}] UnsafeOnCompleted", this.GetHashCode())
        awaiter.UnsafeOnCompleted(Action(fun () -> this.MoveNextAsync(tcs)))

    member this.MoveNextAsync(tcs: TaskCompletionSource<bool>) : unit =
        try
            Console.WriteLine("[{0}] step from {1}", this.GetHashCode(), resumptionPoint)
            let step = this.Step resumptionPoint
            if step.IsDone then 
                tcs.SetResult false
            elif step.IsYield then 
                tcs.SetResult true
        with exn ->
            Console.WriteLine("[{0}] exception {1}", this.GetHashCode(), exn)
            tcs.SetException exn

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IAsyncEnumerable<'T>)
    
type TaskSeqBuilder() =
    
    member inline __.Delay(__expand_f : unit -> TaskSeqStep<'T>) = __expand_f

    member inline __.Run(__expand_code : unit -> TaskSeqStep<'T>) : IAsyncEnumerable<'T> = 
        (__stateMachine
            { new TaskSeqStateMachine<'T>() with 
                member __.Step pc = __jumptable pc __expand_code }).Start()

    member inline __.Zero() : TaskSeqStep<'T> =
        TaskSeqStep<'T>(DONE)

    member inline __.Combine(``__machine_step$cont``: TaskSeqStep<'T>, __expand_task2: unit -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
        if ``__machine_step$cont``.IsDone then 
            __expand_task2()
        else
            ``__machine_step$cont``
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
        let mutable step = TaskSeqStep<'T>(DONE) 
        while step.IsDone && __expand_condition() do
            let ``__machine_step$cont`` = __expand_body ()
            step <- ``__machine_step$cont``
        step

    // Todo: async condition in while loop
    //member inline __.WhileAsync(__expand_condition : unit -> Task<bool>, __expand_body : unit -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
    //    let mutable step = TaskSeqStep<'T>(DONE) 
    //    while step.IsDone && __expand_condition() do
    //        let ``__machine_step$cont`` = __expand_body ()
    //        step <- ``__machine_step$cont``
    //    step

    member inline __.TryWith(__expand_body : unit -> TaskSeqStep<'T>, __expand_catch : exn -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
        let mutable step = TaskSeqStep<'T>(DONE)
        let mutable caught = false
        let mutable savedExn = Unchecked.defaultof<_>
        try
            let ``__machine_step$cont`` = __expand_body ()
            step <- ``__machine_step$cont``
        with exn -> 
            caught <- true
            savedExn <- exn

        if caught then 
            __expand_catch savedExn
        else
            step

    member inline __.TryFinallyAsync(__expand_body: unit -> TaskSeqStep<'T>, compensation : unit -> Task<unit>) : TaskSeqStep<'T> =
        let mutable step = TaskSeqStep<'T>(DONE)
        __machine<TaskSeqStateMachine<'T>>.PushDispose compensation
        try
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            step <- ``__machine_step$cont``
        with _ ->
            __machine<TaskSeqStateMachine<'T>>.PopDispose()
            compensation().Result // TODO: async execution of this
            reraise()

        if step.IsDone then 
            __machine<TaskSeqStateMachine<'T>>.PopDispose()
            compensation().Result // TODO: async execution of this
        step

    member inline this.TryFinally(__expand_body: unit -> TaskSeqStep<'T>, compensation : unit -> unit) : TaskSeqStep<'T> =
        this.TryFinallyAsync(__expand_body, fun () -> Task.FromResult(compensation()))

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> TaskSeqStep<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.UsingAsync(disp : #IAsyncDisposable, __expand_body : #IAsyncDisposable -> TaskSeqStep<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinallyAsync(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.DisposeAsync() else Task.FromResult()))

    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    member inline this.For(source : IAsyncEnumerable<'TElement>, __expand_body : 'TElement -> TaskSeqStep<'T>) : TaskSeqStep<'T> =
        let mutable ct = Unchecked.defaultof<_>
        ct <- __machine<TaskSeqStateMachine<'T>>.CancellationToken
        this.UsingAsync (source.GetAsyncEnumerator(ct), 
            // TODO: This should call WhileAsync
            (fun e -> this.While((fun () -> e.MoveNextAsync().Result), (fun () -> __expand_body e.Current))))

    member inline __.Yield (``__machine_step$cont``: 'T) : TaskSeqStep<'T> =
        let CONT = __newEntryPoint()
        // A dummy to allow us to lay down the code for the continuation
        let mutable afterYield = (# "nop nop" false : bool #) // stop optimization 
        if afterYield then 
            __entryPoint CONT
            printfn "after yield"
            TaskSeqStep<'T>(DONE)
        else
            __machine<TaskSeqStateMachine<'T>>.Yield(``__machine_step$cont``, CONT)

    member inline this.YieldFrom (source: IAsyncEnumerable<'T>) : TaskSeqStep<'T> =
        this.For(source, (fun ``__machine_step$cont`` -> this.Yield(``__machine_step$cont``)))

    member inline __.Bind (task: Task<'TResult1>, __expand_continuation: ('TResult1 -> TaskSeqStep<'T>)) : TaskSeqStep<'T> =
        let CONT = __newEntryPoint()
        let awaiter = task.GetAwaiter()
        if awaiter.IsCompleted then 
            __entryPoint CONT
            __expand_continuation (awaiter.GetResult())
        else
            __machine<TaskSeqStateMachine<'T>>.Await (awaiter, CONT)
            TaskSeqStep<'T>(AWAIT)

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
