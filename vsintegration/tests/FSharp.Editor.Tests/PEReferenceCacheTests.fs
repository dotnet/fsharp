// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The in-memory PE references <c>FSharpProjectOptionsManager</c> keeps for the C# projects an F# project
/// references: kept while the C# project's version holds, replaced when it moves, dropped with the project.
module FSharp.Editor.Tests.PEReferenceCacheTests

// removeLibrary listens to Workspace.WorkspaceChanged, as the manager does: the handlers of that event run in the
// order they were added, while those of its replacement run on another thread, in no order with them.
#nowarn "44"

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers

open Xunit

[<Literal>]
let private Original = "public class Library { public int Value() => 1; }"

[<Literal>]
let private BodyEdit = "public class Library { public int Value() => 2; }"

[<Literal>]
let private DeclarationEdit =
    "public class Library { public int Value() => 1; public int Other() => 1; }"

[<Literal>]
let private BodyEditAfterDeclarationEdit =
    "public class Library { public int Value() => 2; public int Other() => 1; }"

let private analyzerReference =
    AnalyzerImageReference(ImmutableArray<DiagnosticAnalyzer>.Empty) :> AnalyzerReference

/// An F# project that references the C# project <c>Library</c>.
type private Fixture =
    {
        Workspace: AdhocWorkspace
        FSharpProjectId: ProjectId
        Library: ProjectInfo
    }

    member this.LibraryDocumentId = (this.Library.Documents |> Seq.exactlyOne).Id

let private referenceLibrary (fixture: Fixture) =
    let workspace = fixture.Workspace

    let solution =
        workspace.CurrentSolution.AddProjectReference(fixture.FSharpProjectId, ProjectReference fixture.Library.Id)

    Assert.True(workspace.TryApplyChanges solution)

let private createFixture (analyzerReferences: AnalyzerReference list) =
    let solution = RoslynTestHelpers.CreateSolution("module Test")
    let workspace = solution.Workspace :?> AdhocWorkspace
    let libraryId = ProjectId.CreateNewId()

    let library =
        ProjectInfo.Create(
            libraryId,
            VersionStamp.Create(),
            "Library",
            "Library",
            LanguageNames.CSharp,
            filePath = "C:\\Library\\Library.csproj",
            outputFilePath = "C:\\Library\\Library.dll",
            compilationOptions = CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            documents =
                [
                    RoslynTestHelpers.CreateDocumentInfo libraryId "C:\\Library\\Library.cs" Original
                ],
            analyzerReferences = analyzerReferences
        )

    workspace.AddProject library |> ignore

    let fixture =
        {
            Workspace = workspace
            FSharpProjectId = (solution.Projects |> Seq.exactlyOne).Id
            Library = library
        }

    referenceLibrary fixture
    fixture

let private manager (fixture: Fixture) =
    fixture.Workspace.Services.GetService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

let private optionsOf (fixture: Fixture) =
    let project = fixture.Workspace.CurrentSolution.GetProject fixture.FSharpProjectId

    match
        (manager fixture).TryGetOptionsByProject(project, CancellationToken.None)
        |> Async.RunSynchronously
    with
    | ValueSome(_, options) -> options
    | ValueNone -> failwith "The project options manager has no options for the F# project."

let private peReferenceOf (options: FSharpProjectOptions) =
    options.ReferencedProjects
    |> Array.filter (function
        | FSharpReferencedProject.PEReference _ -> true
        | _ -> false)
    |> Array.exactlyOne

let private versionOfLibrary (fixture: Fixture) =
    let library = fixture.Workspace.CurrentSolution.GetProject fixture.Library.Id
    (PEReferenceVersion.getAsync library CancellationToken.None).Result

/// Removes the C# project, and returns once Roslyn has raised the removal to the handlers added before the call.
/// The manager listens from the start, so by then it has been told to forget the project.
let private removeLibrary (fixture: Fixture) =
    let workspace = fixture.Workspace
    let removed = TaskCompletionSource<bool>()

    workspace.WorkspaceChanged.Add(fun args ->
        if args.Kind = WorkspaceChangeKind.ProjectRemoved then
            removed.TrySetResult true |> ignore)

    let libraryReference =
        (workspace.CurrentSolution.GetProject fixture.FSharpProjectId).ProjectReferences
        |> Seq.exactlyOne

    Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProjectReference(fixture.FSharpProjectId, libraryReference)))

    Assert.True(workspace.TryApplyChanges(workspace.CurrentSolution.RemoveProject fixture.Library.Id))
    Assert.True(removed.Task.Wait(TimeSpan.FromSeconds 60.))

let private editLibrary (fixture: Fixture) (source: string) =
    let workspace = fixture.Workspace

    let solution =
        workspace.CurrentSolution.WithDocumentText(fixture.LibraryDocumentId, SourceText.From source)

    Assert.True(workspace.TryApplyChanges solution)

[<Fact>]
let ``an edit that leaves the C# declarations alone keeps the PE reference`` () =
    let fixture = createFixture []
    let options = optionsOf fixture
    let reference = peReferenceOf options

    editLibrary fixture BodyEdit

    // The edit moves the C# project's dependent version, so the options are computed again ...
    let recomputed = optionsOf fixture
    Assert.NotSame(options, recomputed)

    // ... and since its dependent semantic version stands, they keep the reference they had:
    // what FCS cached against the stamp of that reference stays valid.
    Assert.Same(reference, peReferenceOf recomputed)

[<Fact>]
let ``an edit of the C# declarations replaces the PE reference, and the replacement is kept`` () =
    let fixture = createFixture []
    let reference = optionsOf fixture |> peReferenceOf

    editLibrary fixture DeclarationEdit
    let replacement = optionsOf fixture |> peReferenceOf

    Assert.NotSame(reference, replacement)

    editLibrary fixture BodyEditAfterDeclarationEdit
    Assert.Same(replacement, optionsOf fixture |> peReferenceOf)

[<Fact>]
let ``a C# project with an analyzer reference gets a new PE reference on every edit`` () =
    let fixture = createFixture [ analyzerReference ]
    let reference = optionsOf fixture |> peReferenceOf

    editLibrary fixture BodyEdit

    Assert.NotSame(reference, optionsOf fixture |> peReferenceOf)

[<Fact>]
let ``removing the C# project drops its PE reference`` () =
    let fixture = createFixture []
    let reference = optionsOf fixture |> peReferenceOf
    let version = versionOfLibrary fixture

    removeLibrary fixture

    // The very same project comes back in the very same state: the reference it had is not kept for it.
    fixture.Workspace.AddProject fixture.Library |> ignore
    referenceLibrary fixture
    Assert.Equal(version, versionOfLibrary fixture)

    Assert.NotSame(reference, optionsOf fixture |> peReferenceOf)

[<Fact>]
let ``clearing all caches drops the PE reference`` () =
    let fixture = createFixture []
    let reference = optionsOf fixture |> peReferenceOf

    (manager fixture).ClearAllCaches()

    // The command line options go with the caches.
    RoslynTestHelpers.SetProjectOptions fixture.FSharpProjectId fixture.Workspace.CurrentSolution RoslynTestHelpers.DefaultProjectOptions

    Assert.NotSame(reference, optionsOf fixture |> peReferenceOf)
