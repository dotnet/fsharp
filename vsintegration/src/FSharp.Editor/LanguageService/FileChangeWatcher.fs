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

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

open CancellableTasks

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

    static member Covers(directory: WatchedDirectory, filePath: string) =
        filePath.StartsWith(directory.Path, StringComparison.OrdinalIgnoreCase)
        && (directory.ExtensionFilters.IsEmpty
            || directory.ExtensionFilters
               |> Seq.exists (fun filter -> filePath.EndsWith(filter, StringComparison.OrdinalIgnoreCase)))

    static member FilePathCoveredByWatchedDirectories(watchedDirectories: ImmutableArray<WatchedDirectory>, filePath: string) =
        watchedDirectories |> Seq.exists (fun w -> WatchedDirectory.Covers(w, filePath))

type internal IFSharpWatchedFile =
    inherit IDisposable

    /// True once the advise has succeeded. A consumer caching something derived from a watch must gate on this.
    abstract IsActive: bool

type internal IFSharpFileChangeContext =
    inherit IDisposable

    [<CLIEvent>]
    abstract FileChanged: IEvent<string>

    /// A path under one of the context's watched directories gets a watch that needs no advise of its own.
    abstract EnqueueWatchingFile: filePath: string -> IFSharpWatchedFile

type internal IFSharpFileChangeWatcher =
    abstract CreateContext: watchedDirectories: ImmutableArray<WatchedDirectory> -> IFSharpFileChangeContext

/// A path nobody watches is stat'd directly.
type internal IReferenceStamps =
    abstract GetLastWriteTimeUtc: fullFilePath: string -> DateTime
    abstract Invalidate: fullFilePath: string -> unit

type private WatchedReference =
    {
        Token: IFSharpWatchedFile
        mutable Count: int
        mutable Stamp: DateTime voption
        mutable StampDrops: int
    }

[<Struct; RequireQualifiedAccess>]
type private StampLookup =
    | Cached of stamp: DateTime
    | Cold of entry: WatchedReference * stampDropsAtLookup: int
    | Unwatched

[<AutoOpen>]
module private FileChangeWatcherImpl =

    // A reference is watched before the project that produces it is built, and is replaced by a rename:
    // its creation and its deletion have to arrive as well.
    let watchFlags =
        _VSFILECHANGEFLAGS.VSFILECHG_Size
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Time
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Add
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Del

    let relevantFlags =
        _VSFILECHANGEFLAGS.VSFILECHG_Time
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Add
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Del
        ||| _VSFILECHANGEFLAGS.VSFILECHG_Size

    let roslynBatchingDelay = TimeSpan.FromMilliseconds 500.

[<Sealed>]
type internal FSharpWatchedFileToken() =
    member val Cookie: uint32 voption = ValueNone with get, set

    member this.TakeCookie() =
        let cookie = this.Cookie
        this.Cookie <- ValueNone
        cookie

type private WatcherOperation =
    | WatchDir of
        path: string *
        filters: ImmutableArray<string> *
        sink: IVsFreeThreadedFileChangeEvents2 *
        cookies: List<uint32> *
        active: bool ref
    | WatchFiles of paths: string list * tokens: FSharpWatchedFileToken list * sink: IVsFreeThreadedFileChangeEvents2
    | UnwatchFiles of tokens: FSharpWatchedFileToken list
    | UnwatchDirs of cookies: List<uint32>

[<Sealed>]
type internal FSharpFileChangeWatcher(fileChangeService: Task<IVsAsyncFileChangeEx2>, batchingDelay: TimeSpan) =

    let applyBatch (service: IVsAsyncFileChangeEx2) (ops: WatcherOperation list) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let mutable pending = ops

            while not pending.IsEmpty do
                match pending with
                | [] -> ()
                | WatchDir(path, filters, sink, cookies, active) :: rest ->
                    pending <- rest

                    try
                        let! cookie = service.AdviseDirChangeAsync(path, true, sink, ct)
                        cookies.Add cookie

                        if not filters.IsEmpty then
                            do! service.FilterDirectoryChangesAsync(cookie, Seq.toArray filters, ct)

                        active.Value <- true
                    with ex when not (ex :? OperationCanceledException) ->
                        FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)

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

                    try
                        let! cookies = service.AdviseFileChangesAsync(paths, watchFlags, sink, ct)

                        (tokens, cookies)
                        ||> Array.iter2 (fun token cookie -> token.Cookie <- ValueSome cookie)
                    with ex when not (ex :? OperationCanceledException) ->
                        FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)

                | UnwatchFiles _ :: _ ->
                    let batch =
                        pending
                        |> Seq.takeWhile (function
                            | UnwatchFiles _ -> true
                            | _ -> false)
                        |> Seq.toArray

                    pending <- pending |> List.skip batch.Length

                    let cookies =
                        [|
                            for op in batch do
                                match op with
                                | UnwatchFiles tokens ->
                                    for token in tokens do
                                        match token.TakeCookie() with
                                        | ValueSome cookie -> cookie
                                        | ValueNone -> ()
                                | _ -> ()
                        |]

                    try
                        if cookies.Length > 0 then
                            let! _ = service.UnadviseFileChangesAsync(cookies, ct)
                            ()
                    with ex when not (ex :? OperationCanceledException) ->
                        FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)

                | UnwatchDirs cookies :: rest ->
                    pending <- rest

                    try
                        if cookies.Count > 0 then
                            let! _ = service.UnadviseDirChangesAsync(cookies.ToArray(), ct)
                            ()
                    with ex when not (ex :? OperationCanceledException) ->
                        FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)
        }

    let cancellationTokenSource = new CancellationTokenSource()

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
                                FSharpOutputPane.logExceptionWithContext (ex, nameof FSharpFileChangeWatcher)
                    }),
                cancellationTokenSource.Token
            )

    new(fileChangeService) = new FSharpFileChangeWatcher(fileChangeService, roslynBatchingDelay)

    member private _.Enqueue(op: WatcherOperation) = agent.Post op

    /// Asks for SVsFileChangeEx asynchronously, so no background thread waits on the UI thread for it.
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

    let directoryAdviseSucceeded =
        watchedDirectories |> Seq.map (fun _ -> ref false) |> Seq.toArray

    let directoryWatchedFiles =
        directoryAdviseSucceeded
        |> Array.map (fun active ->
            { new IFSharpWatchedFile with
                member _.IsActive = active.Value
                member _.Dispose() = ()
            })

    let raiseChanges (count: uint32) (files: string[]) (changeFlags: uint32[]) =
        for i in 0 .. int count - 1 do
            if
                (enum<_VSFILECHANGEFLAGS> (int changeFlags[i]) &&& relevantFlags)
                <> enum<_VSFILECHANGEFLAGS> 0
            then
                fileChanged.Trigger files[i]

        VSConstants.S_OK

    do
        watchedDirectories
        |> Seq.iteri (fun i watchedDirectory ->
            enqueue (
                WatchDir(
                    watchedDirectory.Path,
                    watchedDirectory.ExtensionFilters,
                    this :> IVsFreeThreadedFileChangeEvents2,
                    directoryCookies,
                    directoryAdviseSucceeded[i]
                )
            ))

    member private _.StopWatchingFile(token: FSharpWatchedFileToken) =
        lock gate (fun () -> activeFileTokens.Remove token |> ignore)
        enqueue (UnwatchFiles [ token ])

    interface IFSharpFileChangeContext with
        [<CLIEvent>]
        member _.FileChanged = fileChanged.Publish

        member _.EnqueueWatchingFile filePath =
            match
                watchedDirectories
                |> Seq.tryFindIndexV (fun w -> WatchedDirectory.Covers(w, filePath))
            with
            | ValueSome i -> directoryWatchedFiles[i]
            | ValueNone ->
                let token = FSharpWatchedFileToken()
                lock gate (fun () -> activeFileTokens.Add token |> ignore)
                enqueue (WatchFiles([ filePath ], [ token ], this :> IVsFreeThreadedFileChangeEvents2))

                { new IFSharpWatchedFile with
                    member _.IsActive = token.Cookie.IsSome
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

/// Ref-counted watches on off-workspace files, each caching its last-write stamp until a change notification drops it.
[<Sealed>]
type internal FSharpReferenceChangeTracker(watcher: IFSharpFileChangeWatcher, getLastWriteTimeUtc: string -> DateTime) =

    let gate = obj ()
    let mutable disposed = false

    let watchedFiles =
        Dictionary<string, WatchedReference>(StringComparer.OrdinalIgnoreCase)

    let dropStamp (entry: WatchedReference) =
        entry.Stamp <- ValueNone
        entry.StampDrops <- entry.StampDrops + 1

    static let frameworkAndPackageDirectories () =
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
            (let ctx = watcher.CreateContext(frameworkAndPackageDirectories ())

             ctx.FileChanged.Add(fun path ->
                 lock gate (fun () ->
                     match watchedFiles.TryGetValue path with
                     | true, entry -> dropStamp entry
                     | _ -> ()))

             ctx)

    new(watcher) = new FSharpReferenceChangeTracker(watcher, IO.File.GetLastWriteTimeUtc)

    /// Each start needs one StopWatchingReference.
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
                            StampDrops = 0
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
            let lookup =
                lock gate (fun () ->
                    match watchedFiles.TryGetValue fullFilePath with
                    | true, entry when entry.Token.IsActive ->
                        match entry.Stamp with
                        | ValueSome stamp -> StampLookup.Cached stamp
                        | ValueNone -> StampLookup.Cold(entry, entry.StampDrops)
                    | _ -> StampLookup.Unwatched)

            match lookup with
            | StampLookup.Cached stamp -> stamp
            | StampLookup.Unwatched -> getLastWriteTimeUtc fullFilePath
            | StampLookup.Cold(entry, stampDropsAtLookup) ->
                // Outside the lock, which every change notification also takes.
                let stamp = getLastWriteTimeUtc fullFilePath

                lock gate (fun () ->
                    match watchedFiles.TryGetValue fullFilePath with
                    | true, current when
                        obj.ReferenceEquals(current, entry)
                        && current.StampDrops = stampDropsAtLookup
                        && current.Token.IsActive
                        ->
                        current.Stamp <- ValueSome stamp
                    | _ -> ())

                stamp

        member _.Invalidate fullFilePath =
            lock gate (fun () ->
                match watchedFiles.TryGetValue fullFilePath with
                | true, entry -> dropStamp entry
                | _ -> ())

    member _.Dispose() =
        lock gate (fun () ->
            if not disposed then
                disposed <- true
                watchedFiles.Clear()

                if context.IsValueCreated then
                    context.Value.Dispose())

    interface IDisposable with
        member this.Dispose() = this.Dispose()

#nowarn "57" // ReferenceOnDisk is part of the experimental FCS snapshot API

[<RequireQualifiedAccess>]
module internal ReferenceStamps =

    /// A path with a different stamp on both sides also has its cached stamp dropped; a different path list drops nothing.
    let changed (stamps: IReferenceStamps) (current: ReferenceOnDisk list) (previous: ReferenceOnDisk list) =
        let rec walk changed current previous =
            match current, previous with
            | [], [] -> changed
            | c :: current, p :: previous ->
                if not (String.Equals(c.Path, p.Path, StringComparison.Ordinal)) then
                    true
                elif c.LastModified <> p.LastModified then
                    stamps.Invalidate c.Path
                    walk true current previous
                else
                    walk changed current previous
            | _ -> true

        walk false current previous
