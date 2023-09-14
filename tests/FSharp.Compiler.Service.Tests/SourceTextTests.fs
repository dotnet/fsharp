module FSharp.Compiler.Service.Tests.SourceTextTests

open FSharp.Compiler.Text
open NUnit.Framework

[<Test>]
let ``Select text from a single line via the range`` () =
    let sourceText = SourceText.ofString """
let a = 2
"""
    let m = Range.mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 2 5)
    let v = sourceText.GetSubTextFromRange m
    Assert.AreEqual("a", v)

[<Test>]
let ``Select text from multiple lines via the range`` () =
    let sourceText = SourceText.ofString """
let a b c =
    // comment
    2
"""
    let m = Range.mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 4 5)
    let v = sourceText.GetSubTextFromRange m
    let sanitized = v.Replace("\r", "")
    Assert.AreEqual("a b c =\n    // comment\n    2", sanitized)
