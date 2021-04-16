
module Tests.ArrayBuilder

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type ResizeArrayBuilderStateMachine<'T> =
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
        ResizeArrayBuilderStateMachine<_>.Run(&sm)
        match sm.Result with 
        | null -> ResizeArray()
        | ra -> ra
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToArray() = 
        ResizeArrayBuilderStateMachine<_>.Run(&sm)
        match sm.Result with 
        | null -> Array.empty
        | ra -> ra.ToArray()

type YieldCode<'T> = delegate of byref<ResizeArrayBuilderStateMachine<'T>> -> unit

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


type ResizeArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : YieldCode<'T>) : ResizeArray<'T> = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<ResizeArrayBuilderStateMachine<'T>, _>
                (MoveNextMethod<ResizeArrayBuilderStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    ResizeArrayBuilderStateMachine<_>.Run(&sm)
                    match sm.Result with 
                    | null -> ResizeArray()
                    | ra -> ra))
        else
            let mutable sm = ResizeArrayBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.Result

let rsarray = ResizeArrayBuilder()

type ArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : YieldCode<'T>) : 'T[] = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<ResizeArrayBuilderStateMachine<'T>, _>
                (MoveNextMethod<ResizeArrayBuilderStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    ResizeArrayBuilderStateMachine<_>.Run(&sm)
                    match sm.Result with 
                    | null -> Array.Empty()
                    | ra -> ra.ToArray()))
        else
            let mutable sm = ResizeArrayBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            match sm.Result with 
            | null -> Array.Empty()
            | ra -> ra.ToArray()

let array = ArrayBuilder()

module Examples =
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
    let perf s f = 
        let t = System.Diagnostics.Stopwatch()
        t.Start()
        f()
        t.Stop()
        printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    perf "perf1A (array builder) " perf1A
    perf "perf2A (array expression explicit yield)" perf2A
    perf "perf3A (array expression implicit yield)" perf3A
    // let dumpSeq (t: IEnumerable<_>) = 
    //     let e = t.GetEnumerator()
    //     while e.MoveNext() do 
    //         printfn "yield %A" e.Current
    // dumpSeq (t1())
    // dumpSeq (t2())
