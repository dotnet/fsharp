// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module CompilerService.EncMethodDebugInformationTests

open System
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit

open Internal.Utilities
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.AbstractIL.EncMethodDebugInformation
open FSharp.Compiler.HotReloadBaseline

// -----------------------------------------------------------------------
// Round-trip properties (pure codec)
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

    Assert.Equal<byte>([| 0x01uy; 0x01uy; 0x01uy; 0x01uy; 0x06uy; 0x02uy |], serializeLambdaMap info)

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

    Assert.Equal<byte>([| 0x00uy; 0x01uy; 0x01uy; 0x82uy; 0x03uy; 0x03uy |], serializeLocalSlots info)

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

[<Fact>]
let ``Synthesized name snapshot preserves mixed bucket allocation order`` () =
    let expectedNames =
        [|
            "endpoints@hotreload"
            "endpoints@hotreload#g0_o0"
            "endpoints@hotreload-2"
            "endpoints@hotreload#g0_o1"
            "endpoints@hotreload-4"
            "endpoints@hotreload#g0_o2"
            "endpoints@hotreload#g0_o3"
            "endpoints@hotreload#g0_o4"
        |]

    let snapshot = [ struct ("endpoints", expectedNames) ]

    let blob = serializeSynthesizedNameSnapshot snapshot
    let decoded = deserializeSynthesizedNameSnapshot blob

    match Map.tryFind "endpoints" decoded with
    | Some actualNames -> Assert.Equal<string[]>(expectedNames, actualNames)
    | None -> failwith "expected endpoints bucket to round-trip"

[<Fact>]
let ``Synthesized name snapshot rejects a present but empty payload`` () =
    // Canonical empty snapshots are represented by an absent CDI row. Accepting this
    // non-canonical payload would suppress reconstruction from the assembly token maps.
    let blob = [| 1uy; 0uy |]
    Assert.Throws<InvalidDataException>(fun () -> deserializeSynthesizedNameSnapshot blob |> ignore)
    |> ignore

[<Fact>]
let ``Synthesized name snapshot bounds name allocation by remaining payload`` () =
    // version=1, buckets=1, key="k", names=0x1fffffff, with no name payload.
    let blob = [| 1uy; 1uy; 1uy; byte 'k'; 0xDFuy; 0xFFuy; 0xFFuy; 0xFFuy |]
    Assert.Throws<InvalidDataException>(fun () -> deserializeSynthesizedNameSnapshot blob |> ignore)
    |> ignore

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
    // Regression: a large in-range parent whose packed key wraps NEGATIVE in int32
    // ((0xFFFE + 1) <<< 16). The int32 packing accepted the wrapped key (negative
    // <= MaxOccurrenceKey), failing open; the int64 packing must reject it.
    Assert.Equal(None, tryEncodeOccurrenceKey [ 0xFFFE; 0 ])
    Assert.Equal(None, tryEncodeOccurrenceKey [ 0x7FFF; 0xFFFF ])
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
        Path.Combine(Path.GetTempPath(), "fsharp-enc-cdi-" + Guid.NewGuid().ToString("N"))

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
    // Resolve the dotnet host like the rest of the test framework: repo-local .dotnet
    // first, PATH fallback otherwise (the hand-rolled path misses on some CI images).
    psi.FileName <- TestFramework.initialConfig.DotNetExe
    // ProcessStartInfo.ArgumentList does not exist on net472, so build the quoted argument
    // string by hand (projPath is the only argument that can contain spaces).
    psi.Arguments <- $"build \"{projPath}\" -c Debug -p:DebugType=portable -v m"
    // net472 defaults UseShellExecute to true, which is incompatible with stream
    // redirection; set it explicitly so the Desktop test legs can start the process.
    psi.UseShellExecute <- false
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

            Assert.Equal<byte>(blob, reencoded)

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

            Assert.Equal<byte>(blob, reencoded)

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

            Assert.Equal<byte>(blob, reencoded)
    finally
        try
            Directory.Delete(workDir, true)
        with _ ->
            ()

// -----------------------------------------------------------------------
// Synthetic plumbing: exercise the real ILBinaryWriter/PortablePdbGenerator path
// (no hot reload flag, no session machinery) with a synthetic CDI row map.
// -----------------------------------------------------------------------

module private Plumbing =

    // A real primary-assembly reference (this process's own corelib) so ilg.typ_Object
    // resolves to an external TypeRef; the IL writer requires every type's 'extends' to
    // resolve to a real System.Object, even one it never loads.
    let private primaryAssemblyRef = ILAssemblyRef.FromAssemblyName(typeof<obj>.Assembly.GetName())

    let private ilg =
        mkILGlobals (ILScopeRef.Assembly primaryAssemblyRef, [], ILScopeRef.Assembly primaryAssemblyRef)

    let private mkAbstractMethod (name: string) : ILMethodDef =
        // MethodBody.Abstract is the smallest body shape the IL writer accepts: it still
        // gets a full PdbMethodData row (token, name), but skips code/IL-body generation
        // entirely, which is all this test needs.
        mkILNonGenericStaticMethod (name, ILMemberAccess.Public, [], mkILReturn ILType.Void, MethodBody.Abstract)

    let private mkType (typeName: string) (methodNames: string list) : ILTypeDef =
        let methods = methodNames |> List.map mkAbstractMethod |> mkILMethods

        ILTypeDef(
            typeName,
            TypeAttributes.Public,
            ILTypeDefLayout.Auto,
            [],
            [],
            Some ilg.typ_Object,
            methods,
            mkILTypeDefs [],
            mkILFields [],
            emptyILMethodImpls,
            mkILEvents [],
            mkILProperties [],
            emptyILSecurityDecls,
            emptyILCustomAttrsStored
        )

    /// Builds a minimal in-memory module with one type per (typeName, methodNames) pair.
    /// Two types may each declare a method of the same name: the IL writer's per-type
    /// method table forbids two same-named methods of the same arity *within one type*
    /// (unrelated to CDI), but the CDI name-keying this test exercises is per-assembly,
    /// so cross-type name clashes are exactly the ambiguous case to cover.
    let buildModuleOfTypes (types: (string * string list) list) : ILModuleDef =
        let typeDefs = types |> List.map (fun (typeName, methodNames) -> mkType typeName methodNames)

        let assemblyName = "EncCdiPlumbing_" + Guid.NewGuid().ToString("N")

        mkILSimpleModule
            assemblyName
            assemblyName
            true
            (4, 0)
            false
            (mkILTypeDefs typeDefs)
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319" // Non-empty: pins the metadata version explicitly rather than relying on primaryAssemblyRef's.

    /// Builds a minimal in-memory module with one type "T" declaring 'methodNames'.
    let buildModule (methodNames: string list) : ILModuleDef = buildModuleOfTypes [ "T", methodNames ]

    /// Writes 'modul' through the same in-memory ILBinaryWriter entry point fsi.fs uses for
    /// dynamic assembly emission, attaching 'methodCustomDebugInfoRows' as the CDI side
    /// channel. No hot reload flag or session state is involved.
    let writeInMemoryWithModuleRows
        (modul: ILModuleDef)
        (moduleCustomDebugInfoRows: PdbModuleCustomDebugInfo list)
        (methodCustomDebugInfoRows: Map<string, PdbMethodCustomDebugInfo list>)
        =
        let options: options =
            {
                ilg = ilg
                outfile = "test.dll"
                pdbfile = Some "test.pdb"
                portablePDB = true
                embeddedPDB = false
                embedAllSource = false
                embedSourceList = []
                allGivenSources = []
                sourceLink = ""
                checksumAlgorithm = HashAlgorithm.Sha256
                signer = None
                emitTailcalls = true
                deterministic = false
                dumpDebugInfo = false
                referenceAssemblyOnly = false
                referenceAssemblyAttribOpt = None
                referenceAssemblySignatureHash = None
                pathMap = PathMap.empty
                moduleCustomDebugInfoRows = moduleCustomDebugInfoRows
                methodCustomDebugInfoRows = methodCustomDebugInfoRows
            }

        match WriteILBinaryInMemory(options, modul, id) with
        | assemblyBytes, Some pdbBytes -> assemblyBytes, pdbBytes
        | _, None -> failwith "expected a portable PDB to be produced"

    let writeInMemory (modul: ILModuleDef) (methodCustomDebugInfoRows: Map<string, PdbMethodCustomDebugInfo list>) =
        writeInMemoryWithModuleRows modul [] methodCustomDebugInfoRows

    type CdiRow =
        {
            MethodName: string option
            Kind: Guid
            Blob: byte[]
        }

    /// All method-parented CustomDebugInformation rows in the produced PDB, read back with
    /// System.Reflection.Metadata (independent of this codebase's own decoders), resolving
    /// each row's parent MethodDef token to its name via the companion assembly image.
    let readAllCdiRows (assemblyBytes: byte[]) (pdbBytes: byte[]) : CdiRow list =
        use peReader = new PEReader(ImmutableArray.CreateRange assemblyBytes)
        let peMdReader = peReader.GetMetadataReader()

        use pdbProvider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
        let pdbMdReader = pdbProvider.GetMetadataReader()

        let methodTokenToName =
            [ for h: MethodDefinitionHandle in peMdReader.MethodDefinitions ->
                  MetadataTokens.GetToken(MethodDefinitionHandle.op_Implicit h: EntityHandle),
                  peMdReader.GetString(peMdReader.GetMethodDefinition(h).Name) ]
            |> Map.ofList

        [ for cdiHandle in pdbMdReader.CustomDebugInformation do
              let cdi = pdbMdReader.GetCustomDebugInformation cdiHandle

              if cdi.Parent.Kind = HandleKind.MethodDefinition then
                  {
                      MethodName = Map.tryFind (MetadataTokens.GetToken cdi.Parent) methodTokenToName
                      Kind = pdbMdReader.GetGuid cdi.Kind
                      Blob = pdbMdReader.GetBlobBytes cdi.Value
                  } ]

    type private StreamHeader =
        {
            HeaderOffset: int
            DataOffset: int
            Size: int
            Name: string
        }

    let private readInt32 (bytes: byte[]) offset =
        int bytes[offset]
        ||| (int bytes[offset + 1] <<< 8)
        ||| (int bytes[offset + 2] <<< 16)
        ||| (int bytes[offset + 3] <<< 24)

    let private writeInt32 (bytes: byte[]) offset value =
        bytes[offset] <- byte value
        bytes[offset + 1] <- byte (value >>> 8)
        bytes[offset + 2] <- byte (value >>> 16)
        bytes[offset + 3] <- byte (value >>> 24)

    let private metadataStreamHeaders (assemblyBytes: byte[]) =
        use peReader = new PEReader(ImmutableArray.CreateRange assemblyBytes)
        let metadataRoot = peReader.PEHeaders.MetadataStartOffset
        let versionLength = readInt32 assemblyBytes (metadataRoot + 12)
        let streamsOffset = metadataRoot + 16 + ((versionLength + 3) &&& ~~~3)
        let streamCount = int assemblyBytes[streamsOffset + 2] ||| (int assemblyBytes[streamsOffset + 3] <<< 8)
        let headers = ResizeArray<StreamHeader>()
        let mutable headerOffset = streamsOffset + 4

        for _ in 1..streamCount do
            let relativeOffset = readInt32 assemblyBytes headerOffset
            let size = readInt32 assemblyBytes (headerOffset + 4)
            let mutable nameEnd = headerOffset + 8

            while assemblyBytes[nameEnd] <> 0uy do
                nameEnd <- nameEnd + 1

            let name = Text.Encoding.ASCII.GetString(assemblyBytes, headerOffset + 8, nameEnd - headerOffset - 8)
            let paddedNameLength = ((nameEnd - headerOffset - 8 + 1) + 3) &&& ~~~3

            headers.Add
                {
                    HeaderOffset = headerOffset
                    DataOffset = metadataRoot + relativeOffset
                    Size = size
                    Name = name
                }

            headerOffset <- headerOffset + 8 + paddedNameLength

        headers |> Seq.toList

    let private mutateStream name mutation (assemblyBytes: byte[]) =
        let copy = Array.copy assemblyBytes

        let stream =
            metadataStreamHeaders copy
            |> List.find (fun stream -> stream.Name = name)

        mutation copy stream
        copy

    let zeroMvidGuid assemblyBytes =
        mutateStream "#GUID" (fun bytes stream -> Array.Clear(bytes, stream.DataOffset, 16)) assemblyBytes

    let truncateGuidStream assemblyBytes =
        mutateStream "#GUID" (fun bytes stream -> writeInt32 bytes (stream.HeaderOffset + 4) 0) assemblyBytes

    let truncateStringsStreamBeforeReferencedStrings assemblyBytes =
        // Keep only the reserved empty string so every non-zero table index points
        // outside the declared heap, while the following metadata bytes remain present.
        mutateStream "#Strings" (fun bytes stream -> writeInt32 bytes (stream.HeaderOffset + 4) 1) assemblyBytes

    let truncateStringsStreamBeforeMethodNameTerminator methodName assemblyBytes =
        let copy = Array.copy assemblyBytes

        let methodNameOffset =
            use peReader = new PEReader(ImmutableArray.CreateRange copy)
            let metadataReader = peReader.GetMetadataReader()

            metadataReader.MethodDefinitions
            |> Seq.map metadataReader.GetMethodDefinition
            |> Seq.find (fun methodDef -> metadataReader.GetString(methodDef.Name) = methodName)
            |> fun methodDef -> MetadataTokens.GetHeapOffset(methodDef.Name)

        let stringsStream =
            metadataStreamHeaders copy
            |> List.find (fun stream -> stream.Name = "#Strings")

        let terminatorOffset = methodNameOffset + Text.Encoding.UTF8.GetByteCount(methodName)

        // The name bytes remain inside #Strings, but its terminating zero is now the
        // first byte outside the declared stream.
        writeInt32 copy (stringsStream.HeaderOffset + 4) terminatorOffset
        copy

    let addUnsupportedFieldPointerTable assemblyBytes =
        mutateStream "#~" (fun bytes stream ->
            bytes[stream.HeaderOffset + 8] <- byte '#'
            bytes[stream.HeaderOffset + 9] <- byte '-'

            let validLow = readInt32 bytes (stream.DataOffset + 8)
            writeInt32 bytes (stream.DataOffset + 8) (validLow ||| (1 <<< 3))) assemblyBytes

[<Fact>]
let ``Synthetic CustomDebugInformation row attaches to the right MethodDef`` () =
    let modul = Plumbing.buildModule [ "Foo"; "Bar" ]

    let blob =
        serializeLambdaMap
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 0
                Closures = [ { SyntaxOffset = 0 } ]
                Lambdas = [ { SyntaxOffset = 5; ClosureOrdinal = 0 } ] }

    let rows: Map<string, PdbMethodCustomDebugInfo list> =
        Map.ofList [ "Foo", [ { KindGuid = PortableCustomDebugInfoKinds.encLambdaAndClosureMap; Blob = blob } ] ]

    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul rows
    let cdiRows = Plumbing.readAllCdiRows assemblyBytes pdbBytes

    let row = Assert.Single cdiRows
    Assert.Equal(Some "Foo", row.MethodName)
    Assert.Equal(PortableCustomDebugInfoKinds.encLambdaAndClosureMap, row.Kind)
    Assert.Equal<byte>(blob, row.Blob)

    // Full circle: the codec decodes exactly what was written.
    let methodOrdinal, closures, lambdas = deserializeLambdaMap row.Blob
    Assert.Equal(0, methodOrdinal)
    Assert.Equal<EncClosureInfo list>([ { SyntaxOffset = 0 } ], closures)
    Assert.Equal<EncLambdaInfo list>([ { SyntaxOffset = 5; ClosureOrdinal = 0 } ], lambdas)

[<Fact>]
let ``Empty map produces zero CustomDebugInformation rows`` () =
    let modul = Plumbing.buildModule [ "Foo" ]
    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul (Map.empty<string, PdbMethodCustomDebugInfo list>)
    Assert.Empty(Plumbing.readAllCdiRows assemblyBytes pdbBytes)

[<Fact>]
let ``A method name absent from the module attaches nothing`` () =
    // Fail closed, matching the feature this codec ports from: an unresolvable name is
    // silently dropped rather than raising, so it can never attach to the wrong method.
    let modul = Plumbing.buildModule [ "Foo" ]

    let blob =
        serializeStateMachineStates
            { EncMethodDebugInformation.Empty with
                StateMachineStates = [ { StateNumber = 0; SyntaxOffset = 1 } ] }

    let rows: Map<string, PdbMethodCustomDebugInfo list> =
        Map.ofList [ "DoesNotExist", [ { KindGuid = PortableCustomDebugInfoKinds.encStateMachineStateMap; Blob = blob } ] ]

    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul rows
    Assert.Empty(Plumbing.readAllCdiRows assemblyBytes pdbBytes)

[<Fact>]
let ``An ambiguous method name attaches to neither method`` () =
    // Two distinct types each declaring a "Dup" method: the CDI name-keying in
    // PortablePdbGenerator is per-assembly (IL method name only, not qualified by
    // declaring type), so this reproduces the ambiguous case without hitting the
    // unrelated IL writer invariant that forbids two same-named/same-arity methods
    // within a single type.
    let modul = Plumbing.buildModuleOfTypes [ "T1", [ "Dup" ]; "T2", [ "Dup" ] ]

    let blob =
        serializeStateMachineStates
            { EncMethodDebugInformation.Empty with
                StateMachineStates = [ { StateNumber = 0; SyntaxOffset = 1 } ] }

    let rows: Map<string, PdbMethodCustomDebugInfo list> =
        Map.ofList [ "Dup", [ { KindGuid = PortableCustomDebugInfoKinds.encStateMachineStateMap; Blob = blob } ] ]

    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul rows
    Assert.Empty(Plumbing.readAllCdiRows assemblyBytes pdbBytes)

[<Fact>]
let ``Synthetic module CustomDebugInformation row round-trips synthesized snapshot`` () =
    let modul = Plumbing.buildModule [ "Foo" ]

    let expected =
        [ struct ("endpoints", [| "endpoints@hotreload#g0_o0"; "endpoints@hotreload"; "endpoints@hotreload#g0_o1" |]) ]

    let moduleRows = computeSynthesizedNameSnapshotCustomDebugInfoRows expected
    let _, pdbBytes =
        Plumbing.writeInMemoryWithModuleRows modul moduleRows (Map.empty<string, PdbMethodCustomDebugInfo list>)

    match readSynthesizedNameSnapshotFromPortablePdb pdbBytes with
    | Some snapshot ->
        match Map.tryFind "endpoints" snapshot with
        | Some names ->
            let struct (_, expectedNames) = List.head expected
            Assert.Equal<string[]>(expectedNames, names)
        | None -> failwith "expected endpoints bucket"
    | None -> failwith "expected recorded synthesized-name snapshot"

[<Fact>]
let ``Baseline reader populates token maps and uses reconstructed synthesized snapshot when no record exists`` () =
    let modul =
        Plumbing.buildModuleOfTypes
            [
                "T", [ "Compute" ]
                "endpoints@hotreload#g0_o0", []
            ]

    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul (Map.empty<string, PdbMethodCustomDebugInfo list>)
    let baseline = readFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)

    Assert.Equal(SynthesizedNameSnapshotSource.Reconstructed, baseline.SynthesizedNameSnapshotSource)

    Assert.True(
        baseline.TokenMaps.TypeTokens
        |> Seq.exists (fun kvp -> kvp.Key.Name = "T" && (kvp.Value &&& 0xFF000000) = 0x02000000),
        "expected T TypeDef token")

    Assert.True(
        baseline.TokenMaps.MethodTokens
        |> Seq.exists (fun kvp ->
            kvp.Key.Name = "Compute"
            && kvp.Key.DeclaringType.Name = "T"
            && (kvp.Value &&& 0xFF000000) = 0x06000000),
        "expected Compute MethodDef token")

    match Map.tryFind "endpoints" baseline.SynthesizedNameSnapshot with
    | Some names -> Assert.Contains("endpoints@hotreload#g0_o0", names)
    | None -> failwith "expected reconstructed endpoints bucket"

[<Fact>]
let ``Baseline reader does not trust a portable PDB from another assembly`` () =
    let assemblyBytes, _ =
        Plumbing.writeInMemory (Plumbing.buildModule [ "First" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let _, unrelatedPdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Second"; "Third" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let baseline = readFromAssemblyAndPdbBytes assemblyBytes (Some unrelatedPdbBytes)

    Assert.True(baseline.PortablePdb.IsNone, "a mismatched PDB must not contribute baseline state")
    Assert.Equal(SynthesizedNameSnapshotSource.Reconstructed, baseline.SynthesizedNameSnapshotSource)

[<Fact>]
let ``Baseline reader rejects an empty MVID`` () =
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Compute" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let assemblyBytes = Plumbing.zeroMvidGuid assemblyBytes
    Assert.True((tryReadFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)).IsNone)

[<Fact>]
let ``Baseline reader bounds the MVID read to the GUID stream`` () =
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Compute" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let assemblyBytes = Plumbing.truncateGuidStream assemblyBytes
    Assert.True((tryReadFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)).IsNone)

[<Fact>]
let ``Baseline reader rejects pointer-table indirection in an uncompressed stream`` () =
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Compute" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let assemblyBytes = Plumbing.addUnsupportedFieldPointerTable assemblyBytes
    Assert.True((tryReadFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)).IsNone)

[<Fact>]
let ``Baseline valid-mask reader does not sign-extend bit 31`` () =
    let bytes = [| 0uy; 0uy; 0uy; 0x80uy; 0uy; 0uy; 0uy; 0uy |]

    Assert.Equal(0x0000000080000000UL, FSharp.Compiler.AbstractIL.ILBaselineReader.readUInt64 bytes 0)

[<Fact>]
let ``Baseline table data starts after valid tables with zero rows`` () =
    let rowCounts = Array.zeroCreate 64
    let valid = 1UL <<< 3

    Assert.Equal(128, FSharp.Compiler.AbstractIL.ILBaselineReader.tableDataStart 100 valid rowCounts)

[<Fact>]
let ``Baseline reader rejects a string offset outside the strings stream`` () =
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Compute" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let assemblyBytes = Plumbing.truncateStringsStreamBeforeReferencedStrings assemblyBytes

    Assert.True((tryReadFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)).IsNone)

[<Fact>]
let ``Baseline reader rejects a string without a terminator inside the strings stream`` () =
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemory (Plumbing.buildModule [ "Compute" ]) (Map.empty<string, PdbMethodCustomDebugInfo list>)

    let assemblyBytes = Plumbing.truncateStringsStreamBeforeMethodNameTerminator "Compute" assemblyBytes

    Assert.True((tryReadFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)).IsNone)

[<Fact>]
let ``Baseline reader gives recorded synthesized snapshot precedence over reconstruction`` () =
    let modul =
        Plumbing.buildModuleOfTypes
            [
                "T", [ "Compute" ]
                "endpoints@hotreload#g0_o0", []
            ]

    let recordedNames =
        [| "recorded@hotreload"; "recorded@hotreload#g0_o0"; "recorded@hotreload-2" |]

    let moduleRows = computeSynthesizedNameSnapshotCustomDebugInfoRows [ struct ("endpoints", recordedNames) ]
    let assemblyBytes, pdbBytes =
        Plumbing.writeInMemoryWithModuleRows modul moduleRows (Map.empty<string, PdbMethodCustomDebugInfo list>)
    let baseline = readFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)

    Assert.Equal(SynthesizedNameSnapshotSource.Recorded, baseline.SynthesizedNameSnapshotSource)

    match Map.tryFind "endpoints" baseline.SynthesizedNameSnapshot with
    | Some names -> Assert.Equal<string[]>(recordedNames, names)
    | None -> failwith "expected recorded endpoints bucket"

[<Fact>]
let ``Baseline reader reconstructs closure names from method CDI rows`` () =
    let modul =
        Plumbing.buildModuleOfTypes
            [
                "T", [ "Compute" ]
                "Compute@hotreload#g0_o0", []
            ]

    let lambdaMap =
        serializeLambdaMap
            { EncMethodDebugInformation.Empty with
                MethodOrdinal = 0
                Closures = [ { SyntaxOffset = 0 } ] }

    let methodRows: Map<string, PdbMethodCustomDebugInfo list> =
        Map.ofList
            [
                "Compute",
                [
                    {
                        KindGuid = PortableCustomDebugInfoKinds.encLambdaAndClosureMap
                        Blob = lambdaMap
                    }
                ]
            ]

    let assemblyBytes, pdbBytes = Plumbing.writeInMemory modul methodRows
    let baseline = readFromAssemblyAndPdbBytes assemblyBytes (Some pdbBytes)

    Assert.False(Map.isEmpty baseline.EncMethodDebugInfos, "expected method EnC debug information")
    Assert.False(Map.isEmpty baseline.EncClosureNames, "expected reconstructed closure-name table")

    let closureNames =
        baseline.EncClosureNames
        |> Map.toSeq
        |> Seq.collect (fun (_, rows) -> rows |> Map.toSeq |> Seq.map snd)
        |> Set.ofSeq

    Assert.Contains("Compute@hotreload#g0_o0", closureNames)
