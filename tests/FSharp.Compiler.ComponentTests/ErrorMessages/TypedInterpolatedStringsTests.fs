// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Typed interpolated strings`` =

    [<Fact>]
    let ``Untyped interpolated strings produce warning``() =
        FSharp """
let hello = "Hello World"
printf $"{hello}"
        """
        |> withWarnOn 3579
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3579, Line 3, Col 8, Line 3, Col 18, "Interpolated string contains untyped identifiers. Adding typed format specifiers is recommended.")

    [<Fact>]
    let ``Do not warn on untyped interpolated strings by default``() =
        FSharp """
let hello = "Hello World"
printf $"{hello}"
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Typed interpolated strings over interpolated one``() =
        FSharp """
let hello = "Hello World"
printf $"%s{hello}"
        """
        |> withWarnOn 3579
        |> typecheck
        |> shouldSucceed
