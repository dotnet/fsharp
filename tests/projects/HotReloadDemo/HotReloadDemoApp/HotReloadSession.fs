#nowarn "57"

namespace HotReloadDemoApp

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.Loader
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

type ApplyDeltaOutcome =
    | Applied of FSharpHotReloadDelta
    | NoChanges
    | CompilationFailed of FSharpDiagnostic[]
    | HotReloadError of string

type DemoSession =
    { Checker: FSharpChecker
      ProjectOptions: FSharpProjectOptions
      SourcePath: string
      BaselineDllPath: string
      RuntimeDllPath: string
      RuntimeAssembly: Assembly
      LoadContext: AssemblyLoadContext option
      WorkingDirectory: string
      mutable Generation: int
      DeltaDumpHistory: ResizeArray<string * string> }

module HotReloadSession =

    [<Literal>]
    let private sampleFileName = "DemoTarget.fs"

    let private sampleSourceDirectory = __SOURCE_DIRECTORY__

    let private shouldDumpDeltas () =
        Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_DUMP_DELTA") = "1"

    let private shouldRunMdv () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_RUN_MDV") with
        | null -> false
        | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
        | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    let private getMdvToolPath () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_MDV_PATH") with
        | null
        | "" -> "mdv"
        | path -> path

    let private shouldTraceRuntimeApply () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_RUNTIME_APPLY") with
        | null -> false
        | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
        | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    let private ensureDirectory (path: string) =
        Directory.CreateDirectory(path) |> ignore

    let private copySampleSource destination =
        let sourcePath = Path.Combine(sampleSourceDirectory, sampleFileName)
        File.Copy(sourcePath, destination, true)

    let private createChecker () =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false
        )

    let private prepareProjectOptions
        (checker: FSharpChecker)
        (sourcePath: string)
        (outputPath: string)
        =
        async {
            let sourceText = SourceText.ofString(File.ReadAllText(sourcePath))

            let! projectOptions, _ =
                checker.GetProjectOptionsFromScript(
                    sourcePath,
                    sourceText,
                    assumeDotNetFramework = false,
                    useSdkRefs = true,
                    useFsiAuxLib = false
                )

            let otherOptions =
                projectOptions.OtherOptions
                |> Array.append
                    [| "--target:library"
                       "--langversion:preview"
                       "--debug:portable"
                       "--optimize-"
                       "--deterministic"
                       "--define:HOT_RELOAD_DEMO"
                       "--enable:hotreloaddeltas"
                       $"--out:{outputPath}" |]

            return
                { projectOptions with
                    SourceFiles = [| sourcePath |]
                    OtherOptions = otherOptions }
        }

    let private compileProject
        (checker: FSharpChecker)
        (projectOptions: FSharpProjectOptions)
        (includeHotReloadCapture: bool)
        =
        async {
            let otherOptions =
                if includeHotReloadCapture then
                    projectOptions.OtherOptions
                else
                    projectOptions.OtherOptions
                    |> Array.filter (fun opt ->
                        not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase)))

            let argv =
                Array.concat
                    [ [| "fsc.exe" |]
                      otherOptions
                      projectOptions.SourceFiles ]

            let! diagnostics, exitCodeOpt = checker.Compile(argv)

            let errors =
                diagnostics
                |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

            match errors, exitCodeOpt with
            | [||], None -> return Ok ()
            | _ -> return Error diagnostics
        }

    type DemoInitializationError =
        | BaselineCompilationFailed of FSharpDiagnostic[]
        | HotReloadSessionFailed of FSharpHotReloadError
        | AssemblyLoadFailed of string

    let initialize () =
        async {
            let checker = createChecker ()

            let workingDirectory =
                Path.Combine(Path.GetTempPath(), "fsharp-hotreload-demo", Guid.NewGuid().ToString("N"))

            let sourcePath = Path.Combine(workingDirectory, sampleFileName)
            let outputDirectory = Path.Combine(workingDirectory, "build")
            let runtimeDirectory = Path.Combine(workingDirectory, "runtime")

            ensureDirectory workingDirectory
            ensureDirectory outputDirectory
            ensureDirectory runtimeDirectory

            copySampleSource sourcePath

            let baselineDllPath = Path.Combine(outputDirectory, "DemoTarget.dll")
            let runtimeDllPath = Path.Combine(runtimeDirectory, "DemoTarget.runtime.dll")

            checker.InvalidateAll()

            let! projectOptions = prepareProjectOptions checker sourcePath baselineDllPath

            let! baselineResult = compileProject checker projectOptions true

            match baselineResult with
            | Error diagnostics -> return Error(BaselineCompilationFailed diagnostics)
            | Ok () ->
                let! sessionResult = checker.StartHotReloadSession(projectOptions)

                match sessionResult with
                | Error error -> return Error(HotReloadSessionFailed error)
                | Ok () ->
                    try
                        File.Copy(baselineDllPath, runtimeDllPath, true)

                        let baselinePdbPath =
                            match Path.ChangeExtension(baselineDllPath, ".pdb") with
                            | null -> None
                            | value -> Some value

                        match baselinePdbPath with
                        | Some pdbPath when File.Exists(pdbPath) ->
                            match Path.GetFileName(pdbPath) with
                            | null -> ()
                            | pdbName -> File.Copy(pdbPath, Path.Combine(runtimeDirectory, pdbName), true)
                        | _ -> ()

                        let runtimeAssembly = Assembly.LoadFrom(runtimeDllPath)
                        let loadContext =
                            match AssemblyLoadContext.GetLoadContext(runtimeAssembly) with
                            | null -> None
                            | ctx when ctx.IsCollectible -> Some ctx
                            | _ -> None

                        return
                            Ok
                                { Checker = checker
                                  ProjectOptions = projectOptions
                                  SourcePath = sourcePath
                                  BaselineDllPath = baselineDllPath
                                  RuntimeDllPath = runtimeDllPath
                                  RuntimeAssembly = runtimeAssembly
                                  LoadContext = loadContext
                                  WorkingDirectory = workingDirectory
                                  Generation = 0
                                  DeltaDumpHistory = ResizeArray() }
                    with ex ->
                        return Error(AssemblyLoadFailed ex.Message)
        }

    let applyDelta (session: DemoSession) (applyRuntimeUpdate: bool) =
        async {
            do!
                session.Checker.NotifyFileChanged(session.SourcePath, session.ProjectOptions)
                |> Async.Ignore

            let! compileResult = compileProject session.Checker session.ProjectOptions false

            match compileResult with
            | Error diagnostics -> return CompilationFailed diagnostics
            | Ok () ->
                let! deltaResult = session.Checker.EmitHotReloadDelta(session.ProjectOptions)

                match deltaResult with
                | Error FSharpHotReloadError.NoChanges -> return NoChanges
                | Error (FSharpHotReloadError.CompilationFailed diagnostics) ->
                    return CompilationFailed diagnostics
                | Error (FSharpHotReloadError.UnsupportedEdit message) ->
                    return HotReloadError $"Unsupported edit: {message}"
                | Error (FSharpHotReloadError.DeltaEmissionFailed message) ->
                    return HotReloadError $"Delta emission failed: {message}"
                | Error FSharpHotReloadError.NoActiveSession ->
                    return HotReloadError "Hot reload session is no longer active."
                | Error FSharpHotReloadError.MissingOutputPath ->
                    return HotReloadError "Project options are missing an output path."
                | Ok delta ->
                    let dumpDirRequired = shouldDumpDeltas () || shouldRunMdv ()

                    if dumpDirRequired then
                        try
                            let nextGeneration = session.Generation + 1
                            let dumpRoot = Path.Combine(session.WorkingDirectory, "delta-dump")
                            let generationDirName = sprintf "gen-%03d" nextGeneration
                            let generationDir = Path.Combine(dumpRoot, generationDirName)
                            Directory.CreateDirectory(generationDir) |> ignore
                            let write (name: string) (bytes: byte[]) =
                                let path = Path.Combine(generationDir, name)
                                File.WriteAllBytes(path, bytes)
                                path
                            let metadataPath = write "metadata.bin" delta.Metadata
                            let ilPath = write "il.bin" delta.IL
                            delta.Pdb |> Option.iter (fun bytes -> write "pdb.bin" bytes |> ignore)
                            File.WriteAllLines(
                                Path.Combine(generationDir, "tokens.txt"),
                                [| sprintf "Updated methods: %A" delta.UpdatedMethods
                                   sprintf "Updated types: %A" delta.UpdatedTypes
                                   sprintf "Generation: %O" delta.GenerationId
                                   sprintf "Base generation: %O" delta.BaseGenerationId |])
                            session.DeltaDumpHistory.Add(metadataPath, ilPath)

                            let mdvArgs =
                                session.DeltaDumpHistory
                                |> Seq.map (fun (metaPath, ilPath) -> $"\"/g:{metaPath};{ilPath}\"")
                                |> Seq.toArray

                            let mdvArgsJoined = String.Join(" ", mdvArgs)

                            let mdvCommand =
                                if String.IsNullOrEmpty mdvArgsJoined then
                                    sprintf "mdv \"%s\"" session.BaselineDllPath
                                else
                                    sprintf "mdv \"%s\" %s" session.BaselineDllPath mdvArgsJoined

                            printfn "[hotreload-delta] %s" mdvCommand

                            if shouldRunMdv () then
                                let psi = ProcessStartInfo()
                                psi.FileName <- getMdvToolPath ()
                                psi.UseShellExecute <- false
                                psi.RedirectStandardOutput <- true
                                psi.RedirectStandardError <- true
                                psi.WorkingDirectory <- generationDir
                                psi.ArgumentList.Add(session.BaselineDllPath)
                                for (metaPath, ilPath) in session.DeltaDumpHistory do
                                    psi.ArgumentList.Add($"/g:{metaPath};{ilPath}")

                                try
                                    let procInstance = Process.Start(psi)
                                    if isNull procInstance then
                                        printfn "[hotreload-mdv] failed to start mdv (Process.Start returned null)"
                                    else
                                        use proc = procInstance
                                        let stdOutTask = proc.StandardOutput.ReadToEndAsync()
                                        let stdErrTask = proc.StandardError.ReadToEndAsync()
                                        proc.WaitForExit()
                                        stdOutTask.Wait()
                                        stdErrTask.Wait()
                                        let stdOut = stdOutTask.Result.TrimEnd()
                                        let stdErr = stdErrTask.Result.TrimEnd()
                                        if stdOut.Length > 0 then
                                            printfn "[hotreload-mdv] %s" stdOut
                                        if stdErr.Length > 0 then
                                            printfn "[hotreload-mdv][stderr] %s" stdErr
                                        if proc.ExitCode <> 0 then
                                            printfn "[hotreload-mdv] mdv exited with code %d" proc.ExitCode
                                with mdvEx ->
                                    printfn "[hotreload-mdv] failed to run mdv: %s" mdvEx.Message
                        with dumpEx ->
                            printfn "Failed to dump delta artifacts: %s" dumpEx.Message

                    if not applyRuntimeUpdate then
                        session.Generation <- session.Generation + 1
                        return Applied delta
                    else
                        if not System.Reflection.Metadata.MetadataUpdater.IsSupported then
                            return HotReloadError "MetadataUpdater reports that runtime apply is not supported in this process."
                        else
                            let pdbBytes =
                                match delta.Pdb with
                                | Some bytes -> bytes
                                | None -> Array.empty<byte>

                            if shouldTraceRuntimeApply () then
                                printfn
                                    "[hotreload-runtime] applying delta gen=%d metadata=%dB il=%dB pdb=%dB"
                                    session.Generation
                                    delta.Metadata.Length
                                    delta.IL.Length
                                    pdbBytes.Length

                            try
                                System.Reflection.Metadata.MetadataUpdater.ApplyUpdate(
                                    session.RuntimeAssembly,
                                    delta.Metadata,
                                    delta.IL,
                                    pdbBytes
                                )

                                session.Generation <- session.Generation + 1
                                return Applied delta
                            with ex ->
                                if shouldTraceRuntimeApply () then
                                    printfn "[hotreload-runtime] ApplyUpdate exception: %s" (ex.ToString())

                                let innerSummary =
                                    match ex.InnerException with
                                    | null -> None
                                    | inner ->
                                        let innerInfo =
                                            sprintf
                                                "%s HR=0x%08X msg=%s"
                                                (inner.GetType().FullName)
                                                inner.HResult
                                                inner.Message
                                        Some innerInfo

                                let detailedMessage =
                                    match innerSummary with
                                    | Some innerInfo ->
                                        sprintf
                                            "MetadataUpdater.ApplyUpdate failed (HR=0x%08X): %s | inner=%s"
                                            ex.HResult
                                            ex.Message
                                            innerInfo
                                    | None ->
                                        sprintf "MetadataUpdater.ApplyUpdate failed (HR=0x%08X): %s" ex.HResult ex.Message

                                return HotReloadError detailedMessage
        }

    let dispose (session: DemoSession) =
        try
            session.Checker.EndHotReloadSession()
        with _ ->
            ()

        match session.LoadContext with
        | Some alc when alc.IsCollectible ->
            try
                alc.Unload()
            with _ ->
                ()
        | _ -> ()

        if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_KEEP_WORKDIR") <> "1" then
            try
                if Directory.Exists session.WorkingDirectory then
                    Directory.Delete(session.WorkingDirectory, true)
            with _ ->
                ()
