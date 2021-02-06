
module Tests.ListAndArrayBuilder

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type YieldStateMachine<'T> =
    [<DefaultValue(false)>]
    val mutable Result : ResizeArray<'T>

    static member Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

    member sm.Yield (value: 'T) = 
        match sm.Result with 
        | null -> 
            let ra = ResizeArray()
            sm.Result <- ra
            ra.Add(value)
        | ra -> ra.Add(value)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToResizeArray() = 
        YieldStateMachine<_>.Run(&sm)
        match sm.Result with 
        | null -> ResizeArray()
        | ra -> ra
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToList() = 
        YieldStateMachine<_>.Run(&sm)
        match sm.Result with 
        | null -> []
        | ra -> ra |> Seq.toList
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToArray() = 
        YieldStateMachine<_>.Run(&sm)
        match sm.Result with 
        | null -> Array.empty
        | ra -> ra.ToArray()

type YieldCode<'T> = delegate of byref<YieldStateMachine<'T>> -> unit

type ResizeArrayBuilderBase() =
    

    member inline __.Delay(__expand_f : unit -> YieldCode<'T>) : YieldCode<'T> =
        YieldCode (fun sm -> (__expand_f()).Invoke &sm)

    member inline __.Zero() : YieldCode<'T> =
        YieldCode(fun _sm -> ())

    member inline __.Combine(__expand_task1: YieldCode<'T>, __expand_task2: YieldCode<'T>) : YieldCode<'T> =
        YieldCode(fun sm -> 
            __expand_task1.Invoke &sm
            __expand_task2.Invoke &sm)
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : YieldCode<'T>) : YieldCode<'T> =
        YieldCode(fun sm -> 
            while __expand_condition() do
                __expand_body.Invoke &sm)

    member inline __.TryWith(__expand_body : YieldCode<'T>, __expand_catch : exn -> YieldCode<'T>) : YieldCode<'T> =
        YieldCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with exn -> 
                (__expand_catch exn).Invoke &sm)

    member inline __.TryFinally(__expand_body: YieldCode<'T>, compensation : unit -> unit) : YieldCode<'T> =
        YieldCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with _ ->
                compensation()
                reraise()

            compensation())

    member inline b.Using(disp : #IDisposable, __expand_body : #IDisposable -> YieldCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        b.TryFinally(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline b.For(sequence : seq<'TElement>, __expand_body : 'TElement -> YieldCode<'T>) : YieldCode<'T> =
        b.Using (sequence.GetEnumerator(), 
            (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    member inline __.Yield (v: 'T) : YieldCode<'T> =
        YieldCode(fun sm ->
            sm.Yield v)

    member inline b.YieldFrom (source: IEnumerable<'T>) : YieldCode<'T> =
        b.For(source, (fun value -> b.Yield(value)))

    member inline __.RunCore(__expand_code : YieldCode<'T>) : ResizeArray<'T> = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<YieldStateMachine<'T>, _>
                (MoveNextMethod<YieldStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    YieldStateMachine<_>.Run(&sm)
                    sm.Result))
        else
            let mutable sm = YieldStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.Result

type ResizeArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : YieldCode<'T>) : ResizeArray<'T> = 
        match b.RunCore(__expand_code) with 
        | null -> ResizeArray()
        | ra -> ra

let rsarray = ResizeArrayBuilder()

type ListBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : YieldCode<'T>) : 'T list = 
        match b.RunCore(__expand_code) with 
        | null -> []
        | ra -> ra |> Seq.toList

let list = ListBuilder()

type ArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : YieldCode<'T>) : 'T[] = 
        match b.RunCore(__expand_code) with 
        | null -> Array.Empty()
        | ra -> ra.ToArray()

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
               if i % 3 = 0 then 
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

    let perf3 () = 
        for i in 1 .. 1000000 do
            [
               "a"
               "b"
               "b"
               "b"
               "b"
               if i % 3 = 0 then 
                   "b"
                   "b"
                   "b"
                   "b"
               "c"
            ] |> Seq.length |> ignore

    let perf1A () = 
        for i in 1 .. 1000000 do
            array {
               yield "a"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               if i % 3 = 0 then 
                   yield "b"
                   yield "b"
                   yield "b"
                   yield "b"
               yield "c"
            } |> Array.length |> ignore

    let perf2A () = 
        for i in 1 .. 1000000 do
            [|
               yield "a"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               if i % 3 = 0 then 
                   yield "b"
                   yield "b"
                   yield "b"
                   yield "b"
               yield "c"
            |] |> Array.length |> ignore

    let perf3A () = 
        for i in 1 .. 1000000 do
            [|
               "a"
               "b"
               "b"
               "b"
               if i % 3 = 0 then 
                   "b"
                   "b"
                   "b"
                   "b"
               "b"
               "c"
            |] |> Array.length |> ignore

(*
    let perf s f = 
        let t = System.Diagnostics.Stopwatch()
        t.Start()
        f()
        t.Stop()
        printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    perf "perf1 (list builder) " perf1 
    perf "perf2 (list expression explicit yield)" perf2 
    perf "perf3 (list expression implicit yield)" perf3
    perf "perf1A (array builder) " perf1A
    perf "perf2A (array expression explicit yield)" perf2A
    perf "perf3A (array expression implicit yield)" perf3A
    let dumpSeq (t: IEnumerable<_>) = 
        let e = t.GetEnumerator()
        while e.MoveNext() do 
            printfn "yield %A" e.Current
    dumpSeq (t1())
    dumpSeq (t2())

*)
