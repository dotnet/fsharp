// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

//# Sanity check - simply check that the option is valid
module crossoptimize =

    //  SOURCE=crossoptimize.fs SCFLAGS="--crossoptimize" 
    [<Fact>]
    let ``crossoptimize_fs --crossoptimize``() =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize"]
        |> compile
        |> shouldSucceed

    //  SOURCE=crossoptimize.fs SCFLAGS="--crossoptimize+" 
    [<Fact>]
    let ``crossoptimize_fs --crossoptimize+``() =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=crossoptimize.fs SCFLAGS="--crossoptimize-" 
    [<Fact>]
    let ``crossoptimize_fs --crossoptimize-``() =
        Fs """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize-"]
        |> compile
        |> shouldSucceed

    //  SOURCE=crossoptimize01.fs SCFLAGS="--crossoptimize"   FSIMODE=EXEC COMPILE_ONLY=1
    [<Fact>]
    let ``crossoptimize_fsx --crossoptimize``() =
        Fsx """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize"]
        |> compile
        |> shouldSucceed

    //  SOURCE=crossoptimize01.fs SCFLAGS="--crossoptimize+"  FSIMODE=EXEC COMPILE_ONLY=1
    [<Fact>]
    let ``crossoptimize_fsx --crossoptimize+``() =
        Fsx """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=crossoptimize01.fs SCFLAGS="--crossoptimize-"  FSIMODE=EXEC COMPILE_ONLY=1
    [<Fact>]
    let ``crossoptimize_fsx --crossoptimize-``() =
        Fsx """printfn "Hello, World!!!" """
        |> asExe
        |> withOptions ["--crossoptimize-"]
        |> compile
        |> shouldSucceed

