
module Tests.SyncBuilder

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<Struct; NoEquality; NoComparison>]
type SyncMachine<'T> =
    
    [<DefaultValue(false)>]
    val mutable Result : 'T

    interface IAsyncStateMachine with 
        member _.MoveNext() = failwith "no dynamic impl"
        member _.SetStateMachine(_state: IAsyncStateMachine) = failwith "no dynamic impl"

    static member inline Run(sm: byref<'K> when 'K :> IAsyncStateMachine) = sm.MoveNext()

[<ResumableCode>]
type SyncCode<'TOverall,'T> = delegate of byref<SyncMachine<'TOverall>> -> 'T

type SyncBuilder() =
    
    member inline _.Delay(f: unit -> SyncCode<'TOverall,'T>) :  SyncCode<'TOverall,'T> =
        SyncCode<_,_>(fun sm -> f().Invoke(&sm))

    member inline _.Run(code : SyncCode<'T,'T>) : 'T = 
        if __useResumableCode then
            __structStateMachine<SyncMachine<'T>, _>
                (MoveNextMethod<_>(fun sm -> 
                       let __stack_result = code.Invoke(&sm)
                       sm.Result <- __stack_result
                       ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    SyncMachine<_>.Run(&sm)
                    sm.Result))
        else
            let mutable sm = SyncMachine<'T>()
            code.Invoke(&sm)

    [<DefaultValue>]
    member inline _.Zero() : SyncCode<'TOverall, unit> = 
        SyncCode<_,_>(fun sm -> ())

    member inline _.Return (x: 'T) : SyncCode<'T,'T> =
        SyncCode<_,_>(fun sm -> x)

    member inline _.Combine(code1: SyncCode<'TOverall,unit>, code2: SyncCode<'TOverall,'T>) : SyncCode<'TOverall,'T> =
        SyncCode<_,_>(fun sm -> 
            code1.Invoke(&sm)
            code2.Invoke(&sm))

    member inline _.While(condition : unit -> bool, body : SyncCode<'TOverall,unit>) : SyncCode<'TOverall,unit> =
       SyncCode<_,_> (fun sm -> 
            while condition() do
                body.Invoke(&sm))

    member inline _.TryWith(body : SyncCode<'TOverall,'T>, catch : exn -> 'T) : SyncCode<'TOverall,'T> =
        SyncCode<_,_>(fun sm -> 
            try
                body.Invoke(&sm)
            with exn -> 
                catch exn)

    member inline _.TryFinally(body: SyncCode<'TOverall,'T>, compensation : unit -> unit) : SyncCode<'TOverall,'T> =
        SyncCode<_,_>(fun sm -> 
            let __stack_step = 
                try
                    body.Invoke(&sm)
                with _ ->
                    compensation()
                    reraise()
            compensation()
            __stack_step)

    member inline this.Using(disp : #IDisposable, body : #IDisposable -> SyncCode<'TOverall,'T>) : SyncCode<'TOverall,'T> = 
        this.TryFinally(
            SyncCode<_,_>(fun sm -> (body disp).Invoke(&sm)),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline this.For(sequence : seq<'T>, body : 'T -> SyncCode<'TOverall,unit>) : SyncCode<'TOverall,unit> =
        this.Using (sequence.GetEnumerator(), 
            (fun e -> this.While((fun () -> e.MoveNext()), SyncCode<_,_>(fun sm -> (body e.Current).Invoke(&sm)))))

    member inline _.ReturnFrom (value: 'T) : SyncCode<'T,'T> =
        SyncCode<_,_>(fun sm -> 
              value)

    member inline _.Bind (v: 'TResult1, continuation: 'TResult1 -> SyncCode<'TOverall,'TResult2>) : SyncCode<'TOverall,'TResult2> =
        SyncCode<_,_>(fun sm -> 
             (continuation v).Invoke(&sm))

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
