// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// DSL for overload resolution tiebreaker rules.
/// This module provides a structured representation of all rules used in method overload resolution.
module internal FSharp.Compiler.OverloadResolutionRules

open FSharp.Compiler.Features
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
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
        paramDataCache: System.Collections.Generic.Dictionary<obj, ParamData list> voption
        /// Per-method cache for SRTP presence checks, avoiding redundant traversals across pairwise comparisons
        srtpCache: System.Collections.Generic.Dictionary<obj, bool> voption
    }

/// Identifies a tiebreaker rule in overload resolution.
/// The integer values are stable conceptual identifiers matching F# Language Spec §14.4; they do
/// NOT define evaluation order. The actual evaluation order is the list order of `allTiebreakRules`
/// (which deliberately runs `MoreConcrete` last — see the rationale there), so do not reorder the
/// list to match these numbers.
[<RequireQualifiedAccess>]
type TiebreakRuleId =
    /// Prefer methods that don't use type-directed conversion
    | NoTDC = 1
    /// Prefer methods that need less type-directed conversion
    | LessTDC = 2
    /// Prefer methods that only have nullable type-directed conversions
    | NullableTDC = 3
    /// Prefer methods that don't give 'this code is less generic' warnings
    | NoWarnings = 4
    /// Prefer methods that don't use param array arg
    | NoParamArray = 5
    /// Prefer methods with more precise param array arg type
    | PreciseParamArray = 6
    /// Prefer methods that don't use out args
    | NoOutArgs = 7
    /// Prefer methods that don't use optional args
    | NoOptionalArgs = 8
    /// Compare regular unnamed args using subsumption ordering
    | UnnamedArgs = 9
    /// Prefer non-extension methods over extension methods
    | PreferNonExtension = 10
    /// Between extension methods, prefer most recently opened
    | ExtensionPriority = 11
    /// Prefer non-generic methods over generic methods
    | PreferNonGeneric = 12
    /// Prefer more concrete type instantiations over more generic ones
    | MoreConcrete = 13
    /// F# 5.0 rule - compare all arguments including optional and named
    | NullableOptionalInterop = 14
    /// For properties, prefer more derived type (partial override support)
    | PropertyOverride = 15

/// A single tiebreaker rule. Evaluation order is the list order of `allTiebreakRules`, not the
/// numeric `Id` (which is a report-only conceptual identifier).
type TiebreakRule =
    {
        Id: TiebreakRuleId
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

/// Fold over two lists pairwise with a comparison function, accumulating dominance state.
/// Early-exits when incomparability is detected (both positive and negative seen).
/// Returns the accumulated state so it can be chained across multiple lists.
let private foldMap2 (f: 'a -> 'b -> int) initP initN (xs: 'a list) (ys: 'b list) =
    let rec loop hasPositive hasNegative xs ys =
        match xs, ys with
        | [], _
        | _, [] -> struct (hasPositive, hasNegative)
        | x :: xt, y :: yt ->
            let c = f x y
            let p = hasPositive || c > 0
            let n = hasNegative || c < 0

            if p && n then
                struct (true, true) // incomparable — early exit
            else
                loop p n xt yt

    loop initP initN xs ys

/// Convert accumulated dominance state into a comparison result.
let private resolveAggregation (struct (hasPositive, hasNegative)) =
    if not hasNegative && hasPositive then 1
    elif not hasPositive && hasNegative then -1
    else 0

/// Fold over two lists pairwise with a comparison function, aggregating using dominance.
let private aggregateMap2 f xs ys =
    foldMap2 f false false xs ys |> resolveAggregation

/// SRTP type parameters use a different constraint solving mechanism and shouldn't
/// be compared under the "more concrete" ordering.
let private isStaticallyResolvedTypeParam (tp: Typar) =
    match tp.StaticReq with
    | TyparStaticReq.HeadType -> true
    | TyparStaticReq.None -> false

/// The type carried by a formal parameter.
let private paramDataType (ParamData(_, _, _, _, _, _, _, ty)) = ty

/// True if any of these parameters' types mentions a comparable (non-SRTP) type variable — from a
/// method type parameter OR an enclosing-type type parameter. The latter lets constructors and
/// generic-type members (whose instantiation is inferred from the arguments, so they carry no
/// method type arguments) participate in the concreteness ordering.
let private paramsMentionComparableTypeVar (g: TcGlobals) (ps: ParamData list) : bool =
    freeInTypesLeftToRight g true (List.map paramDataType ps)
    |> List.exists (fun tp -> not (isStaticallyResolvedTypeParam tp))

/// True if any of these parameters' types mentions a statically-resolved (SRTP) type variable.
/// Complement of paramsMentionComparableTypeVar over the same free-typar set.
let private paramsMentionSRTP (g: TcGlobals) (ps: ParamData list) : bool =
    freeInTypesLeftToRight g true (List.map paramDataType ps)
    |> List.exists isStaticallyResolvedTypeParam

/// True if a method's SRTP surface — its method type parameters, called type arguments, or
/// parameter types — mentions a statically-resolved type variable. SRTP members are excluded from
/// the concreteness ordering because their instantiation is resolved by trait solving, not by
/// betterness. Shared by moreConcreteRule's firing gate and the FS0041 diagnostic explainer so the
/// two cannot drift. Callers pass the already-computed parameter data to avoid recomputing it.
let private methodMentionsSRTP (g: TcGlobals) (meth: CalledMeth<'T>) (paramData: ParamData list) : bool =
    (meth.Method.FormalMethodTypars |> List.exists isStaticallyResolvedTypeParam)
    || (freeInTypesLeftToRight g true meth.CalledTyArgs
        |> List.exists isStaticallyResolvedTypeParam)
    || paramsMentionSRTP g paramData

/// Returns 1 if ty1 is more concrete, -1 if ty2 is more concrete, 0 if incomparable.
let compareTypeConcreteness (g: TcGlobals) ty1 ty2 =
    let rec loop ty1 ty2 =
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
            if not (tyconRefEq g tcref1 tcref2) then 0
            elif args1.Length <> args2.Length then 0
            else aggregateMap2 loop args1 args2

        | TType_tuple(_, elems1), TType_tuple(_, elems2) ->
            if elems1.Length <> elems2.Length then
                0
            else
                aggregateMap2 loop elems1 elems2

        | TType_fun(dom1, rng1, _), TType_fun(dom2, rng2, _) ->
            let cDomain = loop dom1 dom2
            let cRange = loop rng1 rng2
            resolveAggregation (struct (cDomain > 0 || cRange > 0, cDomain < 0 || cRange < 0))

        | TType_anon(info1, tys1), TType_anon(info2, tys2) ->
            if not (anonInfoEquiv info1 info2) then
                0
            else
                aggregateMap2 loop tys1 tys2

        | TType_measure _, TType_measure _ -> 0

        | TType_forall(tps1, body1), TType_forall(tps2, body2) -> if tps1.Length <> tps2.Length then 0 else loop body1 body2

        | _ -> 0

    loop ty1 ty2

/// Represents why two methods are incomparable under concreteness ordering.
type IncomparableConcretenessInfo =
    {
        Method1Signature: string
        Method1BetterPositions: int list
        Method2Signature: string
        Method2BetterPositions: int list
    }

/// Explain why two CalledMeth objects are incomparable under the concreteness ordering.
/// Returns Some info when the methods are incomparable due to mixed concreteness results.
let explainIncomparableMethodConcreteness<'T>
    (ctx: OverloadResolutionContext)
    (infoReader: InfoReader)
    (denv: DisplayEnv)
    (meth1: CalledMeth<'T>)
    (meth2: CalledMeth<'T>)
    : IncomparableConcretenessInfo option =
    let formalParams1 =
        meth1.Method.GetParamDatas(ctx.amap, ctx.m, meth1.Method.FormalMethodInst)
        |> List.concat

    let formalParams2 =
        meth2.Method.GetParamDatas(ctx.amap, ctx.m, meth2.Method.FormalMethodInst)
        |> List.concat

    // Use moreConcreteRule's exact firing gate (via the shared methodMentionsSRTP) so the FS0041
    // detail only explains cases the rule actually ranks: both parameter lists must mention a
    // comparable (non-SRTP) type variable and have equal length, and neither method may involve
    // SRTP anywhere in its type parameters, type arguments, or parameters.
    if
        formalParams1.Length <> formalParams2.Length
        || not (paramsMentionComparableTypeVar ctx.g formalParams1)
        || not (paramsMentionComparableTypeVar ctx.g formalParams2)
        || methodMentionsSRTP ctx.g meth1 formalParams1
        || methodMentionsSRTP ctx.g meth2 formalParams2
    then
        None
    else
        let collectComparisons paramIdx (ty1: TType) (ty2: TType) : (int * int) list =
            let sty1 = stripTyEqns ctx.g ty1
            let sty2 = stripTyEqns ctx.g ty2

            match sty1, sty2 with
            | TType_app(tcref1, args1, _), TType_app(tcref2, args2, _) when tyconRefEq ctx.g tcref1 tcref2 && args1.Length = args2.Length ->
                (args1, args2)
                ||> List.mapi2 (fun argIdx arg1 arg2 ->
                    let c = compareTypeConcreteness ctx.g arg1 arg2
                    (argIdx + 1, c))
            | _ -> [ (paramIdx, compareTypeConcreteness ctx.g ty1 ty2) ]

        // Report the positions at which each candidate is strictly more concrete.
        //
        // With a single formal parameter we decompose a same-constructor application (e.g.
        // Result<_,_>) into its type-argument positions, so the flagship "Result<int,'error> vs
        // Result<'ok,string>" ambiguity is explained per differing type argument. This is unambiguous
        // because every reported position refers to that one parameter's type arguments.
        //
        // With several formal parameters we compare per parameter and report the formal-parameter
        // index instead - matching moreConcreteRule's own unit of comparison. A same-constructor
        // parameter that is internally incomparable is neutral and simply drops out.
        let allComparisons =
            match formalParams1, formalParams2 with
            | [ p1 ], [ p2 ] -> collectComparisons 1 (paramDataType p1) (paramDataType p2)
            | _ ->
                (formalParams1, formalParams2)
                ||> List.mapi2 (fun i p1 p2 -> (i + 1, compareTypeConcreteness ctx.g (paramDataType p1) (paramDataType p2)))

        let meth1Better =
            allComparisons |> List.choose (fun (pos, c) -> if c > 0 then Some pos else None)

        let meth2Better =
            allComparisons |> List.choose (fun (pos, c) -> if c < 0 then Some pos else None)

        if not meth1Better.IsEmpty && not meth2Better.IsEmpty then
            Some
                {
                    Method1Signature = NicePrint.stringOfMethInfoForOverloadError infoReader ctx.m denv meth1.Method
                    Method1BetterPositions = meth1Better
                    Method2Signature = NicePrint.stringOfMethInfoForOverloadError infoReader ctx.m denv meth2.Method
                    Method2BetterPositions = meth2Better
                }
        else
            None

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
                        | ValueSome p -> p.EnclosingPath = [| "System" |]
                        | _ -> false)
                    && isDelegateTy g ty1
                    && isDelegateTy g ty2
                    ->
                    true

                // T is always better than inref<T>
                | _ when isInByrefTy g ty2 && typeEquiv g ty1 (destByrefTy g ty2) -> true

                // T is always better than Nullable<T>
                | _ when isNullableTy g ty2 && typeEquiv g ty1 (destNullableTy g ty2) -> true

                | _ -> false)

        if c <> 0 then c else 0

/// Compare argument lists using dominance: better in at least one, not worse in any
let private compareArgLists ctx (args1: CalledArg list) (args2: CalledArg list) =
    if args1.Length = args2.Length then
        aggregateMap2 (compareArg ctx) args1 args2
    else
        0

/// Build a rule that prefers candidates for which `preferred` holds. The predicate reads only the
/// already-computed per-candidate facts (candidate, type-directed-conversion use, warning count),
/// so no resolution context is needed.
let private preferFlagRule id (preferred: struct (CalledMeth<Expr> * TypeDirectedConversionUsed * int) -> bool) : TiebreakRule =
    {
        Id = id
        RequiredFeature = None
        Compare = fun _ a b -> compare (preferred a) (preferred b)
    }

let private noTDCRule =
    preferFlagRule TiebreakRuleId.NoTDC (fun (struct (_, usesTDC, _)) ->
        match usesTDC with
        | TypeDirectedConversionUsed.No -> true
        | _ -> false)

let private lessTDCRule =
    preferFlagRule TiebreakRuleId.LessTDC (fun (struct (_, usesTDC, _)) ->
        match usesTDC with
        | TypeDirectedConversionUsed.Yes(_, false, _) -> true
        | _ -> false)

let private nullableTDCRule =
    preferFlagRule TiebreakRuleId.NullableTDC (fun (struct (_, usesTDC, _)) ->
        match usesTDC with
        | TypeDirectedConversionUsed.Yes(_, _, true) -> true
        | _ -> false)

let private noWarningsRule =
    preferFlagRule TiebreakRuleId.NoWarnings (fun (struct (_, _, warnCount)) -> warnCount = 0)

let private noParamArrayRule =
    preferFlagRule TiebreakRuleId.NoParamArray (fun (struct (candidate, _, _)) -> not candidate.UsesParamArrayConversion)

let private preciseParamArrayRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PreciseParamArray
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.UsesParamArrayConversion && other.UsesParamArrayConversion then
                    compareTypes ctx (candidate.GetParamArrayElementType()) (other.GetParamArrayElementType())
                else
                    0
    }

let private noOutArgsRule =
    preferFlagRule TiebreakRuleId.NoOutArgs (fun (struct (candidate, _, _)) -> not candidate.HasOutArgs)

let private noOptionalArgsRule =
    preferFlagRule TiebreakRuleId.NoOptionalArgs (fun (struct (candidate, _, _)) -> not candidate.HasOptionalArgs)

let private unnamedArgsRule: TiebreakRule =
    {
        Id = TiebreakRuleId.UnnamedArgs
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.TotalNumUnnamedCalledArgs = other.TotalNumUnnamedCalledArgs then
                    // Fold over obj-args first, then unnamed-args, with shared dominance state.
                    // This avoids intermediate list allocations from `@` concatenation while
                    // still detecting cross-group incomparability correctly.
                    let struct (p, n) =
                        if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then
                            let objArgTys1 = candidate.CalledObjArgTys(ctx.m)
                            let objArgTys2 = other.CalledObjArgTys(ctx.m)

                            if objArgTys1.Length = objArgTys2.Length then
                                foldMap2 (compareTypes ctx) false false objArgTys1 objArgTys2
                            else
                                struct (false, false)
                        else
                            struct (false, false)

                    if p && n then
                        0
                    else
                        foldMap2 (compareArg ctx) p n candidate.AllUnnamedCalledArgs other.AllUnnamedCalledArgs
                        |> resolveAggregation
                else
                    0
    }

let private preferNonExtensionRule =
    preferFlagRule TiebreakRuleId.PreferNonExtension (fun (struct (candidate, _, _)) -> not candidate.Method.IsExtensionMember)

let private extensionPriorityRule: TiebreakRule =
    {
        Id = TiebreakRuleId.ExtensionPriority
        RequiredFeature = None
        Compare =
            fun _ (struct (candidate, _, _)) (struct (other, _, _)) ->
                if candidate.Method.IsExtensionMember && other.Method.IsExtensionMember then
                    compare candidate.Method.ExtensionMemberPriority other.Method.ExtensionMemberPriority
                else
                    0
    }

let private preferNonGenericRule =
    preferFlagRule TiebreakRuleId.PreferNonGeneric (fun (struct (candidate, _, _)) -> candidate.CalledTyArgs.IsEmpty)

let private getCached (cache: System.Collections.Generic.Dictionary<obj, 'v> voption) (key: obj) (compute: unit -> 'v) =
    match cache with
    | ValueNone -> compute ()
    | ValueSome cache ->
        match cache.TryGetValue key with
        | true, v -> v
        | _ ->
            let v = compute ()
            cache[key] <- v
            v

let private getCachedParamData (ctx: OverloadResolutionContext) (meth: CalledMeth<Expr>) =
    getCached ctx.paramDataCache (meth :> obj) (fun () ->
        meth.Method.GetParamDatas(ctx.amap, ctx.m, meth.Method.FormalMethodInst)
        |> List.concat)

let private getCachedHasSRTP (ctx: OverloadResolutionContext) (meth: CalledMeth<Expr>) =
    getCached ctx.srtpCache (meth :> obj) (fun () -> methodMentionsSRTP ctx.g meth (getCachedParamData ctx meth))

let private moreConcreteRule: TiebreakRule =
    {
        Id = TiebreakRuleId.MoreConcrete
        RequiredFeature = Some LanguageFeature.MoreConcreteTiebreaker
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                let formalParams1 = getCachedParamData ctx candidate
                let formalParams2 = getCachedParamData ctx other

                // Fire when both candidates' formal parameters mention a comparable type variable,
                // whether from a method type parameter or an enclosing generic type (the latter
                // covers constructors and generic-type members with inferred instantiation).
                if
                    paramsMentionComparableTypeVar ctx.g formalParams1
                    && paramsMentionComparableTypeVar ctx.g formalParams2
                then
                    if getCachedHasSRTP ctx candidate || getCachedHasSRTP ctx other then
                        0
                    elif formalParams1.Length = formalParams2.Length then
                        aggregateMap2
                            (fun p1 p2 -> compareTypeConcreteness ctx.g (paramDataType p1) (paramDataType p2))
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
        RequiredFeature = None
        Compare =
            fun ctx (struct (candidate, _, _)) (struct (other, _, _)) ->
                let args1 = candidate.AllCalledArgs |> List.concat
                let args2 = other.AllCalledArgs |> List.concat
                compareArgLists ctx args1 args2
    }

let private propertyOverrideRule: TiebreakRule =
    {
        Id = TiebreakRuleId.PropertyOverride
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
        nullableOptionalInteropRule
        propertyOverrideRule
        // The most-concrete tiebreak is a last resort: it must run after every rule that is
        // enabled at default langversion (e.g. the F# 5.0 nullable/optional-interop rule and the
        // property-override rule) so that enabling this preview feature can only break ties those
        // rules left unresolved (i.e. today's FS0041 ambiguities), never re-decide a resolution
        // that already succeeds at default.
        moreConcreteRule
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

/// Apply OverloadResolutionPriority pre-filter to a list of candidates.
/// Groups methods by declaring type and keeps only highest-priority within each group.
let filterByOverloadResolutionPriority<'T> (g: TcGlobals) (getMeth: 'T -> MethInfo) (candidates: 'T list) : 'T list =
    match candidates with
    | []
    | [ _ ] -> candidates
    | _ when not (g.langVersion.SupportsFeature LanguageFeature.OverloadResolutionPriority) -> candidates
    | twoOrMoreCandidates ->
        // Fast path: check if any method has a non-zero priority before allocating the enriched list.
        // In 99% of resolutions no method uses the attribute, so this avoids all allocation.
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
                    (c, m.DeclaringTyconRef.Stamp, m.GetOverloadResolutionPriority()))

            enriched
            |> List.groupBy (fun (_, stamp, _) -> stamp)
            |> List.collect (fun (_, group) ->
                let _, _, maxPrio = group |> List.maxBy (fun (_, _, prio) -> prio)

                group
                |> List.filter (fun (_, _, prio) -> prio = maxPrio)
                |> List.map (fun (c, _, _) -> c))
