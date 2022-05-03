// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DelegateTypes

open Xunit
open FSharp.Test.Compiler

module InvalidDelegateDefinition =

    [<Fact>]
    let ``Illegal definition for runtime implemented delegate method.`` () =
        FSharp """
module FSharpTest =
    type InvalidDelegateDefinition(x: int) =
        delegate of int -> int
    let invalidDelegate = InvalidDelegateDefinition(fun _ -> 1)
        """
        |> compile
        |> shouldFail
        |> withErrorCode 193
        |> withErrorMessage "Illegal definition for runtime implemented delegate method."
        
    [<Fact>]
    let ``Illegal definition for runtime implemented delegate method when using in a fsx.`` () =
        Fsx """
    type InvalidDelegateDefinition(x: int) =
        delegate of int -> int
    let invalidDelegate = InvalidDelegateDefinition(fun _ -> 1)
        """
        |> compile
        |> shouldFail
        |> withErrorCode 193
        |> withErrorMessage "Illegal definition for runtime implemented delegate method."
