// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices


open System
open System.Collections.Generic

open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree

/// Assembly content type.
type public AssemblyContentType = 
/// Public assembly content only.
    | Public 
    /// All assembly content.
    | Full

/// Short identifier, i.e. an identifier that contains no dots.
type public ShortIdent = string

/// An array of `ShortIdent`.
type public Idents = ShortIdent[]

/// `ShortIdent` with a flag indicating if it's resolved in some scope.
type public MaybeUnresolvedIdent = 
    { Ident: ShortIdent; Resolved: bool }

/// Array of `MaybeUnresolvedIdent`.
type public MaybeUnresolvedIdents = MaybeUnresolvedIdent[]

/// Entity lookup type.
[<RequireQualifiedAccess>]
type public LookupType =
    | Fuzzy
    | Precise

/// Assembly path.
type public AssemblyPath = string

/// Represents type, module, member, function or value in a compiled assembly.
[<NoComparison; NoEquality>]
type public AssemblySymbol = 
    {
      /// Full entity name as it's seen in compiled code (raw FSharpEntity.FullName, FSharpValueOrFunction.FullName). 
      FullName: string

      /// Entity name parts with removed module suffixes (Ns.M1Module.M2Module.M3.entity -> Ns.M1.M2.M3.entity)
      /// and replaced compiled names with display names (FSharpEntity.DisplayName, FSharpValueOrFunction.DisplayName).
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
      Kind: LookupType -> EntityKind

      /// Cache display name and namespace, used for completion.
      UnresolvedSymbol: UnresolvedSymbol
    }

/// `RawEntity` list retrieved from an assembly.
type internal AssemblyContentCacheEntry =
    {
      /// Assembly file last write time.
      FileWriteTime: DateTime 

      /// Content type used to get assembly content.
      ContentType: AssemblyContentType 

      /// Assembly content.
      Symbols: AssemblySymbol list
    }

/// Assembly content cache.
[<NoComparison; NoEquality>]
type public IAssemblyContentCache =
    /// Try get an assembly cached content.
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option

    /// Store an assembly content.
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit

/// Thread safe wrapper over `IAssemblyContentCache`.
type public EntityCache =
    interface IAssemblyContentCache 
    new : unit -> EntityCache

    /// Clears the cache.
    member Clear : unit -> unit

    /// Performs an operation on the cache in thread safe manner.
    member Locking : (IAssemblyContentCache -> 'T) -> 'T

/// Long identifier (i.e. it may contain dots).
type public StringLongIdent = string

/// Helper data structure representing a symbol, suitable for implementing unresolved identifiers resolution code fixes.
type public Entity =
    {
      /// Full name, relative to the current scope.
      FullRelativeName: StringLongIdent

      /// Ident parts needed to append to the current ident to make it resolvable in current scope.
      Qualifier: StringLongIdent

      /// Namespace that is needed to open to make the entity resolvable in the current scope.
      Namespace: StringLongIdent option

      /// Full display name (i.e. last ident plus modules with `RequireQualifiedAccess` attribute prefixed).
      Name: StringLongIdent

      /// Last part of the entity's full name.
      LastIdent: string
    }

/// Provides assembly content.
module public AssemblyContentProvider =

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
type public ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

/// Insert open namespace context.
type public InsertContext =
    {
      /// Current scope kind.
      ScopeKind: ScopeKind

      /// Current position (F# compiler line number).
      Pos: pos
    }

/// Where open statements should be added.
type public OpenStatementInsertionPoint =
    | TopLevel
    | Nearest

/// Parse AST helpers.
module public ParsedInput =

    /// Returns `InsertContext` based on current position and symbol idents.
    val tryFindInsertionContext : 
        currentLine: int -> 
        ast: ParsedInput -> MaybeUnresolvedIdents -> 
        insertionPoint: OpenStatementInsertionPoint ->
        (( (* requiresQualifiedAccessParent: *) Idents option * (* autoOpenParent: *) Idents option * (*  entityNamespace *) Idents option * (* entity: *) Idents) -> (Entity * InsertContext)[])
    
    /// Returns `InsertContext` based on current position and symbol idents.
    val findNearestPointToInsertOpenDeclaration : currentLine: int -> ast: ParsedInput -> entity: Idents -> insertionPoint: OpenStatementInsertionPoint -> InsertContext

    /// Returns long identifier at position.
    val getLongIdentAt : ast: ParsedInput -> pos: pos -> LongIdent option

    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    val adjustInsertionPoint : getLineStr: (int -> string) -> ctx: InsertContext -> pos

[<AutoOpen>]
module public Extensions =
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