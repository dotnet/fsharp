// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.PEReferenceCacheTests

// removeLibrary listens to the obsolete WorkspaceChanged like the manager: its handlers run in registration order.
#nowarn "44"

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.PEReferenceVersionTests

open Xunit

[<Literal>]
let private BodyEditAfterDeclarationEdit =
    "public class Input { public int M() => 2; public int N() => 1; }"

/// An F# project that references the C# project <c>Library</c>.
type private Fixture =
    {
        Workspace: AdhocWorkspace
        FSharpProjectId: ProjectId
        Library: ProjectInfo
    }

    member this.LibraryDocumentId = this.Library.Documents |> Seq.exactlyOne |> _.Id

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
            FSharpProjectId = solution.Projects |> Seq.exactlyOne |> _.Id
            Library = library
        }

    referenceLibrary fixture
    fixture

let private manager (fixture: Fixture) =
    fixture.Workspace.Services.GetService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

let private optionsOf (fixture: Fixture) =
    task {
        let project = fixture.Workspace.CurrentSolution.GetProject fixture.FSharpProjectId

        match! (manager fixture).TryGetOptionsByProject(project, CancellationToken.None) with
        | ValueSome(_, options) -> return options
        | ValueNone -> return failwith "The project options manager has no options for the F# project."
    }

let private peReferenceOf (options: FSharpProjectOptions) =
    options.ReferencedProjects
    |> Seq.filter (function
        | FSharpReferencedProject.PEReference _ -> true
        | _ -> false)
    |> Seq.exactlyOne

let private referenceOf (fixture: Fixture) =
    task {
        let! options = optionsOf fixture
        return peReferenceOf options
    }

let private versionOfLibrary (fixture: Fixture) =
    let library = fixture.Workspace.CurrentSolution.GetProject fixture.Library.Id
    PEReferenceVersion.getAsync library CancellationToken.None

/// Removes the C# project, and completes once the manager, which listens from the start, has been told to forget it.
let private removeLibrary (fixture: Fixture) =
    task {
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

        let! first = Task.WhenAny(removed.Task :> Task, Task.Delay(TimeSpan.FromSeconds 60.))
        Assert.True(obj.ReferenceEquals(first, removed.Task), "Roslyn did not raise the removal of the project in time.")
    }

let private editLibrary (fixture: Fixture) (source: string) =
    let workspace = fixture.Workspace

    let solution =
        workspace.CurrentSolution.WithDocumentText(fixture.LibraryDocumentId, SourceText.From source)

    Assert.True(workspace.TryApplyChanges solution)

[<Fact>]
let ``an edit that leaves the C# declarations alone keeps the PE reference`` () =
    task {
        let fixture = createFixture []
        let! options = optionsOf fixture
        let reference = peReferenceOf options

        editLibrary fixture BodyEdit

        // The dependent version moved, so the options are computed again, yet they keep the reference.
        let! recomputed = optionsOf fixture
        Assert.NotSame(options, recomputed)
        Assert.Same(reference, peReferenceOf recomputed)
    }

[<Fact>]
let ``an edit of the C# declarations replaces the PE reference, and the replacement is kept`` () =
    task {
        let fixture = createFixture []
        let! reference = referenceOf fixture

        editLibrary fixture DeclarationEdit
        let! replacement = referenceOf fixture

        Assert.NotSame(reference, replacement)

        editLibrary fixture BodyEditAfterDeclarationEdit
        let! kept = referenceOf fixture
        Assert.Same(replacement, kept)
    }

[<Fact>]
let ``a C# project with an analyzer reference gets a new PE reference on every edit`` () =
    task {
        let fixture = createFixture [ analyzerReference ]
        let! reference = referenceOf fixture

        editLibrary fixture BodyEdit

        let! replacement = referenceOf fixture
        Assert.NotSame(reference, replacement)
    }

[<Fact>]
let ``removing the C# project drops its PE reference`` () =
    task {
        let fixture = createFixture []
        let! reference = referenceOf fixture
        let! version = versionOfLibrary fixture

        do! removeLibrary fixture

        // The very same project comes back in the very same state: the reference it had is not kept for it.
        fixture.Workspace.AddProject fixture.Library |> ignore
        referenceLibrary fixture
        let! versionAfterReturn = versionOfLibrary fixture
        Assert.Equal(version, versionAfterReturn)

        let! returned = referenceOf fixture
        Assert.NotSame(reference, returned)
    }

[<Fact>]
let ``clearing all caches drops the PE reference`` () =
    task {
        let fixture = createFixture []
        let! reference = referenceOf fixture

        (manager fixture).ClearAllCaches()

        // The command line options go with the caches.
        RoslynTestHelpers.SetProjectOptions
            fixture.FSharpProjectId
            fixture.Workspace.CurrentSolution
            RoslynTestHelpers.DefaultProjectOptions

        let! recomputed = referenceOf fixture
        Assert.NotSame(reference, recomputed)
    }
