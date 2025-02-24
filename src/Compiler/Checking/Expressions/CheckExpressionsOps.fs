// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckExpressionsOps

open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Syntax
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.SyntaxTreeOps

let TryAllowFlexibleNullnessInControlFlow isFirst (g: TcGlobals.TcGlobals) ty =
    match isFirst, g.checkNullness, GetTyparTyIfSupportsNull g ty with
    | true, true, ValueSome tp -> tp.SetSupportsNullFlex(true)
    | _ -> ()

let CopyAndFixupTypars g m rigid tpsorig =
    FreshenAndFixupTypars g m rigid [] [] tpsorig

let FreshenPossibleForallTy g m rigid ty =
    let origTypars, tau = tryDestForallTy g ty

    if isNil origTypars then
        [], [], [], tau
    else
        // tps may be have been equated to other tps in equi-recursive type inference and units-of-measure type inference. Normalize them here
        let origTypars = NormalizeDeclaredTyparsForEquiRecursiveInference g origTypars
        let tps, renaming, tinst = CopyAndFixupTypars g m rigid origTypars
        origTypars, tps, tinst, instType renaming tau

/// simplified version of TcVal used in calls to BuildMethodCall (typrelns.fs)
/// this function is used on typechecking step for making calls to provided methods and on optimization step (for the same purpose).
let LightweightTcValForUsingInBuildMethodCall g (vref: ValRef) vrefFlags (vrefTypeInst: TTypes) m =
    let v = vref.Deref
    let vTy = vref.Type
    // byref-typed values get dereferenced
    if isByrefTy g vTy then
        mkAddrGet m vref, destByrefTy g vTy
    else
        match v.LiteralValue with
        | Some literalConst ->
            let _, _, _, tau = FreshenPossibleForallTy g m TyparRigidity.Flexible vTy
            Expr.Const(literalConst, m, tau), tau

        | None ->
            // Instantiate the value
            let tau =
                // If we have got an explicit instantiation then use that
                let _, tps, tpTys, tau = FreshenPossibleForallTy g m TyparRigidity.Flexible vTy

                if tpTys.Length <> vrefTypeInst.Length then
                    error (Error(FSComp.SR.tcTypeParameterArityMismatch (tps.Length, vrefTypeInst.Length), m))

                instType (mkTyparInst tps vrefTypeInst) tau

            let exprForVal = Expr.Val(vref, vrefFlags, m)
            let exprForVal = mkTyAppExpr m (exprForVal, vTy) vrefTypeInst
            exprForVal, tau

//-------------------------------------------------------------------------
// Helpers dealing with pattern match compilation
//-------------------------------------------------------------------------

let CompilePatternForMatch
    (cenv: TcFileState)
    (env: TcEnv)
    mExpr
    mMatch
    warnOnUnused
    actionOnFailure
    (inputVal, generalizedTypars, inputExprOpt)
    clauses
    inputTy
    resultTy
    =
    let g = cenv.g

    let dtree, targets =
        CompilePattern
            g
            env.DisplayEnv
            cenv.amap
            (LightweightTcValForUsingInBuildMethodCall g)
            cenv.infoReader
            mExpr
            mMatch
            warnOnUnused
            actionOnFailure
            (inputVal, generalizedTypars, inputExprOpt)
            clauses
            inputTy
            resultTy

    mkAndSimplifyMatch DebugPointAtBinding.NoneAtInvisible mExpr mMatch resultTy dtree targets

/// Invoke pattern match compilation
let CompilePatternForMatchClauses (cenv: TcFileState) env mExpr mMatch warnOnUnused actionOnFailure inputExprOpt inputTy resultTy tclauses =
    // Avoid creating a dummy in the common cases where we are about to bind a name for the expression
    // CLEANUP: avoid code duplication with code further below, i.e.all callers should call CompilePatternForMatch
    match tclauses with
    | [ MatchClause(TPat_as(pat1, PatternValBinding(asVal, GeneralizedType(generalizedTypars, _)), _), None, TTarget(vs, targetExpr, _), m2) ] ->
        let vs2 = ListSet.remove valEq asVal vs

        let expr =
            CompilePatternForMatch
                cenv
                env
                mExpr
                mMatch
                warnOnUnused
                actionOnFailure
                (asVal, generalizedTypars, None)
                [ MatchClause(pat1, None, TTarget(vs2, targetExpr, None), m2) ]
                inputTy
                resultTy

        asVal, expr
    | _ ->
        let matchValueTmp, _ = mkCompGenLocal mExpr "matchValue" inputTy

        let expr =
            CompilePatternForMatch
                cenv
                env
                mExpr
                mMatch
                warnOnUnused
                actionOnFailure
                (matchValueTmp, [], inputExprOpt)
                tclauses
                inputTy
                resultTy

        matchValueTmp, expr

/// Constrain two types to be equal within this type checking context
let inline UnifyTypes (cenv: TcFileState) (env: TcEnv) m expectedTy actualTy =

    AddCxTypeEqualsType
        env.eContextInfo
        env.DisplayEnv
        cenv.css
        m
        (tryNormalizeMeasureInType cenv.g expectedTy)
        (tryNormalizeMeasureInType cenv.g actualTy)

// Converts 'a..b' to a call to the '(..)' operator in FSharp.Core
// Converts 'a..b..c' to a call to the '(.. ..)' operator in FSharp.Core
//
// NOTE: we could eliminate these more efficiently in LowerComputedCollections.fs, since
//    [| 1..4 |]
// becomes [| for i in (..) 1 4 do yield i |]
// instead of generating the array directly from the ranges
let RewriteRangeExpr synExpr =
    match synExpr with
    // a..b..c (parsed as (a..b)..c )
    | SynExpr.IndexRange(Some(SynExpr.IndexRange(Some synExpr1, _, Some synStepExpr, _, _, _)), _, Some synExpr2, _m1, _m2, mWhole) ->
        let mWhole = mWhole.MakeSynthetic()
        Some(mkSynTrifix mWhole ".. .." synExpr1 synStepExpr synExpr2)
    // a..b
    | SynExpr.IndexRange(Some synExpr1, mOperator, Some synExpr2, _m1, _m2, mWhole) ->
        let otherExpr =
            let mWhole = mWhole.MakeSynthetic()

            match mkSynInfix mOperator synExpr1 ".." synExpr2 with
            | SynExpr.App(a, b, c, d, _) -> SynExpr.App(a, b, c, d, mWhole)
            | _ -> failwith "impossible"

        Some otherExpr
    | _ -> None

/// Check if a computation or sequence expression is syntactically free of 'yield' (though not yield!)
let YieldFree (cenv: TcFileState) expr =
    if cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield then

        // Implement yield free logic for F# Language including the LanguageFeature.ImplicitYield
        let rec YieldFree expr =
            match expr with
            | SynExpr.Sequential(expr1 = expr1; expr2 = expr2) -> YieldFree expr1 && YieldFree expr2

            | SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = elseExprOpt) -> YieldFree thenExpr && Option.forall YieldFree elseExprOpt

            | SynExpr.TryWith(tryExpr = body; withCases = clauses) ->
                YieldFree body
                && clauses |> List.forall (fun (SynMatchClause(resultExpr = res)) -> YieldFree res)

            | SynExpr.Match(clauses = clauses)
            | SynExpr.MatchBang(clauses = clauses) -> clauses |> List.forall (fun (SynMatchClause(resultExpr = res)) -> YieldFree res)

            | SynExpr.For(doBody = body)
            | SynExpr.TryFinally(tryExpr = body)
            | SynExpr.LetOrUse(body = body)
            | SynExpr.While(doExpr = body)
            | SynExpr.WhileBang(doExpr = body)
            | SynExpr.ForEach(bodyExpr = body) -> YieldFree body

            | SynExpr.LetOrUseBang(body = body) -> YieldFree body

            | SynExpr.YieldOrReturn(flags = (true, _)) -> false

            | _ -> true

        YieldFree expr
    else
        // Implement yield free logic for F# Language without the LanguageFeature.ImplicitYield
        let rec YieldFree expr =
            match expr with
            | SynExpr.Sequential(expr1 = expr1; expr2 = expr2) -> YieldFree expr1 && YieldFree expr2

            | SynExpr.IfThenElse(thenExpr = thenExpr; elseExpr = elseExprOpt) -> YieldFree thenExpr && Option.forall YieldFree elseExprOpt

            | SynExpr.TryWith(tryExpr = e1; withCases = clauses) ->
                YieldFree e1
                && clauses |> List.forall (fun (SynMatchClause(resultExpr = res)) -> YieldFree res)

            | SynExpr.Match(clauses = clauses)
            | SynExpr.MatchBang(clauses = clauses) -> clauses |> List.forall (fun (SynMatchClause(resultExpr = res)) -> YieldFree res)

            | SynExpr.For(doBody = body)
            | SynExpr.TryFinally(tryExpr = body)
            | SynExpr.LetOrUse(body = body)
            | SynExpr.While(doExpr = body)
            | SynExpr.WhileBang(doExpr = body)
            | SynExpr.ForEach(bodyExpr = body) -> YieldFree body

            | SynExpr.LetOrUseBang _
            | SynExpr.YieldOrReturnFrom _
            | SynExpr.YieldOrReturn _
            | SynExpr.ImplicitZero _
            | SynExpr.Do _ -> false

            | _ -> true

        YieldFree expr

let inline IsSimpleSemicolonSequenceElement expr cenv acceptDeprecated =
    match expr with
    | SynExpr.IfThenElse _ when acceptDeprecated && YieldFree cenv expr -> true
    | SynExpr.IfThenElse _
    | SynExpr.TryWith _
    | SynExpr.Match _
    | SynExpr.For _
    | SynExpr.ForEach _
    | SynExpr.TryFinally _
    | SynExpr.YieldOrReturnFrom _
    | SynExpr.YieldOrReturn _
    | SynExpr.LetOrUse _
    | SynExpr.Do _
    | SynExpr.MatchBang _
    | SynExpr.LetOrUseBang _
    | SynExpr.While _
    | SynExpr.WhileBang _ -> false
    | _ -> true

[<TailCall>]
let rec TryGetSimpleSemicolonSequenceOfComprehension expr acc cenv acceptDeprecated =
    match expr with
    | SynExpr.Sequential(isTrueSeq = true; expr1 = e1; expr2 = e2) ->
        if IsSimpleSemicolonSequenceElement e1 cenv acceptDeprecated then
            TryGetSimpleSemicolonSequenceOfComprehension e2 (e1 :: acc) cenv acceptDeprecated
        else
            ValueNone
    | _ ->
        if IsSimpleSemicolonSequenceElement expr cenv acceptDeprecated then
            ValueSome(List.rev (expr :: acc))
        else
            ValueNone

/// Determine if a syntactic expression inside 'seq { ... }' or '[...]' counts as a "simple sequence
/// of semicolon separated values". For example [1;2;3].
/// 'acceptDeprecated' is true for the '[ ... ]' case, where we allow the syntax '[ if g then t else e ]' but ask it to be parenthesized
[<return: Struct>]
let (|SimpleSemicolonSequence|_|) cenv acceptDeprecated cexpr =
    TryGetSimpleSemicolonSequenceOfComprehension cexpr [] cenv acceptDeprecated

let elimFastIntegerForLoop (spFor, spTo, id, start: SynExpr, dir, finish: SynExpr, innerExpr, m: range) =
    let mOp = (unionRanges start.Range finish.Range).MakeSynthetic()

    let pseudoEnumExpr =
        if dir then
            mkSynInfix mOp start ".." finish
        else
            mkSynTrifix mOp ".. .." start (SynExpr.Const(SynConst.Int32 -1, mOp)) finish

    SynExpr.ForEach(spFor, spTo, SeqExprOnly false, true, mkSynPatVar None id, pseudoEnumExpr, innerExpr, m)

let mkSeqEmpty (cenv: TcFileState) env m genTy =
    // We must discover the 'zero' of the monadic algebra being generated in order to compile failing matches.
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy g genResultTy)
    mkCallSeqEmpty g m genResultTy

let mkSeqUsing (cenv: TcFileState) (env: TcEnv) m resourceTy genTy resourceExpr lam =
    let g = cenv.g
    AddCxTypeMustSubsumeType ContextInfo.NoContext env.DisplayEnv cenv.css m NoTrace g.system_IDisposableNull_ty resourceTy
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    mkCallSeqUsing cenv.g m resourceTy genResultTy resourceExpr lam

let mkSeqAppend (cenv: TcFileState) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)

    let e1 =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e1) e1

    let e2 =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e2) e2

    mkCallSeqAppend cenv.g m genResultTy e1 e2

let mkSeqDelay (cenv: TcFileState) env m genTy lam =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    mkCallSeqDelay cenv.g m genResultTy (mkUnitDelayLambda cenv.g m lam)

let mkSeqCollect (cenv: TcFileState) env m enumElemTy genTy lam enumExpr =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)

    let enumExpr =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g enumElemTy) (tyOfExpr cenv.g enumExpr) enumExpr

    mkCallSeqCollect cenv.g m enumElemTy genResultTy lam enumExpr

let mkSeqFromFunctions (cenv: TcFileState) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)

    let e2 =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e2) e2

    mkCallSeqGenerated cenv.g m genResultTy e1 e2

let mkSeqFinally (cenv: TcFileState) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)

    let e1 =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e1) e1

    mkCallSeqFinally cenv.g m genResultTy e1 e2

let mkSeqTryWith (cenv: TcFileState) env m genTy origSeq exnFilter exnHandler =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)

    let origSeq =
        mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g origSeq) origSeq

    mkCallSeqTryWith cenv.g m genResultTy origSeq exnFilter exnHandler

let inline mkSeqExprMatchClauses (pat, vspecs) innerExpr =
    [ MatchClause(pat, None, TTarget(vspecs, innerExpr, None), pat.Range) ]

let compileSeqExprMatchClauses (cenv: TcFileState) env inputExprMark (pat: Pattern, vspecs) innerExpr inputExprOpt bindPatTy genInnerTy =
    let patMark = pat.Range
    let tclauses = mkSeqExprMatchClauses (pat, vspecs) innerExpr

    CompilePatternForMatchClauses
        cenv
        env
        inputExprMark
        patMark
        false
        ThrowIncompleteMatchException
        inputExprOpt
        bindPatTy
        genInnerTy
        tclauses
