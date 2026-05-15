// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Regression tests for https://github.com/dotnet/fsharp/issues/19576
///
/// Warnings emitted during command-line option parsing (e.g. FS0075 for internal/test-only
/// options, FS1063 for unknown --test sub-flags, FS3551 for duplicate source files) must
/// honor `--nowarn:<n>` just like any other compiler warning. They are routed through the
/// local `warningCmdLine` helper which consults `tcConfigB.diagnosticsOptions`.
module ``Nowarn for command-line option warnings`` =

    // FS0075: "The command-line option '%s' is for test purposes only" — reportDeprecatedOption.
    // FS1063: "Unknown --test argument: '%s'" — testingAndQAFlags.
    [<InlineData(75, "--extraoptimizationloops:1")>]
    [<InlineData(75, "--typedtree")>]
    [<InlineData(1063, "--test:NoSuchTestFlag")>]
    [<Theory>]
    let ``--nowarn suppresses command-line option warning`` (warnNumber: int) (option: string) =
        FSharp "module Module"
        |> withNoWarn warnNumber
        |> withOptions [option]
        |> compile
        |> shouldSucceed

    // FS3551: "The source file '%s' (at position %d/%d) already appeared in the compilation list ..."
    // Emitted by CheckAndReportSourceFileDuplicates and routed through `warningCmdLine`.
    [<Fact>]
    let ``--nowarn 3551 suppresses duplicate source file warning`` () =
        let file = SourceCodeFileKind.Fs({ FileName = "test.fs"; SourceText = Some """printfn "Hello" """ })

        fsFromString file
        |> FS
        |> asExe
        |> withAdditionalSourceFile file
        |> withNoWarn 3551
        |> compile
        |> shouldSucceed
