// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Operator Errors`` =

    [<Fact>]
    let ``SRTP operator scope hint is shown when function type given``() =
        FSharp """
let inline add (x: ^a) (y: ^a) = x + y
let result = add id id
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 3, Col 18, Line 3, Col 20,
                                 "Expecting a type supporting the operator '+' but given a function type. You may be missing an argument to a function, or the operator may not be in scope. Check that you have opened the correct module or namespace.")

    [<Fact>]
    let ``SRTP operator scope hint is shown when tuple type given``() =
        FSharp """
let inline add (x: ^a) (y: ^a) = x + y
let result = add (1, 2) (3, 4)
        """
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Operator '\+' cannot be applied to a tuple type. You may have an unintended extra comma creating a tuple, or the operator may not be in scope."
