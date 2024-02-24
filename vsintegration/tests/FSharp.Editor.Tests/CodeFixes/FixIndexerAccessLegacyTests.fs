// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.FixIndexerAccessLegacyTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = LegacyFixAddDotToIndexerAccessCodeFixProvider()

[<Fact>]
let ``Fixes FS3217`` () =
    let code =
        """
let list = [ 1; 2; 3 ]
let first = list[2]
"""

    let expected =
        Some
            {
                Message = "Add . for indexer access."
                FixedCode =
                    """
let list = [ 1; 2; 3 ]
let first = list.[2]
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--langversion:5")

    Assert.Equal(expected, actual)
