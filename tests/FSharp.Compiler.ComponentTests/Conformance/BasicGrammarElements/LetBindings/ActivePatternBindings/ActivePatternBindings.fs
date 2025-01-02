// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module LetBindings_ActivePatternBindings =

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
    [<Theory; FileInlineData("SanityCheck.fs")>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--nowarn:52"]
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=parameterizedActivePattern.fs              # parameterizedActivePattern.fs
    [<Theory; FileInlineData("parameterizedActivePattern.fs")>]
    let ``parameterizedActivePattern_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=partialActivePattern.fs                    # partialActivePattern.fs
    [<Theory; FileInlineData("partialActivePattern.fs")>]
    let ``partialActivePattern1_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

