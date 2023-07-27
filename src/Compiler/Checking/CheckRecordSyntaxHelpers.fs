// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open Internal.Utilities.Library.Extras

/// Merges updates to nested record fields on the same level in record copy-and-update.
///
/// `TransformAstForNestedUpdates` expands `{ x with A.B = 10; A.C = "" }`
///
/// into
///
/// { x with
///        A = { x.A with B = 10 };
///        A = { x.A with C = "" }
/// }
///
/// which we here convert to
///
/// { x with A = { x.A with B = 10; C = "" } }
let GroupUpdatesToNestedFields sink nenv ad (fields: ((Ident list * Ident) * Item option * SynExpr option) list) =
    let rec groupIfNested res xs =
        match xs with
        | [] -> res
        | x :: [] -> x :: res
        | x :: y :: ys ->
            // Merging { x with A = { x.A with B = 10 }; A = { x.A with C = "" } }   into   { x with A = { x.A with B = 10; C = "" } }
            //                                           ^
            //                                           |
            //                                            -------
            //                                                   |
            // { x with A.B = 10; A.C = "" }                     |
            //                    ^                              |
            //                    |                              |
            //                     ------------                  |
            //                                 |                 |
            //                           ______________    _______________
            // We're losing track of the original range of this identifier after this point,
            // so report it to the name resolution environment
            match x, y with
            | (lidwid, item, Some (SynExpr.Record (baseInfo, copyInfo, fields1, m1))),
              (_, _, Some (SynExpr.Record (recordFields = fields2; range = m2))) ->
                let reducedRecd =
                    (lidwid, item, Some(SynExpr.Record(baseInfo, copyInfo, fields1 @ fields2, m1.MakeSynthetic())))

                match item with
                | Some item -> CallNameResolutionSink sink (m2, nenv, item, [], ItemOccurence.Use, ad)
                | _ -> ()

                groupIfNested res (reducedRecd :: ys)
            | (lidwid, item, Some (SynExpr.AnonRecd (isStruct, copyInfo, fields1, m1, trivia))),
              (_, _, Some (SynExpr.AnonRecd (recordFields = fields2; range = m2))) ->
                let reducedRecd =
                    (lidwid, item, Some(SynExpr.AnonRecd(isStruct, copyInfo, fields1 @ fields2, m1.MakeSynthetic(), trivia)))

                match item with
                | Some item -> CallNameResolutionSink sink (m2, nenv, item, [], ItemOccurence.Use, ad)
                | _ -> ()

                groupIfNested res (reducedRecd :: ys)
            | _ -> groupIfNested (x :: res) (y :: ys)

    fields
    |> List.groupBy (fun ((_, field), _, _) -> field.idText)
    |> List.collect (fun (_, fields) ->
        if fields.Length < 2 then
            fields
        else
            groupIfNested [] fields)

/// Expands a long identifier into nested copy-and-update expressions.
///
/// `{ x with A.B = 0 }` becomes `{ x with A = { x.A with B = 0 } }`
let TransformAstForNestedUpdates (cenv: TcFileState) env overallTy (lid: LongIdent) exprBeingAssigned withExpr =
    let recdExprCopyInfo ids withExpr id =
        let upToId origSepRng id lidwd =
            let rec buildLid res (id: Ident) =
                function
                | [] -> res
                | (h: Ident) :: t ->
                    // Mark these hidden field accesses as synthetic so that they don't make it
                    // into the name resolution sink.
                    let h = ident (h.idText, h.idRange.MakeSynthetic())

                    if equals h.idRange id.idRange then
                        h :: res
                    else
                        buildLid (h :: res) id t

            let calcLidSeparatorRanges origSepRng lid =
                match lid with
                | []
                | [ _ ] -> [ origSepRng ]
                | _ :: t ->
                    origSepRng
                    :: List.map (fun (s: Ident, e: Ident) -> mkRange s.idRange.FileName s.idRange.End e.idRange.Start) t

            let lid = buildLid [] id lidwd |> List.rev

            (lid, List.pairwise lid |> calcLidSeparatorRanges origSepRng)

        let totalRange (origId: Ident) (id: Ident) =
            mkRange origId.idRange.FileName origId.idRange.End id.idRange.Start

        let rangeOfBlockSeperator (id: Ident) =
            let idEnd = id.idRange.End
            let blockSeperatorStartCol = idEnd.Column
            let blockSeperatorEndCol = blockSeperatorStartCol + 4
            let blockSeperatorStartPos = mkPos idEnd.Line blockSeperatorStartCol
            let blockSeporatorEndPos = mkPos idEnd.Line blockSeperatorEndCol

            mkRange id.idRange.FileName blockSeperatorStartPos blockSeporatorEndPos

        match withExpr with
        | SynExpr.Ident origId, (sepRange, _) ->
            let lid, rng = upToId sepRange id (origId :: ids)
            Some(SynExpr.LongIdent(false, LongIdentWithDots(lid, rng), None, totalRange origId id), (rangeOfBlockSeperator id, None))
        | _ -> None

    let rec synExprRecd copyInfo (id: Ident) fields exprBeingAssigned =
        match fields with
        | [] -> failwith "unreachable"
        | (fieldId, anonInfo, _) :: rest ->
            let nestedField =
                if rest.IsEmpty then
                    exprBeingAssigned
                else
                    synExprRecd copyInfo fieldId rest exprBeingAssigned

            match anonInfo with
            | Some {
                       AnonRecdTypeInfo.TupInfo = TupInfo.Const isStruct
                   } ->
                let fields = [ LongIdentWithDots([ fieldId ], []), None, nestedField ]
                SynExpr.AnonRecd(isStruct, copyInfo id, fields, id.idRange, { OpeningBraceRange = range0 })
            | _ ->
                let fields =
                    [
                        SynExprRecordField((LongIdentWithDots([ fieldId ], []), true), None, Some nestedField, None)
                    ]

                SynExpr.Record(None, copyInfo id, fields, id.idRange)

    let access, fields =
        ResolveNestedField cenv.tcSink cenv.nameResolver env.eNameResEnv env.eAccessRights overallTy lid

    match access, fields with
    | _, [] -> failwith "unreachable"
    | accessIds, [ (fieldId, _, item) ] -> (accessIds, fieldId), item, Some exprBeingAssigned
    | accessIds, (fieldId, _, item) :: rest ->
        checkLanguageFeatureAndRecover cenv.g.langVersion LanguageFeature.NestedCopyAndUpdate (rangeOfLid lid)

        (accessIds, fieldId), item, Some(synExprRecd (recdExprCopyInfo (fields |> List.map p13) withExpr) fieldId rest exprBeingAssigned)
