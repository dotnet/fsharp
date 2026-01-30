// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Tests for CLI subprocess helpers (runFsiProcess, runFscProcess).
// These are FOR CLI TESTS ONLY where subprocess execution is legitimately required.
// Use cases: --help output, exit codes, missing file CLI errors.

module CliProcessTests =

    // ============================================================================
    // FSI Process Tests
    // These test CLI behavior that cannot be tested in-process (--help, exit codes)
    // ============================================================================

    /// CLI Test: FSI --help exits with code 0 and produces help output
    [<Fact>]
    let ``runFsiProcess - help flag produces help output and exits with 0`` () =
        let result = runFsiProcess ["--help"]
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("F# Interactive", result.StdOut)

    /// CLI Test: FSI --version exits with code 0 and produces version output
    [<Fact>]
    let ``runFsiProcess - version flag produces version output`` () =
        let result = runFsiProcess ["--version"]
        Assert.Equal(0, result.ExitCode)
        // Should contain version information
        Assert.True(result.StdOut.Length > 0 || result.StdErr.Length > 0, "Expected some output from --version")

    /// CLI Test: FSI with invalid option returns non-zero exit code
    [<Fact>]
    let ``runFsiProcess - invalid option returns non-zero exit code`` () =
        let result = runFsiProcess ["--this-option-does-not-exist-xyz"]
        // Invalid options typically cause non-zero exit or error message
        Assert.True(result.ExitCode <> 0 || result.StdErr.Contains("error"), "Expected error for invalid option")

    // ============================================================================
    // FSC Process Tests  
    // These test CLI behavior that cannot be tested in-process (missing file errors)
    // ============================================================================

    /// CLI Test: FSC --help exits with code 0 and produces help output
    [<Fact>]
    let ``runFscProcess - help flag produces help output and exits with 0`` () =
        let result = runFscProcess ["--help"]
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("F# Compiler", result.StdOut)

    /// CLI Test: FSC --version exits with code 0 and produces version output
    [<Fact>]
    let ``runFscProcess - version flag produces version output`` () =
        let result = runFscProcess ["--version"]
        Assert.Equal(0, result.ExitCode)
        // Should contain version information
        Assert.True(result.StdOut.Length > 0 || result.StdErr.Length > 0, "Expected some output from --version")

    /// CLI Test: FSC with missing source file returns error
    /// This is a legitimate subprocess case - CLI parsing error for non-existent files
    [<Fact>]
    let ``runFscProcess - missing source file returns error`` () =
        let result = runFscProcess ["nonexistent_file_xyz123.fs"]
        // FSC should return non-zero exit code or error message for missing file
        Assert.True(result.ExitCode <> 0 || result.StdErr.Length > 0, "Expected error for missing source file")

    /// CLI Test: FSC with invalid option returns error
    [<Fact>]
    let ``runFscProcess - invalid option returns error`` () =
        let result = runFscProcess ["--this-option-does-not-exist-xyz"]
        // Invalid options typically cause non-zero exit or error message
        Assert.True(result.ExitCode <> 0 || result.StdErr.Contains("error"), "Expected error for invalid option")
