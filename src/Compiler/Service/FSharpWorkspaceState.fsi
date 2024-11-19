// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState

open System
open System.Runtime.CompilerServices

open Internal.Utilities.DependencyGraph

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

/// Types for the workspace graph. These should not be accessed directly, rather through the
/// extension methods in `WorkspaceDependencyGraphExtensions`.
module internal WorkspaceGraphTypes =

    /// All project information except source files
    type ProjectWithoutFiles = ProjectConfig * FSharpReferencedProjectSnapshot list

    [<RequireQualifiedAccess>]
    type WorkspaceNodeKey =
        | SourceFile of filePath: string
        | ReferenceOnDisk of filePath: string

        /// All project information except source files and (in-memory) project references
        | ProjectConfig of FSharpProjectIdentifier

        /// All project information except source files
        | ProjectWithoutFiles of FSharpProjectIdentifier

        /// Complete project information
        | ProjectSnapshot of FSharpProjectIdentifier

        override ToString: unit -> string

    [<RequireQualifiedAccess>]
    type WorkspaceNodeValue =
        | SourceFile of FSharpFileSnapshot
        | ReferenceOnDisk of ReferenceOnDisk

        /// All project information except source files and (in-memory) project references
        | ProjectConfig of ProjectConfig

        /// All project information except source files
        | ProjectWithoutFiles of ProjectWithoutFiles

        /// Complete project information
        | ProjectSnapshot of FSharpProjectSnapshot

    module WorkspaceNode =

        val projectConfig: value: WorkspaceNodeValue -> ProjectConfig option

        val projectSnapshot: value: WorkspaceNodeValue -> FSharpProjectSnapshot option

        val projectWithoutFiles:
            value: WorkspaceNodeValue -> (ProjectConfig * FSharpReferencedProjectSnapshot list) option

        val sourceFile: value: WorkspaceNodeValue -> FSharpFileSnapshot option

        val referenceOnDisk: value: WorkspaceNodeValue -> ReferenceOnDisk option

        val projectConfigKey: value: WorkspaceNodeKey -> FSharpProjectIdentifier option

        val projectSnapshotKey: value: WorkspaceNodeKey -> FSharpProjectIdentifier option

        val projectWithoutFilesKey: value: WorkspaceNodeKey -> FSharpProjectIdentifier option

        val sourceFileKey: value: WorkspaceNodeKey -> string option

        val referenceOnDiskKey: value: WorkspaceNodeKey -> string option

open WorkspaceGraphTypes

[<AutoOpen>]
module internal WorkspaceDependencyGraphExtensions =

    /// This type adds extension methods to the dependency graph to constraint the types and type relations
    /// that can be added to the graph.
    ///
    /// All unsafe operations that can throw at runtime, i.e. unpacking, are done here.
    [<Extension; Class>]
    type WorkspaceDependencyGraphTypeExtensions =

        [<Extension>]
        static member AddFiles:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * files: (string * FSharpFileSnapshot) seq ->
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, FSharpFileSnapshot seq, unit>

        [<Extension>]
        static member AddOrUpdateFile:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * file: string * snapshot: FSharpFileSnapshot ->
                unit

        [<Extension>]
        static member AddProjectConfig:
            this: GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, ReferenceOnDisk seq, unit> *
            projectIdentifier: FSharpProjectIdentifier *
            computeProjectConfig: (ReferenceOnDisk seq -> ProjectConfig) ->
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, (ProjectConfig * FSharpProjectSnapshot seq), FSharpProjectIdentifier>

        [<Extension>]
        static member AddProjectReference:
            this: IDependencyGraph<WorkspaceNodeKey, 'd> *
            project: FSharpProjectIdentifier *
            dependsOn: FSharpProjectIdentifier ->
                unit

        [<Extension>]
        static member AddProjectSnapshot:
            this:
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, (ProjectWithoutFiles * FSharpFileSnapshot seq), FSharpProjectIdentifier> *
            computeProjectSnapshot: (ProjectWithoutFiles * FSharpFileSnapshot seq -> FSharpProjectSnapshot) ->
                unit

        [<Extension>]
        static member AddProjectWithoutFiles:
            this:
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, (ProjectConfig * FSharpProjectSnapshot seq), FSharpProjectIdentifier> *
            computeProjectWithoutFiles: (ProjectConfig * FSharpProjectSnapshot seq -> ProjectWithoutFiles) ->
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, (ProjectConfig * FSharpReferencedProjectSnapshot list), FSharpProjectIdentifier>

        [<Extension>]
        static member AddReferencesOnDisk:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * references: ReferenceOnDisk seq ->
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, ReferenceOnDisk seq, unit>

        [<Extension>]
        static member AddSourceFiles:
            this: GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, ProjectWithoutFiles, FSharpProjectIdentifier> *
            sourceFiles: (string * #FSharpFileSnapshot) seq ->
                GraphBuilder<WorkspaceNodeKey, WorkspaceNodeValue, ((ProjectConfig *
                FSharpReferencedProjectSnapshot list) *
                FSharpFileSnapshot seq), FSharpProjectIdentifier>

        [<Extension>]
        static member GetProjectReferencesOf:
            this: IDependencyGraph<WorkspaceNodeKey, 'b> * project: FSharpProjectIdentifier ->
                FSharpProjectIdentifier seq

        [<Extension>]
        static member GetProjectSnapshot:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * project: FSharpProjectIdentifier ->
                FSharpProjectSnapshot

        [<Extension>]
        static member GetProjectsContaining:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * file: string -> FSharpProjectSnapshot seq

        [<Extension>]
        static member GetProjectsThatReference:
            this: IDependencyGraph<WorkspaceNodeKey, 'a> * dllPath: string -> FSharpProjectIdentifier seq

        [<Extension>]
        static member GetSourceFile:
            this: IDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * file: string -> FSharpFileSnapshot

        [<Extension>]
        static member RemoveProjectReference:
            this: IDependencyGraph<WorkspaceNodeKey, 'c> *
            project: FSharpProjectIdentifier *
            noLongerDependsOn: FSharpProjectIdentifier ->
                unit

/// Interface for managing files in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceFiles =

    internal new: depGraph: IThreadSafeDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> -> FSharpWorkspaceFiles

    /// Indicates that a file has been closed. Any changes that were not saved to disk are undone and any further reading
    /// of the file's contents will be from the filesystem.
    member Close: file: Uri -> unit

    /// Indicates that a file has been changed and now has the given content. If it wasn't previously open it is considered open now.
    member Edit: file: Uri * content: string -> unit

    member internal GetFileContentIfOpen: path: string -> string option

    /// Indicates that a file has been opened and has the given content. Any updates to the file should be done through `Files.Edit`.
    member Open: (Uri * string -> unit) with get

/// Interface for managing with projects in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceProjects =

    internal new:
        depGraph: IThreadSafeDependencyGraph<WorkspaceNodeKey, WorkspaceNodeValue> * files: FSharpWorkspaceFiles ->
            FSharpWorkspaceProjects

    /// Adds or updates an F# project in the workspace. Project is identified by the project file and output path or FSharpProjectIdentifier.
    member AddOrUpdate: projectConfig: ProjectConfig * sourceFilePaths: string seq -> FSharpProjectIdentifier

    member AddOrUpdate: projectPath: string * outputPath: string * compilerArgs: string seq -> FSharpProjectIdentifier

    member AddOrUpdate:
        projectFileName: string *
        outputFileName: string *
        sourceFiles: string seq *
        referencesOnDisk: string seq *
        otherOptions: string list ->
            FSharpProjectIdentifier
