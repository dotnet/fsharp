// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// One project loaded as two target-framework instances: `plain` compiles without the fourth file
/// and without FOO, `foo` compiles everything with FOO defined.
module FSharp.Editor.Tests.MultiTargetNavigateToSearchTests

open System.Collections.Immutable
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
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

let private searchIn (project: Project) pattern =
    service.SearchProjectAsync(project, ImmutableArray.Empty, pattern, service.KindsProvided, CancellationToken.None).Result
    |> Seq.map _.Name
    |> Seq.filter ((=) pattern)
    |> Seq.toList

let private documentNamed (name: string) (project: Project) =
    project.Documents |> Seq.find (fun document -> document.Name.Contains name)

/// Roslyn submits only the active project when the search is scoped to the current one, so an instance has
/// to report what it compiles even when a sibling compiles the same file. The second search is the one that
/// used to come back empty: by then the file had been parsed, and the parse said it did not depend on the
/// defines.
[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
let ``an instance reports a shared declaration on its own, and again once the parse is cached`` (instance: int) =
    let project = solution.GetProject instances[instance]

    Assert.Equal<string list>([ "plainUse" ], searchIn project "plainUse")
    Assert.Equal<string list>([ "plainUse" ], searchIn project "plainUse")

/// The parse of a file that does hold directives is kept per define set, so reusing it across the instances
/// must not leak the declaration into the instance that does not define FOO.
[<Fact>]
let ``a declaration behind a directive is reported only by the instance that defines it`` () =
    let plain = solution.GetProject instances[0]
    let foo = solution.GetProject instances[1]

    Assert.Equal<string list>([ "fooUse" ], searchIn foo "fooUse")
    Assert.Equal<string list>([], searchIn plain "fooUse")

/// A file with no directives has its parse shared by every instance. When an edit gives it one, that shared
/// parse is stale: the entry is keyed by the text version, so the edit is a different entry rather than a
/// flag left over from before.
[<Fact>]
let ``a declaration an edit puts behind a directive is reported by the instance that defines it`` () =
    let edited =
        (solution, instances)
        ||> Seq.fold (fun (solution: Solution) instance ->
            let document = solution.GetProject instance |> documentNamed "Second"

            solution.WithDocumentText(
                document.Id,
                SourceText.From "module ModuleSecond\n#if FOO\nlet addedFoo = 1\n#endif\n"
            ))

    let foo = edited.GetProject instances[1]
    let plain = edited.GetProject instances[0]

    // The instance without FOO goes first: that order is what left the stale answer behind.
    Assert.Equal<string list>([], searchIn plain "addedFoo")
    Assert.Equal<string list>([ "addedFoo" ], searchIn foo "addedFoo")
