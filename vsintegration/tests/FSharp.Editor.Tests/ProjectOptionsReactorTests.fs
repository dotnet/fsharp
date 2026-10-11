// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open CancellableTasks
open FSharp.Editor.Tests.Helpers

module ProjectOptionsReactorTests =

    let private countingAction () =
        let calls = ref 0

        let action () =
            Interlocked.Increment(&calls.contents) |> ignore
            Task.FromResult()

        action, (fun () -> Volatile.Read(&calls.contents))

    let private countingDebouncer (delay: TimeSpan) =
        let action, calls = countingAction ()
        new Debouncer(delay, action), calls

    [<Fact>]
    let ``Rapid triggers run the action once`` () : Task =
        task {
            let debouncer, calls = countingDebouncer (TimeSpan.FromMilliseconds 50.)
            use _ = debouncer

            let pending = [| for _ in 1..10 -> debouncer.Trigger() |]
            do! Task.WhenAll pending

            Assert.Equal(1, calls ())
        }

    [<Fact>]
    let ``Triggers further apart than the delay each run the action`` () : Task =
        task {
            let debouncer, calls = countingDebouncer (TimeSpan.FromMilliseconds 10.)
            use _ = debouncer

            do! debouncer.Trigger()
            do! debouncer.Trigger()

            Assert.Equal(2, calls ())
        }

    [<Fact>]
    let ``Disposing cancels the pending action`` () : Task =
        task {
            let debouncer, calls = countingDebouncer (TimeSpan.FromSeconds 30.)
            let pending = debouncer.Trigger()

            (debouncer :> IDisposable).Dispose()
            do! pending

            Assert.Equal(0, calls ())
        }

    [<Fact>]
    let ``Disposing after the delay has elapsed keeps the action from starting`` () : Task =
        task {
            let delayElapsed =
                TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

            let disposed =
                TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

            let action, calls = countingAction ()

            let debouncer =
                new Debouncer(
                    (fun token ->
                        task {
                            do! Task.Delay(TimeSpan.FromMilliseconds 10., token)
                            delayElapsed.SetResult()
                            do! disposed.Task
                        }
                        :> Task),
                    action
                )

            let pending = debouncer.Trigger()
            do! delayElapsed.Task

            (debouncer :> IDisposable).Dispose()
            disposed.SetResult()
            do! pending

            Assert.Equal(0, calls ())
        }

    let private referencingSolution () =
        let referencedId = ProjectId.CreateNewId()
        let referencingId = ProjectId.CreateNewId()

        let referencedDocument =
            RoslynTestHelpers.CreateDocumentInfo referencedId "C:\\Referenced.fs" "module Referenced\nlet x = 1"

        let referenced =
            RoslynTestHelpers.CreateProjectInfo referencedId "C:\\Referenced.fsproj" [ referencedDocument ]

        let referencing =
            (RoslynTestHelpers.CreateProjectInfo
                referencingId
                "C:\\Referencing.fsproj"
                [
                    RoslynTestHelpers.CreateDocumentInfo referencingId "C:\\Referencing.fs" "module Referencing"
                ])
                .WithProjectReferences([ ProjectReference(referencedId) ])

        let solution = RoslynTestHelpers.CreateSolution [ referenced; referencing ]
        solution, referencingId, referencedDocument.Id

    let private dependentVersionChanged ct (oldSolution: Solution) (newSolution: Solution) projectId =
        hasDependentVersionChanged (oldSolution.GetProject projectId) (newSolution.GetProject projectId)
        |> CancellableTask.start ct

    [<Fact>]
    let ``Unchanged references keep the dependent version`` () : Task =
        task {
            let solution, referencingId, _ = referencingSolution ()

            let! changed = dependentVersionChanged CancellationToken.None solution solution referencingId

            Assert.False changed
        }

    [<Fact>]
    let ``Adding a file to a referenced project changes the dependent version`` () : Task =
        task {
            let solution, referencingId, referencedDocumentId = referencingSolution ()

            let edited =
                solution.AddDocument(
                    DocumentId.CreateNewId referencedDocumentId.ProjectId,
                    "Added.fs",
                    SourceText.From "module Added",
                    filePath = "C:\\Added.fs"
                )

            let! changed = dependentVersionChanged CancellationToken.None solution edited referencingId

            Assert.True changed
        }

    [<Fact>]
    let ``Comparing references observes cancellation`` () : Task =
        task {
            let solution, referencingId, _ = referencingSolution ()
            use cts = new CancellationTokenSource()
            cts.Cancel()

            let! _ =
                Assert.ThrowsAnyAsync<OperationCanceledException>(fun () ->
                    dependentVersionChanged cts.Token solution solution referencingId :> Task)

            ()
        }
