#r "nuget: Microsoft.Diagnostics.Tracing.TraceEvent, 3.1.21"

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Text.Json

open Microsoft.Diagnostics.Tracing
open Microsoft.Diagnostics.Tracing.Etlx
open Microsoft.Diagnostics.Tracing.Parsers
open Microsoft.Diagnostics.Tracing.Parsers.Clr

let require condition message =
    if not condition then
        invalidOp message

let reduce input expectedPid cache =
    let file = FileInfo input
    require file.Exists "Trace input does not exist"
    require (file.Length > 0L && file.Length <= 512L * 1024L * 1024L) "Trace input size must be 1..512 MiB"

    let providers = HashSet<string>(StringComparer.Ordinal)
    let allocations = ResizeArray<struct {| timestamp_ms: float; bytes: int64 |}>()
    let heaps = ResizeArray<struct {| timestamp_ms: float; bytes: int64; kind: string |}>()
    let collections = ResizeArray<struct {| start_ms: float; end_ms: float; generation: int |}>()
    let pauses = ResizeArray<struct {| start_ms: float; end_ms: float |}>()
    let pendingCollections = Dictionary<struct (int * int), struct (float * int)>()
    let pendingPauses = Dictionary<int, float>()
    let mutable runtime = ""
    let mutable first = Double.PositiveInfinity
    let mutable last = Double.NegativeInfinity
    let mutable previous = Double.NegativeInfinity
    let mutable eventCount = 0
    let mutable rowCount = 0
    let mutable allocationEventCount = 0L

    let row () =
        rowCount <- rowCount + 1
        require (rowCount <= 100000) "Trace reduction row limit exceeded"

    let runtimeInfo (data: RuntimeInformationTraceData) =
        // EventPipe runtime metadata identifies the process; not every event kind has a usable PID.
        require (data.ProcessID = expectedPid) "Runtime metadata process PID does not match expected PID"
        let version = $"{data.VMMajorVersion}.{data.VMMinorVersion}.{data.VMBuildNumber}.{data.VMQfeNumber}"
        require (data.VMMajorVersion > 0 && (runtime = "" || runtime = version)) "Missing or conflicting runtime metadata"
        runtime <- version

    use source = new EventPipeEventSource(input)
    source.Clr.add_RuntimeStart(Action<_> runtimeInfo)
    let rundown = ClrRundownTraceEventParser(source)
    rundown.add_RuntimeStart(Action<_> runtimeInfo)
    source.add_AllEvents(fun data ->
        eventCount <- eventCount + 1
        require (eventCount <= 5000000) "Trace event limit exceeded"
        providers.Add data.ProviderName |> ignore
        if data.ProviderGuid <> ClrRundownTraceEventParser.ProviderGuid then
            let time = data.TimeStampRelativeMSec
            require (Double.IsFinite time && time >= 0.0 && time >= previous) "Trace events are out of order"
            previous <- time
            first <- min first time
            last <- max last time
        if eventCount % 4096 = 0 then
            let pending = FileInfo(cache + ".new")
            require (not pending.Exists || pending.Length <= 1024L * 1024L * 1024L) "Trace cache size limit exceeded")

    source.Clr.add_GCAllocationTick(fun data ->
        // Version 2 introduced the 64-bit estimate; only old payloads use AllocationAmount.
        let bytes = if data.Version >= 2 then data.AllocationAmount64 else int64 (uint32 data.AllocationAmount)
        require (bytes > 0L) "Invalid allocation tick byte estimate"
        allocationEventCount <- allocationEventCount + 1L
        let bucket = floor (data.TimeStampRelativeMSec / 100.0) * 100.0
        let lastIndex = allocations.Count - 1
        if lastIndex >= 0 && allocations[lastIndex].timestamp_ms = bucket then
            let previous = allocations[lastIndex]
            require (bytes <= Int64.MaxValue - previous.bytes) "Allocation bucket byte estimate overflow"
            allocations[lastIndex] <- struct {| timestamp_ms = bucket; bytes = previous.bytes + bytes |}
        else
            row ()
            allocations.Add(struct {| timestamp_ms = bucket; bytes = bytes |}))
    source.Clr.add_GCHeapStats(fun data ->
        row ()
        require (data.TotalHeapSize >= 0L) "Invalid GC heap size"
        heaps.Add(struct {| timestamp_ms = data.TimeStampRelativeMSec; bytes = data.TotalHeapSize; kind = "gc-heap-stats" |}))
    source.Clr.add_GCStart(fun data ->
        let key = struct (data.ClrInstanceID, data.Count)
        require (not (pendingCollections.ContainsKey key)) "Duplicate GC start"
        require (data.Depth >= 0 && data.Depth <= 2) "Invalid GC generation"
        pendingCollections.Add(key, struct (data.TimeStampRelativeMSec, data.Depth)))
    source.Clr.add_GCStop(fun data ->
        let key = struct (data.ClrInstanceID, data.Count)
        match pendingCollections.TryGetValue key with
        | true, struct (start, generation) ->
            row ()
            require (data.TimeStampRelativeMSec >= start) "GC interval is out of order"
            collections.Add(struct {| start_ms = start; end_ms = data.TimeStampRelativeMSec; generation = generation |})
            pendingCollections.Remove key |> ignore
        | _ -> invalidOp "Missing GC start")
    source.Clr.add_GCSuspendEEStart(fun data ->
        if data.Reason = GCSuspendEEReason.SuspendForGC || data.Reason = GCSuspendEEReason.SuspendForGCPrep then
            require (not (pendingPauses.ContainsKey data.ClrInstanceID)) "Duplicate GC suspension"
            pendingPauses.Add(data.ClrInstanceID, data.TimeStampRelativeMSec))
    source.Clr.add_GCRestartEEStop(fun data ->
        match pendingPauses.TryGetValue data.ClrInstanceID with
        | true, start ->
            row ()
            require (data.TimeStampRelativeMSec >= start) "GC pause is out of order"
            pauses.Add(struct {| start_ms = start; end_ms = data.TimeStampRelativeMSec |})
            pendingPauses.Remove data.ClrInstanceID |> ignore
        | _ -> ())

    let options =
        TraceLogOptions(
            ContinueOnError = false,
            KeepAllEvents = false,
            MaxEventCount = 5000000,
            LocalSymbolsOnly = true,
            ConversionLog = TextWriter.Null,
            OnLostEvents = Action<bool, int, int>(fun truncated lost _ ->
                require (not truncated && lost = 0) $"Trace truncated or lost events ({lost})")
        )
    TraceLog.CreateFromEventTraceLogFile(source, cache, options) |> ignore
    require (source.EventsLost = 0) $"Trace lost events ({source.EventsLost})"
    require (runtime <> "") "Missing runtime/process metadata"
    require (providers.Contains ClrTraceEventParser.ProviderName) "Missing runtime provider"
    require (providers.Contains "Microsoft-DotNETCore-SampleProfiler") "Missing CPU sample provider"
    require (pendingCollections.Count = 0 && pendingPauses.Count = 0) "Incomplete GC intervals or pauses"
    require (allocations.Count > 0 && heaps.Count > 0 && collections.Count > 0 && pauses.Count > 0) "Missing allocation or GC data"
    require (last > first) "Missing trace time range"
    require (FileInfo(cache).Length <= 1024L * 1024L * 1024L) "Trace cache size limit exceeded"

    use log = new TraceLog(cache)
    require (not log.Truncated && log.EventsLost = 0) "TraceLog truncated or lost events"
    let counts = Dictionary<CallStackIndex, int64>()
    let mutable samples = 0L
    for data in log.Events do
        if data.ProviderName = "Microsoft-DotNETCore-SampleProfiler" && int data.ID = 0 then
            let index = data.CallStackIndex()
            require (index <> CallStackIndex.Invalid) "CPU sample is missing stack data"
            samples <- samples + 1L
            match counts.TryGetValue index with
            | true, count -> counts[index] <- count + 1L
            | _ ->
                require (counts.Count < 65536) "CPU unique stack limit exceeded"
                counts.Add(index, 1L)
    require (samples > 0L) "Missing CPU stack samples"

    let mutable frameBytes = 0
    let mutable hasNamedFrame = false
    let stacks =
        counts
        |> Seq.sortBy (fun (KeyValue(index, count)) -> struct (-count, int index))
        |> Seq.truncate 128
        |> Seq.map (fun (KeyValue(index, count)) ->
            let frames = ResizeArray<string>()
            let mutable stack = log.CallStacks[index]
            while not (isNull stack) do
                require (frames.Count < 512) "CPU stack depth limit exceeded"
                let address = stack.CodeAddress
                let name =
                    if not (String.IsNullOrWhiteSpace address.FullMethodName) then
                        hasNamedFrame <- true
                        address.FullMethodName
                    else $"0x{address.Address:x}"
                frameBytes <- frameBytes + Encoding.UTF8.GetByteCount name
                require (frameBytes <= 4 * 1024 * 1024) "CPU stack text limit exceeded"
                frames.Add name
                stack <- stack.Caller
            struct {| frames = frames.ToArray(); weight = count |})
        |> Seq.toArray
    require hasNamedFrame "CPU stacks have no resolved method names; missing JIT/loader or rundown metadata"
    {| schema_version = 1
       kind = "trace-reduction"
       process_id = expectedPid
       runtime = runtime
       providers = providers |> Seq.sort |> Seq.toArray
       complete = true
       lost_events = 0
       first_event_ms = first
       last_event_ms = last
       cpu_sample_weight = samples
       cpu_sample_unit = "samples"
       stacks = stacks
       allocation_ticks = allocations
       allocation_aggregation =
        {| kind = "fixed-time-buckets"
           bucket_width_ms = 100
           timestamp = "bucket-start"
           source_event_count = allocationEventCount |}
       gc_intervals = collections |> Seq.sortBy _.start_ms |> Seq.toArray
       gc_pauses = pauses
       heap_observations = heaps |}

let args = fsi.CommandLineArgs |> Array.skip 1
if args.Length <> 3 then
    eprintfn "Usage: dotnet fsi AnalyzeTrace.fsx <input.nettrace> <output.json> <expected-pid>"
    exit 1

let output = Path.GetFullPath args[1]
let cache = output + "." + Guid.NewGuid().ToString("N") + ".etlx"
try
    try
        let pid =
            match Int32.TryParse args[2] with
            | true, value when value > 0 -> value
            | _ -> invalidOp "Expected process PID must be a positive integer"
        require (not (String.Equals(Path.GetFullPath args[0], output, StringComparison.OrdinalIgnoreCase))) "Trace input and output must differ"
        let data = reduce args[0] pid cache |> JsonSerializer.SerializeToUtf8Bytes
        require (data.Length <= 16 * 1024 * 1024) "Trace reduction exceeds 16 MiB"
        File.WriteAllBytes(output, data)
    finally
        File.Delete cache
        File.Delete(cache + ".new")
with error ->
    let message = "Trace reduction blocked: " + error.Message
    File.WriteAllBytes(output, JsonSerializer.SerializeToUtf8Bytes {| schema_version = 1; kind = "trace-reduction"; complete = false; error = message |})
    eprintfn "%s" message
    exit 1
