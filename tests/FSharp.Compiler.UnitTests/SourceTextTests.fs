// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

open FSharp.Compiler.Text

[<TestFixture>]
module SourceTextTests =

    [<Test>]
    let StringText () =
        let text = "test\ntest2\r\ntest3\n\ntest4\ntest5\rtest6\n"
        let sourceText = SourceText.ofString text

        Assert.AreEqual("test",  sourceText.GetLineString(0))
        Assert.AreEqual("test2", sourceText.GetLineString(1))
        Assert.AreEqual("test3", sourceText.GetLineString(2))
        Assert.AreEqual("",      sourceText.GetLineString(3))
        Assert.AreEqual("test4", sourceText.GetLineString(4))
        Assert.AreEqual("test5", sourceText.GetLineString(5))
        Assert.AreEqual("test6", sourceText.GetLineString(6))
        Assert.AreEqual("",      sourceText.GetLineString(7))
        Assert.AreEqual(8,       sourceText.GetLineCount())

        let (count, length) = sourceText.GetLastCharacterPosition()
        Assert.AreEqual(8, count)
        Assert.AreEqual(0, length)

        Assert.True(sourceText.SubTextEquals("test", 0))
        Assert.True(sourceText.SubTextEquals("test2", 5))
        Assert.True(sourceText.SubTextEquals("test3", 12))

        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("test", -1) |> ignore) |> ignore
        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("test", text.Length) |> ignore) |> ignore
        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("", 0) |> ignore) |> ignore
        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals(text + text, 0) |> ignore) |> ignore

        Assert.False(sourceText.SubTextEquals("test", 1))
        Assert.False(sourceText.SubTextEquals("test", 4))
        Assert.False(sourceText.SubTextEquals("test", 11))