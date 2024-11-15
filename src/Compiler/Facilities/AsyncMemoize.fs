namespace Internal.Utilities.Collections

open System
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library
open System.Runtime.CompilerServices

type AsyncLazyState<'t> =
    | Initial of Async<'t>
    | Created of Task<'t> * CancellationTokenSource * int

/// Represents a computation that will execute only once but can be requested by multiple clients.
/// It keeps track of the number of requests. When all clients cancel their requests, the underlying computation will also cancel and can be restarted.
/// If cancelUnawaited is set to false, the computation will run to completion even when all requests are canceled.
type AsyncLazy<'t>(computation: Async<'t>, ?cancelUnawaited: bool) =

    let cancelUnawaited = defaultArg cancelUnawaited true

    let stateUpdateSync = obj()
    let mutable state = Initial computation

    let cancelIfUnawaited () =
        match state with
        | Created(work, cts, 0) when not work.IsCompleted ->
            cts.Cancel()
            state <- Initial computation
        | _ -> ()

    let afterRequest () =
        match state with
        | Created(work, cts, count) ->
            state <- Created(work, cts, count - 1)
            if cancelUnawaited then cancelIfUnawaited () 
        | _ -> ()

    let request () =
        match state with
        | Initial computation ->
            let cts = new CancellationTokenSource()
            let work = Async.StartAsTask(computation, cancellationToken = cts.Token)
            state <- Created (work, cts, 1)
            work
        | Created (work, cts, count) ->
            state <- Created (work, cts, count + 1)
            work

    member _.Request =
        async {
            let work = lock stateUpdateSync request
            try
                let! ct = Async.CancellationToken
                let options = TaskContinuationOptions.ExecuteSynchronously
                try
                    return!
                        // Using ContinueWith with a CancellationToken allows detaching from the running 'work' task.
                        // This ensures the lazy 'work' and its awaiting requests can be independently managed 
                        // by separate CancellationTokenSources, enabling individual cancellation.
                        // Essentially, if this async computation is canceled, it won't wait for the 'work' to complete
                        // but will immediately proceed to the finally block.
                        work.ContinueWith((fun (t: Task<_>) -> t.Result), ct, options, TaskScheduler.Current)
                        |> Async.AwaitTask
                // Cancellation check before entering the `with` ensures TaskCanceledEXception coming from the ContinueWith task will never be raised here.
                // The cancellation continuation will always be called in case of cancellation.
                with ex -> return raise ex 
            finally
                lock stateUpdateSync afterRequest
        }


    member _.CancelIfUnawaited() = lock stateUpdateSync cancelIfUnawaited

    member _.Task = match state with Created(t, _, _) -> Some t | _ -> None
    member this.Result = 
        this.Task
        |> Option.filter (fun t -> t.Status = TaskStatus.RanToCompletion)
        |> Option.map _.Result

[<AutoOpen>]
module internal Utils =

    /// Return file name with one directory above it
    let shortPath (path: string) =
        let dirPath = !! Path.GetDirectoryName(path)

        let dir =
            dirPath.Split Path.DirectorySeparatorChar
            |> Array.tryLast
            |> Option.map (sprintf "%s/")
            |> Option.defaultValue ""

        $"{dir}{Path.GetFileName path}"

type internal JobEvent =
    | Requested
    | Started
    | Restarted
    | Finished
    | Canceled
    | Evicted
    | Collected
    | Weakened
    | Strengthened
    | Failed
    | Cleared
    static member AllEvents = [Requested; Started; Restarted; Finished; Canceled; Evicted; Collected; Weakened; Strengthened; Failed; Cleared]

type internal ICacheKey<'TKey, 'TVersion> =
    abstract member GetKey: unit -> 'TKey
    abstract member GetVersion: unit -> 'TVersion
    abstract member GetLabel: unit -> string

[<Extension>]
type Extensions =

    [<Extension>]
    static member internal WithExtraVersion(cacheKey: ICacheKey<_, _>, extraVersion) =
        { new ICacheKey<_, _> with
            member _.GetLabel() = cacheKey.GetLabel()
            member _.GetKey() = cacheKey.GetKey()
            member _.GetVersion() = cacheKey.GetVersion(), extraVersion
        }

type private KeyData<'TKey, 'TVersion> =
    {
        Label: string
        Key: 'TKey
        Version: 'TVersion
    }

type Job<'t> = AsyncLazy<Result<'t, exn> * CapturingDiagnosticsLogger>

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal AsyncMemoize<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality
#if !NO_CHECKNULLS
    and 'TKey:not null
    and 'TVersion:not null
#endif
    >
    (?keepStrongly, ?keepWeakly, ?name: string, ?cancelUnawaitedJobs: bool, ?cancelDuplicateRunningJobs: bool) =

    let name = defaultArg name "N/A"
    let cancelUnawaitedJobs = defaultArg cancelUnawaitedJobs true
    let cancelDuplicateRunningJobs = defaultArg cancelDuplicateRunningJobs false

    let event = Event<_>()

    let eventCounts = [for j in JobEvent.AllEvents -> j, ref 0] |> dict
    let mutable hits = 0
    let mutable duration = 0L
    let mutable events_in_flight = 0

    let keyTuple (keyData: KeyData<_, _>) = keyData.Label, keyData.Key, keyData.Version

    let logK (eventType: JobEvent) key =
        Interlocked.Increment(eventCounts[eventType]) |> ignore
        event.Trigger(eventType, key)

    let log eventType keyData = logK eventType (keyTuple keyData)

    let cache =
        LruCache<'TKey, 'TVersion, Job<'TValue>>(
            keepStrongly = defaultArg keepStrongly 100,
            keepWeakly = defaultArg keepWeakly 200,
            event =
                (function
                | CacheEvent.Evicted -> logK JobEvent.Evicted
                | CacheEvent.Collected -> logK JobEvent.Collected
                | CacheEvent.Weakened -> logK JobEvent.Weakened
                | CacheEvent.Strengthened -> logK JobEvent.Strengthened
                | CacheEvent.Cleared -> logK JobEvent.Cleared))

    member _.Get(key: ICacheKey<_, _>, computation) =
        let key =
            {
                Label = key.GetLabel()
                Key = key.GetKey()
                Version = key.GetVersion()
            }

        let wrappedComputation =
            async {
                use! _handler = Async.OnCancel (fun () -> log Canceled key)
                let sw = Stopwatch.StartNew()
                log Started key
                let logger = CapturingDiagnosticsLogger "cache"
                SetThreadDiagnosticsLoggerNoUnwind logger

                try
                    let! result = computation
                    log Finished key
                    Interlocked.Add(&duration, sw.ElapsedMilliseconds) |> ignore
                    return Result.Ok result, logger
                with
                | ex ->
                    log Failed key
                    return Result.Error ex, logger
            }

        let getOrAdd () =
            let cached, otherVersions = cache.GetAll(key.Key, key.Version)

            let countHit v = Interlocked.Increment &hits |> ignore; v
            let cacheSetNewJob () =
                let job = Job(wrappedComputation, cancelUnawaited = cancelUnawaitedJobs)
                cache.Set(key.Key, key.Version, key.Label, job)
                job

            otherVersions,

            cached
            |> Option.map countHit
            |> Option.defaultWith cacheSetNewJob

        async {            
            let otherVersions, job = lock cache getOrAdd

            log Requested key

            if cancelDuplicateRunningJobs && not cancelUnawaitedJobs then
                otherVersions |> Seq.map snd |> Seq.iter _.CancelIfUnawaited()

            use _ = new CompilationGlobalsScope()

            let! result, logger = job.Request
            logger.CommitDelayedDiagnostics DiagnosticsThreadStatics.DiagnosticsLogger
            match result with
            | Ok result ->
                return result
            | Error ex ->
                return raise ex
        }

    member _.TryGet(key: 'TKey, predicate: 'TVersion -> bool) : 'TValue option =
        let versionsAndJobs =
            lock cache <| fun () ->cache.GetAll(key)

        versionsAndJobs
        |> Seq.tryPick (fun (version, job) ->
            match predicate version, job.Result with
            | true, Some(Ok result, _) -> Some result
            | _ -> None)

    member _.Clear() = cache.Clear()

    member _.Clear predicate = cache.Clear predicate

    member val Event = event.Publish

    member this.OnEvent = this.Event.Add

    member this.Count = lock cache <| fun () -> cache.Count

    member _.Updating = false

    member this.DebuggerDisplay =

        let (|Running|_|) (job: Job<_>) = job.Task |> Option.filter (_.IsCompleted >> not)
        let (|Faulted|_|) (job: Job<_>) = job.Task |> Option.filter _.IsFaulted

        let status = function
            | Running _ -> "Running"
            | Faulted _ -> "Faulted"
            | _ -> "other"

        let cachedJobs = cache.GetValues() |> Seq.map (fun (_,_,job) -> job)

        let valueStats = cachedJobs |> Seq.countBy status |> Map
        let getStat key = valueStats.TryFind key |> Option.defaultValue 0

        let running =
            let count = getStat "Running"
            if  count > 0 then $" Running {count}" else ""

        let finished = eventCounts[Finished].Value
        let avgDuration = if finished = 0 then "" else $"| Avg: %.0f{float duration / float finished} ms"

        let requests = eventCounts[Requested].Value
        let hitRatio = if requests = 0 then "" else $" (%.0f{float hits / (float (requests)) * 100.0} %%)"

        let faulted = getStat "Faulted"
        let failed = eventCounts[Failed].Value

        let stats =
            seq {
                if faulted + failed > 0 then
                    " (_!_) "
                for j in eventCounts.Keys do
                    let count = eventCounts[j].Value
                    if count > 0 then $"| {j}: {count}" else ""
                $"| hits: {hits}{hitRatio} "
            }
            |> String.concat ""

        $"{running} {cache.DebuggerDisplay} {stats}{avgDuration}"

/// A drop-in replacement for AsyncMemoize that disables caching and just runs the computation every time.
[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal AsyncMemoizeDisabled<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality>
    (?keepStrongly, ?keepWeakly, ?name: string, ?cancelDuplicateRunningJobs: bool) =

    do ignore (keepStrongly, keepWeakly, name, cancelDuplicateRunningJobs)

    let mutable requests = 0

    member _.Get(_key: ICacheKey<_, _>, computation) =
        Interlocked.Increment &requests |> ignore
        computation

    member _.DebuggerDisplay = $"(disabled) requests: {requests}"