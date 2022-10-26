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
"""
    let document = getFsDocument code
    let expected = [
        { Content = ": Song"; Location = (2, 18) }
        { Content = ": string"; Location = (4, 11) }
        { Content = "song = "; Location = (4, 23) }
    ]

    let actual = getAllHints document

    CollectionAssert.AreEquivalent(expected, actual)
