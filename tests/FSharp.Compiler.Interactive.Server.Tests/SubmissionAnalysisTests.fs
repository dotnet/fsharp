// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The rule that decides whether Enter submits an interaction or adds another line.
module FSharp.Compiler.Interactive.Server.Tests.SubmissionAnalysisTests

open Xunit

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Interactive

let private scanners = FSharpLexicalScannerFactory() :> ILexicalScannerFactory

let private scenario (completes: bool) (name: string) (texts: string list) : obj[] list =
    texts |> List.map (fun text -> [| box name; box completes; box text |])

let private submits = scenario true
let private takesAnotherLine = scenario false

let scenarios: obj[] seq =
    [
        submits "an explicit terminator always submits, even mid-construct" [ "1 + 1;;"; "let f x =\n    x + 1\n;;"; "let xs = [1; 2];;" ]
        submits "a finished expression submits without a terminator" [ "1 + 1"; "printfn \"hi\""; "let x = 40"; "let f a b =\n    a + b" ]
        takesAnotherLine "a line that visibly continues" [ "let x ="; "fun x ->"; "1 +"; "if true then" ]
        takesAnotherLine "an unclosed bracket" [ "printfn (\"a\""; "let xs = [1; 2"; "let xs = [| 1; 2"; "let r = {| A = 1" ]
        submits "closed brackets do not hold the submission open" [ "printfn (\"a\")"; "let xs = [1; 2]"; "let xs = [| 1; 2 |]" ]
        takesAnotherLine "an unterminated string or comment" [ "let s = \"abc"; "(* comment" ]
        submits "brackets and terminators inside a string do not count" [ "let s = \"([{\""; "let s = \"a;;b\"" ]
        submits "a string literal can end a submission: the last token is the literal, not the '=' before it" [ "let greeting = \"hello\"" ]
        takesAnotherLine "a terminator inside a string is not a terminator" [ "let s = \";;" ]
        submits "a comment does not change what the last token was" [ "let x = 1 // ([\n"; "(* c *) let x = 1" ]
        takesAnotherLine "a comment does not change what the last token was" [ "let x = // nothing yet" ]
        submits "an empty submission, which the window uses to start a session that has gone away" [ ""; "   \n  " ]
    ]
    |> Seq.concat

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Enter submits only what is visibly finished`` (scenario: string) (completes: bool) (text: string) =
    Assert.True((SubmissionAnalysis.isComplete scanners text = completes), $"{scenario}: <<{text}>>")

[<Theory>]
[<InlineData("1 + 1;;", "1 + 1;;")>]
[<InlineData("1 + 1", "1 + 1\n;;")>]
[<InlineData("let s = \"a;;b\"", "let s = \"a;;b\"\n;;")>]
let ``the terminator is added only when it is missing`` (text: string) (expected: string) =
    Assert.Equal(expected, SubmissionAnalysis.withTerminator scanners text)
