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

and [<Sealed>] Trampoline() =

    static let currentThreadTrampoline = new ThreadLocal<_>(fun () -> Trampoline())

    let stack = System.Collections.Generic.Stack<ITrampolineInvocation>()

    member _.Set(invocation: ITrampolineInvocation) = stack.Push(invocation)

    static member CurrentThreadTrampoline = currentThreadTrampoline.Value

    member this.Execute(invocation) =
        stack.Push invocation

        while stack.Count > 0 do
            stack.Peek().MoveNext()

            if stack.Peek().IsCompleted then
                stack.Pop() |> ignore

[<Struct; NoComparison; NoEquality>]
type CancellableData<'T> =

    [<DefaultValue(false)>]
    val mutable Result: Result<'T, ExceptionDispatchInfo>

    member this.GetValue() =
        match this.Result with
        | Ok value -> value
        | Error edi ->
            edi.Throw()
            Unchecked.defaultof<_>

type ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Hijack: unit -> unit
    abstract Data: CancellableData<'T>

type IMachineTemplateWrapper<'T> =
    abstract Clone: unit -> ITrampolineInvocation<'T>

type ICancellableStateMachine<'T> = IResumableStateMachine<CancellableData<'T>>
type CancellableStateMachine<'T> = ResumableStateMachine<CancellableData<'T>>
type CancellableResumptionFunc<'T> = ResumptionFunc<CancellableData<'T>>
type CancellableResumptionDynamicInfo<'T> = ResumptionDynamicInfo<CancellableData<'T>>
type CancellableCode<'Data, 'T> = ResumableCode<CancellableData<'Data>, 'T>

[<NoEquality; NoComparison>]
type CancellableInvocation<'T, 'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>>(machine: 'Machine)
    =

    let mutable machine = machine

    interface ITrampolineInvocation<'T> with
        member _.MoveNext() = machine.MoveNext()
        member _.IsCompleted = machine.ResumptionPoint = -1
        member _.Data = machine.Data

        member this.Hijack() =
            Trampoline.CurrentThreadTrampoline.Set this

    interface IMachineTemplateWrapper<'T> with
        member _.Clone() = CancellableInvocation<_, _>(machine)

[<Struct; NoComparison>]
type Cancellable<'T>(template: IMachineTemplateWrapper<'T>) =

    member _.GetInvocation() = template.Clone()

module CancellableCode =
    let inline WithCancelCheck (body: CancellableCode<'Data, 'T>) =
        CancellableCode<'Data, 'T>(fun sm ->
            Cancellable.Token.ThrowIfCancellationRequested()
            body.Invoke(&sm))

    let inline FilterOce ([<InlineIfLambda>] catch: exn -> CancellableCode<'Data, 'T>) (exn: exn) =
        CancellableCode<'Data, 'T>(fun sm ->
            match exn with
            | :? OperationCanceledException as oce when oce.CancellationToken = Cancellable.Token -> true
            | _ -> (catch exn).Invoke(&sm))

type CancellableBuilder() =

    member inline _.Zero() : CancellableCode<'Data, unit> = ResumableCode.Zero()

    member inline _.For(sequence, body) : CancellableCode<'Data, unit> = ResumableCode.For(sequence, body)

    member inline _.While(condition, body) : CancellableCode<'Data, unit> =
        ResumableCode.While(condition, CancellableCode.WithCancelCheck body)

    member inline _.Delay(generator: unit -> CancellableCode<'Data, 'T>) =
        CancellableCode<'Data, 'T>(fun sm -> (generator ()).Invoke(&sm))

    member inline _.Combine(code1, code2) =
        ResumableCode.Combine(CancellableCode.WithCancelCheck code1, CancellableCode.WithCancelCheck code2)

    member inline _.Using(resource, body) : CancellableCode<'Data, 'T> = ResumableCode.Using(resource, body)

    member inline _.TryWith(body, catch) : CancellableCode<'Data, 'T> =
        ResumableCode.TryWith(CancellableCode.WithCancelCheck body, (CancellableCode.FilterOce catch))

    member inline _.TryFinally(body: CancellableCode<'Data, 'T>, compensation) : CancellableCode<'Data, 'T> =
        ResumableCode.TryFinally(
            CancellableCode.WithCancelCheck body,
            ResumableCode(fun _sm ->
                compensation ()
                true)
        )

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode(fun sm ->
            sm.Data.Result <- Ok value
            true)

    member inline this.Yield(value) = this.Return(value)

    member inline _.Bind
        (code: Cancellable<'U>, [<InlineIfLambda>] continuation: 'U -> CancellableCode<'Data, 'T>)
        : CancellableCode<'Data, 'T> =
        CancellableCode(fun sm ->
            if __useResumableCode then
                let mutable invocation = code.GetInvocation()

                let __stack_yield_complete = ResumableCode.Yield().Invoke(&sm)

                if __stack_yield_complete then
                    (invocation.Data.GetValue() |> continuation).Invoke(&sm)
                else
                    invocation.Hijack()
                    false
            else
                // Dynamic Bind.

                let mutable invocation = code.GetInvocation()

                let cont =
                    CancellableResumptionFunc<'Data>(fun sm -> (invocation.Data.GetValue() |> continuation).Invoke(&sm))

                invocation.Hijack()
                sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                false)

    member inline this.ReturnFrom(comp: Cancellable<'T>) : CancellableCode<'T, 'T> = this.Bind(comp, this.Return)

    member inline _.Run(code: CancellableCode<'T, 'T>) : Cancellable<'T> =
        if __useResumableCode then
            __stateMachine<CancellableData<'T>, Cancellable<'T>>

                (MoveNextMethodImpl<_>(fun sm ->
                    __resumeAt sm.ResumptionPoint

                    try
                        let __stack_code_fin = (CancellableCode.WithCancelCheck code).Invoke(&sm)

                        if __stack_code_fin then
                            sm.ResumptionPoint <- -1
                    with exn ->
                        sm.Data.Result <- Error(ExceptionDispatchInfo.Capture exn)
                        sm.ResumptionPoint <- -1))

                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))

                (AfterCode<_, _>(fun sm ->
                    sm.Data <- CancellableData()
                    Cancellable(CancellableInvocation<_, _>(sm))))
        else
            // Dynamic Run.

            let initialResumptionFunc =
                CancellableResumptionFunc(fun sm -> (CancellableCode.WithCancelCheck code).Invoke(&sm))

            let resumptionInfo =
                { new CancellableResumptionDynamicInfo<_>(initialResumptionFunc) with
                    member info.MoveNext(sm) =
                        try
                            if info.ResumptionFunc.Invoke(&sm) then
                                sm.ResumptionPoint <- -1
                        with exn ->
                            sm.Data.Result <- Error(ExceptionDispatchInfo.Capture exn)
                            sm.ResumptionPoint <- -1

                    member _.SetStateMachine(_, _) = ()
                }

            let sm =
                CancellableStateMachine(ResumptionDynamicInfo = resumptionInfo, Data = CancellableData())

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
        Trampoline.CurrentThreadTrampoline.Execute invocation
        invocation

    let runWithoutCancellation code =
        run CancellationToken.None code |> _.Data.GetValue()

    let toAsync (code: Cancellable<_>) =
        async {
            let! ct = Async.CancellationToken

            return!
                Async.FromContinuations(fun (cont, econt, ccont) ->
                    match run ct code |> _.Data.Result with
                    | Ok value -> cont value
                    | Error edi ->
                        match edi.SourceException with
                        | :? OperationCanceledException as oce when oce.CancellationToken = ct -> ccont oce
                        | exn -> econt exn)
        }

    let token () =
        cancellable { FSharp.Compiler.Cancellable.Token }
