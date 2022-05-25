// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DelegateTypes

open Xunit
open FSharp.Test.Compiler

module InvalidDelegateDefinition =

//    [<Fact>]
//    let ``FSX Delegate primary constructor with argument.`` () =
//        Fsx
//            """
//namespace FSharpTest
//    type T(x: int) =
//        delegate of int -> int
//        """
//        |> asExe
//        |> compileExeAndRun
//        |> shouldFail
//        |> withErrorCode 3524
//        |> withErrorMessage "No argument are allowed in delegate primary constructor."
//
//    [<Fact>]
//    let ``FSX Delegate primary constructor with no argument.`` () =
//        Fsx
//            """
//namespace FSharpTest
//    type T() =
//        delegate of int -> int
//        """
//        |> asExe
//        |> compileExeAndRun
//        |> shouldSucceed
//
    [<Fact>]
    let ``FSharp Delegate primary constructor with argument.`` () =
        FSharp
            """
namespace FSharpTest
    type T(x: int) =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> withErrorCode 3524
        |> withErrorMessage "No argument are allowed in delegate primary constructor."

    [<Fact>]
    let ``FSharp Delegate primary constructor with no argument`` () =
        FSharp
            """
namespace FSharpTest
    type T() =
        delegate of int -> int
        """
        |> compile
        |> shouldSucceed
