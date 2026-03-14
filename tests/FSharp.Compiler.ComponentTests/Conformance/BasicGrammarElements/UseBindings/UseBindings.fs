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
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBindingDiscard02.fs"|])>]
    let ``Dispose called for discarded value of use binding`` compilation =
        compilation
        |> asExe
        |> withLangVersion80
        |> compileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBindingDiscard03.fs"|])>]
    let ``UseBindings - UseBindingDiscard03_fs - Current LangVersion`` compilation =
        compilation
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBinding01.fs"|])>]
    let ``UseBindings - UseBinding01_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBinding02.fs"|])>]
    let ``UseBindings - UseBinding02_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldSucceed
