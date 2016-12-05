
// To run the tests in this file:
//
// Technique 1: Compile VisualFSharp.Unittests.dll and run it as a set of unit tests
//
// Technique 2:
//
//   Enable some tests in the #if EXE section at the end of the file, 
//   then compile this file as an EXE that has InternalsVisibleTo access into the
//   appropriate DLLs.  This can be the quickest way to get turnaround on updating the tests
//   and capturing large amounts of structured output.
(*
    cd Debug\net40\bin
    .\fsc.exe --define:EXE -r:.\Microsoft.Build.Utilities.Core.dll -o VisualFSharp.Unittests.exe -g --optimize- -r .\FSharp.LanguageService.Compiler.dll  -r .\FSharp.Editor.dll -r nunit.framework.dll ..\..\..\tests\service\FsUnit.fs ..\..\..\tests\service\Common.fs /delaysign /keyfile:..\..\..\src\fsharp\msft.pubkey ..\..\..\vsintegration\tests\unittests\CompletionProviderTests.fs 
    .\VisualFSharp.Unittests.exe 
*)
// Technique 3: 
// 
//    Use F# Interactive.  This only works for FSharp.Compiler.Service.dll which has a public API

// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.DocumentHighlightsServiceTests

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices

let filePath = "C:\\test.fs"

let internal options = { 
    ProjectFileName = "C:\\test.fsproj"
    ProjectFileNames =  [| filePath |]
    ReferencedProjects = [| |]
    OtherOptions = [| |]
    IsIncompleteTypeCheckEnvironment = true
    UseScriptResolutionRules = false
    LoadTime = DateTime.MaxValue
    UnresolvedReferences = None
    ExtraProjectInfo = None
}

[<Test>]
let ShouldHighlightAllLocalSymbolReferences() =
    let fileContents = """
    let foo x = 
        x + x
    let y = foo 2
    """
    let caretPosition = fileContents.IndexOf("foo") + 1
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let spans  =
        FSharpDocumentHighlightsService.GetDocumentHighlights(
            FSharpChecker.Instance, documentId, SourceText.From(fileContents), filePath, caretPosition, [], options, 0, CancellationToken.None)
        |> Async.RunSynchronously
    
    let expected =
        [| { IsDefinition = true; Range = Range.mkRange filePath (Range.mkPos 2 8) (Range.mkPos 2 11) }
           { IsDefinition = false; Range = Range.mkRange filePath (Range.mkPos 4 12) (Range.mkPos 4 15) } |]
    
    Assert.AreEqual(expected, spans)