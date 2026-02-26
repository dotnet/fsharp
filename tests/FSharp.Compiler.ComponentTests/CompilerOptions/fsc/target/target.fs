// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Target =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/target
    // Error cases for target option

    // error01.fs: Unrecognized option: '--a'
    [<Theory; FileInlineData("error01.fs")>]
    let ``target - error01_fs - --a`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--a"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--a'"
        |> ignore

    // error02.fs: The file extension of '/a' is not recognized
    [<Theory; FileInlineData("error02.fs")>]
    let ``target - error02_fs - //a`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["//a"]
        |> compile
        |> shouldFail
        |> withErrorCode 226
        |> withDiagnosticMessageMatches @"is not recognized.+Source files must have extension"
        |> ignore
