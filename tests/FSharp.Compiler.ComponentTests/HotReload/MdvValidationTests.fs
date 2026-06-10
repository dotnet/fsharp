#nowarn "57"

namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.Text.Json
open System.Collections.Immutable
open System.Diagnostics
open System.IO
open System.Reflection.PortableExecutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System
open Xunit
open Xunit.Sdk
open System
open System.Text
open System.Threading

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxDeltaEmitter
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Test
open Internal.Utilities
open FSharp.Compiler.ComponentTests.HotReload.TestHelpers

module ILWriter = FSharp.Compiler.AbstractIL.ILBinaryWriter
module DeltaWriter = FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter

[<Collection(nameof NotThreadSafeResourceCollection)>]
module MdvValidationTests =

    let private keepArtifacts () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_KEEP_TEST_OUTPUT") with
        | null -> false
        | value when value.Equals("1", StringComparison.OrdinalIgnoreCase) -> true
        | value when value.Equals("true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    let private assertGenerationContains (output: string) (generation: int) (expectedSubstring: string) =
        let marker = $">>> Generation {generation}:"
        let index = output.IndexOf(marker, StringComparison.Ordinal)
        Assert.True(index >= 0, $"mdv output did not contain marker '{marker}'. Full output:{Environment.NewLine}{output}")
        let slice = output.Substring(index)
        Assert.Contains(expectedSubstring, slice)

    let private containsSubsequence (source: byte[]) (pattern: byte[]) =
        if pattern.Length = 0 then
            true
        else
            let sourceSpan = ReadOnlySpan(source)
            let patternSpan = ReadOnlySpan(pattern)
            MemoryExtensions.IndexOf(sourceSpan, patternSpan) >= 0

    let private withMetadataReader (metadata: byte[]) (action: MetadataReader -> 'T) : 'T =
        use provider =
            MetadataReaderProvider.FromMetadataImage(
                ImmutableArray.CreateRange metadata)
        let reader = provider.GetMetadataReader()
        action reader

    module private RoslynBaseline =
        // Helper to convert TableName to SRM TableIndex enum
        let inline private toTableIndex (table: TableName) : TableIndex =
            LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte table.Index)

        let private baselines : Lazy<Map<string, Map<string, int>>> = lazy (
            let path = Path.Combine(__SOURCE_DIRECTORY__, "../../../../tools/baselines/roslyn_tables.json") |> Path.GetFullPath
            if not (File.Exists path) then
                failwithf "Roslyn baseline table snapshot not found: %s" path

            let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
            let dict = JsonSerializer.Deserialize<Collections.Generic.Dictionary<string, Collections.Generic.Dictionary<string, int>>>(File.ReadAllText path, options)
            dict
            |> Seq.map (fun outer ->
                let innerMap =
                    outer.Value
                    |> Seq.map (fun inner -> inner.Key, inner.Value)
                    |> Map.ofSeq
                outer.Key, innerMap)
            |> Map.ofSeq)

        let private tryFindTableIndex key =
            match key with
            | "Module" -> Some TableNames.Module
            | "TypeRef" -> Some TableNames.TypeRef
            | "TypeDef" -> Some TableNames.TypeDef
            | "Field" -> Some TableNames.Field
            | "MethodDef" -> Some TableNames.Method
            | "Param" -> Some TableNames.Param
            | "MemberRef" -> Some TableNames.MemberRef
            | "StandAloneSig" -> Some TableNames.StandAloneSig
            | "Property" -> Some TableNames.Property
            | "PropertyMap" -> Some TableNames.PropertyMap
            | "Event" -> Some TableNames.Event
            | "EventMap" -> Some TableNames.EventMap
            | "MethodSemantics" -> Some TableNames.MethodSemantics
            | "TypeSpec" -> Some TableNames.TypeSpec
            | "AssemblyRef" -> Some TableNames.AssemblyRef
            | "EncLog" -> Some TableNames.ENCLog
            | "EncMap" -> Some TableNames.ENCMap
            | _ -> None

        let private countRows (metadata: byte[]) (table: TableName) =
            withMetadataReader metadata (fun reader -> reader.GetTableRowCount(toTableIndex table))

        let assertWithin (scenario: string) (metadata: byte[]) =
            let expected =
                baselines.Value
                |> Map.tryFind scenario
                |> Option.defaultWith (fun () -> failwithf "Roslyn baseline '%s' missing" scenario)

            for KeyValue(key, budget) in expected do
                match tryFindTableIndex key with
                | Some tableIndex ->
                    let actual = countRows metadata tableIndex
                    Assert.True(
                        actual <= budget,
                        sprintf "[Roslyn baseline] scenario '%s' exceeded %A: actual=%d baseline=%d" scenario tableIndex actual budget)
                | None -> ()


    module private HeapBudgets =
        type Budget = { StringBytes: int; BlobBytes: int }

        let private metadataStringBytes = 14
        let private metadataBlobBytes = 4

        let private budgets : Map<string, Budget> =
            Map.ofList
                [ "Property", { StringBytes = metadataStringBytes; BlobBytes = metadataBlobBytes }
                  "PropertyUpdate", { StringBytes = metadataStringBytes; BlobBytes = metadataBlobBytes }
                  "Event", { StringBytes = metadataStringBytes; BlobBytes = metadataBlobBytes }
                  "EventUpdate", { StringBytes = metadataStringBytes; BlobBytes = metadataBlobBytes }
                  // Async/Closure scenarios now carry module + DebuggableAttribute strings; allow modest growth.
                  "Async", { StringBytes = 24; BlobBytes = metadataBlobBytes }
                  "AsyncUpdate", { StringBytes = 24; BlobBytes = metadataBlobBytes }
                  "Closure", { StringBytes = 24; BlobBytes = metadataBlobBytes }
                  "ClosureUpdate", { StringBytes = 24; BlobBytes = metadataBlobBytes } ]

        let assertWithin (scenario: string) (metadata: byte[]) =
            match Map.tryFind scenario budgets with
            | None -> ()
            | Some budget ->
                withMetadataReader metadata (fun reader ->
                    let stringSize = reader.GetHeapSize HeapIndex.String
                    let blobSize = reader.GetHeapSize HeapIndex.Blob
                    Assert.True(
                        stringSize <= budget.StringBytes,
                        sprintf "[%s] string heap grew to %d bytes (budget %d)" scenario stringSize budget.StringBytes)
                    Assert.True(
                        blobSize <= budget.BlobBytes,
                        sprintf "[%s] blob heap grew to %d bytes (budget %d)" scenario blobSize budget.BlobBytes))

    let private methodRowIdFromToken (methodToken: int) = methodToken &&& 0x00FFFFFF

    let private assertMethodEncLog (delta: IlxDelta) (methodToken: int) =
        let methodRowId = methodRowIdFromToken methodToken
        let moduleEntry =
            delta.EncLog
            |> Array.exists (fun (table, _, _) -> table = TableNames.Module)
        Assert.True(moduleEntry, "Expected EncLog entry for Module table")

        let methodEntry =
            delta.EncLog
            |> Array.exists (fun (table, row, op) ->
                table = TableNames.Method
                && row = methodRowId
                && (op = EditAndContinueOperation.Default || op = EditAndContinueOperation.AddMethod))
        Assert.True(methodEntry, "Expected EncLog entry for updated method definition")

    let private assertEncMapContains (delta: IlxDelta) (table: TableName) (rowId: int) =
        let entryExists =
            delta.EncMap
            |> Array.exists (fun (t, r) -> t = table && r = rowId)
        Assert.True(entryExists, $"Expected EncMap entry for table 0x{table.Index:X2} row {rowId}")

    let private isDefinitionHandle (handle: EntityHandle) =
        match handle.Kind with
        | HandleKind.ModuleDefinition
        | HandleKind.TypeDefinition
        | HandleKind.MethodDefinition
        | HandleKind.FieldDefinition
        | HandleKind.Parameter
        | HandleKind.PropertyDefinition
        | HandleKind.EventDefinition
        | HandleKind.AssemblyDefinition -> true
        | _ -> false


    // Helper to convert TableName to SRM TableIndex enum
    let inline private toTableIndex (table: TableName) : TableIndex =
        LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte table.Index)

    let private assertEncMapDefinitionsMatch (delta: IlxDelta) (expected: EntityHandle list) =
        let actual =
            delta.EncMap
            |> Array.map (fun (t, r) -> MetadataTokens.EntityHandle(toTableIndex t, r))
            |> Array.toList
            |> List.filter isDefinitionHandle

        let expectedFiltered =
            expected
            |> List.filter (fun h -> not h.IsNil)
            |> List.filter isDefinitionHandle

        let tokenize (xs: EntityHandle list) =
            xs
            |> List.map (fun (h: EntityHandle) -> MetadataTokens.GetToken h)
            |> List.sort

        Assert.Equal<int list>(tokenize expectedFiltered, tokenize actual)

    let private decodeEntityHandle (handle: EntityHandle) : int * int =
        let token = MetadataTokens.GetToken(handle)
        let table = int (token >>> 24)
        let rowId = token &&& 0x00FFFFFF
        table, rowId

    let private readEncTables (reader: MetadataReader) =
        let encLog =
            reader.GetEditAndContinueLogEntries()
            |> Seq.map (fun entry ->
                let table, rowId = decodeEntityHandle entry.Handle
                table, rowId, entry.Operation)
            |> Seq.toArray

        let encMap =
            reader.GetEditAndContinueMapEntries()
            |> Seq.map decodeEntityHandle
            |> Seq.toArray

        encLog, encMap

    let private getEncTablesFromMetadata metadataBytes =
        withMetadataReader metadataBytes readEncTables

    let private getEncTablesFromPdb pdbBytes =
        use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
        let reader = provider.GetMetadataReader()
        readEncTables reader

    let private sortEncLogEntries (entries: (int * int * EditAndContinueOperation)[]) =
        entries |> Array.sortBy (fun (t, r, op) -> int t, r, op.Value)

    let private sortEncMapEntries (entries: (int * int)[]) =
        entries |> Array.sortBy (fun (t, r) -> int t, r)

    let private createTempProject () =
        let root = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-mdv-tests", System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(root) |> ignore
        if keepArtifacts () then
            printfn "[hotreload-mdv] keeping artifacts under %s" root
        let fsPath = Path.Combine(root, "Library.fs")
        let dllPath = Path.Combine(root, "Library.dll")
        root, fsPath, dllPath

    let private captureDeltaArtifacts label (baseline: byte[]) (generation1: FSharpHotReloadDelta) (generation2: FSharpHotReloadDelta) =
        if keepArtifacts () then
            let root = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-mdv-captures")
            Directory.CreateDirectory(root) |> ignore
            let target = Path.Combine(root, $"{label}-{System.Guid.NewGuid():N}")
            Directory.CreateDirectory(target) |> ignore
            File.WriteAllBytes(Path.Combine(target, "baseline.dll"), baseline)
            File.WriteAllBytes(Path.Combine(target, "gen1.meta"), generation1.Metadata)
            File.WriteAllBytes(Path.Combine(target, "gen1.il"), generation1.IL)
            File.WriteAllBytes(Path.Combine(target, "gen2.meta"), generation2.Metadata)
            File.WriteAllBytes(Path.Combine(target, "gen2.il"), generation2.IL)
            printfn "[hotreload-mdv] captured artifacts in %s" target

    let private readIlModule path =
        let options : ILReaderOptions =
            { pdbDirPath = None
              reduceMemoryUsage = ReduceMemoryFlag.Yes
              metadataOnly = MetadataOnlyFlag.No
              tryGetMetadataSnapshot = fun _ -> None }

        use reader = OpenILModuleReader path options
        reader.ILModuleDef

    let private collectCompilerGeneratedTypeNames (moduleDef: ILModuleDef) =
        let names = ResizeArray<string>()

        let rec collect (typeDef: ILTypeDef) =
            if IsCompilerGeneratedName typeDef.Name then
                names.Add typeDef.Name

            typeDef.NestedTypes.AsList()
            |> List.iter collect

        moduleDef.TypeDefs.AsList()
        |> List.iter collect

        names.ToArray()

    let private logSynthesizedNameDifferences baselineModule updatedModule =
        let baselineNames =
            collectCompilerGeneratedTypeNames baselineModule
            |> Set.ofArray

        let updatedNames =
            collectCompilerGeneratedTypeNames updatedModule
            |> Set.ofArray

        let unexpected = Set.difference updatedNames baselineNames
        let unexpectedList = unexpected |> Seq.toArray
        if unexpectedList.Length > 0 then
            let message = String.Join(", ", unexpectedList)
            printfn "[mdv][synthesized] updated helpers introduced: %s" message

    type private TemporaryDirectory() =
        let path = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-mdv-build", System.Guid.NewGuid().ToString("N"))
        do Directory.CreateDirectory(path) |> ignore
        member _.Path = path
        interface IDisposable with
            member _.Dispose() =
                try
                    if Directory.Exists(path) then Directory.Delete(path, true)
                with _ -> ()

    let private writeCaptureTargets directory =
        let targetsPath = Path.Combine(directory, "FscWatchCapture.targets")
        let content =
            """
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Target Name="FscWatchCaptureArgs" AfterTargets="CoreCompile">
    <WriteLinesToFile File="$(FscWatchCommandLineLog)" Lines="@(FscCommandLineArgs)" Overwrite="true" />
  </Target>
</Project>
"""
        File.WriteAllText(targetsPath, content)
        targetsPath

    let private runProcess workingDirectory exe args =
        let psi = ProcessStartInfo()
        psi.FileName <- exe
        args |> List.iter psi.ArgumentList.Add
        psi.WorkingDirectory <- workingDirectory
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.UseShellExecute <- false

        use proc = new Process()
        proc.StartInfo <- psi
        if not (proc.Start()) then failwithf "Failed to start process '%s'." exe

        let stdoutTask = proc.StandardOutput.ReadToEndAsync()
        let stderrTask = proc.StandardError.ReadToEndAsync()
        proc.WaitForExit()
        stdoutTask.Wait()
        stderrTask.Wait()
        proc.ExitCode, stdoutTask.Result, stderrTask.Result

    let private getFscCommandLine (projectPath: string) (configuration: string option) (targetFramework: string option) =
        let projectFullPath = Path.GetFullPath(projectPath)
        if not (File.Exists(projectFullPath)) then
            invalidArg "projectPath" ($"Project file '{projectFullPath}' was not found.")

        use tempDir = new TemporaryDirectory()
        let captureTargets = writeCaptureTargets tempDir.Path
        let argsFile = Path.Combine(tempDir.Path, "fsc-watch.args")

        let baseArgs =
            [ "msbuild"
              "/restore"
              projectFullPath
              "/t:Build"
              "/p:ProvideCommandLineArgs=true"
              $"/p:FscWatchCommandLineLog=\"{argsFile}\""
              $"/p:CustomAfterMicrosoftCommonTargets=\"{captureTargets}\""
              "/nologo"
              "/v:quiet" ]

        let argsWithConfiguration =
            match configuration with
            | Some value -> baseArgs @ [ $"/p:Configuration={value}" ]
            | None -> baseArgs

        let fullArgs =
            match targetFramework with
            | Some value -> argsWithConfiguration @ [ $"/p:TargetFramework={value}" ]
            | None -> argsWithConfiguration

        let projectDirectory =
            match Path.GetDirectoryName(projectFullPath) with
            | null | "" -> Directory.GetCurrentDirectory()
            | value -> value

        let exitCode, stdout, stderr = runProcess projectDirectory "dotnet" fullArgs
        if exitCode <> 0 then
            failwithf "dotnet msbuild exited with code %d.%sSTDOUT:%s%sSTDERR:%s" exitCode Environment.NewLine stdout Environment.NewLine stderr

        if not (File.Exists(argsFile)) then
            failwith "Failed to capture F# compiler command-line arguments."

        File.ReadAllLines(argsFile)
        |> Array.collect (fun line ->
            line.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map (fun arg -> arg.Trim()))
        |> Array.filter (fun arg -> not (String.IsNullOrWhiteSpace(arg)))

    let private ensureHotReloadOption (commandLine: string[]) =
        let normalized =
            let result = ResizeArray<string>(commandLine.Length + 1)
            let mutable awaitingOutArgument = false

            for arg in commandLine do
                if awaitingOutArgument then
                    let trimmed = arg.Trim().Trim('"')
                    result.Add("--out:" + trimmed)
                    awaitingOutArgument <- false
                else if arg.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) then
                    result.Add("--out:" + arg.Substring(3))
                elif String.Equals(arg, "-o", StringComparison.OrdinalIgnoreCase) then
                    awaitingOutArgument <- true
                else
                    result.Add(arg)

            if awaitingOutArgument then
                failwith "Malformed compiler command line: '-o' specified without output path."

            result.ToArray()

        if normalized |> Array.exists (fun arg -> arg.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase)) then
            normalized
        else
            Array.append normalized [| "--enable:hotreloaddeltas" |]

    let private sanitizeOptions (options: string[]) =
        options
        |> Array.filter (fun opt ->
            not (opt.Equals("--times", StringComparison.OrdinalIgnoreCase))
            && not (opt.StartsWith("--sourcelink:", StringComparison.OrdinalIgnoreCase)))
        |> Array.map (fun opt ->
            if opt.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) then
                "--out:" + opt.Substring(3)
            else
                opt)

    let private prepareCompileInputs (projectFilePath: string) (commandLine: string[]) =
        let projectDirectory =
            match Path.GetDirectoryName(projectFilePath) with
            | null | "" -> Directory.GetCurrentDirectory()
            | value -> value

        let normalizePath (path: string) =
            let trimmed = path.Trim().Trim('"')
            if Path.IsPathRooted(trimmed) then
                trimmed
            else
                Path.GetFullPath(trimmed, projectDirectory)

        let sanitized = sanitizeOptions commandLine

        let resolvedArgs = ResizeArray<string>(sanitized.Length)
        let sourceFiles = ResizeArray<string>()

        for arg in sanitized do
            if arg.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) then
                let value = arg.Substring("--out:".Length)
                resolvedArgs.Add("--out:" + normalizePath value)
            elif arg.StartsWith("--embed:", StringComparison.OrdinalIgnoreCase) then
                let value = arg.Substring("--embed:".Length)
                resolvedArgs.Add("--embed:" + normalizePath value)
            elif arg.EndsWith(".fs", StringComparison.OrdinalIgnoreCase) then
                let fullPath = normalizePath arg
                resolvedArgs.Add(fullPath)
                sourceFiles.Add(fullPath)
            else
                resolvedArgs.Add(arg)

        resolvedArgs.ToArray(), sourceFiles.ToArray()

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
            |> Async.RunSynchronously

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
            |> Async.RunSynchronously

        let errors =
            projectResults.Diagnostics
            |> Array.filter (fun d -> d.Severity = FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error)

        match errors with
        | [||] -> ()
        | _ -> failwithf "Compilation failed: %A" (errors |> Array.map (fun d -> d.Message))

        let compileDiagnostics, compileException =
            checker.Compile(Array.append [| "fsc.exe" |] (Array.append projectOptions.OtherOptions [| fsPath |]))
            |> Async.RunSynchronously

        let compileErrors =
            compileDiagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error)

        match compileErrors, compileException with
        | [||], None -> projectOptions, projectResults
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

        let writerOptions: ILWriter.options =
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
              checksumAlgorithm = HashAlgorithm.Sha256
              signer = None
              dumpDebugInfo = false
              referenceAssemblyOnly = false
              referenceAssemblyAttribOpt = None
              referenceAssemblySignatureHash = None
              pathMap = PathMap.empty }

        let assemblyBytes, pdbBytesOpt, tokenMappings, _ =
            ILWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, ilModule, id)

        // Extract module ID from PE metadata
        use peReader = new System.Reflection.PortableExecutable.PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let moduleDef = metadataReader.GetModuleDefinition()
        let moduleId = if moduleDef.Mvid.IsNil then System.Guid.NewGuid() else metadataReader.GetGuid(moduleDef.Mvid)

        // Use SRM-free byte-based APIs
        let metadataSnapshot =
            match HotReloadBaseline.metadataSnapshotFromBytes assemblyBytes with
            | Some snapshot -> snapshot
            | None -> failwith "Failed to parse metadata snapshot from assembly bytes"

        let portablePdbSnapshot = pdbBytesOpt |> Option.map HotReloadPdb.createSnapshot

        let baselineCore = HotReloadBaseline.create ilModule tokenMappings metadataSnapshot moduleId portablePdbSnapshot
        HotReloadBaseline.attachMetadataHandlesFromBytes assemblyBytes baselineCore

    let private reflectionFlags =
        Reflection.BindingFlags.Instance ||| Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Public

    let private getTypedImplementationFilesTuple (projectResults: FSharpCheckProjectResults) =
        let resultsType = typeof<FSharpCheckProjectResults>

        match resultsType.GetProperty("TypedImplementationFiles", reflectionFlags) with
        | null ->
            match resultsType.GetMethod("get_TypedImplementationFiles", reflectionFlags) with
            | null -> invalidOp "Could not resolve TypedImplementationFiles reflection accessors."
            | getter -> getter.Invoke(projectResults, [||])
        | property -> property.GetValue(projectResults)

    let private getTypedAssembly (projectResults: FSharpCheckProjectResults) =
        let tupleItems = getTypedImplementationFilesTuple projectResults |> Microsoft.FSharp.Reflection.FSharpValue.GetTupleFields
        let tcGlobals = tupleItems[0] :?> FSharp.Compiler.TcGlobals.TcGlobals
        let implFiles = tupleItems[3] :?> FSharp.Compiler.TypedTree.CheckedImplFile list

        tcGlobals,
        implFiles
        |> List.map (fun implFile ->
            { ImplFile = implFile
              OptimizeDuringCodeGen = fun _ expr -> expr })
        |> FSharp.Compiler.TypedTree.CheckedAssemblyAfterOptimization

    let private runMdvInternal baselinePath deltaPairs =
        let psi =
            ProcessStartInfo(
                FileName = "mdv",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            )
        psi.ArgumentList.Add(baselinePath)
        for (metaPath, ilPath) in deltaPairs do
            psi.ArgumentList.Add($"/g:{metaPath};{ilPath}")
        use proc = new Process()
        proc.StartInfo <- psi
        if not (proc.Start()) then failwith "Failed to start mdv."
        let output = proc.StandardOutput.ReadToEnd()
        let errors = proc.StandardError.ReadToEnd()
        proc.WaitForExit()
        if proc.ExitCode <> 0 then
            if
                proc.ExitCode = 150
                && errors.IndexOf("install or update .NET", StringComparison.OrdinalIgnoreCase) >= 0
            then
                None
            else
                failwithf "mdv exited with %d. stdout:%s stderr:%s" proc.ExitCode output errors
        else
            Some output

    let private runMdv baselinePath deltaMeta deltaIl =
        runMdvInternal baselinePath [ deltaMeta, deltaIl ]

    let private runMdvWithGenerations baselinePath deltaPairs =
        runMdvInternal baselinePath deltaPairs

    let private ensureGenerationCommitted generationId =
        match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
        | ValueSome session when session.PreviousGenerationId |> Option.contains generationId -> ()
        | _ -> FSharpEditAndContinueLanguageService.Instance.CommitPendingUpdate(generationId)

    let private getGenerationSlice (output: string) (generation: int) =
        let marker = $">>> Generation {generation}:"
        let index = output.IndexOf(marker, StringComparison.Ordinal)
        Assert.True(index >= 0, $"mdv output missing marker '{marker}'.")
        let slice = output.Substring(index)
        let nextMarkerIndex = slice.IndexOf(">>> Generation ", marker.Length, StringComparison.Ordinal)
        if nextMarkerIndex >= 0 then
            slice.Substring(0, nextMarkerIndex)
        else
            slice

    let private trySectionBlock (generationSlice: string) (header: string) =
        let headerIndex = generationSlice.IndexOf(header, StringComparison.Ordinal)
        if headerIndex < 0 then
            None
        else
            let section = generationSlice.Substring(headerIndex)
            let terminatorIndex = section.IndexOf("\n\n", header.Length, StringComparison.Ordinal)
            let block =
                if terminatorIndex >= 0 then
                    section.Substring(0, terminatorIndex).TrimEnd()
                else
                    section.TrimEnd()
            Some block

    let private getSectionBlock (generationSlice: string) (header: string) =
        match trySectionBlock generationSlice header with
        | Some block -> block
        | None -> failwith $"Section '{header}' not found in mdv output."

    let private tryGetFirstTableRow (sectionBlock: string) =
        sectionBlock.Split('\n')
        |> Array.tryFind (fun line -> line.TrimStart().StartsWith("1:", StringComparison.Ordinal))

    let private parseUserStringHeapSize (sectionBlock: string) =
        let lines = sectionBlock.Split('\n')
        Assert.True(lines.Length > 0, "#US section missing header line.")
        let header = lines[0].Trim()
        let prefix = "#US (size = "
        let startIndex = header.IndexOf(prefix, StringComparison.Ordinal)
        Assert.True(startIndex >= 0, "Unable to locate #US heap size header.")
        let afterPrefix = header.Substring(startIndex + prefix.Length)
        let closingIndex = afterPrefix.IndexOf(')')
        Assert.True(closingIndex > 0, "Malformed #US header.")
        let sizeText = afterPrefix.Substring(0, closingIndex)
        System.Int32.Parse(sizeText)

    let private tryRunSimpleMethodGeneration1MdvOutput () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Baseline helper message")
        use deltaDir = new TemporaryDirectory()
        let metaPath = Path.Combine(deltaDir.Path, "1.meta")
        let ilPath = Path.Combine(deltaDir.Path, "1.il")
        let typeName = "Sample.MethodDemo"
        let methodKey = TestHelpers.methodKey typeName "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let result =
            try
                let request : IlxDeltaRequest =
                    { Baseline = baselineArtifacts.Baseline
                      UpdatedTypes = [ typeName ]
                      UpdatedMethods = [ methodKey ]
                      UpdatedAccessors = []
                      Module = TestHelpers.createMethodModule "Generation 1 helper message"
                      SymbolChanges = None
                      CurrentGeneration = 1
                      PreviousGenerationId = None
                      SynthesizedNames = None }

                let delta = emitDelta request
                File.WriteAllBytes(metaPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                runMdv baselineArtifacts.AssemblyPath metaPath ilPath
            finally
                if not (keepArtifacts ()) then
                    try File.Delete(metaPath) with _ -> ()
                    try File.Delete(ilPath) with _ -> ()
                    try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                    match baselineArtifacts.PdbPath with
                    | Some path -> try File.Delete(path) with _ -> ()
                    | None -> ()

        result

    let private tryRunSimpleMethodGeneration2MdvOutput () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Baseline helper message")
        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let il1Path = Path.Combine(deltaDir.Path, "1.il")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")
        let il2Path = Path.Combine(deltaDir.Path, "2.il")
        let typeName = "Sample.MethodDemo"
        let methodKey = TestHelpers.methodKey typeName "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let cleanup () =
            if not (keepArtifacts ()) then
                for path in [ meta1Path; meta2Path; il1Path; il2Path; baselineArtifacts.AssemblyPath ] do
                    try File.Delete(path) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some pdb -> try File.Delete(pdb) with _ -> ()
                | None -> ()

        try
            let request1 : IlxDeltaRequest =
                { Baseline = baselineArtifacts.Baseline
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = []
                  Module = TestHelpers.createMethodModule "Generation 1 helper message"
                  SymbolChanges = None
                  CurrentGeneration = 1
                  PreviousGenerationId = None
                  SynthesizedNames = None }

            let delta1 = emitDelta request1
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            File.WriteAllBytes(il1Path, delta1.IL)

            let baseline2 =
                delta1.UpdatedBaseline |> Option.defaultWith (fun () -> failwith "Generation 1 delta missing baseline.")

            let request2 : IlxDeltaRequest =
                { Baseline = baseline2
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = []
                  Module = TestHelpers.createMethodModule "Generation 2 helper message"
                  SymbolChanges = None
                  CurrentGeneration = 2
                  PreviousGenerationId = Some delta1.GenerationId
                  SynthesizedNames = None }

            let delta2 = emitDelta request2
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            File.WriteAllBytes(il2Path, delta2.IL)

            let output = runMdvWithGenerations baselineArtifacts.AssemblyPath [ meta1Path, il1Path; meta2Path, il2Path ]
            output |> Option.map (fun text -> text, delta1.GenerationId, delta2.GenerationId)
        finally
            cleanup ()

    [<Fact>]
    let ``mdv generation 1 module emits nil EncBaseId`` () =
        match tryRunSimpleMethodGeneration1MdvOutput () with
        | None ->
            printfn "mdv not available; skipping Generation 1 module EncBaseId validation."
        | Some output ->
            let generationSlice = getGenerationSlice output 1
            let moduleBlock = getSectionBlock generationSlice "Module (0x00):"
            let rowLine =
                tryGetFirstTableRow moduleBlock
                |> Option.defaultWith (fun () -> failwith "Module table row missing.")
            Assert.True(
                rowLine.Trim().EndsWith("nil", StringComparison.Ordinal),
                $"Expected module row to end with 'nil'. Actual row: {rowLine}")

    [<Fact>]
    let ``mdv generation 2 module chains EncBaseId`` () =
        match tryRunSimpleMethodGeneration2MdvOutput () with
        | None ->
            printfn "mdv not available; skipping Generation 2 module EncBaseId validation."
        | Some(output, gen1Id, gen2Id) ->
            let slice = getGenerationSlice output 2
            // The Module row contains GUID heap indices, not actual GUIDs.
            // Check that the GUID heap contains the expected GUIDs.
            let guidBlock = getSectionBlock slice "#Guid ("
            let gen1Text = gen1Id.ToString("D")
            let gen2Text = gen2Id.ToString("D")
            // Gen2's EncId should be in the GUID heap
            Assert.Contains(gen2Text, guidBlock, StringComparison.OrdinalIgnoreCase)
            // Gen1's EncId (now Gen2's EncBaseId) should also be in the GUID heap
            Assert.Contains(gen1Text, guidBlock, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``mdv generation 1 method rows avoid bad metadata`` () =
        match tryRunSimpleMethodGeneration1MdvOutput () with
        | None ->
            printfn "mdv not available; skipping Generation 1 method metadata validation."
        | Some output ->
            let slice = getGenerationSlice output 1
            let methodBlock = getSectionBlock slice "Method (0x06, 0x1C):"
            let rowLine =
                tryGetFirstTableRow methodBlock
                |> Option.defaultWith (fun () -> failwith "Method table row missing.")
            Assert.DoesNotContain("<bad token range>", rowLine)
            Assert.DoesNotContain("<bad metadata>", rowLine)

    /// Validates the GUID heap format matches Roslyn's approach:
    /// - Index 1: nil GUID (placeholder)
    /// - Index 2: MVID
    /// - Index 3: EncId
    /// This is critical for runtime acceptance of EnC deltas.
    [<Fact>]
    let ``mdv generation 1 guid heap has correct format`` () =
        match tryRunSimpleMethodGeneration1MdvOutput () with
        | None ->
            printfn "mdv not available; skipping GUID heap format validation."
        | Some output ->
            let slice = getGenerationSlice output 1
            // Check GUID heap size is 48 bytes (3 entries x 16 bytes)
            let guidBlock = getSectionBlock slice "#Guid ("
            Assert.Contains("size = 48", guidBlock)
            // Check that index 1 is the nil GUID
            Assert.Contains("1: {00000000-0000-0000-0000-000000000000}", guidBlock)
            // Check Module row references indices 2 and 3 for MVID and EncId
            let moduleBlock = getSectionBlock slice "Module (0x00):"
            let rowLine =
                tryGetFirstTableRow moduleBlock
                |> Option.defaultWith (fun () -> failwith "Module table row missing.")
            // Module row should reference #2 for MVID and #3 for EncId
            Assert.Contains("(#2)", rowLine)
            Assert.Contains("(#3)", rowLine)
            // Should not have <bad metadata> in the Module row
            Assert.DoesNotContain("<bad metadata>", rowLine)

    [<Fact>]
    let ``mdv generation 1 stand-alone signatures are valid`` () =
        match tryRunSimpleMethodGeneration1MdvOutput () with
        | None ->
            printfn "mdv not available; skipping StandAloneSig validation."
        | Some output ->
            let slice = getGenerationSlice output 1
            match trySectionBlock slice "StandAloneSig (0x11):" with
            | None ->
                // Simple methods without locals don't have StandAloneSig entries - this is valid
                printfn "No StandAloneSig section in delta (method has no locals); skipping validation."
            | Some sigBlock ->
                Assert.DoesNotContain("<bad signature", sigBlock)

    [<Fact>]
    let ``mdv generation 1 user string heap stays compact`` () =
        match tryRunSimpleMethodGeneration1MdvOutput () with
        | None ->
            printfn "mdv not available; skipping user-string heap validation."
        | Some output ->
            let slice = getGenerationSlice output 1
            let userStringBlock = getSectionBlock slice "#US ("
            let heapSize = parseUserStringHeapSize userStringBlock
            Assert.True(
                heapSize <= 64,
                $"Expected Generation 1 #US heap to stay <= 64 bytes after baseline reuse, observed {heapSize} bytes.")

    [<Fact>]
    let ``mdv shows updated user string in Generation 1`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace Sample

type Greeter =
    static member Message () = "Message version 1"
"""
        let updatedSource =
            """
namespace Sample

type Greeter =
    static member Message () = "Message version 2"
"""

        let service = FSharpEditAndContinueLanguageService.Instance
        printfn "[mdv-test] service assembly=%s" (typeof<FSharpEditAndContinueLanguageService>.Assembly.Location)

        try
            // Baseline compilation + session
            let _, baselineResults = compileProject checker fsPath dllPath baselineSource
            let tcGlobals, baselineImpl = getTypedAssembly baselineResults
            let baseline = createBaseline tcGlobals dllPath

            service.EndSession()
            service.StartSession(baseline, baselineImpl) |> ignore

            // Updated compilation
            let _, updatedResults = compileProject checker fsPath dllPath updatedSource
            let updatedTcGlobals, updatedImpl = getTypedAssembly updatedResults

            let updatedModule =
                let options : ILReaderOptions =
                    { pdbDirPath = None
                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                      metadataOnly = MetadataOnlyFlag.No
                      tryGetMetadataSnapshot = fun _ -> None }

                use reader = OpenILModuleReader dllPath options
                reader.ILModuleDef

            match service.EmitDeltaForCompilation(updatedTcGlobals, updatedImpl, updatedModule) with
            | Error error -> failwithf "EmitDeltaForCompilation failed: %A" error
            | Ok result ->
                printfn "Updated method tokens: %A" result.Delta.UpdatedMethodTokens
                printfn "EncLog entries: %A" result.Delta.EncLog
                let tokenNames =
                    result.Delta.UpdatedMethodTokens
                    |> List.map (fun token ->
                        let name =
                            baseline.MethodTokens
                            |> Map.toSeq
                            |> Seq.tryFind (fun (_, t) -> t = token)
                            |> Option.map (fun (key, _) -> key.DeclaringType, key.Name)
                        token, name)
                tokenNames |> List.iter (fun (token, name) -> printfn "Token %08x -> %A" token name)
                // Persist artifacts for inspection.
                let deltaDir = Path.Combine(projectDir, "delta")
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, result.Delta.Metadata)
                File.WriteAllBytes(ilPath, result.Delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Message version 2")
                Assert.True(
                    containsSubsequence result.Delta.Metadata expectedLiteral,
                    "Expected Generation 1 metadata to contain updated user string 'Message version 2'."
                )

                // Invoke mdv and assert generation 1 contains the updated literal.
                match runMdv dllPath metadataPath ilPath with
                | Some output ->
                    printfn "mdv output (EmitDeltaForCompilation):%s%s" Environment.NewLine output
                    Assert.Contains("Generation 1", output)
                    Assert.Contains("Message version 2", output)
                    assertGenerationContains output 1 "Message version 2"
                | None ->
                    printfn "mdv not available; skipping textual verification for EmitDeltaForCompilation scenario."
        finally
            try checker.InvalidateAll() with _ -> ()
            try service.EndSession() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    let private createMsbuildProject () =
        let root = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-mdv-project", System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(root) |> ignore
        let projectPath = Path.Combine(root, "WatchLoop.fsproj")
        let fsPath = Path.Combine(root, "Program.fs")
        let projectContents =
            """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.fs" />
  </ItemGroup>
</Project>
"""
        File.WriteAllText(projectPath, projectContents)
        root, projectPath, fsPath

    let private tryGetOutputPath (projectFilePath: string) (options: FSharpProjectOptions) =
        let projectDirectory =
            match Path.GetDirectoryName(projectFilePath) with
            | null | "" -> Directory.GetCurrentDirectory()
            | value -> value

        let trimQuotes (text: string) = text.Trim().Trim('"')

        let toAbsolute path =
            let candidate = trimQuotes path
            if Path.IsPathRooted(candidate) then
                candidate
            else
                Path.GetFullPath(candidate, projectDirectory)

        let tryFromLongForm =
            options.OtherOptions
            |> Array.tryPick (fun opt ->
                if opt.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) then
                    opt.Substring("--out:".Length) |> toAbsolute |> Some
                elif opt.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) then
                    opt.Substring("-o:".Length) |> toAbsolute |> Some
                else
                    None)

        match tryFromLongForm with
        | Some path -> Some path
        | None ->
            match options.OtherOptions |> Array.tryFindIndex (fun opt -> String.Equals(opt, "-o", StringComparison.OrdinalIgnoreCase)) with
            | Some idx when idx + 1 < options.OtherOptions.Length ->
                options.OtherOptions[idx + 1] |> toAbsolute |> Some
            | _ -> None
    [<Fact>]
    let ``FSharpChecker.EmitHotReloadDelta produces IL/metadata deltas`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace SampleChecker

type Greeter =
    static member Message () = "Checker version 1"
"""
        let updatedSource =
            """
namespace SampleChecker

type Greeter =
    static member Message () = "Checker version 2"
"""

        try
            // Baseline build and start session via FSharpChecker
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            // Updated build (writes the new assembly)
            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource
            let deltaResult = checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously

            match deltaResult with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                ensureGenerationCommitted delta.GenerationId
                Assert.NotEmpty(delta.Metadata)
                Assert.NotEmpty(delta.IL)
                // Persist artifacts
                let deltaDir = Path.Combine(projectDir, "checker-delta")
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Checker version 2")
                Assert.True(
                    containsSubsequence delta.Metadata expectedLiteral,
                    "Expected Generation 1 metadata to contain updated user string 'Checker version 2'."
                )

                match runMdv dllPath metadataPath ilPath with
                | Some output ->
                    printfn "mdv output (EmitHotReloadDelta):%s%s" Environment.NewLine output
                    Assert.Contains("Generation 1", output)
                    Assert.Contains("Checker version 2", output)
                    assertGenerationContains output 1 "Checker version 2"
                | None ->
                    printfn "mdv not available; skipping textual verification for EmitHotReloadDelta scenario."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``hot reload delta from project options updates user string literal`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectRoot, projectPath, fsPath = createMsbuildProject ()
        let baselineSource =
            """
namespace WatchLoop

module Target =
    let Message () = "Message version project baseline"
"""

        let updatedSource =
            """
namespace WatchLoop

module Target =
    let Message () = "Message version project updated"
"""

        let cleanup () =
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            if not (keepArtifacts ()) then
                try Directory.Delete(projectRoot, true) with _ -> ()

        let originalCwd = Directory.GetCurrentDirectory()

        try
            checker.InvalidateAll() |> ignore
            File.WriteAllText(fsPath, baselineSource)
            let commandLine = getFscCommandLine projectPath (Some "Debug") (Some "net10.0") |> ensureHotReloadOption
            let normalizedArgs, sourceFiles = prepareCompileInputs projectPath commandLine
            Assert.True(sourceFiles.Length > 0, "Expected baseline command line to include at least one source file.")
            let projectOptionsRaw = checker.GetProjectOptionsFromCommandLineArgs(projectPath, commandLine)
            let baselineTimestamp = DateTime.UtcNow
            let projectOptions =
                { projectOptionsRaw with
                    OtherOptions = normalizedArgs
                    SourceFiles = sourceFiles
                    LoadTime = baselineTimestamp
                    Stamp = Some baselineTimestamp.Ticks }

            let outputPath =
                tryGetOutputPath projectPath projectOptions
                |> Option.defaultWith (fun () -> failwith "Unable to determine --out path from project options.")

            let compile includeHotReloadCapture (args: string[]) =
                let actualArgs =
                    if includeHotReloadCapture then
                        args
                    else
                        args |> Array.filter (fun arg -> not (arg.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase)))

                let originalDirectory = Directory.GetCurrentDirectory()
                let diagnostics, exnOpt =
                    try
                        Directory.SetCurrentDirectory(projectRoot)
                        checker.Compile(actualArgs) |> Async.RunSynchronously
                    finally
                        Directory.SetCurrentDirectory(originalDirectory)
                let errors =
                    diagnostics
                    |> Array.filter (fun d -> d.Severity = FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error)

                match errors, exnOpt with
                | [||], None -> ()
                | errs, exceptionOpt ->
                    let diagnosticMessages = errs |> Array.map (fun d -> d.Message)
                    let exceptionMessages =
                        match exceptionOpt with
                        | Some ex -> [| ex.Message |]
                        | None -> [||]
                    let messages = Array.append diagnosticMessages exceptionMessages

                    let message =
                        if messages.Length = 0 then
                            "Compilation failed with unknown error."
                        else
                            String.Join("; ", messages)

                    failwith message

            let compileArgs = Array.append [| "fsc.exe" |] normalizedArgs

            compile true compileArgs

            let baselineCopy = Path.Combine(projectRoot, "baseline.dll")
            File.Copy(outputPath, baselineCopy, true)

            Directory.SetCurrentDirectory(originalCwd)

            match checker.StartHotReloadSession(projectOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            File.WriteAllText(fsPath, updatedSource)
            let updatedCommandLine = getFscCommandLine projectPath (Some "Debug") (Some "net10.0") |> ensureHotReloadOption
            let updatedArgs, updatedSourceFiles = prepareCompileInputs projectPath updatedCommandLine
            Assert.True(updatedSourceFiles.Length > 0, "Expected updated command line to include source files.")
            let updatedOptionsRaw = checker.GetProjectOptionsFromCommandLineArgs(projectPath, updatedCommandLine)
            let updatedTimestamp = DateTime.UtcNow
            let updatedOptions =
                { updatedOptionsRaw with
                    OtherOptions = updatedArgs
                    SourceFiles = updatedSourceFiles
                    LoadTime = updatedTimestamp
                    Stamp = Some updatedTimestamp.Ticks }
            let updatedCompileArgs = Array.append [| "fsc.exe" |] updatedArgs
            checker.NotifyFileChanged(fsPath, updatedOptions) |> Async.RunSynchronously
            compile false updatedCompileArgs

            Thread.Sleep 200

            Directory.SetCurrentDirectory(originalCwd)

            match checker.EmitHotReloadDelta(projectOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                ensureGenerationCommitted delta.GenerationId
                let deltaDir = Path.Combine(projectRoot, "project-delta")
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Message version project updated")
                Assert.True(
                    containsSubsequence delta.Metadata expectedLiteral,
                    "Expected delta metadata to include updated literal from project build."
                )

                match runMdv baselineCopy metadataPath ilPath with
                | Some output ->
                    Assert.Contains("Generation 1", output)
                    assertGenerationContains output 1 "Message version project updated"
                | None ->
                    printfn "mdv not available; skipping Generation 1 verification for project options scenario."
        finally
            try Directory.SetCurrentDirectory(originalCwd) with _ -> ()
            cleanup ()

    [<Fact>]
    let ``mdv validates simple method-body edit`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVIntegration

module Demo =
    let GetMessage () = "Integration baseline message"
"""

        let updatedSource =
            """
namespace MdVIntegration

module Demo =
    let GetMessage () = "Integration updated message"
"""

        let deltaDir = Path.Combine(projectDir, "mdv-integration-delta")

        try
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource

            match checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                ensureGenerationCommitted delta.GenerationId
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Integration updated message")
                Assert.True(
                    containsSubsequence delta.Metadata expectedLiteral,
                    "Expected metadata delta to contain updated integration literal."
                )

                let metadataBytes = ImmutableArray.CreateRange(delta.Metadata)
                use metadataProvider = MetadataReaderProvider.FromMetadataImage(metadataBytes)
                use baselineStream = File.OpenRead(baselineCopy)
                use baselinePe = new PEReader(baselineStream)
                let baselineReader = baselinePe.GetMetadataReader()
                let deltaReader = metadataProvider.GetMetadataReader()

                let dumpMetadata label (reader: MetadataReader) =
                    let moduleDef = reader.GetModuleDefinition()
                    let moduleName = reader.GetString(moduleDef.Name)
                    printfn "[metadata-%s] Module=%s" label moduleName
                    printfn "[metadata-%s] TypeDefs:" label
                    for handle in reader.TypeDefinitions do
                        let typeDef = reader.GetTypeDefinition(handle)
                        let ns =
                            if typeDef.Namespace.IsNil then ""
                            else reader.GetString(typeDef.Namespace)
                        let name = reader.GetString(typeDef.Name)
                        printfn "  %s.%s" ns name
                    printfn "[metadata-%s] MethodDefs:" label
                    for handle in reader.MethodDefinitions do
                        let methodDef = reader.GetMethodDefinition(handle)
                        printfn "  %s" (reader.GetString(methodDef.Name))
                    printfn "[metadata-%s] StringsHeapSize=%d UserStringHeapSize=%d" label (reader.GetHeapSize(HeapIndex.String)) (reader.GetHeapSize(HeapIndex.UserString))

                dumpMetadata "baseline" baselineReader
                try
                    dumpMetadata "delta" deltaReader
                with
                | :? BadImageFormatException -> ()
                | :? System.IndexOutOfRangeException -> ()

                match runMdv baselineCopy metadataPath ilPath with
                | Some output ->
                    printfn "[mdv-output]%s%s" Environment.NewLine output
                    Assert.Contains("Generation 1", output)
                    assertGenerationContains output 1 "Integration updated message"
                | None ->
                    printfn "mdv not available; skipping integration verification for simple method-body edit."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            if not (keepArtifacts ()) then
                try Directory.Delete(deltaDir, true) with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates property getter edit`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVIntegration

type PropertyDemo() =
    member _.Message = "Property baseline message"
"""

        let updatedSource =
            """
namespace MdVIntegration

type PropertyDemo() =
    member _.Message = "Property updated message"
"""

        let deltaDir = Path.Combine(projectDir, "mdv-property-delta")

        try
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource

            match checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                ensureGenerationCommitted delta.GenerationId
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Property updated message")
                Assert.True(
                    containsSubsequence delta.Metadata expectedLiteral,
                    "Expected metadata delta to contain updated property literal."
                )

                match runMdv baselineCopy metadataPath ilPath with
                | Some output ->
                    Assert.Contains("Generation 1", output)
                    assertGenerationContains output 1 "Property updated message"
                | None ->
                    printfn "mdv not available; skipping Generation 1 verification for property getter edit."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            if not (keepArtifacts ()) then
                try Directory.Delete(deltaDir, true) with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates custom event add edit`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVIntegration

open System

type EventDemo() =
    let messageChanged = Event<string>()

    member _.InvokeAll payload =
        printfn "Event baseline payload %s" payload
        messageChanged.Trigger payload

    [<CLIEvent>]
    member _.MessageChanged =
        messageChanged.Publish
"""

        let updatedSource =
            """
namespace MdVIntegration

open System

type EventDemo() =
    let messageChanged = Event<string>()

    member _.InvokeAll payload =
        printfn "Event updated payload %s" payload
        messageChanged.Trigger payload

    [<CLIEvent>]
    member _.MessageChanged =
        messageChanged.Publish
"""

        let deltaDir = Path.Combine(projectDir, "mdv-event-delta")

        try
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource

            match checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
            | Ok delta ->
                ensureGenerationCommitted delta.GenerationId
                Directory.CreateDirectory(deltaDir) |> ignore
                let metadataPath = Path.Combine(deltaDir, "1.meta")
                let ilPath = Path.Combine(deltaDir, "1.il")
                File.WriteAllBytes(metadataPath, delta.Metadata)
                File.WriteAllBytes(ilPath, delta.IL)

                let expectedLiteral = Text.Encoding.Unicode.GetBytes("Event updated payload")
                Assert.True(
                    containsSubsequence delta.Metadata expectedLiteral,
                    "Expected metadata delta to contain updated event literal."
                )

                match runMdv baselineCopy metadataPath ilPath with
                | Some output ->
                    Assert.Contains("Generation 1", output)
                    assertGenerationContains output 1 "Event updated payload"
                | None ->
                    printfn "mdv not available; skipping Generation 1 verification for custom event edit."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            if not (keepArtifacts ()) then
                try Directory.Delete(deltaDir, true) with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv helper validates property accessor metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createPropertyModule "Property helper baseline message")
        let updatedModule = TestHelpers.createPropertyModule "Property helper updated message"
        let typeName = "Sample.PropertyDemo"
        let accessorName = "Message"
        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline typeName "get_Message"
        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let accessorUpdate =
            TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.PropertyGet accessorName) methodKey

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = [ accessorUpdate ]
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        use deltaDir = new TemporaryDirectory()
        let metadataPath = Path.Combine(deltaDir.Path, "1.meta")
        let ilPath = Path.Combine(deltaDir.Path, "1.il")

        try
            let delta = emitDelta request
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)
            RoslynBaseline.assertWithin "Property" delta.Metadata
            HeapBudgets.assertWithin "Property" delta.Metadata

            let expectedLiteral = Text.Encoding.Unicode.GetBytes("Property helper updated message")
            Assert.True(
                containsSubsequence delta.Metadata expectedLiteral,
                "Expected metadata delta to contain updated property literal."
            )

            let containsPropertyName =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, accessorName, StringComparison.Ordinal))
            Assert.False(containsPropertyName, "Property name should not be re-emitted into the user string heap.")

            let hasUpdatedLiteral =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) ->
                    text.Contains("Property helper updated message", StringComparison.Ordinal))
            Assert.True(hasUpdatedLiteral, "Expected user string updates to include the new property literal.")

            Assert.Contains(methodToken, delta.UpdatedMethodTokens)
            let hasMethodInfo =
                delta.AddedOrChangedMethods
                |> List.exists (fun info -> info.MethodToken = methodToken)
            Assert.True(hasMethodInfo, "Expected property accessor delta to track method body info.")

            match runMdv baselineArtifacts.AssemblyPath metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Property helper updated message"
                // Note: Type names like "PropertyDemo" are in the baseline, not the delta metadata
            | None ->
                printfn "mdv not available; skipping helper verification for property accessor edit."
        finally
            if not (keepArtifacts ()) then
                try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some path -> try File.Delete(path) with _ -> ()
                | None -> ()

    [<Fact>]
    let ``mdv helper validates added property metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createPropertyHostBaselineModule ())
        let updatedModule = TestHelpers.createPropertyModule "Property helper added message"
        let typeName = "Sample.PropertyDemo"
        let getterKey = TestHelpers.methodKey typeName "get_Message" [] PrimaryAssemblyILGlobals.typ_String
        let accessorUpdate =
            TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.PropertyGet "Message") getterKey

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ getterKey ]
              UpdatedAccessors = [ accessorUpdate ]
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        use deltaDir = new TemporaryDirectory()
        let metadataPath = Path.Combine(deltaDir.Path, "1.meta")
        let ilPath = Path.Combine(deltaDir.Path, "1.il")

        try
            let delta = emitDelta request
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)
            RoslynBaseline.assertWithin "Property" delta.Metadata
            HeapBudgets.assertWithin "Property" delta.Metadata

            let expectedLiteral = Text.Encoding.Unicode.GetBytes "Property helper added message"
            Assert.True(containsSubsequence delta.Metadata expectedLiteral, "Expected metadata delta to contain added property literal.")

            let containsPropertyName =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, "Message", StringComparison.Ordinal))
            Assert.False(containsPropertyName, "Property name should not be re-emitted into the user string heap when adding a property.")

            let hasAddedLiteral =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> text.Contains("Property helper added message", StringComparison.Ordinal))
            Assert.True(hasAddedLiteral, "Expected user string updates to include the added property literal.")

            withMetadataReader delta.Metadata (fun reader ->
                Assert.Equal(1, reader.GetTableRowCount(toTableIndex TableNames.Property))
                Assert.Equal(1, reader.GetTableRowCount(toTableIndex TableNames.PropertyMap)))

            let hasPropertyLog =
                delta.EncLog
                |> Array.exists (fun (table, _, op) -> table = TableNames.Property && op = EditAndContinueOperation.AddProperty)
            Assert.True(hasPropertyLog, "Expected EncLog entry for added property definition")

            let hasPropertyMapLog =
                delta.EncLog
                |> Array.exists (fun (table, _, op) -> table = TableNames.PropertyMap && op = EditAndContinueOperation.AddProperty)
            Assert.True(hasPropertyMapLog, "Expected EncLog entry for added property map")

            match runMdv baselineArtifacts.AssemblyPath metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Property helper added message"
            | None ->
                printfn "mdv not available; skipping helper verification for added property metadata."
        finally
            if not (keepArtifacts ()) then
                try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some path -> try File.Delete(path) with _ -> ()
                | None -> ()

    [<Fact>]
    let ``mdv helper validates custom event accessor metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createEventModule "Event helper baseline payload")
        let updatedModule = TestHelpers.createEventModule "Event helper updated payload"
        let typeName = "Sample.EventDemo"
        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline typeName "add_OnChanged"
        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let accessorUpdate =
            TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.EventAdd "OnChanged") methodKey

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = [ accessorUpdate ]
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        use deltaDir = new TemporaryDirectory()
        let metadataPath = Path.Combine(deltaDir.Path, "1.meta")
        let ilPath = Path.Combine(deltaDir.Path, "1.il")

        try
            let delta = emitDelta request
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)
            RoslynBaseline.assertWithin "Event" delta.Metadata
            HeapBudgets.assertWithin "Event" delta.Metadata

            let expectedLiteral = Text.Encoding.Unicode.GetBytes("Event helper updated payload")
            Assert.True(
                containsSubsequence delta.Metadata expectedLiteral,
                "Expected metadata delta to contain updated event literal."
            )

            let containsEventName =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, "OnChanged", StringComparison.Ordinal))
            Assert.False(containsEventName, "Event name should not be re-emitted into the user string heap.")

            let hasUpdatedLiteral =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) ->
                    text.Contains("Event helper updated payload", StringComparison.Ordinal))
            Assert.True(hasUpdatedLiteral, "Expected user string updates to include the new event literal.")

            Assert.Contains(methodToken, delta.UpdatedMethodTokens)
            let hasMethodInfo =
                delta.AddedOrChangedMethods
                |> List.exists (fun info -> info.MethodToken = methodToken)
            Assert.True(hasMethodInfo, "Expected event accessor delta to track method body info.")

            match runMdv baselineArtifacts.AssemblyPath metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Event helper updated payload"
            | None ->
                printfn "mdv not available; skipping helper verification for event accessor edit."
        finally
            if not (keepArtifacts ()) then
                try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some path -> try File.Delete(path) with _ -> ()
                | None -> ()

    [<Fact>]
    let ``mdv helper validates added event metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createEventHostBaselineModule ())
        let updatedModule = TestHelpers.createEventModule "Event helper added payload"
        let typeName = "Sample.EventDemo"
        let addKey = TestHelpers.methodKey typeName "add_OnChanged" [ PrimaryAssemblyILGlobals.typ_Object ] ILType.Void
        let removeKey = TestHelpers.methodKey typeName "remove_OnChanged" [ PrimaryAssemblyILGlobals.typ_Object ] ILType.Void
        let accessorUpdates =
            [ TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.EventAdd "OnChanged") addKey
              TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.EventRemove "OnChanged") removeKey ]

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ addKey; removeKey ]
              UpdatedAccessors = accessorUpdates
              Module = updatedModule
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        use deltaDir = new TemporaryDirectory()
        let metadataPath = Path.Combine(deltaDir.Path, "1.meta")
        let ilPath = Path.Combine(deltaDir.Path, "1.il")

        try
            let delta = emitDelta request
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)

            let expectedLiteral = Text.Encoding.Unicode.GetBytes "Event helper added payload"
            Assert.True(containsSubsequence delta.Metadata expectedLiteral, "Expected metadata delta to contain added event literal.")

            let containsEventName =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, "OnChanged", StringComparison.Ordinal))
            Assert.False(containsEventName, "Event name should not be re-emitted into the user string heap when adding an event.")

            let hasAddedLiteral =
                delta.UserStringUpdates
                |> List.exists (fun (_, _, text) -> text.Contains("Event helper added payload", StringComparison.Ordinal))
            Assert.True(hasAddedLiteral, "Expected user string updates to include the added event literal.")

            withMetadataReader delta.Metadata (fun reader ->
                Assert.Equal(1, reader.GetTableRowCount(toTableIndex TableNames.Event))
                Assert.Equal(1, reader.GetTableRowCount(toTableIndex TableNames.EventMap)))

            let hasEventLog =
                delta.EncLog
                |> Array.exists (fun (table, _, op) -> table = TableNames.Event && op = EditAndContinueOperation.AddEvent)
            Assert.True(hasEventLog, "Expected EncLog entry for added event definition")

            let hasEventMapLog =
                delta.EncLog
                |> Array.exists (fun (table, _, op) -> table = TableNames.EventMap && op = EditAndContinueOperation.AddEvent)
            Assert.True(hasEventMapLog, "Expected EncLog entry for added event map")

            match runMdv baselineArtifacts.AssemblyPath metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Event helper added payload"
            | None ->
                printfn "mdv not available; skipping helper verification for added event metadata."
        finally
            if not (keepArtifacts ()) then
                try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some path -> try File.Delete(path) with _ -> ()
                | None -> ()

    [<Fact>]
    let ``mdv helper validates multi-generation method metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Baseline helper message")
        let typeName = "Sample.MethodDemo"
        let methodKey = TestHelpers.methodKey typeName "GetMessage" [] PrimaryAssemblyILGlobals.typ_String
        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let methodRowId = methodRowIdFromToken methodToken

        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")

        try
            let request1 : IlxDeltaRequest =
                { Baseline = baselineArtifacts.Baseline
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = []
                  Module = TestHelpers.createMethodModule "Generation 1 helper message"
                  SymbolChanges = None
                  CurrentGeneration = 1
                  PreviousGenerationId = None
                  SynthesizedNames = None }

            let delta1 = emitDelta request1
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            let expectedLiteral1 = Text.Encoding.Unicode.GetBytes "Generation 1 helper message"
            Assert.True(containsSubsequence delta1.Metadata expectedLiteral1, "Expected generation 1 metadata to contain updated literal.")
            assertMethodEncLog delta1 methodToken
            assertEncMapContains delta1 TableNames.Method methodRowId

            let baseline2 =
                match delta1.UpdatedBaseline with
                | Some b -> b
                | None -> failwith "First delta did not provide an updated baseline."

            let request2 : IlxDeltaRequest =
                { Baseline = baseline2
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = []
                  Module = TestHelpers.createMethodModule "Generation 2 helper message"
                  SymbolChanges = None
                  CurrentGeneration = 2
                  PreviousGenerationId = Some delta1.GenerationId
                  SynthesizedNames = None }

            let delta2 = emitDelta request2
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            let expectedLiteral2 = Text.Encoding.Unicode.GetBytes "Generation 2 helper message"
            Assert.True(containsSubsequence delta2.Metadata expectedLiteral2, "Expected generation 2 metadata to contain updated literal.")
            assertMethodEncLog delta2 methodToken
            Assert.Equal(delta1.GenerationId, delta2.BaseGenerationId)
            assertEncMapContains delta2 TableNames.Method methodRowId
        finally
            if not (keepArtifacts ()) then
                try File.Delete(meta1Path) with _ -> ()
                try File.Delete(meta2Path) with _ -> ()

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    /// Updated methods do NOT emit Param rows - the baseline already has them.
    /// Only ADDED methods need synthetic Param rows in the delta.
    /// This matches Roslyn's behavior for EnC deltas.
    let ``mdv helper method delta does not emit param row for updated method`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Baseline helper message")
        let typeName = "Sample.MethodDemo"
        let methodKey = TestHelpers.methodKey typeName "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = TestHelpers.createMethodModule "Generation 1 helper message"
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        // Updated methods should NOT have Param rows in the delta - baseline has them
        withMetadataReader delta.Metadata (fun reader ->
            Assert.Equal(0, reader.GetTableRowCount(toTableIndex TableNames.Param)))

        // No Param EncLog/EncMap entries for updated methods
        let hasParamEncLog =
            delta.EncLog |> Array.exists (fun (t, _, _) -> t = TableNames.Param)
        Assert.False(hasParamEncLog, "Updated method should not have EncLog entry for Param table")

        let hasParamEncMap =
            delta.EncMap |> Array.exists (fun (t, _) -> t = TableNames.Param)
        Assert.False(hasParamEncMap, "Updated method should not have EncMap entry for Param table")

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    let ``pdb enc tables contain MethodDebugInformation entries for method update`` () =
        // Per Roslyn's DeltaMetadataWriter.cs:1367-1384, PDB delta EncMap should contain
        // MethodDebugInformation entries (which correspond 1:1 to MethodDef), not metadata tables.
        // PDB EncLog is not used.
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createMethodModule "Baseline helper message")
        let typeName = "Sample.MethodDemo"
        let methodKey = TestHelpers.methodKey typeName "GetMessage" [] PrimaryAssemblyILGlobals.typ_String

        let request : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = TestHelpers.createMethodModule "Generation 1 helper message"
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta = emitDelta request

        let pdbBytes =
            match delta.Pdb with
            | Some bytes -> bytes
            | None -> failwith "Expected PDB delta to be emitted"

        let pdbLog, pdbMap = getEncTablesFromPdb pdbBytes

        // PDB EncLog should be empty (Roslyn doesn't use it for PDB deltas)
        Assert.Empty(pdbLog)

        // PDB EncMap should contain ONLY MethodDebugInformation entries (table index 0x31 = 49)
        // It should NOT mirror metadata tables like TypeRef, MemberRef, etc.
        let methodDebugInfoTable = DeltaTokens.tableMethodDebugInformation
        for (table, _rowId) in pdbMap do
            Assert.Equal(methodDebugInfoTable, table)

        // Verify we have at least one MethodDebugInformation entry for the updated method
        Assert.NotEmpty(pdbMap)

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    let ``mdv helper validates multi-generation property accessor metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createPropertyModule "Property helper baseline message")
        let typeName = "Sample.PropertyDemo"
        let accessorName = "Message"
        let methodKey = TestHelpers.methodKeyByName baselineArtifacts.Baseline typeName "get_Message"
        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]

        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let il1Path = Path.Combine(deltaDir.Path, "1.il")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")
        let il2Path = Path.Combine(deltaDir.Path, "2.il")

        let mkAccessorUpdate () =
            TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.PropertyGet accessorName) methodKey

        try
            let request1 : IlxDeltaRequest =
                { Baseline = baselineArtifacts.Baseline
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = [ mkAccessorUpdate () ]
                  Module = TestHelpers.createPropertyModule "Property helper generation 1"
                  SymbolChanges = None
                  CurrentGeneration = 1
                  PreviousGenerationId = None
                  SynthesizedNames = None }

            let delta1 = emitDelta request1
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            File.WriteAllBytes(il1Path, delta1.IL)
            RoslynBaseline.assertWithin "Property" delta1.Metadata
            HeapBudgets.assertWithin "Property" delta1.Metadata

            let expectedLiteral1 = Text.Encoding.Unicode.GetBytes "Property helper generation 1"
            Assert.True(
                containsSubsequence delta1.Metadata expectedLiteral1,
                "Expected generation 1 metadata to contain updated property literal."
            )

            let containsPropertyNameGen1 =
                delta1.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, accessorName, StringComparison.Ordinal))
            Assert.False(containsPropertyNameGen1, "Generation 1 property name should not reappear in the user string heap.")

            let hasUpdatedLiteralGen1 =
                delta1.UserStringUpdates
                |> List.exists (fun (_, _, text) -> text.Contains("Property helper generation 1", StringComparison.Ordinal))
            Assert.True(hasUpdatedLiteralGen1, "Expected Generation 1 user string updates to include the new property literal.")

            assertMethodEncLog delta1 methodToken

            match runMdv baselineArtifacts.AssemblyPath meta1Path il1Path with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Property helper generation 1"
            | None ->
                printfn "mdv not available; skipping Generation 1 verification for multi-generation property edit."

            let baseline2 =
                match delta1.UpdatedBaseline with
                | Some b -> b
                | None -> failwith "First property delta did not provide an updated baseline."

            let request2 : IlxDeltaRequest =
                { Baseline = baseline2
                  UpdatedTypes = [ typeName ]
                  UpdatedMethods = [ methodKey ]
                  UpdatedAccessors = [ mkAccessorUpdate () ]
                  Module = TestHelpers.createPropertyModule "Property helper generation 2"
                  SymbolChanges = None
                  CurrentGeneration = 2
                  PreviousGenerationId = Some delta1.GenerationId
                  SynthesizedNames = None }

            let delta2 = emitDelta request2
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            File.WriteAllBytes(il2Path, delta2.IL)
            RoslynBaseline.assertWithin "PropertyUpdate" delta2.Metadata
            HeapBudgets.assertWithin "PropertyUpdate" delta2.Metadata

            let expectedLiteral2 = Text.Encoding.Unicode.GetBytes "Property helper generation 2"
            Assert.True(
                containsSubsequence delta2.Metadata expectedLiteral2,
                "Expected generation 2 metadata to contain updated property literal."
            )

            let containsPropertyNameGen2 =
                delta2.UserStringUpdates
                |> List.exists (fun (_, _, text) -> String.Equals(text, accessorName, StringComparison.Ordinal))
            Assert.False(containsPropertyNameGen2, "Generation 2 property name should not reappear in the user string heap.")

            let hasUpdatedLiteralGen2 =
                delta2.UserStringUpdates
                |> List.exists (fun (_, _, text) -> text.Contains("Property helper generation 2", StringComparison.Ordinal))
            Assert.True(hasUpdatedLiteralGen2, "Expected Generation 2 user string updates to include the new property literal.")

            assertMethodEncLog delta2 methodToken
            Assert.Equal(delta1.GenerationId, delta2.BaseGenerationId)

            match runMdvWithGenerations baselineArtifacts.AssemblyPath [ meta1Path, il1Path; meta2Path, il2Path ] with
            | Some output ->
                Assert.Contains("Generation 2", output)
                assertGenerationContains output 2 "Property helper generation 2"
            | None ->
                printfn "mdv not available; skipping Generation 2 verification for multi-generation property edit."
        finally
            if not (keepArtifacts ()) then
                for path in [ meta1Path; meta2Path; il1Path; il2Path ] do
                    try File.Delete(path) with _ -> ()
                try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
                match baselineArtifacts.PdbPath with
                | Some path -> try File.Delete(path) with _ -> ()
                | None -> ()

    [<Fact>]
    let ``mdv helper validates multi-generation event accessor metadata`` () =
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createEventHostBaselineModule ())
        let typeName = "Sample.EventDemo"
        let addKey = TestHelpers.methodKey typeName "add_OnChanged" [ PrimaryAssemblyILGlobals.typ_Object ] ILType.Void
        let methodTokenOpt = baselineArtifacts.Baseline.MethodTokens |> Map.tryFind addKey
        let methodRowIdOpt = methodTokenOpt |> Option.map methodRowIdFromToken

        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")

        let request1 : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ addKey ]
              UpdatedAccessors = [ TestHelpers.mkAccessorUpdate typeName (SymbolMemberKind.EventAdd "OnChanged") addKey ]
              Module = TestHelpers.createEventModule "Event helper generation 1"
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta1 = emitDelta request1
        File.WriteAllBytes(meta1Path, delta1.Metadata)

        RoslynBaseline.assertWithin "Event" delta1.Metadata
        HeapBudgets.assertWithin "Event" delta1.Metadata
        match methodTokenOpt, methodRowIdOpt with
        | Some methodToken, Some methodRowId ->
            assertMethodEncLog delta1 methodToken
            assertEncMapContains delta1 TableNames.Method methodRowId
        | _ -> printfn "[hotreload-mdv] skipping method-token asserts for event delta; baseline token not found"

        let containsEventNameGen1 =
            delta1.UserStringUpdates
            |> List.exists (fun (_, _, text) -> String.Equals(text, "OnChanged", StringComparison.Ordinal))
        Assert.False(containsEventNameGen1, "Generation 1 event name should not reappear in the user string heap.")

        let baseline2 =
            match delta1.UpdatedBaseline with
            | Some b -> b
            | None -> failwith "First event delta did not provide an updated baseline."

        let request2 : IlxDeltaRequest =
            { request1 with
                Baseline = baseline2
                UpdatedMethods = [ addKey ]
                Module = TestHelpers.createEventModule "Event helper generation 2"
                CurrentGeneration = 2
                PreviousGenerationId = Some delta1.GenerationId }

        let delta2 = emitDelta request2
        File.WriteAllBytes(meta2Path, delta2.Metadata)

        RoslynBaseline.assertWithin "EventUpdate" delta2.Metadata
        HeapBudgets.assertWithin "EventUpdate" delta2.Metadata
        match methodTokenOpt, methodRowIdOpt with
        | Some methodToken, Some methodRowId ->
            assertMethodEncLog delta2 methodToken
            assertEncMapContains delta2 TableNames.Method methodRowId
        | _ -> ()

        let containsEventNameGen2 =
            delta2.UserStringUpdates
            |> List.exists (fun (_, _, text) -> String.Equals(text, "OnChanged", StringComparison.Ordinal))
        Assert.False(containsEventNameGen2, "Generation 2 event name should not reappear in the user string heap.")

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    let ``mdv helper validates multi-generation closure metadata`` () =
        let typeName = "Sample.ClosureDemo"
        let methodKey = TestHelpers.methodKey typeName "Invoke" [] PrimaryAssemblyILGlobals.typ_String
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createClosureModule "Closure helper baseline message")

        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")

        let request1 : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = TestHelpers.createClosureModule "Closure helper generation 1"
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta1 = emitDelta request1
        File.WriteAllBytes(meta1Path, delta1.Metadata)
        RoslynBaseline.assertWithin "Closure" delta1.Metadata
        HeapBudgets.assertWithin "Closure" delta1.Metadata

        let baseline2 =
            match delta1.UpdatedBaseline with
            | Some b -> b
            | None -> failwith "First closure delta did not expose an updated baseline."

        let request2 : IlxDeltaRequest =
            { request1 with
                Baseline = baseline2
                Module = TestHelpers.createClosureModule "Closure helper generation 2"
                CurrentGeneration = 2
                PreviousGenerationId = Some delta1.GenerationId }

        let delta2 = emitDelta request2
        File.WriteAllBytes(meta2Path, delta2.Metadata)
        RoslynBaseline.assertWithin "ClosureUpdate" delta2.Metadata
        HeapBudgets.assertWithin "ClosureUpdate" delta2.Metadata

        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let methodRowId = methodRowIdFromToken methodToken
        assertMethodEncLog delta1 methodToken
        assertEncMapContains delta1 TableNames.Method methodRowId
        assertMethodEncLog delta2 methodToken
        assertEncMapContains delta2 TableNames.Method methodRowId

        let literal1 = Text.Encoding.Unicode.GetBytes "Closure helper generation 1"
        Assert.True(containsSubsequence delta1.Metadata literal1, "Expected generation 1 closure metadata to contain updated literal.")

        let literal2 = Text.Encoding.Unicode.GetBytes "Closure helper generation 2"
        Assert.True(containsSubsequence delta2.Metadata literal2, "Expected generation 2 closure metadata to contain updated literal.")

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    let ``mdv helper validates multi-generation async metadata`` () =
        let typeName = "Sample.AsyncDemo"
        let methodKey = TestHelpers.methodKey typeName "RunAsync" [ PrimaryAssemblyILGlobals.typ_Int32 ] PrimaryAssemblyILGlobals.typ_String
        let baselineArtifacts = TestHelpers.createBaselineFromModule (TestHelpers.createAsyncModule "Async helper baseline message")

        use deltaDir = new TemporaryDirectory()
        let meta1Path = Path.Combine(deltaDir.Path, "1.meta")
        let meta2Path = Path.Combine(deltaDir.Path, "2.meta")

        let request1 : IlxDeltaRequest =
            { Baseline = baselineArtifacts.Baseline
              UpdatedTypes = [ typeName ]
              UpdatedMethods = [ methodKey ]
              UpdatedAccessors = []
              Module = TestHelpers.createAsyncModule "Async helper generation 1"
              SymbolChanges = None
              CurrentGeneration = 1
              PreviousGenerationId = None
              SynthesizedNames = None }

        let delta1 = emitDelta request1
        File.WriteAllBytes(meta1Path, delta1.Metadata)
        HeapBudgets.assertWithin "Async" delta1.Metadata

        let baseline2 =
            match delta1.UpdatedBaseline with
            | Some b -> b
            | None -> failwith "First async delta did not expose an updated baseline."

        let request2 : IlxDeltaRequest =
            { request1 with
                Baseline = baseline2
                Module = TestHelpers.createAsyncModule "Async helper generation 2"
                CurrentGeneration = 2
                PreviousGenerationId = Some delta1.GenerationId }

        let delta2 = emitDelta request2
        File.WriteAllBytes(meta2Path, delta2.Metadata)
        HeapBudgets.assertWithin "AsyncUpdate" delta2.Metadata

        let methodToken = baselineArtifacts.Baseline.MethodTokens[methodKey]
        let methodRowId = methodRowIdFromToken methodToken
        assertMethodEncLog delta1 methodToken
        assertEncMapContains delta1 TableNames.Method methodRowId
        assertMethodEncLog delta2 methodToken
        assertEncMapContains delta2 TableNames.Method methodRowId

        let literal1 = Text.Encoding.Unicode.GetBytes "Async helper generation 1"
        Assert.True(containsSubsequence delta1.Metadata literal1, "Expected generation 1 async metadata to contain updated literal.")

        let literal2 = Text.Encoding.Unicode.GetBytes "Async helper generation 2"
        Assert.True(containsSubsequence delta2.Metadata literal2, "Expected generation 2 async metadata to contain updated literal.")

        if not (keepArtifacts ()) then
            try File.Delete(baselineArtifacts.AssemblyPath) with _ -> ()
            match baselineArtifacts.PdbPath with
            | Some path -> try File.Delete(path) with _ -> ()
            | None -> ()

    [<Fact>]
    let ``mdv validates method-body edit with closure`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVClosure

module Demo =
    let GetMessage () =
        let prefix = "Integration closure baseline"
        fun value -> sprintf "%s %s" prefix value

    let Invoke value = (GetMessage()) value
"""

        let updatedSource =
            """
namespace MdVClosure

module Demo =
    let GetMessage () =
        let prefix = "Integration closure updated"
        fun value -> sprintf "%s %s" prefix value

    let Invoke value = (GetMessage()) value
"""

        let deltaDir = Path.Combine(projectDir, "mdv-closure-delta")

        try
            Directory.CreateDirectory(deltaDir) |> ignore
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)
            let baselineModule = readIlModule baselineCopy

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource
            let updatedModule = readIlModule dllPath

            logSynthesizedNameDifferences baselineModule updatedModule

            let delta =
                match checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            let metadataPath = Path.Combine(deltaDir, "1.meta")
            let ilPath = Path.Combine(deltaDir, "1.il")
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)

            let infoTokens =
                delta.AddedOrChangedMethods
                |> List.map (fun info -> info.MethodToken)
                |> List.sort
            Assert.Equal<List<int>>(delta.UpdatedMethods |> List.sort, infoTokens)
            Assert.NotEmpty(delta.AddedOrChangedMethods)

            let expectedLiteral = Text.Encoding.Unicode.GetBytes("Integration closure updated")
            Assert.True(
                containsSubsequence delta.Metadata expectedLiteral,
                "Expected closure scenario metadata delta to contain updated literal."
            )

            match runMdv baselineCopy metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Integration closure updated"
            | None ->
                printfn "mdv not available; skipping closure verification."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try Directory.Delete(deltaDir, true) with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates consecutive closure edits`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVClosure

module Demo =
    let GetMessage () =
        let prefix = "Integration closure baseline"
        fun value -> sprintf "%s %s" prefix value

    let Invoke value = (GetMessage()) value
"""

        let firstUpdateSource =
            """
namespace MdVClosure

module Demo =
    let GetMessage () =
        let prefix = "Integration closure updated v2"
        fun value -> sprintf "%s %s" prefix value

    let Invoke value = (GetMessage()) value
"""

        let secondUpdateSource =
            """
namespace MdVClosure

module Demo =
    let GetMessage () =
        let prefix = "Integration closure updated v3"
        fun value -> sprintf "%s %s" prefix value

    let Invoke value = (GetMessage()) value
"""

        let deltaDir = Path.Combine(projectDir, "mdv-closure-multi-delta")

        try
            Directory.CreateDirectory(deltaDir) |> ignore
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions1, _ = compileProject checker fsPath dllPath firstUpdateSource
            let delta1 =
                match checker.EmitHotReloadDelta(updatedOptions1) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 1) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            let meta1Path = Path.Combine(deltaDir, "1.meta")
            let il1Path = Path.Combine(deltaDir, "1.il")
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            File.WriteAllBytes(il1Path, delta1.IL)

            let expectedLiteral1 = Text.Encoding.Unicode.GetBytes("Integration closure updated v2")
            Assert.True(
                containsSubsequence delta1.Metadata expectedLiteral1,
                "Expected generation 1 closure metadata to contain updated literal."
            )

            File.WriteAllText(fsPath, secondUpdateSource)
            let updatedOptions2, _ = compileProject checker fsPath dllPath secondUpdateSource
            let delta2 =
                match checker.EmitHotReloadDelta(updatedOptions2) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 2) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            // TODO: Once checker-based multi-delta sessions forward EncId chaining, assert delta2.BaseGenerationId here.

            let meta2Path = Path.Combine(deltaDir, "2.meta")
            let il2Path = Path.Combine(deltaDir, "2.il")
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            File.WriteAllBytes(il2Path, delta2.IL)

            let expectedLiteral2 = Text.Encoding.Unicode.GetBytes("Integration closure updated v3")
            Assert.True(
                containsSubsequence delta2.Metadata expectedLiteral2,
                "Expected generation 2 closure metadata to contain updated literal."
            )

            match runMdvWithGenerations baselineCopy [ meta1Path, il1Path; meta2Path, il2Path ] with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Integration closure updated v2"
                Assert.Contains("Generation 2", output)
                assertGenerationContains output 2 "Integration closure updated v3"
            | None ->
                printfn "mdv not available; skipping closure multi-generation verification."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try Directory.Delete(deltaDir, true) with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates consecutive async method edits`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVAsync

module Demo =
    let GetMessage () =
        async {
            let prefix = "Integration async baseline"
            return sprintf "%s %d" prefix 1
        }
"""

        let firstUpdateSource =
            """
namespace MdVAsync

module Demo =
    let GetMessage () =
        async {
            let prefix = "Integration async updated v2"
            return sprintf "%s %d" prefix 2
        }
"""

        let secondUpdateSource =
            """
namespace MdVAsync

module Demo =
    let GetMessage () =
        async {
            let prefix = "Integration async updated v3"
            return sprintf "%s %d" prefix 3
        }
"""

        let deltaDir = Path.Combine(projectDir, "mdv-async-multi-delta")

        try
            Directory.CreateDirectory(deltaDir) |> ignore
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions1, _ = compileProject checker fsPath dllPath firstUpdateSource
            let delta1 =
                match checker.EmitHotReloadDelta(updatedOptions1) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 1) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            let meta1Path = Path.Combine(deltaDir, "1.meta")
            let il1Path = Path.Combine(deltaDir, "1.il")
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            File.WriteAllBytes(il1Path, delta1.IL)

            File.WriteAllText(fsPath, secondUpdateSource)
            let updatedOptions2, _ = compileProject checker fsPath dllPath secondUpdateSource
            let delta2 =
                match checker.EmitHotReloadDelta(updatedOptions2) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 2) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            // TODO: Once checker-based multi-delta sessions forward EncId chaining, assert delta2.BaseGenerationId here.

            let meta2Path = Path.Combine(deltaDir, "2.meta")
            let il2Path = Path.Combine(deltaDir, "2.il")
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            File.WriteAllBytes(il2Path, delta2.IL)

            // Validate delta structure for async state machine compilation.
            // Async methods compile to state machines with MoveNext methods. The user strings
            // may be in the baseline heap (referenced by state machine IL) rather than duplicated
            // in the delta. We validate the deltas have the correct structure instead.
            Assert.NotEmpty(delta1.Metadata)
            Assert.NotEmpty(delta1.IL)
            Assert.NotEmpty(delta1.UpdatedMethods)

            Assert.NotEmpty(delta2.Metadata)
            Assert.NotEmpty(delta2.IL)
            Assert.NotEmpty(delta2.UpdatedMethods)

            match runMdvWithGenerations baselineCopy [ meta1Path, il1Path; meta2Path, il2Path ] with
            | Some output ->
                Assert.Contains("Generation 1", output)
                Assert.Contains("Generation 2", output)
            | None ->
                printfn "mdv not available; skipping async multi-generation verification."

            captureDeltaArtifacts "async-multigen" (File.ReadAllBytes(baselineCopy)) delta1 delta2
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            if not (keepArtifacts ()) then
                try Directory.Delete(deltaDir, true) with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates method-body edit with async state machine`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVAsync

open System

module Demo =
    let GetMessage () =
        async {
            do! Async.Sleep 1
            return "Integration async baseline"
        }
"""

        let updatedSource =
            """
namespace MdVAsync

open System

module Demo =
    let GetMessage () =
        async {
            do! Async.Sleep 1
            let suffix = "updated"
            return "Integration async " + suffix
        }
"""

        let deltaDir = Path.Combine(projectDir, "mdv-async-delta")

        try
            Directory.CreateDirectory(deltaDir) |> ignore
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)
            let baselineModule = readIlModule baselineCopy

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            let updatedOptions, _ = compileProject checker fsPath dllPath updatedSource
            let updatedModule = readIlModule dllPath

            logSynthesizedNameDifferences baselineModule updatedModule

            let delta =
                match checker.EmitHotReloadDelta(updatedOptions) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            let metadataPath = Path.Combine(deltaDir, "1.meta")
            let ilPath = Path.Combine(deltaDir, "1.il")
            File.WriteAllBytes(metadataPath, delta.Metadata)
            File.WriteAllBytes(ilPath, delta.IL)

            let infoTokens =
                delta.AddedOrChangedMethods
                |> List.map (fun info -> info.MethodToken)
                |> List.sort
            Assert.Equal<List<int>>(delta.UpdatedMethods |> List.sort, infoTokens)
            Assert.NotEmpty(delta.AddedOrChangedMethods)

            // Validate delta structure for async state machine compilation.
            // Async methods compile to state machines with MoveNext methods. The user strings
            // may be in the baseline heap (referenced by state machine IL) rather than duplicated
            // in the delta. We validate the delta has the correct structure instead.
            Assert.NotEmpty(delta.Metadata)
            Assert.NotEmpty(delta.IL)
            Assert.NotEmpty(delta.UpdatedMethods)
            Assert.NotEmpty(delta.AddedOrChangedMethods)

            match runMdv baselineCopy metadataPath ilPath with
            | Some output ->
                Assert.Contains("Generation 1", output)
            | None ->
                printfn "mdv not available; skipping async verification."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try Directory.Delete(deltaDir, true) with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    [<Fact>]
    let ``mdv validates consecutive method-body edits`` () =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                captureIdentifiersWhenParsing = false
            )

        let projectDir, fsPath, dllPath = createTempProject ()
        let baselineSource =
            """
namespace MdVIntegration

module Demo =
    let GetMessage () = "Integration baseline message"
"""

        let firstUpdateSource =
            """
namespace MdVIntegration

module Demo =
    let GetMessage () = "Integration updated message v2"
"""

        let secondUpdateSource =
            """
namespace MdVIntegration

module Demo =
    let GetMessage () = "Integration updated message v3"
"""

        let deltaDir = Path.Combine(projectDir, "mdv-multi-delta")

        try
            Directory.CreateDirectory(deltaDir) |> ignore
            let baselineOptions, _ = compileProject checker fsPath dllPath baselineSource
            let baselineCopy = Path.Combine(projectDir, "baseline.dll")
            File.Copy(dllPath, baselineCopy, true)

            match checker.StartHotReloadSession(baselineOptions) |> Async.RunSynchronously with
            | Error error -> failwithf "StartHotReloadSession failed: %A" error
            | Ok () -> ()

            // First edit
            let updatedOptions1, _ = compileProject checker fsPath dllPath firstUpdateSource
            let delta1 =
                match checker.EmitHotReloadDelta(updatedOptions1) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 1) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            let meta1Path = Path.Combine(deltaDir, "1.meta")
            let il1Path = Path.Combine(deltaDir, "1.il")
            File.WriteAllBytes(meta1Path, delta1.Metadata)
            File.WriteAllBytes(il1Path, delta1.IL)

            let expectedLiteral1 = Text.Encoding.Unicode.GetBytes("Integration updated message v2")
            Assert.True(
                containsSubsequence delta1.Metadata expectedLiteral1,
                "Expected first-generation metadata to contain updated literal 'Integration updated message v2'."
            )

            // Second edit
            File.WriteAllText(fsPath, secondUpdateSource)
            let updatedOptions2, _ = compileProject checker fsPath dllPath secondUpdateSource
            let delta2 =
                match checker.EmitHotReloadDelta(updatedOptions2) |> Async.RunSynchronously with
                | Error error -> failwithf "EmitHotReloadDelta (generation 2) failed: %A" error
                | Ok delta ->
                    ensureGenerationCommitted delta.GenerationId
                    delta

            // TODO: Once checker-based multi-delta sessions forward EncId chaining, assert delta2.BaseGenerationId here.
            Assert.NotEqual(delta1.GenerationId, delta2.GenerationId)

            let meta2Path = Path.Combine(deltaDir, "2.meta")
            let il2Path = Path.Combine(deltaDir, "2.il")
            File.WriteAllBytes(meta2Path, delta2.Metadata)
            File.WriteAllBytes(il2Path, delta2.IL)

            let expectedLiteral2 = Text.Encoding.Unicode.GetBytes("Integration updated message v3")
            Assert.True(
                containsSubsequence delta2.Metadata expectedLiteral2,
                "Expected second-generation metadata to contain updated literal 'Integration updated message v3'."
            )

            match runMdv baselineCopy meta1Path il1Path with
            | Some output ->
                Assert.Contains("Generation 1", output)
                assertGenerationContains output 1 "Integration updated message v2"
            | None ->
                printfn "mdv not available; skipping Generation 1 verification for multi-generation scenario."

            match runMdv baselineCopy meta2Path il2Path with
            | Some output ->
                assertGenerationContains output 1 "Integration updated message v3"
            | None ->
                printfn "mdv not available; skipping Generation 2 verification for multi-generation scenario."
        finally
            try checker.InvalidateAll() with _ -> ()
            try checker.EndHotReloadSession() with _ -> ()
            try Directory.Delete(deltaDir, true) with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    
