// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Errors assigning to mutable objects`` =

    [<Fact>]
    let ``Assign to immutable error``() =
        FSharp """
let x = 10
x <- 20
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 27, Line 3, Col 1, Line 3, Col 8,
                                 "This value is not mutable. Consider using the mutable keyword, e.g. 'let mutable x = expression'.")
