// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncExceptionRewrite

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let private RuntimeAsyncChoiceTy (g: TcGlobals) (ty: TType) =
    TType_app(g.choice2_tcr, [ ty; g.exn_ty ], g.knownWithoutNull)

let private RuntimeAsyncChoiceCase g m ty caseIndex expr =
    mkUnionCaseExpr (mkChoiceCaseRef g m 2 caseIndex, [ ty; g.exn_ty ], [ expr ], m)

let private RuntimeAsyncReraise m resultTy exnExpr = mkThrow m resultTy exnExpr

let private RuntimeAsyncFilterCondition m resultTy filter thenExpr elseExpr =
    let matchBuilder = MatchBuilder(DebugPointAtBinding.NoneAtInvisible, m)

    let matchCase =
        TCase(DecisionTreeTest.Const(Const.Int32 1), matchBuilder.AddResultTarget thenExpr)

    let defaultCase = matchBuilder.AddResultTarget elseExpr
    let decisionTree = TDSwitch(filter, [ matchCase ], Some defaultCase, m)
    matchBuilder.Close(decisionTree, m, resultTy)

let private IsRuntimeAsyncExceptionHandler (g: TcGlobals) expr =
    match stripExpr expr with
    | TryFinallyExpr(_, _, _, _, compensation, _) -> ExprContainsRuntimeAsyncSuspension g compensation
    | TryWithExpr(_, _, _, _, _, filter, _, handler, _) ->
        ExprContainsRuntimeAsyncSuspension g filter
        || ExprContainsRuntimeAsyncSuspension g handler
    | _ -> false

let private ExprContainsRuntimeAsyncExceptionHandler (g: TcGlobals) expr =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc || IsRuntimeAsyncExceptionHandler g expr then
                        true
                    else
                        noInterceptF acc expr
        }

    FoldExpr folder false expr

let RewriteRuntimeAsyncExceptionHandlers (g: TcGlobals) expr =
    let rewriteCapturedException m resultTy body buildResult =
        let choiceTy = RuntimeAsyncChoiceTy g resultTy
        let resultVal, _ = mkCompGenLocal m "__runtimeAsyncResult" choiceTy
        let caughtVal, _ = mkCompGenLocal m "__runtimeAsyncCaughtException" g.exn_ty
        let captured = exprForVal m resultVal

        let bodyValue =
            mkUnionCaseFieldGetUnprovenViaExprAddr (captured, mkChoiceCaseRef g m 2 0, [ resultTy; g.exn_ty ], 0, m)

        let exceptionValue =
            mkUnionCaseFieldGetUnprovenViaExprAddr (captured, mkChoiceCaseRef g m 2 1, [ resultTy; g.exn_ty ], 0, m)

        let bodySucceeded =
            mkUnionCaseTest g (captured, mkChoiceCaseRef g m 2 0, [ resultTy; g.exn_ty ], m)

        let result = buildResult bodySucceeded bodyValue exceptionValue

        mkCompGenLet
            m
            resultVal
            (mkTryWith
                g
                (RuntimeAsyncChoiceCase g m resultTy 0 body,
                 caughtVal,
                 mkTrue g m,
                 caughtVal,
                 RuntimeAsyncChoiceCase g m resultTy 1 (exprForVal m caughtVal),
                 m,
                 choiceTy,
                 DebugPointAtTry.No,
                 DebugPointAtWith.No))
            result

    let postTransform expr =
        match expr with
        | TryFinallyExpr(_, _, resultTy, body, compensation, m) when IsRuntimeAsyncExceptionHandler g expr ->
            Some(
                rewriteCapturedException m resultTy body (fun bodySucceeded bodyValue exceptionValue ->
                    let result =
                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            m
                            resultTy
                            bodySucceeded
                            bodyValue
                            (RuntimeAsyncReraise m resultTy exceptionValue)

                    mkCompGenSequential m compensation result)
            )
        | TryWithExpr(_, _, resultTy, body, filterVal, filter, handlerVal, handler, m) when IsRuntimeAsyncExceptionHandler g expr ->
            Some(
                rewriteCapturedException m resultTy body (fun bodySucceeded bodyValue exceptionExpr ->
                    let filter =
                        mkCompGenLet
                            m
                            filterVal
                            exceptionExpr
                            (mkCompGenLet
                                m
                                handlerVal
                                exceptionExpr
                                (RuntimeAsyncFilterCondition m resultTy filter handler (RuntimeAsyncReraise m resultTy exceptionExpr)))

                    mkCond DebugPointAtBinding.NoneAtInvisible m resultTy bodySucceeded bodyValue filter)
            )
        | _ -> None

    if ExprContainsRuntimeAsyncExceptionHandler g expr then
        RewriteExpr
            {
                PreIntercept = None
                PostTransform = postTransform
                PreInterceptBinding = None
                RewriteQuotations = false
                StackGuard = StackGuard("RuntimeAsyncExceptionRewrite")
            }
            expr
    else
        expr
