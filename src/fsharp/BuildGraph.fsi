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
type NodeCode<'T>

type Async<'T> with

    static member AwaitNode: node: NodeCode<'T> -> Async<'T>

[<Sealed>]
type NodeCodeBuilder =

    member Bind : NodeCode<'T> * ('T -> NodeCode<'U>) -> NodeCode<'U>

    member Zero : unit -> NodeCode<unit>

    member Delay : (unit -> NodeCode<'T>) -> NodeCode<'T>

    member Return : 'T -> NodeCode<'T> 

    member ReturnFrom : NodeCode<'T> -> NodeCode<'T>

    member TryWith : NodeCode<'T> * (exn -> NodeCode<'T>) -> NodeCode<'T>

    member TryFinally : NodeCode<'T> * (unit -> unit) -> NodeCode<'T>

    member For : xs: 'T seq * binder: ('T -> NodeCode<unit>) -> NodeCode<unit>

    member Combine : x1: NodeCode<unit> * x2: NodeCode<'T> -> NodeCode<'T>

    member Using : CompilationGlobalsScope * (CompilationGlobalsScope -> NodeCode<'T>) -> NodeCode<'T>

val node : NodeCodeBuilder

[<AbstractClass;Sealed>]
type NodeCode =

    static member RunImmediate : computation: NodeCode<'T> * ?ct: CancellationToken -> 'T

    static member StartAsTask : computation: NodeCode<'T> * ?ct: CancellationToken -> Task<'T>

    static member CancellationToken : NodeCode<CancellationToken>

    static member Sequential : computations: NodeCode<'T> seq -> NodeCode<'T []>

    static member AwaitWaitHandle : waitHandle: WaitHandle -> NodeCode<bool>

    static member Sleep : ms: int -> NodeCode<unit>

[<RequireQualifiedAccess>]
module internal GraphNode =

    /// Allows to specify the language for error messages
    val SetPreferredUILang : preferredUiLang: string option -> unit

/// Lazily evaluate the computation asynchronously, then strongly cache the result.
/// Once the result has been cached, the computation function will also be removed, or 'null'ed out, 
///     as to prevent any references captured by the computation from being strongly held.
/// The computation will only be canceled if there are no outstanding requests awaiting a response.
[<Sealed>]
type internal GraphNode<'T> =

    new : computation: NodeCode<'T> -> GraphNode<'T>

    member GetValue: unit -> NodeCode<'T>

    member TryGetValue: unit -> 'T voption

    member HasValue: bool

    member IsComputing: bool