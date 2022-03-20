// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckComputationExpressions

open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val TcSequenceExpressionEntry: cenv:TcFileState -> env:TcEnv -> overallTy:OverallTy -> tpenv:UnscopedTyparEnv -> hasBuilder:bool * comp:SynExpr -> m:range -> Expr * UnscopedTyparEnv    

val TcArrayOrListComputedExpression: cenv:TcFileState -> env:TcEnv -> overallTy:OverallTy -> tpenv:UnscopedTyparEnv -> cc:ConcreteCollection * comp:SynExpr -> m:range -> Expr * UnscopedTyparEnv    

val TcComputationExpression: cenv:TcFileState -> env:TcEnv -> overallTy:OverallTy -> tpenv:UnscopedTyparEnv -> mWhole:range * interpExpr:Expr * builderTy:TType * comp:SynExpr -> Expr * UnscopedTyparEnv    

