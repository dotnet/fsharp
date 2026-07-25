// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open System
open System.Collections.Generic
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Library

// Matches Roslyn's finite include-recursion guard; deep real-world docs stay far below this.
[<Literal>]
let private maxIncludeDepth = 64

// Per-document include budget: high enough for generated docs, finite to stop runaway expansion.
[<Literal>]
let private maxIncludeExpansions = 10000

let private pathComparer = StringComparer.Ordinal

/// Per-pass shared state: file cache only.
type ExpansionEnv =
    {
        FileCache: Dictionary<string, Result<XDocument, string>>
    }

let mkExpansionEnv () : ExpansionEnv =
    {
        FileCache = Dictionary<string, Result<XDocument, string>>(pathComparer)
    }

/// Roslyn-parity comment inserted when an <include> XPath is valid but matches no elements.
/// No warning is emitted in this case; the original tag is kept after the comment.
let private noMatchCommentText =
    " No matching elements were found for the following include tag "

/// Load an XML file from disk, using a per-pass shared cache.
/// The cache avoids re-reading the same file within a single doc generation pass
/// while avoiding stale data across compilations (unlike a global static cache).
let private loadXmlFile (cache: Dictionary<string, Result<XDocument, string>>) (filePath: string) : Result<XDocument, string> =
    match cache.TryGetValue(filePath) with
    | true, result -> result
    | false, _ ->
        let result =
            try
                if not (FileSystem.FileExistsShim(filePath)) then
                    Result.Error $"File not found: {filePath}"
                else
                    use stream = FileSystem.OpenFileForReadShim(filePath)

                    let settings =
                        XmlReaderSettings(DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null)

                    use reader = XmlReader.Create(stream, settings)

                    let doc =
                        XDocument.Load(reader, LoadOptions.PreserveWhitespace ||| LoadOptions.SetLineInfo)

                    Result.Ok doc
            with ex ->
                Result.Error $"Error loading file '{filePath}': {ex.Message}"

        cache[filePath] <- result
        result

/// Resolve a file path (absolute or relative to the source file), normalized via
/// GetFullPath so cycle detection uses canonical paths. GetFullFilePathInDirectoryShim
/// already passes rooted include paths through unchanged, and GetDirectoryNameShim maps an
/// empty or directory-less base file name (such as the "unknown" range sentinel) to ".",
/// i.e. the current directory.
let private resolveFilePath (baseFileName: string) (includePath: string) : string =
    FileSystem.GetFullFilePathInDirectoryShim (FileSystem.GetDirectoryNameShim baseFileName) includePath

/// Evaluate XPath and return matching elements
let private evaluateXPath (doc: XDocument) (xpath: string) : Result<XElement list, string> =
    try
        if String.IsNullOrWhiteSpace(xpath) then
            Result.Error "XPath expression is empty"
        else
            // Materialize inside the try: XPathSelectElements is lazily enumerated and throws
            // InvalidOperationException during enumeration when the result is not a set of elements
            // (for example a text or attribute node-set). Enumerating here keeps that a warning.
            Result.Ok(doc.XPathSelectElements(xpath) |> List.ofSeq)
    with ex ->
        Result.Error $"Invalid XPath expression '{xpath}': {ex.Message}"

/// Include directive information
type private IncludeInfo = { FilePath: string; XPath: string }

/// Quick check if a string might contain an include tag (no allocations)
let private mayContainInclude (text: string) : bool =
    not (String.IsNullOrEmpty(text)) && text.Contains("<include")

/// Classify an XElement as an include directive.
/// Returns Some(Ok info) for valid includes, Some(Error msg) for malformed includes, None for non-includes.
/// Only an unqualified <include> element is the documentation include tag: an element named
/// "include" in a foreign XML namespace is ordinary content and is left untouched (Roslyn parity,
/// matching its ElementNameIs check that the namespace is empty).
let private classifyInclude (elem: XElement) : Result<IncludeInfo, string> option =
    if
        elem.Name.LocalName <> "include"
        || not (String.IsNullOrEmpty elem.Name.NamespaceName)
    then
        None
    else
        let fileAttr = elem.Attribute(!!(XName.op_Implicit "file"))
        let pathAttr = elem.Attribute(!!(XName.op_Implicit "path"))

        match fileAttr, pathAttr with
        | NonNull file, NonNull path ->
            Some(
                Result.Ok
                    {
                        FilePath = file.Value
                        XPath = path.Value
                    }
            )
        | NonNull _, Null -> Some(Result.Error "<include> element is missing required 'path' attribute")
        | Null, NonNull _ -> Some(Result.Error "<include> element is missing required 'file' attribute")
        | Null, Null -> Some(Result.Error "<include> element is missing required 'file' and 'path' attributes")

/// Expansion context threaded through recursive calls
type private ExpansionContext =
    {
        Env: ExpansionEnv
        InProgressIncludes: HashSet<struct (string * string)>
        Depth: int
        Budget: int ref
        BudgetExhaustedWarned: bool ref
        Range: range
        Emit: bool
    }

let private warnIncludeError (ctx: ExpansionContext) (msg: string) =
    if ctx.Emit then
        warning (Error(FSComp.SR.xmlDocIncludeError msg, ctx.Range))

/// Warn about a failure to expand a well-formed <include> directive, naming both the
/// file and the xpath (Roslyn CS1589 parity). The frame is a localized FSComp message;
/// only the short `reason` is a passthrough argument.
let private warnFramedIncludeError (ctx: ExpansionContext) (includeInfo: IncludeInfo) (reason: string) =
    if ctx.Emit then
        warning (Error(FSComp.SR.xmlDocIncludeError2 (includeInfo.XPath, includeInfo.FilePath, reason), ctx.Range))

/// Outcome of resolving a single <include> directive.
type private IncludeOutcome =
    /// Expanded to these nodes.
    | IncludeResolved of XNode seq
    /// Valid XPath but zero matches: Roslyn parity is a comment + the kept tag, with no warning.
    | IncludeNoMatch
    /// Genuine failure (missing file, invalid/empty XPath, cycle): the short reason, framed and warned by the caller.
    | IncludeError of string
    /// The per-document expansion budget is exhausted: the short reason, warned only once per document.
    | IncludeBudgetExceeded of string

/// Load and expand includes from an external file
let rec private resolveSingleInclude (baseFileName: string) (includeInfo: IncludeInfo) (ctx: ExpansionContext) : IncludeOutcome =

    let resolvedPath =
        try
            Some(resolveFilePath baseFileName includeInfo.FilePath)
        with _ ->
            None

    match resolvedPath with
    | None -> IncludeError "the file path is invalid"
    | Some resolvedPath ->

        let key = struct (resolvedPath, includeInfo.XPath)

        if ctx.InProgressIncludes.Contains(key) then
            IncludeError "a circular include was detected"
        elif ctx.Depth >= maxIncludeDepth then
            IncludeError $"the maximum include nesting depth of {maxIncludeDepth} was exceeded"
        elif ctx.Budget.Value <= 0 then
            IncludeBudgetExceeded $"the maximum of {maxIncludeExpansions} include expansions per documentation comment was exceeded"
        else
            match
                loadXmlFile ctx.Env.FileCache resolvedPath
                |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
            with
            | Result.Error msg -> IncludeError msg
            | Result.Ok [] -> IncludeNoMatch
            | Result.Ok matchedElements ->
                ctx.Budget.Value <- ctx.Budget.Value - 1
                let childInProgress = HashSet<struct (string * string)>(ctx.InProgressIncludes)
                childInProgress.Add(key) |> ignore

                let childCtx =
                    { ctx with
                        InProgressIncludes = childInProgress
                        Depth = ctx.Depth + 1
                    }

                IncludeResolved(expandAllIncludeNodes resolvedPath (matchedElements |> Seq.cast<XNode>) childCtx)

/// Recursively expand includes in XElement nodes
and private expandAllIncludeNodes (baseFileName: string) (nodes: XNode seq) (ctx: ExpansionContext) : XNode seq =
    nodes
    |> Seq.collect (fun node ->
        if node.NodeType <> System.Xml.XmlNodeType.Element then
            Seq.singleton node
        else
            let elem = node :?> XElement

            match classifyInclude elem with
            | None ->
                let expandedChildren = expandAllIncludeNodes baseFileName (elem.Nodes()) ctx
                let newElem = XElement(elem.Name, elem.Attributes(), expandedChildren)
                Seq.singleton (newElem :> XNode)
            | Some(Result.Error msg) ->
                warnIncludeError ctx msg
                Seq.singleton node
            | Some(Result.Ok includeInfo) ->
                match resolveSingleInclude baseFileName includeInfo ctx with
                | IncludeResolved expandedNodes -> expandedNodes
                | IncludeNoMatch ->
                    // Roslyn parity: valid XPath, zero matches => comment + keep the tag, no warning.
                    seq {
                        XComment(noMatchCommentText) :> XNode
                        node
                    }
                | IncludeError reason ->
                    warnFramedIncludeError ctx includeInfo reason
                    Seq.singleton node
                | IncludeBudgetExceeded reason ->
                    // Report the document-wide budget limit at most once to avoid warning spam.
                    if not ctx.BudgetExhaustedWarned.Value then
                        ctx.BudgetExhaustedWarned.Value <- true
                        warnFramedIncludeError ctx includeInfo reason

                    Seq.singleton node)

/// Expand all <include> elements in the given elaborated XML doc lines.
/// `emit` controls whether include errors are reported as warnings (build path)
/// or suppressed (quiet validation path). Returns the input unchanged when there
/// are no include tags, when parsing fails, or when nothing was expanded.
let expandIncludeLines (env: ExpansionEnv) (emit: bool) (baseFileName: string) (range: range) (lines: string[]) : string[] =
    let hasIncludes = lines |> Array.exists mayContainInclude

    if not hasIncludes then
        lines
    else
        let text = lines |> String.concat "\n"

        let parsedRoot =
            try
                Some(
                    XElement.Parse(
                        "<__include_root__>" + text + "</__include_root__>",
                        LoadOptions.PreserveWhitespace ||| LoadOptions.SetLineInfo
                    )
                )
            with _ ->
                None

        match parsedRoot with
        | None -> lines
        | Some root ->
            let ctx =
                {
                    Env = env
                    InProgressIncludes = HashSet<struct (string * string)>()
                    Depth = 0
                    Budget = ref maxIncludeExpansions
                    BudgetExhaustedWarned = ref false
                    Range = range
                    Emit = emit
                }

            let expandedText =
                expandAllIncludeNodes baseFileName (root.Nodes()) ctx
                |> Seq.map (fun (n: XNode) -> n.ToString(SaveOptions.DisableFormatting))
                |> String.concat ""

            let expandedLines = String.getLines expandedText

            if Array.lengthsEqAndForall2 (=) expandedLines lines then
                lines
            else
                expandedLines
