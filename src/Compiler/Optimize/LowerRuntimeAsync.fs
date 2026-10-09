// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerRuntimeAsync

open System.Collections.Concurrent

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.RuntimeAsync
open FSharp.Compiler.RuntimeAsyncAnalysis
open FSharp.Compiler.RuntimeAsyncExceptionRewrite
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

let private isQuotationUsing (v: Val) expr =
    match expr with
    | Expr.Quote(body, _, _, _, _) ->
        ExistsExpr
            (function
            | Expr.Val(vref, _, _) -> valEq v vref.Deref
            | _ -> false)
            body
    | _ -> false

let private tryCallbackInvocation (g: TcGlobals) (callback: Val) isDelegate expression =
    match expression with
    | Expr.App(Expr.Val(vref, _, _), _, [], [ arg ], m) when not isDelegate && valEq callback vref.Deref -> ValueSome(struct (arg, m))
    | DelegateInvokeExpr g (_, _, _, Expr.Val(vref, _, _), arg, m) when isDelegate && valEq callback vref.Deref ->
        ValueSome(struct (arg, m))
    | _ -> ValueNone

let private canRewriteCallbackUses g callback isDelegate continuation =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun recurse noIntercept valid expression ->
                    if not valid then
                        false
                    else
                        match tryCallbackInvocation g callback isDelegate expression with
                        | ValueSome(struct (arg, _)) -> recurse true arg
                        | ValueNone ->
                            match expression with
                            | Expr.Val(vref, _, _) when valEq callback vref.Deref -> false
                            | Expr.Quote(_, dataCell, _, _, _) ->
                                if isQuotationUsing callback expression then
                                    false
                                else
                                    match dataCell.Value with
                                    | Some((_, _, args, _), (_, _, legacyArgs, _)) ->
                                        List.fold recurse (List.fold recurse true args) legacyArgs
                                    | None -> true
                            | _ -> noIntercept true expression
        }

    FoldExpr folder true continuation

let rec private tryDelegateSignature g expr =
    match expr with
    | NewDelegateExpr g (_, [ parameter ], body, _, _) -> Some(parameter.Type, tyOfExpr g body)
    | Expr.DebugPoint(_, rest)
    | Expr.Sequential(_, rest, NormalSeq, _)
    | Expr.Let(_, rest, _, _) -> tryDelegateSignature g rest
    | Expr.Match(_, _, _, targets, _, _) when targets.Length > 0 ->
        let (TTarget(_, body, _)) = targets[0]
        tryDelegateSignature g body
    | _ -> None

let private awaitFragment (g: TcGlobals) amap resultTy invocation m =
    let awaiterTy = mkWoNullAppTy g.runtimeAsyncFragmentAwaiter_tcref [ resultTy ]
    let infoReader = InfoReader(g, amap)

    let constructor =
        match GetIntrinsicConstructorInfosOfType infoReader m awaiterTy with
        | [ constructor ] -> constructor
        | _ -> error (InternalError("runtime-async fragment awaiter constructor not found", m))

    let awaiter, awaiterExpr = mkCompGenLocal m "runtimeAsyncFragmentAwaiter" awaiterTy
    let createAwaiter = MakeMethInfoCall amap m constructor [] [ invocation ] None

    let awaitRef =
        mkILMethRef (
            g.FindSysILTypeRef "System.Runtime.CompilerServices.AsyncHelpers",
            ILCallingConv.Static,
            "AwaitAwaiter",
            1,
            [ mkILTyvarTy 0us ],
            ILType.Void
        )

    let suspend =
        Expr.Op(TOp.ILCall(false, false, false, false, NormalValUse, false, false, awaitRef, [], [ awaiterTy ], []), [], [ awaiterExpr ], m)

    let callAwaiterMember name =
        let methodInfo =
            match TryFindIntrinsicMethInfo infoReader m AccessorDomain.AccessibleFromEverywhere name awaiterTy with
            | [ methodInfo ] -> methodInfo
            | _ -> error (InternalError($"runtime-async fragment awaiter {name} not found", m))

        let wrap, address, _, _ =
            mkExprAddrOfExpr g true false NeverMutates awaiterExpr None m

        MakeMethInfoCall amap m methodInfo [] [ address ] None |> wrap

    let suspend =
        mkCond DebugPointAtBinding.NoneAtInvisible m g.unit_ty (callAwaiterMember "get_IsCompleted") (mkUnit g m) suspend

    let result = callAwaiterMember "GetResult"
    mkCompGenLet m awaiter createAwaiter (mkCompGenSequential m suspend result)

let private outlineCallback (g: TcGlobals) amap optimizeExpr (analyzer: RuntimeAsyncAnalyzer) runtimeAsyncContext expr =
    let stackGuard = StackGuard("OutlineRuntimeAsyncCallback")
    let freeVarOptions = CollectLocalsWithStackGuard()

    let rec outlineBranches resultTy expr =
        stackGuard.Guard(fun () ->
            match expr with
            | Expr.Lambda(_, None, None, [ parameter ], body, m, _)
            | NewDelegateExpr g (_, [ parameter ], body, m, _) ->
                let helper = g.cgh__runtimeAsyncOutline_vref
                let callback = mkLambda m parameter (body, resultTy)

                primMkApp (exprForValRef m helper, helper.Type) [ parameter.Type; resultTy ] [ callback ] m
                |> optimizeExpr true
                |> Some
            | Expr.DebugPoint(point, body) ->
                outlineBranches resultTy body
                |> Option.map (fun body -> Expr.DebugPoint(point, body))
            | Expr.Sequential(first, rest, NormalSeq, m) ->
                outlineBranches resultTy rest
                |> Option.map (fun rest -> Expr.Sequential(first, rest, NormalSeq, m))
            | Expr.Let(binding, rest, m, _) -> outlineBranches resultTy rest |> Option.map (mkLetBind m binding)
            | Expr.Match(point, matchRange, tree, targets, m, _) ->
                TryMapRuntimeAsyncMatchTargets g (point, matchRange, tree, targets, m) (fun _ -> outlineBranches resultTy)
            | _ -> None)

    let rec outline expr =
        stackGuard.Guard(fun () ->
            match expr with
            | Expr.Let(TBind(callback, construction, point), continuation, m, _) when
                callback.InlineIfLambda && analyzer.ContainsSuspension construction
                ->
                let keep construction =
                    mkLetBind m (TBind(callback, construction, point)) (outline continuation)

                let shape =
                    match tryDestFunTy g callback.Type with
                    | ValueSome(argTy, resultTy) -> Some(argTy, resultTy, false)
                    | ValueNone when isFSharpDelegateTy g callback.Type ->
                        tryDelegateSignature g construction
                        |> Option.map (fun (argTy, resultTy) -> argTy, resultTy, true)
                    | _ -> None

                match shape with
                | Some(argTy, resultTy, isDelegate) when
                    (runtimeAsyncContext || (TryGetRuntimeAsyncReturn g continuation).IsSome)
                    && not (isByrefLikeTy g m argTy || isByrefLikeTy g m resultTy)
                    ->
                    let canCapture =
                        (freeInExpr freeVarOptions construction).FreeLocals
                        |> Zset.forall (fun v -> not v.IsPinning && not (isByrefTy g v.Type || isByrefLikeTy g m v.Type))

                    if
                        not canCapture
                        || not (canRewriteCallbackUses g callback isDelegate continuation)
                    then
                        keep construction
                    else
                        let construction = outline construction

                        match outlineBranches resultTy construction with
                        | None -> keep construction
                        | Some construction ->
                            let callbackTy = tyOfExpr g construction
                            let outlined, outlinedExpr = mkCompGenLocal m "runtimeAsyncFragment" callbackTy

                            let continuation =
                                RewriteExpr
                                    {
                                        PreIntercept =
                                            Some(fun rewrite expression ->
                                                match tryCallbackInvocation g callback isDelegate expression with
                                                | ValueSome(struct (arg, callRange)) ->
                                                    let invocation =
                                                        mkApps g ((outlinedExpr, callbackTy), [], [ rewrite arg ], callRange)

                                                    Some(awaitFragment g amap resultTy invocation callRange)
                                                | ValueNone -> None)
                                        PreInterceptBinding = None
                                        PostTransform = (fun _ -> None)
                                        RewriteQuotations = false
                                        StackGuard = stackGuard
                                    }
                                    continuation

                            mkLet point m outlined construction (outline continuation)
                | _ -> keep construction
            | Expr.Let(binding, continuation, m, _) -> mkLetBind m binding (outline continuation)
            | Expr.DebugPoint(point, body) -> Expr.DebugPoint(point, outline body)
            | Expr.Sequential(first, rest, NormalSeq, m) -> Expr.Sequential(first, outline rest, NormalSeq, m)
            | _ -> expr)

    outline expr

let private isRuntimeAsyncEntry g expr =
    (TryGetRuntimeAsyncReturn g expr).IsSome
    || (TryGetRuntimeAsyncSequence g expr).IsSome

let private containsRuntimeAsyncEntry g implFile =
    let folder =
        { ExprFolder0 with
            exprIntercept =
                fun _ noInterceptF acc expr ->
                    if acc || isRuntimeAsyncEntry g expr then
                        true
                    else
                        noInterceptF acc expr
        }

    FoldImplFile folder false implFile

/// `inContext` follows runtime-async bodies and sequence recipes, but not ordinary object-expression methods.
let private outlineCallbacks g amap optimizeExpr implFile =
    let analyzer = RuntimeAsyncAnalyzer g
    let stackGuard = StackGuard("OutlineRuntimeAsyncCallbacks")

    let rec leadsToRuntimeAsyncReturn expr =
        match expr with
        | Expr.Let(_, rest, _, _)
        | Expr.DebugPoint(_, rest)
        | Expr.Sequential(_, rest, NormalSeq, _) -> leadsToRuntimeAsyncReturn rest
        | _ -> (TryGetRuntimeAsyncReturn g expr).IsSome

    let rec rewriter inContext =
        {
            PreIntercept = Some(intercept inContext)
            PreInterceptBinding = None
            PostTransform = postTransform inContext
            RewriteQuotations = false
            StackGuard = stackGuard
        }

    and rewrite inContext expr = RewriteExpr (rewriter inContext) expr

    and intercept inContext _ expr =
        match expr with
        | Expr.App(f, fty, tyargs, [ body ], m) when (TryGetRuntimeAsyncReturn g expr).IsSome ->
            Some(Expr.App(f, fty, tyargs, [ rewrite true body ], m))
        | Expr.App(f, fty, tyargs, [ recipe ], m) when (TryGetRuntimeAsyncSequence g expr).IsSome ->
            Some(Expr.App(f, fty, tyargs, [ rewrite true recipe ], m))
        | Expr.Let(TBind(callback, construction, point), continuation, m, _) when
            not inContext
            && callback.InlineIfLambda
            && leadsToRuntimeAsyncReturn continuation
            ->
            Some(mkLetBind m (TBind(callback, rewrite true construction, point)) (rewrite false continuation))
        | DelegateInvokeExpr g (invokeRef, invokeTy, tyargs, receiver, arg, m) when inContext && analyzer.ContainsSuspension receiver ->
            let callback, callbackExpr =
                mkCompGenLocal m "runtimeAsyncDelegate" (tyOfExpr g receiver)

            callback.SetInlineIfLambda()
            let invoke = Expr.App(invokeRef, invokeTy, tyargs, [ callbackExpr; arg ], m)
            Some(rewrite inContext (mkCompGenLet m callback receiver invoke))
        | NewDelegateExpr g _ -> None
        | Expr.Obj _ when inContext -> Some(rewrite false expr)
        | _ -> None

    and postTransform inContext expr =
        match expr with
        | Expr.Let(TBind(callback, construction, _), _, _, _) when callback.InlineIfLambda && analyzer.ContainsSuspension construction ->
            Some(outlineCallback g amap optimizeExpr analyzer inContext expr)
        | _ -> None

    RewriteImplFile (rewriter false) implFile

/// Prepares each runtime-async body once its final shape is known, innermost first.
let private prepareBodies g (reportedRanges: ConcurrentDictionary<range, unit>) implFile =
    let prepare body =
        for v in GetRuntimeAsyncNonPreservableUses g body do
            if reportedRanges.TryAdd(v.Range, ()) then
                errorR (Error(FSComp.SR.ilRuntimeAsyncLocalUsedAfterSuspension (RichText.mkText v.DisplayName), v.Range))

        RewriteRuntimeAsyncExceptionHandlers g body

    RewriteImplFile
        {
            PreIntercept = None
            PreInterceptBinding = None
            PostTransform =
                fun expr ->
                    match expr with
                    | Expr.App(f, fty, tyargs, [ body ], m) when (TryGetRuntimeAsyncReturn g expr).IsSome ->
                        Some(Expr.App(f, fty, tyargs, [ prepare body ], m))
                    | _ -> None
            RewriteQuotations = false
            StackGuard = StackGuard("PrepareRuntimeAsyncBodies")
        }
        implFile

let TransformImplFile (g: TcGlobals) amap optimizeExpr reportedRanges (implFile: CheckedImplFile) =
    if containsRuntimeAsyncEntry g implFile then
        // Bodies inlined from an assembly compiled with the feature are still prepared.
        let implFile =
            if g.langVersion.SupportsFeature LanguageFeature.RuntimeAsync then
                outlineCallbacks g amap optimizeExpr implFile
            else
                implFile

        prepareBodies g reportedRanges implFile
    else
        implFile
