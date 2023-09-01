// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.PrefixUnusedValueTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = PrefixUnusedValueWithUnderscoreCodeFixProvider()

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
                Message = "Prefix 'blah' with underscore"
                FixedCode =
                    """
type T() =
    let _blah = 42
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
                Message = "Prefix 'blah' with underscore"
                FixedCode =
                    """
let f() =
    let _blah = "test"
    42
"""
            }

    let actual = codeFix |> tryFix code (Manual("blah", diag))

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Fixes class identifiers`` diag =
    let code =
        """
type T() as this = class end
"""

    let expected =
        Some
            {
                Message = "Prefix 'this' with underscore"
                FixedCode =
                    """
type T() as _this = class end
"""
            }

    let actual = codeFix |> tryFix code (Manual("this", diag))

    Assert.Equal(expected, actual)

// TODO: add tests for scenarios with signature files
