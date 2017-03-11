
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
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.CompletionProviderTests

open System
open System.Linq

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

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
    OriginalLoadReferences = []
    UnresolvedReferences = None
    ExtraProjectInfo = None
}

let VerifyCompletionList(fileContents: string, marker: string, expected: string list, unexpected: string list) =
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let results = 
        FSharpCompletionProvider.ProvideCompletionsAsyncAux(FSharpChecker.Instance, SourceText.From(fileContents), caretPosition, options, filePath, 0) 
        |> Async.RunSynchronously 
        |> Option.defaultValue (ResizeArray())
        |> Seq.map(fun result -> result.DisplayText)

    for item in expected do
        Assert.IsTrue(results.Contains(item), sprintf "Completions should contain '%s'. Got '%s'." item (String.Join(", ", results)))

    for item in unexpected do
        Assert.IsFalse(results.Contains(item), sprintf "Completions should not contain '%s'. Got '{%s}'" item (String.Join(", ", results)))

let VerifyCompletionListExactly(fileContents: string, marker: string, expected: string list) =
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    
    let actual = 
        FSharpCompletionProvider.ProvideCompletionsAsyncAux(FSharpChecker.Instance, SourceText.From(fileContents), caretPosition, options, filePath, 0) 
        |> Async.RunSynchronously 
        |> Option.defaultValue (ResizeArray())
        |> Seq.toList
        // sort items as Roslyn do - by `SortText`
        |> List.sortBy (fun x -> x.SortText)

    let actualNames = actual |> List.map (fun x -> x.DisplayText)

    if actualNames <> expected then
        Assert.Fail(sprintf "Expected:\n%s,\nbut was:\n%s\nactual with sort text:\n%s" 
                            (String.Join(", ", expected)) 
                            (String.Join(", ", actualNames))
                            (String.Join("\n", actual |> List.map (fun x -> sprintf "%s => %s" x.DisplayText x.SortText))))
    
[<Test>]
let ShouldTriggerCompletionAtCorrectMarkers() =
    let testCases = 
       [("x", false)
        ("y", false)
        ("1", false)
        ("2", false)
        ("x +", false)
        ("Console.Write", false)
        ("System.", true)
        ("Console.", true) ]

    for (marker: string, shouldBeTriggered: bool) in testCases do
    let fileContents = """
let x = 1
let y = 2
System.Console.WriteLine(x + y)
"""

    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let getInfo() = documentId, filePath, []
    let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, getInfo)
    Assert.AreEqual(shouldBeTriggered, triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should compute the correct result")

[<Test>]
let ShouldNotTriggerCompletionAfterAnyTriggerOtherThanInsertion() = 
    for triggerKind in [CompletionTriggerKind.Deletion; CompletionTriggerKind.Other; CompletionTriggerKind.Snippets ] do
    let fileContents = "System.Console.WriteLine(123)"
    let caretPosition = fileContents.IndexOf("System.")
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let getInfo() = documentId, filePath, []
    let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, triggerKind, getInfo)
    Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
[<Test>]
let ShouldNotTriggerCompletionInStringLiterals() =
    let fileContents = "let literal = \"System.Console.WriteLine()\""
    let caretPosition = fileContents.IndexOf("System.")
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let getInfo() = documentId, filePath, []
    let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, getInfo)
    Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
[<Test>]
let ShouldNotTriggerCompletionInComments() =
    let fileContents = """
(*
This is a comment
System.Console.WriteLine()
*)
"""
    let caretPosition = fileContents.IndexOf("System.")
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let getInfo() = documentId, filePath, []
    let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, getInfo)
    Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")
    
[<Test>]
let ShouldNotTriggerCompletionInExcludedCode() =
    let fileContents = """
#if UNDEFINED
System.Console.WriteLine()
#endif
"""
    let caretPosition = fileContents.IndexOf("System.")
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let getInfo() = documentId, filePath, []
    let triggered = FSharpCompletionProvider.ShouldTriggerCompletionAux(SourceText.From(fileContents), caretPosition, CompletionTriggerKind.Insertion, getInfo)
    Assert.IsFalse(triggered, "FSharpCompletionProvider.ShouldTriggerCompletionAux() should not trigger")

[<Test>]
let ShouldDisplayTypeMembers() =
    let fileContents = """
type T1() =
    member this.M1 = 5
    member this.M2 = "literal"

[<EntryPoint>]
let main argv =
    let obj = T1()
    obj.
"""
    VerifyCompletionList(fileContents, "obj.", ["M1"; "M2"], ["System"])

[<Test>]
let ShouldDisplaySystemNamespace() =
    let fileContents = """
type T1 =
    member this.M1 = 5
    member this.M2 = "literal"
System.Console.WriteLine()
"""
    VerifyCompletionList(fileContents, "System.", ["Console"; "Array"; "String"], ["T1"; "M1"; "M2"])

[<Test>]
let ``Class instance members are ordered according to their kind and where they are defined (simple case, by a variable)``() =
    let fileContents = """
type Base() =
    member __.BaseMethod() = 1
    member __.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class()
x.
"""
    let expected = ["MineProp"; "BaseProp"; "MineMethod"; "BaseMethod"; "Equals"; "GetHashCode"; "GetType"; "ToString"]
    VerifyCompletionListExactly(fileContents, "x.", expected)

[<Test>]
let ``Class instance members are ordered according to their kind and where they are defined (simple case, by a constructor)``() =
    let fileContents = """
type Base() =
    member __.BaseMethod() = 1
    member __.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class().
"""
    let expected = ["MineProp"; "BaseProp"; "MineMethod"; "BaseMethod"; "Equals"; "GetHashCode"; "GetType"; "ToString"]
    VerifyCompletionListExactly(fileContents, "let x = Class().", expected)


[<Test>]
let ``Class static members are ordered according to their kind and where they are defined``() =
    let fileContents = """
type Base() =
    static member BaseStaticMethod() = 1
    static member BaseStaticProp = 1

type Class() = 
    inherit Base()
    static member MineStaticMethod() = 1
    static member MineStaticProp = 2

Class.
"""
    let expected = ["MineStaticProp"; "BaseStaticProp"; "MineStaticMethod"; "BaseStaticMethod"]
    VerifyCompletionListExactly(fileContents, "Class.", expected)

[<Test>]
let ``Class instance members are ordered according to their kind and where they are defined (complex case)``() =
    let fileContents = """
type Base() =
    inherit System.Collections.Generic.List<int>
    member __.BaseMethod() = 1
    member __.BaseProp = 1

type Class() = 
    inherit Base()
    member this.MineMethod() = 1
    member this.MineProp = 1

let x = Class()
x.
"""
    let expected = ["MineProp"; "BaseProp"; "Capacity"; "Count"; "Item"; "MineMethod"; "Add"; "AddRange"; "AsReadOnly"; "BaseMethod"; "BinarySearch"; "Clear"; "Contains"
                    "ConvertAll"; "CopyTo"; "Equals"; "Exists"; "Find"; "FindAll"; "FindIndex"; "FindLast"; "FindLastIndex"; "ForEach"; "GetEnumerator"; "GetHashCode"
                    "GetRange"; "GetType"; "IndexOf"; "Insert"; "InsertRange"; "LastIndexOf"; "Remove"; "RemoveAll"; "RemoveAt"; "RemoveRange"; "Reverse"; "Sort"
                    "ToArray"; "ToString"; "TrimExcess"; "TrueForAll"]
    VerifyCompletionListExactly(fileContents, "x.", expected)

#if EXE

ShouldDisplaySystemNamespace()
#endif
