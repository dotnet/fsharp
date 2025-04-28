﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang02.fs"|])>]
    let ``UseBangBindings - UseBang02_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1228, Line 40, Col 14, Line 40, Col 15, "'use!' bindings must be of the form 'use! <var> = <expr>'")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang03.fs"|])>]
    let ``UseBangBindings - UseBang03_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1228, Line 46, Col 14, Line 46, Col 15, "'use!' bindings must be of the form 'use! <var> = <expr>'")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang04.fs"|])>]
    let ``UseBangBindings - UseBang04_fs - Current LangVersion`` compilation =
        compilation
        |> asFsx
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1228, Line 47, Col 14, Line 47, Col 15, "'use!' bindings must be of the form 'use! <var> = <expr>'")
        ]

module UseBangBindingsPreview =
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang01.fs"|])>]
    let ``UseBangBindings - UseBang01_fs - Preview LangVersion`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang02.fs"|])>]
    let ``UseBangBindings - UseBang02_fs - Preview LangVersion`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang03.fs"|])>]
    let ``UseBangBindings - UseBang03_fs - Preview LangVersion`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseBang04.fs"|])>]
    let ``UseBangBindings - UseBang04_fs - Preview LangVersion`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

