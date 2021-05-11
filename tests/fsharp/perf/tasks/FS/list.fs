
module Tests.ListBuilders

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

#nowarn "57"

[<AutoOpen>]
module UsingStateMachines =
    [<Struct; NoEquality; NoComparison>]
    type ListBuilderStateMachine<'T> =
        [<DefaultValue(false)>]
        val mutable Result : 'T list

        [<DefaultValue(false)>]
        val mutable LastCons : 'T list

        interface IAsyncStateMachine with 
            member _.MoveNext() = failwith "no dynamic impl"
            member _.SetStateMachine(_state: IAsyncStateMachine) = failwith "no dynamic impl"

        static member inline Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

        member inline sm.Yield (value: 'T) = 
            match box sm.Result with 
            | null -> 
                let ra = RuntimeHelpers.FreshConsNoTail value
                sm.Result <- ra
                sm.LastCons <- ra
            | ra -> 
                let ra = RuntimeHelpers.FreshConsNoTail value
                RuntimeHelpers.SetFreshConsTail sm.LastCons ra
                sm.LastCons <- ra
    
        member inline sm.ToList() = 
            match box sm.Result with 
            | null -> []
            | _ ->
                RuntimeHelpers.SetFreshConsTail sm.LastCons []
                sm.Result
    
    [<ResumableCode>]
    type ListBuilderStateMachineCode<'T> = delegate of byref<ListBuilderStateMachine<'T>> -> unit

    type ListBuilderViaStateMachine() =

        member inline _.Delay(f : unit -> ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode (fun sm -> (f()).Invoke &sm)

        member inline _.Zero() : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun _sm -> ())

        member inline _.Combine(part1: ListBuilderStateMachineCode<'T>, part2: ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun sm -> 
                part1.Invoke &sm
                part2.Invoke &sm)
            
        member inline _.While(condition : unit -> bool, body : ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun sm -> 
                while condition() do
                    body.Invoke &sm)

        member inline _.TryWith(body : ListBuilderStateMachineCode<'T>, catch : exn -> ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun sm -> 
                try
                    body.Invoke &sm
                with exn -> 
                    (catch exn).Invoke &sm)

        member inline _.TryFinally(body: ListBuilderStateMachineCode<'T>, compensation : unit -> unit) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun sm -> 
                try
                    body.Invoke &sm
                with _ ->
                    compensation()
                    reraise()

                compensation())

        member inline b.Using(disp : #IDisposable, body : #IDisposable -> ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> = 
            // A using statement is just a try/finally with the finally block disposing if non-null.
            b.TryFinally(
                (fun sm -> (body disp).Invoke &sm),
                (fun () -> if not (isNull (box disp)) then disp.Dispose()))

        member inline b.For(sequence : seq<'TElement>, body : 'TElement -> ListBuilderStateMachineCode<'T>) : ListBuilderStateMachineCode<'T> =
            b.Using (sequence.GetEnumerator(), 
                (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke &sm))))

        member inline _.Yield (v: 'T) : ListBuilderStateMachineCode<'T> =
            ListBuilderStateMachineCode(fun sm ->
                sm.Yield v)

        member inline b.YieldFrom (source: IEnumerable<'T>) : ListBuilderStateMachineCode<'T> =
            b.For(source, (fun value -> b.Yield(value)))

        member inline _.Run(code : ListBuilderStateMachineCode<'T>) : 'T list = 
            if __useResumableCode then
                __structStateMachine<ListBuilderStateMachine<'T>, _>
                    (MoveNextMethodImpl<ListBuilderStateMachine<'T>>(fun sm -> 
                           code.Invoke(&sm)
                           ))

                    // SetStateMachine
                    (SetStateMachineMethodImpl<_>(fun sm state -> 
                        ()))

                    // Other interfaces
                    [| |]

                    // Start
                    (AfterCode<_,_>(fun sm -> 
                        ListBuilderStateMachine<_>.Run(&sm)
                        sm.ToList()))
            else
                let mutable sm = ListBuilderStateMachine<'T>()
                code.Invoke(&sm)
                sm.ToList()

    let listsm = ListBuilderViaStateMachine()

[<AutoOpen>]
module UsingInlinedCodeAndCollector =
    [<Struct; NoEquality; NoComparison>]
    type ListBuilderCollector<'T> =
        [<DefaultValue(false)>]
        val mutable Result : 'T list

        [<DefaultValue(false)>]
        val mutable LastCons : 'T list

        member sm.Yield (value: 'T) = 
            match box sm.Result with 
            | null -> 
                let ra = RuntimeHelpers.FreshConsNoTail value
                sm.Result <- ra
                sm.LastCons <- ra
            | ra -> 
                let ra = RuntimeHelpers.FreshConsNoTail value
                RuntimeHelpers.SetFreshConsTail sm.LastCons ra
                sm.LastCons <- ra
    
        member sm.ToList() = 
            match box sm.Result with 
            | null -> []
            | _ ->
                RuntimeHelpers.SetFreshConsTail sm.LastCons []
                sm.Result
    
    type ListBuilderCode<'T> = delegate of byref<ListBuilderCollector<'T>> -> unit

    type ListBuilderViaCollector() =

        member inline _.Delay([<InlineIfLambda>] f: unit -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm -> (f()).Invoke &sm)

        member inline _.Zero() : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun _sm -> ())

        member inline _.Combine([<InlineIfLambda>] part1: ListBuilderCode<'T>, [<InlineIfLambda>] part2: ListBuilderCode<'T>) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm -> 
                part1.Invoke &sm
                part2.Invoke &sm)
            
        member inline _.While([<InlineIfLambda>] condition : unit -> bool, [<InlineIfLambda>] body : ListBuilderCode<'T>) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm -> 
                while condition() do
                    body.Invoke &sm)

        member inline _.TryWith([<InlineIfLambda>] body: ListBuilderCode<'T>, [<InlineIfLambda>] handler: exn -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm -> 
                try
                    body.Invoke &sm
                with exn -> 
                    (handler exn).Invoke &sm)

        member inline _.TryFinally([<InlineIfLambda>] body: ListBuilderCode<'T>, compensation : unit -> unit) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm -> 
                try
                    body.Invoke &sm
                with _ ->
                    compensation()
                    reraise()

                compensation())

        member inline b.Using(disp : #IDisposable, [<InlineIfLambda>] body: #IDisposable -> ListBuilderCode<'T>) : ListBuilderCode<'T> = 
            // A using statement is just a try/finally with the finally block disposing if non-null.
            b.TryFinally(
                (fun sm -> (body disp).Invoke &sm),
                (fun () -> if not (isNull (box disp)) then disp.Dispose()))

        member inline b.For(sequence: seq<'TElement>, [<InlineIfLambda>] body: 'TElement -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
            b.Using (sequence.GetEnumerator(), 
                (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (body e.Current).Invoke &sm))))

        member inline _.Yield (v: 'T) : ListBuilderCode<'T> =
            ListBuilderCode<_>(fun sm ->
                sm.Yield v)

        member inline b.YieldFrom (source: IEnumerable<'T>) : ListBuilderCode<'T> =
            b.For(source, (fun value -> b.Yield(value)))

        member inline _.Run([<InlineIfLambda>] code: ListBuilderCode<'T>) : 'T list = 
            let mutable sm = ListBuilderCollector<'T>()
            code.Invoke &sm
            sm.ToList()

    let listc = ListBuilderViaCollector()

module Examples =
    let t1SM () = 
        listsm {
           printfn "in t1"
           yield "a"
           let x = "d"
           yield "b"
           yield "c" + x
        }

    let t2SM () = 
        listsm {
           printfn "in t2"
           yield "d"
           for x in t1SM () do 
               printfn "t2 - got %A" x
               yield "e"
               yield "[T1]" + x
           yield "f"
        }

    let t1C () = 
        listc {
           printfn "in t1"
           yield "a"
           let x = "d"
           yield "b"
           yield "c" + x
        }

    let t2C () = 
        listc {
           printfn "in t2"
           yield "d"
           for x in t1C () do 
               printfn "t2 - got %A" x
               yield "e"
               yield "[T1]" + x
           yield "f"
        }

    let tinyVariableSizeC () = 
        for i in 1 .. 1000000 do
            listc {
               if i % 3 = 0 then 
                   yield "b"
            } |> List.length |> ignore

    let tinyVariableSizeSM () = 
        for i in 1 .. 1000000 do
            listsm {
               if i % 3 = 0 then 
                   yield "b"
            } |> List.length |> ignore

    let tinyVariableSizeBase () = 
        for i in 1 .. 1000000 do
            [
               if i % 3 = 0 then 
                   yield "b"
            ] |> List.length |> ignore

    let variableSizeSM () = 
        for i in 1 .. 1000000 do
            listsm {
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
            } |> List.length |> ignore

    let variableSizeC () = 
        for i in 1 .. 1000000 do
            listc {
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
            } |> List.length |> ignore

    let variableSizeBase () = 
        for i in 1 .. 1000000 do
            [
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
            ] |> List.length |> ignore

    let fixedSizeSM () = 
        for i in 1 .. 1000000 do
            listsm {
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
             } |> List.length |> ignore

    let fixedSizeC () = 
        for i in 1 .. 1000000 do
            listc {
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
             } |> List.length |> ignore

    let fixedSizeBase () = 
        for i in 1 .. 1000000 do
            [
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
            ] |> List.length |> ignore

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
