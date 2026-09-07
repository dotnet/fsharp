// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// One project loaded as two target-framework instances: `plain` compiles without the fourth file
/// and without FOO, `foo` compiles everything with FOO defined.
module FSharp.Editor.Tests.MultiTargetNavigateToSearchTests

open System.Collections.Immutable
open System.Threading
open Xunit
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

let private project =
    SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource = "let sharedFunc funcParam = funcParam * 2\n"
        },
        { sourceFile "Second" [ "First" ] with
            ExtraSource = "let plainUse x = ModuleFirst.sharedFunc x"
        },
        { sourceFile "Third" [ "First" ] with
            ExtraSource = "#if FOO\nlet fooUse x = ModuleFirst.sharedFunc x\n#endif"
        },
        { sourceFile "Fourth" [ "First" ] with
            ExtraSource = "let fooOnlyFileUse x = ModuleFirst.sharedFunc x"
        }
    )

let private solution, instances =
    RoslynTestHelpers.CreateMultiTargetSolution(
        project,
        [
            {
                Defines = []
                ExcludedFileIds = [ "Fourth" ]
            }
            {
                Defines = [ "FOO" ]
                ExcludedFileIds = []
            }
        ]
    )

let private service: IFSharpNavigateToSearchService =
    MefHelpers.createExportProvider().GetExportedValue()

/// Every instance searched in solution order, as the NavigateTo searcher does.
let private search pattern =
    [
        for id in instances do
            yield!
                service
                    .SearchProjectAsync(
                        solution.GetProject id,
                        ImmutableArray.Empty,
                        pattern,
                        service.KindsProvided,
                        CancellationToken.None
                    )
                    .Result
    ]

[<Theory>]
[<InlineData("plainUse")>]
[<InlineData("fooUse")>]
[<InlineData("fooOnlyFileUse")>]
let ``a declaration is reported once across the instances of a project`` (name: string) =
    let names = search name |> List.map _.Name |> List.filter ((=) name)
    Assert.Equal<string list>([ name ], names)
