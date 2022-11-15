// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace VisualFSharp.UnitTests.Editor.Hints

open NUnit.Framework
open HintTestFramework

module InlineTypeHintTests =

[<Test>]
let ``Hint is shown for a let binding`` () =
    let code = """
type Song = { Artist: string; Title: string }

let s = { Artist = "Moby"; Title = "Porcelain" }
"""
    let document = getFsDocument code
    let expected = [{ Content = ": Song"; Location = (3, 6) }]

    let actual = getTypeHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hint is shown for a parameter`` () =
    let code = """
type Song = { Artist: string; Title: string }

let whoSings s = s.Artist
"""
    let document = getFsDocument code
    let expected = [{ Content = ": Song"; Location = (3, 15) }]
    
    let actual = getTypeHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints are not shown in signature files`` () =
    let fsiCode = """
module Test

val numbers: int[]
"""
    let fsCode = """
module Test

let numbers = [|42|]
"""
    let fsiDocument, _ = getFsiAndFsDocuments fsiCode fsCode

    let result = getTypeHints fsiDocument
    
    Assert.IsEmpty(result)

[<Test>]
let ``Hints are not shown for let-bound functions yet`` () =
    let code = """
let setConsoleOut = System.Console.SetOut
"""
    let document = getFsDocument code

    let result = getTypeHints document

    Assert.IsEmpty(result)

[<Test>]
let ``Hint is not shown for a let binding when the type is manually specified`` () =
    let code = """
type Song = { Artist: string; Title: string }

let s: Song = { Artist = "Moby"; Title = "Porcelain" }
"""
    let document = getFsDocument code
    
    let result = getTypeHints document
    
    Assert.IsEmpty(result)

[<Test>]
let ``Hint is not shown for a parameter when the type is manually specified`` () =
    let code = """
type Song = { Artist: string; Title: string }

let whoSings (s: Song) = s.Artist
"""
    let document = getFsDocument code
    
    let result = getTypeHints document
    
    Assert.IsEmpty(result)

[<Test>] // here we don't want a hint after "this"
let ``Hint is not shown for type self-identifiers`` () =
    let code = """
type Song() =
    member this.GetName() = "Porcelain"
"""
    let document = getFsDocument code
    
    let result = getTypeHints document

    Assert.IsEmpty(result)

[<Test>] // here we don't want a hint after "x"
let ``Hint is not shown for type aliases`` () =
    let code = """
type Song() as x =
    member this.Name = "Porcelain"
"""
    let document = getFsDocument code

    let result = getTypeHints document

    Assert.IsEmpty(result)
