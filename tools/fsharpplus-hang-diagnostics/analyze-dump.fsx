#!/usr/bin/env -S dotnet fsi --langversion:preview
// analyze-dump.fsx
// Analyzes memory dump files to find hang points in FSharpPlus build
// Uses Microsoft.Diagnostics.Runtime (ClrMD) library

#r "nuget: Microsoft.Diagnostics.Runtime"

open System
open System.IO
open System.Text
open System.Collections.Generic
open Microsoft.Diagnostics.Runtime

// Configuration
let scriptDir = __SOURCE_DIRECTORY__
let outputDir = Path.Combine(scriptDir, "output")

// Find dump files
let findDumpFiles () =
    if Directory.Exists(outputDir) then
        Directory.GetFiles(outputDir, "*.dmp")
    else
        [||]

// Data structures
type ThreadInfo = {
    ThreadId: uint32
    ManagedThreadId: int
    IsAlive: bool
    StackFrames: string list
    TopFrame: string
    ThreadState: string
}

type StackFrameGroup = {
    TopFrame: string
    ThreadCount: int
    ThreadIds: uint32 list
    FullStack: string list
}

type HeapStats = {
    TotalObjects: int64
    TotalSize: int64
    Gen0Size: int64
    Gen1Size: int64
    Gen2Size: int64
    LargeObjectHeapSize: int64
}

// Helper to get method signature from frame
let getMethodSignature (frame: ClrStackFrame) =
    try
        if frame.Method <> null then
            let method = frame.Method
            let typeName = if method.Type <> null then method.Type.Name else "?"
            sprintf "%s.%s" typeName method.Name
        else
            "[Native Frame]"
    with _ ->
        "[Unknown Frame]"

// Check if a frame looks like it's waiting on synchronization
let isWaitingFrame (frame: string) =
    frame.Contains("Monitor.Wait") ||
    frame.Contains("Monitor.Enter") ||
    frame.Contains("WaitHandle") ||
    frame.Contains("Thread.Sleep") ||
    frame.Contains("Thread.Join") ||
    frame.Contains("SemaphoreSlim") ||
    frame.Contains("ManualResetEvent") ||
    frame.Contains("AutoResetEvent") ||
    frame.Contains("Task.Wait") ||
    frame.Contains("TaskAwaiter")

// Analyze a single dump file
let analyzeDump (dumpPath: string) =
    printfn "Analyzing dump: %s" dumpPath
    
    try
        use dataTarget = DataTarget.LoadDump(dumpPath)
        
        if dataTarget.ClrVersions.Length = 0 then
            printfn "ERROR: No CLR versions found in dump"
            None
        else
            let clrVersion = dataTarget.ClrVersions.[0]
            printfn "CLR Version: %s" (clrVersion.Version.ToString())
            
            use runtime = clrVersion.CreateRuntime()
            
            // Collect thread information
            let threads = ResizeArray<ThreadInfo>()
            
            for thread in runtime.Threads do
                let frames = 
                    thread.EnumerateStackTrace()
                    |> Seq.map getMethodSignature
                    |> Seq.toList
                
                let topFrame = 
                    if frames.Length > 0 then frames.[0]
                    else "[No Stack]"
                
                threads.Add({
                    ThreadId = thread.OSThreadId
                    ManagedThreadId = thread.ManagedThreadId
                    IsAlive = thread.IsAlive
                    StackFrames = frames
                    TopFrame = topFrame
                    ThreadState = thread.State.ToString()
                })
            
            // Group threads by top frame
            let stackGroups =
                threads
                |> Seq.filter (fun t -> t.StackFrames.Length > 0)
                |> Seq.groupBy (fun t -> 
                    // Use top few frames for grouping
                    t.StackFrames |> List.truncate 3 |> String.concat " -> ")
                |> Seq.map (fun (key, group) ->
                    let groupList = group |> Seq.toList
                    let first = groupList.[0]
                    {
                        TopFrame = key
                        ThreadCount = groupList.Length
                        ThreadIds = groupList |> List.map (fun t -> t.ThreadId)
                        FullStack = first.StackFrames
                    })
                |> Seq.sortByDescending (fun g -> g.ThreadCount)
                |> Seq.toList
            
            // Get heap statistics
            let heapStats =
                try
                    let heap = runtime.Heap
                    let segments = heap.Segments |> Seq.toList
                    
                    let gen0Size = segments |> List.sumBy (fun s -> int64 s.Generation0.Length)
                    let gen1Size = segments |> List.sumBy (fun s -> int64 s.Generation1.Length)
                    let gen2Size = segments |> List.sumBy (fun s -> int64 s.Generation2.Length)
                    let lohSize = 
                        segments 
                        |> List.filter (fun s -> s.Kind = GCSegmentKind.Large) 
                        |> List.sumBy (fun s -> int64 s.Length)
                    
                    let totalSize = gen0Size + gen1Size + gen2Size + lohSize
                    
                    Some {
                        TotalObjects = 0L // Would need to enumerate heap
                        TotalSize = totalSize
                        Gen0Size = gen0Size
                        Gen1Size = gen1Size
                        Gen2Size = gen2Size
                        LargeObjectHeapSize = lohSize
                    }
                with ex ->
                    printfn "Warning: Could not get heap stats: %s" ex.Message
                    None
            
            Some (threads |> Seq.toList, stackGroups, heapStats)
    with ex ->
        printfn "ERROR analyzing dump: %s" ex.Message
        printfn "Stack trace: %s" ex.StackTrace
        None

// Check if frame is F# compiler related
let isFSharpCompilerFrame (frame: string) =
    frame.Contains("FSharp.Compiler", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Microsoft.FSharp", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("TypeChecker", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("TcImports", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Optimizer", StringComparison.OrdinalIgnoreCase)

// Check if frame is MSBuild related
let isMSBuildFrame (frame: string) =
    frame.Contains("Microsoft.Build", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("MSBuild", StringComparison.OrdinalIgnoreCase)

// Check if frame is synchronization related
let isSyncFrame (frame: string) =
    frame.Contains("Monitor.Wait", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Monitor.Enter", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Thread.Join", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("WaitHandle.Wait", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("ManualResetEvent", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("AutoResetEvent", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Semaphore", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("Task.Wait", StringComparison.OrdinalIgnoreCase) ||
    frame.Contains("TaskAwaiter", StringComparison.OrdinalIgnoreCase)

// Generate markdown report
let generateReport (dumpFiles: string[]) (analysisResults: (string * (ThreadInfo list * StackFrameGroup list * HeapStats option) option) list) =
    let sb = StringBuilder()
    
    let appendLine (s: string) = sb.AppendLine(s) |> ignore
    let appendFormat fmt = Printf.kprintf appendLine fmt
    
    appendLine "# Dump Analysis Report"
    appendLine ""
    appendFormat "**Generated:** %s" (DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"))
    appendFormat "**Dump Files Analyzed:** %d" dumpFiles.Length
    appendLine ""
    
    // Executive Summary
    appendLine "## Executive Summary"
    appendLine ""
    
    if dumpFiles.Length = 0 then
        appendLine "### ⚠️ No Dump Files Found"
        appendLine ""
        appendLine "No memory dump files were found in the output directory."
        appendLine "This is **expected** if the process was killed by timeout before a dump could be captured."
        appendLine ""
        appendLine "To capture a dump, the diagnostic process would need to:"
        appendLine "1. Detect the hang while the process is still running"
        appendLine "2. Capture the dump before killing the process"
        appendLine ""
        appendLine "Consider running the collection manually with a longer timeout or manual dump capture."
    else
        let successfulAnalyses = 
            analysisResults 
            |> List.choose (fun (name, result) -> 
                match result with 
                | Some (threads, groups, stats) -> Some (name, threads, groups, stats)
                | None -> None)
        
        if successfulAnalyses.Length = 0 then
            appendLine "### ❌ All Analyses Failed"
            appendLine ""
            appendLine "Could not analyze any of the dump files. They may be corrupted or in an unsupported format."
        else
            appendFormat "### ✅ Analyzed %d Dump File(s)" successfulAnalyses.Length
            appendLine ""
            
            for (name, threads, groups, stats) in successfulAnalyses do
                appendFormat "#### %s" name
                appendFormat "- **Total Threads:** %d" threads.Length
                appendFormat "- **Alive Threads:** %d" (threads |> List.filter (fun t -> t.IsAlive) |> List.length)
                
                // Count threads that appear to be waiting
                let waitingCount = 
                    threads 
                    |> List.filter (fun t -> t.StackFrames |> List.exists isWaitingFrame)
                    |> List.length
                appendFormat "- **Threads in Wait State:** %d" waitingCount
                appendLine ""
                
                // Find most common hang point
                if groups.Length > 0 then
                    let topGroup = groups.[0]
                    if topGroup.ThreadCount > 1 then
                        appendFormat "**Most Common Stack Pattern:** %d threads share the same top frames" topGroup.ThreadCount
                        appendFormat "- Top frames: `%s`" topGroup.TopFrame
                        appendLine ""
                
                match stats with
                | Some s ->
                    appendFormat "**Heap Size:** %.2f MB" (float s.TotalSize / 1024.0 / 1024.0)
                | None -> ()
                appendLine ""
    
    appendLine ""
    
    // Detailed analysis for each dump
    for (name, result) in analysisResults do
        appendFormat "## Dump: %s" name
        appendLine ""
        
        match result with
        | None ->
            appendLine "❌ **Analysis Failed** - Could not open or parse the dump file."
            appendLine ""
        | Some (threads, groups, stats) ->
            // Most Common Hang Point
            appendLine "### Most Common Hang Points"
            appendLine ""
            
            if groups.Length > 0 then
                appendLine "Stack patterns where multiple threads are stuck (potential hang points):"
                appendLine ""
                
                let hangPoints = groups |> List.filter (fun g -> g.ThreadCount > 1) |> List.truncate 5
                if hangPoints.Length > 0 then
                    for group in hangPoints do
                        appendFormat "#### %d Threads at Same Location" group.ThreadCount
                        appendFormat "**Thread IDs:** %s" (group.ThreadIds |> List.map string |> String.concat ", ")
                        appendLine ""
                        appendLine "**Stack Trace (top 15 frames):**"
                        appendLine "```"
                        for frame in group.FullStack |> List.truncate 15 do
                            appendFormat "  %s" frame
                        appendLine "```"
                        appendLine ""
                else
                    appendLine "*No multiple threads found at the same stack location (no clear hang point)*"
            else
                appendLine "*No stack groups found*"
            
            appendLine ""
            
            // F# Compiler Thread Analysis
            appendLine "### F# Compiler Thread Analysis"
            appendLine ""
            
            let fsharpThreads = 
                threads 
                |> List.filter (fun t -> t.StackFrames |> List.exists isFSharpCompilerFrame)
            
            if fsharpThreads.Length > 0 then
                appendFormat "Found **%d threads** with F# compiler frames:" fsharpThreads.Length
                appendLine ""
                
                for thread in fsharpThreads |> List.truncate 10 do
                    appendFormat "#### Thread %d (Managed: %d)" thread.ThreadId thread.ManagedThreadId
                    appendLine ""
                    
                    // Check if thread appears to be waiting
                    let waitingFrame = thread.StackFrames |> List.tryFind isWaitingFrame
                    match waitingFrame with
                    | Some frame -> appendFormat "⚠️ **Waiting at:** `%s`" frame
                    | None -> ()
                    appendLine ""
                    
                    appendLine "**Stack Trace (F# related frames highlighted):**"
                    appendLine "```"
                    for frame in thread.StackFrames |> List.truncate 20 do
                        let marker = 
                            if isFSharpCompilerFrame frame then ">> "
                            elif isSyncFrame frame then "!! "
                            else "   "
                        appendFormat "%s%s" marker frame
                    appendLine "```"
                    appendLine ""
            else
                appendLine "*No threads with F# compiler frames found*"
            
            appendLine ""
            
            // MSBuild Thread Analysis
            appendLine "### MSBuild Thread Analysis"
            appendLine ""
            
            let msbuildThreads = 
                threads 
                |> List.filter (fun t -> t.StackFrames |> List.exists isMSBuildFrame)
            
            if msbuildThreads.Length > 0 then
                appendFormat "Found **%d threads** with MSBuild frames:" msbuildThreads.Length
                appendLine ""
                
                for thread in msbuildThreads |> List.truncate 5 do
                    appendFormat "#### Thread %d (Managed: %d)" thread.ThreadId thread.ManagedThreadId
                    appendLine ""
                    
                    // Check if thread appears to be waiting
                    let waitingFrame = thread.StackFrames |> List.tryFind isWaitingFrame
                    match waitingFrame with
                    | Some frame -> appendFormat "⚠️ **Waiting at:** `%s`" frame
                    | None -> ()
                    appendLine ""
                    
                    appendLine "**Stack Trace (MSBuild related frames highlighted):**"
                    appendLine "```"
                    for frame in thread.StackFrames |> List.truncate 20 do
                        let marker = 
                            if isMSBuildFrame frame then ">> "
                            elif isSyncFrame frame then "!! "
                            else "   "
                        appendFormat "%s%s" marker frame
                    appendLine "```"
                    appendLine ""
            else
                appendLine "*No threads with MSBuild frames found*"
            
            appendLine ""
            
            // Lock and Synchronization State
            appendLine "### Lock and Synchronization State"
            appendLine ""
            
            // Threads waiting on sync primitives
            let syncThreads = 
                threads 
                |> List.filter (fun t -> t.StackFrames |> List.exists isWaitingFrame)
            
            if syncThreads.Length > 0 then
                appendFormat "Found **%d threads** in wait state:" syncThreads.Length
                appendLine ""
                appendLine "| Thread ID | Wait Frame |"
                appendLine "|-----------|------------|"
                for thread in syncThreads |> List.truncate 20 do
                    let syncFrame = thread.StackFrames |> List.find isWaitingFrame
                    appendFormat "| %d | `%s` |" thread.ThreadId syncFrame
                appendLine ""
            else
                appendLine "*No threads found in wait state*"
            
            appendLine ""
            
            // Heap Statistics
            appendLine "### Heap Statistics"
            appendLine ""
            
            match stats with
            | Some s ->
                appendFormat "| Metric | Size |"
                appendLine "|--------|------|"
                appendFormat "| Total Size | %.2f MB |" (float s.TotalSize / 1024.0 / 1024.0)
                appendFormat "| Gen 0 | %.2f MB |" (float s.Gen0Size / 1024.0 / 1024.0)
                appendFormat "| Gen 1 | %.2f MB |" (float s.Gen1Size / 1024.0 / 1024.0)
                appendFormat "| Gen 2 | %.2f MB |" (float s.Gen2Size / 1024.0 / 1024.0)
                appendFormat "| Large Object Heap | %.2f MB |" (float s.LargeObjectHeapSize / 1024.0 / 1024.0)
            | None ->
                appendLine "*Heap statistics not available*"
            
            appendLine ""
            
            // All Thread Summary
            appendLine "### All Threads Summary"
            appendLine ""
            appendLine "| Thread ID | Managed ID | Alive | State | Top Frame |"
            appendLine "|-----------|------------|-------|-------|-----------|"
            
            for thread in threads |> List.truncate 50 do
                let topFrameShort = 
                    if thread.TopFrame.Length > 50 then
                        thread.TopFrame.Substring(0, 47) + "..."
                    else
                        thread.TopFrame
                appendFormat "| %d | %d | %s | %s | `%s` |" 
                    thread.ThreadId 
                    thread.ManagedThreadId
                    (if thread.IsAlive then "✅" else "❌")
                    thread.ThreadState
                    topFrameShort
            
            appendLine ""
    
    // Recommendations
    appendLine "## Recommendations"
    appendLine ""
    
    if dumpFiles.Length = 0 then
        appendLine "Since no dump files were captured:"
        appendLine ""
        appendLine "1. **Run with manual dump capture:** Start the build, wait for hang, then manually run `dotnet-dump collect -p <PID>`"
        appendLine "2. **Use trace analysis:** The `trace-analysis.md` report may provide useful insights"
        appendLine "3. **Check for deadlocks:** If multiple threads are stuck, look for circular wait conditions"
        appendLine "4. **Review F# compiler issues:** Check the F# repository for similar performance issues"
    else
        appendLine "Based on the dump analysis:"
        appendLine ""
        appendLine "1. **Review threads at common hang points:** Multiple threads at the same stack location indicate contention"
        appendLine "2. **Check F# compiler threads:** Look for TypeChecker, TcImports, or Optimizer frames"
        appendLine "3. **Analyze lock waiters:** Identify which locks are causing contention"
        appendLine "4. **Cross-reference with trace:** Use `trace-analysis.md` to understand the timeline"
        appendLine "5. **Look for async/await issues:** Task.Wait or TaskAwaiter frames may indicate deadlocks"
    
    appendLine ""
    appendLine "---"
    appendLine "*Report generated by analyze-dump.fsx*"
    
    sb.ToString()

// Main execution
printfn "Starting dump analysis..."
printfn "Output directory: %s" outputDir

let dumpFiles = findDumpFiles()
printfn "Found %d dump file(s)" dumpFiles.Length

let analysisResults =
    dumpFiles
    |> Array.map (fun path ->
        let name = Path.GetFileName(path)
        let result = analyzeDump path
        (name, result))
    |> Array.toList

printfn "Generating report..."
let report = generateReport dumpFiles analysisResults

let reportPath = Path.Combine(outputDir, "dump-analysis.md")
File.WriteAllText(reportPath, report)
printfn "Report written to: %s" reportPath

printfn "Done."
