// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.Hints.InlineReturnTypeHintTests

open Xunit
open HintTestFramework
open FSharp.Test

[<Fact>]
let ``Hints are shown for let-bound function return types`` () =
    let code =
        """
let func () = 3
let func2 x = x + 1
let setConsoleOut = System.Console.SetOut
"""

    let document = getFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (1, 13)
                ToolTip = "42"
            }
            {
                Content = ": int "
                Location = (2, 13)
                ToolTip = "42"
            }
            {
                Content = ": unit "
                Location = (3, 19)
                ToolTip = "42"
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

    let document = getFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (2, 24)
                ToolTip = "42"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are shown for generic functions`` () =
    let code = "let func _a = 5"

    let document = getFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (0, 13)
                ToolTip = "42"
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

    let document = getFsDocument code

    let result = getReturnTypeHints document

    let expected =
        [
            {
                Content = ": int "
                Location = (2, 21)
                ToolTip = "42"
            }
        ]

    Assert.Equal(expected, result)

[<Fact>]
let ``Hints are not shown for lambda bindings`` () =
    let code = "let func = fun () -> 3"

    let document = getFsDocument code

    let result = getReturnTypeHints document

    Assert.Empty result

[<Fact>]
let ``Hints are not shown when there's type annotation`` () =
    let code = "let func x : int = x"

    let document = getFsDocument code

    let result = getReturnTypeHints document

    Assert.Empty result
