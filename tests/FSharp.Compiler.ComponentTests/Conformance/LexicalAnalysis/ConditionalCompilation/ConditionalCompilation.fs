// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ConditionalCompilation =
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

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withDefines ["DEFINED"; "DEFINED1"; "DEFINED2"]
        |> withOptions ["--test:ErrorRanges"]
        |> withNoWarn 62
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("ConditionalCompilation01.fsx")>]
    [<FileInlineData("ExtendedIfGrammar.fsx")>]
    [<FileInlineData("FSharp01.fsx")>]
    [<FileInlineData("FSharp02.fsx")>]
    [<FileInlineData("InComment01.fsx")>]
    [<FileInlineData("InStringLiteral01.fsx")>]
    [<FileInlineData("InStringLiteral02.fsx")>]
    [<FileInlineData("InStringLiteral03.fsx")>]
    [<FileInlineData("Nested01.fsx")>]
    [<FileInlineData("Nested02.fsx")>]
    [<FileInlineData("OCaml01.fsx")>]
    [<Theory>]
    let ``ConditionalCompilation`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_MustBeIdent01.fsx")>]
    let ``E_MustBeIdent01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 3182, Line 6, Col 15, Line 6, Col 15, "Unexpected character '*' in preprocessor expression")
            (Error 3184, Line 6, Col 15, Line 6, Col 15, "Incomplete preprocessor expression")
        ]

    [<Theory; FileInlineData("E_MustBeIdent02.fsx")>]
    let ``E_MustBeIdent02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 39, Line 6, Col 14, Line 6, Col 17, "The type 'if_' is not defined.")
            (Error 39, Line 7, Col 14, Line 7, Col 20, "The type 'endif_' is not defined.")
        ]

    [<Theory; FileInlineData("E_UnmatchedEndif01.fsx")>]
    let ``E_UnmatchedEndif01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 5, Col 1, Line 5, Col 7, "#endif has no matching #if in implementation file")
        ]

    [<Theory; FileInlineData("E_UnmatchedIf02.fsx")>]
    let ``E_UnmatchedIf02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 513, Line 6, Col 1, Line 6, Col 14, "End of file in #if section begun at or after here")
        ]
