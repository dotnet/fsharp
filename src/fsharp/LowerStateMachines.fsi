// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerStateMachines

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Import
open FSharp.Compiler.Range

/// Analyze a TAST expression to detect the elaborated form of a state machine expression, a special kind
/// of object expression that uses special code generation constructs.
val ConvertStateMachineExprToObject: g: TcGlobals -> overallExpr: Expr -> (Choice<Expr, ( (* templateStructTy *) TType * (* stateVars *) ValRef list * (* thisVars *) ValRef list * (* moveNextMethodThisVar: *) Val * (* moveNextExprWithJumpTable *) Expr * (* setMachineStateExprR *) Expr * (* afterMethodThisVar: *) Val * (* startExprR *) Expr)>) option

val IsPossibleStateMachineExpr: g: TcGlobals -> overallExpr: Expr -> bool