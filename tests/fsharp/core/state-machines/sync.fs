
module Sync

open System
open FSharp.Core.CompilerServices.CodeGenHelpers

type SyncStep<'T> = 'T

[<AbstractClass>]
type SyncMachine<'T>() =
    
    abstract Step : unit -> SyncStep<'T>

    member this.Start() = this.Step()

type SyncBuilder() =
    
    member inline __.Delay(__expand_f : unit -> SyncStep<'T>) = __expand_f

    member inline __.Run(__expand_code : unit -> SyncStep<'T>) : 'T = 
        (__stateMachine
            { new SyncMachine<'T>() with 
                member __.Step ()  = __jumptable 0 __expand_code }).Start()

    member inline __.Zero() : SyncStep<unit> = ()

    member inline __.Return (x: 'T) : SyncStep<'T> = x

    member inline __.Combine(``__machine_step$cont``: SyncStep<unit>, __expand_step2: unit -> SyncStep<'T>) : SyncStep<'T> =
        __expand_step2()

    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> SyncStep<unit>) : SyncStep<unit> =
        while __expand_condition() do
            __expand_body ()

    member inline __.TryWith(__expand_body : unit -> SyncStep<'T>, __expand_catch : exn -> SyncStep<'T>) : SyncStep<'T> =
        try
            __expand_body ()
        with exn -> 
            __expand_catch exn

    member inline __.TryFinally(__expand_body: unit -> SyncStep<'T>, compensation : unit -> unit) : SyncStep<'T> =
        let ``__machine_step$cont`` = 
            try
                __expand_body ()
            with _ ->
                compensation()
                reraise()
        compensation()
        ``__machine_step$cont``

    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> SyncStep<'T>) = 
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, __expand_body : 'T -> SyncStep<unit>) : SyncStep<unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    member inline __.ReturnFrom (value: 'T) : SyncStep<'T> =
         value

    member inline __.Bind (value: 'TResult1, __expand_continuation: ^TResult1 -> SyncStep<'TResult2>) =
         __expand_continuation value

let sync = SyncBuilder()

module Examples =

    let t1 y = 
        sync {
           printfn "in t1"
           let x = 4 + 5 + y
           return x
        }

    let t2 y = 
        sync {
           printfn "in t2"
           let! x = t1 y
           return x + y
        }


    printfn "t2 6 = %d" (t2 6)
