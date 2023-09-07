namespace Internal.Utilities.Collections

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks

[<AutoOpen>]
module Utils =

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
    | New
    | Existing of Task<'TValue>

type MemoizeRequest<'TValue> =
    | GetOrCompute of Async<'TValue> * CancellationToken
    | Sync

type internal Job<'TValue> =
    | Running of TaskCompletionSource<'TValue> * CancellationTokenSource * Async<'TValue>
    | Completed of 'TValue

type internal CacheEvent =
    | Evicted
    | Collected
    | Weakened
    | Strengthened

type internal JobEvent =
    | Started
    | Finished
    | Canceled
    | Evicted
    | Collected
    | Weakened
    | Strengthened
    | Failed

[<StructuralEquality; NoComparison>]
type internal ValueLink<'T when 'T: not struct> =
    | Strong of 'T
    | Weak of WeakReference<'T>

[<DebuggerDisplay("{DebuggerDisplay}")>]
type internal LruCache<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality and 'TValue: not struct>
    (
        keepStrongly,
        ?keepWeakly,
        ?requiredToKeep,
        ?event
    ) =

    let keepWeakly = defaultArg keepWeakly 100
    let requiredToKeep = defaultArg requiredToKeep (fun _ -> false)
    let event = defaultArg event (fun _ _ -> ())

    let dictionary = Dictionary<'TKey, Dictionary<'TVersion, _>>()

    // Lists to keep track of when items were last accessed. First item is most recently accessed.
    let strongList = LinkedList<'TKey * 'TVersion * string * ValueLink<'TValue>>()
    let weakList = LinkedList<'TKey * 'TVersion * string * ValueLink<'TValue>>()

    let rec removeCollected (node: LinkedListNode<_>) =
        if node <> null then
            let key, version, label, value = node.Value

            match value with
            | Weak w ->
                let next = node.Next

                match w.TryGetTarget() with
                | false, _ ->
                    weakList.Remove node
                    dictionary[ key ].Remove version |> ignore

                    if dictionary[key].Count = 0 then
                        dictionary.Remove key |> ignore

                    event CacheEvent.Collected (label, key, version)
                | _ -> ()

                removeCollected next
            | _ -> failwith "Illegal state, strong reference in weak list"

    let cutWeakListIfTooLong () =
        if weakList.Count > keepWeakly then
            removeCollected weakList.First

            let mutable node = weakList.Last

            while weakList.Count > keepWeakly && node <> null do
                let previous = node.Previous
                let key, version, label, _ = node.Value
                weakList.Remove node
                dictionary[ key ].Remove version |> ignore

                if dictionary[key].Count = 0 then
                    dictionary.Remove key |> ignore

                event CacheEvent.Evicted (label, key, version)
                node <- previous

    let cutStrongListIfTooLong () =
        let mutable node = strongList.Last

        while strongList.Count > keepStrongly && node <> null do
            let previous = node.Previous

            match node.Value with
            | _, _, _, Strong v when requiredToKeep v -> ()
            | key, version, label, Strong v ->
                strongList.Remove node
                node.Value <- key, version, label, Weak(WeakReference<_> v)
                weakList.AddFirst node
                event CacheEvent.Weakened (label, key, version)
            | _key, _version, _label, _ -> failwith "Invalid state, weak reference in strong list"

            node <- previous

        cutWeakListIfTooLong ()

    let pushNodeToTop (node: LinkedListNode<_>) =
        match node.Value with
        | _, _, _, Strong _ ->
            strongList.AddFirst node
            cutStrongListIfTooLong ()
        | _, _, _, Weak _ -> failwith "Invalid operation, pushing weak reference to strong list"

    let pushValueToTop key version label value =
        let node = strongList.AddFirst(value = (key, version, label, Strong value))
        cutStrongListIfTooLong ()
        node

    member _.DebuggerDisplay = $"Cache(S:{strongList.Count} W:{weakList.Count})"

    member _.Set(key, version, label, value) =
        match dictionary.TryGetValue key with
        | true, versionDict ->

            if versionDict.ContainsKey version then
                // TODO this is normal for unversioned cache;
                // failwith "Suspicious - overwriting existing version"

                let node: LinkedListNode<_> = versionDict[version]

                match node.Value with
                | _, _, _, Strong _ -> strongList.Remove node
                | _, _, _, Weak _ ->
                    weakList.Remove node
                    event CacheEvent.Strengthened (label, key, version)

                node.Value <- key, version, label, Strong value
                pushNodeToTop node

            else
                let node = pushValueToTop key version label value
                versionDict[version] <- node
                // weaken all other versions (unless they're required to be kept)
                for otherVersion in versionDict.Keys do
                    if otherVersion <> version then
                        let node = versionDict[otherVersion]

                        match node.Value with
                        | _, _, _, Strong value when not (requiredToKeep value) ->
                            strongList.Remove node
                            node.Value <- key, otherVersion, label, Weak(WeakReference<_> value)
                            weakList.AddFirst node
                            event CacheEvent.Weakened (label, key, otherVersion)
                            cutWeakListIfTooLong ()
                        | _ -> ()

        | false, _ ->
            let node = pushValueToTop key version label value
            dictionary[key] <- Dictionary()
            dictionary[key][version] <- node

    member this.Set(key, version, value) =
        this.Set(key, version, "[no label]", value)

    member _.TryGet(key, version) =

        match dictionary.TryGetValue key with
        | false, _ -> None
        | true, versionDict ->
            match versionDict.TryGetValue version with
            | false, _ -> None
            | true, node ->
                match node.Value with
                | _, _, _, Strong v ->
                    strongList.Remove node
                    pushNodeToTop node
                    Some v

                | _, _, label, Weak w ->
                    match w.TryGetTarget() with
                    | true, value ->
                        weakList.Remove node
                        let node = pushValueToTop key version label value
                        event CacheEvent.Strengthened (label, key, version)
                        versionDict[version] <- node
                        Some value
                    | _ ->
                        weakList.Remove node
                        versionDict.Remove version |> ignore

                        if versionDict.Count = 0 then
                            dictionary.Remove key |> ignore

                        event CacheEvent.Collected (label, key, version)
                        None

    member _.Remove(key, version) =
        match dictionary.TryGetValue key with
        | false, _ -> ()
        | true, versionDict ->
            match versionDict.TryGetValue version with
            | true, node ->
                versionDict.Remove version |> ignore

                if versionDict.Count = 0 then
                    dictionary.Remove key |> ignore

                match node.Value with
                | _, _, _, Strong _ -> strongList.Remove node
                | _, _, _, Weak _ -> weakList.Remove node
            | _ -> ()

    member this.Set(key, value) =
        this.Set(key, Unchecked.defaultof<_>, value)

    member this.TryGet(key) =
        this.TryGet(key, Unchecked.defaultof<_>)

    member this.Remove(key) =
        this.Remove(key, Unchecked.defaultof<_>)

    member _.GetValues() =
        strongList
        |> Seq.append weakList
        |> Seq.choose (function
            | _, _, _, Strong v -> Some v
            | _, _, _, Weak w ->
                match w.TryGetTarget() with
                | true, v -> Some v
                | _ -> None)

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

    let empty = String.Empty

    let addBytes (bytes: byte array) (s: string) =
        let sbytes = System.Text.Encoding.UTF8.GetBytes(s)

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

type AsyncLock() =

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
    (
        ?keepStrongly,
        ?keepWeakly,
        ?name: string
    ) =

    let name = defaultArg name "N/A"

    let event = Event<_>()

    let cache =
        LruCache<'TKey, 'TVersion, Job<'TValue>>(
            keepStrongly = defaultArg keepStrongly 100,
            keepWeakly = defaultArg keepWeakly 100,
            requiredToKeep =
                (function
                | Running _ -> true
                | _ -> false),
            event =
                (function
                | CacheEvent.Evicted -> (fun k -> event.Trigger(JobEvent.Evicted, k))
                | CacheEvent.Collected -> (fun k -> event.Trigger(JobEvent.Collected, k))
                | CacheEvent.Weakened -> (fun k -> event.Trigger(JobEvent.Weakened, k))
                | CacheEvent.Strengthened -> (fun k -> event.Trigger(JobEvent.Strengthened, k)))
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

                let cached = cache.TryGet(key.Key, key.Version)

                return
                    match msg, cached with
                    | Sync, _ -> New
                    | GetOrCompute _, Some (Completed result) -> Existing(Task.FromResult result)
                    | GetOrCompute (_, ct), Some (Running (tcs, _, _)) ->
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            post (key, CancelRequest))
                        |> saveRegistration key

                        Existing tcs.Task

                    | GetOrCompute (computation, ct), None ->
                        incrRequestCount key

                        ct.Register(fun _ ->
                            let _name = name
                            post (key, OriginatorCanceled))
                        |> saveRegistration key

                        cache.Set(
                            key.Key,
                            key.Version,
                            key.Label,
                            (Running(TaskCompletionSource(), (new CancellationTokenSource()), computation))
                        )

                        New
            })

    let processStateUpdate post (key: KeyData<_, _>, action: StateUpdate<_>) =
        task {
            do! Task.Delay 0

            do!
                lock.Do(fun () ->
                    task {

                        let cached = cache.TryGet(key.Key, key.Version)

                        match action, cached with

                        | OriginatorCanceled, Some (Running (tcs, cts, computation)) ->

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                cache.Remove(key.Key, key.Version)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)

                            else
                                // We need to restart the computation
                                Task.Run(fun () ->
                                    task {
                                        do! Task.Delay 0

                                        try
                                            log (Started, key)
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

                        | CancelRequest, Some (Running (tcs, cts, _c)) ->

                            decrRequestCount key

                            if requestCounts[key] < 1 then
                                cancelRegistration key
                                cts.Cancel()
                                tcs.TrySetCanceled() |> ignore
                                cache.Remove(key.Key, key.Version)
                                requestCounts.Remove key |> ignore
                                log (Canceled, key)

                        | CancelRequest, None
                        | CancelRequest, Some (Completed _) -> ()

                        | JobFailed ex, Some (Running (tcs, _cts, _c)) ->
                            cancelRegistration key
                            cache.Remove(key.Key, key.Version)
                            requestCounts.Remove key |> ignore
                            log (Failed, key)
                            tcs.TrySetException ex |> ignore

                        | JobCompleted result, Some (Running (tcs, _cts, _c)) ->
                            cancelRegistration key
                            cache.Set(key.Key, key.Version, key.Label, (Completed result))
                            requestCounts.Remove key |> ignore
                            log (Finished, key)

                            if tcs.TrySetResult result = false then
                                failwith "Invalid state: Completed job already completed"
                                ()

                        // Job can't be evicted from cache while it's running because then subsequent requesters would be waiting forever
                        | JobFailed _, None -> failwith "Invalid state: Running job missing in cache (failed)"

                        | OriginatorCanceled, None -> failwith "Invalid state: Running job missing in cache (canceled)"

                        | JobCompleted _, None -> failwith "Invalid state: Running job missing in cache (completed)"

                        | JobFailed ex, Some (Completed _job) ->
                            failwith $"Invalid state: Failed Completed job \n%A{ex}"
                            ignore ex

                        | JobCompleted _result, Some (Completed _job) ->
                            failwith "Invalid state: Double-Completed job"
                            ()

                        | OriginatorCanceled, Some (Completed _result) ->
                            failwith "Invalid state: Canceled Completed job"
                            ()
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
            | New ->
                try
                    return!
                        Async.StartAsTask(
                            async {
                                log (Started, key)
                                let! result = computation
                                post (key, (JobCompleted result))
                                return result
                            },
                            cancellationToken = ct
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

    member val Event = event.Publish

    member this.OnEvent = this.Event.Add

    member _.Locked = lock.Semaphore.CurrentCount < 1

    member _.Running = cache.GetValues() |> Seq.filter (function Running _ -> true | _ -> false) |> Seq.toArray

    member this.DebuggerDisplay =
        let locked = if this.Locked then " [LOCKED]" else ""

        let valueStats =
            cache.GetValues()
            |> Seq.countBy (function
                | Running _ -> "Running"
                | Completed _ -> "Completed")
            |> Seq.map ((<||) (sprintf "%s: %d"))
            |> String.concat " "

        $"{name}{locked} {valueStats} ({cache.DebuggerDisplay})"
