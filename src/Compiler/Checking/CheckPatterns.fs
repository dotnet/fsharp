// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The typechecker. Left-to-right constrained type checking
/// with generalization at appropriate points.
module internal FSharp.Compiler.CheckPatterns

open System
open System.Collections.Generic

open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

type cenv = TcFileState

//-------------------------------------------------------------------------
// Helpers that should be elsewhere
//-------------------------------------------------------------------------

let mkNilListPat (g: TcGlobals) m ty = TPat_unioncase(g.nil_ucref, [ty], [], m)

let mkConsListPat (g: TcGlobals) ty ph pt = TPat_unioncase(g.cons_ucref, [ty], [ph;pt], unionRanges ph.Range pt.Range)

/// Optimized unification routine that avoids creating new inference
/// variables unnecessarily
let UnifyRefTupleType contextInfo (cenv: cenv) denv m ty ps =
    let g = cenv.g
    let ptys =
        if isRefTupleTy g ty then
            let ptys = destRefTupleTy g ty
            if List.length ps = List.length ptys then ptys
            else NewInferenceTypes g ps
        else NewInferenceTypes g ps

    let contextInfo =
        match contextInfo with
        | ContextInfo.RecordFields -> ContextInfo.TupleInRecordFields
        | _ -> contextInfo

    AddCxTypeEqualsType contextInfo denv cenv.css m ty (TType_tuple (tupInfoRef, ptys))
    ptys

let rec TryAdjustHiddenVarNameToCompGenName cenv env (id: Ident) altNameRefCellOpt =
    match altNameRefCellOpt with
    | Some ({contents = SynSimplePatAlternativeIdInfo.Undecided altId } as altNameRefCell) ->
        match ResolvePatternLongIdent cenv.tcSink cenv.nameResolver AllIdsOK false id.idRange env.eAccessRights env.eNameResEnv TypeNameResolutionInfo.Default [id] with
        | Item.NewDef _ ->
            // The name is not in scope as a pattern identifier (e.g. union case), so do not use the alternate ID
            None
        | _ ->
            // The name is in scope as a pattern identifier, so use the alternate ID
            altNameRefCell.Value <- SynSimplePatAlternativeIdInfo.Decided altId
            Some altId
    | Some {contents = SynSimplePatAlternativeIdInfo.Decided altId } -> Some altId
    | None -> None

/// Bind the patterns used in a lambda. Not clear why we don't use TcPat.
and TcSimplePat optionalArgsOK checkConstraints (cenv: cenv) ty env patEnv p =
    let g = cenv.g
    let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv

    match p with
    | SynSimplePat.Id (id, altNameRefCellOpt, isCompGen, isMemberThis, isOpt, m) ->

        // Check to see if pattern translation decides to use an alternative identifier.
        match TryAdjustHiddenVarNameToCompGenName cenv env id altNameRefCellOpt with
        | Some altId ->
            TcSimplePat optionalArgsOK checkConstraints cenv ty env patEnv (SynSimplePat.Id (altId, None, isCompGen, isMemberThis, isOpt, m) )
        | None ->
            if isOpt then
                if not optionalArgsOK then
                    errorR(Error(FSComp.SR.tcOptionalArgsOnlyOnMembers(), m))

                let tyarg = NewInferenceType g
                UnifyTypes cenv env m ty (mkOptionTy g tyarg)

            let vFlags = TcPatValFlags (ValInline.Optional, permitInferTypars, noArgOrRetAttribs, false, None, isCompGen)
            let _, names, takenNames = TcPatBindingName cenv env id ty isMemberThis None None vFlags (names, takenNames)
            let patEnvR = TcPatLinearEnv(tpenv, names, takenNames)
            id.idText, patEnvR

    | SynSimplePat.Typed (p, cty, m) ->
        let ctyR, tpenv = TcTypeAndRecover cenv NewTyparsOK checkConstraints ItemOccurence.UseInType WarnOnIWSAM.Yes env tpenv cty

        match p with
        // Optional arguments on members
        | SynSimplePat.Id(_, _, _, _, true, _) -> UnifyTypes cenv env m ty (mkOptionTy g ctyR)
        | _ -> UnifyTypes cenv env m ty ctyR

        let patEnvR = TcPatLinearEnv(tpenv, names, takenNames)
        TcSimplePat optionalArgsOK checkConstraints cenv ty env patEnvR p

    | SynSimplePat.Attrib (p, _, _) ->
        TcSimplePat optionalArgsOK checkConstraints cenv ty env patEnv p

// raise an error if any optional args precede any non-optional args
and ValidateOptArgOrder (synSimplePats: SynSimplePats) =

    let rec getPats synSimplePats =
        match synSimplePats with
        | SynSimplePats.SimplePats(p, m) -> p, m
        | SynSimplePats.Typed(p, _, _) -> getPats p

    let rec isOptArg pat =
        match pat with
        | SynSimplePat.Id (_, _, _, _, isOpt, _) -> isOpt
        | SynSimplePat.Typed (p, _, _) -> isOptArg p
        | SynSimplePat.Attrib (p, _, _) -> isOptArg p

    let pats, m = getPats synSimplePats

    let mutable hitOptArg = false

    List.iter (fun pat -> if isOptArg pat then hitOptArg <- true elif hitOptArg then error(Error(FSComp.SR.tcOptionalArgsMustComeAfterNonOptionalArgs(), m))) pats


/// Bind the patterns used in argument position for a function, method or lambda.
and TcSimplePats (cenv: cenv) optionalArgsOK checkConstraints ty env patEnv synSimplePats =

    let g = cenv.g
    let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv

    // validate optional argument declaration
    ValidateOptArgOrder synSimplePats

    match synSimplePats with
    | SynSimplePats.SimplePats ([], m) ->
        // Unit "()" patterns in argument position become SynSimplePats.SimplePats([], _) in the
        // syntactic translation when building bindings. This is done because the
        // use of "()" has special significance for arity analysis and argument counting.
        //
        // Here we give a name to the single argument implied by those patterns.
        // This is a little awkward since it would be nice if this was
        // uniform with the process where we give names to other (more complex)
        // patterns used in argument position, e.g. "let f (D(x)) = ..."
        let id = ident("unitVar" + string takenNames.Count, m)
        UnifyTypes cenv env m ty g.unit_ty
        let vFlags = TcPatValFlags (ValInline.Optional, permitInferTypars, noArgOrRetAttribs, false, None, true)
        let _, namesR, takenNamesR = TcPatBindingName cenv env id ty false None None vFlags (names, takenNames)
        let patEnvR = TcPatLinearEnv(tpenv, namesR, takenNamesR)
        [id.idText], patEnvR

    | SynSimplePats.SimplePats ([synSimplePat], _) ->
        let v, patEnv = TcSimplePat optionalArgsOK checkConstraints cenv ty env patEnv synSimplePat
        [v], patEnv

    | SynSimplePats.SimplePats (ps, m) ->
        let ptys = UnifyRefTupleType env.eContextInfo cenv env.DisplayEnv m ty ps
        let ps', patEnvR = (patEnv, List.zip ptys ps) ||> List.mapFold (fun patEnv (ty, pat) -> TcSimplePat optionalArgsOK checkConstraints cenv ty env patEnv pat)
        ps', patEnvR

    | SynSimplePats.Typed (p, cty, m) ->
        let ctyR, tpenv = TcTypeAndRecover cenv NewTyparsOK CheckCxs ItemOccurence.UseInType WarnOnIWSAM.Yes env tpenv cty

        match p with
        // Solitary optional arguments on members
        | SynSimplePats.SimplePats([SynSimplePat.Id(_, _, _, _, true, _)], _) -> UnifyTypes cenv env m ty (mkOptionTy g ctyR)
        | _ -> UnifyTypes cenv env m ty ctyR

        let patEnvR = TcPatLinearEnv(tpenv, names, takenNames)

        TcSimplePats cenv optionalArgsOK checkConstraints ty env patEnvR p

and TcSimplePatsOfUnknownType (cenv: cenv) optionalArgsOK checkConstraints env tpenv synSimplePats =
    let g = cenv.g
    let argTy = NewInferenceType g
    let patEnv = TcPatLinearEnv (tpenv, NameMap.empty, Set.empty)
    TcSimplePats cenv optionalArgsOK checkConstraints argTy env patEnv synSimplePats

and TcPatBindingName cenv env id ty isMemberThis vis1 valReprInfo (vFlags: TcPatValFlags) (names, takenNames: Set<string>) =
    let (TcPatValFlags(inlineFlag, declaredTypars, argAttribs, isMutable, vis2, isCompGen)) = vFlags
    let vis = if Option.isSome vis1 then vis1 else vis2

    if takenNames.Contains id.idText then errorR (VarBoundTwice id)

    let isCompGen = isCompGen || IsCompilerGeneratedName id.idText
    let baseOrThis = if isMemberThis then MemberThisVal else NormalVal
    let prelimVal = PrelimVal1(id, declaredTypars, ty, valReprInfo, None, isMutable, inlineFlag, baseOrThis, argAttribs, vis, isCompGen)
    let names = Map.add id.idText prelimVal names
    let takenNames = Set.add id.idText takenNames

    let phase2 (TcPatPhase2Input (values, isLeftMost)) =
        let vspec, typeScheme =
            let name = id.idText
            match values.TryGetValue name with
            | true, value ->
                if not (String.IsNullOrEmpty name) && not (String.isLeadingIdentifierCharacterUpperCase name) then
                    match env.eNameResEnv.ePatItems.TryGetValue name with
                    | true, Item.Value vref when vref.LiteralValue.IsSome ->
                        warning(Error(FSComp.SR.checkLowercaseLiteralBindingInPattern name, id.idRange))
                    | _ -> ()
                value
            | _ -> error(Error(FSComp.SR.tcNameNotBoundInPattern name, id.idRange))

        // isLeftMost indicates we are processing the left-most path through a disjunctive or pattern.
        // For those binding locations, CallNameResolutionSink is called in MakeAndPublishValue, like all other bindings
        // For non-left-most paths, we register the name resolutions here
        if not isLeftMost && not vspec.IsCompilerGenerated && not (vspec.LogicalName.StartsWithOrdinal("_")) then
            let item = Item.Value(mkLocalValRef vspec)
            CallNameResolutionSink cenv.tcSink (id.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights)

        PatternValBinding(vspec, typeScheme)

    phase2, names, takenNames

and TcPatAndRecover warnOnUpper cenv (env: TcEnv) valReprInfo (vFlags: TcPatValFlags) patEnv ty (synPat: SynPat) =
    try
       TcPat warnOnUpper cenv env valReprInfo vFlags patEnv ty synPat
    with e ->
        // Error recovery - return some rubbish expression, but replace/annotate
        // the type of the current expression with a type variable that indicates an error
        let m = synPat.Range
        errorRecovery e m
        //SolveTypeAsError cenv env.DisplayEnv m ty
        (fun _ -> TPat_error m), patEnv

/// Typecheck a pattern. Patterns are type-checked in three phases:
/// 1. TcPat builds a List.map from simple variable names to inferred types for
///   those variables. It also returns a function to perform the second phase.
/// 2. The second phase assumes the caller has built the actual value_spec's
///    for the values being defined, and has decided if the types of these
///    variables are to be generalized. The caller hands this information to
///    the second-phase function in terms of a List.map from names to actual
///    value specifications.
and TcPat warnOnUpper (cenv: cenv) env valReprInfo vFlags (patEnv: TcPatLinearEnv) ty synPat =
    let g = cenv.g
    let ad = env.AccessRights

    match synPat with
    | SynPat.As (_, SynPat.Named _, _) -> ()
    | SynPat.As (_, _, m) -> checkLanguageFeatureError g.langVersion LanguageFeature.NonVariablePatternsToRightOfAsPatterns m
    | _ -> ()

    match synPat with
    | SynPat.Const (synConst, m) ->
        TcConstPat warnOnUpper cenv env vFlags patEnv ty synConst m

    | SynPat.Wild m ->
        (fun _ -> TPat_wild m), patEnv

    | SynPat.IsInst (synTargetTy, m)
    | SynPat.As (SynPat.IsInst(synTargetTy, m), _, _) ->
        TcPatIsInstance warnOnUpper cenv env valReprInfo vFlags patEnv ty synPat synTargetTy m

    | SynPat.As (synInnerPat, SynPat.Named (SynIdent(id,_), isMemberThis, vis, m), _)
    | SynPat.As (SynPat.Named (SynIdent(id,_), isMemberThis, vis, m), synInnerPat, _) ->
        TcPatNamedAs warnOnUpper cenv env valReprInfo vFlags patEnv ty synInnerPat id isMemberThis vis m

    | SynPat.As (pat1, pat2, m) ->
        TcPatUnnamedAs warnOnUpper cenv env vFlags patEnv ty pat1 pat2 m
        
    | SynPat.Named (SynIdent(id,_), isMemberThis, vis, m) ->
        TcPatNamed warnOnUpper cenv env vFlags patEnv id ty isMemberThis vis valReprInfo m

    | SynPat.OptionalVal (id, m) ->
        errorR (Error (FSComp.SR.tcOptionalArgsOnlyOnMembers (), m))
        let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv
        let bindf, namesR, takenNamesR = TcPatBindingName cenv env id ty false None valReprInfo vFlags (names, takenNames)
        let patEnvR = TcPatLinearEnv(tpenv, namesR, takenNamesR)
        (fun values -> TPat_as (TPat_wild m, bindf values, m)), patEnvR

    | SynPat.Typed (p, cty, m) ->
        let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv
        let ctyR, tpenvR = TcTypeAndRecover cenv NewTyparsOK CheckCxs ItemOccurence.UseInType WarnOnIWSAM.Yes env tpenv cty
        UnifyTypes cenv env m ty ctyR
        let patEnvR = TcPatLinearEnv(tpenvR, names, takenNames)
        TcPat warnOnUpper cenv env valReprInfo vFlags patEnvR ty p

    | SynPat.Attrib (innerPat, attrs, _) ->
        TcPatAttributed warnOnUpper cenv env vFlags patEnv ty innerPat attrs

    | SynPat.Or (pat1, pat2, m, _) ->
        TcPatOr warnOnUpper cenv env vFlags patEnv ty pat1 pat2 m

    | SynPat.ListCons(pat1, pat2, m, trivia) ->
        let longDotId = SynLongIdent((mkSynCaseName trivia.ColonColonRange opNameCons), [], [Some (FSharp.Compiler.SyntaxTrivia.IdentTrivia.OriginalNotation "::")])
        let args = SynArgPats.Pats [ SynPat.Tuple(false, [ pat1; pat2 ], m) ]
        TcPatLongIdent warnOnUpper cenv env ad valReprInfo vFlags patEnv ty (longDotId, None, args, None, m)

    | SynPat.Ands (pats, m) ->
        TcPatAnds warnOnUpper cenv env vFlags patEnv ty pats m

    | SynPat.LongIdent (longDotId=longDotId; typarDecls=tyargs; argPats=args; accessibility=vis; range=m) ->
        TcPatLongIdent warnOnUpper cenv env ad valReprInfo vFlags patEnv ty (longDotId, tyargs, args, vis, m)

    | SynPat.QuoteExpr(_, m) ->
        errorR (Error(FSComp.SR.tcInvalidPattern(), m))
        (fun _ -> TPat_error m), patEnv

    | SynPat.Tuple (isExplicitStruct, args, m) ->
        TcPatTuple warnOnUpper cenv env vFlags patEnv ty isExplicitStruct args m

    | SynPat.Paren (p, _) ->
        TcPat warnOnUpper cenv env None vFlags patEnv ty p

    | SynPat.ArrayOrList (isArray, args, m) ->
        TcPatArrayOrList warnOnUpper cenv env vFlags patEnv ty isArray args m

    | SynPat.Record (flds, m) ->
        TcRecordPat warnOnUpper cenv env vFlags patEnv ty flds m

    | SynPat.DeprecatedCharRange (c1, c2, m) ->
        errorR(Deprecated(FSComp.SR.tcUseWhenPatternGuard(), m))
        UnifyTypes cenv env m ty g.char_ty
        (fun _ -> TPat_range(c1, c2, m)), patEnv

    | SynPat.Null m ->
        TcNullPat cenv env patEnv ty m

    | SynPat.InstanceMember (range=m) ->
        errorR(Error(FSComp.SR.tcIllegalPattern(), synPat.Range))
        (fun _ -> TPat_wild m), patEnv

    | SynPat.FromParseError (pat, _) ->
        suppressErrorReporting (fun () -> TcPatAndRecover warnOnUpper cenv env valReprInfo vFlags patEnv (NewErrorType()) pat)

and TcConstPat warnOnUpper cenv env vFlags patEnv ty synConst m =
    let g = cenv.g
    match synConst with
    | SynConst.Bytes (bytes, _, m) ->
        UnifyTypes cenv env m ty (mkByteArrayTy g)
        let synReplacementExpr = SynPat.ArrayOrList (true, [ for b in bytes -> SynPat.Const(SynConst.Byte b, m) ], m)
        TcPat warnOnUpper cenv env None vFlags patEnv ty synReplacementExpr

    | SynConst.UserNum _ ->
        errorR (Error (FSComp.SR.tcInvalidNonPrimitiveLiteralInPatternMatch (), m))
        (fun _ -> TPat_error m), patEnv

    | _ ->
        try
            let c = TcConst cenv ty m env synConst
            (fun _ -> TPat_const (c, m)), patEnv
        with e ->
            errorRecovery e m
            (fun _ -> TPat_error m), patEnv

and TcPatNamedAs warnOnUpper cenv env valReprInfo vFlags patEnv ty synInnerPat id isMemberThis vis m =
    let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv
    let bindf, namesR, takenNamesR = TcPatBindingName cenv env id ty isMemberThis vis valReprInfo vFlags (names, takenNames)
    let patEnvR = TcPatLinearEnv(tpenv, namesR, takenNamesR)
    let innerPat, acc = TcPat warnOnUpper cenv env None vFlags patEnvR ty synInnerPat
    let phase2 values = TPat_as (innerPat values, bindf values, m)
    phase2, acc

and TcPatUnnamedAs warnOnUpper cenv env vFlags patEnv ty pat1 pat2 m =
    let pats = [pat1; pat2]
    let patsR, patEnvR = TcPatterns warnOnUpper cenv env vFlags patEnv (List.map (fun _ -> ty) pats) pats
    let phase2 values = TPat_conjs(List.map (fun f -> f values) patsR, m)
    phase2, patEnvR

and TcPatNamed warnOnUpper cenv env vFlags patEnv id ty isMemberThis vis valReprInfo m =
    let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv
    let bindf, namesR, takenNamesR = TcPatBindingName cenv env id ty isMemberThis vis valReprInfo vFlags (names, takenNames)
    let patEnvR = TcPatLinearEnv(tpenv, namesR, takenNamesR)
    let pat', acc = TcPat warnOnUpper cenv env None vFlags patEnvR ty (SynPat.Wild m)
    let phase2 values = TPat_as (pat' values, bindf values, m)
    phase2, acc

and TcPatIsInstance warnOnUpper cenv env valReprInfo vFlags patEnv srcTy synPat synTargetTy m =
    let (TcPatLinearEnv(tpenv, names, takenNames)) = patEnv
    let tgtTy, tpenv = TcTypeAndRecover cenv NewTyparsOKButWarnIfNotRigid CheckCxs ItemOccurence.UseInType WarnOnIWSAM.Yes env tpenv synTargetTy
    TcRuntimeTypeTest false true cenv env.DisplayEnv m tgtTy srcTy
    let patEnv = TcPatLinearEnv(tpenv, names, takenNames)
    match synPat with
    | SynPat.IsInst(_, m) ->
        (fun _ -> TPat_isinst (srcTy, tgtTy, None, m)), patEnv
    | SynPat.As (SynPat.IsInst _, p, m) ->
        let pat, acc = TcPat warnOnUpper cenv env valReprInfo vFlags patEnv tgtTy p
        (fun values -> TPat_isinst (srcTy, tgtTy, Some (pat values), m)), acc
    | _ -> failwith "TcPat"

and TcPatAttributed warnOnUpper cenv env vFlags patEnv ty innerPat attrs =
    errorR (Error (FSComp.SR.tcAttributesInvalidInPatterns (), rangeOfNonNilAttrs attrs))
    for attrList in attrs do
        TcAttributes cenv env Unchecked.defaultof<_> attrList.Attributes |> ignore
    TcPat warnOnUpper cenv env None vFlags patEnv ty innerPat

and TcPatOr warnOnUpper cenv env vFlags patEnv ty pat1 pat2 m =
    let (TcPatLinearEnv(_, names, takenNames)) = patEnv
    let pat1R, patEnv1 = TcPat warnOnUpper cenv env None vFlags patEnv ty pat1
    let (TcPatLinearEnv(tpenv, names1, takenNames1)) = patEnv1
    let pat2R, patEnv2 = TcPat warnOnUpper cenv env None vFlags (TcPatLinearEnv(tpenv, names, takenNames)) ty pat2
    let (TcPatLinearEnv(tpenv, names2, takenNames2)) = patEnv2

    if not (takenNames1 = takenNames2) then
        errorR (UnionPatternsBindDifferentNames m)

    names1 |> Map.iter (fun _ (PrelimVal1 (id=id1; prelimType=ty1)) ->
        match names2.TryGetValue id1.idText with
        | true, PrelimVal1 (id=id2; prelimType=ty2) ->
            try UnifyTypes cenv env id2.idRange ty1 ty2
            with exn -> errorRecovery exn m
        | _ -> ())

    let namesR = NameMap.layer names1 names2
    let takenNamesR = Set.union takenNames1 takenNames2
    let patEnvR = TcPatLinearEnv(tpenv, namesR, takenNamesR)
    let phase2 values = TPat_disjs ([pat1R values; pat2R (values.WithRightPath())], m)
    phase2, patEnvR

and TcPatAnds warnOnUpper cenv env vFlags patEnv ty pats m =
    let patsR, acc = TcPatterns warnOnUpper cenv env vFlags patEnv (List.map (fun _ -> ty) pats) pats
    let phase2 values = TPat_conjs(List.map (fun f -> f values) patsR, m)
    phase2, acc

and TcPatTuple warnOnUpper cenv env vFlags patEnv ty isExplicitStruct args m =
    let g = cenv.g
    try
        CheckTupleIsCorrectLength g env m ty args (fun argTys -> TcPatterns warnOnUpper cenv env vFlags patEnv argTys args |> ignore)

        let tupInfo, argTys = UnifyTupleTypeAndInferCharacteristics env.eContextInfo cenv env.DisplayEnv m ty isExplicitStruct args
        let argsR, acc = TcPatterns warnOnUpper cenv env vFlags patEnv argTys args
        let phase2 values = TPat_tuple(tupInfo, List.map (fun f -> f values) argsR, argTys, m)
        phase2, acc
    with e ->
        errorRecovery e m
        let _, acc = TcPatterns warnOnUpper cenv env vFlags patEnv (NewInferenceTypes g args) args
        let phase2 _ = TPat_error m
        phase2, acc

and TcPatArrayOrList warnOnUpper cenv env vFlags patEnv ty isArray args m =
    let g = cenv.g
    let argTy = NewInferenceType g
    UnifyTypes cenv env m ty (if isArray then mkArrayType g argTy else mkListTy g argTy)
    let argsR, acc = TcPatterns warnOnUpper cenv env vFlags patEnv (List.map (fun _ -> argTy) args) args
    let phase2 values =
        let argsR = List.map (fun f -> f values) argsR
        if isArray then TPat_array(argsR, argTy, m)
        else List.foldBack (mkConsListPat g argTy) argsR (mkNilListPat g m argTy)
    phase2, acc

and TcRecordPat warnOnUpper cenv env vFlags patEnv ty fieldPats m =
    let fieldPats = fieldPats |> List.map (fun (fieldId, _, fieldPat) -> fieldId, fieldPat)
    let tinst, tcref, fldsmap, _fldsList = BuildFieldMap cenv env true ty fieldPats m
    let gtyp = mkAppTy tcref tinst
    let inst = List.zip (tcref.Typars m) tinst

    UnifyTypes cenv env m ty gtyp

    let fields = tcref.TrueInstanceFieldsAsList
    let ftys = fields |> List.map (fun fsp -> actualTyOfRecdField inst fsp, fsp)

    let fieldPats, patEnvR =
        (patEnv, ftys) ||> List.mapFold (fun s (ty, fsp) ->
            match fldsmap.TryGetValue fsp.rfield_id.idText with
            | true, v -> TcPat warnOnUpper cenv env None vFlags s ty v
            | _ -> (fun _ -> TPat_wild m), s)

    let phase2 values =
        TPat_recd (tcref, tinst, List.map (fun f -> f values) fieldPats, m)

    phase2, patEnvR

and TcNullPat cenv env patEnv ty m =
    try
        AddCxTypeUseSupportsNull env.DisplayEnv cenv.css m NoTrace ty
    with exn ->
        errorRecovery exn m
    (fun _ -> TPat_null m), patEnv

and CheckNoArgsForLiteral args m =
    match args with
    | SynArgPats.Pats []
    | SynArgPats.NamePatPairs (pats = []) -> ()
    | _ -> errorR (Error (FSComp.SR.tcLiteralDoesNotTakeArguments (), m))

and GetSynArgPatterns args =
    match args with
    | SynArgPats.Pats args -> args
    | SynArgPats.NamePatPairs (pats = pairs) -> List.map (fun (_, _, pat) -> pat) pairs

and TcArgPats warnOnUpper (cenv: cenv) env vFlags patEnv args =
    let g = cenv.g
    let args = GetSynArgPatterns args
    TcPatterns warnOnUpper cenv env vFlags patEnv (NewInferenceTypes g args) args

and IsNameOf (cenv: cenv) (env: TcEnv) ad m (id: Ident) =
    let g = cenv.g
    id.idText = "nameof" &&
    try
        match ResolveExprLongIdent cenv.tcSink cenv.nameResolver m ad env.NameEnv TypeNameResolutionInfo.Default [id] with
        | Result (_, Item.Value vref, _) -> valRefEq g vref g.nameof_vref
        | _ -> false
    with _ -> false

/// Check a long identifier in a pattern
and TcPatLongIdent warnOnUpper cenv env ad valReprInfo vFlags (patEnv: TcPatLinearEnv) ty (longDotId, tyargs, args, vis, m) =
    let (SynLongIdent(longId, _, _)) = longDotId
    
    if tyargs.IsSome then errorR(Error(FSComp.SR.tcInvalidTypeArgumentUsage(), m))

    let warnOnUpperForId =
        match args with
        | SynArgPats.Pats [] -> warnOnUpper
        | _ -> AllIdsOK

    let mLongId = rangeOfLid longId

    match ResolvePatternLongIdent cenv.tcSink cenv.nameResolver warnOnUpperForId false m ad env.NameEnv TypeNameResolutionInfo.Default longId with
    | Item.NewDef id ->
        TcPatLongIdentNewDef warnOnUpperForId warnOnUpper cenv env ad valReprInfo vFlags patEnv ty (vis, id, args, m)

    | Item.ActivePatternCase apref as item ->

        let (APElemRef (apinfo, _vref, idx, _isStructRetTy)) = apref

        match args with
        | SynArgPats.Pats _ -> ()
        | _ -> errorR (Error (FSComp.SR.tcNamedActivePattern apinfo.ActiveTags[idx], m))

        let args = GetSynArgPatterns args

        TcPatLongIdentActivePatternCase warnOnUpper cenv env vFlags patEnv ty (mLongId, item, apref, args, m)

    | Item.UnionCase _ | Item.ExnCase _ as item ->
        TcPatLongIdentUnionCaseOrExnCase warnOnUpper cenv env ad vFlags patEnv ty (mLongId, item, args, m)

    | Item.ILField finfo ->
        TcPatLongIdentILField warnOnUpper cenv env vFlags patEnv ty (mLongId, finfo, args, m)

    | Item.RecdField rfinfo ->
        TcPatLongIdentRecdField warnOnUpper cenv env vFlags patEnv ty (mLongId, rfinfo, args, m)

    | Item.Value vref ->
        TcPatLongIdentLiteral warnOnUpper cenv env vFlags patEnv ty (mLongId, vref, args, m)

    | _ -> error (Error(FSComp.SR.tcRequireVarConstRecogOrLiteral(), m))

/// Check a long identifier in a pattern that has been not been resolved to anything else and represents a new value, or nameof
and TcPatLongIdentNewDef warnOnUpperForId warnOnUpper (cenv: cenv) env ad valReprInfo vFlags patEnv ty (vis, id, args, m) =
    let g = cenv.g
    let (TcPatLinearEnv(tpenv, _, _)) = patEnv

    match GetSynArgPatterns args with
    | [] ->
        TcPat warnOnUpperForId cenv env valReprInfo vFlags patEnv ty (mkSynPatVar vis id)

    | [arg]
        when g.langVersion.SupportsFeature LanguageFeature.NameOf && IsNameOf cenv env ad m id ->
        match TcNameOfExpr cenv env tpenv (ConvSynPatToSynExpr arg) with
        | Expr.Const(c, m, _) -> TcConstPat warnOnUpper cenv env vFlags patEnv ty (SynConst.String(c.ToString(), SynStringKind.Regular, m)) m
        | _ -> failwith "Impossible: TcNameOfExpr must return an Expr.Const"

    | _ ->
        let _, acc = TcArgPats warnOnUpper cenv env vFlags patEnv args
        errorR (UndefinedName (0, FSComp.SR.undefinedNamePatternDiscriminator, id, NoSuggestions))
        (fun _ -> TPat_error m), acc

and ApplyUnionCaseOrExn m (cenv: cenv) env overallTy item =
    let g = cenv.g
    let ad = env.eAccessRights
    match item with
    | Item.ExnCase ecref ->
        CheckEntityAttributes g ecref m |> CommitOperationResult
        UnifyTypes cenv env m overallTy g.exn_ty
        CheckTyconAccessible cenv.amap m ad ecref |> ignore
        let mkf mArgs args = TPat_exnconstr(ecref, args, unionRanges m mArgs)
        mkf, recdFieldTysOfExnDefRef ecref, [ for f in (recdFieldsOfExnDefRef ecref) -> f  ]

    | Item.UnionCase(ucinfo, showDeprecated) ->
        if showDeprecated then
            let diagnostic = Deprecated(FSComp.SR.nrUnionTypeNeedsQualifiedAccess(ucinfo.DisplayName, ucinfo.Tycon.DisplayName) |> snd, m)
            if g.langVersion.SupportsFeature(LanguageFeature.ErrorOnDeprecatedRequireQualifiedAccess) then
                errorR(diagnostic)
            else
                warning(diagnostic)

        let ucref = ucinfo.UnionCaseRef
        CheckUnionCaseAttributes g ucref m |> CommitOperationResult
        CheckUnionCaseAccessible cenv.amap m ad ucref |> ignore
        let resTy = actualResultTyOfUnionCase ucinfo.TypeInst ucref
        let inst = mkTyparInst ucref.TyconRef.TyparsNoRange ucinfo.TypeInst
        UnifyTypes cenv env m overallTy resTy
        let mkf mArgs args = TPat_unioncase(ucref, ucinfo.TypeInst, args, unionRanges m mArgs)
        mkf, actualTysOfUnionCaseFields inst ucref, [ for f in ucref.AllFieldsAsList -> f]

    | _ ->
        invalidArg "item" "not a union case or exception reference"

/// Check a long identifier 'Case' or 'Case argsR that has been resolved to a union case or F# exception constructor
and TcPatLongIdentUnionCaseOrExnCase warnOnUpper cenv env ad vFlags patEnv ty (mLongId, item, args, m) =
    let g = cenv.g

    // Report information about the case occurrence to IDE
    CallNameResolutionSink cenv.tcSink (mLongId, env.NameEnv, item, emptyTyparInst, ItemOccurence.Pattern, env.eAccessRights)

    let mkf, argTys, argNames = ApplyUnionCaseOrExn m cenv env ty item
    let numArgTys = argTys.Length

    let args, extraPatternsFromNames =
        match args with
        | SynArgPats.Pats args ->
            if g.langVersion.SupportsFeature(LanguageFeature.MatchNotAllowedForUnionCaseWithNoData) then
                match args with
                | [ SynPat.Wild _ ] | [ SynPat.Named _ ] when argNames.IsEmpty  ->
                    warning(Error(FSComp.SR.matchNotAllowedForUnionCaseWithNoData(), m))
                    args, []
                | _ -> args, []
            else
                args, []
        | SynArgPats.NamePatPairs (pairs, m, _) ->
            // rewrite patterns from the form (name-N = pat-N; ...) to (..._, pat-N, _...)
            // so type T = Case of name: int * value: int
            // | Case(value = v)
            // will become
            // | Case(_, v)
            let result = Array.zeroCreate numArgTys
            let extraPatterns = List ()

            for id, _, pat in pairs do
                match argNames |> List.tryFindIndex (fun id2 -> id.idText = id2.Id.idText) with
                | None ->
                    extraPatterns.Add pat
                    match item with
                    | Item.UnionCase(uci, _) ->
                        errorR (Error (FSComp.SR.tcUnionCaseConstructorDoesNotHaveFieldWithGivenName (uci.DisplayName, id.idText), id.idRange))
                    | Item.ExnCase tcref ->
                        errorR (Error (FSComp.SR.tcExceptionConstructorDoesNotHaveFieldWithGivenName (tcref.DisplayName, id.idText), id.idRange))
                    | _ ->
                        errorR (Error (FSComp.SR.tcConstructorDoesNotHaveFieldWithGivenName id.idText, id.idRange))

                | Some idx ->
                    let argItem =
                        match item with
                        | Item.UnionCase (uci, _) -> Item.UnionCaseField (uci, idx)
                        | Item.ExnCase tref -> Item.RecdField (RecdFieldInfo ([], RecdFieldRef (tref, id.idText)))
                        | _ -> failwithf "Expecting union case or exception item, got: %O" item

                    CallNameResolutionSink cenv.tcSink (id.idRange, env.NameEnv, argItem, emptyTyparInst, ItemOccurence.Pattern, ad)

                    match box result[idx] with
                    | Null -> result[idx] <- pat
                    | NonNull _ ->
                        extraPatterns.Add pat
                        errorR (Error (FSComp.SR.tcUnionCaseFieldCannotBeUsedMoreThanOnce id.idText, id.idRange))

            for i = 0 to numArgTys - 1 do
                if isNull (box result[i]) then
                    result[i] <- SynPat.Wild (m.MakeSynthetic())

            let extraPatterns = List.ofSeq extraPatterns

            let args = List.ofArray result
            if result.Length = 1 then args, extraPatterns
            else [ SynPat.Tuple(false, args, m) ], extraPatterns

    let args, extraPatterns =
        match args with
        | [] -> [], []

        // note: the next will always be parenthesized
        | [SynPatErrorSkip(SynPat.Tuple (false, args, _)) | SynPatErrorSkip(SynPat.Paren(SynPatErrorSkip(SynPat.Tuple (false, args, _)), _))] when numArgTys > 1 -> args, []

        // note: we allow both 'C _' and 'C (_)' regardless of number of argument of the pattern
        | [SynPatErrorSkip(SynPat.Wild _ as e) | SynPatErrorSkip(SynPat.Paren(SynPatErrorSkip(SynPat.Wild _ as e), _))] -> List.replicate numArgTys e, []

        | args when numArgTys = 0 ->
            if g.langVersion.SupportsFeature(LanguageFeature.MatchNotAllowedForUnionCaseWithNoData) then
                [], args
            else
                errorR (Error (FSComp.SR.tcUnionCaseDoesNotTakeArguments (), m))
                [], args

        | arg :: rest when numArgTys = 1 ->
            if numArgTys = 1 && not (List.isEmpty rest) then
                errorR (Error (FSComp.SR.tcUnionCaseRequiresOneArgument (), m))
            [arg], rest

        | [arg] -> [arg], []

        | args ->
            [], args

    let args, extraPatterns =
        let numArgs = args.Length
        if numArgs = numArgTys then
            args, extraPatterns
        elif numArgs < numArgTys then
            if numArgTys > 1 then
                // Expects tuple without enough args
                let printTy  = NicePrint.minimalStringOfType env.DisplayEnv
                let missingArgs = 
                    argNames.[numArgs..numArgTys - 1]
                    |> List.map (fun id -> (if id.rfield_name_generated then "" else id.DisplayName + ": ") +  printTy  id.FormalType)
                    |> String.concat (Environment.NewLine + "\t")
                    |> fun s -> Environment.NewLine + "\t" + s

                errorR (Error (FSComp.SR.tcUnionCaseExpectsTupledArguments(numArgTys, numArgs, missingArgs), m))
            else
                errorR (UnionCaseWrongArguments (env.DisplayEnv, numArgTys, numArgs, m))
            args @ (List.init (numArgTys - numArgs) (fun _ -> SynPat.Wild (m.MakeSynthetic()))), extraPatterns
        else
            let args, remaining = args |> List.splitAt numArgTys
            for remainingArg in remaining do
                errorR (UnionCaseWrongArguments (env.DisplayEnv, numArgTys, numArgs, remainingArg.Range))
            args, extraPatterns @ remaining

    let extraPatterns = extraPatterns @ extraPatternsFromNames
    let argsR, acc = TcPatterns warnOnUpper cenv env vFlags patEnv argTys args
    let _, acc = TcPatterns warnOnUpper cenv env vFlags acc (NewInferenceTypes g extraPatterns) extraPatterns
    (fun values -> mkf m (List.map (fun f -> f values) argsR)), acc

/// Check a long identifier that has been resolved to an IL field - valid if a literal
and TcPatLongIdentILField warnOnUpper (cenv: cenv) env vFlags patEnv ty (mLongId, finfo, args, m) =
    let g = cenv.g

    CheckILFieldInfoAccessible g cenv.amap mLongId env.AccessRights finfo

    if not finfo.IsStatic then
        errorR (Error (FSComp.SR.tcFieldIsNotStatic finfo.FieldName, mLongId))

    CheckILFieldAttributes g finfo m

    match finfo.LiteralValue with
    | None ->
        error (Error (FSComp.SR.tcFieldNotLiteralCannotBeUsedInPattern (), mLongId))
    | Some lit ->
        CheckNoArgsForLiteral args m
        let _, acc = TcArgPats warnOnUpper cenv env vFlags patEnv args

        UnifyTypes cenv env m ty (finfo.FieldType (cenv.amap, m))
        let c' = TcFieldInit mLongId lit
        let item = Item.ILField finfo
        CallNameResolutionSink cenv.tcSink (mLongId, env.NameEnv, item, emptyTyparInst, ItemOccurence.Pattern, env.AccessRights)
        (fun _ -> TPat_const (c', m)), acc

/// Check a long identifier that has been resolved to a record field
and TcPatLongIdentRecdField warnOnUpper cenv env vFlags patEnv ty (mLongId, rfinfo, args, m) =
    let g = cenv.g
    CheckRecdFieldInfoAccessible cenv.amap mLongId env.AccessRights rfinfo
    if not rfinfo.IsStatic then errorR (Error (FSComp.SR.tcFieldIsNotStatic(rfinfo.DisplayName), mLongId))
    CheckRecdFieldInfoAttributes g rfinfo mLongId |> CommitOperationResult

    match rfinfo.LiteralValue with
    | None -> error (Error(FSComp.SR.tcFieldNotLiteralCannotBeUsedInPattern(), mLongId))
    | Some lit ->
        CheckNoArgsForLiteral args m
        let _, acc = TcArgPats warnOnUpper cenv env vFlags patEnv args

        UnifyTypes cenv env m ty rfinfo.FieldType
        let item = Item.RecdField rfinfo
        // FUTURE: can we do better than emptyTyparInst here, in order to display instantiations
        // of type variables in the quick info provided in the IDE.
        CallNameResolutionSink cenv.tcSink (mLongId, env.NameEnv, item, emptyTyparInst, ItemOccurence.Pattern, env.AccessRights)
        (fun _ -> TPat_const (lit, m)), acc

/// Check a long identifier that has been resolved to an F# value that is a literal
and TcPatLongIdentLiteral warnOnUpper (cenv: cenv) env vFlags patEnv ty (mLongId, vref, args, m) =
    let g = cenv.g
    let (TcPatLinearEnv(tpenv, _, _)) = patEnv

    match vref.LiteralValue with
    | None -> error (Error(FSComp.SR.tcNonLiteralCannotBeUsedInPattern(), m))
    | Some lit ->
        let _, _, _, vexpty, _, _ = TcVal true cenv env tpenv vref None None mLongId
        CheckValAccessible mLongId env.AccessRights vref
        CheckFSharpAttributes g vref.Attribs mLongId |> CommitOperationResult
        CheckNoArgsForLiteral args m
        let _, acc = TcArgPats warnOnUpper cenv env vFlags patEnv args

        UnifyTypes cenv env m ty vexpty
        let item = Item.Value vref
        CallNameResolutionSink cenv.tcSink (mLongId, env.NameEnv, item, emptyTyparInst, ItemOccurence.Pattern, env.AccessRights)
        (fun _ -> TPat_const (lit, m)), acc

and TcPatterns warnOnUpper cenv env vFlags s argTys args =
    assert (List.length args = List.length argTys)
    List.mapFold (fun s (ty, pat) -> TcPat warnOnUpper cenv env None vFlags s ty pat) s (List.zip argTys args)

