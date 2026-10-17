// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.MultiTargetNavigateToSearchTests

open System.IO
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

let private exactNamesIn = NavigateToSearchServiceTests.exactNamesIn

let private syntheticProject =
    SyntheticProject.Create(
        { sourceFile "Plain" [] with
            ExtraSource = "let plainUse = 1"
        },
        { sourceFile "Conditional" [] with
            ExtraSource = "#if FOO\nlet fooUse = 1\n#endif"
        }
    )

let private linkedText path (text: string) (solution: Solution) =
    let linked = TextAndVersion.Create(SourceText.From text, VersionStamp.Create())

    (solution, solution.GetDocumentIdsWithFilePath path)
    ||> Seq.fold (fun (solution: Solution) id -> solution.WithDocumentText(id, linked))

[<AutoOpen>]
module private Fixture =
    let struct (unlinked, instances) =
        RoslynTestHelpers.CreateMultiTargetSolution(
            syntheticProject,
            [ for defs in [ []; [ "FOO" ] ] -> { Defines = defs; ExcludedFileIds = [] } ]
        )

    let solution =
        (unlinked, syntheticProject.SourceFilePaths)
        ||> List.fold (fun solution path -> linkedText path (File.ReadAllText path) solution)

    let withoutFoo, withFoo = instances[0], instances[1]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``an instance reports a shared declaration on its own, and again once the parse is cached`` (instance: int) =
    let project = solution.GetProject instances[instance]

    Assert.Equal<string list>([ "plainUse" ], exactNamesIn project "plainUse")
    Assert.Equal<string list>([ "plainUse" ], exactNamesIn project "plainUse")

[<Fact>]
let ``an instance reuses the parse another instance made of the same text`` () =
    let path = syntheticProject.GetFilePath "Plain"
    let linked = linkedText path "module ModulePlain\nlet reusedUse = 1\n" solution

    let searchEveryInstance () =
        for instance in instances do
            Assert.Equal<string list>([ "reusedUse" ], exactNamesIn (linked.GetProject instance) "reusedUse")

    Assert.Equal(1, NavigateToSearchServiceTests.countParseRequests path searchEveryInstance)

[<Fact>]
let ``a declaration behind a directive is reported only by the instance that defines it`` () =
    Assert.Equal<string list>([ "fooUse" ], exactNamesIn (solution.GetProject withFoo) "fooUse")
    Assert.Equal<string list>([], exactNamesIn (solution.GetProject withoutFoo) "fooUse")

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a declaration an edit puts behind a directive is reported by the instance that defines it`` (fooFirst: bool) =
    Assert.Equal<string list>([ "plainUse" ], exactNamesIn (solution.GetProject withoutFoo) "plainUse")

    let edited =
        solution.WithDocumentText(
            solution.GetDocumentIdsWithFilePath(syntheticProject.GetFilePath "Plain"),
            SourceText.From "module ModulePlain\n#if FOO\nlet addedFoo = 1\n#endif\n"
        )

    let searches = [ withoutFoo, []; withFoo, [ "addedFoo" ] ]

    for instance, expected in (if fooFirst then List.rev searches else searches) do
        Assert.Equal<string list>(expected, exactNamesIn (edited.GetProject instance) "addedFoo")
