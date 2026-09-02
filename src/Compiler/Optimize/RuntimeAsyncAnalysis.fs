// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncAnalysis

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

open FSharp.Compiler.RuntimeAsync

let rec private hasRuntimeAsyncFragmentBody (g: TcGlobals) (getLambdaBody: ValRef -> Expr option) visiting (vref: ValRef) =
    if List.exists ((=) vref.Stamp) visiting then
        false
    else
        match getLambdaBody vref with
        | Some body -> exprContainsRuntimeAsyncFragment g getLambdaBody (vref.Stamp :: visiting) body
        | None -> false

and private exprContainsRuntimeAsyncFragment (g: TcGlobals) (getLambdaBody: ValRef -> Expr option) visiting expr =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc then
                        true
                    else
                        match stripExpr expr with
                        | Expr.App(Expr.Val(RuntimeAsyncReturn g, _, _), _, _, _, _) -> true
                        | _ when IsRuntimeAsyncSuspensionExpr g expr -> true
                        | Expr.Val(vref, _, _) when vref.ShouldInline || vref.IsLocalRef ->
                            hasRuntimeAsyncFragmentBody g getLambdaBody visiting vref
                        | _ -> noInterceptF acc expr
        }

    FoldExpr folder false expr

let ExprContainsRuntimeAsyncFragment (g: TcGlobals) (getLambdaBody: ValRef -> Expr option) expr =
    exprContainsRuntimeAsyncFragment g getLambdaBody [] expr

let ShouldForceRuntimeAsyncInline (g: TcGlobals) runtimeAsyncContext (getLambdaBody: ValRef -> Expr option) (vref: ValRef) inlineBody =
    let containsRuntimeAsyncFragment =
        match inlineBody with
        | Some body -> ExprContainsRuntimeAsyncFragment g getLambdaBody body
        | None -> hasRuntimeAsyncFragmentBody g getLambdaBody [] vref

    if containsRuntimeAsyncFragment then
        true
    elif runtimeAsyncContext && vref.InlineIfLambda && not vref.ShouldInline then
        true
    elif not (vref.ShouldInline || vref.IsLocalRef) then
        false
    else
        hasRuntimeAsyncFragmentBody g getLambdaBody [] vref

let ShouldForceRuntimeAsyncApplication
    (g: TcGlobals)
    runtimeAsyncContext
    (getLambdaBody: ValRef -> Expr option)
    (vref: ValRef)
    inlineBody
    args
    =
    ShouldForceRuntimeAsyncInline g runtimeAsyncContext getLambdaBody vref inlineBody
    || ((vref.ShouldInline || vref.InlineIfLambda)
        && List.exists (ExprContainsRuntimeAsyncFragment g getLambdaBody) args)
    || (runtimeAsyncContext
        && vref.ShouldInline
        && List.exists
            (fun arg ->
                match stripExpr arg with
                | Expr.Lambda _
                | Expr.TyLambda _ -> true
                | _ -> false)
            args)

let InlineRuntimeAsyncLambdaArgument (g: TcGlobals) (isRuntimeAsyncFragment: Expr -> bool) expr =
    let rec isLambdaExpression expr =
        match stripExpr expr with
        | Expr.DebugPoint(_, innerExpr)
        | Expr.Let(_, innerExpr, _, _) -> isLambdaExpression innerExpr
        | Expr.Lambda _
        | Expr.TyLambda _ -> true
        | _ -> false

    let rec stripLambdaDebugPoints expr =
        match expr with
        | Expr.DebugPoint(_, innerExpr) ->
            match stripDebugPoints innerExpr with
            | Expr.Lambda _
            | Expr.TyLambda _ -> stripLambdaDebugPoints innerExpr
            | _ -> expr
        | Expr.Lambda(unique, ctorThisValOpt, baseValOpt, valParams, bodyExpr, m, overallType) ->
            match bodyExpr with
            | Expr.DebugPoint(_, innerExpr) ->
                match stripDebugPoints innerExpr with
                | Expr.Lambda _
                | Expr.TyLambda _ ->
                    Expr.Lambda(unique, ctorThisValOpt, baseValOpt, valParams, stripLambdaDebugPoints bodyExpr, m, overallType)
                | _ -> expr
            | _ -> expr
        | Expr.TyLambda(unique, typeParams, bodyExpr, m, overallType) ->
            match bodyExpr with
            | Expr.DebugPoint(_, innerExpr) ->
                match stripDebugPoints innerExpr with
                | Expr.Lambda _
                | Expr.TyLambda _ -> Expr.TyLambda(unique, typeParams, stripLambdaDebugPoints bodyExpr, m, overallType)
                | _ -> expr
            | _ -> expr
        | _ -> expr

    let rec betaReduceLambdaApplication expr =
        let rec apply f fty tyargs args m =
            match args with
            | [] -> None
            | firstArg :: rest ->
                let f = stripLambdaDebugPoints f

                match f with
                | Expr.Let(bind, body, mLet, _) -> apply body (tyOfExpr g body) tyargs args m |> Option.map (mkLetBind mLet bind)
                | Expr.Lambda(_, _, _, valParams, _, _, _) when valParams.Length = 1 && not rest.IsEmpty ->
                    let reduced = MakeApplicationAndBetaReduce g (f, fty, [ tyargs ], [ firstArg ], m)

                    match reduced with
                    | Expr.Let(bind, body, mLet, _) ->
                        match apply body (tyOfExpr g body) [] rest m with
                        | Some bodyR -> Some(mkLetBind mLet bind bodyR)
                        | None -> Some(mkAppsAux g reduced (tyOfExpr g reduced) [] rest m)
                    | _ -> Some reduced
                | Expr.Lambda _
                | Expr.TyLambda _ -> Some(MakeApplicationAndBetaReduce g (f, fty, [ tyargs ], args, m))
                | _ -> None

        match stripDebugPoints expr with
        | Expr.App(f, fty, tyargs, args, m) -> apply f fty tyargs args m
        | _ -> None

    let inlineBinding (boundVal: Val) boundExpr body =
        let rwenv =
            {
                PreIntercept =
                    Some(fun _ expr ->
                        match betaReduceLambdaApplication expr with
                        | Some reduced -> Some reduced
                        | None ->
                            match stripExpr expr with
                            | Expr.App(f, _, tyargs, args, m) ->
                                match stripDebugPoints f with
                                | Expr.Val(vref, _, _) when valEq boundVal vref.Deref ->
                                    Some(
                                        MakeApplicationAndBetaReduce
                                            g
                                            (copyExpr g CloneAll boundExpr, tyOfExpr g boundExpr, [ tyargs ], args, m)
                                    )
                                | _ -> None
                            | Expr.Val(vref, _, _) when valEq boundVal vref.Deref -> Some(copyExpr g CloneAll boundExpr)
                            | _ -> None)
                PreInterceptBinding = None
                PostTransform = betaReduceLambdaApplication
                RewriteQuotations = false
                StackGuard = StackGuard("InlineRuntimeAsyncLambdaArgument")
            }

        RewriteExpr rwenv body

    let rwenv =
        {
            PreIntercept =
                Some(fun cont expr ->
                    match stripExpr expr with
                    | Expr.Let(TBind(boundVal, boundExpr, _), body, _, _) when
                        boundVal.InlineIfLambda
                        || (isLambdaExpression boundExpr && isRuntimeAsyncFragment boundExpr)
                        ->
                        if not boundVal.InlineIfLambda then
                            boundVal.SetInlineIfLambda()

                        Some(cont (inlineBinding boundVal boundExpr body))
                    | _ -> None)
            PreInterceptBinding = None
            PostTransform = betaReduceLambdaApplication
            RewriteQuotations = false
            StackGuard = StackGuard("InlineRuntimeAsyncLambdaArgument")
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
    v.IsPinning || isByrefTy g v.Type || isByrefLikeTy g v.Range v.Type

let private TryGetRuntimeAsyncNonPreservableAlias (g: TcGlobals) expr =
    match stripExpr expr with
    | Expr.Val(vref, _, _) when IsRuntimeAsyncNonPreservableVal g vref.Deref -> Some vref.Deref
    | _ -> None

let private analyzeRuntimeAsyncExpr (g: TcGlobals) expr =
    let rec analyzeExpr expr =
        match stripExpr expr with
        | Expr.Const _
        | Expr.Val _
        | Expr.WitnessArg _
        | Expr.Lambda _
        | Expr.TyLambda _ ->
            match stripExpr expr with
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
