module FSharpChecker.PathMap

open System.IO
open System.Threading.Tasks
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.ProjectGeneration
open FSharp.Test.Utilities
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
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

/// Compiles the library from its own directory, as a build does: the assembly records that directory as the one it
/// was compiled in.
let private emitLibrary (root: string) (pathMap: string list) =
    let projectDir = Directory.CreateDirectory(Path.Combine(root, "src", "Domain")).FullName
    File.WriteAllText(Path.Combine(projectDir, "Library.fs"), "module Library\n\nlet f x = x + 1\n")

    let result =
        runFscProcessIn projectDir [
            "--target:library"
            yield! CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions
            yield! pathMap
            "-o:Library.dll"
            "Library.fs"
        ]

    Assert.True((result.ExitCode = 0), $"fsc exited with {result.ExitCode}:\n{result.StdOut}\n{result.StdErr}")
    projectDir

/// Read back through the path map, a declaration names the file the library was compiled from, whatever the map.
/// The compiler writes each name under a path map with the mapped directory it compiled in at its start; a #line
/// name, which compilers before F# 10 wrote instead, is relative to that directory.
[<Theory>]
[<InlineData("")>]
[<InlineData(".")>]
[<InlineData("/_/")>]
let ``a declaration in a referenced assembly names the file it was compiled from`` (mappedRoot: string) : Task =
    task {
        let root = TestFramework.createTemporaryDirectory().FullName
        let pathMap = if mappedRoot = "" then [] else [ $"--pathmap:{root}={mappedRoot}" ]
        let projectDir = emitLibrary root pathMap

        let usageLine = "let _ = Library.f 1"
        let source = $"module Consumer\n{usageLine}\n"
        let consumer = Path.Combine(root, "Consumer.fs")
        File.WriteAllText(consumer, source)

        let options =
            let defaults = CompilerAssert.DefaultProjectOptions(TargetFramework.Current)

            { defaults with
                SourceFiles = [| consumer |]
                OtherOptions = [| yield! defaults.OtherOptions; $"""-r:{Path.Combine(projectDir, "Library.dll")}""" |] }

        let! _, answer = FSharpChecker.Create().ParseAndCheckFileInProject(consumer, 0, SourceText.ofString source, options)

        let checkResults =
            match answer with
            | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults
            | FSharpCheckFileAnswer.Aborted -> failwith "the check was aborted"

        let endOfName = usageLine.Length - " 1".Length
        let names = [ "Library"; "f" ]

        let declaration =
            match checkResults.GetDeclarationLocation(2, endOfName, usageLine, names) with
            | FindDeclResult.DeclFound range -> range
            | result -> failwith $"expected the declaration of Library.f, got %A{result}"

        // Since F# 10 the compiler writes the real file for a #line directive, so the name an older one wrote is
        // resolved against this assembly's recorded directory directly
        let fromLineDirective =
            let symbol = checkResults.GetSymbolUseAtLocation(2, endOfName, usageLine, names).Value.Symbol
            let range = Range.mkRange "Original.fs" declaration.Start declaration.End
            SymbolHelpers.fileNameOfItem symbol.SymbolEnv.g None range symbol.Item

        // A reader that knows the map turns a name back into the file it stands for, reading a rooted name as the
        // full path GetDeclarationLocation gives
        let compiledFrom (fileName: string) =
            let asRead (path: string) =
                if Path.IsPathRooted path then Path.GetFullPath path else path

            if mappedRoot = "" then
                Path.GetFullPath fileName
            else
                let fileName, mappedRoot = asRead fileName, asRead mappedRoot
                Assert.StartsWith(mappedRoot, fileName)
                Path.GetFullPath(Path.Combine(root, fileName.Substring(mappedRoot.Length).TrimStart('/', '\\')))

        Assert.Equal(Path.Combine(projectDir, "Library.fs"), compiledFrom declaration.FileName)
        Assert.Equal(Path.Combine(projectDir, "Original.fs"), compiledFrom fromLineDirective)
    }
