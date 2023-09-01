// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveUnusedBindingTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RemoveUnusedBindingCodeFixProvider()

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Fixes FS1182 - let bindings in classes`` diag =
    let code =
        """
type T() =
    let blah = "test"
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type T() =
    
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
                Message = "Remove unused binding"
                FixedCode =
                    """
let f() =
    
    42
"""
            }

    let actual = codeFix |> tryFix code (Manual("blah", diag))

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData(" ", " ")>]
[<InlineData("   ", " ")>]
[<InlineData(" ", "   ")>]
[<InlineData("   ", "   ")>]
let ``Fixes FS1182 - class identifiers`` preSpaces postSpaces =
    let code =
        $"""
type C() as{preSpaces}this{postSpaces}= class end
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type C() = class end
"""
            }

    let actual = codeFix |> tryFix code (Manual("this", "FS1182"))

    Assert.Equal(expected, actual)

// a little copypaste never killed nobody
[<Theory>]
[<InlineData(" ", " ")>]
[<InlineData("   ", " ")>]
[<InlineData(" ", "   ")>]
[<InlineData("   ", "   ")>]
let ``Fixes IDE0059 - class identifiers`` preSpaces postSpaces =
    let code =
        $"""
type C() as{preSpaces}this{postSpaces}= class end
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type C() = class end
"""
            }

    let actual = codeFix |> tryFix code (Manual("this", "IDE0059"))

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "IDE0059">]
[<InlineData "FS1182">]
let ``Doesn't fix member identifiers`` diag =
    let code =
        $"""
type T() =
    member x.FortyTwo = 42
"""

    let expected = None

    let actual = codeFix |> tryFix code (Manual("x", diag))

    Assert.Equal(expected, actual)
