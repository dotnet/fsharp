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

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type internal FSharpXmlDoc =
    /// No documentation is available
    | None
    /// The text for documentation 
    | Text of string
    /// Indicates that the text for the documentation can be found in a .xml documentation file, using the given signature key
    | XmlDocFileSignature of (*File:*) string * (*Signature:*)string

/// A single tool tip display element
//
// Note: instances of this type do not hold any references to any compiler resources.
[<RequireQualifiedAccess>]
type internal FSharpToolTipElement = 
    | None
    /// A single type, method, etc with comment.
    | Single of (* text *) string * FSharpXmlDoc
    /// A single parameter, with the parameter name.
    | SingleParameter of (* text *) string * FSharpXmlDoc * string
    /// For example, a method overload group.
    | Group of ((* text *) string * FSharpXmlDoc) list
    /// An error occurred formatting this element
    | CompositionError of string

/// Information for building a tool tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
type internal FSharpToolTipText = 
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement list  
    
[<Sealed>]
/// Represents a declaration in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListItem =
    /// Get the display name for the declaration.
    member Name : string
    /// Get the description text for the declaration. Commputing this property may require using compiler
    /// resources and may trigger execution of a type provider method to retrieve documentation.
    ///
    /// May return "Loading..." if timeout occurs
    member DescriptionText : FSharpToolTipText
    /// Get the description text, asynchronously.  Never returns "Loading...".
    member DescriptionTextAsync : Async<FSharpToolTipText>
    /// Get the glyph integer for the declaration as used by Visual Studio.
    member Glyph : int
    
[<Sealed>]
/// Represents a set of declarations in F# source code, with information attached ready for display by an editor.
/// Returned by GetDeclarations.
//
// Note: this type holds a weak reference to compiler resources. 
type internal FSharpDeclarationListInfo =
    member Items : FSharpDeclarationListItem[]

    // Implementation details used by other code in the compiler    
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * items:Item list * reactor:IReactorOperations * checkAlive:(unit -> bool) -> FSharpDeclarationListInfo
    static member internal Error : message:string -> FSharpDeclarationListInfo
    static member Empty : FSharpDeclarationListInfo


// implementation details used by other code in the compiler    
module internal ItemDescriptionsImpl = 
    val isFunction : TcGlobals -> TType -> bool
    val ParamNameAndTypesOfUnaryCustomOperation : TcGlobals -> MethInfo -> ParamNameAndType list

    val GetXmlDocSigOfEntityRef : InfoReader -> range -> EntityRef -> (string option * string) option
    val GetXmlDocSigOfScopedValRef : TcGlobals -> TyconRef -> ValRef -> (string option * string) option
    val GetXmlDocSigOfILFieldInfo : InfoReader -> range -> ILFieldInfo -> (string option * string) option
    val GetXmlDocSigOfRecdFieldInfo : RecdFieldInfo -> (string option * string) option
    val GetXmlDocSigOfUnionCaseInfo : UnionCaseInfo -> (string option * string) option
    val GetXmlDocSigOfMethInfo : InfoReader -> range -> MethInfo -> (string option * string) option
    val GetXmlDocSigOfValRef : TcGlobals -> ValRef -> (string option * string) option
    val GetXmlDocSigOfProp : InfoReader -> range -> PropInfo -> (string option * string) option
    val GetXmlDocSigOfEvent : InfoReader -> range -> EventInfo -> (string option * string) option
    val FormatDescriptionOfItem : bool -> InfoReader -> range -> DisplayEnv -> Item -> FSharpToolTipElement
    val FormatReturnTypeOfItem  : InfoReader -> range -> DisplayEnv -> Item -> string
    val RemoveDuplicateItems : TcGlobals -> Item list -> Item list
    val RemoveExplicitlySuppressed : TcGlobals -> Item list -> Item list
    val GetF1Keyword : Item -> string option
    val rangeOfItem : TcGlobals -> bool option -> Item -> range option
    val fileNameOfItem : TcGlobals -> string option -> range -> Item -> string
    val FullNameOfItem : TcGlobals -> Item -> string
    val ccuOfItem : TcGlobals -> Item -> CcuThunk option
    val mutable ToolTipFault : string option

