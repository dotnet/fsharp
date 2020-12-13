// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public FSharp.Compiler.XmlDoc

open FSharp.Compiler.Range

/// Represents collected XmlDoc lines
[<Class>]
type XmlDoc =

    new: unprocessedLines:string [] * range:range -> XmlDoc

    static member Merge: doc1:XmlDoc -> doc2:XmlDoc -> XmlDoc

    member Check: paramNamesOpt:string list option -> unit

    /// Get the lines after insertion of implicit summary tags and encoding
    member GetElaboratedXmlLines: unit -> string []

    member GetXmlText: unit -> string

    member IsEmpty: bool

    member NonEmpty: bool

    member Range: range

    /// Get the lines before insertion of implicit summary tags and encoding
    member UnprocessedLines: string []

    static member Empty: XmlDoc
  
/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector =

    new: unit -> XmlDocCollector

    member AddGrabPoint: pos:pos -> unit

    member AddXmlDocLine: line:string * range:range -> unit

    member LinesBefore: grabPointPos:pos -> (string * range) []
  
/// Represents the XmlDoc fragments as collected from the lexer during parsing
type PreXmlDoc =
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty

    static member CreateFromGrabPoint: collector:XmlDocCollector * grabPointPos:pos -> PreXmlDoc

    static member Merge: a:PreXmlDoc -> b:PreXmlDoc -> PreXmlDoc

    member ToXmlDoc: check:bool * paramNamesOpt:string list option -> XmlDoc

    static member Empty: PreXmlDoc
