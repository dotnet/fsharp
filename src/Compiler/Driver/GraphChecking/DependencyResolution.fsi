module internal FSharp.Compiler.GraphChecking.DependencyResolution

/// Query a TrieNode to find a certain path.
/// <remarks>This code is only used directly in unit tests.</remarks>
val queryTrie: trie: TrieNode -> path: LongIdentifier -> QueryTrieNodeResult

/// Process an open path (found in the ParsedInput) with a given FileContentQueryState.
/// <remarks>This code is only used directly in unit tests.</remarks>
val processOpenPath:
    queryTrie: QueryTrie -> path: LongIdentifier -> state: FileContentQueryState -> FileContentQueryState

/// <summary>
/// A function to determine the dependent relations between files inside a project.
/// The file order is currently heavily used inside the resolution algorithm.
/// When the determining the dependencies of a file,
/// only the indexes that came before it are considered as potential candidates.
/// <param name="compilingFSharpCore">When compiling FSharp.Core, all files always take a dependency on `prim-types-prelude.fsi`.</param>
/// <param name="filePairs">Maps the index of a signature file with the index of its implementation counterpart and vice versa.</param>
/// <param name="files">The files inside a project.</param>
/// <returns>A dictionary of FileIndex (alias for int)</returns>
/// </summary>
val mkGraph: compilingFSharpCore: bool -> filePairs: FilePairMap -> files: FileInProject array -> Graph<FileIndex>
