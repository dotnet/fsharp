// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open System.Runtime.InteropServices
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsc/pdb
// --pdb option tests (WindowsOnly - NOMONO tests for pdb file creation)
// Note: Most original tests involved PRECMD/POSTCMD file system checks which cannot
// be directly migrated. These tests verify the --pdb option behavior and errors.

module Pdb =

    // Test: --pdb without --debug produces expected error (E_pdb_and_debug.fs)
    // Original: NOMONO SOURCE=E_pdb_and_debug.fs SCFLAGS="--pdb:pdb01.pdb"
    [<Fact>]
    let ``pdb - pdb without debug produces error`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """exit 0"""
            |> asExe
            |> withOptions ["--pdb:pdb01.pdb"]
            |> compile
            |> shouldFail
            |> withErrorCode 209
            |> withDiagnosticMessageMatches "The '--pdb' option requires the '--debug' option to be used"
            |> ignore

    // Test: --pdb with --debug (pdb01.fs)
    // Original: NOMONO SOURCE=pdb01.fs SCFLAGS="-g --pdb:pdb01.pdb"
    [<Fact>]
    let ``pdb - pdb with debug succeeds`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """exit 0"""
            |> asExe
            |> withOptions ["-g"; "--pdb:test.pdb"]
            |> compile
            |> shouldSucceed
            |> verifyHasPdb

    // Test: --PDB (uppercase) is not recognized
    // Original: SOURCE=pdb02.fs SCFLAGS="--PDB -g"
    [<Fact>]
    let ``pdb - uppercase PDB is unrecognized`` () =
        FSharp """exit 1"""
        |> asExe
        |> withOptions ["--PDB"; "-g"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PDB'"
        |> ignore

    // Note: pdb03.fsx (--pdb in fsi mode) cannot be migrated - it requires FSIMODE=PIPE
    // which tests actual FSI execution, not fsc compilation.

    // Test: --pdb cannot match the output filename
    // Original: NOMONO SOURCE=pdb04.fs SCFLAGS="-g --pdb:pdb04.exe"
    [<Fact>]
    let ``pdb - pdb cannot match output filename`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """exit 1"""
            |> asExe
            |> withName "testpdb"
            |> withOptions ["-g"; "--pdb:testpdb.exe"]
            |> compile
            |> shouldFail
            |> withErrorCode 1001
            |> withDiagnosticMessageMatches "The pdb output file name cannot match the build output filename"
            |> ignore

    // Test: --debug:embedded should not create separate pdb file
    [<Fact>]
    let ``pdb - debug embedded does not create pdb`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """printfn "Hello, World" """
            |> asExe
            |> withOptions ["-g"; "--debug:embedded"]
            |> compile
            |> shouldSucceed
            |> verifyNoPdb

    // Test: --debug:portable creates portable pdb
    [<Fact>]
    let ``pdb - debug portable creates pdb`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """printfn "Hello, World" """
            |> asExe
            |> withOptions ["-g"; "--debug:portable"]
            |> compile
            |> shouldSucceed
            |> verifyHasPdb

    // Test: --pdb with different file name
    [<Fact>]
    let ``pdb - pdb with different filename succeeds`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """exit 0"""
            |> asExe
            |> withOptions ["--debug"; "--pdb:custom.pdb"]
            |> compile
            |> shouldSucceed
            |> verifyHasPdb

    // Test: --debug with --pdb and embedded source
    [<Fact>]
    let ``pdb - debug portable with embed succeeds`` () =
        if not (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) then
            () // Skip on non-Windows
        else
            FSharp """printfn "Hello, World" """
            |> asExe
            |> withOptions ["-g"; "--debug:portable"]
            |> compile
            |> shouldSucceed
            |> verifyHasPdb
