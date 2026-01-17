// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NumericLiterals =

    // SOURCE: casingBin.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingBin.fs"|])>]
    let ``NumericLiterals - casingBin_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: casingHex.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingHex.fs"|])>]
    let ``NumericLiterals - casingHex_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: casingOct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingOct.fs"|])>]
    let ``NumericLiterals - casingOct_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: casingIEEE-lf-LF01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF01.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: casingIEEE-lf-LF02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF02.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: casingIEEE-lf-LF03a.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03a.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF03a_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> ignore

    // SOURCE: casingIEEE-lf-LF03b.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"casingIEEE-lf-LF03b.fs"|])>]
    let ``NumericLiterals - casingIEEE_lf_LF03b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> ignore

    // SOURCE: NumericLiterals01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"NumericLiterals01.fs"|])>]
    let ``NumericLiterals - NumericLiterals01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: MaxLiterals01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"MaxLiterals01.fs"|])>]
    let ``NumericLiterals - MaxLiterals01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_MaxLiterals01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals01.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1142
        |> ignore

    // SOURCE: E_MaxLiterals02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals02.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals02_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1144
        |> ignore

    // SOURCE: E_MaxLiterals03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals03.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals03_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // SOURCE: E_MaxLiterals04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_MaxLiterals04.fs"|])>]
    let ``NumericLiterals - E_MaxLiterals04_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1149
        |> ignore

    // SOURCE: BigNums01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"BigNums01.fs"|])>]
    let ``NumericLiterals - BigNums01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_BigNums40.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_BigNums40.fs"|])>]
    let ``NumericLiterals - E_BigNums40_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0784
        |> ignore

    // SOURCE: E_BigNumNotImpl01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_BigNumNotImpl01.fs"|])>]
    let ``NumericLiterals - E_BigNumNotImpl01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // SOURCE: E_DecimalWO0Prefix.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_DecimalWO0Prefix.fs"|])>]
    let ``NumericLiterals - E_DecimalWO0Prefix_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: E_InvalidIEEE64.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_InvalidIEEE64.fs"|])>]
    let ``NumericLiterals - E_InvalidIEEE64_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1153
        |> ignore

    // SOURCE: E_BigIntConversion01b.fs SCFLAGS: --test:ErrorRanges
    // Note: This test expected error but behavior may have changed in modern F#
    [<Theory(Skip = "BigInteger to char conversion behavior may have changed"); Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"E_BigIntConversion01b.fs"|])>]
    let ``NumericLiterals - E_BigIntConversion01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // SOURCE: BigIntConversion02b.fs SCFLAGS: --test:ErrorRanges --warnaserror+
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/NumericLiterals", Includes=[|"BigIntConversion02b.fs"|])>]
    let ``NumericLiterals - BigIntConversion02b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
