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
open System.Diagnostics

type ITrampolineInvocation =
    abstract member MoveNext: unit -> unit
    abstract IsCompleted: bool

[<Struct; NoComparison; NoEquality>]
type CancellableStateMachineData<'T> =

    [<DefaultValue(false)>]
    val mutable Result: 'T

and CancellableStateMachine<'TOverall> = ResumableStateMachine<CancellableStateMachineData<'TOverall>>
and ICancellableStateMachine<'TOverall> = IResumableStateMachine<CancellableStateMachineData<'TOverall>>
and CancellableResumptionFunc<'TOverall> = ResumptionFunc<CancellableStateMachineData<'TOverall>>
and CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<CancellableStateMachineData<'TOverall>>
and CancellableCode<'TOverall, 'T> = ResumableCode<CancellableStateMachineData<'TOverall>, 'T>

[<Sealed>]
type Trampoline() =

    [<DefaultValue(false)>]
    val mutable Token: CancellationToken

    [<DefaultValue(false)>]
    val mutable Exception: ExceptionDispatchInfo voption

    [<DefaultValue(false)>]
    val mutable BindDepth: int

    static let current = new ThreadLocal<Trampoline>()

    let stack = System.Collections.Generic.Stack<ITrampolineInvocation>()

    static member IsCancelled = current.Value.Token.IsCancellationRequested
    static member HasError = current.Value.Exception.IsSome

    static member Good =
        not (current.Value.Token.IsCancellationRequested || current.Value.Exception.IsSome)

    static member ThrowIfCancellationRequested() =
        current.Value.Token.ThrowIfCancellationRequested()

    static member ShoudBounce = current.Value.BindDepth % 100 = 0

    static member Install() = current.Value <- Trampoline()

    member _.Set(invocation: ITrampolineInvocation) = stack.Push(invocation)

    [<DebuggerHidden>]
    member this.Execute(invocation) =
        this.BindDepth <- this.BindDepth + 1

        stack.Push invocation

        while not invocation.IsCompleted do
            stack.Peek().MoveNext()

            if stack.Peek().IsCompleted then
                stack.Pop() |> ignore

        this.BindDepth <- this.BindDepth - 1

    static member Current = current.Value

type ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: 'T

type IMachineTemplateWrapper<'T> =
    abstract Clone: unit -> ITrampolineInvocation<'T>

[<NoEquality; NoComparison>]
type CancellableInvocation<'T, 'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>>(machine: 'Machine)
    =

    let mutable machine = machine

    interface ITrampolineInvocation<'T> with
        member _.MoveNext() = machine.MoveNext()
        member _.IsCompleted = machine.ResumptionPoint = -1
        member _.Result = machine.Data.Result

    interface IMachineTemplateWrapper<'T> with
        member _.Clone() = CancellableInvocation<_, _>(machine)

[<Struct; NoComparison>]
type Cancellable<'T>(template: IMachineTemplateWrapper<'T>) =

    member _.GetInvocation() = template.Clone()

[<AutoOpen>]
module CancellableCode =

    let inline captureExn (exn: exn) =
        match exn with
        | :? OperationCanceledException as oce when oce.CancellationToken = Trampoline.Current.Token -> ()
        | exn -> Trampoline.Current.Exception <- ValueSome(ExceptionDispatchInfo.Capture exn)

        Unchecked.defaultof<_>

    let inline captureStackFrame () =
        try
            Trampoline.Current.Exception |> ValueOption.iter _.Throw()
        with exn ->
            Trampoline.Current.Exception <- ValueSome <| ExceptionDispatchInfo.Capture exn

    let inline protect (code: CancellableCode<_, _>) =
        CancellableCode(fun sm ->
            try
                code.Invoke(&sm)
            with exn ->
                captureExn exn
                true)

    let inline notWhenCancelled (code: CancellableCode<_, _>) =
        CancellableCode(fun sm -> Trampoline.IsCancelled || (protect code).Invoke(&sm))

    let inline notWhenError (code: CancellableCode<_, _>) =
        CancellableCode(fun sm -> Trampoline.HasError || (protect code).Invoke(&sm))

    let inline whenGood (code: CancellableCode<_, _>) =
        CancellableCode(fun sm -> Trampoline.HasError || Trampoline.IsCancelled || (protect code).Invoke(&sm))

    let inline whenGoodApply (code: _ -> CancellableCode<_, _>) arg =
        CancellableCode(fun sm ->
            Trampoline.HasError
            || Trampoline.IsCancelled
            || (code arg |> protect).Invoke(&sm))

    let inline throwIfCancellationRequested (code: CancellableCode<_, _>) =
        CancellableCode(fun sm ->
            Trampoline.Current.Token.ThrowIfCancellationRequested()
            code.Invoke(&sm))

type CancellableBuilder() =

    member inline _.Delay(generator: unit -> CancellableCode<'TOverall, 'T>) : CancellableCode<'TOverall, 'T> =
        ResumableCode.Delay(generator) |> protect

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : CancellableCode<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode<'T, _>(fun sm ->
            sm.Data.Result <- value
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine
        (code1: CancellableCode<'TOverall, unit>, code2: CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.Combine(notWhenCancelled code1, whenGood code2) |> protect

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While
        ([<InlineIfLambda>] condition: unit -> bool, body: CancellableCode<'TOverall, unit>)
        : CancellableCode<'TOverall, unit> =
        ResumableCode.While(condition, throwIfCancellationRequested body) |> protect

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith(body: CancellableCode<'TOverall, 'T>, catch: exn -> CancellableCode<'TOverall, 'T>) =
        CancellableCode<'TOverall, 'T>(fun sm ->
            let mutable __stack_fin = true
            let __stack_body_fin = (protect body).Invoke(&sm)
            __stack_fin <- __stack_body_fin

            if __stack_fin && Trampoline.HasError then
                let __stack_filtered_exn = Trampoline.Current.Exception.Value.SourceException
                // Clear for now, will get restored if not handled.
                Trampoline.Current.Exception <- ValueNone
                __stack_fin <- (catch __stack_filtered_exn |> protect |> notWhenCancelled).Invoke(&sm)

            __stack_fin)

    /// Wraps a step in a try/finally. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryFinally
        (body: CancellableCode<'TOverall, 'T>, [<InlineIfLambda>] compensation: unit -> unit)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.TryFinally(
            body,
            ResumableCode<_, _>(fun _sm ->
                compensation ()
                true)
        )

    member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable | null>
        (resource: 'Resource, body: 'Resource -> CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.Using(resource, whenGoodApply body) |> protect

    member inline _.For(sequence: seq<'T>, body: 'T -> CancellableCode<'TOverall, unit>) : CancellableCode<'TOverall, unit> =
        ResumableCode.For(sequence, fun x -> body x |> throwIfCancellationRequested)
        |> protect

    member inline this.Yield(value) = this.Return(value)

    member inline _.Bind
        (code: Cancellable<'U>, [<InlineIfLambda>] continuation: 'U -> CancellableCode<'Data, 'T>)
        : CancellableCode<'Data, 'T> =
        CancellableCode(fun sm ->
            if __useResumableCode then
                let mutable invocation = code.GetInvocation()

                if Trampoline.ShoudBounce then
                    match __resumableEntry () with
                    | Some contID ->
                        sm.ResumptionPoint <- contID
                        Trampoline.Current.Set invocation
                        false
                    | None -> (invocation.Result |> continuation |> whenGood).Invoke(&sm)
                else
                    Trampoline.Current.Execute invocation
                    (invocation.Result |> continuation |> whenGood).Invoke(&sm)

            else
                // Dynamic Bind.

                let mutable invocation = code.GetInvocation()

                if Trampoline.ShoudBounce then
                    let cont =
                        CancellableResumptionFunc<'Data>(fun sm -> (whenGoodApply continuation invocation.Result).Invoke(&sm))

                    Trampoline.Current.Set invocation
                    sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                    false
                else
                    Trampoline.Current.Execute invocation
                    (whenGoodApply continuation invocation.Result).Invoke(&sm))

    member inline this.ReturnFrom(comp: Cancellable<'T>) : CancellableCode<'T, 'T> = this.Bind(comp, this.Return)

    member inline _.Run(code: CancellableCode<'T, 'T>) : Cancellable<'T> =
        if __useResumableCode then
            __stateMachine<_, _>

                (MoveNextMethodImpl<_>(fun sm ->
                    __resumeAt sm.ResumptionPoint
                    let __stack_code_fin = (protect code).Invoke(&sm)

                    if __stack_code_fin then
                        captureStackFrame ()
                        sm.ResumptionPoint <- -1))

                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))

                (AfterCode<_, _>(fun sm -> Cancellable(CancellableInvocation(sm))))
        else
            // Dynamic Run.

            let initialResumptionFunc =
                CancellableResumptionFunc(fun sm -> (protect code).Invoke(&sm))

            let resumptionInfo =
                { new CancellableResumptionDynamicInfo<_>(initialResumptionFunc) with
                    member info.MoveNext(sm) =
                        if info.ResumptionFunc.Invoke(&sm) then
                            captureStackFrame ()
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
        Trampoline.Install()
        Trampoline.Current.Execute invocation
        invocation

    let runWithoutCancellation code =
        let invocation = run CancellationToken.None code

        if Trampoline.IsCancelled then
            raise (OperationCanceledException Trampoline.Current.Token)
        elif Trampoline.HasError then
            Trampoline.Current.Exception.Value.Throw()
            Unchecked.defaultof<_>
        else
            invocation.Result

    let toAsync (code: Cancellable<_>) =
        async {
            let! ct = Async.CancellationToken

            return!
                Async.FromContinuations(fun (cont, econt, ccont) ->
                    let invocation = run ct code

                    if Trampoline.IsCancelled then
                        ccont (OperationCanceledException Trampoline.Current.Token)
                    elif Trampoline.HasError then
                        econt Trampoline.Current.Exception.Value.SourceException
                    else
                        cont invocation.Result)
        }

    let token () =
        cancellable { FSharp.Compiler.Cancellable.Token }
