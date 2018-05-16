// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis.Formatting

[<TestFixture>]
[<Category "Roslyn Services">]
type EditorFormattingServiceTests()  =
    let filePath = "C:\\test.fs"
    let projectOptions : FSharpProjectOptions = { 
        ProjectFileName = "C:\\test.fsproj"
        ProjectId = None
        SourceFiles =  [| filePath |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        OriginalLoadReferences = []
        UnresolvedReferences = None
        ExtraProjectInfo = None
        Stamp = None
    }
    //let parsingOptions: FSharpParsingOptions = 

    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let indentStyle = FormattingOptions.IndentStyle.Smart
    
    let template = """
let foo = [
    15
    ]marker1

async {
    return 10
    }marker2

let abc =
    [|
        10
        |]marker3

let def =
    (
        "hi"
        )marker4
"""
    
    [<TestCase("marker1", "]")>]
    [<TestCase("marker2", "}")>]
    [<TestCase("marker3", "    |]")>]
    [<TestCase("marker4", "    )")>]
    member this.TestIndentation(marker: string, expectedLine: string) =
        let checker = FSharpChecker.Create()
        let position = template.IndexOf(marker)
        Assert.IsTrue(position >= 0, "Precondition failed: unable to find marker in template")

        let sourceText = SourceText.From(template)
        let lineNumber = sourceText.Lines |> Seq.findIndex (fun line -> line.Span.Contains position)
        let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions projectOptions
        
        let changesOpt = FSharpEditorFormattingService.GetFormattingChanges(documentId, sourceText, filePath, checker, indentStyle, Some (parsingOptions, projectOptions), position) |> Async.RunSynchronously
        match changesOpt with
        | None -> Assert.Fail("Expected a text change, but got None")
        | Some change ->
            let changedText = sourceText.WithChanges(change)
            let lineText = changedText.Lines.[lineNumber].ToString()
            Assert.IsTrue(lineText.StartsWith(expectedLine), "Changed line does not start with expected text")
