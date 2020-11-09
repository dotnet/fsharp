// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module OnOverridesAndIFaceImpl =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(29,40-29,42)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_InterfaceImpl01.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_InterfaceImpl01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(23,35-23,36)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_OnOverrides01.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_OnOverrides01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(26,28-26,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_OnOverrides02.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_OnOverrides02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(27,28-27,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_OnOverrides03.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_OnOverrides03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(26,28-26,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_OnOverrides04.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_OnOverrides04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl)
    //<Expects id="FS0941" span="(19,28-19,29)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl", Includes=[|"E_OnOverrides05.fs"|])>]
    let ``OnOverridesAndIFaceImpl - E_OnOverrides05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0941
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on overrides or interface implementations"

