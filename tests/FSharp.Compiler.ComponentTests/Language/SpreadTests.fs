// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.SpreadTests

open FSharp.Test.Compiler
open Xunit

module NominalAndAnonymousRecords =
    let [<Literal>] SupportedLangVersion = "preview"

    module LangVersion =
        [<Fact>]
        let ``10 → error`` () =
            let src =
                """
                type R1 = { A : int; B : int }
                type R2 = { ...R1; C : int }
                let r1 = { A = 1; B = 2 }
                let r2 = { ...r1; C = 3 }
                """

            FSharp src
            |> withLangVersion10
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                Error 3350, Line 3, Col 29, Line 3, Col 34, "Feature 'record type and expression spreads' is not available in F# 10.0. Please use language version 'PREVIEW' or greater."
                Error 3350, Line 5, Col 28, Line 5, Col 33, "Feature 'record type and expression spreads' is not available in F# 10.0. Please use language version 'PREVIEW' or greater."
            ]

        [<Fact>]
        let ``> 10 → success`` () =
            let src =
                """
                type R1 = { A : int; B : int }
                type R2 = { ...R1; C : int }
                let r1 = { A = 1; B = 2 }
                let r2 = { ...r1; C = 3 }
                """

            FSharp src
            |> withLangVersion SupportedLangVersion
            |> typecheck
            |> shouldSucceed

    module Parsing =
        [<Fact>]
        let ``{...} → error`` () =
            let src =
                """
                type R1 = { A : int; B : int }
                type R2 = { ... }
                let r1 : R1 = { ... }
                let r2 = {| ... |}
                let r1' : R1 = { r1 with ... }
                let r2' = {| r1 with ... |}
                """

            FSharp src
            |> withLangVersion SupportedLangVersion
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                Error 3885, Line 3, Col 29, Line 3, Col 32, "Missing spread source type after '...'."
                Error 3884, Line 4, Col 33, Line 4, Col 36, "Missing spread source expression after '...'."
                Error 3884, Line 5, Col 29, Line 5, Col 32, "Missing spread source expression after '...'."
                Error 3884, Line 6, Col 42, Line 6, Col 45, "Missing spread source expression after '...'."
                Error 3884, Line 7, Col 38, Line 7, Col 41, "Missing spread source expression after '...'."
            ]

    module RecordTypeSpreads =
        module Algebra =
            /// No overlap, spread ⊕ field.
            [<Fact>]
            let ``{...{A,B},C} = {A,B} ⊕ {C} = {A,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { ...R1; C : int }

                    let _ : R2 = { A = 1; B = 2; C = 3 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, spread from anonymous record ⊕ field.
            [<Fact>]
            let ``{...{|A,B|},C} = {A,B} ⊕ {C} = {A,B,C}`` () =
                let src =
                    """
                    type R2 = { ...{| A : int; B : int |}; C : int }

                    let _ : R2 = { A = 1; B = 2; C = 3 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, field ⊕ spread.
            [<Fact>]
            let ``{A,...{B,C}} = {A} ⊕ {B,C} = {A,B,C}`` () =
                let src =
                    """
                    type R1 = { B : int; C : int }
                    type R2 = { A : int; ...R1 }

                    let _ : R2 = { A = 1; B = 2; C = 3 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, spread ⊕ spread.
            [<Fact>]
            let ``{...{A,B},...{C,D}} = {A,B} ⊕ {C,D} = {A,B,C,D}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { C : int; D : int }
                    type R3 = { ...R1; ...R2 }

                    let _ : R3 = { A = 1; B = 2; C = 3; D = 4 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward explicit duplicate field shadows field from spread.
            [<Fact>]
            let ``{...{A₀,B},A₁} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { ...R1; A : string }

                    let _ : R2 = { A = "1"; B = 2 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward spread field shadows leftward spread field.
            [<Fact>]
            let ``{...{A₀,B},...{A₁}} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { A : string }
                    type R3 = { ...R1; ...R2 }
                    type R4 = { ...R2; ...R1 }

                    let _ : R3 = { A = "1"; B = 2 }
                    let _ : R4 = { A = 1; B = 2 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward spread field shadows leftward explicit field with warning.
            [<Fact>]
            let ``{A₀,...{A₁,B}} = {A₀} ⊕ {A₁,B} = {A₁_warn,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { A : string; ...R1 }

                    let _ : R2 = { A = 1; B = 2 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Warning 3882, Line 3, Col 45, Line 3, Col 50, "Spread field 'A: int' from type 'R1' shadows an explicitly declared field with the same name.")

            /// Explicit duplicate fields remain disallowed.
            [<Fact>]
            let ``{A₀,...{A₁,B},A₂} = {A₀} ⊕ {A₁,B} ⊕ {A₂} = {A₁_warn,B,A₂_error}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { A : string; ...R1; A : float }

                    let _ : R2 = { A = 1; B = 2 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Warning 3882, Line 3, Col 45, Line 3, Col 50, "Spread field 'A: int' from type 'R1' shadows an explicitly declared field with the same name."
                    Error 37, Line 3, Col 52, Line 3, Col 53, "Duplicate definition of field 'A'"
                ]

            [<Fact>]
            let ``No dupes allowed, multiple`` () =
                let src =
                    """
                    type R1 = { A : int; B : string }
                    type R2 = { A : decimal }
                    type R3 = { ...R2; A : string; ...R1; A : float }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Warning 3882, Line 4, Col 52, Line 4, Col 57, "Spread field 'A: int' from type 'R1' shadows an explicitly declared field with the same name."
                    Error 37, Line 4, Col 59, Line 4, Col 60, "Duplicate definition of field 'A'"
                ]

        module Accessibility =
            /// Fields should have the accessibility of the target type.
            /// A spread from less to more accessible is valid as long as the less accessible
            /// fields are accessible at the point of the spread.
            [<Fact>]
            let ``Accessibility comes from target`` () =
                let src =
                    """
                    type private R1 = { A : int; B : string }
                    type public R2 = { ...R1 }

                    let public r2 : R2 = { A = 1; B = "2" }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module Mutability =
            [<Fact>]
            let ``Mutability is brought over`` () =
                let src =
                    """
                    type R1 = { A : int; mutable B : string }
                    type R2 = { ...R1 }

                    let r2 : R2 = { A = 1; B = "3" }
                    r2.B <- "99"
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module GenericTypeParameters =
            [<Fact>]
            let ``Single type parameter, inferred at usage`` () =
                let src =
                    """
                    type R1<'a> = { A : 'a; B : string }
                    type R2<'a> = { X : 'a; Y : string }
                    type R3<'a> = { ...R1<'a>; ...R2<'a> }

                    let _ : R3<_> = { A = 3; B = "lol"; X = 4; Y = "haha" }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Single type parameter, annotated at usage`` () =
                let src =
                    """
                    type R1<'a> = { A : 'a; B : string }
                    type R2<'a> = { X : 'a; Y : string }
                    type R3<'a> = { ...R1<'a>; ...R2<'a> }

                    let _ : R3<int> = { A = 3; B = "lol"; X = 4; Y = "haha" }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Multiple type parameters`` () =
                let src =
                    """
                    type R1<'a> = { A : 'a; B : string }
                    type R2<'a> = { X : 'a; Y : string }
                    type R3<'a, 'b> = { ...R1<'a>; ...R2<'b> }

                    let _ : R3<_, _> = { A = 3; B = "lol"; X = 3.14; Y = "haha" }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``'a → 'a list`` () =
                let src =
                    """
                    type R1<'a> = { A : 'a }
                    type R2<'a> = { ...R1<'a list> }

                    let _ : R2<int> = { A = [3] }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Single type parameter, not in scope, not allowed`` () =
                let src =
                    """
                    type R1<'a> = { A : 'a; B : string }
                    type R2<'a> = { X : 'a; Y : string }
                    type R3<'a> = { ...R1<'a>; ...R2<'b> }
                    type R4 = { ...R1<'a>; ...R2<'b> }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 39, Line 4, Col 54, Line 4, Col 56, "The type parameter 'b is not defined."
                    Error 39, Line 5, Col 39, Line 5, Col 41, "The type parameter 'a is not defined."
                    Error 39, Line 5, Col 50, Line 5, Col 52, "The type parameter 'b is not defined."
                ]

            /// Akin to:
            ///
            /// type R1<[<Measure>] 'a> = { A : int<'a> }
            /// type R2<'a> = { X : R1<'a> }
            [<Fact>]
            let ``Measure attribute on source, required on spread destination`` () =
                let src =
                    """
                    type R1<[<Measure>] 'a> = { A : int<'a> }
                    type R2<'a> = { ...R1<'a> }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 702, Line 3, Col 43, Line 3, Col 45, "Expected unit-of-measure parameter, not type parameter. Explicit unit-of-measure parameters must be marked with the [<Measure>] attribute.")

            [<Fact>]
            let ``Measure attribute on source, measure on spread destination, OK`` () =
                let src =
                    """
                    type R1<[<Measure>] 'a> = { A : int<'a> }
                    type R2<[<Measure>] 'b> = { ...R1<'b> }

                    type [<Measure>] m
                    type R3 = { ...R1<m> }

                    let _ : R1<m> = { A = 3<m> }
                    let _ : R2<m> = { A = 3<m> }
                    let _ : R3 = { A = 3<m> }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Akin to:
            ///
            /// type R1<'a when 'a : comparison> = { A : 'a }
            /// type R2<'a> = { X : R1<'a> }
            [<Fact>]
            let ``Constraint on source, required on spread destination`` () =
                let src =
                    """
                    type R1<'a when 'a : comparison> = { A : 'a }
                    type R2<'a> = { ...R1<'a> }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 1, Line 3, Col 40, Line 3, Col 46, "A type parameter is missing a constraint 'when 'a: comparison'")

            [<Fact>]
            let ``Constraint on source, required on spread destination, error if not compatible at usage`` () =
                let src =
                    """
                    type R1<'a when 'a : comparison> = { A : 'a list }
                    type R2<'a when 'a : comparison > = { ...R1<'a> }

                    let _ : R2<_> = { A = [obj ()] }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 193, Line 5, Col 44, Line 5, Col 50, "The type 'obj' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")

            [<Fact>]
            let ``Constraint on source, constraint on spread destination, compatible at usage, OK`` () =
                let src =
                    """
                    type R1<'a when 'a : comparison> = { A : 'a }
                    type R2<'a when 'a : comparison> = { ...R1<'a> }
                    type R3<'a when 'a : comparison> = { ...R1<'a list> }

                    let _ : R1<int> = { A = 3 }
                    let _ : R2<int> = { A = 3 }
                    let _ : R3<int list> = { A = [3] }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module NonRecordSource =
            [<Fact>]
            let ``{...class} → error`` () =
                let src =
                    """
                    type C () =
                        member _.A = 1
                        member _.B = 2

                    type R = { ...C }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 6, Col 32, Line 6, Col 36, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

            [<Fact>]
            let ``{...abstract_class} → error`` () =
                let src =
                    """
                    [<AbstractClass>]
                    type C () =
                        abstract A : int
                        default _.A = 1
                        abstract B : int
                        default _.B = 2

                    type R = { ...C }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 9, Col 32, Line 9, Col 36, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

            [<Fact>]
            let ``{...struct} → error`` () =
                let src =
                    """
                    [<Struct>]
                    type S =
                        member _.A = 1
                        member _.B = 2

                    type R = { ...S }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 7, Col 32, Line 7, Col 36, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

            [<Fact>]
            let ``{...interface} → error`` () =
                let src =
                    """
                    type IFace =
                        abstract A : int
                        abstract B : int

                    type R = { ...IFace }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 6, Col 32, Line 6, Col 40, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

            [<Fact>]
            let ``{...int} → error`` () =
                let src =
                    """
                    type R = { ...int }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 2, Col 32, Line 2, Col 38, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

            [<Fact>]
            let ``{...(int -> int)} → error`` () =
                let src =
                    """
                    type R = { ...(int -> int) }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3879, Line 2, Col 32, Line 2, Col 47, "The source type of a spread into a record type definition must itself be a nominal or anonymous record type.")

    module AnonymousRecordExpressionSpreads =
        module Algebra =
            /// No overlap, spread ⊕ field.
            [<Fact>]
            let ``{...{A,B},C} = {A,B} ⊕ {C} = {A,B,C}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}

                    let r2 : {| A : int ; B : int; C : int |} = {| ...r1; C = 3 |}
                    let r2' : {| A : int ; B : int; C : int |} = {| {||} with ...r1; C = 3 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, field ⊕ spread.
            [<Fact>]
            let ``{A,...{B,C}} = {A} ⊕ {B,C} = {A,B,C}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}

                    let r2 : {| A : int ; B : int; C : int |} = {| C = 3; ...r1 |}
                    let r2' : {| A : int ; B : int; C : int |} = {| {||} with C = 3; ...r1 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, spread ⊕ spread.
            [<Fact>]
            let ``{...{A,B},...{C,D}} = {A,B} ⊕ {C,D} = {A,B,C,D}`` () =
                let src =
                    """
                    let r1 = {| A = 1 ; B = 2 |}
                    let r2 = {| C = 3; D = 4 |}

                    let r3 : {| A : int ; B : int; C : int; D : int |} = {| ...r1; ...r2 |}
                    let r4 : {| A : int ; B : int; C : int; D : int |} = {| ...r2; ...r3 |}
                    let r3' : {| A : int ; B : int; C : int; D : int |} = {| {||} with ...r1; ...r2 |}
                    let r4' : {| A : int ; B : int; C : int; D : int |} = {| {||} with ...r2; ...r3 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward explicit duplicate field shadows field from spread.
            [<Fact>]
            let ``{...{A₀,B},A₁} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}

                    let r2 : {| A : string; B : int |} = {| ...r1; A = "A" |}
                    let r2' : {| A : string; B : int |} = {| {||} with ...r1; A = "A" |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward spread field shadows leftward spread field.
            [<Fact>]
            let ``{...{A₀,B},...{A₁}} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}
                    let r2 = {| A = "A" |}

                    let r3 : {| A : string; B : int |} = {| ...r1; ...r2 |}
                    let r4 : {| A : int; B : int |} = {| ...r2; ...r1 |}

                    let r3' : {| A : string; B : int |} = {| {||} with ...r1; ...r2 |}
                    let r4' : {| A : int; B : int |} = {| {||} with ...r2; ...r1 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward spread field shadows leftward explicit field with warning.
            [<Fact>]
            let ``{A₀,...{A₁,B}} = {A₀} ⊕ {A₁,B} = {A₁_warn,B,C}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}

                    let r2 : {| A : int; B : int |} = {| A = "A"; ...r1 |}
                    let r2' : {| A : int; B : int |} = {| {||} with A = "A"; ...r1 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Warning 3883, Line 4, Col 67, Line 4, Col 72, "Spread field 'A: int' shadows an explicitly declared field with the same name."
                    Warning 3883, Line 5, Col 78, Line 5, Col 83, "Spread field 'A: int' shadows an explicitly declared field with the same name."
                ]

            /// Explicit duplicate fields remain disallowed.
            [<Fact>]
            let ``{A₀,...{A₁,B},A₂} = {A₀} ⊕ {A₁,B} ⊕ {A₂} = {A₁_warn,B,A₂_error}`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = 2 |}

                    let r2 = {| A = "A"; ...r1; A = 3.14 |}
                    let r2' = {| {||} with A = "A"; ...r1; A = 3.14 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Warning 3883, Line 4, Col 42, Line 4, Col 47, "Spread field 'A: int' shadows an explicitly declared field with the same name."
                    Error 37, Line 4, Col 49, Line 4, Col 57, "Duplicate definition of field 'A'"
                    Error 3522, Line 5, Col 31, Line 5, Col 71, "The field 'A' appears multiple times in this record expression."
                ]

            [<Fact>]
            let ``No dupes allowed, multiple`` () =
                let src =
                    """
                    let r1 = {| A = 1; B = "B" |}
                    let r2 = {| A = 3m |}

                    let r3 = {| ...r2; A = "A"; ...r1; A = 3.14 |}
                    let r3' = {| {||} with ...r2; A = "A"; ...r1; A = 3.14 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Warning 3883, Line 5, Col 49, Line 5, Col 54, "Spread field 'A: int' shadows an explicitly declared field with the same name."
                    Error 37, Line 5, Col 56, Line 5, Col 64, "Duplicate definition of field 'A'"
                    Error 3522, Line 6, Col 31, Line 6, Col 78, "The field 'A' appears multiple times in this record expression."
                ]

            [<Fact>]
            let ``{...{A,B,C}}:{B} = {A,B,C} ∩ {B} = {B}`` () =
                let src =
                    """
                    let src = {| A = 1; B = "B"; C = 3m |}

                    let typedTarget : {| B : string |} = {| ...src |}
                    let typedTarget' : {| B : string |} = {| {||} with ...src |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``{...{}} = ∅ ⊕ ∅ = ∅`` () =
                let src =
                    """
                    module M

                    let r = {| ...{||} |}
                    let r' = {| {||} with ...{||} |}

                    if r <> {||} then failwith $"Expected {{||}} but got %A{r}."
                    if r' <> {||} then failwith $"Expected {{||}} but got %A{r'}."
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> asExe
                |> compileAndRun
                |> shouldSucceed

        module Accessibility =
            /// Fields should have the accessibility of the target type.
            /// A spread from less to more accessible is valid as long as the less accessible
            /// fields are accessible at the point of the spread.
            [<Fact>]
            let ``Accessibility comes from target`` () =
                let src =
                    """
                    let private r1 = {| A = 1; B = "B" |}

                    let public r2 : {| A : int; B : string |} = {| ...r1 |}
                    let public r2' : {| A : int; B : string |} = {| {||} with ...r1 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module Mutability =
            [<Fact>]
            let ``Mutability is _not_ brought over`` () =
                let src =
                    """
                    type R1 = { A : int; mutable B : string }
                    let r1 = { A = 1; B = "B" }

                    let r2 = {| ...r1 |}
                    r2.B <- "99"

                    let r2' = {| {||} with ...r1 |}
                    r2'.B <- "99"
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 799, Line 6, Col 21, Line 6, Col 25, "Invalid assignment"
                    Error 799, Line 9, Col 21, Line 9, Col 26, "Invalid assignment"
                ]

        module GenericTypeParameters =
            [<Fact>]
            let ``Single type parameter`` () =
                let src =
                    """
                    let f (x : 'a) =
                        let r1 : {| A : 'a; B : string |} = {| A = x; B = "B" |}
                        let r2 : {| X : 'a; Y : string |} = {| X = x; Y = "Y" |}

                        let r3 : {| A : 'a; B : string; X : 'a; Y : string |} = {| ...r1; ...r2 |}
                        let r3' : {| A : 'a; B : string; X : 'a; Y : string |} = {| {||} with ...r1; ...r2 |}
                        r3, r3'
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Multiple type parameters`` () =
                let src =
                    """
                    let r1 (x : 'a) = {| A = x; B = "B" |}
                    let r2 (x : 'a) = {| X = x; Y = "Y" |}

                    let r3 (x : 'a) (y : 'b) : {| A : 'a; B : string; X : 'b; Y : string |} = {| ...r1 x; ...r2 y |}
                    let r3' (x : 'a) (y : 'b) : {| A : 'a; B : string; X : 'b; Y : string |} = {| {||} with ...r1 x; ...r2 y |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Measure attribute on source, present on spread destination`` () =
                let src =
                    """
                    let r1 (r2 : {| A : int<'m> |}) : {| A : int<'m> |} = {| ...r2 |}
                    let r1' (r2 : {| A : int<'m> |}) : {| A : int<'m> |} = {| {||} with ...r2 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``Constraints kept`` () =
                let src =
                    """
                    let r1<'a when 'a : comparison> (r2 : {| A : 'a |}) : unit -> {| A : 'a |} = fun () -> {| ...r2 |}
                    let r1'<'a when 'a : comparison> (r2 : {| A : 'a |}) : unit -> {| A : 'a |} = fun () -> {| {||} with ...r2 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module NonRecordSource =
            [<Fact>]
            let ``{...class} → error`` () =
                let src =
                    """
                    type C () =
                        member _.A = 1
                        member _.B = 2

                    let r = {| ...C () |}
                    let r' = {| {||} with ...C () |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 6, Col 35, Line 6, Col 39, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 7, Col 43, Line 7, Col 50, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...abstract_class} → error`` () =
                let src =
                    """
                    [<AbstractClass>]
                    type C () =
                        abstract A : int
                        abstract B : int

                    let r =
                        {|
                            ...
                                { new C () with
                                    member _.A = 1
                                    member _.B = 2 }
                        |}

                    let r' =
                        {|
                            {||} with
                                ...
                                    { new C () with
                                        member _.A = 1
                                        member _.B = 2 }
                        |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 10, Col 33, Line 12, Col 53, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 18, Col 33, Line 21, Col 57, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...struct} → error`` () =
                let src =
                    """
                    [<Struct>]
                    type S =
                        member _.A = 1
                        member _.B = 2

                    let r = {| ...S () |}
                    let r' = {| {||} with ...S () |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 7, Col 35, Line 7, Col 39, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 8, Col 43, Line 8, Col 50, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...interface} → error`` () =
                let src =
                    """
                    type IFace =
                        abstract A : int
                        abstract B : int

                    let r =
                        {|
                            ...
                                { new IFace with
                                    member _.A = 1
                                    member _.B = 2 }
                        |}

                    let r' =
                        {|
                            {||} with
                                ...
                                    { new IFace with
                                        member _.A = 1
                                        member _.B = 2 }
                        |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 9, Col 33, Line 11, Col 53, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 17, Col 33, Line 20, Col 57, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...int} → error`` () =
                let src =
                    """
                    let r = {| ...0 |}
                    let r' = {| {||} with ...0 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 2, Col 35, Line 2, Col 36, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 3, Col 43, Line 3, Col 47, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...(int -> int)} → error`` () =
                let src =
                    """
                    let r = {| ...(fun x -> x + 1) |}
                    let r' = {| {||} with ...(fun x -> x + 1) |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 2, Col 35, Line 2, Col 51, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 3, Col 43, Line 3, Col 62, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

            [<Fact>]
            let ``{...int list} → error`` () =
                let src =
                    """
                    let r = {| ...[1..10] |}
                    let r' = {| {||} with ...[1..10] |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3881, Line 2, Col 35, Line 2, Col 42, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                    Error 3881, Line 3, Col 43, Line 3, Col 53, "The source expression of a spread into an anonymous record expression must have a nominal or anonymous record type."
                ]

        module MembersOtherThanRecordFields =
            [<Fact>]
            let ``Instance properties that are not record fields are ignored`` () =
                let src =
                    """
                    type R1 =
                        { A : int
                          B : string }
                        member this.Lol = string this.A + this.B

                    type R2 = { ...R1; C : string }

                    let r1 = { A = 3; B = "3"; C = "asdf" }
                    let r2 : {| A : int; B : string; C : string |} = {| ...r1 |}
                    let r2' : {| A : int; B : string; C : string |} = {| {||} with ...r1 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``All members other than record fields are ignored`` () =
                let src =
                    """
                    type R1 =
                        { A : int
                          B : int }
                        member this.Lol = this.A + this.B
                        member _.Ha () = ()
                        static member X = "3"
                        static member val Y = 42
                        static member Q () = ()

                    type R2 = { ...R1; C : string }

                    let r2 : R2 = { A = 3; B = 3; C = "asdf" }
                    let r3 : {| A : int; B : int; C : string |} = {| ...r2 |}
                    let r3' : {| A : int; B : int; C : string |} = {| {||} with ...r2 |}
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module Effects =
            [<Fact>]
            let ``Effects in spread sources are evaluated exactly once per spread, even if all fields are shadowed`` () =
                let src =
                    """
                    let effects = ResizeArray ()
                    let f () = effects.Add "f"; {| A = 0; B = 1 |}
                    let g () = effects.Add "g"; {| A = 2; B = 3 |}
                    let h () = effects.Add "h"; {| A = 99 |}
                    let r = {| ...g (); ...g (); ...h (); A = 100 |}
                    let r' = {| f () with ...g (); ...g (); ...h (); A = 100 |}

                    if r.A <> 100 then failwith $"Expected r.A = 100 but got %d{r.A}."
                    if r'.A <> 100 then failwith $"Expected r'.A = 100 but got %d{r'.A}."
                    match List.ofSeq effects with
                    | ["g"; "g"; "h"; "f"; "g"; "g"; "h"] -> ()
                    | unexpected -> failwith $"Expected [\"g\"; \"g\"; \"h\"; \"f\"; \"g\"; \"g\"; \"h\"] but got %A{unexpected}."
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> compileExeAndRun
                |> shouldSucceed

        module Conversions =
            ()

    module NominalRecordExpressionSpreads =
        module Algebra =
            /// No overlap, spread ⊕ field.
            [<Fact>]
            let ``{...{A,B},C} = {A,B} ⊕ {C} = {A,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { A : int; B : int; C : int }

                    let r1 = { A = 1; B = 2 }
                    let r2 = { ...r1; C = 3 }

                    let r1' = {| A = 1; B = 2 |}
                    let r2' = { ...r1; C = 3 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, field ⊕ spread.
            [<Fact>]
            let ``{A,...{B,C}} = {A} ⊕ {B,C} = {A,B,C}`` () =
                let src =
                    """
                    type R1 = { B : int; C : int }
                    type R2 = { A : int; B : int; C : int }

                    let r1 = { B = 1; C = 2 }
                    let r2 = { A = 3; ...r1 }

                    let r1' = {| B = 1; C = 2 |}
                    let r2' = { A = 3; ...r1 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// No overlap, spread ⊕ spread.
            [<Fact>]
            let ``{...{A,B},...{C,D}} = {A,B} ⊕ {C,D} = {A,B,C,D}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }
                    type R2 = { C : int; D : int }
                    type R3 = { A : int; B : int; C : int; D : int }

                    let r1 = { A = 1; B = 2 }
                    let r2 = { C = 3; D = 4 }
                    let r3 = { ...r1; ...r2 }
                    let r3' = { ...r2; ...r3 }

                    let r1' = {| A = 1; B = 2 |}
                    let r2' = {| C = 3; D = 4 |}
                    let r3'' = { ...r1; ...r2 }
                    let r3''' = { ...r2; ...r3 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            /// Rightward explicit duplicate field shadows field from spread.
            [<Fact>]
            let ``{...{A₀,B},A₁} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    module M

                    type R1 = { A : int; B : int }

                    let r1 = { A = 1; B = 2 }
                    let r1' = { ...r1; A = 99 }

                    if r1'.A <> 99 then failwith $"Expected r1'.A = 99 but got %A{r1'.A}."
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> asExe
                |> compileAndRun
                |> shouldSucceed

            /// Rightward spread field shadows leftward spread field.
            [<Fact>]
            let ``{...{A₀,B},...{A₁}} = {A₀,B} ⊕ {A₁} = {A₁,B,C}`` () =
                let src =
                    """
                    module M

                    type R1 = { A : int; B : int }

                    let r1 = { A = 1; B = 2 }
                    let r1' = { ...r1; ...{| A = 99 |} }

                    if r1'.A <> 99 then failwith $"Expected r1'.A = 99 but got %A{r1'.A}."
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> asExe
                |> compileAndRun
                |> shouldSucceed

            /// Rightward spread field shadows leftward explicit field with warning.
            [<Fact>]
            let ``{A₀,...{A₁,B}} = {A₀} ⊕ {A₁,B} = {A₁_warn,B,C}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }

                    let r1 = { A = 1; B = 2 }
                    let r1' = { A = 0; ...r1 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Warning 3883, Line 5, Col 40, Line 5, Col 45, "Spread field 'A: int' shadows an explicitly declared field with the same name.")

            /// Explicit duplicate fields remain disallowed.
            [<Fact>]
            let ``{A₀,...{A₁,B},A₂} = {A₀} ⊕ {A₁,B} ⊕ {A₂} = {A₁_warn,B,A₂_error}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int }

                    let r1 = { A = 1; B = 2; A = 3; ...{| A = 4 |}; A = 5 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 37, Line 4, Col 46, Line 4, Col 51, "Duplicate definition of field 'A'"
                    Warning 3883, Line 4, Col 53, Line 4, Col 67, "Spread field 'A: int' shadows an explicitly declared field with the same name."
                    Error 37, Line 4, Col 69, Line 4, Col 74, "Duplicate definition of field 'A'"
                    Error 668, Line 4, Col 69, Line 4, Col 70, "The field 'A' appears multiple times in this record expression or pattern"
                ]

            /// Extra fields are ignored.
            [<Fact>]
            let ``{...{A,B,C}}:{B} = {A,B,C} ∩ {B} = {B}`` () =
                let src =
                    """
                    type R1 = { A : int; B : int; C : int }
                    type R2 = { B : int }

                    let r1 = { A = 1; B = 2; C = 3 }
                    let r2 : R2 = { ...r1 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module Accessibility =
            /// Fields should have the accessibility of the target type.
            /// A spread from less to more accessible is valid as long as the less accessible
            /// fields are accessible at the point of the spread.
            [<Fact>]
            let ``Accessibility comes from target`` () =
                let src =
                    """
                    type private R1 = { A : int; B : string }
                    type public R2 = { ...R1 }

                    let private r1 = { A = 1; B = "2" }
                    let public r2 : R2 = { ...r1 }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

        module NonRecordSource =
            [<Fact>]
            let ``{...class} → error`` () =
                let src =
                    """
                    type C () =
                        member _.A = 1
                        member _.B = 2

                    type R = { A : int }

                    let r : R = { ...C () }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3880, Line 8, Col 35, Line 8, Col 42, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type."
                    Error 764, Line 8, Col 33, Line 8, Col 44, "No assignment given for field 'A' of type 'Test.R'"
                ]

            [<Fact>]
            let ``{...abstract_class} → error`` () =
                let src =
                    """
                    [<AbstractClass>]
                    type C () =
                        abstract A : int
                        abstract B : int

                    type R = { A : int }

                    let r : R =
                        {
                            ...
                                { new C () with
                                    member _.A = 1
                                    member _.B = 2 }
                        }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3880, Line 11, Col 29, Line 14, Col 53, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type."
                    Error 764, Line 10, Col 25, Line 15, Col 26, "No assignment given for field 'A' of type 'Test.R'"
                ]

            [<Fact>]
            let ``{...struct} → error`` () =
                let src =
                    """
                    [<Struct>]
                    type S =
                        member _.A = 1
                        member _.B = 2

                    type R = { A : int }

                    let r : R = { ...S () }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3880, Line 9, Col 35, Line 9, Col 42, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type."
                    Error 764, Line 9, Col 33, Line 9, Col 44, "No assignment given for field 'A' of type 'Test.R'"
                ]

            [<Fact>]
            let ``{...interface} → error`` () =
                let src =
                    """
                    type IFace =
                        abstract A : int
                        abstract B : int

                    type R = { A : int }

                    let r : R =
                        {
                            ...
                                { new IFace with
                                    member _.A = 1
                                    member _.B = 2 }
                        }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3880, Line 10, Col 29, Line 13, Col 53, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type."
                    Error 764, Line 9, Col 25, Line 14, Col 26, "No assignment given for field 'A' of type 'Test.R'"
                ]

            [<Fact>]
            let ``{...int} → error`` () =
                let src =
                    """
                    type R = { A : int }

                    let r : R = { ...int }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 3880, Line 4, Col 35, Line 4, Col 41, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type."
                    Error 764, Line 4, Col 33, Line 4, Col 43, "No assignment given for field 'A' of type 'Test.R'"
                ]

            [<Fact>]
            let ``{...(int -> int)} → error`` () =
                let src =
                    """
                    type R = { A : int }

                    let r = { ...(fun x -> x + 1) }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withSingleDiagnostic (Error 3880, Line 4, Col 31, Line 4, Col 50, "The source expression of a spread into a nominal record expression must have a nominal or anonymous record type.")

        module MembersOtherThanRecordFields =
            [<Fact>]
            let ``Instance properties that are not record fields are ignored`` () =
                let src =
                    """
                    type R1 =
                        { A : int
                          B : string }
                        member this.Lol = string this.A + this.B

                    type R2 = { ...R1; C : string }

                    let _ : R2 = { A = 3; B = "3"; C = "asdf" }
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldSucceed

            [<Fact>]
            let ``All members other than record fields are ignored`` () =
                let src =
                    """
                    type R1 =
                        { A : int
                          B : int }
                        member this.Lol = this.A + this.B
                        member _.Ha () = ()
                        static member X = "3"
                        static member val Y = 42
                        static member Q () = ()

                    type R2 = { ...R1; C : string }

                    let r2 : R2 = { A = 3; B = 3; C = "asdf" }
                    ignore r2.Lol   // Should not exist.
                    r2.Ha ()        // Should not exist.
                    ignore R2.Y     // Should not exist.
                    R2.Q ()         // Should not exist.
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> typecheck
                |> shouldFail
                |> withDiagnostics [
                    Error 39, Line 14, Col 31, Line 14, Col 34, "The type 'R2' does not define the field, constructor or member 'Lol'."
                    Error 39, Line 15, Col 24, Line 15, Col 26, "The type 'R2' does not define the field, constructor or member 'Ha'."
                    Error 39, Line 16, Col 31, Line 16, Col 32, "The type 'R2' does not define the field, constructor or member 'Y'."
                    Error 39, Line 17, Col 24, Line 17, Col 25, "The type 'R2' does not define the field, constructor or member 'Q'."
                ]

        module Effects =
            [<Fact>]
            let ``Effects in spread sources are evaluated exactly once per spread, even if all fields are shadowed`` () =
                let src =
                    """
                    type R = { A : int; B : int }

                    let effects = ResizeArray ()
                    let f () = effects.Add "f"; { A = 0; B = 1 }
                    let g () = effects.Add "g"; { A = 2; B = 3 }
                    let h () = effects.Add "h"; {| A = 99 |}
                    let r = { ...g (); ...g (); ...h (); A = 100 }
                    let r' = { f () with ...g (); ...g (); ...h (); A = 100 }

                    if r.A <> 100 then failwith $"Expected r.A = 100 but got %d{r.A}."
                    if r'.A <> 100 then failwith $"Expected r'.A = 100 but got %d{r'.A}."
                    match List.ofSeq effects with
                    | ["g"; "g"; "h"; "f"; "g"; "g"; "h"] -> ()
                    | unexpected -> failwith $"Expected [\"g\"; \"g\"; \"h\"; \"f\"; \"g\"; \"g\"; \"h\"] but got %A{unexpected}."
                    """

                FSharp src
                |> withLangVersion SupportedLangVersion
                |> compileExeAndRun
                |> shouldSucceed
