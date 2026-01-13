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
    val run: ct: CancellationToken -> Cancellable<'T> -> ValueOrCancelled<'T>

    val fold: f: ('State -> 'T -> Cancellable<'State>) -> acc: 'State -> seq: seq<'T> -> Cancellable<'State>

    /// Run the computation in a mode where it may not be cancelled. The computation never results in a
    /// ValueOrCancelled.Cancelled.
    val runWithoutCancellation: comp: Cancellable<'T> -> 'T

    /// Bind the cancellation token associated with the computation
    val token: unit -> Cancellable<CancellationToken>

    val toAsync: Cancellable<'T> -> Async<'T>

type internal CancellableBuilder =

    new: unit -> CancellableBuilder

    member BindReturn: comp: Cancellable<'T> * k: ('T -> 'U) -> Cancellable<'U>

    member Bind: comp: Cancellable<'T> * k: ('T -> Cancellable<'U>) -> Cancellable<'U>

    member Combine: comp1: Cancellable<unit> * comp2: Cancellable<'T> -> Cancellable<'T>

    member Delay: f: (unit -> Cancellable<'T>) -> Cancellable<'T>

    member Return: v: 'T -> Cancellable<'T>

    member ReturnFrom: v: Cancellable<'T> -> Cancellable<'T>

    member TryFinally: comp: Cancellable<'T> * compensation: (unit -> unit) -> Cancellable<'T>

    member TryWith:
        comp: Cancellable<'T> * handler: (exn -> Cancellable<'T>) -> Cancellable<'T>

    member Using:
        resource: 'Resource MaybeNull * comp: ('Resource MaybeNull -> Cancellable<'T>) ->
            Cancellable<'T>
            when 'Resource :> IDisposable and 'Resource: not struct and 'Resource: not null

    member Zero: unit -> Cancellable<unit>

[<AutoOpen>]
module internal CancellableAutoOpens =
    val cancellable: CancellableBuilder
