// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Noframework =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/noframework
    // Regression test for FSHARP1.0:5976
    // System.Func<...> is in System.Core.dll - testing that it's available by default

    [<Theory; FileInlineData("noframework01.fs")>]
    let ``noframework - noframework01_fs - default references`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("noframework01.fs")>]
    let ``noframework - noframework01_fs - fsi`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> compile
        |> shouldSucceed
        |> ignore

    // See FSHARP1.0:6181 - it is no longer an error to specify --noframework and not specify -r to mscorlib/fscore
    [<Theory; FileInlineData("noframework02.fs")>]
    let ``noframework - noframework02_fs - --noframework`` compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withOptions ["--noframework"]
        |> compile
        |> shouldSucceed
        |> ignore

