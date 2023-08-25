// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ProposeUppercaseLabelTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ProposeUppercaseLabelCodeFixProvider()

[<Fact>]
let ``Fixes FS0053 for discriminated unions`` () =
    let code =
        """
type MyNumber = number of int
"""

    let expected =
        Some
            {
                Message = "Replace with 'Number'"
                FixedCode =
                    """
type MyNumber = Number of int
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fix FS0053 for exceptions`` () =
    let code =
        """
exception lowException of string
"""

    let expected =
        Some
            {
                Message = "Replace with 'LowException'"
                FixedCode =
                    """
exception LowException of string
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
