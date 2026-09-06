// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

module GoToDefinitionServiceTests =

    let userOpName = "GoToDefinitionServiceTests"

    let private findDefinition
        (document: Document, sourceText: SourceText, position: int, defines: string list, langVersion: string option)
        : range option =
        maybe {
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! lexerSymbol =
                Tokenizer.getSymbolAtPosition (
                    document.Id,
                    sourceText,
                    position,
                    document.FilePath,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    langVersion,
                    System.Threading.CancellationToken.None
                )

            let _, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (userOpName))
                |> CancellableTask.runSynchronouslyWithoutCancellation

            let declarations =
                checkFileResults.GetDeclarationLocation(
                    fcsTextLineNumber,
                    lexerSymbol.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    lexerSymbol.FullIsland,
                    false
                )

            match declarations with
            | FindDeclResult.DeclFound range -> return range
            | _ -> return! None
        }

    let GoToDefinitionTest (fileContents: string, caretMarker: string, expected) =

        let caretPosition = fileContents.IndexOf(caretMarker) + caretMarker.Length - 1 // inside the marker

        let sourceText = SourceText.From(fileContents)

        let document =
            RoslynTestHelpers.CreateSolution(fileContents)
            |> RoslynTestHelpers.GetSingleDocument

        let actual =
            findDefinition (document, sourceText, caretPosition, [], None)
            |> Option.map (fun range -> (range.StartLine, range.EndLine, range.StartColumn, range.EndColumn))

        if actual <> expected then
            failwithf
                "Incorrect information returned for fileContents=<<<%s>>>, caretMarker=<<<%s>>>, expected =<<<%A>>>, actual = <<<%A>>>"
                fileContents
                caretMarker
                expected
                actual

    [<Fact>]
    let ``goto definition smoke test`` () =

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
                 [
                     ("printf \"%d\" par1", Some(3, 3, 24, 28))
                     ("printf \"%s\" par2", Some(5, 5, 24, 28))
                     ("let obj = TestType", Some(2, 2, 5, 13))
                     ("let obj", Some(10, 10, 8, 11))
                     ("obj.Member1", Some(3, 3, 16, 23))
                     ("obj.Member2", Some(5, 5, 16, 23))
                 ])
                // Test2
                ("""
module Module1 =
    let foo x = x

let _ = Module1.foo 1
""",
                 [ ("let _ = Module", Some(2, 2, 7, 14)) ])
            ]

        for fileContents, testCases in manyTestCases do
            for caretMarker, expected in testCases do

                printfn "Test case: caretMarker=<<<%s>>>" caretMarker
                GoToDefinitionTest(fileContents, caretMarker, expected)

    [<Fact>]
    let ``goto definition for string interpolation`` () =

        let fileContents =
            """
let xxxxx = 1
let yyyy = $"{abc{xxxxx}def}" """

        let caretMarker = "xxxxx"
        let expected = Some(2, 2, 4, 9)

        GoToDefinitionTest(fileContents, caretMarker, expected)

    [<Fact>]
    let ``goto definition for static abstract method invocation`` () =

        let fileContents =
            """
type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
    'T.StaticProperty
"""

        let caretMarker = "'T.StaticProperty"
        let expected = Some(3, 3, 20, 34)

        GoToDefinitionTest(fileContents, caretMarker, expected)

    let private symbolUseAt (document: Document) (sourceText: SourceText) position =
        maybe {
            let textLine = sourceText.Lines.GetLineFromPosition position
            let fcsTextLineNumber = Line.fromZ (sourceText.Lines.GetLinePosition position).Line

            let! lexerSymbol =
                Tokenizer.getSymbolAtPosition (
                    document.Id,
                    sourceText,
                    position,
                    document.FilePath,
                    [],
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    None,
                    CancellationToken.None
                )

            let _, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync userOpName
                |> CancellableTask.runSynchronouslyWithoutCancellation

            return!
                checkFileResults.GetSymbolUseAtLocation(
                    fcsTextLineNumber,
                    lexerSymbol.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    lexerSymbol.FullIsland
                )
        }

    /// An app project referencing a library project. The app document comes from a snapshot that
    /// predates the library's document, the way Roslyn hands out documents while a solution is
    /// still loading, while the workspace's current solution already has it.
    module internal StaleSnapshot =

        let library = SyntheticProject.Create("Library", sourceFile "Library" [])

        let app =
            { SyntheticProject.Create(
                  "App",
                  { sourceFile "App" [ "Library" ] with
                      ExtraSource = "let mapped = List.map id [ 1 ]"
                  }
              ) with
                DependsOn = [ library ]
            }

        let solution, _ = RoslynTestHelpers.CreateMultiProjectSolution app
        let appPath = app.GetFilePath "App"
        let libraryPath = library.GetFilePath "Library"

        let private documentId path =
            solution.GetDocumentIdsWithFilePath path |> Seq.exactlyOne

        let appDocument =
            solution.RemoveDocument(documentId libraryPath).GetDocument(documentId appPath)

        let appSourceText = appDocument.GetTextAsync(CancellationToken.None).Result

        /// The position of the last character of the text, inside the identifier it ends with.
        let positionOf (text: string) =
            appSourceText.ToString().IndexOf(text, StringComparison.Ordinal) + text.Length
            - 1

        let findDefinitionAt position =
            GoToDefinition(FSharpMetadataAsSourceService()).FindDefinitionAtPosition(appDocument, position)
            |> CancellableTask.runSynchronouslyWithoutCancellation

    [<Fact>]
    let ``goto definition finds the target document through the workspace when the origin snapshot predates it`` () =
        let position = StaleSnapshot.positionOf "ModuleLibrary.f"
        let document = StaleSnapshot.appDocument

        let range =
            findDefinition (document, StaleSnapshot.appSourceText, position, [], None)
            |> Option.defaultWith (fun () -> failwith "declaration not found")

        Assert.Equal(StaleSnapshot.libraryPath, range.FileName)
        Assert.True(Option.isNone (document.Project.Solution.TryGetDocumentFromFSharpRange(range, document.Project.Id)))

        match document.TryGetSolutionDocumentFromFSharpRange range with
        | ValueSome target -> Assert.Equal(StaleSnapshot.libraryPath, target.FilePath)
        | ValueNone -> failwith "the workspace's current solution has the library document"

        match StaleSnapshot.findDefinitionAt position with
        | ValueSome(FSharpGoToDefinitionResult.NavigableItem item, _) -> Assert.Equal(StaleSnapshot.libraryPath, item.Document.FilePath)
        | result -> failwith $"expected a navigable item, got %A{result}"

    [<Fact>]
    let ``goto definition treats a symbol whose file is in no solution as external`` () =
        match StaleSnapshot.findDefinitionAt (StaleSnapshot.positionOf "List.map") with
        | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly _, _) -> ()
        | result -> failwith $"expected an external assembly, got %A{result}"

    [<Fact>]
    let ``find references scope includes the declaring project the origin snapshot does not know`` () =
        let position = StaleSnapshot.positionOf "ModuleLibrary.f"

        let symbolUse =
            symbolUseAt StaleSnapshot.appDocument StaleSnapshot.appSourceText position
            |> Option.defaultWith (fun () -> failwith "symbol not found")

        match symbolUse.GetSymbolScope StaleSnapshot.appDocument with
        | Some(SymbolScope.Projects(projects, _)) -> Assert.Contains(StaleSnapshot.library.Name, projects |> List.map _.Name)
        | scope -> failwith $"expected a project scope, got %A{scope}"
