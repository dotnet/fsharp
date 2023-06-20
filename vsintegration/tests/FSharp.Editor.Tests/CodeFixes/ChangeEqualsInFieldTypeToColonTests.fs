﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangeEqualsInFieldTypeToColonTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ChangeEqualsInFieldTypeToColonCodeFixProvider()
let private diagnostic = 0010 // Unexpected symbol...

[<Fact>]
let ``Fixes FS0010 for = in types`` () =
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

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0010 for random unexpected symbols`` () =
    let code =
        """
type Band = { Name open string }
"""

    let expected = None

    let actual = codeFix |> tryFix code diagnostic

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
type Band = { Name: string = }
""")>]
let ``Doesn't fix FS0010 for = in places other than within record field definitions`` code =
    let expected = None

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
