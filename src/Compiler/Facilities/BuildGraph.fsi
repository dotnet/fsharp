// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.BuildGraph

open System.Threading
open System.Threading.Tasks
open Internal.Utilities.Library

/// Contains helpers to specify code that can be run as part of the build graph.
[<AbstractClass; Sealed>]
type Async =

    /// Used in places where we don't care about cancellation, e.g. the command line compiler
    /// and F# Interactive
    static member RunImmediateWithoutCancellation: computation: Async<'T> -> 'T

    /// Execute the cancellable computation synchronously using the ambient cancellation token of
    /// the NodeCode.
    static member FromCancellableWithScope: computation: Cancellable<'T> -> Async<'T>

    /// Wrap the computation with CompilationGlobalsScope to restore current DiagnosticsAsyncState after it returns.
    static member CompilationScope: computation: Async<'T> -> Async<'T>

    /// Only used for testing, do not use
    static member StartAsTask_ForTesting: computation: Async<'T> * ?ct: CancellationToken -> Task<'T>

    static member SequentialImmediate: computations: Async<'T> seq -> Async<'T array>

/// Contains helpers related to the build graph
[<RequireQualifiedAccess>]
module internal GraphNode =

    /// Allows to specify the language for error messages
    val SetPreferredUILang: preferredUiLang: string option -> unit

/// Evaluate the computation, allowing asynchronous waits on existing ongoing evaluations of the
/// same node, and strongly cache the result.
///
/// Once the result has been cached, the computation function will also be removed, or 'null'ed out,
/// as to prevent any references captured by the computation from being strongly held.
[<Sealed>]
type internal GraphNode<'T> =

    /// - computation - The computation code to run.
    new: computation: Async<'T> -> GraphNode<'T>

    /// Creates a GraphNode with given result already cached.
    static member FromResult: 'T -> GraphNode<'T>

    /// Return NodeCode which, when executed, will get the value of the computation if already computed, or
    /// await an existing in-progress computation for the node if one exists, or else will synchronously
    /// start the computation on the current thread.
    member GetOrComputeValue: unit -> Async<'T>

    /// Return 'Some' if the computation has already been computed, else None if
    /// the computation is in-progress or has not yet been started.
    member TryPeekValue: unit -> 'T voption

    /// Return 'true' if the computation has already been computed.
    member HasValue: bool

    /// Return 'true' if the computation is in-progress.
    member IsComputing: bool
