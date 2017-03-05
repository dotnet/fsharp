// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops

[<Sealed>]
/// Represents a declaration in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListItem =
    /// Get the display name for the declaration.
    member Name : string
    /// Get the name for the declaration as it's presented in source code.
    member NameInCode : string
    /// Get the description text for the declaration. Commputing this property may require using compiler
    /// resources and may trigger execution of a type provider method to retrieve documentation.
    ///
    /// May return "Loading..." if timeout occurs
    member StructuredDescriptionText : FSharpStructuredToolTipText
    member DescriptionText : FSharpToolTipText

    /// Get the description text, asynchronously.  Never returns "Loading...".
    member StructuredDescriptionTextAsync : Async<FSharpStructuredToolTipText>
    member DescriptionTextAsync : Async<FSharpToolTipText>
    /// Get the glyph integer for the declaration as used by Visual Studio.
    member Glyph : int
    member GlyphMajor : ItemDescriptionIcons.GlyphMajor
    member GlyphMinor : ItemDescriptionIcons.GlyphMinor
    member IsAttribute : bool
    member Accessibility : FSharpAccessibility option

[<Sealed>]
/// Represents a set of declarations in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListInfo =
    member Items : FSharpDeclarationListItem[]

    // Implementation details used by other code in the compiler    
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * ccu:CcuThunk * tcImports:CompileOps.TcImports * items:Item list * reactor:IReactorOperations * checkAlive:(unit -> bool) -> FSharpDeclarationListInfo
    static member internal Error : message:string -> FSharpDeclarationListInfo
    static member Empty : FSharpDeclarationListInfo