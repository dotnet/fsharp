// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for the rule that decides whether Enter submits an interaction or adds another line.
///
/// The rule is what a user feels on every keystroke, and it is easy to get subtly wrong: a bracket
/// inside a string, a terminator inside a comment, a line that merely looks finished. The analysis
/// is lexical so that it can run on the UI thread, which makes these cases worth pinning down.
module FSharp.Compiler.Interactive.Server.Tests.SubmissionAnalysisTests

open Xunit

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Interactive

// The lexer the Visual Studio tooling runs on, which the window is handed rather than compiling against.
let private scanners = FSharpLexicalScannerFactory() :> ILexicalScannerFactory

let private assertComplete text =
    Assert.True(SubmissionAnalysis.isComplete scanners text, $"expected a complete submission: <<{text}>>")

let private assertIncomplete text =
    Assert.False(SubmissionAnalysis.isComplete scanners text, $"expected an incomplete submission: <<{text}>>")

[<Fact>]
let ``an explicit terminator always submits`` () =
    assertComplete "1 + 1;;"
    assertComplete "let f x =\n    x + 1\n;;"
    // Even mid-construct: the user asked for it.
    assertComplete "let xs = [1; 2];;"

[<Fact>]
let ``a finished expression submits without a terminator`` () =
    assertComplete "1 + 1"
    assertComplete "printfn \"hi\""
    assertComplete "let x = 40"
    assertComplete "let f a b =\n    a + b"

[<Fact>]
let ``a line that visibly continues takes another line`` () =
    assertIncomplete "let x ="
    assertIncomplete "fun x ->"
    assertIncomplete "1 +"
    assertIncomplete "if true then"

[<Fact>]
let ``an unclosed bracket takes another line`` () =
    assertIncomplete "printfn (\"a\""
    assertIncomplete "let xs = [1; 2"
    assertIncomplete "let xs = [| 1; 2"
    assertIncomplete "let r = {| A = 1"

[<Fact>]
let ``closed brackets do not hold the submission open`` () =
    assertComplete "printfn (\"a\")"
    assertComplete "let xs = [1; 2]"
    assertComplete "let xs = [| 1; 2 |]"

[<Fact>]
let ``an unterminated string or comment takes another line`` () =
    assertIncomplete "let s = \"abc"
    assertIncomplete "(* comment"

[<Fact>]
let ``brackets and terminators inside a string do not count`` () =
    // The contents of a literal are not code, so neither the brackets nor the ';;' in these change
    // whether the submission is finished.
    assertComplete "let s = \"([{\""
    assertComplete "let s = \"a;;b\""

[<Fact>]
let ``a string literal can end a submission`` () =
    // The last token here is the literal, not the '=' before it.
    assertComplete "let greeting = \"hello\""

[<Fact>]
let ``a terminator inside a string is not a terminator`` () =
    assertIncomplete "let s = \";;"

[<Fact>]
let ``a comment does not change what the last token was`` () =
    assertComplete "let x = 1 // ([\n"
    assertComplete "(* c *) let x = 1"
    assertIncomplete "let x = // nothing yet"

[<Fact>]
let ``an empty submission is allowed through`` () =
    // The window uses an empty submission to start a session that has gone away.
    assertComplete ""
    assertComplete "   \n  "

[<Fact>]
let ``the terminator is added only when it is missing`` () =
    Assert.Equal("1 + 1;;", SubmissionAnalysis.withTerminator scanners "1 + 1;;")
    Assert.Equal("1 + 1\n;;", SubmissionAnalysis.withTerminator scanners "1 + 1")
    // A ';;' that is only part of a string does not count as one.
    Assert.Equal("let s = \"a;;b\"\n;;", SubmissionAnalysis.withTerminator scanners "let s = \"a;;b\"")
