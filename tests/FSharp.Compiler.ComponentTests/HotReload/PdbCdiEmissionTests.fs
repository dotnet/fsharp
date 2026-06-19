namespace FSharp.Compiler.ComponentTests.HotReload

open System.Collections.Immutable
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Compiler.EncMethodDebugInformation

/// Tests for baseline-time EnC CustomDebugInformation emission: compiling
/// with --test:HotReloadDeltas must attach a decodable EnC Lambda and Closure Map CDI
/// row to lambda-bearing methods in the portable PDB, and compiling without the flag
/// must emit no EnC CDI rows at all.
[<Collection(nameof NotThreadSafeResourceCollection)>]
module PdbCdiEmissionTests =

    /// Two lambda occurrences in 'transform': the outer List.map argument (ordinal 0,
    /// no parent) and the lambda nested inside it (ordinal 1, parent chain [0]).
    let private sampleSource =
        """
module CdiSample

let transform (values: int list) =
    let offset = 1
    values |> List.map (fun v -> List.map (fun u -> u + offset + v) [ v ] |> List.sum)
"""

    let private getOutputPath result =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation did not produce an output path."
        | CompilationResult.Failure f ->
            failwithf "Compilation was expected to succeed, but failed with: %A" f.Diagnostics

    let private compileSource (source: string) extraOptions =
        // The flag-on compile starts a hot reload session as a side effect; keep the
        // shared service clean on both sides of the compile.
        let service = global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance
        service.EndSession()

        try
            FSharp source
            |> withOptions ([ "--langversion:preview"; "--debug+"; "--optimize-" ] @ extraOptions)
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getOutputPath
        finally
            service.EndSession()

    let private compileSample extraOptions = compileSource sampleSource extraOptions

    let private pdbPathFor (assemblyPath: string) =
        let pdbPath = Path.ChangeExtension(assemblyPath, ".pdb")
        Assert.True(File.Exists pdbPath, $"expected portable PDB at %s{pdbPath}")
        pdbPath

    /// Finds the MethodDef row number of the single method with the given name.
    let private findMethodRowNumber (assemblyPath: string) (methodName: string) =
        use peReader = new PEReader(File.OpenRead assemblyPath)
        let reader = peReader.GetMetadataReader()

        let rowNumbers =
            [
                for handle in reader.MethodDefinitions do
                    let methodDef = reader.GetMethodDefinition handle

                    if reader.GetString methodDef.Name = methodName then
                        let entityHandle: EntityHandle = MethodDefinitionHandle.op_Implicit handle
                        MetadataTokens.GetRowNumber entityHandle
            ]

        Assert.True(rowNumbers.Length = 1, $"expected exactly one method named '%s{methodName}', found %d{rowNumbers.Length}")
        rowNumbers.Head

    let private encKindGuids =
        [
            PortableCustomDebugInfoKinds.encLocalSlotMap
            PortableCustomDebugInfoKinds.encLambdaAndClosureMap
            PortableCustomDebugInfoKinds.encStateMachineStateMap
        ]

    /// Reads all (kind, blob) CDI rows attached to the given method row in the PDB.
    let private readMethodCdiRows (pdbPath: string) (methodRowNumber: int) =
        let bytes = ImmutableArray.CreateRange(File.ReadAllBytes pdbPath)
        use provider = MetadataReaderProvider.FromPortablePdbImage bytes
        let pdbReader = provider.GetMetadataReader()

        let methodHandle: EntityHandle =
            MethodDefinitionHandle.op_Implicit (MetadataTokens.MethodDefinitionHandle methodRowNumber)

        [
            for cdiHandle in pdbReader.GetCustomDebugInformation methodHandle do
                let cdi = pdbReader.GetCustomDebugInformation cdiHandle
                pdbReader.GetGuid cdi.Kind, pdbReader.GetBlobBytes cdi.Value
        ]

    [<Fact>]
    let ``Compile with hot reload flag emits decodable EnC lambda and closure map`` () =
        let assemblyPath = compileSample [ "--test:HotReloadDeltas" ]
        let pdbPath = pdbPathFor assemblyPath
        let methodRowNumber = findMethodRowNumber assemblyPath "transform"
        let cdiRows = readMethodCdiRows pdbPath methodRowNumber

        let lambdaMapBlobs =
            cdiRows
            |> List.filter (fun (kind, _) -> kind = PortableCustomDebugInfoKinds.encLambdaAndClosureMap)
            |> List.map snd

        Assert.True(lambdaMapBlobs.Length = 1, $"expected exactly one EnC lambda map CDI row, found %d{lambdaMapBlobs.Length}")

        // The blob decodes with the F# decoder and matches the occurrence extraction for this
        // source: two occurrences, occurrence key [0] for the outer List.map lambda and
        // [0; 1] for the nested one (the packed key preserves the parent relationship).
        let methodOrdinal, closures, lambdas = deserializeLambdaMap lambdaMapBlobs.Head

        Assert.Equal(UndefinedMethodOrdinal, methodOrdinal)
        Assert.Equal(2, lambdas.Length)
        Assert.Equal(2, closures.Length)

        Assert.Equal<int list>([ 0 ], decodeOccurrenceKey lambdas[0].SyntaxOffset)
        Assert.Equal<int list>([ 0; 1 ], decodeOccurrenceKey lambdas[1].SyntaxOffset)

        // One closure scope per occurrence; lambda i references closure i (IlxGen lowers
        // every lambda occurrence to its own closure class).
        Assert.Equal(0, lambdas[0].ClosureOrdinal)
        Assert.Equal(1, lambdas[1].ClosureOrdinal)
        Assert.Equal<int list>([ 0 ], decodeOccurrenceKey closures[0].SyntaxOffset)
        Assert.Equal<int list>([ 0; 1 ], decodeOccurrenceKey closures[1].SyntaxOffset)

        // The EnC Local Slot Map is intentionally omitted at baseline (the lowered local
        // slot layout is an IlxGen emission artifact, not derivable from the typed tree).
        let slotMapRows =
            cdiRows
            |> List.filter (fun (kind, _) -> kind = PortableCustomDebugInfoKinds.encLocalSlotMap)

        Assert.Empty slotMapRows

    [<Fact>]
    let ``Compile without hot reload flag emits no EnC custom debug information`` () =
        let assemblyPath = compileSample []
        let pdbPath = pdbPathFor assemblyPath

        let bytes = ImmutableArray.CreateRange(File.ReadAllBytes pdbPath)
        use provider = MetadataReaderProvider.FromPortablePdbImage bytes
        let pdbReader = provider.GetMetadataReader()

        let encRows =
            [
                for cdiHandle in pdbReader.CustomDebugInformation do
                    let cdi = pdbReader.GetCustomDebugInformation cdiHandle
                    let kind = pdbReader.GetGuid cdi.Kind

                    if List.contains kind encKindGuids then
                        kind
            ]

        Assert.Empty encRows

    [<Fact>]
    let ``Hot reload session baseline exposes decoded EnC method debug information`` () =
        // The flag-on compile captures a baseline from the emitted artifacts (including the
        // portable PDB with its EnC CDI rows) and starts a hot reload session as a side
        // effect; the session baseline must expose the decoded per-method map keyed by
        // MethodDef token.
        let service = global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance
        service.EndSession()

        try
            let assemblyPath =
                FSharp sampleSource
                |> withOptions [ "--langversion:preview"; "--debug+"; "--optimize-"; "--test:HotReloadDeltas" ]
                |> asLibrary
                |> compile
                |> shouldSucceed
                |> getOutputPath

            let baseline =
                match service.TryGetBaseline() with
                | ValueSome baseline -> baseline
                | ValueNone -> failwith "expected the flag-on compile to start a hot reload session"

            let methodToken = 0x06000000 ||| findMethodRowNumber assemblyPath "transform"

            let info =
                match Map.tryFind methodToken baseline.EncMethodDebugInfos with
                | Some info -> info
                | None ->
                    failwithf
                        "expected EnC method debug information for token 0x%08X, found tokens %A"
                        methodToken
                        (baseline.EncMethodDebugInfos |> Map.toList |> List.map fst)

            // Matches the occurrence extraction pinned by the emission test above: occurrence key
            // [0] for the outer List.map lambda and [0; 1] for the nested one, one closure
            // scope per occurrence with lambda i referencing closure i.
            Assert.Equal(UndefinedMethodOrdinal, info.MethodOrdinal)
            Assert.Equal(2, info.Lambdas.Length)
            Assert.Equal(2, info.Closures.Length)
            Assert.Equal<int list>([ 0 ], decodeOccurrenceKey info.Lambdas[0].SyntaxOffset)
            Assert.Equal<int list>([ 0; 1 ], decodeOccurrenceKey info.Lambdas[1].SyntaxOffset)
            Assert.Equal(0, info.Lambdas[0].ClosureOrdinal)
            Assert.Equal(1, info.Lambdas[1].ClosureOrdinal)
            Assert.Equal<int list>([ 0 ], decodeOccurrenceKey info.Closures[0].SyntaxOffset)
            Assert.Equal<int list>([ 0; 1 ], decodeOccurrenceKey info.Closures[1].SyntaxOffset)
            Assert.Empty info.LocalSlots
            Assert.Empty info.StateMachineStates
        finally
            service.EndSession()

    [<Fact>]
    let ``Baseline PDB without EnC rows decodes to an empty method debug information map`` () =
        // Back-compat: a flag-off PDB carries no EnC CDI rows, so the
        // baseline read comes back empty instead of failing and sessions start fine on it.
        let assemblyPath = compileSample []
        let pdbPath = pdbPathFor assemblyPath

        let infos = readEncMethodDebugInfoFromPortablePdb (File.ReadAllBytes pdbPath)

        Assert.True(Map.isEmpty infos, $"expected no EnC method debug information, found %d{Map.count infos} entries")

        // Fail safe on non-PDB inputs too: empty and garbage images decode to the empty map.
        Assert.True(Map.isEmpty (readEncMethodDebugInfoFromPortablePdb Array.empty))
        Assert.True(Map.isEmpty (readEncMethodDebugInfoFromPortablePdb [| 0xBAuy; 0xADuy; 0xF0uy; 0x0Duy |]))

    /// Two resume points in 'computeAsync' (one per let!), state numbers 1 and 2 in
    /// source order (LowerStateMachines.genPC assigns them positionally).
    let private taskSampleSource =
        """
module CdiTaskSample

open System.Threading.Tasks

let computeAsync (input: int) =
    task {
        let! x = Task.FromResult (input + 1)
        let! y = Task.FromResult (x * 2)
        return x + y
    }
"""

    [<Fact>]
    let ``Compile with hot reload flag emits decodable EnC state machine state map for task members`` () =
        let assemblyPath = compileSource taskSampleSource [ "--test:HotReloadDeltas" ]
        let pdbPath = pdbPathFor assemblyPath
        let methodRowNumber = findMethodRowNumber assemblyPath "computeAsync"
        let cdiRows = readMethodCdiRows pdbPath methodRowNumber

        let stateMapBlobs =
            cdiRows
            |> List.filter (fun (kind, _) -> kind = PortableCustomDebugInfoKinds.encStateMachineStateMap)
            |> List.map snd

        Assert.True(
            stateMapBlobs.Length = 1,
            $"expected exactly one EnC state machine state map CDI row, found %d{stateMapBlobs.Length}")

        // The blob decodes with the Roslyn-format decoder: two resume points, state
        // numbers 1 and 2 (positional, from the lowering's conversion order), with the
        // resume-point ORDINAL in the syntax-offset slot (the occurrence-key
        // philosophy: deterministic ints, not source offsets).
        let states = deserializeStateMachineStates stateMapBlobs.Head

        Assert.Equal(2, states.Length)
        Assert.Equal(1, states[0].StateNumber)
        Assert.Equal(0, states[0].SyntaxOffset)
        Assert.Equal(2, states[1].StateNumber)
        Assert.Equal(1, states[1].SyntaxOffset)

    [<Fact>]
    let ``Compile without hot reload flag emits no EnC state machine state map for task members`` () =
        let assemblyPath = compileSource taskSampleSource []
        let pdbPath = pdbPathFor assemblyPath

        let bytes = ImmutableArray.CreateRange(File.ReadAllBytes pdbPath)
        use provider = MetadataReaderProvider.FromPortablePdbImage bytes
        let pdbReader = provider.GetMetadataReader()

        let encRows =
            [
                for cdiHandle in pdbReader.CustomDebugInformation do
                    let cdi = pdbReader.GetCustomDebugInformation cdiHandle
                    let kind = pdbReader.GetGuid cdi.Kind

                    if List.contains kind encKindGuids then
                        kind
            ]

        Assert.Empty encRows

    [<Fact>]
    let ``Hot reload session baseline decodes the state machine state map for task members`` () =
        // The baseline read side (readEncMethodDebugInfoFromPortablePdb, stored on the
        // session as EncMethodDebugInfos) must surface the persisted resume points for
        // disk-started sessions (the dotnet-watch topology).
        let assemblyPath = compileSource taskSampleSource [ "--test:HotReloadDeltas" ]
        let pdbPath = pdbPathFor assemblyPath
        let methodRowNumber = findMethodRowNumber assemblyPath "computeAsync"

        let infos = readEncMethodDebugInfoFromPortablePdb (File.ReadAllBytes pdbPath)
        let methodToken = 0x06000000 ||| methodRowNumber

        match Map.tryFind methodToken infos with
        | Some info ->
            Assert.Equal(2, info.StateMachineStates.Length)
            Assert.Equal(1, info.StateMachineStates[0].StateNumber)
            Assert.Equal(2, info.StateMachineStates[1].StateNumber)
        | None ->
            failwithf
                "expected EnC method debug information for method token %08x; found tokens %A"
                methodToken
                (infos |> Map.toList |> List.map fst)
