// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module MethodApplicationResolution =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/MethodApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/MethodApplicationResolution", Includes=[|"UnitVsNoArgs.fs"|])>]
    let ``MethodApplicationResolution - UnitVsNoArgs.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/MethodApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/MethodApplicationResolution", Includes=[|"UnitVsNoArgs02.fs"|])>]
    let ``MethodApplicationResolution - UnitVsNoArgs02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/MethodApplicationResolution)
    //<Expects id="FS0504" span="(13,9-13,24)" status="error">Incorrect generic instantiation\. No accessible member named 'M' takes 2 generic arguments\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/MethodApplicationResolution", Includes=[|"E_OverloadedGenericArgs.fs"|])>]
    let ``MethodApplicationResolution - E_OverloadedGenericArgs.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0504
        |> withDiagnosticMessageMatches "Incorrect generic instantiation\. No accessible member named 'M' takes 2 generic arguments\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/MethodApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/MethodApplicationResolution", Includes=[|"MultiExtensionMethods01.fs"|])>]
    let ``MethodApplicationResolution - MultiExtensionMethods01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/MethodApplicationResolution)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/MethodApplicationResolution", Includes=[|"ParamArrayToDelegate01.fs"|])>]
    let ``MethodApplicationResolution - ParamArrayToDelegate01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

