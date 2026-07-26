// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Erase discriminated unions - types, classification, and active patterns.
[<AutoOpen>]
module internal FSharp.Compiler.AbstractIL.ILX.EraseUnionsTypes

open FSharp.Compiler.IlxGenSupport

open System.Collections.Generic
open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types

// ============================================================================
// Architecture: Two-axis classification model
//
// Every decision in this module is driven by two independent classifications:
//
// 1. UnionLayout (9 cases) — how the union TYPE is structured in IL
//    Computed once per union via classifyFromSpec / classifyFromDef.
//
// 2. CaseStorage (4 cases) — how each individual CASE is stored
//    Computed per case via classifyCaseStorage. Answers: is this case null?
//    A singleton field? Fields on root? In a nested subtype? Struct tag-only?
//
// Orthogonal concerns read from these:
//   - DataAccess (3 cases) — how callers access data (raw fields vs helpers)
//   - DiscriminationMethod (AP) — how to distinguish cases (tag/isinst/tail-null)
//
// The emit functions match on CaseStorage first (WHERE is it?), then on
// DiscriminationMethod (HOW to tell it apart?). This two-axis pattern
// ensures each function reads as a simple decision table, not a re-derivation.
// ============================================================================

/// How to access union data at a given call site.
/// Combines the per-call-site 'avoidHelpers' flag with the per-union 'HasHelpers' setting
/// into a single value computed once at the entry point.
[<RequireQualifiedAccess>]
type DataAccess =
    /// Use raw field loads/stores (intra-assembly access, or union has no helpers)
    | RawFields
    /// Use helper methods (get_Tag, get_IsXxx, NewXxx) — inter-assembly with AllHelpers
    | ViaHelpers
    /// Use list-specific helper methods (HeadOrDefault, TailOrNull naming) — inter-assembly with SpecialFSharpListHelpers
    | ViaListHelpers
    /// Use helper methods for field access, but raw discrimination for tag access — SpecialFSharpOptionHelpers
    | ViaOptionHelpers

/// Compute the access strategy from the per-call-site flag and per-union helpers setting.
let computeDataAccess (avoidHelpers: bool) (cuspec: IlxUnionSpec) =
    if avoidHelpers then
        DataAccess.RawFields
    else
        match cuspec.HasHelpers with
        | IlxUnionHasHelpers.NoHelpers -> DataAccess.RawFields
        | IlxUnionHasHelpers.AllHelpers -> DataAccess.ViaHelpers
        | IlxUnionHasHelpers.SpecialFSharpOptionHelpers -> DataAccess.ViaOptionHelpers
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
    | UnionLayout.TaggedRef baseTy
    | UnionLayout.TaggedRefAllNullary baseTy
    | UnionLayout.TaggedStruct baseTy
    | UnionLayout.TaggedStructAllNullary baseTy -> DiscriminateByTagField baseTy
    | UnionLayout.SmallRef baseTy -> DiscriminateByRuntimeType(baseTy, None)
    | UnionLayout.SmallRefWithNullAsTrueValue(baseTy, nullIdx) -> DiscriminateByRuntimeType(baseTy, Some nullIdx)
    | UnionLayout.FSharpList baseTy -> DiscriminateByTailNull baseTy
    | UnionLayout.SingleCaseRef baseTy -> NoDiscrimination baseTy
    | UnionLayout.SingleCaseStruct baseTy -> NoDiscrimination baseTy

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

/// Is a specific case (by index) represented as null?
let inline (|CaseIsNull|CaseIsAllocated|) (layout, cidx) =
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

// ---- Layout-Based Helpers ----
// These replace the old representation decision methods.

/// Does this non-nullary alternative fold to root class via fresh instances?
/// Equivalent to the old RepresentAlternativeAsFreshInstancesOfRootClass.
let caseFieldsOnRoot (layout: UnionLayout) (alt: IlxUnionCase) (alts: IlxUnionCase[]) =
    not alt.IsNullary
    && (match layout with
        | UnionLayout.FSharpList _ -> alt.Name = ALT_NAME_CONS
        | UnionLayout.SingleCaseRef _ -> true
        | UnionLayout.SmallRefWithNullAsTrueValue _ -> alts |> Array.existsOne (fun a -> not a.IsNullary)
        | UnionLayout.SmallRef _
        | UnionLayout.SingleCaseStruct _
        | UnionLayout.TaggedRef _
        | UnionLayout.TaggedRefAllNullary _
        | UnionLayout.TaggedStruct _
        | UnionLayout.TaggedStructAllNullary _ -> false)

/// Does this alternative optimize to root class (no nested type needed)?
/// Equivalent to the old OptimizeAlternativeToRootClass.
let caseRepresentedOnRoot (layout: UnionLayout) (alt: IlxUnionCase) (alts: IlxUnionCase[]) (cidx: int) =
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
let needsSingletonField (layout: UnionLayout) (alt: IlxUnionCase) (cidx: int) =
    alt.IsNullary
    && match layout, cidx with
       | CaseIsNull -> false
       | _ ->
           match layout with
           | ReferenceTypeLayout -> true
           | ValueTypeLayout -> false

let tyForAltIdxWith (layout: UnionLayout) (baseTy: ILType) (cuspec: IlxUnionSpec) (alt: IlxUnionCase) cidx =
    if caseRepresentedOnRoot layout alt cuspec.AlternativesArray cidx then
        baseTy
    else
        let isList = (cuspec.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers)
        let altName = alt.Name
        let nm = if alt.IsNullary || isList then "_" + altName else altName
        mkILNamedTy cuspec.Boxity (mkILTyRefInTyRef (mkCasesTypeRef cuspec, nm)) cuspec.GenericArgs

let tyForAltIdx cuspec (alt: IlxUnionCase) cidx =
    tyForAltIdxWith (classifyFromSpec cuspec) (baseTyOfUnionSpec cuspec) cuspec alt cidx

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
            CaseStorage.InNestedType(tyForAltIdxWith layout (baseTyOfUnionSpec cuspec) cuspec alt cidx)

let mkTesterName nm = "Is" + nm

let tagPropertyName = "Tag"

/// Adjust field names for F# list type (Head→HeadOrDefault, Tail→TailOrNull).
let adjustFieldNameForList nm =
    match nm with
    | "Head" -> "HeadOrDefault"
    | "Tail" -> "TailOrNull"
    | _ -> nm

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
        |> Array.tryFindIndex (fun (a: IlxUnionCase) -> a.Name = alt.Name)
        |> Option.defaultWith (fun () -> failwith $"tyForAlt: case '{alt.Name}' not in union spec")

    tyForAltIdx cuspec alt cidx

let GetILTypeForAlternative cuspec alt =
    tyForAlt cuspec (cuspec.Alternative alt)

let mkTagFieldType (ilg: ILGlobals) = ilg.typ_Int32

let mkTagFieldId ilg = "_tag", mkTagFieldType ilg

let altOfUnionSpec (cuspec: IlxUnionSpec) cidx =
    let alts = cuspec.AlternativesArray

    if cidx < 0 || cidx >= alts.Length then
        failwith $"alternative {cidx} not found (union has {alts.Length} cases)"
    else
        alts[cidx]

/// Resolved identity of a union case within a union spec.
[<Struct>]
type CaseIdentity =
    {
        Index: int
        Case: IlxUnionCase
        CaseType: ILType
        CaseName: string
    }

/// Resolve a case by index using precomputed layout and base type.
let resolveCaseWith (layout: UnionLayout) (baseTy: ILType) (cuspec: IlxUnionSpec) (cidx: int) =
    let alt = altOfUnionSpec cuspec cidx

    {
        Index = cidx
        Case = alt
        CaseType = tyForAltIdxWith layout baseTy cuspec alt cidx
        CaseName = alt.Name
    }
