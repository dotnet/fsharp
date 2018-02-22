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
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
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
                                let documentIdsFiltered = documentIds |> Seq.filter workspace.IsDocumentOpen |> Seq.toArray
                                for documentId in documentIdsFiltered do
                                    Trace.TraceInformation("{0:n3} Requesting Roslyn reanalysis of {1}", DateTime.Now.TimeOfDay.TotalSeconds, documentId)
                                if documentIdsFiltered.Length > 0 then 
                                    analyzerService.Reanalyze(workspace,documentIds=documentIdsFiltered)
                        | _ -> ()
                    with ex -> 
                        Assert.Exception(ex)
                } |> Async.StartImmediate
            )
            checker

    member this.Checker = checker.Value


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
    let projectOptionsTable = FSharpProjectOptionsTable()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpParsingOptions * FSharpProjectOptions>()

    let tryGetOrCreateProjectId (projectFileName:string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    /// Retrieve the projectOptionsTable
    member __.FSharpOptions = projectOptionsTable

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) = projectOptionsTable.ClearInfoForProject(projectId)

    /// Clear a project from the single file project table
    member this.ClearInfoForSingleFileProject(projectId) =
        singleFileProjectTable.TryRemove(projectId) |> ignore

    /// Update a project in the single file project table
    member this.AddOrUpdateSingleFileProject(projectId, data) = singleFileProjectTable.[projectId] <- data

    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, fileContents) =
        async {
            let extraProjectInfo = Some(box workspace)
            let tryGetOptionsForReferencedProject f = f |> tryGetOrCreateProjectId |> Option.bind this.TryGetOptionsForProject |> Option.map(fun (_, _, projectOptions) -> projectOptions)
            if SourceFile.MustBeSingleFileProject(fileName) then 
                // NOTE: we don't use a unique stamp for single files, instead comparing options structurally.
                // This is because we repeatedly recompute the options.
                let optionsStamp = None 
                let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo, ?optionsStamp=optionsStamp) 
                // NOTE: we don't use FCS cross-project references from scripts to projects.  THe projects must have been
                // compiled and #r will refer to files on disk
                let referencedProjectFileNames = [| |] 
                let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, referencedProjectFileNames, options)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, options.ExtraProjectInfo, Some projectOptionsTable, true)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
            else
                let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, extraProjectInfo, Some projectOptionsTable, true)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
        }

    /// Update the info for a project in the project table
    member this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId, site, userOpName, invalidateConfig) =
        projectOptionsTable.AddOrUpdateProject(projectId, (fun isRefresh ->
            let extraProjectInfo = Some(box workspace)
            let tryGetOptionsForReferencedProject f = f |> tryGetOrCreateProjectId |> Option.bind this.TryGetOptionsForProject |> Option.map(fun (_, _, projectOptions) -> projectOptions)
            let referencedProjects, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(Settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, site, serviceProvider, (tryGetOrCreateProjectId (site.ProjectFileName)), site.ProjectFileName, extraProjectInfo,  Some projectOptionsTable, true)
            if invalidateConfig then checkerProvider.Checker.InvalidateConfiguration(projectOptions, startBackgroundCompileIfAlreadySeen = not isRefresh, userOpName = userOpName + ".UpdateProjectInfo")
            let referencedProjectIds = referencedProjects |> Array.choose tryGetOrCreateProjectId
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            referencedProjectIds, parsingOptions, Some site, projectOptions))

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document:Document) = 
        let projectOptionsOpt = this.TryGetOptionsForProject(document.Project.Id)  
        let parsingOptions = 
            match projectOptionsOpt with 
            | Some (parsingOptions, _site, _projectOptions) -> parsingOptions
            | _ -> { FSharpParsingOptions.Default with IsInteractive = IsScript document.Name }
        CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions

    /// Try and get the Options for a project 
    member this.TryGetOptionsForProject(projectId:ProjectId) = projectOptionsTable.TryGetOptionsForProject(projectId)

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
                let! _referencedProjectFileNames, parsingOptions, projectOptions = this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, sourceText.ToString())
                this.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))
                return Some (parsingOptions, None, projectOptions)
                with ex -> 
                Assert.Exception(ex)
                return None
            | _ -> return this.TryGetOptionsForProject(projectId)
        }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document:Document) = 
        let projectId = document.Project.Id
        match singleFileProjectTable.TryGetValue(projectId) with 
        | true, (_loadTime, parsingOptions, originalOptions) -> Some (parsingOptions, originalOptions)
        | _ -> this.TryGetOptionsForProject(projectId) |> Option.map(fun (parsingOptions, _, projectOptions) -> parsingOptions, projectOptions)

    /// get a siteprovider
    member this.ProvideProjectSiteProvider(project:Project) = provideProjectSiteProvider(workspace, project, serviceProvider, Some projectOptionsTable)

    /// Tell the checker to update the project info for the specified project id
    member this.UpdateProjectInfoWithProjectId(projectId:ProjectId, userOpName, invalidateConfig) =
        let hier = workspace.GetHierarchy(projectId)
        match hier with
        | null -> ()
        | h when (h.IsCapabilityMatch("CPS")) ->
            let project = workspace.CurrentSolution.GetProject(projectId)
            if not (isNull project) then
                let siteProvider = this.ProvideProjectSiteProvider(project)
                let projectSite = siteProvider.GetProjectSite()
                if projectSite.CompilationSourceFiles.Length <> 0 then
                    this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId, projectSite, userOpName, invalidateConfig)
        | _ -> ()

    /// Tell the checker to update the project info for the specified project id
    member this.UpdateDocumentInfoWithProjectId(projectId:ProjectId, documentId:DocumentId, userOpName, invalidateConfig) =
        if workspace.IsDocumentOpen(documentId) then
            this.UpdateProjectInfoWithProjectId(projectId, userOpName, invalidateConfig)

    [<Export>]
    /// This handles commandline change notifications from the Dotnet Project-system
    member this.HandleCommandLineChanges(path:string, sources:ImmutableArray<CommandLineSourceFile>, references:ImmutableArray<CommandLineReference>, options:ImmutableArray<string>) =
        let fullPath p =
            if Path.IsPathRooted(p) then p
            else Path.Combine(Path.GetDirectoryName(path), p)
        let sourcePaths = sources |> Seq.map(fun s -> fullPath s.Path) |> Seq.toArray
        let referencePaths = references |> Seq.map(fun r -> fullPath r.Reference) |> Seq.toArray
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(path, projectDisplayNameOf path)
        projectOptionsTable.SetOptionsWithProjectId(projectId, sourcePaths, referencePaths, options.ToArray())
        this.UpdateProjectInfoWithProjectId(projectId, "HandleCommandLineChanges", invalidateConfig=true)

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

    member private this.OnProjectAdded(projectId:ProjectId) = projectInfoManager.UpdateProjectInfoWithProjectId(projectId, "OnProjectAdded", invalidateConfig=true)
    member private this.OnProjectReloaded(projectId:ProjectId) = projectInfoManager.UpdateProjectInfoWithProjectId(projectId, "OnProjectReloaded", invalidateConfig=true)
    member private this.OnDocumentAdded(projectId:ProjectId, documentId:DocumentId) = projectInfoManager.UpdateDocumentInfoWithProjectId(projectId, documentId, "OnDocumentAdded", invalidateConfig=true)
    member private this.OnDocumentChanged(projectId:ProjectId, documentId:DocumentId) = projectInfoManager.UpdateDocumentInfoWithProjectId(projectId, documentId, "OnDocumentChanged", invalidateConfig=false)
    member private this.OnDocumentReloaded(projectId:ProjectId, documentId:DocumentId) = projectInfoManager.UpdateDocumentInfoWithProjectId(projectId, documentId, "OnDocumentReloaded", invalidateConfig=true)

    override this.Initialize() = 
        base.Initialize()


        let workspaceChanged (args:WorkspaceChangeEventArgs) =
            match args.Kind with
            | WorkspaceChangeKind.ProjectAdded     -> this.OnProjectAdded(args.ProjectId)
            | WorkspaceChangeKind.ProjectReloaded  -> this.OnProjectReloaded(args.ProjectId)
            | WorkspaceChangeKind.DocumentAdded    -> this.OnDocumentAdded(args.ProjectId, args.DocumentId)
            | WorkspaceChangeKind.DocumentChanged  -> this.OnDocumentChanged(args.ProjectId, args.DocumentId)
            | WorkspaceChangeKind.DocumentReloaded -> this.OnDocumentReloaded(args.ProjectId, args.DocumentId)
            | WorkspaceChangeKind.DocumentRemoved
            | WorkspaceChangeKind.ProjectRemoved
            | WorkspaceChangeKind.AdditionalDocumentAdded
            | WorkspaceChangeKind.AdditionalDocumentReloaded
            | WorkspaceChangeKind.AdditionalDocumentRemoved
            | WorkspaceChangeKind.AdditionalDocumentChanged
            | WorkspaceChangeKind.DocumentInfoChanged
            | WorkspaceChangeKind.DocumentChanged
            | WorkspaceChangeKind.SolutionAdded
            | WorkspaceChangeKind.SolutionChanged
            | WorkspaceChangeKind.SolutionReloaded
            | WorkspaceChangeKind.SolutionCleared
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
            projectInfoManager.UpdateProjectInfo(tryGetOrCreateProjectId workspace, project.Id, site, userOpName + ".SyncProject", invalidateConfig=true)

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
                Debug.Assert(not (isNull hierarchy), "About to CreateProjectContext with a non-hierarchy site")

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

                for referencedSite in ProjectSitesAndFiles.GetReferencedProjectSites(site, this.SystemServiceProvider, Some (this.Workspace :>obj), Some projectInfoManager.FSharpOptions ) do
                    setup referencedSite

        setup (siteProvider.GetProjectSite()) 

    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =
        let loadTime = DateTime.Now
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName

        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        let _referencedProjectFileNames, parsingOptions, projectOptions = projectInfoManager.ComputeSingleFileOptions (tryGetOrCreateProjectId workspace, fileName, loadTime, fileContents) |> Async.RunSynchronously
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
                | h when not (IsScript(filename)) ->
                    let docId = this.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(filename).FirstOrDefault()
                    match docId with
                    | null ->
                        if not (h.IsCapabilityMatch("CPS")) then
                            let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                            this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)
                    | id ->
                        projectInfoManager.UpdateProjectInfoWithProjectId(id.ProjectId, "SetupNewTextView", invalidateConfig=true)
                | _ ->
                    let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                    this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)
            | _ -> ()
        | _ -> ()
