module HotReloadDemoApp.Program

open System
open System.IO
open System.Reflection
open HotReloadDemoApp

type RunMode =
    | Auto
    | Scripted
    | Interactive

module private ConsoleHelpers =
    open FSharp.Compiler.CodeAnalysis
    open FSharp.Compiler.Diagnostics

    let private shouldTraceUserStrings () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_STRINGS") with
        | null -> false
        | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
        | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    let writeDiagnostics (diagnostics: FSharpDiagnostic[]) =
        if diagnostics.Length = 0 then
            ()
        else
            printfn ""
            printfn "Compilation failed with %d diagnostic(s):" diagnostics.Length
            for diagnostic in diagnostics do
                let range = diagnostic.Range
                printfn
                    "  %s(%d,%d): %A %s"
                    diagnostic.FileName
                    range.StartLine
                    range.StartColumn
                    diagnostic.Severity
                    diagnostic.Message
            printfn ""

    let printDeltaSummary (delta: FSharpHotReloadDelta) generation =
        printfn "Δ applied. Generation: %O (base %O)" delta.GenerationId delta.BaseGenerationId
        if delta.UpdatedMethods.Length > 0 then
            printfn "  Updated methods: %A" delta.UpdatedMethods
        if delta.AddedOrChangedMethods.Length > 0 then
            printfn "  Added/changed method details:"
            delta.AddedOrChangedMethods
            |> List.iter (fun info ->
                printfn
                    "    token=0x%08X locals=0x%08X offset=%d length=%d"
                    info.MethodToken
                    info.LocalSignatureToken
                    info.CodeOffset
                    info.CodeLength)
        if shouldTraceUserStrings () && delta.UserStringUpdates.Length > 0 then
            printfn "  Updated user strings:"
            delta.UserStringUpdates
            |> List.iter (fun struct (_, _, literal) -> printfn "    \"%s\"" literal)
        if delta.UpdatedTypes.Length > 0 then
            printfn "  Updated types: %A" delta.UpdatedTypes
        printfn "  Session generation counter: %d" generation

module private DemoInvoker =
    let getMessage (assembly: Assembly) =
        let moduleType = assembly.GetType("HotReloadDemo.Target.Demo", throwOnError = false)

        match moduleType with
        | null -> None
        | typ ->
            let methodInfo =
                typ.GetMethod(
                    "GetMessage",
                    BindingFlags.Public ||| BindingFlags.Static
                )

            match methodInfo with
            | null -> None
            | info ->
                try
                    info.Invoke(null, Array.empty) |> string |> Some
                with ex ->
                    let detail =
                        match ex.InnerException with
                        | null -> ex.Message
                        | inner -> sprintf "%s (inner: %s: %s)" ex.Message (inner.GetType().FullName) inner.Message
                    printfn "Invocation of Demo.GetMessage failed: %s" detail
                    None

let private runNonInteractive description applyRuntimeUpdate multiDelta (session: DemoSession) =
    let originalSource = File.ReadAllText(session.SourcePath)

    let generationTargets =
        if multiDelta then
            [ 1; 2 ]
        else
            [ 1 ]

    let runtimeStatus = if applyRuntimeUpdate then "enabled" else "skipped"

    let exitCode =
        try
            let baselineMessage =
                DemoInvoker.getMessage session.RuntimeAssembly
                |> Option.defaultValue "<baseline message unavailable>"

            printfn "[%s] Baseline message: %s" description baselineMessage
            printfn "[%s] Editing source at %s" description session.SourcePath
            printfn
                "[%s] Planning to emit %d delta(s): %s"
                description
                generationTargets.Length
                (generationTargets |> List.map string |> String.concat ", ")

            let rec applyDeltas (currentSource: string) (previousGeneration: int) (emittedCount: int) (remainingTargets: int list) =
                match remainingTargets with
                | [] ->
                    let summaryLabel =
                        if String.Equals(description, "script", StringComparison.OrdinalIgnoreCase) then
                            "Scripted run"
                        elif String.Equals(description, "auto", StringComparison.OrdinalIgnoreCase) then
                            "Auto run"
                        else
                            "Non-interactive run"

                    printfn
                        "[%s] %s succeeded: emitted %d delta(s) (runtime apply %s)."
                        description
                        summaryLabel
                        emittedCount
                        runtimeStatus
                    0
                | targetGeneration :: rest ->
                    let expectedMarker = $"generation {previousGeneration}"
                    let replacement = $"generation {targetGeneration}"
                    let markerIndex = currentSource.IndexOf(expectedMarker, StringComparison.Ordinal)

                    if markerIndex = -1 then
                        printfn
                            "[%s] Failed: source did not contain the expected marker '%s'."
                            description
                            expectedMarker

                        if emittedCount = 0 then
                            7
                        else
                            9
                    else
                        let patchedSource =
                            currentSource.Substring(0, markerIndex)
                            + replacement
                            + currentSource.Substring(markerIndex + expectedMarker.Length)

                        if String.Equals(patchedSource, currentSource, StringComparison.Ordinal) then
                            printfn
                                "[%s] Failed: source text was unchanged after attempting to set '%s'."
                                description
                                replacement

                            if emittedCount = 0 then
                                8
                            else
                                10
                        else
                            File.WriteAllText(session.SourcePath, patchedSource)

                            printfn
                                "[%s] Patched source written for generation %d -> %d; invoking EmitHotReloadDelta..."
                                description
                                previousGeneration
                                targetGeneration

                            match HotReloadSession.applyDelta session applyRuntimeUpdate |> Async.RunSynchronously with
                            | ApplyDeltaOutcome.Applied delta ->
                                ConsoleHelpers.printDeltaSummary delta session.Generation

                                let runtimeCheckResult =
                                    if applyRuntimeUpdate then
                                        match DemoInvoker.getMessage session.RuntimeAssembly with
                                        | Some message when message.Contains(replacement, StringComparison.Ordinal) ->
                                            printfn
                                                "[%s] Hot reload applied (delta #%d): %s"
                                                description
                                                (emittedCount + 1)
                                                message
                                            None
                                        | Some message ->
                                            printfn
                                                "[%s] Hot reload applied but message did not reflect '%s': %s"
                                                description
                                                replacement
                                                message
                                            Some 1
                                        | None ->
                                            printfn
                                                "[%s] Hot reload applied but Demo.GetMessage returned None"
                                                description
                                            Some 2
                                    else
                                        printfn
                                            "[%s] Delta #%d emitted (runtime apply %s)."
                                            description
                                            (emittedCount + 1)
                                            runtimeStatus
                                        None

                                match runtimeCheckResult with
                                | Some code -> code
                                | None -> applyDeltas patchedSource targetGeneration (emittedCount + 1) rest
                            | ApplyDeltaOutcome.NoChanges ->
                                printfn "[%s] Failed: delta reported no changes." description
                                4
                            | ApplyDeltaOutcome.CompilationFailed diagnostics ->
                                ConsoleHelpers.writeDiagnostics diagnostics
                                5
                            | ApplyDeltaOutcome.HotReloadError message ->
                                printfn "[%s] Failed: %s" description message
                                6

            applyDeltas originalSource 0 0 generationTargets
        finally
            File.WriteAllText(session.SourcePath, originalSource)
            HotReloadSession.dispose session

    exitCode

let private runInteractive (session: DemoSession) =
    printfn "Working directory: %s" session.WorkingDirectory
    printfn "Edit the file below and press Enter to apply a delta:"
    printfn "  %s" session.SourcePath
    printfn ""

    DemoInvoker.getMessage session.RuntimeAssembly
    |> Option.iter (printfn "Current output: %s")

    let rec loop () =
        printf "Press Enter to recompile (or type 'q' to quit) > "
        let input = Console.ReadLine()

        if String.Equals(input, "q", StringComparison.OrdinalIgnoreCase) then
            ()
        else
            match HotReloadSession.applyDelta session true |> Async.RunSynchronously with
            | ApplyDeltaOutcome.Applied delta ->
                ConsoleHelpers.printDeltaSummary delta session.Generation
                DemoInvoker.getMessage session.RuntimeAssembly
                |> Option.iter (printfn "Updated output: %s")
                printfn ""
                loop ()
            | ApplyDeltaOutcome.NoChanges ->
                printfn "No changes detected. Make sure you saved the file before retrying."
                printfn ""
                loop ()
            | ApplyDeltaOutcome.CompilationFailed diagnostics ->
                ConsoleHelpers.writeDiagnostics diagnostics
                loop ()
            | ApplyDeltaOutcome.HotReloadError message ->
                printfn "Hot reload failed: %s" message
                printfn ""
                loop ()

    try
        loop ()
        0
    finally
        HotReloadSession.dispose session

[<EntryPoint>]
let main argv =
    let hasFlag flag =
        argv
        |> Array.exists (fun arg -> String.Equals(arg, flag, StringComparison.OrdinalIgnoreCase))

    let mode =
        if hasFlag "--interactive" then
            RunMode.Interactive
        elif hasFlag "--scripted" then
            RunMode.Scripted
        else
            RunMode.Auto

    let multiDelta = hasFlag "--multi-delta"
    let runtimeApply =
        if hasFlag "--runtime-apply" then true else (match mode with | RunMode.Interactive -> true | _ -> false)
    let keepWorkdirFlag = hasFlag "--keep-workdir"

    let modifiableAssemblies = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")

    if not (String.Equals(modifiableAssemblies, "debug", StringComparison.OrdinalIgnoreCase)) then
        printfn "DOTNET_MODIFIABLE_ASSEMBLIES must be set to 'debug' before launching the demo."
        printfn "Example: DOTNET_MODIFIABLE_ASSEMBLIES=debug ../../../../.dotnet/dotnet run"
        1
    else
        printfn "==============================================="
        printfn "F# Hot Reload Demo (FSharpChecker prototype)"
        printfn "==============================================="
        printfn ""
        printfn "This sample compiles a small library using the F# compiler's hot reload APIs."
        printfn "Tip: set FSHARP_HOTRELOAD_TRACE_STRINGS=1 to log user-string updates, and"
        printfn "     FSHARP_HOTRELOAD_KEEP_WORKDIR=1 if you want to keep the temporary working directory."
        printfn "     (You can also pass --keep-workdir to this demo to set the env var automatically.)"
        printfn ""

        match mode with
        | RunMode.Interactive ->
            printfn "Edit the generated source file, save it, and press Enter to emit a delta."
            printfn "Avoid signature changes to stay within supported method-body edits."
            if multiDelta then
                printfn "The --multi-delta flag is ignored in interactive mode."
            if not runtimeApply then
                printfn "Runtime apply is always enabled in interactive mode."
            printfn ""
        | RunMode.Auto ->
            printfn "Running in auto mode: the demo will edit the generated source automatically and emit a delta."
            printfn "Avoid signature changes—the automation only validates method-body edits."
            if multiDelta then
                printfn "Multi-delta coverage is enabled; the automation will emit multiple generations."
            if runtimeApply then
                printfn "Runtime apply enabled; MetadataUpdater.ApplyUpdate will be invoked."
            printfn ""
        | RunMode.Scripted ->
            printfn "Running in scripted mode for automation."
            if multiDelta then
                printfn "Multi-delta coverage is enabled; the automation will emit multiple generations."
            if runtimeApply then
                printfn "Runtime apply enabled; MetadataUpdater.ApplyUpdate will be invoked."
            printfn ""

        match HotReloadSession.initialize () |> Async.RunSynchronously with
        | Error (HotReloadSession.DemoInitializationError.BaselineCompilationFailed diagnostics) ->
            ConsoleHelpers.writeDiagnostics diagnostics
            printfn "Baseline compilation failed; unable to start the demo."
            1
        | Error (HotReloadSession.DemoInitializationError.HotReloadSessionFailed error) ->
            printfn "Failed to start hot reload session: %A" error
            1
        | Error (HotReloadSession.DemoInitializationError.AssemblyLoadFailed message) ->
            printfn "Failed to load baseline assembly: %s" message
            1
        | Ok session ->
            if keepWorkdirFlag then
                Environment.SetEnvironmentVariable("FSHARP_HOTRELOAD_KEEP_WORKDIR", "1")
            match mode with
            | RunMode.Scripted -> runNonInteractive "script" runtimeApply multiDelta session
            | RunMode.Auto -> runNonInteractive "auto" runtimeApply multiDelta session
            | RunMode.Interactive -> runInteractive session
