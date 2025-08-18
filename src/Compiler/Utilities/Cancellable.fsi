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

type internal ITrampolineInvocation =
    abstract MoveNext: unit -> bool
    abstract IsCompleted: bool

type internal CancellableStateMachine<'TOverall> = ResumableStateMachine<'TOverall>
type internal ICancellableStateMachine<'TOverall> = IResumableStateMachine<'TOverall>
type internal CancellableResumptionFunc<'TOverall> = ResumptionFunc<'TOverall>
type internal CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<'TOverall>
type internal CancellableCode<'TOverall, 'T> = ResumableCode<'TOverall, 'T>

type internal ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: 'T

[<Sealed>]
type internal Trampoline =
    member SetDelayed: ITrampolineInvocation -> unit
    member RunImmediate: ITrampolineInvocation -> unit
    member ReplayException: unit -> unit
    static member Current: Trampoline
    member ShoudBounce: bool

[<NoEquality; NoComparison>]
type internal CancellableInvocation<'T, 'Machine
    when 'Machine: struct and 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>> =
    interface ITrampolineInvocation<'T>
    new: machine: 'Machine -> CancellableInvocation<'T, 'Machine>

[<Struct; NoComparison; NoEquality>]
type internal Cancellable<'T> =
    new: clone: (unit -> ITrampolineInvocation<'T>) -> Cancellable<'T>
    member GetInvocation: unit -> ITrampolineInvocation<'T>

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
    val runWithoutCancellation: code: Cancellable<'a> -> 'a
    val toAsync: code: Cancellable<'a> -> Async<'a>
    val token: unit -> Cancellable<CancellationToken>
