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

/// The file an App names as declaring the function it uses from the library, once another project has filled the
/// cache: the framework imports, and the TcGlobals with them, are cached per framework set.
let private libraryFileSeenAfter (useTransparentCompiler: bool) (cacheFiller: SyntheticProject) (library: SyntheticProject) =
    task {
        let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)

        let app =
            { SyntheticProject.Create("App", sourceFile "App" [ "Library" ]) with
                DependsOn = [ library ] }

        do! checkWith checker cacheFiller
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

        return libraryFunction.Symbol.DeclarationLocation.Value.FileName
    }

/// The path map of the project that filled the cache must not reach the ranges a sibling exposes to its consumers.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a sibling's path map does not reach the ranges of a project without one`` (useTransparentCompiler: bool) : Task =
    task {
        let library = SyntheticProject.Create("Library", sourceFile "Library" [])

        let mapped =
            { SyntheticProject.Create("Mapped", sourceFile "Mapped" []) with
                OtherOptions = [ $"--pathmap:{Path.GetDirectoryName library.ProjectDir}=.\\" ] }

        let! declaredIn = libraryFileSeenAfter useTransparentCompiler mapped library

        Assert.Equal(library.GetFilePath "Library", declaredIn)
    }

/// Nor may a project lose its own path map to a sibling without one that filled the cache first.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``a project's path map reaches its ranges after a sibling without one filled the cache`` (useTransparentCompiler: bool) : Task =
    task {
        let unmapped = SyntheticProject.Create("Unmapped", sourceFile "Unmapped" [])
        let library = SyntheticProject.Create("Library", sourceFile "Library" [])
        let root = Path.GetDirectoryName library.ProjectDir

        let library =
            { library with
                OtherOptions = [ $"--pathmap:{root}=." ] }

        let! declaredIn = libraryFileSeenAfter useTransparentCompiler unmapped library

        Assert.Equal("." + (library.GetFilePath "Library").Substring(root.Length), declaredIn)
    }

let private builderSource =
    """namespace Builders

type Builder() =
    member _.Yield(_: unit) : int list = []

    [<CustomOperation("choose")>]
    member _.Choose(xs: int list, value: int) = value :: xs

module Instance =
    let builder = Builder()
"""

let private compileLibrary (root: string) (mappedRoot: string) (withSignature: bool) =
    let projectDir = Directory.CreateDirectory(Path.Combine(root, "src", "Domain")).FullName

    let sources =
        [
            if withSignature then
                "Library.fsi", "module Library\n\nmodule Nested =\n    val f: x: int -> int\n"
            "Library.fs", "module Library\n\nmodule Nested =\n    let f x = x + 1\n"
            "Builder.fs", builderSource
            "Info.fs", "namespace Builders\n\nopen System.Reflection\n\n[<assembly: AssemblyTitle(\"Library\")>]\ndo ()\n"
        ]

    for fileName, text in sources do
        File.WriteAllText(Path.Combine(projectDir, fileName), text)

    let result =
        runFscProcessIn projectDir [
            "--target:library"
            yield! CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions
            if mappedRoot <> "" then
                $"--pathmap:{root}={mappedRoot}"
            "-o:Library.dll"
            yield! List.map fst sources
        ]

    Assert.True((result.ExitCode = 0), $"fsc exited with {result.ExitCode}:\n{result.StdOut}\n{result.StdErr}")
    projectDir

let private checkAgainst (library: string) (source: string) =
    FSharp source |> withOptions [ $"-r:{library}" ] |> typecheckResults

let private locate (source: string) (name: string) =
    let lines = source.Split '\n'
    let index = lines |> Array.findIndex (fun line -> line.IndexOf(name, System.StringComparison.Ordinal) >= 0)
    let line = lines[index]
    struct (index + 1, line.IndexOf(name, System.StringComparison.Ordinal) + name.Length, line, List.ofArray (name.Split '.'))

let private declarationOf (results: FSharpCheckFileResults) (source: string) (name: string) (preferSignature: bool) =
    let struct (line, column, text, names) = locate source name

    match results.GetDeclarationLocation(line, column, text, names, preferSignature) with
    | FindDeclResult.DeclFound range -> range.FileName
    | result -> failwith $"expected the declaration of {name}, got %A{result}"

let private fileNameResolvedFrom (results: FSharpCheckFileResults) (source: string) (name: string) (recorded: string) =
    let struct (line, column, text, names) = locate source name
    let symbol = results.GetSymbolUseAtLocation(line, column, text, names).Value.Symbol
    SymbolHelpers.fileNameOfItem symbol.SymbolEnv.g None (Range.rangeN recorded line) symbol.Item

[<Theory>]
[<InlineData("", false)>]
[<InlineData(".", false)>]
[<InlineData(".", true)>]
[<InlineData("/_/", false)>]
let ``a declaration in a referenced assembly names the file it was compiled from`` (mappedRoot: string) (withSignature: bool) =
    let root = TestFramework.createTemporaryDirectory().FullName
    let projectDir = compileLibrary root mappedRoot withSignature
    let source = "let _ = Library.Nested.f 1\nlet _ = Builders.Instance.builder { choose 1 }"
    let results = checkAgainst (Path.Combine(projectDir, "Library.dll")) source

    let compiledFrom (fileName: string) =
        let fullIfRooted (path: string) =
            if Path.IsPathRooted path then Path.GetFullPath path else path

        let name = fullIfRooted fileName
        let mapped = fullIfRooted (if mappedRoot = "" then root else mappedRoot)
        Assert.StartsWith(mapped, name)
        Path.GetFullPath(Path.Combine(root, name.Substring(mapped.Length).TrimStart('/', '\\')))

    let assertDeclaredIn name file preferSignature =
        Assert.Equal(Path.Combine(projectDir, file), compiledFrom (declarationOf results source name preferSignature))

    let assertRelativeToCompileDirectory name recorded =
        Assert.Equal(
            Path.GetFullPath(Path.Combine(projectDir, recorded)),
            compiledFrom (fileNameResolvedFrom results source name recorded)
        )

    assertDeclaredIn "Library.Nested.f" "Library.fs" false
    assertDeclaredIn "choose" "Builder.fs" false
    assertDeclaredIn "Builders" "Info.fs" false
    assertRelativeToCompileDirectory "Library.Nested.f" "Original.fs"
    assertRelativeToCompileDirectory "choose" (Path.Combine(".", "src", "Domain", "Original.fs"))

    if withSignature then
        assertDeclaredIn "Library.Nested.f" "Library.fsi" true

/// PathMapFSharp9.dll: FSharp.Compiler.Service 43.9.300 compiling, in a root's src\Domain with --pathmap:root=. --deterministic+,
///     module Library
///     let ordinary x = x + 1
///     #line 1 ".\src\Domain\Original.fs"
///     let fromLine x = x + 2
///     module Nested =
///         let fromLineNested x = x + 3
[<Theory>]
[<InlineData("Library.ordinary", @".\src\Domain\Library.fs", false)>]
[<InlineData("Library.fromLine", @".\src\Domain\Original.fs", true)>]
[<InlineData("Library.Nested.fromLineNested", @".\src\Domain\Original.fs", true)>]
let ``a #line name an older compiler wrote keeps the directory that the name of a compiled file already starts with``
    (name: string)
    (recorded: string)
    (keepsDirectory: bool)
    =
    let source = $"let _ = {name} 1"
    let results = checkAgainst (Path.Combine(__SOURCE_DIRECTORY__, "PathMapFSharp9.dll")) source
    let expected = if keepsDirectory then Path.Combine(@".\src\Domain", recorded) else recorded
    Assert.Equal(expected, declarationOf results source name false)
