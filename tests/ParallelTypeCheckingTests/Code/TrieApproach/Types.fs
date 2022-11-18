namespace ParallelTypeCheckingTests.Code.TrieApproach

open System.Collections.Generic
open FSharp.Compiler.Syntax

type File = string
type ModuleSegment = string

type FileWithAST =
    {
        Idx: int
        File: File
        AST: ParsedInput
    }

/// There is a subtle difference a module and namespace.
/// A namespace does not necessarily expose a set of dependent files.
/// Only when the namespace exposes types that could later be inferred.
/// Children of a namespace don't automatically depend on each other for that reason
type TrieNodeInfo =
    | Root
    | Module of segment: string * file: int
    | Namespace of segment: string * filesThatExposeTypes: HashSet<int>

    member x.Segment =
        match x with
        | Root -> failwith "Root has no segment"
        | Module (segment = segment)
        | Namespace (segment = segment) -> segment

    member x.Files: Set<int> =
        match x with
        | Root -> failwith "Root has no files"
        | Module (file = file) -> Set.singleton file
        | Namespace (filesThatExposeTypes = files) -> set files

type TrieNode =
    {
        Current: TrieNodeInfo
        Children: Dictionary<ModuleSegment, TrieNode>
    }

    member x.Files = x.Current.Files

type FileContentEntry =
    /// Any toplevel namespace a file might have.
    /// In case a file has `module X.Y.Z`, then `X.Y` is considered to be the toplevel namespace
    | TopLevelNamespace of path: ModuleSegment list * content: FileContentEntry list
    /// The `open X.Y.Z` syntax.
    | OpenStatement of path: ModuleSegment list
    /// Any identifier that has more than one piece (LongIdent or SynLongIdent) in it.
    /// The last part of the identifier should not be included.
    | PrefixedIdentifier of path: ModuleSegment list
    /// Being explicit about nested modules allows for easier reasoning what namespaces (paths) are open.
    /// We can scope an `OpenStatement` to the everything that is happening inside the nested module.
    | NestedModule of name: string * nestedContent: FileContentEntry list

type FileContent =
    {
        Name: File
        Idx: int
        Content: FileContentEntry array
    }

type FileContentQueryState =
    {
        OpenNamespaces: Set<ModuleSegment list>
        FoundDependencies: Set<int>
        CurrentFile: int
        KnownFiles: Set<int>
    }

    static member Create (fileIndex: int) (knownFiles: Set<int>) =
        {
            OpenNamespaces = Set.empty
            FoundDependencies = Set.empty
            CurrentFile = fileIndex
            KnownFiles = knownFiles
        }

    member x.AddDependencies(files: Set<int>) : FileContentQueryState =
        let files = Set.filter x.KnownFiles.Contains files |> Set.union x.FoundDependencies
        { x with FoundDependencies = files }

    member x.AddOpenNamespace(path: ModuleSegment list) =
        { x with
            OpenNamespaces = Set.add path x.OpenNamespaces
        }

    member x.AddDependenciesAndOpenNamespace(files: Set<int>, path: ModuleSegment list) =
        let foundDependencies =
            Set.filter x.KnownFiles.Contains files |> Set.union x.FoundDependencies

        { x with
            FoundDependencies = foundDependencies
            OpenNamespaces = Set.add path x.OpenNamespaces
        }

[<RequireQualifiedAccess>]
type QueryTrieNodeResult =
    /// No node was found for the path in the trie
    | NodeDoesNotExist
    /// A node was found but it yielded no file links
    | NodeDoesNotExposeData
    /// A node was found with one or more file links
    | NodeExposesData of Set<int>

type QueryTrie = ModuleSegment list -> QueryTrieNodeResult
