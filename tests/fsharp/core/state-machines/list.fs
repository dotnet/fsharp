
module Tests.ListAndArrayBuilder

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AbstractClass>]
type ListStateMachine<'T>() =
    let res = ResizeArray<'T>()

    abstract Populate : unit -> unit

    member __.Yield (v: 'T) = res.Add(v)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.ToResizeArray() = 
        this.Populate()
        res
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.ToList() = 
        this.Populate()
        Seq.toList res
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.ToArray() = 
        this.Populate() |> ignore
        res.ToArray()

type ListCode<'T> = ListStateMachine<'T> -> unit

type ResizeArrayBuilderBase() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> ListCode<'T>) : ListCode<'T> = (fun sm -> (__expand_f()) sm)

    [<NoDynamicInvocation>]
    member inline __.Zero() : ListCode<'T> =
        (fun _sm -> ())

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_task1: ListCode<'T>, __expand_task2: ListCode<'T>) : ListCode<'T> =
        (fun sm -> 
            __expand_task1 sm
            __expand_task2 sm)
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : ListCode<'T>) : ListCode<'T> =
        (fun sm -> 
            while __expand_condition() do
                __expand_body sm)

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : ListCode<'T>, __expand_catch : exn -> ListCode<'T>) : ListCode<'T> =
        (fun sm -> 
            try
                __expand_body sm
            with exn -> 
                __expand_catch exn sm)

    [<NoDynamicInvocation>]
    member inline __.TryFinally(__expand_body: ListCode<'T>, compensation : unit -> unit) : ListCode<'T> =
        (fun sm -> 
            try
                __expand_body sm
            with _ ->
                compensation()
                reraise()

            compensation())

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> ListCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> __expand_body disp sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> ListCode<'T>) : ListCode<'T> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> __expand_body e.Current sm))))

    [<NoDynamicInvocation>]
    member inline __.Yield (v: 'T) : ListCode<'T> =
        (fun sm ->
            sm.Yield v)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom (source: IEnumerable<'T>) : ListCode<'T> =
        this.For(source, (fun ``__machine_step$cont`` -> this.Yield(``__machine_step$cont``)))

type ResizeArrayBuilder() =     
    inherit ResizeArrayBuilderBase()
    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : ListCode<'T>) : ResizeArray<'T> = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member sm.Populate () = __jumptable 0 (__expand_code sm) }).ToResizeArray()

let rsarray = ResizeArrayBuilder()

type ListBuilder() =     
    inherit ResizeArrayBuilderBase()
    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : ListCode<'T>) : 'T list = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member sm.Populate () = __jumptable 0 (__expand_code sm) }).ToList()
        
let list = ListBuilder()

type ArrayBuilder() =     
    inherit ResizeArrayBuilderBase()
    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : ListCode<'T>) : 'T[] = 
        (__stateMachine
            { new ListStateMachine<'T>() with 
                member sm.Populate () = __jumptable 0 (__expand_code sm) }).ToArray()
        
let array = ArrayBuilder()

module Examples =

    let t1 () = 
        list {
           printfn "in t1"
           yield "a"
           let x = "d"
           yield "b"
           yield "c" + x
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
