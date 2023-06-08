// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The ILX generator.
module internal FSharp.Compiler.IlxGenSupport

open System.Reflection
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTreeOps

/// Make a method that simply loads a field
let mkLdfldMethodDef (ilMethName, iLAccess, isStatic, ilTy, ilFieldName, ilPropType, customAttrs) =
    let ilFieldSpec = mkILFieldSpecInTy (ilTy, ilFieldName, ilPropType)
    let ilReturn = mkILReturn ilPropType

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

    mkILGenericClass (
        name,
        ILTypeDefAccess.Private,
        ILGenericParameterDefs.Empty,
        g.ilg.typ_Attribute,
        ILTypes.Empty,
        ilMethods,
        emptyILFields,
        emptyILTypeDefs,
        emptyILProperties,
        emptyILEvents,
        emptyILCustomAttrs,
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

let mkLocalPrivateAttributeWithPropertyConstructors (g: TcGlobals, name: string, attrProperties: (string * ILType) list option) =
    let ilTypeRef = mkILTyRef (ILScopeRef.Local, name)
    let ilTy = mkILFormalNamedTy ILBoxity.AsObject ilTypeRef []

    let ilElements =
        attrProperties
        |> Option.defaultValue []
        |> List.map (fun (name, ilType) ->
            let fieldName = name + "@"

            (g.AddFieldGeneratedAttributes(mkILInstanceField (fieldName, ilType, None, ILMemberAccess.Private))),
            (g.AddMethodGeneratedAttributes(mkLdfldMethodDef ($"get_{name}", ILMemberAccess.Public, false, ilTy, fieldName, ilType, []))),
            (g.AddPropertyGeneratedAttributes(
                mkILNonGenericInstanceProperty (
                    name,
                    ilType,
                    PropertyAttributes.None,
                    emptyILCustomAttrs,
                    Some(mkILMethRef (ilTypeRef, ILCallingConv.Instance, "get_" + name, 0, [], ilType)),
                    None
                )
            )),
            (name, fieldName, ilType))

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
        ILTypes.Empty,
        mkILMethods (
            ilCtorDef
            :: (ilElements |> List.fold (fun acc (_, getter, _, _) -> getter :: acc) [])
        ),
        mkILFields (ilElements |> List.map (fun (field, _, _, _) -> field)),
        emptyILTypeDefs,
        mkILProperties (ilElements |> List.map (fun (_, _, property, _) -> property)),
        emptyILEvents,
        emptyILCustomAttrs,
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
        ILTypes.Empty,
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

let GetReadOnlyAttribute (g: TcGlobals) =
    let tref = g.attrib_IsReadOnlyAttribute.TypeRef
    g.TryEmbedILType(tref, (fun () -> mkLocalPrivateAttributeWithDefaultConstructor (g, tref.Name)))
    mkILCustomAttribute (g.attrib_IsReadOnlyAttribute.TypeRef, [], [], [])

let GenReadOnlyAttributeIfNecessary g ty =
    if isInByrefTy g ty then
        let attr = GetReadOnlyAttribute g
        Some attr
    else
        None

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

                (mkLocalPrivateInt32Enum (g, tref, values)).WithSerializable(true).WithSealed(true))
        )

    ILType.Value(mkILNonGenericTySpec (tref))

let GetDynamicDependencyAttribute (g: TcGlobals) memberTypes (ilType: ILType) =
    let tref = g.attrib_DynamicDependencyAttribute.TypeRef

    g.TryEmbedILType(
        tref,
        (fun () ->
            let properties =
                Some [ "MemberType", GetDynamicallyAccessedMemberTypes g; "Type", g.ilg.typ_Type ]

            mkLocalPrivateAttributeWithPropertyConstructors (g, tref.Name, properties))
    )

    let typIlMemberTypes =
        ILType.Value(mkILNonGenericTySpec (g.enum_DynamicallyAccessedMemberTypes.TypeRef))

    mkILCustomAttribute (
        tref,
        [ typIlMemberTypes; g.ilg.typ_Type ],
        [ ILAttribElem.Int32 memberTypes; ILAttribElem.TypeRef(Some ilType.TypeRef) ],
        []
    )

/// Generate "modreq([mscorlib]System.Runtime.InteropServices.InAttribute)" on inref types.
let GenReadOnlyModReqIfNecessary (g: TcGlobals) ty ilTy =
    let add = isInByrefTy g ty && g.attrib_InAttribute.TyconRef.CanDeref

    if add then
        ILType.Modified(true, g.attrib_InAttribute.TypeRef, ilTy)
    else
        ilTy
