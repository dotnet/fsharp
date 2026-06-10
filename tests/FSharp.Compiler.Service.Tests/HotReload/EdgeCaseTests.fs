namespace FSharp.Compiler.Service.Tests.HotReload

open System.Reflection.Metadata.Ecma335
open Xunit

open FSharp.Compiler.CodeGen.DeltaIndexSizing
open FSharp.Compiler.AbstractIL.ILBinaryWriter

/// Tests for edge cases in hot reload infrastructure.
/// These tests validate behavior at boundary conditions like large row counts,
/// heap size thresholds, and index size transitions.
module EdgeCaseTests =

    /// Helper to create heap sizes
    let private createHeapSizes string userString blob guid =
        { StringHeapSize = string
          UserStringHeapSize = userString
          BlobHeapSize = blob
          GuidHeapSize = guid }

    /// Helper to create table row counts with specific values
    let private createTableRowCounts (entries: (TableIndex * int) list) =
        let counts = Array.zeroCreate 64
        for (table, count) in entries do
            counts.[int table] <- count
        counts

    module IndexSizeThresholdTests =

        /// 0x10000 (65536) is the threshold where indices switch from 2 bytes to 4 bytes
        let private threshold = 0x10000

        [<Fact>]
        let ``string heap under threshold uses small index`` () =
            let heapSizes = createHeapSizes (threshold - 1) 0 0 0
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.StringsBig, "String heap under threshold should use small index")

        [<Fact>]
        let ``string heap at threshold uses big index`` () =
            let heapSizes = createHeapSizes threshold 0 0 0
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.StringsBig, "String heap at threshold should use big index")

        [<Fact>]
        let ``blob heap under threshold uses small index`` () =
            let heapSizes = createHeapSizes 0 0 (threshold - 1) 0
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.BlobsBig, "Blob heap under threshold should use small index")

        [<Fact>]
        let ``blob heap at threshold uses big index`` () =
            let heapSizes = createHeapSizes 0 0 threshold 0
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.BlobsBig, "Blob heap at threshold should use big index")

        [<Fact>]
        let ``guid heap under threshold uses small index`` () =
            let heapSizes = createHeapSizes 0 0 0 (threshold - 1)
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.GuidsBig, "GUID heap under threshold should use small index")

        [<Fact>]
        let ``guid heap at threshold uses big index`` () =
            let heapSizes = createHeapSizes 0 0 0 threshold
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.GuidsBig, "GUID heap at threshold should use big index")

    module SimpleIndexTests =

        let private threshold = 0x10000

        [<Fact>]
        let ``TypeDef table under threshold uses small index`` () =
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, threshold - 1) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.SimpleIndexBig.[int TableIndex.TypeDef], "TypeDef under threshold should use small index")

        [<Fact>]
        let ``TypeDef table at threshold uses big index`` () =
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, threshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.SimpleIndexBig.[int TableIndex.TypeDef], "TypeDef at threshold should use big index")

        [<Fact>]
        let ``MethodDef table under threshold uses small index`` () =
            let tableRowCounts = createTableRowCounts [ (TableIndex.MethodDef, threshold - 1) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.SimpleIndexBig.[int TableIndex.MethodDef], "MethodDef under threshold should use small index")

        [<Fact>]
        let ``MethodDef table at threshold uses big index`` () =
            let tableRowCounts = createTableRowCounts [ (TableIndex.MethodDef, threshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.SimpleIndexBig.[int TableIndex.MethodDef], "MethodDef at threshold should use big index")

        [<Fact>]
        let ``external row counts contribute to threshold`` () =
            // Local = 30000, External = 40000, Total = 70000 > threshold
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, 30000) ]
            let externalRowCounts = createTableRowCounts [ (TableIndex.TypeDef, 40000) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts externalRowCounts heapSizes false

            Assert.True(sizes.SimpleIndexBig.[int TableIndex.TypeDef],
                "Combined local + external rows exceeding threshold should use big index")

        [<Fact>]
        let ``external row counts under threshold use small index`` () =
            // Local = 30000, External = 30000, Total = 60000 < threshold
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, 30000) ]
            let externalRowCounts = createTableRowCounts [ (TableIndex.TypeDef, 30000) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts externalRowCounts heapSizes false

            Assert.False(sizes.SimpleIndexBig.[int TableIndex.TypeDef],
                "Combined rows under threshold should use small index")

    module CodedIndexTests =

        [<Fact>]
        let ``TypeDefOrRef with 2 tag bits has correct threshold`` () =
            // TypeDefOrRef uses 2 tag bits, so threshold is 2^(16-2) = 16384
            let codedThreshold = pown 2 (16 - 2)  // 16384
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, codedThreshold - 1) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.TypeDefOrRefBig, "TypeDefOrRef under coded threshold should use small index")

        [<Fact>]
        let ``TypeDefOrRef at coded threshold uses big index`` () =
            let codedThreshold = pown 2 (16 - 2)  // 16384
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, codedThreshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.TypeDefOrRefBig, "TypeDefOrRef at coded threshold should use big index")

        [<Fact>]
        let ``MemberRefParent with 3 tag bits has correct threshold`` () =
            // MemberRefParent uses 3 tag bits, so threshold is 2^(16-3) = 8192
            let codedThreshold = pown 2 (16 - 3)  // 8192
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeRef, codedThreshold - 1) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.MemberRefParentBig, "MemberRefParent under coded threshold should use small index")

        [<Fact>]
        let ``MemberRefParent at coded threshold uses big index`` () =
            let codedThreshold = pown 2 (16 - 3)  // 8192
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeRef, codedThreshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.MemberRefParentBig, "MemberRefParent at coded threshold should use big index")

        [<Fact>]
        let ``HasCustomAttribute with 5 tag bits has correct threshold`` () =
            // HasCustomAttribute uses 5 tag bits, so threshold is 2^(16-5) = 2048
            let codedThreshold = pown 2 (16 - 5)  // 2048
            let tableRowCounts = createTableRowCounts [ (TableIndex.MethodDef, codedThreshold - 1) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.HasCustomAttributeBig, "HasCustomAttribute under coded threshold should use small index")

        [<Fact>]
        let ``HasCustomAttribute at coded threshold uses big index`` () =
            let codedThreshold = pown 2 (16 - 5)  // 2048
            let tableRowCounts = createTableRowCounts [ (TableIndex.MethodDef, codedThreshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.HasCustomAttributeBig, "HasCustomAttribute at coded threshold should use big index")

        [<Fact>]
        let ``any table in coded index group exceeding threshold triggers big index`` () =
            // TypeDefOrRef includes TypeDef, TypeRef, TypeSpec
            // If any one exceeds threshold, coded index is big
            let codedThreshold = pown 2 (16 - 2)  // 16384
            let tableRowCounts = createTableRowCounts [
                (TableIndex.TypeDef, 1)
                (TableIndex.TypeRef, 1)
                (TableIndex.TypeSpec, codedThreshold)  // Only TypeSpec exceeds
            ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.TypeDefOrRefBig, "Any table exceeding threshold should trigger big coded index")

    module EncDeltaTests =

        [<Fact>]
        let ``EncDelta mode forces all indices to big`` () =
            // In EnC delta mode (isEncDelta=true), all indices are big regardless of counts
            let tableRowCounts = Array.zeroCreate 64
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes true

            Assert.True(sizes.StringsBig, "EncDelta should force strings big")
            Assert.True(sizes.BlobsBig, "EncDelta should force blobs big")
            Assert.True(sizes.GuidsBig, "EncDelta should force GUIDs big")
            Assert.True(sizes.TypeDefOrRefBig, "EncDelta should force TypeDefOrRef big")
            Assert.True(sizes.MemberRefParentBig, "EncDelta should force MemberRefParent big")
            Assert.True(sizes.HasCustomAttributeBig, "EncDelta should force HasCustomAttribute big")

        [<Fact>]
        let ``EncDelta mode forces simple indices big`` () =
            let tableRowCounts = Array.zeroCreate 64
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes true

            // All simple indices should be big in EncDelta mode
            for i in 0..63 do
                Assert.True(sizes.SimpleIndexBig.[i], $"SimpleIndex[{i}] should be big in EncDelta mode")

    module BoundaryTests =

        [<Fact>]
        let ``zero row counts produce small indices`` () =
            let tableRowCounts = Array.zeroCreate 64
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.StringsBig)
            Assert.False(sizes.BlobsBig)
            Assert.False(sizes.GuidsBig)
            Assert.False(sizes.TypeDefOrRefBig)
            Assert.False(sizes.MemberRefParentBig)

        [<Fact>]
        let ``maximum heap size produces big indices`` () =
            let maxHeap = System.Int32.MaxValue
            let heapSizes = createHeapSizes maxHeap maxHeap maxHeap maxHeap
            let tableRowCounts = Array.zeroCreate 64
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.StringsBig)
            Assert.True(sizes.BlobsBig)
            Assert.True(sizes.GuidsBig)

        [<Fact>]
        let ``maximum row counts produce big indices`` () =
            let tableRowCounts = Array.create 64 System.Int32.MaxValue
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.TypeDefOrRefBig)
            Assert.True(sizes.MemberRefParentBig)
            Assert.True(sizes.HasCustomAttributeBig)
            Assert.True(sizes.HasDeclSecurityBig)

        [<Fact>]
        let ``exactly at threshold minus one is still small`` () =
            // Boundary condition: threshold - 1 should be small
            let threshold = 0x10000
            let tableRowCounts = createTableRowCounts [ (TableIndex.TypeDef, threshold - 1) ]
            let heapSizes = createHeapSizes (threshold - 1) 0 (threshold - 1) (threshold - 1)
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.False(sizes.StringsBig, "Exactly threshold - 1 should be small")
            Assert.False(sizes.SimpleIndexBig.[int TableIndex.TypeDef], "Row count threshold - 1 should be small")

    module DeepNestingTests =
        open FSharp.Compiler.SynthesizedTypeMaps

        [<Fact>]
        let ``SynthesizedTypeMaps handles 10+ nested closure names`` () =
            let map = FSharpSynthesizedTypeMaps()
            map.BeginSession()

            // Simulate deeply nested closure naming (10+ levels)
            let nestedNames = [
                "lambda"; "lambda"; "lambda"; "lambda"; "lambda"
                "lambda"; "lambda"; "lambda"; "lambda"; "lambda"
                "lambda"; "lambda"  // 12 levels
            ]

            let results = nestedNames |> List.map map.GetOrAddName

            // All should produce valid names without crashing
            Assert.Equal(12, results.Length)
            for name in results do
                Assert.StartsWith("lambda@", name)

        [<Fact>]
        let ``SynthesizedTypeMaps handles 100 unique base names`` () =
            let map = FSharpSynthesizedTypeMaps()
            map.BeginSession()

            // Generate 100 different base names (simulating complex module)
            let baseNames = [| for i in 1..100 -> $"closure{i}" |]
            let results = baseNames |> Array.map map.GetOrAddName

            Assert.Equal(100, results.Length)
            for i in 0..99 do
                Assert.StartsWith($"closure{i+1}@", results.[i])

        [<Fact>]
        let ``SynthesizedTypeMaps snapshot handles 100+ entries`` () =
            let map = FSharpSynthesizedTypeMaps()
            map.BeginSession()

            // Add many entries
            for i in 1..100 do
                map.GetOrAddName $"type{i}" |> ignore

            let snapshot = map.Snapshot |> Seq.toArray
            Assert.True(snapshot.Length >= 100, $"Expected at least 100 entries, got {snapshot.Length}")

    module GenerationTrackingTests =
        open FSharp.Compiler.HotReloadState
        open FSharp.Compiler.HotReloadBaseline
        open FSharp.Compiler.TypedTree

        let private createMinimalBaseline () =
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
                ModuleId = System.Guid.NewGuid()
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

        [<Fact>]
        let ``generation counter handles 100+ increments`` () =
            // Arrange
            clearBaseline ()
            let baseline = createMinimalBaseline ()
            setBaseline baseline (CheckedAssemblyAfterOptimization []) |> ignore

            let initialSession = tryGetSession ()
            let initialGen = initialSession.Value.CurrentGeneration

            // Act - simulate 100+ consecutive committed deltas using pending-update semantics
            let mutable workingBaseline = baseline

            for _ in 1..150 do
                let generationId = System.Guid.NewGuid()

                let stagedBaseline =
                    { workingBaseline with
                        EncId = generationId
                        NextGeneration = workingBaseline.NextGeneration + 1 }

                updateBaseline stagedBaseline
                recordDeltaApplied generationId
                workingBaseline <- stagedBaseline

            // Assert
            let finalSession = tryGetSession ()
            Assert.True(finalSession.IsSome)
            Assert.Equal(initialGen + 150, finalSession.Value.CurrentGeneration)

            clearBaseline ()

    module ParameterCountTests =

        [<Fact>]
        let ``parameter table index handles 256+ entries`` () =
            // Test that we can handle modules with many parameters
            // The Param table uses a simple index, threshold is 65536
            let paramCount = 300  // More than 256 (byte boundary)
            let tableRowCounts = createTableRowCounts [ (TableIndex.Param, paramCount) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            // 300 params is still under 65536, so should use small index
            Assert.False(sizes.SimpleIndexBig.[int TableIndex.Param],
                "300 parameters should still use small index")

        [<Fact>]
        let ``parameter table index switches to big at threshold`` () =
            // Test the threshold for parameter table
            let threshold = 0x10000
            let tableRowCounts = createTableRowCounts [ (TableIndex.Param, threshold) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.SimpleIndexBig.[int TableIndex.Param],
                "65536 parameters should use big index")

    module MethodBodyTests =

        [<Fact>]
        let ``IL method body size calculation handles minimal body`` () =
            // A minimal method body is just a 'ret' instruction (1 byte)
            // Test that we can represent this in the sizing infrastructure
            let tableRowCounts = createTableRowCounts [ (TableIndex.MethodDef, 1) ]
            let heapSizes = createHeapSizes 0 0 1 0  // 1 byte blob for method body
            let sizes = compute tableRowCounts [||] heapSizes false

            // Even minimal bodies should work
            Assert.False(sizes.BlobsBig, "Minimal method body should use small blob index")

        [<Fact>]
        let ``blob heap handles large method body`` () =
            // Test that large method bodies (>64KB) trigger big blob index
            let largeBodySize = 0x20000  // 128KB
            let tableRowCounts = Array.zeroCreate 64
            let heapSizes = createHeapSizes 0 0 largeBodySize 0
            let sizes = compute tableRowCounts [||] heapSizes false

            Assert.True(sizes.BlobsBig, "Large method body should use big blob index")

        [<Fact>]
        let ``StandAloneSig table handles method local variables`` () =
            // Methods with local variables use StandAloneSig table
            let sigCount = 1000  // Many methods with locals
            let tableRowCounts = createTableRowCounts [ (TableIndex.StandAloneSig, sigCount) ]
            let heapSizes = createHeapSizes 0 0 0 0
            let sizes = compute tableRowCounts [||] heapSizes false

            // 1000 signatures is under threshold
            Assert.False(sizes.SimpleIndexBig.[int TableIndex.StandAloneSig],
                "1000 local signatures should use small index")
