
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
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.QuickInfoProviderTests

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

//open Microsoft.VisualStudio.FSharp.Editor
//open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
//open Microsoft.FSharp.Compiler.Range

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

let private normalizeLineEnds (s: string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")

let private getQuickInfoText (FSharpStructuredToolTipText.FSharpToolTipText elements) : string =
    let rec parseElement = function
        | FSharpToolTipElement.None -> ""
        | FSharpToolTipElement.Single(text, _) -> text
        | FSharpToolTipElement.SingleParameter(text, _, _) -> text
        | FSharpToolTipElement.Group(xs) -> xs |> List.map fst |> String.concat "\n"
        | FSharpToolTipElement.CompositionError(error) -> error
    elements |> List.map (Tooltips.ToFSharpToolTipElement >> parseElement) |> String.concat "\n" |> normalizeLineEnds

[<Test>]
let ShouldShowQuickInfoAtCorrectPositions() =
    let testCases = 
       [ "x", Some "val x : int\nFull name: Test.x"
         "y", Some "val y : int\nFull name: Test.y"
         "1", None
         "2", None
         "x +", Some "val x : int\nFull name: Test.x"
         "System", Some "namespace System"
         "Console", Some "type Console =
  static member BackgroundColor : ConsoleColor with get, set
  static member Beep : unit -> unit + 1 overload
  static member BufferHeight : int with get, set
  static member BufferWidth : int with get, set
  static member CapsLock : bool
  static member Clear : unit -> unit
  static member CursorLeft : int with get, set
  static member CursorSize : int with get, set
  static member CursorTop : int with get, set
  static member CursorVisible : bool with get, set
  ...
Full name: System.Console"
         "WriteLine", Some "System.Console.WriteLine(value: int) : unit" ]

    for (symbol: string, expected: string option) in testCases do
        let expected = expected |> Option.map normalizeLineEnds
        let fileContents = """
    let x = 1
    let y = 2
    System.Console.WriteLine(x + y)
    """
        let caretPosition = fileContents.IndexOf(symbol)
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let getInfo() = documentId, filePath, []
        
        let quickInfo =
            FSharpQuickInfoProvider.ProvideQuickInfo(FSharpChecker.Instance, documentId, SourceText.From(fileContents), filePath, caretPosition, options, 0)
            |> Async.RunSynchronously
        
        let actual = quickInfo |> Option.map (fun (text, _, _, _) -> getQuickInfoText text)
        Assert.AreEqual(expected, actual)