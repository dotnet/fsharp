module FSharp.Compiler.Service.Tests.SourceTextTests

open System
open System.Threading.Tasks
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open Xunit

[<Fact>]
let ``Select text from a single line via the range`` () =
    let sourceText = SourceText.ofString """
let a = 2
"""
    let m = mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 2 5)
    let v = sourceText.GetSubTextFromRange m
    Assert.Equal("a", v)

[<Fact>]
let ``Select text from multiple lines via the range`` () =
    let sourceText = SourceText.ofString """
let a b c =
    // comment
    2
"""
    let m = mkRange "Sample.fs" (Position.mkPos 2 4) (Position.mkPos 4 5)
    let v = sourceText.GetSubTextFromRange m
    let sanitized = v.Replace("\r", "")
    Assert.Equal("a b c =\n    // comment\n    2", sanitized)

[<Fact>]
let ``Inconsistent return carriage return correct text`` () =
    let sourceText =  SourceText.ofString "let a =\r\n    // foo\n    43"
    let m = mkRange "Sample.fs" (Position.mkPos 1 4) (Position.mkPos 3 6)
    let v = sourceText.GetSubTextFromRange m
    let sanitized = v.Replace("\r", "")
    Assert.Equal("a =\n    // foo\n    43", sanitized)

[<Fact>]
let ``Zero range should return empty string`` () =
    let sourceText = SourceText.ofString "a"
    let v = sourceText.GetSubTextFromRange range0
    Assert.Equal(String.Empty, v)

[<Fact>]
let ``Invalid range should throw argument exception`` () =
    let sourceText = SourceText.ofString "a"
    let mInvalid = mkRange "Sample.fs" (Position.mkPos 3 6) (Position.mkPos 1 4)
    Assert.Throws<ArgumentException>(fun () -> sourceText.GetSubTextFromRange mInvalid |> ignore)

[<Literal>]
let private NonAsciiText = "héllo, Привіт, 世界, \U0001F600"

let private checksumText (sourceText: ISourceTextNew) =
    BitConverter.ToString(Seq.toArray (sourceText.GetChecksum())).Replace("-", "")

[<Theory>]
[<InlineData("", "D41D8CD98F00B204E9800998ECF8427E")>]
[<InlineData("abc", "900150983CD24FB0D6963F7D28E17F72")>]
[<InlineData(NonAsciiText, "8A0CFABFC99225203C28816747780A86")>]
let ``GetChecksum is the MD5 of the UTF-8 text`` (text: string) (expected: string) =
    Assert.Equal(expected, checksumText (SourceTextNew.ofString text))
    Assert.Equal(expected, checksumText (SourceTextNew.ofISourceText (SourceText.ofString text)))

[<Fact>]
let ``GetChecksum gives the same value from repeated parallel calls`` () =
    let texts = [| ""; "abc"; NonAsciiText; String('x', 5000) |]
    let expected = texts |> Array.map (SourceTextNew.ofString >> checksumText)

    Parallel.For(
        0,
        4000,
        fun i ->
            let index = i % texts.Length
            Assert.Equal(expected[index], checksumText (SourceTextNew.ofString texts[index]))
    )
    |> ignore
