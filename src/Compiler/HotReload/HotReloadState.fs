module internal FSharp.Compiler.HotReloadState

open System
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTree

type HotReloadSession =
    {
        Baseline: FSharpEmitBaseline
        ImplementationFiles: CheckedAssemblyAfterOptimization
        CurrentGeneration: int
        PreviousGenerationId: Guid option
        PendingUpdate: PendingHotReloadUpdate option
        /// Runtime capabilities negotiated when the session started; consulted by edit classification.
        Capabilities: EditAndContinueCapabilities
    }

and PendingHotReloadUpdate =
    {
        GenerationId: Guid
        Baseline: FSharpEmitBaseline
    }

/// Records whether starting a baseline session replaced an already-active process-wide session.
type HotReloadSessionStart =
    | StartedFresh
    | ReplacedExisting

let private toCommittedSnapshot (value: HotReloadSession) =
    { value with
        PendingUpdate = None }

/// Session store used by hot reload emit services. This keeps mutable session lifecycle
/// state instance-scoped so ownership can live with the hosting service.
type internal HotReloadSessionStore() =

    let sessionLock = obj ()
    let mutable session: HotReloadSession voption = ValueNone
    let mutable lastCommittedSession: HotReloadSession voption = ValueNone

    member _.SetBaseline
        (
            value: FSharpEmitBaseline,
            implementationFiles: CheckedAssemblyAfterOptimization,
            ?capabilities: EditAndContinueCapabilities
        ) : HotReloadSessionStart =
        lock sessionLock (fun () ->
            let hadExistingSession = session.IsSome

            let previousGenerationId =
                if value.EncId = Guid.Empty then
                    None
                else
                    Some value.EncId

            let newSession =
                {
                    Baseline = value
                    ImplementationFiles = implementationFiles
                    CurrentGeneration = max 1 value.NextGeneration
                    PreviousGenerationId = previousGenerationId
                    PendingUpdate = None
                    // Hosts that do not negotiate capabilities get the Roslyn-conservative default.
                    Capabilities = defaultArg capabilities EditAndContinueCapabilities.BaselineOnly
                }

            session <- ValueSome newSession
            lastCommittedSession <- ValueSome(toCommittedSnapshot newSession)

            if hadExistingSession then
                ReplacedExisting
            else
                StartedFresh)

    member _.ClearBaseline() =
        lock sessionLock (fun () -> session <- ValueNone)

    member _.ClearSessionState() =
        lock sessionLock (fun () ->
            session <- ValueNone
            lastCommittedSession <- ValueNone)

    member _.TryGetBaseline() =
        lock sessionLock (fun () ->
            match session with
            | ValueSome s -> ValueSome s.Baseline
            | ValueNone -> ValueNone)

    member _.TryGetSession() =
        lock sessionLock (fun () -> session)

    member _.TryRestoreSession() =
        lock sessionLock (fun () ->
            match session with
            | ValueSome current -> ValueSome current
            | ValueNone ->
                match lastCommittedSession with
                | ValueSome committed ->
                    let restored = toCommittedSnapshot committed
                    session <- ValueSome restored
                    ValueSome restored
                | ValueNone -> ValueNone)

    /// Replaces the runtime capability set consulted by edit classification. Safe at any point
    /// in the session: capabilities are read per-emit and never affect already-emitted deltas.
    /// Hosts use this when the running process reports its capabilities after the session was
    /// started (the dotnet-watch session is prestarted before the application launches).
    member _.UpdateCapabilities(capabilities: EditAndContinueCapabilities) =
        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                session <- ValueSome { state with Capabilities = capabilities }

                match lastCommittedSession with
                | ValueSome committed ->
                    lastCommittedSession <- ValueSome { committed with Capabilities = capabilities }
                | ValueNone -> ()

                true
            | ValueNone -> false)

    member _.UpdateImplementationFiles(implementationFiles: CheckedAssemblyAfterOptimization) =
        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                let updated =
                    {
                        state with
                            ImplementationFiles = implementationFiles
                    }

                session <- ValueSome updated
                lastCommittedSession <- ValueSome(toCommittedSnapshot updated)
            | ValueNone -> ())

    /// <summary>
    /// Replaces the committed per-method sequence-point view after a line-shift-only update
    /// (Phase G). Such updates carry no metadata/IL and consume no generation, so the full
    /// pending-baseline commit flow does not apply — but the next emit must diff against the
    /// lines the host just rebound in the debugger, exactly as Roslyn diffs line edits against
    /// the last committed solution.
    /// </summary>
    member _.UpdateCommittedSequencePoints
        (snapshots: Map<int, FSharp.Compiler.HotReload.ActiveStatementAnalysis.MethodSequencePoints>)
        =
        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                let updated =
                    { state with
                        Baseline =
                            { state.Baseline with
                                SequencePointSnapshots = snapshots }
                    }

                session <- ValueSome updated
                lastCommittedSession <- ValueSome(toCommittedSnapshot updated)
            | ValueNone -> ())

    member _.UpdateBaseline(baseline: FSharpEmitBaseline) =
        if baseline.EncId = Guid.Empty then
            invalidArg (nameof baseline) "Pending baseline must carry a non-empty EncId."

        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                session <-
                    ValueSome
                        {
                            state with
                                PendingUpdate =
                                    Some
                                        {
                                            GenerationId = baseline.EncId
                                            Baseline = baseline
                                        }
                        }
            | ValueNone -> ())

    member _.RecordDeltaApplied(generationId: Guid) =
        if generationId = Guid.Empty then
            invalidArg (nameof generationId) "Generation ID cannot be empty GUID."

        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                let pending =
                    match state.PendingUpdate with
                    | Some pending when pending.GenerationId = generationId -> pending
                    | Some _ ->
                        invalidArg
                            (nameof generationId)
                            "Generation ID does not match the currently pending hot reload update."
                    | None -> invalidOp "Cannot commit delta: no pending hot reload update."

                let updated =
                    {
                        state with
                            Baseline = pending.Baseline
                            CurrentGeneration = state.CurrentGeneration + 1
                            PreviousGenerationId = Some generationId
                            PendingUpdate = None
                    }

                session <- ValueSome updated
                lastCommittedSession <- ValueSome(toCommittedSnapshot updated)
            | ValueNone ->
                invalidOp "Cannot record delta applied: no active hot reload session.")

    member _.DiscardPendingUpdate() =
        lock sessionLock (fun () ->
            match session with
            | ValueSome state ->
                session <-
                    ValueSome
                        {
                            state with
                                PendingUpdate = None
                        }
            | ValueNone -> ())

let private activeStoreLock = obj ()
let mutable private activeSessionStore = HotReloadSessionStore()

/// The active store remains process-scoped today; callers set this when installing a
/// service-owned session store so global helper calls route to that owner.
let setSessionStore (store: HotReloadSessionStore) =
    if obj.ReferenceEquals(store, null) then
        invalidArg (nameof store) "Hot reload session store cannot be null."

    lock activeStoreLock (fun () -> activeSessionStore <- store)

let getSessionStore () =
    lock activeStoreLock (fun () -> activeSessionStore)

let createSessionStore () =
    HotReloadSessionStore()

// Backward-compatible module functions delegate to the currently active store.
let setBaseline (value: FSharpEmitBaseline) (implementationFiles: CheckedAssemblyAfterOptimization) =
    getSessionStore().SetBaseline(value, implementationFiles)

let clearBaseline () =
    getSessionStore().ClearBaseline()

let clearSessionState () =
    getSessionStore().ClearSessionState()

let tryGetBaseline () =
    getSessionStore().TryGetBaseline()

let tryGetSession () =
    getSessionStore().TryGetSession()

let tryRestoreSession () =
    getSessionStore().TryRestoreSession()

let updateImplementationFiles (implementationFiles: CheckedAssemblyAfterOptimization) =
    getSessionStore().UpdateImplementationFiles(implementationFiles)

let updateBaseline (baseline: FSharpEmitBaseline) =
    getSessionStore().UpdateBaseline(baseline)

let recordDeltaApplied (generationId: Guid) =
    getSessionStore().RecordDeltaApplied(generationId)

let discardPendingUpdate () =
    getSessionStore().DiscardPendingUpdate()
