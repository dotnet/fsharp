// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.IO
open System.Threading
open System.Linq

open NUnit.Framework

open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<TestFixture>]
type GoToDefinitionServiceTests()  =
    
    [<TestCase("printf \"%d\" par1", 3, 24, 28)>]
    [<TestCase("printf \"%s\" par2", 5, 24, 28)>]
    [<TestCase("let obj = TestType", 2, 5, 13)>]
    [<TestCase("let obj", 10, 8, 11)>]
    [<TestCase("obj.Member1", 3, 16, 23)>]
    [<TestCase("obj.Member2", 5, 16, 23)>]
    member this.VerifyDefinition(caretMarker: string, definitionLine: int, definitionStartColumn: int, definitionEndColumn: int) =
        let fileContents = """
type TestType() =
    member this.Member1(par1: int) =
        printf "%d" par1
    member this.Member2(par2: string) =
        printf "%s" par2

[<EntryPoint>]
let main argv =
    let obj = TestType()
    obj.Member1(5)
    obj.Member2("test")"""
        
        let filePath = Path.GetTempFileName() + ".fs"
        let options: FSharpProjectOptions = { 
            ProjectFileName = "C:\\test.fsproj"
            ProjectFileNames =  [| filePath |]
            ReferencedProjects = [| |]
            OtherOptions = [| |]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = false
            LoadTime = DateTime.MaxValue
            UnresolvedReferences = None
        }

        File.WriteAllText(filePath, fileContents)

        let caretPosition = fileContents.IndexOf(caretMarker) + caretMarker.Length - 1 // inside the marker
        let definitionOption = FSharpGoToDefinitionService.FindDefinition(SourceText.From(fileContents), filePath, caretPosition, [], options, 0, CancellationToken.None) |> Async.RunSynchronously

        match definitionOption with
        | None -> Assert.Fail("No definition found")
        | Some(range) ->
            Assert.AreEqual(range.StartLine, range.EndLine, "Range must be on the same line")
            Assert.AreEqual(definitionLine, range.StartLine, "Range line should match")
            Assert.AreEqual(definitionStartColumn, range.StartColumn, "Range start column should match")
            Assert.AreEqual(definitionEndColumn, range.EndColumn, "Range end column should match")
