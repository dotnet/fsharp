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
            let elements = doc.XPathSelectElements(xpath)

            if obj.ReferenceEquals(elements, null) || Seq.isEmpty elements then
                Result.Error $"XPath query returned no results: {xpath}"
            else
                Result.Ok elements
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

/// Active pattern to parse a line as an include directive (must be include tag alone on the line)
let private (|ParsedXmlInclude|_|) (line: string) : Result<IncludeInfo, string> option =
    try
        let elem = XElement.Parse(line.Trim())
        classifyInclude elem
    with _ ->
        None

/// Expansion context threaded through recursive calls
type private ExpansionContext =
    {
        FileCache: Dictionary<string, Result<XDocument, string>>
        InProgressFiles: HashSet<string>
        Range: range
    }

/// Load and expand includes from an external file
let rec private resolveSingleInclude (baseFileName: string) (includeInfo: IncludeInfo) (ctx: ExpansionContext) : Result<XNode seq, string> =

    let resolvedPathResult =
        try
            Result.Ok(resolveFilePath baseFileName includeInfo.FilePath)
        with _ ->
            Result.Error $"Invalid file path: {includeInfo.FilePath}"

    match resolvedPathResult with
    | Result.Error msg -> Result.Error msg
    | Result.Ok resolvedPath ->

        if ctx.InProgressFiles.Contains(resolvedPath) then
            Result.Error $"Circular include detected: {resolvedPath}"
        else
            loadXmlFile ctx.FileCache resolvedPath
            |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
            |> Result.map (fun elements ->
                // Clone the in-progress set and add the current file for recursive expansion
                let childInProgress = HashSet<string>(ctx.InProgressFiles, pathComparer)
                childInProgress.Add(resolvedPath) |> ignore

                let childCtx =
                    {
                        FileCache = ctx.FileCache
                        InProgressFiles = childInProgress
                        Range = ctx.Range
                    }

                let nodes = elements |> Seq.cast<XNode>
                expandAllIncludeNodes resolvedPath nodes childCtx)

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
                | Result.Error msg ->
                    warning (Error(FSComp.SR.xmlDocIncludeError msg, ctx.Range))
                    Seq.singleton node
                | Result.Ok expandedNodes -> expandedNodes)

/// Expand all <include> elements in an XmlDoc.
/// Uses a per-call file cache and case-insensitive cycle detection.
let expandIncludes (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let unprocessedLines = doc.UnprocessedLines
        let baseFileName = doc.Range.FileName

        let hasIncludes = unprocessedLines |> Array.exists mayContainInclude

        if not hasIncludes then
            doc
        else
            let ctx =
                {
                    FileCache = Dictionary<string, Result<XDocument, string>>(pathComparer)
                    InProgressFiles = HashSet<string>(pathComparer)
                    Range = doc.Range
                }

            let expandedLines =
                unprocessedLines
                |> Array.collect (fun line ->
                    match line with
                    | s when not (mayContainInclude s) -> [| line |]
                    | ParsedXmlInclude(Result.Ok includeInfo) ->
                        match resolveSingleInclude baseFileName includeInfo ctx with
                        | Result.Error msg ->
                            warning (Error(FSComp.SR.xmlDocIncludeError msg, doc.Range))
                            [| line |]
                        | Result.Ok nodes -> nodes |> Seq.map (fun n -> n.ToString()) |> Array.ofSeq
                    | ParsedXmlInclude(Result.Error msg) ->
                        warning (Error(FSComp.SR.xmlDocIncludeError msg, doc.Range))
                        [| line |]
                    | _ -> [| line |])

            if
                expandedLines.Length = unprocessedLines.Length
                && Array.forall2 (=) expandedLines unprocessedLines
            then
                doc
            else
                XmlDoc(expandedLines, doc.Range)
