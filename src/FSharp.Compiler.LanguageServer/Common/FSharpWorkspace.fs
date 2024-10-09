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


type FSharpWorkspace() =
    
    let depGraph = LockOperatedDependencyGraph() :> IDependencyGraph<_, _>

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
        
        let projectIdentifier = FSharpProjectIdentifier (projectPath, outputPath)

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
                {
                         Path = path
                         LastModified = File.GetLastWriteTimeUtc path
                     })
            
        let referencesOnDiskNodes = 
            referencesOnDisk 
            |> Seq.map (fun r -> 
                WorkspaceNodeKey.ReferenceOnDisk r.Path,
                WorkspaceNodeValue.ReferenceOnDisk r )
            |> depGraph.AddList

        let otherOptions =
            compilerArgs
            |> Seq.filter (not << isReference)
            |> Seq.filter (not << isFSharpFile)
            |> Seq.toList

        let projectCore = 
            referencesOnDiskNodes
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

        projectIdentifier 

    member _.AddProjectReferences(project: FSharpProjectIdentifier, references: FSharpProjectIdentifier seq) =
        references
        |> Seq.iter (fun reference ->
            
            let outputPath = reference |> function (FSharpProjectIdentifier (_, outputPath)) -> outputPath

            depGraph.AddDependency(WorkspaceNodeKey.ProjectBase project, dependsOn=WorkspaceNodeKey.ProjectSnapshot reference)
            depGraph.RemoveDependency(WorkspaceNodeKey.ProjectCore project, noLongerDependsOn=WorkspaceNodeKey.ReferenceOnDisk outputPath)
            )
        // TODO: might need to remove the -r: references from the project core; maybe even remove the particular reference on disk from the graph

    member _.GetSnapshotForFile(file: Uri) =

        depGraph.GetDependentsOf(WorkspaceNodeKey.SourceFile file.LocalPath)

        // TODO: eventually we need to deal with choosing the appropriate project here
        // Hopefully we will be able to do it through receiving project context from LSP
        // Otherwise we have to keep track of which project/configuration is active
        |> Seq.tryHead // For now just get the first one

        |> Option.map depGraph.GetValue             
        |> Option.map _.UnwrapProjectSnapshot()
