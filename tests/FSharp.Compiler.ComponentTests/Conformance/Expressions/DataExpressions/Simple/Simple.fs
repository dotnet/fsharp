// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Simple =

    // SOURCE=ListSubsumption01.fs
    [<Theory; FileInlineData("ListSubsumption01.fs")>]
    let ``ListSubsumption01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
