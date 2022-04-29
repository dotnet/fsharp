// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module PermittedLocations =

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

    // SOURCE=E_accessibilityOnInterface.fs                SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface.fs"|])>]
    let ``E_accessibilityOnInterface_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 18, Col 5, Line 18, Col 62, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 561, Line 19, Col 5, Line 19, Col 62, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 561, Line 20, Col 5, Line 20, Col 62, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 10, Line 21, Col 14, Line 21, Col 20, "Unexpected keyword 'public' in member definition. Expected identifier, '(', '(*)' or other token.")
            (Error 561, Line 21, Col 14, Line 22, Col 62, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 10, Line 23, Col 14, Line 23, Col 22, "Unexpected keyword 'internal' in member definition. Expected identifier, '(', '(*)' or other token.")
        ]

    // SOURCE=E_accessibilityOnInterface01.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface01.fs"|])>]
    let ``E_accessibilityOnInterface01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 13, Col 5, Line 13, Col 67, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface02.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface02.fs"|])>]
    let ``E_accessibilityOnInterface02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 15, Col 5, Line 15, Col 68, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface03.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface03.fs"|])>]
    let ``E_accessibilityOnInterface03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 15, Col 5, Line 15, Col 69, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface04.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface04.fs"|])>]
    let ``E_accessibilityOnInterface04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 15, Col 14, Line 15, Col 20, "Unexpected keyword 'public' in member definition. Expected identifier, '(', '(*)' or other token.")
        ]

    // SOURCE=E_accessibilityOnInterface05.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface05.fs"|])>]
    let ``E_accessibilityOnInterface05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 15, Col 14, Line 15, Col 21, "Unexpected keyword 'private' in member definition. Expected identifier, '(', '(*)' or other token.")
        ]

    // SOURCE=E_accessibilityOnInterface06.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface06.fs"|])>]
    let ``E_accessibilityOnInterface06_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 15, Col 14, Line 15, Col 22, "Unexpected keyword 'internal' in member definition. Expected identifier, '(', '(*)' or other token.")
        ]

    // SOURCE=E_accessibilityOnRecords.fs                  SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnRecords.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnRecords.fs"|])>]
    let ``E_accessibilityOnRecords_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 575, Line 10, Col 21, Line 10, Col 41, "Accessibility modifiers are not permitted on record fields. Use 'type R = internal ...' or 'type R = private ...' to give an accessibility to the whole representation.")
            (Error 575, Line 11, Col 21, Line 11, Col 39, "Accessibility modifiers are not permitted on record fields. Use 'type R = internal ...' or 'type R = private ...' to give an accessibility to the whole representation.")
            (Error 575, Line 12, Col 21, Line 12, Col 37, "Accessibility modifiers are not permitted on record fields. Use 'type R = internal ...' or 'type R = private ...' to give an accessibility to the whole representation.")
        ]

    // SOURCE=E_accessibilityOnRecords02.fs                SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnRecords02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnRecords02.fs"|])>]
    let ``E_accessibilityOnRecords02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 575, Line 8, Col 13, Line 8, Col 28, "Accessibility modifiers are not permitted on record fields. Use 'type R = internal ...' or 'type R = private ...' to give an accessibility to the whole representation.")
        ]

    // SOURCE=E_orderingOfAccessibilityKeyword_let01.fs    SCFLAGS="--test:ErrorRanges"         # E_orderingOfAccessibilityKeyword_let01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_orderingOfAccessibilityKeyword_let01.fs"|])>]
    let ``E_orderingOfAccessibilityKeyword_let01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 11, Col 13, Line 11, Col 20, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Warning 58, Line 12, Col 23, Line 12, Col 26, "Possible incorrect indentation: this token is offside of context started at position (11:23). Try indenting this token further or using standard formatting conventions.")
            (Error 531, Line 12, Col 13, Line 12, Col 19, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Warning 58, Line 13, Col 23, Line 13, Col 26, "Possible incorrect indentation: this token is offside of context started at position (12:23). Try indenting this token further or using standard formatting conventions.")
            (Error 531, Line 13, Col 13, Line 13, Col 21, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
        ]

    // SOURCE=E_orderingOfAccessibilityKeyword_member01.fs SCFLAGS="--test:ErrorRanges"         # E_orderingOfAccessibilityKeyword_member01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_orderingOfAccessibilityKeyword_member01.fs"|])>]
    let ``E_orderingOfAccessibilityKeyword_member01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 8, Col 26, Line 8, Col 32, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
        ]

    // SOURCE=E_orderingOfAccessibilityKeyword_module01.fs SCFLAGS="--test:ErrorRanges"         # E_orderingOfAccessibilityKeyword_module01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_orderingOfAccessibilityKeyword_module01.fs"|])>]
    let ``E_orderingOfAccessibilityKeyword_module01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 6, Col 1, Line 6, Col 7, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
        ]

    // SOURCE=E_orderingOfAccessibilityKeyword_type01.fs   SCFLAGS="--test:ErrorRanges"         # E_orderingOfAccessibilityKeyword_type01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_orderingOfAccessibilityKeyword_type01.fs"|])>]
    let ``E_orderingOfAccessibilityKeyword_type01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 8, Col 13, Line 8, Col 20, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
        ]
