// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open System
open System.IO
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.Xml
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Library

/// Thread-safe cache for loaded XML files
let private xmlDocCache =
    let cacheOptions =
        FSharp.Compiler.Caches.CacheOptions.getDefault StringComparer.OrdinalIgnoreCase

    new FSharp.Compiler.Caches.Cache<string, Result<XDocument, string>>(cacheOptions, "XmlDocIncludeCache")

/// Load an XML file from disk with caching
let private loadXmlFile (filePath: string) : Result<XDocument, string> =
    xmlDocCache.GetOrAdd(
        filePath,
        fun path ->
            try
                if not (FileSystem.FileExistsShim(path)) then
                    Result.Error $"File not found: {path}"
                else
                    let doc = XDocument.Load(path)
                    Result.Ok doc
            with ex ->
                Result.Error $"Error loading file '{path}': {ex.Message}"
    )

/// Resolve a file path (absolute or relative to source file)
let private resolveFilePath (baseFileName: string) (includePath: string) : string =
    if Path.IsPathRooted(includePath) then
        includePath
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

/// Extract include directive from an XElement if it has both required attributes
let private tryGetInclude (elem: XElement) : IncludeInfo option =
    let fileAttr = elem.Attribute(!!(XName.op_Implicit "file"))
    let pathAttr = elem.Attribute(!!(XName.op_Implicit "path"))

    match fileAttr, pathAttr with
    | NonNull file, NonNull path ->
        Some
            {
                FilePath = file.Value
                XPath = path.Value
            }
    | _ -> None

/// Active pattern to parse a line as an include directive (must be include tag alone on the line)
let private (|ParsedXmlInclude|_|) (line: string) : IncludeInfo option =
    try
        let elem = XElement.Parse(line.Trim())

        if elem.Name.LocalName = "include" then
            tryGetInclude elem
        else
            None
    with _ ->
        None

/// Load and expand includes from an external file
/// This is the single unified error-handling and expansion logic
let rec private resolveSingleInclude
    (baseFileName: string)
    (includeInfo: IncludeInfo)
    (inProgressFiles: Set<string>)
    (range: range)
    : Result<XNode seq, string> =

    let resolvedPath = resolveFilePath baseFileName includeInfo.FilePath

    // Check for circular includes
    if inProgressFiles.Contains(resolvedPath) then
        Result.Error $"Circular include detected: {resolvedPath}"
    else
        loadXmlFile resolvedPath
        |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
        |> Result.map (fun elements ->
            // Expand the loaded content recursively
            let updatedInProgress = inProgressFiles.Add(resolvedPath)
            let nodes = elements |> Seq.collect (fun e -> e.Nodes())
            expandAllIncludeNodes resolvedPath nodes updatedInProgress range)

/// Recursively expand includes in XElement nodes
/// This is the ONLY recursive expansion - works on XElement level, never on strings
and private expandAllIncludeNodes (baseFileName: string) (nodes: XNode seq) (inProgressFiles: Set<string>) (range: range) : XNode seq =
    nodes
    |> Seq.collect (fun node ->
        if node.NodeType <> System.Xml.XmlNodeType.Element then
            Seq.singleton node
        else
            let elem = node :?> XElement

            match tryGetInclude elem with
            | None ->
                // Not an include element, recursively process children
                let expandedChildren =
                    expandAllIncludeNodes baseFileName (elem.Nodes()) inProgressFiles range

                let newElem = XElement(elem.Name, elem.Attributes(), expandedChildren)
                Seq.singleton (newElem :> XNode)
            | Some includeInfo ->
                // This is an include element - expand it
                match resolveSingleInclude baseFileName includeInfo inProgressFiles range with
                | Result.Error msg ->
                    warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                    Seq.singleton node
                | Result.Ok expandedNodes -> expandedNodes)

/// Expand all <include> elements in an XmlDoc
/// Works directly on line array without string concatenation
let expandIncludes (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let unprocessedLines = doc.UnprocessedLines
        let baseFileName = doc.Range.FileName

        // Early exit: check if any line contains "<include" (cheap check)
        let hasIncludes = unprocessedLines |> Array.exists mayContainInclude

        if not hasIncludes then
            doc
        else
            // Expand includes in the line array, keeping the array structure
            let expandedLines =
                unprocessedLines
                |> Array.collect (fun line ->
                    match line with
                    | s when not (mayContainInclude s) -> [| line |]
                    | ParsedXmlInclude includeInfo ->
                        match resolveSingleInclude baseFileName includeInfo Set.empty doc.Range with
                        | Result.Error msg ->
                            warning (Error(FSComp.SR.xmlDocIncludeError msg, doc.Range))
                            [| line |]
                        | Result.Ok nodes ->
                            // Convert nodes to strings (may be multiple lines)
                            nodes |> Seq.map (fun n -> n.ToString()) |> Array.ofSeq
                    | _ -> [| line |])

            // Only create new XmlDoc if something changed
            if
                expandedLines.Length = unprocessedLines.Length
                && Array.forall2 (=) expandedLines unprocessedLines
            then
                doc
            else
                XmlDoc(expandedLines, doc.Range)
