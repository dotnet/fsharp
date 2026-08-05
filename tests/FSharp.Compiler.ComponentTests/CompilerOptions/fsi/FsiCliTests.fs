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
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("RESULT_MARKER_18086", result.StdOut)
        Assert.DoesNotContain("Determining projects to restore", result.StdOut)
        Assert.DoesNotContain("Restored ", result.StdOut)
        Assert.DoesNotContain("NU1", result.StdOut)

    [<Fact>]
    let ``FSI default (non-quiet) mode still evaluates script and prints user output`` () =
        let script = $"""
#r "nuget: {restoreTestPackageId}, {restoreTestPackageVersion}"
printfn "RESULT_MARKER_18086_DEFAULT"
"""
        let result = runFsiScript [] script
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("RESULT_MARKER_18086_DEFAULT", result.StdOut)

    [<Fact>]
    let ``FSI quiet mode still prints user printfn output to stdout`` () =
        let script = """printfn "hello from quiet script" """
        let result = runFsiScript ["--quiet"] script
        Assert.Equal(0, result.ExitCode)
        Assert.Contains("hello from quiet script", result.StdOut)
