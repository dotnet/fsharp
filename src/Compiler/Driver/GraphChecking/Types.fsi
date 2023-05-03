namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic
open FSharp.Compiler.Syntax

/// The index of a file inside a project.
type internal FileIndex = int

/// File name captured by ParsedInput.FileName.
type internal FileName = string

/// Represents the string value of a single identifier in the syntax tree.
/// For example, `"Hello"` in `module Hello`.
type internal Identifier = string

/// Represents one or more identifiers in the syntax tree.
/// For example, `[ "X"; "Y"; "Z" ]` in `open X.Y.Z`
type internal LongIdentifier = Identifier list

/// Combines the file name, index and parsed syntax tree of a file in a project.
type internal FileInProject =
    { Idx: FileIndex
      FileName: FileName
      ParsedInput: ParsedInput }

    static member FromFileInProject: ParsedInput list -> FileInProject array

/// There is a subtle difference between a module and namespace.
/// A namespace does not necessarily expose a set of dependent files.
/// Only when the namespace exposes types that could later be inferred.
/// Children of a namespace don't automatically depend on each other for that reason
type internal TrieNodeInfo =
    | Root of files: HashSet<FileIndex>
    | Module of name: Identifier * file: FileIndex
    | Namespace of
        name: Identifier *
        /// Files that expose types that are part of this namespace.
        filesThatExposeTypes: HashSet<FileIndex> *
        /// Files that use this namespace but don't contain any types.
        filesDefiningNamespaceWithoutTypes: HashSet<FileIndex>

    member Files: Set<FileIndex>

/// A node in the Trie structure.
type internal TrieNode =
    {
        /// Information about this node.
        Current: TrieNodeInfo
        /// Child nodes
        Children: Dictionary<Identifier, TrieNode>
    }

    /// Zero or more files that define the LongIdentifier represented by this node.
    member Files: Set<FileIndex>

/// A significant construct found in the syntax tree of a file.
/// This construct needs to be processed in order to deduce potential links to other files in the project.
type internal FileContentEntry =
    /// Any toplevel namespace a file might have.
    /// In case a file has `module X.Y.Z`, then `X.Y` is considered to be the toplevel namespace
    | TopLevelNamespace of path: LongIdentifier * content: FileContentEntry list
    /// The `open X.Y.Z` syntax.
    | OpenStatement of path: LongIdentifier
    /// Any identifier that has more than one piece (LongIdent or SynLongIdent) in it.
    /// The last part of the identifier should not be included.
    | PrefixedIdentifier of path: LongIdentifier
    /// Being explicit about nested modules allows for easier reasoning what namespaces (paths) are open.
    /// For example we can limit the scope of an `OpenStatement` to symbols defined inside the nested module.
    | NestedModule of name: string * nestedContent: FileContentEntry list

/// File identifiers and its content extract for dependency resolution
type internal FileContent =
    { FileName: FileName
      Idx: FileIndex
      Content: FileContentEntry array }

type internal FileContentQueryState =
    { OwnNamespace: LongIdentifier option
      OpenedNamespaces: Set<LongIdentifier>
      FoundDependencies: Set<FileIndex>
      CurrentFile: FileIndex
      KnownFiles: Set<FileIndex> }

    static member Create:
        fileIndex: FileIndex -> knownFiles: Set<FileIndex> -> filesAtRoot: Set<FileIndex> -> FileContentQueryState
    member AddOwnNamespace: ns: LongIdentifier * ?files: Set<FileIndex> -> FileContentQueryState
    member AddDependencies: files: Set<FileIndex> -> FileContentQueryState
    member AddOpenNamespace: path: LongIdentifier * ?files: Set<FileIndex> -> FileContentQueryState
    member OpenNamespaces: Set<LongIdentifier>

/// Result of querying a Trie Node.
[<RequireQualifiedAccess>]
type internal QueryTrieNodeResult =
    /// No node was found for the path in the trie.
    | NodeDoesNotExist
    /// <summary>A node was found but no file exposes data for the LongIdentifier in question.</summary>
    /// <example>
    /// This could happen if there is a single file with a top-level module `module A.B`,
    /// and we search for `A`.
    /// Although the `A` path exists in the Trie, it does not contain any relevant definitions (beyond itself).
    /// </example>
    | NodeDoesNotExposeData
    /// A node was found with one or more files that contain relevant definitions required for type-checking.
    | NodeExposesData of Set<FileIndex>

/// A function for querying a Trie (the Trie is defined within the function's context)
type internal QueryTrie = LongIdentifier -> QueryTrieNodeResult

/// Helper class for mapping signature files to implementation files and vice versa.
type internal FilePairMap =
    new: files: FileInProject array -> FilePairMap
    member GetSignatureIndex: implementationIndex: FileIndex -> FileIndex
    member GetImplementationIndex: signatureIndex: FileIndex -> FileIndex
    member HasSignature: implementationIndex: FileIndex -> bool
    member TryGetSignatureIndex: implementationIndex: FileIndex -> FileIndex option
    member IsSignature: index: FileIndex -> bool

/// Callback that returns a previously calculated 'Result and updates 'State accordingly.
type internal Finisher<'State, 'Result> = delegate of 'State -> 'Result * 'State
