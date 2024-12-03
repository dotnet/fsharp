module CompilerService.FSharpWorkspace

open System
open Xunit

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open TestFramework
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis.Workspace

#nowarn "57"

type ProjectConfig with

    static member Minimal(?name, ?outputPath, ?referencesOnDisk) =
        let name = defaultArg name "test"
        let projectFileName = $"{name}.fsproj"
        let outputPath = defaultArg outputPath $"{name}.dll"
        let referencesOnDisk = defaultArg referencesOnDisk []
        ProjectConfig(projectFileName, Some outputPath, referencesOnDisk, [])

let getReferencedSnapshot (projectIdentifier: FSharpProjectIdentifier) (projectSnapshot: FSharpProjectSnapshot) =
    projectSnapshot.ReferencedProjects
        |> Seq.pick (function
            | FSharpReference(x, snapshot) when x = projectIdentifier.OutputFileName -> Some snapshot
            | _ -> None)

let sourceFileOnDisk (content: string) =
    let path = getTemporaryFileName () + ".fs"
    FileSystem.OpenFileForWriteShim(path).Write(content)
    Uri(path)

let assertFileHasContent filePath expectedContent (projectSnapshot: FSharpProjectSnapshot) =
    let fileSnapshot =
        projectSnapshot.SourceFiles |> Seq.find (_.FileName >> (=) filePath)

    Assert.Equal(expectedContent, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Add project to workspace`` () =
    let workspace = FSharpWorkspace()
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.Projects.AddOrUpdate(projectPath, outputPath, compilerArgs)
    let projectSnapshot = workspace.Query.GetProjectSnapshot(projectIdentifier).Value
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
    let _projectIdentifier = workspace.Projects.AddOrUpdate(projectPath, outputPath, compilerArgs)

    workspace.Files.Open(fileUri, content)
    let projectSnapshot = workspace.Query.GetProjectSnapshotForFile(fileUri)

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

    let contentOnDisk = "let x = 1"
    let fileOnDisk = sourceFileOnDisk contentOnDisk

    let _projectIdentifier =
        workspace.Projects.AddOrUpdate(ProjectConfig.Minimal(), [ fileOnDisk.LocalPath ])

    workspace.Files.Open(fileOnDisk, contentOnDisk)

    let contentInMemory = "let x = 2"
    workspace.Files.Edit(fileOnDisk, contentInMemory)

    let projectSnapshot =
        workspace.Query.GetProjectSnapshotForFile(fileOnDisk)
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")

    let fileSnapshot =
        projectSnapshot.SourceFiles |> Seq.find (_.FileName >> (=) fileOnDisk.LocalPath)

    Assert.Equal(contentInMemory, fileSnapshot.GetSource().Result.ToString())

    workspace.Files.Close(fileOnDisk)

    let projectSnapshot =
        workspace.Query.GetProjectSnapshotForFile(fileOnDisk)
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")

    let fileSnapshot =
        projectSnapshot.SourceFiles |> Seq.find (_.FileName >> (=) fileOnDisk.LocalPath)

    Assert.Equal(contentOnDisk, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Change file in workspace`` () =
    let workspace = FSharpWorkspace()

    let fileUri = Uri("file:///test.fs")

    let _projectIdentifier =
        workspace.Projects.AddOrUpdate(ProjectConfig.Minimal(), [ fileUri.LocalPath ])

    let initialContent = "let x = 2"

    workspace.Files.Open(fileUri, initialContent)

    let updatedContent = "let x = 3"

    workspace.Files.Edit(fileUri, updatedContent)

    let projectSnapshot =
        workspace.Query.GetProjectSnapshotForFile(fileUri)
        |> Option.defaultWith (fun () -> failwith "Project snapshot not found")

    let fileSnapshot =
        projectSnapshot.SourceFiles |> Seq.find (_.FileName >> (=) fileUri.LocalPath)

    Assert.Equal(updatedContent, fileSnapshot.GetSource().Result.ToString())

[<Fact>]
let ``Add multiple projects with references`` () =
    let workspace = FSharpWorkspace()
    let projectPath1 = "test1.fsproj"
    let outputPath1 = "test1.dll"
    let compilerArgs1 = [| "test1.fs" |]

    let projectIdentifier1 =
        workspace.Projects.AddOrUpdate(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]

    let projectIdentifier2 =
        workspace.Projects.AddOrUpdate(projectPath2, outputPath2, compilerArgs2)

    let projectSnapshot1 = workspace.Query.GetProjectSnapshot(projectIdentifier1).Value
    let projectSnapshot2 = workspace.Query.GetProjectSnapshot(projectIdentifier2).Value
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

    let file1 = sourceFileOnDisk "let x = 1"
    let pid1 = workspace.Projects.AddOrUpdate(ProjectConfig.Minimal("p1"), [ file1.LocalPath ])

    let file2 = sourceFileOnDisk "let y = 2"

    let pid2 =
        workspace.Projects.AddOrUpdate(ProjectConfig.Minimal("p2", referencesOnDisk = [ pid1.OutputFileName ]), [ file2.LocalPath ])

    let file3 = sourceFileOnDisk "let z = 3"

    let pid3 =
        workspace.Projects.AddOrUpdate(ProjectConfig.Minimal("p3", referencesOnDisk = [ pid2.OutputFileName ]), [ file3.LocalPath ])

    let s3 = workspace.Query.GetProjectSnapshot(pid3).Value

    s3
    |> getReferencedSnapshot pid2
    |> getReferencedSnapshot pid1
    |> assertFileHasContent file1.LocalPath "let x = 1"

    let updatedContent = "let x = 2"

    workspace.Files.Edit(file1, updatedContent)

    let s3 = workspace.Query.GetProjectSnapshot(pid3).Value

    s3
    |> getReferencedSnapshot pid2
    |> getReferencedSnapshot pid1
    |> assertFileHasContent file1.LocalPath updatedContent

[<Fact>]
let ``Update project by adding a source file`` () =
    let workspace = FSharpWorkspace()
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.Projects.AddOrUpdate(projectPath, outputPath, compilerArgs)
    let newSourceFile = "newTest.fs"
    let newCompilerArgs = [| "test.fs"; newSourceFile |]
    workspace.Projects.AddOrUpdate(projectPath, outputPath, newCompilerArgs) |> ignore
    let projectSnapshot = workspace.Query.GetProjectSnapshot(projectIdentifier).Value
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
        workspace.Projects.AddOrUpdate(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs" |]

    let projectIdentifier2 =
        workspace.Projects.AddOrUpdate(projectPath2, outputPath2, compilerArgs2)

    let newCompilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]
    workspace.Projects.AddOrUpdate(projectPath2, outputPath2, newCompilerArgs2) |> ignore
    let projectSnapshot1 = workspace.Query.GetProjectSnapshot(projectIdentifier1).Value
    let projectSnapshot2 = workspace.Query.GetProjectSnapshot(projectIdentifier2).Value

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
        workspace.Projects.AddOrUpdate(projectPath1, outputPath1, compilerArgs1)

    let projectPath2 = "test2.fsproj"
    let outputPath2 = "test2.dll"
    let compilerArgs2 = [| "test2.fs" |]

    let projectIdentifier2 =
        workspace.Projects.AddOrUpdate(projectPath2, outputPath2, compilerArgs2)

    let projectSnapshot1 = workspace.Query.GetProjectSnapshot(projectIdentifier1).Value
    let projectSnapshot2 = workspace.Query.GetProjectSnapshot(projectIdentifier2).Value

    Assert.DoesNotContain(
        projectSnapshot1,
        projectSnapshot2.ReferencedProjects
        |> Seq.choose (function
            | FSharpReferencedProjectSnapshot.FSharpReference(_, s) -> Some s
            | _ -> None)
    )

    let newCompilerArgs2 = [| "test2.fs"; "-r:test1.dll" |]
    workspace.Projects.AddOrUpdate(projectPath2, outputPath2, newCompilerArgs2) |> ignore
    let projectSnapshot1 = workspace.Query.GetProjectSnapshot(projectIdentifier1).Value
    let projectSnapshot2 = workspace.Query.GetProjectSnapshot(projectIdentifier2).Value

    Assert.Contains(
        projectSnapshot1,
        projectSnapshot2.ReferencedProjects
        |> Seq.choose (function
            | FSharpReferencedProjectSnapshot.FSharpReference(_, s) -> Some s
            | _ -> None)
    )

[<Fact>]
let ``Asking for an unknown project snapshot returns None`` () =

    let workspace = FSharpWorkspace()

    Assert.Equal(None, workspace.Query.GetProjectSnapshot(FSharpProjectIdentifier("hello", "world")))
