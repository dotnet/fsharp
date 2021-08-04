// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module ``Anonymous records tests`` =

    [<Fact>]
    let ``Simple with- should remove one field`` () =
        Fsx """
type A = { X: int; Y: int }
let a = { X = 1; Y = 2 }
let b = {| a with- Y |}
        """
         |> compile
         |> shouldSucceed
    
         [<Fact>]
    let ``with- shouldn't remove the only field`` () =
        Fsx """
type A = { X: int }
let a = { X = 1 }
let b = {| a with- X |}
        """
         |> compile
         |> shouldSucceed
