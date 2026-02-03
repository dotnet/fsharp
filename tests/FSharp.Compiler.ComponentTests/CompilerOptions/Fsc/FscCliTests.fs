// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - Diagnostics/General/E_MissingSourceFile tests
// These tests require subprocess execution because:
// - They test CLI argument parsing for non-existent file paths
// - The error happens during CLI argument validation, not during compilation
// Original source: git show e77f6e6f^:tests/fsharpqa/Source/Diagnostics/General/

module FscCliTests =

    // ============================================================================
    // E_MissingSourceFile Tests (tests/fsharpqa/Source/Diagnostics/General/env.lst)
    // CLI behavior: FSC reports FS0225 for missing source files on command line
    // ============================================================================

    /// Migrated from: E_MissingSourceFile01.fs
    /// Original: SOURCE="E_MissingSourceFile01.fs doesnotexist.fs"
    /// Expected: //<Expects id="FS0225" status="error">Source file ['"].+['"] could not be found</Expects>
    /// CLI Test: FSC with non-existent local file path
    [<Fact>]
    let ``fsc missing source file - local path reports FS0225`` () =
        let result = runFscProcess ["doesnotexist.fs"]
        Assert.NotEqual(0, result.ExitCode)
        // FS0225: Source file 'X' could not be found
        Assert.Contains("could not be found", result.StdErr)

    /// Migrated from: E_MissingSourceFile02.fs
    /// Original: SOURCE="E_MissingSourceFile02.fs X:\doesnotexist.fs"
    /// Expected: //<Expects id="FS0225" status="error">Source file ['"].+['"] could not be found</Expects>
    /// CLI Test: FSC with non-existent absolute path (Windows-style)
    [<FactForWINDOWS>]
    let ``fsc missing source file - absolute Windows path reports FS0225`` () =
        let result = runFscProcess ["X:\\doesnotexist.fs"]
        Assert.NotEqual(0, result.ExitCode)
        // FS0225: Source file 'X' could not be found
        Assert.Contains("could not be found", result.StdErr)

    /// Alternative test for non-Windows: absolute path that doesn't exist
    /// CLI Test: FSC with non-existent absolute path (Unix-style)
    [<FactForNETCOREAPP>]
    let ``fsc missing source file - absolute Unix path reports FS0225`` () =
        let result = runFscProcess ["/nonexistent/path/doesnotexist.fs"]
        Assert.NotEqual(0, result.ExitCode)
        // FS0225: Source file 'X' could not be found
        Assert.Contains("could not be found", result.StdErr)

    /// Migrated from: E_MissingSourceFile03.fs
    /// Original: SOURCE="E_MissingSourceFile03.fs \\qwerty\y\doesnotexist.fs"
    /// Expected: //<Expects id="FS0225" status="error">Source file ['"].+['"] could not be found</Expects>
    /// CLI Test: FSC with non-existent UNC path
    [<FactForWINDOWS>]
    let ``fsc missing source file - UNC path reports FS0225`` () =
        let result = runFscProcess ["\\\\qwerty\\y\\doesnotexist.fs"]
        Assert.NotEqual(0, result.ExitCode)
        // FS0225: Source file 'X' could not be found
        Assert.Contains("could not be found", result.StdErr)

    /// Migrated from: E_MissingSourceFile04.fs
    /// Original: SOURCE=E_MissingSourceFile04.fs SCFLAGS="--exec doesnotexist.fs" FSIMODE=PIPE
    /// Expected: //<Expects id="FS0078" span="(0,1)" status="error">Unable to find the file 'doesnotexist\.fs' in any of</Expects>
    /// CLI Test: FSI with --exec and non-existent file
    /// Note: This tests FSI, not FSC, but is part of the same migration batch
    [<Fact>]
    let ``fsi missing exec file - reports FS0078`` () =
        let result = runFsiProcess ["--exec"; "doesnotexist.fs"]
        Assert.NotEqual(0, result.ExitCode)
        // FS0078: Unable to find the file 'X' in any of ...
        Assert.Contains("Unable to find the file", result.StdErr)
