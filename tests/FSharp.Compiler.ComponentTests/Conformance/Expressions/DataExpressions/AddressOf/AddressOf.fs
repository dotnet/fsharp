// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AddressOf =

    // SOURCE=addressof_local_unit.fsx SCFLAGS=-a
    [<Theory; FileInlineData("addressof_local_unit.fsx")>]
    let ``addressof_local_unit_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    // SOURCE=E_byrefvaluesnotpermitted001.fs - error FS0431
    [<Theory; FileInlineData("E_byrefvaluesnotpermitted001.fs")>]
    let ``E_byrefvaluesnotpermitted001_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 431
        |> withDiagnosticMessageMatches "byref"
