/// Tools for generating synthetic projects where we can model dependencies between files.
///
/// Each file in the project has a string identifier. It then contains a type and a function.
/// The function calls functions from all the files the given file depends on and returns their
/// results + it's own type in a tuple.
///
/// To model changes, we change the type name in a file which resutls in signatures of all the
/// dependent files also changing.
///
/// To model breaking changes we change the name of the function which will make dependent files
/// not compile.
///
/// To model changes to "private" code in a file we change the body of a second function which
/// no one calls.
///
module FSharp.Test.ProjectGeneration

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open Xunit


let private projectRoot = __SOURCE_DIRECTORY__

let private defaultFunctionName = "f"


type SyntheticSourceFile =
    {
        Id: string
        /// This is part of the file's type name
        PublicVersion: int
        InternalVersion: int
        DependsOn: string list
        /// Changing this makes dependent files' code invalid
        FunctionName: string
        HasSignatureFile: bool
        HasErrors: bool
        EntryPoint: bool
    }

    member this.FileName = $"File{this.Id}.fs"
    member this.SignatureFileName = $"{this.FileName}i"

let sourceFile fileId deps =
    { Id = fileId
      PublicVersion = 1
      InternalVersion = 1
      DependsOn = deps
      FunctionName = defaultFunctionName
      HasSignatureFile = false
      HasErrors = false
      EntryPoint = false }

type SyntheticProject =
    { Name: string
      ProjectDir: string
      SourceFiles: SyntheticSourceFile list
      DependsOn: SyntheticProject list }

    member this.Find fileId =
        this.SourceFiles
        |> List.tryFind (fun f -> f.Id = fileId)
        |> Option.defaultWith (fun () -> failwith $"File with ID '{fileId}' not found in project {this.Name}.")

    member this.FindInAllProjects fileId =
        this.GetAllFiles()
        |> List.tryFind (fun (_, f) -> f.Id = fileId)
        |> Option.defaultWith (fun () -> failwith $"File with ID '{fileId}' not found in any project.")

    member this.FindByPath path =
        this.SourceFiles
        |> List.tryFind (fun f -> this.ProjectDir ++ f.FileName = path)
        |> Option.defaultWith (fun () -> failwith $"File {path} not found in project {this.Name}.")

    member this.ProjectFileName = this.ProjectDir ++ $"{this.Name}.fsproj"

    member this.OutputFilename = this.ProjectDir ++ $"{this.Name}.dll"

    member this.ProjectOptions =
        { ProjectFileName = this.ProjectFileName
          ProjectId = None
          SourceFiles =
            [| for f in this.SourceFiles do
                   if f.HasSignatureFile then
                       this.ProjectDir ++ f.SignatureFileName

                   this.ProjectDir ++ f.FileName |]
          OtherOptions =
            [|
                "--optimize+"
                for p in this.DependsOn do
                    $"-r:{p.OutputFilename}" |]
          ReferencedProjects =
            [| for p in this.DependsOn do
                 FSharpReferencedProject.CreateFSharp(p.OutputFilename, p.ProjectOptions) |]
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = false
          LoadTime = DateTime()
          UnresolvedReferences = None
          OriginalLoadReferences = []
          Stamp = None }

    member this.GetAllFiles() = [
        for f in this.SourceFiles do
            this, f
        for p in this.DependsOn do
            yield! p.GetAllFiles() ]


module Internal =

    let renderSourceFile (project: SyntheticProject) (f: SyntheticSourceFile) =
        seq {
            $"module %s{project.Name}.Module{f.Id}"

            for p in project.DependsOn do
                $"open {p.Name}"

            $"type T{f.Id}V_{f.PublicVersion}<'a> = T{f.Id} of 'a"

            $"let {f.FunctionName} x ="

            for dep in f.DependsOn do
                $"    Module{dep}.{defaultFunctionName} x,"

            $"    T{f.Id} x"

            $"let f2 x = x + {f.InternalVersion}"

            if f.HasErrors then
                "let wrong = 1 + 'a'"

            if f.EntryPoint then
                "[<EntryPoint>]"
                "let main _ ="
                "   f 1 |> ignore"
                "   printfn \"Hello World!\""
                "   0"
        }
        |> String.concat Environment.NewLine

    let renderFsProj (p: SyntheticProject) =
        seq {
            """
            <Project Sdk="Microsoft.NET.Sdk">

            <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net7.0</TargetFramework>
            </PropertyGroup>

            <ItemGroup>
            """

            for f in p.SourceFiles do
                if f.HasSignatureFile then
                    $"<Compile Include=\"{f.SignatureFileName}\" />"

                $"<Compile Include=\"{f.FileName}\" />"

            """
            </ItemGroup>
            </Project>
            """
        }
        |> String.concat Environment.NewLine

    let writeFileIfChanged path content =
        if not (File.Exists path) || File.ReadAllText(path) <> content then
            File.WriteAllText(path, content)

    let writeFile (p: SyntheticProject) (f: SyntheticSourceFile) =
        let fileName = p.ProjectDir ++ f.FileName
        let content = renderSourceFile p f
        writeFileIfChanged fileName content

    let validateFileIdsAreUnique (project: SyntheticProject) =
        let ids = [for _, f in project.GetAllFiles() -> f.Id]
        let duplicates = ids |> List.groupBy id |> List.filter (fun (_, g) -> g.Length > 1)
        if duplicates.Length > 0 then
            failwith $"""Source file IDs have to be unique across the project and all referenced projects. Found duplicates: {String.Join(", ", duplicates |> List.map fst)}"""


open Internal


[<AutoOpen>]
module ProjectOperations =

    let updateFile fileId updateFunction project =
        let index = project.SourceFiles |> List.findIndex (fun file -> file.Id = fileId)

        { project with
            SourceFiles =
                project.SourceFiles
                |> List.updateAt index (updateFunction project.SourceFiles[index]) }

    let updateFileInAnyProject fileId updateFunction (rootProject: SyntheticProject) =
        let project, _ =
            rootProject.GetAllFiles()
            |> List.tryFind (fun (_, f) -> f.Id = fileId)
            |> Option.defaultWith (fun () -> failwith $"File with ID '{fileId}' not found in any project")

        if project = rootProject then
            updateFile fileId updateFunction project
        else
            let index = rootProject.DependsOn |> List.findIndex ((=) project)
            { rootProject with
                DependsOn =
                    rootProject.DependsOn
                    |> List.updateAt index (updateFile fileId updateFunction project) }

    let private counter = (Seq.initInfinite id).GetEnumerator()

    let updatePublicSurface f =
        counter.MoveNext() |> ignore
        { f with PublicVersion = counter.Current }

    let updateInternal f =
        counter.MoveNext() |> ignore
        { f with InternalVersion = counter.Current }

    let breakDependentFiles f = { f with FunctionName = "g" }

    let setPublicVersion n f = { f with PublicVersion = n }

    let addDependency fileId f: SyntheticSourceFile =
        { f with DependsOn = fileId :: f.DependsOn }

    let checkFile fileId (project: SyntheticProject) (checker: FSharpChecker) =
        let file = project.Find fileId
        let contents = renderSourceFile project file
        let absFileName = project.ProjectDir ++ file.FileName
        checker.ParseAndCheckFileInProject(absFileName, 0, SourceText.ofString contents, project.ProjectOptions)

    let getTypeCheckResult (parseResults: FSharpParseFileResults, checkResults: FSharpCheckFileAnswer) =
        Assert.True(not parseResults.ParseHadErrors)

        match checkResults with
        | FSharpCheckFileAnswer.Aborted -> failwith "Type checking was aborted"
        | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults

    let getSignature parseAndCheckResults =
        match (getTypeCheckResult parseAndCheckResults).GenerateSignature() with
        | Some s -> s.ToString()
        | None -> ""

    let expectOk parseAndCheckResults _ =
        let checkResult = getTypeCheckResult parseAndCheckResults

        if checkResult.Diagnostics.Length > 0 then
            failwith $"Expected no errors, but there were some: \n%A{checkResult.Diagnostics}"

    let expectErrors parseAndCheckResults _ =
        let checkResult = getTypeCheckResult parseAndCheckResults

        if
            (checkResult.Diagnostics
             |> Array.where (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))
                .Length = 0
        then
            failwith "Expected errors, but there were none"

    let expectSignatureChanged result (oldSignature: string, newSignature: string) =
        expectOk result ()
        Assert.NotEqual<string>(oldSignature, newSignature)

    let expectSignatureContains expected result (_oldSignature, newSignature) =
        expectOk result ()
        Assert.Contains(expected, newSignature)

    let expectNoChanges result (oldSignature: string, newSignature: string) =
        expectOk result ()
        Assert.Equal<string>(oldSignature, newSignature)

    let rec saveProject (p: SyntheticProject) generateSignatureFiles checker =
        async {
            Directory.CreateDirectory(p.ProjectDir) |> ignore

            for ref in p.DependsOn do
                do! saveProject ref generateSignatureFiles checker

            for i in 0 .. p.SourceFiles.Length - 1 do
                let file = p.SourceFiles.[i]
                writeFile p file

                if file.HasSignatureFile && generateSignatureFiles then
                    let project = { p with SourceFiles = p.SourceFiles.[0..i] }
                    let! results = checkFile file.Id project checker
                    let signature = getSignature results
                    let signatureFileName = p.ProjectDir ++ file.SignatureFileName
                    writeFileIfChanged signatureFileName signature

            writeFileIfChanged (p.ProjectDir ++ $"{p.Name}.fsproj") (renderFsProj p)
        }

type WorkflowContext =
    { Project: SyntheticProject
      Signatures: Map<string, string> }

type ProjectWorkflowBuilder(initialProject: SyntheticProject, ?checker: FSharpChecker) =

    let checker = defaultArg checker (FSharpChecker.Create())

    let mapProject f workflow =
        async {
            let! ctx = workflow
            return { ctx with Project = f ctx.Project }
        }

    member this.Checker = checker

    member this.Yield _ =
        async {
            validateFileIdsAreUnique initialProject

            do! saveProject initialProject true checker

            let! results = checker.ParseAndCheckProject(initialProject.ProjectOptions)

            if not (Array.isEmpty results.Diagnostics) then
                failwith $"Project {initialProject.Name} failed initial check: \n%A{results.Diagnostics}"

            let! signatures =
                Async.Sequential
                    [ for file in initialProject.SourceFiles do
                          async {
                              let! result = checkFile file.Id initialProject checker
                              let signature = getSignature result
                              return file.Id, signature
                          } ]

            return
                { Project = initialProject
                  Signatures = Map signatures }
        }

    member this.Run(workflow: Async<WorkflowContext>) =
        try
            Async.RunSynchronously workflow
        finally
            if Directory.Exists initialProject.ProjectDir then
                Directory.Delete(initialProject.ProjectDir, true)

    /// Change contents of given file using `processFile` function.
    /// Does not save the file to disk.
    [<CustomOperation "updateFile">]
    member this.UpdateFile(workflow: Async<WorkflowContext>, fileId: string, processFile) =
        workflow |> mapProject (updateFileInAnyProject fileId processFile)

    /// Add a file above given file in the project.
    [<CustomOperation "addFileAbove">]
    member this.AddFileAbove(workflow: Async<WorkflowContext>, addAboveId: string, newFile) =
        workflow
        |> mapProject (fun project ->
            let index =
                project.SourceFiles
                |> List.tryFindIndex (fun f -> f.Id = addAboveId)
                |> Option.defaultWith (fun () -> failwith $"File {addAboveId} not found")

            { project with SourceFiles = project.SourceFiles |> List.insertAt index newFile })

    /// Remove a file from the project. The file is not deleted from disk.
    [<CustomOperation "removeFile">]
    member this.RemoveFile(workflow: Async<WorkflowContext>, fileId: string) =
        workflow
        |> mapProject (fun project ->
            { project with SourceFiles = project.SourceFiles |> List.filter (fun f -> f.Id <> fileId) })

    /// Parse and type check given file and process the results using `processResults` function.
    [<CustomOperation "checkFile">]
    member this.CheckFile(workflow: Async<WorkflowContext>, fileId: string, processResults) =
        async {
            let! ctx = workflow
            let! results = checkFile fileId ctx.Project checker

            let oldSignature = ctx.Signatures.[fileId]
            let newSignature = getSignature results

            processResults results (oldSignature, newSignature)

            return { ctx with Signatures = ctx.Signatures.Add(fileId, newSignature) }
        }

    /// Save given file to disk.
    [<CustomOperation "saveFile">]
    member this.SaveFile(workflow: Async<WorkflowContext>, fileId: string) =
        async {
            let! ctx = workflow
            let project, file = ctx.Project.FindInAllProjects fileId
            writeFile project file
            return ctx
        }

    /// Save all files to disk.
    [<CustomOperation "saveAll">]
    member this.SaveAll(workflow: Async<WorkflowContext>) =
        async {
            let! ctx = workflow
            do! saveProject ctx.Project false checker
            return ctx
        }

/// Execute a set of operations on a given synthetic project.
/// The project is saved to disk and type checked at the start.
let projectWorkflow project = ProjectWorkflowBuilder project
