// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.FSharp.Editor

type private MockFileChangeContext() =
    let fileChanged = Event<string>()
    let watched = ResizeArray<string>()

    member _.WatchedFiles = List.ofSeq watched
    member _.Fire path = fileChanged.Trigger path

    interface IFSharpFileChangeContext with
        [<CLIEvent>]
        member _.FileChanged = fileChanged.Publish

        member _.EnqueueWatchingFile path =
            watched.Add path

            { new IFSharpWatchedFile with
                member _.Dispose() = watched.Remove path |> ignore
            }

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

/// Stands in for IVsAsyncFileChangeEx2: hands out sequential cookies and records every call, so a
/// test can see how the watcher turned its queue into service calls.
type private RecordingFileChangeService() =
    let calls = ResizeArray<ServiceCall>()
    let mutable nextCookie = 0u

    let record call = lock calls (fun () -> calls.Add call)

    let newCookie () =
        nextCookie <- nextCookie + 1u
        nextCookie

    member _.Calls = lock calls (fun () -> List.ofSeq calls)

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
