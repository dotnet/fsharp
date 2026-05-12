// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RequireQualifiedAccess =

    // SOURCE=OnRecord.fs
    [<Theory; FileInlineData("OnRecord.fs")>]
    let ``OnRecord_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_OnRecord.fs
    [<Theory; FileInlineData("E_OnRecord.fs")>]
    let ``E_OnRecord_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore

    // SOURCE=OnRecordVsUnion.fs
    [<Theory; FileInlineData("OnRecordVsUnion.fs")>]
    let ``OnRecordVsUnion_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=OnRecordVsUnion2.fs
    [<Theory; FileInlineData("OnRecordVsUnion2.fs")>]
    let ``OnRecordVsUnion2_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=OnDiscriminatedUnion.fs
    [<Theory; FileInlineData("OnDiscriminatedUnion.fs")>]
    let ``OnDiscriminatedUnion_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_OnDiscriminatedUnion.fs
    [<Theory; FileInlineData("E_OnDiscriminatedUnion.fs")>]
    let ``E_OnDiscriminatedUnion_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore

    // SOURCE=OnRecordVsUnion_NoRQA.fs
    [<Theory; FileInlineData("OnRecordVsUnion_NoRQA.fs")>]
    let ``OnRecordVsUnion_NoRQA_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=OnRecordVsUnion_NoRQA2.fs
    [<Theory; FileInlineData("OnRecordVsUnion_NoRQA2.fs")>]
    let ``OnRecordVsUnion_NoRQA2_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=OnUnionWithCaseOfSameName.fs
    [<Theory; FileInlineData("OnUnionWithCaseOfSameName.fs")>]
    let ``OnUnionWithCaseOfSameName_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=OnUnionWithCaseOfSameName2.fs
    [<Theory; FileInlineData("OnUnionWithCaseOfSameName2.fs")>]
    let ``OnUnionWithCaseOfSameName2_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> ignore
