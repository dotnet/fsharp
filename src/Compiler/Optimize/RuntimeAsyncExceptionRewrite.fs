// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.RuntimeAsyncExceptionRewrite

open FSharp.Compiler
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.RuntimeAsyncAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

let private callEdi (g: TcGlobals) amap m name resultTy (objArgs: Expr list) (args: Expr list) =
    let ediTy =
        g.system_ExceptionDispatchInfo_ty
        |> Option.defaultWith (fun () -> error (Error(FSComp.SR.ilRuntimeAsyncSequenceNotStaticallyKnown (), m)))

    let signature = List.map (tyOfExpr g) args @ [ resultTy ]

    let methodInfo =
        TryFindIntrinsicMethInfo (InfoReader(g, amap)) m AccessorDomain.AccessibleFromEverywhere name ediTy
        |> List.tryFind (fun methodInfo ->
            let methodSignature =
                List.concat (methodInfo.GetParamTypes(amap, m, []))
                @ [ methodInfo.GetFSharpReturnType(amap, m, []) ]

            methodInfo.IsInstance = not objArgs.IsEmpty
            && methodSignature.Length = signature.Length
            && List.forall2 (typeEquiv g) methodSignature signature)
        |> Option.defaultWith (fun () -> error (Error(FSComp.SR.ilRuntimeAsyncSequenceNotStaticallyKnown (), m)))

    MakeMethInfoCall amap m methodInfo [] (objArgs @ args) None

/// ExceptionDispatchInfo.Capture(exn).Throw(), so a pending exception keeps the stack trace of its original throw.
let RethrowRuntimeAsyncException (g: TcGlobals) amap m exnExpr =
    let edi =
        callEdi g amap m "Capture" (Option.get g.system_ExceptionDispatchInfo_ty) [] [ exnExpr ]

    callEdi g amap m "Throw" g.unit_ty [ edi ] []

/// Rethrows a pending exception in a position of type `resultTy`; the trailing default value is unreachable.
let private RethrowAs g amap m resultTy exnExpr =
    mkCompGenSequential m (RethrowRuntimeAsyncException g amap m exnExpr) (mkDefault (m, resultTy))

let private RewriteReraise g amap handlerVal handler =
    RewriteExpr
        {
            PreIntercept =
                Some(fun recurse expr ->
                    match stripExpr expr with
                    | TryWithExpr(spTry, spWith, nestedTy, body, filterVal, filter, innerHandlerVal, innerHandler, m) ->
                        Some(mkTryWith g (recurse body, filterVal, filter, innerHandlerVal, innerHandler, m, nestedTy, spTry, spWith))
                    | Expr.Op(TOp.Reraise, _, _, m) -> Some(RethrowAs g amap m (tyOfExpr g expr) (exprForVal m handlerVal))
                    | Expr.App(Expr.Val(vref, _, m), _, _, _, _) when valRefEq g vref g.reraise_vref ->
                        Some(RethrowAs g amap m (tyOfExpr g expr) (exprForVal m handlerVal))
                    | _ -> None)
            PreInterceptBinding = None
            PostTransform = (fun _ -> None)
            RewriteQuotations = false
            StackGuard = StackGuard("RewriteRuntimeAsyncReraise")
        }
        handler

let private IsSuspendingExceptionHandler (analyzer: RuntimeAsyncAnalyzer) expr =
    match stripExpr expr with
    | TryFinallyExpr(_, _, _, _, compensation, _) -> analyzer.ContainsSuspension compensation
    | TryWithExpr(_, _, _, _, _, filter, _, handler, _) -> analyzer.ContainsSuspension filter || analyzer.ContainsSuspension handler
    | _ -> false

let RewriteRuntimeAsyncExceptionHandlers (g: TcGlobals) amap expr =
    let analyzer = RuntimeAsyncAnalyzer(g)

    /// Runs `body` in a try that only records its outcome: the result in one mutable local, or the exception
    /// in another, which stays null when the body succeeds. The handler or compensation then runs outside the
    /// try, where it can suspend. `buildResult` receives the pending exception and the body's result.
    let rewriteCapturedException m resultTy body buildResult =
        let pendingVal, pending =
            mkMutableCompGenLocal m "__runtimeAsyncPendingException" g.exn_ty

        let caughtVal, caught = mkCompGenLocal m "__runtimeAsyncCaughtException" g.exn_ty

        let resultVal, bodyResult =
            if isUnitTy g resultTy then
                None, mkUnit g m
            else
                let resultVal, result = mkMutableCompGenLocal m "__runtimeAsyncResult" resultTy
                Some resultVal, result

        let capturedBody =
            match resultVal with
            | Some resultVal -> mkValSet m (mkLocalValRef resultVal) body
            | None -> body

        let capture =
            mkTryWith
                g
                (capturedBody,
                 caughtVal,
                 mkTrue g m,
                 caughtVal,
                 mkValSet m (mkLocalValRef pendingVal) caught,
                 m,
                 g.unit_ty,
                 DebugPointAtTry.No,
                 DebugPointAtWith.No)

        let rewritten =
            mkCompGenSequential m capture (buildResult pending bodyResult)
            |> mkCompGenLet m pendingVal (mkNull m g.exn_ty)

        match resultVal with
        | Some resultVal -> mkCompGenLet m resultVal (mkDefault (m, resultTy)) rewritten
        | None -> rewritten

    let postTransform expr =
        match expr with
        | TryFinallyExpr(_, _, resultTy, body, compensation, m) when IsSuspendingExceptionHandler analyzer expr ->
            Some(
                rewriteCapturedException m resultTy body (fun pending bodyResult ->
                    let result =
                        mkNonNullCond g m resultTy pending (RethrowAs g amap m resultTy pending) bodyResult

                    mkCompGenSequential m compensation result)
            )
        | TryWithExpr(_, _, resultTy, body, _, _, handlerVal, handler, m) when IsSuspendingExceptionHandler analyzer expr ->
            Some(
                rewriteCapturedException m resultTy body (fun pending bodyResult ->
                    let handler = RewriteReraise g amap handlerVal handler
                    let handler = mkCompGenLet m handlerVal pending handler
                    mkNonNullCond g m resultTy pending handler bodyResult)
            )
        | _ -> None

    if ExistsExpr (IsSuspendingExceptionHandler analyzer) expr then
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
