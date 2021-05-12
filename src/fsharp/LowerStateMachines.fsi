// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerStateMachines

open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
type LoweredStateMachine =
    | LoweredExpr of Expr
    | StructStateMachine of 
         templateStructTy: TType *
         stateVars: ValRef list *
         thisVars: ValRef list *
         moveNextMethodThisVar: Val *
         moveNextExprWithJumpTable: Expr * 
         setStateMachineExpr: Expr *
         otherMethods: (TType * string * Val list * Expr * range) list *
         afterMethodThisVar: Val * 
         afterMethodExpr: Expr

/// Analyze a TAST expression to detect the elaborated form of a state machine expression, a special kind
/// of object expression that uses special code generation constructs.
val ConvertStateMachineExprToObject:
    g: TcGlobals -> 
    overallExpr: Expr -> 
        LoweredStateMachine option

val IsPossibleStateMachineExpr: g: TcGlobals -> overallExpr: Expr -> bool