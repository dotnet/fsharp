// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MethodApplicationResolution =

    // SOURCE=UnitVsNoArgs.fs
    [<Theory; FileInlineData("UnitVsNoArgs.fs")>]
    let ``UnitVsNoArgs_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=UnitVsNoArgs02.fs
    [<Theory; FileInlineData("UnitVsNoArgs02.fs")>]
    let ``UnitVsNoArgs02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_OverloadedGenericArgs.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_OverloadedGenericArgs.fs")>]
    let ``E_OverloadedGenericArgs_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 504
        |> ignore

    // SOURCE=MultiExtensionMethods01.fs
    [<Theory; FileInlineData("MultiExtensionMethods01.fs")>]
    let ``MultiExtensionMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=ParamArrayToDelegate01.fs
    [<Theory; FileInlineData("ParamArrayToDelegate01.fs")>]
    let ``ParamArrayToDelegate01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
