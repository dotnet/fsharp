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


module internal WorkspaceGraphTypes =

    type ProjectBase = ProjectCore * FSharpReferencedProjectSnapshot list

    [<RequireQualifiedAccess>]
    type internal WorkspaceNodeKey =
        // TODO: maybe this should be URI
        | SourceFile of filePath: string
        | ReferenceOnDisk of filePath: string
        /// All project information except source files and (in-memory) project references
        | ProjectCore of FSharpProjectIdentifier
        /// All project information except source files
        | ProjectBase of FSharpProjectIdentifier
        /// Complete project information
        | ProjectSnapshot of FSharpProjectIdentifier

        override this.ToString() =
            match this with
            | SourceFile path -> $"File {shortPath path}"
            | ReferenceOnDisk path -> $"Reference on disk {shortPath path}"
            | ProjectCore id -> $"ProjectCore {id}"
            | ProjectBase id -> $"ProjectBase {id}"
            | ProjectSnapshot id -> $"ProjectSnapshot {id}"

    [<RequireQualifiedAccess>]
    type internal WorkspaceNodeValue =
        | SourceFile of FSharpFileSnapshot
        | ReferenceOnDisk of ReferenceOnDisk
        /// All project information except source files and (in-memory) project references
        | ProjectCore of ProjectCore
        /// All project information except source files
        | ProjectBase of ProjectBase
        /// Complete project information
        | ProjectSnapshot of FSharpProjectSnapshot

    module internal WorkspaceNode =

        let projectCore value =
            match value with
            | WorkspaceNodeValue.ProjectCore p -> Some p
            | _ -> None

        let projectSnapshot value =
            match value with
            | WorkspaceNodeValue.ProjectSnapshot p -> Some p
            | _ -> None

        let projectBase value =
            match value with
            | WorkspaceNodeValue.ProjectBase(p, refs) -> Some(p, refs)
            | _ -> None

        let sourceFile value =
            match value with
            | WorkspaceNodeValue.SourceFile f -> Some f
            | _ -> None

        let referenceOnDisk value =
            match value with
            | WorkspaceNodeValue.ReferenceOnDisk r -> Some r
            | _ -> None

        let projectCoreKey value =
            match value with
            | WorkspaceNodeKey.ProjectCore p -> Some p
            | _ -> None

        let projectSnapshotKey value =
            match value with
            | WorkspaceNodeKey.ProjectSnapshot p -> Some p
            | _ -> None

        let projectBaseKey value =
            match value with
            | WorkspaceNodeKey.ProjectBase x -> Some x
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

    /// This type adds extension methods to the dependency graph to constraint the types and type relations that can be added to the graph.
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
        static member AddProjectCore(this: GraphBuilder<_, _, ReferenceOnDisk seq, unit>, projectIdentifier, computeProjectCore) =
            this.AddDependentNode(
                WorkspaceNodeKey.ProjectCore projectIdentifier,
                computeProjectCore >> WorkspaceNodeValue.ProjectCore,
                _.UnpackOneMany(WorkspaceNode.projectCore, WorkspaceNode.projectSnapshot),
                projectIdentifier
            )

        [<Extension>]
        static member AddProjectBase(this: GraphBuilder<_, _, (ProjectCore * FSharpProjectSnapshot seq), _>, computeProjectBase) =
            this.AddDependentNode(
                WorkspaceNodeKey.ProjectBase this.State,
                computeProjectBase >> WorkspaceNodeValue.ProjectBase,
                _.UnpackOne(WorkspaceNode.projectBase)
            )

        [<Extension>]
        static member AddSourceFiles(this: GraphBuilder<_, _, ProjectBase, FSharpProjectIdentifier>, sourceFiles) =
            let ids =
                sourceFiles
                |> Seq.map (fun (file, snapshot) -> WorkspaceNodeKey.SourceFile file, WorkspaceNodeValue.SourceFile(snapshot))
                |> this.Graph.AddList

            GraphBuilder(
                this.Graph,
                (Seq.append this.Ids ids),
                (_.UnpackOneMany(WorkspaceNode.projectBase, WorkspaceNode.sourceFile)),
                this.State
            )

        [<Extension>]
        static member AddProjectSnapshot(this: GraphBuilder<_, _, (ProjectBase * FSharpFileSnapshot seq), _>, computeProjectSnapshot) =

            this.AddDependentNode(
                WorkspaceNodeKey.ProjectSnapshot this.State,
                computeProjectSnapshot >> WorkspaceNodeValue.ProjectSnapshot,
                ignore
            )
            |> ignore

        [<Extension>]
        static member AddProjectReference(this: IDependencyGraph<_, _>, project, dependsOn) =
            this.AddDependency(WorkspaceNodeKey.ProjectBase project, dependsOn = WorkspaceNodeKey.ProjectSnapshot dependsOn)

        [<Extension>]
        static member RemoveProjectReference(this: IDependencyGraph<_, _>, project, noLongerDependsOn) =
            this.RemoveDependency(
                WorkspaceNodeKey.ProjectBase project,
                noLongerDependsOn = WorkspaceNodeKey.ProjectSnapshot noLongerDependsOn
            )

        [<Extension>]
        static member GetProjectReferencesOf(this: IDependencyGraph<_, _>, project) =
            this.GetDependenciesOf(WorkspaceNodeKey.ProjectBase project)
            |> Seq.choose (function
                | WorkspaceNodeKey.ProjectSnapshot projectId -> Some projectId
                | _ -> None)

        [<Extension>]
        static member GetProjectsThatReference(this: IDependencyGraph<_, _>, dllPath) =
            this
                .GetDependentsOf(WorkspaceNodeKey.ReferenceOnDisk dllPath)
                .UnpackMany(WorkspaceNode.projectCoreKey)

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

    /// A map from reference on disk path to which projects depend on it. It can be used to create in-memory project references based on output paths.
    let referenceMap = ConcurrentDictionary<string, Set<FSharpProjectIdentifier>>()

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

    /// Adds an F# project to the workspace. The project is identified path to the .fsproj file and output path.
    /// The compiler arguments are used to build the project's snapshot.
    /// References are created automatically between known projects based on the compiler arguments and output paths.
    member _.AddCommandLineArgs(projectPath, outputPath, compilerArgs) =

        let outputPath =
            outputPath
            |> Option.ofObj
            // TODO: maybe there are cases where it's appropriate to not have output path?
            |> Option.defaultWith (fun () -> failwith "Output path can't be null for an F# project")

        let projectIdentifier = FSharpProjectIdentifier(projectPath, outputPath)

        // Add the project identifier to the map
        outputPathMap.AddOrUpdate(outputPath, (fun _ -> projectIdentifier), (fun _ _ -> projectIdentifier))
        |> ignore

        let directoryPath = Path.GetDirectoryName(projectPath)

        let fsharpFileExtensions = set [| ".fs"; ".fsi"; ".fsx" |]

        let isFSharpFile (file: string) =
            Set.exists (fun (ext: string) -> file.EndsWith(ext, StringComparison.Ordinal)) fsharpFileExtensions

        let isReference: string -> bool = _.StartsWith("-r:")

        let referencesOnDisk =
            compilerArgs
            |> Seq.filter isReference
            |> Seq.map _.Substring(3)
            |> Seq.map (fun path ->

                referenceMap.AddOrUpdate(
                    path,
                    (fun _ -> Set.singleton projectIdentifier),
                    (fun _ existing -> Set.add projectIdentifier existing)
                )
                |> ignore

                {
                    Path = path
                    LastModified = File.GetLastWriteTimeUtc path
                })
            |> Seq.toList

        let projectReferences =
            referencesOnDisk
            |> Seq.choose (fun ref ->
                match outputPathMap.TryGetValue ref.Path with
                | true, projectIdentifier -> Some projectIdentifier
                | _ -> None)
            |> Set

        let otherOptions =
            compilerArgs
            |> Seq.filter (not << isReference)
            |> Seq.filter (not << isFSharpFile)
            |> Seq.toList

        depGraph.Transact(fun depGraph ->

            depGraph
                .AddReferencesOnDisk(referencesOnDisk)
                .AddProjectCore(
                    projectIdentifier,
                    (fun refsOnDisk ->
                        ProjectCore(
                            ProjectFileName = projectPath,
                            OutputFileName = Some outputPath,
                            ProjectId = None,
                            ReferencesOnDisk = (refsOnDisk |> Seq.toList),
                            OtherOptions = otherOptions,
                            IsIncompleteTypeCheckEnvironment = false,
                            UseScriptResolutionRules = false,
                            LoadTime = DateTime.Now,
                            UnresolvedReferences = None,
                            OriginalLoadReferences = [],
                            Stamp = None
                        ))
                )
                .AddProjectBase(
                    (fun (projectCore, referencedProjects) ->

                        let referencedProjects =
                            referencedProjects
                            |> Seq.map (fun s ->
                                FSharpReferencedProjectSnapshot.FSharpReference(
                                    s.OutputFileName
                                    |> Option.defaultWith (fun () -> failwith "project doesn't have output filename"),
                                    s
                                ))
                            |> Seq.toList

                        projectCore, referencedProjects)
                )
                .AddSourceFiles(
                    compilerArgs
                    |> Seq.choose (fun (line: string) ->
                        if not (isFSharpFile line) then
                            None
                        else
                            let fullPath = Path.Combine(directoryPath, line)
                            if not (File.Exists fullPath) then None else Some fullPath)
                    |> Seq.map (fun path ->
                        path,
                        match openFiles.TryGetValue(path) with
                        | true, content -> FSharpFileSnapshot.CreateFromString(path, content)
                        | false, _ -> FSharpFileSnapshot.CreateFromFileSystem path)
                )
                .AddProjectSnapshot(
                    (fun ((projectCore, referencedProjects), sourceFiles) ->
                        ProjectSnapshot(projectCore, referencedProjects, sourceFiles |> Seq.toList)
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
            let dependentProjectIds = depGraph.GetProjectsThatReference outputPath

            for dependentProjectId in dependentProjectIds do
                depGraph.AddProjectReference(dependentProjectId, projectIdentifier)

            projectIdentifier)

    member _.GetSnapshotForFile(file: Uri) =

        depGraph.GetProjectsContaining file.LocalPath

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Seq.tryHead // For now just get the first one
