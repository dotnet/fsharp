module Tests.ProjectInfoHelpers

open System.IO
open FSharp.Compiler.CodeAnalysis
open Ionide.ProjInfo
open Ionide.ProjInfo.Types
open Xunit.Abstractions

// Source code taken from https://github.com/ionide/proj-info/blob/main/src/Ionide.ProjInfo.FCS/Library.fs

let private loadFromDotnetDll (p: ProjectOptions) =
    /// because only a successful compilation will be written to a DLL, we can rely on
    /// the file metadata for things like write times
    let projectFile = FileInfo p.TargetPath

    let getStamp () = projectFile.LastWriteTimeUtc

    let getStream (_ctok: System.Threading.CancellationToken) =
        projectFile.OpenRead() :> Stream |> Some

    FSharpReferencedProject.CreatePortableExecutable(p.TargetPath, getStamp, getStream)

let private makeFCSOptions mapProjectToReference (project: ProjectOptions) =
    {
        ProjectId = None
        ProjectFileName = project.ProjectFileName
        SourceFiles = List.toArray project.SourceFiles
        OtherOptions = List.toArray project.OtherOptions
        ReferencedProjects = project.ReferencedProjects |> List.toArray |> Array.choose mapProjectToReference
        IsIncompleteTypeCheckEnvironment = false
        UseScriptResolutionRules = false
        LoadTime = project.LoadTime
        UnresolvedReferences = None // it's always None
        OriginalLoadReferences = [] // it's always empty list
        Stamp = None
    }

let rec private makeProjectReference isKnownProject makeFSharpProjectReference (p: ProjectReference) : FSharpReferencedProject option =
    let knownProject = isKnownProject p

    let isDotnetProject (knownProject: ProjectOptions option) =
        match knownProject with
        | Some p ->
            (p.ProjectFileName.EndsWith(".csproj") || p.ProjectFileName.EndsWith(".vbproj"))
            && File.Exists p.TargetPath
        | None -> false

    if p.ProjectFileName.EndsWith ".fsproj" then
        knownProject
        |> Option.map (fun p ->
            let theseOptions = makeFSharpProjectReference p
            FSharpReferencedProject.CreateFSharp(p.TargetPath, theseOptions))
    elif isDotnetProject knownProject then
        knownProject |> Option.map loadFromDotnetDll
    else
        None

let private mapManyOptions (allKnownProjects: ProjectOptions seq) : FSharpProjectOptions seq =
    seq {
        let dict =
            System.Collections.Concurrent.ConcurrentDictionary<ProjectOptions, FSharpProjectOptions>()

        let isKnownProject (p: ProjectReference) =
            allKnownProjects
            |> Seq.tryFind (fun kp -> kp.ProjectFileName = p.ProjectFileName)

        let rec makeFSharpProjectReference (p: ProjectOptions) =
            let factory = makeProjectReference isKnownProject makeFSharpProjectReference
            dict.GetOrAdd(p, (fun p -> makeFCSOptions factory p))

        for project in allKnownProjects do
            let thisProject =
                dict.GetOrAdd(project, (fun p -> makeFCSOptions (makeProjectReference isKnownProject makeFSharpProjectReference) p))

            yield thisProject
    }

let rec private mapToFSharpProjectOptions (projectOptions: ProjectOptions) (allKnownProjects: ProjectOptions seq) : FSharpProjectOptions =
    let isKnownProject (d: ProjectReference) =
        allKnownProjects |> Seq.tryFind (fun n -> n.ProjectFileName = d.ProjectFileName)

    makeFCSOptions (makeProjectReference isKnownProject (fun p -> mapToFSharpProjectOptions p allKnownProjects)) projectOptions

let private mkFSharpProjectOptionsImpl (logger: ITestOutputHelper option) (fsprojPath: string) : FSharpProjectOptions array =
    let fi = FileInfo(fsprojPath)
    let toolsPath = Init.init fi.Directory None
    let loader = WorkspaceLoader.Create(toolsPath, [])

    use subscription =
        match logger with
        | None ->
            { new System.IDisposable with
                member _.Dispose() = ()
            }
        | Some logger -> loader.Notifications.Subscribe(string >> logger.WriteLine)

    let projects = loader.LoadProjects([ fsprojPath ]) |> mapManyOptions |> Seq.toArray
    projects

let mkFSharpProjectOptions (fsprojPath: string) : FSharpProjectOptions array =
    mkFSharpProjectOptionsImpl None fsprojPath

let mkFSharpProjectOptionsWithLogger (logger: ITestOutputHelper) (fsprojPath: string) : FSharpProjectOptions array =
    mkFSharpProjectOptionsImpl (Some logger) fsprojPath
