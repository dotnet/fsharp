// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace VisualFSharp.UnitTests.Editor.Hints

open NUnit.Framework
open HintTestFramework

module HintServiceTests =

[<Test>]
let ``Type hints are not shown in signature files`` () =
    let fsiCode = """
module Test

val numbers: int[]
"""
    let fsCode = """
module Test

let numbers = [|42|]
"""
    let fsiDocument, _ = getFsiAndFsDocuments fsiCode fsCode

    let result = getHints fsiDocument
    
    Assert.IsEmpty(result)

[<Test>]
let ``Type hint is shown for a let binding`` () =
    let code = """
type Song = { Artist: string; Title: string }

let s = { Artist = "Moby"; Title = "Porcelain" }
"""
    let document = getFsDocument code
    let expected = [{ Content = ": Song"; Location = (3, 6) }]

    let actual = getHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Type hint is not shown for a let binding when the type is manually specified`` () =
    let code = """
type Song = { Artist: string; Title: string }

let s: Song = { Artist = "Moby"; Title = "Porcelain" }
"""
    let document = getFsDocument code
    
    let result = getHints document
    
    Assert.IsEmpty(result)

[<Test>]
let ``Type hint is shown for a parameter`` () =
    let code = """
type Song = { Artist: string; Title: string }

let whoSings s = s.Artist
"""
    let document = getFsDocument code
    let expected = [{ Content = ": Song"; Location = (3, 15) }]
    
    let actual = getHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Type hint is not shown for a parameter when the type is manually specified`` () =
    let code = """
type Song = { Artist: string; Title: string }

let whoSings (s: Song) = s.Artist
"""
    let document = getFsDocument code
    
    let result = getHints document
    
    Assert.IsEmpty(result)

[<Test>] // here we don't want a hint after "this"
let ``Type hint is not shown for type self-identifiers`` () =
    let code = """
type Song() =
    member this.GetName() = "Porcelain"
"""
    let document = getFsDocument code
    
    let result = getHints document

    Assert.IsEmpty(result)

[<Test>] // here we don't want a hint after "x"
let ``Type hint is not shown for type aliases`` () =
    let code = """
type Song() as x =
    member this.Name = "Porcelain"
"""
    let document = getFsDocument code

    let result = getHints document

    Assert.IsEmpty(result)
