// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LogicalPropertiesOfTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes=[|"TypeWithNullLiteral_NetRef.fsx"|])>]
    let ``LogicalPropertiesOfTypes - TypeWithNullLiteral_NetRef.fsx - -a`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed
        |> ignore

