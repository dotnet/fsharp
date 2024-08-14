// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.Hints.InlineReturnTypeHintTests

open Xunit

open FSharp.Test
open FSharp.Editor.Tests.Helpers

open HintTestFramework

[<Fact>]
let ``Hints are shown for let-bound function return types`` () =
    let code =
        """
let func () = 3
let func2 x = x + 1
let setConsoleOut = System.Console.SetOut
"""

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (1, 13)
                Tooltip = "type int"
            }
            {
                Content = ": int "
                Location = (2, 13)
                Tooltip = "type int"
            }
            {
                Content = ": unit "
                Location = (3, 19)
                Tooltip = "type unit"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are correct for user types`` () =
    let code =
        """
type Answer = { Text: string }

let getAnswer() = { Text = "42" }
"""

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": Answer "
                Location = (3, 17)
                Tooltip = "type Answer"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are shown for method return types`` () =
    let code =
        """
type Test() =
    member this.Func() = 3
"""

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (2, 24)
                Tooltip = "type int"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are shown for generic functions`` () =
    let code = "let func _a = 5"

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (0, 13)
                Tooltip = "type int"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are shown for functions within expressions`` () =
    let code =
        """
    let _ =
        let func () = 2
"""

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (2, 21)
                Tooltip = "type int"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are not shown for lambda bindings`` () =
    let code = "let func = fun () -> 3"

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    Assert.Empty result

[<Theory>]
[<InlineData("let func x : int = x")>]
[<InlineData("let func (a: 'a) : 'a = a")>]
[<InlineData("let func (a: 'a) : List<'a> = [a]")>]
let ``Hints are not shown when there's type annotation`` code =

    let document = RoslynTestHelpers.GetFsDocument code

    let result = getReturnTypeHints document

    Assert.Empty result
