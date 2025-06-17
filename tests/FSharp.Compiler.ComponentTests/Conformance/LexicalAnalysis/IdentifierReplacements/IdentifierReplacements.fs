// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open System.IO

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IdentifierReplacements =
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

    [<FileInlineData("Line01.fsx")>]
    [<FileInlineData("Line02.fsx")>]
    [<FileInlineData("SourceFile01.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("Line01.fsx")>]
    [<FileInlineData("Line02.fsx")>]
    [<FileInlineData("SourceFile01.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsExeShouldSucceed
