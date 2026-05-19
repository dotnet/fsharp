// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Regression tests for https://github.com/dotnet/fsharp/issues/19576
module ``Nowarn for command-line option warnings`` =

    // -- Baseline: warnings ARE emitted without --nowarn ----------------------------

    [<Theory>]
    [<InlineData(75, "--extraoptimizationloops:1")>]
    [<InlineData(75, "--typedtree")>]
    [<InlineData(1063, "--test:NoSuchTestFlag")>]
    let ``command-line option warning is emitted`` (warnNumber: int) (option: string) =
        FSharp "module Module"
        |> withOptions [ option ]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withWarningCode warnNumber
        |> ignore

    // -- --nowarn suppresses the warning -------------------------------------------

    [<Theory>]
    [<InlineData(75, "--extraoptimizationloops:1")>]
    [<InlineData(75, "--typedtree")>]
    [<InlineData(1063, "--test:NoSuchTestFlag")>]
    let ``--nowarn suppresses command-line option warning`` (warnNumber: int) (option: string) =
        FSharp "module Module"
        |> withNoWarn warnNumber
        |> withOptions [ option ]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``--nowarn 3551 suppresses duplicate source file warning`` () =
        let file =
            SourceCodeFileKind.Fs(
                { FileName = "test.fs"
                  SourceText = Some """printfn "Hello" """ }
            )

        fsFromString file
        |> FS
        |> asExe
        |> withAdditionalSourceFile file
        |> withNoWarn 3551
        |> compile
        |> shouldSucceed

    // -- --warnaserror interaction --------------------------------------------------

    [<Fact>]
    let ``--warnaserror+ with --nowarn suppresses rather than errors`` () =
        FSharp "module Module"
        |> withOptions [ "--warnaserror+"; "--extraoptimizationloops:1"; "--nowarn:75" ]
        |> compile
        |> shouldSucceed
