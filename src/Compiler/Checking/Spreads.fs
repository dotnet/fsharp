[<RequireQualifiedAccess>]
module internal FSharp.Compiler.Spreads

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps

[<AutoOpen>]
module private Patterns =
    [<Literal>]
    let LeftwardExplicit = true

    [<Literal>]
    let NoLeftwardExplicit = false

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
        /// Typechecks the given list of record fields or spreads.
        let check checkSpreadsLanguageFeature tcField tcSpread (fieldsAndSpreads: SynExprRecordFieldOrSpread list) =
            let rec loop fields i spreadSrcExprs fieldsAndSpreads =
                match fieldsAndSpreads with
                | [] ->
                    let fields =
                        fields
                        |> Map.toList
                        |> List.collect (fun (_, (_, dupes)) -> dupes)
                        |> List.sortBy (fun (i, _) -> i)
                        |> List.map (fun (_, r) -> r)

                    List.rev spreadSrcExprs, fields

                | SynExprRecordFieldOrSpread.Field(SynExprRecordField(fieldName = _, (* isOk *) false), _) :: _ ->
                    // if we met at least one field that is not syntactically correct - raise ReportedError to transfer control to the recovery routine
                    // raising ReportedError None transfers control to the closest errorRecovery point but do not make any records into log
                    // we assume that parse errors were already reported
                    raise (FSharp.Compiler.DiagnosticsLogger.ReportedError None)

                | SynExprRecordFieldOrSpread.Field(synExprRecordField, _) :: fieldsAndSpreads ->
                    let fieldId, field, errorAmbiguousShadowing = tcField synExprRecordField

                    let fields =
                        fields
                        |> Map.change fieldId (function
                            | None -> Some(LeftwardExplicit, [ i, field ])
                            | Some(LeftwardExplicit, dupes) ->
                                errorAmbiguousShadowing ()
                                Some(LeftwardExplicit, (i, field) :: dupes)
                            | Some(NoLeftwardExplicit, _dupes) -> Some(LeftwardExplicit, [ i, field ]))

                    loop fields (i + 1) spreadSrcExprs fieldsAndSpreads

                | SynExprRecordFieldOrSpread.Spread(SynExprSpread(range = m) as synExprSpread, _) :: fieldsAndSpreads ->
                    checkSpreadsLanguageFeature m

                    let rec collectFieldsFromSpread fields i fieldsFromSpread =
                        match fieldsFromSpread with
                        | [] -> fields, i
                        | (fieldId, field, warnAmbiguousShadowing) :: fieldsFromSpread ->
                            let tys =
                                fields
                                |> Map.change fieldId (function
                                    | None -> Some(NoLeftwardExplicit, [ i, field ])
                                    | Some(LeftwardExplicit, _dupes) ->
                                        warnAmbiguousShadowing ()
                                        Some(LeftwardExplicit, [ i, field ])
                                    | Some(NoLeftwardExplicit, _dupes) -> Some(NoLeftwardExplicit, [ i, field ]))

                            collectFieldsFromSpread tys (i + 1) fieldsFromSpread

                    let spreadSrcExpr, fieldsFromSpread = tcSpread synExprSpread
                    let fields, i = collectFieldsFromSpread fields i fieldsFromSpread

                    let spreadSrcExprs =
                        spreadSrcExpr
                        |> Option.map (fun spreadSrcExpr -> spreadSrcExpr :: spreadSrcExprs)
                        |> Option.defaultValue spreadSrcExprs

                    loop fields i spreadSrcExprs fieldsAndSpreads

            loop Map.empty 0 [] fieldsAndSpreads

    /// Functions for checking anonymous record spreads.
    module AnonymousRecords =
        /// Typechecks the given list of anonymous record fields or spreads.
        let check
            checkSpreadsLanguageFeature
            tcField
            tcSpread
            (targetAnonRecordTy, targetAnonRecordTyContainsField)
            (fieldsAndSpreads: SynExprAnonRecordFieldOrSpread list)
            =
            // See CheckRecordSyntaxHelpers.GroupUpdatesToNestedFields.
            let (|NestedUpdate|_|) expr2 expr1 =
                match expr1, expr2 with
                | SynExpr.Record(baseInfo, copyInfo, fields1, m), SynExpr.Record(recordFields = fields2) ->
                    Some(SynExpr.Record(baseInfo, copyInfo, fields1 @ fields2, m))
                | SynExpr.AnonRecd(isStruct, copyInfo, fields1, m, trivia), SynExpr.AnonRecd(recordFields = fields2) ->
                    Some(SynExpr.AnonRecd(isStruct, copyInfo, fields1 @ fields2, m, trivia))
                | _ -> None

            let rec loop fields i spreadSrcExprs fieldsAndSpreads =
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

                | SynExprAnonRecordFieldOrSpread.Field(synExprAnonRecordField, _) :: fieldsAndSpreads ->
                    let fieldId, fieldTy, transformedFieldExpr, mkTcField, errorAmbiguousShadowing =
                        tcField synExprAnonRecordField

                    let fields =
                        fields
                        |> Map.change (textOfId fieldId) (function
                            | None ->
                                Some(LeftwardExplicit, transformedFieldExpr, [ i, (fieldId, fieldTy, mkTcField transformedFieldExpr) ])
                            | Some(LeftwardExplicit, NestedUpdate transformedFieldExpr groupedExpr, _ :: dupes) ->
                                Some(LeftwardExplicit, groupedExpr, (i, (fieldId, fieldTy, mkTcField groupedExpr)) :: dupes)
                            | Some(LeftwardExplicit, (SynExpr.Record _ | SynExpr.AnonRecd _ as dupeExpr), dupes) ->
                                errorAmbiguousShadowing ()
                                Some(LeftwardExplicit, dupeExpr, (i, (fieldId, fieldTy, mkTcField transformedFieldExpr)) :: dupes)
                            | Some(LeftwardExplicit, _dupeExpr, dupes) ->
                                errorAmbiguousShadowing ()

                                Some(
                                    LeftwardExplicit,
                                    transformedFieldExpr,
                                    (i, (fieldId, fieldTy, mkTcField transformedFieldExpr)) :: dupes
                                )
                            | Some(NoLeftwardExplicit, _dupeExpr, _dupes) ->
                                Some(LeftwardExplicit, transformedFieldExpr, [ i, (fieldId, fieldTy, mkTcField transformedFieldExpr) ]))

                    loop fields (i + 1) spreadSrcExprs fieldsAndSpreads

                | SynExprAnonRecordFieldOrSpread.Spread(SynExprSpread(expr = expr; range = m), _) :: fieldsAndSpreads ->
                    checkSpreadsLanguageFeature m

                    let rec collectFieldsFromSpread fields i fieldsFromSpread =
                        match fieldsFromSpread with
                        | [] -> fields, i
                        | (fieldId, fieldTy, tcField, warnAmbiguousShadowing) :: fieldsFromSpread ->
                            let tys =
                                fields
                                |> Map.change (textOfId fieldId) (function
                                    | None -> Some(NoLeftwardExplicit, expr, [ i, (fieldId, fieldTy, tcField) ])
                                    | Some(LeftwardExplicit, expr, _dupes) ->
                                        warnAmbiguousShadowing ()
                                        Some(LeftwardExplicit, expr, [ i, (fieldId, fieldTy, tcField) ])
                                    | Some(NoLeftwardExplicit, expr, _dupes) ->
                                        Some(NoLeftwardExplicit, expr, [ i, (fieldId, fieldTy, tcField) ]))

                            collectFieldsFromSpread tys (i + 1) fieldsFromSpread

                    let spreadSrcExpr, fieldsFromSpread = tcSpread expr m
                    let fields, i = collectFieldsFromSpread fields i fieldsFromSpread

                    let spreadSrcExprs =
                        spreadSrcExpr
                        |> Option.map (fun spreadSrcExpr -> spreadSrcExpr :: spreadSrcExprs)
                        |> Option.defaultValue spreadSrcExprs

                    loop fields i spreadSrcExprs fieldsAndSpreads

            loop Map.empty 0 [] fieldsAndSpreads
