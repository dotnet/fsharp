// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace VisualFSharp.UnitTests.Editor.Hints

open NUnit.Framework
open HintTestFramework

module InlineParameterNameHintTests =

[<Test>]
let ``Hint is shown for a let binding`` () =
    let code = """
let greet friend = $"hello {friend}"
let greeting = greet "darkness"
"""
    let document = getFsDocument code
    let expected = [{ Content = "friend = "; Location = (2, 22) }]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints are shown for multiple function calls`` () =
    let code = """
let greet friend = $"hello {friend}"
let greeting1 = greet "Noel"
let greeting2 = greet "Liam"
"""
    let document = getFsDocument code
    let expected = [
        { Content = "friend = "; Location = (2, 23) }
        { Content = "friend = "; Location = (3, 23) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints are shown for multiple parameters`` () =
    let code = """
let greet friend1 friend2 = $"hello {friend1} and {friend2}"
let greeting = greet "Liam" "Noel"
"""
    let document = getFsDocument code
    let expected = [
        { Content = "friend1 = "; Location = (2, 22) }
        { Content = "friend2 = "; Location = (2, 29) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints are shown for tuple items`` () =
    let code = """
let greet (friend1, friend2) = $"hello {friend1} and {friend2}"
let greeting = greet ("Liam", "Noel")
"""
    let document = getFsDocument code
    let expected = [
        { Content = "friend1 = "; Location = (2, 23) }
        { Content = "friend2 = "; Location = (2, 31) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints are shown for active patterns`` () =
    let code = """
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
    let expected = [
        { Content = "number = "; Location = (10, 22) }
        { Content = "number = "; Location = (11, 21) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)


[<Test>] // here we don't want an empty hint before "x"
let ``Hints are not shown for nameless parameters`` () =
    let code = """
let exists predicate option =
    match option with
    | None -> false
    | Some x -> predicate x
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)

[<Test>] // here we don't want a useless (?) hint "value = "
let ``Hints are not shown for parameters of built-in operators`` () =
    let code = """
let postTrue = not true
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)

[<Test>]
let ``Hints are not shown for parameters of custom operators`` () =
    let code = """
let (===) value1 value2 = value1 = value2

let c = "javascript" === "javascript"
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)

[<Test>]
let ``Hints are not (yet) shown for method parameters`` () =
    let code = """
let theAnswer = System.Console.WriteLine 42
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)

[<Test>]
let ``Hints are not (yet) shown for constructor parameters`` () =
    let code = """
type WrappedThing(x) =
    let unwrapped = x

let wrapped = WrappedThing 42
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)

[<Test>]
let ``Hints are shown for discriminated union case fields with explicit names`` () =
    let code = """
type Shape =
    | Square of side: int
    | Rectangle of width: int * height: int
 
let a = Square 1
let b = Rectangle (1, 2)
"""
    let document = getFsDocument code

    let expected = [
        { Content = "side = "; Location = (5, 16) }
        { Content = "width = "; Location = (6, 20) }
        { Content = "height = "; Location = (6, 23) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints for discriminated union case fields are not shown when names are generated`` () =
    let code = """
type Shape =
    | Triangle of side1: int * int * side3: int
    | Circle of int
 
let c = Triangle (1, 2, 3)
let d = Circle 1
"""
    let document = getFsDocument code

    let expected = [
        { Content = "side1 = "; Location = (5, 19) }
        { Content = "side3 = "; Location = (5, 25) }
    ]

    let actual = getParameterNameHints document

    Assert.AreEqual(expected, actual)

[<Test>]
let ``Hints for discriminated union case fields are not shown when provided arguments don't match the expected count`` () =
    let code = """
type Shape =
    | Triangle of side1: int * side2: int * side3: int
    | Circle of int
 
let c = Triangle (1, 2)
"""
    let document = getFsDocument code

    let actual = getParameterNameHints document

    Assert.IsEmpty(actual)

[<Test>]
let ``Hints for discriminated union case fields are not shown for Cons`` () =
    let code = """
type X =
    member _.Test() = 42 :: [42; 42]
"""
    let document = getFsDocument code

    let actual = getParameterNameHints document

    Assert.IsEmpty(actual)

