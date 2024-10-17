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
open FSharp.Compiler.Xml
open FSharp.Compiler.SyntaxTrivia
open TypedTreeOps

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
        | [ x ] -> x :: res
        | x :: y :: ys ->
            match x, y with
            | (lidwid, Some(SynExpr.Record(baseInfo, copyInfo, fields1, m))), (_, Some(SynExpr.Record(recordFields = fields2))) ->
                let reducedRecd =
                    (lidwid, Some(SynExpr.Record(baseInfo, copyInfo, fields1 @ fields2, m)))

                groupIfNested res (reducedRecd :: ys)
            | (lidwid, Some(SynExpr.AnonRecd(isStruct, copyInfo, fields1, m, trivia))), (_, Some(SynExpr.AnonRecd(recordFields = fields2))) ->
                let reducedRecd =
                    (lidwid, Some(SynExpr.AnonRecd(isStruct, copyInfo, fields1 @ fields2, m, trivia)))

                groupIfNested res (reducedRecd :: ys)
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
/// `{ x with A.B = 0; A.C = "" }` becomes `{ x with A = { x.A with B = 0 }; A = { x.A with C = "" } }`
let TransformAstForNestedUpdates (cenv: TcFileState) (env: TcEnv) overallTy (lid: LongIdent) exprBeingAssigned withExpr =
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
                    :: List.map (fun (s: Ident, e: Ident) -> withStartEnd s.idRange.End e.idRange.Start s.idRange) t

            let lid = buildLid [] id lidwd |> List.rev

            (lid, List.pairwise lid |> calcLidSeparatorRanges origSepRng)

        let totalRange (origId: Ident) (id: Ident) =
            withStartEnd origId.idRange.End id.idRange.Start origId.idRange

        let rangeOfBlockSeparator (id: Ident) =
            let idEnd = id.idRange.End
            let blockSeparatorStartCol = idEnd.Column
            let blockSeparatorEndCol = blockSeparatorStartCol + 4
            let blockSeparatorStartPos = mkPos idEnd.Line blockSeparatorStartCol
            let blockSeparatorEndPos = mkPos idEnd.Line blockSeparatorEndCol

            withStartEnd blockSeparatorStartPos blockSeparatorEndPos id.idRange

        match withExpr with
        | SynExpr.Ident origId, (sepRange, _) ->
            let lid, rng = upToId sepRange id (origId :: ids)
            Some(SynExpr.LongIdent(false, LongIdentWithDots(lid, rng), None, totalRange origId id), (rangeOfBlockSeparator id, None))
        | _ -> None

    let rec synExprRecd copyInfo (outerFieldId: Ident) innerFields exprBeingAssigned =
        match innerFields with
        | [] -> failwith "unreachable"
        | (fieldId: Ident, item) :: rest ->
            CallNameResolutionSink cenv.tcSink (fieldId.idRange, env.NameEnv, item, [], ItemOccurrence.Use, env.AccessRights)

            let fieldId = ident (fieldId.idText, fieldId.idRange.MakeSynthetic())

            let nestedField =
                if rest.IsEmpty then
                    exprBeingAssigned
                else
                    synExprRecd copyInfo fieldId rest exprBeingAssigned

            match item with
            | Item.AnonRecdField(
                anonInfo = {
                               AnonRecdTypeInfo.TupInfo = TupInfo.Const isStruct
                           }) ->
                let fields = [ LongIdentWithDots([ fieldId ], []), None, nestedField ]
                SynExpr.AnonRecd(isStruct, copyInfo outerFieldId, fields, outerFieldId.idRange, { OpeningBraceRange = range0 })
            | _ ->
                let fields =
                    [
                        SynExprRecordField((LongIdentWithDots([ fieldId ], []), true), None, Some nestedField, None)
                    ]

                SynExpr.Record(None, copyInfo outerFieldId, fields, outerFieldId.idRange)

    let access, fields =
        ResolveNestedField cenv.tcSink cenv.nameResolver env.eNameResEnv env.eAccessRights overallTy lid

    match access, fields with
    | _, [] -> failwith "unreachable"
    | accessIds, [ (fieldId, _) ] -> (accessIds, fieldId), Some exprBeingAssigned
    | accessIds, (outerFieldId, item) :: rest ->
        checkLanguageFeatureAndRecover cenv.g.langVersion LanguageFeature.NestedCopyAndUpdate (rangeOfLid lid)

        CallNameResolutionSink cenv.tcSink (outerFieldId.idRange, env.NameEnv, item, [], ItemOccurrence.Use, env.AccessRights)

        let outerFieldId = ident (outerFieldId.idText, outerFieldId.idRange.MakeSynthetic())

        (accessIds, outerFieldId),
        Some(synExprRecd (recdExprCopyInfo (fields |> List.map fst) withExpr) outerFieldId rest exprBeingAssigned)

/// When the original expression in copy-and-update is more complex than `{ x with ... }`, like `{ f () with ... }`,
/// we bind it first, so that it's not evaluated multiple times during a nested update
let BindOriginalRecdExpr (withExpr: SynExpr * BlockSeparator) mkRecdExpr =
    let originalExpr, blockSep = withExpr
    let mOrigExprSynth = originalExpr.Range.MakeSynthetic()
    let id = mkSynId mOrigExprSynth "bind@"
    let withExpr = SynExpr.Ident id, blockSep

    let binding =
        mkSynBinding
            (PreXmlDoc.Empty, mkSynPatVar None id)
            (None,
             false,
             false,
             mOrigExprSynth,
             DebugPointAtBinding.NoneAtSticky,
             None,
             originalExpr,
             mOrigExprSynth,
             [],
             [],
             None,
             SynBindingTrivia.Zero)

    SynExpr.LetOrUse(false, false, [ binding ], mkRecdExpr (Some withExpr), mOrigExprSynth, SynExprLetOrUseTrivia.Zero)
