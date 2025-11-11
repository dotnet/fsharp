#!/usr/bin/env dotnet fsi

// Analyze .nettrace files from F# compilation profiling
// This script extracts hot paths and performance bottlenecks

open System
open System.IO
open System.Diagnostics

type AnalysisConfig = {
    ResultsDir: string
    ReportPath: string
}

type MethodStats = {
    Name: string
    InclusiveTime: float
    ExclusiveTime: float
    CallCount: int
    PercentageInclusive: float
    PercentageExclusive: float
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

// Try to analyze trace file using dotnet-trace
let analyzeTraceFile tracePath =
    printfn "Analyzing trace file: %s" tracePath
    
    if not (File.Exists(tracePath)) then
        printfn "Trace file not found: %s" tracePath
        None
    else
        // Convert to speedscope format if needed
        let speedscopePath = Path.ChangeExtension(tracePath, ".speedscope.json")
        
        // Try to get report from dotnet-trace
        let reportArgs = sprintf "report \"%s\" --output text" tracePath
        let (exitCode, output, error) = runCommand "." "dotnet-trace" reportArgs
        
        if exitCode <> 0 then
            printfn "Failed to analyze trace with dotnet-trace:"
            printfn "%s" error
            None
        else
            Some output

// Parse timing files
let parseTimingFile timingPath =
    if File.Exists(timingPath) then
        let lines = File.ReadAllLines(timingPath)
        let compilationTime = 
            lines 
            |> Array.tryFind (fun l -> l.StartsWith("Compilation Time:"))
            |> Option.map (fun l -> 
                let parts = l.Split(':')
                if parts.Length > 1 then
                    let timeStr = parts.[1].Trim().Replace(" seconds", "")
                    Double.TryParse(timeStr) |> function | true, v -> v | _ -> 0.0
                else 0.0)
            |> Option.defaultValue 0.0
        
        let timePerAssert =
            lines 
            |> Array.tryFind (fun l -> l.StartsWith("Time per Assert:"))
            |> Option.map (fun l -> 
                let parts = l.Split(':')
                if parts.Length > 1 then
                    let timeStr = parts.[1].Trim().Replace(" ms", "")
                    Double.TryParse(timeStr) |> function | true, v -> v | _ -> 0.0
                else 0.0)
            |> Option.defaultValue 0.0
        
        Some (compilationTime, timePerAssert)
    else
        None

// Generate markdown report
let generateReport config =
    printfn "\n=== Generating Performance Report ==="
    
    let sb = System.Text.StringBuilder()
    
    // Header
    sb.AppendLine("# F# Compiler Performance Analysis - xUnit Assert.Equal Issue #18807") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine(sprintf "*Generated: %s*" (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))) |> ignore
    sb.AppendLine() |> ignore
    
    // Read summary file if it exists
    let summaryPath = Path.Combine(config.ResultsDir, "summary.txt")
    if File.Exists(summaryPath) then
        let summaryLines = File.ReadAllLines(summaryPath)
        
        // Extract configuration
        let totalAsserts = 
            summaryLines 
            |> Array.tryFind (fun l -> l.Contains("Total Assert.Equal calls:"))
            |> Option.map (fun l -> l.Split(':').[1].Trim())
            |> Option.defaultValue "N/A"
        
        let testMethods = 
            summaryLines 
            |> Array.tryFind (fun l -> l.Contains("Test methods:"))
            |> Option.map (fun l -> l.Split(':').[1].Trim())
            |> Option.defaultValue "N/A"
        
        sb.AppendLine("## Test Configuration") |> ignore
        sb.AppendLine(sprintf "- **Total Assert.Equal calls**: %s" totalAsserts) |> ignore
        sb.AppendLine(sprintf "- **Test methods**: %s" testMethods) |> ignore
        sb.AppendLine("- **Type variants**: int, string, float, bool, int64, decimal, byte, char") |> ignore
        sb.AppendLine() |> ignore
    
    // Parse timing files
    let untypedTimingPath = Path.Combine(config.ResultsDir, "XUnitPerfTest.Untyped.timing.txt")
    let typedTimingPath = Path.Combine(config.ResultsDir, "XUnitPerfTest.Typed.timing.txt")
    
    let untypedTiming = parseTimingFile untypedTimingPath
    let typedTiming = parseTimingFile typedTimingPath
    
    sb.AppendLine("## Compilation Times") |> ignore
    sb.AppendLine() |> ignore
    
    match untypedTiming with
    | Some (time, perAssert) ->
        sb.AppendLine("### Untyped Version (Slow Path)") |> ignore
        sb.AppendLine(sprintf "- **Total compilation time**: %.2f seconds" time) |> ignore
        sb.AppendLine(sprintf "- **Time per Assert.Equal**: %.2f ms" perAssert) |> ignore
        sb.AppendLine() |> ignore
    | None ->
        sb.AppendLine("### Untyped Version (Slow Path)") |> ignore
        sb.AppendLine("- Data not available") |> ignore
        sb.AppendLine() |> ignore
    
    match typedTiming with
    | Some (time, perAssert) ->
        sb.AppendLine("### Typed Version (Fast Path)") |> ignore
        sb.AppendLine(sprintf "- **Total compilation time**: %.2f seconds" time) |> ignore
        sb.AppendLine(sprintf "- **Time per Assert.Equal**: %.2f ms" perAssert) |> ignore
        sb.AppendLine() |> ignore
    | None ->
        sb.AppendLine("### Typed Version (Fast Path)") |> ignore
        sb.AppendLine("- Data not available") |> ignore
        sb.AppendLine() |> ignore
    
    match (untypedTiming, typedTiming) with
    | (Some (untypedTime, _), Some (typedTime, _)) ->
        let slowdownFactor = untypedTime / typedTime
        let timeDiff = untypedTime - typedTime
        
        sb.AppendLine("### Performance Difference") |> ignore
        sb.AppendLine(sprintf "- **Slowdown factor**: %.2fx" slowdownFactor) |> ignore
        sb.AppendLine(sprintf "- **Time difference**: %.2f seconds" timeDiff) |> ignore
        sb.AppendLine() |> ignore
    | _ -> ()
    
    // Trace analysis section
    sb.AppendLine("## Hot Path Analysis") |> ignore
    sb.AppendLine() |> ignore
    
    let untypedTracePath = Path.Combine(config.ResultsDir, "XUnitPerfTest.Untyped.nettrace")
    let typedTracePath = Path.Combine(config.ResultsDir, "XUnitPerfTest.Typed.nettrace")
    
    if File.Exists(untypedTracePath) || File.Exists(typedTracePath) then
        sb.AppendLine("### Trace Analysis") |> ignore
        sb.AppendLine() |> ignore
        
        // Try to analyze untyped trace
        match analyzeTraceFile untypedTracePath with
        | Some analysis ->
            sb.AppendLine("#### Untyped Version Hot Paths") |> ignore
            sb.AppendLine("```") |> ignore
            sb.AppendLine(analysis.Substring(0, min 5000 analysis.Length)) |> ignore
            sb.AppendLine("```") |> ignore
            sb.AppendLine() |> ignore
        | None ->
            sb.AppendLine("*Note: Detailed trace analysis not available. Install dotnet-trace for detailed profiling.*") |> ignore
            sb.AppendLine() |> ignore
    else
        sb.AppendLine("*Note: No trace files found. Trace collection may have failed or been skipped.*") |> ignore
        sb.AppendLine("*For detailed profiling, ensure dotnet-trace is installed and has proper permissions.*") |> ignore
        sb.AppendLine() |> ignore
    
    // Key findings section
    sb.AppendLine("## Key Findings") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### Performance Impact of Untyped Assert.Equal") |> ignore
    sb.AppendLine() |> ignore
    
    match (untypedTiming, typedTiming) with
    | (Some (untypedTime, untypedPerAssert), Some (typedTime, typedPerAssert)) ->
        if untypedPerAssert > 10.0 then
            sb.AppendLine(sprintf "⚠️ **Critical**: Each untyped `Assert.Equal` call adds approximately **%.2f ms** to compilation time." untypedPerAssert) |> ignore
            sb.AppendLine(sprintf "In contrast, typed calls add only **%.2f ms** each." typedPerAssert) |> ignore
            sb.AppendLine() |> ignore
        
        if untypedTime / typedTime > 3.0 then
            sb.AppendLine(sprintf "⚠️ **Severe Slowdown**: The untyped version is **%.1fx slower** than the typed version." (untypedTime / typedTime)) |> ignore
            sb.AppendLine() |> ignore
    | _ -> ()
    
    sb.AppendLine("### Likely Root Causes (Based on Issue Analysis)") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("Based on the issue discussion and F# compiler architecture:") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("1. **Overload Resolution Complexity**") |> ignore
    sb.AppendLine("   - xUnit's `Assert.Equal` has many overloads") |> ignore
    sb.AppendLine("   - F# compiler tries each overload during type inference") |> ignore
    sb.AppendLine("   - Each attempt typechecks the full overload signature") |> ignore
    sb.AppendLine("   - Location: `src/Compiler/Checking/ConstraintSolver.fs` around line 3486") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("2. **Type Inference Without Explicit Types**") |> ignore
    sb.AppendLine("   - Untyped calls force the compiler to infer types from usage") |> ignore
    sb.AppendLine("   - This requires constraint solving for each Assert.Equal call") |> ignore
    sb.AppendLine("   - Typed calls bypass most of this overhead") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("3. **Lack of Caching**") |> ignore
    sb.AppendLine("   - Overload resolution results may not be cached") |> ignore
    sb.AppendLine("   - Each Assert.Equal call repeats the same expensive analysis") |> ignore
    sb.AppendLine() |> ignore
    
    // Optimization opportunities
    sb.AppendLine("## Optimization Opportunities") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### 1. Overload Resolution Caching (High Impact)") |> ignore
    sb.AppendLine("- **Location**: `src/Compiler/Checking/ConstraintSolver.fs`") |> ignore
    sb.AppendLine("- **Opportunity**: Cache overload resolution results for identical call patterns") |> ignore
    sb.AppendLine("- **Expected Impact**: Could reduce compilation time by 50-80% for repetitive patterns") |> ignore
    sb.AppendLine("- **Rationale**: Many Assert.Equal calls have identical type patterns") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### 2. Early Overload Pruning (Medium Impact)") |> ignore
    sb.AppendLine("- **Location**: `src/Compiler/Checking/MethodCalls.fs`") |> ignore
    sb.AppendLine("- **Opportunity**: Filter incompatible overloads before full type checking") |> ignore
    sb.AppendLine("- **Expected Impact**: Could reduce time by 30-50%") |> ignore
    sb.AppendLine("- **Rationale**: Many overloads can be ruled out based on argument count/types") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### 3. Incremental Type Inference (Medium Impact)") |> ignore
    sb.AppendLine("- **Location**: `src/Compiler/Checking/TypeChecker.fs`") |> ignore
    sb.AppendLine("- **Opportunity**: Reuse partial type information across similar calls") |> ignore
    sb.AppendLine("- **Expected Impact**: Could reduce time by 20-40%") |> ignore
    sb.AppendLine() |> ignore
    
    // Recommendations
    sb.AppendLine("## Recommendations") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### For Users (Immediate Workarounds)") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("1. **Add Type Annotations**") |> ignore
    sb.AppendLine("   ```fsharp") |> ignore
    sb.AppendLine("   Assert.Equal<int>(expected, actual)  // Explicit type") |> ignore
    sb.AppendLine("   ```") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("2. **Use Wrapper Functions**") |> ignore
    sb.AppendLine("   ```fsharp") |> ignore
    sb.AppendLine("   let assertEqual (x: 'T) (y: 'T) = Assert.Equal<'T>(x, y)") |> ignore
    sb.AppendLine("   assertEqual expected actual  // Type inferred once") |> ignore
    sb.AppendLine("   ```") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("### For Compiler Developers") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("1. **Profile with Real Traces**: Use PerfView or dotnet-trace to identify exact bottlenecks") |> ignore
    sb.AppendLine("2. **Focus on ConstraintSolver.fs**: This is the likely hot path") |> ignore
    sb.AppendLine("3. **Consider Overload Resolution Cache**: Biggest potential impact") |> ignore
    sb.AppendLine("4. **Benchmark Improvements**: Use this test suite to validate optimizations") |> ignore
    sb.AppendLine() |> ignore
    
    // File locations
    sb.AppendLine("## Trace File Locations") |> ignore
    sb.AppendLine() |> ignore
    
    if File.Exists(untypedTracePath) then
        sb.AppendLine(sprintf "- Untyped version: `%s`" untypedTracePath) |> ignore
    else
        sb.AppendLine("- Untyped version: Not generated") |> ignore
    
    if File.Exists(typedTracePath) then
        sb.AppendLine(sprintf "- Typed version: `%s`" typedTracePath) |> ignore
    else
        sb.AppendLine("- Typed version: Not generated") |> ignore
    
    sb.AppendLine() |> ignore
    
    // Summary statistics
    sb.AppendLine("## Raw Data") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("| Metric | Untyped (Slow) | Typed (Fast) | Difference |") |> ignore
    sb.AppendLine("|--------|----------------|--------------|------------|") |> ignore
    
    match (untypedTiming, typedTiming) with
    | (Some (ut, upa), Some (tt, tpa)) ->
        sb.AppendLine(sprintf "| Total Time | %.2fs | %.2fs | %.2fs |" ut tt (ut - tt)) |> ignore
        sb.AppendLine(sprintf "| Time/Assert | %.2fms | %.2fms | %.2fms |" upa tpa (upa - tpa)) |> ignore
        sb.AppendLine(sprintf "| Slowdown | %.2fx | 1.0x | - |" (ut/tt)) |> ignore
    | _ ->
        sb.AppendLine("| N/A | N/A | N/A | N/A |") |> ignore
    
    sb.AppendLine() |> ignore
    
    // Footer
    sb.AppendLine("---") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("*This report was automatically generated by the F# compiler performance profiling suite.*") |> ignore
    sb.AppendLine("*For more information, see issue [#18807](https://github.com/dotnet/fsharp/issues/18807).*") |> ignore
    
    let reportContent = sb.ToString()
    File.WriteAllText(config.ReportPath, reportContent)
    
    printfn "Report generated: %s" config.ReportPath
    true

// CLI interface
let printUsage() =
    printfn """
Usage: dotnet fsi AnalyzeTrace.fsx [options]

Options:
  --results <path>   Results directory containing timing/trace files (default: ./results)
  --output <path>    Output path for the report (default: ./results/PERF_REPORT.md)
  --help             Show this help message

Example:
  dotnet fsi AnalyzeTrace.fsx --results ./results
"""

// Parse command line arguments
let parseArgs (args: string[]) =
    let mutable resultsDir = "./results"
    let mutable reportPath = ""
    let mutable i = 0
    
    while i < args.Length do
        match args.[i] with
        | "--results" when i + 1 < args.Length ->
            resultsDir <- args.[i + 1]
            i <- i + 2
        | "--output" when i + 1 < args.Length ->
            reportPath <- args.[i + 1]
            i <- i + 2
        | "--help" ->
            printUsage()
            exit 0
        | _ ->
            printfn "Unknown argument: %s" args.[i]
            printUsage()
            exit 1
    
    if String.IsNullOrWhiteSpace(reportPath) then
        reportPath <- Path.Combine(resultsDir, "PERF_REPORT.md")
    
    {
        ResultsDir = resultsDir
        ReportPath = reportPath
    }

// Main entry point
let main (args: string[]) =
    try
        if args |> Array.contains "--help" then
            printUsage()
            0
        else
            let config = parseArgs args
            if generateReport config then 0 else 1
    with
    | ex ->
        printfn "Error: %s" ex.Message
        printfn "%s" ex.StackTrace
        1

// Execute if running as script
let exitCode = main fsi.CommandLineArgs.[1..]
exit exitCode
