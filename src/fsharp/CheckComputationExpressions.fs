// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The typechecker. Left-to-right constrained type checking 
/// with generalization at appropriate points.
module internal FSharp.Compiler.CheckComputationExpressions

open Internal.Utilities.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

type cenv = TcFileState

/// Used to flag if this is the first or a sebsequent translation pass through a computation expression
type CompExprTranslationPass = Initial | Subsequent

/// Used to flag if computation expression custom operations are allowed in a given context
type CustomOperationsMode = Allowed | Denied

let TryFindIntrinsicOrExtensionMethInfo collectionSettings (cenv: cenv) (env: TcEnv) m ad nm ty = 
    AllMethInfosOfTypeInScope collectionSettings cenv.infoReader env.NameEnv (Some nm) ad IgnoreOverrides m ty

/// Ignores an attribute
let IgnoreAttribute _ = None

let (|ExprAsPat|_|) (f: SynExpr) =    
    match f with 
    | SingleIdent v1 | SynExprParen(SingleIdent v1, _, _, _) -> Some (mkSynPatVar None v1)
    | SynExprParen(SynExpr.Tuple (false, elems, _, _), _, _, _) -> 
        let elems = elems |> List.map (|SingleIdent|_|) 
        if elems |> List.forall (fun x -> x.IsSome) then 
            Some (SynPat.Tuple(false, (elems |> List.map (fun x -> mkSynPatVar None x.Value)), f.Range))
        else
            None
    | _ -> None

// For join clauses that join on nullable, we syntactically insert the creation of nullable values on the appropriate side of the condition, 
// then pull the syntax apart again
let (|JoinRelation|_|) cenv env (e: SynExpr) = 
    let m = e.Range
    let ad = env.eAccessRights

    let isOpName opName vref s =
        (s = opName) &&
        match ResolveExprLongIdent cenv.tcSink cenv.nameResolver m ad env.eNameResEnv TypeNameResolutionInfo.Default [ident(opName, m)] with
        | Result (_, Item.Value vref2, []) -> valRefEq cenv.g vref vref2
        | _ -> false

    match e with 
    | BinOpExpr(opId, a, b) when isOpName opNameEquals cenv.g.equals_operator_vref opId.idText -> Some (a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameEqualsNullable cenv.g.equals_nullable_operator_vref opId.idText -> 

        let a = SynExpr.App (ExprAtomicFlag.Atomic, false, mkSynLidGet a.Range [MangledGlobalName;"System"] "Nullable", a, a.Range)
        Some (a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameNullableEquals cenv.g.nullable_equals_operator_vref opId.idText -> 

        let b = SynExpr.App (ExprAtomicFlag.Atomic, false, mkSynLidGet b.Range [MangledGlobalName;"System"] "Nullable", b, b.Range)
        Some (a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameNullableEqualsNullable cenv.g.nullable_equals_nullable_operator_vref opId.idText -> 

        Some (a, b)

    | _ -> None

let elimFastIntegerForLoop (spFor, spTo, id, start, dir, finish, innerExpr, m) = 
    let pseudoEnumExpr = 
        if dir then mkSynInfix m start ".." finish
        else mkSynTrifix m ".. .." start (SynExpr.Const (SynConst.Int32 -1, start.Range)) finish
    SynExpr.ForEach (spFor, spTo, SeqExprOnly false, true, mkSynPatVar None id, pseudoEnumExpr, innerExpr, m)

/// Check if a computation or sequence expression is syntactically free of 'yield' (though not yield!)
let YieldFree (cenv: cenv) expr =
    if cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield then

        // Implement yield free logic for F# Language including the LanguageFeature.ImplicitYield
        let rec YieldFree expr =
            match expr with
            | SynExpr.Sequential (expr1=e1; expr2=e2) ->
                YieldFree e1 && YieldFree e2

            | SynExpr.IfThenElse (thenExpr=e2; elseExpr=e3opt) ->
                YieldFree e2 && Option.forall YieldFree e3opt

            | SynExpr.TryWith (tryExpr=e1; withCases=clauses) ->
                YieldFree e1 && clauses |> List.forall (fun (SynMatchClause(resultExpr = e)) -> YieldFree e)

            | SynExpr.Match (clauses=clauses) | SynExpr.MatchBang (clauses=clauses) ->
                clauses |> List.forall (fun (SynMatchClause(resultExpr = e)) -> YieldFree e)

            | SynExpr.For (doBody=body)
            | SynExpr.TryFinally (tryExpr=body)
            | SynExpr.LetOrUse (body=body)
            | SynExpr.While (doExpr=body)
            | SynExpr.ForEach (bodyExpr=body) ->
                YieldFree body

            | SynExpr.LetOrUseBang(body=body) ->
                YieldFree body

            | SynExpr.YieldOrReturn(flags=(true, _)) -> false

            | _ -> true

        YieldFree expr
    else
        // Implement yield free logic for F# Language without the LanguageFeature.ImplicitYield
        let rec YieldFree expr =
            match expr with
            | SynExpr.Sequential (expr1=e1; expr2=e2) ->
                YieldFree e1 && YieldFree e2

            | SynExpr.IfThenElse (thenExpr=e2; elseExpr=e3opt) ->
                YieldFree e2 && Option.forall YieldFree e3opt

            | SynExpr.TryWith (tryExpr=e1; withCases=clauses) ->
                YieldFree e1 && clauses |> List.forall (fun (SynMatchClause(resultExpr = e)) -> YieldFree e)

            | SynExpr.Match (clauses=clauses) | SynExpr.MatchBang (clauses=clauses) ->
                clauses |> List.forall (fun (SynMatchClause(resultExpr = e)) -> YieldFree e)

            | SynExpr.For (doBody=body)
            | SynExpr.TryFinally (tryExpr=body)
            | SynExpr.LetOrUse (body=body)
            | SynExpr.While (doExpr=body)
            | SynExpr.ForEach (bodyExpr=body) ->
                YieldFree body

            | SynExpr.LetOrUseBang _
            | SynExpr.YieldOrReturnFrom _
            | SynExpr.YieldOrReturn _
            | SynExpr.ImplicitZero _
            | SynExpr.Do _ -> false

            | _ -> true

        YieldFree expr


/// Determine if a syntactic expression inside 'seq { ... }' or '[...]' counts as a "simple sequence
/// of semicolon separated values". For example [1;2;3].
/// 'acceptDeprecated' is true for the '[ ... ]' case, where we allow the syntax '[ if g then t else e ]' but ask it to be parenthesized
///
let (|SimpleSemicolonSequence|_|) cenv acceptDeprecated cexpr = 

    let IsSimpleSemicolonSequenceElement expr = 
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
        | SynExpr.While _ -> false
        | _ -> true

    let rec TryGetSimpleSemicolonSequenceOfComprehension expr acc = 
        match expr with 
        | SynExpr.Sequential (_, true, e1, e2, _) -> 
            if IsSimpleSemicolonSequenceElement e1 then 
                TryGetSimpleSemicolonSequenceOfComprehension e2 (e1 :: acc)
            else
                None 
        | e -> 
            if IsSimpleSemicolonSequenceElement e then 
                Some(List.rev (e :: acc))
            else 
                None 

    TryGetSimpleSemicolonSequenceOfComprehension cexpr []

let RecordNameAndTypeResolutions_IdeallyWithoutHavingOtherEffects cenv env tpenv expr =
    // This function is motivated by cases like
    //    query { for ... join(for x in f(). }
    // where there is incomplete code in a query, and we are current just dropping a piece of the AST on the floor (above, the bit inside the 'join').
    // 
    // The problem with dropping the AST on the floor is that we get no captured resolutions, which means no Intellisense/QuickInfo/ParamHelp.
    //
    // The idea behind the fix is to semi-typecheck this AST-fragment, just to get resolutions captured.
    //
    // The tricky bit is to not also have any other effects from typechecking, namely producing error diagnostics (which may be spurious) or having 
    // side-effects on the typecheck environment.
    //
    // REVIEW: We are yet to deal with the tricky bit. As it stands, we turn off error logging, but still have typechecking environment effects. As a result, 
    // at the very least, you cannot call this function unless you're already reported a typechecking error (the 'worst' possible outcome would be 
    // to incorrectly solve typecheck constraints as a result of effects in this function, and then have the code compile successfully and behave 
    // in some weird way; so ensure the code can't possibly compile before calling this function as an expedient way to get better IntelliSense).
    suppressErrorReporting (fun () -> 
        try ignore(TcExprOfUnknownType cenv env tpenv expr)
        with e -> ())

/// Used for all computation expressions except sequence expressions
let TcComputationExpression (cenv: cenv) env (overallTy: OverallTy) tpenv (mWhole, interpExpr: Expr, builderTy, comp: SynExpr) = 
    let overallTy = overallTy.Commit
    
    let g = cenv.g
    let ad = env.eAccessRights

    let mkSynDelay2 (e: SynExpr) = mkSynDelay (e.Range.MakeSynthetic()) e
    
    let builderValName = CompilerGeneratedName "builder"
    let mBuilderVal = interpExpr.Range
    
    // Give bespoke error messages for the FSharp.Core "query" builder
    let isQuery = 
        match stripDebugPoints interpExpr with 
        | Expr.Val (vf, _, m) -> 
            let item = Item.CustomBuilder (vf.DisplayName, vf)
            CallNameResolutionSink cenv.tcSink (m, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.eAccessRights)
            valRefEq cenv.g vf cenv.g.query_value_vref 
        | _ -> false

    /// Make a builder.Method(...) call
    let mkSynCall nm (m: range) args = 
        let m = m.MakeSynthetic() // Mark as synthetic so the language service won't pick it up.
        let args = 
            match args with 
            | [] -> SynExpr.Const (SynConst.Unit, m)
            | [arg] -> SynExpr.Paren (SynExpr.Paren (arg, range0, None, m), range0, None, m)
            | args -> SynExpr.Paren (SynExpr.Tuple (false, args, [], m), range0, None, m)
                
        let builderVal = mkSynIdGet m builderValName
        mkSynApp1 (SynExpr.DotGet (builderVal, range0, LongIdentWithDots([mkSynId m nm], []), m)) args m

    let hasMethInfo nm = TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad nm builderTy |> isNil |> not

    let sourceMethInfo = TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Source" builderTy 

    // Optionally wrap sources of "let!", "yield!", "use!" in "query.Source"
    let mkSourceExpr callExpr = 
        match sourceMethInfo with 
        | [] -> callExpr
        | _ -> mkSynCall "Source" callExpr.Range [callExpr]

    let mkSourceExprConditional isFromSource callExpr = 
        if isFromSource then mkSourceExpr callExpr else callExpr

    /// Decide if the builder is an auto-quote builder
    let isAutoQuote = hasMethInfo "Quote"

    let customOperationMethods = 
        AllMethInfosOfTypeInScope ResultCollectionSettings.AllResults cenv.infoReader env.NameEnv None ad IgnoreOverrides mBuilderVal builderTy
        |> List.choose (fun methInfo -> 
                if not (IsMethInfoAccessible cenv.amap mBuilderVal ad methInfo) then None else
                let nameSearch = 
                    TryBindMethInfoAttribute cenv.g mBuilderVal cenv.g.attrib_CustomOperationAttribute methInfo 
                        IgnoreAttribute // We do not respect this attribute for IL methods
                        (function Attrib(_, _, [ AttribStringArg msg ], _, _, _, _) -> Some msg | _ -> None)
                        IgnoreAttribute // We do not respect this attribute for provided methods

                match nameSearch with
                | None -> None
                | Some nm ->
                    let joinConditionWord =
                        TryBindMethInfoAttribute cenv.g mBuilderVal cenv.g.attrib_CustomOperationAttribute methInfo 
                            IgnoreAttribute // We do not respect this attribute for IL methods
                            (function Attrib(_, _, _, ExtractAttribNamedArg "JoinConditionWord" (AttribStringArg s), _, _, _) -> Some s | _ -> None)
                            IgnoreAttribute // We do not respect this attribute for provided methods

                    let flagSearch (propName: string) = 
                        TryBindMethInfoAttribute cenv.g mBuilderVal cenv.g.attrib_CustomOperationAttribute methInfo 
                            IgnoreAttribute // We do not respect this attribute for IL methods
                            (function Attrib(_, _, _, ExtractAttribNamedArg propName (AttribBoolArg b), _, _, _) -> Some b | _ -> None)
                            IgnoreAttribute // We do not respect this attribute for provided methods

                    let maintainsVarSpaceUsingBind = defaultArg (flagSearch "MaintainsVariableSpaceUsingBind") false
                    let maintainsVarSpace = defaultArg (flagSearch "MaintainsVariableSpace") false
                    let allowInto = defaultArg (flagSearch "AllowIntoPattern") false
                    let isLikeZip = defaultArg (flagSearch "IsLikeZip") false
                    let isLikeJoin = defaultArg (flagSearch "IsLikeJoin") false
                    let isLikeGroupJoin = defaultArg (flagSearch "IsLikeGroupJoin") false

                    Some (nm, maintainsVarSpaceUsingBind, maintainsVarSpace, allowInto, isLikeZip, isLikeJoin, isLikeGroupJoin, joinConditionWord, methInfo))

    let customOperationMethodsIndexedByKeyword = 
        if cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
            customOperationMethods
            |> Seq.groupBy (fun (nm, _, _, _, _, _, _, _, _) -> nm)
            |> Seq.map (fun (nm, group) ->
                (nm,
                    group
                    |> Seq.toList))
        else
            customOperationMethods
            |> Seq.groupBy (fun (nm, _, _, _, _, _, _, _, _) -> nm)
            |> Seq.map (fun (nm, g) -> (nm, Seq.toList g))
        |> dict

    // Check for duplicates by method name (keywords and method names must be 1:1)
    let customOperationMethodsIndexedByMethodName = 
        if cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
            customOperationMethods
            |> Seq.groupBy (fun (_, _, _, _, _, _, _, _, methInfo) -> methInfo.LogicalName)
            |> Seq.map (fun (nm, group) ->
                (nm,
                    group
                    |> Seq.toList))
        else
            customOperationMethods
            |> Seq.groupBy (fun (_, _, _, _, _, _, _, _, methInfo) -> methInfo.LogicalName)
            |> Seq.map (fun (nm, g) -> (nm, Seq.toList g))
        |> dict

    /// Decide if the identifier represents a use of a custom query operator
    let tryGetDataForCustomOperation (nm: Ident) =
        let isOpDataCountAllowed opDatas =
            match opDatas with
            | [_] -> true
            | _ :: _ -> cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations
            | _ -> false

        match customOperationMethodsIndexedByKeyword.TryGetValue nm.idText with 
        | true, opDatas when isOpDataCountAllowed opDatas -> 
            for opData in opDatas do
                let opName, maintainsVarSpaceUsingBind, maintainsVarSpace, _allowInto, isLikeZip, isLikeJoin, isLikeGroupJoin, _joinConditionWord, methInfo = opData
                if (maintainsVarSpaceUsingBind && maintainsVarSpace) || (isLikeZip && isLikeJoin) || (isLikeZip && isLikeGroupJoin) || (isLikeJoin && isLikeGroupJoin) then 
                     errorR(Error(FSComp.SR.tcCustomOperationInvalid opName, nm.idRange))
                if not (cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations) then
                    match customOperationMethodsIndexedByMethodName.TryGetValue methInfo.LogicalName with 
                    | true, [_] -> ()
                    | _ -> errorR(Error(FSComp.SR.tcCustomOperationMayNotBeOverloaded nm.idText, nm.idRange))
            Some opDatas
        | true, opData :: _ -> errorR(Error(FSComp.SR.tcCustomOperationMayNotBeOverloaded nm.idText, nm.idRange)); Some [opData]
        | _ -> None

    /// Decide if the identifier represents a use of a custom query operator
    let hasCustomOperations () = if isNil customOperationMethods then CustomOperationsMode.Denied else CustomOperationsMode.Allowed

    let isCustomOperation nm = tryGetDataForCustomOperation nm |> Option.isSome

    let customOperationCheckValidity m f opDatas = 
        let vs = opDatas |> List.map f
        let v0 = vs[0]
        let opName, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo = opDatas[0]
        if not (List.allEqual vs) then 
            errorR(Error(FSComp.SR.tcCustomOperationInvalid opName, m))
        v0

    // Check for the MaintainsVariableSpace on custom operation
    let customOperationMaintainsVarSpace (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo) -> maintainsVarSpace)

    let customOperationMaintainsVarSpaceUsingBind (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo) -> maintainsVarSpaceUsingBind)

    let customOperationIsLikeZip (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo) -> isLikeZip)

    let customOperationIsLikeJoin (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo) -> isLikeJoin)

    let customOperationIsLikeGroupJoin (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, isLikeGroupJoin, _joinConditionWord, _methInfo) -> isLikeGroupJoin)

    let customOperationJoinConditionWord (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, joinConditionWord, _methInfo) -> joinConditionWord)
             |> function None -> "on" | Some v -> v
        | _ -> "on"  

    let customOperationAllowsInto (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | None -> false
        | Some opDatas ->
            opDatas |> customOperationCheckValidity nm.idRange (fun (_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, _methInfo) -> allowInto)

    let customOpUsageText nm = 
        match tryGetDataForCustomOperation nm with
        | Some ((_nm, _maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, isLikeZip, isLikeJoin, isLikeGroupJoin, _joinConditionWord, _methInfo) :: _) ->
            if isLikeGroupJoin then
                Some (FSComp.SR.customOperationTextLikeGroupJoin(nm.idText, customOperationJoinConditionWord nm, customOperationJoinConditionWord nm))
            elif isLikeJoin then
                Some (FSComp.SR.customOperationTextLikeJoin(nm.idText, customOperationJoinConditionWord nm, customOperationJoinConditionWord nm))
            elif isLikeZip then
                Some (FSComp.SR.customOperationTextLikeZip(nm.idText))
            else
                None
        | _ -> None 

    /// Inside the 'query { ... }' use a modified name environment that contains fake 'CustomOperation' entries
    /// for all custom operations. This adds them to the completion lists and prevents them being used as values inside
    /// the query.
    let env = 
        if List.isEmpty customOperationMethods then env else
        { env with 
            eNameResEnv =
                (env.eNameResEnv, customOperationMethods) 
                ||> Seq.fold (fun nenv (nm, _, _, _, _, _, _, _, methInfo) -> 
                    AddFakeNameToNameEnv nm nenv (Item.CustomOperation (nm, (fun () -> customOpUsageText (ident (nm, mBuilderVal))), Some methInfo))) }

    // Environment is needed for completions
    CallEnvSink cenv.tcSink (comp.Range, env.NameEnv, ad)

    let tryGetArgAttribsForCustomOperator (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | Some argInfos -> 
            argInfos 
            |> List.map (fun (_nm, __maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, methInfo) -> 
                match methInfo.GetParamAttribs(cenv.amap, mWhole) with 
                | [curriedArgInfo] -> Some curriedArgInfo // one for the actual argument group
                | _ -> None)
            |> Some
        | _ -> None

    let tryGetArgInfosForCustomOperator (nm: Ident) = 
        match tryGetDataForCustomOperation nm with 
        | Some argInfos -> 
            argInfos 
            |> List.map (fun (_nm, __maintainsVarSpaceUsingBind, _maintainsVarSpace, _allowInto, _isLikeZip, _isLikeJoin, _isLikeGroupJoin, _joinConditionWord, methInfo) -> 
                match methInfo with
                | FSMeth(_, _, vref, _) ->
                    match ArgInfosOfMember cenv.g vref with
                    | [curriedArgInfo] -> Some curriedArgInfo
                    | _ -> None
                | _ -> None)
            |> Some
        | _ -> None

    let tryExpectedArgCountForCustomOperator (nm: Ident) = 
        match tryGetArgAttribsForCustomOperator nm with 
        | None -> None
        | Some argInfosForOverloads -> 
            let nums = argInfosForOverloads |> List.map (function None -> -1 | Some argInfos -> List.length argInfos)

            // Prior to 'OverloadsForCustomOperations' we count exact arguments.
            //
            // With 'OverloadsForCustomOperations' we don't compute an exact expected argument count
            // if any arguments are optional, out or ParamArray.
            let isSpecial = 
                if cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
                    argInfosForOverloads |> List.exists (fun info -> 
                        match info with 
                        | None -> false
                        | Some args -> 
                            args |> List.exists (fun (isParamArrayArg, _isInArg, isOutArg, optArgInfo, _callerInfo, _reflArgInfo) -> isParamArrayArg || isOutArg || optArgInfo.IsOptional))
                else
                    false

            if not isSpecial && nums |> List.forall (fun v -> v >= 0 && v = nums[0]) then 
                Some (max (nums[0] - 1) 0) // drop the computation context argument
            else
                None

    // Check for the [<ProjectionParameter>] attribute on an argument position
    let isCustomOperationProjectionParameter i (nm: Ident) = 
        match tryGetArgInfosForCustomOperator nm with
        | None -> false
        | Some argInfosForOverloads ->
            let vs = 
                argInfosForOverloads |> List.map (function 
                    | None -> false
                    | Some argInfos -> 
                        i < argInfos.Length && 
                        let _, argInfo = List.item i argInfos
                        HasFSharpAttribute cenv.g cenv.g.attrib_ProjectionParameterAttribute argInfo.Attribs)
            if List.allEqual vs then vs[0]
            else 
                let opDatas = (tryGetDataForCustomOperation nm).Value
                let opName, _, _, _, _, _, _, _j, _ = opDatas[0]
                errorR(Error(FSComp.SR.tcCustomOperationInvalid opName, nm.idRange))
                false

    let (|ForEachThen|_|) e = 
        match e with 
        | SynExpr.ForEach (_spFor, _spIn, SeqExprOnly false, isFromSource, pat1, expr1, SynExpr.Sequential (_, true, clause, rest, _), _) ->
            Some (isFromSource, pat1, expr1, clause, rest)
        | _ -> None

    let (|CustomOpId|_|) predicate e = 
        match e with 
        | SingleIdent nm when isCustomOperation nm && predicate nm -> Some nm
        | _ -> None

    // e1 in e2 ('in' is parsed as 'JOIN_IN')
    let (|InExpr|_|) (e: SynExpr) = 
        match e with 
        | SynExpr.JoinIn (e1, _, e2, mApp) -> Some (e1, e2, mApp)
        | _ -> None

    // e1 on e2 (note: 'on' is the 'JoinConditionWord')
    let (|OnExpr|_|) nm (e: SynExpr) = 
        match tryGetDataForCustomOperation nm with 
        | None -> None
        | Some _ -> 
            match e with 
            | SynExpr.App (_, _, SynExpr.App (_, _, e1, SingleIdent opName, _), e2, _) when opName.idText = customOperationJoinConditionWord nm -> 
                let item = Item.CustomOperation (opName.idText, (fun () -> None), None)
                CallNameResolutionSink cenv.tcSink (opName.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.AccessRights)
                Some (e1, e2)
            | _ -> None

    // e1 into e2
    let (|IntoSuffix|_|) (e: SynExpr) = 
        match e with 
        | SynExpr.App (_, _, SynExpr.App (_, _, x, SingleIdent nm2, _), ExprAsPat intoPat, _) when nm2.idText = CustomOperations.Into -> 
            Some (x, nm2.idRange, intoPat)
        | _ -> 
            None

    let arbPat (m: range) = mkSynPatVar None (mkSynId (m.MakeSynthetic()) "_missingVar")

    let MatchIntoSuffixOrRecover alreadyGivenError (nm: Ident) (e: SynExpr) = 
        match e with 
        | IntoSuffix (x, intoWordRange, intoPat) -> 
            // record the "into" as a custom operation for colorization
            let item = Item.CustomOperation ("into", (fun () -> None), None)
            CallNameResolutionSink cenv.tcSink (intoWordRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.eAccessRights)
            (x, intoPat, alreadyGivenError)
        | _ -> 
            if not alreadyGivenError then 
                errorR(Error(FSComp.SR.tcOperatorIncorrectSyntax(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            (e, arbPat e.Range, true)

    let MatchOnExprOrRecover alreadyGivenError nm (onExpr: SynExpr) = 
        match onExpr with 
        | OnExpr nm (innerSource, SynExprParen(keySelectors, _, _, _)) -> 
            (innerSource, keySelectors)
        | _ -> 
            if not alreadyGivenError then 
                suppressErrorReporting (fun () -> TcExprOfUnknownType cenv env tpenv onExpr) |> ignore
                errorR(Error(FSComp.SR.tcOperatorIncorrectSyntax(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            (arbExpr("_innerSource", onExpr.Range), mkSynBifix onExpr.Range "=" (arbExpr("_keySelectors", onExpr.Range)) (arbExpr("_keySelector2", onExpr.Range)))

    let JoinOrGroupJoinOp detector e = 
        match e with 
        | SynExpr.App (_, _, CustomOpId detector nm, ExprAsPat innerSourcePat, mJoinCore) ->
            Some(nm, innerSourcePat, mJoinCore, false)
        // join with bad pattern (gives error on "join" and continues)
        | SynExpr.App (_, _, CustomOpId detector nm, _innerSourcePatExpr, mJoinCore) ->
            errorR(Error(FSComp.SR.tcBinaryOperatorRequiresVariable(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            Some(nm, arbPat mJoinCore, mJoinCore, true)
        // join (without anything after - gives error on "join" and continues)
        | CustomOpId detector nm -> 
            errorR(Error(FSComp.SR.tcBinaryOperatorRequiresVariable(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            Some(nm, arbPat e.Range, e.Range, true)
        | _ -> 
            None
            // JoinOrGroupJoinOp customOperationIsLikeJoin

    let (|JoinOp|_|) (e: SynExpr) = JoinOrGroupJoinOp customOperationIsLikeJoin e
    let (|GroupJoinOp|_|) (e: SynExpr) = JoinOrGroupJoinOp customOperationIsLikeGroupJoin e

    let arbKeySelectors m = mkSynBifix m "=" (arbExpr("_keySelectors", m)) (arbExpr("_keySelector2", m))

    let (|JoinExpr|_|) (e: SynExpr) = 
        match e with 
        | InExpr (JoinOp(nm, innerSourcePat, _, alreadyGivenError), onExpr, mJoinCore) -> 
            let innerSource, keySelectors = MatchOnExprOrRecover alreadyGivenError nm onExpr
            Some(nm, innerSourcePat, innerSource, keySelectors, mJoinCore)
        | JoinOp (nm, innerSourcePat, mJoinCore, alreadyGivenError) ->
            if alreadyGivenError then 
                errorR(Error(FSComp.SR.tcOperatorRequiresIn(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            Some (nm, innerSourcePat, arbExpr("_innerSource", e.Range), arbKeySelectors e.Range, mJoinCore)
        | _ -> None

    let (|GroupJoinExpr|_|) (e: SynExpr) = 
        match e with 
        | InExpr (GroupJoinOp (nm, innerSourcePat, _, alreadyGivenError), intoExpr, mGroupJoinCore) ->
            let onExpr, intoPat, alreadyGivenError = MatchIntoSuffixOrRecover alreadyGivenError nm intoExpr 
            let innerSource, keySelectors = MatchOnExprOrRecover alreadyGivenError nm onExpr
            Some (nm, innerSourcePat, innerSource, keySelectors, intoPat, mGroupJoinCore)
        | GroupJoinOp (nm, innerSourcePat, mGroupJoinCore, alreadyGivenError) ->
            if alreadyGivenError then 
               errorR(Error(FSComp.SR.tcOperatorRequiresIn(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            Some (nm, innerSourcePat, arbExpr("_innerSource", e.Range), arbKeySelectors e.Range, arbPat e.Range, mGroupJoinCore)
        | _ -> 
            None


    let (|JoinOrGroupJoinOrZipClause|_|) (e: SynExpr) = 
        match e with 

        // join innerSourcePat in innerSource on (keySelector1 = keySelector2)
        | JoinExpr (nm, innerSourcePat, innerSource, keySelectors, mJoinCore) -> 
                Some(nm, innerSourcePat, innerSource, Some keySelectors, None, mJoinCore)

        // groupJoin innerSourcePat in innerSource on (keySelector1 = keySelector2) into intoPat
        | GroupJoinExpr (nm, innerSourcePat, innerSource, keySelectors, intoPat, mGroupJoinCore) -> 
                Some(nm, innerSourcePat, innerSource, Some keySelectors, Some intoPat, mGroupJoinCore)

        // zip intoPat in secondSource 
        | InExpr (SynExpr.App (_, _, CustomOpId customOperationIsLikeZip nm, ExprAsPat secondSourcePat, _), secondSource, mZipCore) -> 
                Some(nm, secondSourcePat, secondSource, None, None, mZipCore)

        // zip (without secondSource or in - gives error)
        | CustomOpId customOperationIsLikeZip nm -> 
                errorR(Error(FSComp.SR.tcOperatorIncorrectSyntax(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
                Some(nm, arbPat e.Range, arbExpr("_secondSource", e.Range), None, None, e.Range)

        // zip secondSource (without in - gives error)
        | SynExpr.App (_, _, CustomOpId customOperationIsLikeZip nm, ExprAsPat secondSourcePat, mZipCore) -> 
                errorR(Error(FSComp.SR.tcOperatorIncorrectSyntax(nm.idText, Option.get (customOpUsageText nm)), mZipCore))
                Some(nm, secondSourcePat, arbExpr("_innerSource", e.Range), None, None, mZipCore)

        | _ -> 
            None

    let (|ForEachThenJoinOrGroupJoinOrZipClause|_|) strict e = 
        match e with 
        | ForEachThen (isFromSource, firstSourcePat, firstSource, JoinOrGroupJoinOrZipClause(nm, secondSourcePat, secondSource, keySelectorsOpt, pat3opt, mOpCore), innerComp) 
            when 
               (let _firstSourceSimplePats, later1 = 
                    use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                    SimplePatsOfPat cenv.synArgNameGenerator firstSourcePat 
                Option.isNone later1)

             -> Some (isFromSource, firstSourcePat, firstSource, nm, secondSourcePat, secondSource, keySelectorsOpt, pat3opt, mOpCore, innerComp)

        | JoinOrGroupJoinOrZipClause(nm, pat2, expr2, expr3, pat3opt, mOpCore) when strict -> 
            errorR(Error(FSComp.SR.tcBinaryOperatorRequiresBody(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
            Some (true, arbPat e.Range, arbExpr("_outerSource", e.Range), nm, pat2, expr2, expr3, pat3opt, mOpCore, arbExpr("_innerComp", e.Range))

        | _ -> 
            None

    let (|StripApps|) e = 
        let rec strip e = 
            match e with 
            | SynExpr.FromParseError (SynExpr.App (_, _, f, arg, _), _)
            | SynExpr.App (_, _, f, arg, _) -> 
                let g, acc = strip f 
                g, (arg :: acc) 
            | _ -> e, []
        let g, acc = strip e
        g, List.rev acc

    let (|OptionalIntoSuffix|) e = 
        match e with 
        | IntoSuffix (body, intoWordRange, optInfo) -> (body, Some (intoWordRange, optInfo))
        | body -> (body, None)

    let (|CustomOperationClause|_|) e = 
        match e with 
        | OptionalIntoSuffix(StripApps(SingleIdent nm, _) as core, optInto) when isCustomOperation nm ->  
            // Now we know we have a custom operation, commit the name resolution
            let optIntoInfo = 
                match optInto with 
                | Some (intoWordRange, optInfo) -> 
                    let item = Item.CustomOperation ("into", (fun () -> None), None)
                    CallNameResolutionSink cenv.tcSink (intoWordRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.eAccessRights)
                    Some optInfo
                | None -> None

            Some (nm, Option.get (tryGetDataForCustomOperation nm), core, core.Range, optIntoInfo)
        | _ -> None

    let mkSynLambda p e m = SynExpr.Lambda (false, false, p, e, None, m, SynExprLambdaTrivia.Zero)

    let mkExprForVarSpace m (patvs: Val list) = 
        match patvs with 
        | [] -> SynExpr.Const (SynConst.Unit, m)
        | [v] -> SynExpr.Ident v.Id
        | vs -> SynExpr.Tuple (false, (vs |> List.map (fun v -> SynExpr.Ident v.Id)), [], m)  

    let mkSimplePatForVarSpace m (patvs: Val list) = 
        let spats = 
            match patvs with 
            | [] -> []
            | [v] -> [mkSynSimplePatVar false v.Id]
            | vs -> vs |> List.map (fun v -> mkSynSimplePatVar false v.Id)
        SynSimplePats.SimplePats (spats, m)

    let mkPatForVarSpace m (patvs: Val list) = 
        match patvs with 
        | [] -> SynPat.Const (SynConst.Unit, m)
        | [v] -> mkSynPatVar None v.Id
        | vs -> SynPat.Tuple(false, (vs |> List.map (fun x -> mkSynPatVar None x.Id)), m)

    let (|OptionalSequential|) e = 
        match e with 
        | SynExpr.Sequential (_sp, true, dataComp1, dataComp2, _) -> (dataComp1, Some dataComp2)
        | _ -> (e, None)

    // "cexpr; cexpr" is treated as builder.Combine(cexpr1, cexpr1)
    // This is not pretty - we have to decide which range markers we use for the calls to Combine and Delay
    // NOTE: we should probably suppress these sequence points altogether
    let rangeForCombine innerComp1 = 
        let m =
            match innerComp1 with 
            | SynExpr.IfThenElse (trivia={ IfToThenRange = mIfToThen }) -> mIfToThen
            | SynExpr.Match (matchDebugPoint=DebugPointAtBinding.Yes mMatch) -> mMatch
            | SynExpr.TryWith (trivia={ TryKeyword = mTry }) -> mTry
            | SynExpr.TryFinally (trivia={ TryKeyword = mTry })  -> mTry
            | SynExpr.For (forDebugPoint=DebugPointAtFor.Yes mBind) -> mBind
            | SynExpr.ForEach (forDebugPoint=DebugPointAtFor.Yes mBind) -> mBind
            | SynExpr.While (whileDebugPoint=DebugPointAtWhile.Yes mWhile) -> mWhile
            | _ -> innerComp1.Range

        m.NoteSourceConstruct(NotedSourceConstruct.Combine)

    // Check for 'where x > y', 'select x, y' and other mis-applications of infix operators, give a good error message, and return a flag
    let checkForBinaryApp comp = 
        match comp with 
        | StripApps(SingleIdent nm, [StripApps(SingleIdent nm2, args); arg2]) when 
                  IsMangledInfixOperator nm.idText && 
                  (match tryExpectedArgCountForCustomOperator nm2 with Some n -> n > 0 | _ -> false) &&
                  not (List.isEmpty args) -> 
            let estimatedRangeOfIntendedLeftAndRightArguments = unionRanges (List.last args).Range arg2.Range
            errorR(Error(FSComp.SR.tcUnrecognizedQueryBinaryOperator(), estimatedRangeOfIntendedLeftAndRightArguments))
            true
        | SynExpr.Tuple (false, StripApps(SingleIdent nm2, args) :: _, _, m) when 
                  (match tryExpectedArgCountForCustomOperator nm2 with Some n -> n > 0 | _ -> false) &&
                  not (List.isEmpty args) -> 
            let estimatedRangeOfIntendedLeftAndRightArguments = unionRanges (List.last args).Range m.EndRange
            errorR(Error(FSComp.SR.tcUnrecognizedQueryBinaryOperator(), estimatedRangeOfIntendedLeftAndRightArguments))
            true
        | _ ->  
            false
                    
    let addVarsToVarSpace (varSpace: LazyWithContext<Val list * TcEnv, range>) f = 
        LazyWithContext.Create
            ((fun m ->
                  let (patvs: Val list, env) = varSpace.Force m 
                  let vs, envinner = f m env 
                  let patvs = List.append patvs (vs |> List.filter (fun v -> not (patvs |> List.exists (fun v2 -> v.LogicalName = v2.LogicalName))))
                  patvs, envinner), 
              id)

    // Flag that a debug point should get emitted prior to both the evaluation of 'rhsExpr' and the call to Using
    let addBindDebugPoint spBind e =
        match spBind with
        | DebugPointAtBinding.Yes m ->
            SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, e)
        | _ -> e

    let emptyVarSpace = LazyWithContext.NotLazy ([], env)

    // If there are no 'yield' in the computation expression, and the builder supports 'Yield',
    // then allow the type-directed rule interpreting non-unit-typed expressions in statement
    // positions as 'yield'.  'yield!' may be present in the computation expression.
    let enableImplicitYield =
        cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield
        && (hasMethInfo "Yield" && hasMethInfo "Combine"  && hasMethInfo "Delay" && YieldFree cenv comp)

    // q              - a flag indicating if custom operators are allowed. They are not allowed inside try/with, try/finally, if/then/else etc.
    // varSpace       - a lazy data structure indicating the variables bound so far in the overall computation
    // comp           - the computation expression being analyzed
    // translatedCtxt - represents the translation of the context in which the computation expression 'comp' occurs, up to a
    //                  hole to be filled by (part of) the results of translating 'comp'.
    let rec tryTrans firstTry q varSpace comp translatedCtxt =
        // Guard the stack for deeply nested computation expressions
        cenv.stackGuard.Guard <| fun () ->

        match comp with 

        // for firstSourcePat in firstSource do 
        // join secondSourcePat in expr2 on (expr3 = expr4)
        // ...
        //    --> 
        // join expr1 expr2 (fun firstSourcePat -> expr3) (fun secondSourcePat -> expr4) (fun firstSourcePat secondSourcePat -> ...)

        // for firstSourcePat in firstSource do 
        // groupJoin secondSourcePat in expr2 on (expr3 = expr4) into groupPat
        // ...
        //    --> 
        // groupJoin expr1 expr2 (fun firstSourcePat -> expr3) (fun secondSourcePat -> expr4) (fun firstSourcePat groupPat -> ...)

        // for firstSourcePat in firstSource do 
        // zip secondSource into secondSourcePat
        // ...
        //    --> 
        // zip expr1 expr2 (fun pat1 pat3 -> ...)
        | ForEachThenJoinOrGroupJoinOrZipClause true (isFromSource, firstSourcePat, firstSource, nm, secondSourcePat, secondSource, keySelectorsOpt, secondResultPatOpt, mOpCore, innerComp) -> 

            if q = CustomOperationsMode.Denied then error(Error(FSComp.SR.tcCustomOperationMayNotBeUsedHere(), nm.idRange))
            let firstSource = mkSourceExprConditional isFromSource firstSource
            let secondSource = mkSourceExpr secondSource

            // Add the variables to the variable space, on demand
            let varSpaceWithFirstVars = 
                addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                        let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (firstSourcePat, None)
                        vspecs, envinner)

            let varSpaceWithSecondVars = 
                addVarsToVarSpace varSpaceWithFirstVars (fun _mCustomOp env -> 
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                        let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (secondSourcePat, None)
                        vspecs, envinner)

            let varSpaceWithGroupJoinVars = 
                match secondResultPatOpt with 
                | Some pat3 -> 
                    addVarsToVarSpace varSpaceWithFirstVars (fun _mCustomOp env -> 
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                        let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (pat3, None)
                        vspecs, envinner)
                | None -> varSpace

            let firstSourceSimplePats, later1 = SimplePatsOfPat cenv.synArgNameGenerator firstSourcePat 
            let secondSourceSimplePats, later2 = SimplePatsOfPat cenv.synArgNameGenerator secondSourcePat

            if Option.isSome later1 then errorR (Error (FSComp.SR.tcJoinMustUseSimplePattern(nm.idText), firstSourcePat.Range))
            if Option.isSome later2 then errorR (Error (FSComp.SR.tcJoinMustUseSimplePattern(nm.idText), secondSourcePat.Range))

              // check 'join' or 'groupJoin' or 'zip' is permitted for this builder
            match tryGetDataForCustomOperation nm with 
            | None -> error(Error(FSComp.SR.tcMissingCustomOperation(nm.idText), nm.idRange))
            | Some opDatas -> 
            let opName, _, _, _, _, _, _, _, methInfo = opDatas[0]

            // Record the resolution of the custom operation for posterity
            let item = Item.CustomOperation (opName, (fun () -> customOpUsageText nm), Some methInfo)

            // FUTURE: consider whether we can do better than emptyTyparInst here, in order to display instantiations
            // of type variables in the quick info provided in the IDE.
            CallNameResolutionSink cenv.tcSink (nm.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.eAccessRights)

            let mkJoinExpr keySelector1 keySelector2 innerPat e = 
                let mSynthetic = mOpCore.MakeSynthetic()
                mkSynCall methInfo.DisplayName mOpCore
                        [ firstSource
                          secondSource
                          (mkSynLambda firstSourceSimplePats keySelector1 mSynthetic)
                          (mkSynLambda secondSourceSimplePats keySelector2 mSynthetic)
                          (mkSynLambda firstSourceSimplePats (mkSynLambda innerPat e mSynthetic) mSynthetic) ]

            let mkZipExpr e = 
                let mSynthetic = mOpCore.MakeSynthetic()
                mkSynCall methInfo.DisplayName mOpCore
                        [ firstSource
                          secondSource
                          (mkSynLambda firstSourceSimplePats (mkSynLambda secondSourceSimplePats e mSynthetic) mSynthetic) ]
            
            // wraps given expression into sequence with result produced by arbExpr so result will look like: 
            // l; SynExpr.ArbitraryAfterError (...)
            // this allows to handle cases like 'on (a > b)' // '>' is not permitted as correct join relation
            // after wrapping a and b can still be typechecked (so we'll have correct completion inside 'on' part)
            // but presence of SynExpr.ArbitraryAfterError allows to avoid errors about incompatible types in cases like
            // query { 
            //      for a in [1] do
            //      join b in [""] on (a > b)
            //      }
            // if we typecheck raw 'a' and 'b' then we'll end up with 2 errors:
            // 1. incorrect join relation
            // 2. incompatible types: int and string
            // with SynExpr.ArbitraryAfterError we have only first one
            let wrapInArbErrSequence l caption = 
                SynExpr.Sequential (DebugPointAtSequential.SuppressNeither, true, l, (arbExpr(caption, l.Range.EndRange)), l.Range)

            let mkOverallExprGivenVarSpaceExpr, varSpaceInner =
                let isNullableOp opId =
                    match DecompileOpName opId with "?=" | "=?" | "?=?" -> true | _ -> false
                match secondResultPatOpt, keySelectorsOpt with 
                // groupJoin 
                | Some secondResultPat, Some relExpr when customOperationIsLikeGroupJoin nm -> 
                    let secondResultSimplePats, later3 = SimplePatsOfPat cenv.synArgNameGenerator secondResultPat
                    if Option.isSome later3 then errorR (Error (FSComp.SR.tcJoinMustUseSimplePattern(nm.idText), secondResultPat.Range))
                    match relExpr with 
                    | JoinRelation cenv env (keySelector1, keySelector2) -> 
                        mkJoinExpr keySelector1 keySelector2 secondResultSimplePats, varSpaceWithGroupJoinVars
                    | BinOpExpr (opId, l, r) ->
                        if isNullableOp opId.idText then 
                            // When we cannot resolve NullableOps, recommend the relevant namespace to be added
                            errorR(Error(FSComp.SR.cannotResolveNullableOperators(DecompileOpName opId.idText), relExpr.Range))
                        else
                            errorR(Error(FSComp.SR.tcInvalidRelationInJoin(nm.idText), relExpr.Range))
                        let l = wrapInArbErrSequence l "_keySelector1"
                        let r = wrapInArbErrSequence r "_keySelector2"
                        // this is not correct JoinRelation but it is still binary operation
                        // we've already reported error now we can use operands of binary operation as join components
                        mkJoinExpr l r secondResultSimplePats, varSpaceWithGroupJoinVars
                    | _ ->
                        errorR(Error(FSComp.SR.tcInvalidRelationInJoin(nm.idText), relExpr.Range))
                        // since the shape of relExpr doesn't match our expectations (JoinRelation) 
                        // then we assume that this is l.h.s. of the join relation 
                        // so typechecker will treat relExpr as body of outerKeySelector lambda parameter in GroupJoin method
                        mkJoinExpr relExpr (arbExpr("_keySelector2", relExpr.Range)) secondResultSimplePats, varSpaceWithGroupJoinVars
                        
                | None, Some relExpr when customOperationIsLikeJoin nm -> 
                    match relExpr with 
                    | JoinRelation cenv env (keySelector1, keySelector2) -> 
                        mkJoinExpr keySelector1 keySelector2 secondSourceSimplePats, varSpaceWithSecondVars
                    | BinOpExpr (opId, l, r) ->
                        if isNullableOp opId.idText then
                            // When we cannot resolve NullableOps, recommend the relevant namespace to be added
                            errorR(Error(FSComp.SR.cannotResolveNullableOperators(DecompileOpName opId.idText), relExpr.Range))
                        else
                            errorR(Error(FSComp.SR.tcInvalidRelationInJoin(nm.idText), relExpr.Range))
                        // this is not correct JoinRelation but it is still binary operation
                        // we've already reported error now we can use operands of binary operation as join components
                        let l = wrapInArbErrSequence l "_keySelector1"
                        let r = wrapInArbErrSequence r "_keySelector2"
                        mkJoinExpr l r secondSourceSimplePats, varSpaceWithGroupJoinVars
                    | _ -> 
                        errorR(Error(FSComp.SR.tcInvalidRelationInJoin(nm.idText), relExpr.Range))
                        // since the shape of relExpr doesn't match our expectations (JoinRelation) 
                        // then we assume that this is l.h.s. of the join relation 
                        // so typechecker will treat relExpr as body of outerKeySelector lambda parameter in Join method
                        mkJoinExpr relExpr (arbExpr("_keySelector2", relExpr.Range)) secondSourceSimplePats, varSpaceWithGroupJoinVars

                | None, None when customOperationIsLikeZip nm -> 
                    mkZipExpr, varSpaceWithSecondVars

                | _ -> 
                    assert false
                    failwith "unreachable"


            // Case from C# spec: A query expression with a join clause with an into followed by something other than a select clause
            // Case from C# spec: A query expression with a join clause without an into followed by something other than a select clause
            let valsInner, _env = varSpaceInner.Force mOpCore
            let varSpaceExpr = mkExprForVarSpace mOpCore valsInner
            let varSpacePat = mkPatForVarSpace mOpCore valsInner
            let joinExpr = mkOverallExprGivenVarSpaceExpr varSpaceExpr
            let consumingExpr = SynExpr.ForEach (DebugPointAtFor.No, DebugPointAtInOrTo.No, SeqExprOnly false, false, varSpacePat, joinExpr, innerComp, mOpCore)
            Some (trans CompExprTranslationPass.Initial q varSpaceInner consumingExpr translatedCtxt)


        | SynExpr.ForEach (spFor, spIn, SeqExprOnly _seqExprOnly, isFromSource, pat, sourceExpr, innerComp, _mEntireForEach) -> 
            let sourceExpr =
                match RewriteRangeExpr sourceExpr with
                | Some e -> e
                | None -> sourceExpr
            let wrappedSourceExpr = mkSourceExprConditional isFromSource sourceExpr

            let mFor =
                match spFor with
                | DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For)
                | DebugPointAtFor.No -> pat.Range

            // For computation expressions, 'in' or 'to' is hit on each MoveNext.   
            // To support this a named debug point for the "in" keyword is available to inlined code.
            match spIn with
            | DebugPointAtInOrTo.Yes mIn ->
                cenv.namedDebugPointsForInlinedCode[{Range=mFor; Name="ForLoop.InOrToKeyword"}] <- mIn
            | _ -> ()

            let mPat = pat.Range

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mFor ad "For" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("For"), mFor))

            // Add the variables to the query variable space, on demand
            let varSpace = 
                addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                    use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                    let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (pat, None) 
                    vspecs, envinner)

            Some (trans CompExprTranslationPass.Initial q varSpace innerComp
                    (fun innerCompR -> 

                        let forCall = 
                            mkSynCall "For" mFor [wrappedSourceExpr; SynExpr.MatchLambda (false, mPat, [SynMatchClause(pat, None, innerCompR, mPat, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, mFor) ]

                        let forCall =
                            match spFor with
                            | DebugPointAtFor.Yes _ ->
                                SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mFor, false, forCall)
                            | DebugPointAtFor.No -> forCall

                        translatedCtxt forCall))

        | SynExpr.For (forDebugPoint=spFor; toDebugPoint=spTo; ident=id; identBody=start; direction=dir; toBody=finish; doBody=innerComp; range=m) ->
            let mFor = match spFor with DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For) | _ -> m

            if isQuery then errorR(Error(FSComp.SR.tcNoIntegerForLoopInQuery(), mFor))

            let reduced = elimFastIntegerForLoop (spFor, spTo, id, start, dir, finish, innerComp, m)
            Some (trans CompExprTranslationPass.Initial q varSpace reduced translatedCtxt )

        | SynExpr.While (spWhile, guardExpr, innerComp, _) -> 
            let mGuard = guardExpr.Range
            let mWhile = match spWhile with DebugPointAtWhile.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.While) | _ -> mGuard

            if isQuery then error(Error(FSComp.SR.tcNoWhileInQuery(), mWhile))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mWhile ad "While" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("While"), mWhile))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mWhile ad "Delay" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Delay"), mWhile))

            // 'while' is hit just before each time the guard is called
            let guardExpr = 
                match spWhile with
                | DebugPointAtWhile.Yes _ ->
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mWhile, false, guardExpr)
                | DebugPointAtWhile.No -> guardExpr

            Some(trans CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill ->
                translatedCtxt 
                    (mkSynCall "While" mWhile 
                        [ mkSynDelay2 guardExpr; 
                          mkSynCall "Delay" mWhile [mkSynDelay innerComp.Range holeFill]])) )

        | SynExpr.TryFinally (innerComp, unwindExpr, _mTryToLast, spTry, spFinally, trivia) ->

            let mTry = match spTry with DebugPointAtTry.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Try) | _ -> trivia.TryKeyword
            let mFinally = match spFinally with DebugPointAtFinally.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Finally) | _ -> trivia.FinallyKeyword

            // Put down a debug point for the 'finally'
            let unwindExpr2 =
                match spFinally with
                | DebugPointAtFinally.Yes _ ->
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mFinally, true, unwindExpr)
                | DebugPointAtFinally.No -> unwindExpr

            if isQuery then error(Error(FSComp.SR.tcNoTryFinallyInQuery(), mTry))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mTry ad "TryFinally" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("TryFinally"), mTry))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mTry ad "Delay" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Delay"), mTry))

            let innerExpr = transNoQueryOps innerComp

            let innerExpr =
                match spTry with
                | DebugPointAtTry.Yes _ ->
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mTry, true, innerExpr)
                | _ -> innerExpr
                
            Some (translatedCtxt 
                (mkSynCall "TryFinally" mTry [
                    mkSynCall "Delay" mTry [mkSynDelay innerComp.Range innerExpr]
                    mkSynDelay2 unwindExpr2]))

        | SynExpr.Paren (_, _, _, m) -> 
            error(Error(FSComp.SR.tcConstructIsAmbiguousInComputationExpression(), m))

        // In some cases the node produced by `mkSynCall "Zero" m []` may be discarded in the case
        // of implicit yields - for example "list { 1; 2 }" when each expression checks as an implicit yield.
        // If it is not discarded, the syntax node will later be checked and the existence/non-existence of the Zero method
        // will be checked/reported appropriately (though the error message won't mention computation expressions
        // like our other error messages for missing methods).
        | SynExpr.ImplicitZero m -> 
            if (not enableImplicitYield) && 
               isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "Zero" builderTy) then error(Error(FSComp.SR.tcRequireBuilderMethod("Zero"), m))
            Some (translatedCtxt (mkSynCall "Zero" m []))
            
        | OptionalSequential (JoinOrGroupJoinOrZipClause (_, _, _, _, _, mClause), _) 
                      when firstTry = CompExprTranslationPass.Initial ->

            // 'join' clauses preceded by 'let' and other constructs get processed by repackaging with a 'for' loop.
            let patvs, _env = varSpace.Force comp.Range
            let varSpaceExpr = mkExprForVarSpace mClause patvs
            let varSpacePat = mkPatForVarSpace mClause patvs
            
            let dataCompPrior = 
                translatedCtxt (transNoQueryOps (SynExpr.YieldOrReturn ((true, false), varSpaceExpr, mClause)))

            // Rebind using for ... 
            let rebind = 
                SynExpr.ForEach (DebugPointAtFor.No, DebugPointAtInOrTo.No, SeqExprOnly false, false, varSpacePat, dataCompPrior, comp, comp.Range)
                    
            // Retry with the 'for' loop packaging. Set firstTry=false just in case 'join' processing fails
            tryTrans CompExprTranslationPass.Subsequent q varSpace rebind id


        | OptionalSequential (CustomOperationClause (nm, _, opExpr, mClause, _), _) -> 

            if q = CustomOperationsMode.Denied then error(Error(FSComp.SR.tcCustomOperationMayNotBeUsedHere(), opExpr.Range))

            let patvs, _env = varSpace.Force comp.Range
            let varSpaceExpr = mkExprForVarSpace mClause patvs
            
            let dataCompPriorToOp = 
                let isYield = not (customOperationMaintainsVarSpaceUsingBind nm)
                translatedCtxt (transNoQueryOps (SynExpr.YieldOrReturn ((isYield, false), varSpaceExpr, mClause)))
            
            // Now run the consumeCustomOpClauses
            Some (consumeCustomOpClauses q varSpace dataCompPriorToOp comp false mClause)

        | SynExpr.Sequential (sp, true, innerComp1, innerComp2, m) -> 

            // Check for 'where x > y' and other mis-applications of infix operators. If detected, give a good error message, and just ignore innerComp1
          if isQuery && checkForBinaryApp innerComp1 then 
            Some (trans CompExprTranslationPass.Initial q varSpace innerComp2 translatedCtxt)

          else
            
            if isQuery && not(innerComp1.IsArbExprAndThusAlreadyReportedError) then 
                match innerComp1 with 
                | SynExpr.JoinIn _ -> () // an error will be reported later when we process innerComp1 as a sequential
                | _ -> errorR(Error(FSComp.SR.tcUnrecognizedQueryOperator(), innerComp1.RangeOfFirstPortion))

            match tryTrans CompExprTranslationPass.Initial CustomOperationsMode.Denied varSpace innerComp1 id with
            | Some c -> 
                // "cexpr; cexpr" is treated as builder.Combine(cexpr1, cexpr1)
                let m1 = rangeForCombine innerComp1

                if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "Combine" builderTy) then
                    error(Error(FSComp.SR.tcRequireBuilderMethod("Combine"), m))

                if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "Delay" builderTy) then
                    error(Error(FSComp.SR.tcRequireBuilderMethod("Delay"), m))

                let combineCall = mkSynCall "Combine" m1 [c; mkSynCall "Delay" m1 [mkSynDelay innerComp2.Range (transNoQueryOps innerComp2)]]
                
                Some (translatedCtxt combineCall)

            | None -> 
                // "do! expr; cexpr" is treated as { let! () = expr in cexpr }
                match innerComp1 with 
                | SynExpr.DoBang (rhsExpr, m) -> 
                    let sp = 
                        match sp with 
                        | DebugPointAtSequential.SuppressExpr -> DebugPointAtBinding.NoneAtDo 
                        | DebugPointAtSequential.SuppressBoth -> DebugPointAtBinding.NoneAtDo 
                        | DebugPointAtSequential.SuppressStmt -> DebugPointAtBinding.Yes m
                        | DebugPointAtSequential.SuppressNeither -> DebugPointAtBinding.Yes m
                    Some(trans CompExprTranslationPass.Initial q varSpace (SynExpr.LetOrUseBang (sp, false, true, SynPat.Const(SynConst.Unit, rhsExpr.Range), rhsExpr, [], innerComp2, m, SynExprLetOrUseBangTrivia.Zero)) translatedCtxt)

                // "expr; cexpr" is treated as sequential execution
                | _ -> 
                    Some (trans CompExprTranslationPass.Initial q varSpace innerComp2 (fun holeFill ->
                        let fillExpr = 
                            if enableImplicitYield then
                                // When implicit yields are enabled, then if the 'innerComp1' checks as type
                                // 'unit' we interpret the expression as a sequential, and when it doesn't
                                // have type 'unit' we interpret it as a 'Yield + Combine'.
                                let combineExpr = 
                                    let m1 = rangeForCombine innerComp1
                                    let implicitYieldExpr = mkSynCall "Yield" comp.Range [innerComp1]
                                    mkSynCall "Combine" m1 [implicitYieldExpr; mkSynCall "Delay" m1 [mkSynDelay holeFill.Range holeFill]]
                                SynExpr.SequentialOrImplicitYield(sp, innerComp1, holeFill, combineExpr, m)
                            else
                                SynExpr.Sequential(sp, true, innerComp1, holeFill, m)
                        translatedCtxt fillExpr))

        | SynExpr.IfThenElse (guardExpr, thenComp, elseCompOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia) ->
            match elseCompOpt with 
            | Some elseComp -> 
                if isQuery then error(Error(FSComp.SR.tcIfThenElseMayNotBeUsedWithinQueries(), trivia.IfToThenRange))
                Some (translatedCtxt (SynExpr.IfThenElse (guardExpr, transNoQueryOps thenComp, Some(transNoQueryOps elseComp), spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia)))
            | None -> 
                let elseComp = 
                    if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env trivia.IfToThenRange ad "Zero" builderTy) then
                        error(Error(FSComp.SR.tcRequireBuilderMethod("Zero"), trivia.IfToThenRange))
                    mkSynCall "Zero" trivia.IfToThenRange []
                Some (trans CompExprTranslationPass.Initial q varSpace thenComp (fun holeFill -> translatedCtxt (SynExpr.IfThenElse (guardExpr, holeFill, Some elseComp, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia))))

        // 'let binds in expr'
        | SynExpr.LetOrUse (isRec, false, binds, innerComp, m, trivia) ->

            // For 'query' check immediately
            if isQuery then
                match (List.map (BindingNormalization.NormalizeBinding ValOrMemberBinding cenv env) binds) with 
                | [NormalizedBinding(_, SynBindingKind.Normal, (*inline*)false, (*mutable*)false, _, _, _, _, _, _, _, _)] when not isRec -> 
                    ()
                | normalizedBindings -> 
                    let failAt m = error(Error(FSComp.SR.tcNonSimpleLetBindingInQuery(), m))
                    match normalizedBindings with 
                    | NormalizedBinding(_, _, _, _, _, _, _, _, _, _, mBinding, _) :: _ -> failAt mBinding 
                    | _ -> failAt m

            // Add the variables to the query variable space, on demand
            let varSpace = 
                addVarsToVarSpace varSpace (fun mQueryOp env -> 
                    // Normalize the bindings before detecting the bound variables
                    match (List.map (BindingNormalization.NormalizeBinding ValOrMemberBinding cenv env) binds) with 
                    | [NormalizedBinding(_vis, SynBindingKind.Normal, false, false, _, _, _, _, pat, _, _, _)] -> 
                        // successful case
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                        let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (pat, None) 
                        vspecs, envinner
                    | _ -> 
                        // error case
                        error(Error(FSComp.SR.tcCustomOperationMayNotBeUsedInConjunctionWithNonSimpleLetBindings(), mQueryOp)))

            Some (trans CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill -> translatedCtxt (SynExpr.LetOrUse (isRec, false, binds, holeFill, m, trivia))))

        // 'use x = expr in expr'
        | SynExpr.LetOrUse (isUse=true; bindings=[SynBinding (kind=SynBindingKind.Normal; headPat=pat; expr=rhsExpr; debugPoint=spBind)]; body=innerComp) ->
            let mBind = match spBind with DebugPointAtBinding.Yes m -> m | _ -> rhsExpr.Range
            if isQuery then error(Error(FSComp.SR.tcUseMayNotBeUsedInQueries(), mBind))
            let innerCompRange = innerComp.Range
            let consumeExpr = SynExpr.MatchLambda(false, innerCompRange, [SynMatchClause(pat, None, transNoQueryOps innerComp, innerCompRange, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, innerCompRange)

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad "Using" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Using"), mBind))

            Some (translatedCtxt (mkSynCall "Using" mBind [rhsExpr; consumeExpr ]) |> addBindDebugPoint spBind)

        // 'let! pat = expr in expr' 
        //    --> build.Bind(e1, (fun _argN -> match _argN with pat -> expr))
        //  or
        //    --> build.BindReturn(e1, (fun _argN -> match _argN with pat -> expr-without-return))
        | SynExpr.LetOrUseBang (bindDebugPoint=spBind; isUse=false; isFromSource=isFromSource; pat=pat; rhs=rhsExpr; andBangs=[]; body=innerComp) -> 

            let mBind = match spBind with DebugPointAtBinding.Yes m -> m | _ -> rhsExpr.Range
            if isQuery then error(Error(FSComp.SR.tcBindMayNotBeUsedInQueries(), mBind))
                
            // Add the variables to the query variable space, on demand
            let varSpace = 
                addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                        let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (pat, None) 
                        vspecs, envinner)

            let rhsExpr = mkSourceExprConditional isFromSource rhsExpr
            Some (transBind q varSpace mBind (addBindDebugPoint spBind) "Bind" [rhsExpr] pat innerComp translatedCtxt)

        // 'use! pat = e1 in e2' --> build.Bind(e1, (function  _argN -> match _argN with pat -> build.Using(x, (fun _argN -> match _argN with pat -> e2))))
        | SynExpr.LetOrUseBang (bindDebugPoint=spBind; isUse=true; isFromSource=isFromSource; pat=SynPat.Named (ident=id; isThisVal=false) as pat; rhs=rhsExpr; andBangs=[]; body=innerComp)
        | SynExpr.LetOrUseBang (bindDebugPoint=spBind; isUse=true; isFromSource=isFromSource; pat=SynPat.ParametersOwner (namePat = SingleIdentInParametersOwnerNamePat id) as pat; rhs=rhsExpr; andBangs=[]; body=innerComp) ->

            let mBind = match spBind with DebugPointAtBinding.Yes m -> m | _ -> rhsExpr.Range
            if isQuery then error(Error(FSComp.SR.tcBindMayNotBeUsedInQueries(), mBind))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad "Using" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Using"), mBind))
            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad "Bind" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Bind"), mBind))

            let bindExpr =
                let consumeExpr = SynExpr.MatchLambda(false, mBind, [SynMatchClause(pat, None, transNoQueryOps innerComp, innerComp.Range, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, mBind)
                let consumeExpr = mkSynCall "Using" mBind [SynExpr.Ident(id); consumeExpr ]
                let consumeExpr = SynExpr.MatchLambda(false, mBind, [SynMatchClause(pat, None, consumeExpr, id.idRange, DebugPointAtTarget.No, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, mBind)
                let rhsExpr = mkSourceExprConditional isFromSource rhsExpr
                mkSynCall "Bind" mBind [rhsExpr; consumeExpr]
                |> addBindDebugPoint spBind

            Some(translatedCtxt bindExpr)

        // 'use! pat = e1 ... in e2' where 'pat' is not a simple name --> error
        | SynExpr.LetOrUseBang (isUse=true; pat=pat; andBangs=andBangs) ->
            if isNil andBangs then
                error(Error(FSComp.SR.tcInvalidUseBangBinding(), pat.Range))
            else
                error(Error(FSComp.SR.tcInvalidUseBangBindingNoAndBangs(), comp.Range))

        // 'let! pat1 = expr1 and! pat2 = expr2 in ...' -->
        //     build.BindN(expr1, expr2, ...)
        // or
        //     build.BindNReturn(expr1, expr2, ...)
        // or
        //     build.Bind(build.MergeSources(expr1, expr2), ...)
        | SynExpr.LetOrUseBang(bindDebugPoint=spBind; isUse=false; isFromSource=isFromSource; pat=letPat; rhs=letRhsExpr; andBangs=andBangBindings; body=innerComp; range=letBindRange) ->
            if not (cenv.g.langVersion.SupportsFeature LanguageFeature.AndBang) then
                error(Error(FSComp.SR.tcAndBangNotSupported(), comp.Range))

            if isQuery then
                error(Error(FSComp.SR.tcBindMayNotBeUsedInQueries(), letBindRange))

            let mBind = match spBind with DebugPointAtBinding.Yes m -> m | _ -> letRhsExpr.Range
            let sources = (letRhsExpr :: [for SynExprAndBang(body=andExpr) in andBangBindings -> andExpr ]) |> List.map (mkSourceExprConditional isFromSource)
            let pats = letPat :: [for SynExprAndBang(pat = andPat) in andBangBindings -> andPat ]
            let sourcesRange = sources |> List.map (fun e -> e.Range) |> List.reduce unionRanges

            let numSources = sources.Length
            let bindReturnNName = "Bind"+string numSources+"Return"
            let bindNName = "Bind"+string numSources

            // Check if this is a Bind2Return etc.
            let hasBindReturnN = not (isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad bindReturnNName builderTy))
            if hasBindReturnN && Option.isSome (convertSimpleReturnToExpr varSpace innerComp) then 
                let consumePat = SynPat.Tuple(false, pats, letPat.Range)

                // Add the variables to the query variable space, on demand
                let varSpace = 
                    addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                            use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                            let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (consumePat, None) 
                            vspecs, envinner)

                Some (transBind q varSpace mBind (addBindDebugPoint spBind) bindNName sources consumePat innerComp translatedCtxt)

            else

                // Check if this is a Bind2 etc.
                let hasBindN = not (isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad bindNName builderTy))
                if hasBindN then 
                    let consumePat = SynPat.Tuple(false, pats, letPat.Range)

                    // Add the variables to the query variable space, on demand
                    let varSpace = 
                        addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                                use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                                let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (consumePat, None) 
                                vspecs, envinner)

                    Some (transBind q varSpace mBind (addBindDebugPoint spBind) bindNName sources consumePat innerComp translatedCtxt)
                else

                    // Look for the maximum supported MergeSources, MergeSources3, ... 
                    let mkMergeSourcesName n = if n = 2 then "MergeSources" else "MergeSources"+(string n)

                    let maxMergeSources =
                        let rec loop (n: int) = 
                            let mergeSourcesName = mkMergeSourcesName n
                            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad mergeSourcesName builderTy) then
                                (n-1)
                            else
                                loop (n+1)
                        loop 2

                    if maxMergeSources = 1 then error(Error(FSComp.SR.tcRequireMergeSourcesOrBindN(bindNName), mBind))

                    let rec mergeSources (sourcesAndPats: (SynExpr * SynPat) list) = 
                        let numSourcesAndPats = sourcesAndPats.Length
                        assert (numSourcesAndPats <> 0)
                        if numSourcesAndPats = 1 then 
                            sourcesAndPats[0]

                        elif numSourcesAndPats <= maxMergeSources then 

                            // Call MergeSources2(e1, e2), MergeSources3(e1, e2, e3) etc
                            let mergeSourcesName = mkMergeSourcesName numSourcesAndPats

                            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad mergeSourcesName builderTy) then
                                error(Error(FSComp.SR.tcRequireMergeSourcesOrBindN(bindNName), mBind))

                            let source = mkSynCall mergeSourcesName sourcesRange (List.map fst sourcesAndPats)
                            let pat = SynPat.Tuple(false, List.map snd sourcesAndPats, letPat.Range)
                            source, pat

                        else

                            // Call MergeSourcesMax(e1, e2, e3, e4, (...))
                            let nowSourcesAndPats, laterSourcesAndPats = List.splitAt (maxMergeSources - 1) sourcesAndPats
                            let mergeSourcesName = mkMergeSourcesName maxMergeSources

                            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBind ad mergeSourcesName builderTy) then
                                error(Error(FSComp.SR.tcRequireMergeSourcesOrBindN(bindNName), mBind))

                            let laterSource, laterPat = mergeSources laterSourcesAndPats
                            let source = mkSynCall mergeSourcesName sourcesRange (List.map fst nowSourcesAndPats @ [laterSource])
                            let pat = SynPat.Tuple(false, List.map snd nowSourcesAndPats @ [laterPat], letPat.Range)
                            source, pat

                    let mergedSources, consumePat = mergeSources (List.zip sources pats)
                
                    // Add the variables to the query variable space, on demand
                    let varSpace = 
                        addVarsToVarSpace varSpace (fun _mCustomOp env -> 
                                use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink
                                let _, _, vspecs, envinner, _ = TcMatchPattern cenv (NewInferenceType g) env tpenv (consumePat, None) 
                                vspecs, envinner)

                    // Build the 'Bind' call
                    Some (transBind q varSpace mBind (addBindDebugPoint spBind) "Bind" [mergedSources] consumePat innerComp translatedCtxt)

        | SynExpr.Match (spMatch, expr, clauses, m, trivia) ->
            if isQuery then error(Error(FSComp.SR.tcMatchMayNotBeUsedWithQuery(), trivia.MatchKeyword))
            let clauses = clauses |> List.map (fun (SynMatchClause(pat, cond, innerComp, patm, sp, trivia)) -> SynMatchClause(pat, cond, transNoQueryOps innerComp, patm, sp, trivia))
            Some(translatedCtxt (SynExpr.Match (spMatch, expr, clauses, m, trivia)))

        // 'match! expr with pats ...' --> build.Bind(e1, (function pats ...))
        // FUTURE: consider allowing translation to BindReturn
        | SynExpr.MatchBang (spMatch, expr, clauses, _m, trivia) ->
            let inputExpr = mkSourceExpr expr
            if isQuery then error(Error(FSComp.SR.tcMatchMayNotBeUsedWithQuery(), trivia.MatchBangKeyword))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env trivia.MatchBangKeyword ad "Bind" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Bind"), trivia.MatchBangKeyword))

            let clauses = clauses |> List.map (fun (SynMatchClause(pat, cond, innerComp, patm, sp, trivia)) -> SynMatchClause(pat, cond, transNoQueryOps innerComp, patm, sp, trivia))
            let consumeExpr = SynExpr.MatchLambda (false, trivia.MatchBangKeyword, clauses, DebugPointAtBinding.NoneAtInvisible, trivia.MatchBangKeyword)

            let callExpr =
                mkSynCall "Bind" trivia.MatchBangKeyword [inputExpr; consumeExpr]
                |> addBindDebugPoint spMatch
            
            Some(translatedCtxt callExpr)

        | SynExpr.TryWith (innerComp, clauses, mTryToLast, spTry, spWith, trivia) ->
            let mTry = match spTry with DebugPointAtTry.Yes _ -> trivia.TryKeyword.NoteSourceConstruct(NotedSourceConstruct.Try) | _ -> trivia.TryKeyword
            let spWith2 = match spWith with DebugPointAtWith.Yes _ -> DebugPointAtBinding.Yes trivia.WithKeyword | _ -> DebugPointAtBinding.NoneAtInvisible
            
            if isQuery then error(Error(FSComp.SR.tcTryWithMayNotBeUsedInQueries(), mTry))

            let clauses = clauses |> List.map (fun (SynMatchClause(pat, cond, clauseComp, patm, sp, trivia)) -> SynMatchClause(pat, cond, transNoQueryOps clauseComp, patm, sp, trivia))
            let consumeExpr = SynExpr.MatchLambda(true, mTryToLast, clauses, spWith2, mTryToLast)

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mTry ad "TryWith" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("TryWith"), mTry))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mTry ad "Delay" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("Delay"), mTry))

            let innerExpr = transNoQueryOps innerComp

            let innerExpr =
                match spTry with
                | DebugPointAtTry.Yes _ ->
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mTry, true, innerExpr)
                | _ -> innerExpr
                
            let callExpr = 
                mkSynCall "TryWith" mTry [
                    mkSynCall "Delay" mTry [mkSynDelay2 innerExpr]
                    consumeExpr
                ]

            Some(translatedCtxt callExpr)

        | SynExpr.YieldOrReturnFrom ((true, _), synYieldExpr, m) -> 
            let yieldFromExpr = mkSourceExpr synYieldExpr
            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "YieldFrom" builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod("YieldFrom"), m))

            let yieldFromCall = mkSynCall "YieldFrom" m [yieldFromExpr]

            let yieldFromCall =
                if IsControlFlowExpression synYieldExpr then
                    yieldFromCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, yieldFromCall)

            Some (translatedCtxt yieldFromCall)
  
        | SynExpr.YieldOrReturnFrom ((false, _), synReturnExpr, m) -> 
            let returnFromExpr = mkSourceExpr synReturnExpr
            if isQuery then error(Error(FSComp.SR.tcReturnMayNotBeUsedInQueries(), m))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "ReturnFrom" builderTy) then 
                error(Error(FSComp.SR.tcRequireBuilderMethod("ReturnFrom"), m))

            let returnFromCall = mkSynCall "ReturnFrom" m [returnFromExpr]

            let returnFromCall =
                if IsControlFlowExpression synReturnExpr then
                    returnFromCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, returnFromCall)

            Some (translatedCtxt returnFromCall)

        | SynExpr.YieldOrReturn ((isYield, _), synYieldOrReturnExpr, m) -> 
            let methName = (if isYield then "Yield" else "Return")
            if isQuery && not isYield then error(Error(FSComp.SR.tcReturnMayNotBeUsedInQueries(), m))

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad methName builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod(methName), m))

            let yieldOrReturnCall = mkSynCall methName m [synYieldOrReturnExpr]
                
            let yieldOrReturnCall =
                if IsControlFlowExpression synYieldOrReturnExpr then
                    yieldOrReturnCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, yieldOrReturnCall)

            Some(translatedCtxt yieldOrReturnCall)

        | _ -> None

    and consumeCustomOpClauses q (varSpace: LazyWithContext<_, _>) dataCompPrior compClausesExpr lastUsesBind mClause =

        // Substitute 'yield <var-space>' into the context

        let patvs, _env = varSpace.Force comp.Range
        let varSpaceSimplePat = mkSimplePatForVarSpace mClause patvs
        let varSpacePat = mkPatForVarSpace mClause patvs

        match compClausesExpr with 
        
        // Detect one custom operation... This clause will always match at least once...
        | OptionalSequential (CustomOperationClause (nm, opDatas, opExpr, mClause, optionalIntoPat), optionalCont) ->

            let opName, _, _, _, _, _, _, _, methInfo = opDatas[0]
            let isLikeZip = customOperationIsLikeZip nm
            let isLikeJoin = customOperationIsLikeJoin nm
            let isLikeGroupJoin = customOperationIsLikeZip nm

            // Record the resolution of the custom operation for posterity
            let item = Item.CustomOperation (opName, (fun () -> customOpUsageText nm), Some methInfo)

            // FUTURE: consider whether we can do better than emptyTyparInst here, in order to display instantiations
            // of type variables in the quick info provided in the IDE.
            CallNameResolutionSink cenv.tcSink (nm.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.eAccessRights)

            if isLikeZip || isLikeJoin || isLikeGroupJoin then
                errorR(Error(FSComp.SR.tcBinaryOperatorRequiresBody(nm.idText, Option.get (customOpUsageText nm)), nm.idRange))
                match optionalCont with 
                | None -> 
                    // we are about to drop the 'opExpr' AST on the floor. we've already reported an error. attempt to get name resolutions before dropping it
                    RecordNameAndTypeResolutions_IdeallyWithoutHavingOtherEffects cenv env tpenv opExpr
                    dataCompPrior
                | Some contExpr -> consumeCustomOpClauses q varSpace dataCompPrior contExpr lastUsesBind mClause
            else

                let maintainsVarSpace = customOperationMaintainsVarSpace nm
                let maintainsVarSpaceUsingBind = customOperationMaintainsVarSpaceUsingBind nm

                let expectedArgCount = tryExpectedArgCountForCustomOperator nm

                let dataCompAfterOp = 
                    match opExpr with 
                    | StripApps(SingleIdent nm, args) ->
                        let argCountsMatch =
                            match expectedArgCount with
                            | Some n -> n = args.Length
                            | None -> cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations
                        if argCountsMatch then
                            // Check for the [<ProjectionParameter>] attribute on each argument position
                            let args = args |> List.mapi (fun i arg -> 
                                if isCustomOperationProjectionParameter (i+1) nm then 
                                    SynExpr.Lambda (false, false, varSpaceSimplePat, arg, None, arg.Range.MakeSynthetic(), SynExprLambdaTrivia.Zero)
                                else arg)
                            mkSynCall methInfo.DisplayName mClause (dataCompPrior :: args)
                        else 
                            let expectedArgCount = defaultArg expectedArgCount 0
                            errorR(Error(FSComp.SR.tcCustomOperationHasIncorrectArgCount(nm.idText, expectedArgCount, args.Length), nm.idRange))
                            mkSynCall methInfo.DisplayName mClause ([ dataCompPrior ] @ List.init expectedArgCount (fun i -> arbExpr("_arg" + string i, mClause)))
                    | _ -> failwith "unreachable"

                match optionalCont with 
                | None -> 
                    match optionalIntoPat with 
                    | Some intoPat -> errorR(Error(FSComp.SR.tcIntoNeedsRestOfQuery(), intoPat.Range))
                    | None -> ()
                    dataCompAfterOp

                | Some contExpr -> 

                        // select a.Name into name; ...
                        // distinct into d; ...
                        //
                        // Rebind the into pattern and process the rest of the clauses
                        match optionalIntoPat with 
                        | Some intoPat -> 
                            if not (customOperationAllowsInto nm) then 
                                error(Error(FSComp.SR.tcOperatorDoesntAcceptInto(nm.idText), intoPat.Range))

                            // Rebind using either for ... or let!....
                            let rebind = 
                                if maintainsVarSpaceUsingBind then 
                                    SynExpr.LetOrUseBang (DebugPointAtBinding.NoneAtLet, false, false, intoPat, dataCompAfterOp, [], contExpr, intoPat.Range, SynExprLetOrUseBangTrivia.Zero) 
                                else 
                                    SynExpr.ForEach (DebugPointAtFor.No, DebugPointAtInOrTo.No, SeqExprOnly false, false, intoPat, dataCompAfterOp, contExpr, intoPat.Range)

                            trans CompExprTranslationPass.Initial q emptyVarSpace rebind id

                        // select a.Name; ...
                        // distinct; ...
                        //
                        // Process the rest of the clauses
                        | None -> 
                            if maintainsVarSpace || maintainsVarSpaceUsingBind then
                                consumeCustomOpClauses q varSpace dataCompAfterOp contExpr maintainsVarSpaceUsingBind mClause
                            else
                                consumeCustomOpClauses q emptyVarSpace dataCompAfterOp contExpr false mClause

        // No more custom operator clauses in compClausesExpr, but there may be clauses like join, yield etc. 
        // Bind/iterate the dataCompPrior and use compClausesExpr as the body.
        | _ -> 
            // Rebind using either for ... or let!....
            let rebind = 
                if lastUsesBind then 
                    SynExpr.LetOrUseBang (DebugPointAtBinding.NoneAtLet, false, false, varSpacePat, dataCompPrior, [], compClausesExpr, compClausesExpr.Range, SynExprLetOrUseBangTrivia.Zero) 
                else 
                    SynExpr.ForEach (DebugPointAtFor.No, DebugPointAtInOrTo.No, SeqExprOnly false, false, varSpacePat, dataCompPrior, compClausesExpr, compClausesExpr.Range)
            
            trans CompExprTranslationPass.Initial q varSpace rebind id

    and transNoQueryOps comp =
        trans CompExprTranslationPass.Initial CustomOperationsMode.Denied emptyVarSpace comp id

    and trans firstTry q varSpace comp translatedCtxt = 
        match tryTrans firstTry q varSpace comp translatedCtxt with 
        | Some e -> e
        | None -> 
            // This only occurs in final position in a sequence
            match comp with 
            // "do! expr;" in final position is treated as { let! () = expr in return () } when Return is provided (and no Zero with Default attribute is available) or as { let! () = expr in zero } otherwise
            | SynExpr.DoBang (rhsExpr, m) -> 
                let mUnit = rhsExpr.Range
                let rhsExpr = mkSourceExpr rhsExpr
                if isQuery then error(Error(FSComp.SR.tcBindMayNotBeUsedInQueries(), m))
                let bodyExpr =
                    if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "Return" builderTy) then
                        SynExpr.ImplicitZero m
                    else
                        match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env m ad "Zero" builderTy with
                        | minfo :: _ when MethInfoHasAttribute cenv.g m cenv.g.attrib_DefaultValueAttribute minfo -> SynExpr.ImplicitZero m
                        | _ -> SynExpr.YieldOrReturn ((false, true), SynExpr.Const (SynConst.Unit, m), m)
                let letBangBind = SynExpr.LetOrUseBang (DebugPointAtBinding.NoneAtDo, false, false, SynPat.Const(SynConst.Unit, mUnit), rhsExpr, [], bodyExpr, m, SynExprLetOrUseBangTrivia.Zero)
                trans CompExprTranslationPass.Initial q varSpace letBangBind translatedCtxt

            // "expr;" in final position is treated as { expr; zero }
            // Suppress the sequence point on the "zero"
            | _ -> 
                // Check for 'where x > y' and other mis-applications of infix operators. If detected, give a good error message, and just ignore comp
                if isQuery && checkForBinaryApp comp then 
                    trans CompExprTranslationPass.Initial q varSpace (SynExpr.ImplicitZero comp.Range) translatedCtxt
                else
                    if isQuery && not comp.IsArbExprAndThusAlreadyReportedError then 
                        match comp with 
                        | SynExpr.JoinIn _ -> () // an error will be reported later when we process innerComp1 as a sequential
                        | _ -> errorR(Error(FSComp.SR.tcUnrecognizedQueryOperator(), comp.RangeOfFirstPortion))
                    trans CompExprTranslationPass.Initial q varSpace (SynExpr.ImplicitZero comp.Range) (fun holeFill ->
                        let fillExpr = 
                            if enableImplicitYield then 
                                let implicitYieldExpr = mkSynCall "Yield" comp.Range [comp]
                                SynExpr.SequentialOrImplicitYield(DebugPointAtSequential.SuppressExpr, comp, holeFill, implicitYieldExpr, comp.Range)
                            else
                                SynExpr.Sequential(DebugPointAtSequential.SuppressExpr, true, comp, holeFill, comp.Range)
                        translatedCtxt fillExpr) 

    and transBind q varSpace bindRange addBindDebugPoint bindName bindArgs (consumePat: SynPat) (innerComp: SynExpr) translatedCtxt = 

        let innerRange = innerComp.Range
        
        let innerCompReturn = 
            if cenv.g.langVersion.SupportsFeature LanguageFeature.AndBang then
                convertSimpleReturnToExpr varSpace innerComp
            else None

        match innerCompReturn with 
        | Some (innerExpr, customOpInfo) when 
              (let bindName = bindName + "Return"
               not (isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env bindRange ad bindName  builderTy))) ->

            let bindName = bindName + "Return"
        
            // Build the `BindReturn` call
            let dataCompPriorToOp =
                let consumeExpr = SynExpr.MatchLambda(false, consumePat.Range, [SynMatchClause(consumePat, None, innerExpr, innerRange, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, innerRange)
                translatedCtxt (mkSynCall bindName bindRange (bindArgs @ [consumeExpr]))

            match customOpInfo with 
            | None -> dataCompPriorToOp
            | Some (innerComp, mClause) -> 
                // If the `BindReturn` was forced by a custom operation, continue to process the clauses of the CustomOp
                consumeCustomOpClauses q varSpace dataCompPriorToOp innerComp false mClause

        | _ -> 

            if isNil (TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env bindRange ad bindName  builderTy) then
                error(Error(FSComp.SR.tcRequireBuilderMethod(bindName), bindRange))

            // Build the `Bind` call
            trans CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill ->
                let consumeExpr = SynExpr.MatchLambda(false, consumePat.Range, [SynMatchClause(consumePat, None, holeFill, innerRange, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)], DebugPointAtBinding.NoneAtInvisible, innerRange)
                let bindCall = mkSynCall bindName bindRange (bindArgs @ [consumeExpr])
                translatedCtxt (bindCall |> addBindDebugPoint))

    and convertSimpleReturnToExpr varSpace innerComp =
        match innerComp with 
        | SynExpr.YieldOrReturn ((false, _), returnExpr, m) ->
            let returnExpr = SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, returnExpr)
            Some (returnExpr, None)

        | SynExpr.Match (spMatch, expr, clauses, m, trivia) ->
            let clauses = 
                clauses |> List.map (fun (SynMatchClause(pat, cond, innerComp2, patm, sp, trivia)) -> 
                    match convertSimpleReturnToExpr varSpace innerComp2 with
                    | None -> None // failure
                    | Some (_, Some _) -> None // custom op on branch = failure
                    | Some (innerExpr2, None) -> Some (SynMatchClause(pat, cond, innerExpr2, patm, sp, trivia)))
            if clauses |> List.forall Option.isSome then
                Some (SynExpr.Match (spMatch, expr, (clauses |> List.map Option.get), m, trivia), None)
            else
                None

        | SynExpr.IfThenElse (guardExpr, thenComp, elseCompOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia) ->
            match convertSimpleReturnToExpr varSpace thenComp with
            | None -> None
            | Some (_, Some _) -> None
            | Some (thenExpr, None) ->
            let elseExprOptOpt  = 
                match elseCompOpt with 
                | None -> Some None 
                | Some elseComp -> 
                    match convertSimpleReturnToExpr varSpace elseComp with
                    | None -> None // failure
                    | Some (_, Some _) -> None // custom op on branch = failure
                    | Some (elseExpr, None) -> Some (Some elseExpr)
            match elseExprOptOpt with 
            | None -> None
            | Some elseExprOpt -> Some (SynExpr.IfThenElse (guardExpr, thenExpr, elseExprOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia), None)

        | SynExpr.LetOrUse (isRec, false, binds, innerComp, m, trivia) ->
            match convertSimpleReturnToExpr varSpace innerComp with
            | None -> None
            | Some (_, Some _) -> None 
            | Some (innerExpr, None) -> Some (SynExpr.LetOrUse (isRec, false, binds, innerExpr, m, trivia), None)

        | OptionalSequential (CustomOperationClause (nm, _, _, mClause, _), _) when customOperationMaintainsVarSpaceUsingBind nm -> 

            let patvs, _env = varSpace.Force comp.Range
            let varSpaceExpr = mkExprForVarSpace mClause patvs
            
            Some (varSpaceExpr, Some (innerComp, mClause))

        | SynExpr.Sequential (sp, true, innerComp1, innerComp2, m) -> 

            // Check the first part isn't a computation expression construct
            if isSimpleExpr innerComp1 then
                // Check the second part is a simple return
                match convertSimpleReturnToExpr varSpace innerComp2 with
                | None -> None
                | Some (innerExpr2, optionalCont) -> Some (SynExpr.Sequential (sp, true, innerComp1, innerExpr2, m), optionalCont)
            else
                None

        | _ -> None

    /// Check is an expression has no computation expression constructs
    and isSimpleExpr comp =

        match comp with 
        | ForEachThenJoinOrGroupJoinOrZipClause false _ -> false
        | SynExpr.ForEach _ -> false
        | SynExpr.For _ -> false
        | SynExpr.While _ -> false
        | SynExpr.TryFinally _ -> false
        | SynExpr.ImplicitZero _ -> false
        | OptionalSequential (JoinOrGroupJoinOrZipClause _, _) -> false
        | OptionalSequential (CustomOperationClause _, _) -> false
        | SynExpr.Sequential (_, _, innerComp1, innerComp2, _) -> isSimpleExpr innerComp1 && isSimpleExpr innerComp2
        | SynExpr.IfThenElse (thenExpr=thenComp; elseExpr=elseCompOpt) -> 
             isSimpleExpr thenComp && (match elseCompOpt with None -> true | Some c -> isSimpleExpr c)
        | SynExpr.LetOrUse (body=innerComp) -> isSimpleExpr innerComp
        | SynExpr.LetOrUseBang _ -> false
        | SynExpr.Match (clauses=clauses) ->
            clauses |> List.forall (fun (SynMatchClause(resultExpr = innerComp)) -> isSimpleExpr innerComp)
        | SynExpr.MatchBang _ -> false
        | SynExpr.TryWith (tryExpr=innerComp; withCases=clauses) -> 
            isSimpleExpr innerComp && 
            clauses |> List.forall (fun (SynMatchClause(resultExpr = clauseComp)) -> isSimpleExpr clauseComp)
        | SynExpr.YieldOrReturnFrom _ -> false
        | SynExpr.YieldOrReturn _ -> false
        | SynExpr.DoBang _ -> false
        | _ -> true

    let basicSynExpr = 
        trans CompExprTranslationPass.Initial (hasCustomOperations ()) (LazyWithContext.NotLazy ([], env)) comp id

    let mDelayOrQuoteOrRun = mBuilderVal.NoteSourceConstruct(NotedSourceConstruct.DelayOrQuoteOrRun).MakeSynthetic()

    // Add a call to 'Delay' if the method is present
    let delayedExpr = 
        match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Delay" builderTy with 
        | [] -> basicSynExpr
        | _ ->
            mkSynCall "Delay" mDelayOrQuoteOrRun [(mkSynDelay2 basicSynExpr)]

    // Add a call to 'Quote' if the method is present
    let quotedSynExpr = 
        if isAutoQuote then 
            SynExpr.Quote (mkSynIdGet mDelayOrQuoteOrRun (CompileOpName "<@ @>"), (*isRaw=*)false, delayedExpr, (*isFromQueryExpression=*)true, mWhole) 
        else delayedExpr
            
    // Add a call to 'Run' if the method is present
    let runExpr = 
        match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Run" builderTy with 
        | [] -> quotedSynExpr
        | _ -> mkSynCall "Run" mDelayOrQuoteOrRun [quotedSynExpr]

    let lambdaExpr = 
        SynExpr.Lambda (false, false, SynSimplePats.SimplePats ([mkSynSimplePatVar false (mkSynId mBuilderVal builderValName)], mBuilderVal), runExpr, None, mBuilderVal, SynExprLambdaTrivia.Zero)

    let env =
        match comp with
        | SynExpr.YieldOrReturn ((true, _), _, _) -> { env with eContextInfo = ContextInfo.YieldInComputationExpression }
        | SynExpr.YieldOrReturn ((_, true), _, _) -> { env with eContextInfo = ContextInfo.ReturnInComputationExpression }
        | _ -> env

    let lambdaExpr, tpenv = TcExpr cenv (MustEqual (mkFunTy g builderTy overallTy)) env tpenv lambdaExpr

    // beta-var-reduce to bind the builder using a 'let' binding
    let coreExpr = mkApps cenv.g ((lambdaExpr, tyOfExpr cenv.g lambdaExpr), [], [interpExpr], mBuilderVal)

    coreExpr, tpenv

let mkSeqEmpty (cenv: cenv) env m genTy =
    // We must discover the 'zero' of the monadic algebra being generated in order to compile failing matches.
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy g genResultTy)
    mkCallSeqEmpty g m genResultTy 

let mkSeqCollect (cenv: cenv) env m enumElemTy genTy lam enumExpr =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    let enumExpr = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g enumElemTy) (tyOfExpr cenv.g enumExpr) enumExpr
    mkCallSeqCollect cenv.g m enumElemTy genResultTy lam enumExpr

let mkSeqUsing (cenv: cenv) (env: TcEnv) m resourceTy genTy resourceExpr lam =
    let g = cenv.g
    AddCxTypeMustSubsumeType ContextInfo.NoContext env.DisplayEnv cenv.css m NoTrace cenv.g.system_IDisposable_ty resourceTy
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    mkCallSeqUsing cenv.g m resourceTy genResultTy resourceExpr lam 

let mkSeqDelay (cenv: cenv) env m genTy lam =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    mkCallSeqDelay cenv.g m genResultTy (mkUnitDelayLambda cenv.g m lam) 

let mkSeqAppend (cenv: cenv) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    let e1 = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e1) e1
    let e2 = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e2) e2
    mkCallSeqAppend cenv.g m genResultTy e1 e2 

let mkSeqFromFunctions (cenv: cenv) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    let e2 = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e2) e2
    mkCallSeqGenerated cenv.g m genResultTy e1 e2 

let mkSeqFinally (cenv: cenv) env m genTy e1 e2 =
    let g = cenv.g
    let genResultTy = NewInferenceType g
    UnifyTypes cenv env m genTy (mkSeqTy cenv.g genResultTy)
    let e1 = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g genResultTy) (tyOfExpr cenv.g e1) e1
    mkCallSeqFinally cenv.g m genResultTy e1 e2 

let mkSeqExprMatchClauses (pat, vspecs) innerExpr = 
    [TClause(pat, None, TTarget(vspecs, innerExpr, None), pat.Range) ] 

let compileSeqExprMatchClauses (cenv: cenv) env inputExprMark (pat: Pattern, vspecs) innerExpr inputExprOpt bindPatTy genInnerTy = 
    let patMark = pat.Range
    let tclauses = mkSeqExprMatchClauses (pat, vspecs) innerExpr 
    CompilePatternForMatchClauses cenv env inputExprMark patMark false ThrowIncompleteMatchException inputExprOpt bindPatTy genInnerTy tclauses 

/// This case is used for computation expressions which are sequence expressions. Technically the code path is different because it
/// typechecks rather than doing a shallow syntactic translation, and generates calls into the Seq.* library
/// and helpers rather than to the builder methods (there is actually no builder for 'seq' in the library). 
/// These are later detected by state machine compilation. 
///
/// Also "ienumerable extraction" is performed on arguments to "for".
let TcSequenceExpression (cenv: cenv) env tpenv comp (overallTy: OverallTy) m = 

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
        | SynExpr.ForEach (spFor, spIn, SeqExprOnly _seqExprOnly, _isFromSource, pat, pseudoEnumExpr, innerComp, _m) -> 
            let pseudoEnumExpr =
                match RewriteRangeExpr pseudoEnumExpr with
                | Some e -> e
                | None -> pseudoEnumExpr
            // This expression is not checked with the knowledge it is an IEnumerable, since we permit other enumerable types with GetEnumerator/MoveNext methods, as does C# 
            let pseudoEnumExpr, arbitraryTy, tpenv = TcExprOfUnknownType cenv env tpenv pseudoEnumExpr
            let enumExpr, enumElemTy = ConvertArbitraryExprToEnumerable cenv arbitraryTy env pseudoEnumExpr
            let patR, _, vspecs, envinner, tpenv = TcMatchPattern cenv enumElemTy env tpenv (pat, None)
            let innerExpr, tpenv =
                let envinner = { envinner with eIsControlFlow = true }
                tcSequenceExprBody envinner genOuterTy tpenv innerComp
                
            let enumExprRange = enumExpr.Range

            // We attach the debug point to the lambda expression so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mFor = match spFor with DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For) | _ -> enumExprRange

            // We attach the debug point to the lambda expression so we can fetch it out again in LowerComputedListOrArraySeqExpr
            let mIn = match spIn with DebugPointAtInOrTo.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.InOrTo) | _ -> pat.Range

            match patR, vspecs, innerExpr with 
            // Legacy peephole optimization:
            //     "seq { .. for x in e1 -> e2 .. }" == "e1 |> Seq.map (fun x -> e2)"
            //     "seq { .. for x in e1 do yield e2 .. }" == "e1 |> Seq.map (fun x -> e2)"
            //
            // This transformation is visible in quotations and thus needs to remain.
            | (TPat_as (TPat_wild _, PBind (v, _), _), 
                [_],
                DebugPoints(Expr.App (Expr.Val (vf, _, _), _, [genEnumElemTy], [yieldExpr], _mYield), recreate)) 
                    when valRefEq cenv.g vf cenv.g.seq_singleton_vref ->

                // The debug point mFor is attached to the 'map'
                // The debug point mIn is attached to the lambda
                // Note: the 'yield' part of the debug point for 'yield expr' is currently lost in debug points. 
                let lam = mkLambda mIn v (recreate yieldExpr, genEnumElemTy)
                let enumExpr = mkCoerceIfNeeded cenv.g (mkSeqTy cenv.g enumElemTy) (tyOfExpr cenv.g enumExpr) enumExpr
                Some(mkCallSeqMap cenv.g mFor enumElemTy genEnumElemTy lam enumExpr, tpenv)

            | _ -> 
                // The debug point mFor is attached to the 'collect'
                // The debug point mIn is attached to the lambda
                let matchv, matchExpr = compileSeqExprMatchClauses cenv env enumExprRange (patR, vspecs) innerExpr None enumElemTy genOuterTy
                let lam = mkLambda mIn matchv (matchExpr, tyOfExpr cenv.g matchExpr)
                Some(mkSeqCollect cenv env mFor enumElemTy genOuterTy lam enumExpr, tpenv)

        | SynExpr.For (forDebugPoint=spFor; toDebugPoint=spTo; ident=id; identBody=start; direction=dir; toBody=finish; doBody=innerComp; range=m) ->
            Some(tcSequenceExprBody env genOuterTy tpenv (elimFastIntegerForLoop (spFor, spTo, id, start, dir, finish, innerComp, m)))

        | SynExpr.While (spWhile, guardExpr, innerComp, _m) -> 
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

        | SynExpr.TryFinally (innerComp, unwindExpr, mTryToLast, spTry, spFinally, trivia) ->
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

        | SynExpr.Paren (_, _, _, m) when not (cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield)->
            error(Error(FSComp.SR.tcConstructIsAmbiguousInSequenceExpression(), m))

        | SynExpr.ImplicitZero m -> 
            Some(mkSeqEmpty cenv env m genOuterTy, tpenv )

        | SynExpr.DoBang (_rhsExpr, m) -> 
            error(Error(FSComp.SR.tcDoBangIllegalInSequenceExpression(), m))

        | SynExpr.Sequential (sp, true, innerComp1, innerComp2, m) -> 
            let env1 = { env with eIsControlFlow = (match sp with DebugPointAtSequential.SuppressNeither | DebugPointAtSequential.SuppressExpr -> true | _ -> false) }

            let res, tpenv = tcSequenceExprBodyAsSequenceOrStatement env1 genOuterTy tpenv innerComp1 

            let env2 = { env with eIsControlFlow = (match sp with DebugPointAtSequential.SuppressNeither | DebugPointAtSequential.SuppressStmt -> true | _ -> false) }

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

        | SynExpr.IfThenElse (guardExpr, thenComp, elseCompOpt, spIfToThen, _isRecovery, mIfToEndOfElseBranch, trivia) ->
            let guardExpr', tpenv = TcExpr cenv (MustEqual cenv.g.bool_ty) env tpenv guardExpr
            let env = { env with eIsControlFlow = true }
            let thenExpr, tpenv = tcSequenceExprBody env genOuterTy tpenv thenComp
            let elseComp = (match elseCompOpt with Some c -> c | None -> SynExpr.ImplicitZero trivia.IfToThenRange)
            let elseExpr, tpenv = tcSequenceExprBody env genOuterTy tpenv elseComp
            Some(mkCond spIfToThen mIfToEndOfElseBranch genOuterTy guardExpr' thenExpr elseExpr, tpenv)

        // 'let x = expr in expr'
        | SynExpr.LetOrUse (isUse=false (* not a 'use' binding *)) ->
            TcLinearExprs 
                (fun overallTy envinner tpenv e -> tcSequenceExprBody envinner overallTy.Commit tpenv e) 
                cenv env overallTy 
                tpenv 
                true
                comp 
                id |> Some

        // 'use x = expr in expr'
        | SynExpr.LetOrUse (isUse=true; bindings=[SynBinding (kind=SynBindingKind.Normal; headPat=pat; expr=rhsExpr; debugPoint=spBind)]; body=innerComp; range=wholeExprMark) ->

            let bindPatTy = NewInferenceType g
            let inputExprTy = NewInferenceType g
            let pat', _, vspecs, envinner, tpenv = TcMatchPattern cenv bindPatTy env tpenv (pat, None)

            UnifyTypes cenv env m inputExprTy bindPatTy

            let inputExpr, tpenv =
                let env = { env with eIsControlFlow = true }
                TcExpr cenv (MustEqual inputExprTy) env tpenv rhsExpr

            let innerExpr, tpenv =
                let envinner = { envinner with eIsControlFlow = true }
                tcSequenceExprBody envinner genOuterTy tpenv innerComp

            let mBind = 
                match spBind with 
                | DebugPointAtBinding.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Binding)
                | _ -> inputExpr.Range

            let inputExprMark = inputExpr.Range

            let matchv, matchExpr = compileSeqExprMatchClauses cenv envinner inputExprMark (pat', vspecs) innerExpr (Some inputExpr) bindPatTy genOuterTy 

            let consumeExpr = mkLambda mBind matchv (matchExpr, genOuterTy)

            // The 'mBind' is attached to the lambda
            Some(mkSeqUsing cenv env wholeExprMark bindPatTy genOuterTy inputExpr consumeExpr, tpenv)

        | SynExpr.LetOrUseBang (range=m) -> 
            error(Error(FSComp.SR.tcUseForInSequenceExpression(), m))

        | SynExpr.Match (spMatch, expr, clauses, _m, _trivia) ->
            let inputExpr, matchty, tpenv = TcExprOfUnknownType cenv env tpenv expr

            let tclauses, tpenv = 
                (tpenv, clauses) ||> List.mapFold (fun tpenv (SynMatchClause(pat, cond, innerComp, _, sp, _)) ->
                      let patR, condR, vspecs, envinner, tpenv = TcMatchPattern cenv matchty env tpenv (pat, cond)
                      let envinner =
                          match sp with
                          | DebugPointAtTarget.Yes -> { envinner with eIsControlFlow = true }
                          | DebugPointAtTarget.No -> envinner
                      let innerExpr, tpenv = tcSequenceExprBody envinner genOuterTy tpenv innerComp
                      TClause(patR, condR, TTarget(vspecs, innerExpr, None), patR.Range), tpenv)

            let inputExprTy = tyOfExpr cenv.g inputExpr
            let inputExprMark = inputExpr.Range
            let matchv, matchExpr = CompilePatternForMatchClauses cenv env inputExprMark inputExprMark true ThrowIncompleteMatchException (Some inputExpr) inputExprTy genOuterTy tclauses 

            Some(mkLet spMatch inputExprMark matchv inputExpr matchExpr, tpenv)

        | SynExpr.TryWith (trivia={ TryToWithRange = mTryToWith }) ->
            error(Error(FSComp.SR.tcTryIllegalInSequenceExpression(), mTryToWith))

        | SynExpr.YieldOrReturnFrom ((isYield, _), synYieldExpr, m) -> 
            let env = { env with eIsControlFlow = false }
            let resultExpr, genExprTy, tpenv = TcExprOfUnknownType cenv env tpenv synYieldExpr

            if not isYield then errorR(Error(FSComp.SR.tcUseYieldBangForMultipleResults(), m)) 

            AddCxTypeMustSubsumeType ContextInfo.NoContext env.DisplayEnv cenv.css m NoTrace genOuterTy genExprTy

            let resultExpr = mkCoerceExpr(resultExpr, genOuterTy, m, genExprTy)

            let resultExpr =
                if IsControlFlowExpression synYieldExpr then
                    resultExpr
                else
                    mkDebugPoint m resultExpr

            Some(resultExpr, tpenv)

        | SynExpr.YieldOrReturn ((isYield, _), synYieldExpr, m) -> 
            let env = { env with eIsControlFlow = false }
            let genResultTy = NewInferenceType g

            if not isYield then errorR(Error(FSComp.SR.tcSeqResultsUseYield(), m)) 

            UnifyTypes cenv env m genOuterTy (mkSeqTy cenv.g genResultTy)

            let resultExpr, tpenv = TcExprFlex cenv flex true genResultTy env tpenv synYieldExpr

            let resultExpr = mkCallSeqSingleton cenv.g m genResultTy resultExpr

            let resultExpr =
                if IsControlFlowExpression synYieldExpr then
                    resultExpr
                else
                    mkDebugPoint m resultExpr

            Some(resultExpr, tpenv )

        | _ -> None
                
    and tcSequenceExprBody env (genOuterTy: TType) tpenv comp =
        let res, tpenv = tcSequenceExprBodyAsSequenceOrStatement env genOuterTy tpenv comp 
        match res with 
        | Choice1Of2 expr -> 
            expr, tpenv
        | Choice2Of2 stmt -> 
            let m = comp.Range
            let resExpr = Expr.Sequential(stmt, mkSeqEmpty cenv env m genOuterTy, NormalSeq, m)
            resExpr, tpenv

    and tcSequenceExprBodyAsSequenceOrStatement env genOuterTy tpenv comp =
        match tryTcSequenceExprBody env genOuterTy tpenv comp with 
        | Some (expr, tpenv) -> Choice1Of2 expr, tpenv
        | None -> 

            let env = { env with eContextInfo = ContextInfo.SequenceExpression genOuterTy }

            if enableImplicitYield then 
                let hasTypeUnit, expr, tpenv = TryTcStmt cenv env tpenv comp
                if hasTypeUnit then 
                    Choice2Of2 expr, tpenv
                else
                    let genResultTy = NewInferenceType g
                    UnifyTypes cenv env m genOuterTy (mkSeqTy cenv.g genResultTy)
                    let exprTy = tyOfExpr cenv.g expr
                    AddCxTypeMustSubsumeType env.eContextInfo env.DisplayEnv cenv.css m  NoTrace genResultTy exprTy
                    let resExpr = mkCallSeqSingleton cenv.g m genResultTy (mkCoerceExpr(expr, genResultTy, m, exprTy))
                    Choice1Of2 resExpr, tpenv
            else
                let stmt, tpenv = TcStmtThatCantBeCtorBody cenv env tpenv comp
                Choice2Of2 stmt, tpenv

    let coreExpr, tpenv = tcSequenceExprBody env overallTy.Commit tpenv comp
    let delayedExpr = mkSeqDelayedExpr coreExpr.Range coreExpr
    delayedExpr, tpenv

let TcSequenceExpressionEntry (cenv: cenv) env (overallTy: OverallTy) tpenv (hasBuilder, comp) m =
    match RewriteRangeExpr comp with
    | Some replacementExpr -> 
        TcExpr cenv overallTy env tpenv replacementExpr
    | None ->

    let implicitYieldEnabled = cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield

    let validateObjectSequenceOrRecordExpression = not implicitYieldEnabled

    match comp with 
    | SynExpr.New _ -> 
        errorR(Error(FSComp.SR.tcInvalidObjectExpressionSyntaxForm(), m))
    | SimpleSemicolonSequence cenv false _ when validateObjectSequenceOrRecordExpression ->
        errorR(Error(FSComp.SR.tcInvalidObjectSequenceOrRecordExpression(), m))
    | _ -> 
        ()

    if not hasBuilder && not cenv.g.compilingFslib then 
        error(Error(FSComp.SR.tcInvalidSequenceExpressionSyntaxForm(), m))
        
    TcSequenceExpression cenv env tpenv comp overallTy m

let TcArrayOrListComputedExpression (cenv: cenv) env (overallTy: OverallTy) tpenv (isArray, comp) m  =
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
            if cenv.g.compilingFslib then 
                expr 
            else 
                // We add a call to 'seq ... ' to make sure sequence expression compilation gets applied to the contents of the
                // comprehension. But don't do this in FSharp.Core.dll since 'seq' may not yet be defined.
                mkCallSeq cenv.g m genCollElemTy expr
                   
        let expr = mkCoerceExpr(expr, exprTy, expr.Range, overallTy.Commit)

        let expr = 
            if isArray then 
                mkCallSeqToArray cenv.g m genCollElemTy expr
            else 
                mkCallSeqToList cenv.g m genCollElemTy expr
        expr, tpenv

    | None ->

    // LanguageFeatures.ImplicitYield do not require this validation
    let implicitYieldEnabled = cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield
    let validateExpressionWithIfRequiresParenthesis = not implicitYieldEnabled
    let acceptDeprecatedIfThenExpression = not implicitYieldEnabled

    match comp with 
    | SimpleSemicolonSequence cenv acceptDeprecatedIfThenExpression elems -> 
        match comp with
        | SimpleSemicolonSequence cenv false _ -> ()
        | _ when validateExpressionWithIfRequiresParenthesis -> errorR(Deprecated(FSComp.SR.tcExpressionWithIfRequiresParenthesis(), m))
        | _ -> ()

        let replacementExpr = 
            if isArray then 
                // This are to improve parsing/processing speed for parser tables by converting to an array blob ASAP 
                let nelems = elems.Length 
                if nelems > 0 && List.forall (function SynExpr.Const (SynConst.UInt16 _, _) -> true | _ -> false) elems 
                then SynExpr.Const (SynConst.UInt16s (Array.ofList (List.map (function SynExpr.Const (SynConst.UInt16 x, _) -> x | _ -> failwith "unreachable") elems)), m)
                elif nelems > 0 && List.forall (function SynExpr.Const (SynConst.Byte _, _) -> true | _ -> false) elems 
                then SynExpr.Const (SynConst.Bytes (Array.ofList (List.map (function SynExpr.Const (SynConst.Byte x, _) -> x | _ -> failwith "unreachable") elems), SynByteStringKind.Regular, m), m)
                else SynExpr.ArrayOrList (isArray, elems, m)
            else 
                if elems.Length > 500 then 
                    error(Error(FSComp.SR.tcListLiteralMaxSize(), m))
                SynExpr.ArrayOrList (isArray, elems, m)

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
            if cenv.g.compilingFslib then 
                //warning(Error(FSComp.SR.fslibUsingComputedListOrArray(), expr.Range))
                expr 
            else 
                // We add a call to 'seq ... ' to make sure sequence expression compilation gets applied to the contents of the
                // comprehension. But don't do this in FSharp.Core.dll since 'seq' may not yet be defined.
                mkCallSeq cenv.g m genCollElemTy expr
                   
        let expr = mkCoerceExpr(expr, exprTy, expr.Range, overallTy.Commit)

        let expr = 
            if isArray then 
                mkCallSeqToArray cenv.g m genCollElemTy expr
            else 
                mkCallSeqToList cenv.g m genCollElemTy expr
                
        expr, tpenv)
