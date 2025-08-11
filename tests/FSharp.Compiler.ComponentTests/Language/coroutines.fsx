

// This is a sample and test showing how to use resumable code to implement
// coroutines with tailcall support
//
// A coroutine is a value of type Coroutine normally constructed using this form:
//
//    coroutine {
//       printfn "in t1"
//       yield ()
//       printfn "hey"
//    }
//

// Support for tailcalls is provided by the mechanism od detecting tail calls in computation expressions.
// in tail call positions, YieldFromFinal is generated for the yield! keyword.

#nowarn 3513
#nowarn 3514

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open FSharp.Core.LanguagePrimitives.IntrinsicOperators
open FSharp.Collections

let mutable yieldFromFinalCallCount = 0
let mutable yieldFromCount = 0

let verbose = false

/// Helpers to do zero-allocation call to interface methods on structs
[<AutoOpen>]
module internal Helpers =
    let inline MoveNext(x: byref<'T> when 'T :> IAsyncStateMachine) = x.MoveNext()
    let inline SetStateMachine(x: byref<'T> when 'T :> IAsyncStateMachine, state) = x.SetStateMachine(state)
    let inline GetResumptionPoint(x: byref<'T> when 'T :> IResumableStateMachine<'Data>) = x.ResumptionPoint

/// This is the type of coroutines
[<AbstractClass; NoEquality; NoComparison>] 
type Coroutine() =
    
    /// Checks if the coroutine is completed
    abstract IsCompleted: bool

    /// Executes the coroutine until the next 'yield'
    abstract MoveNext: unit -> unit

    /// Gets the tailcall target if the coroutine has executed a `yield!` in tailcall position.
    abstract TailcallTarget: Coroutine option
    
/// This is the implementation of Coroutine with respect to a particular struct state machine type.
and [<NoEquality; NoComparison>] 
    Coroutine<'Machine when 'Machine : struct
                        and 'Machine :> IAsyncStateMachine 
                        and 'Machine :> ICoroutineStateMachine>() =
    inherit Coroutine()

    // The state machine struct
    [<DefaultValue(false)>]
    val mutable Machine: 'Machine

    override cr.IsCompleted =
        match cr.TailcallTarget with 
        | None -> 
            GetResumptionPoint(&cr.Machine) = -1
        | Some tg -> 
            tg.IsCompleted

    override cr.TailcallTarget = 
        CoroutineStateMachineData.GetHijackTarget(&cr.Machine)

    override cr.MoveNext() = 
        match cr.TailcallTarget with 
        | None -> //if verbose then printfn $"[{cr.Id}] move"
            MoveNext(&cr.Machine)
        | Some tg -> 
            match tg.TailcallTarget with 
            | None -> tg.MoveNext()
            | Some tg2 -> 
                // Cut out chains of tailcalls
                CoroutineStateMachineData.SetHijackTarget(&cr.Machine, tg2)
                tg2.MoveNext()
/// This extra data stored in ResumableStateMachine (and it's templated copies using __stateMachine) 
/// It only contains one field, the hijack target for tailcalls.
and [<Struct; NoComparison; NoEquality>]
    CoroutineStateMachineData =

    /// This is used for tailcalls using 'yield!'
    [<DefaultValue(false)>]
    val mutable TailcallTarget: Coroutine option

    static member GetHijackTarget(x: byref<'Machine> when 'Machine :> IResumableStateMachine<CoroutineStateMachineData>) = 
        x.Data.TailcallTarget

    static member SetHijackTarget(x: byref<'Machine>, tg: Coroutine) : unit when 'Machine :> IResumableStateMachine<CoroutineStateMachineData> = 
        let mutable newData = CoroutineStateMachineData()
        newData.TailcallTarget <- Some tg
        x.Data <- newData

/// These are standard definitions filling in the 'Data' parameter of each
and ICoroutineStateMachine = IResumableStateMachine<CoroutineStateMachineData>
and CoroutineStateMachine = ResumableStateMachine<CoroutineStateMachineData>
and CoroutineResumptionFunc = ResumptionFunc<CoroutineStateMachineData>
and CoroutineResumptionDynamicInfo = ResumptionDynamicInfo<CoroutineStateMachineData>
and CoroutineCode = ResumableCode<CoroutineStateMachineData, unit>


/// The builder for tailcalls, defined using resumable code combinators
type CoroutineBuilder() =
    
    member inline _.Delay(f : unit -> CoroutineCode) : CoroutineCode = ResumableCode.Delay(f)

    /// Create the state machine and outer execution logic
    member inline _.Run(code : CoroutineCode) : Coroutine = 
        if __useResumableCode then 
            __stateMachine<CoroutineStateMachineData, Coroutine>

                // IAsyncStateMachine.MoveNext
                (MoveNextMethodImpl<_>(fun sm -> 
                        //-- RESUMABLE CODE START
                        __resumeAt sm.ResumptionPoint 
                        let __stack_code_fin = code.Invoke(&sm)
                        if __stack_code_fin then
                            sm.ResumptionPoint  <- -1 // indicates complete
                        else
                            // Goto request
                            match sm.Data.TailcallTarget with 
                            | Some tg -> tg.MoveNext() // recurse
                            | None -> ()
                        //-- RESUMABLE CODE END
                    ))

                // IAsyncStateMachine.SetStateMachine
                (SetStateMachineMethodImpl<_>(fun sm state -> SetStateMachine(&sm, state)))

                // Box the coroutine.  In this example we don't start execution of the coroutine.
                (AfterCode<_,_>(fun sm -> 
                    let mutable cr = Coroutine<CoroutineStateMachine>()
                    cr.Machine <- sm
                    cr :> Coroutine))
        else 
            // The dynamic implementation
            let initialResumptionFunc = CoroutineResumptionFunc(fun sm -> code.Invoke(&sm))
            let resumptionInfo =
                { new CoroutineResumptionDynamicInfo(initialResumptionFunc) with 
                    member info.MoveNext(sm) = 
                        if info.ResumptionFunc.Invoke(&sm) then
                            sm.ResumptionPoint <- -1
                    member info.SetStateMachine(sm, state) = ()
                 }
            let mutable cr = Coroutine<CoroutineStateMachine>()
            cr.Machine.ResumptionDynamicInfo <- resumptionInfo
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
        ResumableCode.TryFinally(body, ResumableCode<_,_>(fun _ -> compensation(); true))

    member inline _.Using (resource : 'Resource, body : 'Resource -> CoroutineCode) : CoroutineCode when 'Resource :> IDisposable = 
        ResumableCode.Using(resource, body)

    member inline _.For (sequence : seq<'T>, body : 'T -> CoroutineCode) : CoroutineCode =
        ResumableCode.For(sequence, body)

    member inline _.Yield (_dummy: unit) : CoroutineCode = 
        ResumableCode.Yield()

    // The implementation of `yield!`
    member inline _.YieldFrom (other: Coroutine) : CoroutineCode = 
        ResumableCode.While((fun () -> not other.IsCompleted), CoroutineCode(fun sm -> 
            other.MoveNext()
            let __stack_other_fin = other.IsCompleted
            if not __stack_other_fin then
                // This will yield with __stack_yield_fin = false
                // This will resume with __stack_yield_fin = true
                let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                __stack_yield_fin
            else
               yieldFromCount <- yieldFromCount + 1
               true))

    // The implementation of `return!`, non-standard for tailcalls
    member inline _.YieldFromFinal (other: Coroutine) : CoroutineCode = 
        ResumableCode<_,_>(fun sm ->
            yieldFromFinalCallCount <- yieldFromFinalCallCount + 1 
            sm.Data.TailcallTarget <- Some other
            // For tailcalls we return 'false' and re-run from the entry (trampoline)
            false 
            // We could do this immediately with future cut-out, though this will stack-dive on sync code.
            // We could also trampoline less frequently via a counter
            // b.YieldFrom(other).Invoke(&sm)
            )

let coroutine = CoroutineBuilder()

let dumpCoroutine (t: Coroutine) =
    yieldFromFinalCallCount <- 0
    yieldFromCount <- 0
    printfn "-----"
    while ( //if verbose then printfn $"[{t.Id}] calling t.MoveNext, will resume at {t.ResumptionPoint}"; 
            t.MoveNext()
            not t.IsCompleted) do 
        () // printfn "yield"
    printfn $"YieldFromFinal called {yieldFromFinalCallCount} times, YieldFrom called {yieldFromCount} times"

let expect final standard t =
    dumpCoroutine t
    if yieldFromFinalCallCount <> final then failwithf "Expected yieldFromFinalCallCount = %d, got %d" final yieldFromFinalCallCount
    if yieldFromCount <> standard then failwithf "Expected yieldFromCount = %d, got %d" standard yieldFromCount

let t0 () = coroutine { printfn "in t0" } 

let t1 () = 
    coroutine {
        printfn "in t1"
        yield ()
        printfn "hey ho"
        yield ()
        printfn "This should YieldFromFinal because of final position"
        yield! 
            coroutine{ 
                printfn "hey yo"
                yield ()
                printfn "hey go"
            }
    }

t1() |> expect 1 0

let testNonTailcall () = 
    coroutine {
        try 
            printfn "this should not be tailcall because of try/finally"
            yield! t1()
        finally ()
    }

testNonTailcall() |> expect 1 1

let testTailcallTiny () = 
    coroutine {
        printfn "in testTailcallTiny"
        yield! t1()
    }

testTailcallTiny() |> expect 2 0

let testTailcallTinyDoBang () = 
    coroutine {
        printfn "in testTailcallTinyDoBang, desugaring do!"
        do! t1() // this should desugr to YieldFromFinal, because ReturnFromFinal is not provided.
    }

testTailcallTinyDoBang() |> expect 2 0

let rec testTailcall (n: int) = 
    coroutine {
        if n % 10_000 = 0 then printfn $"in testTailcall, n = {n}"
        yield ()
        if n > 0 then
            yield! testTailcall(n-1)
    }


// Large number of recursive calls does not blow up the stack
testTailcall(1000000) |> expect 1000000 0

let t2 () = 
    coroutine {
        printfn "in t2"
        yield ()
        printfn "in t2 b"
        yield! t1()
        printfn "This should YieldFrom because of non final position"
        yield!
            coroutine {
                printfn "hey yo"
            }
        yield ()
    }

t2() |> expect 1 2

let t3 () = 
    coroutine {
        printfn "in t3"
        try
            printfn "in t3 b"
            yield! t1()
            for x in 1 .. 3 do 
                printfn "t2 - got %A" x
                yield ()
                yield! 
                    coroutine {
                        printfn "hey yo"
                    }
            yield ()
            printfn "This should YieldFrom because of try/with"
            yield!
                coroutine {
                    printfn "in t3 inner"
                    printfn "hey yo"
                }
        with _ -> ()
    }

t3() |> expect 1 5

let negativeTest () = 
    coroutine {
        printfn "in negativeTest"
        try 
            yield! t0()
        finally ()

        try
            yield! t0 ()
            failwith "crash"
        with
        | _ ->
            yield! t0 ()
            printfn "in handler"

        let mutable x = 0

        while (x <- x + 1; x < 5) do
            yield! t0 ()

        for x in 1 .. 5 do
            yield! t0 ()

        yield! t0 ()

        printfn "done!"
    }

negativeTest () |> expect 0 13


// Test also translation of return!

type CorutineBuilderWithReturnFrom() =
    inherit CoroutineBuilder()
    
    member inline this.ReturnFrom (value: 'T) : CoroutineCode = 
        this.YieldFrom(value)

    member inline this.ReturnFromFinal (value: 'T) : CoroutineCode = 
        this.YieldFromFinal(value)

let coroutineWithReturnFrom = CorutineBuilderWithReturnFrom()

let mostlyNegativeTestReturnFrom () = 
    coroutineWithReturnFrom {
        printfn "in mostlyNegativeTestReturnFrom"
        try 
            return! t0()
        finally ()

        try
            return! t0 ()
            failwith "crash"
        with
        | _ ->
            return! t0 ()

        let mutable x = 0

        while (x <- x + 1; x < 5) do
            return! t0 ()

        for x in 1 .. 5 do
            return! t0 ()

        return! t0 () // this one is ReturnFromFinal
    }

mostlyNegativeTestReturnFrom () |> expect 1 12


