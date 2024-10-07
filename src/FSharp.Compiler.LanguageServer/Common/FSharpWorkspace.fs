namespace FSharp.Compiler.LanguageServer.Common

open FSharp.Compiler.Text
open System.Collections.Generic
open DependencyGraph
open System.IO

#nowarn "57"

open System
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

[<RequireQualifiedAccess>]
type internal WorkspaceNodeKey =
    // TODO: maybe this should be URI
    | SourceFile of filePath: string
    | ReferenceOnDisk of filePath: string
    | ProjectCore of ProjectIdentifier
    | ProjectBase of ProjectIdentifier
    | ProjectSnapshot of ProjectIdentifier

[<RequireQualifiedAccess>]
type internal WorkspaceNodeValue =
    | SourceFile of FSharpFileSnapshot
    | ReferenceOnDisk of ReferenceOnDisk
    | ProjectCore of ProjectCore
    | ProjectBase of ProjectCore * FSharpReferencedProjectSnapshot list
    | ProjectSnapshot of FSharpProjectSnapshot
    member this.UnwrapSourceFile() =
        match this with
        | SourceFile f -> f
        | x -> failwithf "Expected SourceFile, got %A" x
    member this.UnwrapReferenceOnDisk() =
        match this with
        | ReferenceOnDisk r -> r
        | x -> failwithf "Expected ReferenceOnDisk, got %A" x
    member this.UnwrapProjectCore() =
        match this with
        | ProjectCore p -> p
        | x -> failwithf "Expected ProjectCore, got %A" x
    member this.UnwrapProjectBase() =
        match this with
        | ProjectBase (p, refs) -> p, refs
        | x -> failwithf "Expected ProjectBase, got %A" x
    member this.UnwrapProjectSnapshot() =
        match this with
        | ProjectSnapshot p -> p
        | x -> failwithf "Expected ProjectSnapshot, got %A" x


/// Holds a project snapshot and a queue of changes that will be applied to it when it's requested
///
/// The assumption is that this is faster than actually applying the changes to the snapshot immediately and that
/// we will be doing this on potentially every keystroke. But this should probably be measured at some point.
type SnapshotHolder(snapshot: FSharpProjectSnapshot, changedFiles: Set<string>, openFiles: Map<string, string>) =

    let applyFileChangesToSnapshot () =
        let files =
            changedFiles
            |> Seq.map (fun filePath ->
                match openFiles.TryFind filePath with
                | Some content ->
                    FSharpFileSnapshot.Create(
                        filePath,
                        DateTime.Now.Ticks.ToString(),
                        fun () -> content |> SourceTextNew.ofString |> Task.FromResult
                    )
                | None -> FSharpFileSnapshot.CreateFromFileSystem(filePath))
            |> Seq.toList

        snapshot.Replace files

    // We don't want to mutate the workspace by applying the changes when snapshot is requested because that would force the language
    // requests to be processed sequentially. So instead we keep the change application under lazy so it's still only computed if needed
    // and only once and workspace doesn't change.
    let appliedChanges =
        lazy SnapshotHolder(applyFileChangesToSnapshot (), Set.empty, openFiles)

    member private _.snapshot = snapshot
    member private _.changedFiles = changedFiles

    member private this.GetMostUpToDateInstance() =
        if appliedChanges.IsValueCreated then
            appliedChanges.Value
        else
            this

    member this.WithFileChanged(file, openFiles) =
        let previous = this.GetMostUpToDateInstance()
        SnapshotHolder(previous.snapshot, previous.changedFiles.Add file, openFiles)

    member this.WithoutFileChanged(file, openFiles) =
        let previous = this.GetMostUpToDateInstance()
        SnapshotHolder(previous.snapshot, previous.changedFiles.Remove file, openFiles)

    member _.GetSnapshot() = appliedChanges.Value.snapshot

    static member Of(snapshot: FSharpProjectSnapshot) =
        SnapshotHolder(snapshot, Set.empty, Map.empty)

type FSharpWorkspace() =
    
    let depGraph = DependencyGraph()

    member this.OpenFile(file: Uri, content: string) =
        // No changes in the dep graph. If we already read the contents from disk we don't want to invalidate it. 
        this

    member this.CloseFile(file: Uri) =
        // No changes in the dep graph. Next change will come if we get notified by file watcher. 
        this

    member this.ChangeFile(file: Uri, content: string) =
        
        depGraph.AddOrUpdateNode(WorkspaceNodeKey.SourceFile file.LocalPath, WorkspaceNodeValue.SourceFile (FSharpFileSnapshot.Create(file.LocalPath, content.GetHashCode().ToString(), fun () -> content |> SourceTextNew.ofString |> Task.FromResult))) 
        |> ignore
        
        this

    member _.AddCommandLineArgs(projectPath: string, compilerArgs: string seq) =

        let findOutputFileName args =
            args
            |> Seq.tryFind (fun (x: string) -> x.StartsWith("-o:"))
            |> Option.map (fun x -> x.Substring(3))

        let outputPath = 
            findOutputFileName compilerArgs
            |> Option.defaultWith (fun () -> failwith "Invalid command line arguments for F# project, output file name not found")
        
        let projectIdentifier = ProjectIdentifier (projectPath, outputPath)

        let directoryPath = Path.GetDirectoryName(projectPath)

        let fsharpFileExtensions = set [| ".fs"; ".fsi"; ".fsx" |]

        let isFSharpFile (file: string) =
            Set.exists (fun (ext: string) -> file.EndsWith(ext, StringComparison.Ordinal)) fsharpFileExtensions

        let isReference: string -> bool = _.StartsWith("-r:")

        let fsharpFiles =
            compilerArgs
            |> Seq.choose (fun (line: string) ->
                if not (isFSharpFile line) then
                    None
                else

                    let fullPath = Path.Combine(directoryPath, line)
                    if not (File.Exists fullPath) then None else Some fullPath)
            |> Seq.map(fun path -> 
                WorkspaceNodeKey.SourceFile path,
                WorkspaceNodeValue.SourceFile (FSharpFileSnapshot.CreateFromFileSystem path))
            |> depGraph.AddList

        let referencesOnDisk =
            compilerArgs 
            |> Seq.filter isReference 
            |> Seq.map _.Substring(3) 
            |> Seq.map (fun path -> 
                WorkspaceNodeKey.ReferenceOnDisk path,
                WorkspaceNodeValue.ReferenceOnDisk {
                         Path = path
                         LastModified = File.GetLastWriteTimeUtc path
                     })
            |> depGraph.AddList
            

        let otherOptions =
            compilerArgs
            |> Seq.filter (not << isReference)
            |> Seq.filter (not << isFSharpFile)
            |> Seq.toList

        let projectCore = 
            referencesOnDisk
                .AddDependentNode(WorkspaceNodeKey.ProjectCore projectIdentifier, (fun refsOnDiskNodes ->
                    let refsOnDisk = refsOnDiskNodes |> Seq.map _.UnwrapReferenceOnDisk() |> Seq.toList

                    ProjectCore(
                        ProjectFileName = projectPath,
                        ProjectId = None,
                        ReferencesOnDisk = refsOnDisk,
                        OtherOptions = otherOptions,
                        IsIncompleteTypeCheckEnvironment = false,
                        UseScriptResolutionRules = false,
                        LoadTime = DateTime.Now,
                        UnresolvedReferences = None,
                        OriginalLoadReferences = [],
                        Stamp = None) 

                        |> WorkspaceNodeValue.ProjectCore))

        let projectBase = 
            projectCore.AddDependentNode(WorkspaceNodeKey.ProjectBase projectIdentifier, (fun deps ->
                let projectCore = deps |> Seq.head |> _.UnwrapProjectCore()
                let referencedProjects = 
                    deps 
                    |> Seq.skip 1 
                    |> Seq.map _.UnwrapProjectSnapshot()
                    |> Seq.map (fun s -> FSharpReferencedProjectSnapshot.FSharpReference (s.OutputFileName |> Option.defaultWith(fun () -> failwith "project doesn't have output filename"), s))
                    |> Seq.toList
                WorkspaceNodeValue.ProjectBase(projectCore, referencedProjects)))

        projectBase.And(fsharpFiles).AddDependentNode(WorkspaceNodeKey.ProjectSnapshot projectIdentifier, (fun deps ->
        
            let projectCore, referencedProjects = deps |> Seq.head |> _.UnwrapProjectBase()
            let sourceFiles = deps |> Seq.skip 1 |> Seq.map _.UnwrapSourceFile() |> Seq.toList

            ProjectSnapshot(projectCore, referencedProjects, sourceFiles) 
            |> FSharpProjectSnapshot 
            |> WorkspaceNodeValue.ProjectSnapshot)) 
            |> ignore

    member _.GetSnapshotForFile(file: Uri) =

        depGraph.GetDependentsOf(WorkspaceNodeKey.SourceFile file.LocalPath)

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Seq.tryHead // For now just get the first one

        |> Option.map depGraph.GetValue             
        |> Option.map _.UnwrapProjectSnapshot()


