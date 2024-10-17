namespace FSharp.Compiler.LanguageServer.Common

open FSharp.Compiler.Text
open System.Collections.Generic
open DependencyGraph
open System.IO
open System.Runtime.CompilerServices
open System.Threading

#nowarn "57"

open System
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

[<RequireQualifiedAccess>]
type internal WorkspaceNodeKey =
    // TODO: maybe this should be URI
    | SourceFile of filePath: string
    | ReferenceOnDisk of filePath: string
    | ProjectCore of FSharpProjectIdentifier
    | ProjectBase of FSharpProjectIdentifier
    | ProjectSnapshot of FSharpProjectIdentifier

[<RequireQualifiedAccess>]
type internal WorkspaceNodeValue =
    | SourceFile of FSharpFileSnapshot
    | ReferenceOnDisk of ReferenceOnDisk
    | ProjectCore of ProjectCore
    | ProjectBase of ProjectCore * FSharpReferencedProjectSnapshot list
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

type FSharpWorkspace() =

    let depGraph = LockOperatedDependencyGraph() :> IThreadSafeDependencyGraph<_, _>

    let mutable resultIdCounter = 0

    member internal this.Debug =
        {|
            Snapshots =
                depGraph.Debug_GetNodes (function
                    | WorkspaceNodeKey.ProjectSnapshot _ -> true
                    | _ -> false)

        |}

    // TODO: we might need something more sophisticated eventually
    // for now it's important that the result id is unique every time
    // in order to be able to clear previous diagnostics
    member this.GetDiagnosticResultId() = Interlocked.Increment(&resultIdCounter)

    member this.OpenFile(file: Uri, content: string) =
        // No changes in the dep graph. If we already read the contents from disk we don't want to invalidate it.
        this

    member this.CloseFile(file: Uri) =
        // No changes in the dep graph. Next change will come if we get notified by file watcher.
        this

    member this.ChangeFile(file: Uri, content: string) =

        depGraph.AddOrUpdateNode(
            WorkspaceNodeKey.SourceFile file.LocalPath,
            WorkspaceNodeValue.SourceFile(
                FSharpFileSnapshot.Create(
                    file.LocalPath,
                    content.GetHashCode().ToString(),
                    fun () -> content |> SourceTextNew.ofString |> Task.FromResult
                )
            )
        )
        |> ignore

        this

    member _.AddCommandLineArgs(projectPath: string, outputPath: string | null, compilerArgs: string seq) =

        let outputPath =
            outputPath
            |> Option.ofObj
            // TODO: maybe there are cases where it's appropriate to not have output path?
            |> Option.defaultWith (fun () -> failwith "Output path can't be null for an F# project")

        let projectIdentifier = FSharpProjectIdentifier(projectPath, outputPath)

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
                {
                    Path = path
                    LastModified = File.GetLastWriteTimeUtc path
                })

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
                    WorkspaceNodeKey.SourceFile path, WorkspaceNodeValue.SourceFile(FSharpFileSnapshot.CreateFromFileSystem path))
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

            projectIdentifier)

    member _.AddProjectReferences(project: FSharpProjectIdentifier, references: FSharpProjectIdentifier seq) =
        depGraph.Transact(fun depGraph ->

            references
            |> Seq.iter (fun reference ->

                //let outputPath =
                //    reference
                //    |> function
                //        | (FSharpProjectIdentifier(_, outputPath)) -> outputPath

                depGraph.AddDependency(WorkspaceNodeKey.ProjectBase project, dependsOn = WorkspaceNodeKey.ProjectSnapshot reference)

            // hopefully not needed
            //depGraph.RemoveDependency(
            //    WorkspaceNodeKey.ProjectCore project,
            //    noLongerDependsOn = WorkspaceNodeKey.ReferenceOnDisk outputPath
            //))
            ))
    // TODO: might need to remove the -r: references from the project core; maybe even remove the particular reference on disk from the graph

    member _.GetSnapshotForFile(file: Uri) =
        depGraph.Transact(fun depGraph ->

            depGraph.GetDependentsOf(WorkspaceNodeKey.SourceFile file.LocalPath)

            // TODO: eventually we need to deal with choosing the appropriate project here
            // Hopefully we will be able to do it through receiving project context from LSP
            // Otherwise we have to keep track of which project/configuration is active
            |> Seq.tryHead // For now just get the first one

            |> Option.map depGraph.GetValue
            |> Option.map _.Unpack(WorkspaceNode.projectSnapshot))
