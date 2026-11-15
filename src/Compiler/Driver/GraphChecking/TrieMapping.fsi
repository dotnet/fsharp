module internal FSharp.Compiler.GraphChecking.TrieMapping

/// Process all the files (in parallel) in a project to construct a Root TrieNode.
/// When the project has signature files, the implementation counterparts will not be processed.
val mkTrie: files: FileInProject array -> (FileIndex * TrieNode) array

val serializeToMermaid: path: string -> filesInProject: FileInProject array -> trie: TrieNode -> unit

/// The names a declaration of an implementation file defines: values, types, union cases, record fields, members,
/// custom operation names, modules, namespaces, and every such name inside a nested module or namespace fragment.
/// The flag says whether a custom operation's name is not a string literal, so a name may be missing.
val declarationNames: decl: FSharp.Compiler.Syntax.SynModuleDecl -> string list * bool
