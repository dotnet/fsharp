// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DelegateTypes

open Xunit
open FSharp.Test.Compiler

module InvalidDelegateDefinition =

    [<Fact>]
    let ``Illegal definition for runtime implemented delegate method.`` () =
        FSharp """
namespace FSharpTest
    type T(x: int) =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> withErrorCode 193
        |> withErrorMessage "Illegal definition for runtime implemented delegate method."
