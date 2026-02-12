// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.Infos
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

/// Evaluate all tiebreaker rules and return both the result and the deciding rule.
/// Returns (result, ValueSome ruleId) if a rule decided, or (0, ValueNone) if all rules returned 0.
val findDecidingRule:
    context: OverloadResolutionContext ->
    candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
        other: CalledMeth<Expr> * TypeDirectedConversionUsed * int ->
            int * TiebreakRuleId voption

// -------------------------------------------------------------------------
// OverloadResolutionPriority Pre-Filter
// -------------------------------------------------------------------------

/// Apply OverloadResolutionPriority pre-filter to a list of candidates.
/// Groups methods by declaring type and keeps only highest-priority within each group.
val filterByOverloadResolutionPriority<'T> : g: TcGlobals -> getMeth: ('T -> MethInfo) -> candidates: 'T list -> 'T list
