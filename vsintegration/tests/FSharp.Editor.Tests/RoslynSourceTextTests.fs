// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis.Text

module RoslynSourceTextTests =

    [<Fact>]
    let SourceText () =
        let text = "test\ntest2\r\ntest3\n\ntest4\ntest5\rtest6\n"
        let sourceText = SourceText.From(text).ToFSharpSourceText()

        Assert.Equal("test", sourceText.GetLineString(0))
        Assert.Equal("test2", sourceText.GetLineString(1))
        Assert.Equal("test3", sourceText.GetLineString(2))
        Assert.Equal("", sourceText.GetLineString(3))
        Assert.Equal("test4", sourceText.GetLineString(4))
        Assert.Equal("test5", sourceText.GetLineString(5))
        Assert.Equal("test6", sourceText.GetLineString(6))
        Assert.Equal("", sourceText.GetLineString(7))
        Assert.Equal(8, sourceText.GetLineCount())

        let (count, length) = sourceText.GetLastCharacterPosition()
        Assert.Equal(8, count)
        Assert.Equal(0, length)

        Assert.True(sourceText.SubTextEquals("test", 0))
        Assert.True(sourceText.SubTextEquals("test2", 5))
        Assert.True(sourceText.SubTextEquals("test3", 12))

        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("test", -1) |> ignore)
        |> ignore

        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("test", text.Length) |> ignore)
        |> ignore

        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals("", 0) |> ignore)
        |> ignore

        Assert.Throws<ArgumentException>(fun () -> sourceText.SubTextEquals(text + text, 0) |> ignore)
        |> ignore

        Assert.False(sourceText.SubTextEquals("test", 1))
        Assert.False(sourceText.SubTextEquals("test", 4))
        Assert.False(sourceText.SubTextEquals("test", 11))
