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

[<Fact>]
let ``Expr - If 01`` () =
    assertCapturedType "int * int" "{selstart}if true then 1, 2 else 1, true{selend}"

[<Fact>]
let ``Expr - Literal 01`` () =
    assertCapturedType "int" "{selstart}1{selend}"

[<Fact>]
let ``Expr - Literal 02`` () =
    assertCapturedType "string" "{selstart}\"\"{selend}"

[<Fact>]
let ``Expr - Tuple 01`` () =
    assertCapturedType "int * int" "{selstart}1, 2{selend}"

[<Fact>]
let ``Expr - Tuple 02`` () =
    assertCapturedType "int * int" "if true then {selstart}1, 2{selend} else 1, true"
