// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.``Active pattern arg counts``

open Xunit
open FSharp.Test.Compiler

/// Warning FS0025: Incomplete pattern matches on this expression.
/// We suppress this warning in the assertions below so that we
/// don't need to add a wildcard case to every match.
let [<Literal>] IncompletePatternMatches = 25

module TooFew =
    module ``int → int → unit`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → unit option`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → unit voption`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → Choice<unit, _>`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → bool`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s), e.g., 'P e1'.")

    module ``int → int → int`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → int option`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 19, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 21, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → int voption`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 19, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 21, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → Choice<int, _>`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 15, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 19, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 21, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

module Enough =
    module ``int → unit`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|) expr1 = P
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|) expr1 = P
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P () -> …`` () =
            FSharp """
let (|P|) expr1 = P
match 1 with P () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → unit option`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then Some P else None
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then Some P else None
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P () -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then Some P else None
match 1 with P () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → unit voption`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then ValueSome P else ValueNone
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then ValueSome P else ValueNone
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P () -> …`` () =
            FSharp """
let (|P|_|) expr1 = if expr1 = 1 then ValueSome P else ValueNone
match 1 with P () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → Choice<unit, _>`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|Q|) expr1 = if expr1 = 1 then P else Q
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|Q|) expr1 = if expr1 = 1 then P else Q
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P () -> …`` () =
            FSharp """
let (|P|Q|) expr1 = if expr1 = 1 then P else Q
match 1 with P () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → bool`` =
        [<Fact>]
        let ``match expr1 with P -> …`` () =
            FSharp """
let (|P|_|) expr1 = expr1 = 1
match 1 with P -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int`` =
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P expr1
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int option`` =
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some (P expr1) else None
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int voption`` =
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome (P expr1) else ValueNone
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → Choice<int, _>`` =
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P expr1 else Q
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → unit`` =
        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to unit.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
let expr2 = 2
let expr3 = 3
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is bound to a value of type (int -> unit).
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr2 is actually a pattern bound to a value of type (int -> unit).
        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → unit option`` =
        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to unit.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
let expr2 = 2
let expr3 = 3
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 39, Line 3, Col 16, Line 3, Col 19, "The value or constructor 'pat' is not defined.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → unit voption`` =
        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is unit.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to unit.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
let expr2 = 2
let expr3 = 3
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 39, Line 3, Col 16, Line 3, Col 19, "The value or constructor 'pat' is not defined.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withLangVersionPreview
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → Choice<unit, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 4, Col 14, Line 4, Col 24, "Only active patterns returning exactly one result may accept arguments")

        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 4, Col 14, Line 4, Col 25, "Only active patterns returning exactly one result may accept arguments")

        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
let expr2 = 2
let expr3 = 3
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 5, Col 14, Line 5, Col 27, "Only active patterns returning exactly one result may accept arguments")

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                Error 722, Line 3, Col 14, Line 3, Col 19, "Only active patterns returning exactly one result may accept arguments"
                Error 39, Line 3, Col 16, Line 3, Col 19, "The value or constructor 'pat' is not defined."
            ]

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 4, Col 14, Line 4, Col 21, "Only active patterns returning exactly one result may accept arguments")

    module ``int → int → bool`` =
        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern expects 1 expression argument(s), e.g., 'P e1'.")

        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern expects 1 expression argument(s), e.g., 'P e1'.")

        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
let expr2 = 2
let expr3 = 3
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 5, Col 14, Line 5, Col 27, "This active pattern expects 1 expression argument(s), e.g., 'P e1'.")

        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 39, Line 3, Col 16, Line 3, Col 19, "The value or constructor 'pat' is not defined.")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = expr1 = expr2
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → int`` =
        // Normal usage; pat is int.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to a value of type int.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
let expr2 = 2
let expr3 = 2
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // pat is bound to a value of type (int -> int).
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr2 is actually a pattern bound to a value of type (int -> int).
        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
let expr2 = 2
match 1 with P expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → int option`` =
        // Normal usage; pat is int.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to a value of type int.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
let expr2 = 2
let expr3 = 2
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → int voption`` =
        // Normal usage; pat is int.
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        // expr3 is actually a pattern bound to a value of type int.
        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
let expr2 = 2
let expr3 = 2
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldSucceed

    module ``int → int → Choice<int, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 4, Col 14, Line 4, Col 25, "Only active patterns returning exactly one result may accept arguments")

        [<Fact>]
        let ``match expr1 with P expr2 expr3 -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
let expr2 = 2
let expr3 = 2
match 1 with P expr2 expr3 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 5, Col 14, Line 5, Col 27, "Only active patterns returning exactly one result may accept arguments")

        [<Fact>]
        let ``match expr1 with P expr2 -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 722, Line 4, Col 14, Line 4, Col 25, "Only active patterns returning exactly one result may accept arguments")

module TooMany =
    module ``int → unit`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P () expr2 -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P
let expr2 = 2
match 1 with P () expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

    module ``int → unit option`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some P else None
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some P else None
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P () expr2 -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some P else None
let expr2 = 2
match 1 with P () expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

    module ``int → unit voption`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P () expr2 -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P () expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

    module ``int → Choice<unit, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P else Q
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P else Q
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

        [<Fact>]
        let ``match expr1 with P () expr2 -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P else Q
let expr2 = 2
match 1 with P () expr2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

    module ``int → bool`` =
        [<Fact>]
        let ``match expr1 with P pat -> …`` () =
            FSharp """
let (|P|_|) expr1 = expr1 = 1
match 1 with P pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 3, Col 14, Line 3, Col 19, "This active pattern does not expect any arguments, i.e., it should be used like 'P' instead of 'P x'.")

    module ``int → int`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P expr1
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|) (expr1 : int) = P expr1
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

    module ``int → int option`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some (P expr1) else None
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then Some (P expr1) else None
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

    module ``int → int voption`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome (P expr1) else ValueNone
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|_|) (expr1 : int) = if expr1 = 1 then ValueSome (P expr1) else ValueNone
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

    module ``int → Choice<int, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P expr1 else Q
let expr2 = 2
match 1 with P expr2 pat -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 25, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

        [<Fact>]
        let ``match expr1 with P expr2 () -> …`` () =
            FSharp """
let (|P|Q|) (expr1 : int) = if expr1 = 1 then P expr1 else Q
let expr2 = 2
match 1 with P expr2 () -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4,  Col 24, "This active pattern expects exactly one pattern argument, e.g., 'P pat'.")

    module ``int → int → unit`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|) (expr2 : int) (expr1 : int) = P
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → unit option`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then Some P else None
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → unit voption`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|_|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then ValueSome P else ValueNone
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → Choice<unit, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|Q|) (expr2 : int) (expr1 : int) = if expr1 = expr2 then P else Q
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → int`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|) expr2 expr1 = P (expr1 + expr2)
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → int option`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then Some (P (expr1 + expr2)) else None
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → int voption`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|_|) expr2 expr1 = if expr1 = expr2 then ValueSome (P (expr1 + expr2)) else ValueNone
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")

    module ``int → int → Choice<int, _>`` =
        [<Fact>]
        let ``match expr1 with P expr2 pat1 pat2 -> …`` () =
            FSharp """
let (|P|Q|) expr2 expr1 = if expr1 = expr2 then P (expr1 + expr2) else Q
let expr2 = 2
match 1 with P expr2 pat1 pat2 -> ()
            """
            |> withNoWarn IncompletePatternMatches
            |> typecheck
            |> shouldFail
            |> withSingleDiagnostic (Error 3868, Line 4, Col 14, Line 4, Col 31, "This active pattern expects 1 expression argument(s) and a pattern argument, e.g., 'P e1 pat'.")
