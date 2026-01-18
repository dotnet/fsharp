// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.Features
open FSharp.Compiler.Import
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Text
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.TypeRelations

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

// -------------------------------------------------------------------------
// Helper functions for comparisons
// -------------------------------------------------------------------------

/// Compare two things by the given predicate. 
/// If the predicate returns true for x1 and false for x2, then x1 > x2
/// If the predicate returns false for x1 and true for x2, then x1 < x2
/// Otherwise x1 = x2
let private compareCond (p: 'T -> 'T -> bool) x1 x2 = 
    compare (p x1 x2) (p x2 x1)

/// Compare types under the feasibly-subsumes ordering
let private compareTypes (ctx: OverloadResolutionContext) ty1 ty2 = 
    (ty1, ty2) ||> compareCond (fun x1 x2 -> TypeFeasiblySubsumesType ctx.ndeep ctx.g ctx.amap ctx.m x2 CanCoerce x1) 

/// Compare arguments under the feasibly-subsumes ordering and the adhoc Func-is-better-than-other-delegates rule
let private compareArg (ctx: OverloadResolutionContext) (calledArg1: CalledArg) (calledArg2: CalledArg) =
    let g = ctx.g
    let c = compareTypes ctx calledArg1.CalledArgumentType calledArg2.CalledArgumentType
    if c <> 0 then c else

    let c = 
        (calledArg1.CalledArgumentType, calledArg2.CalledArgumentType) ||> compareCond (fun ty1 ty2 -> 

            // Func<_> is always considered better than any other delegate type
            match tryTcrefOfAppTy g ty1 with 
            | ValueSome tcref1 when 
                tcref1.DisplayName = "Func" &&  
                (match tcref1.PublicPath with Some p -> p.EnclosingPath = [| "System" |] | _ -> false) && 
                isDelegateTy g ty1 &&
                isDelegateTy g ty2 -> true

            // T is always better than inref<T>
            | _ when isInByrefTy g ty2 && typeEquiv g ty1 (destByrefTy g ty2) -> 
                true

            // T is always better than Nullable<T> from F# 5.0 onwards
            | _ when g.langVersion.SupportsFeature(LanguageFeature.NullableOptionalInterop) &&
                        isNullableTy g ty2 &&
                        typeEquiv g ty1 (destNullableTy g ty2) -> 
                true

            | _ -> false)

    if c <> 0 then c else
    0

/// Compare argument lists using dominance: better in at least one, not worse in any
let private compareArgLists ctx (args1: CalledArg list) (args2: CalledArg list) =
    if args1.Length = args2.Length then 
        let cs = (args1, args2) ||> List.map2 (compareArg ctx)
        // "all args are at least as good, and one argument is actually better"
        if cs |> List.forall (fun x -> x >= 0) && cs |> List.exists (fun x -> x > 0) then 
            1
        // "all args are at least as bad, and one argument is actually worse"
        elif cs |> List.forall (fun x -> x <= 0) && cs |> List.exists (fun x -> x < 0) then 
            -1
        else
            0
    else
        0

// -------------------------------------------------------------------------
// Rule Definitions
// -------------------------------------------------------------------------

/// Rule 1: Prefer methods that don't use type-directed conversion
let private noTDCRule : TiebreakRule =
    { Priority = 1
      Name = "NoTDC"
      Description = "Prefer methods that don't use type-directed conversion"
      Compare = fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
        compare 
            (match usesTDC1 with TypeDirectedConversionUsed.No -> 1 | _ -> 0) 
            (match usesTDC2 with TypeDirectedConversionUsed.No -> 1 | _ -> 0) }

/// Rule 2: Prefer methods that need less type-directed conversion
let private lessTDCRule : TiebreakRule =
    { Priority = 2
      Name = "LessTDC"
      Description = "Prefer methods that need less type-directed conversion"
      Compare = fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
        compare 
            (match usesTDC1 with TypeDirectedConversionUsed.Yes(_, false, _) -> 1 | _ -> 0) 
            (match usesTDC2 with TypeDirectedConversionUsed.Yes(_, false, _) -> 1 | _ -> 0) }

/// Rule 3: Prefer methods that only have nullable type-directed conversions
let private nullableTDCRule : TiebreakRule =
    { Priority = 3
      Name = "NullableTDC"
      Description = "Prefer methods that only have nullable type-directed conversions"
      Compare = fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
        compare 
            (match usesTDC1 with TypeDirectedConversionUsed.Yes(_, _, true) -> 1 | _ -> 0) 
            (match usesTDC2 with TypeDirectedConversionUsed.Yes(_, _, true) -> 1 | _ -> 0) }

/// Rule 4: Prefer methods that don't give "this code is less generic" warnings
let private noWarningsRule : TiebreakRule =
    { Priority = 4
      Name = "NoWarnings"
      Description = "Prefer methods that don't give 'this code is less generic' warnings"
      Compare = fun _ (_, _, warnCount1) (_, _, warnCount2) ->
        compare (warnCount1 = 0) (warnCount2 = 0) }

/// Rule 5: Prefer methods that don't use param array arg
let private noParamArrayRule : TiebreakRule =
    { Priority = 5
      Name = "NoParamArray"
      Description = "Prefer methods that don't use param array arg"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        compare (not candidate.UsesParamArrayConversion) (not other.UsesParamArrayConversion) }

/// Rule 6: Prefer methods with more precise param array arg type
let private preciseParamArrayRule : TiebreakRule =
    { Priority = 6
      Name = "PreciseParamArray"
      Description = "Prefer methods with more precise param array arg type"
      Compare = fun ctx (candidate, _, _) (other, _, _) ->
        if candidate.UsesParamArrayConversion && other.UsesParamArrayConversion then
            compareTypes ctx (candidate.GetParamArrayElementType()) (other.GetParamArrayElementType())
        else
            0 }

/// Rule 7: Prefer methods that don't use out args
let private noOutArgsRule : TiebreakRule =
    { Priority = 7
      Name = "NoOutArgs"
      Description = "Prefer methods that don't use out args"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        compare (not candidate.HasOutArgs) (not other.HasOutArgs) }

/// Rule 8: Prefer methods that don't use optional args
let private noOptionalArgsRule : TiebreakRule =
    { Priority = 8
      Name = "NoOptionalArgs"
      Description = "Prefer methods that don't use optional args"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        compare (not candidate.HasOptionalArgs) (not other.HasOptionalArgs) }

/// Rule 9: Compare regular unnamed args (including extension member object args)
let private unnamedArgsRule : TiebreakRule =
    { Priority = 9
      Name = "UnnamedArgs"
      Description = "Compare regular unnamed args using subsumption ordering"
      Compare = fun ctx (candidate, _, _) (other, _, _) ->
        if candidate.TotalNumUnnamedCalledArgs = other.TotalNumUnnamedCalledArgs then
            // For extension members, we also include the object argument type, if any in the comparison set
            // This matches C#, where all extension members are treated and resolved as "static" methods calls
            let objArgComparisons = 
                if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then 
                    let objArgTys1 = candidate.CalledObjArgTys(ctx.m) 
                    let objArgTys2 = other.CalledObjArgTys(ctx.m) 
                    if objArgTys1.Length = objArgTys2.Length then 
                        List.map2 (compareTypes ctx) objArgTys1 objArgTys2
                    else
                        []
                else 
                    []
            let cs = 
                objArgComparisons @
                ((candidate.AllUnnamedCalledArgs, other.AllUnnamedCalledArgs) ||> List.map2 (compareArg ctx))
            // "all args are at least as good, and one argument is actually better"
            if cs |> List.forall (fun x -> x >= 0) && cs |> List.exists (fun x -> x > 0) then 
                1
            // "all args are at least as bad, and one argument is actually worse"
            elif cs |> List.forall (fun x -> x <= 0) && cs |> List.exists (fun x -> x < 0) then 
                -1
            else
                0
        else
            0 }

/// Rule 10: Prefer non-extension methods
let private preferNonExtensionRule : TiebreakRule =
    { Priority = 10
      Name = "PreferNonExtension"
      Description = "Prefer non-extension methods over extension methods"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        compare (not candidate.Method.IsExtensionMember) (not other.Method.IsExtensionMember) }

/// Rule 11: Between extension methods, prefer most recently opened
let private extensionPriorityRule : TiebreakRule =
    { Priority = 11
      Name = "ExtensionPriority"
      Description = "Between extension methods, prefer most recently opened"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then 
            compare candidate.Method.ExtensionMemberPriority other.Method.ExtensionMemberPriority 
        else 
            0 }

/// Rule 12: Prefer non-generic methods
let private preferNonGenericRule : TiebreakRule =
    { Priority = 12
      Name = "PreferNonGeneric"
      Description = "Prefer non-generic methods over generic methods"
      Compare = fun _ (candidate, _, _) (other, _, _) ->
        compare candidate.CalledTyArgs.IsEmpty other.CalledTyArgs.IsEmpty }

/// Rule 13: Prefer more concrete type instantiations (RFC FS-XXXX)
/// This is the "Most Concrete" tiebreaker from the RFC.
/// Only activates when BOTH methods are generic (have type arguments).
/// Note: The actual implementation uses compareTypeConcreteness from ConstraintSolver.fs
let private moreConcreteRule : TiebreakRule =
    { Priority = 13
      Name = "MoreConcrete"
      Description = "Prefer more concrete type instantiations over more generic ones"
      Compare = fun _ctx (candidate, _, _) (other, _, _) ->
        // Note: The actual logic is implemented directly in the better() function
        // in ConstraintSolver.fs because compareTypeConcreteness is defined there
        // and uses the csenv context. This rule documents the priority position.
        // Returns 0 here - the real comparison happens in better().
        if not candidate.CalledTyArgs.IsEmpty && not other.CalledTyArgs.IsEmpty then
            // Placeholder - actual implementation is in ConstraintSolver.fs better()
            0
        else
            0 }

/// Rule 14: F# 5.0 NullableOptionalInterop - compare all args including optional/named
let private nullableOptionalInteropRule : TiebreakRule =
    { Priority = 14
      Name = "NullableOptionalInterop"
      Description = "F# 5.0 rule - compare all arguments including optional and named"
      Compare = fun ctx (candidate, _, _) (other, _, _) ->
        if ctx.g.langVersion.SupportsFeature(LanguageFeature.NullableOptionalInterop) then
            let args1 = candidate.AllCalledArgs |> List.concat
            let args2 = other.AllCalledArgs |> List.concat
            compareArgLists ctx args1 args2
        else
            0 }

/// Rule 15: For properties with partial override, prefer more derived type
let private propertyOverrideRule : TiebreakRule =
    { Priority = 15
      Name = "PropertyOverride"
      Description = "For properties, prefer more derived type (partial override support)"
      Compare = fun ctx (candidate, _, _) (other, _, _) ->
        match candidate.AssociatedPropertyInfo, other.AssociatedPropertyInfo, 
              candidate.Method.IsExtensionMember, other.Method.IsExtensionMember with
        | Some p1, Some p2, false, false -> 
            compareTypes ctx p1.ApparentEnclosingType p2.ApparentEnclosingType
        | _ -> 0 }

// -------------------------------------------------------------------------
// Public API
// -------------------------------------------------------------------------

/// Get all tiebreaker rules in priority order.
/// This includes all existing rules from the better() function plus a placeholder for the new MoreConcrete rule.
let getAllTiebreakRules () : TiebreakRule list =
    [ noTDCRule                 // Priority 1
      lessTDCRule               // Priority 2
      nullableTDCRule           // Priority 3
      noWarningsRule            // Priority 4
      noParamArrayRule          // Priority 5
      preciseParamArrayRule     // Priority 6
      noOutArgsRule             // Priority 7
      noOptionalArgsRule        // Priority 8
      unnamedArgsRule           // Priority 9
      preferNonExtensionRule    // Priority 10
      extensionPriorityRule     // Priority 11
      preferNonGenericRule      // Priority 12
      moreConcreteRule          // Priority 13 (RFC placeholder)
      nullableOptionalInteropRule // Priority 14
      propertyOverrideRule ]    // Priority 15

/// Evaluate all tiebreaker rules to determine which method is better.
/// Returns >0 if candidate is better, <0 if other is better, 0 if they are equal.
let evaluateTiebreakRules 
    (context: OverloadResolutionContext) 
    (candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int) 
    (other: CalledMeth<Expr> * TypeDirectedConversionUsed * int) 
    : int =
    let rules = getAllTiebreakRules()
    let rec loop rules =
        match rules with
        | [] -> 0
        | rule :: rest ->
            let c = rule.Compare context candidate other
            if c <> 0 then c
            else loop rest
    loop rules
