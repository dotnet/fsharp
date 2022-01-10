// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NumericLiterals =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - enumNegativeLiterals001.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals001.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals002.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals002.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals003.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals003.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals004.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals004.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals005.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals005.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals006.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals006.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals007.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals007.fs - `` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,23)" id="FS0951">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals008.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals008.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS0951" span="(11,23-11,35)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals009.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals009.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"
        |> ignore

