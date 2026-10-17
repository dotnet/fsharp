// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncAnalysis

open System.Collections.Generic

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

let rec private containsRecipeConstruction visit expr =
    match stripDebugPoints expr with
    | Expr.Lambda _
    | Expr.TyLambda _ -> false
    | Expr.Let(binding, body, _, _) -> visit binding.Expr || containsRecipeConstruction visit body
    | Expr.Sequential(first, rest, _, _) -> visit first || containsRecipeConstruction visit rest
    | _ -> visit expr

type RuntimeAsyncAnalyzer(g: TcGlobals, getLambdaBody: ValRef -> Expr option) =
    let expressionCache = Dictionary<Expr, bool>(HashIdentity.Reference)
    let suspensionCache = Dictionary<Expr, bool>(HashIdentity.Reference)
    let valueCache = Dictionary<Stamp, bool>()
    let visitingValues = HashSet<Stamp>()
    let stackGuard = StackGuard("RuntimeAsyncAnalyzer")

    let rec containsValue (vref: ValRef) =
        match valueCache.TryGetValue vref.Stamp with
        | true, result -> result, true
        | _ when visitingValues.Contains vref.Stamp -> false, false
        | _ ->
            visitingValues.Add vref.Stamp |> ignore

            let result, complete =
                match getLambdaBody vref with
                | Some body -> containsExpression body
                | None -> false, true

            visitingValues.Remove vref.Stamp |> ignore

            if complete then
                valueCache[vref.Stamp] <- result

            result, complete

    and containsExpression expr =
        stackGuard.Guard(fun () -> containsExpressionCore expr)

    and containsExpressionCore expr =
        match expressionCache.TryGetValue expr with
        | true, result -> result, true
        | _ ->
            let mutable complete = true

            let folder =
                { ExprFolder0 with
                    exprIntercept =
                        fun _ noInterceptF acc expr ->
                            if acc then
                                true
                            elif IsRuntimeAsyncBoundary g expr || (TryGetRuntimeAsyncSequence g expr).IsSome then
                                true
                            else
                                match stripExpr expr with
                                | Expr.Val(vref, _, _) when vref.ShouldInline || vref.IsLocalRef ->
                                    let result, valueComplete = containsValue vref

                                    if not valueComplete then
                                        complete <- false

                                    result
                                | _ -> noInterceptF acc expr
                }

            let result = FoldExpr folder false expr

            if complete then
                expressionCache[expr] <- result

            result, complete

    new(g: TcGlobals) = RuntimeAsyncAnalyzer(g, fun _ -> None)

    member _.ContainsFragment expr = containsExpression expr |> fst

    member this.ContainsSuspension expr =
        stackGuard.Guard(fun () -> this.ContainsSuspensionCore expr)

    member private this.ContainsSuspensionCore expr =
        match suspensionCache.TryGetValue expr with
        | true, result -> result
        | _ ->
            let folder =
                { ExprFolder0 with
                    exprIntercept =
                        fun _ noInterceptF acc expr ->
                            if acc then
                                true
                            else
                                match TryGetRuntimeAsyncSequence g expr with
                                | Some(recipe, _) -> containsRecipeConstruction this.ContainsSuspension recipe
                                | None -> IsRuntimeAsyncSuspensionExpr g expr || noInterceptF acc expr
                }

            let result = FoldExpr folder false expr
            suspensionCache[expr] <- result
            result

let ShouldForceRuntimeAsyncInline (analyzer: RuntimeAsyncAnalyzer) runtimeAsyncContext (vref: ValRef) inlineBody =
    let containsRuntimeAsyncFragment =
        match inlineBody with
        | Some body -> analyzer.ContainsFragment body
        | None -> analyzer.ContainsFragment(exprForValRef vref.Range vref)

    if containsRuntimeAsyncFragment then
        true
    elif runtimeAsyncContext && vref.InlineIfLambda && not vref.ShouldInline then
        true
    elif not (vref.ShouldInline || vref.IsLocalRef) then
        false
    else
        analyzer.ContainsFragment(exprForValRef vref.Range vref)

let ShouldForceRuntimeAsyncApplication (analyzer: RuntimeAsyncAnalyzer) runtimeAsyncContext (vref: ValRef) inlineBody args =
    ShouldForceRuntimeAsyncInline analyzer runtimeAsyncContext vref inlineBody
    || ((vref.ShouldInline || vref.InlineIfLambda)
        && List.exists analyzer.ContainsFragment args)
    || (runtimeAsyncContext
        && vref.ShouldInline
        && List.exists
            (fun arg ->
                match stripExpr arg with
                | Expr.Lambda _
                | Expr.TyLambda _ -> true
                | _ -> false)
            args)

let ReduceRuntimeAsyncReturnedClosureApplications (g: TcGlobals) (analyzer: RuntimeAsyncAnalyzer) expr =
    let stackGuard = StackGuard("ReduceRuntimeAsyncReturnedClosureApplications")

    let rec effectFree expr =
        match stripExpr expr with
        | Expr.Const _
        | Expr.Lambda _
        | Expr.TyLambda _ -> true
        | Expr.Val(vref, _, _) -> not vref.IsMutable && not vref.IsTypeFunction
        | Expr.App(funcExpr, _, _, [], _) -> effectFree funcExpr
        | _ -> false

    let rec apply f fty tyargs args m =
        stackGuard.Guard(fun () -> applyCore f fty tyargs args m)

    and applyCore f fty tyargs args m =
        match f, args with
        | Expr.DebugPoint(_, inner), _ when
            match stripDebugPoints inner with
            | Expr.Lambda _ -> true
            | _ -> false
            ->
            apply inner fty tyargs args m
        | Expr.DebugPoint(_, inner), _ -> apply inner fty tyargs args m |> Option.map (RebuildRuntimeAsyncDebugWrapper f)
        | RuntimeAsyncConstructionPrefix(body, rebuild), _ -> apply body (tyOfExpr g body) tyargs args m |> Option.map rebuild
        | Expr.LetRec(bindings, body, mLet, _), _ ->
            apply body (tyOfExpr g body) tyargs args m
            |> Option.map (mkLetRecBinds mLet bindings)
        | Expr.Match(point, matchRange, tree, targets, mMatch, _), _ when List.forall effectFree args ->
            // Each target after the first needs its own copy of any values bound by the arguments.
            TryMapRuntimeAsyncMatchTargets g (point, matchRange, tree, targets, mMatch) (fun i body ->
                let args = if i = 0 then args else List.map (copyExpr g CloneAll) args
                apply body (tyOfExpr g body) tyargs args m)
        | Expr.Lambda(_, _, _, [ _ ], _, _, _), first :: (_ :: _ as rest) when List.forall effectFree rest ->
            let reduced = MakeApplicationAndBetaReduce g (f, fty, [ tyargs ], [ first ], m)
            apply reduced (tyOfExpr g reduced) [] rest m
        | Expr.Lambda _, _ :: _ -> Some(MakeApplicationAndBetaReduce g (f, fty, [ tyargs ], args, m))
        | _ when not (analyzer.ContainsSuspension f) && List.forall effectFree args -> Some(mkAppsAux g f fty [ tyargs ] args m)
        | _ -> None

    let rwenv =
        {
            PreIntercept = None
            PreInterceptBinding = None
            PostTransform =
                (fun expression ->
                    match expression with
                    | Expr.App((Expr.Lambda _ | Expr.Let _ | Expr.LetRec _ | Expr.Match _ | RuntimeAsyncDebugWrapper _) as f,
                               fty,
                               tyargs,
                               args,
                               m) when not args.IsEmpty && analyzer.ContainsSuspension expression -> apply f fty tyargs args m
                    | _ -> None)
            RewriteQuotations = false
            StackGuard = stackGuard
        }

    RewriteExpr rwenv expr

let PreserveRuntimeAsyncCallSiteDebugPoint (g: TcGlobals) m expr =
    let rwenv =
        {
            PreIntercept = None
            PreInterceptBinding = None
            PostTransform =
                (fun expression ->
                    match expression, TryGetRuntimeAsyncReturn g expression with
                    | Expr.App(f, fty, tyargs, [ body ], range), Some _ ->
                        match body with
                        | Expr.DebugPoint _ -> None
                        | _ -> Some(Expr.App(f, fty, tyargs, [ mkDebugPoint m body ], range))
                    | _ -> None)
            RewriteQuotations = false
            StackGuard = StackGuard("PreserveRuntimeAsyncCallSiteDebugPoint")
        }

    RewriteExpr rwenv expr

type private RuntimeAsyncFlowSummary =
    {
        MaySuspend: bool
        UsedAfterSuspend: FreeLocals
        FreeLocals: FreeLocals
    }

let private emptyRuntimeAsyncFlowSummary =
    {
        MaySuspend = false
        UsedAfterSuspend = emptyFreeLocals
        FreeLocals = emptyFreeLocals
    }

let private mergeRuntimeAsyncFlowSummaries left right =
    {
        MaySuspend = left.MaySuspend || right.MaySuspend
        UsedAfterSuspend = Zset.union left.UsedAfterSuspend right.UsedAfterSuspend
        FreeLocals = Zset.union left.FreeLocals right.FreeLocals
    }

let private sequenceRuntimeAsyncFlowSummaries left right =
    {
        MaySuspend = left.MaySuspend || right.MaySuspend
        UsedAfterSuspend =
            if left.MaySuspend then
                Zset.union left.UsedAfterSuspend (Zset.union right.UsedAfterSuspend right.FreeLocals)
            else
                Zset.union left.UsedAfterSuspend right.UsedAfterSuspend
        FreeLocals = Zset.union left.FreeLocals right.FreeLocals
    }

let private removeRuntimeAsyncBoundVals vals (summary: RuntimeAsyncFlowSummary) =
    let remove vals set =
        (set, vals) ||> List.fold (fun set v -> Zset.remove v set)

    { summary with
        FreeLocals = remove vals summary.FreeLocals
    }

let private addRuntimeAsyncSuspension summary = { summary with MaySuspend = true }

let private IsRuntimeAsyncNonPreservableVal (g: TcGlobals) (v: Val) =
    v.IsPinning || v.IsFixed || isByrefTy g v.Type || isByrefLikeTy g v.Range v.Type

let private TryGetRuntimeAsyncNonPreservableAlias (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Val(vref, _, _) when IsRuntimeAsyncNonPreservableVal g vref.Deref -> Some vref.Deref
    | _ -> None

let private analyzeRuntimeAsyncExpr (g: TcGlobals) expr =
    let cache = Dictionary<Expr, RuntimeAsyncFlowSummary>(HashIdentity.Reference)
    let stackGuard = StackGuard("AnalyzeRuntimeAsyncFlow")

    let rec analyzeExpr expr =
        match cache.TryGetValue expr with
        | true, summary -> summary
        | _ ->
            let summary = stackGuard.Guard(fun () -> analyzeExprCore expr)
            cache[expr] <- summary
            summary

    and analyzeExprCore expr =
        let expr = stripExpr expr

        match expr with
        | Expr.Const _
        | Expr.Val _
        | Expr.WitnessArg _
        | Expr.Lambda _
        | Expr.TyLambda _ ->
            match expr with
            | Expr.Val(vref, _, _) ->
                { emptyRuntimeAsyncFlowSummary with
                    FreeLocals = Zset.add vref.Deref (Zset.empty valOrder)
                }
            | _ -> emptyRuntimeAsyncFlowSummary

        | Expr.Sequential(expr1, expr2, _, _) -> sequenceRuntimeAsyncFlowSummaries (analyzeExpr expr1) (analyzeExpr expr2)

        | Expr.Let(TBind(v, rhs, _), body, _, _) ->
            let rhsSummary = analyzeExpr rhs
            let bodySummary = removeRuntimeAsyncBoundVals [ v ] (analyzeExpr body)

            let bodySummary =
                match TryGetRuntimeAsyncNonPreservableAlias g rhs with
                | Some source when Zset.contains v bodySummary.UsedAfterSuspend ->
                    { bodySummary with
                        UsedAfterSuspend = Zset.add source bodySummary.UsedAfterSuspend
                    }
                | _ -> bodySummary

            sequenceRuntimeAsyncFlowSummaries rhsSummary bodySummary

        | Expr.LetRec(bindings, body, _, _) ->
            let bindingSummary =
                (emptyRuntimeAsyncFlowSummary, bindings)
                ||> List.fold (fun summary (TBind(_, bindingExpr, _)) ->
                    sequenceRuntimeAsyncFlowSummaries summary (analyzeExpr bindingExpr))

            let bodySummary = analyzeExpr body
            let boundVals = bindings |> List.map (fun binding -> binding.Var)
            let bodySummary = removeRuntimeAsyncBoundVals boundVals bodySummary
            sequenceRuntimeAsyncFlowSummaries bindingSummary bodySummary

        | Expr.Match(_, _, decisionTree, targets, _, _) -> analyzeDecisionTree targets decisionTree

        | Expr.Op(TOp.While _, _, [ Expr.Lambda(_, _, _, _, guardExpr, _, _); Expr.Lambda(_, _, _, _, bodyExpr, _, _) ], _) ->
            let guardSummary = analyzeExpr guardExpr
            let bodySummary = analyzeExpr bodyExpr
            let loopSummary = mergeRuntimeAsyncFlowSummaries guardSummary bodySummary

            if loopSummary.MaySuspend then
                { loopSummary with
                    UsedAfterSuspend = Zset.union loopSummary.UsedAfterSuspend loopSummary.FreeLocals
                }
            else
                loopSummary

        | Expr.Op(TOp.IntegerForLoop _,
                  _,
                  [ Expr.Lambda(_, _, _, _, startExpr, _, _)
                    Expr.Lambda(_, _, _, _, finishExpr, _, _)
                    Expr.Lambda(_, _, _, [ loopVal ], bodyExpr, _, _) ],
                  _) ->
            let loopSummary =
                [ analyzeExpr startExpr; analyzeExpr finishExpr; analyzeExpr bodyExpr ]
                |> List.reduce sequenceRuntimeAsyncFlowSummaries
                |> removeRuntimeAsyncBoundVals [ loopVal ]

            if loopSummary.MaySuspend then
                { loopSummary with
                    UsedAfterSuspend = Zset.union loopSummary.UsedAfterSuspend loopSummary.FreeLocals
                }
            else
                loopSummary

        | Expr.Op(TOp.TryFinally _, _, [ Expr.Lambda(_, _, _, _, bodyExpr, _, _); Expr.Lambda(_, _, _, _, compensationExpr, _, _) ], _) ->
            let bodySummary = analyzeExpr bodyExpr
            let compensationSummary = analyzeExpr compensationExpr
            let summary = mergeRuntimeAsyncFlowSummaries bodySummary compensationSummary

            if bodySummary.MaySuspend then
                { summary with
                    UsedAfterSuspend = Zset.union summary.UsedAfterSuspend compensationSummary.FreeLocals
                }
            else
                summary

        | Expr.Op(TOp.TryWith _,
                  _,
                  [ Expr.Lambda(_, _, _, _, bodyExpr, _, _)
                    Expr.Lambda(_, _, _, [ _ ], filterExpr, _, _)
                    Expr.Lambda(_, _, _, [ _ ], handlerExpr, _, _) ],
                  _) ->
            let bodySummary = analyzeExpr bodyExpr
            let filterSummary = analyzeExpr filterExpr
            let handlerSummary = analyzeExpr handlerExpr

            let summary =
                mergeRuntimeAsyncFlowSummaries bodySummary (mergeRuntimeAsyncFlowSummaries filterSummary handlerSummary)

            let usedAfterSuspend =
                summary.UsedAfterSuspend
                |> fun used ->
                    if bodySummary.MaySuspend then
                        Zset.union used (Zset.union filterSummary.FreeLocals handlerSummary.FreeLocals)
                    else
                        used
                |> fun used ->
                    if filterSummary.MaySuspend then
                        Zset.union used handlerSummary.FreeLocals
                    else
                        used

            { summary with
                UsedAfterSuspend = usedAfterSuspend
            }

        | Expr.Op(TOp.LValueOp(_, vref), _, args, _) ->
            let argsSummary =
                (emptyRuntimeAsyncFlowSummary, args)
                ||> List.fold (fun summary arg -> sequenceRuntimeAsyncFlowSummaries summary (analyzeExpr arg))

            { argsSummary with
                FreeLocals = Zset.add vref.Deref argsSummary.FreeLocals
            }

        | Expr.Op(_op, _, args, _) ->
            let argsSummary =
                (emptyRuntimeAsyncFlowSummary, args)
                ||> List.fold (fun summary arg ->
                    match stripExpr arg with
                    | Expr.Lambda _
                    | Expr.TyLambda _ -> summary
                    | _ -> sequenceRuntimeAsyncFlowSummaries summary (analyzeExpr arg))

            if IsRuntimeAsyncSuspensionExpr g expr then
                addRuntimeAsyncSuspension argsSummary
            else
                argsSummary

        | Expr.App(funcExpr, _, _, argGroups, _) ->
            let funcSummary = analyzeExpr funcExpr

            (funcSummary, argGroups)
            ||> List.fold (fun summary arg -> sequenceRuntimeAsyncFlowSummaries summary (analyzeExpr arg))

        | Expr.Obj(_, _, _, ctorCall, _, _, _) -> analyzeExpr ctorCall

        | Expr.StaticOptimization(_, expr1, expr2, _) -> mergeRuntimeAsyncFlowSummaries (analyzeExpr expr1) (analyzeExpr expr2)

        | Expr.Quote(_, splices, _, _, _) ->
            let analyzeSplices (_, _, exprs, _) =
                exprs
                |> List.map analyzeExpr
                |> List.fold mergeRuntimeAsyncFlowSummaries emptyRuntimeAsyncFlowSummary

            match splices.Value with
            | None -> emptyRuntimeAsyncFlowSummary
            | Some(data1, data2) -> mergeRuntimeAsyncFlowSummaries (analyzeSplices data1) (analyzeSplices data2)

        | Expr.Link eref -> analyzeExpr eref.Value

        | Expr.DebugPoint(_, innerExpr) -> analyzeExpr innerExpr

        | Expr.TyChoose(_, innerExpr, _) -> analyzeExpr innerExpr

    and analyzeDecisionTree targets decisionTree =
        let analyzeTarget targetNum =
            let (TTarget(boundVals, targetExpr, _)) = targets[targetNum]
            analyzeExpr targetExpr |> removeRuntimeAsyncBoundVals boundVals

        let rec analyzeTree tree =
            stackGuard.Guard(fun () -> analyzeTreeCore tree)

        and analyzeTreeCore tree =
            match tree with
            | TDSuccess(results, targetNum) ->
                let resultSummary =
                    (emptyRuntimeAsyncFlowSummary, results)
                    ||> List.fold (fun summary resultExpr -> sequenceRuntimeAsyncFlowSummaries summary (analyzeExpr resultExpr))

                sequenceRuntimeAsyncFlowSummaries resultSummary (analyzeTarget targetNum)

            | TDBind(TBind(v, bindingExpr, _), rest) ->
                let bindingSummary = analyzeExpr bindingExpr
                let restSummary = analyzeTree rest |> removeRuntimeAsyncBoundVals [ v ]
                sequenceRuntimeAsyncFlowSummaries bindingSummary restSummary

            | TDSwitch(inputExpr, cases, defaultOpt, _) ->
                let inputSummary = analyzeExpr inputExpr
                let branches = cases |> List.map (fun (TCase(_, tree)) -> analyzeTree tree)

                let branches =
                    match defaultOpt with
                    | Some tree -> analyzeTree tree :: branches
                    | None -> branches

                let branchSummary =
                    branches
                    |> List.fold mergeRuntimeAsyncFlowSummaries emptyRuntimeAsyncFlowSummary

                sequenceRuntimeAsyncFlowSummaries inputSummary branchSummary

        analyzeTree decisionTree

    analyzeExpr expr

let GetRuntimeAsyncNonPreservableUses (g: TcGlobals) expr =
    let summary = analyzeRuntimeAsyncExpr g expr

    summary.UsedAfterSuspend
    |> Zset.elements
    |> List.filter (IsRuntimeAsyncNonPreservableVal g)

let RestoreRuntimeAsyncPinning expr =
    let containsFixedBinding (expr: Expr) =
        ExistsExpr
            (fun expr ->
                match stripExpr expr with
                | Expr.Let(TBind(v, _, _), _, _, _)
                | Expr.LetRec([ TBind(v, _, _) ], _, _, _) -> v.IsFixed
                | Expr.Val(vref, _, _) -> vref.Deref.IsFixed
                | _ -> false)
            expr

    let restoreBindingPinning (v: Val) (rhs: Expr) =
        if containsFixedBinding rhs && not v.IsPinning then
            v.SetIsPinning()

    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF () expr ->
                    match stripExpr expr with
                    | Expr.Let(TBind(v, rhs, _), _, _, _) -> restoreBindingPinning v rhs
                    | Expr.LetRec(bindings, _, _, _) ->
                        for TBind(v, rhs, _) in bindings do
                            restoreBindingPinning v rhs
                    | _ -> ()

                    noInterceptF () expr
        }

    FoldExpr folder () expr |> ignore
