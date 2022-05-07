// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OnOverridesAndIFaceImpl =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=E_InterfaceImpl01.fs SCFLAGS="--test:ErrorRanges"             # E_InterfaceImpl01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InterfaceImpl01.fs"|])>]
    let ``E_InterfaceImpl01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 19, Col 38, Line 19, Col 40, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 24, Col 39, Line 24, Col 41, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 29, Col 40, Line 29, Col 42, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]

    // SOURCE=E_OnOverrides01.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides01.fs"|])>]
    let ``E_OnOverrides01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 13, Col 33, Line 13, Col 34, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 18, Col 34, Line 18, Col 35, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 23, Col 35, Line 23, Col 36, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]

    // SOURCE=E_OnOverrides02.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides02.fs"|])>]
    let ``E_OnOverrides02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 14, Col 28, Line 14, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 20, Col 28, Line 20, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 26, Col 28, Line 26, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]

    // SOURCE=E_OnOverrides03.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides03.fs"|])>]
    let ``E_OnOverrides03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 15, Col 28, Line 15, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 21, Col 28, Line 21, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 27, Col 28, Line 27, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]

    // SOURCE=E_OnOverrides04.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides04.fs"|])>]
    let ``E_OnOverrides04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 14, Col 28, Line 14, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 20, Col 28, Line 20, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 941, Line 26, Col 28, Line 26, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]

    // SOURCE=E_OnOverrides05.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides05.fs"|])>]
    let ``E_OnOverrides05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 19, Col 28, Line 19, Col 29, "Accessibility modifiers are not permitted on overrides or interface implementations")
        ]
