
module Tests.SyncBuilder

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AbstractClass>]
type SyncMachine<'T>() =
    
    abstract Step : unit -> 'T

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member this.Start() = this.Step()

type SyncCode<'T> = unit -> 'T

type SyncBuilder() =
    
    member inline _.Delay([<ResumableCode>] __expand_f: unit -> SyncCode<'T>) : [<ResumableCode>]  SyncCode<'T> =
        (fun () -> __expand_f () ())

    member inline _.Run([<ResumableCode>] __expand_code : SyncCode<'T>) : 'T = 
        if __useResumableStateMachines then
            { new SyncMachine<'T>() with 
                [<ResumableCode>]
                member _.Step ()  = __expand_code () }.Start()
        else
            let sm = 
                { new SyncMachine<'T>() with 
                    member _.Step () = __expand_code () }
            sm.Start()

    member inline _.Zero() : [<ResumableCode>] SyncCode<unit> = 
        (fun () -> ())

    member inline _.Return (x: 'T) : [<ResumableCode>] SyncCode<'T> =
        (fun () -> x)

    member inline _.Combine([<ResumableCode>] __expand_code1: SyncCode<unit>, [<ResumableCode>] __expand_code2: SyncCode<'T>) : [<ResumableCode>] SyncCode<'T> =
        (fun () -> 
            __expand_code1()
            __expand_code2())

    member inline _.While([<ResumableCode>] __expand_condition : unit -> bool, [<ResumableCode>] __expand_body : SyncCode<unit>) : [<ResumableCode>] SyncCode<unit> =
        (fun () -> 
            while __expand_condition() do
                __expand_body ())

    member inline _.TryWith([<ResumableCode>] __expand_body : SyncCode<'T>, [<ResumableCode>] __expand_catch : exn -> 'T) : [<ResumableCode>] SyncCode<'T> =
        (fun () -> 
            try
                __expand_body ()
            with exn -> 
                __expand_catch exn)

    member inline _.TryFinally([<ResumableCode>] __expand_body: SyncCode<'T>, compensation : unit -> unit) : [<ResumableCode>] SyncCode<'T> =
        (fun () -> 
            let __stack_step = 
                try
                    __expand_body ()
                with _ ->
                    compensation()
                    reraise()
            compensation()
            __stack_step)

    member inline this.Using(disp : #IDisposable, [<ResumableCode>] __expand_body : #IDisposable -> SyncCode<'T>) : [<ResumableCode>] SyncCode<'T> = 
        this.TryFinally(
            (fun () -> __expand_body disp ()),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, [<ResumableCode>] __expand_body : 'T -> SyncCode<unit>) : [<ResumableCode>] SyncCode<unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), (fun () -> __expand_body e.Current ()))))

    member inline _.ReturnFrom (value: 'T) : [<ResumableCode>] SyncCode<'T> =
        (fun () -> 
              value)

(*
    [<NoDynamicInvocation>]
    member inline _.Bind (__expand_code1: SyncCode<'TResult1>, __expand_continuation: 'TResult1 -> SyncCode<'TResult2>) : SyncCode<'TResult2> =
        (fun () -> 
             let __stack_step = __expand_code1 ()
             __expand_continuation __stack_step ())
*)
    member inline _.Bind (v: 'TResult1, [<ResumableCode>] __expand_continuation: 'TResult1 -> SyncCode<'TResult2>) : [<ResumableCode>] SyncCode<'TResult2> =
        (fun () -> 
             __expand_continuation v ())

let sync = SyncBuilder()

module Examples =

     let t1 y = 
         sync {
            let x = 4 + 5 + y
            return x
         }

     let t2 y = 
         sync {
            printfn "in t2"
            let! x = t1 y
            return x + y
         }


     //printfn "t2 6 = %d" (t2 6)
