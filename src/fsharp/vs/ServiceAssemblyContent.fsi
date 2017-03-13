// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices


open System
open System.Collections.Generic

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyContentType = Public | Full

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal ShortIdent = string

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Idents = ShortIdent[]

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal MaybeUnresolvedIdent = { Ident: ShortIdent; Resolved: bool }

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal MaybeUnresolvedIdents = MaybeUnresolvedIdent[]

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal IsAutoOpen = bool

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<RequireQualifiedAccess>]
type internal LookupType =
    | Fuzzy
    | Precise

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyPath = string

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<NoComparison; NoEquality>]
type internal RawEntity = 
    { /// Full entity name as it's seen in compiled code (raw FSharpEntity.FullName, FSharpValueOrFunction.FullName). 
      FullName: string
      /// Entity name parts with removed module suffixes (Ns.M1Module.M2Module.M3.entity -> Ns.M1.M2.M3.entity)
      /// and replaced compiled names with display names (FSharpEntity.DisplayName, FSharpValueOrFucntion.DisplayName).
      /// Note: *all* parts are cleaned, not the last one. 
      CleanedIdents: Idents
      Namespace: Idents option
      IsPublic: bool
      TopRequireQualifiedAccessParent: Idents option
      AutoOpenParent: Idents option
      Kind: LookupType -> EntityKind }

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyContentCacheEntry =
    { FileWriteTime: DateTime 
      ContentType: AssemblyContentType 
      Entities: RawEntity list }

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<NoComparison; NoEquality>]
type internal IAssemblyContentCache =
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit


/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal EntityCache =
    interface IAssemblyContentCache 
    new : unit -> EntityCache
    member Clear : unit -> unit
    member Locking : (IAssemblyContentCache -> 'T) -> 'T

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal LongIdent = string

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Entity =
    { FullRelativeName: LongIdent
      Qualifier: LongIdent
      Namespace: LongIdent option
      Name: LongIdent
      LastIdent: string }

/// API CLEANUP: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
module internal AssemblyContentProvider =
    val getAssemblySignatureContent : 
          contentType: AssemblyContentType 
          -> signature: FSharpAssemblySignature
          -> RawEntity list

    val getAssemblyContent : 
          withCache: ((IAssemblyContentCache -> RawEntity list) -> RawEntity list)  
          -> contentType: AssemblyContentType 
          -> fileName: string option 
          -> assemblies: FSharpAssembly list 
          -> RawEntity list

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<Measure>] type internal FCS

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Point<[<Measure>]'t> = { Line : int; Column : int }

/// API CLEANUP: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal InsertContext =
    { ScopeKind: ScopeKind
      Pos: Point<FCS> }

/// API CLEANUP: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
module internal ParsedInput =
    /// API CLEANUP: this function needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
    val tryFindInsertionContext : currentLine: int -> ast: Ast.ParsedInput -> MaybeUnresolvedIdents -> (( (* requiresQualifiedAccessParent: *) Idents option * (* autoOpenParent: *) Idents option * (*  entityNamespace *) Idents option * (* entity: *) Idents) -> (Entity * InsertContext)[])

    /// API CLEANUP: this function needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
    val getLongIdentAt : ast: Ast.ParsedInput -> pos: Range.pos -> Ast.LongIdent option

[<AutoOpen>]
module internal Extensions =
    type FSharpEntity with
        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullName : unit -> string option

        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullDisplayName : unit -> string option

        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullCompiledName : unit -> string option

        /// API CLEANUP: this property needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member PublicNestedEntities : seq<FSharpEntity>

        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetMembersFunctionsAndValues : IList<FSharpMemberOrFunctionOrValue>

    type FSharpMemberOrFunctionOrValue with
        /// API CLEANUP: this property needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member FullTypeSafe : FSharpType option

        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullDisplayName : unit -> string option

        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullCompiledOperatorNameIdents : unit -> Idents option 

    type FSharpAssemblySignature with
        /// API CLEANUP: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetEntities : unit -> seq<FSharpEntity>

/// API CLEANUP: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<AutoOpen>]
module internal Utils =
    val hasAttribute<'T> : attributes: seq<FSharpAttribute> -> bool
