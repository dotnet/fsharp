// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckComputationExpressions

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Infos
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

/// If the given method carries a [<CustomOperation>] attribute, return the operator name it declares
/// (the attribute argument, or the method's logical name when the attribute is parameterless/empty).
val TryGetCustomOperationName: g: TcGlobals -> m: range -> methInfo: MethInfo -> string option

val TcComputationExpression:
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: OverallTy ->
    tpenv: UnscopedTyparEnv ->
    mWhole: range * interpExpr: Expr * builderTy: TType * comp: SynExpr ->
        Expr * UnscopedTyparEnv
