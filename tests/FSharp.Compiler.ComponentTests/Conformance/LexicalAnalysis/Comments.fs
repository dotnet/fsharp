// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Comments =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"star01.fs"|])>]
    let ``Comments - star01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_star02.fs"|])>]
    let ``Comments - E_star02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches @"Unexpected symbol '\)' in implementation file$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Comments)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"star03.fs"|])>]
    let ``Comments - star03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // OCaml-style comment tests
    // Regression test for FSHARP1.0:1561 - Verify that (**) does not leave the lexer in a comment state
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle001.fs"|])>]
    let ``Comments - ocamlstyle001_fs - empty OCaml comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // FSB 2008, Parse error on comment "(**"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle002.fs"|])>]
    let ``Comments - ocamlstyle002_fs - OCaml comment variations`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // Nested OCaml-style comment tests
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested001.fs"|])>]
    let ``Comments - ocamlstyle_nested001_fs - nested empty comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested002.fs"|])>]
    let ``Comments - ocamlstyle_nested002_fs - nested star`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested003.fs"|])>]
    let ``Comments - ocamlstyle_nested003_fs - nested comment block`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested004.fs"|])>]
    let ``Comments - ocamlstyle_nested004_fs - nested double star comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"ocamlstyle_nested005.fs"|])>]
    let ``Comments - ocamlstyle_nested005_fs - nested triple star`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // Error tests for malformed nested OCaml-style comments
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested006.fs"|])>]
    let ``Comments - E_ocamlstyle_nested006_fs - unclosed comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here"
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_ocamlstyle_nested007.fs"|])>]
    let ``Comments - E_ocamlstyle_nested007_fs - malformed nested comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // Embedded string tests - strings embedded within comments
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString001.fs"|])>]
    let ``Comments - embeddedString001_fs - invalid escape in comment string`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString002.fs"|])>]
    let ``Comments - embeddedString002_fs - comment end token in string`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString003.fs"|])>]
    let ``Comments - embeddedString003_fs - legitimate escape in string`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"embeddedString004.fs"|])>]
    let ``Comments - embeddedString004_fs - backslash in string`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_embeddedString005.fs"|])>]
    let ``Comments - E_embeddedString005_fs - malformed embedded string`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0517
        |> withDiagnosticMessageMatches "End of file in string embedded in comment"
        |> ignore

    // Escape characters in comments tests
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments001.fs"|])>]
    let ``Comments - escapeCharsInComments001_fs - escape chars in quotes`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"escapeCharsInComments002.fs"|])>]
    let ``Comments - escapeCharsInComments002_fs - escape chars in verbatim strings`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // Incomplete comment error tests
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment01.fs"|])>]
    let ``Comments - E_IncompleteComment01_fs - incomplete comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here"
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"E_IncompleteComment02.fs"|])>]
    let ``Comments - E_IncompleteComment02_fs - deeply nested incomplete comment`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0516
        |> withDiagnosticMessageMatches "End of file in comment begun at or before here"
        |> ignore

    // XML doc comments test
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"XmlDocComments01.fs"|])>]
    let ``Comments - XmlDocComments01_fs - XML doc comment syntax`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

    // Comment end token within string literal
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Comments", Includes=[|"DontEscapeCommentFromString01.fs"|])>]
    let ``Comments - DontEscapeCommentFromString01_fs - comment end in string literal`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore
