// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Flags enums and generic wrapper for well-known attribute flags.
namespace FSharp.Compiler

/// Flags enum for well-known attributes on Entity (types and modules).
/// Used to avoid O(N) linear scans of attribute lists.
[<System.Flags>]
type internal WellKnownEntityAttributes =
    | None = 0uL
    | RequireQualifiedAccessAttribute = (1uL <<< 0)
    | AutoOpenAttribute = (1uL <<< 1)
    | AbstractClassAttribute = (1uL <<< 2)
    | SealedAttribute_True = (1uL <<< 3)
    | NoEqualityAttribute = (1uL <<< 4)
    | NoComparisonAttribute = (1uL <<< 5)
    | StructuralEqualityAttribute = (1uL <<< 6)
    | StructuralComparisonAttribute = (1uL <<< 7)
    | CustomEqualityAttribute = (1uL <<< 8)
    | CustomComparisonAttribute = (1uL <<< 9)
    | ReferenceEqualityAttribute = (1uL <<< 10)
    | DefaultAugmentationAttribute_True = (1uL <<< 11)
    | CLIMutableAttribute = (1uL <<< 12)
    | AutoSerializableAttribute_True = (1uL <<< 13)
    | StructLayoutAttribute = (1uL <<< 14)
    | DllImportAttribute = (1uL <<< 15)
    | ReflectedDefinitionAttribute = (1uL <<< 16)
    | MeasureableAttribute = (1uL <<< 17)
    | SkipLocalsInitAttribute = (1uL <<< 18)
    | DebuggerTypeProxyAttribute = (1uL <<< 19)
    | ComVisibleAttribute_True = (1uL <<< 20)
    | IsReadOnlyAttribute = (1uL <<< 21)
    | IsByRefLikeAttribute = (1uL <<< 22)
    | ExtensionAttribute = (1uL <<< 23)
    | AttributeUsageAttribute = (1uL <<< 24)
    | WarnOnWithoutNullArgumentAttribute = (1uL <<< 25)
    | AllowNullLiteralAttribute_True = (1uL <<< 26)
    | ClassAttribute = (1uL <<< 27)
    | InterfaceAttribute = (1uL <<< 28)
    | StructAttribute = (1uL <<< 29)
    | MeasureAttribute = (1uL <<< 30)
    | DefaultAugmentationAttribute_False = (1uL <<< 31)
    | AutoSerializableAttribute_False = (1uL <<< 32)
    | ComVisibleAttribute_False = (1uL <<< 33)
    | ObsoleteAttribute = (1uL <<< 34)
    | ComImportAttribute_True = (1uL <<< 35)
    | CompilationRepresentation_ModuleSuffix = (1uL <<< 36)
    | CompilationRepresentation_PermitNull = (1uL <<< 37)
    | CompilationRepresentation_Instance = (1uL <<< 38)
    | CompilationRepresentation_Static = (1uL <<< 39)
    | CLIEventAttribute = (1uL <<< 40)
    | SealedAttribute_False = (1uL <<< 41)
    | AllowNullLiteralAttribute_False = (1uL <<< 42)
    | NotComputed = (1uL <<< 63)

/// Flags enum for well-known attributes on Val (values and members).
/// Used to avoid O(N) linear scans of attribute lists.
[<System.Flags>]
type internal WellKnownValAttributes =
    | None = 0uL
    | DllImportAttribute = (1uL <<< 0)
    | EntryPointAttribute = (1uL <<< 1)
    | LiteralAttribute = (1uL <<< 2)
    | ConditionalAttribute = (1uL <<< 3)
    | ReflectedDefinitionAttribute_True = (1uL <<< 4)
    | RequiresExplicitTypeArgumentsAttribute = (1uL <<< 5)
    | DefaultValueAttribute_True = (1uL <<< 6)
    | SkipLocalsInitAttribute = (1uL <<< 7)
    | ThreadStaticAttribute = (1uL <<< 8)
    | ContextStaticAttribute = (1uL <<< 9)
    | VolatileFieldAttribute = (1uL <<< 10)
    | NoDynamicInvocationAttribute_True = (1uL <<< 11)
    | ExtensionAttribute = (1uL <<< 12)
    | OptionalArgumentAttribute = (1uL <<< 13)
    | InAttribute = (1uL <<< 14)
    | OutAttribute = (1uL <<< 15)
    | ParamArrayAttribute = (1uL <<< 16)
    | CallerMemberNameAttribute = (1uL <<< 17)
    | CallerFilePathAttribute = (1uL <<< 18)
    | CallerLineNumberAttribute = (1uL <<< 19)
    | DefaultParameterValueAttribute = (1uL <<< 20)
    | ProjectionParameterAttribute = (1uL <<< 21)
    | InlineIfLambdaAttribute = (1uL <<< 22)
    | OptionalAttribute = (1uL <<< 23)
    | StructAttribute = (1uL <<< 24)
    | NoCompilerInliningAttribute = (1uL <<< 25)
    | ReflectedDefinitionAttribute_False = (1uL <<< 26)
    | DefaultValueAttribute_False = (1uL <<< 27)
    | NoDynamicInvocationAttribute_False = (1uL <<< 28)
    | GeneralizableValueAttribute = (1uL <<< 29)
    | CLIEventAttribute = (1uL <<< 30)
    | NotComputed = (1uL <<< 63)

/// Generic wrapper for an item list together with cached well-known attribute flags.
/// Used for O(1) lookup of well-known attributes on entities and vals.
[<Struct; NoEquality; NoComparison>]
type internal WellKnownAttribs<'TItem, 'TFlags when 'TFlags: enum<uint64>> =
    val private attribs: 'TItem list
    val private flags: 'TFlags

    new(attribs: 'TItem list, flags: 'TFlags) = { attribs = attribs; flags = flags }

    /// Check if a specific well-known attribute flag is set.
    member x.HasWellKnownAttribute(flag: 'TFlags) : bool =
        let f = LanguagePrimitives.EnumToValue x.flags
        let v = LanguagePrimitives.EnumToValue flag
        f &&& v <> 0uL

    /// Get the underlying attribute list (for remap/display/serialization/full-data extraction).
    member x.AsList() = x.attribs

    /// Get the current flags value.
    member x.Flags = x.flags

    /// Add a single item and OR-in its flag.
    member x.Add(attrib: 'TItem, flag: 'TFlags) =
        let combined =
            LanguagePrimitives.EnumOfValue(LanguagePrimitives.EnumToValue x.flags ||| LanguagePrimitives.EnumToValue flag)

        WellKnownAttribs<'TItem, 'TFlags>(attrib :: x.attribs, combined)

    /// Append items and OR-in flags.
    member x.Append(others: 'TItem list, flags: 'TFlags) =
        let combined =
            LanguagePrimitives.EnumOfValue(LanguagePrimitives.EnumToValue x.flags ||| LanguagePrimitives.EnumToValue flags)

        WellKnownAttribs<'TItem, 'TFlags>(x.attribs @ others, combined)

    /// Returns a copy with recomputed flags (flags set to NotComputed, i.e. bit 63).
    member x.WithRecomputedFlags() =
        if x.attribs.IsEmpty then
            WellKnownAttribs<'TItem, 'TFlags>([], LanguagePrimitives.EnumOfValue 0uL)
        else
            WellKnownAttribs<'TItem, 'TFlags>(x.attribs, LanguagePrimitives.EnumOfValue(1uL <<< 63))
