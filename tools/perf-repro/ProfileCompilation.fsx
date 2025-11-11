#!/usr/bin/env dotnet fsi

// Profile F# compilation of xUnit test projects using dotnet-trace
// This script automates the profiling workflow for the Assert.Equal performance issue

open System
open System.IO
open System.Diagnostics

type ProfileConfig = {
    GeneratedDir: string
    OutputDir: string
    TotalAsserts: int
    MethodsCount: int
}

// Helper to run shell command and capture output
let runCommand workingDir command args =
    let psi = ProcessStartInfo()
    psi.FileName <- command
    psi.Arguments <- args
    psi.WorkingDirectory <- workingDir
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardError <- true
    psi.UseShellExecute <- false
    psi.CreateNoWindow <- true
    
    use proc = Process.Start(psi)
    let output = proc.StandardOutput.ReadToEnd()
    let error = proc.StandardError.ReadToEnd()
    proc.WaitForExit()
    
    (proc.ExitCode, output, error)

// Check if dotnet-trace is installed
let ensureDotnetTrace() =
    printfn "Checking for dotnet-trace..."
    let (exitCode, output, _) = runCommand "." "dotnet" "tool list -g"
    
    if not (output.Contains("dotnet-trace")) then
        printfn "dotnet-trace not found. Installing..."
        let (installCode, installOut, installErr) = runCommand "." "dotnet" "tool install -g dotnet-trace"
        if installCode <> 0 then
            printfn "Failed to install dotnet-trace:"
            printfn "%s" installErr
            false
        else
            printfn "dotnet-trace installed successfully"
            true
    else
        printfn "dotnet-trace is already installed"
        true

// Generate test projects
let generateProjects config =
    printfn "\n=== Generating Test Projects ==="
    let scriptPath = Path.Combine(__SOURCE_DIRECTORY__, "GenerateXUnitPerfTest.fsx")
    
    // Generate untyped version (slow path)
    printfn "\nGenerating untyped version (slow path)..."
    let untypedArgs = sprintf "--total %d --methods %d --output \"%s\" --untyped" 
                        config.TotalAsserts config.MethodsCount config.GeneratedDir
    let (exitCode1, output1, error1) = runCommand "." "dotnet" (sprintf "fsi \"%s\" %s" scriptPath untypedArgs)
    
    if exitCode1 <> 0 then
        printfn "Failed to generate untyped project:"
        printfn "%s" error1
        false
    else
        printfn "%s" output1
        
        // Generate typed version (fast path)
        printfn "\nGenerating typed version (fast path)..."
        let typedArgs = sprintf "--total %d --methods %d --output \"%s\" --typed" 
                          config.TotalAsserts config.MethodsCount config.GeneratedDir
        let (exitCode2, output2, error2) = runCommand "." "dotnet" (sprintf "fsi \"%s\" %s" scriptPath typedArgs)
        
        if exitCode2 <> 0 then
            printfn "Failed to generate typed project:"
            printfn "%s" error2
            false
        else
            printfn "%s" output2
            true

// Restore dependencies for a project
let restoreProject projectDir =
    printfn "\nRestoring dependencies for %s..." (Path.GetFileName(projectDir))
    let (exitCode, output, error) = runCommand projectDir "dotnet" "restore"
    
    if exitCode <> 0 then
        printfn "Failed to restore project:"
        printfn "%s" error
        false
    else
        printfn "Dependencies restored successfully"
        true

// Profile compilation of a project
let profileCompilation projectDir outputDir projectName =
    printfn "\n=== Profiling Compilation: %s ===" projectName
    
    let tracePath = Path.Combine(outputDir, sprintf "%s.nettrace" projectName)
    
    // Clean previous build
    printfn "Cleaning previous build..."
    let (cleanCode, _, _) = runCommand projectDir "dotnet" "clean"
    
    // Start dotnet-trace in the background
    printfn "Starting dotnet-trace..."
    
    // Build the project with tracing
    // We'll use a simpler approach: time the build and collect a trace separately
    let stopwatch = Stopwatch.StartNew()
    
    // For profiling compilation, we need to trace the dotnet build process
    // This is complex, so we'll use a simpler timing approach first
    let buildArgs = "build --no-restore -c Release /p:DebugType=None /p:DebugSymbols=false"
    
    printfn "Running: dotnet %s" buildArgs
    let buildStart = DateTime.Now
    let (buildCode, buildOutput, buildError) = runCommand projectDir "dotnet" buildArgs
    stopwatch.Stop()
    
    if buildCode <> 0 then
        printfn "Build failed:"
        printfn "%s" buildError
        (false, 0.0)
    else
        let compilationTime = stopwatch.Elapsed.TotalSeconds
        printfn "Compilation completed in %.2f seconds" compilationTime
        
        // Save timing information
        let timingPath = Path.Combine(outputDir, sprintf "%s.timing.txt" projectName)
        let timingInfo = sprintf "Compilation Time: %.2f seconds\nTime per Assert: %.2f ms\n" 
                           compilationTime ((compilationTime * 1000.0) / float config.TotalAsserts)
        File.WriteAllText(timingPath, timingInfo)
        
        (true, compilationTime)

// Profile compilation with dotnet-trace
let profileWithTrace projectDir outputDir projectName =
    printfn "\n=== Profiling with dotnet-trace: %s ===" projectName
    
    let tracePath = Path.Combine(outputDir, sprintf "%s.nettrace" projectName)
    
    // Clean previous build
    let (cleanCode, _, _) = runCommand projectDir "dotnet" "clean"
    
    // Create a temporary script to build and capture PID
    let buildScript = Path.Combine(Path.GetTempPath(), "build-with-trace.sh")
    let scriptContent = sprintf """#!/bin/bash
cd "%s"
dotnet build --no-restore -c Release /p:DebugType=None /p:DebugSymbols=false > build.log 2>&1
""" projectDir
    
    File.WriteAllText(buildScript, scriptContent)
    
    // We'll use a different approach: collect trace during build
    // Start trace, run build, stop trace
    let buildArgs = "build --no-restore -c Release /p:DebugType=None /p:DebugSymbols=false"
    
    // Collect trace by wrapping the build command
    let traceArgs = sprintf "collect -o \"%s\" --format speedscope -- dotnet %s" tracePath buildArgs
    
    printfn "Running: dotnet-trace %s" traceArgs
    let stopwatch = Stopwatch.StartNew()
    let (traceCode, traceOutput, traceError) = runCommand projectDir "dotnet-trace" traceArgs
    stopwatch.Stop()
    
    if traceCode <> 0 then
        printfn "Trace collection failed (this is expected on some systems):"
        printfn "%s" traceError
        printfn "Falling back to timing-only mode..."
        // Fallback to simple profiling
        profileCompilation projectDir outputDir projectName
    else
        let compilationTime = stopwatch.Elapsed.TotalSeconds
        printfn "Trace collected successfully: %s" tracePath
        printfn "Compilation time: %.2f seconds" compilationTime
        
        // Save timing information
        let timingPath = Path.Combine(outputDir, sprintf "%s.timing.txt" projectName)
        let timingInfo = sprintf "Compilation Time: %.2f seconds\nTime per Assert: %.2f ms\nTrace File: %s\n" 
                           compilationTime ((compilationTime * 1000.0) / float config.TotalAsserts) tracePath
        File.WriteAllText(timingPath, timingInfo)
        
        (true, compilationTime)

// Main profiling workflow
let runProfilingWorkflow config =
    printfn "=== F# Compilation Performance Profiling ==="
    printfn "Configuration:"
    printfn "  Total Assert.Equal calls: %d" config.TotalAsserts
    printfn "  Test methods: %d" config.MethodsCount
    printfn "  Generated projects: %s" config.GeneratedDir
    printfn "  Output directory: %s" config.OutputDir
    
    // Ensure output directory exists
    Directory.CreateDirectory(config.OutputDir) |> ignore
    
    // Check for dotnet-trace (optional, we can fall back to timing)
    let hasTrace = ensureDotnetTrace()
    
    // Generate test projects
    if not (generateProjects config) then
        printfn "\nFailed to generate test projects"
        false
    else
        let untypedDir = Path.Combine(config.GeneratedDir, "XUnitPerfTest.Untyped")
        let typedDir = Path.Combine(config.GeneratedDir, "XUnitPerfTest.Typed")
        
        // Restore dependencies for both projects
        printfn "\n=== Restoring Dependencies ==="
        if not (restoreProject untypedDir) then
            printfn "Failed to restore untyped project"
            false
        elif not (restoreProject typedDir) then
            printfn "Failed to restore typed project"
            false
        else
            // Profile both versions
            let profileFunc = if hasTrace then profileWithTrace else profileCompilation
            
            let (untypedSuccess, untypedTime) = profileFunc untypedDir config.OutputDir "XUnitPerfTest.Untyped"
            let (typedSuccess, typedTime) = profileFunc typedDir config.OutputDir "XUnitPerfTest.Typed"
            
            if untypedSuccess && typedSuccess then
                printfn "\n=== Profiling Complete ==="
                printfn "Untyped version: %.2f seconds (%.2f ms per Assert)" untypedTime ((untypedTime * 1000.0) / float config.TotalAsserts)
                printfn "Typed version: %.2f seconds (%.2f ms per Assert)" typedTime ((typedTime * 1000.0) / float config.TotalAsserts)
                printfn "Slowdown factor: %.2fx" (untypedTime / typedTime)
                printfn "\nResults saved to: %s" config.OutputDir
                
                // Save summary
                let summaryPath = Path.Combine(config.OutputDir, "summary.txt")
                let summary = sprintf """F# Compilation Performance Summary
=====================================

Configuration:
  Total Assert.Equal calls: %d
  Test methods: %d

Results:
  Untyped (slow path): %.2f seconds (%.2f ms per Assert)
  Typed (fast path):   %.2f seconds (%.2f ms per Assert)
  Slowdown factor:     %.2fx
  Time difference:     %.2f seconds

Output directory: %s
""" config.TotalAsserts config.MethodsCount untypedTime ((untypedTime * 1000.0) / float config.TotalAsserts) 
    typedTime ((typedTime * 1000.0) / float config.TotalAsserts) (untypedTime / typedTime) (untypedTime - typedTime) config.OutputDir
                
                File.WriteAllText(summaryPath, summary)
                printfn "\nSummary written to: %s" summaryPath
                true
            else
                printfn "\nProfiling failed"
                false

// CLI interface
let printUsage() =
    printfn """
Usage: dotnet fsi ProfileCompilation.fsx [options]

Options:
  --total <n>        Total number of Assert.Equal calls (default: 1500)
  --methods <n>      Number of test methods (default: 10)
  --generated <path> Directory for generated projects (default: ./generated)
  --output <path>    Output directory for results (default: ./results)
  --help             Show this help message

Example:
  dotnet fsi ProfileCompilation.fsx --total 1500 --methods 10
"""

// Parse command line arguments
let parseArgs (args: string[]) =
    let mutable totalAsserts = 1500
    let mutable methodsCount = 10
    let mutable generatedDir = "./generated"
    let mutable outputDir = "./results"
    let mutable i = 0
    
    while i < args.Length do
        match args.[i] with
        | "--total" when i + 1 < args.Length ->
            totalAsserts <- Int32.Parse(args.[i + 1])
            i <- i + 2
        | "--methods" when i + 1 < args.Length ->
            methodsCount <- Int32.Parse(args.[i + 1])
            i <- i + 2
        | "--generated" when i + 1 < args.Length ->
            generatedDir <- args.[i + 1]
            i <- i + 2
        | "--output" when i + 1 < args.Length ->
            outputDir <- args.[i + 1]
            i <- i + 2
        | "--help" ->
            printUsage()
            exit 0
        | _ ->
            printfn "Unknown argument: %s" args.[i]
            printUsage()
            exit 1
    
    {
        TotalAsserts = totalAsserts
        MethodsCount = methodsCount
        GeneratedDir = generatedDir
        OutputDir = outputDir
    }

// Main entry point
let main (args: string[]) =
    try
        if args |> Array.contains "--help" then
            printUsage()
            0
        else
            let config = parseArgs args
            if runProfilingWorkflow config then 0 else 1
    with
    | ex ->
        printfn "Error: %s" ex.Message
        printfn "%s" ex.StackTrace
        1

// Execute if running as script
let exitCode = main fsi.CommandLineArgs.[1..]
exit exitCode
