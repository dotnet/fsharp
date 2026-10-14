// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Navigate To keeps the navigable items of a document in persistent storage, so that the next session reads them
/// back instead of parsing — which, before a project has its options, it cannot do at all.
module FSharp.Editor.Tests.NavigateToPersistentCacheTests

open System
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks

open Xunit

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

open FSharp.Editor.Tests.Helpers

[<Literal>]
let private ImplementationSource =
    """
namespace Persisted.Space

exception Failed of string

module Outer =
    type Shape =
        | Dot
        | Circle of radius: float

    type Point = { X: int; Y: int }

    type Colour =
        | Red = 1
        | Green = 2

    type Counter(start: int) =
        new() = Counter 0
        member val Count = start with get, set
        member this.Increment() = this.Count <- this.Count + 1

    module Inner =
        let ``needs backticks`` = 1
        let twice x = 2 * x

    module Alias = Inner
"""

[<Literal>]
let private SignatureSource =
    """
namespace Persisted.Space

module Outer =
    type Shape =
        | Dot
        | Circle of radius: float

    val twice: int -> int
"""

let private checker = FSharpChecker.Create()

let private navigableItems (fileName: string) source =
    task {
        let! results =
            checker.ParseFile(
                fileName,
                SourceText.ofString source,
                { FSharpParsingOptions.Default with
                    SourceFiles = [| fileName |]
                }
            )
            |> Async.StartAsTask

        return NavigateTo.GetNavigableItems results.ParseTree
    }

/// Runs the test against a cache directory of its own, deleted afterwards.
let private withCacheDirectory (test: string -> Task) : Task =
    task {
        let cacheDirectory =
            Path.Combine(Path.GetTempPath(), "FSharp.Editor.Tests", Guid.NewGuid().ToString "N")

        try
            do! test cacheDirectory
        finally
            if Directory.Exists cacheDirectory then
                Directory.Delete(cacheDirectory, true)
    }

/// A Visual Studio session: an export provider of its own, so nothing another session parsed is in memory, keeping
/// its data under the cache directory.
let private session cacheDirectory =
    let exportProvider = MefHelpers.createExportProvider ()
    exportProvider.GetExportedValue<FSharpPersistentStorageConfiguration>().CacheDirectory <- ValueSome cacheDirectory
    exportProvider

let private searchService cacheDirectory : IFSharpNavigateToSearchService =
    (session cacheDirectory).GetExportedValue()

/// The same solution in every session: storage finds a document by the paths and names of the solution, the project
/// and the document. None of the projects has options until a test sets them.
let private openSolution (fileName: string) source =
    let projectId = ProjectId.CreateNewId()

    let project =
        [ RoslynTestHelpers.CreateDocumentInfo projectId fileName source ]
        |> RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj"

    let solution =
        RoslynTestHelpers.CreateSolutionAt "C:\\Persisted\\test.sln" [ project ]

    projectId, solution, solution.GetProject projectId

let private search (service: IFSharpNavigateToSearchService) (project: Project) pattern =
    task {
        let! results = service.SearchProjectAsync(project, ImmutableArray.Empty, pattern, service.KindsProvided, CancellationToken.None)
        return results |> Seq.map _.Name |> Seq.toList
    }

[<Theory>]
[<InlineData("C:\\test.fs", ImplementationSource)>]
[<InlineData("C:\\test.fsi", SignatureSource)>]
let ``navigable items read back as they were written, and only with the checksum they were written with``
    (fileName: string, source: string)
    : Task =
    withCacheDirectory (fun cacheDirectory ->
        task {
            let! items = navigableItems fileName source

            let storage: IFSharpChecksummedPersistentStorageService =
                (session cacheDirectory).GetExportedValue()

            let _, _, project = openSolution fileName source
            let document = project.Documents |> Seq.exactlyOne
            let written = FSharpChecksum.Create "written"

            do! NavigableItemsIndex.save storage document written items CancellationToken.None
            let! restored = NavigableItemsIndex.tryLoad storage document written CancellationToken.None
            let! underAnotherChecksum = NavigableItemsIndex.tryLoad storage document (FSharpChecksum.Create "other") CancellationToken.None

            Assert.Equal<NavigableItem array>(items, restored |> ValueOption.defaultValue [||])
            Assert.Equal(ValueNone, underAnotherChecksum)
        })

[<Fact>]
let ``a file without directives is answered from storage before its project has options`` () : Task =
    withCacheDirectory (fun cacheDirectory ->
        task {
            let source = "module Persisted\n\nlet declaredEarlier = 1\n"

            let projectId, solution, project = openSolution "C:\\test.fs" source
            RoslynTestHelpers.SetProjectOptions projectId solution RoslynTestHelpers.DefaultProjectOptions
            let! found = search (searchService cacheDirectory) project "declaredEarlier"
            Assert.Equal<string list>([ "declaredEarlier" ], found)

            let _, _, reopened = openSolution "C:\\test.fs" source

            let! _ =
                Assert.ThrowsAnyAsync<OperationCanceledException>(fun () ->
                    search (searchService (Path.Combine(cacheDirectory, "empty"))) reopened "declaredEarlier" :> Task)

            let! found = search (searchService cacheDirectory) reopened "declaredEarlier"
            Assert.Equal<string list>([ "declaredEarlier" ], found)
        })

[<Fact>]
let ``a file with directives is answered from storage only under the defines it was parsed with`` () : Task =
    withCacheDirectory (fun cacheDirectory ->
        task {
            let source =
                "module Persisted\n\n#if FOO\nlet fooOnly = 1\n#endif\nlet everywhere = 1\n"

            let projectId, solution, project = openSolution "C:\\test.fs" source

            { RoslynTestHelpers.DefaultProjectOptions with
                OtherOptions = [| "--define:FOO" |]
            }
            |> RoslynTestHelpers.SetProjectOptions projectId solution

            let! found = search (searchService cacheDirectory) project "fooOnly"
            Assert.Equal<string list>([ "fooOnly" ], found)

            let projectId, solution, reopened = openSolution "C:\\test.fs" source
            let service = searchService cacheDirectory

            let! _ = Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> search service reopened "everywhere" :> Task)

            RoslynTestHelpers.SetProjectOptions projectId solution RoslynTestHelpers.DefaultProjectOptions
            let! found = search service reopened "fooOnly"
            Assert.Equal<string list>([], found)
        })

[<Fact>]
let ``a solution without a rooted path keeps nothing`` () : Task =
    withCacheDirectory (fun cacheDirectory ->
        task {
            let projectId = ProjectId.CreateNewId()

            let project =
                [
                    RoslynTestHelpers.CreateDocumentInfo projectId "C:\\test.fs" "module Persisted\n\nlet unkept = 1\n"
                ]
                |> RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj"

            let solution = RoslynTestHelpers.CreateSolution [ project ]
            RoslynTestHelpers.SetProjectOptions projectId solution RoslynTestHelpers.DefaultProjectOptions

            let! found = search (searchService cacheDirectory) (solution.GetProject projectId) "unkept"

            Assert.Equal<string list>([ "unkept" ], found)
            Assert.False(Directory.Exists cacheDirectory)
        })
