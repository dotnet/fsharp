[<RequireQualifiedAccess>]
module internal FSharp.Compiler.Spreads

open System
open FSharp.Compiler
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CheckRecordSyntaxHelpers
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open Internal.Utilities.Library

[<AutoOpen>]
module private Patterns =
    [<Literal>]
    let LeftwardExplicit = true

    [<Literal>]
    let NoLeftwardExplicit = false

/// Merges updates to nested record fields on the same level in record copy-and-update.
///
/// `CheckRecordSyntaxHelpers.TransformAstForNestedUpdates` expands `{ x with A.B = 10; A.C = "" }`
///
/// into
///
/// { x with
///        A = { x.A with B = 10 };
///        A = { x.A with C = "" }
/// }
///
/// which we here combine into
///
/// { x with A = { x.A with B = 10; C = "" } }
let private (|NestedUpdate|_|) expr2 expr1 =
    match expr1, expr2 with
    | SynExpr.Record(baseInfo, copyInfo, fields1, m), SynExpr.Record(recordFields = fields2) ->
        Some(SynExpr.Record(baseInfo, copyInfo, fields1 @ fields2, m))
    | SynExpr.AnonRecd(isStruct, copyInfo, fields1, m, trivia), SynExpr.AnonRecd(recordFields = fields2) ->
        Some(SynExpr.AnonRecd(isStruct, copyInfo, fields1 @ fields2, m, trivia))
    | _ -> None

/// Functions for checking type spreads.
[<RequireQualifiedAccess>]
module Types =
    /// Functions for checking record type spreads.
    [<RequireQualifiedAccess>]
    module Records =
        /// Typechecks the given list of record fields or spreads.
        let check checkSpreadsLanguageFeature tcField tcSpread (fieldsAndSpreads: SynFieldOrSpread list) : _ list =
            let rec loop fields i fieldsAndSpreads =
                match fieldsAndSpreads with
                | [] ->
                    fields
                    |> Map.toList
                    |> List.collect (fun (_, (_, dupes)) -> dupes)
                    |> List.sortBy (fun (i, _) -> i)
                    |> List.map (fun (_, r) -> r)

                | SynFieldOrSpread.Field(SynField(idOpt = None)) :: fieldsAndSpreads -> loop fields i fieldsAndSpreads

                | SynFieldOrSpread.Field(SynField(idOpt = Some fieldId) as synField) :: fieldsAndSpreads ->
                    let field, errorAmbiguousShadowing = tcField synField

                    let fields =
                        fields
                        |> Map.change fieldId.idText (function
                            | None -> Some(LeftwardExplicit, [ i, field ])
                            | Some(LeftwardExplicit, dupes) ->
                                errorAmbiguousShadowing ()
                                Some(LeftwardExplicit, (i, field) :: dupes)
                            | Some(NoLeftwardExplicit, _dupes) -> Some(LeftwardExplicit, [ i, field ]))

                    loop fields (i + 1) fieldsAndSpreads

                | SynFieldOrSpread.Spread(SynTypeSpread(range = m) as synSpread) :: fieldsAndSpreads ->
                    checkSpreadsLanguageFeature m

                    let rec collectFieldsFromSpread fields i fieldsFromSpread =
                        match fieldsFromSpread with
                        | [] -> fields, i
                        | (fieldId, field, warnAmbiguousShadowing) :: fieldsFromSpread ->
                            let fields =
                                fields
                                |> Map.change fieldId (function
                                    | None -> Some(NoLeftwardExplicit, [ i, field ])
                                    | Some(LeftwardExplicit, _dupes) ->
                                        warnAmbiguousShadowing ()
                                        Some(LeftwardExplicit, [ i, field ])
                                    | Some(NoLeftwardExplicit, _dupes) -> Some(NoLeftwardExplicit, [ i, field ]))

                            collectFieldsFromSpread fields (i + 1) fieldsFromSpread

                    let fields, i = collectFieldsFromSpread fields i (tcSpread synSpread)
                    loop fields i fieldsAndSpreads

            loop Map.empty 0 fieldsAndSpreads

/// Functions for checking value spreads.
[<RequireQualifiedAccess>]
module Values =
    /// Functions for checking record spreads.
    [<RequireQualifiedAccess>]
    module Records =
        let private establishFields checkSpreadsLanguageFeature tcField tcSpread (fieldsAndSpreads: SynExprRecordFieldOrSpread list) =
            let rec loop fields i spreadSrcTys spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads =
                match fieldsAndSpreads with
                | [] ->
                    let fields =
                        fields
                        |> Map.toList
                        |> List.collect (fun (_, (_, _, dupes)) -> dupes)
                        |> List.sortBy (fun (i, _) -> i)
                        |> List.map (fun (_, r) -> r)

                    List.rev spreadSrcTys, List.rev spreadSrcExprs, fields

                | SynExprRecordFieldOrSpread.Field(SynExprRecordField(fieldName = _, (* isOk *) false), _) :: _ ->
                    // if we met at least one field that is not syntactically correct - raise ReportedError to transfer control to the recovery routine
                    // raising ReportedError None transfers control to the closest errorRecovery point but do not make any records into log
                    // we assume that parse errors were already reported
                    raise (FSharp.Compiler.DiagnosticsLogger.ReportedError None)

                | SynExprRecordFieldOrSpread.Field((SynExprRecordField(fieldName = synLongId, _; expr = fieldExpr; range = m)), _) :: fieldsAndSpreads ->
                    let interveningSpreadSrc =
                        interveningSpreadSrcs |> Map.tryFind (textOfId (List.head synLongId.LongIdent))

                    let fieldId, path, fieldExpr, errorAmbiguousShadowing =
                        tcField interveningSpreadSrc synLongId fieldExpr m

                    let fields =
                        let (|NestedUpdate|_|) expr1 expr2 =
                            match expr1, expr2 with
                            | None, _
                            | _, None -> None
                            | Some fieldExpr, Some expr -> (|NestedUpdate|_|) fieldExpr expr

                        fields
                        |> Map.change (textOfId fieldId) (function
                            | None -> Some(LeftwardExplicit, fieldExpr, [ i, (fieldId, ExplicitOrSpread.Explicit(path, fieldExpr)) ])
                            | Some(LeftwardExplicit, NestedUpdate fieldExpr combinedExpr, _ :: dupes) ->
                                Some(
                                    LeftwardExplicit,
                                    Some combinedExpr,
                                    (i, (fieldId, ExplicitOrSpread.Explicit(path, Some combinedExpr))) :: dupes
                                )
                            | Some(LeftwardExplicit, _dupeExpr, dupes) ->
                                errorAmbiguousShadowing ()

                                Some(LeftwardExplicit, fieldExpr, (i, (fieldId, ExplicitOrSpread.Explicit(path, fieldExpr))) :: dupes)
                            | Some(NoLeftwardExplicit, _dupeExpr, _dupes) ->
                                Some(LeftwardExplicit, fieldExpr, [ i, (fieldId, ExplicitOrSpread.Explicit(path, fieldExpr)) ]))

                    loop fields (i + 1) spreadSrcTys spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads

                | SynExprRecordFieldOrSpread.Spread(SynExprSpread(expr = spreadSrcSynExpr; range = m) as synExprSpread, _) :: fieldsAndSpreads ->
                    checkSpreadsLanguageFeature m

                    match tcSpread synExprSpread with
                    | Some(spreadSrcExpr, spreadSrcTy, fieldsFromSpread) ->
                        let rec collectFieldsFromSpread fields i interveningSpreadSrcs fieldsFromSpread =
                            match fieldsFromSpread with
                            | [] -> fields, i, interveningSpreadSrcs
                            | (fieldId, field, warnAmbiguousShadowing) :: fieldsFromSpread ->
                                let tys =
                                    fields
                                    |> Map.change (textOfId fieldId) (function
                                        | None -> Some(NoLeftwardExplicit, Some spreadSrcSynExpr, [ i, (fieldId, field) ])
                                        | Some(LeftwardExplicit, _existingExpr, _dupes) ->
                                            warnAmbiguousShadowing ()
                                            Some(LeftwardExplicit, Some spreadSrcSynExpr, [ i, (fieldId, field) ])
                                        | Some(NoLeftwardExplicit, _existingExpr, _dupes) ->
                                            Some(NoLeftwardExplicit, Some spreadSrcSynExpr, [ i, (fieldId, field) ]))

                                let interveningSpreadSrcs =
                                    interveningSpreadSrcs
                                    |> Map.add (textOfId fieldId) (spreadSrcSynExpr, spreadSrcTy)

                                collectFieldsFromSpread tys (i + 1) interveningSpreadSrcs fieldsFromSpread

                        let fields, i, interveningSpreadSrcs =
                            collectFieldsFromSpread fields i interveningSpreadSrcs fieldsFromSpread

                        loop fields i (spreadSrcTy :: spreadSrcTys) (spreadSrcExpr :: spreadSrcExprs) interveningSpreadSrcs fieldsAndSpreads

                    | None -> loop fields i spreadSrcTys spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads

            loop Map.empty 0 [] [] Map.empty fieldsAndSpreads

        /// Typechecks the given list of record fields or spreads.
        let check
            TcExprFlex
            (g: TcGlobals)
            (env: TcEnv)
            (cenv: TcFileState)
            (tpenv: UnscopedTyparEnv)
            (ad: AccessorDomain)
            (mWholeExpr: range)
            withExprOpt
            overallTy
            (fieldsAndSpreads: SynExprRecordFieldOrSpread list)
            =
            let tcField (spreadSrcOpt: (SynExpr * TType) option) (SynLongIdent(lid, _, _)) exprBeingAssigned m =
                let isFromNestedUpdate, path, fieldId, field =
                    let srcExprOpt =
                        spreadSrcOpt
                        |> Option.map (fun (spreadSrc, _) -> spreadSrc, (spreadSrc.Range, None))
                        |> Option.orElse withExprOpt

                    let srcExprTy =
                        spreadSrcOpt
                        |> Option.map (fun (_, spreadSrcTy) -> spreadSrcTy)
                        |> Option.defaultValue overallTy

                    match srcExprOpt, lid, exprBeingAssigned with
                    | _, [ id ], _ -> false, [], id, exprBeingAssigned
                    | Some srcExpr, lid, Some exprBeingAssigned ->
                        let (path, id), exprBeingAssigned =
                            TransformAstForNestedUpdates cenv env srcExprTy lid exprBeingAssigned srcExpr

                        true, path, id, Some exprBeingAssigned
                    | _ ->
                        let (path, id) = List.frontAndBack lid
                        false, path, id, exprBeingAssigned

                let isFromSpread = Option.isSome spreadSrcOpt

                let errorAmbiguousShadowing () =
                    if not isFromNestedUpdate || isFromSpread then
                        errorR (Error(FSComp.SR.tcMultipleFieldsInRecord fieldId.idText, m))

                fieldId, path, field, errorAmbiguousShadowing

            let tcSpread (SynExprSpread(expr = expr; range = m)) =
                let mExpr = expr.Range

                if Option.isSome withExprOpt then
                    errorR (Error(FSComp.SR.tcRecordExprSpreadWithCannotBeUsedWithSpreads (), m))

                let flex = false

                let spreadSrcExpr, _tpenv =
                    TcExprFlex cenv flex false (NewInferenceType g) env tpenv expr

                let tyOfSpreadSrcExpr = tyOfExpr g spreadSrcExpr

                let spreadSrcTyIsNullable =
                    g.checkNullness
                    && (nullnessOfTy g tyOfSpreadSrcExpr).Evaluate() = NullnessInfo.WithNull

                let spreadSrcTyIsRecd =
                    isRecdTy g tyOfSpreadSrcExpr || isAnonRecdTy g tyOfSpreadSrcExpr

                let isValidSpreadSrcTy = not spreadSrcTyIsNullable && spreadSrcTyIsRecd

                if isValidSpreadSrcTy then
                    let spreadSrcAddrExpr, spreadSrc =
                        let srcTyIsStruct = isStructTy g tyOfSpreadSrcExpr

                        let spreadSrcAddrVal, spreadSrcAddrExpr =
                            mkCompGenLocal
                                mWholeExpr
                                "spreadSrc"
                                (if srcTyIsStruct then
                                     mkByrefTy g tyOfSpreadSrcExpr
                                 else
                                     tyOfSpreadSrcExpr)

                        let wrap, oldAddr, _readonly, _writeonly =
                            mkExprAddrOfExpr g srcTyIsStruct false NeverMutates spreadSrcExpr None m

                        spreadSrcAddrExpr, (fun expr -> wrap (mkCompGenLet m spreadSrcAddrVal oldAddr expr))

                    let recordFieldsFromSpread =
                        if isRecdTy g tyOfSpreadSrcExpr then
                            ResolveRecordOrClassFieldsOfType cenv.nameResolver m ad tyOfSpreadSrcExpr false
                        else
                            tryDestAnonRecdTy g tyOfSpreadSrcExpr
                            |> ValueOption.map (fun (anonInfo, tys) ->
                                anonInfo.SortedIds
                                |> List.ofArray
                                |> List.mapi (fun i id -> Item.AnonRecdField(anonInfo, tys, i, id.idRange)))
                            |> ValueOption.defaultValue []

                    let fields =
                        recordFieldsFromSpread
                        |> List.choose (fun field ->
                            match field with
                            | Item.RecdField fieldInfo ->
                                let fieldExpr =
                                    mkRecdFieldGetViaExprAddr (spreadSrcAddrExpr, fieldInfo.RecdFieldRef, fieldInfo.TypeInst, mExpr)

                                let fieldId = ident (fieldInfo.RecdField.Id.idText, mExpr)
                                let ty = fieldInfo.FieldType

                                let warnAmbiguousShadowing () =
                                    let fmtedSpreadField =
                                        NicePrint.stringOfRecdField env.DisplayEnv cenv.infoReader fieldInfo.TyconRef fieldInfo.RecdField

                                    warning (Error(FSComp.SR.tcRecordExprSpreadFieldShadowsExplicitField fmtedSpreadField, m))

                                Some(fieldId, ExplicitOrSpread.Spread(ty, fieldExpr), warnAmbiguousShadowing)

                            | Item.AnonRecdField(anonInfo, tys, fieldIndex, _) ->
                                let fieldExpr =
                                    mkAnonRecdFieldGet g (anonInfo, spreadSrcAddrExpr, tys, fieldIndex, mExpr)

                                let fieldId = anonInfo.SortedIds[fieldIndex]
                                let ty = tys[fieldIndex]

                                let warnAmbiguousShadowing () =
                                    let typars =
                                        tryAppTy g ty
                                        |> ValueOption.map (snd >> List.choose (tryDestTyparTy g >> ValueOption.toOption))
                                        |> ValueOption.defaultValue []

                                    let fmtedSpreadField =
                                        LayoutRender.showL (
                                            NicePrint.prettyLayoutOfMemberSig env.DisplayEnv ([], fieldId.idText, typars, [], ty)
                                        )

                                    warning (Error(FSComp.SR.tcRecordExprSpreadFieldShadowsExplicitField fmtedSpreadField, m))

                                Some(fieldId, ExplicitOrSpread.Spread(ty, fieldExpr), warnAmbiguousShadowing)

                            | _ -> None)

                    Some(spreadSrc, tyOfSpreadSrcExpr, fields)
                else
                    if not expr.IsArbExprAndThusAlreadyReportedError then
                        if not spreadSrcTyIsRecd then
                            errorR (Error(FSComp.SR.tcRecordExprSpreadSourceMustBeRecord (), m))
                        elif spreadSrcTyIsNullable then
                            errorR (Error(FSComp.SR.tcRecordExprSpreadSourceCannotBeNullable (), m))

                    None

            let checkSpreadsLanguageFeature m =
                checkLanguageFeatureAndRecover g.langVersion LanguageFeature.RecordSpreads m

            establishFields checkSpreadsLanguageFeature tcField tcSpread fieldsAndSpreads

    /// Functions for checking anonymous record spreads.
    module AnonymousRecords =
        let private establishFields
            checkSpreadsLanguageFeature
            tcField
            tcSpread
            (targetAnonRecordTy, targetAnonRecordTyContainsField)
            (fieldsAndSpreads: SynExprAnonRecordFieldOrSpread list)
            =
            let rec loop fields i spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads =
                match fieldsAndSpreads with
                | [] ->
                    let processedFieldsList = Map.toList fields

                    let processedFieldsList =
                        // If the target type is a known anonymous record type,
                        // keep only those fields that are present in that type
                        // or that are explicitly defined in this one.
                        if targetAnonRecordTy then
                            processedFieldsList
                            |> List.filter (function
                                | _, (LeftwardExplicit, _, _) -> true
                                | fieldId, (NoLeftwardExplicit, _, _) -> targetAnonRecordTyContainsField fieldId)
                        else
                            processedFieldsList

                    let (|Head|) = List.head

                    let fieldsInAlphabeticalOrder =
                        processedFieldsList |> List.sortBy (fun (fieldName, _) -> fieldName)

                    let fieldTysInAlphabeticalOrder =
                        fieldsInAlphabeticalOrder
                        |> List.map (fun (_, (_, _, Head(_, (_, fieldTy, _)))) -> fieldTy)

                    let fieldIdsInAlphabeticalOrder =
                        fieldsInAlphabeticalOrder
                        |> List.map (fun (_, (_, _, Head(_, (fieldId, _, _)))) -> fieldId)

                    let fieldsInSrcOrder =
                        processedFieldsList
                        |> List.collect (fun (_, (_, _, dupes)) -> dupes)
                        |> List.sortBy (fun (i, _) -> i)
                        |> List.map (fun (_, field) -> field)

                    List.rev spreadSrcExprs, fieldIdsInAlphabeticalOrder, fieldTysInAlphabeticalOrder, fieldsInSrcOrder

                | SynExprAnonRecordFieldOrSpread.Field(SynExprAnonRecordField(fieldName = synLongId) as synExprAnonRecordField, _) :: fieldsAndSpreads ->
                    let interveningSpreadSrc =
                        interveningSpreadSrcs |> Map.tryFind (textOfId (List.head synLongId.LongIdent))

                    let fieldId, fieldTy, transformedFieldExpr, mkTcField, errorAmbiguousShadowing =
                        tcField interveningSpreadSrc synExprAnonRecordField

                    let fields =
                        fields
                        |> Map.change (textOfId fieldId) (function
                            | None ->
                                Some(LeftwardExplicit, transformedFieldExpr, [ i, (fieldId, fieldTy, mkTcField transformedFieldExpr) ])
                            | Some(LeftwardExplicit, NestedUpdate transformedFieldExpr groupedExpr, _ :: dupes) ->
                                Some(LeftwardExplicit, groupedExpr, (i, (fieldId, fieldTy, mkTcField groupedExpr)) :: dupes)
                            | Some(LeftwardExplicit, _dupeExpr, dupes) ->
                                errorAmbiguousShadowing ()

                                Some(
                                    LeftwardExplicit,
                                    transformedFieldExpr,
                                    (i, (fieldId, fieldTy, mkTcField transformedFieldExpr)) :: dupes
                                )
                            | Some(NoLeftwardExplicit, _dupeExpr, _dupes) ->
                                Some(LeftwardExplicit, transformedFieldExpr, [ i, (fieldId, fieldTy, mkTcField transformedFieldExpr) ]))

                    loop fields (i + 1) spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads

                | SynExprAnonRecordFieldOrSpread.Spread(SynExprSpread(expr = spreadSrcSynExpr; range = m), _) :: fieldsAndSpreads ->
                    checkSpreadsLanguageFeature m

                    match tcSpread spreadSrcSynExpr m with
                    | Some(spreadSrcExpr, spreadSrcTy, fieldsFromSpread) ->
                        let rec collectFieldsFromSpread fields i interveningSpreadSrcs fieldsFromSpread =
                            match fieldsFromSpread with
                            | [] -> fields, i, interveningSpreadSrcs
                            | (fieldId, fieldTy, tcField, warnAmbiguousShadowing) :: fieldsFromSpread ->
                                let tys =
                                    fields
                                    |> Map.change (textOfId fieldId) (function
                                        | None -> Some(NoLeftwardExplicit, spreadSrcSynExpr, [ i, (fieldId, fieldTy, tcField) ])
                                        | Some(LeftwardExplicit, _existingExpr, _dupes) ->
                                            warnAmbiguousShadowing ()
                                            Some(LeftwardExplicit, spreadSrcSynExpr, [ i, (fieldId, fieldTy, tcField) ])
                                        | Some(NoLeftwardExplicit, _existingExpr, _dupes) ->
                                            Some(NoLeftwardExplicit, spreadSrcSynExpr, [ i, (fieldId, fieldTy, tcField) ]))

                                let interveningSpreadSrcs =
                                    interveningSpreadSrcs
                                    |> Map.add (textOfId fieldId) (spreadSrcSynExpr, spreadSrcTy)

                                collectFieldsFromSpread tys (i + 1) interveningSpreadSrcs fieldsFromSpread

                        let fields, i, interveningSpreadSrcs =
                            collectFieldsFromSpread fields i interveningSpreadSrcs fieldsFromSpread

                        loop fields i (spreadSrcExpr :: spreadSrcExprs) interveningSpreadSrcs fieldsAndSpreads

                    | None -> loop fields i spreadSrcExprs interveningSpreadSrcs fieldsAndSpreads

            loop Map.empty 0 [] Map.empty fieldsAndSpreads

        /// Typechecks the given list of anonymous record fields or spreads.
        let check
            TcExprFlex
            TcAdjustExprForTypeDirectedConversions
            MustConvertTo
            UnifyOverallType
            errorRIfSpreadUsedWithWith
            (g: TcGlobals)
            (env: TcEnv)
            (cenv: TcFileState)
            (tpenv: UnscopedTyparEnv)
            (ad: AccessorDomain)
            (mWholeExpr: range)
            (maybeAnonRecdTargetTy: (AnonRecdTypeInfo * TType list) voption)
            (origExprOpt: (SynExpr * BlockSeparator) option)
            (origExprTyOrOverallTy: TType)
            (unsortedFieldIdsAndSynExprsGiven: SynExprAnonRecordFieldOrSpread list)
            =
            let checkSpreadsLanguageFeature m =
                checkLanguageFeatureAndRecover g.langVersion LanguageFeature.RecordSpreads m

            let possibleTargetTyAt =
                match maybeAnonRecdTargetTy with
                | ValueSome(anonInfo, tys) ->
                    let names = anonInfo.SortedNames
                    let tys = List.toArray tys

                    fun name ->
                        let i = Array.BinarySearch(names, name)
                        if i < 0 then ValueNone else ValueSome tys[i]
                | ValueNone -> fun _ -> ValueNone

            let tcField
                (spreadSrcOpt: (SynExpr * TType) option)
                (SynExprAnonRecordField(fieldName = SynLongIdent(fieldLid, _, _) as synLongIdent; expr = expr; range = m))
                =
                let isFromNestedUpdate, fieldId, transformedFieldExpr =
                    let srcExpr, srcTy =
                        spreadSrcOpt
                        |> Option.map (fun (spreadSrc, spreadSrcTy) -> (spreadSrc, (spreadSrc.Range, None)), spreadSrcTy)
                        |> Option.orElseWith (fun () -> origExprOpt |> Option.map (fun origExpr -> origExpr, origExprTyOrOverallTy))
                        |> Option.defaultWith (fun () ->
                            (arbExpr ("nestedUpdateSrcExpr", synLongIdent.Range), (synLongIdent.Range, None)), origExprTyOrOverallTy)

                    match fieldLid with
                    | [] -> error (Error(FSComp.SR.nrUnexpectedEmptyLongId (), mWholeExpr))
                    | [ id ] -> false, id, expr
                    | lid ->
                        let (_, id), exprBeingAssigned =
                            TransformAstForNestedUpdates cenv env srcTy lid expr srcExpr

                        true, id, exprBeingAssigned

                let fieldTy =
                    possibleTargetTyAt fieldId.idText
                    |> ValueOption.defaultWith (fun () -> NewInferenceType g)

                let tcField expr =
                    fun () -> let fieldExpr, _ = TcExprFlex cenv true false fieldTy env tpenv expr in fieldExpr

                let errorAmbiguousShadowing () =
                    if not isFromNestedUpdate then
                        errorR (Error(FSComp.SR.tcAnonRecdDuplicateFieldId fieldId.idText, m))

                fieldId, fieldTy, transformedFieldExpr, tcField, errorAmbiguousShadowing

            let tcSpread (expr: SynExpr) m =
                errorRIfSpreadUsedWithWith m

                let flex = false

                let spreadSrcExpr, _ =
                    TcExprFlex cenv flex false (NewInferenceType g) env tpenv expr

                let tyOfSpreadSrcExpr = tyOfExpr g spreadSrcExpr

                let spreadSrcTyIsNullable =
                    g.checkNullness
                    && (nullnessOfTy g tyOfSpreadSrcExpr).Evaluate() = NullnessInfo.WithNull

                let spreadSrcTyIsRecd =
                    isRecdTy g tyOfSpreadSrcExpr || isAnonRecdTy g tyOfSpreadSrcExpr

                let isValidSpreadSrcTy = not spreadSrcTyIsNullable && spreadSrcTyIsRecd

                if isValidSpreadSrcTy then
                    let spreadSrcAddrExpr, spreadSrcExpr =
                        let srcTyIsStruct = isStructTy g tyOfSpreadSrcExpr

                        let spreadSrcAddrVal, spreadSrcAddrExpr =
                            mkCompGenLocal
                                mWholeExpr
                                "spreadSrc"
                                (if srcTyIsStruct then
                                     mkByrefTy g tyOfSpreadSrcExpr
                                 else
                                     tyOfSpreadSrcExpr)

                        let wrap, oldAddr, _readonly, _writeonly =
                            mkExprAddrOfExpr g srcTyIsStruct false NeverMutates spreadSrcExpr None m

                        spreadSrcAddrExpr, (fun expr -> wrap (mkCompGenLet m spreadSrcAddrVal oldAddr expr))

                    let recordFieldsFromSpread =
                        if isRecdTy g tyOfSpreadSrcExpr then
                            ResolveRecordOrClassFieldsOfType cenv.nameResolver m ad tyOfSpreadSrcExpr false
                        else
                            tryDestAnonRecdTy g tyOfSpreadSrcExpr
                            |> ValueOption.map (fun (anonInfo, tys) ->
                                anonInfo.SortedIds
                                |> List.ofArray
                                |> List.mapi (fun i id -> Item.AnonRecdField(anonInfo, tys, i, id.idRange)))
                            |> ValueOption.defaultValue []

                    let fields =
                        recordFieldsFromSpread
                        |> List.choose (fun field ->
                            match field with
                            | Item.RecdField fieldInfo ->
                                let fieldId = fieldInfo.RecdField.Id

                                let ty =
                                    possibleTargetTyAt fieldId.idText
                                    |> ValueOption.defaultValue fieldInfo.FieldType

                                let tcField () =
                                    let get =
                                        mkRecdFieldGetViaExprAddr (spreadSrcAddrExpr, fieldInfo.RecdFieldRef, fieldInfo.TypeInst, m)

                                    let overallTy = MustConvertTo(false, ty)
                                    UnifyOverallType cenv env m overallTy fieldInfo.FieldType

                                    let fieldExpr =
                                        TcAdjustExprForTypeDirectedConversions cenv overallTy fieldInfo.FieldType env m get

                                    let fieldExpr = mkCoerceIfNeeded g ty (tyOfExpr g fieldExpr) fieldExpr
                                    fieldExpr

                                let warnAmbiguousShadowing () =
                                    let fmtedSpreadField =
                                        NicePrint.stringOfRecdField env.DisplayEnv cenv.infoReader fieldInfo.TyconRef fieldInfo.RecdField

                                    warning (Error(FSComp.SR.tcRecordExprSpreadFieldShadowsExplicitField fmtedSpreadField, m))

                                Some(fieldId, ty, tcField, warnAmbiguousShadowing)

                            | Item.AnonRecdField(anonInfo, tys, fieldIndex, _) ->
                                let fieldId = anonInfo.SortedIds[fieldIndex]

                                let ty =
                                    possibleTargetTyAt fieldId.idText
                                    |> ValueOption.defaultWith (fun () -> tys[fieldIndex])

                                let tcField () =
                                    let get =
                                        mkAnonRecdFieldGetViaExprAddr (anonInfo, spreadSrcAddrExpr, tys, fieldIndex, m)

                                    let overallTy = MustConvertTo(false, ty)
                                    UnifyOverallType cenv env m overallTy tys[fieldIndex]

                                    let fieldExpr =
                                        TcAdjustExprForTypeDirectedConversions cenv overallTy tys[fieldIndex] env m get

                                    let fieldExpr = mkCoerceIfNeeded g ty (tyOfExpr g fieldExpr) fieldExpr
                                    fieldExpr

                                let warnAmbiguousShadowing () =
                                    let typars =
                                        tryAppTy g ty
                                        |> ValueOption.map (snd >> List.choose (tryDestTyparTy g >> ValueOption.toOption))
                                        |> ValueOption.defaultValue []

                                    let fmtedSpreadField =
                                        LayoutRender.showL (
                                            NicePrint.prettyLayoutOfMemberSig env.DisplayEnv ([], fieldId.idText, typars, [], ty)
                                        )

                                    warning (Error(FSComp.SR.tcRecordExprSpreadFieldShadowsExplicitField fmtedSpreadField, m))

                                Some(fieldId, ty, tcField, warnAmbiguousShadowing)

                            | _ -> None)

                    Some(spreadSrcExpr, tyOfSpreadSrcExpr, fields)
                else
                    if not expr.IsArbExprAndThusAlreadyReportedError then
                        if not spreadSrcTyIsRecd then
                            errorR (Error(FSComp.SR.tcAnonRecordExprSpreadSourceMustBeRecord (), expr.Range))
                        elif spreadSrcTyIsNullable then
                            errorR (Error(FSComp.SR.tcAnonRecordExprSpreadSourceCannotBeNullable (), m))

                    None

            let targetAnonRecordTy, targetAnonRecordTyContainsField =
                maybeAnonRecdTargetTy
                |> ValueOption.map (fun (anonInfo, _) ->
                    let sortedNames = anonInfo.SortedNames
                    true, fun fieldId -> Array.BinarySearch(sortedNames, fieldId) >= 0)
                |> ValueOption.defaultValue (false, fun _ -> false)

            establishFields
                checkSpreadsLanguageFeature
                tcField
                tcSpread
                (targetAnonRecordTy, targetAnonRecordTyContainsField)
                unsortedFieldIdsAndSynExprsGiven
