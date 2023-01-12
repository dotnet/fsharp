namespace FSharp.Compiler.GraphChecking

open System.Collections.Generic
open FSharp.Compiler.Syntax

type internal File = string
type internal ModuleSegment = string

type internal FileWithAST =
    {
        Idx: int
        File: File
        AST: ParsedInput
    }

/// There is a subtle difference between a module and namespace.
/// A namespace does not necessarily expose a set of dependent files.
/// Only when the namespace exposes types that could later be inferred.
/// Children of a namespace don't automatically depend on each other for that reason
type internal TrieNodeInfo =
    | Root of files: HashSet<int>
    | Module of segment: string * file: int
    | Namespace of segment: string * filesThatExposeTypes: HashSet<int>

    member x.Files: Set<int> =
        match x with
        | Root files -> set files
        | Module (file = file) -> Set.singleton file
        | Namespace (filesThatExposeTypes = files) -> set files

type internal TrieNode =
    {
        Current: TrieNodeInfo
        Children: Dictionary<ModuleSegment, TrieNode>
    }

    member x.Files = x.Current.Files

type internal FileContentEntry =
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

type internal FileContent =
    {
        Name: File
        Idx: int
        Content: FileContentEntry array
    }

type internal FileContentQueryState =
    {
        OwnNamespace: ModuleSegment list option
        OpenedNamespaces: Set<ModuleSegment list>
        FoundDependencies: Set<int>
        CurrentFile: int
        KnownFiles: Set<int>
    }

    static member Create (fileIndex: int) (knownFiles: Set<int>) (filesAtRoot: Set<int>) =
        {
            OwnNamespace = None
            OpenedNamespaces = Set.empty
            FoundDependencies = filesAtRoot
            CurrentFile = fileIndex
            KnownFiles = knownFiles
        }

    member x.AddOwnNamespace(ns: ModuleSegment list, ?files: Set<int>) =
        match files with
        | None -> { x with OwnNamespace = Some ns }
        | Some files ->
            let foundDependencies =
                Set.filter x.KnownFiles.Contains files |> Set.union x.FoundDependencies

            { x with
                OwnNamespace = Some ns
                FoundDependencies = foundDependencies
            }

    member x.AddDependencies(files: Set<int>) : FileContentQueryState =
        let files = Set.filter x.KnownFiles.Contains files |> Set.union x.FoundDependencies
        { x with FoundDependencies = files }

    member x.AddOpenNamespace(path: ModuleSegment list, ?files: Set<int>) =
        match files with
        | None ->
            { x with
                OpenedNamespaces = Set.add path x.OpenedNamespaces
            }
        | Some files ->
            let foundDependencies =
                Set.filter x.KnownFiles.Contains files |> Set.union x.FoundDependencies

            { x with
                FoundDependencies = foundDependencies
                OpenedNamespaces = Set.add path x.OpenedNamespaces
            }

    member x.OpenNamespaces =
        match x.OwnNamespace with
        | None -> x.OpenedNamespaces
        | Some ownNs -> Set.add ownNs x.OpenedNamespaces

[<RequireQualifiedAccess>]
type internal QueryTrieNodeResult =
    /// No node was found for the path in the trie
    | NodeDoesNotExist
    /// A node was found but it yielded no file links
    | NodeDoesNotExposeData
    /// A node was found with one or more file links
    | NodeExposesData of Set<int>

type internal QueryTrie = ModuleSegment list -> QueryTrieNodeResult

/// Helper class to help map signature files to implementation files and vice versa.
type internal FilePairMap(files: FileWithAST array) =
    let implToSig, sigToImpl =
        Array.choose
            (fun f ->
                match f.AST with
                | ParsedInput.SigFile _ ->
                    files
                    |> Array.skip (f.Idx + 1)
                    |> Array.tryFind (fun (implFile: FileWithAST) -> $"{implFile.File}i" = f.File)
                    |> Option.map (fun (implFile: FileWithAST) -> (implFile.Idx, f.Idx))
                | ParsedInput.ImplFile _ -> None)
            files
        |> fun pairs -> Map.ofArray pairs, Map.ofArray (Array.map (fun (a, b) -> (b, a)) pairs)

    member x.GetSignatureIndex(implementationIndex: int) = Map.find implementationIndex implToSig
    member x.GetImplementationIndex(signatureIndex: int) = Map.find signatureIndex sigToImpl

    member x.HasSignature(implementationIndex: int) =
        Map.containsKey implementationIndex implToSig

    member x.TryGetSignatureIndex(implementationIndex: int) =
        if x.HasSignature implementationIndex then
            Some(x.GetSignatureIndex implementationIndex)
        else
            None

    member x.IsSignature(index: int) = Map.containsKey index sigToImpl

/// Callback that returns a previously calculated 'Result and updates 'State accordingly.
type Finisher<'State, 'Result> = delegate of 'State -> 'Result * 'State
