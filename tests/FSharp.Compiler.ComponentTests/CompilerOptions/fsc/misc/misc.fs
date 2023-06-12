// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System
open System.IO

module help_options =

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt -?     2>&1" POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# -?
    [<Fact>]
    let ``Help - variant 1``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["-?"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt /? 2>&1" POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# --help 
    [<Fact>]
    let ``Help - variant 2``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["/?"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt --help 2>&1"     POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# /?
    [<Fact>]
    let ``Help - variant 3``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["--help"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed
