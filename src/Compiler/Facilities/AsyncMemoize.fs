namespace Internal.Utilities.Collections

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler.Diagnostics

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

type internal StateUpdate<'TValue> =
    | CancelRequest
    | OriginatorCanceled
    | JobCompleted of 'TValue
    | JobFailed of exn

type internal MemoizeReply<'TValue> =
    | New of CancellationToken
    | Existing of Task<'TValue>

type internal MemoizeRequest<'TValue> =
    | GetOrCompute of Async<'TValue> * CancellationToken
    | Sync

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal Job<'TValue> =
    | Running of TaskCompletionSource<'TValue> * CancellationTokenSource * Async<'TValue> * DateTime
    | Completed of 'TValue

    member this.DebuggerDisplay =
        match this with
        | Running(_, cts, _, ts) ->
            let cancellation =
                if cts.IsCancellationRequested then
                    " ! Cancellation Requested"
                else
                    ""

            $"Running since {ts.ToShortTimeString()}{cancellation}"
        | Completed value -> $"Completed {value}"

type internal JobEvent =
    | Started
    | Finished
    | Canceled
    | Evicted
    | Collected
    | Weakened
    | Strengthened
    | Failed

type internal ICacheKey<'TKey, 'TVersion> =
    abstract member GetKey: unit -> 'TKey
    abstract member GetVersion: unit -> 'TVersion
    abstract member GetLabel: unit -> string

type private KeyData<'TKey, 'TVersion> =
    {
        Label: string
        Key: 'TKey
        Version: 'TVersion
    }

/// Tools for hashing things with MD5 into a string that can be used as a cache key.
module internal Md5Hasher =

    let private md5 =
        new ThreadLocal<_>(fun () -> System.Security.Cryptography.MD5.Create())

    let private computeHash (bytes: byte array) = md5.Value.ComputeHash(bytes)

    let hashString (s: string) =
        System.Text.Encoding.UTF8.GetBytes(s) |> computeHash

    let empty = String.Empty

    let addBytes (bytes: byte array) (s: string) =
        let sbytes = s |> hashString

        Array.append sbytes bytes
        |> computeHash
        |> System.BitConverter.ToString
        |> (fun x -> x.Replace("-", ""))

    let addString (s: string) (s2: string) =
        s |> System.Text.Encoding.UTF8.GetBytes |> addBytes <| s2

    let addSeq<'item> (items: 'item seq) (addItem: 'item -> string -> string) (s: string) =
        items |> Seq.fold (fun s a -> addItem a s) s

    let addStrings strings = addSeq strings addString

    let addVersions<'a, 'b when 'a :> ICacheKey<'b, string>> (versions: 'a seq) (s: string) =
        versions |> Seq.map (fun x -> x.GetVersion()) |> addStrings <| s

    let addBool (b: bool) (s: string) =
        b |> BitConverter.GetBytes |> addBytes <| s

    let addDateTime (dt: System.DateTime) (s: string) = dt.Ticks.ToString() |> addString <| s

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

    let failures = ResizeArray()
    let mutable avgDurationMs = 0.0

    let cache =
        LruCache<'TKey, 'TVersion, Job<'TValue>>(
            keepStrongly = defaultArg keepStrongly 100,
            keepWeakly = defaultArg keepWeakly 200,
            requiredToKeep =
                (function
                | Running _ -> true
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
                        event.Trigger(JobEvent.Strengthened, k)))
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

    let processRequest post (key: KeyData<_, _>, msg) =

        lock.Do(fun () ->
            task {

                let cached, otherVersions = cache.GetAll(key.Key, key.Version)

                return
                    match msg, cached with
                    | Sync, _ -> New Unchecked.defaultof<_>
                    | GetOrCompute _, Some(Completed result) ->
                        Interlocked.Increment &hits |> ignore
                        Existing(Task.FromResult result)
                    | GetOrCompute(_, ct), Some(Running(tcs, _, _, _)) ->
                        Interlocked.Increment &hits |> ignore
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            post (key, CancelRequest))
                        |> saveRegistration key

                        Existing tcs.Task

                    | GetOrCompute(computation, ct), None ->
                        Interlocked.Increment &started |> ignore
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            post (key, OriginatorCanceled))
                        |> saveRegistration key

                        let cts = new CancellationTokenSource()

                        cache.Set(key.Key, key.Version, key.Label, (Running(TaskCompletionSource(), cts, computation, DateTime.Now)))

                        otherVersions
                        |> Seq.choose (function
                            | v, Running(_tcs, cts, _, _) -> Some(v, cts)
                            | _ -> None)
                        |> Seq.iter (fun (_v, cts) ->
                            use _ = Activity.start $"{name}: Duplicate running job" [| "key", key.Label |]
                            //System.Diagnostics.Trace.TraceWarning($"{name} Duplicate {key.Label}")
                            if cancelDuplicateRunningJobs then
                                //System.Diagnostics.Trace.TraceWarning("Canceling")
                                cts.Cancel())

                        New cts.Token
            })

    let internalError key message =
        let ex = exn (message)
        failures.Add(key, ex)
        Interlocked.Increment &errors |> ignore
        raise ex

    let processStateUpdate post (key: KeyData<_, _>, action: StateUpdate<_>) =
        task {
            do! Task.Delay 0

            do!
                lock.Do(fun () ->
                    task {

                        let cached = cache.TryGet(key.Key, key.Version)

                        match action, cached with

                        | OriginatorCanceled, Some(Running(tcs, cts, computation, _)) ->

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                cache.Remove(key.Key, key.Version)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)
                                Interlocked.Increment &canceled |> ignore
                                use _ = Activity.start $"{name}: Canceled job" [| "key", key.Label |]
                                ()
                            //System.Diagnostics.Trace.TraceInformation $"{name} Canceled {key.Label}"

                            else
                                // We need to restart the computation
                                Task.Run(fun () ->
                                    task {
                                        do! Task.Delay 0

                                        try
                                            log (Started, key)
                                            Interlocked.Increment &restarted |> ignore
                                            System.Diagnostics.Trace.TraceInformation $"{name} Restarted {key.Label}"
                                            let! result = Async.StartAsTask(computation, cancellationToken = cts.Token)
                                            post (key, (JobCompleted result))
                                        with
                                        | :? OperationCanceledException ->
                                            post (key, CancelRequest)
                                            ()
                                        | ex -> post (key, (JobFailed ex))
                                    },
                                    cts.Token)
                                |> ignore

                        | CancelRequest, Some(Running(tcs, cts, _c, _)) ->

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                cache.Remove(key.Key, key.Version)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)
                                Interlocked.Increment &canceled |> ignore
                                use _ = Activity.start $"{name}: Canceled job" [| "key", key.Label |]
                                ()
                        //System.Diagnostics.Trace.TraceInformation $"{name} Canceled {key.Label}"

                        // Probably in some cases cancellation can be fired off even after we just unregistered it
                        | CancelRequest, None
                        | CancelRequest, Some(Completed _)
                        | OriginatorCanceled, None
                        | OriginatorCanceled, Some(Completed _) -> ()

                        | JobFailed ex, Some(Running(tcs, _cts, _c, _ts)) ->
                            cancelRegistration key
                            cache.Remove(key.Key, key.Version)
                            requestCounts.Remove key |> ignore
                            log (Failed, key)
                            Interlocked.Increment &failed |> ignore
                            failures.Add(key.Label, ex)
                            tcs.TrySetException ex |> ignore

                        | JobCompleted result, Some(Running(tcs, _cts, _c, started)) ->
                            cancelRegistration key
                            cache.Set(key.Key, key.Version, key.Label, (Completed result))
                            requestCounts.Remove key |> ignore
                            log (Finished, key)
                            Interlocked.Increment &completed |> ignore
                            let duration = float (DateTime.Now - started).Milliseconds

                            avgDurationMs <-
                                if completed < 2 then
                                    duration
                                else
                                    avgDurationMs + (duration - avgDurationMs) / float completed

                            if tcs.TrySetResult result = false then
                                internalError key.Label "Invalid state: Completed job already completed"

                        // Job can't be evicted from cache while it's running because then subsequent requesters would be waiting forever
                        | JobFailed _, None -> internalError key.Label "Invalid state: Running job missing in cache (failed)"

                        | JobCompleted _, None -> internalError key.Label "Invalid state: Running job missing in cache (completed)"

                        | JobFailed ex, Some(Completed _job) -> internalError key.Label $"Invalid state: Failed Completed job \n%A{ex}"

                        | JobCompleted _result, Some(Completed _job) -> internalError key.Label "Invalid state: Double-Completed job"
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

        async {
            let! ct = Async.CancellationToken

            match! processRequest post (key, GetOrCompute(computation, ct)) |> Async.AwaitTask with
            | New internalCt ->

                let linkedCtSource = CancellationTokenSource.CreateLinkedTokenSource(ct, internalCt)

                try
                    return!
                        Async.StartAsTask(
                            async {
                                log (Started, key)
                                let! result = computation
                                post (key, (JobCompleted result))
                                return result
                            },
                            cancellationToken = linkedCtSource.Token
                        )
                        |> Async.AwaitTask
                with
                | :? TaskCanceledException
                | :? OperationCanceledException as ex -> return raise ex
                | ex ->
                    post (key, (JobFailed ex))
                    return raise ex

            | Existing job -> return! job |> Async.AwaitTask
        }

    member _.Clear() = cache.Clear()

    member val Event = event.Publish

    member this.OnEvent = this.Event.Add

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
                | _, _, Completed _ -> "Completed")
            |> Map

        let running =
            valueStats.TryFind "Running"
            |> Option.map (sprintf " Running: %d")
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
                    " [!] "
                if errors > 0 then $"| ERRORS: {errors} " else ""
                if failed > 0 then $"| FAILED: {failed} " else ""
                $"| hits: {hits}{hitRatio} "
                if started > 0 then $"| started: {started} " else ""
                if completed > 0 then $"| completed: {completed} " else ""
                if canceled > 0 then $"| canceled: {canceled} " else ""
                if restarted > 0 then $"| restarted: {restarted} " else ""
                if evicted > 0 then $"| evicted: {evicted} " else ""
                if collected > 0 then $"| collected: {collected} " else ""
                if strengthened > 0 then
                    $"| strengthened: {strengthened} "
                else
                    ""
            |]
            |> String.concat ""

        $"{locked}{running} {cache.DebuggerDisplay} {stats}{avgDuration}"

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal AsyncMemoizeDisabled<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality>
    (?keepStrongly, ?keepWeakly, ?name: string, ?cancelDuplicateRunningJobs: bool) =

    do ignore (keepStrongly, keepWeakly, name, cancelDuplicateRunningJobs)

    let mutable requests = 0

    member _.Get(_key: ICacheKey<_, _>, computation) =
        Interlocked.Increment &requests |> ignore
        computation

    member _.DebuggerDisplay = $"(disabled) requests: {requests}"
