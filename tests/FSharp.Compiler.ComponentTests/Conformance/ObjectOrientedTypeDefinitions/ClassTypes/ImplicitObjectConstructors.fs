// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ImplicitObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors)
    //<Expects id="FS0044" span="(9,9-9,15)" status="warning">Message1</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/ImplicitObjectConstructors", Includes=[|"WithAttribute.fs"|])>]
    let ``ImplicitObjectConstructors - WithAttribute.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message1"
        |> ignore

