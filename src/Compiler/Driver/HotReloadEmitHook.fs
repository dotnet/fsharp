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

/// Hot reload emit hook implementation used when --enable:hotreloaddeltas is active.
type internal DefaultHotReloadEmitHook(editAndContinueService: FSharpEditAndContinueLanguageService) =

    // Build and register a baseline snapshot from the exact emitted artifacts, then
    // activate synthesized-name replay for subsequent deltas in the same process.
    let captureArtifacts
        (compilerGlobalState: CompilerGlobalState)
        (artifacts: CompilerEmitArtifacts)
        =
        let portablePdbSnapshot = artifacts.PortablePdbBytes |> Option.map HotReloadPdb.createSnapshot

        let ilxGenEnvironment =
            if obj.ReferenceEquals(artifacts.IlxGenEnvSnapshot, null) then
                None
            else
                Some artifacts.IlxGenEnvSnapshot

        let baseline =
            HotReloadBaseline.createFromEmittedArtifacts
                artifacts.IlxMainModule
                artifacts.TokenMappings
                artifacts.AssemblyBytes
                portablePdbSnapshot
                ilxGenEnvironment

        editAndContinueService.StartSession(baseline, artifacts.OptimizedImpls) |> ignore

        match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
        | Some map -> map.BeginSession()
        | None -> ()

    interface ICompilerEmitHook with
        member _.ValidateConfiguration(emitCaptureArtifacts, debugInfo, localOptimizationsEnabled) =
            if emitCaptureArtifacts then
                if not debugInfo then
                    error (Error(FSComp.SR.fscHotReloadRequiresDebugInfo (), rangeStartup))

                if localOptimizationsEnabled then
                    error (Error(FSComp.SR.fscHotReloadIncompatibleWithOptimization (), rangeStartup))

        member _.PrepareForCodeGeneration(emitCaptureArtifacts, compilerGlobalState) =
            if emitCaptureArtifacts then
                match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
                | Some map -> map.BeginSession()
                | None ->
                    let map = FSharpSynthesizedTypeMaps()
                    map.BeginSession()
                    setCompilerGeneratedNameMap (compilerGlobalState :> obj) (map :> ICompilerGeneratedNameMap)
            elif editAndContinueService.IsSessionActive then
                // Preserve synthesized-name replay while a hot reload session is active,
                // even when the output build itself is emitted without capture flags.
                let activeMap =
                    match tryGetCompilerGeneratedNameMap (compilerGlobalState :> obj) with
                    | Some existing -> Some existing
                    | None ->
                        match editAndContinueService.TryGetSession() with
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
                | None ->
                    clearCompilerGeneratedNameMap (compilerGlobalState :> obj)
            else
                clearCompilerGeneratedNameMap (compilerGlobalState :> obj)

        member _.BeforeFileEmit(emitCaptureArtifacts, compilerGlobalState) =
            // Only clear the hot reload session when NOT in capture mode.
            // In IDE scenarios, MSBuild may run in the background and we don't want
            // to clear an active hot reload session being used for live editing.
            if not emitCaptureArtifacts then
                editAndContinueService.EndSession()
                clearCompilerGeneratedNameMap (compilerGlobalState :> obj)

        // Emit through the in-memory writer first so disk bytes and baseline capture share
        // identical inputs; this avoids subtle drift from a second writer invocation.
        member _.TryEmitWithArtifacts(
            emitCaptureArtifacts,
            compilerGlobalState,
            ilWriteOptions,
            ilxMainModule,
            normalizeAssemblyRefs,
            optimizedImpls,
            ilxGenEnvSnapshot,
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
                    { IlxMainModule = ilxMainModule
                      TokenMappings = tokenMappings
                      AssemblyBytes = assemblyBytes
                      PortablePdbBytes = pdbBytesOpt
                      IlxGenEnvSnapshot = ilxGenEnvSnapshot
                      OptimizedImpls = optimizedImpls }

                true

        member _.CaptureArtifacts(compilerGlobalState, artifacts) =
            captureArtifacts compilerGlobalState artifacts

        member _.FallbackEmit(compilerGlobalState) =
            editAndContinueService.EndSession()
            clearCompilerGeneratedNameMap (compilerGlobalState :> obj)

let createHotReloadCompilerEmitHook (editAndContinueService: FSharpEditAndContinueLanguageService) : ICompilerEmitHook =
    DefaultHotReloadEmitHook(editAndContinueService) :> ICompilerEmitHook

let hotReloadCompilerEmitHook : ICompilerEmitHook =
    createHotReloadCompilerEmitHook FSharpEditAndContinueLanguageService.Instance
