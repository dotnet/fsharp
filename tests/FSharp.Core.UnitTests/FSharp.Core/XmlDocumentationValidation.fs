// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Core.UnitTests.XmlDocumentationValidation

open System
open System.IO
open System.Xml
open Xunit

let isConditionalDirectiveLine (trimmedLine: string) =
    trimmedLine.StartsWith("#if")
    || trimmedLine.StartsWith("#else")
    || trimmedLine.StartsWith("#elif")
    || trimmedLine.StartsWith("#endif")

/// Extracts XML documentation blocks from F# signature files
let extractXmlDocBlocks (content: string) =
    seq {
        let currentBlock = ResizeArray<_>()
        let tryFlushCurrentBlock () =
            if currentBlock.Count > 0 then
                let block = currentBlock |> Seq.toList
                currentBlock.Clear()
                Some block
            else
                None

        use reader = new StringReader(content)
        let mutable lineNumber = 0
        let mutable line = reader.ReadLine()

        while not (isNull line) do
            lineNumber <- lineNumber + 1
            let trimmed = line.Trim()

            if trimmed.StartsWith("///") then
                let xmlContent = trimmed.Substring(3).Trim()
                if not (String.IsNullOrWhiteSpace xmlContent) then
                    currentBlock.Add((xmlContent, lineNumber))
            elif isConditionalDirectiveLine trimmed || trimmed.Length = 0 then
                // Keep the current XML documentation block open across conditional directives and blank lines
                // Handles docs that have internal #if/#else/#endif guards within xmldoc blocks to cover TFM variations.
                ()
            else
                match tryFlushCurrentBlock () with
                | Some block -> yield block
                | None -> ()

            line <- reader.ReadLine()

        // Don't forget the last block if file ends with XML comments
        match tryFlushCurrentBlock () with
        | Some block -> yield block
        | None -> ()
    }

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

/// Locates the FSharp.Core.xml emitted next to the referenced FSharp.Core assembly.
let private findEmittedFSharpCoreXml () =
    let asmPath = typeof<int list>.Assembly.Location
    [ if not (String.IsNullOrEmpty asmPath) then Path.ChangeExtension(asmPath, ".xml")
      Path.Combine(AppContext.BaseDirectory, "FSharp.Core.xml") ]
    |> List.tryFind File.Exists

[<Fact>]
let ``Generated FSharp.Core.xml has no unexpanded include tags`` () =
    match findEmittedFSharpCoreXml () with
    | None ->
        Assert.Fail("Could not locate the emitted FSharp.Core.xml next to the FSharp.Core assembly.")
    | Some xmlPath ->
        let doc = XmlDocument()
        doc.Load(xmlPath)

        // A surviving <include> element means compile-time expansion did not happen
        // (missing fragment file, mistyped XPath, zero-match keep, or the feature regressed).
        Assert.Equal(0, doc.SelectNodes("//include").Count)

        // Sanity-check that expansion actually produced shared fragment text, so a silent
        // zero-match (which drops the content without a warning) is also caught.
        Assert.Contains("not a stable sort", doc.OuterXml)