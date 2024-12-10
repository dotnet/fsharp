// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The ILX generator.
module internal FSharp.Compiler.IlxGenSupport

open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTree

/// Make a method that simply loads a field
let mkLdfldMethodDef (ilMethName, iLAccess, isStatic, ilTy, ilFieldName, ilPropType, retTyAttrs, customAttrs) =
    let ilFieldSpec = mkILFieldSpecInTy (ilTy, ilFieldName, ilPropType)

    let ilReturn =
        { mkILReturn ilPropType with
            CustomAttrsStored = storeILCustomAttrs retTyAttrs
        }

    let ilMethodDef =
        if isStatic then
            let body =
                mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkNormalLdsfld ilFieldSpec ], None, None)

            mkILNonGenericStaticMethod (ilMethName, iLAccess, [], ilReturn, body)
        else
            let body =
                mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkLdarg0; mkNormalLdfld ilFieldSpec ], None, None)

            mkILNonGenericInstanceMethod (ilMethName, iLAccess, [], ilReturn, body)

    ilMethodDef.With(customAttrs = mkILCustomAttrs customAttrs).WithSpecialName

let mkFlagsAttribute (g: TcGlobals) =
    mkILCustomAttribute (g.attrib_FlagsAttribute.TypeRef, [], [], [])

let mkLocalPrivateAttributeWithDefaultConstructor (g: TcGlobals, name: string) =
    let ilMethods =
        mkILMethods
            [
                g.AddMethodGeneratedAttributes(mkILNonGenericEmptyCtor (g.ilg.typ_Attribute, None, None))
            ]

    let ilCustomAttrs = mkILCustomAttrsFromArray [| g.CompilerGeneratedAttribute |]

    mkILGenericClass (
        name,
        ILTypeDefAccess.Private,
        ILGenericParameterDefs.Empty,
        g.ilg.typ_Attribute,
        [],
        ilMethods,
        emptyILFields,
        emptyILTypeDefs,
        emptyILProperties,
        emptyILEvents,
        ilCustomAttrs,
        ILTypeInit.BeforeField
    )

let mkILNonGenericInstanceProperty (name, ilType, propertyAttribute, customAttributes, getMethod, setMethod) =
    ILPropertyDef(
        name = name,
        attributes = propertyAttribute,
        setMethod = setMethod,
        getMethod = getMethod,
        callingConv = ILThisConvention.Instance,
        propertyType = ilType,
        init = None,
        args = [],
        customAttrs = customAttributes
    )

type AttrDataGenerationStyle =
    | PublicFields
    | EncapsulatedProperties

let getFieldMemberAccess =
    function
    | PublicFields -> ILMemberAccess.Public
    | EncapsulatedProperties -> ILMemberAccess.Private

let mkLocalPrivateAttributeWithPropertyConstructors
    (
        g: TcGlobals,
        name: string,
        attrProperties: (string * ILType) list option,
        codegenStyle: AttrDataGenerationStyle
    ) =
    let ilTypeRef = mkILTyRef (ILScopeRef.Local, name)
    let ilTy = mkILFormalNamedTy ILBoxity.AsObject ilTypeRef []

    let ilElements =
        attrProperties
        |> Option.defaultValue []
        |> List.map (fun (name, ilType) ->
            match codegenStyle with
            | PublicFields ->
                (g.AddFieldGeneratedAttributes(mkILInstanceField (name, ilType, None, getFieldMemberAccess codegenStyle))),
                [],
                [],
                (name, name, ilType, [])
            | EncapsulatedProperties ->
                let fieldName = name + "@"

                (g.AddFieldGeneratedAttributes(mkILInstanceField (fieldName, ilType, None, getFieldMemberAccess codegenStyle))),
                [
                    g.AddMethodGeneratedAttributes(
                        mkLdfldMethodDef ($"get_{name}", ILMemberAccess.Public, false, ilTy, fieldName, ilType, ILAttributes.Empty, [])
                    )
                ],
                [
                    g.AddPropertyGeneratedAttributes(
                        mkILNonGenericInstanceProperty (
                            name,
                            ilType,
                            PropertyAttributes.None,
                            emptyILCustomAttrs,
                            Some(mkILMethRef (ilTypeRef, ILCallingConv.Instance, "get_" + name, 0, [], ilType)),
                            None
                        )
                    )
                ],
                (name, fieldName, ilType, []))

    // Generate constructor with required arguments
    let ilCtorDef =
        g.AddMethodGeneratedAttributes(
            mkILSimpleStorageCtorWithParamNames (
                Some g.ilg.typ_Attribute.TypeSpec,
                ilTy,
                [],
                (ilElements |> List.map (fun (_, _, _, fieldInfo) -> fieldInfo)),
                ILMemberAccess.Public,
                None,
                None
            )
        )

    mkILGenericClass (
        name,
        ILTypeDefAccess.Private,
        ILGenericParameterDefs.Empty,
        g.ilg.typ_Attribute,
        [],
        mkILMethods (
            ilCtorDef
            :: (ilElements |> List.fold (fun acc (_, getter, _, _) -> getter @ acc) [])
        ),
        mkILFields (ilElements |> List.map (fun (field, _, _, _) -> field)),
        emptyILTypeDefs,
        mkILProperties (ilElements |> List.collect (fun (_, _, props, _) -> props)),
        emptyILEvents,
        mkILCustomAttrsFromArray [| g.CompilerGeneratedAttribute |],
        ILTypeInit.BeforeField
    )

let mkLocalPrivateAttributeWithByteAndByteArrayConstructors (g: TcGlobals, name: string, bytePropertyName: string) =
    let ilTypeRef = mkILTyRef (ILScopeRef.Local, name)
    let ilTy = mkILFormalNamedTy ILBoxity.AsObject ilTypeRef []

    let fieldName = bytePropertyName
    let fieldType = g.ilg.typ_ByteArray

    let fieldDef =
        g.AddFieldGeneratedAttributes(mkILInstanceField (fieldName, fieldType, None, ILMemberAccess.Public))

    // Constructor taking an array
    let ilArrayCtorDef =
        g.AddMethodGeneratedAttributes(
            mkILSimpleStorageCtorWithParamNames (
                Some g.ilg.typ_Attribute.TypeSpec,
                ilTy,
                [],
                [ (fieldName, fieldName, fieldType, []) ],
                ILMemberAccess.Public,
                None,
                None
            )
        )

    let ilScalarCtorDef =
        let scalarValueIlType = g.ilg.typ_Byte

        g.AddMethodGeneratedAttributes(
            let code =
                [
                    mkLdarg0
                    mkNormalCall (mkILCtorMethSpecForTy (mkILBoxedType g.ilg.typ_Attribute.TypeSpec, [])) // Base class .ctor

                    mkLdarg0 // Prepare 'this' to be on bottom of the stack
                    mkLdcInt32 1
                    I_newarr(ILArrayShape.SingleDimensional, scalarValueIlType) // new byte[1]
                    AI_dup // Duplicate the array pointer in stack, 1 for stelem and 1 for stfld
                    mkLdcInt32 0
                    mkLdarg 1us
                    I_stelem DT_I1 // array[0] = argument from .ctor
                    mkNormalStfld (mkILFieldSpecInTy (ilTy, fieldName, fieldType))
                ]

            let body = mkMethodBody (false, [], 8, nonBranchingInstrsToCode code, None, None)
            mkILCtor (ILMemberAccess.Public, [ mkILParamNamed ("scalarByteValue", scalarValueIlType) ], body)
        )

    mkILGenericClass (
        name,
        ILTypeDefAccess.Private,
        ILGenericParameterDefs.Empty,
        g.ilg.typ_Attribute,
        [],
        mkILMethods ([ ilScalarCtorDef; ilArrayCtorDef ]),
        mkILFields [ fieldDef ],
        emptyILTypeDefs,
        emptyILProperties,
        emptyILEvents,
        mkILCustomAttrsFromArray [| g.CompilerGeneratedAttribute |],
        ILTypeInit.BeforeField
    )

let mkLocalPrivateInt32Enum (g: TcGlobals, tref: ILTypeRef, values: (string * int32) array) =
    let ilType = ILType.Value(mkILNonGenericTySpec (tref))

    let enumFields =
        values
        |> Array.map (fun (name, value) -> mkILStaticLiteralField (name, ilType, ILFieldInit.Int32 value, None, ILMemberAccess.Public))
        |> Array.append
            [|
                (mkILInstanceField ("value__", g.ilg.typ_Int32, None, ILMemberAccess.Public))
                    .WithSpecialName(true)
            |]
        |> Array.toList

    mkILGenericClass(
        tref.Name,
        ILTypeDefAccess.Private,
        ILGenericParameterDefs.Empty,
        g.ilg.typ_Enum,
        [],
        mkILMethods [],
        mkILFields enumFields,
        emptyILTypeDefs,
        emptyILProperties,
        emptyILEvents,
        g.AddGeneratedAttributes(mkILCustomAttrs [ mkFlagsAttribute g ]),
        ILTypeInit.OnAny
    )
        .WithSealed(true)

//--------------------------------------------------------------------------
// Generate Local embeddable versions of framework types when necessary
//--------------------------------------------------------------------------

let private getPotentiallyEmbeddableAttribute (g: TcGlobals) (info: BuiltinAttribInfo) =
    let tref = info.TypeRef
    g.TryEmbedILType(tref, (fun () -> mkLocalPrivateAttributeWithDefaultConstructor (g, tref.Name)))
    mkILCustomAttribute (info.TypeRef, [], [], [])

let GetReadOnlyAttribute (g: TcGlobals) =
    getPotentiallyEmbeddableAttribute g g.attrib_IsReadOnlyAttribute

let GetIsUnmanagedAttribute (g: TcGlobals) =
    getPotentiallyEmbeddableAttribute g g.attrib_IsUnmanagedAttribute

let GetDynamicallyAccessedMemberTypes (g: TcGlobals) =
    let tref = g.enum_DynamicallyAccessedMemberTypes.TypeRef

    if not (g.compilingFSharpCore) then
        g.TryEmbedILType(
            tref,
            (fun () ->
                let values =
                    [|
                        ("All", -1)
                        ("None", 0)
                        ("PublicParameterlessConstructor", 1)
                        ("PublicConstructors", 3)
                        ("NonPublicConstructors", 4)
                        ("PublicMethods", 8)
                        ("NonPublicMethods", 16)
                        ("PublicFields", 32)
                        ("NonPublicFields", 64)
                        ("PublicNestedTypes", 128)
                        ("NonPublicNestedTypes", 256)
                        ("PublicProperties", 512)
                        ("NonPublicProperties", 1024)
                        ("PublicEvents", 2048)
                        ("NonPublicEvents", 4096)
                        ("Interfaces", 8192)
                    |]

                (mkLocalPrivateInt32Enum (g, tref, values))
                    .WithSerializable(true)
                    .WithSealed(true))
        )

    ILType.Value(mkILNonGenericTySpec (tref))

let GetDynamicDependencyAttribute (g: TcGlobals) memberTypes (ilType: ILType) =
    let tref = g.attrib_DynamicDependencyAttribute.TypeRef

    g.TryEmbedILType(
        tref,
        (fun () ->
            let properties =
                Some [ "MemberType", GetDynamicallyAccessedMemberTypes g; "Type", g.ilg.typ_Type ]

            mkLocalPrivateAttributeWithPropertyConstructors (g, tref.Name, properties, EncapsulatedProperties))
    )

    let typIlMemberTypes =
        ILType.Value(mkILNonGenericTySpec (g.enum_DynamicallyAccessedMemberTypes.TypeRef))

    mkILCustomAttribute (
        tref,
        [ typIlMemberTypes; g.ilg.typ_Type ],
        [ ILAttribElem.Int32 memberTypes; ILAttribElem.TypeRef(Some ilType.TypeRef) ],
        []
    )

/// Generates NullableContextAttribute[1], which has the meaning of:
/// Nested items not being annotated with Nullable attribute themselves are interpreted as being withoutnull
/// Doing it that way is a heuristical decision supporting limited usage of (| null) annotations and not allowing nulls in >50% of F# code
/// (if majority of fields/parameters/return values would be nullable, this heuristic would lead to bloat of generated metadata)
let GetNullableContextAttribute (g: TcGlobals) flagValue =
    let tref = g.attrib_NullableContextAttribute.TypeRef

    g.TryEmbedILType(
        tref,
        (fun () ->
            let fields = Some [ "Flag", g.ilg.typ_Byte ]
            mkLocalPrivateAttributeWithPropertyConstructors (g, tref.Name, fields, PublicFields))
    )

    mkILCustomAttribute (tref, [ g.ilg.typ_Byte ], [ ILAttribElem.Byte flagValue ], [])

let GetNotNullWhenTrueAttribute (g: TcGlobals) (propNames: string array) =
    let tref = g.attrib_MemberNotNullWhenAttribute.TypeRef

    g.TryEmbedILType(
        tref,
        (fun () ->
            let fields =
                Some [ "ReturnValue", g.ilg.typ_Bool; "Members", g.ilg.typ_StringArray ]

            mkLocalPrivateAttributeWithPropertyConstructors (g, tref.Name, fields, EncapsulatedProperties))
    )

    let stringArgs =
        propNames |> Array.map (Some >> ILAttribElem.String) |> List.ofArray

    mkILCustomAttribute (
        tref,
        [ g.ilg.typ_Bool; g.ilg.typ_StringArray ],
        [ ILAttribElem.Bool true; ILAttribElem.Array(g.ilg.typ_String, stringArgs) ],
        []
    )

let GetNullableAttribute (g: TcGlobals) (nullnessInfos: TypedTree.NullnessInfo list) =
    let tref = g.attrib_NullableAttribute.TypeRef

    g.TryEmbedILType(tref, (fun () -> mkLocalPrivateAttributeWithByteAndByteArrayConstructors (g, tref.Name, "NullableFlags")))

    let byteValue ni =
        match ni with
        | NullnessInfo.WithNull -> 2uy
        | NullnessInfo.AmbivalentToNull -> 0uy
        | NullnessInfo.WithoutNull -> 1uy

    let bytes = nullnessInfos |> List.map (fun ni -> byteValue ni |> ILAttribElem.Byte)

    match bytes with
    | [ singleByte ] -> mkILCustomAttribute (tref, [ g.ilg.typ_Byte ], [ singleByte ], [])
    | listOfBytes -> mkILCustomAttribute (tref, [ g.ilg.typ_ByteArray ], [ ILAttribElem.Array(g.ilg.typ_Byte, listOfBytes) ], [])

let GenReadOnlyIfNecessary g ty =
    if isInByrefTy g ty then
        let attr = GetReadOnlyAttribute g
        Some attr
    else
        None

(* Nullness metadata format in C#: https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
Each type reference in metadata may have an associated NullableAttribute with a byte[] where each byte represents nullability: 0 for oblivious, 1 for not annotated, and 2 for annotated.

The byte[] is constructed as follows:

Reference type: the nullability (0, 1, or 2), followed by the representation of the type arguments in order including containing types
Nullable value type: the representation of the type argument only
Non-generic value type: skipped
Generic value type: 0, followed by the representation of the type arguments in order including containing types
Array: the nullability (0, 1, or 2), followed by the representation of the element type
Tuple: the representation of the underlying constructed type
Type parameter reference: the nullability (0, 1, or 2, with 0 for unconstrained type parameter)
*)
let rec GetNullnessFromTType (g: TcGlobals) ty =
    match ty |> stripTyEqns g with
    | TType_app(tcref, tinst, nullness) ->
        let isValueType = tcref.IsStructOrEnumTycon
        let isNonGeneric = tinst.IsEmpty

        if isNonGeneric && isValueType then
            // Non-generic value type: skipped
            []
        else
            [
                if tyconRefEq g g.system_Nullable_tcref tcref then
                    // Nullable value type: the representation of the type argument only
                    ()
                else if isValueType then
                    // Generic value type: 0, followed by the representation of the type arguments in order including containing types
                    yield NullnessInfo.AmbivalentToNull
                else if
                    IsUnionTypeWithNullAsTrueValue g tcref.Deref
                    || TypeHasAllowNull tcref g FSharp.Compiler.Text.Range.Zero
                then
                    yield NullnessInfo.WithNull
                else
                    // Reference type: the nullability (0, 1, or 2), followed by the representation of the type arguments in order including containing types
                    yield nullness.Evaluate()

                for tt in tinst do
                    yield! GetNullnessFromTType g tt
            ]

    | TType_fun(domainTy, retTy, nullness) ->
        // FsharpFunc<DomainType,ReturnType>
        [
            yield nullness.Evaluate()
            yield! GetNullnessFromTType g domainTy
            yield! GetNullnessFromTType g retTy
        ]

    | TType_tuple(tupInfo, elementTypes) ->
        // Tuple: the representation of the underlying constructed type
        [
            if evalTupInfoIsStruct tupInfo then
                yield NullnessInfo.AmbivalentToNull
            else
                yield NullnessInfo.WithoutNull
            for t in elementTypes do
                yield! GetNullnessFromTType g t
        ]

    | TType_anon(anonInfo, tys) ->
        // It is unlikely for an anon type to be used from C# due to the mangled name, but can still carry the nullability info about it's generic type arguments == the types of the fields
        [
            if evalAnonInfoIsStruct anonInfo then
                yield NullnessInfo.AmbivalentToNull
            else
                yield NullnessInfo.WithoutNull
            for t in tys do
                yield! GetNullnessFromTType g t
        ]
    | TType_forall _
    | TType_ucase _
    | TType_measure _ -> []
    | TType_var(nullness = nullness) -> [ nullness.Evaluate() ]

let GenNullnessIfNecessary (g: TcGlobals) ty =
    if g.langFeatureNullness && g.checkNullness then
        let nullnessList = GetNullnessFromTType g ty

        match nullnessList with
        // Optimizations as done in C# :: If the byte[] is empty, the NullableAttribute is omitted.
        | [] -> None
        // Optimizations as done in C# :: If all values in the byte[] are the same, the NullableAttribute is constructed with that single byte value.
        | head :: tail when tail |> List.forall ((=) head) ->
            match head with
            // For F# code, each type has an automatically generated NullableContextAttribute(1)
            // That means an implicit (hidden, not generated) Nullable(1) attribute
            | NullnessInfo.WithoutNull -> None
            | _ -> GetNullableAttribute g [ head ] |> Some
        | nonUniformList -> GetNullableAttribute g nonUniformList |> Some
    else
        None

let GenAdditionalAttributesForTy g ty =
    let readOnly = GenReadOnlyIfNecessary g ty |> Option.toList
    let nullable = GenNullnessIfNecessary g ty |> Option.toList
    readOnly @ nullable

/// Generate "modreq([mscorlib]System.Runtime.InteropServices.InAttribute)" on inref types.
let GenReadOnlyModReqIfNecessary (g: TcGlobals) ty ilTy =
    let add = isInByrefTy g ty && g.attrib_InAttribute.TyconRef.CanDeref

    if add then
        ILType.Modified(true, g.attrib_InAttribute.TypeRef, ilTy)
    else
        ilTy
