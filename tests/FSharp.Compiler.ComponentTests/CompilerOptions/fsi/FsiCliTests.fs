// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

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
