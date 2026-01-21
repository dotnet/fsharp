#!/usr/bin/env dotnet fsi
// Profiles F# compilation of typed vs untyped xUnit test projects
// Usage: dotnet fsi PerfProfiler.fsx --total 1500

open System
open System.IO
open System.Diagnostics

type Config = { Total: int; Methods: int; Output: string }

let run dir (cmd: string) (args: string) =
    let psi = ProcessStartInfo(cmd, args, WorkingDirectory = dir,
        RedirectStandardOutput = true, RedirectStandardError = true,
        UseShellExecute = false, CreateNoWindow = true)
    use p = Process.Start(psi)
    let out = p.StandardOutput.ReadToEnd()
    let err = p.StandardError.ReadToEnd()
    p.WaitForExit()
    (p.ExitCode, out, err)

let generateProject cfg typed =
    let genScript = Path.Combine(__SOURCE_DIRECTORY__, "PerfTestGenerator.fsx")
    let genDir = Path.Combine(cfg.Output, "generated")
    let flag = if typed then "--typed" else "--untyped"
    let (code, _, err) = run "." "dotnet" $"fsi \"{genScript}\" --total {cfg.Total} --methods {cfg.Methods} --output \"{genDir}\" {flag}"
    if code <> 0 then failwith $"Generation failed: {err}"
    let name = if typed then "XUnitPerfTest.Typed" else "XUnitPerfTest.Untyped"
    Path.Combine(genDir, name)

let profileBuild dir name total =
    printfn "Profiling: %s" name
    let (_, _, _) = run dir "dotnet" "restore --quiet"
    let (_, _, _) = run dir "dotnet" "clean --quiet"
    let sw = Stopwatch.StartNew()
    let (code, _, err) = run dir "dotnet" "build --no-restore -c Release /p:DebugType=None"
    sw.Stop()
    if code <> 0 then printfn "Build failed: %s" err; None
    else
        let secs = sw.Elapsed.TotalSeconds
        let perCall = (secs * 1000.0) / float total
        printfn "  Time: %.2fs (%.2f ms/call)" secs perCall
        Some secs

let profile cfg =
    printfn "=== F# Compilation Performance Profiling ==="
    printfn "Total Assert.Equal calls: %d" cfg.Total
    Directory.CreateDirectory(cfg.Output) |> ignore

    let untypedDir = generateProject cfg false
    let typedDir = generateProject cfg true

    match profileBuild untypedDir "Untyped" cfg.Total, profileBuild typedDir "Typed" cfg.Total with
    | Some ut, Some t ->
        printfn "\n=== Results ==="
        printfn "Untyped: %.2fs (%.2f ms/call)" ut ((ut * 1000.0) / float cfg.Total)
        printfn "Typed:   %.2fs (%.2f ms/call)" t ((t * 1000.0) / float cfg.Total)
        printfn "Ratio:   %.2fx" (ut / t)
        let summary = $"Untyped: {ut:F2}s\nTyped: {t:F2}s\nRatio: {ut/t:F2}x"
        File.WriteAllText(Path.Combine(cfg.Output, "summary.txt"), summary)
    | _ -> printfn "Profiling failed"

let parseArgs (args: string[]) =
    let mutable total, methods, output = 1500, 10, "./results"
    let mutable i = 0
    while i < args.Length do
        match args.[i] with
        | "--total" -> total <- int args.[i+1]; i <- i + 2
        | "--methods" -> methods <- int args.[i+1]; i <- i + 2
        | "--output" -> output <- args.[i+1]; i <- i + 2
        | "--help" -> printfn "Usage: dotnet fsi PerfProfiler.fsx --total N [--output DIR]"; exit 0
        | _ -> printfn "Unknown: %s" args.[i]; exit 1
    { Total = total; Methods = methods; Output = output }

try profile (parseArgs fsi.CommandLineArgs.[1..])
with ex -> printfn "Error: %s" ex.Message; exit 1
