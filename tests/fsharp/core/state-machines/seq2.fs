
module Seq

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices.CodeGenHelpers

let [<Literal>] YIELD = 2
let [<Literal>] DONE = 3

[<Struct>]
type SeqStep<'T>(res: int) = 
    member x.IsYield = (res = 2)
    member x.IsDone = (res = 3)

[<AbstractClass>]
type SeqStateMachine<'T>() =
    let mutable current : 'T = Unchecked.defaultof<'T> 
    let mutable resumptionPoint = 0 
    let disposalStack = ResizeArray<(unit -> unit)>()

    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> SeqStep<'T>

    interface IEnumerable with
        member this.GetEnumerator() = 
            // TODO: make new object if needed
            (this :> IEnumerator)

    interface IEnumerable<'T> with
        member this.GetEnumerator() = 
            // TODO: make new object if needed
            (this :> IEnumerator<'T>)

    interface IDisposable with
        member __.Dispose() = 
            let mutable exn = None
            for d in Seq.rev disposalStack do 
                try 
                    d()
                with e ->
                    exn <- Some e // keep the last exception - TODO - check this
            match exn with 
            | None -> () 
            | Some e -> raise e 

    interface IEnumerator with
        
        member __.Reset() = failwith "no reset supported"
        member __.Current = box current
        
        member this.MoveNext() = 
            this.MoveNextImpl()
        
    interface IEnumerator<'T> with
        member __.Current = current

    member __.PushDispose (f: unit -> unit) = disposalStack.Add(f)
    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member __.Yield (v: 'T, pc: int) = 
        resumptionPoint <- pc
        current <- v
        SeqStep<'T>(YIELD)

    member this.MoveNextImpl() : bool =
        Console.WriteLine("[{0}] step from {1}", this.GetHashCode(), resumptionPoint)
        let step = this.Step resumptionPoint
        step.IsYield

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = (this :> IEnumerable<'T>)
    
type SeqBuilder() =
    
    member inline __.Delay(__expand_f : unit -> SeqStep<'T>) = __expand_f

    member inline __.Run(__expand_code : unit -> SeqStep<'T>) : IEnumerable<'T> = 
        (__stateMachine
            { new SeqStateMachine<'T>() with 
                member __.Step pc = __jumptable pc __expand_code }).Start()

    member inline __.Zero() : SeqStep<'T> =
        SeqStep<'T>(DONE)

    member inline __.Combine(``__machine_step$cont``: SeqStep<'T>, __expand_task2: unit -> SeqStep<'T>) : SeqStep<'T> =
        if ``__machine_step$cont``.IsDone then 
            __expand_task2()
        else
            ``__machine_step$cont``
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> SeqStep<'T>) : SeqStep<'T> =
        let mutable step = SeqStep<'T>(DONE) 
        while step.IsDone && __expand_condition() do
            let ``__machine_step$cont`` = __expand_body ()
            step <- ``__machine_step$cont``
        step

    member inline __.TryWith(__expand_body : unit -> SeqStep<'T>, __expand_catch : exn -> SeqStep<'T>) : SeqStep<'T> =
        let mutable step = SeqStep<'T>(DONE)
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

    member inline __.TryFinally(__expand_body: unit -> SeqStep<'T>, compensation : unit -> unit) : SeqStep<'T> =
        let mutable step = SeqStep<'T>(DONE)
        __machine<SeqStateMachine<'T>>.PushDispose compensation
        try
            let ``__machine_step$cont`` = __expand_body ()
            // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
            // may skip this step.
            step <- ``__machine_step$cont``
        with _ ->
            __machine<SeqStateMachine<'T>>.PopDispose()
            compensation()
            reraise()

        if step.IsDone then 
            __machine<SeqStateMachine<'T>>.PopDispose()
            compensation()
        step

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> SeqStep<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> SeqStep<'T>) : SeqStep<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    member inline __.Yield (``__machine_step$cont``: 'T) : SeqStep<'T> =
        let CONT = __newEntryPoint()
        // A dummy to allow us to lay down the code for the continuation
        let mutable afterYield = (# "nop nop" false : bool #) // stop optimization 
        if afterYield then 
            __entryPoint CONT
            printfn "after yield"
            SeqStep<'T>(DONE)
        else
            __machine<SeqStateMachine<'T>>.Yield(``__machine_step$cont``, CONT)

    member inline this.YieldFrom (source: IEnumerable<'T>) : SeqStep<'T> =
        this.For(source, (fun ``__machine_step$cont`` -> this.Yield(``__machine_step$cont``)))

let seq2 = SeqBuilder()

module Examples =

    let t1 () = 
        seq2 {
           printfn "in t1"
           yield "a"
           let x = 1
           yield "b"
           yield "c"
        }

    let t2 () = 
        seq2 {
           printfn "in t2"
           yield "d"
           for x in t1 () do 
               printfn "t2 - got %A" x
               yield "e"
               yield "[T1]" + x
           yield "f"
        }

    let dumpSeq (t: IEnumerable<_>) = 
        let e = t.GetEnumerator()
        while e.MoveNext() do 
            printfn "yield %A" e.Current
    dumpSeq (t1())
    dumpSeq (t2())
