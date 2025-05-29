// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Comments =
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
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("DontEscapeCommentFromString01.fsx")>]
    [<FileInlineData("embeddedString001.fsx")>]
    [<FileInlineData("embeddedString002.fsx")>]
    [<FileInlineData("embeddedString003.fsx")>]
    [<FileInlineData("embeddedString004.fsx")>]
    [<FileInlineData("escapeCharsInComments001.fsx")>]
    [<FileInlineData("escapeCharsInComments002.fsx")>]
    [<FileInlineData("ocamlstyle001.fsx")>]
    [<FileInlineData("ocamlstyle002.fsx")>]
    [<FileInlineData("ocamlstyle_nested001.fsx")>]
    [<FileInlineData("ocamlstyle_nested002.fsx")>]
    [<FileInlineData("ocamlstyle_nested003.fsx")>]
    [<FileInlineData("ocamlstyle_nested004.fsx")>]
    [<FileInlineData("ocamlstyle_nested005.fsx")>]
    [<FileInlineData("star01.fsx")>]
    [<FileInlineData("star03.fsx")>]
    [<FileInlineData("XmlDocComments01.fsx")>]
    [<FileInlineData("star01.fsx")>]
    [<FileInlineData("star03.fsx")>]
    [<Theory>]
    let ``Comments - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_embeddedString005.fsx")>]
    let ``E_embeddedString005_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 517, Line 9, Col 1, Line 9, Col 3, "End of file in string embedded in comment begun at or before here")
        ]

    [<Theory; FileInlineData("E_IncompleteComment02.fsx")>]
    let ``E_IncompleteComment02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 516, Line 8, Col 1, Line 8, Col 3, "End of file in comment begun at or before here")
        ]

    [<Theory; FileInlineData("E_ocamlstyle_nested006.fsx")>]
    let ``E_ocamlstyle_nested006_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 58, Line 14, Col 1, Line 14, Col 1, """Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (9:1). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.""")
            (Error 10, Line 14, Col 1, Line 14, Col 1, "Incomplete structured construct at or before this point in binding")
            (Error 516, Line 9, Col 10, Line 9, Col 12, "End of file in comment begun at or before here")
        ]

    [<Theory; FileInlineData("E_ocamlstyle_nested007.fsx")>]
    let ``E_ocamlstyle_nested007_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 12, Col 47, Line 14, Col 18, "Unexpected string literal in binding. Expected incomplete structured construct at or before this point or other token.")
            (Error 3118, Line 10, Col 1, Line 10, Col 4, "Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.")
            (Error 3156, Line 14, Col 19, Line 14, Col 20, "Unexpected token '*' or incomplete expression")
            (Error 10, Line 14, Col 20, Line 14, Col 21, "Unexpected symbol ')' in implementation file")
        ]

    [<Theory; FileInlineData("E_star02.fsx")>]
    let ``E_star02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 4, Line 8, Col 5, "Unexpected symbol ')' in implementation file")
        ]

    [<Theory; FileInlineData("E_IncompleteComment01.fsx")>]
    let ``E_star02E_IncompleteComment01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 18, Col 1, Line 18, Col 1, "Incomplete structured construct at or before this point in binding")
            (Error 516, Line 9, Col 5, Line 9, Col 7, "End of file in comment begun at or before here")
        ]
