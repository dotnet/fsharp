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
    | Sequence of (ValRef * ValRef * ValRef * ValRef list * Expr * Expr * Expr * TType * range)

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

let private prepareMethods (g: TcGlobals) amap m (stateVars: ValRef list) generateNext close =
    for value in stateVars do
        if value.Deref.IsPinning || isByrefTy g value.Type || isByrefLikeTy g m value.Type then
            error (Error(FSComp.SR.ilRuntimeAsyncSequenceNotStaticallyKnown (), value.Range))

    let resultVar, resultExpr = mkMutableCompGenLocal m "__sequenceStepResult" g.bool_ty
    let stepExit = generateCodeLabel ()

    let body =
        generateNext
        |> rewrite g (function
            | Expr.Op(TOp.Return, [], [ Expr.Const(Const.Int32 status, _, _) ], range) when status = 0 || status = 1 ->
                Some(
                    mkCompGenSequential
                        range
                        (mkValSet range (mkLocalValRef resultVar) (mkBool g range (status = 1)))
                        (Expr.Op(TOp.Goto stepExit, [], [], range))
                )
            | _ -> None)

    // Yield exits leave the protected body without running fault cleanup.
    let body = mkCompGenSequential m body (mkLabelled m stepExit (mkUnit g m))

    let labels = Dictionary<ILCodeLabel, ILCodeLabel>()

    let relabel label =
        match labels.TryGetValue label with
        | true, target -> target
        | _ ->
            let target = generateCodeLabel ()
            labels.Add(label, target)
            target

    let closeOnFailure =
        copyExpr g CloneAll close
        |> rewrite g (function
            | Expr.Op(TOp.Label label, tys, args, range) -> Some(Expr.Op(TOp.Label(relabel label), tys, args, range))
            | Expr.Op(TOp.Goto label, tys, args, range) -> Some(Expr.Op(TOp.Goto(relabel label), tys, args, range))
            | _ -> None)

    let filterVar, _ = mkLocal m "__sequenceFilter" g.exn_ty
    let errorVar, errorExpr = mkLocal m "__sequenceError" g.exn_ty

    let ediTy =
        g.system_ExceptionDispatchInfo_ty
        |> Option.defaultWith (fun () -> error (Error(FSComp.SR.ilRuntimeAsyncSequenceNotStaticallyKnown (), m)))

    let callEdi name resultTy (objArgs: Expr list) (args: Expr list) =
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

    let dispatchVar, dispatchExpr = mkMutableCompGenLocal m "__sequenceException" ediTy

    let guardedBody =
        mkTryWith
            g
            (body,
             filterVar,
             mkTrue g m,
             errorVar,
             mkValSet m (mkLocalValRef dispatchVar) (callEdi "Capture" ediTy [] [ errorExpr ]),
             m,
             g.unit_ty,
             DebugPointAtTry.No,
             DebugPointAtWith.No)

    let failure =
        mkCompGenSequential
            m
            closeOnFailure
            (mkCompGenSequential m (callEdi "Throw" g.unit_ty [ dispatchExpr ] []) (mkDefault (m, g.bool_ty)))

    let body =
        mkCompGenLet
            m
            resultVar
            (mkFalse g m)
            (mkCompGenLet
                m
                dispatchVar
                (mkDefault (m, ediTy))
                (mkCompGenSequential m guardedBody (mkNonNullCond g m g.bool_ty dispatchExpr failure resultExpr)))

    let envelope marker typeArgs body =
        for value in GetRuntimeAsyncNonPreservableUses g body do
            error (Error(FSComp.SR.ilRuntimeAsyncLocalUsedAfterSuspension value.DisplayName, value.Range))

        let body = RewriteRuntimeAsyncExceptionHandlers g body
        primMkApp (exprForValRef m marker, marker.Type) typeArgs [ body ] m

    envelope g.cgh__runtimeAsyncReturnValueTask_vref [ g.bool_ty ] body, envelope g.cgh__runtimeAsyncReturnValueTaskUnit_vref [] close

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
        let body =
            MakeApplicationAndBetaReduce g (recipe, tyOfExpr g recipe, [], [ mkUnit g m ], m)

        // Unwrap only the recipe's result spine, never nested sequence inputs.
        let rec stripRoot expr =
            match expr with
            | Seq g (inner, _) -> inner
            | Expr.Let(binding, rest, range, _) -> mkLetBind range binding (stripRoot rest)
            | Expr.Sequential(first, rest, kind, range) -> Expr.Sequential(first, stripRoot rest, kind, range)
            | Expr.DebugPoint(point, inner) -> Expr.DebugPoint(point, stripRoot inner)
            | _ -> expr

        let root = mkCallSeq g m elementTy (stripRoot body)

        match ConvertSequenceExprToObject g amap true root with
        | Some(next, pc, current, stateVars, generateNext, close, checkClose, elementTy, range) when
            not ((freeInExpr CollectLocals generateNext).FreeLocals.Contains next.Deref)
            ->
            let generateNext, close = prepareMethods g amap range stateVars generateNext close

            Some(Sequence(next, pc, current, stateVars, generateNext, close, checkClose, elementTy, range))
        | _ -> None
    | _ -> None
