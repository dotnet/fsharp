// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Regression tests for https://github.com/dotnet/fsharp/issues/19576
///
/// Warnings emitted during command-line option parsing (e.g. FS0075 for internal/test-only
/// options, FS3211 for duplicate source files, the test-switch unknown-arg warning) must
/// honor `--nowarn:<n>` just like any other compiler warning.
module ``Nowarn for command-line option warnings`` =

    // FS0075: "The command-line option '%s' is for test purposes only"
    // Emitted by reportDeprecatedOption in CompilerOptions.fs when an InternalCommandLineOption
    // is used (e.g. --extraoptimizationloops).
    [<Fact>]
    let ``--nowarn 75 suppresses FS0075 for --extraoptimizationloops`` () =
        FSharp "module Module"
        |> withNoWarn 75
        |> withOptions ["--extraoptimizationloops:1"]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``--nowarn 75 suppresses FS0075 for --typedtree`` () =
        FSharp "module Module"
        |> withNoWarn 75
        |> withOptions ["--typedtree"]
        |> compile
        |> shouldSucceed

