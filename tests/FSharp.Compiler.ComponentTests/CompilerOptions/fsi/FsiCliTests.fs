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

    // ============================================================================
    // Issue #18086: --quiet must suppress NuGet restore stdout chatter
    // ============================================================================

    let private writeTempScript (content: string) : string =
        let path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"fsi_quiet_{System.Guid.NewGuid():N}.fsx")
        System.IO.File.WriteAllText(path, content)
        path

    let private runFsiScript (extraArgs: string list) (scriptBody: string) =
        let scriptPath = writeTempScript scriptBody
        try
            let result = runFsiProcess (extraArgs @ [scriptPath])
            result
        finally
            try System.IO.File.Delete(scriptPath) with _ -> ()

    // On failure, surface the FSI subprocess output so CI logs show what actually happened (e.g. a
    // NuGet restore error) instead of a bare "Expected 0, Actual 1". xunit's Assert.Equal/Contains do
    // not include the process stdout/stderr, so these wrappers append it to the failure message.
    let private fsiDiagnostics (result: ProcessResult) =
        $"FSI exit code: %d{result.ExitCode}\n--- FSI STDOUT ---\n%s{result.StdOut}\n--- FSI STDERR ---\n%s{result.StdErr}\n--- end FSI output ---"

    let private assertFsiExitCode (expected: int) (result: ProcessResult) =
        if result.ExitCode <> expected then
            Assert.Fail($"Expected FSI exit code %d{expected} but got %d{result.ExitCode}.\n%s{fsiDiagnostics result}")

    let private assertStdOutContains (expected: string) (result: ProcessResult) =
        if not (result.StdOut.Contains(expected)) then
            Assert.Fail($"Expected FSI stdout to contain '%s{expected}'.\n%s{fsiDiagnostics result}")

    let private assertStdOutDoesNotContain (unexpected: string) (result: ProcessResult) =
        if result.StdOut.Contains(unexpected) then
            Assert.Fail($"Expected FSI stdout NOT to contain '%s{unexpected}'.\n%s{fsiDiagnostics result}")

    // The FSI #r "nuget:" restore below must request a package (and closure) already in the offline
    // restore cache on the internal signed build (which cannot restore online), and it must be a genuine
    // third-party assembly (not in the shared framework) so that on .NET Core it resolves to a restored
    // package rather than the framework (which would emit NU1510 and skip real nuget resolution). FsCheck
    // fits: a real third-party library whose only dependency (FSharp.Core) is always cached and filtered
    // from fsx resolution, centrally pinned (eng/Packages.props) and restored by FSharp.Core.UnitTests, so
    // it restores offline-clean on both net472 and .NET Core. Read the exact pinned version baked into this
    // test assembly via AssemblyMetadata (see FSharp.Compiler.ComponentTests.fsproj) so the request never
    // drifts from the pin; keep the package id below in sync with that project.
    [<Literal>]
    let private restoreTestPackageId = "FsCheck"

    let private restoreTestPackageVersion =
        System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof<System.Reflection.AssemblyMetadataAttribute>, false)
        |> Array.tryPick (fun a ->
            let m = a :?> System.Reflection.AssemblyMetadataAttribute
            if m.Key = "FsiRestoreTestPackageVersion" && not (System.String.IsNullOrWhiteSpace m.Value) then Some m.Value else None)
        |> Option.defaultWith (fun () ->
            failwith "AssemblyMetadata 'FsiRestoreTestPackageVersion' is missing. It should be emitted by FSharp.Compiler.ComponentTests.fsproj from the central FsCheck PackageVersion.")

    [<Fact>]
    let ``FSI quiet mode suppresses NuGet restore output from stdout`` () =
        let script = $"""
#r "nuget: {restoreTestPackageId}, {restoreTestPackageVersion}"
printfn "RESULT_MARKER_18086"
"""
        let result = runFsiScript ["--quiet"] script
        assertFsiExitCode 0 result
        assertStdOutContains "RESULT_MARKER_18086" result
        assertStdOutDoesNotContain "Determining projects to restore" result
        assertStdOutDoesNotContain "Restored " result
        assertStdOutDoesNotContain "NU1" result

    [<Fact>]
    let ``FSI default (non-quiet) mode still evaluates script and prints user output`` () =
        let script = $"""
#r "nuget: {restoreTestPackageId}, {restoreTestPackageVersion}"
printfn "RESULT_MARKER_18086_DEFAULT"
"""
        let result = runFsiScript [] script
        assertFsiExitCode 0 result
        assertStdOutContains "RESULT_MARKER_18086_DEFAULT" result

    [<Fact>]
    let ``FSI quiet mode still prints user printfn output to stdout`` () =
        let script = """printfn "hello from quiet script" """
        let result = runFsiScript ["--quiet"] script
        assertFsiExitCode 0 result
        assertStdOutContains "hello from quiet script" result
