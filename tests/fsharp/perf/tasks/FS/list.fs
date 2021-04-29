
module Tests.ListBuilder

open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type ListBuilderStateMachine<'T> =
    [<DefaultValue(false)>]
    val mutable Result : 'T list
    [<DefaultValue(false)>]
    val mutable LastCons : 'T list

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

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
    
type ListBuilderCode<'T> = delegate of byref<ListBuilderStateMachine<'T>> -> unit

type ListBuilder() =

    member inline _.Delay([<ResumableCode>] __expand_f : unit -> ListBuilderCode<'T>) : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode (fun sm -> (__expand_f()).Invoke &sm)

    member inline _.Zero() : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode(fun _sm -> ())

    member inline _.Combine([<ResumableCode>] __expand_part1: ListBuilderCode<'T>, [<ResumableCode>] __expand_part2: ListBuilderCode<'T>) : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            __expand_part1.Invoke &sm
            __expand_part2.Invoke &sm)
            
    member inline _.While([<ResumableCode>] __expand_condition : unit -> bool, [<ResumableCode>] __expand_body : ListBuilderCode<'T>) : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            while __expand_condition() do
                __expand_body.Invoke &sm)

    member inline _.TryWith([<ResumableCode>] __expand_body : ListBuilderCode<'T>, [<ResumableCode>] __expand_catch : exn -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with exn -> 
                (__expand_catch exn).Invoke &sm)

    member inline _.TryFinally([<ResumableCode>] __expand_body: ListBuilderCode<'T>, compensation : unit -> unit) : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with _ ->
                compensation()
                reraise()

            compensation())

    member inline b.Using(disp : #IDisposable, [<ResumableCode>] __expand_body : #IDisposable -> ListBuilderCode<'T>) : [<ResumableCode>] ListBuilderCode<'T> = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        b.TryFinally(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline b.For(sequence : seq<'TElement>, [<ResumableCode>] __expand_body : 'TElement -> ListBuilderCode<'T>) : [<ResumableCode>] ListBuilderCode<'T> =
        b.Using (sequence.GetEnumerator(), 
            (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    member inline _.Yield (v: 'T) : [<ResumableCode>] ListBuilderCode<'T> =
        ListBuilderCode(fun sm ->
            sm.Yield v)

    member inline b.YieldFrom (source: IEnumerable<'T>) : [<ResumableCode>] ListBuilderCode<'T> =
        b.For(source, (fun value -> b.Yield(value)))

    member inline b.Run([<ResumableCode>] __expand_code : ListBuilderCode<'T>) : 'T list = 
        if __useResumableCode then
            __structStateMachine<ListBuilderStateMachine<'T>, _>
                (MoveNextMethod<ListBuilderStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    ListBuilderStateMachine<_>.Run(&sm)
                    sm.ToList()))
        else
            let mutable sm = ListBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.ToList()

let list = ListBuilder()

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

    let tinyVariableSize () = 
        for i in 1 .. 1000000 do
            list {
               if i % 3 = 0 then 
                   yield "b"
            } |> List.length |> ignore

    let tinyVariableSizeBase () = 
        for i in 1 .. 1000000 do
            [
               if i % 3 = 0 then 
                   yield "b"
            ] |> List.length |> ignore

    let variableSize () = 
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

    let fixedSize () = 
        for i in 1 .. 1000000 do
            list {
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
    // let perf s f = 
    //     let t = System.Diagnostics.Stopwatch()
    //     t.Start()
    //     f()
    //     t.Stop()
    //     printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    // perf "tinyVariableSize (list builder) " tinyVariableSize
    // perf "tinyVariableSizeBase (list expression)" tinyVariableSizeBase
    // perf "variableSize (list builder) " variableSize
    // perf "variableSizeBase (list expression)" variableSizeBase
    // perf "fixedSize (list builder) " fixedSize
    // perf "fixedSizeBase (list expression)" fixedSizeBase
    // //printfn "t1() = %A" (t1())
