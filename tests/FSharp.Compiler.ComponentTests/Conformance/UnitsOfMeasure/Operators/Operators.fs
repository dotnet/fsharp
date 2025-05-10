// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Operators =
    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("Operators_decimal_01.fsx")>]
    [<FileInlineData("Operators_float_01.fsx")>]
    [<FileInlineData("Operators_float32_01.fsx")>]
    [<Theory>]
    let ``Operators - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed
