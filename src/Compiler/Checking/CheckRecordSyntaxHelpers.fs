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
let GroupUpdatesToNestedFields (fields: ((Ident list * Ident) * SynExpr option) list) =
    let rec groupIfNested res xs =
        match xs with
        | [] -> res
        | x :: [] -> x :: res
        | x :: y :: ys ->
            match x, y with
            | (lidwid, Some (SynExpr.Record (baseInfo, copyInfo, aFlds, m))), (_, Some (SynExpr.Record (recordFields = bFlds))) ->
                let reducedRecd =
                    (lidwid, Some(SynExpr.Record(baseInfo, copyInfo, aFlds @ bFlds, m)))

                groupIfNested (reducedRecd :: res) ys
            | (lidwid, Some (SynExpr.AnonRecd (isStruct, copyInfo, aFlds, m, trivia))), (_, Some (SynExpr.AnonRecd (recordFields = bFlds))) ->
                let reducedRecd =
                    (lidwid, Some(SynExpr.AnonRecd(isStruct, copyInfo, aFlds @ bFlds, m, trivia)))

                groupIfNested (reducedRecd :: res) ys
            | _ -> groupIfNested (x :: res) (y :: ys)

    fields
    |> List.groupBy (fun ((_, field), _) -> field.idText)
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
        | (fieldId, anonInfo) :: rest ->
            let nestedField =
                if rest.IsEmpty then
                    exprBeingAssigned
                else
                    synExprRecd copyInfo fieldId rest exprBeingAssigned

            let m = id.idRange.MakeSynthetic()

            match anonInfo with
            | Some {
                       AnonRecdTypeInfo.TupInfo = TupInfo.Const isStruct
                   } ->
                let fields = [ LongIdentWithDots([ fieldId ], []), None, nestedField ]
                SynExpr.AnonRecd(isStruct, copyInfo id, fields, m, { OpeningBraceRange = range0 })
            | _ ->
                let fields =
                    [
                        SynExprRecordField((LongIdentWithDots([ fieldId ], []), true), None, Some nestedField, None)
                    ]

                SynExpr.Record(None, copyInfo id, fields, m)

    let access, fields =
        ResolveNestedField cenv.tcSink cenv.nameResolver env.eNameResEnv env.eAccessRights overallTy lid

    match access, fields with
    | _, [] -> failwith "unreachable"
    | accessIds, [ (fieldId, _) ] -> (accessIds, fieldId), Some exprBeingAssigned
    | accessIds, (fieldId, _) :: rest ->
        checkLanguageFeatureAndRecover cenv.g.langVersion LanguageFeature.NestedCopyAndUpdate (rangeOfLid lid)

        (accessIds, fieldId), Some(synExprRecd (recdExprCopyInfo (fields |> List.map fst) withExpr) fieldId rest exprBeingAssigned)
