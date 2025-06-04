// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for XML documentation validation:
// Validates that the generated FSharp.Core.xml file meets quality requirements

namespace FSharp.Core.UnitTests.Core

open System
open System.IO
open System.Xml
open System.Xml.Linq
open System.Reflection
open System.Linq
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

type XmlDocumentationTests() =

    /// Helper to get the path to FSharp.Core.xml file
    let getFSharpCoreXmlPath() =
        let assembly = typeof<int list>.Assembly
        let assemblyLocation = assembly.Location
        let assemblyDir = Path.GetDirectoryName(assemblyLocation)
        Path.Combine(assemblyDir, "FSharp.Core.xml")

    /// Helper to load the XML documentation
    let loadXmlDocument() =
        let xmlPath = getFSharpCoreXmlPath()
        if not (File.Exists(xmlPath)) then
            Assert.Fail(sprintf "FSharp.Core.xml file not found at: %s" xmlPath)
        XDocument.Load(xmlPath)

    [<Fact>]
    member _.``FSharp.Core.xml file exists and is valid XML``() =
        let xmlPath = getFSharpCoreXmlPath()
        
        // Check file exists
        Assert.AreEqual(true, File.Exists(xmlPath), sprintf "FSharp.Core.xml should exist at: %s" xmlPath)
        
        // Check it's valid XML by loading it
        try
            let doc = XDocument.Load(xmlPath)
            // Basic structure validation
            Assert.AreEqual(true, doc.Root <> null, "XML document should have a root element")
            Assert.AreEqual("doc", doc.Root.Name.LocalName, "Root element should be 'doc'")
        with
        | ex -> Assert.Fail(sprintf "FSharp.Core.xml is not valid XML: %s" ex.Message)

    [<Fact>]
    member _.``All documentation text is within parent elements``() =
        let doc = loadXmlDocument()
        
        // Get all member elements
        let members = doc.Descendants(XName.Get("member"))
        
        for memberElement in members do
            // Check that all text nodes are within proper parent elements
            let textNodes = memberElement.DescendantNodes() |> Seq.filter (fun node -> node.NodeType = System.Xml.XmlNodeType.Text)
            
            for textNode in textNodes do
                let text = textNode.ToString().Trim()
                if not (String.IsNullOrWhiteSpace(text)) then
                    let parent = textNode.Parent
                    let parentName = if parent <> null then parent.Name.LocalName else "null"
                    
                    // Text should be within summary, remarks, param, returns, example, etc.
                    let validParents = [ "summary"; "remarks"; "param"; "returns"; "example"; "exception"; "value"; "typeparam"; "c"; "a"; "see"; "paramref"; "typeparamref" ]
                    let isValidParent = validParents |> List.contains parentName
                    
                    if not isValidParent then
                        let memberName = 
                            match memberElement.Attribute(XName.Get("name")) with
                            | null -> "unknown"
                            | attr -> match attr.Value with null -> "unknown" | v -> v
                        Assert.Fail(sprintf "Free-floating text found in member '%s'. Text: '%s' is in element '%s' but should be within: %s" memberName text parentName (String.Join(", ", validParents)))

    [<Fact>]
    member _.``Complexity information in summary elements is at the end``() =
        let doc = loadXmlDocument()
        
        // Get all summary elements that contain complexity information
        let summaryElements = doc.Descendants(XName.Get("summary"))
        
        for summaryElement in summaryElements do
            let summaryText = summaryElement.Value
            
            // Check if it contains complexity information
            if summaryText.Contains("Time complexity") || summaryText.Contains("Space complexity") then
                let memberName = 
                    match summaryElement.Ancestors(XName.Get("member")).FirstOrDefault() with
                    | null -> "unknown"
                    | memberEl -> 
                        match memberEl.Attribute(XName.Get("name")) with
                        | null -> "unknown"
                        | attr -> match attr.Value with null -> "unknown" | v -> v
                
                // Split the summary text into lines and examine the structure
                let lines = summaryText.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries)
                              |> Array.map (fun line -> line.Trim())
                              |> Array.filter (fun line -> not (String.IsNullOrWhiteSpace(line)))
                
                if lines.Length > 0 then
                    // Find where complexity information starts
                    let complexityStartIndex = 
                        lines |> Array.tryFindIndex (fun line -> 
                            line.Contains("Time complexity") || line.Contains("Space complexity"))
                    
                    match complexityStartIndex with
                    | Some startIndex ->
                        // Verify that all lines after complexity start are complexity-related
                        let remainingLines = lines.[startIndex..]
                        let allComplexityRelated = remainingLines |> Array.forall (fun line ->
                            line.Contains("Time complexity") || 
                            line.Contains("Space complexity") ||
                            line.StartsWith("O(") ||
                            String.IsNullOrWhiteSpace(line))
                        
                        if not allComplexityRelated then
                            let nonComplexityLines = remainingLines |> Array.filter (fun line ->
                                not (line.Contains("Time complexity") || 
                                     line.Contains("Space complexity") ||
                                     line.StartsWith("O(") ||
                                     String.IsNullOrWhiteSpace(line)))
                            
                            Assert.Fail(sprintf "In member '%s', complexity information should be at the end of summary. Found non-complexity text after complexity info: %s" memberName (String.Join("; ", nonComplexityLines)))
                    | None ->
                        // This shouldn't happen since we filtered for summaries containing complexity info
                        ()