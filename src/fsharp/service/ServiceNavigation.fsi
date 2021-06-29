// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.Range 
open FSharp.Compiler.SyntaxTree

/// Indicates a kind of item to show in an F# navigation bar
type public FSharpNavigationDeclarationItemKind =
    | NamespaceDecl
    | ModuleFileDecl
    | ExnDecl
    | ModuleDecl
    | TypeDecl
    | MethodDecl
    | PropertyDecl
    | FieldDecl
    | OtherDecl

[<RequireQualifiedAccess>]
type public FSharpEnclosingEntityKind =
    | Namespace
    | Module
    | Class
    | Exception
    | Interface
    | Record
    | Enum
    | DU

/// Represents an item to be displayed in the navigation bar
[<Sealed>]
type public FSharpNavigationDeclarationItem = 
    member Name : string
    member UniqueName : string
    member Glyph : FSharpGlyph
    member Kind : FSharpNavigationDeclarationItemKind
    member Range : range
    member BodyRange : range
    member IsSingleTopLevel : bool
    member EnclosingEntityKind: FSharpEnclosingEntityKind
    member IsAbstract: bool
    member Access : SynAccess option

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type public FSharpNavigationTopLevelDeclaration = 
    { Declaration : FSharpNavigationDeclarationItem
      Nested : FSharpNavigationDeclarationItem[] }
      
/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type public FSharpNavigationItems =
    member Declarations : FSharpNavigationTopLevelDeclaration[]

// Functionality to access navigable F# items.
module public FSharpNavigation =
    val internal empty : FSharpNavigationItems
    val getNavigation : ParsedInput -> FSharpNavigationItems

module public NavigateTo =
    [<RequireQualifiedAccess>]
    type NavigableItemKind =
        | Module
        | ModuleAbbreviation
        | Exception
        | Type
        | ModuleValue
        | Field
        | Property
        | Constructor
        | Member
        | EnumCase
        | UnionCase

    [<RequireQualifiedAccess>]
    type ContainerType =
        | File
        | Namespace
        | Module
        | Type
        | Exception

    type Container =
        { Type: ContainerType
          Name: string }
    
    type NavigableItem = 
        { Name: string
          Range: range
          IsSignature: bool
          Kind: NavigableItemKind
          Container: Container }

    val getNavigableItems : ParsedInput -> NavigableItem []
