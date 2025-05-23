// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicKeywords =
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

    [<FileInlineData("GreaterThanClosedCurly01.fsx")>]
    [<FileInlineData("GreaterThanClosedCurly02.fsx")>]
    [<FileInlineData("GreaterThanClosedCurly03.fsx")>]
    [<FileInlineData("GreaterThanClosedCurly04.fsx")>]
    [<FileInlineData("GreaterThanClosedCurly05.fsx")>]
    [<FileInlineData("GreaterThanClosedSquare01.fsx")>]
    [<Theory>]
    let ``SymbolicKeywords`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed
