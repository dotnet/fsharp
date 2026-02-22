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
let myList = [ 1; 2; 3 ]
let first = myList[2]
"""

    let expected =
        Some
            {
                Message = "Add . for indexer access."
                FixedCode =
                    """
let myList = [ 1; 2; 3 ]
let first = myList.[2]
"""
            }

    // The real FS3217 diagnostic spans just the identifier (e.g., "myList"), not "myList[2]"
    // The codefix expands forward to find '[' then replaces span with "text."
    let actual = codeFix |> tryFix code (Manual("= myList", "FS3217"))

    Assert.Equal(expected, actual)
