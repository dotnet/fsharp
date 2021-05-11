
module Tests.ArrayBuilders

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AutoOpen>]
module UsingStateMachines =
    [<Struct; NoEquality; NoComparison>]
    type ResizeArrayBuilderStateMachine<'T> =
        [<DefaultValue(false)>]
        val mutable Result : ResizeArray<'T>

        interface IAsyncStateMachine with 
            member sm.MoveNext() = failwith "no dynamic impl"
            member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

        static member inline Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

        member inline sm.Yield (value: 'T) = 
            match sm.Result with 
            | null -> 
                let ra = ResizeArray()
                sm.Result <- ra
                ra.Add(value)
            | ra -> ra.Add(value)

        member inline sm.ToResizeArray() = 
            match sm.Result with 
            | null -> ResizeArray()
            | ra -> ra
    
        member inline sm.ToArray() = 
            match sm.Result with 
            | null -> Array.empty
            | ra -> ra.ToArray()

    [<ResumableCode>]
    type ResizeArrayBuilderCode<'T> = delegate of byref<ResizeArrayBuilderStateMachine<'T>> -> unit

    type ResizeArrayBuilderUsingStateMachinesBase() =
    
        member inline __.Delay(f : unit -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode (fun sm -> (f()).Invoke &sm)

        member inline __.Zero() : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun _sm -> ())

        member inline __.Combine(task1: ResizeArrayBuilderCode<'T>, task2: ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun sm -> 
                task1.Invoke &sm
                task2.Invoke &sm)
            
        member inline __.While(condition : unit -> bool, body : ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun sm -> 
                while condition() do
                    body.Invoke &sm)

        member inline __.TryWith(body : ResizeArrayBuilderCode<'T>, catch : exn -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun sm -> 
                try
                    body.Invoke &sm
                with exn -> 
                    (catch exn).Invoke &sm)

        member inline __.TryFinally(body: ResizeArrayBuilderCode<'T>, compensation : unit -> unit) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun sm -> 
                try
                    body.Invoke &sm
                with _ ->
                    compensation()
                    reraise()

                compensation())

        member inline b.Using(disp : #IDisposable, body : #IDisposable -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> = 
            // A using statement is just a try/finally with the finally block disposing if non-null.
            b.TryFinally(
                (fun sm -> (body disp).Invoke &sm),
                (fun () -> if not (isNull (box disp)) then disp.Dispose()))

        member inline b.For(sequence : seq<'TElement>, body : 'TElement -> ResizeArrayBuilderCode<'T>) : ResizeArrayBuilderCode<'T> =
            b.Using (sequence.GetEnumerator(), 
                (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke &sm))))

        member inline __.Yield (v: 'T) : ResizeArrayBuilderCode<'T> =
            ResizeArrayBuilderCode(fun sm ->
                sm.Yield v)

        member inline b.YieldFrom (source: IEnumerable<'T>) : ResizeArrayBuilderCode<'T> =
            b.For(source, (fun value -> b.Yield(value)))


    type ResizeArrayBuilderUsingStateMachines() =     
        inherit ResizeArrayBuilderUsingStateMachinesBase()

        member inline b.Run(code : ResizeArrayBuilderCode<'T>) : ResizeArray<'T> = 
            if __useResumableCode then
                __structStateMachine<ResizeArrayBuilderStateMachine<'T>, _>
                   // IAsyncStateMachine.MoveNext
                    (MoveNextMethodImpl<ResizeArrayBuilderStateMachine<'T>>(fun sm -> 
                           code.Invoke(&sm)
                           ))

                    // IAsyncStateMachine.SetStateMachine
                    (SetStateMachineMethodImpl<_>(fun sm state -> 
                        ()))

                    // Other interfaces
                    [| |]

                    // Start
                    (AfterCode<_,_>(fun sm -> 
                        ResizeArrayBuilderStateMachine<_>.Run(&sm)
                        sm.ToResizeArray()))
            else
                let mutable sm = ResizeArrayBuilderStateMachine<'T>()
                code.Invoke(&sm)
                sm.ToResizeArray()

    let rsarraysm = ResizeArrayBuilderUsingStateMachines()

    type ArrayBuilderUsingStateMachines() =     
        inherit ResizeArrayBuilderUsingStateMachinesBase()

        member inline b.Run(code : ResizeArrayBuilderCode<'T>) : 'T[] = 
            if __useResumableCode then
                __structStateMachine<ResizeArrayBuilderStateMachine<'T>, _>
                    (MoveNextMethodImpl<ResizeArrayBuilderStateMachine<'T>>(fun sm -> 
                           code.Invoke(&sm)
                           ))

                    // SetStateMachine
                    (SetStateMachineMethodImpl<_>(fun sm state -> 
                        ()))

                    // Other interfaces
                    [| |]

                    // Start
                    (AfterCode<_,_>(fun sm -> 
                        ResizeArrayBuilderStateMachine<_>.Run(&sm)
                        sm.ToArray()))
            else
                let mutable sm = ResizeArrayBuilderStateMachine<'T>()
                code.Invoke(&sm)
                sm.ToArray()

    let arraysm = ArrayBuilderUsingStateMachines()

[<AutoOpen>]
module UsingInlinedCodeAndCollector =

    [<Struct; NoEquality; NoComparison>]
    type ArrayBuilderCollector<'T> =
        [<DefaultValue(false)>]
        val mutable Result : ResizeArray<'T>

        member sm.Yield (value: 'T) = 
            match sm.Result with 
            | null -> 
                let ra = ResizeArray()
                sm.Result <- ra
                ra.Add(value)
            | ra -> ra.Add(value)

        member sm.ToResizeArray() = 
            match sm.Result with 
            | null -> ResizeArray()
            | ra -> ra
    
        member sm.ToArray() = 
            match sm.Result with 
            | null -> Array.empty
            | ra -> ra.ToArray()
    
    type ArrayBuilderCode<'T> = delegate of byref<ArrayBuilderCollector<'T>> -> unit

    type ArrayBuilderViaCollector() =

        member inline _.Delay([<InlineIfLambda>] f: unit -> ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm -> (f()).Invoke &sm)

        member inline _.Zero() : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun _sm -> ())

        member inline _.Combine([<InlineIfLambda>] part1: ArrayBuilderCode<'T>, [<InlineIfLambda>] part2: ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm -> 
                part1.Invoke &sm
                part2.Invoke &sm)
            
        member inline _.While([<InlineIfLambda>] condition : unit -> bool, [<InlineIfLambda>] body : ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm -> 
                while condition() do
                    body.Invoke &sm)

        member inline _.TryWith([<InlineIfLambda>] body: ArrayBuilderCode<'T>, [<InlineIfLambda>] handler: exn -> ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm -> 
                try
                    body.Invoke &sm
                with exn -> 
                    (handler exn).Invoke &sm)

        member inline _.TryFinally([<InlineIfLambda>] body: ArrayBuilderCode<'T>, compensation : unit -> unit) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm -> 
                try
                    body.Invoke &sm
                with _ ->
                    compensation()
                    reraise()

                compensation())

        member inline b.Using(disp : #IDisposable, [<InlineIfLambda>] body: #IDisposable -> ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> = 
            // A using statement is just a try/finally with the finally block disposing if non-null.
            b.TryFinally(
                (fun sm -> (body disp).Invoke &sm),
                (fun () -> if not (isNull (box disp)) then disp.Dispose()))

        member inline b.For(sequence: seq<'TElement>, [<InlineIfLambda>] body: 'TElement -> ArrayBuilderCode<'T>) : ArrayBuilderCode<'T> =
            b.Using (sequence.GetEnumerator(), 
                (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke &sm))))

        member inline _.Yield (v: 'T) : ArrayBuilderCode<'T> =
            ArrayBuilderCode<_>(fun sm ->
                sm.Yield v)

        member inline b.YieldFrom (source: IEnumerable<'T>) : ArrayBuilderCode<'T> =
            b.For(source, (fun value -> b.Yield(value)))

        member inline _.Run([<InlineIfLambda>] code: ArrayBuilderCode<'T>) : 'T[] = 
            let mutable sm = ArrayBuilderCollector<'T>()
            code.Invoke &sm
            sm.ToArray()

    let arrayc = ArrayBuilderViaCollector()

module Examples =
    let tinyVariableSizeSM () = 
        for i in 1 .. 1000000 do
            arraysm {
               if i % 3 = 0 then 
                   yield "b"
            } |> Array.length |> ignore

    let tinyVariableSizeC () = 
        for i in 1 .. 1000000 do
            arrayc {
               if i % 3 = 0 then 
                   yield "b"
            } |> Array.length |> ignore

    let tinyVariableSizeBase () = 
        for i in 1 .. 1000000 do
            [|
               if i % 3 = 0 then 
                   yield "b"
            |] |> Array.length |> ignore

    let variableSizeSM () = 
        for i in 1 .. 1000000 do
            arraysm {
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

    let variableSizeC () = 
        for i in 1 .. 1000000 do
            arrayc {
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

    let fixedSizeSM () = 
        for i in 1 .. 1000000 do
            arraysm {
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

    let fixedSizeC () = 
        for i in 1 .. 1000000 do
            arrayc {
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

    perf "tinyVariableSizeBase" tinyVariableSizeBase
    perf "tinyVariableSizeSM " tinyVariableSizeSM
    perf "tinyVariableSizeC " tinyVariableSizeC

    perf "variableSizeBase" variableSizeBase
    perf "variableSizeSM" variableSizeSM
    perf "variableSizeC" variableSizeC

    perf "fixedSizeBase" fixedSizeBase
    perf "fixedSizeSM" fixedSizeSM
    perf "fixedSizeC" fixedSizeC
    // let dumpSeq (t: IEnumerable<_>) = 
    //     let e = t.GetEnumerator()
    //     while e.MoveNext() do 
    //         printfn "yield %A" e.Current
    // dumpSeq (t1())
    // dumpSeq (t2())
