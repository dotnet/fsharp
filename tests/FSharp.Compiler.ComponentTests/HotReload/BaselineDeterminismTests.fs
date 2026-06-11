namespace FSharp.Compiler.ComponentTests.HotReload

// Determinism pins for baseline capture (docs/hot-reload-architecture.md, "Determinism
// pins for baseline capture"): --enable:hotreloaddeltas silently forces deterministic,
// sequential codegen at config finalization in Driver/fsc.fs so a capture compile of
// identical source is byte-reproducible. These tests pin:
//   (a) byte-for-byte DLL and PDB reproducibility across two identical flag-on compiles
//       (without passing --deterministic explicitly — the pin must supply it);
//   (b) occurrence-chain/encoded-key stability across graph vs sequential type checking
//       (--parallelcompilation+/-): the C1/C2 extraction depends on the compilation's
//       file order, not the checking order, so the captured EnC method debug infos and
//       closure-name tables — and the emitted bytes — must be identical in both modes;
//   (c) closure-name derivation purity: reconstructing the chain -> name tables from
//       the ON-DISK artifacts (PDB CDI rows + DLL metadata) with
//       deriveEncClosureNamesFromEncDebugInfos yields exactly the tables the capture
//       compile recorded on the session baseline (the C6 disk-started-session contract).

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.HotReload
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.Utilities

[<Collection(nameof NotThreadSafeResourceCollection)>]
module BaselineDeterminismTests =

    /// Multi-lambda + async sample, split over three files so parallel type checking
    /// (graph mode) and parallel IlxGen/optimization have real cross-file work to
    /// reorder if the pins ever regress. FileC depends on FileA (a graph edge).
    let private fileASource =
        """
module DeterminismSampleA

let transform (values: int list) =
    let offset = 1
    values
    |> List.filter (fun v -> v > 0)
    |> List.map (fun v -> List.map (fun u -> u + offset + v) [ v ] |> List.sum)
"""

    let private fileBSource =
        """
module DeterminismSampleB

open System.Threading.Tasks

let computeAsync (input: int) =
    task {
        let! x = Task.FromResult (input + 1)
        let! y = Task.FromResult (x * 2)
        return x + y
    }
"""

    let private fileCSource =
        """
module DeterminismSampleC

let pipeline (inputs: int list) =
    inputs
    |> List.map (fun a -> DeterminismSampleA.transform [ a; a + 1 ] |> List.sum)
    |> List.filter (fun total -> total > 0)
"""

    let private createChecker () =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false
        )

    /// Lays out the three-file sample in a fresh work directory and returns
    /// (sourcePaths, dllPath, referenceArgs harvested from script options).
    let private prepareWorkDir (checker: FSharpChecker) =
        let workDir =
            Path.Combine(Path.GetTempPath(), "fsharp-hotreload-determinism", Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory workDir |> ignore

        let fileA = Path.Combine(workDir, "FileA.fs")
        let fileB = Path.Combine(workDir, "FileB.fs")
        let fileC = Path.Combine(workDir, "FileC.fs")
        File.WriteAllText(fileA, fileASource)
        File.WriteAllText(fileB, fileBSource)
        File.WriteAllText(fileC, fileCSource)

        let scriptOptions, _ =
            checker.GetProjectOptionsFromScript(
                fileA,
                SourceText.ofString fileASource,
                assumeDotNetFramework = false,
                useSdkRefs = true,
                useFsiAuxLib = false
            )
            |> Async.RunImmediate

        [| fileA; fileB; fileC |], Path.Combine(workDir, "Library.dll"), scriptOptions.OtherOptions

    /// One flag-on capture compile through the real fsc pipeline
    /// (checker.Compile -> CompileFromCommandLineArguments -> main1, where the
    /// determinism pins live). Deliberately does NOT pass --deterministic.
    let private compileCapture
        (checker: FSharpChecker)
        (sourceFiles: string[])
        (dllPath: string)
        (referenceArgs: string[])
        (extraFlags: string list)
        =
        FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

        let argv =
            Array.concat
                [
                    [| "fsc.exe" |]
                    referenceArgs
                    [|
                        "--target:library"
                        "--langversion:preview"
                        "--optimize-"
                        "--debug:portable"
                        "--enable:hotreloaddeltas"
                        $"--out:{dllPath}"
                    |]
                    Array.ofList extraFlags
                    sourceFiles
                ]

        let diagnostics, _ = checker.Compile argv |> Async.RunImmediate

        let errors =
            diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

        if errors.Length > 0 then
            failwithf "capture compile failed: %A" (errors |> Array.map (fun d -> d.Message))

        let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
        Assert.True(File.Exists dllPath, $"expected output assembly at %s{dllPath}")
        Assert.True(File.Exists pdbPath, $"expected portable PDB at %s{pdbPath}")
        File.ReadAllBytes dllPath, File.ReadAllBytes pdbPath

    let private sessionBaseline () =
        match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
        | ValueSome session -> session.Baseline
        | ValueNone -> failwith "expected the flag-on capture compile to start a hot reload session"

    [<Fact>]
    let ``baseline capture is byte-reproducible without passing deterministic explicitly`` () =
        let checker = createChecker ()
        let sourceFiles, dllPath, referenceArgs = prepareWorkDir checker

        try
            // Same inputs, same paths, two independent capture compiles: the silent
            // deterministic+sequential pin must make both artifacts byte-identical
            // (PE timestamp/MVID hashed from content, no parallel row permutation).
            let dll1, pdb1 = compileCapture checker sourceFiles dllPath referenceArgs []
            let dll2, pdb2 = compileCapture checker sourceFiles dllPath referenceArgs []

            Assert.True((dll1 = dll2), "expected two identical capture compiles to produce byte-identical assemblies")
            // The portable PDB is fully covered by deterministic emission too: document
            // paths are inputs (identical here) and the PDB id is a content hash, so
            // exact equality must hold — any drift is a determinism regression.
            Assert.True((pdb1 = pdb2), "expected two identical capture compiles to produce byte-identical PDBs")
        finally
            FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

    [<Fact>]
    let ``occurrence extraction is identical under graph and sequential type checking`` () =
        let checker = createChecker ()
        let sourceFiles, dllPath, referenceArgs = prepareWorkDir checker

        try
            // Graph type-checking is deliberately NOT pinned off by the capture flag
            // (occurrence keys derive from the compilation's file order, not the
            // checking order), so the captured extraction must be identical whichever
            // mode checked the files.
            let graphDll, graphPdb =
                compileCapture checker sourceFiles dllPath referenceArgs [ "--parallelcompilation+" ]

            let graphBaseline = sessionBaseline ()

            let sequentialDll, sequentialPdb =
                compileCapture checker sourceFiles dllPath referenceArgs [ "--parallelcompilation-" ]

            let sequentialBaseline = sessionBaseline ()

            // Occurrence chains + encoded keys (the EnC CDI content, decoded) and the
            // chain -> closure-name tables agree, token for token.
            Assert.Equal<Map<int, FSharp.Compiler.EncMethodDebugInformation.EncMethodDebugInformation>>(
                graphBaseline.EncMethodDebugInfos,
                sequentialBaseline.EncMethodDebugInfos
            )

            Assert.False(Map.isEmpty graphBaseline.EncClosureNames, "expected the capture to record closure-name tables")

            Assert.Equal<Map<int, Map<int list, string>>>(
                graphBaseline.EncClosureNames,
                sequentialBaseline.EncClosureNames
            )

            // Stronger: with codegen pinned deterministic+sequential, the checking mode
            // must not leak into the artifacts at all.
            Assert.True((graphDll = sequentialDll), "expected graph and sequential checking to emit byte-identical assemblies")
            Assert.True((graphPdb = sequentialPdb), "expected graph and sequential checking to emit byte-identical PDBs")
        finally
            FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

    /// Baseline MethodDef names keyed by token, read back from the on-disk assembly
    /// (mirrors the private HotReloadBaseline.methodNamesByToken input).
    let private methodNamesByTokenFromDisk (reader: MetadataReader) =
        reader.MethodDefinitions
        |> Seq.map (fun handle ->
            let methodDef = reader.GetMethodDefinition handle
            let entityHandle: EntityHandle = MethodDefinitionHandle.op_Implicit handle
            MetadataTokens.GetToken entityHandle, reader.GetString methodDef.Name)
        |> Map.ofSeq

    /// Simple names of every TypeDef (mirrors the private
    /// HotReloadBaseline.collectTypeDefSimpleNames over the IL tree, where top-level
    /// names carry their namespace and nested names are simple).
    let private typeDefSimpleNamesFromDisk (reader: MetadataReader) =
        reader.TypeDefinitions
        |> Seq.map (fun handle ->
            let typeDef = reader.GetTypeDefinition handle
            let name = reader.GetString typeDef.Name

            if typeDef.IsNested || typeDef.Namespace.IsNil then
                name
            else
                $"{reader.GetString typeDef.Namespace}.{name}")
        |> Set.ofSeq

    [<Fact>]
    let ``closure-name tables re-derived from on-disk artifacts equal the captured session tables`` () =
        let checker = createChecker ()
        let sourceFiles, dllPath, referenceArgs = prepareWorkDir checker

        try
            compileCapture checker sourceFiles dllPath referenceArgs [] |> ignore

            let capturedTables = (sessionBaseline ()).EncClosureNames
            Assert.False(Map.isEmpty capturedTables, "expected the capture compile to record closure-name tables")

            // Reconstruct exactly like a disk-started session in another process: the
            // EnC CDI occurrence keys from the PDB plus method names and TypeDef names
            // from the assembly metadata are the ONLY inputs.
            let pdbBytes = File.ReadAllBytes(Path.ChangeExtension(dllPath, ".pdb"))

            let encInfos =
                FSharp.Compiler.EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb pdbBytes

            use peReader = new PEReader(File.OpenRead dllPath)
            let reader = peReader.GetMetadataReader()
            let methodNames = methodNamesByTokenFromDisk reader
            let typeNames = typeDefSimpleNamesFromDisk reader

            let derive () =
                FSharp.Compiler.HotReloadBaseline.deriveEncClosureNamesFromEncDebugInfos encInfos methodNames typeNames

            let derived = derive ()

            // Names are a pure function of occurrence identity (C6): the disk
            // reconstruction reproduces the captured tables exactly...
            Assert.Equal<Map<int, Map<int list, string>>>(capturedTables, derived)
            // ...and recomputing from the same CDI keys is stable (purity pin).
            Assert.Equal<Map<int, Map<int list, string>>>(derived, derive ())
        finally
            FSharpEditAndContinueLanguageService.Instance.ResetSessionState()
