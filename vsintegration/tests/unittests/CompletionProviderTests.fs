// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
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
type CompletionProviderTests()  =
    let filePath = "C:\\test.fs"
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

    member private this.VerifyCompletionList(fileContents: string, marker: string, expected: string list, unexpected: string list) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(SourceText.From(fileContents), caretPosition, options, filePath, 0) |>
            Async.RunSynchronously |>
            Seq.map(fun result -> result.DisplayText)

        for item in expected do
            Assert.IsTrue(results.Contains(item), "Completions should contain '{0}'. Got '{1}'.", item, String.Join(", ", results))

        for item in unexpected do
            Assert.IsFalse(results.Contains(item), "Completions should not contain '{0}'. Got '{1}'", item, String.Join(", ", results))
    
    [<TestCase("x", false)>]
    [<TestCase("y", false)>]
    [<TestCase("1", false)>]
    [<TestCase("2", false)>]
    [<TestCase("x +", false)>]
    [<TestCase("Console.Write", false)>]
    [<TestCase("System.", true)>]
    [<TestCase("Console.", true)>]
    member this.ShouldTriggerCompletionAtCorrectMarkers(marker: string, shouldBeTriggered: bool) =
        let fileContents = """
let x = 1
let y = 2
System.Console.WriteLine(x + y)
"""

        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, filePath, [])
        Assert.AreEqual(shouldBeTriggered, triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should compute the correct result")

    [<TestCase(CompletionTriggerKind.Deletion)>]
    [<TestCase(CompletionTriggerKind.Other)>]
    [<TestCase(CompletionTriggerKind.Snippets)>]
    member this.ShouldNotTriggerCompletionAfterAnyTriggerOtherThanInsertion(triggerKind: CompletionTriggerKind) =
        let fileContents = "System.Console.WriteLine(123)"
        let caretPosition = fileContents.IndexOf("System.")
        let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, triggerKind, filePath, [])
        Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
    [<Test>]
    member this.ShouldNotTriggerCompletionInStringLiterals() =
        let fileContents = "let literal = \"System.Console.WriteLine()\""
        let caretPosition = fileContents.IndexOf("System.")
        let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, filePath, [])
        Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
    [<Test>]
    member this.ShouldNotTriggerCompletionInComments() =
        let fileContents = """
(*
This is a comment
System.Console.WriteLine()
*)
"""
        let caretPosition = fileContents.IndexOf("System.")
        let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, filePath, [])
        Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
    [<Test>]
    member this.ShouldNotTriggerCompletionInExcludedCode() =
        let fileContents = """
#if UNDEFINED
System.Console.WriteLine()
#endif
"""
        let caretPosition = fileContents.IndexOf("System.")
        let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, filePath, [])
        Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

    [<Test>]
    member this.ShouldDisplayTypeMembers() =
        let fileContents = """
type T1() =
    member this.M1 = 5
    member this.M2 = "literal"

[<EntryPoint>]
let main argv =
    let obj = T1()
    obj.
"""
        this.VerifyCompletionList(fileContents, "obj.", ["M1"; "M2"], ["System"])

    [<Test>]
    member this.ShouldDisplaySystemNamespace() =
        let fileContents = """
type T1 =
    member this.M1 = 5
    member this.M2 = "literal"
System.Console.WriteLine()
"""
        this.VerifyCompletionList(fileContents, "System.", ["Console"; "Array"; "String"], ["T1"; "M1"; "M2"])
