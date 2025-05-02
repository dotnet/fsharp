// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module UseBindings =

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBindingDiscard01.fs"|])>]
    let ``UseBindings - UseBindingDiscard01_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> withLangVersion60
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBindingDiscard01.fs"|])>]
    let ``UseBindings - UseBindingDiscard01_fs - Bad LangVersion`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--langversion:5.0"]
        |> compile
        |> shouldFail
        |> withErrorCode 3350
        |> withDiagnosticMessageMatches "Feature 'discard pattern in use binding' is not available.*"

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBindingDiscard02.fs"|])>]
    let ``Dispose called for discarded value of use binding`` compilation =
        compilation
        |> asExe
        |> withLangVersion60
        |> compileAndRun
        |> shouldSucceed