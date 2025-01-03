// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LogicalPropertiesOfTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes)
    [<Theory; FileInlineData("TypeWithNullLiteral_NetRef.fsx")>]
    let ``TypeWithNullLiteral_NetRef_fsx`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed
        |> ignore

