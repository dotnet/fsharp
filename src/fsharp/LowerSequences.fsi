// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerSequenceExpressions

open FSharp.Compiler.Import
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.Text

/// Detect a 'seq<int>' type
val (|SeqElemTy|_|): TcGlobals -> ImportMap -> range -> TType -> TType option

val callNonOverloadedILMethod: g: TcGlobals -> amap: ImportMap -> m: range -> methName: string -> ty: TType -> args: Exprs -> Expr

/// Analyze a TAST expression to detect the elaborated form of a sequence expression.
/// Then compile it to a state machine represented as a TAST containing goto, return and label nodes.
/// The returned state machine will also contain references to state variables (from internal 'let' bindings),
/// a program counter (pc) that records the current state, and a current generated value (current).
/// All these variables are then represented as fields in a hosting closure object along with any additional
/// free variables of the sequence expression.
val ConvertSequenceExprToObject:
    g: TcGlobals ->
    amap: ImportMap ->
    overallExpr: Expr ->
        (ValRef * ValRef * ValRef * ValRef list * Expr * Expr * Expr * TType * range) option

val IsPossibleSequenceExpr: g: TcGlobals -> overallExpr: Expr -> bool
