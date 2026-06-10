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

/// Tests for baseline-time EnC CustomDebugInformation emission (Phase C2): compiling
/// with --enable:hotreloaddeltas must attach a decodable EnC Lambda and Closure Map CDI
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

    let private compileSample extraOptions =
        // The flag-on compile starts a hot reload session as a side effect; keep the
        // shared service clean on both sides of the compile.
        let service = global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance
        service.EndSession()

        try
            FSharp sampleSource
            |> withOptions ([ "--langversion:preview"; "--debug+"; "--optimize-" ] @ extraOptions)
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getOutputPath
        finally
            service.EndSession()

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
        let assemblyPath = compileSample [ "--enable:hotreloaddeltas" ]
        let pdbPath = pdbPathFor assemblyPath
        let methodRowNumber = findMethodRowNumber assemblyPath "transform"
        let cdiRows = readMethodCdiRows pdbPath methodRowNumber

        let lambdaMapBlobs =
            cdiRows
            |> List.filter (fun (kind, _) -> kind = PortableCustomDebugInfoKinds.encLambdaAndClosureMap)
            |> List.map snd

        Assert.True(lambdaMapBlobs.Length = 1, $"expected exactly one EnC lambda map CDI row, found %d{lambdaMapBlobs.Length}")

        // The blob decodes with the F# decoder and matches the C1 extraction for this
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
