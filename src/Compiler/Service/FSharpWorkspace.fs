// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis.Workspace

open System.IO
open System.Threading

open FSharp.Compiler.CodeAnalysis

open Internal.Utilities.DependencyGraph
open FSharpWorkspaceState
open FSharpWorkspaceQuery

/// This type holds the current state of an F# workspace. It's mutable but thread-safe.
/// It accepts updates to the state and can be queried for information about the workspace.
///
/// The state can be built up incrementally by adding projects with one of the `Projects.AddOrUpdate` overloads.
/// Updates to any project properties are done the same way. Each project is identified by its project file
/// path and output path or by `FSharpProjectIdentifier`. When the same project is added again, it will be
/// updated with the new information.
///
/// Project references are discovered automatically as projects are added or updated.
///
/// Updates to file contents are signaled through the `Files.Open`, `Files.Edit`, and `Files.Close` methods.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspace(checker: FSharpChecker) =

    let depGraph = LockOperatedDependencyGraph() :> IThreadSafeDependencyGraph<_, _>

    let files = FSharpWorkspaceFiles depGraph

    let projects = FSharpWorkspaceProjects(depGraph, files)

    let query = FSharpWorkspaceQuery(depGraph, checker)

    new() =
        FSharpWorkspace(
            FSharpChecker.Create(
                keepAllBackgroundResolutions = true,
                keepAllBackgroundSymbolUses = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = true,
                enablePartialTypeChecking = true,
                parallelReferenceResolution = true,
                captureIdentifiersWhenParsing = true,
                useTransparentCompiler = true
            )
        )

    member internal this.Debug_DumpMermaid(path) =
        let content =
            depGraph.Debug_RenderMermaid (function
                // Collapse all reference on disk nodes into one. Otherwise the graph is too big to render.
                | WorkspaceGraphTypes.WorkspaceNodeKey.ReferenceOnDisk _ -> WorkspaceGraphTypes.WorkspaceNodeKey.ReferenceOnDisk "..."
                | x -> x)

        File.WriteAllText(__SOURCE_DIRECTORY__ + path, content)

    /// The `FSharpChecker` instance used by this workspace.
    member _.Checker = checker

    /// File management for this workspace
    member _.Files = files

    /// Project management for this workspace
    member _.Projects = projects

    /// Use this to query the workspace for information
    member _.Query = query
