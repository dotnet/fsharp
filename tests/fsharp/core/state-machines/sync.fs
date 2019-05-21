
module Tests.SyncBuilder

open System
open FSharp.Core.CompilerServices.CodeGenHelpers

[<AbstractClass>]
type SyncMachine<'T>() =
    
    abstract Step : unit -> 'T

    member this.Start() = this.Step()

type SyncBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> 'T) = __expand_f

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : unit -> 'T) : 'T = 
        (__stateMachine
            { new SyncMachine<'T>() with 
                member __.Step ()  = __jumptable 0 __expand_code }).Start()

    [<NoDynamicInvocation>]
    member inline __.Zero() : unit = ()

    [<NoDynamicInvocation>]
    member inline __.Return (x: 'T) : 'T = x

    [<NoDynamicInvocation>]
    member inline __.Combine(``__machine_step$cont``: unit, __expand_step2: unit -> 'T) : 'T =
        __expand_step2()

    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : unit -> unit) : unit =
        while __expand_condition() do
            __expand_body ()

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : unit -> 'T, __expand_catch : exn -> 'T) : 'T =
        try
            __expand_body ()
        with exn -> 
            __expand_catch exn

    [<NoDynamicInvocation>]
    member inline __.TryFinally(__expand_body: unit -> 'T, compensation : unit -> unit) : 'T =
        let ``__machine_step$cont`` = 
            try
                __expand_body ()
            with _ ->
                compensation()
                reraise()
        compensation()
        ``__machine_step$cont``

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> 'T) = 
        this.TryFinally(
            (fun () -> __expand_body disp),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline this.For(sequence : seq<'T>, __expand_body : 'T -> unit) : unit =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current))))

    [<NoDynamicInvocation>]
    member inline __.ReturnFrom (value: 'T) : 'T =
         value

    [<NoDynamicInvocation>]
    member inline __.Bind (value: 'TResult1, __expand_continuation: 'TResult1 -> 'TResult2) =
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
