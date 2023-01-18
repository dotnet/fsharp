namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic
open FSharp.Compiler.Syntax

/// The index of a file inside a project.
type internal FileIndex = int

/// File name capture by ParsedInput.FileName
type internal FileName = string

/// Represents the string value of a single identifier in the syntax tree.
/// For example, `"Hello"` in `module Hello`.
type internal Identifier = string

/// Represents one or more identifiers in the syntax tree.
/// For example, `[ "X"; "Y"; "Z" ]` in `open X.Y.Z`
type internal LongIdentifier = string list

/// Combines the file name, index and parsed syntax tree of a file in a project.
type internal FileInProject =
    { Idx: FileIndex
      FileName: FileName
      ParsedInput: ParsedInput }

/// There is a subtle difference between a module and namespace.
/// A namespace does not necessarily expose a set of dependent files.
/// Only when the namespace exposes types that could later be inferred.
/// Children of a namespace don't automatically depend on each other for that reason
type internal TrieNodeInfo =
    | Root of files: HashSet<FileIndex>
    | Module of name: Identifier * file: FileIndex
    | Namespace of name: Identifier * filesThatExposeTypes: HashSet<FileIndex>

    member Files: Set<FileIndex>

type internal TrieNode =
    { Current: TrieNodeInfo
      Children: Dictionary<Identifier, TrieNode> }

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
    /// We can scope an `OpenStatement` to the everything that is happening inside the nested module.
    | NestedModule of name: string * nestedContent: FileContentEntry list

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

/// The result type of querying a Trie Node.
/// The requested node either did not exist, is part of the trie but but no expose data or was found and exposes data.
[<RequireQualifiedAccess>]
type internal QueryTrieNodeResult =
    /// No node was found for the path in the trie
    | NodeDoesNotExist
    /// A node was found but it yielded no file links
    | NodeDoesNotExposeData
    /// A node was found with one or more file links
    | NodeExposesData of Set<FileIndex>

type internal QueryTrie = LongIdentifier -> QueryTrieNodeResult

/// Helper class to help map signature files to implementation files and vice versa.
type internal FilePairMap =
    new: files: FileInProject array -> FilePairMap
    member GetSignatureIndex: implementationIndex: FileIndex -> FileIndex
    member GetImplementationIndex: signatureIndex: FileIndex -> FileIndex
    member HasSignature: implementationIndex: FileIndex -> bool
    member TryGetSignatureIndex: implementationIndex: FileIndex -> FileIndex option
    member IsSignature: index: FileIndex -> bool

/// Callback that returns a previously calculated 'Result and updates 'State accordingly.
type internal Finisher<'State, 'Result> = delegate of 'State -> 'Result * 'State
