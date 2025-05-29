// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open System.IO

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Directives =
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

    [<FileInlineData("dummy.fsx")>]
    [<FileInlineData("multiple_nowarn01.fsx")>]
    [<FileInlineData("multiple_nowarn02.fsx")>]
    [<FileInlineData("multiple_nowarn_many.fsx")>]
    [<FileInlineData("multiple_nowarn_one.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<Fact>]
    let ``load_script_with_multiple_nowarn01`` () =

        Fsx $"""#load @"{Path.Combine(__SOURCE_DIRECTORY__, "multiple_nowarn01.fsx")}" """
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("dummy.fsx")>]
    [<FileInlineData("multiple_nowarn01.fsx")>]
    [<FileInlineData("multiple_nowarn02.fsx")>]
    [<FileInlineData("multiple_nowarn_many.fsx")>]
    [<FileInlineData("multiple_nowarn_one.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsExeShouldSucceed

    [<Theory; FileInlineData("E_R_01.fsx")>]
    let ``E_R_01_fsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed        /// Should be succeed with diagnostics
        |> withDiagnostics [
            (Warning 3353, Line 5, Col 1, Line 5, Col 14, "Invalid directive '#R somefile'")
        ]

    [<Theory; FileInlineData("E_ShebangLocation.fsx")>]
    let ``E_ShebangLocation_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 0, Line 3, Col 1, Line 3, Col 4, "#! may only appear as the first line at the start of a file.")
        ]
