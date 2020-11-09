// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures.NameResolution

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module RequireQualifiedAccess =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnRecord.fs"|])>]
    let ``RequireQualifiedAccess - OnRecord.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    //<Expects id="FS0039" status="error">The record label 'Field1' is not defined\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"E_OnRecord.fs"|])>]
    let ``RequireQualifiedAccess - E_OnRecord.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The record label 'Field1' is not defined\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnRecordVsUnion.fs"|])>]
    let ``RequireQualifiedAccess - OnRecordVsUnion.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnRecordVsUnion2.fs"|])>]
    let ``RequireQualifiedAccess - OnRecordVsUnion2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnDiscriminatedUnion.fs"|])>]
    let ``RequireQualifiedAccess - OnDiscriminatedUnion.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    //<Expects id="FS0039" status="error">The value or constructor 'B' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"E_OnDiscriminatedUnion.fs"|])>]
    let ``RequireQualifiedAccess - E_OnDiscriminatedUnion.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'B' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnRecordVsUnion_NoRQA.fs"|])>]
    let ``RequireQualifiedAccess - OnRecordVsUnion_NoRQA.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnRecordVsUnion_NoRQA2.fs"|])>]
    let ``RequireQualifiedAccess - OnRecordVsUnion_NoRQA2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess", Includes=[|"OnUnionWithCaseOfSameName.fs"|])>]
    let ``RequireQualifiedAccess - OnUnionWithCaseOfSameName.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

