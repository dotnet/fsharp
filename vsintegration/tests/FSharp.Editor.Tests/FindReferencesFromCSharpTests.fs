// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// An F# library and a C# project referencing its built assembly, as VS wires a C# → F# project reference.
module FSharp.Editor.Tests.FindReferencesFromCSharpTests

open System.Collections.Immutable
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

let private library =
    SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource = "let twice x = x * 2\n"
        }
    )

/// The synthetic project puts its modules in a namespace named after the project.
let private moduleName = $"{library.Name}.ModuleFirst"

let private solution =
    let librarySolution, checker = RoslynTestHelpers.CreateMultiProjectSolution library
    let assembly = RoslynTestHelpers.CompileToAssembly(library, checker)

    RoslynTestHelpers.AddCSharpProject(
        librarySolution,
        "Consumer",
        $"class Consumer {{ int M() => {moduleName}.twice(1); }}",
        library.GetProjectOptions checker,
        [ assembly ]
    )

let private consumer =
    solution.Projects |> Seq.find (fun p -> p.Language = LanguageNames.CSharp)

let private fsharpDocument =
    solution.GetDocumentIdsWithFilePath(library.GetFilePath "First")
    |> Seq.exactlyOne
    |> solution.GetDocument

[<Fact>]
let ``the C# compilation resolves the doc comment id of an F# function`` () =
    let compilation = consumer.GetCompilationAsync(CancellationToken.None).Result

    let errors =
        compilation.GetDiagnostics()
        |> Seq.filter (fun d -> d.Severity = DiagnosticSeverity.Error)

    Assert.Empty errors

    let twice =
        DocumentationCommentId.GetFirstSymbolForDeclarationId($"M:{moduleName}.twice(System.Int32)", compilation)

    Assert.NotNull twice

    let references =
        SymbolFinder.FindReferencesAsync(twice, solution, ImmutableHashSet.CreateRange consumer.Documents, CancellationToken.None).Result

    Assert.Single(references |> Seq.collect _.Locations) |> ignore

[<Fact>]
let ``the consumer is found as a project referencing the F# assembly`` () =
    let referencing =
        ProjectFiltering.getProjectsReferencingAssembly fsharpDocument.Project.OutputFilePath solution

    Assert.Equal<ProjectId list>([ consumer.Id ], referencing |> List.map _.Id)
