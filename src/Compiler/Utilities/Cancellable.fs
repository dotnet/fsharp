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
    val mutable Result: 'T

and CancellableStateMachine<'TOverall> = ResumableStateMachine<CancellableStateMachineData<'TOverall>>
and ICancellableStateMachine<'TOverall> = IResumableStateMachine<CancellableStateMachineData<'TOverall>>
and CancellableResumptionFunc<'TOverall> = ResumptionFunc<CancellableStateMachineData<'TOverall>>
and CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<CancellableStateMachineData<'TOverall>>
and CancellableCode<'TOverall, 'T> = ResumableCode<CancellableStateMachineData<'TOverall>, 'T>

[<Sealed>]
type Trampoline(cancellationToken: CancellationToken) =
    let mutable bindDepth = 0

    let stack = Collections.Generic.Stack<ITrampolineInvocation>()

    static let current = new ThreadLocal<Trampoline>()

    member this.IsCancelled = cancellationToken.IsCancellationRequested

    member this.ThrowIfCancellationRequested() =
        cancellationToken.ThrowIfCancellationRequested()

    member this.ShoudBounce = bindDepth % 100 = 0

    static member Install ct = current.Value <- Trampoline ct

    member _.Set(invocation) = stack.Push(invocation)

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
    abstract Result: 'T

type ICancellableInvokable<'T> =
    abstract Create: unit -> ITrampolineInvocation<'T>

[<NoEquality; NoComparison>]
type CancellableInvocation<'T, 'Machine when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>>(machine: 'Machine)
    =

    let mutable machine = machine

    interface ITrampolineInvocation<'T> with
        member _.MoveNext() = machine.MoveNext()
        member _.IsCompleted = machine.ResumptionPoint = -1
        member _.Result = machine.Data.Result

    interface ICancellableInvokable<'T> with
        member _.Create() = CancellableInvocation<_, _>(machine)

[<Struct; NoComparison>]
type Cancellable<'T>(invokable: ICancellableInvokable<'T>) =

    member _.GetInvocation() = invokable.Create()

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

type CancellableBuilder() =

    member inline _.Delay(generator: unit -> CancellableCode<'TOverall, 'T>) : CancellableCode<'TOverall, 'T> =
        ResumableCode.Delay(fun () -> generator () |> throwIfCancellationRequested)

    [<DefaultValue>]
    member inline _.Zero() : CancellableCode<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode<'T, _>(fun sm ->
            sm.Data.Result <- value
            true)

    member inline _.Combine
        (code1: CancellableCode<'TOverall, unit>, code2: CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.Combine(code1, code2)

    member inline _.While
        ([<InlineIfLambda>] condition: unit -> bool, body: CancellableCode<'TOverall, unit>)
        : CancellableCode<'TOverall, unit> =
        ResumableCode.While(condition, throwIfCancellationRequested body)

    member inline _.TryWith
        (body: CancellableCode<'TOverall, 'T>, catch: exn -> CancellableCode<'TOverall, 'T>)
        : CancellableCode<'TOverall, 'T> =
        ResumableCode.TryWith(body, filterCancellation catch)

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
                    | None -> (continuation invocation.Result).Invoke(&sm)
                else
                    Trampoline.Current.Execute invocation
                    (continuation invocation.Result).Invoke(&sm)

            else
                // Dynamic Bind.

                let mutable invocation = code.GetInvocation()

                if Trampoline.Current.ShoudBounce then
                    let cont =
                        CancellableResumptionFunc<'Data>(fun sm -> (continuation invocation.Result).Invoke(&sm))

                    Trampoline.Current.Set invocation
                    sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                    false
                else
                    Trampoline.Current.Execute invocation
                    (continuation invocation.Result).Invoke(&sm))
        |> throwIfCancellationRequested

    member inline this.ReturnFrom(comp: Cancellable<'T>) : CancellableCode<'T, 'T> = this.Bind(comp, this.Return)

    member inline _.Run(code: CancellableCode<'T, 'T>) : Cancellable<'T> =
        if __useResumableCode then
            __stateMachine<_, _>

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
        Trampoline.Install ct
        Trampoline.Current.Execute invocation
        invocation

    let runWithoutCancellation code =
        code |> run CancellationToken.None |> _.Result

    let toAsync code =
        async {
            let! ct = Async.CancellationToken

            return run ct code |> _.Result
        }

    let token () =
        cancellable { FSharp.Compiler.Cancellable.Token }
