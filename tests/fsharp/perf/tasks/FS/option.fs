
module Tests.OptionBuilder

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type OptionStateMachine<'T> =
    [<DefaultValue(false)>]
    val mutable Result : 'T voption

    member sm.ToOption() = match sm.Result with ValueNone -> None | ValueSome x -> Some x

    member sm.ToValueOption() = sm.Result 

    static member Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

type OptionCode<'T> = delegate of byref<OptionStateMachine<'T>> -> unit

type OptionBuilderBase() =

    member inline __.Delay(__expand_f : unit -> OptionCode<'T>) : OptionCode<'T> = OptionCode (fun sm -> (__expand_f()).Invoke &sm)

    member inline __.Combine(__expand_task1: OptionCode<unit>, __expand_task2: OptionCode<'T>) : OptionCode<'T> =
        OptionCode<_>(fun sm -> 
            let mutable sm2 = OptionStateMachine<unit>()
            __expand_task1.Invoke &sm2
            __expand_task2.Invoke &sm)

    member inline __.Bind(res1: 'T1 option, __expand_task2: ('T1 -> OptionCode<'T>)) : OptionCode<'T> =
        OptionCode<_>(fun sm -> 
            match res1 with 
            | None -> ()
            | Some v -> (__expand_task2 v).Invoke &sm)

    member inline __.Bind(res1: 'T1 voption, __expand_task2: ('T1 -> OptionCode<'T>)) : OptionCode<'T> =
        OptionCode<_>(fun sm -> 
            match res1 with 
            | ValueNone -> ()
            | ValueSome v -> (__expand_task2 v).Invoke &sm)
            
    member inline __.While(__expand_condition : unit -> bool, __expand_body : OptionCode<unit>) : OptionCode<unit> =
        OptionCode<_>(fun sm -> 
            while __expand_condition() do
                __expand_body.Invoke &sm)

    member inline __.TryWith(__expand_body : OptionCode<'T>, __expand_catch : exn -> OptionCode<'T>) : OptionCode<'T> =
        OptionCode<_>(fun sm -> 
            try
                __expand_body.Invoke &sm
            with exn -> 
                (__expand_catch exn).Invoke &sm)

    member inline __.TryFinally(__expand_body: OptionCode<'T>, compensation : unit -> unit) : OptionCode<'T> =
        OptionCode<_>(fun sm -> 
            try
                __expand_body.Invoke &sm
            with _ ->
                compensation()
                reraise()

            compensation())

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> OptionCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> (__expand_body disp).Invoke &sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'TElement>, __expand_body : 'TElement -> OptionCode<unit>) : OptionCode<unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun sm -> (__expand_body e.Current).Invoke &sm))))

    member inline __.Return (value: 'T) : OptionCode<'T> =
        OptionCode<_>(fun sm ->
            sm.Result <- ValueSome value)

    member inline this.ReturnFrom (source: option<'T>) : OptionCode<'T> =
        OptionCode<_>(fun sm ->
            sm.Result <- match source with Some x -> ValueOption.Some x | None -> ValueOption.None)

type OptionBuilder() =
    inherit OptionBuilderBase()
    

    member inline __.Run(__expand_code : OptionCode<'T>) : 'T option = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<OptionStateMachine<'T>, 'T option>
                (MoveNextMethod<_>(fun sm -> 
                       __expand_code.Invoke(&sm)))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    OptionStateMachine<_>.Run(&sm)
                    sm.ToOption()))
        else
            let mutable sm = OptionStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.ToOption()

type ValueOptionBuilder() =
    inherit OptionBuilderBase()
    

    member inline __.Run(__expand_code : OptionCode<'T>) : 'T voption = 
        if __useResumableStateMachines then
            __resumableStateMachineStruct<OptionStateMachine<'T>, 'T voption>
                (MoveNextMethod<OptionStateMachine<'T>>(fun sm -> 
                       __expand_code.Invoke(&sm)
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    OptionStateMachine<_>.Run(&sm)
                    sm.ToValueOption()))
        else
            let mutable sm = OptionStateMachine<'T>()
            __expand_code.Invoke(&sm)
            sm.ToValueOption()

let option = OptionBuilder()
let voption = ValueOptionBuilder()

module Examples =

    let t1 () = 
        option {
           printfn "in t1"
           return "a"
        }

    let t2 () = 
        option {
           printfn "in t2"
           let! x = t1 ()
           return "f"
        }
    printfn "t1() = %A" (t1())
    printfn "t2() = %A" (t2())

module Perf = 
    type SlowOptionBuilder() =
        member __.Zero() = None

        member __.Return(x: 'T) = Some x

        member __.ReturnFrom(m: 'T option) = m

        member __.Bind(m: 'T option, f) = Option.bind f m

        member __.Delay(f: unit -> _) = f

        member __.Run(f) = f()

        member this.TryWith(delayedExpr, handler) =
            try this.Run(delayedExpr)
            with exn -> handler exn

        member this.TryFinally(delayedExpr, compensation) =
            try this.Run(delayedExpr)
            finally compensation()

        member this.Using(resource:#IDisposable, body) =
            this.TryFinally(this.Delay(fun ()->body resource), fun () -> match box resource with null -> () | _ -> resource.Dispose())

    let optionSlow = SlowOptionBuilder()

    let perf1 () = 
        let mutable res = 0
        for i in 1 .. 1000000 do
            let v = 
                option {
                   try 
                      let! x1 = (if i % 5 <> 2 then Some i else None)
                      let! x2 = (if i % 3 <> 1 then Some i else None)
                      res <- res + 1 
                      return x1 + x2
                   with e -> 
                      return failwith "unexpected"
                } 
            v |> ignore
        res

    let perf2 () = 
        let mutable res = 0
        for i in 1 .. 1000000 do
            let v = 
                voption {
                   try 
                       let! x1 = (if i % 5 <> 2 then ValueSome i else ValueNone)
                       let! x2 = (if i % 3 <> 1 then ValueSome i else ValueNone)
                       res <- res + 1 
                       return x1 + x2
                   with e -> 
                      return failwith "unexpected"
                } 
            v |> ignore
        res

    let perf3 () = 
        let mutable res = 0
        for i in 1 .. 1000000 do
            let v = 
                optionSlow {
                   try 
                       let! x1 = (if i % 5 <> 2 then Some i else None)
                       let! x2 = (if i % 3 <> 1 then Some i else None)
                       res <- res + 1 
                       return x1 + x2
                   with e -> 
                      return failwith "unexpected"
                } 
            v |> ignore
        res

    let perf4 () = 
        let mutable res = 0
        for i in 1 .. 1000000 do
            let v = 
                try 
                    match (if i % 5 <> 2 then Some i else None) with
                    | None -> None
                    | Some x1 -> 
                    match (if i % 3 <> 1 then Some i else None) with
                    | None -> None
                    | Some x2 -> 
                    res <- res + 1 
                    Some (x1 + x2)
                with e -> 
                    failwith "unexpected"
            v |> ignore
        res

    let perf5 () = 
        let mutable res = 0
        for i in 1 .. 1000000 do
            let v = 
                try 
                    match (if i % 5 <> 2 then ValueSome i else ValueNone) with
                    | ValueNone -> ValueNone
                    | ValueSome x1 -> 
                    match (if i % 3 <> 1 then ValueSome i else ValueNone) with
                    | ValueNone -> ValueNone
                    | ValueSome x2 -> 
                    res <- res + 1 
                    ValueSome (x1 + x2)
                with e -> 
                    failwith "unexpected"
            v |> ignore
        res

    let perf s f = 
        let t = System.Diagnostics.Stopwatch()
        t.Start()
        for i in 1 .. 100 do 
            f() |> ignore
        t.Stop()
        printfn "PERF: %s : %d" s t.ElapsedMilliseconds

    printfn "check %d = %d = %d = %d = %d" (perf1 ()) (perf2()) (perf3()) (perf4()) (perf5())

    perf "perf (builder option)" perf1 
    perf "perf (builder voption)" perf2 
    perf "perf (slow builder option)" perf3
    perf "perf (hand coded option)" perf4
    perf "perf (hand coded voption)" perf5
