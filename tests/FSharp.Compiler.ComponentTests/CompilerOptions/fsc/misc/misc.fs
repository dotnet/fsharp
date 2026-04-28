// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System
open System.IO

module help_options =

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt -?     2>&1" POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# -?
    // Reset enableConsoleColoring before help tests to avoid global mutable state
    // pollution from concurrent tests (e.g., ``fsc --consolecolors switch``).
    [<Fact>]
    let ``Help - variant 1``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["--consolecolors+"; "-?"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt /? 2>&1" POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# --help 
    [<Fact>]
    let ``Help - variant 2``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["--consolecolors+"; "/?"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed

    // ReqENU	SOURCE=dummy.fsx COMPILE_ONLY=1  PRECMD="\$FSC_PIPE >help40.txt --help 2>&1"     POSTCMD="\$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help40.txt help40.437.1033.bsl"	# /?
    [<Fact>]
    let ``Help - variant 3``() =
        FSharp ""
        |> asExe
        |> withBufferWidth 120
        |> withOptions ["--consolecolors+"; "--help"]
        |> compile
        |> verifyOutputWithBaseline (Path.Combine(__SOURCE_DIRECTORY__, "compiler_help_output.bsl"))
        |> shouldSucceed
