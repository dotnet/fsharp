// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

module ByteStrings =

    open Xunit
    open FSharp.Test.Compiler

    /// `'%s' is not a valid character literal.` with note about wrapped value and error soon
    let private invalidCharWarningMsg value wrapped = 
        FSComp.SR.lexInvalidCharLiteralInString (value, wrapped)
        |> snd

    /// `This byte array literal contains %d characters that do not encode as a single byte`
    let private invalidTwoByteErrorMsg count =
        FSComp.SR.lexByteArrayCannotEncode (count)
        |> snd

    /// `This byte array literal contains %d non-ASCII characters.`
    let private invalidAsciiWarningMsg count =
        FSComp.SR.lexByteArrayOutisdeAscii (count)
        |> snd

    [<Fact>]
    let ``Decimal char > 255 is not valid``() =
        Fs """
    // Ω
    let _ = "\837"B
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 1252, Line 3, Col 14, Line 3, Col 18, """'\837' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\069'. In a future F# version this warning will be promoted to an error.""")
        ]

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
    let ``Decimal char < 128 is valid``() =
        Fs """
    let _ = "a\097"B
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``In verbatim string: \937 is valid and not parsed as single char``() =
        Fs """
    if @"\937"B <> "\\937"B then failwith "should not be trigraph"
        """
        |> compileExeAndRun
        |> shouldSucceed


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
    let ``Error messages for different notations only span invalid notation``() =
        Fs """
    "ok:\061;err:\837;err:\U12345678;err:\U00005678;fin"B
    |> printfn "%A"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1245, Line 2, Col 27, Line 2, Col 37, "\\U12345678 is not a valid Unicode character escape sequence")
            (Error 1140, Line 2, Col 5, Line 2, Col 58, "This byte array literal contains 1 characters that do not encode as a single byte")
            (Warning 1252, Line 2, Col 18, Line 2, Col 22, """'\837' is not a valid character literal.
Note: Currently the value is wrapped around byte range to '\069'. In a future F# version this warning will be promoted to an error.""")
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
