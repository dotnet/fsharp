// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.DiscardUnusedValueTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RenameUnusedValueWithUnderscoreCodeFixProvider()

[<Fact>]
let ``Fixes FS1182 - let bindings in classes`` () =
    let code =
        """
type T() =
    let blah = 42
"""

    let expected =
        Some
            {
                Message = "Rename 'blah' to '_'"
                FixedCode =
                    """
type T() =
    let _ = 42
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--warnon:1182")

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS1182 - let bindings within let bindings`` () =
    let code =
        """
let f() =
    let blah = "test"
    42
"""

    let expected =
        Some
            {
                Message = "Rename 'blah' to '_'"
                FixedCode =
                    """
let f() =
    let _ = "test"
    42
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--warnon:1182")

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS1182 - class identifiers`` () =
    let code =
        """
type T() as this = class end
"""

    let expected = None

    let actual = codeFix |> tryFix code (WithOption "--warnon:1182")

    Assert.Equal(expected, actual)

// TODO: add tests for scenarios with signature files
