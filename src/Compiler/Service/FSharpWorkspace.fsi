// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis.Workspace

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
type FSharpWorkspace =

    new: unit -> FSharpWorkspace

    new: checker: FSharp.Compiler.CodeAnalysis.FSharpChecker -> FSharpWorkspace

    member internal Debug_DumpMermaid: path: string -> unit

    /// The `FSharpChecker` instance used by this workspace.
    member Checker: FSharp.Compiler.CodeAnalysis.FSharpChecker with get

    /// File management for this workspace
    member Files: FSharpWorkspaceState.FSharpWorkspaceFiles with get

    /// Project management for this workspace
    member Projects: FSharpWorkspaceState.FSharpWorkspaceProjects with get

    /// Use this to query the workspace for information
    member Query: FSharpWorkspaceQuery.FSharpWorkspaceQuery with get
