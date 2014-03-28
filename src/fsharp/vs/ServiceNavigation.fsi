// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 

type internal DeclarationItemKind =
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
type internal DeclarationItem = 
    member Name : string
    member UniqueName : string
    member Glyph : int
    member Kind : DeclarationItemKind
    member Range : Range
    member BodyRange : Range
    member IsSingleTopLevel : bool

/// Represents top-level declarations (that should be in the type drop-down)
/// with nested declarations (that can be shown in the member drop-down)
[<NoEquality; NoComparison>]
type internal TopLevelDeclaration = 
    { Declaration : DeclarationItem
      Nested : DeclarationItem[] }
      
/// Represents result of 'GetNavigationItems' operation - this contains
/// all the members and currently selected indices. First level correspond to
/// types & modules and second level are methods etc.
[<Sealed>]
type internal NavigationItems =
    member Declarations : TopLevelDeclaration[]

// implementation details used by other code in the compiler    
module internal NavigationImpl =
    val internal getNavigationFromImplFile : Ast.SynModuleOrNamespace list -> NavigationItems
    val internal empty : NavigationItems
