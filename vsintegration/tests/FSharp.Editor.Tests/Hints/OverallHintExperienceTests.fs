// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Xunit
open HintTestFramework
open FSharp.Test

// just some kind of higher level testing
module OverallHintExperienceTests =

    [<Fact>]
    let ``Current baseline hints`` () =
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
                    ToolTip = "42"
                }
                {
                    Content = ": string "
                    Location = (2, 19)
                    ToolTip = "42"
                }
                {
                    Content = "song = "
                    Location = (4, 23)
                    ToolTip = "42"
                }
                {
                    Content = ": string"
                    Location = (4, 11)
                    ToolTip = "42"
                }
                {
                    Content = "side = "
                    Location = (10, 16)
                    ToolTip = "42"
                }
                {
                    Content = ": Shape"
                    Location = (10, 6)
                    ToolTip = "42"
                }
                {
                    Content = "width = "
                    Location = (11, 20)
                    ToolTip = "42"
                }
                {
                    Content = "height = "
                    Location = (11, 23)
                    ToolTip = "42"
                }
                {
                    Content = ": Shape"
                    Location = (11, 6)
                    ToolTip = "42"
                }
                {
                    Content = ": int "
                    Location = (14, 36)
                    ToolTip = "42"
                }
                {
                    Content = "blahFirst = "
                    Location = (16, 11)
                    ToolTip = "42"
                }
                {
                    Content = ": C"
                    Location = (16, 6)
                    ToolTip = "42"
                }
                {
                    Content = "what = "
                    Location = (17, 19)
                    ToolTip = "42"
                }
                {
                    Content = ": int"
                    Location = (17, 7)
                    ToolTip = "42"
                }
            ]

        let actual = getAllHints document

        actual |> Assert.shouldBeEquivalentTo expected
