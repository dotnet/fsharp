// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

// Push-based file watching for FSharp.Editor, modelled on Roslyn's
// Microsoft.VisualStudio.LanguageServices FileChangeWatcher (which is internal and not
// exposed through ExternalAccess.FSharp). Uses the free-threaded IVsAsyncFileChangeEx2
// service: subscriptions are batched off the UI thread and callbacks never marshal to it.

/// A directory to watch recursively (with optional extension filters) so that individual
/// files under it don't each need their own advise cookie.
[<Sealed>]
type internal WatchedDirectory(path: string, extensionFilters: string list) =
    let path =
        if path.EndsWith(string IO.Path.DirectorySeparatorChar) then
            path
        else
            path + string IO.Path.DirectorySeparatorChar

    do
        for filter in extensionFilters do
            if not (filter.StartsWith ".") then
                invalidArg (nameof extensionFilters) $"Filter '{filter}' must start with a period."

    member _.Path = path
    member _.ExtensionFilters = extensionFilters

    static member FilePathCoveredByWatchedDirectories(watchedDirectories: WatchedDirectory list, filePath: string) =
        watchedDirectories
        |> List.exists (fun w ->
            filePath.StartsWith(w.Path, StringComparison.OrdinalIgnoreCase)
            && (w.ExtensionFilters.IsEmpty
                || w.ExtensionFilters
                   |> List.exists (fun f -> filePath.EndsWith(f, StringComparison.OrdinalIgnoreCase))))

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
    abstract CreateContext: watchedDirectories: WatchedDirectory list -> IFSharpFileChangeContext

[<AutoOpen>]
module private FileChangeWatcherImpl =

    // Same flags Roslyn uses for both subscribing and filtering callbacks.
    let watchFlags = _VSFILECHANGEFLAGS.VSFILECHG_Size ||| _VSFILECHANGEFLAGS.VSFILECHG_Time

    let relevantFlags =
        _VSFILECHANGEFLAGS.VSFILECHG_Time
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Add
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Del
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Size

    /// Empirically strong batching window during high activity (solution open/close); see
    /// Roslyn's FileChangeWatcher.
    let batchingDelay = TimeSpan.FromMilliseconds 500.

[<Sealed>]
type internal FSharpWatchedFileToken() =
    member val Cookie: uint32 option = None with get, set

/// Subscription operations queued for batched application against the file change service.
type private WatcherOperation =
    | WatchDir of path: string * filters: string list * sink: IVsFreeThreadedFileChangeEvents2 * cookies: List<uint32>
    | WatchFiles of paths: string list * tokens: FSharpWatchedFileToken list * sink: IVsFreeThreadedFileChangeEvents2
    | UnwatchFiles of tokens: FSharpWatchedFileToken list
    | UnwatchDirs of cookies: List<uint32>

[<Sealed>]
type internal FSharpFileChangeWatcher(fileChangeService: Task<IVsAsyncFileChangeEx2>) =

    let applyBatch (service: IVsAsyncFileChangeEx2) (ops: WatcherOperation list) =
        task {
            // Coalesce adjacent same-kind operations into single service calls, preserving order
            // between kinds (a watch enqueued before an unwatch must be applied first).
            let mutable pending = ops

            while not pending.IsEmpty do
                match pending with
                | [] -> ()
                | WatchDir(path, filters, sink, cookies) :: rest ->
                    pending <- rest
                    let! cookie = service.AdviseDirChangeAsync(path, true, sink, CancellationToken.None)
                    cookies.Add cookie

                    if not filters.IsEmpty then
                        do! service.FilterDirectoryChangesAsync(cookie, List.toArray filters, CancellationToken.None)

                | WatchFiles _ :: _ ->
                    let batch = pending |> List.takeWhile (function WatchFiles _ -> true | _ -> false)
                    pending <- pending |> List.skip batch.Length

                    let paths = batch |> List.collect (function WatchFiles(p, _, _) -> p | _ -> [])
                    let tokens = batch |> List.collect (function WatchFiles(_, t, _) -> t | _ -> [])
                    let sink = batch |> List.pick (function WatchFiles(_, _, s) -> Some s | _ -> None)

                    let! cookies = service.AdviseFileChangesAsync(List.toArray paths, watchFlags, sink, CancellationToken.None)

                    (tokens, List.ofArray cookies)
                    ||> List.iter2 (fun token cookie -> token.Cookie <- Some cookie)

                | UnwatchFiles _ :: _ ->
                    let batch = pending |> List.takeWhile (function UnwatchFiles _ -> true | _ -> false)
                    pending <- pending |> List.skip batch.Length

                    let cookies =
                        batch
                        |> List.collect (function UnwatchFiles t -> t | _ -> [])
                        |> List.choose (fun token -> token.Cookie)

                    if not cookies.IsEmpty then
                        let! _ = service.UnadviseFileChangesAsync(List.toArray cookies, CancellationToken.None)
                        ()

                | UnwatchDirs cookies :: rest ->
                    pending <- rest

                    if cookies.Count > 0 then
                        let! _ = service.UnadviseDirChangesAsync(cookies.ToArray(), CancellationToken.None)
                        ()
        }

    // Single consumer loop: waits for the first queued operation, sleeps out the batching
    // window, drains the queue and applies everything in one pass. Nothing ever blocks on the
    // service being available.
    let agent =
        MailboxProcessor<WatcherOperation>.Start(fun inbox ->
            async {
                while true do
                    try
                        let! first = inbox.Receive()
                        do! Async.Sleep(int batchingDelay.TotalMilliseconds)

                        let ops = ResizeArray [ first ]
                        let mutable draining = true

                        while draining do
                            match! inbox.TryReceive 0 with
                            | Some op -> ops.Add op
                            | None -> draining <- false

                        let! service = fileChangeService |> Async.AwaitTask
                        do! applyBatch service (List.ofSeq ops) |> Async.AwaitTask
                    with _ ->
                        // Never let a failed advise/unadvise (e.g. non-existent path) kill the
                        // subscription loop; we simply won't get events for that path.
                        ()
            })

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

and [<Sealed>] private FileChangeContext(enqueue: WatcherOperation -> unit, watchedDirectories: WatchedDirectory list) as this =

    let gate = obj ()
    let mutable disposed = false
    let activeFileTokens = HashSet<FSharpWatchedFileToken>()
    let directoryCookies = List<uint32>()
    let fileChanged = Event<string>()

    let raiseChanges (count: uint32) (files: string[]) (changeFlags: uint32[]) =
        for i in 0 .. int count - 1 do
            if (enum<_VSFILECHANGEFLAGS> (int changeFlags[i]) &&& relevantFlags) <> enum<_VSFILECHANGEFLAGS> 0 then
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
                // Covered by a directory watch; nothing extra to subscribe.
                { new IFSharpWatchedFile with
                    member _.Dispose() = ()
                }
            else
                let token = FSharpWatchedFileToken()
                lock gate (fun () -> activeFileTokens.Add token |> ignore)
                enqueue (WatchFiles([ filePath ], [ token ], this :> IVsFreeThreadedFileChangeEvents2))

                { new IFSharpWatchedFile with
                    member _.Dispose() = this.StopWatchingFile token
                }

    interface IDisposable with
        member _.Dispose() =
            let alreadyDisposed = lock gate (fun () ->
                let d = disposed
                disposed <- true
                d)

            if not alreadyDisposed then
                enqueue (UnwatchDirs directoryCookies)
                enqueue (UnwatchFiles(lock gate (fun () -> List.ofSeq activeFileTokens)))

    // Free-threaded sink: callbacks arrive on background threads and stay there.
    interface IVsFreeThreadedFileChangeEvents2 with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) = raiseChanges cChanges rgpszFile rggrfChange
        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL
        member _.DirectoryChangedEx(_, _) = VSConstants.E_NOTIMPL
        member _.DirectoryChangedEx2(_, cChanges, rgpszFile, rggrfChange) = raiseChanges cChanges rgpszFile rggrfChange

    interface IVsFreeThreadedFileChangeEvents with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) = raiseChanges cChanges rgpszFile rggrfChange
        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL
        member _.DirectoryChangedEx(_, _) = VSConstants.E_NOTIMPL

    interface IVsFileChangeEvents with
        member _.FilesChanged(cChanges, rgpszFile, rggrfChange) = raiseChanges cChanges rgpszFile rggrfChange
        member _.DirectoryChanged _ = VSConstants.E_NOTIMPL

/// Ref-counted, debounced watching of reference assemblies (or any other off-workspace files),
/// modelled on Roslyn's ReferenceFileChangeTracker. Multiple projects watching the same dll
/// share one subscription; bursts of writes produce a single callback per path.
[<Sealed>]
type internal FSharpReferenceChangeTracker(watcher: IFSharpFileChangeWatcher, onChanged: string -> unit) =

    /// Delay between the last observed change to a path and the callback: a rebuild typically
    /// writes a temp file then renames, producing several rapid notifications.
    static let notificationDelay = TimeSpan.FromSeconds 2.

    let gate = obj ()
    let mutable disposed = false
    let watchedFiles = Dictionary<string, IFSharpWatchedFile * int>(StringComparer.OrdinalIgnoreCase)
    let pendingTimers = ConcurrentDictionary<string, Timer>(StringComparer.OrdinalIgnoreCase)

    // On each platform there is a place framework reference assemblies live; these rarely change
    // but account for most watched paths, so cover them with directory watches up front.
    static let defaultWatchedDirectories () =
        let dotnetRoot = Environment.GetEnvironmentVariable "DOTNET_ROOT"

        [
            if not (String.IsNullOrEmpty dotnetRoot) then
                IO.Path.Combine(dotnetRoot, "packs")

            IO.Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.ProgramFiles, "dotnet", "packs")

            IO.Path.Combine(
                Environment.GetFolderPath Environment.SpecialFolder.ProgramFilesX86,
                "Reference Assemblies",
                "Microsoft",
                "Framework"
            )

            IO.Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget", "packages")
        ]
        |> List.distinct
        |> List.map (fun d -> WatchedDirectory(d, [ ".dll" ]))

    let context =
        lazy
            (let ctx = watcher.CreateContext(defaultWatchedDirectories ())

             ctx.FileChanged.Add(fun path ->
                 let fire (_: obj) =
                     pendingTimers.TryRemove path
                     |> function
                         | true, timer -> timer.Dispose()
                         | _ -> ()

                     // Only notify for paths someone is actually watching; directory watches
                     // cover whole trees.
                     let isWatched = lock gate (fun () -> watchedFiles.ContainsKey path)

                     if isWatched then
                         onChanged path

                 let timer = pendingTimers.GetOrAdd(path, fun _ -> new Timer(fire, null, Timeout.Infinite, Timeout.Infinite))
                 timer.Change(notificationDelay, Timeout.InfiniteTimeSpan) |> ignore)

             ctx)

    /// Starts watching a path, ref-counted. Call StopWatchingReference exactly once per start.
    member _.StartWatchingReference(fullFilePath: string) =
        lock gate (fun () ->
            if not disposed then
                match watchedFiles.TryGetValue fullFilePath with
                | true, (token, count) -> watchedFiles[fullFilePath] <- (token, count + 1)
                | _ -> watchedFiles[fullFilePath] <- (context.Value.EnqueueWatchingFile fullFilePath, 1))

    member _.StopWatchingReference(fullFilePath: string) =
        lock gate (fun () ->
            if not disposed then
                match watchedFiles.TryGetValue fullFilePath with
                | true, (token, 1) ->
                    watchedFiles.Remove fullFilePath |> ignore
                    token.Dispose()
                | true, (token, count) -> watchedFiles[fullFilePath] <- (token, count - 1)
                | _ -> ())

    interface IDisposable with
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
