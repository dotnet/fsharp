
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
        match sm.Result with 
        | null -> ResizeArray()
        | ra -> ra
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToArray() = 
        match sm.Result with 
        | null -> Array.empty
        | ra -> ra.ToArray()

type ResizeArrayBuilderCode<'T> = delegate of byref<ResizeArrayBuilderStateMachine<'T>> -> unit

type ResizeArrayBuilderBase() =
    
    member inline __.Delay(__expand_f : unit -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode (fun sm -> (__expand_f()).Invoke &sm)

    member inline __.Zero() : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun _sm -> ())

    member inline __.Combine(__expand_task1: ResizeArrayBuilderCode<'T>, __expand_task2: ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun sm -> 
            __expand_task1.Invoke &sm
            __expand_task2.Invoke &sm)
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun sm -> 
            while __expand_condition() do
                __expand_body.Invoke &sm)

    member inline __.TryWith(__expand_body : ResizeArrayBuilderCode<'T>, __expand_catch : exn -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with exn -> 
                (__expand_catch exn).Invoke &sm)

    member inline __.TryFinally(__expand_body: ResizeArrayBuilderCode<'T>, compensation : unit -> unit) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with _ ->
                compensation()
                reraise()

            compensation())

    member inline b.Using(disp : #IDisposable, __expand_body : #IDisposable -> ResizeArrayBuilderCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        b.TryFinally(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline b.For(sequence : seq<'TElement>, __expand_body : 'TElement -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
        b.Using (sequence.GetEnumerator(), 
            (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    member inline __.Yield (v: 'T) : ResizeArrayBuilderCode<'T> =
        ResizeArrayBuilderCode(fun sm ->
            sm.Yield v)

    member inline b.YieldFrom (source: IEnumerable<'T>) : ResizeArrayBuilderCode<'T> =
        b.For(source, (fun value -> b.Yield(value)))


type ResizeArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : ResizeArrayBuilderCode<'T>) : ResizeArray<'T> = 
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
                    sm.ToResizeArray()))
        else
            let mutable sm = ResizeArrayBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.ToResizeArray()

let rsarray = ResizeArrayBuilder()

type ArrayBuilder() =     
    inherit ResizeArrayBuilderBase()

    member inline b.Run(__expand_code : ResizeArrayBuilderCode<'T>) : 'T[] = 
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
                    sm.ToArray()))
        else
            let mutable sm = ResizeArrayBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.ToArray()

let array = ArrayBuilder()

module Examples =
    let tinyVariableSize () = 
        for i in 1 .. 1000000 do
            array {
               if i % 3 = 0 then 
                   yield "b"
            } |> Array.length |> ignore

    let tinyVariableSizeBase () = 
        for i in 1 .. 1000000 do
            [|
               if i % 3 = 0 then 
                   yield "b"
            |] |> Array.length |> ignore

    let variableSize () = 
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

    let variableSizeBase () = 
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

    let fixedSize () = 
        for i in 1 .. 1000000 do
            array {
               "a"
               "b"
               "b"
               "b"
               "b"
               "b"
               "b"
               "b"
               "b"
               "c"
             } |> Array.length |> ignore

    let fixedSizeBase () = 
        for i in 1 .. 1000000 do
            [|
               "a"
               "b"
               "b"
               "b"
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

    perf "tinyVariableSize (array builder) " tinyVariableSize
    perf "tinyVariableSizeBase (array expression)" tinyVariableSizeBase
    perf "variableSize (array builder) " variableSize
    perf "variableSizeBase (array expression)" variableSizeBase
    perf "fixedSize (array builder) " fixedSize
    perf "fixedSizeBase (array expression)" fixedSizeBase
    // let dumpSeq (t: IEnumerable<_>) = 
    //     let e = t.GetEnumerator()
    //     while e.MoveNext() do 
    //         printfn "yield %A" e.Current
    // dumpSeq (t1())
    // dumpSeq (t2())
