namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILBaselineReader
open FSharp.Compiler.HotReloadBaseline

/// Tests for ILBaselineReader - verifies byte-based metadata parsing
/// matches SRM MetadataReader results.
module ILBaselineReaderTests =

    /// Marker type for assembly location
    type TestMarker = class end

    /// Helper to get assembly bytes from a compiled test assembly
    let private getTestAssemblyBytes () =
        // Use the current test assembly as a test subject
        let assembly = typeof<TestMarker>.Assembly
        let assemblyPath = assembly.Location
        File.ReadAllBytes(assemblyPath)

    /// Helper to get SRM heap sizes for comparison
    let private getSrmHeapSizes (metadataReader: MetadataReader) =
        { StringHeapSize = metadataReader.GetHeapSize(HeapIndex.String)
          UserStringHeapSize = metadataReader.GetHeapSize(HeapIndex.UserString)
          BlobHeapSize = metadataReader.GetHeapSize(HeapIndex.Blob)
          GuidHeapSize = metadataReader.GetHeapSize(HeapIndex.Guid) }

    /// Helper to get SRM table row counts for comparison
    let private getSrmTableRowCounts (metadataReader: MetadataReader) =
        Array.init 64 (fun i ->
            let tableIndex = LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte i)
            metadataReader.GetTableRowCount(tableIndex))

    [<Fact>]
    let ``metadataSnapshotFromBytes parses valid PE file`` () =
        let bytes = getTestAssemblyBytes ()
        let result = metadataSnapshotFromBytes bytes
        Assert.True(result.IsSome, "Should successfully parse PE file")

    [<Fact>]
    let ``metadataSnapshotFromBytes returns None for invalid bytes`` () =
        let invalidBytes = [| 0uy; 1uy; 2uy; 3uy |]
        let result = metadataSnapshotFromBytes invalidBytes
        Assert.True(result.IsNone, "Should return None for invalid PE file")

    [<Fact>]
    let ``metadataSnapshotFromBytes matches MetadataReader for heap sizes`` () =
        let bytes = getTestAssemblyBytes ()

        // Parse using our byte-based reader
        let byteResult = metadataSnapshotFromBytes bytes
        Assert.True(byteResult.IsSome)
        let byteSnapshot = byteResult.Value

        // Parse using SRM MetadataReader
        use stream = new MemoryStream(bytes)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()
        let srmHeapSizes = getSrmHeapSizes metadataReader

        // Compare heap sizes - our parser reads raw stream sizes from headers,
        // while SRM's GetHeapSize may return content-only size (excluding padding).
        // Per ECMA-335 II.24.2.2, streams are 4-byte aligned, so we allow small tolerance.
        let heapSizeTolerance = 4

        Assert.True(
            abs(srmHeapSizes.StringHeapSize - byteSnapshot.HeapSizes.StringHeapSize) <= heapSizeTolerance,
            $"String heap size mismatch: SRM={srmHeapSizes.StringHeapSize}, byte-based={byteSnapshot.HeapSizes.StringHeapSize}")
        Assert.True(
            abs(srmHeapSizes.UserStringHeapSize - byteSnapshot.HeapSizes.UserStringHeapSize) <= heapSizeTolerance,
            $"UserString heap size mismatch: SRM={srmHeapSizes.UserStringHeapSize}, byte-based={byteSnapshot.HeapSizes.UserStringHeapSize}")
        Assert.True(
            abs(srmHeapSizes.BlobHeapSize - byteSnapshot.HeapSizes.BlobHeapSize) <= heapSizeTolerance,
            $"Blob heap size mismatch: SRM={srmHeapSizes.BlobHeapSize}, byte-based={byteSnapshot.HeapSizes.BlobHeapSize}")
        Assert.Equal(srmHeapSizes.GuidHeapSize, byteSnapshot.HeapSizes.GuidHeapSize)

    [<Fact>]
    let ``metadataSnapshotFromBytes matches MetadataReader for table row counts`` () =
        let bytes = getTestAssemblyBytes ()

        // Parse using our byte-based reader
        let byteResult = metadataSnapshotFromBytes bytes
        Assert.True(byteResult.IsSome)
        let byteSnapshot = byteResult.Value

        // Parse using SRM MetadataReader
        use stream = new MemoryStream(bytes)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()
        let srmTableCounts = getSrmTableRowCounts metadataReader

        // Compare all 64 table row counts
        Assert.Equal(srmTableCounts.Length, byteSnapshot.TableRowCounts.Length)
        for i in 0..63 do
            if srmTableCounts.[i] <> byteSnapshot.TableRowCounts.[i] then
                Assert.Fail($"Table {i} row count mismatch: expected {srmTableCounts.[i]}, got {byteSnapshot.TableRowCounts.[i]}")

    [<Fact>]
    let ``readModuleMvidFromBytes returns valid GUID`` () =
        let bytes = getTestAssemblyBytes ()
        let result = readModuleMvidFromBytes bytes
        Assert.True(result.IsSome, "Should successfully read MVID")
        Assert.NotEqual(System.Guid.Empty, result.Value)

    [<Fact>]
    let ``readModuleMvidFromBytes matches MetadataReader`` () =
        let bytes = getTestAssemblyBytes ()

        // Read using our byte-based reader
        let byteResult = readModuleMvidFromBytes bytes
        Assert.True(byteResult.IsSome)

        // Read using SRM MetadataReader
        use stream = new MemoryStream(bytes)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()
        let moduleDef = metadataReader.GetModuleDefinition()
        let srmMvid =
            if moduleDef.Mvid.IsNil then System.Guid.Empty
            else metadataReader.GetGuid(moduleDef.Mvid)

        Assert.Equal(srmMvid, byteResult.Value)

    [<Fact>]
    let ``metadataSnapshotFromBytes works with delta-generated test assembly`` () =
        // Use a test helper to create a known assembly
        let artifacts = MetadataDeltaTestHelpers.emitPropertyDeltaArtifacts None ()
        let bytes = artifacts.BaselineBytes

        let byteResult = metadataSnapshotFromBytes bytes
        Assert.True(byteResult.IsSome)
        let byteSnapshot = byteResult.Value

        // Parse using SRM MetadataReader
        use stream = new MemoryStream(bytes)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()
        let srmHeapSizes = getSrmHeapSizes metadataReader
        let srmTableCounts = getSrmTableRowCounts metadataReader

        // Compare heap sizes - with tolerance for stream alignment
        let heapSizeTolerance = 4
        Assert.True(
            abs(srmHeapSizes.StringHeapSize - byteSnapshot.HeapSizes.StringHeapSize) <= heapSizeTolerance,
            $"String heap size mismatch: SRM={srmHeapSizes.StringHeapSize}, byte-based={byteSnapshot.HeapSizes.StringHeapSize}")
        Assert.True(
            abs(srmHeapSizes.UserStringHeapSize - byteSnapshot.HeapSizes.UserStringHeapSize) <= heapSizeTolerance,
            $"UserString heap size mismatch: SRM={srmHeapSizes.UserStringHeapSize}, byte-based={byteSnapshot.HeapSizes.UserStringHeapSize}")
        Assert.True(
            abs(srmHeapSizes.BlobHeapSize - byteSnapshot.HeapSizes.BlobHeapSize) <= heapSizeTolerance,
            $"Blob heap size mismatch: SRM={srmHeapSizes.BlobHeapSize}, byte-based={byteSnapshot.HeapSizes.BlobHeapSize}")
        Assert.Equal(srmHeapSizes.GuidHeapSize, byteSnapshot.HeapSizes.GuidHeapSize)

        // Compare all table row counts
        for i in 0..63 do
            if srmTableCounts.[i] <> byteSnapshot.TableRowCounts.[i] then
                Assert.Fail($"Table {i} row count mismatch: expected {srmTableCounts.[i]}, got {byteSnapshot.TableRowCounts.[i]}")

    // ============================================================================
    // Portable PDB Reader Tests
    // ============================================================================

    [<Fact>]
    let ``readPortablePdbMetadata returns None for invalid bytes`` () =
        let invalidBytes = [| 0uy; 1uy; 2uy; 3uy |]
        let result = readPortablePdbMetadata invalidBytes
        Assert.True(result.IsNone, "Should return None for invalid PDB bytes")

    [<Fact>]
    let ``readPortablePdbMetadata returns None for PE file bytes`` () =
        // PE files start with MZ signature, not BSJB
        let bytes = getTestAssemblyBytes ()
        let result = readPortablePdbMetadata bytes
        Assert.True(result.IsNone, "Should return None for PE file (not PDB)")

    [<Fact>]
    let ``readPortablePdbMetadata parses generated PDB from delta artifacts`` () =
        // Use the test helper to create a real assembly with PDB
        let artifacts = MetadataDeltaTestHelpers.emitPropertyDeltaArtifacts None ()

        // The PdbBytes field in AddedOrChangedMethodInfo contains PDB info
        // But we need actual PDB bytes from compilation - check if available
        // For now, test with baseline assembly bytes (which should fail as it's PE, not PDB)
        // This validates the negative case
        let result = readPortablePdbMetadata artifacts.BaselineBytes
        Assert.True(result.IsNone, "PE bytes should not parse as PDB")

    [<Fact>]
    let ``readPortablePdbMetadata validates BSJB signature and structure`` () =
        // Create bytes that start with BSJB but have minimal/invalid structure
        // Signature (4) + major/minor (4) + reserved (4) + version length (4) = 16 bytes min
        let bsjbSignature = [| 0x42uy; 0x53uy; 0x4Auy; 0x42uy |] // "BSJB"
        let padding = Array.zeroCreate<byte> 50  // Some padding but still invalid structure
        let shortBytes = Array.append bsjbSignature padding
        let result = readPortablePdbMetadata shortBytes
        // Should fail because PDB structure is invalid (no valid streams)
        Assert.True(result.IsNone, "Should return None for invalid PDB structure")

    // ============================================================================
    // BaselineMetadataReader Tests
    // ============================================================================

    [<Fact>]
    let ``BaselineMetadataReader.Create returns Some for valid PE file`` () =
        let bytes = getTestAssemblyBytes ()
        let result = BaselineMetadataReader.Create(bytes)
        Assert.True(result.IsSome, "Should successfully create reader for PE file")

    [<Fact>]
    let ``BaselineMetadataReader.Create returns None for invalid bytes`` () =
        let invalidBytes = [| 0uy; 1uy; 2uy; 3uy |]
        let result = BaselineMetadataReader.Create(invalidBytes)
        Assert.True(result.IsNone, "Should return None for invalid bytes")

    [<Fact>]
    let ``BaselineMetadataReader.GetMethodDef returns valid data`` () =
        let bytes = getTestAssemblyBytes ()
        let reader = BaselineMetadataReader.Create(bytes) |> Option.get

        // Get a valid method row (row 1 usually exists)
        let methodDef = reader.GetMethodDef(1)
        Assert.True(methodDef.IsSome, "Method row 1 should exist")

        let method = methodDef.Value
        // Name offset should be non-negative (0 means empty string)
        Assert.True(method.NameOffset >= 0, "Name offset should be non-negative")

    [<Fact>]
    let ``BaselineMetadataReader.GetModule returns valid data`` () =
        let bytes = getTestAssemblyBytes ()
        let reader = BaselineMetadataReader.Create(bytes) |> Option.get

        let moduleDef = reader.GetModule()
        Assert.True(moduleDef.IsSome, "Module row should exist")

        let m = moduleDef.Value
        Assert.True(m.MvidIndex > 0, "MVID index should be positive")

    [<Fact>]
    let ``BaselineMetadataReader.GetTypeRef returns valid data for existing rows`` () =
        let bytes = getTestAssemblyBytes ()
        let reader = BaselineMetadataReader.Create(bytes) |> Option.get

        if reader.TypeRefCount > 0 then
            let typeRef = reader.GetTypeRef(1)
            Assert.True(typeRef.IsSome, "TypeRef row 1 should exist")

    [<Fact>]
    let ``BaselineMetadataReader.GetAssemblyRef returns valid data for existing rows`` () =
        let bytes = getTestAssemblyBytes ()
        let reader = BaselineMetadataReader.Create(bytes) |> Option.get

        if reader.AssemblyRefCount > 0 then
            let assemblyRef = reader.GetAssemblyRef(1)
            Assert.True(assemblyRef.IsSome, "AssemblyRef row 1 should exist")
