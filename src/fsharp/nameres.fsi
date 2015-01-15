// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Nameres

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Outcome
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.PrettyNaming



/// A NameResolver is a context for name resolution. It primarily holds an InfoReader.
type NameResolver =
    new : g:TcGlobals * amap:ImportMap * infoReader:InfoReader * instantiationGenerator:(range -> Typars -> TypeInst) -> NameResolver
    member InfoReader : InfoReader
    member amap : ImportMap
    member g : TcGlobals

[<NoEquality; NoComparison; RequireQualifiedAccess>]
/// Represents the item with which a named argument is associated.
type ArgumentContainer =
    /// The named argument is an argument of a method
    | Method of MethInfo
    /// The named argument is a static parameter to a provided type or a parameter to an F# exception constructor
    | Type of TyconRef
    /// The named argument is a static parameter to a union case constructor
    | UnionCase of UnionCaseInfo

[<NoEquality; NoComparison; RequireQualifiedAccess>]
/// Represents an item that results from name resolution
type Item = 
  // These exist in the "eUnqualifiedItems" List.map in the type environment. 
  | Value of  ValRef
  | UnionCase of UnionCaseInfo
  | ActivePatternResult of ActivePatternInfo * TType * int  * range
  | ActivePatternCase of ActivePatternElemRef 
  | ExnCase of TyconRef 
  | RecdField of RecdFieldInfo
  | NewDef of Ident
  | ILField of ILFieldInfo
  | Event of EventInfo
  | Property of string * PropInfo list
  | MethodGroup of string * MethInfo list
  | CtorGroup of string * MethInfo list
  | FakeInterfaceCtor of TType
  | DelegateCtor of TType
  | Types of string * TType list
  /// CustomOperation(operationName, operationHelpText, operationImplementation).
  /// 
  /// Used to indicate the availability or resolution of a custom query operation such as 'sortBy' or 'where' in computation expression syntax
  | CustomOperation of string * (unit -> string option) * MethInfo option
  | CustomBuilder of string * ValRef
  | TypeVar of string 
  | ModuleOrNamespaces of Tast.ModuleOrNamespaceRef list
  /// Represents the resolution of a source identifier to an implicit use of an infix operator (+solution if such available)
  | ImplicitOp of Ident * TraitConstraintSln option ref
  /// Represents the resolution of a source identifier to a named argument
  | ArgName of Ident * TType * ArgumentContainer option
  | SetterArg of Ident * Item 
  | UnqualifiedType of TyconRef list
  member DisplayName : TcGlobals -> string


/// Information about an extension member held in the name resolution environment
[<Sealed>]
type ExtensionMember 

/// The environment of information used to resolve names
[<NoEquality; NoComparison>]
type NameResolutionEnv =
    {eDisplayEnv: DisplayEnv
     eUnqualifiedItems: LayeredMap<string,Item>
     ePatItems: NameMap<Item>
     eModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef>
     eFullyQualifiedModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef>
     eFieldLabels: NameMultiMap<RecdFieldRef>
     eTyconsByAccessNames: LayeredMultiMap<string,TyconRef>
     eFullyQualifiedTyconsByAccessNames: LayeredMultiMap<string,TyconRef>
     eTyconsByDemangledNameAndArity: LayeredMap<NameArityPair,TyconRef>
     eFullyQualifiedTyconsByDemangledNameAndArity: LayeredMap<NameArityPair,TyconRef>
     eIndexedExtensionMembers: TyconRefMultiMap<ExtensionMember>
     eUnindexedExtensionMembers: ExtensionMember list
     eTypars: NameMap<Typar> }
    static member Empty : g:TcGlobals -> NameResolutionEnv
    member DisplayEnv : DisplayEnv
    member FindUnqualifiedItem : string -> Item

type FullyQualifiedFlag =
  | FullyQualified
  | OpenQualified

[<RequireQualifiedAccess>]
type BulkAdd = Yes | No

/// Add extra items to the environment for Visual Studio, e.g. static members 
val internal AddFakeNamedValRefToNameEnv : string -> NameResolutionEnv -> ValRef -> NameResolutionEnv

/// Add some extra items to the environment for Visual Studio, e.g. record members
val internal AddFakeNameToNameEnv : string -> NameResolutionEnv -> Item -> NameResolutionEnv

/// Add a single F# value to the environment.
val internal AddValRefToNameEnv                    : NameResolutionEnv -> ValRef -> NameResolutionEnv

/// Add active pattern result tags to the environment.
val internal AddActivePatternResultTagsToNameEnv   : ActivePatternInfo -> NameResolutionEnv -> TType -> range -> NameResolutionEnv

/// Add a list of type definitions to the name resolution environment 
val internal AddTyconRefsToNameEnv                 : BulkAdd -> bool -> TcGlobals -> ImportMap -> range -> bool -> NameResolutionEnv -> TyconRef list -> NameResolutionEnv

/// Add an F# exception definition to the name resolution environment 
val internal AddExceptionDeclsToNameEnv            : BulkAdd -> NameResolutionEnv -> TyconRef -> NameResolutionEnv

/// Add a module abbreviation to the name resolution environment 
val internal AddModuleAbbrevToNameEnv              : Ident -> NameResolutionEnv -> ModuleOrNamespaceRef list -> NameResolutionEnv

/// Add a list of module or namespace to the name resolution environment, including any sub-modules marked 'AutoOpen'
val internal AddModuleOrNamespaceRefsToNameEnv                   : TcGlobals -> ImportMap -> range -> bool -> AccessorDomain -> NameResolutionEnv -> ModuleOrNamespaceRef list -> NameResolutionEnv

/// Add a single modules or namespace to the name resolution environment
val internal AddModuleOrNamespaceRefToNameEnv                    : TcGlobals -> ImportMap -> range -> bool -> AccessorDomain -> NameResolutionEnv -> ModuleOrNamespaceRef -> NameResolutionEnv

/// Add a list of modules or namespaces to the name resolution environment
val internal AddModulesAndNamespacesContentsToNameEnv : TcGlobals -> ImportMap -> AccessorDomain -> range -> bool -> NameResolutionEnv -> ModuleOrNamespaceRef list -> NameResolutionEnv

/// A flag which indicates if it is an error to have two declared type parameters with identical names
/// in the name resolution environment.
type CheckForDuplicateTyparFlag =
  | CheckForDuplicateTypars
  | NoCheckForDuplicateTypars

/// Add some declared type parameters to the name resolution environment
val internal AddDeclaredTyparsToNameEnv : CheckForDuplicateTyparFlag -> NameResolutionEnv -> Typar list -> NameResolutionEnv

/// Qualified lookup of type names in the environment
val internal LookupTypeNameInEnvNoArity : FullyQualifiedFlag -> string -> NameResolutionEnv -> TyconRef list

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
[<Sealed;NoEquality; NoComparison>]
type TypeNameResolutionStaticArgsInfo = 
  /// Indicates definite knowledge of empty type arguments, i.e. the logical equivalent of name< >
  static member DefiniteEmpty : TypeNameResolutionStaticArgsInfo
  /// Deduce definite knowledge of type arguments
  static member FromTyArgs : numTyArgs:int -> TypeNameResolutionStaticArgsInfo

/// Represents information which guides name resolution of types.
[<NoEquality; NoComparison>]
type TypeNameResolutionInfo = 
  | TypeNameResolutionInfo of TypeNameResolutionFlag * TypeNameResolutionStaticArgsInfo
  static member Default : TypeNameResolutionInfo
  static member ResolveToTypeRefs : TypeNameResolutionStaticArgsInfo -> TypeNameResolutionInfo

/// Represents the kind of the occurence when reporting a name in name resolution
[<RequireQualifiedAccess>]
type internal ItemOccurence = 
    | Binding = 0
    | Use = 1
    | UseInType = 2
    | UseInAttribute = 3
    | Pattern = 4
  
/// An abstract type for reporting the results of name resolution and type checking
type ITypecheckResultsSink =
    abstract NotifyEnvWithScope   : range * NameResolutionEnv * AccessorDomain -> unit
    abstract NotifyExprHasType    : pos * TType * DisplayEnv * NameResolutionEnv * AccessorDomain * range -> unit
    abstract NotifyNameResolution : pos * Item * Item * ItemOccurence * DisplayEnv * NameResolutionEnv * AccessorDomain * range -> unit

/// An abstract type for reporting the results of name resolution and type checking, and which allows
/// temporary suspension and/or redirection of reporting.
type TcResultsSink = 
    { mutable CurrentSink : ITypecheckResultsSink option }
    static member NoSink : TcResultsSink
    static member WithSink : ITypecheckResultsSink -> TcResultsSink

/// Temporarily redirect reporting of name resolution and type checking results
val internal WithNewTypecheckResultsSink : ITypecheckResultsSink * TcResultsSink -> System.IDisposable

/// Temporarily suspend reporting of name resolution and type checking results
val internal TemporarilySuspendReportingTypecheckResultsToSink : TcResultsSink -> System.IDisposable

/// Report the active name resolution environment for a source range
val internal CallEnvSink                : TcResultsSink -> range * NameResolutionEnv * AccessorDomain -> unit

/// Report a specific name resolution at a source range
val internal CallNameResolutionSink     : TcResultsSink -> range * NameResolutionEnv * Item * Item * ItemOccurence * DisplayEnv * AccessorDomain -> unit

/// Report a specific name resolution at a source range
val internal CallExprHasTypeSink        : TcResultsSink -> range * NameResolutionEnv * TType * DisplayEnv * AccessorDomain -> unit

/// Get all the available properties of a type (both intrinsic and extension)
val internal AllPropInfosOfTypeInScope : InfoReader -> NameResolutionEnv -> string option * AccessorDomain -> FindMemberFlag -> range -> TType -> PropInfo list

/// Get the available methods of a type (both declared and inherited)
val internal AllMethInfosOfTypeInScope : InfoReader -> NameResolutionEnv -> string option * AccessorDomain -> FindMemberFlag -> range -> TType -> MethInfo list

/// Used to report an error condition where name resolution failed due to an indeterminate type
exception internal IndeterminateType of range

/// Used to report a warning condition for the use of upper-case identifiers in patterns
exception internal UpperCaseIdentifierInPattern of range

/// Generate a new reference to a record field with a fresh type instantiation
val FreshenRecdFieldRef :NameResolver -> Range.range -> Tast.RecdFieldRef -> Item

/// Indicates the kind of lookup being performed. Note, this type should be made private to nameres.fs.
[<RequireQualifiedAccess>]
type LookupKind =
  | RecdField
  | Pattern
  | Expr
  | Type
  | Ctor


/// Indicates if a warning should be given for the use of upper-case identifiers in patterns
type WarnOnUpperFlag =
  | WarnOnUpperCase
  | AllIdsOK

/// Indicates whether we permit a direct reference to a type generator. Only set when resolving the
/// right-hand-side of a [<Generate>] declaration.
[<RequireQualifiedAccess>]
type PermitDirectReferenceToGeneratedType = 
    | Yes 
    | No

/// Resolve a long identifier to a namespace or module.
val internal ResolveLongIndentAsModuleOrNamespace   : Import.ImportMap -> range -> FullyQualifiedFlag -> NameResolutionEnv -> AccessorDomain -> Ident list -> ResultOrException<(int * ModuleOrNamespaceRef * ModuleOrNamespaceType) list >

/// Resolve a long identifer to an object constructor.
val internal ResolveObjectConstructor               : NameResolver -> DisplayEnv -> range -> AccessorDomain -> TType -> ResultOrException<Item>

/// Resolve a long identifer using type-qualified name resolution.
val internal ResolveLongIdentInType                 : TcResultsSink -> NameResolver -> NameResolutionEnv -> LookupKind -> range -> AccessorDomain -> Ident list -> FindMemberFlag -> TypeNameResolutionInfo -> TType -> Item * Ident list

/// Resolve a long identifier when used in a pattern.
val internal ResolvePatternLongIdent                : TcResultsSink -> NameResolver -> WarnOnUpperFlag -> bool -> range -> AccessorDomain -> NameResolutionEnv -> TypeNameResolutionInfo -> Ident list -> Item

/// Resolve a long identifier representing a type name 
val internal ResolveTypeLongIdentInTyconRef         : TcResultsSink -> NameResolver -> NameResolutionEnv -> TypeNameResolutionInfo -> AccessorDomain -> range -> ModuleOrNamespaceRef -> Ident list -> TyconRef 

/// Resolve a long identifier to a type definition
val internal ResolveTypeLongIdent                   : TcResultsSink -> NameResolver -> ItemOccurence -> FullyQualifiedFlag -> NameResolutionEnv -> AccessorDomain -> Ident list -> TypeNameResolutionStaticArgsInfo -> PermitDirectReferenceToGeneratedType -> ResultOrException<TyconRef>

/// Resolve a long identifier to a field
val internal ResolveField                           : NameResolver -> NameResolutionEnv -> AccessorDomain -> TType -> Ident list * Ident -> RecdFieldRef list

/// Resolve a long identifier occurring in an expression position
val internal ResolveExprLongIdent                   : TcResultsSink -> NameResolver -> range -> AccessorDomain -> NameResolutionEnv -> TypeNameResolutionInfo -> Ident list -> Item * Ident list

/// Resolve a (possibly incomplete) long identifier to a loist of possible class or record fields
val internal ResolvePartialLongIdentToClassOrRecdFields : NameResolver -> NameResolutionEnv -> range -> AccessorDomain -> string list -> bool -> Item list

/// Return the fields for the given class or record
val internal ResolveRecordOrClassFieldsOfType       : NameResolver -> range -> AccessorDomain -> TType -> bool -> Item list

/// An adjustment to perform to the name resolution results if overload resolution fails.
/// If overload resolution succeeds, the specific overload resolution is reported. If it fails, the 
/// set of possibile overlods is reported via this adjustment.
type IfOverloadResolutionFails = IfOverloadResolutionFails of (unit -> unit)

/// Specifies if overload resolution needs to notify Language Service of overload resolution
[<RequireQualifiedAccess>]
type AfterOverloadResolution =
    /// Notfication is not needed
    |   DoNothing
    /// Notfy the sink
    |   SendToSink of (Item -> unit) * IfOverloadResolutionFails // overload resolution failure fallback
    /// Find override among given overrides and notify the sink. The 'Item' contains the candidate overrides.
    |   ReplaceWithOverrideAndSendToSink of Item * (Item -> unit) * IfOverloadResolutionFails // overload resolution failure fallback

/// Resolve a long identifier occurring in an expression position.
val internal ResolveLongIdentAsExprAndComputeRange  : TcResultsSink -> NameResolver -> range -> AccessorDomain -> NameResolutionEnv -> TypeNameResolutionInfo -> Ident list -> Item * range * Ident list * AfterOverloadResolution

/// Resolve a long identifier occurring in an expression position, qualified by a type.
val internal ResolveExprDotLongIdentAndComputeRange : TcResultsSink -> NameResolver -> range -> AccessorDomain -> NameResolutionEnv -> TType -> Ident list -> FindMemberFlag -> bool -> Item * range * Ident list * AfterOverloadResolution

/// A generator of type instantiations used when no more specific type instantiation is known.
val FakeInstantiationGenerator : range -> Typar list -> TType list

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions.
val ResolvePartialLongIdent : NameResolver -> NameResolutionEnv -> (MethInfo -> TType -> bool) -> range -> AccessorDomain -> string list -> bool -> Item list

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions, qualified by type.
val ResolveCompletionsInType       : NameResolver -> NameResolutionEnv -> (MethInfo -> TType -> bool) -> Range.range -> Infos.AccessorDomain -> bool -> TType -> Item list
