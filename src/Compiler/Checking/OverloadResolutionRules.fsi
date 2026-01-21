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

/// Represents a single tiebreaker rule in overload resolution.
/// Rules are ordered by priority (lower number = higher priority).
type TiebreakRule =
    {
        /// Rule priority (1 = highest priority). Rules are evaluated in priority order.
        Priority: int
        /// Short identifier for the rule
        Name: string
        /// Human-readable description of what the rule does
        Description: string
        /// Comparison function: returns >0 if candidate is better, <0 if other is better, 0 if equal
        Compare:
            OverloadResolutionContext
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int // candidate, TDC, warnCount
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int // other, TDC, warnCount
                -> int
    }

/// Get all tiebreaker rules in priority order.
/// This includes all existing rules from the better() function plus a placeholder for the new MoreConcrete rule.
val getAllTiebreakRules: unit -> TiebreakRule list

/// Evaluate all tiebreaker rules to determine which method is better.
/// Returns >0 if candidate is better, <0 if other is better, 0 if they are equal.
val evaluateTiebreakRules:
    context: OverloadResolutionContext ->
    candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
        other: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
            int

/// Check if a specific rule was the deciding factor between two methods.
/// Returns true if all rules BEFORE the named rule returned 0, and the named rule returned > 0.
val wasDecidedByRule:
    ruleName: string ->
    context: OverloadResolutionContext ->
    winner: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
        loser: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
            bool
