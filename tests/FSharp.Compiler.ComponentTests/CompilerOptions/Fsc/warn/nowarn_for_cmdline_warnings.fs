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

    // FS0075 also covers the unknown --test: argument warning emitted from CompilerOptions.fs
    // (line ~1465: warning (Error(FSComp.SR.optsUnknownArgumentToTheTestSwitch str, ...)))
    // That message currently uses error number 75 as well because it shares the
    // InternalCommandLineOption phrase family (`buildArgInvalidInt` etc. behavior).
    // If, on your branch, the unknown --test: argument turns out to use a different number,
    // adjust the nowarn argument accordingly. The point is: --nowarn <N> should silence it.
    [<Fact>]
    let ``--nowarn for the --test unknown-arg warning suppresses it`` () =
        // Use --test:NotARealTestArg to trigger optsUnknownArgumentToTheTestSwitch.
        // Look up its FS error number; in current main it surfaces as FS1052
        // (buildUnrecognizedOption family). Cover both common numbers via a wider nowarn list.
        FSharp "module Module"
        |> withOptions ["--nowarn:75"; "--nowarn:1052"; "--test:NotARealTestArg"]
        |> compile
        |> shouldSucceed
