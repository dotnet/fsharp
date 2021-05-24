// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.ErrorLogger

/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope =
    new : ErrorLogger * BuildPhase -> CompilationGlobalsScope
    interface IDisposable

[<NoEquality;NoComparison;Sealed>]
type GraphNode<'T>

type Async<'T> with

    static member AwaitGraphNode: node: GraphNode<'T> -> Async<'T>

[<Sealed>]
type GraphNodeBuilder =

    member Bind : GraphNode<'T> * ('T -> GraphNode<'U>) -> GraphNode<'U>

    member Zero : unit -> GraphNode<unit>

    member Delay : (unit -> GraphNode<'T>) -> GraphNode<'T>

    member Return : 'T -> GraphNode<'T> 

    member ReturnFrom : GraphNode<'T> -> GraphNode<'T>

    member TryWith : GraphNode<'T> * (exn -> GraphNode<'T>) -> GraphNode<'T>

    member TryFinally : GraphNode<'T> * (unit -> unit) -> GraphNode<'T>

    member For : xs: 'T seq * binder: ('T -> GraphNode<unit>) -> GraphNode<unit>

    member Combine : x1: GraphNode<unit> * x2: GraphNode<'T> -> GraphNode<'T>

    member Using : CompilationGlobalsScope * (CompilationGlobalsScope -> GraphNode<'T>) -> GraphNode<'T>

val node : GraphNodeBuilder

[<AbstractClass;Sealed>]
type GraphNode =

    static member RunSynchronously : computation: GraphNode<'T> -> 'T

    static member StartAsTask : computation: GraphNode<'T> * ?ct: CancellationToken -> Task<'T>

    static member CancellationToken : GraphNode<CancellationToken>

    static member Sequential : computations: GraphNode<'T> seq -> GraphNode<'T []>

    static member AwaitWaitHandle : waitHandle: WaitHandle -> GraphNode<bool>

[<RequireQualifiedAccess>]
module internal LazyGraphNode =

    /// Allows to specify the language for error messages
    val SetPreferredUILang : preferredUiLang: string option -> unit

/// Lazily evaluate the computation asynchronously, then strongly cache the result.
/// Once the result has been cached, the computation function will also be removed, or 'null'ed out, 
///     as to prevent any references captured by the computation from being strongly held.
/// The computation will only be canceled if there are no outstanding requests awaiting a response.
[<Sealed>]
type internal LazyGraphNode<'T> =

    new : computation: GraphNode<'T> -> LazyGraphNode<'T>

    member GetValue: unit -> GraphNode<'T>

    member TryGetValue: unit -> 'T voption

    member RequestCount: int