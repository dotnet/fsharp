// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.VisualStudio.FSharp.Editor.DebugHelpers

open CancellableTasks

// Push-based file watching for FSharp.Editor, modelled on Roslyn's
// Microsoft.VisualStudio.LanguageServices FileChangeWatcher (which is internal and not
// exposed through ExternalAccess.FSharp). Uses the free-threaded IVsAsyncFileChangeEx2
// service: subscriptions are batched off the UI thread and callbacks never marshal to it.

/// A directory to watch recursively (with optional extension filters) so that individual
/// files under it don't each need their own advise cookie.
[<Sealed>]
type internal WatchedDirectory(path: string, extensionFilters: ImmutableArray<string>) =
    let path =
        if path.EndsWith(string IO.Path.DirectorySeparatorChar, StringComparison.Ordinal) then
            path
        else
            $"{path}{IO.Path.DirectorySeparatorChar}"

    do
        for filter in extensionFilters do
            if not (filter.StartsWith(".", StringComparison.Ordinal)) then
                invalidArg (nameof extensionFilters) $"Filter '{filter}' must start with a period."

    member _.Path = path
    member _.ExtensionFilters = extensionFilters

    static member FilePathCoveredByWatchedDirectories(watchedDirectories: ImmutableArray<WatchedDirectory>, filePath: string) =
        watchedDirectories
        |> Seq.exists (fun w ->
            filePath.StartsWith(w.Path, StringComparison.OrdinalIgnoreCase)
            && (w.ExtensionFilters.IsEmpty
                || w.ExtensionFilters
                   |> Seq.exists (fun filter -> filePath.EndsWith(filter, StringComparison.OrdinalIgnoreCase))))

/// A single watched file; disposing stops watching.
type internal IFSharpWatchedFile =
    inherit IDisposable

/// A group of file/directory watches sharing one event sink. Disposing unsubscribes everything.
type internal IFSharpFileChangeContext =
    inherit IDisposable

    [<CLIEvent>]
    abstract FileChanged: IEvent<string>

    /// Starts watching a file without waiting for the OS registration. No-op (but still valid
    /// to dispose) when the path is already covered by one of the context's watched directories.
    abstract EnqueueWatchingFile: filePath: string -> IFSharpWatchedFile

type internal IFSharpFileChangeWatcher =
    abstract CreateContext: watchedDirectories: ImmutableArray<WatchedDirectory> -> IFSharpFileChangeContext

/// Last-write stamps of watched reference files; a path nobody watches is stat'd directly.
type internal IReferenceStamps =
    abstract GetLastWriteTimeUtc: fullFilePath: string -> DateTime
    abstract Invalidate: fullFilePath: string -> unit

type private WatchedReference =
    {
        Token: IFSharpWatchedFile
        mutable Count: int
        mutable Stamp: DateTime voption
    }

[<AutoOpen>]
module private FileChangeWatcherImpl =

    // Same flags Roslyn uses for both subscribing and filtering callbacks.
    let watchFlags =
        _VSFILECHANGEFLAGS.VSFILECHG_Size ||| _VSFILECHANGEFLAGS.VSFILECHG_Time

    let relevantFlags =
        _VSFILECHANGEFLAGS.VSFILECHG_Time
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Add
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Del
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Size

    /// Empirically strong batching window during high activity (solution open/close); see
    /// Roslyn's FileChangeWatcher.
    let defaultBatchingDelay = TimeSpan.FromMilliseconds 500.

    /// Delay between the last observed change to a path and the callback: a rebuild typically
    /// writes a temp file then renames, producing several rapid notifications.
    let defaultNotificationDelay = TimeSpan.FromSeconds 2.

    let noOpWatchedFile =
        { new IFSharpWatchedFile with
            member _.Dispose() = ()
        }

[<Sealed>]
type internal FSharpWatchedFileToken() =
    member val Cookie: uint32 voption = ValueNone with get, set

/// Subscription operations queued for batched application against the file change service.
type private WatcherOperation =
    | WatchDir of path: string * filters: ImmutableArray<string> * sink: IVsFreeThreadedFileChangeEvents2 * cookies: List<uint32>
    | WatchFiles of paths: string list * tokens: FSharpWatchedFileToken list * sink: IVsFreeThreadedFileChangeEvents2
    | UnwatchFiles of tokens: FSharpWatchedFileToken list
    | UnwatchDirs of cookies: List<uint32>

[<Sealed>]
type internal FSharpFileChangeWatcher(fileChangeService: Task<IVsAsyncFileChangeEx2>, batchingDelay: TimeSpan) =

    let applyBatch (service: IVsAsyncFileChangeEx2) (ops: WatcherOperation list) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()

            // Coalesce adjacent same-kind operations into single service calls, preserving order
            // between kinds (a watch enqueued before an unwatch must be applied first).
            let mutable pending = ops

            while not pending.IsEmpty do
                match pending with
                | [] -> ()
                | WatchDir(path, filters, sink, cookies) :: rest ->
                    pending <- rest
                    let! cookie = service.AdviseDirChangeAsync(path, true, sink, ct)
                    cookies.Add cookie

                    if not filters.IsEmpty then
                        do! service.FilterDirectoryChangesAsync(cookie, Seq.toArray filters, ct)

                | WatchFiles(_, _, sink) :: _ ->
                    let batch =
                        pending
                        |> Seq.takeWhile (function
                            | WatchFiles(_, _, s) -> obj.ReferenceEquals(s, sink)
                            | _ -> false)
                        |> Seq.toArray

                    pending <- pending |> List.skip batch.Length

                    let paths =
                        [|
                            for op in batch do
                                match op with
                                | WatchFiles(p, _, _) -> yield! p
                                | _ -> ()
                        |]

                    let tokens =
                        [|
                            for op in batch do
                                match op with
                                | WatchFiles(_, t, _) -> yield! t
                                | _ -> ()
                        |]

                    let! cookies = service.AdviseFileChangesAsync(paths, watchFlags, sink, ct)

                    (tokens, cookies)
                    ||> Array.iter2 (fun token cookie -> token.Cookie <- ValueSome cookie)

                | UnwatchFiles _ :: _ ->
                    let batch =
                        pending
                        |> Seq.takeWhile (function
                            | UnwatchFiles _ -> true
                            | _ -> false)
                        |> Seq.toArray

                    pending <- pending |> List.skip batch.Length

                    // A token whose watch never got a cookie (or was already unadvised) is a no-op.
                    let cookies =
                        [|
                            for op in batch do
                                match op with
                                | UnwatchFiles tokens ->
                                    for token in tokens do
                                        match token.Cookie with
                                        | ValueSome cookie ->
                                            token.Cookie <- ValueNone
                                            cookie
                                        | ValueNone -> ()
                                | _ -> ()
                        |]

                    if cookies.Length > 0 then
                        let! _ = service.UnadviseFileChangesAsync(cookies, ct)
                        ()

                | UnwatchDirs cookies :: rest ->
                    pending <- rest

                    if cookies.Count > 0 then
                        let! _ = service.UnadviseDirChangesAsync(cookies.ToArray(), ct)
                        ()
        }

    let cancellationTokenSource = new CancellationTokenSource()

    // Single consumer loop: waits for the first queued operation, sleeps out the batching
    // window, drains the queue and applies everything in one pass. Nothing ever blocks on the
    // service being available.
    let agent =
        MailboxProcessor<WatcherOperation>
            .Start(
                (fun inbox ->
                    async {
                        let! ct = Async.CancellationToken

                        while true do
                            try
                                let! first = inbox.Receive()
                                do! Async.Sleep(int batchingDelay.TotalMilliseconds)

                                let ops = ResizeArray [ first ]

                                while inbox.CurrentQueueLength > 0 do
                                    let! op = inbox.Receive()
                                    ops.Add op

                                let! service = fileChangeService |> Async.AwaitTask

                                do!
                                    applyBatch service (List.ofSeq ops)
                                    |> CancellableTask.startAsTask ct
                                    |> Async.AwaitTask
                            with ex when not (ex :? OperationCanceledException) ->
                                // Never let a failed advise/unadvise (e.g. non-existent path) kill the
                                // subscription loop; we simply won't get events for that path.
                                FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)
                    }),
                cancellationTokenSource.Token
            )

    new(fileChangeService) = new FSharpFileChangeWatcher(fileChangeService, defaultBatchingDelay)

    member private _.Enqueue(op: WatcherOperation) = agent.Post op

    /// Production factory: obtains SVsFileChangeEx asynchronously without blocking any
    /// background thread on UI-thread availability.
    static member CreateDefaultServiceTask() =
        task {
            let! service = AsyncServiceProvider.GlobalProvider.GetServiceAsync(typeof<SVsFileChangeEx>)
            return service :?> IVsAsyncFileChangeEx2
        }

    interface IFSharpFileChangeWatcher with
        member _.CreateContext(watchedDirectories) =
            new FileChangeContext(agent.Post, watchedDirectories) :> IFSharpFileChangeContext

    interface IDisposable with
        member _.Dispose() =
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose()
            agent.Dispose()

and [<Sealed>] private FileChangeContext(enqueue: WatcherOperation -> unit, watchedDirectories: ImmutableArray<WatchedDirectory>) as this =

    let gate = obj ()
    let mutable disposed = false
    let activeFileTokens = HashSet<FSharpWatchedFileToken>()
    let directoryCookies = List<uint32>()
    let fileChanged = Event<string>()

    let raiseChanges (count: uint32) (files: string[]) (changeFlags: uint32[]) =
        for i in 0 .. int count - 1 do
            if
                (enum<_VSFILECHANGEFLAGS> (int changeFlags[i]) &&& relevantFlags)
                <> enum<_VSFILECHANGEFLAGS> 0
            then
                fileChanged.Trigger files[i]

        VSConstants.S_OK

    do
        for watchedDirectory in watchedDirectories do
            enqueue (
                WatchDir(
                    watchedDirectory.Path,
                    watchedDirectory.ExtensionFilters,
                    this :> IVsFreeThreadedFileChangeEvents2,
                    directoryCookies
                )
            )

    member private _.StopWatchingFile(token: FSharpWatchedFileToken) =
        lock gate (fun () -> activeFileTokens.Remove token |> ignore)
        enqueue (UnwatchFiles [ token ])

    interface IFSharpFileChangeContext with
        [<CLIEvent>]
        member _.FileChanged = fileChanged.Publish

        member _.EnqueueWatchingFile filePath =
            if WatchedDirectory.FilePathCoveredByWatchedDirectories(watchedDirectories, filePath) then
                noOpWatchedFile
            else
                let token = FSharpWatchedFileToken()
                lock gate (fun () -> activeFileTokens.Add token |> ignore)
                enqueue (WatchFiles([ filePath ], [ token ], this :> IVsFreeThreadedFileChangeEvents2))

                { new IFSharpWatchedFile with
                    member _.Dispose() = this.StopWatchingFile token
                }

    interface IDisposable with
        member _.Dispose() =
            let alreadyDisposed =
                lock gate (fun () ->
                    let d = disposed
                    disposed <- true
                    d)

            if not alreadyDisposed then
                enqueue (UnwatchDirs directoryCookies)
                enqueue (UnwatchFiles(lock gate (fun () -> List.ofSeq activeFileTokens)))

    // Free-threaded sink: callbacks arrive on background threads and stay there.
    interface IVsFreeThreadedFileChangeEvents2 with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) =
            raiseChanges cChanges rgpszFile rggrfChange

        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL
        member _.DirectoryChangedEx(_, _) = VSConstants.E_NOTIMPL

        member _.DirectoryChangedEx2(_, cChanges, rgpszFile, rggrfChange) =
            raiseChanges cChanges rgpszFile rggrfChange

    interface IVsFreeThreadedFileChangeEvents with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) =
            raiseChanges cChanges rgpszFile rggrfChange

        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL
        member _.DirectoryChangedEx(_, _) = VSConstants.E_NOTIMPL

    interface IVsFileChangeEvents with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) =
            raiseChanges cChanges rgpszFile rggrfChange

        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL

/// Ref-counted, debounced watching of reference assemblies (or any other off-workspace files),
/// modelled on Roslyn's ReferenceFileChangeTracker. Multiple projects watching the same dll
/// share one subscription; bursts of writes produce a single callback per path. The last-write
/// stamp of a path lives inside its watch entry, so a cached stamp is only ever served while a
/// change notification can still reach it.
[<Sealed>]
type internal FSharpReferenceChangeTracker(watcher: IFSharpFileChangeWatcher, onChanged: string -> unit, notificationDelay: TimeSpan) =

    let gate = obj ()
    let mutable disposed = false

    let watchedFiles =
        Dictionary<string, WatchedReference>(StringComparer.OrdinalIgnoreCase)

    let pendingTimers = Dictionary<string, Timer>(StringComparer.OrdinalIgnoreCase)

    // On each platform there is a place framework reference assemblies live; these rarely change
    // but account for most watched paths, so cover them with directory watches up front.
    static let defaultWatchedDirectories () =
        let dotnetRoot = Environment.GetEnvironmentVariable "DOTNET_ROOT"
        let nugetPackages = Environment.GetEnvironmentVariable "NUGET_PACKAGES"

        let directories =
            seq {
                if not (String.IsNullOrEmpty dotnetRoot) then
                    IO.Path.Combine(dotnetRoot, "packs")

                IO.Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.ProgramFiles, "dotnet", "packs")

                IO.Path.Combine(
                    Environment.GetFolderPath Environment.SpecialFolder.ProgramFilesX86,
                    "Reference Assemblies",
                    "Microsoft",
                    "Framework"
                )

                if String.IsNullOrEmpty nugetPackages then
                    IO.Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
                else
                    nugetPackages
            }
            |> Seq.distinct
            |> Seq.map (fun d -> WatchedDirectory(d, ImmutableArray.Create ".dll"))

        directories.ToImmutableArray()

    let context =
        lazy
            (let ctx = watcher.CreateContext(defaultWatchedDirectories ())

             ctx.FileChanged.Add(fun path ->
                 let fire (_: obj) =
                     let isWatched =
                         lock gate (fun () ->
                             match pendingTimers.TryGetValue path with
                             | true, timer ->
                                 pendingTimers.Remove path |> ignore
                                 timer.Dispose()
                             | _ -> ()

                             watchedFiles.ContainsKey path)

                     if isWatched then
                         onChanged path

                 lock gate (fun () ->
                     // Directory watches cover whole trees; only debounce paths someone watches.
                     match watchedFiles.TryGetValue path with
                     | true, entry when not disposed ->
                         entry.Stamp <- ValueNone

                         let timer =
                             match pendingTimers.TryGetValue path with
                             | true, timer -> timer
                             | _ ->
                                 let timer = new Timer(fire, null, Timeout.Infinite, Timeout.Infinite)
                                 pendingTimers[path] <- timer
                                 timer

                         timer.Change(notificationDelay, Timeout.InfiniteTimeSpan) |> ignore
                     | _ -> ()))

             ctx)

    new(watcher, onChanged) = new FSharpReferenceChangeTracker(watcher, onChanged, defaultNotificationDelay)

    /// Starts watching a path, ref-counted. Call StopWatchingReference exactly once per start.
    member _.StartWatchingReference(fullFilePath: string) =
        lock gate (fun () ->
            if not disposed then
                match watchedFiles.TryGetValue fullFilePath with
                | true, entry -> entry.Count <- entry.Count + 1
                | _ ->
                    watchedFiles[fullFilePath] <-
                        {
                            Token = context.Value.EnqueueWatchingFile fullFilePath
                            Count = 1
                            Stamp = ValueNone
                        })

    member _.StopWatchingReference(fullFilePath: string) =
        lock gate (fun () ->
            if not disposed then
                match watchedFiles.TryGetValue fullFilePath with
                | true, { Count = 1; Token = token } ->
                    watchedFiles.Remove fullFilePath |> ignore
                    token.Dispose()
                | true, entry -> entry.Count <- entry.Count - 1
                | _ -> ())

    interface IReferenceStamps with
        member _.GetLastWriteTimeUtc fullFilePath =
            lock gate (fun () ->
                match watchedFiles.TryGetValue fullFilePath with
                | true, { Stamp = ValueSome stamp } -> stamp
                | true, entry ->
                    let stamp = IO.File.GetLastWriteTimeUtc fullFilePath
                    entry.Stamp <- ValueSome stamp
                    stamp
                | _ -> IO.File.GetLastWriteTimeUtc fullFilePath)

        member _.Invalidate fullFilePath =
            lock gate (fun () ->
                match watchedFiles.TryGetValue fullFilePath with
                | true, entry -> entry.Stamp <- ValueNone
                | _ -> ())

    member _.Dispose() =
        lock gate (fun () ->
            if not disposed then
                disposed <- true
                watchedFiles.Clear()

                for KeyValue(_, timer) in pendingTimers do
                    timer.Dispose()

                pendingTimers.Clear()

                if context.IsValueCreated then
                    context.Value.Dispose())

    interface IDisposable with
        member this.Dispose() = this.Dispose()
