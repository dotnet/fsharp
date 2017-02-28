// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices


open System
open System.Collections.Generic

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyContentType = Public | Full

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal ShortIdent = string

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Idents = ShortIdent[]

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal MaybeUnresolvedIdent = { Ident: ShortIdent; Resolved: bool }

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal MaybeUnresolvedIdents = MaybeUnresolvedIdent[]

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal IsAutoOpen = bool

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<RequireQualifiedAccess>]
type internal LookupType =
    | Fuzzy
    | Precise

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyPath = string

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
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

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal AssemblyContentCacheEntry =
    { FileWriteTime: DateTime 
      ContentType: AssemblyContentType 
      Entities: RawEntity list }

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<NoComparison; NoEquality>]
type internal IAssemblyContentCache =
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit


/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal EntityCache =
    interface IAssemblyContentCache 
    new : unit -> EntityCache
    member Clear : unit -> unit
    member Locking : (IAssemblyContentCache -> 'T) -> 'T

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal LongIdent = string

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Entity =
    { FullRelativeName: LongIdent
      Qualifier: LongIdent
      Namespace: LongIdent option
      Name: LongIdent
      LastIdent: string }

/// TODO: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
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

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<Measure>] type internal FCS

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal Point<[<Measure>]'t> = { Line : int; Column : int }

/// TODO: this type needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
type internal InsertContext =
    { ScopeKind: ScopeKind
      Pos: Point<FCS> }

/// TODO: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
module internal ParsedInput =
    /// TODO: this function needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
    val tryFindInsertionContext : currentLine: int -> ast: Ast.ParsedInput -> MaybeUnresolvedIdents -> (( (* requiresQualifiedAccessParent: *) Idents option * (* autoOpenParent: *) Idents option * (*  entityNamespace *) Idents option * (* entity: *) Idents) -> (Entity * InsertContext)[])

    /// TODO: this function needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
    val getLongIdentAt : ast: Ast.ParsedInput -> pos: Range.pos -> Ast.LongIdent option

[<AutoOpen>]
module internal Extensions =
    type FSharpEntity with
        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullName : unit -> string option

        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullDisplayName : unit -> string option

        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullCompiledName : unit -> string option

        /// TODO: this property needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member PublicNestedEntities : seq<FSharpEntity>

        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetMembersFunctionsAndValues : IList<FSharpMemberOrFunctionOrValue>

    type FSharpMemberOrFunctionOrValue with
        /// TODO: this property needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member FullTypeSafe : FSharpType option

        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullDisplayName : unit -> string option

        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetFullCompiledOperatorNameIdents : unit -> Idents option 

    type FSharpAssemblySignature with
        /// TODO: this method needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
        member TryGetEntities : unit -> seq<FSharpEntity>

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Array =
        /// Returns a new array with an element replaced with a given value.
        val replace : index: int ->  value: 'T -> array: 'T[] -> 'T[]

/// TODO: this module needs to be cleaned up and documented to be a proper part of the FSharp.Compiler.Service API
[<AutoOpen>]
module internal Utils =
    val hasAttribute<'T> : attributes: seq<FSharpAttribute> -> bool
