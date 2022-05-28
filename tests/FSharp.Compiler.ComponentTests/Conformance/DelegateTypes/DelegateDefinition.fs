// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DelegateTypes

open Xunit
open FSharp.Test.Compiler

module DelegateDefinition =

    [<Fact>]
    let ``Delegate definition with primary constructor and argument.`` () =
        FSharp
            """
namespace FSharpTest
    type T(x: int) =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> withErrorCode 552
        |> withErrorMessage "Only class types may take value arguments"
        
    [<Fact>]
    let ``Delegate definition with primary constructor no argument.`` () =
        FSharp
            """
namespace FSharpTest
    type T() =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> withErrorCode 552
        |> withErrorMessage "Only class types may take value arguments"

    [<Fact>]
    let ``Delegate definition`` () =
        FSharp
            """
namespace FSharpTest
    type T =
        delegate of int -> int
        """
        |> compile
        |> shouldSucceed
