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
    { g: TcGlobals
      amap: ImportMap
      m: range
      /// Nesting depth for subsumption checks
      ndeep: int }

/// Represents a single tiebreaker rule in overload resolution.
/// Rules are ordered by priority (lower number = higher priority).
type TiebreakRule =
    { /// Rule priority (1 = highest priority). Rules are evaluated in priority order.
      Priority: int
      /// Short identifier for the rule
      Name: string  
      /// Human-readable description of what the rule does
      Description: string
      /// Comparison function: returns >0 if candidate is better, <0 if other is better, 0 if equal
      Compare: OverloadResolutionContext 
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int  // candidate, TDC, warnCount
                -> CalledMeth<Expr> * TypeDirectedConversionUsed * int  // other, TDC, warnCount
                -> int }

/// Get all tiebreaker rules in priority order.
/// This includes all existing rules from the better() function plus a placeholder for the new MoreConcrete rule.
val getAllTiebreakRules: unit -> TiebreakRule list

/// Evaluate all tiebreaker rules to determine which method is better.
/// Returns >0 if candidate is better, <0 if other is better, 0 if they are equal.
val evaluateTiebreakRules: 
    context: OverloadResolutionContext 
    -> candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int 
    -> other: CalledMeth<Expr> * TypeDirectedConversionUsed * int 
    -> int
