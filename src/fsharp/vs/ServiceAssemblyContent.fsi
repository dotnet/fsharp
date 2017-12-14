// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices


open System
open System.Collections.Generic

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range

/// Assembly content type.
#if COMPILER_PUBLIC_API
type AssemblyContentType = 
#else
type internal AssemblyContentType = 
#endif
/// Public assembly content only.
    | Public 
    /// All assembly content.
    | Full

/// Short identifier, i.e. an identifier that contains no dots.
#if COMPILER_PUBLIC_API
type ShortIdent = string
#else
type internal ShortIdent = string
#endif

/// An array of `ShortIdent`.
#if COMPILER_PUBLIC_API
type Idents = ShortIdent[]
#else
type internal Idents = ShortIdent[]
#endif

/// `ShortIdent` with a flag indicating if it's resolved in some scope.
#if COMPILER_PUBLIC_API
type MaybeUnresolvedIdent = 
#else
type internal MaybeUnresolvedIdent = 
#endif
    { Ident: ShortIdent; Resolved: bool }

/// Array of `MaybeUnresolvedIdent`.
#if COMPILER_PUBLIC_API
type MaybeUnresolvedIdents = MaybeUnresolvedIdent[]
#else
type internal MaybeUnresolvedIdents = MaybeUnresolvedIdent[]
#endif

/// Entity lookup type.
[<RequireQualifiedAccess>]
#if COMPILER_PUBLIC_API
type LookupType =
#else
type internal LookupType =
#endif
    | Fuzzy
    | Precise

/// Assembly path.
#if COMPILER_PUBLIC_API
type AssemblyPath = string
#else
type internal AssemblyPath = string
#endif

/// Represents type, module, member, function or value in a compiled assembly.
[<NoComparison; NoEquality>]
#if COMPILER_PUBLIC_API
type AssemblySymbol = 
#else
type internal AssemblySymbol = 
#endif
    { /// Full entity name as it's seen in compiled code (raw FSharpEntity.FullName, FSharpValueOrFunction.FullName). 
      FullName: string
      /// Entity name parts with removed module suffixes (Ns.M1Module.M2Module.M3.entity -> Ns.M1.M2.M3.entity)
      /// and replaced compiled names with display names (FSharpEntity.DisplayName, FSharpValueOrFucntion.DisplayName).
      /// Note: *all* parts are cleaned, not the last one. 
      CleanedIdents: Idents
      /// `FSharpEntity.Namespace`.
      Namespace: Idents option
      /// The most narrative parent module that has `RequireQualifiedAccess` attribute.
      NearestRequireQualifiedAccessParent: Idents option
      /// Parent module that has the largest scope and has `RequireQualifiedAccess` attribute.
      TopRequireQualifiedAccessParent: Idents option
      /// Parent module that has `AutoOpen` attribute.
      AutoOpenParent: Idents option
      Symbol: FSharpSymbol
      /// Function that returns `EntityKind` based of given `LookupKind`.
      Kind: LookupType -> EntityKind }

/// `RawEntity` list retrieved from an assembly.
type internal AssemblyContentCacheEntry =
    { /// Assembly file last write time.
      FileWriteTime: DateTime 
      /// Content type used to get assembly content.
      ContentType: AssemblyContentType 
      /// Assembly content.
      Symbols: AssemblySymbol list }

/// Assembly content cache.
[<NoComparison; NoEquality>]
#if COMPILER_PUBLIC_API
type IAssemblyContentCache =
#else
type internal IAssemblyContentCache =
#endif
    /// Try get an assembly cached content.
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option
    /// Store an assembly content.
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit

/// Thread safe wrapper over `IAssemblyContentCache`.
#if COMPILER_PUBLIC_API
type EntityCache =
#else
type internal EntityCache =
#endif
    interface IAssemblyContentCache 
    new : unit -> EntityCache
    /// Clears the cache.
    member Clear : unit -> unit
    /// Performs an operation on the cache in thread safe manner.
    member Locking : (IAssemblyContentCache -> 'T) -> 'T

/// Lond identifier (i.e. it may contain dots).
#if COMPILER_PUBLIC_API
type StringLongIdent = string
#else
type internal StringLongIdent = string
#endif

/// Helper data structure representing a symbol, sutable for implementing unresolved identifiers resolution code fixes.
#if COMPILER_PUBLIC_API
type Entity =
#else
type internal Entity =
#endif
    { /// Full name, relative to the current scope.
      FullRelativeName: StringLongIdent
      /// Ident parts needed to append to the current ident to make it resolvable in current scope.
      Qualifier: StringLongIdent
      /// Namespace that is needed to open to make the entity resolvable in the current scope.
      Namespace: StringLongIdent option
      /// Full display name (i.e. last ident plus modules with `RequireQualifiedAccess` attribute prefixed).
      Name: StringLongIdent
      /// Last part of the entity's full name.
      LastIdent: string }

/// Provides assembly content.
#if COMPILER_PUBLIC_API
module AssemblyContentProvider =
#else
module internal AssemblyContentProvider =
#endif
    /// Given a `FSharpAssemblySignature`, returns assembly content.
    val getAssemblySignatureContent : AssemblyContentType -> FSharpAssemblySignature -> AssemblySymbol list

    /// Returns (possibly cached) assembly content.
    val getAssemblyContent : 
             withCache: ((IAssemblyContentCache -> AssemblySymbol list) -> AssemblySymbol list)  
          -> contentType: AssemblyContentType 
          -> fileName: string option 
          -> assemblies: FSharpAssembly list 
          -> AssemblySymbol list

/// Kind of lexical scope.
#if COMPILER_PUBLIC_API
type ScopeKind =
#else
type internal ScopeKind =
#endif
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

/// Insert open namespace context.
#if COMPILER_PUBLIC_API
type InsertContext =
#else
type internal InsertContext =
#endif
    { /// Current scope kind.
      ScopeKind: ScopeKind
      /// Current position (F# compiler line number).
      Pos: pos }

/// Where open statements should be added.
#if COMPILER_PUBLIC_API
type OpenStatementInsertionPoint =
#else
type internal OpenStatementInsertionPoint =
#endif
    | TopLevel
    | Nearest

/// Parse AST helpers.
#if COMPILER_PUBLIC_API
module ParsedInput =
#else
module internal ParsedInput =
#endif

    /// Returns `InsertContext` based on current position and symbol idents.
    val tryFindInsertionContext : 
        currentLine: int -> 
        ast: Ast.ParsedInput -> MaybeUnresolvedIdents -> 
        insertionPoint: OpenStatementInsertionPoint ->
        (( (* requiresQualifiedAccessParent: *) Idents option * (* autoOpenParent: *) Idents option * (*  entityNamespace *) Idents option * (* entity: *) Idents) -> (Entity * InsertContext)[])
    
    /// Returns `InsertContext` based on current position and symbol idents.
    val tryFindNearestPointToInsertOpenDeclaration : currentLine: int -> ast: Ast.ParsedInput -> entity: Idents -> insertionPoint: OpenStatementInsertionPoint -> InsertContext option

    /// Returns lond identifier at position.
    val getLongIdentAt : ast: Ast.ParsedInput -> pos: Range.pos -> Ast.LongIdent option

    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    val adjustInsertionPoint : getLineStr: (int -> string) -> ctx: InsertContext -> pos

[<AutoOpen>]
#if COMPILER_PUBLIC_API
module Extensions =
#else
module internal Extensions =
#endif
    type FSharpEntity with
        /// Safe version of `FullName`.
        member TryGetFullName : unit -> string option

        /// Safe version of `DisplayName`.
        member TryGetFullDisplayName : unit -> string option

        /// Safe version of `CompiledName`.
        member TryGetFullCompiledName : unit -> string option

        /// Public nested entities (methods, functions, values, nested modules).
        member PublicNestedEntities : seq<FSharpEntity>

        /// Safe version of `GetMembersFunctionsAndValues`.
        member TryGetMembersFunctionsAndValues : IList<FSharpMemberOrFunctionOrValue>

    type FSharpMemberOrFunctionOrValue with
        /// Safe version of `FullType`.
        member FullTypeSafe : FSharpType option

        /// Full name with last part replaced with display name.
        member TryGetFullDisplayName : unit -> string option

        /// Full operator compiled name.
        member TryGetFullCompiledOperatorNameIdents : unit -> Idents option 

    type FSharpAssemblySignature with
        /// Safe version of `Entities`.
        member TryGetEntities : unit -> seq<FSharpEntity>