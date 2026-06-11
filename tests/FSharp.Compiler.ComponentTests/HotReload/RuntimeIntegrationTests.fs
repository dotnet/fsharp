namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.IO
open System.Diagnostics
open System.Reflection
open System.Reflection.PortableExecutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Collections.Immutable
open Microsoft.FSharp.Reflection
open Xunit

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open Internal.Utilities
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Compiler.Diagnostics
open FSharp.Test
open FSharp.Compiler.ComponentTests.HotReload.TestHelpers
open FSharp.Compiler.IlxDeltaEmitter
open System.Runtime.Loader
open FSharp.Compiler.ComponentTests.HotReload.TestHelpers

[<Collection(nameof NotThreadSafeResourceCollection)>]
module RuntimeIntegrationTests =

    let private reflectionFlags =
        BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public

    let private getTypedImplementationFilesTuple (projectResults: FSharpCheckProjectResults) =
        let resultsType = typeof<FSharpCheckProjectResults>

        match resultsType.GetProperty("TypedImplementationFiles", reflectionFlags) with
        | null ->
            match resultsType.GetMethod("get_TypedImplementationFiles", reflectionFlags) with
            | null -> invalidOp "Could not resolve TypedImplementationFiles reflection accessors."
            | getter -> getter.Invoke(projectResults, [||])
        | property -> property.GetValue(projectResults)

    let private getTypedAssembly (projectResults: FSharpCheckProjectResults) =
        let tupleItems =
            getTypedImplementationFilesTuple projectResults
            |> FSharpValue.GetTupleFields

        let tcGlobals = tupleItems[0] :?> FSharp.Compiler.TcGlobals.TcGlobals
        let implFiles = tupleItems[3] :?> CheckedImplFile list

        tcGlobals,
        implFiles
        |> List.map (fun implFile ->
            { ImplFile = implFile
              OptimizeDuringCodeGen = fun _ expr -> expr })
        |> CheckedAssemblyAfterOptimization

    let private createTempProject () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-tests", System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        projectDir, fsPath, dllPath

    let private baselineSource =
        """
namespace Sample

type Type =
    static member GetValue() = 1
"""

    let private updatedSource =
        """
namespace Sample

type Type =
    static member GetValue() = 2
"""

    let private insertedMethodSource =
        """
namespace Sample

type Type =
    static member GetValue() = 1
    static member GetExtra() = 99
"""

    let private compileProject (checker: FSharpChecker) (fsPath: string) (dllPath: string) (source: string) =
        File.WriteAllText(fsPath, source)

        let projectOptions, _ =
            checker.GetProjectOptionsFromScript(
                fsPath,
                SourceText.ofString source,
                assumeDotNetFramework = false,
                useSdkRefs = true,
                useFsiAuxLib = false
            )
            |> Async.RunImmediate

        let projectOptions =
            { projectOptions with
                SourceFiles = [| fsPath |]
                OtherOptions =
                    projectOptions.OtherOptions
                    |> Array.append
                        [| "--target:library"
                           "--langversion:preview"
                           "--optimize-"
                           "--debug:portable"
                           $"--out:{dllPath}" |] }

        let projectResults =
            checker.ParseAndCheckProject(projectOptions)
            |> Async.RunImmediate

        if projectResults.Diagnostics |> Array.exists (fun d -> d.Severity = FSharpDiagnosticSeverity.Error) then
            failwithf "Compilation failed: %A" projectResults.Diagnostics

        let compileDiagnostics, compileException =
            checker.Compile(Array.append [| "fsc.exe" |] (Array.append projectOptions.OtherOptions [| fsPath |]))
            |> Async.RunImmediate

        let compileErrors =
            compileDiagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        match compileErrors, compileException with
        | [||], None -> projectResults
        | errs, _ -> failwithf "Compilation produced errors: %A" (errs |> Array.map (fun d -> d.Message))

    let private createBaseline (tcGlobals: FSharp.Compiler.TcGlobals.TcGlobals) (dllPath: string) =
        let pdbPath = Path.ChangeExtension(dllPath, ".pdb")

        let ilModule =
            let options : ILReaderOptions =
                { pdbDirPath = None
                  reduceMemoryUsage = ReduceMemoryFlag.Yes
                  metadataOnly = MetadataOnlyFlag.No
                  tryGetMetadataSnapshot = fun _ -> None }

            use reader = OpenILModuleReader dllPath options
            reader.ILModuleDef

        let writerOptions: FSharp.Compiler.AbstractIL.ILBinaryWriter.options =
            { ilg = tcGlobals.ilg
              outfile = dllPath
              pdbfile = Some pdbPath
              emitTailcalls = false
              deterministic = true
              portablePDB = true
              embeddedPDB = false
              embedAllSource = false
              embedSourceList = []
              allGivenSources = []
              sourceLink = ""
              checksumAlgorithm = FSharp.Compiler.AbstractIL.ILPdbWriter.HashAlgorithm.Sha256
              signer = None
              dumpDebugInfo = false
              referenceAssemblyOnly = false
              referenceAssemblyAttribOpt = None
              referenceAssemblySignatureHash = None
              pathMap = PathMap.empty
              methodCustomDebugInfoRows = Map.empty }

        let assemblyBytes, pdbBytesOpt, tokenMappings, _ =
            FSharp.Compiler.AbstractIL.ILBinaryWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, ilModule, id)

        // Extract module ID from the PE metadata
        use peReader = new System.Reflection.PortableExecutable.PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let moduleDef = metadataReader.GetModuleDefinition()
        let moduleId = if moduleDef.Mvid.IsNil then System.Guid.NewGuid() else metadataReader.GetGuid(moduleDef.Mvid)

        // Use the SRM-free byte-based APIs
        let metadataSnapshot =
            match HotReloadBaseline.metadataSnapshotFromBytes assemblyBytes with
            | Some snapshot -> snapshot
            | None -> failwith "Failed to parse metadata snapshot from assembly bytes"

        let portablePdbSnapshot = pdbBytesOpt |> Option.map HotReloadPdb.createSnapshot

        let coreBaseline = HotReloadBaseline.create ilModule tokenMappings metadataSnapshot moduleId portablePdbSnapshot
        HotReloadBaseline.attachMetadataHandlesFromBytes assemblyBytes coreBaseline

    [<Fact>]
    let ``EmitDeltaForCompilation produces IL/metadata deltas`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()

        try
            // Baseline compilation
            let baselineResults = compileProject checker fsPath dllPath baselineSource
            let tcGlobals, baselineImplementation = getTypedAssembly baselineResults
            let baseline = createBaseline tcGlobals dllPath

            let service = FSharpEditAndContinueLanguageService.Instance
            service.EndSession()
            service.StartSession(baseline, baselineImplementation) |> ignore

            // Updated compilation
            let updatedResults = compileProject checker fsPath dllPath updatedSource
            let updatedTcGlobals, updatedImplementation = getTypedAssembly updatedResults
            let updatedModule =
                let options : ILReaderOptions =
                    { pdbDirPath = None
                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                      metadataOnly = MetadataOnlyFlag.No
                      tryGetMetadataSnapshot = fun _ -> None }

                use reader = OpenILModuleReader dllPath options
                reader.ILModuleDef

            // The build pipeline clears the active session once the new binary is written; rehydrate it
            // with the previously captured baseline before emitting the delta.
            service.StartSession(baseline, baselineImplementation) |> ignore
            Assert.True(service.IsSessionActive)

            match service.EmitDeltaForCompilation(updatedTcGlobals, updatedImplementation, updatedModule) with
            | Error error -> failwithf "EmitDeltaForCompilation failed: %A" error
            | Ok result ->
                Assert.NotEmpty(result.Delta.Metadata)
                Assert.NotEmpty(result.Delta.IL)
                Assert.NotEmpty(result.Delta.UpdatedMethodTokens)
                let session =
                    match service.TryGetSession() with
                    | ValueSome session -> session
                    | ValueNone -> failwith "Session not found after delta emission."

                Assert.Equal(2, session.CurrentGeneration)
                Assert.True(session.PreviousGenerationId.IsSome)
        finally
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``EmitDeltaForCompilation allows supported method insertion edits`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()

        try
            let baselineResults = compileProject checker fsPath dllPath baselineSource
            let tcGlobals, baselineImplementation = getTypedAssembly baselineResults
            let baseline = createBaseline tcGlobals dllPath

            let service = FSharpEditAndContinueLanguageService.Instance
            service.EndSession()

            // Method insertion requires the runtime to advertise AddMethodToExistingType.
            let insertionCapabilities = EditAndContinueCapabilities.Parse [ "Baseline"; "AddMethodToExistingType" ]
            service.StartSession(baseline, baselineImplementation, insertionCapabilities) |> ignore

            let updatedResults = compileProject checker fsPath dllPath insertedMethodSource
            let updatedTcGlobals, updatedImplementation = getTypedAssembly updatedResults
            let updatedModule =
                let options : ILReaderOptions =
                    { pdbDirPath = None
                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                      metadataOnly = MetadataOnlyFlag.No
                      tryGetMetadataSnapshot = fun _ -> None }

                use reader = OpenILModuleReader dllPath options
                reader.ILModuleDef

            // The build pipeline may clear session state during writes; restore the baseline snapshot before emit.
            service.StartSession(baseline, baselineImplementation, insertionCapabilities) |> ignore

            match service.EmitDeltaForCompilation(updatedTcGlobals, updatedImplementation, updatedModule) with
            | Error error -> failwithf "EmitDeltaForCompilation failed for method insertion: %A" error
            | Ok result ->
                // Roslyn/CLR shape: the AddMethod operation tags the PARENT TypeDef row and
                // is immediately followed by the new Method row with the Default operation.
                let hasMethodAdd =
                    result.Delta.EncLog
                    |> Array.exists (fun (table, _, op) ->
                        table = FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.TypeDef
                        && op = FSharp.Compiler.AbstractIL.ILDeltaHandles.EditAndContinueOperation.AddMethod)

                Assert.True(hasMethodAdd, "Expected (TypeDef, AddMethod) parent operation for inserted method.")

                match result.Delta.UpdatedBaseline with
                | Some updatedBaseline ->
                    let containsInsertedMethod =
                        updatedBaseline.MethodTokens
                        |> Map.exists (fun key _ -> key.DeclaringType = "Sample.Type" && key.Name = "GetExtra")
                    Assert.True(containsInsertedMethod, "Updated baseline missing inserted method token.")
                | None ->
                    Assert.True(false, "Updated baseline missing after method insertion delta.")
        finally
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``EmitDeltaForCompilation rejects method insertion without runtime capability`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()

        try
            let baselineResults = compileProject checker fsPath dllPath baselineSource
            let tcGlobals, baselineImplementation = getTypedAssembly baselineResults
            let baseline = createBaseline tcGlobals dllPath

            let service = FSharpEditAndContinueLanguageService.Instance
            service.EndSession()
            // No capabilities negotiated: the session defaults to baseline-only, so the
            // otherwise-valid method insertion must surface as an unsupported (rude) edit.
            service.StartSession(baseline, baselineImplementation) |> ignore

            let updatedResults = compileProject checker fsPath dllPath insertedMethodSource
            let updatedTcGlobals, updatedImplementation = getTypedAssembly updatedResults
            let updatedModule =
                let options : ILReaderOptions =
                    { pdbDirPath = None
                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                      metadataOnly = MetadataOnlyFlag.No
                      tryGetMetadataSnapshot = fun _ -> None }

                use reader = OpenILModuleReader dllPath options
                reader.ILModuleDef

            service.StartSession(baseline, baselineImplementation) |> ignore

            match service.EmitDeltaForCompilation(updatedTcGlobals, updatedImplementation, updatedModule) with
            | Ok _ ->
                Assert.True(false, "Expected method insertion to be rejected when AddMethodToExistingType is not advertised.")
            | Error (HotReloadError.UnsupportedEdit _) -> ()
            | Error error -> failwithf "Expected UnsupportedEdit, got: %A" error
        finally
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    // Two lambda occurrences in 'transform' (matching the C1 extraction pinned by
    // PdbCdiEmissionTests): the outer List.map argument (occurrence key [0]) and the
    // lambda nested inside it (occurrence key [0; 1]).
    let private lambdaBaselineSource =
        """
module CdiChain

let transform (values: int list) =
    let offset = 1
    values |> List.map (fun v -> List.map (fun u -> u + offset + v) [ v ] |> List.sum)
"""

    // Body-only edit: the lambda set is unchanged, so the delta is a plain MethodBody
    // update and the refreshed occurrence data must show the same two keys.
    let private lambdaUpdatedSource =
        """
module CdiChain

let transform (values: int list) =
    let offset = 2
    values |> List.map (fun v -> List.map (fun u -> u + offset + v) [ v ] |> List.sum)
"""

    [<Fact>]
    let ``EmitDeltaForCompilation chains refreshed EnC method debug information into the next baseline`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let service = FSharpEditAndContinueLanguageService.Instance

        try
            let baselineResults = compileProject checker fsPath dllPath lambdaBaselineSource
            let tcGlobals, baselineImplementation = getTypedAssembly baselineResults
            let baseline = createBaseline tcGlobals dllPath

            // Back-compat: this baseline's in-memory PDB rewrite carries no EnC CDI side
            // channel (and pre-C2 PDBs carry no EnC rows at all), so the session must start
            // fine with an empty per-method map.
            Assert.True(
                Map.isEmpty baseline.EncMethodDebugInfos,
                "expected the CDI-less baseline to expose an empty EnC method debug information map"
            )

            service.EndSession()
            service.StartSession(baseline, baselineImplementation) |> ignore
            Assert.True(service.IsSessionActive)

            let updatedResults = compileProject checker fsPath dllPath lambdaUpdatedSource
            let updatedTcGlobals, updatedImplementation = getTypedAssembly updatedResults

            let updatedModule =
                let options : ILReaderOptions =
                    { pdbDirPath = None
                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                      metadataOnly = MetadataOnlyFlag.No
                      tryGetMetadataSnapshot = fun _ -> None }

                use reader = OpenILModuleReader dllPath options
                reader.ILModuleDef

            // The build pipeline clears the active session once the new binary is written;
            // rehydrate it with the previously captured baseline before emitting the delta.
            service.StartSession(baseline, baselineImplementation) |> ignore

            let transformToken =
                baseline.MethodTokens
                |> Map.toList
                |> List.pick (fun (key, token) -> if key.Name = "transform" then Some token else None)

            match service.EmitDeltaForCompilation(updatedTcGlobals, updatedImplementation, updatedModule) with
            | Error error -> failwithf "EmitDeltaForCompilation failed: %A" error
            | Ok result ->
                let updatedTokens =
                    result.Delta.AddedOrChangedMethods |> List.map (fun info -> info.MethodToken)

                Assert.Contains(transformToken, updatedTokens)

                let chainedBaseline =
                    match service.TryGetBaseline() with
                    | ValueSome chained -> chained
                    | ValueNone -> failwith "Expected an active session baseline after delta emission."

                let info =
                    match Map.tryFind transformToken chainedBaseline.EncMethodDebugInfos with
                    | Some info -> info
                    | None ->
                        failwithf
                            "expected chained EnC method debug information for token 0x%08X, found tokens %A"
                            transformToken
                            (chainedBaseline.EncMethodDebugInfos |> Map.toList |> List.map fst)

                // The chained entry reflects the fresh compile's occurrence data: the body
                // edit adds no lambda, so the same two occurrence keys come back.
                Assert.Equal(2, info.Lambdas.Length)
                Assert.Equal(2, info.Closures.Length)
                Assert.Equal<int list>([ 0 ], EncMethodDebugInformation.decodeOccurrenceKey info.Lambdas[0].SyntaxOffset)
                Assert.Equal<int list>([ 0; 1 ], EncMethodDebugInformation.decodeOccurrenceKey info.Lambdas[1].SyntaxOffset)
        finally
            try service.EndSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``ApplyUpdate succeeds for method body edit`` () =
        // This test requires DOTNET_MODIFIABLE_ASSEMBLIES=debug to be set
        // To run: DOTNET_MODIFIABLE_ASSEMBLIES=debug dotnet test --filter "ApplyUpdate succeeds"
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            // Use the FSharpChecker hot reload API (same as HotReloadDemoApp)
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-applyupdate", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            // Separate runtime copy (matches HotReloadDemoApp pattern)
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")

            try
                File.WriteAllText(fsPath, baselineSource)

                // Get project options with hot reload enabled
                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                // Compile baseline
                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Start hot reload session
                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                // Copy baseline to runtime location and load it (same as HotReloadDemoApp)
                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)

                // Verify baseline method value
                let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                let method = methodType.GetMethod("GetValue", BindingFlags.Public ||| BindingFlags.Static)
                let beforeValue = method.Invoke(null, [||]) :?> int
                Assert.Equal(1, beforeValue)

                // Update source
                File.WriteAllText(fsPath, updatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                // Recompile without hot reload capture (same as HotReloadDemoApp pattern)
                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                // Emit delta
                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    Assert.NotEmpty(delta.Metadata)
                    Assert.NotEmpty(delta.IL)

                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty

                    printfn "[applyupdate-test] Applying delta: metadata=%d IL=%d PDB=%d" delta.Metadata.Length delta.IL.Length pdbBytes.Length

                    // Apply the delta
                    try
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        printfn "[applyupdate-test] ApplyUpdate succeeded!"
                    with
                    | :? InvalidOperationException as ex when ex.Message.Contains("not editable") ->
                        failwithf "Assembly is NOT EnC-capable: %s" ex.Message
                    | :? InvalidOperationException as ex ->
                        failwithf "ApplyUpdate failed (delta rejected): %s" ex.Message

                    // Verify updated method value
                    let afterValue = method.Invoke(null, [||]) :?> int
                    Assert.Equal(2, afterValue)
                    printfn "[applyupdate-test] SUCCESS: value changed from %d to %d" beforeValue afterValue

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    let private moduleValueBaselineSource =
        """
module Sample.Library

let existing () = 1
"""

    let private moduleValueUpdatedSource =
        """
module Sample.Library

let existing () = 1
let mutable newCounter = 41
"""

    let private moduleValueSecondUpdateSource =
        """
module Sample.Library

let existing () = 1
let mutable newCounter = 41
let mutable secondCounter = 7
"""

    let private moduleFunctionUpdatedSource =
        """
module Sample.Library

let existing () = 1
let extra () = 99
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for added module function`` () =
        // Added module functions lower to plain static methods on the module type; this
        // exercises the (TypeDef, AddMethod) + (Method, Default) EncLog pairing at runtime.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-methodadd", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "MethodAddLibrary.dll")
            let runtimeDllPath = Path.Combine(projectDir, "MethodAddLibrary.runtime.dll")

            try
                File.WriteAllText(fsPath, moduleValueBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString moduleValueBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // The unit-argument function classifies through the module-value path, so the
                // static-field capability is required alongside the method capability.
                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "AddStaticFieldToExistingType" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.Library", throwOnError = true)
                let existingMethod = moduleType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(1, existingMethod.Invoke(null, [||]) :?> int)

                File.WriteAllText(fsPath, moduleFunctionUpdatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty

                    try
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        printfn "[methodadd-test] ApplyUpdate succeeded!"
                    with :? InvalidOperationException as ex ->
                        failwithf "ApplyUpdate failed (delta rejected): %s" ex.Message

                    Assert.Equal(1, existingMethod.Invoke(null, [||]) :?> int)

                    let extraMethod = moduleType.GetMethod("extra", BindingFlags.Public ||| BindingFlags.Static)
                    Assert.True(not (isNull extraMethod), "extra method not found after ApplyUpdate.")
                    Assert.Equal(99, extraMethod.Invoke(null, [||]) :?> int)
                    printfn "[methodadd-test] SUCCESS: added module function invocable after ApplyUpdate"
            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``ApplyUpdate succeeds for added module value`` () =
        // Phase B1b: `let mutable newCounter = 41` lowers to static backing fields on the
        // startup-code class plus get_/set_ accessors on the module type. After ApplyUpdate
        // the accessors must be invocable; the observed initialization semantics are
        // asserted below and documented in docs/hot-reload-member-additions.md.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-fieldadd", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            // Unique assembly simple name: other ApplyUpdate tests load a 'Library' assembly
            // into the default load context and LoadFrom refuses same-name/different-path loads.
            let dllPath = Path.Combine(projectDir, "FieldAddLibrary.dll")
            let runtimeDllPath = Path.Combine(projectDir, "FieldAddLibrary.runtime.dll")

            try
                File.WriteAllText(fsPath, moduleValueBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString moduleValueBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Adding a module value needs both the static-field and method capabilities.
                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "AddStaticFieldToExistingType" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)

                let moduleType = assembly.GetType("Sample.Library", throwOnError = true)
                let existingMethod = moduleType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(1, existingMethod.Invoke(null, [||]) :?> int)

                File.WriteAllText(fsPath, moduleValueUpdatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    Assert.NotEmpty(delta.Metadata)
                    Assert.NotEmpty(delta.IL)

                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty

                    printfn
                        "[fieldadd-test] Applying delta: metadata=%d IL=%d PDB=%d"
                        delta.Metadata.Length
                        delta.IL.Length
                        pdbBytes.Length

                    try
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        printfn "[fieldadd-test] ApplyUpdate succeeded!"
                    with :? InvalidOperationException as ex ->
                        failwithf "ApplyUpdate failed (delta rejected): %s" ex.Message

                    // Existing method must still work after the update.
                    Assert.Equal(1, existingMethod.Invoke(null, [||]) :?> int)

                    // The added accessors must be visible and invocable via reflection.
                    let getter = moduleType.GetMethod("get_newCounter", BindingFlags.Public ||| BindingFlags.Static)
                    Assert.True(not (isNull getter), "get_newCounter accessor not found after ApplyUpdate.")
                    let setter = moduleType.GetMethod("set_newCounter", BindingFlags.Public ||| BindingFlags.Static)
                    Assert.True(not (isNull setter), "set_newCounter accessor not found after ApplyUpdate.")

                    // Initialization semantics (matches C# EnC, verified against a Roslyn
                    // delta): the initializer lives in the startup-code class constructor.
                    // If that type has NOT been initialized yet — here the baseline startup
                    // class had no static state, so nothing ever triggered it — the added
                    // constructor runs lazily on first field access and the value reads its
                    // initializer (41). Had the type already been initialized, the added
                    // field would read default(T) instead.
                    let initialValue = getter.Invoke(null, [||]) :?> int
                    printfn "[fieldadd-test] initial newCounter value = %d" initialValue
                    Assert.Equal(41, initialValue)

                    // The accessors are wired to the same backing field.
                    setter.Invoke(null, [| box 7 |]) |> ignore
                    Assert.Equal(7, getter.Invoke(null, [||]) :?> int)
                    printfn "[fieldadd-test] SUCCESS: added module value readable/writable after ApplyUpdate"

                    // ---------------------------------------------------------------
                    // Generation 2: add a SECOND module value. This exercises baseline
                    // chaining: gen-1 field/method/property tokens must resolve so only
                    // the new value is appended, and the startup constructor (added in
                    // gen 1, now part of the chained baseline) is re-emitted as an
                    // UPDATED method body that also initializes the second value.
                    // ---------------------------------------------------------------
                    File.WriteAllText(fsPath, moduleValueSecondUpdateSource)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let compileDiagnostics3, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors3 = compileDiagnostics3 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors3.Length > 0 then failwithf "Second update compilation failed: %A" (errors3 |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "EmitHotReloadDelta (generation 2) failed: %A" error
                    | Ok delta2 ->
                        let pdbBytes2 = delta2.Pdb |> Option.defaultValue Array.empty

                        try
                            MetadataUpdater.ApplyUpdate(assembly, delta2.Metadata.AsSpan(), delta2.IL.AsSpan(), pdbBytes2.AsSpan())
                            printfn "[fieldadd-test] generation 2 ApplyUpdate succeeded!"
                        with :? InvalidOperationException as ex ->
                            failwithf "Generation 2 ApplyUpdate failed (delta rejected): %s" ex.Message

                        // Generation-1 members keep working and keep their state.
                        Assert.Equal(1, existingMethod.Invoke(null, [||]) :?> int)
                        Assert.Equal(7, getter.Invoke(null, [||]) :?> int)

                        let getter2 = moduleType.GetMethod("get_secondCounter", BindingFlags.Public ||| BindingFlags.Static)
                        Assert.True(not (isNull getter2), "get_secondCounter accessor not found after generation 2.")
                        let setter2 = moduleType.GetMethod("set_secondCounter", BindingFlags.Public ||| BindingFlags.Static)
                        Assert.True(not (isNull setter2), "set_secondCounter accessor not found after generation 2.")

                        // The startup class was already type-initialized in generation 1,
                        // so the UPDATED constructor does not re-run: the second value
                        // reads default(int), not its initializer (7) — C# EnC semantics.
                        let initialValue2 = getter2.Invoke(null, [||]) :?> int
                        printfn "[fieldadd-test] initial secondCounter value = %d" initialValue2
                        Assert.Equal(Unchecked.defaultof<int>, initialValue2)

                        setter2.Invoke(null, [| box 13 |]) |> ignore
                        Assert.Equal(13, getter2.Invoke(null, [||]) :?> int)
                        Assert.Equal(7, getter.Invoke(null, [||]) :?> int)
                        printfn "[fieldadd-test] SUCCESS: second module value readable/writable after generation 2"
            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    let private stringLiteralBaselineSource =
        """
namespace Sample

type Type =
    static member GetMessage() = "Hello from generation 0"
"""

    let private stringLiteralUpdatedSource (gen: int) : string =
        $"""
namespace Sample

type Type =
    static member GetMessage() = "Hello from generation {gen}"
"""

    let private applySingleStringUpdateWithCapabilitiesAndAssertRuntimeResult
        (capabilities: string list option)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (baselineExpected: string)
        (updatedExpected: string)
        =
        // These runtime assertions require EnC-capable runtime loading.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for '%s'" testLabel
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-runtime-string-update", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")
            let loadContext = new AssemblyLoadContext($"fsharp-hotreload-runtime-{System.Guid.NewGuid():N}", isCollectible = true)

            try
                File.WriteAllText(fsPath, baselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()

                let baselineCompileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let baselineErrors =
                    baselineCompileDiagnostics
                    |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                if baselineErrors.Length > 0 then
                    failwithf "[%s] baseline compilation failed: %A" testLabel (baselineErrors |> Array.map (fun d -> d.Message))

                let sessionStart =
                    match capabilities with
                    | Some capabilities -> checker.StartHotReloadSession(projectOptions, capabilities = capabilities)
                    | None -> checker.StartHotReloadSession(projectOptions)

                match sessionStart |> Async.RunImmediate with
                | Error error -> failwithf "[%s] failed to start hot reload session: %A" testLabel error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = loadContext.LoadFromAssemblyPath(runtimeDllPath)
                let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                let method = methodType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

                let baselineMessage = method.Invoke(null, [||]) :?> string
                Assert.Equal(baselineExpected, baselineMessage)

                File.WriteAllText(fsPath, updatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let updateCompileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let updateErrors =
                    updateCompileDiagnostics
                    |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                if updateErrors.Length > 0 then
                    failwithf "[%s] updated compilation failed: %A" testLabel (updateErrors |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "[%s] EmitHotReloadDelta failed: %A" testLabel error
                | Ok delta ->
                    Assert.NotEmpty(delta.Metadata)
                    Assert.NotEmpty(delta.IL)

                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                    MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                    let updatedMessage = method.Invoke(null, [||]) :?> string
                    Assert.Equal(updatedExpected, updatedMessage)

            finally
                try loadContext.Unload() with _ -> ()
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    /// Single string update against a default (baseline-only capability) session.
    let private applySingleStringUpdateAndAssertRuntimeResult testLabel baselineSource updatedSource baselineExpected updatedExpected =
        applySingleStringUpdateWithCapabilitiesAndAssertRuntimeResult
            None
            testLabel
            baselineSource
            updatedSource
            baselineExpected
            updatedExpected

    [<Fact>]
    let ``Computation-expression output shape is preserved across desugaring variants`` () =
        let simpleBuilderBaseline =
            """
namespace Sample

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Run(text: string) = text
    member _.Zero() = ""

type Type =
    static member GetMessage() =
        let html = HtmlBuilder()
        html {
            yield "Hello, "
            yield "watch"
        }
"""

        let simpleBuilderUpdated = simpleBuilderBaseline.Replace("Hello, ", "Welcome, ")

        let localLambdaBaseline =
            """
namespace Sample

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Run(text: string) = text
    member _.Zero() = ""

type Type =
    static member GetMessage() =
        let html = HtmlBuilder()
        let prefixFactory = fun () -> "Hello, "
        html {
            yield prefixFactory()
            yield "watch"
        }
"""

        let localLambdaUpdated = localLambdaBaseline.Replace("Hello, ", "Welcome, ")

        let asyncBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        async {
            do! Async.Sleep 1
            let prefix = "Hello"
            return prefix + ", watch"
        }
        |> Async.RunSynchronously
"""

        let asyncUpdated = asyncBaseline.Replace("Hello", "Welcome")

        let scenarios =
            [ ("ce-simple", simpleBuilderBaseline, simpleBuilderUpdated)
              ("ce-local-lambda", localLambdaBaseline, localLambdaUpdated)
              ("ce-async", asyncBaseline, asyncUpdated) ]

        for (label, baseline, updated) in scenarios do
            applySingleStringUpdateAndAssertRuntimeResult label baseline updated "Hello, watch" "Welcome, watch"

    [<Fact>]
    let ``Tier1 construct matrix preserves runtime apply for method-body edits`` () =
        let seqBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        seq {
            yield "Hello"
            yield ", "
            yield "watch"
        }
        |> String.concat ""
"""

        let recordBaseline =
            """
namespace Sample

type Greeting = { Prefix: string; Name: string }

type Type =
    static member GetMessage() =
        let greeting = { Prefix = "Hello"; Name = "watch" }
        $"{greeting.Prefix}, {greeting.Name}"
"""

        let unionBaseline =
            """
namespace Sample

type Greeting =
    | Message of string * string

type Type =
    static member GetMessage() =
        let value = Message("Hello", "watch")
        match value with
        | Message(prefix, name) -> $"{prefix}, {name}"
"""

        let structBaseline =
            """
namespace Sample

[<Struct>]
type Greeting =
    { Prefix: string
      Name: string }

type Type =
    static member GetMessage() =
        let greeting = { Prefix = "Hello"; Name = "watch" }
        greeting.Prefix + ", " + greeting.Name
"""

        let recursiveBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        let rec prefix i =
            if i = 0 then "Hello" else prefix (i - 1)

        let rec suffix i =
            if i = 0 then "watch" else suffix (i - 1)

        prefix 1 + ", " + suffix 1
"""

        let scenarios =
            [ ("tier1-seq", seqBaseline)
              ("tier1-record", recordBaseline)
              ("tier1-union", unionBaseline)
              ("tier1-struct", structBaseline)
              ("tier1-recursive", recursiveBaseline) ]

        for (label, baseline) in scenarios do
            let updated = baseline.Replace("Hello", "Welcome")
            applySingleStringUpdateAndAssertRuntimeResult label baseline updated "Hello, watch" "Welcome, watch"

    [<Fact>]
    let ``Tier2 construct matrix preserves runtime apply for method-body edits`` () =
        let anonymousRecordBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        let greeting = {| Prefix = "Hello"; Name = "watch" |}
        greeting.Prefix + ", " + greeting.Name
"""

        let activePatternBaseline =
            """
namespace Sample

module Internal =
    let (|SplitGreeting|) (text: string) = text.Split(',')

type Type =
    static member GetMessage() =
        match "Hello,watch" with
        | Internal.SplitGreeting parts -> parts.[0] + ", " + parts.[1]
"""

        let objectExpressionBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        let provider =
            { new obj() with
                override _.ToString() = "Hello" }

        provider.ToString() + ", watch"
"""

        let loopBaseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        let parts = ResizeArray<string>()
        for value in [ "Hello"; "watch" ] do
            parts.Add(value)
        parts.[0] + ", " + parts.[1]
"""

        let quotationBaseline =
            """
namespace Sample

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type Type =
    static member GetMessage() =
        let prefix = "Hello"
        let quotation = <@ "watch" @>

        let suffix =
            match quotation with
            | Value(value, _) -> value :?> string
            | _ -> "watch"

        prefix + ", " + suffix
"""

        let scenarios =
            [ ("tier2-anon-record", anonymousRecordBaseline)
              ("tier2-active-pattern", activePatternBaseline)
              ("tier2-object-expression", objectExpressionBaseline)
              ("tier2-loop", loopBaseline)
              ("tier2-quotation", quotationBaseline) ]

        for (label, baseline) in scenarios do
            let updated = baseline.Replace("Hello", "Welcome")
            applySingleStringUpdateAndAssertRuntimeResult label baseline updated "Hello, watch" "Welcome, watch"

    [<Fact>]
    let ``Multi-generation user string literals resolve correctly`` () =
        // This test verifies that user string literals are correctly resolved across
        // multiple delta generations. The bug manifests as CJK character corruption
        // at generation 2+ when stream header sizes don't match padded byte arrays.
        // Requires DOTNET_MODIFIABLE_ASSEMBLIES=debug
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-multigen-userstring", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "StringLiteralMultiGen.fs")
            let dllPath = Path.Combine(projectDir, "StringLiteralMultiGen.dll")
            let runtimeDllPath = Path.Combine(projectDir, "StringLiteralMultiGen.runtime.dll")

            try
                File.WriteAllText(fsPath, stringLiteralBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString stringLiteralBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                // Compile baseline
                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Start hot reload session
                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                // Copy baseline to runtime location and load it
                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                let method = methodType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

                // Verify baseline string
                let baselineMessage = method.Invoke(null, [||]) :?> string
                Assert.Equal("Hello from generation 0", baselineMessage)
                printfn "[multigen-userstring] Baseline: %s" baselineMessage

                // Helper to apply a generation delta
                let applyGeneration gen =
                    let newSource = stringLiteralUpdatedSource gen
                    File.WriteAllText(fsPath, newSource)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    // Recompile without hot reload capture
                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    // Emit delta
                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        printfn "[multigen-userstring] Gen %d: metadata=%d IL=%d PDB=%d" gen delta.Metadata.Length delta.IL.Length pdbBytes.Length

                        // Apply the delta
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                        // Verify the string is correct
                        let message = method.Invoke(null, [||]) :?> string
                        let expectedMessage = sprintf "Hello from generation %d" gen
                        printfn "[multigen-userstring] Gen %d result: %s (expected: %s)" gen message expectedMessage

                        // This assertion will fail at gen 2 if stream header sizes are not aligned
                        Assert.Equal(expectedMessage, message)

                // Apply generations 1, 2, 3 - the bug manifests at generation 2
                applyGeneration 1
                applyGeneration 2
                applyGeneration 3

                printfn "[multigen-userstring] SUCCESS: All 3 generations applied correctly"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    // Regression test for delta signature-blob remapping against SDK-shaped baselines.
    //
    // Baselines produced by a real fsc disk build carry TypeRef rows that the in-memory rewrite
    // performed during delta emission does not regenerate (debug import scopes referencing the
    // F# auto-open types produce duplicate names under different scopes/namespaces - e.g.
    // 'LowPriority' twice - plus nested TypeRefs such as 'LanguagePrimitives'/'IntrinsicOperators').
    // A generic call like 'sprintf' adds a MethodSpec whose instantiation blob embeds
    // TypeDefOrRefOrSpec coded indexes; before the fix those indexes were copied raw from the fresh
    // compile and resolved against the differently-shaped baseline table, producing
    // "The generic type 'IntrinsicOperators' was used with the wrong number of generic arguments"
    // at runtime.
    // The leading GetValue member mirrors the SDK build shape: emitting its (TypeRef-free) body
    // forces the document's debug import scopes to allocate the F# auto-open TypeRefs
    // (LanguagePrimitives/IntrinsicOperators, the TaskBuilderExtensions priority types, ...)
    // BEFORE GetMessage's body TypeRefs, shifting every later row relative to the in-memory
    // rewrite performed at delta-emission time (which carries no debug import scopes).
    // Array.singleton/Array.length instantiated at System.DateTime put a body-allocated
    // (and therefore shifted) TypeRef coded index inside the MethodSpec instantiation blobs.
    let private sprintfMethodSpecBaselineSource =
        """
namespace Sample

type Type =
    static member GetValue() = 1

    static member GetMessage() =
        let mutable count = 0
        count <- count + 1
        let stamp = Array.singleton System.DateTime.MinValue |> Array.length
        sprintf "Hello, %s (gen %d/%d)" "watch" count stamp
"""

    let private sprintfMethodSpecUpdatedSource =
        sprintfMethodSpecBaselineSource.Replace("Hello", "Welcome")

    /// Collects every TypeDefOrRefOrSpec coded index embedded in a MethodSpec instantiation blob.
    let private collectMethodSpecSignatureCodedIndexes (blob: byte[]) : int list =
        let mutable pos = 0

        let readByte () =
            let value = int blob[pos]
            pos <- pos + 1
            value

        let readCompressed () =
            let first = readByte ()

            if first &&& 0x80 = 0 then first
            elif first &&& 0xC0 = 0x80 then ((first &&& 0x3F) <<< 8) ||| readByte ()
            else
                let b2 = readByte ()
                let b3 = readByte ()
                let b4 = readByte ()
                ((first &&& 0x1F) <<< 24) ||| (b2 <<< 16) ||| (b3 <<< 8) ||| b4

        let collected = ResizeArray<int>()

        let rec walkType () =
            match readByte () with
            // CLASS / VALUETYPE carry a coded index
            | 0x11
            | 0x12 -> collected.Add(readCompressed ())
            // GENERICINST: kind byte, coded index, argument count, arguments
            | 0x15 ->
                match readByte () with
                | 0x11
                | 0x12 -> ()
                | kind -> failwithf "unexpected GENERICINST kind 0x%02X in MethodSpec blob" kind

                collected.Add(readCompressed ())
                let argCount = readCompressed ()

                for _ in 1..argCount do
                    walkType ()
            // PTR / BYREF / SZARRAY wrap a single type
            | 0x0F
            | 0x10
            | 0x1D -> walkType ()
            // VAR / MVAR carry an ordinal
            | 0x13
            | 0x1E -> readCompressed () |> ignore
            // primitives and other simple element types
            | 0x01 | 0x02 | 0x03 | 0x04 | 0x05 | 0x06 | 0x07 | 0x08 | 0x09 | 0x0A | 0x0B | 0x0C | 0x0D | 0x0E
            | 0x16 | 0x18 | 0x19 | 0x1C -> ()
            | code -> failwithf "unexpected element type 0x%02X in MethodSpec blob" code

        let prolog = readByte ()

        if prolog <> 0x0A then
            failwithf "expected MethodSpec GENERICINST prolog 0x0A, got 0x%02X" prolog

        let argCount = readCompressed ()

        for _ in 1..argCount do
            walkType ()

        List.ofSeq collected

    [<Fact>]
    let ``Delta MethodSpec signature blob remaps TypeRefs against SDK-shaped baseline tables`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                keepAllBackgroundResolutions = false,
                keepAllBackgroundSymbolUses = false,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                enablePartialTypeChecking = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir =
            Path.Combine(Path.GetTempPath(), "fsharp-hotreload-methodspec-remap", System.Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")

        let loadContext =
            new AssemblyLoadContext($"fsharp-hotreload-methodspec-remap-{System.Guid.NewGuid():N}", isCollectible = true)

        try
            File.WriteAllText(fsPath, sprintfMethodSpecBaselineSource)

            let projectOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fsPath,
                    SourceText.ofString sprintfMethodSpecBaselineSource,
                    assumeDotNetFramework = false,
                    useSdkRefs = true,
                    useFsiAuxLib = false
                )
                |> Async.RunImmediate

            let projectOptions =
                { projectOptions with
                    SourceFiles = [| fsPath |]
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.append
                            [| "--target:library"
                               "--langversion:preview"
                               "--optimize-"
                               "--debug:portable"
                               "--deterministic"
                               "--enable:hotreloaddeltas"
                               $"--out:{dllPath}" |] }

            checker.InvalidateAll()

            let baselineCompileDiagnostics, _ =
                checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                |> Async.RunImmediate

            let baselineErrors =
                baselineCompileDiagnostics
                |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

            if baselineErrors.Length > 0 then
                failwithf "baseline compilation failed: %A" (baselineErrors |> Array.map (fun d -> d.Message))

            match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "failed to start hot reload session: %A" error
            | Ok () -> ()

            File.Copy(dllPath, runtimeDllPath, true)
            let pdbPath = Path.ChangeExtension(dllPath, ".pdb")

            if File.Exists(pdbPath) then
                File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

            // Capture the baseline TypeRef table shape from the on-disk fsc output.
            let baselineTypeRefNames, baselineNestedTypeRefNames, baselineBlobHeapSize =
                use peReader = new PEReader(File.OpenRead runtimeDllPath)
                let reader = peReader.GetMetadataReader()

                let names =
                    reader.TypeReferences
                    |> Seq.map (fun handle ->
                        let typeRef = reader.GetTypeReference handle
                        let rowId = MetadataTokens.GetRowNumber(TypeReferenceHandle.op_Implicit handle : EntityHandle)
                        rowId, reader.GetString typeRef.Name)
                    |> Map.ofSeq

                let nestedNames =
                    reader.TypeReferences
                    |> Seq.choose (fun handle ->
                        let typeRef = reader.GetTypeReference handle

                        if typeRef.ResolutionScope.Kind = HandleKind.TypeReference then
                            Some(reader.GetString typeRef.Name)
                        else
                            None)
                    |> Set.ofSeq

                names, nestedNames, reader.GetHeapSize(HeapIndex.Blob)

            // Precondition guards: this regression only has power when the baseline table has the
            // SDK-build shape (nested TypeRefs and duplicate names across scopes from debug import
            // scopes). If these fire, the baseline compilation no longer produces that shape and the
            // test needs a new misalignment source.
            Assert.True(
                baselineNestedTypeRefNames.Contains "IntrinsicOperators",
                "Expected the fsc-built baseline to contain a nested 'IntrinsicOperators' TypeRef (debug import scopes)."
            )

            // The import-scope rows must precede the body rows for the table shapes to diverge;
            // otherwise a raw (unremapped) blob copy would still resolve correctly by accident.
            let rowIdOfName name =
                baselineTypeRefNames
                |> Map.toList
                |> List.tryPick (fun (rowId, rowName) -> if rowName = name then Some rowId else None)

            match rowIdOfName "IntrinsicOperators", rowIdOfName "DateTime" with
            | Some intrinsicRow, Some dateTimeRow ->
                let tableDump =
                    baselineTypeRefNames
                    |> Map.toList
                    |> List.map (fun (rowId, name) -> sprintf "%d:%s" rowId name)
                    |> String.concat ", "

                Assert.True(
                    intrinsicRow < dateTimeRow,
                    $"Expected import-scope TypeRefs (IntrinsicOperators row {intrinsicRow}) to precede body TypeRefs (DateTime row {dateTimeRow}); the baseline no longer has the SDK-build shape. Table: {tableDump}"
                )
            | intrinsic, dateTime ->
                failwithf "Baseline TypeRef table missing expected rows (IntrinsicOperators=%A, DateTime=%A)." intrinsic dateTime

            let duplicateNames =
                baselineTypeRefNames
                |> Map.toList
                |> List.countBy snd
                |> List.filter (fun (_, count) -> count > 1)

            Assert.True(
                not duplicateNames.IsEmpty,
                "Expected the fsc-built baseline TypeRef table to contain duplicate type names under different scopes."
            )

            // Apply the method-body edit and rebuild on disk, like dotnet-watch does.
            File.WriteAllText(fsPath, sprintfMethodSpecUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

            let updatedOptions =
                { projectOptions with
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.filter (fun opt ->
                            not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

            let updateCompileDiagnostics, _ =
                checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                |> Async.RunImmediate

            let updateErrors =
                updateCompileDiagnostics
                |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

            if updateErrors.Length > 0 then
                failwithf "updated compilation failed: %A" (updateErrors |> Array.map (fun d -> d.Message))

            match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                Assert.NotEmpty(delta.Metadata)
                Assert.NotEmpty(delta.IL)

                use deltaProvider =
                    MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange delta.Metadata)

                let deltaReader = deltaProvider.GetMetadataReader()
                let methodSpecCount = deltaReader.GetTableRowCount(TableIndex.MethodSpec)
                Assert.True(methodSpecCount > 0, "Expected the sprintf edit to add a MethodSpec row to the delta.")

                let baselineTypeRefCount = baselineTypeRefNames.Count

                let deltaTypeRefNames =
                    deltaReader.TypeReferences
                    |> Seq.map (fun handle -> deltaReader.GetString (deltaReader.GetTypeReference handle).Name)
                    |> Seq.toArray

                let resolveTypeRefName (rowId: int) =
                    if rowId >= 1 && rowId <= baselineTypeRefCount then
                        baselineTypeRefNames[rowId]
                    elif rowId > baselineTypeRefCount && rowId <= baselineTypeRefCount + deltaTypeRefNames.Length then
                        deltaTypeRefNames[rowId - baselineTypeRefCount - 1]
                    else
                        failwithf "MethodSpec blob references TypeRef row %d outside baseline+delta tables" rowId

                let codedIndexes =
                    [ for rowNumber in 1..methodSpecCount do
                          let methodSpec =
                              deltaReader.GetMethodSpecification(MetadataTokens.MethodSpecificationHandle rowNumber)

                          // Delta heap offsets are absolute (baseline heap length + delta-local offset).
                          let absoluteOffset = MetadataTokens.GetHeapOffset(methodSpec.Signature)
                          let localOffset = absoluteOffset - baselineBlobHeapSize

                          Assert.True(
                              localOffset > 0,
                              $"Expected the MethodSpec signature blob to live in the delta heap (absolute={absoluteOffset}, baselineBlobHeap={baselineBlobHeapSize})."
                          )

                          let blob = deltaReader.GetBlobBytes(MetadataTokens.BlobHandle localOffset)
                          yield! collectMethodSpecSignatureCodedIndexes blob ]

                // The generic instantiations in the edited body are Array.singleton<DateTime>,
                // Array.length<DateTime>, and sprintf at 'T = string -> int -> int -> string, so
                // the type references embedded in the delta MethodSpec instantiation blobs must
                // resolve - against the BASELINE TypeRef table, not the fresh compile's - to
                // exactly DateTime and FSharpFunc`2.
                Assert.NotEmpty codedIndexes

                let resolvedNames =
                    [ for coded in codedIndexes do
                          Assert.Equal(1, coded &&& 0x3) // TypeRef tag
                          resolveTypeRefName (coded >>> 2) ]

                let unexpectedNames =
                    resolvedNames
                    |> List.filter (fun name -> name <> "DateTime" && name <> "FSharpFunc`2")

                Assert.True(
                    unexpectedNames.IsEmpty,
                    $"""MethodSpec instantiation blob coded indexes resolve to baseline TypeRefs [{String.concat "; " unexpectedNames}] instead of DateTime/FSharpFunc`2."""
                )

                Assert.Contains("DateTime", resolvedNames)

                // Runtime verification mirrors the production failure (TypeLoadException naming
                // 'IntrinsicOperators') and requires an EnC-capable runtime configuration.
                let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")

                if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
                    printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for the runtime apply check"
                else
                    let assembly = loadContext.LoadFromAssemblyPath(runtimeDllPath)
                    let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                    let method = methodType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

                    let baselineMessage = method.Invoke(null, [||]) :?> string
                    Assert.Equal("Hello, watch (gen 1/1)", baselineMessage)

                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                    MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                    // Before the signature-blob remap fix this invoke crashed with
                    // "System.TypeLoadException: The generic type 'IntrinsicOperators' was used
                    // with the wrong number of generic arguments".
                    let updatedMessage = method.Invoke(null, [||]) :?> string
                    Assert.Equal("Welcome, watch (gen 1/1)", updatedMessage)

        finally
            try loadContext.Unload() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // Phase C4: added lambdas emit new closure classes in deltas
    // -----------------------------------------------------------------------------

    // Direct nested applications (no |> chains, no let-bound stages): pipeline
    // desugaring introduces "Pipe #N stage #M" continuation closures and let-bound
    // stages change the closure base names; both have separate identity concerns.
    // This shape keeps every lambda a plain source occurrence of 'transform'.
    let private closureAdditionBaselineSource =
        """
module Sample.Closures

let transform (input: int list) =
    List.map (fun x -> x * 2 + List.length input) (List.filter (fun x -> x > 0) input)

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    // Generation 1: a capture-free lambda is ADDED as the innermost stage, in front of
    // the surviving filter and map lambdas in occurrence order (the case pure sequence
    // replay cannot handle; the C3 allocator names the new occurrence
    // {base}@hotreload#g1_o{i}).
    let private closureAdditionUpdatedSource =
        """
module Sample.Closures

let transform (input: int list) =
    List.map (fun x -> x * 2 + List.length input) (List.filter (fun x -> x > 0) (List.map (fun x -> x + 1) input))

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    // Generation 2: body-edit of the lambda ADDED in generation 1 (its closure class
    // is now part of the chained baseline, so this must be an in-place method update).
    let private closureAdditionSecondUpdateSource =
        """
module Sample.Closures

let transform (input: int list) =
    List.map (fun x -> x * 2 + List.length input) (List.filter (fun x -> x > 0) (List.map (fun x -> x + 5) input))

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for added lambda creating a new closure class`` () =
        // Phase C4 payoff: a member with two lambdas gains a third one. The delta must
        // carry a NEW TypeDef row for the generation-suffixed closure class (plus its
        // .ctor/Invoke methods and NestedClass row), the updated parent method body,
        // and apply cleanly via MetadataUpdater. A further generation then body-edits
        // the ADDED lambda, proving its closure class chained into the baseline.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-added-lambda", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ClosureAddition.fs")
            let dllPath = Path.Combine(projectDir, "ClosureAddition.dll")
            let runtimeDllPath = Path.Combine(projectDir, "ClosureAddition.runtime.dll")

            try
                File.WriteAllText(fsPath, closureAdditionBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString closureAdditionBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                // Compile baseline
                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Added lambdas need the new-type and method capabilities (the closure
                // class plus its .ctor/Invoke methods).
                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "NewTypeDefinition" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.Closures", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                // Baseline: filter [1;2;3] -> map (*2+3) = [5;7;9], sum 21.
                Assert.Equal(21, probe.Invoke(null, [||]) :?> int)

                let baselineClosureCount =
                    moduleType.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
                    |> Array.filter (fun t -> t.Name.Contains("@hotreload"))
                    |> Array.length

                let applyGeneration gen (source: string) expectedValue =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        printfn "[added-lambda] Gen %d: metadata=%d IL=%d PDB=%d" gen delta.Metadata.Length delta.IL.Length pdbBytes.Length

                        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DUMP_DIR") with
                        | null | "" -> ()
                        | dumpDir ->
                            Directory.CreateDirectory(dumpDir) |> ignore
                            File.Copy(runtimeDllPath, Path.Combine(dumpDir, "baseline.dll"), true)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dmeta"), delta.Metadata)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dil"), delta.IL)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dpdb"), pdbBytes)

                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                        let value = probe.Invoke(null, [||]) :?> int
                        printfn "[added-lambda] Gen %d result: %d (expected %d)" gen value expectedValue
                        Assert.Equal(expectedValue, value)
                        delta

                // Generation 1: added lambda runs — map(+1) [1;2;3] = [2;3;4],
                // filter (>0) keeps all, map (*2+3) = [7;9;11], sum 27.
                let delta1 = applyGeneration 1 closureAdditionUpdatedSource 27

                // The delta must carry a NEW TypeDef row for the generation-suffixed
                // closure class.
                let baselineTypeDefCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    peReader.GetMetadataReader().GetTableRowCount(TableIndex.TypeDef)

                // Read the delta metadata's EncLog with SRM: the new closure class appears
                // as a TypeDef row PAST the baseline table with the plain Default (= 0)
                // operation (row content applied via ApplyTableDelta).
                let readEncLog (metadata: byte[]) =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                    |> Seq.toArray

                let gen1EncLog = readEncLog delta1.Metadata

                let newTypeDefEntries =
                    gen1EncLog
                    |> Array.filter (fun (token, op) ->
                        token >>> 24 = 0x02
                        && (token &&& 0x00FFFFFF) > baselineTypeDefCount
                        && op = 0)

                Assert.True(
                    newTypeDefEntries.Length >= 1,
                    sprintf "Expected a new TypeDef row in the generation-1 EncLog; got %A" gen1EncLog)

                // Pattern parity with the recorded C# reference template
                // (docs/hot-reload-member-additions.md): the new TypeDef row's plain
                // Default entry precedes every Add* entry that names it as the parent;
                // each AddField/AddMethod entry is immediately followed by the member row
                // with the Default operation; a NestedClass row trails (F# closure classes
                // are nested in their module type); the Add* parent entries never appear
                // in EncMap.
                let newTypeToken = fst newTypeDefEntries.[0]
                let newTypeRowIndex = gen1EncLog |> Array.findIndex (fun entry -> entry = (newTypeToken, 0))

                let addMemberIndices =
                    gen1EncLog
                    |> Array.indexed
                    |> Array.choose (fun (index, (token, op)) ->
                        // AddMethod = 1, AddField = 2 (EditAndContinueOperation)
                        if token = newTypeToken && (op = 1 || op = 2) then Some(index, op) else None)

                Assert.True(addMemberIndices.Length >= 3, "Expected AddField/AddMethod pairs for the closure members.")

                for index, op in addMemberIndices do
                    Assert.True(newTypeRowIndex < index, "New TypeDef row must precede its Add* entries.")
                    let memberToken, memberOp = gen1EncLog.[index + 1]
                    Assert.Equal(0, memberOp)
                    let expectedTable = if op = 2 then 0x04 else 0x06
                    Assert.Equal(expectedTable, memberToken >>> 24)

                Assert.Contains(gen1EncLog, fun (token, op) -> token >>> 24 = 0x29 && op = 0)

                let gen1EncMap =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> delta1.Metadata)

                    provider.GetMetadataReader().GetEditAndContinueMapEntries()
                    |> Seq.map MetadataTokens.GetToken
                    |> Seq.toArray

                Assert.Contains(newTypeToken, gen1EncMap)
                Assert.Contains(gen1EncMap, fun token -> token >>> 24 = 0x29)

                // NOTE: reflection does not reliably enumerate EnC-added nested types
                // (GetNestedTypes on the live type still reports the baseline set), so the
                // structural evidence is the EncLog above and the behavioral evidence is the
                // probe result; the baseline closure count is only pinned for context.
                let closuresAfterGen1 =
                    moduleType.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
                    |> Array.filter (fun t -> t.Name.Contains("@hotreload"))

                Assert.True(
                    closuresAfterGen1.Length >= baselineClosureCount,
                    "Baseline closure classes must remain visible after the update.")

                // Generation 2: body edit of the ADDED lambda — map(+5) [1;2;3] = [6;7;8],
                // filter keeps all, map (*2+3) = [15;17;19], sum 51. Its closure class is
                // now part of the chained baseline, so NO new TypeDef row may appear.
                let delta2 = applyGeneration 2 closureAdditionSecondUpdateSource 51

                let gen2EncLog = readEncLog delta2.Metadata

                let gen2TypeDefAdds =
                    gen2EncLog
                    |> Array.filter (fun (token, _) ->
                        token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefCount + 1)

                Assert.True(
                    Array.isEmpty gen2TypeDefAdds,
                    sprintf "Generation 2 must update the added closure in place; got %A" gen2EncLog)

                printfn "[added-lambda] SUCCESS: closure addition + in-place follow-up edit applied"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``EmitHotReloadDelta rejects added lambda without NewTypeDefinition capability`` () =
        // Negative gate for Phase C4: a capability-less session (BaselineOnly) must report
        // the addition as NotSupportedByRuntime naming the missing capability, never emit.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-added-lambda-nocap", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ClosureAdditionNoCap.fs")
            let dllPath = Path.Combine(projectDir, "ClosureAdditionNoCap.dll")

            try
                File.WriteAllText(fsPath, closureAdditionBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString closureAdditionBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // No capabilities negotiated: the session defaults to baseline-only.
                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.WriteAllText(fsPath, closureAdditionUpdatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Ok _ -> failwith "Expected the added lambda to be rejected without the NewTypeDefinition capability."
                | Error (FSharpHotReloadError.UnsupportedEdit message) ->
                    Assert.Contains("NewTypeDefinition", message)
                    // FSHRDL016 = RudeEditKind.NotSupportedByRuntime
                    Assert.Contains("FSHRDL016", message)
                | Error other -> failwithf "Expected UnsupportedEdit, got %A" other

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // TypeSpec row emission: added lambdas whose closure classes extend a generic
    // instantiation with NO matching baseline TypeSpec row (the common case for any
    // brand-new FSharpFunc<A,B> shape).
    // -----------------------------------------------------------------------------

    // Baseline carries only int -> int closures (FSharpFunc<int32,int32> TypeSpec rows).
    let private newInstantiationBaselineSource =
        """
module Sample.NewInstantiation

let transform (input: int list) =
    List.map (fun x -> x + 1) input

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    // Generation 1: the added List.filter lambda's closure class extends
    // FSharpFunc<int32,bool> - an instantiation absent from the baseline TypeSpec
    // table, so the delta must APPEND a TypeSpec row (Default op, C# template parity).
    let private newInstantiationGen1Source =
        """
module Sample.NewInstantiation

let transform (input: int list) =
    List.map (fun x -> x + 1) (List.filter (fun x -> x % 2 = 0) input)

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    // Generation 2: ANOTHER added lambda with another brand-new instantiation
    // (FSharpFunc<int32, FSharpList<int32>> for List.collect); its TypeSpec row id
    // must continue past the row appended in generation 1 (baseline chaining). The
    // new stage is added INNERMOST so the surviving lambdas keep their source
    // occurrence order (the C3 closure-identity constraint, same shape as the C4
    // added-lambda test).
    let private newInstantiationGen2Source =
        """
module Sample.NewInstantiation

let transform (input: int list) =
    List.map (fun x -> x + 1) (List.filter (fun x -> x % 2 = 0) (List.collect (fun x -> [ x; x * 10 ]) input))

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for added lambdas with new generic instantiations across generations`` () =
        // TypeSpec emission payoff: each generation adds a lambda whose closure class
        // extends a generic instantiation that no baseline TypeSpec row matches. The
        // delta must carry an appended TypeSpec row with the plain Default operation
        // (mirroring the recorded C# reference template's "TypeSpec 0x1b000001 Default"
        // entry), apply cleanly via MetadataUpdater, and chain the row forward so the
        // next generation reuses it instead of failing or duplicating.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-new-instantiation", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "NewInstantiation.fs")
            let dllPath = Path.Combine(projectDir, "NewInstantiation.dll")
            let runtimeDllPath = Path.Combine(projectDir, "NewInstantiation.runtime.dll")

            try
                File.WriteAllText(fsPath, newInstantiationBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString newInstantiationBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "NewTypeDefinition" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.NewInstantiation", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                // Baseline: map(+1) [1;2;3] = [2;3;4], sum 9.
                Assert.Equal(9, probe.Invoke(null, [||]) :?> int)

                let baselineTypeSpecCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    peReader.GetMetadataReader().GetTableRowCount(TableIndex.TypeSpec)

                let applyGeneration gen (source: string) expectedValue =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        printfn "[typespec-add] Gen %d: metadata=%d IL=%d PDB=%d" gen delta.Metadata.Length delta.IL.Length pdbBytes.Length

                        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DUMP_DIR") with
                        | null | "" -> ()
                        | dumpDir ->
                            Directory.CreateDirectory(dumpDir) |> ignore
                            File.Copy(runtimeDllPath, Path.Combine(dumpDir, "baseline.dll"), true)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dmeta"), delta.Metadata)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dil"), delta.IL)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dpdb"), pdbBytes)

                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                        let value = probe.Invoke(null, [||]) :?> int
                        printfn "[typespec-add] Gen %d result: %d (expected %d)" gen value expectedValue
                        Assert.Equal(expectedValue, value)
                        delta

                // mdv-style structural validation on the raw delta metadata: the appended
                // TypeSpec row appears in the EncLog with the plain Default (= 0) operation
                // and a row id PAST the chained TypeSpec table, the same token is present in
                // the (token-sorted) EncMap, and the EncMap is complete - every appended
                // TypeSpec row in the delta's physical table has exactly one EncMap entry.
                let assertTypeSpecAdd (metadata: byte[]) (minRowIdExclusive: int) =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

                    let reader = provider.GetMetadataReader()

                    let typeSpecEncLog =
                        reader.GetEditAndContinueLogEntries()
                        |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                        |> Seq.filter (fun (token, _) -> token >>> 24 = 0x1B)
                        |> Seq.toArray

                    let added =
                        typeSpecEncLog
                        |> Array.filter (fun (token, op) -> (token &&& 0x00FFFFFF) > minRowIdExclusive && op = 0)

                    Assert.True(
                        added.Length >= 1,
                        sprintf "Expected an appended TypeSpec row (Default op, rowId > %d); got %A" minRowIdExclusive typeSpecEncLog)

                    let encMapTokens =
                        reader.GetEditAndContinueMapEntries()
                        |> Seq.map MetadataTokens.GetToken
                        |> Seq.toArray

                    let encMapTypeSpecs = encMapTokens |> Array.filter (fun token -> token >>> 24 = 0x1B)

                    for token, _ in typeSpecEncLog do
                        Assert.Contains(token, encMapTypeSpecs)

                    // EncMap completeness for the TypeSpec table: one entry per physical row.
                    Assert.Equal(reader.GetTableRowCount TableIndex.TypeSpec, encMapTypeSpecs.Length)

                    // EncMap must be token-sorted (readers reject unsorted maps).
                    Assert.Equal<int[]>(encMapTokens |> Array.sort, encMapTokens)

                    added |> Array.map fst

                // Generation 1: filter [1;2;3] = [2], map(+1) = [3], sum 3.
                let delta1 = applyGeneration 1 newInstantiationGen1Source 3
                let gen1Added = assertTypeSpecAdd delta1.Metadata baselineTypeSpecCount
                let gen1MaxRowId = gen1Added |> Array.map (fun token -> token &&& 0x00FFFFFF) |> Array.max

                // Generation 2: collect [1;2;3] = [1;10;2;20;3;30], filter (%2=0) =
                // [10;2;20;30], map(+1) = [11;3;21;31], sum 66. The new TypeSpec row
                // chains PAST generation 1's appended rows; the FSharpFunc<int32,bool>
                // row added in generation 1 is reused, never re-appended.
                let delta2 = applyGeneration 2 newInstantiationGen2Source 66
                let gen2Added = assertTypeSpecAdd delta2.Metadata gen1MaxRowId

                Assert.True(
                    gen2Added |> Array.forall (fun token -> (token &&& 0x00FFFFFF) > gen1MaxRowId),
                    "Generation 2 TypeSpec rows must chain past generation 1's appended rows.")

                printfn "[typespec-add] SUCCESS: new generic instantiations applied across two generations"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``Disk-started session reconstructs closure names and applies an added lambda`` () =
        // Phase C6 payoff (the dotnet-watch topology): the baseline is built by the
        // command-line fsc path and the session starts from the ON-DISK dll+pdb only -
        // the in-process capture session is explicitly cleared first, so no in-memory
        // stamp -> name state can leak across (this is exactly the cross-process case
        // where the FCS session lives in a different process than the fsc build).
        // The chain -> closure-name tables must be reconstructed from the persisted
        // EnC CDI occurrence keys alone, an added lambda must emit its new closure
        // TypeDef through the C4 machinery, ApplyUpdate must succeed, and a follow-up
        // generation must body-edit the ADDED closure in place.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-disk-start", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ClosureDiskStart.fs")
            let dllPath = Path.Combine(projectDir, "ClosureDiskStart.dll")
            let runtimeDllPath = Path.Combine(projectDir, "ClosureDiskStart.runtime.dll")

            try
                File.WriteAllText(fsPath, closureAdditionBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString closureAdditionBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                // Build the baseline through the command-line fsc path (flag-on, on-disk
                // dll + pdb).
                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                Assert.True(File.Exists(Path.ChangeExtension(dllPath, ".pdb")), "Baseline PDB must exist on disk.")

                // Simulate the process boundary: drop ALL in-process session state the
                // capture compile created. From here on, the dll + pdb on disk are the
                // only baseline inputs (no MVID-matched carry-over is possible).
                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                | ValueSome _ -> failwith "Expected no in-process session after the reset."
                | ValueNone -> ()

                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "NewTypeDefinition" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session from disk: %A" error
                | Ok () -> ()

                // Direct reconstruction evidence: the disk-started session carries the
                // chain -> name tables re-derived from the CDI occurrence keys, naming
                // both baseline occurrences with the generation-0 derived names.
                let session =
                    match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                    | ValueSome session -> session
                    | ValueNone -> failwith "Expected the disk-started session to be active."

                let reconstructedNames =
                    session.Baseline.EncClosureNames
                    |> Map.toList
                    |> List.collect (fun (_, table) -> table |> Map.toList |> List.map snd)
                    |> Set.ofList

                Assert.False(
                    Map.isEmpty session.Baseline.EncClosureNames,
                    "Disk-started session must reconstruct closure-name tables from the baseline CDI."
                )

                Assert.Contains("transform@hotreload#g0_o0", reconstructedNames)
                Assert.Contains("transform@hotreload#g0_o1", reconstructedNames)

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.Closures", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                // Baseline: filter [1;2;3] -> map (*2+3) = [5;7;9], sum 21.
                Assert.Equal(21, probe.Invoke(null, [||]) :?> int)

                // The baseline closure classes carry the occurrence-derived names the
                // reconstruction claimed.
                let baselineClosureNames =
                    moduleType.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
                    |> Array.filter (fun t -> t.Name.Contains("@hotreload"))
                    |> Array.map (fun t -> t.Name)
                    |> Set.ofArray

                Assert.Contains("transform@hotreload#g0_o0", baselineClosureNames)
                Assert.Contains("transform@hotreload#g0_o1", baselineClosureNames)

                let applyGeneration gen (source: string) expectedValue =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        Assert.Equal(expectedValue, probe.Invoke(null, [||]) :?> int)
                        delta

                // Generation 1 (the cross-process add): map(+1) [1;2;3] = [2;3;4],
                // filter (>0) keeps all, map (*2+3) = [7;9;11], sum 27.
                let delta1 = applyGeneration 1 closureAdditionUpdatedSource 27

                let baselineTypeDefCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    peReader.GetMetadataReader().GetTableRowCount(TableIndex.TypeDef)

                let readEncLog (metadata: byte[]) =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                    |> Seq.toArray

                // The delta must carry a NEW TypeDef row (the generation-suffixed
                // closure class) past the baseline table, with AddField/AddMethod pairs.
                let gen1EncLog = readEncLog delta1.Metadata

                let newTypeDefEntries =
                    gen1EncLog
                    |> Array.filter (fun (token, op) ->
                        token >>> 24 = 0x02
                        && (token &&& 0x00FFFFFF) > baselineTypeDefCount
                        && op = 0)

                Assert.True(
                    newTypeDefEntries.Length >= 1,
                    sprintf "Expected a new TypeDef row in the cross-process generation-1 EncLog; got %A" gen1EncLog)

                // Generation 2: body edit of the lambda ADDED via the disk-started chain
                // - map(+5) [1;2;3] = [6;7;8], filter keeps all, map (*2+3) = [15;17;19],
                // sum 51. Its closure class chained into the baseline, so the edit must
                // be an in-place method update (no further TypeDef rows).
                let delta2 = applyGeneration 2 closureAdditionSecondUpdateSource 51

                let gen2TypeDefAdds =
                    readEncLog delta2.Metadata
                    |> Array.filter (fun (token, _) ->
                        token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefCount + 1)

                Assert.True(
                    Array.isEmpty gen2TypeDefAdds,
                    "Generation 2 of the disk-started chain must update the added closure in place.")

                printfn "[disk-start] SUCCESS: CDI-reconstructed names + cross-process closure addition applied"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``Disk-started session applies a removed lambda and keeps editing survivors`` () =
        // C6 removal counterpart of the cross-process addition test: the baseline (three
        // lambdas, built by the command-line fsc path) is session-started from disk only;
        // a generation removes the innermost lambda (no new metadata - the baseline
        // closure class just goes unused) and a follow-up generation body-edits a
        // SURVIVING lambda, proving the reconstructed tables re-keyed the shifted
        // survivors correctly across the removal.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-disk-removal", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ClosureDiskRemoval.fs")
            let dllPath = Path.Combine(projectDir, "ClosureDiskRemoval.dll")
            let runtimeDllPath = Path.Combine(projectDir, "ClosureDiskRemoval.runtime.dll")

            // Baseline: three lambda occurrences - probe = sum (map (*2+3) (filter (>0)
            // (map (+1) [1;2;3]))) = sum [7;9;11] = 27.
            let baselineSource = closureAdditionUpdatedSource

            // Generation 1: the innermost map(+1) lambda is REMOVED -
            // sum (map (*2+3) (filter (>0) [1;2;3])) = sum [5;7;9] = 21.
            let removalSource = closureAdditionBaselineSource

            // Generation 2: body edit of the surviving map lambda (*4 instead of *2) -
            // sum (map (*4+3) [1;2;3]) = sum [7;11;15] = 33.
            let survivorEditSource =
                """
module Sample.Closures

let transform (input: int list) =
    List.map (fun x -> x * 4 + List.length input) (List.filter (fun x -> x > 0) input)

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

            try
                File.WriteAllText(fsPath, baselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Process boundary: disk artifacts only from here on.
                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session from disk: %A" error
                | Ok () -> ()

                // The disk-started session reconstructed tables for all THREE baseline
                // occurrences.
                let session =
                    match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                    | ValueSome session -> session
                    | ValueNone -> failwith "Expected the disk-started session to be active."

                let reconstructedNames =
                    session.Baseline.EncClosureNames
                    |> Map.toList
                    |> List.collect (fun (_, table) -> table |> Map.toList |> List.map snd)
                    |> List.filter (fun name -> name.StartsWith("transform@", StringComparison.Ordinal))

                Assert.Equal(3, reconstructedNames.Length)

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.Closures", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                Assert.Equal(27, probe.Invoke(null, [||]) :?> int)

                let applyGeneration gen (source: string) expectedValue =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        Assert.Equal(expectedValue, probe.Invoke(null, [||]) :?> int)
                        delta

                // Generation 1: removal - no new TypeDef rows may appear (the removed
                // lambda's baseline closure class simply goes unused).
                let delta1 = applyGeneration 1 removalSource 21

                let baselineTypeDefCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    peReader.GetMetadataReader().GetTableRowCount(TableIndex.TypeDef)

                let typeDefAdds (metadata: byte[]) =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle)
                    |> Seq.filter (fun token -> token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefCount)
                    |> Seq.toArray

                Assert.Empty(typeDefAdds delta1.Metadata)

                // Generation 2: the surviving map lambda's body edit stays an in-place
                // update across the removal-induced occurrence re-keying.
                let delta2 = applyGeneration 2 survivorEditSource 33
                Assert.Empty(typeDefAdds delta2.Metadata)

                printfn "[disk-removal] SUCCESS: cross-process lambda removal + survivor edit applied"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``In-process and disk-started sessions reconstruct identical closure-name tables`` () =
        // The C6 determinism property: closure names are a pure function of occurrence
        // identity, so the tables an in-process capture session carries (derived during
        // the emitting compile, validated against the stamp -> name recording) and the
        // tables a DISK-started session reconstructs from the CDI occurrence keys must
        // be identical, token for token and chain for chain - including nested
        // occurrence chains (depth 2). No runtime apply is needed; the equality of the
        // session tables IS the property.
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                keepAllBackgroundResolutions = false,
                keepAllBackgroundSymbolUses = false,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                enablePartialTypeChecking = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-name-determinism", System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "ClosureNameDeterminism.fs")
        let dllPath = Path.Combine(projectDir, "ClosureNameDeterminism.dll")

        // Two top-level occurrences plus a NESTED one (a closure formed inside the map
        // lambda's body, capturing its parameter), so the pin covers the chain-rendered
        // name format ({base}@hotreload#g0_o{c0}_{c1}).
        let source =
            """
module Sample.Determinism

let transform (input: int list) =
    List.concat (List.map (fun x -> List.map (fun y -> y + x) input) (List.filter (fun x -> x > 0) input))

let probe () = List.sum (transform [ 1; 2; 3 ])
"""

        try
            File.WriteAllText(fsPath, source)

            let projectOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fsPath,
                    SourceText.ofString source,
                    assumeDotNetFramework = false,
                    useSdkRefs = true,
                    useFsiAuxLib = false
                )
                |> Async.RunImmediate

            let projectOptions =
                { projectOptions with
                    SourceFiles = [| fsPath |]
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.append
                            [| "--target:library"
                               "--langversion:preview"
                               "--optimize-"
                               "--debug:portable"
                               "--deterministic"
                               "--enable:hotreloaddeltas"
                               $"--out:{dllPath}" |] }

            checker.InvalidateAll()
            let compileDiagnostics, _ =
                checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                |> Async.RunImmediate

            let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

            // Tables of the IN-PROCESS capture session created by the flag-on compile.
            let inProcessTables =
                match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                | ValueSome session -> session.Baseline.EncClosureNames
                | ValueNone -> failwith "Expected the flag-on compile to start an in-process capture session."

            Assert.False(Map.isEmpty inProcessTables, "In-process capture must carry closure-name tables.")

            // Process boundary, then tables of the DISK-started session for the same
            // output assembly.
            FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

            match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to start hot reload session from disk: %A" error
            | Ok () -> ()

            let diskStartedTables =
                match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                | ValueSome session -> session.Baseline.EncClosureNames
                | ValueNone -> failwith "Expected the disk-started session to be active."

            Assert.Equal<Map<int, Map<int list, string>>>(inProcessTables, diskStartedTables)

            // The pin must actually cover nested chains: at least one table entry is
            // keyed by a depth-2 occurrence chain and renders both ordinals.
            let nestedEntries =
                diskStartedTables
                |> Map.toList
                |> List.collect (fun (_, table) -> table |> Map.toList)
                |> List.filter (fun (chain, _) -> List.length chain = 2)

            Assert.False(List.isEmpty nestedEntries, "Expected a depth-2 occurrence chain in the reconstructed tables.")

            for chain, name in nestedEntries do
                Assert.Contains($"""@hotreload#g0_o{chain |> List.map string |> String.concat "_"}""", name)
        finally
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``ApplyUpdate succeeds for removed lambda leaving baseline closure unused`` () =
        // Removed-only lambda sets need no new metadata (C# parity: the deleted lambda's
        // closure class just becomes unreachable). The delta is a plain set of method
        // updates - no TypeDef/Field rows - and applies cleanly.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-removed-lambda", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ClosureRemoval.fs")
            let dllPath = Path.Combine(projectDir, "ClosureRemoval.dll")
            let runtimeDllPath = Path.Combine(projectDir, "ClosureRemoval.runtime.dll")

            try
                // Baseline: the THREE-lambda shape; the update removes the innermost map.
                File.WriteAllText(fsPath, closureAdditionUpdatedSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString closureAdditionUpdatedSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                // Removed-only edits need no capabilities beyond baseline.
                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Sample.Closures", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                // Baseline (3 lambdas): map(+1) -> filter -> map(*2+3) over [1;2;3] = 27.
                Assert.Equal(27, probe.Invoke(null, [||]) :?> int)

                File.WriteAllText(fsPath, closureAdditionBaselineSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    // No new types: the removed lambda's baseline closure class stays, unused.
                    let encLog =
                        use provider =
                            MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> delta.Metadata)

                        provider.GetMetadataReader().GetEditAndContinueLogEntries()
                        |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                        |> Seq.toArray

                    let typeDefEntries = encLog |> Array.filter (fun (token, _) -> token >>> 24 = 0x02)

                    Assert.True(
                        Array.isEmpty typeDefEntries,
                        sprintf "Removed-lambda delta must not touch the TypeDef table; got %A" encLog)

                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                    MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                    // 2 lambdas: filter -> map(*2+3) over [1;2;3] = 21.
                    Assert.Equal(21, probe.Invoke(null, [||]) :?> int)
                    printfn "[removed-lambda] SUCCESS: lambda removal applied as plain body update"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // Phase B2: added instance fields on existing classes
    // -----------------------------------------------------------------------------

    let private instanceFieldBaselineSource =
        """
module Sample.Fields

type Counter() =
    member _.Snapshot() = -1
"""

    // Generation 1: `let mutable` field — initializer folds into the primary ctor,
    // and Snapshot's body update reads the new field.
    let private instanceFieldUpdatedSource =
        """
module Sample.Fields

type Counter() =
    let mutable total = 41
    member _.Snapshot() = total
"""

    // Generation 2: a SECOND field; gen-1's field token must resolve from the chained
    // baseline so only the new field row is appended.
    let private instanceFieldSecondUpdateSource =
        """
module Sample.Fields

type Counter() =
    let mutable total = 41
    let mutable extra = 7
    member _.Snapshot() = total + extra * 1000
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for added instance field`` () =
        // Phase B2: adding `let mutable total = 41` to a class appends an instance Field
        // row ((TypeDef, AddField) + (Field, Default), the recorded C# field_add template)
        // paired with the primary-constructor body update that runs the initializer.
        // Runtime semantics match C# EnC: EXISTING instances read default(T) for the new
        // field; newly constructed instances run the updated ctor and see the initializer.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-instancefield", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "InstanceFieldLibrary.dll")
            let runtimeDllPath = Path.Combine(projectDir, "InstanceFieldLibrary.runtime.dll")

            try
                File.WriteAllText(fsPath, instanceFieldBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString instanceFieldBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let counterType = assembly.GetType("Sample.Fields+Counter", throwOnError = true)
                let snapshot (instance: obj) =
                    counterType.GetMethod("Snapshot", BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||]) :?> int

                // Instance constructed BEFORE the update: must keep working afterwards
                // with the new field zeroed.
                let preUpdateInstance = Activator.CreateInstance(counterType)
                Assert.Equal(-1, snapshot preUpdateInstance)

                let baselineFieldRowCount, baselineTypeDefRowCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    let reader = peReader.GetMetadataReader()
                    reader.GetTableRowCount(TableIndex.Field), reader.GetTableRowCount(TableIndex.TypeDef)

                let applyGeneration gen (source: string) =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        printfn "[instancefield] Gen %d: metadata=%d IL=%d PDB=%d" gen delta.Metadata.Length delta.IL.Length pdbBytes.Length

                        try
                            MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        with :? InvalidOperationException as ex ->
                            failwithf "Gen %d ApplyUpdate failed (delta rejected): %s" gen ex.Message

                        delta

                // ---------------- Generation 1 ----------------
                let delta1 = applyGeneration 1 instanceFieldUpdatedSource

                // EncLog pattern parity with the recorded C# field_add template:
                // (TypeDef, AddField=2) immediately followed by the new Field row.
                let encLog1 =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> delta1.Metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                    |> Seq.toArray

                let addFieldIndex =
                    encLog1
                    |> Array.tryFindIndex (fun (token, op) -> token >>> 24 = 0x02 && op = 2)

                match addFieldIndex with
                | None -> failwithf "Expected (TypeDef, AddField) pair in gen-1 EncLog; got %A" encLog1
                | Some index ->
                    let fieldToken, fieldOp = encLog1.[index + 1]
                    Assert.Equal(0x04, fieldToken >>> 24)
                    Assert.Equal(baselineFieldRowCount + 1, fieldToken &&& 0x00FFFFFF)
                    Assert.Equal(0, fieldOp)

                // No new TypeDef rows: the field lands on the existing class.
                let newTypeDefRows =
                    encLog1
                    |> Array.filter (fun (token, _) ->
                        token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefRowCount)

                Assert.True(Array.isEmpty newTypeDefRows, sprintf "Expected no new TypeDef rows; got %A" encLog1)

                // The pre-update instance never ran the new initializer: zeroed field (C#
                // EnC semantics), updated body reads 0.
                Assert.Equal(0, snapshot preUpdateInstance)

                // A new instance runs the UPDATED constructor and sees the initializer.
                let postUpdateInstance = Activator.CreateInstance(counterType)
                Assert.Equal(41, snapshot postUpdateInstance)

                // The added field is real instance state: writable via reflection.
                let totalField = counterType.GetField("total", BindingFlags.NonPublic ||| BindingFlags.Instance)
                Assert.True(not (isNull totalField), "Added field 'total' not found via reflection after ApplyUpdate.")
                totalField.SetValue(postUpdateInstance, 5)
                Assert.Equal(5, snapshot postUpdateInstance)
                printfn "[instancefield] SUCCESS gen 1: zeroed existing instance, initialized new instance"

                // ---------------- Generation 2 ----------------
                let _delta2 = applyGeneration 2 instanceFieldSecondUpdateSource

                // Gen-1 instance state survives; the gen-2 field reads default(int).
                Assert.Equal(5, snapshot postUpdateInstance)

                // A fresh instance runs the gen-2 constructor: 41 + 7 * 1000.
                let gen2Instance = Activator.CreateInstance(counterType)
                Assert.Equal(7041, snapshot gen2Instance)
                printfn "[instancefield] SUCCESS gen 2: chained second instance field"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    let private defaultValueFieldBaselineSource =
        """
module Sample.Slots

type Holder() =
    member this.Read() = -1
"""

    // Generation 1: [<DefaultValue>] val mutable — no constructor pairing needed and no
    // method-body change at all: the delta is just the appended Field row.
    let private defaultValueFieldUpdatedSource =
        """
module Sample.Slots

type Holder() =
    [<DefaultValue>] val mutable Slot: int
    member this.Read() = -1
"""

    // Generation 2: a body update reads the field added in generation 1.
    let private defaultValueFieldSecondUpdateSource =
        """
module Sample.Slots

type Holder() =
    [<DefaultValue>] val mutable Slot: int
    member this.Read() = this.Slot
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for added DefaultValue field without method updates`` () =
        // Phase B2: a [<DefaultValue>] val mutable field needs no initializer pairing —
        // the generation-1 delta carries ONLY the appended Field row (no method updates),
        // exercising the TypeDefinition-edit-only path through the delta builder.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-defaultvaluefield", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "DefaultValueFieldLibrary.dll")
            let runtimeDllPath = Path.Combine(projectDir, "DefaultValueFieldLibrary.runtime.dll")

            try
                File.WriteAllText(fsPath, defaultValueFieldBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString defaultValueFieldBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                let capabilities = [ "Baseline"; "AddInstanceFieldToExistingType" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let holderType = assembly.GetType("Sample.Slots+Holder", throwOnError = true)
                let read (instance: obj) =
                    holderType.GetMethod("Read", BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||]) :?> int

                let instance = Activator.CreateInstance(holderType)
                Assert.Equal(-1, read instance)

                let applyGeneration gen (source: string) =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty

                        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DUMP_DIR") with
                        | null | "" -> ()
                        | dumpDir ->
                            Directory.CreateDirectory(dumpDir) |> ignore
                            File.Copy(runtimeDllPath, Path.Combine(dumpDir, "baseline.dll"), true)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dmeta"), delta.Metadata)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dil"), delta.IL)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dpdb"), pdbBytes)

                        try
                            MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        with :? InvalidOperationException as ex ->
                            failwithf "Gen %d ApplyUpdate failed (delta rejected): %s" gen ex.Message

                        delta

                // Generation 1: field-only delta.
                let delta1 = applyGeneration 1 defaultValueFieldUpdatedSource

                let encLog1 =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> delta1.Metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                    |> Seq.toArray

                Assert.Contains(encLog1, fun (token, op) -> token >>> 24 = 0x02 && op = 2)
                Assert.Contains(encLog1, fun (token, op) -> token >>> 24 = 0x04 && op = 0)

                let methodUpdates = encLog1 |> Array.filter (fun (token, _) -> token >>> 24 = 0x06)
                Assert.True(Array.isEmpty methodUpdates, sprintf "Expected a field-only delta; got %A" encLog1)

                // The live instance gained the field (zeroed), readable and writable via
                // reflection.
                let slotField = holderType.GetField("Slot", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull slotField), "Added field 'Slot' not found via reflection after ApplyUpdate.")
                Assert.Equal(0, slotField.GetValue(instance) :?> int)
                slotField.SetValue(instance, 9)
                Assert.Equal(9, slotField.GetValue(instance) :?> int)

                // Generation 2: a body update reads the gen-1 field on the SAME instance.
                applyGeneration 2 defaultValueFieldSecondUpdateSource |> ignore
                Assert.Equal(9, read instance)
                printfn "[defaultvaluefield] SUCCESS: field-only delta applied, state visible to gen-2 body"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // Phase B3: added properties and events on existing classes
    // -----------------------------------------------------------------------------

    /// Shared scaffold for the member-addition runtime tests: compiles the baseline with
    /// --enable:hotreloaddeltas, starts a session with the given runtime capabilities,
    /// loads the runtime copy, and hands the test an applyGeneration function that
    /// compiles updated source, emits the delta, applies it, and returns the delta.
    let private runMemberAdditionScenario
        (projectPrefix: string)
        (assemblyName: string)
        (capabilities: string list)
        (baselineSource: string)
        (action: Assembly -> (int -> string -> CodeAnalysis.FSharpHotReloadDelta) -> unit)
        =
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), projectPrefix, System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, assemblyName + ".dll")
            let runtimeDllPath = Path.Combine(projectDir, assemblyName + ".runtime.dll")

            try
                File.WriteAllText(fsPath, baselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()
                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)

                let applyGeneration gen (source: string) =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let updatedOptions =
                        { projectOptions with
                            OtherOptions =
                                projectOptions.OtherOptions
                                |> Array.filter (fun opt ->
                                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        printfn "[%s] Gen %d: metadata=%d IL=%d PDB=%d" projectPrefix gen delta.Metadata.Length delta.IL.Length pdbBytes.Length

                        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DUMP_DIR") with
                        | null | "" -> ()
                        | dumpDir ->
                            Directory.CreateDirectory(dumpDir) |> ignore
                            File.Copy(runtimeDllPath, Path.Combine(dumpDir, "baseline.dll"), true)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dmeta"), delta.Metadata)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dil"), delta.IL)
                            File.WriteAllBytes(Path.Combine(dumpDir, $"gen{gen}.dpdb"), pdbBytes)

                        try
                            MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        with :? InvalidOperationException as ex ->
                            failwithf "Gen %d ApplyUpdate failed (delta rejected): %s" gen ex.Message

                        delta

                action assembly applyGeneration
            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    let private readDeltaEncLog (metadata: byte[]) =
        use provider =
            MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

        provider.GetMetadataReader().GetEditAndContinueLogEntries()
        |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
        |> Seq.toArray

    /// Asserts the EncLog contains an (Add* parent, op) entry immediately followed by a
    /// Default entry for the added member row in the given table.
    let private assertAddPair (encLog: (int * int)[]) (parentTable: int) (operation: int) (memberTable: int) =
        let index =
            encLog
            |> Array.tryFindIndex (fun (token, op) -> token >>> 24 = parentTable && op = operation)

        match index with
        | None -> failwithf "Expected (table 0x%02X, op %d) EncLog entry; got %A" parentTable operation encLog
        | Some i ->
            let token, op = encLog.[i + 1]
            Assert.Equal(memberTable, token >>> 24)
            Assert.Equal(0, op)

    [<Fact>]
    let ``ApplyUpdate succeeds for added properties`` () =
        // Phase B3: generation 1 adds a getter-only property; generation 2 adds a
        // getter+setter property over the same state. Mirrors the recorded C# prop_add
        // EncLog template: (TypeDef, AddMethod)+(MethodDef, Default) per accessor,
        // (PropertyMap, AddProperty)+(Property, Default), MethodSemantics rows binding
        // the accessors. The PropertyMap row is NEW in generation 1 (the baseline class
        // has no properties) and reused from the chained baseline in generation 2.
        let baselineSource =
            """
module Sample.Props

type Widget() =
    let mutable state = 1
    member _.State() = state
"""

        let gen1Source =
            """
module Sample.Props

type Widget() =
    let mutable state = 1
    member _.State() = state
    member _.Doubled = state * 2
"""

        let gen2Source =
            """
module Sample.Props

type Widget() =
    let mutable state = 1
    member _.State() = state
    member _.Doubled = state * 2
    member _.Current
        with get () = state
        and set value = state <- value
"""

        runMemberAdditionScenario
            "fsharp-hotreload-propadd"
            "PropertyAddLibrary"
            [ "Baseline"; "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let widgetType = assembly.GetType("Sample.Props+Widget", throwOnError = true)
                let instance = Activator.CreateInstance(widgetType)
                let state () =
                    widgetType.GetMethod("State", BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||]) :?> int

                Assert.Equal(1, state ())

                // ---------------- Generation 1: getter-only property ----------------
                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata

                // AddMethod pair for the getter, AddProperty pair for the Property row,
                // and MethodSemantics binding (recorded C# template shape).
                assertAddPair encLog1 0x02 1 0x06 // (TypeDef, AddMethod) + MethodDef
                assertAddPair encLog1 0x15 4 0x17 // (PropertyMap, AddProperty) + Property
                Assert.Contains(encLog1, fun (token, op) -> token >>> 24 = 0x18 && op = 0) // MethodSemantics

                // The baseline class had no properties: the NEW PropertyMap row is logged
                // as a plain Default entry BEFORE the AddProperty entry referencing it.
                let mapRowIndex = encLog1 |> Array.findIndex (fun (token, op) -> token >>> 24 = 0x15 && op = 0)
                let addPropertyIndex = encLog1 |> Array.findIndex (fun (token, op) -> token >>> 24 = 0x15 && op = 4)
                Assert.True(mapRowIndex < addPropertyIndex, sprintf "PropertyMap row must precede AddProperty; got %A" encLog1)

                // The added property is reachable through ordinary reflection on the LIVE
                // instance: MethodSemantics rows wired the getter.
                let doubled = widgetType.GetProperty("Doubled", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull doubled), "Added property 'Doubled' not found after ApplyUpdate.")
                Assert.True(doubled.CanRead && not doubled.CanWrite, "Expected a getter-only property.")
                Assert.Equal(2, doubled.GetValue(instance) :?> int)
                printfn "[propadd] SUCCESS gen 1: getter-only property readable via reflection"

                // ---------------- Generation 2: getter+setter property ----------------
                let delta2 = applyGeneration 2 gen2Source
                let encLog2 = readDeltaEncLog delta2.Metadata

                // The PropertyMap row added in generation 1 chained into the baseline: the
                // AddProperty parent reuses it, no NEW PropertyMap row may be emitted.
                assertAddPair encLog2 0x15 4 0x17
                Assert.DoesNotContain(encLog2, fun (token, op) -> token >>> 24 = 0x15 && op = 0)

                let current = widgetType.GetProperty("Current", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull current), "Added property 'Current' not found after generation 2.")
                Assert.True(current.CanRead && current.CanWrite, "Expected a readable+writable property.")
                Assert.Equal(1, current.GetValue(instance) :?> int)
                current.SetValue(instance, 21)
                Assert.Equal(21, state ())
                Assert.Equal(42, doubled.GetValue(instance) :?> int)
                printfn "[propadd] SUCCESS gen 2: getter+setter property writable via reflection")

    [<Fact>]
    let ``ApplyUpdate succeeds for added auto-property`` () =
        // Phase B3 composing with the B2 field machinery: `member val` lowers to a
        // backing instance field (initialized in the primary ctor) plus get_/set_
        // accessors plus the Property row - the full recorded C# prop_add template
        // (AddField pair + two AddMethod pairs + AddProperty pair + MethodSemantics).
        // C# EnC semantics: existing instances read default(T) through the added
        // property; new instances run the updated ctor and see the initializer.
        let baselineSource =
            """
module Sample.AutoProps

type Gadget() =
    member _.Probe() = 0
"""

        let gen1Source =
            """
module Sample.AutoProps

type Gadget() =
    member _.Probe() = 0
    member val Slot = 41 with get, set
"""

        runMemberAdditionScenario
            "fsharp-hotreload-autoprop"
            "AutoPropertyAddLibrary"
            [ "Baseline"; "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let gadgetType = assembly.GetType("Sample.AutoProps+Gadget", throwOnError = true)
                let preUpdateInstance = Activator.CreateInstance(gadgetType)

                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata

                // Full template: backing field + both accessors + property + semantics.
                assertAddPair encLog1 0x02 2 0x04 // (TypeDef, AddField) + Field
                assertAddPair encLog1 0x02 1 0x06 // (TypeDef, AddMethod) + MethodDef
                assertAddPair encLog1 0x15 4 0x17 // (PropertyMap, AddProperty) + Property

                let addMethodPairs = encLog1 |> Array.filter (fun (token, op) -> token >>> 24 = 0x02 && op = 1)
                Assert.Equal(2, addMethodPairs.Length)

                let semanticsRows = encLog1 |> Array.filter (fun (token, op) -> token >>> 24 = 0x18 && op = 0)
                Assert.Equal(2, semanticsRows.Length)

                let slot = gadgetType.GetProperty("Slot", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull slot), "Added auto-property 'Slot' not found after ApplyUpdate.")

                // Existing instance: zeroed backing field (its ctor never ran the
                // initializer) - C# EnC semantics.
                Assert.Equal(0, slot.GetValue(preUpdateInstance) :?> int)

                // New instance: the updated ctor runs the initializer.
                let postUpdateInstance = Activator.CreateInstance(gadgetType)
                Assert.Equal(41, slot.GetValue(postUpdateInstance) :?> int)

                // The property is real instance state, writable through the setter.
                slot.SetValue(preUpdateInstance, 9)
                Assert.Equal(9, slot.GetValue(preUpdateInstance) :?> int)
                Assert.Equal(41, slot.GetValue(postUpdateInstance) :?> int)
                printfn "[autoprop] SUCCESS: zeroed existing instance, initialized new instance, writable property")

    [<Fact>]
    let ``ApplyUpdate succeeds for added CLIEvent`` () =
        // Phase B3: a [<CLIEvent>] member lowers to a backing Event<_,_> instance field
        // (initialized in the primary ctor) plus add_/remove_ accessors plus the Event
        // row; MethodSemantics rows bind the adder/remover and the EventMap row parents
        // the AddEvent entry.
        let baselineSource =
            """
module Sample.Events

open System

type Notifier() =
    let existing = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member _.Existing = existing.Publish
    member _.TriggerExisting() = existing.Trigger(null, EventArgs.Empty)
"""

        let gen1Source =
            """
module Sample.Events

open System

type Notifier() =
    let existing = Event<EventHandler, EventArgs>()
    let added = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member _.Existing = existing.Publish
    member _.TriggerExisting() = existing.Trigger(null, EventArgs.Empty)
    [<CLIEvent>]
    member _.Added = added.Publish
    member _.TriggerAdded() = added.Trigger(null, EventArgs.Empty)
"""

        runMemberAdditionScenario
            "fsharp-hotreload-clievent"
            "CliEventAddLibrary"
            [ "Baseline"; "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let notifierType = assembly.GetType("Sample.Events+Notifier", throwOnError = true)

                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata

                // Backing field + accessor methods + Event row + semantics.
                assertAddPair encLog1 0x02 2 0x04 // (TypeDef, AddField) + Field
                assertAddPair encLog1 0x02 1 0x06 // (TypeDef, AddMethod) + MethodDef
                assertAddPair encLog1 0x12 5 0x14 // (EventMap, AddEvent) + Event

                let semanticsRows = encLog1 |> Array.filter (fun (token, op) -> token >>> 24 = 0x18 && op = 0)
                Assert.Equal(2, semanticsRows.Length)

                // New instance: ctor initializes the added backing field; the event is
                // subscribable through ordinary reflection and the trigger fires it.
                let instance = Activator.CreateInstance(notifierType)
                let addedEvent = notifierType.GetEvent("Added", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull addedEvent), "Added event 'Added' not found after ApplyUpdate.")

                let mutable fired = 0
                let handler = EventHandler(fun _ _ -> fired <- fired + 1)
                addedEvent.AddEventHandler(instance, handler)

                notifierType.GetMethod("TriggerAdded", BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||])
                |> ignore

                Assert.Equal(1, fired)

                addedEvent.RemoveEventHandler(instance, handler)
                notifierType.GetMethod("TriggerAdded", BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||])
                |> ignore

                Assert.Equal(1, fired)
                printfn "[clievent] SUCCESS: added CLIEvent subscribable and firing via reflection")

    [<Fact>]
    let ``ApplyUpdate succeeds for mixed member additions across generations`` () =
        // Coverage consolidation: a three-generation chain mixing the member-addition
        // machineries. Gen 1 adds a method, gen 2 adds an instance field plus a method
        // reading it, gen 3 adds an auto-property (field + accessors + property row).
        // Every generation must chain through the updated baseline (earlier added rows
        // resolve in place) and live instance state must survive each apply.
        let baselineSource =
            """
module Sample.Mixed

type Hub() =
    member _.Version() = 0
"""

        let gen1Source =
            """
module Sample.Mixed

type Hub() =
    member _.Version() = 1
    member _.Extra() = 10
"""

        let gen2Source =
            """
module Sample.Mixed

type Hub() =
    let mutable tally = 5
    member _.Version() = 2
    member _.Extra() = 10
    member _.Tally() = tally
    member _.Bump() = tally <- tally + 1
"""

        let gen3Source =
            """
module Sample.Mixed

type Hub() =
    let mutable tally = 5
    member _.Version() = 3
    member _.Extra() = 10
    member _.Tally() = tally
    member _.Bump() = tally <- tally + 1
    member val Slot = 7 with get, set
"""

        runMemberAdditionScenario
            "fsharp-hotreload-mixedadd"
            "MixedAdditionLibrary"
            [ "Baseline"; "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let hubType = assembly.GetType("Sample.Mixed+Hub", throwOnError = true)
                let invoke (instance: obj) name =
                    hubType.GetMethod(name, BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||])

                let instance = Activator.CreateInstance(hubType)
                Assert.Equal(0, invoke instance "Version" :?> int)

                // ---------------- Generation 1: method addition ----------------
                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata
                assertAddPair encLog1 0x02 1 0x06 // (TypeDef, AddMethod) + MethodDef

                Assert.Equal(1, invoke instance "Version" :?> int)
                Assert.Equal(10, invoke instance "Extra" :?> int)

                // ---------------- Generation 2: field + methods ----------------
                let delta2 = applyGeneration 2 gen2Source
                let encLog2 = readDeltaEncLog delta2.Metadata
                assertAddPair encLog2 0x02 2 0x04 // (TypeDef, AddField) + Field
                assertAddPair encLog2 0x02 1 0x06 // (TypeDef, AddMethod) + MethodDef

                Assert.Equal(2, invoke instance "Version" :?> int)
                Assert.Equal(10, invoke instance "Extra" :?> int)

                // The pre-gen2 instance reads the zeroed field; bumps mutate real state.
                Assert.Equal(0, invoke instance "Tally" :?> int)
                invoke instance "Bump" |> ignore
                invoke instance "Bump" |> ignore
                Assert.Equal(2, invoke instance "Tally" :?> int)

                // A fresh instance runs the gen-2 ctor.
                let gen2Instance = Activator.CreateInstance(hubType)
                Assert.Equal(5, invoke gen2Instance "Tally" :?> int)

                // ---------------- Generation 3: auto-property ----------------
                let delta3 = applyGeneration 3 gen3Source
                let encLog3 = readDeltaEncLog delta3.Metadata
                assertAddPair encLog3 0x02 2 0x04 // backing field
                assertAddPair encLog3 0x02 1 0x06 // accessors
                assertAddPair encLog3 0x15 4 0x17 // (PropertyMap, AddProperty) + Property
                Assert.Contains(encLog3, fun (token, op) -> token >>> 24 = 0x18 && op = 0)

                Assert.Equal(3, invoke instance "Version" :?> int)

                // State added in gen 2 survives the gen-3 apply on both instances.
                Assert.Equal(2, invoke instance "Tally" :?> int)
                Assert.Equal(5, invoke gen2Instance "Tally" :?> int)

                let slot = hubType.GetProperty("Slot", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull slot), "Added auto-property 'Slot' not found after generation 3.")
                Assert.Equal(0, slot.GetValue(instance) :?> int)

                let gen3Instance = Activator.CreateInstance(hubType)
                Assert.Equal(5, invoke gen3Instance "Tally" :?> int)
                Assert.Equal(7, slot.GetValue(gen3Instance) :?> int)
                slot.SetValue(gen3Instance, 11)
                Assert.Equal(11, slot.GetValue(gen3Instance) :?> int)
                printfn "[mixedadd] SUCCESS: method/field/property additions chained across three generations")

    // ------------------------------------------------------------------------------
    // Fresh-vs-baseline Property/Event row coordinates (sibling of the MethodDef
    // coordinate fix): a property ADDED by an earlier generation chains PAST the
    // baseline Property table, while every later fresh compile lays its row out at
    // the natural source position - BEFORE the rows of types declared later in the
    // file. The generation-2 accessor edit re-registers the chained Property row, and
    // its snapshot must read the FRESH compile's row, not index the fresh reader with
    // the chained baseline row id (which lands on the displaced neighbour). The
    // corrupt read was LATENT - both delta writers emit Property/Event rows only for
    // added members, so an accessor body edit produces a MethodDef-only update
    // (Roslyn parity) - this test pins both the coordinate rule and the row-drop.
    [<Fact>]
    let ``Added property survives an accessor edit when a later type displaces its fresh row`` () =
        let baselineSource =
            """
module Sample.DisplacedProps

type Widget() =
    let mutable state = 1
    member _.State() = state

type Anchor() =
    member _.Tag = "anchor"
"""

        // Generation 1 adds a getter-only property to Widget (declared FIRST): the
        // fresh Property table reads [Widget.Doubled; Anchor.Tag] while the chained
        // delta row for Doubled is row 2 (past the baseline's single Anchor.Tag row).
        let gen1Source =
            """
module Sample.DisplacedProps

type Widget() =
    let mutable state = 1
    member _.State() = state
    member _.Doubled = state * 2

type Anchor() =
    member _.Tag = "anchor"
"""

        // Generation 2 edits the added property's getter body only.
        let gen2Source =
            """
module Sample.DisplacedProps

type Widget() =
    let mutable state = 1
    member _.State() = state
    member _.Doubled = state * 3

type Anchor() =
    member _.Tag = "anchor"
"""

        runMemberAdditionScenario
            "fsharp-hotreload-propdisplaced"
            "PropertyDisplacedLibrary"
            [ "Baseline"; "AddMethodToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let widgetType = assembly.GetType("Sample.DisplacedProps+Widget", throwOnError = true)
                let anchorType = assembly.GetType("Sample.DisplacedProps+Anchor", throwOnError = true)
                let instance = Activator.CreateInstance(widgetType)

                // ---------------- Generation 1: property addition ----------------
                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata
                assertAddPair encLog1 0x15 4 0x17 // (PropertyMap, AddProperty) + Property

                let doubled = widgetType.GetProperty("Doubled", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull doubled), "Added property 'Doubled' not found after generation 1.")
                Assert.Equal(2, doubled.GetValue(instance) :?> int)

                // ---------------- Generation 2: accessor body edit ----------------
                // The edit updates the chained getter's MethodDef row only: the
                // re-registered Property row is non-added, and non-added Property rows
                // are dropped by the writers (an accessor body edit does not change the
                // Property row - Roslyn parity). In particular the displaced-row read
                // must not surface: no Property/PropertyMap entries, no re-add.
                let delta2 = applyGeneration 2 gen2Source
                let encLog2 = readDeltaEncLog delta2.Metadata

                Assert.Contains(encLog2, fun (token, op) -> token >>> 24 = 0x06 && op = 0)
                Assert.DoesNotContain(encLog2, fun (token, _) -> token >>> 24 = 0x17)
                Assert.DoesNotContain(encLog2, fun (token, _) -> token >>> 24 = 0x15)

                Assert.Equal(3, doubled.GetValue(instance) :?> int)

                // The property stays resolvable by name with its int signature...
                let refreshed = widgetType.GetProperty("Doubled", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull refreshed), "Property 'Doubled' lost after the generation-2 accessor edit.")
                Assert.Equal(typeof<int>, refreshed.PropertyType)
                Assert.Equal(3, refreshed.GetValue(instance) :?> int)

                // ...and the displaced neighbour row (Anchor.Tag) is untouched.
                let anchorInstance = Activator.CreateInstance(anchorType)
                let tag = anchorType.GetProperty("Tag", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull tag), "Baseline property 'Tag' lost after the generation-2 edit.")
                Assert.Equal("anchor", tag.GetValue(anchorInstance) :?> string)
                printfn "[propdisplaced] SUCCESS: gen-2 accessor edit kept the chained Property row intact")

    [<Fact>]
    let ``Added CLIEvent chains past a baseline event and a later body edit fails closed`` () =
        // Event-table twin of the displaced-property pin. Generation 1 adds a
        // [<CLIEvent>] whose chained Event row is row 2 while the fresh compile lays it
        // out at row 1 (Front is declared before the baseline-evented Rear) - the added
        // row must be emitted from FRESH coordinates with a NEW EventMap row for Front.
        // Generation 2 then edits the event member's body: that fails closed at symbol
        // mapping (the typed-tree get_<Name> PropertyGet symbol has no IL counterpart),
        // so the event accessor re-registration path is unreachable for body edits and
        // the displaced-row coordinates can never surface for events today.
        let baselineSource =
            """
module Sample.DisplacedEvents

open System

type Front() =
    let primary = Event<EventHandler, EventArgs>()
    let secondary = Event<EventHandler, EventArgs>()
    member _.TriggerPrimary() = primary.Trigger(null, EventArgs.Empty)
    member _.TriggerSecondary() = secondary.Trigger(null, EventArgs.Empty)

type Rear() =
    let anchored = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member _.Anchored = anchored.Publish
"""

        let gen1Source =
            """
module Sample.DisplacedEvents

open System

type Front() =
    let primary = Event<EventHandler, EventArgs>()
    let secondary = Event<EventHandler, EventArgs>()
    member _.TriggerPrimary() = primary.Trigger(null, EventArgs.Empty)
    member _.TriggerSecondary() = secondary.Trigger(null, EventArgs.Empty)
    [<CLIEvent>]
    member _.Added = primary.Publish

type Rear() =
    let anchored = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member _.Anchored = anchored.Publish
"""

        // Generation 2 rebinds the added event member's body to the other baseline
        // field - a CLIEvent body edit, which the symbol mapper rejects today.
        let gen2Source =
            """
module Sample.DisplacedEvents

open System

type Front() =
    let primary = Event<EventHandler, EventArgs>()
    let secondary = Event<EventHandler, EventArgs>()
    member _.TriggerPrimary() = primary.Trigger(null, EventArgs.Empty)
    member _.TriggerSecondary() = secondary.Trigger(null, EventArgs.Empty)
    [<CLIEvent>]
    member _.Added = secondary.Publish

type Rear() =
    let anchored = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member _.Anchored = anchored.Publish
"""

        runMemberAdditionScenario
            "fsharp-hotreload-eventdisplaced"
            "EventDisplacedLibrary"
            [ "Baseline"; "AddMethodToExistingType" ]
            baselineSource
            (fun assembly applyGeneration ->
                let frontType = assembly.GetType("Sample.DisplacedEvents+Front", throwOnError = true)
                let instance = Activator.CreateInstance(frontType)
                let trigger name =
                    frontType.GetMethod(name, BindingFlags.Public ||| BindingFlags.Instance).Invoke(instance, [||])
                    |> ignore

                // ---------------- Generation 1: event addition ----------------
                let delta1 = applyGeneration 1 gen1Source
                let encLog1 = readDeltaEncLog delta1.Metadata
                assertAddPair encLog1 0x12 5 0x14 // (EventMap, AddEvent) + Event

                let added = frontType.GetEvent("Added", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull added), "Added event 'Added' not found after generation 1.")

                let mutable fired = 0
                let handler = EventHandler(fun _ _ -> fired <- fired + 1)
                added.AddEventHandler(instance, handler)
                trigger "TriggerPrimary"
                Assert.Equal(1, fired)
                added.RemoveEventHandler(instance, handler)

                // ---------------- Generation 2: event member body edit ----------------
                // Fails closed (UnsupportedEdit) at accessor symbol mapping; the session
                // and the generation-1 event must stay intact afterwards.
                let mutable gen2Error = None

                try
                    applyGeneration 2 gen2Source |> ignore
                with ex ->
                    gen2Error <- Some ex.Message

                match gen2Error with
                | None -> failwith "Expected the CLIEvent body edit to fail closed (UnsupportedEdit)."
                | Some message ->
                    Assert.Contains("full rebuild required", message)
                    Assert.Contains("get_Added", message)

                let survivor = frontType.GetEvent("Added", BindingFlags.Public ||| BindingFlags.Instance)
                Assert.True(not (isNull survivor), "Event 'Added' lost after the rejected generation-2 edit.")
                Assert.Equal(typeof<EventHandler>, survivor.EventHandlerType)

                let mutable stillFiring = 0
                let stillFiringHandler = EventHandler(fun _ _ -> stillFiring <- stillFiring + 1)
                survivor.AddEventHandler(instance, stillFiringHandler)
                trigger "TriggerPrimary"
                Assert.Equal(1, stillFiring)
                printfn "[eventdisplaced] SUCCESS: displaced CLIEvent addition applied; body edit failed closed")

    // ------------------------------------------------------------------------------
    // The dotnet-watch force-rebuild topology: the baseline contains NO lambdas, and
    // every generation's on-disk output is produced by a SEPARATE flag-on fsc build
    // (no session state in that process), so the closure added in generation 1 carries
    // the gen-0 occurrence-derived name {member}@hotreload#g0_o{chain} - NOT an
    // allocator #g1 name. The module-level value places startup-init methods AFTER the
    // closure's rows in the fresh compile's MethodDef table, so the closure rows added
    // by the generation-1 delta (which chain PAST the baseline tables) do NOT coincide
    // with the closure's rows in later fresh compiles.
    let private forceRebuildBaselineSource =
        """
module Program

type Greeter =
    static member Message() =
        "hello message"

let greetingPrefix = System.String.Concat("fs", "harp")

let probe () = greetingPrefix + "|" + Greeter.Message()
"""

    // Generation 1: a lambda is ADDED to the lambda-free Message (its closure class is
    // new metadata; the separate flag-on rebuild names it Message@hotreload#g0_o0).
    let private forceRebuildGen1Source =
        """
module Program

type Greeter =
    static member Message() =
        let parts = [ "lambda"; "message" ] |> List.map (fun s -> s.ToUpper())
        String.concat " " parts

let greetingPrefix = System.String.Concat("fs", "harp")

let probe () = greetingPrefix + "|" + Greeter.Message()
"""

    // Generation 2: body edit of the generation-1-added lambda (ToUpper -> ToLower).
    // The closure class chained into the session baseline, so this is an in-place
    // update of its methods - including the re-emitted .cctor/.ctor rows, whose bodies
    // must be read from the FRESH compile's row coordinates, not the chained baseline
    // row ids (the fresh compile lays the closure out at its natural position, before
    // the startup-init methods).
    let private forceRebuildGen2Source =
        """
module Program

type Greeter =
    static member Message() =
        let parts = [ "lambda"; "message" ] |> List.map (fun s -> s.ToLower())
        String.concat " " parts

let greetingPrefix = System.String.Concat("fs", "harp")

let probe () = greetingPrefix + "|" + Greeter.Message()
"""

    [<Fact>]
    let ``Added closure survives a second edit when the on-disk output is force-rebuilt between generations`` () =
        // Repro of the dotnet-watch gen-2 crash: baseline without lambdas is built and
        // session-started from disk; between edits the host force-rebuilds the on-disk
        // dll with a separate flag-on fsc (simulated here by pre-building each
        // generation's output through the command-line fsc path with ALL in-process
        // session state reset, then restoring those exact bytes at edit time).
        // Generation 1 adds a List.map lambda (new closure TypeDef, named #g0_o0 by the
        // session-less rebuild's baseline derivation); generation 2 edits the lambda
        // body. Before the fresh-vs-baseline row coordinate fix, generation 2 re-emitted
        // the added closure's .cctor/.ctor/Invoke rows with bodies read from the WRONG
        // fresh rows (the chained baseline row ids), and invoking the method after
        // ApplyUpdate crashed with InvalidProgramException in the closure's .cctor.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-force-rebuild", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "ForceRebuild.fs")
            let dllPath = Path.Combine(projectDir, "ForceRebuild.dll")
            let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
            let runtimeDllPath = Path.Combine(projectDir, "ForceRebuild.runtime.dll")

            try
                File.WriteAllText(fsPath, forceRebuildBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString forceRebuildBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                // Pre-build each generation's on-disk output exactly the way the watch
                // host's force rebuild does: a flag-on fsc compile with NO session state
                // (process boundary simulated by resetting all in-process state after
                // each build). --deterministic makes these byte-identical to what a
                // separate fsc process would produce at edit time.
                let buildOnDisk (source: string) =
                    File.WriteAllText(fsPath, source)
                    checker.InvalidateAll()

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "On-disk build failed: %A" (errors |> Array.map (fun d -> d.Message))

                    FSharpEditAndContinueLanguageService.Instance.ResetSessionState()
                    File.ReadAllBytes(dllPath), File.ReadAllBytes(pdbPath)

                let gen1DllBytes, gen1PdbBytes = buildOnDisk forceRebuildGen1Source
                let gen2DllBytes, gen2PdbBytes = buildOnDisk forceRebuildGen2Source

                // The session-less rebuild names the added closure with the BASELINE
                // (generation-0) occurrence derivation - the watch topology's defining
                // property (an allocator-driven delta compile would say #g1).
                let closureTypeNames (dllBytes: byte[]) =
                    use peReader = new PEReader(ImmutableArray.CreateRange<byte> dllBytes)
                    let reader = peReader.GetMetadataReader()
                    reader.TypeDefinitions
                    |> Seq.map (fun handle -> reader.GetString((reader.GetTypeDefinition handle).Name))
                    |> Seq.filter (fun name -> name.Contains("@hotreload"))
                    |> Seq.toList

                Assert.Contains("Message@hotreload#g0_o0", closureTypeNames gen1DllBytes)
                Assert.Contains("Message@hotreload#g0_o0", closureTypeNames gen2DllBytes)

                // Restore the lambda-free baseline on disk and start the session from it
                // (the dotnet-watch topology: fsc built it in another process).
                File.WriteAllText(fsPath, forceRebuildBaselineSource)
                checker.InvalidateAll()

                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                let capabilities =
                    [ "Baseline"; "AddMethodToExistingType"; "NewTypeDefinition" ]

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session from disk: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let moduleType = assembly.GetType("Program", throwOnError = true)
                let probe = moduleType.GetMethod("probe", BindingFlags.Public ||| BindingFlags.Static)

                Assert.Equal("fsharp|hello message", probe.Invoke(null, [||]) :?> string)

                let applyGeneration gen (source: string) (rebuiltDll: byte[]) (rebuiltPdb: byte[]) (expected: string) =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    // The watch host's force rebuild: a separate fsc process rewrites the
                    // project output between the edit and the delta emission.
                    File.WriteAllBytes(dllPath, rebuiltDll)
                    File.WriteAllBytes(pdbPath, rebuiltPdb)

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                        // The crash mode under repro is InvalidProgramException thrown
                        // from the added closure's .cctor when the patched method first
                        // runs; surface it with the generation context.
                        let value =
                            try
                                probe.Invoke(null, [||]) :?> string
                            with :? TargetInvocationException as ex ->
                                failwithf "Gen %d invoke crashed after ApplyUpdate: %O" gen ex.InnerException

                        Assert.Equal(expected, value)
                        delta

                // Generation 1: the added lambda emits a NEW TypeDef row and applies.
                let baselineTypeDefCount =
                    use peReader = new PEReader(File.OpenRead runtimeDllPath)
                    peReader.GetMetadataReader().GetTableRowCount(TableIndex.TypeDef)

                let delta1 =
                    applyGeneration 1 forceRebuildGen1Source gen1DllBytes gen1PdbBytes "fsharp|LAMBDA MESSAGE"

                let readEncLog (metadata: byte[]) =
                    use provider =
                        MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte> metadata)

                    provider.GetMetadataReader().GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry -> MetadataTokens.GetToken entry.Handle, int entry.Operation)
                    |> Seq.toArray

                let gen1TypeDefAdds =
                    readEncLog delta1.Metadata
                    |> Array.filter (fun (token, op) ->
                        token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefCount && op = 0)

                Assert.True(
                    gen1TypeDefAdds.Length >= 1,
                    "Expected the generation-1 delta to add the closure's TypeDef row.")

                // Generation 2: body edit of the generation-1-added lambda. The closure's
                // re-emitted rows must carry bodies read from the FRESH compile's rows;
                // pre-fix this invoke threw InvalidProgramException in the closure's
                // .cctor. No new TypeDef rows may appear.
                let delta2 =
                    applyGeneration 2 forceRebuildGen2Source gen2DllBytes gen2PdbBytes "fsharp|lambda message"

                let gen2TypeDefAdds =
                    readEncLog delta2.Metadata
                    |> Array.filter (fun (token, _) ->
                        token >>> 24 = 0x02 && (token &&& 0x00FFFFFF) > baselineTypeDefCount + 1)

                Assert.True(
                    Array.isEmpty gen2TypeDefAdds,
                    "Generation 2 must update the added closure in place.")

                printfn "[force-rebuild] SUCCESS: gen-2 edit of a closure added over a force-rebuilt baseline applied and ran"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // Phase D: state machine (task/async/seq CE) edit support.
    // Ground truth (see docs/hot-reload-closure-mapping.md, "State machines"):
    //   - `async` lowers to closure chains (FSharpFunc subclasses) - Phase C territory.
    //   - `task` lowers to ONE nested struct state machine (Data/ResumptionPoint/
    //     hoisted locals/awaiterN fields + MoveNext/SetStateMachine/accessors) whose
    //     resume-point state numbers are assigned positionally by the lowering.
    // -----------------------------------------------------------------------------

    /// Runs baseline -> session -> edit -> EmitHotReloadDelta and asserts the delta is
    /// REJECTED, handing the UnsupportedEdit message to the caller for assertions.
    let private assertEmitRejectedWithMessage
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (assertMessage: string -> unit)
        =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                keepAllBackgroundResolutions = false,
                keepAllBackgroundSymbolUses = false,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                enablePartialTypeChecking = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-emit-reject", System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        try
            File.WriteAllText(fsPath, baselineSource)

            let projectOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fsPath,
                    SourceText.ofString baselineSource,
                    assumeDotNetFramework = false,
                    useSdkRefs = true,
                    useFsiAuxLib = false
                )
                |> Async.RunImmediate

            let projectOptions =
                { projectOptions with
                    SourceFiles = [| fsPath |]
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.append
                            [| "--target:library"
                               "--langversion:preview"
                               "--optimize-"
                               "--debug:portable"
                               "--deterministic"
                               "--enable:hotreloaddeltas"
                               $"--out:{dllPath}" |] }

            checker.InvalidateAll()

            let compileDiagnostics, _ =
                checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                |> Async.RunImmediate

            let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then
                failwithf "[%s] baseline compilation failed: %A" testLabel (errors |> Array.map (fun d -> d.Message))

            // Grant the full member-addition capability set: the expected rejection must
            // come from edit classification, never from a missing runtime capability.
            let capabilities =
                [ "Baseline"
                  "AddMethodToExistingType"
                  "AddStaticFieldToExistingType"
                  "AddInstanceFieldToExistingType"
                  "NewTypeDefinition" ]

            match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
            | Error error -> failwithf "[%s] failed to start hot reload session: %A" testLabel error
            | Ok () -> ()

            File.WriteAllText(fsPath, updatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

            let updatedOptions =
                { projectOptions with
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.filter (fun opt ->
                            not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

            let compileDiagnostics2, _ =
                checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                |> Async.RunImmediate

            let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors2.Length > 0 then
                failwithf "[%s] update compilation failed: %A" testLabel (errors2 |> Array.map (fun d -> d.Message))

            match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
            | Ok _ -> failwithf "[%s] expected the edit to be rejected, but a delta was emitted." testLabel
            | Error (FSharpHotReloadError.UnsupportedEdit message) -> assertMessage message
            | Error other -> failwithf "[%s] expected UnsupportedEdit, got %A" testLabel other

        finally
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    let private taskStableResumeBaselineSource =
        """
namespace Sample

open System.Threading.Tasks

type Type =
    static member GetMessage() =
        (task {
            let! prefix = Task.FromResult "Hello"
            let! name = Task.FromResult "watch"
            return prefix + ", " + name
        }).Result
"""

    [<Fact>]
    let ``ApplyUpdate succeeds for task body edit with stable resume points`` () =
        // The resumable step sequence (Delay/Bind/Bind/Return) is unchanged, so the edit
        // is a MoveNext body update on the existing state machine struct: the struct's
        // awaiter/hoisted field layout, the TypeDef, and the state numbers all survive.
        applySingleStringUpdateAndAssertRuntimeResult
            "task-body-edit"
            taskStableResumeBaselineSource
            (taskStableResumeBaselineSource.Replace("Hello", "Welcome"))
            "Hello, watch"
            "Welcome, watch"

    [<Fact>]
    let ``EmitHotReloadDelta rejects task await addition as a state machine shape change`` () =
        // A new `let!` is a new resume point AND a new awaiter struct field; struct
        // layouts are immutable under hot reload (C# parity: ChangingStateMachineShape).
        // Before Phase D classification, this delta was emitted and the patched method
        // crashed at runtime (the silent compiler-generated-field skip in the emitter).
        let updated =
            taskStableResumeBaselineSource.Replace(
                "return prefix + \", \" + name",
                "let! extra = Task.FromResult \"!\"\n            return prefix + \", \" + name + extra")

        assertEmitRejectedWithMessage
            "task-added-await"
            taskStableResumeBaselineSource
            updated
            (fun message ->
                // FSHRDL013 = RudeEditKind.StateMachineShapeChange
                Assert.Contains("FSHRDL013", message)
                Assert.Contains("state-machine", message))

    [<Fact>]
    let ``EmitHotReloadDelta rejects task await reorder as a state machine shape change`` () =
        // Same resume-point count, but state numbers are positional: an in-flight frame
        // suspended at state 1 would resume into the OTHER await's continuation.
        let updated =
            taskStableResumeBaselineSource.Replace(
                "let! prefix = Task.FromResult \"Hello\"\n            let! name = Task.FromResult \"watch\"",
                "let! name = Task.FromResult \"watch\"\n            let! prefix = Task.FromResult \"Hello\"")

        assertEmitRejectedWithMessage
            "task-reordered-awaits"
            taskStableResumeBaselineSource
            updated
            (fun message -> Assert.Contains("FSHRDL013", message))

    [<Fact>]
    let ``ApplyUpdate succeeds for plain method gaining a while loop`` () =
        // while/for/try lower to ordinary IL inside the method body; before Phase D the
        // blanket lowered-shape digest misclassified their introduction as a
        // state-machine shape change (FSHRDL013).
        let baseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        "Hello, watch"
"""

        let updated =
            """
namespace Sample

type Type =
    static member GetMessage() =
        let mutable acc = ""
        let mutable i = 0
        while i < 2 do
            acc <- acc + (if i = 0 then "Welcome" else ", watch")
            i <- i + 1
        acc
"""

        applySingleStringUpdateAndAssertRuntimeResult "plain-gains-while" baseline updated "Hello, watch" "Welcome, watch"

    [<Fact>]
    let ``ApplyUpdate succeeds for async body edit gaining inner try-with`` () =
        // async lowers to closure chains: an expression-level try/with inside the CE is
        // a closure body edit, not state machine evidence.
        let baseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        async {
            let! prefix = async { return "Hello" }
            return prefix + ", watch"
        }
        |> Async.RunSynchronously
"""

        let updated =
            """
namespace Sample

type Type =
    static member GetMessage() =
        async {
            let! prefix = async { return "Welcome" }
            let safe = (try prefix with _ -> "x")
            return safe + ", watch"
        }
        |> Async.RunSynchronously
"""

        applySingleStringUpdateAndAssertRuntimeResult "async-gains-trywith" baseline updated "Hello, watch" "Welcome, watch"

    [<Fact>]
    let ``EmitHotReloadDelta rejects async added bind with a precise closure-chain message`` () =
        // async lowers to closure CHAINS whose classes outnumber the C1 lambda
        // occurrences (inlined AsyncPrimitives internals) and carry legacy `-N` names:
        // a structural CE change shifts the numbering, so the synthesized-type mapping
        // cannot align the chain and must fail closed with a precise message (never
        // silently pair shifted classes - the delta would patch the wrong rows).
        let baseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        async {
            let! prefix = async { return "Hello" }
            return prefix + ", watch"
        }
        |> Async.RunSynchronously
"""

        let updated =
            baseline.Replace(
                "return prefix + \", watch\"",
                "let! name = async { return \"watch\" }\n            return prefix + \", \" + name")

        assertEmitRejectedWithMessage
            "async-added-bind"
            baseline
            updated
            (fun message ->
                Assert.Contains("synthesized type", message)
                Assert.Contains("ebuild", message))

    [<Fact>]
    let ``Disk-started session applies a task body edit with stable resume points`` () =
        // The dotnet-watch topology for state machines (Phase D): the baseline is built
        // by the command-line fsc path (persisting the EnC State Machine State Map CDI
        // rows in the portable PDB), ALL in-process session state is dropped, and the
        // session starts from the on-disk dll + pdb alone. A resume-point-stable body
        // edit must classify as a MethodBody update, emit, apply, and run.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-task-disk-start", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "TaskDiskStart.fs")
            let dllPath = Path.Combine(projectDir, "TaskDiskStart.dll")
            let runtimeDllPath = Path.Combine(projectDir, "TaskDiskStart.runtime.dll")

            try
                File.WriteAllText(fsPath, taskStableResumeBaselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString taskStableResumeBaselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()

                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                Assert.True(File.Exists pdbPath, "Baseline PDB must exist on disk.")

                // The persisted baseline must carry the state map for GetMessage (both
                // resume points, state numbers 1 and 2).
                let baselineStateInfos =
                    FSharp.Compiler.EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb (File.ReadAllBytes pdbPath)

                let stateBearingMethods =
                    baselineStateInfos
                    |> Map.filter (fun _ info -> not info.StateMachineStates.IsEmpty)

                Assert.True(
                    Map.count stateBearingMethods = 1,
                    $"expected exactly one state-map-bearing method in the baseline PDB, found %d{Map.count stateBearingMethods}")

                let states = (Seq.head stateBearingMethods).Value.StateMachineStates
                Assert.Equal<int list>([ 1; 2 ], states |> List.map (fun s -> s.StateNumber))

                // Simulate the process boundary: the dll + pdb on disk are the only
                // baseline inputs from here on.
                FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session from disk: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                let method = methodType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

                Assert.Equal("Hello, watch", method.Invoke(null, [||]) :?> string)

                let updatedSource = taskStableResumeBaselineSource.Replace("Hello", "Welcome")
                File.WriteAllText(fsPath, updatedSource)
                checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let compileDiagnostics2, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors2 = compileDiagnostics2 |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors2.Length > 0 then failwithf "Update compilation failed: %A" (errors2 |> Array.map (fun d -> d.Message))

                match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                    MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

                    Assert.Equal("Welcome, watch", method.Invoke(null, [||]) :?> string)

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``ApplyUpdate succeeds for seq body edit with added yield`` () =
        // seq { } lowers to a CLASS state machine (GenSequenceExpression closure, not
        // ResumableCode): MoveNext is regenerated whole and class layouts may grow, so
        // a fresh enumeration after the update observes the new yields. NOTE the
        // documented caveat (docs/hot-reload-closure-mapping.md, Phase D): enumerators
        // already SUSPENDED mid-sequence at apply time resume on the new MoveNext with
        // their old state number, exactly like C# Debug-mode iterator edits would - F#
        // currently allows the edit where C# reports ChangingStateMachineShape.
        let baseline =
            """
namespace Sample

type Type =
    static member GetMessage() =
        seq {
            yield "Hello"
            yield ", watch"
        }
        |> String.concat ""
"""

        let updated =
            """
namespace Sample

type Type =
    static member GetMessage() =
        seq {
            yield "Welcome"
            yield ", "
            yield "watch"
        }
        |> String.concat ""
"""

        // The added yield is an added closure occurrence at this optimization level
        // (the seq desugaring's closure chain), so the C4 capability set is required.
        applySingleStringUpdateWithCapabilitiesAndAssertRuntimeResult
            (Some [ "Baseline"; "AddMethodToExistingType"; "NewTypeDefinition" ])
            "seq-added-yield"
            baseline
            updated
            "Hello, watch"
            "Welcome, watch"

    [<Fact>]
    let ``ApplyUpdate succeeds for async body edits across generations`` () =
        // Three generations of resume-structure-stable async body edits: the closure
        // chain keeps its deterministic names, so every generation is a set of method
        // updates over the same TypeDefs.
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this test"
        else
            let sourceFor (prefix: string) (suffix: string) =
                $"""
namespace Sample

type Type =
    static member GetMessage() =
        async {{
            let! prefix = async {{ return "{prefix}" }}
            return prefix + ", " + "{suffix}"
        }}
        |> Async.RunSynchronously
"""

            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-async-multigen", System.Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "AsyncMultiGen.fs")
            let dllPath = Path.Combine(projectDir, "AsyncMultiGen.dll")
            let runtimeDllPath = Path.Combine(projectDir, "AsyncMultiGen.runtime.dll")

            try
                let baselineSource = sourceFor "Hello" "watch"
                File.WriteAllText(fsPath, baselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()

                let compileDiagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; projectOptions.OtherOptions; projectOptions.SourceFiles ])
                    |> Async.RunImmediate

                let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                if errors.Length > 0 then failwithf "Baseline compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

                match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start hot reload session: %A" error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = Assembly.LoadFrom(runtimeDllPath)
                let methodType = assembly.GetType("Sample.Type", throwOnError = true)
                let method = methodType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

                Assert.Equal("Hello, watch", method.Invoke(null, [||]) :?> string)

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                let applyGeneration gen (source: string) (expected: string) =
                    File.WriteAllText(fsPath, source)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

                    let compileDiagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; updatedOptions.OtherOptions; updatedOptions.SourceFiles ])
                        |> Async.RunImmediate

                    let errors = compileDiagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
                    if errors.Length > 0 then failwithf "Gen %d compilation failed: %A" gen (errors |> Array.map (fun d -> d.Message))

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "Gen %d EmitHotReloadDelta failed: %A" gen error
                    | Ok delta ->
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        Assert.Equal(expected, method.Invoke(null, [||]) :?> string)

                applyGeneration 1 (sourceFor "Welcome" "watch") "Welcome, watch"
                applyGeneration 2 (sourceFor "Welcome" "world") "Welcome, world"
                applyGeneration 3 (sourceFor "Hi" "world") "Hi, world"

            finally
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()
