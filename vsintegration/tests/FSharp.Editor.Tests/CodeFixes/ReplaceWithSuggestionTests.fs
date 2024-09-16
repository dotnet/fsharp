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

let song = { Title_ = "Jigsaw Falling Into Place" }
"""

    let expected =
        [
            {
                Message = "Replace with 'Title'"
                FixedCode =
                    """
type Song = { Title : string }

let song = { Title = "Jigsaw Falling Into Place" }
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 for mistyped type names`` () =
    let code =
        """
type Song = { Title : string }

let someSong : Wrong = { Title = "The Narcissist" }
"""

    let expected =
        [
            {
                Message = "Replace with 'Song'"
                FixedCode =
                    """
type Song = { Title : string }

let someSong : Song = { Title = "The Narcissist" }
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0039 - multiple suggestions`` () =
    let code =
        """
type TheType1() = class end
type TheType3() = class end

let test = TheType2()
"""

    let expected =
        [
            {
                Message = "Replace with 'TheType1'"
                FixedCode =
                    """
type TheType1() = class end
type TheType3() = class end

let test = TheType1()
"""
            }
            {
                Message = "Replace with 'TheType3'"
                FixedCode =
                    """
type TheType1() = class end
type TheType3() = class end

let test = TheType3()
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

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

    let expected = []

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined stuff`` () =
    let code =
        """
let f = g
"""

    let expected = []

    let actual = codeFix |> multiFix code Auto

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
        [
            {
                Message = "Replace with 'title'"
                FixedCode =
                    """
type Song(title: string) =
    member _.Title = title

let song = Song(title = "Under The Milky Way")
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS1129`` () =
    let code =
        """
type SomeType = { TheField : string }

let f x =
    match x with
    | { TheField = "A" } -> true
    | { TheField_ = "B" } -> true
    | _ -> false
"""

    let expected =
        [
            {
                Message = "Replace with 'TheField'"
                FixedCode =
                    """
type SomeType = { TheField : string }

let f x =
    match x with
    | { TheField = "A" } -> true
    | { TheField = "B" } -> true
    | _ -> false
"""
            }
        ]

    let actual = codeFix |> multiFix code Auto

    Assert.Equal(expected, actual)
