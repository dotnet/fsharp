// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Record =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"recordPatterns01.fs"|])>]
    let ``Record - recordPatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"recordPatterns02.fs"|])>]
    let ``Record - recordPatterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"structRecordPatterns01.fs"|])>]
    let ``Record - structRecordPatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"structRecordPatterns02.fs"|])>]
    let ``Record - structRecordPatterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    //<Expects status="error" id="FS0010">Unexpected symbol '}' in pattern\. Expected '\.', '=' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"E_SyntaxError01.fs"|])>]
    let ``Record - E_SyntaxError01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '}' in pattern\. Expected '\.', '=' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    //<Expects id="FS0039" status="error">The record label 'X' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Record", Includes=[|"E_RecordFieldNotDefined01.fs"|])>]
    let ``Record - E_RecordFieldNotDefined01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The record label 'X' is not defined"

