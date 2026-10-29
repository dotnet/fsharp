// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open System.Collections.Concurrent
open System.Collections.Immutable
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo

module NavigateToSearchServiceTests =

    let fileContents =
        """
module HeyHo =
    let inline (+>) f c = f c

    let Żółwik żwawy = ()

    let ``a few words`` = 0

    let one' = 1
    let two'' = 2

    type CamelCaseLongName = class end

    module Alpha =
        module Beta =
            type Gamma() =
                member val Delta = 3
"""

    let sourceText = SourceText.From(fileContents)

    let solution = RoslynTestHelpers.CreateSolution(fileContents)
    let project = solution.Projects |> Seq.exactlyOne
    let provider = MefHelpers.createExportProvider ()

    let navigateToSearch (project: Project) pattern =
        let service: IFSharpNavigateToSearchService = provider.GetExportedValue()

        service.SearchProjectAsync(project, [] |> Seq.toImmutableArray, pattern, service.KindsProvided, CancellationToken.None).Result

    let exactNamesIn project pattern =
        navigateToSearch project pattern
        |> Seq.map _.Name
        |> Seq.filter ((=) pattern)
        |> Seq.toList

    let countParseRequests (path: string) searches =
        let isParseOfPath (activity: Activity) =
            activity.OperationName = "BackgroundCompiler.ParseFile"
            && activity.Tags |> Seq.exists (fun tag -> tag.Value = path)

        let parseRequests = ref 0

        use listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample = (fun _ -> ActivitySamplingResult.AllData),
                ActivityStarted =
                    (fun activity ->
                        if isParseOfPath activity then
                            Interlocked.Increment(&parseRequests.contents) |> ignore)
            )

        ActivitySource.AddActivityListener listener
        searches ()
        parseRequests.Value

    let assertResultsContain pattern expected =
        navigateToSearch project pattern
        |> Seq.exists (fun i -> i.Name = expected)
        |> Assert.True

    [<Fact>]
    let ``unicode symbols`` () = assertResultsContain "Żó" "Żółwik"

    [<Fact>]
    let ``capitalized camel case`` () =
        assertResultsContain "CLN" "CamelCaseLongName"

    [<Fact>]
    let ``lower camel case`` () =
        assertResultsContain "cln" "CamelCaseLongName"

    [<Fact>]
    let ``substring`` () = assertResultsContain "ne'" "one'"

    [<Fact>]
    let ``backticked identifier`` () =
        assertResultsContain "a few words" "a few words"

    [<Fact>]
    let ``operator`` () = assertResultsContain "+>" "+>"

    [<Fact>]
    let ``nested containers`` () =
        assertResultsContain "hh.a.b.g.d" "Delta"

    [<Fact>]
    let ``a project whose options have not arrived does not answer from another project's parse`` () =
        let path = "C:\\Shared.fs"
        let definesNothingId, noOptionsId = ProjectId.CreateNewId(), ProjectId.CreateNewId()

        let file =
            RoslynTestHelpers.CreateDocumentInfo definesNothingId path "module Shared\n#if FOO\nlet fooOnly = 1\n#endif\nlet always = 2\n"

        let sameVersionFile = file.WithId(DocumentId.CreateNewId noOptionsId)

        let solution =
            RoslynTestHelpers.CreateSolution
                [
                    RoslynTestHelpers.CreateProjectInfo definesNothingId "C:\\DefinesNothing.fsproj" [ file ]
                    RoslynTestHelpers.CreateProjectInfo noOptionsId "C:\\NoOptions.fsproj" [ sameVersionFile ]
                ]

        { RoslynTestHelpers.DefaultProjectOptions with
            SourceFiles = [| path |]
        }
        |> RoslynTestHelpers.SetProjectOptions definesNothingId solution

        Assert.Equal<string list>([ "always" ], exactNamesIn (solution.GetProject definesNothingId) "always")

        let searching =
            Assert.ThrowsAny<exn>(fun () -> exactNamesIn (solution.GetProject noOptionsId) "always" |> ignore)

        Assert.IsAssignableFrom<System.OperationCanceledException>(searching.GetBaseException())

    [<Fact>]
    let ``a repeated search of an unchanged standalone script does not parse it again`` () =
        let path = "C:\\Standalone.fsx"
        let projectId = ProjectId.CreateNewId()

        let solution =
            [
                RoslynTestHelpers.CreateDocumentInfo projectId path "let standaloneValue = 1\n"
            ]
            |> RoslynTestHelpers.CreateProjectInfo projectId FSharpConstants.FSharpMiscellaneousFilesName
            |> List.singleton
            |> RoslynTestHelpers.CreateSolution

        let script = solution.GetProject projectId

        let searchTwice () =
            for _ in 1..2 do
                Assert.Equal<string list>([ "standaloneValue" ], exactNamesIn script "standaloneValue")

        Assert.Equal(1, countParseRequests path searchTwice)

    type private GatedTextLoader(path: string, onLoad: string -> Task) =
        inherit TextLoader()

        override _.LoadTextAndVersionAsync(_: LoadTextOptions, _: CancellationToken) =
            task {
                do! onLoad path
                return TextAndVersion.Create(SourceText.From "let shared = 1\n", VersionStamp.Create(), path)
            }

    [<Fact>]
    let ``a project's priority documents are reached first and do not hold back the rest`` () =
        let first, second = "C:\\First.fs", "C:\\Second.fs"
        let hasSecondWorker = System.Environment.ProcessorCount > 1
        let reached = ConcurrentQueue<string>()

        let firstReached =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let onLoad path : Task =
            reached.Enqueue path

            if path = first then
                firstReached.TrySetResult() |> ignore

            if hasSecondWorker then
                firstReached.Task
            else
                Task.CompletedTask

        let projectId = ProjectId.CreateNewId()

        let documentOf path =
            DocumentInfo.Create(DocumentId.CreateNewId projectId, path, loader = GatedTextLoader(path, onLoad), filePath = path)

        let solution =
            [ documentOf first; documentOf second ]
            |> RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj"
            |> List.singleton
            |> RoslynTestHelpers.CreateSolution

        { RoslynTestHelpers.DefaultProjectOptions with
            SourceFiles = [| first; second |]
        }
        |> RoslynTestHelpers.SetProjectOptions projectId solution

        let project = solution.GetProject projectId

        let priority =
            project.Documents |> Seq.find (fun document -> document.FilePath = second)

        let service: IFSharpNavigateToSearchService = provider.GetExportedValue()

        let search =
            service.SearchProjectAsync(project, ImmutableArray.Create priority, "shared", service.KindsProvided, CancellationToken.None)

        Assert.True(search.Wait(System.TimeSpan.FromMinutes 1.0), "The rest of the project waited for the priority document.")
        Assert.Equal(second, reached |> Seq.head)
