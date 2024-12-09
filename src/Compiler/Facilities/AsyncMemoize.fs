namespace Internal.Utilities.Collections

open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library

type AsyncLazyState<'t> =
    | Initial of computation: Async<'t>
    | Running of initialComputation: Async<'t> * work: Task<'t> * CancellationTokenSource * requestCount: int
    | Completed of result: 't
    | Faulted of exn

/// Represents a computation that will execute only once but can be requested by multiple clients.
/// It keeps track of the number of requests. When all clients cancel their requests, the underlying computation will also cancel and can be restarted.
/// If cancelUnawaited is set to false, the computation will run to completion even when all requests are canceled.
type AsyncLazy<'t> private (initial: AsyncLazyState<'t>, cancelUnawaited: bool, cacheException: bool) =

    let stateUpdateSync = obj()
    let mutable state = initial
    // This should remain the only function that mutates the state.
    let withStateUpdate f =
        lock stateUpdateSync <| fun () ->
            let next, result = f state
            state <- next
            result
    let updateState f = withStateUpdate <| fun prev -> f prev, ()

    let cancelIfUnawaited cancelUnawaited = function
        | Running(computation, _, cts, 0) when cancelUnawaited ->
            // To keep state updates fast we don't actually wait for the work to cancel.
            cts.Cancel()
            Initial computation
        | state -> state

    let afterRequest = function
        | Running(c, work, cts, count) -> Running(c, work, cts, count - 1) |> cancelIfUnawaited cancelUnawaited
        | state -> state // Nothing more to do if state already transitioned.

    let detachable (work: Task<'t>) =
        async {
            try
                let! ct = Async.CancellationToken
                return!
                    // Using ContinueWith with a CancellationToken allows detaching from the running 'work' task.
                    // This ensures the lazy 'work' and its awaiting requests can be independently managed 
                    // by separate CancellationTokenSources, enabling individual cancellation.
                    // Essentially, if this async computation is canceled, it won't wait for the 'work' to complete
                    // but will immediately proceed to the finally block.
                    work.ContinueWith((fun (t: Task<_>) -> t.Result), ct)
                    |> Async.AwaitTask
            // Cancellation check before entering the `with` ensures TaskCanceledException coming from the ContinueWith task will never be raised here.
            // The cancellation continuation will always be called in case of cancellation.
            with exn -> return raise exn
        }

    let request = function
        | Initial computation ->
            let cts = new CancellationTokenSource()
            let work = Async.StartAsTask( async {
                try
                    let! result = computation
                    // If associated cts is signalled it means this work item was abandoned
                    // and it should not alter the state.
                    updateState <| function
                        | state when cts.IsCancellationRequested -> state
                        | _ -> Completed result
                    return result
                with
                | exn ->
                    updateState <| function
                        | state when cts.IsCancellationRequested -> state
                        | _ -> if cacheException then Faulted exn else Initial computation
                    return raise exn
            }, cancellationToken = cts.Token)
            Running (computation, work, cts, 1),
            detachable work
        | Running (c, work, cts, count) ->
            Running (c, work, cts, count + 1),
            detachable work
        | Completed result as state ->
            state, async { return result }
        | Faulted exn as state ->
            state, async { return raise exn }

    // computation will deallocate after state transition to Completed ot Faulted.
    new (computation, ?cancelUnawaited: bool, ?cacheException) =
        AsyncLazy(Initial computation, defaultArg cancelUnawaited true, defaultArg cacheException false)

    member _.Request() =
        async { 
            try
                return! withStateUpdate request
            finally
                updateState afterRequest
        }

    member _.CancelIfUnawaited() = updateState (cancelIfUnawaited true)

    member _.State = state

    member this.TryResult =
        match state with
        | Completed result -> Some result
        | _ -> None

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
    // TODO Key should probably be renamed to Identifier
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

                match! computation |> Async.Catch with
                | Choice1Of2 result ->
                    log Finished key
                    Interlocked.Add(&duration, sw.ElapsedMilliseconds) |> ignore
                    return Result.Ok result, logger
                | Choice2Of2 exn ->
                    log Failed key
                    return Result.Error exn, logger
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

            let! result, logger = job.Request()
            logger.CommitDelayedDiagnostics DiagnosticsThreadStatics.DiagnosticsLogger
            match result with
            | Ok result ->
                return result
            | Error exn ->
                return raise exn
        }

    member _.TryGet(key: 'TKey, predicate: 'TVersion -> bool) : 'TValue option =
        lock cache <| fun () ->
            cache.GetAll(key)
            |> Seq.tryPick (fun (version, job) ->
                match predicate version, job.TryResult with
                | true, Some(Ok result, _) -> Some result
                | _ -> None)

    member _.Clear() = lock cache cache.Clear

    member _.Clear predicate = lock cache <| fun () -> cache.Clear predicate

    member val Event = event.Publish

    member this.OnEvent = this.Event.Add

    member this.Count = lock cache <| fun () -> cache.Count

    member this.DebuggerDisplay =

        let cachedJobs = cache.GetValues() |> Seq.map (fun (_,_,job) -> job)

        let jobStateName = function
        | Initial _ -> nameof Initial
        | Running _ -> nameof Running
        | Completed _ -> nameof Completed
        | Faulted _ -> nameof Faulted

        let valueStats = cachedJobs |> Seq.countBy (_.State >> jobStateName) |> Map
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