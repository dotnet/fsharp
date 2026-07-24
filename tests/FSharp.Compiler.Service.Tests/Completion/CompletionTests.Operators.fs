module FSharp.Compiler.Service.Tests.CompletionOperatorsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AdjacentToDot_01`` () =
    let info = Checker.getCompletionInfo "System.Console.{caret}."

    assertHasItemWithNames [ "BackgroundColor" ] info

[<Fact>]
let ``RangeOperator.IncorrectUsage`` () =
    let info2Dots = Checker.getCompletionInfo "..{caret}"
    Assert.Equal(0, info2Dots.Items.Length)

    let info3Dots = Checker.getCompletionInfo "...{caret}"
    Assert.Equal(0, info3Dots.Items.Length)

[<Fact>]
let ``RangeOperator.CorrectUsage`` () =
    let singleLine = Checker.getCompletionInfo "let _ = [1..{caret}]"
    assertHasItemWithNames [ "abs" ] singleLine

    let multiLine =
        Checker.getCompletionInfo
            """[
   1
    ..{caret}
]"""

    assertHasItemWithNames [ "abs" ] multiLine

[<Fact>]
let ``Array.AfterOperator...Bug65732_A`` () =
    let info = Checker.getCompletionInfo "let r = [1 .. System.{caret}Int32.MaxValue]"

    assertHasItemWithNames [ "Int32" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Theory>]
[<InlineData("let r = [System.Int32.MaxValue..{caret}42]")>]
[<InlineData("let r = [System.Int32.MaxValue..{caret} 42]")>]
[<InlineData("let r = [System.Int32.MaxValue ..{caret} 42]")>]
[<InlineData("let r = [System.Int32.MaxValue..{caret}]")>]
[<InlineData("let r = [System.Int32.MaxValue ..{caret} ]")>]
let ``Array.AfterOperator...Bug65732_B_C_D`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "abs" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact>]
let ``Dot.AfterOperator.Bug69159`` () =
    let info = Checker.getCompletionInfo "let x1 = [|0..1..10|].{caret}"

    assertHasItemWithNames [ "Length" ] info
    assertHasNoItemsWithNames [ "abs" ] info
