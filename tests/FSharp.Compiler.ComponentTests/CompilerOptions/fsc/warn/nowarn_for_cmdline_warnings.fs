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

    [<Theory>]
    [<InlineData(75, "--extraoptimizationloops:1")>]
    [<InlineData(75, "--typedtree")>]
    [<InlineData(1063, "--test:NoSuchTestFlag")>]
    let ``--nowarn after triggering option still suppresses`` (warnNumber: int) (option: string) =
        FSharp "module Module"
        |> withOptions [ option; $"--nowarn:{warnNumber}" ]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``--nowarn suppresses only the targeted warning`` () =
        FSharp "module Module"
        |> withOptions [ "--extraoptimizationloops:1"; "--test:NoSuchTestFlag"; "--nowarn:75" ]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withWarningCode 1063
        |> ignore

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
    let ``--warnaserror+ promotes command-line option warning to error`` () =
        FSharp "module Module"
        |> withOptions [ "--warnaserror+"; "--extraoptimizationloops:1" ]
        |> compile
        |> shouldFail
        |> withErrorCode 75
        |> ignore

    [<Fact>]
    let ``--warnaserror+ with --nowarn suppresses rather than errors`` () =
        FSharp "module Module"
        |> withOptions [ "--warnaserror+"; "--extraoptimizationloops:1"; "--nowarn:75" ]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``--warnaserror 75 with --nowarn 75 still errors because specific warnaserror wins`` () =
        FSharp "module Module"
        |> withOptions [ "--warnaserror:75"; "--extraoptimizationloops:1"; "--nowarn:75" ]
        |> compile
        |> shouldFail
        |> withErrorCode 75
        |> ignore
