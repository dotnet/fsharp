namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    static member internal UseToken: unit -> Async<IDisposable>

    static member HasCancellationToken: bool
    static member Token: CancellationToken

    static member CheckAndThrow: unit -> unit
    static member TryCheckAndThrow: unit -> unit

namespace Internal.Utilities.Library.CancellableImplementation

open System
open Microsoft.FSharp.Core.CompilerServices
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices

type internal ITrampolineInvocation =

    abstract MoveNext: unit -> unit

    abstract IsCompleted: bool

and [<Sealed>] internal Trampoline = class end

[<NoComparison; NoEquality; Struct>]
type internal CancellableData<'T> =

    [<DefaultValue(false)>]
    val mutable Result: Result<'T, ExceptionDispatchInfo>

    member GetValue: unit -> 'T

type internal ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation

    abstract Hijack: unit -> unit

    abstract Data: CancellableData<'T>

type internal IMachineTemplateWrapper<'T> =
    abstract Clone: unit -> ITrampolineInvocation<'T>


type internal ICancellableStateMachine<'T> = IResumableStateMachine<CancellableData<'T>>

type internal CancellableStateMachine<'T> = ResumableStateMachine<CancellableData<'T>>

type internal CancellableResumptionFunc<'T> = ResumptionFunc<CancellableData<'T>>

type internal CancellableResumptionDynamicInfo<'T> = ResumptionDynamicInfo<CancellableData<'T>>

type internal CancellableCode<'Data, 'T> = ResumableCode<CancellableData<'Data>, 'T>

[<NoEquality; NoComparison>]
type internal CancellableInvocation<'T, 'Machine
    when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>> =
    interface IMachineTemplateWrapper<'T>
    interface ITrampolineInvocation<'T>

    new: machine: 'Machine -> CancellableInvocation<'T, 'Machine>

[<NoComparison; Struct>]
type internal Cancellable<'T> =

    new: template: IMachineTemplateWrapper<'T> -> Cancellable<'T>

    member GetInvocation: unit -> ITrampolineInvocation<'T>

module internal CancellableCode =

    val inline WithCancelCheck: body: CancellableCode<'Data, 'T> -> CancellableCode<'Data, 'T>

    val inline FilterOce:
        [<InlineIfLambda>] catch: (exn -> CancellableCode<'Data, 'T>) -> exn: exn -> CancellableCode<'Data, 'T>

type internal CancellableBuilder =

    new: unit -> CancellableBuilder

    member inline Bind:
        code: Cancellable<'U> * [<InlineIfLambda>] continuation: ('U -> CancellableCode<'Data, 'T>) ->
            CancellableCode<'Data, 'T>

    member inline Combine:
        code1: CancellableCode<'c, unit> * code2: CancellableCode<'c, 'd> -> ResumableCode<CancellableData<'c>, 'd>

    member inline Delay: generator: (unit -> CancellableCode<'Data, 'T>) -> CancellableCode<'Data, 'T>

    member inline For: sequence: 'e seq * body: ('e -> CancellableCode<'Data, unit>) -> CancellableCode<'Data, unit>

    member inline Return: value: 'T -> CancellableCode<'T, 'T>

    member inline ReturnFrom: comp: Cancellable<'T> -> CancellableCode<'T, 'T>

    member inline Run: code: CancellableCode<'T, 'T> -> Cancellable<'T>

    member inline TryFinally:
        body: CancellableCode<'Data, 'T> * compensation: (unit -> unit) -> CancellableCode<'Data, 'T>

    member inline TryWith:
        body: CancellableCode<'Data, 'T> * catch: (exn -> CancellableCode<'Data, 'T>) -> CancellableCode<'Data, 'T>

    member inline Using:
        resource: 'b * body: ('b -> CancellableCode<'Data, 'T>) -> CancellableCode<'Data, 'T>
            when 'b :> IDisposable | null

    member inline While: condition: (unit -> bool) * body: CancellableCode<'Data, unit> -> CancellableCode<'Data, unit>

    member inline Yield: value: 'a -> CancellableCode<'a, 'a>

    member inline Zero: unit -> CancellableCode<'Data, unit>

namespace Internal.Utilities.Library

open System.Threading

type internal Cancellable<'T> = CancellableImplementation.Cancellable<'T>

[<AutoOpen>]
module internal CancellableAutoOpens =

    val cancellable: CancellableImplementation.CancellableBuilder

module internal Cancellable =

    val runWithoutCancellation: code: Cancellable<'a> -> 'a

    val toAsync: code: Cancellable<'a> -> Async<'a>

    val token: unit -> Cancellable<CancellationToken>
