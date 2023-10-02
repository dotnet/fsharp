// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.UseMutationWhenValueIsMutableTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = UseMutationWhenValueIsMutableCodeFixProvider()

[<Fact>]
let ``Fixes FS0020 for let bindings`` () =
    let code =
        """
let mutable x = 42
x = 43
"""

    let expected =
        Some
            {
                Message = "Use '<-' to mutate value"
                FixedCode =
                    """
let mutable x = 42
x <- 43
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0020 for parameters`` () =
    let code =
        """
let f x =
    x = 42
    ()
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData """
let square x = x * x
square 32
""">]
[<InlineData """
let band = {| Name = "R.E.M." |}
band""">]
let ``Doesn't fix unrelated FS0020`` code =
    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
