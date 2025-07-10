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
open System.Threading
open Microsoft.FSharp.Core.CompilerServices
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices

type internal ITrampolineInvocation =
    abstract MoveNext: unit -> unit
    abstract IsCompleted: bool

[<Struct; NoComparison; NoEquality>]
type internal CancellableStateMachineData<'T> =
    val mutable Result: 'T

and internal CancellableStateMachine<'TOverall> = ResumableStateMachine<CancellableStateMachineData<'TOverall>>
and internal ICancellableStateMachine<'TOverall> = IResumableStateMachine<CancellableStateMachineData<'TOverall>>
and internal CancellableResumptionFunc<'TOverall> = ResumptionFunc<CancellableStateMachineData<'TOverall>>
and internal CancellableResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<CancellableStateMachineData<'TOverall>>
and internal CancellableCode<'TOverall, 'T> = ResumableCode<CancellableStateMachineData<'TOverall>, 'T>

type internal ITrampolineInvocation<'T> =
    inherit ITrampolineInvocation
    abstract Result: 'T

type internal IMachineTemplateWrapper<'T> =
    abstract Clone: unit -> ITrampolineInvocation<'T>

[<Sealed>]
type internal Trampoline =
    val mutable Token: CancellationToken
    val mutable Exception: ExceptionDispatchInfo voption
    val mutable BindDepth: int
    member Set: ITrampolineInvocation -> unit
    member Execute: ITrampolineInvocation -> unit
    static member Current: Trampoline
    static member IsCancelled: bool
    static member HasError: bool
    static member Good: bool
    static member ThrowIfCancellationRequested: unit -> unit
    static member ShoudBounce: bool

[<NoEquality; NoComparison>]
type internal CancellableInvocation<'T, 'Machine
    when 'Machine :> IAsyncStateMachine and 'Machine :> ICancellableStateMachine<'T>> =
    interface IMachineTemplateWrapper<'T>
    interface ITrampolineInvocation<'T>
    new: machine: 'Machine -> CancellableInvocation<'T, 'Machine>

[<Struct; NoComparison>]
type internal Cancellable<'T> =
    new: template: IMachineTemplateWrapper<'T> -> Cancellable<'T>
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
    val inline captureExn: exn -> 'a
    val inline captureStackFrame: unit -> unit
    val inline protect: CancellableCode<'a, 'b> -> CancellableCode<'a, 'c>
    val inline notWhenCancelled: CancellableCode<'a, 'b> -> CancellableCode<'a, 'c>
    val inline notWhenError: CancellableCode<'a, 'b> -> CancellableCode<'a, 'c>
    val inline whenGood: CancellableCode<'a, 'b> -> CancellableCode<'a, 'c>
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
