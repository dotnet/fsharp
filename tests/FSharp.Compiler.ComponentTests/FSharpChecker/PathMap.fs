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

/// Compiles the library from its own directory, as a build does: the assembly records that directory as the one it
/// was compiled in.
let private emitLibrary (root: string) (pathMap: string list) (withSignature: bool) =
    let projectDir = Directory.CreateDirectory(Path.Combine(root, "src", "Domain")).FullName
    File.WriteAllText(Path.Combine(projectDir, "Library.fs"), "module Library\n\nlet f x = x + 1\n")

    if withSignature then
        File.WriteAllText(Path.Combine(projectDir, "Library.fsi"), "module Library\n\nval f: x: int -> int\n")

    // The arguments are joined into one command line, where a reference under "Program Files" or a root with a
    // space in it would fall apart
    let quoted (option: string) = $"\"{option}\""

    let result =
        runFscProcessIn projectDir [
            "--target:library"
            for option in CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions do
                quoted option
            for option in pathMap do
                quoted option
            "-o:Library.dll"
            if withSignature then
                "Library.fsi"
            "Library.fs"
        ]

    Assert.True((result.ExitCode = 0), $"fsc exited with {result.ExitCode}:\n{result.StdOut}\n{result.StdErr}")
    projectDir

/// Read back through the path map, a declaration names the file the library was compiled from, whatever the map.
/// The compiler writes each name under a path map with the mapped directory it compiled in at its start; a #line
/// name, which compilers before F# 10 wrote instead, is relative to that directory. With a signature, the module
/// is read from the one file and the implementation of its function is in the other.
[<Theory>]
[<InlineData("", false)>]
[<InlineData(".", false)>]
[<InlineData(".", true)>]
[<InlineData("/_/", false)>]
let ``a declaration in a referenced assembly names the file it was compiled from`` (mappedRoot: string) (withSignature: bool) : Task =
    task {
        let root = TestFramework.createTemporaryDirectory().FullName
        let pathMap = if mappedRoot = "" then [] else [ $"--pathmap:{root}={mappedRoot}" ]
        let projectDir = emitLibrary root pathMap withSignature

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

        let declarationOf (preferSignature: bool) =
            match checkResults.GetDeclarationLocation(2, endOfName, usageLine, names, preferSignature) with
            | FindDeclResult.DeclFound range -> range
            | result -> failwith $"expected the declaration of Library.f, got %A{result}"

        let declaration = declarationOf false

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

        if withSignature then
            Assert.Equal(Path.Combine(projectDir, "Library.fsi"), compiledFrom (declarationOf true).FileName)
    }

/// PathMapFSharp9.dll, next to this file, is what the F# 9 compiler (FSharp.Compiler.Service 43.9.300) made of
///
///     module Library
///
///     let ordinary x = x + 1
///
///     #line 1 ".\src\Domain\Original.fs"
///     let fromLine x = x + 2
///
///     module Nested =
///         let fromLineNested x = x + 3
///
/// in a root's src\Domain, with --pathmap:root=. and --deterministic+. Before F# 10 the compiler named whatever
/// follows a #line directive by the directive: here that name starts with the mapped directory, as the name of
/// the compiled file does, and yet is relative to it.
[<Theory>]
[<InlineData("ordinary", @".\src\Domain\Library.fs", false)>]
[<InlineData("fromLine", @".\src\Domain\Original.fs", true)>]
[<InlineData("Nested.fromLineNested", @".\src\Domain\Original.fs", true)>]
let ``a #line name an older compiler wrote keeps the directory that the name of a compiled file already starts with``
    (name: string)
    (recorded: string)
    (keepsDirectory: bool)
    : Task =
    task {
        let usageLine = $"let _ = Library.{name}"
        let source = $"module Consumer\n{usageLine} 1\n"
        let consumer = Path.Combine(TestFramework.createTemporaryDirectory().FullName, "Consumer.fs")
        File.WriteAllText(consumer, source)

        let options =
            let defaults = CompilerAssert.DefaultProjectOptions(TargetFramework.Current)

            { defaults with
                SourceFiles = [| consumer |]
                OtherOptions = [| yield! defaults.OtherOptions; $"""-r:{Path.Combine(__SOURCE_DIRECTORY__, "PathMapFSharp9.dll")}""" |] }

        let! _, answer = FSharpChecker.Create().ParseAndCheckFileInProject(consumer, 0, SourceText.ofString source, options)

        let checkResults =
            match answer with
            | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults
            | FSharpCheckFileAnswer.Aborted -> failwith "the check was aborted"

        let declaration =
            match checkResults.GetDeclarationLocation(2, usageLine.Length, $"{usageLine} 1", "Library" :: List.ofArray (name.Split '.')) with
            | FindDeclResult.DeclFound range -> range
            | result -> failwith $"expected the declaration of Library.{name}, got %A{result}"

        let expected =
            if keepsDirectory then Path.Combine(@".\src\Domain", recorded) else recorded

        Assert.Equal(expected, declaration.FileName)
    }
