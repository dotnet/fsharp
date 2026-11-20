// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers

module PublicationGateTests =

    let private tryPublish (gate: PublicationGate) epoch =
        let ran = ref false
        let published = gate.TryPublish(epoch, fun () -> ran.Value <- true)
        published, ran.Value

    [<Fact>]
    let ``TryPublish runs the action while the epoch is unchanged`` () =
        let gate = PublicationGate()

        Assert.Equal((true, true), tryPublish gate gate.Current)

    [<Fact>]
    let ``TryPublish after Bump neither runs the action nor reports success`` () =
        let gate = PublicationGate()
        let epoch = gate.Current

        gate.Bump ignore

        Assert.Equal((false, false), tryPublish gate epoch)

    [<Fact>]
    let ``Bump and TryPublish exclude each other`` () =
        let gate = PublicationGate()
        let epoch = gate.Current
        use clearing = new ManualResetEventSlim(false)
        use release = new ManualResetEventSlim(false)

        let bump =
            OwnThread.start (fun () ->
                gate.Bump(fun () ->
                    clearing.Set()
                    release.Wait()))

        Assert.True(clearing.Wait(TimeSpan.FromSeconds 30.))

        let published = ref true

        let publish =
            OwnThread.start (fun () -> published.Value <- gate.TryPublish(epoch, ignore))

        Assert.False(publish.Join(TimeSpan.FromMilliseconds 200.))

        release.Set()
        bump.Join()
        publish.Join()
        Assert.False published.Value

/// The MEF-created service hard-wires the real watcher, so these tests build the manager with a mock one.
module FSharpProjectOptionsManagerWatchTests =

    let private referenceDll = typeof<FSharpChecker>.Assembly.Location
    let private otherDll = typeof<Project>.Assembly.Location

    let private metadataReference path =
        MetadataReference.CreateFromFile path :> MetadataReference

    let private snapshotReuse enabled =
        { AdvancedOptions.Default with
            UseTransparentCompiler = true
            TransparentCompilerSnapshotReuse = enabled
        }

    let private setSnapshotReuse (project: Project) enabled =
        RoslynTestHelpers.SetEditorOptions project.Solution (snapshotReuse enabled)

    let private createProject (references: string list) =
        let projectId = ProjectId.CreateNewId()

        let document =
            RoslynTestHelpers.CreateDocumentInfo projectId "C:\\test.fs" "module Test"

        let projectInfo =
            (RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj" [ document ])
                .WithMetadataReferences(references |> List.map metadataReference)

        let solution = RoslynTestHelpers.CreateSolution [ projectInfo ]
        RoslynTestHelpers.SetEditorOptions solution (snapshotReuse true)
        solution.GetProject projectId

    let private createManager (project: Project) =
        let watcher = MockFileChangeWatcher()

        let manager =
            FSharpProjectOptionsManager(FSharpChecker.Create(useTransparentCompiler = true), project.Solution.Workspace, watcher)

        manager.SetCommandLineOptions(project.Id, [| "C:\\test.fs" |], ImmutableArray<string>.Empty)
        watcher, manager

    let private optionsOf (manager: FSharpProjectOptionsManager) (project: Project) =
        manager.TryGetOptionsByProject(project, CancellationToken.None)

    let private watchedFiles (watcher: MockFileChangeWatcher) =
        match watcher.Context with
        | ValueSome context -> context.WatchedFiles
        | ValueNone -> []

    let private applyChange (project: Project) (change: Solution -> Solution) =
        let workspace = project.Solution.Workspace
        Assert.True(workspace.TryApplyChanges(change workspace.CurrentSolution))
        workspace.CurrentSolution.GetProject project.Id

    let private withReferences (references: string list) (project: Project) =
        applyChange project (fun solution -> solution.WithProjectMetadataReferences(project.Id, references |> List.map metadataReference))

    /// What the next request finds after an edit: a new instance of the project with its version unmoved.
    let private afterEdit (project: Project) =
        let edited =
            applyChange project (fun solution ->
                solution.WithDocumentText((Seq.exactlyOne project.Documents).Id, SourceText.From "module Test\nlet edited = 1"))

        Assert.NotSame(project, edited)
        Assert.Equal(project.Version, edited.Version)
        edited

    [<Fact>]
    let ``Computing options watches the project's references`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project

            let! options = optionsOf manager project

            Assert.True options.IsSome
            Assert.Equal<string list>([ referenceDll ], watchedFiles watcher)
        }

    [<Fact>]
    let ``References are not watched when snapshot reuse is off`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            setSnapshotReuse project false
            let watcher, manager = createManager project

            let! options = optionsOf manager project

            Assert.True options.IsSome
            Assert.Empty(watchedFiles watcher)
        }

    let private watchedAfterToggling before after =
        task {
            let project = createProject [ referenceDll ]
            setSnapshotReuse project before
            let watcher, manager = createManager project
            let! _ = optionsOf manager project

            setSnapshotReuse project after
            let! options = optionsOf manager (afterEdit project)

            Assert.True options.IsSome
            return watchedFiles watcher
        }

    [<Fact>]
    let ``Turning snapshot reuse off releases the watches although the options stay cached`` () : Task =
        task {
            let! watched = watchedAfterToggling true false

            Assert.Empty watched
        }

    [<Fact>]
    let ``Turning snapshot reuse on watches the references of options cached without it`` () : Task =
        task {
            let! watched = watchedAfterToggling false true

            Assert.Equal<string list>([ referenceDll ], watched)
        }

    [<Fact>]
    let ``ClearAllCaches releases every reference watch and forgets the project`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = optionsOf manager project

            manager.ClearAllCaches()
            let! options = optionsOf manager project

            Assert.Empty(watchedFiles watcher)
            Assert.True options.IsNone
        }

    [<Fact>]
    let ``Removing a project releases its reference watches`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = optionsOf manager project

            let workspace = project.Solution.Workspace
            Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject project.Id))
            Assert.True(SpinWait.SpinUntil((fun () -> List.isEmpty (watchedFiles watcher)), TimeSpan.FromSeconds 60.))

            let! options = optionsOf manager project
            Assert.True options.IsNone
        }

    [<Fact>]
    let ``Recomputing with a different reference set swaps the watch`` () : Task =
        task {
            let project = createProject [ referenceDll ]
            let watcher, manager = createManager project
            let! _ = optionsOf manager project

            let! options = optionsOf manager (withReferences [ otherDll ] project)

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
                let stamps = manager.ReferenceStamps
                let! _ = optionsOf manager project

                let stale = stamps.GetLastWriteTimeUtc path
                let fresh = stale + TimeSpan.FromSeconds 2.
                File.SetLastWriteTimeUtc(path, fresh)
                Assert.Equal(stale, stamps.GetLastWriteTimeUtc path)

                let! _ = optionsOf manager (withReferences [ path ] project)

                Assert.Equal(fresh, stamps.GetLastWriteTimeUtc path)
            finally
                File.Delete path
        }
