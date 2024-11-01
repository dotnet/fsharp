module FSharpWorkspaceTests

open System
open Xunit
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

#nowarn "57"

[<Fact>]
let ``Add project to workspace`` () =
    let workspace = FSharpWorkspace()
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.AddProject(projectPath, outputPath, compilerArgs)
    let projectSnapshot = workspace.GetProjectSnapshot(projectIdentifier)
    Assert.NotNull(projectSnapshot)
    Assert.Equal(projectPath, projectSnapshot.ProjectFileName)
    Assert.Equal(Some outputPath, projectSnapshot.OutputFileName)
    Assert.Contains("test.fs", projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))

[<Fact>]
let ``Open file in workspace`` () =
    let workspace = FSharpWorkspace()
    let fileUri = Uri("file:///test.fs")
    let content = "let x = 1"

    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| fileUri.LocalPath |]
    let _projectIdentifier = workspace.AddProject(projectPath, outputPath, compilerArgs)

    workspace.OpenFile(fileUri, content)
    let projectSnapshot = workspace.GetProjectSnapshotForFile(fileUri)

    // Retrieve the file snapshot from the project snapshot
    let fileSnapshot =
        projectSnapshot
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")
        |> _.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    // Assert that the content of the file in the snapshot is correct
    Assert.Equal(content, fileSnapshot.GetSource().Result.ToString())

    let fileSnapshot =
        projectSnapshot
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")
        |> _.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    Assert.Equal(content, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Close file in workspace`` () =
    let workspace = FSharpWorkspace()
    let fileUri = Uri("file:///test.fs")
    let content = "let x = 1"
    workspace.OpenFile(fileUri, content)
    workspace.CloseFile(fileUri)
    let projectSnapshot = workspace.GetProjectSnapshotForFile(fileUri)
    Assert.Null(projectSnapshot)

[<Fact>]
let ``Change file in workspace`` () =
    let workspace = FSharpWorkspace()
    let fileUri = Uri("file:///test.fs")
    let content = "let x = 1"
    workspace.ChangeFile(fileUri, content)
    let projectSnapshot = workspace.GetProjectSnapshotForFile(fileUri)
    Assert.NotNull(projectSnapshot)

    let fileSnapshot =
        projectSnapshot
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")
        |> _.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    Assert.Equal(content, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Retrieve project snapshot`` () =
    let workspace = FSharpWorkspace()
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.AddProject(projectPath, outputPath, compilerArgs)
    let projectSnapshot = workspace.GetProjectSnapshot(projectIdentifier)
    Assert.NotNull(projectSnapshot)
    Assert.Equal(projectPath, projectSnapshot.ProjectFileName)
    Assert.Equal(Some outputPath, projectSnapshot.OutputFileName)
    Assert.Contains("test.fs", projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))

[<Fact>]
let ``Retrieve project snapshot for file`` () =
    let workspace = FSharpWorkspace()
    let fileUri = Uri("file:///test.fs")
    let content = "let x = 1"
    workspace.OpenFile(fileUri, content)
    let projectSnapshot = workspace.GetProjectSnapshotForFile(fileUri)
    Assert.NotNull(projectSnapshot)

    let fileSnapshot =
        projectSnapshot
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")
        |> _.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    Assert.Equal(content, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Add multiple projects with references`` () =
    let workspace = FSharpWorkspace()
    let projectPath1 = "test1.fsproj"
    let outputPath1 = "test1.dll"
    let compilerArgs1 = [| "test1.fs" |]

    let projectIdentifier1 =
        workspace.AddProject(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]

    let projectIdentifier2 =
        workspace.AddProject(projectPath2, outputPath2, compilerArgs2)

    let projectSnapshot1 = workspace.GetProjectSnapshot(projectIdentifier1)
    let projectSnapshot2 = workspace.GetProjectSnapshot(projectIdentifier2)
    Assert.Contains("test1.fs", projectSnapshot1.SourceFiles |> Seq.map (fun f -> f.FileName))
    Assert.Contains("test2.fs", projectSnapshot2.SourceFiles |> Seq.map (fun f -> f.FileName))

    Assert.Contains(
        projectSnapshot1,
        projectSnapshot2.ReferencedProjects
        |> Seq.choose (function
            | FSharpReferencedProjectSnapshot.FSharpReference(_, s) -> Some s
            | _ -> None)
    )

[<Fact>]
let ``Propagate changes to snapshots`` () =
    let workspace = FSharpWorkspace()
    let projectPath1 = "test1.fsproj"
    let outputPath1 = "test1.dll"
    let compilerArgs1 = [| "test1.fs" |]

    let projectIdentifier1 =
        workspace.AddProject(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]

    let projectIdentifier2 =
        workspace.AddProject(projectPath2, outputPath2, compilerArgs2)

    let fileUri = Uri("file:///test1.fs")
    let content = "let x = 1"
    workspace.ChangeFile(fileUri, content)
    let projectSnapshot1 = workspace.GetProjectSnapshot(projectIdentifier1)
    let projectSnapshot2 = workspace.GetProjectSnapshot(projectIdentifier2)
    Assert.NotNull(projectSnapshot1)
    Assert.NotNull(projectSnapshot2)

    let fileSnapshot1 =
        projectSnapshot1.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    let fileSnapshot2 =
        projectSnapshot2.SourceFiles
        |> Seq.find (fun f -> f.FileName = fileUri.LocalPath)

    Assert.Equal(content, fileSnapshot1.GetSource().Result.ToString())
    Assert.Equal(content, fileSnapshot2.GetSource().Result.ToString())

[<Fact>]
let ``Update project by adding a source file`` () =
    let workspace = FSharpWorkspace()
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.AddProject(projectPath, outputPath, compilerArgs)
    let newSourceFile = "newTest.fs"
    let newCompilerArgs = [| "test.fs"; newSourceFile |]
    workspace.AddProject(projectPath, outputPath, newCompilerArgs) |> ignore
    let projectSnapshot = workspace.GetProjectSnapshot(projectIdentifier)
    Assert.NotNull(projectSnapshot)
    Assert.Contains("test.fs", projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))
    Assert.Contains(newSourceFile, projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))

[<Fact>]
let ``Update project by adding a reference`` () =
    let workspace = FSharpWorkspace()
    let projectPath1 = "test1.fsproj"
    let outputPath1 = "test1.dll"
    let compilerArgs1 = [| "test1.fs" |]

    let projectIdentifier1 =
        workspace.AddProject(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs" |]

    let projectIdentifier2 =
        workspace.AddProject(projectPath2, outputPath2, compilerArgs2)

    let newCompilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]
    workspace.AddProject(projectPath2, outputPath2, newCompilerArgs2) |> ignore
    let projectSnapshot1 = workspace.GetProjectSnapshot(projectIdentifier1)
    let projectSnapshot2 = workspace.GetProjectSnapshot(projectIdentifier2)
    Assert.NotNull(projectSnapshot1)
    Assert.NotNull(projectSnapshot2)

    Assert.Contains(
        projectSnapshot1,
        projectSnapshot2.ReferencedProjects
        |> Seq.choose (function
            | FSharpReferencedProjectSnapshot.FSharpReference(_, s) -> Some s
            | _ -> None)
    )

[<Fact>]
let ``Create references in existing projects`` () =
    let workspace = FSharpWorkspace()
    let projectPath1 = "test1.fsproj"
    let outputPath1 = "test1.dll"
    let compilerArgs1 = [| "test1.fs" |]

    let projectIdentifier1 =
        workspace.AddProject(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs" |]

    let projectIdentifier2 =
        workspace.AddProject(projectPath2, outputPath2, compilerArgs2)

    let newCompilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]
    workspace.AddProject(projectPath2, outputPath2, newCompilerArgs2) |> ignore
    let projectSnapshot1 = workspace.GetProjectSnapshot(projectIdentifier1)
    let projectSnapshot2 = workspace.GetProjectSnapshot(projectIdentifier2)
    Assert.NotNull(projectSnapshot1)
    Assert.NotNull(projectSnapshot2)

    Assert.Contains(
        projectSnapshot1,
        projectSnapshot2.ReferencedProjects
        |> Seq.choose (function
            | FSharpReferencedProjectSnapshot.FSharpReference(_, s) -> Some s
            | _ -> None)
    )
