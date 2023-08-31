// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ReplaceWithSuggestionTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ReplaceWithSuggestionCodeFixProvider()

[<Fact>]
let ``Fixes FS0039 for mistyped record field names`` () =
    let code =
        """
type Song = { Title : string }

let song = { Titel = "Jigsaw Falling Into Place" }
"""

    let expected =
        Some
            {
                Message = "Replace with 'Title'"
                FixedCode =
                    """
type Song = { Title : string }

let song = { Title = "Jigsaw Falling Into Place" }
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for mistyped type names`` () =
    let code =
        """
type Song = { Title : string }

let someSong : Wrong = { Title = "The Narcissist" }
"""

    let expected =
        Some
            {
                Message = "Replace with 'Song'"
                FixedCode =
                    """
type Song = { Title : string }

let someSong : Song = { Title = "The Narcissist" }
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for out of scope stuff`` () =
    let code =
        """
module Module1 =
    type Song = { Title : string }

module Module2 = 
    let song = { Titel = "Jigsaw Falling Into Place" }
"""

    let expected = None

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

[<Fact>]
let ``Fixes FS0495`` () =
    let code =
        """
type Song(title: string) =
    member _.Title = title

let song = Song(titel = "Under The Milky Way")
"""

    let expected =
        Some
            {
                Message = "Replace with 'title'"
                FixedCode =
                    """
type Song(title: string) =
    member _.Title = title

let song = Song(title = "Under The Milky Way")
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
