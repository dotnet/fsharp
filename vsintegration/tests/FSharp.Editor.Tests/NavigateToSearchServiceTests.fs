// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open System.Threading
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
    let ``results carry the counts Navigate To sorts equal matches by`` () =
        let result = navigateToSearch "+>" |> Seq.find (fun i -> i.Name = "+>")
        Assert.Equal((2, 0), (result.ParameterCount, result.TypeParameterCount))

    /// The text the results window highlights in a result's name.
    let matchedTextOf pattern expected =
        let result = navigateToSearch pattern |> Seq.find (fun i -> i.Name = expected)

        [
            for span in result.NameMatchSpans -> result.Name.Substring(span.Start, span.Length)
        ]

    [<Fact>]
    let ``the highlighted spans are the camel case humps that matched`` () =
        Assert.Equal<string list>([ "C"; "L"; "N" ], matchedTextOf "CLN" "CamelCaseLongName")

    [<Fact>]
    let ``the highlighted spans are offsets in the name, not in the candidate the matcher saw`` () =
        Assert.Equal<string list>([ "Delta" ], matchedTextOf "Delta" "Delta")

    [<Fact>]
    let ``a backticked identifier highlights the matched substring`` () =
        Assert.Equal<string list>([ "few" ], matchedTextOf "few" "a few words")
