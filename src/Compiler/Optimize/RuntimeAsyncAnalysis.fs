// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncAnalysis

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

let IsRuntimeAsyncSuspensionExpr expr =
    match stripExpr expr with
    | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethodRef, _, _, _), _, _, _) ->
        ilMethodRef.DeclaringTypeRef.FullName = "System.Runtime.CompilerServices.AsyncHelpers"
        && ilMethodRef.Name
           |> function
               | "Await"
               | "AwaitAwaiter"
               | "UnsafeAwaitAwaiter" -> true
               | _ -> false
    | _ -> false

let ExprContainsRuntimeAsyncSuspension expr =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc || IsRuntimeAsyncSuspensionExpr expr then
                        true
                    else
                        noInterceptF acc expr
        }

    FoldExpr folder false expr

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

            if IsRuntimeAsyncSuspensionExpr expr then
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
