// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Diagnostics.Records

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``Warning emitted when record update syntax changes all fields in lang preview``() =
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

[<Fact>]
let ``Warning not emitted when record update syntax changes all fields in lang70``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion70
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Warning not emitted when record update syntax changes all fields when disabled manually in lang preview``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersionPreview
    |> withOptions ["--nowarn:3560"]
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Warning emitted when record update syntax changes all fields when enabled manually in lang70``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion70
    |> withOptions ["--warnon:FS3560"]
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3560, Line 6, Col 20, Line 6, Col 46, "This copy-and-update record expression changes all fields of record type 'Records.R'. Consider using the record construction syntax instead.")
    ]