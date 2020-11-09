// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module PermittedLocations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0575" span="(12,21-12,37)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnRecords.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnRecords.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0575
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0575" span="(8,13-8,28)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnRecords02.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnRecords02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0575
        |> withDiagnosticMessageMatches "Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0531" span="(13,13-13,21)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_orderingOfAccessibilityKeyword_let01.fs"|])>]
    let ``PermittedLocations - E_orderingOfAccessibilityKeyword_let01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0531
        |> withDiagnosticMessageMatches "Accessibility modifiers should come immediately prior to the identifier naming a construct"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0531" span="(8,26-8,32)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_orderingOfAccessibilityKeyword_member01.fs"|])>]
    let ``PermittedLocations - E_orderingOfAccessibilityKeyword_member01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0531
        |> withDiagnosticMessageMatches "Accessibility modifiers should come immediately prior to the identifier naming a construct"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0531" span="(6,1-6,7)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_orderingOfAccessibilityKeyword_module01.fs"|])>]
    let ``PermittedLocations - E_orderingOfAccessibilityKeyword_module01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0531
        |> withDiagnosticMessageMatches "Accessibility modifiers should come immediately prior to the identifier naming a construct"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects id="FS0531" span="(8,13-8,20)" status="error">Accessibility modifiers should come immediately prior to the identifier naming a construct</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_orderingOfAccessibilityKeyword_type01.fs"|])>]
    let ``PermittedLocations - E_orderingOfAccessibilityKeyword_type01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0531
        |> withDiagnosticMessageMatches "Accessibility modifiers should come immediately prior to the identifier naming a construct"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" span="(13,5-13,67)" id="FS0561">Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface01.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0561
        |> withDiagnosticMessageMatches "Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" id="FS0561" span="(15,5-15,68)">Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface02.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0561
        |> withDiagnosticMessageMatches "Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" id="FS0561" span="(15,5-15,69)">Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface03.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0561
        |> withDiagnosticMessageMatches "Accessibility modifiers are not allowed on this member\. Abstract slots always have the same visibility as the enclosing type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" span="(15,14-15,20)" id="FS0010">Unexpected keyword 'public' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface04.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'public' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" span="(15,14-15,21)" id="FS0010">Unexpected keyword 'private' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface05.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'private' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations)
    //<Expects status="error" span="(15,14-15,22)" id="FS0010">Unexpected keyword 'internal' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/AccessibilityAnnotations/PermittedLocations", Includes=[|"E_accessibilityOnInterface06.fs"|])>]
    let ``PermittedLocations - E_accessibilityOnInterface06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected keyword 'internal' in member definition\. Expected identifier, '\(', '\(\*\)' or other token\.$"

