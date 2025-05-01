module FSharp.Compiler.Service.Tests.RangeTests

open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Test.Assert
open Xunit

[<Fact>]
let ``withStartEnd Test`` () =
    let z = Range.Zero
    let newStart = mkPos 2 4
    let newEnd = mkPos 7 23
    let r = withStartEnd newStart newEnd z
    r.Start |> shouldEqual newStart
    r.End |> shouldEqual newEnd
    
[<Fact>]
let ``withStart Test`` () =
    let z = Range.Zero
    let newStart = mkPos 2 4
    let r = withStart newStart z
    r.Start |> shouldEqual newStart

[<Fact>]
let ``withEnd Test`` () =
    let z = Range.Zero
    let newEnd = mkPos 2 4
    let r = withEnd newEnd z
    r.End |> shouldEqual newEnd

[<Fact>]
let ``shiftStart Test`` () =
    let z = Range.Zero
    let lineDelta = 10
    let columnDelta = 20
    let r = shiftStart lineDelta columnDelta z
    r.Start.Column |> shouldEqual (z.StartColumn + columnDelta)
    r.Start.Line |> shouldEqual (z.StartLine + lineDelta)
    
[<Fact>]
let ``shiftEnd Test`` () =
    let z = Range.Zero
    let lineDelta = 10
    let columnDelta = 20
    let r = shiftEnd lineDelta columnDelta z
    r.End.Column |> shouldEqual (z.EndColumn + columnDelta)
    r.End.Line |> shouldEqual (z.EndLine + lineDelta)
