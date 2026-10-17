// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.SnippetFunctionTests

open System

open Xunit

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Editor.Tests.Helpers

let private rangeOf (document: Document) (source: string) span =
    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, SourceText.From source)

/// The innermost type's name at `Marked`, or "" when it is not inside a type.
let private containingTypeName (source: string) =
    let document = RoslynTestHelpers.GetFsDocument source
    let marked = TextSpan(source.IndexOf("Marked", StringComparison.Ordinal), 0)

    tryGetContainingTypeName document (rangeOf document source marked).Start
    |> CancellableTask.runSynchronouslyWithoutCancellation
    |> ValueOption.defaultValue ""

/// The rules generated for the expression matched on, one per line, or "" when there are none.
let private matchRules (context: string) (expression: string) =
    let prefix = $"{context}\nlet run () =\n    match "
    let source = $"{prefix}{expression} with\n    | _ -> 0\n"
    let document = RoslynTestHelpers.GetFsDocument source

    tryGetMatchRulesAt document (rangeOf document source (TextSpan(prefix.Length, expression.Length)))
    |> CancellableTask.runSynchronouslyWithoutCancellation
    |> ValueOption.map _.Replace("\r\n", "\n")
    |> ValueOption.defaultValue ""

let typeNames: obj[][] =
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
        [|
            "the type the position is in, of a recursive group"
            "type First() =\n    member _.Marked = 0\n\nand Second = | A"
            "First"
        |]
        [| "no type outside a type"; "let Marked = 1"; "" |]
    |]

let matchCases: obj[][] =
    [|
        [|
            "a union in scope"
            "type U = A | B\nlet value = U.B"
            "value"
            "| A -> ()\n| B -> ()"
        |]
        [|
            "a union with fields"
            "type Shape = Circle of int | Square of int * int | Empty\nlet value = Empty"
            "value"
            "| Circle _ -> ()\n| Square _ -> ()\n| Empty -> ()"
        |]
        [|
            "a union case that needs backticks"
            "type U = | ``A B`` | C\nlet value = C"
            "value"
            "| ``A B`` -> ()\n| C -> ()"
        |]
        [|
            "a union case named like an encoded operator"
            "[<RequireQualifiedAccess>]\ntype U = op_Nil | B\nlet value = U.B"
            "value"
            "| U.op_Nil -> ()\n| U.B -> ()"
        |]
        [|
            "an enum case that needs backticks"
            "type E = | ``A B`` = 1 | C = 2\nlet value = E.C"
            "value"
            "| E.``A B`` -> ()\n| E.C -> ()\n| _ -> ()"
        |]
        [|
            "a RequireQualifiedAccess union in scope"
            "[<RequireQualifiedAccess>]\ntype U = A | B\nlet value = U.B"
            "value"
            "| U.A -> ()\n| U.B -> ()"
        |]
        [|
            "a RequireQualifiedAccess union in a module that is not open"
            "module Outer =\n    [<RequireQualifiedAccess>]\n    type U = A | B\n\nlet value = Outer.U.B"
            "value"
            "| Outer.U.A -> ()\n| Outer.U.B -> ()"
        |]
        [|
            "a union in a module that is not open"
            "module Outer =\n    type U = A | B\n\nlet value = Outer.U.B"
            "value"
            "| Outer.U.A -> ()\n| Outer.U.B -> ()"
        |]
        [|
            "a union in a module that is open"
            "module Outer =\n    type U = A | B\n\nopen Outer\nlet value = U.B"
            "value"
            "| A -> ()\n| B -> ()"
        |]
        [|
            "an enum in a module that is not open"
            "module Outer =\n    type E = | X = 1 | Y = 2\n\nlet value = Outer.E.X"
            "value"
            "| Outer.E.X -> ()\n| Outer.E.Y -> ()\n| _ -> ()"
        |]
        [|
            "a call, whose result is what is matched"
            "type Input = X | Y\ntype Output = A | B\nlet make (_: Input) = B\nlet value = Y"
            "make value"
            "| A -> ()\n| B -> ()"
        |]
        [|
            "a function value gives nothing"
            "type U = A | B\nlet make (_: int) = B"
            "make"
            ""
        |]
        [| "a type that is neither a union nor an enum gives nothing"; ""; "1"; "" |]
    |]

[<Theory; MemberData(nameof typeNames)>]
let ``ClassName names the type the snippet lands in, without its enclosing modules, and nothing outside a type``
    (_name: string, source: string, expected: string)
    =
    Assert.Equal<string>(expected, containingTypeName source)

[<Theory; MemberData(nameof matchCases)>]
let ``GenerateMatchCases spells each case the way it resolves at the match, and nothing for other types``
    (_name: string, context: string, expression: string, expected: string)
    =
    Assert.Equal<string>(expected, matchRules context expression)
