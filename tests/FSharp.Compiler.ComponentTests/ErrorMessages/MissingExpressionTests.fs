// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler


module ``Missing Expression`` =

    [<Fact>]
    let ``Missing Expression after let``() =
        FSharp """
let sum = 0
for x in 0 .. 10 do
    let sum = sum + x
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 588, Line 4, Col 5, Line 4, Col 8,
                                 "The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.")
