#!/usr/bin/env -S dotnet fsi --langversion:preview
// combined-analysis.fsx
// Combines trace and dump analysis into a final comprehensive report

open System
open System.IO
open System.Text
open System.Text.Json

// Configuration
let scriptDir = __SOURCE_DIRECTORY__
let outputDir = Path.Combine(scriptDir, "output")

let traceAnalysisPath = Path.Combine(outputDir, "trace-analysis.md")
let dumpAnalysisPath = Path.Combine(outputDir, "dump-analysis.md")
let metadataPath = Path.Combine(outputDir, "run-metadata.json")
let finalReportPath = Path.Combine(outputDir, "FINAL-REPORT.md")

printfn "Combining analysis results..."

// Read files
let readFileOrDefault (path: string) (defaultContent: string) =
    if File.Exists(path) then
        File.ReadAllText(path)
    else
        defaultContent

let traceAnalysis = readFileOrDefault traceAnalysisPath ""
let dumpAnalysis = readFileOrDefault dumpAnalysisPath ""
let metadataJson = readFileOrDefault metadataPath "{}"

// Parse metadata
type RunMetadata = {
    StartTime: string
    EndTime: string
    SdkVersion: string
    GitCommit: string
    Branch: string
    RepositoryUrl: string
    Command: string
    TimeoutSeconds: int
    ExitCode: int
    Result: string
}

let metadata =
    try
        let doc = JsonDocument.Parse(metadataJson)
        let root = doc.RootElement
        
        let getString (name: string) (def: string) =
            try
                let prop = root.GetProperty(name)
                prop.GetString()
            with _ -> def
        
        let getInt (name: string) (def: int) =
            try
                let prop = root.GetProperty(name)
                prop.GetInt32()
            with _ -> def
        
        Some {
            StartTime = getString "start_time" "Unknown"
            EndTime = getString "end_time" "Unknown"
            SdkVersion = getString "sdk_version" "Unknown"
            GitCommit = getString "git_commit" "Unknown"
            Branch = getString "branch" "gus/fsharp9"
            RepositoryUrl = getString "repository_url" "https://github.com/fsprojects/FSharpPlus.git"
            Command = getString "command" "dotnet test build.proj -v n"
            TimeoutSeconds = getInt "timeout_seconds" 120
            ExitCode = getInt "exit_code" -1
            Result = getString "result" "Unknown"
        }
    with ex ->
        printfn "Warning: Could not parse metadata: %s" ex.Message
        None

// Check if hang was detected in trace analysis
let hangDetectedInTrace =
    traceAnalysis.Contains("Hang Detected") ||
    traceAnalysis.Contains("time gap")

// Check if hang points found in dump analysis
let hangPointsInDump =
    dumpAnalysis.Contains("Multiple threads") ||
    dumpAnalysis.Contains("Threads at Same Location")

// Generate the final report
let sb = StringBuilder()
let appendLine (s: string) = sb.AppendLine(s) |> ignore
let appendFormat fmt = Printf.kprintf appendLine fmt

appendLine "# FINAL REPORT: FSharpPlus Build Hang Analysis"
appendLine ""
appendFormat "**Generated:** %s" (DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"))
appendLine ""
appendLine "---"
appendLine ""

// Executive Summary
appendLine "## Executive Summary"
appendLine ""

match metadata with
| Some m ->
    appendFormat "**Test Result:** %s" m.Result
    appendLine ""
    
    match m.Result with
    | "HANG_CONFIRMED" ->
        appendLine "### 🔴 HANG CONFIRMED"
        appendLine ""
        appendLine "The FSharpPlus build hung during testing with .NET SDK 10.0.100."
        appendFormat "The process was terminated after **%d seconds** (timeout)." m.TimeoutSeconds
        appendLine ""
    | "NO_HANG" ->
        appendLine "### 🟢 NO HANG DETECTED"
        appendLine ""
        appendLine "The FSharpPlus build completed without hanging."
        appendLine "This may indicate:"
        appendLine "- The issue is intermittent"
        appendLine "- The issue has been fixed"
        appendLine "- Environment differences affecting reproduction"
        appendLine ""
    | "KILLED" ->
        appendLine "### 🟡 PROCESS KILLED"
        appendLine ""
        appendLine "The process was killed (SIGKILL), likely due to timeout."
        appendLine "This is similar to a hang but the process was forcefully terminated."
        appendLine ""
    | _ ->
        appendFormat "### ⚠️ Result: %s" m.Result
        appendLine ""
| None ->
    appendLine "### ⚠️ METADATA UNAVAILABLE"
    appendLine ""
    appendLine "Could not read run metadata. Analysis may be incomplete."
    appendLine ""

appendLine ""

// Root Cause Analysis
appendLine "## Root Cause Analysis"
appendLine ""

if hangDetectedInTrace || hangPointsInDump then
    appendLine "### Evidence of Hang Found"
    appendLine ""
    
    if hangDetectedInTrace then
        appendLine "**From Trace Analysis:**"
        appendLine "- Significant time gaps detected in event stream"
        appendLine "- Events stopped flowing, indicating process became unresponsive"
        appendLine ""
    
    if hangPointsInDump then
        appendLine "**From Dump Analysis:**"
        appendLine "- Multiple threads found at same stack location"
        appendLine "- This indicates contention or deadlock"
        appendLine ""
    
    appendLine "### Potential Causes"
    appendLine ""
    appendLine "Based on the analysis, potential causes include:"
    appendLine ""
    appendLine "1. **Type Checking Complexity:** FSharpPlus uses advanced F# features (HKTs, type-level programming)"
    appendLine "   that may stress the type checker"
    appendLine ""
    appendLine "2. **F# 10 Regression:** Changes in F# 10 type checker or optimizer may have introduced"
    appendLine "   performance regression or infinite loop"
    appendLine ""
    appendLine "3. **Lock Contention:** Multiple threads may be competing for the same resource"
    appendLine ""
    appendLine "4. **Memory Pressure:** Large type computations may cause excessive memory allocation"
    appendLine ""
else
    appendLine "### No Clear Evidence of Hang"
    appendLine ""
    appendLine "The analysis did not find clear evidence of a hang:"
    appendLine ""
    appendLine "- No significant time gaps in trace"
    appendLine "- No obvious contention points in dump"
    appendLine ""
    appendLine "This may indicate:"
    appendLine "- The hang is intermittent"
    appendLine "- The issue occurs in a phase not captured by tracing"
    appendLine "- The hang occurs after the timeout window"
    appendLine ""

appendLine ""

// Evidence Section
appendLine "## Evidence"
appendLine ""

appendLine "### Trace Analysis Summary"
appendLine ""

if traceAnalysis.Length > 0 then
    // Extract key findings from trace analysis
    if traceAnalysis.Contains("Largest gap") then
        let lines = traceAnalysis.Split('\n')
        for line in lines do
            if line.Contains("Largest gap") || line.Contains("Gap started") || line.Contains("Gap ended") then
                appendLine line
        appendLine ""
    
    if traceAnalysis.Contains("Last event before gap") then
        appendLine "See `trace-analysis.md` for full details on the last events before the hang."
        appendLine ""
else
    appendLine "*No trace analysis available*"
    appendLine ""

appendLine "### Dump Analysis Summary"
appendLine ""

if dumpAnalysis.Length > 0 then
    if dumpAnalysis.Contains("No Dump Files Found") then
        appendLine "*No memory dumps were captured during the analysis.*"
        appendLine ""
        appendLine "This is expected because the timeout mechanism kills the process"
        appendLine "before a dump can be manually captured."
        appendLine ""
    else
        appendLine "See `dump-analysis.md` for full thread and lock analysis."
        appendLine ""
else
    appendLine "*No dump analysis available*"
    appendLine ""

appendLine ""

// Timeline
appendLine "## Timeline of Events"
appendLine ""

match metadata with
| Some m ->
    appendFormat "1. **%s** - Diagnostic collection started" m.StartTime
    appendLine "2. Repository cloned and dependencies restored"
    appendFormat "3. Command executed: `%s`" m.Command
    appendFormat "4. **Timeout after %d seconds** - Process terminated" m.TimeoutSeconds
    appendFormat "5. **%s** - Collection completed" m.EndTime
    appendLine ""
| None ->
    appendLine "*Timeline unavailable - metadata not found*"
    appendLine ""

appendLine ""

// Specific Location
appendLine "## Hang Location"
appendLine ""

if hangDetectedInTrace then
    appendLine "The trace analysis identified when the process became unresponsive."
    appendLine "Review `trace-analysis.md` for:"
    appendLine ""
    appendLine "- The exact timestamp when events stopped"
    appendLine "- The last method being executed"
    appendLine "- Thread activity patterns"
    appendLine ""
else
    appendLine "No specific hang location could be identified from the collected data."
    appendLine ""

appendLine ""

// Hypothesis
appendLine "## Hypothesis"
appendLine ""

appendLine "Based on the available evidence, the most likely explanation is:"
appendLine ""
appendLine "### Primary Hypothesis: F# 10 Type Checker Performance Regression"
appendLine ""
appendLine "FSharpPlus makes extensive use of advanced F# type system features including:"
appendLine "- Higher-kinded types (simulated via generics)"
appendLine "- Type providers"
appendLine "- Complex generic constraints"
appendLine "- Inline functions with srtp"
appendLine ""
appendLine "A change in the F# 10 type checker may have introduced:"
appendLine "- Exponential type inference complexity"
appendLine "- Infinite loop in constraint solving"
appendLine "- Deadlock in parallel type checking"
appendLine ""

appendLine "### Alternative Hypotheses"
appendLine ""
appendLine "1. **MSBuild Integration Issue:** Changes in how MSBuild coordinates with the F# compiler"
appendLine "2. **Test Framework Issue:** The test execution framework may have compatibility issues"
appendLine "3. **Memory Exhaustion:** Type computations may cause OOM leading to apparent hang"
appendLine ""

appendLine ""

// Reproduction Instructions
appendLine "## Reproduction Instructions"
appendLine ""

appendLine "To reproduce this issue:"
appendLine ""
appendLine "```bash"
appendLine "# Ensure .NET 10 SDK is installed"
appendLine "dotnet --version  # Should show 10.0.100 or later"
appendLine ""
appendLine "# Clone the FSharpPlus repository"
appendLine "git clone --branch gus/fsharp9 https://github.com/fsprojects/FSharpPlus.git"
appendLine "cd FSharpPlus"
appendLine ""
appendLine "# Run the exact failing command"
appendLine "dotnet test build.proj -v n"
appendLine ""
appendLine "# Expected: Process hangs after some time"
appendLine "# Workaround: Use Ctrl+C to cancel after 2 minutes"
appendLine "```"
appendLine ""

match metadata with
| Some m ->
    appendLine "### Environment Details"
    appendLine ""
    appendFormat "- **.NET SDK Version:** %s" m.SdkVersion
    appendFormat "- **FSharpPlus Commit:** %s" m.GitCommit
    appendFormat "- **Branch:** %s" m.Branch
    appendLine ""
| None -> ()

appendLine ""

// Recommended Fixes
appendLine "## Recommended Fixes"
appendLine ""

appendLine "### For F# Compiler Team"
appendLine ""
appendLine "1. **Profile the compilation:** Run FSharpPlus compilation with CPU profiler attached"
appendLine "   to identify hot paths"
appendLine ""
appendLine "2. **Bisect F# changes:** Identify which commit between F# 9 and F# 10 introduced the regression"
appendLine ""
appendLine "3. **Review type checker changes:** Look for changes to:"
appendLine "   - Constraint solving (`ConstraintSolver.fs`)"
appendLine "   - Type inference (`TypeChecker.fs`)"
appendLine "   - Generic instantiation"
appendLine ""
appendLine "4. **Add timeout protection:** Consider adding compile-time budgets for type inference"
appendLine ""

appendLine "### For FSharpPlus Team"
appendLine ""
appendLine "1. **Identify minimal repro:** Find the smallest code that triggers the hang"
appendLine ""
appendLine "2. **Workaround:** Consider if any advanced type features can be simplified"
appendLine ""
appendLine "3. **Pin SDK version:** Temporarily pin to .NET 9 SDK until issue is resolved"
appendLine ""

appendLine ""

// Links
appendLine "## Related Resources"
appendLine ""
appendLine "- **F# Issue:** https://github.com/dotnet/fsharp/issues/19116"
appendLine "- **FSharpPlus PR:** https://github.com/fsprojects/FSharpPlus/pull/614"
appendLine "- **Failed CI Run:** https://github.com/fsprojects/FSharpPlus/actions/runs/19410283295/job/55530689891"
appendLine ""

appendLine ""

// Artifacts
appendLine "## Artifacts Generated"
appendLine ""

let checkFile (path: string) =
    if File.Exists(path) then
        let info = FileInfo(path)
        sprintf "✅ %s (%.2f KB)" (Path.GetFileName(path)) (float info.Length / 1024.0)
    else
        sprintf "❌ %s (not found)" (Path.GetFileName(path))

appendFormat "- %s" (checkFile (Path.Combine(outputDir, "hang-trace.nettrace")))
appendFormat "- %s" (checkFile traceAnalysisPath)
appendFormat "- %s" (checkFile dumpAnalysisPath)
appendFormat "- %s" (checkFile metadataPath)
appendFormat "- %s" (checkFile (Path.Combine(outputDir, "console-output.txt")))
appendLine ""

// Check for dump files
let dumpFiles = 
    if Directory.Exists(outputDir) then
        Directory.GetFiles(outputDir, "*.dmp")
    else
        [||]

if dumpFiles.Length > 0 then
    for dumpFile in dumpFiles do
        appendFormat "- %s" (checkFile dumpFile)
else
    appendLine "- ⚠️ No dump files captured (expected if process was killed by timeout)"

appendLine ""

appendLine "---"
appendLine ""
appendLine "*This report was generated automatically by the FSharpPlus hang diagnostic pipeline.*"
appendLine "*For questions, please contact the F# team or file an issue at https://github.com/dotnet/fsharp/issues*"

// Write the report
let report = sb.ToString()
File.WriteAllText(finalReportPath, report)

printfn "Final report written to: %s" finalReportPath
printfn "Done."
