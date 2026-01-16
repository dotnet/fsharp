// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module HashLight =

    let private resourcePath =
        Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", "resources", "tests", "Conformance", "LexicalFiltering", "HashLight")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; FileInlineData("IndentationWithComputationExpression01.fs")>]
    let ``IndentationWithComputationExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions [ "--warnaserror+" ]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // FSB 1431, 'end' token ambiguity for interface/class
    [<Fact>]
    let ``MissingEndToken01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "MissingEndToken01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // FSB 2150, Offside rule for #light code should set offside to left of accessibility modifier if present
    [<Fact>]
    let ``OffsideAccessibility01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "OffsideAccessibility01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // FSB 2150, Offside rule error test
    [<Fact>]
    let ``W_OffsideAccessibility01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "W_OffsideAccessibility01.fs"))
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldFail
        |> withDiagnostics
            [
                (Error 58, Line 18, Col 5, Line 18, Col 8, "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (17:5). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.")
                (Error 3524, Line 18, Col 5, Line 18, Col 8, "Expecting expression")
                (Error 10, Line 20, Col 5, Line 20, Col 6, "Unexpected symbol '{' in member definition")
            ]
        |> ignore

    // Regression test for FSHARP1.0:1078 - #light is now the default
    // Original test had: //<Expects status="notin">#light</Expects> (negative assertion not easily testable)
    [<Fact>]
    let ``First_Non_Comment_Text01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "First_Non_Comment_Text01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // FSI test - #light is default in fsi.exe (FSIMODE=PIPE)
    // Original test: SOURCE=default_in_fsi01.fs COMPILE_ONLY=1 FSIMODE=PIPE
    [<Fact>]
    let ``default_in_fsi01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "default_in_fsi01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // #light is default in fsi.exe (FSIMODE=EXEC)
    // Original test: SOURCE=default_in_fsi02.fs COMPILE_ONLY=1 FSIMODE=EXEC
    [<Fact>]
    let ``default_in_fsi02_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "default_in_fsi02.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression for FSB 1616 - Verify error on unclosed let-block
    [<Fact>]
    let ``E_UnclosedLetBlock01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "E_UnclosedLetBlock01.fs"))
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics
            [
                (Error 588, Line 10, Col 5, Line 10, Col 8, "The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.")
            ]
        |> ignore

    // Regression from FSB 1829 - Verify error when spaces after let-binding
    [<Fact>]
    let ``E_SpacesAfterLetBinding_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "E_SpacesAfterLetBinding.fs"))
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics
            [
                (Error 10, Line 10, Col 6, Line 10, Col 13, "Unexpected identifier in binding. Expected incomplete structured construct at or before this point or other token.")
                (Error 3118, Line 9, Col 5, Line 9, Col 8, "Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.")
                (Error 10, Line 11, Col 5, Line 11, Col 12, "Incomplete structured construct at or before this point in binding. Expected incomplete structured construct at or before this point or other token.")
                (Error 10, Line 13, Col 1, Line 13, Col 5, "Incomplete structured construct at or before this point in implementation file")
            ]
        |> ignore

    // Regression test for FSHARP1.0:5399 - TABs are not allowed in F# code
    [<Fact>]
    let ``E_TABsNotAllowedIndentOff_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "E_TABsNotAllowedIndentOff.fs"))
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> compile
        |> shouldFail
        |> withDiagnostics
            [
                (Error 1161, Line 5, Col 1, Line 5, Col 2, "TABs are not allowed in F# code")
            ]
        |> ignore

    // Regression for FSHARP1.0:5933 - fix #light syntax for cast operators
    [<Fact>]
    let ``CastOperators01_fs`` () =
        FsFromPath (Path.Combine(resourcePath, "CastOperators01.fs"))
        |> asExe
        |> withOptions [ "--nowarn:988" ]
        |> compile
        |> shouldSucceed
        |> ignore
