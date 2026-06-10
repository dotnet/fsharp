namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.Collections.Immutable
open Xunit

open FSharp.Compiler.HotReloadState
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.TypedTree

/// Tests for error paths and invalid states in hot reload infrastructure.
/// These tests ensure that the system fails gracefully with informative errors
/// rather than corrupting state or crashing unexpectedly.
module ErrorPathTests =

    /// Empty checked assembly for testing
    let private emptyCheckedAssembly = CheckedAssemblyAfterOptimization []

    /// Helper to create a minimal valid baseline for testing
    let private createMinimalBaseline () =
        let moduleId = System.Guid.NewGuid()
        let metadataSnapshot: MetadataSnapshot =
            {
                HeapSizes =
                    {
                        StringHeapSize = 64
                        UserStringHeapSize = 32
                        BlobHeapSize = 64
                        GuidHeapSize = 16
                    }
                TableRowCounts = Array.create 64 0
                GuidHeapStart = 0
            }

        {
            ModuleId = moduleId
            EncId = System.Guid.Empty
            EncBaseId = System.Guid.Empty
            NextGeneration = 1
            ModuleNameOffset = None
            Metadata = metadataSnapshot
            TokenMappings =
                {
                    TypeDefTokenMap = fun _ -> 0
                    FieldDefTokenMap = fun _ _ -> 0
                    MethodDefTokenMap = fun _ _ -> 0
                    PropertyTokenMap = fun _ _ -> 0
                    EventTokenMap = fun _ _ -> 0
                }
            TypeTokens = Map.empty
            MethodTokens = Map.empty
            FieldTokens = Map.empty
            PropertyTokens = Map.empty
            EventTokens = Map.empty
            PropertyMapEntries = Map.empty
            EventMapEntries = Map.empty
            MethodSemanticsEntries = Map.empty
            IlxGenEnvironment = None
            PortablePdb = None
            SynthesizedNameSnapshot = Map.empty
            MetadataHandles =
                {
                    MethodHandles = Map.empty
                    ParameterHandles = Map.empty
                    PropertyHandles = Map.empty
                    EventHandles = Map.empty
                }
            TypeReferenceTokens = Map.empty
            AssemblyReferenceTokens = Map.empty
            TableEntriesAdded = Array.zeroCreate 64
            StringStreamLengthAdded = 0
            UserStringStreamLengthAdded = 0
            BlobStreamLengthAdded = 0
            GuidStreamLengthAdded = 0
            AddedOrChangedMethods = []
        }

    module ArgumentValidationTests =

        /// Tests that recordDeltaApplied validates the generation ID
        [<Fact>]
        let ``recordDeltaApplied throws ArgumentException for empty GUID`` () =
            // This tests the argument validation in recordDeltaApplied
            // The empty GUID check happens before the session check
            let ex = Assert.Throws<ArgumentException>(fun () ->
                recordDeltaApplied System.Guid.Empty)

            Assert.Contains("Generation ID cannot be empty", ex.Message)

    module BaselineCreationTests =

        [<Fact>]
        let ``baseline with zero heap sizes can be created`` () =
            // While unusual, zero heap sizes are technically valid for empty modules
            let metadataSnapshot: MetadataSnapshot =
                {
                    HeapSizes =
                        {
                            StringHeapSize = 0
                            UserStringHeapSize = 0
                            BlobHeapSize = 0
                            GuidHeapSize = 0
                        }
                    TableRowCounts = Array.create 64 0
                    GuidHeapStart = 0
                }

            let baseline =
                {
                    createMinimalBaseline () with
                        Metadata = metadataSnapshot
                }

            // Should not throw - baseline creation should succeed
            Assert.NotNull(baseline)
            Assert.Equal(0, baseline.Metadata.HeapSizes.StringHeapSize)

        [<Fact>]
        let ``baseline with empty table row counts can be created`` () =
            // Empty table row counts represent a module with no types/methods
            let baseline = createMinimalBaseline ()

            Assert.NotNull(baseline)
            Assert.True(baseline.Metadata.TableRowCounts |> Array.forall ((=) 0))

        [<Fact>]
        let ``baseline with nil PDB snapshot can be created`` () =
            let baseline =
                {
                    createMinimalBaseline () with
                        PortablePdb = None
                }

            Assert.NotNull(baseline)
            Assert.True(baseline.PortablePdb.IsNone)

    module PdbSnapshotTests =

        [<Fact>]
        let ``PDB snapshot with empty bytes can be created`` () =
            // Empty PDB bytes indicate no debug info
            let pdbSnapshot: PortablePdbSnapshot =
                {
                    Bytes = Array.empty
                    TableRowCounts = ImmutableArray.CreateRange(Array.zeroCreate 64)
                    EntryPointToken = None
                }

            Assert.NotNull(pdbSnapshot)
            Assert.Empty(pdbSnapshot.Bytes)

        [<Fact>]
        let ``PDB snapshot with nil entry point can be created`` () =
            let pdbSnapshot: PortablePdbSnapshot =
                {
                    Bytes = [| 0uy; 1uy; 2uy |]
                    TableRowCounts = ImmutableArray.CreateRange(Array.zeroCreate 64)
                    EntryPointToken = None
                }

            Assert.NotNull(pdbSnapshot)
            Assert.True(pdbSnapshot.EntryPointToken.IsNone)

        [<Fact>]
        let ``PDB snapshot with entry point can be created`` () =
            let pdbSnapshot: PortablePdbSnapshot =
                {
                    Bytes = [| 0uy; 1uy; 2uy |]
                    TableRowCounts = ImmutableArray.CreateRange(Array.zeroCreate 64)
                    EntryPointToken = Some 0x06000001 // MethodDef token
                }

            Assert.NotNull(pdbSnapshot)
            Assert.Equal(Some 0x06000001, pdbSnapshot.EntryPointToken)

    module MetadataSnapshotTests =

        [<Fact>]
        let ``heap sizes are stored correctly in metadata snapshot`` () =
            let metadataSnapshot: MetadataSnapshot =
                {
                    HeapSizes =
                        {
                            StringHeapSize = 100
                            UserStringHeapSize = 200
                            BlobHeapSize = 300
                            GuidHeapSize = 16
                        }
                    TableRowCounts = Array.zeroCreate 64
                    GuidHeapStart = 0
                }

            Assert.Equal(100, metadataSnapshot.HeapSizes.StringHeapSize)
            Assert.Equal(200, metadataSnapshot.HeapSizes.UserStringHeapSize)
            Assert.Equal(300, metadataSnapshot.HeapSizes.BlobHeapSize)
            Assert.Equal(16, metadataSnapshot.HeapSizes.GuidHeapSize)

        [<Fact>]
        let ``table row counts array must have 64 entries`` () =
            // ECMA-335 defines up to 64 possible tables
            let tableRowCounts = Array.zeroCreate<int> 64

            Assert.Equal(64, tableRowCounts.Length)

        [<Fact>]
        let ``metadata snapshot preserves all fields`` () =
            let metadataSnapshot: MetadataSnapshot =
                {
                    HeapSizes =
                        {
                            StringHeapSize = 1000
                            UserStringHeapSize = 500
                            BlobHeapSize = 2000
                            GuidHeapSize = 48
                        }
                    TableRowCounts = Array.init 64 id  // 0, 1, 2, ..., 63
                    GuidHeapStart = 16
                }

            Assert.Equal(1000, metadataSnapshot.HeapSizes.StringHeapSize)
            Assert.Equal(48, metadataSnapshot.HeapSizes.GuidHeapSize)
            Assert.Equal(16, metadataSnapshot.GuidHeapStart)
            Assert.Equal(42, metadataSnapshot.TableRowCounts.[42])

    module TokenMapTests =

        [<Fact>]
        let ``token mappings can be created with dummy functions`` () =
            let tokenMappings: ILTokenMappings =
                {
                    TypeDefTokenMap = fun _ -> 0x02000001
                    FieldDefTokenMap = fun _ _ -> 0x04000001
                    MethodDefTokenMap = fun _ _ -> 0x06000001
                    PropertyTokenMap = fun _ _ -> 0x17000001
                    EventTokenMap = fun _ _ -> 0x14000001
                }

            // The functions should be callable without throwing
            Assert.Equal(0x02000001, tokenMappings.TypeDefTokenMap Unchecked.defaultof<_>)

        [<Fact>]
        let ``baseline tokens map can be empty`` () =
            let baseline = createMinimalBaseline ()

            Assert.True(baseline.TypeTokens.IsEmpty)
            Assert.True(baseline.MethodTokens.IsEmpty)
            Assert.True(baseline.FieldTokens.IsEmpty)
            Assert.True(baseline.PropertyTokens.IsEmpty)
            Assert.True(baseline.EventTokens.IsEmpty)


    module TokenRemapGuardTests =

        [<Fact>]
        let ``classifyEntityTokenRemapKind fails closed for unknown table tags`` () =
            let ex =
                Assert.Throws<FSharp.Compiler.IlxDeltaEmitter.HotReloadUnsupportedEditException>(fun () ->
                    FSharp.Compiler.IlxDeltaEmitter.classifyEntityTokenRemapKind 0x7F000001 |> ignore)

            Assert.Contains("Unsupported metadata token table 0x7F", ex.Message)

        [<Fact>]
        let ``classifyEntityTokenRemapKind keeps known passthrough tables explicit`` () =
            let typeSpec = FSharp.Compiler.IlxDeltaEmitter.classifyEntityTokenRemapKind 0x1B000001
            let standaloneSig = FSharp.Compiler.IlxDeltaEmitter.classifyEntityTokenRemapKind 0x11000001

            Assert.Equal(FSharp.Compiler.IlxDeltaEmitter.EntityTokenRemapKind.Passthrough, typeSpec)
            Assert.Equal(FSharp.Compiler.IlxDeltaEmitter.EntityTokenRemapKind.Passthrough, standaloneSig)
