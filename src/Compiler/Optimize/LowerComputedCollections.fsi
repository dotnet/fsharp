// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerComputedCollectionExpressions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val LowerComputedListOrArrayExpr:
    tcVal: ConstraintSolver.TcValF ->
    g: TcGlobals ->
    amap: ImportMap ->
    ilTyForTy: (TType -> ILType) ->
    overallExpr: Expr ->
        Expr option
