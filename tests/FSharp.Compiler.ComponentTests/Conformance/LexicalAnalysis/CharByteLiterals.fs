module Conformance.LexicalAnalysis.CharByteLiterals

open Xunit
open FSharp.Test.Compiler

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
    |> withDiagnostics [
        (Error 1157, Line 3, Col 5, Line 3, Col  9, "This is not a valid byte literal")
        (Error 1157, Line 4, Col 5, Line 4, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 5, Col 5, Line 5, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 6, Col 5, Line 6, Col 14, "This is not a valid byte literal")
        (Error 1157, Line 7, Col 5, Line 7, Col 18, "This is not a valid byte literal")
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
    |> withDiagnostics [

        (Error 1157, Line 4, Col 5, Line 4, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 5, Col 5, Line 5, Col 12, "This is not a valid byte literal")
        (Error 1157, Line 6, Col 5, Line 6, Col 14, "This is not a valid byte literal")
        (Error 1157, Line 7, Col 5, Line 7, Col 18, "This is not a valid byte literal")
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
    |> withDiagnostics [
        (Error 1157, Line 3, Col 5, Line 3, Col  9, "This is not a valid byte literal")
        (Error 1157, Line 4, Col 5, Line 4, Col 12, "This is not a valid byte literal")

        (Error 1157, Line 6, Col 5, Line 6, Col 14, "This is not a valid byte literal")
        (Error 1157, Line 7, Col 5, Line 7, Col 18, "This is not a valid byte literal")
    ]
