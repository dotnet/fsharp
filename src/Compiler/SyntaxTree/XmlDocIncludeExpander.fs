// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open System
open System.Collections.Generic
open System.IO
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.Xml
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Library

/// Case-insensitive path comparer for cycle detection and caching
let private pathComparer = StringComparer.OrdinalIgnoreCase

/// Cycle-detection key. The same file included via DIFFERENT XPath sections is not a cycle,
/// so the key combines the (case-folded) resolved path with the exact XPath.
let private includeKey (resolvedPath: string) (xpath: string) =
    resolvedPath.ToUpperInvariant() + "\u0000" + xpath

/// Roslyn-parity comment inserted when an <include> XPath is valid but matches no elements.
/// No warning is emitted in this case; the original tag is kept after the comment.
let private noMatchCommentText =
    " No matching elements were found for the following include tag "

/// Load an XML file from disk, using a per-expansion local cache.
/// The local cache avoids re-reading the same file within a single doc generation pass
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
                    let doc = XDocument.Load(filePath)
                    Result.Ok doc
            with ex ->
                Result.Error $"Error loading file '{filePath}': {ex.Message}"

        cache[filePath] <- result
        result

/// Resolve a file path (absolute or relative to source file).
/// Always normalizes via GetFullPath so that cycle detection uses canonical paths.
let private resolveFilePath (baseFileName: string) (includePath: string) : string =
    if Path.IsPathRooted(includePath) then
        Path.GetFullPath(includePath)
    else
        let baseDir =
            if String.IsNullOrEmpty(baseFileName) || baseFileName = "unknown" then
                Directory.GetCurrentDirectory()
            else
                match Path.GetDirectoryName(baseFileName) with
                | Null -> Directory.GetCurrentDirectory()
                | NonNull dir when String.IsNullOrEmpty(dir) -> Directory.GetCurrentDirectory()
                | NonNull dir -> dir

        Path.GetFullPath(Path.Combine(baseDir, includePath))

/// Evaluate XPath and return matching elements
let private evaluateXPath (doc: XDocument) (xpath: string) : Result<XElement seq, string> =
    try
        if String.IsNullOrWhiteSpace(xpath) then
            Result.Error "XPath expression is empty"
        else
            Result.Ok(doc.XPathSelectElements(xpath))
    with ex ->
        Result.Error $"Invalid XPath expression '{xpath}': {ex.Message}"

/// Include directive information
type private IncludeInfo = { FilePath: string; XPath: string }

/// Quick check if a string might contain an include tag (no allocations)
let private mayContainInclude (text: string) : bool =
    not (String.IsNullOrEmpty(text)) && text.Contains("<include")

/// Classify an XElement as an include directive.
/// Returns Some(Ok info) for valid includes, Some(Error msg) for malformed includes, None for non-includes.
let private classifyInclude (elem: XElement) : Result<IncludeInfo, string> option =
    if elem.Name.LocalName <> "include" then
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
        FileCache: Dictionary<string, Result<XDocument, string>>
        InProgressIncludes: HashSet<string>
        Range: range
    }

/// Outcome of resolving a single <include> directive.
type private IncludeOutcome =
    /// Expanded to these nodes.
    | IncludeResolved of XNode seq
    /// Valid XPath but zero matches: Roslyn parity is a comment + the kept tag, with no warning.
    | IncludeNoMatch
    /// Genuine failure (missing file, invalid/empty XPath, cycle): warn and keep the tag.
    | IncludeError of string

/// Load and expand includes from an external file
let rec private resolveSingleInclude (baseFileName: string) (includeInfo: IncludeInfo) (ctx: ExpansionContext) : IncludeOutcome =

    let resolvedPathResult =
        try
            Result.Ok(resolveFilePath baseFileName includeInfo.FilePath)
        with _ ->
            Result.Error $"Invalid file path: {includeInfo.FilePath}"

    match resolvedPathResult with
    | Result.Error msg -> IncludeError msg
    | Result.Ok resolvedPath ->

        let key = includeKey resolvedPath includeInfo.XPath

        if ctx.InProgressIncludes.Contains(key) then
            IncludeError $"Circular include detected: {resolvedPath}"
        else
            match
                loadXmlFile ctx.FileCache resolvedPath
                |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
            with
            | Result.Error msg -> IncludeError msg
            | Result.Ok elements ->
                let elements = elements |> Seq.toList // materialize once (avoid re-running the XPath query)

                if List.isEmpty elements then
                    IncludeNoMatch
                else
                    // Clone the in-progress set and add this (file,xpath) for recursive expansion
                    let childInProgress =
                        HashSet<string>(ctx.InProgressIncludes, StringComparer.Ordinal)

                    childInProgress.Add(key) |> ignore

                    let childCtx =
                        { ctx with
                            InProgressIncludes = childInProgress
                        }

                    IncludeResolved(expandAllIncludeNodes resolvedPath (elements |> Seq.cast<XNode>) childCtx)

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
                warning (Error(FSComp.SR.xmlDocIncludeError msg, ctx.Range))
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
                | IncludeError msg ->
                    warning (Error(FSComp.SR.xmlDocIncludeError msg, ctx.Range))
                    Seq.singleton node)

/// Expand all <include> elements in an XmlDoc.
/// Uses a per-call file cache and case-insensitive cycle detection.
let expandIncludes (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let elaboratedLines = doc.GetElaboratedXmlLines()
        let hasIncludes = elaboratedLines |> Array.exists mayContainInclude

        if not hasIncludes then
            doc
        else
            let baseFileName = doc.Range.FileName
            let text = elaboratedLines |> String.concat "\n"

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
            | None -> doc
            | Some root ->
                let ctx =
                    {
                        FileCache = Dictionary<string, Result<XDocument, string>>(pathComparer)
                        InProgressIncludes = HashSet<string>(StringComparer.Ordinal)
                        Range = doc.Range
                    }

                let expandedText =
                    expandAllIncludeNodes baseFileName (root.Nodes()) ctx
                    |> Seq.map (fun (n: XNode) -> n.ToString(SaveOptions.DisableFormatting))
                    |> String.concat ""

                let expandedLines = String.getLines expandedText

                if Array.lengthsEqAndForall2 (=) expandedLines elaboratedLines then
                    doc
                else
                    XmlDoc(expandedLines, doc.Range)
