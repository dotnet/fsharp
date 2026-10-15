// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The snapshot caches hold the Roslyn project each snapshot was built from, so they have to follow the
/// workspace: a project that leaves it takes its entries along, also when it leaves while a snapshot of it is
/// being built.
module FSharp.Editor.Tests.ProjectSnapshotCacheTests

open System
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

/// Text that arrives only when the test lets it, so a snapshot request that needs it stays pending, and that
/// says when a request has got as far as asking for it.
type private GatedTextLoader(text: string, release: Task) =
    inherit TextLoader()

    let asked =
        TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

    let loaded () =
        TextAndVersion.Create(SourceText.From text, VersionStamp.Create())

    member _.Asked = asked.Task

    override _.LoadTextAndVersionAsync(_options: LoadTextOptions, _cancellationToken: CancellationToken) : Task<TextAndVersion> =
        task {
            asked.TrySetResult() |> ignore
            do! release
            return loaded ()
        }

let private createProject () =
    RoslynTestHelpers.CreateSolution("module Test").Projects |> Seq.exactlyOne

/// A project whose only document gets its text when `release` completes.
let private createProjectWithGatedText (release: Task) =
    let projectId = ProjectId.CreateNewId()
    let loader = GatedTextLoader("module Test", release)

    let document =
        DocumentInfo.Create(DocumentId.CreateNewId projectId, "test.fs", loader = loader, filePath = "C:\\test.fs")

    let projectInfo =
        RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj" [ document ]

    let solution = RoslynTestHelpers.CreateSolution [ projectInfo ]
    RoslynTestHelpers.SetProjectOptions projectId solution RoslynTestHelpers.DefaultProjectOptions
    loader, projectInfo, solution.GetProject projectId

let private snapshotOf (project: Project) =
    project.GetFSharpProjectSnapshot()
    |> CancellableTask.start CancellationToken.None

/// The workspace raises its change events on a queue of its own: waits until the caches have heard of a removal.
let private waitForRemovalAfter (removalsBefore: int) =
    Assert.True(SpinWait.SpinUntil((fun () -> ProjectSnapshotCache.removalsSoFar () <> removalsBefore), TimeSpan.FromSeconds 60.))

let private removeFromWorkspace (project: Project) =
    let workspace = project.Solution.Workspace
    let before = ProjectSnapshotCache.removalsSoFar ()
    Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject project.Id))
    waitForRemovalAfter before

[<Fact>]
let ``A project's snapshot is forgotten when the project leaves the workspace`` () : Task =
    task {
        let project = createProject ()

        let! first = snapshotOf project
        let! again = snapshotOf project
        Assert.Same(first, again)
        Assert.True(ProjectSnapshotCache.latest.ContainsKey project.Id)

        removeFromWorkspace project
        // The handler counts the removal first and forgets after: wait for the entries to be gone.
        Assert.True(SpinWait.SpinUntil((fun () -> not (ProjectSnapshotCache.latest.ContainsKey project.Id)), TimeSpan.FromSeconds 60.))

        // Asked again, the same Roslyn project gets a snapshot built anew: the one it had is gone.
        let! afterRemoval = snapshotOf project
        Assert.NotSame(first, afterRemoval)
    }

[<Fact>]
let ``A snapshot being built when its project leaves the workspace is not kept`` () : Task =
    task {
        let release =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let loader, _, project = createProjectWithGatedText release.Task

        // On a pool thread: a request starts on its caller's, which has to be free to let the text go.
        let pending = Task.Run(Func<Task<_>>(fun () -> snapshotOf project))
        do! loader.Asked

        removeFromWorkspace project
        release.SetResult()

        let! late = pending
        Assert.False(ProjectSnapshotCache.latest.ContainsKey project.Id)

        // Nothing memoized either: the same Roslyn project gets a snapshot built anew.
        let! later = snapshotOf project
        Assert.NotSame(late, later)
    }

[<Fact>]
let ``A snapshot being built when the solution is reloaded is not kept`` () : Task =
    task {
        let release =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let loader, projectInfo, project = createProjectWithGatedText release.Task
        let workspace = project.Solution.Workspace :?> AdhocWorkspace

        let pending = Task.Run(Func<Task<_>>(fun () -> snapshotOf project))
        do! loader.Asked

        // A reload brings the same project id back, so a request that asked the workspace whether the id is
        // still there would keep what it built for the project that went.
        let before = ProjectSnapshotCache.removalsSoFar ()
        workspace.ClearSolution()

        workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), "test.sln", [ projectInfo ]))
        |> ignore

        waitForRemovalAfter before
        Assert.True(workspace.CurrentSolution.ContainsProject project.Id)

        release.SetResult()
        let! late = pending
        Assert.False(ProjectSnapshotCache.latest.ContainsKey project.Id)

        let! later = snapshotOf project
        Assert.NotSame(late, later)
    }
