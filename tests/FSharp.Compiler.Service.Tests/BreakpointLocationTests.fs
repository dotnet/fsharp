module FSharp.Compiler.Service.Tests.BreakpointLocationTests

open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Test.Assert
open Xunit

let assertBreakpointRange markedSource =
    let context, parseResults = Checker.getParseResultsWithContext markedSource
    let breakpointRange = parseResults.ValidateBreakpointLocation(context.CaretPos).Value
    let selected = context.SelectedRange.Value
    let expectedRange = mkFileIndexRange breakpointRange.FileIndex selected.Start selected.End

    breakpointRange |> shouldEqual expectedRange

[<Fact>]
let ``Let - Function - Body 01`` () =
    assertBreakpointRange """
let f () =
    {selstart}1{selend}
"""

[<Fact>]
let ``Seq 01`` () =
    assertBreakpointRange """
do
    {selstart}1{selend}
    2
"""

[<Fact>]
let ``Seq 02`` () =
    assertBreakpointRange """
do
    1
    {selstart}2{selend}
"""

[<Fact>]
let ``Lambda 01`` () =
    assertBreakpointRange """
[""] |> List.map (fun s -> {selstart}s.Lenght{selend}) 
"""

[<Fact>]
let ``Dot lambda 01`` () =
    assertBreakpointRange """
[""] |> List.map {selstart}_.Lenght{selend} 
"""

[<Fact>]
let ``Dot lambda 02`` () =
    assertBreakpointRange """
[""] |> List.map {selstart}_.ToString().Length{selend} 
"""
