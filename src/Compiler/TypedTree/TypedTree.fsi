/// Defines the typed abstract syntax intermediate representation used throughout the F# compiler.
module internal rec FSharp.Compiler.TypedTree

open System
open System.Diagnostics
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypeProviders
open FSharp.Compiler.Xml
open FSharp.Core.CompilerServices

type Stamp = int64

type StampMap<'T> = Map<Stamp, 'T>

[<RequireQualifiedAccess>]
type ValInline =

    /// Indicates the value is inlined but the .NET IL code for the function still exists, e.g. to satisfy interfaces on objects, but that it is also always inlined
    | Always

    /// Indicates the value may optionally be inlined by the optimizer
    | Optional

    /// Indicates the value must never be inlined by the optimizer
    | Never

    /// Returns true if the implementation of a value must always be inlined
    member MustInline: bool

/// A flag associated with values that indicates whether the recursive scope of the value is currently being processed, type
/// if the value has been generalized or not as yet.
type ValRecursiveScopeInfo =

    /// Set while the value is within its recursive scope. The flag indicates if the value has been eagerly generalized type accepts generic-recursive calls
    | ValInRecScope of bool

    /// The normal value for this flag when the value is not within its recursive scope
    | ValNotInRecScope

type ValMutability =
    | Immutable
    | Mutable

/// Indicates if a type parameter is needed at runtime type may not be eliminated
[<RequireQualifiedAccess>]
type TyparDynamicReq =

    /// Indicates the type parameter is not needed at runtime type may be eliminated
    | No

    /// Indicates the type parameter is needed at runtime type may not be eliminated
    | Yes

type ValBaseOrThisInfo =

    /// Indicates a ref-cell holding 'this' or the implicit 'this' used throughout an
    /// implicit constructor to access type set values
    | CtorThisVal

    /// Indicates the value called 'base' available for calling base class members
    | BaseVal

    /// Indicates a normal value
    | NormalVal

    /// Indicates the 'this' value specified in a memberm e.g. 'x' in 'member x.M() = 1'
    | MemberThisVal

/// Flags on values
[<Struct>]
type ValFlags =

    new:
        recValInfo: ValRecursiveScopeInfo *
        baseOrThis: ValBaseOrThisInfo *
        isCompGen: bool *
        inlineInfo: ValInline *
        isMutable: ValMutability *
        isModuleOrMemberBinding: bool *
        isExtensionMember: bool *
        isIncrClassSpecialMember: bool *
        isTyFunc: bool *
        allowTypeInst: bool *
        isGeneratedEventVal: bool ->
            ValFlags

    new: flags: int64 -> ValFlags

    member WithIsCompilerGenerated: isCompGen: bool -> ValFlags

    member WithRecursiveValInfo: recValInfo: ValRecursiveScopeInfo -> ValFlags

    member BaseOrThisInfo: ValBaseOrThisInfo

    member HasBeenReferenced: bool

    member IgnoresByrefScope: bool

    member InlineIfLambda: bool

    member InlineInfo: ValInline

    member IsCompiledAsStaticPropertyWithoutField: bool

    member IsCompilerGenerated: bool

    member IsExtensionMember: bool

    member IsFixed: bool

    member IsGeneratedEventVal: bool

    member IsIncrClassSpecialMember: bool

    member IsMemberOrModuleBinding: bool

    member IsTypeFunction: bool

    member MakesNoCriticalTailcalls: bool

    member MutabilityInfo: ValMutability

    member PermitsExplicitTypeInstantiation: bool

    /// Get the flags as included in the F# binary metadata
    member PickledBits: int64

    member RecursiveValInfo: ValRecursiveScopeInfo

    member WithHasBeenReferenced: ValFlags

    member WithIgnoresByrefScope: ValFlags

    member WithInlineIfLambda: ValFlags

    member WithIsCompiledAsStaticPropertyWithoutField: ValFlags

    member WithIsFixed: ValFlags

    member WithIsMemberOrModuleBinding: ValFlags

    member WithMakesNoCriticalTailcalls: ValFlags

/// Represents the kind of a type parameter
[<RequireQualifiedAccess>]
type TyparKind =
    | Type
    | Measure

    override ToString: unit -> string

    member AttrName: string voption

/// Indicates if the type variable can be solved or given new constraints. The status of a type variable
/// evolves towards being either rigid or solved.
[<RequireQualifiedAccess>]
type TyparRigidity =

    /// Indicates the type parameter can't be solved
    | Rigid

    /// Indicates the type parameter can't be solved, but the variable is not set to "rigid" until after inference is complete
    | WillBeRigid

    /// Indicates we give a warning if the type parameter is ever solved
    | WarnIfNotRigid

    /// Indicates the type parameter is an inference variable may be solved
    | Flexible

    /// Indicates the type parameter derives from an '_' anonymous type
    /// For units-of-measure, we give a warning if this gets solved to '1'
    | Anon

    member ErrorIfUnified: bool

    member WarnIfMissingConstraint: bool

    member WarnIfUnified: bool

/// Encode typar flags into a bit field
[<Struct>]
type TyparFlags =

    new:
        kind: TyparKind *
        rigidity: TyparRigidity *
        isFromError: bool *
        isCompGen: bool *
        staticReq: Syntax.TyparStaticReq *
        dynamicReq: TyparDynamicReq *
        equalityDependsOn: bool *
        comparisonDependsOn: bool ->
            TyparFlags

    new: flags: int32 -> TyparFlags

    member WithCompatFlex: b: bool -> TyparFlags

    /// Indicates that whether or not a generic type definition satisfies the comparison constraint is dependent on whether this type variable satisfies the comparison constraint.
    member ComparisonConditionalOn: bool

    /// Indicates if a type parameter is needed at runtime type may not be eliminated
    member DynamicReq: TyparDynamicReq

    /// Indicates that whether or not a generic type definition satisfies the equality constraint is dependent on whether this type variable satisfies the equality constraint.
    member EqualityConditionalOn: bool

    /// Indicates that whether this type parameter is a compat-flex type parameter (i.e. where "expr :> tp" only emits an optional warning)
    member IsCompatFlex: bool

    /// Indicates if the type variable is compiler generated, i.e. is an implicit type inference variable
    member IsCompilerGenerated: bool

    /// Indicates if the type inference variable was generated after an error when type checking expressions or patterns
    member IsFromError: bool

    /// Indicates whether a type variable can be instantiated by types or units-of-measure.
    member Kind: TyparKind

    /// Get the flags as included in the F# binary metadata. We pickle this as int64 to allow for future expansion
    member PickledBits: int32

    /// Indicates if the type variable can be solved or given new constraints. The status of a type variable
    /// generally always evolves towards being either rigid or solved.
    member Rigidity: TyparRigidity

    /// Indicates if the type variable has a static "head type" requirement, i.e. ^a variables used in FSharp.Core type member constraints.
    member StaticReq: Syntax.TyparStaticReq

/// Encode entity flags into a bit field. We leave lots of space to allow for future expansion.
[<Struct>]
type EntityFlags =

    new:
        usesPrefixDisplay: bool *
        isModuleOrNamespace: bool *
        preEstablishedHasDefaultCtor: bool *
        hasSelfReferentialCtor: bool *
        isStructRecordOrUnionType: bool ->
            EntityFlags

    new: flags: int64 -> EntityFlags

    /// Adjust the on-demand analysis about whether the entity is assumed to be a readonly struct
    member WithIsAssumedReadOnly: flag: bool -> EntityFlags

    /// Adjust the on-demand analysis about whether the entity has the IsByRefLike attribute
    member WithIsByRefLike: flag: bool -> EntityFlags

    /// Adjust the on-demand analysis about whether the entity has the IsReadOnly attribute
    member WithIsReadOnly: flag: bool -> EntityFlags

    member HasSelfReferentialConstructor: bool

    /// Indicates the Entity is actually a module or namespace, not a type definition
    member IsModuleOrNamespace: bool

    /// Indicates the type prefers the "tycon<a, b>" syntax for display etc.
    member IsPrefixDisplay: bool

    /// This bit represents a F# record that is a value type, or a struct record.
    member IsStructRecordOrUnionType: bool

    /// Get the flags as included in the F# binary metadata
    member PickledBits: int64

    member PreEstablishedHasDefaultConstructor: bool

    /// These two bits represents the on-demand analysis about whether the entity is assumed to be a readonly struct
    member TryIsAssumedReadOnly: bool voption

    /// These two bits represents the on-demand analysis about whether the entity has the IsByRefLike attribute
    member TryIsByRefLike: bool voption

    /// These two bits represents the on-demand analysis about whether the entity has the IsReadOnly attribute
    member TryIsReadOnly: bool voption

    /// This bit is reserved for us in the pickle format, see pickle.fs, it's being listed here to stop it ever being used for anything else
    static member ReservedBitForPickleFormatTyconReprFlag: int64

exception UndefinedName of depth: int * error: (string -> string) * id: Ident * suggestions: Suggestions

exception InternalUndefinedItemRef of (string * string * string -> int * string) * string * string * string

type ModuleOrNamespaceKind =

    /// Indicates that a module is compiled to a class with the "Module" suffix added.
    | FSharpModuleWithSuffix

    /// Indicates that a module is compiled to a class with the same name as the original module
    | ModuleOrType

    /// Indicates that a 'module' is really a namespace
    | Namespace of
        /// Indicates that the sourcecode had a namespace.
        /// If false, this namespace was implicitly constructed during type checking.
        isExplicit: bool

/// A public path records where a construct lives within the global namespace
/// of a CCU.
type PublicPath =
    | PubPath of string[]

    member EnclosingPath: string[]

/// The information ILXGEN needs about the location of an item
type CompilationPath =
    | CompPath of ILScopeRef * (string * ModuleOrNamespaceKind) list

    /// String 'Module' off an F# module name, if FSharpModuleWithSuffix is used
    static member DemangleEntityName: nm: string -> k: ModuleOrNamespaceKind -> string

    member NestedCompPath: n: string -> moduleKind: ModuleOrNamespaceKind -> CompilationPath

    member NestedPublicPath: id: Syntax.Ident -> PublicPath

    member AccessPath: (string * ModuleOrNamespaceKind) list

    member DemangledPath: string list

    member ILScopeRef: ILScopeRef

    member MangledPath: string list

    member ParentCompPath: CompilationPath

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type EntityOptionalData =
    {

        /// The name of the type, possibly with `n mangling
        mutable entity_compiled_name: string option

        /// If this field is populated, this is the implementation range for an item in a signature, otherwise it is
        /// the signature range for an item in an implementation
        mutable entity_other_range: (range * bool) option

        mutable entity_kind: TyparKind

        /// The declared documentation for the type or module
        mutable entity_xmldoc: XmlDoc

        /// The XML document signature for this entity
        mutable entity_xmldocsig: string

        /// If non-None, indicates the type is an abbreviation for another type.
        mutable entity_tycon_abbrev: TType option

        /// The declared accessibility of the representation, not taking signatures into account
        mutable entity_tycon_repr_accessibility: Accessibility

        /// Indicates how visible is the entity is.
        mutable entity_accessibility: Accessibility

        /// Field used when the 'tycon' is really an exception definition
        mutable entity_exn_info: ExceptionInfo
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents a type definition, exception definition, module definition or namespace definition.
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type Entity =
    {

        /// The declared type parameters of the type
        mutable entity_typars: LazyWithContext<Typars, range>

        mutable entity_flags: EntityFlags

        /// The unique stamp of the "tycon blob". Note the same tycon in signature type implementation get different stamps
        mutable entity_stamp: Stamp

        /// The name of the type, possibly with `n mangling
        mutable entity_logical_name: string

        /// The declaration location for the type constructor
        mutable entity_range: range

        /// The declared attributes for the type
        mutable entity_attribs: Attribs

        /// The declared representation of the type, i.e. record, union, class etc.
        mutable entity_tycon_repr: TyconRepresentation

        /// The methods type properties of the type
        mutable entity_tycon_tcaug: TyconAugmentation

        /// This field is used when the 'tycon' is really a module definition. It holds statically nested type definitions type nested modules
        mutable entity_modul_type: MaybeLazy<ModuleOrNamespaceType>

        /// The stable path to the type, e.g. Microsoft.FSharp.Core.FSharpFunc`2
        mutable entity_pubpath: PublicPath option

        /// The stable path to the type, e.g. Microsoft.FSharp.Core.FSharpFunc`2
        mutable entity_cpath: CompilationPath option

        /// Used during codegen to hold the ILX representation indicating how to access the type
        mutable entity_il_repr_cache: cache<CompiledTypeRepr>

        mutable entity_opt_data: EntityOptionalData option
    }

    /// Create a new entity with the given backing data. Only used during unpickling of F# metadata.
    static member New: _reason: 'b -> data: Entity -> Entity

    static member NewEmptyEntityOptData: unit -> EntityOptionalData

    /// Create a new entity with empty, unlinked data. Only used during unpickling of F# metadata.
    static member NewUnlinked: unit -> Entity

    member GetDisplayName: coreName: bool * ?withStaticParameters: bool * ?withUnderscoreTypars: bool -> string

    /// Get a field by index in definition order
    member GetFieldByIndex: n: int -> RecdField

    /// Get a field by name.
    member GetFieldByName: n: string -> RecdField option

    /// Get a union case of a type by name
    member GetUnionCaseByName: n: string -> UnionCase option

    /// Link an entity based on empty, unlinked data to the given data. Only used during unpickling of F# metadata.
    member Link: tg: EntityData -> unit

    /// Set the custom attributes on an F# type definition.
    member SetAttribs: attribs: Attribs -> unit

    member SetCompiledName: name: string option -> unit

    member SetExceptionInfo: exn_info: ExceptionInfo -> unit

    /// Set the on-demand analysis about whether the entity is assumed to be a readonly struct
    member SetIsAssumedReadOnly: b: bool -> unit

    /// Set the on-demand analysis about whether the entity has the IsByRefLike attribute
    member SetIsByRefLike: b: bool -> unit

    /// Set the on-demand analysis about whether the entity has the IsReadOnly attribute
    member SetIsReadOnly: b: bool -> unit

    /// Sets the structness of a record or union type definition
    member SetIsStructRecordOrUnion: b: bool -> unit

    member SetOtherRange: m: (range * bool) -> unit

    member SetTypeAbbrev: tycon_abbrev: TType option -> unit

    member SetTypeOrMeasureKind: kind: TyparKind -> unit

    override ToString: unit -> string

    /// Get the type parameters for an entity that is a type declaration, otherwise return the empty list.
    ///
    /// Lazy because it may read metadata, must provide a context "range" in case error occurs reading metadata.
    member Typars: m: range -> Typars

    /// Get the value representing the accessibility of an F# type definition or module.
    member Accessibility: Accessibility

    /// Get a table of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldTable: TyconRecdFields

    /// Get an array of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldsArray: RecdField[]

    /// Get a list of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldsAsList: RecdField list

    /// Gets all implicit hash/equals/compare methods added to an F# record, union or struct type definition.
    member AllGeneratedValues: ValRef list

    /// Get a list of all instance fields for F#-defined record, struct type class fields in this type definition.
    /// including hidden fields from the compilation of implicit class constructions.
    member AllInstanceFieldsAsList: RecdField list

    /// The F#-defined custom attributes of the entity, if any. If the entity is backed by Abstract IL or provided metadata
    /// then this does not include any attributes from those sources.
    member Attribs: Attribs

    /// Get a blob of data indicating how this type is nested inside other namespaces, modules type types.
    member CompilationPath: CompilationPath

    /// Get a blob of data indicating how this type is nested inside other namespaces, modules type types.
    member CompilationPathOpt: CompilationPath option

    /// The compiled name of the namespace, module or type, e.g. FSharpList`1, ListModule or FailureException
    member CompiledName: string

    /// Get the cache of the compiled ILTypeRef representation of this module or type.
    member CompiledReprCache: cache<CompiledTypeRepr>

    /// Gets the data indicating the compiled representation of a type or module in terms of Abstract IL data structures.
    member CompiledRepresentation: CompiledTypeRepr

    /// Gets the data indicating the compiled representation of a named type or module in terms of Abstract IL data structures.
    member CompiledRepresentationForNamedType: ILTypeRef

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The range in the implementation, adjusted for an item in a signature
    member DefinitionRange: range

    /// Demangle the module name, if FSharpModuleWithSuffix is used
    member DemangledModuleOrNamespaceName: string

    /// The display name of the namespace, module or type, e.g. List instead of List`1, type no static parameters
    /// For modules the Module suffix is removed if FSharpModuleWithSuffix is used.
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayName: string

    /// The display name of the namespace, module or type, e.g. List instead of List`1, type no static parameters.
    /// For modules the Module suffix is removed if FSharpModuleWithSuffix is used.
    ///
    /// No backticks are added for entities with non-identifier names
    member DisplayNameCore: string

    /// The display name of the namespace, module or type, e.g. List instead of List`1, including static parameters if any
    /// For modules the Module suffix is removed if FSharpModuleWithSuffix is used.
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayNameWithStaticParameters: string

    /// The display name of the namespace, module or type with <_, _, _> added for generic types, plus static parameters if any
    /// For modules the Module suffix is removed if FSharpModuleWithSuffix is used.
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayNameWithStaticParametersAndUnderscoreTypars: string

    member EntityCompiledName: string option

    /// The information about the r.h.s. of an F# exception definition, if any.
    member ExceptionInfo: ExceptionInfo

    /// Get the blob of information associated with an F# object-model type definition, i.e. class, interface, struct etc.
    member FSharpObjectModelTypeInfo: TyconObjModelData

    /// Gets any implicit CompareTo methods added to an F# record, union or struct type definition.
    member GeneratedCompareToValues: (ValRef * ValRef) option

    /// Gets any implicit CompareTo (with comparer argument) methods added to an F# record, union or struct type definition.
    member GeneratedCompareToWithComparerValues: ValRef option

    /// Gets any implicit hash/equals methods added to an F# record, union or struct type definition.
    member GeneratedHashAndEqualsValues: (ValRef * ValRef) option

    /// Gets any implicit hash/equals (with comparer argument) methods added to an F# record, union or struct type definition.
    member GeneratedHashAndEqualsWithComparerValues: (ValRef * ValRef * ValRef) option

    /// Indicates if we have pre-determined that a type definition has a self-referential constructor using 'as x'
    member HasSelfReferentialConstructor: bool

    /// Get the Abstract IL scope, nesting type metadata for this
    /// type definition, assuming it is backed by Abstract IL metadata.
    member ILTyconInfo: TILObjectReprData

    /// Get the Abstract IL metadata for this type definition, assuming it is backed by Abstract IL metadata.
    member ILTyconRawMetadata: ILTypeDef

    /// The identifier at the point of declaration of the type definition.
    member Id: Syntax.Ident

    /// Gets the immediate interface types of an F# type definition. Further interfaces may be supported through class type interface inheritance.
    member ImmediateInterfaceTypesOfFSharpTycon: TType list

    /// Gets the immediate interface definitions of an F# type definition. Further interfaces may be supported through class type interface inheritance.
    member ImmediateInterfacesOfFSharpTycon: (TType * bool * range) list

    /// Indicates if this is an F# type definition which is one of the special types in FSharp.Core.dll which uses
    /// an assembly-code representation for the type, e.g. the primitive array type constructor.
    member IsAsmReprTycon: bool

    /// Indicates if this is an enum type definition
    member IsEnumTycon: bool

    /// Indicates if the entity is erased, either a measure definition, or an erased provided type definition
    member IsErased: bool

    /// Indicates if this is an F#-defined class type definition
    member IsFSharpClassTycon: bool

    /// Indicates if this is an F#-defined delegate type definition
    member IsFSharpDelegateTycon: bool

    /// Indicates if this is an F#-defined enum type definition
    member IsFSharpEnumTycon: bool

    /// Indicates if the entity represents an F# exception declaration.
    member IsFSharpException: bool

    /// Indicates if this is an F#-defined interface type definition
    member IsFSharpInterfaceTycon: bool

    /// Indicates if this is an F# type definition whose r.h.s. is known to be some kind of F# object model definition
    member IsFSharpObjectModelTycon: bool

    /// Indicates if this is an F#-defined struct or enum type definition, i.e. a value type definition
    member IsFSharpStructOrEnumTycon: bool

    /// Indicates if this is an F# type definition whose r.h.s. definition is unknown (i.e. a traditional ML 'abstract' type in a signature,
    /// which in F# is called a 'unknown representation' type).
    member IsHiddenReprTycon: bool

    /// Indicates if this is a .NET-defined enum type definition
    member IsILEnumTycon: bool

    /// Indicates if this is a .NET-defined struct or enum type definition, i.e. a value type definition
    member IsILStructOrEnumTycon: bool

    /// Indicate if this is a type definition backed by Abstract IL metadata.
    member IsILTycon: bool

    /// Indicates if the entity is linked to backing data. Only used during unpickling of F# metadata.
    member IsLinked: bool

    /// Indicates if this is an F# type definition which is one of the special types in FSharp.Core.dll like 'float<_>' which
    /// defines a measure type with a relation to an existing non-measure type as a representation.
    member IsMeasureableReprTycon: bool

    /// Indicates if the entity is an F# module definition
    member IsModule: bool

    /// Indicates the Entity is actually a module or namespace, not a type definition
    member IsModuleOrNamespace: bool

    /// Indicates if the entity is a namespace
    member IsNamespace: bool

    /// Indicates if the entity has an implicit namespace
    member IsImplicitNamespace: bool

    /// Indicates the type prefers the "tycon<a, b>" syntax for display etc.
    member IsPrefixDisplay: bool

#if !NO_TYPEPROVIDERS
    /// Indicates if the entity is a provided type or namespace definition
    member IsProvided: bool

    /// Indicates if the entity is an erased provided type definition
    member IsProvidedErasedTycon: bool

    /// Indicates if the entity is a generated provided type definition, i.e. not erased.
    member IsProvidedGeneratedTycon: bool

    /// Indicates if the entity is a provided namespace fragment
    member IsProvidedNamespace: bool
#endif

    /// Indicates if this is an F# type definition whose r.h.s. is known to be a record type definition.
    member IsRecordTycon: bool

#if !NO_TYPEPROVIDERS
    member IsStaticInstantiationTycon: bool
#endif

    /// Indicates if this is a struct or enum type definition, i.e. a value type definition
    member IsStructOrEnumTycon: bool

    /// Indicates if this is an F# type definition whose r.h.s. is known to be a record type definition that is a value type.
    member IsStructRecordOrUnionTycon: bool

    /// Indicates if this entity is an F# type abbreviation definition
    member IsTypeAbbrev: bool

    /// Indicate if this is a type whose r.h.s. is known to be a union type definition.
    member IsUnionTycon: bool

    /// The name of the namespace, module or type, possibly with mangling, e.g. List`1, List or FailureException
    member LogicalName: string

    /// Gets all immediate members of an F# type definition keyed by name, including compiler-generated ones.
    /// Note: result is a indexed table, type for each name the results are in reverse declaration order
    member MembersOfFSharpTyconByName: NameMultiMap<ValRef>

    /// Gets the immediate members of an F# type definition, excluding compiler-generated ones.
    /// Note: result is alphabetically sorted, then for each name the results are in declaration order
    member MembersOfFSharpTyconSorted: ValRef list

    /// The logical contents of the entity when it is a module or namespace fragment.
    member ModuleOrNamespaceType: ModuleOrNamespaceType

    /// Indicates if we have pre-determined that a type definition has a default constructor.
    member PreEstablishedHasDefaultConstructor: bool

    /// Get a blob of data indicating how this type is nested in other namespaces, modules or types.
    member PublicPath: PublicPath option

    /// The code location where the module, namespace or type is defined.
    member Range: range

    member SigRange: range

    /// A unique stamp for this module, namespace or type definition within the context of this compilation.
    /// Note that because of signatures, there are situations where in a single compilation the "same"
    /// module, namespace or type may have two distinct Entity objects that have distinct stamps.
    member Stamp: Stamp

    /// Get a list of all fields for F#-defined record, struct type class fields in this type definition,
    /// including static fields, but excluding compiler-generate fields.
    member TrueFieldsAsList: RecdField list

    /// Get a list of all instance fields for F#-defined record, struct type class fields in this type definition,
    /// excluding compiler-generate fields.
    member TrueInstanceFieldsAsList: RecdField list

    /// These two bits represents the on-demand analysis about whether the entity is assumed to be a readonly struct
    member TryIsAssumedReadOnly: bool voption

    /// The on-demand analysis about whether the entity has the IsByRefLike attribute
    member TryIsByRefLike: bool voption

    /// These two bits represents the on-demand analysis about whether the entity has the IsReadOnly attribute
    member TryIsReadOnly: bool voption

    /// Get the type parameters for an entity that is a type declaration, otherwise return the empty list.
    member TyparsNoRange: Typars

    /// Get the type abbreviated by this type definition, if it is an F# type abbreviation definition
    member TypeAbbrev: TType option

    /// The logical contents of the entity when it is a type definition.
    member TypeContents: TyconAugmentation

    /// The kind of the type definition - is it a measure definition or a type definition?
    member TypeOrMeasureKind: TyparKind

    /// Get the value representing the accessibility of the r.h.s. of an F# type definition.
    member TypeReprAccessibility: Accessibility

    /// The information about the r.h.s. of a type definition, if any. For example, the r.h.s. of a union or record type.
    member TypeReprInfo: TyconRepresentation

    /// Get the union cases for a type, if any
    member UnionCasesArray: UnionCase[]

    /// Get the union cases for a type, if any, as a list
    member UnionCasesAsList: UnionCase list

    /// Get the union cases type other union-type information for a type, if any
    member UnionTypeInfo: TyconUnionData voption

    /// The XML documentation of the entity, if any. If the entity is backed by provided metadata
    /// then this _does_ include this documentation. If the entity is backed by Abstract IL metadata
    /// or comes from another F# assembly then it does not (because the documentation will get read from
    /// an XML file).
    member XmlDoc: XmlDoc

    /// The XML documentation sig-string of the entity, if any, to use to lookup an .xml doc file. This also acts
    /// as a cache for this sig-string computation.
    member XmlDocSig: string with get, set

type EntityData = Entity

/// Represents the parent entity of a type definition, if any
type ParentRef =
    | Parent of parent: EntityRef
    | ParentNone

/// Specifies the compiled representations of type type exception definitions. Basically
/// just an ILTypeRef. Computed type cached by later phases. Stored in
/// type type exception definitions. Not pickled. Store an optional ILType object for
/// non-generic types.
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type CompiledTypeRepr =

    /// An AbstractIL type representation that is just the name of a type.
    ///
    /// CompiledTypeRepr.ILAsmNamed (ilTypeRef, ilBoxity, ilTypeOpt)
    ///
    /// The ilTypeOpt is present for non-generic types. It is an ILType corresponding to the first two elements of the case. This
    /// prevents reallocation of the ILType each time we need to generate it. For generic types, it is None.
    | ILAsmNamed of ilTypeRef: ILTypeRef * ilBoxity: ILBoxity * ilTypeOpt: ILType option

    /// An AbstractIL type representation that may include type variables
    | ILAsmOpen of ilType: ILType

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TyconAugmentation =
    {

        /// This is the value implementing the auto-generated comparison
        /// semantics if any. It is not present if the type defines its own implementation
        /// of IComparable or if the type doesn't implement IComparable implicitly.
        mutable tcaug_compare: (ValRef * ValRef) option

        /// This is the value implementing the auto-generated comparison
        /// semantics if any. It is not present if the type defines its own implementation
        /// of IStructuralComparable or if the type doesn't implement IComparable implicitly.
        mutable tcaug_compare_withc: ValRef option

        /// This is the value implementing the auto-generated equality
        /// semantics if any. It is not present if the type defines its own implementation
        /// of Object.Equals or if the type doesn't override Object.Equals implicitly.
        mutable tcaug_equals: (ValRef * ValRef) option

        /// This is the value implementing the auto-generated comparison
        /// semantics if any. It is not present if the type defines its own implementation
        /// of IStructuralEquatable or if the type doesn't implement IComparable implicitly.
        mutable tcaug_hash_and_equals_withc: (ValRef * ValRef * ValRef) option

        /// True if the type defined an Object.GetHashCode method. In this
        /// case we give a warning if we auto-generate a hash method since the semantics may not match up
        mutable tcaug_hasObjectGetHashCode: bool

        /// Properties, methods etc. in declaration order. The boolean flag for each indicates if the
        /// member is known to be an explicit interface implementation. This must be computed and
        /// saved prior to remapping assembly information.
        tcaug_adhoc_list: ResizeArray<bool * ValRef>

        /// Properties, methods etc. as lookup table
        mutable tcaug_adhoc: NameMultiMap<ValRef>

        /// Interface implementations - boolean indicates compiler-generated
        mutable tcaug_interfaces: (TType * bool * range) list

        /// Super type, if any
        mutable tcaug_super: TType option

        /// Set to true at the end of the scope where proper augmentations are allowed
        mutable tcaug_closed: bool

        /// Set to true if the type is determined to be abstract
        mutable tcaug_abstract: bool
    }

    static member Create: unit -> TyconAugmentation

    member SetCompare: x: (ValRef * ValRef) -> unit

    member SetCompareWith: x: ValRef -> unit

    member SetEquals: x: (ValRef * ValRef) -> unit

    member SetHasObjectGetHashCode: b: bool -> unit

    member SetHashAndEqualsWith: x: (ValRef * ValRef * ValRef) -> unit

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// The information for the contents of a type. Also used for a provided namespace.
[<NoEquality; NoComparison>]
type TyconRepresentation =

    /// Indicates the type is a class, struct, enum, delegate or interface
    | TFSharpObjectRepr of TyconObjModelData

    /// Indicates the type is a record
    | TFSharpRecdRepr of TyconRecdFields

    /// Indicates the type is a discriminated union
    | TFSharpUnionRepr of TyconUnionData

    /// Indicates the type is a type from a .NET assembly without F# metadata.
    | TILObjectRepr of TILObjectReprData

    /// Indicates the type is implemented as IL assembly code using the given closed Abstract IL type
    | TAsmRepr of ILType

    /// Indicates the type is parameterized on a measure (e.g. float<_>) but erases to some other type (e.g. float)
    | TMeasureableRepr of TType

#if !NO_TYPEPROVIDERS
    /// TProvidedTypeRepr
    ///
    /// Indicates the representation information for a provided type.
    | TProvidedTypeRepr of TProvidedTypeInfo

    /// Indicates the representation information for a provided namespace.
    | TProvidedNamespaceRepr of ResolutionEnvironment * Tainted<ITypeProvider> list
#endif

    /// The 'NoRepr' value here has four meanings:
    ///     (1) it indicates 'not yet known' during the first 2 phases of establishing type definitions
    ///     (2) it indicates 'no representation', i.e. 'type X' in signatures
    ///     (3) it is the setting used for exception definitions (!)
    ///     (4) it is the setting used for modules type namespaces.
    ///
    /// It would be better to separate the "not yet known" type other cases out.
    /// The information for exception definitions should be folded into here.
    | TNoRepr

    override ToString: unit -> string

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TILObjectReprData =
    | TILObjectReprData of scope: ILScopeRef * nesting: ILTypeDef list * definition: ILTypeDef

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

#if !NO_TYPEPROVIDERS

/// The information kept about a provided type
[<NoComparison; NoEquality; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TProvidedTypeInfo =
    {

        /// The parameters given to the provider that provided to this type.
        ResolutionEnvironment: ResolutionEnvironment

        /// The underlying System.Type (wrapped as a ProvidedType to make sure we don't call random things on
        /// System.Type, type wrapped as Tainted to make sure we track which provider this came from, for reporting
        /// error messages)
        ProvidedType: Tainted<ProvidedType>

        /// The base type of the type. We use it to compute the compiled representation of the type for erased types.
        /// Reading is delayed, since it does an import on the underlying type
        LazyBaseType: LazyWithContext<TType, (range * TType)>

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsClass: bool

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsSealed: bool

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsAbstract: bool

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsInterface: bool

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsStructOrEnum: bool

        /// A flag read eagerly from the provided type type used to compute basic properties of the type definition.
        IsEnum: bool

        /// A type read from the provided type type used to compute basic properties of the type definition.
        /// Reading is delayed, since it does an import on the underlying type
        UnderlyingTypeOfEnum: unit -> TType

        /// A flag read from the provided type type used to compute basic properties of the type definition.
        /// Reading is delayed, since it looks at the .BaseType
        IsDelegate: unit -> bool

        /// Indicates the type is erased
        IsErased: bool

        /// Indicates the type is generated, but type-relocation is suppressed
        IsSuppressRelocate: bool
    }

    /// Gets the base type of an erased provided type
    member BaseTypeForErased: m: range * objTy: TType -> TType

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Indicates if the provided type is generated, i.e. not erased
    member IsGenerated: bool

#endif

type TyconFSharpObjModelKind =

    /// Indicates the type is an F#-declared class (also used for units-of-measure)
    | TFSharpClass

    /// Indicates the type is an F#-declared interface
    | TFSharpInterface

    /// Indicates the type is an F#-declared struct
    | TFSharpStruct

    /// Indicates the type is an F#-declared delegate with the given Invoke signature
    | TFSharpDelegate of slotSig: SlotSig

    /// Indicates the type is an F#-declared enumeration
    | TFSharpEnum

    /// Indicates if the type definition is a value type
    member IsValueType: bool

/// Represents member values type class fields relating to the F# object model
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TyconObjModelData =
    {

        /// Indicates whether the type declaration is an F# class, interface, enum, delegate or struct
        fsobjmodel_kind: TyconFSharpObjModelKind

        /// The declared abstract slots of the class, interface or struct
        fsobjmodel_vslots: ValRef list

        /// The fields of the class, struct or enum
        fsobjmodel_rfields: TyconRecdFields
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents record fields in an F# type definition
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TyconRecdFields =
    {

        /// The fields of the record, in declaration order.
        FieldsByIndex: RecdField[]

        /// The fields of the record, indexed by name.
        FieldsByName: NameMap<RecdField>
    }

    /// Get a field by index
    member FieldByIndex: n: int -> RecdField

    /// Get a field by name
    member FieldByName: nm: string -> RecdField option

    override ToString: unit -> string

    /// Get all the fields as a list
    member AllFieldsAsList: RecdField list

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get all non-compiler-generated fields as a list
    member TrueFieldsAsList: RecdField list

    /// Get all non-compiler-generated instance fields as a list
    member TrueInstanceFieldsAsList: RecdField list

/// Represents union cases in an F# type definition
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TyconUnionCases =
    {

        /// The cases of the discriminated union, in declaration order.
        CasesByIndex: UnionCase[]

        /// The cases of the discriminated union, indexed by name.
        CasesByName: NameMap<UnionCase>
    }

    /// Get a union case by index
    member GetUnionCaseByIndex: n: int -> UnionCase

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the union cases as a list
    member UnionCasesAsList: UnionCase list

/// Represents the union cases type related information in an F# type definition
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TyconUnionData =
    {

        /// The cases contained in the discriminated union.
        CasesTable: TyconUnionCases

        /// The ILX data structure representing the discriminated union.
        CompiledRepresentation: cache<AbstractIL.ILX.Types.IlxUnionRef>
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the union cases as a list
    member UnionCasesAsList: UnionCase list

/// Represents a union case in an F# type definition
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type UnionCase =
    {

        /// Data carried by the case.
        FieldTable: TyconRecdFields

        /// Return type constructed by the case. Normally exactly the type of the enclosing type, sometimes an abbreviation of it
        ReturnType: TType

        /// Documentation for the case
        XmlDoc: XmlDoc

        /// XML documentation signature for the case
        mutable XmlDocSig: string

        /// Name/range of the case
        Id: Syntax.Ident

        /// If this field is populated, this is the implementation range for an item in a signature, otherwise it is
        /// the signature range for an item in an implementation
        mutable OtherRangeOpt: (range * bool) option

        ///  Indicates the declared visibility of the union constructor, not taking signatures into account
        Accessibility: Accessibility

        /// Attributes, attached to the generated static method to make instances of the case
        mutable Attribs: Attribs
    }

    /// Get a field of the union case by position
    member GetFieldByIndex: n: int -> RecdField

    /// Get a field of the union case by name
    member GetFieldByName: nm: string -> RecdField option

    override ToString: unit -> string

    /// Get the name of the case in generated IL code.
    /// Note logical names `op_Nil` type `op_ConsCons` become `Empty` type `Cons` respectively.
    /// This is because this is how ILX union code gen expects to see them.
    member CompiledName: string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the definition location of the union case
    member DefinitionRange: range

    /// Get the display name of the union case
    ///
    /// Backticks type parens are added for non-identifiers.
    ///
    /// Note logical names op_Nil type op_ConsCons become ([]) type (::) respectively.
    member DisplayName: string

    /// Get the core of the display name of the union case
    ///
    /// Backticks type parens are not added for non-identifiers.
    ///
    /// Note logical names op_Nil type op_ConsCons become [] type :: respectively.
    member DisplayNameCore: string

    /// Indicates if the union case has no fields
    member IsNullary: bool

    /// Get the logical name of the union case
    member LogicalName: string

    /// Get the declaration location of the union case
    member Range: range

    /// Get the full list of fields of the union case
    member RecdFields: RecdField list

    /// Get the full array of fields of the union case
    member RecdFieldsArray: RecdField[]

    /// Get the signature location of the union case
    member SigRange: range

/// Represents a class, struct, record or exception field in an F# type, exception or union-case definition.
/// This may represent a "field" in either a struct, class, record or union.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type RecdField =
    {

        /// Is the field declared mutable in F#?
        rfield_mutable: bool

        /// Documentation for the field
        rfield_xmldoc: XmlDoc

        /// XML Documentation signature for the field
        mutable rfield_xmldocsig: string

        /// The type of the field, w.r.t. the generic parameters of the enclosing type constructor
        rfield_type: TType

        /// Indicates a static field
        rfield_static: bool

        /// Indicates a volatile field
        rfield_volatile: bool

        /// Indicates a compiler generated field, not visible to Intellisense or name resolution
        rfield_secret: bool

        /// The default initialization info, for static literals
        rfield_const: Const option

        ///  Indicates the declared visibility of the field, not taking signatures into account
        rfield_access: Accessibility

        /// Attributes attached to generated property
        mutable rfield_pattribs: Attribs

        /// Attributes attached to generated field
        mutable rfield_fattribs: Attribs

        /// Name/declaration-location of the field
        rfield_id: Syntax.Ident
        rfield_name_generated: bool

        /// If this field is populated, this is the implementation range for an item in a signature, otherwise it is
        /// the signature range for an item in an implementation
        mutable rfield_other_range: (range * bool) option
    }

    override ToString: unit -> string

    ///  Indicates the declared visibility of the field, not taking signatures into account
    member Accessibility: Accessibility

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the definition location of the field
    member DefinitionRange: range

    /// Name of the field
    member DisplayName: string

    /// Name of the field. For fields this is the same as the logical name.
    member DisplayNameCore: string

    /// Attributes attached to generated field
    member FieldAttribs: Attribs

    /// The type of the field, w.r.t. the generic parameters of the enclosing type constructor
    member FormalType: TType

    /// Name/declaration-location of the field
    member Id: Syntax.Ident

    /// Indicates a compiler generated field, not visible to Intellisense or name resolution
    member IsCompilerGenerated: bool

    /// Is the field declared mutable in F#?
    member IsMutable: bool

    /// Indicates a static field
    member IsStatic: bool

    /// Indicates a volatile field
    member IsVolatile: bool

    /// Indicates if the field is zero-initialized
    member IsZeroInit: bool

    /// The default initialization info, for static literals
    member LiteralValue: Const option

    /// Name of the field
    member LogicalName: string

    /// Attributes attached to generated property
    member PropertyAttribs: Attribs

    /// Get the declaration location of the field
    member Range: range

    /// Get the signature location of the field
    member SigRange: range

    /// XML Documentation signature for the field
    member XmlDoc: XmlDoc

    /// Get or set the XML documentation signature for the field
    member XmlDocSig: string with get, set

/// Represents the implementation of an F# exception definition.
[<NoEquality; NoComparison>]
type ExceptionInfo =

    /// Indicates that an exception is an abbreviation for the given exception
    | TExnAbbrevRepr of TyconRef

    /// Indicates that an exception is shorthand for the given .NET exception type
    | TExnAsmRepr of ILTypeRef

    /// Indicates that an exception carries the given record of values
    | TExnFresh of TyconRecdFields

    /// Indicates that an exception is abstract, i.e. is in a signature file, type we do not know the representation
    | TExnNone

    override ToString: unit -> string

/// Represents the contents of of a module of namespace
[<Sealed; StructuredFormatDisplay("{DebugText}")>]
type ModuleOrNamespaceType =

    new: kind: ModuleOrNamespaceKind * vals: QueueList<Val> * entities: QueueList<Entity> -> ModuleOrNamespaceType

    /// Return a new module or namespace type with an entity added.
    member AddEntity: tycon: Tycon -> ModuleOrNamespaceType

    /// Mutation used during compilation of FSharp.Core.dll
    member AddModuleOrNamespaceByMutation: modul: ModuleOrNamespace -> unit

#if !NO_TYPEPROVIDERS
    /// Mutation used in hosting scenarios to hold the hosted types in this module or namespace
    member AddProvidedTypeEntity: entity: Entity -> unit
#endif

    /// Return a new module or namespace type with a value added.
    member AddVal: vspec: Val -> ModuleOrNamespaceType

    override ToString: unit -> string

    /// Try to find the member with the given linkage key in the given module.
    member TryLinkVal: ccu: CcuThunk * key: ValLinkageFullKey -> Val voption

    /// Get a table of the active patterns defined in this module.
    member ActivePatternElemRefLookupTable: NameMap<ActivePatternElemRef> option ref

    /// Type, mapping mangled name to Tycon, e.g.
    member AllEntities: QueueList<Entity>

    /// Get a table of entities indexed by both logical type compiled names
    member AllEntitiesByCompiledAndLogicalMangledNames: NameMap<Entity>

    /// Get a table of entities indexed by both logical name
    member AllEntitiesByLogicalMangledName: NameMap<Entity>

    /// Values, including members in F# types in this module-or-namespace-fragment.
    member AllValsAndMembers: QueueList<Val>

    /// Compute a table of values type members indexed by logical name.
    member AllValsAndMembersByLogicalNameUncached: MultiMap<string, Val>

    /// Get a table of values type members indexed by partial linkage key, which includes name, the mangled name of the parent type (if any),
    /// type the method argument count (if any).
    member AllValsAndMembersByPartialLinkageKey: MultiMap<ValLinkagePartialKey, Val>

    /// Get a table of values indexed by logical name
    member AllValsByLogicalName: NameMap<Val>

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get a list of F# exception definitions defined within this module, namespace or type.
    member ExceptionDefinitions: Entity list

    /// Get a table of F# exception definitions indexed by demangled name, so 'FailureException' is indexed by 'Failure'
    member ExceptionDefinitionsByDemangledName: NameMap<Tycon>

    /// Get a list of module type namespace definitions defined within this module, namespace or type.
    member ModuleAndNamespaceDefinitions: Entity list

    /// Namespace or module-compiled-as-type?
    member ModuleOrNamespaceKind: ModuleOrNamespaceKind

    /// Get a table of nested module type namespace fragments indexed by demangled name (so 'ListModule' becomes 'List')
    member ModulesAndNamespacesByDemangledName: NameMap<ModuleOrNamespace>

    /// Get a list of type type exception definitions defined within this module, namespace or type.
    member TypeAndExceptionDefinitions: Entity list

    /// Get a list of types defined within this module, namespace or type.
    member TypeDefinitions: Entity list

    /// Get a table of types defined within this module, namespace or type. The
    /// table is indexed by both name and, for generic types, also by mangled name.
    member TypesByAccessNames: LayeredMultiMap<string, Tycon>

    /// Get a table of types defined within this module, namespace or type. The
    /// table is indexed by both name type generic arity. This means that for generic
    /// types "List`1", the entry (List, 1) will be present.
    member TypesByDemangledNameAndArity: LayeredMap<Syntax.PrettyNaming.NameArityPair, Tycon>

    member TypesByMangledName: NameMap<Tycon>

/// Represents a module or namespace definition in the typed AST
type ModuleOrNamespace = Entity

/// Represents a type or exception definition in the typed AST
type Tycon = Entity

/// Represents the constraint on access for a construct
[<StructuralEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type Accessibility =

    /// Indicates the construct can only be accessed from any code in the given type constructor, module or assembly. [] indicates global scope.
    | TAccess of compilationPaths: CompilationPath list

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents less-frequently-required data about a type parameter of type inference variable
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TyparOptionalData =
    {

        /// MUTABILITY: we set the names of generalized inference type parameters to make the look nice for IL code generation
        /// The storage for the IL name for the type parameter.
        mutable typar_il_name: string option

        /// The documentation for the type parameter. Empty for inference variables.
        /// MUTABILITY: for linking when unpickling
        mutable typar_xmldoc: XmlDoc

        /// The inferred constraints for the type parameter or inference variable.
        mutable typar_constraints: TyparConstraint list

        /// The declared attributes of the type parameter. Empty for type inference variables.
        mutable typar_attribs: Attribs
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

type TyparData = Typar

/// A declared generic type/measure parameter, or a type/measure inference variable.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type Typar =
    {

        /// MUTABILITY: we set the names of generalized inference type parameters to make the look nice for IL code generation
        /// The identifier for the type parameter
        mutable typar_id: Syntax.Ident

        /// The flag data for the type parameter
        mutable typar_flags: TyparFlags

        /// The unique stamp of the type parameter
        /// MUTABILITY: for linking when unpickling
        mutable typar_stamp: Stamp

        /// An inferred equivalence for a type inference variable.
        mutable typar_solution: TType option

        /// A cached TAST type used when this type variable is used as type.
        mutable typar_astype: TType

        /// The optional data for the type parameter
        mutable typar_opt_data: TyparOptionalData option
    }

    /// Creates a type variable based on the given data. Only used during unpickling of F# metadata.
    static member New: data: TyparData -> Typar

    /// Creates a type variable that contains empty data, type is not yet linked. Only used during unpickling of F# metadata.
    static member NewUnlinked: unit -> Typar

    /// Links a previously unlinked type variable to the given data. Only used during unpickling of F# metadata.
    member Link: tg: TyparData -> unit

    /// Set the attributes on the type parameter
    member SetAttribs: attribs: Attrib list -> unit

    /// Sets whether the comparison constraint of a type definition depends on this type variable
    member SetComparisonDependsOn: b: bool -> unit

    /// Sets whether a type variable is compiler generated
    member SetCompilerGenerated: b: bool -> unit

    /// Adjusts the constraints associated with a type variable
    member SetConstraints: cs: TyparConstraint list -> unit

    /// Sets whether a type variable is required at runtime
    member SetDynamicReq: b: TyparDynamicReq -> unit

    /// Sets whether the equality constraint of a type definition depends on this type variable
    member SetEqualityDependsOn: b: bool -> unit

    /// Set the IL name of the type parameter
    member SetILName: il_name: string option -> unit

    /// Sets the identifier associated with a type variable
    member SetIdent: id: Syntax.Ident -> unit

    /// Set whether this type parameter is a compat-flex type parameter (i.e. where "expr :> tp" only emits an optional warning)
    member SetIsCompatFlex: b: bool -> unit

    /// Sets the rigidity of a type variable
    member SetRigidity: b: TyparRigidity -> unit

    /// Sets whether a type variable has a static requirement
    member SetStaticReq: b: Syntax.TyparStaticReq -> unit

    override ToString: unit -> string

    /// Links a previously unlinked type variable to the given data. Only used during unpickling of F# metadata.
    member AsType: TType

    /// The declared attributes of the type parameter. Empty for type inference variables type parameters from .NET.
    member Attribs: Attribs

    /// Indicates that whether or not a generic type definition satisfies the comparison constraint is dependent on whether this type variable satisfies the comparison constraint.
    member ComparisonConditionalOn: bool

    /// The inferred constraints for the type inference variable, if any
    member Constraints: TyparConstraint list

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Indicates the display name of a type variable
    member DisplayName: string

    /// Indicates if a type parameter is needed at runtime type may not be eliminated
    member DynamicReq: TyparDynamicReq

    /// Indicates that whether or not a generic type definition satisfies the equality constraint is dependent on whether this type variable satisfies the equality constraint.
    member EqualityConditionalOn: bool

    /// Get the IL name of the type parameter
    member ILName: string option

    /// The identifier for a type parameter definition
    member Id: Syntax.Ident

    /// Indicates that whether this type parameter is a compat-flex type parameter (i.e. where "expr :> tp" only emits an optional warning)
    member IsCompatFlex: bool

    /// Indicates if the type variable is compiler generated, i.e. is an implicit type inference variable
    member IsCompilerGenerated: bool

    /// Indicates whether a type variable is erased in compiled .NET IL code, i.e. whether it is a unit-of-measure variable
    member IsErased: bool

    /// Indicates if the type inference variable was generated after an error when type checking expressions or patterns
    member IsFromError: bool

    /// Indicates if a type variable has been linked. Only used during unpickling of F# metadata.
    member IsLinked: bool

    /// Indicates if a type variable has been solved.
    member IsSolved: bool

    /// Indicates whether a type variable can be instantiated by types or units-of-measure.
    member Kind: TyparKind

    /// The name of the type parameter
    member Name: string

    /// The range of the identifier for the type parameter definition
    member Range: range

    /// Indicates if the type variable can be solved or given new constraints. The status of a type variable
    /// generally always evolves towards being either rigid or solved.
    member Rigidity: TyparRigidity

    /// The inferred equivalence for the type inference variable, if any.
    member Solution: TType option

    /// The unique stamp of the type parameter
    member Stamp: Stamp

    /// Indicates if the type variable has a static "head type" requirement, i.e. ^a variables used in FSharp.Core type member constraints.
    member StaticReq: Syntax.TyparStaticReq

    /// Get the XML documetnation for the type parameter
    member XmlDoc: XmlDoc

/// Represents a constraint on a type parameter or type
[<NoEquality; NoComparison; RequireQualifiedAccess>]
type TyparConstraint =

    /// A constraint that a type is a subtype of the given type
    | CoercesTo of ty: TType * range: range

    /// A constraint for a default value for an inference type variable should it be neither generalized nor solved
    | DefaultsTo of priority: int * ty: TType * range: range

    /// A constraint that a type has a 'null' value
    | SupportsNull of range: range

    /// A constraint that a type has a member with the given signature
    | MayResolveMember of constraintInfo: TraitConstraintInfo * range: range

    /// A constraint that a type is a non-Nullable value type
    /// These are part of .NET's model of generic constraints, type in order to
    /// generate verifiable code we must attach them to F# generalized type variables as well.
    | IsNonNullableStruct of range: range

    /// A constraint that a type is a reference type
    | IsReferenceType of range: range

    /// A constraint that a type is a simple choice between one of the given ground types. Only arises from 'printf' format strings. See format.fs
    | SimpleChoice of tys: TTypes * range: range

    /// A constraint that a type has a parameterless constructor
    | RequiresDefaultConstructor of range: range

    /// A constraint that a type is an enum with the given underlying
    | IsEnum of ty: TType * range: range

    /// A constraint that a type implements IComparable, with special rules for some known structural container types
    | SupportsComparison of range: range

    /// A constraint that a type does not have the Equality(false) attribute, or is not a structural type with this attribute, with special rules for some known structural container types
    | SupportsEquality of range: range

    /// A constraint that a type is a delegate from the given tuple of args to the given return type
    | IsDelegate of aty: TType * bty: TType * range: range

    /// A constraint that a type is .NET unmanaged type
    | IsUnmanaged of range: range

    override ToString: unit -> string

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TraitWitnessInfo =
    | TraitWitnessInfo of TTypes * string * Syntax.SynMemberFlags * TTypes * TType option

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the member name associated with the member constraint.
    member MemberName: string

    /// Get the return type recorded in the member constraint.
    member ReturnType: TType option

/// The specification of a member constraint that must be solved
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TraitConstraintInfo =

    /// Indicates the signature of a member constraint. Contains a mutable solution cell
    /// to store the inferred solution of the constraint.
    | TTrait of
        tys: TTypes *
        memberName: string *
        _memFlags: Syntax.SynMemberFlags *
        argTys: TTypes *
        returnTy: TType option *
        solution: TraitConstraintSln option ref

    override ToString: unit -> string

    /// Get the argument types recorded in the member constraint. This includes the object instance type for
    /// instance members.
    member ArgumentTypes: TTypes

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the member flags associated with the member constraint.
    member MemberFlags: Syntax.SynMemberFlags

    /// Get the member name associated with the member constraint.
    member MemberName: string

    /// Get the return type recorded in the member constraint.
    member ReturnType: TType option

    /// Get or set the solution of the member constraint during inference
    member Solution: TraitConstraintSln option with get, set

    /// Get the key associated with the member constraint.
    member TraitKey: TraitWitnessInfo

/// Represents the solution of a member constraint during inference.
[<NoEquality; NoComparison>]
type TraitConstraintSln =

    /// FSMethSln(ty, vref, minst)
    ///
    /// Indicates a trait is solved by an F# method.
    ///    ty -- the type type its instantiation
    ///    vref -- the method that solves the trait constraint
    ///    minst -- the generic method instantiation
    | FSMethSln of ty: TType * vref: ValRef * minst: TypeInst

    /// FSRecdFieldSln(tinst, rfref, isSetProp)
    ///
    /// Indicates a trait is solved by an F# record field.
    ///    tinst -- the instantiation of the declaring type
    ///    rfref -- the reference to the record field
    ///    isSetProp -- indicates if this is a set of a record field
    | FSRecdFieldSln of tinst: TypeInst * rfref: RecdFieldRef * isSetProp: bool

    /// Indicates a trait is solved by an F# anonymous record field.
    | FSAnonRecdFieldSln of anonInfo: AnonRecdTypeInfo * tinst: TypeInst * index: int

    /// ILMethSln(ty, extOpt, ilMethodRef, minst)
    ///
    /// Indicates a trait is solved by a .NET method.
    ///    ty -- the type type its instantiation
    ///    extOpt -- information about an extension member, if any
    ///    ilMethodRef -- the method that solves the trait constraint
    ///    minst -- the generic method instantiation
    | ILMethSln of ty: TType * extOpt: ILTypeRef option * ilMethodRef: ILMethodRef * minst: TypeInst

    /// ClosedExprSln expr
    ///
    /// Indicates a trait is solved by an erased provided expression
    | ClosedExprSln of expr: Expr

    /// Indicates a trait is solved by a 'fake' instance of an operator, like '+' on integers
    | BuiltInSln

    override ToString: unit -> string

/// The partial information used to index the methods of all those in a ModuleOrNamespace.
[<RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type ValLinkagePartialKey =
    {

        /// The name of the type with which the member is associated. None for non-member values.
        MemberParentMangledName: string option

        /// Indicates if the member is an override.
        MemberIsOverride: bool

        /// Indicates the logical name of the member.
        LogicalName: string

        /// Indicates the total argument count of the member.
        TotalArgCount: int
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// The full information used to identify a specific overloaded method
/// amongst all those in a ModuleOrNamespace.
[<StructuredFormatDisplay("{DebugText}")>]
type ValLinkageFullKey =

    new: partialKey: ValLinkagePartialKey * typeForLinkage: TType option -> ValLinkageFullKey

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The partial information used to index the value in a ModuleOrNamespace.
    member PartialKey: ValLinkagePartialKey

    /// The full type of the value for the purposes of linking. May be None for non-members, since they can't be overloaded.
    member TypeForLinkage: TType option

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ValOptionalData =
    {

        /// MUTABILITY: for unpickle linkage
        mutable val_compiled_name: string option

        /// If this field is populated, this is the implementation range for an item in a signature, otherwise it is
        /// the signature range for an item in an implementation
        mutable val_other_range: (range * bool) option
        mutable val_const: Const option

        /// What is the original, unoptimized, closed-term definition, if any?
        /// Used to implement [<ReflectedDefinition>]
        mutable val_defn: Expr option

        /// Records the "extra information" for a value compiled as a method (rather
        /// than a closure or a local), including argument names, attributes etc.
        mutable val_repr_info: ValReprInfo option

        /// Records the "extra information" for display purposes for expression-level function definitions
        /// that may be compiled as closures (that is are not necessarily compiled as top-level methods).
        mutable val_repr_info_for_display: ValReprInfo option

        /// How visible is this?
        /// MUTABILITY: for unpickle linkage
        mutable val_access: Accessibility

        /// XML documentation attached to a value.
        /// MUTABILITY: for unpickle linkage
        mutable val_xmldoc: XmlDoc

        /// Is the value actually an instance method/property/event that augments
        /// a type, type if so what name does it take in the IL?
        /// MUTABILITY: for unpickle linkage
        mutable val_member_info: ValMemberInfo option
        mutable val_declaring_entity: ParentRef

        /// XML documentation signature for the value
        mutable val_xmldocsig: string

        /// Custom attributes attached to the value. These contain references to other values (i.e. constructors in types). Mutable to fixup
        /// these value references after copying a collection of values.
        mutable val_attribs: Attribs
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

type ValData = Val

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type Val =
    {

        /// Mutable for unpickle linkage
        mutable val_logical_name: string

        /// Mutable for unpickle linkage
        mutable val_range: range
        mutable val_type: TType

        /// Mutable for unpickle linkage
        mutable val_stamp: Stamp

        /// See vflags section further below for encoding/decodings here
        mutable val_flags: ValFlags
        mutable val_opt_data: ValOptionalData option
    }

    /// Create a new value with the given backing data. Only used during unpickling of F# metadata.
    static member New: data: Val -> Val

    static member NewEmptyValOptData: unit -> ValOptionalData

    /// Create a new value with empty, unlinked data. Only used during unpickling of F# metadata.
    static member NewUnlinked: unit -> Val

    /// The name of the method in compiled code (with some exceptions where ilxgen.fs decides not to use a method impl)
    ///   - If this is a property then this is 'get_Foo' or 'set_Foo'
    ///   - If this is an implementation of an abstract slot then this may be a mangled name
    ///   - If this is an extension member then this will be a mangled name
    ///   - If this is an operator then this is 'op_Addition'
    member CompiledName: compilerGlobalState: CompilerGlobalState.CompilerGlobalState option -> string

    /// The full information used to identify a specific overloaded method amongst all those in a ModuleOrNamespace.
    member GetLinkageFullKey: unit -> ValLinkageFullKey

    /// The partial information used to index the methods of all those in a ModuleOrNamespace.
    member GetLinkagePartialKey: unit -> ValLinkagePartialKey

    /// Link a value based on empty, unlinked data to the given data. Only used during unpickling of F# metadata.
    member Link: tg: ValData -> unit

    member SetAttribs: attribs: Attribs -> unit

    /// Set all the data on a value
    member SetData: tg: ValData -> unit

    member SetDeclaringEntity: parent: ParentRef -> unit

    member SetHasBeenReferenced: unit -> unit

    member SetIgnoresByrefScope: unit -> unit

    member SetInlineIfLambda: unit -> unit

    member SetIsCompiledAsStaticPropertyWithoutField: unit -> unit

    member SetIsCompilerGenerated: v: bool -> unit

    member SetIsFixed: unit -> unit

    member SetIsMemberOrModuleBinding: unit -> unit

    member SetLogicalName: nm: string -> unit

    member SetMakesNoCriticalTailcalls: unit -> unit

    member SetMemberInfo: member_info: ValMemberInfo -> unit

    member SetOtherRange: m: (range * bool) -> unit

    member SetType: ty: TType -> unit

    member SetValDefn: val_defn: Expr -> unit

    member SetValRec: b: ValRecursiveScopeInfo -> unit

    member SetValReprInfo: info: ValReprInfo option -> unit

    member SetValReprInfoForDisplay: info: ValReprInfo option -> unit

    override ToString: unit -> string

    /// How visible is this value, function or member?
    member Accessibility: Accessibility

    /// Get the apparent parent entity for the value, i.e. the entity under with which the
    /// value is associated. For extension members this is the nominal type the member extends.
    /// For other values it is just the actual parent.
    member ApparentEnclosingEntity: ParentRef

    /// Get the declared attributes for the value
    member Attribs: Attrib list

    /// Indicates if this is a 'base' or 'this' value?
    member BaseOrThisInfo: ValBaseOrThisInfo

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The parent type or module, if any (None for expression bindings type parameters)
    member DeclaringEntity: ParentRef

    /// Range of the definition (implementation) of the value, used by Visual Studio
    member DefinitionRange: range

    /// The full text for the value to show in error messages type to use in code.
    /// This includes backticks, parens etc.
    ///
    ///   - If this is a property                      --> Foo
    ///   - If this is an implementation of an abstract slot then this is the name of the method implemented by the abstract slot
    ///   - If this is an active pattern               --> (|A|_|)
    ///   - If this is an operator                     --> (+)
    ///   - If this is an identifier needing backticks --> ``A-B``
    ///   - If this is a base value  --> base
    ///   - If this is a value named ``base`` --> ``base``
    member DisplayName: string

    /// The display name of the value or method with operator names decompiled but without backticks etc.
    ///
    /// Note: here "Core" means "without added backticks or parens"
    member DisplayNameCore: string

    /// The display name of the value or method but without operator names decompiled type without backticks etc.
    /// This is very close to LogicalName except that properties have get_ removed.
    ///
    /// Note: here "Core" means "without added backticks or parens"
    /// Note: here "Mangled" means "op_Addition"
    ///
    ///   - If this is a property                      --> Foo
    ///   - If this is an implementation of an abstract slot then this is the name of the method implemented by the abstract slot
    ///   - If this is an active pattern               --> |A|_|
    ///   - If this is an operator                     --> op_Addition
    ///   - If this is an identifier needing backticks --> A-B
    member DisplayNameCoreMangled: string

    /// Get the type of the value including any generic type parameters
    member GeneralizedType: Typars * TType

    /// Indicates if this is ever referenced?
    member HasBeenReferenced: bool

    member HasDeclaringEntity: bool

    member Id: Syntax.Ident

    /// Indicates if the value will ignore byref scoping rules
    member IgnoresByrefScope: bool

    /// Gets the dispatch slots implemented by this method
    member ImplementedSlotSigs: SlotSig list

    /// Get the inline declaration on a parameter or other non-function-declaration value, used for optimization
    member InlineIfLambda: bool

    /// Get the inline declaration on the value
    member InlineInfo: ValInline

    /// Indicates if this is a 'base' value?
    member IsBaseVal: bool

    /// Indicates if this is a compiler-generated class constructor member
    member IsClassConstructor: bool

    /// Indicates if the backing field for a static value is suppressed.
    member IsCompiledAsStaticPropertyWithoutField: bool

    /// Is this represented as a "top level" static binding (i.e. a static field, static member,
    /// instance member), rather than an "inner" binding that may result in a closure.
    ///
    /// This is implied by IsMemberOrModuleBinding, however not vice versa, for two reasons.
    /// Some optimizations mutate this value when they decide to change the representation of a
    /// binding to be IsCompiledAsTopLevel. Second, even immediately after type checking we expect
    /// some non-module, non-member bindings to be marked IsCompiledAsTopLevel, e.g. 'y' in
    /// 'let x = let y = 1 in y + y' (NOTE: check this, don't take it as gospel)
    member IsCompiledAsTopLevel: bool

    /// Indicates if this is something compiled into a module, i.e. a user-defined value, an extension member or a compiler-generated value
    member IsCompiledIntoModule: bool

    /// Indicates whether this value was generated by the compiler.
    ///
    /// Note: this is true for the overrides generated by hash/compare augmentations
    member IsCompilerGenerated: bool

    /// Indicates if this is an F#-defined 'new' constructor member
    member IsConstructor: bool

    /// Indicates if this is a 'this' value for an implicit ctor?
    member IsCtorThisVal: bool

    /// Indicates if this member is an F#-defined dispatch slot.
    member IsDispatchSlot: bool

    /// Indicates if this is an F#-defined extension member
    member IsExtensionMember: bool

    /// Indicates if the value is pinned/fixed
    member IsFixed: bool

    /// Indicates if this is a constructor member generated from the de-sugaring of implicit constructor for a class type?
    member IsIncrClassConstructor: bool

    /// Indicates if this is a member generated from the de-sugaring of 'let' function bindings in the implicit class syntax?
    member IsIncrClassGeneratedMember: bool

    /// Indicates if this is an F#-defined instance member.
    ///
    /// Note, the value may still be (a) an extension member or (b) type abstract slot without
    /// a true body. These cases are often causes of bugs in the compiler.
    member IsInstanceMember: bool

    /// Indicates if this is a member, excluding extension members
    member IsIntrinsicMember: bool

    /// Indicates if a value is linked to backing data yet. Only used during unpickling of F# metadata.
    member IsLinked: bool

    /// Indicates if this is a member
    member IsMember: bool

    /// Is this a member definition or module definition?
    member IsMemberOrModuleBinding: bool

    /// Indicates if this is a 'this' value for a member?
    member IsMemberThisVal: bool

    /// Indicates if this is an F#-defined value in a module, or an extension member, but excluding compiler generated bindings from optimizations
    member IsModuleBinding: bool

    /// Indicates if this is declared 'mutable'
    member IsMutable: bool

    /// Indicates if this value was a member declared 'override' or an implementation of an interface slot
    member IsOverrideOrExplicitImpl: bool

    member IsTypeFunction: bool

    /// The value of a value or member marked with [<LiteralAttribute>]
    member LiteralValue: Const option

    /// The name of the method.
    ///   - If this is a property then this is 'get_Foo' or 'set_Foo'
    ///   - If this is an implementation of an abstract slot then this is the name of the method implemented by the abstract slot
    ///   - If this is an extension member then this will be the simple name
    member LogicalName: string

    /// Indicates if this is inferred to be a method or function that definitely makes no critical tailcalls?
    member MakesNoCriticalTailcalls: bool

    /// Get the apparent parent entity for a member
    member MemberApparentEntity: TyconRef

    /// Is this a member, if so some more data about the member.
    ///
    /// Note, the value may still be (a) an extension member or (b) type abstract slot without
    /// a true body. These cases are often causes of bugs in the compiler.
    member MemberInfo: ValMemberInfo option

    /// Indicates whether the inline declaration for the value indicate that the value must be inlined?
    member MustInline: bool

    /// Get the number of 'this'/'self' object arguments for the member. Instance extension members return '1'.
    member NumObjArgs: int

    /// Indicates if this value allows the use of an explicit type instantiation (i.e. does it itself have explicit type arguments,
    /// or does it have a signature?)
    member PermitsExplicitTypeInstantiation: bool

    /// The name of the property.
    /// - If this is a property then this is 'Foo'
    member PropertyName: string

    /// Get the public path to the value, if any? Should be set if type only if
    /// IsMemberOrModuleBinding is set.
    member PublicPath: ValPublicPath option

    /// The place where the value was defined.
    member Range: range

    /// Get the information about the value used during type inference
    member RecursiveValInfo: ValRecursiveScopeInfo

    /// The quotation expression associated with a value given the [<ReflectedDefinition>] tag
    member ReflectedDefinition: Expr option

    /// Range of the definition (signature) of the value, used by Visual Studio
    member SigRange: range

    /// A unique stamp within the context of this invocation of the compiler process
    member Stamp: Stamp

    /// Get the type of the value after removing any generic type parameters
    member TauType: TType

    /// Get the actual parent entity for the value (a module or a type), i.e. the entity under which the
    /// value will appear in compiled code. For extension members this is the module where the extension member
    /// is declared.
    member TopValDeclaringEntity: EntityRef

    /// Get the generic type parameters for the value
    member Typars: Typars

    /// The type of the value.
    /// May be a TType_forall for a generic value.
    /// May be a type variable or type containing type variables during type inference.
    member Type: TType

    member ValCompiledName: string option

    /// Records the "extra information" for a value compiled as a method.
    ///
    /// This indicates the number of arguments in each position for a curried
    /// functions, type relates to the F# spec for arity analysis.
    /// For module-defined values, the currying is based
    /// on the number of lambdas, type in each position the elements are
    /// based on attempting to deconstruct the type of the argument as a
    /// tuple-type.
    ///
    /// The field is mutable because arities for recursive
    /// values are only inferred after the r.h.s. is analyzed, but the
    /// value itself is created before the r.h.s. is analyzed.
    ///
    /// TLR also sets this for inner bindings that it wants to
    /// represent as "top level" bindings.
    member ValReprInfo: ValReprInfo option

    /// Records the "extra information" for display purposes for expression-level function definitions
    /// that may be compiled as closures (that is are not necessarily compiled as top-level methods).
    member ValReprInfoForDisplay: ValReprInfo option

    /// Get the declared documentation for the value
    member XmlDoc: XmlDoc

    ///Get the signature for the value's XML documentation
    member XmlDocSig: string with get, set

/// Represents the extra information stored for a member
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type ValMemberInfo =
    {

        /// The parent type. For an extension member this is the type being extended
        ApparentEnclosingEntity: TyconRef

        /// Updated with the full implemented slotsig after interface implementation relation is checked
        mutable ImplementedSlotSigs: SlotSig list

        /// Gets updated with 'true' if an abstract slot is implemented in the file being typechecked. Internal only.
        mutable IsImplemented: bool
        MemberFlags: Syntax.SynMemberFlags
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type NonLocalValOrMemberRef =
    {

        /// A reference to the entity containing the value or member. This will always be a non-local reference
        EnclosingEntity: EntityRef

        /// The name of the value, or the full signature of the member
        ItemKey: ValLinkageFullKey
    }

    /// For debugging
    override ToString: unit -> string

    /// Get the name of the assembly referred to
    member AssemblyName: string

    /// Get the thunk for the assembly referred to
    member Ccu: CcuThunk

    /// For debugging
    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents the path information for a reference to a value or member in another assembly, disassociated
/// from any particular reference.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ValPublicPath =
    | ValPubPath of PublicPath * ValLinkageFullKey

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents an index into the namespace/module structure of an assembly
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type NonLocalEntityRef =
    | NonLocalEntityRef of CcuThunk * string[]

    /// Try to find the entity corresponding to the given path in the given CCU
    static member TryDerefEntityPath: ccu: CcuThunk * path: string[] * i: int * entity: Entity -> Entity voption

#if !NO_TYPEPROVIDERS
    /// Try to find the entity corresponding to the given path, using type-providers to link the data
    static member TryDerefEntityPathViaProvidedType:
        ccu: CcuThunk * path: string[] * i: int * entity: Entity -> Entity voption
#endif

    override ToString: unit -> string

    /// Try to link a non-local entity reference to an actual entity
    member TryDeref: canError: bool -> Entity voption

    /// Get the name of the assembly referenced by the nonlocal reference.
    member AssemblyName: string

    /// Get the CCU referenced by the nonlocal reference.
    member Ccu: CcuThunk

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Dereference the nonlocal reference, type raise an error if this fails.
    member Deref: Entity

    member DisplayName: string

    /// Get the all-but-last names of the path of the nonlocal reference.
    member EnclosingMangledPath: string[]

    /// Get the mangled name of the last item in the path of the nonlocal reference.
    member LastItemMangledName: string

    /// Get the details of the module or namespace fragment for the entity referred to by this non-local reference.
    member ModuleOrNamespaceType: ModuleOrNamespaceType

    /// Get the path into the CCU referenced by the nonlocal reference.
    member Path: string[]

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type EntityRef =
    {

        /// Indicates a reference to something bound in this CCU
        mutable binding: NonNullSlot<Entity>

        /// Indicates a reference to something bound in another CCU
        nlr: NonLocalEntityRef
    }

    /// Get a field by index in definition order
    member GetFieldByIndex: n: int -> RecdField

    /// Get a field by name.
    member GetFieldByName: n: string -> RecdField option

    /// Get a union case of a type by name
    member GetUnionCaseByName: n: string -> UnionCase option

    member MakeNestedRecdFieldRef: rf: RecdField -> RecdFieldRef

    member MakeNestedUnionCaseRef: uc: UnionCase -> UnionCaseRef

    /// Resolve the reference
    member private Resolve: canError: bool -> unit

    /// Set the on-demand analysis about whether the entity is assumed to be a readonly struct
    member SetIsAssumedReadOnly: b: bool -> unit

    /// Set the on-demand analysis about whether the entity has the IsByRefLike attribute
    member SetIsByRefLike: b: bool -> unit

    /// Set the on-demand analysis about whether the entity has the IsReadOnly attribute
    member SetIsReadOnly: b: bool -> unit

    override ToString: unit -> string

    /// Get the type parameters for an entity that is a type declaration, otherwise return the empty list.
    ///
    /// Lazy because it may read metadata, must provide a context "range" in case error occurs reading metadata.
    member Typars: m: range -> Typars

    /// Get the value representing the accessibility of an F# type definition or module.
    member Accessibility: Accessibility

    member AllFieldAsRefList: RecdFieldRef list

    /// Get a table of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldTable: TyconRecdFields

    /// Get an array of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldsArray: RecdField[]

    /// Get a list of fields for all the F#-defined record, struct type class fields in this type definition, including
    /// static fields, 'val' declarations type hidden fields from the compilation of implicit class constructions.
    member AllFieldsAsList: RecdField list

    /// Get a list of all instance fields for F#-defined record, struct type class fields in this type definition.
    /// including hidden fields from the compilation of implicit class constructions.
    member AllInstanceFieldsAsList: RecdField list

    /// The F#-defined custom attributes of the entity, if any. If the entity is backed by Abstract IL or provided metadata
    /// then this does not include any attributes from those sources.
    member Attribs: Attribs

    /// Is the destination assembly available?
    member CanDeref: bool

    /// Get a blob of data indicating how this type is nested inside other namespaces, modules type types.
    member CompilationPath: CompilationPath

    /// Get a blob of data indicating how this type is nested inside other namespaces, modules type types.
    member CompilationPathOpt: CompilationPath option

    /// The compiled name of the namespace, module or type, e.g. FSharpList`1, ListModule or FailureException
    member CompiledName: string

    /// Get the cache of the compiled ILTypeRef representation of this module or type.
    member CompiledReprCache: cache<CompiledTypeRepr>

    /// Gets the data indicating the compiled representation of a type or module in terms of Abstract IL data structures.
    member CompiledRepresentation: CompiledTypeRepr

    /// Gets the data indicating the compiled representation of a named type or module in terms of Abstract IL data structures.
    member CompiledRepresentationForNamedType: ILTypeRef

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The implementation definition location of the namespace, module or type
    member DefinitionRange: range

    /// Demangle the module name, if FSharpModuleWithSuffix is used
    member DemangledModuleOrNamespaceName: string

    /// Dereference the TyconRef to a Tycon. Amortize the cost of doing this.
    /// This path should not allocate in the amortized case
    member Deref: Entity

    /// The display name of the namespace, module or type, e.g. List instead of List`1, not including static parameters
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayName: string

    /// The display name of the namespace, module or type, e.g. List instead of List`1, not including static parameters
    ///
    /// No backticks are added for entities with non-identifier names
    member DisplayNameCore: string

    /// The display name of the namespace, module or type, e.g. List instead of List`1, including static parameters
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayNameWithStaticParameters: string

    /// The display name of the namespace, module or type with <_, _, _> added for generic types, including static parameters
    ///
    /// Backticks are added implicitly for entities with non-identifier names
    member DisplayNameWithStaticParametersAndUnderscoreTypars: string

    /// The information about the r.h.s. of an F# exception definition, if any.
    member ExceptionInfo: ExceptionInfo

    /// Get the blob of information associated with an F# object-model type definition, i.e. class, interface, struct etc.
    member FSharpObjectModelTypeInfo: TyconObjModelData

    /// Gets any implicit CompareTo methods added to an F# record, union or struct type definition.
    member GeneratedCompareToValues: (ValRef * ValRef) option

    /// Gets any implicit CompareTo (with comparer argument) methods added to an F# record, union or struct type definition.
    member GeneratedCompareToWithComparerValues: ValRef option

    /// Gets any implicit hash/equals methods added to an F# record, union or struct type definition.
    member GeneratedHashAndEqualsValues: (ValRef * ValRef) option

    /// Gets any implicit hash/equals (with comparer argument) methods added to an F# record, union or struct type definition.
    member GeneratedHashAndEqualsWithComparerValues: (ValRef * ValRef * ValRef) option

    /// Indicates if we have pre-determined that a type definition has a self-referential constructor using 'as x'
    member HasSelfReferentialConstructor: bool

    /// Get the Abstract IL scope, nesting type metadata for this
    /// type definition, assuming it is backed by Abstract IL metadata.
    member ILTyconInfo: TILObjectReprData

    /// Get the Abstract IL metadata for this type definition, assuming it is backed by Abstract IL metadata.
    member ILTyconRawMetadata: ILTypeDef

    /// The identifier at the point of declaration of the type definition.
    member Id: Syntax.Ident

    /// Gets the immediate interface types of an F# type definition. Further interfaces may be supported through class type interface inheritance.
    member ImmediateInterfaceTypesOfFSharpTycon: TType list

    /// Gets the immediate interface definitions of an F# type definition. Further interfaces may be supported through class type interface inheritance.
    member ImmediateInterfacesOfFSharpTycon: (TType * bool * range) list

    /// Indicates if this is an F# type definition which is one of the special types in FSharp.Core.dll which uses
    /// an assembly-code representation for the type, e.g. the primitive array type constructor.
    member IsAsmReprTycon: bool

    /// Indicates if this is an enum type definition
    member IsEnumTycon: bool

    /// Indicates if the entity is erased, either a measure definition, or an erased provided type definition
    member IsErased: bool

    /// Indicates if this is an F#-defined delegate type definition
    member IsFSharpDelegateTycon: bool

    /// Indicates if this is an F#-defined enum type definition
    member IsFSharpEnumTycon: bool

    /// Indicates if the entity represents an F# exception declaration.
    member IsFSharpException: bool

    /// Indicates if this is an F#-defined interface type definition
    member IsFSharpInterfaceTycon: bool

    /// Indicates if this is an F# type definition whose r.h.s. is known to be some kind of F# object model definition
    member IsFSharpObjectModelTycon: bool

    /// Indicates if this is an F#-defined struct or enum type definition, i.e. a value type definition
    member IsFSharpStructOrEnumTycon: bool

    /// Indicates if this is an F# type definition whose r.h.s. definition is unknown (i.e. a traditional ML 'abstract' type in a signature,
    /// which in F# is called a 'unknown representation' type).
    member IsHiddenReprTycon: bool

    /// Indicates if this is a .NET-defined enum type definition
    member IsILEnumTycon: bool

    /// Indicates if this is a .NET-defined struct or enum type definition, i.e. a value type definition
    member IsILStructOrEnumTycon: bool

    /// Indicate if this is a type definition backed by Abstract IL metadata.
    member IsILTycon: bool

    /// Indicates if the reference is a local reference
    member IsLocalRef: bool

    /// Indicates if this is an F# type definition which is one of the special types in FSharp.Core.dll like 'float<_>' which
    /// defines a measure type with a relation to an existing non-measure type as a representation.
    member IsMeasureableReprTycon: bool

    /// Indicates if the entity is an F# module definition
    member IsModule: bool

    /// Indicates the "tycon blob" is actually a module
    member IsModuleOrNamespace: bool

    /// Indicates if the entity is a namespace
    member IsNamespace: bool

    /// Indicates the type prefers the "tycon<a, b>" syntax for display etc.
    member IsPrefixDisplay: bool

#if !NO_TYPEPROVIDERS
    /// Indicates if the entity is a provided namespace fragment
    member IsProvided: bool

    /// Indicates if the entity is an erased provided type definition
    member IsProvidedErasedTycon: bool

    /// Indicates if the entity is a generated provided type definition, i.e. not erased.
    member IsProvidedGeneratedTycon: bool

    /// Indicates if the entity is a provided namespace fragment
    member IsProvidedNamespace: bool
#endif

    /// Indicates if this is an F# type definition whose r.h.s. is known to be a record type definition.
    member IsRecordTycon: bool

    /// Indicates if the reference has been resolved
    member IsResolved: bool

#if !NO_TYPEPROVIDERS
    /// Indicates if the entity is an erased provided type definition that incorporates a static instantiation (type therefore in some sense compiler generated)
    member IsStaticInstantiationTycon: bool
#endif

    /// Indicates if this is a struct or enum type definition, i.e. a value type definition
    member IsStructOrEnumTycon: bool

    /// Indicates if this entity is an F# type abbreviation definition
    member IsTypeAbbrev: bool

    /// Indicate if this is a type whose r.h.s. is known to be a union type definition.
    member IsUnionTycon: bool

    /// The name of the namespace, module or type, possibly with mangling, e.g. List`1, List or FailureException
    member LogicalName: string

    /// Gets all immediate members of an F# type definition keyed by name, including compiler-generated ones.
    /// Note: result is a indexed table, type for each name the results are in reverse declaration order
    member MembersOfFSharpTyconByName: NameMultiMap<ValRef>

    /// Gets the immediate members of an F# type definition, excluding compiler-generated ones.
    /// Note: result is alphabetically sorted, then for each name the results are in declaration order
    member MembersOfFSharpTyconSorted: ValRef list

    /// The logical contents of the entity when it is a module or namespace fragment.
    member ModuleOrNamespaceType: ModuleOrNamespaceType

    /// Indicates if we have pre-determined that a type definition has a default constructor.
    member PreEstablishedHasDefaultConstructor: bool

    /// Get a blob of data indicating how this type is nested in other namespaces, modules or types.
    member PublicPath: PublicPath option

    /// The code location where the module, namespace or type is defined.
    member Range: range

    /// The resolved target of the reference
    member ResolvedTarget: NonNullSlot<Entity>

    /// The signature definition location of the namespace, module or type
    member SigRange: range

    /// A unique stamp for this module, namespace or type definition within the context of this compilation.
    /// Note that because of signatures, there are situations where in a single compilation the "same"
    /// module, namespace or type may have two distinct Entity objects that have distinct stamps.
    member Stamp: Stamp

    /// Get a list of all fields for F#-defined record, struct type class fields in this type definition,
    /// including static fields, but excluding compiler-generate fields.
    member TrueFieldsAsList: RecdField list

    /// Get a list of all instance fields for F#-defined record, struct type class fields in this type definition,
    /// excluding compiler-generate fields.
    member TrueInstanceFieldsAsList: RecdField list

    member TrueInstanceFieldsAsRefList: RecdFieldRef list

    /// Dereference the TyconRef to a Tycon option.
    member TryDeref: NonNullSlot<Entity> voption

    /// The on-demand analysis about whether the entity is assumed to be a readonly struct
    member TryIsAssumedReadOnly: bool voption

    /// The on-demand analysis about whether the entity has the IsByRefLike attribute
    member TryIsByRefLike: bool voption

    /// The on-demand analysis about whether the entity has the IsReadOnly attribute
    member TryIsReadOnly: bool voption

    /// Get the type parameters for an entity that is a type declaration, otherwise return the empty list.
    member TyparsNoRange: Typars

    /// Indicates if this entity is an F# type abbreviation definition
    member TypeAbbrev: TType option

    /// The logical contents of the entity when it is a type definition.
    member TypeContents: TyconAugmentation

    /// The kind of the type definition - is it a measure definition or a type definition?
    member TypeOrMeasureKind: TyparKind

    /// Get the value representing the accessibility of the r.h.s. of an F# type definition.
    member TypeReprAccessibility: Accessibility

    /// The information about the r.h.s. of a type definition, if any. For example, the r.h.s. of a union or record type.
    member TypeReprInfo: TyconRepresentation

    /// Get the union cases for a type, if any
    member UnionCasesArray: UnionCase[]

    /// Get the union cases for a type, if any, as a list
    member UnionCasesAsList: UnionCase list

    member UnionCasesAsRefList: UnionCaseRef list

    /// Get the union cases type other union-type information for a type, if any
    member UnionTypeInfo: TyconUnionData voption

    /// The XML documentation of the entity, if any. If the entity is backed by provided metadata
    /// then this _does_ include this documentation. If the entity is backed by Abstract IL metadata
    /// or comes from another F# assembly then it does not (because the documentation will get read from
    /// an XML file).
    member XmlDoc: XmlDoc

    /// The XML documentation sig-string of the entity, if any, to use to lookup an .xml doc file. This also acts
    /// as a cache for this sig-string computation.
    member XmlDocSig: string

/// Represents a module-or-namespace reference in the typed abstract syntax.
type ModuleOrNamespaceRef = EntityRef

/// Represents a type definition reference in the typed abstract syntax.
type TyconRef = EntityRef

/// References are either local or nonlocal
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ValRef =
    {

        /// Indicates a reference to something bound in this CCU
        mutable binding: NonNullSlot<Val>

        /// Indicates a reference to something bound in another CCU
        nlr: NonLocalValOrMemberRef
    }

    override ToString: unit -> string

    /// Get the value representing the accessibility of an F# type definition or module.
    member Accessibility: Accessibility

    /// Get the apparent parent entity for the value, i.e. the entity under with which the
    /// value is associated. For extension members this is the nominal type the member extends.
    /// For other values it is just the actual parent.
    member ApparentEnclosingEntity: ParentRef

    /// Get the declared attributes for the value
    member Attribs: Attrib list

    /// Indicates if this is a 'base' or 'this' value?
    member BaseOrThisInfo: ValBaseOrThisInfo

    /// The name of the method in compiled code (with some exceptions where ilxgen.fs decides not to use a method impl)
    member CompiledName: (CompilerGlobalState.CompilerGlobalState option -> string)

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The parent type or module, if any (None for expression bindings type parameters)
    member DeclaringEntity: ParentRef

    member DefinitionRange: range

    /// Dereference the ValRef to a Val.
    member Deref: Val

    member DisplayName: string

    member DisplayNameCore: string

    member DisplayNameCoreMangled: string

    /// Get the type of the value including any generic type parameters
    member GeneralizedType: Typars * TType

    member HasDeclaringEntity: bool

    member Id: Syntax.Ident

    /// Gets the dispatch slots implemented by this method
    member ImplementedSlotSigs: SlotSig list

    /// Get the inline declaration on a parameter or other non-function-declaration value, used for optimization
    member InlineIfLambda: bool

    /// Get the inline declaration on the value
    member InlineInfo: ValInline

    /// Indicates if this is a 'base' value?
    member IsBaseVal: bool

    /// Is this represented as a "top level" static binding (i.e. a static field, static member,
    /// instance member), rather than an "inner" binding that may result in a closure.
    member IsCompiledAsTopLevel: bool

    /// Indicates whether this value was generated by the compiler.
    ///
    /// Note: this is true for the overrides generated by hash/compare augmentations
    member IsCompilerGenerated: bool

    /// Indicates if this is an F#-defined 'new' constructor member
    member IsConstructor: bool

    /// Indicates if this is a 'this' value for an implicit ctor?
    member IsCtorThisVal: bool

    /// Indicates if this member is an F#-defined dispatch slot.
    member IsDispatchSlot: bool

    /// Indicates if this is an F#-defined extension member
    member IsExtensionMember: bool

    /// Indicates if this is a constructor member generated from the de-sugaring of implicit constructor for a class type?
    member IsIncrClassConstructor: bool

    /// Indicates if this is a member generated from the de-sugaring of 'let' function bindings in the implicit class syntax?
    member IsIncrClassGeneratedMember: bool

    /// Indicates if this is an F#-defined instance member.
    ///
    /// Note, the value may still be (a) an extension member or (b) type abstract slot without
    /// a true body. These cases are often causes of bugs in the compiler.
    member IsInstanceMember: bool

    member IsLocalRef: bool

    /// Indicates if this is a member
    member IsMember: bool

    /// Is this a member definition or module definition?
    member IsMemberOrModuleBinding: bool

    /// Indicates if this is a 'this' value for a member?
    member IsMemberThisVal: bool

    /// Indicates if this is an F#-defined value in a module, or an extension member, but excluding compiler generated bindings from optimizations
    member IsModuleBinding: bool

    /// Indicates if this value is declared 'mutable'
    member IsMutable: bool

    /// Indicates if this value was a member declared 'override' or an implementation of an interface slot
    member IsOverrideOrExplicitImpl: bool

    /// Indicates whether this value represents a property getter.
    member IsPropertyGetterMethod: bool

    /// Indicates whether this value represents a property setter.
    member IsPropertySetterMethod: bool

    member IsResolved: bool

    ///  Indicates if this value was declared to be a type function, e.g. "let f<'a> = typeof<'a>"
    member IsTypeFunction: bool

    /// The value of a value or member marked with [<LiteralAttribute>]
    member LiteralValue: Const option

    member LogicalName: string

    /// Indicates if this is inferred to be a method or function that definitely makes no critical tailcalls?
    member MakesNoCriticalTailcalls: bool

    /// Get the apparent parent entity for a member
    member MemberApparentEntity: TyconRef

    /// Is this a member, if so some more data about the member.
    member MemberInfo: ValMemberInfo option

    /// Indicates whether the inline declaration for the value indicate that the value must be inlined?
    member MustInline: bool

    /// Get the number of 'this'/'self' object arguments for the member. Instance extension members return '1'.
    member NumObjArgs: int

    /// Indicates if this value allows the use of an explicit type instantiation (i.e. does it itself have explicit type arguments,
    /// or does it have a signature?)
    member PermitsExplicitTypeInstantiation: bool

    /// Get the name of the value, assuming it is compiled as a property.
    ///   - If this is a property then this is 'Foo'
    ///   - If this is an implementation of an abstract slot then this is the name of the property implemented by the abstract slot
    member PropertyName: string

    /// Get the public path to the value, if any? Should be set if type only if
    /// IsMemberOrModuleBinding is set.
    member PublicPath: ValPublicPath option

    member Range: range

    /// Get the information about a recursive value used during type inference
    member RecursiveValInfo: ValRecursiveScopeInfo

    /// The quotation expression associated with a value given the [<ReflectedDefinition>] tag
    member ReflectedDefinition: Expr option

    member ResolvedTarget: NonNullSlot<Val>

    member SigRange: range

    /// A unique stamp within the context of this invocation of the compiler process
    member Stamp: Stamp

    /// Get the type of the value after removing any generic type parameters
    member TauType: TType

    /// Get the actual parent entity for the value (a module or a type), i.e. the entity under which the
    /// value will appear in compiled code. For extension members this is the module where the extension member
    /// is declared.
    member TopValDeclaringEntity: EntityRef

    /// Dereference the ValRef to a Val option.
    member TryDeref: Val voption

    member Typars: Typars

    /// The type of the value. May be a TType_forall for a generic value.
    /// May be a type variable or type containing type variables during type inference.
    member Type: TType

    /// Records the "extra information" for a value compiled as a method.
    ///
    /// This indicates the number of arguments in each position for a curried function.
    member ValReprInfo: ValReprInfo option

    /// Get the declared documentation for the value
    member XmlDoc: XmlDoc

    /// Get or set the signature for the value's XML documentation
    member XmlDocSig: string

/// Represents a reference to a case of a union type
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type UnionCaseRef =
    | UnionCaseRef of TyconRef * string

    /// Get a field of the union case by index
    member FieldByIndex: n: int -> RecdField

    override ToString: unit -> string

    /// Get the fields of the union case
    member AllFieldsAsList: RecdField list

    /// Get the attributes associated with the union case
    member Attribs: Attribs

    /// Get the name of this union case
    member CaseName: string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the definition range of the union case
    member DefinitionRange: range

    /// Get the index of the union case amongst the cases
    member Index: int

    /// Get the range of the union case
    member Range: range

    /// Get the resulting type of the union case
    member ReturnType: TType

    /// Get the signature range of the union case
    member SigRange: range

    /// Try to dereference the reference
    member TryUnionCase: UnionCase voption

    /// Get the Entity for the type containing this union case
    member Tycon: Entity

    /// Get a reference to the type containing this union case
    member TyconRef: TyconRef

    /// Dereference the reference to the union case
    member UnionCase: UnionCase

/// Represents a reference to a field in a record, class or struct
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type RecdFieldRef =
    | RecdFieldRef of tcref: TyconRef * id: string

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the definition range of the record field
    member DefinitionRange: range

    /// Get the name of the field, with backticks added for non-identifier names
    member DisplayName: string

    /// Get the name of the field
    member FieldName: string

    member Index: int

    /// Get the attributes associated with the compiled property of the record field
    member PropertyAttribs: Attribs

    /// Get the declaration range of the record field
    member Range: range

    /// Dereference the reference
    member RecdField: RecdField

    /// Get the signature range of the record field
    member SigRange: range

    /// Try to dereference the reference
    member TryRecdField: RecdField voption

    /// Get the Entity for the type containing this union case
    member Tycon: Entity

    /// Get a reference to the type containing this union case
    member TyconRef: TyconRef

/// Represents a type in the typed abstract syntax
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type TType =

    /// Indicates the type is a universal type, only used for types of values type members
    | TType_forall of typars: Typars * bodyTy: TType

    /// Indicates the type is built from a named type type a number of type arguments.
    ///
    /// 'flags' is a placeholder for future features, in particular nullness analysis
    | TType_app of tyconRef: TyconRef * typeInstantiation: TypeInst * flags: byte

    /// Indicates the type is an anonymous record type whose compiled representation is located in the given assembly
    | TType_anon of anonInfo: AnonRecdTypeInfo * tys: TType list

    /// Indicates the type is a tuple type. elementTypes must be of length 2 or greater.
    | TType_tuple of tupInfo: TupInfo * elementTypes: TTypes

    /// Indicates the type is a function type.
    ///
    /// 'flags' is a placeholder for future features, in particular nullness analysis.
    | TType_fun of domainType: TType * rangeType: TType * flags: byte

    /// Indicates the type is a non-F#-visible type representing a "proof" that a union value belongs to a particular union case
    /// These types are not user-visible type will never appear as an inferred type. They are the types given to
    /// the temporaries arising out of pattern matching on union values.
    | TType_ucase of unionCaseRef: UnionCaseRef * typeInstantiation: TypeInst

    /// Indicates the type is a variable type, whether declared, generalized or an inference type parameter
    ///
    /// 'flags' is a placeholder for future features, in particular nullness analysis
    | TType_var of typar: Typar * flags: byte

    /// Indicates the type is a unit-of-measure expression being used as an argument to a type or member
    | TType_measure of measure: Measure

    /// For now, used only as a discriminant in error message.
    /// See https://github.com/dotnet/fsharp/issues/2561
    member GetAssemblyName: unit -> string

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

type TypeInst = TType list

type TTypes = TType list

/// Represents the information identifying an anonymous record
[<RequireQualifiedAccess>]
type AnonRecdTypeInfo =
    { mutable Assembly: CcuThunk
      mutable TupInfo: TupInfo
      mutable SortedIds: Syntax.Ident[]
      mutable Stamp: Stamp
      mutable SortedNames: string[] }

    /// Create an AnonRecdTypeInfo from the basic data
    static member Create: ccu: CcuThunk * tupInfo: TupInfo * ids: Syntax.Ident[] -> AnonRecdTypeInfo

    static member NewUnlinked: unit -> AnonRecdTypeInfo

    member Link: d: AnonRecdTypeInfo -> unit

    /// Get the ILTypeRef for the generated type implied by the anonymous type
    member ILTypeRef: ILTypeRef

    member IsLinked: bool

[<RequireQualifiedAccess>]
type TupInfo =

    /// Some constant, e.g. true or false for tupInfo
    | Const of bool

/// Represents a unit of measure in the typed AST
[<RequireQualifiedAccess>]
type Measure =

    /// A variable unit-of-measure
    | Var of typar: Typar

    /// A constant, leaf unit-of-measure such as 'kg' or 'm'
    | Con of tyconRef: TyconRef

    /// A product of two units of measure
    | Prod of measure1: Measure * measure2: Measure

    /// An inverse of a units of measure expression
    | Inv of measure: Measure

    /// The unit of measure '1', e.g. float = float<1>
    | One

    /// Raising a measure to a rational power
    | RationalPower of measure: Measure * power: Rational

    override ToString: unit -> string

type Attribs = Attrib list

[<NoEquality; NoComparison>]
type AttribKind =

    /// Indicates an attribute refers to a type defined in an imported .NET assembly
    | ILAttrib of ilMethodRef: ILMethodRef

    /// Indicates an attribute refers to a type defined in an imported F# assembly
    | FSAttrib of valRef: ValRef

    override ToString: unit -> string

/// Attrib(tyconRef, kind, unnamedArgs, propVal, appliedToAGetterOrSetter, targetsOpt, range)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type Attrib =
    | Attrib of
        tyconRef: TyconRef *
        kind: AttribKind *
        unnamedArgs: AttribExpr list *
        propVal: AttribNamedArg list *
        appliedToAGetterOrSetter: bool *
        targetsOpt: AttributeTargets option *
        range: range

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    member Range: range

    member TyconRef: TyconRef

/// We keep both source expression type evaluated expression around to help intellisense type signature printing
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type AttribExpr =

    /// AttribExpr(source, evaluated)
    | AttribExpr of source: Expr * evaluated: Expr

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// AttribNamedArg(name, type, isField, value)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type AttribNamedArg =
    | AttribNamedArg of (string * TType * bool * AttribExpr)

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Constants in expressions
[<RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type Const =
    | Bool of bool
    | SByte of sbyte
    | Byte of byte
    | Int16 of int16
    | UInt16 of uint16
    | Int32 of int32
    | UInt32 of uint32
    | Int64 of int64
    | UInt64 of uint64
    | IntPtr of int64
    | UIntPtr of uint64
    | Single of single
    | Double of double
    | Char of char
    | String of string
    | Decimal of Decimal
    | Unit
    | Zero

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Decision trees. Pattern matching has been compiled down to
/// a decision tree by this point. The right-hand-sides (actions) of
/// a decision tree by this point. The right-hand-sides (actions) of
/// the decision tree are labelled by integers that are unique for that
/// particular tree.
[<NoEquality; NoComparison>]
type DecisionTree =

    /// TDSwitch(input, cases, default, range)
    ///
    /// Indicates a decision point in a decision tree.
    ///    input -- The expression being tested. If switching over a struct union this
    ///             must be the address of the expression being tested.
    ///    cases -- The list of tests type their subsequent decision trees
    ///    default -- The default decision tree, if any
    ///    range -- (precise documentation needed)
    | TDSwitch of input: Expr * cases: DecisionTreeCase list * defaultOpt: DecisionTree option * range: range

    /// TDSuccess(results, targets)
    ///
    /// Indicates the decision tree has terminated with success, transferring control to the given target with the given parameters.
    ///    results -- the expressions to be bound to the variables at the target
    ///    target -- the target number for the continuation
    | TDSuccess of results: Exprs * targetNum: int

    /// TDBind(binding, body)
    ///
    /// Bind the given value through the remaining cases of the dtree.
    /// These arise from active patterns type some optimizations to prevent
    /// repeated computations in decision trees.
    ///    binding -- the value type the expression it is bound to
    ///    body -- the rest of the decision tree
    | TDBind of binding: Binding * body: DecisionTree

    override ToString: unit -> string

/// Represents a test type a subsequent decision tree
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type DecisionTreeCase =
    | TCase of discriminator: DecisionTreeTest * caseTree: DecisionTree

    override ToString: unit -> string

    /// Get the decision tree or a successful test
    member CaseTree: DecisionTree

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the discriminator associated with the case
    member Discriminator: DecisionTreeTest

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type DecisionTreeTest =

    /// Test if the input to a decision tree matches the given union case
    | UnionCase of caseRef: UnionCaseRef * tinst: TypeInst

    /// Test if the input to a decision tree is an array of the given length
    | ArrayLength of length: int * ty: TType

    /// Test if the input to a decision tree is the given constant value
    | Const of value: Const

    /// Test if the input to a decision tree is null
    | IsNull

    /// IsInst(source, target)
    ///
    /// Test if the input to a decision tree is an instance of the given type
    | IsInst of source: TType * target: TType

    /// Test.ActivePatternCase(activePatExpr, activePatResTys, isStructRetTy, activePatIdentity, idx, activePatInfo)
    ///
    /// Run the active pattern type bind a successful result to a
    /// variable in the remaining tree.
    ///     activePatExpr -- The active pattern function being called, perhaps applied to some active pattern parameters.
    ///     activePatResTys -- The result types (case types) of the active pattern.
    ///     isStructRetTy -- Is the active pattern a struct return
    ///     activePatIdentity -- The value type the types it is applied to. If there are any active pattern parameters then this is empty.
    ///     idx -- The case number of the active pattern which the test relates to.
    ///     activePatternInfo -- The extracted info for the active pattern.
    | ActivePatternCase of
        activePatExpr: Expr *
        activePatResTys: TTypes *
        isStructRetTy: bool *
        activePatIdentity: (ValRef * TypeInst) option *
        idx: int *
        activePatternInfo: Syntax.PrettyNaming.ActivePatternInfo

    /// Used in error recovery
    | Error of range: range

    override ToString: unit -> string

/// A target of a decision tree. Can be thought of as a little function, though is compiled as a local block.
///   -- boundVals - The values bound at the target, matching the valuesin the TDSuccess
///   -- targetExpr - The expression to evaluate if we branch to the target
///   -- debugPoint - The debug point for the target
///   -- isStateVarFlags - Indicates which, if any, of the values are repesents as state machine variables
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type DecisionTreeTarget =
    | TTarget of boundVals: Val list * targetExpr: Expr * isStateVarFlags: bool list option

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    member TargetExpression: Expr

/// A collection of simultaneous bindings
type Bindings = Binding list

/// A binding of a variable to an expression, as in a `let` binding or similar
///  -- val: The value being bound
///  -- expr: The expression to execute to get the value
///  -- debugPoint: The debug point for the binding
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type Binding =
    | TBind of var: Val * expr: Expr * debugPoint: Syntax.DebugPointAtBinding

    override ToString: unit -> string

    /// The information about whether to emit a sequence point for the binding
    member DebugPoint: Syntax.DebugPointAtBinding

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The expression the value is being bound to
    member Expr: Expr

    /// The value being bound
    member Var: Val

/// Represents a reference to an active pattern element. The
/// integer indicates which choice in the target set is being selected by this item.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ActivePatternElemRef =
    | APElemRef of
        activePatternInfo: Syntax.PrettyNaming.ActivePatternInfo *
        activePatternVal: ValRef *
        caseIndex: int *
        isStructRetTy: bool

    override ToString: unit -> string

    /// Get the full information about the active pattern being referred to
    member ActivePatternInfo: Syntax.PrettyNaming.ActivePatternInfo

    /// Get a reference to the value for the active pattern being referred to
    member ActivePatternVal: ValRef

    /// Get the index of the active pattern element within the overall active pattern
    member CaseIndex: int

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get a reference to the value for the active pattern being referred to
    member IsStructReturn: bool

/// Records the "extra information" for a value compiled as a method (rather
/// than a closure or a local), including argument names, attributes etc.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ValReprInfo =

    /// ValReprInfo (typars, args, result)
    | ValReprInfo of typars: TyparReprInfo list * args: ArgReprInfo list list * result: ArgReprInfo

    override ToString: unit -> string

    /// Get the extra information about the arguments for the value
    member ArgInfos: ArgReprInfo list list

    member ArgNames: string list

    /// Get the number of tupled arguments in each curried argument position
    member AritiesOfArgs: int list

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Indicates if the value has no arguments - neither type parameters nor value arguments
    member HasNoArgs: bool

    /// Get the kind of each type parameter
    member KindsOfTypars: TyparKind list

    /// Get the number of curried arguments of the value
    member NumCurriedArgs: int

    /// Get the number of type parameters of the value
    member NumTypars: int

    /// Get the total number of arguments
    member TotalArgCount: int

/// Records the "extra information" for an argument compiled as a real
/// method argument, specifically the argument name type attributes.
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type ArgReprInfo =
    {

        /// The attributes for the argument
        mutable Attribs: Attribs

        /// The name for the argument at this position, if any
        mutable Name: Syntax.Ident option
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Records the extra metadata stored about typars for type parameters
/// compiled as "real" IL type parameters, specifically for values with
/// ValReprInfo. Any information here is propagated from signature through
/// to the compiled code.
type TyparReprInfo = TyparReprInfo of Syntax.Ident * TyparKind

type Typars = Typar list

type Exprs = Expr list

type Vals = Val list

/// Represents an expression in the typed abstract syntax
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type Expr =

    /// A constant expression.
    | Const of value: Const * range: range * constType: TType

    /// Reference a value. The flag is only relevant if the value is an object model member
    /// type indicates base calls type special uses of object constructors.
    | Val of valRef: ValRef * flags: ValUseFlag * range: range

    /// Sequence expressions, used for "a;b", "let a = e in b;a" type "a then b" (the last an OO constructor).
    | Sequential of expr1: Expr * expr2: Expr * kind: SequentialOpKind * range: range

    /// Lambda expressions.
    /// Why multiple vspecs? A Expr.Lambda taking multiple arguments really accepts a tuple.
    /// But it is in a convenient form to be compile accepting multiple
    /// arguments, e.g. if compiled as a toplevel static method.
    | Lambda of
        unique: CompilerGlobalState.Unique *
        ctorThisValOpt: Val option *
        baseValOpt: Val option *
        valParams: Val list *
        bodyExpr: Expr *
        range: Text.range *
        overallType: TType

    /// Type lambdas. These are used for the r.h.s. of polymorphic 'let' bindings type
    /// for expressions that implement first-class polymorphic values.
    | TyLambda of
        unique: CompilerGlobalState.Unique *
        typeParams: Typars *
        bodyExpr: Expr *
        range: Text.range *
        overallType: TType

    /// Applications.
    /// Applications combine type type term applications, type are normalized so
    /// that sequential applications are combined, so "(f x y)" becomes "f [[x];[y]]".
    /// The type attached to the function is the formal function type, used to ensure we don't build application
    /// nodes that over-apply when instantiating at function types.
    | App of funcExpr: Expr * formalType: TType * typeArgs: TypeInst * args: Exprs * range: Text.range

    /// Bind a recursive set of values.
    | LetRec of bindings: Bindings * bodyExpr: Expr * range: Text.range * frees: FreeVarsCache

    /// Bind a value.
    | Let of binding: Binding * bodyExpr: Expr * range: Text.range * frees: FreeVarsCache
    | Obj of
        unique: CompilerGlobalState.Unique *
        objTy: TType *
        baseVal: Val option *
        ctorCall: Expr *
        overrides: ObjExprMethod list *
        interfaceImpls: (TType * ObjExprMethod list) list *
        range: Text.range

    /// Matches are a more complicated form of "let" with multiple possible destinations
    /// type possibly multiple ways to get to each destination.
    /// The first range is that of the expression being matched, which is used
    /// as the range for all the decision making type binding that happens during the decision tree
    /// execution.
    | Match of
        debugPoint: Syntax.DebugPointAtBinding *
        inputRange: Text.range *
        decision: DecisionTree *
        targets: DecisionTreeTarget array *
        fullRange: Text.range *
        exprType: TType

    /// If we statically know some information then in many cases we can use a more optimized expression
    /// This is primarily used by terms in the standard library, particularly those implementing overloaded
    /// operators.
    | StaticOptimization of conditions: StaticOptimization list * expr: Expr * alternativeExpr: Expr * range: Text.range

    /// An intrinsic applied to some (strictly evaluated) arguments
    /// A few of intrinsics (TOp_try, TOp.While, TOp.IntegerForLoop) expect arguments kept in a normal form involving lambdas
    | Op of op: TOp * typeArgs: TypeInst * args: Exprs * range: Text.range

    /// Indicates the expression is a quoted expression tree.
    ///
    | Quote of
        quotedExpr: Expr *
        quotationInfo: ((ILTypeRef list * TTypes * Exprs * QuotationPickler.ExprData) * (ILTypeRef list * TTypes * Exprs * QuotationPickler.ExprData)) option ref *
        isFromQueryExpression: bool *
        range: Text.range *
        quotedType: TType

    /// Used in quotation generation to indicate a witness argument, spliced into a quotation literal.
    ///
    /// For example:
    ///
    ///     let inline f x = <@ sin x @>
    ///
    /// needs to pass a witness argument to `sin x`, captured from the surrounding context, for the witness-passing
    /// version of the code.  Thus the QuotationTranslation type IlxGen makes the generated code as follows:
    ///
    ///  f(x) { return Deserialize(<@ sin _spliceHole @>, [| x |]) }
    ///
    ///  f$W(witnessForSin, x) { return Deserialize(<@ sin$W _spliceHole1 _spliceHole2 @>, [| WitnessArg(witnessForSin), x |]) }
    ///
    /// where _spliceHole1 will be the location of the witness argument in the quotation data, type
    /// witnessArg is the lambda for the witness
    ///
    | WitnessArg of traitInfo: TraitConstraintInfo * range: Text.range

    /// Indicates a free choice of typars that arises due to
    /// minimization of polymorphism at let-rec bindings. These are
    /// resolved to a concrete instantiation on subsequent rewrites.
    | TyChoose of typeParams: Typars * bodyExpr: Expr * range: Text.range

    /// An instance of a link node occurs for every use of a recursively bound variable. When type-checking
    /// the recursive bindings a dummy expression is stored in the mutable reference cell.
    /// After type checking the bindings this is replaced by a use of the variable, perhaps at an
    /// appropriate type instantiation. These are immediately eliminated on subsequent rewrites.
    | Link of Expr ref

    /// Indicates a debug point should be placed prior to the expression.
    | DebugPoint of Syntax.DebugPointAtLeafExpr * Expr

    member ToDebugString: depth: int -> string

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Get the mark/range/position information from an expression
    member Range: Text.range

[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type TOp =

    /// An operation representing the creation of a union value of the particular union case
    | UnionCase of UnionCaseRef

    /// An operation representing the creation of an exception value using an F# exception declaration
    | ExnConstr of TyconRef

    /// An operation representing the creation of a tuple value
    | Tuple of TupInfo

    /// An operation representing the creation of an anonymous record
    | AnonRecd of AnonRecdTypeInfo

    /// An operation representing the get of a property from an anonymous record
    | AnonRecdGet of AnonRecdTypeInfo * int

    /// An operation representing the creation of an array value
    | Array

    /// Constant byte arrays (used for parser tables type other embedded data)
    | Bytes of byte[]

    /// Constant uint16 arrays (used for parser tables)
    | UInt16s of uint16[]

    /// An operation representing a lambda-encoded while loop. The special while loop marker is used to mark compilations of 'foreach' expressions
    | While of spWhile: Syntax.DebugPointAtWhile * marker: SpecialWhileLoopMarker

    /// An operation representing a lambda-encoded integer for-loop
    | IntegerForLoop of spFor: Syntax.DebugPointAtFor * spTo: Syntax.DebugPointAtInOrTo * style: ForLoopStyle

    /// An operation representing a lambda-encoded try/with
    | TryWith of spTry: Syntax.DebugPointAtTry * spWith: Syntax.DebugPointAtWith

    /// An operation representing a lambda-encoded try/finally
    | TryFinally of spTry: Syntax.DebugPointAtTry * spFinally: Syntax.DebugPointAtFinally

    /// Construct a record or object-model value. The ValRef is for self-referential class constructors, otherwise
    /// it indicates that we're in a constructor type the purpose of the expression is to
    /// fill in the fields of a pre-created but uninitialized object, type to assign the initialized
    /// version of the object into the optional mutable cell pointed to be the given value.
    | Recd of RecordConstructionInfo * TyconRef

    /// An operation representing setting a record or class field
    | ValFieldSet of RecdFieldRef

    /// An operation representing getting a record or class field
    | ValFieldGet of RecdFieldRef

    /// An operation representing getting the address of a record field
    | ValFieldGetAddr of RecdFieldRef * readonly: bool

    /// An operation representing getting an integer tag for a union value representing the union case number
    | UnionCaseTagGet of TyconRef

    /// An operation representing a coercion that proves a union value is of a particular union case. This is not a test, its
    /// simply added proof to enable us to generate verifiable code for field access on union types
    | UnionCaseProof of UnionCaseRef

    /// An operation representing a field-get from a union value, where that value has been proven to be of the corresponding union case.
    | UnionCaseFieldGet of UnionCaseRef * int

    /// An operation representing a field-get from a union value, where that value has been proven to be of the corresponding union case.
    | UnionCaseFieldGetAddr of UnionCaseRef * int * readonly: bool

    /// An operation representing a field-get from a union value. The value is not assumed to have been proven to be of the corresponding union case.
    | UnionCaseFieldSet of UnionCaseRef * int

    /// An operation representing a field-get from an F# exception value.
    | ExnFieldGet of TyconRef * int

    /// An operation representing a field-set on an F# exception value.
    | ExnFieldSet of TyconRef * int

    /// An operation representing a field-get from an F# tuple value.
    | TupleFieldGet of TupInfo * int

    /// IL assembly code - type list are the types pushed on the stack
    | ILAsm of instrs: ILInstr list * retTypes: TTypes

    /// Generate a ldflda on an 'a ref.
    | RefAddrGet of bool

    /// Conversion node, compiled via type-directed translation or to box/unbox
    | Coerce

    /// Represents a "rethrow" operation. May not be rebound, or used outside of try-finally, expecting a unit argument
    | Reraise

    /// Used for state machine compilation
    | Return

    /// Used for state machine compilation
    | Goto of ILCodeLabel

    /// Used for state machine compilation
    | Label of ILCodeLabel

    /// Pseudo method calls. This is used for overloaded operations like op_Addition.
    | TraitCall of TraitConstraintInfo

    /// Operation nodes representing C-style operations on byrefs type mutable vals (l-values)
    | LValueOp of LValueOperation * ValRef

    /// IL method calls.
    ///     isProperty -- used for quotation reflection, property getters & setters
    ///     noTailCall - DllImport? if so don't tailcall
    ///     retTypes -- the types of pushed values, if any
    | ILCall of
        isVirtual: bool *
        isProtected: bool *
        isStruct: bool *
        isCtor: bool *
        valUseFlag: ValUseFlag *
        isProperty: bool *
        noTailCall: bool *
        ilMethRef: ILMethodRef *
        enclTypeInst: TypeInst *
        methInst: TypeInst *
        retTypes: TTypes

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents the kind of record construction operation.
type RecordConstructionInfo =

    /// We're in an explicit constructor. The purpose of the record expression is to
    /// fill in the fields of a pre-created but uninitialized object
    | RecdExprIsObjInit

    /// Normal record construction
    | RecdExpr

/// If this is Some ty then it indicates that a .NET 2.0 constrained call is required, with the given type as the
/// static type of the object argument.
type ConstrainedCallInfo = TType option

/// Represents the kind of looping operation.
type SpecialWhileLoopMarker =
    | NoSpecialWhileLoopMarker

    /// Marks the compiled form of a 'for ... in ... do ' expression
    | WhileLoopForCompiledForEachExprMarker

/// Represents the kind of looping operation.
type ForLoopStyle =

    /// Evaluate start type end once, loop up
    | FSharpForLoopUp

    /// Evaluate start type end once, loop down
    | FSharpForLoopDown

    /// Evaluate start once type end multiple times, loop up
    | CSharpForLoopUp

/// Indicates what kind of pointer operation this is.
type LValueOperation =

    /// In C syntax this is: &localv
    | LAddrOf of readonly: bool

    /// In C syntax this is: *localv_ptr
    | LByrefGet

    /// In C syntax this is: localv = e, note == *(&localv) = e == LAddrOf; LByrefSet
    | LSet

    /// In C syntax this is: *localv_ptr = e
    | LByrefSet

/// Represents the kind of sequential operation, i.e. "normal" or "to a before returning b"
type SequentialOpKind =

    /// a ; b
    | NormalSeq

    /// let res = a in b;res
    | ThenDoSeq

/// Indicates how a value, function or member is being used at a particular usage point.
type ValUseFlag =

    /// Indicates a use of a value represents a call to a method that may require
    /// a .NET 2.0 constrained call. A constrained call is only used for calls where
    | PossibleConstrainedCall of ty: TType

    /// A normal use of a value
    | NormalValUse

    /// A call to a constructor, e.g. 'inherit C()'
    | CtorValUsedAsSuperInit

    /// A call to a constructor, e.g. 'new C() = new C(3)'
    | CtorValUsedAsSelfInit

    /// A call to a base method, e.g. 'base.OnPaint(args)'
    | VSlotDirectCall

/// Represents the kind of an F# core library static optimization construct
type StaticOptimization =

    /// Indicates the static optimization applies when a type equality holds
    | TTyconEqualsTycon of ty1: TType * ty2: TType

    /// Indicates the static optimization applies when a type is a struct
    | TTyconIsStruct of ty: TType

/// A representation of a method in an object expression.
///
/// TObjExprMethod(slotsig, attribs, methTyparsOfOverridingMethod, methodParams, methodBodyExpr, m)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type ObjExprMethod =
    | TObjExprMethod of
        slotSig: SlotSig *
        attribs: Attribs *
        methTyparsOfOverridingMethod: Typars *
        methodParams: Val list list *
        methodBodyExpr: Expr *
        range: Text.range

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    member Id: Syntax.Ident

/// Represents an abstract method slot, or delegate signature.
///
/// TSlotSig(methodName, declaringType, declaringTypeParameters, methodTypeParameters, slotParameters, returnTy)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type SlotSig =
    | TSlotSig of
        methodName: string *
        implementedType: TType *
        classTypars: Typars *
        methodTypars: Typars *
        formalParams: SlotParam list list *
        formalReturn: TType option

    override ToString: unit -> string

    /// The class type parameters of the slot
    member ClassTypars: Typars

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// The formal parameters of the slot (regardless of the type or method instantiation)
    member FormalParams: SlotParam list list

    /// The formal return type of the slot (regardless of the type or method instantiation)
    member FormalReturnType: TType option

    /// The (instantiated) type which the slot is logically a part of
    member ImplementedType: TType

    /// The method type parameters of the slot
    member MethodTypars: Typars

    /// The name of the method
    member Name: string

/// Represents a parameter to an abstract method slot.
///
/// TSlotParam(nm, ty, inFlag, outFlag, optionalFlag, attribs)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type SlotParam =
    | TSlotParam of
        paramName: string option *
        paramType: TType *
        isIn: bool *
        isOut: bool *
        isOptional: bool *
        attributes: Attribs

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    member Type: TType

/// Represents open declaration statement.
type OpenDeclaration =
    {

        /// Syntax after 'open' as it's presented in source code.
        Target: Syntax.SynOpenDeclTarget

        /// Full range of the open declaration.
        Range: Text.range option

        /// Modules or namespaces which is opened with this declaration.
        Modules: ModuleOrNamespaceRef list

        /// Types whose static content is opened with this declaration.
        Types: TType list

        /// Scope in which open declaration is visible.
        AppliedScope: Text.range

        /// If it's `namespace Xxx.Yyy` declaration.
        IsOwnNamespace: bool
    }

    /// Create a new instance of OpenDeclaration.
    static member Create:
        target: Syntax.SynOpenDeclTarget *
        modules: ModuleOrNamespaceRef list *
        types: TType list *
        appliedScope: Text.range *
        isOwnNamespace: bool ->
            OpenDeclaration

/// The contents of a module-or-namespace-fragment definition
[<NoEquality; NoComparison>]
type ModuleOrNamespaceContents =

    /// Indicates the module fragment is made of several module fragments in succession
    | TMDefs of defs: ModuleOrNamespaceContents list

    /// Indicates the given 'open' declarations are active
    | TMDefOpens of openDecls: OpenDeclaration list

    /// Indicates the module fragment is a 'let' definition
    | TMDefLet of binding: Binding * range: Text.range

    /// Indicates the module fragment is an evaluation of expression for side-effects
    | TMDefDo of expr: Expr * range: Text.range

    /// Indicates the module fragment is a 'rec' or 'non-rec' definition of types type modules
    | TMDefRec of
        isRec: bool *
        opens: OpenDeclaration list *
        tycons: Tycon list *
        bindings: ModuleOrNamespaceBinding list *
        range: Text.range

    override ToString: unit -> string

    member DebugText: string

/// A named module-or-namespace-fragment definition
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type ModuleOrNamespaceBinding =
    | Binding of binding: Binding

    /// The moduleOrNamespace represents the signature of the module.
    /// The moduleOrNamespaceContents contains the definitions of the module.
    /// The same set of entities are bound in the ModuleOrNamespace as in the ModuleOrNamespaceContents.
    | Module of moduleOrNamespace: ModuleOrNamespace * moduleOrNamespaceContents: ModuleOrNamespaceContents

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

[<CustomEquality; CustomComparison; RequireQualifiedAccess>]
type NamedDebugPointKey =
    { Range: Text.range
      Name: string }

    interface IComparable

    override Equals: yobj: obj -> bool

    override GetHashCode: unit -> int

/// Represents a complete typechecked implementation file, including its inferred or explicit signature.
///
/// CheckedImplFile (qualifiedNameOfFile, pragmas, signature, contents, hasExplicitEntryPoint, isScript, anonRecdTypeInfo)
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type CheckedImplFile =
    | CheckedImplFile of
        qualifiedNameOfFile: Syntax.QualifiedNameOfFile *
        pragmas: Syntax.ScopedPragma list *
        signature: ModuleOrNamespaceType *
        contents: ModuleOrNamespaceContents *
        hasExplicitEntryPoint: bool *
        isScript: bool *
        anonRecdTypeInfo: StampMap<AnonRecdTypeInfo> *
        namedDebugPointsForInlinedCode: Map<NamedDebugPointKey, Text.range>

    override ToString: unit -> string

    member Contents: ModuleOrNamespaceContents

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    member HasExplicitEntryPoint: bool

    member IsScript: bool

    member Pragmas: Syntax.ScopedPragma list

    member QualifiedNameOfFile: Syntax.QualifiedNameOfFile

    member Signature: ModuleOrNamespaceType

/// Represents a complete typechecked assembly, made up of multiple implementation files.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type CheckedImplFileAfterOptimization =
    { ImplFile: CheckedImplFile
      OptimizeDuringCodeGen: bool -> Expr -> Expr }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents a complete typechecked assembly, made up of multiple implementation files.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type CheckedAssemblyAfterOptimization =
    | CheckedAssemblyAfterOptimization of CheckedImplFileAfterOptimization list

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type CcuData =
    {

        /// Holds the file name for the DLL, if any
        FileName: string option

        /// Holds the data indicating how this assembly/module is referenced from the code being compiled.
        ILScopeRef: ILScopeRef

        /// A unique stamp for this DLL
        Stamp: Stamp

        /// The fully qualified assembly reference string to refer to this assembly. This is persisted in quotations
        QualifiedName: string option

        /// A hint as to where does the code for the CCU live (e.g what was the tcConfig.implicitIncludeDir at compilation time for this DLL?)
        SourceCodeDirectory: string

        /// Indicates that this DLL was compiled using the F# compiler type has F# metadata
        IsFSharp: bool

#if !NO_TYPEPROVIDERS
        /// Is the CCu an assembly injected by a type provider
        IsProviderGenerated: bool

        /// Triggered when the contents of the CCU are invalidated
        InvalidateEvent: IEvent<string>

        /// A helper function used to link method signatures using type equality. This is effectively a forward call to the type equality
        /// logic in tastops.fs
        ImportProvidedType: Tainted<ProvidedType> -> TType
#endif

        /// Indicates that this DLL uses pre-F#-4.0 quotation literals somewhere. This is used to implement a restriction on static linking
        mutable UsesFSharp20PlusQuotations: bool

        /// A handle to the full specification of the contents of the module contained in this ccu
        mutable Contents: ModuleOrNamespace

        /// A helper function used to link method signatures using type equality. This is effectively a forward call to the type equality
        /// logic in tastops.fs
        TryGetILModuleDef: unit -> ILModuleDef option

        /// A helper function used to link method signatures using type equality. This is effectively a forward call to the type equality
        /// logic in tastops.fs
        MemberSignatureEquality: TType -> TType -> bool

        /// The table of .NET CLI type forwarders for this assembly
        TypeForwarders: CcuTypeForwarderTable
        XmlDocumentationInfo: XmlDocumentationInfo option
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

type CcuTypeForwarderTree =
    { Value: Lazy<EntityRef> option
      Children: ImmutableDictionary<string, CcuTypeForwarderTree> }

    static member Empty: CcuTypeForwarderTree

/// Represents a table of .NET CLI type forwarders for an assembly
type CcuTypeForwarderTable =
    { Root: CcuTypeForwarderTree }

    member TryGetValue: path: string array -> item: string -> Lazy<EntityRef> option

    static member Empty: CcuTypeForwarderTable

type CcuReference = string

/// A relinkable handle to the contents of a compilation unit. Relinking is performed by mutation.
[<NoEquality; NoComparison; RequireQualifiedAccess; StructuredFormatDisplay("{DebugText}")>]
type CcuThunk =
    {

        /// ccu.target is null when a reference is missing in the transitive closure of static references that
        /// may potentially be required for the metadata of referenced DLLs.
        mutable target: CcuData
        name: CcuReference
    }

    /// Create a CCU with the given name type contents
    static member Create: nm: CcuReference * x: CcuData -> CcuThunk

    /// Create a CCU with the given name but where the contents have not yet been specified
    static member CreateDelayed: nm: CcuReference -> CcuThunk

    /// Used at the end of comppiling an assembly to get a frozen, final stable CCU
    /// for the compilation which we no longer mutate.
    member CloneWithFinalizedContents: ccuContents: ModuleOrNamespace -> CcuThunk

    /// Ensure the ccu is derefable in advance. Supply a path to attach to any resulting error message.
    member EnsureDerefable: requiringPath: string[] -> unit

    /// Fixup a CCU to have the given contents
    member Fixup: avail: CcuThunk -> unit

#if !NO_TYPEPROVIDERS
    /// Used to make 'forward' calls into the loader during linking
    member ImportProvidedType: ty: Tainted<ProvidedType> -> TType
#endif

    /// Used to make forward calls into the type/assembly loader when comparing member signatures during linking
    member MemberSignatureEquality: ty1: TType * ty2: TType -> bool

    override ToString: unit -> string

    /// Try to resolve a path into the CCU by referencing the .NET/CLI type forwarder table of the CCU
    member TryForward: nlpath: string[] * item: string -> EntityRef option

    /// Try to get the .NET Assembly, if known. May not be present for `IsFSharp` for
    /// in-memory cross-project references
    member TryGetILModuleDef: unit -> ILModuleDef option

    /// The short name of the assembly being referenced
    member AssemblyName: string

    /// A handle to the full specification of the contents of the module contained in this ccu
    member Contents: Entity

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

    /// Dereference the assembly reference
    member Deref: CcuData

    /// Holds the file name for the assembly, if any
    member FileName: string option

    /// Holds the data indicating how this assembly/module is referenced from the code being compiled.
    member ILScopeRef: ILScopeRef

    /// Indicates that this DLL was compiled using the F# compiler type has F# metadata
    member IsFSharp: bool

#if !NO_TYPEPROVIDERS
    /// Is this a provider-injected assembly
    member IsProviderGenerated: bool
#endif

    /// Indicates if this assembly reference is unresolved
    member IsUnresolvedReference: bool

    /// The fully qualified assembly reference string to refer to this assembly. This is persisted in quotations
    member QualifiedName: string option

    /// The table of modules type namespaces at the "root" of the assembly
    member RootModulesAndNamespaces: Entity list

    /// The table of type definitions at the "root" of the assembly
    member RootTypeAndExceptionDefinitions: Entity list

    /// A hint as to where does the code for the CCU live (e.g what was the tcConfig.implicitIncludeDir at compilation time for this DLL?)
    member SourceCodeDirectory: string

    /// A unique stamp for this assembly
    member Stamp: Stamp

    /// The table of type forwarders for this assembly
    member TypeForwarders: CcuTypeForwarderTable

    /// Indicates that this DLL uses F# 2.0+ quotation literals somewhere. This is used to implement a restriction on static linking.
    member UsesFSharp20PlusQuotations: bool with get, set

/// The result of attempting to resolve an assembly name to a full ccu.
/// UnresolvedCcu will contain the name of the assembly that could not be resolved.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type CcuResolutionResult =
    | ResolvedCcu of CcuThunk
    | UnresolvedCcu of string

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents the information saved in the assembly signature data resource for an F# assembly
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type PickledCcuInfo =
    { mspec: ModuleOrNamespace
      compileTimeWorkingDir: string
      usesQuotations: bool }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents a set of free local values. Computed type cached by later phases
/// (never cached type checking). Cached in expressions. Not pickled.
type FreeLocals = Zset<Val>

/// Represents a set of free type parameters. Computed type cached by later phases
/// (never cached type checking). Cached in expressions. Not pickled.
type FreeTypars = Zset<Typar>

/// Represents a set of 'free' named type definitions. Used to collect the named type definitions referred to
/// from a type or expression. Computed type cached by later phases (never cached type checking). Cached
/// in expressions. Not pickled.
type FreeTycons = Zset<Tycon>

/// Represents a set of 'free' record field definitions. Used to collect the record field definitions referred to
/// from an expression.
type FreeRecdFields = Zset<RecdFieldRef>

/// Represents a set of 'free' union cases. Used to collect the union cases referred to from an expression.
type FreeUnionCases = Zset<UnionCaseRef>

/// Represents a set of 'free' type-related elements, including named types, trait solutions, union cases and
/// record fields.
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type FreeTyvars =
    {

        /// The summary of locally defined type definitions used in the expression. These may be made private by a signature
        /// type we have to check various conditions associated with that.
        FreeTycons: FreeTycons

        /// The summary of values used as trait solutions
        FreeTraitSolutions: FreeLocals

        /// The summary of type parameters used in the expression. These may not escape the enclosing generic construct
        /// type we have to check various conditions associated with that.
        FreeTypars: FreeTypars
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// Represents an amortized computation of the free variables in an expression
type FreeVarsCache = cache<FreeVars>

/// Represents the set of free variables in an expression
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type FreeVars =
    {

        /// The summary of locally defined variables used in the expression. These may be hidden at let bindings etc.
        /// or made private by a signature or marked 'internal' or 'private', type we have to check various conditions associated with that.
        FreeLocals: FreeLocals

        /// Indicates if the expression contains a call to a protected member or a base call.
        /// Calls to protected members type direct calls to super classes can't escape, also code can't be inlined
        UsesMethodLocalConstructs: bool

        /// Indicates if the expression contains a call to rethrow that is not bound under a (try-)with branch.
        /// Rethrow may only occur in such locations.
        UsesUnboundRethrow: bool

        /// The summary of locally defined tycon representations used in the expression. These may be made private by a signature
        /// or marked 'internal' or 'private' type we have to check various conditions associated with that.
        FreeLocalTyconReprs: FreeTycons

        /// The summary of fields used in the expression. These may be made private by a signature
        /// or marked 'internal' or 'private' type we have to check various conditions associated with that.
        FreeRecdFields: FreeRecdFields

        /// The summary of union constructors used in the expression. These may be
        /// marked 'internal' or 'private' type we have to check various conditions associated with that.
        FreeUnionCases: FreeUnionCases

        /// See FreeTyvars above.
        FreeTyvars: FreeTyvars
    }

    override ToString: unit -> string

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member DebugText: string

/// A set of static methods for constructing types.
type Construct =

    new: unit -> Construct

#if !NO_TYPEPROVIDERS
    /// Compute the definition location of a provided item
    static member ComputeDefinitionLocationOfProvidedItem:
        p: Tainted<#IProvidedCustomAttributeProvider> -> Text.range option
#endif

    /// Key a Tycon or TyconRef by both mangled type demangled name.
    /// Generic types can be accessed either by 'List' or 'List`1'.
    /// This lists both keys.
    static member KeyTyconByAccessNames: nm: string -> x: 'T -> KeyValuePair<string, 'T>[]

    /// Key a Tycon or TyconRef by decoded name
    static member KeyTyconByDecodedName: nm: string -> x: 'T -> KeyValuePair<Syntax.PrettyNaming.NameArityPair, 'T>

    /// Create the field tables for a record or class type
    static member MakeRecdFieldsTable: ucs: RecdField list -> TyconRecdFields

    /// Create the union case tables for a union type
    static member MakeUnionCases: ucs: UnionCase list -> TyconUnionData

    /// Create a node for a union type
    static member MakeUnionRepr: ucs: UnionCase list -> TyconRepresentation

    /// Create the new contents of an overall assembly
    static member NewCcuContents:
        sref: ILScopeRef -> m: Text.range -> nm: string -> mty: ModuleOrNamespaceType -> ModuleOrNamespace

    /// Create a new module or namespace node by cloning an existing one
    static member NewClonedModuleOrNamespace: orig: Tycon -> Entity

    /// Create a new type definition node by cloning an existing one
    static member NewClonedTycon: orig: Tycon -> Entity

    /// Create a new node for an empty module or namespace contents
    static member NewEmptyModuleOrNamespaceType: mkind: ModuleOrNamespaceKind -> ModuleOrNamespaceType

    /// Create a new TAST Entity node for an F# exception definition
    static member NewExn:
        cpath: CompilationPath option ->
        id: Syntax.Ident ->
        access: Accessibility ->
        repr: ExceptionInfo ->
        attribs: Attribs ->
        doc: XmlDoc ->
            Entity

    /// Create a new unfilled cache for free variable calculations
    static member NewFreeVarsCache: unit -> cache<'a>

    /// Create a new type definition node for a .NET type definition
    static member NewILTycon:
        nlpath: CompilationPath option ->
        nm: string * m: Text.range ->
            tps: LazyWithContext<Typars, Text.range> ->
            scoref: ILScopeRef * enc: ILTypeDef list * tdef: ILTypeDef ->
                mtyp: MaybeLazy<ModuleOrNamespaceType> ->
                    Entity

    /// Create a module Tycon based on an existing one using the function 'f'.
    /// We require that we be given the parent for the new module.
    /// We pass the new module to 'f' in case it needs to reparent the
    /// contents of the module.
    static member NewModifiedModuleOrNamespace:
        f: (ModuleOrNamespaceType -> ModuleOrNamespaceType) -> orig: Tycon -> Entity

    /// Create a tycon based on an existing one using the function 'f'.
    /// We require that we be given the new parent for the new tycon.
    /// We pass the new tycon to 'f' in case it needs to reparent the
    /// contents of the tycon.
    static member NewModifiedTycon: f: (Tycon -> Entity) -> orig: Tycon -> Entity

    /// Create a Val based on an existing one using the function 'f'.
    /// We require that we be given the parent for the new Val.
    static member NewModifiedVal: f: (Val -> Val) -> orig: Val -> Val

    /// Create a new entity node for a module or namespace
    static member NewModuleOrNamespace:
        cpath: CompilationPath option ->
        access: Accessibility ->
        id: Syntax.Ident ->
        xml: XmlDoc ->
        attribs: Attrib list ->
        mtype: MaybeLazy<ModuleOrNamespaceType> ->
            ModuleOrNamespace

    /// Create a new node for the contents of a module or namespace
    static member NewModuleOrNamespaceType:
        mkind: ModuleOrNamespaceKind -> tycons: Entity list -> vals: Val list -> ModuleOrNamespaceType

#if !NO_TYPEPROVIDERS
    /// Create a new entity node for a provided type definition
    static member NewProvidedTycon:
        resolutionEnvironment: ResolutionEnvironment *
        st: Tainted<ProvidedType> *
        importProvidedType: (Tainted<ProvidedType> -> TType) *
        isSuppressRelocate: bool *
        m: Text.range *
        ?access: Accessibility *
        ?cpath: CompilationPath ->
            Entity

    /// Create a new node for the representation information for a provided type definition
    static member NewProvidedTyconRepr:
        resolutionEnvironment: ResolutionEnvironment *
        st: Tainted<ProvidedType> *
        importProvidedType: (Tainted<ProvidedType> -> TType) *
        isSuppressRelocate: bool *
        m: Text.range ->
            TyconRepresentation
#endif

    /// Create a new TAST RecdField node for an F# class, struct or record field
    static member NewRecdField:
        stat: bool ->
        konst: Const option ->
        id: Syntax.Ident ->
        nameGenerated: bool ->
        ty: TType ->
        isMutable: bool ->
        isVolatile: bool ->
        pattribs: Attribs ->
        fattribs: Attribs ->
        docOption: XmlDoc ->
        access: Accessibility ->
        secret: bool ->
            RecdField

    /// Create a new type parameter node for a declared type parameter
    static member NewRigidTypar: nm: string -> m: Text.range -> Typar

    /// Create a new type definition node
    static member NewTycon:
        cpath: CompilationPath option *
        nm: string *
        m: Text.range *
        access: Accessibility *
        reprAccess: Accessibility *
        kind: TyparKind *
        typars: LazyWithContext<Typars, Text.range> *
        doc: XmlDoc *
        usesPrefixDisplay: bool *
        preEstablishedHasDefaultCtor: bool *
        hasSelfReferentialCtor: bool *
        mtyp: MaybeLazy<ModuleOrNamespaceType> ->
            Entity

    /// Create a new type parameter node
    static member NewTypar:
        kind: TyparKind *
        rigid: TyparRigidity *
        Syntax.SynTypar *
        isFromError: bool *
        dynamicReq: TyparDynamicReq *
        attribs: Attrib list *
        eqDep: bool *
        compDep: bool ->
            Typar

    /// Create a new union case node
    static member NewUnionCase:
        id: Syntax.Ident ->
        tys: RecdField list ->
        retTy: TType ->
        attribs: Attribs ->
        docOption: XmlDoc ->
        access: Accessibility ->
            UnionCase

    /// Create a new Val node
    static member NewVal:
        logicalName: string *
        m: Text.range *
        compiledName: string option *
        ty: TType *
        isMutable: ValMutability *
        isCompGen: bool *
        arity: ValReprInfo option *
        access: Accessibility *
        recValInfo: ValRecursiveScopeInfo *
        specialRepr: ValMemberInfo option *
        baseOrThis: ValBaseOrThisInfo *
        attribs: Attrib list *
        inlineInfo: ValInline *
        doc: XmlDoc *
        isModuleOrMemberBinding: bool *
        isExtensionMember: bool *
        isIncrClassSpecialMember: bool *
        isTyFunc: bool *
        allowTypeInst: bool *
        isGeneratedEventVal: bool *
        konst: Const option *
        actualParent: ParentRef ->
            Val

module CcuTypeForwarderTable =

    val findInTree:
        remainingPath: ArraySegment<string> -> finalKey: string -> tree: CcuTypeForwarderTree -> Lazy<EntityRef> option
