// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open System

open Xunit
open FSharp.Test.Compiler


//# Sanity check - simply check that the option is valid
module crossoptimize =

    //  SOURCE=crossoptimize.fs SCFLAGS="--crossoptimize" 
    [<InlineData("--crossoptimize")>]
    [<InlineData("--crossoptimize+")>]
    [<InlineData("--crossoptimize-")>]
    [<Theory>]
    let ``crossoptimize_flag_fs`` option =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions  (if String.IsNullOrWhiteSpace option then [] else [option])
        |> compile
        |> shouldSucceed

    [<InlineData("--crossoptimize")>]
    [<InlineData("--crossoptimize+")>]
    [<InlineData("--crossoptimize-")>]
    [<Theory>]
    let ``crossoptimize_flag_fsx`` option =
        Fsx """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions (if String.IsNullOrWhiteSpace option then [] else [option])
        |> compile
        |> shouldSucceed
