// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.Hints.ReturnTypeHintTests

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
            }
            {
                Content = ": int "
                Location = (2, 13)
            }
            {
                Content = ": unit "
                Location = (3, 19)
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
