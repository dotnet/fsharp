// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module FunctionApplicationResolution =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/FunctionApplicationResolution)
    //<Expects id="FS0001" status="error" span="(8,32-8,37)">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/FunctionApplicationResolution", Includes=[|"E_FOFunction01.fs"|])>]
    let ``FunctionApplicationResolution - E_FOFunction01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/FunctionApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/FunctionApplicationResolution", Includes=[|"InferGenericArgAsTuple01.fs"|])>]
    let ``FunctionApplicationResolution - InferGenericArgAsTuple01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/FunctionApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/FunctionApplicationResolution", Includes=[|"InferGenericArgAsTuple02.fs"|])>]
    let ``FunctionApplicationResolution - InferGenericArgAsTuple02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

