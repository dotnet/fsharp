// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 

/// Indicates a kind of item to show in an F# navigation bar
type internal FSharpNavigationDeclarationItemKind =
    | NamespaceDecl
    | ModuleFileDecl
    | ExnDecl
    | ModuleDecl
    | TypeDecl
    | MethodDecl
    | PropertyDecl
    | FieldDecl
    | OtherDecl

/// Represents an item to be displayed in the navigation bar
[<Sealed>]
type internal FSharpNavigationDeclarationItem = 
    member Name : string
    member UniqueName : string
    member Glyph : int
    member Kind : FSharpNavigationDeclarationItemKind
    member Range : Range.range
    member BodyRange : Range.range
    member IsSingleTopLevel : bool

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type internal FSharpNavigationTopLevelDeclaration = 
    { Declaration : FSharpNavigationDeclarationItem
      Nested : FSharpNavigationDeclarationItem[] }
      
/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type internal FSharpNavigationItems =
    member Declarations : FSharpNavigationTopLevelDeclaration[]

// implementation details used by other code in the compiler    
module internal NavigationImpl =
    val internal getNavigationFromImplFile : Ast.SynModuleOrNamespace list -> FSharpNavigationItems
    val internal empty : FSharpNavigationItems

