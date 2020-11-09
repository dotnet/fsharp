// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ExceptionDefinitions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"EqualAndBoxing01.fs"|])>]
    let ``ExceptionDefinitions - EqualAndBoxing01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"CatchWOTypecheck01.fs"|])>]
    let ``ExceptionDefinitions - CatchWOTypecheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"AddMethsProps01.fs"|])>]
    let ``ExceptionDefinitions - AddMethsProps01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS0053" status="error">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_MustStartWithCap01.fs"|])>]
    let ``ExceptionDefinitions - E_MustStartWithCap01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0053
        |> withDiagnosticMessageMatches "Discriminated union cases and exception labels must be uppercase identifiers"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS0039" span="(11,36-11,61)" status="error">The type 'AssertionFailureException' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_AssertionFailureExn.fs"|])>]
    let ``ExceptionDefinitions - E_AssertionFailureExn.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The type 'AssertionFailureException' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS1133" span="(9,5-9,24)" status="error">No constructors are available for the type 'FSharpExn'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_InheritException.fs"|])>]
    let ``ExceptionDefinitions - E_InheritException.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1133
        |> withDiagnosticMessageMatches "No constructors are available for the type 'FSharpExn'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS3174" span="(13,7-13,9)" status="error">The exception 'AAA' does not have a field named 'V3'\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_ExnConstructorBadFieldName.fs"|])>]
    let ``ExceptionDefinitions - E_ExnConstructorBadFieldName.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3174
        |> withDiagnosticMessageMatches "The exception 'AAA' does not have a field named 'V3'\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS3176" span="(8,28-8,29)" status="error">Named field 'A' is used more than once\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_ExnFieldConflictingName.fs"|])>]
    let ``ExceptionDefinitions - E_ExnFieldConflictingName.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3176
        |> withDiagnosticMessageMatches "Named field 'A' is used more than once\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions)
    //<Expects id="FS3175" span="(13,16-13,18)" status="error">Union case/exception field 'V2' cannot be used more than once\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/ExceptionDefinitions", Includes=[|"E_FieldNameUsedMulti.fs"|])>]
    let ``ExceptionDefinitions - E_FieldNameUsedMulti.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3175
        |> withDiagnosticMessageMatches "Union case/exception field 'V2' cannot be used more than once\."

