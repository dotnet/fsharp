module internal FSharp.Compiler.HotReloadEmitHook

open System
open System.IO
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.HotReloadPdb
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.Text.Range

/// Hot reload emit hook implementation used when --test:HotReloadDeltas is active.
type internal DefaultHotReloadEmitHook(editAndContinueService: FSharpEditAndContinueLanguageService) =

    // Scoped emission context: when the host compiles a session-tracked project in-process
    // (FSharpChecker.Compile sets the context around the compile), the hook serves THAT
    // session's chained closure-name/synthesized-name state. Without a context the hook
    // falls back to the service it was constructed with (the ambient store).
    let resolveSessionAccess () =
        match HotReloadState.tryGetCurrentEmissionContext () with
        | Some context -> FSharpEditAndContinueLanguageService(context.Store), Some context.ProjectKey
        | None -> editAndContinueService, None

    let hasScopedEmissionContext () =
        (HotReloadState.tryGetCurrentEmissionContext ()).IsSome

    // Build a baseline snapshot from the exact emitted artifacts and publish it to the
    // process-local capture slot (HotReloadState's module store, via the service this hook
    // was constructed with), then activate synthesized-name replay for subsequent deltas in
    // the same process. The capture is SESSION-INDEPENDENT: capture compiles never run under
    // a scoped emission context, the capture slot is not any session's store, and sessions
    // never read it — they reconstruct baselines from the on-disk dll + pdb. The slot serves
    // capture-to-capture name chaining within one host, standalone fsc validation and
    // unit-level tooling/tests; checker creation resets it (the host boundary).
    let captureArtifacts (compilerGlobalState: CompilerGlobalState) (artifacts: CompilerEmitArtifacts) =
        let portablePdbSnapshot =
            artifacts.PortablePdbBytes |> Option.map HotReloadPdb.createSnapshot

        let ilxGenEnvironment = artifacts.IlxGenEnvSnapshot

        let baseline =
            HotReloadBaseline.createFromEmittedArtifacts
                artifacts.IlxMainModule
                artifacts.TokenMappings
                artifacts.AssemblyBytes
                portablePdbSnapshot
                ilxGenEnvironment

        // Closure mapping: the per-method occurrence-chain -> closure-name
        // tables. Baseline creation reconstructed them from the emitted CDI occurrence
        // keys (names are a pure function of occurrence identity under the
        // occurrence-derived naming), which is the same computation a disk-started session performs in
        // another process. The emit-time stamp -> name recording (re-keyed by MethodDef
        // token, fail closed on non-unique names) serves two purposes here:
        //  - a RECAPTURE compile emitted under an active session names added closures
        //    with their first-allocation generation (#g{N>=1}); those names are not
        //    derivable from generation-0 identity, so the recording is the table source
        //    (the CDI reconstruction failed closed on the mid-session marker);
        //  - otherwise it validates the reconstruction: wherever both computed a table
        //    for the same method, they must agree (derived == recorded).
        let recordedRows =
            HotReloadBaseline.resolveClosureNameRowsByToken baseline artifacts.ClosureNameRows

        let baseline =
            match editAndContinueService.TryGetSession() with
            | ValueSome session when not (Map.isEmpty session.Baseline.EncClosureNames) ->
                { baseline with
                    EncClosureNames = recordedRows
                }
            | _ ->
                let mismatches =
                    baseline.EncClosureNames
                    |> Map.toList
                    |> List.choose (fun (methodToken, derivedTable) ->
                        match Map.tryFind methodToken recordedRows with
                        | Some recordedTable when recordedTable <> derivedTable -> Some(methodToken, derivedTable, recordedTable)
                        | _ -> None)

                if not mismatches.IsEmpty then
                    if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_CLOSURENAMES") = "1" then
                        printfn
                            "[fsharp-hotreload][closure-names] capture validation FAILED: derived tables disagree with the emit-time recording: %A"
                            mismatches

                    System.Diagnostics.Debug.Assert(
                        false,
                        "Hot reload closure-name reconstruction disagrees with the emit-time stamp -> name recording."
                    )

                baseline

        editAndContinueService.StartSession(baseline, artifacts.OptimizedImpls)
        |> ignore

        match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
        | Some map -> map.BeginSession()
        | None -> ()

    interface ICompilerEmitHook with
        member _.ValidateConfiguration(emitCaptureArtifacts, debugInfo, embeddedPdb, localOptimizationsEnabled) =
            if emitCaptureArtifacts then
                if not debugInfo then
                    error (Error(FSComp.SR.fscHotReloadRequiresDebugInfo (), rangeStartup))

                if embeddedPdb then
                    error (Error(FSComp.SR.fscHotReloadRequiresPortableDebugInfo (), rangeStartup))

                if localOptimizationsEnabled then
                    error (Error(FSComp.SR.fscHotReloadIncompatibleWithOptimization (), rangeStartup))

        member _.PrepareForCodeGeneration(emitCaptureArtifacts, tcGlobals, optimizedImpls) =
            let compilerGlobalState = tcGlobals.CompilerGlobalState.Value

            // Resolve the session this compile serves: the scoped emission context when the
            // host set one (a session entity's tracked project), the ambient service otherwise.
            let sessionService, scopedProjectKey = resolveSessionAccess ()

            let tryGetActiveSession () =
                sessionService.TryGetSession(?projectKey = scopedProjectKey)

            // Closure mapping, the codegen-time hook step: when a session with
            // baseline closure-name tables is active, run the occurrence-keyed allocator
            // over (previous-generation impl files, the impl files about to be lowered)
            // and install the stamp -> assigned-name table for the IlxGen closure call
            // site. Fail closed: no session, or a baseline without tables (flag-off
            // capture, checker read-from-disk baselines), installs nothing and lowering
            // keeps pure sequence-replay naming.
            (match tryGetActiveSession () with
             | ValueSome session when not (Map.isEmpty session.Baseline.EncClosureNames) ->
                 let assignedNames, _refreshedRows =
                     HotReloadBaseline.computeOccurrenceKeyedClosureNames
                         tcGlobals
                         session.Baseline
                         session.ImplementationFiles
                         optimizedImpls
                         session.CurrentGeneration

                 if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_CLOSURENAMES") = "1" then
                     printfn
                         "[fsharp-hotreload][closure-names] hook install: tables=%d gen=%d assigned=%d names=%A"
                         (Map.count session.Baseline.EncClosureNames)
                         session.CurrentGeneration
                         (Map.count assignedNames)
                         (assignedNames |> Map.toList)

                 if emitCaptureArtifacts && Map.isEmpty assignedNames then
                     // A CAPTURE compile the active session does not cover (its occurrence
                     // keys match nothing being lowered — e.g. a stale capture session for
                     // an unrelated project in the process-local store): this is a fresh
                     // baseline capture, so install the occurrence-derived names below
                     // instead of an empty table that would strip the baseline of its
                     // reconstructible closure names.
                     let derivedNames =
                         HotReloadBaseline.computeBaselineOccurrenceKeyedClosureNames tcGlobals optimizedImpls

                     if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_CLOSURENAMES") = "1" then
                         printfn
                             "[fsharp-hotreload][closure-names] capture not covered by active session; baseline install: derived=%d"
                             (Map.count derivedNames)

                     ClosureNameAllocationState.setAssignedClosureNames (compilerGlobalState :> obj) derivedNames
                 else
                     ClosureNameAllocationState.setAssignedClosureNames (compilerGlobalState :> obj) assignedNames
             | _ when emitCaptureArtifacts ->
                 // Closure mapping: a BASELINE capture compile (no session tables to
                 // chain from) derives every closure class name from occurrence identity:
                 // {memberName}@hotreload#g0_o{chain}, installed stamp-keyed exactly like
                 // the delta-compile allocator output. Names are then a pure function of
                 // the occurrence keys the baseline CDI emission persists in the portable PDB,
                 // so a session started from the on-disk baseline in ANOTHER process can
                 // reconstruct the chain -> name tables without any in-memory carry-over.
                 // Members the derivation fails closed on (no unique compiled name,
                 // unencodable chains) keep pure sequence-replay naming.
                 let derivedNames =
                     HotReloadBaseline.computeBaselineOccurrenceKeyedClosureNames tcGlobals optimizedImpls

                 if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_CLOSURENAMES") = "1" then
                     printfn
                         "[fsharp-hotreload][closure-names] baseline install: derived=%d names=%A"
                         (Map.count derivedNames)
                         (derivedNames |> Map.toList)

                 ClosureNameAllocationState.setAssignedClosureNames (compilerGlobalState :> obj) derivedNames
             | _ ->
                 // Explicitly drop any previously installed table in case the
                 // CompilerGlobalState instance is reused across compiles.
                 ClosureNameAllocationState.setAssignedClosureNames (compilerGlobalState :> obj) Map.empty)

            if emitCaptureArtifacts then
                // Closure mapping: capture stamp -> emitted-closure-name pairs during
                // IlxGen so the emit path can join them with the same tree's lambda
                // occurrence extraction (only capture compiles record; everything else
                // sees a strict no-op at the closure call site).
                ClosureNameAllocationState.beginClosureStampNameRecording (compilerGlobalState :> obj)

                match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
                | Some map -> map.BeginSession()
                | None ->
                    let map = FSharpSynthesizedTypeMaps()
                    map.BeginSession()
                    setCompilerGeneratedNameMap (compilerGlobalState :> obj) (map :> ICompilerGeneratedNameMap)
            elif (tryGetActiveSession ()).IsSome then
                // Preserve synthesized-name replay while a hot reload session is active,
                // even when the output build itself is emitted without capture flags.
                let activeMap =
                    match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
                    | Some existing -> Some existing
                    | None ->
                        match tryGetActiveSession () with
                        | ValueSome session ->
                            let restored = FSharpSynthesizedTypeMaps()

                            session.Baseline.SynthesizedNameSnapshot
                            |> Map.toSeq
                            |> Seq.map (fun (k, v) -> struct (k, v))
                            |> restored.LoadSnapshot

                            Some(restored :> ICompilerGeneratedNameMap)
                        | ValueNone -> None

                match activeMap with
                | Some map ->
                    map.BeginSession()
                    setCompilerGeneratedNameMap (compilerGlobalState :> obj) map
                | None -> clearCompilerGeneratedNameMap (compilerGlobalState :> obj)
            else
                clearCompilerGeneratedNameMap (compilerGlobalState :> obj)

        member _.BeforeFileEmit(emitCaptureArtifacts, compilerGlobalState) =
            // Only clear the hot reload session when NOT in capture mode.
            // In IDE scenarios, MSBuild may run in the background and we don't want
            // to clear an active hot reload session being used for live editing.
            if not emitCaptureArtifacts then
                // A scoped compile serves a live session entity that owns its own lifecycle;
                // the compile must not end it (the legacy ambient cleanup stays for
                // context-less compiles).
                if not (hasScopedEmissionContext ()) then
                    editAndContinueService.EndSession()

                clearCompilerGeneratedNameMap (compilerGlobalState :> obj)
                ClosureNameAllocationState.clearClosureNameState (compilerGlobalState :> obj)

        // Emit through the in-memory writer first so disk bytes and baseline capture share
        // identical inputs; this avoids subtle drift from a second writer invocation.
        member _.TryEmitWithArtifacts
            (
                emitCaptureArtifacts,
                compilerGlobalState,
                ilWriteOptions,
                ilxMainModule,
                normalizeAssemblyRefs,
                optimizedImpls,
                ilxGenEnvSnapshot,
                methodClosureNameRows,
                outputFile,
                pdbfile
            ) =
            if not emitCaptureArtifacts then
                false
            else
                let assemblyBytes, pdbBytesOpt, tokenMappings, _ =
                    WriteILBinaryInMemoryWithArtifacts(ilWriteOptions, ilxMainModule, normalizeAssemblyRefs)

                // Emit once in-memory and persist those exact artifacts to disk to avoid
                // a second write pass diverging from the captured baseline input.
                File.WriteAllBytes(outputFile, assemblyBytes)

                match pdbfile, pdbBytesOpt with
                | Some pdbPath, Some pdbBytes -> File.WriteAllBytes(pdbPath, pdbBytes)
                | _ -> ()

                captureArtifacts
                    compilerGlobalState
                    {
                        IlxMainModule = ilxMainModule
                        TokenMappings = tokenMappings
                        AssemblyBytes = assemblyBytes
                        PortablePdbBytes = pdbBytesOpt
                        IlxGenEnvSnapshot = ilxGenEnvSnapshot
                        OptimizedImpls = optimizedImpls
                        ClosureNameRows = methodClosureNameRows
                    }

                true

        member _.CaptureArtifacts(compilerGlobalState, artifacts) =
            captureArtifacts compilerGlobalState artifacts

        member _.FallbackEmit(compilerGlobalState) =
            // Same ownership rule as BeforeFileEmit: never end a session entity's session
            // from a compile it merely scoped.
            if not (hasScopedEmissionContext ()) then
                editAndContinueService.EndSession()

            clearCompilerGeneratedNameMap (compilerGlobalState :> obj)
            ClosureNameAllocationState.clearClosureNameState (compilerGlobalState :> obj)

let createHotReloadCompilerEmitHook (editAndContinueService: FSharpEditAndContinueLanguageService) : ICompilerEmitHook =
    DefaultHotReloadEmitHook(editAndContinueService) :> ICompilerEmitHook

let hotReloadCompilerEmitHook: ICompilerEmitHook =
    createHotReloadCompilerEmitHook FSharpEditAndContinueLanguageService.Instance
