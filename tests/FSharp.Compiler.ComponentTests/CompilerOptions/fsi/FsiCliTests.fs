// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsi/help, highentropyva, subsystemversion
// These tests require subprocess execution because:
// - Help options (-?, --help, /?) cause FSI to print help and exit before session creation
// - Unrecognized options (--highentropyva+, --subsystemversion) cause FS0243 and exit
// Original source: git show eb1873ff3:tests/fsharpqa/Source/CompilerOptions/fsi/

module FsiCliTests =

    // ============================================================================
    // Help Tests (tests/fsharpqa/Source/CompilerOptions/fsi/help/env.lst)
    // CLI behavior: FSI prints help and exits - cannot be tested in-process
    // ============================================================================

    /// Migrated from: -?-40, --help-40, /?-40
    [<InlineData("-?", "INPUT FILES")>]
    [<InlineData("--help", "CODE GENERATION")>]
    [<InlineData("/?", "--reference:")>]
    [<Theory>]
    let ``fsi help - flag shows help and exits with 0`` (flag: string, expectedContent: string) =
        let result = runFsiProcess [flag]
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("Usage:", result.StdOut)
        Assert.Contains(expectedContent, result.StdOut)

    /// Migrated from: -? --nologo-40
    [<Fact>]
    let ``fsi help - nologo -? shows help without copyright banner`` () =
        let result = runFsiProcess ["--nologo"; "-?"]
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("Usage:", result.StdOut)
        Assert.DoesNotContain("Microsoft (R) F# Interactive", result.StdOut)

    // ============================================================================
    // Language Version Help (documented in help baseline)
    // CLI behavior: FSI prints language version info and exits
    // ============================================================================

    /// Migrated from: help baseline documentation (lines 66-67)
    [<Fact>]
    let ``fsi help - langversion ? shows available versions and exits with 0`` () =
        let result = runFsiProcess ["--langversion:?"]
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("Supported language versions:", result.StdOut)
        Assert.Contains("preview", result.StdOut)
        Assert.Contains("latest", result.StdOut)

    // ============================================================================
    // Unrecognized Option Tests (highentropyva, subsystemversion)
    // CLI behavior: FSI reports FS0243 and exits - cannot be tested in-process
    // Original source: tests/fsharpqa/Source/CompilerOptions/fsi/highentropyva/
    //                  tests/fsharpqa/Source/CompilerOptions/fsi/subsystemversion/
    // ============================================================================

    /// Migrated from: E_highentropyva01.fsx, E_subsystemversion01.fsx
    [<InlineData("--highentropyva+", "Unrecognized option: '--highentropyva+'")>]
    [<InlineData("--subsystemversion:4.00", "Unrecognized option: '--subsystemversion'")>]
    [<Theory>]
    let ``fsi unrecognized option - reports FS0243`` (option: string, expectedError: string) =
        let result = runFsiProcess [option]
        Assert.NotEqual(0, result.ExitCode)
        Assert.Contains(expectedError, result.StdErr)

    // ============================================================================
    // --quiet option tests
    // ============================================================================

    /// CLI Test: --quiet suppresses the banner
    [<Fact>]
    let ``fsi quiet - suppresses banner`` () =
        let result = runFsiProcess ["--quiet"; "--exec"; "--nologo"]
        Assert.Equal(0, result.ExitCode)
        Assert.DoesNotContain("Microsoft (R) F# Interactive", result.StdOut)

    /// In-process test: --quiet suppresses feedback output but expressions still evaluate
    [<Fact>]
    let ``fsi quiet - expressions evaluate correctly`` () =
        Fsx """let x = 1 + 1"""
        |> withOptions ["--quiet"]
        |> runFsi
        |> shouldSucceed

    // ============================================================================
    // --exec option tests
    // ============================================================================

    /// CLI Test: --exec causes FSI to exit after evaluating (no interactive prompt)
    [<Fact>]
    let ``fsi exec - exits after evaluating script`` () =
        let tmpFile = Path.Combine(Path.GetTempPath(), $"fsi_exec_test_{Guid.NewGuid()}.fsx")
        try
            File.WriteAllText(tmpFile, "printfn \"hello from exec\"")
            let result = runFsiProcess ["--exec"; "--nologo"; tmpFile]
            Assert.Equal(0, result.ExitCode)
            Assert.Contains("hello from exec", result.StdOut)
        finally
            try File.Delete(tmpFile) with _ -> ()

    // ============================================================================
    // --use option tests
    // ============================================================================

    /// CLI Test: --use:file.fsx loads and executes a script file
    [<Fact>]
    let ``fsi use - loads and executes script file`` () =
        let tmpFile = Path.Combine(Path.GetTempPath(), $"fsi_use_test_{Guid.NewGuid()}.fsx")
        try
            File.WriteAllText(tmpFile, "printfn \"loaded via use\"")
            let result = runFsiProcess ["--nologo"; "--exec"; $"--use:{tmpFile}"]
            Assert.Equal(0, result.ExitCode)
            Assert.Contains("loaded via use", result.StdOut)
        finally
            try File.Delete(tmpFile) with _ -> ()

    /// CLI Test: --use with nonexistent file produces error
    [<Fact>]
    let ``fsi use - nonexistent file produces error`` () =
        let result = runFsiProcess ["--exec"; "--use:nonexistent_file_xyz.fsx"]
        Assert.NotEqual(0, result.ExitCode)

    // ============================================================================
    // --load option tests
    // ============================================================================

    /// CLI Test: --load:file.fsx loads a file (definitions available)
    [<Fact>]
    let ``fsi load - loads file definitions`` () =
        let tmpFile = Path.Combine(Path.GetTempPath(), $"fsi_load_test_{Guid.NewGuid()}.fsx")
        try
            File.WriteAllText(tmpFile, "let loadedValue = 42")
            let result = runFsiProcess ["--nologo"; "--exec"; $"--load:{tmpFile}"]
            Assert.Equal(0, result.ExitCode)
        finally
            try File.Delete(tmpFile) with _ -> ()

    /// CLI Test: --load with nonexistent file produces error
    [<Fact>]
    let ``fsi load - nonexistent file produces error`` () =
        let result = runFsiProcess ["--exec"; "--load:nonexistent_file_xyz.fsx"]
        Assert.NotEqual(0, result.ExitCode)

    // ============================================================================
    // --gui option tests (switch: +/-)
    // ============================================================================

    /// CLI Test: --gui- is accepted without error
    [<Fact>]
    let ``fsi gui - gui minus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--gui-"]
        |> runFsi
        |> shouldSucceed

    /// CLI Test: --gui+ is accepted without error
    [<Fact>]
    let ``fsi gui - gui plus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--gui+"]
        |> runFsi
        |> shouldSucceed

    // ============================================================================
    // --readline option tests (switch: +/-)
    // ============================================================================

    /// CLI Test: --readline- is accepted without error
    [<Fact>]
    let ``fsi readline - readline minus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--readline-"]
        |> runFsi
        |> shouldSucceed

    /// CLI Test: --readline+ is accepted without error
    [<Fact>]
    let ``fsi readline - readline plus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--readline+"]
        |> runFsi
        |> shouldSucceed

    // ============================================================================
    // --quotations-debug option tests (switch: +/-)
    // ============================================================================

    /// CLI Test: --quotations-debug+ is accepted without error
    [<Fact>]
    let ``fsi quotations-debug - plus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--quotations-debug+"]
        |> runFsi
        |> shouldSucceed

    /// CLI Test: --quotations-debug- is accepted without error
    [<Fact>]
    let ``fsi quotations-debug - minus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--quotations-debug-"]
        |> runFsi
        |> shouldSucceed

    // ============================================================================
    // --shadowcopyreferences option tests (switch: +/-)
    // ============================================================================

    /// CLI Test: --shadowcopyreferences+ is accepted without error
    [<Fact>]
    let ``fsi shadowcopyreferences - plus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--shadowcopyreferences+"]
        |> runFsi
        |> shouldSucceed

    /// CLI Test: --shadowcopyreferences- is accepted without error
    [<Fact>]
    let ``fsi shadowcopyreferences - minus accepted`` () =
        Fsx """1+1"""
        |> withOptions ["--shadowcopyreferences-"]
        |> runFsi
        |> shouldSucceed

    // ============================================================================
    // --nologo option tests
    // ============================================================================

    /// CLI Test: --nologo suppresses the banner
    [<Fact>]
    let ``fsi nologo - suppresses banner in subprocess`` () =
        let result = runFsiProcess ["--nologo"; "--exec"]
        Assert.Equal(0, result.ExitCode)
        Assert.DoesNotContain("Microsoft (R) F# Interactive", result.StdOut)

    /// In-process test: FSI without --nologo shows the banner
    [<Fact>]
    let ``fsi nologo - without nologo shows banner`` () =
        Fsx """1+1"""
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "Microsoft"

    // ============================================================================
    // Additional error case tests
    // ============================================================================

    /// CLI Test: completely unknown option produces FS0243
    [<Fact>]
    let ``fsi error - unknown option produces FS0243`` () =
        let result = runFsiProcess ["--not-a-real-option"]
        Assert.NotEqual(0, result.ExitCode)
        Assert.Contains("Unrecognized option: '--not-a-real-option'", result.StdErr)

    /// CLI Test: --warn with invalid level produces error
    [<Fact>]
    let ``fsi error - invalid warn level produces error`` () =
        let result = runFsiProcess ["--warn:invalid"; "--exec"]
        Assert.NotEqual(0, result.ExitCode)
