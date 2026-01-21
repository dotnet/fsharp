// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.Features
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Text
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.TypeRelations

/// The context needed for overload resolution rule evaluation
type OverloadResolutionContext =
    {
        g: TcGlobals
        amap: ImportMap
        m: range
        /// Nesting depth for subsumption checks
        ndeep: int
    }

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

// -------------------------------------------------------------------------
// Type Concreteness Comparison (RFC FS-XXXX)
// -------------------------------------------------------------------------

/// Aggregate pairwise comparison results using dominance rule.
/// Returns 1 if ty1 dominates (better in some positions, not worse in any),
/// -1 if ty2 dominates, 0 if incomparable or equal.
let aggregateComparisons (comparisons: int list) =
    let hasPositive = comparisons |> List.exists (fun c -> c > 0)
    let hasNegative = comparisons |> List.exists (fun c -> c < 0)

    if not hasNegative && hasPositive then 1
    elif not hasPositive && hasNegative then -1
    else 0

/// Count the effective constraints on a type parameter for concreteness comparison.
/// Counts: CoercesTo (:>), IsNonNullableStruct, IsReferenceType, MayResolveMember, 
/// RequiresDefaultConstructor, IsEnum, IsDelegate, IsUnmanaged, SupportsComparison, SupportsEquality
let countTypeParamConstraints (tp: Typar) =
    tp.Constraints
    |> List.sumBy (function
        | TyparConstraint.CoercesTo _ -> 1
        | TyparConstraint.IsNonNullableStruct _ -> 1
        | TyparConstraint.IsReferenceType _ -> 1
        | TyparConstraint.MayResolveMember _ -> 1
        | TyparConstraint.RequiresDefaultConstructor _ -> 1
        | TyparConstraint.IsEnum _ -> 1
        | TyparConstraint.IsDelegate _ -> 1
        | TyparConstraint.IsUnmanaged _ -> 1
        | TyparConstraint.SupportsComparison _ -> 1
        | TyparConstraint.SupportsEquality _ -> 1
        // Don't count: DefaultsTo (inference-only), SupportsNull, NotSupportsNull (nullability), 
        // SimpleChoice (printf-specific), AllowsRefStruct (anti-constraint)
        | TyparConstraint.DefaultsTo _ -> 0
        | TyparConstraint.SupportsNull _ -> 0
        | TyparConstraint.NotSupportsNull _ -> 0
        | TyparConstraint.SimpleChoice _ -> 0
        | TyparConstraint.AllowsRefStruct _ -> 0)

/// Compare types under the "more concrete" partial ordering.
/// Returns 1 if ty1 is more concrete, -1 if ty2 is more concrete, 0 if incomparable.
let rec compareTypeConcreteness (g: TcGlobals) ty1 ty2 =
    let sty1 = stripTyEqns g ty1
    let sty2 = stripTyEqns g ty2

    match sty1, sty2 with
    // Case 1: Both are type variables - compare constraint counts (RFC section-algorithm.md lines 136-146)
    | TType_var(tp1, _), TType_var(tp2, _) ->
        let c1 = countTypeParamConstraints tp1
        let c2 = countTypeParamConstraints tp2
        if c1 > c2 then 1
        elif c2 > c1 then -1
        else 0

    // Case 2: Type variable vs concrete type - concrete is more concrete
    | TType_var _, _ -> -1
    | _, TType_var _ -> 1

    // Case 3: Type applications - compare type arguments when constructors match
    | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) ->
        if not (tyconRefEq g tcref1 tcref2) then
            0
        elif args1.Length <> args2.Length then
            0
        else
            let comparisons = List.map2 (compareTypeConcreteness g) args1 args2
            aggregateComparisons comparisons

    // Case 4: Tuple types - compare element-wise
    | TType_tuple(_, elems1), TType_tuple(_, elems2) ->
        if elems1.Length <> elems2.Length then
            0
        else
            let comparisons = List.map2 (compareTypeConcreteness g) elems1 elems2
            aggregateComparisons comparisons

    // Case 5: Function types - compare domain and range
    | TType_fun(dom1, rng1, _), TType_fun(dom2, rng2, _) ->
        let cDomain = compareTypeConcreteness g dom1 dom2
        let cRange = compareTypeConcreteness g rng1 rng2
        aggregateComparisons [ cDomain; cRange ]

    // Case 6: Anonymous record types - compare fields
    | TType_anon(info1, tys1), TType_anon(info2, tys2) ->
        if not (anonInfoEquiv info1 info2) then
            0
        else
            let comparisons = List.map2 (compareTypeConcreteness g) tys1 tys2
            aggregateComparisons comparisons

    // Case 7: Measure types - equal or incomparable
    | TType_measure _, TType_measure _ -> 0

    // Case 8: Universal quantified types (forall)
    | TType_forall(tps1, body1), TType_forall(tps2, body2) ->
        if tps1.Length <> tps2.Length then
            0
        else
            compareTypeConcreteness g body1 body2

    // Default: Different structural forms are incomparable
    | _ -> 0

/// Collect position-by-position comparison results for type arguments.
/// Returns a list of (position, ty1Arg, ty2Arg, comparison) tuples.
let private collectTypeArgComparisons (g: TcGlobals) (args1: TType list) (args2: TType list) : (int * TType * TType * int) list =
    if args1.Length <> args2.Length then
        []
    else
        (args1, args2)
        ||> List.mapi2 (fun i ty1 ty2 -> (i, ty1, ty2, compareTypeConcreteness g ty1 ty2))

/// Explain why two types are incomparable under the concreteness ordering.
/// Returns Some with position-by-position details when types are incomparable (mixed results),
/// Returns None when one type strictly dominates or they are equal.
let explainIncomparableConcreteness (g: TcGlobals) (ty1: TType) (ty2: TType) : (int * TType * TType * int) list option =
    let sty1 = stripTyEqns g ty1
    let sty2 = stripTyEqns g ty2

    let checkIncomparable (args1: TType list) (args2: TType list) =
        let comparisons = collectTypeArgComparisons g args1 args2
        let hasPositive = comparisons |> List.exists (fun (_, _, _, c) -> c > 0)
        let hasNegative = comparisons |> List.exists (fun (_, _, _, c) -> c < 0)
        // Incomparable means mixed results: at least one positive AND at least one negative
        if hasPositive && hasNegative then
            Some comparisons
        else
            None

    match sty1, sty2 with
    // Type applications - check if incomparable
    | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) ->
        if tyconRefEq g tcref1 tcref2 && args1.Length = args2.Length then
            checkIncomparable args1 args2
        else
            None

    // Tuple types - check element-wise
    | TType_tuple(_, elems1), TType_tuple(_, elems2) ->
        if elems1.Length = elems2.Length then
            checkIncomparable elems1 elems2
        else
            None

    // Function types - check domain and range
    | TType_fun(dom1, rng1, _), TType_fun(dom2, rng2, _) -> checkIncomparable [ dom1; rng1 ] [ dom2; rng2 ]

    // Anonymous record types - check fields
    | TType_anon(info1, tys1), TType_anon(info2, tys2) ->
        if anonInfoEquiv info1 info2 then
            checkIncomparable tys1 tys2
        else
            None

    // All other cases are not incomparable in a way we can explain
    | _ -> None

/// Represents why two methods are incomparable under concreteness ordering.
/// Contains (method1Name, method1BetterPositions, method2Name, method2BetterPositions)
type IncomparableConcretenessInfo =
    {
        Method1Name: string
        Method1BetterPositions: int list
        Method2Name: string
        Method2BetterPositions: int list
    }

/// Explain why two CalledMeth objects are incomparable under the concreteness ordering.
/// Returns Some info when the methods are incomparable due to mixed concreteness results.
let explainIncomparableMethodConcreteness<'T>
    (ctx: OverloadResolutionContext)
    (meth1: CalledMeth<'T>)
    (meth2: CalledMeth<'T>)
    : IncomparableConcretenessInfo option =
    // Only applies when both methods are generic
    if meth1.CalledTyArgs.IsEmpty || meth2.CalledTyArgs.IsEmpty then
        None
    else
        // Get formal (uninstantiated) parameter types
        let formalParams1 =
            meth1.Method.GetParamDatas(ctx.amap, ctx.m, meth1.Method.FormalMethodInst)
            |> List.concat

        let formalParams2 =
            meth2.Method.GetParamDatas(ctx.amap, ctx.m, meth2.Method.FormalMethodInst)
            |> List.concat

        if formalParams1.Length <> formalParams2.Length then
            None
        else
            // Collect all type argument comparisons, drilling into type applications
            let rec collectComparisons paramIdx (ty1: TType) (ty2: TType) : (int * int) list =
                let sty1 = stripTyEqns ctx.g ty1
                let sty2 = stripTyEqns ctx.g ty2

                match sty1, sty2 with
                | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) when
                    tyconRefEq ctx.g tcref1 tcref2 && args1.Length = args2.Length
                    ->
                    // Compare type arguments of the type application
                    args1
                    |> List.mapi2
                        (fun argIdx arg1 arg2 ->
                            let c = compareTypeConcreteness ctx.g arg1 arg2
                            (argIdx + 1, c)) // 1-based position for type args
                        args2
                | _ ->
                    // Compare at parameter level
                    [ (paramIdx, compareTypeConcreteness ctx.g ty1 ty2) ]

            let allComparisons =
                List.mapi2
                    (fun i (ParamData(_, _, _, _, _, _, _, ty1)) (ParamData(_, _, _, _, _, _, _, ty2)) ->
                        collectComparisons (i + 1) ty1 ty2)
                    formalParams1
                    formalParams2
                |> List.concat

            let meth1Better =
                allComparisons |> List.choose (fun (pos, c) -> if c > 0 then Some pos else None)

            let meth2Better =
                allComparisons |> List.choose (fun (pos, c) -> if c < 0 then Some pos else None)

            // Incomparable means each method is better in at least one position
            if not meth1Better.IsEmpty && not meth2Better.IsEmpty then
                Some
                    {
                        Method1Name = meth1.Method.DisplayName
                        Method1BetterPositions = meth1Better
                        Method2Name = meth2.Method.DisplayName
                        Method2BetterPositions = meth2Better
                    }
            else
                None

// -------------------------------------------------------------------------
// Helper functions for comparisons
// -------------------------------------------------------------------------

/// Compare two things by the given predicate.
/// If the predicate returns true for x1 and false for x2, then x1 > x2
/// If the predicate returns false for x1 and true for x2, then x1 < x2
/// Otherwise x1 = x2
let private compareCond (p: 'T -> 'T -> bool) x1 x2 = compare (p x1 x2) (p x2 x1)

/// Compare types under the feasibly-subsumes ordering
let private compareTypes (ctx: OverloadResolutionContext) ty1 ty2 =
    (ty1, ty2)
    ||> compareCond (fun x1 x2 -> TypeFeasiblySubsumesType ctx.ndeep ctx.g ctx.amap ctx.m x2 CanCoerce x1)

/// Compare arguments under the feasibly-subsumes ordering and the adhoc Func-is-better-than-other-delegates rule
let private compareArg (ctx: OverloadResolutionContext) (calledArg1: CalledArg) (calledArg2: CalledArg) =
    let g = ctx.g
    let c = compareTypes ctx calledArg1.CalledArgumentType calledArg2.CalledArgumentType

    if c <> 0 then
        c
    else

        let c =
            (calledArg1.CalledArgumentType, calledArg2.CalledArgumentType)
            ||> compareCond (fun ty1 ty2 ->

                // Func<_> is always considered better than any other delegate type
                match tryTcrefOfAppTy g ty1 with
                | ValueSome tcref1 when
                    tcref1.DisplayName = "Func"
                    && (match tcref1.PublicPath with
                        | Some p -> p.EnclosingPath = [| "System" |]
                        | _ -> false)
                    && isDelegateTy g ty1
                    && isDelegateTy g ty2
                    ->
                    true

                // T is always better than inref<T>
                | _ when isInByrefTy g ty2 && typeEquiv g ty1 (destByrefTy g ty2) -> true

                // T is always better than Nullable<T> from F# 5.0 onwards
                | _ when
                    g.langVersion.SupportsFeature(LanguageFeature.NullableOptionalInterop)
                    && isNullableTy g ty2
                    && typeEquiv g ty1 (destNullableTy g ty2)
                    ->
                    true

                | _ -> false)

        if c <> 0 then c else 0

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
let private noTDCRule: TiebreakRule =
    {
        Priority = 1
        Name = "NoTDC"
        Description = "Prefer methods that don't use type-directed conversion"
        Compare =
            fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.No -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.No -> 1
                     | _ -> 0)
    }

/// Rule 2: Prefer methods that need less type-directed conversion
let private lessTDCRule: TiebreakRule =
    {
        Priority = 2
        Name = "LessTDC"
        Description = "Prefer methods that need less type-directed conversion"
        Compare =
            fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.Yes(_, false, _) -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.Yes(_, false, _) -> 1
                     | _ -> 0)
    }

/// Rule 3: Prefer methods that only have nullable type-directed conversions
let private nullableTDCRule: TiebreakRule =
    {
        Priority = 3
        Name = "NullableTDC"
        Description = "Prefer methods that only have nullable type-directed conversions"
        Compare =
            fun _ (_, usesTDC1, _) (_, usesTDC2, _) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.Yes(_, _, true) -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.Yes(_, _, true) -> 1
                     | _ -> 0)
    }

/// Rule 4: Prefer methods that don't give "this code is less generic" warnings
let private noWarningsRule: TiebreakRule =
    {
        Priority = 4
        Name = "NoWarnings"
        Description = "Prefer methods that don't give 'this code is less generic' warnings"
        Compare = fun _ (_, _, warnCount1) (_, _, warnCount2) -> compare (warnCount1 = 0) (warnCount2 = 0)
    }

/// Rule 5: Prefer methods that don't use param array arg
let private noParamArrayRule: TiebreakRule =
    {
        Priority = 5
        Name = "NoParamArray"
        Description = "Prefer methods that don't use param array arg"
        Compare =
            fun _ (candidate, _, _) (other, _, _) -> compare (not candidate.UsesParamArrayConversion) (not other.UsesParamArrayConversion)
    }

/// Rule 6: Prefer methods with more precise param array arg type
let private preciseParamArrayRule: TiebreakRule =
    {
        Priority = 6
        Name = "PreciseParamArray"
        Description = "Prefer methods with more precise param array arg type"
        Compare =
            fun ctx (candidate, _, _) (other, _, _) ->
                if candidate.UsesParamArrayConversion && other.UsesParamArrayConversion then
                    compareTypes ctx (candidate.GetParamArrayElementType()) (other.GetParamArrayElementType())
                else
                    0
    }

/// Rule 7: Prefer methods that don't use out args
let private noOutArgsRule: TiebreakRule =
    {
        Priority = 7
        Name = "NoOutArgs"
        Description = "Prefer methods that don't use out args"
        Compare = fun _ (candidate, _, _) (other, _, _) -> compare (not candidate.HasOutArgs) (not other.HasOutArgs)
    }

/// Rule 8: Prefer methods that don't use optional args
let private noOptionalArgsRule: TiebreakRule =
    {
        Priority = 8
        Name = "NoOptionalArgs"
        Description = "Prefer methods that don't use optional args"
        Compare = fun _ (candidate, _, _) (other, _, _) -> compare (not candidate.HasOptionalArgs) (not other.HasOptionalArgs)
    }

/// Rule 9: Compare regular unnamed args (including extension member object args)
let private unnamedArgsRule: TiebreakRule =
    {
        Priority = 9
        Name = "UnnamedArgs"
        Description = "Compare regular unnamed args using subsumption ordering"
        Compare =
            fun ctx (candidate, _, _) (other, _, _) ->
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
                        objArgComparisons
                        @ ((candidate.AllUnnamedCalledArgs, other.AllUnnamedCalledArgs)
                           ||> List.map2 (compareArg ctx))
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
    }

/// Rule 10: Prefer non-extension methods
let private preferNonExtensionRule: TiebreakRule =
    {
        Priority = 10
        Name = "PreferNonExtension"
        Description = "Prefer non-extension methods over extension methods"
        Compare =
            fun _ (candidate, _, _) (other, _, _) -> compare (not candidate.Method.IsExtensionMember) (not other.Method.IsExtensionMember)
    }

/// Rule 11: Between extension methods, prefer most recently opened
let private extensionPriorityRule: TiebreakRule =
    {
        Priority = 11
        Name = "ExtensionPriority"
        Description = "Between extension methods, prefer most recently opened"
        Compare =
            fun _ (candidate, _, _) (other, _, _) ->
                if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then
                    compare candidate.Method.ExtensionMemberPriority other.Method.ExtensionMemberPriority
                else
                    0
    }

/// Rule 12: Prefer non-generic methods
let private preferNonGenericRule: TiebreakRule =
    {
        Priority = 12
        Name = "PreferNonGeneric"
        Description = "Prefer non-generic methods over generic methods"
        Compare = fun _ (candidate, _, _) (other, _, _) -> compare candidate.CalledTyArgs.IsEmpty other.CalledTyArgs.IsEmpty
    }

/// Rule 13: Prefer more concrete type instantiations (RFC FS-XXXX)
/// This is the "Most Concrete" tiebreaker from the RFC.
/// Only activates when BOTH methods are generic (have type arguments).
let private moreConcreteRule: TiebreakRule =
    {
        Priority = 13
        Name = "MoreConcrete"
        Description = "Prefer more concrete type instantiations over more generic ones"
        Compare =
            fun ctx (candidate, _, _) (other, _, _) ->
                if
                    ctx.g.langVersion.SupportsFeature(LanguageFeature.MoreConcreteTiebreaker)
                    && not candidate.CalledTyArgs.IsEmpty
                    && not other.CalledTyArgs.IsEmpty
                then
                    // Get formal (uninstantiated) parameter types using FormalMethodInst
                    let formalParams1 =
                        candidate.Method.GetParamDatas(ctx.amap, ctx.m, candidate.Method.FormalMethodInst)
                        |> List.concat

                    let formalParams2 =
                        other.Method.GetParamDatas(ctx.amap, ctx.m, other.Method.FormalMethodInst)
                        |> List.concat

                    if formalParams1.Length = formalParams2.Length then
                        let comparisons =
                            List.map2
                                (fun (ParamData(_, _, _, _, _, _, _, ty1)) (ParamData(_, _, _, _, _, _, _, ty2)) ->
                                    compareTypeConcreteness ctx.g ty1 ty2)
                                formalParams1
                                formalParams2

                        aggregateComparisons comparisons
                    else
                        0
                else
                    0
    }

/// Rule 14: F# 5.0 NullableOptionalInterop - compare all args including optional/named
let private nullableOptionalInteropRule: TiebreakRule =
    {
        Priority = 14
        Name = "NullableOptionalInterop"
        Description = "F# 5.0 rule - compare all arguments including optional and named"
        Compare =
            fun ctx (candidate, _, _) (other, _, _) ->
                if ctx.g.langVersion.SupportsFeature(LanguageFeature.NullableOptionalInterop) then
                    let args1 = candidate.AllCalledArgs |> List.concat
                    let args2 = other.AllCalledArgs |> List.concat
                    compareArgLists ctx args1 args2
                else
                    0
    }

/// Rule 15: For properties with partial override, prefer more derived type
let private propertyOverrideRule: TiebreakRule =
    {
        Priority = 15
        Name = "PropertyOverride"
        Description = "For properties, prefer more derived type (partial override support)"
        Compare =
            fun ctx (candidate, _, _) (other, _, _) ->
                match
                    candidate.AssociatedPropertyInfo,
                    other.AssociatedPropertyInfo,
                    candidate.Method.IsExtensionMember,
                    other.Method.IsExtensionMember
                with
                | Some p1, Some p2, false, false -> compareTypes ctx p1.ApparentEnclosingType p2.ApparentEnclosingType
                | _ -> 0
    }

// -------------------------------------------------------------------------
// Public API
// -------------------------------------------------------------------------

/// Get all tiebreaker rules in priority order.
/// This includes all existing rules from the better() function plus a placeholder for the new MoreConcrete rule.
let getAllTiebreakRules () : TiebreakRule list =
    [
        noTDCRule // Priority 1
        lessTDCRule // Priority 2
        nullableTDCRule // Priority 3
        noWarningsRule // Priority 4
        noParamArrayRule // Priority 5
        preciseParamArrayRule // Priority 6
        noOutArgsRule // Priority 7
        noOptionalArgsRule // Priority 8
        unnamedArgsRule // Priority 9
        preferNonExtensionRule // Priority 10
        extensionPriorityRule // Priority 11
        preferNonGenericRule // Priority 12
        moreConcreteRule // Priority 13 (RFC placeholder)
        nullableOptionalInteropRule // Priority 14
        propertyOverrideRule
    ] // Priority 15

/// Evaluate all tiebreaker rules to determine which method is better.
/// Returns >0 if candidate is better, <0 if other is better, 0 if they are equal.
let evaluateTiebreakRules
    (context: OverloadResolutionContext)
    (candidate: CalledMeth<Expr> * TypeDirectedConversionUsed * int)
    (other: CalledMeth<Expr> * TypeDirectedConversionUsed * int)
    : int =
    let rules = getAllTiebreakRules ()

    let rec loop rules =
        match rules with
        | [] -> 0
        | rule :: rest ->
            let c = rule.Compare context candidate other
            if c <> 0 then c else loop rest

    loop rules

/// Check if a specific rule was the deciding factor between two methods.
/// Returns true if all rules BEFORE the named rule returned 0, and the named rule returned > 0.
let wasDecidedByRule
    (ruleName: string)
    (context: OverloadResolutionContext)
    (winner: CalledMeth<Expr> * TypeDirectedConversionUsed * int)
    (loser: CalledMeth<Expr> * TypeDirectedConversionUsed * int)
    : bool =
    let rules = getAllTiebreakRules ()

    let rec loop rules =
        match rules with
        | [] -> false
        | rule :: rest ->
            let c = rule.Compare context winner loser

            if rule.Name = ruleName then c > 0 // The named rule decided in favor of winner
            elif c <> 0 then false // An earlier rule decided, so the named rule wasn't the decider
            else loop rest

    loop rules
