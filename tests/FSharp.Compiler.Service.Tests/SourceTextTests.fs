module FSharp.Compiler.Service.Tests.SourceTextTests

open System
open FSharp.Compiler.Text
open Xunit

[<Fact>]
let ``Select text from a single line via the range`` () =
    let sourceText = SourceText.ofString """
let a = 2
"""
    let m = Range.mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 2 5)
    let v = sourceText.GetSubTextFromRange m
    Assert.Equal("a", v)

[<Fact>]
let ``Select text from multiple lines via the range`` () =
    let sourceText = SourceText.ofString """
let a b c =
    // comment
    2
"""
    let m = Range.mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 4 5)
    let v = sourceText.GetSubTextFromRange m
    let sanitized = v.Replace("\r", "")
    Assert.Equal("a b c =\n    // comment\n    2", sanitized)

[<Fact>]
let ``Inconsistent return carriage return correct text`` () =
    let sourceText =  SourceText.ofString "let a =\r\n    // foo\n    43"
    let m = Range.mkRange "Sample.fs" (Position.mkPos 1 4) (Position.mkPos 3 6) 
    let v = sourceText.GetSubTextFromRange m
    let sanitized = v.Replace("\r", "")
    Assert.Equal("a =\n    // foo\n    43", sanitized)

[<Fact>]
let ``Zero range should return empty string`` () =
    let sourceText = SourceText.ofString "a"
    let v = sourceText.GetSubTextFromRange Range.Zero
    Assert.Equal(String.Empty, v)
    
[<Fact>]
let ``Invalid range should throw argument exception`` () =
    let sourceText = SourceText.ofString "a"
    let mInvalid = Range.mkRange "Sample.fs" (Position.mkPos 3 6) (Position.mkPos 1 4)
    Assert.Throws<ArgumentException>(fun () -> sourceText.GetSubTextFromRange mInvalid |> ignore)
