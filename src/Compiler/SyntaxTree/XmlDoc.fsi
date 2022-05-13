// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Xml

open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL

/// Represents collected XmlDoc lines
[<Class>]
type public XmlDoc =

    new: unprocessedLines: string [] * range: range -> XmlDoc

    /// Merge two XML documentation
    static member Merge: doc1: XmlDoc -> doc2: XmlDoc -> XmlDoc

    /// Check the XML documentation
    member internal Check: paramNamesOpt: string list option -> unit

    /// Get the lines after insertion of implicit summary tags and encoding
    member GetElaboratedXmlLines: unit -> string []

    /// Get the elaborated XML documentation as XML text
    member GetXmlText: unit -> string

    /// Indicates if the XmlDoc is empty
    member IsEmpty: bool

    /// Indicates if the XmlDoc is non-empty
    member NonEmpty: bool

    /// Indicates the overall original source range of the XmlDoc
    member Range: range

    /// Get the lines before insertion of implicit summary tags and encoding
    member UnprocessedLines: string []

    /// Get the empty XmlDoc
    static member Empty: XmlDoc

/// Used to collect XML documentation during lexing and parsing.
type internal XmlDocCollector =

    /// Create a fresh XmlDocCollector
    new: unit -> XmlDocCollector

    /// Add a point where prior XmlDoc are collected
    member AddGrabPoint: pos: pos -> unit

    /// Indicate the next XmlDoc will act as a point where prior XmlDoc are collected
    member AddGrabPointDelayed: pos: pos -> unit

    /// Add a line of XmlDoc text
    member AddXmlDocLine: line: string * range: range -> unit

    /// Get the documentation lines before the given point
    member LinesBefore: grabPointPos: pos -> (string * range) []

    /// Indicates it the given point has XmlDoc comments
    member HasComments: grabPointPos: pos -> bool

    /// Check if XmlDoc comments are at invalid positions, and if so report them
    member CheckInvalidXmlDocPositions: unit -> range list

/// Represents the XmlDoc fragments as collected from the lexer during parsing
[<Sealed>]
type public PreXmlDoc =

    static member internal CreateFromGrabPoint: collector: XmlDocCollector * grabPointPos: pos -> PreXmlDoc

    /// Merge two PreXmlDoc
    static member Merge: a: PreXmlDoc -> b: PreXmlDoc -> PreXmlDoc

    /// Create a PreXmlDoc from a collection of unprocessed lines
    static member Create: unprocessedLines: string [] * range: range -> PreXmlDoc

    /// Process and check the PreXmlDoc, checking with respect to the given parameter names
    member ToXmlDoc: check: bool * paramNamesOpt: string list option -> XmlDoc

    /// Get the overall range of the PreXmlDoc
    member internal Range: Range

    /// Indicates if the PreXmlDoc is non-empty
    member IsEmpty: bool

    /// Mark the PreXmlDoc as invalid
    member internal MarkAsInvalid: unit -> unit

    /// Get the empty PreXmlDoc
    static member Empty: PreXmlDoc

/// Represents access to an XmlDoc file
[<Sealed>]
type internal XmlDocumentationInfo =

    /// Look up an item in the XmlDoc file
    member TryGetXmlDocBySig: xmlDocSig: string -> XmlDoc option

    /// Create an XmlDocumentationInfo from a file
    static member TryCreateFromFile: xmlFileName: string -> XmlDocumentationInfo option

/// Represents a capability to access XmlDoc files
type internal IXmlDocumentationInfoLoader =

    /// Try to get the XmlDocumentationInfo for a file
    abstract TryLoad: assemblyFileName: string -> XmlDocumentationInfo option
