// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open System.IO

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IdentifiersAndKeywords =

    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let compileAndRunAsFsxShouldSucceed compilation =
        compilation
        |> asFsx
        |> withNoWarn 988
        |> runFsi
        |> shouldSucceed

    let compileAndRunAsExeShouldSucceed compilation =
        compilation
        |> withNoWarn 988
        |> asFs
        |> compileExeAndRun
        |> shouldSucceed

    [<FileInlineData("backtickmoduleandtypenames.fsx")>]
    [<FileInlineData("ValidIdentifier01.fsx")>]
    [<FileInlineData("ValidIdentifier02.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("Line02.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsExeShouldSucceed

    [<FileInlineData("Line02.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsExeShouldSucceed

    [<Theory; FileInlineData("E_Line01.fsx")>]
    let ``E_InvalidIdentifier01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1156, Line 7, Col 5, Line 7, Col 19, "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int/int32), 1u (uint/uint32), 1L (int64), 1UL (uint64), 1s (int16), 1us (uint16), 1y (int8/sbyte), 1uy (uint8/byte), 1.0 (float/double), 1.0f (float32/single), 1.0m (decimal), 1I (bigint).")
        ]
