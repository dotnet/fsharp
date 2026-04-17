// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Erase discriminated unions - type definition generation.
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

// ============================================================================
// Type Definition Generation for F# Discriminated Unions
//
// Entry point: mkClassUnionDef (bottom of file, F# requires definitions before use)
//
// Pipeline:
//   1. Classify union layout (Types.fs: classifyFromDef → UnionLayout)
//   2. For each case: classify storage (Types.fs: classifyCaseStorage → CaseStorage)
//   3. For each case: emit maker methods, tester properties, nested types, debug proxies
//   4. Emit root class: fields, constructors, tag infrastructure
//   5. Assemble everything into the final ILTypeDef
//
// Key context: TypeDefContext bundles all generation parameters.
// Results per case: AlternativeDefResult collects methods/fields/types.
//
// Example mappings (DU → UnionLayout → CaseStorage):
//   type Option<'T> = None | Some of 'T
//     → SmallRefWithNullAsTrueValue
//     → None=Null, Some=OnRoot
//
//   type Color = Red | Green | Blue | Yellow
//     → TaggedRefAllNullary
//     → all cases=Singleton
//
//   [<Struct>] type Result<'T,'E> = Ok of 'T | Error of 'E
//     → TaggedStruct
//     → Ok=OnRoot, Error=OnRoot
//
//   type Shape = Circle of float | Square of float | Point
//     → SmallRef (3 cases, ref, not all-nullary)
//     → Circle=InNestedType, Square=InNestedType, Point=Singleton
//
//   type Token = Ident of string | IntLit of int | Plus | Minus | Star
//     → TaggedRef (≥4 cases, ref)
//     → Ident=InNestedType, IntLit=InNestedType, Plus/Minus/Star=Singleton
// ============================================================================

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

/// DynamicallyAccessedMemberTypes flags for [DynamicDependency] on case ctors
[<Literal>]
let private DynamicDependencyPublicMembers = 0x660

/// DynamicallyAccessedMemberTypes flags for [DynamicDependency] on base ctor
[<Literal>]
let private DynamicDependencyAllCtorsAndPublicMembers = 0x7E0

//---------------------------------------------------
// Generate the union classes

let private mkMethodsAndPropertiesForFields (ctx: TypeDefContext) (ilTy: ILType) (fields: IlxUnionCaseField[]) =
    let g = ctx.g
    let cud = ctx.cud
    let access = cud.UnionCasesAccessibility
    let attr = cud.DebugPoint
    let imports = cud.DebugImports
    let hasHelpers = cud.HasHelpers
    let addMethodGeneratedAttrs = ctx.stamping.stampMethodAsGenerated
    let addPropertyGeneratedAttrs = ctx.stamping.stampPropertyAsGenerated

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
            .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g DynamicDependencyPublicMembers baseTy ])
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
    let altTy = tyForAltIdxWith ctx.layout ctx.baseTy cuspec alt num

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
    let altTy = tyForAltIdxWith ctx.layout ctx.baseTy cuspec alt num
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

            let basicProps, basicMethods = mkMethodsAndPropertiesForFields ctx altTy fields

            let basicCtorInstrs =
                [
                    yield mkLdarg0

                    match ctx.layout with
                    | HasTagField ->
                        yield mkLdcInt32 num
                        yield mkNormalCall (mkILCtorMethSpecForTy (baseTy, [ mkTagFieldType g.ilg ]))
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
                    .With(customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g DynamicDependencyPublicMembers baseTy ])
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

// ---- Nullable Attribute Rewriting ----
// When struct DUs have multiple cases, all boxed fields become potentially nullable
// because only one case's fields are valid at a time. These helpers rewrite [Nullable]
// attributes accordingly. rootTypeNullableAttrs handles the union type itself.

/// Rewrite field nullable attributes for struct flattening.
/// When a struct DU has multiple cases, all boxed fields become potentially nullable
/// because only one case's fields are valid at a time. This rewrites the [Nullable] attribute
/// on a field to WithNull (2uy) if it was marked as non-nullable (1uy) within its case.
let private rewriteNullableAttrForFlattenedField (g: TcGlobals) (existingAttrs: ILAttribute[]) =
    let nullableIdx =
        existingAttrs |> Array.tryFindIndex (IsILAttrib g.attrib_NullableAttribute)

    match nullableIdx with
    | None ->
        existingAttrs
        |> Array.append [| GetNullableAttribute g [ NullnessInfo.WithNull ] |]
    | Some idx ->
        let replacementAttr =
            match existingAttrs[idx] with
            // Single byte: change non-nullable (1) to WithNull (2); leave nullable (2) and ambivalent (0) as-is
            | Encoded(method, _data, [ ILAttribElem.Byte 1uy ]) -> mkILCustomAttribMethRef (method, [ ILAttribElem.Byte 2uy ], [])
            // Array of bytes: change first element only (field itself); leave generic type arg nullability unchanged
            | Encoded(method, _data, [ ILAttribElem.Array(elemType, ILAttribElem.Byte 1uy :: otherElems) ]) ->
                mkILCustomAttribMethRef (method, [ ILAttribElem.Array(elemType, (ILAttribElem.Byte 2uy) :: otherElems) ], [])
            | attrAsBefore -> attrAsBefore

        existingAttrs |> Array.replace idx replacementAttr

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
                    rewriteNullableAttrForFlattenedField g (field.ILField.CustomAttrs.AsArray())

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
                    // Structs use static maker methods for non-nullary cases.
                    // For nullary struct cases, we emit a single shared ctor (for the min-index nullary)
                    // that takes only the tag value — all other nullary cases reuse it via the maker.
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
                                .With(
                                    customAttrs = mkILCustomAttrs [ GetDynamicDependencyAttribute g DynamicDependencyPublicMembers baseTy ]
                                )
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
                    mkMethodsAndPropertiesForFields ctx baseTy fieldsToBeAddedIntoType

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

    let onlyMethodsOnRoot =
        List.isEmpty rootCaseFields
        && List.isEmpty tagFieldsInObject
        && not (List.isEmpty rootCaseMethods)

    if td.IsStruct || allCasesFoldToRoot || onlyMethodsOnRoot then
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
                .With(
                    customAttrs =
                        mkILCustomAttrs
                            [
                                GetDynamicDependencyAttribute g DynamicDependencyAllCtorsAndPublicMembers baseTy
                            ]
                )
            |> ctx.stamping.stampMethodAsGenerated
        ]

/// Generate static constructor code to initialize nullary case singleton fields.
let private emitConstFieldInitializers (ctx: TypeDefContext) (altNullaryFields: NullaryConstFieldInfo list) =
    let g = ctx.g
    let cud = ctx.cud
    let baseTy = ctx.baseTy

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
                                yield mkNormalNewobj (mkILCtorMethSpecForTy (r.CaseType, [ mkTagFieldType g.ilg ]))
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

    let tagFieldType = mkTagFieldType g.ilg

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
    // The root type is abstract when every case has its own nested subtype.
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
        |> Array.mapi (fun i alt -> processAlternative ctx i alt)
        |> Array.toList

    let tagFieldsInObject =
        match ctx.layout with
        | HasTagField -> [ let n, t = mkTagFieldId g.ilg in n, t, [] ]
        | NoTagField -> []

    let rootCaseFields, rootCaseMethods, rootCaseProperties =
        emitRootClassFields ctx tagFieldsInObject

    let rootAndTagFields =
        computeRootInstanceFields ctx rootCaseFields tagFieldsInObject

    let ctorMeths =
        emitRootConstructors ctx rootCaseFields tagFieldsInObject rootCaseMethods

    let tagMeths, tagProps, tagEnumFields = emitTagInfrastructure ctx

    assembleUnionTypeDef ctx results ctorMeths rootCaseMethods rootAndTagFields tagMeths tagProps tagEnumFields rootCaseProperties
