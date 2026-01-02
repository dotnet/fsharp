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
        let elements = doc.XPathSelectElements(xpath)

        if obj.ReferenceEquals(elements, null) || Seq.isEmpty elements then
            Result.Error $"XPath query returned no results: {xpath}"
        else
            Result.Ok elements
    with ex ->
        Result.Error $"Invalid XPath expression '{xpath}': {ex.Message}"

/// Include directive information
type private IncludeInfo = { FilePath: string; XPath: string }

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

/// Try to parse a line as an include directive (must be include tag alone on the line)
let private tryParseIncludeLine (line: string) : IncludeInfo option =
    let trimmed = line.Trim()
    // Quick check: must start with < and contain "include"
    if not (trimmed.StartsWith("<") && trimmed.Contains("include")) then
        None
    else
        try
            let elem = XElement.Parse(trimmed)

            if elem.Name.LocalName = "include" then
                tryGetInclude elem
            else
                None
        with _ ->
            None

/// Generic include expansion driver that works for both lines and XElement nodes
let rec private expandIncludes'<'T>
    (tryGetIncludeInfo: 'T -> IncludeInfo option)
    (expandLoaded: string -> XElement seq -> Set<string> -> range -> 'T seq)
    (baseFileName: string)
    (items: 'T seq)
    (inProgressFiles: Set<string>)
    (range: range)
    : 'T seq =
    items
    |> Seq.collect (fun item ->
        match tryGetIncludeInfo item with
        | None -> Seq.singleton item
        | Some includeInfo ->
            let resolvedPath = resolveFilePath baseFileName includeInfo.FilePath

            // Check for circular includes
            if inProgressFiles.Contains(resolvedPath) then
                warning (Error(FSComp.SR.xmlDocIncludeError $"Circular include detected: {resolvedPath}", range))
                Seq.singleton item
            else
                match loadXmlFile resolvedPath with
                | Result.Error msg ->
                    warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                    Seq.singleton item
                | Result.Ok includeDoc ->
                    match evaluateXPath includeDoc includeInfo.XPath with
                    | Result.Error msg ->
                        warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                        Seq.singleton item
                    | Result.Ok elements ->
                        // Expand the loaded content recursively
                        let updatedInProgress = inProgressFiles.Add(resolvedPath)
                        expandLoaded resolvedPath elements updatedInProgress range)

/// Recursively expand includes in XElement nodes
let rec private expandElements (baseFileName: string) (nodes: XNode seq) (inProgressFiles: Set<string>) (range: range) : XNode seq =
    let tryGetIncludeFromNode (node: XNode) =
        if node.NodeType <> System.Xml.XmlNodeType.Element then
            None
        else
            let elem = node :?> XElement
            tryGetInclude elem

    let expandLoadedElements resolvedPath (elements: XElement seq) updatedInProgress range =
        let nodes = elements |> Seq.collect (fun (e: XElement) -> e.Nodes())
        expandElements resolvedPath nodes updatedInProgress range

    // Handle non-include elements by recursing on children
    expandIncludes' tryGetIncludeFromNode expandLoadedElements baseFileName nodes inProgressFiles range
    |> Seq.collect (fun node ->
        if node.NodeType <> System.Xml.XmlNodeType.Element then
            Seq.singleton node
        else
            let elem = node :?> XElement

            match tryGetInclude elem with
            | Some _ -> Seq.singleton node // Already handled by expandIncludes'
            | None ->
                // Not an include, recurse on children
                let expandedChildren =
                    expandElements baseFileName (elem.Nodes()) inProgressFiles range

                let newElem = XElement(elem.Name, elem.Attributes(), expandedChildren)
                Seq.singleton (newElem :> XNode))

/// Process XML content line by line, only parsing lines with includes
let rec private expandIncludesInContent (baseFileName: string) (content: string) (inProgressFiles: Set<string>) (range: range) : string =
    // Early exit if content doesn't contain "<include" (cheap check, no allocation)
    if not (content.Contains("<include")) then
        content
    else
        let lines = content.Split([| '\r'; '\n' |], StringSplitOptions.None)
        let mutable hasIncludes = false

        // First pass: check if any lines contain include tags
        for line in lines do
            if line.Contains("<include") then
                hasIncludes <- true

        if not hasIncludes then
            content
        else
            // Process lines with includes
            let processedLines =
                lines
                |> Array.map (fun line ->
                    // Cheap detection: only parse if line contains <include
                    if not (line.Contains("<include")) then
                        line
                    else
                        match tryParseIncludeLine line with
                        | None -> line // Not a valid include directive, keep as-is
                        | Some includeInfo ->
                            let resolvedPath = resolveFilePath baseFileName includeInfo.FilePath

                            // Check for circular includes
                            if inProgressFiles.Contains(resolvedPath) then
                                warning (Error(FSComp.SR.xmlDocIncludeError $"Circular include detected: {resolvedPath}", range))
                                line
                            else
                                match loadXmlFile resolvedPath with
                                | Result.Error msg ->
                                    warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                                    line
                                | Result.Ok includeDoc ->
                                    match evaluateXPath includeDoc includeInfo.XPath with
                                    | Result.Error msg ->
                                        warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                                        line
                                    | Result.Ok elements ->
                                        // Expand the loaded content recursively
                                        let updatedInProgress = inProgressFiles.Add(resolvedPath)

                                        let expandedNodes =
                                            expandElements
                                                resolvedPath
                                                (elements |> Seq.collect (fun e -> e.Nodes()))
                                                updatedInProgress
                                                range

                                        let expandedText =
                                            expandedNodes |> Seq.map (fun n -> n.ToString()) |> String.concat ""
                                        // Recursively expand any includes in the loaded content
                                        expandIncludesInContent resolvedPath expandedText updatedInProgress range)

            String.concat Environment.NewLine processedLines

/// Expand all <include> elements in XML documentation text
let expandIncludesInText (baseFileName: string) (xmlText: string) (range: range) : string =
    // Early exit if content doesn't contain "<include" (case-insensitive)
    if
        String.IsNullOrEmpty(xmlText)
        || not (xmlText.IndexOf("<include", StringComparison.OrdinalIgnoreCase) >= 0)
    then
        xmlText
    else
        expandIncludesInContent baseFileName xmlText Set.empty range

/// Expand all <include> elements in an XmlDoc
let expandIncludes (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let unprocessedLines = doc.UnprocessedLines
        let baseFileName = doc.Range.FileName

        // Early exit: check if any line contains "<include" (cheap check)
        let hasIncludes =
            unprocessedLines |> Array.exists (fun line -> line.Contains("<include"))

        if not hasIncludes then
            doc
        else
            // Expand includes in the line array, keeping the array structure
            let expandedLines =
                expandIncludes'
                    tryParseIncludeLine
                    (fun resolvedPath elements updatedInProgress range ->
                        // Convert XElements to lines (may be multiple lines)
                        let nodes = elements |> Seq.collect (fun e -> e.Nodes())
                        let expandedNodes = expandElements resolvedPath nodes updatedInProgress range
                        expandedNodes |> Seq.map (fun n -> n.ToString()))
                    baseFileName
                    unprocessedLines
                    Set.empty
                    doc.Range
                |> Array.ofSeq

            // Only create new XmlDoc if something changed
            if expandedLines = unprocessedLines then
                doc
            else
                XmlDoc(expandedLines, doc.Range)
