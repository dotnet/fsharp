// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Post-optimization lowering for runtime-async bodies.
///
/// Runs after the first optimization loop and before LowerLocalMutables. It outlines suspending
/// InlineIfLambda callbacks that the optimizer could not inline, using the Core context-handoff template, then
/// prepares every `__runtimeAsyncReturn*` body exactly once (non-preservable local diagnostics and
/// exception-handler rewriting). `__runtimeAsyncSequence` recipes are not prepared here: LowerAsyncSeq
/// prepares their MoveNextAsync body after state-machine conversion.
///
/// `reportedRanges` is shared by all files of a compilation so that a diagnostic in an inline body is
/// reported once, not again at every inlining site.
module internal FSharp.Compiler.LowerRuntimeAsync

open System.Collections.Concurrent
open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val TransformImplFile:
    g: TcGlobals ->
    amap: ImportMap ->
    optimizeExpr: (bool -> Expr -> Expr) ->
    reportedRanges: ConcurrentDictionary<range, unit> ->
    implFile: CheckedImplFile ->
        CheckedImplFile
