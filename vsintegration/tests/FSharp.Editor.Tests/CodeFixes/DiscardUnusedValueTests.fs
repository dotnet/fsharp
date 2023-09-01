// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.DiscardUnusedValueTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RenameUnusedValueWithUnderscoreCodeFixProvider()

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Fixes let bindings in classes`` diag =
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

    let actual = codeFix |> tryFix code (Manual("blah", diag))

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Fixes let bindings within let bindings`` diag =
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

    let actual = codeFix |> tryFix code (Manual("blah", diag))

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Doesn't fix class identifiers`` diag =
    let code =
        """
type T() as this = class end
"""

    let expected = None

    let actual = codeFix |> tryFix code (Manual("this", diag))

    Assert.Equal(expected, actual)

// TODO: add tests for scenarios with signature files
