// Original notice:
// To the extent possible under law, the author(s) have dedicated all copyright and related and neighboring rights
// to this software to the public domain worldwide. This software is distributed without any warranty.
//
// Updates:
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "9"
#nowarn "51"
namespace Microsoft.FSharp.Core.CompilerServices

#if !BUILDING_WITH_LKG && !BUILD_FROM_SOURCE
open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control
open Microsoft.FSharp.Collections

type IPriority3 = interface end

type IPriority2 = interface inherit IPriority3 end

type IPriority1 = interface inherit IPriority2 end

type MoveNextMethodImpl<'Template> = delegate of byref<'Template> -> unit

type SetStateMachineMethodImpl<'Template> = delegate of byref<'Template> * IAsyncStateMachine -> unit

type AfterCode<'Template, 'Result> = delegate of byref<'Template> -> 'Result

type GetResumptionPointMethodImpl<'Template> = delegate of byref<'Template> -> int

type SetResumableStateMachineDataMethodImpl<'Template, 'Data> = delegate of byref<'Template> * 'Data -> unit

type GetResumableStateMachineDataMethodImpl<'Template, 'Data> = delegate of byref<'Template> -> 'Data

type IResumableStateMachine<'Data> =
    abstract ResumptionPoint: int
    abstract Data: 'Data with get, set


[<AttributeUsage (AttributeTargets.Delegate ||| AttributeTargets.Method,AllowMultiple=false)>]  
[<Sealed>]
type ResumableCodeAttribute() = 
    inherit System.Attribute()

module StateMachineHelpers = 

    /// Statically determines whether resumable code is being used
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let __useResumableCode<'T> : bool = false
        
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let __resumableEntry () : int option = 
        failwith "__resumableEntry should always be guarded by __useResumableCode and only used in valid state machine implementations"

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let __resumeAt<'T> (programLabel: int) : 'T = 
        ignore programLabel
        failwith "__resumeAt should always be guarded by __useResumableCode and only used in valid state machine implementations"

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let __structStateMachine<'Template, 'Result> (moveNextMethod: MoveNextMethodImpl<'Template>) (setStateMachineMethod: SetStateMachineMethodImpl<'Template>) (otherMethodImpls: (Type * string * Delegate)[]) (afterCode: AfterCode<'Template, 'Result>): 'Result =
        ignore moveNextMethod
        ignore setStateMachineMethod
        ignore otherMethodImpls
        ignore afterCode
        failwith "__structStateMachine should always be guarded by __useResumableCode and only used in valid state machine implementations"

/// Acts as a template for struct state machines introduced by __structStateMachine, and also as a reflective implementation
[<Struct; NoComparison; NoEquality>]
type ResumableStateMachine<'Data> =

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    [<DefaultValue(false)>]
    val mutable Data: 'Data

    /// When statically compiled, holds the continuation goto-label further execution of the state machine
    /// The special value -1000 indicates a 'true' condition for an async while loop
    /// The special value -1001 indicates a 'false' condition for an async while loop
    [<DefaultValue(false)>]
    val mutable ResumptionPoint: int

    /// When interpreted, holds the continuation for the further execution of the state machine
    [<DefaultValue(false)>]
    val mutable ResumptionFunc: ResumptionFunc<'Data>

    //[<DefaultValue(false)>]
    //val mutable Id: int

    interface IResumableStateMachine<'Data> with 
        member sm.ResumptionPoint = sm.ResumptionPoint
        member sm.Data with get() = sm.Data and set v = sm.Data <- v

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
            (sm <- (state :?> ResumableStateMachine<'Data>))

and ResumptionFunc<'Data> = delegate of byref<ResumableStateMachine<'Data>> -> bool

[<ResumableCodeAttribute>]
type ResumableCode<'Data, 'T> = delegate of byref<ResumableStateMachine<'Data>> -> bool

type NonResumableCode<'Data, 'T> = delegate of byref<ResumableStateMachine<'Data>> -> 'T

open StateMachineHelpers
module ResumableCode =
    
    let inline Delay(f : unit -> ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm -> (f()).Invoke(&sm))

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    let inline Zero() : ResumableCode<'Data, unit> =
        ResumableCode<'Data, unit>(fun sm -> true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    let CombineDynamic(code1: ResumableCode<'Data, unit>, code2: ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            if code1.Invoke(&sm) then 
                code2.Invoke(&sm)
            else
                let rec resume (mf: ResumptionFunc<'Data>) =
                    ResumptionFunc<'Data>(fun sm -> 
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
    let inline Combine(code1: ResumableCode<'Data, unit>, code2: ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
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

    let WhileAsyncDynamic (condition: ResumableCode<'Data,bool>, body: ResumableCode<'Data, unit>) : ResumableCode<'Data, unit> =
        ResumableCode<'Data, unit>(fun sm ->
            let rec runGuard() =
                resumeGuard(ResumptionFunc<'Data>(fun sm -> condition.Invoke(&sm)))
            and resumeGuard (mf: ResumptionFunc<'Data>)  : ResumptionFunc<'Data> =
                ResumptionFunc<'Data>(fun sm -> 
                    let fin = mf.Invoke(&sm)
                    if fin then 
                        // On completion of the asynchronous condition we expect either -1000 or -1001 to be written into ResumptionPoint
                        if (sm.ResumptionPoint = -1000) then 
                            runBody().Invoke(&sm)
                        else
                            true
                    else
                        sm.ResumptionFunc <- resumeGuard sm.ResumptionFunc
                        false)
            and runBody() : ResumptionFunc<'Data> = 
                resumeBody(ResumptionFunc<'Data>(fun sm -> body.Invoke(&sm)))
            and resumeBody (mf: ResumptionFunc<'Data>) =
                ResumptionFunc<'Data>(fun sm -> 
                    let fin = mf.Invoke(&sm)
                    if fin then 
                        runGuard().Invoke(&sm)
                    else
                        sm.ResumptionFunc <- resumeBody sm.ResumptionFunc
                        false)

            runGuard().Invoke(&sm))

    let inline WhileAsync(condition: ResumableCode<'Data,bool>, body : ResumableCode<'Data,unit>) : ResumableCode<'Data,unit> =
        ResumableCode<'Data,unit>(fun sm -> 
            if __useResumableCode then
                //-- RESUMABLE CODE START
                let mutable __stack_fin = true
                let mutable __stack_go = true
                while __stack_go do
                    //if verbose then printfn $"starting condition task"
                    let __stack_condition_completed = condition.Invoke(&sm)
                    if __stack_condition_completed then
                        // On completion of the condition we expect either -1000 or -1001 to be written into ResumptionPoint
                        //if verbose then printfn $"condition task was synchronous"
                        __stack_go <- (sm.ResumptionPoint = -1000) // 'true'
                    else
                        __stack_go <- false

                    if __stack_go then
                        let __stack_body_fin = body.Invoke(&sm)
                        __stack_fin <- __stack_body_fin
                        __stack_go <- __stack_fin
                __stack_fin
                //-- RESUMABLE CODE END
            else
                WhileAsyncDynamic(condition, body).Invoke(&sm))

    /// Builds a step that executes the body while the condition predicate is true.
    let inline While ([<InlineIfLambda>] condition : unit -> bool, body : ResumableCode<'Data, unit>) : ResumableCode<'Data, unit> =
        ResumableCode<'Data, unit>(fun sm ->
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
                WhileAsyncDynamic(
                    ResumableCode<_,_>(fun sm -> 
                        let step = condition() 
                        sm.ResumptionPoint <- (if step then -1000 else -1001)
                        true), 
                    body).Invoke(&sm))

    let TryWithDynamic (body: ResumableCode<'Data, 'T>, handler: exn -> ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            let rec resume (mf: ResumptionFunc<'Data>) =
                ResumptionFunc<'Data>(fun sm -> 
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
    let inline TryWith (body: ResumableCode<'Data, 'T>, catch: exn -> ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_fin = false
                let mutable __stack_caught = false
                let mutable __stack_savedExn = Unchecked.defaultof<_>
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/with for code as we enter into the handler.
                __stack_fin <- __stack_fin || __stack_fin
                try
                    // The try block may contain await points.
                    let __stack_fin2 = body.Invoke(&sm)
                    // If we make it to the assignment we prove we've made a step
                    __stack_fin <- __stack_fin
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
                    __stack_fin
                //-- RESUMABLE CODE END

            else
                TryWithDynamic(body, catch).Invoke(&sm))

    let TryFinallyAsyncDynamic (body: ResumableCode<'Data, 'T>, compensation: ResumableCode<'Data,unit>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            let mutable completed = false
            let mutable savedExn = None
            let rec compensate (mf: ResumptionFunc<'Data>) =
                ResumptionFunc<'Data>(fun sm -> 
                    completed <- mf.Invoke(&sm)
                    if not completed then 
                        sm.ResumptionFunc <- compensate sm.ResumptionFunc
                    if completed then
                        match savedExn with 
                        | None -> ()
                        | Some exn -> raise exn
                    completed)
            let rec resume (mf: ResumptionFunc<'Data>) =
                ResumptionFunc<'Data>(fun sm -> 
                    try
                        completed <- mf.Invoke(&sm)
                        if not completed then 
                            sm.ResumptionFunc <- resume sm.ResumptionFunc
                    with exn ->
                        savedExn <- Some exn 
                        completed <- true
                    if completed then 
                        compensate(ResumptionFunc<'Data>(fun sm -> compensation.Invoke(&sm))).Invoke(&sm)
                    else
                        false)

            resume(ResumptionFunc<'Data>(fun sm -> body.Invoke(&sm))).Invoke(&sm))

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline TryFinally (body: ResumableCode<'Data, 'T>, [<InlineIfLambda>] compensation: NonResumableCode<'Data,unit>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_fin = false
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/finally. The 'try' is used as the range for the
                // F# computation expression desugaring to 'TryFinally' and this range in turn gets applied
                // to inlined code.
                __stack_fin <- __stack_fin || __stack_fin
                try
                    let __stack_body_fin = body.Invoke(&sm)
                    // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                    // may skip this step.
                    __stack_fin <- __stack_body_fin
                with _ ->
                    // Note, remarkExpr in the F# compiler detects this pattern as the code
                    // is inlined and elides the debug sequence point on either the 'compensation'
                    // or 'reraise' statement for the code. This is because the inlining will associate
                    // the sequence point with the 'try' of the TryFinally because that is the range
                    // given for the whole expression 
                    //      task.TryFinally(....) 
                    // If you change this code you should check debug sequence points and the generated
                    // code tests for try/finally in tasks.
                    compensation.Invoke(&sm)
                    reraise()

                if __stack_fin then 
                    compensation.Invoke(&sm)
                __stack_fin
                //-- RESUMABLE CODE END
            else
                TryFinallyAsyncDynamic(body, ResumableCode<_,_>(fun sm -> compensation.Invoke(&sm); true)).Invoke(&sm))

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    let inline TryFinallyAsync (body: ResumableCode<'Data, 'T>, [<InlineIfLambda>] compensation: ResumableCode<'Data,unit>) : ResumableCode<'Data, 'T> =
        ResumableCode<'Data, 'T>(fun sm ->
            if __useResumableCode then 
                //-- RESUMABLE CODE START
                let mutable __stack_fin = false
                let mutable savedExn = ValueNone
                // This is a meaningless assignment but ensures a debug point gets laid down
                // at the 'try' in the try/finally. The 'try' is used as the range for the
                // F# computation expression desugaring to 'TryFinally' and this range in turn gets applied
                // to inlined code.
                __stack_fin <- __stack_fin || __stack_fin
                try
                    let __stack_body_fin = body.Invoke(&sm)
                    // If we make it to the assignment we prove we've made a step, an early 'ret' exit out of the try/with
                    // may skip this step.
                    __stack_fin <- __stack_body_fin
                with exn ->
                    savedExn <- ValueSome exn
                    __stack_fin <- true

                if __stack_fin then 
                    __stack_fin <- compensation.Invoke(&sm)
                __stack_fin
                //-- RESUMABLE CODE END
            else
                TryFinallyAsyncDynamic(body, compensation).Invoke(&sm))

    let inline Using (resource : 'Resource, body : 'Resource -> ResumableCode<'Data, 'T>) : ResumableCode<'Data, 'T> when 'Resource :> IDisposable = 
        // A using statement is just a try/finally with the finally block disposing if non-null.
        TryFinally(
            ResumableCode<'Data, 'T>(fun sm -> (body resource).Invoke(&sm)),
            NonResumableCode<'Data,unit>(fun sm -> if not (isNull (box resource)) then resource.Dispose()))

    let inline For (sequence : seq<'T>, body : 'T -> ResumableCode<'Data, unit>) : ResumableCode<'Data, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        Using (sequence.GetEnumerator(), 
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e -> While((fun () -> e.MoveNext()), ResumableCode<'Data, unit>(fun sm -> (body e.Current).Invoke(&sm)))))

    let YieldDynamic () : ResumableCode<'Data, unit> = 
        ResumableCode<'Data, unit>(fun sm -> 
            let cont = ResumptionFunc<'Data>(fun sm -> true)
            sm.ResumptionFunc <- cont
            false)

    let inline Yield () : ResumableCode<'Data, unit> = 
        ResumableCode<'Data, unit>(fun sm -> 
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

#endif
