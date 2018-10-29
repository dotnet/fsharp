// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.IO
open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open System.Threading
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList

type private CpsStamp = DateTime

[<AutoOpen>]
module private FSharpProjectOptionsHelpers =

    let mapCpsProjectToSite(workspace:VisualStudioWorkspaceImpl, project:Project, serviceProvider:System.IServiceProvider, cpsCommandLineOptions: IDictionary<ProjectId, CpsStamp * string[] * string[] * string[]>) =
        let hier = workspace.GetHierarchy(project.Id)
        let cpsStampOpt, sourcePaths, referencePaths, options =
            match cpsCommandLineOptions.TryGetValue(project.Id) with
            | true, (cpsStamp, sourcePaths, referencePaths, options) -> Some(cpsStamp), sourcePaths, referencePaths, options
            | false, _ -> None, [||], [||], [||]
        cpsStampOpt,
        {
            new IProvideProjectSite with
                member x.GetProjectSite() =
                    let fst (a, _, _) = a
                    let snd (_, b, _) = b
                    let mutable errorReporter = 
                        let reporter = ProjectExternalErrorReporter(project.Id, "FS", serviceProvider)
                        Some(reporter:> IVsLanguageServiceBuildErrorReporter2)

                    {
                        new IProjectSite with
                            member __.Description = project.Name
                            member __.CompilationSourceFiles = sourcePaths
                            member __.CompilationOptions =
                                Array.concat [options; referencePaths |> Array.map(fun r -> "-r:" + r)]
                            member __.CompilationReferences = referencePaths
                            member site.CompilationBinOutputPath = site.CompilationOptions |> Array.tryPick (fun s -> if s.StartsWith("-o:") then Some s.[3..] else None)
                            member __.ProjectFileName = project.FilePath
                            member __.AdviseProjectSiteChanges(_,_) = ()
                            member __.AdviseProjectSiteCleaned(_,_) = ()
                            member __.AdviseProjectSiteClosed(_,_) = ()
                            member __.IsIncompleteTypeCheckEnvironment = false
                            member __.TargetFrameworkMoniker = ""
                            member __.ProjectGuid =  project.Id.Id.ToString()
                            member __.LoadTime = System.DateTime.Now
                            member __.ProjectProvider = Some (x)
                            member __.BuildErrorReporter with get () = errorReporter and set (v) = errorReporter <- v
                    }
            interface IVsHierarchy with
                member __.SetSite(psp) = hier.SetSite(psp)
                member __.GetSite(psp) = hier.GetSite(ref psp)
                member __.QueryClose(pfCanClose)= hier.QueryClose(ref pfCanClose)
                member __.Close() = hier.Close()
                member __.GetGuidProperty(itemid, propid, pguid) = hier.GetGuidProperty(itemid, propid, ref pguid)
                member __.SetGuidProperty(itemid, propid, rguid) = hier.SetGuidProperty(itemid, propid, ref rguid)
                member __.GetProperty(itemid, propid, pvar) = hier.GetProperty(itemid, propid, ref pvar) 
                member __.SetProperty(itemid, propid, var)  = hier.SetProperty(itemid, propid, var)
                member __.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = 
                    hier.GetNestedHierarchy(itemid, ref iidHierarchyNested, ref ppHierarchyNested, ref pitemidNested)
                member __.GetCanonicalName(itemid, pbstrName) = hier.GetCanonicalName(itemid, ref pbstrName)
                member __.ParseCanonicalName(pszName, pitemid) = hier.ParseCanonicalName(pszName, ref pitemid)
                member __.Unused0() = hier.Unused0()
                member __.AdviseHierarchyEvents(pEventSink, pdwCookie) = hier.AdviseHierarchyEvents(pEventSink, ref pdwCookie)
                member __.UnadviseHierarchyEvents(dwCookie) = hier.UnadviseHierarchyEvents(dwCookie)
                member __.Unused1() = hier.Unused1()
                member __.Unused2() = hier.Unused2()
                member __.Unused3() = hier.Unused3()
                member __.Unused4() = hier.Unused4()
        }

[<RequireQualifiedAccess>]
type private FSharpProjectOptionsMessage =
    | TryGetOptions of Project * AsyncReplyChannel<(FSharpParsingOptions * FSharpProjectOptions) option>
    | ClearOptions of ProjectId

[<Sealed>]
type private FSharpProjectOptionsReactor (workspace: VisualStudioWorkspaceImpl, settings: EditorOptions, serviceProvider, checkerProvider: FSharpCheckerProvider) =
    let cancellationTokenSource = new CancellationTokenSource()

    // Hack to store command line options from HandleCommandLineChanges
    let cpsCommandLineOptions = new ConcurrentDictionary<ProjectId, DateTime * string[] * string[] * string[]>()

    let cache = Dictionary<ProjectId, CpsStamp option * VersionStamp * FSharpParsingOptions * FSharpProjectOptions>()
    
    let rec tryComputeOptions (project: Project) =
        let projectId = project.Id
        let projectStamp = project.Version
        match cache.TryGetValue(projectId) with
        | false, _ ->

            // Because this code can be kicked off before the hack, HandleCommandLineChanges, occurs,
            //     the command line options will not be available and we should bail if one of the project references does not give us anything.
            let mutable canBail = false

            let referencedProjects =
                if settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences then
                    project.ProjectReferences
                    |> Seq.choose (fun projectReference ->
                        let referenceProject = project.Solution.GetProject(projectReference.ProjectId)
                        let result =
                            tryComputeOptions referenceProject
                            |> Option.map (fun (_, projectOptions) ->
                                (referenceProject.OutputFilePath, projectOptions)
                            )

                        if result.IsNone then
                            canBail <- true

                        result
                    )
                    |> Seq.toArray
                else
                    [||]

            if canBail then
                None
            else

            let hier = workspace.GetHierarchy(projectId)
            let cpsStampOpt, projectSite = 
                match hier with
                // Legacy
                | (:? IProvideProjectSite as provideSite) -> None, provideSite.GetProjectSite()
                // Cps
                | _ -> 
                    let cpsStampOpt, provideSite = mapCpsProjectToSite(workspace, project, serviceProvider, cpsCommandLineOptions)
                    cpsStampOpt, provideSite.GetProjectSite()

            let projectOptions =
                {
                    ProjectFileName = projectSite.ProjectFileName
                    ProjectId = None
                    SourceFiles = projectSite.CompilationSourceFiles
                    OtherOptions = projectSite.CompilationOptions
                    ReferencedProjects = referencedProjects
                    IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
                    UseScriptResolutionRules = SourceFile.MustBeSingleFileProject (Path.GetFileName(project.FilePath))
                    LoadTime = projectSite.LoadTime
                    UnresolvedReferences = None
                    OriginalLoadReferences = []
                    ExtraProjectInfo= None
                    Stamp = Some(int64 <| projectStamp.GetHashCode())
                }

            // This can happen if we didn't receive the callback from HandleCommandLineChanges yet.
            if Array.isEmpty projectOptions.SourceFiles then
                None
            else
                checkerProvider.Checker.InvalidateConfiguration(projectOptions, startBackgroundCompileIfAlreadySeen = true, userOpName = "computeOptions")

                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)

                cache.[projectId] <- (cpsStampOpt, projectStamp, parsingOptions, projectOptions)

                Some(parsingOptions, projectOptions)
  
        | true, (cpsStampOpt, projectStamp2, parsingOptions, projectOptions) ->
            let cpsStampOpt2 =
                match cpsCommandLineOptions.TryGetValue(projectId) with
                | true, (cpsStampOpt2, _, _, _) -> Some(cpsStampOpt2)
                | _ -> None
            if projectStamp <> projectStamp2 || cpsStampOpt <> cpsStampOpt2 then
                cache.Remove(projectId) |> ignore
                tryComputeOptions project
            else
                Some(parsingOptions, projectOptions)

    let loop (agent: MailboxProcessor<FSharpProjectOptionsMessage>) =
        async {
            while true do
                match! agent.Receive() with
                | FSharpProjectOptionsMessage.TryGetOptions(project, reply) ->
                    try
                        reply.Reply(tryComputeOptions project)
                    with
                    | _ ->
                        reply.Reply(None)
                | FSharpProjectOptionsMessage.ClearOptions(projectId) ->
                    cache.Remove(projectId) |> ignore
        }

    let agent = MailboxProcessor.Start((fun agent -> loop agent), cancellationToken = cancellationTokenSource.Token)

    member __.TryGetOptionsByProjectAsync(project) =
        agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.TryGetOptions(project, reply))

    member __.ClearOptionsByProjectId(projectId) =
        agent.Post(FSharpProjectOptionsMessage.ClearOptions(projectId))

    member __.SetCpsCommandLineOptions(projectId, stamp, sourcePaths, referencePaths, options) =
        cpsCommandLineOptions.[projectId] <- (stamp, sourcePaths, referencePaths, options)

    member __.TryGetCachedOptionsByProjectId(projectId) =
        match cache.TryGetValue(projectId) with
        | true, result -> Some(result)
        | _ -> None

    interface IDisposable with
        member __.Dispose() = 
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose() 
            (agent :> IDisposable).Dispose()

/// Exposes FCS FSharpProjectOptions information management as MEF component.
//
// This service allows analyzers to get an appropriate FSharpProjectOptions value for a project or single file.
// It also allows a 'cheaper' route to get the project options relevant to parsing (e.g. the #define values).
// The main entrypoints are TryGetOptionsForDocumentOrProject and TryGetOptionsForEditingDocumentOrProject.
[<Export(typeof<FSharpProjectOptionsManager>); Composition.Shared>]
type internal FSharpProjectOptionsManager
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        [<Import(typeof<VisualStudioWorkspace>)>] workspace: VisualStudioWorkspaceImpl,
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
        settings: EditorOptions
    ) =

    let reactor = new FSharpProjectOptionsReactor(workspace, settings, serviceProvider, checkerProvider)

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpParsingOptions * FSharpProjectOptions>()

    do
        // We need to listen to this event for lifecycle purposes.
        workspace.WorkspaceChanged.Add(fun args ->
            match args.Kind with
            | WorkspaceChangeKind.ProjectRemoved -> reactor.ClearOptionsByProjectId(args.ProjectId)
            | _ -> ()
        )

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) = 
        reactor.ClearOptionsByProjectId(projectId)

    /// Clear a project from the single file project table
    member this.ClearInfoForSingleFileProject(projectId) =
        singleFileProjectTable.TryRemove(projectId) |> ignore

    /// Update a project in the single file project table
    member this.AddOrUpdateSingleFileProject(projectId, data) = singleFileProjectTable.[projectId] <- data

    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, fileContents, solution) =
        async {
            let extraProjectInfo = Some(box workspace)
            if SourceFile.MustBeSingleFileProject(fileName) then 
                // NOTE: we don't use a unique stamp for single files, instead comparing options structurally.
                // This is because we repeatedly recompute the options.
                let optionsStamp = None 
                let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo, ?optionsStamp=optionsStamp) 
                // NOTE: we don't use FCS cross-project references from scripts to projects.  THe projects must have been
                // compiled and #r will refer to files on disk
                let referencedProjectFileNames = [| |] 
                let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, referencedProjectFileNames, options)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, options.ExtraProjectInfo, solution, None)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
            else
                let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, extraProjectInfo, solution, None)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
        }

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document:Document) = 
        let parsingOptions =
            match reactor.TryGetCachedOptionsByProjectId(document.Project.Id) with
            | Some (_, _, parsingOptions, _) -> parsingOptions
            | _ -> { FSharpParsingOptions.Default with IsInteractive = IsScript document.Name }
        CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions     

    member this.TryGetOptionsByProject(project) =
        reactor.TryGetOptionsByProjectAsync(project)

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) =
        async { 
            let projectId = document.Project.Id

            // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
            // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
            // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
            match singleFileProjectTable.TryGetValue(projectId) with
            | true, (loadTime, _, _) ->
                try
                    let fileName = document.FilePath
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    // NOTE: we don't use FCS cross-project references from scripts to projects.  The projects must have been
                    // compiled and #r will refer to files on disk.
                    let tryGetOrCreateProjectId _ = None 
                    let! _referencedProjectFileNames, parsingOptions, projectOptions = this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, sourceText.ToString(), document.Project.Solution)
                    this.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))
                    return Some (parsingOptions, None, projectOptions)
                with ex -> 
                    Assert.Exception(ex)
                    return None
            | _ ->
                match! reactor.TryGetOptionsByProjectAsync(document.Project) with
                | Some(parsingOptions, projectOptions) ->
                    return Some(parsingOptions, None, projectOptions)
                | _ ->
                    return None
        }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document:Document) = 
        let projectId = document.Project.Id
        match singleFileProjectTable.TryGetValue(projectId) with 
        | true, (_loadTime, parsingOptions, originalOptions) -> async { return Some (parsingOptions, originalOptions) }
        | _ -> 
            async {
                let! result = this.TryGetOptionsForDocumentOrProject(document) 
                return result |> Option.map(fun (parsingOptions, _, projectOptions) -> parsingOptions, projectOptions)
            }

    [<Export>]
    /// This handles commandline change notifications from the Dotnet Project-system
    /// Prior to VS 15.7 path contained path to project file, post 15.7 contains target binpath
    /// binpath is more accurate because a project file can have multiple in memory projects based on configuration
    member __.HandleCommandLineChanges(path:string, sources:ImmutableArray<CommandLineSourceFile>, references:ImmutableArray<CommandLineReference>, options:ImmutableArray<string>) =
        use _logBlock = Logger.LogBlock(LogEditorFunctionId.LanguageService_HandleCommandLineArgs)

        let projectId =
            match workspace.ProjectTracker.TryGetProjectByBinPath(path) with
            | true, project -> project.Id
            | false, _ -> workspace.ProjectTracker.GetOrCreateProjectIdForPath(path, projectDisplayNameOf path)
        let project =  workspace.ProjectTracker.GetProject(projectId)
        let path = project.ProjectFilePath
        let fullPath p =
            if Path.IsPathRooted(p) || path = null then p
            else Path.Combine(Path.GetDirectoryName(path), p)
        let sourcePaths = sources |> Seq.map(fun s -> fullPath s.Path) |> Seq.toArray
        let referencePaths = references |> Seq.map(fun r -> fullPath r.Reference) |> Seq.toArray

        reactor.SetCpsCommandLineOptions(projectId, DateTime.UtcNow, sourcePaths, referencePaths, options.ToArray())

    member __.Checker = checkerProvider.Checker
