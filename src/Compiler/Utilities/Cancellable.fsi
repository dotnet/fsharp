namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    static member internal UsingToken: CancellationToken -> IDisposable
    static member Token: CancellationToken
    static member CheckAndThrow: unit -> unit

namespace Internal.Utilities.Library

open System
open System.Threading

[<RequireQualifiedAccess; Struct>]
type internal ValueOrCancelled<'TResult> =
    | Value of result: 'TResult
    | Cancelled of ``exception``: OperationCanceledException

/// Represents a synchronous, cold-start, cancellable computation with explicit representation of a cancelled result.
///
/// A cancellable computation may be cancelled via a CancellationToken, which is propagated implicitly.
/// If cancellation occurs, it is propagated as data rather than by raising an OperationCanceledException.
[<Struct>]
type internal Cancellable<'T> = Cancellable of (CancellationToken -> ValueOrCancelled<'T>)

module internal Cancellable =

    /// Run a cancellable computation using the given cancellation token
    val inline run: ct: CancellationToken -> Cancellable<'T> -> ValueOrCancelled<'T>

    val fold: f: ('State -> 'T -> Cancellable<'State>) -> acc: 'State -> seq: seq<'T> -> Cancellable<'State>

    /// Run the computation in a mode where it may not be cancelled. The computation never results in a
    /// ValueOrCancelled.Cancelled.
    val runWithoutCancellation: comp: Cancellable<'T> -> 'T

    /// Bind the cancellation token associated with the computation
    val token: unit -> Cancellable<CancellationToken>

    val toAsync: Cancellable<'T> -> Async<'T>

type internal CancellableBuilder =

    new: unit -> CancellableBuilder

    member inline BindReturn: comp: Cancellable<'T> * [<InlineIfLambda>] k: ('T -> 'U) -> Cancellable<'U>

    member inline Bind: comp: Cancellable<'T> * [<InlineIfLambda>] k: ('T -> Cancellable<'U>) -> Cancellable<'U>

    member inline Combine: comp1: Cancellable<unit> * comp2: Cancellable<'T> -> Cancellable<'T>

    member inline Delay: [<InlineIfLambda>] f: (unit -> Cancellable<'T>) -> Cancellable<'T>

    member inline Return: v: 'T -> Cancellable<'T>

    member inline ReturnFrom: v: Cancellable<'T> -> Cancellable<'T>

    member inline TryFinally: comp: Cancellable<'T> * [<InlineIfLambda>] compensation: (unit -> unit) -> Cancellable<'T>

    member inline TryWith:
        comp: Cancellable<'T> * [<InlineIfLambda>] handler: (exn -> Cancellable<'T>) -> Cancellable<'T>

    member inline Using:
        resource: 'Resource * [<InlineIfLambda>] comp: ('Resource -> Cancellable<'T>) -> Cancellable<'T>
            when 'Resource :> IDisposable

    member inline Zero: unit -> Cancellable<unit>

[<AutoOpen>]
module internal CancellableAutoOpens =
    val cancellable: CancellableBuilder
