// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Navigate To runs a search of its own while the solution is still loading, and nothing it searches
/// then has its compilation options yet.
module FSharp.Editor.Tests.NavigateToSearchWhileLoadingTests

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Xunit

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo

open FSharp.Editor.Tests.Helpers

/// A project the project system has not yet handed its command line options: the state every project
/// of the solution is in until it has loaded.
let private loadingProject name source =
    let projectId = ProjectId.CreateNewId()

    [ RoslynTestHelpers.CreateDocumentInfo projectId $"C:\\{name}.fs" source ]
    |> RoslynTestHelpers.CreateProjectInfo projectId $"C:\\{name}.fsproj"

let private loadingSolution source =
    let project = loadingProject "test" source
    let solution = RoslynTestHelpers.CreateSolution [ project ]
    project.Id, solution, solution.Projects |> Seq.exactlyOne

/// One export provider per test: the navigable items are cached in a shared one.
let private searchServices () =
    let service: IFSharpNavigateToSearchService =
        MefHelpers.createExportProvider().GetExportedValue()

    service, service :?> IFSharpAdvancedNavigateToSearchService

let private namesFound (results: ImmutableArray<FSharpNavigateToSearchResult>) = results |> Seq.map _.Name |> Seq.toList

/// The loading search as the searcher drives it: results and project completions arrive through callbacks.
/// Returns the names found and how many times a project was reported complete.
let private searchWhileLoading
    (service: IFSharpNavigateToSearchService, advanced: IFSharpAdvancedNavigateToSearchService)
    (projects: Project list)
    pattern
    =
    task {
        let found = ResizeArray()
        let completed = ref 0

        do!
            advanced.SearchCachedDocumentsAsync(
                (List.head projects).Solution,
                ImmutableArray.CreateRange projects,
                ImmutableArray.Empty,
                pattern,
                service.KindsProvided,
                null,
                (fun results ->
                    lock found (fun () -> found.AddRange results)
                    Task.CompletedTask),
                (fun () ->
                    Interlocked.Increment &completed.contents |> ignore
                    Task.CompletedTask),
                CancellationToken.None
            )

        return found |> Seq.map _.Name |> Seq.toList, completed.Value
    }

[<Fact>]
let ``the loading search finds what the search that waits for the options cannot`` () : Task =
    task {
        let _, _, project =
            loadingSolution "module Sample =\n    let declaredWhileLoading = 1\n"

        let (service, _) as services = searchServices ()

        let searchAccurately () =
            service.SearchProjectAsync(project, ImmutableArray.Empty, "declaredWhileLoading", service.KindsProvided, CancellationToken.None)
            :> Task

        let! _ = Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> searchAccurately ())

        let! names, completed = searchWhileLoading services [ project ] "declaredWhileLoading"

        Assert.Equal<string list>([ "declaredWhileLoading" ], names)
        Assert.Equal(1, completed)

        // What the loading search left in the cache must not be handed to the accurate search: it was
        // read without the project's defines.
        let! _ = Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> searchAccurately ())
        ()
    }

[<Fact>]
let ``the loading search reads a file under the wrong defines and does not keep the answer`` () : Task =
    task {
        let projectId, solution, project =
            loadingSolution "#if FOO\nlet fooOnly = 1\n#endif\n"

        let (service, _) as services = searchServices ()

        let! namesWhileLoading, _ = searchWhileLoading services [ project ] "fooOnly"
        Assert.Equal<string list>([], namesWhileLoading)

        { RoslynTestHelpers.DefaultProjectOptions with
            OtherOptions = [| "--define:FOO" |]
        }
        |> RoslynTestHelpers.SetProjectOptions projectId solution

        let! found = service.SearchProjectAsync(project, ImmutableArray.Empty, "fooOnly", service.KindsProvided, CancellationToken.None)

        Assert.Equal<string list>([ "fooOnly" ], namesFound found)
    }

[<Fact>]
let ``every project is reported complete once, whether or not anything is found in it`` () : Task =
    task {
        let solution =
            RoslynTestHelpers.CreateSolution
                [
                    loadingProject "first" "let found = 1\n"
                    loadingProject "second" "let other = 2\n"
                ]

        let! names, completed = searchWhileLoading (searchServices ()) (solution.Projects |> Seq.toList) "found"

        Assert.Equal<string list>([ "found" ], names)
        Assert.Equal(2, completed)
    }
