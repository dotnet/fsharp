// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState

/// Types for the workspace graph. These should not be accessed directly, rather through the
/// extension methods in `WorkspaceDependencyGraphExtensions`.
module internal WorkspaceGraphTypes =

    /// All project information except source files
    type ProjectWithoutFiles =
        FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
        FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpReferencedProjectSnapshot list

    [<RequireQualifiedAccess>]
    type WorkspaceNodeKey =
        | SourceFile of filePath: string
        | ReferenceOnDisk of filePath: string

        /// All project information except source files and (in-memory) project references
        | ProjectConfig of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

        /// All project information except source files
        | ProjectWithoutFiles of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

        /// Complete project information
        | ProjectSnapshot of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

        override ToString: unit -> string

    [<RequireQualifiedAccess>]
    type WorkspaceNodeValue =
        | SourceFile of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot
        | ReferenceOnDisk of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk

        /// All project information except source files and (in-memory) project references
        | ProjectConfig of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig

        /// All project information except source files
        | ProjectWithoutFiles of ProjectWithoutFiles

        /// Complete project information
        | ProjectSnapshot of
          FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot

    module WorkspaceNode =

        val projectConfig:
          value: WorkspaceNodeValue ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig option

        val projectSnapshot:
          value: WorkspaceNodeValue ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot option

        val projectWithoutFiles:
          value: WorkspaceNodeValue ->
            (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
             FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpReferencedProjectSnapshot list) option

        val sourceFile:
          value: WorkspaceNodeValue ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot option

        val referenceOnDisk:
          value: WorkspaceNodeValue ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk option

        val projectConfigKey:
          value: WorkspaceNodeKey ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier option

        val projectSnapshotKey:
          value: WorkspaceNodeKey ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier option

        val projectWithoutFilesKey:
          value: WorkspaceNodeKey ->
            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier option

        val sourceFileKey: value: WorkspaceNodeKey -> string option

        val referenceOnDiskKey: value: WorkspaceNodeKey -> string option

[<AutoOpen>]
module internal WorkspaceDependencyGraphExtensions =

    /// This type adds extension methods to the dependency graph to constraint the types and type relations
    /// that can be added to the graph.
    ///
    /// All unsafe operations that can throw at runtime, i.e. unpacking, are done here.
    [<System.Runtime.CompilerServices.Extension; Class>]
    type WorkspaceDependencyGraphTypeExtensions =

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddFiles: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                              WorkspaceGraphTypes.WorkspaceNodeValue> *
                    files: (string *
                            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot) seq ->
                      Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                      WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                      FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot seq,
                                                                      unit>

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddOrUpdateFile: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                     WorkspaceGraphTypes.WorkspaceNodeValue> *
                           file: string *
                           snapshot: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot ->
                             unit

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddProjectConfig: this: Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                  WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                  FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk seq,
                                                                                  unit> *
                            projectIdentifier: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier *
                            computeProjectConfig: (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk seq ->
                                                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig) ->
                              Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                              WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                              (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                                                                               FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot seq),
                                                                              FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier>

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddProjectReference: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                         'd> *
                               project: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier *
                               dependsOn: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier ->
                                 unit

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddProjectSnapshot: this: Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                    WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                    (WorkspaceGraphTypes.ProjectWithoutFiles *
                                                                                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot seq),
                                                                                    FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier> *
                              computeProjectSnapshot: (WorkspaceGraphTypes.ProjectWithoutFiles *
                                                       FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot seq ->
                                                         FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot) ->
                                unit

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddProjectWithoutFiles: this: Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                        WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                        (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                                                                                         FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot seq),
                                                                                        FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier> *
                                  computeProjectWithoutFiles: (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                                                               FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot seq ->
                                                                 WorkspaceGraphTypes.ProjectWithoutFiles) ->
                                    Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                    WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                    (FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                                                                                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpReferencedProjectSnapshot list),
                                                                                    FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier>

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddReferencesOnDisk: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                         WorkspaceGraphTypes.WorkspaceNodeValue> *
                               references: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk seq ->
                                 Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                 WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                 FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ReferenceOnDisk seq,
                                                                                 unit>

        [<System.Runtime.CompilerServices.Extension>]
        static member
          AddSourceFiles: this: Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                                WorkspaceGraphTypes.ProjectWithoutFiles,
                                                                                FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier> *
                          sourceFiles: (string *
                                        #FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot) seq ->
                            Internal.Utilities.DependencyGraph.GraphBuilder<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                            WorkspaceGraphTypes.WorkspaceNodeValue,
                                                                            ((FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                                                                              FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpReferencedProjectSnapshot list) *
                                                                             FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot seq),
                                                                            FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier>

        [<System.Runtime.CompilerServices.Extension>]
        static member
          GetProjectReferencesOf: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                            'b> *
                                  project: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier ->
                                    FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier seq

        [<System.Runtime.CompilerServices.Extension>]
        static member
          GetProjectSnapshot: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                        WorkspaceGraphTypes.WorkspaceNodeValue> *
                              project: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier ->
                                FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot

        [<System.Runtime.CompilerServices.Extension>]
        static member
          GetProjectsContaining: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                           WorkspaceGraphTypes.WorkspaceNodeValue> *
                                 file: string ->
                                   FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectSnapshot seq

        [<System.Runtime.CompilerServices.Extension>]
        static member
          GetProjectsThatReference: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                              'a> *
                                    dllPath: string ->
                                      FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier seq

        [<System.Runtime.CompilerServices.Extension>]
        static member
          GetSourceFile: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                   WorkspaceGraphTypes.WorkspaceNodeValue> *
                         file: string ->
                           FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpFileSnapshot

        [<System.Runtime.CompilerServices.Extension>]
        static member
          RemoveProjectReference: this: Internal.Utilities.DependencyGraph.IDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                            'c> *
                                  project: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier *
                                  noLongerDependsOn: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier ->
                                    unit

/// Interface for managing files in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceFiles =

    internal new: depGraph: Internal.Utilities.DependencyGraph.IThreadSafeDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                          WorkspaceGraphTypes.WorkspaceNodeValue> ->
                    FSharpWorkspaceFiles

    /// Indicates that a file has been closed. Any changes that were not saved to disk are undone and any further reading
    /// of the file's contents will be from the filesystem.
    member Close: file: System.Uri -> unit

    /// Indicates that a file has been changed and now has the given content. If it wasn't previously open it is considered open now.
    member Edit: file: System.Uri * content: string -> unit

    member internal GetFileContentIfOpen: path: string -> string option

    /// Indicates that a file has been opened and has the given content. Any updates to the file should be done through `Files.Edit`.
    member Open: (System.Uri * string -> unit) with get

/// Interface for managing with projects in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceProjects =

    internal new: depGraph: Internal.Utilities.DependencyGraph.IThreadSafeDependencyGraph<WorkspaceGraphTypes.WorkspaceNodeKey,
                                                                                          WorkspaceGraphTypes.WorkspaceNodeValue> *
                  files: FSharpWorkspaceFiles -> FSharpWorkspaceProjects

    /// Adds or updates an F# project in the workspace. Project is identified by the project file and output path or FSharpProjectIdentifier.
    member
      AddOrUpdate: projectConfig: FSharp.Compiler.CodeAnalysis.ProjectSnapshot.ProjectConfig *
                   sourceFilePaths: string seq ->
                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

    member
      AddOrUpdate: projectPath: string * outputPath: string *
                   compilerArgs: string seq ->
                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

    member
      AddOrUpdate: projectFileName: string * outputFileName: string *
                   sourceFiles: string seq * referencesOnDisk: string seq *
                   otherOptions: string list ->
                     FSharp.Compiler.CodeAnalysis.ProjectSnapshot.FSharpProjectIdentifier

