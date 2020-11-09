// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module StringsAndCharacters =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"Backslash01.fs"|])>]
    let ``StringsAndCharacters - Backslash01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"Backslash02.fs"|])>]
    let ``StringsAndCharacters - Backslash02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteChars01.fs"|])>]
    let ``StringsAndCharacters - ByteChars01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteChars02.fs"|])>]
    let ``StringsAndCharacters - ByteChars02.fs - --codepage:1252`` compilation =
        compilation
        |> withOptions ["--codepage:1252"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects id="FS1157" span="(7,4-7,8)" status="error">This is not a valid byte literal</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteChars02.fs"|])>]
    let ``StringsAndCharacters - E_ByteChars02.fs - --codepage:1252 --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--codepage:1252"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1157
        |> withDiagnosticMessageMatches "This is not a valid byte literal"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString01.fs"|])>]
    let ``StringsAndCharacters - ByteString01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString02.fs"|])>]
    let ``StringsAndCharacters - ByteString02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"ByteString03.fs"|])>]
    let ``StringsAndCharacters - ByteString03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"VerbatimString01.fs"|])>]
    let ``StringsAndCharacters - VerbatimString01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals01.fs"|])>]
    let ``StringsAndCharacters - CharLiterals01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals02.fs"|])>]
    let ``StringsAndCharacters - CharLiterals02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"CharLiterals03.fs"|])>]
    let ``StringsAndCharacters - CharLiterals03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"EscapeSequences01.fs"|])>]
    let ``StringsAndCharacters - EscapeSequences01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"EscapeSequences02.fs"|])>]
    let ``StringsAndCharacters - EscapeSequences02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString01.fs"|])>]
    let ``StringsAndCharacters - UnicodeString01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString02.fs"|])>]
    let ``StringsAndCharacters - UnicodeString02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects id="FS1245" span="(8,21-8,33)" status="error">\\UFFFF0000 is not a valid Unicode character escape sequence</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_BogusLongUnicodeEscape.fs"|])>]
    let ``StringsAndCharacters - E_BogusLongUnicodeEscape.fs - --codepage:1252 --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--codepage:1252"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1245
        |> withDiagnosticMessageMatches "\\UFFFF0000 is not a valid Unicode character escape sequence"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects id="FS1140" status="error">This byte array literal contains characters that do not encode as a single byte</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteStrUnicodeChar01.fs"|])>]
    let ``StringsAndCharacters - E_ByteStrUnicodeChar01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1140
        |> withDiagnosticMessageMatches "This byte array literal contains characters that do not encode as a single byte"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects id="FS1157" status="error">This is not a valid byte literal</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_ByteCharUnicodeChar01.fs"|])>]
    let ``StringsAndCharacters - E_ByteCharUnicodeChar01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1157
        |> withDiagnosticMessageMatches "This is not a valid byte literal"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects id="FS0010" span="(15,17-15,18)" status="error">Unexpected character '\\' in binding</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"E_MalformedShortUnicode01.fs"|])>]
    let ``StringsAndCharacters - E_MalformedShortUnicode01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected character '\\' in binding"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"UnicodeString03.fs"|])>]
    let ``StringsAndCharacters - UnicodeString03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuote.fs"|])>]
    let ``StringsAndCharacters - TripleQuote.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuoteString01.fs"|])>]
    let ``StringsAndCharacters - TripleQuoteString01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/StringsAndCharacters)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/StringsAndCharacters", Includes=[|"TripleQuoteString02.fs"|])>]
    let ``StringsAndCharacters - TripleQuoteString02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

