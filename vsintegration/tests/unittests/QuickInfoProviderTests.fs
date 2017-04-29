
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
            let remarks = xs |> List.choose (fun item -> item.Remarks)
            let tpText = (match tps with [] -> "" | _ -> "\n" + String.concat "\n" tps)
            let remarksText = (match remarks with [] -> "" | _ -> "\n" + String.concat "\n" remarks)
            text + remarksText + tpText
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
'TSource is int * string
'TKey is int");
         ("Sort", Some "System.Array.Sort<'T>(array: 'T []) : unit
'T is int");
         ("let test4 x = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T is 'a list");
         ("let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T is 'U list");
         ("let test6 = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams : a:'T0 * y:'T0 -> 'T0 * 'T0
'T is int");
         ("let test7 x = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams : a:'a1 * y:'b2 -> 'a1 * 'b2
'a is 'a0 list
'b is 'a0 list");
         ("let test8 = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams : a:'a0 * y:'b1 -> 'a0 * 'b1
'a is int
'b is int");
         ("let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams : a:'a0 * y:'b1 -> 'a0 * 'b1
'a is 'U list
'b is 'U list");
         ("let res3 = [1] |>",
          Some
            "val ( |> ) : arg:'T1 -> func:('T1 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.( |> )
'T1 is int list
'U is int list");
         ("let res3 = [1] |> List.map id",
          Some
            "val id : x:'T -> 'T
Full name: Microsoft.FSharp.Core.Operators.id
'T is int");
         ("let res4 = (1.0,[1]) ||>",
          Some
            "val ( ||> ) : arg1:'T1 * arg2:'T2 -> func:('T1 -> 'T2 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.( ||> )
'T1 is float
'T2 is int list
'U is float");
         ("let res4 = (1.0,[1]) ||> List.fold",
          Some
            "val fold : folder:('State -> 'T -> 'State) -> state:'State -> list:'T list -> 'State
Full name: Microsoft.FSharp.Collections.List.fold
'T is int
'State is float");
         ("let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s +",
          Some
            "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
Full name: Microsoft.FSharp.Core.Operators.( + )
'T1 is string
'T2 is string
'T3 is float");
         ("let res5 = 1 +",
          Some
            "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
Full name: Microsoft.FSharp.Core.Operators.( + )
'T1 is int
'T2 is int
'T3 is int");
         ("let res6 = System.DateTime.Now +",
          Some
            "val ( + ) : x:'T1 -> y:'T2 -> 'T3 (requires member ( + ))
Full name: Microsoft.FSharp.Core.Operators.( + )
'T1 is System.DateTime
'T2 is System.TimeSpan
'T3 is System.DateTime");
         ("let res7 = sin",
          Some
            "val sin : value:'T -> 'T (requires member Sin)
Full name: Microsoft.FSharp.Core.Operators.sin
'T is float");
         ("let res8 = abs",
          Some
            "val abs : value:'T -> 'T (requires member Abs)
Full name: Microsoft.FSharp.Core.Operators.abs
'T is int")]    
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