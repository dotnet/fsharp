// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ConditionalCompilation =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    //<Expects id="FS3184" status="error">Incomplete preprocessor expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_MustBeIdent01.fs"|])>]
    let ``ConditionalCompilation - E_MustBeIdent01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3184
        |> withDiagnosticMessageMatches "Incomplete preprocessor expression"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    //<Expects id="FS0583" span="(8,8-8,9)" status="error">Unmatched '\('</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_MustBeIdent02.fs"|])>]
    let ``ConditionalCompilation - E_MustBeIdent02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0583
        |> withDiagnosticMessageMatches "Unmatched '\('"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    //<Expects id="FS0010" span="(5,1-5,7)" status="error">#endif has no matching #if in implementation file</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedEndif01.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedEndif01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "#endif has no matching #if in implementation file"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    //<Expects id="FS1169" span="(5,1-5,4)" status="error">#if directive should be immediately followed by an identifier</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedIf01.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedIf01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1169
        |> withDiagnosticMessageMatches "#if directive should be immediately followed by an identifier"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    //<Expects id="FS0513" status="error">End of file in #if section begun at or after here</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedIf02.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedIf02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0513
        |> withDiagnosticMessageMatches "End of file in #if section begun at or after here"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"ConditionalCompilation01.fs"|])>]
    let ``ConditionalCompilation - ConditionalCompilation01.fs - --define:THIS_IS_DEFINED`` compilation =
        compilation
        |> withOptions ["--define:THIS_IS_DEFINED"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"Nested01.fs"|])>]
    let ``ConditionalCompilation - Nested01.fs - --define:DEFINED1 --define:DEFINED2`` compilation =
        compilation
        |> withOptions ["--define:DEFINED1"; "--define:DEFINED2"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"Nested02.fs"|])>]
    let ``ConditionalCompilation - Nested02.fs - --define:DEFINED1 --define:DEFINED2`` compilation =
        compilation
        |> withOptions ["--define:DEFINED1"; "--define:DEFINED2"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral01.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral02.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral02.fs - --define:DEFINED`` compilation =
        compilation
        |> withOptions ["--define:DEFINED"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral03.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InComment01.fs"|])>]
    let ``ConditionalCompilation - InComment01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/ConditionalCompilation)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"ExtendedIfGrammar.fs"|])>]
    let ``ConditionalCompilation - ExtendedIfGrammar.fs - --define:DEFINED`` compilation =
        compilation
        |> withOptions ["--define:DEFINED"]
        |> typecheck
        |> shouldSucceed

