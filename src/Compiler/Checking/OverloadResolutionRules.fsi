// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Text
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.Import

/// The context needed for overload resolution rule evaluation
type OverloadResolutionContext =
    {
        g: TcGlobals
        amap: ImportMap
        m: range
        /// Nesting depth for subsumption checks
        ndeep: int
    }

/// Aggregate pairwise comparison results using dominance rule.
/// Returns 1 if ty1 dominates (better in some positions, not worse in any),
/// -1 if ty2 dominates, 0 if incomparable or equal.
val aggregateComparisons: comparisons: int list -> int

/// Compare types under the "more concrete" partial ordering.
/// Returns 1 if ty1 is more concrete, -1 if ty2 is more concrete, 0 if incomparable.
val compareTypeConcreteness: g: TcGlobals -> ty1: TType -> ty2: TType -> int

/// Explain why two types are incomparable under the concreteness ordering.
/// Returns Some with position-by-position details when types are incomparable (mixed results),
/// Returns None when one type strictly dominates or they are equal.
/// Each tuple contains (position, ty1Arg, ty2Arg, comparison) where comparison is 1/-1/0.
val explainIncomparableConcreteness: g: TcGlobals -> ty1: TType -> ty2: TType -> (int * TType * TType * int) list option

/// Represents why two methods are incomparable under concreteness ordering.
type IncomparableConcretenessInfo =
    { Method1Name: string
      Method1BetterPositions: int list
      Method2Name: string
      Method2BetterPositions: int list }

/// Explain why two CalledMeth objects are incomparable under the concreteness ordering.
/// Returns Some info when the methods are incomparable due to mixed concreteness results.
val explainIncomparableMethodConcreteness:
    ctx: OverloadResolutionContext ->
    meth1: CalledMeth<'T> ->
    meth2: CalledMeth<'T> ->
        IncomparableConcretenessInfo option

/// Identifies a tiebreaker rule in overload resolution.
/// Values are assigned to match the conceptual ordering in F# Language Spec §14.4.
/// Rules are evaluated in ascending order by their integer value.
[<RequireQualifiedAccess>]
type TiebreakRuleId =
    | NoTDC = 1
    | LessTDC = 2
    | NullableTDC = 3
    | NoWarnings = 4
    | NoParamArray = 5
    | PreciseParamArray = 6
    | NoOutArgs = 7
    | NoOptionalArgs = 8
    | UnnamedArgs = 9
    | PreferNonExtension = 10
    | ExtensionPriority = 11
    | PreferNonGeneric = 12
    | MoreConcrete = 13
    | NullableOptionalInterop = 14
    | PropertyOverride = 15

/// Represents a single tiebreaker rule in overload resolution.
/// Rules are ordered by their TiebreakRuleId (lower value = higher priority).
type TiebreakRule =
    {
        /// Rule identifier. Rules are evaluated in ascending order by this value.
        Id: TiebreakRuleId
        /// Human-readable description of what the rule does
        Description: string
        /// Comparison function: returns >0 if candidate is better, <0 if other is better, 0 if equal
        Compare:
            OverloadResolutionContext
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int // candidate, TDC, warnCount
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int // other, TDC, warnCount
                -> int
    }

/// Get all tiebreaker rules in priority order (ascending by TiebreakRuleId value).
val getAllTiebreakRules: unit -> TiebreakRule list

/// Evaluate all tiebreaker rules to determine which method is better.
/// Returns >0 if candidate is better, <0 if other is better, 0 if they are equal.
val evaluateTiebreakRules:
    context: OverloadResolutionContext ->
    candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
        other: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
            int

/// Check if a specific rule was the deciding factor between two methods.
/// Returns true if all rules BEFORE the specified rule returned 0, and the specified rule returned > 0.
val wasDecidedByRule:
    ruleId: TiebreakRuleId ->
    context: OverloadResolutionContext ->
    winner: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
        loser: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
            bool
