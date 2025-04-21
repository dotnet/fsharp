// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module UseBangBindingsVersion9 =
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang01.fs"|])>]
    let ``UseBangBindings - UseBang01_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1228, Line 43, Col 14, Line 43, Col 15, "'use!' bindings must be of the form 'use! <var> = <expr>'")
        ]

module UseBangBindingsPreview =
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang01.fs"|])>]
    let ``UseBangBindings - UseBang01_fs - Preview LangVersion`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
