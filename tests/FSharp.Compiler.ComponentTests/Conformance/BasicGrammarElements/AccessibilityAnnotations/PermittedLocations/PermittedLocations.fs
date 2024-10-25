// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AccessibilityAnnotations_PermittedLocations =

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
            (Error 531, Line 18, Col 5, Line 18, Col 11, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 18, Col 5, Line 18, Col 11, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 531, Line 19, Col 5, Line 19, Col 12, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 19, Col 5, Line 19, Col 12, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 531, Line 20, Col 5, Line 20, Col 13, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 20, Col 5, Line 20, Col 13, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 561, Line 21, Col 14, Line 21, Col 20, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 561, Line 22, Col 14, Line 22, Col 21, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 561, Line 23, Col 14, Line 23, Col 22, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface01.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface01.fs"|])>]
    let ``E_accessibilityOnInterface01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 13, Col 5, Line 13, Col 11, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 13, Col 5, Line 13, Col 11, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface02.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface02.fs"|])>]
    let ``E_accessibilityOnInterface02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 15, Col 5, Line 15, Col 12, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 15, Col 5, Line 15, Col 12, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface03.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface03.fs"|])>]
    let ``E_accessibilityOnInterface03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 15, Col 5, Line 15, Col 13, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 561, Line 15, Col 5, Line 15, Col 13, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface04.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface04.fs"|])>]
    let ``E_accessibilityOnInterface04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 15, Col 14, Line 15, Col 20, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface05.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface05.fs"|])>]
    let ``E_accessibilityOnInterface05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 15, Col 14, Line 15, Col 21, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]

    // SOURCE=E_accessibilityOnInterface06.fs              SCFLAGS="--test:ErrorRanges"         # E_accessibilityOnInterface06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_accessibilityOnInterface06.fs"|])>]
    let ``E_accessibilityOnInterface06_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 561, Line 15, Col 14, Line 15, Col 22, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
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
            (Error 58, Line 12, Col 23, Line 12, Col 26, "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (11:23). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.")
            (Error 531, Line 12, Col 13, Line 12, Col 19, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 58, Line 13, Col 23, Line 13, Col 26, "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (12:23). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.")
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

    [<Fact>]
    let ``Signature File Test: abstract member cannot have access modifiers`` () =
        Fsi """module Program

type A =
    abstract internal B: int ->int
    abstract member internal E: int ->int
    abstract member C: int with internal get, private set
    abstract internal D: int with get, set
    static abstract internal B2: int ->int
    static abstract member internal E2: int ->int
    static abstract member C2: int with internal get, private set
    static abstract internal D2: int with get, set""" 
        |> withOptions ["--nowarn:3535"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 0561, Line 4, Col 14, Line 4, Col 22, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 5, Col 21, Line 5, Col 29, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 6, Col 33, Line 6, Col 41, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 6, Col 47, Line 6, Col 54, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 7, Col 14, Line 7, Col 22, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 8, Col 21, Line 8, Col 29, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 9, Col 28, Line 9, Col 36, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 10, Col 41, Line 10, Col 49, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 10, Col 55, Line 10, Col 62, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
            (Error 0561, Line 11, Col 21, Line 11, Col 29, "Accessibility modifiers are not allowed on this member. Abstract slots always have the same visibility as the enclosing type.")
        ]
