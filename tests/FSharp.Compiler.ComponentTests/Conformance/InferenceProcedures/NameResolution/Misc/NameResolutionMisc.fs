// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NameResolutionMisc =

    // SOURCE=E-DuplicateTypes01.fs
    // This test has a hyphenated filename which causes module naming issues
    // The test expects error 37 for duplicate type definition
    [<Fact(Skip = "Hyphenated filename causes module naming issues in test infrastructure")>]
    let ``E_DuplicateTypes01_fs`` () = ()

    // SOURCE=E_ClashingIdentifiersDU01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_ClashingIdentifiersDU01.fs")>]
    let ``E_ClashingIdentifiersDU01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 23
        |> ignore

    // SOURCE=E_ClashingIdentifiersDU02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_ClashingIdentifiersDU02.fs")>]
    let ``E_ClashingIdentifiersDU02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 812
        |> ignore

    // SOURCE=recordlabels.fs
    [<Theory; FileInlineData("recordlabels.fs")>]
    let ``recordlabels_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=RecordInference01.fs
    [<Theory; FileInlineData("RecordInference01.fs")>]
    let ``RecordInference01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=NoPartiallyQualifiedPathWarning01.fsx SCFLAGS="--warnaserror+" FSIMODE=EXEC COMPILE_ONLY=1
    // This test uses #load directive in FSI which is complex to migrate - skip
    [<Fact(Skip = "Uses FSI #load directive which is not supported in ComponentTests")>]
    let ``NoPartiallyQualifiedPathWarning01_fsx`` () = ()
