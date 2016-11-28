// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Linq
open System.IO

open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.DebuggerIntelliSense
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService

// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid(FSharpCommonConstants.svsSettingsPersistenceManagerGuidString)>]
type internal SVsSettingsPersistenceManager = class end

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
type internal FSharpLanguageService(package : FSharpPackage) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    // A table of information about projects, excluding single-file projects.  
    static let projectTable = ConcurrentDictionary<ProjectId, FSharpProjectOptions>()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    static let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpProjectOptions>()

    static let checker =  lazy FSharpChecker.Create()


    /// Update the info for a proejct in the project table
    static let UpdateProjectInfo(projectId: ProjectId, site: IProjectSite, workspace: Workspace) =
        let extraProjectInfo = Some(box workspace)
        let options = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site, site.ProjectFileName(), extraProjectInfo)
        checker.Value.InvalidateConfiguration(options)
        projectTable.[projectId] <- options

    /// Clear a project from the project table
    static let ClearProjectInfo(projectId: ProjectId) =
        projectTable.TryRemove(projectId) |> ignore
        
    /// Get the exact options for a single-file script
    static let ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace: Workspace) = async {
        let extraProjectInfo = Some(box workspace)
        if SourceFile.MustBeSingleFileProject(fileName) then 
            let! options = checker.Value.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, options)
            return ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site,fileName,options.ExtraProjectInfo)
        else
            let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
            return ProjectSitesAndFiles.GetProjectOptionsForProjectSite(site,fileName,extraProjectInfo)
      }

    /// Get the options for a project
    static member TryGetOptionsForProject(projectId: ProjectId) = 
        if projectTable.ContainsKey(projectId) then
            Some(projectTable.[projectId])
        else
            None

    /// Get the exact options for a document or project
    static member TryGetOptionsForDocumentOrProject(document: Document) = async { 
        let projectId = document.Project.Id
        
        // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
        // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
        // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
        if singleFileProjectTable.ContainsKey(projectId) then 
          try
            let loadTime,_ = singleFileProjectTable.[projectId]
            let fileName = document.FilePath
            let! cancellationToken = Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! options = ComputeSingleFileOptions (fileName, loadTime, sourceText.ToString(), document.Project.Solution.Workspace)
            singleFileProjectTable.[projectId] <- (loadTime, options)
            return Some options
          with ex -> 
            Assert.Exception(ex)
            return None
        else return FSharpLanguageService.TryGetOptionsForProject(projectId) 
     }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    static member TryGetOptionsForEditingDocumentOrProject(document: Document) = 
        let projectId = document.Project.Id
        if singleFileProjectTable.ContainsKey(projectId) then 
            let _loadTime, originalOptions = singleFileProjectTable.[projectId]
            Some originalOptions
        else 
            FSharpLanguageService.TryGetOptionsForProject(projectId) 

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    static member GetCompilationDefinesForEditingDocument(document: Document) = 
        let projectOptionsOpt = FSharpLanguageService.TryGetOptionsForProject(document.Project.Id)  
        let otherOptions = 
            match projectOptionsOpt with 
            | None -> []
            | Some options -> options.OtherOptions |> Array.toList
        CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, otherOptions)

    /// Get the global FSharpChecker component
    static member Checker with get() = checker.Value

    /// Sync the information for the project 
    member this.SyncProject(project: AbstractProject, projectContext: IWorkspaceProjectContext, site: IProjectSite) =

        let hashSetIgnoreCase x = new HashSet<string>(x, StringComparer.OrdinalIgnoreCase)
        let updatedFiles = site.SourceFilesOnDisk() |> hashSetIgnoreCase
        let workspaceFiles = project.GetCurrentDocuments() |> Seq.map(fun file -> file.FilePath) |> hashSetIgnoreCase

        let mutable updated = false
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
            UpdateProjectInfo(project.Id, site, project.Workspace)

    member this.SetupProjectFile(siteProvider: IProvideProjectSite, workspace: VisualStudioWorkspaceImpl) =
        let site = siteProvider.GetProjectSite()
        let projectGuid = Guid(site.ProjectGuid)
        let projectFileName = site.ProjectFileName()
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectFileName)

        UpdateProjectInfo(projectId, site, workspace)

        match workspace.ProjectTracker.GetProject(projectId) with
        | null ->
            let projectContextFactory = this.Package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
            let errorReporter = ProjectExternalErrorReporter(projectId, "FS", this.SystemServiceProvider)

            let projectContext = projectContextFactory.CreateProjectContext(FSharpCommonConstants.FSharpLanguageName, projectFileName, projectFileName, projectGuid, siteProvider, null, errorReporter)
            let project = projectContext :?> AbstractProject

            this.SyncProject(project, projectContext, site)
            site.AdviseProjectSiteChanges(FSharpCommonConstants.FSharpLanguageServiceCallbackName, AdviseProjectSiteChanges(fun () -> this.SyncProject(project, projectContext, site)))
            site.AdviseProjectSiteClosed(FSharpCommonConstants.FSharpLanguageServiceCallbackName, AdviseProjectSiteChanges(fun () -> ClearProjectInfo(project.Id); project.Disconnect()))
        | _ -> ()

    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =

        let loadTime = DateTime.Now
        let options = ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace) |> Async.RunSynchronously

        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(options.ProjectFileName, options.ProjectFileName)
        singleFileProjectTable.[projectId] <- (loadTime, options)

        if obj.ReferenceEquals(workspace.ProjectTracker.GetProject(projectId), null) then
            let projectContextFactory = this.Package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
            let errorReporter = ProjectExternalErrorReporter(projectId, "FS", this.SystemServiceProvider)

            let projectContext = projectContextFactory.CreateProjectContext(FSharpCommonConstants.FSharpLanguageName, options.ProjectFileName, options.ProjectFileName, projectId.Id, hier, null, errorReporter)
            projectContext.AddSourceFile(fileName)
            
            let project = projectContext :?> AbstractProject
            let document = project.GetCurrentDocumentFromPath(fileName)

            document.Closing.Add(fun _ ->  
                singleFileProjectTable.TryRemove(projectId) |> ignore; 
                project.Disconnect())

    override this.ContentTypeName = FSharpCommonConstants.FSharpContentTypeName
    override this.LanguageName = FSharpCommonConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSharpCommonConstants.languageServiceGuidString)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(textView) =
        base.SetupNewTextView(textView)
        let workspace = this.Package.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

        // FSROSLYNTODO: Hide navigation bars for now. Enable after adding tests
        workspace.Options <- workspace.Options.WithChangedOption(NavigationBarOptions.ShowNavigationBar, FSharpCommonConstants.FSharpLanguageName, false)
        
        match textView.GetBuffer() with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines
            match VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable,filename) with
            | Some (hier, _) ->
                match hier with
                | :? IProvideProjectSite as siteProvider when not (IsScript(filename)) -> 
                    this.SetupProjectFile(siteProvider, workspace)
                | _ -> 
                    let editorAdapterFactoryService = this.Package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()
                    let fileContents = VsTextLines.GetFileContents(textLines, editorAdapterFactoryService)
                    this.SetupStandAloneFile(filename, fileContents, workspace, hier)
            | _ -> ()
        | _ -> ()


and [<Guid(FSharpCommonConstants.packageGuidString)>]
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
                             DefaultToInsertSpaces  = true,
                             CodeSense = true,
                             DefaultToNonHotURLs = true,
                             EnableCommenting = true,
                             CodeSenseDelay = 100)>]
        internal FSharpPackage() =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()

    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

    override this.CreateLanguageService() = new FSharpLanguageService(this)

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()
