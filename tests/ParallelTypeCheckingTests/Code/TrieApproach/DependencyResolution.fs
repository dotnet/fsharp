module ParallelTypeCheckingTests.Code.TrieApproach.DependencyResolution

open FSharp.Compiler.Syntax

// This is pseudo code of how we could restructure the trie code
// My main benefit is that you can easily visually inspect if an identifier will match something in the trie

// This code just looks for a path in the trie
// It could be cached and is easy to reason about.
let queryTrie (trie: TrieNode) (path: ModuleSegment list) : QueryTrieNodeResult =
    let rec visit (currentNode: TrieNode) (path: ModuleSegment list) =
        match path with
        | [] -> failwith "path should not be empty"
        | [ lastNodeFromPath ] ->
            let childResults =
                currentNode.Children
                |> Seq.tryFind (fun (KeyValue (segment, _childNode)) -> segment = lastNodeFromPath)

            match childResults with
            | None -> QueryTrieNodeResult.NodeDoesNotExist
            | Some (KeyValue (_, childNode)) ->
                if Set.isEmpty childNode.Files then
                    QueryTrieNodeResult.NodeDoesNotExposeData
                else
                    QueryTrieNodeResult.NodeExposesData(childNode.Files)
        | currentPath :: restPath ->
            let childResults =
                currentNode.Children
                |> Seq.tryFind (fun (KeyValue (segment, _childNode)) -> segment = currentPath)

            match childResults with
            | None -> QueryTrieNodeResult.NodeDoesNotExist
            | Some (KeyValue (_, childNode)) -> visit childNode restPath

    visit trie path

// Now how to detect the deps between files?
// Process the content of each file using some state

// Helper function to process a open statement
// The statement could link to files and/or should be tracked as an open namespace
let processOpenPath (trie: TrieNode) (path: ModuleSegment list) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie trie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOpenNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependenciesAndOpenNamespace(files, path)

// Helper function to process an identifier
let processIdentifier (trie: TrieNode) (path: ModuleSegment list) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie trie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData ->
        // This can occur when you are have a file that uses a known namespace (for example namespace System).
        // When any other code uses that System namespace it won't find anything in the user code.
        state
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependencies files

// Typically used to folder FileContentEntry items over a FileContentQueryState
let rec processStateEntry (trie: TrieNode) (state: FileContentQueryState) (entry: FileContentEntry) : FileContentQueryState =
    match entry with
    | FileContentEntry.TopLevelNamespace (topLevelPath, content) ->
        let state =
            match topLevelPath with
            | [] -> state
            | _ -> processOpenPath trie topLevelPath state

        List.fold (processStateEntry trie) state content

    | FileContentEntry.OpenStatement path ->
        // An open statement can directly reference file or be a partial open statement
        // Both cases need to be processed.
        let stateAfterFullOpenPath = processOpenPath trie path state

        // Any existing open statement could be extended with the current path (if that node where to exists in the trie)
        // The extended path could add a new link (in case of a module or namespace with types)
        // It might also not add anything at all (in case it the extended path is still a partial one)
        (stateAfterFullOpenPath, state.OpenNamespaces)
        ||> Seq.fold (fun acc openNS -> processOpenPath trie [ yield! openNS; yield! path ] acc)

    | FileContentEntry.PrefixedIdentifier path ->
        // process the name was if it were a FQN
        let stateAfterFullIdentifier = processIdentifier trie path state

        // Process the name in combination with the existing open namespaces
        (stateAfterFullIdentifier, state.OpenNamespaces)
        ||> Seq.fold (fun acc openNS -> processIdentifier trie [ yield! openNS; yield! path ] acc)

    | FileContentEntry.NestedModule (nestedContent = nestedContent) ->
        // We don't want our current state to be affect by any open statements in the nested module
        let nestedState = List.fold (processStateEntry trie) state nestedContent
        // Afterward we are only interested in the found dependencies in the nested module
        let foundDependencies =
            Set.union state.FoundDependencies nestedState.FoundDependencies

        { state with
            FoundDependencies = foundDependencies
        }

let getFileNameBefore (files: FileWithAST array) idx =
    files.[0 .. (idx - 1)] |> Array.map (fun f -> f.File) |> Set.ofArray

let mkGraph (files: FileWithAST array) =
    let trie =
        let input =
            files
            |> Array.filter (fun f ->
                match f.AST with
                | ParsedInput.SigFile _ -> true
                | ParsedInput.ImplFile _ -> Array.forall (fun (sigFile: FileWithAST) -> sigFile.File <> $"{f.File}i") files)

        TrieMapping.mkTrie input

    let fileContents = Array.Parallel.map FileContentMapping.mkFileContent files

    files
    |> Array.map (fun (file: FileWithAST) ->
        let fileContent = fileContents.[file.Idx]
        let knownFiles = getFileNameBefore files file.Idx

        let result =
            Seq.fold (processStateEntry trie) (FileContentQueryState.Create file.File knownFiles) fileContent

        file.File, Set.toArray result.FoundDependencies)

// =============================================================================================================
// =============================================================================================================

open NUnit.Framework
open FSharp.Compiler.Service.Tests.Common

[<Test>]
let ``Fantomas.Core for realzies`` () =
    let files =
        [|
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\obj\Debug\netstandard2.0\.NETStandard,Version=v2.0.AssemblyAttributes.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\obj\Debug\netstandard2.0\Fantomas.Core.AssemblyInfo.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\AssemblyInfo.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\ISourceTextExtensions.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\RangeHelpers.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\AstExtensions.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\AstExtensions.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\TriviaTypes.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Utils.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\SourceParser.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\AstTransformer.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\AstTransformer.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Version.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Queue.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\FormatConfig.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Defines.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Defines.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Trivia.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Trivia.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\SourceTransformer.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Context.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodePrinter.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodePrinter.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodeFormatterImpl.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodeFormatterImpl.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Validation.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Selection.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\Selection.fs"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodeFormatter.fsi"
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodeFormatter.fs"
        |]

    let filesWithAST =
        files
        |> Array.mapi (fun idx file ->
            {
                Idx = idx
                AST = parseSourceCode (file, System.IO.File.ReadAllText(file))
                File = file
            })

    let graph = mkGraph filesWithAST

    for fileName, deps in graph do
        let depString = String.concat "\n    " deps

        if deps.Length = 0 then
            printfn $"%s{fileName}: []"
        else
            printfn $"%s{fileName}:\n    {depString}"
