// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Sequence expressions checking
module internal FSharp.Compiler.CheckSequenceExpressions

open Internal.Utilities.Library
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckExpressionsOps
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.Features
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.SyntaxTreeOps

/// This case is used for computation expressions which are sequence expressions. Technically the code path is different because it
/// typechecks rather than doing a shallow syntactic translation, and generates calls into the Seq.* library
/// and helpers rather than to the builder methods (there is actually no builder for 'seq' in the library).
/// These are later detected by state machine compilation.
///
/// Also "ienumerable extraction" is performed on arguments to "for".
let TcSequenceExpression (cenv: TcFileState) env tpenv comp (overallTy: OverallTy) m =

    let g = cenv.g
    let genEnumElemTy = NewInferenceType g
    UnifyTypes cenv env m overallTy.Commit (mkSeqTy cenv.g genEnumElemTy)

    // Allow subsumption at 'yield' if the element type is nominal prior to the analysis of the body of the sequence expression
    let flex = not (isTyparTy cenv.g genEnumElemTy)

    // If there are no 'yield' in the computation expression then allow the type-directed rule
    // interpreting non-unit-typed expressions in statement positions as 'yield'.  'yield!' may be
    // present in the computation expression.
    let enableImplicitYield =
        cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield
        && (YieldFree cenv comp)

    let mkSeqDelayedExpr m (coreExpr: Expr) =
        let overallTy = tyOfExpr cenv.g coreExpr
        mkSeqDelay cenv env m overallTy coreExpr

    let rec tryTcSequenceExprBody env genOuterTy tpenv comp =
        match comp with
        | SynExpr.ForEach(spFor, spIn, SeqExprOnly _seqExprOnly, _isFromSource, pat, pseudoEnumExpr, innerComp, _m) ->
            let pseudoEnumExpr =
                match RewriteRangeExpr pseudoEnumExpr with
                | Some e -> e
                | None -> pseudoEnumExpr
            // This expression is not checked with the knowledge it is an IEnumerable, since we permit other enumerable types with GetEnumerator/MoveNext methods, as does C#
            let pseudoEnumExpr, arbitraryTy, tpenv =
                TcExprOfUnknownType cenv env tpenv pseudoEnumExpr

            let enumExpr, enumElemTy =
                ConvertArbitraryExprToEnumerable cenv arbitraryTy env pseudoEnumExpr

            let patR, _, vspecs, envinner, tpenv =
                TcMatchPattern cenv enumElemTy env tpenv pat None TcTrueMatchClause.No

            let innerExpr, tpenv =
                let envinner = { envinner with eIsControlFlow = true }
                tcSequenceExprBody envinner genOuterTy tpenv innerComp

            let enumExprRange = enumExpr.Range

            // We attach the debug point to the lambda expression so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mFor =
                match spFor with
                | DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For)
                | _ -> enumExprRange

            // We attach the debug point to the lambda expression so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mIn =
                match spIn with
                | DebugPointAtInOrTo.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.InOrTo)
                | _ -> pat.Range

            match patR, vspecs, innerExpr with
            // Legacy peephole optimization:
            //     "seq { .. for x in e1 -> e2 .. }" == "e1 |> Seq.map (fun x -> e2)"
            //     "seq { .. for x in e1 do yield e2 .. }" == "e1 |> Seq.map (fun x -> e2)"
            //
            // This transformation is visible in quotations and thus needs to remain.
            | (TPat_as(TPat_wild _, PatternValBinding(v, _), _),
               [ _ ],
               DebugPoints(Expr.App(Expr.Val(vref, _, _), _, [ genEnumElemTy ], [ yieldExpr ], _mYield), recreate)) when
                valRefEq cenv.g vref cenv.g.seq_singleton_vref
                ->

                // The debug point mFor is attached to the 'map'
                // The debug point mIn is attached to the lambda
                // Note: the 'yield' part of the debug point for 'yield expr' is currently lost in debug points.
                let lam = mkLambda mIn v (recreate yieldExpr, genEnumElemTy)

                let enumExpr =
                    mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g enumElemTy) (tyOfExpr cenv.g enumExpr) enumExpr

                Some(mkCallSeqMap cenv.g mFor enumElemTy genEnumElemTy lam enumExpr, tpenv)

            | _ ->
                // The debug point mFor is attached to the 'collect'
                // The debug point mIn is attached to the lambda
                let matchv, matchExpr =
                    compileSeqExprMatchClauses cenv env enumExprRange (patR, vspecs) innerExpr None enumElemTy genOuterTy

                let lam = mkLambda mIn matchv (matchExpr, tyOfExpr cenv.g matchExpr)
                Some(mkSeqCollect cenv env mFor enumElemTy genOuterTy lam enumExpr, tpenv)

        | SynExpr.For(
            forDebugPoint = spFor
            toDebugPoint = spTo
            ident = id
            identBody = start
            direction = dir
            toBody = finish
            doBody = innerComp
            range = m) ->
            Some(tcSequenceExprBody env genOuterTy tpenv (elimFastIntegerForLoop (spFor, spTo, id, start, dir, finish, innerComp, m)))

        | SynExpr.While(spWhile, guardExpr, innerComp, _m) ->
            let guardExpr, tpenv =
                let env = { env with eIsControlFlow = false }
                TcExpr cenv (MustEqual cenv.g.bool_ty) env tpenv guardExpr

            let innerExpr, tpenv =
                let env = { env with eIsControlFlow = true }
                tcSequenceExprBody env genOuterTy tpenv innerComp

            let guardExprMark = guardExpr.Range
            let guardLambdaExpr = mkUnitDelayLambda cenv.g guardExprMark guardExpr

            // We attach the debug point to the lambda expression so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mWhile =
                match spWhile with
                | DebugPointAtWhile.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.While)
                | _ -> guardExprMark

            let innerDelayedExpr = mkSeqDelayedExpr mWhile innerExpr
            Some(mkSeqFromFunctions cenv env guardExprMark genOuterTy guardLambdaExpr innerDelayedExpr, tpenv)

        | SynExpr.TryFinally(innerComp, unwindExpr, mTryToLast, spTry, spFinally, trivia) ->
            let env = { env with eIsControlFlow = true }
            let innerExpr, tpenv = tcSequenceExprBody env genOuterTy tpenv innerComp
            let unwindExpr, tpenv = TcExpr cenv (MustEqual cenv.g.unit_ty) env tpenv unwindExpr

            // We attach the debug points to the lambda expressions so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mTry =
                match spTry with
                | DebugPointAtTry.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Try)
                | _ -> trivia.TryKeyword

            let mFinally =
                match spFinally with
                | DebugPointAtFinally.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Finally)
                | _ -> trivia.FinallyKeyword

            let innerExpr = mkSeqDelayedExpr mTry innerExpr
            let unwindExpr = mkUnitDelayLambda cenv.g mFinally unwindExpr

            Some(mkSeqFinally cenv env mTryToLast genOuterTy innerExpr unwindExpr, tpenv)

        | SynExpr.Paren(range = m) when not (cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield) ->
            error (Error(FSComp.SR.tcConstructIsAmbiguousInSequenceExpression (), m))

        | SynExpr.ImplicitZero m -> Some(mkSeqEmpty cenv env m genOuterTy, tpenv)

        | SynExpr.DoBang(trivia = { DoBangKeyword = m }) -> error (Error(FSComp.SR.tcDoBangIllegalInSequenceExpression (), m))

        | SynExpr.Sequential(sp, true, innerComp1, innerComp2, m, _) ->
            let env1 =
                { env with
                    eIsControlFlow =
                        (match sp with
                         | DebugPointAtSequential.SuppressNeither
                         | DebugPointAtSequential.SuppressExpr -> true
                         | _ -> false)
                }

            let res, tpenv =
                tcSequenceExprBodyAsSequenceOrStatement env1 genOuterTy tpenv innerComp1

            let env2 =
                { env with
                    eIsControlFlow =
                        (match sp with
                         | DebugPointAtSequential.SuppressNeither
                         | DebugPointAtSequential.SuppressStmt -> true
                         | _ -> false)
                }

            // "expr; cexpr" is treated as sequential execution
            // "cexpr; cexpr" is treated as append
            match res with
            | Choice1Of2 innerExpr1 ->
                let innerExpr2, tpenv = tcSequenceExprBody env2 genOuterTy tpenv innerComp2
                let innerExpr2 = mkSeqDelayedExpr innerExpr2.Range innerExpr2
                Some(mkSeqAppend cenv env innerComp1.Range genOuterTy innerExpr1 innerExpr2, tpenv)
            | Choice2Of2 stmt1 ->
                let innerExpr2, tpenv = tcSequenceExprBody env2 genOuterTy tpenv innerComp2
                Some(Expr.Sequential(stmt1, innerExpr2, NormalSeq, m), tpenv)

        | SynExpr.IfThenElse(guardExpr, thenComp, elseCompOpt, spIfToThen, _isRecovery, mIfToEndOfElseBranch, trivia) ->
            let guardExpr', tpenv = TcExpr cenv (MustEqual cenv.g.bool_ty) env tpenv guardExpr
            let env = { env with eIsControlFlow = true }
            let thenExpr, tpenv = tcSequenceExprBody env genOuterTy tpenv thenComp

            let elseComp =
                (match elseCompOpt with
                 | Some c -> c
                 | None -> SynExpr.ImplicitZero trivia.IfToThenRange)

            let elseExpr, tpenv = tcSequenceExprBody env genOuterTy tpenv elseComp
            Some(mkCond spIfToThen mIfToEndOfElseBranch genOuterTy guardExpr' thenExpr elseExpr, tpenv)

        // 'let x = expr in expr'
        | SynExpr.LetOrUse(isUse = false) ->
            TcLinearExprs
                (fun overallTy envinner tpenv e -> tcSequenceExprBody envinner overallTy.Commit tpenv e)
                cenv
                env
                overallTy
                tpenv
                true
                comp
                id
            |> Some

        // 'use x = expr in expr'
        | SynExpr.LetOrUse(
            isUse = true
            bindings = [ SynBinding(kind = SynBindingKind.Normal; headPat = pat; expr = rhsExpr) ]
            body = innerComp
            range = wholeExprMark
            trivia = { LetOrUseKeyword = mBind }) ->

            let bindPatTy = NewInferenceType g
            let inputExprTy = NewInferenceType g

            let pat', _, vspecs, envinner, tpenv =
                TcMatchPattern cenv bindPatTy env tpenv pat None TcTrueMatchClause.No

            UnifyTypes cenv env m inputExprTy bindPatTy

            let inputExpr, tpenv =
                let env = { env with eIsControlFlow = true }
                TcExpr cenv (MustEqual inputExprTy) env tpenv rhsExpr

            let innerExpr, tpenv =
                let envinner = { envinner with eIsControlFlow = true }
                tcSequenceExprBody envinner genOuterTy tpenv innerComp

            let inputExprMark = inputExpr.Range

            let matchv, matchExpr =
                compileSeqExprMatchClauses cenv envinner inputExprMark (pat', vspecs) innerExpr (Some inputExpr) bindPatTy genOuterTy

            let consumeExpr = mkLambda mBind matchv (matchExpr, genOuterTy)

            // The 'mBind' is attached to the lambda
            Some(mkSeqUsing cenv env wholeExprMark bindPatTy genOuterTy inputExpr consumeExpr, tpenv)

        | SynExpr.LetOrUseBang(range = m) -> error (Error(FSComp.SR.tcUseForInSequenceExpression (), m))

        | SynExpr.Match(spMatch, expr, clauses, _m, _trivia) ->
            let inputExpr, inputTy, tpenv = TcExprOfUnknownType cenv env tpenv expr

            let tclauses, tpenv =
                (tpenv, clauses)
                ||> List.mapFold (fun tpenv (SynMatchClause(pat, cond, innerComp, _, sp, trivia) as clause) ->
                    let isTrueMatchClause =
                        if clause.IsTrueMatchClause then
                            TcTrueMatchClause.Yes
                        else
                            TcTrueMatchClause.No

                    let patR, condR, vspecs, envinner, tpenv =
                        TcMatchPattern cenv inputTy env tpenv pat cond isTrueMatchClause

                    let envinner =
                        match sp with
                        | DebugPointAtTarget.Yes -> { envinner with eIsControlFlow = true }
                        | DebugPointAtTarget.No -> envinner

                    let innerExpr, tpenv = tcSequenceExprBody envinner genOuterTy tpenv innerComp
                    MatchClause(patR, condR, TTarget(vspecs, innerExpr, None), patR.Range), tpenv)

            let inputExprTy = tyOfExpr cenv.g inputExpr
            let inputExprMark = inputExpr.Range

            let matchv, matchExpr =
                CompilePatternForMatchClauses
                    cenv
                    env
                    inputExprMark
                    inputExprMark
                    true
                    ThrowIncompleteMatchException
                    (Some inputExpr)
                    inputExprTy
                    genOuterTy
                    tclauses

            Some(mkLet spMatch inputExprMark matchv inputExpr matchExpr, tpenv)

        | SynExpr.TryWith(innerTry, withList, mTryToWith, _spTry, _spWith, trivia) ->
            if not (g.langVersion.SupportsFeature(LanguageFeature.TryWithInSeqExpression)) then
                error (Error(FSComp.SR.tcTryIllegalInSequenceExpression (), mTryToWith))

            let env = { env with eIsControlFlow = true }

            let tryExpr, tpenv =
                let inner, tpenv = tcSequenceExprBody env genOuterTy tpenv innerTry
                mkSeqDelayedExpr mTryToWith inner, tpenv

            // Compile the pattern twice, once as a filter with all succeeding targets returning "1", and once as a proper catch block.
            let clauses, tpenv =
                (tpenv, withList)
                ||> List.mapFold (fun tpenv (SynMatchClause(pat, cond, innerComp, m, sp, trivia) as clause) ->
                    let isTrueMatchClause =
                        if clause.IsTrueMatchClause then
                            TcTrueMatchClause.Yes
                        else
                            TcTrueMatchClause.No

                    let patR, condR, vspecs, envinner, tpenv =
                        TcMatchPattern cenv g.exn_ty env tpenv pat cond isTrueMatchClause

                    let envinner =
                        match sp with
                        | DebugPointAtTarget.Yes -> { envinner with eIsControlFlow = true }
                        | DebugPointAtTarget.No -> envinner

                    let matchBody, tpenv = tcSequenceExprBody envinner genOuterTy tpenv innerComp

                    let handlerClause =
                        MatchClause(patR, condR, TTarget(vspecs, matchBody, None), patR.Range)

                    let filterClause =
                        MatchClause(patR, condR, TTarget(vspecs, Expr.Const(Const.Int32 1, m, g.int_ty), None), patR.Range)

                    (handlerClause, filterClause), tpenv)

            let handlers, filterClauses = List.unzip clauses
            let withRange = trivia.WithToEndRange

            let v1, filterExpr =
                CompilePatternForMatchClauses cenv env withRange withRange true FailFilter None g.exn_ty g.int_ty filterClauses

            let v2, handlerExpr =
                CompilePatternForMatchClauses cenv env withRange withRange true FailFilter None g.exn_ty genOuterTy handlers

            let filterLambda = mkLambda filterExpr.Range v1 (filterExpr, genOuterTy)
            let handlerLambda = mkLambda handlerExpr.Range v2 (handlerExpr, genOuterTy)

            let combinatorExpr =
                mkSeqTryWith cenv env mTryToWith genOuterTy tryExpr filterLambda handlerLambda

            Some(combinatorExpr, tpenv)

        | SynExpr.YieldOrReturnFrom(flags = (isYield, _); expr = synYieldExpr; trivia = { YieldOrReturnFromKeyword = m }) ->
            let env = { env with eIsControlFlow = false }
            let resultExpr, genExprTy, tpenv = TcExprOfUnknownType cenv env tpenv synYieldExpr

            if not isYield then
                errorR (Error(FSComp.SR.tcUseYieldBangForMultipleResults (), m))

            AddCxTypeMustSubsumeType ContextInfo.NoContext env.DisplayEnv cenv.css synYieldExpr.Range NoTrace genOuterTy genExprTy

            let resultExpr =
                mkCoerceExpr (resultExpr, genOuterTy, synYieldExpr.Range, genExprTy)

            let resultExpr =
                if IsControlFlowExpression synYieldExpr then
                    resultExpr
                else
                    mkDebugPoint resultExpr.Range resultExpr

            Some(resultExpr, tpenv)

        | SynExpr.YieldOrReturn(flags = (isYield, _); expr = synYieldExpr; trivia = { YieldOrReturnKeyword = m }) ->
            let env = { env with eIsControlFlow = false }
            let genResultTy = NewInferenceType g

            if not isYield then
                errorR (Error(FSComp.SR.tcSeqResultsUseYield (), m))

            UnifyTypes cenv env synYieldExpr.Range genOuterTy (mkSeqTy cenv.g genResultTy)

            let resultExpr, tpenv = TcExprFlex cenv flex true genResultTy env tpenv synYieldExpr

            let resultExpr = mkCallSeqSingleton cenv.g synYieldExpr.Range genResultTy resultExpr

            let resultExpr =
                if IsControlFlowExpression synYieldExpr then
                    resultExpr
                else
                    mkDebugPoint synYieldExpr.Range resultExpr

            Some(resultExpr, tpenv)

        | _ -> None

    and tcSequenceExprBody env (genOuterTy: TType) tpenv comp =
        let res, tpenv = tcSequenceExprBodyAsSequenceOrStatement env genOuterTy tpenv comp

        match res with
        | Choice1Of2 expr -> expr, tpenv
        | Choice2Of2 stmt ->
            let m = comp.Range
            let resExpr = Expr.Sequential(stmt, mkSeqEmpty cenv env m genOuterTy, NormalSeq, m)
            resExpr, tpenv

    and tcSequenceExprBodyAsSequenceOrStatement env genOuterTy tpenv comp =
        match tryTcSequenceExprBody env genOuterTy tpenv comp with
        | Some(expr, tpenv) -> Choice1Of2 expr, tpenv
        | None ->

            let env =
                { env with
                    eContextInfo = ContextInfo.SequenceExpression genOuterTy
                }

            if enableImplicitYield then
                let hasTypeUnit, _ty, expr, tpenv = TryTcStmt cenv env tpenv comp

                if hasTypeUnit then
                    Choice2Of2 expr, tpenv
                else
                    let genResultTy = NewInferenceType g
                    let mExpr = expr.Range
                    UnifyTypes cenv env mExpr genOuterTy (mkSeqTy cenv.g genResultTy)
                    let expr, tpenv = TcExprFlex cenv flex true genResultTy env tpenv comp
                    let exprTy = tyOfExpr cenv.g expr
                    AddCxTypeMustSubsumeType env.eContextInfo env.DisplayEnv cenv.css mExpr NoTrace genResultTy exprTy

                    let resExpr =
                        mkCallSeqSingleton cenv.g mExpr genResultTy (mkCoerceExpr (expr, genResultTy, mExpr, exprTy))

                    Choice1Of2 resExpr, tpenv
            else
                let stmt, tpenv = TcStmtThatCantBeCtorBody cenv env tpenv comp
                Choice2Of2 stmt, tpenv

    let coreExpr, tpenv = tcSequenceExprBody env overallTy.Commit tpenv comp
    let delayedExpr = mkSeqDelayedExpr coreExpr.Range coreExpr
    delayedExpr, tpenv

let TcSequenceExpressionEntry (cenv: TcFileState) env (overallTy: OverallTy) tpenv (hasBuilder, comp) m =
    match RewriteRangeExpr comp with
    | Some replacementExpr -> TcExpr cenv overallTy env tpenv replacementExpr
    | None ->
        let implicitYieldEnabled =
            cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield

        let validateObjectSequenceOrRecordExpression = not implicitYieldEnabled

        match comp with
        | SimpleSemicolonSequence cenv false _ when validateObjectSequenceOrRecordExpression ->
            errorR (Error(FSComp.SR.tcInvalidObjectSequenceOrRecordExpression (), m))
        | _ -> ()

        if not hasBuilder && not cenv.g.compilingFSharpCore then
            error (Error(FSComp.SR.tcInvalidSequenceExpressionSyntaxForm (), m))

        TcSequenceExpression cenv env tpenv comp overallTy m
