
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
[<NUnit.Framework.Category "Roslyn Services">]
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.QuickInfoProviderTests

open System

open NUnit.Framework

open Microsoft.CodeAnalysis
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

let private normalizeLineEnds (s: string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")

let private getQuickInfoText (FSharpToolTipText elements) : string =
    let rec parseElement = function
        | FSharpToolTipElement.None -> ""
        | FSharpToolTipElement.Group(xs) -> 
            let text = xs |> List.map (fun item -> item.MainDescription) |> String.concat "\n"
            let tps = xs |> List.collect (fun item -> item.TypeMapping)
            let tptext = (match tps with [] -> "" | _ -> "\n" + String.concat "\n" tps)
            text + tptext
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
        
        let quickInfo =
            FSharpQuickInfoProvider.ProvideQuickInfo(FSharpChecker.Instance, documentId, SourceText.From(fileContents), filePath, caretPosition, options, 0)
            |> Async.RunSynchronously
        
        let actual = quickInfo |> Option.map (fun (text, _, _, _) -> getQuickInfoText text)
        Assert.AreEqual(expected, actual)

[<Test>]
let ShouldShowQuickInfoForGenericParameters() =
    let testCases = 

        [("GroupBy",
          Some
            "(extension) System.Collections.Generic.IEnumerable.GroupBy<'TSource,'TKey>(keySelector: System.Func<'TSource,'TKey>) : System.Collections.Generic.IEnumerable<IGrouping<'TKey,'TSource>>
'TSource: int * string
'TKey: int");
         ("Sort", Some "System.Array.Sort<'T>(array: 'T []) : unit
'T: int");
         ("let test4 x = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T: 'a list");
         ("let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams",
          Some
    "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T: 'U list");
         ("let test6 = C().FSharpGenericMethodExplitTypeParams",
          Some
    "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T: int");
         ("let test7 x = C().FSharpGenericMethodInferredTypeParams",
          Some
    "member C.FSharpGenericMethodInferredTypeParams : a:'a1 * y:'b2 -> 'a1 * 'b2
'a: 'a0 list
'b: 'a0 list");
         ("let test8 = C().FSharpGenericMethodInferredTypeParams",
          Some
    "member C.FSharpGenericMethodInferredTypeParams : a:'a0 * y:'b1 -> 'a0 * 'b1
'a: int
'b: int");
         ("let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams",
          Some
    "member C.FSharpGenericMethodInferredTypeParams : a:'a0 * y:'b1 -> 'a0 * 'b1
'a: 'U list
'b: 'U list");
         ("let res3 = [1] |>",
          Some
    "val ( |> ) : arg:'T1 -> func:('T1 -> 'U) -> 'U
'T1: int list
'U: int list");
         ("let res3 = [1] |> List.map id", Some "val id : x:'T -> 'T
'T: int");
         ("let res4 = (1.0,[1]) ||>",
          Some
    "val ( ||> ) : arg1:'T1 * arg2:'T2 -> func:('T1 -> 'T2 -> 'U) -> 'U
'T1: float
'T2: int list
'U: float");
         ("let res4 = (1.0,[1]) ||> List.fold",
          Some
    "val fold : folder:('State -> 'T -> 'State) -> state:'State -> list:'T list -> 'State
'T: int
'State: float");
         ("let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s +",
          Some
    "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
'T1: string
'T2: string
'T3: float");
         ("let res5 = 1 +",
          Some
    "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
'T1: int
'T2: int
'T3: int");
         ("let res6 = System.DateTime.Now +",
          Some
    "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
'T1: System.DateTime
'T2: System.TimeSpan
'T3: System.DateTime");
         ("let res7 = sin",
          Some "val sin : value:'T -> 'T (requires member Sin)
'T: float");
         ("let res8 = abs",
          Some "val abs : value:'T -> 'T (requires member Abs)
'T: int")]
    let actualForAllTests = 
     [ for (symbol: string, expected: string option) in testCases do
        let expected = expected |> Option.map normalizeLineEnds
        let fileContents = """

type C() = 
    member x.FSharpGenericMethodExplitTypeParams<'T>(a:'T, y:'T) = (a,y)

    member x.FSharpGenericMethodInferredTypeParams(a, y) = (a,y)

open System.Linq
let coll = [ for i in 1 .. 100 -> (i, string i) ]
let res1 = coll.GroupBy (fun (a, b) -> a)
let res2 = System.Array.Sort [| 1 |]
let test4 x = C().FSharpGenericMethodExplitTypeParams([x], [x])
let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams([x], [x])
let test6 = C().FSharpGenericMethodExplitTypeParams(1, 1)
let test7 x = C().FSharpGenericMethodInferredTypeParams([x], [x])
let test8 = C().FSharpGenericMethodInferredTypeParams(1, 1)
let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams([x], [x])
let res3 = [1] |> List.map id
let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s + string x) // note there is a type error here, still cehck quickinfo any way
let res5 = 1 + 2
let res6 = System.DateTime.Now + System.TimeSpan.Zero
let res7 = sin 5.0
let res8 = abs 5.0<kg>
    """
        let caretPosition = fileContents.IndexOf(symbol) + symbol.Length - 1
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        
        let quickInfo =
            FSharpQuickInfoProvider.ProvideQuickInfo(FSharpChecker.Instance, documentId, SourceText.From(fileContents), filePath, caretPosition, options, 0)
            |> Async.RunSynchronously
        
        let actual = quickInfo |> Option.map (fun (text, _, _, _) -> getQuickInfoText text)
        yield symbol, actual ]
    printfn "results:\n%A" actualForAllTests
    for ((symbol, expected),(_,actual)) in List.zip testCases actualForAllTests do
       Assert.AreEqual(Option.map normalizeLineEnds expected, Option.map normalizeLineEnds actual)