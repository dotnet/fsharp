module ParallelTypeCheckingTests.Tests.TypedTreeGraph

open System
open System.Collections.Generic
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Symbols
open NUnit.Framework
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.Utils
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.DepResolving
open ParallelTypeCheckingTests.TestUtils

type Codebase = { WorkDir: string; Path: string }

let codebases =
    [|
        {
            WorkDir = $@"{__SOURCE_DIRECTORY__}\.fcs_test\src\compiler"
            Path = $@"{__SOURCE_DIRECTORY__}\FCS.args.txt"
        }
        {
            WorkDir = $@"{__SOURCE_DIRECTORY__}\.fcs_test\tests\FSharp.Compiler.ComponentTests"
            Path = $@"{__SOURCE_DIRECTORY__}\ComponentTests.args.txt"
        }
        // Hard coded example ;)
        {
            WorkDir = @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core"
            Path = @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\args.txt"
        }
        {
            WorkDir = @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core.Tests"
            Path = @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core.Tests\args.txt"
        }
    |]

let checker = FSharpChecker.Create(keepAssemblyContents = true)

type DepCollector(projectRoot: string, projectFile: string) =
    let deps = HashSet<string>()

    member this.Add(declarationLocation: range) : unit =
        let sourceLocation = Path.GetFullPath declarationLocation.FileName
        let ext = Path.GetExtension sourceLocation

        if
            (ext = ".fs" || ext = ".fsi")
            && sourceLocation.StartsWith projectRoot
            && sourceLocation <> projectFile
        then
            deps.Add(sourceLocation.Substring(projectRoot.Length + 1)) |> ignore

    member this.Deps = Seq.toArray deps

let rec collectFromSymbol (collector: DepCollector) (s: FSharpSymbol) =
    match s with
    | :? FSharpMemberOrFunctionOrValue as mfv ->
        if mfv.ImplementationLocation.IsSome || mfv.SignatureLocation.IsSome then
            collector.Add mfv.DeclarationLocation

        collectFromSymbol collector mfv.ReturnParameter

        for cpg in mfv.CurriedParameterGroups do
            for p in cpg do
                collectFromSymbol collector p

    | :? FSharpParameter as fp ->
        if fp.Type.HasTypeDefinition then
            collector.Add fp.Type.TypeDefinition.DeclarationLocation

    | :? FSharpEntity as e ->
        if
            not (e.IsFSharpModule || e.IsNamespace)
            && (e.ImplementationLocation.IsSome || e.SignatureLocation.IsSome)
        then
            collector.Add e.DeclarationLocation

    | :? FSharpActivePatternCase as apc -> collector.Add apc.DeclarationLocation
    | _ -> ()

// Fair warning: this isn't fast or optimized code
let graphFromTypedTree
    (checker: FSharpChecker)
    (projectDir: string)
    (projectOptions: FSharpProjectOptions)
    : Async<Dictionary<string, File> * DepsGraph> =
    async {
        let files = Dictionary<string, File>()

        let! filesWithDeps =
            projectOptions.SourceFiles
            |> Array.mapi (fun idx fileName ->
                async {
                    let sourceText = (File.ReadAllText >> SourceText.ofString) fileName
                    let! parseResult, checkResult = checker.ParseAndCheckFileInProject(fileName, 1, sourceText, projectOptions)

                    let isFisBacked =
                        not (fileName.EndsWith(".fsi"))
                        && (Array.exists (fun (option: string) -> option.Contains($"{fileName}i")) projectOptions.OtherOptions
                            || Array.exists (fun (file: string) -> file.Contains($"{fileName}i")) projectOptions.SourceFiles)

                    match checkResult with
                    | FSharpCheckFileAnswer.Aborted -> return failwith "aborted"
                    | FSharpCheckFileAnswer.Succeeded fileResult ->
                        let allSymbols = fileResult.GetAllUsesOfAllSymbolsInFile() |> Seq.toArray
                        let fullPath = Path.GetFullPath fileName
                        let collector = DepCollector(projectDir, fullPath)

                        for s in allSymbols do
                            collectFromSymbol collector s.Symbol

                        let file: File =
                            {
                                Idx = FileIdx.make idx
                                AST = ASTOrFsix.AST parseResult.ParseTree
                                FsiBacked = isFisBacked
                            }

                        files.Add(Path.GetRelativePath(projectDir, fileName), file)

                        return (file, collector.Deps)
                })
            |> Async.Parallel

        let graph =
            filesWithDeps
            |> Seq.sortBy (fun (file, _) -> file.Idx.Idx)
            |> Seq.map (fun (file, deps) ->
                let depsAsFiles = deps |> Array.map (fun dep -> files.[dep])
                file, depsAsFiles)
            |> readOnlyDict

        return files, graph
    }

[<TestCaseSource(nameof codebases)>]
[<Explicit("Slow! Only useful as a sanity check that the test codebase is sound.")>]
let ``Create Graph from typed tree`` (code: Codebase) =
    let previousDir = Environment.CurrentDirectory

    async {
        try
            Environment.CurrentDirectory <- code.WorkDir

            let args = File.ReadAllLines(code.Path) |> Array.map replacePaths
            let fileName = Path.GetFileNameWithoutExtension(args.[0].Replace("-o:", ""))

            let sourceFiles, otherOptions =
                args
                |> Array.partition (fun option ->
                    not (option.StartsWith("-"))
                    && (option.EndsWith(".fs") || option.EndsWith(".fsi")))

            let otherOptions =
                otherOptions
                |> Array.map (fun otherOption ->
                    // The reference to fsharp code needs to be an absolute one
                    if otherOption.StartsWith("-r:..") then
                        let absoluteBit = otherOption.Split(':').[1]
                        $"-r:{Path.Combine(code.WorkDir, absoluteBit)}"
                    else
                        otherOption)

            let proj =
                {
                    ProjectFileName = fileName
                    ProjectId = None
                    SourceFiles = sourceFiles
                    OtherOptions = otherOptions
                    ReferencedProjects = [||]
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = false
                    LoadTime = DateTime.Now
                    UnresolvedReferences = None
                    OriginalLoadReferences = []
                    Stamp = None
                }

            let! files, graphFromTypedTree = graphFromTypedTree checker code.WorkDir proj
            let path = $"{fileName}.typed-tree.deps.json"
            graphFromTypedTree |> Graph.map (fun n -> n.Name) |> Graph.serialiseToJson path

            let fileIndexToName =
                files.Values |> Seq.map (fun file -> file.Idx.Idx, file.Name) |> dict

            let sourceFiles =
                files.Values
                |> Seq.sortBy (fun file -> file.Idx.Idx)
                |> Seq.map (fun file ->
                    let ast =
                        match file.AST with
                        | ASTOrFsix.AST ast -> ast
                        | ASTOrFsix.Fsix _ -> failwith "unexpected fsix"

                    { Idx = file.Idx; AST = ast }: SourceFile)
                |> Seq.toArray

            let graphFromHeuristic = DependencyResolution.detectFileDependencies sourceFiles
            let path = $"{fileName}.deps.json"

            graphFromHeuristic.Graph
            |> Graph.map (fun n -> n.Name)
            |> Graph.serialiseToJson path

            Assert.True(graphFromTypedTree.Count = graphFromHeuristic.Graph.Count, "Both graphs should have the same amount of entries.")

            let depNames (depIdxs: int seq) =
                depIdxs
                |> Seq.map (fun idx -> fileIndexToName.[idx] |> Path.GetFileName)
                |> String.concat ", "

            let collectAllDeps (graph: DepsGraph) =
                let getKeyByIdx idx =
                    graph.Keys |> Seq.find (fun file -> file.Idx.Idx = idx)

                (Map.empty, [ 0 .. (sourceFiles.Length - 1) ])
                ||> List.fold (fun acc idx ->
                    let deps = graph.[getKeyByIdx idx]

                    let allDeps =
                        set
                            [|
                                yield! (Seq.map (fun dep -> dep.Idx.Idx) deps)
                                yield! (Seq.collect (fun dep -> Map.find dep.Idx.Idx acc) deps)
                            |]

                    Map.add idx allDeps acc)

            let typedTreeMap = collectAllDeps graphFromTypedTree
            let heuristicMap = collectAllDeps graphFromHeuristic.Graph

            /// Compare the found dependencies of a specified heuristic versus the dependencies found in the typed tree
            let compareDeps source fileName idx (depsFromHeuristic: Set<int>) =
                let depsFromTypedTree = Map.find idx typedTreeMap

                if Set.isEmpty depsFromTypedTree && not (Set.isEmpty depsFromHeuristic) then
                    printfn $"{source}:{fileName} has %A{(depNames depsFromHeuristic)} while the typed tree had none!"
                else
                    let isSuperSet = Set.isSuperset depsFromHeuristic depsFromTypedTree
                    let delta = Set.difference depsFromTypedTree depsFromHeuristic

                    Assert.IsTrue(
                        isSuperSet,
                        $"""{fileName} did not contain a superset of the typed tree dependencies:
{source} is missing dependencies: %A{depNames delta}."""
                    )

            graphFromHeuristic.Graph.Keys
            |> Seq.toArray
            |> Array.Parallel.iter (fun file -> compareDeps "Current heuristic" file.Name file.Idx.Idx (Map.find file.Idx.Idx heuristicMap))

            let alternativeGraphFromHeuristic =
                let sourceFiles =
                    files.Values
                    |> Seq.sortBy (fun file -> file.Idx.Idx)
                    |> Seq.map (fun file ->
                        let ast =
                            match file.AST with
                            | ASTOrFsix.AST ast -> ast
                            | ASTOrFsix.Fsix _ -> failwith "unexpected fsix"

                        {
                            Idx = file.Idx.Idx
                            File = file.Name
                            AST = ast
                        }: ParallelTypeCheckingTests.Code.TrieApproach.FileWithAST)
                    |> Seq.toArray

                ParallelTypeCheckingTests.Code.TrieApproach.DependencyResolution.mkGraph sourceFiles

            let alternateMap =
                let getDepsByIdx idx =
                    alternativeGraphFromHeuristic
                    |> Seq.find (fun (file, _) -> file.Idx = idx)
                    |> snd

                (Map.empty, [ 0 .. (sourceFiles.Length - 1) ])
                ||> List.fold (fun acc idx ->
                    let deps = getDepsByIdx idx

                    let allDeps =
                        set [| yield! deps; yield! (Seq.collect (fun dep -> Map.find dep acc) deps) |]

                    Map.add idx allDeps acc)

            let rec getAllDepsFromAlt idx : Set<int> =
                let currentDeps =
                    alternativeGraphFromHeuristic
                    |> Seq.find (fun (file, _) -> file.Idx = idx)
                    |> snd

                let transitiveDeps = Seq.collect getAllDepsFromAltMemoized currentDeps

                set [| yield! currentDeps; yield! transitiveDeps |]

            and getAllDepsFromAltMemoized =
                Internal.Utilities.Library.Tables.memoize getAllDepsFromAlt

            Array.Parallel.iter
                (fun (file: ParallelTypeCheckingTests.Code.TrieApproach.FileWithAST, _) ->
                    compareDeps "Alternative heuristic" file.File file.Idx (Map.find file.Idx alternateMap))
                alternativeGraphFromHeuristic

        finally
            Environment.CurrentDirectory <- previousDir
    }
