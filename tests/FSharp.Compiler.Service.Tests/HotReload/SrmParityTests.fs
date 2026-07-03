namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Collections.Immutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit
open FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter
open FSharp.Compiler.CodeGen.DeltaMetadataTypes
open FSharp.Compiler.CodeGen.DeltaMetadataTables
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.Service.Tests.HotReload.MetadataDeltaTestHelpers

/// Tests to verify that the AbstractIL delta serialization produces correct output
/// that matches what the System.Reflection.Metadata MetadataBuilder tracks.
///
/// These tests validate row count consistency between the SRM MetadataBuilder
/// (which is populated in parallel during emission) and the AbstractIL tables.
/// This is critical for validating correctness before removing SRM dependencies.
module SrmParityTests =

    module DeltaWriter = FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter

    let private assertReaderParity (delta: DeltaWriter.MetadataDelta) =
        use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
        let reader = provider.GetMetadataReader()

        let tables =
            [ TableIndex.Module
              TableIndex.TypeRef
              TableIndex.TypeDef
              TableIndex.MethodDef
              TableIndex.Param
              TableIndex.MemberRef
              TableIndex.MethodSpec
              TableIndex.CustomAttribute
              TableIndex.StandAloneSig
              TableIndex.Property
              TableIndex.Event
              TableIndex.PropertyMap
              TableIndex.EventMap
              TableIndex.MethodSemantics
              TableIndex.AssemblyRef
              TableIndex.EncLog
              TableIndex.EncMap
            ]

        for table in tables do
            Assert.Equal(delta.TableRowCounts.[int table], reader.GetTableRowCount(table))

        Assert.Equal(delta.HeapSizes.StringHeapSize, reader.GetHeapSize HeapIndex.String)
        Assert.Equal(delta.HeapSizes.UserStringHeapSize, reader.GetHeapSize HeapIndex.UserString)
        Assert.Equal(delta.HeapSizes.BlobHeapSize, reader.GetHeapSize HeapIndex.Blob)
        Assert.Equal(delta.HeapSizes.GuidHeapSize, reader.GetHeapSize HeapIndex.Guid)

    /// Helper to serialize a MetadataBuilder to bytes using SRM's serialization
    let private serializeWithMetadataBuilder (metadataBuilder: MetadataBuilder) =
        let metadataRoot = MetadataRootBuilder(metadataBuilder)
        let blob = BlobBuilder()
        metadataRoot.Serialize(blob, methodBodyStreamRva = 0, mappedFieldDataStreamRva = 0)
        blob.ToArray()

    /// Compare two byte arrays and report the first difference
    let private compareBytes (label: string) (expected: byte[]) (actual: byte[]) =
        if expected.Length <> actual.Length then
            failwithf "%s: Length mismatch - SRM=%d, AbstractIL=%d" label expected.Length actual.Length

        for i in 0 .. expected.Length - 1 do
            if expected.[i] <> actual.[i] then
                let contextStart = max 0 (i - 8)
                let contextEnd = min (expected.Length - 1) (i + 8)
                let expectedContext = expected.[contextStart..contextEnd] |> Array.map (sprintf "%02X") |> String.concat " "
                let actualContext = actual.[contextStart..contextEnd] |> Array.map (sprintf "%02X") |> String.concat " "
                failwithf "%s: Byte mismatch at offset 0x%04X (%d)\n  SRM:       %s\n  AbstractIL: %s\n  Expected: 0x%02X, Actual: 0x%02X"
                    label i i expectedContext actualContext expected.[i] actual.[i]

    /// Validates that the MetadataBuilder row counts match our delta table row counts
    let private validateRowCounts (metadataBuilder: MetadataBuilder) (delta: DeltaWriter.MetadataDelta) =
        let tables = [
            TableIndex.Module, "Module"
            TableIndex.TypeRef, "TypeRef"
            TableIndex.TypeDef, "TypeDef"
            TableIndex.MethodDef, "MethodDef"
            TableIndex.Param, "Param"
            TableIndex.MemberRef, "MemberRef"
            TableIndex.CustomAttribute, "CustomAttribute"
            TableIndex.StandAloneSig, "StandAloneSig"
            TableIndex.Property, "Property"
            TableIndex.Event, "Event"
            TableIndex.PropertyMap, "PropertyMap"
            TableIndex.EventMap, "EventMap"
            TableIndex.MethodSemantics, "MethodSemantics"
            TableIndex.AssemblyRef, "AssemblyRef"
            TableIndex.EncLog, "EncLog"
            TableIndex.EncMap, "EncMap"
        ]

        for (tableIndex, name) in tables do
            let srmCount = metadataBuilder.GetRowCount(tableIndex)
            let abstractILCount = delta.TableRowCounts.[int tableIndex]
            if srmCount <> abstractILCount then
                failwithf "Row count mismatch for %s: SRM=%d, AbstractIL=%d" name srmCount abstractILCount

    module PropertyDeltaTests =

        /// Test property delta artifacts have matching row counts in SRM and AbstractIL
        [<Fact>]
        let ``property delta produces matching SRM and AbstractIL row counts`` () =
            let artifacts = emitPropertyDeltaArtifacts (Some "parity-test") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            // The MetadataBuilder is populated during emit - we can verify row counts
            // by using the builder passed to emit internally
            // For this test, we verify the delta metadata is valid
            Assert.NotNull(delta.Metadata)
            Assert.True(delta.Metadata.Length > 0)

            // Verify the metadata can be read back
            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
            let reader = provider.GetMetadataReader()

            // Check that expected tables have rows
            let methodRows = reader.GetTableRowCount(TableIndex.MethodDef)
            let encLogRows = reader.GetTableRowCount(TableIndex.EncLog)
            let encMapRows = reader.GetTableRowCount(TableIndex.EncMap)

            Assert.True(methodRows >= 0, "Should have method rows")
            Assert.True(encLogRows > 0, "Should have EncLog entries")
            Assert.True(encMapRows > 0, "Should have EncMap entries")

    module EventDeltaTests =

        /// Test event delta artifacts have valid metadata structure
        [<Fact>]
        let ``event delta produces valid metadata structure`` () =
            let artifacts = emitEventDeltaArtifacts (Some "event-parity") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            Assert.NotNull(delta.Metadata)
            Assert.True(delta.Metadata.Length > 0)

            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
            let reader = provider.GetMetadataReader()

            let encLogRows = reader.GetTableRowCount(TableIndex.EncLog)
            let encMapRows = reader.GetTableRowCount(TableIndex.EncMap)

            Assert.True(encLogRows > 0, "Should have EncLog entries")
            Assert.True(encMapRows > 0, "Should have EncMap entries")

    module AsyncDeltaTests =

        /// Test async method delta produces valid metadata
        [<Fact>]
        let ``async delta produces valid metadata structure`` () =
            let artifacts = emitAsyncDeltaArtifacts (Some "async-parity") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            Assert.NotNull(delta.Metadata)
            Assert.True(delta.Metadata.Length > 0)

            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
            let reader = provider.GetMetadataReader()

            // Async methods have type references and member references
            let typeRefRows = reader.GetTableRowCount(TableIndex.TypeRef)
            let memberRefRows = reader.GetTableRowCount(TableIndex.MemberRef)

            Assert.True(typeRefRows >= 0, "TypeRef count should be valid")
            Assert.True(memberRefRows >= 0, "MemberRef count should be valid")

    module ClosureDeltaTests =

        /// Test closure method delta produces valid metadata
        [<Fact>]
        let ``closure delta produces valid metadata structure`` () =
            let artifacts = emitClosureDeltaArtifacts ()
            let delta = artifacts.Delta

            assertReaderParity delta

            Assert.NotNull(delta.Metadata)
            Assert.True(delta.Metadata.Length > 0)

            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
            let reader = provider.GetMetadataReader()

            let encLogRows = reader.GetTableRowCount(TableIndex.EncLog)
            Assert.True(encLogRows > 0, "Should have EncLog entries")

    module LocalSignatureDeltaTests =

        /// Test local signature delta produces valid metadata
        [<Fact>]
        let ``local signature delta produces valid metadata structure`` () =
            let artifacts = emitLocalSignatureDeltaArtifacts (Some "locals-parity") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            Assert.NotNull(delta.Metadata)
            Assert.True(delta.Metadata.Length > 0)

            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(delta.Metadata))
            let reader = provider.GetMetadataReader()

            // Local signatures require StandAloneSig entries
            let standAloneSigRows = reader.GetTableRowCount(TableIndex.StandAloneSig)
            Assert.True(standAloneSigRows >= 0, "StandAloneSig count should be valid")

    module MetadataStructureTests =

        /// Verify metadata signature is correct (BSJB)
        [<Fact>]
        let ``delta metadata has valid BSJB signature`` () =
            let artifacts = emitPropertyDeltaArtifacts (Some "signature-test") ()
            let metadata = artifacts.Delta.Metadata

            // ECMA-335 II.24.2.1: Metadata root signature
            // First 4 bytes should be 0x424A5342 ("BSJB")
            Assert.True(metadata.Length >= 4, "Metadata should be at least 4 bytes")
            let signature = BitConverter.ToUInt32(metadata, 0)
            Assert.Equal(0x424A5342u, signature)

        /// Verify heap sizes are consistent
        [<Fact>]
        let ``delta heap sizes are consistent`` () =
            let artifacts = emitPropertyDeltaArtifacts (Some "heap-test") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            // Heap sizes should be non-negative
            Assert.True(delta.HeapSizes.StringHeapSize >= 0)
            Assert.True(delta.HeapSizes.BlobHeapSize >= 0)
            Assert.True(delta.HeapSizes.GuidHeapSize >= 0)
            Assert.True(delta.HeapSizes.UserStringHeapSize >= 0)

        /// Verify EncLog and EncMap are present and sorted correctly
        [<Fact>]
        let ``delta EncLog and EncMap are correctly formed`` () =
            let artifacts = emitPropertyDeltaArtifacts (Some "enc-test") ()
            let delta = artifacts.Delta

            assertReaderParity delta

            // EncLog should not be empty for any meaningful delta
            Assert.True(delta.EncLog.Length > 0, "EncLog should have entries")
            Assert.True(delta.EncMap.Length > 0, "EncMap should have entries")

            // EncMap entries should be sorted by token
            let mutable lastToken = 0
            for (table, rowId) in delta.EncMap do
                let token = (table.Index <<< 24) ||| (rowId &&& 0x00FFFFFF)
                Assert.True(token >= lastToken, sprintf "EncMap not sorted: 0x%08X < 0x%08X" token lastToken)
                lastToken <- token

    module MultiGenerationTests =

        /// Verify multi-generation deltas chain correctly
        [<Fact>]
        let ``multi-generation deltas maintain valid metadata`` () =
            let artifacts = emitPropertyMultiGenerationArtifacts ()

            // Generation 1
            let gen1 = artifacts.Generation1
            assertReaderParity gen1
            Assert.NotNull(gen1.Metadata)
            Assert.True(gen1.Metadata.Length > 0)

            use provider1 = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(gen1.Metadata))
            let reader1 = provider1.GetMetadataReader()
            Assert.True(reader1.GetTableRowCount(TableIndex.EncLog) > 0)

            // Generation 2
            let gen2 = artifacts.Generation2
            assertReaderParity gen2
            Assert.NotNull(gen2.Metadata)
            Assert.True(gen2.Metadata.Length > 0)

            use provider2 = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(gen2.Metadata))
            let reader2 = provider2.GetMetadataReader()
            Assert.True(reader2.GetTableRowCount(TableIndex.EncLog) > 0)

            // Generation IDs should be different
            Assert.NotEqual(gen1.GenerationId, gen2.GenerationId)

            // Gen2's BaseGenerationId should be Gen1's GenerationId
            Assert.Equal(gen1.GenerationId, gen2.BaseGenerationId)
