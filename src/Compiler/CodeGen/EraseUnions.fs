// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Erase discriminated unions.
module internal FSharp.Compiler.AbstractIL.ILX.EraseUnions

open FSharp.Compiler.IlxGenSupport

open System.Collections.Generic
open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

/// How to access union data at a given call site.
/// Combines the per-call-site 'avoidHelpers' flag with the per-union 'HasHelpers' setting
/// into a single value computed once at the entry point.
[<RequireQualifiedAccess>]
type DataAccess =
    /// Use raw field loads/stores (intra-assembly access, or union has no helpers)
    | RawFields
    /// Use helper methods (get_Tag, get_IsXxx, NewXxx) — inter-assembly with AllHelpers or SpecialFSharpOptionHelpers
    | ViaHelpers
    /// Use list-specific helper methods (HeadOrDefault, TailOrNull naming) — inter-assembly with SpecialFSharpListHelpers
    | ViaListHelpers

/// Compute the access strategy from the per-call-site flag and per-union helpers setting.
let computeDataAccess (avoidHelpers: bool) (cuspec: IlxUnionSpec) =
    if avoidHelpers then
        DataAccess.RawFields
    else
        match cuspec.HasHelpers with
        | IlxUnionHasHelpers.NoHelpers -> DataAccess.RawFields
        | IlxUnionHasHelpers.AllHelpers
        | IlxUnionHasHelpers.SpecialFSharpOptionHelpers -> DataAccess.ViaHelpers
        | IlxUnionHasHelpers.SpecialFSharpListHelpers -> DataAccess.ViaListHelpers

[<Literal>]
let TagNil = 0

[<Literal>]
let TagCons = 1

[<Literal>]
let ALT_NAME_CONS = "Cons"

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type UnionLayout =
    /// F# list<'a> only. Discrimination via tail field == null.
    | FSharpList of baseTy: ILType
    /// Single case, reference type. No discrimination needed.
    | SingleCaseRef of baseTy: ILType
    /// Single case, struct. No discrimination needed.
    | SingleCaseStruct of baseTy: ILType
    /// 2-3 cases, reference, not all-nullary, no null-as-true-value. Discrimination via isinst.
    | SmallRef of baseTy: ILType
    /// 2-3 cases, reference, not all-nullary, one case represented as null. Discrimination via isinst.
    | SmallRefWithNullAsTrueValue of baseTy: ILType * nullAsTrueValueIdx: int
    /// ≥4 cases (or 2-3 all-nullary), reference, not all nullary. Discrimination via integer _tag field.
    | TaggedRef of baseTy: ILType
    /// ≥4 cases (or 2-3 all-nullary), reference, all nullary. Discrimination via integer _tag field.
    | TaggedRefAllNullary of baseTy: ILType
    /// Struct DU with >1 case, not all nullary. Discrimination via integer _tag field.
    | TaggedStruct of baseTy: ILType
    /// Struct DU with >1 case, all nullary. Discrimination via integer _tag field.
    | TaggedStructAllNullary of baseTy: ILType

let baseTyOfUnionSpec (cuspec: IlxUnionSpec) =
    mkILNamedTy cuspec.Boxity cuspec.TypeRef cuspec.GenericArgs

let mkMakerName (cuspec: IlxUnionSpec) nm =
    match cuspec.HasHelpers with
    | SpecialFSharpListHelpers
    | SpecialFSharpOptionHelpers -> nm // Leave 'Some', 'None', 'Cons', 'Empty' as is
    | AllHelpers
    | NoHelpers -> "New" + nm

let mkCasesTypeRef (cuspec: IlxUnionSpec) = cuspec.TypeRef

/// Core classification logic. Computes the UnionLayout for any union.
let private classifyUnion baseTy (alts: IlxUnionCase[]) nullPermitted isList isStruct =
    let allNullary = alts |> Array.forall (fun alt -> alt.IsNullary)

    match isList, alts.Length, isStruct with
    | true, _, _ -> UnionLayout.FSharpList baseTy
    | _, 1, true -> UnionLayout.SingleCaseStruct baseTy
    | _, 1, false -> UnionLayout.SingleCaseRef baseTy
    | _, n, false when n < 4 && not allNullary ->
        // Small ref union (2-3 cases, not all nullary): discriminate by isinst
        let nullAsTrueValueIdx =
            if
                nullPermitted
                && alts |> Array.existsOne (fun alt -> alt.IsNullary)
                && alts |> Array.exists (fun alt -> not alt.IsNullary)
            then
                alts |> Array.tryFindIndex (fun alt -> alt.IsNullary)
            else
                None

        match nullAsTrueValueIdx with
        | Some idx -> UnionLayout.SmallRefWithNullAsTrueValue(baseTy, idx)
        | None -> UnionLayout.SmallRef baseTy
    | _ ->
        match isStruct, allNullary with
        | true, true -> UnionLayout.TaggedStructAllNullary baseTy
        | true, false -> UnionLayout.TaggedStruct baseTy
        | false, true -> UnionLayout.TaggedRefAllNullary baseTy
        | false, false -> UnionLayout.TaggedRef baseTy

/// Classify from an IlxUnionSpec (used in IL instruction generation).
let classifyFromSpec (cuspec: IlxUnionSpec) =
    let baseTy = baseTyOfUnionSpec cuspec
    let alts = cuspec.AlternativesArray
    let nullPermitted = cuspec.IsNullPermitted
    let isList = (cuspec.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers)
    let isStruct = (cuspec.Boxity = ILBoxity.AsValue)
    classifyUnion baseTy alts nullPermitted isList isStruct

/// Classify from an ILTypeDef + IlxUnionInfo (used in type definition generation).
let classifyFromDef (td: ILTypeDef) (cud: IlxUnionInfo) (baseTy: ILType) =
    let alts = cud.UnionCases
    let nullPermitted = cud.IsNullPermitted
    let isList = (cud.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers)
    let isStruct = td.IsStruct
    classifyUnion baseTy alts nullPermitted isList isStruct

// ---- Exhaustive Active Patterns for UnionLayout ----

/// How to discriminate between cases at runtime.
let (|DiscriminateByTagField|DiscriminateByRuntimeType|DiscriminateByTailNull|NoDiscrimination|) layout =
    match layout with
    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> DiscriminateByTagField
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _ -> DiscriminateByRuntimeType
    | UnionLayout.FSharpList _ -> DiscriminateByTailNull
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _ -> NoDiscrimination

/// Does the root type have a _tag integer field?
let (|HasTagField|NoTagField|) layout =
    match layout with
    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> HasTagField
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _
    | UnionLayout.FSharpList _
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _ -> NoTagField

/// Where are case fields stored?
let (|FieldsOnRootType|FieldsOnNestedTypes|) layout =
    match layout with
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.FSharpList _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> FieldsOnRootType
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _
    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _ -> FieldsOnNestedTypes

/// Is a specific case (by index) represented as null?
let (|CaseIsNull|CaseIsAllocated|) (layout, cidx) =
    match layout with
    | UnionLayout.SmallRefWithNullAsTrueValue(_, nullIdx) when nullIdx = cidx -> CaseIsNull
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _
    | UnionLayout.FSharpList _
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> CaseIsAllocated

/// Is this a value type (struct) or reference type layout?
let (|ValueTypeLayout|ReferenceTypeLayout|) layout =
    match layout with
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> ValueTypeLayout
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _
    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _
    | UnionLayout.FSharpList _ -> ReferenceTypeLayout

/// Does a non-nullary case fold its fields into the root class (no nested type)?
let (|NonNullaryFoldsToRoot|NonNullaryInNestedType|) (layout, alt: IlxUnionCase) =
    match layout with
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _
    | UnionLayout.FSharpList _ -> NonNullaryFoldsToRoot
    | UnionLayout.TaggedRefAllNullary _ -> NonNullaryFoldsToRoot
    | UnionLayout.TaggedRef _ when not alt.IsNullary -> NonNullaryInNestedType
    | UnionLayout.TaggedRef _ -> NonNullaryFoldsToRoot
    | UnionLayout.SmallRef _ when not alt.IsNullary -> NonNullaryInNestedType
    | UnionLayout.SmallRef _ -> NonNullaryFoldsToRoot
    | UnionLayout.SmallRefWithNullAsTrueValue _ when not alt.IsNullary -> NonNullaryInNestedType
    | UnionLayout.SmallRefWithNullAsTrueValue _ -> NonNullaryFoldsToRoot

/// Compile-time validation that all active patterns cover all UnionLayout cases.
/// Also validates that classifyFromSpec and classifyFromDef compile correctly.
let private _validateActivePatterns
    (layout: UnionLayout)
    (alt: IlxUnionCase)
    (cuspec: IlxUnionSpec)
    (td: ILTypeDef)
    (cud: IlxUnionInfo)
    (baseTy: ILType)
    =
    let _fromSpec = classifyFromSpec cuspec
    let _fromDef = classifyFromDef td cud baseTy

    match layout with
    | DiscriminateByTagField
    | DiscriminateByRuntimeType
    | DiscriminateByTailNull
    | NoDiscrimination -> ()

    match layout with
    | HasTagField
    | NoTagField -> ()

    match layout with
    | FieldsOnRootType
    | FieldsOnNestedTypes -> ()

    match layout, 0 with
    | CaseIsNull
    | CaseIsAllocated -> ()

    match layout with
    | ValueTypeLayout
    | ReferenceTypeLayout -> ()

    match layout, alt with
    | NonNullaryFoldsToRoot
    | NonNullaryInNestedType -> ()

// ---- Layout-Based Helpers ----
// These replace the old representation decision methods.

/// Does this non-nullary alternative fold to root class via fresh instances?
/// Equivalent to the old RepresentAlternativeAsFreshInstancesOfRootClass.
let private caseFieldsOnRoot (layout: UnionLayout) (alt: IlxUnionCase) (alts: IlxUnionCase[]) =
    not alt.IsNullary
    && (match layout with
        | UnionLayout.FSharpList _ -> alt.Name = ALT_NAME_CONS
        | UnionLayout.SingleCaseRef _ -> true
        | UnionLayout.SmallRefWithNullAsTrueValue _ -> alts |> Array.filter (fun a -> not a.IsNullary) |> Array.length = 1
        | UnionLayout.SmallRef _
        | UnionLayout.SingleCaseStruct _
        | UnionLayout.TaggedRef _
        | UnionLayout.TaggedRefAllNullary _
        | UnionLayout.TaggedStruct _
        | UnionLayout.TaggedStructAllNullary _ -> false)

/// Does this alternative optimize to root class (no nested type needed)?
/// Equivalent to the old OptimizeAlternativeToRootClass.
let private caseRepresentedOnRoot (layout: UnionLayout) (alt: IlxUnionCase) (alts: IlxUnionCase[]) (cidx: int) =
    match layout with
    | UnionLayout.FSharpList _
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ -> true
    | UnionLayout.TaggedRefAllNullary _ -> true
    | UnionLayout.TaggedRef _ -> alt.IsNullary
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _ ->
        (match layout, cidx with
         | CaseIsNull -> true
         | CaseIsAllocated -> false)
        || caseFieldsOnRoot layout alt alts

/// Should a static constant field be maintained for this nullary alternative?
/// Equivalent to the old MaintainPossiblyUniqueConstantFieldForAlternative.
/// Only for nullary cases on reference types that are not null-represented.
let private needsSingletonField (layout: UnionLayout) (alt: IlxUnionCase) (cidx: int) =
    alt.IsNullary
    && match layout, cidx with
       | CaseIsNull -> false
       | _ ->
           match layout with
           | ReferenceTypeLayout -> true
           | ValueTypeLayout -> false

let private tyForAltIdx cuspec (alt: IlxUnionCase) cidx =
    let layout = classifyFromSpec cuspec
    let baseTy = baseTyOfUnionSpec cuspec

    if caseRepresentedOnRoot layout alt cuspec.AlternativesArray cidx then
        baseTy
    else
        let isList = (cuspec.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers)
        let altName = alt.Name
        let nm = if alt.IsNullary || isList then "_" + altName else altName
        mkILNamedTy cuspec.Boxity (mkILTyRefInTyRef (mkCasesTypeRef cuspec, nm)) cuspec.GenericArgs

/// How a specific union case is physically stored.
[<RequireQualifiedAccess>]
type CaseStorage =
    /// Represented as null reference (UseNullAsTrueValue)
    | Null
    /// Singleton static field on root class (nullary, reference type)
    | Singleton
    /// Fields stored directly on root class (single-case, list cons, struct, folded SmallRef)
    | OnRoot
    /// Fields stored in a nested subtype
    | InNestedType of nestedType: ILType

let classifyCaseStorage (layout: UnionLayout) (cuspec: IlxUnionSpec) (cidx: int) (alt: IlxUnionCase) =
    match layout, cidx with
    | CaseIsNull -> CaseStorage.Null
    | _ ->
        if caseRepresentedOnRoot layout alt cuspec.AlternativesArray cidx then
            if alt.IsNullary then
                match layout with
                | ValueTypeLayout -> CaseStorage.OnRoot
                | ReferenceTypeLayout -> CaseStorage.Singleton
            else
                CaseStorage.OnRoot
        elif needsSingletonField layout alt cidx then
            CaseStorage.Singleton
        else
            CaseStorage.InNestedType(tyForAltIdx cuspec alt cidx)

// ---- Context Records ----

/// Bundles the IL attribute-stamping callbacks used during type definition generation.
type ILStamping =
    {
        stampMethodAsGenerated: ILMethodDef -> ILMethodDef
        stampPropertyAsGenerated: ILPropertyDef -> ILPropertyDef
        stampPropertyAsNever: ILPropertyDef -> ILPropertyDef
        stampFieldAsGenerated: ILFieldDef -> ILFieldDef
        stampFieldAsNever: ILFieldDef -> ILFieldDef
        mkDebuggerTypeProxyAttr: ILType -> ILAttribute
    }

/// Bundles the parameters threaded through type definition generation.
/// Replaces the 6-callback tuple + scattered parameter threading in convAlternativeDef/mkClassUnionDef.
type TypeDefContext =
    {
        g: TcGlobals
        layout: UnionLayout
        cuspec: IlxUnionSpec
        cud: IlxUnionInfo
        td: ILTypeDef
        baseTy: ILType
        stamping: ILStamping
    }

/// Information about a nullary case's singleton static field.
type NullaryConstFieldInfo =
    {
        Case: IlxUnionCase
        CaseType: ILType
        CaseIndex: int
        Field: ILFieldDef
        InRootClass: bool
    }

/// Result of processing a single union alternative for type definition generation.
/// Replaces the 6-element tuple return from convAlternativeDef.
type AlternativeDefResult =
    {
        BaseMakerMethods: ILMethodDef list
        BaseMakerProperties: ILPropertyDef list
        ConstantAccessors: ILMethodDef list
        NestedTypeDefs: ILTypeDef list
        DebugProxyTypeDefs: ILTypeDef list
        NullaryConstFields: NullaryConstFieldInfo list
    }

let mkTesterName nm = "Is" + nm

let tagPropertyName = "Tag"

let mkUnionCaseFieldId (fdef: IlxUnionCaseField) =
    // Use the lower case name of a field or constructor as the field/parameter name if it differs from the uppercase name
    fdef.LowerName, fdef.Type

/// Is nullness checking enabled in the compiler settings?
let inline nullnessCheckingEnabled (g: TcGlobals) =
    g.checkNullness && g.langFeatureNullness

let inline getFieldsNullability (g: TcGlobals) (ilf: ILFieldDef) =
    if g.checkNullness then
        ilf.CustomAttrs.AsArray()
        |> Array.tryFind (IsILAttrib g.attrib_NullableAttribute)
    else
        None

let mkUnionCaseFieldIdAndAttrs g fdef =
    let nm, t = mkUnionCaseFieldId fdef
    let attrs = getFieldsNullability g fdef.ILField
    nm, t, attrs |> Option.toList

let refToFieldInTy ty (nm, fldTy) = mkILFieldSpecInTy (ty, nm, fldTy)

let formalTypeArgs (baseTy: ILType) =
    List.mapi (fun i _ -> mkILTyvarTy (uint16 i)) baseTy.GenericArgs

let constFieldName nm = "_unique_" + nm

let constFormalFieldTy (baseTy: ILType) =
    mkILNamedTy baseTy.Boxity baseTy.TypeRef (formalTypeArgs baseTy)

let mkConstFieldSpecFromId (baseTy: ILType) constFieldId = refToFieldInTy baseTy constFieldId

let mkConstFieldSpec nm (baseTy: ILType) =
    mkConstFieldSpecFromId baseTy (constFieldName nm, constFormalFieldTy baseTy)

let tyForAlt (cuspec: IlxUnionSpec) (alt: IlxUnionCase) =
    let cidx =
        cuspec.AlternativesArray
        |> Array.findIndex (fun (a: IlxUnionCase) -> a.Name = alt.Name)

    tyForAltIdx cuspec alt cidx

let GetILTypeForAlternative cuspec alt =
    tyForAlt cuspec (cuspec.Alternative alt)

let mkTagFieldType (ilg: ILGlobals) _cuspec = ilg.typ_Int32

let mkTagFieldId ilg cuspec = "_tag", mkTagFieldType ilg cuspec

let altOfUnionSpec (cuspec: IlxUnionSpec) cidx =
    try
        cuspec.Alternative cidx
    with _ ->
        failwith ("alternative " + string cidx + " not found")

/// Resolved identity of a union case within a union spec.
type CaseIdentity =
    {
        Index: int
        Case: IlxUnionCase
        CaseType: ILType
        CaseName: string
    }

/// Resolve a case by index, computing its type and name.
let resolveCase (cuspec: IlxUnionSpec) (cidx: int) =
    let alt = altOfUnionSpec cuspec cidx

    {
        Index = cidx
        Case = alt
        CaseType = tyForAltIdx cuspec alt cidx
        CaseName = alt.Name
    }

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

let mkGetTagFromField ilg cuspec baseTy =
    mkNormalLdfld (refToFieldInTy baseTy (mkTagFieldId ilg cuspec))

let mkSetTagToField ilg cuspec baseTy =
    mkNormalStfld (refToFieldInTy baseTy (mkTagFieldId ilg cuspec))

let adjustFieldNameForTypeDef hasHelpers nm =
    match hasHelpers, nm with
    | SpecialFSharpListHelpers, "Head" -> "HeadOrDefault"
    | SpecialFSharpListHelpers, "Tail" -> "TailOrNull"
    | _ -> nm

let adjustFieldName access nm =
    match access, nm with
    | DataAccess.ViaListHelpers, "Head" -> "HeadOrDefault"
    | DataAccess.ViaListHelpers, "Tail" -> "TailOrNull"
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
        mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "Get" + tagPropertyName, [ baseTy ], mkTagFieldType ilg cuspec))
    | _ -> mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + tagPropertyName, [], mkTagFieldType ilg cuspec))

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
    let ci = resolveCase cuspec cidx
    let storage = classifyCaseStorage layout cuspec cidx ci.Case

    match storage with
    | CaseStorage.Null ->
        // Null-represented case: just load null
        [ AI_ldnull ]
    | CaseStorage.Singleton ->
        // Nullary ref type: load the singleton static field
        let baseTy = baseTyOfUnionSpec cuspec
        [ I_ldsfld(Nonvolatile, mkConstFieldSpec ci.CaseName baseTy) ]
    | CaseStorage.OnRoot ->
        let baseTy = baseTyOfUnionSpec cuspec

        if ci.Case.IsNullary then
            // Struct + nullary: create via root ctor with tag
            let tagField = [ mkTagFieldType ilg cuspec ]
            [ mkLdcInt32 cidx; mkNormalNewobj (mkILCtorMethSpecForTy (baseTy, tagField)) ]
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
    let ci = resolveCase cuspec cidx

    match layout, cidx with
    | CaseIsNull ->
        // Null-represented case: compare with null
        [ AI_ldnull; AI_ceq ]
    | _ ->
        match layout with
        | UnionLayout.SmallRefWithNullAsTrueValue _ when caseFieldsOnRoot layout ci.Case cuspec.AlternativesArray ->
            // Single non-nullary with all null siblings: test via non-null
            [ AI_ldnull; AI_cgt_un ]
        | UnionLayout.SingleCaseRef _
        | UnionLayout.SingleCaseStruct _ -> [ mkLdcInt32 1 ]
        | UnionLayout.SmallRef _
        | UnionLayout.SmallRefWithNullAsTrueValue _ -> mkRuntimeTypeDiscriminate ilg access cuspec ci.Case ci.CaseName ci.CaseType
        | UnionLayout.TaggedRef _
        | UnionLayout.TaggedRefAllNullary _
        | UnionLayout.TaggedStruct _
        | UnionLayout.TaggedStructAllNullary _ -> mkTagDiscriminate ilg cuspec (baseTyOfUnionSpec cuspec) cidx
        | UnionLayout.FSharpList _ ->
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
    let ci = resolveCase cuspec cidx

    match layout, cidx with
    | CaseIsNull ->
        // Null-represented case: branch on null
        [ I_brcmp(neg, tg) ]
    | _ ->
        match layout with
        | UnionLayout.SmallRefWithNullAsTrueValue _ when caseFieldsOnRoot layout ci.Case cuspec.AlternativesArray ->
            // Single non-nullary with all null siblings: branch on non-null
            [ I_brcmp(pos, tg) ]
        | UnionLayout.SingleCaseRef _
        | UnionLayout.SingleCaseStruct _ -> []
        | UnionLayout.SmallRef _
        | UnionLayout.SmallRefWithNullAsTrueValue _ ->
            mkRuntimeTypeDiscriminateThen ilg access cuspec ci.Case ci.CaseName ci.CaseType (I_brcmp(pos, tg))
        | UnionLayout.TaggedRef _
        | UnionLayout.TaggedRefAllNullary _
        | UnionLayout.TaggedStruct _
        | UnionLayout.TaggedStructAllNullary _ -> mkTagDiscriminateThen ilg cuspec cidx (I_brcmp(pos, tg))
        | UnionLayout.FSharpList _ ->
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
    | DataAccess.RawFields ->

        let layout = classifyFromSpec cuspec
        let alts = cuspec.Alternatives

        match layout with
        | UnionLayout.FSharpList _ ->
            // leaves 1 if cons, 0 if not
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ mkGetTailOrNull access cuspec; AI_ldnull; AI_cgt_un ]
        | UnionLayout.TaggedRef baseTy
        | UnionLayout.TaggedRefAllNullary baseTy
        | UnionLayout.TaggedStruct baseTy
        | UnionLayout.TaggedStructAllNullary baseTy ->
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstr(mkGetTagFromField ilg cuspec baseTy)
        | UnionLayout.SingleCaseRef _
        | UnionLayout.SingleCaseStruct _ ->
            ldOpt |> Option.iter cg.EmitInstr
            cg.EmitInstrs [ AI_pop; mkLdcInt32 0 ]
        | UnionLayout.SmallRef baseTy
        | UnionLayout.SmallRefWithNullAsTrueValue(baseTy, _) ->
            // RuntimeTypes: emit multi-way isinst chain
            let nullAsTrueValueIdx =
                match layout with
                | UnionLayout.SmallRefWithNullAsTrueValue(_, idx) -> Some idx
                | _ -> None

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
                        let altTy = tyForAltIdx cuspec alt cidx
                        mkRuntimeTypeDiscriminateThen ilg access cuspec alt altName altTy test

                cg.EmitInstrs(ld :: testBlock)
                cg.SetMarkToHere internalLab
                cg.EmitInstrs [ mkLdcInt32 cidx; I_br(cg.CodeLabel outlab) ]
                cg.SetMarkToHere failLab

            // Make the blocks for the remaining tests.
            for n in alts.Length - 1 .. -1 .. 1 do
                emitCase n

            // Make the block for the last test.
            cg.EmitInstr(mkLdcInt32 0)
            cg.SetMarkToHere outlab

let emitLdDataTag ilg (cg: ICodeGen<'Mark>) (access, cuspec: IlxUnionSpec) =
    emitLdDataTagPrim ilg None cg (access, cuspec)

let private emitCastToCase ilg (cg: ICodeGen<'Mark>) canfail access cuspec (layout: UnionLayout) cidx =
    let ci = resolveCase cuspec cidx
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
    let baseTy = baseTyOfUnionSpec cuspec

    match layout with
    | UnionLayout.SmallRef _
    | UnionLayout.SmallRefWithNullAsTrueValue _ ->
        let nullAsTrueValueIdx =
            match layout with
            | UnionLayout.SmallRefWithNullAsTrueValue(_, idx) -> Some idx
            | _ -> None

        let locn = cg.GenLocal baseTy

        cg.EmitInstr(mkStloc locn)

        for cidx, tg in cases do
            let alt = altOfUnionSpec cuspec cidx
            let altTy = tyForAltIdx cuspec alt cidx
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

    | UnionLayout.TaggedRef _
    | UnionLayout.TaggedRefAllNullary _
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ ->
        match cases with
        | [] -> cg.EmitInstr AI_pop
        | _ ->
            // Use a dictionary to avoid quadratic lookup in case list
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

    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _ ->
        match cases with
        | [ (0, tg) ] -> cg.EmitInstrs [ AI_pop; I_br tg ]
        | [] -> cg.EmitInstr AI_pop
        | _ -> failwith "unexpected: strange switch on single-case unions should not be present"

    | UnionLayout.FSharpList _ -> failwith "unexpected: switches on lists should have been eliminated to brisdata tests"

let emitDataSwitch ilg (cg: ICodeGen<'Mark>) (access, cuspec, cases) =
    let layout = classifyFromSpec cuspec
    emitCaseSwitch ilg cg access cuspec layout cases

//---------------------------------------------------
// Generate the union classes

let mkMethodsAndPropertiesForFields
    (addMethodGeneratedAttrs, addPropertyGeneratedAttrs)
    (g: TcGlobals)
    access
    attr
    imports
    hasHelpers
    (ilTy: ILType)
    (fields: IlxUnionCaseField[])
    =
    let basicProps =
        fields
        |> Array.map (fun field ->
            ILPropertyDef(
                name = adjustFieldNameForTypeDef hasHelpers field.Name,
                attributes = PropertyAttributes.None,
                setMethod = None,
                getMethod =
                    Some(
                        mkILMethRef (
                            ilTy.TypeRef,
                            ILCallingConv.Instance,
                            "get_" + adjustFieldNameForTypeDef hasHelpers field.Name,
                            0,
                            [],
                            field.Type
                        )
                    ),
                callingConv = ILThisConvention.Instance,
                propertyType = field.Type,
                init = None,
                args = [],
                customAttrs = field.ILField.CustomAttrs
            )
            |> addPropertyGeneratedAttrs)
        |> Array.toList

    let basicMethods =
        [
            for field in fields do
                let fspec = mkILFieldSpecInTy (ilTy, field.LowerName, field.Type)

                let ilReturn = mkILReturn field.Type

                let ilReturn =
                    match getFieldsNullability g field.ILField with
                    | None -> ilReturn
                    | Some a -> ilReturn.WithCustomAttrs(mkILCustomAttrsFromArray [| a |])

                yield
                    mkILNonGenericInstanceMethod (
                        "get_" + adjustFieldNameForTypeDef hasHelpers field.Name,
                        access,
                        [],
                        ilReturn,
                        mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkLdarg 0us; mkNormalLdfld fspec ], attr, imports)
                    )
                    |> addMethodGeneratedAttrs

        ]

    basicProps, basicMethods

/// Generate a debug proxy type for a union alternative.
/// Returns (debugProxyTypeDefs, debugProxyAttrs).
let private emitDebugProxyType (ctx: TypeDefContext) (altTy: ILType) (fields: IlxUnionCaseField[]) =
    let g = ctx.g
    let td = ctx.td
    let baseTy = ctx.baseTy
    let cud = ctx.cud
    let imports = cud.DebugImports

    let debugProxyTypeName = altTy.TypeSpec.Name + "@DebugTypeProxy"

    let debugProxyTy =
        mkILBoxedTy (mkILNestedTyRef (altTy.TypeSpec.Scope, altTy.TypeSpec.Enclosing, debugProxyTypeName)) altTy.GenericArgs

    let debugProxyFieldName = "_obj"

    let debugProxyFields =
        [
            mkILInstanceField (debugProxyFieldName, altTy, None, ILMemberAccess.Assembly)
            |> ctx.stamping.stampFieldAsNever
            |> ctx.stamping.stampFieldAsGenerated
        ]

    let debugProxyCode =
        [
            mkLdarg0
            mkNormalCall (mkILCtorMethSpecForTy (g.ilg.typ_Object, []))
            mkLdarg0
            mkLdarg 1us
            mkNormalStfld (mkILFieldSpecInTy (debugProxyTy, debugProxyFieldName, altTy))
        ]
        |> nonBranchingInstrsToCode

    let debugProxyCtor =
        (mkILCtor (
            ILMemberAccess.Public (* must always be public - see jared parson blog entry on implementing debugger type proxy *) ,
            [ mkILParamNamed ("obj", altTy) ],
            mkMethodBody (false, [], 3, debugProxyCode, None, imports)
        ))
            .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
        |> ctx.stamping.stampMethodAsGenerated

    let debugProxyGetterMeths =
        fields
        |> Array.map (fun field ->
            let fldName, fldTy = mkUnionCaseFieldId field

            let instrs =
                [
                    mkLdarg0
                    (if td.IsStruct then mkNormalLdflda else mkNormalLdfld) (mkILFieldSpecInTy (debugProxyTy, debugProxyFieldName, altTy))
                    mkNormalLdfld (mkILFieldSpecInTy (altTy, fldName, fldTy))
                ]
                |> nonBranchingInstrsToCode

            let mbody = mkMethodBody (true, [], 2, instrs, None, imports)

            mkILNonGenericInstanceMethod ("get_" + field.Name, ILMemberAccess.Public, [], mkILReturn field.Type, mbody)
            |> ctx.stamping.stampMethodAsGenerated)
        |> Array.toList

    let debugProxyGetterProps =
        fields
        |> Array.map (fun fdef ->
            ILPropertyDef(
                name = fdef.Name,
                attributes = PropertyAttributes.None,
                setMethod = None,
                getMethod = Some(mkILMethRef (debugProxyTy.TypeRef, ILCallingConv.Instance, "get_" + fdef.Name, 0, [], fdef.Type)),
                callingConv = ILThisConvention.Instance,
                propertyType = fdef.Type,
                init = None,
                args = [],
                customAttrs = fdef.ILField.CustomAttrs
            )
            |> ctx.stamping.stampPropertyAsGenerated)
        |> Array.toList

    let debugProxyTypeDef =
        mkILGenericClass (
            debugProxyTypeName,
            ILTypeDefAccess.Nested ILMemberAccess.Assembly,
            td.GenericParams,
            g.ilg.typ_Object,
            [],
            mkILMethods ([ debugProxyCtor ] @ debugProxyGetterMeths),
            mkILFields debugProxyFields,
            emptyILTypeDefs,
            mkILProperties debugProxyGetterProps,
            emptyILEvents,
            emptyILCustomAttrs,
            ILTypeInit.BeforeField
        )

    [ debugProxyTypeDef.WithSpecialName(true) ],
    ([ ctx.stamping.mkDebuggerTypeProxyAttr debugProxyTy ]
     @ cud.DebugDisplayAttributes)

let private emitMakerMethod (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let g = ctx.g
    let baseTy = ctx.baseTy
    let cuspec = ctx.cuspec
    let cud = ctx.cud
    let fields = alt.FieldDefs
    let altName = alt.Name
    let imports = cud.DebugImports
    let attr = cud.DebugPoint

    let locals, ilInstrs =
        match ctx.layout with
        | ValueTypeLayout ->
            let local = mkILLocal baseTy None
            let ldloca = I_ldloca(0us)

            let ilInstrs =
                [
                    ldloca
                    ILInstr.I_initobj baseTy
                    match ctx.layout with
                    | HasTagField when num <> 0 ->
                        ldloca
                        mkLdcInt32 num
                        mkSetTagToField g.ilg cuspec baseTy
                    | _ -> ()
                    for i in 0 .. fields.Length - 1 do
                        ldloca
                        mkLdarg (uint16 i)
                        mkNormalStfld (mkILFieldSpecInTy (baseTy, fields[i].LowerName, fields[i].Type))
                    mkLdloc 0us
                ]

            [ local ], ilInstrs
        | ReferenceTypeLayout ->
            let ilInstrs =
                [
                    for i in 0 .. fields.Length - 1 do
                        mkLdarg (uint16 i)
                    yield! emitRawNewData g.ilg cuspec num
                ]

            [], ilInstrs

    mkILNonGenericStaticMethod (
        mkMakerName cuspec altName,
        cud.HelpersAccessibility,
        fields
        |> Array.map (fun fd ->
            let plainParam = mkILParamNamed (fd.LowerName, fd.Type)

            match getFieldsNullability g fd.ILField with
            | None -> plainParam
            | Some a ->
                { plainParam with
                    CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrsFromArray [| a |])
                })

        |> Array.toList,
        mkILReturn baseTy,
        mkMethodBody (true, locals, fields.Length + locals.Length, nonBranchingInstrsToCode ilInstrs, attr, imports)
    )
    |> (fun mdef -> mdef.With(customAttrs = alt.altCustomAttrs))
    |> ctx.stamping.stampMethodAsGenerated

let private emitTesterMethodAndProperty (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let g = ctx.g
    let cud = ctx.cud
    let cuspec = ctx.cuspec
    let baseTy = ctx.baseTy
    let altName = alt.Name
    let imports = cud.DebugImports
    let attr = cud.DebugPoint

    // No tester needed for single-case unions or null-discriminated (SmallRefWithNullAsTrueValue)
    match ctx.layout with
    | UnionLayout.SingleCaseRef _
    | UnionLayout.SingleCaseStruct _
    | UnionLayout.SmallRefWithNullAsTrueValue _ -> [], []
    | _ ->
        let additionalAttributes =
            match ctx.layout with
            | ValueTypeLayout when nullnessCheckingEnabled g && not alt.IsNullary ->
                let notnullfields =
                    alt.FieldDefs
                    // Fields that are nullable even from F# perspective has an [Nullable] attribute on them
                    // Non-nullable fields are implicit in F#, therefore not annotated separately
                    |> Array.filter (fun f ->
                        f.ILField.HasWellKnownAttribute(g, WellKnownILAttributes.NullableAttribute)
                        |> not)

                let fieldNames =
                    notnullfields
                    |> Array.map (fun f -> f.LowerName)
                    |> Array.append (notnullfields |> Array.map (fun f -> f.Name))

                if fieldNames |> Array.isEmpty then
                    emptyILCustomAttrs
                else
                    mkILCustomAttrsFromArray [| GetNotNullWhenTrueAttribute g fieldNames |]
            | _ -> emptyILCustomAttrs

        [
            (mkILNonGenericInstanceMethod (
                "get_" + mkTesterName altName,
                cud.HelpersAccessibility,
                [],
                mkILReturn g.ilg.typ_Bool,
                mkMethodBody (
                    true,
                    [],
                    2,
                    nonBranchingInstrsToCode ([ mkLdarg0 ] @ mkIsData g.ilg (DataAccess.RawFields, cuspec, num)),
                    attr,
                    imports
                )
            ))
                .With(customAttrs = additionalAttributes)
            |> ctx.stamping.stampMethodAsGenerated
        ],
        [
            ILPropertyDef(
                name = mkTesterName altName,
                attributes = PropertyAttributes.None,
                setMethod = None,
                getMethod = Some(mkILMethRef (baseTy.TypeRef, ILCallingConv.Instance, "get_" + mkTesterName altName, 0, [], g.ilg.typ_Bool)),
                callingConv = ILThisConvention.Instance,
                propertyType = g.ilg.typ_Bool,
                init = None,
                args = [],
                customAttrs = additionalAttributes
            )
            |> ctx.stamping.stampPropertyAsGenerated
            |> ctx.stamping.stampPropertyAsNever
        ]

let private emitNullaryCaseAccessor (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let g = ctx.g
    let td = ctx.td
    let cud = ctx.cud
    let cuspec = ctx.cuspec
    let baseTy = ctx.baseTy
    let altName = alt.Name
    let fields = alt.FieldDefs
    let imports = cud.DebugImports
    let attr = cud.DebugPoint

    let attributes =
        match ctx.layout, num with
        | CaseIsNull when nullnessCheckingEnabled g ->
            let noTypars = td.GenericParams.Length

            GetNullableAttribute
                g
                [
                    yield NullnessInfo.WithNull // The top-level value itself, e.g. option, is nullable
                    yield! List.replicate noTypars NullnessInfo.AmbivalentToNull
                ] // The typars are not (i.e. do not change option<string> into option<string?>
            |> Array.singleton
            |> mkILCustomAttrsFromArray
        | _ -> emptyILCustomAttrs

    let nullaryMeth =
        mkILNonGenericStaticMethod (
            "get_" + altName,
            cud.HelpersAccessibility,
            [],
            (mkILReturn baseTy).WithCustomAttrs attributes,
            mkMethodBody (true, [], fields.Length, nonBranchingInstrsToCode (emitRawNewData g.ilg cuspec num), attr, imports)
        )
        |> (fun mdef -> mdef.With(customAttrs = alt.altCustomAttrs))
        |> ctx.stamping.stampMethodAsGenerated

    let nullaryProp =
        ILPropertyDef(
            name = altName,
            attributes = PropertyAttributes.None,
            setMethod = None,
            getMethod = Some(mkILMethRef (baseTy.TypeRef, ILCallingConv.Static, "get_" + altName, 0, [], baseTy)),
            callingConv = ILThisConvention.Static,
            propertyType = baseTy,
            init = None,
            args = [],
            customAttrs = attributes
        )
        |> ctx.stamping.stampPropertyAsGenerated
        |> ctx.stamping.stampPropertyAsNever

    [ nullaryMeth ], [ nullaryProp ]

let private emitConstantAccessor (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let cud = ctx.cud
    let baseTy = ctx.baseTy
    let altName = alt.Name
    let fields = alt.FieldDefs
    let imports = cud.DebugImports
    let attr = cud.DebugPoint

    // This method is only generated if helpers are not available. It fetches the unique object for the alternative
    // without exposing direct access to the underlying field
    match cud.HasHelpers with
    | AllHelpers
    | SpecialFSharpOptionHelpers
    | SpecialFSharpListHelpers -> []
    | _ ->
        if alt.IsNullary && needsSingletonField ctx.layout alt num then
            let methName = "get_" + altName

            let meth =
                mkILNonGenericStaticMethod (
                    methName,
                    cud.UnionCasesAccessibility,
                    [],
                    mkILReturn baseTy,
                    mkMethodBody (
                        true,
                        [],
                        fields.Length,
                        nonBranchingInstrsToCode [ I_ldsfld(Nonvolatile, mkConstFieldSpec altName baseTy) ],
                        attr,
                        imports
                    )
                )
                |> ctx.stamping.stampMethodAsGenerated

            [ meth ]

        else
            []

let private emitNullaryConstField (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let cud = ctx.cud
    let baseTy = ctx.baseTy
    let cuspec = ctx.cuspec
    let altName = alt.Name
    let altTy = tyForAltIdx cuspec alt num

    if needsSingletonField ctx.layout alt num then
        let basic: ILFieldDef =
            mkILStaticField (constFieldName altName, baseTy, None, None, ILMemberAccess.Assembly)
            |> ctx.stamping.stampFieldAsNever
            |> ctx.stamping.stampFieldAsGenerated

        let uniqObjField = basic.WithInitOnly(true)
        let inRootClass = caseRepresentedOnRoot ctx.layout alt cud.UnionCases num

        [
            {
                Case = alt
                CaseType = altTy
                CaseIndex = num
                Field = uniqObjField
                InRootClass = inRootClass
            }
        ]
    else
        []

let private emitNestedAlternativeType (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let g = ctx.g
    let td = ctx.td
    let cud = ctx.cud
    let cuspec = ctx.cuspec
    let baseTy = ctx.baseTy
    let altTy = tyForAltIdx cuspec alt num
    let fields = alt.FieldDefs
    let imports = cud.DebugImports
    let attr = cud.DebugPoint
    let isTotallyImmutable = (cud.HasHelpers <> SpecialFSharpListHelpers)

    if caseRepresentedOnRoot ctx.layout alt cud.UnionCases num then
        [], []
    else
        let altDebugTypeDefs, debugAttrs =
            if not cud.GenerateDebugProxies then
                [], []
            else
                emitDebugProxyType ctx altTy fields

        let altTypeDef =
            let basicFields =
                fields
                |> Array.map (fun field ->
                    let fldName, fldTy, attrs = mkUnionCaseFieldIdAndAttrs g field
                    let fdef = mkILInstanceField (fldName, fldTy, None, ILMemberAccess.Assembly)

                    let fdef =
                        match attrs with
                        | [] -> fdef
                        | attrs -> fdef.With(customAttrs = mkILCustomAttrs attrs)

                        |> ctx.stamping.stampFieldAsNever
                        |> ctx.stamping.stampFieldAsGenerated

                    fdef.WithInitOnly(isTotallyImmutable))

                |> Array.toList

            let basicProps, basicMethods =
                mkMethodsAndPropertiesForFields
                    (ctx.stamping.stampMethodAsGenerated, ctx.stamping.stampPropertyAsGenerated)
                    g
                    cud.UnionCasesAccessibility
                    attr
                    imports
                    cud.HasHelpers
                    altTy
                    fields

            let basicCtorInstrs =
                [
                    yield mkLdarg0

                    match ctx.layout with
                    | HasTagField ->
                        yield mkLdcInt32 num
                        yield mkNormalCall (mkILCtorMethSpecForTy (baseTy, [ mkTagFieldType g.ilg cuspec ]))
                    | NoTagField -> yield mkNormalCall (mkILCtorMethSpecForTy (baseTy, []))
                ]

            let basicCtorAccess =
                (if cuspec.HasHelpers = AllHelpers then
                     ILMemberAccess.Assembly
                 else
                     cud.UnionCasesAccessibility)

            let basicCtorFields =
                basicFields
                |> List.map (fun fdef ->
                    let nullableAttr = getFieldsNullability g fdef |> Option.toList
                    fdef.Name, fdef.FieldType, nullableAttr)

            let basicCtorMeth =
                (mkILStorageCtor (basicCtorInstrs, altTy, basicCtorFields, basicCtorAccess, attr, imports))
                    .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
                |> ctx.stamping.stampMethodAsGenerated

            let attrs =
                if nullnessCheckingEnabled g then
                    GetNullableContextAttribute g 1uy :: debugAttrs
                else
                    debugAttrs

            let altTypeDef =
                mkILGenericClass (
                    altTy.TypeSpec.Name,
                    // Types for nullary's become private, they also have names like _Empty
                    ILTypeDefAccess.Nested(
                        if alt.IsNullary && cud.HasHelpers = IlxUnionHasHelpers.AllHelpers then
                            ILMemberAccess.Assembly
                        else
                            cud.UnionCasesAccessibility
                    ),
                    td.GenericParams,
                    baseTy,
                    [],
                    mkILMethods ([ basicCtorMeth ] @ basicMethods),
                    mkILFields basicFields,
                    emptyILTypeDefs,
                    mkILProperties basicProps,
                    emptyILEvents,
                    mkILCustomAttrs attrs,
                    ILTypeInit.BeforeField
                )

            altTypeDef.WithSpecialName(true).WithSerializable(td.IsSerializable)

        [ altTypeDef ], altDebugTypeDefs

let private processAlternative (ctx: TypeDefContext) (num: int) (alt: IlxUnionCase) =
    let cud = ctx.cud

    let constantAccessors = emitConstantAccessor ctx num alt

    let baseMakerMeths, baseMakerProps =
        match cud.HasHelpers with
        | AllHelpers
        | SpecialFSharpOptionHelpers
        | SpecialFSharpListHelpers ->
            let testerMeths, testerProps = emitTesterMethodAndProperty ctx num alt

            let makerMeths, makerProps =
                if alt.IsNullary then
                    emitNullaryCaseAccessor ctx num alt
                else
                    [ emitMakerMethod ctx num alt ], []

            (makerMeths @ testerMeths), (makerProps @ testerProps)

        | NoHelpers ->
            match ctx.layout with
            | ValueTypeLayout when not alt.IsNullary -> [ emitMakerMethod ctx num alt ], []
            | _ -> [], []

    let typeDefs, debugTypeDefs, nullaryFields =
        match classifyCaseStorage ctx.layout ctx.cuspec num alt with
        | CaseStorage.Null -> [], [], []
        | CaseStorage.OnRoot -> [], [], []
        | CaseStorage.Singleton
        | CaseStorage.InNestedType _ ->
            let nullaryFields = emitNullaryConstField ctx num alt
            let typeDefs, debugTypeDefs = emitNestedAlternativeType ctx num alt
            typeDefs, debugTypeDefs, nullaryFields

    {
        BaseMakerMethods = baseMakerMeths
        BaseMakerProperties = baseMakerProps
        ConstantAccessors = constantAccessors
        NestedTypeDefs = typeDefs
        DebugProxyTypeDefs = debugTypeDefs
        NullaryConstFields = nullaryFields
    }

/// Rewrite field nullable attributes for struct flattening.
/// When a struct DU has multiple cases, all boxed fields become potentially nullable
/// because only one case's fields are valid at a time.
let private rewriteFieldsForStructFlattening (g: TcGlobals) (alt: IlxUnionCase) (layout: UnionLayout) =
    match layout with
    | UnionLayout.TaggedStruct _
    | UnionLayout.TaggedStructAllNullary _ when nullnessCheckingEnabled g ->
        alt.FieldDefs
        |> Array.map (fun field ->
            if field.Type.IsNominal && field.Type.Boxity = AsValue then
                field
            else
                let attrs =
                    let existingAttrs = field.ILField.CustomAttrs.AsArray()

                    let nullableIdx =
                        existingAttrs |> Array.tryFindIndex (IsILAttrib g.attrib_NullableAttribute)

                    match nullableIdx with
                    | None ->
                        existingAttrs
                        |> Array.append [| GetNullableAttribute g [ NullnessInfo.WithNull ] |]
                    | Some idx ->
                        let replacementAttr =
                            match existingAttrs[idx] with
                            (*
                             The attribute carries either a single byte, or a list of bytes for the fields itself and all its generic type arguments
                             The way we lay out DUs does not affect nullability of the typars of a field, therefore we just change the very first byte
                             If the field was already declared as nullable (value = 2uy) or ambivalent(value = 0uy), we can keep it that way
                             If it was marked as non-nullable within that UnionCase, we have to convert it to WithNull (2uy) due to other cases being possible
                            *)
                            | Encoded(method, _data, [ ILAttribElem.Byte 1uy ]) ->
                                mkILCustomAttribMethRef (method, [ ILAttribElem.Byte 2uy ], [])
                            | Encoded(method, _data, [ ILAttribElem.Array(elemType, ILAttribElem.Byte 1uy :: otherElems) ]) ->
                                mkILCustomAttribMethRef (
                                    method,
                                    [ ILAttribElem.Array(elemType, (ILAttribElem.Byte 2uy) :: otherElems) ],
                                    []
                                )
                            | attrAsBefore -> attrAsBefore

                        existingAttrs |> Array.replace idx replacementAttr

                field.ILField.With(customAttrs = mkILCustomAttrsFromArray attrs)
                |> IlxUnionCaseField)
    | _ -> alt.FieldDefs

/// Add [Nullable(2)] attribute to union root type when null is permitted.
let private rootTypeNullableAttrs (g: TcGlobals) (td: ILTypeDef) (cud: IlxUnionInfo) =
    if cud.IsNullPermitted && nullnessCheckingEnabled g then
        td.CustomAttrs.AsArray()
        |> Array.append [| GetNullableAttribute g [ NullnessInfo.WithNull ] |]
        |> mkILCustomAttrsFromArray
        |> storeILCustomAttrs
    else
        td.CustomAttrsStored

/// Compute fields, methods, and properties that live on the root class.
/// For struct DUs, all fields are flattened onto root. For ref DUs, only
/// cases that fold to root (list Cons, single-non-nullary-with-null-siblings).
let private emitRootClassFields (ctx: TypeDefContext) (tagFieldsInObject: (string * ILType * 'a list) list) =
    let g = ctx.g
    let td = ctx.td
    let cud = ctx.cud
    let baseTy = ctx.baseTy
    let cuspec = ctx.cuspec
    let isStruct = td.IsStruct

    let ctorAccess =
        if cuspec.HasHelpers = AllHelpers then
            ILMemberAccess.Assembly
        else
            cud.UnionCasesAccessibility

    [
        let minNullaryIdx =
            cud.UnionCases
            |> Array.tryFindIndex (fun t -> t.IsNullary)
            |> Option.defaultValue -1

        let fieldsEmitted = HashSet<_>()

        for cidx, alt in Array.indexed cud.UnionCases do
            let fieldsOnRoot =
                match ctx.layout with
                | ValueTypeLayout -> true
                | ReferenceTypeLayout -> caseFieldsOnRoot ctx.layout alt cud.UnionCases

            if fieldsOnRoot then

                let baseInit =
                    if isStruct then
                        None
                    else
                        match td.Extends.Value with
                        | None -> Some g.ilg.typ_Object.TypeSpec
                        | Some ilTy -> Some ilTy.TypeSpec

                let ctor =
                    // Structs with fields are created using static makers methods
                    // Structs without fields can share constructor for the 'tag' value, we just create one
                    if isStruct && not (cidx = minNullaryIdx) then
                        []
                    else
                        let fields =
                            alt.FieldDefs |> Array.map (mkUnionCaseFieldIdAndAttrs g) |> Array.toList

                        [
                            (mkILSimpleStorageCtor (
                                baseInit,
                                baseTy,
                                [],
                                (fields @ tagFieldsInObject),
                                ctorAccess,
                                cud.DebugPoint,
                                cud.DebugImports
                            ))
                                .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x660 baseTy ])
                            |> ctx.stamping.stampMethodAsGenerated
                        ]

                let fieldDefs = rewriteFieldsForStructFlattening g alt ctx.layout

                let fieldsToBeAddedIntoType =
                    fieldDefs
                    |> Array.filter (fun f -> fieldsEmitted.Add(struct (f.LowerName, f.Type)))

                let fields =
                    fieldsToBeAddedIntoType
                    |> Array.map (mkUnionCaseFieldIdAndAttrs g)
                    |> Array.toList

                let props, meths =
                    mkMethodsAndPropertiesForFields
                        (ctx.stamping.stampMethodAsGenerated, ctx.stamping.stampPropertyAsGenerated)
                        g
                        cud.UnionCasesAccessibility
                        cud.DebugPoint
                        cud.DebugImports
                        cud.HasHelpers
                        baseTy
                        fieldsToBeAddedIntoType

                yield (fields, (ctor @ meths), props)
    ]
    |> List.unzip3
    |> (fun (a, b, c) -> List.concat a, List.concat b, List.concat c)

/// Compute the root class default constructor (when needed).
let private emitRootConstructors (ctx: TypeDefContext) rootCaseFields tagFieldsInObject rootCaseMethods =
    let g = ctx.g
    let td = ctx.td
    let cud = ctx.cud
    let baseTy = ctx.baseTy

    // The root-class base ctor (taking only tag fields) is needed when:
    // - There are nested subtypes that call super(tag) — i.e. not all cases fold to root
    // - It's not a struct (structs use static maker methods)
    // - There aren't already instance fields from folded cases covering the ctor need
    let allCasesFoldToRoot =
        cud.UnionCases
        |> Array.forall (fun alt -> caseFieldsOnRoot ctx.layout alt cud.UnionCases)

    let hasFieldsOrTagButNoMethods =
        not (
            List.isEmpty rootCaseFields
            && List.isEmpty tagFieldsInObject
            && not (List.isEmpty rootCaseMethods)
        )

    if td.IsStruct || allCasesFoldToRoot || not hasFieldsOrTagButNoMethods then
        []
    else
        let baseTySpec =
            (match td.Extends.Value with
             | None -> g.ilg.typ_Object
             | Some ilTy -> ilTy)
                .TypeSpec

        [
            (mkILSimpleStorageCtor (
                Some baseTySpec,
                baseTy,
                [],
                tagFieldsInObject,
                ILMemberAccess.Assembly,
                cud.DebugPoint,
                cud.DebugImports
            ))
                .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g 0x7E0 baseTy ])
            |> ctx.stamping.stampMethodAsGenerated
        ]

/// Generate static constructor code to initialize nullary case singleton fields.
let private emitConstFieldInitializers (ctx: TypeDefContext) (altNullaryFields: NullaryConstFieldInfo list) =
    let g = ctx.g
    let cud = ctx.cud
    let baseTy = ctx.baseTy
    let cuspec = ctx.cuspec

    fun (cd: ILTypeDef) ->
        if List.isEmpty altNullaryFields then
            cd
        else
            prependInstrsToClassCtor
                [
                    for r in altNullaryFields do
                        let constFieldId = (r.Field.Name, baseTy)
                        let constFieldSpec = mkConstFieldSpecFromId baseTy constFieldId

                        match ctx.layout with
                        | NoTagField -> yield mkNormalNewobj (mkILCtorMethSpecForTy (r.CaseType, []))
                        | HasTagField ->
                            if r.InRootClass then
                                yield mkLdcInt32 r.CaseIndex
                                yield mkNormalNewobj (mkILCtorMethSpecForTy (r.CaseType, [ mkTagFieldType g.ilg cuspec ]))
                            else
                                yield mkNormalNewobj (mkILCtorMethSpecForTy (r.CaseType, []))

                        yield mkNormalStsfld constFieldSpec
                ]
                cud.DebugPoint
                cud.DebugImports
                cd

/// Create the Tag property, get_Tag method, and Tags enum-like constants.
let private emitTagInfrastructure (ctx: TypeDefContext) =
    let g = ctx.g
    let cud = ctx.cud
    let baseTy = ctx.baseTy
    let cuspec = ctx.cuspec

    let tagFieldType = mkTagFieldType g.ilg cuspec

    let tagEnumFields =
        cud.UnionCases
        |> Array.mapi (fun num alt -> mkILLiteralField (alt.Name, tagFieldType, ILFieldInit.Int32 num, None, ILMemberAccess.Public))
        |> Array.toList

    let tagMeths, tagProps =

        let code =
            genWith (fun cg ->
                emitLdDataTagPrim g.ilg (Some mkLdarg0) cg (DataAccess.RawFields, cuspec)
                cg.EmitInstr I_ret)

        let body = mkMethodBody (true, [], 2, code, cud.DebugPoint, cud.DebugImports)

        // If we are using NULL as a representation for an element of this type then we cannot
        // use an instance method
        match ctx.layout with
        | UnionLayout.SmallRefWithNullAsTrueValue _ ->
            [
                mkILNonGenericStaticMethod (
                    "Get" + tagPropertyName,
                    cud.HelpersAccessibility,
                    [ mkILParamAnon baseTy ],
                    mkILReturn tagFieldType,
                    body
                )
                |> ctx.stamping.stampMethodAsGenerated
            ],
            []

        | _ ->
            [
                mkILNonGenericInstanceMethod ("get_" + tagPropertyName, cud.HelpersAccessibility, [], mkILReturn tagFieldType, body)
                |> ctx.stamping.stampMethodAsGenerated
            ],

            [
                ILPropertyDef(
                    name = tagPropertyName,
                    attributes = PropertyAttributes.None,
                    setMethod = None,
                    getMethod = Some(mkILMethRef (baseTy.TypeRef, ILCallingConv.Instance, "get_" + tagPropertyName, 0, [], tagFieldType)),
                    callingConv = ILThisConvention.Instance,
                    propertyType = tagFieldType,
                    init = None,
                    args = [],
                    customAttrs = emptyILCustomAttrs
                )
                |> ctx.stamping.stampPropertyAsGenerated
                |> ctx.stamping.stampPropertyAsNever
            ]

    tagMeths, tagProps, tagEnumFields

/// Compute instance fields from rootCaseFields and tagFieldsInObject.
let private computeRootInstanceFields (ctx: TypeDefContext) rootCaseFields (tagFieldsInObject: (string * ILType * ILAttribute list) list) =
    let isStruct = ctx.td.IsStruct
    let isTotallyImmutable = (ctx.cud.HasHelpers <> SpecialFSharpListHelpers)

    [
        for fldName, fldTy, attrs in (rootCaseFields @ tagFieldsInObject) do
            let fdef =
                let fdef = mkILInstanceField (fldName, fldTy, None, ILMemberAccess.Assembly)

                match attrs with
                | [] -> fdef
                | attrs -> fdef.With(customAttrs = mkILCustomAttrs attrs)

                |> ctx.stamping.stampFieldAsNever
                |> ctx.stamping.stampFieldAsGenerated

            yield fdef.WithInitOnly(not isStruct && isTotallyImmutable)
    ]

/// Compute the nested Tags type definition (elided when ≤1 case).
let private computeEnumTypeDef (g: TcGlobals) (td: ILTypeDef) (cud: IlxUnionInfo) tagEnumFields =
    if List.length tagEnumFields <= 1 then
        None
    else
        let tdef =
            ILTypeDef(
                name = "Tags",
                nestedTypes = emptyILTypeDefs,
                genericParams = td.GenericParams,
                attributes = enum 0,
                layout = ILTypeDefLayout.Auto,
                implements = [],
                extends = Some g.ilg.typ_Object,
                methods = emptyILMethods,
                securityDecls = emptyILSecurityDecls,
                fields = mkILFields tagEnumFields,
                methodImpls = emptyILMethodImpls,
                events = emptyILEvents,
                properties = emptyILProperties,
                customAttrs = emptyILCustomAttrsStored
            )
                .WithNestedAccess(cud.UnionCasesAccessibility)
                .WithAbstract(true)
                .WithSealed(true)
                .WithImport(false)
                .WithEncoding(ILDefaultPInvokeEncoding.Ansi)
                .WithHasSecurity(false)

        Some tdef

/// Assemble all pieces into the final union ILTypeDef.
let private assembleUnionTypeDef
    (ctx: TypeDefContext)
    (results: AlternativeDefResult list)
    ctorMeths
    rootCaseMethods
    rootAndTagFields
    tagMeths
    tagProps
    tagEnumFields
    rootCaseProperties
    =
    let g = ctx.g
    let td = ctx.td
    let cud = ctx.cud

    let altNullaryFields = results |> List.collect (fun r -> r.NullaryConstFields)
    let baseMethsFromAlt = results |> List.collect (fun r -> r.BaseMakerMethods)
    let basePropsFromAlt = results |> List.collect (fun r -> r.BaseMakerProperties)
    let altUniqObjMeths = results |> List.collect (fun r -> r.ConstantAccessors)
    let altTypeDefs = results |> List.collect (fun r -> r.NestedTypeDefs)
    let altDebugTypeDefs = results |> List.collect (fun r -> r.DebugProxyTypeDefs)
    let enumTypeDef = computeEnumTypeDef g td cud tagEnumFields
    let addConstFieldInit = emitConstFieldInitializers ctx altNullaryFields

    let existingMeths = td.Methods.AsList()
    let existingProps = td.Properties.AsList()
    let isAbstract = (altTypeDefs.Length = cud.UnionCases.Length)

    let baseTypeDef: ILTypeDef =
        td
            .WithInitSemantics(ILTypeInit.BeforeField)
            .With(
                nestedTypes =
                    mkILTypeDefs (
                        Option.toList enumTypeDef
                        @ altTypeDefs
                        @ altDebugTypeDefs
                        @ td.NestedTypes.AsList()
                    ),
                extends =
                    (match td.Extends.Value with
                     | None -> Some g.ilg.typ_Object |> notlazy
                     | _ -> td.Extends),
                methods =
                    mkILMethods (
                        ctorMeths
                        @ baseMethsFromAlt
                        @ rootCaseMethods
                        @ tagMeths
                        @ altUniqObjMeths
                        @ existingMeths
                    ),
                fields =
                    mkILFields (
                        rootAndTagFields
                        @ List.map (fun r -> r.Field) altNullaryFields
                        @ td.Fields.AsList()
                    ),
                properties = mkILProperties (tagProps @ basePropsFromAlt @ rootCaseProperties @ existingProps),
                customAttrs = rootTypeNullableAttrs g td cud
            )
        |> addConstFieldInit

    baseTypeDef.WithAbstract(isAbstract).WithSealed(altTypeDefs.IsEmpty)

let mkClassUnionDef
    (
        addMethodGeneratedAttrs,
        addPropertyGeneratedAttrs,
        addPropertyNeverAttrs,
        addFieldGeneratedAttrs: ILFieldDef -> ILFieldDef,
        addFieldNeverAttrs: ILFieldDef -> ILFieldDef,
        mkDebuggerTypeProxyAttribute
    )
    (g: TcGlobals)
    tref
    (td: ILTypeDef)
    cud
    =
    let boxity = if td.IsStruct then ILBoxity.AsValue else ILBoxity.AsObject
    let baseTy = mkILFormalNamedTy boxity tref td.GenericParams

    let cuspec =
        IlxUnionSpec(IlxUnionRef(boxity, baseTy.TypeRef, cud.UnionCases, cud.IsNullPermitted, cud.HasHelpers), baseTy.GenericArgs)

    let ctx =
        {
            g = g
            layout = classifyFromDef td cud baseTy
            cuspec = cuspec
            cud = cud
            td = td
            baseTy = baseTy
            stamping =
                {
                    stampMethodAsGenerated = addMethodGeneratedAttrs
                    stampPropertyAsGenerated = addPropertyGeneratedAttrs
                    stampPropertyAsNever = addPropertyNeverAttrs
                    stampFieldAsGenerated = addFieldGeneratedAttrs
                    stampFieldAsNever = addFieldNeverAttrs
                    mkDebuggerTypeProxyAttr = mkDebuggerTypeProxyAttribute
                }
        }

    let results =
        cud.UnionCases
        |> List.ofArray
        |> List.mapi (fun i alt -> processAlternative ctx i alt)

    let tagFieldsInObject =
        match ctx.layout with
        | HasTagField -> [ let n, t = mkTagFieldId g.ilg cuspec in n, t, [] ]
        | NoTagField -> []

    let rootCaseFields, rootCaseMethods, rootCaseProperties =
        emitRootClassFields ctx tagFieldsInObject

    let rootAndTagFields =
        computeRootInstanceFields ctx rootCaseFields tagFieldsInObject

    let ctorMeths =
        emitRootConstructors ctx rootCaseFields tagFieldsInObject rootCaseMethods

    let tagMeths, tagProps, tagEnumFields = emitTagInfrastructure ctx

    assembleUnionTypeDef ctx results ctorMeths rootCaseMethods rootAndTagFields tagMeths tagProps tagEnumFields rootCaseProperties
