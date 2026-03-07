// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module FunctionApplicationResolution =

    // SOURCE=E_FOFunction01.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; FileInlineData("E_FOFunction01.fs")>]
    let ``E_FOFunction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=InferGenericArgAsTuple01.fs
    [<Theory; FileInlineData("InferGenericArgAsTuple01.fs")>]
    let ``InferGenericArgAsTuple01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=InferGenericArgAsTuple02.fs
    [<Theory; FileInlineData("InferGenericArgAsTuple02.fs")>]
    let ``InferGenericArgAsTuple02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
