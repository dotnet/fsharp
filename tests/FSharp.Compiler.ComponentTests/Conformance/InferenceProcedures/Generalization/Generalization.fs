// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Generalization =

    // SOURCE=GenGroup01.fs
    [<Theory; FileInlineData("GenGroup01.fs")>]
    let ``GenGroup01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=NoMoreValueRestriction01.fs
    [<Theory; FileInlineData("NoMoreValueRestriction01.fs")>]
    let ``NoMoreValueRestriction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_NoMoreValueRestriction01.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_NoMoreValueRestriction01.fs")>]
    let ``E_NoMoreValueRestriction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_DynamicTypeTestOverFreeArg01.fs SCFLAGS="--test:ErrorRanges"
    // This test expects warning 64 when type variable is constrained
    [<Theory; FileInlineData("E_DynamicTypeTestOverFreeArg01.fs")>]
    let ``E_DynamicTypeTestOverFreeArg01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withSingleDiagnostic (Warning 64, Line 8, Col 14, Line 8, Col 15, "This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near 'E_DynamicTypeTestOverFreeArg01.fs(8,13)-(8,14)' has been constrained to be type 'obj'.")

    // SOURCE=LessRestrictive01.fs
    [<Theory; FileInlineData("LessRestrictive01.fs")>]
    let ``LessRestrictive01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=LessRestrictive02.fs
    [<Theory; FileInlineData("LessRestrictive02.fs")>]
    let ``LessRestrictive02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=LessRestrictive03.fs
    // Skipped: Uses System.Configuration.SettingsPropertyCollection which is not available in .NET Core
    [<Theory(Skip = "Uses System.Configuration.SettingsPropertyCollection not available in .NET Core"); FileInlineData("LessRestrictive03.fs")>]
    let ``LessRestrictive03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    // SOURCE=TypeAnnotation01.fs
    [<Theory; FileInlineData("TypeAnnotation01.fs")>]
    let ``TypeAnnotation01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_GeneralizeMemberInGeneric01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_GeneralizeMemberInGeneric01.fs")>]
    let ``E_GeneralizeMemberInGeneric01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3068
        |> ignore

    // SOURCE=RecordProperty01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("RecordProperty01.fs")>]
    let ``RecordProperty01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3068
        |> ignore

    // SOURCE=PropertyConstraint01.fs
    [<Theory; FileInlineData("PropertyConstraint01.fs")>]
    let ``PropertyConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
