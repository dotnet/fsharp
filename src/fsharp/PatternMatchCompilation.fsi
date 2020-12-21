// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.PatternMatchCompilation

open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Range
open FSharp.Compiler.InfoReader

/// What should the decision tree contain for any incomplete match? 
type ActionOnFailure = 
    | ThrowIncompleteMatchException 
    | IgnoreWithWarning 
    | Throw 
    | Rethrow 
    | FailFilter

/// Represents the typechecked, elaborated form of a pattern, prior to pattern-match compilation.
[<NoEquality; NoComparison>]
type Pattern =
    | TPat_const of Const * Range
    | TPat_wild of Range
    | TPat_as of Pattern * PatternValBinding * Range
    | TPat_disjs of Pattern list * Range
    | TPat_conjs of Pattern list * Range
    | TPat_query of (Expr * TType list * (ValRef * TypeInst) option * int * PrettyNaming.ActivePatternInfo) * Pattern * Range
    | TPat_unioncase of UnionCaseRef * TypeInst * Pattern list * Range
    | TPat_exnconstr of TyconRef * Pattern list * Range
    | TPat_tuple of TupInfo * Pattern list * TType list * Range
    | TPat_array of Pattern list * TType * Range
    | TPat_recd of TyconRef * TypeInst * Pattern list * Range
    | TPat_range of char * char * Range
    | TPat_null of Range
    | TPat_isinst of TType * TType * PatternValBinding option * Range
    | TPat_error of Range

    member Range: Range

and PatternValBinding = 
    | PBind of Val * TypeScheme

and TypedMatchClause =  
    | TClause of Pattern * Expr option * DecisionTreeTarget * Range

val ilFieldToTastConst: ILFieldInit -> Const

/// Compile a pattern into a decision tree and a set of targets.
val internal CompilePattern: 
    TcGlobals ->
    DisplayEnv ->
    Import.ImportMap ->
    // tcVal
    (ValRef -> ValUseFlag -> TTypes -> Range -> Expr * TType) ->
    InfoReader ->
    // range of the expression we are matching on 
    Range ->  
    // range to report "incomplete match" on
    Range ->  
    // warn on unused? 
    bool ->   
    ActionOnFailure -> 
    // the value being matched against, perhaps polymorphic. Optionally includes the
    // input expression, only for the case of immediate matching on a byref pointer
    Val * Typars * Expr option -> 
    // input type-checked syntax of pattern matching
    TypedMatchClause list ->
    // input type 
    TType -> 
    // result type
    TType ->
        // produce TAST nodes
        DecisionTree * DecisionTreeTarget list

exception internal MatchIncomplete of bool * (string * bool) option * Range

exception internal RuleNeverMatched of Range

exception internal EnumMatchIncomplete of bool * (string * bool) option * Range