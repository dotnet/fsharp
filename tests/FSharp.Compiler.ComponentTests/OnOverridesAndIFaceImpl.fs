// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

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
    let ``E_InterfaceImpl01.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    // SOURCE=E_OnOverrides01.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides01.fs"|])>]
    let ``E_OnOverrides01.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    // SOURCE=E_OnOverrides02.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides02.fs"|])>]
    let ``E_OnOverrides02.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    // SOURCE=E_OnOverrides03.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides03.fs"|])>]
    let ``E_OnOverrides03.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    // SOURCE=E_OnOverrides04.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides04.fs"|])>]
    let ``E_OnOverrides04.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    // SOURCE=E_OnOverrides05.fs SCFLAGS="--test:ErrorRanges"               # E_OnOverrides05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OnOverrides05.fs"|])>]
    let ``E_OnOverrides05.fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]
