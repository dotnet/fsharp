// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ConditionalCompilation =

    // SOURCE: E_MustBeIdent01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_MustBeIdent01.fs"|])>]
    let ``ConditionalCompilation - E_MustBeIdent01_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3182
        |> ignore

    // SOURCE: E_MustBeIdent02.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_MustBeIdent02.fs"|])>]
    let ``ConditionalCompilation - E_MustBeIdent02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // SOURCE: E_UnmatchedEndif01.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedEndif01.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedEndif01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // SOURCE: E_UnmatchedIf01.fs SCFLAGS: --test:ErrorRanges
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedIf01.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedIf01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1169
        |> ignore

    // SOURCE: E_UnmatchedIf02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"E_UnmatchedIf02.fs"|])>]
    let ``ConditionalCompilation - E_UnmatchedIf02_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0513
        |> ignore

    // SOURCE: ConditionalCompilation01.fs SCFLAGS: --define:THIS_IS_DEFINED
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"ConditionalCompilation01.fs"|])>]
    let ``ConditionalCompilation - ConditionalCompilation01_fs`` compilation =
        compilation
        |> withOptions ["--define:THIS_IS_DEFINED"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: Nested01.fs SCFLAGS: --define:DEFINED1 --define:DEFINED2
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"Nested01.fs"|])>]
    let ``ConditionalCompilation - Nested01_fs`` compilation =
        compilation
        |> withOptions ["--define:DEFINED1"; "--define:DEFINED2"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: Nested02.fs SCFLAGS: --define:DEFINED1 --define:DEFINED2
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"Nested02.fs"|])>]
    let ``ConditionalCompilation - Nested02_fs`` compilation =
        compilation
        |> withOptions ["--define:DEFINED1"; "--define:DEFINED2"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: InStringLiteral01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral01.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: InStringLiteral02.fs SCFLAGS: --define:DEFINED
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral02.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral02_fs`` compilation =
        compilation
        |> withOptions ["--define:DEFINED"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: InStringLiteral03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InStringLiteral03.fs"|])>]
    let ``ConditionalCompilation - InStringLiteral03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: InComment01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"InComment01.fs"|])>]
    let ``ConditionalCompilation - InComment01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: ExtendedIfGrammar.fs SCFLAGS: --define:DEFINED
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/ConditionalCompilation", Includes=[|"ExtendedIfGrammar.fs"|])>]
    let ``ConditionalCompilation - ExtendedIfGrammar_fs`` compilation =
        compilation
        |> withOptions ["--define:DEFINED"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
