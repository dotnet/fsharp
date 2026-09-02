// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsync

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

type RuntimeAsyncReturnInfo =
    { Value: ValRef
      Flags: ValUseFlag
      Body: Expr
      TypeArgs: TType list }

type RuntimeAsyncBoundary =
    | ReturnMarker of RuntimeAsyncReturnInfo
    | Suspension of ILMethodRef

val TryGetRuntimeAsyncReturn: g: TcGlobals -> expr: Expr -> RuntimeAsyncReturnInfo option

val TryGetRuntimeAsyncReturnFunction: g: TcGlobals -> expr: Expr -> (ValRef * ValUseFlag * range) option

val IsRuntimeAsyncSuspensionMethod: g: TcGlobals -> ilMethRef: ILMethodRef -> bool

val IsRuntimeAsyncSuspensionExpr: g: TcGlobals -> expr: Expr -> bool

val TryGetRuntimeAsyncBoundary: g: TcGlobals -> expr: Expr -> RuntimeAsyncBoundary option
