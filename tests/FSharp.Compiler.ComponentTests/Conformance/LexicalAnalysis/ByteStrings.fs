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
        (Error 1158, Line 3, Col 9, Line 3, Col 16, "This is not a valid character literal")
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
