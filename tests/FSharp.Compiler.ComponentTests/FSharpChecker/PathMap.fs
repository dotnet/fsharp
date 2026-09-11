module FSharpChecker.PathMap

open System.IO
open System.Threading.Tasks
open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

let private checkWith (checker: FSharpChecker) (project: SyntheticProject) =
    ProjectWorkflowBuilder(project, checker = checker).Yield() |> Async.Ignore

/// The framework imports, and the TcGlobals with them, are cached per framework set; the path map of
/// the project that filled the cache must not reach the ranges a sibling exposes to its consumers.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a sibling's path map does not reach the ranges of a project without one`` (useTransparentCompiler: bool) : Task =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
        let library = SyntheticProject.Create("Library", sourceFile "Library" [])

        let mapped =
            { SyntheticProject.Create("Mapped", sourceFile "Mapped" []) with
                OtherOptions = [ $"--pathmap:{Path.GetDirectoryName library.ProjectDir}=.\\" ] }

        let app =
            { SyntheticProject.Create("App", sourceFile "App" [ "Library" ]) with
                DependsOn = [ library ] }

        do! checkWith checker mapped
        do! checkWith checker app

        let appFile = app.GetFilePath "App"

        let! _, answer =
            checker.ParseAndCheckFileInProject(
                appFile,
                0,
                SourceText.ofString (File.ReadAllText appFile),
                app.GetProjectOptions checker
            )

        let checkResults =
            match answer with
            | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults
            | FSharpCheckFileAnswer.Aborted -> failwith "the check was aborted"

        let libraryFunction =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Seq.tryFind (fun symbolUse -> symbolUse.Symbol.FullName = $"{library.Name}.ModuleLibrary.f")
            |> Option.defaultWith (fun () ->
                failwith
                    $"""ModuleLibrary.f not used; symbols: %A{checkResults.GetAllUsesOfAllSymbolsInFile() |> Seq.map _.Symbol.FullName |> Seq.distinct |> List.ofSeq}""")

        Assert.Equal(library.GetFilePath "Library", libraryFunction.Symbol.DeclarationLocation.Value.FileName)
    }

/// A build that maps its source paths maps the directory it compiled in as well, so the file name of a
/// range and that directory both reach the same root on their own. Joining them names the directory twice.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a declaration in a project with a path map is named from the root once`` (useTransparentCompiler: bool) : Task =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
        let library = SyntheticProject.Create("MappedLibrary", sourceFile "Library" [])
        let root = Path.GetDirectoryName library.ProjectDir

        let library =
            { library with
                OtherOptions = [ $"--pathmap:{root}=.{Path.DirectorySeparatorChar}" ] }

        let app =
            { SyntheticProject.Create("MappedApp", sourceFile "App" [ "Library" ]) with
                DependsOn = [ library ] }

        do! checkWith checker library
        do! checkWith checker app

        let appFile = app.GetFilePath "App"
        let appLines = File.ReadAllLines appFile

        let! _, answer =
            checker.ParseAndCheckFileInProject(
                appFile,
                0,
                SourceText.ofString (File.ReadAllText appFile),
                app.GetProjectOptions checker
            )

        let checkResults =
            match answer with
            | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults
            | FSharpCheckFileAnswer.Aborted -> failwith "the check was aborted"

        let usage =
            checkResults.GetAllUsesOfAllSymbolsInFile()
            |> Seq.find (fun symbolUse -> symbolUse.Symbol.FullName = $"{library.Name}.ModuleLibrary.f")

        let declaration =
            checkResults.GetDeclarationLocation(
                usage.Range.EndLine,
                usage.Range.EndColumn,
                appLines[usage.Range.EndLine - 1],
                [ "ModuleLibrary"; "f" ]
            )

        match declaration with
        | FindDeclResult.DeclFound range ->
            Assert.Equal(library.GetFilePath "Library", Path.GetFullPath(Path.Combine(root, range.FileName)))
        | result -> failwith $"expected the declaration of ModuleLibrary.f, got %A{result}"
    }
