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
    val mutable Result: Result<'T, exn>

and CancellableStateMachine<'TOverall> = ResumableStateMachine<CancellableStateMachineData<'TOverall>>
and ICancellableStateMachine<'TOverall> = IResumableStateMachine<CancellableStateMachineData<'TOverall>>
and CancellableResumptionFunc<'TOverall> = ResumptionFunc<CancellableStateMachineData<'TOverall>>
and CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<CancellableStateMachineData<'TOverall>>
and CancellableCode<'TOverall, 'T> = ResumableCode<CancellableStateMachineData<'TOverall>, 'T>

[<Sealed>]
type Trampoline(cancellationToken: CancellationToken) =
    let mutable bindDepth = 0
    let mutable storedException: ExceptionDispatchInfo voption = ValueNone
    let mutable capturedFramesCount = 0

    let captureStackFrame exn =
        match storedException with
        | ValueSome edi when edi.SourceException = exn ->
            try
                edi.Throw()
                Unchecked.defaultof<_>
            with exn ->
                capturedFramesCount <- capturedFramesCount + 1
                let edi = ExceptionDispatchInfo.Capture exn
                storedException <- ValueSome edi
                edi.SourceException
        | _ ->
            capturedFramesCount <- 1
            let edi = ExceptionDispatchInfo.Capture exn
            storedException <- ValueSome edi
            edi.SourceException

    let stack = System.Collections.Generic.Stack<ITrampolineInvocation>()

    static let current = new ThreadLocal<Trampoline>()

    member this.IsCancelled = cancellationToken.IsCancellationRequested

    member this.ThrowIfCancellationRequested() =
        cancellationToken.ThrowIfCancellationRequested()

    member this.ShoudBounce = bindDepth % 100 = 0

    member this.CaptureStackFrame(exn) =
        if not this.IsCancelled && (bindDepth < 100 || capturedFramesCount < 200) then
            captureStackFrame exn
        else
            exn

    static member Install ct = current.Value <- Trampoline ct

    member _.Set(invocation: ITrampolineInvocation) = stack.Push(invocation)

    [<DebuggerHidden>]
    member this.Execute(invocation) =
        bindDepth <- bindDepth + 1

        stack.Push invocation

        while not invocation.IsCompleted do
            stack.Peek().MoveNext()

            if stack.Peek().IsCompleted then
                stack.Pop() |> ignore

        bindDepth <- bindDepth - 1

    static member Current = current.Value

type ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: Result<'T, exn>

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

    let inline filterCancellation (catch: exn -> CancellableCode<_, _>) exn =
        CancellableCode(fun sm ->
            try
                (catch exn).Invoke(&sm)
            with :? OperationCanceledException when Trampoline.Current.IsCancelled ->
                true)

    let inline throwIfCancellationRequested (code: CancellableCode<_, _>) =
        CancellableCode(fun sm ->
            Trampoline.Current.ThrowIfCancellationRequested()
            code.Invoke(&sm))

    let inline getResult (invocation: ITrampolineInvocation<_>) =
        match invocation.Result with
        | Ok value -> value
        | Error exn -> raise exn

type CancellableBuilder() =

    member inline _.Delay(generator: unit -> CancellableCode<'TOverall, 'T>) : CancellableCode<'TOverall, 'T> =
        ResumableCode.Delay(fun () -> generator () |> throwIfCancellationRequested)

    /// Used to represent no-ops like the implicit empty "else" branch of an "if" expression.
    [<DefaultValue>]
    member inline _.Zero() : CancellableCode<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode<'T, _>(fun sm ->
            sm.Data.Result <- Ok value
            true)

    /// Chains together a step with its following step.
    /// Note that this requires that the first step has no result.
    /// This prevents constructs like `task { return 1; return 2; }`.
    member inline _.Combine
        (code1: CancellableCode<'TOverall, unit>, code2: CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.Combine(code1, code2)

    /// Builds a step that executes the body while the condition predicate is true.
    member inline _.While
        ([<InlineIfLambda>] condition: unit -> bool, body: CancellableCode<'TOverall, unit>)
        : CancellableCode<'TOverall, unit> =
        ResumableCode.While(condition, throwIfCancellationRequested body)

    /// Wraps a step in a try/with. This catches exceptions both in the evaluation of the function
    /// to retrieve the step, and in the continuation of the step (if any).
    member inline _.TryWith
        (body: CancellableCode<'TOverall, 'T>, catch: exn -> CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.TryWith(body, filterCancellation catch)

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
        ResumableCode.Using(resource, body)

    member inline _.For(sequence: seq<'T>, body: 'T -> CancellableCode<'TOverall, unit>) : CancellableCode<'TOverall, unit> =
        ResumableCode.For(sequence, fun x -> body x |> throwIfCancellationRequested)

    member inline this.Yield(value) = this.Return(value)

    member inline _.Bind
        (code: Cancellable<'U>, [<InlineIfLambda>] continuation: 'U -> CancellableCode<'Data, 'T>)
        : CancellableCode<'Data, 'T> =
        CancellableCode(fun sm ->
            if __useResumableCode then
                let mutable invocation = code.GetInvocation()

                if Trampoline.Current.ShoudBounce then
                    match __resumableEntry () with
                    | Some contID ->
                        sm.ResumptionPoint <- contID
                        Trampoline.Current.Set invocation
                        false
                    | None -> (invocation |> getResult |> continuation).Invoke(&sm)
                else
                    Trampoline.Current.Execute invocation
                    (invocation |> getResult |> continuation).Invoke(&sm)

            else
                // Dynamic Bind.

                let mutable invocation = code.GetInvocation()

                if Trampoline.Current.ShoudBounce then
                    let cont =
                        CancellableResumptionFunc<'Data>(fun sm -> (invocation |> getResult |> continuation).Invoke(&sm))

                    Trampoline.Current.Set invocation
                    sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                    false
                else
                    Trampoline.Current.Execute invocation
                    (invocation |> getResult |> continuation).Invoke(&sm))

    member inline this.ReturnFrom(comp: Cancellable<'T>) : CancellableCode<'T, 'T> = this.Bind(comp, this.Return)

    member inline _.Run(code: CancellableCode<'T, 'T>) : Cancellable<'T> =
        if __useResumableCode then
            __stateMachine<_, _>

                (MoveNextMethodImpl<_>(fun sm ->
                    __resumeAt sm.ResumptionPoint

                    try
                        let __stack_code_fin = code.Invoke(&sm)

                        if __stack_code_fin then
                            sm.ResumptionPoint <- -1
                    with exn ->
                        sm.Data.Result <- Error <| Trampoline.Current.CaptureStackFrame exn
                        sm.ResumptionPoint <- -1))

                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))

                (AfterCode<_, _>(fun sm -> Cancellable(CancellableInvocation(sm))))
        else
            // Dynamic Run.

            let initialResumptionFunc = CancellableResumptionFunc(fun sm -> code.Invoke(&sm))

            let resumptionInfo =
                { new CancellableResumptionDynamicInfo<_>(initialResumptionFunc) with
                    member info.MoveNext(sm) =
                        try
                            if info.ResumptionFunc.Invoke(&sm) then
                                sm.ResumptionPoint <- -1
                        with exn ->
                            sm.Data.Result <- Error <| Trampoline.Current.CaptureStackFrame exn
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
        Trampoline.Install ct
        Trampoline.Current.Execute invocation
        invocation

    let runWithoutCancellation code =
        let invocation = run CancellationToken.None code

        if Trampoline.Current.IsCancelled then
            failwith "Unexpected cancellation in Cancellable.runWithoutCancellation"
        else
            getResult invocation

    let toAsync code =
        async {
            let! ct = Async.CancellationToken

            return!
                Async.FromContinuations(fun (cont, econt, ccont) ->
                    match run ct code |> _.Result with
                    | _ when Trampoline.Current.IsCancelled -> ccont (OperationCanceledException ct)
                    | Ok value -> cont value
                    | Error exn -> econt exn)
        }

    let token () =
        cancellable { FSharp.Compiler.Cancellable.Token }
