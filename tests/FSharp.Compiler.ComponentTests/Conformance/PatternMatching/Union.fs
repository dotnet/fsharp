// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Union =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"unionPattern01.fs"|])>]
    let ``Union - unionPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"unionPattern02.fs"|])>]
    let ``Union - unionPattern02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"unionPattern03.fs"|])>]
    let ``Union - unionPattern03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"unionPattern04.fs"|])>]
    let ``Union - unionPattern04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    //<Expects id="FS0018" status="error">The two sides of this 'or' pattern bind different sets of variables</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"E_NotAllCaptureSameVal01.fs"|])>]
    let ``Union - E_NotAllCaptureSameVal01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0018
        |> withDiagnosticMessageMatches "The two sides of this 'or' pattern bind different sets of variables"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    //<Expects id="FS0018" status="error">The two sides of this 'or' pattern bind different sets of variables</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Union", Includes=[|"E_CapturesDiffVal01.fs"|])>]
    let ``Union - E_CapturesDiffVal01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0018
        |> withDiagnosticMessageMatches "The two sides of this 'or' pattern bind different sets of variables"

