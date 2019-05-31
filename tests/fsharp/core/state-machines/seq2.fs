
module Tests.Seq2

#nowarn "42"
open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AbstractClass>]
type SeqStateMachine<'T>() =
    let disposalStack = ResizeArray<(unit -> unit)>()

    /// Proceed to the next state or raise an exception
    abstract Step : pc: int -> bool

    member val Current : 'T = Unchecked.defaultof<'T> with get, set

    member val ResumptionPoint : int = 0 with get, set

    member val Resumption : (SeqStateMachine<'T> -> bool) = Unchecked.defaultof<_> with get, set

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
        member sm.Current = box sm.Current
        
        member this.MoveNext() = 
            this.MoveNextImpl()
        
    interface IEnumerator<'T> with
        member sm.Current = sm.Current

    member __.PushDispose (f: unit -> unit) = disposalStack.Add(f)
    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    member sm.MoveNextImpl() : bool =
        Console.WriteLine("[{0}] step from {1}", sm.GetHashCode(), sm.ResumptionPoint)
        sm.Step sm.ResumptionPoint

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.Start() = (sm :> IEnumerable<'T>)

type SeqCode<'T> = SeqStateMachine<'T> -> bool

type SeqBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> SeqCode<'T>) : SeqCode<'T> = (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : SeqCode<'T>) : IEnumerable<'T> = 
        (__stateMachine
            { new SeqStateMachine<'T>() with 
                member sm.Step pc = __jumptable pc (__expand_code sm) }).Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : SeqCode<'T> =
        (fun _sm -> false)

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_task1: SeqCode<'T>, __expand_task2: SeqCode<'T>) : SeqCode<'T> =
        (fun sm -> 
            let ``__machine_step$cont`` = __expand_task1 sm
            if ``__machine_step$cont`` then 
                true
            else
                __expand_task2 sm)
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : SeqCode<'T>) : SeqCode<'T> =
        (fun sm -> 
            let mutable step = false 
            while step && __expand_condition() do
                let ``__machine_step$cont`` = __expand_body sm
                step <- ``__machine_step$cont``
            step)

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : SeqCode<'T>, __expand_catch : exn -> SeqCode<'T>) : SeqCode<'T> =
        (fun sm -> 
            let mutable step = false
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
    member inline __.TryFinally(__expand_body: SeqCode<'T>, compensation : unit -> unit) : SeqCode<'T> =
        (fun sm -> 
            let mutable step = false
            sm.PushDispose compensation
            try
                let ``__machine_step$cont`` = __expand_body sm
                // If we make it to the assignment we prove we've made a step without an exception
                step <- ``__machine_step$cont``
            with _ ->
                sm.PopDispose()
                compensation()
                reraise()

            if step then 
                sm.PopDispose()
                compensation()
            step)

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> SeqCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> __expand_body disp sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> SeqCode<'T>) : SeqCode<'T> =
        // A for loop is just a using statement on the sequence's enumerator...
        this.Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> __expand_body e.Current sm))))

    [<NoDynamicInvocation>]
    member inline __.Yield (v: 'T) : SeqCode<'T> =
        (fun sm ->
            let CONT = __entryPoint (fun sm -> false)
            if __stateMachinesSupported then 
                sm.ResumptionPoint <- __entryPointStaticId CONT
            else 
                sm.Resumption <- CONT
            sm.Current <- v
            true)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IEnumerable<'T>) : SeqCode<'T> =
        this.For(source, (fun v -> this.Yield v))

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
