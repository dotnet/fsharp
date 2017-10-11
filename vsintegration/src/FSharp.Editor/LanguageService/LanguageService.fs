// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Linq
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading

open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.ComponentModelHost

// Exposes FSharpChecker as MEF export
[<Export(typeof<FSharpCheckerProvider>); Composition.Shared>]
type internal FSharpCheckerProvider 
    [<ImportingConstructor>]
    (
        analyzerService: IDiagnosticAnalyzerService
    ) =

    // Enabling this would mean that if devenv.exe goes above 2.3GB we do a one-off downsize of the F# Compiler Service caches
    //let maxMemory = 2300 

    let checker = 
        lazy
            let checker = 
                FSharpChecker.Create(
                    projectCacheSize = Settings.LanguageServicePerformance.ProjectCheckCacheSize, 
                    keepAllBackgroundResolutions = false,
                    (* , MaxMemory = 2300 *) 
                    legacyReferenceResolver=Microsoft.FSharp.Compiler.MSBuildReferenceResolver.Resolver)

            // This is one half of the bridge between the F# background builder and the Roslyn analysis engine.
            // When the F# background builder refreshes the background semantic build context for a file,
            // we request Roslyn to reanalyze that individual file.
            checker.BeforeBackgroundFileCheck.Add(fun (fileName, extraProjectInfo) ->  
               async {
                try 
                    match extraProjectInfo with 
                    | Some (:? Workspace as workspace) -> 
                        let solution = workspace.CurrentSolution
                        let documentIds = solution.GetDocumentIdsWithFilePath(fileName)
                        if not documentIds.IsEmpty then 
                            let docuentIdsFiltered = documentIds |> Seq.filter workspace.IsDocumentOpen |> Seq.toArray
                            for documentId in docuentIdsFiltered do
                                Trace.TraceInformation("{0:n3} Requesting Roslyn reanalysis of {1}", DateTime.Now.TimeOfDay.TotalSeconds, documentId)
                            if docuentIdsFiltered.Length > 0 then 
                                analyzerService.Reanalyze(workspace,documentIds=docuentIdsFiltered)
                    | _ -> ()
                with ex -> 
                    Assert.Exception(ex)
                } |> Async.StartImmediate
            )
            checker

    member this.Checker = checker.Value


/// A value and a function to recompute/refresh the value.  The function is passed a flag indicating if a refresh is happening.
type Refreshable<'T> = 'T * (bool -> 'T)

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
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider
    ) =

    // A table of information about projects, excluding single-file projects.
    let projectTable = ConcurrentDictionary<ProjectId, Refreshable<ProjectId[] * FSharpParsingOptions * FSharpProjectOptions>>()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpParsingOptions * FSharpProjectOptions>()

    // Accumulate sources and references for each project file
    let projectInfo = new ConcurrentDictionary<string, string[]*string[]*string[]>()

    let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let tryGetOrCreateProjectId (projectFileName: string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId: ProjectId) =
        projectTable.TryRemove(projectId) |> ignore
        this.RefreshInfoForProjectsThatReferenceThisProject(projectId)

    member this.ClearInfoForSingleFileProject(projectId) =
        singleFileProjectTable.TryRemove(projectId) |> ignore

    member this.RefreshInfoForProjectsThatReferenceThisProject(projectId: ProjectId) =
        // Search the projectTable for things to refresh
        for KeyValue(otherProjectId, ((referencedProjectIds, _parsingOptions, _options), refresh)) in projectTable.ToArray() do
           for referencedProjectId in referencedProjectIds do
              if referencedProjectId = projectId then 
                  projectTable.[otherProjectId] <- (refresh true, refresh)

    member this.AddOrUpdateProject(projectId, refresh) =
        projectTable.[projectId] <- (refresh false, refresh)
        this.RefreshInfoForProjectsThatReferenceThisProject(projectId)

    member this.AddOrUpdateSingleFileProject(projectId, data) =
        singleFileProjectTable.[projectId] <- data

    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, fileContents, workspace: Workspace) = async {
        let extraProjectInfo = Some(box workspace)
        let tryGetOptionsForReferencedProject f = f |> tryGetOrCreateProjectId |> Option.bind this.TryGetOptionsForProject |> Option.map snd
        if SourceFile.MustBeSingleFileProject(fileName) then 
            // NOTE: we don't use a unique stamp for single files, instead comparing options structurally.
            // This is because we repeatedly recompute the options.
            let optionsStamp = None 
            let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo, ?optionsStamp=optionsStamp) 
            // NOTE: we don't use FCS cross-project references from scripts to projects.  THe projects must have been
            // compiled and #r will refer to files on disk
            let referencedProjectFileNames = [| |] 
            let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, referencedProjectFileNames, options)
            let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject,site,fileName,options.ExtraProjectInfo,serviceProvider, true)
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            return (deps, parsingOptions, projectOptions)
        else
            let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
            let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject,site,fileName,extraProjectInfo,serviceProvider, true)
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            return (deps, parsingOptions, projectOptions)
      }

    /// Update the info for a project in the project table
    member this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId: ProjectId, site: IProjectSite, userOpName) =
        this.AddOrUpdateProject(projectId, (fun isRefresh -> 
            let extraProjectInfo = Some(box workspace)
            let tryGetOptionsForReferencedProject f = f |> tryGetOrCreateProjectId |> Option.bind this.TryGetOptionsForProject |> Option.map snd
            let referencedProjects, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, site, site.ProjectFileName, extraProjectInfo, serviceProvider, true)
            let referencedProjectIds = referencedProjects |> Array.choose tryGetOrCreateProjectId
            checkerProvider.Checker.InvalidateConfiguration(projectOptions, startBackgroundCompileIfAlreadySeen = not isRefresh, userOpName= userOpName + ".UpdateProjectInfo")
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            referencedProjectIds, parsingOptions, projectOptions))
 
    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document: Document) = 
        let projectOptionsOpt = this.TryGetOptionsForProject(document.Project.Id)  
        let parsingOptions = 
            match projectOptionsOpt with 
            | None -> FSharpParsingOptions.Default
            | Some (parsingOptions, _projectOptions) -> parsingOptions
        CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, parsingOptions)

    /// Get the options for a project
    member this.TryGetOptionsForProject(projectId: ProjectId) = 
        match projectTable.TryGetValue(projectId) with
        | true, ((_referencedProjects, parsingOptions, projectOptions), _) -> Some (parsingOptions, projectOptions)
        | _ -> None

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) = async { 
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
            let! _referencedProjectFileNames, parsingOptions, projectOptions = this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, sourceText.ToString(), document.Project.Solution.Workspace)
            this.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))
            return Some (parsingOptions, projectOptions)
          with ex -> 
            Assert.Exception(ex)
            return None
        | _ -> return this.TryGetOptionsForProject(projectId) 
     }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document: Document) = 
        let projectId = document.Project.Id
        match singleFileProjectTable.TryGetValue(projectId) with 
        | true, (_loadTime, parsingOptions, originalOptions) -> Some (parsingOptions, originalOptions)
        | _ -> this.TryGetOptionsForProject(projectId) 

    member this.ProvideProjectSiteProvider(project:Project) =
        let hier = workspace.GetHierarchy(project.Id)

        {new IProvideProjectSite with
            member iProvideProjectSite.GetProjectSite() =
                let fst  (a, _, _) = a
                let thrd (_, _, c) = c
                let mutable errorReporter = 
                    let reporter = ProjectExternalErrorReporter(project.Id, "FS", serviceProvider)
                    Some(reporter:> Microsoft.VisualStudio.Shell.Interop.IVsLanguageServiceBuildErrorReporter2)

                {new Microsoft.VisualStudio.FSharp.LanguageService.IProjectSite with
                    member __.CompilationSourceFiles = this.GetProjectInfo(project.FilePath) |> fst
                    member __.CompilationOptions =
                        let _,references,options = this.GetProjectInfo(project.FilePath)
                        Array.concat [options; references |> Array.map(fun r -> "-r:" + r)]
                    member __.CompilationReferences = this.GetProjectInfo(project.FilePath) |> thrd
                    member site.CompilationBinOutputPath = site.CompilationOptions |> Array.tryPick (fun s -> if s.StartsWith("-o:") then Some s.[3..] else None)
                    member __.Description = project.Name
                    member __.ProjectFileName = project.FilePath
                    member __.AdviseProjectSiteChanges(_,_) = ()
                    member __.AdviseProjectSiteCleaned(_,_) = ()
                    member __.AdviseProjectSiteClosed(_,_) = ()
                    member __.IsIncompleteTypeCheckEnvironment = false
                    member __.TargetFrameworkMoniker = ""
                    member __.ProjectGuid = project.Id.Id.ToString()
                    member __.LoadTime = System.DateTime.Now
                    member __.ProjectProvider = Some iProvideProjectSite
                    member __.BuildErrorReporter with get () = errorReporter and 
                                                      set (v) = errorReporter <- v
                }

        // TODO:   figure out why this is necessary
        interface IVsHierarchy with
            member __.SetSite(psp)                                    = hier.SetSite(psp)
            member __.GetSite(psp)                                    = hier.GetSite(ref psp)
            member __.QueryClose(pfCanClose)                          = hier.QueryClose(ref pfCanClose)
            member __.Close()                                         = hier.Close()
            member __.GetGuidProperty(itemid, propid, pguid)          = hier.GetGuidProperty(itemid, propid, ref pguid)
            member __.SetGuidProperty(itemid, propid, rguid)          = hier.SetGuidProperty(itemid, propid, ref rguid)
            member __.GetProperty(itemid, propid, pvar)               = hier.GetProperty(itemid, propid, ref pvar) 
            member __.SetProperty(itemid, propid, var)                = hier.SetProperty(itemid, propid, var)
            member __.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = hier.GetNestedHierarchy(itemid, ref iidHierarchyNested, ref ppHierarchyNested, ref pitemidNested)
            member __.GetCanonicalName(itemid, pbstrName)             = hier.GetCanonicalName(itemid, ref pbstrName)
            member __.ParseCanonicalName(pszName, pitemid)            = hier.ParseCanonicalName(pszName, ref pitemid)
            member __.Unused0()                                       = hier.Unused0()
            member __.AdviseHierarchyEvents(pEventSink, pdwCookie)    = hier.AdviseHierarchyEvents(pEventSink, ref pdwCookie)
            member __.UnadviseHierarchyEvents(dwCookie)               = hier.UnadviseHierarchyEvents(dwCookie)
            member __.Unused1()                                       = hier.Unused1()
            member __.Unused2()                                       = hier.Unused2()
            member __.Unused3()                                       = hier.Unused3()
            member __.Unused4()                                       = hier.Unused4()
        }

    member this.UpdateProjectInfoWithProjectId(projectId:ProjectId, userOpName) =
        let hier = workspace.GetHierarchy(projectId)
        match hier with
        | h when (h.IsCapabilityMatch("CPS")) ->
            let project = workspace.CurrentSolution.GetProject(projectId)
            let siteProvider = this.ProvideProjectSiteProvider(project)
            this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId, siteProvider.GetProjectSite(), userOpName)
        | _ -> ()

    member this.UpdateProjectInfoWithPath(path, userOpName) =
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(path, projectDisplayNameOf path)
        this.UpdateProjectInfoWithProjectId(projectId, userOpName)

    [<Export>]
    /// This handles commandline change notifications from the Dotnet Project-system
    member this.HandleCommandLineChanges(path:string, sources:ImmutableArray<CommandLineSourceFile>, references:ImmutableArray<CommandLineReference>, options:ImmutableArray<string>) =
        let fullPath p =
            if Path.IsPathRooted(p) then p
            else Path.Combine(Path.GetDirectoryName(path), p)
        let sourcePaths = sources |> Seq.map(fun s -> fullPath s.Path) |> Seq.toArray
        let referencePaths = references |> Seq.map(fun r -> fullPath r.Reference) |> Seq.toArray
        projectInfo.[path] <- (sourcePaths,referencePaths,options.ToArray())
        this.UpdateProjectInfoWithPath(path, "HandleCommandLineChanges")

    member __.GetProjectInfo(path:string) = 
        match projectInfo.TryGetValue path with
        | true, value -> value
        | _ -> [||], [||], [||]

    member __.Checker = checkerProvider.Checker

// Used to expose FSharpChecker/ProjectInfo manager to diagnostic providers
// Diagnostic providers can be executed in environment that does not use MEF so they can rely only
// on services exposed by the workspace
type internal FSharpCheckerWorkspaceService =
    inherit Microsoft.CodeAnalysis.Host.IWorkspaceService
    abstract Checker: FSharpChecker
    abstract FSharpProjectOptionsManager: FSharpProjectOptionsManager

type internal RoamingProfileStorageLocation(keyName: string) =
    inherit OptionStorageLocation()
    
    member __.GetKeyNameForLanguage(languageName: string) =
        let unsubstitutedKeyName = keyName
        match languageName with
        | null -> unsubstitutedKeyName
        | _ ->
            let substituteLanguageName = if languageName = FSharpConstants.FSharpLanguageName then "FSharp" else languageName
            unsubstitutedKeyName.Replace("%LANGUAGE%", substituteLanguageName)
 
[<Composition.Shared>]
[<Microsoft.CodeAnalysis.Host.Mef.ExportWorkspaceServiceFactory(typeof<FSharpCheckerWorkspaceService>, Microsoft.CodeAnalysis.Host.Mef.ServiceLayer.Default)>]
type internal FSharpCheckerWorkspaceServiceFactory
    [<Composition.ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_workspaceServices) =
            upcast { new FSharpCheckerWorkspaceService with
                member this.Checker = checkerProvider.Checker
                member this.FSharpProjectOptionsManager = projectInfoManager }

type
    [<Guid(FSharpConstants.packageGuidString)>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.IntelliSenseOptionPage>, "F#", null, "IntelliSense", "6008")>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.QuickInfoOptionPage>, "F#", null, "QuickInfo", "6009")>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.CodeFixesOptionPage>, "F#", null, "Code Fixes", "6010")>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.LanguageServicePerformanceOptionPage>, "F#", null, "Performance", "6011")>]
    [<ProvideLanguageService(languageService = typeof<FSharpLanguageService>,
                             strLanguageName = FSharpConstants.FSharpLanguageName,
                             languageResourceID = 100,
                             MatchBraces = true,
                             MatchBracesAtCaret = true,
                             ShowCompletion = true,
                             ShowMatchingBrace = true,
                             ShowSmartIndent = true,
                             EnableAsyncCompletion = true,
                             QuickInfo = true,
                             DefaultToInsertSpaces = true,
                             CodeSense = true,
                             DefaultToNonHotURLs = true,
                             RequestStockColors = true,
                             EnableCommenting = true,
                             CodeSenseDelay = 100,
                             ShowDropDownOptions = true)>]
    internal FSharpPackage() =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()

    override this.Initialize() =
        base.Initialize()
        this.ComponentModel.GetService<SettingsPersistence.ISettings>() |> ignore

    override this.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
    override this.CreateLanguageService() = FSharpLanguageService(this)
    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>
    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()

type
    [<Guid(FSharpConstants.languageServiceGuidString)>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fs")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsi")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsx")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsscript")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".ml")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".mli")>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".fs", 97)>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".fsi", 97)>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".fsx", 97)>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".fsscript", 97)>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".ml", 97)>]
    [<ProvideEditorExtension(FSharpConstants.editorFactoryGuidString, ".mli", 97)>]
    internal FSharpLanguageService(package : FSharpPackage) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<FSharpProjectOptionsManager>().Value

    let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let singleFileProjects = ConcurrentDictionary<_, AbstractProject>()

    let tryRemoveSingleFileProject projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.ClearInfoForSingleFileProject(projectId)
            project.Disconnect()
        | _ -> ()

    let invalidPathChars = set (Path.GetInvalidPathChars())
    let isPathWellFormed (path: string) = not (String.IsNullOrWhiteSpace path) && path |> Seq.forall (fun c -> not (Set.contains c invalidPathChars))

    let tryGetOrCreateProjectId (workspace: VisualStudioWorkspaceImpl) (projectFileName: string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    let optionsAssociation = ConditionalWeakTable<IWorkspaceProjectContext, string[]>()

    member private this.OnProjectAdded(projectId:ProjectId, _newSolution:Solution) = projectInfoManager.UpdateProjectInfoWithProjectId(projectId, "OnProjectAdded")
    override this.Initialize() =
        base.Initialize()

        let workspaceChanged (args:WorkspaceChangeEventArgs) =
            match args.Kind with
            | WorkspaceChangeKind.ProjectAdded   -> this.OnProjectAdded(args.ProjectId,   args.NewSolution)
            | _ -> ()

        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)
        this.Workspace.WorkspaceChanged.Add(workspaceChanged)
        this.Workspace.DocumentClosed.Add <| fun args -> tryRemoveSingleFileProject args.Document.Project.Id

        Events.SolutionEvents.OnAfterCloseSolution.Add <| fun _ ->
            //checkerProvider.Checker.StopBackgroundCompile()

            // FUTURE: consider enbling some or all of these to flush all caches and stop all background builds. However the operations
            // are asynchronous and we need to decide if we stop everything synchronously.

            //checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            //checkerProvider.Checker.InvalidateAll()

            singleFileProjects.Keys |> Seq.iter tryRemoveSingleFileProject

        let ctx = System.Threading.SynchronizationContext.Current

        let rec setupProjectsAfterSolutionOpen() =
            async {
                use openedProjects = MailboxProcessor.Start <| fun inbox ->
                    async { 
                        // waits for AfterOpenSolution and then starts projects setup
                        do! Async.AwaitEvent Events.SolutionEvents.OnAfterOpenSolution |> Async.Ignore
                        while true do
                            let! siteProvider = inbox.Receive()
                            do! Async.SwitchToContext ctx
                            this.SetupProjectFile(siteProvider, this.Workspace, "SetupProjectsAfterSolutionOpen")
                            do! Async.SwitchToThreadPool()
                    }

                use _ = Events.SolutionEvents.OnAfterOpenProject |> Observable.subscribe ( fun args ->
                    match args.Hierarchy with
                    | :? IProvideProjectSite as siteProvider -> openedProjects.Post(siteProvider)
                    | _ -> () )

                do! Async.AwaitEvent Events.SolutionEvents.OnAfterCloseSolution |> Async.Ignore
                do! setupProjectsAfterSolutionOpen() 
            }
        setupProjectsAfterSolutionOpen() |> Async.StartImmediate

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors()
        
    /// Sync the Roslyn information for the project held in 'projectContext' to match the information given by 'site'.
    /// Also sync the info in ProjectInfoManager if necessary.
    member this.SyncProject(project: AbstractProject, projectContext: IWorkspaceProjectContext, site: IProjectSite, workspace, forceUpdate, userOpName) =
        let wellFormedFilePathSetIgnoreCase (paths: seq<string>) =
            HashSet(paths |> Seq.filter isPathWellFormed |> Seq.map (fun s -> try Path.GetFullPath(s) with _ -> s), StringComparer.OrdinalIgnoreCase)

        let mutable updated = forceUpdate

        // Sync the source files in projectContext.  Note that these source files are __not__ maintained in order in projectContext
        // as edits are made. It seems this is ok because the source file list is only used to drive roslyn per-file checking.
        let updatedFiles = site.CompilationSourceFiles |> wellFormedFilePathSetIgnoreCase
        let originalFiles = project.GetCurrentDocuments() |> Seq.map (fun file -> file.FilePath) |> wellFormedFilePathSetIgnoreCase
        
        for file in updatedFiles do
            if not(originalFiles.Contains(file)) then
                projectContext.AddSourceFile(file)
                updated <- true

        for file in originalFiles do
            if not(updatedFiles.Contains(file)) then
                projectContext.RemoveSourceFile(file)
                updated <- true

        let updatedRefs = site.CompilationReferences |> wellFormedFilePathSetIgnoreCase
        let originalRefs = project.GetCurrentMetadataReferences() |> Seq.map (fun ref -> ref.FilePath) |> wellFormedFilePathSetIgnoreCase

        for ref in updatedRefs do
            if not(originalRefs.Contains(ref)) then
                projectContext.AddMetadataReference(ref, MetadataReferenceProperties.Assembly)
                updated <- true

        for ref in originalRefs do
            if not(updatedRefs.Contains(ref)) then
                projectContext.RemoveMetadataReference(ref)
                updated <- true

        // Update the project options association
        let ok,originalOptions = optionsAssociation.TryGetValue(projectContext)
        let updatedOptions = site.CompilationOptions
        if not ok || originalOptions <> updatedOptions then 

            // OK, project options have changed, try to fake out Roslyn to convince it to reparse things.
            // Calling SetOptions fails because the CPS project system being used by the F# project system 
            // imlpementation at the moment has no command line parser installed, so we remove/add all the files 
            // instead.  A change of flags doesn't happen very often and the remove/add is fast in any case.
            //projectContext.SetOptions(String.concat " " updatedOptions)
            for file in updatedFiles do
                projectContext.RemoveSourceFile(file)
                projectContext.AddSourceFile(file)

            // Record the last seen options as an associated value
            if ok then optionsAssociation.Remove(projectContext) |> ignore
            optionsAssociation.Add(projectContext, updatedOptions)

            updated <- true

        // update the cached options
        if updated then
            projectInfoManager.UpdateProjectInfo(tryGetOrCreateProjectId workspace, project.Id, site, userOpName + ".SyncProject")

    member this.SetupProjectFile(siteProvider: IProvideProjectSite, workspace: VisualStudioWorkspaceImpl, userOpName) =
        let userOpName = userOpName + ".SetupProjectFile"
        let  rec setup (site: IProjectSite) =
            let projectGuid = Guid(site.ProjectGuid)
            let projectFileName = site.ProjectFileName
            let projectDisplayName = projectDisplayNameOf projectFileName

            let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            if isNull (workspace.ProjectTracker.GetProject projectId) then
                let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
                let errorReporter = ProjectExternalErrorReporter(projectId, "FS", this.SystemServiceProvider)

                let hierarchy =
                    site.ProjectProvider
                    |> Option.map (fun p -> p :?> IVsHierarchy)
                    |> Option.toObj

                // Roslyn is expecting site to be an IVsHierarchy.
                // It just so happens that the object that implements IProvideProjectSite is also
                // an IVsHierarchy. This assertion is to ensure that the assumption holds true.
                Debug.Assert(hierarchy <> null, "About to CreateProjectContext with a non-hierarchy site")

                let projectContext = 
                    projectContextFactory.CreateProjectContext(
                        FSharpConstants.FSharpLanguageName,
                        projectDisplayName,
                        projectFileName,
                        projectGuid,
                        hierarchy,
                        Option.toObj site.CompilationBinOutputPath,
                        errorReporter)

                let project = projectContext :?> AbstractProject

                // Sync IProjectSite --> projectContext, and IProjectSite --> ProjectInfoManage
                this.SyncProject(project, projectContext, site, workspace, forceUpdate=true, userOpName=userOpName)

                site.BuildErrorReporter <- Some (errorReporter :> Microsoft.VisualStudio.Shell.Interop.IVsLanguageServiceBuildErrorReporter2)

                // TODO: consider forceUpdate = false here.  forceUpdate=true may be causing repeated computation?
                site.AdviseProjectSiteChanges(FSharpConstants.FSharpLanguageServiceCallbackName, 
                                              AdviseProjectSiteChanges(fun () -> this.SyncProject(project, projectContext, site, workspace, forceUpdate=true, userOpName="AdviseProjectSiteChanges."+userOpName)))

                site.AdviseProjectSiteClosed(FSharpConstants.FSharpLanguageServiceCallbackName, 
                                             AdviseProjectSiteChanges(fun () -> 
                                                projectInfoManager.ClearInfoForProject(project.Id)
                                                optionsAssociation.Remove(projectContext) |> ignore
                                                project.Disconnect()))

                for referencedSite in ProjectSitesAndFiles.GetReferencedProjectSites (site, this.SystemServiceProvider) do
                    setup referencedSite

        setup (siteProvider.GetProjectSite()) 

    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =
        let loadTime = DateTime.Now
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName

        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        let _referencedProjectFileNames, parsingOptions, projectOptions = projectInfoManager.ComputeSingleFileOptions (tryGetOrCreateProjectId workspace, fileName, loadTime, fileContents, workspace) |> Async.RunSynchronously
        projectInfoManager.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))

        if isNull (workspace.ProjectTracker.GetProject projectId) then
            let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
            let errorReporter = ProjectExternalErrorReporter(projectId, "FS", this.SystemServiceProvider)

            let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hier, null, errorReporter)
            projectContext.AddSourceFile(fileName)
            
            let project = projectContext :?> AbstractProject
            singleFileProjects.[projectId] <- project

    override this.ContentTypeName = FSharpConstants.FSharpContentTypeName
    override this.LanguageName = FSharpConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSharpConstants.languageServiceGuidString)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(textView) =
        base.SetupNewTextView(textView)

        let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()

        match textView.GetBuffer() with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines

            // CPS projects don't implement IProvideProjectSite and IVSProjectHierarchy
            // Simple explanation:
            //    Legacy projects have IVSHierarchy and IPRojectSite
            //    CPS Projects and loose script files don't
            match VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable,filename) with
            | Some (hier, _) ->
                match hier with
                | :? IProvideProjectSite as siteProvider when not (IsScript(filename)) ->
                    this.SetupProjectFile(siteProvider, this.Workspace, "SetupNewTextView")
                | _ when not (IsScript(filename)) ->
                    let docId = this.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(filename).FirstOrDefault()
                    match docId with
                    | null ->
                        let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                        this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)
                    | id ->
                        projectInfoManager.UpdateProjectInfoWithProjectId(id.ProjectId, "SetupNewTextView")
                | _ ->
                    let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                    this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)
            | _ -> ()
        | _ -> ()
