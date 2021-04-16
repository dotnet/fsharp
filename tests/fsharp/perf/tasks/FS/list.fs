
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

    static member Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

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
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.ToList() = 
        ListBuilderStateMachine<_>.Run(&sm)
        match box sm.Result with 
        | null -> []
        | _ ->
            RuntimeHelpers.SetFreshConsTail sm.LastCons []
            sm.Result
    
type ListBuilderCode<'T> = delegate of byref<ListBuilderStateMachine<'T>> -> unit

type ListBuilder() =

    member inline __.Delay(__expand_f : unit -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
        ListBuilderCode (fun sm -> (__expand_f()).Invoke &sm)

    member inline __.Zero() : ListBuilderCode<'T> =
        ListBuilderCode(fun _sm -> ())

    member inline __.Combine(__expand_task1: ListBuilderCode<'T>, __expand_task2: ListBuilderCode<'T>) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            __expand_task1.Invoke &sm
            __expand_task2.Invoke &sm)
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : ListBuilderCode<'T>) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            while __expand_condition() do
                __expand_body.Invoke &sm)

    member inline __.TryWith(__expand_body : ListBuilderCode<'T>, __expand_catch : exn -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with exn -> 
                (__expand_catch exn).Invoke &sm)

    member inline __.TryFinally(__expand_body: ListBuilderCode<'T>, compensation : unit -> unit) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm -> 
            try
                __expand_body.Invoke &sm
            with _ ->
                compensation()
                reraise()

            compensation())

    member inline b.Using(disp : #IDisposable, __expand_body : #IDisposable -> ListBuilderCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        b.TryFinally(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline b.For(sequence : seq<'TElement>, __expand_body : 'TElement -> ListBuilderCode<'T>) : ListBuilderCode<'T> =
        b.Using (sequence.GetEnumerator(), 
            (fun e -> b.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    member inline __.Yield (v: 'T) : ListBuilderCode<'T> =
        ListBuilderCode(fun sm ->
            sm.Yield v)

    member inline b.YieldFrom (source: IEnumerable<'T>) : ListBuilderCode<'T> =
        b.For(source, (fun value -> b.Yield(value)))

    member inline b.Run(__expand_code : ListBuilderCode<'T>) : 'T list = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<ListBuilderStateMachine<'T>, _>
                (MoveNextMethod<ListBuilderStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    ListBuilderStateMachine<_>.Run(&sm)
                    sm.Result))
        else
            let mutable sm = ListBuilderStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.Result

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

    let perf1L () = 
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

    let perf1Li () = 
        for i in 1 .. 1000000 do
            list {
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
            } |> Seq.length |> ignore

    let perf2L () = 
        for i in 1 .. 1000000 do
            [
               yield "a"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               yield "b"
               if i % 3 = 0 then 
                   yield "b"
                   yield "b"
                   yield "b"
                   yield "c"
            ] |> Seq.length |> ignore

    // Should be identical to perf2
    let perf3L () = 
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

    let perf s f = 
        let t = System.Diagnostics.Stopwatch()
        t.Start()
        f()
        t.Stop()
        printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    perf "perf1 (list builder yield) " perf1L
    perf "perf1 (list builder implicit yield) " perf1Li
    perf "perf2 (list expression explicit yield)" perf2L
    perf "perf3 (list expression implicit yield)" perf3L
    //printfn "t1() = %A" (t1())
