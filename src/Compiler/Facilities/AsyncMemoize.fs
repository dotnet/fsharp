namespace Internal.Utilities.Collections

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal Utils =

    /// Return file name with one directory above it
    let shortPath path =
        let dirPath = Path.GetDirectoryName path

        let dir =
            dirPath.Split Path.DirectorySeparatorChar
            |> Array.tryLast
            |> Option.map (sprintf "%s/")
            |> Option.defaultValue ""

        $"{dir}{Path.GetFileName path}"

    let replayDiagnostics (logger: DiagnosticsLogger) = Seq.iter ((<|) logger.DiagnosticSink)

    let (|TaskCancelled|_|) (ex: exn) =
        match ex with
        | :? System.Threading.Tasks.TaskCanceledException as tce -> Some tce
        //| :? System.AggregateException as ae ->
        //    if ae.InnerExceptions |> Seq.forall (fun e -> e :? System.Threading.Tasks.TaskCanceledException) then
        //        ae.InnerExceptions |> Seq.tryHead |> Option.map (fun e -> e :?> System.Threading.Tasks.TaskCanceledException)
        //    else
        //        None
        | _ -> None

type internal StateUpdate<'TValue> =
    | CancelRequest
    | OriginatorCanceled
    | JobCompleted of 'TValue * (PhasedDiagnostic * FSharpDiagnosticSeverity) list
    | JobFailed of exn * (PhasedDiagnostic * FSharpDiagnosticSeverity) list

type internal MemoizeReply<'TValue> =
    | New of CancellationToken
    | Existing of Task<'TValue>

type internal MemoizeRequest<'TValue> = GetOrCompute of NodeCode<'TValue> * CancellationToken

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal Job<'TValue> =
    | Running of TaskCompletionSource<'TValue> * CancellationTokenSource * NodeCode<'TValue> * DateTime * ResizeArray<DiagnosticsLogger>
    | Completed of 'TValue * (PhasedDiagnostic * FSharpDiagnosticSeverity) list
    | Canceled of DateTime
    | Failed of DateTime * exn // TODO: probably we don't need to keep this

    member this.DebuggerDisplay =
        match this with
        | Running(_, cts, _, ts, _) ->
            let cancellation =
                if cts.IsCancellationRequested then
                    " ! Cancellation Requested"
                else
                    ""

            $"Running since {ts.ToShortTimeString()}{cancellation}"
        | Completed(value, diags) -> $"Completed {value}" + (if diags.Length > 0 then $" ({diags.Length})" else "")
        | Canceled _ -> "Canceled"
        | Failed(_, ex) -> $"Failed {ex}"

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

type internal AsyncLock() =

    let semaphore = new SemaphoreSlim(1, 1)

    member _.Semaphore = semaphore

    member _.Do(f) =
        task {
            do! semaphore.WaitAsync()

            try
                return! f ()
            finally
                semaphore.Release() |> ignore
        }

    interface IDisposable with
        member _.Dispose() = semaphore.Dispose()

type internal CachingDiagnosticsLogger(originalLogger: DiagnosticsLogger option) =
    inherit DiagnosticsLogger($"CachingDiagnosticsLogger")

    let capturedDiagnostics = ResizeArray()

    override _.ErrorCount =
        originalLogger
        |> Option.map (fun x -> x.ErrorCount)
        |> Option.defaultValue capturedDiagnostics.Count

    override _.DiagnosticSink(diagnostic: PhasedDiagnostic, severity: FSharpDiagnosticSeverity) =
        originalLogger |> Option.iter (fun x -> x.DiagnosticSink(diagnostic, severity))
        capturedDiagnostics.Add(diagnostic, severity)

    member _.CapturedDiagnostics = capturedDiagnostics |> Seq.toList

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal AsyncMemoize<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality>
    (?keepStrongly, ?keepWeakly, ?name: string, ?cancelDuplicateRunningJobs: bool) =

    let name = defaultArg name "N/A"
    let cancelDuplicateRunningJobs = defaultArg cancelDuplicateRunningJobs false

    let event = Event<_>()

    let mutable errors = 0
    let mutable hits = 0
    let mutable started = 0
    let mutable completed = 0
    let mutable canceled = 0
    let mutable restarted = 0
    let mutable failed = 0
    let mutable evicted = 0
    let mutable collected = 0
    let mutable strengthened = 0
    let mutable cleared = 0

    let mutable cancel_ct_registration_original = 0
    let mutable cancel_exception_original = 0
    let mutable cancel_original_processed = 0
    let mutable cancel_ct_registration_subsequent = 0
    let mutable cancel_exception_subsequent = 0
    let mutable cancel_subsequent_processed = 0

    let failures = ResizeArray()
    let mutable avgDurationMs = 0.0

    let cache =
        LruCache<'TKey, 'TVersion, Job<'TValue>>(
            keepStrongly = defaultArg keepStrongly 100,
            keepWeakly = defaultArg keepWeakly 200,
            requiredToKeep =
                (function
                | Running _ -> true
                | Job.Canceled at when at > DateTime.Now.AddMinutes -5.0 -> true
                | Job.Failed(at, _) when at > DateTime.Now.AddMinutes -5.0 -> true
                | _ -> false),
            event =
                (function
                | CacheEvent.Evicted ->
                    (fun k ->
                        Interlocked.Increment &evicted |> ignore
                        event.Trigger(JobEvent.Evicted, k))
                | CacheEvent.Collected ->
                    (fun k ->
                        Interlocked.Increment &collected |> ignore
                        event.Trigger(JobEvent.Collected, k))
                | CacheEvent.Weakened -> (fun k -> event.Trigger(JobEvent.Weakened, k))
                | CacheEvent.Strengthened ->
                    (fun k ->
                        Interlocked.Increment &strengthened |> ignore
                        event.Trigger(JobEvent.Strengthened, k))
                | CacheEvent.Cleared ->
                    (fun k ->
                        Interlocked.Increment &cleared |> ignore
                        event.Trigger(JobEvent.Cleared, k)))
        )

    let requestCounts = Dictionary<KeyData<_, _>, int>()
    let cancellationRegistrations = Dictionary<_, _>()

    let saveRegistration key registration =
        cancellationRegistrations[key] <-
            match cancellationRegistrations.TryGetValue key with
            | true, registrations -> registration :: registrations
            | _ -> [ registration ]

    let cancelRegistration key =
        match cancellationRegistrations.TryGetValue key with
        | true, registrations ->
            for r: CancellationTokenRegistration in registrations do
                r.Dispose()

            cancellationRegistrations.Remove key |> ignore
        | _ -> ()

    let incrRequestCount key =
        requestCounts[key] <-
            if requestCounts.ContainsKey key then
                requestCounts[key] + 1
            else
                1

    let decrRequestCount key =
        if requestCounts.ContainsKey key then
            requestCounts[key] <- requestCounts[key] - 1

    let log (eventType, keyData: KeyData<_, _>) =
        event.Trigger(eventType, (keyData.Label, keyData.Key, keyData.Version))

    let lock = new AsyncLock()

    let processRequest post (key: KeyData<_, _>, msg) diagnosticLogger =

        lock.Do(fun () ->
            task {

                let cached, otherVersions = cache.GetAll(key.Key, key.Version)

                let result =
                    match msg, cached with
                    | GetOrCompute _, Some(Completed(result, diags)) ->
                        Interlocked.Increment &hits |> ignore
                        diags |> replayDiagnostics diagnosticLogger
                        Existing(Task.FromResult result)
                    | GetOrCompute(_, ct), Some(Running(tcs, _, _, _, loggers)) ->
                        Interlocked.Increment &hits |> ignore
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            Interlocked.Increment &cancel_ct_registration_subsequent |> ignore
                            post (key, CancelRequest))
                        |> saveRegistration key

                        loggers.Add diagnosticLogger

                        Existing tcs.Task

                    | GetOrCompute(computation, ct), None
                    | GetOrCompute(computation, ct), Some(Job.Canceled _)
                    | GetOrCompute(computation, ct), Some(Job.Failed _) ->
                        Interlocked.Increment &started |> ignore
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            Interlocked.Increment &cancel_ct_registration_original |> ignore
                            post (key, OriginatorCanceled))
                        |> saveRegistration key

                        let cts = new CancellationTokenSource()

                        cache.Set(
                            key.Key,
                            key.Version,
                            key.Label,
                            (Running(TaskCompletionSource(), cts, computation, DateTime.Now, ResizeArray()))
                        )

                        otherVersions
                        |> Seq.choose (function
                            | v, Running(_tcs, cts, _, _, _) -> Some(v, cts)
                            | _ -> None)
                        |> Seq.iter (fun (_v, cts) ->
                            use _ = Activity.start $"{name}: Duplicate running job" [| "key", key.Label |]
                            //System.Diagnostics.Trace.TraceWarning($"{name} Duplicate {key.Label}")
                            if cancelDuplicateRunningJobs then
                                //System.Diagnostics.Trace.TraceWarning("Canceling")
                                cts.Cancel())

                        New cts.Token

                log (Requested, key)
                return result
            })

    let internalError key message =
        let ex = exn (message)
        failures.Add(key, ex)
        Interlocked.Increment &errors |> ignore
    // raise ex -- Suppose there's no need to raise here - where does it even go?

    let processStateUpdate post (key: KeyData<_, _>, action: StateUpdate<_>) =
        task {
            do! Task.Delay 0

            do!
                lock.Do(fun () ->
                    task {

                        let cached = cache.TryGet(key.Key, key.Version)

                        match action, cached with

                        | OriginatorCanceled, Some(Running(tcs, cts, computation, _, _)) ->

                            Interlocked.Increment &cancel_original_processed |> ignore

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                // Remember the job in case it completes after cancellation
                                cache.Set(key.Key, key.Version, key.Label, Job.Canceled DateTime.Now)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)
                                Interlocked.Increment &canceled |> ignore
                                use _ = Activity.start $"{name}: Canceled job" [| "key", key.Label |]
                                ()

                            else
                                // We need to restart the computation
                                Task.Run(fun () ->
                                    Async.StartAsTask(
                                        async {

                                            let cachingLogger = new CachingDiagnosticsLogger(None)

                                            try
                                                // TODO: Should unify starting and restarting
                                                log (Restarted, key)
                                                Interlocked.Increment &restarted |> ignore
                                                System.Diagnostics.Trace.TraceInformation $"{name} Restarted {key.Label}"
                                                let currentLogger = DiagnosticsThreadStatics.DiagnosticsLogger
                                                DiagnosticsThreadStatics.DiagnosticsLogger <- cachingLogger

                                                try
                                                    let! result = computation |> Async.AwaitNodeCode
                                                    post (key, (JobCompleted(result, cachingLogger.CapturedDiagnostics)))
                                                    return ()
                                                finally
                                                    DiagnosticsThreadStatics.DiagnosticsLogger <- currentLogger
                                            with
                                            | TaskCancelled _ ->
                                                Interlocked.Increment &cancel_exception_subsequent |> ignore
                                                post (key, CancelRequest)
                                                ()
                                            | ex -> post (key, (JobFailed(ex, cachingLogger.CapturedDiagnostics)))
                                        }
                                    ),
                                    cts.Token)
                                |> ignore

                        | CancelRequest, Some(Running(tcs, cts, _c, _, _)) ->

                            Interlocked.Increment &cancel_subsequent_processed |> ignore

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                // Remember the job in case it completes after cancellation
                                cache.Set(key.Key, key.Version, key.Label, Job.Canceled DateTime.Now)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)
                                Interlocked.Increment &canceled |> ignore
                                use _ = Activity.start $"{name}: Canceled job" [| "key", key.Label |]
                                ()

                        // Probably in some cases cancellation can be fired off even after we just unregistered it
                        | CancelRequest, None
                        | CancelRequest, Some(Completed _)
                        | CancelRequest, Some(Job.Canceled _)
                        | CancelRequest, Some(Job.Failed _)
                        | OriginatorCanceled, None
                        | OriginatorCanceled, Some(Completed _)
                        | OriginatorCanceled, Some(Job.Canceled _)
                        | OriginatorCanceled, Some(Job.Failed _) -> ()

                        | JobFailed(ex, diags), Some(Running(tcs, _cts, _c, _ts, loggers)) ->
                            cancelRegistration key
                            cache.Set(key.Key, key.Version, key.Label, Job.Failed(DateTime.Now, ex))
                            requestCounts.Remove key |> ignore
                            log (Failed, key)
                            Interlocked.Increment &failed |> ignore
                            failures.Add(key.Label, ex)

                            for logger in loggers do
                                diags |> replayDiagnostics logger

                            tcs.TrySetException ex |> ignore

                        | JobCompleted(result, diags), Some(Running(tcs, _cts, _c, started, loggers)) ->
                            cancelRegistration key
                            cache.Set(key.Key, key.Version, key.Label, (Completed(result, diags)))
                            requestCounts.Remove key |> ignore
                            log (Finished, key)
                            Interlocked.Increment &completed |> ignore
                            let duration = float (DateTime.Now - started).Milliseconds

                            avgDurationMs <-
                                if completed < 2 then
                                    duration
                                else
                                    avgDurationMs + (duration - avgDurationMs) / float completed

                            for logger in loggers do
                                diags |> replayDiagnostics logger

                            if tcs.TrySetResult result = false then
                                internalError key.Label "Invalid state: Completed job already completed"

                        // Sometimes job can be canceled but it still manages to complete (or fail)
                        | JobFailed _, Some(Job.Canceled _)
                        | JobCompleted _, Some(Job.Canceled _) -> ()

                        // Job can't be evicted from cache while it's running because then subsequent requesters would be waiting forever
                        | JobFailed _, None -> internalError key.Label "Invalid state: Running job missing in cache (failed)"

                        | JobCompleted _, None -> internalError key.Label "Invalid state: Running job missing in cache (completed)"

                        | JobFailed(ex, _diags), Some(Completed(_job, _diags2)) ->
                            internalError key.Label $"Invalid state: Failed Completed job \n%A{ex}"

                        | JobCompleted(_result, _diags), Some(Completed(_job, _diags2)) ->
                            internalError key.Label "Invalid state: Double-Completed job"

                        | JobFailed(ex, _diags), Some(Job.Failed(_, ex2)) ->
                            internalError key.Label $"Invalid state: Double-Failed job \n%A{ex} \n%A{ex2}"

                        | JobCompleted(_result, _diags), Some(Job.Failed(_, ex2)) ->
                            internalError key.Label $"Invalid state: Completed Failed job \n%A{ex2}"
                    })
        }

    let rec post msg =
        Task.Run(fun () -> processStateUpdate post msg :> Task) |> ignore

    member this.Get'(key, computation) =

        let wrappedKey =
            { new ICacheKey<_, _> with
                member _.GetKey() = key
                member _.GetVersion() = Unchecked.defaultof<_>
                member _.GetLabel() = key.ToString()
            }

        this.Get(wrappedKey, computation)

    member _.Get(key: ICacheKey<_, _>, computation) =

        let key =
            {
                Label = key.GetLabel()
                Key = key.GetKey()
                Version = key.GetVersion()
            }

        node {
            let! ct = NodeCode.CancellationToken

            let callerDiagnosticLogger = DiagnosticsThreadStatics.DiagnosticsLogger

            match!
                processRequest post (key, GetOrCompute(computation, ct)) callerDiagnosticLogger
                |> NodeCode.AwaitTask
            with
            | New internalCt ->

                let linkedCtSource = CancellationTokenSource.CreateLinkedTokenSource(ct, internalCt)
                let cachingLogger = new CachingDiagnosticsLogger(Some callerDiagnosticLogger)

                try
                    return!
                        Async.StartAsTask(
                            async {
                                // TODO: Should unify starting and restarting
                                let currentLogger = DiagnosticsThreadStatics.DiagnosticsLogger
                                DiagnosticsThreadStatics.DiagnosticsLogger <- cachingLogger

                                log (Started, key)

                                try
                                    let! result = computation |> Async.AwaitNodeCode
                                    post (key, (JobCompleted(result, cachingLogger.CapturedDiagnostics)))
                                    return result
                                finally
                                    DiagnosticsThreadStatics.DiagnosticsLogger <- currentLogger
                            },
                            cancellationToken = linkedCtSource.Token
                        )
                        |> NodeCode.AwaitTask
                with
                | TaskCancelled ex ->
                    // TODO: do we need to do anything else here? Presumably it should be done by the registration on
                    // the cancellation token or before we triggered our own cancellation

                    // Let's send this again just in case. It seems sometimes it's not triggered from the registration?

                    Interlocked.Increment &cancel_exception_original |> ignore

                    post (key, (OriginatorCanceled))
                    return raise ex
                | ex ->
                    post (key, (JobFailed(ex, cachingLogger.CapturedDiagnostics)))
                    return raise ex

            | Existing job -> return! job |> NodeCode.AwaitTask

        }

    member _.TryGet(key: 'TKey, predicate: 'TVersion -> bool) : 'TValue option =
        let versionsAndJobs = cache.GetAll(key)

        versionsAndJobs
        |> Seq.tryPick (fun (version, job) ->
            match predicate version, job with
            | true, Completed(completed, _) -> Some completed
            | _ -> None)

    member _.Clear() = cache.Clear()

    member _.Clear predicate = cache.Clear predicate

    member val Event = event.Publish

    member this.OnEvent = this.Event.Add

    member this.Count = cache.Count

    member _.Locked = lock.Semaphore.CurrentCount < 1

    member _.Running =
        cache.GetValues()
        |> Seq.filter (function
            | _, _, Running _ -> true
            | _ -> false)
        |> Seq.toArray

    member this.DebuggerDisplay =
        let locked = if this.Locked then " [LOCKED]" else ""

        let valueStats =
            cache.GetValues()
            |> Seq.countBy (function
                | _, _, Running _ -> "Running"
                | _, _, Completed _ -> "Completed"
                | _, _, Job.Canceled _ -> "Canceled"
                | _, _, Job.Failed _ -> "Failed")
            |> Map

        let running =
            valueStats.TryFind "Running"
            |> Option.map (sprintf " Running: %d ")
            |> Option.defaultValue ""

        let avgDuration = avgDurationMs |> sprintf "| Avg: %.0f ms"

        let hitRatio =
            if started > 0 then
                $" (%.0f{float hits / (float (started + hits)) * 100.0} %%)"
            else
                ""

        let stats =
            [|
                if errors + failed > 0 then
                    " (_!_) "
                if errors > 0 then $"| ERRORS: {errors} " else ""
                if failed > 0 then $"| FAILED: {failed} " else ""
                $"| hits: {hits}{hitRatio} "
                if started > 0 then $"| started: {started} " else ""
                if completed > 0 then $"| completed: {completed} " else ""
                if canceled > 0 then $"| canceled: {canceled} " else ""
                if restarted > 0 then $"| restarted: {restarted} " else ""
                if evicted > 0 then $"| evicted: {evicted} " else ""
                if collected > 0 then $"| collected: {collected} " else ""
                if cleared > 0 then $"| cleared: {cleared} " else ""
                if strengthened > 0 then
                    $"| strengthened: {strengthened} "
                else
                    ""
            |]
            |> String.concat ""

        $"{locked}{running}{cache.DebuggerDisplay} {stats}{avgDuration}"

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
