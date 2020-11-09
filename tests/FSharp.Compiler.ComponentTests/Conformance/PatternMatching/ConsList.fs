// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ConsList =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/ConsList", Includes=[|"consPattern01.fs"|])>]
    let ``ConsList - consPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    //<Expects id="FS0001" status="error" span="(15,22-15,24)">This expression was expected to have type.    'int'    .but here has type.    ''a list'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/ConsList", Includes=[|"E_consPattern01.fs"|])>]
    let ``ConsList - E_consPattern01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    ''a list'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/ConsList)
    //<Expects span="(4,21-4,28)" status="error" id="FS0001">This expression was expected to have type.    'int'    .but here has type.    ''a list'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/ConsList", Includes=[|"E_consOnNonList.fs"|])>]
    let ``ConsList - E_consOnNonList.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    ''a list'"

