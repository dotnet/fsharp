// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open System.Diagnostics
open System.Threading
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

        let isParseOfScript (activity: Activity) =
            activity.OperationName = "BackgroundCompiler.ParseFile"
            && activity.Tags |> Seq.exists (fun tag -> tag.Value = path)

        let parseRequests = ref 0

        use listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample = (fun _ -> ActivitySamplingResult.AllData),
                ActivityStarted =
                    (fun activity ->
                        if isParseOfScript activity then
                            Interlocked.Increment(&parseRequests.contents) |> ignore)
            )

        ActivitySource.AddActivityListener listener

        let script = solution.GetProject projectId

        Assert.Equal<string list>([ "standaloneValue" ], exactNamesIn script "standaloneValue")
        Assert.Equal<string list>([ "standaloneValue" ], exactNamesIn script "standaloneValue")
        Assert.Equal(1, parseRequests.Value)
