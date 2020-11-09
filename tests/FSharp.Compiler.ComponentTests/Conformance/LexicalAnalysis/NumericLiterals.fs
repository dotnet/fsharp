// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module NumericLiterals =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingBin.fs"|])>]
    let ``NumericLiterals - casingBin.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingHex.fs"|])>]
    let ``NumericLiterals - casingHex.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingOct.fs"|])>]
    let ``NumericLiterals - casingOct.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF01.fs"|])>]
    let ``NumericLiterals - casingIEEE-lf-LF01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF02.fs"|])>]
    let ``NumericLiterals - casingIEEE-lf-LF02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(6,9-6,14)" id="FS1156">This is not a valid numeric literal. Valid numeric literals include</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03a.fs"|])>]
    let ``NumericLiterals - casingIEEE-lf-LF03a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal. Valid numeric literals include"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" id="FS1156" span="(6,9-6,14)">This is not a valid numeric literal. Valid numeric literals include</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03b.fs"|])>]
    let ``NumericLiterals - casingIEEE-lf-LF03b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> withDiagnosticMessageMatches "This is not a valid numeric literal. Valid numeric literals include"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - enumNegativeLiterals001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals001.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals002.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals003.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals003.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals004.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals004.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals005.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals005.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals006.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals006.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,30)" id="FS0010">Unexpected symbol '-' in union case$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals007.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals007.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '-' in union case$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,23)" id="FS0951">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals008.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals008.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS0951" span="(11,23-11,35)" status="error">Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_enumNegativeLiterals009.fs"|])>]
    let ``NumericLiterals - E_enumNegativeLiterals009.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0951
        |> withDiagnosticMessageMatches "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"NumericaLiterals01.fs"|])>]
    let ``NumericLiterals - NumericaLiterals01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"MaxLiterals01.fs"|])>]
    let ``NumericLiterals - MaxLiterals01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals01.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1149
        |> withDiagnosticMessageMatches "This number is outside the allowable range for 64-bit signed integers"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS1150" status="error">This number is outside the allowable range for 64-bit unsigned integers</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals02.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1150
        |> withDiagnosticMessageMatches "This number is outside the allowable range for 64-bit unsigned integers"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(18,18)" id="FS0001">The type 'uint64' does not support the operator '~-'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals03.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'uint64' does not support the operator '~-'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS1149" status="error">This number is outside the allowable range for 64-bit signed integers</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals04.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1149
        |> withDiagnosticMessageMatches "This number is outside the allowable range for 64-bit signed integers"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"BigNums01.fs"|])>]
    let ``NumericLiterals - BigNums01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS0039" span="(13,23)" status="error">The value, constructor, namespace or type 'FromString' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_BigNumNotImpl01.fs"|])>]
    let ``NumericLiterals - E_BigNumNotImpl01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value, constructor, namespace or type 'FromString' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS0010" span="(6,9)" status="error">Unexpected symbol '\.' in binding</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_DecimalWO0Prefix.fs"|])>]
    let ``NumericLiterals - E_DecimalWO0Prefix.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\.' in binding"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects id="FS1153" status="error" span="(7,20-7,46)">Invalid floating point number$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_InvalidIEEE64.fs"|])>]
    let ``NumericLiterals - E_InvalidIEEE64.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1153
        |> withDiagnosticMessageMatches "Invalid floating point number$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/NumericLiterals)
    //<Expects status="error" span="(8,14-8,17)" id="FS0001">The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'char'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_BigIntConversion01b.fs"|])>]
    let ``NumericLiterals - E_BigIntConversion01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'System\.Numerics\.BigInteger' does not support a conversion to the type 'char'$"

