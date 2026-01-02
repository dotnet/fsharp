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

/// Recursively expand includes in XML content
let rec private expandIncludesInContent (baseFileName: string) (content: string) (inProgressFiles: Set<string>) (range: range) : string =
    // Early exit if content doesn't contain "<include" (case-insensitive check)
    if not (content.IndexOf("<include", StringComparison.OrdinalIgnoreCase) >= 0) then
        content
    else
        try
            // Wrap content in a root element to handle multiple top-level elements
            let wrappedContent = "<root>" + content + "</root>"
            let doc = XDocument.Parse(wrappedContent)

            let includeElements = doc.Descendants(!!(XName.op_Implicit "include")) |> Seq.toList

            if includeElements.IsEmpty then
                content
            else
                let mutable modified = false

                for includeElem in includeElements do
                    let fileAttr = includeElem.Attribute(!!(XName.op_Implicit "file"))
                    let pathAttr = includeElem.Attribute(!!(XName.op_Implicit "path"))

                    match fileAttr, pathAttr with
                    | Null, _ -> warning (Error(FSComp.SR.xmlDocIncludeError "Missing 'file' attribute", range))
                    | _, Null -> warning (Error(FSComp.SR.xmlDocIncludeError "Missing 'path' attribute", range))
                    | NonNull fileAttr, NonNull pathAttr ->
                        let includePath = fileAttr.Value
                        let xpath = pathAttr.Value
                        let resolvedPath = resolveFilePath baseFileName includePath

                        // Check for circular includes
                        if inProgressFiles.Contains(resolvedPath) then
                            warning (Error(FSComp.SR.xmlDocIncludeError $"Circular include detected: {resolvedPath}", range))
                        else
                            match loadXmlFile resolvedPath with
                            | Result.Error msg -> warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                            | Result.Ok includeDoc ->
                                match evaluateXPath includeDoc xpath with
                                | Result.Error msg -> warning (Error(FSComp.SR.xmlDocIncludeError msg, range))
                                | Result.Ok elements ->
                                    // Get the inner content of selected elements
                                    let newNodes = elements |> Seq.collect (fun elem -> elem.Nodes()) |> Seq.toList

                                    // Recursively expand includes in the loaded content
                                    let updatedInProgress = inProgressFiles.Add(resolvedPath)

                                    let expandedNodes =
                                        newNodes
                                        |> List.map (fun node ->
                                            if node.NodeType = System.Xml.XmlNodeType.Element then
                                                let elemNode = node :?> XElement
                                                let elemContent = elemNode.ToString()

                                                let expanded =
                                                    expandIncludesInContent resolvedPath elemContent updatedInProgress range

                                                XElement.Parse(expanded) :> XNode
                                            else
                                                node)

                                    // Replace the include element with expanded content
                                    includeElem.ReplaceWith(expandedNodes)
                                    modified <- true

                if modified then
                    // Extract content from root wrapper
                    match doc.Root with
                    | Null -> content
                    | NonNull root ->
                        let resultDoc = root.Nodes() |> Seq.map (fun n -> n.ToString()) |> String.concat ""
                        resultDoc
                else
                    content
        with ex ->
            warning (Error(FSComp.SR.xmlDocIncludeError $"Error parsing XML: {ex.Message}", range))
            content

/// Expand all <include> elements in an XmlDoc
let expandIncludes (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        // Get the elaborated XML text which includes proper XML structure
        let content = doc.GetXmlText()

        // Early exit if content doesn't contain "<include" (case-insensitive)
        if not (content.IndexOf("<include", StringComparison.OrdinalIgnoreCase) >= 0) then
            doc
        else
            let baseFileName = doc.Range.FileName

            let expandedContent =
                expandIncludesInContent baseFileName content Set.empty doc.Range

            // Create new XmlDoc with expanded content if it changed
            if expandedContent = content then
                doc
            else
                // Split back into lines to match the XmlDoc structure
                let lines =
                    expandedContent.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)

                XmlDoc(lines, doc.Range)
