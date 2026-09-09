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

/// Stands in for the token EnqueueWatchingFile hands back: IsActive defaults to true (the mock's
/// watch is live the moment it is created) so most tests do not need to think about it; tests
/// exercising the pending/failed states flip it through the owning context's SetActive.
type private MockWatchedFile(onDispose: unit -> unit) =
    let mutable isActive = true

    member _.SetActive value = isActive <- value

    interface IFSharpWatchedFile with
        member _.IsActive = isActive
        member _.Dispose() = onDispose ()

type private MockFileChangeContext() =
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
            let token = MockWatchedFile(fun () -> watched.Remove path |> ignore)
            tokens[path] <- token
            token :> IFSharpWatchedFile

        member _.Dispose() = watched.Clear()

type private MockFileChangeWatcher() =
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

module FileChangeWatcherTests =

    let private testDelay = TimeSpan.FromMilliseconds 50.

    let private batchDelay = TimeSpan.FromMilliseconds 100.

    let private noDirectories = ImmutableArray<WatchedDirectory>.Empty

    let private createWatcher (service: RecordingFileChangeService) =
        new FSharpFileChangeWatcher(Task.FromResult(service :> IVsAsyncFileChangeEx2), batchDelay)

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
        use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)

        tracker.StartWatchingReference @"C:\x\a.dll"
        tracker.StartWatchingReference @"C:\x\a.dll"
        tracker.StartWatchingReference @"C:\x\b.dll"

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll" ], watcher.Context.Value.WatchedFiles)

        tracker.StopWatchingReference @"C:\x\a.dll"
        Assert.Contains(@"C:\x\a.dll", watcher.Context.Value.WatchedFiles)

        tracker.StopWatchingReference @"C:\x\a.dll"
        Assert.Equal<string list>([ @"C:\x\b.dll" ], watcher.Context.Value.WatchedFiles)

    [<Fact>]
    let ``Tracker debounces bursts into a single callback for watched paths only`` () =
        let watcher = MockFileChangeWatcher()
        let calls = ResizeArray<string>()
        use signal = new ManualResetEventSlim(false)

        use tracker =
            new FSharpReferenceChangeTracker(
                watcher,
                (fun path ->
                    lock calls (fun () -> calls.Add path)
                    signal.Set()),
                testDelay
            )

        tracker.StartWatchingReference @"C:\x\a.dll"
        let context = watcher.Context.Value

        context.Fire @"C:\x\a.dll"
        context.Fire @"C:\x\a.dll"
        context.Fire @"C:\x\unwatched.dll"

        Assert.True(signal.Wait(TimeSpan.FromSeconds 10.))
        // Allow a trailing duplicate timer to fire if one was pending.
        Thread.Sleep(testDelay + testDelay)

        Assert.Equal<string list>([ @"C:\x\a.dll" ], lock calls (fun () -> List.ofSeq calls))

    [<Fact>]
    let ``Disposed tracker ignores further changes`` () =
        let watcher = MockFileChangeWatcher()
        let mutable called = false

        let tracker =
            new FSharpReferenceChangeTracker(watcher, (fun _ -> called <- true), testDelay)

        tracker.StartWatchingReference @"C:\x\a.dll"
        let context = watcher.Context.Value
        (tracker :> IDisposable).Dispose()

        context.Fire @"C:\x\a.dll"
        Thread.Sleep(testDelay + testDelay)

        Assert.False called

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
        let service = RecordingFileChangeService()
        use watcher = createWatcher service

        let directories =
            ImmutableArray.Create(WatchedDirectory(@"C:\refs", ImmutableArray.Create ".dll"))

        use context = (watcher :> IFSharpFileChangeWatcher).CreateContext directories
        let watched = context.EnqueueWatchingFile @"C:\refs\a.dll"

        Assert.False watched.IsActive
        service.WaitForCalls 2 |> ignore
        Assert.True watched.IsActive

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

        // The failing directory's advise never gets recorded; the good one still does.
        service.WaitForCalls 2 |> ignore

        Assert.False badWatch.IsActive
        Assert.True goodWatch.IsActive

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
            use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)
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
            use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)
            let stamps = tracker :> IReferenceStamps

            Assert.Equal(t0, stamps.GetLastWriteTimeUtc path)

            File.SetLastWriteTimeUtc(path, t1)
            Assert.Equal(t1, stamps.GetLastWriteTimeUtc path))

    [<Fact>]
    let ``Invalidate drops the cached stamp`` () =
        withTempFile (fun path ->
            let watcher = MockFileChangeWatcher()
            use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)
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
            use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)
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
            use tracker = new FSharpReferenceChangeTracker(watcher, ignore, testDelay)
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
