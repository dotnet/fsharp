// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ExplicitTypeParameters =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:3370"]
        |> compileAndRun

    //SOURCE=SanityCheck.fs                     # SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=SanityCheck2.fs                    # SanityCheck2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck2.fs"|])>]
    let ``SanityCheck2_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=W_TypeParamsWhenNotNeeded.fs       # W_TypeParamsWhenNotNeeded.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_TypeParamsWhenNotNeeded.fs"|])>]
    let ``W_TypeParamsWhenNotNeeded_fs_warning`` compilation =
        compilation
        |> asFsx
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 686, Line 10, Col 4, Line 10, Col 5, "The method or function 'f' should not be given explicit type argument(s) because it does not declare its type parameters explicitly")
        ]

    //SOURCE=W_TypeParamsWhenNotNeeded.fs       # W_TypeParamsWhenNotNeeded.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_TypeParamsWhenNotNeeded.fs"|])>]
    let ``W_TypeParamsWhenNotNeeded_fs_execute`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--nowarn:686"]
        |> verifyCompileAndRun
        |> shouldSucceed

