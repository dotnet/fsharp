// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangeEqualsInFieldTypeToColonTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ChangeEqualsInFieldTypeToColonCodeFixProvider()

[<Fact>]
let ``Fixes FS0010 for = in record definitions`` () =
    let code =
        """
type Band = { Name = string }
"""

    let expected =
        Some
            {
                Message = "Use ':' for type in field declaration"
                FixedCode =
                    """
type Band = { Name : string }
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0010 for = in anonymous record definitions`` () =
    let code =
        """
type Band = {| Name = string |}
"""

    let expected =
        Some
            {
                Message = "Use ':' for type in field declaration"
                FixedCode =
                    """
type Band = {| Name : string |}
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData("""
type Band = { Name open string }
""")>]
[<InlineData("""
type Band = {| Name open string |}
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

[<Theory>]
[<InlineData("""
=
""")>]
[<InlineData("""
type Band = { Name: string }

= open
""")>]
[<InlineData("""
type Band = {| Name: string |}

= open
""")>]
let ``Doesn't fix FS0010 for = in places other than within record field definitions`` code =
    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
