// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Xunit
open HintTestFramework
open FSharp.Test

// just some kind of higher level testing
module OverallHintExperienceTests =

    [<Fact>]
    let ``Baseline hints`` () =
        let code =
            """
type Song = { Artist: string; Title: string }
let whoSings song = song.Artist

let artist = whoSings { Artist = "Květy"; Title = "Je podzim" }

type Shape =
    | Square of side: int
    | Rectangle of width: int * height: int

let a = Square 1
let b = Rectangle (1, 2)

type C (blahFirst: int) =
    member _.Normal (what: string) = 1

let a = C 1
let cc = a.Normal "hmm"
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = ": Song"
                    Location = (2, 18)
                    Tooltip = "type Song"
                }
                {
                    Content = ": string "
                    Location = (2, 19)
                    Tooltip = "type string"
                }
                {
                    Content = "song = "
                    Location = (4, 23)
                    Tooltip = "parameter song"
                }
                {
                    Content = ": string"
                    Location = (4, 11)
                    Tooltip = "type string"
                }
                {
                    Content = "side = "
                    Location = (10, 16)
                    Tooltip = "field side"
                }
                {
                    Content = ": Shape"
                    Location = (10, 6)
                    Tooltip = "type Shape"
                }
                {
                    Content = "width = "
                    Location = (11, 20)
                    Tooltip = "field width"
                }
                {
                    Content = "height = "
                    Location = (11, 23)
                    Tooltip = "field height"
                }
                {
                    Content = ": Shape"
                    Location = (11, 6)
                    Tooltip = "type Shape"
                }
                {
                    Content = ": int "
                    Location = (14, 36)
                    Tooltip = "type int"
                }
                {
                    Content = "blahFirst = "
                    Location = (16, 11)
                    Tooltip = "parameter blahFirst"
                }
                {
                    Content = ": C"
                    Location = (16, 6)
                    Tooltip = "type C"
                }
                {
                    Content = "what = "
                    Location = (17, 19)
                    Tooltip = "parameter what"
                }
                {
                    Content = ": int"
                    Location = (17, 7)
                    Tooltip = "type int"
                }
            ]

        let actual = getAllHints document

        actual |> Assert.shouldBeEquivalentTo expected
