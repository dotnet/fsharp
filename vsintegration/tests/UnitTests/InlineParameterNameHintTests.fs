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
let ``Hint are shown for multiple parameters`` () =
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

[<Test>] // here we don't want an empty hint before "x"
let ``Hint are not shown for nameless parameters`` () =
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
let ``Hint are not shown for operator parameters`` () =
    let code = """
let postTrue = not true
"""
    let document = getFsDocument code

    let result = getParameterNameHints document

    Assert.IsEmpty(result)