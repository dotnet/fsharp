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
    static let tokenHolder = AsyncLocal<CancellationToken>()

    static member HasCancellationToken = tokenHolder.Value <> CancellationToken.None

    static member Token = tokenHolder.Value

    static member UseToken() =
        async {
            let! ct = Async.CancellationToken
            return Cancellable.UsingToken ct
        }

    static member UsingToken(ct) =
        let oldCt = tokenHolder.Value
        tokenHolder.Value <- ct

        { new IDisposable with
            member _.Dispose() = tokenHolder.Value <- oldCt
        }

    static member CheckAndThrow() =
        tokenHolder.Value.ThrowIfCancellationRequested()

    static member TryCheckAndThrow() =
        tokenHolder.Value.ThrowIfCancellationRequested()

namespace Internal.Utilities.Library.CancellableImplementation

type Cancellable = FSharp.Compiler.Cancellable

open System
open System.Threading

open FSharp.Core.CompilerServices.StateMachineHelpers

open Microsoft.FSharp.Core.CompilerServices
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices

type ITrampolineInvocation =
    abstract member MoveNext: unit -> bool
    abstract IsCompleted: bool

type internal CancellableStateMachine<'TOverall> = ResumableStateMachine<'TOverall>
type internal ICancellableStateMachine<'TOverall> = IResumableStateMachine<'TOverall>
type internal CancellableResumptionFunc<'TOverall> = ResumptionFunc<'TOverall>
type internal CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<'TOverall>
type internal CancellableCode<'TOverall, 'T> = ResumableCode<'TOverall, 'T>

[<Struct; NoComparison; NoEquality>]
type PendingInvocation =
    | Delayed of ITrampolineInvocation
    | Immediate of ITrampolineInvocation

[<Sealed>]
type Trampoline() =

    let mutable bindDepth = 0

    [<Literal>]
    static let bindDepthLimit = 100

    static let current = new AsyncLocal<Trampoline voption>()

    let pending = System.Collections.Generic.Stack<_>()

    let mutable lastError: ExceptionDispatchInfo voption = ValueNone
    let mutable storedError: ExceptionDispatchInfo voption = ValueNone

    member _.ReplayException() =
        match storedError with
        | ValueSome edi ->
            storedError <- ValueNone
            edi.Throw()
        | _ -> ()

    member this.ShoudBounce = bindDepth % bindDepthLimit = 0

    member this.SetDelayed(invocation) = pending.Push(Delayed invocation)

    member this.RunImmediate(invocation: ITrampolineInvocation) =
        let captureException exn =
            match lastError with
            | ValueSome edi when edi.SourceException = exn -> ()
            | _ -> lastError <- ValueSome <| ExceptionDispatchInfo.Capture exn

            storedError <- lastError

        bindDepth <- bindDepth + 1

        pending.Push(Immediate invocation)

        try
            while not invocation.IsCompleted do
                match pending.Peek() with
                | Immediate i ->
                    if i.MoveNext() then
                        pending.Pop() |> ignore
                | Delayed d ->
                    try
                        if d.MoveNext() then
                            pending.Pop() |> ignore
                    with exn ->
                        pending.Pop() |> ignore
                        captureException exn

            this.ReplayException()
        finally
            bindDepth <- bindDepth - 1

    static member Current = current.Value.Value

    static member Install() =
        current.Value <- ValueSome <| Trampoline()

type ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: 'T

[<NoEquality; NoComparison>]
type ICancellableInvokable<'T> =
    abstract Create: unit -> ITrampolineInvocation<'T>

[<NoEquality; NoComparison>]
type CancellableInvocation<'T, 'Machine
    when 'Machine: struct and 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>>(machine: 'Machine) =
    let mutable machine = machine

    interface ITrampolineInvocation<'T> with
        member _.MoveNext() =
            machine.MoveNext()
            machine.ResumptionPoint = -1

        member _.Result = machine.Data
        member _.IsCompleted = machine.ResumptionPoint = -1

[<Struct; NoComparison>]
type Cancellable<'T>(clone: unit -> ITrampolineInvocation<'T>) =

    member _.GetInvocation() = clone ()

[<AutoOpen>]
module CancellableCode =

    let inline filterCancellation (catch: exn -> CancellableCode<_, _>) (exn: exn) =
        CancellableCode(fun sm ->
            match exn with
            | :? OperationCanceledException as oce when oce.CancellationToken = Cancellable.Token -> raise exn
            | _ -> (catch exn).Invoke(&sm))

    let inline throwIfCancellationRequested (code: CancellableCode<_, _>) =
        CancellableCode(fun sm ->
            Cancellable.Token.ThrowIfCancellationRequested()
            code.Invoke(&sm))

type CancellableBuilder() =

    member inline _.Delay(generator: unit -> CancellableCode<'TOverall, 'T>) : CancellableCode<'TOverall, 'T> =
        ResumableCode.Delay(fun () -> generator () |> throwIfCancellationRequested)

    [<DefaultValue>]
    member inline _.Zero() : CancellableCode<'TOverall, unit> = ResumableCode.Zero()

    member inline _.Return(value: 'T) : CancellableCode<'T, 'T> =
        CancellableCode<'T, _>(fun sm ->
            sm.Data <- value
            true)
        |> throwIfCancellationRequested

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
                    // Suspend this state machine and schedule both parts to run on the trampoline.
                    match __resumableEntry () with
                    // Suspending
                    | Some contID ->
                        sm.ResumptionPoint <- contID
                        Trampoline.Current.SetDelayed invocation
                        false
                    // Resuming
                    | None ->
                        Trampoline.Current.ReplayException()
                        (continuation invocation.Result).Invoke(&sm)
                else
                    Trampoline.Current.RunImmediate invocation
                    (continuation invocation.Result).Invoke(&sm)

            else
                // Dynamic Bind.
                let mutable invocation = code.GetInvocation()

                if Trampoline.Current.ShoudBounce then
                    let cont =
                        CancellableResumptionFunc<'Data>(fun sm ->
                            Trampoline.Current.ReplayException()
                            (continuation invocation.Result).Invoke(&sm))

                    Trampoline.Current.SetDelayed invocation
                    sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                    false
                else
                    Trampoline.Current.RunImmediate invocation
                    (continuation invocation.Result).Invoke(&sm))

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

                (AfterCode<_, _>(fun sm ->
                    let copy = sm
                    Cancellable(fun () -> CancellableInvocation(copy))))
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
            Cancellable(fun () -> CancellableInvocation(sm))

namespace Internal.Utilities.Library

open System.Threading

type Cancellable<'T> = CancellableImplementation.Cancellable<'T>

[<AutoOpen>]
module CancellableAutoOpens =

    let cancellable = CancellableImplementation.CancellableBuilder()

module Cancellable =
    open Internal.Utilities.Library.CancellableImplementation

    let run (code: Cancellable<_>) =
        let invocation = code.GetInvocation()
        Trampoline.Install()
        Trampoline.Current.RunImmediate invocation
        invocation.Result

    let runWithoutCancellation code =
        use _ = Cancellable.UsingToken CancellationToken.None
        run code

    let toAsync code =
        async {
            use! _holder = Cancellable.UseToken()
            return run code
        }

    let token () =
        cancellable { Cancellable.Token }
