// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertCSharpLambdaToFSharpLambdaTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ConvertCSharpLambdaToFSharpLambdaCodeFixProvider()

[<Fact>]
let ``Fixes FS0039 for lambdas`` () =
    let code =
        """
let incAll = List.map (n => n + 1)
"""

    let expected =
        Some
            {
                Message = "Use F# lambda syntax"
                FixedCode =
                    """
let incAll = List.map (fun n -> n + 1)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined stuff`` () =
    let code =
        """
let f = g
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
