namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open Xunit

open FSharp.Compiler.EncMethodDebugInformation

/// Tests for the EnC method debug information blob encoder/decoder
/// (EncMethodDebugInformation.fs), which must be byte-for-byte compatible with
/// Roslyn's EditAndContinueMethodDebugInformation serialization.
module EncMethodDebugInformationTests =

    // -----------------------------------------------------------------------
    // Round-trip properties
    // -----------------------------------------------------------------------

    [<Fact>]
    let ``Empty maps serialize to empty blobs and deserialize to the empty record`` () =
        let info = EncMethodDebugInformation.Empty

        Assert.Empty(serializeLocalSlots info)
        Assert.Empty(serializeLambdaMap info)
        Assert.Empty(serializeStateMachineStates info)

        let decoded = deserialize Array.empty Array.empty Array.empty
        Assert.Equal<EncMethodDebugInformation>(EncMethodDebugInformation.Empty, decoded)

        // Null blobs (absent CDI rows) behave like empty ones.
        let decodedNull = deserialize null null null
        Assert.Equal<EncMethodDebugInformation>(EncMethodDebugInformation.Empty, decodedNull)

    [<Fact>]
    let ``Lambda map with a single closure round-trips`` () =
        let info =
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 0
                Closures = [ { SyntaxOffset = 3 } ] }

        let blob = serializeLambdaMap info
        let methodOrdinal, closures, lambdas = deserializeLambdaMap blob

        Assert.Equal(0, methodOrdinal)
        Assert.Equal<EncClosureInfo list>([ { SyntaxOffset = 3 } ], closures)
        Assert.Empty lambdas

    [<Fact>]
    let ``Lambda map with several lambdas and negative-baseline offsets round-trips`` () =
        // Out-of-order and negative offsets exercise the syntax-offset-baseline record;
        // closure ordinals cover in-range, static (-1) and this-only (-2) lambdas.
        let closures = [ { SyntaxOffset = 12 }; { SyntaxOffset = -7 }; { SyntaxOffset = 3 } ]

        let lambdas =
            [ { SyntaxOffset = 30; ClosureOrdinal = 1 }
              { SyntaxOffset = -7; ClosureOrdinal = StaticClosureOrdinal }
              { SyntaxOffset = 0; ClosureOrdinal = ThisOnlyClosureOrdinal }
              { SyntaxOffset = 5; ClosureOrdinal = 2 } ]

        let info =
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 5
                Closures = closures
                Lambdas = lambdas }

        let blob = serializeLambdaMap info
        let methodOrdinal, decodedClosures, decodedLambdas = deserializeLambdaMap blob

        Assert.Equal(5, methodOrdinal)
        Assert.Equal<EncClosureInfo list>(closures, decodedClosures)
        Assert.Equal<EncLambdaInfo list>(lambdas, decodedLambdas)

    [<Fact>]
    let ``Lambda map golden bytes match the Roslyn encoding`` () =
        // methodOrdinal 0 -> compressed(1); baseline -1 -> compressed(1); one closure at
        // offset 0 -> compressed(1); lambda at offset 5 -> compressed(6) with closure
        // ordinal 0 -> compressed(0 - (-2)) = compressed(2).
        let info =
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 0
                Closures = [ { SyntaxOffset = 0 } ]
                Lambdas = [ { SyntaxOffset = 5; ClosureOrdinal = 0 } ] }

        Assert.Equal<byte[]>([| 0x01uy; 0x01uy; 0x01uy; 0x01uy; 0x06uy; 0x02uy |], serializeLambdaMap info)

    [<Fact>]
    let ``Lambda map rejects closure ordinals outside the valid range`` () =
        let mk ordinal =
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 0
                Closures = [ { SyntaxOffset = 0 } ]
                Lambdas = [ { SyntaxOffset = 1; ClosureOrdinal = ordinal } ] }

        Assert.Throws<ArgumentException>(fun () -> serializeLambdaMap (mk 1) |> ignore) |> ignore
        Assert.Throws<ArgumentException>(fun () -> serializeLambdaMap (mk -3) |> ignore) |> ignore

    [<Fact>]
    let ``Slot map with temps, ordinal-flagged slots and negative offsets round-trips`` () =
        let slots =
            [ EncLocalSlotInfo.Temp
              EncLocalSlotInfo.Slot(0, 10, 0)
              EncLocalSlotInfo.Slot(MaxSerializableLocalKind, -42, 3)
              EncLocalSlotInfo.Temp
              EncLocalSlotInfo.Slot(7, 0, 1) ]

        let info =
            { EncMethodDebugInformation.Empty with
                LocalSlots = slots }

        let blob = serializeLocalSlots info
        Assert.Equal<EncLocalSlotInfo list>(slots, deserializeLocalSlots blob)

        // The baseline record must be present (an offset below -1 exists) and must be
        // the Roslyn marker byte 0xFF followed by compressed(42).
        Assert.Equal(0xFFuy, blob[0])

    [<Fact>]
    let ``Slot map golden bytes match the Roslyn encoding`` () =
        // No offset below -1 -> no baseline record (implicit baseline -1).
        // Temp -> 0x00.
        // Slot(kind 0, offset 0, ordinal 0) -> byte 0x01 (kind+1), compressed(0 - (-1)) = 0x01.
        // Slot(kind 1, offset 2, ordinal 3) -> byte 0x82 (kind+1, bit 7 = has ordinal),
        //   compressed(3), compressed(3).
        let info =
            { EncMethodDebugInformation.Empty with
                LocalSlots =
                    [ EncLocalSlotInfo.Temp
                      EncLocalSlotInfo.Slot(0, 0, 0)
                      EncLocalSlotInfo.Slot(1, 2, 3) ] }

        Assert.Equal<byte[]>([| 0x00uy; 0x01uy; 0x01uy; 0x82uy; 0x03uy; 0x03uy |], serializeLocalSlots info)

    [<Fact>]
    let ``Slot map rejects kinds outside the serializable range`` () =
        let mk kind =
            { EncMethodDebugInformation.Empty with
                LocalSlots = [ EncLocalSlotInfo.Slot(kind, 0, 0) ] }

        Assert.Throws<ArgumentException>(fun () -> serializeLocalSlots (mk -1) |> ignore) |> ignore

        Assert.Throws<ArgumentException>(fun () -> serializeLocalSlots (mk (MaxSerializableLocalKind + 1)) |> ignore)
        |> ignore

    [<Fact>]
    let ``State machine map with negative state numbers round-trips ordered by offset`` () =
        // Input deliberately unsorted; the writer orders entries by syntax offset
        // (stably, so the two entries sharing offset 20 keep their relative order).
        let states =
            [ { StateNumber = -4; SyntaxOffset = 20 }
              { StateNumber = 0; SyntaxOffset = -5 }
              { StateNumber = 3; SyntaxOffset = 20 }
              { StateNumber = 1; SyntaxOffset = 7 } ]

        let info =
            { EncMethodDebugInformation.Empty with
                StateMachineStates = states }

        let expected =
            [ { StateNumber = 0; SyntaxOffset = -5 }
              { StateNumber = 1; SyntaxOffset = 7 }
              { StateNumber = -4; SyntaxOffset = 20 }
              { StateNumber = 3; SyntaxOffset = 20 } ]

        let blob = serializeStateMachineStates info
        Assert.Equal<EncStateMachineStateInfo list>(expected, deserializeStateMachineStates blob)

    [<Fact>]
    let ``Full record round-trips through the three blobs`` () =
        let info =
            { MethodOrdinal = 2
              LocalSlots = [ EncLocalSlotInfo.Slot(0, 4, 0); EncLocalSlotInfo.Temp ]
              Closures = [ { SyntaxOffset = 0 } ]
              Lambdas = [ { SyntaxOffset = 9; ClosureOrdinal = 0 } ]
              StateMachineStates = [ { StateNumber = 0; SyntaxOffset = 9 } ] }

        let decoded =
            deserialize (serializeLocalSlots info) (serializeLambdaMap info) (serializeStateMachineStates info)

        Assert.Equal<EncMethodDebugInformation>(info, decoded)

    // -----------------------------------------------------------------------
    // Occurrence-key packing
    // -----------------------------------------------------------------------

    [<Fact>]
    let ``Occurrence keys pack and unpack ordinal chains`` () =
        // Depth 1: the key is the ordinal itself.
        Assert.Equal(Some 0, tryEncodeOccurrenceKey [ 0 ])
        Assert.Equal(Some 5, tryEncodeOccurrenceKey [ 5 ])
        Assert.Equal(Some 0xFFFF, tryEncodeOccurrenceKey [ 0xFFFF ])
        Assert.Equal<int list>([ 5 ], decodeOccurrenceKey 5)

        // Depth 2: the parent segment is stored biased by one, so [0; 0] never
        // collides with the depth-1 key 0.
        Assert.Equal(Some 0x10000, tryEncodeOccurrenceKey [ 0; 0 ])
        Assert.Equal<int list>([ 0; 0 ], decodeOccurrenceKey 0x10000)
        Assert.Equal(Some 0x40007, tryEncodeOccurrenceKey [ 3; 7 ])
        Assert.Equal<int list>([ 3; 7 ], decodeOccurrenceKey 0x40007)

        // Every encodable chain round-trips ([0x1FFE; 0xFFFD] packs to the maximum
        // key 0x1FFFFFFD that still fits the compressed-integer budget after the
        // baseline adjustment).
        for chain in [ [ 0 ]; [ 42 ]; [ 0xFFFF ]; [ 0; 0 ]; [ 3; 7 ]; [ 0x1FFE; 0xFFFD ] ] do
            match tryEncodeOccurrenceKey chain with
            | Some key -> Assert.Equal<int list>(chain, decodeOccurrenceKey key)
            | None -> failwith $"expected chain %A{chain} to be encodable"

    [<Fact>]
    let ``Occurrence key packing fails closed past its limits`` () =
        // Deeper than two segments.
        Assert.Equal(None, tryEncodeOccurrenceKey [ 1; 2; 3 ])
        // Empty chain.
        Assert.Equal(None, tryEncodeOccurrenceKey [])
        // Ordinal past 16 bits.
        Assert.Equal(None, tryEncodeOccurrenceKey [ 0x10000 ])
        Assert.Equal(None, tryEncodeOccurrenceKey [ 0; 0x10000 ])
        // Parent past the compressed-integer budget (29 bits incl. the bias).
        Assert.Equal(None, tryEncodeOccurrenceKey [ 0x1FFF; 0 ])
        // Packed key past the budget even though both segments are individually valid.
        Assert.Equal(None, tryEncodeOccurrenceKey [ 0x1FFE; 0xFFFF ])
        // Negative ordinals.
        Assert.Equal(None, tryEncodeOccurrenceKey [ -1 ])
        Assert.Equal(None, tryEncodeOccurrenceKey [ -1; 0 ])

    // -----------------------------------------------------------------------
    // Cross-validation against Roslyn-emitted blobs
    // -----------------------------------------------------------------------

    /// C# source with nested capturing lambdas, LINQ lambdas, and an async method, so a
    /// debug build emits all three EnC CDI kinds.
    let private crossValidationSource =
        """
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scratch
{
    public class Lambdas
    {
        public Func<int, int> MakeAdder(int x)
        {
            int y = x + 1;
            Func<int, int> inner = a => a + x + y;
            return b => inner(b) + x;
        }

        public int UseLinq(IEnumerable<int> items, int threshold)
        {
            var filtered = items.Where(i => i > threshold).Select(i => i * 2);
            return filtered.Sum(i => i + threshold);
        }

        public async Task<int> ComputeAsync(int x)
        {
            await Task.Delay(1);
            int y = x * 2;
            await Task.Yield();
            Func<int, int> f = a => a + y;
            return f(x);
        }
    }
}
"""

    /// Builds the cross-validation C# library with the repo SDK (DebugType=portable) and
    /// returns the path of the produced Portable PDB.
    let private buildCSharpScratchPdb () =
        let workDir =
            Path.Combine(Path.GetTempPath(), "fsharp-hotreload-enc-cdi-" + Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory workDir |> ignore
        let projPath = Path.Combine(workDir, "scratch.csproj")
        File.WriteAllText(Path.Combine(workDir, "Scratch.cs"), crossValidationSource)

        File.WriteAllText(
            projPath,
            """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <DebugType>portable</DebugType>
    <Optimize>false</Optimize>
    <DebugSymbols>true</DebugSymbols>
    <Nullable>disable</Nullable>
  </PropertyGroup>
</Project>
"""
        )

        let psi = System.Diagnostics.ProcessStartInfo()
        psi.FileName <- Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", ".dotnet", "dotnet")
        psi.ArgumentList.Add "build"
        psi.ArgumentList.Add projPath
        psi.ArgumentList.Add "-c"
        psi.ArgumentList.Add "Debug"
        psi.ArgumentList.Add "-p:DebugType=portable"
        psi.ArgumentList.Add "-v"
        psi.ArgumentList.Add "m"
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.WorkingDirectory <- workDir

        use p = new System.Diagnostics.Process()
        p.StartInfo <- psi
        p.Start() |> ignore
        let stdout = p.StandardOutput.ReadToEnd()
        let stderr = p.StandardError.ReadToEnd()
        p.WaitForExit()

        if p.ExitCode <> 0 then
            failwith $"dotnet build of the C# scratch library failed: {stdout}\n{stderr}"

        let pdbPath = Path.Combine(workDir, "bin", "Debug", "net10.0", "scratch.pdb")
        Assert.True(File.Exists pdbPath, $"expected portable PDB at {pdbPath}")
        workDir, pdbPath

    /// Reads all CustomDebugInformation rows of the given kind from a portable PDB,
    /// returning (parent method token, blob bytes) pairs.
    let private readCdiBlobs (reader: MetadataReader) (kind: Guid) =
        [ for cdiHandle in reader.CustomDebugInformation do
              let cdi = reader.GetCustomDebugInformation cdiHandle

              if reader.GetGuid cdi.Kind = kind then
                  let parent = MetadataTokens.GetToken cdi.Parent
                  parent, reader.GetBlobBytes cdi.Value ]

    [<Fact>]
    let ``Roslyn-emitted EnC CDI blobs decode and re-encode byte-for-byte`` () =
        let workDir, pdbPath = buildCSharpScratchPdb ()

        try
            use stream = File.OpenRead pdbPath
            use provider = MetadataReaderProvider.FromPortablePdbStream stream
            let reader = provider.GetMetadataReader()

            // ---- EnC Lambda and Closure Map ----
            let lambdaMaps = readCdiBlobs reader PortableCustomDebugInfoKinds.encLambdaAndClosureMap
            Assert.NotEmpty lambdaMaps

            let mutable totalLambdas = 0
            let mutable totalClosures = 0

            for _, blob in lambdaMaps do
                let methodOrdinal, closures, lambdas = deserializeLambdaMap blob

                // Structural sanity: defined ordinal, at least one lambda or closure,
                // closure ordinals within range.
                Assert.True(methodOrdinal >= 0, "Roslyn lambda maps carry a defined method ordinal")
                Assert.True(not (List.isEmpty closures) || not (List.isEmpty lambdas))

                for lambda in lambdas do
                    Assert.InRange(lambda.ClosureOrdinal, MinClosureOrdinal, closures.Length - 1)

                totalLambdas <- totalLambdas + lambdas.Length
                totalClosures <- totalClosures + closures.Length

                // Byte-for-byte: re-encoding the decoded map must reproduce Roslyn's blob.
                let reencoded =
                    serializeLambdaMap
                        { EncMethodDebugInformation.Empty with
                            MethodOrdinal = methodOrdinal
                            Closures = closures
                            Lambdas = lambdas }

                Assert.Equal<byte[]>(blob, reencoded)

            // The source has 6 lambdas (2 in MakeAdder, 3 in UseLinq, 1 in ComputeAsync)
            // and capturing closures in every method.
            Assert.True(totalLambdas >= 6, $"expected at least 6 lambdas, found {totalLambdas}")
            Assert.True(totalClosures >= 3, $"expected at least 3 closures, found {totalClosures}")

            // ---- EnC Local Slot Map ----
            let slotMaps = readCdiBlobs reader PortableCustomDebugInfoKinds.encLocalSlotMap
            Assert.NotEmpty slotMaps

            let mutable longLivedSlots = 0

            for _, blob in slotMaps do
                let slots = deserializeLocalSlots blob
                Assert.NotEmpty slots

                for slot in slots do
                    match slot with
                    | EncLocalSlotInfo.Temp -> ()
                    | EncLocalSlotInfo.Slot(kind, _, ordinal) ->
                        Assert.InRange(kind, 0, MaxSerializableLocalKind)
                        Assert.True(ordinal >= 0)
                        longLivedSlots <- longLivedSlots + 1

                let reencoded =
                    serializeLocalSlots
                        { EncMethodDebugInformation.Empty with
                            LocalSlots = slots }

                Assert.Equal<byte[]>(blob, reencoded)

            Assert.True(longLivedSlots > 0, "expected at least one long-lived local slot")

            // ---- EnC State Machine State Map ----
            let stateMaps = readCdiBlobs reader PortableCustomDebugInfoKinds.encStateMachineStateMap
            Assert.NotEmpty stateMaps

            for _, blob in stateMaps do
                let states = deserializeStateMachineStates blob

                // ComputeAsync has two suspension points (await Task.Delay, await
                // Task.Yield); the decoder enforces monotone offsets, re-check here.
                Assert.True(states.Length >= 2, $"expected at least 2 states, found {states.Length}")

                let offsets = states |> List.map (fun s -> s.SyntaxOffset)
                Assert.Equal<int list>(List.sort offsets, offsets)

                let reencoded =
                    serializeStateMachineStates
                        { EncMethodDebugInformation.Empty with
                            StateMachineStates = states }

                Assert.Equal<byte[]>(blob, reencoded)
        finally
            try
                Directory.Delete(workDir, true)
            with _ ->
                ()
