// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.IO

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

// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid(FSharpCommonConstants.svsSettingsPersistenceManagerGuidString)>]
type internal SVsSettingsPersistenceManager = class end

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
                            analyzerService.Reanalyze(workspace,documentIds=documentIds)
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
        if projectTable.ContainsKey(projectId) then
            Some(projectTable.[projectId])
        else
            None

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) = async { 
        let projectId = document.Project.Id
        
        // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
        // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
        // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
        if singleFileProjectTable.ContainsKey(projectId) then 
          try
            let loadTime,_ = singleFileProjectTable.[projectId]
            let fileName = document.FilePath
            let! cancellationToken = Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! options = this.ComputeSingleFileOptions (fileName, loadTime, sourceText.ToString(), document.Project.Solution.Workspace)
            singleFileProjectTable.[projectId] <- (loadTime, options)
            return Some options
          with ex -> 
            Assert.Exception(ex)
            return None
        else return this.TryGetOptionsForProject(projectId) 
     }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document: Document) = 
        let projectId = document.Project.Id
        if singleFileProjectTable.ContainsKey(projectId) then 
            let _loadTime, originalOptions = singleFileProjectTable.[projectId]
            Some originalOptions
        else 
            this.TryGetOptionsForProject(projectId) 

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
            let substituteLanguageName = if languageName = FSharpCommonConstants.FSharpLanguageName then "FSharp" else languageName
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
    [<Guid(FSharpCommonConstants.packageGuidString)>]
    [<ProvideLanguageService(languageService = typeof<FSharpLanguageService>,
                             strLanguageName = FSharpCommonConstants.FSharpLanguageName,
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
                             CodeSenseDelay = 100,
                             ShowDropDownOptions = true)>]
    internal FSharpPackage() =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()
    
    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

    override this.CreateLanguageService() = 
        FSharpLanguageService(this)        

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()
    
and 
    [<Guid(FSharpCommonConstants.languageServiceGuidString)>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fs")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsi")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsx")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsscript")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".ml")>]
    [<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".mli")>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".fs", 97)>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".fsi", 97)>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".fsx", 97)>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".fsscript", 97)>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".ml", 97)>]
    [<ProvideEditorExtension(FSharpCommonConstants.editorFactoryGuidString, ".mli", 97)>]
    internal FSharpLanguageService(package : FSharpPackage) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let checkerProvider = package.ComponentModel.DefaultExportProvider.GetExport<FSharpCheckerProvider>().Value
    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<ProjectInfoManager>().Value

    let projectDisplayNameOf projectFileName = 
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let openedProjects = Queue<IProvideProjectSite>()

    do
      Events.SolutionEvents.OnAfterOpenProject.Add (fun (args: Events.OpenProjectEventArgs) -> 
        match args.Hierarchy with
        | :? IProvideProjectSite as siteProvider ->
            openedProjects.Enqueue(siteProvider)            
        | _ -> ())
        
    member this.BatchSetupProjects() =    
        if openedProjects.Count > 0 then    
            let workspace = package.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
            for siteProvider in openedProjects do
                this.SetupProjectFile(siteProvider, workspace)
            openedProjects.Clear()
        
    /// Sync the information for the project 
    member this.SyncProject(project: AbstractProject, projectContext: IWorkspaceProjectContext, site: IProjectSite, forceUpdate) =

        let hashSetIgnoreCase x = new HashSet<string>(x, StringComparer.OrdinalIgnoreCase)
        let updatedFiles = site.SourceFilesOnDisk() |> hashSetIgnoreCase
        let workspaceFiles = project.GetCurrentDocuments() |> Seq.map(fun file -> file.FilePath) |> hashSetIgnoreCase

        // If syncing project upon some reference changes, we don't have a mechanism to recognize which references have been added/removed.
        // Hence, the current solution is to force update current project options.
        let mutable updated = forceUpdate
        for file in updatedFiles do
            if not(workspaceFiles.Contains(file)) then
                projectContext.AddSourceFile(file)
                updated <- true
        for file in workspaceFiles do
            if not(updatedFiles.Contains(file)) then
                projectContext.RemoveSourceFile(file)
                updated <- true

        // update the cached options
        if updated then
            projectInfoManager.UpdateProjectInfo(project.Id, site, project.Workspace)

    member this.SetupProjectFile(siteProvider: IProvideProjectSite, workspace: VisualStudioWorkspaceImpl) =
        let  rec setup (site: IProjectSite) =
            let projectGuid = Guid(site.ProjectGuid)
            let projectFileName = site.ProjectFileName()
            let projectDisplayName = projectDisplayNameOf projectFileName
            let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            if isNull (workspace.ProjectTracker.GetProject projectId) then
                projectInfoManager.UpdateProjectInfo(projectId, site, workspace)
                let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
                let errorReporter = ProjectExternalErrorReporter(projectId, "FS", this.SystemServiceProvider)
                
                let projectContext = 
                    projectContextFactory.CreateProjectContext(
                        FSharpCommonConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectGuid, siteProvider, null, errorReporter)

                let project = projectContext :?> AbstractProject

                this.SyncProject(project, projectContext, site, forceUpdate=false)
                site.AdviseProjectSiteChanges(FSharpCommonConstants.FSharpLanguageServiceCallbackName, 
                                              AdviseProjectSiteChanges(fun () -> this.SyncProject(project, projectContext, site, forceUpdate=true)))
                site.AdviseProjectSiteClosed(FSharpCommonConstants.FSharpLanguageServiceCallbackName, 
                                             AdviseProjectSiteChanges(fun () -> 
                                                projectInfoManager.ClearProjectInfo(project.Id)
                                                project.Disconnect()))
                for referencedSite in ProjectSitesAndFiles.GetReferencedProjectSites (site, this.SystemServiceProvider) do
                    let referencedProjectId = setup referencedSite                    
                    project.AddProjectReference(ProjectReference referencedProjectId)
            projectId
        setup (siteProvider.GetProjectSite()) |> ignore

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

            let projectContext = projectContextFactory.CreateProjectContext(FSharpCommonConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hier, null, errorReporter)
            projectContext.AddSourceFile(fileName)
            
            let project = projectContext :?> AbstractProject
            let documentId = project.GetCurrentDocumentFromPath(fileName).Id

            let rec onDocumentClosed = EventHandler<DocumentEventArgs> (fun _ args ->
                if args.Document.Id = documentId then
                    projectInfoManager.RemoveSingleFileProject(projectId)
                    project.Disconnect()
                    workspace.DocumentClosed.RemoveHandler(onDocumentClosed)
            )
            workspace.DocumentClosed.AddHandler(onDocumentClosed)

    override this.ContentTypeName = FSharpCommonConstants.FSharpContentTypeName
    override this.LanguageName = FSharpCommonConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSharpCommonConstants.languageServiceGuidString)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(textView) =
        base.SetupNewTextView(textView)
        let workspace = package.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
        let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()
        
        workspace.Options <- workspace.Options.WithChangedOption(Completion.CompletionOptions.BlockForCompletionItems, FSharpCommonConstants.FSharpLanguageName, false)
        workspace.Options <- workspace.Options.WithChangedOption(Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpCommonConstants.FSharpLanguageName, Nullable false)
               
        match textView.GetBuffer() with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines
            match VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable,filename) with
            | Some (hier, _) ->
                match hier with
                | :? IProvideProjectSite as siteProvider when not (IsScript(filename)) -> 
                    this.SetupProjectFile(siteProvider, workspace)
                | _ -> 
                    let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                    this.SetupStandAloneFile(filename, fileContents, workspace, hier)
            | _ -> ()
        | _ -> ()

        // This is the second half of the bridge between the F# IncrementalBuild analysis engine and the Roslyn analysis
        // engine.  When a document gets the focus, we call FSharpLanguageService.Checker.StartBackgroundCompile for the
        // project containing that file. This ensures the F# IncrementalBuild engine starts analyzing the project.
        let wpfTextView = textViewAdapter.GetWpfTextView(textView)
        match wpfTextView.TextBuffer.Properties.TryGetProperty<ITextDocument>(typeof<ITextDocument>) with
        | true, textDocument ->
            let filePath = textDocument.FilePath
            let onGotFocus = new EventHandler(fun _ _ ->
                for documentId in workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath) do 
                    let document = workspace.CurrentSolution.GetDocument(documentId)
                    match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) with 
                    | Some options -> checkerProvider.Checker.StartBackgroundCompile(options)
                    | None -> ()
            )
            let rec onViewClosed = new EventHandler(fun _ _ -> 
                wpfTextView.GotAggregateFocus.RemoveHandler(onGotFocus)
                wpfTextView.Closed.RemoveHandler(onViewClosed)
            )
            wpfTextView.GotAggregateFocus.AddHandler(onGotFocus)
            wpfTextView.Closed.AddHandler(onViewClosed)
        | _ -> ()

        // When the text view is opened, setup all remaining projects on the background.
        async {
            do this.BatchSetupProjects()
        }
        |> Async.StartImmediate
            