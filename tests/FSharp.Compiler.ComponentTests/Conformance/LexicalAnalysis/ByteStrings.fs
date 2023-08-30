module Conformance.LexicalAnalysis.ByteStrings

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Decimal char > 255 is not valid``() =
    Fs """
// Ω
let _ = "\937"B
    """
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 1252, Line 3, Col 10, Line 3, Col 14, "'\\937' is not a valid character literal")
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
        (Error 1157, Line 3, Col 9, Line 3, Col 16, "This is not a valid byte literal")
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
let ``values in different notations are invalid above 127``() =
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
        (Error 1157, Line 3, Col 5, Line 3, Col  9, "This is not a valid byte literal")
        (Error 1157, Line 4, Col 5, Line 4, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 5, Col 5, Line 5, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 6, Col 5, Line 6, Col 14, "This is not a valid byte literal")
        (Error 1157, Line 7, Col 5, Line 7, Col 18, "This is not a valid byte literal")
    ]
    
[<Fact>]
let ``Error messages for different notations only span invalid notation``() =
    Fs """
"ok:\061;err:\937;err:\U12345678;err:\U00005678;fin"B
|> printfn "%A"
    """
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 1252, Line 2, Col 14, Line 2, Col 18, "'\\937' is not a valid character literal")
        (Error 1245, Line 2, Col 23, Line 2, Col 33, "\\U12345678 is not a valid Unicode character escape sequence")

        // Note: Error for `\U00005678` spans full byte string:
        //       Is a valid char, but two bytes -> not valid inside byte string
        //       But check for correct byte happens after string is finished 
        //           (because `B` suffix -> only know at end if it's a byte string)
        //       -> Don't have direct access to range of invalid char any more
        (Error 1140, Line 2, Col 1, Line 2, Col 54, "This byte array literal contains characters that do not encode as a single byte")
    ]
