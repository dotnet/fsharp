// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.Features
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.TypeRelations

type OverloadResolutionContext =
    {
        g: TcGlobals
        amap: ImportMap
        m: range
        /// Nesting depth for subsumption checks
        ndeep: int
        /// Per-method cache for GetParamDatas results, avoiding redundant calls across pairwise comparisons
        paramDataCache: System.Collections.Generic.Dictionary<obj, ParamData list>
        /// Per-method cache for SRTP presence checks, avoiding redundant traversals across pairwise comparisons
        srtpCache: System.Collections.Generic.Dictionary<obj, bool>
    }

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

/// Rules are ordered by their TiebreakRuleId (lower value = higher priority).
type TiebreakRule =
    {
        Id: TiebreakRuleId
        Description: string
        /// Optional LanguageFeature required for this rule to be active.
        /// If Some, the rule is skipped when the feature is not supported.
        RequiredFeature: LanguageFeature option
        /// Comparison function: returns >0 if candidate is better, <0 if other is better, 0 if equal
        Compare:
            OverloadResolutionContext
                -> struct (CalledMeth<Expr> * TypeDirectedConversionUsed * int) // candidate, TDC, warnCount
                -> struct (CalledMeth<Expr> * TypeDirectedConversionUsed * int) // other, TDC, warnCount
                -> int
    }

// -------------------------------------------------------------------------
// Type Concreteness Comparison
// -------------------------------------------------------------------------

/// Fold over two lists pairwise with a comparison function, aggregating using dominance.
/// Early-exits when incomparability is detected (both positive and negative seen).
let private aggregateMap2 (f: 'a -> 'b -> int) (xs: 'a list) (ys: 'b list) =
    let rec loop hasPositive hasNegative xs ys =
        match xs, ys with
        | [], _
        | _, [] ->
            if not hasNegative && hasPositive then 1
            elif not hasPositive && hasNegative then -1
            else 0
        | x :: xt, y :: yt ->
            let c = f x y
            let p = hasPositive || c > 0
            let n = hasNegative || c < 0
            if p && n then 0 // incomparable — early exit
            else loop p n xt yt

    loop false false xs ys

/// SRTP type parameters use a different constraint solving mechanism and shouldn't
/// be compared under the "more concrete" ordering.
let private isStaticallyResolvedTypeParam (tp: Typar) =
    match tp.StaticReq with
    | TyparStaticReq.HeadType -> true
    | TyparStaticReq.None -> false

let rec private containsSRTPTypeVar (g: TcGlobals) (ty: TType) : bool =
    let sty = stripTyEqns g ty

    match sty with
    | TType_var(tp, _) -> isStaticallyResolvedTypeParam tp
    | TType_app(_, args, _) -> args |> List.exists (containsSRTPTypeVar g)
    | TType_tuple(_, elems) -> elems |> List.exists (containsSRTPTypeVar g)
    | TType_fun(dom, rng, _) -> containsSRTPTypeVar g dom || containsSRTPTypeVar g rng
    | TType_anon(_, tys) -> tys |> List.exists (containsSRTPTypeVar g)
    | TType_forall(_, body) -> containsSRTPTypeVar g body
    | TType_measure _ -> false
    | TType_ucase _ -> false

/// Returns 1 if ty1 is more concrete, -1 if ty2 is more concrete, 0 if incomparable.
let rec compareTypeConcreteness (g: TcGlobals) ty1 ty2 =
    let sty1 = stripTyEqns g ty1
    let sty2 = stripTyEqns g ty2

    match sty1, sty2 with
    // Neither F# nor C# allows constraint-only method overloads, so comparing
    // constraint counts would be dead code. Both type vars are treated as equal.
    | TType_var _, TType_var _ -> 0

    | TType_var(tp, _), _ when isStaticallyResolvedTypeParam tp -> 0
    | _, TType_var(tp, _) when isStaticallyResolvedTypeParam tp -> 0
    | TType_var _, _ -> -1
    | _, TType_var _ -> 1

    | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) ->
        if not (tyconRefEq g tcref1 tcref2) then
            0
        elif args1.Length <> args2.Length then
            0
        else
            aggregateMap2 (compareTypeConcreteness g) args1 args2

    | TType_tuple(_, elems1), TType_tuple(_, elems2) ->
        if elems1.Length <> elems2.Length then
            0
        else
            aggregateMap2 (compareTypeConcreteness g) elems1 elems2

    | TType_fun(dom1, rng1, _), TType_fun(dom2, rng2, _) ->
        let cDomain = compareTypeConcreteness g dom1 dom2
        let cRange = compareTypeConcreteness g rng1 rng2
        // Inline aggregation for 2 elements to avoid list allocation
        let hasPositive = cDomain > 0 || cRange > 0
        let hasNegative = cDomain < 0 || cRange < 0

        if not hasNegative && hasPositive then 1
        elif not hasPositive && hasNegative then -1
        else 0

    | TType_anon(info1, tys1), TType_anon(info2, tys2) ->
        if not (anonInfoEquiv info1 info2) then
            0
        else
            aggregateMap2 (compareTypeConcreteness g) tys1 tys2

    | TType_measure _, TType_measure _ -> 0

    | TType_forall(tps1, body1), TType_forall(tps2, body2) ->
        if tps1.Length <> tps2.Length then
            0
        else
            compareTypeConcreteness g body1 body2

    | _ -> 0

/// Represents why two methods are incomparable under concreteness ordering.
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
    if meth1.CalledTyArgs.IsEmpty || meth2.CalledTyArgs.IsEmpty then
        None
    else
        let formalParams1 =
            meth1.Method.GetParamDatas(ctx.amap, ctx.m, meth1.Method.FormalMethodInst)
            |> List.concat

        let formalParams2 =
            meth2.Method.GetParamDatas(ctx.amap, ctx.m, meth2.Method.FormalMethodInst)
            |> List.concat

        if formalParams1.Length <> formalParams2.Length then
            None
        else
            let rec collectComparisons paramIdx (ty1: TType) (ty2: TType) : (int * int) list =
                let sty1 = stripTyEqns ctx.g ty1
                let sty2 = stripTyEqns ctx.g ty2

                match sty1, sty2 with
                | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) when
                    tyconRefEq ctx.g tcref1 tcref2 && args1.Length = args2.Length
                    ->
                    args1
                    |> List.mapi2
                        (fun argIdx arg1 arg2 ->
                            let c = compareTypeConcreteness ctx.g arg1 arg2
                            (argIdx + 1, c))
                        args2
                | _ ->
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
        aggregateMap2 (compareArg ctx) args1 args2
    else
        0

// -------------------------------------------------------------------------
// Rule Definitions
// -------------------------------------------------------------------------

let private noTDCRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NoTDC
        Description = "Prefer methods that don't use type-directed conversion"
        RequiredFeature = None
        Compare =
            fun _ (struct (_, usesTDC1, _)) (struct (_, usesTDC2, _)) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.No -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.No -> 1
                     | _ -> 0)
    }

let private lessTDCRule: TiebreakRule =
    {
        Id = TiebreakRuleId.LessTDC
        Description = "Prefer methods that need less type-directed conversion"
        RequiredFeature = None
        Compare =
            fun _ (struct (_, usesTDC1, _)) (struct (_, usesTDC2, _)) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.Yes(_, false, _) -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.Yes(_, false, _) -> 1
                     | _ -> 0)
    }

let private nullableTDCRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NullableTDC
        Description = "Prefer methods that only have nullable type-directed conversions"
        RequiredFeature = None
        Compare =
            fun _ (struct (_, usesTDC1, _)) (struct (_, usesTDC2, _)) ->
                compare
                    (match usesTDC1 with
                     | TypeDirectedConversionUsed.Yes(_, _, true) -> 1
                     | _ -> 0)
                    (match usesTDC2 with
                     | TypeDirectedConversionUsed.Yes(_, _, true) -> 1
                     | _ -> 0)
    }

let private noWarningsRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NoWarnings
        Description = "Prefer methods that don't give 'this code is less generic' warnings"
        RequiredFeature = None
        Compare = fun _ (struct (_, _, warnCount1)) (struct (_, _, warnCount2)) -> compare (warnCount1 = 0) (warnCount2 = 0)
    }

let private noParamArrayRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NoParamArray
        Description = "Prefer methods that don't use param array arg"
        RequiredFeature = None
        Compare =
            fun _ (struct (candidate, _, _)) (struct (other, _, _)) -> compare (not candidate.UsesParamArrayConversion) (not other.UsesParamArrayConversion)
    }

let private preciseParamArrayRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PreciseParamArray
        Description = "Prefer methods with more precise param array arg type"
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.UsesParamArrayConversion && other.UsesParamArrayConversion then
                    compareTypes ctx (candidate.GetParamArrayElementType()) (other.GetParamArrayElementType())
                else
                    0
    }

let private noOutArgsRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NoOutArgs
        Description = "Prefer methods that don't use out args"
        RequiredFeature = None
        Compare = fun _ (struct (candidate, _, _)) (struct (other, _, _)) -> compare (not candidate.HasOutArgs) (not other.HasOutArgs)
    }

let private noOptionalArgsRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NoOptionalArgs
        Description = "Prefer methods that don't use optional args"
        RequiredFeature = None
        Compare = fun _ (struct (candidate, _, _)) (struct (other, _, _)) -> compare (not candidate.HasOptionalArgs) (not other.HasOptionalArgs)
    }

let private unnamedArgsRule: TiebreakRule =
    {
        Id = TiebreakRuleId.UnnamedArgs
        Description = "Compare regular unnamed args using subsumption ordering"
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.TotalNumUnnamedCalledArgs = other.TotalNumUnnamedCalledArgs then
                    // For extension members, we also include the object argument type, if any in the comparison set
                    // This matches C#, where all extension members are treated and resolved as "static" methods calls
                    let objArgResult =
                        if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then
                            let objArgTys1 = candidate.CalledObjArgTys(ctx.m)
                            let objArgTys2 = other.CalledObjArgTys(ctx.m)

                            if objArgTys1.Length = objArgTys2.Length then
                                aggregateMap2 (compareTypes ctx) objArgTys1 objArgTys2
                            else
                                0
                        else
                            0

                    let unnamedResult =
                        aggregateMap2 (compareArg ctx) candidate.AllUnnamedCalledArgs other.AllUnnamedCalledArgs

                    // Combine the two sub-results using dominance
                    let hasPositive = objArgResult > 0 || unnamedResult > 0
                    let hasNegative = objArgResult < 0 || unnamedResult < 0

                    if not hasNegative && hasPositive then 1
                    elif not hasPositive && hasNegative then -1
                    else 0
                else
                    0
    }

let private preferNonExtensionRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PreferNonExtension
        Description = "Prefer non-extension methods over extension methods"
        RequiredFeature = None
        Compare =
            fun _ (struct (candidate, _, _)) (struct (other, _, _)) -> compare (not candidate.Method.IsExtensionMember) (not other.Method.IsExtensionMember)
    }

let private extensionPriorityRule: TiebreakRule =
    {
        Id = TiebreakRuleId.ExtensionPriority
        Description = "Between extension methods, prefer most recently opened"
        RequiredFeature = None
        Compare =
            fun _ (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then
                    compare candidate.Method.ExtensionMemberPriority other.Method.ExtensionMemberPriority
                else
                    0
    }

let private preferNonGenericRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PreferNonGeneric
        Description = "Prefer non-generic methods over generic methods"
        RequiredFeature = None
        Compare = fun _ (struct (candidate, _, _)) (struct (other, _, _)) -> compare candidate.CalledTyArgs.IsEmpty other.CalledTyArgs.IsEmpty
    }

let private getCachedParamData (ctx: OverloadResolutionContext) (meth: CalledMeth<Expr>) =
    let key = meth :> obj

    match ctx.paramDataCache.TryGetValue(key) with
    | true, v -> v
    | _ ->
        let v =
            meth.Method.GetParamDatas(ctx.amap, ctx.m, meth.Method.FormalMethodInst)
            |> List.concat

        ctx.paramDataCache[key] <- v
        v

let private getCachedHasSRTP (ctx: OverloadResolutionContext) (meth: CalledMeth<Expr>) =
    let key = meth :> obj

    match ctx.srtpCache.TryGetValue(key) with
    | true, v -> v
    | _ ->
        let hasTyparSRTP =
            meth.Method.FormalMethodTypars |> List.exists isStaticallyResolvedTypeParam

        let hasTyArgSRTP =
            hasTyparSRTP
            || meth.CalledTyArgs |> List.exists (containsSRTPTypeVar ctx.g)

        let result =
            hasTyArgSRTP
            || (let paramData = getCachedParamData ctx meth in
                paramData
                |> List.exists (fun (ParamData(_, _, _, _, _, _, _, ty)) -> containsSRTPTypeVar ctx.g ty))

        ctx.srtpCache[key] <- result
        result

let private moreConcreteRule: TiebreakRule =
    {
        Id = TiebreakRuleId.MoreConcrete
        Description = "Prefer more concrete type instantiations over more generic ones"
        RequiredFeature = Some LanguageFeature.MoreConcreteTiebreaker
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                if not candidate.CalledTyArgs.IsEmpty && not other.CalledTyArgs.IsEmpty then
                    if getCachedHasSRTP ctx candidate || getCachedHasSRTP ctx other then
                        0
                    else
                        let formalParams1 = getCachedParamData ctx candidate
                        let formalParams2 = getCachedParamData ctx other

                        if formalParams1.Length = formalParams2.Length then
                            aggregateMap2
                                (fun (ParamData(_, _, _, _, _, _, _, ty1)) (ParamData(_, _, _, _, _, _, _, ty2)) ->
                                    compareTypeConcreteness ctx.g ty1 ty2)
                                formalParams1
                                formalParams2
                        else
                            0
                else
                    0
    }

let private nullableOptionalInteropRule: TiebreakRule =
    {
        Id = TiebreakRuleId.NullableOptionalInterop
        Description = "F# 5.0 rule - compare all arguments including optional and named"
        RequiredFeature = Some LanguageFeature.NullableOptionalInterop
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                let args1 = candidate.AllCalledArgs |> List.concat
                let args2 = other.AllCalledArgs |> List.concat
                compareArgLists ctx args1 args2
    }

let private propertyOverrideRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PropertyOverride
        Description = "For properties, prefer more derived type (partial override support)"
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
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

let private allTiebreakRules: TiebreakRule list =
    [
        noTDCRule
        lessTDCRule
        nullableTDCRule
        noWarningsRule
        noParamArrayRule
        preciseParamArrayRule
        noOutArgsRule
        noOptionalArgsRule
        unnamedArgsRule
        preferNonExtensionRule
        extensionPriorityRule
        preferNonGenericRule
        moreConcreteRule
        nullableOptionalInteropRule
        propertyOverrideRule
    ]

let private isRuleEnabled (context: OverloadResolutionContext) (rule: TiebreakRule) =
    match rule.RequiredFeature with
    | None -> true
    | Some feature -> context.g.langVersion.SupportsFeature(feature)

/// Evaluate all tiebreaker rules and return both the result and the deciding rule.
/// Returns struct(result, ValueSome ruleId) if a rule decided, or struct(0, ValueNone) if all rules returned 0.
let findDecidingRule
    (context: OverloadResolutionContext)
    (candidate: struct (CalledMeth<Expr> * TypeDirectedConversionUsed * int))
    (other: struct (CalledMeth<Expr> * TypeDirectedConversionUsed * int))
    : struct (int * TiebreakRuleId voption) =

    let rec loop rules =
        match rules with
        | [] -> struct (0, ValueNone)
        | rule :: rest ->
            if isRuleEnabled context rule then
                let c = rule.Compare context candidate other
                if c <> 0 then struct (c, ValueSome rule.Id) else loop rest
            else
                loop rest

    loop allTiebreakRules

// -------------------------------------------------------------------------
// OverloadResolutionPriority Pre-Filter (RFC: .NET 9 attribute)
// -------------------------------------------------------------------------

/// Apply OverloadResolutionPriority pre-filter to a list of candidates.
/// Groups methods by declaring type and keeps only highest-priority within each group.
let filterByOverloadResolutionPriority<'T> (g: TcGlobals) (getMeth: 'T -> MethInfo) (candidates: 'T list) : 'T list =
    match candidates with
    | []
    | [ _ ] -> candidates
    | _ when not (g.langVersion.SupportsFeature LanguageFeature.OverloadResolutionPriority) -> candidates
    | twoOrMoreCandidates ->
        // Fast path: check if any method has a non-zero priority before allocating
        let hasAnyPriority =
            twoOrMoreCandidates
            |> List.exists (fun c -> (getMeth c).GetOverloadResolutionPriority() <> 0)

        if not hasAnyPriority then
            candidates
        else
            let enriched =
                twoOrMoreCandidates
                |> List.map (fun c ->
                    let m = getMeth c

                    let stamp =
                        match tryTcrefOfAppTy g m.ApparentEnclosingType with
                        | ValueSome tcref -> tcref.Stamp
                        | ValueNone -> 0L

                    (c, stamp, m.GetOverloadResolutionPriority()))

            enriched
            |> List.groupBy (fun (_, stamp, _) -> stamp)
            |> List.collect (fun (_, group) ->
                let _, _, maxPrio = group |> List.maxBy (fun (_, _, prio) -> prio)

                group
                |> List.filter (fun (_, _, prio) -> prio = maxPrio)
                |> List.map (fun (c, _, _) -> c))
