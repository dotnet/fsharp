/// Logic for constructing a file dependency graph for the purposes of parallel type-checking.
module internal FSharp.Compiler.GraphChecking.DependencyResolution

/// <summary>
/// Query a TrieNode to find a certain path.
/// </summary>
/// <remarks>This code is only used directly in unit tests.</remarks>
val queryTrie: trie: TrieNode -> path: LongIdentifier -> QueryTrieNodeResult

/// <summary>Process an open path (found in the ParsedInput) with a given FileContentQueryState.</summary>
/// <remarks>This code is only used directly in unit tests.</remarks>
val processOpenPath: trie: TrieNode -> path: LongIdentifier -> state: FileContentQueryState -> FileContentQueryState

/// <summary>
/// Construct an approximate* dependency graph for files within a project, based on their ASTs.
/// </summary>
/// <param name="filePairs">Maps the index of a signature file with the index of its implementation counterpart and vice versa.</param>
/// <param name="files">The files inside a project.</param>
/// <returns>A tuple consisting of a dictionary of FileIndex (alias for int) and a Trie</returns>
/// <remarks>
/// <para>
/// *The constructed graph is a supergraph of the "necessary" file dependency graph,
/// ie. if file A is necessary to type-check file B, the resulting graph will contain edge B -> A.
/// The opposite is not true, ie. if file A is not necessary to type-check file B, the resulting graph *might* contain edge B -> A.
/// This is because the graph resolution algorithm has limited capability as it is based on ASTs alone.
/// </para>
/// <para>
/// The file order is used by the resolution algorithm to remove edges not allowed by the language.
/// Ie. if file B precedes file A, the resulting graph will not contain edge B -> A.
/// Hence this function cannot, as it stands, be used to help create a "reasonable" file ordering for an unordered set of files.
/// </para>
/// </remarks>
val mkGraph: filePairs: FilePairMap -> files: FileInProject array -> Graph<FileIndex> * TrieNode
