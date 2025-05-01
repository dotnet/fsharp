// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveReturnOrYieldTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RemoveReturnOrYieldCodeFixProvider()

// TODO: write some negative tests here

[<Fact>]
let ``Fixes FS0747 - yield`` () =
    let code =
        """
let answer question =
    yield 42
"""

    let expected =
        Some
            {
                Message = "Remove 'yield'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0747 - yield!`` () =
    let code =
        """
let answer question =
    yield! 42
"""

    let expected =
        Some
            {
                Message = "Remove 'yield!'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0748 - return`` () =
    let code =
        """
let answer question =
    return 42
"""

    let expected =
        Some
            {
                Message = "Remove 'return'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0748 - return!`` () =
    let code =
        """
let answer question =
    return! 42
"""

    let expected =
        Some
            {
                Message = "Remove 'return!'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Handles spaces`` () =
    let code =
        """
let answer question =
    yield      42
"""

    let expected =
        Some
            {
                Message = "Remove 'yield'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Handles new lines`` () =
    let code =
        """
let answer question =
    return!
        42
"""

    let expected =
        Some
            {
                Message = "Remove 'return!'"
                FixedCode =
                    """
let answer question =
    42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
