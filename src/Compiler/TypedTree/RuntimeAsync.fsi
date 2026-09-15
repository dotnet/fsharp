// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsync

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

type RuntimeAsyncReturnInfo = { Body: Expr; TypeArgs: TType list }

val TryGetRuntimeAsyncReturn: g: TcGlobals -> expr: Expr -> RuntimeAsyncReturnInfo option

val (|RuntimeAsyncReturnFunction|_|): g: TcGlobals -> expr: Expr -> (ValRef * ValUseFlag * range) voption

val TryGetRuntimeAsyncSequence: g: TcGlobals -> expr: Expr -> (Expr * TType) option

val (|RuntimeAsyncSequenceFunction|_|): g: TcGlobals -> expr: Expr -> ValRef voption

val IsRuntimeAsyncSuspensionMethod: g: TcGlobals -> ilMethRef: ILMethodRef -> bool

val IsRuntimeAsyncSuspensionExpr: g: TcGlobals -> expr: Expr -> bool

val IsRuntimeAsyncBoundary: g: TcGlobals -> expr: Expr -> bool

val ExistsExpr: predicate: (Expr -> bool) -> expr: Expr -> bool
