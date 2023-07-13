// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertToSingleEqualsEqualityExpressionTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ConvertToSingleEqualsEqualityExpressionCodeFixProvider()
let private diagnostic = 0043 // The type doesn't support the value...

[<Fact>]
let ``Fixes FS0043 for C# equality operator`` () =
    let code =
        """
let areEqual (x: int) (y: int) = x == y
"""

    let expected =
        Some
            {
                Message = "Use '=' for equality check"
                FixedCode =
                    """
let areEqual (x: int) (y: int) = x = y
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0043 for random unsupported values`` () =
    let code =
        """
type RecordType = { X : int }

let x : RecordType = null
"""

    let expected = None

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
