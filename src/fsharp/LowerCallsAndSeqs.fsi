// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerCallsAndSeqs

open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Import
open FSharp.Compiler.Range

/// An "expr -> expr" pass that eta-expands under-applied values of
/// known arity to lambda expressions and beta-var-reduces to bind
/// any known arguments.  The results are later optimized by the peephole
/// optimizer in opt.fs
val LowerImplFile: g: TcGlobals -> assembly: TypedImplFile -> TypedImplFile

/// Analyze a TAST expression to detect the elaborated form of a sequence expression.
/// Then compile it to a state machine represented as a TAST containing goto, return and label nodes.
/// The returned state machine will also contain references to state variables (from internal 'let' bindings),
/// a program counter (pc) that records the current state, and a current generated value (current).
/// All these variables are then represented as fields in a hosting closure object along with any additional
/// free variables of the sequence expression.
val ConvertSequenceExprToObject: g: TcGlobals -> amap: ImportMap -> overallExpr: Expr -> (ValRef * ValRef * ValRef * ValRef list * Expr * Expr * Expr * TType * range) option

/// Analyze a TAST expression to detect the elaborated form of a state machine expression, a special kind
/// of object expression that uses special code generation constructs.
val ConvertStateMachineExprToObject: g: TcGlobals -> overallExpr: Expr -> Expr option
