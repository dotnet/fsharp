// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DispatchSlotChecking =

    // SOURCE="InferSlotType01.fs"
    [<Theory; FileInlineData("InferSlotType01.fs")>]
    let ``InferSlotType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
