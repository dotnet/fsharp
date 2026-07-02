namespace FSharp.Compiler.HotReload

open System
open System.IO
open FSharp.Compiler
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxDeltaEmitter
open FSharp.Compiler.HotReload.DeltaBuilder
open FSharp.Compiler.TypedTree
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.EnvironmentHelpers

/// <summary>
/// Entry point mirroring Roslyn's <c>EditAndContinueLanguageService</c>. It centralises session lifecycle
/// management so callers do not talk to <see cref="HotReloadState"/> directly.
/// </summary>
type internal FSharpEditAndContinueLanguageService private (getSessionStore: unit -> HotReloadState.HotReloadSessionStore) =

    static let lazyInstance =
        lazy FSharpEditAndContinueLanguageService(fun () -> FSharp.Compiler.HotReloadState.getSessionStore ())

    let sessionStore () = getSessionStore ()
    static let traceMetadataFlagName = "FSHARP_HOTRELOAD_TRACE_METADATA"

    static let traceMethodsFlagName = "FSHARP_HOTRELOAD_TRACE_METHODS"

    // Keep hot reload activity tags local so Activity.fsi stays main-compatible.
    static let activityTagGeneration = "generation"

    static let activityTagHotReloadAction = "hotReloadAction"

    static let shouldTraceMetadata () = isEnvVarTruthy traceMetadataFlagName

    static let shouldTraceMethods () = isEnvVarTruthy traceMethodsFlagName

    static let dedupeMethodKeys (keys: MethodDefinitionKey list) =
        let seen = Collections.Generic.HashSet<MethodDefinitionKey>(HashIdentity.Structural)

        keys
        |> List.fold (fun acc key -> if seen.Add key then key :: acc else acc) []
        |> List.rev

    static let tryGetStartupRoot (declaringType: string) =
        let markerIndex = declaringType.IndexOf("@hotreload", StringComparison.Ordinal)

        if markerIndex <= 0 then
            None
        else
            let prefix = declaringType.Substring(0, markerIndex)
            let lastDot = prefix.LastIndexOf('.')

            if lastDot > 0 then
                Some(prefix.Substring(0, lastDot))
            else
                None

    // Roslyn parity intent: preserve user-authored method identity first, then include
    // compiler-generated companion methods tied to the same startup scope so transformed
    // CE/async output shapes apply in place.
    static let augmentWithCompilerGeneratedCompanions (baseline: FSharpEmitBaseline) (updatedMethods: MethodDefinitionKey list) =
        let baselineMethods =
            baseline.MethodTokens |> Map.toSeq |> Seq.map fst |> Seq.toArray

        let globalStartupRoots =
            baselineMethods
            |> Array.choose (fun candidate -> tryGetStartupRoot candidate.DeclaringType)
            |> Array.distinct

        let companions =
            updatedMethods
            |> List.collect (fun updatedMethod ->
                let marker = updatedMethod.Name + "@hotreload"

                let directMatches =
                    baselineMethods
                    |> Array.choose (fun candidate ->
                        let matchesDeclaringType =
                            candidate.DeclaringType.IndexOf(marker, StringComparison.Ordinal) >= 0

                        let matchesMethodName = candidate.Name.StartsWith(marker, StringComparison.Ordinal)

                        if matchesDeclaringType || matchesMethodName then
                            Some candidate
                        else
                            None)

                let startupRoots =
                    let directRoots =
                        directMatches
                        |> Array.choose (fun candidate -> tryGetStartupRoot candidate.DeclaringType)
                        |> Array.distinct

                    if directRoots.Length > 0 then
                        directRoots
                    else
                        globalStartupRoots

                baselineMethods
                |> Array.choose (fun candidate ->
                    let isCompilerGeneratedCompanion =
                        candidate.DeclaringType.IndexOf("@hotreload", StringComparison.Ordinal) >= 0

                    let inTransitiveScope =
                        startupRoots
                        |> Array.exists (fun root -> candidate.DeclaringType.StartsWith(root + ".", StringComparison.Ordinal))

                    if isCompilerGeneratedCompanion && inTransitiveScope then
                        Some candidate
                    else
                        None)
                |> Array.toList)

        let augmented = dedupeMethodKeys (updatedMethods @ companions)

        if shouldTraceMethods () && not (List.isEmpty companions) then
            let names =
                companions
                |> List.map (fun key -> $"{key.DeclaringType}::{key.Name}")
                |> String.concat ", "

            printfn "[fsharp-hotreload][service] compiler-generated companion methods selected: %s" names

        augmented

    static let createSynthesizedMapFromSnapshot (snapshot: Map<string, string[]>) =
        let map = FSharpSynthesizedTypeMaps()
        map.LoadSnapshot(snapshot |> Map.toSeq |> Seq.map (fun (k, v) -> struct (k, v)))
        map.BeginSession()
        map

    new(sessionStore: HotReloadState.HotReloadSessionStore) = FSharpEditAndContinueLanguageService(fun () -> sessionStore)

    /// <summary>Singleton instance consumed by CLI and IDE hosts.</summary>
    static member Instance = lazyInstance.Value

    /// <summary>
    /// Initialise or replace the current baseline and reset the generation counters. When the host
    /// negotiated runtime capabilities, pass them via <paramref name="capabilities"/>; otherwise the
    /// session defaults to <see cref="EditAndContinueCapabilities.BaselineOnly"/>. Replaces the
    /// WHOLE session (legacy single-session semantics); pass <paramref name="projectKey"/> to
    /// record the project identity the new session is keyed by.
    /// </summary>
    member _.StartSession
        (baseline: FSharpEmitBaseline, ?capabilities: EditAndContinueCapabilities, ?projectKey: HotReloadState.HotReloadProjectKey)
        : HotReloadState.HotReloadSessionStart =
        use _ =
            Activity.start
                "HotReload.StartSession"
                [|
                    Activity.Tags.project, baseline.ModuleId.ToString()
                    activityTagHotReloadAction, "baseline"
                |]

        sessionStore().SetBaseline(baseline, CheckedAssemblyAfterOptimization [], ?capabilities = capabilities, ?key = projectKey)

    /// <summary>Initialise or replace the current baseline together with its typed implementation files.</summary>
    member _.StartSession
        (
            baseline: FSharpEmitBaseline,
            implementationFiles: CheckedAssemblyAfterOptimization,
            ?capabilities: EditAndContinueCapabilities,
            ?projectKey: HotReloadState.HotReloadProjectKey
        ) : HotReloadState.HotReloadSessionStart =
        use _ =
            Activity.start
                "HotReload.StartSession"
                [|
                    Activity.Tags.project, baseline.ModuleId.ToString()
                    activityTagHotReloadAction, "baseline+impl"
                |]

        sessionStore().SetBaseline(baseline, implementationFiles, ?capabilities = capabilities, ?key = projectKey)

    /// <summary>
    /// Adds (or recaptures) the baseline of ONE project without replacing the other projects in
    /// the session or its session-wide capability/active-statement state (the session-entity
    /// semantic; Roslyn analog: one more per-module <c>EmitBaseline</c> in a DebuggingSession).
    /// </summary>
    member _.AddProject
        (projectKey: HotReloadState.HotReloadProjectKey, baseline: FSharpEmitBaseline, implementationFiles: CheckedAssemblyAfterOptimization) : HotReloadState.HotReloadSessionStart =
        use _ =
            Activity.start
                "HotReload.AddProject"
                [|
                    Activity.Tags.project, baseline.ModuleId.ToString()
                    activityTagHotReloadAction, "baseline+impl"
                |]

        sessionStore().AddProject(projectKey, baseline, implementationFiles)

    /// <summary>Attempts to fetch the current baseline.</summary>
    member _.TryGetBaseline() = sessionStore().TryGetBaseline()

    /// <summary>Attempts to fetch the current session (baseline + generation metadata).</summary>
    member _.TryGetSession(?projectKey: HotReloadState.HotReloadProjectKey) =
        sessionStore().TryGetSession(?key = projectKey)

    /// <summary>Attempts to restore the active session from the last committed snapshot.</summary>
    member _.TryRestoreSession(?projectKey: HotReloadState.HotReloadProjectKey) =
        sessionStore().TryRestoreSession(?key = projectKey)

    /// <summary>Updates the stored EncId after a successful delta application.</summary>
    member _.OnDeltaApplied(generationId: Guid) =
        sessionStore().RecordDeltaApplied(generationId)

    /// <summary>
    /// Replaces the runtime capability set consulted by edit classification for the active
    /// session. Safe at any point: capabilities are read per-emit. Returns false when no
    /// session is active.
    /// </summary>
    member _.UpdateCapabilities(capabilities: EditAndContinueCapabilities) =
        sessionStore().UpdateCapabilities(capabilities)

    /// <summary>Clears the session, typically when hot reload is disabled or the build finishes.</summary>
    member _.EndSession() = sessionStore().ClearBaseline()

    /// <summary>Clears both active and restorable session state.</summary>
    member _.ResetSessionState() = sessionStore().ClearSessionState()

    /// <summary>
    /// Emits a delta for the supplied request; callers may commit the delta by invoking <see cref="OnDeltaApplied"/>.
    /// <paramref name="freshDebugPdb"/> optionally carries the fresh compile's on-disk portable PDB:
    /// modules read back from disk have no debug points, so the sibling PDB is the real
    /// source of the fresh sequence points for line-shift detection, active-statement remapping and
    /// the emitted PDB delta.
    /// </summary>
    member _.EmitDelta(request: DeltaEmissionRequest, ?freshDebugPdb: byte[], ?projectKey: HotReloadState.HotReloadProjectKey) =
        let trace = shouldTraceMetadata ()

        if trace then
            let asm = typeof<FSharpEditAndContinueLanguageService>.Assembly

            let message =
                $"[fsharp-hotreload][service] EmitDelta invoked (assembly={asm.Location})\n"

            printf "%s" message

            try
                let path = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-service.log")
                File.AppendAllText(path, message)
            with :? IOException as ex ->
                eprintfn "[fsharp-hotreload][service] Failed to write trace log: %s" ex.Message

        match sessionStore().TryGetSession(?key = projectKey) with
        | ValueNone -> Error HotReloadError.NoActiveSession
        | ValueSome session ->
            use _ =
                Activity.start
                    "HotReload.EmitDelta"
                    [|
                        activityTagGeneration, string session.CurrentGeneration
                        Activity.Tags.project, session.Baseline.ModuleId.ToString()
                    |]

            try
                if trace then
                    printfn
                        "[fsharp-hotreload][service] session prev=%A baselineEncId=%O"
                        session.PreviousGenerationId
                        session.Baseline.EncId

                let synthesizedMap =
                    createSynthesizedMapFromSnapshot session.Baseline.SynthesizedNameSnapshot

                let deltaRequest =
                    {
                        IlxDeltaRequest.Baseline = session.Baseline
                        UpdatedTypes = request.UpdatedTypes
                        UpdatedMethods = request.UpdatedMethods
                        UpdatedAccessors = request.UpdatedAccessors
                        Module = request.IlModule
                        SymbolChanges = request.SymbolChanges
                        CurrentGeneration = session.CurrentGeneration
                        PreviousGenerationId = session.PreviousGenerationId
                        SynthesizedNames = Some synthesizedMap
                    }

                let delta =
                    FSharp.Compiler.IlxDeltaEmitter.emitDeltaWithDebugData freshDebugPdb deltaRequest

                if trace then
                    let line = $"[fsharp-hotreload][service] EmitDelta produced encLog={delta.EncLog}\n"
                    printf "%s" line

                    try
                        let path = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-service.log")
                        File.AppendAllText(path, line)
                    with :? IOException as ex ->
                        eprintfn "[fsharp-hotreload][service] Failed to write trace log: %s" ex.Message

                // Active statements: remap the debugger-supplied active statements against the emitted
                // delta BEFORE any session state is staged — a rude remap (an edit that destroys
                // an active statement or changes the statement a non-leaf frame is suspended in)
                // blocks the whole update, leaving the session at the previous generation.
                let activeStatementRemap =
                    let recompiledTokens =
                        delta.AddedOrChangedMethods
                        |> List.map (fun info -> info.MethodToken)
                        |> Set.ofList

                    ActiveStatementAnalysis.remapActiveStatements
                        session.Baseline.SequencePointSnapshots
                        delta.ChainedSequencePoints
                        recompiledTokens
                        session.ActiveStatements

                match activeStatementRemap with
                | Error rudeMessages ->
                    let details = String.concat Environment.NewLine rudeMessages

                    Error(
                        HotReloadError.UnsupportedEdit
                            [
                                RudeEditDiagnostics.unsupported (
                                    $"Updating an active statement requires restarting the application.{Environment.NewLine}{details}"
                                )
                            ]
                    )
                | Ok activeStatementResults ->

                    let delta =
                        { delta with
                            ActiveStatementUpdates = activeStatementResults
                        }

                    let delta =
                        match delta.UpdatedBaseline with
                        | Some updatedBaseline ->
                            // Generation chaining: replace the updated methods' EnC debug
                            // information with the data recomputed from the fresh typed tree.
                            // The delta PDB does not carry EnC CDI rows, so this in-memory chain
                            // is what the closure mapping consumes.
                            let updatedMethodTokens =
                                delta.AddedOrChangedMethods |> List.map (fun info -> info.MethodToken)

                            let chainedBaseline =
                                chainEncMethodDebugInfos updatedBaseline request.RefreshedEncDebugInfos updatedMethodTokens

                            // Closure mapping: chain the refreshed occurrence-chain ->
                            // closure-name tables forward with the same replace-or-drop
                            // semantics, so the NEXT delta compile's allocator sees this
                            // generation's names for the updated methods.
                            let chainedBaseline =
                                chainClosureNameRows chainedBaseline request.RefreshedClosureNameRows updatedMethodTokens

                            if trace then
                                printfn
                                    "[fsharp-hotreload][service] staging pending baseline encId=%O baseId=%O newBaselineEncId=%O"
                                    delta.GenerationId
                                    delta.BaseGenerationId
                                    chainedBaseline.EncId

                            sessionStore().UpdateBaseline(chainedBaseline, ?key = projectKey)

                            { delta with
                                UpdatedBaseline = Some chainedBaseline
                            }
                        | None -> delta

                    Ok { Delta = delta }
            with
            | HotReloadUnsupportedEditException message -> Error(HotReloadError.UnsupportedEdit [ RudeEditDiagnostics.unsupported message ])
            | ex -> Error(HotReloadError.DeltaEmissionException ex)

    /// <summary>Returns <c>true</c> if a hot reload session is active.</summary>
    member _.IsSessionActive = sessionStore().TryGetSession().IsSome

    /// <summary>Convenience helper that both emits and commits a delta when the request succeeds.</summary>
    member this.EmitAndCommitDelta(request: DeltaEmissionRequest) =
        match this.EmitDelta(request) with
        | Ok result ->
            this.OnDeltaApplied(result.Delta.GenerationId)
            Ok result
        | Error error -> Error error

    member this.EmitDeltaForCompilation
        (
            tcGlobals: TcGlobals,
            updatedImplementation: CheckedAssemblyAfterOptimization,
            ilModule: ILModuleDef,
            ?freshDebugPdb: byte[],
            ?projectKey: HotReloadState.HotReloadProjectKey,
            ?deferCommit: bool
        ) : Result<DeltaEmissionResult, HotReloadError> =
        // Deferred commit (the session-entity flow): stage everything as the project's pending
        // update and let the host commit/discard all projects together (Roslyn's
        // EmitSolutionUpdate/CommitSolutionUpdate split). The legacy flow commits immediately.
        let deferCommit = defaultArg deferCommit false

        // Restore from the last committed snapshot before emitting if an overlapping
        // compile cleared the currently active session.
        let sessionOpt = sessionStore().TryRestoreSession(?key = projectKey)

        match sessionOpt with
        | ValueNone -> Error HotReloadError.NoActiveSession
        | ValueSome session ->
            use _ =
                Activity.start
                    "HotReload.EmitDeltaForCompilation"
                    [|
                        activityTagGeneration, string session.CurrentGeneration
                        Activity.Tags.project, session.Baseline.ModuleId.ToString()
                    |]

            let symbolChanges =
                computeSymbolChanges tcGlobals session.Capabilities session.ImplementationFiles updatedImplementation

            if not (List.isEmpty symbolChanges.RudeEdits) then
                // Carry the per-edit structured diagnostics (Id + Severity + Message, including
                // which runtime capability is missing for RudeEditKind.NotSupportedByRuntime) so
                // hosts can report an actionable reason instead of a pre-flattened string.
                Error(HotReloadError.UnsupportedEdit(RudeEditDiagnostics.ofRudeEdits symbolChanges.RudeEdits))
            elif not (List.isEmpty symbolChanges.Deleted) then
                Error(
                    HotReloadError.UnsupportedEdit
                        [
                            RudeEditDiagnostics.unsupported "Deleted symbols detected; full rebuild required."
                        ]
                )
            else
                match mapSymbolChangesToDelta session.Baseline symbolChanges with
                | Error mappingErrors -> Error(HotReloadError.UnsupportedEdit(mappingErrors |> List.map RudeEditDiagnostics.unsupported))
                | Ok(updatedTypes, updatedMethods, accessorUpdates) ->
                    let updatedMethods =
                        augmentWithCompilerGeneratedCompanions session.Baseline updatedMethods

                    // Insert-only edits (for example, adding an allowed non-virtual method) may not produce
                    // method-body updates, but still need to flow to IlxDeltaEmitter so new MethodDef rows are emitted.
                    let hasUpdates =
                        not (List.isEmpty updatedTypes)
                        || not (List.isEmpty updatedMethods)
                        || not (List.isEmpty accessorUpdates)
                        || not (List.isEmpty symbolChanges.Added)

                    // Sequence-point tracking: even when the typed-tree diff found no semantic edits (its hashes are
                    // deliberately range-independent), the fresh compile's sequence points may have
                    // moved — a line-shift edit (blank line/comment above a method). The emitter
                    // detects those by diffing sequence points against the committed snapshot, so
                    // emission proceeds and "no changes" is decided from the emitted artifacts
                    // (Roslyn parity: line-only document changes are significant valid changes).
                    let request: DeltaEmissionRequest =
                        {
                            IlModule = ilModule
                            UpdatedTypes = updatedTypes
                            UpdatedMethods = updatedMethods
                            UpdatedAccessors = accessorUpdates
                            SymbolChanges = Some symbolChanges
                            // Recomputed occurrence data from the fresh typed tree; EmitDelta
                            // chains it into the next-generation baseline for the updated methods.
                            RefreshedEncDebugInfos =
                                computeRefreshedEncMethodDebugInfosWithScope
                                    tcGlobals
                                    session.Baseline
                                    ImplementationFileScope.ReferenceChanged
                                    (Some session.ImplementationFiles)
                                    updatedImplementation
                            // Closure mapping: the same allocator run the emit hook used
                            // when the delta compile was lowered (deterministic over identical
                            // session state + fresh tree), keeping the chained tables in sync
                            // with the closure names the compile actually emitted.
                            RefreshedClosureNameRows =
                                computeOccurrenceKeyedClosureNamesWithScope
                                    tcGlobals
                                    session.Baseline
                                    ImplementationFileScope.ReferenceChanged
                                    session.ImplementationFiles
                                    updatedImplementation
                                    session.CurrentGeneration
                                |> snd
                        }

                    match this.EmitDelta(request, ?freshDebugPdb = freshDebugPdb, ?projectKey = projectKey) with
                    | Ok result ->
                        let delta = result.Delta

                        if delta.UpdatedBaseline.IsSome then
                            if deferCommit then
                                // Stage the fresh diff inputs with the pending baseline; the
                                // host's session-wide Commit advances both together.
                                sessionStore().StagePendingImplementationFiles(updatedImplementation, ?key = projectKey)
                            else
                                this.CommitPendingUpdate(delta.GenerationId)
                                sessionStore().UpdateImplementationFiles(updatedImplementation, ?key = projectKey)

                            Ok result
                        elif not (List.isEmpty delta.SequencePointUpdates) then
                            // Line-shift-only update: no metadata/IL, no generation consumed.
                            // Commit the rebound sequence-point view so the next emit diffs
                            // against the lines the host just applied to the debugger.
                            // (Committed immediately in both modes: there is no generation to
                            // stage and nothing to pass to MetadataUpdater.ApplyUpdate.)
                            delta.ChainedSequencePoints
                            |> Option.iter (fun snapshots -> sessionStore().UpdateCommittedSequencePoints(snapshots, ?key = projectKey))

                            sessionStore().UpdateImplementationFiles(updatedImplementation, ?key = projectKey)
                            Ok result
                        elif hasUpdates then
                            // Semantic edits that materialized into no emitted rows keep the
                            // legacy behavior: surface the (empty) delta to the caller.
                            sessionStore().UpdateImplementationFiles(updatedImplementation, ?key = projectKey)
                            Ok result
                        else
                            Error HotReloadError.NoChanges
                    | Error error -> Error error

    /// <summary>Explicit commit hook mirroring Roslyn's service contract.</summary>
    member this.CommitPendingUpdate(generationId: Guid) = this.OnDeltaApplied(generationId)

    /// <summary>
    /// Commits ALL pending project updates atomically without generation-id validation —
    /// the session entity's solution-wide commit (Roslyn's <c>CommitSolutionUpdate</c>).
    /// Returns the committed generation ids (empty when nothing was pending).
    /// </summary>
    member _.CommitAllPendingUpdates() = sessionStore().CommitAllPending()

    /// <summary>Explicit discard hook mirroring Roslyn's pending-update semantics (solution-wide).</summary>
    member _.DiscardPendingUpdate() = sessionStore().DiscardPendingUpdate()

    /// <summary>Keys of the projects currently active in the session.</summary>
    member _.ProjectKeys = sessionStore().ProjectKeys

    /// <summary>Unconditionally sets the session-wide capability set (session-entity creation path).</summary>
    member _.SetCapabilities(capabilities: EditAndContinueCapabilities) =
        sessionStore().SetCapabilities(capabilities)

    /// <summary>Unconditionally replaces the session-wide active statements (session-entity path).</summary>
    member _.SetActiveStatements(activeStatements: FSharp.Compiler.CodeAnalysis.FSharpManagedActiveStatementDebugInfo list) =
        sessionStore().SetActiveStatements(activeStatements)
