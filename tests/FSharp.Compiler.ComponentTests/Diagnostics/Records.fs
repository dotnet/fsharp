// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Diagnostics.Records

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``Warning emitted when record update syntax changes all fields``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateOk r = { r with F1 = 1 }
let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3560, Line 7, Col 20, Line 7, Col 46, "This copy-and-update record expression changes all fields of record type 'Records.R'. Consider using the record construction syntax instead.")
    ]