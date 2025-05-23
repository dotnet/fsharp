// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.NameResolution

open Internal.Utilities.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Infos
open FSharp.Compiler.Import
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

/// A NameResolver is a context for name resolution. It primarily holds an InfoReader.
type NameResolver =

    new:
        g: TcGlobals * amap: ImportMap * infoReader: InfoReader * instantiationGenerator: (range -> Typars -> TypeInst) ->
            NameResolver

    member InfoReader: InfoReader

    member amap: ImportMap

    member g: TcGlobals

    member languageSupportsNameOf: bool

/// Get the active pattern elements defined in a module, if any. Cache in the slot in the module type.
val ActivePatternElemsOfModuleOrNamespace: g: TcGlobals -> ModuleOrNamespaceRef -> NameMap<ActivePatternElemRef>

/// Represents the item with which a named argument is associated.
[<NoEquality; NoComparison; RequireQualifiedAccess>]
type ArgumentContainer =
    /// The named argument is an argument of a method
    | Method of MethInfo

    /// The named argument is a static parameter to a provided type.
    | Type of TyconRef

type EnclosingTypeInst = TypeInst

/// Represents an item that results from name resolution
[<NoEquality; NoComparison; RequireQualifiedAccess>]
type Item =
    /// Represents the resolution of a name to an F# value or function.
    | Value of ValRef

    /// Represents the resolution of a name to an F# union case.
    | UnionCase of UnionCaseInfo * hasRequireQualifiedAccessAttr: bool

    /// Represents the resolution of a name to an F# active pattern result.
    | ActivePatternResult of apinfo: ActivePatternInfo * apOverallTy: TType * index: int * range: range

    /// Represents the resolution of a name to an F# active pattern case within the body of an active pattern.
    | ActivePatternCase of ActivePatternElemRef

    /// Represents the resolution of a name to an F# exception definition.
    | ExnCase of TyconRef

    /// Represents the resolution of a name to an F# record or exception field.
    | RecdField of RecdFieldInfo

    /// Represents the resolution of a name to an F# trait
    | Trait of TraitConstraintInfo

    /// Represents the resolution of a name to a union case field.
    | UnionCaseField of UnionCaseInfo * fieldIndex: int

    /// Represents the resolution of a name to a field of an anonymous record type.
    | AnonRecdField of anonInfo: AnonRecdTypeInfo * tys: TTypes * fieldIndex: int * range: range

    // The following are never in the items table but are valid results of binding
    // an identifier in different circumstances.

    /// Represents the resolution of a name at the point of its own definition.
    | NewDef of Ident

    /// Represents the resolution of a name to a .NET field
    | ILField of ILFieldInfo

    /// Represents the resolution of a name to an event
    | Event of EventInfo

    /// Represents the resolution of a name to a property
    | Property of name: string * info: PropInfo list * sourceIdentifierRange: range option

    /// Represents the resolution of a name to a group of methods.
    | MethodGroup of displayName: string * methods: MethInfo list * uninstantiatedMethodOpt: MethInfo option

    /// Represents the resolution of a name to a constructor
    | CtorGroup of string * MethInfo list

    /// Represents the resolution of a name to a delegate
    | DelegateCtor of TType

    /// Represents the resolution of a name to a group of types
    | Types of string * TType list

    /// CustomOperation(nm, helpText, methInfo)
    ///
    /// Used to indicate the availability or resolution of a custom query operation such as 'sortBy' or 'where' in computation expression syntax
    | CustomOperation of string * (unit -> string option) * MethInfo option

    /// Represents the resolution of a name to a custom builder in the F# computation expression syntax
    | CustomBuilder of string * ValRef

    /// Represents the resolution of a name to a type variable
    | TypeVar of string * Typar

    /// Represents the resolution of a name to a module or namespace
    | ModuleOrNamespaces of ModuleOrNamespaceRef list

    /// Represents the resolution of a name to an operator
    | ImplicitOp of Ident * TraitConstraintSln option ref

    /// Represents the resolution of a name to a named argument
    //
    // In the FCS API, Item.OtherName corresponds to FSharpParameter symbols.
    // Not all parameters have names, e.g. for 'g' in this:
    //
    //    let f (g: int -> int) x = ...
    //
    // then the symbol for 'g' reports FSharpParameters via CurriedParameterGroups
    // based on analyzing the type of g as a function type.
    //
    // For these parameters, the identifier will be missing.
    | OtherName of
        ident: Ident option *
        argType: TType *
        argInfo: ArgReprInfo option *
        container: ArgumentContainer option *
        range: range

    /// Represents the resolution of a name to a named property setter
    | SetterArg of Ident * Item

    /// Represents the potential resolution of an unqualified name to a type.
    | UnqualifiedType of TyconRef list

    /// The text for the item to use in the declaration list.
    /// This does not include backticks, parens etc.
    ///
    /// Note: here "Core" means "without added backticks or parens"
    member DisplayNameCore: string

    /// The full text for the item to show in error messages and to use in code.
    /// This includes backticks, parens etc.
    member DisplayName: string

/// Pairs an Item with a TyparInstantiation showing how generic type variables of the item are instantiated at
/// a particular usage point.
[<RequireQualifiedAccess>]
type ItemWithInst =
    { Item: Item
      TyparInstantiation: TyparInstantiation }

val (|ItemWithInst|): ItemWithInst -> Item * TyparInstantiation
val ItemWithNoInst: Item -> ItemWithInst

/// Represents a record field resolution and the information if the usage is deprecated.
type FieldResolution = FieldResolution of RecdFieldInfo * bool

/// Information about an extension member held in the name resolution environment
type ExtensionMember =
    /// F#-style Extrinsic extension member, defined in F# code
    | FSExtMem of ValRef * ExtensionMethodPriority

    /// ILExtMem(declaringTyconRef, ilMetadata, pri)
    ///
    /// IL-style extension member, backed by some kind of method with an [<Extension>] attribute
    | ILExtMem of TyconRef * MethInfo * ExtensionMethodPriority

    /// Describes the sequence order of the introduction of an extension method. Extension methods that are introduced
    /// later through 'open' get priority in overload resolution.
    member Priority: ExtensionMethodPriority

/// The environment of information used to resolve names
[<NoEquality; NoComparison>]
type NameResolutionEnv =
    {
        /// Display environment information for output
        eDisplayEnv: DisplayEnv

        /// Values and Data Tags available by unqualified name
        eUnqualifiedItems: LayeredMap<string, Item>

        /// Enclosing type instantiations that are associated with an unqualified type item
        eUnqualifiedEnclosingTypeInsts: TyconRefMap<EnclosingTypeInst>

        /// Data Tags and Active Pattern Tags available by unqualified name
        ePatItems: NameMap<Item>

        /// Modules accessible via "." notation. Note this is a multi-map.
        /// Adding a module abbreviation adds it a local entry to this List.map.
        /// Likewise adding a ccu or opening a path adds entries to this List.map.
        eModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef>

        /// Fully qualified modules and namespaces. 'open' does not change this.
        eFullyQualifiedModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef>

        /// RecdField labels in scope.  RecdField labels are those where type are inferred
        /// by label rather than by known type annotation.
        /// Bools indicate if from a record, where no warning is given on indeterminate lookup
        eFieldLabels: NameMultiMap<RecdFieldRef>

        /// Record or unions that may have type instantiations associated with them
        /// when record labels or union cases are used in an unqualified context.
        eUnqualifiedRecordOrUnionTypeInsts: TyconRefMap<TypeInst>

        /// Tycons indexed by the various names that may be used to access them, e.g.
        ///     "List" --> multiple TyconRef's for the various tycons accessible by this name.
        ///     "List`1" --> TyconRef
        eTyconsByAccessNames: LayeredMultiMap<string, TyconRef>

        eFullyQualifiedTyconsByAccessNames: LayeredMultiMap<string, TyconRef>

        /// Tycons available by unqualified, demangled names (i.e. (List,1) --> TyconRef)
        eTyconsByDemangledNameAndArity: LayeredMap<NameArityPair, TyconRef>

        /// Tycons available by unqualified, demangled names (i.e. (List,1) --> TyconRef)
        eFullyQualifiedTyconsByDemangledNameAndArity: LayeredMap<NameArityPair, TyconRef>

        /// Extension members by type and name
        eIndexedExtensionMembers: TyconRefMultiMap<ExtensionMember>

        /// Other extension members unindexed by type
        eUnindexedExtensionMembers: ExtensionMember list

        /// Typars (always available by unqualified names). Further typars can be
        /// in the tpenv, a structure folded through each top-level definition.
        eTypars: NameMap<Typar>

    }

    static member Empty: g: TcGlobals -> NameResolutionEnv
    member DisplayEnv: DisplayEnv
    member FindUnqualifiedItem: string -> Item

type FullyQualifiedFlag =
    | FullyQualified
    | OpenQualified

/// Indicates whether an identifier (single or long) is followed by an extra dot. Typically used
/// to provide better tooling and error reporting.
[<RequireQualifiedAccess>]
type ExtraDotAfterIdentifier =
    | Yes
    | No

[<RequireQualifiedAccess>]
type BulkAdd =
    | Yes
    | No

/// Find a field in anonymous record type
val internal TryFindAnonRecdFieldOfType: TcGlobals -> TType -> string -> Item option

/// Add extra items to the environment for Visual Studio, e.g. static members
val internal AddFakeNamedValRefToNameEnv: string -> NameResolutionEnv -> ValRef -> NameResolutionEnv

/// Add some extra items to the environment for Visual Studio, e.g. record members
val internal AddFakeNameToNameEnv: string -> NameResolutionEnv -> Item -> NameResolutionEnv

/// Add a single F# value to the environment.
val internal AddValRefToNameEnv: TcGlobals -> NameResolutionEnv -> ValRef -> NameResolutionEnv

/// Add active pattern result tags to the environment.
val internal AddActivePatternResultTagsToNameEnv:
    ActivePatternInfo -> NameResolutionEnv -> TType -> range -> NameResolutionEnv

/// Add a list of type definitions to the name resolution environment
val internal AddTyconRefsToNameEnv:
    BulkAdd ->
    bool ->
    TcGlobals ->
    ImportMap ->
    AccessorDomain ->
    range ->
    bool ->
    NameResolutionEnv ->
    TyconRef list ->
        NameResolutionEnv

/// Add an F# exception definition to the name resolution environment
val internal AddExceptionDeclsToNameEnv: BulkAdd -> NameResolutionEnv -> TyconRef -> NameResolutionEnv

/// Add a module abbreviation to the name resolution environment
val internal AddModuleAbbrevToNameEnv: Ident -> NameResolutionEnv -> ModuleOrNamespaceRef list -> NameResolutionEnv

/// Add a list of module or namespace to the name resolution environment, including any sub-modules marked 'AutoOpen'
val internal AddModuleOrNamespaceRefsToNameEnv:
    TcGlobals ->
    ImportMap ->
    range ->
    bool ->
    AccessorDomain ->
    NameResolutionEnv ->
    ModuleOrNamespaceRef list ->
        NameResolutionEnv

/// Add a single modules or namespace to the name resolution environment
val internal AddModuleOrNamespaceRefToNameEnv:
    TcGlobals ->
    ImportMap ->
    range ->
    bool ->
    AccessorDomain ->
    NameResolutionEnv ->
    ModuleOrNamespaceRef ->
        NameResolutionEnv

/// Add a list of modules or namespaces to the name resolution environment
val internal AddModuleOrNamespaceRefsContentsToNameEnv:
    TcGlobals ->
    ImportMap ->
    AccessorDomain ->
    range ->
    bool ->
    NameResolutionEnv ->
    ModuleOrNamespaceRef list ->
        NameResolutionEnv

/// Add the content of a type to the name resolution environment
val internal AddTypeContentsToNameEnv:
    TcGlobals -> ImportMap -> AccessorDomain -> range -> NameResolutionEnv -> TType -> NameResolutionEnv

/// A flag which indicates if it is an error to have two declared type parameters with identical names
/// in the name resolution environment.
type CheckForDuplicateTyparFlag =
    | CheckForDuplicateTypars
    | NoCheckForDuplicateTypars

/// Add some declared type parameters to the name resolution environment
val internal AddDeclaredTyparsToNameEnv:
    CheckForDuplicateTyparFlag -> NameResolutionEnv -> Typar list -> NameResolutionEnv

/// Qualified lookup of type names in the environment
val internal LookupTypeNameInEnvNoArity: FullyQualifiedFlag -> string -> NameResolutionEnv -> TyconRef list

/// Indicates whether we are resolving type names to type definitions or to constructor methods.
type TypeNameResolutionFlag =
    /// Indicates we are resolving type names to constructor methods.
    | ResolveTypeNamesToCtors

    /// Indicates we are resolving type names to type definitions
    | ResolveTypeNamesToTypeRefs

/// Represents information about the generic argument count of a type name when resolving it.
///
/// In some situations we resolve "List" to any type definition with that name regardless of the number
/// of generic arguments. In others, we know precisely how many generic arguments are needed.
[<Sealed; NoEquality; NoComparison>]
type TypeNameResolutionStaticArgsInfo =

    /// Indicates definite knowledge of empty type arguments, i.e. the logical equivalent of name< >
    static member DefiniteEmpty: TypeNameResolutionStaticArgsInfo

    /// Deduce definite knowledge of type arguments
    static member FromTyArgs: numTyArgs: int -> TypeNameResolutionStaticArgsInfo

/// Represents information which guides name resolution of types.
[<NoEquality; NoComparison>]
type TypeNameResolutionInfo =
    | TypeNameResolutionInfo of TypeNameResolutionFlag * TypeNameResolutionStaticArgsInfo

    static member Default: TypeNameResolutionInfo

    static member ResolveToTypeRefs: TypeNameResolutionStaticArgsInfo -> TypeNameResolutionInfo

/// Represents the kind of the occurrence when reporting a name in name resolution
[<RequireQualifiedAccess; Struct>]
type internal ItemOccurrence =
    | Binding
    | Use
    | UseInType
    | UseInAttribute
    | Pattern
    | Implemented
    | RelatedText
    | Open
    | InvalidUse

/// Check for equality, up to signature matching
val ItemsAreEffectivelyEqual: TcGlobals -> Item -> Item -> bool

/// Hash compatible with ItemsAreEffectivelyEqual
val ItemsAreEffectivelyEqualHash: TcGlobals -> Item -> int

[<Class>]
type internal CapturedNameResolution =
    /// line and column
    member Pos: pos

    /// Named item
    member Item: Item

    /// The active instantiation for any generic type parameters
    member ItemWithInst: ItemWithInst

    /// Information about the occurrence of the symbol
    member ItemOccurrence: ItemOccurrence

    /// Information about printing. For example, should redundant keywords be hidden?
    member DisplayEnv: DisplayEnv

    /// Naming environment--for example, currently open namespaces.
    member NameResolutionEnv: NameResolutionEnv

    /// The access rights of code at the location
    member AccessorDomain: AccessorDomain

    /// The starting and ending position
    member Range: range

[<Class>]
type internal TcResolutions =

    /// Name resolution environments for every interesting region in the file. These regions may
    /// overlap, in which case the smallest region applicable should be used.
    member CapturedEnvs: ResizeArray<range * NameResolutionEnv * AccessorDomain>

    /// Information of exact types found for expressions, that can be to the left of a dot.
    /// typ - the inferred type for an expression
    member CapturedExpressionTypings: ResizeArray<TType * NameResolutionEnv * AccessorDomain * range>

    /// Exact name resolutions
    member CapturedNameResolutions: ResizeArray<CapturedNameResolution>

    /// Represents additional resolutions of names to groups of methods.
    /// CapturedNameResolutions should be checked when no captured method group is found.
    /// See TypeCheckInfo.GetCapturedNameResolutions for example.
    member CapturedMethodGroupResolutions: ResizeArray<CapturedNameResolution>

    /// Represents the empty set of resolutions
    static member Empty: TcResolutions

[<Struct>]
type TcSymbolUseData =
    { ItemWithInst: ItemWithInst
      ItemOccurrence: ItemOccurrence
      DisplayEnv: DisplayEnv
      Range: range }

/// Represents container for all name resolutions that were met so far when typechecking some particular file
[<Class>]
type internal TcSymbolUses =

    /// Get all the uses of a particular item within the file
    member GetUsesOfSymbol: Item -> TcSymbolUseData[]

    /// All the uses of all items within the file
    member AllUsesOfSymbols: TcSymbolUseData[][]

    /// Get the locations of all the printf format specifiers in the file
    member GetFormatSpecifierLocationsAndArity: unit -> (range * int)[]

    /// Empty collection of symbol uses
    static member Empty: TcSymbolUses

/// Source text and an array of line end positions, used for format string parsing
type FormatStringCheckContext =
    {
        /// Source text
        SourceText: ISourceText

        /// Array of line start positions
        LineStartPositions: int[]
    }

/// An abstract type for reporting the results of name resolution and type checking
type ITypecheckResultsSink =

    /// Record that an environment is active over the given scope range
    abstract NotifyEnvWithScope: range * NameResolutionEnv * AccessorDomain -> unit

    /// Record that an expression has a specific type at the given range.
    abstract NotifyExprHasType: TType * NameResolutionEnv * AccessorDomain * range -> unit

    /// Record that a name resolution occurred at a specific location in the source
    abstract NotifyNameResolution:
        pos * Item * TyparInstantiation * ItemOccurrence * NameResolutionEnv * AccessorDomain * range * bool -> unit

    /// Record that a method group name resolution occurred at a specific location in the source
    abstract NotifyMethodGroupNameResolution:
        pos * Item * Item * TyparInstantiation * ItemOccurrence * NameResolutionEnv * AccessorDomain * range * bool ->
            unit

    /// Record that a printf format specifier occurred at a specific location in the source
    abstract NotifyFormatSpecifierLocation: range * int -> unit

    /// Record that an open declaration occurred in a given scope range
    abstract NotifyOpenDeclaration: OpenDeclaration -> unit

    /// Get the current source
    abstract CurrentSourceText: ISourceText option

    /// Cached line-end normalized source text and an array of line end positions, used for format string parsing
    abstract FormatStringCheckContext: FormatStringCheckContext option

/// An implementation of ITypecheckResultsSink to collect information during type checking
type internal TcResultsSinkImpl =

    /// Create a TcResultsSinkImpl
    new: tcGlobals: TcGlobals * ?sourceText: ISourceText -> TcResultsSinkImpl

    /// Get all the resolutions reported to the sink
    member GetResolutions: unit -> TcResolutions

    /// Get all the uses of all symbols reported to the sink
    member GetSymbolUses: unit -> TcSymbolUses

    /// Get all open declarations reported to the sink
    member GetOpenDeclarations: unit -> OpenDeclaration[]

    /// Get the format specifier locations
    member GetFormatSpecifierLocations: unit -> (range * int)[]

    interface ITypecheckResultsSink

/// An abstract type for reporting the results of name resolution and type checking, and which allows
/// temporary suspension and/or redirection of reporting.
type TcResultsSink =
    { mutable CurrentSink: ITypecheckResultsSink option }

    static member NoSink: TcResultsSink
    static member WithSink: ITypecheckResultsSink -> TcResultsSink

/// Indicates if we only need one result or all possible results from a resolution.
[<RequireQualifiedAccess>]
type ResultCollectionSettings =
    | AllResults
    | AtMostOneResult

/// Indicates if a lookup requires a match on the instance/static characteristic.
///
/// This is not supplied at all lookup sites - in theory it could be, but currently diagnostics on many paths
/// rely on returning all the content and then filtering it later for instance/static characteristic.
///
/// When applied, this also currently doesn't filter all content - it is currently only applied to filter out extension methods
/// that have a static/instance mismatch.
[<RequireQualifiedAccess>]
type LookupIsInstance =
    | Ambivalent
    | Yes
    | No

/// Indicates the kind of lookup being performed. Note, this type should be made private to nameres.fs.
[<RequireQualifiedAccess>]
type LookupKind =
    | RecdField

    | Pattern

    /// Indicates resolution within an expression, either A.B.C (static) or expr.A.B.C (instance) and
    /// whether we should filter content according to instance/static characteristic.
    | Expr of isInstanceFilter: LookupIsInstance

    | Type

    | Ctor

/// Indicates if a warning should be given for the use of upper-case identifiers in patterns
type WarnOnUpperFlag =
    | WarnOnUpperUnionCaseLabel
    | WarnOnUpperVariablePatterns
    | AllIdsOK

/// Indicates whether we permit a direct reference to a type generator. Only set when resolving the
/// right-hand-side of a [<Generate>] declaration.
[<RequireQualifiedAccess>]
type PermitDirectReferenceToGeneratedType =
    | Yes
    | No

/// Specifies extra work to do after overload resolution
[<RequireQualifiedAccess>]
type AfterResolution =
    /// Notification is not needed
    | DoNothing

    /// Notify the sink of the information needed to complete recording a use of a symbol
    /// for the purposes of the language service.  One of the callbacks should be called by
    /// the checker.
    ///
    /// The first callback represents a case where we have learned the type
    /// instantiation of a generic method or value.
    ///
    /// The second represents the case where we have resolved overloading and/or
    /// a specific override. The 'Item option' contains the candidate overrides.
    | RecordResolution of
        Item option *
        (TyparInstantiation -> unit) *
        (MethInfo * PropInfo option * TyparInstantiation -> unit) *
        (unit -> unit)

/// Indicates whether we want to report found items to the name resolution sink
[<RequireQualifiedAccess>]
type ShouldNotifySink =
    | Yes
    | No

/// Temporarily redirect reporting of name resolution and type checking results
val internal WithNewTypecheckResultsSink: ITypecheckResultsSink * TcResultsSink -> System.IDisposable

/// Temporarily suspend reporting of name resolution and type checking results
val internal TemporarilySuspendReportingTypecheckResultsToSink: TcResultsSink -> System.IDisposable

/// Report the active name resolution environment for a source range
val internal CallEnvSink: TcResultsSink -> range * NameResolutionEnv * AccessorDomain -> unit

/// Report a specific name resolution at a source range
val internal CallNameResolutionSink:
    TcResultsSink -> range * NameResolutionEnv * Item * TyparInstantiation * ItemOccurrence * AccessorDomain -> unit

/// Report a specific method group name resolution at a source range
val internal CallMethodGroupNameResolutionSink:
    TcResultsSink ->
    range * NameResolutionEnv * Item * Item * TyparInstantiation * ItemOccurrence * AccessorDomain ->
        unit

/// Report a specific name resolution at a source range, replacing any previous resolutions
val internal CallNameResolutionSinkReplacing:
    TcResultsSink -> range * NameResolutionEnv * Item * TyparInstantiation * ItemOccurrence * AccessorDomain -> unit

/// Report a specific name resolution at a source range
val internal CallExprHasTypeSink: TcResultsSink -> range * NameResolutionEnv * TType * AccessorDomain -> unit

/// Report an open declaration
val internal CallOpenDeclarationSink: TcResultsSink -> OpenDeclaration -> unit

/// Get all the available properties of a type (both intrinsic and extension)
val internal AllPropInfosOfTypeInScope:
    collectionSettings: ResultCollectionSettings ->
    infoReader: InfoReader ->
    nenv: NameResolutionEnv ->
    optFilter: string option ->
    ad: AccessorDomain ->
    findFlag: FindMemberFlag ->
    m: range ->
    ty: TType ->
        PropInfo list

/// Get all the available properties of a type (only extension)
val internal ExtensionPropInfosOfTypeInScope:
    collectionSettings: ResultCollectionSettings ->
    infoReader: InfoReader ->
    nenv: NameResolutionEnv ->
    optFilter: string option ->
    isInstanceFilter: LookupIsInstance ->
    ad: AccessorDomain ->
    m: range ->
    ty: TType ->
        PropInfo list

/// Get the available methods of a type (both declared and inherited)
val internal AllMethInfosOfTypeInScope:
    collectionSettings: ResultCollectionSettings ->
    infoReader: InfoReader ->
    nenv: NameResolutionEnv ->
    optFilter: string option ->
    ad: AccessorDomain ->
    findFlag: FindMemberFlag ->
    m: range ->
    ty: TType ->
        MethInfo list

/// Used to report an error condition where name resolution failed due to an indeterminate type
exception internal IndeterminateType of range

/// Used to report a warning condition for the use of upper-case identifiers in patterns
exception internal UpperCaseIdentifierInPattern of range

/// Generate a new reference to a record field with a fresh type instantiation
val FreshenRecdFieldRef: NameResolver -> range -> RecdFieldRef -> RecdFieldInfo

/// Create a type variable representing the use of a "_" in F# code
val NewAnonTypar: TyparKind * range * TyparRigidity * TyparStaticReq * TyparDynamicReq -> Typar

val NewNamedInferenceMeasureVar: range * TyparRigidity * TyparStaticReq * Ident -> Typar

val NewNamedInferenceMeasureVar: range * TyparRigidity * TyparStaticReq * Ident -> Typar

val NewInferenceMeasurePar: unit -> Typar

/// Create an inference type variable
val NewInferenceType: TcGlobals -> TType

/// Create an inference type variable for the kind of a byref pointer
val NewByRefKindInferenceType: TcGlobals -> range -> TType

/// Create an inference type variable representing an error condition when checking an expression
val NewErrorType: unit -> TType

/// Create an inference type variable representing an error condition when checking a measure
val NewErrorMeasure: unit -> Measure

/// Create a list of inference type variables, one for each element in the input list
val NewInferenceTypes: TcGlobals -> 'T list -> TType list

/// Given a set of type parameters, make new inference type variables for
/// each and ensure that the constraints on the new type variables are adjusted.
///
/// Returns the inference type variables as a list of types.
val FreshenTypars: g: TcGlobals -> range -> Typars -> TType list

/// Given a method, which may be generic, make new inference type variables for
/// its generic parameters, and ensure that the constraints the new type variables are adjusted.
///
/// Returns the inference type variables as a list of types.
val FreshenMethInfo: range -> MethInfo -> TType list

/// Given a set of formal type parameters and their constraints, make new inference type variables for
/// each and ensure that the constraints on the new type variables are adjusted to refer to these.
///
/// Returns
///   1. the new type parameters
///   2. the instantiation mapping old type parameters to inference variables
///   3. the inference type variables as a list of types.
val FreshenAndFixupTypars:
    g: TcGlobals ->
    m: range ->
    rigid: TyparRigidity ->
    fctps: Typars ->
    tinst: TType list ->
    tpsorig: Typar list ->
        Typar list * TyparInstantiation * TTypes

/// Given a set of type parameters, make new inference type variables for
/// each and ensure that the constraints on the new type variables are adjusted.
///
/// Returns
///   1. the new type parameters
///   2. the instantiation mapping old type parameters to inference variables
///   3. the inference type variables as a list of types.
val FreshenTypeInst: g: TcGlobals -> m: range -> tpsorig: Typar list -> Typar list * TyparInstantiation * TTypes

/// Resolve a long identifier to a namespace, module.
val internal ResolveLongIdentAsModuleOrNamespace:
    sink: TcResultsSink ->
    amap: ImportMap ->
    m: range ->
    first: bool ->
    fullyQualified: FullyQualifiedFlag ->
    nenv: NameResolutionEnv ->
    ad: AccessorDomain ->
    id: Ident ->
    rest: Ident list ->
    isOpenDecl: bool ->
    notifySink: ShouldNotifySink ->
        ResultOrException<(int * ModuleOrNamespaceRef * ModuleOrNamespaceType) list>

/// Resolve a long identifier to an object constructor.
val internal ResolveObjectConstructor:
    ncenv: NameResolver -> denv: DisplayEnv -> m: range -> ad: AccessorDomain -> ty: TType -> ResultOrException<Item>

/// Resolve a long identifier using type-qualified name resolution.
val internal ResolveLongIdentInType:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    nenv: NameResolutionEnv ->
    lookupKind: LookupKind ->
    m: range ->
    ad: AccessorDomain ->
    id: Ident ->
    findFlag: FindMemberFlag ->
    typeNameResInfo: TypeNameResolutionInfo ->
    ty: TType ->
        Item * Ident list

/// Resolve a long identifier when used in a pattern.
val internal ResolvePatternLongIdent:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    warnOnUpper: WarnOnUpperFlag ->
    newDef: bool ->
    m: range ->
    ad: AccessorDomain ->
    nenv: NameResolutionEnv ->
    numTyArgsOpt: TypeNameResolutionInfo ->
    lid: Ident list ->
    extraDotAtTheEnd: ExtraDotAfterIdentifier ->
        Item

/// Resolve a long identifier representing a type name
val internal ResolveTypeLongIdentInTyconRef:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    nenv: NameResolutionEnv ->
    typeNameResInfo: TypeNameResolutionInfo ->
    ad: AccessorDomain ->
    m: range ->
    tcref: TyconRef ->
    lid: Ident list ->
        TyconRef * TypeInst

/// Resolve a long identifier to a type definition
val internal ResolveTypeLongIdent:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    occurrence: ItemOccurrence ->
    fullyQualified: FullyQualifiedFlag ->
    nenv: NameResolutionEnv ->
    ad: AccessorDomain ->
    lid: Ident list ->
    staticResInfo: TypeNameResolutionStaticArgsInfo ->
    genOk: PermitDirectReferenceToGeneratedType ->
        ResultOrException<EnclosingTypeInst * TyconRef * TypeInst>

/// Resolve a long identifier to a field
val internal ResolveField:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    nenv: NameResolutionEnv ->
    ad: AccessorDomain ->
    ty: TType ->
    mp: Ident list ->
    id: Ident ->
    allFields: Ident list ->
        FieldResolution list

/// Resolve a long identifier to a nested field
val internal ResolveNestedField:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    nenv: NameResolutionEnv ->
    ad: AccessorDomain ->
    recdTy: TType ->
    lid: Ident list ->
        Ident list * (Ident * Item) list

/// Resolve a long identifier occurring in an expression position
val internal ResolveExprLongIdent:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    m: range ->
    ad: AccessorDomain ->
    nenv: NameResolutionEnv ->
    typeNameResInfo: TypeNameResolutionInfo ->
    lid: Ident list ->
    maybeAppliedArgExpr: SynExpr option ->
        ResultOrException<EnclosingTypeInst * Item * Ident list>

val internal getRecordFieldsInScope: NameResolutionEnv -> Item list

/// Resolve a (possibly incomplete) long identifier to a list of possible class or record fields
val internal ResolvePartialLongIdentToClassOrRecdFields:
    NameResolver -> NameResolutionEnv -> range -> AccessorDomain -> string list -> bool -> bool -> Item list

/// Return the fields for the given class or record
val internal ResolveRecordOrClassFieldsOfType: NameResolver -> range -> AccessorDomain -> TType -> bool -> Item list

/// Resolve a long identifier occurring in an expression position.
val internal ResolveLongIdentAsExprAndComputeRange:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    wholem: range ->
    ad: AccessorDomain ->
    nenv: NameResolutionEnv ->
    typeNameResInfo: TypeNameResolutionInfo ->
    lid: Ident list ->
    maybeAppliedArgExpr: SynExpr option ->
        ResultOrException<EnclosingTypeInst * Item * range * Ident list * AfterResolution>

/// Resolve a long identifier occurring in an expression position, qualified by a type.
val internal ResolveExprDotLongIdentAndComputeRange:
    sink: TcResultsSink ->
    ncenv: NameResolver ->
    wholem: range ->
    ad: AccessorDomain ->
    nenv: NameResolutionEnv ->
    ty: TType ->
    lid: Ident list ->
    typeNameResInfo: TypeNameResolutionInfo ->
    findFlag: FindMemberFlag ->
    staticOnly: bool ->
    maybeAppliedArgExpr: SynExpr option ->
        Item * range * Ident list * AfterResolution

/// A generator of type instantiations used when no more specific type instantiation is known.
val FakeInstantiationGenerator: range -> Typar list -> TType list

/// Try to resolve a long identifier as type.
val TryToResolveLongIdentAsType: NameResolver -> NameResolutionEnv -> range -> string list -> TType option

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions.
val ResolvePartialLongIdent:
    ncenv: NameResolver ->
    nenv: NameResolutionEnv ->
    isApplicableMeth: (MethInfo -> TType -> bool) ->
    m: range ->
    ad: AccessorDomain ->
    plid: string list ->
    allowObsolete: bool ->
        Item list

[<RequireQualifiedAccess>]
type ResolveCompletionTargets =
    | All of (MethInfo -> TType -> bool)
    | SettablePropertiesAndFields

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions, qualified by type.
val ResolveCompletionsInType:
    NameResolver ->
    NameResolutionEnv ->
    ResolveCompletionTargets ->
    range ->
    AccessorDomain ->
    bool ->
    TType ->
        Item list

val GetVisibleNamespacesAndModulesAtPoint:
    NameResolver -> NameResolutionEnv -> FullyQualifiedFlag -> range -> AccessorDomain -> ModuleOrNamespaceRef list

val IsItemResolvable: NameResolver -> NameResolutionEnv -> range -> AccessorDomain -> string list -> Item -> bool

val TrySelectExtensionMethInfoOfILExtMem:
    range -> ImportMap -> TType -> TyconRef * MethInfo * ExtensionMethodPriority -> MethInfo option
