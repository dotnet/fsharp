// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddMissingEqualsToTypeDefinitionTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddMissingEqualsToTypeDefinitionCodeFixProvider()
let private diagnostic = 3360 // Unexpected token in type def...

[<Fact>]
let ``Fixes FS0360 for missing equals in type def - simple types`` () =
    let code =
        """
type Song { Artist : string; Title : int }
"""

    let expected =
        Some
            {
                Message = "Add missing '=' to type definition"
                FixedCode =
                    """
type Song = { Artist : string; Title : int }
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0360 for missing equals in type def - records`` () =
    let code =
        """
type Name Name of string
"""

    let expected =
        Some
            {
                Message = "Add missing '=' to type definition"
                FixedCode =
                    """
type Name = Name of string
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
// apparently this throws the same error hah
let ``Doesn't fix FS0360 for invalid interface defs`` () =
    let code =
        """
type IA<'b> = 
    abstract Foo : int -> int

type IB<'b> = 
    inherit IA<'b>
    inherit IA<int>
"""

    let expected = None

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
