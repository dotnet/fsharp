// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Shift =
    //let shouldFailWithDiagnostics expectedDiagnostics compilation =
    //    compilation
    //    |> asLibrary
    //    |> withOptions ["--test:ErrorRanges"]
    //    |> typecheck
    //    |> shouldFail
    //    |> withDiagnostics expectedDiagnostics
    //
    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("GenericsRightShift001.fsx")>]
    [<FileInlineData("GenericsRightShift002.fsx")>]
    [<Theory>]
    let ``Shift`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed
