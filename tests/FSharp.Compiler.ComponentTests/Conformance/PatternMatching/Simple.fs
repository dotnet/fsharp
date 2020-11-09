// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Simple =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"E_constPattern01.fs"|])>]
    let ``Simple - E_constPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects status="warning" id="FS0104">Enums may take values outside known cases.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"E_namedLiberal01.fs"|])>]
    let ``Simple - E_namedLiberal01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0104
        |> withDiagnosticMessageMatches "Enums may take values outside known cases."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0025" span="(92,13-92,14)" status="warning">Incomplete pattern matches on this expression. For example, the value 'Result \(_\)' may indicate a case not covered by the pattern\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_Incomplete01.fs"|])>]
    let ``Simple - W_Incomplete01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression. For example, the value 'Result \(_\)' may indicate a case not covered by the pattern\(s\)"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0026" span="(32,11-32,13)" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_Incomplete02.fs"|])>]
    let ``Simple - W_Incomplete02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0010" status="error" span="(8,14)">Unexpected symbol'\[' in pattern matching. Expected '->' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"E_SyntaxError01.fs"|])>]
    let ``Simple - E_SyntaxError01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "' or other token"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0049" span="(10,16-10,19)" status="warning">Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_BindCaptialIdent.fs"|])>]
    let ``Simple - W_BindCaptialIdent.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0049
        |> withDiagnosticMessageMatches "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"CodeGenReg01.fs"|])>]
    let ``Simple - CodeGenReg01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"MatchFailureExn01.fs"|])>]
    let ``Simple - MatchFailureExn01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"ValueCapture01.fs"|])>]
    let ``Simple - ValueCapture01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"ValueCapture02.fs"|])>]
    let ``Simple - ValueCapture02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0039" status="error">The value or constructor 'ident2' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"E_ValueCapture01.fs"|])>]
    let ``Simple - E_ValueCapture01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'ident2' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns01.fs"|])>]
    let ``Simple - simplePatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns02.fs"|])>]
    let ``Simple - simplePatterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns03.fs"|])>]
    let ``Simple - simplePatterns03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns04.fs"|])>]
    let ``Simple - simplePatterns04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns05.fs"|])>]
    let ``Simple - simplePatterns05.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns06.fs"|])>]
    let ``Simple - simplePatterns06.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns07.fs"|])>]
    let ``Simple - simplePatterns07.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns08.fs"|])>]
    let ``Simple - simplePatterns08.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns09.fs"|])>]
    let ``Simple - simplePatterns09.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns10.fs"|])>]
    let ``Simple - simplePatterns10.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns11.fs"|])>]
    let ``Simple - simplePatterns11.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns12.fs"|])>]
    let ``Simple - simplePatterns12.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns13.fs"|])>]
    let ``Simple - simplePatterns13.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns14.fs"|])>]
    let ``Simple - simplePatterns14.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns15.fs"|])>]
    let ``Simple - simplePatterns15.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns16.fs"|])>]
    let ``Simple - simplePatterns16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns17.fs"|])>]
    let ``Simple - simplePatterns17.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns18.fs"|])>]
    let ``Simple - simplePatterns18.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns19.fs"|])>]
    let ``Simple - simplePatterns19.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"simplePatterns20.fs"|])>]
    let ``Simple - simplePatterns20.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

