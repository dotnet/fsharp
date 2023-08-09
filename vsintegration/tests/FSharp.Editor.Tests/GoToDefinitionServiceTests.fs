// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open System.Threading

module GoToDefinitionServiceTests =

    let userOpName = "GoToDefinitionServiceTests"

    let private findDefinition
        (
            document: Document,
            sourceText: SourceText,
            position: int,
            defines: string list,
            langVersion: string option
        ) : range option =
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
                    None,
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
