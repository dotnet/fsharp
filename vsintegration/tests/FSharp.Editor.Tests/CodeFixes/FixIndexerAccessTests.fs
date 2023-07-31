// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.FixIndexerAccessTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RemoveDotFromIndexerAccessOptInCodeFixProvider()

[<Fact>]
let ``Fixes FS3366`` () =
    let code =
        """
let list = [ 42 ]

let _ = list.[0]
"""

    let expected =
        Some
            {
                Message = "The syntax 'arr.[idx]' is now revised to 'arr[idx]'. Please update your code."
                FixedCode =
                    """
let list = [ 42 ]

let _ = list[0]
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--warnon:3366")

    Assert.Equal(expected, actual)
