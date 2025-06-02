module CompilerService.FSharpWorkspace

open System

open Xunit

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.Diagnostics
open FSharp.Test.ProjectGeneration.WorkspaceHelpers
open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace
open OpenTelemetry.Exporter
open System.IO

#nowarn "57"

// System.Diagnostics.DiagnosticSource seems to be missing in NET FW. Might investigate this later
#if !NETFRAMEWORK

//type FilteredJaegerExporter(predicate) =

//    inherit SimpleActivityExportProcessor(new JaegerExporter(new JaegerExporterOptions()))

//    override _.OnEnd(activity: System.Diagnostics.Activity) =
//        if predicate activity then
//            base.OnEnd activity

/// Wrapper for FSharpWorkspace to use in tests. Provides OpenTelemetry tracing.
type TestingWorkspace(testName) as _this =
    inherit FSharpWorkspace()

    let debugGraphPath = __SOURCE_DIRECTORY__ ++ $"{testName}.md"

    //let tracerProvider =
    //    Sdk
    //        .CreateTracerProviderBuilder()
    //        .AddSource("fsc")
    //        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName="F#", serviceVersion = "1"))
    //        .AddProcessor(new FilteredJaegerExporter(_.DisplayName >> (<>) "DiagnosticsLogger.StackGuard.Guard"))
    //        .Build()

    let activity = Activity.start $"Test FSharpWorkspace {testName}" []

    do _this.Projects.Debug_DumpGraphOnEveryChange <- Some debugGraphPath

    member _.DebugGraphPath = debugGraphPath

    interface IDisposable with
        member _.Dispose() =
            use _ = activity in ()
            //tracerProvider.ForceFlush() |> ignore
            //tracerProvider.Dispose()

[<Fact>]
let ``Add project to workspace`` () =
    use workspace = new TestingWorkspace("Add project to workspace")
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
    use workspace = new TestingWorkspace("Open file in workspace")
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
    use workspace = new TestingWorkspace("Close file in workspace")

    let contentOnDisk = "let x = 1"
    let fileOnDisk = sourceFileOnDisk contentOnDisk

    let _projectIdentifier =
        workspace.Projects.AddOrUpdate(ProjectConfig.Empty(), [ fileOnDisk.LocalPath ])

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
    use workspace = new TestingWorkspace("Change file in workspace")

    let fileUri = Uri("file:///test.fs")

    let _projectIdentifier =
        workspace.Projects.AddOrUpdate(ProjectConfig.Empty(), [ fileUri.LocalPath ])

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
    use workspace = new TestingWorkspace("Add multiple projects with references")
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
    use workspace = new TestingWorkspace("Propagate changes to snapshots")

    let file1 = sourceFileOnDisk "let x = 1"
    let pid1 = workspace.Projects.AddOrUpdate(ProjectConfig.Empty("p1"), [ file1.LocalPath ])

    let file2 = sourceFileOnDisk "let y = 2"

    let pid2 =
        workspace.Projects.AddOrUpdate(ProjectConfig.Empty("p2", referencesOnDisk = [ pid1.OutputFileName ]), [ file2.LocalPath ])

    let file3 = sourceFileOnDisk "let z = 3"

    let pid3 =
        workspace.Projects.AddOrUpdate(ProjectConfig.Empty("p3", referencesOnDisk = [ pid2.OutputFileName ]), [ file3.LocalPath ])

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
let ``AddOrUpdate project by adding a source file`` () =
    use workspace = new TestingWorkspace("Update project by adding a source file")
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
let ``Update project by adding a source file`` () =
    use workspace = new TestingWorkspace("Update project by adding a source file")
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs" |]
    let projectIdentifier = workspace.Projects.AddOrUpdate(projectPath, outputPath, compilerArgs)
    let newSourceFile = "newTest.fs"
    let newSourceFiles = [| "test.fs"; newSourceFile |]
    workspace.Projects.Update(projectIdentifier, newSourceFiles) |> ignore
    let projectSnapshot = workspace.Query.GetProjectSnapshot(projectIdentifier).Value
    Assert.NotNull(projectSnapshot)
    Assert.Contains("test.fs", projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))
    Assert.Contains(newSourceFile, projectSnapshot.SourceFiles |> Seq.map (fun f -> f.FileName))

[<Fact>]
let ``Update project by removing a source file`` () =
    use workspace = new TestingWorkspace("Update project by removing a source file")
    let projectPath = "test.fsproj"
    let outputPath = "test.dll"
    let compilerArgs = [| "test.fs"; "newTest.fs" |]
    let projectIdentifier = workspace.Projects.AddOrUpdate(projectPath, outputPath, compilerArgs)
    let newCompilerArgs = [| "test.fs" |]
    workspace.Projects.AddOrUpdate(projectPath, outputPath, newCompilerArgs) |> ignore
    let files = workspace.Files.OfProject(projectIdentifier) |> Seq.toArray
    Assert.Equal<string array>([| "test.fs" |], files)

[<Fact>]
let ``Update project by adding a reference`` () =
    use workspace = new TestingWorkspace("Update project by adding a reference")
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
    use workspace = new TestingWorkspace("Create references in existing projects")
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

    use workspace = new TestingWorkspace("Asking for an unknown project snapshot returns None")

    Assert.Equal(None, workspace.Query.GetProjectSnapshot(FSharpProjectIdentifier("hello", "world")))


[<Fact>]
let ``Works with signature files`` () =
    task {

        use workspace = new TestingWorkspace("Works with signature files")

        let projectConfig = ProjectConfig.Create()

        let sourceFileUri = projectConfig.FileUri "test.fs"

        let source = "let x = 1"

        let projectIdentifier = workspace.Projects.AddOrUpdate(projectConfig, [ sourceFileUri ])

        workspace.Files.Open(sourceFileUri, source)

        let! signatureUri, _signatureSource = workspace.AddSignatureFile(projectIdentifier, sourceFileUri, writeToDisk=false)

        let! diag = workspace.Query.GetDiagnosticsForFile(signatureUri)

        Assert.Equal(0, diag.Diagnostics.Length)

        workspace.Files.Edit(signatureUri, "module Test\n\nval x: potato")

        let! diag = workspace.Query.GetDiagnosticsForFile(signatureUri)

        Assert.Equal(1, diag.Diagnostics.Length)
        Assert.Equal("The type 'potato' is not defined.", diag.Diagnostics[0].Message)

        workspace.Files.Edit(signatureUri, "module Test\n\nval y: int")

        let! diag = workspace.Query.GetDiagnosticsForFile(sourceFileUri)

        Assert.Equal(1, diag.Diagnostics.Length)
        Assert.Equal("Module 'Test' requires a value 'y'", diag.Diagnostics[0].Message)
    }


let reposDir = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ ".."
let giraffeDir = reposDir ++ "Giraffe" ++ "src" ++ "Giraffe" |> Path.GetFullPath
let giraffeTestsDir = reposDir ++ "Giraffe" ++ "tests" ++ "Giraffe.Tests" |> Path.GetFullPath
let giraffeSampleDir = reposDir ++ "Giraffe" ++ "samples" ++ "EndpointRoutingApp" ++ "EndpointRoutingApp" |> Path.GetFullPath
let giraffeSignaturesDir = reposDir ++ "giraffe-signatures" ++ "src" ++ "Giraffe" |> Path.GetFullPath
let giraffeSignaturesTestsDir = reposDir ++ "giraffe-signatures" ++ "tests" ++ "Giraffe.Tests" |> Path.GetFullPath
let giraffeSignaturesSampleDir = reposDir ++ "giraffe-signatures" ++ "samples" ++ "EndpointRoutingApp" ++ "EndpointRoutingApp" |> Path.GetFullPath

type GiraffeFactAttribute() =
    inherit Xunit.FactAttribute()
        do
            if not (Directory.Exists giraffeDir) then
                do base.Skip <- $"Giraffe not found ({giraffeDir}). You can get it here: https://github.com/giraffe-fsharp/Giraffe"
            if not (Directory.Exists giraffeSignaturesDir) then
                do base.Skip <- $"Giraffe (with signatures) not found ({giraffeSignaturesDir}). You can get it here: https://github.com/nojaf/Giraffe/tree/signatures"


[<GiraffeFact>]
let ``Giraffe signature test`` () =
    task {
        use workspace = new TestingWorkspace("Giraffe signature test")

        let responseFileName = "compilerArgs.rsp"

        let _identifiers =
            [
                giraffeSignaturesDir
                giraffeSignaturesTestsDir
                giraffeSignaturesSampleDir ]
            |> Seq.map (fun dir ->
                let projectName = Path.GetFileName dir
                let dllName = $"{projectName}.dll"
                let responseFile = dir ++ responseFileName
                let outputFile = dir ++ "bin" ++ "Debug" ++ "net6.0" ++ dllName
                let projectFile = dir ++ projectName + ".fsproj"
                let compilerArgs = File.ReadAllLines responseFile
                workspace.Projects.AddOrUpdate(projectFile, outputFile, compilerArgs)
            )
            |> Seq.toList

        let _ = workspace.Files.OpenFromDisk(giraffeSignaturesSampleDir ++ "Program.fs")

        let! diag = workspace.Query.GetDiagnosticsForFile(Uri(giraffeSignaturesSampleDir ++ "Program.fs"))
        Assert.Equal(0, diag.Diagnostics.Length)

        let middlewareFsiSource = workspace.Files.OpenFromDisk(giraffeSignaturesDir ++ "Middleware.fsi")
        let middlewareFsiNewSource = middlewareFsiSource.Replace("static member AddGiraffe:", "static member AddGiraffe2:")

        let! diag = workspace.Query.GetDiagnosticsForFile(Uri(giraffeSignaturesDir ++ "Middleware.fsi"))
        Assert.Equal(0, diag.Diagnostics.Length)

        workspace.Files.Edit(Uri(giraffeSignaturesDir ++ "Middleware.fsi"), middlewareFsiNewSource)

        let! diag = workspace.Query.GetDiagnosticsForFile(Uri(giraffeSignaturesSampleDir ++ "Program.fs"))
        Assert.Equal(1, diag.Diagnostics.Length)
        Assert.Equal("The type 'IServiceCollection' does not define the field, constructor or member 'AddGiraffe'.", diag.Diagnostics[0].Message)
    }

#endif