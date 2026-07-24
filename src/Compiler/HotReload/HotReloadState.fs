module internal FSharp.Compiler.HotReloadState

open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTree

/// <summary>
/// Identity of the project a per-project hot reload slot belongs to. The
/// <c>Project</c> case structurally mirrors <c>FSharpProjectIdentifier</c>
/// (projectFileName * outputFileName), which is defined later in the compilation order
/// (FSharpProjectSnapshot.fs), so the service layer converts between the two.
/// </summary>
[<RequireQualifiedAccess>]
type HotReloadProjectKey =
    | Project of projectFileName: string * outputFileName: string
    /// Identity-less hosts: the fsc emit-hook capture path and the legacy module-level
    /// helpers do not know which project they serve; their state lives in this slot.
    | Ambient

/// <summary>
/// Per-project committed hot reload state — the F# analogue of one entry in Roslyn's
/// <c>DebuggingSession</c> project baselines (<c>ProjectId -&gt; ProjectBaseline</c>).
/// The chained CDI/closure-name/sequence-point state rides on
/// <see cref="FSharpEmitBaseline"/>, so keying the baseline per project keys all of it.
/// </summary>
type HotReloadProjectState =
    {
        Baseline: FSharpEmitBaseline
        ImplementationFiles: CheckedAssemblyAfterOptimization
        CurrentGeneration: int
        PreviousGenerationId: Guid option
        PendingUpdate: PendingHotReloadUpdate option
    }

and PendingHotReloadUpdate =
    | Delta of PendingDeltaHotReloadUpdate
    | LineOnly of PendingLineHotReloadUpdate

and PendingDeltaHotReloadUpdate =
    {
        GenerationId: Guid
        Baseline: FSharpEmitBaseline
        /// Implementation files staged by a deferred-commit emit (the session-entity flow,
        /// mirroring Roslyn's EmitSolutionUpdate/CommitSolutionUpdate split). None in the
        /// legacy auto-commit flow, where the language service updates them directly.
        ImplementationFiles: CheckedAssemblyAfterOptimization option
    }

and PendingLineHotReloadUpdate =
    {
        SequencePointSnapshots: Map<int, FSharp.Compiler.HotReload.ActiveStatementAnalysis.MethodSequencePoints>
        ImplementationFiles: CheckedAssemblyAfterOptimization
    }

/// <summary>
/// Combined per-project + session-wide view handed to existing callers: the per-project
/// slot joined with the session-wide capability set and active statements.
/// </summary>
type HotReloadSession =
    {
        Baseline: FSharpEmitBaseline
        ImplementationFiles: CheckedAssemblyAfterOptimization
        CurrentGeneration: int
        PreviousGenerationId: Guid option
        PendingUpdate: PendingHotReloadUpdate option
        /// Runtime capabilities negotiated for the session; consulted by edit classification.
        /// Session-wide (Roslyn parity: capabilities are a debugging-session property).
        Capabilities: EditAndContinueCapabilities
        /// <summary>
        /// Active statements supplied by the debugger host before emitting. The next
        /// emitted delta remaps each statement (or fails rude when an edit destroys one). Empty
        /// when no debugger is attached or the host has not reported a break state; the setter
        /// REPLACES the whole set, mirroring Roslyn's per-edit-session active statement fetch.
        /// Session-wide (the break state describes the host process, not one project).
        /// </summary>
        ActiveStatements: FSharpManagedActiveStatementDebugInfo list
    }

/// Records whether starting a baseline for a project replaced an already-active slot.
type HotReloadSessionStart =
    | StartedFresh
    | ReplacedExisting

let private toCommittedSnapshot (value: HotReloadProjectState) = { value with PendingUpdate = None }

let private mkProjectState (value: FSharpEmitBaseline) (implementationFiles: CheckedAssemblyAfterOptimization) =
    let previousGenerationId =
        if value.EncId = Guid.Empty then None else Some value.EncId

    {
        Baseline = value
        ImplementationFiles = implementationFiles
        CurrentGeneration = max 1 value.NextGeneration
        PreviousGenerationId = previousGenerationId
        PendingUpdate = None
    }

/// <summary>
/// Per-session storage used by hot reload emit services: per-project committed state keyed
/// by <see cref="HotReloadProjectKey"/> plus session-wide capabilities and active statements.
/// One instance backs one logical session (a checker's default compatibility session, or one
/// <c>FSharpHotReloadSession</c>); instances are fully independent of each other.
/// Commit/discard operate across all pending project updates atomically (Roslyn's
/// solution-wide commit semantics).
/// </summary>
type internal HotReloadSessionStore() =

    let sessionLock = obj ()
    let mutable projects: Map<HotReloadProjectKey, HotReloadProjectState> = Map.empty

    let mutable committedProjects: Map<HotReloadProjectKey, HotReloadProjectState> =
        Map.empty

    // The slot identity-less callers (legacy module helpers, the fsc emit hook) operate on:
    // the most recently started project. Single-project compatibility flows therefore behave
    // exactly like the previous one-session store.
    let mutable currentKey = HotReloadProjectKey.Ambient

    // Session-wide state (Roslyn parity: DebuggingSession-level, not per-project).
    let mutable sessionCapabilities = EditAndContinueCapabilities.BaselineOnly
    let mutable sessionActiveStatements: FSharpManagedActiveStatementDebugInfo list = []

    let resolveKey (key: HotReloadProjectKey option) =
        match key with
        | Some key -> key
        | None -> currentKey

    let toSessionView (state: HotReloadProjectState) =
        {
            Baseline = state.Baseline
            ImplementationFiles = state.ImplementationFiles
            CurrentGeneration = state.CurrentGeneration
            PreviousGenerationId = state.PreviousGenerationId
            PendingUpdate = state.PendingUpdate
            Capabilities = sessionCapabilities
            ActiveStatements =
                sessionActiveStatements
                |> List.filter (fun statement -> statement.ActiveInstruction.Method.ModuleId = state.Baseline.ModuleId)
        }

    let updateProject key (update: HotReloadProjectState -> HotReloadProjectState) (commit: bool) =
        match Map.tryFind key projects with
        | Some state ->
            let updated = update state
            projects <- Map.add key updated projects

            if commit then
                committedProjects <- Map.add key (toCommittedSnapshot updated) committedProjects
        | None -> ()

    /// <summary>
    /// Starts (or restarts) the session with a single project, REPLACING all existing
    /// per-project state and resetting the session-wide capability/active-statement state.
    /// This is the legacy single-session semantic: hosts on the compatibility surface get
    /// "starting a session replaces the previous one".
    /// </summary>
    member _.SetBaseline
        (
            value: FSharpEmitBaseline,
            implementationFiles: CheckedAssemblyAfterOptimization,
            ?capabilities: EditAndContinueCapabilities,
            ?key: HotReloadProjectKey
        ) : HotReloadSessionStart =
        lock sessionLock (fun () ->
            let hadExistingSession = not (Map.isEmpty projects)
            let key = defaultArg key HotReloadProjectKey.Ambient
            let newState = mkProjectState value implementationFiles

            projects <- Map.ofList [ key, newState ]
            committedProjects <- Map.ofList [ key, toCommittedSnapshot newState ]
            currentKey <- key
            // Hosts that do not negotiate capabilities get the Roslyn-conservative default.
            sessionCapabilities <- defaultArg capabilities EditAndContinueCapabilities.BaselineOnly
            sessionActiveStatements <- []

            if hadExistingSession then
                ReplacedExisting
            else
                StartedFresh)

    /// <summary>
    /// Adds (or recaptures) ONE project without touching other projects or the session-wide
    /// capability/active-statement state — the session-entity semantic (Roslyn analog:
    /// capturing an <c>EmitBaseline</c> for one more module inside the same DebuggingSession).
    /// </summary>
    member _.AddProject
        (key: HotReloadProjectKey, value: FSharpEmitBaseline, implementationFiles: CheckedAssemblyAfterOptimization)
        : HotReloadSessionStart =
        lock sessionLock (fun () ->
            let hadExistingSlot = Map.containsKey key projects
            let newState = mkProjectState value implementationFiles

            projects <- Map.add key newState projects
            committedProjects <- Map.add key (toCommittedSnapshot newState) committedProjects
            currentKey <- key

            if hadExistingSlot then ReplacedExisting else StartedFresh)

    /// Clears all active per-project state, keeping the committed snapshots restorable.
    member _.ClearBaseline() =
        lock sessionLock (fun () -> projects <- Map.empty)

    /// Clears both active and restorable session state.
    member _.ClearSessionState() =
        lock sessionLock (fun () ->
            projects <- Map.empty
            committedProjects <- Map.empty
            currentKey <- HotReloadProjectKey.Ambient
            sessionCapabilities <- EditAndContinueCapabilities.BaselineOnly
            sessionActiveStatements <- [])

    /// Keys of the projects currently active in the session.
    member _.ProjectKeys =
        lock sessionLock (fun () -> projects |> Map.toList |> List.map fst)

    member _.TryGetBaseline(?key: HotReloadProjectKey) =
        lock sessionLock (fun () ->
            match Map.tryFind (resolveKey key) projects with
            | Some state -> ValueSome state.Baseline
            | None -> ValueNone)

    member _.TryGetSession(?key: HotReloadProjectKey) =
        lock sessionLock (fun () ->
            match Map.tryFind (resolveKey key) projects with
            | Some state -> ValueSome(toSessionView state)
            | None -> ValueNone)

    member _.TryRestoreSession(?key: HotReloadProjectKey) =
        lock sessionLock (fun () ->
            let key = resolveKey key

            match Map.tryFind key projects with
            | Some current -> ValueSome(toSessionView current)
            | None ->
                match Map.tryFind key committedProjects with
                | Some committed ->
                    let restored = toCommittedSnapshot committed
                    projects <- Map.add key restored projects
                    ValueSome(toSessionView restored)
                | None -> ValueNone)

    /// <summary>
    /// Replaces the runtime capability set consulted by edit classification. Safe at any point
    /// in the session: capabilities are read per-emit and never affect already-emitted deltas.
    /// Hosts use this when the running process reports its capabilities after the session was
    /// started (the dotnet-watch session is prestarted before the application launches).
    /// Session-wide. Returns false when no project is active.
    /// </summary>
    member _.UpdateCapabilities(value: EditAndContinueCapabilities) =
        lock sessionLock (fun () ->
            if Map.isEmpty projects then
                false
            else
                sessionCapabilities <- value
                true)

    /// Unconditionally sets the session-wide capability set; used by the session entity,
    /// whose capabilities are fixed at creation time, before any project is added.
    member _.SetCapabilities(value: EditAndContinueCapabilities) =
        lock sessionLock (fun () -> sessionCapabilities <- value)

    /// Unconditionally sets the session-wide active statements; used by the session entity.
    member _.SetActiveStatements(value: FSharpManagedActiveStatementDebugInfo list) =
        lock sessionLock (fun () -> sessionActiveStatements <- value)

    member _.UpdateImplementationFiles(implementationFiles: CheckedAssemblyAfterOptimization, ?key: HotReloadProjectKey) =
        lock sessionLock (fun () ->
            updateProject
                (resolveKey key)
                (fun state ->
                    { state with
                        ImplementationFiles = implementationFiles
                    })
                true)

    /// <summary>
    /// Replaces the committed per-method sequence-point view after a line-shift-only update.
    /// Such updates carry no metadata/IL and consume no generation, so the full
    /// pending-baseline commit flow does not apply — but the next emit must diff against the
    /// lines the host just rebound in the debugger, exactly as Roslyn diffs line edits against
    /// the last committed solution.
    /// </summary>
    member _.UpdateCommittedSequencePoints
        (snapshots: Map<int, FSharp.Compiler.HotReload.ActiveStatementAnalysis.MethodSequencePoints>, ?key: HotReloadProjectKey)
        =
        lock sessionLock (fun () ->
            updateProject
                (resolveKey key)
                (fun state ->
                    { state with
                        Baseline =
                            { state.Baseline with
                                SequencePointSnapshots = snapshots
                            }
                    })
                true)

    /// Stages the next-generation baseline as the project's pending update.
    member _.UpdateBaseline(baseline: FSharpEmitBaseline, ?key: HotReloadProjectKey) =
        if baseline.EncId = Guid.Empty then
            invalidArg (nameof baseline) "Pending baseline must carry a non-empty EncId."

        lock sessionLock (fun () ->
            updateProject
                (resolveKey key)
                (fun state ->
                    match state.PendingUpdate with
                    | Some _ -> invalidOp "Cannot emit another hot reload update before the pending update is committed or discarded."
                    | None ->
                        { state with
                            PendingUpdate =
                                Some(
                                    Delta
                                        {
                                            GenerationId = baseline.EncId
                                            Baseline = baseline
                                            ImplementationFiles = None
                                        }
                                )
                        })
                false)

    /// Stages a debugger line-rebind update without consuming a metadata generation.
    member _.StageLineOnlyUpdate
        (
            snapshots: Map<int, FSharp.Compiler.HotReload.ActiveStatementAnalysis.MethodSequencePoints>,
            implementationFiles: CheckedAssemblyAfterOptimization,
            ?key: HotReloadProjectKey
        ) =
        lock sessionLock (fun () ->
            updateProject
                (resolveKey key)
                (fun state ->
                    match state.PendingUpdate with
                    | Some _ -> invalidOp "Cannot emit another hot reload update before the pending update is committed or discarded."
                    | None ->
                        { state with
                            PendingUpdate =
                                Some(
                                    LineOnly
                                        {
                                            SequencePointSnapshots = snapshots
                                            ImplementationFiles = implementationFiles
                                        }
                                )
                        })
                false)

    /// <summary>
    /// Attaches the fresh typed implementation files to the project's pending update so a
    /// deferred commit (the session-entity flow) can advance the committed diff inputs together
    /// with the baseline. No-op when the project has no pending update.
    /// </summary>
    member _.StagePendingImplementationFiles(implementationFiles: CheckedAssemblyAfterOptimization, ?key: HotReloadProjectKey) =
        lock sessionLock (fun () ->
            updateProject
                (resolveKey key)
                (fun state ->
                    match state.PendingUpdate with
                    | Some(Delta pending) ->
                        { state with
                            PendingUpdate =
                                Some(
                                    Delta
                                        { pending with
                                            ImplementationFiles = Some implementationFiles
                                        }
                                )
                        }
                    | Some(LineOnly _)
                    | None -> state)
                false)

    member private _.CommitPendingLocked(pendings: (HotReloadProjectKey * HotReloadProjectState * PendingHotReloadUpdate) list) =
        for key, state, pending in pendings do
            let updated =
                match pending with
                | Delta delta ->
                    { state with
                        Baseline = delta.Baseline
                        ImplementationFiles = defaultArg delta.ImplementationFiles state.ImplementationFiles
                        CurrentGeneration = state.CurrentGeneration + 1
                        PreviousGenerationId = Some delta.GenerationId
                        PendingUpdate = None
                    }
                | LineOnly lineUpdate ->
                    { state with
                        Baseline =
                            { state.Baseline with
                                SequencePointSnapshots = lineUpdate.SequencePointSnapshots
                            }
                        ImplementationFiles = lineUpdate.ImplementationFiles
                        PendingUpdate = None
                    }

            projects <- Map.add key updated projects
            committedProjects <- Map.add key (toCommittedSnapshot updated) committedProjects

    /// <summary>
    /// Commits the applied update identified by <paramref name="generationId"/> together with
    /// every other pending project update — solution-wide commit semantics (Roslyn's
    /// <c>CommitSolutionUpdate</c>): partial cross-project commits are unrepresentable.
    /// </summary>
    member this.RecordDeltaApplied(generationId: Guid) =
        if generationId = Guid.Empty then
            invalidArg (nameof generationId) "Generation ID cannot be empty GUID."

        lock sessionLock (fun () ->
            if Map.isEmpty projects then
                invalidOp "Cannot record delta applied: no active hot reload session."

            let pendings =
                projects
                |> Map.toList
                |> List.choose (fun (key, state) -> state.PendingUpdate |> Option.map (fun pending -> key, state, pending))

            if List.isEmpty pendings then
                invalidOp "Cannot commit delta: no pending hot reload update."

            let matchesPending =
                pendings
                |> List.exists (fun (_, _, pending) ->
                    match pending with
                    | Delta delta -> delta.GenerationId = generationId
                    | LineOnly _ -> false)

            if not matchesPending then
                invalidArg (nameof generationId) "Generation ID does not match the currently pending hot reload update."

            this.CommitPendingLocked pendings)

    /// <summary>
    /// Commits ALL pending project updates atomically without generation-id validation —
    /// the session entity's <c>Commit</c> (Roslyn's <c>CommitSolutionUpdate</c>). Returns the
    /// generation ids committed (empty when nothing was pending).
    /// </summary>
    member this.CommitAllPending() : Guid list =
        lock sessionLock (fun () ->
            let pendings =
                projects
                |> Map.toList
                |> List.choose (fun (key, state) -> state.PendingUpdate |> Option.map (fun pending -> key, state, pending))

            this.CommitPendingLocked pendings

            pendings
            |> List.choose (fun (_, _, pending) ->
                match pending with
                | Delta delta -> Some delta.GenerationId
                | LineOnly _ -> None))

    /// Discards ALL pending project updates (Roslyn's <c>DiscardSolutionUpdate</c>).
    member _.DiscardPendingUpdate() =
        lock sessionLock (fun () -> projects <- projects |> Map.map (fun _ state -> { state with PendingUpdate = None }))

/// <summary>
/// The project being compiled in-process on behalf of a hot reload session: the store of the
/// session that owns it plus the project's identity inside that store. The session owner
/// (<c>FSharpChecker.Compile</c>) sets this around an in-process compile of a session-tracked
/// project so the fsc emit hook serves THAT session's chained closure-name/synthesized-name
/// state, instead of consulting a process-wide ambient registration.
/// </summary>
type HotReloadEmissionContext =
    {
        Store: HotReloadSessionStore
        ProjectKey: HotReloadProjectKey
    }

// The compiler service can compile projects concurrently. AsyncLocal scopes the emit-hook route
// to one logical compile while still flowing across the thread hops made by F# async.
let private currentEmissionContext =
    Threading.AsyncLocal<HotReloadEmissionContext option>()

/// Sets (or clears, with None) the scoped emission context consulted by the fsc emit hook.
let setCurrentEmissionContext (context: HotReloadEmissionContext option) = currentEmissionContext.Value <- context

let tryGetCurrentEmissionContext () = currentEmissionContext.Value

let private activeSessionStore = HotReloadSessionStore()

/// The process-local store identity-less callers operate on: the fsc emit hook when no scoped
/// emission context is set (standalone fsc capture compiles publish their captured baseline
/// here as a side-channel), and the module-level helpers below (unit-level tooling and tests).
/// It is NEVER a session's store: session entities own private store instances and reconstruct
/// baselines from disk artifacts, and no registration can replace this store.
let getSessionStore () = activeSessionStore

let createSessionStore () = HotReloadSessionStore()

// Backward-compatible module functions delegate to the currently active store and operate on
// its current (most recently started) project slot.
let setBaseline (value: FSharpEmitBaseline) (implementationFiles: CheckedAssemblyAfterOptimization) =
    getSessionStore().SetBaseline(value, implementationFiles)

let clearBaseline () = getSessionStore().ClearBaseline()

let clearSessionState () = getSessionStore().ClearSessionState()

let tryGetBaseline () = getSessionStore().TryGetBaseline()

let tryGetSession () = getSessionStore().TryGetSession()

let tryRestoreSession () = getSessionStore().TryRestoreSession()

let updateImplementationFiles (implementationFiles: CheckedAssemblyAfterOptimization) =
    getSessionStore().UpdateImplementationFiles(implementationFiles)

let updateBaseline (baseline: FSharpEmitBaseline) =
    getSessionStore().UpdateBaseline(baseline)

let recordDeltaApplied (generationId: Guid) =
    getSessionStore().RecordDeltaApplied(generationId)

let discardPendingUpdate () =
    getSessionStore().DiscardPendingUpdate()
