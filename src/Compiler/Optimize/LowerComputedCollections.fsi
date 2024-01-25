// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerComputedCollectionExpressions

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

[<RequireQualifiedAccess>]
type ComputedCollectionExprLowering =
    | Expr of initExpr: Expr
    | Either of branch1: Expr * branch2: Expr

val LowerComputedListOrArrayExpr:
    tcVal: ConstraintSolver.TcValF -> g: TcGlobals -> amap: ImportMap -> Expr -> ComputedCollectionExprLowering option
