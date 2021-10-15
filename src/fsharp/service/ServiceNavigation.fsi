// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

/// Indicates a kind of item to show in an F# navigation bar
[<RequireQualifiedAccess>]
type public NavigationItemKind =
    | Namespace
    | ModuleFile
    | Exception
    | Module
    | Type
    | Method
    | Property
    | Field
    | Other

[<RequireQualifiedAccess>]
type public NavigationEntityKind =
    | Namespace
    | Module
    | Class
    | Exception
    | Interface
    | Record
    | Enum
    | Union

/// Represents an item to be displayed in the navigation bar
[<Sealed>]
type public NavigationItem = 
    member Name: string

    member UniqueName: string

    member Glyph: FSharpGlyph

    member Kind: NavigationItemKind

    member Range: range

    member BodyRange: range

    member IsSingleTopLevel: bool

    member EnclosingEntityKind: NavigationEntityKind

    member IsAbstract: bool

    member Access: SynAccess option

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type public NavigationTopLevelDeclaration = 
    { Declaration: NavigationItem
      Nested: NavigationItem[] }
      
/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type public NavigationItems =
    member Declarations: NavigationTopLevelDeclaration[]

// Functionality to access navigable F# items.
module public Navigation =
    val internal empty: NavigationItems
    val getNavigation: ParsedInput -> NavigationItems

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
type NavigableContainerType =
    | File
    | Namespace
    | Module
    | Type
    | Exception

type NavigableContainer =
    { Type: NavigableContainerType
      Name: string }
    
type NavigableItem = 
    { Name: string
      Range: range
      IsSignature: bool
      Kind: NavigableItemKind
      Container: NavigableContainer }

module public NavigateTo =
    val GetNavigableItems: ParsedInput -> NavigableItem []
