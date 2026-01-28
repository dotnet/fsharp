// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NumericLiterals =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - enumNegativeLiterals001_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals001_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals002_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals003_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals004_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals005_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals006_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals007_fs - `` compilation =
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
    let ``NumericLiterals - E_enumNegativeLiterals008_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals009.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals009_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"
        |> ignore

    // Binary literal casing: 0b vs 0B both work
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingBin.fs"|])>]
    let ``NumericLiterals - casingBin_fs - binary casing`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // Hex literal casing: 0x vs 0X both work
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingHex.fs"|])>]
    let ``NumericLiterals - casingHex_fs - hex casing`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // Octal literal casing: 0o vs 0O both work
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingOct.fs"|])>]
    let ``NumericLiterals - casingOct_fs - octal casing`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // IEEE float casing: lf vs LF produce different types (IEEE32 vs IEEE64)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF01.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF01_fs - lf vs LF types`` compilation =
        compilation
        |> asExe
        |> withNoWarn 52  // Ignore defensive copy warnings
        |> withNoWarn 221 // Ignore implicit module name warning
        |> compile
        |> shouldSucceed
        |> ignore

    // IEEE float casing: 0X prefix with lf/LF
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF02.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF02_fs - 0X prefix with lf LF`` compilation =
        compilation
        |> asExe
        |> withNoWarn 52  // Ignore defensive copy warnings
        |> withNoWarn 221 // Ignore implicit module name warning
        |> compile
        |> shouldSucceed
        |> ignore

    // IEEE float casing: lF is illegal (mixed case)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03a.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF03a_fs - lF illegal`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal"
        |> ignore

    // IEEE float casing: Lf is illegal (mixed case)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03b.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF03b_fs - Lf illegal`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal"
        |> ignore

    // Signed integer literals overflow (MaxSize + 1 / MinSize - 1)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals01.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals01_fs - signed overflow`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCodes [1142; 1145; 1147; 1149]
        |> ignore

    // Unsigned integer literals overflow (MaxSize + 1)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals02.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals02_fs - unsigned overflow`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCodes [1144; 1146; 1148; 1150]
        |> ignore

    // Negative unsigned integers (operator ~- not supported)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals03.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals03_fs - negative unsigned`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "does not support the operator '~-'"
        |> ignore

    // 64-bit signed overflow with binary and octal literals
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals04.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals04_fs - 64-bit binary octal overflow`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldFail
        |> withErrorCode 1149
        |> withDiagnosticMessageMatches "outside the allowable range for 64-bit signed integers"
        |> ignore

