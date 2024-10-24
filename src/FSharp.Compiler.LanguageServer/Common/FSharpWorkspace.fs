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
        | ReferenceOnDisk path -> $"Reference {shortPath path}"
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
    | ProjectBase of ProjectCore * FSharpReferencedProjectSnapshot list
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

    member internal this.Debug =
        {|
            Snapshots =
                depGraph.Debug_GetNodes (function
                    | WorkspaceNodeKey.ProjectSnapshot _ -> true
                    | _ -> false)
        |}

    member internal this.Debug_DumpMermaid(path) =
        let content = depGraph.Debug_RenderMermaid()
        File.WriteAllText(path, content)

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
        depGraph.AddOrUpdateNode(
            WorkspaceNodeKey.SourceFile file.LocalPath,
            WorkspaceNodeValue.SourceFile(FSharpFileSnapshot.CreateFromFileSystem(file.LocalPath))
        )
        |> ignore

        this

    member this.ChangeFile(file: Uri, content) =

        depGraph.AddOrUpdateNode(
            WorkspaceNodeKey.SourceFile file.LocalPath,
            WorkspaceNodeValue.SourceFile(FSharpFileSnapshot.CreateFromString(file.LocalPath, content))
        )
        |> ignore

        this.OpenFile(file, content)

    /// Adds an F# project to the workspace. The project is identified path to the .fsproj file and output path. The compiler arguments are used to build the project's snapshot.
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

            let fsharpFiles =
                compilerArgs
                |> Seq.choose (fun (line: string) ->
                    if not (isFSharpFile line) then
                        None
                    else
                        let fullPath = Path.Combine(directoryPath, line)
                        if not (File.Exists fullPath) then None else Some fullPath)
                |> Seq.map (fun path ->
                    WorkspaceNodeKey.SourceFile path,
                    WorkspaceNodeValue.SourceFile(
                        match openFiles.TryGetValue(path) with
                        | true, content -> FSharpFileSnapshot.CreateFromString(path, content)
                        | false, _ -> FSharpFileSnapshot.CreateFromFileSystem path
                    ))
                |> depGraph.AddList

            let referencesOnDiskNodes =
                referencesOnDisk
                |> Seq.map (fun r -> WorkspaceNodeKey.ReferenceOnDisk r.Path, WorkspaceNodeValue.ReferenceOnDisk r)
                |> depGraph.AddList

            let projectCore =
                referencesOnDiskNodes.AddDependentNode(
                    WorkspaceNodeKey.ProjectCore projectIdentifier,
                    (fun deps ->
                        let refsOnDisk = deps.UnpackMany WorkspaceNode.referenceOnDisk |> Seq.toList

                        ProjectCore(
                            ProjectFileName = projectPath,
                            OutputFileName = Some outputPath,
                            ProjectId = None,
                            ReferencesOnDisk = refsOnDisk,
                            OtherOptions = otherOptions,
                            IsIncompleteTypeCheckEnvironment = false,
                            UseScriptResolutionRules = false,
                            LoadTime = DateTime.Now,
                            UnresolvedReferences = None,
                            OriginalLoadReferences = [],
                            Stamp = None
                        )

                        |> WorkspaceNodeValue.ProjectCore)
                )

            let projectBase =
                projectCore.AddDependentNode(
                    WorkspaceNodeKey.ProjectBase projectIdentifier,
                    (fun deps ->
                        let projectCore, referencedProjects =
                            deps.UnpackOneMany(WorkspaceNode.projectCore, WorkspaceNode.projectSnapshot)

                        let referencedProjects =
                            referencedProjects
                            |> Seq.map (fun s ->
                                FSharpReferencedProjectSnapshot.FSharpReference(
                                    s.OutputFileName
                                    |> Option.defaultWith (fun () -> failwith "project doesn't have output filename"),
                                    s
                                ))
                            |> Seq.toList

                        WorkspaceNodeValue.ProjectBase(projectCore, referencedProjects))
                )

            // In case this is an update, we should check for any existing project references that are not contained in the incoming compiler args and remove them
            let existingReferences =
                depGraph.GetDependenciesOf(WorkspaceNodeKey.ProjectBase projectIdentifier)
                |> Seq.choose (function
                    | WorkspaceNodeKey.ProjectSnapshot projectId -> Some projectId
                    | _ -> None)
                |> Set

            let referencesToRemove = existingReferences - projectReferences
            let referencesToAdd = projectReferences - existingReferences

            for projectId in referencesToRemove do
                depGraph.RemoveDependency(
                    WorkspaceNodeKey.ProjectBase projectIdentifier,
                    noLongerDependsOn = WorkspaceNodeKey.ProjectSnapshot projectId
                )

            for projectId in referencesToAdd do
                depGraph.AddDependency(
                    WorkspaceNodeKey.ProjectBase projectIdentifier,
                    dependsOn = WorkspaceNodeKey.ProjectSnapshot projectId
                )

            projectBase
                .And(fsharpFiles)
                .AddDependentNode(
                    WorkspaceNodeKey.ProjectSnapshot projectIdentifier,
                    (fun deps ->

                        let (projectCore, referencedProjects), sourceFiles =
                            deps.UnpackOneMany(WorkspaceNode.projectBase, WorkspaceNode.sourceFile)

                        ProjectSnapshot(projectCore, referencedProjects, sourceFiles |> Seq.toList)
                        |> FSharpProjectSnapshot
                        |> WorkspaceNodeValue.ProjectSnapshot)
                )
            |> ignore

            // Check if any projects we know about depend on this project and add the references if they don't already exist
            let dependentProjectIds =
                depGraph
                    .GetDependentsOf(WorkspaceNodeKey.ReferenceOnDisk outputPath)
                    .UnpackMany(WorkspaceNode.projectCoreKey)

            for dependentProjectId in dependentProjectIds do
                depGraph.AddDependency(
                    WorkspaceNodeKey.ProjectBase dependentProjectId,
                    dependsOn = WorkspaceNodeKey.ProjectSnapshot projectIdentifier
                )

            projectIdentifier)

    member _.GetSnapshotForFile(file: Uri) =
        depGraph.Transact(fun depGraph ->

            depGraph.GetDependentsOf(WorkspaceNodeKey.SourceFile file.LocalPath)

            // TODO: eventually we need to deal with choosing the appropriate project here
            // Hopefully we will be able to do it through receiving project context from LSP
            // Otherwise we have to keep track of which project/configuration is active
            |> Seq.tryHead // For now just get the first one

            |> Option.map depGraph.GetValue
            |> Option.map _.Unpack(WorkspaceNode.projectSnapshot))
