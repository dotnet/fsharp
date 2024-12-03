// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The typechecker. Left-to-right constrained type checking
/// with generalization at appropriate points.
module internal FSharp.Compiler.CheckComputationExpressions

open Internal.Utilities.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckExpressionsOps
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open System.Collections.Generic

type cenv = TcFileState

/// Used to flag if this is the first or a subsequent translation pass through a computation expression
[<RequireQualifiedAccess; Struct; NoComparison>]
type CompExprTranslationPass =
    | Initial
    | Subsequent

/// Used to flag if computation expression custom operations are allowed in a given context
[<RequireQualifiedAccess; Struct; NoComparison; NoEquality>]
type CustomOperationsMode =
    | Allowed
    | Denied

[<NoComparison; NoEquality>]
type ComputationExpressionContext<'a> =
    {
        cenv: TcFileState
        env: TcEnv
        tpenv: UnscopedTyparEnv
        customOperationMethodsIndexedByKeyword:
            IDictionary<string, list<string * bool * bool * bool * bool * bool * bool * option<string> * MethInfo>>
        customOperationMethodsIndexedByMethodName:
            IDictionary<string, list<string * bool * bool * bool * bool * bool * bool * option<string> * MethInfo>>
        sourceMethInfo: 'a list
        builderValName: string
        ad: AccessorDomain
        builderTy: TType
        isQuery: bool
        enableImplicitYield: bool
        origComp: SynExpr
        mWhole: range
        emptyVarSpace: LazyWithContext<list<Val> * TcEnv, range>
    }

let inline TryFindIntrinsicOrExtensionMethInfo collectionSettings (cenv: cenv) (env: TcEnv) m ad nm ty =
    AllMethInfosOfTypeInScope collectionSettings cenv.infoReader env.NameEnv (Some nm) ad IgnoreOverrides m ty

/// Ignores an attribute
let inline IgnoreAttribute _ = None

let inline arbPat (m: range) =
    mkSynPatVar None (mkSynId (m.MakeSynthetic()) "_missingVar")

let inline arbKeySelectors m =
    mkSynBifix m "=" (arbExpr ("_keySelectors", m)) (arbExpr ("_keySelector2", m))

// Flag that a debug point should get emitted prior to both the evaluation of 'rhsExpr' and the call to Using
let inline addBindDebugPoint spBind e =
    match spBind with
    | DebugPointAtBinding.Yes m -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, e)
    | _ -> e

let inline mkSynDelay2 (e: SynExpr) = mkSynDelay (e.Range.MakeSynthetic()) e

/// Make a builder.Method(...) call
let mkSynCall nm (m: range) args builderValName =
    let m = m.MakeSynthetic() // Mark as synthetic so the language service won't pick it up.

    let args =
        match args with
        | [] -> SynExpr.Const(SynConst.Unit, m)
        | [ arg ] -> SynExpr.Paren(SynExpr.Paren(arg, range0, None, m), range0, None, m)
        | args -> SynExpr.Paren(SynExpr.Tuple(false, args, [], m), range0, None, m)

    let builderVal = mkSynIdGet m builderValName
    mkSynApp1 (SynExpr.DotGet(builderVal, range0, SynLongIdent([ mkSynId m nm ], [], [ None ]), m)) args m

// Optionally wrap sources of "let!", "yield!", "use!" in "query.Source"
let mkSourceExpr callExpr sourceMethInfo builderValName =
    match sourceMethInfo with
    | [] -> callExpr
    | _ -> mkSynCall "Source" callExpr.Range [ callExpr ] builderValName

let mkSourceExprConditional isFromSource callExpr sourceMethInfo builderValName =
    if isFromSource then
        mkSourceExpr callExpr sourceMethInfo builderValName
    else
        callExpr

let inline mkSynLambda p e m =
    SynExpr.Lambda(false, false, p, e, None, m, SynExprLambdaTrivia.Zero)

let mkExprForVarSpace m (patvs: Val list) =
    match patvs with
    | [] -> SynExpr.Const(SynConst.Unit, m)
    | [ v ] -> SynExpr.Ident v.Id
    | vs -> SynExpr.Tuple(false, (vs |> List.map (fun v -> SynExpr.Ident(v.Id))), [], m)

let mkSimplePatForVarSpace m (patvs: Val list) =
    let spats =
        match patvs with
        | [] -> []
        | [ v ] -> [ mkSynSimplePatVar false v.Id ]
        | vs -> vs |> List.map (fun v -> mkSynSimplePatVar false v.Id)

    SynSimplePats.SimplePats(spats, [], m)

let mkPatForVarSpace m (patvs: Val list) =
    match patvs with
    | [] -> SynPat.Const(SynConst.Unit, m)
    | [ v ] -> mkSynPatVar None v.Id
    | vs -> SynPat.Tuple(false, (vs |> List.map (fun x -> mkSynPatVar None x.Id)), [], m)

let hasMethInfo nm cenv env mBuilderVal ad builderTy =
    match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad nm builderTy with
    | [] -> false
    | _ -> true

let getCustomOperationMethods (cenv: TcFileState) (env: TcEnv) ad mBuilderVal builderTy =
    let allMethInfos =
        AllMethInfosOfTypeInScope
            ResultCollectionSettings.AllResults
            cenv.infoReader
            env.NameEnv
            None
            ad
            IgnoreOverrides
            mBuilderVal
            builderTy

    [
        for methInfo in allMethInfos do
            if IsMethInfoAccessible cenv.amap mBuilderVal ad methInfo then
                let nameSearch =
                    TryBindMethInfoAttribute
                        cenv.g
                        mBuilderVal
                        cenv.g.attrib_CustomOperationAttribute
                        methInfo
                        IgnoreAttribute // We do not respect this attribute for IL methods
                        (fun attr ->
                            // NOTE: right now, we support of custom operations with spaces in them ([<CustomOperation("foo bar")>])
                            // In the parameterless CustomOperationAttribute - we use the method name, and also allow it to be ````-quoted (member _.``foo bar`` _ = ...)
                            match attr with
                            // Empty string and parameterless constructor - we use the method name
                            | Attrib(unnamedArgs = [ AttribStringArg "" ]) // Empty string as parameter
                            | Attrib(unnamedArgs = []) -> // No parameters, same as empty string for compat reasons.
                                Some methInfo.LogicalName
                            // Use the specified name
                            | Attrib(unnamedArgs = [ AttribStringArg msg ]) -> Some msg
                            | _ -> None)
                        IgnoreAttribute // We do not respect this attribute for provided methods

                match nameSearch with
                | None -> ()
                | Some nm ->
                    let joinConditionWord =
                        TryBindMethInfoAttribute
                            cenv.g
                            mBuilderVal
                            cenv.g.attrib_CustomOperationAttribute
                            methInfo
                            IgnoreAttribute // We do not respect this attribute for IL methods
                            (function
                            | Attrib(propVal = ExtractAttribNamedArg "JoinConditionWord" (AttribStringArg s)) -> Some s
                            | _ -> None)
                            IgnoreAttribute // We do not respect this attribute for provided methods

                    let flagSearch (propName: string) =
                        TryBindMethInfoAttribute
                            cenv.g
                            mBuilderVal
                            cenv.g.attrib_CustomOperationAttribute
                            methInfo
                            IgnoreAttribute // We do not respect this attribute for IL methods
                            (function
                            | Attrib(propVal = ExtractAttribNamedArg propName (AttribBoolArg b)) -> Some b
                            | _ -> None)
                            IgnoreAttribute // We do not respect this attribute for provided methods

                    let maintainsVarSpaceUsingBind =
                        defaultArg (flagSearch "MaintainsVariableSpaceUsingBind") false

                    let maintainsVarSpace = defaultArg (flagSearch "MaintainsVariableSpace") false
                    let allowInto = defaultArg (flagSearch "AllowIntoPattern") false
                    let isLikeZip = defaultArg (flagSearch "IsLikeZip") false
                    let isLikeJoin = defaultArg (flagSearch "IsLikeJoin") false
                    let isLikeGroupJoin = defaultArg (flagSearch "IsLikeGroupJoin") false

                    nm,
                    maintainsVarSpaceUsingBind,
                    maintainsVarSpace,
                    allowInto,
                    isLikeZip,
                    isLikeJoin,
                    isLikeGroupJoin,
                    joinConditionWord,
                    methInfo
    ]

/// Decide if the identifier represents a use of a custom query operator
let tryGetDataForCustomOperation (nm: Ident) ceenv =
    let isOpDataCountAllowed opDatas =
        match opDatas with
        | [ _ ] -> true
        | _ :: _ -> ceenv.cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations
        | _ -> false

    match ceenv.customOperationMethodsIndexedByKeyword.TryGetValue nm.idText with
    | true, opDatas when isOpDataCountAllowed opDatas ->
        for opData in opDatas do
            let (opName,
                 maintainsVarSpaceUsingBind,
                 maintainsVarSpace,
                 _allowInto,
                 isLikeZip,
                 isLikeJoin,
                 isLikeGroupJoin,
                 _joinConditionWord,
                 methInfo) =
                opData

            if
                (maintainsVarSpaceUsingBind && maintainsVarSpace)
                || (isLikeZip && isLikeJoin)
                || (isLikeZip && isLikeGroupJoin)
                || (isLikeJoin && isLikeGroupJoin)
            then
                errorR (Error(FSComp.SR.tcCustomOperationInvalid opName, nm.idRange))

            if not (ceenv.cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations) then
                match ceenv.customOperationMethodsIndexedByMethodName.TryGetValue methInfo.LogicalName with
                | true, [ _ ] -> ()
                | _ -> errorR (Error(FSComp.SR.tcCustomOperationMayNotBeOverloaded nm.idText, nm.idRange))

        Some opDatas
    | true, opData :: _ ->
        errorR (Error(FSComp.SR.tcCustomOperationMayNotBeOverloaded nm.idText, nm.idRange))
        Some [ opData ]
    | _ -> None

let isCustomOperation ceenv nm =
    tryGetDataForCustomOperation nm ceenv |> Option.isSome

let customOperationCheckValidity m f opDatas =
    let vs = List.map f opDatas
    let v0 = vs[0]

    let (opName,
         _maintainsVarSpaceUsingBind,
         _maintainsVarSpace,
         _allowInto,
         _isLikeZip,
         _isLikeJoin,
         _isLikeGroupJoin,
         _joinConditionWord,
         _methInfo) =
        opDatas[0]

    if not (List.allEqual vs) then
        errorR (Error(FSComp.SR.tcCustomOperationInvalid opName, m))

    v0

// Check for the MaintainsVariableSpace on custom operation
let customOperationMaintainsVarSpace ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> maintainsVarSpace)

let customOperationMaintainsVarSpaceUsingBind ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> maintainsVarSpaceUsingBind)

let customOperationIsLikeZip ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> isLikeZip)

let customOperationIsLikeJoin ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> isLikeJoin)

let customOperationIsLikeGroupJoin ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> isLikeGroupJoin)

let customOperationJoinConditionWord ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 joinConditionWord,
                 _methInfo) -> joinConditionWord)
        |> function
            | None -> "on"
            | Some v -> v
    | _ -> "on"

let customOperationAllowsInto ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> false
    | Some opDatas ->
        opDatas
        |> customOperationCheckValidity
            nm.idRange
            (fun
                (_nm,
                 _maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 _methInfo) -> allowInto)

let customOpUsageText ceenv nm =
    match tryGetDataForCustomOperation nm ceenv with
    | Some((_nm,
            _maintainsVarSpaceUsingBind,
            _maintainsVarSpace,
            _allowInto,
            isLikeZip,
            isLikeJoin,
            isLikeGroupJoin,
            _joinConditionWord,
            _methInfo) :: _) ->
        if isLikeGroupJoin then
            Some(
                FSComp.SR.customOperationTextLikeGroupJoin (
                    nm.idText,
                    customOperationJoinConditionWord ceenv nm,
                    customOperationJoinConditionWord ceenv nm
                )
            )
        elif isLikeJoin then
            Some(
                FSComp.SR.customOperationTextLikeJoin (
                    nm.idText,
                    customOperationJoinConditionWord ceenv nm,
                    customOperationJoinConditionWord ceenv nm
                )
            )
        elif isLikeZip then
            Some(FSComp.SR.customOperationTextLikeZip (nm.idText))
        else
            None
    | _ -> None

let tryGetArgAttribsForCustomOperator ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | Some argInfos ->
        argInfos
        |> List.map
            (fun
                (_nm,
                 __maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 methInfo) ->
                match methInfo.GetParamAttribs(ceenv.cenv.amap, ceenv.mWhole) with
                | [ curriedArgInfo ] -> Some curriedArgInfo // one for the actual argument group
                | _ -> None)
        |> Some
    | _ -> None

let tryGetArgInfosForCustomOperator ceenv (nm: Ident) =
    match tryGetDataForCustomOperation nm ceenv with
    | Some argInfos ->
        argInfos
        |> List.map
            (fun
                (_nm,
                 __maintainsVarSpaceUsingBind,
                 _maintainsVarSpace,
                 _allowInto,
                 _isLikeZip,
                 _isLikeJoin,
                 _isLikeGroupJoin,
                 _joinConditionWord,
                 methInfo) ->
                match methInfo with
                | FSMeth(_, _, vref, _) ->
                    match ArgInfosOfMember ceenv.cenv.g vref with
                    | [ curriedArgInfo ] -> Some curriedArgInfo
                    | _ -> None
                | _ -> None)
        |> Some
    | _ -> None

let tryExpectedArgCountForCustomOperator ceenv (nm: Ident) =
    match tryGetArgAttribsForCustomOperator ceenv nm with
    | None -> None
    | Some argInfosForOverloads ->
        let nums =
            argInfosForOverloads
            |> List.map (function
                | None -> -1
                | Some argInfos -> List.length argInfos)

        // Prior to 'OverloadsForCustomOperations' we count exact arguments.
        //
        // With 'OverloadsForCustomOperations' we don't compute an exact expected argument count
        // if any arguments are optional, out or ParamArray.
        let isSpecial =
            if ceenv.cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
                argInfosForOverloads
                |> List.exists (fun info ->
                    match info with
                    | None -> false
                    | Some args ->
                        args
                        |> List.exists (fun (ParamAttribs(isParamArrayArg, _isInArg, isOutArg, optArgInfo, _callerInfo, _reflArgInfo)) ->
                            isParamArrayArg || isOutArg || optArgInfo.IsOptional))
            else
                false

        if not isSpecial && nums |> List.forall (fun v -> v >= 0 && v = nums[0]) then
            Some(max (nums[0] - 1) 0) // drop the computation context argument
        else
            None

// Check for the [<ProjectionParameter>] attribute on an argument position
let isCustomOperationProjectionParameter ceenv i (nm: Ident) =
    match tryGetArgInfosForCustomOperator ceenv nm with
    | None -> false
    | Some argInfosForOverloads ->
        let vs =
            argInfosForOverloads
            |> List.map (function
                | None -> false
                | Some argInfos ->
                    i < argInfos.Length
                    && let _, argInfo = List.item i argInfos in
                       HasFSharpAttribute ceenv.cenv.g ceenv.cenv.g.attrib_ProjectionParameterAttribute argInfo.Attribs)

        if List.allEqual vs then
            vs[0]
        else
            let opDatas = (tryGetDataForCustomOperation nm ceenv).Value

            let opName, _, _, _, _, _, _, _j, _ = opDatas[0]
            errorR (Error(FSComp.SR.tcCustomOperationInvalid opName, nm.idRange))
            false

[<return: Struct>]
let (|ExprAsPat|_|) (f: SynExpr) =
    match f with
    | SingleIdent v1
    | SynExprParen(SingleIdent v1, _, _, _) -> ValueSome(mkSynPatVar None v1)
    | SynExprParen(SynExpr.Tuple(false, elems, commas, _), _, _, _) ->
        let elems = elems |> List.map (|SingleIdent|_|)

        if elems |> List.forall (fun x -> x.IsSome) then
            ValueSome(SynPat.Tuple(false, (elems |> List.map (fun x -> mkSynPatVar None x.Value)), commas, f.Range))
        else
            ValueNone
    | _ -> ValueNone

// For join clauses that join on nullable, we syntactically insert the creation of nullable values on the appropriate side of the condition,
// then pull the syntax apart again
[<return: Struct>]
let (|JoinRelation|_|) ceenv (expr: SynExpr) =
    let m = expr.Range
    let ad = ceenv.env.eAccessRights

    let isOpName opName vref s =
        (s = opName)
        && match
            ResolveExprLongIdent
                ceenv.cenv.tcSink
                ceenv.cenv.nameResolver
                m
                ad
                ceenv.env.eNameResEnv
                TypeNameResolutionInfo.Default
                [ ident (opName, m) ]
                None
           with
           | Result(_, Item.Value vref2, []) -> valRefEq ceenv.cenv.g vref vref2
           | _ -> false

    match expr with
    | BinOpExpr(opId, a, b) when isOpName opNameEquals ceenv.cenv.g.equals_operator_vref opId.idText -> ValueSome(a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameEqualsNullable ceenv.cenv.g.equals_nullable_operator_vref opId.idText ->

        let a =
            SynExpr.App(ExprAtomicFlag.Atomic, false, mkSynLidGet a.Range [ MangledGlobalName; "System" ] "Nullable", a, a.Range)

        ValueSome(a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameNullableEquals ceenv.cenv.g.nullable_equals_operator_vref opId.idText ->

        let b =
            SynExpr.App(ExprAtomicFlag.Atomic, false, mkSynLidGet b.Range [ MangledGlobalName; "System" ] "Nullable", b, b.Range)

        ValueSome(a, b)

    | BinOpExpr(opId, a, b) when isOpName opNameNullableEqualsNullable ceenv.cenv.g.nullable_equals_nullable_operator_vref opId.idText ->

        ValueSome(a, b)

    | _ -> ValueNone

let (|ForEachThen|_|) synExpr =
    match synExpr with
    | SynExpr.ForEach(_spFor,
                      _spIn,
                      SeqExprOnly false,
                      isFromSource,
                      pat1,
                      expr1,
                      SynExpr.Sequential(isTrueSeq = true; expr1 = clause; expr2 = rest),
                      _) -> Some(isFromSource, pat1, expr1, clause, rest)
    | _ -> None

let (|CustomOpId|_|) isCustomOperation predicate synExpr =
    match synExpr with
    | SingleIdent nm when isCustomOperation nm && predicate nm -> Some nm
    | _ -> None

// e1 in e2 ('in' is parsed as 'JOIN_IN')
let (|InExpr|_|) synExpr =
    match synExpr with
    | SynExpr.JoinIn(e1, _, e2, mApp) -> Some(e1, e2, mApp)
    | _ -> None

// e1 on e2 (note: 'on' is the 'JoinConditionWord')
let (|OnExpr|_|) ceenv nm synExpr =
    match tryGetDataForCustomOperation nm ceenv with
    | None -> None
    | Some _ ->
        match synExpr with
        | SynExpr.App(funcExpr = SynExpr.App(funcExpr = e1; argExpr = SingleIdent opName); argExpr = e2) when
            opName.idText = customOperationJoinConditionWord ceenv nm
            ->
            let item = Item.CustomOperation(opName.idText, (fun () -> None), None)

            CallNameResolutionSink
                ceenv.cenv.tcSink
                (opName.idRange, ceenv.env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, ceenv.env.AccessRights)

            Some(e1, e2)
        | _ -> None

// e1 into e2
let (|IntoSuffix|_|) (e: SynExpr) =
    match e with
    | SynExpr.App(funcExpr = SynExpr.App(funcExpr = x; argExpr = SingleIdent nm2); argExpr = ExprAsPat intoPat) when
        nm2.idText = CustomOperations.Into
        ->
        Some(x, nm2.idRange, intoPat)
    | _ -> None

let JoinOrGroupJoinOp ceenv detector synExpr =
    match synExpr with
    | SynExpr.App(_, _, CustomOpId (isCustomOperation ceenv) detector nm, ExprAsPat innerSourcePat, mJoinCore) ->
        Some(nm, innerSourcePat, mJoinCore, false)
    // join with bad pattern (gives error on "join" and continues)
    | SynExpr.App(_, _, CustomOpId (isCustomOperation ceenv) detector nm, _innerSourcePatExpr, mJoinCore) ->
        errorR (Error(FSComp.SR.tcBinaryOperatorRequiresVariable (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(nm, arbPat mJoinCore, mJoinCore, true)
    // join (without anything after - gives error on "join" and continues)
    | CustomOpId (isCustomOperation ceenv) detector nm ->
        errorR (Error(FSComp.SR.tcBinaryOperatorRequiresVariable (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(nm, arbPat synExpr.Range, synExpr.Range, true)
    | _ -> None
// JoinOrGroupJoinOp customOperationIsLikeJoin

let (|JoinOp|_|) ceenv synExpr =
    JoinOrGroupJoinOp ceenv (customOperationIsLikeJoin ceenv) synExpr

let (|GroupJoinOp|_|) ceenv synExpr =
    JoinOrGroupJoinOp ceenv (customOperationIsLikeGroupJoin ceenv) synExpr

let MatchIntoSuffixOrRecover ceenv alreadyGivenError (nm: Ident) synExpr =
    match synExpr with
    | IntoSuffix(x, intoWordRange, intoPat) ->
        // record the "into" as a custom operation for colorization
        let item = Item.CustomOperation("into", (fun () -> None), None)

        CallNameResolutionSink
            ceenv.cenv.tcSink
            (intoWordRange, ceenv.env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, ceenv.env.eAccessRights)

        (x, intoPat, alreadyGivenError)
    | _ ->
        if not alreadyGivenError then
            errorR (Error(FSComp.SR.tcOperatorIncorrectSyntax (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        (synExpr, arbPat synExpr.Range, true)

let MatchOnExprOrRecover ceenv alreadyGivenError nm (onExpr: SynExpr) =
    match onExpr with
    | OnExpr ceenv nm (innerSource, SynExprParen(keySelectors, _, _, _)) -> (innerSource, keySelectors)
    | _ ->
        if not alreadyGivenError then
            suppressErrorReporting (fun () -> TcExprOfUnknownType ceenv.cenv ceenv.env ceenv.tpenv onExpr)
            |> ignore

            errorR (Error(FSComp.SR.tcOperatorIncorrectSyntax (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        (arbExpr ("_innerSource", onExpr.Range),
         mkSynBifix onExpr.Range "=" (arbExpr ("_keySelectors", onExpr.Range)) (arbExpr ("_keySelector2", onExpr.Range)))

let (|JoinExpr|_|) (ceenv: ComputationExpressionContext<'a>) synExpr =
    match synExpr with
    | InExpr(JoinOp ceenv (nm, innerSourcePat, _, alreadyGivenError), onExpr, mJoinCore) ->
        let innerSource, keySelectors =
            MatchOnExprOrRecover ceenv alreadyGivenError nm onExpr

        Some(nm, innerSourcePat, innerSource, keySelectors, mJoinCore)
    | JoinOp ceenv (nm, innerSourcePat, mJoinCore, alreadyGivenError) ->
        if alreadyGivenError then
            errorR (Error(FSComp.SR.tcOperatorRequiresIn (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(nm, innerSourcePat, arbExpr ("_innerSource", synExpr.Range), arbKeySelectors synExpr.Range, mJoinCore)
    | _ -> None

let (|GroupJoinExpr|_|) ceenv synExpr =
    match synExpr with
    | InExpr(GroupJoinOp ceenv (nm, innerSourcePat, _, alreadyGivenError), intoExpr, mGroupJoinCore) ->
        let onExpr, intoPat, alreadyGivenError =
            MatchIntoSuffixOrRecover ceenv alreadyGivenError nm intoExpr

        let innerSource, keySelectors =
            MatchOnExprOrRecover ceenv alreadyGivenError nm onExpr

        Some(nm, innerSourcePat, innerSource, keySelectors, intoPat, mGroupJoinCore)
    | GroupJoinOp ceenv (nm, innerSourcePat, mGroupJoinCore, alreadyGivenError) ->
        if alreadyGivenError then
            errorR (Error(FSComp.SR.tcOperatorRequiresIn (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(
            nm,
            innerSourcePat,
            arbExpr ("_innerSource", synExpr.Range),
            arbKeySelectors synExpr.Range,
            arbPat synExpr.Range,
            mGroupJoinCore
        )
    | _ -> None

let (|JoinOrGroupJoinOrZipClause|_|) (ceenv: ComputationExpressionContext<'a>) synExpr =

    match synExpr with
    // join innerSourcePat in innerSource on (keySelector1 = keySelector2)
    | JoinExpr ceenv (nm, innerSourcePat, innerSource, keySelectors, mJoinCore) ->
        Some(nm, innerSourcePat, innerSource, Some keySelectors, None, mJoinCore)

    // groupJoin innerSourcePat in innerSource on (keySelector1 = keySelector2) into intoPat
    | GroupJoinExpr ceenv (nm, innerSourcePat, innerSource, keySelectors, intoPat, mGroupJoinCore) ->
        Some(nm, innerSourcePat, innerSource, Some keySelectors, Some intoPat, mGroupJoinCore)

    // zip intoPat in secondSource
    | InExpr(SynExpr.App(_, _, CustomOpId (isCustomOperation ceenv) (customOperationIsLikeZip ceenv) nm, ExprAsPat secondSourcePat, _),
             secondSource,
             mZipCore) -> Some(nm, secondSourcePat, secondSource, None, None, mZipCore)

    // zip (without secondSource or in - gives error)
    | CustomOpId (isCustomOperation ceenv) (customOperationIsLikeZip ceenv) nm ->
        errorR (Error(FSComp.SR.tcOperatorIncorrectSyntax (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(nm, arbPat synExpr.Range, arbExpr ("_secondSource", synExpr.Range), None, None, synExpr.Range)

    // zip secondSource (without in - gives error)
    | SynExpr.App(_, _, CustomOpId (isCustomOperation ceenv) (customOperationIsLikeZip ceenv) nm, ExprAsPat secondSourcePat, mZipCore) ->
        errorR (Error(FSComp.SR.tcOperatorIncorrectSyntax (nm.idText, Option.get (customOpUsageText ceenv nm)), mZipCore))

        Some(nm, secondSourcePat, arbExpr ("_innerSource", synExpr.Range), None, None, mZipCore)

    | _ -> None

let (|ForEachThenJoinOrGroupJoinOrZipClause|_|) (ceenv: ComputationExpressionContext<'a>) strict synExpr =
    match synExpr with
    | ForEachThen(isFromSource,
                  firstSourcePat,
                  firstSource,
                  JoinOrGroupJoinOrZipClause ceenv (nm, secondSourcePat, secondSource, keySelectorsOpt, pat3opt, mOpCore),
                  innerComp) when
        (let _firstSourceSimplePats, later1 =
            use _holder = TemporarilySuspendReportingTypecheckResultsToSink ceenv.cenv.tcSink
            SimplePatsOfPat ceenv.cenv.synArgNameGenerator firstSourcePat

         Option.isNone later1)
        ->
        Some(isFromSource, firstSourcePat, firstSource, nm, secondSourcePat, secondSource, keySelectorsOpt, pat3opt, mOpCore, innerComp)

    | JoinOrGroupJoinOrZipClause ceenv (nm, pat2, expr2, expr3, pat3opt, mOpCore) when strict ->
        errorR (Error(FSComp.SR.tcBinaryOperatorRequiresBody (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

        Some(
            true,
            arbPat synExpr.Range,
            arbExpr ("_outerSource", synExpr.Range),
            nm,
            pat2,
            expr2,
            expr3,
            pat3opt,
            mOpCore,
            arbExpr ("_innerComp", synExpr.Range)
        )

    | _ -> None

let (|StripApps|) e =
    let rec strip e =
        match e with
        | SynExpr.FromParseError(SynExpr.App(funcExpr = f; argExpr = arg), _)
        | SynExpr.App(funcExpr = f; argExpr = arg) ->
            let g, acc = strip f
            g, (arg :: acc)
        | _ -> e, []

    let g, acc = strip e
    g, List.rev acc

let (|OptionalIntoSuffix|) e =
    match e with
    | IntoSuffix(body, intoWordRange, intoInfo) -> (body, Some(intoWordRange, intoInfo))
    | body -> (body, None)

let (|CustomOperationClause|_|) ceenv e =
    match e with
    | OptionalIntoSuffix(StripApps(SingleIdent nm, _) as core, intoOpt) when isCustomOperation ceenv nm ->
        // Now we know we have a custom operation, commit the name resolution
        let intoInfoOpt =
            match intoOpt with
            | Some(intoWordRange, intoInfo) ->
                let item = Item.CustomOperation("into", (fun () -> None), None)

                CallNameResolutionSink
                    ceenv.cenv.tcSink
                    (intoWordRange, ceenv.env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, ceenv.env.eAccessRights)

                Some intoInfo
            | None -> None

        Some(nm, Option.get (tryGetDataForCustomOperation nm ceenv), core, core.Range, intoInfoOpt)
    | _ -> None

let (|OptionalSequential|) e =
    match e with
    | SynExpr.Sequential(debugPoint = _sp; isTrueSeq = true; expr1 = dataComp1; expr2 = dataComp2) -> (dataComp1, Some dataComp2)
    | _ -> (e, None)

// "cexpr; cexpr" is treated as builder.Combine(cexpr1, cexpr1)
// This is not pretty - we have to decide which range markers we use for the calls to Combine and Delay
// NOTE: we should probably suppress these sequence points altogether
let rangeForCombine innerComp1 =
    let m =
        match innerComp1 with
        | SynExpr.IfThenElse(trivia = { IfToThenRange = mIfToThen }) -> mIfToThen
        | SynExpr.Match(matchDebugPoint = DebugPointAtBinding.Yes mMatch) -> mMatch
        | SynExpr.TryWith(trivia = { TryKeyword = mTry }) -> mTry
        | SynExpr.TryFinally(trivia = { TryKeyword = mTry }) -> mTry
        | SynExpr.For(forDebugPoint = DebugPointAtFor.Yes mBind) -> mBind
        | SynExpr.ForEach(forDebugPoint = DebugPointAtFor.Yes mBind) -> mBind
        | SynExpr.While(whileDebugPoint = DebugPointAtWhile.Yes mWhile) -> mWhile
        | _ -> innerComp1.Range

    m.NoteSourceConstruct(NotedSourceConstruct.Combine)

// Check for 'where x > y', 'select x, y' and other mis-applications of infix operators, give a good error message, and return a flag
let checkForBinaryApp ceenv comp =
    match comp with
    | StripApps(SingleIdent nm, [ StripApps(SingleIdent nm2, args); arg2 ]) when
        IsLogicalInfixOpName nm.idText
        && (match tryExpectedArgCountForCustomOperator ceenv nm2 with
            | Some n -> n > 0
            | _ -> false)
        && not (List.isEmpty args)
        ->
        let estimatedRangeOfIntendedLeftAndRightArguments =
            unionRanges (List.last args).Range arg2.Range

        errorR (Error(FSComp.SR.tcUnrecognizedQueryBinaryOperator (), estimatedRangeOfIntendedLeftAndRightArguments))
        true
    | SynExpr.Tuple(false, StripApps(SingleIdent nm2, args) :: _, _, m) when
        (match tryExpectedArgCountForCustomOperator ceenv nm2 with
         | Some n -> n > 0
         | _ -> false)
        && not (List.isEmpty args)
        ->
        let estimatedRangeOfIntendedLeftAndRightArguments =
            unionRanges (List.last args).Range m.EndRange

        errorR (Error(FSComp.SR.tcUnrecognizedQueryBinaryOperator (), estimatedRangeOfIntendedLeftAndRightArguments))
        true
    | _ -> false

let inline addVarsToVarSpace (varSpace: LazyWithContext<Val list * TcEnv, range>) f =
    LazyWithContext.Create(
        (fun m ->
            let (patvs: Val list, env) = varSpace.Force m
            let vs, envinner = f m env

            let patvs =
                List.append
                    patvs
                    (vs
                     |> List.filter (fun v -> not (patvs |> List.exists (fun v2 -> v.LogicalName = v2.LogicalName))))

            patvs, envinner),
        id
    )

/// <summary>
/// Try translate the syntax sugar
/// </summary>
/// <param name="ceenv">Computation expression context (carrying caches, environments, ranges, etc)</param>
/// <param name="firstTry">Flag if it's inital check</param>
/// <param name="q">a flag indicating if custom operators are allowed. They are not allowed inside try/with, try/finally, if/then/else etc.</param>
/// <param name="varSpace">a lazy data structure indicating the variables bound so far in the overall computation</param>
/// <param name="comp">the computation expression being analyzed</param>
/// <param name="translatedCtxt">represents the translation of the context in which the computation expression 'comp' occurs,
/// up to a hole to be filled by (part of) the results of translating 'comp'.</param>
/// <typeparam name="'a"></typeparam>
/// <returns></returns>
let rec TryTranslateComputationExpression
    (ceenv: ComputationExpressionContext<'a>)
    (firstTry: CompExprTranslationPass)
    (q: CustomOperationsMode)
    (varSpace: LazyWithContext<(Val list * TcEnv), range>)
    (comp: SynExpr)
    (translatedCtxt: SynExpr -> SynExpr)
    : SynExpr option =
    // Guard the stack for deeply nested computation expressions

    let cenv = ceenv.cenv

    cenv.stackGuard.Guard
    <| fun () ->

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
        | ForEachThenJoinOrGroupJoinOrZipClause ceenv true (isFromSource,
                                                            firstSourcePat,
                                                            firstSource,
                                                            nm,
                                                            secondSourcePat,
                                                            secondSource,
                                                            keySelectorsOpt,
                                                            secondResultPatOpt,
                                                            mOpCore,
                                                            innerComp) ->
            match q with
            | CustomOperationsMode.Denied -> error (Error(FSComp.SR.tcCustomOperationMayNotBeUsedHere (), nm.idRange))
            | CustomOperationsMode.Allowed ->

                let firstSource =
                    mkSourceExprConditional isFromSource firstSource ceenv.sourceMethInfo ceenv.builderValName

                let secondSource =
                    mkSourceExpr secondSource ceenv.sourceMethInfo ceenv.builderValName

                // Add the variables to the variable space, on demand
                let varSpaceWithFirstVars =
                    addVarsToVarSpace varSpace (fun _mCustomOp env ->
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                        let _, _, vspecs, envinner, _ =
                            TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv firstSourcePat None TcTrueMatchClause.No

                        vspecs, envinner)

                let varSpaceWithSecondVars =
                    addVarsToVarSpace varSpaceWithFirstVars (fun _mCustomOp env ->
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                        let _, _, vspecs, envinner, _ =
                            TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv secondSourcePat None TcTrueMatchClause.No

                        vspecs, envinner)

                let varSpaceWithGroupJoinVars =
                    match secondResultPatOpt with
                    | Some pat3 ->
                        addVarsToVarSpace varSpaceWithFirstVars (fun _mCustomOp env ->
                            use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                            let _, _, vspecs, envinner, _ =
                                TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv pat3 None TcTrueMatchClause.No

                            vspecs, envinner)
                    | None -> varSpace

                let firstSourceSimplePats, later1 =
                    SimplePatsOfPat cenv.synArgNameGenerator firstSourcePat

                let secondSourceSimplePats, later2 =
                    SimplePatsOfPat cenv.synArgNameGenerator secondSourcePat

                if Option.isSome later1 then
                    errorR (Error(FSComp.SR.tcJoinMustUseSimplePattern (nm.idText), firstSourcePat.Range))

                if Option.isSome later2 then
                    errorR (Error(FSComp.SR.tcJoinMustUseSimplePattern (nm.idText), secondSourcePat.Range))

                // check 'join' or 'groupJoin' or 'zip' is permitted for this builder
                match tryGetDataForCustomOperation nm ceenv with
                | None -> error (Error(FSComp.SR.tcMissingCustomOperation (nm.idText), nm.idRange))
                | Some opDatas ->
                    let opName, _, _, _, _, _, _, _, methInfo = opDatas[0]

                    // Record the resolution of the custom operation for posterity
                    let item =
                        Item.CustomOperation(opName, (fun () -> customOpUsageText ceenv nm), Some methInfo)

                    // FUTURE: consider whether we can do better than emptyTyparInst here, in order to display instantiations
                    // of type variables in the quick info provided in the IDE.
                    CallNameResolutionSink
                        cenv.tcSink
                        (nm.idRange, ceenv.env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, ceenv.env.eAccessRights)

                    let mkJoinExpr keySelector1 keySelector2 innerPat e =
                        let mSynthetic = mOpCore.MakeSynthetic()

                        mkSynCall
                            methInfo.DisplayName
                            mOpCore
                            [
                                firstSource
                                secondSource
                                mkSynLambda firstSourceSimplePats keySelector1 mSynthetic
                                mkSynLambda secondSourceSimplePats keySelector2 mSynthetic
                                mkSynLambda firstSourceSimplePats (mkSynLambda innerPat e mSynthetic) mSynthetic
                            ]

                    let mkZipExpr e =
                        let mSynthetic = mOpCore.MakeSynthetic()

                        mkSynCall
                            methInfo.DisplayName
                            mOpCore
                            [
                                firstSource
                                secondSource
                                mkSynLambda firstSourceSimplePats (mkSynLambda secondSourceSimplePats e mSynthetic) mSynthetic
                            ]

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
                        SynExpr.Sequential(
                            DebugPointAtSequential.SuppressNeither,
                            true,
                            l,
                            (arbExpr (caption, l.Range.EndRange)),
                            l.Range,
                            SynExprSequentialTrivia.Zero
                        )

                    let mkOverallExprGivenVarSpaceExpr, varSpaceInner =

                        let isNullableOp opId =
                            match ConvertValLogicalNameToDisplayNameCore opId with
                            | "?="
                            | "=?"
                            | "?=?" -> true
                            | _ -> false

                        match secondResultPatOpt, keySelectorsOpt with
                        // groupJoin
                        | Some secondResultPat, Some relExpr when customOperationIsLikeGroupJoin ceenv nm ->
                            let secondResultSimplePats, later3 =
                                SimplePatsOfPat cenv.synArgNameGenerator secondResultPat

                            if Option.isSome later3 then
                                errorR (Error(FSComp.SR.tcJoinMustUseSimplePattern (nm.idText), secondResultPat.Range))

                            match relExpr with
                            | JoinRelation ceenv (keySelector1, keySelector2) ->
                                mkJoinExpr keySelector1 keySelector2 secondResultSimplePats, varSpaceWithGroupJoinVars
                            | BinOpExpr(opId, l, r) ->
                                if isNullableOp opId.idText then
                                    // When we cannot resolve NullableOps, recommend the relevant namespace to be added
                                    errorR (
                                        Error(
                                            FSComp.SR.cannotResolveNullableOperators (ConvertValLogicalNameToDisplayNameCore opId.idText),
                                            relExpr.Range
                                        )
                                    )
                                else
                                    errorR (Error(FSComp.SR.tcInvalidRelationInJoin (nm.idText), relExpr.Range))

                                let l = wrapInArbErrSequence l "_keySelector1"
                                let r = wrapInArbErrSequence r "_keySelector2"
                                // this is not correct JoinRelation but it is still binary operation
                                // we've already reported error now we can use operands of binary operation as join components
                                mkJoinExpr l r secondResultSimplePats, varSpaceWithGroupJoinVars
                            | _ ->
                                errorR (Error(FSComp.SR.tcInvalidRelationInJoin (nm.idText), relExpr.Range))
                                // since the shape of relExpr doesn't match our expectations (JoinRelation)
                                // then we assume that this is l.h.s. of the join relation
                                // so typechecker will treat relExpr as body of outerKeySelector lambda parameter in GroupJoin method
                                mkJoinExpr relExpr (arbExpr ("_keySelector2", relExpr.Range)) secondResultSimplePats,
                                varSpaceWithGroupJoinVars

                        | None, Some relExpr when customOperationIsLikeJoin ceenv nm ->
                            match relExpr with
                            | JoinRelation ceenv (keySelector1, keySelector2) ->
                                mkJoinExpr keySelector1 keySelector2 secondSourceSimplePats, varSpaceWithSecondVars
                            | BinOpExpr(opId, l, r) ->
                                if isNullableOp opId.idText then
                                    // When we cannot resolve NullableOps, recommend the relevant namespace to be added
                                    errorR (
                                        Error(
                                            FSComp.SR.cannotResolveNullableOperators (ConvertValLogicalNameToDisplayNameCore opId.idText),
                                            relExpr.Range
                                        )
                                    )
                                else
                                    errorR (Error(FSComp.SR.tcInvalidRelationInJoin (nm.idText), relExpr.Range))
                                // this is not correct JoinRelation but it is still binary operation
                                // we've already reported error now we can use operands of binary operation as join components
                                let l = wrapInArbErrSequence l "_keySelector1"
                                let r = wrapInArbErrSequence r "_keySelector2"
                                mkJoinExpr l r secondSourceSimplePats, varSpaceWithGroupJoinVars
                            | _ ->
                                errorR (Error(FSComp.SR.tcInvalidRelationInJoin (nm.idText), relExpr.Range))
                                // since the shape of relExpr doesn't match our expectations (JoinRelation)
                                // then we assume that this is l.h.s. of the join relation
                                // so typechecker will treat relExpr as body of outerKeySelector lambda parameter in Join method
                                mkJoinExpr relExpr (arbExpr ("_keySelector2", relExpr.Range)) secondSourceSimplePats,
                                varSpaceWithGroupJoinVars

                        | None, None when customOperationIsLikeZip ceenv nm -> mkZipExpr, varSpaceWithSecondVars

                        | _ ->
                            assert false
                            failwith "unreachable"

                    // Case from C# spec: A query expression with a join clause with an into followed by something other than a select clause
                    // Case from C# spec: A query expression with a join clause without an into followed by something other than a select clause
                    let valsInner, _env = varSpaceInner.Force mOpCore
                    let varSpaceExpr = mkExprForVarSpace mOpCore valsInner
                    let varSpacePat = mkPatForVarSpace mOpCore valsInner
                    let joinExpr = mkOverallExprGivenVarSpaceExpr varSpaceExpr ceenv.builderValName

                    let consumingExpr =
                        SynExpr.ForEach(
                            DebugPointAtFor.No,
                            DebugPointAtInOrTo.No,
                            SeqExprOnly false,
                            false,
                            varSpacePat,
                            joinExpr,
                            innerComp,
                            mOpCore
                        )

                    Some(TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpaceInner consumingExpr translatedCtxt)

        | SynExpr.ForEach(spFor, spIn, SeqExprOnly _seqExprOnly, isFromSource, pat, sourceExpr, innerComp, _mEntireForEach) ->
            let sourceExpr =
                match RewriteRangeExpr sourceExpr with
                | Some e -> e
                | None -> sourceExpr

            let wrappedSourceExpr =
                mkSourceExprConditional isFromSource sourceExpr ceenv.sourceMethInfo ceenv.builderValName

            let mFor =
                match spFor with
                | DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For)
                | DebugPointAtFor.No -> pat.Range

            // For computation expressions, 'in' or 'to' is hit on each MoveNext.
            // To support this a named debug point for the "in" keyword is available to inlined code.
            match spIn with
            | DebugPointAtInOrTo.Yes mIn ->
                cenv.namedDebugPointsForInlinedCode[{
                    Range = mFor
                    Name = "ForLoop.InOrToKeyword"
                }] <- mIn
            | _ -> ()

            let mPat = pat.Range

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mFor
                        ceenv.ad
                        "For"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("For"), mFor))

            // Add the variables to the query variable space, on demand
            let varSpace =
                addVarsToVarSpace varSpace (fun _mCustomOp env ->
                    use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                    let _, _, vspecs, envinner, _ =
                        TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv pat None TcTrueMatchClause.No

                    vspecs, envinner)

            Some(
                TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp (fun innerCompR ->

                    let forCall =
                        mkSynCall
                            "For"
                            mFor
                            [
                                wrappedSourceExpr
                                SynExpr.MatchLambda(
                                    false,
                                    mPat,
                                    [
                                        SynMatchClause(pat, None, innerCompR, mPat, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)
                                    ],
                                    DebugPointAtBinding.NoneAtInvisible,
                                    mFor
                                )
                            ]
                            ceenv.builderValName

                    let forCall =
                        match spFor with
                        | DebugPointAtFor.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mFor, false, forCall)
                        | DebugPointAtFor.No -> forCall

                    translatedCtxt forCall)
            )

        | SynExpr.For(
            forDebugPoint = spFor
            toDebugPoint = spTo
            ident = id
            identBody = start
            direction = dir
            toBody = finish
            doBody = innerComp
            range = m) ->
            let mFor =
                match spFor with
                | DebugPointAtFor.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.For)
                | _ -> m

            if ceenv.isQuery then
                errorR (Error(FSComp.SR.tcNoIntegerForLoopInQuery (), mFor))

            let reduced =
                elimFastIntegerForLoop (spFor, spTo, id, start, dir, finish, innerComp, m)

            Some(TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace reduced translatedCtxt)
        | SynExpr.While(spWhile, guardExpr, innerComp, _) ->
            let mGuard = guardExpr.Range

            let mWhile =
                match spWhile with
                | DebugPointAtWhile.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.While)
                | _ -> mGuard

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcNoWhileInQuery (), mWhile))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mWhile
                        ceenv.ad
                        "While"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("While"), mWhile))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mWhile
                        ceenv.ad
                        "Delay"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Delay"), mWhile))

            // 'while' is hit just before each time the guard is called
            let guardExpr =
                match spWhile with
                | DebugPointAtWhile.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mWhile, false, guardExpr)
                | DebugPointAtWhile.No -> guardExpr

            Some(
                TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill ->
                    translatedCtxt (
                        mkSynCall
                            "While"
                            mWhile
                            [
                                mkSynDelay2 guardExpr
                                mkSynCall "Delay" mWhile [ mkSynDelay innerComp.Range holeFill ] ceenv.builderValName
                            ]
                            ceenv.builderValName
                    ))
            )

        | SynExpr.WhileBang(spWhile, guardExpr, innerComp, mOrig) ->
            let mGuard = guardExpr.Range

            let mWhile =
                match spWhile with
                | DebugPointAtWhile.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.While)
                | _ -> mGuard

            let mGuard = mGuard.MakeSynthetic()

            // 'while!' is hit just before each time the guard is called
            let guardExpr =
                match spWhile with
                | DebugPointAtWhile.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mWhile, false, guardExpr)
                | DebugPointAtWhile.No -> guardExpr

            let rewrittenWhileExpr =
                let idFirst = mkSynId mGuard (CompilerGeneratedName "first")
                let patFirst = mkSynPatVar None idFirst

                let body =
                    let idCond = mkSynId mGuard (CompilerGeneratedName "cond")
                    let patCond = mkSynPatVar None idCond

                    let condBinding =
                        mkSynBinding
                            (Xml.PreXmlDoc.Empty, patCond)
                            (None,
                             false,
                             true,
                             mGuard,
                             DebugPointAtBinding.NoneAtSticky,
                             None,
                             SynExpr.Ident idFirst,
                             mGuard,
                             [],
                             [],
                             None,
                             SynBindingTrivia.Zero)

                    let setCondExpr = SynExpr.Set(SynExpr.Ident idCond, SynExpr.Ident idFirst, mGuard)

                    let bindCondExpr =
                        SynExpr.LetOrUseBang(
                            DebugPointAtBinding.NoneAtSticky,
                            false,
                            true,
                            patFirst,
                            guardExpr,
                            [],
                            setCondExpr,
                            mGuard,
                            SynExprLetOrUseBangTrivia.Zero
                        )

                    let whileExpr =
                        SynExpr.While(
                            DebugPointAtWhile.No,
                            SynExpr.Ident idCond,
                            SynExpr.Sequential(
                                DebugPointAtSequential.SuppressBoth,
                                true,
                                innerComp,
                                bindCondExpr,
                                mWhile,
                                SynExprSequentialTrivia.Zero
                            ),
                            mOrig
                        )

                    SynExpr.LetOrUse(false, false, [ condBinding ], whileExpr, mGuard, SynExprLetOrUseTrivia.Zero)

                SynExpr.LetOrUseBang(
                    DebugPointAtBinding.NoneAtSticky,
                    false,
                    true,
                    patFirst,
                    guardExpr,
                    [],
                    body,
                    mGuard,
                    SynExprLetOrUseBangTrivia.Zero
                )

            TryTranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace rewrittenWhileExpr translatedCtxt

        | SynExpr.TryFinally(innerComp, unwindExpr, _mTryToLast, spTry, spFinally, trivia) ->

            let mTry =
                match spTry with
                | DebugPointAtTry.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Try)
                | _ -> trivia.TryKeyword

            let mFinally =
                match spFinally with
                | DebugPointAtFinally.Yes m -> m.NoteSourceConstruct(NotedSourceConstruct.Finally)
                | _ -> trivia.FinallyKeyword

            // Put down a debug point for the 'finally'
            let unwindExpr2 =
                match spFinally with
                | DebugPointAtFinally.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mFinally, true, unwindExpr)
                | DebugPointAtFinally.No -> unwindExpr

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcNoTryFinallyInQuery (), mTry))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mTry
                        ceenv.ad
                        "TryFinally"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("TryFinally"), mTry))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mTry
                        ceenv.ad
                        "Delay"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Delay"), mTry))

            let innerExpr = TranslateComputationExpressionNoQueryOps ceenv innerComp

            let innerExpr =
                match spTry with
                | DebugPointAtTry.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mTry, true, innerExpr)
                | _ -> innerExpr

            Some(
                translatedCtxt (
                    mkSynCall
                        "TryFinally"
                        mTry
                        [
                            mkSynCall "Delay" mTry [ mkSynDelay innerComp.Range innerExpr ] ceenv.builderValName
                            mkSynDelay2 unwindExpr2
                        ]
                        ceenv.builderValName
                )
            )

        | SynExpr.Paren(range = m) -> error (Error(FSComp.SR.tcConstructIsAmbiguousInComputationExpression (), m))

        // In some cases the node produced by `mkSynCall "Zero" m []` may be discarded in the case
        // of implicit yields - for example "list { 1; 2 }" when each expression checks as an implicit yield.
        // If it is not discarded, the syntax node will later be checked and the existence/non-existence of the Zero method
        // will be checked/reported appropriately (though the error message won't mention computation expressions
        // like our other error messages for missing methods).
        | SynExpr.ImplicitZero m ->
            if
                (not ceenv.enableImplicitYield)
                && isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        m
                        ceenv.ad
                        "Zero"
                        ceenv.builderTy
                )
            then
                match ceenv.origComp with
                // builder { }
                //
                // The compiler inserts a dummy () in CheckExpressions.fs for
                // empty-bodied computation expressions. In this case, the user
                // has not actually written any "control construct" in the body,
                // and so we use a more specific error message for clarity.
                | SynExpr.Const(SynConst.Unit, mUnit) when
                    cenv.g.langVersion.SupportsFeature LanguageFeature.EmptyBodiedComputationExpressions
                    && Range.equals mUnit range0
                    ->
                    error (Error(FSComp.SR.tcEmptyBodyRequiresBuilderZeroMethod (), ceenv.mWhole))
                | _ -> error (Error(FSComp.SR.tcRequireBuilderMethod ("Zero"), m))

            Some(translatedCtxt (mkSynCall "Zero" m [] ceenv.builderValName))

        | OptionalSequential(JoinOrGroupJoinOrZipClause ceenv (_, _, _, _, _, mClause), _) when firstTry = CompExprTranslationPass.Initial ->

            // 'join' clauses preceded by 'let' and other constructs get processed by repackaging with a 'for' loop.
            let patvs, _env = varSpace.Force comp.Range
            let varSpaceExpr = mkExprForVarSpace mClause patvs
            let varSpacePat = mkPatForVarSpace mClause patvs

            let dataCompPrior =
                translatedCtxt (
                    TranslateComputationExpressionNoQueryOps
                        ceenv
                        (SynExpr.YieldOrReturn((true, false), varSpaceExpr, mClause, SynExprYieldOrReturnTrivia.Zero))
                )

            // Rebind using for ...
            let rebind =
                SynExpr.ForEach(
                    DebugPointAtFor.No,
                    DebugPointAtInOrTo.No,
                    SeqExprOnly false,
                    false,
                    varSpacePat,
                    dataCompPrior,
                    comp,
                    comp.Range
                )

            // Retry with the 'for' loop packaging. Set firstTry=false just in case 'join' processing fails
            TryTranslateComputationExpression ceenv CompExprTranslationPass.Subsequent q varSpace rebind id

        | OptionalSequential(CustomOperationClause ceenv (nm, _, opExpr, mClause, _), _) ->

            match q with
            | CustomOperationsMode.Denied -> error (Error(FSComp.SR.tcCustomOperationMayNotBeUsedHere (), opExpr.Range))
            | CustomOperationsMode.Allowed ->
                let patvs, _env = varSpace.Force comp.Range
                let varSpaceExpr = mkExprForVarSpace mClause patvs

                let dataCompPriorToOp =
                    let isYield = not (customOperationMaintainsVarSpaceUsingBind ceenv nm)

                    translatedCtxt (
                        TranslateComputationExpressionNoQueryOps
                            ceenv
                            (SynExpr.YieldOrReturn((isYield, false), varSpaceExpr, mClause, SynExprYieldOrReturnTrivia.Zero))
                    )

                // Now run the consumeCustomOpClauses
                Some(ConsumeCustomOpClauses ceenv comp q varSpace dataCompPriorToOp comp false mClause)

        | SynExpr.Sequential(sp, true, innerComp1, innerComp2, m, _) ->

            // Check for 'where x > y' and other mis-applications of infix operators. If detected, give a good error message, and just ignore innerComp1
            if ceenv.isQuery && checkForBinaryApp ceenv innerComp1 then
                Some(TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp2 translatedCtxt)

            else
                if ceenv.isQuery && not (innerComp1.IsArbExprAndThusAlreadyReportedError) then
                    match innerComp1 with
                    | SynExpr.JoinIn _ -> ()
                    | SynExpr.DoBang(trivia = { DoBangKeyword = m }) -> errorR (Error(FSComp.SR.tcBindMayNotBeUsedInQueries (), m))
                    | _ -> errorR (Error(FSComp.SR.tcUnrecognizedQueryOperator (), innerComp1.RangeOfFirstPortion))

                match
                    TryTranslateComputationExpression
                        ceenv
                        CompExprTranslationPass.Initial
                        CustomOperationsMode.Denied
                        varSpace
                        innerComp1
                        id
                with
                | Some c ->
                    // "cexpr; cexpr" is treated as builder.Combine(cexpr1, cexpr1)
                    let m1 = rangeForCombine innerComp1

                    if
                        isNil (
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                cenv
                                ceenv.env
                                m
                                ceenv.ad
                                "Combine"
                                ceenv.builderTy
                        )
                    then
                        error (Error(FSComp.SR.tcRequireBuilderMethod ("Combine"), m))

                    if
                        isNil (
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                cenv
                                ceenv.env
                                m
                                ceenv.ad
                                "Delay"
                                ceenv.builderTy
                        )
                    then
                        error (Error(FSComp.SR.tcRequireBuilderMethod ("Delay"), m))

                    let combineCall =
                        mkSynCall
                            "Combine"
                            m1
                            [
                                c
                                mkSynCall
                                    "Delay"
                                    m1
                                    [
                                        mkSynDelay innerComp2.Range (TranslateComputationExpressionNoQueryOps ceenv innerComp2)
                                    ]
                                    ceenv.builderValName
                            ]
                            ceenv.builderValName

                    Some(translatedCtxt combineCall)

                | None ->
                    // "do! expr; cexpr" is treated as { let! () = expr in cexpr }
                    match innerComp1 with
                    | SynExpr.DoBang(expr = rhsExpr; range = m) ->
                        let sp =
                            match sp with
                            | DebugPointAtSequential.SuppressExpr -> DebugPointAtBinding.NoneAtDo
                            | DebugPointAtSequential.SuppressBoth -> DebugPointAtBinding.NoneAtDo
                            | DebugPointAtSequential.SuppressStmt -> DebugPointAtBinding.Yes m
                            | DebugPointAtSequential.SuppressNeither -> DebugPointAtBinding.Yes m

                        Some(
                            TranslateComputationExpression
                                ceenv
                                CompExprTranslationPass.Initial
                                q
                                varSpace
                                (SynExpr.LetOrUseBang(
                                    sp,
                                    false,
                                    true,
                                    SynPat.Const(SynConst.Unit, rhsExpr.Range),
                                    rhsExpr,
                                    [],
                                    innerComp2,
                                    m,
                                    SynExprLetOrUseBangTrivia.Zero
                                ))
                                translatedCtxt
                        )

                    // "expr; cexpr" is treated as sequential execution
                    | _ ->
                        Some(
                            TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp2 (fun holeFill ->
                                let fillExpr =
                                    if ceenv.enableImplicitYield then
                                        // When implicit yields are enabled, then if the 'innerComp1' checks as type
                                        // 'unit' we interpret the expression as a sequential, and when it doesn't
                                        // have type 'unit' we interpret it as a 'Yield + Combine'.
                                        let combineExpr =
                                            let m1 = rangeForCombine innerComp1

                                            let implicitYieldExpr =
                                                mkSynCall "Yield" comp.Range [ innerComp1 ] ceenv.builderValName

                                            mkSynCall
                                                "Combine"
                                                m1
                                                [
                                                    implicitYieldExpr
                                                    mkSynCall "Delay" m1 [ mkSynDelay holeFill.Range holeFill ] ceenv.builderValName
                                                ]
                                                ceenv.builderValName

                                        SynExpr.SequentialOrImplicitYield(sp, innerComp1, holeFill, combineExpr, m)
                                    else
                                        SynExpr.Sequential(sp, true, innerComp1, holeFill, m, SynExprSequentialTrivia.Zero)

                                translatedCtxt fillExpr)
                        )

        | SynExpr.IfThenElse(guardExpr, thenComp, elseCompOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia) ->
            match elseCompOpt with
            | Some elseComp ->
                if ceenv.isQuery then
                    error (Error(FSComp.SR.tcIfThenElseMayNotBeUsedWithinQueries (), trivia.IfToThenRange))

                Some(
                    translatedCtxt (
                        SynExpr.IfThenElse(
                            guardExpr,
                            TranslateComputationExpressionNoQueryOps ceenv thenComp,
                            Some(TranslateComputationExpressionNoQueryOps ceenv elseComp),
                            spIfToThen,
                            isRecovery,
                            mIfToEndOfElseBranch,
                            trivia
                        )
                    )
                )
            | None ->
                let elseComp =
                    if
                        isNil (
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                cenv
                                ceenv.env
                                trivia.IfToThenRange
                                ceenv.ad
                                "Zero"
                                ceenv.builderTy
                        )
                    then
                        error (Error(FSComp.SR.tcRequireBuilderMethod ("Zero"), trivia.IfToThenRange))

                    mkSynCall "Zero" trivia.IfToThenRange [] ceenv.builderValName

                Some(
                    TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace thenComp (fun holeFill ->
                        translatedCtxt (
                            SynExpr.IfThenElse(guardExpr, holeFill, Some elseComp, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia)
                        ))
                )

        // 'let binds in expr'
        | SynExpr.LetOrUse(isRec, false, binds, innerComp, m, trivia) ->

            // For 'query' check immediately
            if ceenv.isQuery then
                match (List.map (BindingNormalization.NormalizeBinding ValOrMemberBinding cenv ceenv.env) binds) with
                | [ NormalizedBinding(_, SynBindingKind.Normal, false, false, _, _, _, _, _, _, _, _) ] when not isRec -> ()
                | normalizedBindings ->
                    let failAt m =
                        error (Error(FSComp.SR.tcNonSimpleLetBindingInQuery (), m))

                    match normalizedBindings with
                    | NormalizedBinding(mBinding = mBinding) :: _ -> failAt mBinding
                    | _ -> failAt m

            // Add the variables to the query variable space, on demand
            let varSpace =
                addVarsToVarSpace varSpace (fun mQueryOp env ->
                    // Normalize the bindings before detecting the bound variables
                    match (List.map (BindingNormalization.NormalizeBinding ValOrMemberBinding cenv env) binds) with
                    | [ NormalizedBinding(kind = SynBindingKind.Normal; shouldInline = false; isMutable = false; pat = pat) ] ->
                        // successful case
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                        let _, _, vspecs, envinner, _ =
                            TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv pat None TcTrueMatchClause.No

                        vspecs, envinner
                    | _ ->
                        // error case
                        error (Error(FSComp.SR.tcCustomOperationMayNotBeUsedInConjunctionWithNonSimpleLetBindings (), mQueryOp)))

            Some(
                TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill ->
                    translatedCtxt (SynExpr.LetOrUse(isRec, false, binds, holeFill, m, trivia)))
            )

        // 'use x = expr in expr'
        | SynExpr.LetOrUse(
            isUse = true
            bindings = [ SynBinding(kind = SynBindingKind.Normal; headPat = pat; expr = rhsExpr; debugPoint = spBind) ]
            body = innerComp
            trivia = { LetOrUseKeyword = mBind }) ->

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcUseMayNotBeUsedInQueries (), mBind))

            let innerCompRange = innerComp.Range

            let consumeExpr =
                SynExpr.MatchLambda(
                    false,
                    innerCompRange,
                    [
                        SynMatchClause(
                            pat,
                            None,
                            TranslateComputationExpressionNoQueryOps ceenv innerComp,
                            innerCompRange,
                            DebugPointAtTarget.Yes,
                            SynMatchClauseTrivia.Zero
                        )
                    ],
                    DebugPointAtBinding.NoneAtInvisible,
                    innerCompRange
                )

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mBind
                        ceenv.ad
                        "Using"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Using"), mBind))

            Some(
                translatedCtxt (mkSynCall "Using" mBind [ rhsExpr; consumeExpr ] ceenv.builderValName)
                |> addBindDebugPoint spBind
            )

        // 'let! pat = expr in expr'
        //    --> build.Bind(e1, (fun _argN -> match _argN with pat -> expr))
        //  or
        //    --> build.BindReturn(e1, (fun _argN -> match _argN with pat -> expr-without-return))
        | SynExpr.LetOrUseBang(
            bindDebugPoint = spBind
            isUse = false
            isFromSource = isFromSource
            pat = pat
            rhs = rhsExpr
            andBangs = []
            body = innerComp
            trivia = { LetOrUseBangKeyword = mBind }) ->

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcBindMayNotBeUsedInQueries (), mBind))

            // Add the variables to the query variable space, on demand
            let varSpace =
                addVarsToVarSpace varSpace (fun _mCustomOp env ->
                    use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                    let _, _, vspecs, envinner, _ =
                        TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv pat None TcTrueMatchClause.No

                    vspecs, envinner)

            let rhsExpr =
                mkSourceExprConditional isFromSource rhsExpr ceenv.sourceMethInfo ceenv.builderValName

            Some(
                TranslateComputationExpressionBind
                    ceenv
                    comp
                    q
                    varSpace
                    mBind
                    (addBindDebugPoint spBind)
                    "Bind"
                    [ rhsExpr ]
                    pat
                    innerComp
                    translatedCtxt
            )

        // 'use! pat = e1 in e2' --> build.Bind(e1, (function  _argN -> match _argN with pat -> build.Using(x, (fun _argN -> match _argN with pat -> e2))))
        | SynExpr.LetOrUseBang(
            bindDebugPoint = spBind
            isUse = true
            isFromSource = isFromSource
            pat = SynPat.Named(ident = SynIdent(id, _); isThisVal = false) as pat
            rhs = rhsExpr
            andBangs = []
            body = innerComp
            trivia = { LetOrUseBangKeyword = mBind })
        | SynExpr.LetOrUseBang(
            bindDebugPoint = spBind
            isUse = true
            isFromSource = isFromSource
            pat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ id ])) as pat
            rhs = rhsExpr
            andBangs = []
            body = innerComp
            trivia = { LetOrUseBangKeyword = mBind }) ->

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcBindMayNotBeUsedInQueries (), mBind))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mBind
                        ceenv.ad
                        "Using"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Using"), mBind))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mBind
                        ceenv.ad
                        "Bind"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Bind"), mBind))

            let bindExpr =
                let consumeExpr =
                    SynExpr.MatchLambda(
                        false,
                        mBind,
                        [
                            SynMatchClause(
                                pat,
                                None,
                                TranslateComputationExpressionNoQueryOps ceenv innerComp,
                                innerComp.Range,
                                DebugPointAtTarget.Yes,
                                SynMatchClauseTrivia.Zero
                            )
                        ],
                        DebugPointAtBinding.NoneAtInvisible,
                        mBind
                    )

                let consumeExpr =
                    mkSynCall "Using" mBind [ SynExpr.Ident id; consumeExpr ] ceenv.builderValName

                let consumeExpr =
                    SynExpr.MatchLambda(
                        false,
                        mBind,
                        [
                            SynMatchClause(pat, None, consumeExpr, id.idRange, DebugPointAtTarget.No, SynMatchClauseTrivia.Zero)
                        ],
                        DebugPointAtBinding.NoneAtInvisible,
                        mBind
                    )

                let rhsExpr =
                    mkSourceExprConditional isFromSource rhsExpr ceenv.sourceMethInfo ceenv.builderValName

                mkSynCall "Bind" mBind [ rhsExpr; consumeExpr ] ceenv.builderValName
                |> addBindDebugPoint spBind

            Some(translatedCtxt bindExpr)

        // 'use! pat = e1 ... in e2' where 'pat' is not a simple name -> error
        | SynExpr.LetOrUseBang(isUse = true; andBangs = andBangs; trivia = { LetOrUseBangKeyword = mBind }) ->
            if isNil andBangs then
                error (Error(FSComp.SR.tcInvalidUseBangBinding (), mBind))
            else
                let m =
                    match andBangs with
                    | [] -> comp.Range
                    | h :: _ -> h.Trivia.AndBangKeyword

                error (Error(FSComp.SR.tcInvalidUseBangBindingNoAndBangs (), m))

        // 'let! pat1 = expr1 and! pat2 = expr2 in ...' -->
        //     build.BindN(expr1, expr2, ...)
        // or
        //     build.BindNReturn(expr1, expr2, ...)
        // or
        //     build.Bind(build.MergeSources(expr1, expr2), ...)
        | SynExpr.LetOrUseBang(
            bindDebugPoint = spBind
            isUse = false
            isFromSource = isFromSource
            pat = letPat
            rhs = letRhsExpr
            andBangs = andBangBindings
            body = innerComp
            trivia = { LetOrUseBangKeyword = mBind }) ->
            if not (cenv.g.langVersion.SupportsFeature LanguageFeature.AndBang) then
                let andBangRange =
                    match andBangBindings with
                    | [] -> comp.Range
                    | h :: _ -> h.Trivia.AndBangKeyword

                error (Error(FSComp.SR.tcAndBangNotSupported (), andBangRange))

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcBindMayNotBeUsedInQueries (), mBind))

            let sources =
                (letRhsExpr
                 :: [ for SynExprAndBang(body = andExpr) in andBangBindings -> andExpr ])
                |> List.map (fun expr -> mkSourceExprConditional isFromSource expr ceenv.sourceMethInfo ceenv.builderValName)

            let pats =
                letPat :: [ for SynExprAndBang(pat = andPat) in andBangBindings -> andPat ]

            let sourcesRange = sources |> List.map (fun e -> e.Range) |> List.reduce unionRanges

            let numSources = sources.Length
            let bindReturnNName = "Bind" + string numSources + "Return"
            let bindNName = "Bind" + string numSources

            // Check if this is a Bind2Return etc.
            let hasBindReturnN =
                not (
                    isNil (
                        TryFindIntrinsicOrExtensionMethInfo
                            ResultCollectionSettings.AtMostOneResult
                            cenv
                            ceenv.env
                            mBind
                            ceenv.ad
                            bindReturnNName
                            ceenv.builderTy
                    )
                )

            if
                hasBindReturnN
                && Option.isSome (convertSimpleReturnToExpr ceenv comp varSpace innerComp)
            then
                let consumePat = SynPat.Tuple(false, pats, [], letPat.Range)

                // Add the variables to the query variable space, on demand
                let varSpace =
                    addVarsToVarSpace varSpace (fun _mCustomOp env ->
                        use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                        let _, _, vspecs, envinner, _ =
                            TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv consumePat None TcTrueMatchClause.No

                        vspecs, envinner)

                Some(
                    TranslateComputationExpressionBind
                        ceenv
                        comp
                        q
                        varSpace
                        mBind
                        (addBindDebugPoint spBind)
                        bindNName
                        sources
                        consumePat
                        innerComp
                        translatedCtxt
                )

            else

                // Check if this is a Bind2 etc.
                let hasBindN =
                    not (
                        isNil (
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                cenv
                                ceenv.env
                                mBind
                                ceenv.ad
                                bindNName
                                ceenv.builderTy
                        )
                    )

                if hasBindN then
                    let consumePat = SynPat.Tuple(false, pats, [], letPat.Range)

                    // Add the variables to the query variable space, on demand
                    let varSpace =
                        addVarsToVarSpace varSpace (fun _mCustomOp env ->
                            use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                            let _, _, vspecs, envinner, _ =
                                TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv consumePat None TcTrueMatchClause.No

                            vspecs, envinner)

                    Some(
                        TranslateComputationExpressionBind
                            ceenv
                            comp
                            q
                            varSpace
                            mBind
                            (addBindDebugPoint spBind)
                            bindNName
                            sources
                            consumePat
                            innerComp
                            translatedCtxt
                    )
                else

                    // Look for the maximum supported MergeSources, MergeSources3, ...
                    let mkMergeSourcesName n =
                        if n = 2 then
                            "MergeSources"
                        else
                            "MergeSources" + (string n)

                    let maxMergeSources =
                        let rec loop (n: int) =
                            let mergeSourcesName = mkMergeSourcesName n

                            if
                                isNil (
                                    TryFindIntrinsicOrExtensionMethInfo
                                        ResultCollectionSettings.AtMostOneResult
                                        cenv
                                        ceenv.env
                                        mBind
                                        ceenv.ad
                                        mergeSourcesName
                                        ceenv.builderTy
                                )
                            then
                                (n - 1)
                            else
                                loop (n + 1)

                        loop 2

                    if maxMergeSources = 1 then
                        error (Error(FSComp.SR.tcRequireMergeSourcesOrBindN (bindNName), mBind))

                    let rec mergeSources (sourcesAndPats: (SynExpr * SynPat) list) =
                        let numSourcesAndPats = sourcesAndPats.Length
                        assert (numSourcesAndPats <> 0)

                        if numSourcesAndPats = 1 then
                            sourcesAndPats[0]

                        elif numSourcesAndPats <= maxMergeSources then

                            // Call MergeSources2(e1, e2), MergeSources3(e1, e2, e3) etc
                            let mergeSourcesName = mkMergeSourcesName numSourcesAndPats

                            if
                                isNil (
                                    TryFindIntrinsicOrExtensionMethInfo
                                        ResultCollectionSettings.AtMostOneResult
                                        cenv
                                        ceenv.env
                                        mBind
                                        ceenv.ad
                                        mergeSourcesName
                                        ceenv.builderTy
                                )
                            then
                                error (Error(FSComp.SR.tcRequireMergeSourcesOrBindN (bindNName), mBind))

                            let source =
                                mkSynCall mergeSourcesName sourcesRange (List.map fst sourcesAndPats) ceenv.builderValName

                            let pat = SynPat.Tuple(false, List.map snd sourcesAndPats, [], letPat.Range)
                            source, pat

                        else

                            // Call MergeSourcesMax(e1, e2, e3, e4, (...))
                            let nowSourcesAndPats, laterSourcesAndPats =
                                List.splitAt (maxMergeSources - 1) sourcesAndPats

                            let mergeSourcesName = mkMergeSourcesName maxMergeSources

                            if
                                isNil (
                                    TryFindIntrinsicOrExtensionMethInfo
                                        ResultCollectionSettings.AtMostOneResult
                                        cenv
                                        ceenv.env
                                        mBind
                                        ceenv.ad
                                        mergeSourcesName
                                        ceenv.builderTy
                                )
                            then
                                error (Error(FSComp.SR.tcRequireMergeSourcesOrBindN (bindNName), mBind))

                            let laterSource, laterPat = mergeSources laterSourcesAndPats

                            let source =
                                mkSynCall
                                    mergeSourcesName
                                    sourcesRange
                                    (List.map fst nowSourcesAndPats @ [ laterSource ])
                                    ceenv.builderValName

                            let pat =
                                SynPat.Tuple(false, List.map snd nowSourcesAndPats @ [ laterPat ], [], letPat.Range)

                            source, pat

                    let mergedSources, consumePat = mergeSources (List.zip sources pats)

                    // Add the variables to the query variable space, on demand
                    let varSpace =
                        addVarsToVarSpace varSpace (fun _mCustomOp env ->
                            use _holder = TemporarilySuspendReportingTypecheckResultsToSink cenv.tcSink

                            let _, _, vspecs, envinner, _ =
                                TcMatchPattern cenv (NewInferenceType cenv.g) env ceenv.tpenv consumePat None TcTrueMatchClause.No

                            vspecs, envinner)

                    // Build the 'Bind' call
                    Some(
                        TranslateComputationExpressionBind
                            ceenv
                            comp
                            q
                            varSpace
                            mBind
                            (addBindDebugPoint spBind)
                            "Bind"
                            [ mergedSources ]
                            consumePat
                            innerComp
                            translatedCtxt
                    )

        | SynExpr.Match(spMatch, expr, clauses, m, trivia) ->
            if ceenv.isQuery then
                error (Error(FSComp.SR.tcMatchMayNotBeUsedWithQuery (), trivia.MatchKeyword))

            let clauses =
                clauses
                |> List.map (fun (SynMatchClause(pat, cond, innerComp, patm, sp, trivia)) ->
                    SynMatchClause(pat, cond, TranslateComputationExpressionNoQueryOps ceenv innerComp, patm, sp, trivia))

            Some(translatedCtxt (SynExpr.Match(spMatch, expr, clauses, m, trivia)))

        // 'match! expr with pats ...' --> build.Bind(e1, (function pats ...))
        // FUTURE: consider allowing translation to BindReturn
        | SynExpr.MatchBang(spMatch, expr, clauses, _m, trivia) ->
            let inputExpr = mkSourceExpr expr ceenv.sourceMethInfo ceenv.builderValName

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcMatchMayNotBeUsedWithQuery (), trivia.MatchBangKeyword))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        trivia.MatchBangKeyword
                        ceenv.ad
                        "Bind"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Bind"), trivia.MatchBangKeyword))

            let clauses =
                clauses
                |> List.map (fun (SynMatchClause(pat, cond, innerComp, patm, sp, trivia)) ->
                    SynMatchClause(pat, cond, TranslateComputationExpressionNoQueryOps ceenv innerComp, patm, sp, trivia))

            let consumeExpr =
                SynExpr.MatchLambda(false, trivia.MatchBangKeyword, clauses, DebugPointAtBinding.NoneAtInvisible, trivia.MatchBangKeyword)

            let callExpr =
                mkSynCall "Bind" trivia.MatchBangKeyword [ inputExpr; consumeExpr ] ceenv.builderValName
                |> addBindDebugPoint spMatch

            Some(translatedCtxt callExpr)

        | SynExpr.TryWith(innerComp, clauses, mTryToLast, spTry, spWith, trivia) ->
            let mTry =
                match spTry with
                | DebugPointAtTry.Yes _ -> trivia.TryKeyword.NoteSourceConstruct(NotedSourceConstruct.Try)
                | _ -> trivia.TryKeyword

            let spWith2 =
                match spWith with
                | DebugPointAtWith.Yes _ -> DebugPointAtBinding.Yes trivia.WithKeyword
                | _ -> DebugPointAtBinding.NoneAtInvisible

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcTryWithMayNotBeUsedInQueries (), mTry))

            let clauses =
                clauses
                |> List.map (fun (SynMatchClause(pat, cond, clauseComp, patm, sp, trivia)) ->
                    SynMatchClause(pat, cond, TranslateComputationExpressionNoQueryOps ceenv clauseComp, patm, sp, trivia))

            let consumeExpr =
                SynExpr.MatchLambda(true, mTryToLast, clauses, spWith2, mTryToLast)

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mTry
                        ceenv.ad
                        "TryWith"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("TryWith"), mTry))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        mTry
                        ceenv.ad
                        "Delay"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("Delay"), mTry))

            let innerExpr = TranslateComputationExpressionNoQueryOps ceenv innerComp

            let innerExpr =
                match spTry with
                | DebugPointAtTry.Yes _ -> SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes mTry, true, innerExpr)
                | _ -> innerExpr

            let callExpr =
                mkSynCall
                    "TryWith"
                    mTry
                    [
                        mkSynCall "Delay" mTry [ mkSynDelay2 innerExpr ] ceenv.builderValName
                        consumeExpr
                    ]
                    ceenv.builderValName

            Some(translatedCtxt callExpr)

        | SynExpr.YieldOrReturnFrom((true, _), synYieldExpr, _, { YieldOrReturnFromKeyword = m }) ->
            let yieldFromExpr =
                mkSourceExpr synYieldExpr ceenv.sourceMethInfo ceenv.builderValName

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        m
                        ceenv.ad
                        "YieldFrom"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("YieldFrom"), m))

            let yieldFromCall =
                mkSynCall "YieldFrom" synYieldExpr.Range [ yieldFromExpr ] ceenv.builderValName

            let yieldFromCall =
                if IsControlFlowExpression synYieldExpr then
                    yieldFromCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, yieldFromCall)

            Some(translatedCtxt yieldFromCall)

        | SynExpr.YieldOrReturnFrom((false, _), synReturnExpr, _, { YieldOrReturnFromKeyword = m }) ->
            let returnFromExpr =
                mkSourceExpr synReturnExpr ceenv.sourceMethInfo ceenv.builderValName

            if ceenv.isQuery then
                error (Error(FSComp.SR.tcReturnMayNotBeUsedInQueries (), m))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        m
                        ceenv.ad
                        "ReturnFrom"
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod ("ReturnFrom"), m))

            let returnFromCall =
                mkSynCall "ReturnFrom" synReturnExpr.Range [ returnFromExpr ] ceenv.builderValName

            let returnFromCall =
                if IsControlFlowExpression synReturnExpr then
                    returnFromCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, returnFromCall)

            Some(translatedCtxt returnFromCall)

        | SynExpr.YieldOrReturn((isYield, _), synYieldOrReturnExpr, _, { YieldOrReturnKeyword = m }) ->
            let methName = (if isYield then "Yield" else "Return")

            if ceenv.isQuery && not isYield then
                error (Error(FSComp.SR.tcReturnMayNotBeUsedInQueries (), m))

            if
                isNil (
                    TryFindIntrinsicOrExtensionMethInfo
                        ResultCollectionSettings.AtMostOneResult
                        cenv
                        ceenv.env
                        m
                        ceenv.ad
                        methName
                        ceenv.builderTy
                )
            then
                error (Error(FSComp.SR.tcRequireBuilderMethod methName, m))

            let yieldOrReturnCall =
                mkSynCall methName synYieldOrReturnExpr.Range [ synYieldOrReturnExpr ] ceenv.builderValName

            let yieldOrReturnCall =
                if IsControlFlowExpression synYieldOrReturnExpr then
                    yieldOrReturnCall
                else
                    SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, yieldOrReturnCall)

            Some(translatedCtxt yieldOrReturnCall)

        | _ -> None

and ConsumeCustomOpClauses
    (ceenv: ComputationExpressionContext<'a>)
    (comp: SynExpr)
    q
    (varSpace: LazyWithContext<_, _>)
    dataCompPrior
    compClausesExpr
    lastUsesBind
    mClause
    =

    // Substitute 'yield <var-space>' into the context

    let patvs, _env = varSpace.Force comp.Range
    let varSpaceSimplePat = mkSimplePatForVarSpace mClause patvs
    let varSpacePat = mkPatForVarSpace mClause patvs

    match compClausesExpr with

    // Detect one custom operation... This clause will always match at least once...
    | OptionalSequential(CustomOperationClause ceenv (nm, opDatas, opExpr, mClause, optionalIntoPat), optionalCont) ->

        let opName, _, _, _, _, _, _, _, methInfo = opDatas[0]

        let isLikeZip = customOperationIsLikeZip ceenv nm

        let isLikeJoin = customOperationIsLikeJoin ceenv nm

        let isLikeGroupJoin = customOperationIsLikeZip ceenv nm

        // Record the resolution of the custom operation for posterity
        let item =
            Item.CustomOperation(opName, (fun () -> customOpUsageText ceenv nm), Some methInfo)

        // FUTURE: consider whether we can do better than emptyTyparInst here, in order to display instantiations
        // of type variables in the quick info provided in the IDE.
        CallNameResolutionSink
            ceenv.cenv.tcSink
            (nm.idRange, ceenv.env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, ceenv.env.eAccessRights)

        if isLikeZip || isLikeJoin || isLikeGroupJoin then
            errorR (Error(FSComp.SR.tcBinaryOperatorRequiresBody (nm.idText, Option.get (customOpUsageText ceenv nm)), nm.idRange))

            match optionalCont with
            | None ->
                // we are about to drop the 'opExpr' AST on the floor. we've already reported an error. attempt to get name resolutions before dropping it
                RecordNameAndTypeResolutions ceenv.cenv ceenv.env ceenv.tpenv opExpr
                dataCompPrior
            | Some contExpr -> ConsumeCustomOpClauses ceenv comp q varSpace dataCompPrior contExpr lastUsesBind mClause
        else

            let maintainsVarSpace = customOperationMaintainsVarSpace ceenv nm

            let maintainsVarSpaceUsingBind = customOperationMaintainsVarSpaceUsingBind ceenv nm

            let expectedArgCount = tryExpectedArgCountForCustomOperator ceenv nm

            let dataCompAfterOp =
                match opExpr with
                | StripApps(SingleIdent nm, args) ->
                    let argCountsMatch =
                        match expectedArgCount with
                        | Some n -> n = args.Length
                        | None -> ceenv.cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations

                    if argCountsMatch then
                        // Check for the [<ProjectionParameter>] attribute on each argument position
                        let args =
                            args
                            |> List.mapi (fun i arg ->
                                if isCustomOperationProjectionParameter ceenv (i + 1) nm then
                                    SynExpr.Lambda(
                                        false,
                                        false,
                                        varSpaceSimplePat,
                                        arg,
                                        None,
                                        arg.Range.MakeSynthetic(),
                                        SynExprLambdaTrivia.Zero
                                    )
                                else
                                    arg)

                        mkSynCall methInfo.DisplayName mClause (dataCompPrior :: args) ceenv.builderValName
                    else
                        let expectedArgCount = defaultArg expectedArgCount 0

                        errorR (
                            Error(FSComp.SR.tcCustomOperationHasIncorrectArgCount (nm.idText, expectedArgCount, args.Length), nm.idRange)
                        )

                        mkSynCall
                            methInfo.DisplayName
                            mClause
                            ([ dataCompPrior ]
                             @ List.init expectedArgCount (fun i -> arbExpr ("_arg" + string i, mClause)))
                            ceenv.builderValName
                | _ -> failwith "unreachable"

            match optionalCont with
            | None ->
                match optionalIntoPat with
                | Some intoPat -> errorR (Error(FSComp.SR.tcIntoNeedsRestOfQuery (), intoPat.Range))
                | None -> ()

                dataCompAfterOp

            | Some contExpr ->

                // select a.Name into name; ...
                // distinct into d; ...
                //
                // Rebind the into pattern and process the rest of the clauses
                match optionalIntoPat with
                | Some intoPat ->
                    if not (customOperationAllowsInto ceenv nm) then
                        error (Error(FSComp.SR.tcOperatorDoesntAcceptInto (nm.idText), intoPat.Range))

                    // Rebind using either for ... or let!....
                    let rebind =
                        if maintainsVarSpaceUsingBind then
                            SynExpr.LetOrUseBang(
                                DebugPointAtBinding.NoneAtLet,
                                false,
                                false,
                                intoPat,
                                dataCompAfterOp,
                                [],
                                contExpr,
                                intoPat.Range,
                                SynExprLetOrUseBangTrivia.Zero
                            )
                        else
                            SynExpr.ForEach(
                                DebugPointAtFor.No,
                                DebugPointAtInOrTo.No,
                                SeqExprOnly false,
                                false,
                                intoPat,
                                dataCompAfterOp,
                                contExpr,
                                intoPat.Range
                            )

                    TranslateComputationExpression ceenv CompExprTranslationPass.Initial q ceenv.emptyVarSpace rebind id

                // select a.Name; ...
                // distinct; ...
                //
                // Process the rest of the clauses
                | None ->
                    if maintainsVarSpace || maintainsVarSpaceUsingBind then
                        ConsumeCustomOpClauses ceenv comp q varSpace dataCompAfterOp contExpr maintainsVarSpaceUsingBind mClause
                    else
                        ConsumeCustomOpClauses ceenv comp q ceenv.emptyVarSpace dataCompAfterOp contExpr false mClause

    // No more custom operator clauses in compClausesExpr, but there may be clauses like join, yield etc.
    // Bind/iterate the dataCompPrior and use compClausesExpr as the body.
    | _ ->
        // Rebind using either for ... or let!....
        let rebind =
            if lastUsesBind then
                SynExpr.LetOrUseBang(
                    DebugPointAtBinding.NoneAtLet,
                    false,
                    false,
                    varSpacePat,
                    dataCompPrior,
                    [],
                    compClausesExpr,
                    compClausesExpr.Range,
                    SynExprLetOrUseBangTrivia.Zero
                )
            else
                SynExpr.ForEach(
                    DebugPointAtFor.No,
                    DebugPointAtInOrTo.No,
                    SeqExprOnly false,
                    false,
                    varSpacePat,
                    dataCompPrior,
                    compClausesExpr,
                    compClausesExpr.Range
                )

        TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace rebind id

and TranslateComputationExpressionNoQueryOps ceenv comp =
    TranslateComputationExpression ceenv CompExprTranslationPass.Initial CustomOperationsMode.Denied ceenv.emptyVarSpace comp id

and TranslateComputationExpressionBind
    (ceenv: ComputationExpressionContext<'a>)
    comp
    q
    varSpace
    bindRange
    addBindDebugPoint
    bindName
    (bindArgs: SynExpr list)
    (consumePat: SynPat)
    (innerComp: SynExpr)
    translatedCtxt
    =

    let innerRange = innerComp.Range

    let innerCompReturn =
        if ceenv.cenv.g.langVersion.SupportsFeature LanguageFeature.AndBang then
            convertSimpleReturnToExpr ceenv comp varSpace innerComp
        else
            None

    match innerCompReturn with
    | Some(innerExpr, customOpInfo) when
        (let bindName = bindName + "Return"

         not (
             isNil (
                 TryFindIntrinsicOrExtensionMethInfo
                     ResultCollectionSettings.AtMostOneResult
                     ceenv.cenv
                     ceenv.env
                     bindRange
                     ceenv.ad
                     bindName
                     ceenv.builderTy
             )
         ))
        ->

        let bindName = bindName + "Return"

        // Build the `BindReturn` call
        let dataCompPriorToOp =
            let consumeExpr =
                SynExpr.MatchLambda(
                    false,
                    consumePat.Range,
                    [
                        SynMatchClause(consumePat, None, innerExpr, innerRange, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)
                    ],
                    DebugPointAtBinding.NoneAtInvisible,
                    innerRange
                )

            translatedCtxt (mkSynCall bindName bindRange (bindArgs @ [ consumeExpr ]) ceenv.builderValName)

        match customOpInfo with
        | None -> dataCompPriorToOp
        | Some(innerComp, mClause) ->
            // If the `BindReturn` was forced by a custom operation, continue to process the clauses of the CustomOp
            ConsumeCustomOpClauses ceenv comp q varSpace dataCompPriorToOp innerComp false mClause

    | _ ->

        if
            isNil (
                TryFindIntrinsicOrExtensionMethInfo
                    ResultCollectionSettings.AtMostOneResult
                    ceenv.cenv
                    ceenv.env
                    bindRange
                    ceenv.ad
                    bindName
                    ceenv.builderTy
            )
        then
            error (Error(FSComp.SR.tcRequireBuilderMethod (bindName), bindRange))

        // Build the `Bind` call
        TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace innerComp (fun holeFill ->
            let consumeExpr =
                SynExpr.MatchLambda(
                    false,
                    consumePat.Range,
                    [
                        SynMatchClause(consumePat, None, holeFill, innerRange, DebugPointAtTarget.Yes, SynMatchClauseTrivia.Zero)
                    ],
                    DebugPointAtBinding.NoneAtInvisible,
                    innerRange
                )

            let bindCall =
                mkSynCall bindName bindRange (bindArgs @ [ consumeExpr ]) ceenv.builderValName

            translatedCtxt (bindCall |> addBindDebugPoint))

/// This function is for desugaring into .Bind{N}Return calls if possible
/// The outer option indicates if .BindReturn is possible. When it returns None, .BindReturn cannot be used
/// The inner option indicates if a custom operation is involved inside
and convertSimpleReturnToExpr (ceenv: ComputationExpressionContext<'a>) comp varSpace innerComp =
    match innerComp with
    | SynExpr.YieldOrReturn((false, _), returnExpr, m, _) ->
        let returnExpr = SynExpr.DebugPoint(DebugPointAtLeafExpr.Yes m, false, returnExpr)
        Some(returnExpr, None)

    | SynExpr.Match(spMatch, expr, clauses, m, trivia) ->
        let clauses =
            clauses
            |> List.map (fun (SynMatchClause(pat, cond, innerComp2, patm, sp, trivia)) ->
                match convertSimpleReturnToExpr ceenv comp varSpace innerComp2 with
                | None -> None // failure
                | Some(_, Some _) -> None // custom op on branch = failure
                | Some(innerExpr2, None) -> Some(SynMatchClause(pat, cond, innerExpr2, patm, sp, trivia)))

        if clauses |> List.forall Option.isSome then
            Some(SynExpr.Match(spMatch, expr, (clauses |> List.map Option.get), m, trivia), None)
        else
            None

    | SynExpr.IfThenElse(guardExpr, thenComp, elseCompOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia) ->
        match convertSimpleReturnToExpr ceenv comp varSpace thenComp with
        | None -> None
        | Some(_, Some _) -> None
        | Some(thenExpr, None) ->
            let elseExprOptOpt =
                match elseCompOpt with
                // When we are missing an 'else' part alltogether in case of 'if cond then return exp', we fallback from BindReturn into regular Bind+Return
                | None -> None
                | Some elseComp ->
                    match convertSimpleReturnToExpr ceenv comp varSpace elseComp with
                    | None -> None // failure
                    | Some(_, Some _) -> None // custom op on branch = failure
                    | Some(elseExpr, None) -> Some(Some elseExpr)

            match elseExprOptOpt with
            | None -> None
            | Some elseExprOpt ->
                Some(SynExpr.IfThenElse(guardExpr, thenExpr, elseExprOpt, spIfToThen, isRecovery, mIfToEndOfElseBranch, trivia), None)

    | SynExpr.LetOrUse(isRec, false, binds, innerComp, m, trivia) ->
        match convertSimpleReturnToExpr ceenv comp varSpace innerComp with
        | None -> None
        | Some(_, Some _) -> None
        | Some(innerExpr, None) -> Some(SynExpr.LetOrUse(isRec, false, binds, innerExpr, m, trivia), None)

    | OptionalSequential(CustomOperationClause ceenv (nm, _, _, mClause, _), _) when customOperationMaintainsVarSpaceUsingBind ceenv nm ->

        let patvs, _env = varSpace.Force comp.Range
        let varSpaceExpr = mkExprForVarSpace mClause patvs

        Some(varSpaceExpr, Some(innerComp, mClause))

    | SynExpr.Sequential(sp, true, innerComp1, innerComp2, m, trivia) ->

        // Check the first part isn't a computation expression construct
        if (isSimpleExpr ceenv innerComp1) then
            // Check the second part is a simple return
            match convertSimpleReturnToExpr ceenv comp varSpace innerComp2 with
            | None -> None
            | Some(innerExpr2, optionalCont) -> Some(SynExpr.Sequential(sp, true, innerComp1, innerExpr2, m, trivia), optionalCont)
        else
            None

    | _ -> None

/// Check if an expression has no computation expression constructs
and isSimpleExpr ceenv comp =

    match comp with
    | ForEachThenJoinOrGroupJoinOrZipClause ceenv false _ -> false
    | SynExpr.ForEach _ -> false
    | SynExpr.For _ -> false
    | SynExpr.While _ -> false
    | SynExpr.WhileBang _ -> false
    | SynExpr.TryFinally _ -> false
    | SynExpr.ImplicitZero _ -> false
    | OptionalSequential(JoinOrGroupJoinOrZipClause ceenv _, _) -> false
    | OptionalSequential(CustomOperationClause ceenv _, _) -> false
    | SynExpr.Sequential(expr1 = innerComp1; expr2 = innerComp2) -> isSimpleExpr ceenv innerComp1 && isSimpleExpr ceenv innerComp2
    | SynExpr.IfThenElse(thenExpr = thenComp; elseExpr = elseCompOpt) ->
        isSimpleExpr ceenv thenComp
        && (match elseCompOpt with
            | None -> true
            | Some c -> isSimpleExpr ceenv c)
    | SynExpr.LetOrUse(body = innerComp) -> isSimpleExpr ceenv innerComp
    | SynExpr.LetOrUseBang _ -> false
    | SynExpr.Match(clauses = clauses) ->
        clauses
        |> List.forall (fun (SynMatchClause(resultExpr = innerComp)) -> isSimpleExpr ceenv innerComp)
    | SynExpr.MatchBang _ -> false
    | SynExpr.TryWith(tryExpr = innerComp; withCases = clauses) ->
        isSimpleExpr ceenv innerComp
        && clauses
           |> List.forall (fun (SynMatchClause(resultExpr = clauseComp)) -> isSimpleExpr ceenv clauseComp)
    | SynExpr.YieldOrReturnFrom _ -> false
    | SynExpr.YieldOrReturn _ -> false
    | SynExpr.DoBang _ -> false
    | _ -> true

and TranslateComputationExpression (ceenv: ComputationExpressionContext<'a>) firstTry q varSpace comp translatedCtxt =

    ceenv.cenv.stackGuard.Guard
    <| fun () ->
        match TryTranslateComputationExpression ceenv firstTry q varSpace comp translatedCtxt with
        | Some e -> e
        | None ->
            // This only occurs in final position in a sequence
            match comp with
            // "do! expr;" in final position is treated as { let! () = expr in return () } when Return is provided (and no Zero with Default attribute is available) or as { let! () = expr in zero } otherwise
            | SynExpr.DoBang(expr = rhsExpr; trivia = { DoBangKeyword = m }) ->
                let mUnit = rhsExpr.Range
                let rhsExpr = mkSourceExpr rhsExpr ceenv.sourceMethInfo ceenv.builderValName

                if ceenv.isQuery then
                    error (Error(FSComp.SR.tcBindMayNotBeUsedInQueries (), m))

                let bodyExpr =
                    if
                        isNil (
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                ceenv.cenv
                                ceenv.env
                                m
                                ceenv.ad
                                "Return"
                                ceenv.builderTy
                        )
                    then
                        SynExpr.ImplicitZero m
                    else
                        match
                            TryFindIntrinsicOrExtensionMethInfo
                                ResultCollectionSettings.AtMostOneResult
                                ceenv.cenv
                                ceenv.env
                                m
                                ceenv.ad
                                "Zero"
                                ceenv.builderTy
                        with
                        | minfo :: _ when MethInfoHasAttribute ceenv.cenv.g m ceenv.cenv.g.attrib_DefaultValueAttribute minfo ->
                            SynExpr.ImplicitZero m
                        | _ -> SynExpr.YieldOrReturn((false, true), SynExpr.Const(SynConst.Unit, m), m, SynExprYieldOrReturnTrivia.Zero)

                let letBangBind =
                    SynExpr.LetOrUseBang(
                        DebugPointAtBinding.NoneAtDo,
                        false,
                        false,
                        SynPat.Const(SynConst.Unit, mUnit),
                        rhsExpr,
                        [],
                        bodyExpr,
                        m,
                        SynExprLetOrUseBangTrivia.Zero
                    )

                TranslateComputationExpression ceenv CompExprTranslationPass.Initial q varSpace letBangBind translatedCtxt

            // "expr;" in final position is treated as { expr; zero }
            // Suppress the sequence point on the "zero"
            | _ ->
                // Check for 'where x > y' and other mis-applications of infix operators. If detected, give a good error message, and just ignore comp
                if ceenv.isQuery && checkForBinaryApp ceenv comp then
                    TranslateComputationExpression
                        ceenv
                        CompExprTranslationPass.Initial
                        q
                        varSpace
                        (SynExpr.ImplicitZero comp.Range)
                        translatedCtxt
                else
                    if ceenv.isQuery && not comp.IsArbExprAndThusAlreadyReportedError then
                        match comp with
                        | SynExpr.JoinIn _ -> () // an error will be reported later when we process innerComp1 as a sequential
                        | _ -> errorR (Error(FSComp.SR.tcUnrecognizedQueryOperator (), comp.RangeOfFirstPortion))

                    TranslateComputationExpression
                        ceenv
                        CompExprTranslationPass.Initial
                        q
                        varSpace
                        (SynExpr.ImplicitZero comp.Range)
                        (fun holeFill ->
                            let fillExpr =
                                if ceenv.enableImplicitYield then
                                    let implicitYieldExpr = mkSynCall "Yield" comp.Range [ comp ] ceenv.builderValName

                                    SynExpr.SequentialOrImplicitYield(
                                        DebugPointAtSequential.SuppressExpr,
                                        comp,
                                        holeFill,
                                        implicitYieldExpr,
                                        comp.Range
                                    )
                                else
                                    SynExpr.Sequential(
                                        DebugPointAtSequential.SuppressExpr,
                                        true,
                                        comp,
                                        holeFill,
                                        comp.Range,
                                        SynExprSequentialTrivia.Zero
                                    )

                            translatedCtxt fillExpr)

/// Used for all computation expressions except sequence expressions
let TcComputationExpression (cenv: TcFileState) env (overallTy: OverallTy) tpenv (mWhole, interpExpr: Expr, builderTy, comp: SynExpr) =
    let overallTy = overallTy.Commit

    let ad = env.eAccessRights

    let builderValName = CompilerGeneratedName "builder"
    let mBuilderVal = interpExpr.Range

    // Give bespoke error messages for the FSharp.Core "query" builder
    let isQuery =
        match stripDebugPoints interpExpr with
        // An unparameterized custom builder, e.g., `query`, `async`.
        | Expr.Val(vref, _, m)
        // A parameterized custom builder, e.g., `builder<…>`, `builder ()`.
        | Expr.App(funcExpr = Expr.Val(vref, _, m)) when not vref.IsMember || vref.IsConstructor ->
            let item = Item.CustomBuilder(vref.DisplayName, vref)
            CallNameResolutionSink cenv.tcSink (m, env.NameEnv, item, emptyTyparInst, ItemOccurrence.Use, env.eAccessRights)
            valRefEq cenv.g vref cenv.g.query_value_vref
        | _ -> false

    let sourceMethInfo =
        TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Source" builderTy

    /// Decide if the builder is an auto-quote builder
    let isAutoQuote = hasMethInfo "Quote" cenv env mBuilderVal ad builderTy

    let customOperationMethods =
        getCustomOperationMethods cenv env ad mBuilderVal builderTy

    /// Decide if the identifier represents a use of a custom query operator
    let hasCustomOperations =
        match customOperationMethods with
        | [] -> CustomOperationsMode.Denied
        | _ -> CustomOperationsMode.Allowed

    let customOperationMethodsIndexedByKeyword =
        if cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
            customOperationMethods
            |> Seq.groupBy (fun (nm, _, _, _, _, _, _, _, _) -> nm)
            |> Seq.map (fun (nm, group) -> (nm, Seq.toList group))
        else
            customOperationMethods
            |> Seq.groupBy (fun (nm, _, _, _, _, _, _, _, _) -> nm)
            |> Seq.map (fun (nm, group) -> (nm, Seq.toList group))
        |> dict

    // Check for duplicates by method name (keywords and method names must be 1:1)
    let customOperationMethodsIndexedByMethodName =
        if cenv.g.langVersion.SupportsFeature LanguageFeature.OverloadsForCustomOperations then
            customOperationMethods
            |> Seq.groupBy (fun (_, _, _, _, _, _, _, _, methInfo) -> methInfo.LogicalName)
            |> Seq.map (fun (nm, group) -> (nm, Seq.toList group))
        else
            customOperationMethods
            |> Seq.groupBy (fun (_, _, _, _, _, _, _, _, methInfo) -> methInfo.LogicalName)
            |> Seq.map (fun (nm, group) -> (nm, Seq.toList group))
        |> dict

    // If there are no 'yield' in the computation expression, and the builder supports 'Yield',
    // then allow the type-directed rule interpreting non-unit-typed expressions in statement
    // positions as 'yield'.  'yield!' may be present in the computation expression.
    let enableImplicitYield =
        cenv.g.langVersion.SupportsFeature LanguageFeature.ImplicitYield
        && (hasMethInfo "Yield" cenv env mBuilderVal ad builderTy
            && hasMethInfo "Combine" cenv env mBuilderVal ad builderTy
            && hasMethInfo "Delay" cenv env mBuilderVal ad builderTy
            && YieldFree cenv comp)

    let origComp = comp

    let ceenv =
        {
            cenv = cenv
            env = env
            tpenv = tpenv
            customOperationMethodsIndexedByKeyword = customOperationMethodsIndexedByKeyword
            customOperationMethodsIndexedByMethodName = customOperationMethodsIndexedByMethodName
            sourceMethInfo = sourceMethInfo
            builderValName = builderValName
            ad = ad
            builderTy = builderTy
            isQuery = isQuery
            enableImplicitYield = enableImplicitYield
            origComp = origComp
            mWhole = mWhole
            emptyVarSpace = LazyWithContext.NotLazy([], env)
        }

    /// Inside the 'query { ... }' use a modified name environment that contains fake 'CustomOperation' entries
    /// for all custom operations. This adds them to the completion lists and prevents them being used as values inside
    /// the query.
    let env =
        if List.isEmpty customOperationMethods then
            env
        else
            { env with
                eNameResEnv =
                    (env.eNameResEnv, customOperationMethods)
                    ||> Seq.fold (fun nenv (nm, _, _, _, _, _, _, _, methInfo) ->
                        AddFakeNameToNameEnv
                            nm
                            nenv
                            (Item.CustomOperation(nm, (fun () -> customOpUsageText ceenv (ident (nm, mBuilderVal))), Some methInfo)))
            }

    // Environment is needed for completions
    CallEnvSink cenv.tcSink (comp.Range, env.NameEnv, ad)

    let ceenv = { ceenv with env = env }

    let basicSynExpr =
        TranslateComputationExpression ceenv CompExprTranslationPass.Initial hasCustomOperations (LazyWithContext.NotLazy([], env)) comp id

    let mDelayOrQuoteOrRun =
        mBuilderVal
            .NoteSourceConstruct(NotedSourceConstruct.DelayOrQuoteOrRun)
            .MakeSynthetic()

    // Add a call to 'Delay' if the method is present
    let delayedExpr =
        match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Delay" builderTy with
        | [] -> basicSynExpr
        | _ -> mkSynCall "Delay" mDelayOrQuoteOrRun [ (mkSynDelay2 basicSynExpr) ] builderValName

    // Add a call to 'Quote' if the method is present
    let quotedSynExpr =
        if isAutoQuote then
            SynExpr.Quote(mkSynIdGet mDelayOrQuoteOrRun (CompileOpName "<@ @>"), false, delayedExpr, true, mWhole)
        else
            delayedExpr

    // Add a call to 'Run' if the method is present
    let runExpr =
        match TryFindIntrinsicOrExtensionMethInfo ResultCollectionSettings.AtMostOneResult cenv env mBuilderVal ad "Run" builderTy with
        | [] -> quotedSynExpr
        | _ -> mkSynCall "Run" mDelayOrQuoteOrRun [ quotedSynExpr ] builderValName

    let lambdaExpr =
        SynExpr.Lambda(
            false,
            false,
            SynSimplePats.SimplePats([ mkSynSimplePatVar false (mkSynId mBuilderVal builderValName) ], [], mBuilderVal),
            runExpr,
            None,
            mBuilderVal,
            SynExprLambdaTrivia.Zero
        )

    let env =
        match comp with
        | SynExpr.YieldOrReturn(flags = (true, _)) ->
            { env with
                eContextInfo = ContextInfo.YieldInComputationExpression
            }
        | SynExpr.YieldOrReturn(flags = (_, true)) ->
            { env with
                eContextInfo = ContextInfo.ReturnInComputationExpression
            }
        | _ -> env

    let lambdaExpr, tpenv =
        TcExpr cenv (MustEqual(mkFunTy cenv.g builderTy overallTy)) env tpenv lambdaExpr

    // beta-var-reduce to bind the builder using a 'let' binding
    let coreExpr =
        mkApps cenv.g ((lambdaExpr, tyOfExpr cenv.g lambdaExpr), [], [ interpExpr ], mBuilderVal)

    coreExpr, tpenv
