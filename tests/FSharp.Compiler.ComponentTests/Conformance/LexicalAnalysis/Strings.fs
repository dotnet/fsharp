module Conformance.LexicalAnalysis.Strings

open Xunit
open FSharp.Test.Compiler

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
    |> withSingleDiagnostic (Warning 1252, Line 2, Col 11, Line 2, Col 15, invalidCharWarningMsg "\\937" "\\169")

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
    |> withSingleDiagnostic (Warning 1252, Line 2, Col 13, Line 2, Col 17, invalidCharWarningMsg "\\937" "\\169")

[<Fact>]
let ``Error messages for different notations only span invalid notation``() =
    Fs """
printfn "ok:\061;err:\937;err:\U12345678;ok:\U00005678;fin" 
    """
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 1245, Line 2, Col 31, Line 2, Col 41, "\\U12345678 is not a valid Unicode character escape sequence")
        (Warning 1252, Line 2, Col 22, Line 2, Col 26,  invalidCharWarningMsg "\\937" "\\169")
    ]
