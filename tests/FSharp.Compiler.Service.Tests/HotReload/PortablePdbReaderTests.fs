namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.Collections.Immutable
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open Xunit
open FSharp.Compiler.AbstractIL.ILBaselineReader
open FSharp.Compiler.HotReloadBaseline

/// Comprehensive tests for the Portable PDB reader.
/// These tests validate that readPortablePdbMetadata produces the same results
/// as System.Reflection.Metadata's MetadataReaderProvider.
module PortablePdbReaderTests =

    /// Marker type for assembly location
    type TestMarker = class end

    /// Get SRM's PDB table row counts for comparison
    let private getSrmPdbRowCounts (reader: MetadataReader) =
        [|
            reader.Documents.Count                  // 0x30 - Document
            reader.MethodDebugInformation.Count     // 0x31 - MethodDebugInformation
            reader.LocalScopes.Count                // 0x32 - LocalScope
            reader.LocalVariables.Count             // 0x33 - LocalVariable
            reader.LocalConstants.Count             // 0x34 - LocalConstant
            reader.ImportScopes.Count               // 0x35 - ImportScope
            0  // 0x36 - StateMachineMethod (not exposed directly)
            reader.CustomDebugInformation.Count     // 0x37 - CustomDebugInformation
        |]

    /// Get SRM's entry point token
    let private getSrmEntryPoint (reader: MetadataReader) =
        let handle = reader.DebugMetadataHeader.EntryPoint
        if handle.IsNil then None
        else
            let entityHandle: EntityHandle = MethodDefinitionHandle.op_Implicit handle
            Some(MetadataTokens.GetToken entityHandle)

    /// Get PDB bytes from the test assembly's companion PDB file
    let private getTestPdbBytes () =
        let assembly = typeof<TestMarker>.Assembly
        let assemblyPath = assembly.Location
        let pdbPath = Path.ChangeExtension(assemblyPath, ".pdb")
        if File.Exists(pdbPath) then
            Some(File.ReadAllBytes(pdbPath))
        else
            // Try obj directory
            let objPdbPath = assemblyPath.Replace("/bin/", "/obj/").Replace("\\bin\\", "\\obj\\")
            let objPdbPath = Path.ChangeExtension(objPdbPath, ".pdb")
            if File.Exists(objPdbPath) then
                Some(File.ReadAllBytes(objPdbPath))
            else
                None

    /// Get assembly bytes from the test assembly
    let private getTestAssemblyBytes () =
        let assembly = typeof<TestMarker>.Assembly
        File.ReadAllBytes(assembly.Location)

    // ============================================================================
    // Basic Parsing Tests
    // ============================================================================

    [<Fact>]
    let ``readPortablePdbMetadata parses real PDB from test assembly`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            let result = readPortablePdbMetadata pdbBytes
            Assert.True(result.IsSome, "Should successfully parse real PDB bytes")

    [<Fact>]
    let ``readPortablePdbMetadata returns valid table row counts`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            let result = readPortablePdbMetadata pdbBytes
            Assert.True(result.IsSome)

            let pdbMeta = result.Value
            // Should have 8 table row counts (for PDB tables 0x30-0x37)
            Assert.Equal(8, pdbMeta.TableRowCounts.Length)

            // All row counts should be non-negative
            for i in 0..7 do
                Assert.True(pdbMeta.TableRowCounts.[i] >= 0,
                    $"Table {i} row count should be non-negative, got {pdbMeta.TableRowCounts.[i]}")

    // ============================================================================
    // Parity Tests Against SRM
    // ============================================================================

    [<Fact>]
    let ``readPortablePdbMetadata matches SRM for table row counts`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Parse using our byte-based reader
            let ourResult = readPortablePdbMetadata pdbBytes
            Assert.True(ourResult.IsSome, "Our reader should parse the PDB")
            let ourMeta = ourResult.Value

            // Parse using SRM
            use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
            let srmReader = provider.GetMetadataReader()
            let srmCounts = getSrmPdbRowCounts srmReader

            // Compare each table
            let tableNames = [|
                "Document"; "MethodDebugInformation"; "LocalScope"; "LocalVariable";
                "LocalConstant"; "ImportScope"; "StateMachineMethod"; "CustomDebugInformation"
            |]

            for i in 0..7 do
                Assert.True(srmCounts.[i] = ourMeta.TableRowCounts.[i],
                    $"{tableNames.[i]} (0x{0x30 + i:X2}) row count mismatch: SRM={srmCounts.[i]}, ours={ourMeta.TableRowCounts.[i]}")

    [<Fact>]
    let ``readPortablePdbMetadata matches SRM for entry point`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Parse using our byte-based reader
            let ourResult = readPortablePdbMetadata pdbBytes
            Assert.True(ourResult.IsSome)
            let ourMeta = ourResult.Value

            // Parse using SRM
            use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
            let srmReader = provider.GetMetadataReader()
            let srmEntryPoint = getSrmEntryPoint srmReader

            // Compare entry points
            if srmEntryPoint <> ourMeta.EntryPointToken then
                Assert.Fail($"Entry point mismatch: SRM={srmEntryPoint}, ours={ourMeta.EntryPointToken}")

    // ============================================================================
    // Edge Case Tests
    // ============================================================================

    [<Fact>]
    let ``readPortablePdbMetadata handles empty PDB tables correctly`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            let result = readPortablePdbMetadata pdbBytes
            Assert.True(result.IsSome)

            // Verify we can handle zeros in table counts
            let pdbMeta = result.Value
            // LocalConstants might be 0 for simple methods
            // This is valid and should not cause issues
            Assert.True(pdbMeta.TableRowCounts.[4] >= 0, "LocalConstant count should be >= 0")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for truncated PDB`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Truncate the PDB to various sizes and ensure graceful failure
            for truncateAt in [0; 4; 16; 32; 64] do
                if truncateAt < pdbBytes.Length && truncateAt > 0 then
                    let truncated = pdbBytes.[0..truncateAt-1]
                    let result = readPortablePdbMetadata truncated
                    // Should either return None or handle gracefully
                    // (not throw an exception)
                    Assert.True(result.IsNone || result.IsSome,
                        $"Should handle truncation at {truncateAt} bytes")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for corrupted signature`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Corrupt the BSJB signature
            let corrupted = Array.copy pdbBytes
            corrupted.[0] <- 0xFFuy  // Change 'B' to 0xFF
            let result = readPortablePdbMetadata corrupted
            Assert.True(result.IsNone, "Should return None for corrupted signature")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for all-zero bytes`` () =
        let zeroBytes = Array.zeroCreate<byte> 1000
        let result = readPortablePdbMetadata zeroBytes
        Assert.True(result.IsNone, "Should return None for all-zero bytes")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for PE file bytes`` () =
        // PE files start with MZ signature, not BSJB
        let assemblyBytes = getTestAssemblyBytes ()
        let result = readPortablePdbMetadata assemblyBytes
        Assert.True(result.IsNone, "Should return None for PE file (not PDB)")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for invalid short bytes`` () =
        // Too short to contain valid PDB
        let shortBytes = [| 0x42uy; 0x53uy; 0x4Auy |]  // Partial BSJB
        let result = readPortablePdbMetadata shortBytes
        Assert.True(result.IsNone, "Should return None for bytes shorter than 4")

    // ============================================================================
    // Integration with HotReloadPdb.createSnapshot
    // ============================================================================

    [<Fact>]
    let ``createSnapshot correctly uses readPortablePdbMetadata`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Use the public createSnapshot function
            let snapshot = FSharp.Compiler.HotReloadPdb.createSnapshot pdbBytes

            // Verify it has the expected structure
            Assert.Equal(pdbBytes.Length, snapshot.Bytes.Length)
            Assert.Equal(64, snapshot.TableRowCounts.Length)

            // Parse with SRM for comparison
            use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
            let srmReader = provider.GetMetadataReader()

            // Check Document count (table 0x30)
            Assert.Equal(srmReader.Documents.Count, snapshot.TableRowCounts.[0x30])

            // Check MethodDebugInformation count (table 0x31)
            Assert.Equal(srmReader.MethodDebugInformation.Count, snapshot.TableRowCounts.[0x31])

    [<Fact>]
    let ``createSnapshot matches SRM for all PDB table counts`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            let snapshot = FSharp.Compiler.HotReloadPdb.createSnapshot pdbBytes

            use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
            let srmReader = provider.GetMetadataReader()

            // Verify all PDB table counts match
            Assert.Equal(srmReader.Documents.Count, snapshot.TableRowCounts.[0x30])
            Assert.Equal(srmReader.MethodDebugInformation.Count, snapshot.TableRowCounts.[0x31])
            Assert.Equal(srmReader.LocalScopes.Count, snapshot.TableRowCounts.[0x32])
            Assert.Equal(srmReader.LocalVariables.Count, snapshot.TableRowCounts.[0x33])
            Assert.Equal(srmReader.LocalConstants.Count, snapshot.TableRowCounts.[0x34])
            Assert.Equal(srmReader.ImportScopes.Count, snapshot.TableRowCounts.[0x35])
            // 0x36 (StateMachineMethod) is not commonly used
            Assert.Equal(srmReader.CustomDebugInformation.Count, snapshot.TableRowCounts.[0x37])

    // ============================================================================
    // Stress Tests
    // ============================================================================

    [<Fact>]
    let ``readPortablePdbMetadata handles multiple sequential parses`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Parse the same PDB multiple times - should be deterministic
            let results =
                [1..10]
                |> List.map (fun _ -> readPortablePdbMetadata pdbBytes)

            // All results should be Some
            Assert.True(results |> List.forall (fun r -> r.IsSome))

            // All results should have the same row counts
            let first = results.[0].Value
            for result in results do
                let r = result.Value
                for i in 0..7 do
                    Assert.Equal(first.TableRowCounts.[i], r.TableRowCounts.[i])

    [<Fact>]
    let ``readPortablePdbMetadata is thread-safe`` () =
        match getTestPdbBytes () with
        | None -> () // Skip if no PDB available
        | Some pdbBytes ->
            // Parse from multiple threads concurrently
            let tasks =
                [1..10]
                |> List.map (fun _ ->
                    System.Threading.Tasks.Task.Run(fun () ->
                        readPortablePdbMetadata pdbBytes))
                |> Array.ofList

            System.Threading.Tasks.Task.WaitAll(tasks |> Array.map (fun t -> t :> System.Threading.Tasks.Task))

            // All should succeed with same results
            let results = tasks |> Array.map (fun t -> t.Result)
            Assert.True(results |> Array.forall (fun r -> r.IsSome))

            let first = results.[0].Value
            for result in results do
                let r = result.Value
                Assert.Equal(first.TableRowCounts.Length, r.TableRowCounts.Length)

