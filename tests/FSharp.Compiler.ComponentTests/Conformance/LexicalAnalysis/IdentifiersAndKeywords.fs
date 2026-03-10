// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IdentifiersAndKeywords =

    // SOURCE: ValidIdentifier01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"ValidIdentifier01.fs"|])>]
    let ``IdentifiersAndKeywords - ValidIdentifier01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ValidIdentifier02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"ValidIdentifier02.fs"|])>]
    let ``IdentifiersAndKeywords - ValidIdentifier02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_InvalidIdentifier01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_InvalidIdentifier01.fs"|])>]
    let ``IdentifiersAndKeywords - E_InvalidIdentifier01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1156
        |> ignore

    // SOURCE: E_NameCollision01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_NameCollision01.fs"|])>]
    let ``IdentifiersAndKeywords - E_NameCollision01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> ignore

    // SOURCE: W_ReservedWord01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"W_ReservedWord01.fs"|])>]
    let ``IdentifiersAndKeywords - W_ReservedWord01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_KeywordIdent01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_KeywordIdent01.fs"|])>]
    let ``IdentifiersAndKeywords - E_KeywordIdent01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: E_ValidIdentifier03.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ValidIdentifier03.fs"|])>]
    let ``IdentifiersAndKeywords - E_ValidIdentifier03_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0883
        |> ignore

    // SOURCE: E_ValidIdentifier04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ValidIdentifier04.fs"|])>]
    let ``IdentifiersAndKeywords - E_ValidIdentifier04_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: backtickmoduleandtypenames.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"backtickmoduleandtypenames.fsx"|])>]
    let ``IdentifiersAndKeywords - backtickmoduleandtypenames_fsx`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: backtickmoduleandtypenames.fsx (FSIMODE=EXEC)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"backtickmoduleandtypenames.fsx"|])>]
    let ``IdentifiersAndKeywords - backtickmoduleandtypenames_fsx - FSIMODE`` compilation =
        compilation
        |> asFsx
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: StructNotAllowDoKeyword.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"StructNotAllowDoKeyword.fs"|])>]
    let ``IdentifiersAndKeywords - StructNotAllowDoKeyword_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> ignore

    // SOURCE: E_MissingQualification.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_MissingQualification.fs"|])>]
    let ``IdentifiersAndKeywords - E_MissingQualification_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0599
        |> ignore

    // SOURCE: W_IdentContainsAtSign.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"W_IdentContainsAtSign.fs"|])>]
    let ``IdentifiersAndKeywords - W_IdentContainsAtSign_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 1104
        |> ignore

    // SOURCE: E_ReservedIdentKeywords.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_ReservedIdentKeywords.fs"|])>]
    let ``IdentifiersAndKeywords - E_ReservedIdentKeywords_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 0046
        |> ignore

    // SOURCE: E_QuotedTypeModuleNames01.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifiersAndKeywords", Includes=[|"E_QuotedTypeModuleNames01.fs"|])>]
    let ``IdentifiersAndKeywords - E_QuotedTypeModuleNames01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0883
        |> ignore
