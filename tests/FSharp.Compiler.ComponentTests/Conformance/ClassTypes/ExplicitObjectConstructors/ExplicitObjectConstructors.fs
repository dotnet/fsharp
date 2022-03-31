// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ExplicitObjectConstructors =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"new_while_01.fs"|])>]
    let ``ExplicitObjectConstructors - new_while_01_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    //<Expects id="FS0044" span="(14,9-14,15)" status="warning">Message2</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"WithAttribute01.fs"|])>]
    let ``ExplicitObjectConstructors - WithAttribute01_fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message2"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/ClassTypes/ExplicitObjectConstructors)
    //<Expects id="FS0044" span="(11,9-11,14)" status="warning">Message1</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"WithAttribute02.fs"|])>]
    let ``ExplicitObjectConstructors - WithAttribute02_fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0044
        |> withDiagnosticMessageMatches "Message1"
        |> ignore

