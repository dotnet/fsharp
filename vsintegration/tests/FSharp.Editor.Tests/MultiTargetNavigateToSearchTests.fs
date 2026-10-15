// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.MultiTargetNavigateToSearchTests

open Xunit
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

[<AutoOpen>]
module private Fixture =
    let struct (solution, instances) =
        RoslynTestHelpers.CreateMultiTargetSolution(
            syntheticProject,
            [ for defs in [ []; [ "FOO" ] ] -> { Defines = defs; ExcludedFileIds = [] } ]
        )

    let withoutFoo, withFoo = instances[0], instances[1]

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``an instance reports a shared declaration on its own, and again once the parse is cached`` (instance: int) =
    let project = solution.GetProject instances[instance]

    Assert.Equal<string list>([ "plainUse" ], exactNamesIn project "plainUse")
    Assert.Equal<string list>([ "plainUse" ], exactNamesIn project "plainUse")

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
