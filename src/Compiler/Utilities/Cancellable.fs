#nowarn FS3513
namespace FSharp.Compiler

open System
open System.Threading

// This code provides two methods for handling cancellation in synchronous code:
// 1. Explicitly, by calling Cancellable.CheckAndThrow().
// 2. Implicitly, by wrapping the code in a cancellable computation.
// The cancellable computation propagates the CancellationToken and checks for cancellation implicitly.
// When it is impractical to use the cancellable computation, such as in deeply nested functions, Cancellable.CheckAndThrow() can be used.
// It checks a CancellationToken local to the current async execution context, held in AsyncLocal.
// Before calling Cancellable.CheckAndThrow(), this token must be set.
// The token is guaranteed to be set during execution of cancellable computation.
// Otherwise, it can be passed explicitly from the ambient async computation using Cancellable.UseToken().

[<Sealed>]
type Cancellable =
    static let tokenHolder = AsyncLocal<CancellationToken voption>()

    static let guard =
        String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DISABLE_CHECKANDTHROW_ASSERT"))

    static let ensureToken msg =
        tokenHolder.Value
        |> ValueOption.defaultWith (fun () -> if guard then failwith msg else CancellationToken.None)

    static member HasCancellationToken = tokenHolder.Value.IsSome

    static member Token = ensureToken "Token not available outside of Cancellable computation."

    static member UseToken() =
        async {
            let! ct = Async.CancellationToken
            return Cancellable.UsingToken ct
        }

    static member UsingToken(ct) =
        let oldCt = tokenHolder.Value
        tokenHolder.Value <- ValueSome ct

        { new IDisposable with
            member _.Dispose() = tokenHolder.Value <- oldCt
        }

    static member CheckAndThrow() =
        let token = ensureToken "CheckAndThrow invoked outside of Cancellable computation."
        token.ThrowIfCancellationRequested()

    static member TryCheckAndThrow() =
        match tokenHolder.Value with
        | ValueNone -> ()
        | ValueSome token -> token.ThrowIfCancellationRequested()

namespace Internal.Utilities.Library.CancellableImplementation

open System
open System.Threading
open FSharp.Compiler

open FSharp.Core.CompilerServices.StateMachineHelpers

open Microsoft.FSharp.Core.CompilerServices
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices

type ITrampolineInvocation =
    abstract member MoveNext: unit -> unit
    abstract IsCompleted: bool

[<Struct; NoComparison>]
type ExecutionState =
    | Running
    | Complete
    | Cancelled of oce: OperationCanceledException
    | Error of edi: ExceptionDispatchInfo

[<Sealed>]
type Trampoline() =

    static let current = new ThreadLocal<_>(fun () -> Trampoline())

    let stack = System.Collections.Generic.Stack<ITrampolineInvocation>()

    member val State: ExecutionState = Running with get, set

    member _.Set(invocation: ITrampolineInvocation) = stack.Push(invocation)

    member this.Execute(invocation) =
        this.State <- Running

        stack.Push invocation

        while stack.Count > 0 do
            stack.Peek().MoveNext()

            if stack.Peek().IsCompleted then
                stack.Pop() |> ignore

    static member Current = current.Value

type ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Data: 'T

type IMachineTemplateWrapper<'T> =
    abstract Clone: unit -> ITrampolineInvocation<'T>

type ICancellableStateMachine<'T> = IResumableStateMachine<'T>
type CancellableStateMachine<'T> = ResumableStateMachine<'T>
type CancellableResumptionFunc<'T> = ResumptionFunc<'T>
type CancellableResumptionDynamicInfo<'T> = ResumptionDynamicInfo<'T>
type CancellableCode<'Data, 'T> = ResumableCode<'Data, 'T>

[<NoEquality; NoComparison>]
type CancellableInvocation<'T, 'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>>(machine: 'Machine)
    =

    let mutable machine = machine

    interface ITrampolineInvocation<'T> with
        member _.MoveNext() = machine.MoveNext()
        member _.IsCompleted = machine.ResumptionPoint = -1
        member _.Data = machine.Data

    interface IMachineTemplateWrapper<'T> with
        member _.Clone() = CancellableInvocation<_, _>(machine)

[<Struct; NoComparison>]
type Cancellable<'T>(template: IMachineTemplateWrapper<'T>) =

    member _.GetInvocation() = template.Clone()

type CancellableBuilder() =

    // Delay checks for cancellation and skips further steps when Cancelled / Errored.
    member inline _.Delay(generator: unit -> CancellableCode<'Data, 'T>) =
        CancellableCode<'Data, 'T>(fun sm ->
            if Cancellable.Token.IsCancellationRequested then
                Trampoline.Current.State <- Cancelled(OperationCanceledException Cancellable.Token)

            match Trampoline.Current.State with
            | Running ->
                try
                    (generator ()).Invoke(&sm)
                with
                | :? OperationCanceledException as oce when oce.CancellationToken = Cancellable.Token ->
                    Trampoline.Current.State <- Cancelled oce
                    true
                | _exn ->
                    Trampoline.Current.State <- Error(ExceptionDispatchInfo.Capture _exn)
                    true
            | _ -> true)

    member inline _.Zero() : CancellableCode<'Data, unit> = ResumableCode.Zero()

    member inline _.While(condition, body) : CancellableCode<'Data, unit> =
        ResumableCode.While((fun () -> Trampoline.Current.State.IsRunning && condition ()), body)

    member inline this.For(sequence: seq<'T>, body: 'T -> CancellableCode<'Data, unit>) : CancellableCode<'Data, unit> =
        // A for loop is just a using statement on the sequence's enumerator...
        ResumableCode.Using(
            sequence.GetEnumerator(),
            // ... and its body is a while loop that advances the enumerator and runs the body on each element.
            (fun e ->
                this.While(
                    (fun () ->
                        __debugPoint "ForLoop.InOrToKeyword"
                        e.MoveNext()),
                    CancellableCode<'Data, unit>(fun sm -> (body e.Current).Invoke(&sm))
                ))
        )

    member inline _.Combine(code1, code2) : CancellableCode<'Data, 'T> = ResumableCode.Combine(code1, code2)

    member inline _.Using(resource, body) : CancellableCode<'Data, 'T> = ResumableCode.Using(resource, body)

    member inline _.TryWith(body: CancellableCode<'Data, 'T>, catch: exn -> CancellableCode<'Data, 'T>) : CancellableCode<'Data, 'T> =
        CancellableCode(fun sm ->
            let __stack_body_fin = body.Invoke(&sm)

            match Trampoline.Current.State with
            | Error edi when __stack_body_fin ->
                try
                    Trampoline.Current.State <- Running
                    (catch edi.SourceException).Invoke(&sm)
                with
                // Unhandled, restore state.
                | exn when exn = edi.SourceException ->
                    Trampoline.Current.State <- Error edi
                    true
                // Exception in handler.
                | _newExn ->
                    Trampoline.Current.State <- Error(ExceptionDispatchInfo.Capture _newExn)
                    true
            | _ -> __stack_body_fin)

    member inline _.TryFinally(body: CancellableCode<'Data, 'T>, compensation) : CancellableCode<'Data, 'T> =
        ResumableCode.TryFinally(
            body,
            ResumableCode(fun _sm ->
                compensation ()
                true)
        )

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode(fun sm ->
            sm.Data <- value
            true)

    member inline this.Yield(value) = this.Return(value)

    member inline _.Bind
        (code: Cancellable<'U>, [<InlineIfLambda>] continuation: 'U -> CancellableCode<'Data, 'T>)
        : CancellableCode<'Data, 'T> =
        CancellableCode(fun sm ->
            if __useResumableCode then
                let mutable invocation = code.GetInvocation()

                match __resumableEntry () with
                | Some contID ->
                    sm.ResumptionPoint <- contID
                    Trampoline.Current.Set invocation
                    false
                | None ->
                    if Trampoline.Current.State.IsRunning then
                        (invocation.Data |> continuation).Invoke(&sm)
                    else
                        true

            else
                // Dynamic Bind.

                let mutable invocation = code.GetInvocation()

                let cont =
                    CancellableResumptionFunc<'Data>(fun sm ->
                        if Trampoline.Current.State.IsRunning then
                            (invocation.Data |> continuation).Invoke(&sm)
                        else
                            true)

                Trampoline.Current.Set invocation
                sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                false)

    member inline this.ReturnFrom(comp: Cancellable<'T>) : CancellableCode<'T, 'T> = this.Bind(comp, this.Return)

    member inline _.Run(code: CancellableCode<'T, 'T>) : Cancellable<'T> =
        if __useResumableCode then
            __stateMachine<'T, Cancellable<'T>>

                (MoveNextMethodImpl<_>(fun sm ->
                    __resumeAt sm.ResumptionPoint
                    let __stack_code_fin = code.Invoke(&sm)

                    if __stack_code_fin then
                        sm.ResumptionPoint <- -1))

                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))

                (AfterCode<_, _>(fun sm -> Cancellable(CancellableInvocation(sm))))
        else
            // Dynamic Run.

            let initialResumptionFunc = CancellableResumptionFunc(fun sm -> code.Invoke(&sm))

            let resumptionInfo =
                { new CancellableResumptionDynamicInfo<_>(initialResumptionFunc) with
                    member info.MoveNext(sm) =
                        if info.ResumptionFunc.Invoke(&sm) then
                            sm.ResumptionPoint <- -1

                    member _.SetStateMachine(_, _) = ()
                }

            let sm = CancellableStateMachine(ResumptionDynamicInfo = resumptionInfo)

            Cancellable(CancellableInvocation(sm))

namespace Internal.Utilities.Library

open System
open System.Threading

type Cancellable<'T> = CancellableImplementation.Cancellable<'T>

[<AutoOpen>]
module CancellableAutoOpens =

    let cancellable = CancellableImplementation.CancellableBuilder()

module Cancellable =
    open Internal.Utilities.Library.CancellableImplementation

    let run ct (code: Cancellable<_>) =
        use _ = FSharp.Compiler.Cancellable.UsingToken ct

        let invocation = code.GetInvocation()
        Trampoline.Current.Execute invocation
        invocation

    let runWithoutCancellation code =
        let invocation = run CancellationToken.None code

        match Trampoline.Current.State with
        | Error edi ->
            edi.Throw()
            Unchecked.defaultof<_>
        | Cancelled _ -> failwith "Unexpected cancel in runWithoutCancellation."
        | _ -> invocation.Data

    let toAsync (code: Cancellable<_>) =
        async {
            let! ct = Async.CancellationToken

            return!
                Async.FromContinuations(fun (cont, econt, ccont) ->
                    let invocation = run ct code

                    match Trampoline.Current.State with
                    | Cancelled oce -> ccont oce
                    | Error edi -> econt edi.SourceException
                    | _ -> cont invocation.Data)
        }

    let token () =
        cancellable { FSharp.Compiler.Cancellable.Token }
