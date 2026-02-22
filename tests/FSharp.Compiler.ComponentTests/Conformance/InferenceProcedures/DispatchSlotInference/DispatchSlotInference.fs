// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DispatchSlotInference =

    // SOURCE=GenInterfaceWGenMethods01.fs
    [<Theory; FileInlineData("GenInterfaceWGenMethods01.fs")>]
    let ``GenInterfaceWGenMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_GenInterfaceWGenMethods01.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_GenInterfaceWGenMethods01.fs")>]
    let ``E_GenInterfaceWGenMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 30
        |> ignore

    // SOURCE=E_MoreThanOneDispatchSlotMatch01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; FileInlineData("E_MoreThanOneDispatchSlotMatch01.fs")>]
    let ``E_MoreThanOneDispatchSlotMatch01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 361
        |> ignore
