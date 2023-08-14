// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddMissingFunKeywordTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddMissingFunKeywordCodeFixProvider()

[<Fact>]
let ``Fixes FS0010 for missing fun keyword`` () =
    let code =
        """
let gettingEven numbers =
    numbers
    |> Seq.filter (x -> x / 2 = 0)
"""

    let expected =
        Some
            {
                Message = "Add missing 'fun' keyword"
                FixedCode =
                    """
let gettingEven numbers =
    numbers
    |> Seq.filter (fun x -> x / 2 = 0)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData("""
=
""")>]
[<InlineData "let f x = 
    match x with
    | _ ->
        let _ = [
            x with
        ]
">]
let ``Doesn't fix FS0010 for random unexpected symbols`` code =
    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
