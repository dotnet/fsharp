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

    member _.WatchedFiles = lock watched (fun () -> List.ofSeq watched)
    member _.Fire path = fileChanged.Trigger path

    member _.SetActive(path, active) =
        (lock watched (fun () -> tokens[path])).SetActive active

    interface IFSharpFileChangeContext with
        [<CLIEvent>]
        member _.FileChanged = fileChanged.Publish

        member _.EnqueueWatchingFile path =
            let token =
                new MockWatchedFile(fun () -> lock watched (fun () -> watched.Remove path |> ignore))

            lock watched (fun () ->
                watched.Add path
                tokens[path] <- token)

            token :> IFSharpWatchedFile

        member _.Dispose() = lock watched watched.Clear

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

type private RecordingFileChangeService() =
    let calls = ResizeArray<ServiceCall>()
    let mutable nextCookie = 0u
    let mutable failAdviseDirForPath: string voption = ValueNone

    let record call = lock calls (fun () -> calls.Add call)

    let newCookie () =
        nextCookie <- nextCookie + 1u
        nextCookie

    member _.Calls = lock calls (fun () -> List.ofSeq calls)

    member _.FailAdviseDirForPath
        with set value = failAdviseDirForPath <- value

    member this.WaitForCalls(count: int) =
        let deadline = DateTime.UtcNow + TimeSpan.FromSeconds 60.

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
            | ValueSome p when String.Equals(p, directory, StringComparison.OrdinalIgnoreCase) ->
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

#nowarn "57"

module internal OwnThread =

    /// In a full test run the pool can take longer to start work than any timeout.
    let start (action: unit -> unit) =
        let thread = Thread(ThreadStart action, IsBackground = true)
        thread.Start()
        thread

module FileChangeWatcherTests =

    let private batchDelay = TimeSpan.FromMilliseconds 100.

    let private noDirectories = ImmutableArray<WatchedDirectory>.Empty

    let private dllDirectories (paths: string list) =
        ImmutableArray.CreateRange [ for path in paths -> WatchedDirectory(path, ImmutableArray.Create ".dll") ]

    let private createWatcher (service: RecordingFileChangeService) =
        new FSharpFileChangeWatcher(Task.FromResult(service :> IVsAsyncFileChangeEx2), batchDelay)

    let private contextOf (watcher: FSharpFileChangeWatcher) directories =
        (watcher :> IFSharpFileChangeWatcher).CreateContext directories

    let private withContext directories (test: RecordingFileChangeService -> IFSharpFileChangeContext -> unit) =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        use context = contextOf watcher directories
        test service context

    let private becomesActive (watched: IFSharpWatchedFile) =
        SpinWait.SpinUntil((fun () -> watched.IsActive), TimeSpan.FromSeconds 60.)

    let private unexpectedCalls (calls: ServiceCall list) : 'a = failwith $"Unexpected calls: %A{calls}"

    [<Fact>]
    let ``WatchedDirectory covers files under it matching the extension filter`` () =
        let dirs = dllDirectories [ @"C:\refs" ]

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
    let ``Consecutive file watches are advised in one service call`` () =
        withContext noDirectories (fun service context ->
            for path in [ @"C:\x\a.dll"; @"C:\x\b.dll"; @"C:\x\c.dll" ] do
                context.EnqueueWatchingFile path |> ignore

            match service.WaitForCalls 1 with
            | [ AdvisedFiles(paths, _, cookies) ] ->
                Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll"; @"C:\x\c.dll" ], paths)
                Assert.Equal<uint32 list>([ 1u; 2u; 3u ], cookies)
            | calls -> unexpectedCalls calls)

    [<Fact>]
    let ``A run of file watches is split when the sink changes`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        use first = contextOf watcher noDirectories
        use second = contextOf watcher noDirectories

        first.EnqueueWatchingFile @"C:\x\a.dll" |> ignore
        first.EnqueueWatchingFile @"C:\x\b.dll" |> ignore
        second.EnqueueWatchingFile @"C:\x\c.dll" |> ignore

        match service.WaitForCalls 2 with
        | [ AdvisedFiles(firstPaths, firstSink, [ 1u; 2u ]); AdvisedFiles(secondPaths, secondSink, [ 3u ]) ] ->
            Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\b.dll" ], firstPaths)
            Assert.Equal<string list>([ @"C:\x\c.dll" ], secondPaths)
            Assert.False(obj.ReferenceEquals(firstSink, secondSink))
        | calls -> unexpectedCalls calls

    [<Fact>]
    let ``A watch followed by an unwatch in the same batch unadvises the cookie the watch received`` () =
        withContext noDirectories (fun service context ->
            let watched = context.EnqueueWatchingFile @"C:\x\a.dll"
            watched.Dispose()

            match service.WaitForCalls 2 with
            | [ AdvisedFiles(_, _, [ advised ]); UnadvisedFiles [ unadvised ] ] -> Assert.Equal(advised, unadvised)
            | calls -> unexpectedCalls calls)

    [<Fact>]
    let ``Unwatching a token that holds no cookie is a no-op`` () =
        withContext noDirectories (fun service context ->
            let watched = context.EnqueueWatchingFile @"C:\x\a.dll"
            service.WaitForCalls 1 |> ignore

            watched.Dispose()
            service.WaitForCalls 2 |> ignore

            watched.Dispose()
            let after = @"C:\x\after.dll"
            context.EnqueueWatchingFile after |> ignore

            match service.WaitForCalls 3 with
            | [ AdvisedFiles(_, _, [ advised ]); UnadvisedFiles [ unadvised ]; AdvisedFiles([ watchedAfter ], _, _) ] ->
                Assert.Equal(advised, unadvised)
                Assert.Equal(after, watchedAfter)
            | calls -> unexpectedCalls calls)

    [<Fact>]
    let ``Disposing a context unadvises its directory and remaining file cookies`` () =
        let service = RecordingFileChangeService()
        use watcher = createWatcher service
        let context = contextOf watcher (dllDirectories [ @"C:\refs" ])
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
        | calls -> unexpectedCalls calls

    [<Fact>]
    let ``A directory-covered watch becomes active once the directory's advise succeeds`` () =
        let serviceAvailable =
            TaskCompletionSource<IVsAsyncFileChangeEx2>(TaskCreationOptions.RunContinuationsAsynchronously)

        use watcher = new FSharpFileChangeWatcher(serviceAvailable.Task, batchDelay)
        use context = contextOf watcher (dllDirectories [ @"C:\refs" ])
        let watched = context.EnqueueWatchingFile @"C:\refs\a.dll"

        Assert.False watched.IsActive

        serviceAvailable.SetResult(RecordingFileChangeService())
        Assert.True(becomesActive watched)

    [<Fact>]
    let ``A failing directory advise does not block the rest of the batch`` () =
        let service = RecordingFileChangeService()
        service.FailAdviseDirForPath <- ValueSome @"C:\bad\"
        use watcher = createWatcher service
        use context = contextOf watcher (dllDirectories [ @"C:\bad"; @"C:\good" ])
        let badWatch = context.EnqueueWatchingFile @"C:\bad\a.dll"
        let goodWatch = context.EnqueueWatchingFile @"C:\good\a.dll"

        // Same batch, the failing directory first: once the good one is active the bad one has had its only chance.
        Assert.True(becomesActive goodWatch)
        Assert.False badWatch.IsActive

    let private t0 = DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    let private t1 = t0.AddHours 1.

    type private Tracked() =
        let path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dll")
        let watcher = MockFileChangeWatcher()
        let mutable duringStat = ignore
        let mutable statCount = 0

        let tracker =
            new FSharpReferenceChangeTracker(
                watcher,
                fun path ->
                    Interlocked.Increment &statCount |> ignore
                    duringStat ()
                    File.GetLastWriteTimeUtc path
            )

        do
            File.WriteAllBytes(path, Array.empty)
            File.SetLastWriteTimeUtc(path, t0)

        member _.Path = path
        member _.Tracker = tracker
        member _.Stamps = tracker :> IReferenceStamps
        member _.Context = watcher.Context.Value
        member _.StatCount = statCount

        member _.DuringStat
            with set value = duringStat <- value

        member _.Watch() = tracker.StartWatchingReference path

        member _.Read() =
            (tracker :> IReferenceStamps).GetLastWriteTimeUtc path

        member _.Touch(time: DateTime) = File.SetLastWriteTimeUtc(path, time)

        interface IDisposable with
            member _.Dispose() =
                tracker.Dispose()
                File.Delete path

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

    let private statRecorder () =
        let stat = ResizeArray<string>()

        stat,
        fun path ->
            stat.Add path
            DateTime.UtcNow

    [<Fact>]
    let ``A change notification for an unwatched path is ignored`` () =
        let watcher = MockFileChangeWatcher()
        let stat, getLastWriteTimeUtc = statRecorder ()
        use tracker = new FSharpReferenceChangeTracker(watcher, getLastWriteTimeUtc)
        let stamps = tracker :> IReferenceStamps
        tracker.StartWatchingReference @"C:\x\a.dll"

        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        watcher.Context.Value.Fire @"C:\x\unwatched.dll"

        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\unwatched.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\unwatched.dll" |> ignore

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\unwatched.dll"; @"C:\x\unwatched.dll" ], List.ofSeq stat)

    [<Fact>]
    let ``Disposed tracker stats directly and ignores notifications`` () =
        let watcher = MockFileChangeWatcher()
        let stat, getLastWriteTimeUtc = statRecorder ()
        let tracker = new FSharpReferenceChangeTracker(watcher, getLastWriteTimeUtc)
        let stamps = tracker :> IReferenceStamps
        tracker.StartWatchingReference @"C:\x\a.dll"
        let context = watcher.Context.Value
        tracker.Dispose()

        context.Fire @"C:\x\a.dll"
        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore
        stamps.GetLastWriteTimeUtc @"C:\x\a.dll" |> ignore

        Assert.Equal<string list>([ @"C:\x\a.dll"; @"C:\x\a.dll" ], List.ofSeq stat)

    [<Fact>]
    let ``Watched path is served from the cache until a change notification`` () =
        use file = new Tracked()
        file.Watch()
        Assert.Equal(t0, file.Read())

        file.Touch t1
        Assert.Equal(t0, file.Read())

        file.Context.Fire file.Path
        Assert.Equal(t1, file.Read())

    [<Fact>]
    let ``Unwatched path is stat'd on every read`` () =
        use file = new Tracked()
        Assert.Equal(t0, file.Read())

        file.Touch t1
        Assert.Equal(t1, file.Read())

    [<Fact>]
    let ``Invalidate drops the cached stamp`` () =
        use file = new Tracked()
        file.Watch()
        Assert.Equal(t0, file.Read())

        file.Touch t1
        file.Stamps.Invalidate file.Path
        Assert.Equal(t1, file.Read())

    [<Fact>]
    let ``Stopping the last watch on a path falls back to stat`` () =
        use file = new Tracked()
        file.Watch()
        file.Watch()
        Assert.Equal(t0, file.Read())

        file.Touch t1
        file.Tracker.StopWatchingReference file.Path
        Assert.Equal(t0, file.Read())

        file.Tracker.StopWatchingReference file.Path
        Assert.Equal(t1, file.Read())

    [<Fact>]
    let ``A pending watch always stats fresh; caching begins only once it becomes active`` () =
        use file = new Tracked()
        file.Watch()
        file.Context.SetActive(file.Path, false)
        Assert.Equal(t0, file.Read())

        file.Touch t1
        Assert.Equal(t1, file.Read())

        file.Context.SetActive(file.Path, true)
        Assert.Equal(t1, file.Read())

        file.Touch t0
        Assert.Equal(t1, file.Read())

    let private assertNotCachedWhenDuringStat (interfere: Tracked -> unit) =
        use file = new Tracked()
        file.Watch()

        file.DuringStat <- fun () -> interfere file
        Assert.Equal(t0, file.Read())
        file.DuringStat <- ignore

        file.Touch t1
        Assert.Equal(t1, file.Read())
        Assert.Equal(t1, file.Read())
        Assert.Equal(2, file.StatCount)

    [<Fact>]
    let ``A change reported while a stat is in flight is not cached`` () =
        assertNotCachedWhenDuringStat (fun file -> file.Context.Fire file.Path)

    [<Fact>]
    let ``Invalidate while a stat is in flight is not cached`` () =
        assertNotCachedWhenDuringStat (fun file -> file.Stamps.Invalidate file.Path)

    [<Fact>]
    let ``Restarting the watch while a stat is in flight leaves the new watch cold`` () =
        assertNotCachedWhenDuringStat (fun file ->
            file.Tracker.StopWatchingReference file.Path
            file.Watch())

    [<Fact>]
    let ``Concurrent cold readers both stat and the stamp is then served from the cache`` () =
        use file = new Tracked()
        file.Watch()
        use barrier = new Barrier(2)
        let met = ref true

        file.DuringStat <-
            fun () ->
                if not (barrier.SignalAndWait(TimeSpan.FromSeconds 30.)) then
                    met.Value <- false

        // Both readers meet inside the stat, which they can only do if neither holds the tracker's lock there.
        let readers = [ for _ in 1..2 -> OwnThread.start (fun () -> file.Read() |> ignore) ]

        for reader in readers do
            reader.Join()

        Assert.True met.Value
        Assert.Equal(2, file.StatCount)

        file.DuringStat <- ignore
        Assert.Equal(t0, file.Read())
        Assert.Equal(2, file.StatCount)

    [<Fact>]
    let ``The stat does not hold the tracker lock`` () =
        use file = new Tracked()
        file.Watch()
        let otherPathReachable = ref false

        file.DuringStat <-
            fun () ->
                let other = OwnThread.start (fun () -> file.Stamps.Invalidate @"C:\x\other.dll")

                otherPathReachable.Value <- other.Join(TimeSpan.FromSeconds 30.)

        Assert.Equal(t0, file.Read())
        Assert.True otherPathReachable.Value

    type private RecordingStamps() =
        let invalidated = ResizeArray<string>()

        member _.Invalidated = List.ofSeq invalidated

        interface IReferenceStamps with
            member _.GetLastWriteTimeUtc _ = failwith "not expected"
            member _.Invalidate path = invalidated.Add path

    let private aDll, bDll, cDll = @"C:\x\a.dll", @"C:\x\b.dll", @"C:\x\c.dll"

    let private onDisk entries : ReferenceOnDisk list =
        [ for path, stamp in entries -> { Path = path; LastModified = stamp } ]

    let private assertChanged previous current expectedInvalidated =
        let stamps = RecordingStamps()
        Assert.True(ReferenceStamps.changed stamps (onDisk current) (onDisk previous))
        Assert.Equal<string list>(expectedInvalidated, stamps.Invalidated)

    [<Fact>]
    let ``Equal reference lists are unchanged and invalidate nothing`` () =
        let stamps = RecordingStamps()
        let references = onDisk [ aDll, t0; bDll, t0 ]

        Assert.False(ReferenceStamps.changed stamps references references)
        Assert.Empty stamps.Invalidated

    [<Fact>]
    let ``A different stamp on the same path is a change that invalidates only that path`` () =
        assertChanged [ aDll, t0; bDll, t0; cDll, t0 ] [ aDll, t0; bDll, t1; cDll, t0 ] [ bDll ]

    [<Fact>]
    let ``Two stale stamps are both invalidated`` () =
        assertChanged [ aDll, t0; bDll, t0; cDll, t0 ] [ aDll, t1; bDll, t0; cDll, t1 ] [ aDll; cDll ]

    [<Fact>]
    let ``A different number of references is a change that invalidates nothing`` () =
        assertChanged [ aDll, t0; bDll, t0 ] [ aDll, t0 ] []

    [<Fact>]
    let ``A different path at the same position is a change that invalidates nothing`` () =
        assertChanged [ aDll, t0; bDll, t0 ] [ aDll, t0; cDll, t1 ] []
