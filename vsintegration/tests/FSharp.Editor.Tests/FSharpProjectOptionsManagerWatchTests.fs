// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers

module PublicationGateTests =

    [<Fact>]
    let ``TryPublish runs the action while the epoch is unchanged`` () =
        let gate = PublicationGate()
        let published = ref false

        Assert.True(gate.TryPublish(gate.Current, fun () -> published.Value <- true))
        Assert.True published.Value

    [<Fact>]
    let ``TryPublish after Bump neither runs the action nor reports success`` () =
        let gate = PublicationGate()
        let epoch = gate.Current
        let published = ref false

        gate.Bump ignore

        Assert.False(gate.TryPublish(epoch, fun () -> published.Value <- true))
        Assert.False published.Value

    [<Fact>]
    let ``Bump and TryPublish exclude each other`` () =
        let gate = PublicationGate()
        let epoch = gate.Current
        use clearing = new ManualResetEventSlim(false)
        use release = new ManualResetEventSlim(false)

        let bump =
            Task.Run(fun () ->
                gate.Bump(fun () ->
                    clearing.Set()
                    release.Wait()))

        Assert.True(clearing.Wait(TimeSpan.FromSeconds 10.))

        // A publication that started under the old epoch waits for the clear, then sees the epoch moved.
        let publish = Task.Run(fun () -> gate.TryPublish(epoch, ignore))
        Assert.False(publish.Wait(TimeSpan.FromMilliseconds 200.))

        release.Set()
        bump.Wait()
        Assert.False publish.Result

/// The reactor through its manager, with the mock watcher from FileChangeWatcherTests standing in for
/// the VS file change service. The MEF-created service hard-wires the real watcher, so these tests
/// build the manager directly.
module FSharpProjectOptionsManagerWatchTests =

    let private referenceDll = typeof<FSharpChecker>.Assembly.Location
    let private otherDll = typeof<Project>.Assembly.Location

    let private metadataReference path =
        MetadataReference.CreateFromFile path :> MetadataReference

    /// Reference watching serves the snapshot-reuse guard, so the reactor only watches when that guard runs.
    let private snapshotReuseOptions =
        { AdvancedOptions.Default with
            UseTransparentCompiler = true
            TransparentCompilerSnapshotReuse = true
        }

    let private createProject (references: string list) =
        let projectId = ProjectId.CreateNewId()

        let document =
            RoslynTestHelpers.CreateDocumentInfo projectId "C:\\test.fs" "module Test"

        let projectInfo =
            (RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj" [ document ])
                .WithMetadataReferences(references |> List.map metadataReference)

        let solution = RoslynTestHelpers.CreateSolution [ projectInfo ]
        RoslynTestHelpers.SetEditorOptions solution snapshotReuseOptions
        solution.GetProject projectId

    let private createManager (project: Project) =
        let watcher = MockFileChangeWatcher()

        let manager =
            FSharpProjectOptionsManager(FSharpChecker.Create(useTransparentCompiler = true), project.Solution.Workspace, watcher)

        manager.SetCommandLineOptions(project.Id, [| "C:\\test.fs" |], ImmutableArray<string>.Empty)
        watcher, manager

    let private watchedFiles (watcher: MockFileChangeWatcher) =
        match watcher.Context with
        | ValueSome context -> context.WatchedFiles
        | ValueNone -> []

    [<Fact>]
    let ``Computing options watches the project's references`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project

            let! options = manager.TryGetOptionsByProject(project, CancellationToken.None)

            Assert.True options.IsSome
            Assert.Equal<string list>([ referenceDll ], watchedFiles watcher)
        }

    [<Fact>]
    let ``References are not watched when snapshot reuse is off`` () : Task =
        task {
            let project = createProject [ referenceDll ]

            RoslynTestHelpers.SetEditorOptions
                project.Solution
                { snapshotReuseOptions with
                    TransparentCompilerSnapshotReuse = false
                }

            let watcher, manager = createManager project

            let! options = manager.TryGetOptionsByProject(project, CancellationToken.None)

            Assert.True options.IsSome
            Assert.Empty(watchedFiles watcher)
        }

    [<Fact>]
    let ``ClearAllCaches releases every reference watch and forgets the project`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = manager.TryGetOptionsByProject(project, CancellationToken.None)

            manager.ClearAllCaches()

            Assert.Empty(watchedFiles watcher)

            // The command line options went with the caches, so the project has no options any more.
            let! options = manager.TryGetOptionsByProject(project, CancellationToken.None)
            Assert.True options.IsNone
        }

    [<Fact>]
    let ``Removing a project releases its reference watches`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = manager.TryGetOptionsByProject(project, CancellationToken.None)

            let workspace = project.Solution.Workspace
            Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject project.Id))

            // The manager's workspace handler posts ClearOptions to the agent; the watch is gone once that ran.
            Assert.True(SpinWait.SpinUntil((fun () -> List.isEmpty (watchedFiles watcher)), TimeSpan.FromSeconds 10.))

            let! options = manager.TryGetOptionsByProject(project, CancellationToken.None)
            Assert.True options.IsNone
        }

    [<Fact>]
    let ``Recomputing with a different reference set swaps the watch`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = manager.TryGetOptionsByProject(project, CancellationToken.None)

            let workspace = project.Solution.Workspace

            Assert.True(
                workspace.TryApplyChanges(
                    workspace.CurrentSolution.WithProjectMetadataReferences(project.Id, [ metadataReference otherDll ])
                )
            )

            let! options = manager.TryGetOptionsByProject(workspace.CurrentSolution.GetProject project.Id, CancellationToken.None)

            Assert.True options.IsSome
            Assert.Equal<string list>([ otherDll ], watchedFiles watcher)
        }

    [<Fact>]
    let ``A reference Roslyn swapped loses its stamp although its change notification was missed`` () : Task =
        task {
            let path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".dll")
            File.Copy(referenceDll, path)

            try
                let project = createProject [ path ]
                let _, manager = createManager project
                let! _ = manager.TryGetOptionsByProject(project, CancellationToken.None)

                let stale = manager.ReferenceStamps.GetLastWriteTimeUtc path

                // The file changes and no notification arrives: the tracker answers what it remembers.
                let fresh = stale + TimeSpan.FromSeconds 2.
                File.SetLastWriteTimeUtc(path, fresh)
                Assert.Equal(stale, manager.ReferenceStamps.GetLastWriteTimeUtc path)

                // Roslyn saw the change and put a new reference at the same path.
                let workspace = project.Solution.Workspace

                Assert.True(
                    workspace.TryApplyChanges(
                        workspace.CurrentSolution.WithProjectMetadataReferences(project.Id, [ metadataReference path ])
                    )
                )

                let! _ = manager.TryGetOptionsByProject(workspace.CurrentSolution.GetProject project.Id, CancellationToken.None)

                Assert.Equal(fresh, manager.ReferenceStamps.GetLastWriteTimeUtc path)
            finally
                File.Delete path
        }
