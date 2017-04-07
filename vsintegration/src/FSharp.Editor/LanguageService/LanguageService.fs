// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.IO
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Collections.Generic
open Microsoft.VisualStudio.Editor
open System.Collections.Concurrent



// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid (FSharpConstants.svsSettingsPersistenceManagerGuidString)>]
type internal SVsSettingsPersistenceManager = class end


// Exposes FSharpChecker as MEF export
[<Export (typeof<FSharpCheckerProvider>); Composition.Shared>]
type internal FSharpCheckerProvider [<ImportingConstructor>]
    (   analyzerService: IDiagnosticAnalyzerService
    ) =
    let checker = 
        lazy
            let checker = FSharpChecker.Create(projectCacheSize = 200, keepAllBackgroundResolutions = false)

            // This is one half of the bridge between the F# background builder and the Roslyn analysis engine.
            // When the F# background builder refreshes the background semantic build context for a file,
            // we request Roslyn to reanalyze that individual file.
            checker.BeforeBackgroundFileCheck.Add (fun (fileName, extraProjectInfo) ->  
               async {
                try match extraProjectInfo with 
                    | Some (:? Workspace as workspace) -> 
                        let solution = workspace.CurrentSolution
                        let documentIds = solution.GetDocumentIdsWithFilePath(fileName)
                        if not documentIds.IsEmpty then 
                            analyzerService.Reanalyze(workspace,documentIds=documentIds)
                    | _ -> ()
                with ex -> Assert.Exception(ex)
               } |> Async.StartImmediate
            )
            checker

    member this.Checker = checker.Value


/// Exposes project information as MEF component
[<Export(typeof<ProjectInfoManager>); Composition.Shared>]
type internal ProjectInfoManager [<ImportingConstructor>] (checkerProvider:FSharpCheckerProvider) =
    let projectCache = ProjectOptionsCache checkerProvider.Checker

    let getProjectOptionsForProjectSite ( fileName,loadTime,options:FSharpProjectOptions,  extraProjectInfo) = 
        {   ProjectFileName = options.ProjectFileName
            ProjectFileNames = options.ProjectFileNames
            OtherOptions = [||]
            ReferencedProjects = options.ReferencedProjects
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = SourceFile.MustBeSingleFileProject fileName
            LoadTime = loadTime
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo=extraProjectInfo 
        }  

    member __.Checker = checkerProvider.Checker
    member this.SingleFileProjectTable = projectCache.SingleFileProjectTable


    member __.GetProjectOptionsForProjectSite ( fileName, loadTime,options, extraProjectInfo) =
        getProjectOptionsForProjectSite ( fileName,loadTime,options,  extraProjectInfo)


    /// Get the exact options for a single-file script
    member self.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace: Workspace) = async {
        let solution = workspace.CurrentSolution
        let docId = solution.GetDocumentIdsWithFilePath fileName |> Seq.head
        let project = solution.GetProject docId.ProjectId

        let options = ProjectOptionsCache.toFSharpProjectOptions  projectCache.ProjectTable workspace project
        
        let extraProjectInfo = Some (box workspace)
        if SourceFile.MustBeSingleFileProject fileName then 
        //if isScriptFile fileName then 
            let! _options, _diagnostics = self.Checker.GetProjectOptionsFromScript (fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            //ProjectOptionsCache.fsxProjectOptions fileName (Some workspace)
            return getProjectOptionsForProjectSite ( fileName, loadTime,options,extraProjectInfo)
        else
            //ProjectOptionsCache.singleFileProjectOptions fileName
            return getProjectOptionsForProjectSite ( fileName, loadTime,options, extraProjectInfo)
    }

    /// Get the exact options for a document or project
    member self.TryGetOptionsForDocumentOrProject(document: Document) = asyncMaybe { 
        let projectId = document.Project.Id
        // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
        // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
        // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
        match tryGet projectId projectCache.SingleFileProjectTable with
        | Some (loadTime,_) ->
            let fileName = document.FilePath
            let! cancellationToken = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync cancellationToken
            let! options = self.ComputeSingleFileOptions( fileName, loadTime,sourceText.ToString(), document.Project.Solution.Workspace) |> liftAsync
            projectCache.SingleFileProjectTable.[projectId] <- (loadTime, options)
            return options
        | None ->
            match self.TryGetOptionsForProject projectId with
            | Some options -> return options
            | None -> return! None
    }

    member __.GetVSProjectReferences (hierarchy:IVsHierarchy) =
        hierarchy.TryGetProject () |> Option.map (fun proj ->
            proj.GetReferencedProjects () 
        ) |> Option.defaultValue []
    
    member self.GetProjectReferences (hierarchy:IVsHierarchy) =
        self.GetVSProjectReferences hierarchy |> List.map (fun proj ->
            (proj.GetProjectGuid >> ProjectId.CreateFromSerialized >> ProjectReference) ()
        )

    member self.GetProjectReferences (projectDTE:EnvDTE.Project) =
        projectDTE.GetReferencedProjects () |> List.map (fun proj ->
            (proj.GetProjectGuid >> ProjectId.CreateFromSerialized >> ProjectReference) ()
        )
    

    member this.AddSingleFileProject (projectId, timeStampAndOptions) =
        projectCache.AddSingleFileProject (projectId,timeStampAndOptions) 


    member this.AddProject (project:Project) =
        projectCache.AddProject project 


    member this.RemoveSingleFileProject(projectId) =
        projectCache.RemoveSingleFileProject projectId


    /// Clear a project from the project table
    member this.ClearProjectInfo(project: Project) =
        projectCache.RemoveProject project 


    /// Update the info for a project in the project table
    member this.UpdateProjectInfo (project: Project) =
        projectCache.UpdateProject project 
       

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
    member this.TryGetOptionsForProject(projectId: ProjectId) : FSharpProjectOptions option = 
        projectCache.TryGetOptions projectId


    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject (document: Document) = 
        projectCache.TryGetOptionsForEditingDocumentOrProject document
    
    member self.Clear () = projectCache.Clear ()


// Used to expose FSharpChecker/ProjectInfo manager to diagnostic providers
// Diagnostic providers can be executed in environment that does not use MEF so they can rely only
// on services exposed by the workspace
type internal FSharpCheckerWorkspaceService =
    inherit Microsoft.CodeAnalysis.Host.IWorkspaceService
    abstract Checker: FSharpChecker
    abstract ProjectInfoManager: ProjectInfoManager


type internal RoamingProfileStorageLocation (keyName: string) =
    inherit OptionStorageLocation ()
    
    member __.GetKeyNameForLanguage (languageName: string) =
        let unsubstitutedKeyName = keyName
        if isNull languageName then unsubstitutedKeyName else
        let substituteLanguageName = 
            if languageName = FSharpConstants.FSharpLanguageName then "FSharp" else languageName
        unsubstitutedKeyName.Replace ("%LANGUAGE%", substituteLanguageName)
 

[<Composition.Shared>]
[<Microsoft.CodeAnalysis.Host.Mef.ExportWorkspaceServiceFactory (typeof<FSharpCheckerWorkspaceService>, Microsoft.CodeAnalysis.Host.Mef.ServiceLayer.Default)>]
type internal FSharpCheckerWorkspaceServiceFactory [<Composition.ImportingConstructor>]
    (   checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_workspaceServices) =
            upcast { new FSharpCheckerWorkspaceService with
                member this.Checker = checkerProvider.Checker
                member this.ProjectInfoManager = projectInfoManager }


type
    [<Guid (FSharpConstants.packageGuidString)>]
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
                             CodeSenseDelay = 100,
                             ShowDropDownOptions = true)>]
    internal FSharpPackage () =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()

    override __.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override self.CreateWorkspace () = self.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

    override self.CreateLanguageService () = FSharpLanguageService(self)

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation _ = ()
    
and 
    [<Guid (FSharpConstants.languageServiceGuidString)>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fs")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsi")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsx")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsscript")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".ml")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".mli")>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fs", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsi", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsx", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsscript", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".ml", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".mli", 97)>]
    internal FSharpLanguageService (package:FSharpPackage) as self =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<ProjectInfoManager>().Value
    let solutionDTE = package.DTE.Solution

    let mutable projectTracker = None : VisualStudioProjectTracker option

    let projectDisplayNameOf projectFileName = 
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName
    
    let singleFileProjects = ConcurrentDictionary<ProjectId, AbstractProject>()


    let tryRemoveSingleFileProject projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.RemoveSingleFileProject(projectId)
            project.Disconnect()
        | _ -> ()
    
    let getProject (projectId:ProjectId) =
        self.Workspace.CurrentSolution.GetProject projectId


    member self.OnWorkspaceChanged (args:WorkspaceChangeEventArgs) =
        match args.Kind with
        | WorkspaceChangeKind.SolutionAdded ->
            args.NewSolution.Projects ?> Seq.iter projectInfoManager.AddProject
        | WorkspaceChangeKind.SolutionReloaded ->
            projectInfoManager.Clear ()
            args.NewSolution.Projects ?> Seq.iter projectInfoManager.AddProject
        | WorkspaceChangeKind.SolutionRemoved
        | WorkspaceChangeKind.SolutionCleared ->
            projectInfoManager.Clear ()
        | WorkspaceChangeKind.ProjectAdded ->
            args.ProjectId |> getProject ?> projectInfoManager.AddProject 
        | WorkspaceChangeKind.ProjectRemoved ->
            args.ProjectId |> getProject ?> projectInfoManager.ClearProjectInfo 
        | WorkspaceChangeKind.ProjectReloaded 
        | WorkspaceChangeKind.ProjectChanged ->
            args.ProjectId |> getProject ?> projectInfoManager.UpdateProjectInfo 
        | WorkspaceChangeKind.DocumentRemoved
        | WorkspaceChangeKind.DocumentAdded ->
            args.DocumentId.ProjectId |> getProject ?> projectInfoManager.UpdateProjectInfo 
        | _ -> ()
    

    member self.SyncProject (abstractProject: AbstractProject, projectContext: IWorkspaceProjectContext, projectDTE:EnvDTE.Project, forceUpdate) =
        let hashSetIgnoreCase x = new HashSet<string>(x, StringComparer.OrdinalIgnoreCase)
        let updatedFiles = projectDTE.GetFiles () |> hashSetIgnoreCase
        let workspaceFiles = abstractProject.GetCurrentDocuments () |> Seq.map(fun file -> file.FilePath) |> hashSetIgnoreCase

        // If syncing project upon some reference changes, we don't have a mechanism to recognize which references have been added/removed.
        // Hence, the current solution is to force update current project options.
        let mutable updated = forceUpdate
        for file in updatedFiles do
            if not (workspaceFiles.Contains file) then
                projectContext.AddSourceFile file
                updated <- true
        for file in workspaceFiles do
            if not (updatedFiles.Contains file) then
                projectContext.RemoveSourceFile file
                updated <- true
        // update the cached options
        if updated then projectInfoManager.UpdateProjectInfo <| getProject abstractProject.Id
    

    member self.SetupProject (projectDTE:EnvDTE.Project) =
        let rec setupProject (projectDTE:EnvDTE.Project) =
            let solutionVS = package.GetService<SVsSolution,IVsSolution>()
            let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>()
            let projectId = self.ProjectTracker.GetOrCreateProjectIdForPath (projectDTE.FullName, projectDTE.Name)
            match solutionVS.GetProjectOfUniqueName projectDTE.UniqueName with
            | VSConstants.S_OK, hierarchy ->
                if isNull (self.Workspace.ProjectTracker.GetProject projectId) then
                    let errorReporter = ProjectExternalErrorReporter (projectId, "FS", self.SystemServiceProvider)
                    let projectContext = projectContextFactory.CreateProjectContext (FSharpConstants.FSharpLanguageName, projectDTE.Name, projectDTE.FullName, projectId.Id, hierarchy, null, errorReporter)
                    let abstractProject = projectContext :?> AbstractProject
                    self.SyncProject(abstractProject,projectContext, projectDTE,false)
                    projectDTE.GetReferencedProjects ()
                    |> List.iter(fun (project:EnvDTE.Project) -> 
                        let projectId = setupProject project
                        abstractProject.AddProjectReference <| ProjectReference projectId
                    )
                    self.ProjectTracker.AddProject abstractProject
            | _ -> ()
            projectId
        setupProject projectDTE
    

    member self.SetupProjectHierarchy (hierarchy:IVsHierarchy) =
        match hierarchy.TryGetProject () with
        | None -> ()
        | Some projectDTE -> self.SetupProject projectDTE |> getProject |> projectInfoManager.AddProject


    member self.OnAfterLoadProject (args:Events.LoadProjectEventArgs) = self.SetupProjectHierarchy args.RealHierarchy


    /// Get the exact options for a single-file script
    member __.ComputeSingleFileOptions (fileName, loadTime, fileContents, hierarchy, workspace: Workspace) = async {
        let extraProjectInfo = Some (box workspace)
        if SourceFile.MustBeSingleFileProject fileName then 
        //if isScriptFile fileName then 
            let! _options, _diagnostics = projectInfoManager.Checker.GetProjectOptionsFromScript (fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            //ProjectOptionsCache.fsxProjectOptions fileName (Some workspace)
            return projectInfoManager.GetProjectOptionsForProjectSite ( fileName, loadTime,hierarchy,extraProjectInfo)
        else
            //ProjectOptionsCache.singleFileProjectOptions fileName
            return projectInfoManager.GetProjectOptionsForProjectSite ( fileName, loadTime,hierarchy, extraProjectInfo)
    }


    member self.ProjectTracker : VisualStudioProjectTracker =
        match projectTracker with
        | Some tracker -> tracker 
        | None -> 
            let tracker = self.Workspace.GetProjectTrackerAndInitializeIfNecessary Shell.ServiceProvider.GlobalProvider 
            projectTracker <- Some tracker
            tracker 


    override self.Initialize () =
        base.Initialize ()

        let projectTracker = (self.ProjectTracker: VisualStudioProjectTracker) 
        let solutionVS = package.GetService<SVsSolution,IVsSolution>()
        self.Workspace.AdviseSolutionEvents solutionVS
        
        self.Workspace.Options <- self.Workspace.Options.WithChangedOption (Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        self.Workspace.Options <- self.Workspace.Options.WithChangedOption (Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)

        self.Workspace.DocumentClosed.Add (fun args ->
           tryRemoveSingleFileProject args.Document.Project.Id 
        )
        
        self.Workspace.DocumentOpened.Add (fun args ->
            projectInfoManager.AddProject <| getProject args.Document.Project.Id
        )

        self.Workspace.WorkspaceChanged.Add self.OnWorkspaceChanged
        Events.SolutionEvents.OnAfterLoadProject.Add self.OnAfterLoadProject
        
        let setupProjects () = async {
            do! Async.AwaitEvent Events.SolutionEvents.OnAfterOpenSolution |> Async.Ignore
            use _ = Events.SolutionEvents.OnAfterOpenProject |> Observable.subscribe ( fun args ->
                self.SetupProjectHierarchy args.Hierarchy |> ignore
            )
            for project in solutionDTE.GetProjects () do 
                self.SetupProject project |> getProject |> projectInfoManager.AddProject
        }

        setupProjects () |> Async.StartImmediate

        projectTracker.StartPushingToWorkspaceAndNotifyOfOpenDocuments projectTracker.ImmutableProjects

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors ()


    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =

        let loadTime = DateTime.Now
        let options = projectInfoManager.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace) |> Async.RunSynchronously

        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName

        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        projectInfoManager.AddSingleFileProject (projectId, (loadTime, options))

        if isNull (workspace.ProjectTracker.GetProject projectId) then
            let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();
            let errorReporter = ProjectExternalErrorReporter (projectId, "FS", this.SystemServiceProvider)

            let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hier, null, errorReporter)
            projectContext.AddSourceFile fileName
            
            let project = projectContext :?> AbstractProject
            singleFileProjects.[projectId] <- project


    override __.ContentTypeName    = FSharpConstants.FSharpContentTypeName
    override __.LanguageName       = FSharpConstants.FSharpLanguageName
    override __.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    override __.LanguageServiceId  = new Guid (FSharpConstants.languageServiceGuidString)
    override __.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID ()

    override __.CreateContext (_,_,_,_,_) = raise (System.NotImplementedException ())

    override self.SetupNewTextView textView =
        base.SetupNewTextView textView
        let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()
        match textView.GetBuffer () with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines
            match VsRunningDocumentTable.FindDocumentWithoutLocking (package.RunningDocumentTable, filename) with
            | Some (hier, _) ->            
                hier.TryGetProject()
                |> Option.iter (fun _project ->
                    if not (isScriptFile filename) then 
                        self.SetupProjectHierarchy hier
                    else
                        let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                        self.SetupStandAloneFile (filename,fileContents, self.Workspace, hier))
            | None -> ()   
        | _ -> ()

