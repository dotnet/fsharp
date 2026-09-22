// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open System.Collections.Immutable
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

    let navigateToSearch pattern =
        let service: IFSharpNavigateToSearchService = provider.GetExportedValue()

        service.SearchProjectAsync(project, [] |> Seq.toImmutableArray, pattern, service.KindsProvided, CancellationToken.None).Result

    let assertResultsContain pattern expected =
        navigateToSearch pattern
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
    let ``a project's priority documents are searched first`` () =
        let projectId = ProjectId.CreateNewId()

        let solution =
            [
                RoslynTestHelpers.CreateDocumentInfo projectId "C:\\First.fs" "module First\n\nlet shared = 1\n"
                RoslynTestHelpers.CreateDocumentInfo projectId "C:\\Second.fs" "module Second\n\nlet shared = 2\n"
            ]
            |> RoslynTestHelpers.CreateProjectInfo projectId "C:\\test.fsproj"
            |> List.singleton
            |> RoslynTestHelpers.CreateSolution

        { RoslynTestHelpers.DefaultProjectOptions with
            SourceFiles = [| "C:\\First.fs"; "C:\\Second.fs" |]
        }
        |> RoslynTestHelpers.SetProjectOptions projectId solution

        let project = solution.GetProject projectId

        let second =
            project.Documents |> Seq.find (fun document -> document.Name = "C:\\Second.fs")

        let service: IFSharpNavigateToSearchService = provider.GetExportedValue()

        let results =
            service
                .SearchProjectAsync(project, ImmutableArray.Create second, "shared", service.KindsProvided, CancellationToken.None)
                .Result

        Assert.Equal<string list>([ "C:\\Second.fs"; "C:\\First.fs" ], results |> Seq.map _.NavigableItem.Document.Name |> Seq.toList)
