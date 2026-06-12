// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection
open System.Security.Cryptography
open System.Threading
open Microsoft.FSharp.Reflection
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
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.TransparentCompiler
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Driver
open FSharp.Compiler.DiagnosticsLogger
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
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.EnvironmentHelpers

[<RequireQualifiedAccess>]
type FSharpHotReloadError =
    | NoActiveSession
    | NoChanges
    | MissingOutputPath
    | UnsupportedEdit of string
    | CompilationFailed of FSharpDiagnostic[]
    | DeltaEmissionFailed of string

[<System.Flags>]
type FSharpHotReloadCapability =
    | None = 0
    | Il = 1
    | Metadata = 2
    | PortablePdb = 4
    | MultipleGenerations = 8
    | RuntimeApply = 16

type FSharpHotReloadCapabilities internal (flags: FSharpHotReloadCapability) =
    member _.Flags = flags
    member _.SupportsIl = flags.HasFlag(FSharpHotReloadCapability.Il)
    member _.SupportsMetadata = flags.HasFlag(FSharpHotReloadCapability.Metadata)
    member _.SupportsPortablePdb = flags.HasFlag(FSharpHotReloadCapability.PortablePdb)
    member _.SupportsMultipleGenerations = flags.HasFlag(FSharpHotReloadCapability.MultipleGenerations)
    member _.SupportsRuntimeApply = flags.HasFlag(FSharpHotReloadCapability.RuntimeApply)

    static member internal FromInternalFlags(flags: HotReloadCapabilityFlags) =
        let casted = enum<FSharpHotReloadCapability>(int flags)
        FSharpHotReloadCapabilities(casted)

type FSharpAddedOrChangedMethodInfo =
    { MethodToken: int
      LocalSignatureToken: int
      CodeOffset: int
      CodeLength: int }

type FSharpHotReloadDelta =
    { Metadata: byte[]
      IL: byte[]
      Pdb: byte[] option
      UpdatedTypes: int list
      UpdatedMethods: int list
      AddedOrChangedMethods: FSharpAddedOrChangedMethodInfo list
      UserStringUpdates: struct (int * int * string) list
      GenerationId: Guid
      BaseGenerationId: Guid
      SequencePointUpdates: FSharpSequencePointUpdates list
      ActiveStatementUpdates: FSharpActiveStatementRemapResult list }

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

[<Sealed>]
type internal FSharpHotReloadService
    (
        // Session store owned by the entity holding this service: the checker's default
        // compatibility session, or one FSharpHotReloadSession instance. The owner decides
        // whether the store is also registered as the process-wide store the module-level
        // helpers and the fsc emit hook route to.
        sessionStore: FSharp.Compiler.HotReloadState.HotReloadSessionStore,
        readIlModule: string -> ILModuleDef,
        createBaseline: TcGlobals -> ILModuleDef -> string -> FSharpEmitBaseline,
        getHotReloadDiffInputs: FSharpCheckProjectResults -> TcGlobals * CheckedAssemblyAfterOptimization,
        getErrorDiagnostics: FSharpDiagnostic[] -> FSharpDiagnostic[],
        waitForStableFile: string -> unit,
        tryGetOutputFingerprint: string -> (DateTime * byte[] option) option,
        hasOutputFingerprintChanged:
            string -> (DateTime * byte[] option) option -> (DateTime * byte[] option) option -> bool,
        toPublicDelta: IlxDelta -> FSharpHotReloadDelta,
        mapHotReloadError: HotReloadError -> FSharpHotReloadError
    ) =

    let hotReloadGate = obj()

    // Per-project synthesized-name replay maps (keyed like the session store slots).
    let synthesizedTypeMaps =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, FSharpSynthesizedTypeMaps>()

    let editAndContinueService = FSharpEditAndContinueLanguageService(sessionStore)

    // Per-project snapshot of the last committed output assembly. If semantic edits are detected
    // while this fingerprint remains unchanged, we refuse to emit deltas from stale binaries.
    let committedOutputFingerprints =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, (DateTime * byte[] option) option>()

    // Fingerprints staged by deferred-commit emits; promoted to committed on session commit.
    let pendingOutputFingerprints =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, (DateTime * byte[] option) option>()

    let traceSessionTransitions = isEnvVarTruthy "FSHARP_HOTRELOAD_TRACE_SESSIONS"

    let describeSessionStartTransition transition =
        match transition with
        | FSharp.Compiler.HotReloadState.HotReloadSessionStart.StartedFresh -> "started-fresh"
        | FSharp.Compiler.HotReloadState.HotReloadSessionStart.ReplacedExisting -> "replaced-existing"

    // Must be called under hotReloadGate.
    let getOrCreateSynthesizedTypeMap (projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey) =
        match synthesizedTypeMaps.TryGetValue projectKey with
        | true, existing -> existing
        | false, _ ->
            let created = FSharpSynthesizedTypeMaps()
            synthesizedTypeMaps[projectKey] <- created
            created

    /// Captures one more project baseline inside the session (Roslyn analog: capturing an
    /// <c>EmitBaseline</c> for one more module of the debugged process). Re-adding a project
    /// the session already tracks recaptures its baseline; other projects and the
    /// session-wide capability/active-statement state stay untouched.
    member _.AddHotReloadProject
        (projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey)
        (parseAndCheckProject: unit -> Async<FSharpCheckProjectResults>)
        (outputPath: string option)
        =
        async {
            match outputPath with
            | None -> return Result.Error FSharpHotReloadError.MissingOutputPath
            | Some outputPath ->
                let! projectResults = parseAndCheckProject ()
                let errors = getErrorDiagnostics projectResults.Diagnostics

                if projectResults.HasCriticalErrors || errors.Length > 0 then
                    return Result.Error(FSharpHotReloadError.CompilationFailed errors)
                elif not (File.Exists(outputPath)) then
                    return
                        Result.Error(
                            FSharpHotReloadError.DeltaEmissionFailed(
                                $"Output assembly '{outputPath}' was not found. Build the project before starting a hot reload session."
                            )
                        )
                else
                    let tcGlobals, implementationFiles = getHotReloadDiffInputs projectResults
                    waitForStableFile outputPath

                    let baselineResult : Result<_, FSharpHotReloadError> =
                        try
                            let ilModule = readIlModule outputPath
                            let baseline = createBaseline tcGlobals ilModule outputPath
                            Ok(baseline, implementationFiles)
                        with ex ->
                            Result.Error(
                                FSharpHotReloadError.DeltaEmissionFailed(
                                    $"Failed to create hot reload baseline: {ex.Message}"
                                )
                            )

                    match baselineResult with
                    | Result.Error error -> return Result.Error error
                    | Ok(baseline, implementationFiles) ->
                        lock hotReloadGate (fun () ->
                            // Closure mapping (C6): the per-method occurrence-chain -> closure-name
                            // tables were reconstructed from the on-disk CDI occurrence keys by
                            // createBaseline (baseline closure names are occurrence-derived under the
                            // flag), so no in-process carry-over is needed — this is exactly what a
                            // session started in a fresh process computes. The previous MVID-matched
                            // carry-over is demoted to a consistency check: when the session being
                            // replaced was captured in-process for the SAME output assembly at
                            // generation 0, its tables must match the reconstruction. (A mid-chain
                            // session legitimately differs — its tables carry later-generation names
                            // for closures added by deltas, while a restart from the gen-0 disk
                            // baseline correctly resets to the generation-0 tables.)
                            (match editAndContinueService.TryGetSession() with
                             | ValueSome existing when
                                 existing.Baseline.ModuleId = baseline.ModuleId
                                 // CurrentGeneration is the generation the NEXT delta will
                                 // produce; 1 means no delta has been applied yet, i.e. the
                                 // session still holds the unmodified gen-0 capture.
                                 && existing.CurrentGeneration = 1
                                 && not (Map.isEmpty existing.Baseline.EncClosureNames)
                                 // An EMPTY reconstruction is a designed fail-closed outcome
                                 // (recapture DLLs carry mid-session generation-suffixed names
                                 // that are not derivable from gen-0 identity); only a
                                 // CONFLICTING non-empty reconstruction is an inconsistency.
                                 && not (Map.isEmpty baseline.EncClosureNames)
                                 && existing.Baseline.EncClosureNames <> baseline.EncClosureNames
                                 ->
                                 if traceSessionTransitions then
                                     printfn
                                         "[fsharp-hotreload][session] WARNING: CDI-reconstructed closure-name tables (%d methods) disagree with the in-process capture session's tables (%d methods) for %s"
                                         (Map.count baseline.EncClosureNames)
                                         (Map.count existing.Baseline.EncClosureNames)
                                         outputPath

                                 System.Diagnostics.Debug.Assert(
                                     false,
                                     "Hot reload closure-name reconstruction from disk disagrees with the in-process capture session being replaced."
                                 )
                             | _ -> ())

                            let compilerState = tcGlobals.CompilerGlobalState.Value
                            let map =
                                let targetMap = getOrCreateSynthesizedTypeMap projectKey

                                baseline.SynthesizedNameSnapshot
                                |> Map.toSeq
                                |> Seq.map (fun (k, v) -> struct (k, v))
                                |> targetMap.LoadSnapshot
                                targetMap.BeginSession()
                                targetMap

                            setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)

                            let startTransition =
                                // Session-entity AddProject: other projects and the
                                // session-wide capability set stay untouched.
                                editAndContinueService.AddProject(projectKey, baseline, implementationFiles)

                            if traceSessionTransitions then
                                printfn
                                    "[fsharp-hotreload][session] start transition=%s moduleId=%O output=%s"
                                    (describeSessionStartTransition startTransition)
                                    baseline.ModuleId
                                    outputPath

                            committedOutputFingerprints[projectKey] <- tryGetOutputFingerprint outputPath
                            pendingOutputFingerprints.Remove projectKey |> ignore)

                        return Result.Ok ()
        }

    /// <summary>
    /// Emits a delta for the project by diffing the fresh snapshot against the COMMITTED
    /// baseline. The emitted update is always STAGED as pending (Roslyn's
    /// EmitSolutionUpdate/CommitSolutionUpdate split); the session advances it via
    /// <c>CommitSession</c> after the runtime applied it, or drops it via <c>DiscardSession</c>.
    /// </summary>
    member _.EmitHotReloadDelta
        (projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey)
        (parseAndCheckProject: unit -> Async<FSharpCheckProjectResults>)
        (outputPath: string option)
        =
        async {
            match outputPath with
            | None -> return Result.Error FSharpHotReloadError.MissingOutputPath
            | Some outputPath ->
                let! projectResults = parseAndCheckProject ()

                let errors = getErrorDiagnostics projectResults.Diagnostics

                if projectResults.HasCriticalErrors || errors.Length > 0 then
                    return Result.Error(FSharpHotReloadError.CompilationFailed errors)
                elif not (File.Exists(outputPath)) then
                    return
                        Result.Error(
                            FSharpHotReloadError.DeltaEmissionFailed(
                                $"Output assembly '{outputPath}' was not found. Build the project before emitting a hot reload delta."
                            )
                        )
                else
                    let tcGlobals, implementationFiles = getHotReloadDiffInputs projectResults
                    waitForStableFile outputPath
                    let outputFingerprint = tryGetOutputFingerprint outputPath

                    let sessionActive =
                        lock hotReloadGate (fun () ->
                            match editAndContinueService.TryGetSession(projectKey) with
                            | ValueSome _ -> true
                            | ValueNone ->
                                // Restore from the last committed snapshot if an overlapping
                                // compile cleared the currently active session.
                                match editAndContinueService.TryRestoreSession(projectKey) with
                                | ValueSome restoredSession ->
                                    let compilerState = tcGlobals.CompilerGlobalState.Value
                                    let map = getOrCreateSynthesizedTypeMap projectKey

                                    restoredSession.Baseline.SynthesizedNameSnapshot
                                    |> Map.toSeq
                                    |> Seq.map (fun (k, v) -> struct (k, v))
                                    |> map.LoadSnapshot
                                    map.BeginSession()
                                    setCompilerGeneratedNameMap (compilerState :> obj) (map :> ICompilerGeneratedNameMap)
                                    true
                                | ValueNone -> false)

                    if not sessionActive then
                        return Result.Error FSharpHotReloadError.NoActiveSession
                    else
                        let staleOutputErrorOpt =
                            lock hotReloadGate (fun () ->
                                match editAndContinueService.TryGetSession(projectKey) with
                                | ValueNone -> None
                                | ValueSome session ->
                                    let symbolChanges =
                                        computeSymbolChanges
                                            tcGlobals
                                            session.Capabilities
                                            session.ImplementationFiles
                                            implementationFiles

                                    match mapSymbolChangesToDelta session.Baseline symbolChanges with
                                    | Error mappingErrors ->
                                        Some(FSharpHotReloadError.UnsupportedEdit(String.concat Environment.NewLine mappingErrors))
                                    | Ok(updatedTypes, updatedMethods, accessorUpdates) ->
                                        let hasUpdates =
                                            not (List.isEmpty updatedTypes)
                                            || not (List.isEmpty updatedMethods)
                                            || not (List.isEmpty accessorUpdates)
                                            || not (List.isEmpty symbolChanges.Added)

                                        let committedFingerprint =
                                            match committedOutputFingerprints.TryGetValue projectKey with
                                            | true, fingerprint -> fingerprint
                                            | false, _ -> None

                                        if hasUpdates && not (hasOutputFingerprintChanged outputPath committedFingerprint outputFingerprint) then
                                            Some(
                                                FSharpHotReloadError.DeltaEmissionFailed(
                                                    $"Output assembly '{outputPath}' did not change after compilation; refusing to emit a delta from stale build output."
                                                )
                                            )
                                        else
                                            None)

                        match staleOutputErrorOpt with
                        | Some staleError -> return Result.Error staleError
                        | None ->
                            let ilModuleResult : Result<_, FSharpHotReloadError> =
                                try
                                    readIlModule outputPath |> Ok
                                with ex ->
                                    Result.Error(
                                        FSharpHotReloadError.DeltaEmissionFailed(
                                            $"Failed to read updated assembly '{outputPath}': {ex.Message}"
                                        )
                                    )

                            match ilModuleResult with
                            | Result.Error error -> return Result.Error error
                            | Ok ilModule ->
                                lock hotReloadGate (fun () ->
                                    match synthesizedTypeMaps.TryGetValue projectKey with
                                    | true, map ->
                                        map.BeginSession()
                                        setCompilerGeneratedNameMap (tcGlobals.CompilerGlobalState.Value :> obj) (map :> ICompilerGeneratedNameMap)
                                    | false, _ -> ())

                                // Phase G: the fresh compile's on-disk PDB carries the sequence
                                // points the IL module read loses (readIlModule attaches no debug
                                // points). It feeds line-shift detection, active-statement
                                // remapping and the emitted PDB delta. Missing/unreadable PDBs
                                // degrade gracefully (analysis stays inert, fail closed).
                                let freshDebugPdb =
                                    let pdbPath =
                                        Path.ChangeExtension(outputPath, ".pdb")
                                        |> Option.ofObj
                                        |> Option.defaultValue (outputPath + ".pdb")

                                    if File.Exists(pdbPath) then
                                        try
                                            Some(File.ReadAllBytes(pdbPath))
                                        with :? IOException ->
                                            None
                                    else
                                        None

                                match
                                    editAndContinueService.EmitDeltaForCompilation(
                                        tcGlobals,
                                        implementationFiles,
                                        ilModule,
                                        ?freshDebugPdb = freshDebugPdb,
                                        projectKey = projectKey,
                                        deferCommit = true
                                    )
                                with
                                | Ok result ->
                                    match result.Delta.UpdatedBaseline with
                                    | Some _ ->
                                        lock hotReloadGate (fun () ->
                                            pendingOutputFingerprints[projectKey] <- outputFingerprint)
                                    | None -> ()
                                    return Result.Ok(toPublicDelta result.Delta)
                                | Error error -> return Result.Error(mapHotReloadError error)
        }

    /// <summary>
    /// Commits ALL pending project updates atomically (Roslyn's <c>CommitSolutionUpdate</c>):
    /// pending baselines, staged implementation files and staged output fingerprints advance
    /// together across every project in the session.
    /// </summary>
    member _.CommitSession() =
        lock hotReloadGate (fun () ->
            editAndContinueService.CommitAllPendingUpdates() |> ignore

            for entry in List.ofSeq pendingOutputFingerprints do
                committedOutputFingerprints[entry.Key] <- entry.Value

            pendingOutputFingerprints.Clear())

    /// <summary>Discards ALL pending project updates (Roslyn's <c>DiscardSolutionUpdate</c>).</summary>
    member _.DiscardSession() =
        lock hotReloadGate (fun () ->
            editAndContinueService.DiscardPendingUpdate()
            pendingOutputFingerprints.Clear())

    member _.EndSession() =
        lock hotReloadGate (fun () ->
            synthesizedTypeMaps.Clear()
            committedOutputFingerprints.Clear()
            pendingOutputFingerprints.Clear()
            editAndContinueService.ResetSessionState())

    /// Session-wide capability set (Roslyn parity: a DebuggingSession-level property).
    member _.SetSessionCapabilities(capabilities: EditAndContinueCapabilities) =
        editAndContinueService.SetCapabilities(capabilities)

    /// Session-wide active statements (the break state describes the host process).
    member _.SetSessionActiveStatements(activeStatements: FSharpManagedActiveStatementDebugInfo seq) =
        editAndContinueService.SetActiveStatements(List.ofSeq activeStatements)

    member _.ProjectKeys = editAndContinueService.ProjectKeys

    member _.TryGetProjectSession(projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey) =
        editAndContinueService.TryGetSession(projectKey)

/// <summary>
/// A hot reload (Edit and Continue) session — the F# analogue of Roslyn's
/// <c>DebuggingSession</c>. One instance per host session (a <c>dotnet watch</c> run, a debug
/// session), created via <c>FSharpChecker.CreateHotReloadSession</c>. The session owns
/// per-project committed state (snapshot view, emit baseline, generation chain) keyed by
/// <c>FSharpProjectIdentifier</c>, plus session-wide runtime capabilities and active
/// statements. Sessions are fully independent instances: creating a second session does not
/// disturb the first, and disposing one ends only that session.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
[<Sealed; AutoSerializable(false)>]
type FSharpHotReloadSession
    internal
    (
        hotReloadService: FSharpHotReloadService,
        parseAndCheckSnapshot: FSharpProjectSnapshot -> string -> Async<FSharpCheckProjectResults>,
        tryGetOutputPath: FSharpProjectSnapshot -> string option,
        // Registers a successfully baselined project (resolved output path + project key) in
        // the owning checker's live-session registry, which FSharpChecker.Compile consults to
        // scope in-process compiles of session-tracked projects to this session.
        onProjectBaselined: string -> FSharp.Compiler.HotReloadState.HotReloadProjectKey -> unit,
        // Removes this session's entries from that registry when the session ends.
        onDispose: unit -> unit
    ) =

    let mutable disposed = 0

    // Output paths explicitly supplied to AddProject, consulted before deriving the path from
    // the snapshot's command-line options (hosts whose snapshots carry no -o option).
    let outputPathOverrides =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, string>()

    let outputPathGate = obj ()

    let projectKeyOfSnapshot (projectSnapshot: FSharpProjectSnapshot) =
        let identifier = projectSnapshot.Identifier
        FSharp.Compiler.HotReloadState.HotReloadProjectKey.Project(identifier.ProjectFileName, identifier.OutputFileName)

    let resolveOutputPath (projectKey: FSharp.Compiler.HotReloadState.HotReloadProjectKey) (projectSnapshot: FSharpProjectSnapshot) =
        let overridePath =
            lock outputPathGate (fun () ->
                match outputPathOverrides.TryGetValue projectKey with
                | true, path -> Some path
                | false, _ -> None)

        match overridePath with
        | Some path -> Some path
        | None -> tryGetOutputPath projectSnapshot

    let ensureNotDisposed () =
        if disposed = 1 then
            raise (ObjectDisposedException(nameof FSharpHotReloadSession))

    /// <summary>
    /// Captures the baseline of one more project into the session (Roslyn analog: capturing the
    /// per-module <c>EmitBaseline</c> when a module of the debugged process loads). The project's
    /// built output must exist on disk; build it before adding. Adding a project the session
    /// already tracks recaptures its baseline; other projects and the session-wide
    /// capability/active-statement state are untouched.
    /// </summary>
    /// <param name="projectSnapshot">The snapshot describing the project to track.</param>
    /// <param name="outputPath">Overrides the output-assembly path derived from the snapshot's
    /// command-line options; required when the snapshot carries no output option.</param>
    /// <param name="userOpName">An optional string used for tracing compiler operations associated with this request.</param>
    member _.AddProject(projectSnapshot: FSharpProjectSnapshot, ?outputPath: string, ?userOpName: string) =
        async {
            ensureNotDisposed ()
            let opName = defaultArg userOpName "Unknown"

            use _ =
                Activity.start "FSharpHotReloadSession.AddProject" [|
                    Activity.Tags.userOpName, opName
                    Activity.Tags.project, projectSnapshot.ProjectFileName
                |]

            let projectKey = projectKeyOfSnapshot projectSnapshot

            match outputPath with
            | Some path -> lock outputPathGate (fun () -> outputPathOverrides[projectKey] <- path)
            | None -> ()

            let resolvedOutputPath = resolveOutputPath projectKey projectSnapshot

            let! result =
                hotReloadService.AddHotReloadProject
                    projectKey
                    (fun () -> parseAndCheckSnapshot projectSnapshot opName)
                    resolvedOutputPath

            match result, resolvedOutputPath with
            | Ok(), Some path -> onProjectBaselined path projectKey
            | _ -> ()

            return result
        }

    /// <summary>
    /// Emits a metadata/IL/PDB delta for the given project by diffing the snapshot's typed trees
    /// and rebuilt output against the project's COMMITTED baseline. The emitted update is staged
    /// as pending: after the host applies it (<c>MetadataUpdater.ApplyUpdate</c>), call
    /// <see cref="M:FSharp.Compiler.CodeAnalysis.FSharpHotReloadSession.Commit"/> to advance the
    /// committed view, or <see cref="M:FSharp.Compiler.CodeAnalysis.FSharpHotReloadSession.Discard"/>
    /// to drop it (Roslyn's EmitSolutionUpdate/CommitSolutionUpdate/DiscardSolutionUpdate split).
    /// Returns <c>NoActiveSession</c> for projects the session does not track.
    /// </summary>
    /// <param name="projectSnapshot">The snapshot describing the edited project.</param>
    /// <param name="userOpName">An optional string used for tracing compiler operations associated with this request.</param>
    member _.EmitDelta(projectSnapshot: FSharpProjectSnapshot, ?userOpName: string) =
        async {
            ensureNotDisposed ()
            let opName = defaultArg userOpName "Unknown"

            use _ =
                Activity.start "FSharpHotReloadSession.EmitDelta" [|
                    Activity.Tags.userOpName, opName
                    Activity.Tags.project, projectSnapshot.ProjectFileName
                |]

            let projectKey = projectKeyOfSnapshot projectSnapshot

            return!
                hotReloadService.EmitHotReloadDelta
                    projectKey
                    (fun () -> parseAndCheckSnapshot projectSnapshot opName)
                    (resolveOutputPath projectKey projectSnapshot)
        }

    /// <summary>
    /// Commits ALL pending project updates atomically — the runtime applied them, so the
    /// committed per-project baselines, diff inputs and generation counters advance together
    /// (Roslyn's <c>CommitSolutionUpdate</c>). No-op when nothing is pending.
    /// </summary>
    member _.Commit() =
        ensureNotDisposed ()
        hotReloadService.CommitSession()

    /// <summary>
    /// Discards ALL pending project updates — the host did not apply them, so the next emit
    /// diffs against the unchanged committed view (Roslyn's <c>DiscardSolutionUpdate</c>).
    /// </summary>
    member _.Discard() =
        ensureNotDisposed ()
        hotReloadService.DiscardSession()

    /// <summary>
    /// Replaces the session-wide runtime capability set consulted by edit classification (for
    /// example after the target process reported its Edit and Continue capabilities). Applies to
    /// every project in the session; already-emitted deltas are unaffected.
    /// </summary>
    /// <param name="capabilities">Runtime capability names (for example <c>AddMethodToExistingType</c>); unknown names are ignored.</param>
    member _.UpdateCapabilities(capabilities: string seq) =
        ensureNotDisposed ()
        hotReloadService.SetSessionCapabilities(EditAndContinueCapabilities.Parse capabilities)

    /// <summary>
    /// Replaces the session-wide debugger-supplied active statements consulted by the next emit
    /// (the break state describes the host process, not one project). Pass an empty sequence to
    /// clear the set. Mirrors Roslyn's per-edit-session active-statement fetch, inverted to a
    /// push: the host reports the break state before emitting.
    /// </summary>
    member _.SetActiveStatements(activeStatements: FSharpManagedActiveStatementDebugInfo seq) =
        ensureNotDisposed ()
        hotReloadService.SetSessionActiveStatements(activeStatements)

    /// <summary>Identifiers of the projects the session currently tracks.</summary>
    member _.ProjectIdentifiers =
        ensureNotDisposed ()

        hotReloadService.ProjectKeys
        |> List.choose (function
            | FSharp.Compiler.HotReloadState.HotReloadProjectKey.Project(projectFileName, outputFileName) ->
                Some(ProjectSnapshot.FSharpProjectIdentifier(projectFileName, outputFileName))
            | FSharp.Compiler.HotReloadState.HotReloadProjectKey.Ambient -> None)

    /// Test seam: the per-project session view (committed baseline joined with the
    /// session-wide capability/active-statement state).
    member internal _.TryGetProjectView(projectSnapshot: FSharpProjectSnapshot) =
        hotReloadService.TryGetProjectSession(projectKeyOfSnapshot projectSnapshot)

    /// <summary>Ends the session: all per-project state is dropped. Subsequent member calls
    /// raise <see cref="T:System.ObjectDisposedException"/>. Other sessions are unaffected.</summary>
    interface IDisposable with
        member _.Dispose() =
            if Interlocked.Exchange(&disposed, 1) = 0 then
                onDispose ()
                hotReloadService.EndSession()

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

    let trimQuotes (text: string) =
        text.Trim().Trim('"')

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
            | null
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
                |> Array.tryFindIndex (fun opt -> String.Equals(opt, "-o", StringComparison.OrdinalIgnoreCase))
            with
            | Some idx when idx + 1 < otherOptions.Length ->
                otherOptions[idx + 1] |> resolveOutputPath |> Some
            | _ -> None

    let tryGetOutputPathFromProjectOptions (options: FSharpProjectOptions) =
        tryGetOutputPathFromCommandLineOptions options.ProjectFileName options.OtherOptions

    let tryGetOutputPathFromProjectSnapshot (projectSnapshot: FSharpProjectSnapshot) =
        tryGetOutputPathFromCommandLineOptions
            projectSnapshot.ProjectFileName
            (projectSnapshot.OtherOptions |> List.toArray)

    [<Literal>]
    let HotReloadTraceOutputFlagName = "FSHARP_HOTRELOAD_TRACE_OUTPUT"

    [<Literal>]
    let StableFileMaxTotalWaitMs = 5000

    [<Literal>]
    let StableFileInitialDelayMs = 25

    [<Literal>]
    let StableFileMaxBackoffMs = 200

    [<Literal>]
    let StableFileRequiredStableReads = 2

    let traceOutputFingerprint = isEnvVarTruthy HotReloadTraceOutputFlagName

    let getErrorDiagnostics (diagnostics: FSharpDiagnostic[]) =
        diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

    let waitForStableFile path =
        // Use exponential backoff: 25ms, 50ms, 100ms, 200ms, 200ms, ...
        // Total max wait ~5 seconds (vs 500ms before) for slow I/O scenarios.
        let mutable totalWaited = 0
        let mutable sleepMillis = StableFileInitialDelayMs
        let mutable stableCount = 0
        let mutable lastWrite = DateTime.MinValue
        let mutable lastSize = -1L

        while totalWaited < StableFileMaxTotalWaitMs && stableCount < StableFileRequiredStableReads do
            let exists = File.Exists path
            let currentWrite =
                if exists then File.GetLastWriteTimeUtc path else DateTime.MinValue
            let currentSize =
                if exists then FileInfo(path).Length else -1L

            if currentWrite = lastWrite && currentSize = lastSize then
                stableCount <- stableCount + 1
            else
                stableCount <- 0
                lastWrite <- currentWrite
                lastSize <- currentSize

            if stableCount < StableFileRequiredStableReads then
                Thread.Sleep sleepMillis
                totalWaited <- totalWaited + sleepMillis
                sleepMillis <- min StableFileMaxBackoffMs (sleepMillis * 2) // Exponential backoff, capped at 200ms

    let computeFileHash (path: string) : byte[] option =
        if File.Exists path then
            try
                use stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                use sha = System.Security.Cryptography.SHA256.Create()
                Some(sha.ComputeHash stream)
            with _ -> None
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
        let hashesEqual left right =
            match left, right with
            | Some x, Some y -> StructuralComparisons.StructuralEqualityComparer.Equals(x, y)
            | None, None -> true
            | _ -> false

        match previous, current with
        | Some(previousTimestamp, previousHash), Some(currentTimestamp, currentHash) ->
            let timestampChanged = previousTimestamp <> currentTimestamp
            let hashChanged = not (hashesEqual previousHash currentHash)

            if traceOutputFingerprint && timestampChanged then
                printfn $"[fsharp-hotreload][trace] detected write timestamp change for {path} (prev={previousTimestamp:O}, new={currentTimestamp:O})"

            if traceOutputFingerprint && hashChanged then
                printfn $"[fsharp-hotreload][trace] detected content hash change for {path}"

            timestampChanged || hashChanged
        | None, Some _ -> true
        | Some _, None -> true
        | None, None -> false

    let readIlModule path =
        waitForStableFile path
        let options : ILReaderOptions =
            { pdbDirPath = None
              reduceMemoryUsage = ReduceMemoryFlag.Yes
              metadataOnly = MetadataOnlyFlag.No
              tryGetMetadataSnapshot = fun _ -> None }

        use reader = OpenILModuleReader path options
        reader.ILModuleDef

    let toPublicDelta (delta: IlxDelta) : FSharpHotReloadDelta =
        { Metadata = Array.copy delta.Metadata
          IL = Array.copy delta.IL
          Pdb = delta.Pdb |> Option.map Array.copy
          UpdatedTypes = delta.UpdatedTypeTokens
          UpdatedMethods = delta.UpdatedMethodTokens
          AddedOrChangedMethods =
              delta.AddedOrChangedMethods
              |> List.map (fun info ->
                  { MethodToken = info.MethodToken
                    LocalSignatureToken = info.LocalSignatureToken
                    CodeOffset = info.CodeOffset
                    CodeLength = info.CodeLength })
          UserStringUpdates = delta.UserStringUpdates |> List.map (fun (o, n, s) -> struct (o, n, s))
          GenerationId = delta.GenerationId
          BaseGenerationId = delta.BaseGenerationId
          SequencePointUpdates = delta.SequencePointUpdates
          ActiveStatementUpdates = delta.ActiveStatementUpdates }

    let mapHotReloadError =
        function
        | HotReloadError.NoActiveSession -> FSharpHotReloadError.NoActiveSession
        | HotReloadError.NoChanges -> FSharpHotReloadError.NoChanges
        | HotReloadError.UnsupportedEdit message -> FSharpHotReloadError.UnsupportedEdit message
        | HotReloadError.DeltaEmissionException ex -> FSharpHotReloadError.DeltaEmissionFailed ex.Message

    let createBaseline (tcGlobals: TcGlobals) (ilModule: ILModuleDef) (outputPath: string) =
        let pdbPath =
            Path.ChangeExtension(outputPath, ".pdb")
            |> Option.ofObj
            |> Option.defaultValue (outputPath + ".pdb")

        let writerOptions: ILBinaryWriter.options =
            { ilg = tcGlobals.ilg
              outfile = outputPath
              pdbfile =
                if File.Exists(pdbPath) then
                    Some pdbPath
                else
                    None
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
              methodCustomDebugInfoRows = Map.empty }

        let _, pdbBytesOpt, tokenMappings, _ =
            ILBinaryWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, ilModule, id)

        let portablePdbSnapshot = pdbBytesOpt |> Option.map HotReloadPdb.createSnapshot
        let assemblyBytes = File.ReadAllBytes(outputPath)

        let baseline =
            HotReloadBaseline.createFromEmittedArtifacts
                ilModule
                tokenMappings
                assemblyBytes
                portablePdbSnapshot
                None

        // The in-memory rewrite above passes no EnC CDI side channel (methodCustomDebugInfoRows =
        // Map.empty), so its PDB never carries EnC rows. The on-disk PDB produced by the flag-on
        // build is the durable source of the baseline EnC method debug information: read it as a
        // sibling input when the rewrite yielded none (flag-off/pre-C2 PDBs still decode to the
        // empty map, and the session starts fine either way).
        let baseline =
            if Map.isEmpty baseline.EncMethodDebugInfos && File.Exists(pdbPath) then
                let baseline =
                    { baseline with
                        EncMethodDebugInfos =
                            EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb (File.ReadAllBytes(pdbPath)) }

                // Closure mapping (C6): the chain -> closure-name tables are a pure function
                // of the occurrence keys just decoded (baseline names are occurrence-derived
                // under the flag), so a session started from disk — typically in a different
                // process than the fsc that built the baseline — reconstructs exactly the
                // tables the emitting compile installed. Fail closed for pre-C6 and
                // mid-session baselines (see deriveEncClosureNamesFromEncDebugInfos).
                { baseline with
                    EncClosureNames = HotReloadBaseline.deriveEncClosureNames ilModule baseline }
            else
                baseline

        // Phase G sibling-read: the IL module is read back from disk WITHOUT debug points, so
        // the in-memory rewrite's PDB decodes to an empty sequence-point view. The on-disk PDB
        // written by the build is the real source of the committed lines that line-shift
        // detection and active-statement remapping diff against.
        if Map.isEmpty baseline.SequencePointSnapshots && File.Exists(pdbPath) then
            { baseline with
                SequencePointSnapshots =
                    FSharp.Compiler.HotReload.ActiveStatementAnalysis.decodeMethodSequencePoints (
                        File.ReadAllBytes(pdbPath)
                    ) }
        else
            baseline

    let toHotReloadImplementationSnapshot (typedImplFiles: CheckedImplFile list) : CheckedAssemblyAfterOptimization =
        typedImplFiles
        |> List.map (fun implFile ->
            { CheckedImplFileAfterOptimization.ImplFile = implFile
              OptimizeDuringCodeGen = fun _ expr -> expr })
        |> CheckedAssemblyAfterOptimization

    let typedImplementationFilesGetterMethod =
        typeof<FSharpCheckProjectResults>.GetMethod(
            "get_TypedImplementationFiles",
            BindingFlags.Instance ||| BindingFlags.NonPublic)

    let getTypedImplementationFilesViaReflection (projectResults: FSharpCheckProjectResults) =
        if obj.ReferenceEquals(typedImplementationFilesGetterMethod, null) then
            invalidOp "Could not resolve get_TypedImplementationFiles on FSharpCheckProjectResults."

        let tupleFields =
            typedImplementationFilesGetterMethod.Invoke(projectResults, [||])
            |> FSharpValue.GetTupleFields

        let tcGlobals = tupleFields[0] :?> TcGlobals
        let typedImplFiles = tupleFields[3] :?> CheckedImplFile list
        tcGlobals, typedImplFiles

    let getHotReloadDiffInputs (projectResults: FSharpCheckProjectResults) =
        // Use non-optimized typed implementation trees for symbol diffing so method-body edits
        // keep user-authored identities (Roslyn parity), while IL deltas still come from built output.
        let tcGlobals, typedImplFiles = getTypedImplementationFilesViaReflection projectResults
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

    // Creating a checker resets the process-local capture slot (the module store the fsc emit
    // hook publishes flag-on baseline captures into). This preserves the retired default-store
    // registration's freshness property: a freshly created checker never observes (or chains
    // capture naming against) another owner's stale captures, while consecutive captures with
    // NO checker creation in between (one logical host) still chain. Session entities are
    // unaffected: they own private stores and reconstruct baselines from disk artifacts.
    do FSharp.Compiler.HotReloadState.clearSessionState ()

    // Projects tracked by LIVE session entities created via CreateHotReloadSession, keyed by
    // the resolved output path each AddProject baselined (most recent first). Compile consults
    // this to resolve the scoped emission context — which session, and which project inside
    // it, a given in-process compile serves. Disposing a session removes its entries.
    let liveHotReloadEmissionTargets =
        ResizeArray<
            string *
            FSharp.Compiler.HotReloadState.HotReloadSessionStore *
            FSharp.Compiler.HotReloadState.HotReloadProjectKey
         >()

    let liveHotReloadEmissionTargetsGate = obj ()

    let normalizeOutputPathForEmissionTargets (path: string) =
        try
            Path.GetFullPath(path)
        with _ ->
            path

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
                    if String.Equals(registeredPath, target, StringComparison.OrdinalIgnoreCase) then
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
            (fun () -> unregisterHotReloadEmissionTargets sessionStore)
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

        let hasEnableArgument (feature: string) (argv: string[]) =
            let inlineEnabled =
                argv
                |> Array.exists (fun arg ->
                    arg.StartsWith("--enable:", StringComparison.OrdinalIgnoreCase)
                    && arg.Substring("--enable:".Length).Equals(feature, StringComparison.OrdinalIgnoreCase))

            let splitEnabled =
                argv
                |> Array.pairwise
                |> Array.exists (fun (arg, value) ->
                    arg.Equals("--enable", StringComparison.OrdinalIgnoreCase)
                    && value.Equals(feature, StringComparison.OrdinalIgnoreCase))

            inlineEnabled || splitEnabled

        // A non-capture compile of a project tracked by a live session entity is scoped to
        // that session: the emit hook then replays the session's chained closure-name and
        // synthesized-name state into this compile. Capture compiles stay session-independent
        // (they publish to the process-local capture slot, never to a session's store).
        let emissionContext =
            if hasEnableArgument "hotreloaddeltas" argv then
                None
            else
                tryResolveHotReloadEmissionContext (tryGetOutputPathFromCommandLineOptions "" argv)

        let ensureHotReloadSessionHookArgument (argv: string[]) =
            // Keep synthesized-name replay active for checker-owned hot reload sessions even when
            // callers intentionally compile updates without --enable:hotreloaddeltas.
            if
                emissionContext.IsSome
                && not (hasEnableArgument "hotreloaddeltas" argv)
                && not (hasEnableArgument "hotreloadhook" argv)
            then
                Array.append argv [| "--enable:hotreloadhook" |]
            else
                argv

        let argv = ensureHotReloadSessionHookArgument argv

        async {
            let ctok = CompilationThreadToken()

            match emissionContext with
            | Some context ->
                FSharp.Compiler.HotReloadState.setCurrentEmissionContext (Some context)

                try
                    return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
                finally
                    FSharp.Compiler.HotReloadState.setCurrentEmissionContext None
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

    static member internal CreateOverloadCacheMetricsListener() =
        new CacheMetrics.CacheMetricsListener("overloadResolutionCache")

    member internal _.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

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
