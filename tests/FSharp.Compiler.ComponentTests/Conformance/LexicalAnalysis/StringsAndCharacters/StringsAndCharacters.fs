// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open System.IO

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StringsAndCharacters =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let compileAndRunAsFsxShouldSucceed compilation =
        compilation
        |> asFsx
        |> withNoWarn 988
        |> runFsi
        |> shouldSucceed

    let compileAndRunAsExeShouldSucceed compilation =
        compilation
        |> withNoWarn 988
        |> asFs
        |> compileExeAndRun
        |> shouldSucceed

    [<FileInlineData("Backslash01.fsx")>]
    [<FileInlineData("Backslash02.fsx")>]
    [<FileInlineData("ByteChars01.fsx")>]
    [<FileInlineData("ByteChars02.fsx")>]
    [<FileInlineData("ByteString01.fsx")>]
    [<FileInlineData("ByteString02.fsx")>]
    [<FileInlineData("ByteString03.fsx")>]
    [<FileInlineData("CharLiterals01.fsx")>]
    [<FileInlineData("CharLiterals02.fsx")>]
    [<FileInlineData("CharLiterals03.fsx")>]
    [<FileInlineData("EscapeSequences01.fsx")>]
    [<FileInlineData("EscapeSequences02.fsx")>]
    [<FileInlineData("TripleQuote.fsx")>]
    [<FileInlineData("TripleQuoteString01.fsx")>]
    [<FileInlineData("TripleQuoteString02.fsx")>]
    [<FileInlineData("TripleQuoteStringInFSI01.fsx")>]
    [<FileInlineData("TripleQuoteStringInFSI02.fsx")>]
    [<FileInlineData("UnicodeString01.fsx")>]
    [<FileInlineData("UnicodeString02.fsx")>]
    [<FileInlineData("UnicodeString03.fsx")>]
    [<FileInlineData("VerbatimString01.fsx")>]
    [<FileInlineData("WhiteSpace01.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("Backslash01.fsx")>]
    [<FileInlineData("Backslash02.fsx")>]
    [<FileInlineData("ByteChars01.fsx")>]
    [<FileInlineData("ByteChars02.fsx")>]
    [<FileInlineData("ByteString01.fsx")>]
    [<FileInlineData("ByteString02.fsx")>]
    [<FileInlineData("ByteString03.fsx")>]
    [<FileInlineData("CharLiterals01.fsx")>]
    [<FileInlineData("CharLiterals02.fsx")>]
    [<FileInlineData("CharLiterals03.fsx")>]
    [<FileInlineData("EscapeSequences01.fsx")>]
    [<FileInlineData("EscapeSequences02.fsx")>]
    [<FileInlineData("TripleQuote.fsx")>]
    [<FileInlineData("TripleQuoteString01.fsx")>]
    [<FileInlineData("TripleQuoteString02.fsx")>]
    [<FileInlineData("TripleQuoteStringInFSI01.fsx")>]
    [<FileInlineData("TripleQuoteStringInFSI02.fsx")>]
    [<FileInlineData("UnicodeString01.fsx")>]
    [<FileInlineData("UnicodeString02.fsx")>]
    [<FileInlineData("UnicodeString03.fsx")>]
    [<FileInlineData("VerbatimString01.fsx")>]
    [<FileInlineData("WhiteSpace01.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsExeShouldSucceed


    [<Theory; FileInlineData("E_BogusLongUnicodeEscape.fsx")>]
    let ``E_BogusLongUnicodeEscape_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1159, Line 6, Col 22, Line 6, Col 34, @"This Unicode encoding is only valid in string literals")
            (Error 1245, Line 7, Col 22, Line 7, Col 32, @"\U00110000 is not a valid Unicode character escape sequence")
            (Error 1245, Line 8, Col 22, Line 8, Col 32, @"\UFFFF0000 is not a valid Unicode character escape sequence")
        ]

    [<Theory; FileInlineData("E_ByteChars02.fsx")>]
    let ``E_ByteChars02_fsx`` compilation =
        compilation
        |> getCompilation
        |> withCodepage "1252"
        |> shouldFailWithDiagnostics [
            (Error 1157, Line 7, Col 4, Line 7, Col 8, @"This is not a valid byte character literal. The value must be less than or equal to '\127'B.")
        ]

    [<Theory; FileInlineData("E_ByteCharUnicodeChar01.fsx")>]
    let ``E_ByteCharUnicodeChar01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1157, Line 9, Col 9, Line 9, Col 18, @"This is not a valid byte character literal. The value must be less than or equal to '\127'B.")
        ]

    [<Theory; FileInlineData("E_ByteStrUnicodeChar01.fsx")>]
    let ``E_ByteStrUnicodeChar01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1140, Line 9, Col 9, Line 9, Col 18, "This byte array literal contains 1 characters that do not encode as a single byte")
        ]

    [<Theory; FileInlineData("E_MalformedShortUnicode01.fsx")>]
    let ``E_MalformedShortUnicode01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 11, Col 18, Line 11, Col 19, @"Unexpected character '\' in expression. Expected identifier or other token.")
            (Error 10, Line 13, Col 18, Line 13, Col 19, @"Unexpected character '\' in expression. Expected identifier or other token.")
            (Error 10, Line 15, Col 17, Line 15, Col 18, @"Unexpected character '\' in binding")
        ]

    /// `'%s' is not a valid character literal.` with note about wrapped value and error soon
    let private invalidCharWarningMsg value wrapped = 
        FSComp.SR.lexInvalidCharLiteralInString (value, wrapped)
        |> snd

    [<Fact>]
    let ``Decimal char > 255 is not valid``() =
        Fs """
    printfn "Ω\937"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 1252, Line 2, Col 15, Line 2, Col 19, """'\937' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\169'. In a future F# version this warning will be promoted to an error.""")
        ]

    [<Fact>]
    let ``Decimal char between 128 and 256 is valid``() =
        Fs """
    printfn "ú\250"
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Decimal char < 128 is valid``() =
        Fs """
    printfn "a\097"
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``In verbatim string: \937 is valid and not parsed as single char``() =
        Fs """
    if @"\937" <> "\\937" then failwith "should not be trigraph"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Error message for invalid decimal char contains only invalid trigraph``() =
        Fs """
    printfn "foo\937bar"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 1252, Line 2, Col 17, Line 2, Col 21, """'\937' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\169'. In a future F# version this warning will be promoted to an error.""")
        ]

    [<Fact>]
    let ``Error messages for different notations only span invalid notation``() =
        Fs """
    printfn "ok:\061;err:\937;err:\U12345678;ok:\U00005678;fin" 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1245, Line 2, Col 35, Line 2, Col 45, "\\U12345678 is not a valid Unicode character escape sequence");
            (Warning 1252, Line 2, Col 26, Line 2, Col 30, """'\937' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\169'. In a future F# version this warning will be promoted to an error.""")
        ]

    /// `This is not a valid ASCII byte literal. Value should be < 128y.` with note that error soon
    let private invalidTrigraphCharWarningMsg = 
        FSComp.SR.lexInvalidTrigraphAsciiByteLiteral ()
        |> snd

    [<Fact>]
    let ``all byte char notations pass type check`` () =
        Fs """
    [
        'a'B
        '\097'B
        '\x61'B
        '\u0061'B
        '\U00000061'B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``all byte char notations produce correct value``() =
        Fs """
    let chars = [
        'a'B
        '\097'B
        '\x61'B
        '\u0061'B
        '\U00000061'B
    ]
    let expected = 97uy
    chars
    |> List.iteri (fun i actual -> if actual <> expected then failwithf "[%i]: Expected %A, but was %A" i expected actual)
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``byte value > 128uy fails in all char notations``() =
        Fs """
    [
        'ú'B
        '\250'B
        '\xFA'B
        '\u00FA'B
        '\U000000FA'B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1157, Line 3, Col 9, Line 3, Col 13, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 5, Col 9, Line 5, Col 16, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 6, Col 9, Line 6, Col 18, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 7, Col 9, Line 7, Col 22, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Warning 1157, Line 4, Col 9, Line 4, Col 16, """This is not a valid byte character literal. The value must be less than or equal to '\127'B.
Note: In a future F# version this warning will be promoted to an error.""")
        ]

    [<Fact>]
    let ``127uy typechecks in char notations``() =
        Fs """
    [
        // DELETE -> no direct char representation
        '\127'B
        '\x7F'B
        '\u007F'B
        '\U0000007F'B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``128uy fails typecheck in char notations``() =
        Fs """
    [
        // Padding Character -> no direct char representation
        '\128'B
        '\x80'B
        '\u0080'B
        '\U00000080'B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1157, Line 5, Col 9, Line 5, Col 16, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 6, Col 9, Line 6, Col 18, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 7, Col 9, Line 7, Col 22, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Warning 1157, Line 4, Col 9, Line 4, Col 16, """This is not a valid byte character literal. The value must be less than or equal to '\127'B.
Note: In a future F# version this warning will be promoted to an error.""")
        ]

    [<Fact>]
    let ``value out of byte range fails in char notations``() =
        Fs """
    [
        'β'B
        '\946'B
        // requires more than 2 digits -> no decimal representation
        '\u03B2'B
        '\U000003B2'B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1157, Line 3, Col 9, Line 3, Col 13, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 4, Col 9, Line 4, Col 16, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 6, Col 9, Line 6, Col 18, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
            (Error 1157, Line 7, Col 9, Line 7, Col 22, "This is not a valid byte character literal. The value must be less than or equal to '\\127'B.")
        ]

    /// `This byte array literal contains %d characters that do not encode as a single byte`
    let private invalidTwoByteErrorMsg count =
        FSComp.SR.lexByteArrayCannotEncode (count)
        |> snd

    /// `This byte array literal contains %d non-ASCII characters.`
    let private invalidAsciiWarningMsg count =
        FSComp.SR.lexByteArrayOutisdeAscii (count)
        |> snd

    [<Fact>]
    let ``Decimal char between 128 and 256 is not valid``() =
        Fs """
    // ú
    let _ = "\250"B
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
              (Warning 1253, Line 3, Col 13, Line 3, Col 20, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
        ]

    [<Fact>]
    let ``Values in different notations are invalid above 127``() =
        Fs """
    [
        "ú"B
        "\128"B
        "\x80"B
        "\u0080"B
        "\U00000080"B
    ]
    |> ignore
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 1253, Line 3, Col 9, Line 3, Col 13, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
            (Warning 1253, Line 4, Col 9, Line 4, Col 16, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
            (Warning 1253, Line 5, Col 9, Line 5, Col 16, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
            (Warning 1253, Line 6, Col 9, Line 6, Col 18, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
            (Warning 1253, Line 7, Col 9, Line 7, Col 22, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
        ]

    [<Fact>]
    let ``Decimal char > 255 and > 128 after wrapping triggers two diagnostics``() =
        // Currently `\937` trigger TWO diags:
        // * Invalid trigraph which gets wrapped -> warning spanning just trigraph
        // * Wrapped char is still >= 128 (but < 256) -> warning spanning full byte string
        // Those two are checked at different locations (trigraph: while parsing trigraph; 2-byte char: after Byte string was parsed)
        //   with different infos available (trigraph: trigraph, but don't know if string or byte string; 2-byte-char: byte string, but don't know source notation)
        // -> both diags get emitted!
        //
        // Should be resolved once invalid trigraph gets promoted to error.
        Fs """
    // Ω
    let _ = "\937"B
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 1252, Line 3, Col 14, Line 3, Col 18, """'\937' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\169'. In a future F# version this warning will be promoted to an error.""")
            (Warning 1253, Line 3, Col 13, Line 3, Col 20, "This byte array literal contains 1 non-ASCII characters. All characters should be < 128y.")
        ]

    [<Fact>]
    let ``Emit both Error and Warning with correct count``() =
        Fs """
    let _ = "Ω --- Ω\169 --- Ωä  --- Ωü --- Ω"B
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1140, Line 2, Col 13, Line 2, Col 48, "This byte array literal contains 5 characters that do not encode as a single byte")
            (Warning 1253, Line 2, Col 13, Line 2, Col 48, "This byte array literal contains 3 non-ASCII characters. All characters should be < 128y.")
        ]
