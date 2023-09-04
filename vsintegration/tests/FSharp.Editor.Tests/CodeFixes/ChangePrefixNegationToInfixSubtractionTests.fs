﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangePrefixNegationToInfixNegationTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ChangePrefixNegationToInfixSubtractionCodeFixProvider()

[<Theory>]
[<InlineData " ">]
[<InlineData "   ">]
let ``Fixes FS0003 for accidental negation`` spaces =
    let code =
        $"""
let f (numbers: 'a array) = 
    for x = 0 to numbers.Length{spaces}-1 do 
        printfn "%%i" x
"""

    let expected =
        Some
            {
                Message = "Use subtraction instead of negation"
                FixedCode =
                    $"""
let f (numbers: 'a array) = 
    for x = 0 to numbers.Length{spaces}- 1 do 
        printfn "%%i" x
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0003 for random incorrect operator usage`` () =
    let code =
        """
let x = 1 (+) 2
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
