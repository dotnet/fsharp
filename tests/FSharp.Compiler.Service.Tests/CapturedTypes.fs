module FSharp.Compiler.Service.Tests.CapturedTypes

open FSharp.Compiler.Symbols
open FSharp.Test.Assert
open Xunit

let private displayContext =
    FSharpDisplayContext.Empty.WithShortTypeNames(true)

let tryGetCapturedType markedSource =
    let context, checkResults = Checker.getCheckedResolveContext markedSource
    checkResults.TryGetCapturedType context.SelectedRange.Value

let assertCapturedType expectedTypeString markedSource =
    let capturedType = tryGetCapturedType markedSource
    capturedType.Value.Format displayContext |> shouldEqual expectedTypeString

module Expr =
    [<Fact>]
    let ``Function 01`` () =
        assertCapturedType "string -> int" "[\"\"] |> List.map ({selstart}function s -> s.Length{selend})"

    [<Fact>]
    let ``Function 02`` () =
        assertCapturedType "string" "[\"\"] |> List.map ({selstart}{selend}function s -> s.Length)"

    [<Fact>]
    let ``Function 03`` () =
        assertCapturedType "string" "[\"\"] |> List.map ({selstart}{selend}function)"

    [<Fact(Skip = "Implement parser recovery")>]
    let ``Function 04`` () =
        assertCapturedType "string" "[\"\"] |> List.map {selstart}{selend}function"

    [<Fact>]
    let ``If 01`` () =
        assertCapturedType "int * int" "{selstart}if true then 1, 2 else 1, true{selend}"

    [<Fact>]
    let ``Lambda 01`` () =
        assertCapturedType "string -> int" "[\"\"] |> List.map ({selstart}fun s -> s.Length{selend})"

    [<Fact>]
    let ``Literal 01`` () =
        assertCapturedType "int" "{selstart}1{selend}"

    [<Fact>]
    let ``Literal 02`` () =
        assertCapturedType "string" "{selstart}\"\"{selend}"

    [<Fact>]
    let ``Paren 01`` () =
        assertCapturedType "string -> int" "[\"\"] |> List.map {selstart}(fun s -> s.Length){selend}"

    [<Fact>]
    let ``Short lambda 01`` () =
        assertCapturedType "string" "[\"\"] |> List.map {selstart}_{selend}.Length"

    [<Fact>]
    let ``Short lambda 02`` () =
        assertCapturedType "string -> int" "[\"\"] |> List.map {selstart}_.Length{selend}"

    [<Fact>]
    let ``Tuple 01`` () =
        assertCapturedType "int * int" "{selstart}1, 2{selend}"

    [<Fact>]
    let ``Tuple 02`` () =
        assertCapturedType "int * int" "if true then {selstart}1, 2{selend} else 1, true"

module Pattern =
    [<Fact>]
    let ``Literal 01`` () =
        assertCapturedType "int" "let {selstart}i{selend} = 1"

    [<Fact>]
    let ``Wild 01`` () =
        assertCapturedType "int" "let {selstart}_{selend} = 1"
