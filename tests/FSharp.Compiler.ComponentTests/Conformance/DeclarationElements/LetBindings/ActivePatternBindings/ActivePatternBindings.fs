// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ActivePatternBindings =

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

    //SOURCE=SanityCheck.fs SCFLAGS=""                  # SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:52"]
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=parameterizedActivePattern.fs              # parameterizedActivePattern.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"parameterizedActivePattern.fs"|])>]
    let ``parameterizedActivePattern_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=partialActivePattern.fs                    # partialActivePattern.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"partialActivePattern.fs"|])>]
    let ``partialActivePattern1_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

