#!/usr/bin/env -S dotnet fsi --langversion:preview
// analyze-trace.fsx
// Analyzes .nettrace files to find hang points in FSharpPlus build
// Uses Microsoft.Diagnostics.Tracing.TraceEvent library

#r "nuget: Microsoft.Diagnostics.Tracing.TraceEvent, 3.1.8"

open System
open System.IO
open System.Text
open System.Collections.Generic
open Microsoft.Diagnostics.Tracing
open Microsoft.Diagnostics.Tracing.Etlx
open Microsoft.Diagnostics.Tracing.Parsers.Clr

// Configuration
let scriptDir = __SOURCE_DIRECTORY__
let outputDir = Path.Combine(scriptDir, "output")
let traceFile = 
    let defaultPath = Path.Combine(outputDir, "hang-trace.nettrace")
    if fsi.CommandLineArgs.Length > 1 then fsi.CommandLineArgs.[1] else defaultPath

printfn "Analyzing trace file: %s" traceFile

// Data structures for analysis
type EventInfo = {
    TimeStamp: DateTime
    ProcessId: int
    ThreadId: int
    ProviderName: string
    EventName: string
    PayloadSummary: string
}

type MethodInfo = {
    MethodName: string
    Namespace: string
    Count: int
    FirstSeen: DateTime
    LastSeen: DateTime
}

type ThreadActivity = {
    ThreadId: int
    EventCount: int
    FirstEvent: DateTime
    LastEvent: DateTime
    Providers: Set<string>
}

type TimeGap = {
    StartTime: DateTime
    EndTime: DateTime
    GapSeconds: float
    LastEventBefore: EventInfo option
    FirstEventAfter: EventInfo option
}

// Analysis state
let allEvents = ResizeArray<EventInfo>()
let methodJitCounts = Dictionary<string, int>()
let gcEvents = ResizeArray<EventInfo>()
let lockContentionEvents = ResizeArray<EventInfo>()
let threadActivity = Dictionary<int, ResizeArray<EventInfo>>()
let providerCounts = Dictionary<string, int>()
let fsharpCompilerEvents = ResizeArray<EventInfo>()
let msbuildEvents = ResizeArray<EventInfo>()

// Helper to increment dictionary count
let incrCount (dict: Dictionary<'K, int>) key =
    if dict.ContainsKey(key) then
        dict.[key] <- dict.[key] + 1
    else
        dict.[key] <- 1

// Create event info from trace event
let createEventInfo (data: TraceEvent) (payloadSummary: string) =
    {
        TimeStamp = data.TimeStamp
        ProcessId = data.ProcessID
        ThreadId = data.ThreadID
        ProviderName = data.ProviderName
        EventName = data.EventName
        PayloadSummary = payloadSummary
    }

// Check if event is F# compiler related
let isFSharpCompilerEvent (providerName: string) (eventName: string) (payload: string) =
    providerName.Contains("FSharp", StringComparison.OrdinalIgnoreCase) ||
    eventName.Contains("FSharp", StringComparison.OrdinalIgnoreCase) ||
    payload.Contains("FSharp.Compiler", StringComparison.OrdinalIgnoreCase) ||
    payload.Contains("TypeChecker", StringComparison.OrdinalIgnoreCase) ||
    payload.Contains("TcImports", StringComparison.OrdinalIgnoreCase) ||
    payload.Contains("Optimizer", StringComparison.OrdinalIgnoreCase)

// Check if event is MSBuild related
let isMSBuildEvent (providerName: string) (eventName: string) (payload: string) =
    providerName.Contains("Microsoft-Build", StringComparison.OrdinalIgnoreCase) ||
    providerName.Contains("MSBuild", StringComparison.OrdinalIgnoreCase) ||
    eventName.Contains("Build", StringComparison.OrdinalIgnoreCase) ||
    payload.Contains("Microsoft.Build", StringComparison.OrdinalIgnoreCase)

// Process events
let processEvents () =
    if not (File.Exists(traceFile)) then
        printfn "ERROR: Trace file not found: %s" traceFile
        false
    else
        printfn "Opening trace file..."
        try
            // Convert nettrace to etlx for analysis
            let etlxPath = Path.ChangeExtension(traceFile, ".etlx")
            let traceLog = TraceLog.OpenOrConvert(traceFile, etlxPath)
            let source = traceLog.Events.GetSource()
            
            printfn "Processing events..."
            
            // Subscribe to CLR events
            source.Clr.add_MethodJittingStarted(fun data ->
                let methodName = data.MethodName
                let ns = data.MethodNamespace
                let fullName = sprintf "%s.%s" ns methodName
                incrCount methodJitCounts fullName
                let info = createEventInfo data fullName
                allEvents.Add(info)
                incrCount providerCounts data.ProviderName
                
                if isFSharpCompilerEvent data.ProviderName data.EventName fullName then
                    fsharpCompilerEvents.Add(info)
                    
                // Track thread activity
                if not (threadActivity.ContainsKey(data.ThreadID)) then
                    threadActivity.[data.ThreadID] <- ResizeArray<EventInfo>()
                threadActivity.[data.ThreadID].Add(info)
            )
            
            source.Clr.add_GCStart(fun data ->
                let info = createEventInfo data (sprintf "GC Gen%d" data.Depth)
                gcEvents.Add(info)
                allEvents.Add(info)
                incrCount providerCounts data.ProviderName
            )
            
            source.Clr.add_ContentionStart(fun data ->
                let info = createEventInfo data "Contention"
                lockContentionEvents.Add(info)
                allEvents.Add(info)
                incrCount providerCounts data.ProviderName
            )
            
            // Subscribe to all dynamic events
            source.Dynamic.add_All(fun data ->
                let payload = 
                    try
                        let names = data.PayloadNames
                        if names <> null && names.Length > 0 then
                            names 
                            |> Array.truncate 3
                            |> Array.map (fun n -> sprintf "%s=%O" n (data.PayloadByName(n)))
                            |> String.concat ", "
                        else
                            ""
                    with _ -> ""
                    
                let info = createEventInfo data payload
                allEvents.Add(info)
                incrCount providerCounts data.ProviderName
                
                if isFSharpCompilerEvent data.ProviderName data.EventName payload then
                    fsharpCompilerEvents.Add(info)
                    
                if isMSBuildEvent data.ProviderName data.EventName payload then
                    msbuildEvents.Add(info)
                    
                // Track thread activity
                if not (threadActivity.ContainsKey(data.ThreadID)) then
                    threadActivity.[data.ThreadID] <- ResizeArray<EventInfo>()
                threadActivity.[data.ThreadID].Add(info)
            )
            
            source.Process() |> ignore
            printfn "Processed %d events" allEvents.Count
            true
        with ex ->
            printfn "ERROR processing trace: %s" ex.Message
            printfn "Stack trace: %s" ex.StackTrace
            false

// Find time gaps in events
let findTimeGaps (minGapSeconds: float) =
    let sorted = allEvents |> Seq.sortBy (fun e -> e.TimeStamp) |> Seq.toArray
    let gaps = ResizeArray<TimeGap>()
    
    for i in 1 .. sorted.Length - 1 do
        let prev = sorted.[i-1]
        let curr = sorted.[i]
        let gapSeconds = (curr.TimeStamp - prev.TimeStamp).TotalSeconds
        
        if gapSeconds >= minGapSeconds then
            gaps.Add({
                StartTime = prev.TimeStamp
                EndTime = curr.TimeStamp
                GapSeconds = gapSeconds
                LastEventBefore = Some prev
                FirstEventAfter = Some curr
            })
    
    gaps |> Seq.toList

// Get thread activity summary
let getThreadActivitySummary () =
    threadActivity
    |> Seq.map (fun kvp ->
        let events = kvp.Value |> Seq.toArray
        if events.Length > 0 then
            let providers = events |> Seq.map (fun e -> e.ProviderName) |> Set.ofSeq
            Some {
                ThreadId = kvp.Key
                EventCount = events.Length
                FirstEvent = events |> Seq.map (fun e -> e.TimeStamp) |> Seq.min
                LastEvent = events |> Seq.map (fun e -> e.TimeStamp) |> Seq.max
                Providers = providers
            }
        else
            None
    )
    |> Seq.choose id
    |> Seq.sortByDescending (fun t -> t.EventCount)
    |> Seq.toList

// Get hot methods (most JIT'd)
let getHotMethods (count: int) =
    methodJitCounts
    |> Seq.sortByDescending (fun kvp -> kvp.Value)
    |> Seq.truncate count
    |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
    |> Seq.toList

// Generate the markdown report
let generateReport () =
    let sb = StringBuilder()
    
    let appendLine (s: string) = sb.AppendLine(s) |> ignore
    let appendFormat fmt = Printf.kprintf appendLine fmt
    
    appendLine "# Trace Analysis Report"
    appendLine ""
    appendFormat "**Generated:** %s" (DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"))
    appendFormat "**Trace File:** %s" (Path.GetFileName(traceFile))
    appendFormat "**Total Events:** %d" allEvents.Count
    appendLine ""
    
    // Executive Summary
    appendLine "## Executive Summary"
    appendLine ""
    
    let timeGaps = findTimeGaps 1.0 // Gaps > 1 second
    if timeGaps.Length > 0 then
        appendLine "### ⚠️ Hang Detected"
        appendLine ""
        appendFormat "Found **%d significant time gap(s)** (> 1 second) in event stream." timeGaps.Length
        appendLine ""
        
        let largestGap = timeGaps |> List.maxBy (fun g -> g.GapSeconds)
        appendFormat "**Largest gap:** %.2f seconds" largestGap.GapSeconds
        appendFormat "- Gap started at: %s" (largestGap.StartTime.ToString("HH:mm:ss.fff"))
        appendFormat "- Gap ended at: %s" (largestGap.EndTime.ToString("HH:mm:ss.fff"))
        appendLine ""
        
        match largestGap.LastEventBefore with
        | Some evt ->
            appendLine "**Last event before gap:**"
            appendFormat "- Provider: `%s`" evt.ProviderName
            appendFormat "- Event: `%s`" evt.EventName
            appendFormat "- Thread: %d" evt.ThreadId
            appendFormat "- Payload: `%s`" evt.PayloadSummary
        | None -> ()
        appendLine ""
        
        match largestGap.FirstEventAfter with
        | Some evt ->
            appendLine "**First event after gap:**"
            appendFormat "- Provider: `%s`" evt.ProviderName
            appendFormat "- Event: `%s`" evt.EventName
            appendFormat "- Thread: %d" evt.ThreadId
            appendFormat "- Payload: `%s`" evt.PayloadSummary
        | None -> ()
    else
        appendLine "### ✅ No Significant Hang Detected"
        appendLine ""
        appendLine "No time gaps > 1 second found in the event stream."
    
    appendLine ""
    
    // Timeline Analysis
    appendLine "## Timeline Analysis"
    appendLine ""
    
    if allEvents.Count > 0 then
        let sorted = allEvents |> Seq.sortBy (fun e -> e.TimeStamp) |> Seq.toArray
        let firstEvent = sorted.[0]
        let lastEvent = sorted.[sorted.Length - 1]
        let duration = (lastEvent.TimeStamp - firstEvent.TimeStamp).TotalSeconds
        
        appendFormat "**First event:** %s" (firstEvent.TimeStamp.ToString("HH:mm:ss.fff"))
        appendFormat "**Last event:** %s" (lastEvent.TimeStamp.ToString("HH:mm:ss.fff"))
        appendFormat "**Duration:** %.2f seconds" duration
        appendFormat "**Events per second:** %.2f" (float allEvents.Count / duration)
        appendLine ""
        
        // Event density over time (buckets of 10 seconds)
        appendLine "### Event Density Over Time"
        appendLine ""
        appendLine "| Time Range | Event Count | Events/sec |"
        appendLine "|------------|-------------|------------|"
        
        let bucketSize = TimeSpan.FromSeconds(10.0)
        let mutable bucketStart = firstEvent.TimeStamp
        while bucketStart < lastEvent.TimeStamp do
            let bucketEnd = bucketStart + bucketSize
            let count = sorted |> Seq.filter (fun e -> e.TimeStamp >= bucketStart && e.TimeStamp < bucketEnd) |> Seq.length
            let eventsPerSec = float count / 10.0
            appendFormat "| %s - %s | %d | %.1f |" 
                (bucketStart.ToString("HH:mm:ss")) 
                (bucketEnd.ToString("HH:mm:ss"))
                count 
                eventsPerSec
            bucketStart <- bucketEnd
    else
        appendLine "*No events recorded*"
    
    appendLine ""
    
    // All time gaps
    if timeGaps.Length > 0 then
        appendLine "### All Significant Time Gaps (> 1 second)"
        appendLine ""
        appendLine "| # | Start | End | Duration (s) | Last Event Before |"
        appendLine "|---|-------|-----|--------------|-------------------|"
        
        for i, gap in timeGaps |> List.indexed do
            let lastEvent = 
                match gap.LastEventBefore with
                | Some e -> sprintf "%s/%s" e.ProviderName e.EventName
                | None -> "N/A"
            appendFormat "| %d | %s | %s | %.2f | %s |" 
                (i + 1)
                (gap.StartTime.ToString("HH:mm:ss.fff"))
                (gap.EndTime.ToString("HH:mm:ss.fff"))
                gap.GapSeconds
                lastEvent
        appendLine ""
    
    // Hot Methods
    appendLine "## Hot Methods (Most JIT'd)"
    appendLine ""
    
    let hotMethods = getHotMethods 20
    if hotMethods.Length > 0 then
        appendLine "| Method | JIT Count |"
        appendLine "|--------|-----------|"
        for (method, count) in hotMethods do
            appendFormat "| `%s` | %d |" method count
    else
        appendLine "*No method JIT events recorded*"
    
    appendLine ""
    
    // Provider Statistics
    appendLine "## Provider Statistics"
    appendLine ""
    appendLine "| Provider | Event Count |"
    appendLine "|----------|-------------|"
    
    for kvp in providerCounts |> Seq.sortByDescending (fun k -> k.Value) do
        appendFormat "| `%s` | %d |" kvp.Key kvp.Value
    
    appendLine ""
    
    // Thread Activity
    appendLine "## Thread Activity"
    appendLine ""
    
    let threadSummary = getThreadActivitySummary ()
    if threadSummary.Length > 0 then
        appendLine "| Thread ID | Event Count | First Event | Last Event | Active Duration (s) |"
        appendLine "|-----------|-------------|-------------|------------|---------------------|"
        
        for thread in threadSummary |> List.truncate 20 do
            let duration = (thread.LastEvent - thread.FirstEvent).TotalSeconds
            appendFormat "| %d | %d | %s | %s | %.2f |" 
                thread.ThreadId 
                thread.EventCount
                (thread.FirstEvent.ToString("HH:mm:ss.fff"))
                (thread.LastEvent.ToString("HH:mm:ss.fff"))
                duration
    else
        appendLine "*No thread activity recorded*"
    
    appendLine ""
    
    // F# Compiler Activity
    appendLine "## F# Compiler Activity"
    appendLine ""
    
    if fsharpCompilerEvents.Count > 0 then
        appendFormat "**Total F# compiler related events:** %d" fsharpCompilerEvents.Count
        appendLine ""
        appendLine "### Last 20 F# Compiler Events"
        appendLine ""
        appendLine "| Time | Thread | Provider | Event | Payload |"
        appendLine "|------|--------|----------|-------|---------|"
        
        for evt in fsharpCompilerEvents |> Seq.sortByDescending (fun e -> e.TimeStamp) |> Seq.truncate 20 do
            appendFormat "| %s | %d | `%s` | `%s` | `%s` |"
                (evt.TimeStamp.ToString("HH:mm:ss.fff"))
                evt.ThreadId
                evt.ProviderName
                evt.EventName
                (if evt.PayloadSummary.Length > 50 then evt.PayloadSummary.Substring(0, 50) + "..." else evt.PayloadSummary)
    else
        appendLine "*No F# compiler specific events recorded*"
    
    appendLine ""
    
    // MSBuild Activity
    appendLine "## MSBuild Activity"
    appendLine ""
    
    if msbuildEvents.Count > 0 then
        appendFormat "**Total MSBuild related events:** %d" msbuildEvents.Count
        appendLine ""
        appendLine "### Last 20 MSBuild Events"
        appendLine ""
        appendLine "| Time | Thread | Provider | Event | Payload |"
        appendLine "|------|--------|----------|-------|---------|"
        
        for evt in msbuildEvents |> Seq.sortByDescending (fun e -> e.TimeStamp) |> Seq.truncate 20 do
            appendFormat "| %s | %d | `%s` | `%s` | `%s` |"
                (evt.TimeStamp.ToString("HH:mm:ss.fff"))
                evt.ThreadId
                evt.ProviderName
                evt.EventName
                (if evt.PayloadSummary.Length > 50 then evt.PayloadSummary.Substring(0, 50) + "..." else evt.PayloadSummary)
    else
        appendLine "*No MSBuild specific events recorded*"
    
    appendLine ""
    
    // Lock Contention
    appendLine "## Lock Contention Events"
    appendLine ""
    
    if lockContentionEvents.Count > 0 then
        appendFormat "**Total contention events:** %d" lockContentionEvents.Count
        appendLine ""
        appendLine "### Contention Events (last 20)"
        appendLine ""
        appendLine "| Time | Thread | Provider |"
        appendLine "|------|--------|----------|"
        
        for evt in lockContentionEvents |> Seq.sortByDescending (fun e -> e.TimeStamp) |> Seq.truncate 20 do
            appendFormat "| %s | %d | `%s` |"
                (evt.TimeStamp.ToString("HH:mm:ss.fff"))
                evt.ThreadId
                evt.ProviderName
    else
        appendLine "*No lock contention events recorded*"
    
    appendLine ""
    
    // GC Activity
    appendLine "## GC Activity"
    appendLine ""
    
    if gcEvents.Count > 0 then
        appendFormat "**Total GC events:** %d" gcEvents.Count
        appendLine ""
        
        let gen0 = gcEvents |> Seq.filter (fun e -> e.PayloadSummary.Contains("Gen0")) |> Seq.length
        let gen1 = gcEvents |> Seq.filter (fun e -> e.PayloadSummary.Contains("Gen1")) |> Seq.length
        let gen2 = gcEvents |> Seq.filter (fun e -> e.PayloadSummary.Contains("Gen2")) |> Seq.length
        
        appendFormat "- Gen 0 collections: %d" gen0
        appendFormat "- Gen 1 collections: %d" gen1
        appendFormat "- Gen 2 collections: %d" gen2
    else
        appendLine "*No GC events recorded*"
    
    appendLine ""
    
    // Recommendations
    appendLine "## Recommendations"
    appendLine ""
    
    if timeGaps.Length > 0 then
        appendLine "Based on the trace analysis:"
        appendLine ""
        appendLine "1. **Investigate the hang point:** The trace shows significant time gaps indicating where the process became unresponsive."
        appendLine "2. **Check the last active method:** Review the method(s) active before the hang occurred."
        appendLine "3. **Analyze dump file:** If a memory dump was captured, use `analyze-dump.fsx` for detailed thread and lock analysis."
        appendLine "4. **Look for deadlocks:** Multiple threads waiting on locks may indicate a deadlock condition."
        appendLine "5. **Check F# compiler activity:** Review F# compiler events near the hang point for type checking or optimization issues."
    else
        appendLine "The trace completed without significant hangs. Consider:"
        appendLine ""
        appendLine "1. **Performance analysis:** Review hot methods and GC pressure."
        appendLine "2. **Verify reproduction:** The hang may be intermittent."
        appendLine "3. **Increase timeout:** If the process was slow but didn't hang, increase the timeout."
    
    appendLine ""
    appendLine "---"
    appendLine "*Report generated by analyze-trace.fsx*"
    
    sb.ToString()

// Main execution
printfn "Starting trace analysis..."
let success = processEvents()

if success then
    printfn "Generating report..."
    let report = generateReport()
    let reportPath = Path.Combine(outputDir, "trace-analysis.md")
    File.WriteAllText(reportPath, report)
    printfn "Report written to: %s" reportPath
else
    printfn "Analysis failed - no report generated"
    // Write a failure report
    let failureReport = """# Trace Analysis Report

## ❌ Analysis Failed

The trace file could not be analyzed. This may be because:

1. The trace file does not exist
2. The trace file is corrupted
3. The trace file is in an unsupported format
4. dotnet-trace was killed before generating the output

### Next Steps

1. Verify the trace file exists and has non-zero size
2. Try running `dotnet-trace convert` on the file manually
3. Check the console output for errors during collection
4. Try collecting a new trace with a longer timeout

---
*Report generated by analyze-trace.fsx*
"""
    let reportPath = Path.Combine(outputDir, "trace-analysis.md")
    File.WriteAllText(reportPath, failureReport)
    printfn "Failure report written to: %s" reportPath

printfn "Done."
