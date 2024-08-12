// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Sequence expressions checking
module internal FSharp.Compiler.CheckArrayOrListComputedExpressions

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.CheckExpressionsOps
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Features
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.CheckSequenceExpressions

let TcArrayOrListComputedExpression (cenv: TcFileState) env (overallTy: OverallTy) tpenv (isArray, comp) m =
    let g = cenv.g

    // The syntax '[ n .. m ]' and '[ n .. step .. m ]' is not really part of array or list syntax.
    // It could be in the future, e.g. '[ 1; 2..30; 400 ]'
    //
    // The elaborated form of '[ n .. m ]' is 'List.ofSeq (seq (op_Range n m))' and this shouldn't change
    match RewriteRangeExpr comp with
    | Some replacementExpr ->
        let genCollElemTy = NewInferenceType g

        let genCollTy = (if isArray then mkArrayType else mkListTy) cenv.g genCollElemTy

        UnifyTypes cenv env m overallTy.Commit genCollTy

        let exprTy = mkSeqTy cenv.g genCollElemTy

        let expr, tpenv = TcExpr cenv (MustEqual exprTy) env tpenv replacementExpr

        let expr =
            if cenv.g.compilingFSharpCore then
                expr
            else
                // We add a call to 'seq ... ' to make sure sequence expression compilation gets applied to the contents of the
                // comprehension. But don't do this in FSharp.Core.dll since 'seq' may not yet be defined.
                mkCallSeq cenv.g m genCollElemTy expr

        let expr = mkCoerceExpr (expr, exprTy, expr.Range, overallTy.Commit)

        let expr =
            if isArray then
                mkCallSeqToArray cenv.g m genCollElemTy expr
            else
                mkCallSeqToList cenv.g m genCollElemTy expr

        expr, tpenv

    | None ->

        // LanguageFeatures.ImplicitYield do not require this validation
        let implicitYieldEnabled =
            cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield

        let validateExpressionWithIfRequiresParenthesis = not implicitYieldEnabled
        let acceptDeprecatedIfThenExpression = not implicitYieldEnabled

        match comp with
        | SimpleSemicolonSequence cenv acceptDeprecatedIfThenExpression elems ->
            match comp with
            | SimpleSemicolonSequence cenv false _ -> ()
            | _ when validateExpressionWithIfRequiresParenthesis ->
                errorR (Deprecated(FSComp.SR.tcExpressionWithIfRequiresParenthesis (), m))
            | _ -> ()

            let replacementExpr =
                if isArray then
                    // This are to improve parsing/processing speed for parser tables by converting to an array blob ASAP
                    let nelems = elems.Length

                    if
                        nelems > 0
                        && List.forall
                            (function
                            | SynExpr.Const(SynConst.UInt16 _, _) -> true
                            | _ -> false)
                            elems
                    then
                        SynExpr.Const(
                            SynConst.UInt16s(
                                Array.ofList (
                                    List.map
                                        (function
                                        | SynExpr.Const(SynConst.UInt16 x, _) -> x
                                        | _ -> failwith "unreachable")
                                        elems
                                )
                            ),
                            m
                        )
                    elif
                        nelems > 0
                        && List.forall
                            (function
                            | SynExpr.Const(SynConst.Byte _, _) -> true
                            | _ -> false)
                            elems
                    then
                        SynExpr.Const(
                            SynConst.Bytes(
                                Array.ofList (
                                    List.map
                                        (function
                                        | SynExpr.Const(SynConst.Byte x, _) -> x
                                        | _ -> failwith "unreachable")
                                        elems
                                ),
                                SynByteStringKind.Regular,
                                m
                            ),
                            m
                        )
                    else
                        SynExpr.ArrayOrList(isArray, elems, m)
                else if cenv.g.langVersion.SupportsFeature(LanguageFeature.ReallyLongLists) then
                    SynExpr.ArrayOrList(isArray, elems, m)
                else
                    if elems.Length > 500 then
                        error (Error(FSComp.SR.tcListLiteralMaxSize (), m))

                    SynExpr.ArrayOrList(isArray, elems, m)

            TcExprUndelayed cenv overallTy env tpenv replacementExpr
        | _ ->

            let genCollElemTy = NewInferenceType g

            let genCollTy = (if isArray then mkArrayType else mkListTy) cenv.g genCollElemTy

            // Propagating type directed conversion, e.g. for
            //     let x : seq<int64>  = [ yield 1; if true then yield 2 ]
            TcPropagatingExprLeafThenConvert cenv overallTy genCollTy env (* canAdhoc  *) m (fun () ->

                let exprTy = mkSeqTy cenv.g genCollElemTy

                // Check the comprehension
                let expr, tpenv = TcSequenceExpression cenv env tpenv comp (MustEqual exprTy) m

                let expr = mkCoerceIfNeeded cenv.g exprTy (tyOfExpr cenv.g expr) expr

                let expr =
                    if cenv.g.compilingFSharpCore then
                        //warning(Error(FSComp.SR.fslibUsingComputedListOrArray(), expr.Range))
                        expr
                    else
                        // We add a call to 'seq ... ' to make sure sequence expression compilation gets applied to the contents of the
                        // comprehension. But don't do this in FSharp.Core.dll since 'seq' may not yet be defined.
                        mkCallSeq cenv.g m genCollElemTy expr

                let expr = mkCoerceExpr (expr, exprTy, expr.Range, overallTy.Commit)

                let expr =
                    if isArray then
                        mkCallSeqToArray cenv.g m genCollElemTy expr
                    else
                        mkCallSeqToList cenv.g m genCollElemTy expr

                expr, tpenv)
