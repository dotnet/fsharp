// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Security.Cryptography
open System.Threading
open Internal.Utilities.Collections
open Internal.Utilities
open Internal.Utilities.Library
open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.ILDynamicAssemblyWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.Caches
open FSharp.Compiler.CheckExpressionsOps
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.TransparentCompiler
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.CreateILModule
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Driver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IlxGen
open FSharp.Compiler.OptimizeInputs
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.HotReload.DeltaBuilder
open FSharp.Compiler.IlxDeltaEmitter
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.EnvironmentHelpers

/// Callback that indicates whether a requested result has become obsolete.
[<NoComparison; NoEquality>]
type IsResultObsolete = IsResultObsolete of (unit -> bool)

module CompileHelpers =
    let mkCompilationDiagnosticsHandlers flatErrors =
        let diagnostics = ResizeArray<_>()

        let diagnosticsLogger =
            { new DiagnosticsLogger("CompileAPI") with

                member _.DiagnosticSink(diagnostic) =
                    diagnostics.Add(FSharpDiagnostic.CreateFromException(diagnostic, true, flatErrors, None)) // Suggest names for errors

                member _.ErrorCount =
                    diagnostics
                    |> Seq.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)
                    |> Seq.length
            }

        let loggerProvider =
            { new IDiagnosticsLoggerProvider with
                member _.CreateLogger(_tcConfigB, _exiter) = diagnosticsLogger
            }

        diagnostics, diagnosticsLogger, loggerProvider

    let tryCompile diagnosticsLogger f =
        use _ = UseBuildPhase BuildPhase.Parse
        use _ = UseDiagnosticsLogger diagnosticsLogger

        let exiter = StopProcessingExiter()

        try
            f exiter
            None
        with e ->
            stopProcessingRecovery e range0
            Some e

    /// Compile using the given flags.  Source files names are resolved via the FileSystem API. The output file must be given by a -o flag.
    let compileFromArgs (ctok, argv: string[], legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator) =

        let diagnostics, diagnosticsLogger, loggerProvider =
            mkCompilationDiagnosticsHandlers (argv |> Array.contains "--flaterrors")

        let result =
            tryCompile diagnosticsLogger (fun exiter ->
                CompileFromCommandLineArguments(
                    ctok,
                    argv,
                    legacyReferenceResolver,
                    true,
                    ReduceMemoryFlag.Yes,
                    CopyFSharpCoreFlag.No,
                    exiter,
                    loggerProvider,
                    tcImportsCapture,
                    dynamicAssemblyCreator
                ))

        diagnostics.ToArray(), result

module internal HotReloadIncrementalEmit =
    /// Stable inputs that must match before an optimized prefix can be reused.
    [<NoEquality; NoComparison>]
    type OptimizationContext =
        {
            TcGlobals: obj
            TcImports: obj
            Settings: Optimizer.OptimizationSettings
            EmitTailcalls: bool
        }

    [<NoEquality; NoComparison>]
    type private CachedStep =
        {
            Input: CheckedImplFile
            Output: CheckedImplFileAfterOptimization
            Environment: Optimizer.IncrementalOptimizationEnv
            HidingInfo: SignatureHidingInfo
        }

    [<NoEquality; NoComparison>]
    type private CacheEntry =
        {
            Context: OptimizationContext
            Steps: CachedStep array
        }

    let private sameContext left right =
        // The CcuSignature is deliberately not part of this context: an unchanged prefix cannot
        // depend on a changed later file, and a changed prefix file already stops reuse by identity.
        obj.ReferenceEquals(left.TcGlobals, right.TcGlobals)
        && obj.ReferenceEquals(left.TcImports, right.TcImports)
        && left.Settings = right.Settings
        && left.EmitTailcalls = right.EmitTailcalls

    /// Caches a sequential optimizer prefix for each output path. Reuse stops at the first
    /// changed CheckedImplFile, then the changed file and complete tail are recomputed while
    /// threading the real optimizer environment and signature-hiding state.
    type Cache() =
        let gate = obj ()

        let pathComparer =
            if Path.DirectorySeparatorChar = '\\' then
                StringComparer.OrdinalIgnoreCase
            else
                StringComparer.Ordinal

        let entries = Dictionary<string, CacheEntry>(pathComparer)

        /// Reuses the longest unchanged prefix and recomputes the remaining files sequentially.
        member _.Optimize(outputPath, context, initialEnvironment, inputs, optimize) =
            lock gate (fun () ->
                let inputs = List.toArray inputs

                let reusablePrefixLength, cachedSteps =
                    match entries.TryGetValue outputPath with
                    | true, cached when sameContext cached.Context context ->
                        let maxPrefixLength = min inputs.Length cached.Steps.Length
                        let mutable prefixLength = 0

                        while prefixLength < maxPrefixLength
                              && obj.ReferenceEquals(inputs[prefixLength], cached.Steps[prefixLength].Input) do
                            prefixLength <- prefixLength + 1

                        prefixLength, cached.Steps
                    | _ -> 0, Array.empty

                let updatedSteps = Array.zeroCreate<CachedStep> inputs.Length

                if reusablePrefixLength > 0 then
                    Array.Copy(cachedSteps, updatedSteps, reusablePrefixLength)

                let mutable environment, hidingInfo =
                    if reusablePrefixLength = 0 then
                        initialEnvironment, SignatureHidingInfo.Empty
                    else
                        let lastReusedStep = cachedSteps[reusablePrefixLength - 1]
                        lastReusedStep.Environment, lastReusedStep.HidingInfo

                for index in reusablePrefixLength .. inputs.Length - 1 do
                    let output, nextEnvironment, nextHidingInfo = optimize environment hidingInfo inputs[index]

                    updatedSteps[index] <-
                        {
                            Input = inputs[index]
                            Output = output
                            Environment = nextEnvironment
                            HidingInfo = nextHidingInfo
                        }

                    environment <- nextEnvironment
                    hidingInfo <- nextHidingInfo

                // Publish only after the whole tail succeeds. A failed incremental attempt leaves
                // the last known-good prefix available and the caller can safely retry in full.
                entries[outputPath] <-
                    {
                        Context = context
                        Steps = updatedSteps
                    }

                if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_TIMING") = "1" then
                    printfn "[fsharp-hotreload][incremental-emit] reused-prefix=%d total=%d" reusablePrefixLength inputs.Length

                updatedSteps |> Array.map _.Output |> Array.toList)

    /// Runs the incremental path and falls back to the full threaded optimizer if it fails.
    let runWithFallback incremental fallback =
        try
            incremental ()
        with
        | :? OperationCanceledException
        | :? OutOfMemoryException
        | :? StackOverflowException
        | :? AccessViolationException -> reraise ()
        | _ -> fallback ()

[<Sealed; AutoSerializable(false)>]
// There is typically only one instance of this type in an IDE process.
type FSharpChecker
    (
        legacyReferenceResolver,
        projectCacheSize,
        keepAssemblyContents,
        keepAllBackgroundResolutions,
        tryGetMetadataSnapshot,
        suggestNamesForErrors,
        keepAllBackgroundSymbolUses,
        enableBackgroundItemKeyStoreAndSemanticClassification,
        enablePartialTypeChecking,
        parallelReferenceResolution,
        captureIdentifiersWhenParsing,
        getSource,
        useChangeNotifications,
        useTransparentCompiler,
        ?transparentCompilerCacheSizes
    ) =

    let backgroundCompiler =
        if useTransparentCompiler = Some true then
            TransparentCompiler(
                legacyReferenceResolver,
                projectCacheSize,
                keepAssemblyContents,
                keepAllBackgroundResolutions,
                tryGetMetadataSnapshot,
                suggestNamesForErrors,
                keepAllBackgroundSymbolUses,
                enableBackgroundItemKeyStoreAndSemanticClassification,
                enablePartialTypeChecking,
                parallelReferenceResolution,
                captureIdentifiersWhenParsing,
                getSource,
                useChangeNotifications,
                ?cacheSizes = transparentCompilerCacheSizes
            )
            :> IBackgroundCompiler

        else
            BackgroundCompiler(
                legacyReferenceResolver,
                projectCacheSize,
                keepAssemblyContents,
                keepAllBackgroundResolutions,
                tryGetMetadataSnapshot,
                suggestNamesForErrors,
                keepAllBackgroundSymbolUses,
                enableBackgroundItemKeyStoreAndSemanticClassification,
                enablePartialTypeChecking,
                parallelReferenceResolution,
                captureIdentifiersWhenParsing,
                getSource,
                useChangeNotifications
            )
            :> IBackgroundCompiler

    let hotReloadIncrementalEmitCache = lazy HotReloadIncrementalEmit.Cache()

    static let globalInstance = lazy FSharpChecker.Create()

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.braceMatchCache. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    //
    // This cache is safe for concurrent access.
    let braceMatchCache =
        MruCache<AnyCallerThreadToken, _, _>(braceMatchCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing)

    static let inferParallelReferenceResolution (parallelReferenceResolution: bool option) =
        let explicitValue =
            parallelReferenceResolution
            |> Option.defaultValue false
            |> function
                | true -> ParallelReferenceResolution.On
                | false -> ParallelReferenceResolution.Off

        let withEnvOverride =
            // Override ParallelReferenceResolution set on the constructor with an environment setting if present.
            getParallelReferenceResolutionFromEnvironment ()
            |> Option.defaultValue explicitValue

        withEnvOverride

    let ensureKeepAssemblyContents () =
        if not keepAssemblyContents then
            invalidOp
                "Hot reload APIs require the checker to be created with keepAssemblyContents=true. Pass keepAssemblyContents=true when calling FSharpChecker.Create."

    let trimQuotes (text: string) = text.Trim().Trim('"')

    let tryGetOutputPathFromCommandLineOptions (projectFileName: string) (otherOptions: string array) =
        let projectDirectory =
            let resolveDirectory (path: string) =
                if String.IsNullOrWhiteSpace(path) then
                    Directory.GetCurrentDirectory()
                else
                    let absolute =
                        if Path.IsPathRooted(path) then
                            path
                        else
                            Path.GetFullPath(path)

                    match Path.GetDirectoryName(absolute) with
                    | null
                    | "" -> Directory.GetCurrentDirectory()
                    | value -> value

            match projectFileName with
            | "" -> Directory.GetCurrentDirectory()
            | fileName -> resolveDirectory fileName

        let resolveOutputPath (path: string) =
            let trimmed = trimQuotes path

            if Path.IsPathRooted(trimmed) then
                Path.GetFullPath(trimmed)
            else
                let baseDirectory =
                    if String.IsNullOrWhiteSpace(projectDirectory) then
                        Directory.GetCurrentDirectory()
                    else
                        projectDirectory

                let combined =
                    if String.IsNullOrWhiteSpace(trimmed) then
                        baseDirectory
                    else
                        Path.Combine(baseDirectory, trimmed)

                Path.GetFullPath(combined)

        let tryFromInlineForm =
            otherOptions
            |> Array.tryPick (fun opt ->
                if opt.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) then
                    opt.Substring("--out:".Length) |> resolveOutputPath |> Some
                elif opt.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) then
                    opt.Substring("-o:".Length) |> resolveOutputPath |> Some
                else
                    None)

        match tryFromInlineForm with
        | Some path -> Some path
        | None ->
            match
                otherOptions
                |> Array.tryFindIndex (fun opt ->
                    String.Equals(opt, "-o", StringComparison.OrdinalIgnoreCase)
                    || String.Equals(opt, "--out", StringComparison.OrdinalIgnoreCase))
            with
            | Some idx when idx + 1 < otherOptions.Length -> otherOptions[idx + 1] |> resolveOutputPath |> Some
            | _ -> None

    let tryGetOutputPathFromProjectSnapshot (projectSnapshot: FSharpProjectSnapshot) =
        tryGetOutputPathFromCommandLineOptions projectSnapshot.ProjectFileName (projectSnapshot.OtherOptions |> List.toArray)

    [<Literal>]
    let HotReloadTraceOutputFlagName = "FSHARP_HOTRELOAD_TRACE_OUTPUT"

    let traceOutputFingerprint = isEnvVarTruthy HotReloadTraceOutputFlagName

    let getErrorDiagnostics (diagnostics: FSharpDiagnostic[]) =
        diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

    let waitForStableFile = FSharpHotReloadFileSystem.waitForStableFile

    let computeFileHash (path: string) : byte[] option =
        if File.Exists path then
            try
                use stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                use sha = System.Security.Cryptography.SHA256.Create()
                Some(sha.ComputeHash stream)
            with _ ->
                None
        else
            None

    let tryGetOutputFingerprint (path: string) =
        if File.Exists path then
            let timestamp = File.GetLastWriteTimeUtc path
            let hash = computeFileHash path
            Some(timestamp, hash)
        else
            None

    let hasOutputFingerprintChanged path previous current =
        match previous, current with
        | Some(previousTimestamp, previousHash), Some(currentTimestamp, currentHash) ->
            let timestampChanged = previousTimestamp <> currentTimestamp

            // Prefer content identity whenever both reads produced hashes. Rebuilding or merely
            // touching an unchanged output must not make a stale semantic edit look current.
            let hashChanged =
                match previousHash, currentHash with
                | Some previousBytes, Some currentBytes ->
                    Some(not (StructuralComparisons.StructuralEqualityComparer.Equals(previousBytes, currentBytes)))
                | _ -> None

            let changed = defaultArg hashChanged timestampChanged

            if traceOutputFingerprint && timestampChanged then
                printfn $"[fsharp-hotreload][trace] detected write timestamp change for {path} (prev={previousTimestamp:O}, new={currentTimestamp:O})"

            if traceOutputFingerprint && hashChanged = Some true then
                printfn $"[fsharp-hotreload][trace] detected content hash change for {path}"

            changed
        | None, Some _ -> true
        | Some _, None -> true
        | None, None -> false

    let readIlModule path =
        waitForStableFile path

        let options: ILReaderOptions =
            {
                pdbDirPath = None
                reduceMemoryUsage = ReduceMemoryFlag.Yes
                metadataOnly = MetadataOnlyFlag.No
                tryGetMetadataSnapshot = fun _ -> None
            }

        use reader = OpenILModuleReader path options
        reader.ILModuleDef

    let toPublicDelta (delta: IlxDelta) : FSharpHotReloadDelta =
        {
            Metadata = Array.copy delta.Metadata
            IL = Array.copy delta.IL
            Pdb = delta.Pdb |> Option.map Array.copy
            UpdatedTypes = delta.UpdatedTypeTokens
            UpdatedMethods = delta.UpdatedMethodTokens
            RequiredCapabilities = delta.RequiredCapabilities
            AddedOrChangedMethods =
                delta.AddedOrChangedMethods
                |> List.map (fun info ->
                    {
                        MethodToken = info.MethodToken
                        LocalSignatureToken = info.LocalSignatureToken
                        CodeOffset = info.CodeOffset
                        CodeLength = info.CodeLength
                    })
            UserStringUpdates = delta.UserStringUpdates |> List.map (fun (o, n, s) -> struct (o, n, s))
            GenerationId = delta.GenerationId
            BaseGenerationId = delta.BaseGenerationId
            SequencePointUpdates = delta.SequencePointUpdates
            ActiveStatementUpdates = delta.ActiveStatementUpdates
        }

    let mapHotReloadError =
        function
        | HotReloadError.NoActiveSession -> FSharpHotReloadError.NoActiveSession
        | HotReloadError.NoChanges -> FSharpHotReloadError.NoChanges
        | HotReloadError.UnsupportedEdit diagnostics -> FSharpHotReloadError.UnsupportedEdit(FSharpHotReloadRudeEditMapping.ofDiagnostics diagnostics)
        | HotReloadError.DeltaEmissionException ex -> FSharpHotReloadError.DeltaEmissionFailed ex.Message

    let createBaseline (tcGlobals: TcGlobals) (ilModule: ILModuleDef) (outputPath: string) =
        let pdbPath =
            Path.ChangeExtension(outputPath, ".pdb")
            |> Option.ofObj
            |> Option.defaultValue (outputPath + ".pdb")

        let writerOptions: ILBinaryWriter.options =
            {
                ilg = tcGlobals.ilg
                outfile = outputPath
                pdbfile = if File.Exists(pdbPath) then Some pdbPath else None
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
                pathMap = PathMap.empty
                moduleCustomDebugInfoRows = []
                methodCustomDebugInfoRows = Map.empty
            }

        let _, pdbBytesOpt, tokenMappings, _ =
            ILBinaryWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, ilModule, id)

        let assemblyBytes = File.ReadAllBytes(outputPath)

        let siblingPdbBytes =
            if File.Exists(pdbPath) then
                let bytes = File.ReadAllBytes(pdbPath)

                if HotReloadPdb.matchesAssembly assemblyBytes bytes then
                    Some bytes
                else
                    None
            else
                None

        let portablePdbSnapshot =
            pdbBytesOpt
            |> Option.bind (HotReloadPdb.tryCreateSnapshotForAssembly assemblyBytes)
            |> Option.orElseWith (fun () -> siblingPdbBytes |> Option.map HotReloadPdb.createSnapshot)

        let baseline =
            HotReloadBaseline.createFromEmittedArtifacts ilModule tokenMappings assemblyBytes portablePdbSnapshot None

        // The in-memory rewrite above passes no hot reload CDI side channel, so its PDB
        // never carries EnC rows or the F# synthesized-name snapshot. The on-disk PDB
        // produced by the flag-on build is the durable source: read it as a sibling input
        // when the rewrite yielded none. Recorded synthesized-name snapshots take
        // precedence over IL reconstruction; absent records preserve the old fallback.
        let baseline =
            if
                baseline.SynthesizedNameSnapshotSource = SynthesizedNameSnapshotSource.Reconstructed
                && siblingPdbBytes.IsSome
            then
                match FSharp.Compiler.EncMethodDebugInformation.readSynthesizedNameSnapshotFromPortablePdb siblingPdbBytes.Value with
                | Some recordedSnapshot ->
                    if isEnvVarTruthy "FSHARP_HOTRELOAD_TRACE_CLOSURENAMES" then
                        printfn "[fsharp-hotreload][closure-names] synthesized-name snapshot source=recorded buckets=%d" (Map.count recordedSnapshot)

                    { baseline with
                        SynthesizedNameSnapshot = recordedSnapshot
                        SynthesizedNameSnapshotSource = SynthesizedNameSnapshotSource.Recorded
                    }
                | None ->
                    if isEnvVarTruthy "FSHARP_HOTRELOAD_TRACE_CLOSURENAMES" then
                        printfn
                            "[fsharp-hotreload][closure-names] synthesized-name snapshot source=reconstructed buckets=%d"
                            (Map.count baseline.SynthesizedNameSnapshot)

                    baseline
            else
                baseline

        // The in-memory rewrite above passes no EnC CDI side channel (methodCustomDebugInfoRows =
        // Map.empty), so its PDB never carries EnC rows. The on-disk PDB produced by the flag-on
        // build is the durable source of the baseline EnC method debug information: read it as a
        // sibling input when the rewrite yielded none (flag-off PDBs and PDBs without EnC rows still decode to the
        // empty map, and the session starts fine either way).
        let baseline =
            if Map.isEmpty baseline.EncMethodDebugInfos && siblingPdbBytes.IsSome then
                let baseline =
                    { baseline with
                        EncMethodDebugInfos = FSharp.Compiler.EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb siblingPdbBytes.Value
                    }

                // Closure mapping: the chain -> closure-name tables are a pure function
                // of the occurrence keys just decoded (baseline names are occurrence-derived
                // under the flag), so a session started from disk — typically in a different
                // process than the fsc that built the baseline — reconstructs exactly the
                // tables the emitting compile installed. Fail closed for replay-named and
                // mid-session baselines (see deriveEncClosureNamesFromEncDebugInfos).
                { baseline with
                    EncClosureNames = HotReloadBaseline.deriveEncClosureNames ilModule baseline
                }
            else
                baseline

        // Sequence-point sibling-read: the IL module is read back from disk WITHOUT debug points, so
        // the in-memory rewrite's PDB decodes to an empty sequence-point view. The on-disk PDB
        // written by the build is the real source of the committed lines that line-shift
        // detection and active-statement remapping diff against.
        if Map.isEmpty baseline.SequencePointSnapshots && siblingPdbBytes.IsSome then
            { baseline with
                SequencePointSnapshots = FSharp.Compiler.HotReload.ActiveStatementAnalysis.decodeMethodSequencePoints siblingPdbBytes.Value
            }
        else
            baseline

    let toHotReloadImplementationSnapshot (typedImplFiles: CheckedImplFile list) : CheckedAssemblyAfterOptimization =
        typedImplFiles
        |> List.map (fun implFile ->
            {
                CheckedImplFileAfterOptimization.ImplFile = implFile
                OptimizeDuringCodeGen = fun _ expr -> expr
            })
        |> CheckedAssemblyAfterOptimization

    let getHotReloadDiffInputs (projectResults: FSharpCheckProjectResults) =
        // Use non-optimized typed implementation trees for symbol diffing so method-body edits
        // keep user-authored identities (Roslyn parity), while IL deltas still come from built output.
        let tcGlobals, _thisCcu, _tcImports, typedImplFiles = projectResults.TypedImplementationFiles
        tcGlobals, toHotReloadImplementationSnapshot typedImplFiles

    let createHotReloadService sessionStore =
        FSharpHotReloadService(
            sessionStore,
            readIlModule,
            createBaseline,
            getHotReloadDiffInputs,
            getErrorDiagnostics,
            waitForStableFile,
            tryGetOutputFingerprint,
            hasOutputFingerprintChanged,
            toPublicDelta,
            mapHotReloadError
        )

    // Projects tracked by LIVE session entities created via CreateHotReloadSession, keyed by
    // the resolved output path each AddProject baselined (most recent first). Compile consults
    // this to resolve the scoped emission context — which session, and which project inside
    // it, a given in-process compile serves. Disposing a session removes its entries.
    let liveHotReloadEmissionTargets =
        ResizeArray<string * FSharp.Compiler.HotReloadState.HotReloadSessionStore * FSharp.Compiler.HotReloadState.HotReloadProjectKey>()

    let liveHotReloadEmissionTargetsGate = obj ()

    let normalizeOutputPathForEmissionTargets (path: string) =
        try
            Path.GetFullPath(path)
        with _ ->
            path

    let outputPathComparison =
        if Path.DirectorySeparatorChar = '\\' then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal

    let registerHotReloadEmissionTarget
        (store: FSharp.Compiler.HotReloadState.HotReloadSessionStore)
        (outputPath: string)
        (projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey)
        =
        let normalized = normalizeOutputPathForEmissionTargets outputPath

        lock liveHotReloadEmissionTargetsGate (fun () ->
            // A recapture of the same project in the same session replaces its entry.
            liveHotReloadEmissionTargets.RemoveAll(fun (_, existingStore, existingKey) ->
                obj.ReferenceEquals(existingStore, store) && existingKey = projectKey)
            |> ignore

            liveHotReloadEmissionTargets.Insert(0, (normalized, store, projectKey)))

    let unregisterHotReloadEmissionTargets (store: FSharp.Compiler.HotReloadState.HotReloadSessionStore) =
        lock liveHotReloadEmissionTargetsGate (fun () ->
            liveHotReloadEmissionTargets.RemoveAll(fun (_, existingStore, _) -> obj.ReferenceEquals(existingStore, store))
            |> ignore)

    // Resolves the session entity (and tracked project) an in-process compile belongs to by
    // the compile's output path. The most recently baselined project wins when several live
    // sessions track the same output.
    let tryResolveHotReloadEmissionContext (outputPath: string option) =
        match outputPath with
        | None -> None
        | Some outputPath ->
            let target = normalizeOutputPathForEmissionTargets outputPath

            lock liveHotReloadEmissionTargetsGate (fun () ->
                liveHotReloadEmissionTargets
                |> Seq.tryPick (fun (registeredPath, store, projectKey) ->
                    if String.Equals(registeredPath, target, outputPathComparison) then
                        Some(
                            {
                                FSharp.Compiler.HotReloadState.HotReloadEmissionContext.Store = store
                                FSharp.Compiler.HotReloadState.HotReloadEmissionContext.ProjectKey = projectKey
                            }
                        )
                    else
                        None))

    static member getParallelReferenceResolutionFromEnvironment() =
        getParallelReferenceResolutionFromEnvironment ()

    /// Instantiate an interactive checker.
    static member Create
        (
            ?projectCacheSize,
            ?keepAssemblyContents,
            ?keepAllBackgroundResolutions,
            ?legacyReferenceResolver,
            ?tryGetMetadataSnapshot,
            ?suggestNamesForErrors,
            ?keepAllBackgroundSymbolUses,
            ?enableBackgroundItemKeyStoreAndSemanticClassification,
            ?enablePartialTypeChecking,
            ?parallelReferenceResolution: bool,
            ?captureIdentifiersWhenParsing: bool,
            ?documentSource: DocumentSource,
            ?useTransparentCompiler: bool,
            ?transparentCompilerCacheSizes: CacheSizes
        ) =

        use _ = Activity.startNoTags "FSharpChecker.Create"

        let legacyReferenceResolver =
            match legacyReferenceResolver with
            | Some rr -> rr
            | None -> SimulatedMSBuildReferenceResolver.getResolver ()

        let keepAssemblyContents = defaultArg keepAssemblyContents false
        let keepAllBackgroundResolutions = defaultArg keepAllBackgroundResolutions true
        let projectCacheSizeReal = defaultArg projectCacheSize projectCacheSizeDefault
        let tryGetMetadataSnapshot = defaultArg tryGetMetadataSnapshot (fun _ -> None)
        let suggestNamesForErrors = defaultArg suggestNamesForErrors false
        let keepAllBackgroundSymbolUses = defaultArg keepAllBackgroundSymbolUses true

        let enableBackgroundItemKeyStoreAndSemanticClassification =
            defaultArg enableBackgroundItemKeyStoreAndSemanticClassification false

        let enablePartialTypeChecking = defaultArg enablePartialTypeChecking false
        let captureIdentifiersWhenParsing = defaultArg captureIdentifiersWhenParsing false

        let useChangeNotifications =
            match documentSource with
            | Some(DocumentSource.Custom _) -> true
            | _ -> false

        if keepAssemblyContents && enablePartialTypeChecking then
            invalidArg "enablePartialTypeChecking" "'keepAssemblyContents' and 'enablePartialTypeChecking' cannot be both enabled."

        let parallelReferenceResolution = inferParallelReferenceResolution parallelReferenceResolution

        FSharpChecker(
            legacyReferenceResolver,
            projectCacheSizeReal,
            keepAssemblyContents,
            keepAllBackgroundResolutions,
            tryGetMetadataSnapshot,
            suggestNamesForErrors,
            keepAllBackgroundSymbolUses,
            enableBackgroundItemKeyStoreAndSemanticClassification,
            enablePartialTypeChecking,
            parallelReferenceResolution,
            captureIdentifiersWhenParsing,
            (match documentSource with
             | Some(DocumentSource.Custom f) -> Some f
             | _ -> None),
            useChangeNotifications,
            useTransparentCompiler,
            ?transparentCompilerCacheSizes = transparentCompilerCacheSizes
        )

    // Runtime capability strings cross the public boundary once and are parsed into the typed
    // model here; everything downstream consults EditAndContinueCapabilities only.
    static member private ParseHotReloadCapabilities(capabilities: string seq option) =
        capabilities
        |> Option.map EditAndContinueCapabilities.Parse
        |> Option.defaultValue EditAndContinueCapabilities.BaselineOnly

    member this.CreateHotReloadSession(?capabilities: string seq) =
        ensureKeepAssemblyContents ()
        use _ = Activity.startNoTags "FSharpChecker.CreateHotReloadSession"

        // The session owns a private store instance that is NEVER registered as the
        // process-wide store: it is fully independent of the checker's default session
        // and of any other session created from this (or another) checker.
        let sessionStore = FSharp.Compiler.HotReloadState.createSessionStore ()
        let sessionService = createHotReloadService sessionStore
        sessionService.SetSessionCapabilities(FSharpChecker.ParseHotReloadCapabilities capabilities)

        new FSharpHotReloadSession(
            sessionService,
            (fun projectSnapshot opName -> this.ParseAndCheckProject(projectSnapshot, userOpName = opName)),
            tryGetOutputPathFromProjectSnapshot,
            (fun outputPath projectKey -> registerHotReloadEmissionTarget sessionStore outputPath projectKey),
            (fun () -> unregisterHotReloadEmissionTargets sessionStore),
            Some(fun (results, outfile, naming) -> this.CompileFromCheckedProject(results, outfile, naming))
        )

    member _.HotReloadCapabilities =
        let capabilities = HotReloadCapability.current
        FSharpHotReloadCapabilities.FromInternalFlags(capabilities.Flags)

    member _.UsesTransparentCompiler = useTransparentCompiler = Some true

    member _.TransparentCompiler =
        match useTransparentCompiler with
        | Some true -> backgroundCompiler :?> TransparentCompiler
        | _ -> failwith "Transparent Compiler is not enabled."

    member this.Caches = this.TransparentCompiler.Caches

    member _.ReferenceResolver = legacyReferenceResolver

    member _.MatchBraces(fileName, sourceText: ISourceText, options: FSharpParsingOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        use _ =
            Activity.start "FSharpChecker.MatchBraces" [| Activity.Tags.fileName, fileName; Activity.Tags.userOpName, userOpName |]

        let hash = sourceText.GetHashCode() |> int64

        async {
            match braceMatchCache.TryGet(AnyCallerThread, (fileName, hash, options)) with
            | Some res -> return res
            | None ->
                let! ct = Async.CancellationToken

                let res =
                    ParseAndCheckFile.matchBraces (sourceText, fileName, options, userOpName, suggestNamesForErrors, ct)

                braceMatchCache.Set(AnyCallerThread, (fileName, hash, options), res)
                return res
        }

    member ic.MatchBraces(fileName, source: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.MatchBraces(fileName, SourceText.ofString source, parsingOptions, userOpName)

    member ic.GetParsingOptionsFromProjectOptions(options) : FSharpParsingOptions * _ =
        let sourceFiles = List.ofArray options.SourceFiles
        let argv = List.ofArray options.OtherOptions
        ic.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, options.UseScriptResolutionRules)

    member _.ParseFile(fileName, sourceText, options, ?cache, ?userOpName: string) =
        let cache = defaultArg cache true
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ParseFile(fileName, sourceText, options, cache, false, userOpName)

    member _.ParseFile(fileName, projectSnapshot, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseFile(fileName, projectSnapshot, userOpName)

    member ic.ParseFileInProject(fileName, source: string, options, ?cache: bool, ?userOpName: string) =
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.ParseFile(fileName, SourceText.ofString source, parsingOptions, ?cache = cache, ?userOpName = userOpName)

    member _.GetBackgroundParseResultsForFileInProject(fileName, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

    member _.GetBackgroundCheckResultsForFileInProject(fileName, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetBackgroundCheckResultsForFileInProject(fileName, options, userOpName)

    /// Try to get recent approximate type check results for a file.
    member _.TryGetRecentCheckResultsForFile(fileName: string, options: FSharpProjectOptions, ?sourceText, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, options, sourceText, userOpName)

    member _.TryGetRecentCheckResultsForFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(fileName, projectSnapshot, userOpName)

    member _.Compile(argv: string[], ?userOpName: string) =
        let _userOpName = defaultArg userOpName "Unknown"
        use _ = Activity.start "FSharpChecker.Compile" [| Activity.Tags.userOpName, _userOpName |]

        let hasTestArgument (feature: string) (argv: string[]) =
            let inlineEnabled =
                argv
                |> Array.exists (fun arg ->
                    arg.StartsWith("--test:", StringComparison.OrdinalIgnoreCase)
                    && arg.Substring("--test:".Length).Equals(feature, StringComparison.OrdinalIgnoreCase))

            let splitEnabled =
                argv
                |> Array.pairwise
                |> Array.exists (fun (arg, value) ->
                    arg.Equals("--test", StringComparison.OrdinalIgnoreCase)
                    && value.Equals(feature, StringComparison.OrdinalIgnoreCase))

            inlineEnabled || splitEnabled

        // A non-capture compile of a project tracked by a live session entity is scoped to
        // that session: the emit hook then replays the session's chained closure-name and
        // synthesized-name state into this compile. Capture compiles stay session-independent
        // (they publish to the process-local capture slot, never to a session's store).
        let emissionContext =
            if hasTestArgument "HotReloadDeltas" argv then
                None
            else
                tryResolveHotReloadEmissionContext (tryGetOutputPathFromCommandLineOptions "" argv)

        let ensureHotReloadSessionHookArgument (argv: string[]) =
            // Keep synthesized-name replay active for checker-owned hot reload sessions even when
            // callers intentionally compile updates without --test:HotReloadDeltas.
            if
                emissionContext.IsSome
                && not (hasTestArgument "HotReloadDeltas" argv)
                && not (hasTestArgument "HotReloadHook" argv)
            then
                Array.append argv [| "--test:HotReloadHook" |]
            else
                argv

        let argv = ensureHotReloadSessionHookArgument argv

        async {
            let ctok = CompilationThreadToken()

            match emissionContext with
            | Some context ->
                let previousContext = FSharp.Compiler.HotReloadState.tryGetCurrentEmissionContext ()

                FSharp.Compiler.HotReloadState.setCurrentEmissionContext (Some context)

                try
                    return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
                finally
                    FSharp.Compiler.HotReloadState.setCurrentEmissionContext previousContext
            | None -> return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
        }

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() = ic.ClearCaches()

    member ic.ClearCaches() =
        let utok = AnyCallerThread
        braceMatchCache.Clear(utok)
        backgroundCompiler.ClearCaches()
        ClearAllILModuleReaderCache()

    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        use _ =
            Activity.startNoTags "FsharpChecker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients"

        ic.ClearCaches()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        FxResolver.ClearStaticCaches()

    /// This function is called when the configuration is known to have changed for reasons not encoded in the ProjectOptions.
    /// For example, dependent references may have been deleted or created.
    member _.InvalidateConfiguration(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(options, userOpName)

    member _.InvalidateConfiguration(projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(projectSnapshot, userOpName)

    /// Clear the internal cache of the given projects.
    member _.ClearCache(options: seq<FSharpProjectOptions>, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ClearCache(options, userOpName)

    member _.ClearCache(projects: ProjectSnapshot.FSharpProjectIdentifier seq, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ClearCache(projects, userOpName)

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member _.NotifyProjectCleaned(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.NotifyProjectCleaned(options, userOpName)

    member _.NotifyFileChanged(fileName: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.NotifyFileChanged(fileName, options, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.CheckFileInProjectAllowingStaleCachedResults
        (parseResults: FSharpParseFileResults, fileName: string, fileVersion: int, source: string, options: FSharpProjectOptions, ?userOpName: string)
        =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(
            parseResults,
            fileName,
            fileVersion,
            SourceText.ofString source,
            options,
            userOpName
        )

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.CheckFileInProject
        (
            parseResults: FSharpParseFileResults,
            fileName: string,
            fileVersion: int,
            sourceText: ISourceText,
            options: FSharpProjectOptions,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.CheckFileInProject(parseResults, fileName, fileVersion, sourceText, options, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the
    /// parse including the reconstructed types in the file.
    member _.ParseAndCheckFileInProject
        (fileName: string, fileVersion: int, sourceText: ISourceText, options: FSharpProjectOptions, ?userOpName: string)
        =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckFileInProject(fileName, fileVersion, sourceText, options, userOpName)

    member _.ParseAndCheckFileInProject(fileName: string, projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckFileInProject(fileName, projectSnapshot, userOpName)

    member _.ParseAndCheckProject(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckProject(options, userOpName)

    member _.ParseAndCheckProject(projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.ParseAndCheckProject(projectSnapshot, userOpName)

    member _.FindBackgroundReferencesInFile
        (fileName: string, options: FSharpProjectOptions, symbol: FSharpSymbol, ?canInvalidateProject: bool, ?fastCheck: bool, ?userOpName: string)
        =
        let canInvalidateProject = defaultArg canInvalidateProject true
        let userOpName = defaultArg userOpName "Unknown"

        async {
            if fastCheck <> Some true || not captureIdentifiersWhenParsing then
                return! backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)
            else
                let! parseResults = backgroundCompiler.GetBackgroundParseResultsForFileInProject(fileName, options, userOpName)

                if
                    parseResults.ParseTree.Identifiers |> Set.contains symbol.DisplayNameCore
                    || parseResults.ParseTree.Identifiers |> NamesContainAttribute symbol
                then
                    return! backgroundCompiler.FindReferencesInFile(fileName, options, symbol, canInvalidateProject, userOpName)
                else
                    return Seq.empty
        }

    member _.FindBackgroundReferencesInFile(fileName: string, projectSnapshot: FSharpProjectSnapshot, symbol: FSharpSymbol, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"

        async {
            let! parseResults = backgroundCompiler.ParseFile(fileName, projectSnapshot, userOpName)

            if
                parseResults.ParseTree.Identifiers |> Set.contains symbol.DisplayNameCore
                || parseResults.ParseTree.Identifiers |> NamesContainAttribute symbol
            then
                return! backgroundCompiler.FindReferencesInFile(fileName, projectSnapshot, symbol, userOpName)
            else
                return Seq.empty
        }

    member _.GetBackgroundSemanticClassificationForFile(fileName: string, options: FSharpProjectOptions, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetSemanticClassificationForFile(fileName, options, userOpName)

    member _.GetBackgroundSemanticClassificationForFile(fileName: string, snapshot: FSharpProjectSnapshot, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetSemanticClassificationForFile(fileName, snapshot, userOpName)

    /// For a given script file, get the ProjectOptions implied by the #load closure
    member _.GetProjectOptionsFromScript
        (
            fileName,
            source,
            ?caret,
            ?previewEnabled,
            ?loadedTimeStamp,
            ?otherFlags,
            ?useFsiAuxLib,
            ?useSdkRefs,
            ?assumeDotNetFramework,
            ?sdkDirOverride,
            ?optionsStamp: int64,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"

        backgroundCompiler.GetProjectOptionsFromScript(
            fileName,
            source,
            caret,
            previewEnabled,
            loadedTimeStamp,
            otherFlags,
            useFsiAuxLib,
            useSdkRefs,
            sdkDirOverride,
            assumeDotNetFramework,
            optionsStamp,
            userOpName
        )

    /// For a given script file, get the ProjectSnapshot implied by the #load closure
    member _.GetProjectSnapshotFromScript
        (
            fileName,
            source,
            ?caret,
            ?documentSource,
            ?previewEnabled,
            ?loadedTimeStamp,
            ?otherFlags,
            ?useFsiAuxLib,
            ?useSdkRefs,
            ?assumeDotNetFramework,
            ?sdkDirOverride,
            ?optionsStamp: int64,
            ?userOpName: string
        ) =
        let userOpName = defaultArg userOpName "Unknown"
        let documentSource = defaultArg documentSource DocumentSource.FileSystem

        backgroundCompiler.GetProjectSnapshotFromScript(
            fileName,
            source,
            caret,
            documentSource,
            previewEnabled,
            loadedTimeStamp,
            otherFlags,
            useFsiAuxLib,
            useSdkRefs,
            sdkDirOverride,
            assumeDotNetFramework,
            optionsStamp,
            userOpName
        )

    member _.GetProjectOptionsFromCommandLineArgs(projectFileName, argv, ?loadedTimeStamp, ?isInteractive, ?isEditing) =
        let isEditing = defaultArg isEditing false
        let isInteractive = defaultArg isInteractive false
        let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading

        let argv =
            let define =
                if isInteractive then
                    "--define:INTERACTIVE"
                else
                    "--define:COMPILED"

            Array.append argv [| define |]

        let argv =
            if isEditing then
                Array.append argv [| "--define:EDITING" |]
            else
                argv

        {
            ProjectFileName = projectFileName
            ProjectId = None
            SourceFiles = [||] // the project file names will be inferred from the ProjectOptions
            OtherOptions = argv
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = loadedTimeStamp
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = None
        }

    member _.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, ?isInteractive, ?isEditing) =
        let isEditing = defaultArg isEditing false
        let isInteractive = defaultArg isInteractive false
        use errorScope = new DiagnosticsScope(argv |> List.contains "--flaterrors")

        let tcConfigB =
            TcConfigBuilder.CreateNew(
                legacyReferenceResolver,
                defaultFSharpBinariesDir = FSharpCheckerResultsSettings.defaultFSharpBinariesDir,
                reduceMemoryUsage = ReduceMemoryFlag.Yes,
                implicitIncludeDir = "",
                isInteractive = isInteractive,
                isInvalidationSupported = false,
                defaultCopyFSharpCore = CopyFSharpCoreFlag.No,
                tryGetMetadataSnapshot = tryGetMetadataSnapshot,
                sdkDirOverride = None,
                rangeForErrors = range0
            )

        // These defines are implied by the F# compiler
        tcConfigB.conditionalDefines <-
            let define = if isInteractive then "INTERACTIVE" else "COMPILED"
            define :: tcConfigB.conditionalDefines

        if isEditing then
            tcConfigB.conditionalDefines <- "EDITING" :: tcConfigB.conditionalDefines

        tcConfigB.realsig <- List.contains "--realsig" argv || List.contains "--realsig+" argv

        // Apply command-line arguments and collect more source files if they are in the arguments
        let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, argv)
        FSharpParsingOptions.FromTcConfigBuilder(tcConfigB, Array.ofList sourceFilesNew, isInteractive), errorScope.Diagnostics

    member ic.GetParsingOptionsFromCommandLineArgs(argv, ?isInteractive: bool, ?isEditing) =
        ic.GetParsingOptionsFromCommandLineArgs([], argv, ?isInteractive = isInteractive, ?isEditing = isEditing)

    member _.BeforeBackgroundFileCheck = backgroundCompiler.BeforeBackgroundFileCheck

    member _.FileParsed = backgroundCompiler.FileParsed

    member _.FileChecked = backgroundCompiler.FileChecked

    member _.ProjectChecked = backgroundCompiler.ProjectChecked

    static member ActualParseFileCount = BackgroundCompiler.ActualParseFileCount

    static member ActualCheckFileCount = BackgroundCompiler.ActualCheckFileCount

    static member Instance = globalInstance.Force()

    member internal _.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

    /// Compile a DLL from cached typecheck results, skipping parse/typecheck/optimization.
    /// For dev-loop use only. Requires keepAssemblyContents=true.
    /// Writes the assembly and portable PDB to outfile and returns the emitted module
    /// parsed back from the written bytes.
    member internal _.CompileFromCheckedProject(results: FSharpCheckProjectResults, outfile: string, naming: HotReloadEmitNaming) =
        async {
            let tcConfig, tcGlobals, tcImports, unfinalizedCcu, ccuSig, topAttrsOpt, _ilAssemRef, typedImplFilesOpt =
                results.CompilationData

            // This fast path writes only the implementation assembly. Projects that request a
            // reference assembly must use the normal compiler pipeline so dependents never see a
            // stale reference surface after a successful hot reload update.
            match tcConfig.emitMetadataAssembly with
            | MetadataAssemblyGeneration.None -> ()
            | MetadataAssemblyGeneration.ReferenceOnly
            | MetadataAssemblyGeneration.ReferenceOut _ ->
                invalidOp "In-process hot reload compilation does not support reference-assembly outputs; use the external compiler pipeline."

            ReportTime tcConfig "CompileFromCheckedProject: Setup"

            // Clear mode preserves the original emit-from-cache behavior: lay out closures with
            // normal @<line> names and let the delta emitter bridge them. Preserve mode is used
            // only after the session has installed the same replay tables the fsc emit hook
            // installs in PrepareForCodeGeneration; the caller owns clearing them in a finally.
            tcGlobals.CompilerGlobalState
            |> Option.iter (fun cgs ->
                match naming with
                | HotReloadEmitNaming.ClearForLineBasedBaseline ->
                    FSharp.Compiler.ClosureNameAllocationState.clearClosureNameState (cgs :> obj)
                    FSharp.Compiler.CompilerGeneratedNameMapState.clearCompilerGeneratedNameMap (cgs :> obj)
                | HotReloadEmitNaming.PreserveInstalledState _ -> ()

                // Reset occurrence counters so closure names (name@line-N) match a fresh-process
                // build instead of drifting as the reused CompilerGlobalState accumulates across edits.
                cgs.ResetCompilerGeneratedNameState())

            // The CCU from TransparentCompiler has unfinalized Contents (empty ModuleOrNamespaceType).
            // Finalize it using ccuSig, matching what CheckClosedInputSetFinish does.
            let ccuContents = Construct.NewCcuContents ILScopeRef.Local range0 unfinalizedCcu.AssemblyName ccuSig
            let generatedCcu = unfinalizedCcu.CloneWithFinalizedContents(ccuContents)

            let topAttrs =
                match topAttrsOpt with
                | Some a -> a
                | None -> raise (InvalidOperationException "CompileFromCheckedProject: no top attributes available")

            let typedImplFiles =
                match typedImplFilesOpt with
                | Some files -> files
                | None -> raise (InvalidOperationException "CompileFromCheckedProject: keepAssemblyContents must be true")

            // Note: We do NOT filter files with diagnostics here. FSharpCheckProjectResults.Diagnostics
            // may include warnings promoted to errors (e.g. FS1182 from --warnaserror+:1182) that
            // are suppressed by #nowarn in the source. These files compiled successfully in the
            // normal fsc pipeline and must be included here for IlxGen to resolve all types.
            // If there are genuine type-check errors, IlxGen will fail and we fall back to fsc.

            // Deduplicate QualifiedNameOfFile values. TransparentCompiler processes files
            // via dependency graph (potentially parallel), so the per-file DeduplicateParsedInputModuleName
            // may not see all prior names. Re-deduplicate here to avoid startup code type collisions.
            let typedImplFiles =
                typedImplFiles
                |> List.mapFold
                    (fun (seen: Map<string, int>) (f: CheckedImplFile) ->
                        let name = f.QualifiedNameOfFile.Text

                        match seen.TryFind name with
                        | None -> f, seen.Add(name, 1)
                        | Some count ->
                            let newCount = count + 1
                            let newName = name + "___" + string newCount

                            let newQName =
                                FSharp.Compiler.Syntax.QualifiedNameOfFile(FSharp.Compiler.Syntax.Ident(newName, f.QualifiedNameOfFile.Range))

                            let (CheckedImplFile(_, sig', contents, hasEntry, isScript, anonRecs, namedDbgPts)) = f
                            CheckedImplFile(newQName, sig', contents, hasEntry, isScript, anonRecs, namedDbgPts), seen.Add(name, newCount))
                    Map.empty
                |> fst

            // Save and restore CCU attribs to prevent quadratic growth on repeated compile calls.
            let originalAttribs = generatedCcu.Contents.Attribs
            generatedCcu.Contents.SetAttribs(originalAttribs @ topAttrs.assemblyAttrs)

            use _restoreAttribs =
                { new System.IDisposable with
                    member _.Dispose() =
                        generatedCcu.Contents.SetAttribs(originalAttribs)
                }

            let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents

            ReportTime tcConfig "CompileFromCheckedProject: Encode Signature Data"

            let sigDataAttributes, sigDataResources =
                EncodeSignatureData(tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, false)

            let tcVal = LightweightTcValForUsingInBuildMethodCall tcGlobals
            let importMap = tcImports.GetImportMap()
            let optEnv0 = GetInitialOptimizationEnv(tcImports, tcGlobals)

            ReportTime tcConfig "CompileFromCheckedProject: Optimizations"

            // Dev-loop optimization: use minimal passes only (no extra loops, no detuple,
            // no TLR, no cross-assembly opt). This DLL is for local testing, not shipping.
            // OptimizeImplFile + LowerLocalMutables + LowerCalls are mandatory for correct IlxGen.
            let minimalSettings =
                { tcConfig.optSettings with
                    jitOptUser = Some false
                    localOptUser = Some false
                    crossAssemblyOptimizationUser = Some false
                    lambdaInlineThreshold = 0
                    abstractBigTargets = false
                    reportingPhase = false
                }

            let optimizeImplFile env hidingInfo implFile =
                let (env', file, _optInfo, hidingInfo'), optDuringCodeGen =
                    Optimizer.OptimizeImplFile(
                        minimalSettings,
                        generatedCcu,
                        tcGlobals,
                        tcVal,
                        importMap,
                        env,
                        false,
                        tcConfig.emitTailcalls,
                        hidingInfo,
                        implFile
                    )

                let file = LowerLocalMutables.TransformImplFile tcGlobals importMap file
                let file = LowerCalls.LowerImplFile tcGlobals file

                {
                    ImplFile = file
                    OptimizeDuringCodeGen = optDuringCodeGen
                },
                env',
                hidingInfo'

            let optimizeWithThreadedEnvironment () =
                typedImplFiles
                |> List.mapFold
                    (fun (env, hidingInfo) implFile ->
                        let file, env', hidingInfo' = optimizeImplFile env hidingInfo implFile
                        file, (env', hidingInfo'))
                    (optEnv0, SignatureHidingInfo.Empty)
                |> fst
                |> CheckedAssemblyAfterOptimization

            let optimizeWithCachedPerFileTrees () =
                let context: HotReloadIncrementalEmit.OptimizationContext =
                    {
                        TcGlobals = tcGlobals
                        TcImports = tcImports
                        Settings = minimalSettings
                        EmitTailcalls = tcConfig.emitTailcalls
                    }

                hotReloadIncrementalEmitCache.Value.Optimize(outfile, context, optEnv0, typedImplFiles, optimizeImplFile)
                |> CheckedAssemblyAfterOptimization

            let optimizedImpls, optDataResources =
                let impls =
                    if isEnvVarTruthy "FSHARP_HOTRELOAD_INCREMENTAL_EMIT" then
                        HotReloadIncrementalEmit.runWithFallback optimizeWithCachedPerFileTrees optimizeWithThreadedEnvironment
                    else
                        optimizeWithThreadedEnvironment ()

                impls, []

            match naming with
            | HotReloadEmitNaming.ClearForLineBasedBaseline -> ()
            | HotReloadEmitNaming.PreserveInstalledState replay ->
                // Mirrors HotReloadEmitHook.PrepareForCodeGeneration: compute the
                // stamp-keyed closure-name replay over the optimized implementation that
                // IlxGen is about to lower, then leave cleanup to the caller's finally.
                let assignedNames, _ =
                    HotReloadBaseline.computeOccurrenceKeyedClosureNames
                        tcGlobals
                        replay.Baseline
                        replay.BaselineImplementation
                        optimizedImpls
                        replay.CurrentGeneration

                if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_CLOSURENAMES") = "1" then
                    printfn
                        "[fsharp-hotreload][closure-names] in-process install: tables=%d gen=%d assigned=%d names=%A"
                        (Map.count replay.Baseline.EncClosureNames)
                        replay.CurrentGeneration
                        (Map.count assignedNames)
                        (assignedNames |> Map.toList)

                FSharp.Compiler.ClosureNameAllocationState.setAssignedClosureNames (tcGlobals.CompilerGlobalState.Value :> obj) assignedNames

            ReportTime tcConfig "CompileFromCheckedProject: TAST -> IL"
            let ilxGenerator = CreateIlxAssemblyGenerator(tcConfig, tcImports, tcGlobals, tcVal, generatedCcu)

            let codegenResults =
                GenerateIlxCode(IlWriteBackend, false, tcConfig, topAttrs, optimizedImpls, generatedCcu.AssemblyName, ilxGenerator)

            let topAssemblyAttrs = codegenResults.topAssemblyAttrs

            let topAttrs =
                { topAttrs with
                    assemblyAttrs = topAssemblyAttrs
                }

            let secDecls = mkILSecurityDecls codegenResults.permissionSets

            let metadataVersion =
                match tcConfig.metadataVersion with
                | Some v -> v
                | _ -> ""

            let ctok = CompilationThreadToken()

            // Extract AssemblyVersionAttribute from typed assembly attributes, matching fsc's logic.
            let assemVerFromAttrib =
                match AttributeHelpers.TryFindStringAttribute tcGlobals "System.Reflection.AssemblyVersionAttribute" topAttrs.assemblyAttrs with
                | Some versionString ->
                    try
                        Some(parseILVersion versionString)
                    with _ ->
                        None
                | _ ->
                    match tcConfig.version with
                    | VersionNone -> Some(ILVersionInfo(0us, 0us, 0us, 0us))
                    | _ -> Some(tcConfig.version.GetVersionInfo tcConfig.implicitIncludeDir)

            let ilxMainModule =
                let m =
                    MainModuleBuilder.CreateMainModule(
                        ctok,
                        tcConfig,
                        tcGlobals,
                        tcImports,
                        None,
                        generatedCcu.AssemblyName,
                        outfile,
                        topAttrs,
                        sigDataAttributes,
                        sigDataResources,
                        optDataResources,
                        codegenResults,
                        assemVerFromAttrib,
                        metadataVersion,
                        secDecls
                    )
                // Strip native resources — default.win32manifest may not exist on all platforms.
                { m with NativeResources = [] }

            let normalizeAssemblyRefs (aref: ILAssemblyRef) =
                match tcImports.TryFindDllInfo(ctok, rangeStartup, aref.Name, lookupOnly = false) with
                | Some dllInfo ->
                    match dllInfo.ILScopeRef with
                    | ILScopeRef.Assembly normalized -> normalized
                    | _ -> aref
                | None -> aref

            ReportTime tcConfig "CompileFromCheckedProject: Write .NET Binary"

            let moduleCustomDebugInfoRows =
                match naming with
                | HotReloadEmitNaming.PreserveInstalledState _ when
                    Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DISABLE_SYNTHESIZED_NAME_SNAPSHOT_CDI")
                    <> "1"
                    ->
                    match tryGetCompilerGeneratedNameMap (tcGlobals.CompilerGlobalState.Value :> obj) with
                    | Some map ->
                        HotReloadBaseline.collectRecordedSynthesizedNameSnapshot (tcGlobals.CompilerGlobalState.Value :> obj) map
                        |> FSharp.Compiler.EncMethodDebugInformation.computeSynthesizedNameSnapshotCustomDebugInfoRows
                    | None -> []
                | _ -> []

            // Emit the sibling portable PDB alongside the DLL (mirroring a normal
            // --debug:portable fsc write). The hot reload emit path reads this PDB back as the
            // fresh sequence-point source for line-shift detection and active-statement
            // remapping; leaving the external build's stale PDB on disk would silently feed
            // outdated lines into that analysis. Method CDI rows stay empty here; the only
            // hot reload CDI written by this path is the F# allocation-ordered
            // synthesized-name snapshot when preserving session naming state.
            // Write once in memory: the bytes are persisted to disk below (the runtime and
            // debugger read them there) AND parsed straight back for the caller, so the hot
            // reload delta path consumes the exact read-back representation a disk parse
            // would give, without the disk round trip.
            let writerOptions: ILBinaryWriter.options =
                {
                    ilg = tcGlobals.ilg
                    outfile = outfile
                    pdbfile = Some(!!Path.ChangeExtension(outfile, ".pdb"))
                    emitTailcalls = tcConfig.emitTailcalls
                    // The in-process path bypasses fsc's TcConfigBuilder determinism pins.
                    // Force deterministic PE/PDB emission without mutating the cached TcConfig.
                    deterministic = true
                    portablePDB = true
                    embeddedPDB = false
                    embedAllSource = false
                    embedSourceList = []
                    allGivenSources = []
                    sourceLink = ""
                    checksumAlgorithm = tcConfig.checksumAlgorithm
                    signer = GetStrongNameSigner(ValidateKeySigningAttributes(tcConfig, tcGlobals, topAttrs))
                    dumpDebugInfo = false
                    referenceAssemblyOnly = false
                    referenceAssemblyAttribOpt = None
                    referenceAssemblySignatureHash = None
                    pathMap = tcConfig.pathMap
                    moduleCustomDebugInfoRows = moduleCustomDebugInfoRows
                    methodCustomDebugInfoRows = Map.empty
                }

            let assemblyBytes, pdbBytesOpt, _writerTokenMappings, _ =
                WriteILBinaryInMemoryWithArtifacts(writerOptions, ilxMainModule, normalizeAssemblyRefs)

            // WriteILBinaryFile created the output directory; keep that contract.
            match Path.GetDirectoryName outfile with
            | null
            | "" -> ()
            | dir -> Directory.CreateDirectory dir |> ignore

            File.WriteAllBytes(outfile, assemblyBytes)

            let pdbPath = !!Path.ChangeExtension(outfile, ".pdb")

            match pdbBytesOpt with
            | Some pdbBytes -> File.WriteAllBytes(pdbPath, pdbBytes)
            | None -> ()

            ReportTime tcConfig "Exiting"

            // Parse the just-written bytes back into the read-back representation the hot
            // reload delta emitter and symbol matcher expect (a writer-side ILModuleDef is
            // NOT equivalent: scope refs and body layout differ from a read module).
            let readerOptions: ILReaderOptions =
                {
                    pdbDirPath = None
                    reduceMemoryUsage = ReduceMemoryFlag.Yes
                    metadataOnly = MetadataOnlyFlag.No
                    tryGetMetadataSnapshot = fun _ -> None
                }

            use reader = OpenILModuleReaderFromBytes outfile assemblyBytes readerOptions
            let emittedModule = reader.ILModuleDef

            // The writer's token-mapping closures are keyed by writer-side signatures and are
            // not directly callable with the read-back IL objects used by the delta emitter. The
            // read-back objects carry metadata row ids from the same bytes, so these mappings
            // expose the actual emitted tokens without a second module write.
            let token (table: FSharp.Compiler.AbstractIL.BinaryConstants.TableName) rowId =
                if rowId > 0 then (table.Index <<< 24) ||| rowId else 0

            let tokenMappings: ILBinaryWriter.ILTokenMappings =
                {
                    TypeDefTokenMap = fun (_, typeDef) -> token FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.TypeDef typeDef.MetadataIndex
                    FieldDefTokenMap = fun _ fieldDef -> token FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.Field fieldDef.MetadataIndex
                    MethodDefTokenMap = fun _ methodDef -> token FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.Method methodDef.MetadataIndex
                    PropertyTokenMap =
                        fun _ propertyDef -> token FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.Property propertyDef.MetadataIndex
                    EventTokenMap = fun _ eventDef -> token FSharp.Compiler.AbstractIL.BinaryConstants.TableNames.Event eventDef.MetadataIndex
                }

            return
                {
                    IlModule = emittedModule
                    EmittedArtifacts =
                        {
                            AssemblyBytes = assemblyBytes
                            PdbBytes = pdbBytesOpt
                            TokenMappings = tokenMappings
                        }
                }
        }

    /// Tokenize a single line, returning token information and a tokenization state represented by an integer
    member _.TokenizeLine(line: string, state: FSharpTokenizerLexState) =
        let tokenizer = FSharpSourceTokenizer([], None, None, None)
        let lineTokenizer = tokenizer.CreateLineTokenizer line
        let mutable state = (None, state)

        let tokens =
            [|
                while (state <- lineTokenizer.ScanToken(snd state)
                       (fst state).IsSome) do
                    yield (fst state).Value
            |]

        tokens, snd state

    /// Tokenize an entire file, line by line
    member x.TokenizeFile(source: string) : FSharpTokenInfo[][] =
        let lines = source.Split('\n')

        let tokens =
            [|
                let mutable state = FSharpTokenizerLexState.Initial

                for line in lines do
                    let tokens, n = x.TokenizeLine(line, state)
                    state <- n
                    yield tokens
            |]

        tokens

namespace FSharp.Compiler

open System
open System.IO
open Internal.Utilities
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text.Range
open FSharp.Compiler.DiagnosticsLogger

type CompilerEnvironment() =
    /// Source file extensions
    static let compilableExtensions = FSharpSigFileSuffixes @ FSharpImplFileSuffixes @ FSharpScriptFileSuffixes

    /// Single file projects extensions
    static let singleFileProjectExtensions = FSharpScriptFileSuffixes

    static member BinFolderOfDefaultFSharpCompiler(?probePoint) =
        FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(probePoint)

    // Legacy entry point, no longer used by FSharp.Editor
    static member DefaultReferencesForOrphanSources assumeDotNetFramework =
        let currentDirectory = Directory.GetCurrentDirectory()

        let fxResolver =
            FxResolver(
                assumeDotNetFramework,
                currentDirectory,
                rangeForErrors = range0,
                useSdkRefs = true,
                isInteractive = false,
                sdkDirOverride = None
            )

        let references, _ = fxResolver.GetDefaultReferences(useFsiAuxLib = false)
        references

    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    static member GetConditionalDefinesForEditing(parsingOptions: FSharpParsingOptions) =
        SourceFileImpl.GetImplicitConditionalDefinesForEditing(parsingOptions.IsInteractive)
        @ parsingOptions.ConditionalDefines

    /// Return true if this is a subcategory of error or warning message that the language service can emit
    static member IsCheckerSupportedSubcategory(subcategory: string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedDiagnostic.IsSubcategoryOfCompile(subcategory)

    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    static member GetDebuggerLanguageID() =
        Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)

    static member IsScriptFile(fileName: string) = ParseAndCheckInputs.IsScript fileName

    /// Whether or not this file is compilable
    static member IsCompilable(file: string) =
        let ext = Path.GetExtension file

        compilableExtensions
        |> List.exists (fun e -> 0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))

    /// Whether or not this file should be a single-file project
    static member MustBeSingleFileProject(file: string) =
        let ext = Path.GetExtension file

        singleFileProjectExtensions
        |> List.exists (fun e -> 0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))
