// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Tuple =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Tuple", Includes=[|"tuples01.fs"|])>]
    let ``Tuple - tuples01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Tuple", Includes=[|"SimpleTuples01.fs"|])>]
    let ``Tuple - SimpleTuples01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    //<Expects id="FS0025" span="(8,5)" status="warning">Incomplete pattern matches on this expression.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Tuple", Includes=[|"W_IncompleteMatches01.fs"|])>]
    let ``Tuple - W_IncompleteMatches01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    //<Expects id="FS0026" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Tuple", Includes=[|"W_RedundantPattern01.fs"|])>]
    let ``Tuple - W_RedundantPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    //<Expects id="FS0026" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Tuple", Includes=[|"W_RedundantPattern02.fs"|])>]
    let ``Tuple - W_RedundantPattern02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"

