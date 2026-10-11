// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

/// Stands in for the token EnqueueWatchingFile hands back: IsActive defaults to true (the mock's
/// watch is live the moment it is created) so most tests do not need to think about it; tests
/// exercising the pending/failed states flip it through the owning context's SetActive.
type internal MockWatchedFile(onDispose: unit -> unit) =
    let mutable isActive = true

    member _.SetActive value = isActive <- value

    interface IFSharpWatchedFile with
        member _.IsActive = isActive
        member _.Dispose() = onDispose ()

type internal MockFileChangeContext() =
    let fileChanged = Event<string>()
    let watched = ResizeArray<string>()
    let tokens = Dictionary<string, MockWatchedFile>(StringComparer.OrdinalIgnoreCase)

    member _.WatchedFiles = List.ofSeq watched
    member _.Fire path = fileChanged.Trigger path
    member _.SetActive(path, active) = tokens[path].SetActive active

    interface IFSharpFileChangeContext with
        [<CLIEvent>]
        member _.FileChanged = fileChanged.Publish

        member _.EnqueueWatchingFile path =
            watched.Add path
            let token = new MockWatchedFile(fun () -> watched.Remove path |> ignore)
            tokens[path] <- token
            token :> IFSharpWatchedFile

        member _.Dispose() = watched.Clear()

type internal MockFileChangeWatcher() =
    let mutable context: MockFileChangeContext voption = ValueNone

    member _.Context = context

    interface IFSharpFileChangeWatcher with
        member _.CreateContext _ =
            let ctx = new MockFileChangeContext()
            context <- ValueSome ctx
            ctx :> IFSharpFileChangeContext

type private ServiceCall =
    | AdvisedDir of path: string * cookie: uint32
    | FilteredDir of cookie: uint32 * extensions: string list
    | AdvisedFiles of paths: string list * sink: obj * cookies: uint32 list
    | UnadvisedFiles of cookies: uint32 list
    | UnadvisedDirs of cookies: uint32 list

/// <summary>
/// Stands in for <see cref="IVsAsyncFileChangeEx2"/>: hands out sequential cookies and records every call, so a
/// test can see how the watcher turned its queue into service calls.
/// </summary>
type private RecordingFileChangeService() =
    let calls = ResizeArray<ServiceCall>()
    let mutable nextCookie = 0u
    let mutable failAdviseDirForPath: string option = None

    let record call = lock calls (fun () -> calls.Add call)

    let newCookie () =
        nextCookie <- nextCookie + 1u
        nextCookie

    member _.Calls = lock calls (fun () -> List.ofSeq calls)

    /// Every AdviseDirChangeAsync for this exact (already directory-separator-normalized) path
    /// throws instead of succeeding, so a test can exercise what happens to the rest of a batch
    /// when one operation in it fails.
    member _.FailAdviseDirForPath
        with set value = failAdviseDirForPath <- value

    member this.WaitForCalls(count: int) =
        let deadline = DateTime.UtcNow + TimeSpan.FromSeconds 10.

        while lock calls (fun () -> calls.Count) < count && DateTime.UtcNow < deadline do
            Thread.Sleep 10

        this.Calls

    interface IVsAsyncFileChangeEx2 with
        member _.AdviseFileChangesAsync(filenames, _, sink, _) =
            let cookies = [| for _ in filenames -> newCookie () |]
            record (AdvisedFiles(List.ofSeq filenames, box sink, List.ofArray cookies))
            Task.FromResult cookies

    interface IVsAsyncFileChangeEx with
        member _.AdviseFileChangeAsync(_, _, _, _) = Task.FromResult(newCookie ())
        member _.UnadviseFileChangeAsync(_, _) = Task.FromResult ""

        member _.UnadviseFileChangesAsync(cookies, _) =
            record (UnadvisedFiles(List.ofSeq cookies))
            Task.FromResult Array.empty<string>

        member _.AdviseDirChangeAsync(directory, _, _, _) =
            match failAdviseDirForPath with
            | Some p when String.Equals(p, directory, StringComparison.OrdinalIgnoreCase) ->
                Task.FromException<uint32>(InvalidOperationException "simulated advise failure")
            | _ ->
                let cookie = newCookie ()
                record (AdvisedDir(directory, cookie))
                Task.FromResult cookie

        member _.UnadviseDirChangeAsync(_, _) = Task.FromResult ""

        member _.UnadviseDirChangesAsync(cookies, _) =
            record (UnadvisedDirs(List.ofSeq cookies))
            Task.FromResult Array.empty<string>

        member _.SyncFileAsync(_, _) = Task.CompletedTask
        member _.IgnoreFileAsync(_, _, _, _) = Task.CompletedTask
        member _.IgnoreDirAsync(_, _, _) = Task.CompletedTask

        member _.FilterDirectoryChangesAsync(cookie, extensions, _) =
            record (FilteredDir(cookie, List.ofArray extensions))
            Task.CompletedTask

#nowarn "57" // ReferenceOnDisk is part of the experimental FCS snapshot API

module FileChangeWatcherTests =

    let private batchDelay = TimeSpan.FromMilliseconds 100.

    let private noDirectories = ImmutableArray<WatchedDirectory>.Empty

    let private createWatcher (service: RecordingFileChangeService) =
        new FSharpFileChangeWatcher(Task.FromResult(service :> IVsAsyncFileChangeEx2), batchDelay)

    let private becomesActive (watched: IFSharpWatchedFile) =
        SpinWait.SpinUntil((fun () -> watched.IsActive), TimeSpan.FromSeconds 10.)

    [<Fact>]
    let ``WatchedDirectory covers files under it matching the extension filter`` () =
        let dirs =
            ImmutableArray.Create(WatchedDirectory(@"C:\refs", ImmutableArray.Create ".dll"))

        Assert.True(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refs\sub\a.dll"))
        Assert.True(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\REFS\A.DLL"))
        Assert.False(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refs\a.xml"))
        Assert.False(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\other\a.dll"))

    [<Fact>]
    let ``WatchedDirectory without filters covers any file under it`` () =
        let dirs =
            ImmutableArray.Create(WatchedDirectory(@"C:\refs", ImmutableArray<string>.Empty))

        Assert.True(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refs\a.xml"))
        Assert.False(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refsx\a.xml"))

    [<Fact>]
    let ``Tracker ref-counts subscriptions per path`` () =
        let watcher = MockFileChangeWatcher()
        use tracker = new FSharpReferenceChangeTracker(watcher)

        tracker.StartWatchingReference @"C:\x\a.dll"
        tracker.StartWatchingReference @"C:\x\a.dll"
        tracker.StartWatchingReference @"C:\x\b.dll"

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll" ], watcher.Context.Value.WatchedFiles)

        tracker.StopWatchingReference @"C:\x\a.dll"
        Assert.Contains(@"C:\x\a.dll", watcher.Context.Value.WatchedFiles)

        tracker.StopWatchingReference @"C:\x\a.dll"
        Assert.Equal<string list>([ @"C:\x\b.dll" ], watcher.Context.Value.WatchedFiles)

    [<Fact>]
    let ``A change notification for an unwatched path is ignored`` () =
        let watcher = MockFileChangeWatcher()
        let stat = ResizeArray<string>()

        use tracker =
            new FSharpReferenceChangeTracker(
                watcher,
                fun path ->
                    stat.Add path
                    DateTime.UtcNow
            )

        let stamps = tracker :> IReferenceStamps
        tracker.StartWatchingReference @"C:\x\a.dll"

        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        watcher.Context.Value.Fire @"C:\x\unwatched.dll"

        // The watched path keeps its cached stamp; the unwatched one is stat'd on every read.
        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\unwatched.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\unwatched.dll" |> ignore

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\unwatched.dll"; @"C:\x\unwatched.dll" ], List.ofSeq stat)

    [<Fact>]
    let ``Disposed tracker stats directly and ignores notifications`` () =
        let watcher = MockFileChangeWatcher()
        let stat = ResizeArray<string>()

        let tracker =
            new FSharpReferenceChangeTracker(
                watcher,
                fun path ->
                    stat.Add path
                    DateTime.UtcNow
            )

        let stamps = tracker :> IReferenceStamps
        tracker.StartWatchingReference @"C:\x\a.dll"
        let context = watcher.Context.Value
        (tracker :> IDisposable).Dispose()

        context.Fire @"C:\x\a.dll"
        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\a.dll" ], List.ofSeq stat)

    [<Fact>]
    let ``Consecutive file watches are advised in one service call`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext noDirectories

        for path in [ @"C:\x\a.dll"; @"C:\x\b.dll"; @"C:\x\c.dll" ] do
            context.EnqueueWatchingFile path |> ignore

        match service.WaitForCalls 1 with
        | [ AdvisedFiles(paths, _, cookies) ] ->
            Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll"; @"C:\x\c.dll" ], paths)
            Assert.Equal<uint32 list>([ 1u; 2u; 3u ], cookies)
        | calls -> failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``A run of file watches is split when the sink changes`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        let factory = watcher :> IFSharpFileChangeWatcher
        use first = factory.CreateContext noDirectories
        use second = factory.CreateContext noDirectories

        first.EnqueueWatchingFile @"C:\x\a.dll" |> ignore
        first.EnqueueWatchingFile @"C:\x\b.dll" |> ignore
        second.EnqueueWatchingFile @"C:\x\c.dll" |> ignore

        match service.WaitForCalls 2 with
        | [ AdvisedFiles(firstPaths, firstSink, [ 1u; 2u ]); AdvisedFiles(secondPaths, secondSink, [ 3u ]) ] ->
            Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll" ], firstPaths)
            Assert.Equal<string list>([ @"C:\x\c.dll" ], secondPaths)
            Assert.False(obj.ReferenceEquals(firstSink, secondSink))
        | calls -> failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``A watch followed by an unwatch in the same batch unadvises the cookie the watch received`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext noDirectories

        let watched = context.EnqueueWatchingFile @"C:\x\a.dll"
        watched.Dispose()

        match service.WaitForCalls 2 with
        | [ AdvisedFiles(_, _, [ advised ]); UnadvisedFiles [ unadvised ] ] -> Assert.Equal(advised, unadvised)
        | calls -> failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``Unwatching a token that holds no cookie is a no-op`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext noDirectories

        let watched = context.EnqueueWatchingFile @"C:\x\a.dll"
        service.WaitForCalls 1 |> ignore

        watched.Dispose()
        service.WaitForCalls 2 |> ignore

        watched.Dispose()
        Thread.Sleep(batchDelay + batchDelay + batchDelay)

        match service.Calls with
        | [ AdvisedFiles(_, _, [ advised ]); UnadvisedFiles [ unadvised ] ] -> Assert.Equal(advised, unadvised)
        | calls -> failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``Disposing a context unadvises its directory and remaining file cookies`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service

        let directories =
            ImmutableArray.Create(WatchedDirectory(@"C:\refs", ImmutableArray.Create ".dll"))

        let context = (watcher :> IFSharpFileChangeWatcher).CreateContext directories
        context.EnqueueWatchingFile @"C:\refs\covered.dll" |> ignore
        context.EnqueueWatchingFile @"C:\other\a.dll" |> ignore
        service.WaitForCalls 3 |> ignore

        context.Dispose()

        match service.WaitForCalls 5 with
        | [ AdvisedDir(directory, 1u)
            FilteredDir(1u, [ ".dll" ])
            AdvisedFiles([ @"C:\other\a.dll" ], _, [ 2u ])
            UnadvisedDirs [ 1u ]
            UnadvisedFiles [ 2u ] ] -> Assert.Equal(@"C:\refs\", directory)
        | calls -> failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``A directory-covered watch becomes active once the directory's advise succeeds`` () =
        let serviceAvailable =
            TaskCompletionSource<IVsAsyncFileChangeEx2>(TaskCreationOptions.RunContinuationsAsynchronously)

        use watcher = new FSharpFileChangeWatcher(serviceAvailable.Task, batchDelay)

        let directories =
            ImmutableArray.Create(WatchedDirectory(@"C:\refs", ImmutableArray.Create ".dll"))

        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext directories
        let watched = context.EnqueueWatchingFile @"C:\refs\a.dll"

        // The advise is queued but cannot run until the service is available, however long that takes.
        Assert.False watched.IsActive

        serviceAvailable.SetResult(RecordingFileChangeService())
        Assert.True(becomesActive watched)

    [<Fact>]
    let ``A failing directory advise does not block the rest of the batch`` () =
        let service = RecordingFileChangeService()
        service.FailAdviseDirForPath <- Some @"C:\bad\"
        use watcher = createWatcher service

        let directories =
            ImmutableArray.Create(
                WatchedDirectory(@"C:\bad", ImmutableArray.Create ".dll"),
                WatchedDirectory(@"C:\good", ImmutableArray.Create ".dll")
            )

        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext directories
        let badWatch = context.EnqueueWatchingFile @"C:\bad\a.dll"
        let goodWatch = context.EnqueueWatchingFile @"C:\good\a.dll"

        // Both directories are advised in the same batch, the failing one first, so once the good
        // one is active the bad one has already had its only chance.
        Assert.True(becomesActive goodWatch)
        Assert.False badWatch.IsActive

    let private t0 = DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let private t1 = t0.AddHours 1.

    let private withTempFile (test: string -> unit) =
        let path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dll")
        File.WriteAllBytes(path, Array.empty)
        File.SetLastWriteTimeUtc(path, t0)

        try
            test path
        finally
            File.Delete path

    [<Fact>]
    let ``Watched path is served from the cache until a change notification`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher)
            let stamps = tracker :> IReferenceStamps

            tracker.StartWatchingReference path
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            watcher.Context.Value.Fire path
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    [<Fact>]
    let ``Unwatched path is stat'd on every read`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher)
            let stamps = tracker :> IReferenceStamps

            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    [<Fact>]
    let ``Invalidate drops the cached stamp`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher)
            let stamps = tracker :> IReferenceStamps

            tracker.StartWatchingReference path
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t1)
            stamps.Invalidate path
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    [<Fact>]
    let ``Stopping the last watch on a path falls back to stat`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher)
            let stamps = tracker :> IReferenceStamps

            tracker.StartWatchingReference path
            tracker.StartWatchingReference path
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t1)
            tracker.StopWatchingReference path
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            tracker.StopWatchingReference path
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    [<Fact>]
    let ``A pending watch always stats fresh; caching begins only once it becomes active`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher)
            let stamps = tracker :> IReferenceStamps

            tracker.StartWatchingReference path
            let context = watcher.Context.Value
            context.SetActive(path, false)

            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            // Still pending: an external change is visible on the very next read, nothing cached yet.
            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path)

            // Once the advise is confirmed, this read is the one that gets trusted from here on.
            context.SetActive(path, true)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t0)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    /// A stat source that counts its calls and runs `duringStat` in the middle of each one, i.e.
    /// while the tracker's lock is not held.
    let private countingStat (duringStat: string -> unit) =
        let count = ref 0

        let stat (path: string) =
            Interlocked.Increment &count.contents |> ignore
            duringStat path
            File.GetLastWriteTimeUtc path

        count, stat

    [<Fact>]
    let ``A change reported while a stat is in flight is not cached`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            let fireDuringStat = ref false

            let count, stat =
                countingStat (fun _ ->
                    if fireDuringStat.Value then
                        watcher.Context.Value.Fire path)

            use tracker = new FSharpReferenceChangeTracker(watcher, stat)
            let stamps = tracker :> IReferenceStamps
            tracker.StartWatchingReference path

            fireDuringStat.Value <- true
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)
            fireDuringStat.Value <- false

            // The value read before the change was not stored: this read stats again and sees the new time.
            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path)
            Assert.Equal(2, count.Value))

    [<Fact>]
    let ``Invalidate while a stat is in flight is not cached`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            let invalidateDuringStat = ref false
            let stamps = ref ValueNone

            let count, stat =
                countingStat (fun _ ->
                    if invalidateDuringStat.Value then
                        stamps.Value
                        |> ValueOption.iter (fun (stamps: IReferenceStamps) -> stamps.Invalidate path))

            use tracker = new FSharpReferenceChangeTracker(watcher, stat)
            stamps.Value <- ValueSome(tracker :> IReferenceStamps)
            tracker.StartWatchingReference path

            invalidateDuringStat.Value <- true
            Assert.Equal(t0, stamps.Value.Value.GetLastWriteTimeUtc path)
            invalidateDuringStat.Value <- false

            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t1, stamps.Value.Value.GetLastWriteTimeUtc path)
            Assert.Equal(t1, stamps.Value.Value.GetLastWriteTimeUtc path)
            Assert.Equal(2, count.Value))

    [<Fact>]
    let ``Restarting the watch while a stat is in flight leaves the new watch cold`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            let restartDuringStat = ref false
            let tracker = ref ValueNone

            let count, stat =
                countingStat (fun _ ->
                    if restartDuringStat.Value then
                        tracker.Value
                        |> ValueOption.iter (fun (tracker: FSharpReferenceChangeTracker) ->
                            tracker.StopWatchingReference path
                            tracker.StartWatchingReference path))

            use created = new FSharpReferenceChangeTracker(watcher, stat)
            tracker.Value <- ValueSome created
            let stamps = created :> IReferenceStamps
            created.StartWatchingReference path

            restartDuringStat.Value <- true
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)
            restartDuringStat.Value <- false

            // The entry the stat was taken for is gone; the new one stats for itself, then caches.
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)
            Assert.Equal(2, count.Value))

    [<Fact>]
    let ``Concurrent cold readers both stat and the stamp is then served from the cache`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use barrier = new Barrier(2)
            let meetInStat = ref true
            let met = ref true

            let count, stat =
                countingStat (fun _ ->
                    if meetInStat.Value && not (barrier.SignalAndWait(TimeSpan.FromSeconds 5.)) then
                        met.Value <- false)

            use tracker = new FSharpReferenceChangeTracker(watcher, stat)
            let stamps = tracker :> IReferenceStamps
            tracker.StartWatchingReference path

            // Two readers meet inside the stat, which they can only do if neither holds the tracker's lock there.
            Parallel.Invoke((fun () -> stamps.GetLastWriteTimeUtc path |> ignore), (fun () -> stamps.GetLastWriteTimeUtc path |> ignore))

            Assert.True met.Value
            Assert.Equal(2, count.Value)

            meetInStat.Value <- false
            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)
            Assert.Equal(2, count.Value))

    [<Fact>]
    let ``The stat does not hold the tracker lock`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            let stamps = ref ValueNone
            let otherPathReachable = ref false

            let _, stat =
                countingStat (fun _ ->
                    stamps.Value
                    |> ValueOption.iter (fun (stamps: IReferenceStamps) ->
                        otherPathReachable.Value <- Task.Run(fun () -> stamps.Invalidate @"C:\x\other.dll").Wait(TimeSpan.FromSeconds 1.)))

            use tracker = new FSharpReferenceChangeTracker(watcher, stat)
            stamps.Value <- ValueSome(tracker :> IReferenceStamps)
            tracker.StartWatchingReference path

            Assert.Equal(t0, stamps.Value.Value.GetLastWriteTimeUtc path)
            Assert.True otherPathReachable.Value)

    type private RecordingStamps() =
        let invalidated = ResizeArray<string>()

        member _.Invalidated = List.ofSeq invalidated

        interface IReferenceStamps with
            member _.GetLastWriteTimeUtc _ = failwith "not expected"
            member _.Invalidate path = invalidated.Add path

    let private reference path stamp : ReferenceOnDisk = { Path = path; LastModified = stamp }

    [<Fact>]
    let ``Equal reference lists are unchanged and invalidate nothing`` () =
        let stamps = RecordingStamps()
        let references = [ reference @"C:\x\a.dll" t0; reference @"C:\x\b.dll" t0 ]

        Assert.False(ReferenceStamps.changed stamps references references)
        Assert.Empty stamps.Invalidated

    [<Fact>]
    let ``A different stamp on the same path is a change that invalidates only that path`` () =
        let stamps = RecordingStamps()

        let previous =
            [
                reference @"C:\x\a.dll" t0
                reference @"C:\x\b.dll" t0
                reference @"C:\x\c.dll" t0
            ]

        let current =
            [
                reference @"C:\x\a.dll" t0
                reference @"C:\x\b.dll" t1
                reference @"C:\x\c.dll" t0
            ]

        Assert.True(ReferenceStamps.changed stamps current previous)
        Assert.Equal<string list>([ @"C:\x\b.dll" ], stamps.Invalidated)

    [<Fact>]
    let ``Two stale stamps are both invalidated`` () =
        let stamps = RecordingStamps()

        let previous =
            [
                reference @"C:\x\a.dll" t0
                reference @"C:\x\b.dll" t0
                reference @"C:\x\c.dll" t0
            ]

        let current =
            [
                reference @"C:\x\a.dll" t1
                reference @"C:\x\b.dll" t0
                reference @"C:\x\c.dll" t1
            ]

        Assert.True(ReferenceStamps.changed stamps current previous)
        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\c.dll" ], stamps.Invalidated)

    [<Fact>]
    let ``A different number of references is a change that invalidates nothing`` () =
        let stamps = RecordingStamps()
        let previous = [ reference @"C:\x\a.dll" t0; reference @"C:\x\b.dll" t0 ]
        let current = [ reference @"C:\x\a.dll" t0 ]

        Assert.True(ReferenceStamps.changed stamps current previous)
        Assert.Empty stamps.Invalidated

    [<Fact>]
    let ``A different path at the same position is a change that invalidates nothing`` () =
        let stamps = RecordingStamps()
        let previous = [ reference @"C:\x\a.dll" t0; reference @"C:\x\b.dll" t0 ]
        let current = [ reference @"C:\x\a.dll" t0; reference @"C:\x\c.dll" t1 ]

        Assert.True(ReferenceStamps.changed stamps current previous)
        Assert.Empty stamps.Invalidated
