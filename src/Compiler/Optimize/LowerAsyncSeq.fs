// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerAsyncSeq

open System.Collections.Generic
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.LowerSequenceExpressions
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

type Lowering =
    | Evaluate of Expr
    | Sequence of ((ValRef * ValRef * ValRef * ValRef list * Expr * Expr * Expr * TType * range) * ValRef option)

let private rewrite (g: TcGlobals) transform =
    RewriteExpr
        {
            PreIntercept =
                Some(fun _ expr ->
                    match expr with
                    | Seq g _ -> Some expr
                    | _ when TryGetRuntimeAsyncSequence g expr |> Option.isSome -> Some expr
                    | _ -> transform expr)
            PreInterceptBinding = None
            PostTransform = (fun _ -> None)
            RewriteQuotations = false
            StackGuard = StackGuard("LowerAsyncSeq")
        }

let private rewriteCancellationToken (g: TcGlobals) tokenExpr =
    RewriteExpr
        {
            PreIntercept =
                Some(fun _ expr ->
                    match expr with
                    | ValApp g g.cgh__runtimeAsyncSequenceCancellationToken_vref ([], [ _ ], _) -> Some tokenExpr
                    | Expr.App(Expr.Val(value, _, _), _, [], [ _ ], _) ->
                        if valRefEq g value g.cgh__runtimeAsyncSequenceCancellationToken_vref then
                            Some tokenExpr
                        else
                            None
                    | _ -> None)
            PreInterceptBinding = None
            PostTransform = (fun _ -> None)
            RewriteQuotations = false
            StackGuard = StackGuard("LowerAsyncSeqCancellationToken")
        }

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

/// ExceptionDispatchInfo.Capture(exn).Throw()
let private rethrow (g: TcGlobals) amap m exnExpr =
    let edi =
        callEdi g amap m "Capture" (Option.get g.system_ExceptionDispatchInfo_ty) [] [ exnExpr ]

    callEdi g amap m "Throw" g.unit_ty [ edi ] []

let private prepareMethods
    (g: TcGlobals)
    amap
    m
    (stateVars: ValRef list)
    generateNext
    (unwind: RuntimeAsyncUnwind)
    cancellationTokenValRef
    =
    for value in stateVars do
        if value.Deref.IsPinning || isByrefTy g value.Type || isByrefLikeTy g m value.Type then
            error (Error(FSComp.SR.ilRuntimeAsyncSequenceNotStaticallyKnown (), value.Range))

    let resultVar, resultExpr =
        mkMutableCompGenLocal m "__sequenceStepResult" g.int32_ty

    let retryVar, retryExpr = mkMutableCompGenLocal m "__sequenceRetry" g.bool_ty
    let stepExit = generateCodeLabel ()

    let body =
        generateNext
        |> rewrite g (function
            | Expr.Op(TOp.Return, [], [ Expr.Const(Const.Int32 status, _, _) ], range) when status = 0 || status = 1 ->
                Some(
                    mkCompGenSequential
                        range
                        (mkValSet range (mkLocalValRef resultVar) (mkInt32 g range status))
                        (Expr.Op(TOp.Goto stepExit, [], [], range))
                )
            | _ -> None)

    let faultExpr = exprForValRef m unwind.Fault

    // Cleanup runs regardless of cancellation.
    let body =
        match cancellationTokenValRef with
        | Some cancellationTokenValRef ->
            let wrap, cancellationTokenAddress, _, _ =
                mkExprAddrOfExpr g true false NeverMutates (exprForValRef m cancellationTokenValRef) None m

            let cancellationCheck =
                callNonOverloadedILMethod g amap m "ThrowIfCancellationRequested" g.system_CancellationToken_ty [ cancellationTokenAddress ]
                |> wrap

            let isUnwinding =
                mkLazyOr g m (mkNonNullTest g m faultExpr) (exprForValRef m unwind.Disposing)

            mkCompGenSequential m (mkCond DebugPointAtBinding.NoneAtInvisible m g.unit_ty isUnwinding (mkUnit g m) cancellationCheck) body
        | None -> body

    // Yield exits leave the protected body.
    let body = mkCompGenSequential m body (mkLabelled m stepExit (mkUnit g m))

    // A fault runs the step again, and the jump table then goes to the innermost pending finally or handler.
    let filterVar, _ = mkLocal m "__sequenceFilter" g.exn_ty
    let errorVar, errorExpr = mkLocal m "__sequenceError" g.exn_ty

    let guardedBody =
        mkTryWith
            g
            (body,
             filterVar,
             mkTrue g m,
             errorVar,
             mkCompGenSequential m (mkValSet m unwind.Fault errorExpr) (mkValSet m (mkLocalValRef retryVar) (mkTrue g m)),
             m,
             g.unit_ty,
             DebugPointAtTry.No,
             DebugPointAtWith.No)

    let loop =
        mkWhile
            g
            (DebugPointAtWhile.No,
             NoSpecialWhileLoopMarker,
             retryExpr,
             mkCompGenSequential m (mkValSet m (mkLocalValRef retryVar) (mkFalse g m)) guardedBody,
             m)

    // An exception that no handler caught is rethrown once all cleanup has run.
    let body =
        mkCompGenSequential
            m
            loop
            (mkNonNullCond
                g
                m
                g.bool_ty
                faultExpr
                (mkCompGenSequential m (rethrow g amap m faultExpr) (mkFalse g m))
                (mkILAsmCeq g m resultExpr (mkInt32 g m 1)))

    let body =
        body
        |> mkCompGenLet m unwind.Fault.Deref (mkNull m g.exn_ty)
        |> mkCompGenLet m retryVar (mkTrue g m)
        |> mkCompGenLet m resultVar (mkInt32 g m 0)

    for value in GetRuntimeAsyncNonPreservableUses g body do
        error (Error(FSComp.SR.ilRuntimeAsyncLocalUsedAfterSuspension value.DisplayName, value.Range))

    let body = RewriteRuntimeAsyncExceptionHandlers g body
    let marker = g.cgh__runtimeAsyncReturnValueTask_vref
    primMkApp (exprForValRef m marker, marker.Type) [ g.bool_ty ] [ body ] m

let TryConvert g amap (expr: Expr) =
    let m = expr.Range
    let recipe, elementTy = TryGetRuntimeAsyncSequence g expr |> Option.get

    let rebuild recipe =
        let value = g.cgh__runtimeAsyncSequence_vref
        primMkApp (exprForValRef m value, value.Type) [ elementTy ] [ recipe ] m

    match stripDebugPoints recipe with
    | Expr.Let(binding, body, range, _) -> Some(Evaluate(mkLetBind range binding (rebuild body)))
    | Expr.Sequential(first, rest, kind, range) -> Some(Evaluate(Expr.Sequential(first, rebuild rest, kind, range)))
    | Expr.Lambda(_, _, _, [ parameter ], _, _, _) as recipe when isUnitTy g parameter.Type ->
        let cancellationTokenVar, cancellationTokenExpr =
            mkMutableCompGenLocal m "__cancellationToken" g.system_CancellationToken_ty

        // Unwrap only the recipe's result spine, never nested sequence inputs.
        let rec stripRoot expr =
            match expr with
            | Seq g (inner, _) -> inner
            | Expr.Let(binding, rest, range, _) -> mkLetBind range binding (stripRoot rest)
            | Expr.Sequential(first, rest, kind, range) -> Expr.Sequential(first, stripRoot rest, kind, range)
            | Expr.DebugPoint(point, inner) -> Expr.DebugPoint(point, stripRoot inner)
            | _ -> expr

        let body =
            MakeApplicationAndBetaReduce g (recipe, tyOfExpr g recipe, [], [ mkUnit g m ], m)
            |> rewriteCancellationToken g cancellationTokenExpr

        let root = mkCallSeq g m elementTy (stripRoot body)

        let faultVar, _ = mkMutableCompGenLocal m "__sequenceFault" g.exn_ty
        let disposingVar, _ = mkMutableCompGenLocal m "__disposing" g.bool_ty

        let unwind =
            {
                Fault = mkLocalValRef faultVar
                Disposing = mkLocalValRef disposingVar
                Rethrow = rethrow g amap m
            }

        match ConvertSequenceExprToObject g amap (Some unwind) root with
        | Some(next, pc, current, stateVars, generateNext, close, checkClose, elementTy, range) when
            not ((freeInExpr CollectLocals generateNext).FreeLocals.Contains next.Deref)
            ->
            let stateVars = mkLocalValRef cancellationTokenVar :: unwind.Disposing :: stateVars

            let generateNext =
                prepareMethods g amap range stateVars generateNext unwind (Some(mkLocalValRef cancellationTokenVar))

            Some(
                Sequence(
                    (next, pc, current, stateVars, generateNext, close, checkClose, elementTy, range),
                    Some(mkLocalValRef cancellationTokenVar)
                )
            )
        | _ -> None
    | _ -> None
