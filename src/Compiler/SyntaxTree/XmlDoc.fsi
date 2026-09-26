// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Xml

open FSharp.Compiler.Text

/// The tag an XML doc attribute value belongs to
[<RequireQualifiedAccess>]
type XmlDocRefKind =
    | Param
    | ParamRef
    | TypeParam
    | TypeParamRef
    | Cref

/// An attribute value in an XML doc that names something else: a `name` or a `cref`
[<Struct>]
type XmlDocRef =
    {
        Kind: XmlDocRefKind
        /// The attribute value as written
        Text: string
        /// The source range of the value, without its quotes
        Range: range
    }

/// Represents collected XmlDoc lines
[<Class>]
type public XmlDoc =

    new: unprocessedLines: string[] * range: range -> XmlDoc

    /// Lines with their source ranges; one range per line, or none when the doc did not come from source
    new: unprocessedLines: string[] * lineRanges: range[] * range: range -> XmlDoc

    /// Merge two XML documentation
    static member Merge: doc1: XmlDoc -> doc2: XmlDoc -> XmlDoc

    /// Check the XML documentation
    member internal Check: paramNamesOpt: string list option -> unit

    /// Get the lines after insertion of implicit summary tags and encoding
    member GetElaboratedXmlLines: unit -> string[]

    /// Get the elaborated XML documentation as XML text
    member GetXmlText: unit -> string

    /// Get the elaborated XML documentation as XML text after expanding includes
    member internal GetExpandedXmlText: emit: bool -> string

    /// Get the elaborated XML documentation as XML text after expanding includes
    member internal GetExpandedXmlText: emit: bool * env: XmlDocIncludeExpander.ExpansionEnv -> string

    /// Indicates if the XmlDoc is empty
    member IsEmpty: bool

    /// Indicates if the XmlDoc is non-empty
    member NonEmpty: bool

    /// Indicates the overall original source range of the XmlDoc
    member Range: range

    /// The source range of each unprocessed line; empty when the doc did not come from source
    member LineRanges: range[]

    /// The `name` and `cref` attribute values of `param`, `paramref`, `typeparam`, `typeparamref`,
    /// `see`, `seealso`, `exception` and `permission` tags, with their source ranges. Empty without line ranges.
    member GetRefs: unit -> XmlDocRef[]

    /// Get the lines before insertion of implicit summary tags and encoding
    member UnprocessedLines: string[]

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
    member LinesBefore: grabPointPos: pos -> (string * range)[]

    /// Indicates it the given point has XmlDoc comments
    member HasComments: grabPointPos: pos -> bool

    /// Check if XmlDoc comments are at invalid positions, and if so report them
    member CheckInvalidXmlDocPositions: unit -> range list

    member SetLastNonCommentTokenLine: int -> unit

    member LastNonCommentTokenLine: int

/// Represents the XmlDoc fragments as collected from the lexer during parsing
[<Sealed>]
type public PreXmlDoc =

    static member internal CreateFromGrabPoint: collector: XmlDocCollector * grabPointPos: pos -> PreXmlDoc

    /// Merge two PreXmlDoc
    static member Merge: a: PreXmlDoc -> b: PreXmlDoc -> PreXmlDoc

    /// Wrap a PreXmlDoc with additional parameter names that should be considered valid
    /// when the doc is checked. Used for property get/set pairs so that each accessor's
    /// xmldoc validation sees the union of both accessors' parameter names.
    static member WithExtraParamsForCheck: doc: PreXmlDoc * extraParamNames: string list -> PreXmlDoc

    /// Create a PreXmlDoc from a collection of unprocessed lines
    static member Create: unprocessedLines: string[] * range: range -> PreXmlDoc

    /// Process and check the PreXmlDoc, checking with respect to the given parameter names
    member ToXmlDoc: check: bool * paramNamesOpt: string list option -> XmlDoc

    /// Get the overall range of the PreXmlDoc
    member Range: Range

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
