// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Xunit
open HintTestFramework

module InlineParameterNameHintTests =

    [<Fact>]
    let ``Hint is shown for a let binding`` () =
        let code =
            """
let greet friend = $"hello {friend}"
let greeting = greet "darkness"
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "friend = "
                    Location = (2, 22)
                    Tooltip = "parameter friend"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for multiple function calls`` () =
        let code =
            """
let greet friend = $"hello {friend}"
let greeting1 = greet "Noel"
let greeting2 = greet "Liam"
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "friend = "
                    Location = (2, 23)
                    Tooltip = "parameter friend"
                }
                {
                    Content = "friend = "
                    Location = (3, 23)
                    Tooltip = "parameter friend"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for multiple parameters`` () =
        let code =
            """
let greet friend1 friend2 = $"hello {friend1} and {friend2}"
let greeting = greet "Liam" "Noel"
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "friend1 = "
                    Location = (2, 22)
                    Tooltip = "parameter friend1"
                }
                {
                    Content = "friend2 = "
                    Location = (2, 29)
                    Tooltip = "parameter friend2"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for tuple items`` () =
        let code =
            """
let greet (friend1, friend2) = $"hello {friend1} and {friend2}"
let greeting = greet ("Liam", "Noel")
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "friend1 = "
                    Location = (2, 23)
                    Tooltip = "parameter friend1"
                }
                {
                    Content = "friend2 = "
                    Location = (2, 31)
                    Tooltip = "parameter friend2"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for active patterns`` () =
        let code =
            """
let (|Even|Odd|) n =
    if n % 2 = 0 then Even
    else Odd
    
let evenOrOdd number =
    match number with
    | Even -> "even"
    | Odd -> "odd"

let even = evenOrOdd 42
let odd = evenOrOdd 41
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "number = "
                    Location = (10, 22)
                    Tooltip = "parameter number"
                }
                {
                    Content = "number = "
                    Location = (11, 21)
                    Tooltip = "parameter number"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>] // here we don't want an empty hint before "x"
    let ``Hints are not shown for nameless parameters`` () =
        let code =
            """
let exists predicate option =
    match option with
    | None -> false
    | Some x -> predicate x
"""

        let document = getFsDocument code

        let result = getParameterNameHints document

        Assert.Empty(result)

    [<Fact>] // here we don't want a useless (?) hint "value = "
    let ``Hints are not shown for parameters of built-in operators`` () =
        let code =
            """
let postTrue = not true
"""

        let document = getFsDocument code

        let result = getParameterNameHints document

        Assert.Empty(result)

    [<Fact>]
    let ``Hints are not shown for parameters of custom operators`` () =
        let code =
            """
let (===) value1 value2 = value1 = value2

let c = "javascript" === "javascript"
"""

        let document = getFsDocument code

        let result = getParameterNameHints document

        Assert.Empty(result)

    [<Fact>]
    let ``Hints are shown for method parameters`` () =
        let code =
            """
let theAnswer = System.Console.WriteLine 42
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "value = "
                    Location = (1, 42)
                    Tooltip = "parameter value"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for parameters of overloaded and curried methods`` () =
        let code =
            """
type C () =
    member _.Normal (alone: string) = 1 
    member _.Normal (what: string, what2: int) = 1 
    member _.Curried (curr1: string, curr2: int) (x: int) = 1

let c = C ()

let a = c.Curried ("hmm", 2) 1
let a = c.Normal ("hmm", 2)
let a = c.Normal "hmm"
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "curr1 = "
                    Location = (8, 20)
                    Tooltip = "parameter curr1"
                }
                {
                    Content = "curr2 = "
                    Location = (8, 27)
                    Tooltip = "parameter curr2"
                }
                {
                    Content = "x = "
                    Location = (8, 30)
                    Tooltip = "parameter x"
                }
                {
                    Content = "what = "
                    Location = (9, 19)
                    Tooltip = "parameter what"
                }
                {
                    Content = "what2 = "
                    Location = (9, 26)
                    Tooltip = "parameter what2"
                }
                {
                    Content = "alone = "
                    Location = (10, 18)
                    Tooltip = "parameter alone"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for constructor parameters`` () =
        let code =
            """
type C (blahFirst: int) =
    new (blah: int, blah2: string) = C blah

let a = C (1, "")
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "blahFirst = "
                    Location = (2, 40)
                    Tooltip = "parameter blahFirst"
                }
                {
                    Content = "blah = "
                    Location = (4, 12)
                    Tooltip = "parameter blah"
                }
                {
                    Content = "blah2 = "
                    Location = (4, 15)
                    Tooltip = "parameter blah2"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown for discriminated union case fields with explicit names`` () =
        let code =
            """
type Shape =
    | Square of side: int
    | Rectangle of width: int * height: int
 
let a = Square 1
let b = Rectangle (1, 2)
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "side = "
                    Location = (5, 16)
                    Tooltip = "field side"
                }
                {
                    Content = "width = "
                    Location = (6, 20)
                    Tooltip = "field width"
                }
                {
                    Content = "height = "
                    Location = (6, 23)
                    Tooltip = "field height"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints for discriminated union case fields are not shown when names are generated`` () =
        let code =
            """
type Shape =
    | Triangle of side1: int * int * side3: int
    | Circle of int
 
let c = Triangle (1, 2, 3)
let d = Circle 1
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "side1 = "
                    Location = (5, 19)
                    Tooltip = "field side1"
                }
                {
                    Content = "side3 = "
                    Location = (5, 25)
                    Tooltip = "field side3"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints for discriminated union case fields are not shown when provided arguments don't match the expected count`` () =
        let code =
            """
type Shape =
    | Triangle of side1: int * side2: int * side3: int
    | Circle of int
 
let c = Triangle (1, 2)
"""

        let document = getFsDocument code

        let actual = getParameterNameHints document

        Assert.Empty(actual)

    [<Fact>]
    let ``Hints for discriminated union case fields are not shown for Cons`` () =
        let code =
            """
type X =
    member _.Test() = 42 :: [42; 42]
"""

        let document = getFsDocument code

        let actual = getParameterNameHints document

        Assert.Empty(actual)

    [<Fact>]
    let ``Hints are not shown in front of indexes`` () =
        let code =
            """
let x = "test".Split("").[0].Split("");
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "separator = "
                    Location = (1, 22)
                    Tooltip = "parameter separator"
                }
                {
                    Content = "separator = "
                    Location = (1, 36)
                    Tooltip = "parameter separator"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are not shown for optional parameters with specified names`` () =
        let code =
            """
type MyType() =

    member _.MyMethod(?beep: int, ?bap: int, ?boop: int) = ()

    member this.Foo = this.MyMethod(3, boop = 4)
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "beep = "
                    Location = (5, 37)
                    Tooltip = "parameter beep"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are not shown when all optional parameters are named`` () =
        let code =
            """
type MyType() =

    member _.MyMethod(?beep: int, ?bap : int, ?boop : int) = ()

    member this.Foo = this.MyMethod(bap = 3, beep = 4)
"""

        let document = getFsDocument code

        let actual = getParameterNameHints document

        Assert.Empty(actual)

    [<Fact>]
    let ``Hints are shown correctly for inner bindings`` () =
        let code =
            """
let test sequences = 
    sequences
    |> Seq.map (fun sequence -> sequence |> Seq.map (fun sequence' -> sequence' |> Seq.map (fun item -> item)))
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "mapping = "
                    Location = (3, 16)
                    Tooltip = "parameter mapping"
                }
                {
                    Content = "mapping = "
                    Location = (3, 53)
                    Tooltip = "parameter mapping"
                }
                {
                    Content = "mapping = "
                    Location = (3, 92)
                    Tooltip = "parameter mapping"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are shown correctly for custom operations`` () =
        let code =
            """
let q = query { for x in { 1 .. 10 } do select x }
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "projection = "
                    Location = (1, 48)
                    Tooltip = "parameter projection"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints are not shown for custom operations with only 1 parameter`` () =
        let code =
            """
let q = query { for _ in { 1 .. 10 } do count }
"""

        let document = getFsDocument code

        let actual = getParameterNameHints document

        Assert.Empty(actual)

    [<Fact>]
    let ``Hints are not shown when parameter names coincide with variable names`` () =
        let code =
            """
let getFullName name surname = $"{name} {surname}"

let name = "Robert"
let lastName = "Smith"
let fullName = getFullName name lastName
"""

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "surname = "
                    Location = (5, 33)
                    Tooltip = "parameter surname"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Hints don't break with multi-line arguments`` () =
        let code =
            """
None
|> Option.map (fun x ->
    x + 5
    )
|> ignore
        """

        let document = getFsDocument code

        let expected =
            [
                {
                    Content = "mapping = "
                    Location = (2, 15)
                    Tooltip = "parameter mapping"
                }
            ]

        let actual = getParameterNameHints document

        Assert.Equal(expected, actual)
