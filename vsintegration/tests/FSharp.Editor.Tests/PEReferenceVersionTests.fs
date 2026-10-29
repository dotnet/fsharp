// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The version a referenced C# project's in-memory PE reference is kept for. Only the version is read, which
/// needs neither an F# project nor the editor services: the projects live in a plain workspace.
module FSharp.Editor.Tests.PEReferenceVersionTests

open System.Collections.Immutable
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

[<Literal>]
let private BodyEdit = "public class Input { public int M() => 2; }"

[<Literal>]
let private DeclarationEdit =
    "public class Input { public int M() => 1; public int N() => 1; }"

let private analyzerReference =
    AnalyzerImageReference(ImmutableArray<DiagnosticAnalyzer>.Empty) :> AnalyzerReference

let private projectNamed (name: string) (solution: Solution) =
    solution.Projects |> Seq.find (fun project -> project.Name = name)

let private version (projectName: string) (solution: Solution) =
    (PEReferenceVersion.getAsync (projectNamed projectName solution) CancellationToken.None).Result

/// `Library` references `Dependency`, whose only document is the one edited.
[<Theory>]
[<InlineData("", "Dependency", BodyEdit, false)>]
[<InlineData("", "Library", BodyEdit, false)>]
[<InlineData("", "Dependency", DeclarationEdit, true)>]
[<InlineData("Dependency", "Dependency", BodyEdit, true)>]
[<InlineData("Dependency", "Library", BodyEdit, true)>]
[<InlineData("Library", "Library", BodyEdit, true)>]
[<InlineData("Library", "Dependency", BodyEdit, false)>]
let ``only a declaration edit moves the version, unless a source generator may run``
    (projectWithAnalyzerReference: string)
    (versionedProject: string)
    (editedSource: string)
    (movesVersion: bool)
    =
    use workspace = new AdhocWorkspace()
    let dependencyId = ProjectId.CreateNewId()
    let libraryId = ProjectId.CreateNewId()
    let documentId = DocumentId.CreateNewId dependencyId

    let solution =
        workspace.CurrentSolution
            .AddProject(dependencyId, "Dependency", "Dependency", LanguageNames.CSharp)
            .AddDocument(documentId, "Input.cs", "public class Input { public int M() => 1; }")
            .AddProject(libraryId, "Library", "Library", LanguageNames.CSharp)
            .AddProjectReference(libraryId, ProjectReference dependencyId)

    let solution =
        match projectWithAnalyzerReference with
        | "" -> solution
        | name -> solution.AddAnalyzerReference((projectNamed name solution).Id, analyzerReference)

    let before = version versionedProject solution

    let after =
        solution.WithDocumentText(documentId, SourceText.From editedSource)
        |> version versionedProject

    Assert.Equal(movesVersion, before <> after)
