// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Erase discriminated unions - IL instruction emission.
[<AutoOpen>]
module internal FSharp.Compiler.AbstractIL.ILX.EraseUnionsEmit

open FSharp.Compiler.IlxGenSupport

open System.Collections.Generic
open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

// Nullary cases on types with helpers do not reveal their underlying type even when
// using runtime type discrimination, because the underlying type is never needed from
// C# code and pollutes the visible API surface. In this case we must discriminate by
// calling the IsFoo helper. This only applies when accessing via helpers (inter-assembly).
let mkRuntimeTypeDiscriminate (ilg: ILGlobals) (access: DataAccess) cuspec (alt: IlxUnionCase) altName altTy =
    if alt.IsNullary && access = DataAccess.ViaHelpers then
        let baseTy = baseTyOfUnionSpec cuspec

        [
            mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + mkTesterName altName, [], ilg.typ_Bool))
        ]
    else
        [ I_isinst altTy; AI_ldnull; AI_cgt_un ]

let mkRuntimeTypeDiscriminateThen ilg (access: DataAccess) cuspec (alt: IlxUnionCase) altName altTy after =
    let useHelper = alt.IsNullary && access = DataAccess.ViaHelpers

    match after with
    | I_brcmp(BI_brfalse, _)
    | I_brcmp(BI_brtrue, _) when not useHelper -> [ I_isinst altTy; after ]
    | _ -> mkRuntimeTypeDiscriminate ilg access cuspec alt altName altTy @ [ after ]

let mkGetTagFromField ilg _cuspec baseTy =
    mkNormalLdfld (refToFieldInTy baseTy (mkTagFieldId ilg))

let mkSetTagToField ilg _cuspec baseTy =
    mkNormalStfld (refToFieldInTy baseTy (mkTagFieldId ilg))

let adjustFieldNameForTypeDef hasHelpers nm =
    match hasHelpers with
    | SpecialFSharpListHelpers -> adjustFieldNameForList nm
    | _ -> nm

let adjustFieldName access nm =
    match access with
    | DataAccess.ViaListHelpers -> adjustFieldNameForList nm
    | _ -> nm

let mkLdData (access, cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAltIdx cuspec alt cidx
    let fieldDef = alt.FieldDef fidx

    match access with
    | DataAccess.RawFields -> mkNormalLdfld (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))
    | _ -> mkNormalCall (mkILNonGenericInstanceMethSpecInTy (altTy, "get_" + adjustFieldName access fieldDef.Name, [], fieldDef.Type))

let mkLdDataAddr (access, cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAltIdx cuspec alt cidx
    let fieldDef = alt.FieldDef fidx

    match access with
    | DataAccess.RawFields -> mkNormalLdflda (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))
    | _ -> failwith (sprintf "can't load address using helpers, for fieldDef %s" fieldDef.LowerName)

let mkGetTailOrNull access cuspec =
    mkLdData (access, cuspec, 1, 1) (* tail is in alternative 1, field number 1 *)

let mkGetTagFromHelpers ilg (cuspec: IlxUnionSpec) =
    let baseTy = baseTyOfUnionSpec cuspec

    match classifyFromSpec cuspec with
    | UnionLayout.SmallRefWithNullAsTrueValue _ ->
        mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "Get" + tagPropertyName, [ baseTy ], mkTagFieldType ilg))
    | _ -> mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + tagPropertyName, [], mkTagFieldType ilg))

let mkGetTag ilg (cuspec: IlxUnionSpec) =
    match cuspec.HasHelpers with
    | AllHelpers -> mkGetTagFromHelpers ilg cuspec
    | _hasHelpers -> mkGetTagFromField ilg cuspec (baseTyOfUnionSpec cuspec)

let mkCeqThen after =
    match after with
    | I_brcmp(BI_brfalse, a) -> [ I_brcmp(BI_bne_un, a) ]
    | I_brcmp(BI_brtrue, a) -> [ I_brcmp(BI_beq, a) ]
    | _ -> [ AI_ceq; after ]

let mkTagDiscriminate ilg cuspec _baseTy cidx =
    [ mkGetTag ilg cuspec; mkLdcInt32 cidx; AI_ceq ]

let mkTagDiscriminateThen ilg cuspec cidx after =
    [ mkGetTag ilg cuspec; mkLdcInt32 cidx ] @ mkCeqThen after

let private emitRawConstruction ilg cuspec (layout: UnionLayout) cidx =
    let baseTy = baseTyOfUnionSpec cuspec
    let ci = resolveCaseWith layout baseTy cuspec cidx
    let storage = classifyCaseStorage layout cuspec cidx ci.Case

    match storage with
    | CaseStorage.Null ->
        // Null-represented case: just load null
        [ AI_ldnull ]
    | CaseStorage.Singleton ->
        // Nullary ref type: load the singleton static field
        [ I_ldsfld(Nonvolatile, mkConstFieldSpec ci.CaseName baseTy) ]
    | CaseStorage.OnRoot ->

        if ci.Case.IsNullary then
            match layout with
            | HasTagField ->
                // Multi-case struct nullary: create via root ctor with tag
                let tagField = [ mkTagFieldType ilg ]
                [ mkLdcInt32 cidx; mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, tagField)) ]
            | NoTagField ->
                // Single-case nullary: create via parameterless root ctor
                [ mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, [])) ]
        else
            // Non-nullary fields on root: create via root ctor with fields
            let ctorFieldTys = ci.Case.FieldTypes |> Array.toList
            [ mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, ctorFieldTys)) ]
    | CaseStorage.InNestedType _ ->
        // Case lives in a nested subtype
        [
            mkNormalNewobj (mkILCtorMethSpecForTy (ci.CaseType, Array.toList ci.Case.FieldTypes))
        ]

let emitRawNewData ilg cuspec cidx =
    emitRawConstruction ilg cuspec (classifyFromSpec cuspec) cidx

// The stdata 'instruction' is only ever used for the F# "List" type within FSharp.Core.dll
let mkStData (cuspec, cidx, fidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAltIdx cuspec alt cidx
    let fieldDef = alt.FieldDef fidx
    mkNormalStfld (mkILFieldSpecInTy (altTy, fieldDef.LowerName, fieldDef.Type))

let mkNewData ilg (cuspec, cidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altName = alt.Name
    let baseTy = baseTyOfUnionSpec cuspec
    let layout = classifyFromSpec cuspec

    let viaMakerCall () =
        [
            mkNormalCall (
                mkILNonGenericStaticMethSpecInTy (
                    baseTy,
                    mkMakerName cuspec altName,
                    Array.toList alt.FieldTypes,
                    constFormalFieldTy baseTy
                )
            )
        ]

    let viaGetAltNameProperty () =
        [
            mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "get_" + altName, [], constFormalFieldTy baseTy))
        ]

    // If helpers exist, use them
    match cuspec.HasHelpers with
    | AllHelpers
    | SpecialFSharpListHelpers
    | SpecialFSharpOptionHelpers ->
        match layout, cidx with
        | CaseIsNull -> [ AI_ldnull ]
        | _ ->
            if alt.IsNullary then
                viaGetAltNameProperty ()
            else
                viaMakerCall ()

    | NoHelpers ->
        match layout, cidx with
        | CaseIsNull -> [ AI_ldnull ]
        | _ ->
            match layout with
            // Struct non-nullary: use maker method (handles initobj + field stores)
            | ValueTypeLayout when not alt.IsNullary -> viaMakerCall ()
            // Ref nullary (not null-represented): use property accessor for singleton
            | ReferenceTypeLayout when alt.IsNullary -> viaGetAltNameProperty ()
            // Everything else: raw construction
            | _ -> emitRawConstruction ilg cuspec layout cidx

let private emitIsCase ilg access cuspec (layout: UnionLayout) cidx =
    let baseTy = baseTyOfUnionSpec cuspec
    let ci = resolveCaseWith layout baseTy cuspec cidx
    let storage = classifyCaseStorage layout cuspec cidx ci.Case

    match storage with
    | CaseStorage.Null ->
        // Null-represented case: compare with null
        [ AI_ldnull; AI_ceq ]
    | _ ->
        match storage, layout with
        // Single non-nullary folded to root with null siblings: test non-null
        | CaseStorage.OnRoot, DiscriminateByRuntimeType _ -> [ AI_ldnull; AI_cgt_un ]
        | _, NoDiscrimination _ -> [ mkLdcInt32 1 ]
        | _, DiscriminateByRuntimeType _ -> mkRuntimeTypeDiscriminate ilg access cuspec ci.Case ci.CaseName ci.CaseType
        | _, DiscriminateByTagField baseTy -> mkTagDiscriminate ilg cuspec baseTy cidx
        | _, DiscriminateByTailNull _ ->
            match cidx with
            | TagNil -> [ mkGetTailOrNull access cuspec; AI_ldnull; AI_ceq ]
            | TagCons -> [ mkGetTailOrNull access cuspec; AI_ldnull; AI_cgt_un ]
            | _ -> failwith "emitIsCase - unexpected list case index"

let mkIsData ilg (access, cuspec, cidx) =
    let layout = classifyFromSpec cuspec
    emitIsCase ilg access cuspec layout cidx

type ICodeGen<'Mark> =
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark -> unit
    abstract EmitInstr: ILInstr -> unit
    abstract EmitInstrs: ILInstr list -> unit
    abstract MkInvalidCastExnNewobj: unit -> ILInstr

let genWith g : ILCode =
    let instrs = ResizeArray()
    let lab2pc = Dictionary()

    g
        { new ICodeGen<ILCodeLabel> with
            member _.CodeLabel(m) = m
            member _.GenerateDelayMark() = generateCodeLabel ()
            member _.GenLocal(ilTy) = failwith "not needed"
            member _.SetMarkToHere(m) = lab2pc[m] <- instrs.Count
            member _.EmitInstr x = instrs.Add x

            member cg.EmitInstrs xs =
                for i in xs do
                    cg.EmitInstr i

            member _.MkInvalidCastExnNewobj() = failwith "not needed"
        }

    {
        Labels = lab2pc
        Instrs = instrs.ToArray()
        Exceptions = []
        Locals = []
    }

let private emitBranchOnCase ilg sense access cuspec (layout: UnionLayout) cidx tg =
    let neg = (if sense then BI_brfalse else BI_brtrue)
    let pos = (if sense then BI_brtrue else BI_brfalse)
    let baseTy = baseTyOfUnionSpec cuspec
    let ci = resolveCaseWith layout baseTy cuspec cidx
    let storage = classifyCaseStorage layout cuspec cidx ci.Case

    match storage with
    | CaseStorage.Null ->
        // Null-represented case: branch on null
        [ I_brcmp(neg, tg) ]
    | _ ->
        match storage, layout with
        // Single non-nullary folded to root with null siblings: branch on non-null
        | CaseStorage.OnRoot, DiscriminateByRuntimeType _ -> [ I_brcmp(pos, tg) ]
        | _, NoDiscrimination _ -> []
        | _, DiscriminateByRuntimeType _ ->
            mkRuntimeTypeDiscriminateThen ilg access cuspec ci.Case ci.CaseName ci.CaseType (I_brcmp(pos, tg))
        | _, DiscriminateByTagField _ -> mkTagDiscriminateThen ilg cuspec cidx (I_brcmp(pos, tg))
        | _, DiscriminateByTailNull _ ->
            match cidx with
            | TagNil -> [ mkGetTailOrNull access cuspec; I_brcmp(neg, tg) ]
            | TagCons -> [ mkGetTailOrNull access cuspec; I_brcmp(pos, tg) ]
            | _ -> failwith "emitBranchOnCase - unexpected list case index"

let mkBrIsData ilg sense (access, cuspec, cidx, tg) =
    let layout = classifyFromSpec cuspec
    emitBranchOnCase ilg sense access cuspec layout cidx tg

let emitLdDataTagPrim ilg ldOpt (cg: ICodeGen<'Mark>) (access, cuspec: IlxUnionSpec) =
    match access with
    | DataAccess.ViaHelpers
    | DataAccess.ViaListHelpers ->
        ldOpt |> Option.iter cg.EmitInstr
        cg.EmitInstr(mkGetTagFromHelpers ilg cuspec)
    | DataAccess.RawFields
    | DataAccess.ViaOptionHelpers ->

        let layout = classifyFromSpec cuspec
        let alts = cuspec.AlternativesArray

        match layout with
        | DiscriminateByTailNull _ ->
            // leaves 1 if cons, 0 if not
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ mkGetTailOrNull access cuspec; AI_ldnull; AI_cgt_un ]
        | DiscriminateByTagField baseTy ->
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstr(mkGetTagFromField ilg cuspec baseTy)
        | NoDiscrimination _ ->
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ AI_pop; mkLdcInt32 0 ]
        | DiscriminateByRuntimeType(baseTy, nullAsTrueValueIdx) ->
            // RuntimeTypes: emit multi-way isinst chain
            let ld =
                match ldOpt with
                | None ->
                    let locn = cg.GenLocal baseTy
                    cg.EmitInstr(mkStloc locn)
                    mkLdloc locn
                | Some i -> i

            let outlab = cg.GenerateDelayMark()

            let emitCase cidx =
                let alt = altOfUnionSpec cuspec cidx
                let internalLab = cg.GenerateDelayMark()
                let failLab = cg.GenerateDelayMark()
                let cmpNull = (nullAsTrueValueIdx = Some cidx)

                let test =
                    I_brcmp((if cmpNull then BI_brtrue else BI_brfalse), cg.CodeLabel failLab)

                let testBlock =
                    if cmpNull || caseFieldsOnRoot layout alt cuspec.AlternativesArray then
                        [ test ]
                    else
                        let altName = alt.Name
                        let altTy = tyForAltIdxWith layout baseTy cuspec alt cidx
                        mkRuntimeTypeDiscriminateThen ilg access cuspec alt altName altTy test

                cg.EmitInstrs(ld :: testBlock)
                cg.SetMarkToHere internalLab
                cg.EmitInstrs [ mkLdcInt32 cidx; I_br(cg.CodeLabel outlab) ]
                cg.SetMarkToHere failLab

            // Emit type tests in reverse order; case 0 is the fallback (loaded after the loop).
            for n in alts.Length - 1 .. -1 .. 1 do
                emitCase n

            // Make the block for the last test.
            cg.EmitInstr(mkLdcInt32 0)
            cg.SetMarkToHere outlab

let emitLdDataTag ilg (cg: ICodeGen<'Mark>) (access, cuspec: IlxUnionSpec) =
    emitLdDataTagPrim ilg None cg (access, cuspec)

let private emitCastToCase ilg (cg: ICodeGen<'Mark>) canfail access cuspec (layout: UnionLayout) cidx =
    let baseTy = baseTyOfUnionSpec cuspec
    let ci = resolveCaseWith layout baseTy cuspec cidx
    let storage = classifyCaseStorage layout cuspec cidx ci.Case

    match storage with
    | CaseStorage.Null ->
        // Null-represented case
        if canfail then
            let outlab = cg.GenerateDelayMark()
            let internal1 = cg.GenerateDelayMark()
            cg.EmitInstrs [ AI_dup; I_brcmp(BI_brfalse, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs [ cg.MkInvalidCastExnNewobj(); I_throw ]
            cg.SetMarkToHere outlab
    | CaseStorage.OnRoot ->
        // Fields on root: tag check if canfail for structs, else leave on stack
        match layout with
        | ValueTypeLayout when canfail ->
            let outlab = cg.GenerateDelayMark()
            let internal1 = cg.GenerateDelayMark()
            cg.EmitInstr AI_dup
            emitLdDataTagPrim ilg None cg (access, cuspec)
            cg.EmitInstrs [ mkLdcInt32 cidx; I_brcmp(BI_beq, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs [ cg.MkInvalidCastExnNewobj(); I_throw ]
            cg.SetMarkToHere outlab
        | _ -> ()
    | CaseStorage.Singleton ->
        // Nullary case with singleton field on root class, no cast needed
        ()
    | CaseStorage.InNestedType altTy ->
        // Case lives in a nested subtype: emit castclass
        cg.EmitInstr(I_castclass altTy)

let emitCastData ilg (cg: ICodeGen<'Mark>) (canfail, access, cuspec, cidx) =
    let layout = classifyFromSpec cuspec
    emitCastToCase ilg cg canfail access cuspec layout cidx

let private emitCaseSwitch ilg (cg: ICodeGen<'Mark>) access cuspec (layout: UnionLayout) cases =
    match layout with
    | DiscriminateByRuntimeType(baseTy, nullAsTrueValueIdx) ->
        let locn = cg.GenLocal baseTy

        cg.EmitInstr(mkStloc locn)

        for cidx, tg in cases do
            let alt = altOfUnionSpec cuspec cidx
            let altTy = tyForAltIdxWith layout baseTy cuspec alt cidx
            let altName = alt.Name
            let failLab = cg.GenerateDelayMark()
            let cmpNull = (nullAsTrueValueIdx = Some cidx)

            cg.EmitInstr(mkLdloc locn)
            let testInstr = I_brcmp((if cmpNull then BI_brfalse else BI_brtrue), tg)

            if cmpNull || caseFieldsOnRoot layout alt cuspec.AlternativesArray then
                cg.EmitInstr testInstr
            else
                cg.EmitInstrs(mkRuntimeTypeDiscriminateThen ilg access cuspec alt altName altTy testInstr)

            cg.SetMarkToHere failLab

    | DiscriminateByTagField _ ->
        match cases with
        | [] -> cg.EmitInstr AI_pop
        | _ ->
            let dict = Dictionary<int, _>()

            for i, case in cases do
                dict[i] <- case

            let failLab = cg.GenerateDelayMark()

            let emitCase i _ =
                match dict.TryGetValue i with
                | true, res -> res
                | _ -> cg.CodeLabel failLab

            let dests = Array.mapi emitCase cuspec.AlternativesArray
            cg.EmitInstr(mkGetTag ilg cuspec)
            cg.EmitInstr(I_switch(Array.toList dests))
            cg.SetMarkToHere failLab

    | NoDiscrimination _ ->
        match cases with
        | [ (0, tg) ] -> cg.EmitInstrs [ AI_pop; I_br tg ]
        | [] -> cg.EmitInstr AI_pop
        | _ -> failwith "unexpected: strange switch on single-case unions should not be present"

    | DiscriminateByTailNull _ -> failwith "unexpected: switches on lists should have been eliminated to brisdata tests"

let emitDataSwitch ilg (cg: ICodeGen<'Mark>) (access, cuspec, cases) =
    let layout = classifyFromSpec cuspec
    emitCaseSwitch ilg cg access cuspec layout cases
