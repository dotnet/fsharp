
module Tests.Coroutines

open System
open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open FSharp.Core.LanguagePrimitives.IntrinsicOperators
open FSharp.Collections

let verbose = true 
type ICoroutineStateMachine<'Data> =
    abstract GetResumptionPoint: unit -> int

type GetResumptionPointMethodImpl<'Template> = delegate of byref<'Template> -> int

let inline MoveNext(x: byref<'T> when 'T :> IAsyncStateMachine) = x.MoveNext()
let inline SetStateMachine(x: byref<'T> when 'T :> IAsyncStateMachine, state) = x.SetStateMachine(state)
let inline GetResumptionPoint(x: byref<'T> when 'T :> ICoroutineStateMachine<'Data>) = x.GetResumptionPoint()

/// Acts as a template for struct state machines introduced by __structStateMachine, and also as a reflective implementation
[<Struct; NoComparison; NoEquality>]
type CoroutineStateMachine<'Data> =

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable Data: 'Data

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionPoint: int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc: CoroutineResumption<'Data>

    //[<DefaultValue(false)>]
    //val mutable Id: int

    interface ICoroutineStateMachine<'Data> with 
        member sm.GetResumptionPoint() = sm.ResumptionPoint

    interface IAsyncStateMachine with 
        
        // Used when interpreted.  For "__structStateMachine" it is replaced.
        member sm.MoveNext() = 
            //if verbose then printfn $"[{sm.Id}] dynamic invoke"
            let fin = sm.ResumptionFunc.Invoke(&sm)
            if fin then
                //if verbose then printfn $"[{sm.Id}] dynamic terminate"
                sm.ResumptionPoint  <- -1

        // Used when interpreted.  For "__structStateMachine" it is replaced.
        member sm.SetStateMachine(state) = 
            (sm <- (state :?> CoroutineStateMachine<'Data>))

and CoroutineResumption<'Data> = delegate of byref<CoroutineStateMachine<'Data>> -> bool

and [<AbstractClass; NoEquality; NoComparison>] 
    Coroutine() =
    static let mutable x = 1000
    do x <- x + 1
    let id = x
    //do printfn $"[{id}] created"
    member _.Id = id
    abstract ResumptionPoint: int
    abstract Completed: bool
    abstract MoveNext: unit -> unit
    
and [<NoEquality; NoComparison>] 
    Coroutine<'Data, 'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> ICoroutineStateMachine<'Data>>() =
    inherit Coroutine()

    [<DefaultValue(false)>]
    val mutable Machine: 'Machine

    override cr.ResumptionPoint =
        GetResumptionPoint(&cr.Machine)

    override cr.Completed =
        GetResumptionPoint(&cr.Machine) = -1

    override cr.MoveNext() = 
        //if verbose then printfn $"[{cr.Id}] move"
        MoveNext(&cr.Machine)

[<ResumableCode>]
type CoroutineCode<'Data, 'T> = delegate of byref<CoroutineStateMachine<'Data>> -> bool
//type CoroutineCode<'Data> = CoroutineCode<'Data, unit>

module Coroutine =
    
    let inline Delay(f : unit -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm -> (f()).Invoke(&sm))

    let inline Run(code : CoroutineCode<'Data, 'T>) : Coroutine = 
        if __useResumableCode then 
            __structStateMachine<CoroutineStateMachine<'Data>, Coroutine>
                // IAsyncStateMachine.MoveNext
                (MoveNextMethod<CoroutineStateMachine<'Data>>(fun sm -> 
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
                (SetStateMachineMethod<CoroutineStateMachine<'Data>>(fun sm state -> 
                    SetStateMachine(&sm, state)))

                // Other interfaces (ICoroutineStateMachine)
                [| 
                   (typeof<ICoroutineStateMachine<'Data>>, "GetResumptionPoint", GetResumptionPointMethodImpl<CoroutineStateMachine<'Data>>(fun sm -> 
                        sm.ResumptionPoint) :> _);
                 |]

                // Start
                (AfterCode<CoroutineStateMachine<'Data>,_>(fun sm -> 
                    let mutable cr = Coroutine<'Data, CoroutineStateMachine<'Data>>()
                    cr.Machine <- sm
                    //cr.Machine.Id <- cr.Id
                    //if verbose then printfn $"[{cr.Id}] static create"
                    cr :> Coroutine))
        else 
            let mutable cr = Coroutine<'Data, CoroutineStateMachine<'Data>>()
            cr.Machine.ResumptionFunc <- CoroutineResumption(fun sm -> code.Invoke(&sm))
            //cr.Machine.Id <- cr.Id
            //if verbose then printfn $"[{cr.Id}] dynamic create"
            cr :> Coroutine


    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    let inline Zero() : CoroutineCode<'Data, unit> =
        CoroutineCode<'Data, unit>(fun sm -> true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let CombineDynamic(code1: CoroutineCode<'Data, unit>, code2: CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            if code1.Invoke(&sm) then 
                code2.Invoke(&sm)
            else
                let rec resume (mf: CoroutineResumption<'Data>) =
                    CoroutineResumption<'Data>(fun sm -> 
                        if mf.Invoke(&sm) then 
                            code2.Invoke(&sm)
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false)

                sm.ResumptionFunc <- resume sm.ResumptionFunc
                false)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let inline Combine(code1: CoroutineCode<'Data, unit>, code2: CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            if __useResumableCode then
                //-- RESUMABLE CODE START
                // NOTE: The code for code1 may contain await points! Resuming may branch directly
                // into this code!
                let __stack_fin = code1.Invoke(&sm)
                if __stack_fin then 
                    code2.Invoke(&sm)
                else
                    false
                //-- RESUMABLE CODE END
            else
                CombineDynamic(code1, code2).Invoke(&sm))

    //let inline WhileAsync([<InlineIfLambda>] condition : byref<bool> -> CoroutineCode<'Data>, body : CoroutineCode<'Data>) : CoroutineCode<'Data> =
    //    CoroutineCode<'Data>(fun sm -> 
    //        if __useResumableCode then
    //            //-- RESUMABLE CODE START
    //            let mutable __stack_fin = false
    //            let mutable __stack_proceed = true
    //            while __stack_proceed do
    //                let mutable __stack_guard_result = false
    //                let __stack_guard_fin = condition(&__stack_guard_result).Invoke(&sm)
    //                if not __stack_guard_fin then
    //                    __stack_proceed <- false
    //                    __stack_fin <- false
    //                elif __stack_guard_result then
    //                    let __stack_body_fin = body.Invoke(&sm)
    //                    __stack_fin <- __stack_body_fin
    //                    __stack_proceed <- __stack_body_fin 
    //                else 
    //                    __stack_proceed <- false
    //                    __stack_fin <- true
    //            __stack_fin
    //            //-- RESUMABLE CODE END
    //        else
    //            failwith "reflective execution of WhileAsync NYI")

    let WhileDynamic (condition: unit -> bool, body: CoroutineCode<'Data, unit>) : CoroutineCode<'Data, unit> =
        CoroutineCode<'Data, unit>(fun sm ->
            let rec repeat() = 
                CoroutineResumption<'Data>(fun sm -> 
                    if condition() then 
                        if body.Invoke(&sm) then
                            repeat().Invoke(&sm)
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    else
                        true)
            and resume (mf: CoroutineResumption<'Data>) =
                CoroutineResumption<'Data>(fun sm -> 
                    let step = mf.Invoke(&sm)
                    if step then 
                        repeat().Invoke(&sm)
                    else
                        sm.ResumptionFunc <- resume sm.ResumptionFunc
                        false)

            repeat().Invoke(&sm))

    /// Builds a step that executes the body while the condition predicate is true.
    let inline While ([<InlineIfLambda>] condition : unit -> bool, body : CoroutineCode<'Data, unit>) : CoroutineCode<'Data, unit> =
        CoroutineCode<'Data, unit>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_go = true 
                while __stack_go && condition() do
                    // NOTE: The body of the state machine code for 'while' may contain await points, so resuming
                    // the code will branch directly into the expanded 'body', branching directly into the while loop
                    let __stack_body_fin = body.Invoke(&sm)
                    //if verbose then printfn "__stack_body_fin = %b" __stack_body_fin
                    // If the body completed, we go back around the loop (__stack_go = true)
                    // If the body yielded, we yield (__stack_go = false)
                    __stack_go <- __stack_body_fin
                __stack_go
                //-- RESUMABLE CODE END
            else
                WhileDynamic(condition, body).Invoke(&sm))

    let TryWithDynamic (body: CoroutineCode<'Data, 'T>, handler: exn -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            let rec resume (mf: CoroutineResumption<'Data>) =
                CoroutineResumption<'Data>(fun sm -> 
                    try
                        if mf.Invoke(&sm) then 
                            true
                        else
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                            false
                    with exn -> 
                        (handler exn).Invoke(&sm))
            try
                let step = body.Invoke(&sm)
                if not step then 
                    sm.ResumptionFunc <- sm.ResumptionFunc
                step
                        
            with exn -> 
                (handler exn).Invoke(&sm))

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline TryWith (body: CoroutineCode<'Data, 'T>, catch: exn -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_completed = false
                let mutable __stack_caught = false
                let mutable __stack_savedExn = Unchecked.defaultof<_>
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/with for code as we enter into the handler.
                __stack_completed <- __stack_completed || __stack_completed
                try
                    // The try block may contain await points.
                    let __stack_fin = body.Invoke(&sm)
                    // If we make it to the assignment we prove we've made a step
                    __stack_completed <- __stack_fin
                with exn -> 
                    // Note, remarkExpr in the F# compiler detects this pattern as the code
                    // is inlined and elides the debug sequence point on either the 'compensation'
                    // or 'reraise' statement for the code. This is because the inlining will associate
                    // the sequence point with the 'try' of the TryFinally because that is the range
                    // given for the whole expression 
                    //      task.TryWith(....) 
                    // If you change this code you should check debug sequence points and the generated
                    // code tests for try/with in tasks.
                    __stack_caught <- true
                    __stack_savedExn <- exn

                if __stack_caught then 
                    // Place the catch code outside the catch block 
                    (catch __stack_savedExn).Invoke(&sm)
                else
                    __stack_completed
                //-- RESUMABLE CODE END

            else
                TryWithDynamic(body, catch).Invoke(&sm))

    let TryFinallyDynamic (body: CoroutineCode<'Data, 'T>, compensation : unit -> unit) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            let rec resume (mf: CoroutineResumption<'Data>) =
                CoroutineResumption<'Data>(fun sm -> 
                    let mutable completed = false
                    try
                        completed <- mf.Invoke(&sm)
                        if not completed then 
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                    with _ ->
                        compensation()
                        reraise()
                    if completed then 
                        compensation()
                    completed)

            let mutable completed = false
            try
                completed <- body.Invoke(&sm)
                if not completed then 
                    sm.ResumptionFunc <- resume sm.ResumptionFunc
                       
            with _ ->
                compensation()
                reraise()

            if completed then 
                compensation()

            completed)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline TryFinally (body: CoroutineCode<'Data, 'T>, [<InlineIfLambda>] compensation : unit -> unit) : CoroutineCode<'Data, 'T> =
        CoroutineCode<'Data, 'T>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_completed = false
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/finally. The 'try' is used as the range for the
                // F# computation expression desugaring to 'TryFinally' and this range in turn gets applied
                // to inlined code.
                __stack_completed <- __stack_completed || __stack_completed
                try
                    let __stack_fin = body.Invoke(&sm)
                    // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                    // may skip this step.
                    __stack_completed <- __stack_fin
                with _ ->
                    // Note, remarkExpr in the F# compiler detects this pattern as the code
                    // is inlined and elides the debug sequence point on either the 'compensation'
                    // or 'reraise' statement for the code. This is because the inlining will associate
                    // the sequence point with the 'try' of the TryFinally because that is the range
                    // given for the whole expression 
                    //      task.TryFinally(....) 
                    // If you change this code you should check debug sequence points and the generated
                    // code tests for try/finally in tasks.
                    compensation()
                    reraise()

                if __stack_completed then 
                    compensation()
                __stack_completed
                //-- RESUMABLE CODE END
            else
                TryFinallyDynamic(body, compensation).Invoke(&sm))

    let inline Using (resource : 'Resource, body : 'Resource -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> when 'Resource :> IDisposable = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        TryFinally(
            CoroutineCode<'Data, 'T>(fun sm -> (body resource).Invoke(&sm)),
            (fun () -> if not (isNull (box resource)) then resource.Dispose()))

    let inline For (sequence : seq<'T>, body : 'T -> CoroutineCode<'Data, unit>) : CoroutineCode<'Data, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> While((fun () -> e.MoveNext()), CoroutineCode<'Data, unit>(fun sm -> (body e.Current).Invoke(&sm)))))

    let YieldDynamic () : CoroutineCode<'Data, unit> = 
        CoroutineCode<'Data, unit>(fun sm -> 
            let cont = CoroutineResumption<'Data>(fun sm -> true)
            sm.ResumptionFunc <- cont
            false)

    let inline Yield () : CoroutineCode<'Data, 'T> = 
        CoroutineCode<'Data, 'T>(fun sm -> 
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                //if verbose then printfn "Yield! - resumable" 
                match __resumableEntry() with 
                | Some contID ->
                    sm.ResumptionPoint <- contID
                    //if verbose then printfn $"[{sm.Id}] Yield: returning false to indicate yield, contID = {contID}" 
                    false
                | None ->
                    //if verbose then printfn $"[{sm.Id}] Yield: returning true to indicate post-yield" 
                    true
                //-- RESUMABLE CODE END
            else
                //if verbose then printfn "Yield - dynamic" 
                YieldDynamic().Invoke(&sm))

    let inline YieldFrom (other: Coroutine) : CoroutineCode<'Data, unit> = 
        While((fun () -> not other.Completed), CoroutineCode<'Data, unit>(fun sm -> 
            //if verbose then printfn $"[{sm.Id}] calling [{other.Id}].MoveNext, it will resume at {other.ResumptionPoint}";
            other.MoveNext()
            let __stack_fin = other.Completed
            if not __stack_fin then
                // This will yield with __stack_fin2 = false
                // This will resume with __stack_fin2 = true
                let __stack_fin2 = Yield().Invoke(&sm)
                __stack_fin2
            else
               true))

type CoroutineBuilder() =
    
    member inline _.Delay(f : unit -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> = Coroutine.Delay(f)

    member inline _.Run(code : CoroutineCode<'Data, 'T>) : Coroutine = 
        if __useResumableCode then 
            __structStateMachine<CoroutineStateMachine<'Data>, Coroutine>
                // IAsyncStateMachine.MoveNext
                (MoveNextMethod<CoroutineStateMachine<'Data>>(fun sm -> 
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
                (SetStateMachineMethod<CoroutineStateMachine<'Data>>(fun sm state -> 
                    SetStateMachine(&sm, state)))

                // Other interfaces (ICoroutineStateMachine)
                [| 
                   (typeof<ICoroutineStateMachine<'Data>>, "GetResumptionPoint", GetResumptionPointMethodImpl<CoroutineStateMachine<'Data>>(fun sm -> 
                        sm.ResumptionPoint) :> _);
                 |]

                // Start
                (AfterCode<CoroutineStateMachine<'Data>,_>(fun sm -> 
                    let mutable cr = Coroutine<'Data, CoroutineStateMachine<'Data>>()
                    cr.Machine <- sm
                    //cr.Machine.Id <- cr.Id
                    //if verbose then printfn $"[{cr.Id}] static create"
                    cr :> Coroutine))
        else 
            let mutable cr = Coroutine<'Data, CoroutineStateMachine<'Data>>()
            cr.Machine.ResumptionFunc <- CoroutineResumption(fun sm -> code.Invoke(&sm))
            //cr.Machine.Id <- cr.Id
            //if verbose then printfn $"[{cr.Id}] dynamic create"
            cr :> Coroutine


    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : CoroutineCode<'Data, unit> = Coroutine.Zero()

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine(code1: CoroutineCode<'Data, unit>, code2: CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        Coroutine.Combine(code1, code2)

    //member inline _.WhileAsync([<InlineIfLambda>] condition : byref<bool> -> CoroutineCode<'Data>, body : CoroutineCode<'Data>) : CoroutineCode<'Data> =
    //    CoroutineCode<'Data>(fun sm -> 
    //        if __useResumableCode then
    //            //-- RESUMABLE CODE START
    //            let mutable __stack_fin = false
    //            let mutable __stack_proceed = true
    //            while __stack_proceed do
    //                let mutable __stack_guard_result = false
    //                let __stack_guard_fin = condition(&__stack_guard_result).Invoke(&sm)
    //                if not __stack_guard_fin then
    //                    __stack_proceed <- false
    //                    __stack_fin <- false
    //                elif __stack_guard_result then
    //                    let __stack_body_fin = body.Invoke(&sm)
    //                    __stack_fin <- __stack_body_fin
    //                    __stack_proceed <- __stack_body_fin 
    //                else 
    //                    __stack_proceed <- false
    //                    __stack_fin <- true
    //            __stack_fin
    //            //-- RESUMABLE CODE END
    //        else
    //            failwith "reflective execution of WhileAsync NYI")

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While ([<InlineIfLambda>] condition : unit -> bool, body : CoroutineCode<'Data, unit>) : CoroutineCode<'Data, unit> =
        Coroutine.While(condition, body)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith (body: CoroutineCode<'Data, 'T>, catch: exn -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        Coroutine.TryWith(body, catch)

    static member TryWithDynamic (body: CoroutineCode<'Data, 'T>, handler: exn -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> =
        Coroutine.TryWithDynamic(body, handler)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally (body: CoroutineCode<'Data, 'T>, [<InlineIfLambda>] compensation : unit -> unit) : CoroutineCode<'Data, 'T> =
        Coroutine.TryFinally(body, compensation)

    member inline builder.Using (resource : 'Resource, body : 'Resource -> CoroutineCode<'Data, 'T>) : CoroutineCode<'Data, 'T> when 'Resource :> IDisposable = 
        Coroutine.Using(resource, body)

    member inline builder.For (sequence : seq<'T>, body : 'T -> CoroutineCode<'Data, unit>) : CoroutineCode<'Data, unit> =
        Coroutine.For(sequence, body)

    member inline _.Yield (_dummy: unit) : CoroutineCode<'Data, 'T> = 
        Coroutine.Yield()

    member inline builder.YieldFrom (other: Coroutine) : CoroutineCode<'Data, unit> = 
        Coroutine.YieldFrom(other)

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
                not t.Completed) do 
            printfn "yield"

    dumpCoroutine (t1())
    //dumpCoroutine (t2())

    //if verbose then printfn "t1() = %A" (TaskSeq.toArray (t1()))
    //if verbose then printfn "t2() = %A" (TaskSeq.toArray (t2()))

    //if verbose then printfn "perf2() = %A" (TaskSeq.toArray (perf2()))



