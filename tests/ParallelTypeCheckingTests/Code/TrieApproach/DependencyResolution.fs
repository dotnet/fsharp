module ParallelTypeCheckingTests.Code.TrieApproach.DependencyResolution
module ParallelTypeCheckingTests.Code.TrieApproach.DependencyResolution

open System.Linq
open FSharp.Compiler.Syntax
open Internal.Utilities.Library.Extras

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

let queryTrieMemoized (trie: TrieNode) : QueryTrie =
    Internal.Utilities.Library.Tables.memoize (queryTrie trie)

// Now how to detect the deps between files?
// Process the content of each file using some state

// Helper function to process a open statement
// The statement could link to files and/or should be tracked as an open namespace
let processOpenPath (queryTrie: QueryTrie) (path: ModuleSegment list) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData -> state.AddOpenNamespace path
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependenciesAndOpenNamespace(files, path)

// Helper function to process an identifier
let processIdentifier (queryTrie: QueryTrie) (path: ModuleSegment list) (state: FileContentQueryState) : FileContentQueryState =
    let queryResult = queryTrie path

    match queryResult with
    | QueryTrieNodeResult.NodeDoesNotExist -> state
    | QueryTrieNodeResult.NodeDoesNotExposeData ->
        // This can occur when you are have a file that uses a known namespace (for example namespace System).
        // When any other code uses that System namespace it won't find anything in the user code.
        state
    | QueryTrieNodeResult.NodeExposesData files -> state.AddDependencies files

// Typically used to folder FileContentEntry items over a FileContentQueryState
let rec processStateEntry (queryTrie: QueryTrie) (state: FileContentQueryState) (entry: FileContentEntry) : FileContentQueryState =
    match entry with
    | FileContentEntry.TopLevelNamespace (topLevelPath, content) ->
        let state =
            match topLevelPath with
            | [] -> state
            | _ -> processOpenPath queryTrie topLevelPath state

        List.fold (processStateEntry queryTrie) state content

    | FileContentEntry.OpenStatement path ->
        // An open statement can directly reference file or be a partial open statement
        // Both cases need to be processed.
        let stateAfterFullOpenPath = processOpenPath queryTrie path state

        // Any existing open statement could be extended with the current path (if that node where to exists in the trie)
        // The extended path could add a new link (in case of a module or namespace with types)
        // It might also not add anything at all (in case it the extended path is still a partial one)
        (stateAfterFullOpenPath, state.OpenNamespaces)
        ||> Seq.fold (fun acc openNS -> processOpenPath queryTrie [ yield! openNS; yield! path ] acc)

    | FileContentEntry.PrefixedIdentifier path ->
        match path with
        | [] ->
            // should not be possible though
            state
        | _ ->
            // path could consist out of multiple segments
            (state, [| 1 .. path.Length |])
            ||> Seq.fold (fun state takeParts ->
                let path = List.take takeParts path
                // process the name was if it were a FQN
                let stateAfterFullIdentifier = processIdentifier queryTrie path state

                // Process the name in combination with the existing open namespaces
                (stateAfterFullIdentifier, state.OpenNamespaces)
                ||> Seq.fold (fun acc openNS -> processIdentifier queryTrie [ yield! openNS; yield! path ] acc))

    | FileContentEntry.NestedModule (nestedContent = nestedContent) ->
        // We don't want our current state to be affect by any open statements in the nested module
        let nestedState = List.fold (processStateEntry queryTrie) state nestedContent
        // Afterward we are only interested in the found dependencies in the nested module
        let foundDependencies =
            Set.union state.FoundDependencies nestedState.FoundDependencies

        { state with
            FoundDependencies = foundDependencies
        }

let getFileNameBefore (files: FileWithAST array) idx =
    files.[0 .. (idx - 1)] |> Array.map (fun f -> f.Idx) |> Set.ofArray

let time msg f a =
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let result = f a
    sw.Stop()
    printfn $"{msg} took %A{sw.Elapsed}"
    result

let mkGraph (files: FileWithAST array) =
    // Implementation files backed by signatures should be excluded to construct the trie.
    let trieInput =
        files
        |> Array.filter (fun f ->
            match f.AST with
            | ParsedInput.SigFile _ -> true
            | ParsedInput.ImplFile _ -> Array.forall (fun (sigFile: FileWithAST) -> sigFile.File <> $"{f.File}i") files)

    let trie = time "TrieMapping.mkTrie" TrieMapping.mkTrie trieInput

    let queryTrie: QueryTrie = queryTrieMemoized trie

    let fileContents =
        time "FileContentMapping.mkFileContent" Array.Parallel.map FileContentMapping.mkFileContent files

    let filesWithAutoOpen =
        trieInput
        |> Array.filter (fun f -> AutoOpenDetection.hasAutoOpenAttributeInFile f.AST)
        |> Array.map (fun f -> f.Idx)

    time
        "mkGraph"
        Array.Parallel.map
        (fun (file: FileWithAST) ->
            let fileContent = fileContents.[file.Idx]
            let knownFiles = getFileNameBefore files file.Idx

            // Process all entries of a file and query the trie when required to find the dependent files.
            let result =
                Seq.fold (processStateEntry queryTrie) (FileContentQueryState.Create file.Idx knownFiles) fileContent

            let allDependencies =
                if filesWithAutoOpen.Length > 0 then
                    // Automatically add all files that came before the current file that use the [<AutoOpen>] attribute.
                    let autoOpenDependencies =
                        set ([| 0 .. (file.Idx - 1) |].Intersect(filesWithAutoOpen))

                    Set.union result.FoundDependencies autoOpenDependencies
                else
                    result.FoundDependencies

            file, Set.toArray allDependencies)
        files

// =============================================================================================================
// =============================================================================================================

open NUnit.Framework
open FSharp.Compiler.Service.Tests.Common

let mkGraphAndReport files =
    let filesWithAST =
        files
        |> Array.Parallel.mapi (fun idx file ->
            {
                Idx = idx
                AST = parseSourceCode (file, System.IO.File.ReadAllText(file))
                File = file
            })

    let graph = mkGraph filesWithAST

    for fileName, deps in graph do
        let depString =
            deps
            |> Array.map (fun depIdx -> filesWithAST.[depIdx].File)
            |> String.concat "\n    "

        if deps.Length = 0 then
            printfn $"%s{fileName.File}: []"
        else
            printfn $"%s{fileName.File}:\n    {depString}"

[<Test>]
let ``Fantomas.Core for realzies`` () =
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
    |> mkGraphAndReport

let fcsFiles =
    [|
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\FSComp.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\FSIstrings.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\UtilsStrings.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\buildproperties.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\FSharp.Compiler.Service.InternalsVisibleTo.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\.NETStandard,Version=v2.0.AssemblyAttributes.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\FSharp.Compiler.Service.AssemblyInfo.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\sformat.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\sformat.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\sr.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\sr.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ResizeArray.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ResizeArray.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\HashMultiMap.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\HashMultiMap.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\EditDistance.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\EditDistance.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\TaggedCollections.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\TaggedCollections.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\illib.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\illib.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\FileSystem.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\FileSystem.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ildiag.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ildiag.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\zmap.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\zmap.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\zset.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\zset.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\XmlAdapters.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\XmlAdapters.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\InternalCollections.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\InternalCollections.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\QueueList.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\QueueList.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\lib.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\lib.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ImmutableArray.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\ImmutableArray.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\rational.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\rational.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\PathMap.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\PathMap.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\RidHelpers.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\range.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Utilities\range.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\Logger.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\Logger.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\LanguageFeatures.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\LanguageFeatures.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticOptions.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticOptions.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\TextLayoutRender.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\TextLayoutRender.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticsLogger.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticsLogger.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticResolutionHints.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\DiagnosticResolutionHints.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\prim-lexing.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\prim-lexing.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\prim-parsing.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\prim-parsing.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\ReferenceResolver.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\ReferenceResolver.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\SimulatedMSBuildReferenceResolver.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\SimulatedMSBuildReferenceResolver.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\CompilerLocation.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\CompilerLocation.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\BuildGraph.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Facilities\BuildGraph.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\il.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\il.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilx.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilx.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilascii.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilascii.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\ilpars.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\illex.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilprint.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilprint.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilmorph.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilmorph.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilsign.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilsign.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilnativeres.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilnativeres.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilsupp.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilsupp.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilbinary.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilbinary.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilread.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilread.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilwritepdb.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilwritepdb.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilwrite.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilwrite.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilreflect.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\AbstractIL\ilreflect.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\PrettyNaming.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\PrettyNaming.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\UnicodeLexing.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\UnicodeLexing.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\XmlDoc.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\XmlDoc.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTrivia.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTrivia.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTree.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTree.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTreeOps.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\SyntaxTreeOps.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\ParseHelpers.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\ParseHelpers.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\pppars.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\pars.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\LexHelpers.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\LexHelpers.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\pplex.fs"
        @"C:\projekty\fsharp\fsharp_main\artifacts\obj\FSharp.Compiler.Service\Debug\netstandard2.0\\lex.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\LexFilter.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\SyntaxTree\LexFilter.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\tainted.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\tainted.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypeProviders.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypeProviders.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\QuotationPickler.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\QuotationPickler.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\CompilerGlobalState.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\CompilerGlobalState.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTree.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTree.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreeBasics.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreeBasics.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TcGlobals.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreeOps.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreeOps.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreePickle.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\TypedTree\TypedTreePickle.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\import.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\import.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\TypeHierarchy.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\TypeHierarchy.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\infos.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\infos.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AccessibilityLogic.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AccessibilityLogic.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AttributeChecking.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AttributeChecking.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\TypeRelations.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\TypeRelations.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\InfoReader.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\InfoReader.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\NicePrint.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\NicePrint.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AugmentWithHashCompare.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\AugmentWithHashCompare.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\NameResolution.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\NameResolution.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\SignatureConformance.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\SignatureConformance.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\MethodOverrides.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\MethodOverrides.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\MethodCalls.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\MethodCalls.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\PatternMatchCompilation.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\PatternMatchCompilation.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\ConstraintSolver.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\ConstraintSolver.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckFormatStrings.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckFormatStrings.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\FindUnsolved.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\FindUnsolved.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\QuotationTranslator.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\QuotationTranslator.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\PostInferenceChecks.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\PostInferenceChecks.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckBasics.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckBasics.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckExpressions.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckExpressions.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckPatterns.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckPatterns.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckComputationExpressions.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckComputationExpressions.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckIncrementalClasses.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckIncrementalClasses.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckDeclarations.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Checking\CheckDeclarations.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\Optimizer.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\Optimizer.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\DetupleArgs.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\DetupleArgs.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\InnerLambdasToTopLevelFuncs.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\InnerLambdasToTopLevelFuncs.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerCalls.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerCalls.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerSequences.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerSequences.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerComputedCollections.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerComputedCollections.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerStateMachines.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerStateMachines.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerLocalMutables.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Optimize\LowerLocalMutables.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\EraseClosures.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\EraseClosures.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\EraseUnions.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\EraseUnions.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\IlxGen.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\CodeGen\IlxGen.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\FxResolver.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\FxResolver.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/AssemblyResolveHandler.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/AssemblyResolveHandler.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/NativeDllResolveHandler.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/NativeDllResolveHandler.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/DependencyProvider.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\DependencyManager/DependencyProvider.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerConfig.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerConfig.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerImports.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerImports.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerDiagnostics.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerDiagnostics.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\ParseAndCheckInputs.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\ParseAndCheckInputs.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\ScriptClosure.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\ScriptClosure.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerOptions.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CompilerOptions.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\OptimizeInputs.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\OptimizeInputs.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\XmlDocFileWriter.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\XmlDocFileWriter.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\BinaryResourceFormats.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\BinaryResourceFormats.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\StaticLinking.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\StaticLinking.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CreateILModule.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\CreateILModule.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\fsc.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Driver\fsc.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\FSharpDiagnostic.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\FSharpDiagnostic.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\SymbolHelpers.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\SymbolHelpers.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\Symbols.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\Symbols.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\Exprs.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\Exprs.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\SymbolPatterns.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Symbols\SymbolPatterns.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\SemanticClassification.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\SemanticClassification.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ItemKey.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ItemKey.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\SemanticClassificationKey.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\SemanticClassificationKey.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpSource.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpSource.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\IncrementalBuild.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\IncrementalBuild.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceCompilerDiagnostics.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceCompilerDiagnostics.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceConstants.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceDeclarationLists.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceDeclarationLists.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceLexing.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceLexing.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParseTreeWalk.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParseTreeWalk.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceNavigation.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceNavigation.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParamInfoLocations.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParamInfoLocations.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpParseFileResults.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpParseFileResults.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParsedInputOps.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceParsedInputOps.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceAssemblyContent.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceAssemblyContent.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceXmlDocParser.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceXmlDocParser.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ExternalSymbol.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ExternalSymbol.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\QuickParse.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\QuickParse.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpCheckerResults.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\FSharpCheckerResults.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\service.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\service.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceInterfaceStubGenerator.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceInterfaceStubGenerator.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceStructure.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceStructure.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceAnalysis.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Service\ServiceAnalysis.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Interactive\ControlledExecution.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Interactive\fsi.fsi"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Interactive\fsi.fs"
        // @"C:\projekty\fsharp\fsharp_main\src\Compiler\Legacy\LegacyMSBuildReferenceResolver.fsi"
        // @"C:\projekty\fsharp\fsharp_main\src\Compiler\Legacy\LegacyMSBuildReferenceResolver.fs"
        @"C:\projekty\fsharp\fsharp_main\src\Compiler\Legacy\LegacyHostedCompilerForTesting.fs"
    |]

[<Test>]
let ``FCS for realzies`` () = mkGraphAndReport fcsFiles

[<Test>]
let ``FCS for debugging`` () =
    let filesWithAST =
        fcsFiles
        |> Array.mapi (fun idx file ->
            {
                Idx = idx
                AST = parseSourceCode (file, System.IO.File.ReadAllText(file))
                File = file
            })

    let contents =
        Array.map (fun (file: FileWithAST) -> FileContentMapping.mkFileContent file) filesWithAST

    ignore contents
