// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open System
open FSharp.Compiler.Symbols

/// Assembly content type.
[<RequireQualifiedAccess>]
type public AssemblyContentType = 
/// Public assembly content only.
    | Public 
    /// All assembly content.
    | Full

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
      CleanedIdents: ShortIdents

      /// `FSharpEntity.Namespace`.
      Namespace: ShortIdents option

      /// The most narrative parent module that has `RequireQualifiedAccess` attribute.
      NearestRequireQualifiedAccessParent: ShortIdents option

      /// Parent module that has the largest scope and has `RequireQualifiedAccess` attribute.
      TopRequireQualifiedAccessParent: ShortIdents option

      /// Parent module that has `AutoOpen` attribute.
      AutoOpenParent: ShortIdents option

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

/// Provides assembly content.
module public AssemblyContent =

    /// Given a `FSharpAssemblySignature`, returns assembly content.
    val GetAssemblySignatureContent : AssemblyContentType -> FSharpAssemblySignature -> AssemblySymbol list

    /// Returns (possibly cached) assembly content.
    val GetAssemblyContent : 
          withCache: ((IAssemblyContentCache -> AssemblySymbol list) -> AssemblySymbol list)  
          -> contentType: AssemblyContentType 
          -> fileName: string option 
          -> assemblies: FSharpAssembly list 
          -> AssemblySymbol list

