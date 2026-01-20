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
let xs = [ 1; 2; 3 ]
let first = xs[2]
"""

    let expected =
        Some
            {
                Message = "Add . for indexer access."
                FixedCode =
                    """
let xs = [ 1; 2; 3 ]
let first = xs.[2]
"""
            }

    // Squiggly must end BEFORE '[' - the codefix expands forward to find '[' then appends '.'
    let actual = codeFix |> tryFix code (Manual("xs", "FS3217"))

    Assert.Equal(expected, actual)
