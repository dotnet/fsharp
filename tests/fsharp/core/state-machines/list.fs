
module Seq

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices.CodeGenHelpers

let [<Literal>] DONE = 3

[<Struct>]
type ListStep<'T>(res: int) = 
    member x.IsDone = (res = 3)

[<AbstractClass>]
type ListStateMachine<'T>() =
    let res = ResizeArray<'T>()

    abstract Populate : unit -> ListStep<'T>

    member __.Yield (v: 'T) = res.Add(v)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.StartAsResizeArray() = 
        this.Populate() |> ignore
        res
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.StartAsList() = 
        this.Populate() |> ignore
        Seq.toList res
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.StartAsArray() = 
        this.Populate() |> ignore
        res.ToArray()
    
type ResizeArrayBuilderBase() =
    
    member inline __.Delay(__expand_f : unit -> ListStep<'T>) = __expand_f

    member inline __.Zero() : ListStep<'T> =
        ListStep<'T>(DONE)

    member inline __.Combine(``__machine_step$cont``: ListStep<'T>, __expand_task2: unit -> ListStep<'T>) : ListStep<'T> =
        __expand_task2()
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> ListStep<'T>) : ListStep<'T> =
        while __expand_condition() do
            let ``__machine_step$cont`` = __expand_body ()
            ()
        ListStep<'T>(DONE)

    member inline __.TryWith(__expand_body : unit -> ListStep<'T>, __expand_catch : exn -> ListStep<'T>) : ListStep<'T> =
        try
            __expand_body ()
        with exn -> 
            __expand_catch exn
        ListStep<'T>(DONE)

    member inline __.TryFinally(__expand_body: unit -> ListStep<'T>, compensation : unit -> unit) : ListStep<'T> =
        try
            let ``__machine_step$cont`` = __expand_body ()
            ()
        with _ ->
            compensation()
            reraise()

        compensation()
        ListStep<'T>(DONE)

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> ListStep<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> ListStep<'T>) : ListStep<'T> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    member inline __.Yield (``__machine_step$cont``: 'T) : ListStep<'T> =
        __machine<ListStateMachine<'T>>.Yield(``__machine_step$cont``)
        ListStep<'T>(DONE)

    member inline this.YieldFrom (source: IEnumerable<'T>) : ListStep<'T> =
        this.For(source, (fun ``__machine_step$cont`` -> this.Yield(``__machine_step$cont``)))

type ResizeArrayBuilder() =     
    inherit ResizeArrayBuilderBase()
    member inline __.Run(__expand_code : unit -> ListStep<'T>) : ResizeArray<'T> = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member __.Populate () = __jumptable 0 __expand_code }).StartAsResizeArray()

let rsarray = ResizeArrayBuilder()

type ListBuilder() =     
    inherit ResizeArrayBuilderBase()
    member inline __.Run(__expand_code : unit -> ListStep<'T>) : 'T list = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member __.Populate () = __jumptable 0 __expand_code }).StartAsList()
        
let list = ListBuilder()

type ArrayBuilder() =     
    inherit ResizeArrayBuilderBase()
    member inline __.Run(__expand_code : unit -> ListStep<'T>) : 'T[] = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member __.Populate () = __jumptable 0 __expand_code }).StartAsArray()
        
let array = ArrayBuilder()

module Examples =

    let t1 () = 
        list {
           printfn "in t1"
           yield "a"
           let x = 1
           yield "b"
           yield "c"
        }

    let t2 () = 
        list {
           printfn "in t2"
           yield "d"
           for x in t1 () do 
               printfn "t2 - got %A" x
               yield "e"
               yield "[T1]" + x
           yield "f"
        }

    let perf1 () = 
        for i in 1 .. 1000000 do
            list {
               yield "a"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "c"
            } |> Seq.length |> ignore

    let perf2 () = 
        for i in 1 .. 1000000 do
            [
               yield "a"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "c"
            ] |> Seq.length |> ignore

    let perf s f = 
        let t = System.Diagnostics.Stopwatch()
        t.Start()
        f()
        t.Stop()
        printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    perf "perf1" perf1 
    perf "perf2" perf2 
    let dumpSeq (t: IEnumerable<_>) = 
        let e = t.GetEnumerator()
        while e.MoveNext() do 
            printfn "yield %A" e.Current
    dumpSeq (t1())
    dumpSeq (t2())
