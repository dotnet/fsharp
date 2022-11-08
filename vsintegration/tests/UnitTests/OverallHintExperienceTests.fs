// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace VisualFSharp.UnitTests.Editor.Hints

open NUnit.Framework
open HintTestFramework

// just some kind of higher level testing
module OverallHintExperienceTests =

[<Test>]
let ``Current baseline hints`` () =
    let code = """
type Song = { Artist: string; Title: string }
let whoSings song = song.Artist

let artist = whoSings { Artist = "Květy"; Title = "Je podzim" }

type Shape =
    | Square of side: int
    | Rectangle of width: int * height: int

let a = Square 1
let b = Rectangle (1, 2)
"""
    let document = getFsDocument code
    let expected = [
        { Content = ": Song"; Location = (2, 18) }
        { Content = "song = "; Location = (4, 23) }
        { Content = ": string"; Location = (4, 11) }
        { Content = "side = "; Location = (10, 16) }
        { Content = ": Shape"; Location = (10, 6) }
        { Content = "width = "; Location = (11, 20) }
        { Content = "height = "; Location = (11, 23) }
        { Content = ": Shape"; Location = (11, 6) }
    ]

    let actual = getAllHints document

    CollectionAssert.AreEquivalent(expected, actual)
