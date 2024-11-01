namespace FSharp.Compiler.LanguageServer.Common

open FSharp.Compiler.Text
open System.Collections.Generic
open DependencyGraph
open System.IO
open System.Runtime.CompilerServices
open System.Threading
open System.Collections.Concurrent

#nowarn "57"

open System
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open Internal.Utilities.Collections

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

/// This type holds the current state of an F# workspace (or, solution). It's mutable but thread-safe. It accepts updates to the state and can provide immutable snapshots of contained F# projects. The state can be built up incrementally by adding projects and dependencies between them.
type FSharpWorkspace() =

    let depGraph = LockOperatedDependencyGraph() :> IThreadSafeDependencyGraph<_, _>

    /// A map from project output path to project identifier.
    let outputPathMap = ConcurrentDictionary<string, FSharpProjectIdentifier>()

    /// Open files in the editor.
    let openFiles = ConcurrentDictionary<string, string>()

    let mutable resultIdCounter = 0

    member internal this.Debug_DumpMermaid(path) =
        let content =
            depGraph.Debug_RenderMermaid (function
                | WorkspaceGraphTypes.WorkspaceNodeKey.ReferenceOnDisk _ -> WorkspaceGraphTypes.WorkspaceNodeKey.ReferenceOnDisk "..."
                | x -> x)

        File.WriteAllText(__SOURCE_DIRECTORY__ + path, content)

    // TODO: we might need something more sophisticated eventually
    // for now it's important that the result id is unique every time
    // in order to be able to clear previous diagnostics
    member this.GetDiagnosticResultId() = Interlocked.Increment(&resultIdCounter)

    member this.OpenFile(file: Uri, content) =
        openFiles.AddOrUpdate(file.LocalPath, content, (fun _ _ -> content)) |> ignore

        // No changes in the dep graph. If we already read the contents from disk we don't want to invalidate it.
        this

    member this.CloseFile(file: Uri) =
        openFiles.TryRemove(file.LocalPath) |> ignore

        // The file may have had changes that weren't saved to disk and are therefore undone by closing it.
        depGraph.AddOrUpdateFile(file.LocalPath, FSharpFileSnapshot.CreateFromFileSystem(file.LocalPath))

        this

    member this.ChangeFile(file: Uri, content) =

        depGraph.AddOrUpdateFile(file.LocalPath, FSharpFileSnapshot.CreateFromString(file.LocalPath, content))

        this.OpenFile(file, content)

    /// Adds an F# project to the workspace. The project is identified by path to the .fsproj file and output path.
    /// The compiler arguments are used to build the project's snapshot.
    /// References are created automatically between known projects based on the compiler arguments and output paths.
    member this.AddProject(projectPath: string, outputPath, compilerArgs) =

        let directoryPath = Path.GetDirectoryName(projectPath)

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
                    let fullPath = Path.Combine(directoryPath, line)
                    if not (File.Exists fullPath) then None else Some fullPath)

        this.AddProject(projectPath, outputPath, sourceFiles, referencesOnDisk, otherOptions)

    /// Adds an F# project to the workspace. The project is identified by path to the .fsproj file and output path.
    /// References are created automatically between known projects based on the compiler arguments and output paths.
    member _.AddProject(projectFileName, outputFileName, sourceFiles, referencesOnDisk, otherOptions) =

        let projectIdentifier = FSharpProjectIdentifier(projectFileName, outputFileName)

        // Add the project identifier to the map
        outputPathMap.AddOrUpdate(outputFileName, (fun _ -> projectIdentifier), (fun _ _ -> projectIdentifier))
        |> ignore

        let referencesOnDisk =
            referencesOnDisk
            |> Seq.map (fun path ->
                {
                    Path = path
                    LastModified = File.GetLastWriteTimeUtc path
                })
            |> Seq.toList

        // Find any referenced projects that we aleady know about
        let projectReferences =
            referencesOnDisk
            |> Seq.choose (fun ref ->
                match outputPathMap.TryGetValue ref.Path with
                | true, projectIdentifier -> Some projectIdentifier
                | _ -> None)
            |> Set

        depGraph.Transact(fun depGraph ->

            depGraph
                .AddReferencesOnDisk(referencesOnDisk)
                .AddProjectConfig(
                    projectIdentifier,
                    (fun refsOnDisk ->
                        ProjectConfig(
                            projectFileName = projectIdentifier.ProjectFileName,
                            outputFileName = Some projectIdentifier.OutputFileName,
                            projectId = None,
                            referencesOnDisk = (refsOnDisk |> Seq.toList),
                            otherOptions = otherOptions,
                            isIncompleteTypeCheckEnvironment = false,
                            useScriptResolutionRules = false,
                            loadTime = DateTime.Now,
                            unresolvedReferences = None,
                            originalLoadReferences = [],
                            stamp = None
                        ))
                )
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
                    sourceFiles
                    |> Seq.map (fun path ->
                        path,
                        match openFiles.TryGetValue(path) with
                        | true, content -> FSharpFileSnapshot.CreateFromString(path, content)
                        | false, _ -> FSharpFileSnapshot.CreateFromFileSystem path)
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
            let dependentProjectIds = depGraph.GetProjectsThatReference outputFileName

            for dependentProjectId in dependentProjectIds do
                depGraph.AddProjectReference(dependentProjectId, projectIdentifier)

            projectIdentifier)

    member _.GetProjectSnapshot = depGraph.GetProjectSnapshot

    member _.GetProjectSnapshotForFile(file: Uri) =

        depGraph.GetProjectsContaining file.LocalPath

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Seq.tryHead // For now just get the first one
