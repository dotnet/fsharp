// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading
open Xunit
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
    let mutable context: MockFileChangeContext option = None

    member _.Context = context

    interface IFSharpFileChangeWatcher with
        member _.CreateContext _ =
            let ctx = new MockFileChangeContext()
            context <- Some ctx
            ctx :> IFSharpFileChangeContext

module FileChangeWatcherTests =

    let private testDelay = TimeSpan.FromMilliseconds 50.

    [<Fact>]
    let ``WatchedDirectory covers files under it matching the extension filter`` () =
        let dirs = [ WatchedDirectory(@"C:\refs", [ ".dll" ]) ]

        Assert.True(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refs\sub\a.dll"))
        Assert.True(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\REFS\A.DLL"))
        Assert.False(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\refs\a.xml"))
        Assert.False(WatchedDirectory.FilePathCoveredByWatchedDirectories(dirs, @"C:\other\a.dll"))

    [<Fact>]
    let ``WatchedDirectory without filters covers any file under it`` () =
        let dirs = [ WatchedDirectory(@"C:\refs", []) ]

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
