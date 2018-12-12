// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

open Microsoft.FSharp.Compiler.Text

[<TestFixture>]
module SourceTextTests =

    [<Test>]
    let StringText () =
        let text = "test\ntest2\r\ntest3\n\ntest4\ntest5\rtest6\n"
        let sourceText = SourceText.ofString text
        let lines = sourceText.Lines

        Assert.AreEqual("test\n", sourceText.GetTextString(lines.[0]))
        Assert.AreEqual("test2\r\n", sourceText.GetTextString(lines.[1]))
        Assert.AreEqual("test3\n", sourceText.GetTextString(lines.[2]))
        Assert.AreEqual("\n", sourceText.GetTextString(lines.[3]))
        Assert.AreEqual("test4\n", sourceText.GetTextString(lines.[4]))
        Assert.AreEqual("test5\r", sourceText.GetTextString(lines.[5]))
        Assert.AreEqual("test6\n", sourceText.GetTextString(lines.[6]))