// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncAnalysis

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open System.Collections.Generic

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

open FSharp.Compiler.RuntimeAsync

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

/// Rebuilds a match with every target rewritten, or returns None if any target cannot be.
let private tryMapMatchTargets (g: TcGlobals) (point, matchRange, tree, targets: DecisionTreeTarget array, m) mapTarget =
    let targets =
        targets
        |> Array.mapi (fun i (TTarget(vals, body, flags)) -> mapTarget i body |> Option.map (fun body -> TTarget(vals, body, flags)))

    if targets.Length > 0 && Array.forall Option.isSome targets then
        let targets = Array.map Option.get targets
        Some(Expr.Match(point, matchRange, tree, targets, m, tyOfExpr g targets[0].TargetExpression))
    else
        None

let private isQuotationUsing (v: Val) expr =
    match expr with
    | Expr.Quote(body, _, _, _, _) ->
        ExistsExpr
            (function
            | Expr.Val(vref, _, _) -> valEq v vref.Deref
            | _ -> false)
            body
    | _ -> false

let ReduceRuntimeAsyncReturnedClosureApplications (g: TcGlobals) (analyzer: RuntimeAsyncAnalyzer) expr =
    let rec effectFree expr =
        match stripExpr expr with
        | Expr.Const _
        | Expr.Lambda _
        | Expr.TyLambda _ -> true
        | Expr.Val(vref, _, _) -> not vref.IsMutable && not vref.IsTypeFunction
        | Expr.App(funcExpr, _, _, [], _) -> effectFree funcExpr
        | _ -> false

    let rec apply f fty tyargs args m =
        match f, args with
        | Expr.DebugPoint(_, inner), _ when
            match stripDebugPoints inner with
            | Expr.Lambda _ -> true
            | _ -> false
            ->
            apply inner fty tyargs args m
        | Expr.Let(binding, body, mLet, _), _ ->
            apply body (tyOfExpr g body) tyargs args m
            |> Option.map (mkLetBind mLet binding)
        | Expr.Sequential(first, rest, NormalSeq, mSeq), _ ->
            apply rest (tyOfExpr g rest) tyargs args m
            |> Option.map (fun rest -> Expr.Sequential(first, rest, NormalSeq, mSeq))
        | Expr.Match(point, matchRange, tree, targets, mMatch, _), _ when List.forall effectFree args ->
            // Each target after the first needs its own copy of any values bound by the arguments.
            tryMapMatchTargets g (point, matchRange, tree, targets, mMatch) (fun i body ->
                let args = if i = 0 then args else List.map (copyExpr g CloneAll) args
                apply body (tyOfExpr g body) tyargs args m)
        | Expr.Lambda(_, _, _, [ _ ], _, _, _), first :: (_ :: _ as rest) when List.forall effectFree rest ->
            let reduced = MakeApplicationAndBetaReduce g (f, fty, [ tyargs ], [ first ], m)

            let rec applyRest expr =
                match expr with
                | Expr.Let(binding, body, mLet, _) -> applyRest body |> Option.map (mkLetBind mLet binding)
                | _ -> apply expr (tyOfExpr g expr) [] rest m

            applyRest reduced
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
                    | Expr.App((Expr.Lambda _ | Expr.Let _ | Expr.Match _ | Expr.DebugPoint _) as f, fty, tyargs, args, m) when
                        not args.IsEmpty && analyzer.ContainsSuspension expression
                        ->
                        apply f fty tyargs args m
                    | _ -> None)
            RewriteQuotations = false
            StackGuard = StackGuard("ReduceRuntimeAsyncReturnedClosureApplications")
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

let OutlineRuntimeAsyncCallback (g: TcGlobals) (analyzer: RuntimeAsyncAnalyzer) runtimeAsyncContext prepareBody (expr: Expr) =
    let rec outlineBranches resultTy expr =
        match expr with
        | Expr.Lambda(_, None, None, [ parameter ], body, m, _)
        | NewDelegateExpr g (_, [ parameter ], body, m, _) ->
            let marker = g.cgh__runtimeAsyncReturnValueTask_vref
            let body = prepareBody body

            let markedBody =
                primMkApp (exprForValRef m marker, marker.Type) [ resultTy ] [ body ] m

            Some(mkLambda m parameter (markedBody, tyOfExpr g markedBody))
        | Expr.DebugPoint(point, body) ->
            outlineBranches resultTy body
            |> Option.map (fun body -> Expr.DebugPoint(point, body))
        | Expr.Sequential(first, rest, NormalSeq, m) ->
            outlineBranches resultTy rest
            |> Option.map (fun rest -> Expr.Sequential(first, rest, NormalSeq, m))
        | Expr.Let(binding, rest, m, _) -> outlineBranches resultTy rest |> Option.map (mkLetBind m binding)
        | Expr.Match(point, matchRange, tree, targets, m, _) ->
            tryMapMatchTargets g (point, matchRange, tree, targets, m) (fun _ -> outlineBranches resultTy)
        | _ -> None

    let rec tryDelegateSignature expr =
        match expr with
        | NewDelegateExpr g (_, [ parameter ], body, _, _) -> Some(parameter.Type, tyOfExpr g body)
        | Expr.DebugPoint(_, rest)
        | Expr.Sequential(_, rest, NormalSeq, _)
        | Expr.Let(_, rest, _, _) -> tryDelegateSignature rest
        | Expr.Match(_, _, _, targets, _, _) when targets.Length > 0 ->
            let (TTarget(_, body, _)) = targets[0]
            tryDelegateSignature body
        | _ -> None

    let isTrivialValue =
        function
        | Expr.Const _ -> true
        | Expr.Val(vref, _, _) -> vref.IsLocalRef && not vref.IsMutable && not vref.IsTypeFunction
        | _ -> false

    // Construction moves to the invocation, so its evaluation must not have observable effects.
    let rec inlineDelegateInvoke (invokeRef, invokeTy, tyargs, arg, callRange) construction =
        match construction with
        | NewDelegateExpr g (_, [ _ ], _, _, _) ->
            Some(MakeFSharpDelegateInvokeAndTryBetaReduce g (invokeRef, construction, invokeTy, tyargs, arg, callRange))
        | Expr.DebugPoint(point, rest) ->
            inlineDelegateInvoke (invokeRef, invokeTy, tyargs, arg, callRange) rest
            |> Option.map (fun body -> Expr.DebugPoint(point, body))
        | Expr.Sequential(first, rest, NormalSeq, m) when isTrivialValue first ->
            inlineDelegateInvoke (invokeRef, invokeTy, tyargs, arg, callRange) rest
            |> Option.map (fun body -> Expr.Sequential(first, body, NormalSeq, m))
        | Expr.Let((TBind(_, rhs, _)) as binding, rest, m, _) when isTrivialValue rhs ->
            inlineDelegateInvoke (invokeRef, invokeTy, tyargs, arg, callRange) rest
            |> Option.map (mkLetBind m binding)
        | Expr.Match(point,
                     matchRange,
                     (TDSwitch(Expr.Val(vref, _, _), [ TCase(_, TDSuccess([], _)) ], Some(TDSuccess([], _)), _) as tree),
                     targets,
                     m,
                     _) when vref.IsLocalRef && not vref.IsMutable && not vref.IsTypeFunction ->
            tryMapMatchTargets g (point, matchRange, tree, targets, m) (fun _ ->
                inlineDelegateInvoke (invokeRef, invokeTy, tyargs, arg, callRange))
        | _ -> None

    // A residual Invoke can surface after ordinary optimization; try its single use before outlining.
    let tryInlineDelegate callback construction continuation =
        let mutable invocations = 0
        let mutable invalidUse = false

        let rwenv =
            {
                PreIntercept =
                    Some(fun rewrite expression ->
                        match expression with
                        | DelegateInvokeExpr g (invokeRef, invokeTy, tyargs, Expr.Val(vref, _, _), arg, callRange) when
                            valEq callback vref.Deref
                            ->
                            match inlineDelegateInvoke (invokeRef, invokeTy, tyargs, rewrite arg, callRange) construction with
                            | Some body ->
                                invocations <- invocations + 1
                                Some body
                            | None ->
                                invalidUse <- true
                                Some expression
                        | Expr.Val(vref, _, _) when valEq callback vref.Deref ->
                            invalidUse <- true
                            Some expression
                        | Expr.Quote _ when isQuotationUsing callback expression ->
                            invalidUse <- true
                            Some expression
                        | _ -> None)
                PreInterceptBinding = None
                PostTransform = (fun _ -> None)
                RewriteQuotations = false
                StackGuard = StackGuard("InlineRuntimeAsyncDelegateInvocation")
            }

        let continuation = RewriteExpr rwenv continuation

        if invocations = 1 && not invalidUse then
            Some continuation
        else
            None

    let stackGuard = StackGuard("OutlineRuntimeAsyncCallback")

    let rec outline expr =
        stackGuard.Guard(fun () -> outlineCore expr)

    and outlineCore expr =
        match expr with
        | Expr.Let(TBind(callback, construction, point), continuation, m, _) when
            callback.InlineIfLambda && analyzer.ContainsSuspension construction
            ->
            let keep construction =
                mkLetBind m (TBind(callback, construction, point)) (outline continuation)

            let canOutline =
                runtimeAsyncContext || (TryGetRuntimeAsyncReturn g continuation).IsSome

            let shape =
                match tryDestFunTy g callback.Type with
                | ValueSome(argTy, resultTy) -> Some(argTy, resultTy, false)
                | ValueNone when isFSharpDelegateTy g callback.Type ->
                    tryDelegateSignature construction
                    |> Option.map (fun (argTy, resultTy) -> argTy, resultTy, true)
                | _ -> None

            match shape with
            | Some(argTy, resultTy, isDelegate) when canOutline && not (isByrefLikeTy g m argTy || isByrefLikeTy g m resultTy) ->
                let freeVals = (freeInExpr CollectLocals construction).FreeLocals

                let canCapture =
                    freeVals
                    |> Zset.forall (fun v -> not v.IsPinning && not (isByrefTy g v.Type || isByrefLikeTy g m v.Type))

                let construction = if canCapture then outline construction else construction

                let inlined =
                    if canCapture && isDelegate then
                        tryInlineDelegate callback construction continuation
                    else
                        None

                let outlinedConstruction =
                    if canCapture && Option.isNone inlined then
                        outlineBranches resultTy construction
                    else
                        None

                match inlined, outlinedConstruction with
                | Some body, _ -> outline body
                | None, Some callbackConstruction ->
                    let callbackTy = tyOfExpr g callbackConstruction
                    let outlined, outlinedExpr = mkCompGenLocal m "runtimeAsyncCallback" callbackTy
                    let mutable invalidUse = false

                    let rwenv =
                        {
                            PreIntercept =
                                Some(fun rewrite expression ->
                                    let call =
                                        match expression with
                                        | Expr.App(Expr.Val(vref, _, _), _, [], [ arg ], callRange) when
                                            not isDelegate && valEq callback vref.Deref
                                            ->
                                            Some(arg, callRange)
                                        | DelegateInvokeExpr g (_, _, _, Expr.Val(vref, _, _), arg, callRange) when
                                            isDelegate && valEq callback vref.Deref
                                            ->
                                            Some(arg, callRange)
                                        | _ -> None

                                    match call with
                                    | Some(arg, callRange) ->
                                        let invocation =
                                            mkApps g ((outlinedExpr, callbackTy), [], [ rewrite arg ], callRange)

                                        Some(mkRuntimeAsyncAwaitValueTask g callRange (tyOfExpr g expression) invocation)
                                    | None ->
                                        match expression with
                                        | Expr.Val(vref, _, _) when valEq callback vref.Deref ->
                                            invalidUse <- true
                                            Some expression
                                        | Expr.Quote _ when isQuotationUsing callback expression ->
                                            invalidUse <- true
                                            Some expression
                                        | _ -> None)
                            PreInterceptBinding = None
                            PostTransform = (fun _ -> None)
                            RewriteQuotations = false
                            StackGuard = stackGuard
                        }

                    let continuation = RewriteExpr rwenv continuation

                    if invalidUse then
                        keep construction
                    else
                        mkLet point m outlined callbackConstruction (outline continuation)
                | None, None -> keep construction
            | _ -> keep construction
        | Expr.Let(binding, continuation, m, _) -> mkLetBind m binding (outline continuation)
        | Expr.DebugPoint(point, inner) -> Expr.DebugPoint(point, outline inner)
        | Expr.Sequential(first, rest, NormalSeq, m) -> Expr.Sequential(first, outline rest, NormalSeq, m)
        | _ -> expr

    outline expr

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

    let rec analyzeExpr expr =
        match cache.TryGetValue expr with
        | true, summary -> summary
        | _ ->
            let summary = analyzeExprCore expr
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
