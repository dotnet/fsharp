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

    /// Migrated from: -?-40
    /// Original: SOURCE=dummy.fsx PRECMD="$FSI_PIPE >help.txt -? 2>&1"
    /// CLI Test: FSI -? shorthand help option
    [<Fact>]
    let ``fsi help - shorthand -? shows help and exits with 0`` () =
        let result = runFsiProcess ["-?"]
        Assert.Equal(0, result.ExitCode)
        // Verify key sections from help40.437.1033.bsl baseline
        Assert.Contains("Usage:", result.StdOut)
        Assert.Contains("INPUT FILES", result.StdOut)
        Assert.Contains("--use:", result.StdOut)

    /// Migrated from: --help-40
    /// Original: SOURCE=dummy.fsx PRECMD="$FSI_PIPE >help.txt --help 2>&1"
    /// CLI Test: FSI --help long form option
    [<Fact>]
    let ``fsi help - long form --help shows help and exits with 0`` () =
        let result = runFsiProcess ["--help"]
        Assert.Equal(0, result.ExitCode)
        // Verify key sections from help40.437.1033.bsl baseline
        Assert.Contains("Usage:", result.StdOut)
        Assert.Contains("INPUT FILES", result.StdOut)
        Assert.Contains("CODE GENERATION", result.StdOut)

    /// Migrated from: /?-40
    /// Original: SOURCE=dummy.fsx PRECMD="$FSI_PIPE >help.txt /? 2>&1"
    /// CLI Test: FSI /? Windows-style help option
    [<Fact>]
    let ``fsi help - Windows-style /? shows help and exits with 0`` () =
        let result = runFsiProcess ["/?"]
        Assert.Equal(0, result.ExitCode)
        // Verify key sections from help40.437.1033.bsl baseline
        Assert.Contains("Usage:", result.StdOut)
        Assert.Contains("--reference:", result.StdOut)

    /// Migrated from: -? --nologo-40
    /// Original: SOURCE=dummy.fsx PRECMD="$FSI_PIPE >help.txt --nologo -? 2>&1"
    /// CLI Test: FSI --nologo -? shows help without banner
    [<Fact>]
    let ``fsi help - nologo -? shows help without copyright banner`` () =
        let result = runFsiProcess ["--nologo"; "-?"]
        Assert.Equal(0, result.ExitCode)
        // Verify help content from help40-nologo.437.1033.bsl baseline
        Assert.Contains("Usage:", result.StdOut)
        // With --nologo, should NOT contain copyright header
        Assert.DoesNotContain("Microsoft (R) F# Interactive", result.StdOut)

    // ============================================================================
    // Language Version Help (documented in help baseline)
    // CLI behavior: FSI prints language version info and exits
    // ============================================================================

    /// Migrated from: help baseline documentation (lines 66-67)
    /// CLI Test: FSI --langversion:? shows available language versions
    [<Fact>]
    let ``fsi help - langversion ? shows available versions and exits with 0`` () =
        let result = runFsiProcess ["--langversion:?"]
        Assert.Equal(0, result.ExitCode)
        // Should list available language versions
        Assert.Contains("Supported language versions:", result.StdOut)
        Assert.Contains("preview", result.StdOut)
        Assert.Contains("latest", result.StdOut)

    // ============================================================================
    // Unrecognized Option Tests (highentropyva, subsystemversion)
    // CLI behavior: FSI reports FS0243 and exits - cannot be tested in-process
    // Original source: tests/fsharpqa/Source/CompilerOptions/fsi/highentropyva/
    //                  tests/fsharpqa/Source/CompilerOptions/fsi/subsystemversion/
    // ============================================================================

    /// Migrated from: E_highentropyva01.fsx
    /// Original: SOURCE=E_highentropyva01.fsx SCFLAGS="--highentropyva+"
    /// Expected: //<Expects status="error" id="FS0243">Unrecognized option: '--highentropyva+'</Expects>
    /// CLI Test: --highentropyva+ is valid for fsc but not fsi
    [<Fact>]
    let ``fsi unrecognized option - highentropyva reports FS0243`` () =
        let result = runFsiProcess ["--highentropyva+"]
        // FSI should report error for unrecognized option
        Assert.NotEqual(0, result.ExitCode)
        Assert.Contains("Unrecognized option: '--highentropyva+'", result.StdErr)

    /// Migrated from: E_subsystemversion01.fsx
    /// Original: SOURCE=E_subsystemversion01.fsx SCFLAGS="--subsystemversion:4.00"
    /// Expected: //<Expects status="error" id="FS0243">Unrecognized option: '--subsystemversion'</Expects>
    /// CLI Test: --subsystemversion is valid for fsc but not fsi
    [<Fact>]
    let ``fsi unrecognized option - subsystemversion reports FS0243`` () =
        let result = runFsiProcess ["--subsystemversion:4.00"]
        // FSI should report error for unrecognized option
        Assert.NotEqual(0, result.ExitCode)
        Assert.Contains("Unrecognized option: '--subsystemversion'", result.StdErr)
