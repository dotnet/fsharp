// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

type private GatedTextLoader(text: string, release: Task) =
    inherit TextLoader()

    let asked =
        TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

    member _.Asked = asked.Task

    override _.LoadTextAndVersionAsync(_options: LoadTextOptions, _cancellationToken: CancellationToken) : Task<TextAndVersion> =
        task {
            asked.TrySetResult() |> ignore
            do! release
            return TextAndVersion.Create(SourceText.From text, VersionStamp.Create())
        }

let private createProject () =
    RoslynTestHelpers.CreateSolution("module Test").Projects |> Seq.exactlyOne

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

let private waitForRemovalsAfter (removalsBefore: int) (projectId: ProjectId) =
    let removed () =
        ProjectSnapshotCache.removalsSoFar () <> removalsBefore
        && not (ProjectSnapshotCache.latest.ContainsKey projectId)

    Assert.True(SpinWait.SpinUntil(removed, TimeSpan.FromSeconds 60.))

let private removeFromWorkspace (project: Project) =
    let workspace = project.Solution.Workspace
    let removalsBefore = ProjectSnapshotCache.removalsSoFar ()
    Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject project.Id))
    waitForRemovalsAfter removalsBefore project.Id

let private reloadWorkspace (workspace: AdhocWorkspace) (projectInfo: ProjectInfo) =
    let removalsBefore = ProjectSnapshotCache.removalsSoFar ()
    workspace.ClearSolution()

    workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(), "test.sln", [ projectInfo ]))
    |> ignore

    waitForRemovalsAfter removalsBefore projectInfo.Id

/// Runs `disturb` while the project's first snapshot request waits for the text of its document.
let private snapshotBuiltWhile (disturb: Project -> ProjectInfo -> unit) =
    task {
        let release =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let loader, projectInfo, project = createProjectWithGatedText release.Task
        let pending = Task.Run(Func<Task<_>>(fun () -> snapshotOf project))
        do! loader.Asked

        disturb project projectInfo
        release.SetResult()

        let! built = pending
        return project, box built
    }

let private assertForgotten (project: Project) (previous: obj) =
    task {
        Assert.False(ProjectSnapshotCache.latest.ContainsKey project.Id)

        let! rebuilt = snapshotOf project
        Assert.NotSame(previous, rebuilt)
    }

[<Fact>]
let ``A project's snapshots are forgotten when it leaves the workspace, and a later request keeps none`` () : Task =
    task {
        let project = createProject ()

        let! first = snapshotOf project
        let! again = snapshotOf project
        Assert.Same(first, again)

        removeFromWorkspace project
        do! assertForgotten project first
        Assert.False(ProjectSnapshotCache.latest.ContainsKey project.Id)
    }

[<Fact>]
let ``A snapshot being built when its project leaves the workspace is not kept`` () : Task =
    task {
        let! project, built = snapshotBuiltWhile (fun project _ -> removeFromWorkspace project)
        do! assertForgotten project built
    }

[<Fact>]
let ``A snapshot being built when the solution is reloaded under the same project ids is not kept`` () : Task =
    task {
        let! project, built =
            snapshotBuiltWhile (fun project projectInfo -> reloadWorkspace (project.Solution.Workspace :?> AdhocWorkspace) projectInfo)

        Assert.True(project.Solution.Workspace.CurrentSolution.ContainsProject project.Id)
        do! assertForgotten project built
    }
