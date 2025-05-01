// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangeRefCellDerefToNotExpressionTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ChangeRefCellDerefToNotExpressionCodeFixProvider()

[<Fact>]
let ``Fixes FS0001 for invalid negation syntax`` () =
    let code =
        """
let myNot (value: bool) = !value
"""

    let expected =
        Some
            {
                Message = "Use 'not' to negate expression"
                FixedCode =
                    """
let myNot (value: bool) = not value
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0001 for random type mismatch`` () =
    let code =
        """
let one, two = 1, 2, 3
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
