// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Sequence expressions checking
module internal FSharp.Compiler.CheckArrayOrListComputedExpressions

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.CheckExpressionsOps
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckSequenceExpressions
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Features
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax

let private tcMixedSequencesWithRanges (cenv: TcFileState) env overallTy tpenv isArray elems m =
    let g = cenv.g
    let transformedBody = insertImplicitYieldsAndYieldBangs elems m

    let genCollElemTy = NewInferenceType g
    let genCollTy = (if isArray then mkArrayType else mkListTy) g genCollElemTy

    TcPropagatingExprLeafThenConvert cenv overallTy genCollTy env m (fun () ->
        let exprTy = mkSeqTy g genCollElemTy

        let expr, tpenv' =
            TcSequenceExpression cenv env tpenv transformedBody (MustEqual exprTy) m

        let expr = mkCoerceIfNeeded g exprTy (tyOfExpr g expr) expr

        let expr =
            if g.compilingFSharpCore then
                expr
            else
                mkCallSeq g m genCollElemTy expr

        let expr = mkCoerceExpr (expr, exprTy, expr.Range, overallTy.Commit)

        let expr =
            if isArray then
                mkCallSeqToArray g m genCollElemTy expr
            else
                mkCallSeqToList g m genCollElemTy expr

        expr, tpenv')

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

            if g.langVersion.SupportsFeature LanguageFeature.AllowMixedRangesAndValuesInSeqExpressions then
                tcMixedSequencesWithRanges cenv env overallTy tpenv isArray elems m
            else
                let reportRangeExpressionsNotSupported expr =
                    let rec loop exprs =
                        match exprs with
                        | [] -> ()
                        | SynExpr.IndexRange(_, _, _, _, _, m) :: exprs ->
                            checkLanguageFeatureAndRecover g.langVersion LanguageFeature.AllowMixedRangesAndValuesInSeqExpressions m
                            loop exprs
                        | SynExpr.Sequential(_, true, e1, e2, _, _) :: exprs -> loop (e1 :: e2 :: exprs)
                        | _ :: exprs -> loop exprs

                    loop [ expr ]

                reportRangeExpressionsNotSupported comp

                let replacementExpr =
                    // These are to improve parsing/processing speed for parser tables by converting to an array blob ASAP
                    if isArray then
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
            let containsRangeMixedWithYields =
                let rec hasRangeAndYield expr hasRange hasYield cont =
                    match expr with
                    | SynExpr.IndexRange _ -> cont true hasYield
                    | SynExpr.YieldOrReturnFrom _ -> cont hasRange true
                    | SynExpr.Sequential(_, _, e1, e2, _, _) ->
                        hasRangeAndYield e1 hasRange hasYield (fun r1 y1 -> hasRangeAndYield e2 r1 y1 cont)
                    | _ -> cont hasRange hasYield

                hasRangeAndYield comp false false (fun r y -> r && y)

            // Transform mixed expressions with explicit yields to ensure all elements are properly yielded
            let comp =
                if
                    containsRangeMixedWithYields
                    && g.langVersion.SupportsFeature LanguageFeature.AllowMixedRangesAndValuesInSeqExpressions
                then
                    match comp with
                    | SynExpr.Sequential _ ->
                        // Extract the elements from the sequential expression
                        let rec getElems expr acc =
                            match expr with
                            | SynExpr.Sequential(_, true, e1, e2, _, _) -> getElems e2 (e1 :: acc)
                            | e -> List.rev (e :: acc)

                        let elems = getElems comp []
                        insertImplicitYieldsAndYieldBangs elems m
                    | _ -> comp
                else
                    // Report language feature error for each range expression in a sequence
                    let reportRangeExpressionsNotSupported expr =
                        let rec loop exprs =
                            match exprs with
                            | [] -> ()
                            | SynExpr.IndexRange(_, _, _, _, _, m) :: exprs ->
                                checkLanguageFeatureAndRecover g.langVersion LanguageFeature.AllowMixedRangesAndValuesInSeqExpressions m
                                loop exprs
                            | SynExpr.Sequential(_, true, e1, e2, _, _) :: exprs -> loop (e1 :: e2 :: exprs)
                            | _ :: exprs -> loop exprs

                        loop [ expr ]

                    reportRangeExpressionsNotSupported comp

                    comp

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
