
module Tests.Value2

#nowarn "42"
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers

[<AbstractClass>]
type StateMachine<'T>() =
    abstract Step : unit -> bool

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    member sm.Start() : StateMachine<'T> = Unchecked.defaultof<_>

type ValueCode = unit -> bool

type ValueBuilder() =
    
    member inline __.Run() : StateMachine<'T> = 
        if __useResumableStateMachines then
            (__resumableStateMachine
                { new StateMachine<'T>() with 
                    member sm.Step () =
                        //__resumeAt sm.ResumptionPoint
                        true }).Start()
        else
            let sm = 
                { new StateMachine<'T>() with 
                    member sm.Step () = 
                        true }
            sm.Start()

    // member inline __.Zero() : ValueCode<'T> =
    //     (fun _sm -> true)

    // member inline __.Combine(__expand_task1: ValueCode<'T>, __expand_task2: ValueCode<'T>) : ValueCode<'T> =
    //     (fun sm -> 
    //         if __useResumableStateMachines then
    //             let __stack_step = __expand_task1 sm
    //             if __stack_step then 
    //                 __expand_task2 sm
    //             else
    //                 false
    //         else
    //             failwith "no dynamic implementation")

    // member inline __.While(__expand_condition : unit -> bool, __expand_body : ValueCode<'T>) : ValueCode<'T> =
    //     (fun sm -> 
    //         if __useResumableStateMachines then
    //             let mutable __stack_completed = false 
    //             while __stack_completed && __expand_condition() do
    //                 // NOTE: The body of the 'while' may contain await points, resuming may branch directly into the while loop
    //                 let __stack_step = __expand_body sm
    //                 // If we make it to the assignment we prove we've made a step 
    //                 __stack_completed <- __stack_step
    //             __stack_completed
    //         else
    //             failwith "no dynamic implementation")

    // member inline __.TryWith(__expand_body : ValueCode<'T>, __expand_catch : exn -> ValueCode<'T>) : ValueCode<'T> =
    //     (fun sm -> 
    //         if __useResumableStateMachines then
    //             let mutable __stack_completed = false
    //             let mutable __stack_caught = false
    //             let mutable __stack_savedExn = Unchecked.defaultof<_>
    //             try
    //                 // The try block may contain await points.
    //                 let __stack_step = __expand_body sm
    //                 // If we make it to the assignment we prove we've made a step
    //                 __stack_completed <- __stack_step
    //             with exn -> 
    //                 __stack_caught <- true
    //                 __stack_savedExn <- exn

    //             if __stack_caught then 
    //                 // Place the catch code outside the catch block 
    //                 __expand_catch __stack_savedExn sm
    //             else
    //                 __stack_completed
    //         else
    //             failwith "no dynamic implementation")

    // member inline __.TryFinally(__expand_body: ValueCode<'T>, compensation : unit -> unit) : ValueCode<'T> =
    //     (fun sm -> 
    //         if __useResumableStateMachines then
    //             let mutable completed = false
    //             sm.PushDispose compensation
    //             try
    //                 let __stack_step = __expand_body sm
    //                 // If we make it to the assignment we prove we've made a step without an exception
    //                 completed <- __stack_step
    //             with _ ->
    //                 sm.PopDispose()
    //                 compensation()
    //                 reraise()

    //             if completed then 
    //                 sm.PopDispose()
    //                 compensation()
    //             completed
    //         else
    //             failwith "no dynamic implementation")

    // member inline this.Using(disp : #IDisposable, __expand_body : #IDisposable -> ValueCode<'T>) = 
    //     // A using statement is just a try/finally with the finally block disposing if non-null.
    //     this.TryFinally(
    //         (fun sm -> __expand_body disp sm),
    //         (fun () -> if not (isNull (box disp)) then disp.Dispose()))

    member inline __.Return (v: 'T) : ValueCode =
        (fun sm ->
            //if __useResumableStateMachines then
                // match __resumableEntry() with
                // | Some contID ->
                //     sm.ResumptionPoint <- contID
                //     sm.Current <- ValueSome v
                //     false
                // | None -> 
                //     sm.Current <- ValueNone
                    true
            //else
            //    failwith "no dynamic implementation"
          )

    //[<NoDynamicInvocation>]
    //member inline this.ReturnFrom (source: IValuable<'T>) : ValueCode<'T> =
    //    this.For(source, (fun v -> this.Yield v))

let value = ValueBuilder()

module Examples =

    let t1 () = 
        value.Run()

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

    //let dumpSeq (t: IValuable<_>) = 
    //    let e = t.GetValuator()
    //    while e.MoveNext() do 
    //        printfn "return %A" e.Current
    //dumpSeq (t1())
    //dumpSeq (t2())
