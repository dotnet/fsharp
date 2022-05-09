// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerComputedCollectionExpressions

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

val LowerComputedListOrArrayExpr:
    tcVal: ConstraintSolver.TcValF -> g: TcGlobals -> amap: ImportMap -> Expr -> Expr option
