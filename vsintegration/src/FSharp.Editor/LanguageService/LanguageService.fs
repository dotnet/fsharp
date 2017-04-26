// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.IO
open System.Diagnostics

open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.TextManager.Interop
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
    let checker = 
        lazy
            let checker = FSharpChecker.Create(projectCacheSize = 200, keepAllBackgroundResolutions = false)

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
                                analyzerService.Reanalyze(workspace, documentIds=documentIds, highPriority=true)
                        | _ -> ()
                    with ex -> 
                        Assert.Exception(ex)
                } |> Async.StartImmediate
            )

            checker

    member this.Checker = checker.Value

// Exposes project information as MEF component
[<Export(typeof<ProjectInfoManager>); Composition.Shared>]
type internal ProjectInfoManager 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider
    ) =
    // A table of information about projects, excluding single-file projects.  
    let projectTable = ConcurrentDictionary<ProjectId, FSharpProjectOptions>()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpProjectOptions>()

    member this.AddSingleFileProject(projectId, timeStampAndOptions) =
        singleFileProjectTable.TryAdd(projectId, timeStampAndOptions) |> ignore

    member this.RemoveSingleFileProject(projectId) =
        singleFileProjectTable.TryRemove(projectId) |> ignore

    /// Clear a project from the project table
    member this.ClearProjectInfo(projectId: ProjectId) =
        projectTable.TryRemove(projectId) |> ignore
        
    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace: Workspace) = async {
        let extraProjectInfo = Some(box workspace)
        if SourceFile.MustBeSingleFileProject(fileName) then 
            let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, options)
            return ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site,fileName,options.ExtraProjectInfo,serviceProvider)
        else
            let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
            return ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site,fileName,extraProjectInfo,serviceProvider)
      }

    /// Update the info for a project in the project table
    member this.UpdateProjectInfo(projectId: ProjectId, site: IProjectSite, workspace: Workspace) =
        let extraProjectInfo = Some(box workspace)
        let options = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site, site.ProjectFileName(), extraProjectInfo, serviceProvider)
        checkerProvider.Checker.InvalidateConfiguration(options)
        projectTable.[projectId] <- options

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document: Document) = 
        let projectOptionsOpt = this.TryGetOptionsForProject(document.Project.Id)  
        let otherOptions = 
            match projectOptionsOpt with 
            | None -> []
            | Some options -> options.OtherOptions |> Array.toList
        CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, otherOptions)

    /// Get the options for a project
    member this.TryGetOptionsForProject(projectId: ProjectId) = 
        match projectTable.TryGetValue(projectId) with
        | true, options -> Some options 
        | _ -> None

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) = async { 
        let projectId = document.Project.Id
        
        // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
        // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
        // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
        match singleFileProjectTable.TryGetValue(projectId) with
        | true, (loadTime,_) ->
          try
            let fileName = document.FilePath
            let! cancellationToken = Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! options = this.ComputeSingleFileOptions (fileName, loadTime, sourceText.ToString(), document.Project.Solution.Workspace)
            singleFileProjectTable.[projectId] <- (loadTime, options)
            return Some options
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
        | true, (_loadTime, originalOptions) -> Some originalOptions
        | _ -> this.TryGetOptionsForProject(projectId) 

// Used to expose FSharpChecker/ProjectInfo manager to diagnostic providers
// Diagnostic providers can be executed in environment that does not use MEF so they can rely only
// on services exposed by the workspace
type internal FSharpCheckerWorkspaceService =
    inherit Microsoft.CodeAnalysis.Host.IWorkspaceService
    abstract Checker: FSharpChecker
    abstract ProjectInfoManager: ProjectInfoManager

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
        projectInfoManager: ProjectInfoManager
    ) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_workspaceServices) =
            upcast { new FSharpCheckerWorkspaceService with
                member this.Checker = checkerProvider.Checker
                member this.ProjectInfoManager = projectInfoManager }
                
type
    [<Guid(FSharpConstants.packageGuidString)>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.IntelliSenseOptionPage>, "F#", null, "IntelliSense", "6008")>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.QuickInfoOptionPage>, "F#", null, "QuickInfo", "6009")>]
    [<ProvideLanguageEditorOptionPage(typeof<OptionsUI.CodeFixesOptionPage>, "F#", null, "Code Fixes", "6010")>]
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
                             EnableCommenting = true,
                             CodeSenseDelay = 0,
                             ShowDropDownOptions = true)>]
    internal FSharpPackage() =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()

    override this.Initialize() =
        base.Initialize()
        //initialize settings
        this.ComponentModel.GetService<SettingsPersistence.ISettings>() |> ignore

    override this.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

    override this.CreateLanguageService() = 
        FSharpLanguageService(this)        

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()
    
and 
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

    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<ProjectInfoManager>().Value
    let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()
    let runningDocumentTable = package.GetService<SVsRunningDocumentTable>() :?> IVsRunningDocumentTable4

    let projectDisplayNameOf projectFileName = 
        if String.IsNullOrWhiteSpace projectFileName
        then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName
    
    let projectContexts = ConcurrentDictionary<ProjectId, IWorkspaceProjectContext>()
    let singleFileProjects = ConcurrentDictionary<ProjectId, AbstractProject>()

    let tryRemoveSingleFileProjectById projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.RemoveSingleFileProject(projectId)
            project.Disconnect()
        | _ ->
            ()
    
    let tryRemoveSingleFileProjectByName fileName =
        singleFileProjects
        |> Seq.tryFind (fun (KeyValue(_, project)) ->
            project.GetCurrentDocuments()
            |> Seq.exists (fun doc -> doc.FilePath.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            )
        |> Option.iter (fun (KeyValue(id, _)) ->
            tryRemoveSingleFileProjectById id
            )
     
    // Ideally we'd move this over to Roslyn's MiscellaneousFilesWorkspace, but we can't do that
    // until 15.3. We can just implement the barebones of what we need with respect to file
    // tracking with the running document table.

    interface IVsRunningDocTableEvents with
        member this.OnAfterAttributeChange(_docCookie: uint32, _grfAttribs: uint32): int = 
            VSConstants.S_OK
        member this.OnAfterDocumentWindowHide(_docCookie: uint32, _pFrame: IVsWindowFrame): int = 
            VSConstants.E_NOTIMPL
        member this.OnAfterFirstDocumentLock(_docCookie: uint32, _dwRDTLockType: uint32, _dwReadLocksRemaining: uint32, _dwEditLocksRemaining: uint32): int = 
            VSConstants.E_NOTIMPL
        member this.OnAfterSave(_docCookie: uint32): int = 
            VSConstants.E_NOTIMPL
        member this.OnBeforeDocumentWindowShow(_docCookie: uint32, _fFirstShow: int, _pFrame: IVsWindowFrame): int = 
            VSConstants.E_NOTIMPL
        member this.OnBeforeLastDocumentUnlock(docCookie: uint32, _dwRDTLockType: uint32, dwReadLocksRemaining: uint32, dwEditLocksRemaining: uint32): int = 
            if dwReadLocksRemaining + dwEditLocksRemaining = 0u then
                runningDocumentTable.GetDocumentMoniker docCookie
                |> tryRemoveSingleFileProjectByName
                |> ignore
 
            VSConstants.S_OK

    interface IVsRunningDocTableEvents2 with
        member this.OnAfterAttributeChange(_docCookie: uint32, _grfAttribs: uint32): int = 
            VSConstants.S_OK
        member this.OnAfterDocumentWindowHide(_docCookie: uint32, _pFrame: IVsWindowFrame): int = 
            VSConstants.E_NOTIMPL
        member this.OnAfterFirstDocumentLock(_docCookie: uint32, _dwRDTLockType: uint32, _dwReadLocksRemaining: uint32, _dwEditLocksRemaining: uint32): int = 
            VSConstants.E_NOTIMPL
        member this.OnAfterSave(_docCookie: uint32): int = 
            VSConstants.E_NOTIMPL
        member this.OnBeforeDocumentWindowShow(_docCookie: uint32, _fFirstShow: int, _pFrame: IVsWindowFrame): int = 
            VSConstants.E_NOTIMPL
        member this.OnBeforeLastDocumentUnlock(_docCookie: uint32, _dwRDTLockType: uint32, _dwReadLocksRemaining: uint32, _dwEditLocksRemaining: uint32): int = 
            VSConstants.S_OK
        member this.OnAfterAttributeChangeEx(docCookie: uint32, grfAttribs: uint32, _pHierOld: IVsHierarchy, _itemidOld: uint32, pszMkDocumentOld: string, _pHierNew: IVsHierarchy, _itemidNew: uint32, pszMkDocumentNew: string): int = 
            // Did we rename?
            if grfAttribs &&& uint32 __VSRDTATTRIB.RDTA_MkDocument <> 0u then
                tryRemoveSingleFileProjectByName pszMkDocumentOld
                this.TrackDocument pszMkDocumentNew
            
            // The document is now initialized, we should try tracking it
            if grfAttribs &&& uint32 __VSRDTATTRIB3.RDTA_DocumentInitialized <> 0u then
                runningDocumentTable.GetDocumentMoniker docCookie
                |> this.TrackDocument

            VSConstants.S_OK

    override this.Initialize() =
        base.Initialize()
        
        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)
        
        let cookie = ref 0u
        (runningDocumentTable :?> IVsRunningDocumentTable).AdviseRunningDocTableEvents(this, cookie) |> ignore
            
        // Setup the project with Roslyn workspaces
        Events.SolutionEvents.OnAfterOpenProject.Add <| fun args ->
            match args.Hierarchy with
            | :? IProvideProjectSite as siteProvider ->
                this.SetupProjectFile(siteProvider, this.Workspace)
            | _ -> ()
            
        // Clear any state that we had for projects in the closing solution
        Events.SolutionEvents.OnAfterCloseSolution.Add <| fun _ ->
            singleFileProjects.Keys |> Seq.iter tryRemoveSingleFileProjectById
            projectContexts.Clear()
            ProjectSitesAndFiles.ClearCache()

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors()
        
    /// Sync the information for the project 
    member this.SyncProject(project: AbstractProject, projectContext: IWorkspaceProjectContext, site: IProjectSite, forceUpdate) =
        let hashSetIgnoreCase x = HashSet<string>(x, StringComparer.OrdinalIgnoreCase)
        
        let mutable updated = forceUpdate

        // Synchronise source files
        let updatedFiles = site.SourceFilesOnDisk() |> hashSetIgnoreCase
        let workspaceFiles = project.GetCurrentDocuments() |> Seq.map(fun file -> file.FilePath) |> hashSetIgnoreCase

        for file in updatedFiles do
            if not(workspaceFiles.Contains(file)) then
                projectContext.AddSourceFile(file)
                updated <- true
        for file in workspaceFiles do
            if not(updatedFiles.Contains(file)) then
                projectContext.RemoveSourceFile(file)
                updated <- true
        
        // Synchronise assembly references
        let updatedRefs = site.AssemblyReferences() |> Seq.filter (String.IsNullOrEmpty >> not) |> hashSetIgnoreCase
        let workspaceRefs = project.GetCurrentMetadataReferences() |> Seq.map(fun ref -> ref.FilePath) |> hashSetIgnoreCase

        for ref in updatedRefs do
            if not(workspaceRefs.Contains(ref)) then
                projectContext.AddMetadataReference(ref, MetadataReferenceProperties.Assembly)
                updated <- true
        for ref in workspaceRefs do
            if not(updatedRefs.Contains(ref)) then
                projectContext.RemoveMetadataReference(ref)
                updated <- true
        
        // Synchronise project references
        let getProjectContextByName name =
            projectContexts.Values
            |> Seq.find (fun context -> context.ProjectFilePath.Equals(name, StringComparison.OrdinalIgnoreCase))
            
        let updatedProjects =
            ProjectSitesAndFiles.GetReferencedProjectSites (site, this.SystemServiceProvider)
            |> Seq.map (fun s -> s.ProjectFileName())
            |> hashSetIgnoreCase
        let workspaceProjects =
            project.GetCurrentProjectReferences()
            |> Seq.map(fun ref -> projectContexts.[ref.ProjectId].ProjectFilePath)
            |> hashSetIgnoreCase

        for projectName in updatedProjects do
            if not(workspaceProjects.Contains(projectName)) then
                projectContext.AddProjectReference(getProjectContextByName projectName, MetadataReferenceProperties())
                updated <- true
        for projectName in workspaceProjects do
            if not(updatedProjects.Contains(projectName)) then
                projectContext.RemoveProjectReference(getProjectContextByName projectName)
                updated <- true
            
        if updated then
            projectInfoManager.UpdateProjectInfo(project.Id, site, project.Workspace)

    member this.SetupProjectFile(siteProvider: IProvideProjectSite, workspace: VisualStudioWorkspaceImpl) =
        let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>()

        let rec setup (site: IProjectSite) =
            let projectGuid = Guid site.ProjectGuid
            let projectFileName = site.ProjectFileName()
            let projectDisplayName = projectDisplayNameOf projectFileName
            let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            if isNull (workspace.ProjectTracker.GetProject projectId) then
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
                        FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectGuid, hierarchy, null, errorReporter)

                let project = projectContext :?> AbstractProject
                projectContexts.[projectId] <- projectContext

                // Recursively create any referenced projects.
                // We need the project IDs in the projectContexts dictionary for SyncProject
                ProjectSitesAndFiles.GetReferencedProjectSites (site, this.SystemServiceProvider)
                |> Seq.iter setup
                
                // Syncronise the project with Roslyn and update the cached project info
                // if necessary.
                this.SyncProject(project, projectContext, site, forceUpdate=false)

                site.AdviseProjectSiteChanges(
                    FSharpConstants.FSharpLanguageServiceCallbackName, 
                    AdviseProjectSiteChanges (fun () ->
                        // Force an update so that the project options cache is blown away for
                        // this project
                        this.SyncProject(project, projectContext, site, forceUpdate=true)
                        )
                    )

                site.AdviseProjectSiteClosed(
                    FSharpConstants.FSharpLanguageServiceCallbackName, 
                    AdviseProjectSiteChanges (fun () -> 
                        projectInfoManager.ClearProjectInfo(project.Id)
                        project.Disconnect()
                        )
                    )

        setup (siteProvider.GetProjectSite())

    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =
    
        let loadTime = DateTime.Now
        let options = projectInfoManager.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace) |> Async.RunSynchronously
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName
        
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        projectInfoManager.AddSingleFileProject(projectId, (loadTime, options))

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
    
    member this.TrackDocument(fileName) =
        match VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable, fileName) with
        | Some (:? IProvideProjectSite as siteProvider, _) when not (IsScript fileName) ->
            this.SetupProjectFile(siteProvider, this.Workspace)
        | Some (hier, textLines) when IsScript fileName ->
            let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
            this.SetupStandAloneFile(fileName, fileContents, this.Workspace, hier)
        | _ -> ()
        