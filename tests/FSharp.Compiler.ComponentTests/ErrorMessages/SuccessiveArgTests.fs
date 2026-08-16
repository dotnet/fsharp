// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Successive arguments`` =

    [<Fact>]
    let ``Method call as argument needs parentheses``() =
        FSharp """
let f x y = x + y
let g() = 1
let r = f g() 2
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 597

    [<Fact>]
    let ``Constructor call as argument needs parentheses``() =
        FSharp """
let f (x: obj) (y: int) = x
let r = f System.Object() 2
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 597
