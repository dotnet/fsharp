
module Tests.Value2

#nowarn "42"
open System
open System.Collections
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

type IValuator<'T> =
   inherit IDisposable
   abstract MoveNext : unit -> bool
   abstract Current : 'T
   
type IValuable<'T> =
   abstract GetValuator: unit -> IValuator<'T>

[<AbstractClass>]
type ValueStateMachine<'T>() =
    let disposalStack = ResizeArray<(unit -> unit)>()

    /// Proceed to the next state or raise an exception. Returns true if completed
    abstract Step : unit -> bool

    member val Current : 'T voption = ValueNone with get, set

    member val ResumptionPoint: int = 0 with get, set

    interface IDisposable with
        member __.Dispose() = 
            let mutable exn = None
            for d in Seq.rev disposalStack do 
                try 
                    d()
                with e ->
                    exn <- Some e // keep the last exception - TODO - check this
            match exn with 
            | None -> () 
            | Some e -> raise e 

    member __.PushDispose (f: unit -> unit) = disposalStack.Add(f)

    member __.PopDispose () = disposalStack.RemoveAt(disposalStack.Count - 1)
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.Start() = (sm :> IValuable<'T>)

    interface IValuable<'T> with
        member this.GetValuator() = 
            (this :> IValuator<'T>)

    interface IValuator<'T> with
        member sm.Current = match sm.Current with ValueNone -> failwith "no value available yet" | ValueSome x -> x
        member sm.MoveNext() = sm.Step()


type ValueCode<'T> = ValueStateMachine<'T> -> bool

type ValueBuilder() =
    
    [<NoDynamicInvocation>]
    member inline __.Delay(__expand_f : unit -> ValueCode<'T>) : ValueCode<'T> = (fun sm -> __expand_f () sm)

    [<NoDynamicInvocation>]
    member inline __.Run(__expand_code : ValueCode<'T>) : IValuable<'T> = 
        if __useResumableCode then
            (__resumableObject
                { new ValueStateMachine<'T>() with 
                    member sm.Step () =
                        __resumeAt sm.ResumptionPoint
                        __expand_code sm }).Start()
        else
            failwith "no dynamic implementation"

    [<NoDynamicInvocation>]
    member inline __.Zero() : ValueCode<'T> =
        (fun _sm -> true)

    [<NoDynamicInvocation>]
    member inline __.Combine(__expand_task1: ValueCode<'T>, __expand_task2: ValueCode<'T>) : ValueCode<'T> =
        (fun sm -> 
            if __useResumableCode then
                let ``__machine_step$cont`` = __expand_task1 sm
                if ``__machine_step$cont`` then 
                    __expand_task2 sm
                else
                    false
            else
                failwith "no dynamic implementation")
            
    [<NoDynamicInvocation>]
    member inline __.While(__expand_condition : unit -> bool, __expand_body : ValueCode<'T>) : ValueCode<'T> =
        (fun sm -> 
            if __useResumableCode then
                let mutable __stack_completed = false 
                while __stack_completed && __expand_condition() do
                    // NOTE: The body of the 'while' may contain await points, resuming may branch directly into the while loop
                    let ``__machine_step$cont`` = __expand_body sm
                    // If we make it to the assignment we prove we've made a step 
                    __stack_completed <- ``__machine_step$cont``
                __stack_completed
            else
                failwith "no dynamic implementation")

    [<NoDynamicInvocation>]
    member inline __.TryWith(__expand_body : ValueCode<'T>, __expand_catch : exn -> ValueCode<'T>) : ValueCode<'T> =
        (fun sm -> 
            if __useResumableCode then
                let mutable __stack_completed = false
                let mutable __stack_caught = false
                let mutable __stack_savedExn = Unchecked.defaultof<_>
                try
                    // The try block may contain await points.
                    let ``__machine_step$cont`` = __expand_body sm
                    // If we make it to the assignment we prove we've made a step
                    __stack_completed <- ``__machine_step$cont``
                with exn -> 
                    __stack_caught <- true
                    __stack_savedExn <- exn

                if __stack_caught then 
                    // Place the catch code outside the catch block 
                    __expand_catch __stack_savedExn sm
                else
                    __stack_completed
            else
                failwith "no dynamic implementation")

    [<NoDynamicInvocation>]
    member inline __.TryFinally(__expand_body: ValueCode<'T>, compensation : unit -> unit) : ValueCode<'T> =
        (fun sm -> 
            if __useResumableCode then
                let mutable completed = false
                sm.PushDispose compensation
                try
                    let ``__machine_step$cont`` = __expand_body sm
                    // If we make it to the assignment we prove we've made a step without an exception
                    completed <- ``__machine_step$cont``
                with _ ->
                    sm.PopDispose()
                    compensation()
                    reraise()

                if completed then 
                    sm.PopDispose()
                    compensation()
                completed
            else
                failwith "no dynamic implementation")

    [<NoDynamicInvocation>]
    member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> ValueCode<'T>) = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        this.TryFinally(
            (fun sm -> __expand_body disp sm),
            (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    [<NoDynamicInvocation>]
    member inline __.Return (v: 'T) : ValueCode<'T> =
        (fun sm ->
            if __useResumableCode then
                match __resumableEntry() with
                | Some contID ->
                    sm.ResumptionPoint <- contID
                    sm.Current <- ValueSome v
                    false
                | None -> 
                    sm.Current <- ValueNone
                    true
            else
                failwith "no dynamic implementation")

    //[<NoDynamicInvocation>]
    //member inline this.ReturnFrom (source: IValuable<'T>) : ValueCode<'T> =
    //    this.For(source, (fun v -> this.Yield v))

let value = ValueBuilder()

module Examples =

    let t1 () = 
        value {
           printfn "in t1"
           return "a"
           let x = 1
           return "b"
           return "c"
        }

(*
    let t2 () = 
        value {
           printfn "in t2"
           return "d"
           for x in t1 () do 
               printfn "t2 - got %A" x
               return "e"
               return "[T1]" + x
           return "f"
        }
*)

    let dumpSeq (t: IValuable<_>) = 
        let e = t.GetValuator()
        while e.MoveNext() do 
            printfn "return %A" e.Current
    dumpSeq (t1())
    //dumpSeq (t2())
