// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncAnalysis

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

type RuntimeAsyncAnalyzer =
    new: g: TcGlobals * getLambdaBody: (ValRef -> Expr option) -> RuntimeAsyncAnalyzer

    member ContainsFragment: expr: Expr -> bool
    member ContainsSuspension: expr: Expr -> bool

val ShouldForceRuntimeAsyncInline:
    analyzer: RuntimeAsyncAnalyzer -> runtimeAsyncContext: bool -> vref: ValRef -> inlineBody: Expr option -> bool

val ShouldForceRuntimeAsyncApplication:
    analyzer: RuntimeAsyncAnalyzer ->
    runtimeAsyncContext: bool ->
    vref: ValRef ->
    inlineBody: Expr option ->
    args: Expr list ->
        bool

val InlineRuntimeAsyncLambdaArgument: g: TcGlobals -> isRuntimeAsyncFragment: (Expr -> bool) -> expr: Expr -> Expr

val GetRuntimeAsyncNonPreservableUses: g: TcGlobals -> expr: Expr -> Val list
