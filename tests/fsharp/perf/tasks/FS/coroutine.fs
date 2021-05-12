
// Coroutines with tailcall support
module Tests.Coroutines

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open FSharp.Core.LanguagePrimitives.IntrinsicOperators
open FSharp.Collections

let verbose = true 

// Call interface methods on structs
let inline MoveNext(x: byref<'T> when 'T :> IAsyncStateMachine) = x.MoveNext()
let inline SetStateMachine(x: byref<'T> when 'T :> IAsyncStateMachine, state) = x.SetStateMachine(state)
let inline GetResumptionPoint(x: byref<'T> when 'T :> IResumableStateMachine<'Data>) = x.ResumptionPoint
let inline SetResumptionFunc (sm: byref<ResumableStateMachine<'Data>>) f =
    let (_, e) = sm.ResumptionFuncData
    sm.ResumptionFuncData <- (f, e)

let inline GetResumptionFunc (sm: byref<ResumableStateMachine<'Data>>) =
    let (f, _) = sm.ResumptionFuncData
    f


/// The extra data stored in ResumableStateMachine for coroutines
[<Struct; NoComparison; NoEquality>]
type CoroutineStateMachineData =

    // For tailcalls using 'return!'
    [<DefaultValue(false)>]
    val mutable HijackTarget: Coroutine option

    static member GetHijackTarget(x: byref<'T> when 'T :> IResumableStateMachine<CoroutineStateMachineData>) = 
        x.Data.HijackTarget
    static member SetHijackTarget(x: byref<'T>, tg: Coroutine) : unit when 'T :> IResumableStateMachine<CoroutineStateMachineData> = 
        let mutable newData = CoroutineStateMachineData()
        newData.HijackTarget <- Some tg
        x.Data <- newData

and CoroutineStateMachine = ResumableStateMachine<CoroutineStateMachineData>
and CoroutineResumption = ResumptionFunc<CoroutineStateMachineData>
and CoroutineResumptionExecutor = ResumptionFuncExecutor<CoroutineStateMachineData>

and CoroutineCode = ResumableCode<CoroutineStateMachineData, unit>

and [<AbstractClass; NoEquality; NoComparison>] 
    Coroutine() =
    static let mutable x = 1000
    //do x <- x + 1
    //let id = x
    ////do printfn $"[{id}] created"
    //member _.Id = id
    abstract IsCompleted: bool
    abstract MoveNext: unit -> unit
    abstract HijackTarget: Coroutine option
    
and [<NoEquality; NoComparison>] 
    Coroutine<'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> IResumableStateMachine<CoroutineStateMachineData>>() =
    inherit Coroutine()

    [<DefaultValue(false)>]
    val mutable Machine: 'Machine

    override cr.IsCompleted =
        match cr.HijackTarget with 
        | None -> //if verbose then printfn $"[{cr.Id}] move"
            GetResumptionPoint(&cr.Machine) = -1
        | Some tg -> 
            tg.IsCompleted

    override cr.HijackTarget = 
        CoroutineStateMachineData.GetHijackTarget(&cr.Machine)

    override cr.MoveNext() = 
        match cr.HijackTarget with 
        | None -> //if verbose then printfn $"[{cr.Id}] move"
            MoveNext(&cr.Machine)
        | Some tg -> 
            match tg.HijackTarget with 
            | None -> tg.MoveNext()
            | Some tg2 -> 
                // Cut out chains of tailcalls
                CoroutineStateMachineData.SetHijackTarget(&cr.Machine, tg2)
                tg2.MoveNext()

type CoroutineBuilder() =
    
    member inline _.Delay(f : unit -> CoroutineCode) : CoroutineCode = ResumableCode.Delay(f)

    member inline _.Run(code : CoroutineCode) : Coroutine = 
        if __useResumableCode then 
            __structStateMachine<CoroutineStateMachine, Coroutine>
                // IAsyncStateMachine.MoveNext
                (MoveNextMethodImpl<CoroutineStateMachine>(fun sm -> 
                    if __useResumableCode then 
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint 
                        //if verbose then printfn $"[{sm.Id}] Run: resumable code, sm.ResumptionPoint = {sm.ResumptionPoint}"
                        let __stack_code_fin = code.Invoke(&sm)
                        if __stack_code_fin then
                            //if verbose then printfn $"[{sm.Id}] terminate"
                            sm.ResumptionPoint  <- -1
                        //if verbose then printfn $"[{sm.Id}] done MoveNext, sm.ResumptionPoint = {sm.ResumptionPoint}"
                        //-- RESUMABLE CODE END
                    else
                        failwith "Run: non-resumable - unreachable"))

                // IAsyncStateMachine.SetStateMachine
                (SetStateMachineMethodImpl<CoroutineStateMachine>(fun sm state -> 
                    SetStateMachine(&sm, state)))

                // Other interfaces (IResumableStateMachine)
                [| 
                   (typeof<IResumableStateMachine<CoroutineStateMachineData>>, "get_ResumptionPoint", GetResumptionPointMethodImpl<CoroutineStateMachine>(fun sm -> 
                        sm.ResumptionPoint) :> _);
                   (typeof<IResumableStateMachine<CoroutineStateMachineData>>, "get_Data", GetResumableStateMachineDataMethodImpl<CoroutineStateMachine, CoroutineStateMachineData>(fun sm -> 
                        sm.Data) :> _);
                   (typeof<IResumableStateMachine<CoroutineStateMachineData>>, "set_Data", SetResumableStateMachineDataMethodImpl<CoroutineStateMachine, CoroutineStateMachineData>(fun sm data -> 
                        sm.Data <- data) :> _);
                 |]

                // Start
                (AfterCode<CoroutineStateMachine,_>(fun sm -> 
                    let mutable cr = Coroutine<CoroutineStateMachine>()
                    cr.Machine <- sm
                    //cr.Machine.Id <- cr.Id
                    //if verbose then printfn $"[{cr.Id}] static create"
                    cr :> Coroutine))
        else 
            let mutable cr = Coroutine<CoroutineStateMachine>()
            let initialResumptionFunc = CoroutineResumption(fun sm -> code.Invoke(&sm))
            let resumptionFuncExecutor = 
                CoroutineResumptionExecutor(fun sm f -> 
                    if f.Invoke(&sm) then
                        sm.ResumptionPoint <- -1)
            cr.Machine.ResumptionFuncData <- (initialResumptionFunc, resumptionFuncExecutor)
            //cr.Machine.Id <- cr.Id
            //if verbose then printfn $"[{cr.Id}] dynamic create"
            cr :> Coroutine

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : CoroutineCode = ResumableCode.Zero()

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine(code1: CoroutineCode, code2: CoroutineCode) : CoroutineCode =
        ResumableCode.Combine(code1, code2)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While ([<InlineIfLambda>] condition : unit -> bool, body : CoroutineCode) : CoroutineCode =
        ResumableCode.While(condition, body)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith (body: CoroutineCode, catch: exn -> CoroutineCode) : CoroutineCode =
        ResumableCode.TryWith(body, catch)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally (body: CoroutineCode, [<InlineIfLambda>] compensation : unit -> unit) : CoroutineCode =
        ResumableCode.TryFinally(body, NonResumableCode<_,_>(fun _ -> compensation()))

    member inline _.Using (resource : 'Resource, body : 'Resource -> CoroutineCode) : CoroutineCode when 'Resource :> IDisposable = 
        ResumableCode.Using(resource, body)

    member inline _.For (sequence : seq<'T>, body : 'T -> CoroutineCode) : CoroutineCode =
        ResumableCode.For(sequence, body)

    member inline _.Yield (_dummy: unit) : CoroutineCode = 
        ResumableCode.Yield()

    member inline _.YieldFrom (other: Coroutine) : CoroutineCode = 
        ResumableCode.While((fun () -> not other.IsCompleted), CoroutineCode(fun sm -> 
            //if verbose then printfn $"[{sm.Id}] calling [{other.Id}].MoveNext, it will resume at {other.ResumptionPoint}";
            other.MoveNext()
            let __stack_other_fin = other.IsCompleted
            if not __stack_other_fin then
                // This will yield with __stack_yield_fin = false
                // This will resume with __stack_yield_fin = true
                let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                __stack_yield_fin
            else
               true))

    member inline _.ReturnFrom (other: Coroutine) : CoroutineCode = 
        ResumableCode<_,_>(fun sm -> 
            sm.Data.HijackTarget <- Some other
            // We return 'false' and re-run from the entry (trampoline)
            false 
            // We could do this immediately with future cut-out, though this will stack-dive on sync code.
            // We could also trampoline less frequently via a counter
            // b.YieldFrom(other).Invoke(&sm)
            )

[<AutoOpen>]
module CoroutineBuilder = 

    let coroutine = CoroutineBuilder()


module Examples =

    let t1 () = 
        coroutine {
           printfn "in t1"
           yield ()
           //let x = 1
           printfn "hey"
           yield ()
           yield! 
               coroutine{ 
                   printfn "hey yo"
                   yield ()
                   printfn "hey go"

               }
           //yied! t1()
           //yield ()
        }

    let testTailcallTiny () = 
        coroutine {
           return! t1()
        }
    let rec testTailcall (n: int) = 
        coroutine {
           if n % 100 = 0 then printfn $"in t1, n = {n}"
           yield ()
           if n > 0 then
               return! testTailcall(n-1)
           //yield ()
        }


    //let t2 () = 
    //    coroutine {
    //       printfn "in t2"
    //       yield ()
    //       printfn "in t2 b"
    //       yield! t1()
    //       //for x in t1 () do 
    //       //    printfn "t2 - got %A" x
    //       //    yield ()
    //       //    yield! 
    //       //        coroutine {
    //       //            printfn "hey yo"
    //       //        }
    //       //    yield "[T1]" + x
    //       yield!
    //           coroutine {
    //               printfn "hey yo"
    //               //do! Task.Delay(10)
    //           }
    //       yield ()
    //    }

    //let perf1 (x: int) = 
    //    coroutine {
    //       yield ()
    //       yield ()
    //       if x >= 2 then 
    //           yield ()
    //           yield ()
    //    }

    //let perf2 () = 
    //    coroutine {
    //       for i1 in perf1 3 do
    //         for i2 in perf1 3 do
    //           for i3 in perf1 3 do
    //             for i4 in perf1 3 do
    //               for i5 in perf1 3 do
    //                  yield! perf1 i5
    //    }

    //let perf1_AsyncSeq (x: int) = 
    //    FSharp.Control.AsyncSeqExtensions.asyncSeq {
    //       yield 1
    //       yield 2
    //       if x >= 2 then 
    //           yield 3
    //           yield 4
    //    }

    //let perf2_AsyncSeq () = 
    //    FSharp.Control.AsyncSeqExtensions.asyncSeq {
    //       for i1 in perf1_AsyncSeq 3 do
    //         for i2 in perf1_AsyncSeq 3 do
    //           for i3 in perf1_AsyncSeq 3 do
    //             for i4 in perf1_AsyncSeq 3 do
    //               for i5 in perf1_AsyncSeq 3 do
    //                 yield! perf1_AsyncSeq i5
    //    }

    let dumpCoroutine (t: Coroutine) = 
        printfn "-----"
        while ( //if verbose then printfn $"[{t.Id}] calling t.MoveNext, will resume at {t.ResumptionPoint}"; 
                t.MoveNext()
                not t.IsCompleted) do 
            () // printfn "yield"

    //dumpCoroutine (t1())
    //dumpCoroutine (testTailcallTiny())
    dumpCoroutine (testTailcall(1000000))
    //dumpCoroutine (t2())

    //if verbose then printfn "t1() = %A" (TaskSeq.toArray (t1()))
    //if verbose then printfn "t2() = %A" (TaskSeq.toArray (t2()))

    //if verbose then printfn "perf2() = %A" (TaskSeq.toArray (perf2()))


