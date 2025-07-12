namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    static member internal UseToken: unit -> Async<IDisposable>
    static member internal UsingToken: CancellationToken -> IDisposable
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
    abstract ReplayExceptionIfStored: unit -> unit

[<Struct; NoComparison; NoEquality>]
type internal CancellableStateMachineData<'T> =
    val mutable Result: 'T
    val mutable NextInvocation: ITrampolineInvocation voption

and internal CancellableStateMachine<'TOverall> = ResumableStateMachine<CancellableStateMachineData<'TOverall>>
and internal ICancellableStateMachine<'TOverall> = IResumableStateMachine<CancellableStateMachineData<'TOverall>>
and internal CancellableResumptionFunc<'TOverall> = ResumptionFunc<CancellableStateMachineData<'TOverall>>
and internal CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<CancellableStateMachineData<'TOverall>>
and internal CancellableCode<'TOverall, 'T> = ResumableCode<CancellableStateMachineData<'TOverall>, 'T>

type internal ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: 'T

type internal ICancellableInvokable<'T> =
    abstract Create: bool -> ITrampolineInvocation<'T>

[<Sealed>]
type internal Trampoline =
    member RunDelayed: ITrampolineInvocation * ITrampolineInvocation -> unit
    member RunImmediate: ITrampolineInvocation -> unit
    static member Current: Trampoline
    member IsCancelled: bool
    member ThrowIfCancellationRequested: unit -> unit
    member ShoudBounce: bool

[<NoEquality; NoComparison>]
type internal CancellableInvocation<'T, 'Machine
    when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>> =
    interface ICancellableInvokable<'T>
    interface ITrampolineInvocation<'T>
    new: machine: 'Machine -> CancellableInvocation<'T, 'Machine>

[<Struct; NoComparison; NoEquality>]
type internal Cancellable<'T> =
    new: invokable: ICancellableInvokable<'T> -> Cancellable<'T>
    member GetInvocation: bool -> ITrampolineInvocation<'T>

[<AutoOpen>]
module internal ExceptionDispatchInfoHelpers =
    type ExceptionDispatchInfo with
        member ThrowAny: unit -> 'T
        static member RestoreOrCapture: exn -> ExceptionDispatchInfo

type internal CancellableBuilder =
    new: unit -> CancellableBuilder

    member inline Bind:
        code: Cancellable<'U> * [<InlineIfLambda>] continuation: ('U -> CancellableCode<'Data, 'T>) ->
            CancellableCode<'Data, 'T>

    member inline Combine:
        code1: CancellableCode<'Data, unit> * code2: CancellableCode<'Data, 'T> -> CancellableCode<'Data, 'T>

    member inline Delay: generator: (unit -> CancellableCode<'Data, 'T>) -> CancellableCode<'Data, 'T>
    member inline For: sequence: 'e seq * body: ('e -> CancellableCode<'Data, unit>) -> CancellableCode<'Data, unit>
    member inline Return: value: 'T -> CancellableCode<'T, 'T>
    member inline ReturnFrom: comp: Cancellable<'T> -> CancellableCode<'T, 'T>
    member inline Run: code: CancellableCode<'T, 'T> -> Cancellable<'T>

    member inline TryFinally:
        body: CancellableCode<'Data, 'T> * compensation: (unit -> unit) -> CancellableCode<'Data, 'T>

    member inline TryWith:
        body: CancellableCode<'Data, 'T> * catch: (exn -> CancellableCode<'Data, 'T>) -> CancellableCode<'Data, 'T>

    member inline While: condition: (unit -> bool) * body: CancellableCode<'Data, unit> -> CancellableCode<'Data, unit>
    member inline Yield: value: 'a -> CancellableCode<'a, 'a>
    member inline Zero: unit -> CancellableCode<'Data, unit>

    member inline Using:
        resource: 'Resource * body: ('Resource -> CancellableCode<'TOverall, 'T>) -> CancellableCode<'TOverall, 'T>
            when 'Resource :> IDisposable | null

[<AutoOpen>]
module internal CancellableCode =
    val inline throwIfCancellationRequested: CancellableCode<'a, 'b> -> CancellableCode<'a, 'c>

namespace Internal.Utilities.Library

open System.Threading
open Internal.Utilities.Library.CancellableImplementation

type internal Cancellable<'T> = CancellableImplementation.Cancellable<'T>

[<AutoOpen>]
module internal CancellableAutoOpens =
    val cancellable: CancellableImplementation.CancellableBuilder

module internal Cancellable =
    val run: ct: CancellationToken -> code: Cancellable<'a> -> ITrampolineInvocation<'a>
    val runWithoutCancellation: code: Cancellable<'a> -> 'a
    val toAsync: code: Cancellable<'a> -> Async<'a>
    val token: unit -> Cancellable<CancellationToken>
