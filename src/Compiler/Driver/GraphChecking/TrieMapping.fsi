module internal FSharp.Compiler.GraphChecking.TrieMapping

/// Process all the files (in parallel) in a project to construct a Root TrieNode.
/// When the project has signature files, the implementation counterparts will not be processed.
val mkTrie: files: FileInProject array -> (FileIndex * TrieNode) array

val serializeToMermaid: path: string -> filesInProject: FileInProject array -> trie: TrieNode -> unit
