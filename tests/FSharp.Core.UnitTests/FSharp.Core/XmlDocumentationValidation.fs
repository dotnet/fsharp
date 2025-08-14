// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.XmlDocumentationValidation

open System
open System.IO
open System.Text.RegularExpressions
open System.Xml
open Xunit

/// Extracts XML documentation blocks from F# signature files
let extractXmlDocBlocks (content: string) =
    // Regex to match XML documentation comments (/// followed by XML content)
    let xmlDocPattern = @"^\s*///\s*(.*)$"
    let regex = Regex(xmlDocPattern, RegexOptions.Multiline)
    
    let lines = content.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries)
    let mutable xmlBlocks = []
    let mutable currentBlock = []
    let mutable lineNumber = 0
    
    for line in lines do
        lineNumber <- lineNumber + 1
        let trimmedLine = line.Trim()
        if trimmedLine.StartsWith("///") then
            let xmlContent = trimmedLine.Substring(3).Trim()
            currentBlock <- (xmlContent, lineNumber) :: currentBlock
        else
            if not (List.isEmpty currentBlock) then
                xmlBlocks <- List.rev currentBlock :: xmlBlocks
                currentBlock <- []
    
    // Don't forget the last block if file ends with XML comments
    if not (List.isEmpty currentBlock) then
        xmlBlocks <- List.rev currentBlock :: xmlBlocks
    
    List.rev xmlBlocks

/// Validates that XML content is well-formed
let validateXmlBlock (xmlLines: (string * int) list) =
    if List.isEmpty xmlLines then
        Ok ()
    else
        let xmlContent = xmlLines |> List.map fst |> String.concat "\n"
        let firstLineNumber = xmlLines |> List.head |> snd
        
        // Skip empty or whitespace-only blocks
        if String.IsNullOrWhiteSpace(xmlContent) then
            Ok ()
        else
            try
                // Wrap content in a root element to make it valid XML document
                let wrappedXml = sprintf "<root>%s</root>" xmlContent
                let doc = XmlDocument()
                doc.LoadXml(wrappedXml)
                Ok ()
            with
            | :? XmlException as ex ->
                Error (sprintf "Line %d: Invalid XML - %s" firstLineNumber ex.Message)
            | ex ->
                Error (sprintf "Line %d: XML parsing error - %s" firstLineNumber ex.Message)

/// Gets all .fsi files in FSharp.Core directory
let getFSharpCoreFsiFiles () =
    let coreDir = Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "src", "FSharp.Core")
    let fullPath = Path.GetFullPath(coreDir)
    if Directory.Exists(fullPath) then
        Directory.GetFiles(fullPath, "*.fsi", SearchOption.AllDirectories)
        |> Array.toList
    else
        []

[<Fact>]
let ``XML documentation in FSharp.Core fsi files should be well-formed`` () =
    let fsiFiles = getFSharpCoreFsiFiles()
    
    Assert.False(List.isEmpty fsiFiles, "No .fsi files found in FSharp.Core directory")
    
    let mutable errors = []
    let mutable totalBlocks = 0
    
    for fsiFile in fsiFiles do
        let relativePath = Path.GetFileName(fsiFile)
        try
            let content = File.ReadAllText(fsiFile)
            let xmlBlocks = extractXmlDocBlocks content
            
            for xmlBlock in xmlBlocks do
                totalBlocks <- totalBlocks + 1
                match validateXmlBlock xmlBlock with
                | Ok () -> ()
                | Error errorMsg ->
                    let error = sprintf "%s: %s" relativePath errorMsg
                    errors <- error :: errors
        with
        | ex ->
            let error = sprintf "%s: Failed to read file - %s" relativePath ex.Message
            errors <- error :: errors
    
    // Report statistics
    let validBlocks = totalBlocks - List.length errors
    let message = sprintf "Validated %d XML documentation blocks in %d .fsi files. %d valid, %d invalid." 
                    totalBlocks (List.length fsiFiles) validBlocks (List.length errors)
    
    if not (List.isEmpty errors) then
        let errorDetails = errors |> List.rev |> String.concat "\n"
        Assert.Fail(sprintf "%s\n\nErrors:\n%s" message errorDetails)
    else
        // This will show in test output for successful runs
        Assert.True(true, message)