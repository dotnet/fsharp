// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SimpleConstant =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"matchConst01.fs"|])>]
    let ``SimpleConstant - matchConst01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"matchConst02.fs"|])>]
    let ``SimpleConstant - matchConst02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"matchConst03.fs"|])>]
    let ``SimpleConstant - matchConst03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"matchConst04.fs"|])>]
    let ``SimpleConstant - matchConst04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    //<Expects id="FS0010" status="error">Unexpected symbol '..' in pattern matching. Expected '->' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"E_NoRangeConst01.fs"|])>]
    let ``SimpleConstant - E_NoRangeConst01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "' or other token"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"DiffAssembly.fs"|])>]
    let ``SimpleConstant - DiffAssembly.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"MatchNaN.fs"|])>]
    let ``SimpleConstant - MatchNaN.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    //<Expects id="FS0026" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"MatchLiteral01.fs"|])>]
    let ``SimpleConstant - MatchLiteral01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"FullyQualify01.fs"|])>]
    let ``SimpleConstant - FullyQualify01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_bigint.fs"|])>]
    let ``SimpleConstant - type_bigint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    //<Expects id="FS0720" span="(10,7-10,13)" status="error">Non-primitive numeric literal constants.+</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"E_type_bignum40.fs"|])>]
    let ``SimpleConstant - E_type_bignum40.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0720
        |> withDiagnosticMessageMatches "Non-primitive numeric literal constants.+"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_bool.fs"|])>]
    let ``SimpleConstant - type_bool.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_byte.fs"|])>]
    let ``SimpleConstant - type_byte.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_byteArr.fs"|])>]
    let ``SimpleConstant - type_byteArr.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_char.fs"|])>]
    let ``SimpleConstant - type_char.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_double.fs"|])>]
    let ``SimpleConstant - type_double.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_float32.fs"|])>]
    let ``SimpleConstant - type_float32.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_int.fs"|])>]
    let ``SimpleConstant - type_int.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_int16.fs"|])>]
    let ``SimpleConstant - type_int16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_int64.fs"|])>]
    let ``SimpleConstant - type_int64.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_nativenint.fs"|])>]
    let ``SimpleConstant - type_nativenint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_sbyte.fs"|])>]
    let ``SimpleConstant - type_sbyte.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_string.fs"|])>]
    let ``SimpleConstant - type_string.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_uint16.fs"|])>]
    let ``SimpleConstant - type_uint16.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_uint32.fs"|])>]
    let ``SimpleConstant - type_uint32.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_uint64.fs"|])>]
    let ``SimpleConstant - type_uint64.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_unativenint.fs"|])>]
    let ``SimpleConstant - type_unativenint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/SimpleConstant)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/SimpleConstant", Includes=[|"type_unit.fs"|])>]
    let ``SimpleConstant - type_unit.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

