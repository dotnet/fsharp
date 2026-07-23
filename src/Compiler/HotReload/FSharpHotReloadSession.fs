// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections.Generic
open System.IO
open System.Threading
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.HotReload.DeltaBuilder
open FSharp.Compiler.IlxDeltaEmitter
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.EnvironmentHelpers

/// A single structured rude-edit diagnostic surfaced when an edit cannot be applied.
/// Carried (rather than flattened to a string) so the host can report the reason and act on
/// severity. The Id is the F#-owned FSHRDL* code from RudeEditDiagnostics.diagnosticId.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpHotReloadRudeEdit =
    {
        Id: string
        Severity: FSharpDiagnosticSeverity
        Message: string
        SymbolName: string option
    }

[<RequireQualifiedAccess>]
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpHotReloadError =
    | NoActiveSession
    | NoChanges
    | MissingOutputPath
    | UnsupportedEdit of FSharpHotReloadRudeEdit list
    | CompilationFailed of FSharpDiagnostic[]
    | DeltaEmissionFailed of string

/// Projects the internal structured rude-edit diagnostics onto the public surface.
module internal FSharpHotReloadRudeEditMapping =
    let ofDiagnostic (d: RudeEditDiagnostic) : FSharpHotReloadRudeEdit =
        {
            Id = d.Id
            Severity = d.Severity
            Message = d.Message
            SymbolName = d.SymbolName
        }

    let ofDiagnostics (diagnostics: RudeEditDiagnostic list) = diagnostics |> List.map ofDiagnostic

/// File-system operations whose failure behavior is part of the hot reload session contract.
module internal FSharpHotReloadFileSystem =

    [<Literal>]
    let private StableFileMaxTotalWaitMs = 5000

    [<Literal>]
    let private StableFileInitialDelayMs = 25

    [<Literal>]
    let private StableFileMaxBackoffMs = 200

    [<Literal>]
    let private StableFileRequiredStableReads = 2

    /// Waits until an output file has the same timestamp and size on consecutive reads.
    let waitForStableFile path =
        // Use exponential backoff: 25ms, 50ms, 100ms, 200ms, 200ms, ...
        // Total max wait ~5 seconds for slow I/O scenarios.
        let mutable totalWaited = 0
        let mutable sleepMillis = StableFileInitialDelayMs
        let mutable stableCount = 0
        let mutable lastWrite = DateTime.MinValue
        let mutable lastSize = -1L

        while totalWaited < StableFileMaxTotalWaitMs
              && stableCount < StableFileRequiredStableReads do
            let exists = File.Exists path

            let currentWrite =
                if exists then
                    File.GetLastWriteTimeUtc path
                else
                    DateTime.MinValue

            let currentSize = if exists then FileInfo(path).Length else -1L

            // A missing path uses the sentinel timestamp/size too, so existence must be
            // part of the invariant or a file that has not materialized looks stable.
            if exists && currentWrite = lastWrite && currentSize = lastSize then
                stableCount <- stableCount + 1
            else
                stableCount <- 0
                lastWrite <- currentWrite
                lastSize <- currentSize

            if stableCount < StableFileRequiredStableReads then
                Thread.Sleep sleepMillis
                totalWaited <- totalWaited + sleepMillis
                sleepMillis <- min StableFileMaxBackoffMs (sleepMillis * 2)

    /// Reads optional debug data, returning None for an ordinary unreadable-file failure.
    let tryReadAllBytes (readAllBytes: string -> byte[]) path =
        try
            Some(readAllBytes path)
        with
        | :? IOException
        | :? UnauthorizedAccessException
        | :? System.Security.SecurityException
        | :? NotSupportedException -> None

[<System.Flags>]
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpHotReloadCapability =
    | None = 0
    | Il = 1
    | Metadata = 2
    | PortablePdb = 4
    | MultipleGenerations = 8
    | RuntimeApply = 16

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpHotReloadCapabilities internal (flags: FSharpHotReloadCapability) =
    member _.Flags = flags
    member _.SupportsIl = flags.HasFlag(FSharpHotReloadCapability.Il)
    member _.SupportsMetadata = flags.HasFlag(FSharpHotReloadCapability.Metadata)
    member _.SupportsPortablePdb = flags.HasFlag(FSharpHotReloadCapability.PortablePdb)

    member _.SupportsMultipleGenerations =
        flags.HasFlag(FSharpHotReloadCapability.MultipleGenerations)

    member _.SupportsRuntimeApply = flags.HasFlag(FSharpHotReloadCapability.RuntimeApply)

    static member internal FromInternalFlags(flags: HotReloadCapabilityFlags) =
        let casted = enum<FSharpHotReloadCapability> (int flags)
        FSharpHotReloadCapabilities(casted)

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpAddedOrChangedMethodInfo =
    {
        MethodToken: int
        LocalSignatureToken: int
        CodeOffset: int
        CodeLength: int
    }

[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpHotReloadDelta =
    {
        Metadata: byte[]
        IL: byte[]
        Pdb: byte[] option
        UpdatedTypes: int list
        UpdatedMethods: int list
        /// Runtime capabilities the host must verify before applying this delta.
        RequiredCapabilities: string list
        AddedOrChangedMethods: FSharpAddedOrChangedMethodInfo list
        UserStringUpdates: struct (int * int * string) list
        GenerationId: Guid
        BaseGenerationId: Guid
        SequencePointUpdates: FSharpSequencePointUpdates list
        ActiveStatementUpdates: FSharpActiveStatementRemapResult list
    }

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
        hasOutputFingerprintChanged: string -> (DateTime * byte[] option) option -> (DateTime * byte[] option) option -> bool,
        toPublicDelta: IlxDelta -> FSharpHotReloadDelta,
        mapHotReloadError: HotReloadError -> FSharpHotReloadError
    ) =

    let hotReloadGate = obj ()

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

    let loadSynthesizedNameSnapshot (baseline: FSharpEmitBaseline) (map: FSharpSynthesizedTypeMaps) =
        let snapshot =
            baseline.SynthesizedNameSnapshot
            |> Map.toSeq
            |> Seq.map (fun (k, v) -> struct (k, v))

        match baseline.SynthesizedNameSnapshotSource with
        | SynthesizedNameSnapshotSource.Recorded -> map.LoadRecordedSnapshot snapshot
        | SynthesizedNameSnapshotSource.Reconstructed -> map.LoadSnapshot snapshot

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

                    let baselineResult: Result<_, FSharpHotReloadError> =
                        try
                            let ilModule = readIlModule outputPath
                            let baseline = createBaseline tcGlobals ilModule outputPath
                            Ok(baseline, implementationFiles)
                        with ex ->
                            Result.Error(FSharpHotReloadError.DeltaEmissionFailed($"Failed to create hot reload baseline: {ex.Message}"))

                    match baselineResult with
                    | Result.Error error -> return Result.Error error
                    | Ok(baseline, implementationFiles) ->
                        lock hotReloadGate (fun () ->
                            // Closure mapping: the per-method occurrence-chain -> closure-name
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

                                 // A conflicting reconstruction would install names that do not
                                 // describe the running generation, so fail closed in Release too.
                                 raise (
                                     InvalidOperationException(
                                         "Hot reload closure-name reconstruction from disk disagrees with the in-process capture session being replaced."
                                     )
                                 )
                             | _ -> ())

                            let compilerState = tcGlobals.CompilerGlobalState.Value

                            let map =
                                let targetMap = getOrCreateSynthesizedTypeMap projectKey

                                loadSynthesizedNameSnapshot baseline targetMap
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

                        return Result.Ok()
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
        // True when the session observed a change to a tracked non-source on-disk input
        // (embedded resource, key file, ...) since the committed baseline. Such a change does
        // not surface as a symbol edit, but it still requires a rebuilt output assembly before
        // a delta can be emitted.
        (trackedInputsChanged: bool)
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

                                    loadSynthesizedNameSnapshot restoredSession.Baseline map
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
                                        computeSymbolChanges tcGlobals session.Capabilities session.ImplementationFiles implementationFiles

                                    match mapSymbolChangesToDelta session.Baseline symbolChanges with
                                    | Error mappingErrors ->
                                        Some(
                                            FSharpHotReloadError.UnsupportedEdit(
                                                mappingErrors
                                                |> List.map (
                                                    RudeEditDiagnostics.unsupported >> FSharpHotReloadRudeEditMapping.ofDiagnostic
                                                )
                                            )
                                        )
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

                                        if
                                            (hasUpdates || trackedInputsChanged)
                                            && not (hasOutputFingerprintChanged outputPath committedFingerprint outputFingerprint)
                                        then
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
                            let ilModuleResult: Result<_, FSharpHotReloadError> =
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

                                        setCompilerGeneratedNameMap
                                            (tcGlobals.CompilerGlobalState.Value :> obj)
                                            (map :> ICompilerGeneratedNameMap)
                                    | false, _ -> ())

                                // Sequence-point tracking: the fresh compile's on-disk PDB carries the sequence
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
                                        FSharpHotReloadFileSystem.tryReadAllBytes File.ReadAllBytes pdbPath
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
                                    if
                                        result.Delta.UpdatedBaseline.IsSome
                                        || not (List.isEmpty result.Delta.SequencePointUpdates)
                                    then
                                        lock hotReloadGate (fun () -> pendingOutputFingerprints[projectKey] <- outputFingerprint)

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
    let lifecycleGate = obj ()

    // Output paths explicitly supplied to AddProject, consulted before deriving the path from
    // the snapshot's command-line options (hosts whose snapshots carry no -o option).
    let outputPathOverrides =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, string>()

    let outputPathGate = obj ()

    // Tracked non-source on-disk inputs (embedded resources, key files, ...) observed when each
    // project's baseline was committed. EmitDelta compares the current on-disk state against
    // this snapshot: a tracked input change is not visible as a symbol edit, so it must force
    // the stale-build-output check even when the typed trees are unchanged. Staged/committed in
    // lockstep with the emit pipeline's pending/committed split (Commit promotes, Discard drops).
    let committedTrackedInputs =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, FSharp.Compiler.HotReload.TrackedInputs.TrackedInput list>()

    let pendingTrackedInputs =
        Dictionary<FSharp.Compiler.HotReloadState.HotReloadProjectKey, FSharp.Compiler.HotReload.TrackedInputs.TrackedInput list>()

    let trackedInputsGate = obj ()

    let computeTrackedInputs (projectSnapshot: FSharpProjectSnapshot) =
        FSharp.Compiler.HotReload.TrackedInputs.compute projectSnapshot.ProjectFileName projectSnapshot.ProjectConfig.OtherOptions

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
        if Volatile.Read(&disposed) = 1 then
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
                Activity.start
                    "FSharpHotReloadSession.AddProject"
                    [|
                        Activity.Tags.userOpName, opName
                        Activity.Tags.project, projectSnapshot.ProjectFileName
                    |]

            let projectKey = projectKeyOfSnapshot projectSnapshot

            match outputPath with
            | Some path -> lock outputPathGate (fun () -> outputPathOverrides[projectKey] <- path)
            | None -> ()

            let resolvedOutputPath = resolveOutputPath projectKey projectSnapshot

            // Observe tracked inputs before the baseline capture so an input edited while the
            // capture runs is seen as changed by the next emit rather than silently absorbed.
            let trackedInputs = computeTrackedInputs projectSnapshot

            let! result =
                hotReloadService.AddHotReloadProject projectKey (fun () -> parseAndCheckSnapshot projectSnapshot opName) resolvedOutputPath

            match result, resolvedOutputPath with
            | Ok(), Some path ->
                // AddProject completes asynchronously. Serialize its final registration with
                // Dispose so an in-flight capture cannot resurrect a disposed session.
                lock lifecycleGate (fun () ->
                    ensureNotDisposed ()

                    lock trackedInputsGate (fun () ->
                        committedTrackedInputs[projectKey] <- trackedInputs
                        pendingTrackedInputs.Remove projectKey |> ignore)

                    onProjectBaselined path projectKey)
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
                Activity.start
                    "FSharpHotReloadSession.EmitDelta"
                    [|
                        Activity.Tags.userOpName, opName
                        Activity.Tags.project, projectSnapshot.ProjectFileName
                    |]

            let projectKey = projectKeyOfSnapshot projectSnapshot

            let currentTrackedInputs = computeTrackedInputs projectSnapshot

            let trackedInputsChanged =
                lock trackedInputsGate (fun () ->
                    match committedTrackedInputs.TryGetValue projectKey with
                    | true, committed -> committed <> currentTrackedInputs
                    | false, _ -> false)

            let! result =
                hotReloadService.EmitHotReloadDelta
                    projectKey
                    (fun () -> parseAndCheckSnapshot projectSnapshot opName)
                    (resolveOutputPath projectKey projectSnapshot)
                    trackedInputsChanged

            match result with
            | Ok _ ->
                // Stage the observed tracked-input state with the emitted update; Commit
                // promotes it alongside the pending baseline, Discard drops it so the next
                // emit re-compares against the unchanged committed view.
                lock trackedInputsGate (fun () -> pendingTrackedInputs[projectKey] <- currentTrackedInputs)
            | Error _ -> ()

            return result
        }

    /// <summary>
    /// Commits ALL pending project updates atomically — the runtime applied them, so the
    /// committed per-project baselines, diff inputs and generation counters advance together
    /// (Roslyn's <c>CommitSolutionUpdate</c>). No-op when nothing is pending.
    /// </summary>
    member _.Commit() =
        ensureNotDisposed ()

        lock trackedInputsGate (fun () ->
            for entry in List.ofSeq pendingTrackedInputs do
                committedTrackedInputs[entry.Key] <- entry.Value

            pendingTrackedInputs.Clear())

        hotReloadService.CommitSession()

    /// <summary>
    /// Discards ALL pending project updates — the host did not apply them, so the next emit
    /// diffs against the unchanged committed view (Roslyn's <c>DiscardSolutionUpdate</c>).
    /// </summary>
    member _.Discard() =
        ensureNotDisposed ()
        lock trackedInputsGate (fun () -> pendingTrackedInputs.Clear())
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
                lock lifecycleGate (fun () ->
                    onDispose ()
                    hotReloadService.EndSession())
