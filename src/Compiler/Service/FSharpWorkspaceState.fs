// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Code to handle state management in an F# workspace.
module FSharp.Compiler.CodeAnalysis.Workspace.FSharpWorkspaceState

open System
open System.IO
open System.Runtime.CompilerServices
open System.Collections.Concurrent

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open Internal.Utilities.Collections
open Internal.Utilities.DependencyGraph

#nowarn "57"

/// Types for the workspace graph. These should not be accessed directly, rather through the
/// extension methods in `WorkspaceDependencyGraphExtensions`.
module internal WorkspaceGraphTypes =

    /// All project information except source files
    type ProjectWithoutFiles = ProjectConfig * FSharpReferencedProjectSnapshot list

    [<RequireQualifiedAccess>]
    type internal WorkspaceNodeKey =
        // TODO: maybe this should be URI
        | SourceFile of filePath: string
        | ReferenceOnDisk of filePath: string
        /// All project information except source files and (in-memory) project references
        | ProjectConfig of FSharpProjectIdentifier
        /// All project information except source files
        | ProjectWithoutFiles of FSharpProjectIdentifier
        /// Complete project information
        | ProjectSnapshot of FSharpProjectIdentifier

        override this.ToString() =
            match this with
            | SourceFile path -> $"File {shortPath path}"
            | ReferenceOnDisk path -> $"Reference on disk {shortPath path}"
            | ProjectConfig id -> $"ProjectConfig {id}"
            | ProjectWithoutFiles id -> $"ProjectWithoutFiles {id}"
            | ProjectSnapshot id -> $"ProjectSnapshot {id}"

    [<RequireQualifiedAccess>]
    type internal WorkspaceNodeValue =
        | SourceFile of FSharpFileSnapshot
        | ReferenceOnDisk of ReferenceOnDisk
        /// All project information except source files and (in-memory) project references
        | ProjectConfig of ProjectConfig
        /// All project information except source files
        | ProjectWithoutFiles of ProjectWithoutFiles
        /// Complete project information
        | ProjectSnapshot of FSharpProjectSnapshot

    module internal WorkspaceNode =

        let projectConfig value =
            match value with
            | WorkspaceNodeValue.ProjectConfig p -> Some p
            | _ -> None

        let projectSnapshot value =
            match value with
            | WorkspaceNodeValue.ProjectSnapshot p -> Some p
            | _ -> None

        let projectWithoutFiles value =
            match value with
            | WorkspaceNodeValue.ProjectWithoutFiles(p, refs) -> Some(p, refs)
            | _ -> None

        let sourceFile value =
            match value with
            | WorkspaceNodeValue.SourceFile f -> Some f
            | _ -> None

        let referenceOnDisk value =
            match value with
            | WorkspaceNodeValue.ReferenceOnDisk r -> Some r
            | _ -> None

        let projectConfigKey value =
            match value with
            | WorkspaceNodeKey.ProjectConfig p -> Some p
            | _ -> None

        let projectSnapshotKey value =
            match value with
            | WorkspaceNodeKey.ProjectSnapshot p -> Some p
            | _ -> None

        let projectWithoutFilesKey value =
            match value with
            | WorkspaceNodeKey.ProjectWithoutFiles x -> Some x
            | _ -> None

        let sourceFileKey value =
            match value with
            | WorkspaceNodeKey.SourceFile f -> Some f
            | _ -> None

        let referenceOnDiskKey value =
            match value with
            | WorkspaceNodeKey.ReferenceOnDisk r -> Some r
            | _ -> None

[<AutoOpen>]
module internal WorkspaceDependencyGraphExtensions =

    open WorkspaceGraphTypes

    /// This type adds extension methods to the dependency graph to constraint the types and type relations
    /// that can be added to the graph.
    ///
    /// All unsafe operations that can throw at runtime, i.e. unpacking, are done here.
    type internal WorkspaceDependencyGraphTypeExtensions =

        [<Extension>]
        static member AddOrUpdateFile(this: IDependencyGraph<_, _>, file: string, snapshot) =
            this.AddOrUpdateNode(WorkspaceNodeKey.SourceFile file, WorkspaceNodeValue.SourceFile(snapshot))

        [<Extension>]
        static member AddFiles(this: IDependencyGraph<_, _>, files: seq<string * FSharpFileSnapshot>) =
            let ids =
                files
                |> Seq.map (fun (file, snapshot) -> WorkspaceNodeKey.SourceFile file, WorkspaceNodeValue.SourceFile(snapshot))
                |> this.AddList

            GraphBuilder(this, ids, _.UnpackMany(WorkspaceNode.sourceFile), ())

        [<Extension>]
        static member AddReferencesOnDisk(this: IDependencyGraph<_, _>, references: seq<ReferenceOnDisk>) =
            let ids =
                references
                |> Seq.map (fun r -> WorkspaceNodeKey.ReferenceOnDisk r.Path, WorkspaceNodeValue.ReferenceOnDisk r)
                |> this.AddList

            GraphBuilder(this, ids, _.UnpackMany(WorkspaceNode.referenceOnDisk), ())

        [<Extension>]
        static member AddProjectConfig(this: GraphBuilder<_, _, ReferenceOnDisk seq, unit>, projectIdentifier, computeProjectConfig) =
            this.AddDependentNode(
                WorkspaceNodeKey.ProjectConfig projectIdentifier,
                computeProjectConfig >> WorkspaceNodeValue.ProjectConfig,
                _.UnpackOneMany(WorkspaceNode.projectConfig, WorkspaceNode.projectSnapshot),
                projectIdentifier
            )

        [<Extension>]
        static member AddProjectWithoutFiles
            (
                this: GraphBuilder<_, _, (ProjectConfig * FSharpProjectSnapshot seq), _>,
                computeProjectWithoutFiles
            ) =
            this.AddDependentNode(
                WorkspaceNodeKey.ProjectWithoutFiles this.State,
                computeProjectWithoutFiles >> WorkspaceNodeValue.ProjectWithoutFiles,
                _.UnpackOne(WorkspaceNode.projectWithoutFiles)
            )

        [<Extension>]
        static member AddSourceFiles(this: GraphBuilder<_, _, ProjectWithoutFiles, FSharpProjectIdentifier>, sourceFiles) =
            let ids =
                sourceFiles
                |> Seq.map (fun (file, snapshot) -> WorkspaceNodeKey.SourceFile file, WorkspaceNodeValue.SourceFile(snapshot))
                |> this.Graph.AddList

            GraphBuilder(
                this.Graph,
                (Seq.append this.Ids ids),
                (_.UnpackOneMany(WorkspaceNode.projectWithoutFiles, WorkspaceNode.sourceFile)),
                this.State
            )

        [<Extension>]
        static member AddProjectSnapshot
            (
                this: GraphBuilder<_, _, (ProjectWithoutFiles * FSharpFileSnapshot seq), _>,
                computeProjectSnapshot
            ) =

            this.AddDependentNode(
                WorkspaceNodeKey.ProjectSnapshot this.State,
                computeProjectSnapshot >> WorkspaceNodeValue.ProjectSnapshot,
                ignore
            )
            |> ignore

        [<Extension>]
        static member AddProjectReference(this: IDependencyGraph<_, _>, project, dependsOn) =
            this.AddDependency(WorkspaceNodeKey.ProjectWithoutFiles project, dependsOn = WorkspaceNodeKey.ProjectSnapshot dependsOn)

        [<Extension>]
        static member RemoveProjectReference(this: IDependencyGraph<_, _>, project, noLongerDependsOn) =
            this.RemoveDependency(
                WorkspaceNodeKey.ProjectWithoutFiles project,
                noLongerDependsOn = WorkspaceNodeKey.ProjectSnapshot noLongerDependsOn
            )

        [<Extension>]
        static member GetProjectSnapshot(this: IDependencyGraph<_, _>, project) =
            this
                .GetValue(WorkspaceNodeKey.ProjectSnapshot project)
                .Unpack(WorkspaceNode.projectSnapshot)

        [<Extension>]
        static member GetProjectReferencesOf(this: IDependencyGraph<_, _>, project) =
            this.GetDependenciesOf(WorkspaceNodeKey.ProjectWithoutFiles project)
            |> Seq.choose (function
                | WorkspaceNodeKey.ProjectSnapshot projectId -> Some projectId
                | _ -> None)

        [<Extension>]
        static member GetProjectsThatReference(this: IDependencyGraph<_, _>, dllPath) =
            this
                .GetDependentsOf(WorkspaceNodeKey.ReferenceOnDisk dllPath)
                .UnpackMany(WorkspaceNode.projectConfigKey)

        [<Extension>]
        static member GetProjectsContaining(this: IDependencyGraph<_, _>, file) =
            this.GetDependentsOf(WorkspaceNodeKey.SourceFile file)
            |> Seq.map this.GetValue
            |> _.UnpackMany(WorkspaceNode.projectSnapshot)

        [<Extension>]
        static member GetSourceFile(this: IDependencyGraph<_, _>, file) =
            this.GetValue(WorkspaceNodeKey.SourceFile file)
            |> _.Unpack(WorkspaceNode.sourceFile)

/// Interface for managing files in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceFiles internal (depGraph: IThreadSafeDependencyGraph<_, _>) =

    /// Open files in the editor.
    let openFiles = ConcurrentDictionary<string, string>()

    /// Indicates that a file has been opened and has the given content. Any updates to the file should be done through `Files.Edit`.
    member this.Open = this.Edit

    /// Indicates that a file has been changed and now has the given content. If it wasn't previously open it is considered open now.
    member _.Edit(file: Uri, content) =
        openFiles.AddOrUpdate(file.LocalPath, content, (fun _ _ -> content)) |> ignore
        depGraph.AddOrUpdateFile(file.LocalPath, FSharpFileSnapshot.CreateFromString(file.LocalPath, content))

    /// Indicates that a file has been closed. Any changes that were not saved to disk are undone and any further reading
    /// of the file's contents will be from the filesystem.
    member _.Close(file: Uri) =
        openFiles.TryRemove(file.LocalPath) |> ignore

        // The file may have had changes that weren't saved to disk and are therefore undone by closing it.
        depGraph.AddOrUpdateFile(file.LocalPath, FSharpFileSnapshot.CreateFromFileSystem(file.LocalPath))

    member internal _.GetFileContentIfOpen(path: string) =
        match openFiles.TryGetValue(path) with
        | true, content -> Some content
        | false, _ -> None

/// Interface for managing with projects in an F# workspace.
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpWorkspaceProjects internal (depGraph: IThreadSafeDependencyGraph<_, _>, files: FSharpWorkspaceFiles) =

    /// A map from project output path to project identifier.
    let outputPathMap = ConcurrentDictionary<string, FSharpProjectIdentifier>()

    /// Adds or updates an F# project in the workspace. Project is identified by the project file and output path or FSharpProjectIdentifier.
    member _.AddOrUpdate(projectConfig: ProjectConfig, sourceFilePaths: string seq) =

        let projectIdentifier = projectConfig.Identifier

        // Add the project identifier to the map
        // TODO: do something if it's empty?
        outputPathMap.AddOrUpdate(projectIdentifier.OutputFileName, (fun _ -> projectIdentifier), (fun _ _ -> projectIdentifier))
        |> ignore

        // Find any referenced projects that we aleady know about
        let projectReferences =
            projectConfig.ReferencesOnDisk
            |> Seq.choose (fun ref ->
                match outputPathMap.TryGetValue ref.Path with
                | true, projectIdentifier -> Some projectIdentifier
                | _ -> None)
            |> Set

        depGraph.Transact(fun depGraph ->

            depGraph
                .AddReferencesOnDisk(projectConfig.ReferencesOnDisk)
                .AddProjectConfig(projectIdentifier, (fun refsOnDisk -> projectConfig.With(refsOnDisk |> Seq.toList)))
                .AddProjectWithoutFiles(
                    (fun (projectConfig, referencedProjects) ->

                        let referencedProjects =
                            referencedProjects
                            |> Seq.map (fun s ->
                                FSharpReferencedProjectSnapshot.FSharpReference(
                                    s.OutputFileName
                                    |> Option.defaultWith (fun () -> failwith "project doesn't have output filename"),
                                    s
                                ))
                            |> Seq.toList

                        projectConfig, referencedProjects)
                )
                .AddSourceFiles(
                    sourceFilePaths
                    |> Seq.map (fun path ->
                        path,
                        files.GetFileContentIfOpen path
                        |> Option.map (fun content -> FSharpFileSnapshot.CreateFromString(path, content))
                        |> Option.defaultWith (fun () -> FSharpFileSnapshot.CreateFromFileSystem path))
                )
                .AddProjectSnapshot(
                    (fun ((projectConfig, referencedProjects), sourceFiles) ->
                        ProjectSnapshot(projectConfig, referencedProjects, sourceFiles |> Seq.toList)
                        |> FSharpProjectSnapshot)
                )

            // In case this is an update, we should check for any existing project references that are not contained in the incoming compiler args and remove them
            let existingReferences = depGraph.GetProjectReferencesOf projectIdentifier |> Set

            let referencesToRemove = existingReferences - projectReferences
            let referencesToAdd = projectReferences - existingReferences

            for projectId in referencesToRemove do
                depGraph.RemoveProjectReference(projectIdentifier, projectId)

            for projectId in referencesToAdd do
                depGraph.AddProjectReference(projectIdentifier, projectId)

            // Check if any projects we know about depend on this project and add the references if they don't already exist
            let dependentProjectIds =
                depGraph.GetProjectsThatReference projectIdentifier.OutputFileName

            for dependentProjectId in dependentProjectIds do
                depGraph.AddProjectReference(dependentProjectId, projectIdentifier)

            projectIdentifier)

    member this.AddOrUpdate(projectPath: string, outputPath, compilerArgs) =

        let directoryPath =
            Path.GetDirectoryName(projectPath) |> Option.ofObj |> Option.defaultValue ""

        let fsharpFileExtensions = set [| ".fs"; ".fsi"; ".fsx" |]

        let isFSharpFile (file: string) =
            Set.exists (fun (ext: string) -> file.EndsWith(ext, StringComparison.Ordinal)) fsharpFileExtensions

        let isReference: string -> bool = _.StartsWith("-r:")

        let referencesOnDisk =
            compilerArgs |> Seq.filter isReference |> Seq.map _.Substring(3)

        let otherOptions =
            compilerArgs
            |> Seq.filter (not << isReference)
            |> Seq.filter (not << isFSharpFile)
            |> Seq.toList

        let sourceFiles =
            compilerArgs
            |> Seq.choose (fun (line: string) ->
                if not (isFSharpFile line) then
                    None
                else
                    Some(Path.Combine(directoryPath, line)))

        this.AddOrUpdate(projectPath, outputPath, sourceFiles, referencesOnDisk, otherOptions)

    member this.AddOrUpdate(projectFileName, outputFileName, sourceFiles, referencesOnDisk, otherOptions) =

        let projectConfig =
            ProjectConfig(projectFileName, Some outputFileName, referencesOnDisk, otherOptions)

        this.AddOrUpdate(projectConfig, sourceFiles)
