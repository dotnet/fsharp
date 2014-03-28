// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open System.Collections.Generic
open Microsoft.FSharp.Compiler.Env 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Nameres
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
//
// Note: instances of this type do not hold any references to any compiler resources.
type internal XmlComment =
    | XmlCommentNone
    | XmlCommentText of string
    | XmlCommentSignature of (*File:*) string * (*Signature:*)string

/// A single data tip display element
//
// Note: instances of this type do not hold any references to any compiler resources.
type internal DataTipElement = 
    | DataTipElementNone
    /// A single type, method, etc with comment.
    | DataTipElement of (* text *) string * XmlComment
    /// A parameter of a method.
    | DataTipElementParameter of string * XmlComment * string
    /// For example, a method overload group.
    | DataTipElementGroup of ((* text *) string * XmlComment) list
    /// An error occurred formatting this element
    | DataTipElementCompositionError of string

/// Information for building a data tip box.
//
// Note: instances of this type do not hold any references to any compiler resources.
type internal DataTipText = 
    /// A list of data tip elements to display.
    | DataTipText of DataTipElement list  
    
[<Sealed>]
// Note: this type holds a weak reference to compiler resources. 
type internal Declaration =
    member Name : string
    member DescriptionText : DataTipText
    member Glyph : int
    
[<Sealed>]
// Note: this type holds a weak reference to compiler resources. 
type internal DeclarationSet =
    member Items : Declaration[]

    // Implementation details used by other code in the compiler    
    static member internal Create : infoReader:InfoReader * m:range * denv:DisplayEnv * items:Item list * syncop:((unit->unit)->unit) * checkAlive:(unit -> bool) -> DeclarationSet
    static member internal Error : message:string -> DeclarationSet
    static member internal Empty : DeclarationSet


module internal TestHooks =
    val FormatOverloadsToListScope                   : (DataTipElement->DataTipElement) -> System.IDisposable
    
    
// implementation details used by other code in the compiler    
module internal ItemDescriptionsImpl = 
    val isFunction : TcGlobals -> TType -> bool
    val ParamNameAndTypesOfUnaryCustomOperation : TcGlobals -> MethInfo -> ParamNameAndType list
    val FormatDescriptionOfItem : bool -> InfoReader -> range -> DisplayEnv -> Item -> DataTipElement
    val FormatReturnTypeOfItem  : InfoReader -> range -> DisplayEnv -> Item -> string
    val RemoveDuplicateItems : TcGlobals -> Item list -> Item list
    val RemoveExplicitlySuppressed : TcGlobals -> Item list -> Item list
    val GetF1Keyword : Item -> string option
    val rangeOfItem : TcGlobals -> bool -> Item -> range option
    val fileNameOfItem : TcGlobals -> string option -> range -> Item -> string



