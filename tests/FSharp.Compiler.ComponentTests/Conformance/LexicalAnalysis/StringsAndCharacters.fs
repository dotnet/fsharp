// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StringsAndCharacters =

    // SOURCE: Backslash01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"Backslash01.fs"|])>]
    let ``StringsAndCharacters - Backslash01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: Backslash02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"Backslash02.fs"|])>]
    let ``StringsAndCharacters - Backslash02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ByteChars01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteChars01.fs"|])>]
    let ``StringsAndCharacters - ByteChars01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ByteChars02.fs SCFLAGS: --codepage:1252
    // Skip: codepage option may not work on all platforms
    [<Theory(Skip = "codepage:1252 may not work on macOS"); Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteChars02.fs"|])>]
    let ``StringsAndCharacters - ByteChars02_fs`` compilation =
        compilation
        |> withOptions ["--codepage:1252"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_ByteChars02.fs SCFLAGS: --codepage:1252 --test:ErrorRanges
    // Skip: codepage option may not work on all platforms
    [<Theory(Skip = "codepage:1252 may not work on macOS"); Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteChars02.fs"|])>]
    let ``StringsAndCharacters - E_ByteChars02_fs`` compilation =
        compilation
        |> withOptions ["--codepage:1252"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1157
        |> ignore

    // SOURCE: ByteString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString01.fs"|])>]
    let ``StringsAndCharacters - ByteString01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ByteString02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString02.fs"|])>]
    let ``StringsAndCharacters - ByteString02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ByteString03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString03.fs"|])>]
    let ``StringsAndCharacters - ByteString03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: VerbatimString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"VerbatimString01.fs"|])>]
    let ``StringsAndCharacters - VerbatimString01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: CharLiterals01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals01.fs"|])>]
    let ``StringsAndCharacters - CharLiterals01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: CharLiterals02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals02.fs"|])>]
    let ``StringsAndCharacters - CharLiterals02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: CharLiterals03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals03.fs"|])>]
    let ``StringsAndCharacters - CharLiterals03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: EscapeSequences01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"EscapeSequences01.fs"|])>]
    let ``StringsAndCharacters - EscapeSequences01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: EscapeSequences02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"EscapeSequences02.fs"|])>]
    let ``StringsAndCharacters - EscapeSequences02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: UnicodeString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString01.fs"|])>]
    let ``StringsAndCharacters - UnicodeString01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: UnicodeString02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString02.fs"|])>]
    let ``StringsAndCharacters - UnicodeString02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_BogusLongUnicodeEscape.fs SCFLAGS: --codepage:1252 --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_BogusLongUnicodeEscape.fs"|])>]
    let ``StringsAndCharacters - E_BogusLongUnicodeEscape_fs`` compilation =
        compilation
        |> withOptions ["--codepage:1252"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1159
        |> ignore

    // SOURCE: E_ByteStrUnicodeChar01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteStrUnicodeChar01.fs"|])>]
    let ``StringsAndCharacters - E_ByteStrUnicodeChar01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1140
        |> ignore

    // SOURCE: E_ByteCharUnicodeChar01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteCharUnicodeChar01.fs"|])>]
    let ``StringsAndCharacters - E_ByteCharUnicodeChar01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1157
        |> ignore

    // SOURCE: E_MalformedShortUnicode01.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_MalformedShortUnicode01.fs"|])>]
    let ``StringsAndCharacters - E_MalformedShortUnicode01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: UnicodeString03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString03.fs"|])>]
    let ``StringsAndCharacters - UnicodeString03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: TripleQuote.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuote.fs"|])>]
    let ``StringsAndCharacters - TripleQuote_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: TripleQuoteString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuoteString01.fs"|])>]
    let ``StringsAndCharacters - TripleQuoteString01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: TripleQuoteString02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuoteString02.fs"|])>]
    let ``StringsAndCharacters - TripleQuoteString02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: TripleQuoteStringInFSI01.fsx
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``StringsAndCharacters - TripleQuoteStringInFSI01_fsx`` () = ()

    // SOURCE: TripleQuoteStringInFSI02.fsx
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``StringsAndCharacters - TripleQuoteStringInFSI02_fsx`` () = ()
