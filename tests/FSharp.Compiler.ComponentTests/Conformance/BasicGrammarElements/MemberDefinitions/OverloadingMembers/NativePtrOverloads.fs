// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.TypeEquivalence

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NativePtrOverloads =

    let verifyCompile compilation =
        compilation
        |> asLibrary
        |> withOptions ["--nowarn:9"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asLibrary
        |> withOptions ["--nowarn:9"]
        |> compileAndRun

    // Positive test: distinct native pointer element types should compile
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NativePtrOverloads01.fs"|])>]
    let ``NativePtrOverloads01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // Negative test: duplicate exact signatures should fail
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NativePtrOverloads02.fs"|])>]
    let ``NativePtrOverloads02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 6, Col 17, Line 6, Col 18, "Duplicate method. The method 'M' has the same name and signature as another method in type 'Q'.")
            (Error 438, Line 7, Col 17, Line 7, Col 18, "Duplicate method. The method 'M' has the same name and signature as another method in type 'Q'.")
        ]

    // Regression test: previously failing overloads should now compile
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NativePtrOverloads03.fs"|])>]
    let ``NativePtrOverloads03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // Negative test: erased differences via measures should still fail
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NativePtrOverloads04.fs"|])>]
    let ``NativePtrOverloads04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 7, Col 17, Line 7, Col 18, "Duplicate method. The method 'H' has the same name and signature as another method in type 'S'.")
            (Error 438, Line 8, Col 17, Line 8, Col 18, "Duplicate method. The method 'H' has the same name and signature as another method in type 'S'.")
        ]