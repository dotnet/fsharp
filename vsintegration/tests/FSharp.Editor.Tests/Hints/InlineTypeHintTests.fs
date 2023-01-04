// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open NUnit.Framework
open HintTestFramework

module InlineTypeHintTests =

    [<Test>]
    let ``Hint is shown for a let binding`` () =
        let code =
            """
type Song = { Artist: string; Title: string }

let s = { Artist = "Moby"; Title = "Porcelain" }
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = ": Song"
                    Location = (3, 6)
                }
            ]

        let actual = getTypeHints document

        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Hint is shown for a parameter`` () =
        let code =
            """
type Song = { Artist: string; Title: string }

let whoSings s = s.Artist
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = ": Song"
                    Location = (3, 15)
                }
            ]

        let actual = getTypeHints document

        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Hints are not shown in signature files`` () =
        let fsiCode =
            """
module Test

val numbers: int[]
"""

        let fsCode =
            """
module Test

let numbers = [|42|]
"""

        let fsiDocument, _ = getFsiAndFsDocuments fsiCode fsCode

        let result = getTypeHints fsiDocument

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hints are not shown for let-bound functions yet`` () =
        let code =
            """
let setConsoleOut = System.Console.SetOut
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hint is not shown for a let binding when the type is manually specified`` () =
        let code =
            """
type Song = { Artist: string; Title: string }

let s: Song = { Artist = "Moby"; Title = "Porcelain" }
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hint is not shown for a parameter when the type is manually specified`` () =
        let code =
            """
type Song = { Artist: string; Title: string }

let whoSings (s: Song) = s.Artist
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>] // here we don't want a hint after "this"
    let ``Hint is not shown for type self-identifiers`` () =
        let code =
            """
type Song() =
    member this.GetName() = "Porcelain"
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>] // here we don't want a hint after "x"
    let ``Hint is not shown for type aliases`` () =
        let code =
            """
type Song() as x =
    member this.Name = "Porcelain"
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hints are shown for lambdas`` () =
        let code =
            """
let iamboring() =
    fun x -> x
"""

        let document = getFsDocument code
        let expected = [ { Content = ": 'a"; Location = (2, 10) } ]

        let actual = getTypeHints document

        Assert.AreEqual(expected, actual)

    [<Test>]
    let ``Hints are shown for lambdas with tuples`` () =
        let code =
            """
let zip4 (l1: 'a list) (l2: 'b list) (l3: 'c list) (l4: 'd list) =
    List.zip l1 (List.zip3 l2 l3 l4)
    |> List.map (fun (x1, (x2, x3, x4)) -> (x1, x2, x3, x4))
"""

        let document = getFsDocument code

        let expected =
            [
                { Content = ": 'a"; Location = (3, 25) }
                { Content = ": 'b"; Location = (3, 30) }
                { Content = ": 'c"; Location = (3, 34) }
                { Content = ": 'd"; Location = (3, 38) }
            ]

        let actual = getTypeHints document

        CollectionAssert.AreEquivalent(expected, actual)

    [<Test>]
    let ``Hints are not shown for unfinished expressions`` () =
        let code =
            """
let x
"""
        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hints are not shown for unsolved types in _for_ expressions in collections`` () = 
        let code =
            """
let _ = [ for x ]
"""
        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hints are not shown for unsolved types in _for_ expressions within computational expressions`` () = 
        let code =
            """
do task {
    for x

    do! Task.Delay 0
    }
"""
        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)

    [<Test>]
    let ``Hints are shown for IWSAM`` () =
        let code =
            """
type IAddition<'T when 'T :> IAddition<'T>> =
    static abstract op_Addition: 'T * 'T -> 'T

type Number<'T when IAddition<'T>>(value: 'T) =
    member _.Value with get() = value
    interface IAddition<Number<'T>> with
        static member op_Addition(a, b) = Number(a.Value + b.Value)
"""
        let document = getFsDocument code

        let expected =
            [
                { Content = ": Number<'T>"; Location = (7, 36) }
                { Content = ": Number<'T>"; Location = (7, 39) }
            ]

        let actual = getTypeHints document

        CollectionAssert.AreEquivalent(expected, actual)


    [<Test>]
    let ``Hints are not shown when type is specified`` () =
        let code =
            """
type MyType() =

    member _.MyMethod(?beep: int, ?bap: int, ?boop: int) = ()

    member this.Foo = this.MyMethod(bap = 3, boop = 4)
"""

        let document = getFsDocument code

        let result = getTypeHints document

        Assert.IsEmpty(result)