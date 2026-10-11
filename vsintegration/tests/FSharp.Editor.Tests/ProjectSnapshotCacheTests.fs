// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The snapshot caches hold the Roslyn project each snapshot was built from, so they have to follow the
/// workspace: a project that leaves it takes its entries along.
module FSharp.Editor.Tests.ProjectSnapshotCacheTests

open System
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers

let private createProject () =
    RoslynTestHelpers.CreateSolution("module Test").Projects |> Seq.exactlyOne

let private snapshotOf (project: Project) =
    project.GetFSharpProjectSnapshot()
    |> CancellableTask.start CancellationToken.None

/// Removes the project and waits for the caches to hear of it: the workspace raises its change events on a
/// queue of its own.
let private removeFromWorkspace (project: Project) =
    let workspace = project.Solution.Workspace
    Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject project.Id))

    Assert.True(SpinWait.SpinUntil((fun () -> not (ProjectSnapshotCache.latest.ContainsKey project.Id)), TimeSpan.FromSeconds 10.))

[<Fact>]
let ``A project's snapshot is forgotten when the project leaves the workspace`` () : Task =
    task {
        let project = createProject ()

        let! first = snapshotOf project
        let! again = snapshotOf project
        Assert.Same(first, again)
        Assert.True(ProjectSnapshotCache.latest.ContainsKey project.Id)

        removeFromWorkspace project

        // Asked again, the same Roslyn project gets a snapshot built anew: the one it had is gone.
        let! afterRemoval = snapshotOf project
        Assert.NotSame(first, afterRemoval)
    }

[<Fact>]
let ``A snapshot built for a project that has left the workspace is not kept`` () : Task =
    task {
        let project = createProject ()
        let! _ = snapshotOf project
        removeFromWorkspace project

        // Work under way for a project when it leaves still finishes, and must not bring its entries back.
        let! late = snapshotOf project
        Assert.False(ProjectSnapshotCache.latest.ContainsKey project.Id)

        let! later = snapshotOf project
        Assert.NotSame(late, later)
    }
