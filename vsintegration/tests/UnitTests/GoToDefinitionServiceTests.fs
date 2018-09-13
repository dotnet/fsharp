// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// To run the tests in this file:
//
// Technique 1: Compile VisualFSharp.UnitTests.dll and run it as a set of unit tests
//
// Technique 2:
//
//   Enable some tests in the #if EXE section at the end of the file, 
//   then compile this file as an EXE that has InternalsVisibleTo access into the
//   appropriate DLLs.  This can be the quickest way to get turnaround on updating the tests
//   and capturing large amounts of structured output.
(*
    cd Debug\net40\bin
    .\fsc.exe --define:EXE -r:.\Microsoft.Build.Utilities.Core.dll -o VisualFSharp.UnitTests.exe -g --optimize- -r .\FSharp.Compiler.Private.dll  -r .\FSharp.Editor.dll -r nunit.framework.dll ..\..\..\tests\service\FsUnit.fs ..\..\..\tests\service\Common.fs /delaysign /keyfile:..\..\..\src\fsharp\msft.pubkey ..\..\..\vsintegration\tests\UnitTests\GoToDefinitionServiceTests.fs 
    .\VisualFSharp.UnitTests.exe 
*)
// Technique 3: 
// 
//    Use F# Interactive.  This only works for FSharp.Compiler.Private.dll which has a public API

namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.IO
open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open UnitTests.TestLib.LanguageService

[<TestFixture>][<Category "Roslyn Services">]
module GoToDefinitionServiceTests =

    let userOpName = "GoToDefinitionServiceTests"

    let private findDefinition
        (
            checker: FSharpChecker, 
            documentKey: DocumentId, 
            sourceText: SourceText, 
            filePath: string, 
            position: int,
            defines: string list, 
            options: FSharpProjectOptions, 
            textVersionHash: int
        ) : range option = 
        maybe {
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(documentKey, sourceText, position, filePath, defines, SymbolLookupKind.Greedy, false)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (filePath, textVersionHash, sourceText.ToString(), options, LanguageServicePerformanceOptions.Default, userOpName=userOpName)  |> Async.RunSynchronously

            let declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, false, userOpName=userOpName) |> Async.RunSynchronously
            
            match declarations with
            | FSharpFindDeclResult.DeclFound range -> return range
            | _ -> return! None
        }

    [<Test>]
    let VerifyDefinition() =

      let manyTestCases = 
        [ 
// Test1
          ("""
type TestType() =
    member this.Member1(par1: int) =
        printf "%d" par1
    member this.Member2(par2: string) =
        printf "%s" par2

[<EntryPoint>]
let main argv =
    let obj = TestType()
    obj.Member1(5)
    obj.Member2("test")""",
                [ ("printf \"%d\" par1", Some(3, 3, 24, 28));
                  ("printf \"%s\" par2", Some(5, 5, 24, 28));
                  ("let obj = TestType", Some(2, 2, 5, 13));
                  ("let obj", Some(10, 10, 8, 11));
                  ("obj.Member1", Some(3, 3, 16, 23));
                  ("obj.Member2", Some(5, 5, 16, 23)); ]);
// Test2
          ("""
module Module1 =
    let foo x = x

let _ = Module1.foo 1
""",
                [ ("let _ = Module", Some (2, 2, 7, 14)) ]) 
          ]

      for fileContents, testCases in manyTestCases do
       for caretMarker, expected in testCases do
        
        printfn "Test case: caretMarker=<<<%s>>>" caretMarker 
        let filePath = Path.GetTempFileName() + ".fs"
        let options: FSharpProjectOptions = { 
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

        File.WriteAllText(filePath, fileContents)

        let caretPosition = fileContents.IndexOf(caretMarker) + caretMarker.Length - 1 // inside the marker
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let actual = 
           findDefinition(checker, documentId, SourceText.From(fileContents), filePath, caretPosition, [], options, 0) 
           |> Option.map (fun range -> (range.StartLine, range.EndLine, range.StartColumn, range.EndColumn))

        if actual <> expected then 
            Assert.Fail(sprintf "Incorrect information returned for fileContents=<<<%s>>>, caretMarker=<<<%s>>>, expected =<<<%A>>>, actual = <<<%A>>>" fileContents caretMarker expected actual)

#if EXE
    VerifyDefinition()
#endif