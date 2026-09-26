// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System

open Xunit

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Text

open FSharp.Editor.Tests.Helpers

/// `ClassName()` and `GenerateMatchCases()` against a real document and its check results, the way the
/// expansion engine calls them.
module internal SnippetFunctionTestHelpers =

    let documentOf (source: string) =
        RoslynTestHelpers.CreateSolution(source) |> RoslynTestHelpers.GetSingleDocument

    let containingTypeName (source: string) =
        let position = source.IndexOf("Marked", StringComparison.Ordinal)

        tryGetContainingTypeName (documentOf source) position
        |> CancellableTask.runSynchronouslyWithoutCancellation

    /// The rules generated for `match <expression> with`, one per element.
    let matchRules (context: string) (expression: string) =
        let source = $"{context}\nlet run () =\n    match {expression} with\n    | _ -> 0\n"

        let document = documentOf source
        let text = SourceText.From source

        let positionOf offset =
            let linePosition = text.Lines.GetLinePosition offset
            Position.mkPos (linePosition.Line + 1) linePosition.Character

        let start =
            source.IndexOf($"match {expression} with", StringComparison.Ordinal)
            + "match ".Length

        let range =
            Range.mkRange document.FilePath (positionOf start) (positionOf (start + expression.Length))

        tryGetMatchRulesAt document range
        |> CancellableTask.runSynchronouslyWithoutCancellation
        |> ValueOption.map (fun rules -> rules.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries))

type SnippetFunctionTests() =

    static member typeNames: obj[][] =
        [|
            [| "a type at the top level"; "type C() =\n    member _.Marked = 0"; "C" |]
            [|
                "a type in a module"
                "module Outer =\n    type C() =\n        member _.Marked = 0"
                "C"
            |]
            [|
                "a type in nested modules"
                "module A =\n    module B =\n        type C() =\n            member _.Marked = 0"
                "C"
            |]
            [|
                "the type the position is in, of two"
                "type First() =\n    member _.Value = 0\n\ntype Second() =\n    member _.Marked = 0"
                "Second"
            |]
        |]

    static member matchCases: obj[][] =
        [|
            [|
                "a union in scope"
                "type U = A | B\nlet value = U.B"
                "value"
                [| "| A -> ()"; "| B -> ()" |]
            |]
            [|
                "a union with fields"
                "type Shape = Circle of int | Square of int * int | Empty\nlet value = Empty"
                "value"
                [| "| Circle _ -> ()"; "| Square _ -> ()"; "| Empty -> ()" |]
            |]
            [|
                "a RequireQualifiedAccess union in scope"
                "[<RequireQualifiedAccess>]\ntype U = A | B\nlet value = U.B"
                "value"
                [| "| U.A -> ()"; "| U.B -> ()" |]
            |]
            [|
                "a RequireQualifiedAccess union in a module that is not open"
                "module Outer =\n    [<RequireQualifiedAccess>]\n    type U = A | B\n\nlet value = Outer.U.B"
                "value"
                [| "| Outer.U.A -> ()"; "| Outer.U.B -> ()" |]
            |]
            [|
                "a union in a module that is not open"
                "module Outer =\n    type U = A | B\n\nlet value = Outer.U.B"
                "value"
                [| "| Outer.U.A -> ()"; "| Outer.U.B -> ()" |]
            |]
            [|
                "a union in a module that is open"
                "module Outer =\n    type U = A | B\n\nopen Outer\nlet value = U.B"
                "value"
                [| "| A -> ()"; "| B -> ()" |]
            |]
            [|
                "an enum in a module that is not open"
                "module Outer =\n    type E = | X = 1 | Y = 2\n\nlet value = Outer.E.X"
                "value"
                [| "| Outer.E.X -> ()"; "| Outer.E.Y -> ()"; "| _ -> ()" |]
            |]
            [|
                "a call, whose result is what is matched"
                "type Input = X | Y\ntype Output = A | B\nlet make (_: Input) = B\nlet value = Y"
                "make value"
                [| "| A -> ()"; "| B -> ()" |]
            |]
        |]

    static member noMatchCases: obj[][] =
        [|
            [| "a function value"; "type U = A | B\nlet make (_: int) = B"; "make" |]
            [| "a type that is neither a union nor an enum"; ""; "1" |]
        |]

    [<Theory>]
    [<MemberData(nameof (SnippetFunctionTests.typeNames))>]
    member _.``ClassName names the type the snippet lands in, without its enclosing modules``
        (_name: string, source: string, expected: string)
        =
        Assert.Equal<string>(
            expected,
            SnippetFunctionTestHelpers.containingTypeName source
            |> ValueOption.defaultValue "no type"
        )

    [<Fact>]
    member _.``ClassName has no answer outside a type``() =
        Assert.True((SnippetFunctionTestHelpers.containingTypeName "let Marked = 1").IsNone)

    [<Theory>]
    [<MemberData(nameof (SnippetFunctionTests.matchCases))>]
    member _.``GenerateMatchCases spells each case the way it resolves at the match``
        (_name: string, context: string, expression: string, expected: string[])
        =
        match SnippetFunctionTestHelpers.matchRules context expression with
        | ValueSome rules -> Assert.Equal<string list>(List.ofArray expected, List.ofArray rules)
        | ValueNone -> failwith "no match rules were generated"

    [<Theory>]
    [<MemberData(nameof (SnippetFunctionTests.noMatchCases))>]
    member _.``GenerateMatchCases leaves the default alone when the expression is not a union or enum``
        (_name: string, context: string, expression: string)
        =
        Assert.True((SnippetFunctionTestHelpers.matchRules context expression).IsNone)
