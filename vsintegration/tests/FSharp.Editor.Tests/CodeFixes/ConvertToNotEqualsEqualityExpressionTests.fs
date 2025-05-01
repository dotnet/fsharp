// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertToNotEqualsEqualityExpressionTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ConvertToNotEqualsEqualityExpressionCodeFixProvider()

[<Fact>]
let ``Fixes FS0043 for C# inequality operator`` () =
    let code =
        """
let areNotEqual (x: int) (y: int) = x != y
"""

    let expected =
        Some
            {
                Message = "Use '<>' for inequality check"
                FixedCode =
                    """
let areNotEqual (x: int) (y: int) = x <> y
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0043 for random unsupported values`` () =
    let code =
        """
type RecordType = { X : int }

let x : RecordType = null
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
